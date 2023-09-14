namespace SRMVision
{
    partial class LightChannelRegistryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LightChannelRegistryForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbl_Title = new SRMControl.SRMLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dgd_VisionList = new System.Windows.Forms.DataGridView();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_Remove = new SRMControl.SRMButton();
            this.btn_Add = new SRMControl.SRMButton();
            this.dc_PortName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dc_VisionID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dc_Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dc_LightChannel = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TypeNameNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_VisionList)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            resources.ApplyResources(this.lbl_Title, "lbl_Title");
            this.lbl_Title.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Title.ForeColor = System.Drawing.Color.Navy;
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // dgd_VisionList
            // 
            resources.ApplyResources(this.dgd_VisionList, "dgd_VisionList");
            this.dgd_VisionList.AllowUserToAddRows = false;
            this.dgd_VisionList.AllowUserToResizeColumns = false;
            this.dgd_VisionList.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            this.dgd_VisionList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgd_VisionList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgd_VisionList.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.dgd_VisionList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_VisionList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_VisionList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Verdana", 9.75F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgd_VisionList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgd_VisionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_VisionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dc_PortName,
            this.dc_VisionID,
            this.dc_Type,
            this.dc_LightChannel,
            this.TypeNameNum});
            this.dgd_VisionList.MultiSelect = false;
            this.dgd_VisionList.Name = "dgd_VisionList";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Verdana", 9.75F);
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgd_VisionList.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgd_VisionList.RowHeadersVisible = false;
            this.dgd_VisionList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dgd_VisionList.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Aqua;
            this.dgd_VisionList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgd_VisionList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_VisionList_CellClick);
            this.dgd_VisionList.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_VisionList_CellEndEdit);
            this.dgd_VisionList.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_VisionList_CellLeave);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Remove
            // 
            resources.ApplyResources(this.btn_Remove, "btn_Remove");
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.UseVisualStyleBackColor = true;
            this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
            // 
            // btn_Add
            // 
            resources.ApplyResources(this.btn_Add, "btn_Add");
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // dc_PortName
            // 
            this.dc_PortName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dc_PortName.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            resources.ApplyResources(this.dc_PortName, "dc_PortName");
            this.dc_PortName.Name = "dc_PortName";
            this.dc_PortName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dc_VisionID
            // 
            this.dc_VisionID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.NullValue = "0";
            this.dc_VisionID.DefaultCellStyle = dataGridViewCellStyle8;
            this.dc_VisionID.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            resources.ApplyResources(this.dc_VisionID, "dc_VisionID");
            this.dc_VisionID.Name = "dc_VisionID";
            this.dc_VisionID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dc_Type
            // 
            this.dc_Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dc_Type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            resources.ApplyResources(this.dc_Type, "dc_Type");
            this.dc_Type.Name = "dc_Type";
            this.dc_Type.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dc_LightChannel
            // 
            this.dc_LightChannel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle9.NullValue = "0";
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.dc_LightChannel.DefaultCellStyle = dataGridViewCellStyle9;
            this.dc_LightChannel.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            resources.ApplyResources(this.dc_LightChannel, "dc_LightChannel");
            this.dc_LightChannel.Name = "dc_LightChannel";
            this.dc_LightChannel.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // TypeNameNum
            // 
            resources.ApplyResources(this.TypeNameNum, "TypeNameNum");
            this.TypeNameNum.Name = "TypeNameNum";
            this.TypeNameNum.ReadOnly = true;
            // 
            // LightChannelRegistryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.btn_Remove);
            this.Controls.Add(this.lbl_Title);
            this.Controls.Add(this.dgd_VisionList);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LightChannelRegistryForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_VisionList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel lbl_Title;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridView dgd_VisionList;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMButton btn_Remove;
        private SRMControl.SRMButton btn_Add;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_PortName;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_VisionID;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_Type;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_LightChannel;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeNameNum;
    }
}