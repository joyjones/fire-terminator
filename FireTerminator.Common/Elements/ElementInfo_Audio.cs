using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Audio;

namespace FireTerminator.Common.Elements
{
    public class ElementInfo_Audio : ElementInfo
    {
        public ElementInfo_Audio(ResourceInfo res, ViewportInfo vi, bool isPreview, System.Drawing.PointF pos)
            : base(res, vi, isPreview, pos)
        {
            Channel = ProjectDoc.Instance.FModPlayer.CreateChannel(res.SubPathFileName);
            m_bPlaying = isPreview;
            IsSoundLooping = isPreview;
        }
        public ElementInfo_Audio(ElementInfo_Audio e)
            : base(e)
        {
            m_bPlaying = e.m_bPlaying;
            m_nPlayCounter = e.m_nPlayCounter;
        }
        [Browsable(false)]
        public ResourceInfo_Audio ResAudio
        {
            get { return Resource as ResourceInfo_Audio; }
            set
            {
                if (value != null)
                    Resource = value;
            }
        }
        [Browsable(false)]
        public bool IsValid
        {
            get { return Channel != null && Channel.文件已载入; }
        }
        [Browsable(false)]
        public SoundChannel_FMOD Channel
        {
            get;
            private set;
        }
        [Browsable(false)]
        public override bool UseDefaultRectangleBorder
        {
            get { return false; }
        }
        [Browsable(false)]
        public override string SoundFile
        {
            get { return ""; }
            set { }
        }
        private bool m_bLoop = false;
        public override bool IsSoundLooping
        {
            get { return m_bLoop; }
            set { m_bLoop = value; }
        }
        private float m_SoundDelayTime = 0;
        public override float SoundDelayTime
        {
            get { return m_SoundDelayTime; }
            set { m_SoundDelayTime = value; }
        }
        private float m_SoundPersistTime = 0;
        public override float SoundPersistTime
        {
            get { return m_SoundPersistTime; }
            set { m_SoundPersistTime = value; }
        }

        protected bool m_bPlaying = false;
        protected int m_nPlayCounter = 0;
        protected float m_fPlayingTime = 0;
        public void UnloadSound()
        {
            if (Channel != null)
                Channel.Clear();
            Channel = null;
        }
        public void ReloadSound()
        {
            Channel = ProjectDoc.Instance.FModPlayer.CreateChannel(ResAudio.SubPathFileName);
        }
        public override void Update(float elapsedTime, ref float curViewportTime)
        {
            base.Update(elapsedTime, ref curViewportTime);
            if (Channel != null)
            {
                if (m_bPlaying)
                {
                    if (IsPreviewElement && Channel.播放中 && ProjectDoc.Instance.IsProjectAnimationPlaying)
                        Channel.播放中 = false;
                    m_fPlayingTime += elapsedTime;
                    if (m_nPlayCounter == 0 || IsSoundLooping)
                    {
                        if (!Channel.播放中)
                        {
                            if (m_fPlayingTime >= SoundDelayTime)
                            {
                                Channel.播放中 = true;
                                ++m_nPlayCounter;
                            }
                        }
                    }
                    bool isFinished = (m_nPlayCounter > 0 && !IsSoundLooping && !Channel.播放中);
                    if (!isFinished && SoundPersistTime > 0 && Channel.播放中)
                    {
                        if (m_fPlayingTime >= SoundDelayTime + SoundPersistTime)
                        {
                            Channel.播放中 = false;
                            isFinished = true;
                        }
                    }
                    if (isFinished)
                    {
                        m_bPlaying = false;
                        m_nPlayCounter = 0;
                        m_fPlayingTime = 0;
                    }
                }
                else if (Channel.播放中)
                {
                    Channel.播放中 = false;
                }
                Channel.Tick(elapsedTime, null);
            }
        }
        public override bool Draw()
        {
            return true;
        }
        public override void OnAnimationPlayingChanged(bool bPlaying)
        {
            base.OnAnimationPlayingChanged(bPlaying);
            if (bPlaying && BeginMethod == AnimBurstType.热键触发)
                bPlaying = false;
            m_nPlayCounter = 0;
            m_fPlayingTime = 0;
            m_bPlaying = bPlaying;
        }
        public override bool OnKeyDown(System.Windows.Forms.Keys key)
        {
            bool result = base.OnKeyDown(key);
            if (!m_bPlaying && ProjectDoc.Instance.IsProjectAnimationPlaying)
            {
                if (BeginMethod == AnimBurstType.热键触发 && key == BeginHotKey)
                {
                    m_fPlayingTime = 0;
                    m_nPlayCounter = 0;
                    m_bPlaying = true;
                    return true;
                }
            }
            return result;
        }
    }
}
