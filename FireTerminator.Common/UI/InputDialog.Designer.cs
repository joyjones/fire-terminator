namespace FireTerminator.Common.UI
{
    partial class InputDialog
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
            this.txbInput = new DevExpress.XtraEditors.TextEdit();
            this.lblCaption = new DevExpress.XtraEditors.LabelControl();
            this.bnOK = new DevExpress.XtraEditors.SimpleButton();
            this.bnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txbInput.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txbInput
            // 
            this.txbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txbInput.Location = new System.Drawing.Point(88, 9);
            this.txbInput.Name = "txbInput";
            this.txbInput.Size = new System.Drawing.Size(209, 20);
            this.txbInput.TabIndex = 0;
            this.txbInput.EditValueChanged += new System.EventHandler(this.txbInput_EditValueChanged);
            // 
            // lblCaption
            // 
            this.lblCaption.Location = new System.Drawing.Point(12, 12);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(28, 14);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "名称:";
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.Location = new System.Drawing.Point(161, 35);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(65, 23);
            this.bnOK.TabIndex = 2;
            this.bnOK.Text = "确定";
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.Location = new System.Drawing.Point(232, 35);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(65, 23);
            this.bnCancel.TabIndex = 3;
            this.bnCancel.Text = "取消";
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // InputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 70);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.txbInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "请输入";
            this.Load += new System.EventHandler(this.InputDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txbInput.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txbInput;
        private DevExpress.XtraEditors.LabelControl lblCaption;
        private DevExpress.XtraEditors.SimpleButton bnOK;
        private DevExpress.XtraEditors.SimpleButton bnCancel;


    }
}