namespace VisionProcessForm
{
    partial class PadInspectionAreaSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PadInspectionAreaSettingForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgd_MiddlePad = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_Close = new SRMControl.SRMButton();
            this.srmGroupBox5 = new SRMControl.SRMGroupBox();
            this.radioBtn_Middle = new SRMControl.SRMRadioButton();
            this.radioBtn_Down = new SRMControl.SRMRadioButton();
            this.radioBtn_Left = new SRMControl.SRMRadioButton();
            this.radioBtn_Up = new SRMControl.SRMRadioButton();
            this.radioBtn_Right = new SRMControl.SRMRadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_SetDescription = new SRMControl.SRMLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_MiddlePad)).BeginInit();
            this.panel1.SuspendLayout();
            this.srmGroupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgd_MiddlePad
            // 
            resources.ApplyResources(this.dgd_MiddlePad, "dgd_MiddlePad");
            this.dgd_MiddlePad.AllowUserToAddRows = false;
            this.dgd_MiddlePad.AllowUserToDeleteRows = false;
            this.dgd_MiddlePad.AllowUserToResizeColumns = false;
            this.dgd_MiddlePad.AllowUserToResizeRows = false;
            this.dgd_MiddlePad.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_MiddlePad.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_MiddlePad.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_MiddlePad.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_MiddlePad.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgd_MiddlePad.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_MiddlePad.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_MiddlePad.DefaultCellStyle = dataGridViewCellStyle14;
            this.dgd_MiddlePad.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_MiddlePad.MultiSelect = false;
            this.dgd_MiddlePad.Name = "dgd_MiddlePad";
            this.dgd_MiddlePad.ReadOnly = true;
            this.dgd_MiddlePad.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_MiddlePad.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dgd_MiddlePad.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_MiddlePad.RowsDefaultCellStyle = dataGridViewCellStyle16;
            this.dgd_MiddlePad.RowTemplate.Height = 24;
            this.dgd_MiddlePad.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_MiddlePad.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgd_MiddlePad.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_MiddlePad_CellDoubleClick);
            this.dgd_MiddlePad.ColumnHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgd_MiddlePad_ColumnHeaderMouseDoubleClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle10;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle11;
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle12;
            resources.ApplyResources(this.dataGridViewTextBoxColumn3, "dataGridViewTextBoxColumn3");
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle13;
            resources.ApplyResources(this.dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.btn_Save);
            this.panel1.Controls.Add(this.btn_Close);
            this.panel1.Controls.Add(this.srmGroupBox5);
            this.panel1.Name = "panel1";
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
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
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lbl_SetDescription
            // 
            resources.ApplyResources(this.lbl_SetDescription, "lbl_SetDescription");
            this.lbl_SetDescription.Name = "lbl_SetDescription";
            this.lbl_SetDescription.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // PadInspectionAreaSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.lbl_SetDescription);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgd_MiddlePad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PadInspectionAreaSettingForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PadInspectionAreaSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.PadInspectionAreaSettingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_MiddlePad)).EndInit();
            this.panel1.ResumeLayout(false);
            this.srmGroupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgd_MiddlePad;
        private System.Windows.Forms.Panel panel1;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMGroupBox srmGroupBox5;
        private SRMControl.SRMRadioButton radioBtn_Middle;
        private SRMControl.SRMRadioButton radioBtn_Down;
        private SRMControl.SRMRadioButton radioBtn_Left;
        private SRMControl.SRMRadioButton radioBtn_Up;
        private SRMControl.SRMRadioButton radioBtn_Right;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private SRMControl.SRMLabel lbl_SetDescription;
    }
}