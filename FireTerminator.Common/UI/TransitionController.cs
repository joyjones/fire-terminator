using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using DevExpress.XtraTreeList.Nodes;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.Operations;

namespace FireTerminator.Common.UI
{
    public partial class TransitionController : UserControl
    {
        public TransitionController()
        {
            InitializeComponent();
            Enabled = false;
            Drawer = new TransitionGraphics(pnlMain, pnlDraw, trvTransHeaders.RowHeight + 2, trvTransHeaders.RowHeight + 1);
            Drawer.TimeRuler.CurTimeChanged += new TransitionTimeRuler.Delegate_CurTimeChanged(TimeRuler_CurTimeChanged);
            ProjectDoc.Instance.TransGraphics = Drawer;
        }

        public TransitionGraphics Drawer
        {
            get;
            set;
        }

        public ElementInfo ParentElement
        {
            get { return Drawer.BindedElement; }
            set
            {
                if (Drawer.BindedElement != value)
                {
                    Drawer.BindedElement = value;
                    if (Drawer.BindedElement == null)
                        Enabled = false;
                }
            }
        }

        private void TransitionController_Load(object sender, EventArgs e)
        {
            trvTransHeaders.Nodes.Clear();
            foreach (TransitionKind tk in Enum.GetValues(typeof(TransitionKind)))
            {
                if (tk == TransitionKind.Unknown)
                    continue;
                TreeListNode node = trvTransHeaders.Nodes.Add(new object[] { tk });
                node.Tag = tk;
            }
            //pnlDraw.MouseWheel += new MouseEventHandler(pnlDraw_MouseWheel);
            trvTransHeaders.MouseWheel += new MouseEventHandler(pnlDraw_MouseWheel);
        }

        private void TimeRuler_CurTimeChanged(float time)
        {
            int s = ((int)time) % 60;
            int m = ((int)time) / 60;
            int ms = (int)((time - (int)time) * 1000);
            string _s = s >= 10 ? s.ToString() : ("0" + s.ToString());
            string _m = m >= 10 ? m.ToString() : ("0" + m.ToString());
            string _ms = ms.ToString();
            if (_ms.Length == 1)
                _ms = "00" + _ms;
            else if (_ms.Length == 2)
                _ms = "0" + _ms;
            chkAnimPlay.Text = String.Format("{0}:{1}:{2}", _m, _s, _ms);
        }

