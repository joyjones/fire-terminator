using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireTerminator.Common.Audio
{
    public class SoundPlayer_MCI : SoundPlayer
    {
        public SoundPlayer_MCI()
        {
            string handle = "";
            for (int i = 0; i < 16; ++i)
            {
                if (i == 0)
                    handle = "BGMusic";
                else
                    handle = "FGMusic" + i;
                m_aChannels.Add(new SoundChannel_MCI(this, handle));
            }
        }
    }
}
