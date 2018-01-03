namespace FireTerminator.Launch
{
    partial class StartupForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupForm));
            this.bnStartupClient = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.bnStartupServer = new System.Windows.Forms.Button();
            this.bnStartupEditor = new System.Windows.Forms.Button();
            this.bnQuit = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.bnStartTestClients = new System.Windows.Forms.Button();
            this.bnStartupJudgeClient = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bnStartupClient
            // 
            this.bnStartupClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnStartupClient.ImageIndex = 2;
            this.bnStartupClient.ImageList = this.imageList1;
            this.bnStartupClient.Location = new System.Drawing.Point(69, 403);
            this.bnStartupClient.Name = "bnStartupClient";
            this.bnStartupClient.Size = new System.Drawing.Size(100, 60);
            this.bnStartupClient.TabIndex = 0;
            this.bnStartupClient.Text = "模拟作业系统";
            this.bnStartupClient.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bnStartupClient.UseVisualStyleBackColor = true;
            this.bnStartupClient.Click += new System.EventHandler(this.bnStartupClient_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Server");
            this.imageList1.Images.SetKeyName(1, "Editor");
            this.imageList1.Images.SetKeyName(2, "Client");
            this.imageList1.Images.SetKeyName(3, "Employee_32x32.png");
            this.imageList1.Images.SetKeyName(4, "Close_16x16.png");
            // 
            // bnStartupServer
            // 
            this.bnStartupServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnStartupServer.ImageIndex = 0;
            this.bnStartupServer.ImageList = this.imageList1;
            this.bnStartupServer.Location = new System.Drawing.Point(281, 403);
            this.bnStartupServer.Name = "bnStartupServer";
            this.bnStartupServer.Size = new System.Drawing.Size(100, 60);
            this.bnStartupServer.TabIndex = 1;
            this.bnStartupServer.Text = "监督控制系统";
            this.bnStartupServer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bnStartupServer.UseVisualStyleBackColor = true;
            this.bnStartupServer.Click += new System.EventHandler(this.bnStartupServer_Click);
            // 
            // bnStartupEditor
            // 
            this.bnStartupEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnStartupEditor.ImageIndex = 1;
            this.bnStartupEditor.ImageList = this.imageList1;
            this.bnStartupEditor.Location = new System.Drawing.Point(387, 403);
            this.bnStartupEditor.Name = "bnStartupEditor";
            this.bnStartupEditor.Size = new System.Drawing.Size(100, 60);
            this.bnStartupEditor.TabIndex = 2;
            this.bnStartupEditor.Text = "任务编辑系统";
            this.bnStartupEditor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bnStartupEditor.UseVisualStyleBackColor = true;
            this.bnStartupEditor.Click += new System.EventHandler(this.bnStartupEditor_Click);
            // 
            // bnQuit
            // 
            this.bnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnQuit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bnQuit.ImageIndex = 0;
            this.bnQuit.ImageList = this.imageList2;
            this.bnQuit.Location = new System.Drawing.Point(12, 495);
            this.bnQuit.Name = "bnQuit";
            this.bnQuit.Size = new System.Drawing.Size(62, 32);
            this.bnQuit.TabIndex = 3;
            this.bnQuit.Text = "退出";
            this.bnQuit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bnQuit.UseVisualStyleBackColor = true;
            this.bnQuit.Click += new System.EventHandler(this.bnQuit_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Close_16x16.png");
            // 
            // bnStartTestClients
            // 
            this.bnStartTestClients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnStartTestClients.ImageIndex = 2;
            this.bnStartTestClients.ImageList = this.imageList1;
            this.bnStartTestClients.Location = new System.Drawing.Point(650, 403);
            this.bnStartTestClients.Name = "bnStartTestClients";
            this.bnStartTestClients.Size = new System.Drawing.Size(100, 60);
            this.bnStartTestClients.TabIndex = 5;
            this.bnStartTestClients.Text = "启动50个测试客户端";
            this.bnStartTestClients.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bnStartTestClients.UseVisualStyleBackColor = true;
            this.bnStartTestClients.Visible = false;
            this.bnStartTestClients.Click += new System.EventHandler(this.bnStartTestClients_Click);
            // 
            // bnStartupJudgeClient
            // 
            this.bnStartupJudgeClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnStartupJudgeClient.ImageIndex = 3;
            this.bnStartupJudgeClient.ImageList = this.imageList1;
            this.bnStartupJudgeClient.Location = new System.Drawing.Point(175, 403);
            this.bnStartupJudgeClient.Name = "bnStartupJudgeClient";
            this.bnStartupJudgeClient.Size = new System.Drawing.Size(100, 60);
            this.bnStartupJudgeClient.TabIndex = 6;
            this.bnStartupJudgeClient.Text = "专家评判系统";
            this.bnStartupJudgeClient.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bnStartupJudgeClient.UseVisualStyleBackColor = true;
            this.bnStartupJudgeClient.Click += new System.EventHandler(this.bnStartupJudgeClient_Click);
            // 
            // StartupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::FireTerminator.Launch.Properties.Resources.download;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(762, 539);
            this.Controls.Add(this.bnStartupJudgeClient);
            this.Controls.Add(this.bnStartTestClients);
            this.Controls.Add(this.bnQuit);
            this.Controls.Add(this.bnStartupEditor);
            this.Controls.Add(this.bnStartupServer);
            this.Controls.Add(this.bnStartupClient);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StartupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "分布式灭火救援虚拟训练系统 启动器";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.StartupForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnStartupClient;
        private System.Windows.Forms.Button bnStartupServer;
        private System.Windows.Forms.Button bnStartupEditor;
        private System.Windows.Forms.Button bnQuit;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button bnStartTestClients;
        private System.Windows.Forms.Button bnStartupJudgeClient;
        private System.Windows.Forms.ImageList imageList2;
    }
}

