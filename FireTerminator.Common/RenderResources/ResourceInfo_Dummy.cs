using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FireTerminator.Common.RenderResources
{
    public class ResourceInfo_Dummy : ResourceInfo
    {
        public ResourceInfo_Dummy(ResourceKind kind)
            : base(kind)
        {
            CustomImageSize = new System.Drawing.SizeF(50, 50);
        }
        [Category("默认资源"), DisplayName("尺寸")]
        public override System.Drawing.SizeF ImageSize
        {
            get { return CustomImageSize; }
        }
        public System.Drawing.SizeF CustomImageSize
        {
            get;
            set;
        }
        public override void Load()
        {
            IsLoaded = true;
        }
    }
}
