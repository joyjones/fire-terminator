using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FireTerminator.Common.Services;
using FireTerminator.Common;
using System.Drawing;
using FireTerminator.Server.Services;
using FireTerminator.Common.Structures;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.Elements;

namespace FireTerminator.Server
{
    public abstract class UserNotification
    {
        public UserNotification(long[] ids)
        {
            UserIDs = ids;
        }
        public long[] UserIDs = null;
        public long SendingUserID = -1;
        public long SpecificTargetUserID = -1;
        public abstract void Notify();
        public IServiceCallback[] UserFeedBacks
        {
            get
            {
                if (UserIDs == null)
                    return new IServiceCallback[] { };
                List<IServiceCallback> rst = new List<IServiceCallback>();
                foreach (long id in UserIDs)
                {
                    if ((SpecificTargetUserID > 0 && id == SpecificTargetUserID) || (SpecificTargetUserID <= 0))
                    {
                        var u = Program.UserMgr.GetLoginUserInfo(id);
                        if (u != null && u.Feedback != null && u.Status >= LoginStatus.在线)
                        {
                            if (id != SendingUserID)
                                rst.Add(u.Feedback);
                            if (!u.LoginInfo.AdjudicatorMode)
                            {
                                // 同时发送给监控用户
                                long[] muserIds;
                                if (u.LoginInfo.ParentGroup.IsCooperation)
                                    muserIds = u.LoginInfo.ParentGroup.BeMonitoringUserIDs;
                                else
                                    muserIds = u.LoginInfo.BeMonitoringUserIDs;
                                foreach (var mid in muserIds)
                                {
                                    var mu = Program.UserMgr.GetLoginUserInfo(mid);
                                    if (mu != null && mu.Feedback != null && mu.Status >= LoginStatus.在线)
                                    {
                                        rst.Add(mu.Feedback);
                                    }
                                }
                            }
                        }
                    }
                }
                return rst.ToArray();
            }
        }
    }

    public class UserNotification_SetTaskGroup : UserNotification
    {
        public UserNotification_SetTaskGroup(long[] ids, int groupId)
            : base(ids)
        {
            TaskGroupID = groupId;
        }
        public UserNotification_SetTaskGroup(long id, int groupId)
            : this(new long[] { id }, groupId)
        {
        }
        public int TaskGroupID = 0;

        public override void Notify()
        {
            var group = Program.UserMgr.GetTaskGroup(TaskGroupID);
            foreach (var fb in UserFeedBacks)
            {
                fb.SetCurrentTaskGroup(group);
            }
        }
    }

