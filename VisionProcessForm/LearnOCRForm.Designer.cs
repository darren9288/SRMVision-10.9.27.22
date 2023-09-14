namespace VisionProcessForm
{
    partial class LearnOCRForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearnOCRForm));
            this.btn_RemoveSubROI = new System.Windows.Forms.Button();
            this.btn_AddSubROI = new System.Windows.Forms.Button();
            this.cbo_SubROIList = new SRMControl.SRMComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.srmLabel32 = new SRMControl.SRMLabel();
            this.lbl_OrientScore = new SRMControl.SRMLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer_MarkOrient = new System.Windows.Forms.Timer(this.components);
            this.ils_ImageListTree = new System.Windows.Forms.ImageList(this.components);
            this.btn_Previous = new SRMControl.SRMButton();
            this.btn_Next = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.tp_Detection = new System.Windows.Forms.TabPage();
            this.pnl_LearnManual = new System.Windows.Forms.Panel();
            this.btn_Reset = new SRMControl.SRMButton();
            this.btn_Learn = new SRMControl.SRMButton();
            this.txt_SetChar = new System.Windows.Forms.TextBox();
            this.pnl_pic = new System.Windows.Forms.Panel();
            this.radio_FontDetection = new System.Windows.Forms.RadioButton();
            this.radio_LearnManual = new System.Windows.Forms.RadioButton();
            this.btn_LoadDatabase = new SRMControl.SRMButton();
            this.btn_OCRSettings = new SRMControl.SRMButton();
            this.btn_Recognise = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.tp_Segmentation = new System.Windows.Forms.TabPage();
            this.radioBtn_RepairObj = new SRMControl.SRMRadioButton();
            this.srmGroupBox1 = new SRMControl.SRMGroupBox();
            this.btn_MarkGrayValueSensitivitySetting = new SRMControl.SRMButton();
            this.txt_MinArea = new SRMControl.SRMInputBox();
            this.btn_Threshold = new SRMControl.SRMButton();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.radioBtn_CutObj = new SRMControl.SRMRadioButton();
            this.tp_MarkROI = new System.Windows.Forms.TabPage();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.srmButton2 = new SRMControl.SRMButton();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.tp_SearchROI = new System.Windows.Forms.TabPage();
            this.pnl_Template = new System.Windows.Forms.Panel();
            this.Mark2 = new System.Windows.Forms.PictureBox();
            this.Mark3 = new System.Windows.Forms.PictureBox();
            this.Mark0 = new System.Windows.Forms.PictureBox();
            this.Mark1 = new System.Windows.Forms.PictureBox();
            this.srmGroupBox5 = new SRMControl.SRMGroupBox();
            this.btn_ClockWisePrecisely = new System.Windows.Forms.Button();
            this.btn_CounterClockWisePrecisely = new System.Windows.Forms.Button();
            this.srmLabel28 = new SRMControl.SRMLabel();
            this.srmLabel29 = new SRMControl.SRMLabel();
            this.btn_ROISaveClose = new SRMControl.SRMButton();
            this.chk_DeleteAllTemplates = new SRMControl.SRMCheckBox();
            this.srmGroupBox2 = new SRMControl.SRMGroupBox();
            this.srmLabel30 = new SRMControl.SRMLabel();
            this.btn_CounterClockWise = new System.Windows.Forms.Button();
            this.srmLabel27 = new SRMControl.SRMLabel();
            this.lbl_OrientationAngle = new SRMControl.SRMLabel();
            this.btn_ClockWise = new System.Windows.Forms.Button();
            this.srmLabel24 = new SRMControl.SRMLabel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lbl_TitleStep1 = new SRMControl.SRMLabel();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.tabCtrl_OCR = new SRMControl.SRMTabControl();
            this.tp_DunCare = new System.Windows.Forms.TabPage();
            this.srmLabel23 = new SRMControl.SRMLabel();
            this.srmLabel22 = new SRMControl.SRMLabel();
            this.pnl_DontCareFreeShape = new System.Windows.Forms.Panel();
            this.srmLabel21 = new SRMControl.SRMLabel();
            this.srmLabel65 = new SRMControl.SRMLabel();
            this.btn_Undo = new SRMControl.SRMButton();
            this.pictureBox26 = new System.Windows.Forms.PictureBox();
            this.srmLabel64 = new SRMControl.SRMLabel();
            this.srmLabel63 = new SRMControl.SRMLabel();
            this.btn_DeleteDontCareROI = new SRMControl.SRMButton();
            this.btn_AddDontCareROI = new SRMControl.SRMButton();
            this.srmLabel66 = new SRMControl.SRMLabel();
            this.cbo_DontCareAreaDrawMethod = new SRMControl.SRMComboBox();
            this.lbl_SearchROI = new SRMControl.SRMLabel();
            this.lbl_StepNo = new SRMControl.SRMLabel();
            this.lbl_MarkROI = new SRMControl.SRMLabel();
            this.lbl_Segmentation = new SRMControl.SRMLabel();
            this.lbl_DnR = new SRMControl.SRMLabel();
            this.lbl_DunCare = new SRMControl.SRMLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_Topology = new System.Windows.Forms.Panel();
            this.tp_Detection.SuspendLayout();
            this.pnl_LearnManual.SuspendLayout();
            this.tp_Segmentation.SuspendLayout();
            this.srmGroupBox1.SuspendLayout();
            this.tp_MarkROI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.tp_SearchROI.SuspendLayout();
            this.pnl_Template.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Mark2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mark3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mark0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mark1)).BeginInit();
            this.srmGroupBox5.SuspendLayout();
            this.srmGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabCtrl_OCR.SuspendLayout();
            this.tp_DunCare.SuspendLayout();
            this.pnl_DontCareFreeShape.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox26)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_RemoveSubROI
            // 
            resources.ApplyResources(this.btn_RemoveSubROI, "btn_RemoveSubROI");
            this.btn_RemoveSubROI.Name = "btn_RemoveSubROI";
            this.btn_RemoveSubROI.UseVisualStyleBackColor = true;
            // 
            // btn_AddSubROI
            // 
            resources.ApplyResources(this.btn_AddSubROI, "btn_AddSubROI");
            this.btn_AddSubROI.Name = "btn_AddSubROI";
            this.btn_AddSubROI.UseVisualStyleBackColor = true;
            // 
            // cbo_SubROIList
            // 
            this.cbo_SubROIList.BackColor = System.Drawing.Color.White;
            this.cbo_SubROIList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_SubROIList.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SubROIList.FormattingEnabled = true;
            resources.ApplyResources(this.cbo_SubROIList, "cbo_SubROIList");
            this.cbo_SubROIList.Name = "cbo_SubROIList";
            this.cbo_SubROIList.NormalBackColor = System.Drawing.Color.White;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // srmLabel32
            // 
            resources.ApplyResources(this.srmLabel32, "srmLabel32");
            this.srmLabel32.Name = "srmLabel32";
            this.srmLabel32.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_OrientScore
            // 
            resources.ApplyResources(this.lbl_OrientScore, "lbl_OrientScore");
            this.lbl_OrientScore.Name = "lbl_OrientScore";
            this.lbl_OrientScore.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // timer_MarkOrient
            // 
            this.timer_MarkOrient.Enabled = true;
            this.timer_MarkOrient.Tick += new System.EventHandler(this.timer_MarkOrient_Tick);
            // 
            // ils_ImageListTree
            // 
            this.ils_ImageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ils_ImageListTree.ImageStream")));
            this.ils_ImageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.ils_ImageListTree.Images.SetKeyName(0, "32px-Crystal_Clear_action_button_cancel.png");
            this.ils_ImageListTree.Images.SetKeyName(1, "32px-Crystal_Clear_action_apply.png");
            this.ils_ImageListTree.Images.SetKeyName(2, "");
            // 
            // btn_Previous
            // 
            resources.ApplyResources(this.btn_Previous, "btn_Previous");
            this.btn_Previous.Name = "btn_Previous";
            this.btn_Previous.UseVisualStyleBackColor = true;
            this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
            // 
            // btn_Next
            // 
            resources.ApplyResources(this.btn_Next, "btn_Next");
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // tp_Detection
            // 
            this.tp_Detection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Detection.Controls.Add(this.panel1);
            this.tp_Detection.Controls.Add(this.pnl_LearnManual);
            this.tp_Detection.Controls.Add(this.radio_FontDetection);
            this.tp_Detection.Controls.Add(this.radio_LearnManual);
            this.tp_Detection.Controls.Add(this.btn_LoadDatabase);
            this.tp_Detection.Controls.Add(this.btn_OCRSettings);
            this.tp_Detection.Controls.Add(this.btn_Recognise);
            this.tp_Detection.Controls.Add(this.btn_Save);
            resources.ApplyResources(this.tp_Detection, "tp_Detection");
            this.tp_Detection.Name = "tp_Detection";
            // 
            // pnl_LearnManual
            // 
            this.pnl_LearnManual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_LearnManual.Controls.Add(this.btn_Reset);
            this.pnl_LearnManual.Controls.Add(this.btn_Learn);
            this.pnl_LearnManual.Controls.Add(this.txt_SetChar);
            this.pnl_LearnManual.Controls.Add(this.pnl_pic);
            resources.ApplyResources(this.pnl_LearnManual, "pnl_LearnManual");
            this.pnl_LearnManual.Name = "pnl_LearnManual";
            // 
            // btn_Reset
            // 
            resources.ApplyResources(this.btn_Reset, "btn_Reset");
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // btn_Learn
            // 
            resources.ApplyResources(this.btn_Learn, "btn_Learn");
            this.btn_Learn.Name = "btn_Learn";
            this.btn_Learn.UseVisualStyleBackColor = true;
            this.btn_Learn.Click += new System.EventHandler(this.btn_Learn_Click);
            // 
            // txt_SetChar
            // 
            resources.ApplyResources(this.txt_SetChar, "txt_SetChar");
            this.txt_SetChar.Name = "txt_SetChar";
            this.txt_SetChar.TextChanged += new System.EventHandler(this.txt_SetChar_TextChanged);
            this.txt_SetChar.MouseEnter += new System.EventHandler(this.txt_SetChar_MouseEnter);
            this.txt_SetChar.MouseLeave += new System.EventHandler(this.txt_SetChar_MouseLeave);
            // 
            // pnl_pic
            // 
            this.pnl_pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pnl_pic, "pnl_pic");
            this.pnl_pic.Name = "pnl_pic";
            this.pnl_pic.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_pic_Paint);
            // 
            // radio_FontDetection
            // 
            resources.ApplyResources(this.radio_FontDetection, "radio_FontDetection");
            this.radio_FontDetection.Name = "radio_FontDetection";
            this.radio_FontDetection.TabStop = true;
            this.radio_FontDetection.UseVisualStyleBackColor = true;
            this.radio_FontDetection.Click += new System.EventHandler(this.radio_FontDetection_Click);
            // 
            // radio_LearnManual
            // 
            resources.ApplyResources(this.radio_LearnManual, "radio_LearnManual");
            this.radio_LearnManual.Name = "radio_LearnManual";
            this.radio_LearnManual.TabStop = true;
            this.radio_LearnManual.UseVisualStyleBackColor = true;
            this.radio_LearnManual.Click += new System.EventHandler(this.radio_LearnManual_Click);
            // 
            // btn_LoadDatabase
            // 
            resources.ApplyResources(this.btn_LoadDatabase, "btn_LoadDatabase");
            this.btn_LoadDatabase.Name = "btn_LoadDatabase";
            this.btn_LoadDatabase.UseVisualStyleBackColor = true;
            this.btn_LoadDatabase.Click += new System.EventHandler(this.btn_LoadDatabase_Click);
            // 
            // btn_OCRSettings
            // 
            resources.ApplyResources(this.btn_OCRSettings, "btn_OCRSettings");
            this.btn_OCRSettings.Name = "btn_OCRSettings";
            this.btn_OCRSettings.UseVisualStyleBackColor = true;
            this.btn_OCRSettings.Click += new System.EventHandler(this.btn_OCRSettings_Click);
            // 
            // btn_Recognise
            // 
            resources.ApplyResources(this.btn_Recognise, "btn_Recognise");
            this.btn_Recognise.Name = "btn_Recognise";
            this.btn_Recognise.UseVisualStyleBackColor = true;
            this.btn_Recognise.Click += new System.EventHandler(this.btn_Recognise_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // tp_Segmentation
            // 
            this.tp_Segmentation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Segmentation.Controls.Add(this.radioBtn_RepairObj);
            this.tp_Segmentation.Controls.Add(this.srmGroupBox1);
            this.tp_Segmentation.Controls.Add(this.radioBtn_CutObj);
            resources.ApplyResources(this.tp_Segmentation, "tp_Segmentation");
            this.tp_Segmentation.Name = "tp_Segmentation";
            // 
            // radioBtn_RepairObj
            // 
            resources.ApplyResources(this.radioBtn_RepairObj, "radioBtn_RepairObj");
            this.radioBtn_RepairObj.Name = "radioBtn_RepairObj";
            this.radioBtn_RepairObj.UseVisualStyleBackColor = true;
            // 
            // srmGroupBox1
            // 
            this.srmGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox1.Controls.Add(this.btn_MarkGrayValueSensitivitySetting);
            this.srmGroupBox1.Controls.Add(this.txt_MinArea);
            this.srmGroupBox1.Controls.Add(this.btn_Threshold);
            this.srmGroupBox1.Controls.Add(this.srmLabel4);
            resources.ApplyResources(this.srmGroupBox1, "srmGroupBox1");
            this.srmGroupBox1.Name = "srmGroupBox1";
            this.srmGroupBox1.TabStop = false;
            // 
            // btn_MarkGrayValueSensitivitySetting
            // 
            resources.ApplyResources(this.btn_MarkGrayValueSensitivitySetting, "btn_MarkGrayValueSensitivitySetting");
            this.btn_MarkGrayValueSensitivitySetting.Name = "btn_MarkGrayValueSensitivitySetting";
            this.btn_MarkGrayValueSensitivitySetting.UseVisualStyleBackColor = true;
            this.btn_MarkGrayValueSensitivitySetting.Click += new System.EventHandler(this.btn_MarkGrayValueSensitivitySetting_Click);
            // 
            // txt_MinArea
            // 
            this.txt_MinArea.BackColor = System.Drawing.Color.White;
            this.txt_MinArea.DecimalPlaces = 0;
            this.txt_MinArea.DecMaxValue = new decimal(new int[] {
            99999999,
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
            this.txt_MinArea.TextChanged += new System.EventHandler(this.txt_MinArea_TextChanged);
            // 
            // btn_Threshold
            // 
            resources.ApplyResources(this.btn_Threshold, "btn_Threshold");
            this.btn_Threshold.Name = "btn_Threshold";
            this.btn_Threshold.UseVisualStyleBackColor = true;
            this.btn_Threshold.Click += new System.EventHandler(this.btn_Threshold_Click);
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // radioBtn_CutObj
            // 
            this.radioBtn_CutObj.Checked = true;
            resources.ApplyResources(this.radioBtn_CutObj, "radioBtn_CutObj");
            this.radioBtn_CutObj.Name = "radioBtn_CutObj";
            this.radioBtn_CutObj.TabStop = true;
            this.radioBtn_CutObj.UseVisualStyleBackColor = true;
            // 
            // tp_MarkROI
            // 
            this.tp_MarkROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_MarkROI.Controls.Add(this.pictureBox5);
            this.tp_MarkROI.Controls.Add(this.srmButton2);
            this.tp_MarkROI.Controls.Add(this.srmLabel8);
            resources.ApplyResources(this.tp_MarkROI, "tp_MarkROI");
            this.tp_MarkROI.Name = "tp_MarkROI";
            // 
            // pictureBox5
            // 
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // srmButton2
            // 
            resources.ApplyResources(this.srmButton2, "srmButton2");
            this.srmButton2.Name = "srmButton2";
            this.srmButton2.UseVisualStyleBackColor = true;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_SearchROI
            // 
            this.tp_SearchROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_SearchROI.Controls.Add(this.pnl_Template);
            this.tp_SearchROI.Controls.Add(this.srmGroupBox5);
            this.tp_SearchROI.Controls.Add(this.btn_ROISaveClose);
            this.tp_SearchROI.Controls.Add(this.chk_DeleteAllTemplates);
            this.tp_SearchROI.Controls.Add(this.srmGroupBox2);
            this.tp_SearchROI.Controls.Add(this.pictureBox3);
            this.tp_SearchROI.Controls.Add(this.pictureBox2);
            this.tp_SearchROI.Controls.Add(this.lbl_TitleStep1);
            this.tp_SearchROI.Controls.Add(this.srmLabel9);
            resources.ApplyResources(this.tp_SearchROI, "tp_SearchROI");
            this.tp_SearchROI.Name = "tp_SearchROI";
            // 
            // pnl_Template
            // 
            this.pnl_Template.BackColor = System.Drawing.Color.Black;
            this.pnl_Template.Controls.Add(this.Mark2);
            this.pnl_Template.Controls.Add(this.Mark3);
            this.pnl_Template.Controls.Add(this.Mark0);
            this.pnl_Template.Controls.Add(this.Mark1);
            resources.ApplyResources(this.pnl_Template, "pnl_Template");
            this.pnl_Template.Name = "pnl_Template";
            // 
            // Mark2
            // 
            this.Mark2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.Mark2, "Mark2");
            this.Mark2.Name = "Mark2";
            this.Mark2.TabStop = false;
            this.Mark2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Mark2_MouseClick);
            // 
            // Mark3
            // 
            this.Mark3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.Mark3, "Mark3");
            this.Mark3.Name = "Mark3";
            this.Mark3.TabStop = false;
            this.Mark3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Mark3_MouseClick);
            // 
            // Mark0
            // 
            this.Mark0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.Mark0, "Mark0");
            this.Mark0.Name = "Mark0";
            this.Mark0.TabStop = false;
            this.Mark0.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Mark0_MouseClick);
            // 
            // Mark1
            // 
            this.Mark1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.Mark1, "Mark1");
            this.Mark1.Name = "Mark1";
            this.Mark1.TabStop = false;
            this.Mark1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Mark1_MouseClick);
            // 
            // srmGroupBox5
            // 
            this.srmGroupBox5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox5.Controls.Add(this.btn_ClockWisePrecisely);
            this.srmGroupBox5.Controls.Add(this.btn_CounterClockWisePrecisely);
            this.srmGroupBox5.Controls.Add(this.srmLabel28);
            this.srmGroupBox5.Controls.Add(this.srmLabel29);
            resources.ApplyResources(this.srmGroupBox5, "srmGroupBox5");
            this.srmGroupBox5.Name = "srmGroupBox5";
            this.srmGroupBox5.TabStop = false;
            // 
            // btn_ClockWisePrecisely
            // 
            resources.ApplyResources(this.btn_ClockWisePrecisely, "btn_ClockWisePrecisely");
            this.btn_ClockWisePrecisely.Name = "btn_ClockWisePrecisely";
            this.btn_ClockWisePrecisely.UseVisualStyleBackColor = true;
            this.btn_ClockWisePrecisely.Click += new System.EventHandler(this.btn_ClockWisePrecisely_Click);
            // 
            // btn_CounterClockWisePrecisely
            // 
            resources.ApplyResources(this.btn_CounterClockWisePrecisely, "btn_CounterClockWisePrecisely");
            this.btn_CounterClockWisePrecisely.Name = "btn_CounterClockWisePrecisely";
            this.btn_CounterClockWisePrecisely.UseVisualStyleBackColor = true;
            this.btn_CounterClockWisePrecisely.Click += new System.EventHandler(this.btn_CounterClockWisePrecisely_Click);
            // 
            // srmLabel28
            // 
            resources.ApplyResources(this.srmLabel28, "srmLabel28");
            this.srmLabel28.Name = "srmLabel28";
            this.srmLabel28.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel29
            // 
            resources.ApplyResources(this.srmLabel29, "srmLabel29");
            this.srmLabel29.Name = "srmLabel29";
            this.srmLabel29.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_ROISaveClose
            // 
            resources.ApplyResources(this.btn_ROISaveClose, "btn_ROISaveClose");
            this.btn_ROISaveClose.Name = "btn_ROISaveClose";
            this.btn_ROISaveClose.UseVisualStyleBackColor = true;
            this.btn_ROISaveClose.Click += new System.EventHandler(this.btn_ROISaveClose_Click);
            // 
            // chk_DeleteAllTemplates
            // 
            this.chk_DeleteAllTemplates.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_DeleteAllTemplates, "chk_DeleteAllTemplates");
            this.chk_DeleteAllTemplates.Name = "chk_DeleteAllTemplates";
            this.chk_DeleteAllTemplates.Selected = true;
            this.chk_DeleteAllTemplates.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_DeleteAllTemplates.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_DeleteAllTemplates.UseVisualStyleBackColor = true;
            this.chk_DeleteAllTemplates.CheckedChanged += new System.EventHandler(this.chk_DeleteAllTemplates_CheckedChanged);
            // 
            // srmGroupBox2
            // 
            this.srmGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox2.Controls.Add(this.srmLabel30);
            this.srmGroupBox2.Controls.Add(this.btn_CounterClockWise);
            this.srmGroupBox2.Controls.Add(this.srmLabel27);
            this.srmGroupBox2.Controls.Add(this.lbl_OrientationAngle);
            this.srmGroupBox2.Controls.Add(this.btn_ClockWise);
            this.srmGroupBox2.Controls.Add(this.srmLabel24);
            resources.ApplyResources(this.srmGroupBox2, "srmGroupBox2");
            this.srmGroupBox2.Name = "srmGroupBox2";
            this.srmGroupBox2.TabStop = false;
            // 
            // srmLabel30
            // 
            this.srmLabel30.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.srmLabel30, "srmLabel30");
            this.srmLabel30.ForeColor = System.Drawing.Color.Yellow;
            this.srmLabel30.Name = "srmLabel30";
            this.srmLabel30.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_CounterClockWise
            // 
            resources.ApplyResources(this.btn_CounterClockWise, "btn_CounterClockWise");
            this.btn_CounterClockWise.Name = "btn_CounterClockWise";
            this.btn_CounterClockWise.UseVisualStyleBackColor = true;
            this.btn_CounterClockWise.Click += new System.EventHandler(this.btn_RotateUnit_Click);
            // 
            // srmLabel27
            // 
            resources.ApplyResources(this.srmLabel27, "srmLabel27");
            this.srmLabel27.Name = "srmLabel27";
            this.srmLabel27.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_OrientationAngle
            // 
            this.lbl_OrientationAngle.BackColor = System.Drawing.Color.Black;
            this.lbl_OrientationAngle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lbl_OrientationAngle, "lbl_OrientationAngle");
            this.lbl_OrientationAngle.ForeColor = System.Drawing.Color.Yellow;
            this.lbl_OrientationAngle.Name = "lbl_OrientationAngle";
            this.lbl_OrientationAngle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_ClockWise
            // 
            resources.ApplyResources(this.btn_ClockWise, "btn_ClockWise");
            this.btn_ClockWise.Name = "btn_ClockWise";
            this.btn_ClockWise.UseVisualStyleBackColor = true;
            this.btn_ClockWise.Click += new System.EventHandler(this.btn_RotateUnit_Click);
            // 
            // srmLabel24
            // 
            resources.ApplyResources(this.srmLabel24, "srmLabel24");
            this.srmLabel24.Name = "srmLabel24";
            this.srmLabel24.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pictureBox3
            // 
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // lbl_TitleStep1
            // 
            resources.ApplyResources(this.lbl_TitleStep1, "lbl_TitleStep1");
            this.lbl_TitleStep1.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStep1.Name = "lbl_TitleStep1";
            this.lbl_TitleStep1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tabCtrl_OCR
            // 
            this.tabCtrl_OCR.Controls.Add(this.tp_SearchROI);
            this.tabCtrl_OCR.Controls.Add(this.tp_MarkROI);
            this.tabCtrl_OCR.Controls.Add(this.tp_Segmentation);
            this.tabCtrl_OCR.Controls.Add(this.tp_Detection);
            this.tabCtrl_OCR.Controls.Add(this.tp_DunCare);
            resources.ApplyResources(this.tabCtrl_OCR, "tabCtrl_OCR");
            this.tabCtrl_OCR.Name = "tabCtrl_OCR";
            this.tabCtrl_OCR.SelectedIndex = 0;
            this.tabCtrl_OCR.TabStop = false;
            // 
            // tp_DunCare
            // 
            this.tp_DunCare.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_DunCare.Controls.Add(this.srmLabel23);
            this.tp_DunCare.Controls.Add(this.srmLabel22);
            this.tp_DunCare.Controls.Add(this.pnl_DontCareFreeShape);
            this.tp_DunCare.Controls.Add(this.btn_DeleteDontCareROI);
            this.tp_DunCare.Controls.Add(this.btn_AddDontCareROI);
            this.tp_DunCare.Controls.Add(this.srmLabel66);
            this.tp_DunCare.Controls.Add(this.cbo_DontCareAreaDrawMethod);
            resources.ApplyResources(this.tp_DunCare, "tp_DunCare");
            this.tp_DunCare.Name = "tp_DunCare";
            // 
            // srmLabel23
            // 
            resources.ApplyResources(this.srmLabel23, "srmLabel23");
            this.srmLabel23.Name = "srmLabel23";
            this.srmLabel23.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel22
            // 
            resources.ApplyResources(this.srmLabel22, "srmLabel22");
            this.srmLabel22.Name = "srmLabel22";
            this.srmLabel22.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_DontCareFreeShape
            // 
            this.pnl_DontCareFreeShape.Controls.Add(this.srmLabel21);
            this.pnl_DontCareFreeShape.Controls.Add(this.srmLabel65);
            this.pnl_DontCareFreeShape.Controls.Add(this.btn_Undo);
            this.pnl_DontCareFreeShape.Controls.Add(this.pictureBox26);
            this.pnl_DontCareFreeShape.Controls.Add(this.srmLabel64);
            this.pnl_DontCareFreeShape.Controls.Add(this.srmLabel63);
            resources.ApplyResources(this.pnl_DontCareFreeShape, "pnl_DontCareFreeShape");
            this.pnl_DontCareFreeShape.Name = "pnl_DontCareFreeShape";
            // 
            // srmLabel21
            // 
            resources.ApplyResources(this.srmLabel21, "srmLabel21");
            this.srmLabel21.Name = "srmLabel21";
            this.srmLabel21.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel65
            // 
            resources.ApplyResources(this.srmLabel65, "srmLabel65");
            this.srmLabel65.Name = "srmLabel65";
            this.srmLabel65.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Undo
            // 
            resources.ApplyResources(this.btn_Undo, "btn_Undo");
            this.btn_Undo.Name = "btn_Undo";
            this.btn_Undo.UseVisualStyleBackColor = true;
            // 
            // pictureBox26
            // 
            resources.ApplyResources(this.pictureBox26, "pictureBox26");
            this.pictureBox26.Name = "pictureBox26";
            this.pictureBox26.TabStop = false;
            // 
            // srmLabel64
            // 
            resources.ApplyResources(this.srmLabel64, "srmLabel64");
            this.srmLabel64.Name = "srmLabel64";
            this.srmLabel64.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel63
            // 
            resources.ApplyResources(this.srmLabel63, "srmLabel63");
            this.srmLabel63.Name = "srmLabel63";
            this.srmLabel63.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_DeleteDontCareROI
            // 
            resources.ApplyResources(this.btn_DeleteDontCareROI, "btn_DeleteDontCareROI");
            this.btn_DeleteDontCareROI.Name = "btn_DeleteDontCareROI";
            this.btn_DeleteDontCareROI.UseVisualStyleBackColor = true;
            // 
            // btn_AddDontCareROI
            // 
            resources.ApplyResources(this.btn_AddDontCareROI, "btn_AddDontCareROI");
            this.btn_AddDontCareROI.Name = "btn_AddDontCareROI";
            this.btn_AddDontCareROI.UseVisualStyleBackColor = true;
            // 
            // srmLabel66
            // 
            resources.ApplyResources(this.srmLabel66, "srmLabel66");
            this.srmLabel66.Name = "srmLabel66";
            this.srmLabel66.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_DontCareAreaDrawMethod
            // 
            this.cbo_DontCareAreaDrawMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
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
            // 
            // lbl_SearchROI
            // 
            resources.ApplyResources(this.lbl_SearchROI, "lbl_SearchROI");
            this.lbl_SearchROI.ForeColor = System.Drawing.Color.Black;
            this.lbl_SearchROI.Name = "lbl_SearchROI";
            this.lbl_SearchROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_StepNo
            // 
            resources.ApplyResources(this.lbl_StepNo, "lbl_StepNo");
            this.lbl_StepNo.ForeColor = System.Drawing.Color.Black;
            this.lbl_StepNo.Name = "lbl_StepNo";
            this.lbl_StepNo.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_MarkROI
            // 
            resources.ApplyResources(this.lbl_MarkROI, "lbl_MarkROI");
            this.lbl_MarkROI.ForeColor = System.Drawing.Color.Black;
            this.lbl_MarkROI.Name = "lbl_MarkROI";
            this.lbl_MarkROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Segmentation
            // 
            resources.ApplyResources(this.lbl_Segmentation, "lbl_Segmentation");
            this.lbl_Segmentation.ForeColor = System.Drawing.Color.Black;
            this.lbl_Segmentation.Name = "lbl_Segmentation";
            this.lbl_Segmentation.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_DnR
            // 
            resources.ApplyResources(this.lbl_DnR, "lbl_DnR");
            this.lbl_DnR.ForeColor = System.Drawing.Color.Black;
            this.lbl_DnR.Name = "lbl_DnR";
            this.lbl_DnR.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_DunCare
            // 
            resources.ApplyResources(this.lbl_DunCare, "lbl_DunCare");
            this.lbl_DunCare.ForeColor = System.Drawing.Color.Black;
            this.lbl_DunCare.Name = "lbl_DunCare";
            this.lbl_DunCare.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.pnl_Topology);
            this.panel1.Name = "panel1";
            // 
            // pnl_Topology
            // 
            resources.ApplyResources(this.pnl_Topology, "pnl_Topology");
            this.pnl_Topology.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pnl_Topology.Name = "pnl_Topology";
            this.pnl_Topology.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_Topology_Paint);
            // 
            // LearnOCRForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Previous);
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.tabCtrl_OCR);
            this.Controls.Add(this.lbl_DunCare);
            this.Controls.Add(this.lbl_DnR);
            this.Controls.Add(this.lbl_Segmentation);
            this.Controls.Add(this.lbl_MarkROI);
            this.Controls.Add(this.lbl_SearchROI);
            this.Controls.Add(this.lbl_StepNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LearnOCRForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LearnOCRForm_FormClosing);
            this.Load += new System.EventHandler(this.LearnOCRForm_Load);
            this.tp_Detection.ResumeLayout(false);
            this.tp_Detection.PerformLayout();
            this.pnl_LearnManual.ResumeLayout(false);
            this.pnl_LearnManual.PerformLayout();
            this.tp_Segmentation.ResumeLayout(false);
            this.srmGroupBox1.ResumeLayout(false);
            this.srmGroupBox1.PerformLayout();
            this.tp_MarkROI.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.tp_SearchROI.ResumeLayout(false);
            this.pnl_Template.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Mark2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mark3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mark0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mark1)).EndInit();
            this.srmGroupBox5.ResumeLayout(false);
            this.srmGroupBox5.PerformLayout();
            this.srmGroupBox2.ResumeLayout(false);
            this.srmGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabCtrl_OCR.ResumeLayout(false);
            this.tp_DunCare.ResumeLayout(false);
            this.pnl_DontCareFreeShape.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox26)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_RemoveSubROI;
        private System.Windows.Forms.Button btn_AddSubROI;
        private SRMControl.SRMComboBox cbo_SubROIList;
        private System.Windows.Forms.Label label6;
        private SRMControl.SRMLabel srmLabel32;
        private SRMControl.SRMLabel lbl_OrientScore;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timer_MarkOrient;
        private System.Windows.Forms.ImageList ils_ImageListTree;
        private SRMControl.SRMButton btn_Previous;
        private SRMControl.SRMButton btn_Next;
        private SRMControl.SRMButton btn_Cancel;
        private System.Windows.Forms.TabPage tp_Detection;
        private System.Windows.Forms.RadioButton radio_FontDetection;
        private System.Windows.Forms.RadioButton radio_LearnManual;
        private SRMControl.SRMButton btn_LoadDatabase;
        private SRMControl.SRMButton btn_OCRSettings;
        private SRMControl.SRMButton btn_Recognise;
        private SRMControl.SRMButton btn_Save;
        private System.Windows.Forms.TabPage tp_Segmentation;
        private SRMControl.SRMRadioButton radioBtn_RepairObj;
        private SRMControl.SRMGroupBox srmGroupBox1;
        private SRMControl.SRMButton btn_MarkGrayValueSensitivitySetting;
        private SRMControl.SRMInputBox txt_MinArea;
        private SRMControl.SRMButton btn_Threshold;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMRadioButton radioBtn_CutObj;
        private System.Windows.Forms.TabPage tp_MarkROI;
        private System.Windows.Forms.PictureBox pictureBox5;
        private SRMControl.SRMButton srmButton2;
        private SRMControl.SRMLabel srmLabel8;
        private System.Windows.Forms.TabPage tp_SearchROI;
        private System.Windows.Forms.Panel pnl_Template;
        private System.Windows.Forms.PictureBox Mark2;
        private System.Windows.Forms.PictureBox Mark3;
        private System.Windows.Forms.PictureBox Mark0;
        private System.Windows.Forms.PictureBox Mark1;
        private SRMControl.SRMGroupBox srmGroupBox5;
        private System.Windows.Forms.Button btn_ClockWisePrecisely;
        private System.Windows.Forms.Button btn_CounterClockWisePrecisely;
        private SRMControl.SRMLabel srmLabel28;
        private SRMControl.SRMLabel srmLabel29;
        private SRMControl.SRMButton btn_ROISaveClose;
        private SRMControl.SRMCheckBox chk_DeleteAllTemplates;
        private SRMControl.SRMGroupBox srmGroupBox2;
        private SRMControl.SRMLabel srmLabel30;
        private System.Windows.Forms.Button btn_CounterClockWise;
        private SRMControl.SRMLabel srmLabel27;
        private SRMControl.SRMLabel lbl_OrientationAngle;
        private System.Windows.Forms.Button btn_ClockWise;
        private SRMControl.SRMLabel srmLabel24;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private SRMControl.SRMLabel lbl_TitleStep1;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMTabControl tabCtrl_OCR;
        private SRMControl.SRMLabel lbl_SearchROI;
        private SRMControl.SRMLabel lbl_StepNo;
        private SRMControl.SRMLabel lbl_MarkROI;
        private SRMControl.SRMLabel lbl_Segmentation;
        private SRMControl.SRMLabel lbl_DnR;
        private SRMControl.SRMLabel lbl_DunCare;
        private System.Windows.Forms.Panel pnl_LearnManual;
        private System.Windows.Forms.Panel pnl_pic;
        private System.Windows.Forms.TextBox txt_SetChar;
        private SRMControl.SRMButton btn_Learn;
        private SRMControl.SRMButton btn_Reset;
        private System.Windows.Forms.TabPage tp_DunCare;
        private SRMControl.SRMLabel srmLabel23;
        private SRMControl.SRMLabel srmLabel22;
        private System.Windows.Forms.Panel pnl_DontCareFreeShape;
        private SRMControl.SRMLabel srmLabel21;
        private SRMControl.SRMLabel srmLabel65;
        private SRMControl.SRMButton btn_Undo;
        private System.Windows.Forms.PictureBox pictureBox26;
        private SRMControl.SRMLabel srmLabel64;
        private SRMControl.SRMLabel srmLabel63;
        private SRMControl.SRMButton btn_DeleteDontCareROI;
        private SRMControl.SRMButton btn_AddDontCareROI;
        private SRMControl.SRMLabel srmLabel66;
        private SRMControl.SRMComboBox cbo_DontCareAreaDrawMethod;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnl_Topology;
    }
}