using DevExpress.XtraBars.Ribbon;
namespace FireTerminator.Common.UI
{
    partial class PrintViewControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.ddbDuplex = new DevExpress.XtraEditors.DropDownButton();
            this.printerLabel = new FireTerminator.Common.UI.BackstageViewLabel();
            this.ddbPrinter = new DevExpress.XtraEditors.DropDownButton();
            this.copySpinEdit = new DevExpress.XtraEditors.SpinEdit();
            this.printButton = new DevExpress.XtraEditors.SimpleButton();
            this.backstageViewLabel1 = new FireTerminator.Common.UI.BackstageViewLabel();
            this.backstageViewLabel2 = new FireTerminator.Common.UI.BackstageViewLabel();
            this.ddbOrientation = new DevExpress.XtraEditors.DropDownButton();
            this.ddbPaperSize = new DevExpress.XtraEditors.DropDownButton();
            this.ddbMargins = new DevExpress.XtraEditors.DropDownButton();
            this.ddbCollate = new DevExpress.XtraEditors.DropDownButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPrintButton = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOrientation = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMargins = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPaperSize = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPrintLabel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCopiesSpinEdit = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciSettingsLabel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPrinterLabel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPrinter = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDuplex = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCollated = new DevExpress.XtraLayout.LayoutControlItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.printControl1 = new DevExpress.XtraPrinting.Control.PrintControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.zoomTextEdit = new DevExpress.XtraEditors.TextEdit();
            this.pageButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
            this.panel2 = new System.Windows.Forms.Panel();
            this.zoomTrackBarControl1 = new DevExpress.XtraEditors.ZoomTrackBarControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.copySpinEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrintButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOrientation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMargins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaperSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrintLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCopiesSpinEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSettingsLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrinterLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrinter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDuplex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCollated)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pageButtonEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBarControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBarControl1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.layoutControl1);
            this.splitContainer1.Panel1MinSize = 280;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.labelControl4);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer1.Size = new System.Drawing.Size(848, 556);
            this.splitContainer1.SplitterDistance = 298;
            this.splitContainer1.TabIndex = 0;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.ddbDuplex);
            this.layoutControl1.Controls.Add(this.printerLabel);
            this.layoutControl1.Controls.Add(this.ddbPrinter);
            this.layoutControl1.Controls.Add(this.copySpinEdit);
            this.layoutControl1.Controls.Add(this.printButton);
            this.layoutControl1.Controls.Add(this.backstageViewLabel1);
            this.layoutControl1.Controls.Add(this.backstageViewLabel2);
            this.layoutControl1.Controls.Add(this.ddbOrientation);
            this.layoutControl1.Controls.Add(this.ddbPaperSize);
            this.layoutControl1.Controls.Add(this.ddbMargins);
            this.layoutControl1.Controls.Add(this.ddbCollate);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(532, 478, 648, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(298, 556);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // ddbDuplex
            // 
            this.ddbDuplex.Appearance.Options.UseTextOptions = true;
            this.ddbDuplex.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ddbDuplex.Location = new System.Drawing.Point(12, 220);
            this.ddbDuplex.Name = "ddbDuplex";
            this.ddbDuplex.Size = new System.Drawing.Size(274, 52);
            this.ddbDuplex.StyleController = this.layoutControl1;
            this.ddbDuplex.TabIndex = 4;
            this.ddbDuplex.Text = "单面打印";
            // 
            // printerLabel
            // 
            this.printerLabel.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.printerLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.printerLabel.LineLocation = DevExpress.XtraEditors.LineLocation.Bottom;
            this.printerLabel.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.printerLabel.LineVisible = true;
            this.printerLabel.Location = new System.Drawing.Point(12, 92);
            this.printerLabel.Name = "printerLabel";
            this.printerLabel.ShowLineShadow = false;
            this.printerLabel.Size = new System.Drawing.Size(274, 32);
            this.printerLabel.StyleController = this.layoutControl1;
            this.printerLabel.TabIndex = 9;
            this.printerLabel.Text = "打印机";
            // 
            // ddbPrinter
            // 
            this.ddbPrinter.Appearance.Options.UseTextOptions = true;
            this.ddbPrinter.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ddbPrinter.Location = new System.Drawing.Point(12, 128);
            this.ddbPrinter.Name = "ddbPrinter";
            this.ddbPrinter.Size = new System.Drawing.Size(274, 52);
            this.ddbPrinter.StyleController = this.layoutControl1;
            this.ddbPrinter.TabIndex = 8;
            this.ddbPrinter.Text = "打印机";
            // 
            // copySpinEdit
            // 
            this.copySpinEdit.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.copySpinEdit.Location = new System.Drawing.Point(146, 48);
            this.copySpinEdit.Name = "copySpinEdit";
            this.copySpinEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.copySpinEdit.Properties.MaxValue = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.copySpinEdit.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.copySpinEdit.Size = new System.Drawing.Size(108, 20);
            this.copySpinEdit.StyleController = this.layoutControl1;
            this.copySpinEdit.TabIndex = 6;
            // 
            // printButton
            // 
            this.printButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.TopCenter;
            this.printButton.Location = new System.Drawing.Point(12, 12);
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size(80, 76);
            this.printButton.StyleController = this.layoutControl1;
            this.printButton.TabIndex = 4;
            this.printButton.Text = "打印";
            // 
            // backstageViewLabel1
            // 
            this.backstageViewLabel1.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.backstageViewLabel1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.backstageViewLabel1.LineLocation = DevExpress.XtraEditors.LineLocation.Bottom;
            this.backstageViewLabel1.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.backstageViewLabel1.LineVisible = true;
            this.backstageViewLabel1.Location = new System.Drawing.Point(96, 12);
            this.backstageViewLabel1.Name = "backstageViewLabel1";
            this.backstageViewLabel1.Padding = new System.Windows.Forms.Padding(20, 18, 20, 18);
            this.backstageViewLabel1.ShowLineShadow = false;
            this.backstageViewLabel1.Size = new System.Drawing.Size(190, 32);
            this.backstageViewLabel1.StyleController = this.layoutControl1;
            this.backstageViewLabel1.TabIndex = 5;
            this.backstageViewLabel1.Text = "打印";
            // 
            // backstageViewLabel2
            // 
            this.backstageViewLabel2.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.backstageViewLabel2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.backstageViewLabel2.LineLocation = DevExpress.XtraEditors.LineLocation.Bottom;
            this.backstageViewLabel2.LineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.backstageViewLabel2.LineVisible = true;
            this.backstageViewLabel2.Location = new System.Drawing.Point(12, 184);
            this.backstageViewLabel2.Name = "backstageViewLabel2";
            this.backstageViewLabel2.ShowLineShadow = false;
            this.backstageViewLabel2.Size = new System.Drawing.Size(274, 32);
            this.backstageViewLabel2.StyleController = this.layoutControl1;
            this.backstageViewLabel2.TabIndex = 7;
            this.backstageViewLabel2.Text = "设置";
            // 
            // ddbOrientation
            // 
            this.ddbOrientation.Appearance.Options.UseTextOptions = true;
            this.ddbOrientation.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ddbOrientation.Location = new System.Drawing.Point(12, 332);
            this.ddbOrientation.Name = "ddbOrientation";
            this.ddbOrientation.Size = new System.Drawing.Size(274, 52);
            this.ddbOrientation.StyleController = this.layoutControl1;
            this.ddbOrientation.TabIndex = 0;
            this.ddbOrientation.Text = "纵向";
            // 
            // ddbPaperSize
            // 
            this.ddbPaperSize.Appearance.Options.UseTextOptions = true;
            this.ddbPaperSize.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ddbPaperSize.Location = new System.Drawing.Point(12, 444);
            this.ddbPaperSize.Name = "ddbPaperSize";
            this.ddbPaperSize.Size = new System.Drawing.Size(274, 52);
            this.ddbPaperSize.StyleController = this.layoutControl1;
            this.ddbPaperSize.TabIndex = 2;
            this.ddbPaperSize.Text = "页面尺寸";
            // 
            // ddbMargins
            // 
            this.ddbMargins.Appearance.Options.UseTextOptions = true;
            this.ddbMargins.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ddbMargins.Location = new System.Drawing.Point(12, 388);
            this.ddbMargins.Name = "ddbMargins";
            this.ddbMargins.Size = new System.Drawing.Size(274, 52);
            this.ddbMargins.StyleController = this.layoutControl1;
            this.ddbMargins.TabIndex = 1;
            this.ddbMargins.Text = "边距";
            // 
            // ddbCollate
            // 
            this.ddbCollate.Appearance.Options.UseTextOptions = true;
            this.ddbCollate.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ddbCollate.Location = new System.Drawing.Point(12, 276);
            this.ddbCollate.Name = "ddbCollate";
            this.ddbCollate.Size = new System.Drawing.Size(274, 52);
            this.ddbCollate.StyleController = this.layoutControl1;
            this.ddbCollate.TabIndex = 3;
            this.ddbCollate.Text = "调整";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPrintButton,
            this.lciOrientation,
            this.lciMargins,
            this.lciPaperSize,
            this.lciPrintLabel,
            this.lciCopiesSpinEdit,
            this.emptySpaceItem1,
            this.lciSettingsLabel,
            this.lciPrinterLabel,
            this.lciPrinter,
            this.lciDuplex,
            this.lciCollated});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(298, 556);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciPrintButton
            // 
            this.lciPrintButton.Control = this.printButton;
            this.lciPrintButton.CustomizationFormText = "lciPrintButton";
            this.lciPrintButton.Location = new System.Drawing.Point(0, 0);
            this.lciPrintButton.MaxSize = new System.Drawing.Size(84, 80);
            this.lciPrintButton.MinSize = new System.Drawing.Size(84, 80);
            this.lciPrintButton.Name = "lciPrintButton";
            this.lciPrintButton.Size = new System.Drawing.Size(84, 80);
            this.lciPrintButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPrintButton.Text = "lciPrintButton";
            this.lciPrintButton.TextSize = new System.Drawing.Size(0, 0);
            this.lciPrintButton.TextToControlDistance = 0;
            this.lciPrintButton.TextVisible = false;
            // 
            // lciOrientation
            // 
            this.lciOrientation.Control = this.ddbOrientation;
            this.lciOrientation.CustomizationFormText = "lciOrientation";
            this.lciOrientation.Location = new System.Drawing.Point(0, 320);
            this.lciOrientation.MaxSize = new System.Drawing.Size(0, 56);
            this.lciOrientation.MinSize = new System.Drawing.Size(100, 56);
            this.lciOrientation.Name = "lciOrientation";
            this.lciOrientation.Size = new System.Drawing.Size(278, 56);
            this.lciOrientation.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciOrientation.Text = "lciOrientation";
            this.lciOrientation.TextSize = new System.Drawing.Size(0, 0);
            this.lciOrientation.TextToControlDistance = 0;
            this.lciOrientation.TextVisible = false;
            // 
            // lciMargins
            // 
            this.lciMargins.Control = this.ddbMargins;
            this.lciMargins.CustomizationFormText = "lciMargins";
            this.lciMargins.Location = new System.Drawing.Point(0, 376);
            this.lciMargins.MaxSize = new System.Drawing.Size(0, 56);
            this.lciMargins.MinSize = new System.Drawing.Size(100, 56);
            this.lciMargins.Name = "lciMargins";
            this.lciMargins.Size = new System.Drawing.Size(278, 56);
            this.lciMargins.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciMargins.Text = "lciMargins";
            this.lciMargins.TextSize = new System.Drawing.Size(0, 0);
            this.lciMargins.TextToControlDistance = 0;
            this.lciMargins.TextVisible = false;
            // 
            // lciPaperSize
            // 
            this.lciPaperSize.Control = this.ddbPaperSize;
            this.lciPaperSize.CustomizationFormText = "lciPaperSize";
            this.lciPaperSize.Location = new System.Drawing.Point(0, 432);
            this.lciPaperSize.MaxSize = new System.Drawing.Size(0, 56);
            this.lciPaperSize.MinSize = new System.Drawing.Size(100, 56);
            this.lciPaperSize.Name = "lciPaperSize";
            this.lciPaperSize.Size = new System.Drawing.Size(278, 104);
            this.lciPaperSize.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPaperSize.Text = "lciPaperSize";
            this.lciPaperSize.TextSize = new System.Drawing.Size(0, 0);
            this.lciPaperSize.TextToControlDistance = 0;
            this.lciPaperSize.TextVisible = false;
            // 
            // lciPrintLabel
            // 
            this.lciPrintLabel.Control = this.backstageViewLabel1;
            this.lciPrintLabel.CustomizationFormText = "lciPrintLabel";
            this.lciPrintLabel.Location = new System.Drawing.Point(84, 0);
            this.lciPrintLabel.MaxSize = new System.Drawing.Size(0, 36);
            this.lciPrintLabel.MinSize = new System.Drawing.Size(14, 36);
            this.lciPrintLabel.Name = "lciPrintLabel";
            this.lciPrintLabel.Size = new System.Drawing.Size(194, 36);
            this.lciPrintLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPrintLabel.Text = "lciPrintLabel";
            this.lciPrintLabel.TextSize = new System.Drawing.Size(0, 0);
            this.lciPrintLabel.TextToControlDistance = 0;
            this.lciPrintLabel.TextVisible = false;
            // 
            // lciCopiesSpinEdit
            // 
            this.lciCopiesSpinEdit.Control = this.copySpinEdit;
            this.lciCopiesSpinEdit.CustomizationFormText = "Copies:";
            this.lciCopiesSpinEdit.Location = new System.Drawing.Point(84, 36);
            this.lciCopiesSpinEdit.MaxSize = new System.Drawing.Size(180, 24);
            this.lciCopiesSpinEdit.MinSize = new System.Drawing.Size(180, 24);
            this.lciCopiesSpinEdit.Name = "lciCopiesSpinEdit";
            this.lciCopiesSpinEdit.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 20, 2, 2);
            this.lciCopiesSpinEdit.Size = new System.Drawing.Size(180, 44);
            this.lciCopiesSpinEdit.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCopiesSpinEdit.Text = "份数:";
            this.lciCopiesSpinEdit.TextSize = new System.Drawing.Size(28, 14);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(264, 36);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(14, 44);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciSettingsLabel
            // 
            this.lciSettingsLabel.Control = this.backstageViewLabel2;
            this.lciSettingsLabel.CustomizationFormText = "Settings";
            this.lciSettingsLabel.Location = new System.Drawing.Point(0, 172);
            this.lciSettingsLabel.MaxSize = new System.Drawing.Size(0, 36);
            this.lciSettingsLabel.MinSize = new System.Drawing.Size(14, 36);
            this.lciSettingsLabel.Name = "lciSettingsLabel";
            this.lciSettingsLabel.Size = new System.Drawing.Size(278, 36);
            this.lciSettingsLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciSettingsLabel.Text = "Settings";
            this.lciSettingsLabel.TextSize = new System.Drawing.Size(0, 0);
            this.lciSettingsLabel.TextToControlDistance = 0;
            this.lciSettingsLabel.TextVisible = false;
            // 
            // lciPrinterLabel
            // 
            this.lciPrinterLabel.Control = this.printerLabel;
            this.lciPrinterLabel.CustomizationFormText = "lciPrinterLabel";
            this.lciPrinterLabel.Location = new System.Drawing.Point(0, 80);
            this.lciPrinterLabel.MaxSize = new System.Drawing.Size(0, 36);
            this.lciPrinterLabel.MinSize = new System.Drawing.Size(14, 36);
            this.lciPrinterLabel.Name = "lciPrinterLabel";
            this.lciPrinterLabel.Size = new System.Drawing.Size(278, 36);
            this.lciPrinterLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPrinterLabel.Text = "lciPrinterLabel";
            this.lciPrinterLabel.TextSize = new System.Drawing.Size(0, 0);
            this.lciPrinterLabel.TextToControlDistance = 0;
            this.lciPrinterLabel.TextVisible = false;
            // 
            // lciPrinter
            // 
            this.lciPrinter.Control = this.ddbPrinter;
            this.lciPrinter.CustomizationFormText = "lciPrinter";
            this.lciPrinter.Location = new System.Drawing.Point(0, 116);
            this.lciPrinter.MaxSize = new System.Drawing.Size(0, 56);
            this.lciPrinter.MinSize = new System.Drawing.Size(100, 56);
            this.lciPrinter.Name = "lciPrinter";
            this.lciPrinter.Size = new System.Drawing.Size(278, 56);
            this.lciPrinter.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPrinter.Text = "lciPrinter";
            this.lciPrinter.TextSize = new System.Drawing.Size(0, 0);
            this.lciPrinter.TextToControlDistance = 0;
            this.lciPrinter.TextVisible = false;
            // 
            // lciDuplex
            // 
            this.lciDuplex.Control = this.ddbDuplex;
            this.lciDuplex.CustomizationFormText = "lciDuplex";
            this.lciDuplex.Location = new System.Drawing.Point(0, 208);
            this.lciDuplex.MaxSize = new System.Drawing.Size(0, 56);
            this.lciDuplex.MinSize = new System.Drawing.Size(68, 56);
            this.lciDuplex.Name = "lciDuplex";
            this.lciDuplex.Size = new System.Drawing.Size(278, 56);
            this.lciDuplex.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDuplex.Text = "lciDuplex";
            this.lciDuplex.TextSize = new System.Drawing.Size(0, 0);
            this.lciDuplex.TextToControlDistance = 0;
            this.lciDuplex.TextVisible = false;
            // 
            // lciCollated
            // 
            this.lciCollated.Control = this.ddbCollate;
            this.lciCollated.CustomizationFormText = "lciPagePerSheet";
            this.lciCollated.Location = new System.Drawing.Point(0, 264);
            this.lciCollated.MaxSize = new System.Drawing.Size(0, 56);
            this.lciCollated.MinSize = new System.Drawing.Size(100, 56);
            this.lciCollated.Name = "lciCollated";
            this.lciCollated.Size = new System.Drawing.Size(278, 56);
            this.lciCollated.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCollated.Text = "lciCollated";
            this.lciCollated.TextSize = new System.Drawing.Size(0, 0);
            this.lciCollated.TextToControlDistance = 0;
            this.lciCollated.TextVisible = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.printControl1);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(7, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(536, 550);
            this.panel3.TabIndex = 2;
            // 
            // printControl1
            // 
            this.printControl1.BackColor = System.Drawing.Color.Empty;
            this.printControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printControl1.ForeColor = System.Drawing.Color.Empty;
            this.printControl1.IsMetric = true;
            this.printControl1.Location = new System.Drawing.Point(0, 0);
            this.printControl1.Name = "printControl1";
            this.printControl1.Size = new System.Drawing.Size(536, 526);
            this.printControl1.TabIndex = 0;
            this.printControl1.TooltipFont = new System.Drawing.Font("Tahoma", 8.25F);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.zoomTextEdit);
            this.panel1.Controls.Add(this.pageButtonEdit);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.zoomTrackBarControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 526);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel1.Size = new System.Drawing.Size(536, 24);
            this.panel1.TabIndex = 1;
            // 
            // zoomTextEdit
            // 
            this.zoomTextEdit.Dock = System.Windows.Forms.DockStyle.Right;
            this.zoomTextEdit.EditValue = ((short)(100));
            this.zoomTextEdit.Location = new System.Drawing.Point(239, 3);
            this.zoomTextEdit.Name = "zoomTextEdit";
            this.zoomTextEdit.Properties.DisplayFormat.FormatString = "{0}%";
            this.zoomTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.zoomTextEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.zoomTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.zoomTextEdit.Size = new System.Drawing.Size(73, 20);
            this.zoomTextEdit.TabIndex = 4;
            // 
            // pageButtonEdit
            // 
            this.pageButtonEdit.Dock = System.Windows.Forms.DockStyle.Left;
            this.pageButtonEdit.EditValue = "1";
            this.pageButtonEdit.Location = new System.Drawing.Point(0, 3);
            this.pageButtonEdit.Name = "pageButtonEdit";
            this.pageButtonEdit.Properties.Appearance.Options.UseTextOptions = true;
            this.pageButtonEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.pageButtonEdit.Properties.AutoHeight = false;
            this.pageButtonEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pageButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Left, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, "", null, null, true),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Right)});
            this.pageButtonEdit.Properties.DisplayFormat.FormatString = "当前页 {0} / 1";
            this.pageButtonEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.pageButtonEdit.Size = new System.Drawing.Size(118, 18);
            this.pageButtonEdit.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(312, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(10, 18);
            this.panel2.TabIndex = 1;
            // 
            // zoomTrackBarControl1
            // 
            this.zoomTrackBarControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.zoomTrackBarControl1.EditValue = 40;
            this.zoomTrackBarControl1.Location = new System.Drawing.Point(322, 3);
            this.zoomTrackBarControl1.Name = "zoomTrackBarControl1";
            this.zoomTrackBarControl1.Properties.Alignment = DevExpress.Utils.VertAlignment.Center;
            this.zoomTrackBarControl1.Properties.Maximum = 80;
            this.zoomTrackBarControl1.Properties.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
            this.zoomTrackBarControl1.Properties.SmallChange = 2;
            this.zoomTrackBarControl1.Size = new System.Drawing.Size(214, 18);
            this.zoomTrackBarControl1.TabIndex = 0;
            this.zoomTrackBarControl1.Value = 40;
            // 
            // labelControl4
            // 
            this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl4.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl4.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
            this.labelControl4.LineVisible = true;
            this.labelControl4.Location = new System.Drawing.Point(3, 3);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(4, 550);
            this.labelControl4.TabIndex = 4;
            // 
            // PrintViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PrintViewControl";
            this.Size = new System.Drawing.Size(848, 556);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.copySpinEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrintButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOrientation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMargins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaperSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrintLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCopiesSpinEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSettingsLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrinterLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrinter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDuplex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCollated)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pageButtonEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBarControl1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBarControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private DevExpress.XtraPrinting.Control.PrintControl printControl1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.ZoomTrackBarControl zoomTrackBarControl1;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.ButtonEdit pageButtonEdit;
        private DevExpress.XtraEditors.TextEdit zoomTextEdit;
        private DevExpress.XtraEditors.DropDownButton ddbOrientation;
        private DevExpress.XtraEditors.DropDownButton ddbCollate;
        private DevExpress.XtraEditors.DropDownButton ddbPaperSize;
        private DevExpress.XtraEditors.DropDownButton ddbMargins;
        private DevExpress.XtraEditors.SimpleButton printButton;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private BackstageViewLabel backstageViewLabel1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciPrintButton;
        private DevExpress.XtraLayout.LayoutControlItem lciOrientation;
        private DevExpress.XtraLayout.LayoutControlItem lciMargins;
        private DevExpress.XtraLayout.LayoutControlItem lciPaperSize;
        private DevExpress.XtraLayout.LayoutControlItem lciCollated;
        private DevExpress.XtraLayout.LayoutControlItem lciPrintLabel;
        private DevExpress.XtraEditors.SpinEdit copySpinEdit;
        private DevExpress.XtraLayout.LayoutControlItem lciCopiesSpinEdit;
        private BackstageViewLabel backstageViewLabel2;
        private DevExpress.XtraLayout.LayoutControlItem lciSettingsLabel;
        private BackstageViewLabel printerLabel;
        private DevExpress.XtraEditors.DropDownButton ddbPrinter;
        private DevExpress.XtraLayout.LayoutControlItem lciPrinterLabel;
        private DevExpress.XtraLayout.LayoutControlItem lciPrinter;
        private DevExpress.XtraEditors.DropDownButton ddbDuplex;
        private DevExpress.XtraLayout.LayoutControlItem lciDuplex;
        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