    public class UserNotification_SyncTaskGroupList : UserNotification
    {
        public UserNotification_SyncTaskGroupList(long userId)
            : base(new long[] { userId })
        {
        }
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SyncUserTaskGroups(Program.UserMgr.TaskGroups.Values.ToArray());
            }
        }
    }

    public class UserNotification_SyncJudgementContext : UserNotification
    {
        public UserNotification_SyncJudgementContext(long userId)
            : base(new long[] { userId })
        {
        }
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SyncJudgementConfigContext(JudgementSettings.ConfigFileContext);
            }
        }
    }

    public class UserNotification_SyncTaskGroupListToMonitorUsers : UserNotification
    {
        public UserNotification_SyncTaskGroupListToMonitorUsers()
            : base(Program.UserMgr.MonitoringUserIDs)
        {
        }
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SyncUserTaskGroups(Program.UserMgr.TaskGroups.Values.ToArray());
            }
        }
    }

    public class UserNotification_SetTrainingProject : UserNotification_SetTaskGroup
    {
        public UserNotification_SetTrainingProject(long id, bool isGroupId, string projFile)
            : base(null, isGroupId ? (int)id : -1)
        {
            while (projFile.EndsWith(ProjectInfo.DefaultFileExt))
                projFile = projFile.Substring(0, projFile.Length - ProjectInfo.DefaultFileExt.Length);
            if (!isGroupId)
                UserIDs = new long[] { id };
            else
            {
                var group = Program.UserMgr.GetTaskGroup(TaskGroupID);
                if (group != null)
                {
                    UserIDs = group.Users.Keys.ToArray();
                    IsCooperation = group.IsCooperation;
                }
            }
            ProjectFile = projFile;
            string file = Options.DefaultProjectsRootPath + projFile + ProjectInfo.DefaultFileExt;
            if (System.IO.File.Exists(file))
                ProjectFileMD5 = CommonMethods.GetFileMD5(file);
        }
        public string ProjectFile = "";
        public string ProjectFileMD5 = "";
        public bool IsCooperation = false;

        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetTrainingProject(ProjectFile, ProjectFileMD5, IsCooperation);
            }
        }
    }

    public class UserNotification_StartMission : UserNotification
    {
        public UserNotification_StartMission(ProjectReferrencer pr, bool start)
            : base(null)
        {
            UserIDs = pr.UserIDs;
            IsStart = start;
        }
        public bool IsStart = true;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                if (IsStart)
                    fb.StartMission();
                else
                    fb.StopMission();
            }
        }
    }

    public class UserNotification_PauseMission : UserNotification
    {
        public UserNotification_PauseMission(ProjectReferrencer pr, bool paused)
            : base(null)
        {
            UserIDs = pr.UserIDs;
            IsPaused = paused;
        }
        public bool IsPaused = true;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.PauseMission(IsPaused);
            }
        }
    }

    public class UserNotification_SetTrainingMission : UserNotification
    {
        public UserNotification_SetTrainingMission(ProjectReferrencer pr)
            : base(null)
        {
            ProjRefer = pr;
            UserIDs = ProjRefer.UserIDs;
        }
        public ProjectReferrencer ProjRefer = null;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetTaskGroupMission(ProjRefer.TaskName, ProjRefer.SceneName, ProjRefer.ViewportIndex);
                fb.OnFreeTaskPermissionChanged(ProjRefer.FreeTaskPerm);
            }
        }
    }

    public class UserNotification_CommitRemarkRibbon : UserNotification
    {
        public UserNotification_CommitRemarkRibbon(ProjectReferrencer pr, float lifeTime)
            : base(null)
        {
            UserIDs = pr.UserIDs;
            LifeTime = lifeTime;
        }
        public float LifeTime = -1;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.FinishRemarkRibbon(LifeTime);
            }
        }
    }
    
    public class UserNotification_DrawRemarkRibbon : UserNotification
    {
        public UserNotification_DrawRemarkRibbon(ProjectReferrencer pr, float xr, float yr)
            : base(null)
        {
            UserIDs = pr.UserIDs;
            Location = new PointF(xr, yr);
        }
        public PointF Location;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.DrawRemarkRibbon(Location.X, Location.Y);
            }
        }
    }

    public class UserNotification_AddOrRemoveTaskGroupUser : UserNotification_SetTaskGroup
    {
        public UserNotification_AddOrRemoveTaskGroupUser(int groupId, long userId, bool adding)
            : base(null, groupId)
        {
            var group = Program.UserMgr.GetTaskGroup(TaskGroupID);
            UserIDs = group.Users.Keys.ToArray();
            TargetUserID = userId;
            IsAdding = adding;
            SendingUserID = userId;
        }
        public long TargetUserID = -1;
        public bool IsAdding = true;
        public override void Notify()
        {
            var user = Program.UserMgr.GetLoginUserInfo(TargetUserID);
            foreach (var fb in UserFeedBacks)
            {
                if (IsAdding)
                    fb.OnAppendedTaskGroupUser(user.LoginInfo);
                else
                    fb.OnRemovedTaskGroupUser(TargetUserID);
            }
        }
    }

    public class UserNotification_StartOrStopTaskGroupMission : UserNotification_SetTrainingMission
    {
        public UserNotification_StartOrStopTaskGroupMission(ProjectReferrencer pr, bool start)
            : base(pr)
        {
            IsStarting = start;
        }
        public bool IsStarting = false;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                if (IsStarting)
                    fb.StartMission();
                else
                    fb.StopMission();
            }
        }
    }

    public class UserNotification_UserMessage : UserNotification_SetTaskGroup
    {
        public UserNotification_UserMessage(int groupId, long userId, long targetId, MessageLevel lv, MessageType type, Color color, string message)
            : base(null, groupId)
        {
            if (groupId < 0)
            {
                UserIDs = (from grp in Program.UserMgr.TaskGroups.Values.ToArray()
                           from id in grp.Users.Keys
                           select id).ToArray();
            }
            SendingUserID = userId;
            SpecificTargetUserID = targetId;
            MsgLv = lv;
            MsgType = type;
            MsgColor = color;
            Text = message;
        }
        public MessageLevel MsgLv;
        public MessageType MsgType;
        public Color MsgColor;
        public string Text;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.OnReceiveMessage(SendingUserID, MsgLv, MsgType, MsgColor, Text);
            }
        }
    }

    public class UserNotification_ChangeUserStatus : UserNotification_SetTaskGroup
    {
        public UserNotification_ChangeUserStatus(int groupId, long userId, LoginStatus status)
            : base(null, groupId)
        {
            var grp = Program.UserMgr.GetTaskGroup(groupId);
            if (grp != null)
                UserIDs = grp.UserIDs;
            SendingUserID = userId;
            Status = status;
        }
        public LoginStatus Status = LoginStatus.在线;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.OnUserStatusChanged(SendingUserID, Status);
            }
        }
    }

    public class UserNotification_SetFreeTaskPermission : UserNotification
    {
        public UserNotification_SetFreeTaskPermission(ProjectReferrencer pr)
            : base(pr.UserIDs)
        {
            Permission = pr.FreeTaskPerm;
        }
        public FreeTaskPermission Permission = FreeTaskPermission.禁止自由选择;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.OnFreeTaskPermissionChanged(Permission);
            }
        }
    }

    public class UserNotification_SetCurViewportTime : UserNotification
    {
        public UserNotification_SetCurViewportTime(ProjectReferrencer pr)
            : base(null)
        {
            ProjRefer = pr;
            if (pr is LoginUserInfo)
                UserIDs = new long[] { ((LoginUserInfo)pr).AccountID };
            else if (pr is UserTaskGroup)
                UserIDs = ((UserTaskGroup)pr).Users.Keys.ToArray();
        }
        public ProjectReferrencer ProjRefer = null;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                if (ProjRefer.ProjectInstance != null)
                    fb.SyncPlayingViewportProgress(ProjRefer.ProjectInstance.PlayCenter.PlayingViewportIndex, ProjRefer.CurAnimTime);
            }
        }
    }

    public abstract class UserNotification_SetGroupUserOperation : UserNotification
    {
        public UserNotification_SetGroupUserOperation(ServerLoginUserInfo user)
            : base(null)
        {
            TargetGroup = user.LoginInfo.ParentGroup;
            if (TargetGroup.IsCooperation)
                UserIDs = TargetGroup.Users.Keys.ToArray();
            else
                UserIDs = user.LoginInfo.BeMonitoringUserIDs;
            SendingUserID = user.AccountID;
        }
        public UserTaskGroup TargetGroup;
    }

    public class UserNotification_SetGroupUserOperation_Create : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_Create(ServerLoginUserInfo user, ElementCreateInfo info)
            : base(user)
        {
            CreateInfo = info;
        }
        public ElementCreateInfo CreateInfo;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Create(SendingUserID, CreateInfo);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_Modify : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_Modify(ServerLoginUserInfo user, Guid guid, ElementTransitionInfo info, SizeF scale)
            : base(user)
        {
            GUID = guid;
            TransInfo = info;
            Scale = scale;
        }
        public Guid GUID;
        public ElementTransitionInfo TransInfo;
        public SizeF Scale;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Modify(GUID, TransInfo, Scale);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_Drift : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_Drift(ServerLoginUserInfo user, Guid guid, int depth)
            : base(user)
        {
            GUID = guid;
            Depth = depth;
        }
        public Guid GUID;
        public int Depth;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Drift(GUID, Depth);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_Flip : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_Flip(ServerLoginUserInfo user, Guid guid, FlippingState state)
            : base(user)
        {
            GUID = guid;
            State = state;
        }
        public Guid GUID;
        public FlippingState State;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Flip(GUID, State);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_Property : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_Property(ServerLoginUserInfo user, Guid guid, string propertyName, object value)
            : base(user)
        {
            GUID = guid;
            PropertyName = propertyName;
            Value = value;
        }
        public Guid GUID;
        public string PropertyName;
        public object Value;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_ChangeProperty(GUID, PropertyName, Value);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_Drive : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_Drive(ServerLoginUserInfo user, Guid guid, int x, int y)
            : base(user)
        {
            GUID = guid;
            Location = new Point(x, y);
        }
        public Guid GUID;
        public Point Location;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Drive(GUID, Location.X, Location.Y);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_HotkeyAnimation : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_HotkeyAnimation(ServerLoginUserInfo user, Guid guid, float time)
            : base(user)
        {
            GUID = guid;
            Time = time;
        }
        public Guid GUID;
        public float Time;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_HotkeyAnimation(GUID, Time);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_SetMaskInfo : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_SetMaskInfo(ServerLoginUserInfo user, Guid guid, Microsoft.Xna.Framework.Vector2[] vecs)
            : base(user)
        {
            GUID = guid;
            Vecs = vecs;
        }
        public Guid GUID;
        public Microsoft.Xna.Framework.Vector2[] Vecs;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_MaskTool(GUID, Vecs);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_SetWaterbagInfo : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_SetWaterbagInfo(ServerLoginUserInfo user, Guid guid, float width, Microsoft.Xna.Framework.Vector2[] vecs)
            : base(user)
        {
            GUID = guid;
            Width = width;
            Vecs = vecs;
        }
        public Guid GUID;
        public float Width;
        public Microsoft.Xna.Framework.Vector2[] Vecs;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_WaterbagTool(GUID, Width, Vecs);
            }
        }
    }

    public class UserNotification_SetGroupUserOperation_Delete : UserNotification_SetGroupUserOperation
    {
        public UserNotification_SetGroupUserOperation_Delete(ServerLoginUserInfo user, Guid guid)
            : base(user)
        {
            GUID = guid;
        }
        public Guid GUID;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Delete(GUID);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_Create : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_Create(ProjectReferrencer targetRefer, ElementInfo ei)
            : base(targetRefer.UserIDs)
        {
            CreateInfo = new ElementCreateInfo(ei);
        }
        public ElementCreateInfo CreateInfo;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Create(LoginUserInfo.SystemUserID, CreateInfo);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_Modify : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_Modify(ProjectReferrencer targetRefer, ElementInfo ei)
            : base(targetRefer.UserIDs)
        {
            GUID = ei.GUID;
            TransInfo = ei.BaseTrans;
            Scale = ei.ManualScaleOnSrcBackImage;
        }
        public Guid GUID;
        public ElementTransitionInfo TransInfo;
        public SizeF Scale;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Modify(GUID, TransInfo, Scale);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_Drift : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_Drift(ProjectReferrencer targetRefer, ElementInfo ei)
            : base(targetRefer.UserIDs)
        {
            GUID = ei.GUID;
            Depth = ei.DepthLevel;
        }
        public Guid GUID;
        public int Depth;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Drift(GUID, Depth);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_Flip : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_Flip(ProjectReferrencer targetRefer, ElementInfo ei)
            : base(targetRefer.UserIDs)
        {
            GUID = ei.GUID;
            State = ei.CurFlippingState;
        }
        public Guid GUID;
        public FlippingState State;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Flip(GUID, State);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_ChangeProperty : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_ChangeProperty(ProjectReferrencer targetRefer, ElementInfo ei, string propertyName, object value)
            : base(targetRefer.UserIDs)
        {
            GUID = ei.GUID;
            PropertyName = propertyName;
            Value = value;
        }
        public Guid GUID;
        public string PropertyName;
        public object Value;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_ChangeProperty(GUID, PropertyName, Value);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_Drive : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_Drive(ProjectReferrencer targetRefer, ElementInfo ei, int x, int y)
            : base(targetRefer.UserIDs)
        {
            GUID = ei.GUID;
            X = x; Y = y;
        }
        public Guid GUID;
        public int X;
        public int Y;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Drive(GUID, X, Y);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_SetMask : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_SetMask(ProjectReferrencer targetRefer, ElementInfo_Mask ei)
            : base(targetRefer.UserIDs)
        {
            GUID = ei.GUID;
            Vecs = ei.CornerVectors;
        }
        public Guid GUID;
        public Microsoft.Xna.Framework.Vector2[] Vecs;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_MaskTool(GUID, Vecs);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_SetWaterbag : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_SetWaterbag(ProjectReferrencer targetRefer, ElementInfo_Waterbag ei)
            : base(targetRefer.UserIDs)
        {
            GUID = ei.GUID;
            Width = ei.BagWidth;
            Vecs = ei.JointVecs;
        }
        public Guid GUID;
        public float Width;
        public Microsoft.Xna.Framework.Vector2[] Vecs;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_WaterbagTool(GUID, Width, Vecs);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_HotkeyAnimation : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_HotkeyAnimation(ProjectReferrencer targetRefer, ElementInfo ei)
            : base(targetRefer.UserIDs)
        {
            GUID = ei.GUID;
            Time = ei.HotKeyAnimBeginTime;
        }
        public Guid GUID;
        public float Time;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_HotkeyAnimation(GUID, Time);
            }
        }
    }

    public class UserNotification_SetGroupSystemOperation_Delete : UserNotification
    {
        public UserNotification_SetGroupSystemOperation_Delete(ProjectReferrencer targetRefer, Guid guid)
            : base(targetRefer.UserIDs)
        {
            GUID = guid;
        }
        public Guid GUID;
        public override void Notify()
        {
            foreach (var fb in UserFeedBacks)
            {
                fb.SetGroupUserOperation_Delete(GUID);
            }
        }
    }
}
