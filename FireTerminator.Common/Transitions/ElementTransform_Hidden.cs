using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FireTerminator.Common.Elements;
using System.Xml;

namespace FireTerminator.Common.Transitions
{
    public class ElementTransform_Hidden : ElementTransform
    {
        public ElementTransform_Hidden(ElementInfo elm)
            : base(elm)
        {
            IsHidden = true;
        }
        public ElementTransform_Hidden(ElementInfo elm, float time, float length)
            : base(elm, time, length)
        {
            IsHidden = true;
        }
        public override TransitionKind Kind
        {
            get { return TransitionKind.隐藏; }
        }
        [Category("隐藏"), DisplayName("是否隐藏")]
        public bool IsHidden
        {
            get;
            set;
        }
        public override bool CopyFrom(ElementTransform trans)
        {
            if (!base.CopyFrom(trans))
                return false;
            var t = trans as ElementTransform_Hidden;
            IsHidden = t.IsHidden;
            return true;
        }
        public override TransPercentResult Update(float time)
        {
            TransPercentResult rst;
            float percent = GetTimePercent(time, out rst);
            if (rst == TransPercentResult.Transforming)
                ParentElement.AnimateTrans.IsHidden = IsHidden;
            return rst;
        }
        public override bool Split(float time, out ElementTransform trans)
        {
            if (!base.Split(time, out trans))
                return false;
            return true;
        }
        public override XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = base.GenerateXmlElement(doc);
            node.SetAttribute("IsHidden", IsHidden.ToString());
            return node;
        }
        public override void LoadFromXmlElement(XmlElement node)
        {
            base.LoadFromXmlElement(node);
            IsHidden = Boolean.Parse(node.GetAttribute("IsHidden"));
        }
    }
}
