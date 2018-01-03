using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireTerminator.Common.Audio
{
    public class SoundPlayer : IDisposable
    {
        public SoundPlayer()
        {
        }
        public virtual void Dispose()
        {
            Clear();
        }
        protected List<SoundChannel> m_aChannels = new List<SoundChannel>();
        protected float m_fVolumnMult = 1;
        protected int m_iCurSceneID = -1;
        protected List<int> m_aForbiddenAreas = new List<int>();
        protected bool m_bLoopBackground = false;
        protected bool m_bMuted = false;

        private static SoundPlayer sm_pInst;
        public static SoundPlayer Ptr
        {
            get
            {
                if (sm_pInst == null)
#if true
                    sm_pInst = new SoundPlayer_FMOD();
#else
                    sm_pInst = new SoundPlayer_MCI();
#endif
                return sm_pInst;
            }
        }

        public virtual float 音量倍率
        {
            get { return m_fVolumnMult; }
            set
            {
                if (value < 0) value = 0;
                if (value > 1) value = 1;
                if (m_fVolumnMult != value)
                {
                    m_fVolumnMult = value;
                    foreach (SoundChannel chnl in m_aChannels)
                    {
                        if (chnl != null)
                            chnl.UpdateVolume();
                    }
                }
            }
        }
        public bool 静音
        {
            get { return m_bMuted; }
        }
        public virtual bool 播放中
        {
            get
            {
                foreach (var chnl in m_aChannels)
                {
                    if (chnl.播放中)
                        return true;
                }
                return false;
            }
        }
        protected virtual SoundChannel GetFreeChannel()
        {
            foreach (SoundChannel chnl in m_aChannels)
            {
                if (!chnl.播放中 && !chnl.文件已载入)
                    return chnl;
            }
            return null;
        }
        public virtual void Clear()
        {
            foreach (SoundChannel chnl in m_aChannels)
                chnl.Clear();
        }
        public virtual void Tick(float fElapsedTime)
        {
            foreach (SoundChannel chnl in m_aChannels)
            {
                chnl.Tick(fElapsedTime, null);
            }
        }
    }
}
