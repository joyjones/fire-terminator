using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FireTerminator.Common.Elements;
using System.Reflection;

namespace FireTerminator.Common.Operations
{
    public class Operation_Element : Operation
    {
        public Operation_Element(ElementInfo e)
        {
            Element = e;
        }
        public override void Undo() { }
        public override void Do() { }
        public override void Commit() { }
        public override void Merge(Operation opt) { }
        public ElementInfo Element
        {
            get;
            private set;
        }
        public virtual bool DonotMakeUserDirty
        {
            get { return false; }
        }
    }

    public class Operation_Element_Create : Operation_Element
    {
        public Operation_Element_Create(ElementInfo e, ViewportInfo viewport)
            : base(e)
        {
            Viewport = viewport;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            Viewport.RemoveElement(Element);
        }
        public override void Do()
        {
            Viewport.AddElement(Element);
        }
        public ViewportInfo Viewport
        {
            get;
            protected set;
        }
    }

    public class Operation_Element_Duplicate : Operation_Element
    {
        public Operation_Element_Duplicate(ElementInfo e, ViewportInfo viewport)
            : base(e)
        {
            Viewport = viewport;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            Viewport.RemoveElement(DuplicatedElementInfo);
        }
        public override void Do()
        {
            if (DuplicatedElementInfo == null)
                DuplicatedElementInfo = Viewport.DuplicateElement(Element, Element.Location);
            else
                Viewport.AddElement(DuplicatedElementInfo);
        }
        public ViewportInfo Viewport
        {
            get;
            protected set;
        }
        public ElementInfo DuplicatedElementInfo
        {
            get;
            protected set;
        }
    }

    public class Operation_Element_ChangeViewport : Operation_Element
    {
        public Operation_Element_ChangeViewport(ElementInfo e, ViewportInfo viewport)
            : base(e)
        {
            ViewportOld = e.ParentViewport;
            ViewportNew = viewport;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            if (ViewportOld != ViewportNew)
            {
                if (ViewportNew != null)
                    ViewportNew.RemoveElement(Element);
                if (ViewportOld != null)
                    ViewportOld.AddElement(Element);
            }
        }
        public override void Do()
        {
            if (ViewportOld != ViewportNew)
            {
                if (ViewportOld != null)
                    ViewportOld.RemoveElement(Element);
                if (ViewportNew != null)
                    ViewportNew.AddElement(Element);
            }
        }
        public ViewportInfo ViewportNew
        {
            get;
            protected set;
        }
        public ViewportInfo ViewportOld
        {
            get;
            protected set;
        }
    }

    public class Operation_Element_Delete : Operation_Element
    {
        public Operation_Element_Delete(ElementInfo e)
            : base(e)
        {
            Viewport = e.ParentViewport;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            Viewport.AddElement(Element);
        }
        public override void Do()
        {
            bool bSelecting = ProjectDoc.Instance.SelectedElementInfo == Element;
            Viewport.RemoveElement(Element);
            if (bSelecting)
                ProjectDoc.Instance.SelectedElementInfo = null;
        }
        public ViewportInfo Viewport
        {
            get;
            protected set;
        }
    }

    public class Operation_Element_Drift : Operation_Element
    {
        public Operation_Element_Drift(ElementInfo e, int depthAdd)
            : base(e)
        {
            DepthLevelOld = e.DepthLevel;
            DepthLevelNew = DepthLevelOld + depthAdd;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            Element.DepthLevel = DepthLevelOld;
        }
        public override void Do()
        {
            Element.DepthLevel = DepthLevelNew;
        }
        public int DepthLevelOld
        {
            get;
            protected set;
        }
        public int DepthLevelNew
        {
            get;
            protected set;
        }
    }

    public class Operation_Element_Flip : Operation_Element
    {
        public Operation_Element_Flip(ElementInfo e, FlippingState stateOld, FlippingState stateNew)
            : base(e)
        {
            StateOld = stateOld;
            StateNew = stateNew;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            Element.CurFlippingState = StateOld;
        }
        public override void Do()
        {
            Element.CurFlippingState = StateNew;
        }
        public FlippingState StateOld
        {
            get;
            protected set;
        }
        public FlippingState StateNew
        {
            get;
            protected set;
        }
    }

