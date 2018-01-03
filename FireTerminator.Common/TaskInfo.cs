using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using FireTerminator.Common.Operations;

namespace FireTerminator.Common
{
    // 任务信息
    public class TaskInfo : TreeListNodeObject
    {
        public TaskInfo(ProjectInfo proj, string name)
        {
            Name = name;
            ParentProjectInfo = proj;
        }
        public TaskInfo(ProjectInfo proj, XmlElement root)
        {
            ParentProjectInfo = proj;
            Name = root.GetAttribute("Name");
            foreach (XmlElement node in root.GetElementsByTagName("Scene"))
            {
                SceneInfos.Add(new SceneInfo(this, node));
            }
        }
        public TaskInfo(TaskInfo tinfo)
        {
            ParentProjectInfo = null;
            Name = tinfo.Name;
            foreach (var si in tinfo.SceneInfos)
            {
                AddScene(new SceneInfo(si));
            }
        }
        public ProjectInfo ParentProjectInfo
        {
            get;
            internal set;
        }
        [Category("场景"), DisplayName("名称")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        private SceneInfo m_SelectedSceneInfo = null;
        [Browsable(false)]
        public SceneInfo SelectedSceneInfo
        {
            get { return m_SelectedSceneInfo; }
            set
            {
                if (m_SelectedSceneInfo != value)
                {
                    if (value != null && value.ParentTaskInfo != this)
                        return;
                    if (m_SelectedSceneInfo != null)
                        m_SelectedSceneInfo.IsSelected = false;
                    m_SelectedSceneInfo = value;
                    if (m_SelectedSceneInfo != null)
                    {
                        //System.Diagnostics.Debug.WriteLine("Cur Scene = " + m_SelectedSceneInfo.Name);
                        m_SelectedSceneInfo.IsSelected = true;
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("Cur Scene = CLEAR");
                    }
                }
            }
        }
        [Browsable(false)]
        public string FreeDefaultSceneName
        {
            get
            {
                int index = 0;
                string name;
                do
                {
                    ++index;
                    name = "新建场景" + index;
                } while (GetScene(name) != null);
                return name;
            }
        }
        public void Clear()
        {
            SelectedSceneInfo = null;
            foreach (SceneInfo si in SceneInfos)
                si.Clear();
            SceneInfos.Clear();
        }
        public XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = doc.CreateElement("Task");
            node.SetAttribute("Name", Name);
            foreach (var si in SceneInfos)
            {
                var subnode = si.GenerateXmlElement(doc);
                node.AppendChild(subnode);
            }
            return node;
        }
        public SceneInfo CreateScene()
        {
            var si = new SceneInfo(this, FreeDefaultSceneName);
            SceneInfos.Add(si);
            return si;
        }
        public void AddScene(SceneInfo si)
        {
            if (si == null || si.ParentTaskInfo == this)
                return;
            si.Remove();
            int i = 0;
            string name = si.Name;
            while (GetScene(name) != null)
                name = si.Name + "_" + (++i).ToString();
            if (i > 0)
                si.Name = name;
            si.ParentTaskInfo = this;
            SceneInfos.Add(si);
            ProjectDoc.Instance.OnAppendedSceneInfo(si);
            OperationHistory.Instance.IsDirty = true;
        }
        public bool RemoveScene(SceneInfo si)
        {
            if (SceneInfos.Contains(si))
            {
                if (SceneInfos.Remove(si))
                {
                    OperationHistory.Instance.IsDirty = true;
                    return true;
                }
            }
            return false;
        }
        public bool Remove()
        {
            if (ParentProjectInfo != null)
                return ParentProjectInfo.RemoveTask(this);
            return true;
        }
        public bool BringSceneEarly(SceneInfo si)
        {
            int index = SceneInfos.IndexOf(si);
            if (index <= 0)
                return false;
            var si1 = SceneInfos[index - 1];
            SceneInfos.Remove(si1);
            SceneInfos.Insert(index, si1);
            OperationHistory.Instance.IsDirty = true;
            return true;
        }
        public bool BringSceneDelay(SceneInfo si)
        {
            int index = SceneInfos.IndexOf(si);
            if (index < 0 || index == SceneInfos.Count - 1)
                return false;
            SceneInfos.Remove(si);
            SceneInfos.Insert(index + 1, si);
            OperationHistory.Instance.IsDirty = true;
            return true;
        }
        public SceneInfo GetScene(string name)
        {
            foreach (var si in SceneInfos)
            {
                if (si.Name == name)
                    return si;
            }
            return null;
        }
        public void Update(float elapsedTime)
        {
            if (SelectedSceneInfo != null)
            {
                SelectedSceneInfo.Update(elapsedTime);
            }
        }
        public void OnKeyDown(System.Windows.Forms.Keys key)
        {
            if (SelectedSceneInfo != null)
            {
                SelectedSceneInfo.OnKeyDown(key);
            }
        }
        public override string ToString()
        {
            return Name;
        }
        public List<SceneInfo> SceneInfos = new List<SceneInfo>();
    }
}
