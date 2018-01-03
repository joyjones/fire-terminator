using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;

namespace FireTerminator.Common.RenderResources
{
    public class ResourceInfo_StaticImage : ResourceInfo
    {
        public ResourceInfo_StaticImage(ResourceGroup rg, string file)
            : base(rg, file)
        {
        }
        [Category("图片"), DisplayName("尺寸")]
        public override System.Drawing.SizeF ImageSize
        {
            get
            {
                if (ResTexture == null)
                    return new System.Drawing.SizeF(0, 0);
                return new System.Drawing.SizeF(ResTexture.Width, ResTexture.Height);
            }
        }
        [Browsable(false)]
        public Texture2D ResTexture
        {
            get;
            protected set;
        }
        public override void Load()
        {
            base.Load();
            if (ResTexture == null && IsFileSupported(this.FileName))
            {
                try
                {
                    ResTexture = Texture2D.FromFile(ProjectDoc.Instance.HostGame.GraphicsDevice, FullFilePath);
                }
                catch (Exception ex)
                {
                    ResTexture = null;
                    System.Windows.Forms.MessageBox.Show("读取图像文件'" + FileName + "'失败!\r\n" + ex.Message);
                }
            }
        }
        public override void Unload()
        {
            if (ResTexture != null)
                ResTexture.Dispose();
            ResTexture = null;
            base.Unload();
        }
        public static readonly List<string> SupportedFormats = new List<string>()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".tga", ".bmp"
        };
        public static new bool IsFileSupported(string file)
        {
            foreach (var fmt in SupportedFormats)
            {
                if (file.ToLower().EndsWith(fmt))
                    return true;
            }
            return false;
        }
    }
}