    public class Operation_Element_ChangeLocation : Operation_Element
    {
        public Operation_Element_ChangeLocation(ElementInfo e, Point pos)
            : base(e)
        {
            LocationOld = e.Location;
            LocationNew = pos;
        }
        public Operation_Element_ChangeLocation(ElementInfo e, Point pos, bool drive)
            : this(e, pos)
        {
            Driving = drive;
        }
        public override void Undo()
        {
            var grp = Element.ParentElementGroup;
            if (grp != null)
                grp.Move(Element, LocationOld);
            else
                Element.Location = LocationOld;
        }
        public override void Do()
        {
            if (Driving)
                Element.TargetLocation = LocationNew;
            else
            {
                var grp = Element.ParentElementGroup;
                if (grp != null)
                    grp.Move(Element, LocationNew);
                else
                    Element.Location = LocationNew;
            }
        }
        public override void Merge(Operation opt)
        {
            var o = opt as Operation_Element_ChangeLocation;
            LocationNew = o.LocationNew;
        }
        public Point LocationOld
        {
            get;
            protected set;
        }
        public Point LocationNew
        {
            get;
            protected set;
        }
        public bool Driving
        {
            get;
            protected set;
        }
        public Point Offset
        {
            get { return new Point(LocationNew.X - LocationOld.X, LocationNew.Y - LocationOld.Y); }
        }
    }

    public class Operation_Element_ChangeScale : Operation_Element
    {
        public Operation_Element_ChangeScale(ElementInfo e, SizeF scale)
            : base(e)
        {
            ScaleOld = e.ManualScale;
            ScaleNew = scale;
        }
        public override void Undo()
        {
            Element.ManualScale = ScaleOld;
        }
        public override void Do()
        {
            Element.ManualScale = ScaleNew;
        }
        public override void Merge(Operation opt)
        {
            var o = opt as Operation_Element_ChangeScale;
            ScaleNew = o.ScaleNew;
        }
        public SizeF ScaleOld
        {
            get;
            protected set;
        }
        public SizeF ScaleNew
        {
            get;
            protected set;
        }
        public SizeF Offset
        {
            get { return new SizeF(ScaleNew.Width - ScaleOld.Width, ScaleNew.Height - ScaleOld.Height); }
        }
    }

    public class Operation_Element_ChangeProperty : Operation_Element
    {
        public Operation_Element_ChangeProperty(ElementInfo e, string propertyName, object oldValue, object newValue)
            : base(e)
        {
            PropertyName = propertyName;
            ValueOld = oldValue;
            ValueNew = newValue;

            //string[] names = PropertyName.Split('.');
            //if (Element != null && names.Length > 0)
            //{
            //    m_ProprotyObject = Element;
            //    m_PropertyInfo = Element.GetType().GetProperty(names[0]);
            //    if (names.Length > 1)
            //    {
            //        m_ProprotyObject = Element.Location;
            //        m_PropertyInfo = m_PropertyInfo.PropertyType.GetProperty(names[1]);
            //    }
            //}
        }
        public override void Undo()
        {
            var pi = Element.GetType().GetProperty(PropertyName);
            pi.SetValue(Element, ValueOld, null);
        }
        public override void Do()
        {
            var pi = Element.GetType().GetProperty(PropertyName);
            pi.SetValue(Element, ValueNew, null);
        }
        public override void Merge(Operation opt)
        {
            var o = opt as Operation_Element_ChangeProperty;
            ValueNew = o.ValueNew;
        }
        public string PropertyName
        {
            get;
            protected set;
        }
        public object ValueOld
        {
            get;
            protected set;
        }
        public object ValueNew
        {
            get;
            protected set;
        }
        //private PropertyInfo m_PropertyInfo = null;
        //private object m_ProprotyObject = null;
    }

