using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace FireTerminator.Common.Audio
{
    public class SoundChannel_MCI : SoundChannel
    {
        public SoundChannel_MCI(SoundPlayer player, string handle)
            : base(player, handle)
        {
        }

        protected string m_strCommand = "";
        #region 静音属性
        public override bool 静音
        {
            get
            {
                return base.静音;
            }
            set
            {
                base.静音 = value;
                //if (m_bMuted)
                //{
                //    m_strCommand = "setaudio " + m_strHandle + " off";
                //    MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                //}
                //else
                //{
                //    m_strCommand = "setaudio " + m_strHandle + " on";
                //    MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                //}
            }
        }
        #endregion

        #region 音量属性
        public override int 音量高音
        {
            get
            {
                return base.音量高音;
            }
            set
            {
                if (m_bOpened && (value >= 0 && value <= 1000))
                {
                    m_iVolumeTreble = value;
                    m_strCommand = String.Format("setaudio {0} treble to {1}", m_strHandle, m_iVolumeTreble);
                    MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                }
            }
        }

        public override int 音量低音
        {
            get
            {
                return base.音量低音;
            }
            set
            {
                if (m_bOpened && (value >= 0 && value <= 1000))
                {
                    m_iVolumeBass = value;
                    m_strCommand = String.Format("setaudio {0} bass to {1}", m_strHandle, m_iVolumeBass);
                    MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                }
            }
        }

        #endregion

        #region 其他属性

        public override ulong 当前位置
        {
            get
            {
                if (m_bOpened && m_bPlaying)
                {
                    m_strCommand = "status " + m_strHandle + " position";
                    String str = GUnrealLoop.Ptr.MCICommandQuery(m_strCommand);
                    if (str.Length < 1)
                        return 0;
                    ulong pos = Convert.ToUInt64(str.ToString());
                    return pos;
                }
                else return 0;
            }
        }

        #endregion

        #region 处理函数

        public override void Seek(ulong Millisecs)
        {
            if (m_bOpened && Millisecs <= m_nLength)
            {
                if (m_bPlaying)
                {
                    if (m_bPaused)
                    {
                        m_strCommand = String.Format("seek {0} to {1}", m_strHandle, Millisecs);
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                    }
                    else
                    {
                        m_strCommand = String.Format("seek {0} to {1}", m_strHandle, Millisecs);
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                        m_strCommand = "play " + m_strHandle;
                        //if (m_SoundInfo.是否循环) m_strCommand += " REPEAT";
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                    }
                }
            }
        }

        public override void Clear()
        {
            if (m_bOpened)
            {
                if (m_bPlaying) Stop();
                m_strCommand = "close " + m_strHandle;
                MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                m_bOpened = false;
                m_bPlaying = false;
                m_bPaused = false;
                m_aFileLst.Clear();
                m_bClearing = false;
            }
        }

        protected override void Open(string sFileName)
        {
            if (!m_bOpened)
            {
                string strFullPath = GetValidPathFile(sFileName);
                if (String.IsNullOrEmpty(strFullPath))
                    return;
                m_strCommand = "open \"" + strFullPath + "\" type mpegvideo alias " + m_strHandle;
                MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                m_bOpened = true;
                m_bPlaying = false;
                m_bPaused = false;
                m_strCommand = "set " + m_strHandle + " time format milliseconds";
                MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                m_strCommand = "set " + m_strHandle + " seek exactly on";
                MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);

                m_strCommand = "status " + m_strHandle + " length";
                String str = GUnrealLoop.Ptr.MCICommandQuery(m_strCommand);
                if (str.Length > 0)
                    m_nLength = Convert.ToUInt64(str.ToString());
                else
                    m_nLength = 0;
                m_bClearing = false;
            }
        }

        public override void Play()
        {
            if (!m_bOpened)
            {
                string strFile = 文件名;
                if (strFile.Length > 0)
                    Open(strFile);
            }
            if (m_bOpened)
            {
                if (!m_bPlaying)
                {
                    if (m_fFirstPlayDelayTick < 0 && m_SoundInfo.播放延迟时间 > 0)
                    {
                        m_fFirstPlayDelayTick = 0;
                        return;
                    }
                    m_bPlaying = true;
                    m_strCommand = "play " + m_strHandle;
                    //if (m_SoundInfo.是否循环) m_strCommand += " REPEAT";
                    MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                    UpdateVolume();
                    if (m_bMuted)
                    {
                        m_strCommand = "setaudio " + m_strHandle + " off";
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                    }
                }
                else
                {
                    if (!m_bPaused)
                    {
                        m_strCommand = "seek " + m_strHandle + " to start";
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                        m_strCommand = "play " + m_strHandle;
                        //if (m_SoundInfo.是否循环) m_strCommand += " REPEAT";
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                    }
                    else
                    {
                        m_bPaused = false;
                        m_strCommand = "play " + m_strHandle;
                        //if (m_SoundInfo.是否循环) m_strCommand += " REPEAT";
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                    }
                }
            }
        }

        public override void Pause()
        {
            if (m_bOpened)
            {
                if (!m_bPaused)
                {
                    m_bPaused = true;
                    m_strCommand = "pause " + m_strHandle;
                    MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                }
                else
                {
                    m_bPaused = false;
                    m_strCommand = "play " + m_strHandle;
                    //if (m_SoundInfo.是否循环) m_strCommand += " REPEAT";
                    MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                }
            }
        }

        public override void Stop()
        {
            if (m_bOpened && m_bPlaying)
            {
                m_bPlaying = false;
                m_bPaused = false;
                m_strCommand = "seek " + m_strHandle + " to start";
                MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                m_strCommand = "stop " + m_strHandle;
                MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
            }
        }

        public override void UpdateVolume()
        {
            if (this.文件已载入)//设置音量后，只有已载入文件的channel才去实际设置音量，否则仅设置音量数值
            {
                int iVolumeAll = (int)(m_SoundInfo.音量 * m_fVolumeMult * m_fVolumeAttuMult * m_fVolumeMutedMult);
                if (m_HostPlayer.静音 || m_bMuted)
                    iVolumeAll = 0;

                m_strCommand = String.Format("setaudio {0} volume to {1}", m_strHandle, iVolumeAll);
                MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                if (m_fVolBalance != 0)
                {
                    string strL = "setaudio {0} left volume to {1}";
                    string strR = "setaudio {0} right volume to {1}";
                    if (m_fVolBalance < 0)
                    {
                        m_strCommand = String.Format(strL, m_strHandle, iVolumeAll);
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                        int vr = (int)((float)iVolumeAll * (m_fVolBalance - 1));
                        m_strCommand = String.Format(strR, m_strHandle, vr);
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                    }
                    else
                    {
                        m_strCommand = String.Format(strR, m_strHandle, iVolumeAll);
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                        int vl = (int)((float)iVolumeAll * (1 - m_fVolBalance));
                        m_strCommand = String.Format(strL, m_strHandle, vl);
                        MCIERROR err = mciSendString(m_strCommand, NULL, 0, NULL);
                    }
                }
            }
        }

        public override void Tick(float fElapsedTime, mVector3 vListenerPos)
        {
            base.Tick(fElapsedTime, vListenerPos);
        }
        #endregion
    }
}
