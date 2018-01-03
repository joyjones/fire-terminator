using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireTerminator.Client
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            this.grdProperties.SelectedObject = Program.Option;
        }

        private void OptionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Option.Save();
        }
    }
}
