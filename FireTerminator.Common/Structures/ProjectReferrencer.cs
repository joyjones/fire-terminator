using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DevExpress.XtraTreeList.Nodes;

namespace FireTerminator.Common.Structures
{
    [DataContract]
    public abstract class ProjectReferrencer
    {
        private ProjectInfo m_ProjectInstance = null;
        public virtual ProjectInfo ProjectInstance
        {
            get { return m_ProjectInstance; }
            internal set
            {
                if (m_ProjectInstance != value)
                {
                    if (m_ProjectInstance != null)
                        ProjectDoc.Instance.CloseProject(m_ProjectInstance);
                    m_ProjectInstance = value;
                    if (m_ProjectInstance != null)
                    {
                        ViewportIndex = -1;
                        SceneName = "";
                        TaskName = "";
                        FreeTaskPerm = FreeTaskPermission.禁止自由选择;
                        m_ProjectInstance.LockAllElements();
                    }
                    foreach (TaskGroupColumn col in Enum.GetValues(typeof(TaskGroupColumn)))
                    {
                        UpdateNode(col);
                    }
                }
            }
        }
        public virtual ProjectInfo ProjectInstanceDirectly
        {
            get { return m_ProjectInstance; }
            set { m_ProjectInstance = value; }
        }
        public virtual ProjectDesc ProjectDescription
        {
            get
            {
                if (ProjectInstance == null)
                    return null;
                return ProjectDoc.Instance.GetProjectDescription(ProjectInstance.FileName);
            }
        }
        public abstract long[] UserIDs
        {
            get;
        }
        public virtual bool IsProjectSpecified
        {
            get { return ProjectInstance != null; }
        }
        public virtual bool IsTaskSceneSpecified
        {
            get { return IsProjectSpecified && ProjectInstance.SelectedTaskInfo != null && ProjectInstance.SelectedTaskInfo.SelectedSceneInfo != null; }
        }
        public virtual bool IsTaskViewSpecified
        {
            get { return IsTaskSceneSpecified && ViewportIndex >= 0; }
        }
        public virtual ViewportInfo SpecificViewportInfo
        {
            get
            {
                if (!IsTaskViewSpecified)
                    return null;
                return ProjectInstance.SelectedTaskInfo.SelectedSceneInfo.SelectedViewportInfo;
            }
        }
        public virtual string ProjectName
        {
            get
            {
                if (IsProjectSpecified)
                    return ProjectInstance.FileName;
                return "";
            }
            set
            {
                if (ProjectName != value)
                {
                    if (ProjectInstance != null)
                        ProjectDoc.Instance.CloseProject(ProjectInstance);
                    ProjectInstance = ProjectDoc.Instance.CreateProject(value, true, true);
                }
            }
        }
        public virtual string TaskName
        {
            get
            {
                if (IsTaskSceneSpecified)
                    return ProjectInstance.SelectedTaskInfo.Name;
                return "";
            }
            set
            {
                if (IsProjectSpecified)
                    ProjectInstance.SelectedTaskInfo = ProjectInstance.GetTask(value);
                UpdateNode(TaskGroupColumn.任务名称);
            }
        }
        public virtual string SceneName
        {
            get
            {
                if (IsTaskSceneSpecified)
                    return ProjectInstance.SelectedTaskInfo.SelectedSceneInfo.Name;
                return "";
            }
            set
            {
                if (IsProjectSpecified && ProjectInstance.SelectedTaskInfo != null)
                    ProjectInstance.SelectedTaskInfo.SelectedSceneInfo = ProjectInstance.SelectedTaskInfo.GetScene(value);
                UpdateNode(TaskGroupColumn.场景名称);
            }
        }
        private int m_ViewportIndex = -1;
        public virtual int ViewportIndex
        {
            get { return m_ViewportIndex; }
            set
            {
                if (!IsTaskSceneSpecified)
                    m_ViewportIndex = -1;
                else
                {
                    if (value < 0 || value >= 4)
                    {
                        value = -1;
                        ProjectInstance.SelectedTaskInfo.SelectedSceneInfo.IsSelectedViewportMaximized = false;
                        ProjectInstance.SelectedTaskInfo.SelectedSceneInfo.SelectedViewportInfo = null;
                    }
                    else
                    {
                        ProjectInstance.SelectedTaskInfo.SelectedSceneInfo.SelectedViewportInfo = ProjectInstance.SelectedTaskInfo.SelectedSceneInfo.Viewports[value];
                        ProjectInstance.SelectedTaskInfo.SelectedSceneInfo.IsSelectedViewportMaximized = true;
                    }
                    m_ViewportIndex = value;
                }
                UpdateNode(TaskGroupColumn.窗口索引);
            }
        }
        public virtual float CurAnimTime
        {
            get
            {
                if (SpecificViewportInfo != null)
                    return SpecificViewportInfo.CurTimeTick;
                return 0;
            }
        }
        public TreeListNode Node
        {
            get;
            set;
        }
        protected List<string> m_MissingResourceFiles = new List<string>();
        public string[] MissingResourceFiles
        {
            get { return m_MissingResourceFiles.ToArray(); }
            set
            {
                m_MissingResourceFiles.Clear();
                if (value != null)
                    m_MissingResourceFiles.AddRange(value);
                UpdateNode(TaskGroupColumn.项目及完整度);
            }
        }
        [DataMember]
        public string Name = "";
        [DataMember]
        public bool IsReplaying = false;
        [DataMember]
        public bool IsResSynchronizing = false;
        [DataMember]
        public FreeTaskPermission FreeTaskPerm = FreeTaskPermission.禁止自由选择;
        [DataMember]
        public Dictionary<int, float> TaskScores = new Dictionary<int, float>();

