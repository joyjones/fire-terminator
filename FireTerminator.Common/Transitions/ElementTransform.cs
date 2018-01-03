using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using System.Xml;
using System.ComponentModel;
using FireTerminator.Common.Elements;

namespace FireTerminator.Common.Transitions
{
    public abstract class ElementTransform
    {
        static ElementTransform()
        {
            TransTypes[TransitionKind.Unknown] = null;
            TransTypes[TransitionKind.隐藏] = typeof(ElementTransform_Hidden);
            TransTypes[TransitionKind.位移] = typeof(ElementTransform_Move);
            TransTypes[TransitionKind.缩放] = typeof(ElementTransform_Scale);
            TransTypes[TransitionKind.旋转] = typeof(ElementTransform_Rotation);
            TransTypes[TransitionKind.半透] = typeof(ElementTransform_Alpha);
        }
        public ElementTransform(ElementInfo elm)
        {
            ParentElement = elm;
            TimeBegin = 0;
            TimeLength = 0;
            PrevTrans = null;
            NextTrans = null;
        }
        public ElementTransform(ElementInfo elm, float time, float length)
            : this(elm)
        {
            if (time < 0)
                time = 0;
            TimeBegin = time;
            TimeLength = length;
        }
        public enum TransPercentResult
        {
            NotReached,
            Transforming,
            HasPassed
        }
        [Browsable(false)]
        public ElementInfo ParentElement
        {
            get;
            set;
        }
        [Category("动画"), DisplayName("类型")]
        public abstract TransitionKind Kind
        {
            get;
        }
        protected float m_TimeBegin = 0;
        [Category("动画"), DisplayName("起始时间")]
        public float TimeBegin
        {
            get { return m_TimeBegin; }
            set
            {
                if (value < 0)
                    value = 0;
                m_TimeBegin = value;
            }
        }
        protected float m_TimeLength = 0;
        [Category("动画"), DisplayName("持续时间")]
        public float TimeLength
        {
            get { return m_TimeLength; }
            set
            {
                if (value < 0.1F)
                    value = 0.1F;
                m_TimeLength = value;
            }
        }
        [Category("动画"), DisplayName("终止时间")]
        public float TimeEnd
        {
            get { return TimeBegin + TimeLength; }
            set { TimeLength = value - TimeBegin; }
        }
        public ElementTransform PrevTrans
        {
            get;
            set;
        }
        public ElementTransform NextTrans
        {
            get;
            set;
        }
        public virtual bool CopyFrom(ElementTransform trans)
        {
            if (Kind != trans.Kind)
                return false;
            //ParentElement = trans.ParentElement;
            //TimeBegin = trans.TimeBegin;
            TimeLength = trans.TimeLength;
            //PrevTrans = null;
            //NextTrans = null;
            return true;
        }
        public abstract TransPercentResult Update(float time);
        public float GetTimePercent(float time, out TransPercentResult result)
        {
            if (TimeLength == 0 || time < TimeBegin)
            {
                result = TransPercentResult.NotReached;
                return 0;
            }
            if (time > TimeBegin + TimeLength)
            {
                result = TransPercentResult.HasPassed;
                return 1;
            }
            result = TransPercentResult.Transforming;
            return (time - TimeBegin) / TimeLength;
        }
        public bool ContainTime(float time)
        {
            return TimeBegin <= time && TimeEnd >= time;
        }
        public virtual void Merge(ElementTransform trans)
        {
            TimeBegin = trans.TimeBegin;
            TimeLength += trans.TimeLength;
        }
        public virtual bool Split(float time, out ElementTransform trans)
        {
            trans = null;
            if (!ContainTime(time))
                return false;
            trans = System.Activator.CreateInstance(TransTypes[Kind], ParentElement) as ElementTransform;
            trans.PrevTrans = PrevTrans;
            trans.NextTrans = this;
            trans.TimeBegin = TimeBegin;
            trans.TimeLength = time - TimeBegin;
            TimeLength = TimeEnd - time;
            TimeBegin = time;
            ParentElement.AddTransition(trans);
            return true;
        }
        public virtual XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = doc.CreateElement("Transition");
            node.SetAttribute("Kind", Kind.ToString());
            node.SetAttribute("TimeBegin", TimeBegin.ToString());
            node.SetAttribute("TimeLength", TimeLength.ToString());
            return node;
        }
        public virtual void LoadFromXmlElement(XmlElement node)
        {
            TimeBegin = Single.Parse(node.GetAttribute("TimeBegin"));
            TimeLength = Single.Parse(node.GetAttribute("TimeLength"));
        }
        public static List<ElementTransform> LoadXml(ElementInfo elm, XmlElement root)
        {
            List<ElementTransform> rst = new List<ElementTransform>();
            foreach (XmlElement node in root.GetElementsByTagName("Transition"))
            {
                TransitionKind tk = (TransitionKind)Enum.Parse(typeof(TransitionKind), node.GetAttribute("Kind"));
                if (TransTypes[tk] != null)
                {
                    var trans = System.Activator.CreateInstance(TransTypes[tk], elm) as ElementTransform;
                    trans.LoadFromXmlElement(node);
                    rst.Add(trans);
                }
            }
            return rst;
        }
        public static Dictionary<TransitionKind, Type> TransTypes = new Dictionary<TransitionKind, Type>();
    }
}
