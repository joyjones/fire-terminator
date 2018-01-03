using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FireTerminator.Common.Operations;

namespace FireTerminator.Common.Transitions
{
    public class TransitionLine : RenderingRectangle
    {
        public TransitionLine(TransitionKind kind, TransitionGraphics drawer, int yPos, int height)
        {
            Kind = kind;
            ParentDrawer = drawer;
            OffsetY = yPos;
            Height = height;
        }
        public TransitionKind Kind
        {
            get;
            private set;
        }
        public TransitionGraphics ParentDrawer
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            set;
        }
        public int OffsetY
        {
            get;
            set;
        }
        public int BottomY
        {
            get { return OffsetY + Height; }
        }
        public Color BackColor
        {
            get
            {
                if (IsFocused)
                    return CommonMethods.ConvertColor(ProjectDoc.Instance.Option.TransitionViewBackColorFocus);
                return CommonMethods.ConvertColor(ProjectDoc.Instance.Option.TransitionViewBackColor);
            }
        }
        private bool m_IsFocused = false;
        public bool IsFocused
        {
            get { return m_IsFocused; }
            set { m_IsFocused = value; }
        }

        public void Clear()
        {
            Ranges.Clear();
        }
        public void Update(float elapsedTime)
        {
            base.Update(0, OffsetY, ParentDrawer.ViewSize.Width, Height - 1, BackColor);
            foreach (var tr in Ranges.Values)
            {
                tr.RenderOffsetX = RenderOffsetX;
                tr.RenderOffsetY = RenderOffsetY;
                tr.Update(elapsedTime);
            }
        }
        public override void Draw(GraphicsDevice device)
        {
            base.Draw(device);
            foreach (var tr in Ranges.Values)
            {
                tr.Draw(device);
            }
        }
        public void Refresh()
        {
            Clear();
            if (ParentDrawer.BindedElement != null)
            {
                foreach (var t in ParentDrawer.BindedElement.GetTransitions(Kind))
                {
                    Ranges[t] = new TransitionRange(this, t);
                }
            }
        }
        public TransitionRange GetTransposeRange(ElementTransform trans)
        {
            TransitionRange tr = null;
            Ranges.TryGetValue(trans, out tr);
            return tr;
        }
        public virtual bool RemoveTimeKey(float time, out ElementTransform transRemoved)
        {
            transRemoved = ParentDrawer.BindedElement.GetTransitionOnTime(Kind, time, false);
            if (transRemoved == null)
                return false;
            float length = transRemoved.TimeLength;
            ParentDrawer.BindedElement.RemoveTransition(transRemoved);
            var transBefore = ParentDrawer.BindedElement.GetTransitionOnTime(Kind, time, true);
            if (transBefore != null)
                transBefore.TimeLength += length;
            return true;
        }
        public TransitionRange OnMouseMove(System.Drawing.Point pos)
        {
            if (pos.X >= 0 && pos.X < Region.Width && pos.Y >= 0 && pos.Y < Region.Height)
            {
                foreach (var tr in Ranges.Values)
                {
                    System.Drawing.Point posRel = new System.Drawing.Point((int)(pos.X - tr.Region.X), pos.Y);
                    BodyOperationPart part = tr.OnMouseMove(posRel);
                    if (part != BodyOperationPart.Nothing)
                        return tr;
                }
            }
            return null;
        }
        public Dictionary<ElementTransform, TransitionRange> Ranges = new Dictionary<ElementTransform, TransitionRange>();
    }
}
