using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.Operations;
using System.Windows.Forms;
using FireTerminator.Common.Services;

namespace FireTerminator.Server
{
    public class ServerEditViewOperater : EditViewOperater
    {
        public ServerEditViewOperater(Form form, Panel view, Panel preview)
            : base(form, view, preview)
        {
            ParentForm = form as MainForm;
        }
        public MainForm ParentForm
        {
            get;
            private set;
        }
        protected override void CurFocusSceneInfo_ViewportAnimEditChanged(FireTerminator.Common.ViewportInfo vi)
        {
            base.CurFocusSceneInfo_ViewportAnimEditChanged(vi);
        }
        protected override void CurFocusSceneInfo_ViewportElementChanged(FireTerminator.Common.Elements.ElementInfo elm)
        {
            base.CurFocusSceneInfo_ViewportElementChanged(elm);
        }
    }
}