        private void trvTransHeaders_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                Drawer.SetFocusLine((TransitionKind)e.Node.Tag);
                Drawer.SelectedRange = Drawer.FocusRange;
            }
        }

        public void tsmiAddTransition_Click(object sender, EventArgs e)
        {
            ParentElement.CreateOrSplitCurrentTransRange(Drawer.FocusedLine.Kind, Drawer.CurEditFocusTime);
        }

        public void tsmiRemoveTransition_Click(object sender, EventArgs e)
        {
            OperationHistory.Instance.CommitOperation(new Transition_Element_Delete(ParentElement, Drawer.SelectedRange.OwnerTrans));
        }

        private void trvTransHeaders_SizeChanged(object sender, EventArgs e)
        {
        }

        private void ctxMnuTransView_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = Drawer.FocusedLine == null || Drawer.BindedElement == null;
            tsmiAddTransition.Enabled = Drawer.FocusedLine != null && Drawer.SelectedRange == null;
            if (Drawer.FocusedLine != null)
                tsmiAddTransition.Text = String.Format("添加{0}变换", Drawer.FocusedLine.Kind.ToString());
            else
                tsmiAddTransition.Text = "添加变换";
            tsmiRemoveTransition.Enabled = Drawer.SelectedRange != null;
            tsmiClearLine.Enabled = Drawer.FocusedLine != null;
            if (Drawer.FocusedLine != null)
                tsmiClearLine.Text = String.Format("清除所有{0}变换", Drawer.FocusedLine.Kind.ToString());
            else
                tsmiClearLine.Text = "清除所有当前行变换";
            tsmiCopyTrans.Enabled = Drawer.SelectedRange != null;
            tsmiCutTrans.Enabled = Drawer.SelectedRange != null;
            bool bCanPaste = false;
            if (Drawer.FocusedLine != null)
            {
                if (m_CopiedElementTrans != null && Drawer.FocusedLine.Kind == m_CopiedElementTrans.Kind)
                    bCanPaste = true;
                else if (m_CuttedElementTrans != null && Drawer.FocusedLine.Kind == m_CuttedElementTrans.Kind)
                    bCanPaste = true;
            }
            tsmiPasteTrans.Enabled = bCanPaste;
        }

        private void pnlDraw_MouseDown(object sender, MouseEventArgs e)
        {
            //var pos = pnlDraw.AutoScrollPosition;
            //pnlDraw.Focus();
            //pnlDraw.AutoScrollPosition = pos;
            Drawer.OnMouseDown(e);
            var ln = Drawer.FocusedLine;
            if (ln != null)
                trvTransHeaders.Nodes[(int)ln.Kind - 1].Selected = true;
            else
                trvTransHeaders.FocusedNode = null;
        }

        private void pnlDraw_MouseMove(object sender, MouseEventArgs e)
        {
            Drawer.OnMouseMove(e.Location);
        }

        private void pnlDraw_MouseLeave(object sender, EventArgs e)
        {
            if (!ctxMnuTransView.Visible)
                Drawer.OnMouseMove(new System.Drawing.Point(-1, -1));
        }

        private void pnlDraw_MouseUp(object sender, MouseEventArgs e)
        {
            Drawer.OnMouseUp(e);
        }

        private void pnlDraw_MouseWheel(object sender, MouseEventArgs e)
        {
            IncreaseViewScale(e.Delta > 0);
        }

        private void pnlDraw_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //switch (e.KeyCode)
            //{
            //    case Keys.Add:
            //        IncreaseViewScale(true);
            //        break;
            //    case Keys.Subtract:
            //        IncreaseViewScale(false);
            //        break;
            //    case Keys.Space:
            //        chkAnimPlay.Checked = !chkAnimPlay.Checked;
            //        break;
            //}
        }

        protected void IncreaseViewScale(bool adding)
        {
            if (adding)
            {
                if (Drawer.ViewScale < 10)
                    Drawer.ViewScale += 0.1F;
            }
            else
            {
                if (Drawer.ViewScale > 0.2F)
                    Drawer.ViewScale -= 0.1F;
            }
        }

        private void TransitionController_EnabledChanged(object sender, EventArgs e)
        {
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null && (vi.IsPlaying || ProjectDoc.Instance.IsProjectAnimationPlaying))
                TimeRuler_CurTimeChanged(vi.CurTimeTick);
        }

        private void tsmiClearLine_Click(object sender, EventArgs e)
        {
            if (Drawer.FocusedLine != null)
            {
                OperationHistory.Instance.BeginTransaction();
                foreach (var trans in Drawer.FocusedLine.Ranges.Keys.ToArray())
                    OperationHistory.Instance.CommitOperation(new Transition_Element_Delete(ParentElement, trans));
                OperationHistory.Instance.EndTransaction();
            }
        }

        private void tsmiClearAll_Click(object sender, EventArgs e)
        {
            OperationHistory.Instance.BeginTransaction();
            foreach (var ln in Drawer.Lines.Values)
            {
                foreach (var trans in ln.Ranges.Keys.ToArray())
                    OperationHistory.Instance.CommitOperation(new Transition_Element_Delete(ParentElement, trans));
            }
            OperationHistory.Instance.EndTransaction();
        }

        private ElementTransform m_CopiedElementTrans = null;
        private ElementTransform m_CuttedElementTrans = null;
        private void tsmiCopyTrans_Click(object sender, EventArgs e)
        {
            if (Drawer.SelectedRange != null)
            {
                m_CuttedElementTrans = null;
                m_CopiedElementTrans = Drawer.SelectedRange.OwnerTrans;
            }
        }

        private void tsmiCutTrans_Click(object sender, EventArgs e)
        {
            if (Drawer.SelectedRange != null)
            {
                m_CopiedElementTrans = null;
                m_CuttedElementTrans = Drawer.SelectedRange.OwnerTrans;
                OperationHistory.Instance.CommitOperation(new Transition_Element_Delete(ParentElement, Drawer.SelectedRange.OwnerTrans));
            }
        }

        private void tsmiPasteTrans_Click(object sender, EventArgs e)
        {
            if (Drawer.FocusedLine != null)
            {
                if (m_CopiedElementTrans != null && m_CopiedElementTrans.Kind == Drawer.FocusedLine.Kind)
                {
                    float time = Drawer.IsTimeRulerSelected ? Drawer.TimeRuler.CurTime : Drawer.CurMouseFocusTime;
                    OperationHistory.Instance.CommitOperation(new Transition_Element_Duplicate(ParentElement, m_CopiedElementTrans, time));
                }
                else if (m_CuttedElementTrans != null && m_CuttedElementTrans.Kind == Drawer.FocusedLine.Kind)
                {
                    OperationHistory.Instance.CommitOperation(new Transition_Element_Add(ParentElement, m_CuttedElementTrans));
                    m_CuttedElementTrans = null;
                }
            }
        }

        private void chkAnimPlay_CheckedChanged(object sender, EventArgs e)
        {
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi != null)
                vi.IsPlaying = chkAnimPlay.Checked;
        }

        private void pnlMain_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void trvTransHeaders_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Add:
                    IncreaseViewScale(true);
                    break;
                case Keys.Subtract:
                    IncreaseViewScale(false);
                    break;
                case Keys.Space:
                    chkAnimPlay.Checked = !chkAnimPlay.Checked;
                    break;
            }
        }
    }
}
