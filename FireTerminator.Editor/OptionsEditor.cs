using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common;
using System.ComponentModel;

namespace FireTerminator.Editor
{
    public class OptionsEditor : Options
    {
        public OptionsEditor()
        {
            VideoEncProfileName = "";
        }
        public override bool Load()
        {
            bool result = base.Load();
            //if (String.IsNullOrEmpty(VideoEncProfileName))
            //{
            //    foreach (var file in VideoRecorder.Instance.EncProfiles)
            //    {
            //        if ((file.Contains("屏幕视频/音频") && file.Contains("高")) ||
            //            (file.Contains("Screen Video/Audio") && file.Contains("High")))
            //        {
            //            VideoEncProfileName = file;
            //            break;
            //        }
            //    }
            //}
            return result;
        }
        public override string ConfigFileName
        {
            get { return "FireTerminator.Editor.configs.xml"; }
        }
        public override string LayoutConfigFileName
        {
            get { return "FireTerminator.Editor.layout.xml"; }
        }
        [Category("录制"), DisplayName("视频编码格式")]
        public string VideoEncProfileName
        {
            get;
            set;
        }
    }
}
