using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.Structures;
using System.Xml;
using System.IO;
using FireTerminator.Common.Services;
using FireTerminator.Common;
using FireTerminator.Server.Services;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FireTerminator.Server
{
    public class UserManager
    {
        public string DataFile
        {
            get { return System.Windows.Forms.Application.StartupPath + "\\FireTerminator.Server.users.xml"; }
        }
        public long NewRegisterID
        {
            get
            {
                long id = Program.Option.UserAccountBeginID;
                while (GetRegisteredUserInfo(id) != null)
                    ++id;
                return id;
            }
        }
        public int NewTaskGroupID
        {
            get
            {
                int id = 1;
                while (GetTaskGroup(id) != null)
                    ++id;
                return id;
            }
        }
        public float ProcessIntervalTime
        {
            get { return 0.05F; }
        }
        public ViewportUserOperations UserOperations
        {
            get { return m_Operations; }
        }
        public long[] MonitoringUserIDs
        {
            get
            {
                List<long> ids = new List<long>();
                foreach (var g in TaskGroups.Values.ToArray())
                {
                    foreach (var u in g.Users.Values.ToArray())
                    {
                        if (u.AdjudicatorMode)
                            ids.Add(u.AccountID);
                    }
                }
                return ids.ToArray();
            }
        }

        public class ProjectReferrencerReq
        {
            public ProjectReferrencerReq(ProjectReferrencer user, RequestType type)
            {
                User = user;
                ReqType = type;
            }
            public enum RequestType
            {
                ChangeTask,
                StartTask,
                StopTask
            }
            public ProjectReferrencer User = null;
            public RequestType ReqType = RequestType.ChangeTask;
        }

        public void Load()
        {
            RegisteredUsers.Clear();
            TaskGroups.Clear();
            var defGroup = CreateTaskGroup(0);
            defGroup.Name = "未分组";
            if (File.Exists(DataFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(DataFile);
                XmlElement nodeUsers = doc.DocumentElement.GetElementsByTagName("Users")[0] as XmlElement;
                foreach (XmlElement elm in nodeUsers.ChildNodes)
                {
                    RegisteredUserInfo user = new RegisteredUserInfo(elm);
                    RegisteredUsers[user.AccountID] = user;
                    var group = CreateTaskGroup(user.TaskGroupID);
                    if (group == null)
                        group = defGroup;
                    var suser = new ServerLoginUserInfo(user, LoginStatus.离线, null);
                    group.AddUser(suser.LoginInfo);
                }
                XmlElement nodeGrps = doc.DocumentElement.GetElementsByTagName("Groups")[0] as XmlElement;
                foreach (XmlElement elm in nodeGrps.ChildNodes)
                {
                    int groupId = int.Parse(elm.GetAttribute("ID"));
                    var grp = CreateTaskGroup(groupId);
                    if (grp != null)
                    {
                        grp.Name = elm.GetAttribute("Name");
                    }
                }
            }
        }

        public void Save()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<FireTerminatorUsers></FireTerminatorUsers>");
            var nodeUsers = doc.CreateElement("Users");
            foreach (var user in RegisteredUsers.Values)
            {
                var elm = doc.CreateElement("User");
                user.SaveXml(elm);
                nodeUsers.AppendChild(elm);
            }
            doc.DocumentElement.AppendChild(nodeUsers);
            var nodeGrps = doc.CreateElement("Groups");
            foreach (var grp in TaskGroups.Values)
            {
                var elm = doc.CreateElement("Group");
                elm.SetAttribute("ID", grp.GroupID.ToString());
                elm.SetAttribute("Name", grp.Name);
                nodeGrps.AppendChild(elm);
            }
            doc.DocumentElement.AppendChild(nodeGrps);
            doc.Save(DataFile);
        }

        public UserTaskGroup GetTaskGroup(int groupId)
        {
            UserTaskGroup group = null;
            TaskGroups.TryGetValue(groupId, out group);
            return group;
        }

        public UserTaskGroup GetTaskGroup(string groupName)
        {
            foreach (var grp in TaskGroups.Values)
            {
                if (grp.Name == groupName)
                    return grp;
            }
            return null;
        }

        public UserTaskGroup CreateTaskGroup(string name)
        {
            var grp = CreateTaskGroup(NewTaskGroupID);
            if (grp != null)
                grp.Name = name;
            return grp;
        }

        public UserTaskGroup CreateTaskGroup(int groupId)
        {
            UserTaskGroup group = null;
            if (groupId >= 0)
            {
                group = GetTaskGroup(groupId);
                if (group == null)
                {
                    group = new UserTaskGroup(groupId);
                    TaskGroups[groupId] = group;
                }
            }
            return group;
        }

        public bool RemoveTaskGroup(int groupId)
        {
            if (groupId <= 0)
                return false;
            if (TaskGroups.ContainsKey(groupId))
            {
                var grp = TaskGroups[groupId];
                var users = grp.Users.Values.ToArray();
                List<long> ids = new List<long>();
                foreach (var user in users)
                {
                    if (TaskGroups[0].AddUser(user))
                        ids.Add(user.AccountID);
                }
                grp.Users.Clear();
                TaskGroups.Remove(groupId);
                PushNotification(new UserNotification_SetTaskGroup(ids.ToArray(), 0));
                PushNotification(new UserNotification_SyncTaskGroupListToMonitorUsers());
                return true;
            }
            return false;
        }

        public bool SetUserTaskGroup(long userId, int groupId)
        {
            var user = GetLoginUserInfo(userId);
            if (user == null)
                return false;
            int groupIdSrc = user.LoginInfo.ParentGroup.GroupID;
            if (groupIdSrc == groupId)
                return true;
            var defGroup = GetTaskGroup(0);
            var srcGroup = GetTaskGroup(groupIdSrc);
            var group = GetTaskGroup(groupId);
            if (group == null)
                return false;
            if (srcGroup.RemoveUser(userId))
            {
                if (!group.AddUser(user.LoginInfo))
                {
                    if (!srcGroup.AddUser(user.LoginInfo))
                        defGroup.AddUser(user.LoginInfo);
                }
                else
                {
                    // 通知原组其他组员删除userId
                    PushNotification(new UserNotification_AddOrRemoveTaskGroupUser(groupIdSrc, userId, false));
                    // 通知userId新的分组
                    PushNotification(new UserNotification_SetTaskGroup(userId, groupId));
                    // 通知userId新分组的组员userId的加入
                    PushNotification(new UserNotification_AddOrRemoveTaskGroupUser(groupId, userId, true));
                    // 通知所有监控用户分组信息
                    PushNotification(new UserNotification_SyncTaskGroupListToMonitorUsers());
                    return true;
                }
            }
            return false;
        }

        public RegisteredUserInfo GetRegisteredUserInfo(string account)
        {
            if (!String.IsNullOrEmpty(account))
            {
                if (LoginInfo.IsNumberAccount(account))
                    return GetRegisteredUserInfo(long.Parse(account));
                foreach (var info in RegisteredUsers.Values)
                {
                    if (info.Name == account)
                        return info;
                }
            }
            return null;
        }

        public RegisteredUserInfo GetRegisteredUserInfo(long accountId)
        {
            RegisteredUserInfo info = null;
            RegisteredUsers.TryGetValue(accountId, out info);
            return info;
        }

        public RegisteredUserInfo RegisterUser(LoginInfo info, OperationContext context)
        {
            if (!info.IsAccountNameValid)
                return null;
            var user = new RegisteredUserInfo(NewRegisterID, info.AccountName, info.Password);
            RegisteredUsers[user.AccountID] = user;
            var suser = new ServerLoginUserInfo(user, LoginStatus.登录中, context);
            TaskGroups[0].AddUser(suser.LoginInfo);
            return user;
        }

        public bool UnRegisterUser(LoginUserInfo info)
        {
            if (RegisteredUsers.ContainsKey(info.AccountID))
            {
                if (info.Status >= LoginStatus.在线)
                    KickUser(info.AccountID, 0, LogoutReason.系统踢出, null);
                if (info.ParentGroup != null)
                    info.ParentGroup.RemoveUser(info.AccountID);
                return RegisteredUsers.Remove(info.AccountID);
            }
            return false;
        }

        public void StartUserKickingDetectingThread()
        {
#if !DEBUG
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                List<ServerLoginUserInfo> lstKicks = new List<ServerLoginUserInfo>();
                while (Program.IsAppRunning)
                {
                    lstKicks.Clear();
                    foreach (var grp in TaskGroups.Values)
                    {
                        lstKicks.AddRange(from user in grp.OnlineUsers
                                          let suser = user.LocalObj as ServerLoginUserInfo
                                          where suser.IsBeatingHeartTimeout
                                          select suser);
                    }
                    foreach (var user in lstKicks)
                    {
                        KickUser(user.AccountID, 10, LogoutReason.超时踢出, null);
                    }
                    Thread.Sleep(2000);
                }
            }));
