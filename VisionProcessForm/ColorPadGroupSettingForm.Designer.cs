namespace VisionProcessForm
{
    partial class ColorPadGroupSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPadGroupSettingForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.srmLabel128 = new SRMControl.SRMLabel();
            this.srmLabel59 = new SRMControl.SRMLabel();
            this.srmLabel64 = new SRMControl.SRMLabel();
            this.srmLabel65 = new SRMControl.SRMLabel();
            this.srmLabel66 = new SRMControl.SRMLabel();
            this.cbo_Center = new SRMControl.SRMImageComboBox();
            this.cbo_Top = new SRMControl.SRMImageComboBox();
            this.cbo_Right = new SRMControl.SRMImageComboBox();
            this.cbo_Bottom = new SRMControl.SRMImageComboBox();
            this.cbo_Left = new SRMControl.SRMImageComboBox();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.dgd_Defect = new System.Windows.Forms.DataGridView();
            this.col_Center = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Top = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Right = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Bottom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Left = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_Defect)).BeginInit();
            this.SuspendLayout();
            // 
            // srmLabel128
            // 
            resources.ApplyResources(this.srmLabel128, "srmLabel128");
            this.srmLabel128.Name = "srmLabel128";
            this.srmLabel128.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel59
            // 
            resources.ApplyResources(this.srmLabel59, "srmLabel59");
            this.srmLabel59.Name = "srmLabel59";
            this.srmLabel59.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel64
            // 
            resources.ApplyResources(this.srmLabel64, "srmLabel64");
            this.srmLabel64.Name = "srmLabel64";
            this.srmLabel64.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel65
            // 
            resources.ApplyResources(this.srmLabel65, "srmLabel65");
            this.srmLabel65.Name = "srmLabel65";
            this.srmLabel65.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel66
            // 
            resources.ApplyResources(this.srmLabel66, "srmLabel66");
            this.srmLabel66.Name = "srmLabel66";
            this.srmLabel66.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_Center
            // 
            resources.ApplyResources(this.cbo_Center, "cbo_Center");
            this.cbo_Center.BackColor = System.Drawing.Color.White;
            this.cbo_Center.DisplayMember = "ItemData";
            this.cbo_Center.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Center.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Center.FormattingEnabled = true;
            this.cbo_Center.Name = "cbo_Center";
            this.cbo_Center.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Center.SelectedIndexChanged += new System.EventHandler(this.cbo_Center_SelectedIndexChanged);
            // 
            // cbo_Top
            // 
            resources.ApplyResources(this.cbo_Top, "cbo_Top");
            this.cbo_Top.BackColor = System.Drawing.Color.White;
            this.cbo_Top.DisplayMember = "ItemData";
            this.cbo_Top.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Top.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Top.FormattingEnabled = true;
            this.cbo_Top.Name = "cbo_Top";
            this.cbo_Top.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Top.SelectedIndexChanged += new System.EventHandler(this.cbo_Top_SelectedIndexChanged);
            // 
            // cbo_Right
            // 
            resources.ApplyResources(this.cbo_Right, "cbo_Right");
            this.cbo_Right.BackColor = System.Drawing.Color.White;
            this.cbo_Right.DisplayMember = "ItemData";
            this.cbo_Right.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Right.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Right.FormattingEnabled = true;
            this.cbo_Right.Name = "cbo_Right";
            this.cbo_Right.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Right.SelectedIndexChanged += new System.EventHandler(this.cbo_Right_SelectedIndexChanged);
            // 
            // cbo_Bottom
            // 
            resources.ApplyResources(this.cbo_Bottom, "cbo_Bottom");
            this.cbo_Bottom.BackColor = System.Drawing.Color.White;
            this.cbo_Bottom.DisplayMember = "ItemData";
            this.cbo_Bottom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Bottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Bottom.FormattingEnabled = true;
            this.cbo_Bottom.Name = "cbo_Bottom";
            this.cbo_Bottom.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Bottom.SelectedIndexChanged += new System.EventHandler(this.cbo_Bottom_SelectedIndexChanged);
            // 
            // cbo_Left
            // 
            resources.ApplyResources(this.cbo_Left, "cbo_Left");
            this.cbo_Left.BackColor = System.Drawing.Color.White;
            this.cbo_Left.DisplayMember = "ItemData";
            this.cbo_Left.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Left.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Left.FormattingEnabled = true;
            this.cbo_Left.Name = "cbo_Left";
            this.cbo_Left.NormalBackColor = System.Drawing.Color.White;
            this.cbo_Left.SelectedIndexChanged += new System.EventHandler(this.cbo_Left_SelectedIndexChanged);
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
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // dgd_Defect
            // 
            resources.ApplyResources(this.dgd_Defect, "dgd_Defect");
            this.dgd_Defect.AllowUserToAddRows = false;
            this.dgd_Defect.AllowUserToDeleteRows = false;
            this.dgd_Defect.AllowUserToResizeColumns = false;
            this.dgd_Defect.AllowUserToResizeRows = false;
            this.dgd_Defect.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_Defect.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_Defect.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_Defect.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Defect.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_Defect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_Defect.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Center,
            this.col_Top,
            this.col_Right,
            this.col_Bottom,
            this.col_Left});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Defect.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgd_Defect.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_Defect.MultiSelect = false;
            this.dgd_Defect.Name = "dgd_Defect";
            this.dgd_Defect.ReadOnly = true;
            this.dgd_Defect.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Defect.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgd_Defect.RowHeadersVisible = false;
            this.dgd_Defect.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_Defect.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgd_Defect.RowTemplate.Height = 24;
            this.dgd_Defect.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Defect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            // 
            // col_Center
            // 
            this.col_Center.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.col_Center.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.col_Center, "col_Center");
            this.col_Center.Name = "col_Center";
            this.col_Center.ReadOnly = true;
            this.col_Center.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_Top
            // 
            this.col_Top.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.col_Top.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.col_Top, "col_Top");
            this.col_Top.Name = "col_Top";
            this.col_Top.ReadOnly = true;
            this.col_Top.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_Right
            // 
            this.col_Right.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.col_Right.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.col_Right, "col_Right");
            this.col_Right.Name = "col_Right";
            this.col_Right.ReadOnly = true;
            this.col_Right.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_Bottom
            // 
            this.col_Bottom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.col_Bottom.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.col_Bottom, "col_Bottom");
            this.col_Bottom.Name = "col_Bottom";
            this.col_Bottom.ReadOnly = true;
            this.col_Bottom.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_Left
            // 
            this.col_Left.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.col_Left.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.col_Left, "col_Left");
            this.col_Left.Name = "col_Left";
            this.col_Left.ReadOnly = true;
            this.col_Left.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColorPadGroupSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.dgd_Defect);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.cbo_Left);
            this.Controls.Add(this.cbo_Bottom);
            this.Controls.Add(this.cbo_Right);
            this.Controls.Add(this.cbo_Top);
            this.Controls.Add(this.cbo_Center);
            this.Controls.Add(this.srmLabel128);
            this.Controls.Add(this.srmLabel59);
            this.Controls.Add(this.srmLabel64);
            this.Controls.Add(this.srmLabel65);
            this.Controls.Add(this.srmLabel66);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorPadGroupSettingForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorPadGroupSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.ColorPadGroupSettingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_Defect)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMLabel srmLabel128;
        private SRMControl.SRMLabel srmLabel59;
        private SRMControl.SRMLabel srmLabel64;
        private SRMControl.SRMLabel srmLabel65;
        private SRMControl.SRMLabel srmLabel66;
        private SRMControl.SRMImageComboBox cbo_Center;
        private SRMControl.SRMImageComboBox cbo_Top;
        private SRMControl.SRMImageComboBox cbo_Right;
        private SRMControl.SRMImageComboBox cbo_Bottom;
        private SRMControl.SRMImageComboBox cbo_Left;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.DataGridView dgd_Defect;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Center;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Top;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Right;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Bottom;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Left;
    }
}