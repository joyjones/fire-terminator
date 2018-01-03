using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.Structures;
using FireTerminator.Common;
using FireTerminator.Common.Services;
using System.Drawing;
using FireTerminator.Common.Operations;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.Elements;
using Microsoft.Xna.Framework;
using System.IO;
using System.Runtime.Serialization;

namespace FireTerminator.Client
{
    public partial class MainForm : IServiceCallback
    {
        #region IServiceCallback 成员
        public void SetCurrentTaskGroup(UserTaskGroup group)
        {
            Program.CurUser.ParentGroup = group;
            RefreshStudentsGroupList();
        }
        private void CheckShowDefaultScene()
        {
            var prj = ProjectDoc.Instance.SelectedProject;
            if (prj != null)
            {
                if (ProjectDoc.Instance.SelectedSceneInfo == null && prj.TaskInfos.Count > 0 && prj.TaskInfos[0].SceneInfos.Count > 0)
                {
                    ProjectDoc.Instance.SelectedTaskInfo = prj.TaskInfos[0];
                    ProjectDoc.Instance.SelectedSceneInfo = prj.TaskInfos[0].SceneInfos[0];
                    ProjectDoc.Instance.SelectedViewportInfo = null;
                }
            }
        }
        private string m_WaitForReadyProjectName = null;
        public void SetTrainingProject(string projectName, string fileMd5, bool coOperation)
        {
            if (Program.CurUser.ParentGroup != null)
                Program.CurUser.ParentGroup.IsCooperation = coOperation;
            Program.CurUser.ProjectName = projectName;
            ProjectDoc.Instance.SelectedProject = Program.CurUser.ProjectInstance;
            if (!String.IsNullOrEmpty(projectName) && String.IsNullOrEmpty(Program.CurUser.ProjectName))
                m_WaitForReadyProjectName = projectName;
            string projectFile = projectName + ProjectInfo.DefaultFileExt;

            string msg = "";
            if (String.IsNullOrEmpty(Program.CurUser.ProjectName))
                msg = "当前训练项目已被重置。";
            else
                msg = String.Format("当前训练项目已被设置为：{0}。", Program.CurUser.ProjectName);
            MessageController.PushMessage(MessageLevel.提示, MessageType.任务操作, msg);

            if (!String.IsNullOrEmpty(projectFile) && !String.IsNullOrEmpty(fileMd5))
            {
                bool hasCorrectFile = false;
                string file = Options.DefaultProjectsRootPath + projectFile;
                if (System.IO.File.Exists(file))
                {
                    string md5 = CommonMethods.GetFileMD5(file);
                    hasCorrectFile = fileMd5 == md5;
                }
                if (hasCorrectFile)
                    OnTrainingProjectFileReady();
                else
                    Program.Service.QueryTransmissionFile(TransFileKind.ProjectFile, projectFile);
            }
            var group = Program.CurUser.ParentGroup;
            if (Program.CurUser.ViewportIndex < 0 && group != null && group.IsTaskSceneSpecified)
                SetTaskGroupMission(group.TaskName, group.SceneName, group.ViewportIndex);
            if (Program.CurUser.ProjectName.Length > 0)
                this.Ribbon.SelectedPage = rbpCurProject;
            Program.CurUser.IsStarted = false;
            CheckShowDefaultScene();
            RefreshTrainningUI();
        }
        // 改变任务分组任务状态
        public void SetTaskGroupMission(string task, string scene, int viewportIdx)
        {
            Program.CurUser.TaskName = task;
            Program.CurUser.SceneName = scene;
            Program.CurUser.ViewportIndex = viewportIdx;

            string msg = "";
            if (viewportIdx < 0)
                msg = "当前训练任务已被重置。";
            else
                msg = String.Format("当前训练任务已被设置为：任务'{0}'-场景'{1}'-窗口{2}", task, scene, viewportIdx + 1);
            MessageController.PushMessage(MessageLevel.提示, MessageType.任务操作, msg);
            m_EditViewOperater.RefreshProjectTree();
            CheckShowDefaultScene();
            RefreshTrainningUI();
        }
        // 添加分组用户
        public void OnAppendedTaskGroupUser(LoginUserInfo user)
        {
            var group = Program.CurUser.ParentGroup;
            group.AddUser(user);
            RefreshStudentsGroupList();
        }
        // 移除分组用户
        public void OnRemovedTaskGroupUser(long userId)
        {
            var group = Program.CurUser.ParentGroup;
            group.RemoveUser(userId);
            RefreshStudentsGroupList();
        }
        // 启动任务分组
        public void StartMission()
        {
            Program.CurUser.IsStarted = true;
            Program.CurUser.IsPaused = false;
            // 禁止客户端自动切换窗口而等待服务器同步
            ProjectDoc.Instance.SelectedProject.PlayCenter.AutoNextView = false;
            CurStatus = TaskGroupMissionStatus.训练进行中;
        }
        public void PauseMission(bool bPause)
        {
            Program.CurUser.IsStarted = true;
            Program.CurUser.IsPaused = bPause;
            if (bPause)
                CurStatus = TaskGroupMissionStatus.训练暂停;
            else
                CurStatus = TaskGroupMissionStatus.训练进行中;
        }
        public void StopMission()
        {
            Program.CurUser.IsStarted = false;
            if (Program.CurUser.IsPaused)
                ProjectDoc.Instance.IsProjectAnimationPlaying = true;
            Program.CurUser.IsPaused = false;
            ProjectDoc.Instance.IsProjectAnimationPlaying = false;
            if (ProjectDoc.Instance.SelectedViewportInfo != null)
                ProjectDoc.Instance.SelectedViewportInfo.RemoveAllUserElements();
            CurStatus = TaskGroupMissionStatus.等待训练开始;
        }
        // 系统或用户消息
        public void OnReceiveMessage(long fromUserId, MessageLevel lv, MessageType type, Color color, string message)
        {
            //if (fromUserId > 0)
            //{
            //    var user = Program.CurUser.ParentGroup.GetUser(fromUserId);
            //    if (user != null)
            //        message = String.Format("{0}: {1}", user.Name, message);
            //}
            MessageController.PushMessage(lv, type, color, message);
        }
        // 改变用户状态
        public void OnUserStatusChanged(long userId, LoginStatus status)
        {
            LoginUserInfo user = null;
            if (Program.CurUser.AdjudicatorMode)
                user = Program.GetLoginUserInfo(userId);
            else
            {
                var group = Program.CurUser.ParentGroup;
                if (group != null)
                    user = group.GetUser(userId);
            }
            if (user != null && user.Status != status)
            {
                user.Status = status;
                if (user.Status < LoginStatus.在线)
                {
                    if (ProjectDoc.Instance.SelectedViewportInfo != null)
                        ProjectDoc.Instance.SelectedViewportInfo.RemoveAllCreatorElements(user.AccountID);
                }
                RefreshStudentsGroupList();
            }
        }
        // 开始被服务端监控
        public void StartMonitor()
        {
            if (!OperationHistory.Instance.IsServerMonitoring)
            {
                OperationHistory.Instance.IsServerMonitoring = true;
                var vi = ProjectDoc.Instance.SelectedViewportInfo;
                if (vi != null)
                {
                    var infos = new List<ElementCreateInfo>();
                    foreach (var e in vi.Elements)
                    {
                        if (e == null || e.IsLocked || !e.CanSelect)
                            continue;
                        infos.Add(new ElementCreateInfo(e));
                    }
                    Program.Service.MainChannel.SetElementOperation_Init(infos.ToArray());
                }
            }
        }
        // 终止被服务端监控
        public void TerminateMonitor()
        {
            OperationHistory.Instance.IsServerMonitoring = false;
        }
        private int m_LastPlayingViewportIndex = -1;
        // 同步场景动画时间
        public void SyncPlayingViewportProgress(int playIndex, float time)
        {
            try
            {
                var proj = ProjectDoc.Instance.SelectedProject;
                if (proj != null && proj.PlayCenter.PlayingViewport != null)
                {
                    proj.PlayCenter.PlayingViewportIndex = playIndex;
                    float percent = 0;
                    if (proj.PlayCenter.PlayingViewport != null)
                    {
                        proj.PlayCenter.PlayingViewport.CurTimeTick = time;
                        percent = (float)proj.PlayCenter.PlayingViewport.CurTimeTick / proj.PlayCenter.PlayingViewport.TimeLength;
                    }
                    beiStatusProgress.EditValue = (int)(percent * repositoryItemViewProgressBar.Maximum);
                    if (m_LastPlayingViewportIndex != proj.PlayCenter.PlayingViewportIndex)
                    {
                        if (proj.PlayCenter.PlayingViewport != null)
                            Program.CurUser.ViewportIndex = proj.PlayCenter.PlayingViewport.ScreenIndex;
                        m_LastPlayingViewportIndex = proj.PlayCenter.PlayingViewportIndex;
                        RefreshTrainningUI();
                    }
                }
            }
            catch
            {
            }
        }
        // 合作组员创建元素
        public void SetGroupUserOperation_Create(long userId, ElementCreateInfo info)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var rg = ProjectDoc.Instance.ResourceGroups[info.ResKind];
            var res = rg.GetResourceInfo(info.ResPathFile);
            if (res == null)
                MessageController.PushMessage(MessageLevel.警告, MessageType.任务操作, Color.Red, "未能找到所需的同步资源：" + info.ResPathFile);
            else
            {
                var e = ElementInfo.CreateElement(res, vi, new System.Drawing.PointF(0, 0));
                e.GUID = info.GUID;
                e.CreatorId = userId;
                e.BaseTrans.Copy(info.TransInfo);
                e.ManualScaleOnSrcBackImage = info.ManualScaleOnSrcBackImage;
                vi.AddElement(e);
            }
        }
        // 合作组员删除元素
        public void SetGroupUserOperation_Delete(Guid guid)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var e = vi.GetElementInfo(guid);
            if (e != null && !e.IsLocked)
            {
                vi.RemoveElement(e);
            }
        }
        // 合作组员修改元素
        public void SetGroupUserOperation_Modify(Guid guid, ElementTransitionInfo info, SizeF scale)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null)
            {
                var e = vi.GetElementInfo(guid);
                if (e != null)
                {
                    e.BaseTrans.Copy(info);
                    e.ManualScaleOnSrcBackImage = scale;
                }
            }
        }
        // 合作组员移动元素深度
        public void SetGroupUserOperation_Drift(Guid guid, int depth)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var e = vi.GetElementInfo(guid);
            if (e != null)
            {
                e.DepthLevel = depth;
            }
        }
        // 合作组员翻转元素
        public void SetGroupUserOperation_Flip(Guid guid, FlippingState state)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var e = vi.GetElementInfo(guid);
            if (e != null)
            {
                e.CurFlippingState = state;
            }
        }
        // 合作组员操作遮罩元素
        public void SetGroupUserOperation_MaskTool(Guid guid, Vector2[] vecs)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var e = vi.GetElementInfo(guid);
            if (e != null && e is ElementInfo_Mask)
            {
                ((ElementInfo_Mask)e).SetPosition(vecs);
            }
        }
        // 合作组员操作水带元素
        public void SetGroupUserOperation_WaterbagTool(Guid guid, float width, Vector2[] vecs)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var e = vi.GetElementInfo(guid);
            if (e != null && e is ElementInfo_Waterbag)
            {
                ((ElementInfo_Waterbag)e).SetProperties(width, vecs);
            }
        }
        // 合作组员设置元素属性
        public void SetGroupUserOperation_ChangeProperty(Guid guid, string propertyName, object value)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var e = vi.GetElementInfo(guid);
            if (e != null)
            {
                var pi = e.GetType().GetProperty(propertyName);
                if (pi != null)
                {
                    pi.SetValue(e, value, null);
                }
            }
        }
        // 合作组员远程移动元素
        public void SetGroupUserOperation_Drive(Guid guid, int x, int y)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var e = vi.GetElementInfo(guid);
            if (e != null)
            {
                e.TargetLocation = new System.Drawing.Point(x, y);
            }
        }
        // 合作组员触发元素按键动画
        public void SetGroupUserOperation_HotkeyAnimation(Guid guid, float time)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            var e = vi.GetElementInfo(guid);
            if (e != null)
            {
                e.HotKeyAnimBeginTime = time;
            }
        }
        // 改变任务自由选择权限
        public void OnFreeTaskPermissionChanged(FreeTaskPermission permission)
        {
            if (Program.CurUser.FreeTaskPerm != permission && !Program.CurUser.AdjudicatorMode)
            {
                Program.CurUser.FreeTaskPerm = permission;
                RefreshTrainningUI();

                string msg = String.Format("你的任务选择权限已被设置为：{0}", permission);
                if (permission == FreeTaskPermission.禁止自由选择)
                    msg += "，你现在不可以主动切换任何任务、场景或窗口。";
                else if ((permission & FreeTaskPermission.自由选择任务) == FreeTaskPermission.自由选择任务)
                    msg += "，你现在可以自由切换当前指定项目中的任意任务、场景或窗口。";
                else if ((permission & FreeTaskPermission.自由选择场景) == FreeTaskPermission.自由选择场景)
                    msg += "，你现在可以在当前项目指定任务中自由切换任意场景和窗口。";
                else if ((permission & FreeTaskPermission.自由选择窗口) == FreeTaskPermission.自由选择窗口)
                    msg += "，你现在可以在当前项目指定任务和场景中自由切换任意窗口。";
                MessageController.PushMessage(MessageLevel.提示, MessageType.任务操作, msg);
            }
        }
        // 同步完整分组信息
        public void SyncUserTaskGroups(UserTaskGroup[] grps)
        {
            Program.TaskGroups.Clear();
            foreach (var grp in grps)
            {
                Program.TaskGroups[grp.GroupID] = grp;
            }
        }
        // 回复监控请求
        public void ReplyMonitorTargetGroupOrUser(long targetId, bool isGroup, bool succeeded)
        {
            if (succeeded)
            {
                if (isGroup)
                    Program.CurUser.TargetMonitorUserOrGroupID = targetId * -1;
                else
                    Program.CurUser.TargetMonitorUserOrGroupID = targetId;
                RefreshTrainningUI();
                RefreshJudgementUI();
                RefreshTitleCaption();
            }
        }
        // 同步评审配置信息
        public void SyncJudgementConfigContext(string context)
        {
            using (var sw = new StreamWriter(JudgementSettings.ConfigFile, false, Encoding.UTF8))
            {
                sw.Write(context);
            }
            Program.JudgementSet.Load();
            RefreshJudgementUI();
        }
        // 绘制点评绘制样条
        public void DrawRemarkRibbon(float xr, float yr)
        {
            if (Program.Graphic.CreatingRibbon == null)
                Program.Graphic.CreateCurveRibbon();
            if (Program.Graphic.CreatingRibbon != null)
                Program.Graphic.CreatingRibbon.AppendPoint(xr, yr);
        }
        // 结束创建点评绘制样条
        public void FinishRemarkRibbon(float lifeTime)
        {
            Program.Graphic.ConfirmCurveRibbon(lifeTime);
        }
        #endregion
    }
}
