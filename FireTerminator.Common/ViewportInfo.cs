using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Structures;
using FireTerminator.Common.RenderResources;

namespace FireTerminator.Common
{
    public enum ViewportSwitchMode
    {
        时间切换,
        热键触发,
    }
    // 窗口信息
    public class ViewportInfo : TreeListNodeObject
    {
        public ViewportInfo(SceneInfo scene)
        {
            ParentSceneInfo = scene;
            IsSelected = false;
            TimeLength = 30;
            CurTimeTick = 0;
            IsVisible = true;
            IsMaximized = false;
            IsFullscreenMode = false;
            Name = "";
            Elements.Add(null);
            SwitchMode = ViewportSwitchMode.时间切换;
            SwitchHotKey = System.Windows.Forms.Keys.None;
            IsCombineElementsMode = false;
            ElementsCaptionColor = System.Drawing.Color.White;
            ElementsCaptionScale = 1;
            ElemGroupCollector = new ElementGroupCollector(this);

            for (int i = 0; i < 4; ++i)
                m_vFrameRc[i] = new VertexPositionColor();
        }
        public ViewportInfo(ViewportInfo info)
            : base(info)
        {
            ParentSceneInfo = info.ParentSceneInfo;
            IsSelected = false;
            TimeLength = info.TimeLength;
            CurTimeTick = info.CurTimeTick;
            IsVisible = info.IsVisible;
            IsMaximized = info.IsMaximized;
            IsFullscreenMode = info.IsFullscreenMode;
            Name = ToString();
            SwitchMode = info.SwitchMode;
            SwitchHotKey = info.SwitchHotKey;
            ViewScale = info.ViewScale;
            ElementsCaptionColor = info.ElementsCaptionColor;
            ElementsCaptionScale = info.ElementsCaptionScale;
            ElemGroupCollector = new ElementGroupCollector(this);

            for (int i = 0; i < info.Elements.Count; ++i)
            {
                if (i == 0 && info.Elements[i] == null)
                    Elements.Add(null);
                else
                    AddElement(ElementInfo.CreateElement(info.Elements[i]));
            }

            for (int i = 0; i < 4; ++i)
                m_vFrameRc[i] = info.m_vFrameRc[i];
        }
        private VertexPositionColor[] m_vFrameRc = new VertexPositionColor[4];
        private short[] indices = new short[] { 0, 1, 2, 3, 0 };
        [Category("窗口"), DisplayName("序号")]
        public int ScreenIndex
        {
            get
            {
                if (ParentSceneInfo == null)
                    return -1;
                return Array.IndexOf(ParentSceneInfo.Viewports, this);
            }
        }
        [Category("窗口"), DisplayName("名称")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(false)]
        public SceneInfo ParentSceneInfo
        {
            get;
            internal set;
        }
        [Category("窗口"), DisplayName("动画总时间")]
        public float TimeLength
        {
            get;
            set;
        }
        [Browsable(false)]
        public float CurTimeTick
        {
            get;
            set;
        }
        [Browsable(false)]
        public bool IsAnimationEnding
        {
            get
            {
                if (IsPlaying)
                {
                    if (SwitchMode == ViewportSwitchMode.时间切换 && CurTimeTick >= TimeLength)
                        return true;
                    if (m_bSwitchHotKeyPressed)
                    {
                        m_bSwitchHotKeyPressed = false;
                        return true;
                    }
                }
                return false;
            }
        }
        [Browsable(false)]
        public override bool IsSelected
        {
            get { return base.IsSelected; }
            set
            {
                bool changed = base.IsSelected != value;
                base.IsSelected = value;
                if (changed)
                {
                    if (!IsSelected)
                        IsAnimEditingMode = false;
                    OnResolutionChanged();
                }
            }
        }
        [Browsable(false)]
        public bool IsCombineElementsMode
        {
            get;
            set;
        }
        [Category("窗口"), DisplayName("背景资源")]
        public string BackImageFile
        {
            get
            {
                if (BackImage != null)
                    return BackImage.ResBackgroundImage.FileName;
                return "";
            }
            set
            {
                var res = ProjectDoc.Instance.ResourceGroups[ResourceKind.背景].GetResourceInfo(value);
                if (res != null)
                {
                    RemoveElement(BackImage);
                    AddElement(ElementInfo.CreateElement(res, this, new System.Drawing.PointF(0, 0)));
                }
            }
        }
        [Browsable(false)]
        public ElementInfo_BackgroundImage BackImage
        {
            get
            {
                if (Elements.Count == 0 || Elements[0] == null)
                    return null;
                return Elements[0] as ElementInfo_BackgroundImage;
            }
            set
            {
                if (value != null)
                    AddElement(value);
                else if (Elements.Count > 0)
                    RemoveElement(Elements[0]);
            }
        }
        [Category("窗口"), DisplayName("视口尺寸")]
        public System.Drawing.Size ViewportSize
        {
            get { return new System.Drawing.Size(ViewportPtr.Width, ViewportPtr.Height); }
            set
            {
                ViewportPtr.Width = value.Width;
                ViewportPtr.Height = value.Height;
            }
        }
        [Browsable(false)]
        public System.Drawing.Size BackgroundViewSize
        {
            get
            {
                if (BackImage != null)
                    return new System.Drawing.Size(BackImage.Size.Width, BackImage.Size.Height);
                return new System.Drawing.Size(ViewportPtr.Width, ViewportPtr.Height);
            }
        }
        [Browsable(false)]
        public float ViewportSizeAspect
        {
            get { return (float)ViewportPtr.Width / (float)ViewportPtr.Height; }
        }
        [Category("窗口"), DisplayName("背景位移"), DefaultValue(typeof(System.Drawing.Point), "0,0")]
        public System.Drawing.Point BackImageViewOffset
        {
            get
            {
                if (BackImage == null)
                    return new System.Drawing.Point(0, 0);
                var offset = BackImage.CurViewOffset;
                return new System.Drawing.Point((int)offset.X, (int)offset.Y);
            }
            set
            {
                if (BackImage != null)
                    BackImage.CurViewOffset = value;
            }
        }
        [Category("窗口"), DisplayName("可用"), DefaultValue(true)]
        public bool IsVisible
        {
            get;
            set;
        }
        private bool m_IsMaximized = false;
        [Browsable(false)]
        public bool IsMaximized
        {
            get { return m_IsMaximized; }
            set
            {
                if (m_IsMaximized != value)
                {
                    m_IsMaximized = value;
                    OnResolutionChanged();
                }
            }
        }
        [Browsable(false)]
        public bool IsFullscreenMode
        {
            get;
            set;
        }
        private ElementInfo m_SelectedElementInfo;
        [Browsable(false)]
        public ElementInfo SelectedElementInfo
        {
            get { return m_SelectedElementInfo; }
            set
            {
                if (m_SelectedElementInfo != value || (value != null && !value.IsSelected))
                {
                    if (value != null && (!value.CanSelect || value.ParentViewport != this))
                        return;
                    if (m_SelectedElementInfo != null)
                        m_SelectedElementInfo.IsSelected = false;
                    m_SelectedElementInfo = value;
                    if (m_SelectedElementInfo != null)
                    {
                        //System.Diagnostics.Debug.WriteLine("Cur Element = " + m_SelectedElementInfo.Name);
                        m_SelectedElementInfo.IsSelected = true;
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("Cur Element = CLEAR");
                    }
                }
            }
        }
        private bool m_IsAnimEditingMode = false;
        [Browsable(false)]
        public bool IsAnimEditingMode
        {
            get { return m_IsAnimEditingMode; }
            set
            {
                if (value && SelectedElementInfo == null)
                    value = false;
                if (m_IsAnimEditingMode != value)
                {
                    m_IsAnimEditingMode = value;
                    OnResolutionChanged();
                    ParentSceneInfo.OnViewportAnimEditChanged(this);
                    //if (m_IsAnimEditingMode)
                    //    CurTimeTick = 0;
                }
            }
        }
        private bool m_IsPlaying = false;
        [Browsable(false)]
        public bool IsPlaying
        {
            get { return m_IsPlaying; }
            set
            {
                if (m_IsPlaying != value)
                {
                    m_IsPlaying = value;
                    if (!m_IsPlaying)
                        OnFinishPlaying();
                    OnAnimationPlayingChanged(m_IsPlaying);
                }
            }
        }
        private bool m_IsPaused = false;
        [Browsable(false)]
        public bool IsPaused
        {
            get { return m_IsPaused; }
            set { m_IsPaused = value; }
        }
        [Category("行为"), DisplayName("跳转方式"), DefaultValue(ViewportSwitchMode.时间切换)]
        public ViewportSwitchMode SwitchMode
        {
            get;
            set;
        }
        private bool m_bSwitchHotKeyPressed = false;
        [Category("行为"), DisplayName("跳转热键"), DefaultValue(System.Windows.Forms.Keys.None)]
        public System.Windows.Forms.Keys SwitchHotKey
        {
            get;
            set;
        }
        [Category("窗口"), DisplayName("缩放比例"), DefaultValue(1.0F)]
        public float ViewScale
        {
            get
            {
                if (BackImage == null)
                    return 1;
                return BackImage.CurZoomScale;
            }
            set
            {
                if (BackImage != null)
                    BackImage.CurZoomScale = value;
            }
        }
        [Browsable(false)]
        public ElementGroupCollector ElemGroupCollector
        {
            get;
            protected set;
        }
        [Category("窗口"), DisplayName("元素标题颜色"), DefaultValue(typeof(System.Drawing.Color), "White")]
        public System.Drawing.Color ElementsCaptionColor
        {
            get;
            set;
        }
        [Category("窗口"), DisplayName("元素标题缩放"), DefaultValue(1.0F)]
        public float ElementsCaptionScale
        {
            get;
            set;
        }

