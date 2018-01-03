using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FireTerminator.Common.Services;
using FireTerminator.Common;
using FireTerminator.Common.UI;
using DevExpress.XtraTreeList.Nodes;
using FireTerminator.Server.Services;
using FireTerminator.Common.Structures;
using DevExpress.XtraBars;
using System.Threading;
using FireTerminator.Common.Operations;
using FireTerminator.Common.RenderResources;
using System.Diagnostics;

namespace FireTerminator.Server
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MainForm()
        {
            splashScreenMgr = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(SplashScreen), true, true);
            InitializeComponent();
            Program.Option.Load();
            InitEditViewOperator();
            DevExpress.XtraBars.Helpers.SkinHelper.InitSkinGallery(rgbiSkins, true);
            rgbiSkins.GalleryItemClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(rgbiSkins_GalleryItemClick);
            RefreshJudgementTable();
        }

        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenMgr;
        private ServerEditViewOperater m_EditViewOperater = null;

        public MessageControl MessageController
        {
            get;
            private set;
        }
        public ProjectReferrencer CurFocusTreeNodeProjReferrencer
        {
            get
            {
                var node = trvStudentGroups.FocusedNode;
                if (node == null)
                    return null;
                var suser = node.Tag as ServerLoginUserInfo;
                if (suser != null)
                    return suser.LoginInfo;
                return node.Tag as UserTaskGroup;
            }
        }
        public ProjectReferrencer CurAvailableProjReferrencer
        {
            get
            {
                var user = SelectedUserInfo;
                if (user != null && user.ParentGroup.IsCooperation)
                    return user.ParentGroup;
                return CurFocusTreeNodeProjReferrencer;
            }
        }
        public UserTaskGroup SelectedTaskGroup
        {
            get { return CurFocusTreeNodeProjReferrencer as UserTaskGroup; }
        }
        public LoginUserInfo SelectedUserInfo
        {
            get { return CurFocusTreeNodeProjReferrencer as LoginUserInfo; }
        }
        public bool IsCurTaskGroupNeedingTransferFiles
        {
            get
            {
                var pr = CurFocusTreeNodeProjReferrencer;
                if (pr == null)
                    return false;
                foreach (var id in pr.UserIDs)
                {
                    var suser = Program.UserMgr.GetLoginUserInfo(id);
                    if (suser.LoginInfo.MissingResourceFiles.Length > 0)
                        return true;
                }
                return false;
            }
        }
        public void RefreshUI()
        {
            var prSel = CurFocusTreeNodeProjReferrencer;
            bool bSelected = prSel != null;
            bool bProjSelected = bSelected && prSel.ProjectInstance != null;
            bool bGroupSelected = bSelected && prSel is UserTaskGroup;
            bool bSelInDefaultGroup = ((bGroupSelected && SelectedTaskGroup.GroupID == 0)
                || (!bGroupSelected && SelectedUserInfo != null && SelectedUserInfo.ParentGroup != null && SelectedUserInfo.ParentGroup.GroupID == 0));
            bool bSelNoCooperatUser = !bGroupSelected && SelectedUserInfo != null && !SelectedUserInfo.ParentGroup.IsCooperation;
            if (!bSelected)
                iSetCurTaskGroup.Caption = "<选择分组>";
            else
            {
                if (bGroupSelected)
                    iSetCurTaskGroup.Caption = SelectedTaskGroup.Name;
                else
                    iSetCurTaskGroup.Caption = SelectedUserInfo.ParentGroup.Name;
            }
            iAddStudent.Enabled = bSelected && !prSel.IsStarted;
            iSetCurProject.Enabled = (bGroupSelected || bSelNoCooperatUser) && !prSel.IsStarted && !bSelInDefaultGroup;
            iSetCurTask.Enabled = bProjSelected && iSetCurProject.Enabled;
            icSyncResources.Enabled = bProjSelected && IsCurTaskGroupNeedingTransferFiles;
            icSyncResources.Checked = bProjSelected && icSyncResources.Enabled && prSel.IsResSynchronizing;
            iRemoveTaskGroup.Enabled = bSelected && !bSelInDefaultGroup;
            iRenameTaskGroup.Enabled = bSelected && !bSelInDefaultGroup;
            icCooperateTask.Enabled = bGroupSelected && !prSel.IsStarted && !bSelInDefaultGroup;
            icCooperateTask.Checked = bGroupSelected && SelectedTaskGroup.IsCooperation;
            //icReplayMission.Checked = false;
            icFreeSelViewport.Enabled = bProjSelected && bSelNoCooperatUser;
            icFreeSelViewport.Checked = icFreeSelViewport.Enabled && ((prSel.FreeTaskPerm & FreeTaskPermission.自由选择窗口) == FreeTaskPermission.自由选择窗口);
            icFreeSelScenes.Enabled = bProjSelected && icFreeSelViewport.Checked;
            icFreeSelScenes.Checked = icFreeSelScenes.Enabled && ((prSel.FreeTaskPerm & FreeTaskPermission.自由选择场景) == FreeTaskPermission.自由选择场景);
            icFreeSelTasks.Enabled = bProjSelected && icFreeSelScenes.Checked;
            icFreeSelTasks.Checked = icFreeSelTasks.Enabled && ((prSel.FreeTaskPerm & FreeTaskPermission.自由选择任务) == FreeTaskPermission.自由选择任务);
            icReplayMission.Enabled = icStartMission.Enabled && !icStartMission.Checked && Program.UserMgr.UserOperations.Operations.Count > 0;
            icRemarkTool.Enabled = icStartMission.Enabled && icStartMission.Checked;
            beiPenColor.Enabled = icRemarkTool.Enabled;
            beiPenThickness.Enabled = icRemarkTool.Enabled;
            UpdateStartMissionButton(bProjSelected && !icSyncResources.Enabled, bSelected && prSel.IsStarted, bSelected && prSel.IsPaused);
        }

        private byte[] DefaultDockManagerLayoutData = null;
        private void MainForm_Load(object sender, EventArgs e)
        {
            MessageController = new MessageControl();
            MessageController.Dock = DockStyle.Fill;
            dcpMessages.Controls.Add(MessageController);
            MessageController.DefaultMessageType = MessageType.服务端消息;
            MessageController.SendMessage += new MessageControl.Delegate_SendMessage(MessageController_SendMessage);

            //SelectedTaskGroup = null;
            m_EditViewOperater.RefreshAppProjectTitle();
            RefreshStudentsList();
            RefreshStudentGroupsList();
            m_EditViewOperater.RefreshResourceTree(false);
            m_EditViewOperater.RefreshProjectTree();
            beiPenColor.EditValue = Color.OrangeRed;
            RefreshUI();

            ServerHost host1 = new ServerHost("登录");
            host1.OnErrorMessage += new ServerHost.Delegate_OnErrorMessage(MessageController.PushErrorMessage);
            host1.OnMessage += new ServerHost.Delegate_OnMessage(MessageController.PushMessage);
            Program.OnStartUpLoginService(host1);

            ServerHost host2 = new ServerHost("主");
            host2.OnErrorMessage += new ServerHost.Delegate_OnErrorMessage(MessageController.PushErrorMessage);
            host2.OnMessage += new ServerHost.Delegate_OnMessage(MessageController.PushMessage);
            Program.OnStartUpMainService(host2);
            
            ServerHost host3 = new ServerHost("文件传输");
            host3.OnErrorMessage += new ServerHost.Delegate_OnErrorMessage(MessageController.PushErrorMessage);
            host3.OnMessage += new ServerHost.Delegate_OnMessage(MessageController.PushMessage);
            Program.OnStartUpTransmissionRequestService(host3);

            Program.UserMgr.UserLogined += new UserManager.Delegate_LoginUser(UserMgr_UserLogined);
            Program.UserMgr.UserLogouted += new UserManager.Delegate_LogoutUser(UserMgr_UserLogouted);
            MainService.UserSentMessage += new MainService.Delegate_SendMessage(MainService_UserSentMessage);
            MainService.UserReportMissingResourceFiles += new MainService.Delegate_UserReportFiles(MainService_UserReportMissingResourceFiles);
            MainService.UserReportDownloadedOneResourceFiles += new MainService.Delegate_UserReportFiles(MainService_UserReportDownloadedOneResourceFiles);

            dockMgr.ClosedPanel += new DevExpress.XtraBars.Docking.DockPanelEventHandler(dockMgr_ClosedPanel);
            if (System.IO.File.Exists(Program.Option.LayoutConfigFile))
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    dockMgr.SaveLayoutToStream(ms);
                    DefaultDockManagerLayoutData = ms.ToArray();
                }
                dockMgr.RestoreLayoutFromXml(Program.Option.LayoutConfigFile);
            }
            OperationHistory.Instance.NewOperationPushed += new OperationHistory.Delegate_OnOperationChanged(OperattionHistory_NewOperationPushed);
            ProjectDoc.Instance.CurEditUserID = LoginUserInfo.SystemUserID;
            ProjectDoc.Instance.IsCooperatingEditMode = true;
        }

        void dockMgr_ClosedPanel(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            if (e.Panel == dcpMessages)
                icShowMessageBar.Checked = false;
            else if (e.Panel == dcpProject)
                icShowProjectBar.Checked = false;
            else if (e.Panel == dcpResources)
                icShowResourceBar.Checked = false;
            else if (e.Panel == dcpStudents)
                icShowStudentList.Checked = false;
            else if (e.Panel == dcpStudentGroups)
                icShowStudentGroups.Checked = false;
            else if (e.Panel == dcpProperties)
                icShowPropertyBar.Checked = false;
        }

        private void MessageController_SendMessage(long targetId, Color clr, string message)
        {
            Program.UserMgr.PushNotification(new UserNotification_UserMessage(-1, -1, targetId, MessageLevel.一般, MessageType.服务端消息, clr, message));
        }

        private void MainService_UserSentMessage(long userId, long targetId, Color color, string message)
        {
            this.InvokeEx(() =>
            {
                var user = Program.UserMgr.GetLoginUserInfo(userId);
                if (user != null)
                {
                    var targetUser = Program.UserMgr.GetLoginUserInfo(targetId);
                    if (targetUser != null)
                        message = String.Format("{0}对{1}说: {2}", user.Name, targetUser.Name, message);
                    else
                        message = String.Format("{0}: {1}", user.Name, message);
                }
                MessageController.PushMessage(color, message);
                Program.UserMgr.PushNotification(new UserNotification_UserMessage(user.Info.TaskGroupID, userId, targetId, MessageLevel.一般, MessageType.用户消息, color, message));
            });
        }

        private void MainService_UserReportMissingResourceFiles(long userId, string[] files)
        {
            this.InvokeEx(() =>
            {
                var user = Program.UserMgr.GetLoginUserInfo(userId);
                if (user != null)
                {
                    user.LoginInfo.MissingResourceFiles = files;
                    RefreshUI();
                    if (user.LoginInfo.IsStarted)
                    {
                        if (user.LoginInfo.MissingResourceFiles.Length > 0)
                            Program.ResSynchronizer.PushRequest(user.LoginInfo);
                        else
                        {
                            user.Feedback.SetTaskGroupMission(user.LoginInfo.TaskName, user.LoginInfo.SceneName, user.LoginInfo.ViewportIndex);
                            user.CheckStartMission();
                        }
                    }
                }
            });
        }

        private void MainService_UserReportDownloadedOneResourceFiles(long userId, string[] files)
        {
            this.InvokeEx(() =>
            {
                var user = Program.UserMgr.GetLoginUserInfo(userId);
                if (user != null)
                {
                    foreach (var file in files)
                        user.LoginInfo.RemoveMissingFile(file);
                    RefreshUI();
                    if (user.LoginInfo.MissingResourceFiles.Length == 0)
                    {
                        user.Feedback.SetTaskGroupMission(user.LoginInfo.TaskName, user.LoginInfo.SceneName, user.LoginInfo.ViewportIndex);
                        user.CheckStartMission();
                    }
                }
            });
        }

        private void UserMgr_UserLogined(long id)
        {
            this.InvokeEx(() =>
            {
                var user = Program.UserMgr.GetLoginUserInfo(id);
                bool judgeMode = user.LoginInfo.AdjudicatorMode;
                TreeListNode node = null;
                if (!RegisterUserTreeNodes.TryGetValue(user.AccountID, out node))
                    RefreshStudentsList();
                else
                {
                    node.ImageIndex = node.SelectImageIndex = GetUserSmallImageIndex(user.Info.SexMale, true, judgeMode);
                    node.SetValue(2, user.LinkEndPoint.Address);
                }
                if (user.LoginInfo.Node == null || judgeMode)
                    RefreshStudentGroupsList();
                else
                    user.LoginInfo.Node.ImageIndex = user.LoginInfo.Node.SelectImageIndex = GetUserSmallImageIndex(user.Info.SexMale, true, judgeMode);
                string msg = String.Format("用户<{0}>{1}登录。", user.Name, judgeMode ? "以评审模式" : "");
                MessageController.PushMessage(MessageLevel.提示, MessageType.登录连接, msg);
            });
        }

        private void UserMgr_UserLogouted(long id, LogoutReason reason)
        {
            this.InvokeEx(() =>
            {
                string action = "";
                switch (reason)
                {
                    case LogoutReason.主动退出:
                        action = "退出"; break;
                    case LogoutReason.超时踢出:
                        action = "超时被踢出"; break;
                    case LogoutReason.系统踢出:
                        action = "被踢出"; break;
                    case LogoutReason.异地登录:
                        action = "异地登录被踢出"; break;
                    case LogoutReason.更改用户:
                    case LogoutReason.服务退出:
                    default:
                        break;
                }
                var user = Program.UserMgr.GetRegisteredUserInfo(id);
                if (action.Length > 0)
                {
                    string msg = String.Format("用户{0}{1}。", user.Name, action);
                    MessageController.PushMessage(MessageLevel.提示, MessageType.登录连接, msg);
                }
                TreeListNode node = null;
                var suser = Program.UserMgr.GetLoginUserInfo(user.AccountID);
                if (RegisterUserTreeNodes.TryGetValue(user.AccountID, out node))
                {
                    node.ImageIndex = node.SelectImageIndex = GetUserSmallImageIndex(user.SexMale, false, false);
                    node.SetValue(2, "");
                }
                if (suser != null && suser.LoginInfo.Node != null)
                {
                    suser.LoginInfo.Node.ImageIndex = suser.LoginInfo.Node.SelectImageIndex = GetUserSmallImageIndex(user.SexMale, false, false);
                }
            });
        }

        public void RefreshJudgementTable()
        {
            //gdcJudgements.DataSource = Program.Option.JudgementSet.BuildTable(null);
        }

        public void RefreshGroupUserNodes()
        {

        }

        public Dictionary<long, TreeListNode> RegisterUserTreeNodes = new Dictionary<long, TreeListNode>();
        private int GetUserSmallImageIndex(bool male, bool online, bool judging)
        {
            if (online)
            {
                if (judging)
                    return 66;
                return male ? 40 : 42;
            }
            return male ? 41 : 43;
        }

        public void RefreshStudentsList()
        {
            trvStudents.Nodes.Clear();
            RegisterUserTreeNodes.Clear();
            MessageController.ClearStudentList();
            foreach (var user in Program.UserMgr.RegisteredUsers.Values)
            {
                var loginUser = Program.UserMgr.GetLoginUserInfo(user.AccountID);
                int imgIdx = GetUserSmallImageIndex(user.SexMale, loginUser.Status >= LoginStatus.登录中, loginUser.LoginInfo.AdjudicatorMode);
                var ip = "";
                if (loginUser != null && loginUser.LinkEndPoint != null)
                    ip = loginUser.LinkEndPoint.Address;
                var node = trvStudents.Nodes.Add(new object[] { user.AccountID, user.Name, ip });
                node.Tag = user;
                node.ImageIndex = node.SelectImageIndex = imgIdx;
                RegisterUserTreeNodes[user.AccountID] = node;
                if (loginUser.Status != LoginStatus.离线)
                    MessageController.AddStudent(user.AccountID, user.Name);
            }
        }

        public void RefreshStudentGroupsList()
        {
            trvStudentGroups.Nodes.Clear();
            var grps = Program.UserMgr.TaskGroups.Values.ToArray();
            foreach (var grp in grps)
            {
                string strVi = grp.ViewportIndex < 0 ? "" : (grp.ViewportIndex + 1).ToString();
                var gnode = trvStudentGroups.Nodes.Add(new object[] { grp.ToString(), grp.ProjectName, grp.TaskName, grp.SceneName, strVi });
                gnode.Tag = grp;
                grp.Node = gnode;
                foreach (var user in grp.Users.Values)
                {
                    var unode = gnode.Nodes.Add(new object[] { user.Name, "", "", "", "" });
                    unode.Tag = Program.UserMgr.GetLoginUserInfo(user.AccountID);
                    unode.ImageIndex = unode.SelectImageIndex = GetUserSmallImageIndex(user.Info.SexMale, user.Status >= LoginStatus.登录中, user.AdjudicatorMode);
                    user.Node = unode;
                    foreach (TaskGroupColumn col in Enum.GetValues(typeof(TaskGroupColumn)))
                    {
                        user.UpdateNode(col);
                    }
                }
                gnode.Expanded = true;
            }
            trvStudentGroups.BestFitColumns();
        }

        private void trvStudentGroups_DragDrop(object sender, DragEventArgs e)
        {
        }

        private void trvStudentGroups_DragOver(object sender, DragEventArgs e)
        {
            var pt = trvStudentGroups.PointToClient(new Point(e.X, e.Y));
            var info = trvStudentGroups.CalcHitInfo(pt);
            var user = info.Node == null ? null : info.Node.Tag as RegisteredUserInfo;
            if (user != null)
                e.Effect = DragDropEffects.None;
        }

        private void trvStudentGroups_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
        }

        private void trvStudentGroups_BeforeDragNode(object sender, DevExpress.XtraTreeList.BeforeDragNodeEventArgs e)
        {
            var user = e.Node.Tag as ServerLoginUserInfo;
            if (user == null)
                e.CanDrag = false;
        }

        private void trvStudentGroups_AfterDragNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            var user = e.Node.Tag as ServerLoginUserInfo;
            var pnode = e.Node.ParentNode;
            if (pnode == null || pnode.ParentNode != null)
                RefreshStudentGroupsList();
            else
            {
                var group = pnode.Tag as UserTaskGroup;
                if (!Program.UserMgr.SetUserTaskGroup(user.AccountID, group.GroupID))
                    RefreshStudentGroupsList();
            }
            RefreshUI();
        }

        private void iAddTaskGroup_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dlg = new InputDialog("添加任务分组", "分组名称：", "");
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (Program.UserMgr.GetTaskGroup(dlg.ResultText) != null)
                    MessageBox.Show(String.Format("已经存在名称为'{0}'的分组！", dlg.ResultText), "添加分组失败");
                else
                {
                    var grp = Program.UserMgr.CreateTaskGroup(dlg.ResultText);
                    RefreshStudentGroupsList();
                    foreach (TreeListNode node in trvStudentGroups.Nodes)
                    {
                        if (node.Tag == grp)
                        {
                            node.Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        private void iRemoveTaskGroup_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var grp = SelectedTaskGroup;
            if (grp == null)
                return;
            if (grp.OnlineUsers.Length > 0)
                MessageBox.Show("不能删除拥有在线用户的分组！");
            else if (MessageBox.Show("确定删除分组'" + grp.Name + "'吗？", "删除分组", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (Program.UserMgr.RemoveTaskGroup(grp.GroupID))
                    RefreshStudentGroupsList();
            }
        }

        private void iRenameTaskGroup_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var grp = SelectedTaskGroup;
            if (grp != null)
            {
                var dlg = new InputDialog("重命名任务分组", "分组名称：", grp.Name);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    grp.Name = dlg.ResultText;
                    RefreshStudentGroupsList();
                }
            }
        }

        private void UpdateStartMissionButton(bool enabled, bool starting, bool pausing)
        {
            icStartMission.Enabled = enabled;
            icStartMission.Checked = starting;
            if (!starting)
            {
                icStartMission.LargeImageIndex = 31;
                icStartMission.Caption = "开始训练";
                iStopMission.Enabled = false;
            }
            else if (pausing)
            {
                icStartMission.LargeImageIndex = 31;
                icStartMission.Caption = "继续训练";
                iStopMission.Enabled = true;
                icReplayMission.Checked = false;
            }
            else
            {
                icStartMission.LargeImageIndex = 37;
                icStartMission.Caption = "暂停训练";
                iStopMission.Enabled = true;
                icReplayMission.Checked = false;
            }
        }

        private void SelectTaskGroupProject(string projName)
        {
            var pr = CurFocusTreeNodeProjReferrencer;
            if (pr == null && projName == pr.ProjectName)
                return;
            m_EditViewOperater.RefreshAppProjectTitle();
            //ResetCurTaskGroupTasksToClients(pr as UserTaskGroup);

            if (pr is UserTaskGroup)
                Program.UserMgr.PushNotification(new UserNotification_SetTrainingProject(((UserTaskGroup)pr).GroupID, true, projName));
            else
                Program.UserMgr.PushNotification(new UserNotification_SetTrainingProject(((LoginUserInfo)pr).AccountID, false, projName));
            if (pr.ProjectInstance != null)
                ProjectDoc.Instance.CloseProject(pr.ProjectInstance);
            pr.ProjectName = projName;

            SelectTaskGroupProjectTask(null, null, -1);
            ProjectDoc.Instance.SelectedViewportInfo = null;
            if (pr.ProjectInstance == null ||
                pr.ProjectInstance.TaskInfos.Count == 0 ||
                pr.ProjectInstance.TaskInfos[0].SceneInfos.Count == 0)
            {
                ProjectDoc.Instance.SelectedSceneInfo = null;
                ProjectDoc.Instance.SelectedTaskInfo = null;
            }
            else
            {
                ProjectDoc.Instance.SelectedTaskInfo = pr.ProjectInstance.TaskInfos[0];
                ProjectDoc.Instance.SelectedSceneInfo = ProjectDoc.Instance.SelectedTaskInfo.SceneInfos[0];
            }
            m_EditViewOperater.RefreshProjectTree();
        }

        private void SelectTaskGroupProjectTask(string taskName, string sceneName, int viewportIndex)
        {
            var prSel = CurFocusTreeNodeProjReferrencer;
            if (prSel == null)
                return;
            prSel.TaskName = taskName;
            prSel.SceneName = sceneName;
            prSel.ViewportIndex = viewportIndex;
            // 没有指定任务场景则默认指定第一个任务的第一个场景
            if (!prSel.IsTaskViewSpecified)
                prSel.ResetDefaultTaskScene();
            Program.UserMgr.PushNotification(new UserNotification_SetTrainingMission(prSel));
            var doc = ProjectDoc.Instance;
            doc.SelectedProject = prSel.ProjectInstance;
            if (doc.SelectedViewportInfo != null)
            {
                doc.SelectedViewportInfo.CurTimeTick = 0;
                doc.SelectedViewportInfo.IsMaximized = true;
            }
            RefreshUI();
        }

        private void trvStudentGroups_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            RefreshUI();
            var pr = CurFocusTreeNodeProjReferrencer;
            if (pr == null)
                ProjectDoc.Instance.SelectedProject = null;
            else
                ProjectDoc.Instance.SelectedProject = pr.ProjectInstance;
            m_EditViewOperater.RefreshProjectTree();

            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null)
            {
                var selUser = SelectedUserInfo;
                if (selUser != null)
                {
                    vi.SetRenderingCreatorIDs(new long[] { selUser.AccountID });
                    //IsScoreDataViewEditable = selUser.IsOnline;
                    RefreshJudgementsScores(ref selUser.TaskScores);
                }
                else if (SelectedTaskGroup != null)
                {
                    vi.ClearRenderingCreatorID();
                    if (!SelectedTaskGroup.IsCooperation)
                        vi.AddRenderingCreatorID(long.MaxValue);
                    else
                    {
                        foreach (var u in SelectedTaskGroup.OnlineUsers)
                            vi.AddRenderingCreatorID(u.AccountID);
                    }
                    //IsScoreDataViewEditable = m_SelectedTaskGroup.ActiveUsers.Length > 0;
                    RefreshJudgementsScores(ref SelectedTaskGroup.TaskScores);
                }
            }
        }

        private void RefreshJudgementsScores(ref Dictionary<int, float> lst)
        {
            //int count = Program.Option.JudgementSet.MaxItemID;
            //if (lst == null)
            //    lst = new List<float>();
            //while (lst.Count > count)
            //    lst.RemoveAt(lst.Count - 1);
            //while (lst.Count < count)
            //    lst.Add(0);
            //ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            //{
            //    this.BeginInvokeEx(() =>
            //    {
            //        List<float> _lst = obj as List<float>;
            //        var tb = gdcJudgements.DataSource as System.Data.DataTable;
            //        float score = 0;
            //        gdcJudgements.SuspendLayout();
            //        foreach (System.Data.DataRow row in tb.Rows)
            //        {
            //            int id = (int)row["项目序号"];
            //            row["得分"] = _lst[id - 1];
            //            score += _lst[id - 1];
            //        }
            //        lblTotalScore.Text = score.ToString();
            //        gdcJudgements.ResumeLayout(true);
            //    });
            //}), lst);
        }

        private void trvStudentGroups_Leave(object sender, EventArgs e)
        {
            //IsSelectedTaskGroupPageVisible = false;
        }

        private void trvStudentGroups_Enter(object sender, EventArgs e)
        {
            //IsSelectedTaskGroupPageVisible = true;
        }

        private void icStudentList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowStudentList.Checked)
                dcpStudents.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpStudents.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icStudentGroups_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowStudentGroups.Checked)
                dcpStudentGroups.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpStudentGroups.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icPropertyBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowPropertyBar.Checked)
                dcpProperties.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpProperties.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icMessageBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowMessageBar.Checked)
                dcpMessages.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpMessages.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowResourceBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowResourceBar.Checked)
                dcpResources.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpResources.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowProjectBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowProjectBar.Checked)
                dcpProject.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpProject.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icSyncResources_ItemClick(object sender, ItemClickEventArgs e)
        {
            var pr = CurFocusTreeNodeProjReferrencer;
            if (pr == null)
                icSyncResources.Checked = false;
            else
            {
                if (!icSyncResources.Checked)
                    Program.ResSynchronizer.CancelRequest(pr);
                else
                {
                    bool succeeded = Program.ResSynchronizer.PushRequest(pr);
                    if (!succeeded)
                    {
                        icSyncResources.Checked = false;
                        MessageController.PushMessage(MessageLevel.警告, MessageType.文件传输, "该分组各成员资源已完整！");
                    }
                }
            }
        }

        private void icCooperateTask_ItemClick(object sender, ItemClickEventArgs e)
        {
            var grp = SelectedTaskGroup;
            if (grp != null)
            {
                if (!grp.IsCooperation)
                {
                    foreach (var user in grp.Users.Values)
                    {
                        if (user.IsStarted)
                        {
                            user.IsStarted = false;
                            user.IsPaused = false;
                            Program.UserMgr.PushNotification(new UserNotification_StartMission(user, false));
                        }
                        Program.UserMgr.MonitorGroupUsers(user);
                    }
                }
                grp.IsCooperation = icCooperateTask.Checked;
                grp.ProjectName = null;
                Program.UserMgr.PushNotification(new UserNotification_SetTrainingProject(grp.GroupID, true, ""));
            }
        }

        private void icStartMission_ItemClick(object sender, ItemClickEventArgs e)
        {
            icStartMission.Checked = true;
            var pr = CurFocusTreeNodeProjReferrencer;
            if (pr == null)
                return;
            var grp = pr as UserTaskGroup;
            if (!pr.IsStarted)
            {
                if (pr.SpecificViewportInfo == null)
                {
                    MessageBox.Show("请先为该组指定一个训练任务！");
                    UpdateStartMissionButton(true, false, false);
                    return;
                }
                if (pr.UserIDs.Length == 0)
                {
                    MessageBox.Show("请向该组添加至少一个组员！");
                    UpdateStartMissionButton(true, false, false);
                    return;
                }
                if (grp != null && grp.ActiveUsers.Length == 0)
                {
                    MessageBox.Show("请先等待该组组员上线！");
                    UpdateStartMissionButton(true, false, false);
                    return;
                }
                UpdateStartMissionButton(true, true, false);

                pr.IsStarted = true;
                pr.IsPaused = false;

                RefreshUI();

                trvStudentGroups_FocusedNodeChanged(trvStudentGroups, null);
                Program.UserMgr.PushNotification(new UserNotification_StartMission(pr, true));
                Program.UserMgr.MonitorGroupUsers(pr);

                Program.UserMgr.UserOperations.ResetOperations(pr.SpecificViewportInfo);
            }
            else if (!pr.IsPaused)
            {
                pr.IsPaused = true;
                UpdateStartMissionButton(true, true, true);
                Program.UserMgr.PushNotification(new UserNotification_PauseMission(pr, true));
            }
            else
            {
                pr.IsPaused = false;
                UpdateStartMissionButton(true, true, false);
                Program.UserMgr.PushNotification(new UserNotification_PauseMission(pr, false));
            }
        }

        private void iStopMission_ItemClick(object sender, ItemClickEventArgs e)
        {
            var pr = CurFocusTreeNodeProjReferrencer;
            pr.IsStarted = false;
            pr.IsPaused = false;
            RefreshUI();

            trvStudentGroups_FocusedNodeChanged(trvStudentGroups, null);
            Program.UserMgr.PushNotification(new UserNotification_StartMission(pr, false));
            Program.UserMgr.MonitorGroupUsers(pr);

            Program.UserMgr.UserOperations.ResetOperations(pr.SpecificViewportInfo);

            UpdateStartMissionButton(true, false, false);
            icReplayMission.Enabled = Program.UserMgr.UserOperations.Operations.Count > 0;
            iStopMission.Enabled = false;
        }

        private void trbProgress_ValueChanged(object sender, EventArgs e)
        {
            string m = "", s = "", ms = "";
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null)
            {
                int msv = (int)(vi.CurTimeTick * 1000);
                ms = (msv % 1000).ToString();
                s = ((msv / 1000) % 60).ToString();
                m = ((msv / 1000 / 60) % 60).ToString();
            }
            for (int i = m.Length; i < 2; ++i)
                m = "0" + m;
            for (int i = s.Length; i < 2; ++i)
                s = "0" + s;
            for (int i = ms.Length; i < 3; ++i)
                ms = "0" + ms;
            lblViewportTime.Text = String.Format("{0}:{1}:{2}", m, s, ms);
        }

        private float m_ProcessElapsedTime = 0;
        private float m_GcElapsedTime = 0;
        public void Tick(float elapsedTime)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            // 统计逻辑实例（合作组或非合作个人）
            var prs = new List<ProjectReferrencer>();
            foreach (var grp in Program.UserMgr.TaskGroups.Values)
            {
                if (grp.IsCooperation)
                    prs.Add(grp);
                else
                    prs.AddRange(grp.Users.Values.ToArray());
            }

            m_ProcessElapsedTime += elapsedTime;
            if (m_ProcessElapsedTime > 0.1F)
            {
                m_ProcessElapsedTime = 0;
                foreach (var pr in prs)
                {
                    if (pr.IsStarted)
                    {
                        Program.UserMgr.PushNotification(new UserNotification_SetCurViewportTime(pr));
                        if (pr.SpecificViewportInfo != null)
                        {
                            pr.UpdateNode(TaskGroupColumn.任务名称);
                            pr.UpdateNode(TaskGroupColumn.场景名称);
                            pr.ViewportIndex = pr.SpecificViewportInfo.ScreenIndex;
                        }
                    }
                }
                trbProgress.Enabled = vi != null;
                if (vi == null)
                {
                    trbProgress.Properties.Maximum = 100;
                    trbProgress.Value = 0;
                }
                else
                {
                    trbProgress.Properties.Maximum = (int)(vi.TimeLength * 1000);
                    trbProgress.Value = (int)(vi.CurTimeTick * 1000);
                }
            }
            var ei = ProjectDoc.Instance.SelectedElementInfo;
            if (ei != null && ei.TargetLocation.HasValue)
            {
                Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_Modify(CurFocusTreeNodeProjReferrencer, ei));
            }

            var req = Program.UserMgr.PopTaskChangeRequester();
            if (req != null)
            {
                switch (req.ReqType)
                {
                    case UserManager.ProjectReferrencerReq.RequestType.ChangeTask:
                        Program.UserMgr.PushNotification(new UserNotification_SetTrainingMission(req.User));
                        break;
                    case UserManager.ProjectReferrencerReq.RequestType.StartTask:
                        if (req.User.IsTaskViewSpecified)
                        {
                            req.User.IsStarted = true;
                            Program.UserMgr.PushNotification(new UserNotification_StartMission(req.User, true));
                        }
                        break;
                    case UserManager.ProjectReferrencerReq.RequestType.StopTask:
                        req.User.IsStarted = false;
                        Program.UserMgr.PushNotification(new UserNotification_StartMission(req.User, false));
                        break;
                }
                if (ProjectDoc.Instance.SelectedProject == req.User.ProjectInstance)
                {
                    if (ProjectDoc.Instance.SelectedViewportInfo != null)
                    {
                        ProjectDoc.Instance.SelectedViewportInfo.CurTimeTick = 0;
                        ProjectDoc.Instance.SelectedViewportInfo.IsMaximized = true;
                    }
                    RefreshUI();
                }
                string msg = String.Format("用户[{0}]已成功请求切换训练任务：{1}-{2}-{3}。",
                    req.User.Name, req.User.TaskName, req.User.SceneName, req.User.ViewportIndex);
                MessageController.PushMessage(MessageLevel.提示, MessageType.任务操作, msg);
            }

            if (Program.Option.GCIntervalTime > 0)
            {
                m_GcElapsedTime += elapsedTime;
                if (m_GcElapsedTime > Program.Option.GCIntervalTime)
                {
                    m_GcElapsedTime = 0;
                    GC.Collect();
                }
            }

            string log = AppLogger.Pick();
            if (!String.IsNullOrEmpty(log))
                MessageController.PushMessage(log + "\r\n");
        }

        private void iExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void iOption_ItemClick(object sender, ItemClickEventArgs e)
        {
            OptionsForm frm = new OptionsForm();
            frm.ShowDialog(this);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dockMgr.SaveLayoutToXml(Program.Option.LayoutConfigFile);
            Program.IsAppRunning = false;
        }

        private void iAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            var frm = new AboutBox();
            frm.ShowDialog();
        }

        private void iJudgement_ItemClick(object sender, ItemClickEventArgs e)
        {
            JudgementSetting dlg = new JudgementSetting();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                RefreshJudgementTable();
            }
        }

        private void icReplayMission_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void icReplayMission_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (icReplayMission.Checked)
                icStartMission.Checked = false;

            Program.UserMgr.UserOperations.Replay(CurFocusTreeNodeProjReferrencer, icReplayMission.Checked);
        }

        private void pnlMain_MouseUp(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseUp(e);

            if (icRemarkTool.Checked)
            {
                icRemarkTool.Checked = false;
                Program.Graphic.ConfirmCurveRibbon(5);
                Program.UserMgr.PushNotification(new UserNotification_CommitRemarkRibbon(CurAvailableProjReferrencer, 5));
            }
        }

        private void pnlMain_MouseDown(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseDown(e);
            if (icRemarkTool.Checked)
            {
                Program.Graphic.CreateCurveRibbon();
            }
        }

        Point m_LastRibbonPoint = new Point(0, 0);
        private void pnlMain_MouseMove(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseMove(e);

            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null && e.Button == MouseButtons.Left)
            {
                if (icRemarkTool.Checked && Program.Graphic.CreatingRibbon != null)
                {
                    int dx = Math.Abs(m_LastRibbonPoint.X - e.X);
                    int dy = Math.Abs(m_LastRibbonPoint.Y - e.Y);
                    if (dx > 1 || dy > 1)
                    {
                        m_LastRibbonPoint = e.Location;
                        var pos = vi.GetLocationRate(true, new PointF(e.X, e.Y));
                        Program.Graphic.CreatingRibbon.AppendPoint(pos.X, pos.Y);
                        Program.UserMgr.PushNotification(new UserNotification_DrawRemarkRibbon(CurAvailableProjReferrencer, pos.X, pos.Y));
                    }
                }
            }
        }

        private void pnlMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseDoubleClick(e);
            if (ProjectDoc.Instance.SelectedElementInfo != null && ProjectDoc.Instance.SelectedElementInfo.IsInnerEditable)
                return;
            var prSel = CurAvailableProjReferrencer;
            if (prSel != null && prSel.IsTaskSceneSpecified)
            {
                var s = prSel.ProjectInstance.SelectedTaskInfo.SelectedSceneInfo;
                if (s.SelectedViewportInfo != null)
                {
                    s.IsSelectedViewportMaximized = !s.IsSelectedViewportMaximized;
                }
            }
        }

        private void bbiResetLayout_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DefaultDockManagerLayoutData == null ||
                MessageBox.Show("确定恢复到默认面板布局吗？", "重置布局", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return; 
            using (var ms = new System.IO.MemoryStream(DefaultDockManagerLayoutData))
            {
                dockMgr.RestoreLayoutFromStream(ms);
            }
        }

        void rgbiSkins_GalleryItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e)
        {
            Program.Option.Save();
        }

        private void trvResources_MouseMove(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_MouseMove(e);
        }

        private void trvResources_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_MouseDoubleClick();
        }

        private void trvResources_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_FocusedNodeChanged(e);
        }

        private void trvResources_AfterFocusNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            Program.Graphic.PreviewResourceInfo = trvResources.FocusedNode.Tag as ResourceInfo;
        }

        private void trvResources_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_GiveFeedback(e);
        }

        private void pnlMain_DragDrop(object sender, DragEventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragDrop(e);
        }

        private void pnlMain_DragEnter(object sender, DragEventArgs e)
        {
            var pr = CurFocusTreeNodeProjReferrencer;
            if (pr == null || !pr.IsStarted)
                this.Cursor = Cursors.No;
            else
                m_EditViewOperater.OnPanelView_DragEnter(e);
        }

        private void pnlMain_DragLeave(object sender, EventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragLeave();
        }

        private void pnlMain_DragOver(object sender, DragEventArgs e)
        {
            var pr = CurFocusTreeNodeProjReferrencer;
            if (pr == null || !pr.IsStarted)
                this.Cursor = Cursors.No;
            else
                m_EditViewOperater.OnPanelView_DragEnter(e);
        }

        private void icMoveElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icMoveElement.Checked)
                icRotateElement.Checked = false;
        }

        private void icRotateElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icRotateElement.Checked)
                icMoveElement.Checked = false;
        }

        private void icScaleElement_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void icTargetMove_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void icAnimElement_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnChkBtnAnimElement_CheckedChanged(e);
        }

        private void iMoveElementUp_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnBtnClick_iMoveElementUp();
        }

        private void iMoveElementDown_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnBtnClick_iMoveElementDown();
        }

        private void iDeleteElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnBtnClick_iDeleteElement();
        }

        private void iCopyElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnBtnClick_iCopyElement();
        }

        private void iCutElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnBtnClick_iCutElement();
        }

        private void iPasteElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnBtnClick_iPasteElement();
        }

        private void trvProject_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            bool? selTask = null;
            bool? selScene = null;
            bool? selViewport = null;
            bool? selElement = null;
            m_EditViewOperater.OnProjectTree_FocusedNodeChanged(e, out selTask, out selScene, out selViewport, out selElement);
            //IsTaskNodeSelected = selTask.HasValue && selTask.Value;
            //IsSceneNodeSelected = selScene.HasValue && selScene.Value;
            //IsViewportNodeSelected = selViewport.HasValue && selViewport.Value;
            //IsElementNodeSelected = selElement.HasValue && selElement.Value;
        }

        private void appMenuSetGroupProject_BeforePopup(object sender, CancelEventArgs e)
        {
            var pr = CurFocusTreeNodeProjReferrencer;
            appMenuSetGroupProject.ItemLinks.Clear();
            foreach (var desc in ProjectDoc.Instance.ProjectDescs)
            {
                var bnItem = new BarCheckItem(barMgr, false);
                bnItem.Caption = desc.FileName;
                bnItem.Checked = pr.ProjectDescription != null && desc.FileName == pr.ProjectDescription.FileName;
                bnItem.ItemClick += new ItemClickEventHandler(appMenuSetGroupProject_ItemClick);
                appMenuSetGroupProject.ItemLinks.Add(bnItem);
            }
        }

        private void appMenuSetGroupProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.Item as BarCheckItem;
            SelectTaskGroupProject(item.Caption);
        }

        private void appMenuSetTaskGroup_BeforePopup(object sender, CancelEventArgs e)
        {
            appMenuSetTaskGroup.ItemLinks.Clear();
            foreach (var grp in Program.UserMgr.TaskGroups.Values.ToArray())
            {
                if (grp.GroupID > 0)
                {
                    var bnItem = new BarCheckItem(null, false);
                    bnItem.Caption = grp.Name;
                    bnItem.Checked = grp == SelectedTaskGroup;
                    bnItem.Tag = grp;
                    bnItem.ItemClick += new ItemClickEventHandler(appMenuSetTaskGroupItem_ItemClick);
                    appMenuSetTaskGroup.ItemLinks.Add(bnItem);
                }
            }
        }

        private void appMenuSetTaskGroupItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var grp = e.Item.Tag as UserTaskGroup;
            var item = e.Item as BarCheckItem;
            if (!item.Checked)
                item.Checked = true;
            else
                grp.Node.Selected = true;
        }

        private void appMenuSetGroupStudents_BeforePopup(object sender, CancelEventArgs e)
        {
            appMenuSetGroupStudents.ItemLinks.Clear();
            foreach (var grp in Program.UserMgr.TaskGroups.Values.ToArray())
            {
                if (grp == SelectedTaskGroup)
                    continue;
                var subMenu = new BarSubItem(barMgr, grp.Name);
                subMenu.Tag = grp;
                foreach (var user in grp.Users.Values)
                {
                    var bnItem = new BarCheckItem(barMgr, false);
                    bnItem.Caption = user.Name;
                    bnItem.Tag = user;
                    bnItem.ItemClick += new ItemClickEventHandler(appMenuSetGroupStudents_ItemClick);
                    subMenu.ItemLinks.Add(bnItem);
                }
                appMenuSetGroupStudents.ItemLinks.Add(subMenu);
            }
            if (SelectedTaskGroup != null)
            {
                foreach (var user in SelectedTaskGroup.Users.Values.ToArray())
                {
                    var bnItem = new BarCheckItem(barMgr, false);
                    bnItem.Caption = user.Name;
                    bnItem.Checked = true;
                    bnItem.Tag = user;
                    bnItem.ItemClick += new ItemClickEventHandler(appMenuSetGroupStudents_ItemClick);
                    appMenuSetGroupStudents.ItemLinks.Add(bnItem);
                }
            }
        }

        private void appMenuSetGroupStudents_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.Item as BarCheckItem;
            var user = item.Tag as LoginUserInfo;
            var selUser = SelectedUserInfo;
            var selGrp = SelectedTaskGroup;
            if (selGrp == null)
            {
                if (selUser == null)
                    return;
                selGrp = selUser.ParentGroup;
                if (selGrp == null || selGrp.Node == null)
                    return;
            }
            if (user.ParentGroup == selGrp)
                Program.UserMgr.SetUserTaskGroup(user.AccountID, 0);
            else
                Program.UserMgr.SetUserTaskGroup(user.AccountID, selGrp.GroupID);
            RefreshStudentGroupsList();
            selGrp.Node.Selected = true;
        }

        private void appMenuSetGroupTask_BeforePopup(object sender, CancelEventArgs e)
        {
            appMenuSetGroupTask.ItemLinks.Clear();
            var pr = CurFocusTreeNodeProjReferrencer;
            if (pr == null || pr.ProjectDescription == null)
                e.Cancel = true;
            else
            {
                foreach (var t in pr.ProjectDescription.Tasks)
                {
                    var subTaskMenu = new BarSubItem(barMgr, t.Name);
                    if (t.Name == pr.TaskName)
                        subTaskMenu.ImageIndex = 52;
                    foreach (var s in t.Scenes)
                    {
                        var subSceneMenu = new BarSubItem(barMgr, s.Name);
                        if (t.Name == pr.TaskName && s.Name == pr.SceneName)
                            subSceneMenu.ImageIndex = 52;
                        foreach (var v in s.Viewports)
                        {
                            var bnItem = new BarCheckItem(barMgr, false);
                            bnItem.Caption = v.Name;
                            bnItem.Checked = (t.Name == pr.TaskName && s.Name == pr.SceneName && v.Index == pr.ViewportIndex);
                            bnItem.Tag = v;
                            bnItem.ItemClick += new ItemClickEventHandler(appMenuSetGroupTask_ItemClick);
                            subSceneMenu.ItemLinks.Add(bnItem);
                        }
                        subTaskMenu.ItemLinks.Add(subSceneMenu);
                    }
                    appMenuSetGroupTask.ItemLinks.Add(subTaskMenu);
                }
            }
        }

        private void appMenuSetGroupTask_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.Item as BarCheckItem;
            var v = item.Tag as ViewportDesc;
            var s = v.ParentScene;
            var t = s.ParentTask;
            SelectTaskGroupProjectTask(t.Name, s.Name, v.Index);
        }

        private void icToolMask_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnChkBtnToolElement_Mask_ItemClicked();
        }

        private void icToolWaterbag_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnChkBtnToolElement_Waterbag_ItemClicked();
        }

        private void icToolTipText_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnChkBtnToolElement_TipText_ItemClicked();
        }

        private void ConfirmFreeTaskPermission()
        {
            FreeTaskPermission ft = FreeTaskPermission.禁止自由选择;
            if (!icCooperateTask.Enabled || !icCooperateTask.Checked)
            {
                if (icFreeSelViewport.Checked)
                    ft |= FreeTaskPermission.自由选择窗口;
                if (icFreeSelViewport.Checked && icFreeSelScenes.Checked)
                    ft |= FreeTaskPermission.自由选择场景;
                if (icFreeSelViewport.Checked && icFreeSelScenes.Checked && icFreeSelTasks.Checked)
                    ft |= FreeTaskPermission.自由选择任务;
            }
            CurAvailableProjReferrencer.FreeTaskPerm = ft;
            Program.UserMgr.PushNotification(new UserNotification_SetFreeTaskPermission(CurAvailableProjReferrencer));
        }
        private void icFreeSelTasks_ItemClick(object sender, ItemClickEventArgs e)
        {
            ConfirmFreeTaskPermission();
            RefreshUI();
        }

        private void icFreeSelScenes_ItemClick(object sender, ItemClickEventArgs e)
        {
            ConfirmFreeTaskPermission();
            RefreshUI();
        }

        private void icFreeSelViewport_ItemClick(object sender, ItemClickEventArgs e)
        {
            ConfirmFreeTaskPermission();
            RefreshUI();
        }

        private void grdProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_EditViewOperater.OnPropertyBar_PropertyValueChanged(e);
        }

        private void iDeleteUser_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoginUserInfo delUser = null;
            if (trvStudentGroups.Focused)
                delUser = SelectedUserInfo;
            else if (trvStudents.Focused)
            {
                var ruser = trvStudents.FocusedNode.Tag as RegisteredUserInfo;
                var suser = Program.UserMgr.GetLoginUserInfo(ruser.AccountID);
                if (suser != null)
                    delUser = suser.LoginInfo;
            }
            if (delUser != null)
            {
                if (delUser.ParentGroup.IsCooperation && delUser.ParentGroup.IsStarted)
                {
                    MessageBox.Show("删除学员前请先停止当前分组的任务训练。", "删除学员失败");
                    return;
                }
                if (MessageBox.Show(String.Format("确定要彻底删除学员'{0}({1})'吗？", delUser.Name, delUser.AccountID), "删除学员", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                if (Program.UserMgr.UnRegisterUser(delUser))
                {
                    RefreshStudentsList();
                    RefreshStudentGroupsList();
                }
            }
        }

        private void pnlMain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            m_EditViewOperater.OnPanelView_PreviewKeyDown(e);
        }

        private void icRemarkTool_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void beiPenThickness_EditValueChanged(object sender, EventArgs e)
        {
            ProjectDoc.Instance.DefaultCubicCurveRibbonWidth = float.Parse(beiPenThickness.EditValue as string);
        }

        private void beiPenColor_EditValueChanged(object sender, EventArgs e)
        {
            ProjectDoc.Instance.DefaultCubicCurveRibbonColor = CommonMethods.ConvertColor((System.Drawing.Color)beiPenColor.EditValue);
        }

        private void tmDogChecker_Tick(object sender, EventArgs e)
        {
            tmDogChecker.Enabled = false;
            if (!SecurityGrandDog.Instance.RunningCheck())
                Process.GetCurrentProcess().Kill();
            else
                tmDogChecker.Enabled = true;
        }
    }
}
