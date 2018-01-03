using System.Collections.Generic;
using System.ComponentModel;
using FireTerminator.Common.RenderResources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Linq;
using System;

namespace FireTerminator.Common.Elements
{
    public class ElementInfo_Waterbag : ElementInfo
    {
        public ElementInfo_Waterbag(ResourceInfo res, ViewportInfo vi, bool isPreview, System.Drawing.PointF pos)
            : base(res, vi, isPreview, pos)
        {
            Name = "水带";
            if (CommonTexture == null)
            {
                try
                {
                    CommonTexture = Texture2D.FromFile(ProjectDoc.Instance.HostGame.GraphicsDevice, Options.SystemResourceRootPath + "waterbag.tga");
                }
                catch { }
            }
            AddJoint(new System.Drawing.Point((int)pos.X, (int)pos.Y));
            AddJoint(new System.Drawing.Point((int)pos.X + 50, (int)pos.Y));
        }
        public ElementInfo_Waterbag(ElementInfo_Waterbag e)
            : base(e)
        {
            m_BagWidth = e.m_BagWidth;
            m_PressingShift = false;
            foreach (var j in e.m_Joints)
                m_Joints.Add(new Joint(j));
            m_Vectors = new List<VertexPositionColorTexture>(e.m_Vectors);
            m_Segments.Clear();
            foreach (var seg in e.m_Segments)
                m_Segments.Add(new Segment(this, seg));
        }
        private float m_BagWidth = 3;
        private List<Joint> m_Joints = new List<Joint>();
        private List<VertexPositionColorTexture> m_Vectors = new List<VertexPositionColorTexture>();
        private List<Segment> m_Segments = new List<Segment>();
        private bool m_PressingShift = false;
        private CubicCurve3 m_Curve = new CubicCurve3();

        [Browsable(false)]
        public ResourceInfo_Dummy ResWaterbag
        {
            get { return Resource as ResourceInfo_Dummy; }
        }
        [Browsable(false)]
        public override bool IsInnerEditingMode
        {
            get { return base.IsInnerEditingMode; }
            set { base.IsInnerEditingMode = value; }
        }
        [Browsable(false)]
        public override bool IsInnerEditable
        {
            get { return true; }
        }
        [Category("外观"), DisplayName("水带宽度"), DefaultValue(3.0F)]
        public float BagWidth
        {
            get { return m_BagWidth; }
            set { m_BagWidth = value; }
        }
        [Browsable(false)]
        public float BagWidthRate
        {
            get { return ParentViewport.GetLengthRate(true, BagWidth); }
        }
        [Browsable(false)]
        public Vector2[] JointVecs
        {
            get
            {
                return (from j in m_Joints
                        select j.RatePos).ToArray();
            }
        }
        [Browsable(false)]
        public static Texture2D CommonTexture
        {
            get;
            protected set;
        }