    public class Operation_Element_ChangeAlpha : Operation_Element
    {
        public Operation_Element_ChangeAlpha(ElementInfo e, float alphaOld, float alphaNew)
            : base(e)
        {
            AlphaOld = alphaOld;
            AlphaNew = alphaNew;
        }
        public override void Undo()
        {
            Element.Alpha = AlphaOld;
        }
        public override void Do()
        {
            Element.Alpha = AlphaNew;
        }
        public override void Merge(Operation opt)
        {
            var o = opt as Operation_Element_ChangeAlpha;
            AlphaNew = o.AlphaNew;
        }
        public float AlphaOld
        {
            get;
            protected set;
        }
        public float AlphaNew
        {
            get;
            protected set;
        }
        public float Offset
        {
            get { return AlphaNew - AlphaOld; }
        }
    }

    public class Operation_Element_ChangeRotAngle : Operation_Element
    {
        public Operation_Element_ChangeRotAngle(ElementInfo e, float angleOld, float angleNew)
            : base(e)
        {
            RotateAngleOld = angleOld;
            RotateAngleNew = angleNew;
        }
        public override void Undo()
        {
            Element.RotateAngle = RotateAngleOld;
        }
        public override void Do()
        {
            Element.RotateAngle = RotateAngleNew;
        }
        public override void Merge(Operation opt)
        {
            var o = opt as Operation_Element_ChangeRotAngle;
            RotateAngleNew = o.RotateAngleNew;
        }
        public float RotateAngleOld
        {
            get;
            protected set;
        }
        public float RotateAngleNew
        {
            get;
            protected set;
        }
        public float Offset
        {
            get { return RotateAngleNew - RotateAngleOld; }
        }
    }

    public class Operation_Element_SetHotkeyAnimation : Operation_Element
    {
        public Operation_Element_SetHotkeyAnimation(ElementInfo e, System.Windows.Forms.Keys key, float time)
            : base(e)
        {
            Key = key;
            Time = time;
        }
        public override void Undo()
        {
        }
        public override void Do()
        {
        }
        public override void Merge(Operation opt)
        {
        }
        public System.Windows.Forms.Keys Key
        {
            get;
            private set;
        }
        public float Time
        {
            get;
            private set;
        }
        public override bool DonotMakeUserDirty
        {
            get { return true; }
        }
    }

    public class Operation_Element_ChangeMaskInfo : Operation_Element
    {
        public Operation_Element_ChangeMaskInfo(ElementInfo_Mask e)
            : base(e)
        {
            VecsOld = e.CornerVectors;
            VecsNew = e.CornerVectors;
        }
        public override void Undo()
        {
            ((ElementInfo_Mask)Element).SetPosition(VecsOld);
        }
        public override void Do()
        {
            ((ElementInfo_Mask)Element).SetPosition(VecsNew);
        }
        public override void Merge(Operation opt)
        {
            var o = opt as Operation_Element_ChangeMaskInfo;
            VecsNew = o.VecsNew;
        }
        public Microsoft.Xna.Framework.Vector2[] VecsOld
        {
            get;
            protected set;
        }
        public Microsoft.Xna.Framework.Vector2[] VecsNew
        {
            get;
            protected set;
        }
    }

    public class Operation_Element_ChangeWaterbagInfo : Operation_Element
    {
        public Operation_Element_ChangeWaterbagInfo(ElementInfo_Waterbag e)
            : base(e)
        {
            VecsOld = e.JointVecs;
            VecsNew = e.JointVecs;
        }
        public override void Undo()
        {
            WaterElement.SetProperties(WaterElement.BagWidth, VecsOld);
        }
        public override void Do()
        {
            WaterElement.SetProperties(WaterElement.BagWidth, VecsNew);
        }
        public override void Merge(Operation opt)
        {
            var o = opt as Operation_Element_ChangeWaterbagInfo;
            VecsNew = o.VecsNew;
        }
        public ElementInfo_Waterbag WaterElement
        {
            get { return Element as ElementInfo_Waterbag; }
        }
        public Microsoft.Xna.Framework.Vector2[] VecsOld
        {
            get;
            protected set;
        }
        public Microsoft.Xna.Framework.Vector2[] VecsNew
        {
            get;
            protected set;
        }
    }
}
