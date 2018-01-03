using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Text.RegularExpressions;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.Audio;
using FireTerminator.Common.Operations;

namespace FireTerminator.Common.Elements
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract partial class ElementInfo : TreeListNodeObject
    {
        static ElementInfo()
        {
            ElementTypes[ResourceKind.背景] = typeof(ElementInfo_BackgroundImage);
            ElementTypes[ResourceKind.图像] = typeof(ElementInfo_StaticImage);
            ElementTypes[ResourceKind.效果] = typeof(ElementInfo_AnimImage);
            ElementTypes[ResourceKind.视频] = typeof(ElementInfo_Video);
            ElementTypes[ResourceKind.音频] = typeof(ElementInfo_Audio);
            ElementTypes[ResourceKind.水带] = typeof(ElementInfo_Waterbag);
            ElementTypes[ResourceKind.遮罩] = typeof(ElementInfo_Mask);
            ElementTypes[ResourceKind.文本] = typeof(ElementInfo_TipText);
        }
        public ElementInfo(ElementInfo e)
            : base(e)
        {
            IsDuplicatedObject = true;
            GUID = Guid.NewGuid();
            CreatorId = e.CreatorId;
            Resource = e.Resource;
            if (Resource != null)
                Resource.Load();
            UsingFont = e.UsingFont;
            UsingEffect = ProjectDoc.Instance.CreateBasicEffect();
            Name = e.Name;
            Caption = e.Caption;
            ShowCaption = e.ShowCaption;
            m_ParentViewport = e.ParentViewport;
            ScaleProportionOnBackImage = e.ScaleProportionOnBackImage;
            IsPreviewElement = e.IsPreviewElement;
            Barycenter = e.Barycenter;
            BlendColor = e.BlendColor;
            IsTextureHoriFlipped = e.IsTextureHoriFlipped;
            IsTextureVertFlipped = e.IsTextureVertFlipped;
            IsBlendingDisabled = e.IsBlendingDisabled;
            IsHidden = e.IsHidden;
            IsLocked = e.IsLocked;
            SoundFile = e.SoundFile;
            SpeedRate = 1;
            m_BaseTrans.Copy(e.BaseTrans);
            m_ManualScale = e.ManualScale;
            BeginMethod = e.BeginMethod;
            BeginHotKey = e.BeginHotKey;
            FinishMethod = e.FinishMethod;
            FinishHotKey = e.FinishHotKey;
            SoundDelayTime = e.SoundDelayTime;
            IsGroupLeader = e.IsGroupLeader;
            foreach (TransitionKind tk in Enum.GetValues(typeof(TransitionKind)))
            {
                if (tk == TransitionKind.Unknown)
                    continue;
                m_Transitions[tk] = new List<ElementTransform>();
                foreach (var trans in e.GetTransitions(tk))
                {
                    var t = System.Activator.CreateInstance(ElementTransform.TransTypes[tk], this) as ElementTransform;
                    t.CopyFrom(trans);
                    AddTransition(t);
                }
                m_TransitionEnables[tk] = e.IsTransitionBlendingEnabled(tk);
            }

            float zero = 0;
            Update(0, ref zero);
        }
        public ElementInfo(ResourceInfo res, ViewportInfo vi, bool isPreview, System.Drawing.PointF pos)
        {
            IsDuplicatedObject = false;
            GUID = Guid.NewGuid();
            CreatorId = 0;
            Resource = res;
            if (Resource != null)
            {
                Resource.Load();
                Name = Path.GetFileNameWithoutExtension(Resource.FileName);
            }
            m_ParentViewport = vi;
            Caption = "";
            ShowCaption = true;
            ScaleProportionOnBackImage = 0;
            IsPreviewElement = isPreview;
            if (IsPreviewElement)
                Barycenter = new System.Drawing.PointF(0, 0);
            else
                Barycenter = new System.Drawing.PointF(0.5F, 0.5F);
            BlendColor = System.Drawing.Color.White;
            IsTextureHoriFlipped = false;
            IsTextureVertFlipped = false;
            IsBlendingDisabled = false;
            IsHidden = false;
            IsLocked = false;
            SpeedRate = 1;
            UsingFont = ProjectDoc.Instance.DefaultFont;
            UsingEffect = ProjectDoc.Instance.CreateBasicEffect();
            BeginMethod = AnimBurstType.时间同步;
            BeginHotKey = System.Windows.Forms.Keys.None;
            FinishMethod = AnimBurstType.时间同步;
            FinishHotKey = System.Windows.Forms.Keys.None;
            SoundDelayTime = 0;

            m_BaseTrans.Alpha = 1;
            m_BaseTrans.RateLocation = ParentViewport.GetLocationRate(IsBasedOnBackImageElement, pos);
            if (Resource != null)
                m_BaseTrans.RateSize = ParentViewport.GetSizeRate(IsBasedOnBackImageElement, res.ImageSize);
            foreach (TransitionKind tk in Enum.GetValues(typeof(TransitionKind)))
            {
                if (tk == TransitionKind.Unknown)
                    continue;
                m_Transitions[tk] = new List<ElementTransform>();
                m_TransitionEnables[tk] = true;
            }

            float zero = 0;
            Update(0, ref zero);
        }
        protected ElementTransitionInfo m_BaseTrans = new ElementTransitionInfo();
        protected ElementTransitionInfo m_AnimateTrans = new ElementTransitionInfo();
        protected ElementTransitionInfo m_BlendedTransInfo = null;
        protected Dictionary<TransitionKind, List<ElementTransform>> m_Transitions = new Dictionary<TransitionKind, List<ElementTransform>>();
        protected Dictionary<TransitionKind, bool> m_TransitionEnables = new Dictionary<TransitionKind, bool>();
        protected ElementInfo_Audio m_InternalSound = null;
        public BasicEffect UsingEffect = null;

        [Browsable(false)]
        public Guid GUID
        {
            get;
            set;
        }
        [Browsable(false)]
        public long CreatorId
        {
            get;
            set;
        }
        [Browsable(false)]
        public ElementTransitionInfo BaseTrans
        {
            get { return m_BaseTrans; }
        }
        [Browsable(false)]
        public ElementTransitionInfo AnimateTrans
        {
            get { return m_AnimateTrans; }
        }
        [Browsable(false)]
        public ElementTransitionInfo ExtraAnimateTrans
        {
            get;
            set;
        }
        [Browsable(false)]
        public ElementTransitionInfo BlendedTrans
        {
            get { return m_BlendedTransInfo; }
        }
        [Browsable(false)]
        public ResourceInfo Resource
        {
            get;
            protected set;
        }
        [Browsable(false)]
        public bool IsDuplicatedObject
        {
            get;
            private set;
        }
        internal ViewportInfo m_ParentViewport = null;
        [Browsable(false)]
        public ViewportInfo ParentViewport
        {
            get { return m_ParentViewport; }
            set
            {
                if (m_ParentViewport != value)
                {
                    if (m_ParentViewport != null)
                        m_ParentViewport.RemoveElement(this);
                    m_ParentViewport = value;
                    if (m_ParentViewport != null)
                        m_ParentViewport.AddElement(this);
                }
            }
        }
        [Browsable(false)]
        public virtual bool CanSelect
        {
            get { return !IsLocked && (CreatorId <= 0 || CreatorId == ProjectDoc.Instance.CurEditUserID); }
        }
        [Browsable(false)]
        public virtual bool UseDefaultRectangleBorder
        {
            get { return true; }
        }
        protected bool m_IsInnerEditingMode = false;
        [Browsable(false)]
        public virtual bool IsInnerEditingMode
        {
            get { return m_IsInnerEditingMode; }
            set
            {
                if (m_IsInnerEditingMode != value)
                {
                    m_IsInnerEditingMode = value;
                    if (ParentViewport.ParentSceneInfo != null)
                        ParentViewport.ParentSceneInfo.OnViewportElementInnerEditModeChanged(this);
                }
            }
        }
        [Browsable(false)]
        public virtual bool IsInnerEditable
        {
            get { return false; }
        }
        protected float m_fHotKeyAnimBeginTime = -1;
        [Browsable(false)]
        public virtual float HotKeyAnimBeginTime
        {
            get { return m_fHotKeyAnimBeginTime; }
            set { m_fHotKeyAnimBeginTime = value; }
        }
        [Browsable(false)]
        public virtual bool IsHotKeyAnimVisible
        {
            get { return m_fHotKeyAnimBeginTime >= 0; }
            set
            {
                if (value && m_fHotKeyAnimBeginTime < 0)
                    m_fHotKeyAnimBeginTime = 0;
                else if (!value)
                    m_fHotKeyAnimBeginTime = -1;
            }
        }
        [Category("基本"), DisplayName("名称"), DefaultValue("")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        protected string m_Caption = "";
        [Category("基本"), DisplayName("标题"), DefaultValue("")]
        public virtual string Caption
        {
            get { return m_Caption; }
            set
            {
                if (m_Caption != value && value != null && UsingFont != null)
                {
                    m_Caption = value;
                    for (int i = 0; UsingFont != null && i < m_Caption.Length; ++i)
                    {
                        if (!UsingFont.Characters.Contains(m_Caption[i]))
                            m_Caption = m_Caption.Remove(i--);
                    }
                }
            }
        }
        [Category("基本"), DisplayName("显示标题"), DefaultValue(true)]
        public virtual bool ShowCaption
        {
            get;
            set;
        }
        [Category("外观"), DisplayName("是否隐藏"), DefaultValue(false)]
        public virtual bool IsHidden
        {
            get { return m_BaseTrans.IsHidden; }
            set
            {
                if (m_BaseTrans.IsHidden != value)
                {
                    m_BaseTrans.IsHidden = value;
                }
            }
        }
        [Browsable(false)]
        public virtual bool IsBlendedHidden
        {
            get { return m_BlendedTransInfo.IsHidden; }
        }
        [Browsable(false)]
        public bool IsHotKeyAnimMode
        {
            get
            {
                if ((BeginMethod == AnimBurstType.热键触发 || FinishMethod == AnimBurstType.热键触发))
                    return true;
                return false;
            }
        }
        [Browsable(false)]
        public bool IsRenderable
        {
            get
            {
                if (IsBlendedHidden)
                    return false;
                if (ProjectDoc.Instance.IsProjectAnimationPlaying &&
                    IsHotKeyAnimMode && !IsHotKeyAnimVisible)
                    return false;
                if (!ParentViewport.IsCreatorIDInRendering(CreatorId))
                    return false;
                return true;
            }
        }
        [Browsable(false)]
        public bool IsUpdatable
        {
            get
            {
                if (ProjectDoc.Instance.IsProjectAnimationPlaying &&
                    IsHotKeyAnimMode && !IsHotKeyAnimVisible)
                    return false;
                return true;
            }
        }
        [Browsable(false)]
        public bool IsPreviewElement
        {
            get;
            set;
        }
        [Browsable(false)]
        public float ScaleProportionOnBackImage
        {
            get;
            protected set;
        }
        [Browsable(false)]
        public virtual bool IsBasedOnBackImageElement
        {
            get { return true; }
        }
        [Browsable(false)]
        public bool IsBlendingDisabled
        {
            get;
            set;
        }
        private bool m_IsLocked = false;
        [Browsable(false)]
        public bool IsLocked
        {
            get { return m_IsLocked; }
            set
            {
                m_IsLocked = value;
            }
        }
        [Browsable(false)]
        public virtual System.Drawing.PointF LocationRate
        {
            get
            {
                return new System.Drawing.PointF(
                    m_BaseTrans.Xr - m_BaseTrans.Wr * Barycenter.X,
                    m_BaseTrans.Yr - m_BaseTrans.Hr * Barycenter.Y);
            }
        }
        [Category("外观"), DisplayName("位置"), DefaultValue(typeof(System.Drawing.Point), "0,0")]
        public virtual System.Drawing.Point Location
        {
            get
            {
                var pos = ParentViewport.GetRateLocation(IsBasedOnBackImageElement, LocationRate);
                return new System.Drawing.Point((int)pos.X, (int)pos.Y);
            }
            set
            {
                System.Drawing.PointF pos = value;
                pos = ParentViewport.GetLocationRate(IsBasedOnBackImageElement, pos);
                double x = (double)pos.X + m_BaseTrans.Wr * Barycenter.X;
                double y = (double)pos.Y + m_BaseTrans.Hr * Barycenter.Y;
                m_BaseTrans.Xr = (float)(x + 0.0000005);
                m_BaseTrans.Yr = (float)(y + 0.0000005);
            }
        }
        [Category("状态"), DisplayName("当前位置")]
        public virtual System.Drawing.Point BlendedLocation
        {
            get
            {
                var pos = new System.Drawing.PointF(
                    m_BlendedTransInfo.Xr - m_BlendedTransInfo.Wr * Barycenter.X,
                    m_BlendedTransInfo.Yr - m_BlendedTransInfo.Hr * Barycenter.Y);
                pos = ParentViewport.GetRateLocation(IsBasedOnBackImageElement, pos);
                return new System.Drawing.Point((int)pos.X, (int)pos.Y);
            }
        }
        [Browsable(false)]
        public virtual System.Drawing.Point BlendedBaryLocation
        {
            get
            {
                var pos = ParentViewport.GetRateLocation(IsBasedOnBackImageElement, m_BlendedTransInfo.RateLocation);
                return new System.Drawing.Point((int)pos.X, (int)pos.Y);
            }
        }
        [Category("状态"), DisplayName("当前尺寸")]
        public virtual System.Drawing.Size Size
        {
            get
            {
                var size = ParentViewport.GetRateSize(IsBasedOnBackImageElement, m_BaseTrans.RateSize);
                return new System.Drawing.Size((int)size.Width, (int)size.Height);
            }
        }
        [Category("状态"), DisplayName("当前尺寸")]
        public virtual System.Drawing.Size BlendedSize
        {
            get
            {
                var size = ParentViewport.GetRateSize(IsBasedOnBackImageElement, m_BlendedTransInfo.RateSize);
                return new System.Drawing.Size((int)size.Width, (int)size.Height);
            }
        }
        [Category("外观"), DisplayName("旋转角度"), DefaultValue(0.0F)]
        public virtual float RotateAngle
        {
            get { return m_BaseTrans.Angle; }
            set
            {
                while (value < -360)
                    value += 360;
                while (value > 360)
                    value -= 360;
                if (m_BaseTrans.Angle != value)
                {
                    m_BaseTrans.Angle = value;
                }
            }
        }
        [Category("状态"), DisplayName("当前旋转角度")]
        public virtual float BlendedRotateAngle
        {
            get { return m_BlendedTransInfo.Angle; }
        }
        [Category("外观"), DisplayName("不透明度"), DefaultValue(1.0F)]
        public virtual float Alpha
        {
            get { return m_BaseTrans.Alpha; }
            set
            {
                CommonMethods.ClampValue(ref value, 0, 1);
                if (m_BaseTrans.Alpha != value)
                {
                    m_BaseTrans.Alpha = value;
                }
            }
        }
        [Category("状态"), DisplayName("当前不透明度")]
        public virtual float BlendedAlpha
        {
            get { return m_BlendedTransInfo.Alpha; }
        }
        protected System.Drawing.SizeF m_ManualScale = new System.Drawing.SizeF(1, 1);
        [Category("外观"), DisplayName("附加放缩比"), DefaultValue(typeof(System.Drawing.SizeF), "1,1")]
        public virtual System.Drawing.SizeF ManualScale
        {
            get { return m_ManualScale; }
            set
            {
                if (Resource == null || Resource.ImageSize.Width == 0 || Resource.ImageSize.Height == 0)
                    return;
                float minW = 4 / Resource.ImageSize.Width;
                float minH = 4 / Resource.ImageSize.Height;
                if (value.Width < minW)
                    value.Width = minW;
                if (value.Height < minH)
                    value.Height = minH;
                m_ManualScale = value;
            }
        }
        [Browsable(false)]// 绝对附加放缩比
        public virtual System.Drawing.SizeF ManualScaleOnSrcBackImage
        {
            get
            {
                var scale = m_ManualScale;
                if (ParentViewport.BackImage != null && ParentViewport.BackImage.CurImageScale != 0)
                {
                    scale.Width /= ParentViewport.BackImage.CurImageScale;
                    scale.Height /= ParentViewport.BackImage.CurImageScale;
                }
                return scale;
            }
            set
            {
                m_ManualScale = value;
                if (ParentViewport.BackImage != null && ParentViewport.BackImage.CurImageScale != 0)
                {
                    m_ManualScale.Width *= ParentViewport.BackImage.CurImageScale;
                    m_ManualScale.Height *= ParentViewport.BackImage.CurImageScale;
                }
            }
        }
        [Browsable(false)]// 重心比例
        public System.Drawing.PointF Barycenter
        {
            get;
            set;
        }
        [Category("外观"), DisplayName("颜色叠加"), DefaultValue(typeof(System.Drawing.Color), "White")]
        public System.Drawing.Color BlendColor
        {
            get;
            set;
        }
        protected FlippingState m_FlippingState = FlippingState.正常;
        [Category("外观"), DisplayName("水平翻转"), DefaultValue(false)]
        public bool IsTextureHoriFlipped
        {
            get { return (m_FlippingState & FlippingState.水平翻转) == FlippingState.水平翻转; }
            set
            {
                if (IsTextureHoriFlipped != value)
                {
                    FlippingState fsOld = m_FlippingState;
                    if (value)
                        m_FlippingState |= FlippingState.水平翻转;
                    else
                        m_FlippingState &= ~FlippingState.水平翻转;
                }
            }
        }
        [Category("外观"), DisplayName("竖直翻转"), DefaultValue(false)]
        public bool IsTextureVertFlipped
        {
            get { return (m_FlippingState & FlippingState.竖直翻转) == FlippingState.竖直翻转; }
            set
            {
                if (IsTextureVertFlipped != value)
                {
                    FlippingState fsOld = m_FlippingState;
                    if (value)
                        m_FlippingState |= FlippingState.竖直翻转;
                    else
                        m_FlippingState &= ~FlippingState.竖直翻转;
                }
            }
        }
        [Browsable(false)]
        public FlippingState CurFlippingState
        {
            get { return m_FlippingState; }
            set { m_FlippingState = value; }
        }
        [Category("行为"), DisplayName("分组主导元素"), DefaultValue(false)]
        public bool IsGroupLeader
        {
            get { return ParentElementGroup != null && ParentElementGroup.LeadingElement == this; }
            set
            {
                var grp = ParentElementGroup;
                if (grp != null && IsGroupLeader != value)
                {
                    if (value)
                        ParentElementGroup.LeadingElement = this;
                    else
                        ParentElementGroup.LeadingElement = null;
                }
            }
        }
        [Category("行为"), DisplayName("动画播放倍速"), DefaultValue(1.0F)]
        public float SpeedRate
        {
            get;
            set;
        }
        [Browsable(false)]
        public SpriteFont UsingFont
        {
            get;
            set;
        }
        [Category("行为"), DisplayName("音频文件"), DefaultValue("")]
        public virtual string SoundFile
        {
            get
            {
                if (m_InternalSound == null || m_InternalSound.Channel == null)
                    return "";
                return m_InternalSound.ResAudio.SubPathFileName.TrimStart('\\');
            }
            set
            {
                if (!String.IsNullOrEmpty(value) && SoundFile.ToLower() != value.TrimStart('\\').ToLower())
                {
                    if (m_InternalSound != null && m_InternalSound.IsValid)
                        m_InternalSound.Channel.Clear();
                    var res = ProjectDoc.Instance.ResourceGroups[ResourceKind.音频].GetResourceInfo(value);
                    if (res != null)
                    {
                        if (m_InternalSound == null)
                            m_InternalSound = ElementInfo.CreateElement(res, ParentViewport, new System.Drawing.PointF(0, 0)) as ElementInfo_Audio;
                        else
                        {
                            m_InternalSound.ResAudio = res as ResourceInfo_Audio;
                            m_InternalSound.ReloadSound();
                        }
                    }
                }
                else
                {
                    if (m_InternalSound != null && m_InternalSound.IsValid)
                        m_InternalSound.Channel.Clear();
                    m_InternalSound = null;
                }
            }
        }
        [Category("行为"), DisplayName("音效循环"), DefaultValue(false)]
        public virtual bool IsSoundLooping
        {
            get
            {
                if (m_InternalSound != null)
                    return m_InternalSound.IsSoundLooping;
                return false;
            }
            set
            {
                if (m_InternalSound != null)
                    m_InternalSound.IsSoundLooping = value;
            }
        }
        [Category("行为"), DisplayName("音效延迟时间"), DefaultValue(0.0F)]
        public virtual float SoundDelayTime
        {
            get
            {
                if (m_InternalSound != null)
                    return m_InternalSound.SoundDelayTime;
                return 0;
            }
            set
            {
                if (m_InternalSound != null)
                    m_InternalSound.SoundDelayTime = value;
            }
        }
        [Category("行为"), DisplayName("音效持续时间"), DefaultValue(0.0F)]
        public virtual float SoundPersistTime
        {
            get
            {
                if (m_InternalSound != null)
                    return m_InternalSound.SoundPersistTime;
                return 0;
            }
            set
            {
                if (m_InternalSound != null)
                    m_InternalSound.SoundPersistTime = value;
            }
        }
        [Browsable(false)]
        public System.Drawing.PointF BackViewOffsetRate
        {
            get
            {
                if (!IsBasedOnBackImageElement || ParentViewport.BackImage == null)
                    return new System.Drawing.PointF(0, 0);
                var pos = ParentViewport.BackImage.BaseTrans.RateLocation;
                pos.X *= -1; pos.Y *= -1;
                return pos;
            }
        }
        [Browsable(false)]
        public virtual System.Drawing.RectangleF BoundRect
        {
            get
            {
                var pos = BlendedLocation;
                var size = BlendedSize;
                return new System.Drawing.RectangleF(pos.X, pos.Y, size.Width, size.Height);
            }
        }
        [Browsable(false)]
        public virtual System.Drawing.RectangleF BoundRectRate
        {
            get
            {
                var rect = BoundRect;
                var lu = new System.Drawing.PointF(rect.X, rect.Y);
                lu = ParentViewport.GetLocationRate(true, lu);
                var size = ParentViewport.GetSizeRate(true, rect.Size);
                return new System.Drawing.RectangleF(lu.X, lu.Y, size.Width, size.Height);
                //float x = m_BlendedTransInfo.Xr - m_BlendedTransInfo.Wr * Barycenter.X;
                //float y = m_BlendedTransInfo.Yr - m_BlendedTransInfo.Hr * Barycenter.Y;
                //return new System.Drawing.RectangleF(x, y, m_BlendedTransInfo.Wr, m_BlendedTransInfo.Hr);
            }
        }
        [Browsable(false)]
        public ElementTransform[] Transitions
        {
            get
            {
                return (from k1 in m_Transitions
                        from k2 in k1.Value
                        select k2).ToArray();
            }
        }
        [Browsable(false)]
        public int DepthLevel
        {
            get
            {
                if (GetType() == typeof(ElementInfo_BackgroundImage))
                    return 0;
                return ParentViewport.Elements.IndexOf(this);
            }
            set
            {
                if (value <= 0)
                    value = 1;
                if (value >= ParentViewport.Elements.Count)
                    value = ParentViewport.Elements.Count - 1;
                while (DepthLevel > 0 && DepthLevel != value)
                {
                    if (value > DepthLevel)
                        ParentViewport.BringElementUp(this);
                    else
                        ParentViewport.BringElementDown(this);
                }
            }
        }
        [Category("行为"), DisplayName("开始方式"), DefaultValue(AnimBurstType.时间同步)]
        public AnimBurstType BeginMethod
        {
            get;
            set;
        }
        [Category("行为"), DisplayName("结束方式"), DefaultValue(AnimBurstType.时间同步)]
        public AnimBurstType FinishMethod
        {
            get;
            set;
        }
        private System.Windows.Forms.Keys m_BeginHotKey;
        [Category("行为"), DisplayName("开始热键"), DefaultValue(System.Windows.Forms.Keys.None)]
        public System.Windows.Forms.Keys BeginHotKey
        {
            get { return m_BeginHotKey; }
            set { m_BeginHotKey = value; }
        }
        [Category("行为"), DisplayName("结束热键"), DefaultValue(System.Windows.Forms.Keys.None)]
        public System.Windows.Forms.Keys FinishHotKey
        {
            get;
            set;
        }
        [Browsable(false)]
        public ElementGroup ParentElementGroup
        {
            get { return ParentViewport.ElemGroupCollector.FindElementGroup(this); }
        }
        [Browsable(false)]
        public bool IsAnimationUpdatable
        {
            get
            {
                if (IsBlendingDisabled)
                    return false;
                if (ProjectDoc.Instance.IsProjectAnimationPlaying)
                    return true;
                if (ParentViewport.IsAnimEditingMode && IsSelected)
                    return true;
                var group = ParentElementGroup;
                if (group != null && group.LeadingElement != null && group.LeadingElement != this && group.LeadingElement.IsAnimationUpdatable)
                    return true;
                return false;
            }
        }
        protected float m_TargetMovingSpeed = 1;
        [Category("行为"), DisplayName("目标位移速度"), DefaultValue(1.0f)]
        public float TargetMovingSpeed
        {
            get { return m_TargetMovingSpeed; }
            set { m_TargetMovingSpeed = value; }
        }
        protected System.Drawing.Point? m_TargetLocation = null;
        [Category("行为"), DisplayName("目标点")]
        public System.Drawing.Point? TargetLocation
        {
            get { return m_TargetLocation; }
            set
            {
                m_TargetLocation = value;
                m_CurTargetMovingLocation = Location;
                m_CurTargetMovingLocation.X += Size.Width * Barycenter.X;
                m_CurTargetMovingLocation.Y += Size.Height * Barycenter.Y;
            }
        }

        public void ClearAllTransitions()
        {
            foreach (TransitionKind tk in Enum.GetValues(typeof(TransitionKind)))
            {
                if (tk != TransitionKind.Unknown)
                    m_Transitions[tk].Clear();
            }
            ParentViewport.ParentSceneInfo.OnViewportElementTransitionChanged(this, TransitionKind.Unknown);
        }
        public void AddTransition(ElementTransform trans)
        {
            if (trans != null && !m_Transitions[trans.Kind].Contains(trans))
            {
                m_Transitions[trans.Kind].Add(trans);
                SortTransitions(trans.Kind);
                ParentViewport.ParentSceneInfo.OnViewportElementTransitionChanged(this, trans.Kind);
            }
        }
        public bool RemoveTransition(ElementTransform trans)
        {
            if (m_Transitions[trans.Kind].Remove(trans))
            {
                ParentViewport.ParentSceneInfo.OnViewportElementTransitionChanged(this, trans.Kind);
                return true;
            }
            return false;
        }
        public ElementTransform[] GetTransitions(TransitionKind kind)
        {
            if (!m_Transitions.ContainsKey(kind))
                return new ElementTransform[] { };
            return m_Transitions[kind].ToArray();
        }
        public ElementTransform GetTransitionOnTime(TransitionKind kind, float time, bool getBeforeIfNull)
        {
            var lst = m_Transitions[kind];
            ElementTransform tBefore = null;
            foreach (var trans in lst)
            {
                if (trans.ContainTime(time))
                    return trans;
                else if (trans.TimeEnd <= time)
                    tBefore = trans;
            }
            return getBeforeIfNull ? tBefore : null;
        }
        public ElementTransform CreateOrSplitCurrentTransRange(TransitionKind kind, float time)
        {
            Transition_Element topt;
            return CreateOrSplitCurrentTransRange(kind, time, out topt);
        }
        public ElementTransform CreateOrSplitCurrentTransRange(TransitionKind kind, float time, out Transition_Element topt)
        {
            topt = null;
            if (time > 0)
            {
                var t = GetTransitionOnTime(kind, time, true);
                if (t == null)
                {
                    var opt = new Transition_Element_Create(this, kind, 0, time);
                    topt = opt;
                    if (!OperationHistory.Instance.IsInternalOperating)
                    {
                        OperationHistory.Instance.CommitOperation(opt);
                        return opt.ResultTransform;
                    }
                }
                else if (t.TimeEnd == time)
                {
                    return t;
                }
                else if (t.TimeEnd < time)
                {
                    var opt = new Transition_Element_Create(this, kind, t.TimeEnd, time - t.TimeEnd);
                    topt = opt;
                    if (!OperationHistory.Instance.IsInternalOperating)
                    {
                        OperationHistory.Instance.CommitOperation(opt);
                        return opt.ResultTransform;
                    }
                }
                else
                {
                    var opt = new Transition_Element_Split(this, kind, t, time);
                    topt = opt;
                    if (!OperationHistory.Instance.IsInternalOperating)
                    {
                        OperationHistory.Instance.CommitOperation(opt);
                        return opt.ResultTransform;
                    }
                }
            }
            return null;
        }
        public virtual void Update(float elapsedTime, ref float curViewportTime)
        {
            float curInnerTime = curViewportTime;
            if (!IsPreviewElement && IsHotKeyAnimVisible)
            {
                curInnerTime -= m_fHotKeyAnimBeginTime;
                if (curInnerTime < 0)
                    curInnerTime = 0;
            }
            elapsedTime *= SpeedRate;

            m_BaseTrans.RateSize = ParentViewport.GetSizeRate(IsBasedOnBackImageElement, Resource.ImageSize);
            if (ScaleProportionOnBackImage > 0 && ParentViewport.BackImage != null)
            {
                float rate = ScaleProportionOnBackImage * ParentViewport.BackImage.Size.Width / Size.Width;
                m_BaseTrans.Wr *= rate;
                m_BaseTrans.Hr *= rate;
            }
            m_BaseTrans.Wr *= m_ManualScale.Width;
            m_BaseTrans.Hr *= m_ManualScale.Height;
            OnBeforeBlendedTransforms();
            m_AnimateTrans.Reset();
            if (IsAnimationUpdatable)
            {
                foreach (var kv in m_Transitions)
                {
                    if (m_TransitionEnables[kv.Key])
                    {
                        foreach (var t in kv.Value)
                        {
                            t.Update(curInnerTime);
                        }
                    }
                }
                if (IsGroupLeader)
                    ParentElementGroup.UpdateLeadingChildElements();
                else if (ExtraAnimateTrans != null)
                    m_AnimateTrans = m_AnimateTrans + ExtraAnimateTrans;
                m_BlendedTransInfo = BaseTrans + m_AnimateTrans;
            }
            else
            {
                m_BlendedTransInfo = BaseTrans;
                if (IsGroupLeader)
                    ParentElementGroup.UpdateLeadingChildElements();
            }
            //if (m_BlendedTransInfo.Xr != sx || m_BlendedTransInfo.Yr != sy)
            //{
            //    sx = m_BlendedTransInfo.Xr;
            //    sy = m_BlendedTransInfo.Yr;
            //    //System.Diagnostics.Debug.WriteLine(String.Format("NewXY={0},{1}", sx, sy));
            //}
            OnAfterBlendedTransforms();

            if (m_InternalSound != null)
            {
                if (m_InternalSound.Channel != null)
                    m_InternalSound.Channel.音量倍率 = this.BlendedAlpha;
                m_InternalSound.Update(elapsedTime, ref curViewportTime);
            }
        }
        public virtual bool Draw()
        {
            if (!IsRenderable)
                return false;
            if (m_InternalSound != null)
                m_InternalSound.Draw();
            if (UsingEffect != null)
            {
                if (ParentViewport.SelectedElementInfo != null && ParentViewport.SelectedElementInfo != this && ParentViewport.IsAnimEditingMode)
                    UsingEffect.Alpha = 0.5F;
                else
                    UsingEffect.Alpha = 1.0F;
            }
            if (ShowCaption && !String.IsNullOrEmpty(Caption))
            {
                var rs = ProjectDoc.Instance.HostGame.GraphicsDevice.RenderState;

                var arg_c = rs.CullMode;
                var arg_d = rs.DestinationBlend;
                var arg_s = rs.SourceBlend;

                var ecs = ProjectDoc.Instance.ElementCaptionSprite;
                Vector2 region = ProjectDoc.Instance.DefaultFont.MeasureString(Caption);
                ecs.Begin();
                float scale = ProjectDoc.Instance.Option.DefaultElementCaptionScale;
                if (ProjectDoc.Instance.Option.AutoScaleElementCaption)
                    scale *= m_ManualScale.Width;
                scale *= ParentViewport.ElementsCaptionScale;
                var pos = new Vector2(BlendedBaryLocation.X, BlendedLocation.Y - region.Y * scale);
                var clr = CommonMethods.ConvertColor(ParentViewport.ElementsCaptionColor);
                var clrBk = clr == Color.Black ? Color.White : Color.Black;
                ecs.DrawString(ProjectDoc.Instance.DefaultFont, Caption, pos, clrBk, 0, new Vector2(region.X * 0.5f, 0), scale * 1.1f, SpriteEffects.None, 0.5f);
                ecs.DrawString(ProjectDoc.Instance.DefaultFont, Caption, pos, clr, 0, new Vector2(region.X * 0.5f, 0), scale * 1, SpriteEffects.None, 0.5f);
                ecs.End();

                rs.CullMode = arg_c;
                rs.DestinationBlend = arg_d;
                rs.SourceBlend = arg_s;
            }
            return true;
        }
        public virtual void OnBeforeRemove()
        {
            if (m_InternalSound != null)
                m_InternalSound.UnloadSound();
        }
        public virtual void OnAfterAdded()
        {
            if (m_InternalSound != null)
                m_InternalSound.ReloadSound();
            foreach (var ts in m_Transitions.Values)
            {
                foreach (var t in ts)
                    t.ParentElement = this;
            }
        }
        public virtual void OnBeforeBlendedTransforms()
        {
            if (IsPreviewElement)
            {
                var sizeIn = ParentViewport.GetRateSize(false, m_BaseTrans.RateSize);
                var sizeOut = new System.Drawing.SizeF(ParentViewport.ViewportPtr.Width, ParentViewport.ViewportPtr.Height);
                System.Drawing.PointF offset;
                CommonMethods.GainInnerFittableRegion(ref sizeIn, sizeOut, out offset);
                m_BaseTrans.RateLocation = ParentViewport.GetLocationRate(false, offset);
                m_BaseTrans.RateSize = ParentViewport.GetSizeRate(false, sizeIn);
            }
        }
        private System.Drawing.PointF m_CurTargetMovingLocation = new System.Drawing.PointF();
        public virtual void OnAfterBlendedTransforms()
        {
            if (m_TargetLocation.HasValue)
            {
                var nowPos = Location;
                nowPos.X += (int)(Size.Width * Barycenter.X);
                nowPos.Y += (int)(Size.Height * Barycenter.Y);
                var v1 = new Vector2(nowPos.X, nowPos.Y);
                var v2 = new Vector2(m_TargetLocation.Value.X, m_TargetLocation.Value.Y);
                var dist = v2 - v1;
                float length1 = dist.Length();
                var towards = dist; towards.Normalize();
                v1 += towards * m_TargetMovingSpeed;
                float length2 = (v2 - v1).Length();
                if (length2 < 2 || length2 >= length1)
                {
                    m_TargetLocation = null;
                    v1 = v2;
                }
                m_CurTargetMovingLocation.X += v1.X - nowPos.X;
                m_CurTargetMovingLocation.Y += v1.Y - nowPos.Y;
                Location = new System.Drawing.Point()
                {
                    X = (int)(m_CurTargetMovingLocation.X - Size.Width * Barycenter.X),
                    Y = (int)(m_CurTargetMovingLocation.Y - Size.Height * Barycenter.Y)
                };
            }
        }
        public virtual void OnViewportBackImageChanged(ElementInfo_BackgroundImage elmBack, bool forceChange)
        {
            if ((ScaleProportionOnBackImage != 0 && !forceChange) || Resource == null)
                return;
            if (elmBack == null || elmBack.Size.Width == 0 || elmBack.Size.Height == 0)
                ScaleProportionOnBackImage = (float)Resource.ImageSize.Width / ParentViewport.ViewportSize.Width;
            else
                ScaleProportionOnBackImage = (float)Resource.ImageSize.Width / elmBack.Size.Width;
        }
        public virtual bool OnKeyDown(System.Windows.Forms.Keys key)
        {
            if (ProjectDoc.Instance.IsProjectAnimationPlaying)
            {
                if (!IsHotKeyAnimVisible)
                {
                    if (BeginMethod == AnimBurstType.热键触发 && key == BeginHotKey)
                    {
                        m_fHotKeyAnimBeginTime = ParentViewport.CurTimeTick;
                        FireTerminator.Common.Operations.OperationHistory.Instance.
                            CommitOperation(new FireTerminator.Common.Operations.Operation_Element_SetHotkeyAnimation(this, key, m_fHotKeyAnimBeginTime));
                        return true;
                    }
                }
                else
                {
                    if (FinishMethod == AnimBurstType.热键触发 && key == FinishHotKey)
                    {
                        if (IsHotKeyAnimVisible)
                        {
                            IsHotKeyAnimVisible = false;
                            FireTerminator.Common.Operations.OperationHistory.Instance.
                                CommitOperation(new FireTerminator.Common.Operations.Operation_Element_SetHotkeyAnimation(this, key, -1));
                        }
                        return true;
                    }
                }
            }
            if (m_InternalSound != null)
                m_InternalSound.OnKeyDown(key);
            return false;
        }
        public virtual void OnViewportFinishPlaying()
        {
            if (m_InternalSound != null)
                m_InternalSound.OnViewportFinishPlaying();
            if (IsHotKeyAnimMode)
                IsHotKeyAnimVisible = false;
        }
        public virtual void OnViewportSizeChanged()
        {
            if (m_InternalSound != null)
                m_InternalSound.OnViewportSizeChanged();
        }
        public virtual void OnAnimationPlayingChanged(bool bPlaying)
        {
            if (m_InternalSound != null)
                m_InternalSound.OnAnimationPlayingChanged(bPlaying);
            if (!bPlaying && IsHotKeyAnimMode)
                IsHotKeyAnimVisible = false;
        }
        public void SortTransitions(TransitionKind kind)
        {
            if (kind == TransitionKind.Unknown)
                return;
            var lst = m_Transitions[kind];
            var lstNew = new List<ElementTransform>();
            while (lst.Count > 0)
            {
                float time = Single.MaxValue;
                ElementTransform rst = null;
                foreach (var t in lst)
                {
                    if (t.TimeBegin < time)
                    {
                        time = t.TimeBegin;
                        rst = t;
                    }
                }
                lst.Remove(rst);
                lstNew.Add(rst);
            }
            for (int i = 0; i < lstNew.Count; ++i)
            {
                var t0 = i == 0 ? null : lstNew[i - 1];
                var t1 = lstNew[i];
                var t2 = i == lstNew.Count - 1 ? null : lstNew[i + 1];
                t1.PrevTrans = t0;
                t1.NextTrans = t2;
                if (t2 != null && t1.TimeEnd > t2.TimeBegin)
                {
                    t1.TimeEnd = t2.TimeBegin;
                    if (t1.TimeLength < 0.2F)
                        t1.TimeLength = 0.2F;
                }
            }
            m_Transitions[kind] = lstNew;
        }

        public bool IsTransitionBlendingEnabled(TransitionKind kind)
        {
            bool rst = false;
            m_TransitionEnables.TryGetValue(kind, out rst);
            return rst;
        }
        public void EnableTransitionBlending(TransitionKind kind, bool enabled)
        {
            if (m_TransitionEnables.ContainsKey(kind))
                m_TransitionEnables[kind] = enabled;
        }
        public override string ToString()
        {
            return Name;
        }
        public virtual BodyOperationPart GetPointBodyOprPart(System.Drawing.Point pt)
        {
            System.Drawing.PointF pos = pt;
            pos = ParentViewport.GetLocationRate(IsBasedOnBackImageElement, pos);
            var rect = BoundRectRate;
            if (Single.IsNaN(rect.X) || Single.IsNaN(rect.Y))
                return BodyOperationPart.Nothing;
            float fHalfBorder = (float)ProjectDoc.Instance.Option.ElementScaleFrameThickness / ParentViewport.ViewportPtr.Width * 0.5F;
            if (ParentViewport.ViewScale > 0)
                fHalfBorder /= ParentViewport.ViewScale;
            if (pos.X < rect.X - fHalfBorder || pos.X > rect.Right + fHalfBorder)
                return BodyOperationPart.Nothing;
            if (pos.Y < rect.Y - fHalfBorder || pos.Y > rect.Bottom + fHalfBorder)
                return BodyOperationPart.Nothing;
            if (pos.X < rect.X + fHalfBorder)
            {
                if (pos.Y < rect.Y + fHalfBorder)
                    return BodyOperationPart.CornerLU;
                else if (pos.Y > rect.Bottom - fHalfBorder)
                    return BodyOperationPart.CornerLD;
                return BodyOperationPart.BorderL;
            }
            else if (pos.X > rect.Right - fHalfBorder)
            {
                if (pos.Y < rect.Y + fHalfBorder)
                    return BodyOperationPart.CornerRU;
                else if (pos.Y > rect.Bottom - fHalfBorder)
                    return BodyOperationPart.CornerRD;
                return BodyOperationPart.BorderR;
            }
            else if (pos.Y < rect.Y + fHalfBorder)
            {
                return BodyOperationPart.BorderU;
            }
            else if (pos.Y > rect.Bottom - fHalfBorder)
            {
                return BodyOperationPart.BorderD;
            }

            if (ProjectDoc.Instance.Option.UseElementPixelSelection && Resource is ResourceInfo_StaticImage)
            {
                try
                {
                    var imgRes = Resource as ResourceInfo_StaticImage;
                    float ix = (pos.X - rect.X) / rect.Width * Resource.ImageSize.Width;
                    float iy = (pos.Y - rect.Y) / rect.Height * Resource.ImageSize.Height;
                    Rectangle rcPick = new Rectangle((int)ix, (int)iy, 1, 1);
                    Color[] clrPick = new Color[1];
                    imgRes.ResTexture.GetData<Color>(0, rcPick, clrPick, 0, 1);
                    if (clrPick[0].A == 0)
                        return BodyOperationPart.Nothing;
                }
                catch
                {
                    return BodyOperationPart.Nothing;
                }
            }
            return BodyOperationPart.Body;
        }
        public System.Drawing.Point GetViewportScreenLocationOffset(System.Drawing.Point pt, bool onBary)
        {
            if (onBary)
            {
                System.Drawing.Point ptBary = BlendedBaryLocation;
                pt.X -= ptBary.X;
                pt.Y -= ptBary.Y;
                return pt;
            }
            else
            {
                System.Drawing.PointF pos = pt;
                pos = ParentViewport.GetLocationRate(true, pos);
                pos.X -= m_BlendedTransInfo.Xr - m_BlendedTransInfo.Wr * Barycenter.X;
                pos.Y -= m_BlendedTransInfo.Yr - m_BlendedTransInfo.Hr * Barycenter.Y;
                pos = ParentViewport.GetRateLocation(true, pos);
                if (ParentViewport.BackImage != null)
                {
                    var offset = ParentViewport.BackImage.Location;
                    pos.X -= offset.X;
                    pos.Y -= offset.Y;
                }
                return new System.Drawing.Point((int)pos.X, (int)pos.Y);
            }
        }
        public void GenerateSelectionFrameBuffer(ref List<VertexPositionColor> buffer)
        {
            buffer.Clear();
            if ((!ProjectDoc.Instance.IsProjectAnimationPlaying || !ProjectDoc.Instance.HideSelectionOnPlayingAnimation)
                && CanSelect && UseDefaultRectangleBorder)
            {
                System.Drawing.RectangleF rc;
                var group = ParentViewport.IsAnimEditingMode ? null : ParentViewport.ElemGroupCollector.FindElementGroup(this);
                if (group == null)
                    rc = BoundRect;
                else
                    rc = group.BoundRect;
                rc.X += ParentViewport.ViewportPtr.X;
                rc.Y += ParentViewport.ViewportPtr.Y;
                Vector2[] poses = new Vector2[]
                {
                    new Vector2(rc.X - 1, rc.Y - 1),
                    new Vector2(rc.X + rc.Width + 1, rc.Y - 1),
                    new Vector2(rc.X + rc.Width + 1, rc.Y + rc.Height + 1),
                    new Vector2(rc.X - 1, rc.Y + rc.Height + 1),
                    new Vector2(rc.X - 2, rc.Y - 2),
                    new Vector2(rc.X + rc.Width + 2, rc.Y - 2),
                    new Vector2(rc.X + rc.Width + 2, rc.Y + rc.Height + 2),
                    new Vector2(rc.X - 2, rc.Y + rc.Height + 2),
                };
                for (int i = 0; i < poses.Length; ++i)
                {
                    var vpc = new VertexPositionColor();
                    vpc.Color = CommonMethods.ConvertColor(ProjectDoc.Instance.Option.ElementSelectedFrameColor);
                    vpc.Position = new Vector3(poses[i], 0);
                    buffer.Add(vpc);
                }
            }
        }
        public virtual XmlElement GenerateXmlElement(XmlDocument doc)
        {
            var node = doc.CreateElement("Element");
            node.SetAttribute("Name", Name);
            node.SetAttribute("Guid", GUID.ToString());
            node.SetAttribute("Kind", Resource.Kind.ToString());
            node.SetAttribute("ResourceFile", Resource.SubPath + Resource.FileName);
            if (IsTextureHoriFlipped)
                node.SetAttribute("IsTextureHoriFlipped", IsTextureHoriFlipped.ToString());
            if (IsTextureVertFlipped)
                node.SetAttribute("IsTextureVertFlipped", IsTextureVertFlipped.ToString());
            if (BlendColor != System.Drawing.Color.White)
                node.SetAttribute("BlendColor", BlendColor.ToArgb().ToString());
            if (ManualScale.Width != 1 || ManualScale.Height != 1)
                node.SetAttribute("ManualScale", String.Format("{0},{1}", m_ManualScale.Width, m_ManualScale.Height));
            if (ScaleProportionOnBackImage != 0)
                node.SetAttribute("ScaleProportionOnBackImage", ScaleProportionOnBackImage.ToString());
            node.SetAttribute("ShowCaption", ShowCaption.ToString());
            if (!String.IsNullOrEmpty(Caption))
                node.SetAttribute("Caption", Caption);
            if (!String.IsNullOrEmpty(SoundFile))
                node.SetAttribute("SoundFile", SoundFile);
            if (SoundDelayTime != 0)
                node.SetAttribute("SoundDelayTime", SoundDelayTime.ToString());
            if (SoundPersistTime != 0)
                node.SetAttribute("SoundPersistTime", SoundPersistTime.ToString());
            node.SetAttribute("IsAnimLooping", IsSoundLooping.ToString());
            if (BeginMethod != AnimBurstType.时间同步)
                node.SetAttribute("BeginMethod", BeginMethod.ToString());
            if (BeginHotKey != System.Windows.Forms.Keys.None)
                node.SetAttribute("BeginHotKey", BeginHotKey.ToString());
            if (FinishMethod != AnimBurstType.时间同步)
                node.SetAttribute("FinishMethod", FinishMethod.ToString());
            if (FinishHotKey != System.Windows.Forms.Keys.None)
                node.SetAttribute("FinishHotKey", FinishHotKey.ToString());
            m_BaseTrans.SaveToXmlElement(node);
            foreach (var lst in m_Transitions.Values)
            {
                foreach (var tr in lst)
                    node.AppendChild(tr.GenerateXmlElement(doc));
            }
            return node;
        }
        public virtual void LoadFromXmlElement(XmlElement node)
        {
            Name = node.GetAttribute("Name");
            var value = node.GetAttribute("Guid");
            if (!String.IsNullOrEmpty(value))
                GUID = new Guid(value);
            value = node.GetAttribute("IsTextureHoriFlipped");
            if (!String.IsNullOrEmpty(value))
                IsTextureHoriFlipped = Convert.ToBoolean(value);
            value = node.GetAttribute("IsTextureVertFlipped");
            if (!String.IsNullOrEmpty(value))
                IsTextureVertFlipped = Convert.ToBoolean(value);
            value = node.GetAttribute("BlendColor"); int dwClr;
            if (!String.IsNullOrEmpty(value) && Int32.TryParse(value, out dwClr))
                BlendColor = System.Drawing.Color.FromArgb(dwClr);
            value = node.GetAttribute("ManualScale");
            if (String.IsNullOrEmpty(value))
                m_ManualScale = new System.Drawing.SizeF(1, 1);
            else
                CommonMethods.GetFormatFloat2(value, ref m_ManualScale);
            value = node.GetAttribute("ScaleProportionOnBackImage");
            if (!String.IsNullOrEmpty(value))
                ScaleProportionOnBackImage = float.Parse(value);
            value = node.GetAttribute("ShowCaption");
            if (!String.IsNullOrEmpty(value))
                ShowCaption = Convert.ToBoolean(value);
            value = node.GetAttribute("Caption");
            if (!String.IsNullOrEmpty(value))
                Caption = value;
            else
            {
                value = node.GetAttribute("Text");
                if (!String.IsNullOrEmpty(value))
                    Caption = value;
            }
            value = node.GetAttribute("SoundFile");
            if (String.IsNullOrEmpty(value))
                SoundFile = "";
            else
                SoundFile = value;
            value = node.GetAttribute("SoundDelayTime");
            if (!String.IsNullOrEmpty(value))
                SoundDelayTime = float.Parse(value);
            value = node.GetAttribute("SoundPersistTime");
            if (!String.IsNullOrEmpty(value))
                SoundPersistTime = float.Parse(value);
            value = node.GetAttribute("IsAnimLooping");
            if (!String.IsNullOrEmpty(value))
                IsSoundLooping = Convert.ToBoolean(value);
            value = node.GetAttribute("BeginMethod");
            if (!String.IsNullOrEmpty(value))
                BeginMethod = (AnimBurstType)Enum.Parse(typeof(AnimBurstType), value);
            value = node.GetAttribute("FinishMethod");
            if (!String.IsNullOrEmpty(value))
                FinishMethod = (AnimBurstType)Enum.Parse(typeof(AnimBurstType), value);
            value = node.GetAttribute("BeginHotKey");
            if (!String.IsNullOrEmpty(value))
                BeginHotKey = (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), value);
            value = node.GetAttribute("FinishHotKey");
            if (!String.IsNullOrEmpty(value))
                FinishHotKey = (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), value);

            m_BaseTrans.LoadFromXmlElement(node);

            ClearAllTransitions();
            foreach (var trans in ElementTransform.LoadXml(this, node))
            {
                m_Transitions[trans.Kind].Add(trans);
            }
            foreach (TransitionKind tk in Enum.GetValues(typeof(TransitionKind)))
            {
                if (tk != TransitionKind.Unknown)
                    SortTransitions(tk);
            }
        }

        public static ElementInfo CreateElement(ResourceInfo res, ViewportInfo viewport, System.Drawing.PointF pos)
        {
            return System.Activator.CreateInstance(ElementTypes[res.Kind], res, viewport, false, pos) as ElementInfo;
        }
        public static ElementInfo CreateElement(Type type, ViewportInfo viewport, System.Drawing.PointF pos)
        {
            if (type == null || type.BaseType != typeof(ElementInfo))
                return null;
            foreach (var kv in ElementTypes)
            {
                if (kv.Value == type)
                {
                    var res = System.Activator.CreateInstance(ResourceInfo.ResTypes[kv.Key], kv.Key) as ResourceInfo;
                    return System.Activator.CreateInstance(type, res, viewport, false, pos) as ElementInfo;
                }
            }
            return null;
        }
        public static ElementInfo CreateElement(ElementInfo e)
        {
            return System.Activator.CreateInstance(ElementTypes[e.Resource.Kind], e) as ElementInfo;
        }
        public static Dictionary<ResourceKind, Type> ElementTypes = new Dictionary<ResourceKind, Type>();
    }
}
