using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using FireTerminator.Common.Elements;

namespace FireTerminator.Common.RenderResources
{
    public abstract class ResourceInfo
    {
        static ResourceInfo()
        {
            ResTypes[ResourceKind.背景] = typeof(ResourceInfo_BackgroundImage);
            ResTypes[ResourceKind.图像] = typeof(ResourceInfo_StaticImage);
            ResTypes[ResourceKind.效果] = typeof(ResourceInfo_AnimImage);
            ResTypes[ResourceKind.视频] = typeof(ResourceInfo_Video);
            ResTypes[ResourceKind.音频] = typeof(ResourceInfo_Audio);
            ResTypes[ResourceKind.水带] = typeof(ResourceInfo_Dummy);
            ResTypes[ResourceKind.遮罩] = typeof(ResourceInfo_Dummy);
            ResTypes[ResourceKind.文本] = typeof(ResourceInfo_Dummy);
        }
        public ResourceInfo(ResourceGroup rg, string file)
        {
            ParentGroup = rg;
            IsLoaded = false;
            if (rg != null && !String.IsNullOrEmpty(file))
            {
                FileName = Path.GetFileName(file);
                SubPath = file.Substring(rg.RootPath.Length, file.Length - rg.RootPath.Length - FileName.Length).TrimEnd('\\') + "\\";
            }
            else if (!String.IsNullOrEmpty(file) && File.Exists(file))
            {
                FileName = Path.GetFileName(file);
                m_CustomFullFilePath = file;
            }
        }
        public ResourceInfo(ResourceKind kind)
        {
            m_CustomKind = kind;
        }
        
        private ResourceKind m_CustomKind = ResourceKind.图像;
        [Category("资源文件"), DisplayName("资源类型")]
        public virtual ResourceKind Kind
        {
            get
            {
                if (ParentGroup == null)
                    return m_CustomKind;
                return ParentGroup.ResKind;
            }
            set
            {
                m_CustomKind = value;
            }
        }
        [Category("资源文件"), DisplayName("相对路径")]
        public string SubPath
        {
            get;
            private set;
        }
        [Category("资源文件"), DisplayName("文件名")]
        public string FileName
        {
            get;
            private set;
        }
        [Browsable(false)]
        public string SubPathFileName
        {
            get { return SubPath + FileName; }
        }
        private string m_CustomFullFilePath = "";
        [Category("资源文件"), DisplayName("文件路径")]
        public string FullFilePath
        {
            get
            {
                if (ParentGroup == null)
                    return m_CustomFullFilePath;
                return ParentGroup.RootPath + SubPath + FileName;
            }
            set { m_CustomFullFilePath = value; }
        }
        [Browsable(false)]
        public ResourceGroup ParentGroup
        {
            get;
            private set;
        }
        [Category("资源文件"), DisplayName("尺寸")]
        public abstract System.Drawing.SizeF ImageSize
        {
            get;
        }
        [Browsable(false)]
        public ElementInfo PreviewElement
        {
            get;
            protected set;
        }
        [Browsable(false)]
        public bool IsLoaded
        {
            get;
            set;
        }

        public virtual void Load()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;
                PreviewElement = System.Activator.CreateInstance(ElementInfo.ElementTypes[Kind], this, ProjectDoc.Instance.PreviewViewport, true, new System.Drawing.PointF(0, 0)) as ElementInfo;
                PreviewElement.IsPreviewElement = true;
            }
        }
        public virtual void Unload()
        {
            PreviewElement = null;
            IsLoaded = false;
        }
        public override string ToString()
        {
            return FileName;
        }
        public bool IsFileSupported(string file)
        {
            foreach (var fmt in ResourceGroup.GetFileFilters(Kind))
            {
                if (file.ToLower().EndsWith(fmt))
                    return true;
            }
            return false;
        }
        public static Dictionary<ResourceKind, Type> ResTypes = new Dictionary<ResourceKind, Type>();
    }
}
