namespace VisionProcessForm
{
    partial class AdvancedLGaugeForm
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
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.txt_MeasFilter = new SRMControl.SRMInputBox();
            this.txt_MeasThickness = new SRMControl.SRMInputBox();
            this.txt_MeasMinArea = new SRMControl.SRMInputBox();
            this.txt_MeasMinAmp = new SRMControl.SRMInputBox();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.cbo_TransChoice = new SRMControl.SRMComboBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.cbo_TransType = new SRMControl.SRMComboBox();
            this.txt_FilteringPass = new SRMControl.SRMInputBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_threshold = new SRMControl.SRMInputBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.txt_FilteringThreshold = new SRMControl.SRMInputBox();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(105, 268);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 16;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(9, 268);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 34);
            this.btn_OK.TabIndex = 15;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // txt_MeasFilter
            // 
            this.txt_MeasFilter.BackColor = System.Drawing.Color.White;
            this.txt_MeasFilter.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasFilter.DecimalPlaces = 0;
            this.txt_MeasFilter.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasFilter.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MeasFilter.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MeasFilter.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasFilter.InputType = SRMControl.InputType.Number;
            this.txt_MeasFilter.Location = new System.Drawing.Point(116, 101);
            this.txt_MeasFilter.Name = "txt_MeasFilter";
            this.txt_MeasFilter.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasFilter.Size = new System.Drawing.Size(77, 20);
            this.txt_MeasFilter.TabIndex = 106;
            this.txt_MeasFilter.Text = "1";
            this.txt_MeasFilter.TextChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
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
            this.txt_MeasThickness.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasThickness.InputType = SRMControl.InputType.Number;
            this.txt_MeasThickness.Location = new System.Drawing.Point(12, 218);
            this.txt_MeasThickness.Name = "txt_MeasThickness";
            this.txt_MeasThickness.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasThickness.Size = new System.Drawing.Size(77, 20);
            this.txt_MeasThickness.TabIndex = 105;
            this.txt_MeasThickness.Text = "1";
            this.txt_MeasThickness.TextChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
            // 
            // txt_MeasMinArea
            // 
            this.txt_MeasMinArea.BackColor = System.Drawing.Color.White;
            this.txt_MeasMinArea.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasMinArea.DecimalPlaces = 0;
            this.txt_MeasMinArea.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MeasMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MeasMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasMinArea.InputType = SRMControl.InputType.Number;
            this.txt_MeasMinArea.Location = new System.Drawing.Point(12, 137);
            this.txt_MeasMinArea.Name = "txt_MeasMinArea";
            this.txt_MeasMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasMinArea.Size = new System.Drawing.Size(77, 20);
            this.txt_MeasMinArea.TabIndex = 104;
            this.txt_MeasMinArea.Text = "1";
            this.txt_MeasMinArea.TextChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
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
            this.txt_MeasMinAmp.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasMinAmp.InputType = SRMControl.InputType.Number;
            this.txt_MeasMinAmp.Location = new System.Drawing.Point(12, 101);
            this.txt_MeasMinAmp.Name = "txt_MeasMinAmp";
            this.txt_MeasMinAmp.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasMinAmp.Size = new System.Drawing.Size(77, 20);
            this.txt_MeasMinAmp.TabIndex = 103;
            this.txt_MeasMinAmp.Text = "1";
            this.txt_MeasMinAmp.TextChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
            // 
            // srmLabel13
            // 
            this.srmLabel13.AutoSize = true;
            this.srmLabel13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel13.Location = new System.Drawing.Point(115, 86);
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.Size = new System.Drawing.Size(32, 13);
            this.srmLabel13.TabIndex = 101;
            this.srmLabel13.Text = "Filter:";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel12
            // 
            this.srmLabel12.AutoSize = true;
            this.srmLabel12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel12.Location = new System.Drawing.Point(12, 202);
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.Size = new System.Drawing.Size(59, 13);
            this.srmLabel12.TabIndex = 100;
            this.srmLabel12.Text = "Thickness:";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel11
            // 
            this.srmLabel11.AutoSize = true;
            this.srmLabel11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel11.Location = new System.Drawing.Point(12, 124);
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.Size = new System.Drawing.Size(55, 13);
            this.srmLabel11.TabIndex = 99;
            this.srmLabel11.Text = "Min. Area:";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel10
            // 
            this.srmLabel10.AutoSize = true;
            this.srmLabel10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel10.Location = new System.Drawing.Point(12, 86);
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.Size = new System.Drawing.Size(51, 13);
            this.srmLabel10.TabIndex = 98;
            this.srmLabel10.Text = "Min.Amp:";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel8
            // 
            this.srmLabel8.AutoSize = true;
            this.srmLabel8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel8.Location = new System.Drawing.Point(12, 47);
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.Size = new System.Drawing.Size(92, 13);
            this.srmLabel8.TabIndex = 2;
            this.srmLabel8.Text = "Transition Choice:";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_TransChoice
            // 
            this.cbo_TransChoice.BackColor = System.Drawing.Color.White;
            this.cbo_TransChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_TransChoice.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_TransChoice.FormattingEnabled = true;
            this.cbo_TransChoice.Items.AddRange(new object[] {
            "From Begin",
            "From End",
            "Largest Amplitud",
            "Largest Area",
            "Closest"});
            this.cbo_TransChoice.Location = new System.Drawing.Point(12, 61);
            this.cbo_TransChoice.Name = "cbo_TransChoice";
            this.cbo_TransChoice.NormalBackColor = System.Drawing.Color.White;
            this.cbo_TransChoice.Size = new System.Drawing.Size(174, 21);
            this.cbo_TransChoice.TabIndex = 2;
            this.cbo_TransChoice.SelectedIndexChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
            // 
            // srmLabel2
            // 
            this.srmLabel2.AutoSize = true;
            this.srmLabel2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel2.Location = new System.Drawing.Point(12, 7);
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.Size = new System.Drawing.Size(83, 13);
            this.srmLabel2.TabIndex = 131;
            this.srmLabel2.Text = "Transition Type:";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_TransType
            // 
            this.cbo_TransType.BackColor = System.Drawing.Color.White;
            this.cbo_TransType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_TransType.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_TransType.FormattingEnabled = true;
            this.cbo_TransType.Items.AddRange(new object[] {
            "Black To White",
            "White To Black",
            "White To Black OR Black To White",
            "Black To White To Black",
            "White To Black To White"});
            this.cbo_TransType.Location = new System.Drawing.Point(12, 23);
            this.cbo_TransType.Name = "cbo_TransType";
            this.cbo_TransType.NormalBackColor = System.Drawing.Color.White;
            this.cbo_TransType.Size = new System.Drawing.Size(174, 21);
            this.cbo_TransType.TabIndex = 130;
            this.cbo_TransType.SelectedIndexChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
            // 
            // txt_FilteringPass
            // 
            this.txt_FilteringPass.BackColor = System.Drawing.Color.White;
            this.txt_FilteringPass.DataType = SRMControl.SRMDataType.Int32;
            this.txt_FilteringPass.DecimalPlaces = 0;
            this.txt_FilteringPass.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_FilteringPass.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_FilteringPass.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_FilteringPass.ForeColor = System.Drawing.Color.Black;
            this.txt_FilteringPass.InputType = SRMControl.InputType.Number;
            this.txt_FilteringPass.Location = new System.Drawing.Point(116, 139);
            this.txt_FilteringPass.Name = "txt_FilteringPass";
            this.txt_FilteringPass.NormalBackColor = System.Drawing.Color.White;
            this.txt_FilteringPass.Size = new System.Drawing.Size(77, 20);
            this.txt_FilteringPass.TabIndex = 133;
            this.txt_FilteringPass.Text = "1";
            this.txt_FilteringPass.TextChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
            // 
            // srmLabel1
            // 
            this.srmLabel1.AutoSize = true;
            this.srmLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel1.Location = new System.Drawing.Point(115, 124);
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.Size = new System.Drawing.Size(72, 13);
            this.srmLabel1.TabIndex = 132;
            this.srmLabel1.Text = "Filtering Pass:";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
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
            this.txt_threshold.ForeColor = System.Drawing.Color.Black;
            this.txt_threshold.InputType = SRMControl.InputType.Number;
            this.txt_threshold.Location = new System.Drawing.Point(13, 177);
            this.txt_threshold.Name = "txt_threshold";
            this.txt_threshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_threshold.Size = new System.Drawing.Size(77, 20);
            this.txt_threshold.TabIndex = 135;
            this.txt_threshold.Text = "1";
            this.txt_threshold.TextChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
            // 
            // srmLabel3
            // 
            this.srmLabel3.AutoSize = true;
            this.srmLabel3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel3.Location = new System.Drawing.Point(12, 162);
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.Size = new System.Drawing.Size(57, 13);
            this.srmLabel3.TabIndex = 134;
            this.srmLabel3.Text = "Threshold:";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_FilteringThreshold
            // 
            this.txt_FilteringThreshold.BackColor = System.Drawing.Color.White;
            this.txt_FilteringThreshold.DataType = SRMControl.SRMDataType.Int32;
            this.txt_FilteringThreshold.DecimalPlaces = 1;
            this.txt_FilteringThreshold.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_FilteringThreshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_FilteringThreshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_FilteringThreshold.ForeColor = System.Drawing.Color.Black;
            this.txt_FilteringThreshold.InputType = SRMControl.InputType.Number;
            this.txt_FilteringThreshold.Location = new System.Drawing.Point(116, 177);
            this.txt_FilteringThreshold.Name = "txt_FilteringThreshold";
            this.txt_FilteringThreshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_FilteringThreshold.Size = new System.Drawing.Size(77, 20);
            this.txt_FilteringThreshold.TabIndex = 137;
            this.txt_FilteringThreshold.Text = "1";
            this.txt_FilteringThreshold.TextChanged += new System.EventHandler(this.txt_GaugeSetting_TextChanged);
            // 
            // srmLabel4
            // 
            this.srmLabel4.AutoSize = true;
            this.srmLabel4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel4.Location = new System.Drawing.Point(115, 162);
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.Size = new System.Drawing.Size(96, 13);
            this.srmLabel4.TabIndex = 136;
            this.srmLabel4.Text = "Filtering Threshold:";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // AdvancedLGaugeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(224, 324);
            this.ControlBox = false;
            this.Controls.Add(this.txt_FilteringThreshold);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.txt_threshold);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.txt_FilteringPass);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.cbo_TransType);
            this.Controls.Add(this.srmLabel8);
            this.Controls.Add(this.txt_MeasFilter);
            this.Controls.Add(this.cbo_TransChoice);
            this.Controls.Add(this.txt_MeasThickness);
            this.Controls.Add(this.txt_MeasMinArea);
            this.Controls.Add(this.txt_MeasMinAmp);
            this.Controls.Add(this.srmLabel13);
            this.Controls.Add(this.srmLabel12);
            this.Controls.Add(this.srmLabel11);
            this.Controls.Add(this.srmLabel10);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedLGaugeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMInputBox txt_MeasFilter;
        private SRMControl.SRMInputBox txt_MeasThickness;
        private SRMControl.SRMInputBox txt_MeasMinArea;
        private SRMControl.SRMInputBox txt_MeasMinAmp;
        private SRMControl.SRMLabel srmLabel13;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMComboBox cbo_TransChoice;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMComboBox cbo_TransType;
        private SRMControl.SRMInputBox txt_FilteringPass;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_threshold;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMInputBox txt_FilteringThreshold;
        private SRMControl.SRMLabel srmLabel4;
    }
}