namespace FireTerminator.Editor
{
    partial class PlayControlForm
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
            this.bnQuit = new System.Windows.Forms.Button();
            this.chkPlay = new System.Windows.Forms.CheckBox();
            this.chkRecord = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // bnQuit
            // 
            this.bnQuit.BackColor = System.Drawing.Color.WhiteSmoke;
            this.bnQuit.Location = new System.Drawing.Point(129, 6);
            this.bnQuit.Name = "bnQuit";
            this.bnQuit.Size = new System.Drawing.Size(55, 23);
            this.bnQuit.TabIndex = 2;
            this.bnQuit.Text = "退出";
            this.bnQuit.UseVisualStyleBackColor = false;
            this.bnQuit.Click += new System.EventHandler(this.bnQuit_Click);
            // 
            // chkPlay
            // 
            this.chkPlay.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkPlay.BackColor = System.Drawing.Color.WhiteSmoke;
            this.chkPlay.Location = new System.Drawing.Point(7, 6);
            this.chkPlay.Name = "chkPlay";
            this.chkPlay.Size = new System.Drawing.Size(55, 23);
            this.chkPlay.TabIndex = 3;
            this.chkPlay.Text = "播放";
            this.chkPlay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkPlay.UseVisualStyleBackColor = false;
            this.chkPlay.CheckedChanged += new System.EventHandler(this.chkPlay_CheckedChanged);
            // 
            // chkRecord
            // 
            this.chkRecord.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkRecord.BackColor = System.Drawing.Color.WhiteSmoke;
            this.chkRecord.Location = new System.Drawing.Point(68, 6);
            this.chkRecord.Name = "chkRecord";
            this.chkRecord.Size = new System.Drawing.Size(55, 23);
            this.chkRecord.TabIndex = 4;
            this.chkRecord.Text = "录制";
            this.chkRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkRecord.UseVisualStyleBackColor = false;
            this.chkRecord.CheckedChanged += new System.EventHandler(this.chkRecord_CheckedChanged);
            // 
            // PlayControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 34);
            this.ControlBox = false;
            this.Controls.Add(this.chkRecord);
            this.Controls.Add(this.chkPlay);
            this.Controls.Add(this.bnQuit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlayControlForm";
            this.ShowInTaskbar = false;
            this.Text = "播放控制";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.PlayControlForm_Shown);
            this.VisibleChanged += new System.EventHandler(this.PlayControlForm_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnQuit;
        private System.Windows.Forms.CheckBox chkPlay;
        private System.Windows.Forms.CheckBox chkRecord;
    }
}