namespace FireTerminator.Server
{
    partial class JudgementSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JudgementSetting));
            this.gdcItems = new DevExpress.XtraGrid.GridControl();
            this.gridView_Items = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_ItemClassID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ItemID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ItemName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ItemScore = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ItemDesc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemPopupContainerEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.grpClasses = new DevExpress.XtraEditors.GroupControl();
            this.bnDelClass = new DevExpress.XtraEditors.SimpleButton();
            this.bnAddClass = new DevExpress.XtraEditors.SimpleButton();
            this.gdcClasses = new DevExpress.XtraGrid.GridControl();
            this.gridView_Classes = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_ClassID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ClassName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ClassScore = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutView1 = new DevExpress.XtraGrid.Views.Layout.LayoutView();
            this.advBandedGridView1 = new DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView();
            this.grpProjItems = new DevExpress.XtraEditors.GroupControl();
            this.bnDelItem = new DevExpress.XtraEditors.SimpleButton();
            this.bnAddItem = new DevExpress.XtraEditors.SimpleButton();
            this.bnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.bnOK = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gdcItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_Items)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupContainerEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpClasses)).BeginInit();
            this.grpClasses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gdcClasses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_Classes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.advBandedGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpProjItems)).BeginInit();
            this.grpProjItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // gdcItems
            // 
            this.gdcItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gdcItems.Location = new System.Drawing.Point(5, 25);
            this.gdcItems.MainView = this.gridView_Items;
            this.gdcItems.Name = "gdcItems";
            this.gdcItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1,
            this.repositoryItemPopupContainerEdit1});
            this.gdcItems.Size = new System.Drawing.Size(640, 222);
            this.gdcItems.TabIndex = 1;
            this.gdcItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_Items});
            // 
            // gridView_Items
            // 
            this.gridView_Items.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_ItemClassID,
            this.gridColumn_ItemID,
            this.gridColumn_ItemName,
            this.gridColumn_ItemScore,
            this.gridColumn_ItemDesc});
            this.gridView_Items.GridControl = this.gdcItems;
            this.gridView_Items.Name = "gridView_Items";
            this.gridView_Items.OptionsView.ShowGroupPanel = false;
            this.gridView_Items.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gridView_Items.OptionsView.ShowIndicator = false;
            this.gridView_Items.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            // 
            // gridColumn_ItemClassID
            // 
            this.gridColumn_ItemClassID.Caption = "类别编号";
            this.gridColumn_ItemClassID.FieldName = "类别编号";
            this.gridColumn_ItemClassID.Name = "gridColumn_ItemClassID";
            this.gridColumn_ItemClassID.OptionsColumn.AllowEdit = false;
            this.gridColumn_ItemClassID.Visible = true;
            this.gridColumn_ItemClassID.VisibleIndex = 0;
            this.gridColumn_ItemClassID.Width = 55;
            // 
            // gridColumn_ItemID
            // 
            this.gridColumn_ItemID.Caption = "项目序号";
            this.gridColumn_ItemID.FieldName = "项目序号";
            this.gridColumn_ItemID.Name = "gridColumn_ItemID";
            this.gridColumn_ItemID.OptionsColumn.AllowEdit = false;
            this.gridColumn_ItemID.Visible = true;
            this.gridColumn_ItemID.VisibleIndex = 1;
            this.gridColumn_ItemID.Width = 55;
            // 
            // gridColumn_ItemName
            // 
            this.gridColumn_ItemName.Caption = "项目名称";
            this.gridColumn_ItemName.FieldName = "项目名称";
            this.gridColumn_ItemName.Name = "gridColumn_ItemName";
            this.gridColumn_ItemName.Visible = true;
            this.gridColumn_ItemName.VisibleIndex = 2;
            this.gridColumn_ItemName.Width = 132;
            // 
            // gridColumn_ItemScore
            // 
            this.gridColumn_ItemScore.Caption = "分数";
            this.gridColumn_ItemScore.FieldName = "分数";
            this.gridColumn_ItemScore.Name = "gridColumn_ItemScore";
            this.gridColumn_ItemScore.Visible = true;
            this.gridColumn_ItemScore.VisibleIndex = 3;
            this.gridColumn_ItemScore.Width = 41;
            // 
            // gridColumn_ItemDesc
            // 
            this.gridColumn_ItemDesc.Caption = "评判说明(以\"：\"号\"；\"号分隔)";
            this.gridColumn_ItemDesc.ColumnEdit = this.repositoryItemPopupContainerEdit1;
            this.gridColumn_ItemDesc.FieldName = "评判说明";
            this.gridColumn_ItemDesc.Name = "gridColumn_ItemDesc";
            this.gridColumn_ItemDesc.Visible = true;
            this.gridColumn_ItemDesc.VisibleIndex = 4;
            this.gridColumn_ItemDesc.Width = 233;
            // 
            // repositoryItemPopupContainerEdit1
            // 
            this.repositoryItemPopupContainerEdit1.AutoHeight = false;
            this.repositoryItemPopupContainerEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemPopupContainerEdit1.Name = "repositoryItemPopupContainerEdit1";
            this.repositoryItemPopupContainerEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // grpClasses
            // 
            this.grpClasses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpClasses.Controls.Add(this.bnDelClass);
            this.grpClasses.Controls.Add(this.bnAddClass);
            this.grpClasses.Controls.Add(this.gdcClasses);
            this.grpClasses.Location = new System.Drawing.Point(2, 2);
            this.grpClasses.Name = "grpClasses";
            this.grpClasses.Size = new System.Drawing.Size(650, 174);
            this.grpClasses.TabIndex = 2;
            this.grpClasses.Text = "评审类别";
            // 
            // bnDelClass
            // 
            this.bnDelClass.Location = new System.Drawing.Point(86, 145);
            this.bnDelClass.Name = "bnDelClass";
            this.bnDelClass.Size = new System.Drawing.Size(75, 23);
            this.bnDelClass.TabIndex = 2;
            this.bnDelClass.Text = "删除类别";
            this.bnDelClass.Click += new System.EventHandler(this.bnDelClass_Click);
            // 
            // bnAddClass
            // 
            this.bnAddClass.Location = new System.Drawing.Point(5, 145);
            this.bnAddClass.Name = "bnAddClass";
            this.bnAddClass.Size = new System.Drawing.Size(75, 23);
            this.bnAddClass.TabIndex = 1;
            this.bnAddClass.Text = "添加类别";
            this.bnAddClass.Click += new System.EventHandler(this.bnAddClass_Click);
            // 
            // gdcClasses
            // 
            this.gdcClasses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gdcClasses.Location = new System.Drawing.Point(5, 25);
            this.gdcClasses.MainView = this.gridView_Classes;
            this.gdcClasses.Name = "gdcClasses";
            this.gdcClasses.Size = new System.Drawing.Size(640, 114);
            this.gdcClasses.TabIndex = 0;
            this.gdcClasses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_Classes,
            this.layoutView1,
            this.advBandedGridView1});
            // 
            // gridView_Classes
            // 
            this.gridView_Classes.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_ClassID,
            this.gridColumn_ClassName,
            this.gridColumn_ClassScore});
            this.gridView_Classes.GridControl = this.gdcClasses;
            this.gridView_Classes.Name = "gridView_Classes";
            this.gridView_Classes.OptionsView.ShowGroupPanel = false;
            this.gridView_Classes.OptionsView.ShowIndicator = false;
            // 
            // gridColumn_ClassID
            // 
            this.gridColumn_ClassID.Caption = "类别编号";
            this.gridColumn_ClassID.FieldName = "类别编号";
            this.gridColumn_ClassID.Name = "gridColumn_ClassID";
            this.gridColumn_ClassID.OptionsColumn.AllowEdit = false;
            this.gridColumn_ClassID.Visible = true;
            this.gridColumn_ClassID.VisibleIndex = 0;
            this.gridColumn_ClassID.Width = 55;
            // 
            // gridColumn_ClassName
            // 
            this.gridColumn_ClassName.Caption = "类别名称";
            this.gridColumn_ClassName.FieldName = "类别名称";
            this.gridColumn_ClassName.Name = "gridColumn_ClassName";
            this.gridColumn_ClassName.Visible = true;
            this.gridColumn_ClassName.VisibleIndex = 1;
            this.gridColumn_ClassName.Width = 230;
            // 
            // gridColumn_ClassScore
            // 
            this.gridColumn_ClassScore.Caption = "类别分值";
            this.gridColumn_ClassScore.FieldName = "类别分值";
            this.gridColumn_ClassScore.Name = "gridColumn_ClassScore";
            this.gridColumn_ClassScore.Visible = true;
            this.gridColumn_ClassScore.VisibleIndex = 2;
            this.gridColumn_ClassScore.Width = 231;
            // 
            // layoutView1
            // 
            this.layoutView1.GridControl = this.gdcClasses;
            this.layoutView1.Name = "layoutView1";
            this.layoutView1.TemplateCard = null;
            // 
            // advBandedGridView1
            // 
            this.advBandedGridView1.GridControl = this.gdcClasses;
            this.advBandedGridView1.Name = "advBandedGridView1";
            this.advBandedGridView1.OptionsView.ShowGroupPanel = false;
            this.advBandedGridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.advBandedGridView1.OptionsView.ShowIndicator = false;
            this.advBandedGridView1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            // 
            // grpProjItems
            // 
            this.grpProjItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProjItems.Controls.Add(this.bnDelItem);
            this.grpProjItems.Controls.Add(this.bnAddItem);
            this.grpProjItems.Controls.Add(this.gdcItems);
            this.grpProjItems.Location = new System.Drawing.Point(2, 182);
            this.grpProjItems.Name = "grpProjItems";
            this.grpProjItems.Size = new System.Drawing.Size(650, 281);
            this.grpProjItems.TabIndex = 1;
            this.grpProjItems.Text = "评审项目";
            // 
            // bnDelItem
            // 
            this.bnDelItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnDelItem.Location = new System.Drawing.Point(86, 253);
            this.bnDelItem.Name = "bnDelItem";
            this.bnDelItem.Size = new System.Drawing.Size(75, 23);
            this.bnDelItem.TabIndex = 4;
            this.bnDelItem.Text = "删除项目";
            this.bnDelItem.Click += new System.EventHandler(this.bnDelItem_Click);
            // 
            // bnAddItem
            // 
            this.bnAddItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnAddItem.Location = new System.Drawing.Point(5, 253);
            this.bnAddItem.Name = "bnAddItem";
            this.bnAddItem.Size = new System.Drawing.Size(75, 23);
            this.bnAddItem.TabIndex = 3;
            this.bnAddItem.Text = "添加项目";
            this.bnAddItem.Click += new System.EventHandler(this.bnAddItem_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(575, 469);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 4;
            this.bnCancel.Text = "取消";
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.Location = new System.Drawing.Point(494, 469);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 3;
            this.bnOK.Text = "确定";
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // JudgementSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 497);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.grpProjItems);
            this.Controls.Add(this.grpClasses);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "JudgementSetting";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "评审设置";
            this.Load += new System.EventHandler(this.JudgementSetting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gdcItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_Items)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupContainerEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpClasses)).EndInit();
            this.grpClasses.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gdcClasses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_Classes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.advBandedGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpProjItems)).EndInit();
            this.grpProjItems.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gdcItems;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_Items;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ItemID;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ItemName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ItemScore;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ItemDesc;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraEditors.GroupControl grpClasses;
        private DevExpress.XtraEditors.GroupControl grpProjItems;
        private DevExpress.XtraEditors.SimpleButton bnDelClass;
        private DevExpress.XtraEditors.SimpleButton bnAddClass;
        private DevExpress.XtraEditors.SimpleButton bnDelItem;
        private DevExpress.XtraEditors.SimpleButton bnAddItem;
        private DevExpress.XtraEditors.SimpleButton bnCancel;
        private DevExpress.XtraEditors.SimpleButton bnOK;
        private DevExpress.XtraGrid.GridControl gdcClasses;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_Classes;
        private DevExpress.XtraGrid.Views.Layout.LayoutView layoutView1;
        private DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView advBandedGridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ClassID;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ClassName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ClassScore;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ItemClassID;
        private DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit repositoryItemPopupContainerEdit1;


    }
}