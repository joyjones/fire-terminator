using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using FireTerminator.Common.Elements;

namespace FireTerminator.Common
{
    // 场景信息
    public class SceneInfo : TreeListNodeObject
    {
        private void OnInit()
        {
            for (int i = 0; i < m_vViewCutLinePts.Length; ++i)
                m_vViewCutLinePts[i] = new VertexPositionColor();
            OnResolutionChanged(ProjectDoc.Instance.ResolutionRatio);
            ProjectDoc.Instance.OnAppendedSceneInfo(this);
        }
        public SceneInfo(TaskInfo task, string name)
        {
            ParentTaskInfo = task;
            Name = name;
            for (int i = 0; i < 4; ++i)
                Viewports[i] = new ViewportInfo(this);
            OnInit();
        }
        public SceneInfo(TaskInfo task, XmlElement root)
        {
            ParentTaskInfo = task;
            Name = root.GetAttribute("Name");
            for (int i = 0; i < 4; ++i)
            {
                var lst = root.GetElementsByTagName("Viewport");
                Viewports[i] = new ViewportInfo(this);
                Viewports[i].LoadFromXmlElement(lst[i] as XmlElement);
            }
            OnInit();
        }
        public SceneInfo(SceneInfo sinfo)
        {
            ParentTaskInfo = null;
            Name = sinfo.Name;
            for (int i = 0; i < 4; ++i)
            {
                ViewportInfo vi = new ViewportInfo(sinfo.Viewports[i]);
                vi.ParentSceneInfo = this;
                Viewports[vi.ScreenIndex] = vi;
            }
            for (int i = 0; i < m_vViewCutLinePts.Length; ++i)
                m_vViewCutLinePts[i] = sinfo.m_vViewCutLinePts[i];
            OnResolutionChanged(ProjectDoc.Instance.ResolutionRatio);
        }
        [Category("场景"), DisplayName("名称")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(false)]
        public TaskInfo ParentTaskInfo
        {
            get;
            internal set;
        }
        [Browsable(false)]
        public ElementInfo[] AllElements
        {
            get
            {
                return (from v in Viewports
                        from e in v.Elements
                        select e).ToArray();
            }
        }
        [Browsable(false)]
        public ElementInfo SelectedViewportElementInfo
        {
            get
            {
                if (SelectedViewportInfo != null)
                    return SelectedViewportInfo.SelectedElementInfo;
                return null;
            }
        }
        private ViewportInfo m_SelectedViewportInfo;
        [Browsable(false)]
        public ViewportInfo SelectedViewportInfo
        {
            get { return m_SelectedViewportInfo; }
            set
            {
                if (m_SelectedViewportInfo != value)
                {
                    if (value != null && value.ParentSceneInfo != this)
                        return;
                    bool maxed = IsSelectedViewportMaximized;
                    if (maxed)
                        IsSelectedViewportMaximized = false;
                    if (m_SelectedViewportInfo != null)
                        m_SelectedViewportInfo.IsSelected = false;
                    m_SelectedViewportInfo = value;
                    if (m_SelectedViewportInfo != null)
                    {
                        //System.Diagnostics.Debug.WriteLine("Cur Viewport = " + m_SelectedViewportInfo.ScreenIndex);
                        m_SelectedViewportInfo.IsSelected = true;
                        if (maxed)
                            IsSelectedViewportMaximized = true;
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("Cur Viewport = CLEAR");
                    }
                }
            }
        }
        [Browsable(false)]
        public bool IsSelectedViewportMaximized
        {
            get
            {
                if (SelectedViewportInfo == null)
                    return false;
                return SelectedViewportInfo.IsMaximized;
            }
            set
            {
                if (SelectedViewportInfo != null)
                {
                    if (value)
                    {
                        foreach (var vi in Viewports)
                        {
                            if (vi != SelectedViewportInfo)
                                vi.IsMaximized = false;
                        }
                    }
                    SelectedViewportInfo.IsMaximized = value;
                }
            }
        }
        [Browsable(false)]
        public ViewportInfo[] RenderingViewports
        {
            get
            {
                if (IsSelectedViewportMaximized)
                    return new ViewportInfo[] { SelectedViewportInfo };
                return Viewports;
            }
        }
        [Browsable(false)]
        public bool IsAnimationEnding
        {
            get
            {
                if (!IsSelectedViewportMaximized)
                    return false;
                if (SelectedViewportInfo == null || !SelectedViewportInfo.IsVisible)
                    return true;
                return SelectedViewportInfo.ScreenIndex == 3 && SelectedViewportInfo.IsAnimationEnding;
            }
        }
        private VertexDeclaration m_VertexDeclaration = null;

