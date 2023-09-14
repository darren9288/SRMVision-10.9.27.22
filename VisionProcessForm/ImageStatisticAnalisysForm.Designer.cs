namespace VisionProcessForm
{
    partial class ImageStatisticAnalisysForm
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
            this.btn_SetAsReferenceData = new SRMControl.SRMButton();
            this.btn_StartAnalysis = new SRMControl.SRMButton();
            this.chk_WantSaveToTrackLog = new SRMControl.SRMCheckBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.lbl_GrayMin1 = new SRMControl.SRMLabel();
            this.lbl_GrayMax1 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.lbl_Average1 = new SRMControl.SRMLabel();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.lbl_Variance1 = new SRMControl.SRMLabel();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.lbl_StdDev1 = new SRMControl.SRMLabel();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.lbl_StdDev2 = new SRMControl.SRMLabel();
            this.lbl_Variance2 = new SRMControl.SRMLabel();
            this.lbl_Average2 = new SRMControl.SRMLabel();
            this.lbl_GrayMax2 = new SRMControl.SRMLabel();
            this.lbl_GrayMin2 = new SRMControl.SRMLabel();
            this.srmLabel18 = new SRMControl.SRMLabel();
            this.lbl_StdDev3 = new SRMControl.SRMLabel();
            this.lbl_Variance3 = new SRMControl.SRMLabel();
            this.lbl_Average3 = new SRMControl.SRMLabel();
            this.lbl_GrayMax3 = new SRMControl.SRMLabel();
            this.lbl_GrayMin3 = new SRMControl.SRMLabel();
            this.srmLabel24 = new SRMControl.SRMLabel();
            this.lbl_TotalData = new SRMControl.SRMLabel();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btn_SetAsReferenceData
            // 
            this.btn_SetAsReferenceData.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.btn_SetAsReferenceData.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_SetAsReferenceData.Location = new System.Drawing.Point(13, 13);
            this.btn_SetAsReferenceData.Margin = new System.Windows.Forms.Padding(4);
            this.btn_SetAsReferenceData.Name = "btn_SetAsReferenceData";
            this.btn_SetAsReferenceData.Size = new System.Drawing.Size(197, 37);
            this.btn_SetAsReferenceData.TabIndex = 78;
            this.btn_SetAsReferenceData.Text = "Set Image As Reference Data";
            this.btn_SetAsReferenceData.UseVisualStyleBackColor = true;
            this.btn_SetAsReferenceData.Click += new System.EventHandler(this.btn_SetAsReferenceData_Click);
            // 
            // btn_StartAnalysis
            // 
            this.btn_StartAnalysis.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.btn_StartAnalysis.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_StartAnalysis.Location = new System.Drawing.Point(13, 58);
            this.btn_StartAnalysis.Margin = new System.Windows.Forms.Padding(4);
            this.btn_StartAnalysis.Name = "btn_StartAnalysis";
            this.btn_StartAnalysis.Size = new System.Drawing.Size(197, 35);
            this.btn_StartAnalysis.TabIndex = 79;
            this.btn_StartAnalysis.Text = "Start Analysis";
            this.btn_StartAnalysis.UseVisualStyleBackColor = true;
            this.btn_StartAnalysis.Click += new System.EventHandler(this.btn_StartAnalysis_Click);
            // 
            // chk_WantSaveToTrackLog
            // 
            this.chk_WantSaveToTrackLog.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WantSaveToTrackLog.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_WantSaveToTrackLog.Location = new System.Drawing.Point(214, 63);
            this.chk_WantSaveToTrackLog.Name = "chk_WantSaveToTrackLog";
            this.chk_WantSaveToTrackLog.Selected = true;
            this.chk_WantSaveToTrackLog.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantSaveToTrackLog.Size = new System.Drawing.Size(178, 27);
            this.chk_WantSaveToTrackLog.TabIndex = 134;
            this.chk_WantSaveToTrackLog.Text = "Save Statistic To Log File";
            this.chk_WantSaveToTrackLog.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantSaveToTrackLog.UseVisualStyleBackColor = true;
            this.chk_WantSaveToTrackLog.Click += new System.EventHandler(this.chk_WantSaveToTrackLog_Click);
            // 
            // srmLabel1
            // 
            this.srmLabel1.AutoSize = true;
            this.srmLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel1.Location = new System.Drawing.Point(13, 165);
            this.srmLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.Size = new System.Drawing.Size(93, 13);
            this.srmLabel1.TabIndex = 135;
            this.srmLabel1.Text = "Gray Minimum (px)";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_GrayMin1
            // 
            this.lbl_GrayMin1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrayMin1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_GrayMin1.Location = new System.Drawing.Point(127, 161);
            this.lbl_GrayMin1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_GrayMin1.Name = "lbl_GrayMin1";
            this.lbl_GrayMin1.Size = new System.Drawing.Size(64, 20);
            this.lbl_GrayMin1.TabIndex = 136;
            this.lbl_GrayMin1.Text = "0";
            this.lbl_GrayMin1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_GrayMin1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_GrayMax1
            // 
            this.lbl_GrayMax1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrayMax1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_GrayMax1.Location = new System.Drawing.Point(127, 190);
            this.lbl_GrayMax1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_GrayMax1.Name = "lbl_GrayMax1";
            this.lbl_GrayMax1.Size = new System.Drawing.Size(64, 20);
            this.lbl_GrayMax1.TabIndex = 138;
            this.lbl_GrayMax1.Text = "0";
            this.lbl_GrayMax1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_GrayMax1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            this.srmLabel4.AutoSize = true;
            this.srmLabel4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel4.Location = new System.Drawing.Point(13, 194);
            this.srmLabel4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.Size = new System.Drawing.Size(96, 13);
            this.srmLabel4.TabIndex = 137;
            this.srmLabel4.Text = "Gray Maximum (px)";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Average1
            // 
            this.lbl_Average1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Average1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Average1.Location = new System.Drawing.Point(127, 219);
            this.lbl_Average1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Average1.Name = "lbl_Average1";
            this.lbl_Average1.Size = new System.Drawing.Size(64, 20);
            this.lbl_Average1.TabIndex = 140;
            this.lbl_Average1.Text = "0";
            this.lbl_Average1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Average1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel6
            // 
            this.srmLabel6.AutoSize = true;
            this.srmLabel6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel6.Location = new System.Drawing.Point(13, 223);
            this.srmLabel6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.Size = new System.Drawing.Size(47, 13);
            this.srmLabel6.TabIndex = 139;
            this.srmLabel6.Text = "Average";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Variance1
            // 
            this.lbl_Variance1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Variance1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Variance1.Location = new System.Drawing.Point(127, 249);
            this.lbl_Variance1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Variance1.Name = "lbl_Variance1";
            this.lbl_Variance1.Size = new System.Drawing.Size(64, 20);
            this.lbl_Variance1.TabIndex = 142;
            this.lbl_Variance1.Text = "0";
            this.lbl_Variance1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Variance1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel8
            // 
            this.srmLabel8.AutoSize = true;
            this.srmLabel8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel8.Location = new System.Drawing.Point(13, 253);
            this.srmLabel8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.Size = new System.Drawing.Size(49, 13);
            this.srmLabel8.TabIndex = 141;
            this.srmLabel8.Text = "Variance";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_StdDev1
            // 
            this.lbl_StdDev1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_StdDev1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_StdDev1.Location = new System.Drawing.Point(127, 278);
            this.lbl_StdDev1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_StdDev1.Name = "lbl_StdDev1";
            this.lbl_StdDev1.Size = new System.Drawing.Size(64, 20);
            this.lbl_StdDev1.TabIndex = 144;
            this.lbl_StdDev1.Text = "0";
            this.lbl_StdDev1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_StdDev1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel10
            // 
            this.srmLabel10.AutoSize = true;
            this.srmLabel10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel10.Location = new System.Drawing.Point(13, 282);
            this.srmLabel10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.Size = new System.Drawing.Size(74, 13);
            this.srmLabel10.TabIndex = 143;
            this.srmLabel10.Text = "Std. Deviation";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel11
            // 
            this.srmLabel11.AutoSize = true;
            this.srmLabel11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel11.Location = new System.Drawing.Point(137, 130);
            this.srmLabel11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.Size = new System.Drawing.Size(45, 13);
            this.srmLabel11.TabIndex = 145;
            this.srmLabel11.Text = "Image 1";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel12
            // 
            this.srmLabel12.AutoSize = true;
            this.srmLabel12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel12.Location = new System.Drawing.Point(211, 130);
            this.srmLabel12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.Size = new System.Drawing.Size(45, 13);
            this.srmLabel12.TabIndex = 151;
            this.srmLabel12.Text = "Image 2";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_StdDev2
            // 
            this.lbl_StdDev2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_StdDev2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_StdDev2.Location = new System.Drawing.Point(201, 278);
            this.lbl_StdDev2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_StdDev2.Name = "lbl_StdDev2";
            this.lbl_StdDev2.Size = new System.Drawing.Size(64, 20);
            this.lbl_StdDev2.TabIndex = 150;
            this.lbl_StdDev2.Text = "0";
            this.lbl_StdDev2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_StdDev2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Variance2
            // 
            this.lbl_Variance2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Variance2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Variance2.Location = new System.Drawing.Point(201, 249);
            this.lbl_Variance2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Variance2.Name = "lbl_Variance2";
            this.lbl_Variance2.Size = new System.Drawing.Size(64, 20);
            this.lbl_Variance2.TabIndex = 149;
            this.lbl_Variance2.Text = "0";
            this.lbl_Variance2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Variance2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Average2
            // 
            this.lbl_Average2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Average2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Average2.Location = new System.Drawing.Point(201, 219);
            this.lbl_Average2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Average2.Name = "lbl_Average2";
            this.lbl_Average2.Size = new System.Drawing.Size(64, 20);
            this.lbl_Average2.TabIndex = 148;
            this.lbl_Average2.Text = "0";
            this.lbl_Average2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Average2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_GrayMax2
            // 
            this.lbl_GrayMax2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrayMax2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_GrayMax2.Location = new System.Drawing.Point(201, 190);
            this.lbl_GrayMax2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_GrayMax2.Name = "lbl_GrayMax2";
            this.lbl_GrayMax2.Size = new System.Drawing.Size(64, 20);
            this.lbl_GrayMax2.TabIndex = 147;
            this.lbl_GrayMax2.Text = "0";
            this.lbl_GrayMax2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_GrayMax2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_GrayMin2
            // 
            this.lbl_GrayMin2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrayMin2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_GrayMin2.Location = new System.Drawing.Point(201, 161);
            this.lbl_GrayMin2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_GrayMin2.Name = "lbl_GrayMin2";
            this.lbl_GrayMin2.Size = new System.Drawing.Size(64, 20);
            this.lbl_GrayMin2.TabIndex = 146;
            this.lbl_GrayMin2.Text = "0";
            this.lbl_GrayMin2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_GrayMin2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel18
            // 
            this.srmLabel18.AutoSize = true;
            this.srmLabel18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel18.Location = new System.Drawing.Point(285, 130);
            this.srmLabel18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel18.Name = "srmLabel18";
            this.srmLabel18.Size = new System.Drawing.Size(45, 13);
            this.srmLabel18.TabIndex = 157;
            this.srmLabel18.Text = "Image 3";
            this.srmLabel18.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_StdDev3
            // 
            this.lbl_StdDev3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_StdDev3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_StdDev3.Location = new System.Drawing.Point(275, 278);
            this.lbl_StdDev3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_StdDev3.Name = "lbl_StdDev3";
            this.lbl_StdDev3.Size = new System.Drawing.Size(64, 20);
            this.lbl_StdDev3.TabIndex = 156;
            this.lbl_StdDev3.Text = "0";
            this.lbl_StdDev3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_StdDev3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Variance3
            // 
            this.lbl_Variance3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Variance3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Variance3.Location = new System.Drawing.Point(275, 249);
            this.lbl_Variance3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Variance3.Name = "lbl_Variance3";
            this.lbl_Variance3.Size = new System.Drawing.Size(64, 20);
            this.lbl_Variance3.TabIndex = 155;
            this.lbl_Variance3.Text = "0";
            this.lbl_Variance3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Variance3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Average3
            // 
            this.lbl_Average3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Average3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Average3.Location = new System.Drawing.Point(275, 219);
            this.lbl_Average3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_Average3.Name = "lbl_Average3";
            this.lbl_Average3.Size = new System.Drawing.Size(64, 20);
            this.lbl_Average3.TabIndex = 154;
            this.lbl_Average3.Text = "0";
            this.lbl_Average3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Average3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_GrayMax3
            // 
            this.lbl_GrayMax3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrayMax3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_GrayMax3.Location = new System.Drawing.Point(275, 190);
            this.lbl_GrayMax3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_GrayMax3.Name = "lbl_GrayMax3";
            this.lbl_GrayMax3.Size = new System.Drawing.Size(64, 20);
            this.lbl_GrayMax3.TabIndex = 153;
            this.lbl_GrayMax3.Text = "0";
            this.lbl_GrayMax3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_GrayMax3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_GrayMin3
            // 
            this.lbl_GrayMin3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrayMin3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_GrayMin3.Location = new System.Drawing.Point(275, 161);
            this.lbl_GrayMin3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_GrayMin3.Name = "lbl_GrayMin3";
            this.lbl_GrayMin3.Size = new System.Drawing.Size(64, 20);
            this.lbl_GrayMin3.TabIndex = 152;
            this.lbl_GrayMin3.Text = "0";
            this.lbl_GrayMin3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_GrayMin3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel24
            // 
            this.srmLabel24.AutoSize = true;
            this.srmLabel24.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel24.Location = new System.Drawing.Point(13, 97);
            this.srmLabel24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.srmLabel24.Name = "srmLabel24";
            this.srmLabel24.Size = new System.Drawing.Size(66, 13);
            this.srmLabel24.TabIndex = 158;
            this.srmLabel24.Text = "Total Data : ";
            this.srmLabel24.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TotalData
            // 
            this.lbl_TotalData.AutoSize = true;
            this.lbl_TotalData.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TotalData.Location = new System.Drawing.Point(87, 97);
            this.lbl_TotalData.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_TotalData.Name = "lbl_TotalData";
            this.lbl_TotalData.Size = new System.Drawing.Size(13, 13);
            this.lbl_TotalData.TabIndex = 159;
            this.lbl_TotalData.Text = "0";
            this.lbl_TotalData.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            this.Timer.Interval = 1;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // ImageStatisticAnalisysForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(386, 314);
            this.Controls.Add(this.lbl_TotalData);
            this.Controls.Add(this.srmLabel24);
            this.Controls.Add(this.srmLabel18);
            this.Controls.Add(this.lbl_StdDev3);
            this.Controls.Add(this.lbl_Variance3);
            this.Controls.Add(this.lbl_Average3);
            this.Controls.Add(this.lbl_GrayMax3);
            this.Controls.Add(this.lbl_GrayMin3);
            this.Controls.Add(this.srmLabel12);
            this.Controls.Add(this.lbl_StdDev2);
            this.Controls.Add(this.lbl_Variance2);
            this.Controls.Add(this.lbl_Average2);
            this.Controls.Add(this.lbl_GrayMax2);
            this.Controls.Add(this.lbl_GrayMin2);
            this.Controls.Add(this.srmLabel11);
            this.Controls.Add(this.lbl_StdDev1);
            this.Controls.Add(this.srmLabel10);
            this.Controls.Add(this.lbl_Variance1);
            this.Controls.Add(this.srmLabel8);
            this.Controls.Add(this.lbl_Average1);
            this.Controls.Add(this.srmLabel6);
            this.Controls.Add(this.lbl_GrayMax1);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.lbl_GrayMin1);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.chk_WantSaveToTrackLog);
            this.Controls.Add(this.btn_StartAnalysis);
            this.Controls.Add(this.btn_SetAsReferenceData);
            this.Name = "ImageStatisticAnalisysForm";
            this.Text = "Image Statistic Analisys Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageStatisticAnalisysForm_FormClosing);
            this.Load += new System.EventHandler(this.ImageStatisticAnalisysForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_SetAsReferenceData;
        private SRMControl.SRMButton btn_StartAnalysis;
        private SRMControl.SRMCheckBox chk_WantSaveToTrackLog;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel lbl_GrayMin1;
        private SRMControl.SRMLabel lbl_GrayMax1;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel lbl_Average1;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMLabel lbl_Variance1;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMLabel lbl_StdDev1;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMLabel lbl_StdDev2;
        private SRMControl.SRMLabel lbl_Variance2;
        private SRMControl.SRMLabel lbl_Average2;
        private SRMControl.SRMLabel lbl_GrayMax2;
        private SRMControl.SRMLabel lbl_GrayMin2;
        private SRMControl.SRMLabel srmLabel18;
        private SRMControl.SRMLabel lbl_StdDev3;
        private SRMControl.SRMLabel lbl_Variance3;
        private SRMControl.SRMLabel lbl_Average3;
        private SRMControl.SRMLabel lbl_GrayMax3;
        private SRMControl.SRMLabel lbl_GrayMin3;
        private SRMControl.SRMLabel srmLabel24;
        private SRMControl.SRMLabel lbl_TotalData;
        private System.Windows.Forms.Timer Timer;
    }
}