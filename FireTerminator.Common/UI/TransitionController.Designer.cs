namespace FireTerminator.Common.UI
{
    partial class TransitionController
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransitionController));
            this.pnlDraw = new System.Windows.Forms.Panel();
            this.ctxMnuTransView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddTransition = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveTransition = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyTrans = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCutTrans = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPasteTrans = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiClearLine = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearAll = new System.Windows.Forms.ToolStripMenuItem();
            this.trvTransHeaders = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.chkAnimPlay = new DevExpress.XtraEditors.CheckButton();
            this.pnlMain = new DevExpress.XtraEditors.XtraScrollableControl();
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.ctxMnuTransView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trvTransHeaders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlDraw
            // 
            this.pnlDraw.BackColor = System.Drawing.Color.Black;
            this.pnlDraw.ContextMenuStrip = this.ctxMnuTransView;
            this.pnlDraw.Location = new System.Drawing.Point(0, 0);
            this.pnlDraw.Name = "pnlDraw";
            this.pnlDraw.Size = new System.Drawing.Size(433, 131);
            this.pnlDraw.TabIndex = 0;
            this.pnlDraw.MouseLeave += new System.EventHandler(this.pnlDraw_MouseLeave);
            this.pnlDraw.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.pnlDraw_PreviewKeyDown);
            this.pnlDraw.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlDraw_MouseMove);
            this.pnlDraw.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlDraw_MouseDown);
            this.pnlDraw.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlDraw_MouseUp);
            // 
            // ctxMnuTransView
            // 
            this.ctxMnuTransView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddTransition,
            this.tsmiRemoveTransition,
            this.toolStripMenuItem1,
            this.tsmiCopyTrans,
            this.tsmiCutTrans,
            this.tsmiPasteTrans,
            this.toolStripMenuItem2,
            this.tsmiClearLine,
            this.tsmiClearAll});
            this.ctxMnuTransView.Name = "ctxMnuTransView";
            this.ctxMnuTransView.Size = new System.Drawing.Size(165, 170);
            this.ctxMnuTransView.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMnuTransView_Opening);
            // 
            // tsmiAddTransition
            // 
            this.tsmiAddTransition.Image = ((System.Drawing.Image)(resources.GetObject("tsmiAddTransition.Image")));
            this.tsmiAddTransition.Name = "tsmiAddTransition";
            this.tsmiAddTransition.Size = new System.Drawing.Size(164, 22);
            this.tsmiAddTransition.Text = "添加变换";
            this.tsmiAddTransition.Click += new System.EventHandler(this.tsmiAddTransition_Click);
            // 
            // tsmiRemoveTransition
            // 
            this.tsmiRemoveTransition.Image = global::FireTerminator.Common.Properties.Resources.Delete_16x16;
            this.tsmiRemoveTransition.Name = "tsmiRemoveTransition";
            this.tsmiRemoveTransition.Size = new System.Drawing.Size(164, 22);
            this.tsmiRemoveTransition.Text = "删除变换";
            this.tsmiRemoveTransition.Click += new System.EventHandler(this.tsmiRemoveTransition_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 6);
            // 
            // tsmiCopyTrans
            // 
            this.tsmiCopyTrans.Image = ((System.Drawing.Image)(resources.GetObject("tsmiCopyTrans.Image")));
            this.tsmiCopyTrans.Name = "tsmiCopyTrans";
            this.tsmiCopyTrans.Size = new System.Drawing.Size(164, 22);
            this.tsmiCopyTrans.Text = "复制";
            this.tsmiCopyTrans.Click += new System.EventHandler(this.tsmiCopyTrans_Click);
            // 
            // tsmiCutTrans
            // 
            this.tsmiCutTrans.Image = ((System.Drawing.Image)(resources.GetObject("tsmiCutTrans.Image")));
            this.tsmiCutTrans.Name = "tsmiCutTrans";
            this.tsmiCutTrans.Size = new System.Drawing.Size(164, 22);
            this.tsmiCutTrans.Text = "剪切";
            this.tsmiCutTrans.Click += new System.EventHandler(this.tsmiCutTrans_Click);
            // 
            // tsmiPasteTrans
            // 
            this.tsmiPasteTrans.Image = ((System.Drawing.Image)(resources.GetObject("tsmiPasteTrans.Image")));
            this.tsmiPasteTrans.Name = "tsmiPasteTrans";
            this.tsmiPasteTrans.Size = new System.Drawing.Size(164, 22);
            this.tsmiPasteTrans.Text = "粘贴";
            this.tsmiPasteTrans.Click += new System.EventHandler(this.tsmiPasteTrans_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 6);
            // 
            // tsmiClearLine
            // 
            this.tsmiClearLine.Image = ((System.Drawing.Image)(resources.GetObject("tsmiClearLine.Image")));
            this.tsmiClearLine.Name = "tsmiClearLine";
            this.tsmiClearLine.Size = new System.Drawing.Size(164, 22);
            this.tsmiClearLine.Text = "清除所有XX变换";
            this.tsmiClearLine.Click += new System.EventHandler(this.tsmiClearLine_Click);
            // 
            // tsmiClearAll
            // 
            this.tsmiClearAll.Image = global::FireTerminator.Common.Properties.Resources.Deleted;
            this.tsmiClearAll.Name = "tsmiClearAll";
            this.tsmiClearAll.Size = new System.Drawing.Size(164, 22);
            this.tsmiClearAll.Text = "清除所有变换";
            this.tsmiClearAll.Click += new System.EventHandler(this.tsmiClearAll_Click);
            // 
            // trvTransHeaders
            // 
            this.trvTransHeaders.Appearance.FocusedCell.BackColor = System.Drawing.Color.Silver;
            this.trvTransHeaders.Appearance.FocusedCell.Options.UseBackColor = true;
            this.trvTransHeaders.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1});
            this.trvTransHeaders.ContextMenuStrip = this.ctxMnuTransView;
            this.trvTransHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvTransHeaders.Location = new System.Drawing.Point(0, 18);
            this.trvTransHeaders.Name = "trvTransHeaders";
            this.trvTransHeaders.BeginUnboundLoad();
            this.trvTransHeaders.AppendNode(new object[] {
            "隐藏"}, -1);
            this.trvTransHeaders.AppendNode(new object[] {
            "位移"}, -1);
            this.trvTransHeaders.AppendNode(new object[] {
            "缩放"}, -1);
            this.trvTransHeaders.AppendNode(new object[] {
            "旋转"}, -1);
            this.trvTransHeaders.AppendNode(new object[] {
            "半透"}, -1);
            this.trvTransHeaders.EndUnboundLoad();
            this.trvTransHeaders.OptionsBehavior.Editable = false;
            this.trvTransHeaders.OptionsBehavior.ResizeNodes = false;
            this.trvTransHeaders.OptionsPrint.UsePrintStyles = true;
            this.trvTransHeaders.OptionsView.ShowColumns = false;
            this.trvTransHeaders.OptionsView.ShowFilterPanelMode = DevExpress.XtraTreeList.ShowFilterPanelMode.Never;
            this.trvTransHeaders.OptionsView.ShowIndicator = false;
            this.trvTransHeaders.OptionsView.ShowRoot = false;
            this.trvTransHeaders.OptionsView.ShowVertLines = false;
            this.trvTransHeaders.RowHeight = 20;
            this.trvTransHeaders.SelectImageList = this.imageCollection1;
            this.trvTransHeaders.Size = new System.Drawing.Size(73, 116);
            this.trvTransHeaders.TabIndex = 1;
            this.trvTransHeaders.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.trvTransHeaders_PreviewKeyDown);
            this.trvTransHeaders.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.trvTransHeaders_FocusedNodeChanged);
            this.trvTransHeaders.SizeChanged += new System.EventHandler(this.trvTransHeaders_SizeChanged);
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "属性";
            this.treeListColumn1.FieldName = "属性";
            this.treeListColumn1.MinWidth = 33;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.Images.SetKeyName(0, "Movie_16x16.png");
            this.imageCollection1.Images.SetKeyName(1, "NewViaWizard_16x16.png");
            this.imageCollection1.Images.SetKeyName(2, "note-16x16.png");
            this.imageCollection1.Images.SetKeyName(3, "scene16.png");
            this.imageCollection1.Images.SetKeyName(4, "Undo_16x16.png");
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.trvTransHeaders);
            this.splitContainerControl1.Panel1.Controls.Add(this.chkAnimPlay);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.pnlMain);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(514, 134);
            this.splitContainerControl1.SplitterPosition = 73;
            this.splitContainerControl1.TabIndex = 2;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // chkAnimPlay
            // 
            this.chkAnimPlay.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.chkAnimPlay.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.chkAnimPlay.Appearance.Options.UseBackColor = true;
            this.chkAnimPlay.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.chkAnimPlay.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkAnimPlay.Location = new System.Drawing.Point(0, 0);
            this.chkAnimPlay.Name = "chkAnimPlay";
            this.chkAnimPlay.Size = new System.Drawing.Size(73, 18);
            this.chkAnimPlay.TabIndex = 2;
            this.chkAnimPlay.Text = "00:00:000";
            this.chkAnimPlay.CheckedChanged += new System.EventHandler(this.chkAnimPlay_CheckedChanged);
            // 
            // pnlMain
            // 
            this.pnlMain.AllowTouchScroll = true;
            this.pnlMain.Controls.Add(this.pnlDraw);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FireScrollEventOnMouseWheel = true;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(436, 134);
            this.pnlMain.TabIndex = 1;
            // 
            // tmUpdate
            // 
            this.tmUpdate.Enabled = true;
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // TransitionController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "TransitionController";
            this.Size = new System.Drawing.Size(514, 134);
            this.Load += new System.EventHandler(this.TransitionController_Load);
            this.EnabledChanged += new System.EventHandler(this.TransitionController_EnabledChanged);
            this.ctxMnuTransView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trvTransHeaders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList trvTransHeaders;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private System.Windows.Forms.Panel pnlDraw;
        private System.Windows.Forms.ContextMenuStrip ctxMnuTransView;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddTransition;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveTransition;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearLine;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearAll;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyTrans;
        private System.Windows.Forms.ToolStripMenuItem tsmiCutTrans;
        private System.Windows.Forms.ToolStripMenuItem tsmiPasteTrans;
        private DevExpress.XtraEditors.CheckButton chkAnimPlay;
        private DevExpress.XtraEditors.XtraScrollableControl pnlMain;
    }
}
