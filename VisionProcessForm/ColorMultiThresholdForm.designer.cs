namespace VisionProcessForm
{
    partial class ColorMultiThresholdForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorMultiThresholdForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbl_Lightness = new SRMControl.SRMLabel();
            this.txt_Value3 = new SRMControl.SRMInputBox();
            this.lbl_Saturation = new SRMControl.SRMLabel();
            this.txt_Value2 = new SRMControl.SRMInputBox();
            this.lbl_Hue = new SRMControl.SRMLabel();
            this.trackBar_Value1 = new System.Windows.Forms.TrackBar();
            this.txt_Value1 = new SRMControl.SRMInputBox();
            this.group_LSHSetting = new SRMControl.SRMGroupBox();
            this.lbl_HueTolerance = new SRMControl.SRMLabel();
            this.lbl_SaturationTolerance = new SRMControl.SRMLabel();
            this.txt_Value2Tolerance = new System.Windows.Forms.NumericUpDown();
            this.txt_Value1Tolerance = new System.Windows.Forms.NumericUpDown();
            this.lbl_LightnessTolerance = new SRMControl.SRMLabel();
            this.txt_Value3Tolerance = new System.Windows.Forms.NumericUpDown();
            this.trackBar_Value2 = new System.Windows.Forms.TrackBar();
            this.pic_Saturation = new System.Windows.Forms.PictureBox();
            this.pic_Lightness = new System.Windows.Forms.PictureBox();
            this.pic_Hue = new System.Windows.Forms.PictureBox();
            this.trackBar_Value3 = new System.Windows.Forms.TrackBar();
            this.group_ThresholdSetting = new SRMControl.SRMGroupBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.trackBar_Threshold = new System.Windows.Forms.TrackBar();
            this.txt_Threshold = new SRMControl.SRMInputBox();
            this.btn_UpdateThreshold = new SRMControl.SRMButton();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_MinArea = new SRMControl.SRMInputBox();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.chk_Preview = new SRMControl.SRMCheckBox();
            this.btn_AddThreshold = new SRMControl.SRMButton();
            this.btn_DeleteThreshold = new SRMControl.SRMButton();
            this.group_SelectTemplate = new SRMControl.SRMGroupBox();
            this.srmRadioButton1 = new SRMControl.SRMRadioButton();
            this.radioBtn_RGB = new SRMControl.SRMRadioButton();
            this.radioBtn_HSL = new SRMControl.SRMRadioButton();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.txt_ThresholdName = new SRMControl.SRMInputBox();
            this.dgd_ThresholdSetting = new System.Windows.Forms.DataGridView();
            this.column_ThresholdNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_ThresName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_ColorSystem = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.column_Value1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Tolerance1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Value2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Tolerance2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Value3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Tolerance3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_MinArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.col_ImageNo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.column_CloseIteration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Invert = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tab_VisionControl = new SRMControl.SRMTabControl();
            this.tp_Pin1 = new System.Windows.Forms.TabPage();
            this.groupBox_Pin1 = new SRMControl.SRMGroupBox();
            this.groupBox_Pin1Score = new SRMControl.SRMGroupBox();
            this.lbl_PinScoreTitle = new SRMControl.SRMLabel();
            this.lbl_Pin1ScorePercent = new SRMControl.SRMLabel();
            this.lbl_Pin1Score = new SRMControl.SRMLabel();
            this.group_Pin1Setting = new SRMControl.SRMGroupBox();
            this.lbl_Pin1ToleranceScore = new SRMControl.SRMLabel();
            this.trackBar_Pin1Tolerance = new System.Windows.Forms.TrackBar();
            this.lbl_Pin1TolerancePercent = new SRMControl.SRMLabel();
            this.txt_Pin1Tolerance = new SRMControl.SRMInputBox();
            this.tp_PH = new System.Windows.Forms.TabPage();
            this.group_PHSetting = new SRMControl.SRMGroupBox();
            this.lbl_PHBlobBlackArea = new System.Windows.Forms.Label();
            this.srmLabel25 = new SRMControl.SRMLabel();
            this.txt_BlackAreaArea = new SRMControl.SRMInputBox();
            this.srmLabel29 = new SRMControl.SRMLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pic_ThresholdImage = new System.Windows.Forms.PictureBox();
            this.srmTabControl1 = new SRMControl.SRMTabControl();
            this.tp_ColorProfile = new System.Windows.Forms.TabPage();
            this.group_ImageProcessingSetting = new SRMControl.SRMGroupBox();
            this.chk_InvertBlackWhite = new SRMControl.SRMCheckBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.txt_CloseIteration = new System.Windows.Forms.NumericUpDown();
            this.tp_ROITolerance = new System.Windows.Forms.TabPage();
            this.chk_SetToAllEdge = new SRMControl.SRMCheckBox();
            this.pnl_Bottom = new System.Windows.Forms.Panel();
            this.srmLabel36 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromBottom = new SRMControl.SRMInputBox();
            this.pnl_Right = new System.Windows.Forms.Panel();
            this.srmLabel34 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromRight = new SRMControl.SRMInputBox();
            this.pnl_Left = new System.Windows.Forms.Panel();
            this.srmLabel38 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromLeft = new SRMControl.SRMInputBox();
            this.pnl_Top = new System.Windows.Forms.Panel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromTop = new SRMControl.SRMInputBox();
            this.picUnitROI = new System.Windows.Forms.PictureBox();
            this.tp_DontCare = new System.Windows.Forms.TabPage();
            this.radioButton_NoneDontCare = new SRMControl.SRMRadioButton();
            this.pnl_DrawMethod = new System.Windows.Forms.Panel();
            this.srmLabel66 = new SRMControl.SRMLabel();
            this.srmLabel23 = new SRMControl.SRMLabel();
            this.cbo_DontCareAreaDrawMethod = new SRMControl.SRMComboBox();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.btn_Undo = new SRMControl.SRMButton();
            this.pic_Help = new System.Windows.Forms.PictureBox();
            this.btn_AddDontCareROI = new SRMControl.SRMButton();
            this.btn_DeleteDontCareROI = new SRMControl.SRMButton();
            this.radioButton_ManualDontCare = new SRMControl.SRMRadioButton();
            this.radioButton_PackageDontCare = new SRMControl.SRMRadioButton();
            this.radioButton_PadDontCare = new SRMControl.SRMRadioButton();
            this.ils_ImageListTree = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnl_HelpMessage = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.srmGroupBox5 = new SRMControl.SRMGroupBox();
            this.radioBtn_Middle = new SRMControl.SRMRadioButton();
            this.radioBtn_Down = new SRMControl.SRMRadioButton();
            this.radioBtn_Left = new SRMControl.SRMRadioButton();
            this.radioBtn_Up = new SRMControl.SRMRadioButton();
            this.radioBtn_Right = new SRMControl.SRMRadioButton();
            this.btn_Up = new System.Windows.Forms.Button();
            this.btn_Down = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Value1)).BeginInit();
            this.group_LSHSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Value2Tolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Value1Tolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Value3Tolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Value2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Saturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Lightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Hue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Value3)).BeginInit();
            this.group_ThresholdSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Threshold)).BeginInit();
            this.group_SelectTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ThresholdSetting)).BeginInit();
            this.tab_VisionControl.SuspendLayout();
            this.tp_Pin1.SuspendLayout();
            this.groupBox_Pin1.SuspendLayout();
            this.groupBox_Pin1Score.SuspendLayout();
            this.group_Pin1Setting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Pin1Tolerance)).BeginInit();
            this.tp_PH.SuspendLayout();
            this.group_PHSetting.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ThresholdImage)).BeginInit();
            this.srmTabControl1.SuspendLayout();
            this.tp_ColorProfile.SuspendLayout();
            this.group_ImageProcessingSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CloseIteration)).BeginInit();
            this.tp_ROITolerance.SuspendLayout();
            this.pnl_Bottom.SuspendLayout();
            this.pnl_Right.SuspendLayout();
            this.pnl_Left.SuspendLayout();
            this.pnl_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitROI)).BeginInit();
            this.tp_DontCare.SuspendLayout();
            this.pnl_DrawMethod.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Help)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnl_HelpMessage.SuspendLayout();
            this.srmGroupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_Lightness
            // 
            resources.ApplyResources(this.lbl_Lightness, "lbl_Lightness");
            this.lbl_Lightness.Name = "lbl_Lightness";
            this.lbl_Lightness.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Value3
            // 
            this.txt_Value3.BackColor = System.Drawing.Color.White;
            this.txt_Value3.DecimalPlaces = 0;
            this.txt_Value3.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Value3.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Value3.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Value3.ForeColor = System.Drawing.Color.Black;
            this.txt_Value3.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Value3, "txt_Value3");
            this.txt_Value3.Name = "txt_Value3";
            this.txt_Value3.NormalBackColor = System.Drawing.Color.White;
            this.txt_Value3.TextChanged += new System.EventHandler(this.txt_Lightness_TextChanged);
            // 
            // lbl_Saturation
            // 
            resources.ApplyResources(this.lbl_Saturation, "lbl_Saturation");
            this.lbl_Saturation.Name = "lbl_Saturation";
            this.lbl_Saturation.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Value2
            // 
            this.txt_Value2.BackColor = System.Drawing.Color.White;
            this.txt_Value2.DecimalPlaces = 0;
            this.txt_Value2.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Value2.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Value2.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Value2.ForeColor = System.Drawing.Color.Black;
            this.txt_Value2.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Value2, "txt_Value2");
            this.txt_Value2.Name = "txt_Value2";
            this.txt_Value2.NormalBackColor = System.Drawing.Color.White;
            this.txt_Value2.TextChanged += new System.EventHandler(this.txt_Saturation_TextChanged);
            // 
            // lbl_Hue
            // 
            resources.ApplyResources(this.lbl_Hue, "lbl_Hue");
            this.lbl_Hue.Name = "lbl_Hue";
            this.lbl_Hue.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_Value1
            // 
            resources.ApplyResources(this.trackBar_Value1, "trackBar_Value1");
            this.trackBar_Value1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Value1.LargeChange = 1;
            this.trackBar_Value1.Maximum = 255;
            this.trackBar_Value1.Name = "trackBar_Value1";
            this.trackBar_Value1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Value1.Value = 125;
            this.trackBar_Value1.Scroll += new System.EventHandler(this.trackBar_Hue_Scroll);
            // 
            // txt_Value1
            // 
            this.txt_Value1.BackColor = System.Drawing.Color.White;
            this.txt_Value1.DecimalPlaces = 0;
            this.txt_Value1.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Value1.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Value1.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Value1.ForeColor = System.Drawing.Color.Black;
            this.txt_Value1.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Value1, "txt_Value1");
            this.txt_Value1.Name = "txt_Value1";
            this.txt_Value1.NormalBackColor = System.Drawing.Color.White;
            this.txt_Value1.TextChanged += new System.EventHandler(this.txt_Hue_TextChanged);
            // 
            // group_LSHSetting
            // 
            this.group_LSHSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_LSHSetting.Controls.Add(this.lbl_HueTolerance);
            this.group_LSHSetting.Controls.Add(this.lbl_SaturationTolerance);
            this.group_LSHSetting.Controls.Add(this.txt_Value2Tolerance);
            this.group_LSHSetting.Controls.Add(this.txt_Value1Tolerance);
            this.group_LSHSetting.Controls.Add(this.lbl_LightnessTolerance);
            this.group_LSHSetting.Controls.Add(this.txt_Value3Tolerance);
            this.group_LSHSetting.Controls.Add(this.lbl_Lightness);
            this.group_LSHSetting.Controls.Add(this.lbl_Saturation);
            this.group_LSHSetting.Controls.Add(this.txt_Value2);
            this.group_LSHSetting.Controls.Add(this.txt_Value3);
            this.group_LSHSetting.Controls.Add(this.trackBar_Value2);
            this.group_LSHSetting.Controls.Add(this.pic_Saturation);
            this.group_LSHSetting.Controls.Add(this.pic_Lightness);
            this.group_LSHSetting.Controls.Add(this.pic_Hue);
            this.group_LSHSetting.Controls.Add(this.trackBar_Value3);
            this.group_LSHSetting.Controls.Add(this.lbl_Hue);
            this.group_LSHSetting.Controls.Add(this.trackBar_Value1);
            this.group_LSHSetting.Controls.Add(this.txt_Value1);
            resources.ApplyResources(this.group_LSHSetting, "group_LSHSetting");
            this.group_LSHSetting.Name = "group_LSHSetting";
            this.group_LSHSetting.TabStop = false;
            // 
            // lbl_HueTolerance
            // 
            resources.ApplyResources(this.lbl_HueTolerance, "lbl_HueTolerance");
            this.lbl_HueTolerance.Name = "lbl_HueTolerance";
            this.lbl_HueTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_SaturationTolerance
            // 
            resources.ApplyResources(this.lbl_SaturationTolerance, "lbl_SaturationTolerance");
            this.lbl_SaturationTolerance.Name = "lbl_SaturationTolerance";
            this.lbl_SaturationTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Value2Tolerance
            // 
            resources.ApplyResources(this.txt_Value2Tolerance, "txt_Value2Tolerance");
            this.txt_Value2Tolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Value2Tolerance.Name = "txt_Value2Tolerance";
            this.txt_Value2Tolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_Value2Tolerance.ValueChanged += new System.EventHandler(this.txt_SaturationTolerance_ValueChanged);
            // 
            // txt_Value1Tolerance
            // 
            resources.ApplyResources(this.txt_Value1Tolerance, "txt_Value1Tolerance");
            this.txt_Value1Tolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Value1Tolerance.Name = "txt_Value1Tolerance";
            this.txt_Value1Tolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_Value1Tolerance.ValueChanged += new System.EventHandler(this.txt_HueTolerance_ValueChanged);
            // 
            // lbl_LightnessTolerance
            // 
            resources.ApplyResources(this.lbl_LightnessTolerance, "lbl_LightnessTolerance");
            this.lbl_LightnessTolerance.Name = "lbl_LightnessTolerance";
            this.lbl_LightnessTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Value3Tolerance
            // 
            resources.ApplyResources(this.txt_Value3Tolerance, "txt_Value3Tolerance");
            this.txt_Value3Tolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Value3Tolerance.Name = "txt_Value3Tolerance";
            this.txt_Value3Tolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_Value3Tolerance.ValueChanged += new System.EventHandler(this.txt_LightnessTolerance_ValueChanged);
            // 
            // trackBar_Value2
            // 
            resources.ApplyResources(this.trackBar_Value2, "trackBar_Value2");
            this.trackBar_Value2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Value2.LargeChange = 1;
            this.trackBar_Value2.Maximum = 255;
            this.trackBar_Value2.Name = "trackBar_Value2";
            this.trackBar_Value2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Value2.Value = 125;
            this.trackBar_Value2.Scroll += new System.EventHandler(this.trackBar_Saturation_Scroll);
            // 
            // pic_Saturation
            // 
            resources.ApplyResources(this.pic_Saturation, "pic_Saturation");
            this.pic_Saturation.Name = "pic_Saturation";
            this.pic_Saturation.TabStop = false;
            // 
            // pic_Lightness
            // 
            resources.ApplyResources(this.pic_Lightness, "pic_Lightness");
            this.pic_Lightness.Name = "pic_Lightness";
            this.pic_Lightness.TabStop = false;
            // 
            // pic_Hue
            // 
            resources.ApplyResources(this.pic_Hue, "pic_Hue");
            this.pic_Hue.Name = "pic_Hue";
            this.pic_Hue.TabStop = false;
            // 
            // trackBar_Value3
            // 
            resources.ApplyResources(this.trackBar_Value3, "trackBar_Value3");
            this.trackBar_Value3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Value3.LargeChange = 1;
            this.trackBar_Value3.Maximum = 255;
            this.trackBar_Value3.Name = "trackBar_Value3";
            this.trackBar_Value3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Value3.Value = 125;
            this.trackBar_Value3.Scroll += new System.EventHandler(this.trackBar_Lightness_Scroll);
            // 
            // group_ThresholdSetting
            // 
            this.group_ThresholdSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_ThresholdSetting.Controls.Add(this.srmLabel8);
            this.group_ThresholdSetting.Controls.Add(this.trackBar_Threshold);
            this.group_ThresholdSetting.Controls.Add(this.txt_Threshold);
            resources.ApplyResources(this.group_ThresholdSetting, "group_ThresholdSetting");
            this.group_ThresholdSetting.Name = "group_ThresholdSetting";
            this.group_ThresholdSetting.TabStop = false;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_Threshold
            // 
            resources.ApplyResources(this.trackBar_Threshold, "trackBar_Threshold");
            this.trackBar_Threshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Threshold.LargeChange = 1;
            this.trackBar_Threshold.Maximum = 255;
            this.trackBar_Threshold.Name = "trackBar_Threshold";
            this.trackBar_Threshold.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Threshold.Value = 125;
            this.trackBar_Threshold.Scroll += new System.EventHandler(this.trackBar_Threshold_Scroll);
            // 
            // txt_Threshold
            // 
            this.txt_Threshold.BackColor = System.Drawing.Color.White;
            this.txt_Threshold.DecimalPlaces = 0;
            this.txt_Threshold.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Threshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Threshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Threshold.ForeColor = System.Drawing.Color.Black;
            this.txt_Threshold.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Threshold, "txt_Threshold");
            this.txt_Threshold.Name = "txt_Threshold";
            this.txt_Threshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_Threshold.TextChanged += new System.EventHandler(this.txt_Threshold_TextChanged);
            // 
            // btn_UpdateThreshold
            // 
            resources.ApplyResources(this.btn_UpdateThreshold, "btn_UpdateThreshold");
            this.btn_UpdateThreshold.Name = "btn_UpdateThreshold";
            this.btn_UpdateThreshold.Tag = "Update Threshold Value";
            this.btn_UpdateThreshold.UseVisualStyleBackColor = true;
            this.btn_UpdateThreshold.Click += new System.EventHandler(this.btn_UpdateThreshold_Click);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinArea
            // 
            this.txt_MinArea.BackColor = System.Drawing.Color.White;
            this.txt_MinArea.DecimalPlaces = 0;
            this.txt_MinArea.DecMaxValue = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.txt_MinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MinArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MinArea, "txt_MinArea");
            this.txt_MinArea.Name = "txt_MinArea";
            this.txt_MinArea.NormalBackColor = System.Drawing.Color.White;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // chk_Preview
            // 
            this.chk_Preview.Checked = true;
            this.chk_Preview.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Preview.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.chk_Preview, "chk_Preview");
            this.chk_Preview.Name = "chk_Preview";
            this.chk_Preview.Selected = false;
            this.chk_Preview.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Preview.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Preview.UseVisualStyleBackColor = true;
            this.chk_Preview.Click += new System.EventHandler(this.chk_Preview_Click);
            // 
            // btn_AddThreshold
            // 
            resources.ApplyResources(this.btn_AddThreshold, "btn_AddThreshold");
            this.btn_AddThreshold.Name = "btn_AddThreshold";
            this.btn_AddThreshold.Tag = "Add New Threshold Row";
            this.btn_AddThreshold.UseVisualStyleBackColor = true;
            this.btn_AddThreshold.Click += new System.EventHandler(this.btn_AddThreshold_Click);
            // 
            // btn_DeleteThreshold
            // 
            resources.ApplyResources(this.btn_DeleteThreshold, "btn_DeleteThreshold");
            this.btn_DeleteThreshold.Name = "btn_DeleteThreshold";
            this.btn_DeleteThreshold.Tag = "Delete Threshold row";
            this.btn_DeleteThreshold.UseVisualStyleBackColor = true;
            this.btn_DeleteThreshold.Click += new System.EventHandler(this.btn_DeleteThreshold_Click);
            // 
            // group_SelectTemplate
            // 
            this.group_SelectTemplate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_SelectTemplate.Controls.Add(this.srmRadioButton1);
            this.group_SelectTemplate.Controls.Add(this.radioBtn_RGB);
            this.group_SelectTemplate.Controls.Add(this.radioBtn_HSL);
            resources.ApplyResources(this.group_SelectTemplate, "group_SelectTemplate");
            this.group_SelectTemplate.Name = "group_SelectTemplate";
            this.group_SelectTemplate.TabStop = false;
            // 
            // srmRadioButton1
            // 
            resources.ApplyResources(this.srmRadioButton1, "srmRadioButton1");
            this.srmRadioButton1.Name = "srmRadioButton1";
            this.srmRadioButton1.UseVisualStyleBackColor = true;
            // 
            // radioBtn_RGB
            // 
            resources.ApplyResources(this.radioBtn_RGB, "radioBtn_RGB");
            this.radioBtn_RGB.Name = "radioBtn_RGB";
            this.radioBtn_RGB.UseVisualStyleBackColor = true;
            // 
            // radioBtn_HSL
            // 
            this.radioBtn_HSL.Checked = true;
            resources.ApplyResources(this.radioBtn_HSL, "radioBtn_HSL");
            this.radioBtn_HSL.Name = "radioBtn_HSL";
            this.radioBtn_HSL.TabStop = true;
            this.radioBtn_HSL.UseVisualStyleBackColor = true;
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_ThresholdName
            // 
            this.txt_ThresholdName.BackColor = System.Drawing.Color.White;
            this.txt_ThresholdName.DecimalPlaces = 0;
            this.txt_ThresholdName.DecMaxValue = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.txt_ThresholdName.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ThresholdName.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ThresholdName.ForeColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.txt_ThresholdName, "txt_ThresholdName");
            this.txt_ThresholdName.Name = "txt_ThresholdName";
            this.txt_ThresholdName.NormalBackColor = System.Drawing.Color.White;
            // 
            // dgd_ThresholdSetting
            // 
            this.dgd_ThresholdSetting.AllowUserToAddRows = false;
            this.dgd_ThresholdSetting.AllowUserToDeleteRows = false;
            this.dgd_ThresholdSetting.AllowUserToResizeColumns = false;
            this.dgd_ThresholdSetting.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgd_ThresholdSetting.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_ThresholdSetting.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_ThresholdSetting.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_ThresholdSetting.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_ThresholdSetting.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ThresholdSetting.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgd_ThresholdSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_ThresholdSetting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column_ThresholdNo,
            this.column_ThresName,
            this.column_ColorSystem,
            this.column_Value1,
            this.column_Tolerance1,
            this.column_Value2,
            this.column_Tolerance2,
            this.column_Value3,
            this.column_Tolerance3,
            this.column_MinArea,
            this.col_Type,
            this.col_ImageNo,
            this.column_CloseIteration,
            this.column_Invert});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ThresholdSetting.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgd_ThresholdSetting.GridColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.dgd_ThresholdSetting, "dgd_ThresholdSetting");
            this.dgd_ThresholdSetting.MultiSelect = false;
            this.dgd_ThresholdSetting.Name = "dgd_ThresholdSetting";
            this.dgd_ThresholdSetting.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ThresholdSetting.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_ThresholdSetting.RowHeadersVisible = false;
            this.dgd_ThresholdSetting.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgd_ThresholdSetting.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgd_ThresholdSetting.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ThresholdSetting.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgd_ThresholdSetting.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_ThresholdSetting_CellClick);
            this.dgd_ThresholdSetting.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_ThresholdSetting_CellEndEdit);
            this.dgd_ThresholdSetting.Click += new System.EventHandler(this.dgd_ThresholdSetting_Click);
            // 
            // column_ThresholdNo
            // 
            resources.ApplyResources(this.column_ThresholdNo, "column_ThresholdNo");
            this.column_ThresholdNo.Name = "column_ThresholdNo";
            this.column_ThresholdNo.ReadOnly = true;
            this.column_ThresholdNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_ThresName
            // 
            resources.ApplyResources(this.column_ThresName, "column_ThresName");
            this.column_ThresName.Name = "column_ThresName";
            this.column_ThresName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_ColorSystem
            // 
            resources.ApplyResources(this.column_ColorSystem, "column_ColorSystem");
            this.column_ColorSystem.Items.AddRange(new object[] {
            "HSL",
            "RGB",
            "Saturation"});
            this.column_ColorSystem.Name = "column_ColorSystem";
            this.column_ColorSystem.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // column_Value1
            // 
            resources.ApplyResources(this.column_Value1, "column_Value1");
            this.column_Value1.Name = "column_Value1";
            this.column_Value1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_Tolerance1
            // 
            resources.ApplyResources(this.column_Tolerance1, "column_Tolerance1");
            this.column_Tolerance1.Name = "column_Tolerance1";
            this.column_Tolerance1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_Value2
            // 
            resources.ApplyResources(this.column_Value2, "column_Value2");
            this.column_Value2.Name = "column_Value2";
            this.column_Value2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_Tolerance2
            // 
            resources.ApplyResources(this.column_Tolerance2, "column_Tolerance2");
            this.column_Tolerance2.Name = "column_Tolerance2";
            this.column_Tolerance2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_Value3
            // 
            resources.ApplyResources(this.column_Value3, "column_Value3");
            this.column_Value3.Name = "column_Value3";
            this.column_Value3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_Tolerance3
            // 
            resources.ApplyResources(this.column_Tolerance3, "column_Tolerance3");
            this.column_Tolerance3.Name = "column_Tolerance3";
            this.column_Tolerance3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_MinArea
            // 
            resources.ApplyResources(this.column_MinArea, "column_MinArea");
            this.column_MinArea.Name = "column_MinArea";
            this.column_MinArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_Type
            // 
            resources.ApplyResources(this.col_Type, "col_Type");
            this.col_Type.Items.AddRange(new object[] {
            "Defect",
            "Good"});
            this.col_Type.Name = "col_Type";
            // 
            // col_ImageNo
            // 
            resources.ApplyResources(this.col_ImageNo, "col_ImageNo");
            this.col_ImageNo.Name = "col_ImageNo";
            // 
            // column_CloseIteration
            // 
            resources.ApplyResources(this.column_CloseIteration, "column_CloseIteration");
            this.column_CloseIteration.Name = "column_CloseIteration";
            this.column_CloseIteration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_Invert
            // 
            resources.ApplyResources(this.column_Invert, "column_Invert");
            this.column_Invert.Name = "column_Invert";
            // 
            // tab_VisionControl
            // 
            resources.ApplyResources(this.tab_VisionControl, "tab_VisionControl");
            this.tab_VisionControl.Controls.Add(this.tp_Pin1);
            this.tab_VisionControl.Controls.Add(this.tp_PH);
            this.tab_VisionControl.Name = "tab_VisionControl";
            this.tab_VisionControl.SelectedIndex = 0;
            // 
            // tp_Pin1
            // 
            this.tp_Pin1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Pin1.Controls.Add(this.groupBox_Pin1);
            resources.ApplyResources(this.tp_Pin1, "tp_Pin1");
            this.tp_Pin1.Name = "tp_Pin1";
            // 
            // groupBox_Pin1
            // 
            this.groupBox_Pin1.BorderColor = System.Drawing.Color.Transparent;
            this.groupBox_Pin1.Controls.Add(this.groupBox_Pin1Score);
            this.groupBox_Pin1.Controls.Add(this.group_Pin1Setting);
            resources.ApplyResources(this.groupBox_Pin1, "groupBox_Pin1");
            this.groupBox_Pin1.Name = "groupBox_Pin1";
            this.groupBox_Pin1.TabStop = false;
            // 
            // groupBox_Pin1Score
            // 
            this.groupBox_Pin1Score.BorderColor = System.Drawing.Color.Transparent;
            this.groupBox_Pin1Score.Controls.Add(this.lbl_PinScoreTitle);
            this.groupBox_Pin1Score.Controls.Add(this.lbl_Pin1ScorePercent);
            this.groupBox_Pin1Score.Controls.Add(this.lbl_Pin1Score);
            resources.ApplyResources(this.groupBox_Pin1Score, "groupBox_Pin1Score");
            this.groupBox_Pin1Score.Name = "groupBox_Pin1Score";
            this.groupBox_Pin1Score.TabStop = false;
            // 
            // lbl_PinScoreTitle
            // 
            resources.ApplyResources(this.lbl_PinScoreTitle, "lbl_PinScoreTitle");
            this.lbl_PinScoreTitle.Name = "lbl_PinScoreTitle";
            this.lbl_PinScoreTitle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Pin1ScorePercent
            // 
            resources.ApplyResources(this.lbl_Pin1ScorePercent, "lbl_Pin1ScorePercent");
            this.lbl_Pin1ScorePercent.Name = "lbl_Pin1ScorePercent";
            this.lbl_Pin1ScorePercent.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Pin1Score
            // 
            this.lbl_Pin1Score.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Pin1Score, "lbl_Pin1Score");
            this.lbl_Pin1Score.Name = "lbl_Pin1Score";
            this.lbl_Pin1Score.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // group_Pin1Setting
            // 
            this.group_Pin1Setting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_Pin1Setting.Controls.Add(this.lbl_Pin1ToleranceScore);
            this.group_Pin1Setting.Controls.Add(this.trackBar_Pin1Tolerance);
            this.group_Pin1Setting.Controls.Add(this.lbl_Pin1TolerancePercent);
            this.group_Pin1Setting.Controls.Add(this.txt_Pin1Tolerance);
            resources.ApplyResources(this.group_Pin1Setting, "group_Pin1Setting");
            this.group_Pin1Setting.Name = "group_Pin1Setting";
            this.group_Pin1Setting.TabStop = false;
            // 
            // lbl_Pin1ToleranceScore
            // 
            resources.ApplyResources(this.lbl_Pin1ToleranceScore, "lbl_Pin1ToleranceScore");
            this.lbl_Pin1ToleranceScore.Name = "lbl_Pin1ToleranceScore";
            this.lbl_Pin1ToleranceScore.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_Pin1Tolerance
            // 
            resources.ApplyResources(this.trackBar_Pin1Tolerance, "trackBar_Pin1Tolerance");
            this.trackBar_Pin1Tolerance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Pin1Tolerance.LargeChange = 1;
            this.trackBar_Pin1Tolerance.Maximum = 100;
            this.trackBar_Pin1Tolerance.Minimum = 1;
            this.trackBar_Pin1Tolerance.Name = "trackBar_Pin1Tolerance";
            this.trackBar_Pin1Tolerance.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Pin1Tolerance.Value = 1;
            // 
            // lbl_Pin1TolerancePercent
            // 
            resources.ApplyResources(this.lbl_Pin1TolerancePercent, "lbl_Pin1TolerancePercent");
            this.lbl_Pin1TolerancePercent.Name = "lbl_Pin1TolerancePercent";
            this.lbl_Pin1TolerancePercent.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Pin1Tolerance
            // 
            this.txt_Pin1Tolerance.BackColor = System.Drawing.Color.White;
            this.txt_Pin1Tolerance.DecimalPlaces = 0;
            this.txt_Pin1Tolerance.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_Pin1Tolerance.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Pin1Tolerance.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Pin1Tolerance.ForeColor = System.Drawing.Color.Black;
            this.txt_Pin1Tolerance.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Pin1Tolerance, "txt_Pin1Tolerance");
            this.txt_Pin1Tolerance.Name = "txt_Pin1Tolerance";
            this.txt_Pin1Tolerance.NormalBackColor = System.Drawing.Color.White;
            // 
            // tp_PH
            // 
            this.tp_PH.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_PH.Controls.Add(this.group_PHSetting);
            resources.ApplyResources(this.tp_PH, "tp_PH");
            this.tp_PH.Name = "tp_PH";
            // 
            // group_PHSetting
            // 
            this.group_PHSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_PHSetting.Controls.Add(this.lbl_PHBlobBlackArea);
            this.group_PHSetting.Controls.Add(this.srmLabel25);
            this.group_PHSetting.Controls.Add(this.txt_BlackAreaArea);
            this.group_PHSetting.Controls.Add(this.srmLabel29);
            resources.ApplyResources(this.group_PHSetting, "group_PHSetting");
            this.group_PHSetting.Name = "group_PHSetting";
            this.group_PHSetting.TabStop = false;
            // 
            // lbl_PHBlobBlackArea
            // 
            this.lbl_PHBlobBlackArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PHBlobBlackArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_PHBlobBlackArea, "lbl_PHBlobBlackArea");
            this.lbl_PHBlobBlackArea.Name = "lbl_PHBlobBlackArea";
            // 
            // srmLabel25
            // 
            resources.ApplyResources(this.srmLabel25, "srmLabel25");
            this.srmLabel25.Name = "srmLabel25";
            this.srmLabel25.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_BlackAreaArea
            // 
            this.txt_BlackAreaArea.BackColor = System.Drawing.Color.White;
            this.txt_BlackAreaArea.DecimalPlaces = 0;
            this.txt_BlackAreaArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_BlackAreaArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_BlackAreaArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_BlackAreaArea.ForeColor = System.Drawing.Color.Black;
            this.txt_BlackAreaArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_BlackAreaArea, "txt_BlackAreaArea");
            this.txt_BlackAreaArea.Name = "txt_BlackAreaArea";
            this.txt_BlackAreaArea.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel29
            // 
            resources.ApplyResources(this.srmLabel29, "srmLabel29");
            this.srmLabel29.Name = "srmLabel29";
            this.srmLabel29.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pic_ThresholdImage);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pic_ThresholdImage
            // 
            resources.ApplyResources(this.pic_ThresholdImage, "pic_ThresholdImage");
            this.pic_ThresholdImage.Name = "pic_ThresholdImage";
            this.pic_ThresholdImage.TabStop = false;
            this.pic_ThresholdImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_ThresholdImage_Paint);
            // 
            // srmTabControl1
            // 
            resources.ApplyResources(this.srmTabControl1, "srmTabControl1");
            this.srmTabControl1.Controls.Add(this.tp_ColorProfile);
            this.srmTabControl1.Controls.Add(this.tp_ROITolerance);
            this.srmTabControl1.Controls.Add(this.tp_DontCare);
            this.srmTabControl1.Name = "srmTabControl1";
            this.srmTabControl1.SelectedIndex = 0;
            this.srmTabControl1.SelectedIndexChanged += new System.EventHandler(this.srmTabControl1_SelectedIndexChanged);
            // 
            // tp_ColorProfile
            // 
            this.tp_ColorProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_ColorProfile.Controls.Add(this.group_ImageProcessingSetting);
            this.tp_ColorProfile.Controls.Add(this.group_ThresholdSetting);
            this.tp_ColorProfile.Controls.Add(this.group_LSHSetting);
            resources.ApplyResources(this.tp_ColorProfile, "tp_ColorProfile");
            this.tp_ColorProfile.Name = "tp_ColorProfile";
            // 
            // group_ImageProcessingSetting
            // 
            this.group_ImageProcessingSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_ImageProcessingSetting.Controls.Add(this.chk_InvertBlackWhite);
            this.group_ImageProcessingSetting.Controls.Add(this.srmLabel3);
            this.group_ImageProcessingSetting.Controls.Add(this.txt_CloseIteration);
            resources.ApplyResources(this.group_ImageProcessingSetting, "group_ImageProcessingSetting");
            this.group_ImageProcessingSetting.Name = "group_ImageProcessingSetting";
            this.group_ImageProcessingSetting.TabStop = false;
            // 
            // chk_InvertBlackWhite
            // 
            this.chk_InvertBlackWhite.Checked = true;
            this.chk_InvertBlackWhite.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_InvertBlackWhite.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.chk_InvertBlackWhite, "chk_InvertBlackWhite");
            this.chk_InvertBlackWhite.Name = "chk_InvertBlackWhite";
            this.chk_InvertBlackWhite.Selected = false;
            this.chk_InvertBlackWhite.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_InvertBlackWhite.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_InvertBlackWhite.UseVisualStyleBackColor = true;
            this.chk_InvertBlackWhite.Click += new System.EventHandler(this.chk_InvertBlackWhite_Click);
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_CloseIteration
            // 
            resources.ApplyResources(this.txt_CloseIteration, "txt_CloseIteration");
            this.txt_CloseIteration.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_CloseIteration.Name = "txt_CloseIteration";
            this.txt_CloseIteration.ValueChanged += new System.EventHandler(this.txt_CloseIteration_ValueChanged);
            // 
            // tp_ROITolerance
            // 
            this.tp_ROITolerance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_ROITolerance.Controls.Add(this.chk_SetToAllEdge);
            this.tp_ROITolerance.Controls.Add(this.pnl_Bottom);
            this.tp_ROITolerance.Controls.Add(this.pnl_Right);
            this.tp_ROITolerance.Controls.Add(this.pnl_Left);
            this.tp_ROITolerance.Controls.Add(this.pnl_Top);
            this.tp_ROITolerance.Controls.Add(this.picUnitROI);
            resources.ApplyResources(this.tp_ROITolerance, "tp_ROITolerance");
            this.tp_ROITolerance.Name = "tp_ROITolerance";
            // 
            // chk_SetToAllEdge
            // 
            this.chk_SetToAllEdge.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_SetToAllEdge, "chk_SetToAllEdge");
            this.chk_SetToAllEdge.Name = "chk_SetToAllEdge";
            this.chk_SetToAllEdge.Selected = false;
            this.chk_SetToAllEdge.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllEdge.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllEdge.UseVisualStyleBackColor = true;
            // 
            // pnl_Bottom
            // 
            this.pnl_Bottom.Controls.Add(this.srmLabel36);
            this.pnl_Bottom.Controls.Add(this.txt_StartPixelFromBottom);
            resources.ApplyResources(this.pnl_Bottom, "pnl_Bottom");
            this.pnl_Bottom.Name = "pnl_Bottom";
            // 
            // srmLabel36
            // 
            resources.ApplyResources(this.srmLabel36, "srmLabel36");
            this.srmLabel36.Name = "srmLabel36";
            this.srmLabel36.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_StartPixelFromBottom
            // 
            this.txt_StartPixelFromBottom.BackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromBottom.DecimalPlaces = 0;
            this.txt_StartPixelFromBottom.DecMaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.txt_StartPixelFromBottom.DecMinValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.txt_StartPixelFromBottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_StartPixelFromBottom.ForeColor = System.Drawing.Color.Black;
            this.txt_StartPixelFromBottom.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_StartPixelFromBottom, "txt_StartPixelFromBottom");
            this.txt_StartPixelFromBottom.Name = "txt_StartPixelFromBottom";
            this.txt_StartPixelFromBottom.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromBottom.TextChanged += new System.EventHandler(this.txt_StartPixelFromBottom_TextChanged);
            // 
            // pnl_Right
            // 
            this.pnl_Right.Controls.Add(this.srmLabel34);
            this.pnl_Right.Controls.Add(this.txt_StartPixelFromRight);
            resources.ApplyResources(this.pnl_Right, "pnl_Right");
            this.pnl_Right.Name = "pnl_Right";
            // 
            // srmLabel34
            // 
            resources.ApplyResources(this.srmLabel34, "srmLabel34");
            this.srmLabel34.Name = "srmLabel34";
            this.srmLabel34.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_StartPixelFromRight
            // 
            this.txt_StartPixelFromRight.BackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromRight.DecimalPlaces = 0;
            this.txt_StartPixelFromRight.DecMaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.txt_StartPixelFromRight.DecMinValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.txt_StartPixelFromRight.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_StartPixelFromRight.ForeColor = System.Drawing.Color.Black;
            this.txt_StartPixelFromRight.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_StartPixelFromRight, "txt_StartPixelFromRight");
            this.txt_StartPixelFromRight.Name = "txt_StartPixelFromRight";
            this.txt_StartPixelFromRight.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromRight.TextChanged += new System.EventHandler(this.txt_StartPixelFromRight_TextChanged);
            // 
            // pnl_Left
            // 
            this.pnl_Left.Controls.Add(this.srmLabel38);
            this.pnl_Left.Controls.Add(this.txt_StartPixelFromLeft);
            resources.ApplyResources(this.pnl_Left, "pnl_Left");
            this.pnl_Left.Name = "pnl_Left";
            // 
            // srmLabel38
            // 
            resources.ApplyResources(this.srmLabel38, "srmLabel38");
            this.srmLabel38.Name = "srmLabel38";
            this.srmLabel38.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_StartPixelFromLeft
            // 
            this.txt_StartPixelFromLeft.BackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromLeft.DecimalPlaces = 0;
            this.txt_StartPixelFromLeft.DecMaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.txt_StartPixelFromLeft.DecMinValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.txt_StartPixelFromLeft.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_StartPixelFromLeft.ForeColor = System.Drawing.Color.Black;
            this.txt_StartPixelFromLeft.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_StartPixelFromLeft, "txt_StartPixelFromLeft");
            this.txt_StartPixelFromLeft.Name = "txt_StartPixelFromLeft";
            this.txt_StartPixelFromLeft.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromLeft.TextChanged += new System.EventHandler(this.txt_StartPixelFromLeft_TextChanged);
            // 
            // pnl_Top
            // 
            this.pnl_Top.Controls.Add(this.srmLabel4);
            this.pnl_Top.Controls.Add(this.txt_StartPixelFromTop);
            resources.ApplyResources(this.pnl_Top, "pnl_Top");
            this.pnl_Top.Name = "pnl_Top";
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_StartPixelFromTop
            // 
            this.txt_StartPixelFromTop.BackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromTop.DecimalPlaces = 0;
            this.txt_StartPixelFromTop.DecMaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.txt_StartPixelFromTop.DecMinValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.txt_StartPixelFromTop.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_StartPixelFromTop.ForeColor = System.Drawing.Color.Black;
            this.txt_StartPixelFromTop.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_StartPixelFromTop, "txt_StartPixelFromTop");
            this.txt_StartPixelFromTop.Name = "txt_StartPixelFromTop";
            this.txt_StartPixelFromTop.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromTop.TextChanged += new System.EventHandler(this.txt_StartPixelFromTop_TextChanged);
            // 
            // picUnitROI
            // 
            this.picUnitROI.BackColor = System.Drawing.Color.Black;
            this.picUnitROI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.picUnitROI, "picUnitROI");
            this.picUnitROI.Name = "picUnitROI";
            this.picUnitROI.TabStop = false;
            // 
            // tp_DontCare
            // 
            this.tp_DontCare.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_DontCare.Controls.Add(this.radioButton_NoneDontCare);
            this.tp_DontCare.Controls.Add(this.pnl_DrawMethod);
            this.tp_DontCare.Controls.Add(this.radioButton_ManualDontCare);
            this.tp_DontCare.Controls.Add(this.radioButton_PackageDontCare);
            this.tp_DontCare.Controls.Add(this.radioButton_PadDontCare);
            resources.ApplyResources(this.tp_DontCare, "tp_DontCare");
            this.tp_DontCare.Name = "tp_DontCare";
            // 
            // radioButton_NoneDontCare
            // 
            this.radioButton_NoneDontCare.Checked = true;
            resources.ApplyResources(this.radioButton_NoneDontCare, "radioButton_NoneDontCare");
            this.radioButton_NoneDontCare.Name = "radioButton_NoneDontCare";
            this.radioButton_NoneDontCare.TabStop = true;
            this.radioButton_NoneDontCare.UseVisualStyleBackColor = true;
            this.radioButton_NoneDontCare.Click += new System.EventHandler(this.radioButton_DontCare_Click);
            // 
            // pnl_DrawMethod
            // 
            this.pnl_DrawMethod.Controls.Add(this.srmLabel66);
            this.pnl_DrawMethod.Controls.Add(this.srmLabel23);
            this.pnl_DrawMethod.Controls.Add(this.cbo_DontCareAreaDrawMethod);
            this.pnl_DrawMethod.Controls.Add(this.srmLabel6);
            this.pnl_DrawMethod.Controls.Add(this.btn_Undo);
            this.pnl_DrawMethod.Controls.Add(this.pic_Help);
            this.pnl_DrawMethod.Controls.Add(this.btn_AddDontCareROI);
            this.pnl_DrawMethod.Controls.Add(this.btn_DeleteDontCareROI);
            resources.ApplyResources(this.pnl_DrawMethod, "pnl_DrawMethod");
            this.pnl_DrawMethod.Name = "pnl_DrawMethod";
            // 
            // srmLabel66
            // 
            resources.ApplyResources(this.srmLabel66, "srmLabel66");
            this.srmLabel66.Name = "srmLabel66";
            this.srmLabel66.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel23
            // 
            resources.ApplyResources(this.srmLabel23, "srmLabel23");
            this.srmLabel23.Name = "srmLabel23";
            this.srmLabel23.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_DontCareAreaDrawMethod
            // 
            this.cbo_DontCareAreaDrawMethod.BackColor = System.Drawing.Color.White;
            this.cbo_DontCareAreaDrawMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_DontCareAreaDrawMethod.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_DontCareAreaDrawMethod.FormattingEnabled = true;
            resources.ApplyResources(this.cbo_DontCareAreaDrawMethod, "cbo_DontCareAreaDrawMethod");
            this.cbo_DontCareAreaDrawMethod.Items.AddRange(new object[] {
            resources.GetString("cbo_DontCareAreaDrawMethod.Items"),
            resources.GetString("cbo_DontCareAreaDrawMethod.Items1"),
            resources.GetString("cbo_DontCareAreaDrawMethod.Items2")});
            this.cbo_DontCareAreaDrawMethod.Name = "cbo_DontCareAreaDrawMethod";
            this.cbo_DontCareAreaDrawMethod.NormalBackColor = System.Drawing.Color.White;
            this.cbo_DontCareAreaDrawMethod.SelectedIndexChanged += new System.EventHandler(this.cbo_DontCareAreaDrawMethod_SelectedIndexChanged);
            // 
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Undo
            // 
            resources.ApplyResources(this.btn_Undo, "btn_Undo");
            this.btn_Undo.Name = "btn_Undo";
            this.btn_Undo.UseVisualStyleBackColor = true;
            this.btn_Undo.Click += new System.EventHandler(this.btn_Undo_Click);
            // 
            // pic_Help
            // 
            resources.ApplyResources(this.pic_Help, "pic_Help");
            this.pic_Help.Name = "pic_Help";
            this.pic_Help.TabStop = false;
            this.pic_Help.MouseEnter += new System.EventHandler(this.pic_Help_MouseEnter);
            this.pic_Help.MouseLeave += new System.EventHandler(this.pic_Help_MouseLeave);
            // 
            // btn_AddDontCareROI
            // 
            resources.ApplyResources(this.btn_AddDontCareROI, "btn_AddDontCareROI");
            this.btn_AddDontCareROI.Name = "btn_AddDontCareROI";
            this.btn_AddDontCareROI.UseVisualStyleBackColor = true;
            this.btn_AddDontCareROI.Click += new System.EventHandler(this.btn_AddDontCareROI_Click);
            // 
            // btn_DeleteDontCareROI
            // 
            resources.ApplyResources(this.btn_DeleteDontCareROI, "btn_DeleteDontCareROI");
            this.btn_DeleteDontCareROI.Name = "btn_DeleteDontCareROI";
            this.btn_DeleteDontCareROI.UseVisualStyleBackColor = true;
            this.btn_DeleteDontCareROI.Click += new System.EventHandler(this.btn_DeleteDontCareROI_Click);
            // 
            // radioButton_ManualDontCare
            // 
            resources.ApplyResources(this.radioButton_ManualDontCare, "radioButton_ManualDontCare");
            this.radioButton_ManualDontCare.Name = "radioButton_ManualDontCare";
            this.radioButton_ManualDontCare.UseVisualStyleBackColor = true;
            this.radioButton_ManualDontCare.Click += new System.EventHandler(this.radioButton_DontCare_Click);
            // 
            // radioButton_PackageDontCare
            // 
            resources.ApplyResources(this.radioButton_PackageDontCare, "radioButton_PackageDontCare");
            this.radioButton_PackageDontCare.Name = "radioButton_PackageDontCare";
            this.radioButton_PackageDontCare.UseVisualStyleBackColor = true;
            this.radioButton_PackageDontCare.Click += new System.EventHandler(this.radioButton_DontCare_Click);
            // 
            // radioButton_PadDontCare
            // 
            resources.ApplyResources(this.radioButton_PadDontCare, "radioButton_PadDontCare");
            this.radioButton_PadDontCare.Name = "radioButton_PadDontCare";
            this.radioButton_PadDontCare.UseVisualStyleBackColor = true;
            this.radioButton_PadDontCare.Click += new System.EventHandler(this.radioButton_DontCare_Click);
            // 
            // ils_ImageListTree
            // 
            this.ils_ImageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ils_ImageListTree.ImageStream")));
            this.ils_ImageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.ils_ImageListTree.Images.SetKeyName(0, "5S Center Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(1, "5S Top Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(2, "5S Right Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(3, "5S Bottom Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(4, "5S Left Pkg ROI.png");
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // pnl_HelpMessage
            // 
            this.pnl_HelpMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.pnl_HelpMessage.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.pnl_HelpMessage, "pnl_HelpMessage");
            this.pnl_HelpMessage.Name = "pnl_HelpMessage";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // srmGroupBox5
            // 
            this.srmGroupBox5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox5.Controls.Add(this.radioBtn_Middle);
            this.srmGroupBox5.Controls.Add(this.radioBtn_Down);
            this.srmGroupBox5.Controls.Add(this.radioBtn_Left);
            this.srmGroupBox5.Controls.Add(this.radioBtn_Up);
            this.srmGroupBox5.Controls.Add(this.radioBtn_Right);
            resources.ApplyResources(this.srmGroupBox5, "srmGroupBox5");
            this.srmGroupBox5.Name = "srmGroupBox5";
            this.srmGroupBox5.TabStop = false;
            // 
            // radioBtn_Middle
            // 
            this.radioBtn_Middle.Checked = true;
            resources.ApplyResources(this.radioBtn_Middle, "radioBtn_Middle");
            this.radioBtn_Middle.Name = "radioBtn_Middle";
            this.radioBtn_Middle.TabStop = true;
            this.radioBtn_Middle.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // radioBtn_Down
            // 
            resources.ApplyResources(this.radioBtn_Down, "radioBtn_Down");
            this.radioBtn_Down.Name = "radioBtn_Down";
            this.radioBtn_Down.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // radioBtn_Left
            // 
            resources.ApplyResources(this.radioBtn_Left, "radioBtn_Left");
            this.radioBtn_Left.Name = "radioBtn_Left";
            this.radioBtn_Left.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // radioBtn_Up
            // 
            resources.ApplyResources(this.radioBtn_Up, "radioBtn_Up");
            this.radioBtn_Up.Name = "radioBtn_Up";
            this.radioBtn_Up.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // radioBtn_Right
            // 
            resources.ApplyResources(this.radioBtn_Right, "radioBtn_Right");
            this.radioBtn_Right.Name = "radioBtn_Right";
            this.radioBtn_Right.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // btn_Up
            // 
            resources.ApplyResources(this.btn_Up, "btn_Up");
            this.btn_Up.Name = "btn_Up";
            this.btn_Up.UseVisualStyleBackColor = true;
            this.btn_Up.Click += new System.EventHandler(this.btn_Up_Click);
            // 
            // btn_Down
            // 
            resources.ApplyResources(this.btn_Down, "btn_Down");
            this.btn_Down.Name = "btn_Down";
            this.btn_Down.UseVisualStyleBackColor = true;
            this.btn_Down.Click += new System.EventHandler(this.btn_Down_Click);
            // 
            // ColorMultiThresholdForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Down);
            this.Controls.Add(this.btn_Up);
            this.Controls.Add(this.srmGroupBox5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnl_HelpMessage);
            this.Controls.Add(this.srmTabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tab_VisionControl);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.txt_ThresholdName);
            this.Controls.Add(this.btn_UpdateThreshold);
            this.Controls.Add(this.dgd_ThresholdSetting);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.txt_MinArea);
            this.Controls.Add(this.group_SelectTemplate);
            this.Controls.Add(this.btn_DeleteThreshold);
            this.Controls.Add(this.btn_AddThreshold);
            this.Controls.Add(this.chk_Preview);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorMultiThresholdForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorThresholdForm_FormClosing);
            this.Load += new System.EventHandler(this.ColorThresholdForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Value1)).EndInit();
            this.group_LSHSetting.ResumeLayout(false);
            this.group_LSHSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Value2Tolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Value1Tolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Value3Tolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Value2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Saturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Lightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Hue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Value3)).EndInit();
            this.group_ThresholdSetting.ResumeLayout(false);
            this.group_ThresholdSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Threshold)).EndInit();
            this.group_SelectTemplate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ThresholdSetting)).EndInit();
            this.tab_VisionControl.ResumeLayout(false);
            this.tp_Pin1.ResumeLayout(false);
            this.groupBox_Pin1.ResumeLayout(false);
            this.groupBox_Pin1Score.ResumeLayout(false);
            this.groupBox_Pin1Score.PerformLayout();
            this.group_Pin1Setting.ResumeLayout(false);
            this.group_Pin1Setting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Pin1Tolerance)).EndInit();
            this.tp_PH.ResumeLayout(false);
            this.group_PHSetting.ResumeLayout(false);
            this.group_PHSetting.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_ThresholdImage)).EndInit();
            this.srmTabControl1.ResumeLayout(false);
            this.tp_ColorProfile.ResumeLayout(false);
            this.group_ImageProcessingSetting.ResumeLayout(false);
            this.group_ImageProcessingSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CloseIteration)).EndInit();
            this.tp_ROITolerance.ResumeLayout(false);
            this.pnl_Bottom.ResumeLayout(false);
            this.pnl_Bottom.PerformLayout();
            this.pnl_Right.ResumeLayout(false);
            this.pnl_Right.PerformLayout();
            this.pnl_Left.ResumeLayout(false);
            this.pnl_Left.PerformLayout();
            this.pnl_Top.ResumeLayout(false);
            this.pnl_Top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitROI)).EndInit();
            this.tp_DontCare.ResumeLayout(false);
            this.pnl_DrawMethod.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Help)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnl_HelpMessage.ResumeLayout(false);
            this.srmGroupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel lbl_Lightness;
        private SRMControl.SRMInputBox txt_Value3;
        private SRMControl.SRMLabel lbl_Saturation;
        private SRMControl.SRMInputBox txt_Value2;
        private SRMControl.SRMLabel lbl_Hue;
        private System.Windows.Forms.TrackBar trackBar_Value1;
        private SRMControl.SRMInputBox txt_Value1;
        private SRMControl.SRMGroupBox group_LSHSetting;
        private System.Windows.Forms.PictureBox pic_Hue;
        private System.Windows.Forms.PictureBox pic_Saturation;
        private System.Windows.Forms.TrackBar trackBar_Value2;
        private System.Windows.Forms.PictureBox pic_Lightness;
        private System.Windows.Forms.TrackBar trackBar_Value3;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.NumericUpDown txt_Value3Tolerance;
        private SRMControl.SRMLabel lbl_LightnessTolerance;
        private SRMControl.SRMLabel lbl_HueTolerance;
        private System.Windows.Forms.NumericUpDown txt_Value1Tolerance;
        private SRMControl.SRMLabel lbl_SaturationTolerance;
        private System.Windows.Forms.NumericUpDown txt_Value2Tolerance;
        private System.Windows.Forms.Timer timer;
        private SRMControl.SRMCheckBox chk_Preview;
        private SRMControl.SRMButton btn_AddThreshold;
        private SRMControl.SRMButton btn_DeleteThreshold;
        private SRMControl.SRMGroupBox group_SelectTemplate;
        private SRMControl.SRMRadioButton radioBtn_RGB;
        private SRMControl.SRMRadioButton radioBtn_HSL;
        private SRMControl.SRMButton btn_UpdateThreshold;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_MinArea;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMInputBox txt_ThresholdName;
        private System.Windows.Forms.DataGridView dgd_ThresholdSetting;
        private SRMControl.SRMRadioButton srmRadioButton1;
        private SRMControl.SRMTabControl tab_VisionControl;
        private System.Windows.Forms.TabPage tp_Pin1;
        private SRMControl.SRMGroupBox groupBox_Pin1;
        private SRMControl.SRMGroupBox groupBox_Pin1Score;
        private SRMControl.SRMLabel lbl_PinScoreTitle;
        private SRMControl.SRMLabel lbl_Pin1ScorePercent;
        private SRMControl.SRMLabel lbl_Pin1Score;
        private SRMControl.SRMGroupBox group_Pin1Setting;
        private SRMControl.SRMLabel lbl_Pin1ToleranceScore;
        private System.Windows.Forms.TrackBar trackBar_Pin1Tolerance;
        private SRMControl.SRMLabel lbl_Pin1TolerancePercent;
        private SRMControl.SRMInputBox txt_Pin1Tolerance;
        private System.Windows.Forms.TabPage tp_PH;
        private SRMControl.SRMGroupBox group_PHSetting;
        private System.Windows.Forms.Label lbl_PHBlobBlackArea;
        private SRMControl.SRMLabel srmLabel25;
        private SRMControl.SRMInputBox txt_BlackAreaArea;
        private SRMControl.SRMLabel srmLabel29;
        private SRMControl.SRMGroupBox group_ThresholdSetting;
        private SRMControl.SRMLabel srmLabel8;
        private System.Windows.Forms.TrackBar trackBar_Threshold;
        private SRMControl.SRMInputBox txt_Threshold;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pic_ThresholdImage;
        private SRMControl.SRMTabControl srmTabControl1;
        private System.Windows.Forms.TabPage tp_ColorProfile;
        private System.Windows.Forms.TabPage tp_ROITolerance;
        private System.Windows.Forms.TabPage tp_DontCare;
        private System.Windows.Forms.Panel pnl_Bottom;
        private SRMControl.SRMLabel srmLabel36;
        private SRMControl.SRMInputBox txt_StartPixelFromBottom;
        private System.Windows.Forms.Panel pnl_Right;
        private SRMControl.SRMLabel srmLabel34;
        private SRMControl.SRMInputBox txt_StartPixelFromRight;
        private System.Windows.Forms.Panel pnl_Left;
        private SRMControl.SRMLabel srmLabel38;
        private SRMControl.SRMInputBox txt_StartPixelFromLeft;
        private System.Windows.Forms.Panel pnl_Top;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMInputBox txt_StartPixelFromTop;
        private System.Windows.Forms.PictureBox picUnitROI;
        private SRMControl.SRMCheckBox chk_SetToAllEdge;
        private SRMControl.SRMRadioButton radioButton_ManualDontCare;
        private SRMControl.SRMRadioButton radioButton_PackageDontCare;
        private SRMControl.SRMRadioButton radioButton_PadDontCare;
        private System.Windows.Forms.ImageList ils_ImageListTree;
        private SRMControl.SRMLabel srmLabel66;
        private SRMControl.SRMComboBox cbo_DontCareAreaDrawMethod;
        private SRMControl.SRMButton btn_Undo;
        private SRMControl.SRMButton btn_DeleteDontCareROI;
        private SRMControl.SRMButton btn_AddDontCareROI;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pic_Help;
        private SRMControl.SRMLabel srmLabel23;
        private SRMControl.SRMLabel srmLabel6;
        private System.Windows.Forms.Panel pnl_HelpMessage;
        private System.Windows.Forms.Panel pnl_DrawMethod;
        private SRMControl.SRMRadioButton radioButton_NoneDontCare;
        private System.Windows.Forms.Label label1;
        private SRMControl.SRMGroupBox srmGroupBox5;
        private SRMControl.SRMRadioButton radioBtn_Middle;
        private SRMControl.SRMRadioButton radioBtn_Down;
        private SRMControl.SRMRadioButton radioBtn_Left;
        private SRMControl.SRMRadioButton radioBtn_Up;
        private SRMControl.SRMRadioButton radioBtn_Right;
        private SRMControl.SRMGroupBox group_ImageProcessingSetting;
        private SRMControl.SRMLabel srmLabel3;
        private System.Windows.Forms.NumericUpDown txt_CloseIteration;
        private SRMControl.SRMCheckBox chk_InvertBlackWhite;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_ThresholdNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_ThresName;
        private System.Windows.Forms.DataGridViewComboBoxColumn column_ColorSystem;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Value1;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Tolerance1;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Value2;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Tolerance2;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Value3;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Tolerance3;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_MinArea;
        private System.Windows.Forms.DataGridViewComboBoxColumn col_Type;
        private System.Windows.Forms.DataGridViewComboBoxColumn col_ImageNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_CloseIteration;
        private System.Windows.Forms.DataGridViewCheckBoxColumn column_Invert;
        private System.Windows.Forms.Button btn_Up;
        private System.Windows.Forms.Button btn_Down;
    }
}