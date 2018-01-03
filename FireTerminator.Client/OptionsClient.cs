using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common;

namespace FireTerminator.Client
{
    public class OptionsClient : Options
    {
        public override string ConfigFileName
        {
            get { return "FireTerminator.Client.configs.xml"; }
        }
        public override string LayoutConfigFileName
        {
            get { return "FireTerminator.Client.layout.xml"; }
        }
    }
}