        public void Clear()
        {
            SelectedElementInfo = null;
            while (Elements.Count > 1)
                RemoveElement(Elements[1]);
            if (Elements.Count > 0)
                RemoveElement(Elements[0]);
        }
        public XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = doc.CreateElement("Viewport");
            node.SetAttribute("Index", ScreenIndex.ToString());
            node.SetAttribute("Name", Name);
            if (!IsVisible)
                node.SetAttribute("IsVisible", IsVisible.ToString());
            node.SetAttribute("TimeLength", TimeLength.ToString());
            node.SetAttribute("SwitchMode", SwitchMode.ToString());
            node.SetAttribute("SwitchHotKey", SwitchHotKey.ToString());
            if (ElementsCaptionColor != System.Drawing.Color.White)
                node.SetAttribute("ElementsCaptionColor", ElementsCaptionColor.ToArgb().ToString());
            if (ElementsCaptionScale != 1)
                node.SetAttribute("ElementsCaptionScale", ElementsCaptionScale.ToString());
            foreach (var elm in Elements)
            {
                if (elm == null)
                    continue;
                if (elm.Resource != null && elm.Resource.FullFilePath == ResourceInfo_BackgroundImage.DefaultBackgroundImageFile)
                    continue;
                var enode = elm.GenerateXmlElement(doc);
                node.AppendChild(enode);
            }
            var gnode = ElemGroupCollector.GenerateXmlElement(doc);
            if (gnode != null)
                node.AppendChild(gnode);
            return node;
        }
        public void LoadFromXmlElement(XmlElement root)
        {
            OnResolutionChanged();
            var val = root.GetAttribute("Name");
            if (!String.IsNullOrEmpty(val))
                Name = val;
            val = root.GetAttribute("IsVisible");
            if (!String.IsNullOrEmpty(val))
                IsVisible = bool.Parse(val);
            TimeLength = Convert.ToSingle(root.GetAttribute("TimeLength"));
            val = root.GetAttribute("SwitchMode");
            if (!String.IsNullOrEmpty(val))
                SwitchMode = (ViewportSwitchMode)Enum.Parse(typeof(ViewportSwitchMode), val);
            else
                SwitchMode = ViewportSwitchMode.时间切换;
            val = root.GetAttribute("SwitchHotKey");
            if (!String.IsNullOrEmpty(val))
                SwitchHotKey = (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), val);
            else
                SwitchHotKey = System.Windows.Forms.Keys.None;
            val = root.GetAttribute("ElementsCaptionColor"); int dwClr;
            if (!String.IsNullOrEmpty(val) && Int32.TryParse(val, out dwClr))
                ElementsCaptionColor = System.Drawing.Color.FromArgb(dwClr);
            val = root.GetAttribute("ElementsCaptionScale");
            if (!String.IsNullOrEmpty(val))
                ElementsCaptionScale = float.Parse(val);
            foreach (XmlElement node in root.GetElementsByTagName("Element"))
            {
                if (node.ParentNode == root)
                {
                    var kind = (ResourceKind)Enum.Parse(typeof(ResourceKind), node.GetAttribute("Kind"));
                    var file = node.GetAttribute("ResourceFile");
                    var res = ProjectDoc.Instance.ResourceGroups[kind].GetResourceInfo(file);
                    if (res != null)
                    {
                        var e = ElementInfo.CreateElement(res, this, new System.Drawing.PointF(0, 0));
                        AddElement(e);
                        e.LoadFromXmlElement(node);
                    }
                }
            }
            ElemGroupCollector.LoadFromParentXmlElement(root);
        }
        public bool AddElement(ElementInfo elm)
        {
            if (Elements.Contains(elm))
                return false;
            elm.m_ParentViewport = this;
            if (Elements.Count == 0 || !(elm is ElementInfo_BackgroundImage))
                Elements.Add(elm);
            else
                Elements[0] = elm;
            if (elm.Resource != null && !elm.Resource.IsLoaded)
                elm.Resource.Load();
            elm.OnAfterAdded();
            if (elm == Elements[0])
            {
                float zero = 0;
                elm.Update(0, ref zero);
                foreach (var e in Elements)
                {
                    if (e != null && e != elm)
                        e.OnViewportBackImageChanged(elm as ElementInfo_BackgroundImage, true);
                }
            }
            else if (BackImage != null)
            {
                elm.OnViewportBackImageChanged(BackImage, false);
            }
            ParentSceneInfo.OnViewportElementAdded(elm);
            return true;
        }
        public ElementInfo DuplicateElement(ElementInfo elm, System.Drawing.Point? pos)
        {
            var newElm = ElementInfo.CreateElement(elm);
            if (AddElement(newElm))
            {
                if (!pos.HasValue)
                {
                    var p = elm.Location;
                    pos = new System.Drawing.Point(p.X, p.Y);
                }
                newElm.Location = pos.Value;
            }
            return newElm;
        }
        public bool RemoveElement(ElementInfo elm)
        {
            if (elm != null && elm.ParentViewport == this)// && !elm.IsLocked)
            {
                bool bIsBack = (Elements.Count > 0 && elm == Elements[0]);
                elm.OnBeforeRemove();
                if (Elements.Remove(elm))
                {
                    if (SelectedElementInfo == elm)
                        SelectedElementInfo = null;
                    ParentSceneInfo.OnViewportElementRemoved(elm);
                    if (bIsBack)
                    {
                        Elements.Insert(0, null);
                        foreach (var e in Elements)
                        {
                            if (e != null && e != elm)
                                e.OnViewportBackImageChanged(null, true);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public bool RemoveElement(Guid guid)
        {
            foreach (var elm in Elements)
            {
                if (elm != null && elm.GUID == guid)
                    return RemoveElement(elm);
            }
            return false;
        }
        public void RemoveAllUnlockElements()
        {
            var elms = (from e in Elements
                        where e != null && !e.IsLocked
                        select e).ToArray();
            foreach (var e in elms)
            {
                RemoveElement(e);
            }
        }
        public void RemoveAllCreatorElements(long creatorId)
        {
            var elms = (from e in Elements
                        where e != null && e.CreatorId == creatorId
                        select e).ToArray();
            foreach (var e in elms)
            {
                RemoveElement(e);
            }
        }
        public void RemoveAllUserElements()
        {
            var elms = (from e in Elements
                        where e != null && e.CreatorId > 0
                        select e).ToArray();
            foreach (var e in elms)
            {
                RemoveElement(e);
            }
        }
        public void LockAllElements()
        {
            foreach (var e in Elements)
            {
                if (e != null)
                    e.IsLocked = true;
            }
        }
        public bool BringElementUp(ElementInfo elm)
        {
            int index = Elements.IndexOf(elm);
            if (index <= 0 || index >= Elements.Count - 1)
                return false;
            Elements.Remove(elm);
            Elements.Insert(index + 1, elm);
            return true;
        }
        public bool BringElementDown(ElementInfo elm)
        {
            int index = Elements.IndexOf(elm);
            if (index <= 1)
                return false;
            Elements.Remove(elm);
            Elements.Insert(index - 1, elm);
            return true;
        }
        public void Update(float elapsedTime)
        {
            if (IsPlaying)
            {
                if (!IsPaused)
                    CurTimeTick += elapsedTime;
                if (CurTimeTick > TimeLength)
                    CurTimeTick = TimeLength;
            }
            foreach (var e in Elements.ToArray())
            {
                if (e != null)
                {
                    float curTime = CurTimeTick;
                    e.Update(elapsedTime, ref curTime);
                }
            }
        }
        public void DrawFrame(BasicEffect effect)
        {
            if (IsSelected && IsVisible)
            {
                effect.GraphicsDevice.VertexDeclaration = new VertexDeclaration(effect.GraphicsDevice, VertexPositionColor.VertexElements);
                effect.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, m_vFrameRc, 0, 4, indices, 0, 4);
            }
        }
        private Dictionary<Guid, ElementInfo> CrashedElements = new Dictionary<Guid, ElementInfo>();
        public void DrawElements()
        {
            if (IsVisible)
            {
                ProjectDoc.Instance.HostGame.GraphicsDevice.Viewport = ViewportPtr;
                foreach (var elm in Elements.ToArray())
                {
                    if (elm != null && !CrashedElements.ContainsKey(elm.GUID))
                    {
                        try
                        {
                            elm.Draw();
                        }
                        catch (System.Exception ex)
                        {
                            AppLogger.Write("错误：元素" + elm.GUID.ToString() + "绘制失败！\r\n" + ex.Message);
                            CrashedElements[elm.GUID] = elm;
                        }
                    }
                }
                if (IsAnimEditingMode && SelectedElementInfo != null)
                {
                    SelectedElementInfo.Draw();
                }
            }
        }
        public System.Drawing.PointF GetLocationRate(bool baseOnImage, System.Drawing.PointF pos, bool relativePos)
        {
            if (baseOnImage && (BackImage == null || BackImage.Size.Width == 0 || BackImage.Size.Height == 0))
                baseOnImage = false;
            if (!baseOnImage)
            {
                if (ViewportSize.Width == 0 || ViewportSize.Height == 0)
                    pos = new System.Drawing.PointF(0, 0);
                else
                {
                    pos.X /= ViewportSize.Width;
                    pos.Y /= ViewportSize.Height;
                }
            }
            else
            {
                if (!relativePos)
                {
                    pos.X -= BackImage.Location.X;
                    pos.Y -= BackImage.Location.Y;
                }
                pos.X /= BackImage.Size.Width;
                pos.Y /= BackImage.Size.Height;
            }
            return pos;
        }
        public float GetLengthRate(bool baseOnImage, float length)
        {
            if (baseOnImage && (BackImage == null || BackImage.Size.Width == 0 || BackImage.Size.Height == 0))
                baseOnImage = false;
            if (baseOnImage)
                return length / BackImage.Size.Width;
            if (ViewportSize.Width == 0 || ViewportSize.Height == 0)
                return 0;
            return length / ViewportSize.Width;
        }
        public System.Drawing.PointF GetLocationRate(bool baseOnImage, System.Drawing.PointF pos)
        {
            return GetLocationRate(baseOnImage, pos, false);
        }
        public System.Drawing.SizeF GetSizeRate(bool baseOnImage, System.Drawing.SizeF size)
        {
            if (baseOnImage && BackImage == null)
                baseOnImage = false;
            var bkSize = baseOnImage ? BackImage.Size : ViewportSize;
            size.Width /= bkSize.Width;
            size.Height /= bkSize.Height;
            return size;
        }
        public System.Drawing.PointF GetRateLocation(bool baseOnImage, System.Drawing.PointF posRate)
        {
            if (baseOnImage && BackImage == null)
                baseOnImage = false;
            if (!baseOnImage)
            {
                posRate.X *= ViewportSize.Width;
                posRate.Y *= ViewportSize.Height;
            }
            else
            {
                posRate.X *= BackImage.Size.Width;
                posRate.Y *= BackImage.Size.Height;
                posRate.X += BackImage.Location.X;
                posRate.Y += BackImage.Location.Y;
            }
            return posRate;
        }
        public System.Drawing.SizeF GetRateSize(bool baseOnImage, System.Drawing.SizeF sizeRate)
        {
            if (baseOnImage && BackImage == null)
                baseOnImage = false;
            var bkSize = baseOnImage ? BackImage.Size : ViewportSize;
            sizeRate.Width *= bkSize.Width;
            sizeRate.Height *= bkSize.Height;
            return sizeRate;
        }
        public bool ContainPoint(System.Drawing.Point pt)
        {
            if (IsFullscreenMode)
                return true;
            if (!IsVisible)
                return false;
            if (pt.X < ViewportPtr.X)
                return false;
            if (pt.X > ViewportPtr.X + ViewportPtr.Width)
                return false;
            if (pt.Y < ViewportPtr.Y)
                return false;
            if (pt.Y > ViewportPtr.Y + ViewportPtr.Height)
                return false;
            return true;
        }
        public System.Drawing.Point PointToClient(bool baseOnImage, System.Drawing.Point pt)
        {
            pt.X -= ViewportPtr.X;
            pt.Y -= ViewportPtr.Y;
            if (baseOnImage && BackImage != null)
            {
                pt.X -= BackImage.Location.X;
                pt.Y -= BackImage.Location.Y;
            }
            return pt;
        }
        public ElementInfo GetElementInfo(Guid guid)
        {
            foreach (var elm in Elements)
            {
                if (elm != null && elm.GUID == guid)
                    return elm;
            }
            return null;
        }
        public ElementInfo GetElementInfo(System.Drawing.Point pt, ref BodyOperationPart part)
        {
            for (int i = Elements.Count - 1; i > 0; --i)
            {
                var elm = Elements[i];
                if (!elm.CanSelect)
                    continue;
                part = elm.GetPointBodyOprPart(pt);
                if (part != BodyOperationPart.Nothing)
                    return elm;
            }
            part = BodyOperationPart.Nothing;
            return null;
        }
        public void AddRenderingCreatorID(long id)
        {
            if (!RenderingCreatorIDs.Contains(id))
                RenderingCreatorIDs.Add(id);
        }
        public void SetRenderingCreatorIDs(long[] ids)
        {
            RenderingCreatorIDs.Clear();
            RenderingCreatorIDs.AddRange(ids);
        }
        public void RemoveRenderingCreatorID(long id)
        {
            RenderingCreatorIDs.Remove(id);
        }
        public void ClearRenderingCreatorID()
        {
            RenderingCreatorIDs.Clear();
        }
        public bool IsCreatorIDInRendering(long id)
        {
            if (id <= 0 || id == LoginUserInfo.SystemUserID || RenderingCreatorIDs.Count == 0)
                return true;
            return RenderingCreatorIDs.Contains(id);
        }
        public void OnResolutionChanged()
        {
            //if (ProjectDoc.Instance.SelectedProject == null)
            //    return;
            System.Drawing.Size size = ProjectDoc.Instance.ResolutionRatio;
            int idx = IsMaximized ? 0 : ScreenIndex;
            int w = IsMaximized ? size.Width : size.Width / 2;
            int h = IsMaximized ? size.Height : size.Height / 2;
            int vx = 0, vy = 0, vw = 0, vh = 0;
            int fx1 = 0, fy1 = 0, fx2 = 0, fy2 = 0;
            switch (idx)
            {
                case 0:
                    vx = 1; vy = 1;
                    vw = w - 2; vh = h - 2;
                    fx1 = 0; fx2 = w - 1;
                    fy1 = 0; fy2 = h - 1;
                    break;
                case 1:
                    vx = w + 2; vy = 1;
                    vw = w - 2; vh = h - 2;
                    fx1 = w + 1; fx2 = 2 * w - 1;
                    fy1 = 0; fy2 = h - 1;
                    break;
                case 2:
                    vx = 1; vy = h + 1;
                    vw = w - 2; vh = h - 2;
                    fx1 = 0; fx2 = w - 1;
                    fy1 = h + 1; fy2 = 2 * h - 1;
                    break;
                case 3:
                    vx = w + 2; vy = h + 2;
                    vw = w - 2; vh = h - 2;
                    fx1 = w + 1; fx2 = 2 * w - 1;
                    fy1 = h + 1; fy2 = 2 * h - 1;
                    break;
            }
            ViewportPtr.X = vx;
            ViewportPtr.Y = vy;
            ViewportPtr.Width = vw;
            ViewportPtr.Height = vh;
            Color clr;
            if (IsAnimEditingMode)
                clr = CommonMethods.ConvertColor(ProjectDoc.Instance.Option.ViewportFrameSelectedAnimModeColor);
            else if (SelectedElementInfo != null && SelectedElementInfo.IsInnerEditingMode)
                clr = Color.Blue;
            else
                clr = CommonMethods.ConvertColor(ProjectDoc.Instance.Option.ViewportFrameSelectedColor);
            m_vFrameRc[0].Position = new Vector3(fx1, fy1, 0);
            m_vFrameRc[0].Color = clr;
            m_vFrameRc[1].Position = new Vector3(fx2, fy1, 0);
            m_vFrameRc[1].Color = clr;
            m_vFrameRc[2].Position = new Vector3(fx2, fy2, 0);
            m_vFrameRc[2].Color = clr;
            m_vFrameRc[3].Position = new Vector3(fx1, fy2, 0);
            m_vFrameRc[3].Color = clr;
            foreach (var elm in Elements)
            {
                if (elm != null)
                    elm.OnViewportSizeChanged();
            }
            if (ParentSceneInfo != null && BackImage == null)
            {
                var res = new ResourceInfo_BackgroundImage(null, ResourceInfo_BackgroundImage.DefaultBackgroundImageFile);
                res.Kind = ResourceKind.背景;
                AddElement(new ElementInfo_BackgroundImage(res, this, false, new System.Drawing.PointF(0, 0)));
            }
        }
        public void OnKeyDown(System.Windows.Forms.Keys key)
        {
            if (!m_bSwitchHotKeyPressed &&
                ProjectDoc.Instance.IsProjectAnimationPlaying &&
                SwitchMode == ViewportSwitchMode.热键触发 && 
                SwitchHotKey != System.Windows.Forms.Keys.None &&
                key == SwitchHotKey)
            {
                m_bSwitchHotKeyPressed = true;
            }
            else
            {
                if (SelectedElementInfo != null && ProjectDoc.Instance.IsElementModifyEnabled &&
                    (key == System.Windows.Forms.Keys.Left ||
                    key == System.Windows.Forms.Keys.Right ||
                    key == System.Windows.Forms.Keys.Up ||
                    key == System.Windows.Forms.Keys.Down))
                {
                    var pos = SelectedElementInfo.Location;
                    switch (key)
                    {
                        case System.Windows.Forms.Keys.Left:
                            pos.X -= 1; break;
                        case System.Windows.Forms.Keys.Right:
                            pos.X += 1; break;
                        case System.Windows.Forms.Keys.Up:
                            pos.Y -= 1; break;
                        case System.Windows.Forms.Keys.Down:
                            pos.Y += 1; break;
                    }
                    FireTerminator.Common.Operations.OperationHistory.Instance.
                        CommitOperation(new FireTerminator.Common.Operations.Operation_Element_ChangeLocation(SelectedElementInfo, pos));
                    return;
                }
                foreach (var elm in Elements)
                {
                    if (elm != null)
                        elm.OnKeyDown(key);
                }
            }
        }
        public void OnFinishPlaying()
        {
            foreach (var elm in Elements)
            {
                if (elm != null)
                    elm.OnViewportFinishPlaying();
            }
        }
        public void OnAnimationPlayingChanged(bool bPlaying)
        {
            foreach (var elm in Elements)
            {
                if (elm != null)
                    elm.OnAnimationPlayingChanged(bPlaying);
            }
        }
        public override string ToString()
        {
            if (!String.IsNullOrEmpty(Name))
                return Name;
            return String.Format("窗口{0}", ScreenIndex + 1);
        }

        public Viewport ViewportPtr = new Viewport();
        public List<ElementInfo> Elements = new List<ElementInfo>();
        public List<long> RenderingCreatorIDs = new List<long>();
    }
}
