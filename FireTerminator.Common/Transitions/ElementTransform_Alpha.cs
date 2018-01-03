using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FireTerminator.Common.Elements;
using System.Xml;

namespace FireTerminator.Common.Transitions
{
    public class ElementTransform_Alpha : ElementTransform
    {
        public ElementTransform_Alpha(ElementInfo elm)
            : base(elm)
        {
        }
        public ElementTransform_Alpha(ElementInfo elm, float time, float length)
            : base(elm, time, length)
        {
        }
        public override TransitionKind Kind
        {
            get { return TransitionKind.半透; }
        }
        protected float m_Alpha = 0;
        [Category("半透"), DisplayName("不透明度增值")]
        public float Alpha
        {
            get { return m_Alpha; }
            set
            {
                if (NextTrans == null)
                    m_Alpha = value;
                else
                {
                    var trans = NextTrans as ElementTransform_Alpha;
                    var fSum = m_Alpha + trans.Alpha;
                    m_Alpha = value;
                    trans.Alpha = fSum - m_Alpha;
                }
            }
        }
        public override bool CopyFrom(ElementTransform trans)
        {
            if (!base.CopyFrom(trans))
                return false;
            var t = trans as ElementTransform_Alpha;
            Alpha = t.Alpha;
            return true;
        }
        public override TransPercentResult Update(float time)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (rst == TransPercentResult.Transforming)
                ParentElement.AnimateTrans.Alpha += percent * Alpha;
            else if (rst == TransPercentResult.HasPassed)
                ParentElement.AnimateTrans.Alpha += Alpha;
            return rst;
        }
        public override void Merge(ElementTransform trans)
        {
            base.Merge(trans);
            var t = trans as ElementTransform_Alpha;
            if (t != null)
            {
                m_Alpha += t.Alpha;
            }
        }
        public override bool Split(float time, out ElementTransform trans)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (!base.Split(time, out trans))
                return false;
            var etrans = trans as ElementTransform_Alpha;
            etrans.Alpha = (int)(percent * Alpha);
            return true;
        }
        public override XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = base.GenerateXmlElement(doc);
            node.SetAttribute("Alpha", Alpha.ToString());
            return node;
        }
        public override void LoadFromXmlElement(XmlElement node)
        {
            base.LoadFromXmlElement(node);
            Alpha = Single.Parse(node.GetAttribute("Alpha"));
        }
    }
}
