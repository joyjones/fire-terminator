using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Transitions;

namespace FireTerminator.Common.Operations
{
    public class Operation_Element_ChangeLocation_Trans : Operation_Element
    {
        public Operation_Element_ChangeLocation_Trans(ElementInfo e, Point pos)
            : base(e)
        {
            LocationOld = e.Location;
            BlendedLocationOld = e.BlendedLocation;
            BlendedLocationNew = pos;
        }
        public override void Undo()
        {
            if (m_TransOperation != null)
                m_TransOperation.Undo();
        }
        public override void Do()
        {
            if (m_TransOperation != null)
                m_TransOperation.Do();
        }
        public override void TryDo()
        {
            Element.EnableTransitionBlending(TransitionKind.位移, false);

            var pos = Element.ParentViewport.GetLocationRate(Element.IsBasedOnBackImageElement, BlendedLocationNew);
            pos.X += Element.BlendedTrans.Wr * Element.Barycenter.X;
            pos.Y += Element.BlendedTrans.Hr * Element.Barycenter.Y;
            Element.BaseTrans.RateLocation = pos;
        }
        public override void Commit()
        {
            var trans = Element.CreateOrSplitCurrentTransRange(TransitionKind.位移, ProjectDoc.Instance.TransGraphics.CurEditFocusTime, out m_TransOperation) as ElementTransform_Move;
            if (trans == null && m_TransOperation != null)
            {
                m_TransOperation.Do();
                trans = m_TransOperation.ResultTransform as ElementTransform_Move;
            }
            if (trans != null)
            {
                Element.Location = LocationOld;
                var offsetNow = trans.Offset;
                var offsetNew = Offset;
                trans.Offset = new System.Drawing.Point(offsetNow.X + offsetNew.X, offsetNow.Y + offsetNew.Y);
            }
            Element.EnableTransitionBlending(TransitionKind.位移, true);
        }
        public override void Merge(Operation opt)
        {
            var o = opt as Operation_Element_ChangeLocation_Trans;
            BlendedLocationNew = o.BlendedLocationNew;
        }
        protected Transition_Element m_TransOperation = null;
        public Point BlendedLocationOld
        {
            get;
            protected set;
        }
        public Point LocationOld
        {
            get;
            protected set;
        }
        public Point BlendedLocationNew
        {
            get;
            protected set;
        }
        public Point Offset
        {
            get { return new Point(BlendedLocationNew.X - BlendedLocationOld.X, BlendedLocationNew.Y - BlendedLocationOld.Y); }
        }
    }

    public class Operation_Element_ChangeScale_Trans : Operation_Element_ChangeScale
    {
        public Operation_Element_ChangeScale_Trans(ElementInfo e, SizeF scale)
            : base(e, scale)
        {
        }
        public override void Undo()
        {
            if (m_TransOperation != null)
                m_TransOperation.Undo();
        }
        public override void Do()
        {
            if (m_TransOperation != null)
                m_TransOperation.Do();
        }
        public override void TryDo()
        {
            base.Do();
        }
        public override void Commit()
        {
            var trans = Element.CreateOrSplitCurrentTransRange(TransitionKind.缩放, ProjectDoc.Instance.TransGraphics.CurEditFocusTime, out m_TransOperation) as ElementTransform_Scale;
            if (trans == null && m_TransOperation != null)
            {
                m_TransOperation.Do();
                trans = m_TransOperation.ResultTransform as ElementTransform_Scale;
            }
            if (trans != null)
            {
                Element.ManualScale = ScaleOld;
                var invNow = trans.Scale;
                var invNew = Offset;
                trans.Scale = new System.Drawing.SizeF(invNow.Width + invNew.Width, invNow.Height + invNew.Height);
            }
        }
        protected Transition_Element m_TransOperation = null;
    }

    public class Operation_Element_ChangeRotAngle_Trans : Operation_Element_ChangeRotAngle
    {
        public Operation_Element_ChangeRotAngle_Trans(ElementInfo e, float angleOld, float angleNew)
            : base(e, angleOld, angleNew)
        {
        }
        public override void Undo()
        {
            if (m_TransOperation != null)
                m_TransOperation.Undo();
        }
        public override void Do()
        {
            if (m_TransOperation != null)
                m_TransOperation.Do();
        }
        public override void TryDo()
        {
            Element.EnableTransitionBlending(TransitionKind.旋转, false);
            base.Do();
        }
        public override void Commit()
        {
            var trans = Element.CreateOrSplitCurrentTransRange(TransitionKind.旋转, ProjectDoc.Instance.TransGraphics.CurEditFocusTime, out m_TransOperation) as ElementTransform_Rotation;
            if (trans == null && m_TransOperation != null)
            {
                m_TransOperation.Do();
                trans = m_TransOperation.ResultTransform as ElementTransform_Rotation;
            }
            if (trans != null)
            {
                Element.RotateAngle = RotateAngleOld;
                trans.Angle = trans.Angle + Offset;
            }
            Element.EnableTransitionBlending(TransitionKind.旋转, true);
        }
        protected Transition_Element m_TransOperation = null;
    }

    public class Operation_Element_ChangeAlpha_Trans : Operation_Element_ChangeAlpha
    {
        public Operation_Element_ChangeAlpha_Trans(ElementInfo e, float alphaOld, float alphaNew)
            : base(e, alphaOld, alphaNew)
        {
        }
        public override void Undo()
        {
            if (m_TransOperation != null)
                m_TransOperation.Undo();
        }
        public override void Do()
        {
            if (m_TransOperation != null)
                m_TransOperation.Do();
        }
        public override void TryDo()
        {
            Element.EnableTransitionBlending(TransitionKind.半透, false);
            base.Do();
        }
        public override void Commit()
        {
            var trans = Element.CreateOrSplitCurrentTransRange(TransitionKind.半透, ProjectDoc.Instance.TransGraphics.CurEditFocusTime, out m_TransOperation) as ElementTransform_Alpha;
            if (trans == null && m_TransOperation != null)
            {
                m_TransOperation.Do();
                trans = m_TransOperation.ResultTransform as ElementTransform_Alpha;
            }
            if (trans != null)
            {
                Element.Alpha = AlphaOld;
                trans.Alpha = trans.Alpha + Offset;
            }
            Element.EnableTransitionBlending(TransitionKind.半透, true);
        }
        protected Transition_Element m_TransOperation = null;
    }
}
