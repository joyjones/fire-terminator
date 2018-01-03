using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FireTerminator.Common.Transitions
{
    public class RenderingRectangle
    {
        public RenderingRectangle()
        {
            UseFrameRectLines = true;
            UseFrameRectProjectingEffect = false;
            for (int i = 0; i < m_Vectors.Length; ++i)
                m_Vectors[i] = new VertexPositionColorTexture();
            m_Vectors[0].TextureCoordinate = new Vector2(0, 0);
            m_Vectors[1].TextureCoordinate = new Vector2(1, 0);
            m_Vectors[2].TextureCoordinate = new Vector2(1, 1);
            m_Vectors[3].TextureCoordinate = new Vector2(0, 1);

            for (int i = 0; i < m_RectLines.Length; ++i)
            {
                m_RectLines[i] = new VertexPositionColor();
                m_RectLines[i].Color = Color.Black;
            }
            FrameRectLineColorLight = Color.White;
            FrameRectLineColorDark = Color.Gray;
            RenderOffsetX = 0;
            RenderOffsetY = 0;
        }
        private System.Drawing.RectangleF m_Region = new System.Drawing.RectangleF();
        public System.Drawing.RectangleF Region
        {
            get { return m_Region; }
        }
        public bool UseFrameRectLines
        {
            get;
            set;
        }
        public bool UseFrameRectProjectingEffect
        {
            get;
            set;
        }
        public int RenderOffsetX
        {
            get;
            set;
        }
        public int RenderOffsetY
        {
            get;
            set;
        }
        private Color[] m_FrameRectLineColors = new Color[2];
        public Color FrameRectLineColorLight
        {
            get { return m_FrameRectLineColors[0]; }
            set
            {
                m_FrameRectLineColors[0] = value;
                foreach (int idx in new int[] { 0, 1, 6, 7 })
                    m_RectLines[idx].Color = value;
            }
        }
        public Color FrameRectLineColorDark
        {
            get { return m_FrameRectLineColors[1]; }
            set
            {
                m_FrameRectLineColors[1] = value;
                foreach (int idx in new int[] { 2, 3, 4, 5 })
                    m_RectLines[idx].Color = value;
            }
        }
        public void Update(float x, float y, float w, float h, Color clr)
        {
            m_Region.X = x; m_Region.Y = y;
            m_Region.Width = w; m_Region.Height = h;
            m_Vectors[0].Color = clr;
            m_Vectors[0].Position = new Vector3(x, y, 0);
            m_Vectors[1].Color = clr;
            m_Vectors[1].Position = new Vector3(x + w, y, 0);
            m_Vectors[2].Color = clr;
            m_Vectors[2].Position = new Vector3(x + w, y + h, 0);
            m_Vectors[3].Color = clr;
            m_Vectors[3].Position = new Vector3(x, y + h, 0);
            for (int i = 0; i < 4; i++)
            {
                m_Vectors[i].Position.X += RenderOffsetX;
                m_Vectors[i].Position.Y += RenderOffsetY;
            }

            if (UseFrameRectLines)
            {
                int d = UseFrameRectProjectingEffect ? 1 : 0;
                m_RectLines[0].Position = new Vector3(x, y + d, 0);
                m_RectLines[1].Position = new Vector3(x + w, y + d, 0);
                m_RectLines[6].Position = new Vector3(x + d, y + h, 0);
                m_RectLines[7].Position = new Vector3(x + d, y, 0);

                m_RectLines[2].Position = new Vector3(x + w - d, y + h, 0);
                m_RectLines[3].Position = new Vector3(x + w - d, y, 0);
                m_RectLines[4].Position = new Vector3(x + w, y + h - d, 0);
                m_RectLines[5].Position = new Vector3(x, y + h - d, 0);

                m_RectLines[8].Position = new Vector3(x + w, y, 0);
                m_RectLines[9].Position = new Vector3(x + w, y + h, 0);
                m_RectLines[10].Position = new Vector3(x + w, y + h, 0);
                m_RectLines[11].Position = new Vector3(x, y + h, 0);
                for (int i = 0; i < 12; i++)
                {
                    m_RectLines[i].Position.X += RenderOffsetX;
                    m_RectLines[i].Position.Y += RenderOffsetY;
                }
            }
        }
        public virtual void Draw(GraphicsDevice device)
        {
            device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColorTexture.VertexElements);
            device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, m_Vectors, 0, 4, sm_Indices, 0, 2);

            if (UseFrameRectLines)
            {
                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, m_RectLines, 0, UseFrameRectProjectingEffect ? 6 : 4);
            }
        }
        private VertexPositionColorTexture[] m_Vectors = new VertexPositionColorTexture[4];
        private static short[] sm_Indices = new short[] { 0, 1, 2, 0, 2, 3 };
        private VertexPositionColor[] m_RectLines = new VertexPositionColor[12];
    }
}
