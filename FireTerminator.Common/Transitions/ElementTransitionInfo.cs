using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Runtime.Serialization;

namespace FireTerminator.Common.Transitions
{
    [DataContract]
    public class ElementTransitionInfo
    {
        public ElementTransitionInfo()
        {
        }
        public ElementTransitionInfo(PointF posRate, SizeF sizeRate)
        {
            Xr = posRate.X;
            Yr = posRate.Y;
            Wr = sizeRate.Width;
            Hr = sizeRate.Height;
        }
        public void Copy(ElementTransitionInfo info)
        {
            IsHidden = info.IsHidden;
            Xr = info.Xr;
            Yr = info.Yr;
            Wr = info.Wr;
            Hr = info.Hr;
            Angle = info.Angle;
            Alpha = info.Alpha;
        }
        public void Reset()
        {
            IsHidden = false;
            Xr = 0;
            Yr = 0;
            Wr = 1;
            Hr = 1;
            Angle = 0;
            Alpha = 0;
        }
        [DataMember]
        public bool IsHidden = false;
        [DataMember]
        public float Xr = 0;
        [DataMember]
        public float Yr = 0;
        [DataMember]
        public float Wr = 1;
        [DataMember]
        public float Hr = 1;
        [DataMember]
        public float Angle = 0;
        [DataMember]
        public float Alpha = 0;

        public System.Drawing.PointF RateLocation
        {
            get { return new System.Drawing.PointF(Xr, Yr); }
            set { Xr = value.X; Yr = value.Y; }
        }
        public System.Drawing.SizeF RateSize
        {
            get { return new System.Drawing.SizeF(Wr, Hr); }
            set { Wr = value.Width; Hr = value.Height; }
        }
        public static ElementTransitionInfo operator +(ElementTransitionInfo ti1, ElementTransitionInfo ti2)
        {
            ElementTransitionInfo ti = new ElementTransitionInfo();
            ti.IsHidden = ti2.IsHidden;
            ti.Xr = ti1.Xr + ti2.Xr;
            ti.Yr = ti1.Yr + ti2.Yr;
            ti.Wr = ti1.Wr * ti2.Wr;
            ti.Hr = ti1.Hr * ti2.Hr;
            ti.Alpha = ti1.Alpha + ti2.Alpha;
            CommonMethods.ClampValue(ref ti.Alpha, 0, 1);
            ti.Angle = ti1.Angle + ti2.Angle;
            CommonMethods.ClampValue(ref ti.Alpha, 0, 360);
            return ti;
        }
        public void SaveToXmlElement(XmlElement node)
        {
            if (IsHidden)
                node.SetAttribute("IsHidden", IsHidden.ToString());
            if (Xr != 0)
                node.SetAttribute("Xr", Xr.ToString());
            if (Yr != 0)
                node.SetAttribute("Yr", Yr.ToString());
            if (Wr != 0)
                node.SetAttribute("Wr", Wr.ToString());
            if (Hr != 0)
                node.SetAttribute("Hr", Hr.ToString());
            if (Angle != 0)
                node.SetAttribute("Angle", Angle.ToString());
            if (Alpha != 1)
                node.SetAttribute("Alpha", Alpha.ToString());
        }
        public void LoadFromXmlElement(XmlElement node)
        {
            var value = node.GetAttribute("IsHidden");
            if (String.IsNullOrEmpty(value))
                IsHidden = false;
            else
                IsHidden = Convert.ToBoolean(value);
            value = node.GetAttribute("Xr");
            if (String.IsNullOrEmpty(value))
                Xr = 0;
            else
                Xr = Convert.ToSingle(value);
            value = node.GetAttribute("Yr");
            if (String.IsNullOrEmpty(value))
                Yr = 0;
            else
                Yr = Convert.ToSingle(value);
            value = node.GetAttribute("Wr");
            if (String.IsNullOrEmpty(value))
                Wr = 0;
            else
                Wr = Convert.ToSingle(value);
            value = node.GetAttribute("Hr");
            if (String.IsNullOrEmpty(value))
                Hr = 0;
            else
                Hr = Convert.ToSingle(value);
            value = node.GetAttribute("Angle");
            if (String.IsNullOrEmpty(value))
                Angle = 0;
            else
                Angle = Convert.ToSingle(value);
            value = node.GetAttribute("Alpha");
            if (String.IsNullOrEmpty(value))
                Alpha = 1;
            else
                Alpha = Convert.ToSingle(value);
        }
    }
}
