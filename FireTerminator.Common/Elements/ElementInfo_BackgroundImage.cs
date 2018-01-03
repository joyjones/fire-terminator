using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FireTerminator.Common.RenderResources;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace FireTerminator.Common.Elements
{
    public class ElementInfo_BackgroundImage : ElementInfo_StaticImage
    {
        public ElementInfo_BackgroundImage(ResourceInfo res, ViewportInfo vi, bool isPreview, System.Drawing.PointF pos)
            : base(res, vi, isPreview, pos)
        {
            Barycenter = new System.Drawing.PointF(0, 0);
        }
        public ElementInfo_BackgroundImage(ElementInfo_BackgroundImage e)
            : base(e)
        {
            Barycenter = e.Barycenter;
            m_fZoomScale = e.m_fZoomScale;
            m_CurViewOffset = e.m_CurViewOffset;
            m_MinViewOffset = e.m_MinViewOffset;
            m_MaxViewOffset = e.m_MaxViewOffset;
            m_CurScaledImageSize = e.m_CurScaledImageSize;
            m_CurScaleProportion = e.m_CurScaleProportion;
        }
        [Browsable(false)]
        public ResourceInfo_BackgroundImage ResBackgroundImage
        {
            get { return Resource as ResourceInfo_BackgroundImage; }
        }
        [Browsable(false)]
        public override Microsoft.Xna.Framework.Graphics.Texture2D CurrentTexture
        {
            get
            {
                if (ResBackgroundImage.IsVideoResource)
                {
                    if (DsVideo == null)
                        DsVideo = new DSVideoPlayer(ResBackgroundImage.FullFilePath, ProjectDoc.Instance.HostGame.GraphicsDevice);
                    return DsVideo.OutputFrame;
                }
                return base.CurrentTexture;
            }
        }
        public DSVideoPlayer DsVideo
        {
            get;
            private set;
        }
        public override bool CanSelect
        {
            get { return false; }
        }
        public override System.Drawing.Point Location
        {
            get { return base.Location; }
            set { }
        }
        public override System.Drawing.Size Size
        {
            get { return base.Size; }
        }
        public override float RotateAngle
        {
            get { return 0; }
            set { }
        }
        public float CurImageScale
        {
            get
            {
                if (ResBackgroundImage == null)
                    return 1;
                return m_CurScaledImageSize.Width / ResBackgroundImage.ImageSize.Width;
            }
        }
        public System.Drawing.SizeF CurImageSize
        {
            get { return m_CurScaledImageSize; }
        }
        public float CurZoomScale
        {
            get { return m_fZoomScale; }
            set
            {
                CommonMethods.ClampValue(ref value, 0.001F, 1000F);
                Scale(0.5F, 0.5F, value);
            }
        }
        public override bool IsBasedOnBackImageElement
        {
            get { return false; }
        }
        public System.Drawing.PointF CurViewOffset
        {
            get { return m_CurViewOffset; }
            set
            {
                m_CurViewOffset = value;
                m_CurViewOffset.X = CommonMethods.ClampValue(m_CurViewOffset.X, m_MinViewOffset.X, m_MaxViewOffset.X);
                m_CurViewOffset.Y = CommonMethods.ClampValue(m_CurViewOffset.Y, m_MinViewOffset.Y, m_MaxViewOffset.Y);
            }
        }
        public System.Drawing.PointF CurMaxViewOffset
        {
            get { return m_MaxViewOffset; }
        }
        public System.Drawing.PointF CurMinViewOffset
        {
            get { return m_MinViewOffset; }
        }
        [Browsable(false)]
        public static Texture2D DefaultTexture
        {
            get;
            protected set;
        }

        protected float m_fZoomScale = 1;
        private System.Drawing.PointF m_CurViewOffset = new System.Drawing.PointF(0, 0);
        private System.Drawing.PointF m_MinViewOffset = new System.Drawing.PointF(0, 0);
        private System.Drawing.PointF m_MaxViewOffset = new System.Drawing.PointF(0, 0);
        private System.Drawing.SizeF m_CurScaledImageSize = new System.Drawing.SizeF(0, 0);
        private System.Drawing.PointF m_CurScaleProportion = new System.Drawing.PointF(0, 0);

        public override void Update(float elapsedTime, ref float curViewportTime)
        {
            base.Update(elapsedTime, ref curViewportTime);

            if (DsVideo != null)
            {
                if (DsVideo.CurrentState == VideoState.Stopped)
                    DsVideo.Play();
                DsVideo.Update();
            }
        }
        public void Scale(float proportionX, float proportionY, float zoomRate)
        {
            if (ResBackgroundImage == null || zoomRate <= 0)
                return;
            CommonMethods.ClampValue(ref proportionX, 0, 1);
            CommonMethods.ClampValue(ref proportionY, 0, 1);
            var sizeView = ParentViewport.ViewportSize;
            System.Drawing.SizeF sizeMin = ResBackgroundImage.ImageSize;
            CommonMethods.GainOutterFittableRegion(ref sizeMin, sizeView, out m_MaxViewOffset);
            m_MaxViewOffset = new System.Drawing.PointF(0, 0);

            System.Drawing.SizeF imgProportion = new System.Drawing.SizeF();
            imgProportion.Width = (proportionX * sizeView.Width + Math.Abs(m_CurViewOffset.X)) / m_CurScaledImageSize.Width;
            imgProportion.Height = (proportionY * sizeView.Height + Math.Abs(m_CurViewOffset.Y)) / m_CurScaledImageSize.Height;
            
            m_CurScaledImageSize.Width = sizeMin.Width * zoomRate;
            m_CurScaledImageSize.Height = sizeMin.Height * zoomRate;

            m_MinViewOffset.X = sizeView.Width - m_CurScaledImageSize.Width;
            m_MinViewOffset.Y = sizeView.Height - m_CurScaledImageSize.Height;

            System.Drawing.PointF offset = new System.Drawing.PointF();
            offset.X = sizeView.Width * proportionX - m_CurScaledImageSize.Width * imgProportion.Width;
            offset.Y = sizeView.Height * proportionY - m_CurScaledImageSize.Height * imgProportion.Height;
            CurViewOffset = offset;

            m_fZoomScale = zoomRate;
            m_CurScaleProportion.X = proportionX;
            m_CurScaleProportion.Y = proportionY;
        }
        public override void OnBeforeBlendedTransforms()
        {
            if (IsPreviewElement)
                base.OnBeforeBlendedTransforms();
            else
            {
                m_BaseTrans.RateLocation = ParentViewport.GetLocationRate(false, m_CurViewOffset);
                m_BaseTrans.RateSize = ParentViewport.GetSizeRate(false, m_CurScaledImageSize);
            }
        }
        public override void OnViewportSizeChanged()
        {
            base.OnViewportSizeChanged();
            m_CurViewOffset = ParentViewport.GetRateLocation(false, m_BaseTrans.RateLocation);
            m_CurScaledImageSize = ParentViewport.GetRateSize(false, m_BaseTrans.RateSize);
            Scale(m_CurScaleProportion.X, m_CurScaleProportion.Y, m_fZoomScale);
        }
        public override void OnAfterAdded()
        {
            base.OnAfterAdded();
            if (!IsDuplicatedObject)
            {
                CurZoomScale = m_fZoomScale;
                System.Drawing.PointF offset = new System.Drawing.PointF();
                offset.X = (m_MaxViewOffset.X + m_MinViewOffset.X) * 0.5F;
                offset.Y = (m_MaxViewOffset.Y + m_MinViewOffset.Y) * 0.5F;
                CurViewOffset = offset;
            }
        }
        public override void OnBeforeRemove()
        {
            base.OnBeforeRemove();
            if (DsVideo != null)
                DsVideo.Stop();
        }
        public override System.Xml.XmlElement GenerateXmlElement(System.Xml.XmlDocument doc)
        {
            XmlElement node = base.GenerateXmlElement(doc);
            node.SetAttribute("ZoomScale", m_fZoomScale.ToString());
            return node;
        }
        public override void LoadFromXmlElement(System.Xml.XmlElement node)
        {
            base.LoadFromXmlElement(node);
            float fZoomScale;
            if (!float.TryParse(node.GetAttribute("ZoomScale"), out fZoomScale))
                fZoomScale = 1;
            m_CurViewOffset = ParentViewport.GetRateLocation(false, m_BaseTrans.RateLocation);
            m_CurScaledImageSize = ParentViewport.GetRateSize(false, m_BaseTrans.RateSize);
            CurZoomScale = fZoomScale;
        }
    }
}
