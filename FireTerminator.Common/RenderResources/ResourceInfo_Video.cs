using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FireTerminator.Common.Elements;

namespace FireTerminator.Common.RenderResources
{
    public class ResourceInfo_Video : ResourceInfo
    {
        public ResourceInfo_Video(ResourceGroup rg, string file)
            : base(rg, file)
        {
        }
        [Category("视频"), DisplayName("尺寸")]
        public override System.Drawing.SizeF ImageSize
        {
            get
            {
                if (VideoInfo == null)
                {
                    var elm = PreviewElement as ElementInfo_Video;
                    if (elm != null && elm.DsVideo != null)
                        VideoInfo = elm.DsVideo.Info;
                }
                if (VideoInfo != null)
                    return new System.Drawing.SizeF(VideoInfo.Width, VideoInfo.Height);
                return new System.Drawing.SizeF(1, 1);
            }
        }
        public DSVideoInfo VideoInfo
        {
            get;
            protected set;
        }

        public override void Load()
        {
            base.Load();
        }
        public override void Unload()
        {
            if (PreviewElement != null)
                PreviewElement.OnBeforeRemove();
            VideoInfo = null;
            base.Unload();
        }
    }
}
