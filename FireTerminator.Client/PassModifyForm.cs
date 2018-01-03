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
    public partial class PassModifyForm : Form
    {
        public PassModifyForm()
        {
            InitializeComponent();
        }

        private void bnConfirm_Click(object sender, EventArgs e)
        {
            if (Program.CurUser == null || Program.CurUser.Info == null)
                return;
            if (txbPswCurrent.Text != Program.CurUser.Info.Password)
            {
                MessageBox.Show("当前密码不正确！");
                return;
            }
            if (txbPswNew.Text != txbPswNewConfirm.Text)
            {
                MessageBox.Show("两次输入的新密码不同！");
                return;
            }
            if (!Program.Service.MainChannel.ModifyPassword(txbPswNew.Text))
            {
                MessageBox.Show("服务端拒绝请求，修改密码失败！");
                return;
            }
            Program.CurUser.Info.Password = txbPswNew.Text;
            MessageBox.Show("修改密码成功！");
            this.DialogResult = DialogResult.OK;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
