using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using FireTerminator.Common;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using System.Diagnostics;
using FireTerminator.Common.UI;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Operations;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.Structures;

namespace FireTerminator.Editor
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MainForm()
        {
            splashScreenMgr = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(SplashScreen), true, true);
            InitializeComponent();
            DevExpress.XtraBars.Helpers.SkinHelper.InitSkinGallery(rgbiSkins, true);
            rgbiSkins.GalleryItemClick += new GalleryItemClickEventHandler(rgbiSkins_GalleryItemClick);

            try
            {
                Program.Option.Load();
                InitEditViewOperator();
                m_EditViewOperater.RefreshResourceTree(false);
                ProjectDoc.Instance.LoadProjectsDescriptions();
                ProjectDoc.Instance.CreateProject(false);
                m_EditViewOperater.RefreshProjectTree();
                m_EditViewOperater.ConfirmSelectCurrentViewport();

                ststrbViewScale.Edit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(ststrbViewScale_EditValueChanging);
                OperationHistory.Instance.CurOperationChanged += new OperationHistory.Delegate_OnOperationChanged(RefreshOnCurOperationChanged);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        void rgbiSkins_GalleryItemClick(object sender, GalleryItemClickEventArgs e)
        {
            Program.Option.Save();
        }

        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenMgr;
        private FireTerminator.Common.UI.TransitionController tcTransCtrl;
        private EditViewOperater m_EditViewOperater = null;
        private byte[] DefaultDockManagerLayoutData = null;

        public bool IsSelectedTaskScenePageVisible
        {
            get { return rbpcSelection.Visible; }
            set
            {
                rbpcSelection.Visible = value;
                if (value)
                    Ribbon.SelectedPage = rbpProject;
            }
        }
        public bool IsSceneNodeSelected
        {
            set
            {
                if (value)
                    IsSelectedTaskScenePageVisible = true;
                if (value)
                {
                    iCopyTask.Enabled = false;
                    iDeleteTask.Enabled = false;
                    iTaskMoveUp.Enabled = false;
                    iTaskMoveDown.Enabled = false;
                    iRenameTask.Enabled = false;
                    iCopyScene.Enabled = true;
                    iCutScene.Enabled = true;
                    iPasteScene.Enabled = m_CopyingSceneInfo != null || m_CuttingSceneInfo != null;
                    iDeleteScene.Enabled = ProjectDoc.Instance.SelectedTaskInfo.SceneInfos.Count > 1;
                    iSceneMoveUp.Enabled = true;
                    iSceneMoveDown.Enabled = true;
                    iRenameScene.Enabled = true;
                }
            }
        }
        public bool IsTaskNodeSelected
        {
            set
            {
                if (value)
                    IsSelectedTaskScenePageVisible = true;
                if (value)
                {
                    iCopyTask.Enabled = true;
                    iDeleteTask.Enabled = ProjectDoc.Instance.SelectedProject.TaskInfos.Count > 1;
                    iTaskMoveUp.Enabled = true;
                    iTaskMoveDown.Enabled = true;
                    iRenameTask.Enabled = true;
                    iCopyScene.Enabled = false;
                    iCutScene.Enabled = false;
                    iPasteScene.Enabled = m_CopyingSceneInfo != null || m_CuttingSceneInfo != null;
                    iDeleteScene.Enabled = false;
                    iSceneMoveUp.Enabled = false;
                    iSceneMoveDown.Enabled = false;
                    iRenameScene.Enabled = false;
                }
            }
        }
        public bool IsViewportNodeSelected
        {
            set
            {
                if (value)
                    IsSelectedTaskScenePageVisible = false;
                iCopyViewport.Enabled = value;
                iCutViewport.Enabled = value;
                iClearViewport.Enabled = value;
                iPasteViewport.Enabled = value && (m_EditViewOperater.CopiedViewport != null || m_EditViewOperater.CuttedViewport != null);
                icPasteViewIncludeAnim.Enabled = iPasteViewport.Enabled;
                icPasteViewIncludeBack.Enabled = iPasteViewport.Enabled;
                var vi = ProjectDoc.Instance.SelectedViewportInfo;
                icViewportMaximium.Checked = vi != null && vi.IsMaximized;
                var si = ProjectDoc.Instance.SelectedSceneInfo;
                icShowViewport1.Checked = si != null && si.Viewports[0] != null && si.Viewports[0].IsVisible;
                icShowViewport2.Checked = si != null && si.Viewports[1] != null && si.Viewports[1].IsVisible;
                icShowViewport3.Checked = si != null && si.Viewports[2] != null && si.Viewports[2].IsVisible;
                icShowViewport4.Checked = si != null && si.Viewports[3] != null && si.Viewports[3].IsVisible;
            }
        }
        public bool IsElementNodeSelected
        {
            set
            {
                if (value)
                    IsSelectedTaskScenePageVisible = false;
                icMoveElement.Enabled = value;
                icRotateElement.Enabled = value;
                icScaleElement.Enabled = value;
                iMoveElemUp.Enabled = value;
                iMoveElemDown.Enabled = value;
                iCopyElement.Enabled = value;
                iCutElement.Enabled = value;
                iPasteElement.Enabled = m_EditViewOperater.CopiedElement != null || m_EditViewOperater.CuttedElement != null;
                iDeleteElem.Enabled = value;
                icAnimElement.Enabled = value;
                icCombineElement.Enabled = value;
                iBreakElementGroup.Enabled = value && ProjectDoc.Instance.SelectedElementInfo != null && ProjectDoc.Instance.SelectedElementInfo.ParentElementGroup != null;
                iSeperateElementFromGroup.Enabled = iBreakElementGroup.Enabled;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tcTransCtrl = new FireTerminator.Common.UI.TransitionController();
            tcTransCtrl.Dock = DockStyle.Fill;
            this.dockPanel3_Container.Controls.Add(tcTransCtrl);
            if (Program.Graphic != null)
            {
                Program.Graphic.TransDrawer = tcTransCtrl.Drawer;
                tcTransCtrl.Drawer.TransitionRangeSelectChanged += new TransitionGraphics.Delegate_OnTransitionRangeSelectChanged(TransDrawer_TransitionRangeSelectChanged);
            }
            m_EditViewOperater.TransCtrl = tcTransCtrl;
            ProjectDoc.Instance.HideSelectionOnPlayingAnimation = true;

            m_EditViewOperater.RefreshAppProjectTitle();
            RefreshOnCurOperationChanged(null);
            
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
        }

        void dockMgr_ClosedPanel(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            if (e.Panel == dcpProject)
                icShowProjectBar.Checked = false;
            else if (e.Panel == dcpProperties)
                icShowPropertyBar.Checked = false;
            else if (e.Panel == dcpResources)
                icShowResourceBar.Checked = false;
            else if (e.Panel == dcpAnimation)
                icShowAnimationBar.Checked = false;
        }

        private void trvProject_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            bool? selTask = null;
            bool? selScene = null;
            bool? selViewport = null;
            bool? selElement = null;
            m_EditViewOperater.OnProjectTree_FocusedNodeChanged(e, out selTask, out selScene, out selViewport, out selElement);
            bool bSelViewport = selViewport.HasValue && selViewport.Value;
            if (!bSelViewport)
            {
                IsTaskNodeSelected = selTask.HasValue && selTask.Value;
                IsSceneNodeSelected = selScene.HasValue && selScene.Value;
            }
            IsViewportNodeSelected = bSelViewport;
            IsElementNodeSelected = selElement.HasValue && selElement.Value;
        }

        private void trvResources_MouseMove(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnResourceTree_MouseMove(e);
        }

        private void trvResources_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
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

        private void pnlMain_DragEnter(object sender, DragEventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragEnter(e);
        }

        private void pnlMain_DragOver(object sender, DragEventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragEnter(e);
        }

        private void pnlMain_DragDrop(object sender, DragEventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragDrop(e);
        }

        private void pnlMain_DragLeave(object sender, EventArgs e)
        {
            m_EditViewOperater.OnPanelView_DragLeave();
        }

        void ststrbViewScale_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            int value = (int)e.NewValue;
            ProjectDoc.Instance.SelectedViewportInfo.ViewScale = value * 0.01F;
            ststrbViewScale.Caption = value.ToString() + "%";
        }

        private void pnlMain_MouseDown(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseDown(e);
        }

        private void pnlMain_MouseMove(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseMove(e);
        }

        private void pnlMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseDoubleClick(e);
        }

        private void pnlMain_MouseUp(object sender, MouseEventArgs e)
        {
            m_EditViewOperater.OnPanelView_MouseUp(e);
        }

        #region 元素操作
        private void icViewportMaximium_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var si = ProjectDoc.Instance.SelectedSceneInfo;
            if (si != null)
                si.IsSelectedViewportMaximized = icViewportMaximium.Checked;
        }

        private void OnicShowViewport_CheckedChanged(int index, BarCheckItem ctrl)
        {
            var si = ProjectDoc.Instance.SelectedSceneInfo;
            if (si != null && si.Viewports[index] != null)
            {
                if (si.Viewports[index].IsMaximized)
                    ctrl.Checked = true;
                else
                    si.Viewports[index].IsVisible = ctrl.Checked;
            }
        }
        private void icShowViewport1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnicShowViewport_CheckedChanged(0, sender as BarCheckItem);
        }

        private void icShowViewport2_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnicShowViewport_CheckedChanged(1, sender as BarCheckItem);
        }

        private void icShowViewport3_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnicShowViewport_CheckedChanged(2, sender as BarCheckItem);
        }

        private void icShowViewport4_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OnicShowViewport_CheckedChanged(3, sender as BarCheckItem);
        }
        private void icMoveElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icMoveElement.Checked)
                icRotateElement.Checked = false;
        }

        private void icScaleElement_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void icRotateElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icRotateElement.Checked)
                icMoveElement.Checked = false;
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

        #endregion
        #region 任务操作
        private void iNewTask_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ProjectDoc.Instance.SelectedProject.CreateTask() != null)
                m_EditViewOperater.RefreshProjectTree();
        }

        private void iCopyTask_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ti = new TaskInfo(ProjectDoc.Instance.SelectedTaskInfo);
            ProjectDoc.Instance.SelectedProject.AddTask(ti);
            m_EditViewOperater.RefreshProjectTree();
        }

        private void iDeleteTask_ItemClick(object sender, ItemClickEventArgs e)
        {
            var task = ProjectDoc.Instance.SelectedTaskInfo;
            string msg = String.Format("确定删除任务“{0}”及其下所有场景吗？操作将不可恢复。", task.Name);
            if (MessageBox.Show(msg, "删除任务", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (ProjectDoc.Instance.SelectedTaskInfo.Remove())
                    m_EditViewOperater.RefreshProjectTree();
            }
        }

        private void iTaskMoveUp_ItemClick(object sender, ItemClickEventArgs e)
        {
            var task = ProjectDoc.Instance.SelectedTaskInfo;
            var proj = task.ParentProjectInfo;
            if (proj.BringTaskEarly(task))
                m_EditViewOperater.RefreshProjectTree();
        }

        private void iTaskMoveDown_ItemClick(object sender, ItemClickEventArgs e)
        {
            var task = ProjectDoc.Instance.SelectedTaskInfo;
            var proj = task.ParentProjectInfo;
            if (proj.BringTaskDelay(task))
                m_EditViewOperater.RefreshProjectTree();
        }
        private void iRenameTask_ItemClick(object sender, ItemClickEventArgs e)
        {
            var task = ProjectDoc.Instance.SelectedTaskInfo;
            var dlg = new InputDialog("重命名任务", "任务名称：", task.Name);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                task.Name = dlg.ResultText;
                m_EditViewOperater.RefreshProjectTree();
            }
        }
        #endregion
        #region 场景操作
        private void iNewScene_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ProjectDoc.Instance.SelectedTaskInfo.CreateScene() != null)
                m_EditViewOperater.RefreshProjectTree();
        }

        private SceneInfo m_CopyingSceneInfo = null;
        private SceneInfo m_CuttingSceneInfo = null;
        private void iCopyScene_ItemClick(object sender, ItemClickEventArgs e)
        {
            var si = ProjectDoc.Instance.SelectedSceneInfo;
            if (si != null)
            {
                m_CuttingSceneInfo = null;
                m_CopyingSceneInfo = si;
            }
        }

        private void iCutScene_ItemClick(object sender, ItemClickEventArgs e)
        {
            var si = ProjectDoc.Instance.SelectedSceneInfo;
            if (si != null)
            {
                m_CopyingSceneInfo = null;
                m_CuttingSceneInfo = si;
            }
        }

        private void iPasteScene_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ti = ProjectDoc.Instance.SelectedTaskInfo;
            SceneInfo si = null;
            if (m_CopyingSceneInfo != null)
                si = new SceneInfo(m_CopyingSceneInfo);
            else
                si = m_CuttingSceneInfo;
            ti.AddScene(si);
            if (m_CuttingSceneInfo != null)
            {
                m_CuttingSceneInfo = null;
                iPasteScene.Enabled = false;
            }
            m_EditViewOperater.RefreshProjectTree();
        }

        private void iDeleteScene_ItemClick(object sender, ItemClickEventArgs e)
        {
            var scene = ProjectDoc.Instance.SelectedSceneInfo;
            string msg = String.Format("确定删除场景“{0}”及其下所有内容吗？此操作将不可恢复。", scene.Name);
            if (MessageBox.Show(msg, "删除场景", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (ProjectDoc.Instance.SelectedSceneInfo.Remove())
                    m_EditViewOperater.RefreshProjectTree();
            }
        }

        private void iSceneMoveUp_ItemClick(object sender, ItemClickEventArgs e)
        {
            var scene = ProjectDoc.Instance.SelectedSceneInfo;
            var task = scene.ParentTaskInfo;
            if (task.BringSceneEarly(scene))
                m_EditViewOperater.RefreshProjectTree();
        }

        private void iSceneMoveDown_ItemClick(object sender, ItemClickEventArgs e)
        {
            var scene = ProjectDoc.Instance.SelectedSceneInfo;
            var task = scene.ParentTaskInfo;
            if (task.BringSceneDelay(scene))
                m_EditViewOperater.RefreshProjectTree();
        }

        private void iRenameScene_ItemClick(object sender, ItemClickEventArgs e)
        {
            var scene = ProjectDoc.Instance.SelectedSceneInfo;
            var dlg = new InputDialog("重命名场景", "场景名称：", scene.Name);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                scene.Name = dlg.ResultText;
                m_EditViewOperater.RefreshProjectTree();
            }
        }
        #endregion

        private void iUndo_ItemClick(object sender, ItemClickEventArgs e)
        {
            OperationHistory.Instance.Undo();
        }

        private void iRedo_ItemClick(object sender, ItemClickEventArgs e)
        {
            OperationHistory.Instance.Redo();
        }

        private void iSaveProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            iProjectSave_ItemClick(sender, null);
        }

        private void iPlayAnimation_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            ProjectDoc.Instance.IsProjectAnimationPlaying = iPlayAnimation.Checked;
            trvProject.Enabled = !ProjectDoc.Instance.IsProjectAnimationPlaying;
            trvResources.Enabled = !ProjectDoc.Instance.IsProjectAnimationPlaying;
            dcpProject.Text = trvProject.Enabled ? "项目" : "项目 (播放中)";
            dcpResources.Text = trvResources.Enabled ? "资源" : "资源 (播放中)";
        }

        private void TransDrawer_TransitionRangeSelectChanged(TransitionRange tr)
        {
            grdProperties.SelectedObject = null;
            if (tr != null)
                grdProperties.SelectedObject = tr.OwnerTrans;
            else if (ProjectDoc.Instance.SelectedElementInfo != null)
                grdProperties.SelectedObject = ProjectDoc.Instance.SelectedElementInfo;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckSaveCurrentProject(true))
                e.Cancel = true;
            else
                dockMgr.SaveLayoutToXml(Program.Option.LayoutConfigFile);
        }

        private void iAddElemMask_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void iOptions_ItemClick(object sender, ItemClickEventArgs e)
        {
            OptionsForm frm = new OptionsForm();
            frm.ShowDialog();
        }

        private void bvItemExportProjResources_ItemClick(object sender, BackstageViewItemEventArgs e)
        {
        }
        private void icFullScreenPlay_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            //Keys code = Keys.P;
            //API.SendMessage(this.Handle.ToInt32(), API.WM_KEYDOWN, (int)code, (int)code);
            //API.SendMessage(this.Handle.ToInt32(), API.WM_KEYUP, (int)code, 0);
            //System.Threading.Thread.Sleep(2000);
            //System.Windows.Forms.SendKeys.Send("{PRTSC}");
        }

        private void iCopyViewport_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.CopiedViewport = ProjectDoc.Instance.SelectedViewportInfo;
            m_EditViewOperater.CuttedViewport = null;
            IsViewportNodeSelected = ProjectDoc.Instance.SelectedViewportInfo.TreeNode.Selected;
        }

        private void iClearViewport_ItemClick(object sender, ItemClickEventArgs e)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null && vi.Elements.Count > 0)
            {
                OperationHistory.Instance.BeginTransaction();
                var elms = vi.Elements.ToArray();
                foreach (var elm in elms)
                {
                    if (elm != null)
                        OperationHistory.Instance.CommitOperation(new Operation_Element_Delete(elm));
                }
                m_EditViewOperater.CuttedViewport = null;
                OperationHistory.Instance.EndTransaction();
                //RefreshProjectTree();
            }
        }

        private void iCutViewport_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.CuttedViewport = ProjectDoc.Instance.SelectedViewportInfo;
            m_EditViewOperater.CopiedViewport = null;
            IsViewportNodeSelected = ProjectDoc.Instance.SelectedViewportInfo.TreeNode.Selected;
        }

        private void iPasteViewport_ItemClick(object sender, ItemClickEventArgs e)
        {
            var viFrom = m_EditViewOperater.CopiedViewport;
            if (viFrom == null && m_EditViewOperater.CuttedViewport != null)
                viFrom = m_EditViewOperater.CuttedViewport;
            if (viFrom == null)
                return;
            var viTo = ProjectDoc.Instance.SelectedViewportInfo;
            if (viFrom == viTo)
                return;
            OperationHistory.Instance.BeginTransaction();
            // 清空目标窗口元素
            var elms = viTo.Elements.ToArray();
            foreach (var elm in elms)
            {
                if (elm != null)
                {
                    if (!icPasteViewIncludeBack.Checked && elm is ElementInfo_BackgroundImage)
                        continue;
                    OperationHistory.Instance.CommitOperation(new Operation_Element_Delete(elm));
                }
            }
            // 复制源窗口元素到目标窗口
            elms = viFrom.Elements.ToArray();
            foreach (var elm in elms)
            {
                if (elm != null)
                {
                    if (!icPasteViewIncludeBack.Checked && elm is ElementInfo_BackgroundImage)
                        continue;
                    OperationHistory.Instance.CommitOperation(new Operation_Element_Duplicate(elm, viTo));
                }
            }
            if (!icPasteViewIncludeAnim.Checked)
            {
                foreach (var ei in viTo.Elements)
                {
                    if (ei != null)
                        ei.ClearAllTransitions();
                }
            }
            // 剪切模式时删除源窗口元素
            if (viFrom == m_EditViewOperater.CuttedViewport)
            {
                elms = viFrom.Elements.ToArray();
                foreach (var elm in elms)
                {
                    if (elm != null)
                        OperationHistory.Instance.CommitOperation(new Operation_Element_Delete(elm));
                }
                m_EditViewOperater.CuttedViewport = null;
            }
            OperationHistory.Instance.EndTransaction();
        }

        private bool CheckSaveCurrentProject(bool bCheckModify)
        {
            if (!OperationHistory.Instance.IsDirty)
                return true;
            if (bCheckModify)
            {
                var rst = MessageBox.Show("保存对当前项目的修改吗？", "项目未保存", MessageBoxButtons.YesNoCancel);
                if (rst == DialogResult.Cancel)
                    return false;
                if (rst == DialogResult.No)
                    return true;
            }

            iProjectSave_ItemClick(null, null);
            return true;
        }

        private void iProjectNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            ResetProject();
        }

        private void iProjectSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!ProjectDoc.Instance.IsSelectedProjectFileOpened)
                iProjectSaveAs_ItemClick(sender, e);
            else// if (OperationHistory.Instance.IsDirty)
                ProjectDoc.Instance.SelectedProject.Save();
        }

        private void iProjectOpen_ItemClick(object sender, ItemClickEventArgs e)
        {
            BeforeResetProject();
            if (CheckSaveCurrentProject(true))
            {
                if (dlgOpenProject.ShowDialog() == DialogResult.OK)
                {
                    ProjectDoc.Instance.SelectedProject.Load(dlgOpenProject.FileName);
                    m_EditViewOperater.RefreshProjectTree();
                    m_EditViewOperater.RefreshAppProjectTitle();
                    m_EditViewOperater.ConfirmSelectCurrentViewport();
                    RefreshOnCurOperationChanged(null);
                    AfterResetProject();
                }
            }
        }

        private void iProjectSaveAs_ItemClick(object sender, ItemClickEventArgs e)
        {
            BeforeResetProject();
            if (dlgSaveProject.ShowDialog() == DialogResult.OK)
            {
                ProjectDoc.Instance.SelectedProject.SaveAs(dlgSaveProject.FileName);
                m_EditViewOperater.RefreshAppProjectTitle();
            }
        }

        private void iProjectExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dlgBrowseFolder.ShowDialog() == DialogResult.OK)
            {
                ProjectDoc.Instance.SelectedProject.ExportResourceFiles(dlgBrowseFolder.SelectedPath);
            }
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

        private void grdProperties_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void iAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            AboutBox frm = new AboutBox();
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

        private void iRefreshResourceList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ProjectDoc.Instance.IsSelectedProjectFileOpened || OperationHistory.Instance.IsDirty)
            {
                if (MessageBox.Show("刷新资源列表必须重置当前项目。", "刷新资源", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return;
                if (!CheckSaveCurrentProject(true))
                    return;
            }
            m_EditViewOperater.RefreshResourceTree(true);
            ResetProject();
        }

        private void iProjectClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            ResetProject();
        }

        string PropertyCellChangeDenying_ViewportName = null;
        private void grdProperties_CellValueChanging(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            var si = ProjectDoc.Instance.SelectedSceneInfo;
            if (si != null && grdProperties.SelectedObject == si.SelectedViewportInfo)
            {
                if (e.Row.Properties.Caption == "名称")
                {
                    string newName = e.Value as string;
                    foreach (var v in si.Viewports)
                    {
                        if (v != si.SelectedViewportInfo && v.Name == newName)
                        {
                            PropertyCellChangeDenying_ViewportName = si.SelectedViewportInfo.Name;
                            return;
                        }
                    }
                }
            }
        }
        
        private void grdProperties_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            var si = ProjectDoc.Instance.SelectedSceneInfo;
            if (si != null && grdProperties.SelectedObject == si.SelectedViewportInfo)
            {
                if (e.Row.Properties.Caption == "名称" && !String.IsNullOrEmpty(PropertyCellChangeDenying_ViewportName))
                {
                    MessageBox.Show("已存在同名窗口，请重新命名。");
                    si.SelectedViewportInfo.Name = PropertyCellChangeDenying_ViewportName;
                    PropertyCellChangeDenying_ViewportName = null;
                    grdProperties.Refresh();
                    return;
                }
            }
            OperationHistory.Instance.IsDirty = true;
        }

        private void icShowProjectBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowProjectBar.Checked)
                dcpProject.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpProject.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowPropertyBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowPropertyBar.Checked)
                dcpProperties.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpProperties.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowAnimationBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowAnimationBar.Checked)
                dcpAnimation.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpAnimation.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
        }

        private void icShowResourceBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (icShowResourceBar.Checked)
                dcpResources.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            else
                dcpResources.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
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

        private void trvProject_CalcNodeDragImageIndex(object sender, DevExpress.XtraTreeList.CalcNodeDragImageIndexEventArgs e)
        {
            var tl = sender as DevExpress.XtraTreeList.TreeList;
            if (GetProjectTreeDragDropEffect(tl, tl.FocusedNode) == DragDropEffects.None)
                e.ImageIndex = -1;  // no icon
            else
                e.ImageIndex = 1;  // the reorder icon (a curved arrow)
        }

        private DragDropEffects GetProjectTreeDragDropEffect(DevExpress.XtraTreeList.TreeList tl, TreeListNode dragNode)
        {
            Point p = tl.PointToClient(MousePosition);
            var targetNode = tl.CalcHitInfo(p).Node;

            if (dragNode == null || targetNode == null || dragNode == targetNode)
                return DragDropEffects.None;
            if (dragNode.Tag == null || targetNode.Tag == null)
                return DragDropEffects.None;
            var t1 = dragNode.Tag.GetType();
            var t2 = targetNode.Tag.GetType();
            if (t1 == typeof(ViewportInfo) || t1.IsSubclassOf(typeof(ViewportInfo)))
            {
                if (t2 != t1)
                    return DragDropEffects.None;
                if (dragNode.ParentNode != targetNode.ParentNode)
                    return DragDropEffects.None;
            }
            else if (t1.IsSubclassOf(typeof(ElementInfo)))
            {
                if (!t2.IsSubclassOf(typeof(ElementInfo)) && !(t2 == typeof(ViewportInfo) || t2.IsSubclassOf(typeof(ViewportInfo))))
                    return DragDropEffects.None;
            }
            return DragDropEffects.Move;
        }

        private void trvProject_DragOver(object sender, DragEventArgs e)
        {
            TreeListNode dragNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            e.Effect = GetProjectTreeDragDropEffect(sender as DevExpress.XtraTreeList.TreeList, dragNode);
        }

        private void trvProject_DragDrop(object sender, DragEventArgs e)
        {
            var tl = sender as DevExpress.XtraTreeList.TreeList;
            Point p = tl.PointToClient(new Point(e.X, e.Y));

            var dragNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            var targetNode = tl.CalcHitInfo(p).Node;
            var t1 = dragNode.Tag.GetType();
            var t2 = targetNode.Tag.GetType();
            if (t1 == typeof(ViewportInfo) || t1.IsSubclassOf(typeof(ViewportInfo)))
            {
                var v1 = dragNode.Tag as ViewportInfo;
                var v2 = targetNode.Tag as ViewportInfo;
                ProjectDoc.Instance.SelectedSceneInfo.SwapViewport(v1.ScreenIndex, v2.ScreenIndex);
                m_EditViewOperater.RefreshProjectTree();
            }
            else if (t1.IsSubclassOf(typeof(ElementInfo)))
            {
                var e1 = dragNode.Tag as ElementInfo;
                var t2p = targetNode.ParentNode.Tag.GetType();
                if (t2p == typeof(ViewportInfo) || t2p.IsSubclassOf(typeof(ViewportInfo)))
                {
                    var v1 = dragNode.ParentNode.Tag as ViewportInfo;
                    var v2 = targetNode.ParentNode.Tag as ViewportInfo;
                    var e2 = targetNode.Tag as ElementInfo;
                    if (v1 != v2)
                        OperationHistory.Instance.CommitOperation(new Operation_Element_ChangeViewport(e1, v2));
                    else
                        OperationHistory.Instance.CommitOperation(new Operation_Element_Drift(e1, e2.DepthLevel - e1.DepthLevel));
                    m_EditViewOperater.RefreshProjectTree();
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void icCombineElement_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnChkBtnCombineElement_ItemClicked();
        }

        private void icCombineElement_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            m_EditViewOperater.OnChkBtnCombineElement_CheckChanged();
            IsElementNodeSelected = true;
        }

        private void iBreakElementGroup_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ei = ProjectDoc.Instance.SelectedElementInfo;
            if (ei != null && ei.ParentElementGroup != null)
            {
                ei.ParentElementGroup.Dismiss();
                IsElementNodeSelected = true;
            }
        }

        private void iSeperateElementFromGroup_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ei = ProjectDoc.Instance.SelectedElementInfo;
            if (ei != null && ei.ParentElementGroup != null)
            {
                ei.ParentElementGroup.RemoveElement(ei);
                IsElementNodeSelected = true;
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

        public void Update(float elapsedTime)
        {
            string msg = AppLogger.Pick();
            if (!String.IsNullOrEmpty(msg))
                txbLogs.AppendText(msg + "\r\n");
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
