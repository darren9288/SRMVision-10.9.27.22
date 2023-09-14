namespace User
{
    partial class UserManagerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserManagerForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.CloseButton = new SRMControl.SRMButton();
            this.UserRightButton = new SRMControl.SRMButton();
            this.NewUserButton = new SRMControl.SRMButton();
            this.GroupGridView = new System.Windows.Forms.DataGridView();
            this.UserGridView = new System.Windows.Forms.DataGridView();
            this.btn_LoadUserRight = new SRMControl.SRMButton();
            this.UserNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FullNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupNoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.GroupGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // UserRightButton
            // 
            resources.ApplyResources(this.UserRightButton, "UserRightButton");
            this.UserRightButton.Name = "UserRightButton";
            this.UserRightButton.Click += new System.EventHandler(this.UserRightButton_Click);
            // 
            // NewUserButton
            // 
            resources.ApplyResources(this.NewUserButton, "NewUserButton");
            this.NewUserButton.Name = "NewUserButton";
            this.NewUserButton.Click += new System.EventHandler(this.NewUserButton_Click);
            // 
            // GroupGridView
            // 
            resources.ApplyResources(this.GroupGridView, "GroupGridView");
            this.GroupGridView.AllowUserToAddRows = false;
            this.GroupGridView.AllowUserToDeleteRows = false;
            this.GroupGridView.AllowUserToResizeColumns = false;
            this.GroupGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.GroupGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.GroupGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.GroupGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GroupGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.GroupGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.GroupGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.GroupNoColumn,
            this.GroupDescriptionColumn});
            this.GroupGridView.MultiSelect = false;
            this.GroupGridView.Name = "GroupGridView";
            this.GroupGridView.RowHeadersVisible = false;
            this.GroupGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.AliceBlue;
            this.GroupGridView.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.GroupGridView.RowTemplate.Height = 24;
            this.GroupGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GroupGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GroupGridView_CellDoubleClick);
            // 
            // UserGridView
            // 
            resources.ApplyResources(this.UserGridView, "UserGridView");
            this.UserGridView.AllowUserToAddRows = false;
            this.UserGridView.AllowUserToDeleteRows = false;
            this.UserGridView.AllowUserToResizeColumns = false;
            this.UserGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.UserGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.UserGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.UserGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.UserGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.UserGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.UserGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UserNameColumn,
            this.FullNameColumn,
            this.DescriptionColumn,
            this.GroupColumn});
            this.UserGridView.MultiSelect = false;
            this.UserGridView.Name = "UserGridView";
            this.UserGridView.RowHeadersVisible = false;
            this.UserGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.AliceBlue;
            this.UserGridView.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.UserGridView.RowTemplate.Height = 24;
            this.UserGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.UserGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.UserGridView_CellDoubleClick);
            // 
            // btn_LoadUserRight
            // 
            resources.ApplyResources(this.btn_LoadUserRight, "btn_LoadUserRight");
            this.btn_LoadUserRight.Name = "btn_LoadUserRight";
            this.btn_LoadUserRight.Click += new System.EventHandler(this.btn_LoadUserRight_Click);
            // 
            // UserNameColumn
            // 
            this.UserNameColumn.Frozen = true;
            resources.ApplyResources(this.UserNameColumn, "UserNameColumn");
            this.UserNameColumn.Name = "UserNameColumn";
            this.UserNameColumn.ReadOnly = true;
            this.UserNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FullNameColumn
            // 
            resources.ApplyResources(this.FullNameColumn, "FullNameColumn");
            this.FullNameColumn.Name = "FullNameColumn";
            this.FullNameColumn.ReadOnly = true;
            this.FullNameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FullNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // DescriptionColumn
            // 
            resources.ApplyResources(this.DescriptionColumn, "DescriptionColumn");
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.ReadOnly = true;
            this.DescriptionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DescriptionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GroupColumn
            // 
            resources.ApplyResources(this.GroupColumn, "GroupColumn");
            this.GroupColumn.Name = "GroupColumn";
            this.GroupColumn.ReadOnly = true;
            this.GroupColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.GroupColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GroupNoColumn
            // 
            this.GroupNoColumn.Frozen = true;
            resources.ApplyResources(this.GroupNoColumn, "GroupNoColumn");
            this.GroupNoColumn.Name = "GroupNoColumn";
            this.GroupNoColumn.ReadOnly = true;
            this.GroupNoColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.GroupNoColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GroupDescriptionColumn
            // 
            resources.ApplyResources(this.GroupDescriptionColumn, "GroupDescriptionColumn");
            this.GroupDescriptionColumn.Name = "GroupDescriptionColumn";
            this.GroupDescriptionColumn.ReadOnly = true;
            this.GroupDescriptionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.GroupDescriptionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // UserManagerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_LoadUserRight);
            this.Controls.Add(this.GroupGridView);
            this.Controls.Add(this.UserGridView);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.UserRightButton);
            this.Controls.Add(this.NewUserButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserManagerForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.UserManagerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GroupGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton CloseButton;
        private SRMControl.SRMButton UserRightButton;
        private SRMControl.SRMButton NewUserButton;
        private System.Windows.Forms.DataGridView GroupGridView;
        private System.Windows.Forms.DataGridView UserGridView;
        private SRMControl.SRMButton btn_LoadUserRight;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupNoColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupDescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FullNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupColumn;
    }
}