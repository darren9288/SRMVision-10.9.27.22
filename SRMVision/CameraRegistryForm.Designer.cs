namespace SRMVision
{
    partial class CameraRegistryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraRegistryForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbl_Title = new SRMControl.SRMLabel();
            this.lbl_SubTitle = new SRMControl.SRMLabel();
            this.lbl_Message1 = new SRMControl.SRMLabel();
            this.btn_Next = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.dgd_VisionList = new System.Windows.Forms.DataGridView();
            this.dc_VisionID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dc_CameraModel = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dc_camera = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dc_PortNo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dc_SerialNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dc_VisionName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dc_VisionDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_Message2 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.btn_Finish = new SRMControl.SRMButton();
            this.btn_Remove = new SRMControl.SRMButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_VisionList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            // lbl_SubTitle
            // 
            resources.ApplyResources(this.lbl_SubTitle, "lbl_SubTitle");
            this.lbl_SubTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbl_SubTitle.ForeColor = System.Drawing.Color.Gray;
            this.lbl_SubTitle.Name = "lbl_SubTitle";
            this.lbl_SubTitle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Message1
            // 
            resources.ApplyResources(this.lbl_Message1, "lbl_Message1");
            this.lbl_Message1.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Message1.ForeColor = System.Drawing.Color.Gray;
            this.lbl_Message1.Name = "lbl_Message1";
            this.lbl_Message1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Next
            // 
            resources.ApplyResources(this.btn_Next, "btn_Next");
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // dgd_VisionList
            // 
            this.dgd_VisionList.AllowUserToAddRows = false;
            this.dgd_VisionList.AllowUserToDeleteRows = false;
            this.dgd_VisionList.AllowUserToResizeColumns = false;
            this.dgd_VisionList.AllowUserToResizeRows = false;
            this.dgd_VisionList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgd_VisionList.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.dgd_VisionList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_VisionList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgd_VisionList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.dgd_VisionList, "dgd_VisionList");
            this.dgd_VisionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgd_VisionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dc_VisionID,
            this.dc_CameraModel,
            this.dc_camera,
            this.dc_PortNo,
            this.dc_SerialNo,
            this.dc_VisionName,
            this.dc_VisionDisplayName});
            this.dgd_VisionList.MultiSelect = false;
            this.dgd_VisionList.Name = "dgd_VisionList";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgd_VisionList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgd_VisionList.RowHeadersVisible = false;
            this.dgd_VisionList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dgd_VisionList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgd_VisionList.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_VisionList_CellEndEdit);
            this.dgd_VisionList.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgd_VisionList_DataError);
            // 
            // dc_VisionID
            // 
            this.dc_VisionID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.NullValue = "0";
            this.dc_VisionID.DefaultCellStyle = dataGridViewCellStyle2;
            this.dc_VisionID.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            resources.ApplyResources(this.dc_VisionID, "dc_VisionID");
            this.dc_VisionID.Name = "dc_VisionID";
            this.dc_VisionID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dc_CameraModel
            // 
            this.dc_CameraModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dc_CameraModel, "dc_CameraModel");
            this.dc_CameraModel.Name = "dc_CameraModel";
            this.dc_CameraModel.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dc_camera
            // 
            this.dc_camera.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dc_camera, "dc_camera");
            this.dc_camera.Items.AddRange(new object[] {
            "Mono",
            "Color"});
            this.dc_camera.Name = "dc_camera";
            this.dc_camera.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dc_PortNo
            // 
            this.dc_PortNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dc_PortNo, "dc_PortNo");
            this.dc_PortNo.Name = "dc_PortNo";
            this.dc_PortNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dc_SerialNo
            // 
            this.dc_SerialNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dc_SerialNo, "dc_SerialNo");
            this.dc_SerialNo.Name = "dc_SerialNo";
            this.dc_SerialNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dc_SerialNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dc_VisionName
            // 
            this.dc_VisionName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dc_VisionName, "dc_VisionName");
            this.dc_VisionName.Name = "dc_VisionName";
            this.dc_VisionName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dc_VisionDisplayName
            // 
            this.dc_VisionDisplayName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.dc_VisionDisplayName, "dc_VisionDisplayName");
            this.dc_VisionDisplayName.Name = "dc_VisionDisplayName";
            this.dc_VisionDisplayName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dc_VisionDisplayName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // lbl_Message2
            // 
            resources.ApplyResources(this.lbl_Message2, "lbl_Message2");
            this.lbl_Message2.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Message2.ForeColor = System.Drawing.Color.Gray;
            this.lbl_Message2.Name = "lbl_Message2";
            this.lbl_Message2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.BackColor = System.Drawing.Color.Transparent;
            this.srmLabel1.ForeColor = System.Drawing.Color.Gray;
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Finish
            // 
            resources.ApplyResources(this.btn_Finish, "btn_Finish");
            this.btn_Finish.Name = "btn_Finish";
            this.btn_Finish.UseVisualStyleBackColor = true;
            this.btn_Finish.Click += new System.EventHandler(this.btn_Finish_Click);
            // 
            // btn_Remove
            // 
            resources.ApplyResources(this.btn_Remove, "btn_Remove");
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.UseVisualStyleBackColor = true;
            this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
            // 
            // CameraRegistryForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgd_VisionList);
            this.Controls.Add(this.btn_Remove);
            this.Controls.Add(this.btn_Finish);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.lbl_Message2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.lbl_Message1);
            this.Controls.Add(this.lbl_SubTitle);
            this.Controls.Add(this.lbl_Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CameraRegistryForm";
            this.Load += new System.EventHandler(this.RegistryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_VisionList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel lbl_Title;
        private SRMControl.SRMLabel lbl_SubTitle;
        private SRMControl.SRMLabel lbl_Message1;
        private SRMControl.SRMButton btn_Next;
        private SRMControl.SRMButton btn_Cancel;
        private System.Windows.Forms.DataGridView dgd_VisionList;
        private System.Windows.Forms.PictureBox pictureBox1;
        private SRMControl.SRMLabel lbl_Message2;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMButton btn_Finish;
        private SRMControl.SRMButton btn_Remove;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_VisionID;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_CameraModel;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_camera;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_PortNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_SerialNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn dc_VisionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_VisionDisplayName;
    }
}