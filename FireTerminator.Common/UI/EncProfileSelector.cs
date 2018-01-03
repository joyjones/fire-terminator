using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireTerminator.Common.UI
{
    public partial class EncProfileSelector : Form
    {
        public EncProfileSelector()
        {
            InitializeComponent();
        }

        public EncProfileSelector(string[] list)
        {
            InitializeComponent();
            foreach (var file in list)
            {
                cmbEncodings.Items.Add(file);
            }
        }

        private string m_SelectedResult = null;
        public string SelectedResult
        {
            get { return m_SelectedResult; }
        }

        private void EncProfileSelector_Load(object sender, EventArgs e)
        {
        }

        private void cmbEncodings_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_SelectedResult = cmbEncodings.SelectedItem as string;
        }
    }
}
