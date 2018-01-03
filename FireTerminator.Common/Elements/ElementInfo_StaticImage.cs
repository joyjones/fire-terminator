using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FireTerminator.Common.RenderResources;

namespace FireTerminator.Common.Elements
{
    public class ElementInfo_StaticImage : ElementInfo
    {
        public ElementInfo_StaticImage(ResourceInfo res, ViewportInfo vi, bool isPreview, System.Drawing.PointF pos)
            : base(res, vi, isPreview, pos)
        {
            if (vi != null && vi.BackImage != null)
            {
                m_ManualScale.Width = vi.BackImage.CurImageScale;
                m_ManualScale.Height = vi.BackImage.CurImageScale;
            }

            for (int i = 0; i < 4; ++i)
                m_Vectors[i] = new VertexPositionColorTexture();
        }
        public ElementInfo_StaticImage(ElementInfo_StaticImage e)
            : base(e)
        {
            for (int i = 0; i < 4; ++i)
                m_Vectors[i] = e.m_Vectors[i];
        }
        [Browsable(false)]
        public ResourceInfo_StaticImage ResStaticImage
        {
            get { return Resource as ResourceInfo_StaticImage; }
        }
        [Browsable(false)]
        public virtual Texture2D CurrentTexture
        {
            get
            {
                if (ResStaticImage != null)
                    return ResStaticImage.ResTexture;
                return null;
            }
        }
        public static readonly string[] SupportedFormats =
        {
            ".jpg", ".jpeg", ".png", ".gif", ".tga", ".bmp", ".avi", ".mpg", ".wmv"
        };
        protected VertexPositionColorTexture[] m_Vectors = new VertexPositionColorTexture[4];
        protected static short[] sm_Indices = new short[] { 0, 1, 2, 0, 2, 3 };
        protected virtual void UpdateTextureCoordinates()
        {
            if (IsTextureHoriFlipped)
            {
                var vec = m_Vectors[1].TextureCoordinate;
                m_Vectors[1].TextureCoordinate = m_Vectors[0].TextureCoordinate;
                m_Vectors[0].TextureCoordinate = vec;
                vec = m_Vectors[3].TextureCoordinate;
                m_Vectors[3].TextureCoordinate = m_Vectors[2].TextureCoordinate;
                m_Vectors[2].TextureCoordinate = vec;
            }
            if (IsTextureVertFlipped)
            {
                var vec = m_Vectors[0].TextureCoordinate;
                m_Vectors[0].TextureCoordinate = m_Vectors[3].TextureCoordinate;
                m_Vectors[3].TextureCoordinate = vec;
                vec = m_Vectors[1].TextureCoordinate;
                m_Vectors[1].TextureCoordinate = m_Vectors[2].TextureCoordinate;
                m_Vectors[2].TextureCoordinate = vec;
            }
        }
        public override void Update(float elapsedTime, ref float curViewportTime)
        {
            base.Update(elapsedTime, ref curViewportTime);
            elapsedTime *= SpeedRate;
            Color clr = new Color(BlendColor.R, BlendColor.G, BlendColor.B, (byte)(m_BlendedTransInfo.Alpha * 255.0F));
            var pos = ParentViewport.GetRateLocation(IsBasedOnBackImageElement, m_BlendedTransInfo.RateLocation);
            var size = ParentViewport.GetRateSize(IsBasedOnBackImageElement, m_BlendedTransInfo.RateSize);
            pos.X -= size.Width * Barycenter.X;
            pos.Y -= size.Height * Barycenter.Y;
            m_Vectors[0].Color = clr;
            m_Vectors[0].Position = new Vector3(pos.X, pos.Y, 0);
            m_Vectors[1].Color = clr;
            m_Vectors[1].Position = new Vector3(pos.X + size.Width, pos.Y, 0);
            m_Vectors[2].Color = clr;
            m_Vectors[2].Position = new Vector3(pos.X + size.Width, pos.Y + size.Height, 0);
            m_Vectors[3].Color = clr;
            m_Vectors[3].Position = new Vector3(pos.X, pos.Y + size.Height, 0);
            m_Vectors[0].TextureCoordinate = new Vector2(0, 0);
            m_Vectors[1].TextureCoordinate = new Vector2(1, 0);
            m_Vectors[2].TextureCoordinate = new Vector2(1, 1);
            m_Vectors[3].TextureCoordinate = new Vector2(0, 1);
            UpdateTextureCoordinates();
            if (m_BlendedTransInfo.Angle != 0)
            {
                float angle = m_BlendedTransInfo.Angle / 180.0F * (float)Math.PI;
                for (int i = 0; i < 4; ++i)
                {
                    var bbl = BlendedBaryLocation;
                    var cen = new Vector3(bbl.X, bbl.Y, 0);
                    var mat = Matrix.CreateTranslation(cen * -1)
                        * Matrix.CreateRotationZ(angle)
                        * Matrix.CreateTranslation(cen);
                    m_Vectors[i].Position = Vector3.Transform(m_Vectors[i].Position, mat);
                }
            }
        }
        public override bool Draw()
        {
            if (!base.Draw())
                return false;
            UsingEffect.Projection = Matrix.CreateOrthographicOffCenter(0, ParentViewport.ViewportPtr.Width, ParentViewport.ViewportPtr.Height, 0, 1.0f, 1000.0f);
            if (CurrentTexture == null)
            {
                UsingEffect.TextureEnabled = false;
                UsingEffect.Texture = null;
            }
            else
            {
                UsingEffect.TextureEnabled = true;
                UsingEffect.Texture = CurrentTexture;
            }
            UsingEffect.Begin();
            foreach (EffectPass pass in UsingEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                UsingEffect.GraphicsDevice.VertexDeclaration = new VertexDeclaration(UsingEffect.GraphicsDevice, VertexPositionColorTexture.VertexElements);
                UsingEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, m_Vectors, 0, 4, sm_Indices, 0, 2);

                pass.End();
            }
            UsingEffect.End();
            return true;
        }
    }
}
