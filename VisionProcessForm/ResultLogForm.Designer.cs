namespace VisionProcessForm
{
    partial class ResultLogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultLogForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgd_ResultLog = new System.Windows.Forms.DataGridView();
            this.txt_TotalResultCount = new SRMControl.SRMInputBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.lbl_PinScoreTitle = new SRMControl.SRMLabel();
            this.cbo_Lot = new SRMControl.SRMComboBox();
            this.btn_Close = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_SaveResult = new SRMControl.SRMButton();
            this.cbo_File = new SRMControl.SRMComboBox();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.chk_WantRecordResult = new SRMControl.SRMCheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ResultLog)).BeginInit();
            this.SuspendLayout();
            // 
            // dgd_ResultLog
            // 
            resources.ApplyResources(this.dgd_ResultLog, "dgd_ResultLog");
            this.dgd_ResultLog.AllowUserToAddRows = false;
            this.dgd_ResultLog.AllowUserToDeleteRows = false;
            this.dgd_ResultLog.AllowUserToResizeColumns = false;
            this.dgd_ResultLog.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgd_ResultLog.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_ResultLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgd_ResultLog.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgd_ResultLog.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_ResultLog.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_ResultLog.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgd_ResultLog.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ResultLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgd_ResultLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ResultLog.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgd_ResultLog.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_ResultLog.MultiSelect = false;
            this.dgd_ResultLog.Name = "dgd_ResultLog";
            this.dgd_ResultLog.ReadOnly = true;
            this.dgd_ResultLog.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ResultLog.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_ResultLog.RowHeadersVisible = false;
            this.dgd_ResultLog.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgd_ResultLog.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgd_ResultLog.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgd_ResultLog.RowTemplate.ReadOnly = true;
            this.dgd_ResultLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // txt_TotalResultCount
            // 
            resources.ApplyResources(this.txt_TotalResultCount, "txt_TotalResultCount");
            this.txt_TotalResultCount.BackColor = System.Drawing.Color.White;
            this.txt_TotalResultCount.DecimalPlaces = 0;
            this.txt_TotalResultCount.DecMaxValue = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.txt_TotalResultCount.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_TotalResultCount.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_TotalResultCount.ForeColor = System.Drawing.Color.Black;
            this.txt_TotalResultCount.InputType = SRMControl.InputType.Number;
            this.txt_TotalResultCount.Name = "txt_TotalResultCount";
            this.txt_TotalResultCount.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PinScoreTitle
            // 
            resources.ApplyResources(this.lbl_PinScoreTitle, "lbl_PinScoreTitle");
            this.lbl_PinScoreTitle.Name = "lbl_PinScoreTitle";
            this.lbl_PinScoreTitle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_Lot
            // 
            resources.ApplyResources(this.cbo_Lot, "cbo_Lot");
            this.cbo_Lot.BackColor = System.Drawing.Color.White;
            this.cbo_Lot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Lot.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Lot.Name = "cbo_Lot";
            this.cbo_Lot.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Lot.SelectedIndexChanged += new System.EventHandler(this.cbo_Lot_SelectedIndexChanged);
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_SaveResult
            // 
            resources.ApplyResources(this.btn_SaveResult, "btn_SaveResult");
            this.btn_SaveResult.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SaveResult.Name = "btn_SaveResult";
            this.btn_SaveResult.Click += new System.EventHandler(this.btn_SaveResult_Click);
            // 
            // cbo_File
            // 
            resources.ApplyResources(this.cbo_File, "cbo_File");
            this.cbo_File.BackColor = System.Drawing.Color.White;
            this.cbo_File.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_File.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_File.Name = "cbo_File";
            this.cbo_File.NormalBackColor = System.Drawing.Color.White;
            this.cbo_File.SelectedIndexChanged += new System.EventHandler(this.cbo_File_SelectedIndexChanged);
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_WantRecordResult
            // 
            resources.ApplyResources(this.chk_WantRecordResult, "chk_WantRecordResult");
            this.chk_WantRecordResult.CheckedColor = System.Drawing.Color.Empty;
            this.chk_WantRecordResult.Name = "chk_WantRecordResult";
            this.chk_WantRecordResult.Selected = false;
            this.chk_WantRecordResult.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantRecordResult.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ResultLogForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.chk_WantRecordResult);
            this.Controls.Add(this.cbo_File);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.btn_SaveResult);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.cbo_Lot);
            this.Controls.Add(this.lbl_PinScoreTitle);
            this.Controls.Add(this.txt_TotalResultCount);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.dgd_ResultLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResultLogForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResultLogForm_FormClosing);
            this.Load += new System.EventHandler(this.ResultLogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ResultLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgd_ResultLog;
        private SRMControl.SRMInputBox txt_TotalResultCount;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel lbl_PinScoreTitle;
        private SRMControl.SRMComboBox cbo_Lot;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMButton btn_SaveResult;
        private SRMControl.SRMComboBox cbo_File;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMCheckBox chk_WantRecordResult;
        private System.Windows.Forms.Timer timer1;
    }
}