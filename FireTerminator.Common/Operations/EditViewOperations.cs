using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.RenderResources;
using System.Windows.Forms;
using FireTerminator.Common.Elements;
using System.Drawing;

namespace FireTerminator.Common.Operations
{
    public class EditViewOperater
    {
        public EditViewOperater(Form form, Panel view, Panel preview)
        {
            ParentForm = form;
            PanelView = view;
            PanelPreview = preview;
            ProjectDoc.Instance.AppendedSceneInfo += new ProjectDoc.Delegate_SceneInfo(ProjectDoc_CreatedSceneInfo);
            ProjectDoc.Instance.RemovedSceneInfo += new ProjectDoc.Delegate_SceneInfo(ProjectDoc_RemovedSceneInfo);
        }

        public Form ParentForm { get; private set; }
        public Panel PanelView { get; private set; }
        public Panel PanelPreview { get; private set; }
        public DevExpress.XtraTreeList.TreeList ResourceTree { get; set; }
        public DevExpress.XtraTreeList.TreeList ProjectTree { get; set; }
        public PropertyGrid PropertyBar { get; set; }
        public FireTerminator.Common.UI.TransitionController TransCtrl { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_MoveElement { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_ScaleElement { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_RotateElement { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_DriveElement { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_AnimElement { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_CombineElement { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_ToolElement_Mask { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_ToolElement_Waterbag { get; set; }
        public DevExpress.XtraBars.BarCheckItem ChkBtn_ToolElement_TipText { get; set; }
        public DevExpress.XtraBars.BarButtonItem Btn_CopyElement { get; set; }
        public DevExpress.XtraBars.BarButtonItem Btn_CutElement { get; set; }
        public DevExpress.XtraBars.BarButtonItem Btn_PasteElement { get; set; }
        public ToolTip Cursor_ToolTip { get; set; }
        public string CustomAppTitleTailText { get; set; }

        public List<string> ForbiddingResourcePathNames = new List<string>();

        public Dictionary<ResourceKind, int[]> IconImageIndices = new Dictionary<ResourceKind, int[]>();
        public int[] ExtraIconImageIndices = new int[] { 0, 0, 0, 0 };// folder; task; scene; viewport
        public ElementInfo CopiedElement = null;
        public ElementInfo CuttedElement = null;
        public ViewportInfo CopiedViewport = null;
        public ViewportInfo CuttedViewport = null;
        public Type SelectedElementToolType = null;
        private DevExpress.XtraBars.BarCheckItem CheckingToolElementButton = null;
        private DragingInfomation DragingInfo = new DragingInfomation();
        private ElementGroup OperatingElementGroup = null;

        public ResourceInfo FocusedResourceInfo
        {
            get;
            private set;
        }
        public class DraggingResourceInfo
        {
            public DraggingResourceInfo(ResourceInfo info)
            {
                Info = info;
            }
            public ResourceInfo Info
            {
                get;
                private set;
            }
            public bool IsAudio
            {
                get { return Info != null && Info is ResourceInfo_Audio; }
            }
        }
        public class DragingInfomation
        {
            public System.Drawing.Point? ElmDragPosBegin = null;
            public System.Drawing.Point? ElmDragOffset = null;
            public System.Drawing.Point? BackViewDragOffset = null;
            public System.Drawing.SizeF? PosViewRateBegin = null;
            public System.Drawing.Point? CursorPosBegin = null;
            public System.Drawing.PointF? BackImageBeginOffset = null;
            public float? ViewScaleRateBegin = null;
            public float ElmDragAngleBegin = 0;
            public System.Drawing.SizeF? ElmDragSizeBegin = null;
            public System.Drawing.SizeF? ElmDragBlendSizeBegin = null;
        }

        public ElementInfo GetElementOnCursor(Point pt)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null)
            {
                pt = PanelView.PointToClient(pt);
                pt = vi.PointToClient(false, pt);
                BodyOperationPart bop = BodyOperationPart.Body;
                return vi.GetElementInfo(pt, ref bop);
            }
            return null;
        }

        private void ProjectDoc_CreatedSceneInfo(SceneInfo si)
        {
            BindDelegatesToSceneInfo(si, true);
        }

        private void ProjectDoc_RemovedSceneInfo(SceneInfo si)
        {
            BindDelegatesToSceneInfo(si, false);
        }

        public void OnResourceTree_MouseMove(MouseEventArgs e)
        {
            if (FocusedResourceInfo == null || e.Button != MouseButtons.Left)
                return;
            ResourceTree.DoDragDrop(new DraggingResourceInfo(FocusedResourceInfo), DragDropEffects.Copy);
        }
        public void OnResourceTree_MouseDoubleClick()
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null)
            {
                var res = ResourceTree.FocusedNode.Tag as ResourceInfo;
                if (res != null)
                {
                    if (res is ResourceInfo_Audio && ProjectDoc.Instance.SelectedElementInfo != null)
                    {
                        var resSound = res as ResourceInfo_Audio;
                        ProjectDoc.Instance.SelectedElementInfo.SoundFile = resSound.SubPathFileName;
                    }
                    else
                    {
                        CreateNewElement(res, vi, res.ImageSize.Width / 2, res.ImageSize.Height / 2);
                    }
                }
            }
        }
        public void OnResourceTree_FocusedNodeChanged(DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            FocusedResourceInfo = null;
            if (e.Node != null)
            {
                if (e.Node.Tag is string)
                    PropertyBar.SelectedObject = null;
                else
                {
                    FocusedResourceInfo = e.Node.Tag as ResourceInfo;
                    PropertyBar.SelectedObject = FocusedResourceInfo;
                }
            }
        }
        public void OnResourceTree_GiveFeedback(GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
        }
        public void OnPreviewPanel_GiveFeedback(GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
        }
        public void OnPreviewPanel_MouseMove(MouseEventArgs e)
        {
            if (FocusedResourceInfo == null || e.Button != MouseButtons.Left)
                return;
            PanelPreview.DoDragDrop(new DraggingResourceInfo(FocusedResourceInfo), DragDropEffects.Copy);
        }
        //static int debugcount = 0;
        private bool bInner_trvProject_FocusedNodeChanged = false;
        public void OnProjectTree_FocusedNodeChanged(DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e, out bool? selTask, out bool? selScene, out bool? selViewport, out bool? selElement)
        {
            selTask = null;
            selScene = null;
            selViewport = null;
            selElement = null;

            if (e.Node != null && e.Node.TreeList.IsLoading)
                return;
            //System.Diagnostics.Debug.WriteLine(String.Format("[{0}]OnProjectTree_FocusedNodeChanged", ++debugcount));

            if (e.Node == null || bInner_trvProject_FocusedNodeChanged)
                return;
            bInner_trvProject_FocusedNodeChanged = true;
            var node = e.Node;
            TaskInfo ti = null;
            SceneInfo si = null;
            ViewportInfo vi = null;
            SceneInfo presi = ProjectDoc.Instance.SelectedSceneInfo;

            ElementInfo ei = node.Tag as ElementInfo;
            if (ei != null)
            {
                vi = ei.ParentViewport;
                si = vi.ParentSceneInfo;
                ti = si.ParentTaskInfo;
                PropertyBar.SelectedObject = ei;
                ProjectDoc.Instance.SelectedElementInfo = ei;
                if (TransCtrl != null)
                    TransCtrl.ParentElement = ei;
                selElement = true;
            }
            else
            {
                if (TransCtrl != null)
                    TransCtrl.ParentElement = null;
                selElement = false;
                if (CurEditingAnimModeViewport != null)
                {
                    CurEditingAnimModeViewport.IsAnimEditingMode = false;
                    if (TransCtrl != null)
                        TransCtrl.ParentElement = null;
                }
                vi = node.Tag as ViewportInfo;
                if (vi != null)
                {
                    si = vi.ParentSceneInfo;
                    ti = si.ParentTaskInfo;
                    PropertyBar.SelectedObject = vi;
                    ProjectDoc.Instance.SelectedViewportInfo = vi;
                    ProjectDoc.Instance.SelectedElementInfo = null;
                    selViewport = true;
                }
                else
                {
                    vi = ProjectDoc.Instance.SelectedViewportInfo;
                    si = e.Node.Tag as SceneInfo;
                    if (si != null)
                    {
                        ti = si.ParentTaskInfo;
                        PropertyBar.SelectedObject = si;
                        ProjectDoc.Instance.SelectedSceneInfo = si;
                        selScene = true;
                    }
                    else
                    {
                        si = ProjectDoc.Instance.SelectedSceneInfo;
                        ti = e.Node.Tag as TaskInfo;
                        if (ti == null)
                            ti = ProjectDoc.Instance.SelectedTaskInfo;
                        else
                            ProjectDoc.Instance.SelectedTaskInfo = ti;
                        PropertyBar.SelectedObject = ti;
                        selTask = true;
                    }
                }
            }
            bInner_trvProject_FocusedNodeChanged = false;
        }

        private void BindDelegatesToSceneInfo(SceneInfo si, bool bind)
        {
            if (si == null)
                return;
            if (bind)
            {
                BindDelegatesToSceneInfo(si, false);
                si.ViewportElementAdded += new SceneInfo.Delegate_OnViewportElementChanged(CurFocusSceneInfo_ViewportElementChanged);
                si.ViewportElementRemoved += new SceneInfo.Delegate_OnViewportElementChanged(CurFocusSceneInfo_ViewportElementChanged);
                si.ViewportAnimEditChanged += new SceneInfo.Delegate_OnViewportChanged(CurFocusSceneInfo_ViewportAnimEditChanged);
                si.ViewportElementInnerEditModeChanged += new SceneInfo.Delegate_OnViewportElementChanged(CurFocusSceneInfo_ViewportElementInnerEditModeChanged);
                si.ViewportElementTransitionChanged += new SceneInfo.Delegate_OnViewportElementTransitionChanged(CurFocusSceneInfo_ViewportElementTransitionChanged);
            }
            else
            {
                si.ViewportElementAdded -= new SceneInfo.Delegate_OnViewportElementChanged(CurFocusSceneInfo_ViewportElementChanged);
                si.ViewportElementRemoved -= new SceneInfo.Delegate_OnViewportElementChanged(CurFocusSceneInfo_ViewportElementChanged);
                si.ViewportAnimEditChanged -= new SceneInfo.Delegate_OnViewportChanged(CurFocusSceneInfo_ViewportAnimEditChanged);
                si.ViewportElementInnerEditModeChanged -= new SceneInfo.Delegate_OnViewportElementChanged(CurFocusSceneInfo_ViewportElementInnerEditModeChanged);
                si.ViewportElementTransitionChanged -= new SceneInfo.Delegate_OnViewportElementTransitionChanged(CurFocusSceneInfo_ViewportElementTransitionChanged);
                if (CopiedViewport != null && CopiedViewport.ParentSceneInfo == si)
                    CopiedViewport = null;
                if (CuttedViewport != null && CuttedViewport.ParentSceneInfo == si)
                    CuttedViewport = null;
            }
        }

        protected void CurFocusSceneInfo_ViewportElementInnerEditModeChanged(ElementInfo elm)
        {
            ViewportInfo vi = elm.ParentViewport;
            vi.IsAnimEditingMode = false;
            if (!elm.IsInnerEditingMode)
            {
                if (elm is ElementInfo_Mask)
                {
                    OperationHistory.Instance.PushOperation(new Operation_Element_ChangeMaskInfo(elm as ElementInfo_Mask));
                    OperationHistory.Instance.CommitOperation();
                }
                else if (elm is ElementInfo_Waterbag)
                {
                    OperationHistory.Instance.PushOperation(new Operation_Element_ChangeWaterbagInfo(elm as ElementInfo_Waterbag));
                    OperationHistory.Instance.CommitOperation();
                }
                ProjectTree.Appearance.FocusedCell.BackColor = Color.Gainsboro;
                ProjectTree.Appearance.FocusedCell.BackColor2 = Color.White;
            }
            else
            {
                if (elm is ElementInfo_Mask)
                    OperationHistory.Instance.PushOperation(new Operation_Element_ChangeMaskInfo(elm as ElementInfo_Mask));
                else if (elm is ElementInfo_Waterbag)
                    OperationHistory.Instance.PushOperation(new Operation_Element_ChangeWaterbagInfo(elm as ElementInfo_Waterbag));
                ProjectTree.Appearance.FocusedCell.BackColor = Color.White;
                ProjectTree.Appearance.FocusedCell.BackColor2 = Color.Blue;
            }
        }

        protected virtual void CurFocusSceneInfo_ViewportAnimEditChanged(ViewportInfo vi)
        {
            if (TransCtrl == null)
                return;
            bool editMode = vi.IsAnimEditingMode;
            if (editMode)
            {
                TransCtrl.ParentElement = vi.SelectedElementInfo;
                TransCtrl.Enabled = true;
                vi.CurTimeTick = TransCtrl.Drawer.TimeRuler.CurTime;
            }
            else if (TransCtrl.ParentElement != null && TransCtrl.ParentElement.ParentViewport == vi)
            {
                TransCtrl.ParentElement = null;
            }
            if (TransCtrl.ParentElement == null)
            {
                ProjectTree.Appearance.FocusedCell.BackColor = Color.Gainsboro;
                ProjectTree.Appearance.FocusedCell.BackColor2 = Color.White;
                ChkBtn_AnimElement.Checked = false;
            }
            else
            {
                ProjectTree.Appearance.FocusedCell.BackColor = Color.White;
                ProjectTree.Appearance.FocusedCell.BackColor2 = Color.Red;
                ChkBtn_AnimElement.Checked = true;
            }
        }

        protected virtual void CurFocusSceneInfo_ViewportElementChanged(ElementInfo elm)
        {
            try
            {
                ViewportInfo vi = elm.ParentViewport;
                bool deleted = !vi.Elements.Contains(elm);
                if (deleted)
                {
                    if (CopiedElement == elm)
                        CopiedElement = null;
                    if (CuttedElement == elm)
                        CuttedElement = null;
                }
                if ((!deleted && !elm.CanSelect) || vi.TreeNode == null)
                    return;
                //if (OperationHistory.Instance.IsInTransaction)
                //    return;
                ((System.ComponentModel.ISupportInitialize)(vi.TreeNode.TreeList)).BeginInit();
                var vnode = vi.TreeNode;
                DevExpress.XtraTreeList.Nodes.TreeListNode selNode = null;
                if (deleted)
                    vnode.Nodes.Remove(elm.TreeNode);
                else
                {
                    int index = vi.Elements.IndexOf(elm);
                    while (vnode.Nodes.Count > index)
                        vnode.Nodes.RemoveAt(index);
                    for (int i = index; i < vi.Elements.Count; ++i)
                    {
                        var e = vi.Elements[i];
                        var enode = vnode.Nodes.Add(new object[] { e });
                        enode.Tag = e;
                        enode.ImageIndex = enode.SelectImageIndex = IconImageIndices[e.Resource.Kind][1];
                        if (e == elm)
                            selNode = enode;
                        e.TreeNode = enode;
                    }
                }
                ((System.ComponentModel.ISupportInitialize)(vi.TreeNode.TreeList)).EndInit();
                vnode.Selected = true;
                vnode.Expanded = true;
                if (!deleted)
                {
                    ProjectDoc.Instance.SelectedElementInfo = elm;
                    elm.TreeNode.Selected = true;
                }
            }
            catch{}
        }

        protected void CurFocusSceneInfo_ViewportElementTransitionChanged(ElementInfo elm, TransitionKind kind)
        {
            if (elm == TransCtrl.Drawer.BindedElement)
                TransCtrl.Drawer.RefreshTransitionLine(kind);
        }

        public void OnPanelView_DragEnter(DragEventArgs e)
        {
            var dri = e.Data.GetData(typeof(DraggingResourceInfo)) as DraggingResourceInfo;
            if (dri == null)
                ParentForm.Cursor = Cursors.No;
            else
            {
                e.Effect = DragDropEffects.Copy;
                ParentForm.Cursor = new Cursor(Properties.Resources.ArrowPlus.Handle);
            }
        }
        public void OnPanelView_DragDrop(DragEventArgs e)
        {
            var dri = e.Data.GetData(typeof(DraggingResourceInfo)) as DraggingResourceInfo;
            var res = ResourceTree.FocusedNode.Tag as ResourceInfo;
            if (dri != null && res != null)
            {
                var elm = GetElementOnCursor(new Point(e.X, e.Y));
                if (dri.IsAudio && elm != null)
                {
                    ResourceInfo_Audio resSound = dri.Info as ResourceInfo_Audio;
                    elm.SoundFile = resSound.SubPathFileName;
                }
                else
                {
                    var pt = PanelView.PointToClient(Cursor.Position);
                    var vi = ProjectDoc.Instance.SelectedSceneInfo.GetViewportInfoByPosition(pt);
                    if (vi != null)
                    {
                        pt.X -= vi.ViewportPtr.X;
                        pt.Y -= vi.ViewportPtr.Y;
                        CreateNewElement(res, vi, pt.X, pt.Y);
                    }
                }
            }
            ParentForm.Cursor = Cursors.Default;
        }
        public void OnPanelView_DragLeave()
        {
            ParentForm.Cursor = Cursors.Default;
        }
        private ElementInfo CreateToolElement(Type type, ViewportInfo vi, float x, float y)
        {
            var ei = ElementInfo.CreateElement(type, vi, new PointF(x, y));
            if (ei != null)
            {
                OperationHistory.Instance.CommitOperation(new Operation_Element_Create(ei, vi));
                ProjectDoc.Instance.SelectedElementInfo = ei;
            }
            return ei;
        }
        private ElementInfo CreateNewElement(ResourceInfo res, ViewportInfo vi, float x, float y)
        {
            bool bDeleteBackImage = false;
            if (res is ResourceInfo_BackgroundImage && vi.BackImage != null)
            {
                if (vi.BackImage.Resource == res)
                    return vi.BackImage;
                bDeleteBackImage = true;
            }
            var ei = ElementInfo.CreateElement(res, vi, new PointF(x, y));
            if (ei != null)
            {
                if (bDeleteBackImage)
                {
                    var opt = new Operation_Element_Delete(vi.BackImage);
                    opt.LinkNext = true;
                    OperationHistory.Instance.CommitOperation(opt);
                }
                OperationHistory.Instance.CommitOperation(new Operation_Element_Create(ei, vi));
                ProjectDoc.Instance.SelectedElementInfo = ei;
            }
            return ei;
        }

        private BodyOperation CurBodyOperation = BodyOperation.None;
        private BodyOperationPart CurBodyOperationPart = BodyOperationPart.Nothing;
        private ViewportInfo CurEditingAnimModeViewport = null;

        public void OnPanelView_MouseDown(MouseEventArgs e)
        {
            PanelView.Focus();
            if (m_LastMouseFocusElementForTooltip != null)
            {
                Cursor_ToolTip.Hide(PanelView);
                m_LastMouseFocusElementForTooltip = null;
            }
            var pt = PanelView.PointToClient(Cursor.Position);
            if (ProjectDoc.Instance.SelectedSceneInfo == null)
                return;
            var curElm = ProjectDoc.Instance.SelectedElementInfo;
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (CurBodyOperationPart == BodyOperationPart.Nothing)
                vi = ProjectDoc.Instance.SelectedSceneInfo.GetViewportInfoByPosition(pt);
            if (vi != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (ChkBtn_CombineElement != null && ChkBtn_CombineElement.Checked)
                    {
                        if (CurMouseFocusElement != null && OperatingElementGroup != null)
                            OperatingElementGroup.AddElement(CurMouseFocusElement);
                    }
                    else
                    {
                        DragingInfo.BackViewDragOffset = null;
                        pt = vi.PointToClient(false, pt);
                        CurBodyOperation = BodyOperation.None;
                        BodyOperationPart part = BodyOperationPart.Nothing;
                        ElementInfo ei = null;
                        if (curElm != null && curElm.IsInnerEditingMode)
                            ei = curElm;
                        else if (SelectedElementToolType != null)
                        {
                            ei = CreateToolElement(SelectedElementToolType, vi, pt.X, pt.Y);
                            SelectedElementToolType = null;
                            if (CheckingToolElementButton != null)
                            {
                                CheckingToolElementButton.Checked = false;
                                CheckingToolElementButton = null;
                            }
                        }
                        else if (curElm != null && curElm.ParentViewport == vi)
                        {
                            if (ChkBtn_DriveElement != null && ChkBtn_DriveElement.Checked)
                            {
                                ei = curElm;
                                curElm.TargetLocation = pt;
                                OperationHistory.Instance.PushOperation(new Operation_Element_ChangeLocation(ei, pt, true));
                            }
                            else
                            {
                                part = curElm.GetPointBodyOprPart(pt);
                                if (part > BodyOperationPart.Body && part < BodyOperationPart.AddJoint)
                                    ei = curElm;
                            }
                        }
                        if (ei == null)
                            ei = CurMouseFocusElement;
                        if (ei == null)
                        {
                            DragingInfo.ElmDragPosBegin = null;
                            DragingInfo.ElmDragOffset = null;
                            if (vi.TreeNode != null)
                                vi.TreeNode.Selected = true;
                        }
                        else
                        {
                            if (CurBodyOperationPart > BodyOperationPart.Body)
                            {
                                CurBodyOperation = BodyOperation.Scale;
                                if (ei is ElementInfo_Waterbag && ei.IsInnerEditingMode)
                                {
                                    var bag = ei as ElementInfo_Waterbag;
                                    bag.SetPosition(CurBodyOperationPart, pt);
                                }
                            }
                            else if (CurBodyOperationPart == BodyOperationPart.Body)
                            {
                                if (ChkBtn_MoveElement.Checked)
                                    CurBodyOperation = BodyOperation.Move;
                                else if (ChkBtn_RotateElement.Checked)
                                    CurBodyOperation = BodyOperation.Rotate;
                            }
                            DragingInfo.ElmDragPosBegin = pt;
                            DragingInfo.ElmDragOffset = ei.GetViewportScreenLocationOffset(pt, false);
                            DragingInfo.ElmDragSizeBegin = ei.Size;
                            DragingInfo.ElmDragBlendSizeBegin = ei.BlendedSize;
                            DragingInfo.ElmDragAngleBegin = ei.RotateAngle;
                            if (vi.TreeNode != null)
                                ei.TreeNode.Selected = true;
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (ChkBtn_CombineElement != null && ChkBtn_CombineElement.Checked)
                        ChkBtn_CombineElement.Checked = false;
                    else if (vi.BackImage == null)
                        CancelElementSelectionAndAnimEdition();
                    else
                    {
                        DragingInfo.ElmDragOffset = null;
                        DragingInfo.BackViewDragOffset = vi.PointToClient(false, pt);
                        DragingInfo.BackImageBeginOffset = null;// vi.BackImageViewOffset;
                        CurBodyOperation = BodyOperation.PanView;
                        Cursor.Current = new Cursor(Properties.Resources.Pan.Handle);
                    }
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    if (!ProjectDoc.Instance.Option.UseSafeViewportSwitching && vi.TreeNode != null)
                        vi.TreeNode.Selected = true;

                    if (vi.BackImage != null)
                    {
                        DragingInfo.ElmDragOffset = null;
                        DragingInfo.BackViewDragOffset = vi.PointToClient(false, pt);
                        CurBodyOperation = BodyOperation.ZoomView;
                        //Cursor.Current = Cursors.NoMoveVert;

                        DragingInfo.CursorPosBegin = Cursor.Position;
                        float rw = DragingInfo.BackViewDragOffset.Value.X / (float)vi.ViewportSize.Width;
                        float rh = DragingInfo.BackViewDragOffset.Value.Y / (float)vi.ViewportSize.Height;
                        DragingInfo.PosViewRateBegin = new System.Drawing.SizeF(rw, rh);
                        DragingInfo.ViewScaleRateBegin = vi.ViewScale;
                        Cursor.Hide();
                    }
                }
            }
        }

        private ElementInfo CurMouseFocusElement = null;
        private ElementInfo m_LastMouseFocusElementForTooltip = null;
        private Point m_LastPanelViewMouseLocation = new Point(0, 0);
        public void OnPanelView_MouseMove(MouseEventArgs e)
        {
            bool bMouseMoved = (m_LastPanelViewMouseLocation != e.Location);
            bool bMouseMoreMoved = (Math.Abs(m_LastPanelViewMouseLocation.X - e.X) > 1 || Math.Abs(m_LastPanelViewMouseLocation.Y - e.Y) > 1);
            m_LastPanelViewMouseLocation = e.Location;
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            // 取得当前视口的焦点元素
            if (vi != null && e.Button == MouseButtons.None)
            {
                var pt = vi.PointToClient(false, e.Location);
                CurBodyOperationPart = BodyOperationPart.Nothing;
                CurMouseFocusElement = null;
                var curElm = ProjectDoc.Instance.SelectedElementInfo;
                if (curElm != null && curElm.ParentViewport == vi)
                {
                    CurBodyOperationPart = curElm.GetPointBodyOprPart(pt);
                    if (CurBodyOperationPart > BodyOperationPart.Body && CurBodyOperationPart < BodyOperationPart.AddJoint)
                        CurMouseFocusElement = curElm;
                }
                if (CurMouseFocusElement == null)
                    CurMouseFocusElement = vi.GetElementInfo(pt, ref CurBodyOperationPart);
                if (CurMouseFocusElement == null)
                {
                    Cursor.Current = Cursors.Default;
                    if (Cursor_ToolTip != null && m_LastMouseFocusElementForTooltip != null)
                    {
                        Cursor_ToolTip.Hide(PanelView);
                        m_LastMouseFocusElementForTooltip = null;
                    }
                }
                else if (Cursor_ToolTip != null && m_LastMouseFocusElementForTooltip != CurMouseFocusElement)
                {
                    string text = CurMouseFocusElement.Name;
                    if (!String.IsNullOrEmpty(CurMouseFocusElement.Caption))
                        text += String.Format(" ({0})", CurMouseFocusElement.Caption);
                    Cursor_ToolTip.Show(text, PanelView, e.Location.X + 8, e.Location.Y + 8);
                    m_LastMouseFocusElementForTooltip = CurMouseFocusElement;
                }
            }
            // 鼠标按键操作 - 平移背景
            if (CurBodyOperation == BodyOperation.PanView)
            {
                if (DragingInfo.BackViewDragOffset == null || (e.Button & MouseButtons.Right) != MouseButtons.Right)
                    CurBodyOperation = BodyOperation.None;
                else if (bMouseMoved)
                {
                    if (DragingInfo.BackImageBeginOffset == null)
                        DragingInfo.BackImageBeginOffset = vi.BackImageViewOffset;
                    Cursor.Current = new Cursor(Properties.Resources.Pan.Handle);
                    var offset = vi.PointToClient(false, e.Location);
                    offset.X = offset.X - DragingInfo.BackViewDragOffset.Value.X + (int)DragingInfo.BackImageBeginOffset.Value.X;
                    offset.Y = offset.Y - DragingInfo.BackViewDragOffset.Value.Y + (int)DragingInfo.BackImageBeginOffset.Value.Y;
                    vi.BackImageViewOffset = offset;
                    OperationHistory.Instance.IsDirty = true;
                }
            }
            // 鼠标按键操作 - 局部缩放背景
            else if (CurBodyOperation == BodyOperation.ZoomView)
            {
                if (DragingInfo.BackViewDragOffset == null || (e.Button & MouseButtons.Middle) != MouseButtons.Middle)
                    CurBodyOperation = BodyOperation.None;
                else
                {
                    //Cursor.Current = Cursors.NoMoveVert;
                    var offset = vi.PointToClient(false, e.Location);
                    offset.Y -= DragingInfo.BackViewDragOffset.Value.Y;
                    float scale = DragingInfo.ViewScaleRateBegin.Value - offset.Y / 100.0F;
                    if (scale < 1)
                        scale = 1;
                    vi.BackImage.Scale(DragingInfo.PosViewRateBegin.Value.Width, DragingInfo.PosViewRateBegin.Value.Height, scale);
                    OperationHistory.Instance.IsDirty = true;
                }
            }

            if (CurBodyOperationPart != BodyOperationPart.Nothing && ProjectDoc.Instance.SelectedElementInfo == null)
                CurBodyOperationPart = BodyOperationPart.Nothing;
            // 确定鼠标指针 - 当焦点元素落在热点区域 - 元素内容时
            if (CurBodyOperationPart == BodyOperationPart.Body)
            {
                if (ChkBtn_CombineElement != null && ChkBtn_CombineElement.Checked)
                    Cursor.Current = Cursors.PanNorth;
                else if (ChkBtn_MoveElement.Checked)
                    Cursor.Current = Cursors.SizeAll;
                else if (ChkBtn_RotateElement.Checked)
                    Cursor.Current = new Cursor(Properties.Resources.Rotate.Handle);
                else
                    Cursor.Current = Cursors.Default;
            }
            // 确定鼠标指针 - 当焦点元素落在热点区域 - 元素边角时
            else if (CurBodyOperationPart > BodyOperationPart.Body)
            {
                if (ChkBtn_CombineElement != null && ChkBtn_CombineElement.Checked)
                    Cursor.Current = Cursors.PanNorth;
                else if (ChkBtn_ScaleElement.Checked)
                {
                    switch (CurBodyOperationPart)
                    {
                        case BodyOperationPart.BorderL:
                        case BodyOperationPart.BorderR:
                            Cursor.Current = Cursors.SizeWE; break;
                        case BodyOperationPart.BorderU:
                        case BodyOperationPart.BorderD:
                            Cursor.Current = Cursors.SizeNS; break;
                        case BodyOperationPart.CornerLU:
                        case BodyOperationPart.CornerRD:
                            Cursor.Current = Cursors.SizeNWSE; break;
                        case BodyOperationPart.CornerRU:
                        case BodyOperationPart.CornerLD:
                            Cursor.Current = Cursors.SizeNESW; break;
                        case BodyOperationPart.AddJoint:
                            Cursor.Current = new Cursor(Properties.Resources.ArrowPlus.Handle); break;
                        case BodyOperationPart.DelJoint:
                            Cursor.Current = new Cursor(Properties.Resources.ArrowMinus.Handle); break;
                    }
                }
            }
            if (CurBodyOperation == BodyOperation.None && CurBodyOperationPart == BodyOperationPart.Nothing)
                Cursor.Current = Cursors.Default;
            else if (vi != null && DragingInfo.ElmDragOffset != null && e.Button == MouseButtons.Left && ProjectDoc.Instance.IsElementModifyEnabled && bMouseMoved)
            {
                var pt = vi.PointToClient(false, e.Location);
                var ei = ProjectDoc.Instance.SelectedElementInfo;
                if (ei is ElementInfo_Mask && CurBodyOperationPart != BodyOperationPart.Body)
                {
                    var mask = ei as ElementInfo_Mask;
                    switch (CurBodyOperationPart)
                    {
                        case BodyOperationPart.CornerLU:
                        case BodyOperationPart.CornerRU:
                        case BodyOperationPart.CornerRD:
                        case BodyOperationPart.CornerLD:
                            mask.SetPosition(CurBodyOperationPart, pt);
                            break;
                    }
                }
                else if (ei is ElementInfo_Waterbag && ei.IsInnerEditingMode)
                {
                    var bag = ei as ElementInfo_Waterbag;
                    if (CurBodyOperationPart == BodyOperationPart.Body)
                    {
                        var offset = new System.Drawing.Point(pt.X - DragingInfo.ElmDragPosBegin.Value.X, pt.Y - DragingInfo.ElmDragPosBegin.Value.Y);
                        bag.SetPosition(CurBodyOperationPart, offset);
                        DragingInfo.ElmDragPosBegin = pt;
                    }
                }
                else if (CurBodyOperationPart == BodyOperationPart.Body)
                {
                    if (ei != null)
                    {
                        // 移动元素
                        if (ChkBtn_MoveElement.Checked)
                        {
                            if (vi.IsAnimEditingMode)
                            {
                                var offset = new System.Drawing.Point(pt.X - DragingInfo.ElmDragOffset.Value.X, pt.Y - DragingInfo.ElmDragOffset.Value.Y);
                                if (!offset.IsEmpty)
                                    OperationHistory.Instance.PushOperation(new Operation_Element_ChangeLocation_Trans(ei, offset));
                            }
                            else
                            {
                                var offset = new System.Drawing.Point(pt.X - DragingInfo.ElmDragOffset.Value.X, pt.Y - DragingInfo.ElmDragOffset.Value.Y);
                                if (!offset.IsEmpty)
                                    OperationHistory.Instance.PushOperation(new Operation_Element_ChangeLocation(ei, offset));
                            }
                        }
                        // 旋转元素
                        else if (ChkBtn_RotateElement.Checked)
                        {
                            int offsetY = (int)(DragingInfo.ElmDragAngleBegin + (pt.Y - DragingInfo.ElmDragPosBegin.Value.Y));
                            if (offsetY != 0)
                            {
                                if (vi.IsAnimEditingMode)
                                    OperationHistory.Instance.PushOperation(new Operation_Element_ChangeRotAngle_Trans(ei, ei.RotateAngle, offsetY));
                                else
                                    OperationHistory.Instance.PushOperation(new Operation_Element_ChangeProperty(ei, "RotateAngle", ei.RotateAngle, offsetY));
                            }
                        }
                    }
                }
                else if (ei != null)
                {
                    // 缩放元素
                    bool bIsShiftDown = Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift)
                        || Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);

                    float curSizeProportion = DragingInfo.ElmDragBlendSizeBegin.Value.Width / (float)DragingInfo.ElmDragBlendSizeBegin.Value.Height;
                    var offset = new System.Drawing.Point(pt.X - DragingInfo.ElmDragPosBegin.Value.X, pt.Y - DragingInfo.ElmDragPosBegin.Value.Y);
                    var sizeBegin = DragingInfo.ElmDragSizeBegin.Value;
                    var sizeImage = ei.Resource.ImageSize;
                    if (ei.ScaleProportionOnBackImage != 0 && vi.BackImage != null)
                    {
                        float rate = ei.ScaleProportionOnBackImage * vi.BackImage.Size.Width / ei.Resource.ImageSize.Width;
                        sizeImage.Width *= rate;
                        sizeImage.Height *= rate;
                    }
                    var scale = ei.ManualScale;
                    if (bIsShiftDown)
                    {
                        switch (CurBodyOperationPart)
                        {
                            case BodyOperationPart.BorderL:
                                CurBodyOperationPart = BodyOperationPart.CornerLU;
                                offset.Y = (int)(offset.X / curSizeProportion);
                                break;
                            case BodyOperationPart.BorderR:
                                CurBodyOperationPart = BodyOperationPart.CornerRD;
                                offset.Y = (int)(offset.X / curSizeProportion);
                                break;
                            case BodyOperationPart.BorderU:
                                CurBodyOperationPart = BodyOperationPart.CornerLU;
                                offset.X = (int)(offset.Y * curSizeProportion);
                                break;
                            case BodyOperationPart.BorderD:
                                CurBodyOperationPart = BodyOperationPart.CornerRD;
                                offset.X = (int)(offset.Y * curSizeProportion);
                                break;
                            default:
                                offset.X = (int)(offset.Y * curSizeProportion);
                                break;
                        }
                    }
                    else if (ProjectDoc.Instance.Option.ScaleElementInProportionAtCorner &&
                        CurBodyOperationPart >= BodyOperationPart.CornerLU && CurBodyOperationPart <= BodyOperationPart.CornerRD)
                    {
                        switch (CurBodyOperationPart)
                        {
                            case BodyOperationPart.CornerLU:
                            case BodyOperationPart.CornerRD:
                                offset.X = (int)(offset.Y * curSizeProportion);
                                break;
                            case BodyOperationPart.CornerRU:
                            case BodyOperationPart.CornerLD:
                                offset.X = -(int)(offset.Y * curSizeProportion);
                                break;
                        }
                    }
                    //float imgScale = vi.BackImage == null ? 1 : vi.BackImage.CurImageScale;
                    //var offsetR = vi.GetLocationRate(true, new PointF(offset.X, offset.Y), true);
                    switch (CurBodyOperationPart)
                    {
                        case BodyOperationPart.BorderL:
                            scale.Width = (-(float)offset.X * 2 + sizeBegin.Width) / sizeImage.Width;
                            break;
                        case BodyOperationPart.BorderR:
                            scale.Width = ((float)offset.X * 2 + sizeBegin.Width) / sizeImage.Width;
                            break;
                        case BodyOperationPart.BorderU:
                            scale.Height = (-(float)offset.Y * 2 + sizeBegin.Height) / sizeImage.Height;
                            break;
                        case BodyOperationPart.BorderD:
                            scale.Height = ((float)offset.Y * 2 + sizeBegin.Height) / sizeImage.Height;
                            break;
                        case BodyOperationPart.CornerLU:
                            scale.Width = (-(float)offset.X * 2 + sizeBegin.Width) / sizeImage.Width;
                            scale.Height = (-(float)offset.Y * 2 + sizeBegin.Height) / sizeImage.Height;
                            break;
                        case BodyOperationPart.CornerRD:
                            scale.Width = ((float)offset.X * 2 + sizeBegin.Width) / sizeImage.Width;
                            scale.Height = ((float)offset.Y * 2 + sizeBegin.Height) / sizeImage.Height;
                            break;
                        case BodyOperationPart.CornerRU:
                            scale.Width = ((float)offset.X * 2 + sizeBegin.Width) / sizeImage.Width;
                            scale.Height = (-(float)offset.Y * 2 + sizeBegin.Height) / sizeImage.Height;
                            break;
                        case BodyOperationPart.CornerLD:
                            scale.Width = (-(float)offset.X * 2 + sizeBegin.Width) / sizeImage.Width;
                            scale.Height = ((float)offset.Y * 2 + sizeBegin.Height) / sizeImage.Height;
                            break;
                    }
                    if (scale.Width != 0 && scale.Height != 0)
                    {
                        if (vi.IsAnimEditingMode)
                            OperationHistory.Instance.PushOperation(new Operation_Element_ChangeScale_Trans(ei, scale));
                        else
                            OperationHistory.Instance.PushOperation(new Operation_Element_ChangeScale(ei, scale));
                    }
                }
            }
        }

        public void OnPanelView_MouseDoubleClick(MouseEventArgs e)
        {
            var ei = ProjectDoc.Instance.SelectedElementInfo;
            if (ei != null)
            {
                if (ei.IsInnerEditable)
                    ei.IsInnerEditingMode = true;
                else if (ChkBtn_AnimElement.Enabled)
                    ChkBtn_AnimElement.Checked = true;
            }
        }

        public void OnPanelView_MouseUp(MouseEventArgs e)
        {
            CurBodyOperation = BodyOperation.None;
            if (DragingInfo.CursorPosBegin.HasValue)
                Cursor.Position = DragingInfo.CursorPosBegin.Value;
            DragingInfo.CursorPosBegin = null;
            Cursor.Show();
            DragingInfo.ElmDragOffset = null;
            if (DragingInfo.BackViewDragOffset != null)
            {
                // 右键按下但没有拖动过时，选中当前窗口并取消当前动画编辑模式
                if (DragingInfo.BackImageBeginOffset == null)
                    CancelElementSelectionAndAnimEdition();
                DragingInfo.BackViewDragOffset = null;
            }
            if (PropertyBar != null)
                PropertyBar.Refresh();
            if (ProjectDoc.Instance.SelectedElementInfo == null || !ProjectDoc.Instance.SelectedElementInfo.IsInnerEditingMode)
                OperationHistory.Instance.CommitOperation();
        }

        public void OnPanelView_PreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void CancelElementSelectionAndAnimEdition()
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (ProjectDoc.Instance.SelectedElementInfo != null)
                ProjectDoc.Instance.SelectedElementInfo.IsInnerEditingMode = false;
            if (!ProjectDoc.Instance.Option.UseSafeViewportSwitching && vi.TreeNode != null)
                vi.TreeNode.Selected = true;
            if (vi.IsAnimEditingMode)
                vi.IsAnimEditingMode = false;
        }

        public void OnChkBtnAnimElement_CheckedChanged(DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (ProjectDoc.Instance.SelectedElementInfo == null)
                return;
            if (ChkBtn_AnimElement.Checked)
            {
                var elm = ProjectDoc.Instance.SelectedElementInfo;
                if (CurEditingAnimModeViewport != null)
                    CurEditingAnimModeViewport.IsAnimEditingMode = false;
                CurEditingAnimModeViewport = elm.ParentViewport;
                CurEditingAnimModeViewport.IsAnimEditingMode = true;
            }
            else if (CurEditingAnimModeViewport != null && CurEditingAnimModeViewport.IsAnimEditingMode)
                CurEditingAnimModeViewport.IsAnimEditingMode = false;
        }

        public void OnChkBtnToolElement_Mask_ItemClicked()
        {
            if (ChkBtn_ToolElement_Mask == null || !ChkBtn_ToolElement_Mask.Checked)
            {
                SelectedElementToolType = null;
                CheckingToolElementButton = null;
            }
            else
            {
                CheckingToolElementButton = ChkBtn_ToolElement_Mask;
                SelectedElementToolType = typeof(ElementInfo_Mask);
                if (ChkBtn_ToolElement_Waterbag != null)
                    ChkBtn_ToolElement_Waterbag.Checked = false;
                if (ChkBtn_ToolElement_TipText != null)
                    ChkBtn_ToolElement_TipText.Checked = false;
            }
        }

        public void OnChkBtnToolElement_Waterbag_ItemClicked()
        {
            if (ChkBtn_ToolElement_Waterbag == null || !ChkBtn_ToolElement_Waterbag.Checked)
            {
                SelectedElementToolType = null;
                CheckingToolElementButton = null;
            }
            else
            {
                CheckingToolElementButton = ChkBtn_ToolElement_Waterbag;
                SelectedElementToolType = typeof(ElementInfo_Waterbag);
                if (ChkBtn_ToolElement_Mask != null)
                    ChkBtn_ToolElement_Mask.Checked = false;
                if (ChkBtn_ToolElement_TipText != null)
                    ChkBtn_ToolElement_TipText.Checked = false;
            }
        }

        public void OnChkBtnToolElement_TipText_ItemClicked()
        {
            if (ChkBtn_ToolElement_TipText == null || !ChkBtn_ToolElement_TipText.Checked)
            {
                SelectedElementToolType = null;
                CheckingToolElementButton = null;
            }
            else
            {
                CheckingToolElementButton = ChkBtn_ToolElement_TipText;
                SelectedElementToolType = typeof(ElementInfo_TipText);
                if (ChkBtn_ToolElement_Mask != null)
                    ChkBtn_ToolElement_Mask.Checked = false;
                if (ChkBtn_ToolElement_Waterbag != null)
                    ChkBtn_ToolElement_Waterbag.Checked = false;
            }
        }

        public void OnChkBtnCombineElement_ItemClicked()
        {
            var ei = ProjectDoc.Instance.SelectedElementInfo;
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (ei != null && vi != null && ChkBtn_CombineElement.Checked && OperatingElementGroup == null)
            {
                OperatingElementGroup = ei.ParentElementGroup;
                if (OperatingElementGroup == null)
                    OperatingElementGroup = new ElementGroup(vi.ElemGroupCollector, ei);
            }
        }

        public void OnChkBtnCombineElement_CheckChanged()
        {
            if (!ChkBtn_CombineElement.Checked)
            {
                if (OperatingElementGroup != null)
                {
                    if (OperatingElementGroup.Elements.Count < 2)
                        OperatingElementGroup.ParentCollector.RemoveGroup(OperatingElementGroup);
                    OperatingElementGroup = null;
                }
            }
        }

        public void OnBtnClick_iMoveElementUp()
        {
            var ei = ProjectDoc.Instance.SelectedElementInfo;
            if (ei != null && ei.DepthLevel > 0 && ei.DepthLevel < ei.ParentViewport.Elements.Count - 1)
            {
                OperationHistory.Instance.CommitOperation(new Operation_Element_Drift(ei, 1));
                OperationHistory.Instance.IsDirty = true;
                RefreshProjectTree();
            }
        }

        public void OnBtnClick_iMoveElementDown()
        {
            var ei = ProjectDoc.Instance.SelectedElementInfo;
            if (ei != null && ei.DepthLevel > 1)
            {
                OperationHistory.Instance.CommitOperation(new Operation_Element_Drift(ei, -1));
                OperationHistory.Instance.IsDirty = true;
                RefreshProjectTree();
            }
        }

        public void OnBtnClick_iDeleteElement()
        {
            var ei = ProjectDoc.Instance.SelectedElementInfo;
            if (ei != null)
            {
                if (TransCtrl != null && TransCtrl.Focused && TransCtrl.Drawer.BindedElement == ei && TransCtrl.Drawer.SelectedRange != null)
                    TransCtrl.tsmiRemoveTransition_Click(null, null);
                else
                {
                    OperationHistory.Instance.CommitOperation(new Operation_Element_Delete(ei));
                    ProjectDoc.Instance.SelectedElementInfo = null;
                    OperationHistory.Instance.IsDirty = true;
                    RefreshProjectTree();
                }
            }
        }

        public void OnBtnClick_iCopyElement()
        {
            CopiedElement = ProjectDoc.Instance.SelectedElementInfo;
            CuttedElement = null;
            if (Btn_PasteElement != null)
                Btn_PasteElement.Enabled = CopiedElement != null;
        }

        public void OnBtnClick_iCutElement()
        {
            CuttedElement = ProjectDoc.Instance.SelectedElementInfo;
            CopiedElement = null;
            if (Btn_CutElement != null)
                Btn_CutElement.Enabled = CuttedElement != null;
        }

        public void OnBtnClick_iPasteElement()
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (CopiedElement != null)
                OperationHistory.Instance.CommitOperation(new Operation_Element_Duplicate(CopiedElement, vi));
            else if (CuttedElement != null)
            {
                OperationHistory.Instance.CommitOperation(new Operation_Element_ChangeViewport(CuttedElement, vi));
                CuttedElement = null;
            }
        }

        public void OnPropertyBar_PropertyValueChanged(PropertyValueChangedEventArgs e)
        {
            if (PropertyBar == null)
                return;
            var ei = PropertyBar.SelectedObject as ElementInfo;
            if (ei == null)
            {
                OperationHistory.Instance.IsDirty = true;
                return;
            }
            object newValue = e.ChangedItem.Value;
            object oldValue = e.OldValue;
            string prpName = e.ChangedItem.PropertyDescriptor.Name;
            if (e.ChangedItem.Parent != null && e.ChangedItem.Parent.GridItemType == GridItemType.Property)
            {
                if (e.ChangedItem.Parent.PropertyDescriptor.PropertyType == typeof(System.Drawing.Point))
                {
                    var v = (System.Drawing.Point)e.ChangedItem.Parent.Value;
                    if (prpName == "X")
                        v.X = (int)e.OldValue;
                    else if (prpName == "Y")
                        v.Y = (int)e.OldValue;
                    oldValue = v;
                }
                else if (e.ChangedItem.Parent.PropertyDescriptor.PropertyType == typeof(System.Drawing.PointF))
                {
                    var v = (System.Drawing.PointF)e.ChangedItem.Parent.Value;
                    if (prpName == "X")
                        v.X = (float)e.OldValue;
                    else if (prpName == "Y")
                        v.Y = (float)e.OldValue;
                    oldValue = v;
                }
                else if (e.ChangedItem.Parent.PropertyDescriptor.PropertyType == typeof(System.Drawing.Size))
                {
                    var v = (System.Drawing.Size)e.ChangedItem.Parent.Value;
                    if (prpName == "Width")
                        v.Width = (int)e.OldValue;
                    else if (prpName == "Height")
                        v.Height = (int)e.OldValue;
                    oldValue = v;
                }
                else if (e.ChangedItem.Parent.PropertyDescriptor.PropertyType == typeof(System.Drawing.SizeF))
                {
                    var v = (System.Drawing.SizeF)e.ChangedItem.Parent.Value;
                    if (prpName == "Width")
                        v.Width = (float)e.OldValue;
                    else if (prpName == "Height")
                        v.Height = (float)e.OldValue;
                    oldValue = v;
                }
                else
                    return;
                newValue = e.ChangedItem.Parent.Value;
                prpName = e.ChangedItem.Parent.PropertyDescriptor.Name;
            }
            OperationHistory.Instance.CommitOperation(new Operation_Element_ChangeProperty(ei, prpName, oldValue, newValue));
            if (prpName == "Alpha")
            {
                if (ei.ParentViewport.IsAnimEditingMode)
                    OperationHistory.Instance.CommitOperation(new Operation_Element_ChangeAlpha_Trans(ei, (float)oldValue, (float)newValue));
            }
        }

        public bool ConfirmSelectCurrentViewport()
        {
            if (ProjectDoc.Instance.SelectedTaskInfo != null)
            {
                var si = ProjectDoc.Instance.SelectedTaskInfo.SelectedSceneInfo;
                if (si != null)
                {
                    BindDelegatesToSceneInfo(si, true);
                    if (si.Viewports[0].TreeNode != null)
                    {
                        si.Viewports[0].TreeNode.Selected = true;
                        return true;
                    }
                }
            }
            return false;
        }
        public void OnCreatingNewProject()
        {
            CurEditingAnimModeViewport = null;
        }

        public void RefreshAppProjectTitle()
        {
            var t1 = ProductInfo.AssemblyTitle;
            var t2 = ProductInfo.AssemblyProduct;
            if (t2.Length > 0)
                t2 = " " + t2;
            var t3 = "";
            if (!String.IsNullOrEmpty(CustomAppTitleTailText))
                t3 = CustomAppTitleTailText;
            else if (ProjectDoc.Instance.SelectedProject != null)
                t3 = ProjectDoc.Instance.SelectedProject.Name;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is DevExpress.XtraBars.Ribbon.RibbonForm)
                {
                    if (t3.Length == 0)
                        frm.Text = String.Format("{0}{1}", t1, t2);
                    else
                        frm.Text = String.Format("{0}{1} - {2}", t1, t2, t3);
                }
            }
        }
        public void RefreshResourceTree(bool reloadFiles)
        {
            ResourceTree.Nodes.Clear();
            if (reloadFiles)
                ProjectDoc.Instance.ReloadResourceFiles();
            foreach (ResourceKind rp in Enum.GetValues(typeof(ResourceKind)))
            {
                if (ResourceInfo.ResTypes[rp] == typeof(ResourceInfo_Dummy))
                    continue;
                ResourceGroup rg = ProjectDoc.Instance.ResourceGroups[rp];
                var node = ResourceTree.Nodes.Add(new object[] { rg });
                node.Tag = rg;
                int imgIdx = IconImageIndices[rp][0];
                int subImgIdx = IconImageIndices[rp][1];
                node.ImageIndex = node.SelectImageIndex = imgIdx;
                bool depthed = false;
                foreach (var kv in rg.ResInfos)
                {
                    DevExpress.XtraTreeList.Nodes.TreeListNode subnode = null;
                    if (kv.Key == "\\")
                        subnode = node;
                    else if (ForbiddingResourcePathNames.Contains(kv.Key.Trim('\\')))
                        continue;
                    else
                    {
                        depthed = true;
                        subnode = node.Nodes.Add(new object[] { kv.Key });
                        subnode.ImageIndex = subnode.SelectImageIndex = ExtraIconImageIndices[0];
                        subnode.Tag = kv.Key;
                    }
                    foreach (var info in kv.Value)
                    {
                        var childnode = subnode.Nodes.Add(new object[] { info });
                        childnode.ImageIndex = childnode.SelectImageIndex = subImgIdx;
                        childnode.Tag = info;
                    }
                }
                if (depthed)
                    node.Expanded = true;
            }
        }
        public void RefreshProjectTree()
        {
            var _ti = ProjectDoc.Instance.SelectedTaskInfo;
            var _si = ProjectDoc.Instance.SelectedSceneInfo;
            var _vi = ProjectDoc.Instance.SelectedViewportInfo;
            var _ei = ProjectDoc.Instance.SelectedElementInfo;

            ProjectTree.BeginInit();
            var pi = ProjectDoc.Instance.SelectedProject;
            ProjectTree.Nodes.Clear();
            if (pi != null)
            {
                foreach (var ti in pi.TaskInfos)
                {
                    var tnode = ProjectTree.Nodes.Add(new object[] { ti });
                    tnode.Tag = ti;
                    tnode.ImageIndex = tnode.SelectImageIndex = ExtraIconImageIndices[1];
                    ti.TreeNode = tnode;
                    foreach (var si in ti.SceneInfos)
                    {
                        var snode = tnode.Nodes.Add(new object[] { si });
                        snode.Tag = si;
                        snode.ImageIndex = snode.SelectImageIndex = ExtraIconImageIndices[2];
                        if (ProjectDoc.Instance.SelectedSceneInfo == si)
                            snode.Selected = true;
                        si.TreeNode = snode;
                        foreach (var vi in si.Viewports)
                        {
                            var vnode = snode.Nodes.Add(new object[] { vi });
                            vnode.Tag = vi;
                            vnode.ImageIndex = vnode.SelectImageIndex = ExtraIconImageIndices[3];
                            vi.TreeNode = vnode;
                            foreach (var ei in vi.Elements)
                            {
                                if (ei != null && ei.CanSelect)
                                {
                                    var enode = vnode.Nodes.Add(new object[] { ei });
                                    enode.Tag = ei;
                                    enode.ImageIndex = enode.SelectImageIndex = IconImageIndices[ei.Resource.Kind][1];
                                    ei.TreeNode = enode;
                                }
                            }
                        }
                        snode.Expanded = true;
                    }
                    tnode.Expanded = true;
                }
            }
            ProjectTree.EndInit();

            if (_ei != null)
                _ei.TreeNode.Selected = true;
            else if (_vi != null)
                _vi.TreeNode.Selected = true;
            else if (_si != null)
                _si.TreeNode.Selected = true;
            else if (_ti != null)
                _ti.TreeNode.Selected = true;
        }
    }
}
