namespace VisionProcessForm
{
    partial class Lead3DSkewInspectOptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Lead3DSkewInspectOptionForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgd_MiddleLead = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_Close = new SRMControl.SRMButton();
            this.chk_SetToAllRows = new SRMControl.SRMCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_MiddleLead)).BeginInit();
            this.SuspendLayout();
            // 
            // dgd_MiddleLead
            // 
            resources.ApplyResources(this.dgd_MiddleLead, "dgd_MiddleLead");
            this.dgd_MiddleLead.AllowUserToAddRows = false;
            this.dgd_MiddleLead.AllowUserToDeleteRows = false;
            this.dgd_MiddleLead.AllowUserToResizeColumns = false;
            this.dgd_MiddleLead.AllowUserToResizeRows = false;
            this.dgd_MiddleLead.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_MiddleLead.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_MiddleLead.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_MiddleLead.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_MiddleLead.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_MiddleLead.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_MiddleLead.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_MiddleLead.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgd_MiddleLead.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_MiddleLead.MultiSelect = false;
            this.dgd_MiddleLead.Name = "dgd_MiddleLead";
            this.dgd_MiddleLead.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_MiddleLead.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_MiddleLead.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_MiddleLead.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgd_MiddleLead.RowTemplate.Height = 24;
            this.dgd_MiddleLead.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_MiddleLead.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgd_MiddleLead.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_MiddleLead_CellContentClick);
            this.dgd_MiddleLead.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_MiddleLead_CellContentDoubleClick);
            this.dgd_MiddleLead.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_MiddleLead_CellValueChanged);
            this.dgd_MiddleLead.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgd_MiddleLead_CurrentCellDirtyStateChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.NullValue = false;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
            // Lead3DSkewInspectOptionForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.chk_SetToAllRows);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.dgd_MiddleLead);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Lead3DSkewInspectOptionForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dgd_MiddleLead)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgd_MiddleLead;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMCheckBox chk_SetToAllRows;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewTextBoxColumn1;
    }
}