#endif
        }

        public ServerLoginUserInfo RegisterService(OperationContext context, int linkPort)
        {
            var imp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ipport1 = String.Format("{0}:{1}", imp.Address, linkPort);
            ServerLoginUserInfo user = GetLoginUserInfo(ipport1);
            if (user != null)
            {
                string ipport2 = String.Format("{0}:{1}", imp.Address, imp.Port);
                OnlineSessions[ipport2] = user.AccountID;
                user.RegisterOperationContext(context);
                return user;
            }
            return null;
        }

        public ServerLoginUserInfo GetLoginUserInfo(OperationContext context)
        {
            var imp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            ServerLoginUserInfo user = GetLoginUserInfo(String.Format("{0}:{1}", imp.Address, imp.Port));
            if (user != null)
            {
                user.RegisterOperationContext(context);
                return user;
            }
            return null;
        }

        public ServerLoginUserInfo GetLoginUserInfo(string ipport)
        {
            long accId = -1;
            if (OnlineSessions.TryGetValue(ipport, out accId))
            {
                ServerLoginUserInfo suser = null;
                OnlineUsers.TryGetValue(accId, out suser);
                return suser;
            }
            return null;
        }

        public ServerLoginUserInfo GetLoginUserInfo(IContextChannel channel)
        {
            foreach (var user in OnlineUsers.Values.ToArray())
            {
                if (user.Channel == channel)
                    return user;
            }
            return null;
        }

        public ServerLoginUserInfo GetLoginUserInfo(long accountId)
        {
            var user = GetRegisteredUserInfo(accountId);
            if (user != null)
            {
                var group = GetTaskGroup(user.TaskGroupID);
                if (group != null)
                {
                    var luser = group.GetUser(accountId);
                    if (luser != null)
                        return luser.LocalObj as ServerLoginUserInfo;
                }
            }
            return null;
        }

        public bool KickUser(long accountId, int timeOut, LogoutReason reason, AutoResetEvent evt)
        {
            ServerLoginUserInfo user = GetLoginUserInfo(accountId);
            if (user == null)
                return false;
            try
            {
                PushNotification(new UserNotification_ChangeUserStatus(user.LoginInfo.Info.TaskGroupID, user.AccountID, LoginStatus.离线));
                if (ProjectDoc.Instance.SelectedViewportInfo != null)
                    ProjectDoc.Instance.SelectedViewportInfo.RemoveAllCreatorElements(user.AccountID);
                if (user.LoginInfo.AdjudicatorMode)
                    RemoveMonitorUserFromAllReferrencers(user.AccountID);
                OnlineUsers.Remove(user.AccountID);
                OnlineSessions.Remove(user.LinkIPPortCaption);
                OnlineSessions.Remove(user.ServiceIPPortCaption);
                if (user.Channel.State != CommunicationState.Closed)
                    CommonMethods.CloseChannel(user.Channel, timeOut, evt);
                user.Channel.Closed -= new EventHandler(Channel_Closed);
                user.Context = null;
                if (UserLogouted != null)
                    UserLogouted(accountId, reason);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public void KickAllUsers(LogoutReason reason)
        {
            List<AutoResetEvent> evts = new List<AutoResetEvent>();
            foreach (var user in OnlineUsers.Values.ToArray())
            {
                AutoResetEvent evt = new AutoResetEvent(false);
                KickUser(user.AccountID, 30, reason, evt);
                evts.Add(evt);
            }
            WaitHandle.WaitAll(evts.ToArray());
        }

        private void Channel_Closed(object sender, EventArgs e)
        {
            var user = GetLoginUserInfo(sender as IContextChannel);
            if (user != null)
                KickUser(user.AccountID, 10, LogoutReason.主动退出, null);
        }

        public void OnUserLoginSucceeded(long userId, OperationContext context)
        {
            var suser = Program.UserMgr.GetLoginUserInfo(userId);
            suser.Context = context;
            suser.Channel.Closed += new EventHandler(Channel_Closed);
            OnlineSessions[suser.LinkIPPortCaption] = suser.AccountID;
            OnlineUsers[suser.AccountID] = suser;
            if (UserLogined != null)
                UserLogined(suser.AccountID);
            PushNotification(new UserNotification_SetTaskGroup(userId, suser.LoginInfo.Info.TaskGroupID));
        }

        public void TickNotifications(float elapsedTime)
        {
            m_ProcessElapsedTime += elapsedTime;
            if (m_ProcessElapsedTime > ProcessIntervalTime)
            {
                m_ProcessElapsedTime = 0;
                lock (m_Notifies)
                {
                    while (m_Notifies.Count > 0)
                    {
                        var notify = m_Notifies.Dequeue();
                        try
                        {
                            notify.Notify();
                        }
                        catch (System.Exception ex)
                        {
                        }
                    }
                }
            }
        }

        public void PushNotification(UserNotification notify)
        {
            lock (m_Notifies)
            {
                m_Notifies.Enqueue(notify);
            }
        }

        public void PushTaskChangeRequester(ProjectReferrencerReq user)
        {
            lock (m_TaskRequester)
            {
                m_TaskRequester.Enqueue(user);
            }
        }

        public ProjectReferrencerReq PopTaskChangeRequester()
        {
            ProjectReferrencerReq req = null;
            if (m_TaskRequester.Count > 0)
            {
                lock (m_TaskRequester)
                    req = m_TaskRequester.Dequeue();
            }
            return req;
        }

        public void MonitorGroupUsers(ProjectReferrencer pr)
        {
            if (pr == null)
                return;
            if (!pr.IsStarted)// || !group.IsCooperation)
            {
                foreach (var id in pr.UserIDs)
                {
                    var suser = Program.UserMgr.GetLoginUserInfo(id);
                    if (suser != null && suser.Status >= LoginStatus.在线 && suser.Feedback != null)
                        suser.Feedback.TerminateMonitor();
                }
            }
            else
            {
                Dictionary<long, ServerLoginUserInfo> newUsers = new Dictionary<long, ServerLoginUserInfo>();
                foreach (var id in pr.UserIDs)
                {
                    var suser = Program.UserMgr.GetLoginUserInfo(id);
                    if (suser.Status >= LoginStatus.在线)
                        newUsers[suser.AccountID] = suser;
                }
                List<long> ids = new List<long>();
                foreach (var u in ServerMonitoringUsers.Values)
                {
                    if ((u.LoginInfo == null || u.LoginInfo == pr || u.LoginInfo.ParentGroup == pr) && !newUsers.ContainsKey(u.AccountID))
                    {
                        if (ProjectDoc.Instance.SelectedViewportInfo != null)
                            ProjectDoc.Instance.SelectedViewportInfo.RemoveAllCreatorElements(u.AccountID);
                        u.Feedback.TerminateMonitor();
                        ids.Add(u.AccountID);
                    }
                }
                foreach (var suser in newUsers.Values)
                {
                    if (!ServerMonitoringUsers.ContainsKey(suser.AccountID))
                    {
                        ServerMonitoringUsers[suser.AccountID] = suser;
                        suser.Feedback.StartMonitor();
                    }
                }
                foreach (long id in ids)
                {
                    ServerMonitoringUsers.Remove(id);
                }
            }
        }
        public void RemoveMonitorUserFromAllReferrencers(long monitorUserId)
        {
            foreach (var grp in TaskGroups.Values.ToArray())
            {
                grp.BeMonitoringUsers.Remove(monitorUserId);
                foreach (var user in grp.Users.Values.ToArray())
                {
                    user.BeMonitoringUsers.Remove(monitorUserId);
                }
            }
        }

        public delegate void Delegate_LoginUser(long id);
        public delegate void Delegate_LogoutUser(long id, LogoutReason reason);
        public event Delegate_LoginUser UserLogined;
        public event Delegate_LogoutUser UserLogouted;
        public ManualResetEvent ClosingChannelExecutingEvt = new ManualResetEvent(false);

        public Dictionary<long, RegisteredUserInfo> RegisteredUsers = new Dictionary<long, RegisteredUserInfo>();
        public Dictionary<long, ServerLoginUserInfo> OnlineUsers = new Dictionary<long, ServerLoginUserInfo>();
        public Dictionary<string, long> OnlineSessions = new Dictionary<string, long>();
        public Dictionary<int, UserTaskGroup> TaskGroups = new Dictionary<int, UserTaskGroup>();
        public Dictionary<long, ServerLoginUserInfo> ServerMonitoringUsers = new Dictionary<long, ServerLoginUserInfo>();
        private float m_ProcessElapsedTime = 0;
        private Queue<UserNotification> m_Notifies = new Queue<UserNotification>();
        private ViewportUserOperations m_Operations = new ViewportUserOperations();
        private Queue<ProjectReferrencerReq> m_TaskRequester = new Queue<ProjectReferrencerReq>();
    }
}
