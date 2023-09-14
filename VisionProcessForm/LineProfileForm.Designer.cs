namespace VisionProcessForm
{
    partial class LineProfileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LineProfileForm));
            this.pic_Histogram = new System.Windows.Forms.PictureBox();
            this.gb_EdgePolarity = new System.Windows.Forms.GroupBox();
            this.radioBtn_WhiteToBlack = new SRMControl.SRMRadioButton();
            this.radioBtn_BlackToWhite = new SRMControl.SRMRadioButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioBtn_Close = new SRMControl.SRMRadioButton();
            this.radioBtn_LargeAmplitude = new SRMControl.SRMRadioButton();
            this.radioBtn_FromEnd = new SRMControl.SRMRadioButton();
            this.radioBtn_FromBegin = new SRMControl.SRMRadioButton();
            this.srmLabel14 = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.txt_threshold = new SRMControl.SRMInputBox();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.txt_MeasThickness = new SRMControl.SRMInputBox();
            this.trackBar_Thickness = new System.Windows.Forms.TrackBar();
            this.trackBar_Derivative = new System.Windows.Forms.TrackBar();
            this.panel_SelectEdge = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chk_SetToAll = new SRMControl.SRMCheckBox();
            this.cbo_ROI = new SRMControl.SRMComboBox();
            this.lbl_ROI = new SRMControl.SRMLabel();
            this.cbo_PadNo = new SRMControl.SRMComboBox();
            this.pic_Right = new System.Windows.Forms.PictureBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.pic_Left = new System.Windows.Forms.PictureBox();
            this.pic_Down = new System.Windows.Forms.PictureBox();
            this.pic_Up = new System.Windows.Forms.PictureBox();
            this.radioBtn_Left = new SRMControl.SRMRadioButton();
            this.radioBtn_Down = new SRMControl.SRMRadioButton();
            this.radioBtn_Up = new SRMControl.SRMRadioButton();
            this.radioBtn_Right = new SRMControl.SRMRadioButton();
            this.panel_Setting = new System.Windows.Forms.Panel();
            this.trackBar_MinAmp = new System.Windows.Forms.TrackBar();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.txt_MeasMinAmp = new SRMControl.SRMInputBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Histogram)).BeginInit();
            this.gb_EdgePolarity.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Thickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Derivative)).BeginInit();
            this.panel_SelectEdge.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Right)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Left)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Down)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Up)).BeginInit();
            this.panel_Setting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_MinAmp)).BeginInit();
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
            // gb_EdgePolarity
            // 
            this.gb_EdgePolarity.BackColor = System.Drawing.Color.Transparent;
            this.gb_EdgePolarity.Controls.Add(this.radioBtn_WhiteToBlack);
            this.gb_EdgePolarity.Controls.Add(this.radioBtn_BlackToWhite);
            resources.ApplyResources(this.gb_EdgePolarity, "gb_EdgePolarity");
            this.gb_EdgePolarity.Name = "gb_EdgePolarity";
            this.gb_EdgePolarity.TabStop = false;
            // 
            // radioBtn_WhiteToBlack
            // 
            this.radioBtn_WhiteToBlack.BackColor = System.Drawing.Color.Transparent;
            this.radioBtn_WhiteToBlack.Checked = true;
            resources.ApplyResources(this.radioBtn_WhiteToBlack, "radioBtn_WhiteToBlack");
            this.radioBtn_WhiteToBlack.Name = "radioBtn_WhiteToBlack";
            this.radioBtn_WhiteToBlack.TabStop = true;
            this.radioBtn_WhiteToBlack.UseVisualStyleBackColor = false;
            this.radioBtn_WhiteToBlack.Click += new System.EventHandler(this.radioBtn_WhiteToBlack_Click);
            // 
            // radioBtn_BlackToWhite
            // 
            this.radioBtn_BlackToWhite.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioBtn_BlackToWhite, "radioBtn_BlackToWhite");
            this.radioBtn_BlackToWhite.Name = "radioBtn_BlackToWhite";
            this.radioBtn_BlackToWhite.UseVisualStyleBackColor = false;
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
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioBtn_Close);
            this.groupBox3.Controls.Add(this.radioBtn_LargeAmplitude);
            this.groupBox3.Controls.Add(this.radioBtn_FromEnd);
            this.groupBox3.Controls.Add(this.radioBtn_FromBegin);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioBtn_Close
            // 
            this.radioBtn_Close.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioBtn_Close, "radioBtn_Close");
            this.radioBtn_Close.Name = "radioBtn_Close";
            this.radioBtn_Close.UseVisualStyleBackColor = false;
            this.radioBtn_Close.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // radioBtn_LargeAmplitude
            // 
            this.radioBtn_LargeAmplitude.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioBtn_LargeAmplitude, "radioBtn_LargeAmplitude");
            this.radioBtn_LargeAmplitude.Name = "radioBtn_LargeAmplitude";
            this.radioBtn_LargeAmplitude.UseVisualStyleBackColor = false;
            this.radioBtn_LargeAmplitude.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // radioBtn_FromEnd
            // 
            this.radioBtn_FromEnd.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioBtn_FromEnd, "radioBtn_FromEnd");
            this.radioBtn_FromEnd.Name = "radioBtn_FromEnd";
            this.radioBtn_FromEnd.UseVisualStyleBackColor = false;
            this.radioBtn_FromEnd.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // radioBtn_FromBegin
            // 
            this.radioBtn_FromBegin.BackColor = System.Drawing.Color.Transparent;
            this.radioBtn_FromBegin.Checked = true;
            resources.ApplyResources(this.radioBtn_FromBegin, "radioBtn_FromBegin");
            this.radioBtn_FromBegin.Name = "radioBtn_FromBegin";
            this.radioBtn_FromBegin.TabStop = true;
            this.radioBtn_FromBegin.UseVisualStyleBackColor = false;
            this.radioBtn_FromBegin.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // srmLabel14
            // 
            resources.ApplyResources(this.srmLabel14, "srmLabel14");
            this.srmLabel14.Name = "srmLabel14";
            this.srmLabel14.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
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
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
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
            // panel_SelectEdge
            // 
            this.panel_SelectEdge.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.panel_SelectEdge, "panel_SelectEdge");
            this.panel_SelectEdge.Name = "panel_SelectEdge";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chk_SetToAll);
            this.groupBox1.Controls.Add(this.cbo_ROI);
            this.groupBox1.Controls.Add(this.lbl_ROI);
            this.groupBox1.Controls.Add(this.cbo_PadNo);
            this.groupBox1.Controls.Add(this.pic_Right);
            this.groupBox1.Controls.Add(this.srmLabel1);
            this.groupBox1.Controls.Add(this.pic_Left);
            this.groupBox1.Controls.Add(this.pic_Down);
            this.groupBox1.Controls.Add(this.pic_Up);
            this.groupBox1.Controls.Add(this.radioBtn_Left);
            this.groupBox1.Controls.Add(this.radioBtn_Down);
            this.groupBox1.Controls.Add(this.radioBtn_Up);
            this.groupBox1.Controls.Add(this.radioBtn_Right);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // chk_SetToAll
            // 
            this.chk_SetToAll.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_SetToAll, "chk_SetToAll");
            this.chk_SetToAll.Name = "chk_SetToAll";
            this.chk_SetToAll.Selected = false;
            this.chk_SetToAll.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAll.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAll.UseVisualStyleBackColor = true;
            this.chk_SetToAll.Click += new System.EventHandler(this.chk_SetToAll_Click);
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
            // lbl_ROI
            // 
            resources.ApplyResources(this.lbl_ROI, "lbl_ROI");
            this.lbl_ROI.Name = "lbl_ROI";
            this.lbl_ROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_PadNo
            // 
            this.cbo_PadNo.BackColor = System.Drawing.Color.White;
            this.cbo_PadNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_PadNo.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.cbo_PadNo, "cbo_PadNo");
            this.cbo_PadNo.FormattingEnabled = true;
            this.cbo_PadNo.Name = "cbo_PadNo";
            this.cbo_PadNo.NormalBackColor = System.Drawing.Color.White;
            this.cbo_PadNo.SelectedIndexChanged += new System.EventHandler(this.cbo_PadNo_SelectedIndexChanged);
            // 
            // pic_Right
            // 
            this.pic_Right.BackColor = System.Drawing.Color.Black;
            this.pic_Right.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pic_Right, "pic_Right");
            this.pic_Right.Name = "pic_Right";
            this.pic_Right.TabStop = false;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pic_Left
            // 
            this.pic_Left.BackColor = System.Drawing.Color.Black;
            this.pic_Left.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pic_Left, "pic_Left");
            this.pic_Left.Name = "pic_Left";
            this.pic_Left.TabStop = false;
            // 
            // pic_Down
            // 
            this.pic_Down.BackColor = System.Drawing.Color.Black;
            this.pic_Down.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pic_Down, "pic_Down");
            this.pic_Down.Name = "pic_Down";
            this.pic_Down.TabStop = false;
            // 
            // pic_Up
            // 
            this.pic_Up.BackColor = System.Drawing.Color.Black;
            this.pic_Up.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pic_Up, "pic_Up");
            this.pic_Up.Name = "pic_Up";
            this.pic_Up.TabStop = false;
            // 
            // radioBtn_Left
            // 
            resources.ApplyResources(this.radioBtn_Left, "radioBtn_Left");
            this.radioBtn_Left.Name = "radioBtn_Left";
            this.radioBtn_Left.UseVisualStyleBackColor = true;
            this.radioBtn_Left.Click += new System.EventHandler(this.radioBtn_Left_Click);
            // 
            // radioBtn_Down
            // 
            resources.ApplyResources(this.radioBtn_Down, "radioBtn_Down");
            this.radioBtn_Down.Name = "radioBtn_Down";
            this.radioBtn_Down.Click += new System.EventHandler(this.radioBtn_Left_Click);
            // 
            // radioBtn_Up
            // 
            this.radioBtn_Up.Checked = true;
            resources.ApplyResources(this.radioBtn_Up, "radioBtn_Up");
            this.radioBtn_Up.Name = "radioBtn_Up";
            this.radioBtn_Up.TabStop = true;
            this.radioBtn_Up.Click += new System.EventHandler(this.radioBtn_Left_Click);
            // 
            // radioBtn_Right
            // 
            resources.ApplyResources(this.radioBtn_Right, "radioBtn_Right");
            this.radioBtn_Right.Name = "radioBtn_Right";
            this.radioBtn_Right.Click += new System.EventHandler(this.radioBtn_Left_Click);
            // 
            // panel_Setting
            // 
            this.panel_Setting.Controls.Add(this.trackBar_MinAmp);
            this.panel_Setting.Controls.Add(this.srmLabel10);
            this.panel_Setting.Controls.Add(this.txt_MeasMinAmp);
            this.panel_Setting.Controls.Add(this.gb_EdgePolarity);
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
            // trackBar_MinAmp
            // 
            resources.ApplyResources(this.trackBar_MinAmp, "trackBar_MinAmp");
            this.trackBar_MinAmp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_MinAmp.LargeChange = 1;
            this.trackBar_MinAmp.Maximum = 100;
            this.trackBar_MinAmp.Name = "trackBar_MinAmp";
            this.trackBar_MinAmp.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_MinAmp.Value = 50;
            this.trackBar_MinAmp.Scroll += new System.EventHandler(this.trackBar_MinAmp_Scroll);
            // 
            // srmLabel10
            // 
            resources.ApplyResources(this.srmLabel10, "srmLabel10");
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MeasMinAmp
            // 
            this.txt_MeasMinAmp.BackColor = System.Drawing.Color.White;
            this.txt_MeasMinAmp.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasMinAmp.DecimalPlaces = 0;
            this.txt_MeasMinAmp.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasMinAmp.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MeasMinAmp.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_MeasMinAmp, "txt_MeasMinAmp");
            this.txt_MeasMinAmp.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasMinAmp.InputType = SRMControl.InputType.Number;
            this.txt_MeasMinAmp.Name = "txt_MeasMinAmp";
            this.txt_MeasMinAmp.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasMinAmp.TextChanged += new System.EventHandler(this.txt_MeasMinAmp_TextChanged);
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
            // LineProfileForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_Setting);
            this.Controls.Add(this.panel_SelectEdge);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LineProfileForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LineProfileForm_FormClosing);
            this.Load += new System.EventHandler(this.LineProfileForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Histogram)).EndInit();
            this.gb_EdgePolarity.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Thickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Derivative)).EndInit();
            this.panel_SelectEdge.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Right)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Left)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Down)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Up)).EndInit();
            this.panel_Setting.ResumeLayout(false);
            this.panel_Setting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_MinAmp)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pic_Histogram;
        private System.Windows.Forms.GroupBox gb_EdgePolarity;
        private SRMControl.SRMRadioButton radioBtn_WhiteToBlack;
        private SRMControl.SRMRadioButton radioBtn_BlackToWhite;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.Timer Timer;
        private SRMControl.SRMLabel srmLabel9;
        private System.Windows.Forms.GroupBox groupBox3;
        private SRMControl.SRMRadioButton radioBtn_FromEnd;
        private SRMControl.SRMRadioButton radioBtn_FromBegin;
        private SRMControl.SRMRadioButton radioBtn_LargeAmplitude;
        private SRMControl.SRMLabel srmLabel14;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMInputBox txt_MeasThickness;
        private System.Windows.Forms.TrackBar trackBar_Thickness;
        private System.Windows.Forms.TrackBar trackBar_Derivative;
        private System.Windows.Forms.Panel panel_SelectEdge;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pic_Right;
        private System.Windows.Forms.PictureBox pic_Left;
        private System.Windows.Forms.PictureBox pic_Down;
        private System.Windows.Forms.PictureBox pic_Up;
        private SRMControl.SRMRadioButton radioBtn_Left;
        private SRMControl.SRMRadioButton radioBtn_Down;
        private SRMControl.SRMRadioButton radioBtn_Up;
        private SRMControl.SRMRadioButton radioBtn_Right;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMComboBox cbo_PadNo;
        private System.Windows.Forms.Panel panel_Setting;
        private System.Windows.Forms.Panel panel1;
        private SRMControl.SRMInputBox txt_threshold;
        private SRMControl.SRMComboBox cbo_ROI;
        private SRMControl.SRMLabel lbl_ROI;
        private System.Windows.Forms.ImageList imageList1;
        private SRMControl.SRMCheckBox chk_SetToAll;
        private System.Windows.Forms.TrackBar trackBar_MinAmp;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMInputBox txt_MeasMinAmp;
        private SRMControl.SRMRadioButton radioBtn_Close;
    }
}