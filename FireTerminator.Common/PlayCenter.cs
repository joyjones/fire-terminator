using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireTerminator.Common
{
    public class PlayCenter
    {
        public PlayCenter(ProjectInfo proj)
        {
            ParentProject = proj;
            PlayingViewport = null;
            AutoNextView = true;
            AutoNextScene = true;
            AutoNextTask = false;
        }
        private List<ViewportInfo> m_ViewportsInOrder = new List<ViewportInfo>();
        private ViewportInfo m_FirstViewport = null;
        public ViewportInfo FirstViewport
        {
            get { return m_FirstViewport; }
            set
            {
                m_FirstViewport = value;
                m_ViewportsInOrder.Clear();
                if (m_FirstViewport != null)
                {
                    if (!m_FirstViewport.IsMaximized)
                        m_ViewportsInOrder.Add(m_FirstViewport);
                    else
                        StatisticsOrderViewports(m_FirstViewport);
                }
                if (m_FirstViewport == null && IsPlaying)
                {
                    IsPlaying = false;
                }
            }
        }
        public ProjectInfo ParentProject
        {
            get;
            private set;
        }
        public ViewportInfo PlayingViewport
        {
            get;
            private set;
        }
        public int CurPlayViewportCount
        {
            get { return m_ViewportsInOrder.Count; }
        }
        public int PlayingViewportIndex
        {
            get
            {
                if (PlayingViewport == null)
                    return -1;
                return m_ViewportsInOrder.IndexOf(PlayingViewport);
            }
            set
            {
                if (value < 0 || m_ViewportsInOrder.Count == 0)
                {
                    if (IsPlaying)
                        IsPlaying = false;
                }
                else
                {
                    if (value >= m_ViewportsInOrder.Count)
                        value = m_ViewportsInOrder.Count - 1;
                    if (value != PlayingViewportIndex)
                    {
                        if (PlayingViewport != null)
                            PlayingViewport.IsPlaying = false;
                        PlayingViewport = m_ViewportsInOrder[value];
                        PlayingViewport.CurTimeTick = 0;
                        PlayingViewport.IsMaximized = true;
                        var si = PlayingViewport.ParentSceneInfo;
                        var ti = si.ParentTaskInfo;
                        ParentProject.SelectedTaskInfo = ti;
                        ti.SelectedSceneInfo = si;
                        si.SelectedViewportInfo = PlayingViewport;
                        PlayingViewport.IsPlaying = true;
                    }
                }
            }
        }
        public SceneInfo PlayingScene
        {
            get
            {
                if (PlayingViewport == null)
                    return null;
                return PlayingViewport.ParentSceneInfo;
            }
        }
        public TaskInfo PlayingTask
        {
            get
            {
                if (PlayingScene == null)
                    return null;
                return PlayingScene.ParentTaskInfo;
            }
        }
        public bool AutoNextView
        {
            get;
            set;
        }
        public bool AutoNextScene
        {
            get;
            set;
        }
        public bool AutoNextTask
        {
            get;
            set;
        }
        public bool IsPlaying
        {
            get { return PlayingViewport != null && PlayingViewport.IsPlaying; }
            set
            {
                if (FirstViewport == null && value)
                {
                    if (ParentProject.IsViewportSelected)
                        FirstViewport = ParentProject.SelectedTaskInfo.SelectedSceneInfo.SelectedViewportInfo;
                    else
                        FirstViewport = null;
                    if (FirstViewport == null)
                    {
                        m_ViewportsInOrder.Clear();
                        PlayingViewport = null;
                    }
                }
                if (FirstViewport != null && !value)
                {
                    m_FirstViewport = null;
                    m_ViewportsInOrder.Clear();
                    if (PlayingViewport != null)
                    {
                        PlayingViewport.CurTimeTick = 0;
                        PlayingViewport.IsPlaying = false;
                    }
                    PlayingViewport = null;
                }
                if (FirstViewport != null && value)
                {
                    FirstViewport.CurTimeTick = 0;
                    FirstViewport.IsPlaying = value;
                    if (FirstViewport.IsPlaying)
                    {
                        var si = FirstViewport.ParentSceneInfo;
                        var ti = si.ParentTaskInfo;
                        ParentProject.SelectedTaskInfo = ti;
                        ti.SelectedSceneInfo = si;
                        si.SelectedViewportInfo = FirstViewport;
                    }
                    PlayingViewport = FirstViewport;
                }
            }
        }
        public bool IsPaused
        {
            get
            {
                if (PlayingViewport == null)
                    return false;
                return PlayingViewport.IsPaused;
            }
            set
            {
                if (PlayingViewport != null)
                    PlayingViewport.IsPaused = value;
            }
        }

        private void StatisticsOrderViewports(ViewportInfo vi)
        {
            if (vi == null)
                return;
            if (vi.IsVisible)
                m_ViewportsInOrder.Add(vi);
            var si = vi.ParentSceneInfo;
            if (si == null)
                return;
            var ti = si.ParentTaskInfo;
            if (ti == null)
                return;
            var pi = ti.ParentProjectInfo;
            if (pi == null)
                return;

            int vIndex = vi.ScreenIndex + 1;
            if (vIndex < si.Viewports.Length)
            {
                //if (AutoNextView)
                StatisticsOrderViewports(si.Viewports[vIndex]);
            }
            else// if (AutoNextScene)
            {
                int sIndex = ti.SceneInfos.IndexOf(si) + 1;
                if (sIndex < ti.SceneInfos.Count)
                {
                    si = ti.SceneInfos[sIndex];
                    StatisticsOrderViewports(si.Viewports[0]);
                }
                else// if (AutoNextTask)
                {
                    do
                    {
                        int tIndex = pi.TaskInfos.IndexOf(ti) + 1;
                        if (tIndex >= pi.TaskInfos.Count)
                        {
                            ti = null;
                            break;
                        }
                        ti = pi.TaskInfos[tIndex];
                    } while (ti.SceneInfos.Count == 0);
                    if (ti != null)
                    {
                        si = ti.SceneInfos[0];
                        StatisticsOrderViewports(si.Viewports[0]);
                    }
                }
            }
        }
        public void Update(float elapsedTime)
        {
            if (IsPlaying)
            {
                if (PlayingViewport.IsAnimationEnding)
                {
                    if (!PlayingViewport.IsMaximized)
                    {
                        PlayingViewport.CurTimeTick = 0;
                        PlayingViewport.IsPlaying = false;
                        PlayingViewport.IsPlaying = true;
                    }
                    else if (AutoNextView)
                    {
                        int index = m_ViewportsInOrder.IndexOf(PlayingViewport) + 1;
                        if (index < m_ViewportsInOrder.Count)
                        {
                            var nextView = m_ViewportsInOrder[index];
                            if (nextView.ParentSceneInfo == PlayingViewport.ParentSceneInfo || AutoNextScene)
                            {
                                PlayingViewportIndex = index;
                            }
                        }
                    }
                }
            }
        }
    }
}
