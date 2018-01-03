namespace FireTerminator.Client
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txbPassword = new DevExpress.XtraEditors.TextEdit();
            this.bnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.cmbAccount = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblUpdateStatus = new DevExpress.XtraEditors.LabelControl();
            this.cmbServerIP = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblServerIP = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txbPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAccount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbServerIP.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPassword
            // 
            this.lblPassword.Location = new System.Drawing.Point(12, 64);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(28, 14);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "密码:";
            // 
            // txbPassword
            // 
            this.txbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPassword.Location = new System.Drawing.Point(86, 61);
            this.txbPassword.Name = "txbPassword";
            this.txbPassword.Properties.PasswordChar = '*';
            this.txbPassword.Size = new System.Drawing.Size(141, 20);
            this.txbPassword.TabIndex = 2;
            // 
            // bnLogin
            // 
            this.bnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnLogin.Location = new System.Drawing.Point(233, 35);
            this.bnLogin.Name = "bnLogin";
            this.bnLogin.Size = new System.Drawing.Size(75, 46);
            this.bnLogin.TabIndex = 3;
            this.bnLogin.Text = "登录";
            this.bnLogin.Click += new System.EventHandler(this.bnLogin_Click);
            // 
            // cmbAccount
            // 
            this.cmbAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAccount.Location = new System.Drawing.Point(86, 35);
            this.cmbAccount.Name = "cmbAccount";
            this.cmbAccount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbAccount.Size = new System.Drawing.Size(141, 20);
            this.cmbAccount.TabIndex = 1;
            // 
            // lblUpdateStatus
            // 
            this.lblUpdateStatus.Appearance.ForeColor = System.Drawing.Color.Gray;
            this.lblUpdateStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblUpdateStatus.Location = new System.Drawing.Point(12, 87);
            this.lblUpdateStatus.Name = "lblUpdateStatus";
            this.lblUpdateStatus.Size = new System.Drawing.Size(296, 34);
            this.lblUpdateStatus.TabIndex = 7;
            this.lblUpdateStatus.Text = "请输入登录信息。";
            // 
            // cmbServerIP
            // 
            this.cmbServerIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbServerIP.Location = new System.Drawing.Point(86, 9);
            this.cmbServerIP.Name = "cmbServerIP";
            this.cmbServerIP.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbServerIP.Size = new System.Drawing.Size(222, 20);
            this.cmbServerIP.TabIndex = 0;
            // 
            // lblServerIP
            // 
            this.lblServerIP.Location = new System.Drawing.Point(12, 12);
            this.lblServerIP.Name = "lblServerIP";
            this.lblServerIP.Size = new System.Drawing.Size(64, 14);
            this.lblServerIP.TabIndex = 4;
            this.lblServerIP.Text = "服务器地址:";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 38);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(28, 14);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "帐号:";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 127);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.cmbServerIP);
            this.Controls.Add(this.lblServerIP);
            this.Controls.Add(this.lblUpdateStatus);
            this.Controls.Add(this.cmbAccount);
            this.Controls.Add(this.bnLogin);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txbPassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "模拟作业系统 登录";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.LoginForm_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txbPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAccount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbServerIP.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblPassword;
        private DevExpress.XtraEditors.TextEdit txbPassword;
        private DevExpress.XtraEditors.SimpleButton bnLogin;
        private DevExpress.XtraEditors.ComboBoxEdit cmbAccount;
        public DevExpress.XtraEditors.LabelControl lblUpdateStatus;
        private DevExpress.XtraEditors.ComboBoxEdit cmbServerIP;
        private DevExpress.XtraEditors.LabelControl lblServerIP;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}