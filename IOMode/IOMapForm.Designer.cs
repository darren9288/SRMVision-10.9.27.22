namespace IOMode
{
    partial class IOMapForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IOMapForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.IOTreeView = new System.Windows.Forms.TreeView();
            this.IOImageListTree = new System.Windows.Forms.ImageList(this.components);
            this.CloseButton = new SRMControl.SRMButton();
            this.IOGridView = new System.Windows.Forms.DataGridView();
            this.OnOffColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.StateColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.dc_IOOnOff = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dc_IOState = new System.Windows.Forms.DataGridViewImageColumn();
            this.NumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardNoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChannelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BitColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PinColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModuleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OutputImageListTree = new System.Windows.Forms.ImageList(this.components);
            this.IOTimer = new System.Windows.Forms.Timer(this.components);
            this.InputImageListTree = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.IOGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // IOTreeView
            // 
            this.IOTreeView.BackColor = System.Drawing.Color.AliceBlue;
            resources.ApplyResources(this.IOTreeView, "IOTreeView");
            this.IOTreeView.ImageList = this.IOImageListTree;
            this.IOTreeView.ItemHeight = 24;
            this.IOTreeView.Name = "IOTreeView";
            this.IOTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.IOTreeView_AfterSelect);
            this.IOTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.IOTreeView_NodeMouseClick);
            this.IOTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.IOTreeView_BeforeSelect);
            // 
            // IOImageListTree
            // 
            this.IOImageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IOImageListTree.ImageStream")));
            this.IOImageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.IOImageListTree.Images.SetKeyName(0, "input.png");
            this.IOImageListTree.Images.SetKeyName(1, "output.png");
            this.IOImageListTree.Images.SetKeyName(2, "io_on.png");
            this.IOImageListTree.Images.SetKeyName(3, "io_off.png");
            this.IOImageListTree.Images.SetKeyName(4, "CheckedNormal.png");
            this.IOImageListTree.Images.SetKeyName(5, "CheckedDisabled.png");
            this.IOImageListTree.Images.SetKeyName(6, "UncheckedNormal.png");
            this.IOImageListTree.Images.SetKeyName(7, "UncheckedDisabled.png");
            this.IOImageListTree.Images.SetKeyName(8, "io_trigger_24.png");
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // IOGridView
            // 
            this.IOGridView.AllowUserToAddRows = false;
            this.IOGridView.AllowUserToDeleteRows = false;
            this.IOGridView.AllowUserToResizeColumns = false;
            this.IOGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.IOGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.IOGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.IOGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(190)))), ((int)(((byte)(226)))));
            this.IOGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.IOGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(190)))), ((int)(((byte)(226)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.IOGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.IOGridView, "IOGridView");
            this.IOGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.IOGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OnOffColumn,
            this.StateColumn,
            this.dc_IOOnOff,
            this.dc_IOState,
            this.NumberColumn,
            this.DescriptionColumn,
            this.CardNoColumn,
            this.ChannelColumn,
            this.BitColumn,
            this.PinColumn,
            this.ModuleColumn});
            this.IOGridView.EnableHeadersVisualStyles = false;
            this.IOGridView.GridColor = System.Drawing.Color.White;
            this.IOGridView.MultiSelect = false;
            this.IOGridView.Name = "IOGridView";
            this.IOGridView.RowHeadersVisible = false;
            this.IOGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.AliceBlue;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.IOGridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.IOGridView.RowTemplate.Height = 24;
            this.IOGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.IOGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.IOGridView_CellClick);
            // 
            // OnOffColumn
            // 
            resources.ApplyResources(this.OnOffColumn, "OnOffColumn");
            this.OnOffColumn.Name = "OnOffColumn";
            // 
            // StateColumn
            // 
            resources.ApplyResources(this.StateColumn, "StateColumn");
            this.StateColumn.Name = "StateColumn";
            this.StateColumn.ReadOnly = true;
            // 
            // dc_IOOnOff
            // 
            resources.ApplyResources(this.dc_IOOnOff, "dc_IOOnOff");
            this.dc_IOOnOff.Name = "dc_IOOnOff";
            // 
            // dc_IOState
            // 
            resources.ApplyResources(this.dc_IOState, "dc_IOState");
            this.dc_IOState.Name = "dc_IOState";
            // 
            // NumberColumn
            // 
            resources.ApplyResources(this.NumberColumn, "NumberColumn");
            this.NumberColumn.Name = "NumberColumn";
            this.NumberColumn.ReadOnly = true;
            this.NumberColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // DescriptionColumn
            // 
            resources.ApplyResources(this.DescriptionColumn, "DescriptionColumn");
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.ReadOnly = true;
            // 
            // CardNoColumn
            // 
            resources.ApplyResources(this.CardNoColumn, "CardNoColumn");
            this.CardNoColumn.Name = "CardNoColumn";
            this.CardNoColumn.ReadOnly = true;
            // 
            // ChannelColumn
            // 
            resources.ApplyResources(this.ChannelColumn, "ChannelColumn");
            this.ChannelColumn.Name = "ChannelColumn";
            this.ChannelColumn.ReadOnly = true;
            // 
            // BitColumn
            // 
            resources.ApplyResources(this.BitColumn, "BitColumn");
            this.BitColumn.Name = "BitColumn";
            this.BitColumn.ReadOnly = true;
            // 
            // PinColumn
            // 
            resources.ApplyResources(this.PinColumn, "PinColumn");
            this.PinColumn.Name = "PinColumn";
            // 
            // ModuleColumn
            // 
            resources.ApplyResources(this.ModuleColumn, "ModuleColumn");
            this.ModuleColumn.Name = "ModuleColumn";
            // 
            // OutputImageListTree
            // 
            this.OutputImageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("OutputImageListTree.ImageStream")));
            this.OutputImageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.OutputImageListTree.Images.SetKeyName(0, "io_out_off_32.png");
            this.OutputImageListTree.Images.SetKeyName(1, "output_on_32.png");
            this.OutputImageListTree.Images.SetKeyName(2, "in_out_off.png");
            this.OutputImageListTree.Images.SetKeyName(3, "output_on.png");
            // 
            // IOTimer
            // 
            this.IOTimer.Enabled = true;
            this.IOTimer.Interval = 10;
            this.IOTimer.Tick += new System.EventHandler(this.IOTimer_Tick);
            // 
            // InputImageListTree
            // 
            this.InputImageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("InputImageListTree.ImageStream")));
            this.InputImageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.InputImageListTree.Images.SetKeyName(0, "io_out_off_32.png");
            this.InputImageListTree.Images.SetKeyName(1, "input_on_32.png");
            this.InputImageListTree.Images.SetKeyName(2, "in_out_off.png");
            this.InputImageListTree.Images.SetKeyName(3, "input_on.png");
            // 
            // IOMapForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.IOGridView);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.IOTreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IOMapForm";
            ((System.ComponentModel.ISupportInitialize)(this.IOGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView IOTreeView;
        private System.Windows.Forms.ImageList IOImageListTree;
        private SRMControl.SRMButton CloseButton;
        private System.Windows.Forms.DataGridView IOGridView;
        private System.Windows.Forms.ImageList OutputImageListTree;
        private System.Windows.Forms.Timer IOTimer;
        private System.Windows.Forms.ImageList InputImageListTree;
        private System.Windows.Forms.DataGridViewCheckBoxColumn OnOffColumn;
        private System.Windows.Forms.DataGridViewImageColumn StateColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dc_IOOnOff;
        private System.Windows.Forms.DataGridViewImageColumn dc_IOState;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardNoColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BitColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PinColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModuleColumn;
    }
}