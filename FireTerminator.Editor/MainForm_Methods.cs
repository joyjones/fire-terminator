using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common;
using DevExpress.XtraTreeList.Nodes;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Operations;
using FireTerminator.Common.RenderResources;

namespace FireTerminator.Editor
{
    public partial class MainForm
    {
        private void InitEditViewOperator()
        {
            m_EditViewOperater = new EditViewOperater(this, pnlMain, pnlPreview);
            m_EditViewOperater.ChkBtn_AnimElement = icAnimElement;
            m_EditViewOperater.ChkBtn_MoveElement = icMoveElement;
            m_EditViewOperater.ChkBtn_RotateElement = icRotateElement;
            m_EditViewOperater.ChkBtn_ScaleElement = icScaleElement;
            m_EditViewOperater.ChkBtn_DriveElement = barCheckItem1;
            m_EditViewOperater.ChkBtn_CombineElement = icCombineElement;
            m_EditViewOperater.ChkBtn_ToolElement_Mask = icToolMask;
            m_EditViewOperater.ChkBtn_ToolElement_Waterbag = icToolWaterbag;
            m_EditViewOperater.ChkBtn_ToolElement_TipText = icToolTipText;
            m_EditViewOperater.Btn_CopyElement = iCopyElement;
            m_EditViewOperater.Btn_CutElement = iCutElement;
            m_EditViewOperater.Btn_PasteElement = iPasteElement;
            m_EditViewOperater.ProjectTree = trvProject;
            m_EditViewOperater.ResourceTree = trvResources;
            m_EditViewOperater.TransCtrl = tcTransCtrl;
            m_EditViewOperater.PropertyBar = grdProperties;
            m_EditViewOperater.Cursor_ToolTip = toolTipOnCursor;
            m_EditViewOperater.IconImageIndices[ResourceKind.图像] = new int[] { 16, 26 };
            m_EditViewOperater.IconImageIndices[ResourceKind.背景] = new int[] { 32, 26 };
            m_EditViewOperater.IconImageIndices[ResourceKind.效果] = new int[] { 21, 39 };
            m_EditViewOperater.IconImageIndices[ResourceKind.视频] = new int[] { 37, 38 };
            m_EditViewOperater.IconImageIndices[ResourceKind.音频] = new int[] { 44, 43 };
            m_EditViewOperater.IconImageIndices[ResourceKind.遮罩] = new int[] { 22, 26 };
            m_EditViewOperater.IconImageIndices[ResourceKind.水带] = new int[] { 22, 26 };
            m_EditViewOperater.IconImageIndices[ResourceKind.文本] = new int[] { 22, 26 };
            m_EditViewOperater.ExtraIconImageIndices[0] = 3;
            m_EditViewOperater.ExtraIconImageIndices[1] = 40;
            m_EditViewOperater.ExtraIconImageIndices[2] = 41;
            m_EditViewOperater.ExtraIconImageIndices[3] = 42;
        }

        public void BeforeResetProject()
        {
            icToolMask.Checked = false;
            icToolTipText.Checked = false;
            icToolWaterbag.Checked = false;
            icCombineElement.Checked = false;
        }
        public void AfterResetProject()
        {
            icViewportMaximium.Checked = false;
            GC.Collect();
        }
        public bool ResetProject()
        {
            BeforeResetProject();
            if (CheckSaveCurrentProject(true))
            {
                m_EditViewOperater.OnCreatingNewProject();
                ProjectDoc.Instance.CreateProject(false);
                m_EditViewOperater.RefreshProjectTree();
                m_EditViewOperater.RefreshAppProjectTitle();
                RefreshOnCurOperationChanged(null);
                AfterResetProject();
                return true;
            }
            return false;
        }
        public void RefreshOnCurOperationChanged(Operation opt)
        {
            iUndo.Enabled = OperationHistory.Instance.CanUndo;
            iRedo.Enabled = OperationHistory.Instance.CanRedo;
        }
    }
}
