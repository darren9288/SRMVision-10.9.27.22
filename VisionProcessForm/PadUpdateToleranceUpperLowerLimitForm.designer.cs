namespace VisionProcessForm
{
    partial class PadUpdateToleranceUpperLowerLimitForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PadUpdateToleranceUpperLowerLimitForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.txt_MinArea = new SRMControl.SRMInputBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.srmInputBox1 = new SRMControl.SRMInputBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmInputBox2 = new SRMControl.SRMInputBox();
            this.srmInputBox3 = new SRMControl.SRMInputBox();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.srmInputBox4 = new SRMControl.SRMInputBox();
            this.srmInputBox5 = new SRMControl.SRMInputBox();
            this.dgd_Setting = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_SaveAndClose = new SRMControl.SRMButton();
            this.btn_SaveAndUpdateToTable = new SRMControl.SRMButton();
            this.chk_SetToSideROI = new SRMControl.SRMCheckBox();
            this.chk_SetToCenterROI = new SRMControl.SRMCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_Setting)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinArea
            // 
            resources.ApplyResources(this.txt_MinArea, "txt_MinArea");
            this.txt_MinArea.BackColor = System.Drawing.Color.White;
            this.txt_MinArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            this.txt_MinArea.Name = "txt_MinArea";
            this.txt_MinArea.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmInputBox1
            // 
            resources.ApplyResources(this.srmInputBox1, "srmInputBox1");
            this.srmInputBox1.BackColor = System.Drawing.Color.White;
            this.srmInputBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.srmInputBox1.DecimalPlaces = 0;
            this.srmInputBox1.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.srmInputBox1.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox1.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox1.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox1.InputType = SRMControl.InputType.Number;
            this.srmInputBox1.Name = "srmInputBox1";
            this.srmInputBox1.NormalBackColor = System.Drawing.Color.White;
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
            // srmInputBox2
            // 
            resources.ApplyResources(this.srmInputBox2, "srmInputBox2");
            this.srmInputBox2.BackColor = System.Drawing.Color.White;
            this.srmInputBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.srmInputBox2.DecimalPlaces = 0;
            this.srmInputBox2.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.srmInputBox2.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox2.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox2.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox2.InputType = SRMControl.InputType.Number;
            this.srmInputBox2.Name = "srmInputBox2";
            this.srmInputBox2.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmInputBox3
            // 
            resources.ApplyResources(this.srmInputBox3, "srmInputBox3");
            this.srmInputBox3.BackColor = System.Drawing.Color.White;
            this.srmInputBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.srmInputBox3.DecimalPlaces = 0;
            this.srmInputBox3.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.srmInputBox3.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox3.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox3.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox3.InputType = SRMControl.InputType.Number;
            this.srmInputBox3.Name = "srmInputBox3";
            this.srmInputBox3.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmInputBox4
            // 
            resources.ApplyResources(this.srmInputBox4, "srmInputBox4");
            this.srmInputBox4.BackColor = System.Drawing.Color.White;
            this.srmInputBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.srmInputBox4.DecimalPlaces = 0;
            this.srmInputBox4.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.srmInputBox4.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox4.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox4.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox4.InputType = SRMControl.InputType.Number;
            this.srmInputBox4.Name = "srmInputBox4";
            this.srmInputBox4.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmInputBox5
            // 
            resources.ApplyResources(this.srmInputBox5, "srmInputBox5");
            this.srmInputBox5.BackColor = System.Drawing.Color.White;
            this.srmInputBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.srmInputBox5.DecimalPlaces = 0;
            this.srmInputBox5.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.srmInputBox5.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox5.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox5.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox5.InputType = SRMControl.InputType.Number;
            this.srmInputBox5.Name = "srmInputBox5";
            this.srmInputBox5.NormalBackColor = System.Drawing.Color.White;
            // 
            // dgd_Setting
            // 
            resources.ApplyResources(this.dgd_Setting, "dgd_Setting");
            this.dgd_Setting.AllowUserToAddRows = false;
            this.dgd_Setting.AllowUserToDeleteRows = false;
            this.dgd_Setting.AllowUserToResizeColumns = false;
            this.dgd_Setting.AllowUserToResizeRows = false;
            this.dgd_Setting.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_Setting.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_Setting.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_Setting.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Setting.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_Setting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_Setting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.Column1});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Setting.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_Setting.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_Setting.MultiSelect = false;
            this.dgd_Setting.Name = "dgd_Setting";
            this.dgd_Setting.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Setting.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgd_Setting.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White;
            this.dgd_Setting.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgd_Setting.RowTemplate.Height = 24;
            this.dgd_Setting.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Setting.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgd_Setting.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_Setting_CellValueChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            // 
            // btn_SaveAndClose
            // 
            resources.ApplyResources(this.btn_SaveAndClose, "btn_SaveAndClose");
            this.btn_SaveAndClose.Name = "btn_SaveAndClose";
            this.btn_SaveAndClose.UseVisualStyleBackColor = true;
            this.btn_SaveAndClose.Click += new System.EventHandler(this.btn_SaveAndClose_Click);
            // 
            // btn_SaveAndUpdateToTable
            // 
            resources.ApplyResources(this.btn_SaveAndUpdateToTable, "btn_SaveAndUpdateToTable");
            this.btn_SaveAndUpdateToTable.Name = "btn_SaveAndUpdateToTable";
            this.btn_SaveAndUpdateToTable.UseVisualStyleBackColor = true;
            this.btn_SaveAndUpdateToTable.Click += new System.EventHandler(this.btn_SaveAndUpdateToTable_Click);
            // 
            // chk_SetToSideROI
            // 
            resources.ApplyResources(this.chk_SetToSideROI, "chk_SetToSideROI");
            this.chk_SetToSideROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToSideROI.Name = "chk_SetToSideROI";
            this.chk_SetToSideROI.Selected = false;
            this.chk_SetToSideROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToSideROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToSideROI.UseVisualStyleBackColor = true;
            // 
            // chk_SetToCenterROI
            // 
            resources.ApplyResources(this.chk_SetToCenterROI, "chk_SetToCenterROI");
            this.chk_SetToCenterROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToCenterROI.Name = "chk_SetToCenterROI";
            this.chk_SetToCenterROI.Selected = false;
            this.chk_SetToCenterROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToCenterROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToCenterROI.UseVisualStyleBackColor = true;
            // 
            // PadUpdateToleranceUpperLowerLimitForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.chk_SetToCenterROI);
            this.Controls.Add(this.chk_SetToSideROI);
            this.Controls.Add(this.btn_SaveAndUpdateToTable);
            this.Controls.Add(this.btn_SaveAndClose);
            this.Controls.Add(this.dgd_Setting);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.srmInputBox4);
            this.Controls.Add(this.srmInputBox5);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.srmInputBox2);
            this.Controls.Add(this.srmInputBox3);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.srmInputBox1);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.srmLabel5);
            this.Controls.Add(this.txt_MinArea);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PadUpdateToleranceUpperLowerLimitForm";
            this.Load += new System.EventHandler(this.PadUpdateToleranceUpperLowerLimitForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_Setting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMInputBox txt_MinArea;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox srmInputBox1;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMInputBox srmInputBox2;
        private SRMControl.SRMInputBox srmInputBox3;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMInputBox srmInputBox4;
        private SRMControl.SRMInputBox srmInputBox5;
        private System.Windows.Forms.DataGridView dgd_Setting;
        private SRMControl.SRMButton btn_SaveAndClose;
        private SRMControl.SRMButton btn_SaveAndUpdateToTable;
        private SRMControl.SRMCheckBox chk_SetToSideROI;
        private SRMControl.SRMCheckBox chk_SetToCenterROI;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}