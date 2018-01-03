using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FireTerminator.Common;

namespace FireTerminator.Common.UI
{
    public partial class InputDialog : Form
    {
        public InputDialog()
        {
            InitializeComponent();
        }
        public InputDialog(string title, string caption, string text)
        {
            InitializeComponent();
            if (String.IsNullOrEmpty(title))
                Text = "请输入";
            else
                Text = title;
            if (String.IsNullOrEmpty(caption))
                lblCaption.Text = "值：";
            else
                lblCaption.Text = caption;
            txbInput.Text = text;
            bnOK.Enabled = txbInput.Text.Length > 0;
        }

        public string ResultText
        {
            get { return txbInput.Text; }
        }

        private void InputDialog_Load(object sender, EventArgs e)
        {
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            txbInput.Text = "";
            DialogResult = DialogResult.Cancel;
        }

        private void txbInput_EditValueChanged(object sender, EventArgs e)
        {
            bnOK.Enabled = txbInput.Text.Length > 0;
        }
    }
}
