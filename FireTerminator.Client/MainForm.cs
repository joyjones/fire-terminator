using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FireTerminator.Common.UI;
using FireTerminator.Common.Services;
using FireTerminator.Common;
using DevExpress.XtraBars;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.Operations;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Structures;

namespace FireTerminator.Client
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MainForm()
        {
            Instance = this;
            if (Program.IsLoginJudgeMode)
                splashScreenMgr = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(SplashScreen_Judge), true, true);
            else
                splashScreenMgr = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(SplashScreen), true, true);
            InitializeComponent();
            DevExpress.XtraBars.Helpers.SkinHelper.InitSkinGallery(rgbiSkins, true);
            rgbiSkins.GalleryItemClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(rgbiSkins_GalleryItemClick);

            ProjectDoc.Instance.IsCooperatingEditMode = true;
            Program.Option.Load();
            Program.JudgementSet.Load();
            InitEditViewOperator();
            trvResources.Nodes.Clear();
            trvProject.Nodes.Clear();
            trvStudentGroup.Nodes.Clear();
            m_ViewportItems = new BarCheckItem[] { icViewport1, icViewport2, icViewport3, icViewport4 };
            OperationHistory.Instance.CurOperationChanged += new OperationHistory.Delegate_OnOperationChanged(RefreshOnCurOperationChanged);
            OperationHistory.Instance.NewOperationPushed += new OperationHistory.Delegate_OnOperationChanged(OperattionHistory_NewOperationPushed);
            OperationHistory.Instance.AfterUndoOperation += new OperationHistory.Delegate_OnOperationChanged(OperattionHistory_AfterOperationUndo);
            OperationHistory.Instance.AfterRedoOperation += new OperationHistory.Delegate_OnOperationChanged(OperattionHistory_NewOperationPushed);

            icAnimElement.Visibility = BarItemVisibility.Never;
            IsElementNodeSelected = false;
        }

        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenMgr;
        private EditViewOperater m_EditViewOperater = null;
        private byte[] DefaultDockManagerLayoutData = null;
        private BarCheckItem[] m_ViewportItems = null;
        public static MainForm Instance
        {
            get;
            private set;
        }
        public MessageControl MessageController
        {
            get;
            private set;
        }
        public LoginForm FrmLogin
        {
            get;
            private set;
        }
        public EditViewOperater ViewOperater
        {
            get { return m_EditViewOperater; }
        }
        public bool IsElementNodeSelected
        {
            set
            {
                icMoveElement.Enabled = value;
                icRotateElement.Enabled = value;
                icScaleElement.Enabled = value;
                icAnimElement.Enabled = false;// value;
                iMoveElementUp.Enabled = value;
                iMoveElementDown.Enabled = value;
                iCopyElement.Enabled = value;
                iCutElement.Enabled = value;
                iPasteElement.Enabled = m_EditViewOperater.CopiedElement != null || m_EditViewOperater.CuttedElement != null;
                iDeleteElement.Enabled = value;
            }
        }
        public int SelectedViewportCheckItemIndex
        {
            get
            {
                for (int i = 0; i < m_ViewportItems.Length; ++i)
                    if (m_ViewportItems[i].Checked)
                        return i;
                return -1;
            }
            set
            {
                for (int i = 0; i < m_ViewportItems.Length; ++i)
                    m_ViewportItems[i].Checked = i == value;
                var pr = Program.CurUser;
                Program.Service.MainChannel.RequestChangeTrainningTask(pr.TaskName, pr.SceneName, value);
            }
        }
        private TaskGroupMissionStatus m_CurStatus = TaskGroupMissionStatus.初始化;
        public TaskGroupMissionStatus CurStatus
        {
            get { return m_CurStatus; }
            set
            {
                if (m_CurStatus != value)
                {
                    m_CurStatus = value;
                    pnlMain.Enabled = m_CurStatus == TaskGroupMissionStatus.训练进行中;
                    icCurStatus.Caption = m_CurStatus.ToString();
                    if (m_CurStatus == TaskGroupMissionStatus.训练进行中)
                        icCurStatus.ItemAppearance.Disabled.ForeColor = Color.YellowGreen;
                    else
                        icCurStatus.ItemAppearance.Disabled.ForeColor = Color.Red;
                    if (m_CurStatus < TaskGroupMissionStatus.训练进行中)
                        beiStatusProgress.EditValue = 0;
                    RefreshStartMissionButton();
                    MessageController.PushMessage(MessageLevel.提示, MessageType.任务操作, "当前训练状态改变为：" + m_CurStatus.ToString());
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MessageController = new MessageControl();
            MessageController.Dock = DockStyle.Fill;
            dcpMessages.Controls.Add(MessageController);
            MessageController.SendMessage += new MessageControl.Delegate_SendMessage(MessageController_SendMessage);

            CurStatus = TaskGroupMissionStatus.等待指定训练项目;

            dockMgr.ClosedPanel += new DevExpress.XtraBars.Docking.DockPanelEventHandler(dockMgr_ClosedPanel);
            try
            {
                if (System.IO.File.Exists(Program.Option.LayoutConfigFile))
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        dockMgr.SaveLayoutToStream(ms);
                        DefaultDockManagerLayoutData = ms.ToArray();
                    }
                    dockMgr.RestoreLayoutFromXml(Program.Option.LayoutConfigFile);
                }
            }
            catch
            {
            }
            dockMgr.ActivePanel = dcpStudentGroups;

            RefreshTrainningUI();
            RefreshJudgementUI();
        }

        void dockMgr_ClosedPanel(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            if (e.Panel == dcpProject)
                icShowProjectBar.Checked = false;
            else if (e.Panel == dcpProperties)
                icShowPropertyBar.Checked = false;
            else if (e.Panel == dcpResources)
                icShowResourceBar.Checked = false;
            else if (e.Panel == dcpStudentGroups)
                icShowStudentGroup.Checked = false;
            else if (e.Panel == dcpMessages)
                icShowMessages.Checked = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Service.Logout(true);
            dockMgr.SaveLayoutToXml(Program.Option.LayoutConfigFile);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ShowLoginForm();
        }

        private void MessageController_SendMessage(long targetId, Color clr, string message)
        {
            Program.Service.MainChannel.SendMessage(targetId, clr, message);
        }

        private void InitEditViewOperator()
        {
            m_EditViewOperater = new EditViewOperater(this, pnlMain, pnlPreview);
            m_EditViewOperater.ChkBtn_AnimElement = icAnimElement;
            m_EditViewOperater.ChkBtn_MoveElement = icMoveElement;
            m_EditViewOperater.ChkBtn_RotateElement = icRotateElement;
            m_EditViewOperater.ChkBtn_ScaleElement = icScaleElement;
            m_EditViewOperater.ChkBtn_DriveElement = icTargetMove;
            m_EditViewOperater.ChkBtn_ToolElement_Mask = icToolMask;
            m_EditViewOperater.ChkBtn_ToolElement_Waterbag = icToolWaterbag;
            m_EditViewOperater.ChkBtn_ToolElement_TipText = icToolTipText;
            m_EditViewOperater.Btn_CopyElement = iCopyElement;
            m_EditViewOperater.Btn_CutElement = iCutElement;
            m_EditViewOperater.Btn_PasteElement = iPasteElement;
            m_EditViewOperater.ProjectTree = trvProject;
            m_EditViewOperater.ResourceTree = trvResources;
            m_EditViewOperater.TransCtrl = null;
            m_EditViewOperater.PropertyBar = grdProperties;
            m_EditViewOperater.Cursor_ToolTip = toolTipOnCursor;
            m_EditViewOperater.IconImageIndices[ResourceKind.图像] = new int[] { 26, 38 };
            m_EditViewOperater.IconImageIndices[ResourceKind.背景] = new int[] { 46, 38 };
            m_EditViewOperater.IconImageIndices[ResourceKind.效果] = new int[] { 33, 37 };
            m_EditViewOperater.IconImageIndices[ResourceKind.视频] = new int[] { 47, 48 };
            m_EditViewOperater.IconImageIndices[ResourceKind.音频] = new int[] { 55, 54 };
            m_EditViewOperater.IconImageIndices[ResourceKind.遮罩] = new int[] { 34, 38 };
            m_EditViewOperater.IconImageIndices[ResourceKind.水带] = new int[] { 34, 38 };
            m_EditViewOperater.IconImageIndices[ResourceKind.文本] = new int[] { 34, 38 };
            m_EditViewOperater.ForbiddingResourcePathNames = new List<string>()
            {
                "烟雾", "火焰", "泄漏", "爆炸"
            };
            m_EditViewOperater.ExtraIconImageIndices[0] = 7;
            m_EditViewOperater.ExtraIconImageIndices[1] = 51;
            m_EditViewOperater.ExtraIconImageIndices[2] = 52;
            m_EditViewOperater.ExtraIconImageIndices[3] = 53;
        }

        //private float m_LastSyncOperationBackImageScale = 0;
        private void OperattionHistory_NewOperationPushed(Operation opt)
        {
            var eo = opt as Operation_Element;
            if (Program.CurUser.IsStarted)
            {
                if (opt is Operation_Element_Create)
                {
                    eo.Element.CreatorId = ProjectDoc.Instance.CurEditUserID;
                    Program.Service.MainChannel.SetElementOperation_Create(new ElementCreateInfo(eo.Element));
                }
                if (opt is Operation_Element_Duplicate)
                {
                    var dopt = opt as Operation_Element_Duplicate;
                    dopt.DuplicatedElementInfo.CreatorId = ProjectDoc.Instance.CurEditUserID;
                    Program.Service.MainChannel.SetElementOperation_Create(new ElementCreateInfo(dopt.DuplicatedElementInfo));
                }
                else if (opt is Operation_Element_Delete)
                    Program.Service.MainChannel.SetElementOperation_Delete(eo.Element.GUID);
                else if (opt is Operation_Element_Drift)
                    Program.Service.MainChannel.SetElementDepthLevel(eo.Element.GUID, eo.Element.DepthLevel);
                else if (opt is Operation_Element_Flip)
                    Program.Service.MainChannel.SetElementFlipState(eo.Element.GUID, eo.Element.CurFlippingState);
                else if (opt is Operation_Element_ChangeProperty)
                {
                    var popt = opt as Operation_Element_ChangeProperty;
                    Program.Service.MainChannel.SetElementProperty(eo.Element.GUID, popt.PropertyName, popt.ValueNew);
                }
                //else if (opt is Operation_Element_ChangeLocation && ((Operation_Element_ChangeLocation)opt).Driving)
                //{
                //    var popt = opt as Operation_Element_ChangeLocation;
                //    Program.Service.MainChannel.SetElementTargetLocation(eo.Element.GUID, popt.LocationNew.X, popt.LocationNew.Y);
                //}
                else if (opt is Operation_Element_ChangeMaskInfo)
                    Program.Service.MainChannel.SetToolElementInfo_Mask(eo.Element.GUID, ((ElementInfo_Mask)eo.Element).CornerVectors);
                else if (opt is Operation_Element_ChangeWaterbagInfo)
                    Program.Service.MainChannel.SetToolElementInfo_Waterbag(eo.Element.GUID, ((ElementInfo_Waterbag)eo.Element).BagWidth, ((ElementInfo_Waterbag)eo.Element).JointVecs);
                else if (opt is Operation_Element_SetHotkeyAnimation)
                    Program.Service.MainChannel.SetElementHotkeyAnimation(eo.Element.GUID, (int)(((Operation_Element_SetHotkeyAnimation)eo).Key), ((Operation_Element_SetHotkeyAnimation)eo).Time);
                else
                {
                    var scale = eo.Element.ManualScaleOnSrcBackImage;
                    //if (eo.Element.ParentViewport.BackImage != null)
                    //{
                    //    float newScale = eo.Element.ParentViewport.BackImage.CurImageScale;
                    //    if (newScale != 0)
                    //    {
                    //        if (m_LastSyncOperationBackImageScale == 0)
                    //            m_LastSyncOperationBackImageScale = newScale;
                    //        else if (m_LastSyncOperationBackImageScale != newScale)
                    //        {
                    //            scale.Width *= newScale / m_LastSyncOperationBackImageScale;
                    //            scale.Height *= newScale / m_LastSyncOperationBackImageScale;
                    //        }
                    //    }
                    //}
                    Program.Service.MainChannel.SetElementOperation(eo.Element.GUID, eo.Element.BaseTrans, scale);
                }
            }
        }

        private void OperattionHistory_AfterOperationUndo(Operation opt)
        {
            var eo = opt as Operation_Element;
            if (Program.CurUser.IsStarted)
            {
                if (opt is Operation_Element_Create)
                    Program.Service.MainChannel.SetElementOperation_Delete(eo.Element.GUID);
                else if (opt is Operation_Element_Duplicate)
                    Program.Service.MainChannel.SetElementOperation_Delete(((Operation_Element_Duplicate)eo).DuplicatedElementInfo.GUID);
                else if (opt is Operation_Element_Delete)
                {
                    eo.Element.CreatorId = ProjectDoc.Instance.CurEditUserID;
                    Program.Service.MainChannel.SetElementOperation_Create(new ElementCreateInfo(eo.Element));
                }
                else
                {
                    OperattionHistory_NewOperationPushed(opt);
                }
            }
        }

        public void ShowLoginForm()
        {
            if (FrmLogin != null && FrmLogin.Visible)
                return;
            RecreateProject(null);
            Program.Service.Logout(false);
            RefreshStudentsGroupList();
            Program.CurUser.ProjectName = "";

            if (FrmLogin == null)
                FrmLogin = new LoginForm();
            //FrmLogin.Location = new Point(Left + Width / 2 - FrmLogin.Width / 2, Top + Height / 2 - FrmLogin.Height / 2);
            if (FrmLogin.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            Program.Service.UserEnter();
            RecreateProject(null);
            RefreshTitleCaption();
        }

        private int GetUserSmallImageIndex(bool male, bool online)
        {
            if (online)
                return male ? 41 : 43;
            return male ? 42 : 44;
        }

        private void TransDrawer_TransitionRangeSelectChanged(TransitionRange tr)
        {
            grdProperties.SelectedObject = null;
            if (tr != null)
                grdProperties.SelectedObject = tr.OwnerTrans;
            else if (ProjectDoc.Instance.SelectedElementInfo != null)
                grdProperties.SelectedObject = ProjectDoc.Instance.SelectedElementInfo;
        }

        public void RefreshTitleCaption()
        {
            if (Program.CurUser.Info == null)
                m_EditViewOperater.CustomAppTitleTailText = null;
            else
            {
                string tail = "";
                if (Program.CurUser.AdjudicatorMode)
                {
                    if (Program.CurUser.IsMoniteringCooperationGroup)
                        tail = String.Format(" [评审模式 - 监控分组：{0}]", -Program.CurUser.TargetMonitorUserOrGroupID);
                    else if (Program.CurUser.IsMoniteringNoCooperationUser)
                        tail = String.Format(" [评审模式 - 监控用户：{0}]", Program.CurUser.TargetMonitorUserOrGroupID);
                    else
                        tail = " [评审模式 - 未选择监控目标]";
                }
                m_EditViewOperater.CustomAppTitleTailText = Program.CurUser.Name + tail;
            }
            m_EditViewOperater.RefreshAppProjectTitle();
        }

        public void RefreshStudentsGroupList()
        {
            trvStudentGroup.Nodes.Clear();
            MessageController.ClearStudentList();
            if (Program.CurUser.Info == null || Program.CurUser.ParentGroup == null)
                return;
            var group = Program.CurUser.ParentGroup;
            foreach (var user in group.Users.Values)
            {
                var node = trvStudentGroup.Nodes.Add(new object[] { user.Name });
                node.Tag = user;
                node.ImageIndex = node.SelectImageIndex = GetUserSmallImageIndex(user.Info.SexMale, user.Status >= LoginStatus.在线);
                if (user.AccountID != Program.CurUser.AccountID)
                    MessageController.AddStudent(user.AccountID, user.Name);
            }
        }

        public void RefreshTrainningUI()
        {
            if (!String.IsNullOrEmpty(Program.CurUser.ProjectName))
                rbpCurProject.Text = Program.CurUser.ProjectName;
            else
                rbpCurProject.Text = "<训练项目>";
            rbpCurProject.Tag = Program.CurUser.ProjectName;
            if (Program.CurUser.ProjectInstance == null)
            {
                if (!Program.CurUser.AdjudicatorMode || Program.CurUser.TargetMonitorUserOrGroupID != 0)
                    CurStatus = TaskGroupMissionStatus.等待指定训练项目;
                else
                    CurStatus = TaskGroupMissionStatus.等待指定监控用户;
            }
            ibnCurGroup.Enabled = Program.CurUser.AdjudicatorMode;
            ibnCurGroup.ActAsDropDown = Program.CurUser.AdjudicatorMode;
            ibnCurGroup.ButtonStyle = Program.CurUser.AdjudicatorMode ? BarButtonStyle.DropDown : BarButtonStyle.Default;
            if (Program.CurUser.AdjudicatorMode)
            {
                ribbonPageGroup_CurGroup.Text = "监控目标";
                if (Program.CurUser.IsMoniteringCooperationGroup)
                {
                    var grp = Program.GetTaskGroup((int)Program.CurUser.TargetMonitorUserOrGroupID * -1);
                    if (grp == null)
                        ibnCurGroup.Caption = "<未知分组>";
                    else
                        ibnCurGroup.Caption = grp.Name;
                }
                else
                {
                    var user = Program.GetLoginUserInfo(Program.CurUser.TargetMonitorUserOrGroupID);
                    if (user == null)
                        ibnCurGroup.Caption = "<未知用户>";
                    else
                        ibnCurGroup.Caption = user.Name;
                }
                trvResources.Enabled = false;
                rbpEdit.Visible = false;
            }
            else
            {
                ribbonPageGroup_CurGroup.Text = "所在分组";
                if (Program.CurUser.ParentGroup == null)
                    ibnCurGroup.Caption = "<未指定>";
                else
                    ibnCurGroup.Caption = Program.CurUser.ParentGroup.Name;
                trvResources.Enabled = true;
                rbpEdit.Visible = true;
            }
            
            bool bFreeTask = (Program.CurUser.FreeTaskPerm & FreeTaskPermission.自由选择任务) == FreeTaskPermission.自由选择任务;
            if (!String.IsNullOrEmpty(Program.CurUser.TaskName))
                ibnCurTask.Caption = Program.CurUser.TaskName;
            else
                ibnCurTask.Caption = "<当前任务>";
            ibnCurTask.Tag = Program.CurUser.TaskName;
            ibnCurTask.ActAsDropDown = bFreeTask;
            ibnCurTask.ButtonStyle = bFreeTask ? BarButtonStyle.DropDown : BarButtonStyle.Default;
            ibnCurTask.Enabled = bFreeTask;

            bool bFreeScene = (Program.CurUser.FreeTaskPerm & FreeTaskPermission.自由选择场景) == FreeTaskPermission.自由选择场景;
            if (!String.IsNullOrEmpty(Program.CurUser.SceneName))
                ibnCurScene.Caption = Program.CurUser.SceneName;
            else
                ibnCurScene.Caption = "<当前场景>";
            ibnCurScene.Tag = Program.CurUser.SceneName;
            ibnCurScene.ActAsDropDown = bFreeScene;
            ibnCurScene.ButtonStyle = bFreeScene ? BarButtonStyle.DropDown : BarButtonStyle.Default;
            ibnCurScene.Enabled = bFreeScene;

            bool bFreeView = (Program.CurUser.FreeTaskPerm & FreeTaskPermission.自由选择窗口) == FreeTaskPermission.自由选择窗口;
            foreach (var v in m_ViewportItems)
            {
                v.Checked = (int)v.Tag == Program.CurUser.ViewportIndex;
                v.Enabled = bFreeView;
            }
            RefreshStartMissionButton();
        }

        private void RefreshStartMissionButton()
        {
            iStartMission.Visibility = Program.CurUser.AdjudicatorMode ? BarItemVisibility.Never : BarItemVisibility.Always;
            iStartMission.Enabled = Program.CurUser.FreeTaskPerm != FreeTaskPermission.禁止自由选择 && Program.CurUser.IsTaskViewSpecified;
            if (Program.CurUser.IsStarted)
            {
                iStartMission.Caption = "请求停止训练";
                iStartMission.LargeImageIndex = 39;
            }
            else
            {
                iStartMission.Caption = "请求开始训练";
                iStartMission.LargeImageIndex = 40;
            }
        }

        public void RecreateProject(string projName)
        {
            m_EditViewOperater.RefreshResourceTree(true);
            Program.CurUser.ProjectName = projName;
            Program.CurUser.TaskName = ibnCurTask.Caption;
            Program.CurUser.SceneName = ibnCurScene.Caption;
            Program.CurUser.ViewportIndex = SelectedViewportCheckItemIndex;
            RefreshTrainningUI();
            m_EditViewOperater.RefreshProjectTree();

            m_EditViewOperater.ConfirmSelectCurrentViewport();
        }

        public void OnTrainingProjectFileReady()
        {
            ProjectDoc.Instance.LoadProjectsDescriptions();
            if (!String.IsNullOrEmpty(m_WaitForReadyProjectName))
            {
                Program.CurUser.ProjectName = m_WaitForReadyProjectName;
                if (!String.IsNullOrEmpty(Program.CurUser.ProjectName))
                    m_WaitForReadyProjectName = null;
            }
            var desc = ProjectDoc.Instance.GetProjectDescription(Program.CurUser.ProjectName);
            var files = desc.MissingResourceFiles;
            Program.Service.MainChannel.ReportMissingResourceFiles(files);
            this.InvokeEx(() =>
            {
                if (files.Length > 0)
                {
                    CurStatus = TaskGroupMissionStatus.请求同步资源文件;
                    //RecreateProject(null);
                }
                else
                {
                    CurStatus = TaskGroupMissionStatus.等待训练开始;
                    RecreateProject(desc.FileName);
                }
            });
        }

        private void ichkImReady_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void trvResources_AfterFocusNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            Program.Graphic.PreviewResourceInfo = trvResources.FocusedNode.Tag as ResourceInfo;
        }

        public void RefreshOnCurOperationChanged(Operation opt)
        {
            iUndo.Enabled = OperationHistory.Instance.CanUndo;
            iRedo.Enabled = OperationHistory.Instance.CanRedo;
        }

        private void iUndo_ItemClick(object sender, ItemClickEventArgs e)
        {
            OperationHistory.Instance.Undo();
        }

        private void iRedo_ItemClick(object sender, ItemClickEventArgs e)
        {
            OperationHistory.Instance.Redo();
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

        private void icTargetMove_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void trvResources_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_GiveFeedback(e);
        }

        private void trvResources_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_FocusedNodeChanged(e);
        }

        private void trvResources_MouseMove(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_MouseMove(e);
        }

        private void trvResources_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_MouseDoubleClick();
        }

        private void trvProject_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            bool? selTask = null;
            bool? selScene = null;
            bool? selViewport = null;
            bool? selElement = null;
            m_EditViewOperater.OnProjectTree_FocusedNodeChanged(e, out selTask, out selScene, out selViewport, out selElement);
            if (selElement.HasValue)
                IsElementNodeSelected = selElement.Value;
        }

        private void pnlMain_DragDrop(object sender, DragEventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragDrop(e);
        }

        private void pnlMain_DragEnter(object sender, DragEventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragEnter(e);
        }

        private void pnlMain_DragLeave(object sender, EventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragLeave();
        }

        private void pnlMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseDoubleClick(e);
        }

        private void pnlMain_MouseDown(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseDown(e);
        }

        private void pnlMain_MouseMove(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseMove(e);
        }

        private void pnlMain_MouseUp(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseUp(e);
        }

        private void iChangeUser_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Ribbon.SelectedPage = rbpCurProject;
            this.BeginInvokeEx(() => ShowLoginForm());
        }

        private void iModifyPassword_ItemClick(object sender, ItemClickEventArgs e)
        {
            PassModifyForm frm = new PassModifyForm();
            frm.ShowDialog(this);
        }

        private void iExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void pnlPreview_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            m_EditViewOperater.OnPreviewPanel_GiveFeedback(e);
        }

        private void pnlPreview_MouseMove(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPreviewPanel_MouseMove(e);
        }

        private void iOption_ItemClick(object sender, ItemClickEventArgs e)
        {
            OptionsForm frm = new OptionsForm();
            frm.ShowDialog(this);
        }

        private void iAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            var frm = new AboutBox();
            frm.ShowDialog();
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

        private void icShowMessages_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowMessages.Checked)
                dcpMessages.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpMessages.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowPropertyBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowPropertyBar.Checked)
                dcpProperties.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpProperties.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowStudentGroup_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowStudentGroup.Checked)
                dcpResources.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpResources.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowResourceBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowResourceBar.Checked)
                dcpStudentGroups.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpStudentGroups.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowProjectBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowProjectBar.Checked)
                dcpProject.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpProject.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
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

        private void mnuProjTasks_BeforePopup(object sender, CancelEventArgs e)
        {
            mnuProjTasks.ItemLinks.Clear();
            var pr = Program.CurUser;
            if (pr == null || pr.ProjectDescription == null)
                e.Cancel = true;
            else
            {
                foreach (var t in pr.ProjectDescription.Tasks)
                {
                    var item = new BarCheckItem(barMgr, t.Name == pr.TaskName);
                    item.Caption = t.Name;
                    item.ItemClick += new ItemClickEventHandler(mnuProjTasks_ItemClick);
                    mnuProjTasks.ItemLinks.Add(item);
                }
            }
        }

        private void mnuProjTasks_ItemClick(object sender, ItemClickEventArgs e)
        {
            var pr = Program.CurUser;
            Program.Service.MainChannel.RequestChangeTrainningTask(e.Item.Caption, pr.SceneName, pr.ViewportIndex);
        }

        private void mnuTaskScenes_BeforePopup(object sender, CancelEventArgs e)
        {
            mnuTaskScenes.ItemLinks.Clear();
            var pr = Program.CurUser;
            if (pr == null || pr.ProjectDescription == null || String.IsNullOrEmpty(pr.TaskName))
                e.Cancel = true;
            else
            {
                var t = pr.ProjectDescription.GetTask(pr.TaskName);
                foreach (var s in t.Scenes)
                {
                    var item = new BarCheckItem(barMgr, s.Name == pr.SceneName);
                    item.Caption = s.Name;
                    item.ItemClick += new ItemClickEventHandler(mnuTaskScenes_ItemClick);
                    mnuTaskScenes.ItemLinks.Add(item);
                }
            }
        }

        private void mnuTaskScenes_ItemClick(object sender, ItemClickEventArgs e)
        {
            var pr = Program.CurUser;
            Program.Service.MainChannel.RequestChangeTrainningTask(pr.TaskName, e.Item.Caption, pr.ViewportIndex);
        }

        private void icViewport1_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectedViewportCheckItemIndex = 0;
        }

        private void icViewport2_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectedViewportCheckItemIndex = 1;
        }

        private void icViewport3_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectedViewportCheckItemIndex = 2;
        }

        private void icViewport4_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectedViewportCheckItemIndex = 3;
        }

        private void mnuGroupUsers_BeforePopup(object sender, CancelEventArgs e)
        {
            mnuGroupUsers.ItemLinks.Clear();
            var pr = Program.CurUser;
            if (pr == null || !pr.AdjudicatorMode)
                e.Cancel = true;
            else
            {
                foreach (var grp in Program.TaskGroups.Values)
                {
                    var subGroupMenu = new BarSubItem(barMgr, grp.Name);
                    if (grp.IsCooperation)
                    {
                        subGroupMenu.Tag = grp;
                        if (grp.GroupID == pr.TargetMonitorUserOrGroupID * -1)
                            subGroupMenu.ImageIndex = 56;
                        subGroupMenu.ItemClick += new ItemClickEventHandler(mnuGroupUsers_ItemClick);
                    }
                    else
                    {
                        foreach (var p in grp.Users.Values)
                        {
                            if (p.AccountID == pr.AccountID)
                                continue;
                            if (p.Status < LoginStatus.在线)
                                continue;
                            if (p.AdjudicatorMode)
                                continue;
                            var item = new BarCheckItem(barMgr, p.AccountID == pr.TargetMonitorUserOrGroupID);
                            item.Tag = p;
                            item.Caption = p.Name;
                            item.ItemClick += new ItemClickEventHandler(mnuGroupUsers_ItemClick);
                            subGroupMenu.ItemLinks.Add(item);
                        }
                    }
                    mnuGroupUsers.ItemLinks.Add(subGroupMenu);
                }
            }
        }

        private void mnuGroupUsers_ItemClick(object sender, ItemClickEventArgs e)
        {
            var pr = e.Item.Tag as ProjectReferrencer;
            if (pr != null)
            {
                if (pr is UserTaskGroup)
                    Program.Service.MainChannel.RequestMonitorGroupOrUser(((UserTaskGroup)pr).GroupID, true);
                else if (pr is LoginUserInfo)
                    Program.Service.MainChannel.RequestMonitorGroupOrUser(((LoginUserInfo)pr).AccountID, false);
            }
        }

        private void trvJudgement_AfterFocusNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            var item = e.Node.Tag as JudgeItem;
            lsvDeductions.Items.Clear();
            if (item != null)
            {
                foreach (var duc in item.Deductions)
                {
                    var lsi = new ListViewItem(new string[] { duc.Description, duc.DuductScore.ToString() });
                    lsvDeductions.Items.Add(lsi);
                }
            }
        }

        private void trvJudgement_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            var pr = Program.TargetJudgementPR;
            if (e.Column == trvJudgeCol_Score)
            {
                float scoreNow = 0, scoreTol = 0;
                var cls = e.Node.Tag as JudgeClass;
                var itm = e.Node.Tag as JudgeItem;
                if (cls != null)
                {
                    scoreTol = cls.Score;
                    if (pr != null)
                    {
                        foreach (var i in cls.Items)
                            scoreNow += pr.GetTaskScore(i.ID);
                    }
                }
                else if (itm != null)
                {
                    scoreTol = itm.Score;
                    if (pr != null)
                        scoreNow = pr.GetTaskScore(itm.ID);
                }

                e.CellText = String.Format("{0}/{1}", scoreNow, scoreTol);
            }
        }

        public void RefreshJudgementUI()
        {
            var pr = Program.TargetJudgementPR;
            trvJudgement.Nodes.Clear();
            foreach (var cls in Program.JudgementSet.JudgeClasses)
            {
                float scoreCls = 0;
                var cnode = trvJudgement.Nodes.Add(new object[] { cls.Name, 0 });
                foreach (var itm in cls.Items)
                {
                    float score = pr == null ? 0 : pr.GetTaskScore(itm.ID);
                    var inode = cnode.Nodes.Add(new object[] { itm.Name, score });
                    inode.Tag = itm;
                    scoreCls += score;
                }
                cnode.SetValue(1, scoreCls);
                cnode.Tag = cls;
                cnode.Expanded = true;
            }
            RefreshJudgementTitle();
        }

        public void RefreshJudgementTitle()
        {
            var pr = Program.TargetJudgementPR;
            float totalScore = 0;
            if (pr != null)
                totalScore = pr.TotalTaskScore;
            dcpJudgement.Text = String.Format("评审 - 总分:{0}", totalScore.ToString());
        }

        private void trvJudgement_ShowingEditor(object sender, CancelEventArgs e)
        {
            var lst = sender as DevExpress.XtraTreeList.TreeList;
            if (trvJudgeCol_Score.ColumnEdit != null)
            {
                bool ediable = Program.TargetJudgementPR != null && lst.FocusedNode.ParentNode != null;
                trvJudgeCol_Score.ColumnEdit.Properties.ReadOnly = !ediable;
                e.Cancel = !ediable;
            }
        }

        private void trvJudgement_CellValueChanging(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            var pr = Program.TargetJudgementPR;
            var itm = e.Node.Tag as JudgeItem;
            if (e.Column == trvJudgeCol_Score && pr != null && itm != null)
            {
                float fOldValue = pr.GetTaskScore(itm.ID);
                float fVal = 0;
                if (e.Value == null ||  !float.TryParse(e.Value as string, out fVal))
                    e.Value = fOldValue;
                else if (fVal < 0)
                    e.Value = 0.0F;
                else if (fVal > itm.Score)
                    e.Value = itm.Score;
            }
        }

        private void trvJudgement_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            var pr = Program.TargetJudgementPR;
            var itm = e.Node.Tag as JudgeItem;
            if (e.Column == trvJudgeCol_Score && pr != null && itm != null)
            {
                float fVal = 0;
                if (float.TryParse(e.Value as string, out fVal))
                {
                    pr.TaskScores[itm.ID] = fVal;
                    RefreshJudgementUI();
                }
            }
        }

        private void grdProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_EditViewOperater.OnPropertyBar_PropertyValueChanged(e);
        }

        private void pnlMain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            m_EditViewOperater.OnPanelView_PreviewKeyDown(e);
        }

        private void iStartMission_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Program.CurUser.FreeTaskPerm == FreeTaskPermission.禁止自由选择)
            {
                MessageBox.Show("你的权限不够。");
                return;
            }
            Program.Service.MainChannel.RequestStartOrStopMission(!Program.CurUser.IsStarted);
        }

        private void beiStatusProgress_EditValueChanged(object sender, EventArgs e)
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
            lblViewportTime.Caption = String.Format("{0}:{1}:{2}", m, s, ms);
        }

        private void tmTick_Tick(object sender, EventArgs e)
        {
            var elm = ProjectDoc.Instance.SelectedElementInfo;
            if (elm != null && elm.TargetLocation.HasValue)
            {
                Program.Service.MainChannel.SetElementOperation(elm.GUID, elm.BaseTrans, elm.ManualScaleOnSrcBackImage);
            }
        }
    }
}
