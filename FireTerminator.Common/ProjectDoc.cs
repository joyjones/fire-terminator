using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Audio;
using FireTerminator.Common.Operations;

namespace FireTerminator.Common
{
    public class ProjectDoc
    {
        public static object SyncObj = new object();
        private static ProjectDoc m_Instance = null;
        public static ProjectDoc Instance
        {
            get
            {
                lock (SyncObj)
                {
                    if (m_Instance == null)
                        m_Instance = new ProjectDoc();
                }
                return m_Instance;
            }
        }
        private ProjectDoc()
        {
            foreach (ResourceKind rp in Enum.GetValues(typeof(ResourceKind)))
                ResourceGroups[rp] = new ResourceGroup(rp);
            CurEditUserID = 0;
            IsCooperatingEditMode = false;
            ResolutionRatio = new System.Drawing.Size(100, 100);
            PreviewViewport = new ViewportInfo(null as SceneInfo);
            FModPlayer = new SoundPlayer_FMOD();
            HideSelectionOnPlayingAnimation = false;
            DefaultCubicCurveRibbonColor = Color.OrangeRed;
            DefaultCubicCurveRibbonWidth = 6;
        }
        private Game m_HostGame = null;
        public Game HostGame
        {
            get { return m_HostGame; }
            set
            {
                if (value != null && m_HostGame != value)
                {
                    m_HostGame = value;
                    DefaultFont = m_HostGame.Content.Load<SpriteFont>("depend\\DefaultFont");
                    ElementCaptionSprite = new SpriteBatch(m_HostGame.GraphicsDevice);
                }
            }
        }
        public Options Option
        {
            get;
            set;
        }
        public SpriteFont DefaultFont
        {
            get;
            private set;
        }
        public SpriteBatch ElementCaptionSprite
        {
            get;
            private set;
        }
        public Color DefaultCubicCurveRibbonColor
        {
            get;
            set;
        }
        public float DefaultCubicCurveRibbonWidth
        {
            get;
            set;
        }
        private int m_iSelectedProjectIndex = -1;
        private List<ProjectInfo> m_Projects = new List<ProjectInfo>();
        public Dictionary<ResourceKind, ResourceGroup> ResourceGroups = new Dictionary<ResourceKind, ResourceGroup>();

