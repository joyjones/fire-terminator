using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FireTerminator.Common.Elements;
using System.Xml;

namespace FireTerminator.Common.Transitions
{
    public class ElementTransform_Rotation : ElementTransform
    {
        public ElementTransform_Rotation(ElementInfo elm)
            : base(elm)
        {
        }
        public ElementTransform_Rotation(ElementInfo elm, float time, float length)
            : base(elm, time, length)
        {
        }
        public override TransitionKind Kind
        {
            get { return TransitionKind.旋转; }
        }
        protected float m_Angle = 0;
        [Category("旋转"), DisplayName("角度增值")]
        public float Angle
        {
            get { return m_Angle; }
            set
            {
                if (NextTrans == null)
                    m_Angle = value;
                else
                {
                    var trans = NextTrans as ElementTransform_Rotation;
                    var fSum = m_Angle + trans.Angle;
                    m_Angle = value;
                    trans.Angle = fSum - m_Angle;
                }
            }
        }
        public override bool CopyFrom(ElementTransform trans)
        {
            if (!base.CopyFrom(trans))
                return false;
            var t = trans as ElementTransform_Rotation;
            Angle = t.Angle;
            return true;
        }
        public override TransPercentResult Update(float time)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (rst == TransPercentResult.Transforming)
                ParentElement.AnimateTrans.Angle += percent * Angle;
            else if (rst == TransPercentResult.HasPassed)
                ParentElement.AnimateTrans.Angle += Angle;
            return rst;
        }
        public override void Merge(ElementTransform trans)
        {
            base.Merge(trans);
            var t = trans as ElementTransform_Rotation;
            if (t != null)
            {
                m_Angle += t.Angle;
            }
        }
        public override bool Split(float time, out ElementTransform trans)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (!base.Split(time, out trans))
                return false;
            var etrans = trans as ElementTransform_Rotation;
            etrans.Angle = (int)(percent * Angle);
            return true;
        }
        public override XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = base.GenerateXmlElement(doc);
            node.SetAttribute("Angle", Angle.ToString());
            return node;
        }
        public override void LoadFromXmlElement(XmlElement node)
        {
            base.LoadFromXmlElement(node);
            Angle = Single.Parse(node.GetAttribute("Angle"));
        }
    }
}
