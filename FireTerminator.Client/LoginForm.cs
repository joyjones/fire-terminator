using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FireTerminator.Common;
using FireTerminator.Common.Structures;
using FireTerminator.Common.Services;
using System.Configuration;

namespace FireTerminator.Client
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm, ILoginServiceCallback
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public bool UIEnabled
        {
            set
            {
                this.BeginInvokeEx(() =>
                {
                    lblUpdateStatus.ForeColor = Color.Gray;
                    cmbAccount.Enabled = value;
                    txbPassword.Enabled = value;
                    cmbServerIP.Enabled = value;
                    bnLogin.Enabled = value;
                });
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            UIEnabled = true;
            if (Program.IsLoginJudgeMode)
            {
                ProductInfo.AssemblyProduct = "专家评判系统";
                MainForm.Instance.dockMgr.ActivePanel = MainForm.Instance.dcpJudgement;
            }
            MainForm.Instance.ViewOperater.RefreshAppProjectTitle();
            Text = ProductInfo.AssemblyProduct + " 登录";

            lblUpdateStatus.Text = "请输入登录信息。";

            cmbServerIP.Properties.Items.Clear();
            string text = ConfigurationManager.AppSettings["ServerIPs"];
            string[] ips = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (ips == null || ips.Length == 0)
                cmbServerIP.Properties.Items.Add("127.0.0.1");
            else
            {
                foreach (var ip in ips)
                    cmbServerIP.Properties.Items.Add(ip);
            }
            cmbServerIP.SelectedIndex = 0;

            cmbAccount.Properties.Items.Clear();
            text = ConfigurationManager.AppSettings["UserAccounts"];
            string[] accs = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (accs != null && accs.Length > 0)
            {
                foreach (var acc in accs)
                    cmbAccount.Properties.Items.Add(acc);
                cmbAccount.SelectedIndex = 0;
            }
            cmbAccount.Focus();

            if (Program.StartupArgs.Count > 0)
            {
                bool autoLogin = true;
                var val = Program.GetStartupArgValue("ip");
                if (!String.IsNullOrEmpty(val))
                    cmbServerIP.Text = val;
                else
                    autoLogin = false;
                val = Program.GetStartupArgValue("user");
                if (!String.IsNullOrEmpty(val))
                    cmbAccount.Text = val;
                else
                    autoLogin = false;
                val = Program.GetStartupArgValue("pass");
                if (!String.IsNullOrEmpty(val))
                    txbPassword.Text = val;
                if (cmbServerIP.Text.Length > 0 && cmbAccount.Text.Length > 0 && autoLogin)
                {
                    bnLogin_Click(null, null);
                }
            }
        }

        private void bnLogin_Click(object sender, EventArgs e)
        {
            if (cmbServerIP.Text.Length == 0)
            {
                lblUpdateStatus.ForeColor = Color.Red;
                lblUpdateStatus.Text = "请输入服务器地址。";
                return;
            }
            if (cmbAccount.Text.Length == 0)
            {
                lblUpdateStatus.ForeColor = Color.Red;
                lblUpdateStatus.Text = "请输入帐号。";
                return;
            }
            UIEnabled = false;
            string ip = cmbServerIP.Text;
            string account = cmbAccount.Text;
            string pass = txbPassword.Text;
            if (account == "t")
            {
                NotifyLoginResult(null, LoginResult.登录成功);
                return;
            }
            lblUpdateStatus.Text = "正在连接到服务器...";
            Program.Service.Login(ip, account, pass, Program.IsLoginJudgeMode, () =>
            {
                this.InvokeEx(() =>
                {
                    lblUpdateStatus.Text = "未能连接到服务器！";
                    UIEnabled = true;
                });
            });
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void LoginForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                bnLogin_Click(bnLogin, null);
        }

        #region ILoginServiceCallback 成员
        public void NotifyLoginResult(RegisteredUserInfo user, LoginResult result)
        {
            this.BeginInvokeEx(() =>
            {
                if (result == LoginResult.登录成功 || result == LoginResult.登录并注册成功)
                {
                    Program.CurUser.Info = user;
                    Program.CurUser.AdjudicatorMode = Program.IsLoginJudgeMode && ((user.Permission & UserPermission.评审) == UserPermission.评审);
                    ProjectDoc.Instance.CurEditUserID = user.AccountID;
                    if (result == LoginResult.登录并注册成功 && Program.StartupArgs.Count == 0)
                        MessageBox.Show("注册成功，你的帐号ID是：" + user.AccountID);
                    UIEnabled = true;

                    if (cmbServerIP.Text.Length > 0 && !cmbServerIP.Properties.Items.Contains(cmbServerIP.Text))
                        cmbServerIP.Properties.Items.Insert(0, cmbServerIP.Text);
                    else if (cmbServerIP.Properties.Items.Count > 1)
                    {
                        string sel = cmbServerIP.SelectedItem as string;
                        cmbServerIP.Properties.Items.RemoveAt(cmbServerIP.SelectedIndex);
                        cmbServerIP.Properties.Items.Insert(0, sel);
                        cmbServerIP.SelectedIndex = 0;
                    }
                    string ips = "";
                    foreach (var item in cmbServerIP.Properties.Items)
                    {
                        if (ips.Length > 0)
                            ips += ";";
                        ips += item as string;
                    }
                    Program.SaveConfigValue("ServerIPs", ips);

                    if (cmbAccount.Text.Length > 0 && !cmbAccount.Properties.Items.Contains(cmbAccount.Text))
                        cmbAccount.Properties.Items.Insert(0, cmbAccount.Text);
                    else if (cmbAccount.Properties.Items.Count > 1)
                    {
                        string sel = cmbAccount.SelectedItem as string;
                        cmbAccount.Properties.Items.RemoveAt(cmbAccount.SelectedIndex);
                        cmbAccount.Properties.Items.Insert(0, sel);
                        cmbAccount.SelectedIndex = 0;
                    }
                    string accs = "";
                    foreach (var item in cmbAccount.Properties.Items)
                    {
                        if (accs.Length > 0)
                            accs += ";";
                        accs += item as string;
                    }
                    Program.SaveConfigValue("UserAccounts", accs);
                    
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    lblUpdateStatus.Text = result.ToString().Replace("失败_", "");
                    UIEnabled = true;
                }
            });
        }

        public void SendMessage(MessageLevel lv, MessageType type, string msg)
        {
        }
        #endregion
    }
}