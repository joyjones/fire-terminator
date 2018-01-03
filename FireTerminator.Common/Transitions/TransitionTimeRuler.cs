using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FireTerminator.Common.Transitions
{
    public class TransitionTimeRuler : RenderingRectangle
    {
        public TransitionTimeRuler(TransitionGraphics drawer, int height)
        {
            ParentDrawer = drawer;
            Height = height;
            CurFocusTimePixelWidth = -1;
            m_rcVertTimeLine.UseFrameRectLines = false;
            m_rcVertTimeLine.UseFrameRectProjectingEffect = false;

            if (ProjectDoc.Instance.HostGame != null)
            {
                m_TextSprite = new SpriteBatch(ProjectDoc.Instance.HostGame.GraphicsDevice);
                m_TextFont = ProjectDoc.Instance.HostGame.Content.Load<SpriteFont>("depend\\DefaultFont");
            }
        }
        public TransitionGraphics ParentDrawer
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            set;
        }
        private SpriteFont m_TextFont = null;
        private SpriteBatch m_TextSprite = null;
        private float m_CurTime = 0;
        public float CurTime
        {
            get
            {
                ViewportInfo vi = null;
                if (ParentDrawer.BindedElement != null)
                    vi = ParentDrawer.BindedElement.ParentViewport;
                else
                    vi = ProjectDoc.Instance.SelectedViewportInfo;
                if (vi != null && (ProjectDoc.Instance.IsProjectAnimationPlaying || (vi.IsAnimEditingMode && vi.IsPlaying)))
                    m_CurTime = vi.CurTimeTick;
                return m_CurTime;
            }
            set
            {
                var vi = ParentDrawer.BindedElement.ParentViewport;
                if (!vi.IsPlaying)
                {
                    CommonMethods.ClampValue(ref value, 0, CurMaxTime);
                    if (m_CurTime != value)
                    {
                        m_CurTime = value;
                        if (CurTimeChanged != null)
                            CurTimeChanged(m_CurTime);
                    }
                    if (vi.IsAnimEditingMode)
                    {
                        vi.CurTimeTick = m_CurTime;
                    }
                }
            }
        }
        public int CurTimePixelWidth
        {
            get { return (int)(CurTime * ParentDrawer.PixelsPerSecond); }
            set { CurTime = value / (float)ParentDrawer.PixelsPerSecond; }
        }
        public int CurFocusTimePixelWidth
        {
            get;
            set;
        }
        public float CurMaxTime
        {
            get
            {
                ViewportInfo vi = null;
                if (ParentDrawer.BindedElement != null)
                    vi = ParentDrawer.BindedElement.ParentViewport;
                else
                    vi = ProjectDoc.Instance.SelectedViewportInfo;
                if (vi != null)
                    return vi.TimeLength;
                return 0;
            }
        }
        public int CurMaxTimePixelWidth
        {
            get
            {
                ViewportInfo vi = null;
                if (ParentDrawer.BindedElement != null)
                    vi = ParentDrawer.BindedElement.ParentViewport;
                else
                    vi = ProjectDoc.Instance.SelectedViewportInfo;
                if (vi == null)
                    return ParentDrawer.ViewSize.Width;
                return (int)(vi.TimeLength * ParentDrawer.PixelsPerSecond);
            }
        }
        public void Update(float elapsedTime)
        {
            base.Update(0, 0, CurMaxTimePixelWidth, Height, Color.White);
            if (ParentDrawer.ViewSize.Width != CurMaxTimePixelWidth)
            {
                ParentDrawer.ParentPanel.Width = CurMaxTimePixelWidth;
                ParentDrawer.ParentPanel.Height = ParentDrawer.TimeRuler.Height + Height * 5;
            }
            float w = ParentDrawer.PixelsPerSecond;
            m_VecLines.Clear();
            int y1 = (int)(Height * 0.4F);
            int y2 = (int)(Height * 0.6F);
            int y3 = (int)(Height * 0.8F);
            for (int i = 1; i < (int)ParentDrawer.MaxTimeLength; ++i)
            {
                int x = (int)(w * i) + RenderOffsetX;
                int y;
                if ((i % 10) == 0)
                    y = y1;
                else if ((i % 5) == 0)
                    y = y2;
                else
                    y = y3;
                y += RenderOffsetY;
                //m_VecLines.Add(new VertexPositionColor(new Vector3(x - 1, y, 0), Color.Black));
                //m_VecLines.Add(new VertexPositionColor(new Vector3(x - 1, Height, 0), Color.Black));
                m_VecLines.Add(new VertexPositionColor(new Vector3(x, y, 0), Color.Black));
                m_VecLines.Add(new VertexPositionColor(new Vector3(x, Height, 0), Color.Black));
            }
            if (CurFocusTimePixelWidth > 0)
            {
                float x = CurFocusTimePixelWidth + RenderOffsetX;
                m_VecLines.Add(new VertexPositionColor(new Vector3(x, RenderOffsetY, 0), Color.Yellow));
                m_VecLines.Add(new VertexPositionColor(new Vector3(x, ParentDrawer.ViewSize.Height + RenderOffsetY, 0), Color.Yellow));
            }
            m_rcVertTimeLine.Update(CurTimePixelWidth - 1 + RenderOffsetX, RenderOffsetY, 2, ParentDrawer.ViewSize.Height, Color.Red);
        }
        public override void Draw(GraphicsDevice device)
        {
            base.Draw(device);
            if (m_VecLines.Count > 0)
            {
                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, m_VecLines.ToArray(), 0, m_VecLines.Count / 2);
            }

            m_rcVertTimeLine.Draw(device);

            m_TextSprite.Begin();
            int vcount = m_VecLines.Count;
            if (CurFocusTimePixelWidth > 0)
                vcount -= 2;
            float drawScale = 0.4f;
            for (int i = 0; i < vcount; i += 2)
            {
                int s = i / 2 + 1;
                if (s % 5 == 0)
                {
                    float x = m_VecLines[i].Position.X;
                    Vector2 orign = m_TextFont.MeasureString(s.ToString()) / 2;
                    m_TextSprite.DrawString(m_TextFont, s.ToString(), new Vector2(x, 0), Color.Brown, 0, new Vector2(orign.X, 0), drawScale, SpriteEffects.None, 0.5f);
                }
            }
            if (CurFocusTimePixelWidth > 0)
            {
                float x = m_VecLines[vcount].Position.X;
                string text = ParentDrawer.CurMouseFocusTime.ToString("0.00");
                Vector2 orign = m_TextFont.MeasureString(text) / 2;
                m_TextSprite.DrawString(m_TextFont, text.ToString(), new Vector2(x, 0), Color.Brown, 0, new Vector2(orign.X, 0), drawScale, SpriteEffects.None, 0.5f);
            }
            m_TextSprite.End();
        }
        public delegate void Delegate_CurTimeChanged(float time);
        public event Delegate_CurTimeChanged CurTimeChanged;
        private RenderingRectangle m_rcVertTimeLine = new RenderingRectangle();
        private List<VertexPositionColor> m_VecLines = new List<VertexPositionColor>();
    }
}
