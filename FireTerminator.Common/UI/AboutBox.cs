using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FireTerminator.Common.UI
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("关于 {0}", ProductInfo.AssemblyTitle);
            this.labelProductName.Text = ProductInfo.AssemblyProduct;
            this.labelVersion.Text = String.Format("版本: {0}", ProductInfo.AssemblyVersion);
            this.labelCopyright.Text = ProductInfo.AssemblyCopyright;
            this.labelCompanyName.Text = ProductInfo.AssemblyCompany;
            this.textBoxDescription.Text = ProductInfo.AssemblyDescription;
        }
    }
}
