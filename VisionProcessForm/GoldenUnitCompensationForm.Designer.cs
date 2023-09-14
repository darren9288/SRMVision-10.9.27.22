namespace VisionProcessForm
{
    partial class GoldenUnitCompensationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoldenUnitCompensationForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_CalculateCompensation = new SRMControl.SRMButton();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.srmGroupBox7 = new SRMControl.SRMGroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_OffSetY = new System.Windows.Forms.Label();
            this.lbl_OffSetX = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.chk_ViewGoldenColumn = new SRMControl.SRMCheckBox();
            this.btn_AddGoldenDataSet = new SRMControl.SRMButton();
            this.btn_DeleteGoldenDataSet = new SRMControl.SRMButton();
            this.dgd_GoldenSetList = new System.Windows.Forms.DataGridView();
            this.column_SetNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Use = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.btn_CalculateUsingThreshold = new SRMControl.SRMButton();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmGroupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_GoldenSetList)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_CalculateCompensation
            // 
            resources.ApplyResources(this.btn_CalculateCompensation, "btn_CalculateCompensation");
            this.btn_CalculateCompensation.Name = "btn_CalculateCompensation";
            this.btn_CalculateCompensation.UseVisualStyleBackColor = true;
            this.btn_CalculateCompensation.Click += new System.EventHandler(this.btn_CalculateCompensation_Click);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmGroupBox7
            // 
            this.srmGroupBox7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox7.Controls.Add(this.label2);
            this.srmGroupBox7.Controls.Add(this.label1);
            this.srmGroupBox7.Controls.Add(this.lbl_OffSetY);
            this.srmGroupBox7.Controls.Add(this.lbl_OffSetX);
            this.srmGroupBox7.Controls.Add(this.label7);
            this.srmGroupBox7.Controls.Add(this.label12);
            resources.ApplyResources(this.srmGroupBox7, "srmGroupBox7");
            this.srmGroupBox7.Name = "srmGroupBox7";
            this.srmGroupBox7.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lbl_OffSetY
            // 
            this.lbl_OffSetY.BackColor = System.Drawing.Color.AliceBlue;
            this.lbl_OffSetY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OffSetY, "lbl_OffSetY");
            this.lbl_OffSetY.Name = "lbl_OffSetY";
            // 
            // lbl_OffSetX
            // 
            this.lbl_OffSetX.BackColor = System.Drawing.Color.AliceBlue;
            this.lbl_OffSetX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OffSetX, "lbl_OffSetX");
            this.lbl_OffSetX.Name = "lbl_OffSetX";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // chk_ViewGoldenColumn
            // 
            resources.ApplyResources(this.chk_ViewGoldenColumn, "chk_ViewGoldenColumn");
            this.chk_ViewGoldenColumn.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_ViewGoldenColumn.Name = "chk_ViewGoldenColumn";
            this.chk_ViewGoldenColumn.Selected = false;
            this.chk_ViewGoldenColumn.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_ViewGoldenColumn.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_ViewGoldenColumn.UseVisualStyleBackColor = true;
            // 
            // btn_AddGoldenDataSet
            // 
            resources.ApplyResources(this.btn_AddGoldenDataSet, "btn_AddGoldenDataSet");
            this.btn_AddGoldenDataSet.Name = "btn_AddGoldenDataSet";
            this.btn_AddGoldenDataSet.UseVisualStyleBackColor = true;
            this.btn_AddGoldenDataSet.Click += new System.EventHandler(this.btn_AddGoldenDataSet_Click);
            // 
            // btn_DeleteGoldenDataSet
            // 
            resources.ApplyResources(this.btn_DeleteGoldenDataSet, "btn_DeleteGoldenDataSet");
            this.btn_DeleteGoldenDataSet.Name = "btn_DeleteGoldenDataSet";
            this.btn_DeleteGoldenDataSet.UseVisualStyleBackColor = true;
            this.btn_DeleteGoldenDataSet.Click += new System.EventHandler(this.btn_DeleteGoldenDataSet_Click);
            // 
            // dgd_GoldenSetList
            // 
            this.dgd_GoldenSetList.AllowUserToAddRows = false;
            this.dgd_GoldenSetList.AllowUserToDeleteRows = false;
            this.dgd_GoldenSetList.AllowUserToResizeColumns = false;
            this.dgd_GoldenSetList.AllowUserToResizeRows = false;
            this.dgd_GoldenSetList.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_GoldenSetList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_GoldenSetList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_GoldenSetList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_GoldenSetList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.dgd_GoldenSetList, "dgd_GoldenSetList");
            this.dgd_GoldenSetList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_GoldenSetList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column_SetNo,
            this.column_Use});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_GoldenSetList.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgd_GoldenSetList.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_GoldenSetList.MultiSelect = false;
            this.dgd_GoldenSetList.Name = "dgd_GoldenSetList";
            this.dgd_GoldenSetList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_GoldenSetList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgd_GoldenSetList.RowHeadersVisible = false;
            this.dgd_GoldenSetList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_GoldenSetList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_GoldenSetList.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_GoldenSetList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgd_GoldenSetList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_GoldenSetList_CellValueChanged);
            // 
            // column_SetNo
            // 
            this.column_SetNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.column_SetNo, "column_SetNo");
            this.column_SetNo.Name = "column_SetNo";
            this.column_SetNo.ReadOnly = true;
            this.column_SetNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.column_SetNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_Use
            // 
            resources.ApplyResources(this.column_Use, "column_Use");
            this.column_Use.Name = "column_Use";
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_CalculateUsingThreshold
            // 
            resources.ApplyResources(this.btn_CalculateUsingThreshold, "btn_CalculateUsingThreshold");
            this.btn_CalculateUsingThreshold.Name = "btn_CalculateUsingThreshold";
            this.btn_CalculateUsingThreshold.UseVisualStyleBackColor = true;
            this.btn_CalculateUsingThreshold.Click += new System.EventHandler(this.btn_CalculateUsingThreshold_Click);
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // GoldenUnitCompensationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.btn_CalculateUsingThreshold);
            this.Controls.Add(this.dgd_GoldenSetList);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.btn_DeleteGoldenDataSet);
            this.Controls.Add(this.btn_AddGoldenDataSet);
            this.Controls.Add(this.chk_ViewGoldenColumn);
            this.Controls.Add(this.srmGroupBox7);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.btn_CalculateCompensation);
            this.Controls.Add(this.btn_Cancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GoldenUnitCompensationForm";
            this.srmGroupBox7.ResumeLayout(false);
            this.srmGroupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_GoldenSetList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_CalculateCompensation;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMGroupBox srmGroupBox7;
        private System.Windows.Forms.Label lbl_OffSetY;
        private System.Windows.Forms.Label lbl_OffSetX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private SRMControl.SRMCheckBox chk_ViewGoldenColumn;
        private SRMControl.SRMButton btn_AddGoldenDataSet;
        private SRMControl.SRMButton btn_DeleteGoldenDataSet;
        private System.Windows.Forms.DataGridView dgd_GoldenSetList;
        private SRMControl.SRMLabel srmLabel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_SetNo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn column_Use;
        private SRMControl.SRMButton btn_CalculateUsingThreshold;
        private SRMControl.SRMLabel srmLabel3;
    }
}