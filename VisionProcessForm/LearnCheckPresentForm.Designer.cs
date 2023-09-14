namespace VisionProcessForm
{
    partial class LearnCheckPresentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearnCheckPresentForm));
            this.btn_Next = new SRMControl.SRMButton();
            this.lbl_StepNo = new SRMControl.SRMLabel();
            this.tabCtrl_Setup = new SRMControl.SRMTabControl();
            this.tp_SearchROI = new System.Windows.Forms.TabPage();
            this.btn_AdvancedSettings = new SRMControl.SRMButton();
            this.cbo_UnitType = new SRMControl.SRMComboBox();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.tp_UnitROI = new System.Windows.Forms.TabPage();
            this.chk_AdjustBasedOnCornerROI = new SRMControl.SRMCheckBox();
            this.chk_FollowFirstROISize = new SRMControl.SRMCheckBox();
            this.txt_TotalUnitROIs = new SRMControl.SRMLabel();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.txt_UnitROICountX = new SRMControl.SRMInputBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.txt_UnitROICountY = new SRMControl.SRMInputBox();
            this.btn_ThresholdUnitROI = new SRMControl.SRMButton();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.tp_SegmentObj = new System.Windows.Forms.TabPage();
            this.lbl_TotalObjects = new SRMControl.SRMLabel();
            this.srmLabel17 = new SRMControl.SRMLabel();
            this.srmGroupBox8 = new SRMControl.SRMGroupBox();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.cbo_ClassSelection = new SRMControl.SRMComboBox();
            this.srmLabel15 = new SRMControl.SRMLabel();
            this.btn_UndoObjects = new SRMControl.SRMButton();
            this.srmLabel16 = new SRMControl.SRMLabel();
            this.txt_MaxArea = new SRMControl.SRMInputBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.btn_Threshold = new SRMControl.SRMButton();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.txt_MinArea = new SRMControl.SRMInputBox();
            this.tp_Finish = new System.Windows.Forms.TabPage();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.lbl_TitleStep4 = new SRMControl.SRMLabel();
            this.lbl_TitleStep5 = new SRMControl.SRMLabel();
            this.lbl_TitleStep2 = new SRMControl.SRMLabel();
            this.lbl_TitleStep1 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.btn_Previous = new SRMControl.SRMButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbl_TitleStep3 = new SRMControl.SRMLabel();
            this.lbl_TitleStep6 = new SRMControl.SRMLabel();
            this.lbl_TitleStepPocket = new SRMControl.SRMLabel();
            this.lbl_TitleStepDontCareArea = new SRMControl.SRMLabel();
            this.lbl_TitleStepOrientROI = new SRMControl.SRMLabel();
            this.tabCtrl_Setup.SuspendLayout();
            this.tp_SearchROI.SuspendLayout();
            this.tp_UnitROI.SuspendLayout();
            this.tp_SegmentObj.SuspendLayout();
            this.srmGroupBox8.SuspendLayout();
            this.tp_Finish.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Next
            // 
            this.btn_Next.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Next.Location = new System.Drawing.Point(71, 531);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Size = new System.Drawing.Size(61, 30);
            this.btn_Next.TabIndex = 161;
            this.btn_Next.Text = ">>";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // lbl_StepNo
            // 
            this.lbl_StepNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lbl_StepNo.ForeColor = System.Drawing.Color.Black;
            this.lbl_StepNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_StepNo.Location = new System.Drawing.Point(0, 1);
            this.lbl_StepNo.Name = "lbl_StepNo";
            this.lbl_StepNo.Size = new System.Drawing.Size(75, 24);
            this.lbl_StepNo.TabIndex = 165;
            this.lbl_StepNo.Text = "Step 1:";
            this.lbl_StepNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_StepNo.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tabCtrl_Setup
            // 
            this.tabCtrl_Setup.Controls.Add(this.tp_SearchROI);
            this.tabCtrl_Setup.Controls.Add(this.tp_UnitROI);
            this.tabCtrl_Setup.Controls.Add(this.tp_SegmentObj);
            this.tabCtrl_Setup.Controls.Add(this.tp_Finish);
            this.tabCtrl_Setup.Location = new System.Drawing.Point(-4, 3);
            this.tabCtrl_Setup.Name = "tabCtrl_Setup";
            this.tabCtrl_Setup.SelectedIndex = 0;
            this.tabCtrl_Setup.Size = new System.Drawing.Size(258, 525);
            this.tabCtrl_Setup.TabIndex = 159;
            this.tabCtrl_Setup.TabStop = false;
            // 
            // tp_SearchROI
            // 
            this.tp_SearchROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_SearchROI.Controls.Add(this.btn_AdvancedSettings);
            this.tp_SearchROI.Controls.Add(this.cbo_UnitType);
            this.tp_SearchROI.Controls.Add(this.srmLabel7);
            this.tp_SearchROI.Location = new System.Drawing.Point(4, 22);
            this.tp_SearchROI.Name = "tp_SearchROI";
            this.tp_SearchROI.Size = new System.Drawing.Size(250, 499);
            this.tp_SearchROI.TabIndex = 1;
            this.tp_SearchROI.Text = "SearchROI";
            // 
            // btn_AdvancedSettings
            // 
            this.btn_AdvancedSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_AdvancedSettings.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_AdvancedSettings.Location = new System.Drawing.Point(15, 446);
            this.btn_AdvancedSettings.Name = "btn_AdvancedSettings";
            this.btn_AdvancedSettings.Size = new System.Drawing.Size(110, 35);
            this.btn_AdvancedSettings.TabIndex = 104;
            this.btn_AdvancedSettings.Text = "Advance Setting";
            this.btn_AdvancedSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_AdvancedSettings.UseVisualStyleBackColor = true;
            this.btn_AdvancedSettings.Visible = false;
            this.btn_AdvancedSettings.Click += new System.EventHandler(this.btn_AdvancedSettings_Click);
            // 
            // cbo_UnitType
            // 
            this.cbo_UnitType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_UnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_UnitType.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_UnitType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbo_UnitType.FormattingEnabled = true;
            this.cbo_UnitType.ItemHeight = 20;
            this.cbo_UnitType.Items.AddRange(new object[] {
            "Unit",
            "EmptyUnit"});
            this.cbo_UnitType.Location = new System.Drawing.Point(15, 62);
            this.cbo_UnitType.Name = "cbo_UnitType";
            this.cbo_UnitType.NormalBackColor = System.Drawing.Color.White;
            this.cbo_UnitType.Size = new System.Drawing.Size(208, 26);
            this.cbo_UnitType.TabIndex = 96;
            this.cbo_UnitType.Visible = false;
            // 
            // srmLabel7
            // 
            this.srmLabel7.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.srmLabel7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel7.Location = new System.Drawing.Point(12, 35);
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.Size = new System.Drawing.Size(211, 24);
            this.srmLabel7.TabIndex = 95;
            this.srmLabel7.Text = "Learn Type";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            this.srmLabel7.Visible = false;
            // 
            // tp_UnitROI
            // 
            this.tp_UnitROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_UnitROI.Controls.Add(this.chk_AdjustBasedOnCornerROI);
            this.tp_UnitROI.Controls.Add(this.chk_FollowFirstROISize);
            this.tp_UnitROI.Controls.Add(this.txt_TotalUnitROIs);
            this.tp_UnitROI.Controls.Add(this.srmLabel11);
            this.tp_UnitROI.Controls.Add(this.srmLabel10);
            this.tp_UnitROI.Controls.Add(this.txt_UnitROICountX);
            this.tp_UnitROI.Controls.Add(this.srmLabel8);
            this.tp_UnitROI.Controls.Add(this.txt_UnitROICountY);
            this.tp_UnitROI.Controls.Add(this.btn_ThresholdUnitROI);
            this.tp_UnitROI.Controls.Add(this.srmLabel6);
            this.tp_UnitROI.Location = new System.Drawing.Point(4, 22);
            this.tp_UnitROI.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tp_UnitROI.Name = "tp_UnitROI";
            this.tp_UnitROI.Size = new System.Drawing.Size(250, 499);
            this.tp_UnitROI.TabIndex = 3;
            this.tp_UnitROI.Text = "UnitROI";
            this.tp_UnitROI.Click += new System.EventHandler(this.tp_UnitROI_Click);
            // 
            // chk_AdjustBasedOnCornerROI
            // 
            this.chk_AdjustBasedOnCornerROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AdjustBasedOnCornerROI.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_AdjustBasedOnCornerROI.Location = new System.Drawing.Point(23, 302);
            this.chk_AdjustBasedOnCornerROI.Name = "chk_AdjustBasedOnCornerROI";
            this.chk_AdjustBasedOnCornerROI.Selected = true;
            this.chk_AdjustBasedOnCornerROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AdjustBasedOnCornerROI.Size = new System.Drawing.Size(169, 24);
            this.chk_AdjustBasedOnCornerROI.TabIndex = 196;
            this.chk_AdjustBasedOnCornerROI.Text = "Adjust based on corner ROIs";
            this.chk_AdjustBasedOnCornerROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AdjustBasedOnCornerROI.UseVisualStyleBackColor = true;
            this.chk_AdjustBasedOnCornerROI.Click += new System.EventHandler(this.chk_AdjustBasedOnCornerROI_Click);
            // 
            // chk_FollowFirstROISize
            // 
            this.chk_FollowFirstROISize.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_FollowFirstROISize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_FollowFirstROISize.Location = new System.Drawing.Point(23, 271);
            this.chk_FollowFirstROISize.Name = "chk_FollowFirstROISize";
            this.chk_FollowFirstROISize.Selected = true;
            this.chk_FollowFirstROISize.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_FollowFirstROISize.Size = new System.Drawing.Size(169, 24);
            this.chk_FollowFirstROISize.TabIndex = 195;
            this.chk_FollowFirstROISize.Text = "Follow first ROI size";
            this.chk_FollowFirstROISize.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_FollowFirstROISize.UseVisualStyleBackColor = true;
            this.chk_FollowFirstROISize.Click += new System.EventHandler(this.chk_FollowFirstROISize_Click);
            // 
            // txt_TotalUnitROIs
            // 
            this.txt_TotalUnitROIs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_TotalUnitROIs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_TotalUnitROIs.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txt_TotalUnitROIs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txt_TotalUnitROIs.Location = new System.Drawing.Point(67, 135);
            this.txt_TotalUnitROIs.Name = "txt_TotalUnitROIs";
            this.txt_TotalUnitROIs.Size = new System.Drawing.Size(82, 26);
            this.txt_TotalUnitROIs.TabIndex = 190;
            this.txt_TotalUnitROIs.Text = " ";
            this.txt_TotalUnitROIs.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel11
            // 
            this.srmLabel11.AutoSize = true;
            this.srmLabel11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel11.Location = new System.Drawing.Point(20, 137);
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.Size = new System.Drawing.Size(52, 20);
            this.srmLabel11.TabIndex = 194;
            this.srmLabel11.Text = "Total :";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel10
            // 
            this.srmLabel10.AutoSize = true;
            this.srmLabel10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel10.Location = new System.Drawing.Point(138, 98);
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.Size = new System.Drawing.Size(28, 20);
            this.srmLabel10.TabIndex = 193;
            this.srmLabel10.Text = "Y :";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_UnitROICountX
            // 
            this.txt_UnitROICountX.BackColor = System.Drawing.Color.White;
            this.txt_UnitROICountX.DecimalPlaces = 0;
            this.txt_UnitROICountX.DecMaxValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.txt_UnitROICountX.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_UnitROICountX.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_UnitROICountX.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_UnitROICountX.ForeColor = System.Drawing.Color.Black;
            this.txt_UnitROICountX.InputType = SRMControl.InputType.Number;
            this.txt_UnitROICountX.Location = new System.Drawing.Point(56, 96);
            this.txt_UnitROICountX.Name = "txt_UnitROICountX";
            this.txt_UnitROICountX.NormalBackColor = System.Drawing.Color.White;
            this.txt_UnitROICountX.Size = new System.Drawing.Size(50, 26);
            this.txt_UnitROICountX.TabIndex = 188;
            this.txt_UnitROICountX.Text = "0";
            this.txt_UnitROICountX.TextChanged += new System.EventHandler(this.txt_UnitROICount_TextChanged);
            // 
            // srmLabel8
            // 
            this.srmLabel8.AutoSize = true;
            this.srmLabel8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel8.Location = new System.Drawing.Point(29, 98);
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.Size = new System.Drawing.Size(28, 20);
            this.srmLabel8.TabIndex = 192;
            this.srmLabel8.Text = "X :";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_UnitROICountY
            // 
            this.txt_UnitROICountY.BackColor = System.Drawing.Color.White;
            this.txt_UnitROICountY.DecimalPlaces = 0;
            this.txt_UnitROICountY.DecMaxValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.txt_UnitROICountY.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_UnitROICountY.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_UnitROICountY.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_UnitROICountY.ForeColor = System.Drawing.Color.Black;
            this.txt_UnitROICountY.InputType = SRMControl.InputType.Number;
            this.txt_UnitROICountY.Location = new System.Drawing.Point(165, 96);
            this.txt_UnitROICountY.Name = "txt_UnitROICountY";
            this.txt_UnitROICountY.NormalBackColor = System.Drawing.Color.White;
            this.txt_UnitROICountY.Size = new System.Drawing.Size(50, 26);
            this.txt_UnitROICountY.TabIndex = 191;
            this.txt_UnitROICountY.Text = "0";
            this.txt_UnitROICountY.TextChanged += new System.EventHandler(this.txt_UnitROICount_TextChanged);
            // 
            // btn_ThresholdUnitROI
            // 
            this.btn_ThresholdUnitROI.Image = ((System.Drawing.Image)(resources.GetObject("btn_ThresholdUnitROI.Image")));
            this.btn_ThresholdUnitROI.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_ThresholdUnitROI.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_ThresholdUnitROI.Location = new System.Drawing.Point(23, 195);
            this.btn_ThresholdUnitROI.Name = "btn_ThresholdUnitROI";
            this.btn_ThresholdUnitROI.Size = new System.Drawing.Size(110, 35);
            this.btn_ThresholdUnitROI.TabIndex = 189;
            this.btn_ThresholdUnitROI.Text = "Threshold";
            this.btn_ThresholdUnitROI.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_ThresholdUnitROI.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_ThresholdUnitROI.UseVisualStyleBackColor = true;
            this.btn_ThresholdUnitROI.Click += new System.EventHandler(this.btn_ThresholdUnitROI_Click);
            // 
            // srmLabel6
            // 
            this.srmLabel6.AutoSize = true;
            this.srmLabel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel6.Location = new System.Drawing.Point(20, 62);
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.Size = new System.Drawing.Size(177, 20);
            this.srmLabel6.TabIndex = 187;
            this.srmLabel6.Text = "Enter Unit ROIs Count :";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_SegmentObj
            // 
            this.tp_SegmentObj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_SegmentObj.Controls.Add(this.lbl_TotalObjects);
            this.tp_SegmentObj.Controls.Add(this.srmLabel17);
            this.tp_SegmentObj.Controls.Add(this.srmGroupBox8);
            this.tp_SegmentObj.Location = new System.Drawing.Point(4, 22);
            this.tp_SegmentObj.Name = "tp_SegmentObj";
            this.tp_SegmentObj.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tp_SegmentObj.Size = new System.Drawing.Size(250, 499);
            this.tp_SegmentObj.TabIndex = 0;
            this.tp_SegmentObj.Text = "SegObj";
            // 
            // lbl_TotalObjects
            // 
            this.lbl_TotalObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_TotalObjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_TotalObjects.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_TotalObjects.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TotalObjects.Location = new System.Drawing.Point(23, 332);
            this.lbl_TotalObjects.Name = "lbl_TotalObjects";
            this.lbl_TotalObjects.Size = new System.Drawing.Size(82, 26);
            this.lbl_TotalObjects.TabIndex = 186;
            this.lbl_TotalObjects.Text = "0";
            this.lbl_TotalObjects.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel17
            // 
            this.srmLabel17.AutoSize = true;
            this.srmLabel17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel17.Location = new System.Drawing.Point(20, 303);
            this.srmLabel17.Name = "srmLabel17";
            this.srmLabel17.Size = new System.Drawing.Size(115, 13);
            this.srmLabel17.TabIndex = 185;
            this.srmLabel17.Text = "Total Objects Selected";
            this.srmLabel17.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmGroupBox8
            // 
            this.srmGroupBox8.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox8.Controls.Add(this.srmLabel4);
            this.srmGroupBox8.Controls.Add(this.cbo_ClassSelection);
            this.srmGroupBox8.Controls.Add(this.srmLabel15);
            this.srmGroupBox8.Controls.Add(this.btn_UndoObjects);
            this.srmGroupBox8.Controls.Add(this.srmLabel16);
            this.srmGroupBox8.Controls.Add(this.txt_MaxArea);
            this.srmGroupBox8.Controls.Add(this.srmLabel2);
            this.srmGroupBox8.Controls.Add(this.btn_Threshold);
            this.srmGroupBox8.Controls.Add(this.srmLabel9);
            this.srmGroupBox8.Controls.Add(this.txt_MinArea);
            this.srmGroupBox8.Location = new System.Drawing.Point(10, 17);
            this.srmGroupBox8.Name = "srmGroupBox8";
            this.srmGroupBox8.Size = new System.Drawing.Size(226, 283);
            this.srmGroupBox8.TabIndex = 184;
            this.srmGroupBox8.TabStop = false;
            this.srmGroupBox8.Text = "Filter";
            // 
            // srmLabel4
            // 
            this.srmLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel4.Location = new System.Drawing.Point(22, 216);
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.Size = new System.Drawing.Size(198, 30);
            this.srmLabel4.TabIndex = 186;
            this.srmLabel4.Text = "Press Undo and manually select objects on image using mouse.";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_ClassSelection
            // 
            this.cbo_ClassSelection.BackColor = System.Drawing.Color.White;
            this.cbo_ClassSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ClassSelection.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ClassSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbo_ClassSelection.FormattingEnabled = true;
            this.cbo_ClassSelection.ItemHeight = 20;
            this.cbo_ClassSelection.Items.AddRange(new object[] {
            "WhiteOnBlack",
            "BlackOnWhite"});
            this.cbo_ClassSelection.Location = new System.Drawing.Point(13, 19);
            this.cbo_ClassSelection.Name = "cbo_ClassSelection";
            this.cbo_ClassSelection.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ClassSelection.Size = new System.Drawing.Size(155, 26);
            this.cbo_ClassSelection.TabIndex = 137;
            this.cbo_ClassSelection.SelectedIndexChanged += new System.EventHandler(this.cbo_ClassSelection_SelectedIndexChanged);
            // 
            // srmLabel15
            // 
            this.srmLabel15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel15.Location = new System.Drawing.Point(172, 82);
            this.srmLabel15.Name = "srmLabel15";
            this.srmLabel15.Size = new System.Drawing.Size(36, 18);
            this.srmLabel15.TabIndex = 136;
            this.srmLabel15.Text = "pixel";
            this.srmLabel15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.srmLabel15.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_UndoObjects
            // 
            this.btn_UndoObjects.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_UndoObjects.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_UndoObjects.Location = new System.Drawing.Point(25, 178);
            this.btn_UndoObjects.Name = "btn_UndoObjects";
            this.btn_UndoObjects.Size = new System.Drawing.Size(110, 35);
            this.btn_UndoObjects.TabIndex = 67;
            this.btn_UndoObjects.Text = "Undo Objects";
            this.btn_UndoObjects.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_UndoObjects.UseVisualStyleBackColor = true;
            this.btn_UndoObjects.Click += new System.EventHandler(this.btn_UndoObjects_Click);
            // 
            // srmLabel16
            // 
            this.srmLabel16.AutoSize = true;
            this.srmLabel16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel16.Location = new System.Drawing.Point(10, 85);
            this.srmLabel16.Name = "srmLabel16";
            this.srmLabel16.Size = new System.Drawing.Size(52, 13);
            this.srmLabel16.TabIndex = 134;
            this.srmLabel16.Text = "Max Area";
            this.srmLabel16.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MaxArea
            // 
            this.txt_MaxArea.BackColor = System.Drawing.Color.White;
            this.txt_MaxArea.DecimalPlaces = 0;
            this.txt_MaxArea.DecMaxValue = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txt_MaxArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MaxArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MaxArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MaxArea.InputType = SRMControl.InputType.Number;
            this.txt_MaxArea.Location = new System.Drawing.Point(102, 82);
            this.txt_MaxArea.Name = "txt_MaxArea";
            this.txt_MaxArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_MaxArea.Size = new System.Drawing.Size(65, 20);
            this.txt_MaxArea.TabIndex = 135;
            this.txt_MaxArea.Text = "100";
            this.txt_MaxArea.TextChanged += new System.EventHandler(this.txt_MaxArea_TextChanged);
            // 
            // srmLabel2
            // 
            this.srmLabel2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel2.Location = new System.Drawing.Point(173, 58);
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.Size = new System.Drawing.Size(36, 18);
            this.srmLabel2.TabIndex = 133;
            this.srmLabel2.Text = "pixel";
            this.srmLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Threshold
            // 
            this.btn_Threshold.Image = ((System.Drawing.Image)(resources.GetObject("btn_Threshold.Image")));
            this.btn_Threshold.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Threshold.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Threshold.Location = new System.Drawing.Point(25, 125);
            this.btn_Threshold.Name = "btn_Threshold";
            this.btn_Threshold.Size = new System.Drawing.Size(110, 35);
            this.btn_Threshold.TabIndex = 65;
            this.btn_Threshold.Text = "Threshold";
            this.btn_Threshold.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_Threshold.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Threshold.UseVisualStyleBackColor = true;
            this.btn_Threshold.Click += new System.EventHandler(this.btn_Threshold_Click);
            // 
            // srmLabel9
            // 
            this.srmLabel9.AutoSize = true;
            this.srmLabel9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel9.Location = new System.Drawing.Point(11, 62);
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.Size = new System.Drawing.Size(49, 13);
            this.srmLabel9.TabIndex = 50;
            this.srmLabel9.Text = "Min Area";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinArea
            // 
            this.txt_MinArea.BackColor = System.Drawing.Color.White;
            this.txt_MinArea.DecimalPlaces = 0;
            this.txt_MinArea.DecMaxValue = new decimal(new int[] {
            999999999,
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
            this.txt_MinArea.Location = new System.Drawing.Point(103, 58);
            this.txt_MinArea.Name = "txt_MinArea";
            this.txt_MinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_MinArea.Size = new System.Drawing.Size(65, 20);
            this.txt_MinArea.TabIndex = 65;
            this.txt_MinArea.Text = "10";
            this.txt_MinArea.TextChanged += new System.EventHandler(this.txt_MinArea_TextChanged);
            // 
            // tp_Finish
            // 
            this.tp_Finish.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Finish.Controls.Add(this.srmLabel3);
            this.tp_Finish.Controls.Add(this.btn_Save);
            this.tp_Finish.Location = new System.Drawing.Point(4, 22);
            this.tp_Finish.Name = "tp_Finish";
            this.tp_Finish.Size = new System.Drawing.Size(250, 499);
            this.tp_Finish.TabIndex = 2;
            this.tp_Finish.Text = "Finish";
            // 
            // srmLabel3
            // 
            this.srmLabel3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold);
            this.srmLabel3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel3.Location = new System.Drawing.Point(96, 197);
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.Size = new System.Drawing.Size(65, 30);
            this.srmLabel3.TabIndex = 121;
            this.srmLabel3.Text = "Finish";
            this.srmLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Save
            // 
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(81, 246);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(90, 57);
            this.btn_Save.TabIndex = 120;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Cancel.Location = new System.Drawing.Point(164, 529);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 160;
            this.btn_Cancel.Text = "Close";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // lbl_TitleStep4
            // 
            this.lbl_TitleStep4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStep4.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStep4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStep4.Location = new System.Drawing.Point(73, 1);
            this.lbl_TitleStep4.Name = "lbl_TitleStep4";
            this.lbl_TitleStep4.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStep4.TabIndex = 166;
            this.lbl_TitleStep4.Text = "Unit ROI";
            this.lbl_TitleStep4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStep4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStep5
            // 
            this.lbl_TitleStep5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStep5.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStep5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStep5.Location = new System.Drawing.Point(75, 1);
            this.lbl_TitleStep5.Name = "lbl_TitleStep5";
            this.lbl_TitleStep5.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStep5.TabIndex = 169;
            this.lbl_TitleStep5.Text = "Learn Pattern";
            this.lbl_TitleStep5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStep5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStep2
            // 
            this.lbl_TitleStep2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStep2.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStep2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStep2.Location = new System.Drawing.Point(75, 1);
            this.lbl_TitleStep2.Name = "lbl_TitleStep2";
            this.lbl_TitleStep2.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStep2.TabIndex = 168;
            this.lbl_TitleStep2.Text = "Define Objects";
            this.lbl_TitleStep2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStep2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStep1
            // 
            this.lbl_TitleStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStep1.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStep1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStep1.Location = new System.Drawing.Point(75, 1);
            this.lbl_TitleStep1.Name = "lbl_TitleStep1";
            this.lbl_TitleStep1.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStep1.TabIndex = 167;
            this.lbl_TitleStep1.Text = "Define Search ROI";
            this.lbl_TitleStep1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStep1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            this.srmLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.srmLabel1.ForeColor = System.Drawing.Color.Black;
            this.srmLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel1.Location = new System.Drawing.Point(0, 3);
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.Size = new System.Drawing.Size(75, 24);
            this.srmLabel1.TabIndex = 172;
            this.srmLabel1.Text = "Step 1:";
            this.srmLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Previous
            // 
            this.btn_Previous.Enabled = false;
            this.btn_Previous.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Previous.Location = new System.Drawing.Point(8, 531);
            this.btn_Previous.Name = "btn_Previous";
            this.btn_Previous.Size = new System.Drawing.Size(61, 30);
            this.btn_Previous.TabIndex = 162;
            this.btn_Previous.Text = "<<";
            this.btn_Previous.UseVisualStyleBackColor = true;
            this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbl_TitleStep3
            // 
            this.lbl_TitleStep3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStep3.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStep3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStep3.Location = new System.Drawing.Point(73, 1);
            this.lbl_TitleStep3.Name = "lbl_TitleStep3";
            this.lbl_TitleStep3.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStep3.TabIndex = 173;
            this.lbl_TitleStep3.Text = "Confirm Location";
            this.lbl_TitleStep3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStep3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStep6
            // 
            this.lbl_TitleStep6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStep6.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStep6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStep6.Location = new System.Drawing.Point(75, 1);
            this.lbl_TitleStep6.Name = "lbl_TitleStep6";
            this.lbl_TitleStep6.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStep6.TabIndex = 174;
            this.lbl_TitleStep6.Text = "Learn Empty";
            this.lbl_TitleStep6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStep6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStepPocket
            // 
            this.lbl_TitleStepPocket.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStepPocket.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStepPocket.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStepPocket.Location = new System.Drawing.Point(75, 1);
            this.lbl_TitleStepPocket.Name = "lbl_TitleStepPocket";
            this.lbl_TitleStepPocket.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStepPocket.TabIndex = 179;
            this.lbl_TitleStepPocket.Text = "Learn Pocket";
            this.lbl_TitleStepPocket.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStepPocket.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStepDontCareArea
            // 
            this.lbl_TitleStepDontCareArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStepDontCareArea.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStepDontCareArea.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStepDontCareArea.Location = new System.Drawing.Point(75, 1);
            this.lbl_TitleStepDontCareArea.Name = "lbl_TitleStepDontCareArea";
            this.lbl_TitleStepDontCareArea.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStepDontCareArea.TabIndex = 180;
            this.lbl_TitleStepDontCareArea.Text = "Dont Care Area";
            this.lbl_TitleStepDontCareArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStepDontCareArea.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStepOrientROI
            // 
            this.lbl_TitleStepOrientROI.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lbl_TitleStepOrientROI.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStepOrientROI.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TitleStepOrientROI.Location = new System.Drawing.Point(75, 1);
            this.lbl_TitleStepOrientROI.Name = "lbl_TitleStepOrientROI";
            this.lbl_TitleStepOrientROI.Size = new System.Drawing.Size(177, 24);
            this.lbl_TitleStepOrientROI.TabIndex = 181;
            this.lbl_TitleStepOrientROI.Text = "Orient ROI";
            this.lbl_TitleStepOrientROI.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TitleStepOrientROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // LearnCheckPresentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(444, 565);
            this.Controls.Add(this.tabCtrl_Setup);
            this.Controls.Add(this.lbl_TitleStepOrientROI);
            this.Controls.Add(this.lbl_TitleStepDontCareArea);
            this.Controls.Add(this.lbl_TitleStepPocket);
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Previous);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.lbl_StepNo);
            this.Controls.Add(this.lbl_TitleStep5);
            this.Controls.Add(this.lbl_TitleStep2);
            this.Controls.Add(this.lbl_TitleStep1);
            this.Controls.Add(this.lbl_TitleStep3);
            this.Controls.Add(this.lbl_TitleStep4);
            this.Controls.Add(this.lbl_TitleStep6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LearnCheckPresentForm";
            this.Text = "LearnCheckPresentForm";
            this.Load += new System.EventHandler(this.LearnCheckPresentForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LearnCheckPresentForm_FormClosing);
            this.tabCtrl_Setup.ResumeLayout(false);
            this.tp_SearchROI.ResumeLayout(false);
            this.tp_UnitROI.ResumeLayout(false);
            this.tp_UnitROI.PerformLayout();
            this.tp_SegmentObj.ResumeLayout(false);
            this.tp_SegmentObj.PerformLayout();
            this.srmGroupBox8.ResumeLayout(false);
            this.srmGroupBox8.PerformLayout();
            this.tp_Finish.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Next;
        private SRMControl.SRMLabel lbl_StepNo;
        private SRMControl.SRMTabControl tabCtrl_Setup;
        private System.Windows.Forms.TabPage tp_SegmentObj;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMLabel lbl_TitleStep4;
        private SRMControl.SRMLabel lbl_TitleStep5;
        private SRMControl.SRMLabel lbl_TitleStep2;
        private SRMControl.SRMLabel lbl_TitleStep1;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMButton btn_Previous;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tp_SearchROI;
        private System.Windows.Forms.TabPage tp_Finish;
        private SRMControl.SRMLabel lbl_TitleStep3;
        private SRMControl.SRMComboBox cbo_UnitType;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMLabel lbl_TitleStep6;
        private SRMControl.SRMLabel lbl_TitleStepPocket;
        private SRMControl.SRMLabel lbl_TitleStepDontCareArea;
        private SRMControl.SRMLabel lbl_TitleStepOrientROI;
        private SRMControl.SRMButton btn_AdvancedSettings;
        private SRMControl.SRMLabel lbl_TotalObjects;
        private SRMControl.SRMLabel srmLabel17;
        private SRMControl.SRMGroupBox srmGroupBox8;
        private SRMControl.SRMLabel srmLabel15;
        private SRMControl.SRMButton btn_UndoObjects;
        private SRMControl.SRMLabel srmLabel16;
        private SRMControl.SRMInputBox txt_MaxArea;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMButton btn_Threshold;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMInputBox txt_MinArea;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMComboBox cbo_ClassSelection;
        private SRMControl.SRMLabel srmLabel4;
        private System.Windows.Forms.TabPage tp_UnitROI;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMButton btn_ThresholdUnitROI;
        private SRMControl.SRMInputBox txt_UnitROICountX;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMInputBox txt_UnitROICountY;
        private SRMControl.SRMLabel txt_TotalUnitROIs;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMCheckBox chk_FollowFirstROISize;
        private SRMControl.SRMCheckBox chk_AdjustBasedOnCornerROI;
    }
}