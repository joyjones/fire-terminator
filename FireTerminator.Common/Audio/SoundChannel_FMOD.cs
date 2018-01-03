using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FireTerminator.Common.Audio
{
    public class SoundChannel_FMOD : SoundChannel
    {
        internal SoundChannel_FMOD(SoundPlayer player, string file)
            : base(player, file)
        {
            音频文件 = file;
        }

        public FMOD.Sound RefFMODSound = null;
        private FMOD.Channel m_FMODChannel = null;
        private bool ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                string err = "Error:FMOD Command Failure: " + result + " - " + FMOD.Error.String(result);
                //Framework.Log.ApplicationLogSink.Write(err);
                return false;
            }
            return true;
        }
        public FMOD.System FModSystem
        {
            get
            {
                return (m_HostPlayer as SoundPlayer_FMOD).FModSystem;
            }
        }
        public override bool 静音
        {
            get
            {
                return base.静音;
            }
            set
            {
                base.静音 = value;
                if (m_FMODChannel != null)
                    m_FMODChannel.setMute(m_bMuted);
            }
        }

        public override float 音量平衡
        {
            get
            {
                return base.音量平衡;
            }
            set
            {
                base.音量平衡 = value;
                if (m_FMODChannel != null)
                    m_FMODChannel.setPan(m_fVolBalance);
            }
        }

        public override bool 循环
        {
            get { return base.循环; }
            set
            {
                if (m_bLoop != value)
                {
                    m_bLoop = value;
                    //if (m_FMODChannel != null)
                    //{
                    //    if (m_bLoop)
                    //        m_FMODChannel.setLoopCount(-1);
                    //    else
                    //        m_FMODChannel.setLoopCount(0);
                    //}
                }
            }
        }
        public override ulong 当前位置
        {
            get
            {
                if (m_bOpened && m_bPlaying && m_FMODChannel != null)
                {
                    uint position = 0;
                    m_FMODChannel.getPosition(ref position, FMOD.TIMEUNIT.MS);
                    return (ulong)position;
                }
                return 0;
            }
        }

        public override void Seek(ulong Millisecs)
        {
            if (m_bOpened && Millisecs <= m_nLength)
            {
                if (m_bPlaying)
                {
                    if (m_FMODChannel != null)
                        m_FMODChannel.setPosition((uint)Millisecs, FMOD.TIMEUNIT.MS);
                }
            }
        }

        public override void Clear()
        {
            if (m_bOpened)
            {
                播放中 = false;

                if (m_FMODChannel != null)
                    m_FMODChannel.stop();
                m_FMODChannel = null;
                ((SoundPlayer_FMOD)m_HostPlayer).OnChannelCleared(this);
                m_bOpened = false;
                m_bPlaying = false;
                m_bPaused = false;
                m_strFileName = "";
            }
        }

        protected override void Open(string fileName)
        {
            if (!m_bOpened)
            {
                string path = ProjectDoc.Instance.ResourceGroups[ResourceKind.音频].RootPath;
                RefFMODSound = ((SoundPlayer_FMOD)m_HostPlayer).GetSoundInstance(path + fileName);
                if (RefFMODSound == null)
                    return;

                uint length = 0;
                if (ERRCHECK(RefFMODSound.getLength(ref length, FMOD.TIMEUNIT.MS)))
                    m_nLength = (ulong)length;
                else
                    m_nLength = 0;
                m_bOpened = true;
            }
        }

        public override bool 播放中
        {
            get { return base.播放中; }
            set
            {
                if (m_bPlaying == value)
                    return;
                if (value)
                {
                    if (!m_bOpened)
                    {
                        if (音频文件.Length > 0)
                            Open(音频文件);
                    }
                    if (m_bOpened)
                    {
                        if (!m_bPlaying)
                        {
                            m_bPlaying = true;
                            FMOD.RESULT result = FModSystem.playSound(FMOD.CHANNELINDEX.FREE, RefFMODSound, true, ref m_FMODChannel);
                            if (!ERRCHECK(result)) return;

                            m_FMODChannel.setVolume(音量倍率);
                            m_FMODChannel.setMute(静音);
                            m_FMODChannel.setPaused(false);
                            m_FMODChannel.setLoopCount(0);

                            UpdateVolume();
                        }
                        else
                        {
                            if (!m_bPaused)
                                m_FMODChannel.setPosition(0, FMOD.TIMEUNIT.MS);
                            m_bPaused = false;
                            m_FMODChannel.setPaused(m_bPaused);
                        }
                    }
                }
                else
                {
                    if (m_bOpened && m_bPlaying)
                    {
                        m_bPlaying = false;
                        m_bPaused = false;
                        m_FMODChannel.stop();
                    }
                }
            }
        }

        public override bool 暂停中
        {
            get { return base.暂停中; }
            set
            {
                if (m_bOpened && m_bPaused != value)
                {
                    base.暂停中 = value;
                    m_FMODChannel.setPaused(m_bPaused);
                }
            }
        }

        public override void UpdateVolume()
        {
            //设置音量后，只有已载入文件的channel才去实际设置音量，否则仅设置音量数值
            if (this.文件已载入 && m_FMODChannel != null)
            {
                float fVolumeAll = m_fVolumeMult * m_HostPlayer.音量倍率;
                if (m_HostPlayer.静音)
                    fVolumeAll = 0;
                m_FMODChannel.setVolume(fVolumeAll);
            }
        }

        //float fTicking = 0;
        public override void Tick(float fElapsedTime, Vector3? vListenerPos)
        {
            base.Tick(fElapsedTime, vListenerPos);
            if (this.文件已载入 && m_FMODChannel != null)
            {
                m_FMODChannel.getMute(ref m_bMuted);
                //m_FMODChannel.getVolume(ref m_fVolumeMult);
                m_FMODChannel.getPaused(ref m_bPaused);
                m_FMODChannel.isPlaying(ref m_bPlaying);
                //fTicking += fElapsedTime;
            }
            //if (fTicking > 1.0F)
            //{
            //    fTicking = 0;
            //    string msg = String.Format("GM:{0},M:{1},GV:{2},V:{3}/{4}/{5},U:{6},P:{7}",
            //        m_HostPlayer.静音, m_bMuted, m_HostPlayer.音量倍率, m_fVolumeMult, m_fVolumeAttuMult, m_fVolumeMutedMult, m_bPaused, m_bPlaying);
            //    Framework.Log.ApplicationLogSink.Write("Sound:" + msg);
            //}
        }
    }
}
