using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Operations;

namespace FireTerminator.Common.Transitions
{
    public class TransitionGraphics
    {
        public TransitionGraphics(DevExpress.XtraEditors.XtraScrollableControl pnlContainer, Panel pnlParent, int rulerHeight, int lineHeight)
        {
            ParentPanel = pnlParent;
            ParentPanelContainter = pnlContainer;
            ParentPanelContainter.SizeChanged += new EventHandler(ParentPanelContainer_SizeChanged);
            PanelViewport.X = 0;
            PanelViewport.Y = 0;
            IsTimeRulerSelected = false;
            ParentPanelContainer_SizeChanged(null, null);
            TimeRuler = new TransitionTimeRuler(this, rulerHeight);
            int yPos = rulerHeight;
            foreach (TransitionKind tk in Enum.GetValues(typeof(TransitionKind)))
            {
                if (tk == TransitionKind.Unknown)
                    continue;
                Lines[tk] = new TransitionLine(tk, this, yPos, lineHeight);
                yPos += lineHeight;
            }
            OperationHistory.Instance.NewOperationCommited += new OperationHistory.Delegate_OnOperationChanged(OperationHistory_OnCommitedNewOperation);
        }

        [Browsable(false)]
        public Panel ParentPanel
        {
            get;
            private set;
        }
        [Browsable(false)]
        public DevExpress.XtraEditors.XtraScrollableControl ParentPanelContainter
        {
            get;
            private set;
        }
        [Browsable(false)]
        public int CurViewOffsetX
        {
            get { return ParentPanelContainter.AutoScrollPosition.X; }
        }
        private ElementInfo m_BindedElement = null;
        [Browsable(false)]
        public ElementInfo BindedElement
        {
            get { return m_BindedElement; }
            set
            {
                if (m_BindedElement != value)
                {
                    m_BindedElement = value;
                    RefreshTransitionLine(TransitionKind.Unknown);
                }
            }
        }
        [Browsable(false)]
        public TransitionTimeRuler TimeRuler
        {
            get;
            private set;
        }
        [Category("动画面板"), DisplayName("窗口动画总时间")]
        public float MaxTimeLength
        {
            get
            {
                ViewportInfo vi = null;
                if (m_BindedElement != null)
                    vi = m_BindedElement.ParentViewport;
                else
                    vi = ProjectDoc.Instance.SelectedViewportInfo;
                if (vi != null)
                    return vi.TimeLength;
                return 0;
            }
            set
            {
                CommonMethods.ClampValue(ref value, 0, 9999);
                ViewportInfo vi = null;
                if (m_BindedElement != null)
                    vi = m_BindedElement.ParentViewport;
                else
                    vi = ProjectDoc.Instance.SelectedViewportInfo;
                if (vi != null)
                    vi.TimeLength = value;
            }
        }
        [Browsable(false)]
        public TransitionLine FocusedLine
        {
            get
            {
                foreach (var ln in Lines.Values)
                {
                    if (ln.IsFocused)
                        return ln;
                }
                return null;
            }
        }
        [Browsable(false)]
        public float PixelsPerSecond
        {
            get { return 5 * ViewScale; }
        }
        [Browsable(false)]
        public float CurMouseFocusTime
        {
            get
            {
                if (TimeRuler.CurFocusTimePixelWidth <= 0)
                    return 0;
                return TimeRuler.CurFocusTimePixelWidth / (float)PixelsPerSecond;
            }
        }
        private float m_ViewScale = 4;
        [Category("动画面板"), DisplayName("视图放缩比例")]
        public float ViewScale
        {
            get { return m_ViewScale; }
            set { m_ViewScale = value; }
        }
        [Browsable(false)]
        public System.Drawing.Size ViewSize
        {
            get { return ParentPanel.Size; }
        }
        [Browsable(false)]
        public TransitionRange FocusRange
        {
            get;
            private set;
        }
        private TransitionRange m_SelectedRange = null;
        [Browsable(false)]
        public TransitionRange SelectedRange
        {
            get { return m_SelectedRange; }
            set
            {
                if (m_SelectedRange != value)
                {
                    m_SelectedRange = value;
                    if (m_SelectedRange != null && !BindedElement.ParentViewport.IsPlaying)
                        TimeRuler.CurTime = m_SelectedRange.OwnerTrans.TimeEnd;
                    if (TransitionRangeSelectChanged != null)
                        TransitionRangeSelectChanged(m_SelectedRange);
                }
            }
        }
        [Browsable(false)]
        public bool IsTimeRulerSelected
        {
            get;
            set;
        }
        [Browsable(false)]
        public float CurEditFocusTime
        {
            get
            {
                if (IsTimeRulerSelected || SelectedRange != null)
                    return TimeRuler.CurTime;
                return CurMouseFocusTime;
            }
        }

