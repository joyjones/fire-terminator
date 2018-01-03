using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using FireTerminator.Common;
using FireTerminator.Common.Services;
using FireTerminator.Common.Structures;
using DevExpress.XtraTreeList.Nodes;

namespace FireTerminator.Server.Services
{
    public class ServerLoginUserInfo
    {
        //static ServerLoginUserInfo()
        //{
        //    LoginUserInfo.DataContractKnownTypes.Add(typeof(ServerLoginUserInfo));
        //}
        public ServerLoginUserInfo(RegisteredUserInfo user, LoginStatus status, OperationContext context)
        {
            LoginInfo = new LoginUserInfo(user, 0, status);
            LoginInfo.LocalObj = this;
            Context = context;
        }
        public static float BeatingHeartTimeoutSeconds = 30.0F;
        public OperationContext Context
        {
            set
            {
                if (value == null)
                {
                    Channel = null;
                    LinkEndPoint = null;
                    LoginInfo.Info.LinkPort = 0;
                    Feedback = null;
                    FeedbackLogin = null;
                    ServicePort = 0;
                    LastBeatingHeartTick = 0;
                    LoginInfo.Status = LoginStatus.离线;
                }
                else
                {
                    Channel = value.Channel;
                    LinkEndPoint = value.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    LoginInfo.Info.LinkPort = LinkEndPoint.Port;
                    RegisterOperationContext(value);
                    LastBeatingHeartTick = DateTime.Now.Ticks;
                }
            }
        }
        public LoginUserInfo LoginInfo
        {
            get;
            private set;
        }
        public RegisteredUserInfo Info
        {
            get { return LoginInfo.Info; }
        }
        public long AccountID
        {
            get { return LoginInfo.AccountID; }
        }
        public string Name
        {
            get { return LoginInfo.Name; }
        }
        public LoginStatus Status
        {
            get { return LoginInfo.Status; }
            set { LoginInfo.Status = value; }
        }
        public bool IsOnline
        {
            get { return Status >= LoginStatus.在线 && Feedback != null; }
        }
        public RemoteEndpointMessageProperty LinkEndPoint
        {
            get;
            private set;
        }
        public int ServicePort
        {
            get;
            private set;
        }
        public IContextChannel Channel
        {
            get;
            private set;
        }
        public ILoginServiceCallback FeedbackLogin
        {
            get;
            private set;
        }
        public IServiceCallback Feedback
        {
            get;
            private set;
        }
        public long LastBeatingHeartTick
        {
            get;
            set;
        }
        public bool IsBeatingHeartTimeout
        {
            get
            {
                var ts = new TimeSpan(DateTime.Now.Ticks - LastBeatingHeartTick);
                return ts.TotalMilliseconds > BeatingHeartTimeoutSeconds * 1000;
            }
        }
        public string LinkIPPortCaption
        {
            get
            {
                if (LinkEndPoint == null)
                    return "";
                return String.Format("{0}:{1}", LinkEndPoint.Address, LinkEndPoint.Port);
            }
        }
        public string ServiceIPPortCaption
        {
            get
            {
                if (LinkEndPoint == null)
                    return "";
                return String.Format("{0}:{1}", LinkEndPoint.Address, ServicePort);
            }
        }

        public void RegisterOperationContext(OperationContext context)
        {
            if (context == null)
                return;
            string ContractName = context.EndpointDispatcher.ContractName;
            if (ContractName == typeof(ILoginService).Name)
            {
                FeedbackLogin = context.GetCallbackChannel<ILoginServiceCallback>();
                if (FeedbackLogin != null && Status == LoginStatus.离线)
                    Status = LoginStatus.登录中;
            }
            else if (ContractName == typeof(IMainService).Name)
            {
                Feedback = context.GetCallbackChannel<IServiceCallback>();
                var imp = context.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                ServicePort = imp.Port;
                if (Feedback != null && Status == LoginStatus.登录中)
                {
                    Status = LoginStatus.在线;
                    Program.UserMgr.PushNotification(new UserNotification_SetTaskGroup(AccountID, LoginInfo.Info.TaskGroupID));
                    Program.UserMgr.PushNotification(new UserNotification_ChangeUserStatus(LoginInfo.Info.TaskGroupID, AccountID, Status));
                    if (LoginInfo.AdjudicatorMode)
                    {
                        Program.UserMgr.PushNotification(new UserNotification_SyncTaskGroupList(AccountID));
                        if (JudgementSettings.ConfigFileMD5 != LoginInfo.JudgementConfigFileMD5)
                        {
                            Program.UserMgr.PushNotification(new UserNotification_SyncJudgementContext(AccountID));
                        }
                    }
                    else
                    {
                        Program.UserMgr.PushNotification(new UserNotification_SyncTaskGroupListToMonitorUsers());
                    }
                    if (LoginInfo.ProjectDescription != null)
                    {
                        Program.UserMgr.PushNotification(new UserNotification_SetTrainingProject(AccountID, false, LoginInfo.ProjectDescription.FileName));
                        Program.UserMgr.PushNotification(new UserNotification_SetTrainingMission(LoginInfo));
                    }
                }
            }
        }
        public void CheckStartMission()
        {
            if (LoginInfo.IsStarted)
            {
                Program.UserMgr.PushNotification(new UserNotification_StartMission(LoginInfo, true));
                if (LoginInfo.IsPaused)
                    Program.UserMgr.PushNotification(new UserNotification_PauseMission(LoginInfo, true));
                if (LoginInfo.ParentGroup.IsCooperation)
                    Program.UserMgr.MonitorGroupUsers(LoginInfo.ParentGroup);
            }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
