namespace AutoMode
{
    partial class LoadImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadImageForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_OK = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.cbo_ImageSelection = new SRMControl.SRMComboBox();
            this.cbo_ImageNoSelection = new SRMControl.SRMComboBox();
            this.btn_Browse = new SRMControl.SRMButton();
            this.dgd_ImageList = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ErrorMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dlg_ImageFile = new System.Windows.Forms.OpenFileDialog();
            this.pic_ImagePreview = new System.Windows.Forms.PictureBox();
            this.pic_ImagePreview2 = new System.Windows.Forms.PictureBox();
            this.pic_ImagePreview3 = new System.Windows.Forms.PictureBox();
            this.pic_ImagePreview4 = new System.Windows.Forms.PictureBox();
            this.pic_ImagePreview5 = new System.Windows.Forms.PictureBox();
            this.pic_ImagePreview6 = new System.Windows.Forms.PictureBox();
            this.lbl_ErrorMessge = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ImageList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview6)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // cbo_ImageSelection
            // 
            resources.ApplyResources(this.cbo_ImageSelection, "cbo_ImageSelection");
            this.cbo_ImageSelection.BackColor = System.Drawing.Color.White;
            this.cbo_ImageSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ImageSelection.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ImageSelection.Name = "cbo_ImageSelection";
            this.cbo_ImageSelection.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ImageSelection.SelectedIndexChanged += new System.EventHandler(this.cbo_ImageSelection_SelectedIndexChanged);
            // 
            // cbo_ImageNoSelection
            // 
            resources.ApplyResources(this.cbo_ImageNoSelection, "cbo_ImageNoSelection");
            this.cbo_ImageNoSelection.BackColor = System.Drawing.Color.White;
            this.cbo_ImageNoSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ImageNoSelection.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ImageNoSelection.Name = "cbo_ImageNoSelection";
            this.cbo_ImageNoSelection.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ImageNoSelection.SelectedIndexChanged += new System.EventHandler(this.cbo_ImageNoSelection_SelectedIndexChanged);
            // 
            // btn_Browse
            // 
            resources.ApplyResources(this.btn_Browse, "btn_Browse");
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // dgd_ImageList
            // 
            resources.ApplyResources(this.dgd_ImageList, "dgd_ImageList");
            this.dgd_ImageList.AllowUserToAddRows = false;
            this.dgd_ImageList.AllowUserToDeleteRows = false;
            this.dgd_ImageList.AllowUserToResizeColumns = false;
            this.dgd_ImageList.AllowUserToResizeRows = false;
            this.dgd_ImageList.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_ImageList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_ImageList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_ImageList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ImageList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgd_ImageList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_ImageList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn4,
            this.FilePath,
            this.ErrorMessage});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ImageList.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_ImageList.GridColor = System.Drawing.SystemColors.Control;
            this.dgd_ImageList.MultiSelect = false;
            this.dgd_ImageList.Name = "dgd_ImageList";
            this.dgd_ImageList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ImageList.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgd_ImageList.RowHeadersVisible = false;
            this.dgd_ImageList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgd_ImageList.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_ImageList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgd_ImageList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_ImageList_CellClick);
            this.dgd_ImageList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgd_ImageList_KeyDown);
            this.dgd_ImageList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgd_ImageList_KeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FilePath
            // 
            resources.ApplyResources(this.FilePath, "FilePath");
            this.FilePath.Name = "FilePath";
            this.FilePath.ReadOnly = true;
            this.FilePath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ErrorMessage
            // 
            resources.ApplyResources(this.ErrorMessage, "ErrorMessage");
            this.ErrorMessage.Name = "ErrorMessage";
            this.ErrorMessage.ReadOnly = true;
            this.ErrorMessage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dlg_ImageFile
            // 
            resources.ApplyResources(this.dlg_ImageFile, "dlg_ImageFile");
            // 
            // pic_ImagePreview
            // 
            resources.ApplyResources(this.pic_ImagePreview, "pic_ImagePreview");
            this.pic_ImagePreview.BackColor = System.Drawing.Color.Black;
            this.pic_ImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_ImagePreview.Name = "pic_ImagePreview";
            this.pic_ImagePreview.TabStop = false;
            // 
            // pic_ImagePreview2
            // 
            resources.ApplyResources(this.pic_ImagePreview2, "pic_ImagePreview2");
            this.pic_ImagePreview2.BackColor = System.Drawing.Color.Black;
            this.pic_ImagePreview2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_ImagePreview2.Name = "pic_ImagePreview2";
            this.pic_ImagePreview2.TabStop = false;
            // 
            // pic_ImagePreview3
            // 
            resources.ApplyResources(this.pic_ImagePreview3, "pic_ImagePreview3");
            this.pic_ImagePreview3.BackColor = System.Drawing.Color.Black;
            this.pic_ImagePreview3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_ImagePreview3.Name = "pic_ImagePreview3";
            this.pic_ImagePreview3.TabStop = false;
            // 
            // pic_ImagePreview4
            // 
            resources.ApplyResources(this.pic_ImagePreview4, "pic_ImagePreview4");
            this.pic_ImagePreview4.BackColor = System.Drawing.Color.Black;
            this.pic_ImagePreview4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_ImagePreview4.Name = "pic_ImagePreview4";
            this.pic_ImagePreview4.TabStop = false;
            // 
            // pic_ImagePreview5
            // 
            resources.ApplyResources(this.pic_ImagePreview5, "pic_ImagePreview5");
            this.pic_ImagePreview5.BackColor = System.Drawing.Color.Black;
            this.pic_ImagePreview5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_ImagePreview5.Name = "pic_ImagePreview5";
            this.pic_ImagePreview5.TabStop = false;
            // 
            // pic_ImagePreview6
            // 
            resources.ApplyResources(this.pic_ImagePreview6, "pic_ImagePreview6");
            this.pic_ImagePreview6.BackColor = System.Drawing.Color.Black;
            this.pic_ImagePreview6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_ImagePreview6.Name = "pic_ImagePreview6";
            this.pic_ImagePreview6.TabStop = false;
            // 
            // lbl_ErrorMessge
            // 
            resources.ApplyResources(this.lbl_ErrorMessge, "lbl_ErrorMessge");
            this.lbl_ErrorMessge.BackColor = System.Drawing.Color.White;
            this.lbl_ErrorMessge.ForeColor = System.Drawing.Color.Red;
            this.lbl_ErrorMessge.Name = "lbl_ErrorMessge";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lbl_ErrorMessge);
            this.panel1.Name = "panel1";
            // 
            // LoadImageForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pic_ImagePreview6);
            this.Controls.Add(this.pic_ImagePreview5);
            this.Controls.Add(this.pic_ImagePreview4);
            this.Controls.Add(this.pic_ImagePreview3);
            this.Controls.Add(this.pic_ImagePreview2);
            this.Controls.Add(this.dgd_ImageList);
            this.Controls.Add(this.btn_Browse);
            this.Controls.Add(this.cbo_ImageNoSelection);
            this.Controls.Add(this.cbo_ImageSelection);
            this.Controls.Add(this.pic_ImagePreview);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btn_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LoadImageForm";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ImageList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImagePreview6)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMComboBox cbo_ImageSelection;
        private SRMControl.SRMComboBox cbo_ImageNoSelection;
        private SRMControl.SRMButton btn_Browse;
        private System.Windows.Forms.DataGridView dgd_ImageList;
        private System.Windows.Forms.OpenFileDialog dlg_ImageFile;
        private System.Windows.Forms.PictureBox pic_ImagePreview;
        private System.Windows.Forms.PictureBox pic_ImagePreview2;
        private System.Windows.Forms.PictureBox pic_ImagePreview3;
        private System.Windows.Forms.PictureBox pic_ImagePreview4;
        private System.Windows.Forms.PictureBox pic_ImagePreview5;
        private System.Windows.Forms.PictureBox pic_ImagePreview6;
        private System.Windows.Forms.Label lbl_ErrorMessge;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrorMessage;
    }
}