        public void Clear()
        {
            SelectedViewportInfo = null;
            for (int i = 0; i < 4; ++i)
            {
                if (Viewports[i] != null)
                {
                    Viewports[i].Clear();
                    Viewports[i] = null;
                }
            }
            ProjectDoc.Instance.OnRemovedSceneInfo(this);
        }
        public XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = doc.CreateElement("Scene");
            node.SetAttribute("Name", Name);
            foreach (var vi in Viewports)
            {
                var subnode = vi.GenerateXmlElement(doc);
                node.AppendChild(subnode);
            }
            return node;
        }
        public void Update(float elapsedTime)
        {
            if (IsSelectedViewportMaximized)
                SelectedViewportInfo.Update(elapsedTime);
            else
            {
                foreach (var vi in Viewports)
                {
                    vi.Update(elapsedTime);
                }
            }

            var elm = SelectedViewportElementInfo;
            if (elm == null)
                m_vSelectedFramePts.Clear();
            else
                elm.GenerateSelectionFrameBuffer(ref m_vSelectedFramePts);
        }
        public void Draw(GraphicsDevice device)
        {
            ConfirmUsingEffect(device);
            var srcViewport = device.Viewport;
            foreach (var vi in RenderingViewports)
            {
                vi.DrawElements();
            }
            UsingEffect.Begin();
            foreach (EffectPass pass in UsingEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                UsingEffect.GraphicsDevice.Viewport = srcViewport;
                if (m_VertexDeclaration == null || m_VertexDeclaration.GraphicsDevice != device)
                    m_VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
                UsingEffect.GraphicsDevice.VertexDeclaration = m_VertexDeclaration;
                if (!IsSelectedViewportMaximized)
                    UsingEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, m_vViewCutLinePts, 0, 2);
                if (m_vSelectedFramePts.Count > 0)
                {
                    UsingEffect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, m_vSelectedFramePts.ToArray(), 0, 8, m_SelectedFrameIndices, 0, 9);
                }
                foreach (var vi in RenderingViewports)
                {
                    vi.DrawFrame(UsingEffect);
                }
                pass.End();
            }
            UsingEffect.End();
            device.Viewport = srcViewport;
        }
        public ViewportInfo GetViewportInfoByPosition(System.Drawing.Point pt)
        {
            foreach (var vi in RenderingViewports)
            {
                if (vi.ContainPoint(pt))
                    return vi;
            }
            return null;
        }
        public void OnResolutionChanged(System.Drawing.Size size)
        {
            var clr = CommonMethods.ConvertColor(ProjectDoc.Instance.Option.ViewportFrameColor);
            m_vViewCutLinePts[0].Position = new Vector3(0, size.Height / 2, 0);
            m_vViewCutLinePts[0].Color = clr;
            m_vViewCutLinePts[1].Position = new Vector3(size.Width, size.Height / 2, 0);
            m_vViewCutLinePts[1].Color = clr;
            m_vViewCutLinePts[2].Position = new Vector3(size.Width / 2, 0, 0);
            m_vViewCutLinePts[2].Color = clr;
            m_vViewCutLinePts[3].Position = new Vector3(size.Width / 2, size.Height, 0);
            m_vViewCutLinePts[3].Color = clr;
            ConfirmUsingEffect(null);
            foreach (var vi in Viewports)
            {
                vi.OnResolutionChanged();
            }
        }
        public void OnViewportScaleChanged(float scale)
        {
        }
        public void OnAnimationPlayingChanged(bool bPlaying)
        {
            foreach (var vi in Viewports)
            {
                vi.OnAnimationPlayingChanged(bPlaying);
            }
        }
        private void ConfirmUsingEffect(GraphicsDevice device)
        {
            if (UsingEffect == null && device != null)
            {
                UsingEffect = new BasicEffect(device, null);
                UsingEffect.World = Matrix.Identity;
                UsingEffect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
                UsingEffect.VertexColorEnabled = true;
            }
            if (UsingEffect != null)
            {
                var viewport = UsingEffect.GraphicsDevice.Viewport;
                UsingEffect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)viewport.Width, (float)viewport.Height, 0, 1.0f, 1000.0f);
            }
        }
        public void SwapViewport(int index1, int index2)
        {
            if (index1 == index2 || index1 < 0 || index2 < 0 || index1 >= Viewports.Length || index2 >= Viewports.Length)
                return;
            var v = Viewports[index1];
            Viewports[index1] = Viewports[index2];
            Viewports[index2] = v;
            Viewports[index1].OnResolutionChanged();
            Viewports[index2].OnResolutionChanged();
        }
        private VertexPositionColor[] m_vViewCutLinePts = new VertexPositionColor[4];
        private List<VertexPositionColor> m_vSelectedFramePts = new List<VertexPositionColor>();
        private short[] m_SelectedFrameIndices = new short[] { 0, 1, 2, 3, 0, 4, 5, 6, 7, 4 };
        public delegate void Delegate_OnViewportElementChanged(ElementInfo elm);
        public delegate void Delegate_OnViewportChanged(ViewportInfo vi);
        public delegate void Delegate_OnViewportElementPropertyChanged(ElementInfo elm, string attr, object oldValue, object newValue);
        public delegate void Delegate_OnViewportElementTransitionChanged(ElementInfo elm, TransitionKind kind);
        public event Delegate_OnViewportElementChanged ViewportElementAdded;
        public event Delegate_OnViewportElementChanged ViewportElementRemoved;
        public event Delegate_OnViewportElementChanged ViewportElementInnerEditModeChanged;
        public event Delegate_OnViewportElementTransitionChanged ViewportElementTransitionChanged;
        public event Delegate_OnViewportChanged ViewportAnimEditChanged;
        internal void OnViewportElementAdded(ElementInfo elm)
        {
            if (ViewportElementAdded != null && !ParentTaskInfo.ParentProjectInfo.IsInTransaction)
                ViewportElementAdded(elm);
        }
        internal void OnViewportElementRemoved(ElementInfo elm)
        {
            if (ViewportElementRemoved != null && !ParentTaskInfo.ParentProjectInfo.IsInTransaction)
                ViewportElementRemoved(elm);
        }
        internal void OnViewportElementInnerEditModeChanged(ElementInfo elm)
        {
            elm.ParentViewport.OnResolutionChanged();
            if (ViewportElementInnerEditModeChanged != null)
                ViewportElementInnerEditModeChanged(elm);
        }
        public void OnViewportAnimEditChanged(ViewportInfo vi)
        {
            if (ViewportAnimEditChanged != null)
                ViewportAnimEditChanged(vi);
        }
        public void OnViewportElementTransitionChanged(ElementInfo elm, TransitionKind kind)
        {
            if (ViewportElementTransitionChanged != null)
                ViewportElementTransitionChanged(elm, kind);
        }
        public void OnKeyDown(System.Windows.Forms.Keys key)
        {
            if (m_SelectedViewportInfo != null)
                m_SelectedViewportInfo.OnKeyDown(key);
        }
        public bool Remove()
        {
            if (ParentTaskInfo != null)
            {
                if (ParentTaskInfo.RemoveScene(this))
                {
                    ProjectDoc.Instance.OnRemovedSceneInfo(this);
                    return true;
                }
            }
            return true;
        }
        public override string ToString()
        {
            return Name;
        }
        public BasicEffect UsingEffect
        {
            get;
            private set;
        }
        public ViewportInfo[] Viewports = new ViewportInfo[4];
    }
}
