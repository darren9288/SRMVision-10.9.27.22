namespace History
{
    partial class LotHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LotHistory));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new SRMControl.SRMLabel();
            this.cbo_Month = new SRMControl.SRMComboBox();
            this.cbo_Lot = new SRMControl.SRMComboBox();
            this.btn_Close = new SRMControl.SRMButton();
            this.dlg_ReportFile = new System.Windows.Forms.SaveFileDialog();
            this.cbo_GRR = new SRMControl.SRMComboBox();
            this.radioBtn_GRR = new SRMControl.SRMRadioButton();
            this.radioBtn_Lot = new SRMControl.SRMRadioButton();
            this.btn_GRRDisplaySetting = new SRMControl.SRMButton();
            this.tabCtrl_History = new SRMControl.SRMTabControl();
            this.tp_LotRecord = new System.Windows.Forms.TabPage();
            this.HistoryCR = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.tp_DeviceLog = new System.Windows.Forms.TabPage();
            this.dgd_DeviceEdit = new System.Windows.Forms.DataGridView();
            this.dc_User = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dc_Group = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dc_Module = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dc_Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dc_OriginalValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dc_NewValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dc_Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LotID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbo_VisionModule = new SRMControl.SRMComboBox();
            this.cbo_CPK = new SRMControl.SRMComboBox();
            this.radioBtn_CPK = new SRMControl.SRMRadioButton();
            this.btn_CPKDisplaySetting = new SRMControl.SRMButton();
            this.radioBtn_FilterByLot = new SRMControl.SRMRadioButton();
            this.radioBtn_FilterByMonth = new SRMControl.SRMRadioButton();
            this.lbl_Notice = new System.Windows.Forms.Label();
            this.btn_Lead3DGRRDisplaySetting = new SRMControl.SRMButton();
            this.btn_DeviceEditLogDisplaySetting = new SRMControl.SRMButton();
            this.btn_RemoveFilter = new SRMControl.SRMButton();
            this.lbl_Filter = new SRMControl.SRMLabel();
            this.lbl_Notice2 = new System.Windows.Forms.Label();
            this.btn_SavetoPDF = new SRMControl.SRMButton();
            this.btn_FolderStored = new SRMControl.SRMButton();
            this.tabCtrl_History.SuspendLayout();
            this.tp_LotRecord.SuspendLayout();
            this.tp_DeviceLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_DeviceEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_Month
            // 
            resources.ApplyResources(this.cbo_Month, "cbo_Month");
            this.cbo_Month.BackColor = System.Drawing.Color.White;
            this.cbo_Month.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Month.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Month.Name = "cbo_Month";
            this.cbo_Month.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Month.SelectedIndexChanged += new System.EventHandler(this.cbo_Month_SelectedIndexChanged);
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
            // dlg_ReportFile
            // 
            resources.ApplyResources(this.dlg_ReportFile, "dlg_ReportFile");
            // 
            // cbo_GRR
            // 
            resources.ApplyResources(this.cbo_GRR, "cbo_GRR");
            this.cbo_GRR.BackColor = System.Drawing.Color.White;
            this.cbo_GRR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_GRR.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_GRR.Name = "cbo_GRR";
            this.cbo_GRR.NormalBackColor = System.Drawing.Color.White;
            this.cbo_GRR.SelectedIndexChanged += new System.EventHandler(this.cbo_GRR_SelectedIndexChanged);
            // 
            // radioBtn_GRR
            // 
            resources.ApplyResources(this.radioBtn_GRR, "radioBtn_GRR");
            this.radioBtn_GRR.Name = "radioBtn_GRR";
            this.radioBtn_GRR.UseVisualStyleBackColor = true;
            this.radioBtn_GRR.Click += new System.EventHandler(this.radioBtn_GRR_Click);
            // 
            // radioBtn_Lot
            // 
            resources.ApplyResources(this.radioBtn_Lot, "radioBtn_Lot");
            this.radioBtn_Lot.Name = "radioBtn_Lot";
            this.radioBtn_Lot.UseVisualStyleBackColor = true;
            this.radioBtn_Lot.Click += new System.EventHandler(this.radioBtn_Lot_Click);
            // 
            // btn_GRRDisplaySetting
            // 
            resources.ApplyResources(this.btn_GRRDisplaySetting, "btn_GRRDisplaySetting");
            this.btn_GRRDisplaySetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_GRRDisplaySetting.Name = "btn_GRRDisplaySetting";
            this.btn_GRRDisplaySetting.Click += new System.EventHandler(this.btn_GRRDisplaySetting_Click);
            // 
            // tabCtrl_History
            // 
            resources.ApplyResources(this.tabCtrl_History, "tabCtrl_History");
            this.tabCtrl_History.Controls.Add(this.tp_LotRecord);
            this.tabCtrl_History.Controls.Add(this.tp_DeviceLog);
            this.tabCtrl_History.Name = "tabCtrl_History";
            this.tabCtrl_History.SelectedIndex = 0;
            this.tabCtrl_History.SelectedIndexChanged += new System.EventHandler(this.tabCtrl_History_SelectedIndexChanged);
            this.tabCtrl_History.TabIndexChanged += new System.EventHandler(this.tabCtrl_History_TabIndexChanged);
            // 
            // tp_LotRecord
            // 
            resources.ApplyResources(this.tp_LotRecord, "tp_LotRecord");
            this.tp_LotRecord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_LotRecord.Controls.Add(this.HistoryCR);
            this.tp_LotRecord.Name = "tp_LotRecord";
            // 
            // HistoryCR
            // 
            resources.ApplyResources(this.HistoryCR, "HistoryCR");
            this.HistoryCR.ActiveViewIndex = -1;
            this.HistoryCR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HistoryCR.Cursor = System.Windows.Forms.Cursors.Default;
            this.HistoryCR.DisplayStatusBar = false;
            this.HistoryCR.EnableToolTips = false;
            this.HistoryCR.Name = "HistoryCR";
            this.HistoryCR.SelectionFormula = "";
            this.HistoryCR.ViewTimeSelectionFormula = "";
            // 
            // tp_DeviceLog
            // 
            resources.ApplyResources(this.tp_DeviceLog, "tp_DeviceLog");
            this.tp_DeviceLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_DeviceLog.Controls.Add(this.dgd_DeviceEdit);
            this.tp_DeviceLog.Name = "tp_DeviceLog";
            // 
            // dgd_DeviceEdit
            // 
            resources.ApplyResources(this.dgd_DeviceEdit, "dgd_DeviceEdit");
            this.dgd_DeviceEdit.AllowUserToAddRows = false;
            this.dgd_DeviceEdit.AllowUserToDeleteRows = false;
            this.dgd_DeviceEdit.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(222)))), ((int)(((byte)(255)))));
            this.dgd_DeviceEdit.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_DeviceEdit.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dgd_DeviceEdit.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(190)))), ((int)(((byte)(226)))));
            this.dgd_DeviceEdit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_DeviceEdit.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(190)))), ((int)(((byte)(226)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgd_DeviceEdit.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgd_DeviceEdit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_DeviceEdit.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dc_User,
            this.dc_Group,
            this.dc_Module,
            this.dc_Desc,
            this.dc_OriginalValue,
            this.dc_NewValue,
            this.dc_Date,
            this.LotID});
            this.dgd_DeviceEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgd_DeviceEdit.EnableHeadersVisualStyles = false;
            this.dgd_DeviceEdit.GridColor = System.Drawing.Color.White;
            this.dgd_DeviceEdit.MultiSelect = false;
            this.dgd_DeviceEdit.Name = "dgd_DeviceEdit";
            this.dgd_DeviceEdit.RowHeadersVisible = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(236)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(0, 2, 0, 1);
            this.dgd_DeviceEdit.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgd_DeviceEdit.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgd_DeviceEdit.RowTemplate.Height = 20;
            this.dgd_DeviceEdit.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_DeviceEdit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgd_DeviceEdit.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_DeviceEdit_CellDoubleClick);
            // 
            // dc_User
            // 
            this.dc_User.DataPropertyName = "UserName";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dc_User.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.dc_User, "dc_User");
            this.dc_User.Name = "dc_User";
            this.dc_User.ReadOnly = true;
            this.dc_User.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dc_Group
            // 
            this.dc_Group.DataPropertyName = "Group";
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dc_Group.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.dc_Group, "dc_Group");
            this.dc_Group.Name = "dc_Group";
            this.dc_Group.ReadOnly = true;
            this.dc_Group.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dc_Module
            // 
            this.dc_Module.DataPropertyName = "Module";
            resources.ApplyResources(this.dc_Module, "dc_Module");
            this.dc_Module.Name = "dc_Module";
            this.dc_Module.ReadOnly = true;
            // 
            // dc_Desc
            // 
            this.dc_Desc.DataPropertyName = "Description";
            resources.ApplyResources(this.dc_Desc, "dc_Desc");
            this.dc_Desc.Name = "dc_Desc";
            this.dc_Desc.ReadOnly = true;
            // 
            // dc_OriginalValue
            // 
            this.dc_OriginalValue.DataPropertyName = "OriginalValue";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dc_OriginalValue.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.dc_OriginalValue, "dc_OriginalValue");
            this.dc_OriginalValue.Name = "dc_OriginalValue";
            this.dc_OriginalValue.ReadOnly = true;
            this.dc_OriginalValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dc_NewValue
            // 
            this.dc_NewValue.DataPropertyName = "NewValue";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dc_NewValue.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.dc_NewValue, "dc_NewValue");
            this.dc_NewValue.Name = "dc_NewValue";
            this.dc_NewValue.ReadOnly = true;
            this.dc_NewValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dc_Date
            // 
            this.dc_Date.DataPropertyName = "ModifiedDate";
            resources.ApplyResources(this.dc_Date, "dc_Date");
            this.dc_Date.Name = "dc_Date";
            this.dc_Date.ReadOnly = true;
            // 
            // LotID
            // 
            this.LotID.DataPropertyName = "LotID";
            resources.ApplyResources(this.LotID, "LotID");
            this.LotID.Name = "LotID";
            this.LotID.ReadOnly = true;
            // 
            // cbo_VisionModule
            // 
            resources.ApplyResources(this.cbo_VisionModule, "cbo_VisionModule");
            this.cbo_VisionModule.BackColor = System.Drawing.Color.White;
            this.cbo_VisionModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_VisionModule.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_VisionModule.Name = "cbo_VisionModule";
            this.cbo_VisionModule.NormalBackColor = System.Drawing.Color.White;
            this.cbo_VisionModule.SelectedIndexChanged += new System.EventHandler(this.cbo_VisionModule_SelectedIndexChanged);
            // 
            // cbo_CPK
            // 
            resources.ApplyResources(this.cbo_CPK, "cbo_CPK");
            this.cbo_CPK.BackColor = System.Drawing.Color.White;
            this.cbo_CPK.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_CPK.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_CPK.Name = "cbo_CPK";
            this.cbo_CPK.NormalBackColor = System.Drawing.Color.White;
            this.cbo_CPK.SelectedIndexChanged += new System.EventHandler(this.cbo_CPK_SelectedIndexChanged);
            // 
            // radioBtn_CPK
            // 
            resources.ApplyResources(this.radioBtn_CPK, "radioBtn_CPK");
            this.radioBtn_CPK.Name = "radioBtn_CPK";
            this.radioBtn_CPK.UseVisualStyleBackColor = true;
            this.radioBtn_CPK.Click += new System.EventHandler(this.radioBtn_CPK_Click);
            // 
            // btn_CPKDisplaySetting
            // 
            resources.ApplyResources(this.btn_CPKDisplaySetting, "btn_CPKDisplaySetting");
            this.btn_CPKDisplaySetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CPKDisplaySetting.Name = "btn_CPKDisplaySetting";
            this.btn_CPKDisplaySetting.Click += new System.EventHandler(this.sbtn_CPKDisplaySetting_Click);
            // 
            // radioBtn_FilterByLot
            // 
            resources.ApplyResources(this.radioBtn_FilterByLot, "radioBtn_FilterByLot");
            this.radioBtn_FilterByLot.Name = "radioBtn_FilterByLot";
            this.radioBtn_FilterByLot.UseVisualStyleBackColor = true;
            this.radioBtn_FilterByLot.Click += new System.EventHandler(this.radioBtn_Filter_Click);
            // 
            // radioBtn_FilterByMonth
            // 
            resources.ApplyResources(this.radioBtn_FilterByMonth, "radioBtn_FilterByMonth");
            this.radioBtn_FilterByMonth.Name = "radioBtn_FilterByMonth";
            this.radioBtn_FilterByMonth.UseVisualStyleBackColor = true;
            this.radioBtn_FilterByMonth.Click += new System.EventHandler(this.radioBtn_Filter_Click);
            // 
            // lbl_Notice
            // 
            resources.ApplyResources(this.lbl_Notice, "lbl_Notice");
            this.lbl_Notice.Name = "lbl_Notice";
            // 
            // btn_Lead3DGRRDisplaySetting
            // 
            resources.ApplyResources(this.btn_Lead3DGRRDisplaySetting, "btn_Lead3DGRRDisplaySetting");
            this.btn_Lead3DGRRDisplaySetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Lead3DGRRDisplaySetting.Name = "btn_Lead3DGRRDisplaySetting";
            this.btn_Lead3DGRRDisplaySetting.Click += new System.EventHandler(this.btn_Lead3DGRRDisplaySetting_Click);
            // 
            // btn_DeviceEditLogDisplaySetting
            // 
            resources.ApplyResources(this.btn_DeviceEditLogDisplaySetting, "btn_DeviceEditLogDisplaySetting");
            this.btn_DeviceEditLogDisplaySetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_DeviceEditLogDisplaySetting.Name = "btn_DeviceEditLogDisplaySetting";
            this.btn_DeviceEditLogDisplaySetting.Click += new System.EventHandler(this.btn_DeviceEditLogDisplaySetting_Click);
            // 
            // btn_RemoveFilter
            // 
            resources.ApplyResources(this.btn_RemoveFilter, "btn_RemoveFilter");
            this.btn_RemoveFilter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_RemoveFilter.Name = "btn_RemoveFilter";
            this.btn_RemoveFilter.Click += new System.EventHandler(this.btn_RemoveFilter_Click);
            // 
            // lbl_Filter
            // 
            resources.ApplyResources(this.lbl_Filter, "lbl_Filter");
            this.lbl_Filter.ForeColor = System.Drawing.Color.Red;
            this.lbl_Filter.Name = "lbl_Filter";
            this.lbl_Filter.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Notice2
            // 
            resources.ApplyResources(this.lbl_Notice2, "lbl_Notice2");
            this.lbl_Notice2.Name = "lbl_Notice2";
            // 
            // btn_SavetoPDF
            // 
            resources.ApplyResources(this.btn_SavetoPDF, "btn_SavetoPDF");
            this.btn_SavetoPDF.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SavetoPDF.Name = "btn_SavetoPDF";
            this.btn_SavetoPDF.Click += new System.EventHandler(this.btn_SavetoPDF_Click);
            // 
            // btn_FolderStored
            // 
            resources.ApplyResources(this.btn_FolderStored, "btn_FolderStored");
            this.btn_FolderStored.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_FolderStored.Name = "btn_FolderStored";
            this.btn_FolderStored.Click += new System.EventHandler(this.btn_FolderStored_Click);
            // 
            // LotHistory
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_FolderStored);
            this.Controls.Add(this.btn_SavetoPDF);
            this.Controls.Add(this.lbl_Notice2);
            this.Controls.Add(this.lbl_Filter);
            this.Controls.Add(this.btn_RemoveFilter);
            this.Controls.Add(this.btn_DeviceEditLogDisplaySetting);
            this.Controls.Add(this.lbl_Notice);
            this.Controls.Add(this.radioBtn_FilterByMonth);
            this.Controls.Add(this.radioBtn_FilterByLot);
            this.Controls.Add(this.btn_CPKDisplaySetting);
            this.Controls.Add(this.cbo_CPK);
            this.Controls.Add(this.radioBtn_CPK);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.cbo_VisionModule);
            this.Controls.Add(this.cbo_GRR);
            this.Controls.Add(this.tabCtrl_History);
            this.Controls.Add(this.radioBtn_Lot);
            this.Controls.Add(this.radioBtn_GRR);
            this.Controls.Add(this.cbo_Lot);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbo_Month);
            this.Controls.Add(this.btn_GRRDisplaySetting);
            this.Controls.Add(this.btn_Lead3DGRRDisplaySetting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LotHistory";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LotHistory_FormClosing);
            this.Load += new System.EventHandler(this.LotHistory_Load);
            this.tabCtrl_History.ResumeLayout(false);
            this.tp_LotRecord.ResumeLayout(false);
            this.tp_DeviceLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_DeviceEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel label1;
        private SRMControl.SRMComboBox cbo_Month;
        private SRMControl.SRMComboBox cbo_Lot;
        private SRMControl.SRMButton btn_Close;
        private System.Windows.Forms.SaveFileDialog dlg_ReportFile;
        private SRMControl.SRMComboBox cbo_GRR;
        private SRMControl.SRMRadioButton radioBtn_GRR;
        private SRMControl.SRMRadioButton radioBtn_Lot;
        private SRMControl.SRMButton btn_GRRDisplaySetting;
        private SRMControl.SRMTabControl tabCtrl_History;
        private System.Windows.Forms.TabPage tp_LotRecord;
        private System.Windows.Forms.TabPage tp_DeviceLog;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer HistoryCR;
        private System.Windows.Forms.DataGridView dgd_DeviceEdit;
        private SRMControl.SRMComboBox cbo_VisionModule;
        private SRMControl.SRMComboBox cbo_CPK;
        private SRMControl.SRMRadioButton radioBtn_CPK;
        private SRMControl.SRMButton btn_CPKDisplaySetting;
        private SRMControl.SRMRadioButton radioBtn_FilterByLot;
        private SRMControl.SRMRadioButton radioBtn_FilterByMonth;
        private System.Windows.Forms.Label lbl_Notice;
        private SRMControl.SRMButton btn_Lead3DGRRDisplaySetting;
        private SRMControl.SRMButton btn_DeviceEditLogDisplaySetting;
        private SRMControl.SRMButton btn_RemoveFilter;
        private SRMControl.SRMLabel lbl_Filter;
        private System.Windows.Forms.Label lbl_Notice2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_User;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_Group;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_Module;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_Desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_OriginalValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_NewValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn LotID;
        private SRMControl.SRMButton btn_SavetoPDF;
        private SRMControl.SRMButton btn_FolderStored;
    }
}