namespace VisionModule
{
    partial class Vision2OfflinePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vision2OfflinePage));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle29 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle30 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle31 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle35 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle36 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle28 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle32 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle33 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle34 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbl_GrabDelay = new System.Windows.Forms.Label();
            this.lbl_TotalTime = new SRMControl.SRMLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_V2Grab = new System.Windows.Forms.Label();
            this.lbl_V2Process = new System.Windows.Forms.Label();
            this.lbl_Pin1ROI = new SRMControl.SRMLabel();
            this.lbl_GrabTime = new SRMControl.SRMLabel();
            this.lbl_TestResultIndicatorUnit1 = new SRMControl.SRMLabel();
            this.lbl_ProcessTime = new SRMControl.SRMLabel();
            this.chk_Grab = new SRMControl.SRMCheckBox();
            this.btn_Inspect = new SRMControl.SRMButton();
            this.btn_Close = new SRMControl.SRMButton();
            this.tab_VisionControl = new SRMControl.SRMTabControl();
            this.tabPage_UnitPresent = new System.Windows.Forms.TabPage();
            this.dgd_UnitPresent = new System.Windows.Forms.DataGridView();
            this.tabPage_UnitOffSet = new System.Windows.Forms.TabPage();
            this.dgd_UnitOffSet = new System.Windows.Forms.DataGridView();
            this.imageList_AutoForm = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.column_No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_AreaResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_UnitPresentDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tab_VisionControl.SuspendLayout();
            this.tabPage_UnitPresent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_UnitPresent)).BeginInit();
            this.tabPage_UnitOffSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_UnitOffSet)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_GrabDelay
            // 
            resources.ApplyResources(this.lbl_GrabDelay, "lbl_GrabDelay");
            this.lbl_GrabDelay.BackColor = System.Drawing.Color.White;
            this.lbl_GrabDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrabDelay.Name = "lbl_GrabDelay";
            // 
            // lbl_TotalTime
            // 
            resources.ApplyResources(this.lbl_TotalTime, "lbl_TotalTime");
            this.lbl_TotalTime.BackColor = System.Drawing.Color.White;
            this.lbl_TotalTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_TotalTime.Name = "lbl_TotalTime";
            this.lbl_TotalTime.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Name = "label1";
            // 
            // lbl_V2Grab
            // 
            resources.ApplyResources(this.lbl_V2Grab, "lbl_V2Grab");
            this.lbl_V2Grab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_V2Grab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_V2Grab.Name = "lbl_V2Grab";
            // 
            // lbl_V2Process
            // 
            resources.ApplyResources(this.lbl_V2Process, "lbl_V2Process");
            this.lbl_V2Process.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_V2Process.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_V2Process.Name = "lbl_V2Process";
            // 
            // lbl_Pin1ROI
            // 
            resources.ApplyResources(this.lbl_Pin1ROI, "lbl_Pin1ROI");
            this.lbl_Pin1ROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lbl_Pin1ROI.ForeColor = System.Drawing.Color.White;
            this.lbl_Pin1ROI.Name = "lbl_Pin1ROI";
            this.lbl_Pin1ROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_GrabTime
            // 
            resources.ApplyResources(this.lbl_GrabTime, "lbl_GrabTime");
            this.lbl_GrabTime.BackColor = System.Drawing.Color.White;
            this.lbl_GrabTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrabTime.Name = "lbl_GrabTime";
            this.lbl_GrabTime.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TestResultIndicatorUnit1
            // 
            resources.ApplyResources(this.lbl_TestResultIndicatorUnit1, "lbl_TestResultIndicatorUnit1");
            this.lbl_TestResultIndicatorUnit1.BackColor = System.Drawing.Color.Lime;
            this.lbl_TestResultIndicatorUnit1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_TestResultIndicatorUnit1.Name = "lbl_TestResultIndicatorUnit1";
            this.lbl_TestResultIndicatorUnit1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ProcessTime
            // 
            resources.ApplyResources(this.lbl_ProcessTime, "lbl_ProcessTime");
            this.lbl_ProcessTime.BackColor = System.Drawing.Color.White;
            this.lbl_ProcessTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ProcessTime.Name = "lbl_ProcessTime";
            this.lbl_ProcessTime.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_Grab
            // 
            resources.ApplyResources(this.chk_Grab, "chk_Grab");
            this.chk_Grab.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Grab.Name = "chk_Grab";
            this.chk_Grab.Selected = false;
            this.chk_Grab.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Grab.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Grab.UseVisualStyleBackColor = true;
            // 
            // btn_Inspect
            // 
            resources.ApplyResources(this.btn_Inspect, "btn_Inspect");
            this.btn_Inspect.Name = "btn_Inspect";
            this.btn_Inspect.UseVisualStyleBackColor = true;
            this.btn_Inspect.Click += new System.EventHandler(this.btn_Inspect_Click);
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // tab_VisionControl
            // 
            resources.ApplyResources(this.tab_VisionControl, "tab_VisionControl");
            this.tab_VisionControl.Controls.Add(this.tabPage_UnitPresent);
            this.tab_VisionControl.Controls.Add(this.tabPage_UnitOffSet);
            this.tab_VisionControl.ImageList = this.imageList_AutoForm;
            this.tab_VisionControl.Name = "tab_VisionControl";
            this.tab_VisionControl.SelectedIndex = 0;
            // 
            // tabPage_UnitPresent
            // 
            resources.ApplyResources(this.tabPage_UnitPresent, "tabPage_UnitPresent");
            this.tabPage_UnitPresent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tabPage_UnitPresent.Controls.Add(this.dgd_UnitPresent);
            this.tabPage_UnitPresent.Name = "tabPage_UnitPresent";
            // 
            // dgd_UnitPresent
            // 
            resources.ApplyResources(this.dgd_UnitPresent, "dgd_UnitPresent");
            this.dgd_UnitPresent.AllowUserToAddRows = false;
            this.dgd_UnitPresent.AllowUserToDeleteRows = false;
            this.dgd_UnitPresent.AllowUserToResizeColumns = false;
            this.dgd_UnitPresent.AllowUserToResizeRows = false;
            this.dgd_UnitPresent.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_UnitPresent.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_UnitPresent.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_UnitPresent.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle25.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle25.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle25.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle25.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle25.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_UnitPresent.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle25;
            this.dgd_UnitPresent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_UnitPresent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column_No,
            this.column_AreaResult,
            this.column_UnitPresentDesc});
            dataGridViewCellStyle29.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle29.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle29.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle29.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle29.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle29.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle29.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_UnitPresent.DefaultCellStyle = dataGridViewCellStyle29;
            this.dgd_UnitPresent.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_UnitPresent.MultiSelect = false;
            this.dgd_UnitPresent.Name = "dgd_UnitPresent";
            this.dgd_UnitPresent.ReadOnly = true;
            this.dgd_UnitPresent.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle30.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle30.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle30.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle30.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle30.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle30.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle30.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_UnitPresent.RowHeadersDefaultCellStyle = dataGridViewCellStyle30;
            this.dgd_UnitPresent.RowHeadersVisible = false;
            this.dgd_UnitPresent.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgd_UnitPresent.RowTemplate.Height = 24;
            this.dgd_UnitPresent.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_UnitPresent.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // tabPage_UnitOffSet
            // 
            resources.ApplyResources(this.tabPage_UnitOffSet, "tabPage_UnitOffSet");
            this.tabPage_UnitOffSet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tabPage_UnitOffSet.Controls.Add(this.dgd_UnitOffSet);
            this.tabPage_UnitOffSet.Name = "tabPage_UnitOffSet";
            // 
            // dgd_UnitOffSet
            // 
            resources.ApplyResources(this.dgd_UnitOffSet, "dgd_UnitOffSet");
            this.dgd_UnitOffSet.AllowUserToAddRows = false;
            this.dgd_UnitOffSet.AllowUserToDeleteRows = false;
            this.dgd_UnitOffSet.AllowUserToResizeColumns = false;
            this.dgd_UnitOffSet.AllowUserToResizeRows = false;
            this.dgd_UnitOffSet.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_UnitOffSet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_UnitOffSet.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_UnitOffSet.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle31.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle31.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle31.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle31.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle31.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle31.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle31.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_UnitOffSet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle31;
            this.dgd_UnitOffSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_UnitOffSet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn4});
            dataGridViewCellStyle35.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle35.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle35.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle35.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle35.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle35.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle35.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_UnitOffSet.DefaultCellStyle = dataGridViewCellStyle35;
            this.dgd_UnitOffSet.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_UnitOffSet.MultiSelect = false;
            this.dgd_UnitOffSet.Name = "dgd_UnitOffSet";
            this.dgd_UnitOffSet.ReadOnly = true;
            this.dgd_UnitOffSet.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle36.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle36.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle36.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle36.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle36.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle36.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle36.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_UnitOffSet.RowHeadersDefaultCellStyle = dataGridViewCellStyle36;
            this.dgd_UnitOffSet.RowHeadersVisible = false;
            this.dgd_UnitOffSet.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgd_UnitOffSet.RowTemplate.Height = 24;
            this.dgd_UnitOffSet.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_UnitOffSet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // imageList_AutoForm
            // 
            this.imageList_AutoForm.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_AutoForm.ImageStream")));
            this.imageList_AutoForm.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_AutoForm.Images.SetKeyName(0, "input_on.png");
            this.imageList_AutoForm.Images.SetKeyName(1, "input_off.bmp");
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // column_No
            // 
            this.column_No.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle26.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle26.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.column_No.DefaultCellStyle = dataGridViewCellStyle26;
            resources.ApplyResources(this.column_No, "column_No");
            this.column_No.Name = "column_No";
            this.column_No.ReadOnly = true;
            this.column_No.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_AreaResult
            // 
            this.column_AreaResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle27.BackColor = System.Drawing.Color.Lime;
            dataGridViewCellStyle27.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.column_AreaResult.DefaultCellStyle = dataGridViewCellStyle27;
            this.column_AreaResult.FillWeight = 50F;
            resources.ApplyResources(this.column_AreaResult, "column_AreaResult");
            this.column_AreaResult.Name = "column_AreaResult";
            this.column_AreaResult.ReadOnly = true;
            this.column_AreaResult.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.column_AreaResult.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // column_UnitPresentDesc
            // 
            this.column_UnitPresentDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle28.BackColor = System.Drawing.Color.Lime;
            this.column_UnitPresentDesc.DefaultCellStyle = dataGridViewCellStyle28;
            resources.ApplyResources(this.column_UnitPresentDesc, "column_UnitPresentDesc");
            this.column_UnitPresentDesc.Name = "column_UnitPresentDesc";
            this.column_UnitPresentDesc.ReadOnly = true;
            this.column_UnitPresentDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.column_UnitPresentDesc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle32.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle32.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle32;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle33.BackColor = System.Drawing.Color.Lime;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle33;
            resources.ApplyResources(this.dataGridViewTextBoxColumn6, "dataGridViewTextBoxColumn6");
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle34.BackColor = System.Drawing.Color.Lime;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle34;
            resources.ApplyResources(this.dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Vision2OfflinePage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.tab_VisionControl);
            this.Controls.Add(this.chk_Grab);
            this.Controls.Add(this.btn_Inspect);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.lbl_GrabDelay);
            this.Controls.Add(this.lbl_TotalTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_V2Grab);
            this.Controls.Add(this.lbl_V2Process);
            this.Controls.Add(this.lbl_Pin1ROI);
            this.Controls.Add(this.lbl_GrabTime);
            this.Controls.Add(this.lbl_TestResultIndicatorUnit1);
            this.Controls.Add(this.lbl_ProcessTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Vision2OfflinePage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Vision2OfflinePage_FormClosing);
            this.Load += new System.EventHandler(this.Vision2OfflinePage_Load);
            this.tab_VisionControl.ResumeLayout(false);
            this.tabPage_UnitPresent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_UnitPresent)).EndInit();
            this.tabPage_UnitOffSet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_UnitOffSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl_GrabDelay;
        private SRMControl.SRMLabel lbl_TotalTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_V2Grab;
        private System.Windows.Forms.Label lbl_V2Process;
        private SRMControl.SRMLabel lbl_Pin1ROI;
        private SRMControl.SRMLabel lbl_GrabTime;
        private SRMControl.SRMLabel lbl_TestResultIndicatorUnit1;
        private SRMControl.SRMLabel lbl_ProcessTime;
        private SRMControl.SRMCheckBox chk_Grab;
        private SRMControl.SRMButton btn_Inspect;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMTabControl tab_VisionControl;
        private System.Windows.Forms.TabPage tabPage_UnitPresent;
        private System.Windows.Forms.DataGridView dgd_UnitPresent;
        private System.Windows.Forms.TabPage tabPage_UnitOffSet;
        private System.Windows.Forms.DataGridView dgd_UnitOffSet;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ImageList imageList_AutoForm;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_No;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_AreaResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_UnitPresentDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    }
}