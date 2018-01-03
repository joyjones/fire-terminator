namespace FireTerminator.Common.UI
{
    partial class MessageControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.bnSendMessage = new DevExpress.XtraEditors.SimpleButton();
            this.cmbStudentsList = new DevExpress.XtraEditors.ComboBoxEdit();
            this.rtxMessages = new System.Windows.Forms.RichTextBox();
            this.cmbMsgColor = new DevExpress.XtraEditors.ColorEdit();
            this.txbMessage = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbStudentsList.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMsgColor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbMessage.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // bnSendMessage
            // 
            this.bnSendMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnSendMessage.Location = new System.Drawing.Point(416, 89);
            this.bnSendMessage.Name = "bnSendMessage";
            this.bnSendMessage.Size = new System.Drawing.Size(38, 23);
            this.bnSendMessage.TabIndex = 10;
            this.bnSendMessage.Text = "发送";
            this.bnSendMessage.Click += new System.EventHandler(this.bnSendMessage_Click);
            // 
            // cmbStudentsList
            // 
            this.cmbStudentsList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbStudentsList.EditValue = "全部学员";
            this.cmbStudentsList.Location = new System.Drawing.Point(3, 91);
            this.cmbStudentsList.Name = "cmbStudentsList";
            this.cmbStudentsList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbStudentsList.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbStudentsList.Size = new System.Drawing.Size(76, 20);
            this.cmbStudentsList.TabIndex = 9;
            this.cmbStudentsList.SelectedIndexChanged += new System.EventHandler(this.cmbStudentsList_SelectedIndexChanged);
            // 
            // rtxMessages
            // 
            this.rtxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxMessages.BackColor = System.Drawing.Color.White;
            this.rtxMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxMessages.Location = new System.Drawing.Point(3, 3);
            this.rtxMessages.Name = "rtxMessages";
            this.rtxMessages.ReadOnly = true;
            this.rtxMessages.Size = new System.Drawing.Size(451, 82);
            this.rtxMessages.TabIndex = 8;
            this.rtxMessages.Text = "";
            // 
            // cmbMsgColor
            // 
            this.cmbMsgColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbMsgColor.EditValue = System.Drawing.Color.Black;
            this.cmbMsgColor.Location = new System.Drawing.Point(85, 91);
            this.cmbMsgColor.Name = "cmbMsgColor";
            this.cmbMsgColor.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.cmbMsgColor.Properties.Appearance.Options.UseBackColor = true;
            this.cmbMsgColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbMsgColor.Size = new System.Drawing.Size(96, 20);
            this.cmbMsgColor.TabIndex = 7;
            this.cmbMsgColor.EditValueChanged += new System.EventHandler(this.cmbMsgColor_EditValueChanged);
            // 
            // txbMessage
            // 
            this.txbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txbMessage.Location = new System.Drawing.Point(187, 91);
            this.txbMessage.Name = "txbMessage";
            this.txbMessage.Size = new System.Drawing.Size(223, 20);
            this.txbMessage.TabIndex = 6;
            this.txbMessage.EditValueChanged += new System.EventHandler(this.txbMessage_EditValueChanged);
            this.txbMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txbMessage_KeyDown);
            // 
            // MessageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbMsgColor);
            this.Controls.Add(this.bnSendMessage);
            this.Controls.Add(this.cmbStudentsList);
            this.Controls.Add(this.rtxMessages);
            this.Controls.Add(this.txbMessage);
            this.Name = "MessageControl";
            this.Size = new System.Drawing.Size(457, 119);
            ((System.ComponentModel.ISupportInitialize)(this.cmbStudentsList.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMsgColor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbMessage.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton bnSendMessage;
        private DevExpress.XtraEditors.ComboBoxEdit cmbStudentsList;
        private System.Windows.Forms.RichTextBox rtxMessages;
        private DevExpress.XtraEditors.ColorEdit cmbMsgColor;
        private DevExpress.XtraEditors.TextEdit txbMessage;
    }
}
