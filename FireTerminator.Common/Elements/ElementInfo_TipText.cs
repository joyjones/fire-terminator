using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.RenderResources;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace FireTerminator.Common.Elements
{
    public class ElementInfo_TipText : ElementInfo
    {
        private void Initialize()
        {
            m_TextSprite = new SpriteBatch(ProjectDoc.Instance.HostGame.GraphicsDevice);
            Name = "文本";
            DrawScale = 1;
        }
        public ElementInfo_TipText(ResourceInfo res, ViewportInfo vi, bool isPreview, System.Drawing.PointF pos)
            : base(res, vi, isPreview, pos)
        {
            Initialize();
            Caption = "NewText";
        }
        public ElementInfo_TipText(ElementInfo_TipText e)
            : base(e)
        {
            Initialize();
            DrawScale = e.DrawScale;
        }
        [Browsable(false)]
        public override System.Drawing.SizeF ManualScale
        {
            get { return base.ManualScale; }
            set { base.ManualScale = value; }
        }
        [Category("外观"), DisplayName("文字缩放比"), DefaultValue(1.0F)]
        public float DrawScale
        {
            get { return m_ManualScale.Height; }
            set { ManualScale = new System.Drawing.SizeF(value, value); }
        }
        [Browsable(false)]
        public override bool ShowCaption
        {
            get { return false; }
            set { }
        }
        [Browsable(false)]
        public ResourceInfo_Dummy CustomResource
        {
            get { return Resource as ResourceInfo_Dummy; }
        }
        private SpriteBatch m_TextSprite = null;
        public override void Update(float elapsedTime, ref float curViewportTime)
        {
            base.Update(elapsedTime, ref curViewportTime);

            Vector2 orign = UsingFont.MeasureString(Caption);
            if (orign.X == 0 || orign.Y == 0)
                orign = new Vector2(10, 10);
            var size = new System.Drawing.SizeF(orign.X, orign.Y);
            if (size != CustomResource.CustomImageSize)
            {
                CustomResource.CustomImageSize = size;
                OnViewportBackImageChanged(ParentViewport.BackImage, true);
            }
        }
        public override bool Draw()
        {
            if (!base.Draw())
                return false;

            var rs = ProjectDoc.Instance.HostGame.GraphicsDevice.RenderState;

            var arg_c = rs.CullMode;
            var arg_d = rs.DestinationBlend;
            var arg_s = rs.SourceBlend;

            m_TextSprite.Begin();
            SpriteEffects se = SpriteEffects.None;
            if (IsTextureHoriFlipped)
                se = SpriteEffects.FlipHorizontally;
            else if (IsTextureVertFlipped)
                se = SpriteEffects.FlipVertically;
            var clr = new Color(BlendColor.R, BlendColor.G, BlendColor.B, BlendColor.A);
            float rot = MathHelper.ToRadians(RotateAngle);
            float scale = DrawScale;
            Vector2 region = ProjectDoc.Instance.DefaultFont.MeasureString(Caption) * 0.5F;
            var pos = new Vector2(Location.X + region.X, Location.Y + region.Y);
            if (!IsBlendingDisabled && (ProjectDoc.Instance.IsProjectAnimationPlaying || (ParentViewport.IsAnimEditingMode && IsSelected)))
            {
                scale *= m_AnimateTrans.Hr;
                rot = BlendedRotateAngle;
                pos = new Vector2(BlendedLocation.X + region.X, BlendedLocation.Y + region.Y);
            }
            m_TextSprite.DrawString(UsingFont, Caption, pos, clr, rot, new Vector2(region.X, region.Y), scale, se, 0.5f);
            m_TextSprite.End();

            rs.CullMode = arg_c;
            rs.DestinationBlend = arg_d;
            rs.SourceBlend = arg_s;

            return true;
        }
        public override void OnViewportBackImageChanged(ElementInfo_BackgroundImage elmBack, bool forceChange)
        {
            ScaleProportionOnBackImage = 0;
        }
        public override System.Xml.XmlElement GenerateXmlElement(System.Xml.XmlDocument doc)
        {
            return base.GenerateXmlElement(doc);
        }
        public override void LoadFromXmlElement(System.Xml.XmlElement node)
        {
            base.LoadFromXmlElement(node);
        }
    }
}