        protected bool m_IsStarted = false;
        public virtual bool IsStarted
        {
            get { return m_IsStarted; }
            set
            {
                bool changed = m_IsStarted != value;
                m_IsStarted = value;
                if (ProjectInstance != null)
                {
                    if (changed)
                        ProjectInstance.RemoveAllUserElements();
                    if (IsTaskViewSpecified)
                        ProjectInstance.PlayCenter.IsPlaying = m_IsStarted;
                }
            }
        }
        public virtual bool IsPaused
        {
            get
            {
                if (ProjectInstance != null)
                    return ProjectInstance.PlayCenter.IsPaused;
                return false;
            }
            set
            {
                if (ProjectInstance != null)
                    ProjectInstance.PlayCenter.IsPaused = value;
            }
        }
        public long[] BeMonitoringUserIDs
        {
            get { return BeMonitoringUsers.Keys.ToArray(); }
        }
        public float TotalTaskScore
        {
            get
            {
                float score = 0;
                foreach (var ts in TaskScores.Values.ToArray())
                    score += ts;
                return score;
            }
        }

        // 正在监控自身的监控用户
        public Dictionary<long, LoginUserInfo> BeMonitoringUsers = new Dictionary<long, LoginUserInfo>();
        public virtual void RemoveMissingFile(string file)
        {
            m_MissingResourceFiles.Remove(file);
            UpdateNode(TaskGroupColumn.项目及完整度);
        }
        public virtual void ResetDefaultTaskScene()
        {
            ViewportIndex = -1;
            if (IsProjectSpecified)
            {
                if (ProjectInstance.TaskInfos.Count == 0)
                    ProjectInstance.SelectedTaskInfo = null;
                else
                {
                    ProjectInstance.SelectedTaskInfo = ProjectInstance.TaskInfos[0];
                    if (ProjectInstance.SelectedTaskInfo.SceneInfos.Count == 0)
                        ProjectInstance.SelectedTaskInfo.SelectedSceneInfo = null;
                    else
                    {
                        ProjectInstance.SelectedTaskInfo.SelectedSceneInfo = ProjectInstance.SelectedTaskInfo.SceneInfos[0];
                        ProjectInstance.SelectedTaskInfo.SelectedSceneInfo.SelectedViewportInfo = null;
                    }
                }
            }
            foreach (TaskGroupColumn col in Enum.GetValues(typeof(TaskGroupColumn)))
            {
                UpdateNode(col);
            }
        }
        public virtual string GetTaskGroupColumnText(TaskGroupColumn col)
        {
            switch (col)
            {
                case TaskGroupColumn.名称:
                    return Name;
                case TaskGroupColumn.项目及完整度:
                    if (ProjectInstance == null)
                        return "";
                    return ProjectInstance.Name;
                case TaskGroupColumn.任务名称:
                    return TaskName;
                case TaskGroupColumn.场景名称:
                    return SceneName;
                case TaskGroupColumn.窗口索引:
                    if (ViewportIndex < 0)
                        return "";
                    return (ViewportIndex + 1).ToString();
            }
            return "";
        }
        public virtual void UpdateNode(TaskGroupColumn col)
        {
            if (Node != null)
                Node.SetValue((int)col, GetTaskGroupColumnText(col));
        }
        public float GetTaskScore(int judgeItemId)
        {
            float score = 0;
            TaskScores.TryGetValue(judgeItemId, out score);
            return score;
        }
    }
}
