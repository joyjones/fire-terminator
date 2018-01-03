namespace FireTerminator.Client
{
    partial class PassModifyForm
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
            this.txbPswCurrent = new DevExpress.XtraEditors.TextEdit();
            this.txbPswNew = new DevExpress.XtraEditors.TextEdit();
            this.txbPswNewConfirm = new DevExpress.XtraEditors.TextEdit();
            this.bnConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.bnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txbPswCurrent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPswNew.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPswNewConfirm.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txbPswCurrent
            // 
            this.txbPswCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPswCurrent.Location = new System.Drawing.Point(82, 9);
            this.txbPswCurrent.Name = "txbPswCurrent";
            this.txbPswCurrent.Size = new System.Drawing.Size(134, 20);
            this.txbPswCurrent.TabIndex = 0;
            // 
            // txbPswNew
            // 
            this.txbPswNew.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPswNew.Location = new System.Drawing.Point(82, 35);
            this.txbPswNew.Name = "txbPswNew";
            this.txbPswNew.Size = new System.Drawing.Size(134, 20);
            this.txbPswNew.TabIndex = 1;
            // 
            // txbPswNewConfirm
            // 
            this.txbPswNewConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPswNewConfirm.Location = new System.Drawing.Point(82, 61);
            this.txbPswNewConfirm.Name = "txbPswNewConfirm";
            this.txbPswNewConfirm.Size = new System.Drawing.Size(134, 20);
            this.txbPswNewConfirm.TabIndex = 2;
            // 
            // bnConfirm
            // 
            this.bnConfirm.Location = new System.Drawing.Point(90, 87);
            this.bnConfirm.Name = "bnConfirm";
            this.bnConfirm.Size = new System.Drawing.Size(60, 23);
            this.bnConfirm.TabIndex = 3;
            this.bnConfirm.Text = "确定";
            this.bnConfirm.Click += new System.EventHandler(this.bnConfirm_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Location = new System.Drawing.Point(156, 87);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(60, 23);
            this.bnCancel.TabIndex = 4;
            this.bnCancel.Text = "取消";
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(52, 14);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "当前密码:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(12, 38);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(40, 14);
            this.labelControl2.TabIndex = 6;
            this.labelControl2.Text = "新密码:";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(12, 64);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(64, 14);
            this.labelControl3.TabIndex = 7;
            this.labelControl3.Text = "确定新密码:";
            // 
            // PassModifyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 116);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnConfirm);
            this.Controls.Add(this.txbPswNewConfirm);
            this.Controls.Add(this.txbPswNew);
            this.Controls.Add(this.txbPswCurrent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PassModifyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "修改密码";
            ((System.ComponentModel.ISupportInitialize)(this.txbPswCurrent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPswNew.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPswNewConfirm.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txbPswCurrent;
        private DevExpress.XtraEditors.TextEdit txbPswNew;
        private DevExpress.XtraEditors.TextEdit txbPswNewConfirm;
        private DevExpress.XtraEditors.SimpleButton bnConfirm;
        private DevExpress.XtraEditors.SimpleButton bnCancel;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
    }
}