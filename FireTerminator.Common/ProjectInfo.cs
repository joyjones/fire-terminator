using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using FireTerminator.Common.Operations;
using System.Windows.Forms;
using System.IO;
using FireTerminator.Common.RenderResources;

namespace FireTerminator.Common
{
    // 项目信息
    public class ProjectInfo
    {
        public ProjectInfo(string name)
        {
            Name = name;
            IsInTransaction = false;
            PlayCenter = new PlayCenter(this);
            CreateTask();
        }
        public string Name
        {
            get;
            set;
        }
        public string File
        {
            get;
            private set;
        }
        public string FileName
        {
            get
            {
                if (String.IsNullOrEmpty(File))
                    return "";
                return Path.GetFileNameWithoutExtension(File);
            }
        }
        public static readonly string DefaultFileExt = ".fpj";
        private TaskInfo m_SelectedTaskInfo = null;
        [Browsable(false)]
        public TaskInfo SelectedTaskInfo
        {
            get { return m_SelectedTaskInfo; }
            set
            {
                if (m_SelectedTaskInfo != value)
                {
                    if (value != null && value.ParentProjectInfo != this)
                        return;
                    if (m_SelectedTaskInfo != null)
                        m_SelectedTaskInfo.IsSelected = false;
                    m_SelectedTaskInfo = value;
                    if (m_SelectedTaskInfo != null)
                    {
                        m_SelectedTaskInfo.IsSelected = true;
                        //System.Diagnostics.Debug.WriteLine("Cur Task = " + m_SelectedTaskInfo.Name);
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("Cur Task = CLEAR");
                    }
                }
            }
        }
        [Browsable(false)]
        public bool IsViewportSelected
        {
            get
            {
                if (SelectedTaskInfo == null)
                    return false;
                if (SelectedTaskInfo.SelectedSceneInfo == null)
                    return false;
                return SelectedTaskInfo.SelectedSceneInfo.SelectedViewportInfo != null;
            }
        }
        [Browsable(false)]
        public string FreeDefaultTaskName
        {
            get
            {
                int index = 0;
                string name;
                do 
                {
                    ++index;
                    name = "新建任务" + index;
                } while (GetTask(name) != null) ;
                return name;
            }
        }
        [Browsable(false)]
        public PlayCenter PlayCenter
        {
            get;
            protected set;
        }
        public bool IsInTransaction
        {
            get;
            protected set;
        }

