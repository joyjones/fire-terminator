using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FireTerminator.Common.Audio
{
    public class SoundChannel
    {
        protected string m_strFileName = "";
        protected List<int> m_aStopAreaIDs = new List<int>();

        protected SoundPlayer m_HostPlayer = null;
        protected string m_strHandle = "";
        protected bool m_bOpened = false, m_bPlaying = false, m_bPaused = false, m_bMuted = false, m_bLoop = false;
        //protected int m_iVolumeTreble = 1000, m_iVolumeBass = 1000;
        protected float m_fVolBalance = 0;
        protected ulong m_nLength = 0;
        //protected float m_fCurGapTick = 0;
        protected uint m_nErrorCode = 0;
        protected float m_fVolumeMult = 1;
        protected Vector3 m_vCenterPos = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
        //protected float m_fVolumeAttuMult = 1;
        //protected float m_fVolumeFadeMult = 1;
        //protected float m_fVolumeMutedMult = 1;
        //protected float m_fMutedDelayTime = 3;
        //protected float m_fCurMutedDelayTick = 3;
        //protected float m_fFirstPlayDelayTick = -1;
        //protected float m_fClearDelayTick = 0;
        //protected float m_fClearDelayTime = 0;
        //protected bool m_bClearing = false;

        internal SoundChannel(SoundPlayer player, string handle)
        {
            m_HostPlayer = player;
            m_strHandle = handle;
        }

        #region 静音属性
        public virtual bool 静音
        {
            get { return m_bMuted; }
            set
            {
                if (m_bMuted != value)
                {
                    m_bMuted = value;
                    UpdateVolume();
                }
            }
        }

        #endregion

        #region 音量属性
        public virtual float 音量倍率
        {
            get { return m_fVolumeMult; }
            set
            {
                m_fVolumeMult = value;
                UpdateVolume();
            }
        }
        public virtual float 音量平衡
        {
            get
            {
                return m_fVolBalance;
            }
            set
            {
                if (value < -1) value = -1;
                if (value > 1) value = 1;
                if (m_bOpened)
                {
                    m_fVolBalance = value;
                    UpdateVolume();
                }
            }
        }
        #endregion

        #region 其他属性

        public List<int> 排斥区域索引
        {
            get { return m_aStopAreaIDs; }
        }
        public virtual bool 播放中
        {
            get { return m_bPlaying; }
            set
            {
                if (m_bOpened && m_bPlaying != value)
                {
                    m_bPlaying = value;
                    m_bPaused = false;
                }
            }
        }
        public virtual bool 暂停中
        {
            get { return m_bPaused; }
            set
            {
                if (m_bOpened)
                    m_bPaused = value;
            }
        }
        public bool 文件已载入
        {
            get { return m_bOpened; }
        }
        public string 通道名称
        {
            get { return m_strHandle; }
        }
        public string 音频文件
        {
            get { return m_strFileName; }
            set
            {
                if (m_strFileName != value)
                {
                    播放中 = false;
                    m_strFileName = value;
                }
            }
        }

        public virtual bool 循环
        {
            get { return m_bLoop; }
            set
            {
                if (m_bLoop != value)
                {
                    m_bLoop = value;
                    //if (m_bLoop && m_bOpened && m_SoundInfo != null && !m_bPlaying)
                    //    m_fCurGapTick = m_SoundInfo.播放间隔时间;
                }
            }
        }

        public virtual ulong 长度
        {
            get { return m_bOpened ? m_nLength : 0; }
        }

        public virtual ulong 当前位置
        {
            get
            {
                return 0;
            }
        }

        public virtual Vector3 声源坐标
        {
            get { return m_vCenterPos; }
            set
            {
                m_vCenterPos.X = value.X;
                m_vCenterPos.Y = value.Y;
                m_vCenterPos.Z = value.Z;
            }
        }
        #endregion

        #region 处理函数

        public virtual void Seek(ulong Millisecs)
        {
        }

        public virtual void Clear()
        {
            if (m_bOpened)
            {
                if (m_bPlaying)
                    播放中 = false;
                m_bOpened = false;
                m_bPlaying = false;
                m_bPaused = false;
                m_strFileName = "";
            }
        }

        protected virtual void Open(string sFileName)
        {
        }

        public virtual void UpdateVolume()
        {
        }

        public virtual void Tick(float fElapsedTime, Vector3? vListenerPos)
        {
            if (播放中)
            {
                float fCurTime = 当前位置 * 0.001f;
                float fTotalTime = 长度 * 0.001f;
                if (fCurTime < fTotalTime)
                    UpdateVolume();
                else if (循环)
                {
                    播放中 = false;
                    播放中 = true;
                }
            }
        }
        #endregion
    }
}
