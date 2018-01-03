using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using FireTerminator.Common.Elements;
using System.Xml;

namespace FireTerminator.Common.Transitions
{
    public class ElementTransform_Scale : ElementTransform
    {
        public ElementTransform_Scale(ElementInfo elm)
            : base(elm)
        {
        }
        public ElementTransform_Scale(ElementInfo elm, float time, float length)
            : base(elm, time, length)
        {
        }
        public override TransitionKind Kind
        {
            get { return TransitionKind.缩放; }
        }
        private SizeF m_Scale = new SizeF(0, 0);
        [Category("缩放"), DisplayName("增加缩放比")]
        public SizeF Scale
        {
            get { return m_Scale; }
            set
            {
                if (NextTrans == null)
                    m_Scale = value;
                else
                {
                    var trans = NextTrans as ElementTransform_Scale;
                    var szSum = m_Scale + trans.Scale;
                    m_Scale = value;
                    trans.Scale = szSum - m_Scale;
                }
            }
        }
        public override bool CopyFrom(ElementTransform trans)
        {
            if (!base.CopyFrom(trans))
                return false;
            var t = trans as ElementTransform_Scale;
            Scale = t.Scale;
            return true;
        }
        public override TransPercentResult Update(float time)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (rst == TransPercentResult.Transforming)
            {
                ParentElement.AnimateTrans.Wr *= (1 + percent * Scale.Width);
                ParentElement.AnimateTrans.Hr *= (1 + percent * Scale.Height);
            }
            else if (rst == TransPercentResult.HasPassed)
            {
                ParentElement.AnimateTrans.Wr *= 1 + Scale.Width;
                ParentElement.AnimateTrans.Hr *= 1 + Scale.Height;
            }
            return rst;
        }
        public override void Merge(ElementTransform trans)
        {
            base.Merge(trans);
            var t = trans as ElementTransform_Scale;
            if (t != null)
            {
                m_Scale += t.Scale;
            }
        }
        public override bool Split(float time, out ElementTransform trans)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (!base.Split(time, out trans))
                return false;
            var etrans = trans as ElementTransform_Scale;
            int dw = (int)(percent * Scale.Width);
            int dh = (int)(percent * Scale.Height);
            etrans.Scale = new System.Drawing.SizeF(dw, dh);
            return true;
        }
        public override XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = base.GenerateXmlElement(doc);
            node.SetAttribute("Scale", String.Format("{0},{1}", Scale.Width, Scale.Height));
            return node;
        }
        public override void LoadFromXmlElement(XmlElement node)
        {
            base.LoadFromXmlElement(node);
            CommonMethods.GetFormatFloat2(node.GetAttribute("Scale"), ref m_Scale);
        }
    }
}
