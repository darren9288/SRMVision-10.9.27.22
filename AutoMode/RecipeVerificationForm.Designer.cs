namespace AutoMode
{
    partial class RecipeVerificationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecipeVerificationForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle46 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle47 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle52 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle53 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle54 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle48 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle49 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle50 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle51 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_ContinueProduction = new SRMControl.SRMButton();
            this.btn_CancelProduction = new SRMControl.SRMButton();
            this.cbo_VisionModule = new SRMControl.SRMComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgd_Result = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Separator1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Separator2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.pnl_Details = new System.Windows.Forms.Panel();
            this.cbo_ViewImage = new SRMControl.SRMComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_HideDetails = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lst_Details = new System.Windows.Forms.ListBox();
            this.lbl_FinalResult = new System.Windows.Forms.Label();
            this.lbl_VerificationResult = new System.Windows.Forms.Label();
            this.lbl_ExpectedResult = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_ImageName = new System.Windows.Forms.Label();
            this.pnl_Image = new System.Windows.Forms.Panel();
            this.pic_Image = new System.Windows.Forms.PictureBox();
            this.btn_Prev = new SRMControl.SRMButton();
            this.btn_Next = new SRMControl.SRMButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_Result)).BeginInit();
            this.pnl_Details.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnl_Image.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_ContinueProduction
            // 
            resources.ApplyResources(this.btn_ContinueProduction, "btn_ContinueProduction");
            this.btn_ContinueProduction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ContinueProduction.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ContinueProduction.Name = "btn_ContinueProduction";
            // 
            // btn_CancelProduction
            // 
            resources.ApplyResources(this.btn_CancelProduction, "btn_CancelProduction");
            this.btn_CancelProduction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CancelProduction.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_CancelProduction.Name = "btn_CancelProduction";
            // 
            // cbo_VisionModule
            // 
            this.cbo_VisionModule.BackColor = System.Drawing.Color.White;
            this.cbo_VisionModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_VisionModule.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.cbo_VisionModule, "cbo_VisionModule");
            this.cbo_VisionModule.FormattingEnabled = true;
            this.cbo_VisionModule.Name = "cbo_VisionModule";
            this.cbo_VisionModule.NormalBackColor = System.Drawing.Color.White;
            this.cbo_VisionModule.SelectedIndexChanged += new System.EventHandler(this.cbo_VisionModule_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dgd_Result
            // 
            this.dgd_Result.AllowUserToAddRows = false;
            this.dgd_Result.AllowUserToDeleteRows = false;
            this.dgd_Result.AllowUserToResizeColumns = false;
            this.dgd_Result.AllowUserToResizeRows = false;
            dataGridViewCellStyle46.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgd_Result.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle46;
            this.dgd_Result.BackgroundColor = System.Drawing.Color.LightSteelBlue;
            this.dgd_Result.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgd_Result.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgd_Result.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle47.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle47.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle47.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle47.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle47.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle47.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle47.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Result.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle47;
            this.dgd_Result.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_Result.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column4,
            this.col1,
            this.col_Separator1,
            this.col2,
            this.col_Separator2,
            this.col3,
            this.Column7,
            this.Column8});
            dataGridViewCellStyle52.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle52.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle52.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle52.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle52.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle52.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle52.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Result.DefaultCellStyle = dataGridViewCellStyle52;
            this.dgd_Result.GridColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.dgd_Result, "dgd_Result");
            this.dgd_Result.MultiSelect = false;
            this.dgd_Result.Name = "dgd_Result";
            this.dgd_Result.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle53.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle53.BackColor = System.Drawing.Color.LightYellow;
            dataGridViewCellStyle53.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle53.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle53.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle53.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle53.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_Result.RowHeadersDefaultCellStyle = dataGridViewCellStyle53;
            this.dgd_Result.RowHeadersVisible = false;
            this.dgd_Result.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle54.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgd_Result.RowsDefaultCellStyle = dataGridViewCellStyle54;
            this.dgd_Result.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgd_Result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgd_Result.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_Result_CellContentClick);
            // 
            // Column3
            // 
            resources.ApplyResources(this.Column3, "Column3");
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column4
            // 
            dataGridViewCellStyle48.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle48.SelectionBackColor = System.Drawing.Color.DimGray;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle48;
            resources.ApplyResources(this.Column4, "Column4");
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col1
            // 
            resources.ApplyResources(this.col1, "col1");
            this.col1.Name = "col1";
            this.col1.ReadOnly = true;
            this.col1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_Separator1
            // 
            dataGridViewCellStyle49.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle49.SelectionBackColor = System.Drawing.Color.DimGray;
            this.col_Separator1.DefaultCellStyle = dataGridViewCellStyle49;
            resources.ApplyResources(this.col_Separator1, "col_Separator1");
            this.col_Separator1.Name = "col_Separator1";
            this.col_Separator1.ReadOnly = true;
            this.col_Separator1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Separator1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col2
            // 
            resources.ApplyResources(this.col2, "col2");
            this.col2.Name = "col2";
            this.col2.ReadOnly = true;
            this.col2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_Separator2
            // 
            dataGridViewCellStyle50.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle50.SelectionBackColor = System.Drawing.Color.DimGray;
            this.col_Separator2.DefaultCellStyle = dataGridViewCellStyle50;
            resources.ApplyResources(this.col_Separator2, "col_Separator2");
            this.col_Separator2.Name = "col_Separator2";
            this.col_Separator2.ReadOnly = true;
            this.col_Separator2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Separator2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col3
            // 
            resources.ApplyResources(this.col3, "col3");
            this.col3.Name = "col3";
            this.col3.ReadOnly = true;
            this.col3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column7
            // 
            dataGridViewCellStyle51.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle51.SelectionBackColor = System.Drawing.Color.DimGray;
            this.Column7.DefaultCellStyle = dataGridViewCellStyle51;
            resources.ApplyResources(this.Column7, "Column7");
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column8
            // 
            resources.ApplyResources(this.Column8, "Column8");
            this.Column8.Name = "Column8";
            this.Column8.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column8.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // pnl_Details
            // 
            this.pnl_Details.Controls.Add(this.cbo_ViewImage);
            this.pnl_Details.Controls.Add(this.panel3);
            this.pnl_Details.Controls.Add(this.label9);
            this.pnl_Details.Controls.Add(this.lst_Details);
            this.pnl_Details.Controls.Add(this.lbl_FinalResult);
            this.pnl_Details.Controls.Add(this.lbl_VerificationResult);
            this.pnl_Details.Controls.Add(this.lbl_ExpectedResult);
            this.pnl_Details.Controls.Add(this.label6);
            this.pnl_Details.Controls.Add(this.label5);
            this.pnl_Details.Controls.Add(this.label4);
            this.pnl_Details.Controls.Add(this.lbl_ImageName);
            this.pnl_Details.Controls.Add(this.pnl_Image);
            this.pnl_Details.Controls.Add(this.btn_Prev);
            this.pnl_Details.Controls.Add(this.btn_Next);
            resources.ApplyResources(this.pnl_Details, "pnl_Details");
            this.pnl_Details.Name = "pnl_Details";
            // 
            // cbo_ViewImage
            // 
            this.cbo_ViewImage.BackColor = System.Drawing.Color.White;
            this.cbo_ViewImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ViewImage.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.cbo_ViewImage, "cbo_ViewImage");
            this.cbo_ViewImage.FormattingEnabled = true;
            this.cbo_ViewImage.Name = "cbo_ViewImage";
            this.cbo_ViewImage.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ViewImage.SelectedIndexChanged += new System.EventHandler(this.cbo_ViewImage_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btn_HideDetails);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // btn_HideDetails
            // 
            this.btn_HideDetails.BackColor = System.Drawing.Color.Red;
            this.btn_HideDetails.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btn_HideDetails, "btn_HideDetails");
            this.btn_HideDetails.ForeColor = System.Drawing.Color.White;
            this.btn_HideDetails.Name = "btn_HideDetails";
            this.btn_HideDetails.UseVisualStyleBackColor = false;
            this.btn_HideDetails.Click += new System.EventHandler(this.btn_HideDetails_Click);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // lst_Details
            // 
            this.lst_Details.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lst_Details.FormattingEnabled = true;
            resources.ApplyResources(this.lst_Details, "lst_Details");
            this.lst_Details.Items.AddRange(new object[] {
            resources.GetString("lst_Details.Items")});
            this.lst_Details.Name = "lst_Details";
            this.lst_Details.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lst_Details_DrawItem);
            // 
            // lbl_FinalResult
            // 
            resources.ApplyResources(this.lbl_FinalResult, "lbl_FinalResult");
            this.lbl_FinalResult.Name = "lbl_FinalResult";
            // 
            // lbl_VerificationResult
            // 
            resources.ApplyResources(this.lbl_VerificationResult, "lbl_VerificationResult");
            this.lbl_VerificationResult.Name = "lbl_VerificationResult";
            // 
            // lbl_ExpectedResult
            // 
            resources.ApplyResources(this.lbl_ExpectedResult, "lbl_ExpectedResult");
            this.lbl_ExpectedResult.Name = "lbl_ExpectedResult";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lbl_ImageName
            // 
            resources.ApplyResources(this.lbl_ImageName, "lbl_ImageName");
            this.lbl_ImageName.Name = "lbl_ImageName";
            // 
            // pnl_Image
            // 
            this.pnl_Image.Controls.Add(this.pic_Image);
            resources.ApplyResources(this.pnl_Image, "pnl_Image");
            this.pnl_Image.Name = "pnl_Image";
            // 
            // pic_Image
            // 
            this.pic_Image.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.pic_Image, "pic_Image");
            this.pic_Image.Name = "pic_Image";
            this.pic_Image.TabStop = false;
            // 
            // btn_Prev
            // 
            resources.ApplyResources(this.btn_Prev, "btn_Prev");
            this.btn_Prev.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Prev.Name = "btn_Prev";
            this.btn_Prev.Click += new System.EventHandler(this.btn_Prev_Click);
            // 
            // btn_Next
            // 
            resources.ApplyResources(this.btn_Next, "btn_Next");
            this.btn_Next.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // RecipeVerificationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.pnl_Details);
            this.Controls.Add(this.dgd_Result);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbo_VisionModule);
            this.Controls.Add(this.btn_ContinueProduction);
            this.Controls.Add(this.btn_CancelProduction);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecipeVerificationForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dgd_Result)).EndInit();
            this.pnl_Details.ResumeLayout(false);
            this.pnl_Details.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.pnl_Image.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_ContinueProduction;
        private SRMControl.SRMButton btn_CancelProduction;
        private SRMControl.SRMComboBox cbo_VisionModule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgd_Result;
        private System.Windows.Forms.Panel pnl_Details;
        private System.Windows.Forms.Panel pnl_Image;
        private System.Windows.Forms.PictureBox pic_Image;
        private SRMControl.SRMButton btn_Prev;
        private System.Windows.Forms.Label lbl_FinalResult;
        private System.Windows.Forms.Label lbl_VerificationResult;
        private System.Windows.Forms.Label lbl_ExpectedResult;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbl_ImageName;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_HideDetails;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox lst_Details;
        private SRMControl.SRMButton btn_Next;
        private SRMControl.SRMComboBox cbo_ViewImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn col1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Separator1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col2;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Separator2;
        private System.Windows.Forms.DataGridViewTextBoxColumn col3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewLinkColumn Column8;
        private System.Windows.Forms.Timer timer1;
    }
}