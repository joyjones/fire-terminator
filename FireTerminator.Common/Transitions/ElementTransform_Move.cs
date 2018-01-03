using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;
using FireTerminator.Common.Elements;

namespace FireTerminator.Common.Transitions
{
    public class ElementTransform_Move : ElementTransform
    {
        public ElementTransform_Move(ElementInfo elm)
            : base(elm)
        {
        }
        public ElementTransform_Move(ElementInfo elm, float time, float length)
            : base(elm, time, length)
        {
        }
        public override TransitionKind Kind
        {
            get { return TransitionKind.位移; }
        }
        private System.Drawing.PointF m_OffsetRate = new System.Drawing.PointF(0, 0);
        [Category("位移"), DisplayName("偏移量")]
        public System.Drawing.Point Offset
        {
            get
            {
                var pos = new System.Drawing.Point();
                var size = ParentElement.ParentViewport.BackgroundViewSize;
                pos.X = (int)(m_OffsetRate.X * size.Width);
                pos.Y = (int)(m_OffsetRate.Y * size.Height);
                return pos;
            }
            set
            {
                var size = ParentElement.ParentViewport.BackgroundViewSize;
                if (NextTrans == null)
                {
                    m_OffsetRate.X = value.X / (float)size.Width;
                    m_OffsetRate.Y = value.Y / (float)size.Height;
                }
                else
                {
                    var trans = NextTrans as ElementTransform_Move;
                    var offset1 = Offset;
                    var offset2 = trans.Offset;
                    var ptSum = new System.Drawing.Point(offset1.X + offset2.X, offset1.Y + offset2.Y);
                    m_OffsetRate.X = value.X / (float)size.Width;
                    m_OffsetRate.Y = value.Y / (float)size.Height;
                    offset1 = Offset;
                    offset2.X = ptSum.X - offset1.X;
                    offset2.Y = ptSum.Y - offset1.Y;
                    trans.Offset = offset2;
                }
            }
        }
        public override bool CopyFrom(ElementTransform trans)
        {
            if (!base.CopyFrom(trans))
                return false;
            var t = trans as ElementTransform_Move;
            Offset = t.Offset;
            return true;
        }
        public override TransPercentResult Update(float time)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (rst == TransPercentResult.Transforming)
            {
                ParentElement.AnimateTrans.Xr += percent * m_OffsetRate.X;
                ParentElement.AnimateTrans.Yr += percent * m_OffsetRate.Y;
            }
            else if (rst == TransPercentResult.HasPassed)
            {
                ParentElement.AnimateTrans.Xr += m_OffsetRate.X;
                ParentElement.AnimateTrans.Yr += m_OffsetRate.Y;
            }
            return rst;
        }
        public override void Merge(ElementTransform trans)
        {
            base.Merge(trans);
            var t = trans as ElementTransform_Move;
            if (t != null)
            {
                var offset1 = Offset;
                var offset2 = t.Offset;
                offset1.X += offset2.X;
                offset1.Y += offset2.Y;
                Offset = offset1;
            }
        }
        public override bool Split(float time, out ElementTransform trans)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (!base.Split(time, out trans))
                return false;
            var etrans = trans as ElementTransform_Move;
            int dx = (int)(percent * Offset.X);
            int dy = (int)(percent * Offset.Y);
            etrans.Offset = new System.Drawing.Point(dx, dy);
            return true;
        }
        public override XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = base.GenerateXmlElement(doc);
            node.SetAttribute("OffsetRate", String.Format("{0},{1}", m_OffsetRate.X, m_OffsetRate.Y));
            return node;
        }
        public override void LoadFromXmlElement(XmlElement node)
        {
            base.LoadFromXmlElement(node);
            CommonMethods.GetFormatFloat2(node.GetAttribute("OffsetRate"), ref m_OffsetRate);
        }
    }
}
