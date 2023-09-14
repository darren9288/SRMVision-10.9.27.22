namespace VisionProcessForm
{
    partial class LeadLineProfileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LeadLineProfileForm));
            this.pic_Histogram = new System.Windows.Forms.PictureBox();
            this.radioBtn_WhiteToBlack = new SRMControl.SRMRadioButton();
            this.radioBtn_BlackToWhite = new SRMControl.SRMRadioButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.radioBtn_LargestAmplitude = new SRMControl.SRMRadioButton();
            this.radioBtn_FromBegin = new SRMControl.SRMRadioButton();
            this.radioBtn_FromEnd = new SRMControl.SRMRadioButton();
            this.pnl_Center = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbo_ROI = new SRMControl.SRMComboBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.radioBtn_CenterTipEnd = new SRMControl.SRMRadioButton();
            this.radioBtn_CenterTipStart = new SRMControl.SRMRadioButton();
            this.cbo_LeadNo_Center = new SRMControl.SRMComboBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.radioBtn_CenterTipCenter = new SRMControl.SRMRadioButton();
            this.radioBtn_CenterBaseEnd = new SRMControl.SRMRadioButton();
            this.radioBtn_CenterBaseStart = new SRMControl.SRMRadioButton();
            this.radioBtn_CenterBaseCenter = new SRMControl.SRMRadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel_Setting = new System.Windows.Forms.Panel();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.txt_MinAmplitude = new SRMControl.SRMInputBox();
            this.trackBar_MinAmplitude = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel14 = new SRMControl.SRMLabel();
            this.txt_threshold = new SRMControl.SRMInputBox();
            this.trackBar_Derivative = new System.Windows.Forms.TrackBar();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.trackBar_Thickness = new System.Windows.Forms.TrackBar();
            this.txt_MeasThickness = new SRMControl.SRMInputBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.chk_AllLeads = new SRMControl.SRMCheckBox();
            this.chk_AllROIs = new SRMControl.SRMCheckBox();
            this.chk_AllPoints = new SRMControl.SRMCheckBox();
            this.srmLabel4 = new SRMControl.SRMLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Histogram)).BeginInit();
            this.pnl_Center.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            this.panel_Setting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_MinAmplitude)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Derivative)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Thickness)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pic_Histogram
            // 
            this.pic_Histogram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            resources.ApplyResources(this.pic_Histogram, "pic_Histogram");
            this.pic_Histogram.Name = "pic_Histogram";
            this.pic_Histogram.TabStop = false;
            this.pic_Histogram.Click += new System.EventHandler(this.pic_Histogram_Click);
            this.pic_Histogram.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_Histogram_Paint);
            // 
            // radioBtn_WhiteToBlack
            // 
            this.radioBtn_WhiteToBlack.Checked = true;
            resources.ApplyResources(this.radioBtn_WhiteToBlack, "radioBtn_WhiteToBlack");
            this.radioBtn_WhiteToBlack.Name = "radioBtn_WhiteToBlack";
            this.radioBtn_WhiteToBlack.TabStop = true;
            this.radioBtn_WhiteToBlack.Click += new System.EventHandler(this.radioBtn_WhiteToBlack_Click);
            // 
            // radioBtn_BlackToWhite
            // 
            resources.ApplyResources(this.radioBtn_BlackToWhite, "radioBtn_BlackToWhite");
            this.radioBtn_BlackToWhite.Name = "radioBtn_BlackToWhite";
            this.radioBtn_BlackToWhite.UseVisualStyleBackColor = true;
            this.radioBtn_BlackToWhite.Click += new System.EventHandler(this.radioBtn_WhiteToBlack_Click);
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
            // Timer
            // 
            this.Timer.Enabled = true;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // radioBtn_LargestAmplitude
            // 
            resources.ApplyResources(this.radioBtn_LargestAmplitude, "radioBtn_LargestAmplitude");
            this.radioBtn_LargestAmplitude.Name = "radioBtn_LargestAmplitude";
            this.radioBtn_LargestAmplitude.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // radioBtn_FromBegin
            // 
            this.radioBtn_FromBegin.Checked = true;
            resources.ApplyResources(this.radioBtn_FromBegin, "radioBtn_FromBegin");
            this.radioBtn_FromBegin.Name = "radioBtn_FromBegin";
            this.radioBtn_FromBegin.TabStop = true;
            this.radioBtn_FromBegin.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // radioBtn_FromEnd
            // 
            resources.ApplyResources(this.radioBtn_FromEnd, "radioBtn_FromEnd");
            this.radioBtn_FromEnd.Name = "radioBtn_FromEnd";
            this.radioBtn_FromEnd.UseVisualStyleBackColor = true;
            this.radioBtn_FromEnd.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // pnl_Center
            // 
            this.pnl_Center.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.pnl_Center, "pnl_Center");
            this.pnl_Center.Name = "pnl_Center";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbo_ROI);
            this.groupBox2.Controls.Add(this.srmLabel2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.pictureBox9);
            this.groupBox2.Controls.Add(this.radioBtn_CenterTipEnd);
            this.groupBox2.Controls.Add(this.radioBtn_CenterTipStart);
            this.groupBox2.Controls.Add(this.cbo_LeadNo_Center);
            this.groupBox2.Controls.Add(this.srmLabel1);
            this.groupBox2.Controls.Add(this.radioBtn_CenterTipCenter);
            this.groupBox2.Controls.Add(this.radioBtn_CenterBaseEnd);
            this.groupBox2.Controls.Add(this.radioBtn_CenterBaseStart);
            this.groupBox2.Controls.Add(this.radioBtn_CenterBaseCenter);
            this.groupBox2.Controls.Add(this.panel3);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cbo_ROI
            // 
            this.cbo_ROI.BackColor = System.Drawing.Color.White;
            this.cbo_ROI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ROI.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.cbo_ROI, "cbo_ROI");
            this.cbo_ROI.FormattingEnabled = true;
            this.cbo_ROI.Name = "cbo_ROI";
            this.cbo_ROI.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ROI.SelectedIndexChanged += new System.EventHandler(this.cbo_ROI_SelectedIndexChanged);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Name = "label4";
            // 
            // pictureBox9
            // 
            resources.ApplyResources(this.pictureBox9, "pictureBox9");
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.TabStop = false;
            // 
            // radioBtn_CenterTipEnd
            // 
            resources.ApplyResources(this.radioBtn_CenterTipEnd, "radioBtn_CenterTipEnd");
            this.radioBtn_CenterTipEnd.Name = "radioBtn_CenterTipEnd";
            this.radioBtn_CenterTipEnd.UseVisualStyleBackColor = true;
            this.radioBtn_CenterTipEnd.Click += new System.EventHandler(this.radioBtn_CenterDirection_Click);
            // 
            // radioBtn_CenterTipStart
            // 
            resources.ApplyResources(this.radioBtn_CenterTipStart, "radioBtn_CenterTipStart");
            this.radioBtn_CenterTipStart.Name = "radioBtn_CenterTipStart";
            this.radioBtn_CenterTipStart.UseVisualStyleBackColor = true;
            this.radioBtn_CenterTipStart.Click += new System.EventHandler(this.radioBtn_CenterDirection_Click);
            // 
            // cbo_LeadNo_Center
            // 
            this.cbo_LeadNo_Center.BackColor = System.Drawing.Color.White;
            this.cbo_LeadNo_Center.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_LeadNo_Center.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.cbo_LeadNo_Center, "cbo_LeadNo_Center");
            this.cbo_LeadNo_Center.FormattingEnabled = true;
            this.cbo_LeadNo_Center.Name = "cbo_LeadNo_Center";
            this.cbo_LeadNo_Center.NormalBackColor = System.Drawing.Color.White;
            this.cbo_LeadNo_Center.SelectedIndexChanged += new System.EventHandler(this.cbo_LeadNo_Center_SelectedIndexChanged);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // radioBtn_CenterTipCenter
            // 
            resources.ApplyResources(this.radioBtn_CenterTipCenter, "radioBtn_CenterTipCenter");
            this.radioBtn_CenterTipCenter.Name = "radioBtn_CenterTipCenter";
            this.radioBtn_CenterTipCenter.UseVisualStyleBackColor = true;
            this.radioBtn_CenterTipCenter.Click += new System.EventHandler(this.radioBtn_CenterDirection_Click);
            // 
            // radioBtn_CenterBaseEnd
            // 
            resources.ApplyResources(this.radioBtn_CenterBaseEnd, "radioBtn_CenterBaseEnd");
            this.radioBtn_CenterBaseEnd.Name = "radioBtn_CenterBaseEnd";
            this.radioBtn_CenterBaseEnd.Click += new System.EventHandler(this.radioBtn_CenterDirection_Click);
            // 
            // radioBtn_CenterBaseStart
            // 
            resources.ApplyResources(this.radioBtn_CenterBaseStart, "radioBtn_CenterBaseStart");
            this.radioBtn_CenterBaseStart.Name = "radioBtn_CenterBaseStart";
            this.radioBtn_CenterBaseStart.Click += new System.EventHandler(this.radioBtn_CenterDirection_Click);
            // 
            // radioBtn_CenterBaseCenter
            // 
            resources.ApplyResources(this.radioBtn_CenterBaseCenter, "radioBtn_CenterBaseCenter");
            this.radioBtn_CenterBaseCenter.Checked = true;
            this.radioBtn_CenterBaseCenter.Name = "radioBtn_CenterBaseCenter";
            this.radioBtn_CenterBaseCenter.TabStop = true;
            this.radioBtn_CenterBaseCenter.Click += new System.EventHandler(this.radioBtn_CenterDirection_Click);
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // panel_Setting
            // 
            this.panel_Setting.Controls.Add(this.srmLabel5);
            this.panel_Setting.Controls.Add(this.txt_MinAmplitude);
            this.panel_Setting.Controls.Add(this.trackBar_MinAmplitude);
            this.panel_Setting.Controls.Add(this.groupBox1);
            this.panel_Setting.Controls.Add(this.groupBox3);
            this.panel_Setting.Controls.Add(this.srmLabel9);
            this.panel_Setting.Controls.Add(this.srmLabel3);
            this.panel_Setting.Controls.Add(this.srmLabel14);
            this.panel_Setting.Controls.Add(this.txt_threshold);
            this.panel_Setting.Controls.Add(this.trackBar_Derivative);
            this.panel_Setting.Controls.Add(this.srmLabel12);
            this.panel_Setting.Controls.Add(this.trackBar_Thickness);
            this.panel_Setting.Controls.Add(this.txt_MeasThickness);
            resources.ApplyResources(this.panel_Setting, "panel_Setting");
            this.panel_Setting.Name = "panel_Setting";
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinAmplitude
            // 
            this.txt_MinAmplitude.BackColor = System.Drawing.Color.White;
            this.txt_MinAmplitude.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MinAmplitude.DecimalPlaces = 0;
            this.txt_MinAmplitude.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MinAmplitude.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinAmplitude.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_MinAmplitude, "txt_MinAmplitude");
            this.txt_MinAmplitude.ForeColor = System.Drawing.Color.Black;
            this.txt_MinAmplitude.InputType = SRMControl.InputType.Number;
            this.txt_MinAmplitude.Name = "txt_MinAmplitude";
            this.txt_MinAmplitude.NormalBackColor = System.Drawing.Color.White;
            this.txt_MinAmplitude.TextChanged += new System.EventHandler(this.txt_MinAmplitude_TextChanged);
            // 
            // trackBar_MinAmplitude
            // 
            resources.ApplyResources(this.trackBar_MinAmplitude, "trackBar_MinAmplitude");
            this.trackBar_MinAmplitude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_MinAmplitude.LargeChange = 1;
            this.trackBar_MinAmplitude.Maximum = 100;
            this.trackBar_MinAmplitude.Name = "trackBar_MinAmplitude";
            this.trackBar_MinAmplitude.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_MinAmplitude.Value = 50;
            this.trackBar_MinAmplitude.Scroll += new System.EventHandler(this.trackBar_MinAmplitude_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.radioBtn_WhiteToBlack);
            this.groupBox1.Controls.Add(this.radioBtn_BlackToWhite);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioBtn_LargestAmplitude);
            this.groupBox3.Controls.Add(this.radioBtn_FromBegin);
            this.groupBox3.Controls.Add(this.radioBtn_FromEnd);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel14
            // 
            resources.ApplyResources(this.srmLabel14, "srmLabel14");
            this.srmLabel14.Name = "srmLabel14";
            this.srmLabel14.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_threshold
            // 
            this.txt_threshold.BackColor = System.Drawing.Color.White;
            this.txt_threshold.DataType = SRMControl.SRMDataType.Int32;
            this.txt_threshold.DecimalPlaces = 0;
            this.txt_threshold.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_threshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_threshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_threshold, "txt_threshold");
            this.txt_threshold.ForeColor = System.Drawing.Color.Black;
            this.txt_threshold.InputType = SRMControl.InputType.Number;
            this.txt_threshold.Name = "txt_threshold";
            this.txt_threshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_threshold.TextChanged += new System.EventHandler(this.txt_threshold_TextChanged);
            // 
            // trackBar_Derivative
            // 
            resources.ApplyResources(this.trackBar_Derivative, "trackBar_Derivative");
            this.trackBar_Derivative.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Derivative.LargeChange = 1;
            this.trackBar_Derivative.Maximum = 100;
            this.trackBar_Derivative.Name = "trackBar_Derivative";
            this.trackBar_Derivative.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Derivative.Value = 50;
            this.trackBar_Derivative.Scroll += new System.EventHandler(this.trackBar_Derivative_Scroll);
            // 
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_Thickness
            // 
            resources.ApplyResources(this.trackBar_Thickness, "trackBar_Thickness");
            this.trackBar_Thickness.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Thickness.LargeChange = 1;
            this.trackBar_Thickness.Maximum = 100;
            this.trackBar_Thickness.Minimum = 1;
            this.trackBar_Thickness.Name = "trackBar_Thickness";
            this.trackBar_Thickness.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Thickness.Value = 50;
            this.trackBar_Thickness.Scroll += new System.EventHandler(this.trackBar_Thickness_Scroll);
            // 
            // txt_MeasThickness
            // 
            this.txt_MeasThickness.BackColor = System.Drawing.Color.White;
            this.txt_MeasThickness.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasThickness.DecimalPlaces = 0;
            this.txt_MeasThickness.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasThickness.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_MeasThickness.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_MeasThickness, "txt_MeasThickness");
            this.txt_MeasThickness.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasThickness.InputType = SRMControl.InputType.Number;
            this.txt_MeasThickness.Name = "txt_MeasThickness";
            this.txt_MeasThickness.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasThickness.TextChanged += new System.EventHandler(this.txt_MeasThickness_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pic_Histogram);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "bw_ToDown.png");
            this.imageList1.Images.SetKeyName(1, "bw_ToLeft.png");
            this.imageList1.Images.SetKeyName(2, "bw_ToTop.png");
            this.imageList1.Images.SetKeyName(3, "bw_ToRight.png");
            this.imageList1.Images.SetKeyName(4, "wb_ToDown.png");
            this.imageList1.Images.SetKeyName(5, "wb_ToLeft.png");
            this.imageList1.Images.SetKeyName(6, "wb_ToTop.png");
            this.imageList1.Images.SetKeyName(7, "wb_ToRight.png");
            // 
            // chk_AllLeads
            // 
            this.chk_AllLeads.Checked = true;
            this.chk_AllLeads.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AllLeads.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.chk_AllLeads, "chk_AllLeads");
            this.chk_AllLeads.Name = "chk_AllLeads";
            this.chk_AllLeads.Selected = false;
            this.chk_AllLeads.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AllLeads.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AllLeads.UseVisualStyleBackColor = true;
            this.chk_AllLeads.Click += new System.EventHandler(this.chk_AllLeads_Click);
            // 
            // chk_AllROIs
            // 
            this.chk_AllROIs.Checked = true;
            this.chk_AllROIs.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AllROIs.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.chk_AllROIs, "chk_AllROIs");
            this.chk_AllROIs.Name = "chk_AllROIs";
            this.chk_AllROIs.Selected = false;
            this.chk_AllROIs.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AllROIs.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AllROIs.UseVisualStyleBackColor = true;
            this.chk_AllROIs.Click += new System.EventHandler(this.chk_AllROIs_Click);
            // 
            // chk_AllPoints
            // 
            this.chk_AllPoints.Checked = true;
            this.chk_AllPoints.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AllPoints.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.chk_AllPoints, "chk_AllPoints");
            this.chk_AllPoints.Name = "chk_AllPoints";
            this.chk_AllPoints.Selected = false;
            this.chk_AllPoints.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AllPoints.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AllPoints.UseVisualStyleBackColor = true;
            this.chk_AllPoints.Click += new System.EventHandler(this.chk_AllPoints_Click);
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            this.srmLabel4.Click += new System.EventHandler(this.srmLabel4_Click);
            // 
            // LeadLineProfileForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.chk_AllLeads);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.chk_AllPoints);
            this.Controls.Add(this.chk_AllROIs);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_Setting);
            this.Controls.Add(this.pnl_Center);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LeadLineProfileForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LineProfileForm_FormClosing);
            this.Load += new System.EventHandler(this.LineProfileForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Histogram)).EndInit();
            this.pnl_Center.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            this.panel_Setting.ResumeLayout(false);
            this.panel_Setting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_MinAmplitude)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Derivative)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Thickness)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pic_Histogram;
        private SRMControl.SRMRadioButton radioBtn_WhiteToBlack;
        private SRMControl.SRMRadioButton radioBtn_BlackToWhite;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.Timer Timer;
        private SRMControl.SRMRadioButton radioBtn_LargestAmplitude;
        private SRMControl.SRMRadioButton radioBtn_FromBegin;
        private SRMControl.SRMRadioButton radioBtn_FromEnd;
        private System.Windows.Forms.Panel pnl_Center;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox9;
        private SRMControl.SRMRadioButton radioBtn_CenterTipEnd;
        private SRMControl.SRMRadioButton radioBtn_CenterTipStart;
        private SRMControl.SRMComboBox cbo_LeadNo_Center;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMRadioButton radioBtn_CenterTipCenter;
        private SRMControl.SRMRadioButton radioBtn_CenterBaseEnd;
        private SRMControl.SRMRadioButton radioBtn_CenterBaseStart;
        private SRMControl.SRMRadioButton radioBtn_CenterBaseCenter;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel_Setting;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel14;
        private SRMControl.SRMInputBox txt_threshold;
        private System.Windows.Forms.TrackBar trackBar_Derivative;
        private SRMControl.SRMLabel srmLabel12;
        private System.Windows.Forms.TrackBar trackBar_Thickness;
        private SRMControl.SRMInputBox txt_MeasThickness;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ImageList imageList1;
        private SRMControl.SRMComboBox cbo_ROI;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMCheckBox chk_AllLeads;
        private SRMControl.SRMCheckBox chk_AllROIs;
        private SRMControl.SRMCheckBox chk_AllPoints;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMInputBox txt_MinAmplitude;
        private System.Windows.Forms.TrackBar trackBar_MinAmplitude;
    }
}