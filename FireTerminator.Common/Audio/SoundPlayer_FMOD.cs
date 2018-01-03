using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireTerminator.Common.Audio
{
    public class SoundPlayer_FMOD : SoundPlayer
    {
        public SoundPlayer_FMOD()
        {
            uint version = 0;
            FMOD.RESULT result;

            // Create a System object and initialize.
            result = FMOD.Factory.System_Create(ref FModSystem);
            if (!ERRCHECK(result)) return;
            result = FModSystem.getVersion(ref version);
            if (!ERRCHECK(result)) return;
            if (version < FMOD.VERSION.number)
            {
                //Framework.Log.ApplicationLogSink.Write("FMOD Error! You are using an old version of FMOD " + version.ToString("X") + ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");
                return;
            }

            result = FModSystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            if (!ERRCHECK(result)) return;
        }

        public override void Dispose()
        {
            base.Dispose();
            FMOD.RESULT result;
            foreach (var snd in Sounds)
            {
                if (snd != null)
                    snd.release();
            }
            Sounds.Clear();
            if (FModSystem != null)
            {
                result = FModSystem.close();
                ERRCHECK(result);
                result = FModSystem.release();
                ERRCHECK(result);
            }
        }

        private bool ERRCHECK(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                string err = "Error:FMOD Command Failure: " + result + " - " + FMOD.Error.String(result);
                MessageBox.Show(err);
                //Framework.Log.ApplicationLogSink.Write(err);
                return false;
            }
            return true;
        }
        public FMOD.System FModSystem = null;
        private int channelsplaying = 0;
        private List<FMOD.Sound> Sounds = new List<FMOD.Sound>();

        public SoundChannel_FMOD CreateChannel(string file)
        {
            var chnl = new SoundChannel_FMOD(this, file);
            m_aChannels.Add(chnl);
            return chnl;
        }
        public override void Clear()
        {
            base.Clear();
        }
        public FMOD.Sound GetSoundInstance(string filename)
        {
            //if (dcSounds.ContainsKey(filename))
            //    return dcSounds[filename];
            FMOD.Sound sound = null;
            FMOD.RESULT result = FModSystem.createSound(filename, FMOD.MODE.HARDWARE | FMOD.MODE.CREATESTREAM | FMOD.MODE._3D, ref sound);
            if (!ERRCHECK(result))
                return null;
            //dcSounds[filename] = sound;
            Sounds.Add(sound);
            return sound;
        }
        internal void OnChannelCleared(SoundChannel_FMOD chnl)
        {
            FMOD.Sound snd = chnl.RefFMODSound;
            if (snd != null)
            {
                Sounds.Remove(snd);
                snd.release();
                //bool invalid = true;
                //if (dcSounds.ContainsValue(snd))
                //{
                //    foreach (SoundChannel_FMOD ch in m_aChannels)
                //    {
                //        if (ch != chnl && ch.RefFMODSound == snd)
                //        {
                //            invalid = false;
                //            break;
                //        }
                //    }
                //}
                //if (invalid)
                //{
                //    if (dcSounds.ContainsValue(snd))
                //    {
                //        string[] files = (from kv in dcSounds where kv.Value == snd select kv.Key).ToArray();
                //        if (files != null)
                //        {
                //            foreach (var file in files)
                //                dcSounds.Remove(file);
                //        }
                //    }
                //    snd.release();
                //}
                chnl.RefFMODSound = null;
            }
        }
        public override void Tick(float fElapsedTime)
        {
            base.Tick(fElapsedTime);
            if (FModSystem != null)
            {
                FModSystem.getChannelsPlaying(ref channelsplaying);
                FModSystem.update();
            }
        }
    }
}