        [Browsable(false)]
        public override System.Drawing.RectangleF BoundRect
        {
            get
            {
                var pts = SegmentPoints;
                float x1 = Single.MaxValue, x2 = Single.MinValue, y1 = Single.MaxValue, y2 = Single.MinValue;
                foreach (var p in pts)
                {
                    var pos = new System.Drawing.PointF(p.X, p.Y);
                    if (x1 > pos.X) x1 = pos.X;
                    if (x2 < pos.X) x2 = pos.X;
                    if (y1 > pos.Y) y1 = pos.Y;
                    if (y2 < pos.Y) y2 = pos.Y;
                }
                float w = BagWidth;
                return new System.Drawing.RectangleF(x1 - w, y1 - w, x2 - x1 + w * 2, y2 - y1 + w * 2);
            }
        }
        [Browsable(false)]
        public Vector2[] SegmentPoints
        {
            get
            {
                var br = m_BlendedTransInfo.RateLocation;
                List<Vector2> rst = new List<Vector2>();
                foreach (var seg in m_Segments)
                {
                    var p = ParentViewport.GetRateLocation(true, new System.Drawing.PointF(br.X + seg.J1.RatePos.X, br.Y + seg.J1.RatePos.Y));
                    rst.Add(new Vector2(p.X, p.Y));
                }
                if (m_Segments.Count > 0)
                {
                    var v = m_Segments[m_Segments.Count - 1].J2.RatePos;
                    var p = ParentViewport.GetRateLocation(true, new System.Drawing.PointF(br.X + v.X, br.Y + v.Y));
                    rst.Add(new Vector2(p.X, p.Y));
                }
                return rst.ToArray();
            }
        }
        private static float[,] UVIndices = new float[6,2]
        {
            {0, 0},
            {0, 1},
            {1, 1},
            {0, 0},
            {1, 1},
            {1, 0},
        };
        private List<Vector2[]> m_TempBufferVecs = new List<Vector2[]>();
        //private List<VertexPositionColor> m_TempBufferCurve = new List<VertexPositionColor>();
        public override void Update(float elapsedTime, ref float curViewportTime)
        {
            base.Update(elapsedTime, ref curViewportTime);

            m_Vectors.Clear();
            m_TempBufferVecs.Clear();
            var pr = m_BlendedTransInfo.RateLocation;
            foreach (var seg in m_Segments)
            {
                var vrs = seg.GetBuffer(seg != m_Segments[0]);
                for (int i = 0; i < vrs.Length; ++i)
                {
                    var v = ParentViewport.GetRateLocation(true, new System.Drawing.PointF(pr.X + vrs[i].X, pr.Y + vrs[i].Y));
                    Color clr;
                    if (!IsInnerEditingMode || i < 6)
                        clr = new Color(BlendColor.R, BlendColor.G, BlendColor.B, BlendColor.A);
                    else
                        clr = Color.Red;
                    Vector2 uv;
                    if (i < 6)
                        uv = new Vector2(UVIndices[i, 0], UVIndices[i, 1]);
                    else if (IsInnerEditingMode)
                        uv = new Vector2(0.5F, 0.5F);
                    else
                        uv = new Vector2(ProjectDoc.Instance.Option.WaterbagJointColorUV.X, ProjectDoc.Instance.Option.WaterbagJointColorUV.Y);
                    var vec = new VertexPositionColorTexture(new Vector3(v.X, v.Y, 0), clr, uv);
                    m_Vectors.Add(vec);
                }
                m_TempBufferVecs.Add(vrs);
            }

            //m_TempBufferCurve.Clear();
            //int curveSegs = 100;
            //for (int i = 0; i < curveSegs; ++i)
            //{
            //    if (m_TempBufferCurve.Count > 1)
            //        m_TempBufferCurve.Add(m_TempBufferCurve[m_TempBufferCurve.Count - 1]);
            //    var pos = m_Curve.GetPosition(i / (float)curveSegs);
            //    var posAbs = ParentViewport.GetRateLocation(true, new System.Drawing.PointF(pr.X + pos.X, pr.Y + pos.Y));
            //    m_TempBufferCurve.Add(new VertexPositionColor(new Vector3(posAbs.X, posAbs.Y, 0), Color.OrangeRed));
            //}
        }
        public override bool Draw()
        {
            if (!base.Draw())
                return false;

            if (CommonTexture != null)
            {
                UsingEffect.TextureEnabled = true;
                UsingEffect.Texture = CommonTexture;
            }
            else
            {
                UsingEffect.TextureEnabled = false;
                UsingEffect.Texture = null;
            }
            UsingEffect.Projection = Matrix.CreateOrthographicOffCenter(0, ParentViewport.ViewportPtr.Width, ParentViewport.ViewportPtr.Height, 0, 1.0f, 1000.0f);
            UsingEffect.Begin();
            foreach (EffectPass pass in UsingEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                UsingEffect.GraphicsDevice.VertexDeclaration = new VertexDeclaration(UsingEffect.GraphicsDevice, VertexPositionColorTexture.VertexElements);
                UsingEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, m_Vectors.ToArray(), 0, m_Vectors.Count / 3);
                pass.End();
            }
            UsingEffect.End();
            
