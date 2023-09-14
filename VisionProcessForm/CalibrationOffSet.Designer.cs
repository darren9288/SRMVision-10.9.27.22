namespace VisionProcessForm
{
    partial class CalibrationOffSet
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.dgd_PitchGapSetting = new System.Windows.Forms.DataGridView();
            this.column_FromPadNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_ToPadNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_MinSetPitch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_MaxSetPitch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Add = new SRMControl.SRMButton();
            this.btn_Delete = new SRMControl.SRMButton();
            this.txt_LineAngle = new SRMControl.SRMInputBox();
            this.lbl_deg = new System.Windows.Forms.Label();
            this.lbl_UnitWidthOffSet = new System.Windows.Forms.Label();
            this.lbl_UnitHeightOffSet = new System.Windows.Forms.Label();
            this.srmInputBox1 = new SRMControl.SRMInputBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_PitchGapSetting)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(270, 349);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 141;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(184, 349);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 34);
            this.btn_OK.TabIndex = 140;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // dgd_PitchGapSetting
            // 
            this.dgd_PitchGapSetting.AllowUserToAddRows = false;
            this.dgd_PitchGapSetting.AllowUserToDeleteRows = false;
            this.dgd_PitchGapSetting.AllowUserToResizeColumns = false;
            this.dgd_PitchGapSetting.AllowUserToResizeRows = false;
            this.dgd_PitchGapSetting.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_PitchGapSetting.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_PitchGapSetting.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_PitchGapSetting.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_PitchGapSetting.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_PitchGapSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_PitchGapSetting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column_FromPadNo,
            this.column_ToPadNo,
            this.column_MinSetPitch,
            this.column_MaxSetPitch});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_PitchGapSetting.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgd_PitchGapSetting.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_PitchGapSetting.Location = new System.Drawing.Point(12, 79);
            this.dgd_PitchGapSetting.MultiSelect = false;
            this.dgd_PitchGapSetting.Name = "dgd_PitchGapSetting";
            this.dgd_PitchGapSetting.ReadOnly = true;
            this.dgd_PitchGapSetting.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_PitchGapSetting.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgd_PitchGapSetting.RowHeadersVisible = false;
            this.dgd_PitchGapSetting.RowHeadersWidth = 100;
            this.dgd_PitchGapSetting.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgd_PitchGapSetting.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_PitchGapSetting.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_PitchGapSetting.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgd_PitchGapSetting.Size = new System.Drawing.Size(338, 264);
            this.dgd_PitchGapSetting.TabIndex = 871;
            // 
            // column_FromPadNo
            // 
            this.column_FromPadNo.HeaderText = "Level #";
            this.column_FromPadNo.Name = "column_FromPadNo";
            this.column_FromPadNo.ReadOnly = true;
            this.column_FromPadNo.Width = 70;
            // 
            // column_ToPadNo
            // 
            this.column_ToPadNo.HeaderText = "From Size";
            this.column_ToPadNo.Name = "column_ToPadNo";
            this.column_ToPadNo.ReadOnly = true;
            this.column_ToPadNo.Width = 80;
            // 
            // column_MinSetPitch
            // 
            this.column_MinSetPitch.HeaderText = "To Size";
            this.column_MinSetPitch.Name = "column_MinSetPitch";
            this.column_MinSetPitch.ReadOnly = true;
            this.column_MinSetPitch.Width = 80;
            // 
            // column_MaxSetPitch
            // 
            this.column_MaxSetPitch.HeaderText = "Off Set";
            this.column_MaxSetPitch.Name = "column_MaxSetPitch";
            this.column_MaxSetPitch.ReadOnly = true;
            // 
            // btn_Add
            // 
            this.btn_Add.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Add.Location = new System.Drawing.Point(12, 349);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(80, 34);
            this.btn_Add.TabIndex = 872;
            this.btn_Add.Text = "Add";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Delete.Location = new System.Drawing.Point(98, 349);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(80, 34);
            this.btn_Delete.TabIndex = 873;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // txt_LineAngle
            // 
            this.txt_LineAngle.BackColor = System.Drawing.Color.White;
            this.txt_LineAngle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_LineAngle.DecimalPlaces = 0;
            this.txt_LineAngle.DecMaxValue = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.txt_LineAngle.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_LineAngle.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_LineAngle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.txt_LineAngle.ForeColor = System.Drawing.Color.Black;
            this.txt_LineAngle.InputType = SRMControl.InputType.Number;
            this.txt_LineAngle.Location = new System.Drawing.Point(123, 11);
            this.txt_LineAngle.Name = "txt_LineAngle";
            this.txt_LineAngle.NormalBackColor = System.Drawing.Color.White;
            this.txt_LineAngle.Size = new System.Drawing.Size(79, 24);
            this.txt_LineAngle.TabIndex = 876;
            this.txt_LineAngle.Text = "0";
            // 
            // lbl_deg
            // 
            this.lbl_deg.AutoSize = true;
            this.lbl_deg.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_deg.Location = new System.Drawing.Point(209, 17);
            this.lbl_deg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_deg.Name = "lbl_deg";
            this.lbl_deg.Size = new System.Drawing.Size(23, 13);
            this.lbl_deg.TabIndex = 875;
            this.lbl_deg.Text = "mm";
            // 
            // lbl_UnitWidthOffSet
            // 
            this.lbl_UnitWidthOffSet.AutoSize = true;
            this.lbl_UnitWidthOffSet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_UnitWidthOffSet.Location = new System.Drawing.Point(16, 17);
            this.lbl_UnitWidthOffSet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_UnitWidthOffSet.Name = "lbl_UnitWidthOffSet";
            this.lbl_UnitWidthOffSet.Size = new System.Drawing.Size(90, 13);
            this.lbl_UnitWidthOffSet.TabIndex = 874;
            this.lbl_UnitWidthOffSet.Text = "Unit Width OffSet";
            // 
            // lbl_UnitHeightOffSet
            // 
            this.lbl_UnitHeightOffSet.AutoSize = true;
            this.lbl_UnitHeightOffSet.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_UnitHeightOffSet.Location = new System.Drawing.Point(16, 42);
            this.lbl_UnitHeightOffSet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_UnitHeightOffSet.Name = "lbl_UnitHeightOffSet";
            this.lbl_UnitHeightOffSet.Size = new System.Drawing.Size(93, 13);
            this.lbl_UnitHeightOffSet.TabIndex = 877;
            this.lbl_UnitHeightOffSet.Text = "Unit Height OffSet";
            // 
            // srmInputBox1
            // 
            this.srmInputBox1.BackColor = System.Drawing.Color.White;
            this.srmInputBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.srmInputBox1.DecimalPlaces = 0;
            this.srmInputBox1.DecMaxValue = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.srmInputBox1.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox1.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.srmInputBox1.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox1.InputType = SRMControl.InputType.Number;
            this.srmInputBox1.Location = new System.Drawing.Point(123, 37);
            this.srmInputBox1.Name = "srmInputBox1";
            this.srmInputBox1.NormalBackColor = System.Drawing.Color.White;
            this.srmInputBox1.Size = new System.Drawing.Size(79, 24);
            this.srmInputBox1.TabIndex = 878;
            this.srmInputBox1.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(209, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 879;
            this.label1.Text = "mm";
            // 
            // CalibrationOffSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(363, 396);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.srmInputBox1);
            this.Controls.Add(this.lbl_UnitHeightOffSet);
            this.Controls.Add(this.txt_LineAngle);
            this.Controls.Add(this.lbl_deg);
            this.Controls.Add(this.lbl_UnitWidthOffSet);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.dgd_PitchGapSetting);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CalibrationOffSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calibration OffSet Setting";
            this.Load += new System.EventHandler(this.CalibrationOffSet_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationOffSet_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_PitchGapSetting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.DataGridView dgd_PitchGapSetting;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_FromPadNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_ToPadNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_MinSetPitch;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_MaxSetPitch;
        private SRMControl.SRMButton btn_Add;
        private SRMControl.SRMButton btn_Delete;
        private SRMControl.SRMInputBox txt_LineAngle;
        private System.Windows.Forms.Label lbl_deg;
        private System.Windows.Forms.Label lbl_UnitWidthOffSet;
        private System.Windows.Forms.Label lbl_UnitHeightOffSet;
        private SRMControl.SRMInputBox srmInputBox1;
        private System.Windows.Forms.Label label1;
    }
}