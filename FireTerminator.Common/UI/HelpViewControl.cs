using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireTerminator.Common.UI
{
    public partial class HelpViewControl : RibbonApplicationUserControl
    {
        public HelpViewControl()
        {
            InitializeComponent();
            this.versionLabel.Text = AssemblyInfo.Version;
            this.copyrightLabel.Text = AssemblyInfo.AssemblyCopyright;
            this.serialNumberLabel.Text = DevExpress.Utils.About.Utility.GetSerial(DevExpress.Utils.About.ProductKind.XtraBars, DevExpress.Utils.About.ProductInfoStage.Registered);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void treeList1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
        }

        private void treeList1_DragOver(object sender, DragEventArgs e)
        {

        }

        private void treeList1_DragObjectStart(object sender, DevExpress.XtraTreeList.DragObjectStartEventArgs e)
        {

        }

        private void treeList1_DragObjectOver(object sender, DevExpress.XtraTreeList.DragObjectOverEventArgs e)
        {

        }

        private void treeList1_DragLeave(object sender, EventArgs e)
        {

        }

        private void treeList1_DragDrop(object sender, DragEventArgs e)
        {

        }
    }
}
