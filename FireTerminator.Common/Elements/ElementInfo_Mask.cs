using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FireTerminator.Common.RenderResources;
using System.Xml;

namespace FireTerminator.Common.Elements
{
    public class ElementInfo_Mask : ElementInfo
    {
        public ElementInfo_Mask(ResourceInfo res, ViewportInfo vi, bool isPreview, System.Drawing.PointF pos)
            : base(res, vi, isPreview, pos)
        {
            Name = "遮罩";
            var p = m_BaseTrans.RateLocation;
            var s = m_BaseTrans.RateSize;
            m_CornerVectorR[0] = new Vector2(-s.Width * 0.5F, -s.Height * 0.5F);
            m_CornerVectorR[1] = new Vector2(s.Width * 0.5F, -s.Height * 0.5F);
            m_CornerVectorR[2] = new Vector2(s.Width * 0.5F, s.Height * 0.5F);
            m_CornerVectorR[3] = new Vector2(-s.Width * 0.5F, s.Height * 0.5F);
            for (int i = 0; i < 4; ++i)
                m_Vectors[i] = new VertexPositionColorTexture();
            for (int i = 0; i < 5; ++i)
                m_LnVectors[i] = new VertexPositionColor();
        }
        public ElementInfo_Mask(ElementInfo_Mask e)
            : base(e)
        {
            for (int i = 0; i < 4; ++i)
                m_CornerVectorR[i] = e.m_CornerVectorR[i];
            for (int i = 0; i < 4; ++i)
                m_Vectors[i] = e.m_Vectors[i];
            for (int i = 0; i < 5; ++i)
                m_LnVectors[i] = e.m_LnVectors[i];
        }
        [Browsable(false)]
        public ResourceInfo_Dummy ResMask
        {
            get { return Resource as ResourceInfo_Dummy; }
        }
        [Browsable(false)]
        public override bool IsInnerEditable
        {
            get { return true; }
        }
        [Browsable(false)]
        public override bool UseDefaultRectangleBorder
        {
            get { return false; }
        }
        [Browsable(false)]
        public override System.Drawing.RectangleF BoundRect
        {
            get
            {
                float x1 = Single.MaxValue, x2 = Single.MinValue, y1 = Single.MaxValue, y2 = Single.MinValue;
                for (int i = 0; i < 4; ++i)
                {
                    var pos = m_Vectors[i].Position;
                    if (x1 > pos.X) x1 = pos.X;
                    if (x2 < pos.X) x2 = pos.X;
                    if (y1 > pos.Y) y1 = pos.Y;
                    if (y2 < pos.Y) y2 = pos.Y;
                }
                return new System.Drawing.RectangleF(x1, y1, x2 - x1, y2 - y1);
            }
        }
        [Browsable(false)]
        public Vector2[] CornerVectors
        {
            get
            {
                List<Vector2> rst = new List<Vector2>();
                foreach (var v in m_CornerVectorR)
                    rst.Add(new Vector2(v.X, v.Y));
                return rst.ToArray();
            }
        }

        private BodyOperationPart[] VectorBodyOptParts = new BodyOperationPart[]
        {
            BodyOperationPart.CornerLU,
            BodyOperationPart.CornerRU,
            BodyOperationPart.CornerRD,
            BodyOperationPart.CornerLD,
        };

