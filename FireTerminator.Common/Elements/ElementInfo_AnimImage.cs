using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using FireTerminator.Common.RenderResources;

namespace FireTerminator.Common.Elements
{
    public class ElementInfo_AnimImage : ElementInfo_StaticImage
    {
        public ElementInfo_AnimImage(ResourceInfo res, ViewportInfo vi, bool isPreview, System.Drawing.PointF pos)
            : base(res, vi, isPreview, pos)
        {
            CurFrame = 0;
            CurElapsedAnimTime = 0;
        }
        public ElementInfo_AnimImage(ElementInfo_AnimImage e)
            : base(e)
        {
            CurFrame = e.CurFrame;
            CurElapsedAnimTime = e.CurElapsedAnimTime;
        }
        [Browsable(false)]
        public ResourceInfo_AnimImage ResAnimImage
        {
            get { return Resource as ResourceInfo_AnimImage; }
        }
        [Browsable(false)]
        public int CurFrame
        {
            get;
            private set;
        }
        [Browsable(false)]
        public float CurElapsedAnimTime
        {
            get;
            private set;
        }
        protected override void UpdateTextureCoordinates()
        {
            int frmCount = ResAnimImage.FrameCount;
            int frmCountPerRow = ResAnimImage.RowFrameCount;
            int rowCount = frmCount / frmCountPerRow;
            if ((frmCount % frmCountPerRow) != 0)
                rowCount += 1;
            float timePerFrm = 1.0F / ProjectDoc.Instance.Option.AnimFrameCountPerSecond;
            CurFrame = (int)(CurElapsedAnimTime / timePerFrm);
            if (CurFrame >= frmCount)
            {
                CurFrame = 0;
                CurElapsedAnimTime = 0;
            }
            int frmRow = CurFrame / frmCountPerRow;
            if (!ProjectDoc.Instance.Option.AnimFrameRowsUpToDown)
                frmRow = rowCount - frmRow - 1;
            int frmCol = CurFrame % frmCountPerRow;
            float u1 = frmCol / (float)frmCountPerRow;
            float u2 = (frmCol + 1) / (float)frmCountPerRow;
            float v1 = frmRow / (float)rowCount;
            float v2 = (frmRow + 1) / (float)rowCount;
            m_Vectors[0].TextureCoordinate = new Vector2(u1, v1);
            m_Vectors[1].TextureCoordinate = new Vector2(u2, v1);
            m_Vectors[2].TextureCoordinate = new Vector2(u2, v2);
            m_Vectors[3].TextureCoordinate = new Vector2(u1, v2);
            base.UpdateTextureCoordinates();
        }
        public override void Update(float elapsedTime, ref float curViewportTime)
        {
            CurElapsedAnimTime += elapsedTime * SpeedRate;
            base.Update(elapsedTime, ref curViewportTime);
        }
    }
}
