using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using FireTerminator.Common.Transitions;

namespace FireTerminator.Common.Elements
{
    public class ElementGroup
    {
        public ElementGroup(ElementGroupCollector collector, ElementInfo firstElem)
        {
            ParentCollector = collector;
            AddElement(firstElem);
            ParentCollector.AddGroup(this);
        }

        public Dictionary<Guid, ElementInfo> Elements = new Dictionary<Guid, ElementInfo>();
        private Dictionary<Guid, PointF> m_OffsetsToDraggingElement = new Dictionary<Guid, PointF>();

        public ElementGroupCollector ParentCollector
        {
            get;
            private set;
        }
        public RectangleF BoundRect
        {
            get
            {
                RectangleF rcRst = new RectangleF(0, 0, 0, 0);
                foreach (var ei in Elements.Values)
                {
                    var rc = ei.BoundRect;
                    if (rcRst.IsEmpty)
                        rcRst = rc;
                    else
                        rcRst = RectangleF.Union(rcRst, rc);
                }
                return rcRst;
            }
        }
        private ElementInfo m_DraggingElement = null;
        public ElementInfo DraggingElement
        {
            get { return m_DraggingElement; }
            set
            {
                if (m_DraggingElement != value)
                {
                    if (value != null && !HasElement(value.GUID))
                        return;
                    m_DraggingElement = value;
                    m_OffsetsToDraggingElement.Clear();
                    if (m_DraggingElement != null)
                    {
                        var p2 = m_DraggingElement.LocationRate;
                        foreach (var e in Elements.Values)
                        {
                            if (e == m_DraggingElement)
                                m_OffsetsToDraggingElement[e.GUID] = new PointF(0, 0);
                            else
                            {
                                var p1 = e.LocationRate;
                                m_OffsetsToDraggingElement[e.GUID] = new PointF(p1.X - p2.X, p1.Y - p2.Y);
                            }
                        }
                    }
                }
            }
        }
        private ElementInfo m_LeadingElement = null;
        public ElementInfo LeadingElement
        {
            get { return m_LeadingElement; }
            set
            {
                if (m_LeadingElement != value)
                {
                    if (value != null && !HasElement(value.GUID))
                        return;
                    m_LeadingElement = value;
                }
            }
        }
        public int ElementsCount
        {
            get { return Elements.Count; }
        }

        public bool AddElement(ElementInfo info)
        {
            if (info == null || info.ParentElementGroup != null)
                return false;
            if (info.ParentViewport != ParentCollector.ParentViewport)
                return false;
            Elements[info.GUID] = info;
            return true;
        }
        public void RemoveElement(ElementInfo info)
        {
            Elements.Remove(info.GUID);
        }
        public void RemoveElement(Guid guid)
        {
            Elements.Remove(guid);
        }
        public ElementInfo GetElement(Guid guid)
        {
            ElementInfo ei;
            Elements.TryGetValue(guid, out ei);
            return ei;
        }
        public bool HasElement(Guid guid)
        {
            return GetElement(guid) != null;
        }
        public void Dismiss()
        {
            ParentCollector.RemoveGroup(this);
        }
        public void Move(ElementInfo leadingElm, Point ptNew)
        {
            DraggingElement = leadingElm;
            var pr = ParentCollector.ParentViewport.GetLocationRate(true, ptNew, false);
            foreach (var e in Elements.Values)
            {
                var pt = new PointF(pr.X + m_OffsetsToDraggingElement[e.GUID].X, pr.Y + m_OffsetsToDraggingElement[e.GUID].Y);
                pt = ParentCollector.ParentViewport.GetRateLocation(true, pt);
                e.Location = new Point((int)pt.X, (int)pt.Y);
            }
        }
        public XmlElement GenerateXmlElement(XmlDocument doc)
        {
            XmlElement node = doc.CreateElement("Group");
            foreach (var e in Elements.Values)
            {
                XmlElement enode = doc.CreateElement("Element");
                enode.SetAttribute("Guid", e.GUID.ToString());
                node.AppendChild(enode);
            }
            if (m_LeadingElement != null)
                node.SetAttribute("LeadingElement", m_LeadingElement.GUID.ToString());
            return node;
        }
        public void LoadFromXmlElement(XmlElement node)
        {
            Elements.Clear();
            foreach (XmlElement enode in node.GetElementsByTagName("Element"))
            {
                var guid = enode.GetAttribute("Guid");
                var g = new Guid(guid);
                AddElement(ParentCollector.ParentViewport.GetElementInfo(g));
            }
            var leadGuid = node.GetAttribute("LeadingElement");
            if (!String.IsNullOrEmpty(leadGuid))
            {
                var g = new Guid(leadGuid);
                LeadingElement = GetElement(g);
            }
        }
        public void UpdateLeadingChildElements()
        {
            ElementTransitionInfo trans = null;
            if (m_LeadingElement != null)
                trans = m_LeadingElement.AnimateTrans;
            foreach (var e in Elements.Values)
            {
                if (e != m_LeadingElement)
                {
                    e.ExtraAnimateTrans = trans;
                }
            }
        }
    }

    public class ElementGroupCollector
    {
        public ElementGroupCollector(ViewportInfo vi)
        {
            ParentViewport = vi;
        }
        public void AddGroup(ElementGroup grp)
        {
            if (grp != null && !Groups.Contains(grp))
                Groups.Add(grp);
        }
        public void RemoveGroup(ElementGroup grp)
        {
            Groups.Remove(grp);
        }
        public ElementGroup FindElementGroup(ElementInfo elm)
        {
            foreach (var grp in Groups)
            {
                if (grp.HasElement(elm.GUID))
                    return grp;
            }
            return null;
        }
        public XmlElement GenerateXmlElement(XmlDocument doc)
        {
            XmlElement node = doc.CreateElement("Groups");
            foreach (var grp in Groups)
            {
                if (grp.ElementsCount > 1)
                    node.AppendChild(grp.GenerateXmlElement(doc));
            }
            return node;
        }
        public void LoadFromParentXmlElement(XmlElement root)
        {
            Groups.Clear();
            var lst = root.GetElementsByTagName("Groups");
            if (lst.Count > 0)
            {
                foreach (XmlElement node in ((XmlElement)lst[0]).GetElementsByTagName("Group"))
                {
                    var grp = new ElementGroup(this, null);
                    grp.LoadFromXmlElement(node);
                }
            }
        }

        public ViewportInfo ParentViewport
        {
            get;
            private set;
        }
        public List<ElementGroup> Groups = new List<ElementGroup>();
    }
}