        public void Clear()
        {
            foreach (var ln in Lines.Values)
            {
                ln.Clear();
            }
        }
        private void ParentPanelContainer_SizeChanged(object sender, EventArgs e)
        {
            PanelViewport.Width = ParentPanelContainter.Width;
            PanelViewport.Height = ParentPanelContainter.Height;
        }
        private void ConfirmUsingEffect(GraphicsDevice device)
        {
            if (UsingEffect == null && device != null)
            {
                UsingEffect = new BasicEffect(device, null);
                UsingEffect.World = Matrix.Identity;
                UsingEffect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
                UsingEffect.VertexColorEnabled = true;
            }
            if (UsingEffect != null)
            {
                UsingEffect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)PanelViewport.Width, (float)PanelViewport.Height, 0, 1.0f, 1000.0f);
            }
        }
        public TransitionLine GetLine(TransitionKind kind)
        {
            TransitionLine ln = null;
            Lines.TryGetValue(kind, out ln);
            return ln;
        }
        public TransitionKind? GetTransitionKind(int yPos)
        {
            TransitionKind? tk = null;
            foreach (var ln in Lines.Values)
            {
                if (yPos >= ln.OffsetY && yPos < ln.OffsetY + ln.Height)
                {
                    tk = ln.Kind;
                    break;
                }
            }
            return tk;
        }
        public TransitionLine SetFocusLine(TransitionKind lnKind)
        {
            foreach (var ln in Lines.Values)
                ln.IsFocused = false;
            if (lnKind == TransitionKind.Unknown)
                return null;
            Lines[lnKind].IsFocused = true;
            return Lines[lnKind];
        }
        public TransitionLine SetFocusLine(int yPos)
        {
            TransitionKind? tk = GetTransitionKind(yPos);
            if (tk.HasValue)
                return SetFocusLine(tk.Value);
            return SetFocusLine(TransitionKind.Unknown);
        }
        public void Update(float elapsedTime)
        {
            TimeRuler.RenderOffsetX = CurViewOffsetX;
            TimeRuler.Update(elapsedTime);
            foreach (var ln in Lines.Values)
            {
                ln.RenderOffsetX = CurViewOffsetX;
                ln.Update(elapsedTime);
            }
        }
        public void Draw(GraphicsDevice device)
        {
            device.Clear(Color.Black);
            ConfirmUsingEffect(device);
            UsingEffect.Begin();
            device.Viewport = PanelViewport;
            foreach (EffectPass pass in UsingEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                foreach (var ln in Lines.Values)
                    ln.Draw(device);
                TimeRuler.Draw(device);
                pass.End();
            }
            UsingEffect.End();

            var rect = device.Viewport.TitleSafeArea;
            var rectDest = device.Viewport.TitleSafeArea;
            rectDest.X -= CurViewOffsetX;
            device.Present(rect, rectDest, (IntPtr)ParentPanel.Handle);
        }
        public void OnMouseMove(System.Drawing.Point pos)
        {
            TimeRuler.CurFocusTimePixelWidth = pos.X;
            if (SelectedRange != null && RangeDraggingOffsetX >= 0)
            {
                var ln = SelectedRange.ParentLine;
                if (SelectedRange.CurFocusRangeOptPart == BodyOperationPart.Body)
                {
                    int x = pos.X - RangeDraggingOffsetX;
                    x = (int)CommonMethods.ClampValue((float)x, 0, (float)TimeRuler.CurMaxTimePixelWidth);
                    int rx = x + (int)SelectedRange.Region.Width;
                    foreach (var r in ln.Ranges.Values)
                    {
                        if (r == SelectedRange)
                            continue;
                        if (x >= r.Region.Left && x < r.Region.Right)
                            x = (int)r.Region.Right;
                        else if (rx >= r.Region.Left && rx < r.Region.Right)
                            x = (int)(r.Region.Left - SelectedRange.Region.Width);
                        else if (x < r.Region.Left && rx >= r.Region.Right)
                            x = (int)r.Region.Right;
                    }
                    SelectedRange.OwnerTrans.TimeBegin = x / (float)PixelsPerSecond;
                }
                else if (SelectedRange.CurFocusRangeOptPart == BodyOperationPart.BorderL)
                {
                    int x = pos.X - RangeDraggingOffsetX;
                    x = (int)CommonMethods.ClampValue((float)x, 0, (float)TimeRuler.CurMaxTimePixelWidth);
                    foreach (var r in ln.Ranges.Values)
                    {
                        if (r != SelectedRange && r.Region.Right < SelectedRange.Region.Right && r.Region.Right > x)
                            x = (int)r.Region.Right + 1;
                    }
                    float tBegin = x / (float)PixelsPerSecond;
                    float tEnd = SelectedRange.OwnerTrans.TimeEnd;
                    if (tEnd - tBegin < 1)
                        tBegin = tEnd - 1;
                    SelectedRange.OwnerTrans.TimeBegin = tBegin;
                    SelectedRange.OwnerTrans.TimeLength = tEnd - tBegin;
                }
                else if (SelectedRange.CurFocusRangeOptPart == BodyOperationPart.BorderR)
                {
                    pos.X = (int)CommonMethods.ClampValue((float)pos.X, 0, (float)TimeRuler.CurMaxTimePixelWidth);
                    foreach (var r in ln.Ranges.Values)
                    {
                        if (r != SelectedRange && r.Region.Left > SelectedRange.Region.Left && r.Region.X <= pos.X)
                            pos.X = (int)r.Region.Left - 1;
                    }
                    int w = pos.X - (int)SelectedRange.Region.X;
                    if (w < PixelsPerSecond)
                        w = (int)PixelsPerSecond;
                    SelectedRange.OwnerTrans.TimeLength = w / (float)PixelsPerSecond;
                }
            }
            else if (DraggingTimeRuler)
            {
                TimeRuler.CurTimePixelWidth = TimeRuler.CurFocusTimePixelWidth;
            }
            else
            {
                FocusRange = null;
                if (FocusedLine != null)
                {
                    System.Drawing.Point posRel = new System.Drawing.Point((int)(pos.X - FocusedLine.Region.X), (int)(pos.Y - FocusedLine.Region.Y));
                    FocusRange = FocusedLine.OnMouseMove(posRel);
                }
            }
            if (FocusRange != null)
            {
                switch (FocusRange.CurFocusRangeOptPart)
                {
                    case BodyOperationPart.BorderL:
                    case BodyOperationPart.BorderR:
                        Cursor.Current = Cursors.SizeWE; break;
                    case BodyOperationPart.Body:
                        Cursor.Current = Cursors.SizeAll; break;
                }
            }
        }
        public void OnMouseDown(MouseEventArgs e)
        {
            IsTimeRulerSelected = !(e.Y >= Lines[TransitionKind.隐藏].OffsetY && e.Y < Lines[TransitionKind.半透].BottomY);
            if (IsTimeRulerSelected)
            {
                FocusRange = null;
                SelectedRange = null;
            }
            else if (FocusedLine != null)
            {
                var tk = GetTransitionKind(e.Y);
                if (tk.HasValue && tk.Value == FocusedLine.Kind)
                {
                    SelectedRange = FocusRange;
                }
            }
            if (e.Button == MouseButtons.Left)
            {
                if (SelectedRange == null)
                    RangeDraggingOffsetX = -1;
                else
                    RangeDraggingOffsetX = e.X - (int)SelectedRange.Region.X;
                if (IsTimeRulerSelected)
                {
                    TimeRuler.CurTimePixelWidth = TimeRuler.CurFocusTimePixelWidth;
                    DraggingTimeRuler = true;
                }
            }
        }
        public void OnMouseUp(MouseEventArgs e)
        {
            RangeDraggingOffsetX = -1;
            DraggingTimeRuler = false;
            if (SelectedRange != null)
            {
                if (TransitionRangeSelectChanged != null)
                    TransitionRangeSelectChanged(SelectedRange);
            }
        }
        private void OperationHistory_OnCommitedNewOperation(Operation opt)
        {
        }
        public void RefreshTransitionLine(TransitionKind kind)
        {
            if (kind != TransitionKind.Unknown)
                Lines[kind].Refresh();
            else
            {
                foreach (TransitionKind tk in Enum.GetValues(typeof(TransitionKind)))
                {
                    if (tk != TransitionKind.Unknown)
                        Lines[tk].Refresh();
                }
            }
        }

        public delegate void Delegate_OnTransitionRangeSelectChanged(TransitionRange tr);
        public event Delegate_OnTransitionRangeSelectChanged TransitionRangeSelectChanged;
        private int RangeDraggingOffsetX = -1;
        private bool DraggingTimeRuler = false;
        public Dictionary<TransitionKind, TransitionLine> Lines = new Dictionary<TransitionKind, TransitionLine>();
        private BasicEffect UsingEffect = null;
        private Viewport PanelViewport = new Viewport();
    }
}