        public int CurProjectsCount
        {
            get { return m_Projects.Count; }
        }
        public ProjectInfo[] Projects
        {
            get { return m_Projects.ToArray(); }
        }
        public ProjectInfo SelectedProject
        {
            get { return GetProject(m_iSelectedProjectIndex); }
            set
            {
                if (value != null && m_Projects.Contains(value))
                    m_iSelectedProjectIndex = m_Projects.IndexOf(value);
                else
                    m_iSelectedProjectIndex = -1;
            }
        }
        public bool IsSelectedProjectFileOpened
        {
            get { return SelectedProject != null && !String.IsNullOrEmpty(SelectedProject.File); }
        }
        public TransitionGraphics TransGraphics
        {
            get;
            set;
        }
        public SoundPlayer_FMOD FModPlayer
        {
            get;
            private set;
        }
        public long CurEditUserID
        {
            get;
            set;
        }
        public bool HideSelectionOnPlayingAnimation
        {
            get;
            set;
        }
        private System.Drawing.Size m_ResolutionRatio = new System.Drawing.Size(0, 0);
        public System.Drawing.Size ResolutionRatio
        {
            get { return m_ResolutionRatio; }
            set
            {
                if (m_ResolutionRatio != value)
                {
                    m_ResolutionRatio = value;
                    foreach (var proj in m_Projects)
                    {
                        var scenes = (from t in proj.TaskInfos
                                      from s in t.SceneInfos
                                      select s).ToArray();
                        foreach (var s in scenes)
                        {
                            s.OnResolutionChanged(m_ResolutionRatio);
                        }
                    }
                }
            }
        }
        public TaskInfo SelectedTaskInfo
        {
            get
            {
                var proj = SelectedProject;
                if (proj != null)
                    return proj.SelectedTaskInfo;
                return null;
            }
            set
            {
                var proj = SelectedProject;
                if (proj != null)
                    proj.SelectedTaskInfo = value;
            }
        }
        public SceneInfo SelectedSceneInfo
        {
            get
            {
                var task = SelectedTaskInfo;
                if (task != null)
                    return task.SelectedSceneInfo;
                return null;
            }
            set
            {
                if (value != null)
                    SelectedTaskInfo = value.ParentTaskInfo;
                if (SelectedTaskInfo != null)
                    SelectedTaskInfo.SelectedSceneInfo = value;
            }
        }
        public ViewportInfo SelectedViewportInfo
        {
            get
            {
                var scene = SelectedSceneInfo;
                if (scene != null)
                    return scene.SelectedViewportInfo;
                return null;
            }
            set
            {
                if (value != null)
                    SelectedSceneInfo = value.ParentSceneInfo;
                if (SelectedSceneInfo != null)
                    SelectedSceneInfo.SelectedViewportInfo = value;
            }
        }
        public ElementInfo SelectedElementInfo
        {
            get
            {
                var view = SelectedViewportInfo;
                if (view != null)
                    return view.SelectedElementInfo;
                return null;
            }
            set
            {
                if (value != null && !value.CanSelect)
                    return;
                if (value != null)
                    SelectedViewportInfo = value.ParentViewport;
                if (SelectedViewportInfo != null)
                    SelectedViewportInfo.SelectedElementInfo = value;
            }
        }
        public ViewportInfo PreviewViewport
        {
            get;
            protected set;
        }
        public bool IsProjectAnimationPlaying
        {
            get { return (SelectedProject != null && SelectedProject.PlayCenter.IsPlaying); }
            set
            {
                if (SelectedProject != null)
                    SelectedProject.PlayCenter.IsPlaying = value;
            }
        }
        // 客户端合作编辑模式
        public bool IsCooperatingEditMode
        {
            get;
            set;
        }
        public bool IsElementModifyEnabled
        {
            get { return IsCooperatingEditMode || !IsProjectAnimationPlaying; }
        }
        private Dictionary<string, ProjectDesc> m_ProjectDescs = new Dictionary<string, ProjectDesc>();
        public ProjectDesc[] ProjectDescs
        {
            get { return m_ProjectDescs.Values.ToArray(); }
        }