        public override void Update(float elapsedTime, ref float curViewportTime)
        {
            base.Update(elapsedTime, ref curViewportTime);
            for (int i = 0; i < 4; ++i)
            {
                var pr = m_BlendedTransInfo.RateLocation;
                var offset = ParentViewport.GetRateLocation(IsBasedOnBackImageElement,
                    new System.Drawing.PointF(pr.X + m_CornerVectorR[i].X, pr.Y + m_CornerVectorR[i].Y));
                m_Vectors[i].Position = new Vector3(offset.X, offset.Y, 0);
                m_Vectors[i].Color = Color.White;
                var p = ParentViewport.GetLocationRate(true, new System.Drawing.PointF(m_Vectors[i].Position.X, m_Vectors[i].Position.Y));
                m_Vectors[i].TextureCoordinate = new Vector2(p.X, p.Y);
                m_LnVectors[i].Position = m_Vectors[i].Position;
                m_LnVectors[i].Color = Color.Yellow;
            }
            m_LnVectors[4].Position = m_Vectors[0].Position;
            m_LnVectors[4].Color = Color.Yellow;
        }
        public override bool Draw()
        {
            if (!base.Draw())
                return false;
            UsingEffect.GraphicsDevice.Viewport = ParentViewport.ViewportPtr;
            UsingEffect.Projection = Matrix.CreateOrthographicOffCenter(0, ParentViewport.ViewportPtr.Width, ParentViewport.ViewportPtr.Height, 0, 1.0f, 1000.0f);
            if (ParentViewport.BackImage != null && ParentViewport.BackImage.ResBackgroundImage != null)
            {
                UsingEffect.TextureEnabled = true;
                UsingEffect.Texture = ParentViewport.BackImage.ResBackgroundImage.ResTexture;
            }
            else
            {
                UsingEffect.TextureEnabled = false;
                UsingEffect.Texture = null;
            }
            
            UsingEffect.Begin();
            foreach (EffectPass pass in UsingEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                UsingEffect.GraphicsDevice.VertexDeclaration = new VertexDeclaration(UsingEffect.GraphicsDevice, VertexPositionColorTexture.VertexElements);
                UsingEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, m_Vectors, 0, 4, sm_Indices, 0, 2);
                if (IsSelected)
                    UsingEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, m_LnVectors, 0, 4);
                pass.End();
            }
            UsingEffect.End();
            return true;
        }
        public override BodyOperationPart GetPointBodyOprPart(System.Drawing.Point pt)
        {
            System.Drawing.PointF pos = pt;
            pos = ParentViewport.GetLocationRate(IsBasedOnBackImageElement, pos);
            float radius = 8.0F / ParentViewport.ViewportPtr.Width;
            if (IsInnerEditingMode)
            {
                for (int i = 0; i < 4; ++i)
                {
                    var p = new System.Drawing.PointF(m_Vectors[i].Position.X, m_Vectors[i].Position.Y);
                    p = ParentViewport.GetLocationRate(IsBasedOnBackImageElement, p);
                    double dx = p.X - pos.X;
                    double dy = p.Y - pos.Y;
                    double dist = System.Math.Sqrt(dx * dx + dy * dy);
                    if (dist <= radius)
                        return VectorBodyOptParts[i];
                }
            }
            if (BoundRectRate.Contains(pos))
                return BodyOperationPart.Body;
            return BodyOperationPart.Nothing;
        }
        public void SetPosition(BodyOperationPart part, System.Drawing.Point pPosOrBodyOffset)
        {
            System.Drawing.PointF p = pPosOrBodyOffset;
            p = ParentViewport.GetLocationRate(IsBasedOnBackImageElement, pPosOrBodyOffset, part == BodyOperationPart.Body);
            if (part != BodyOperationPart.Body)
            {
                p.X -= m_BaseTrans.Xr;
                p.Y -= m_BaseTrans.Yr;
            }
            switch (part)
            {
                case BodyOperationPart.CornerLU:
                    m_CornerVectorR[0].X = p.X; m_CornerVectorR[0].Y = p.Y; break;
                case BodyOperationPart.CornerRU:
                    m_CornerVectorR[1].X = p.X; m_CornerVectorR[1].Y = p.Y; break;
                case BodyOperationPart.CornerRD:
                    m_CornerVectorR[2].X = p.X; m_CornerVectorR[2].Y = p.Y; break;
                case BodyOperationPart.CornerLD:
                    m_CornerVectorR[3].X = p.X; m_CornerVectorR[3].Y = p.Y; break;
                case BodyOperationPart.Body:
                    {
                        m_BaseTrans.Xr = p.X;
                        m_BaseTrans.Yr = p.Y;
                    } break;
            }
        }
        public void SetPosition(Vector2[] vecs)
        {
            for (int i = 0; i < 4; ++i)
                m_CornerVectorR[i] = vecs[i];
        }
        public override XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = base.GenerateXmlElement(doc);
            for (int i = 1; i <= m_CornerVectorR.Length; ++i)
                node.SetAttribute("Corner" + i, String.Format("{0},{1}", m_CornerVectorR[i - 1].X, m_CornerVectorR[i - 1].Y));
            return node;
        }
        public override void LoadFromXmlElement(XmlElement node)
        {
            for (int i = 1; i <= m_CornerVectorR.Length; ++i)
            {
                var value = node.GetAttribute("Corner" + i);
                if (!String.IsNullOrEmpty(value))
                {
                    System.Drawing.PointF pos = new System.Drawing.PointF(0, 0);
                    CommonMethods.GetFormatFloat2(value, ref pos);
                    m_CornerVectorR[i - 1] = new Vector2(pos.X, pos.Y);
                }
            }
        }

        protected Vector2[] m_CornerVectorR = new Vector2[4];
        protected VertexPositionColorTexture[] m_Vectors = new VertexPositionColorTexture[4];
        protected VertexPositionColor[] m_LnVectors = new VertexPositionColor[5];
        protected static short[] sm_Indices = new short[] { 0, 1, 2, 0, 2, 3 };
    }
}
