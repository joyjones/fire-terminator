namespace FireTerminator.Common.UI
{
    partial class EncProfileSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EncProfileSelector));
            this.cmbEncodings = new System.Windows.Forms.ComboBox();
            this.bnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbEncodings
            // 
            this.cmbEncodings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEncodings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncodings.FormattingEnabled = true;
            this.cmbEncodings.Location = new System.Drawing.Point(12, 12);
            this.cmbEncodings.Name = "cmbEncodings";
            this.cmbEncodings.Size = new System.Drawing.Size(269, 20);
            this.cmbEncodings.TabIndex = 0;
            this.cmbEncodings.SelectedIndexChanged += new System.EventHandler(this.cmbEncodings_SelectedIndexChanged);
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(206, 38);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 1;
            this.bnOK.Text = "确定";
            this.bnOK.UseVisualStyleBackColor = true;
            // 
            // EncProfileSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 71);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.cmbEncodings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EncProfileSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选择视频编码格式";
            this.Load += new System.EventHandler(this.EncProfileSelector_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbEncodings;
        private System.Windows.Forms.Button bnOK;
    }
}