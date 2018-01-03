using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FireTerminator.Common.RenderResources
{
    public class ResourceInfo_AnimImage : ResourceInfo_StaticImage
    {
        public ResourceInfo_AnimImage(ResourceGroup rg, string file)
            : base(rg, file)
        {
        }
        public override System.Drawing.SizeF ImageSize
        {
            get
            {
                if (ResTexture == null)
                    return new System.Drawing.SizeF(0, 0);
                return new System.Drawing.SizeF(ResTexture.Width / 8, ResTexture.Height / 8);
            }
        }
        [Category("帧动画"), DisplayName("总帧数")]
        public int FrameCount
        {
            get { return 64; }
        }
        [Category("帧动画"), DisplayName("每行帧数")]
        public int RowFrameCount
        {
            get { return 8; }
        }
    }
}