        public ProjectInfo GetProject(int index)
        {
            if (index >= 0 && index < m_Projects.Count)
                return m_Projects[index];
            return null;
        }
        public ProjectInfo GetProject(string filename)
        {
            foreach (var proj in m_Projects)
            {
                if (proj.FileName.ToLower() == filename.ToLower())
                    return proj;
            }
            return null;
        }
        public void LoadProjectsDescriptions()
        {
            m_ProjectDescs.Clear();
            List<string> files = new List<string>();
            var dir = Options.DefaultProjectsRootPath;
            if (Directory.Exists(dir))
                files.AddRange(Directory.GetFiles(dir, "*" + ProjectInfo.DefaultFileExt, SearchOption.AllDirectories));
            foreach (var file in files)
            {
                var pd = new ProjectDesc(file);
                m_ProjectDescs[file.ToLower()] = pd;
            }
        }
        public void ReloadProjectDescription(string file)
        {
            if (File.Exists(file))
            {
                var pd = new ProjectDesc(file);
                m_ProjectDescs[pd.FullFileName.ToLower()] = pd;
            }
        }
        public ProjectDesc GetProjectDescription(string projFile)
        {
            if (!projFile.ToLower().EndsWith(ProjectInfo.DefaultFileExt))
                projFile += ProjectInfo.DefaultFileExt;
            if (projFile.IndexOf(':') < 0)
                projFile = Options.DefaultProjectsRootPath + projFile;
            ProjectDesc desc = null;
            m_ProjectDescs.TryGetValue(projFile.ToLower(), out desc);
            return desc;
        }
        public void CloseProjects()
        {
            foreach (var proj in m_Projects)
                proj.Clear();
            m_Projects.Clear();
            m_iSelectedProjectIndex = -1;
        }
        public void CloseProject(ProjectInfo proj)
        {
            if (proj != null)
            {
                proj.Clear();
                m_Projects.Remove(proj);
                if (m_iSelectedProjectIndex >= m_Projects.Count)
                    m_iSelectedProjectIndex = -1;
            }
        }
        public ProjectInfo CreateProject(bool bAppend)
        {
            if (!bAppend)
            {
                OperationHistory.Instance.Clear();
                CloseProjects();
            }
            var proj = new ProjectInfo("新建项目");
            m_Projects.Add(proj);
            if (m_iSelectedProjectIndex < 0)
                m_iSelectedProjectIndex = m_Projects.Count - 1;
            return proj;
        }
        public ProjectInfo CreateProject(string projFile, bool bAppend)
        {
            if (String.IsNullOrEmpty(projFile))
                return null;
            if (!projFile.EndsWith(ProjectInfo.DefaultFileExt))
                projFile += ProjectInfo.DefaultFileExt;
            if (projFile[1] != ':')
                projFile = Options.DefaultProjectsRootPath + projFile;
            if (!File.Exists(projFile))
                return null;
            var proj = CreateProject(bAppend);
            proj.Load(projFile);
            return proj;
        }
        public ProjectInfo CreateProject(string projFile, bool bAppend, bool bSelect)
        {
            var proj = CreateProject(projFile, bAppend);
            if (proj != null && bSelect)
                SelectedProject = proj;
            return proj;
        }
        public BasicEffect CreateBasicEffect()
        {
            var effect = new BasicEffect(HostGame.GraphicsDevice, null);
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)HostGame.GraphicsDevice.Viewport.Width, (float)HostGame.GraphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;
            return effect;
        }
        public void ReloadResourceFiles()
        {
            CloseProjects();
            foreach (ResourceKind rp in Enum.GetValues(typeof(ResourceKind)))
                ProjectDoc.Instance.ResourceGroups[rp].Reload();
            System.GC.Collect();
        }
        public void ClearResourceFiles()
        {
            CloseProjects();
            foreach (ResourceKind rp in Enum.GetValues(typeof(ResourceKind)))
                ProjectDoc.Instance.ResourceGroups[rp].Clear();
            System.GC.Collect();
        }
        public bool ExportProjectResourceFiles(string projFile, string targetDir, bool prompt)
        {
            if (!System.IO.File.Exists(projFile))
            {
                if (prompt)
                    MessageBox.Show("未能找到项目文件" + projFile + "！");
                return false;
            }
            var desc = GetProjectDescription(projFile);
            if (desc == null)
                return false;
            if (!System.IO.Directory.Exists(targetDir))
                CommonMethods.InheritCreateFolder(targetDir, false);
            targetDir = targetDir.TrimEnd('\\') + "\\";
            CommonMethods.InheritDeleteFolder(targetDir, false);

            foreach (var name in desc.ResourceFiles)
            {
                string targetFile = targetDir + name;
                CommonMethods.InheritCreateFolder(targetFile, true);
                System.IO.File.Copy(Options.UserResourceRootPath + name, targetDir + name, true);
            }
            return true;
        }
        public int CheckResourceFinalizations()
        {
            int count = 0;
            Dictionary<string, ResourceInfo> reses = new Dictionary<string, ResourceInfo>();
            foreach (var prj in m_Projects)
            {
                prj.GetUsingResources(ref reses);
            }
            foreach (var grp in ResourceGroups.Values)
            {
                count += grp.CheckResourceFinalizations(reses);
            }
            return count;
        }
        public delegate void Delegate_SceneInfo(SceneInfo si);
        public event Delegate_SceneInfo AppendedSceneInfo;
        public event Delegate_SceneInfo RemovedSceneInfo;
        public void OnAppendedSceneInfo(SceneInfo si)
        {
            if (AppendedSceneInfo != null)
                AppendedSceneInfo(si);
        }
        public void OnRemovedSceneInfo(SceneInfo si)
        {
            if (RemovedSceneInfo != null)
                RemovedSceneInfo(si);
        }
    }
}
