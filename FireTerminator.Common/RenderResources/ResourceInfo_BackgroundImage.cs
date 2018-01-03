using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FireTerminator.Common.Elements;

namespace FireTerminator.Common.RenderResources
{
    public class ResourceInfo_BackgroundImage : ResourceInfo_StaticImage
    {
        public ResourceInfo_BackgroundImage(ResourceGroup rg, string file)
            : base(rg, file)
        {
            IsVideoResource = false;
            foreach (var fmt in ResourceGroup.GetFileFilters(ResourceKind.视频))
            {
                if (file.ToLower().EndsWith(fmt))
                {
                    IsVideoResource = true;
                    break;
                }
            }
        }
        public static string DefaultBackgroundImageFile
        {
            get { return Options.SystemResourceRootPath + "background.png"; }
        }
        [Category("背景"), DisplayName("尺寸")]
        public override System.Drawing.SizeF ImageSize
        {
            get
            {
                if (VideoInfo == null)
                {
                    var elm = PreviewElement as ElementInfo_BackgroundImage;
                    if (elm != null && elm.DsVideo != null)
                        VideoInfo = elm.DsVideo.Info;
                }
                if (VideoInfo != null)
                    return new System.Drawing.SizeF(VideoInfo.Width, VideoInfo.Height);
                return base.ImageSize;
            }
        }
        public override void Load()
        {
            base.Load();
        }
        public bool IsVideoResource
        {
            get;
            private set;
        }
        public DSVideoInfo VideoInfo
        {
            get;
            private set;
        }
    }
}
