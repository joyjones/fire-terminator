using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FireTerminator.Common
{
    public class ViewportDesc
    {
        public ViewportDesc(SceneDesc scene, XmlElement vnode)
        {
            ParentScene = scene;
            Index = int.Parse(vnode.GetAttribute("Index"));
            foreach (XmlElement enode in vnode.GetElementsByTagName("Element"))
            {
                if (enode.ParentNode == vnode)
                {
                    Elements.Add(enode.GetAttribute("Name"));
                    var kind = (ResourceKind)Enum.Parse(typeof(ResourceKind), enode.GetAttribute("Kind"));
                    if (kind == ResourceKind.水带 || kind == ResourceKind.文本 || kind == ResourceKind.遮罩)
                        continue;
                    var name = enode.GetAttribute("ResourceFile").TrimStart('\\');
                    if (kind == ResourceKind.背景 && name == "background.png")
                        continue;
                    var resFile = ProjectDoc.Instance.ResourceGroups[kind].PathName + "\\" + name;
                    if (!ParentScene.ParentTask.ParentProject.ResourceFiles.Contains(resFile))
                    {
                        ParentScene.ParentTask.ParentProject.ResourceFiles.Add(resFile);
                    }
                }
            }
        }
        public int Index
        {
            get;
            set;
        }
        public string Name
        {
            get { return "窗口" + (Index + 1).ToString(); }
        }
        public SceneDesc ParentScene
        {
            get;
            private set;
        }
        public List<string> Elements = new List<string>();
    }
    public class SceneDesc
    {
        public SceneDesc(TaskDesc task, XmlElement snode)
        {
            ParentTask = task;
            Name = snode.GetAttribute("Name");
            int index = -1;
            foreach (XmlElement vnode in snode.GetElementsByTagName("Viewport"))
            {
                if (++index > 3)
                    break;
                Viewports[index] = new ViewportDesc(this, vnode);
            }
        }
        public string Name
        {
            get;
            private set;
        }
        public TaskDesc ParentTask
        {
            get;
            private set;
        }
        public ViewportDesc[] Viewports = new ViewportDesc[4];
    }
    public class TaskDesc
    {
        public TaskDesc(ProjectDesc proj, XmlElement tnode)
        {
            ParentProject = proj;
            Name = tnode.GetAttribute("Name");
            foreach (XmlElement snode in tnode.GetElementsByTagName("Scene"))
            {
                Scenes.Add(new SceneDesc(this, snode));
            }
        }
        public string Name
        {
            get;
            private set;
        }
        public ProjectDesc ParentProject
        {
            get;
            private set;
        }
        public SceneDesc GetScene(string name)
        {
            foreach (var s in Scenes)
            {
                if (s.Name == name)
                    return s;
            }
            return null;
        }
        public List<SceneDesc> Scenes = new List<SceneDesc>();
    }
    public class ProjectDesc
    {
        public ProjectDesc(string projFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(projFile);
            Name = Path.GetFileNameWithoutExtension(projFile);
            FilePath = projFile.Substring(0, projFile.LastIndexOf('\\') + 1);
            foreach (XmlElement tnode in doc.DocumentElement.GetElementsByTagName("Task"))
            {
                Tasks.Add(new TaskDesc(this, tnode));
            }
        }
        public string FilePath
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }
        public string FileName
        {
            get { return Name + ProjectInfo.DefaultFileExt; }
        }
        public string FullFileName
        {
            get { return FilePath + FileName; }
        }
        public string[] MissingResourceFiles
        {
            get
            {
                List<string> files = new List<string>();
                foreach (var resName in ResourceFiles)
                {
                    string resFile = Options.UserResourceRootPath + resName;
                    if (!File.Exists(resFile))
                    {
                        files.Add(resName);
                    }
                }
                return files.ToArray();
            }
        }
        public TaskDesc GetTask(string name)
        {
            foreach (var t in Tasks)
            {
                if (t.Name == name)
                    return t;
            }
            return null;
        }
        public List<TaskDesc> Tasks = new List<TaskDesc>();
        public List<string> ResourceFiles = new List<string>();
    }
}
