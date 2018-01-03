using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using FireTerminator.Common.Structures;

namespace FireTerminator.Common.RenderResources
{
    public class ResourceGroup
    {
        public ResourceGroup(ResourceKind rp)
        {
            ResKind = rp;
            PathName = rp.ToString();
        }
        public void Clear()
        {
            foreach (var lst in ResInfos.Values)
            {
                foreach (var res in lst)
                    res.Unload();
            }
            ResInfos.Clear();
        }
        public void Reload()
        {
            Clear();
            LoadPath(RootPath);
        }
        private void LoadPath(string dir)
        {
            if (!Directory.Exists(dir))
                return;
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                if (IsFileNameValid(file))
                {
                    var info = System.Activator.CreateInstance(ResourceInfo.ResTypes[ResKind], this, file) as ResourceInfo;
                    if (!ResInfos.ContainsKey(info.SubPath))
                        ResInfos[info.SubPath] = new List<ResourceInfo> { info };
                    else
                    {
                        var lst = ResInfos[info.SubPath];
                        bool exist = false;
                        foreach (var r in lst)
                        {
                            if (r.SubPathFileName == info.SubPathFileName)
                            {
                                exist = true;
                                break;
                            }
                        }
                        if (!exist)
                            ResInfos[info.SubPath].Add(info);
                    }
                }
            }
            var dirs = Directory.GetDirectories(dir);
            foreach (var subdir in dirs)
            {
                LoadPath(subdir);
            }
        }
        [Category("资源组"), DisplayName("目录名"), ReadOnly(true)]
        public string PathName
        {
            get;
            set;
        }
        [Category("资源组"), DisplayName("路径")]
        public string RootPath
        {
            get { return Options.UserResourceRootPath + PathName + "\\"; }
        }
        [Category("资源组"), DisplayName("资源类型")]
        public ResourceKind ResKind
        {
            get;
            private set;
        }
        public static string[] GetFileFilters(ResourceKind kind)
        {
            switch (kind)
            {
                case ResourceKind.背景:
                    return new string[] { ".jpg", ".jpeg", ".png", ".gif", ".tga", ".bmp", ".avi", ".mpg", ".wmv" };
                case ResourceKind.图像:
                case ResourceKind.效果:
                    return new string[] { ".jpg", ".jpeg", ".png", ".gif", ".tga", ".bmp" };
                case ResourceKind.音频:
                    return new string[] { ".ogg", ".mp3", ".wma", ".wav" };
                case ResourceKind.视频:
                    return new string[] { ".avi", ".mpg", ".wmv", ".asf", ".rm", ".rmvb", ".bik", ".mov", ".flv", ".3gp", ".vob", ".flc", ".mp4" };//, ".f4v", ".swf", ".mkv"
            }
            return new string[] { };
        }
        public override string ToString()
        {
            return ResKind.ToString();
        }
        public bool IsFileNameValid(string file)
        {
            if (ResKind == ResourceKind.视频)
                return true;
            foreach (var ext in GetFileFilters(ResKind))
            {
                if (file.ToLower().EndsWith(ext))
                    return true;
            }
            return false;
        }
        public ResourceInfo GetResourceInfo(string relativePath)
        {
            if (ResourceInfo.ResTypes[ResKind] == typeof(ResourceInfo_Dummy))
                return System.Activator.CreateInstance(ResourceInfo.ResTypes[ResKind], ResKind) as ResourceInfo;
            relativePath = relativePath.Replace("\\\\", "\\");
            string name = Path.GetFileName(relativePath);
            string subdir = Path.GetDirectoryName(relativePath).TrimEnd('\\') + "\\";
            string fullfile = RootPath + relativePath;
            fullfile = fullfile.Replace("\\\\", "\\");
            if (!File.Exists(fullfile))
            {
                fullfile = SearchResourceFile(name);
                if (!File.Exists(fullfile))
                {
                    AppLogger.Write(String.Format("错误：未能找到资源文件<{0}>！", RootPath + relativePath));
                    return null;
                }
                else
                {
                    name = Path.GetFileName(fullfile);
                    subdir = Path.GetDirectoryName(fullfile.Substring(RootPath.Length)).TrimEnd('\\') + "\\";
                    AppLogger.Write(String.Format("警告：资源文件<{0}>在路径中不存在，已重定位至<{1}>！", RootPath + relativePath, fullfile));
                }
            }
            List<ResourceInfo> lst = null;
            if (ResInfos.TryGetValue(subdir, out lst))
            {
                foreach (var res in lst)
                {
                    if (res.FileName == name)
                        return res;
                }
            }
            return null;
        }
        public string SearchResourceFile(string filename)
        {
            string[] files = Directory.GetFiles(RootPath, filename, SearchOption.AllDirectories);
            if (files.Length == 0)
                return "";
            return files[0];
        }
        public int CheckResourceFinalizations(Dictionary<string, ResourceInfo> usingReses)
        {
            int count = 0;
            foreach (var lst in ResInfos.Values)
            {
                foreach (var res in lst)
                {
                    if (res.IsLoaded && !usingReses.ContainsKey(res.FullFilePath))
                    {
                        res.Unload();
                        ++count;
                    }
                }
            }
            return count;
        }
        public Dictionary<string, List<ResourceInfo>> ResInfos = new Dictionary<string, List<ResourceInfo>>();
    }
}
