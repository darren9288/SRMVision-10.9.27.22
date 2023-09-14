namespace IOMode
{
    partial class IOForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IOForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            this.InputIOGroupBox = new SRMControl.SRMGroupBox();
            this.InputIODataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnInputIOOnOff = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnInputIOState = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnInputIONumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnInputIODesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnInputIOChannel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnInputIOBit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InputIOComboBox = new SRMControl.SRMComboBox();
            this.lblInputIO = new SRMControl.SRMLabel();
            this.OutputIOGroupBox = new SRMControl.SRMGroupBox();
            this.OutputIOComboBox = new SRMControl.SRMComboBox();
            this.OutputIODataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnOutputIOOnOff = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnOutputIOState = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnOutputIONumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnOutputIODesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dc_CardNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnOutputIOChannel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnOutputIOBit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnOutputIOUserGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblOutputIO = new SRMControl.SRMLabel();
            this.CloseButton = new SRMControl.SRMButton();
            this.ModuleRadioButton = new SRMControl.SRMRadioButton();
            this.CardRadioButton = new SRMControl.SRMRadioButton();
            this.InputIOImageList = new System.Windows.Forms.ImageList(this.components);
            this.OutputIOImageList = new System.Windows.Forms.ImageList(this.components);
            this.SynchronizeCheckBox = new SRMControl.SRMCheckBox();
            this.InputIOTimer = new System.Windows.Forms.Timer(this.components);
            this.OutputIOTimer = new System.Windows.Forms.Timer(this.components);
            this.WantIODiagnosticCheckBox = new SRMControl.SRMCheckBox();
            this.IOContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Select1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Select2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Select3MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Select4MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Remove1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Remove2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Remove3MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Remove4MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IODiagnosticButton = new SRMControl.SRMButton();
            this.InputIOGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InputIODataGridView)).BeginInit();
            this.OutputIOGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputIODataGridView)).BeginInit();
            this.IOContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // InputIOGroupBox
            // 
            resources.ApplyResources(this.InputIOGroupBox, "InputIOGroupBox");
            this.InputIOGroupBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.InputIOGroupBox.Controls.Add(this.InputIODataGridView);
            this.InputIOGroupBox.Controls.Add(this.InputIOComboBox);
            this.InputIOGroupBox.Controls.Add(this.lblInputIO);
            this.InputIOGroupBox.Name = "InputIOGroupBox";
            this.InputIOGroupBox.TabStop = false;
            // 
            // InputIODataGridView
            // 
            resources.ApplyResources(this.InputIODataGridView, "InputIODataGridView");
            this.InputIODataGridView.AllowUserToAddRows = false;
            this.InputIODataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(222)))), ((int)(((byte)(255)))));
            this.InputIODataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.InputIODataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.InputIODataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(190)))), ((int)(((byte)(226)))));
            this.InputIODataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.InputIODataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(190)))), ((int)(((byte)(226)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.InputIODataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.InputIODataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.InputIODataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnInputIOOnOff,
            this.ColumnInputIOState,
            this.ColumnInputIONumber,
            this.ColumnInputIODesc,
            this.dataGridViewTextBoxColumn1,
            this.ColumnInputIOChannel,
            this.ColumnInputIOBit});
            this.InputIODataGridView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InputIODataGridView.EnableHeadersVisualStyles = false;
            this.InputIODataGridView.GridColor = System.Drawing.Color.White;
            this.InputIODataGridView.MultiSelect = false;
            this.InputIODataGridView.Name = "InputIODataGridView";
            this.InputIODataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(236)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(0, 2, 0, 1);
            this.InputIODataGridView.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.InputIODataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputIODataGridView.RowTemplate.Height = 20;
            this.InputIODataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.InputIODataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.InputIODataGridView_CellClick);
            // 
            // ColumnInputIOOnOff
            // 
            resources.ApplyResources(this.ColumnInputIOOnOff, "ColumnInputIOOnOff");
            this.ColumnInputIOOnOff.Name = "ColumnInputIOOnOff";
            // 
            // ColumnInputIOState
            // 
            resources.ApplyResources(this.ColumnInputIOState, "ColumnInputIOState");
            this.ColumnInputIOState.Name = "ColumnInputIOState";
            // 
            // ColumnInputIONumber
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnInputIONumber.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.ColumnInputIONumber, "ColumnInputIONumber");
            this.ColumnInputIONumber.Name = "ColumnInputIONumber";
            this.ColumnInputIONumber.ReadOnly = true;
            this.ColumnInputIONumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnInputIODesc
            // 
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnInputIODesc.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.ColumnInputIODesc, "ColumnInputIODesc");
            this.ColumnInputIODesc.Name = "ColumnInputIODesc";
            this.ColumnInputIODesc.ReadOnly = true;
            this.ColumnInputIODesc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn1
            // 
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // ColumnInputIOChannel
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnInputIOChannel.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.ColumnInputIOChannel, "ColumnInputIOChannel");
            this.ColumnInputIOChannel.Name = "ColumnInputIOChannel";
            this.ColumnInputIOChannel.ReadOnly = true;
            this.ColumnInputIOChannel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnInputIOBit
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnInputIOBit.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.ColumnInputIOBit, "ColumnInputIOBit");
            this.ColumnInputIOBit.Name = "ColumnInputIOBit";
            this.ColumnInputIOBit.ReadOnly = true;
            this.ColumnInputIOBit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // InputIOComboBox
            // 
            resources.ApplyResources(this.InputIOComboBox, "InputIOComboBox");
            this.InputIOComboBox.BackColor = System.Drawing.Color.AliceBlue;
            this.InputIOComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InputIOComboBox.DropDownHeight = 482;
            this.InputIOComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.InputIOComboBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.InputIOComboBox.FormattingEnabled = true;
            this.InputIOComboBox.Name = "InputIOComboBox";
            this.InputIOComboBox.NormalBackColor = System.Drawing.Color.AliceBlue;
            this.InputIOComboBox.SelectedIndexChanged += new System.EventHandler(this.InputIOComboBox_SelectedIndexChanged);
            // 
            // lblInputIO
            // 
            resources.ApplyResources(this.lblInputIO, "lblInputIO");
            this.lblInputIO.Name = "lblInputIO";
            this.lblInputIO.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // OutputIOGroupBox
            // 
            resources.ApplyResources(this.OutputIOGroupBox, "OutputIOGroupBox");
            this.OutputIOGroupBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.OutputIOGroupBox.Controls.Add(this.OutputIOComboBox);
            this.OutputIOGroupBox.Controls.Add(this.OutputIODataGridView);
            this.OutputIOGroupBox.Controls.Add(this.lblOutputIO);
            this.OutputIOGroupBox.Name = "OutputIOGroupBox";
            this.OutputIOGroupBox.TabStop = false;
            // 
            // OutputIOComboBox
            // 
            resources.ApplyResources(this.OutputIOComboBox, "OutputIOComboBox");
            this.OutputIOComboBox.BackColor = System.Drawing.Color.AliceBlue;
            this.OutputIOComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OutputIOComboBox.DropDownHeight = 482;
            this.OutputIOComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OutputIOComboBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.OutputIOComboBox.FormattingEnabled = true;
            this.OutputIOComboBox.Name = "OutputIOComboBox";
            this.OutputIOComboBox.NormalBackColor = System.Drawing.Color.AliceBlue;
            this.OutputIOComboBox.SelectedIndexChanged += new System.EventHandler(this.OutputIOComboBox_SelectedIndexChanged);
            // 
            // OutputIODataGridView
            // 
            resources.ApplyResources(this.OutputIODataGridView, "OutputIODataGridView");
            this.OutputIODataGridView.AllowUserToAddRows = false;
            this.OutputIODataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(222)))), ((int)(((byte)(255)))));
            this.OutputIODataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle8;
            this.OutputIODataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.OutputIODataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(190)))), ((int)(((byte)(226)))));
            this.OutputIODataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.OutputIODataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(190)))), ((int)(((byte)(226)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.OutputIODataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.OutputIODataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.OutputIODataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnOutputIOOnOff,
            this.ColumnOutputIOState,
            this.ColumnOutputIONumber,
            this.ColumnOutputIODesc,
            this.dc_CardNo,
            this.ColumnOutputIOChannel,
            this.ColumnOutputIOBit,
            this.ColumnOutputIOUserGroup});
            this.OutputIODataGridView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OutputIODataGridView.EnableHeadersVisualStyles = false;
            this.OutputIODataGridView.GridColor = System.Drawing.Color.White;
            this.OutputIODataGridView.MultiSelect = false;
            this.OutputIODataGridView.Name = "OutputIODataGridView";
            this.OutputIODataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(236)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.Padding = new System.Windows.Forms.Padding(0, 2, 0, 1);
            this.OutputIODataGridView.RowsDefaultCellStyle = dataGridViewCellStyle14;
            this.OutputIODataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputIODataGridView.RowTemplate.Height = 20;
            this.OutputIODataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.OutputIODataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.OutputIODataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OutputIODataGridView_CellClick);
            // 
            // ColumnOutputIOOnOff
            // 
            resources.ApplyResources(this.ColumnOutputIOOnOff, "ColumnOutputIOOnOff");
            this.ColumnOutputIOOnOff.Name = "ColumnOutputIOOnOff";
            // 
            // ColumnOutputIOState
            // 
            resources.ApplyResources(this.ColumnOutputIOState, "ColumnOutputIOState");
            this.ColumnOutputIOState.Name = "ColumnOutputIOState";
            // 
            // ColumnOutputIONumber
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnOutputIONumber.DefaultCellStyle = dataGridViewCellStyle10;
            resources.ApplyResources(this.ColumnOutputIONumber, "ColumnOutputIONumber");
            this.ColumnOutputIONumber.Name = "ColumnOutputIONumber";
            this.ColumnOutputIONumber.ReadOnly = true;
            this.ColumnOutputIONumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnOutputIODesc
            // 
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnOutputIODesc.DefaultCellStyle = dataGridViewCellStyle11;
            resources.ApplyResources(this.ColumnOutputIODesc, "ColumnOutputIODesc");
            this.ColumnOutputIODesc.Name = "ColumnOutputIODesc";
            this.ColumnOutputIODesc.ReadOnly = true;
            this.ColumnOutputIODesc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dc_CardNo
            // 
            resources.ApplyResources(this.dc_CardNo, "dc_CardNo");
            this.dc_CardNo.Name = "dc_CardNo";
            this.dc_CardNo.ReadOnly = true;
            // 
            // ColumnOutputIOChannel
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnOutputIOChannel.DefaultCellStyle = dataGridViewCellStyle12;
            resources.ApplyResources(this.ColumnOutputIOChannel, "ColumnOutputIOChannel");
            this.ColumnOutputIOChannel.Name = "ColumnOutputIOChannel";
            this.ColumnOutputIOChannel.ReadOnly = true;
            this.ColumnOutputIOChannel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnOutputIOBit
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnOutputIOBit.DefaultCellStyle = dataGridViewCellStyle13;
            resources.ApplyResources(this.ColumnOutputIOBit, "ColumnOutputIOBit");
            this.ColumnOutputIOBit.Name = "ColumnOutputIOBit";
            this.ColumnOutputIOBit.ReadOnly = true;
            this.ColumnOutputIOBit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnOutputIOUserGroup
            // 
            resources.ApplyResources(this.ColumnOutputIOUserGroup, "ColumnOutputIOUserGroup");
            this.ColumnOutputIOUserGroup.Name = "ColumnOutputIOUserGroup";
            // 
            // lblOutputIO
            // 
            resources.ApplyResources(this.lblOutputIO, "lblOutputIO");
            this.lblOutputIO.Name = "lblOutputIO";
            this.lblOutputIO.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ModuleRadioButton
            // 
            resources.ApplyResources(this.ModuleRadioButton, "ModuleRadioButton");
            this.ModuleRadioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ModuleRadioButton.Name = "ModuleRadioButton";
            this.ModuleRadioButton.Click += new System.EventHandler(this.RadioButton_Click);
            // 
            // CardRadioButton
            // 
            resources.ApplyResources(this.CardRadioButton, "CardRadioButton");
            this.CardRadioButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CardRadioButton.Name = "CardRadioButton";
            this.CardRadioButton.Click += new System.EventHandler(this.RadioButton_Click);
            // 
            // InputIOImageList
            // 
            this.InputIOImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("InputIOImageList.ImageStream")));
            this.InputIOImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.InputIOImageList.Images.SetKeyName(0, "");
            this.InputIOImageList.Images.SetKeyName(1, "");
            // 
            // OutputIOImageList
            // 
            this.OutputIOImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("OutputIOImageList.ImageStream")));
            this.OutputIOImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.OutputIOImageList.Images.SetKeyName(0, "");
            this.OutputIOImageList.Images.SetKeyName(1, "");
            // 
            // SynchronizeCheckBox
            // 
            resources.ApplyResources(this.SynchronizeCheckBox, "SynchronizeCheckBox");
            this.SynchronizeCheckBox.CheckedColor = System.Drawing.Color.GreenYellow;
            this.SynchronizeCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SynchronizeCheckBox.Name = "SynchronizeCheckBox";
            this.SynchronizeCheckBox.Selected = false;
            this.SynchronizeCheckBox.SelectedBorderColor = System.Drawing.Color.Red;
            this.SynchronizeCheckBox.UnCheckedColor = System.Drawing.Color.Red;
            this.SynchronizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // InputIOTimer
            // 
            this.InputIOTimer.Enabled = true;
            this.InputIOTimer.Tick += new System.EventHandler(this.InputIOTimer_Tick);
            // 
            // OutputIOTimer
            // 
            this.OutputIOTimer.Enabled = true;
            this.OutputIOTimer.Tick += new System.EventHandler(this.OutputIOTimer_Tick);
            // 
            // WantIODiagnosticCheckBox
            // 
            resources.ApplyResources(this.WantIODiagnosticCheckBox, "WantIODiagnosticCheckBox");
            this.WantIODiagnosticCheckBox.CheckedColor = System.Drawing.Color.GreenYellow;
            this.WantIODiagnosticCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.WantIODiagnosticCheckBox.Name = "WantIODiagnosticCheckBox";
            this.WantIODiagnosticCheckBox.Selected = false;
            this.WantIODiagnosticCheckBox.SelectedBorderColor = System.Drawing.Color.Red;
            this.WantIODiagnosticCheckBox.UnCheckedColor = System.Drawing.Color.Red;
            this.WantIODiagnosticCheckBox.UseVisualStyleBackColor = true;
            this.WantIODiagnosticCheckBox.Click += new System.EventHandler(this.WantIODiagnosticCheckBox_Click);
            // 
            // IOContextMenuStrip
            // 
            resources.ApplyResources(this.IOContextMenuStrip, "IOContextMenuStrip");
            this.IOContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Select1MenuItem,
            this.Select2MenuItem,
            this.Select3MenuItem,
            this.Select4MenuItem,
            this.toolStripSeparator1,
            this.Remove1MenuItem,
            this.Remove2MenuItem,
            this.Remove3MenuItem,
            this.Remove4MenuItem});
            this.IOContextMenuStrip.Name = "IOContextMenuStrip";
            this.IOContextMenuStrip.ShowImageMargin = false;
            this.IOContextMenuStrip.ShowItemToolTips = false;
            // 
            // Select1MenuItem
            // 
            resources.ApplyResources(this.Select1MenuItem, "Select1MenuItem");
            this.Select1MenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Select1MenuItem.Name = "Select1MenuItem";
            this.Select1MenuItem.Tag = "0";
            this.Select1MenuItem.Click += new System.EventHandler(this.Select1MenuItem_Click);
            // 
            // Select2MenuItem
            // 
            resources.ApplyResources(this.Select2MenuItem, "Select2MenuItem");
            this.Select2MenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Select2MenuItem.Name = "Select2MenuItem";
            this.Select2MenuItem.Tag = "1";
            this.Select2MenuItem.Click += new System.EventHandler(this.Select2MenuItem_Click);
            // 
            // Select3MenuItem
            // 
            resources.ApplyResources(this.Select3MenuItem, "Select3MenuItem");
            this.Select3MenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Select3MenuItem.Name = "Select3MenuItem";
            this.Select3MenuItem.Tag = "2";
            this.Select3MenuItem.Click += new System.EventHandler(this.Select3MenuItem_Click);
            // 
            // Select4MenuItem
            // 
            resources.ApplyResources(this.Select4MenuItem, "Select4MenuItem");
            this.Select4MenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Select4MenuItem.Name = "Select4MenuItem";
            this.Select4MenuItem.Tag = "3";
            this.Select4MenuItem.Click += new System.EventHandler(this.Select4MenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // Remove1MenuItem
            // 
            resources.ApplyResources(this.Remove1MenuItem, "Remove1MenuItem");
            this.Remove1MenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Remove1MenuItem.Name = "Remove1MenuItem";
            this.Remove1MenuItem.Tag = "0";
            this.Remove1MenuItem.Click += new System.EventHandler(this.Remove1MenuItem_Click);
            // 
            // Remove2MenuItem
            // 
            resources.ApplyResources(this.Remove2MenuItem, "Remove2MenuItem");
            this.Remove2MenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Remove2MenuItem.Name = "Remove2MenuItem";
            this.Remove2MenuItem.Tag = "1";
            this.Remove2MenuItem.Click += new System.EventHandler(this.Remove2MenuItem_Click);
            // 
            // Remove3MenuItem
            // 
            resources.ApplyResources(this.Remove3MenuItem, "Remove3MenuItem");
            this.Remove3MenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Remove3MenuItem.Name = "Remove3MenuItem";
            this.Remove3MenuItem.Tag = "2";
            this.Remove3MenuItem.Click += new System.EventHandler(this.Remove3MenuItem_Click);
            // 
            // Remove4MenuItem
            // 
            resources.ApplyResources(this.Remove4MenuItem, "Remove4MenuItem");
            this.Remove4MenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Remove4MenuItem.Name = "Remove4MenuItem";
            this.Remove4MenuItem.Tag = "3";
            this.Remove4MenuItem.Click += new System.EventHandler(this.Remove4MenuItem_Click);
            // 
            // IODiagnosticButton
            // 
            resources.ApplyResources(this.IODiagnosticButton, "IODiagnosticButton");
            this.IODiagnosticButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.IODiagnosticButton.Name = "IODiagnosticButton";
            this.IODiagnosticButton.Click += new System.EventHandler(this.IODiagnosticButton_Click);
            // 
            // IOForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.OutputIOGroupBox);
            this.Controls.Add(this.InputIOGroupBox);
            this.Controls.Add(this.WantIODiagnosticCheckBox);
            this.Controls.Add(this.SynchronizeCheckBox);
            this.Controls.Add(this.ModuleRadioButton);
            this.Controls.Add(this.CardRadioButton);
            this.Controls.Add(this.IODiagnosticButton);
            this.Controls.Add(this.CloseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IOForm";
            this.ShowInTaskbar = false;
            this.Activated += new System.EventHandler(this.IOForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IOTriggerForm_FormClosing);
            this.InputIOGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.InputIODataGridView)).EndInit();
            this.OutputIOGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OutputIODataGridView)).EndInit();
            this.IOContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public SRMControl.SRMGroupBox InputIOGroupBox;
        private System.Windows.Forms.DataGridView InputIODataGridView;
        private SRMControl.SRMComboBox InputIOComboBox;
        private SRMControl.SRMLabel lblInputIO;
        public SRMControl.SRMGroupBox OutputIOGroupBox;
        private SRMControl.SRMComboBox OutputIOComboBox;
        private System.Windows.Forms.DataGridView OutputIODataGridView;
        private SRMControl.SRMLabel lblOutputIO;
        private SRMControl.SRMButton CloseButton;
        private SRMControl.SRMRadioButton ModuleRadioButton;
        private SRMControl.SRMButton IODiagnosticButton;
        private SRMControl.SRMRadioButton CardRadioButton;
        private System.Windows.Forms.ImageList InputIOImageList;
        private System.Windows.Forms.ImageList OutputIOImageList;
        private SRMControl.SRMCheckBox SynchronizeCheckBox;
        private System.Windows.Forms.Timer InputIOTimer;
        private System.Windows.Forms.Timer OutputIOTimer;
        private SRMControl.SRMCheckBox WantIODiagnosticCheckBox;
        private System.Windows.Forms.ContextMenuStrip IOContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem Select1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem Select2MenuItem;
        private System.Windows.Forms.ToolStripMenuItem Select3MenuItem;
        private System.Windows.Forms.ToolStripMenuItem Select4MenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Remove1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem Remove2MenuItem;
        private System.Windows.Forms.ToolStripMenuItem Remove3MenuItem;
        private System.Windows.Forms.ToolStripMenuItem Remove4MenuItem;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnInputIOOnOff;
        private System.Windows.Forms.DataGridViewImageColumn ColumnInputIOState;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInputIONumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInputIODesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInputIOChannel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnInputIOBit;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnOutputIOOnOff;
        private System.Windows.Forms.DataGridViewImageColumn ColumnOutputIOState;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOutputIONumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOutputIODesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn dc_CardNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOutputIOChannel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOutputIOBit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnOutputIOUserGroup;
    }
}