using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.Services;
using System.ServiceModel;
using System.Drawing;
using FireTerminator.Common.Operations;
using FireTerminator.Common;
using FireTerminator.Common.Elements;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Transitions;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using FireTerminator.Common.Structures;

namespace FireTerminator.Server.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class MainService : IMainService
    {
        public bool BeatingHeart(int linkPort)
        {
            var user = Program.UserMgr.RegisterService(OperationContext.Current, linkPort);
            if (user != null)
            {
                user.LastBeatingHeartTick = DateTime.Now.Ticks;
                return !user.IsBeatingHeartTimeout;
            }
            return false;
        }
        public void Logout()
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                Program.UserMgr.KickUser(user.AccountID, 5, LogoutReason.主动退出, null);
            }
        }
        public void SendMessage(long targetId, Color color, string message)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (UserSentMessage != null)
                UserSentMessage(user.AccountID, targetId, color, message);
        }
        public void ReportMissingResourceFiles(string[] files)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (UserReportMissingResourceFiles != null && user != null)
                UserReportMissingResourceFiles(user.AccountID, files);
        }
        public void ReportDownloadedOneResourceFile(string file)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (UserReportDownloadedOneResourceFiles != null)
                UserReportDownloadedOneResourceFiles(user.AccountID, new string[] { file });
        }
        public bool ModifyPassword(string newPassword)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                user.Info.Password = newPassword;
                return true;
            }
            return false;
        }
        public void RequestChangeTrainningTask(string taskName, string sceneName, int viewIndex)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var pr = user.LoginInfo;
                if (pr != null && pr.ParentGroup != null && !pr.ParentGroup.IsCooperation && pr.ProjectDescription != null)
                {
                    bool tChanged = pr.TaskName != taskName;
                    bool sChanged = pr.SceneName != sceneName;
                    bool vChanged = pr.ViewportIndex != viewIndex;

                    if (tChanged && (pr.FreeTaskPerm & FreeTaskPermission.自由选择任务) == FreeTaskPermission.自由选择任务)
                    {
                        var t = pr.ProjectDescription.GetTask(taskName);
                        if (t != null)
                        {
                            pr.TaskName = taskName;
                            if (t.Scenes.Count == 0)
                            {
                                pr.SceneName = "";
                                pr.ViewportIndex = -1;
                            }
                            else
                            {
                                pr.SceneName = t.Scenes[0].Name;
                                pr.ViewportIndex = 0;
                            }
                            Program.UserMgr.PushTaskChangeRequester(new UserManager.ProjectReferrencerReq(pr, UserManager.ProjectReferrencerReq.RequestType.ChangeTask));
                        }
                    }
                    if (sChanged && (pr.FreeTaskPerm & FreeTaskPermission.自由选择场景) == FreeTaskPermission.自由选择场景)
                    {
                        var t = pr.ProjectDescription.GetTask(taskName);
                        if (t != null)
                        {
                            var s = t.GetScene(sceneName);
                            if (s != null)
                            {
                                pr.SceneName = s.Name;
                                pr.ViewportIndex = 0;
                                Program.UserMgr.PushTaskChangeRequester(new UserManager.ProjectReferrencerReq(pr, UserManager.ProjectReferrencerReq.RequestType.ChangeTask));
                            }
                        }
                    }
                    if (vChanged && (pr.FreeTaskPerm & FreeTaskPermission.自由选择窗口) == FreeTaskPermission.自由选择窗口)
                    {
                        var t = pr.ProjectDescription.GetTask(taskName);
                        if (t != null)
                        {
                            var s = t.GetScene(sceneName);
                            if (s != null && viewIndex >= 0 && viewIndex < 4)
                            {
                                pr.ViewportIndex = viewIndex;
                                Program.UserMgr.PushTaskChangeRequester(new UserManager.ProjectReferrencerReq(pr, UserManager.ProjectReferrencerReq.RequestType.ChangeTask));
                            }
                        }
                    }
                }
            }
        }
        public void RequestStartOrStopMission(bool start)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var pr = user.LoginInfo;
                if (pr != null && pr.ParentGroup != null &&
                    !pr.ParentGroup.IsCooperation && pr.ProjectDescription != null &&
                    pr.IsTaskViewSpecified && pr.FreeTaskPerm != FreeTaskPermission.禁止自由选择)
                {
                    var type = start ? UserManager.ProjectReferrencerReq.RequestType.StartTask : UserManager.ProjectReferrencerReq.RequestType.StopTask;
                    Program.UserMgr.PushTaskChangeRequester(new UserManager.ProjectReferrencerReq(pr, type));
                }
            }
        }
        public void SetElementOperation_Init(ElementCreateInfo[] infos)
        {
            //var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            //if (user != null && Program.UserMgr.IsMonitoringUser(user.AccountID))
            //{
            //    foreach (var info in infos)
            //    {
            //        AppendElementForCurrentViewport(user.AccountID, info);
            //    }
            //}
        }
        public void SetElementOperation_Create(ElementCreateInfo info)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_Create(user.AccountID, vi, info));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_Create(user, info));
            }
        }
        public void SetElementOperation_Delete(Guid guid)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_Delete(user.AccountID, vi, guid));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_Delete(user, guid));
            }
        }
        public void SetElementOperation(Guid guid, ElementTransitionInfo info, SizeF scale)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_Trans(user.AccountID, vi, guid, info, scale));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_Modify(user, guid, info, scale));
            }
        }
        public void SetElementDepthLevel(Guid guid, int depth)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_MoveDepth(user.AccountID, vi, guid, depth));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_Drift(user, guid, depth));
            }
        }
        public void SetElementFlipState(Guid guid, FlippingState state)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_SetFlip(user.AccountID, vi, guid, state));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_Flip(user, guid, state));
            }
        }
        public void SetToolElementInfo_Mask(Guid guid, Vector2[] vecs)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_SetMaskInfo(user.AccountID, vi, guid, vecs));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_SetMaskInfo(user, guid, vecs));
            }
        }
        public void SetToolElementInfo_Waterbag(Guid guid, float width, Vector2[] vecs)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_SetWaterbagInfo(user.AccountID, vi, guid, width, vecs));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_SetWaterbagInfo(user, guid, width, vecs));
            }
        }
        public void SetElementProperty(Guid guid, string propertyName, object newValue)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_SetProperty(user.AccountID, vi, guid, propertyName, newValue));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_Property(user, guid, propertyName, newValue));
            }
        }
        public void SetElementTargetLocation(Guid guid, int x, int y)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_Drive(user.AccountID, vi, guid, x, y));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_Drive(user, guid, x, y));
            }
        }
        public void SetElementHotkeyAnimation(Guid guid, int keycode, float time)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                var vi = user.LoginInfo.SpecificViewportInfo;
                Program.UserMgr.UserOperations.PushOperation(new UserOperation_HotkeyAnimation(user.AccountID, vi, guid, time));
                Program.UserMgr.PushNotification(new UserNotification_SetGroupUserOperation_HotkeyAnimation(user, guid, time));
            }
        }
        public void RequestMonitorGroupOrUser(long targetId, bool isGroup)
        {
            var user = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (user != null)
            {
                try
                {
                    ProjectReferrencer prTarget = null;
                    if (!user.LoginInfo.AdjudicatorMode)
                        user.Feedback.ReplyMonitorTargetGroupOrUser(targetId, isGroup, false);
                    else if (isGroup)
                    {
                        var grp = Program.UserMgr.GetTaskGroup((int)targetId);
                        if (grp == null || !grp.IsCooperation)
                            user.Feedback.ReplyMonitorTargetGroupOrUser(targetId, isGroup, false);
                        else
                        {
                            Program.UserMgr.RemoveMonitorUserFromAllReferrencers(user.AccountID);
                            user.LoginInfo.TargetMonitorUserOrGroupID = grp.GroupID * -1;
                            grp.BeMonitoringUsers[user.AccountID] = user.LoginInfo;
                            user.Feedback.ReplyMonitorTargetGroupOrUser(targetId, true, true);
                            prTarget = grp;
                        }
                    }
                    else
                    {
                        var target = Program.UserMgr.GetLoginUserInfo(targetId);
                        if (target == null || target.LoginInfo.ParentGroup.IsCooperation)
                            user.Feedback.ReplyMonitorTargetGroupOrUser(targetId, false, false);
                        else
                        {
                            Program.UserMgr.RemoveMonitorUserFromAllReferrencers(user.AccountID);
                            user.LoginInfo.TargetMonitorUserOrGroupID = targetId;
                            target.LoginInfo.BeMonitoringUsers[user.AccountID] = user.LoginInfo;
                            user.Feedback.ReplyMonitorTargetGroupOrUser(targetId, false, true);
                            prTarget = target.LoginInfo;
                        }
                    }
                    if (prTarget != null)
                    {
                        user.LoginInfo.ProjectInstanceDirectly = prTarget.ProjectInstance;
                        user.LoginInfo.ViewportIndex = prTarget.ViewportIndex;
                        Program.UserMgr.PushNotification(new UserNotification_SetTrainingProject(user.AccountID, false, user.LoginInfo.ProjectName));
                        Program.UserMgr.PushNotification(new UserNotification_SetTrainingMission(user.LoginInfo));
                        user.CheckStartMission();
                    }
                }
                catch
                {
                }
            }
        }

        public delegate void Delegate_SendMessage(long userId, long targetId, Color color, string message);
        public delegate void Delegate_UserReportFiles(long userId, string[] files);
        public static event Delegate_SendMessage UserSentMessage;
        public static event Delegate_UserReportFiles UserReportMissingResourceFiles;
        public static event Delegate_UserReportFiles UserReportDownloadedOneResourceFiles;
    }
}