        public void Clear()
        {
            IsInTransaction = true;
            File = "";
            SelectedTaskInfo = null;
            OperationHistory.Instance.Clear();
            foreach (TaskInfo ti in TaskInfos)
                ti.Clear();
            TaskInfos.Clear();
            Name = "新建项目";
            IsInTransaction = false;
        }
        public void Load(string file)
        {
            SelectedTaskInfo = null;
            Clear();
            if (file.Length <= 3)
                return;
            if (file[1] != ':')
                file = Options.DefaultProjectsRootPath + file;
            File = file;
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            Name = doc.DocumentElement.GetAttribute("Name");
            foreach (XmlElement node in doc.DocumentElement.GetElementsByTagName("Task"))
            {
                TaskInfos.Add(new TaskInfo(this, node));
            }
            if (TaskInfos.Count > 0)
            {
                ProjectDoc.Instance.SelectedTaskInfo = TaskInfos[0];
                if (TaskInfos[0].SceneInfos.Count > 0)
                {
                    TaskInfos[0].SelectedSceneInfo = TaskInfos[0].SceneInfos[0];
                }
            }

            if (ProjectDoc.Instance.GetProjectDescription(file) == null)
            {
                ProjectDoc.Instance.ReloadProjectDescription(file);
            }
        }
        public void Save()
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(File);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<FireTerminatorProject></FireTerminatorProject>");
            doc.DocumentElement.SetAttribute("Name", Name);
            foreach (var ti in TaskInfos)
            {
                var node = ti.GenerateXmlElement(doc);
                doc.DocumentElement.AppendChild(node);
            }
            doc.Save(File);
            OperationHistory.Instance.IsDirty = false;
            ProjectDoc.Instance.ReloadProjectDescription(File);
        }
        public void SaveAs(string file)
        {
            File = file;
            Save();
        }
        public bool ExportResourceFiles(string targetDir)
        {
            if (String.IsNullOrEmpty(File) || OperationHistory.Instance.IsDirty)
            {
                MessageBox.Show("请先保存项目！", "导出项目资源");
                return false;
            }
            if (String.IsNullOrEmpty(targetDir))
            {
                return false;
            }
            return ProjectDoc.Instance.ExportProjectResourceFiles(File, targetDir, true);
        }
        public TaskInfo CreateTask()
        {
            SelectedTaskInfo = new TaskInfo(this, FreeDefaultTaskName);
            SelectedTaskInfo.SelectedSceneInfo = SelectedTaskInfo.CreateScene();
            TaskInfos.Add(SelectedTaskInfo);
            return SelectedTaskInfo;
        }
        public TaskInfo GetTask(string name)
        {
            foreach (var ti in TaskInfos)
            {
                if (ti.Name == name)
                    return ti;
            }
            return null;
        }
        public void AddTask(TaskInfo ti)
        {
            if (ti == null || ti.ParentProjectInfo == this)
                return;
            ti.Remove();
            int i = 0;
            string name = ti.Name;
            while (GetTask(name) != null)
                name = ti.Name + "_" + (++i).ToString();
            if (i > 0)
                ti.Name = name;
            ti.ParentProjectInfo = this;
            TaskInfos.Add(ti);
            OperationHistory.Instance.IsDirty = true;
        }
        public bool RemoveTask(TaskInfo ti)
        {
            if (TaskInfos.Contains(ti))
            {
                if (TaskInfos.Remove(ti))
                {
                    OperationHistory.Instance.IsDirty = true;
                    return true;
                }
            }
            return false;
        }
        public bool BringTaskEarly(TaskInfo ti)
        {
            int index = TaskInfos.IndexOf(ti);
            if (index <= 0)
                return false;
            var ti1 = TaskInfos[index - 1];
            TaskInfos.Remove(ti1);
            TaskInfos.Insert(index, ti1);
            OperationHistory.Instance.IsDirty = true;
            return true;
        }
        public bool BringTaskDelay(TaskInfo ti)
        {
            int index = TaskInfos.IndexOf(ti);
            if (index < 0 || index == TaskInfos.Count - 1)
                return false;
            TaskInfos.Remove(ti);
            TaskInfos.Insert(index + 1, ti);
            OperationHistory.Instance.IsDirty = true;
            return true;
        }
        public void LockAllElements()
        {
            foreach (var t in TaskInfos)
            {
                foreach (var s in t.SceneInfos)
                {
                    foreach (var v in s.Viewports)
                    {
                        v.LockAllElements();
                    }
                }
            }
        }
        public void RemoveAllUserElements()
        {
            foreach (var t in TaskInfos)
            {
                foreach (var s in t.SceneInfos)
                {
                    foreach (var v in s.Viewports)
                    {
                        v.RemoveAllUserElements();
                    }
                }
            }
        }
        public void Update(float elapsedTime)
        {
            if (SelectedTaskInfo != null)
            {
                SelectedTaskInfo.Update(elapsedTime);
                PlayCenter.Update(elapsedTime);
            }
        }
        public void OnKeyDown(System.Windows.Forms.Keys key)
        {
            if (SelectedTaskInfo != null)
            {
                SelectedTaskInfo.OnKeyDown(key);
            }
        }
        public int GetUsingResources(ref Dictionary<string, ResourceInfo> results)
        {
            foreach (var t in TaskInfos.ToArray())
            {
                foreach (var s in t.SceneInfos.ToArray())
                {
                    foreach (var v in s.Viewports.ToArray())
                    {
                        foreach (var e in v.Elements.ToArray())
                        {
                            if (e != null && e.Resource != null && !(e.Resource is ResourceInfo_Dummy))
                            {
                                results[e.Resource.FullFilePath] = e.Resource;
                            }
                        }
                    }
                }
            }
            return results.Count;
        }

        public List<TaskInfo> TaskInfos = new List<TaskInfo>();
    }
}
