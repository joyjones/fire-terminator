using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common;
using System.ComponentModel;
using System.Drawing;
using FireTerminator.Common.Structures;

namespace FireTerminator.Server
{
    public class OptionsServer : Options
    {
        public OptionsServer()
        {
            GCIntervalTime = 30;
            JudgementSet = new JudgementSettings();
            JudgementSet.Load();
        }
        [Browsable(false)]
        public JudgementSettings JudgementSet
        {
            get;
            private set;
        }
        public override string ConfigFileName
        {
            get { return "FireTerminator.Server.configs.xml"; }
        }
        public override string LayoutConfigFileName
        {
            get { return "FireTerminator.Server.layout.xml"; }
        }
        [Category("颜色-消息"), DisplayName("系统提示消息颜色"), ReadOnly(false)]
        public override Color MessageTextColor_SystemPrompt
        {
            get;
            set;
        }
        [Category("颜色-消息"), DisplayName("系统警告消息颜色"), ReadOnly(false)]
        public override Color MessageTextColor_SystemWarning
        {
            get;
            set;
        }
        [Category("颜色-消息"), DisplayName("系统错误消息颜色"), ReadOnly(false)]
        public override Color MessageTextColor_SystemError
        {
            get;
            set;
        }
        [Category("用户"), DisplayName("注册帐号起始ID"), Browsable(true)]
        public override long UserAccountBeginID
        {
            get;
            set;
        }
        [Category("用户"), DisplayName("分组最大用户数量"), Browsable(true)]
        public override int MaxGroupUserCount
        {
            get;
            set;
        }
        [Category("系统"), DisplayName("内存清理间隔时间"), Browsable(true)]
        public int GCIntervalTime
        {
            get;
            set;
        }
        public override void Save()
        {
            base.Save();
            JudgementSet.Save();
        }
    }
}