            //UsingEffect.TextureEnabled = false;
            //UsingEffect.Begin();
            //foreach (EffectPass pass in UsingEffect.CurrentTechnique.Passes)
            //{
            //    pass.Begin();
            //    UsingEffect.GraphicsDevice.VertexDeclaration = new VertexDeclaration(UsingEffect.GraphicsDevice, VertexPositionColor.VertexElements);
            //    UsingEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, m_TempBufferCurve.ToArray(), 0, m_TempBufferCurve.Count / 2);
            //    pass.End();
            //}
            //UsingEffect.End();
            return true;
        }
        private int m_iFocusJointIndex = -1;
        public override BodyOperationPart GetPointBodyOprPart(System.Drawing.Point pt)
        {
            m_iFocusJointIndex = -1;
            float radius = BagWidth;
            if (radius < 5)
                radius = 5;
            System.Drawing.PointF pos = pt;
            if (!IsInnerEditingMode)
            {
                foreach (var sp in SegmentPoints)
                {
                    var p = new System.Drawing.PointF(sp.X, sp.Y);
                    double dx = p.X - pos.X;
                    double dy = p.Y - pos.Y;
                    double dist = System.Math.Sqrt(dx * dx + dy * dy);
                    if (dist <= radius * 1.5F)
                    {
                        return BodyOperationPart.Body;
                    }
                }
                return BodyOperationPart.Nothing;
            }
            if (ParentViewport.SelectedElementInfo != this && base.GetPointBodyOprPart(pt) == BodyOperationPart.Nothing)
                return BodyOperationPart.Nothing;
            int index = -1;
            foreach (var sp in SegmentPoints)
            {
                ++index;
                var p = new System.Drawing.PointF(sp.X, sp.Y);
                double dx = p.X - pos.X;
                double dy = p.Y - pos.Y;
                double dist = System.Math.Sqrt(dx * dx + dy * dy);
                if (dist <= radius * 1.5F)
                {
                    m_iFocusJointIndex = index;
                    if (m_PressingShift && m_Joints.Count > 2)
                        return BodyOperationPart.DelJoint;
                    else
                        return BodyOperationPart.Body;
                }
            }
            return BodyOperationPart.AddJoint;
        }
        public void SetPosition(BodyOperationPart part, System.Drawing.Point pPosOrBodyOffset)
        {
            switch (part)
            {
                case BodyOperationPart.Body:
                    {
                        if (m_iFocusJointIndex >= 0)
                            MoveJoint(m_iFocusJointIndex, pPosOrBodyOffset);
                    } break;
                case BodyOperationPart.AddJoint:
                    {
                        var pos = pPosOrBodyOffset;
                        //pos.X -= Location.X;
                        //pos.Y -= Location.Y;
                        AddJoint(pos);
                    } break;
                case BodyOperationPart.DelJoint:
                    {
                        if (m_iFocusJointIndex >= 0)
                            RemoveJoint(m_iFocusJointIndex);
                    } break;
            }
        }
        public void SetProperties(float width, Vector2[] vecs)
        {
            BagWidth = width;
            m_Joints.Clear();
            m_Segments.Clear();
            m_Curve.RemoveAll();
            foreach (var v in vecs)
            {
                AddJoint(v.X, v.Y);
            }
        }
        public void AddJoint(float xr, float yr)
        {
            m_Joints.Add(new Joint(xr, yr));
            if (m_Joints.Count >= 2)
            {
                var j1 = m_Joints[m_Joints.Count - 2];
                var j2 = m_Joints[m_Joints.Count - 1];
                m_Segments.Add(new Segment(this, j1, j2));
            }
            m_Curve.AppendNode(new Vector3(xr, yr, 0));
        }
        public void AddJoint(System.Drawing.Point pt)
        {
            var ptf = ParentViewport.GetLocationRate(true, new System.Drawing.PointF(pt.X, pt.Y));
            var br = m_BlendedTransInfo.RateLocation;
            AddJoint(ptf.X - br.X, ptf.Y - br.Y);
        }
        public void RemoveJoint(int index)
        {
            if (index >= 0 && index < m_Joints.Count)
            {
                var jt = m_Joints[index];
                for (int i = 0; i < m_Segments.Count; ++i)
                {
                    if (m_Segments[i].J1 == jt)
                    {
                        if (i > 0)
                            m_Segments[i - 1].J2 = m_Segments[i].J2;
                        m_Segments.RemoveAt(i--);
                    }
                    else if (i == m_Segments.Count - 1 && m_Segments[i].J2 == jt)
                    {
                        m_Segments.RemoveAt(i);
                    }
                }
                m_Joints.RemoveAt(index);
                m_Curve.DeleteNode(index);
            }
        }
        public void MoveJoint(int index, System.Drawing.Point offset)
        {
            var ptf = ParentViewport.GetLocationRate(true, new System.Drawing.PointF(offset.X, offset.Y), true);
            if (index >= 0 && index < m_Joints.Count)
            {
                var rp = m_Joints[index].RatePos;
                rp.X += ptf.X;
                rp.Y += ptf.Y;
                m_Joints[index].RatePos = rp;
                m_Curve.SetPosition(index, new Vector3(rp.X, rp.Y, 0));
            }
        }
        public override void OnViewportSizeChanged()
        {
        }
        public override bool OnKeyDown(System.Windows.Forms.Keys key)
        {
            if (!base.OnKeyDown(key))
            {
                m_PressingShift = (key == System.Windows.Forms.Keys.LShiftKey ||
                                key == System.Windows.Forms.Keys.RShiftKey ||
                                key == System.Windows.Forms.Keys.Shift ||
                                key == System.Windows.Forms.Keys.ShiftKey);
                return m_PressingShift;
            }
            return true;
        }
        public override XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = base.GenerateXmlElement(doc);
            foreach (var j in m_Joints)
            {
                var jnode = doc.CreateElement("Joint");
                jnode.SetAttribute("Xr", j.RatePos.X.ToString());
                jnode.SetAttribute("Yr", j.RatePos.Y.ToString());
                node.AppendChild(jnode);
            }
            return node;
        }
        public override void LoadFromXmlElement(XmlElement node)
        {
            base.LoadFromXmlElement(node);
            var nodes = node.GetElementsByTagName("Joint");
            foreach (XmlElement jnode in nodes)
            {
                float xr = Convert.ToSingle(jnode.GetAttribute("Xr"));
                float yr = Convert.ToSingle(jnode.GetAttribute("Yr"));
                AddJoint(xr, yr);
            }
        }
    }
    public class Joint
    {
        public Joint(float xr, float yr)
        {
            RatePos = new Vector2(xr, yr);
        }
        public Joint(Vector2 pos)
        {
            RatePos = pos;
        }
        public Joint(Joint jt)
        {
            RatePos = jt.RatePos;
        }
        public Vector2 RatePos
        {
            get;
            set;
        }
    }
    public class Segment
    {
        public Segment(ElementInfo_Waterbag elm, Joint j1, Joint j2)
        {
            ParentElement = elm;
            J1 = j1;
            J2 = j2;
        }
        public Segment(ElementInfo_Waterbag elm, Segment seg)
        {
            ParentElement = elm;
            J1 = seg.J1;
            J2 = seg.J2;
        }

        public Vector2[] GetBuffer(bool linkingJoint)
        {
            List<Vector2> vecs = new List<Vector2>();

            float vr = (float)ParentElement.ParentViewport.ViewportSize.Width / ParentElement.ParentViewport.ViewportSize.Height;
            var v1 = new Vector3(J1.RatePos, 0);
            var v2 = new Vector3(J2.RatePos, 0);
            Vector3 dir1 = v2 - v1, dir2;
            if (dir1.Length() == 0)
                dir2 = dir1;
            else
            {
                dir1.Normalize();
                dir2 = Vector3.Cross(dir1, new Vector3(0, 0, 1));
                dir2.Normalize();
                dir1.Y *= vr;
                dir2.Y *= vr;
            }
            float wr = ParentElement.BagWidthRate;
            var v = v1 + dir2 * wr;
            m_RectVecs[0] = new Vector2(v.X, v.Y);
            v = v1 - dir2 * wr;
            m_RectVecs[1] = new Vector2(v.X, v.Y);
            v = v2 + dir2 * wr;
            m_RectVecs[3] = new Vector2(v.X, v.Y);
            v = v2 - dir2 * wr;
            m_RectVecs[2] = new Vector2(v.X, v.Y);

            int[] indices = new int[] { 0, 1, 2, 0, 2, 3 };
            for (int i = 0; i < indices.Length; ++i)
            {
                vecs.Add(m_RectVecs[indices[i]]);
            }

            m_JointVecs.Clear();
            if (linkingJoint)
            {
                for (int i = 0; i < JointSectionCount; ++i)
                {
                    double angle = 360.0 / JointSectionCount * i * (System.Math.PI / 180.0);
                    double dx = System.Math.Sin(angle);
                    double dy = System.Math.Cos(angle) * vr;
                    var p = J1.RatePos + (new Vector2((float)dx, (float)dy) * ParentElement.BagWidthRate);
                    m_JointVecs.Add(p);
                }

                for (int i = 0; i < m_JointVecs.Count; ++i)
                {
                    vecs.Add(J1.RatePos);
                    vecs.Add(m_JointVecs[i]);
                    int j = ((i == m_JointVecs.Count - 1) ? 0 : (i + 1));
                    vecs.Add(m_JointVecs[j]);
                }
            }
            return vecs.ToArray();
        }
        public static readonly int JointSectionCount = 36;
        private Vector2[] m_RectVecs = new Vector2[4];
        private List<Vector2> m_JointVecs = new List<Vector2>();
        public Joint J1
        {
            get;
            set;
        }
        public Joint J2
        {
            get;
            set;
        }
        public Vector2[] Vectors
        {
            get
            {
                var vs = new List<Vector2>();
                foreach (var v in m_RectVecs)
                    vs.Add(new Vector2(v.X, v.Y));
                return vs.ToArray();
            }
        }
        public Vector2 Direction
        {
            get
            {
                var dir = (m_RectVecs[3] - m_RectVecs[0]);
                dir.Normalize();
                return new Vector2(dir.X, dir.Y);
            }
        }
        public ElementInfo_Waterbag ParentElement
        {
            get;
            private set;
        }
    }
}
