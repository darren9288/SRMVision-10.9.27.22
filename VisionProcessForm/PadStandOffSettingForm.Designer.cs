namespace VisionProcessForm
{
    partial class PadStandOffSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PadStandOffSettingForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgd_PadStandOffSetting = new System.Windows.Forms.DataGridView();
            this.col_ReferTopBottom = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.col_Top = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_Bottom = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_Separate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ReferLeftRight = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.col_Left = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_Right = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.timer_PadResult = new System.Windows.Forms.Timer(this.components);
            this.srmGroupBox5 = new SRMControl.SRMGroupBox();
            this.radioBtn_Middle = new SRMControl.SRMRadioButton();
            this.radioBtn_Down = new SRMControl.SRMRadioButton();
            this.radioBtn_Left = new SRMControl.SRMRadioButton();
            this.radioBtn_Up = new SRMControl.SRMRadioButton();
            this.radioBtn_Right = new SRMControl.SRMRadioButton();
            this.btn_Close = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.chk_SetToAllRows = new SRMControl.SRMCheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chk_SetToAllSideROI = new SRMControl.SRMCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_PadStandOffSetting)).BeginInit();
            this.srmGroupBox5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgd_PadStandOffSetting
            // 
            resources.ApplyResources(this.dgd_PadStandOffSetting, "dgd_PadStandOffSetting");
            this.dgd_PadStandOffSetting.AllowUserToAddRows = false;
            this.dgd_PadStandOffSetting.AllowUserToDeleteRows = false;
            this.dgd_PadStandOffSetting.AllowUserToResizeColumns = false;
            this.dgd_PadStandOffSetting.AllowUserToResizeRows = false;
            this.dgd_PadStandOffSetting.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_PadStandOffSetting.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_PadStandOffSetting.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_PadStandOffSetting.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_PadStandOffSetting.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_PadStandOffSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_PadStandOffSetting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_ReferTopBottom,
            this.col_Top,
            this.col_Bottom,
            this.Column_Separate,
            this.col_ReferLeftRight,
            this.col_Left,
            this.col_Right});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_PadStandOffSetting.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgd_PadStandOffSetting.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_PadStandOffSetting.MultiSelect = false;
            this.dgd_PadStandOffSetting.Name = "dgd_PadStandOffSetting";
            this.dgd_PadStandOffSetting.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_PadStandOffSetting.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgd_PadStandOffSetting.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_PadStandOffSetting.RowsDefaultCellStyle = dataGridViewCellStyle11;
            this.dgd_PadStandOffSetting.RowTemplate.Height = 24;
            this.dgd_PadStandOffSetting.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_PadStandOffSetting.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgd_PadStandOffSetting.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_PadStandOffSetting_CellClick);
            this.dgd_PadStandOffSetting.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_PadStandOffSetting_CellContentClick);
            this.dgd_PadStandOffSetting.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_PadStandOffSetting_CellDoubleClick);
            this.dgd_PadStandOffSetting.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_PadStandOffSetting_CellValueChanged);
            this.dgd_PadStandOffSetting.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgd_PadStandOffSetting_CurrentCellDirtyStateChanged);
            // 
            // col_ReferTopBottom
            // 
            this.col_ReferTopBottom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.col_ReferTopBottom.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.col_ReferTopBottom, "col_ReferTopBottom");
            this.col_ReferTopBottom.Items.AddRange(new object[] {
            "Top",
            "Bottom"});
            this.col_ReferTopBottom.Name = "col_ReferTopBottom";
            this.col_ReferTopBottom.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // col_Top
            // 
            this.col_Top.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.NullValue = false;
            this.col_Top.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.col_Top, "col_Top");
            this.col_Top.Name = "col_Top";
            this.col_Top.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // col_Bottom
            // 
            this.col_Bottom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.NullValue = false;
            this.col_Bottom.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.col_Bottom, "col_Bottom");
            this.col_Bottom.Name = "col_Bottom";
            this.col_Bottom.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Column_Separate
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.DimGray;
            this.Column_Separate.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.Column_Separate, "Column_Separate");
            this.Column_Separate.Name = "Column_Separate";
            this.Column_Separate.ReadOnly = true;
            this.Column_Separate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // col_ReferLeftRight
            // 
            this.col_ReferLeftRight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.col_ReferLeftRight.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.col_ReferLeftRight, "col_ReferLeftRight");
            this.col_ReferLeftRight.Items.AddRange(new object[] {
            "Left",
            "Right"});
            this.col_ReferLeftRight.Name = "col_ReferLeftRight";
            this.col_ReferLeftRight.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // col_Left
            // 
            this.col_Left.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.NullValue = false;
            this.col_Left.DefaultCellStyle = dataGridViewCellStyle7;
            resources.ApplyResources(this.col_Left, "col_Left");
            this.col_Left.Name = "col_Left";
            this.col_Left.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // col_Right
            // 
            this.col_Right.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.NullValue = false;
            this.col_Right.DefaultCellStyle = dataGridViewCellStyle8;
            resources.ApplyResources(this.col_Right, "col_Right");
            this.col_Right.Name = "col_Right";
            this.col_Right.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // timer_PadResult
            // 
            this.timer_PadResult.Enabled = true;
            this.timer_PadResult.Interval = 10;
            // 
            // srmGroupBox5
            // 
            resources.ApplyResources(this.srmGroupBox5, "srmGroupBox5");
            this.srmGroupBox5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox5.Controls.Add(this.radioBtn_Middle);
            this.srmGroupBox5.Controls.Add(this.radioBtn_Down);
            this.srmGroupBox5.Controls.Add(this.radioBtn_Left);
            this.srmGroupBox5.Controls.Add(this.radioBtn_Up);
            this.srmGroupBox5.Controls.Add(this.radioBtn_Right);
            this.srmGroupBox5.Name = "srmGroupBox5";
            this.srmGroupBox5.TabStop = false;
            // 
            // radioBtn_Middle
            // 
            resources.ApplyResources(this.radioBtn_Middle, "radioBtn_Middle");
            this.radioBtn_Middle.Name = "radioBtn_Middle";
            this.radioBtn_Middle.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // radioBtn_Down
            // 
            resources.ApplyResources(this.radioBtn_Down, "radioBtn_Down");
            this.radioBtn_Down.Name = "radioBtn_Down";
            this.radioBtn_Down.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // radioBtn_Left
            // 
            resources.ApplyResources(this.radioBtn_Left, "radioBtn_Left");
            this.radioBtn_Left.Name = "radioBtn_Left";
            this.radioBtn_Left.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // radioBtn_Up
            // 
            resources.ApplyResources(this.radioBtn_Up, "radioBtn_Up");
            this.radioBtn_Up.Name = "radioBtn_Up";
            this.radioBtn_Up.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // radioBtn_Right
            // 
            resources.ApplyResources(this.radioBtn_Right, "radioBtn_Right");
            this.radioBtn_Right.Name = "radioBtn_Right";
            this.radioBtn_Right.Click += new System.EventHandler(this.radioBtn_PadIndex_Click);
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // chk_SetToAllRows
            // 
            resources.ApplyResources(this.chk_SetToAllRows, "chk_SetToAllRows");
            this.chk_SetToAllRows.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToAllRows.Name = "chk_SetToAllRows";
            this.chk_SetToAllRows.Selected = false;
            this.chk_SetToAllRows.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllRows.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllRows.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.chk_SetToAllRows);
            this.panel1.Controls.Add(this.chk_SetToAllSideROI);
            this.panel1.Controls.Add(this.btn_Save);
            this.panel1.Controls.Add(this.btn_Close);
            this.panel1.Controls.Add(this.srmGroupBox5);
            this.panel1.Name = "panel1";
            // 
            // chk_SetToAllSideROI
            // 
            resources.ApplyResources(this.chk_SetToAllSideROI, "chk_SetToAllSideROI");
            this.chk_SetToAllSideROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToAllSideROI.Name = "chk_SetToAllSideROI";
            this.chk_SetToAllSideROI.Selected = true;
            this.chk_SetToAllSideROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllSideROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllSideROI.UseVisualStyleBackColor = true;
            // 
            // PadStandOffSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgd_PadStandOffSetting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PadStandOffSettingForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PadStandOffSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.PadStandOffSettingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_PadStandOffSetting)).EndInit();
            this.srmGroupBox5.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgd_PadStandOffSetting;
        private System.Windows.Forms.Timer timer_PadResult;
        private SRMControl.SRMGroupBox srmGroupBox5;
        private SRMControl.SRMRadioButton radioBtn_Middle;
        private SRMControl.SRMRadioButton radioBtn_Down;
        private SRMControl.SRMRadioButton radioBtn_Left;
        private SRMControl.SRMRadioButton radioBtn_Up;
        private SRMControl.SRMRadioButton radioBtn_Right;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMCheckBox chk_SetToAllRows;
        private System.Windows.Forms.Panel panel1;
        private SRMControl.SRMCheckBox chk_SetToAllSideROI;
        private System.Windows.Forms.DataGridViewComboBoxColumn col_ReferTopBottom;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_Top;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_Bottom;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Separate;
        private System.Windows.Forms.DataGridViewComboBoxColumn col_ReferLeftRight;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_Left;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_Right;
    }
}