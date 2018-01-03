using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FireTerminator.Common.Audio;
using FireTerminator.Common.Elements;

namespace FireTerminator.Common.RenderResources
{
    public class ResourceInfo_Audio : ResourceInfo
    {
        public ResourceInfo_Audio(ResourceGroup rg, string file)
            : base(rg, file)
        {
        }
        [Category("音频"), DisplayName("尺寸")]
        public override System.Drawing.SizeF ImageSize
        {
            get { return new System.Drawing.SizeF(1, 1); }
        }
        public override void Load()
        {
            base.Load();
        }
        public override void Unload()
        {
            if (PreviewElement != null)
            {
                var sound = PreviewElement as ElementInfo_Audio;
                sound.Channel.播放中 = false;
            }
            base.Unload();
        }
    }
}
