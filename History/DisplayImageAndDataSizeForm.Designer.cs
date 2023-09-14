namespace History
{
    partial class DisplayImageAndDataSizeForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabCtrl_History = new SRMControl.SRMTabControl();
            this.tp_Image = new System.Windows.Forms.TabPage();
            this.dgd_ImageFolder = new System.Windows.Forms.DataGridView();
            this.ImageFolderName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FolderSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tp_Data = new System.Windows.Forms.TabPage();
            this.dgd_Data = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tp_ImageDeleted = new System.Windows.Forms.TabPage();
            this.cbo_ViewPartition = new SRMControl.SRMComboBox();
            this.dgd_ImageDeleted = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tp_Graph = new System.Windows.Forms.TabPage();
            this.Chart_Space = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dgd_DisplaySpaceUsed = new System.Windows.Forms.DataGridView();
            this.Partition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Image = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Others = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Empty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabCtrl_History.SuspendLayout();
            this.tp_Image.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ImageFolder)).BeginInit();
            this.tp_Data.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_Data)).BeginInit();
            this.tp_ImageDeleted.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ImageDeleted)).BeginInit();
            this.tp_Graph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Space)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_DisplaySpaceUsed)).BeginInit();
            this.SuspendLayout();
            // 
            // tabCtrl_History
            // 
            this.tabCtrl_History.Controls.Add(this.tp_Image);
            this.tabCtrl_History.Controls.Add(this.tp_Data);
            this.tabCtrl_History.Controls.Add(this.tp_ImageDeleted);
            this.tabCtrl_History.Controls.Add(this.tp_Graph);
            this.tabCtrl_History.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            this.tabCtrl_History.ItemSize = new System.Drawing.Size(89, 30);
            this.tabCtrl_History.Location = new System.Drawing.Point(1, 1);
            this.tabCtrl_History.Name = "tabCtrl_History";
            this.tabCtrl_History.SelectedIndex = 0;
            this.tabCtrl_History.Size = new System.Drawing.Size(774, 467);
            this.tabCtrl_History.TabIndex = 196;
            this.tabCtrl_History.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabCtrl_History_Selected);
            // 
            // tp_Image
            // 
            this.tp_Image.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Image.Controls.Add(this.dgd_ImageFolder);
            this.tp_Image.Location = new System.Drawing.Point(4, 34);
            this.tp_Image.Name = "tp_Image";
            this.tp_Image.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Image.Size = new System.Drawing.Size(635, 408);
            this.tp_Image.TabIndex = 0;
            this.tp_Image.Text = "Image Folder";
            // 
            // dgd_ImageFolder
            // 
            this.dgd_ImageFolder.AllowUserToAddRows = false;
            this.dgd_ImageFolder.AllowUserToDeleteRows = false;
            this.dgd_ImageFolder.AllowUserToResizeColumns = false;
            this.dgd_ImageFolder.AllowUserToResizeRows = false;
            this.dgd_ImageFolder.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgd_ImageFolder.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgd_ImageFolder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_ImageFolder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImageFolderName,
            this.FolderSize});
            this.dgd_ImageFolder.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgd_ImageFolder.Location = new System.Drawing.Point(0, 0);
            this.dgd_ImageFolder.Name = "dgd_ImageFolder";
            this.dgd_ImageFolder.ReadOnly = true;
            this.dgd_ImageFolder.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgd_ImageFolder.RowHeadersVisible = false;
            this.dgd_ImageFolder.RowHeadersWidth = 40;
            this.dgd_ImageFolder.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgd_ImageFolder.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgd_ImageFolder.Size = new System.Drawing.Size(632, 408);
            this.dgd_ImageFolder.TabIndex = 0;
            // 
            // ImageFolderName
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImageFolderName.DefaultCellStyle = dataGridViewCellStyle1;
            this.ImageFolderName.DividerWidth = 1;
            this.ImageFolderName.HeaderText = "FolderName";
            this.ImageFolderName.Name = "ImageFolderName";
            this.ImageFolderName.ReadOnly = true;
            this.ImageFolderName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ImageFolderName.Width = 430;
            // 
            // FolderSize
            // 
            this.FolderSize.DividerWidth = 1;
            this.FolderSize.HeaderText = "Size(MB)";
            this.FolderSize.Name = "FolderSize";
            this.FolderSize.ReadOnly = true;
            this.FolderSize.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // tp_Data
            // 
            this.tp_Data.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Data.Controls.Add(this.dgd_Data);
            this.tp_Data.Location = new System.Drawing.Point(4, 34);
            this.tp_Data.Name = "tp_Data";
            this.tp_Data.Size = new System.Drawing.Size(635, 408);
            this.tp_Data.TabIndex = 2;
            this.tp_Data.Text = "Data Folder";
            // 
            // dgd_Data
            // 
            this.dgd_Data.AllowUserToAddRows = false;
            this.dgd_Data.AllowUserToDeleteRows = false;
            this.dgd_Data.AllowUserToResizeColumns = false;
            this.dgd_Data.AllowUserToResizeRows = false;
            this.dgd_Data.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgd_Data.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgd_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_Data.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgd_Data.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgd_Data.Location = new System.Drawing.Point(-3, 0);
            this.dgd_Data.Name = "dgd_Data";
            this.dgd_Data.ReadOnly = true;
            this.dgd_Data.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgd_Data.RowHeadersVisible = false;
            this.dgd_Data.RowHeadersWidth = 40;
            this.dgd_Data.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgd_Data.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgd_Data.Size = new System.Drawing.Size(635, 408);
            this.dgd_Data.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn1.DividerWidth = 1;
            this.dataGridViewTextBoxColumn1.HeaderText = "FolderName";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.Width = 300;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DividerWidth = 1;
            this.dataGridViewTextBoxColumn2.HeaderText = "Size(MB)";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // tp_ImageDeleted
            // 
            this.tp_ImageDeleted.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_ImageDeleted.Controls.Add(this.cbo_ViewPartition);
            this.tp_ImageDeleted.Controls.Add(this.dgd_ImageDeleted);
            this.tp_ImageDeleted.Location = new System.Drawing.Point(4, 34);
            this.tp_ImageDeleted.Name = "tp_ImageDeleted";
            this.tp_ImageDeleted.Padding = new System.Windows.Forms.Padding(3);
            this.tp_ImageDeleted.Size = new System.Drawing.Size(635, 408);
            this.tp_ImageDeleted.TabIndex = 1;
            this.tp_ImageDeleted.Text = "Image Deleted";
            // 
            // cbo_ViewPartition
            // 
            this.cbo_ViewPartition.BackColor = System.Drawing.Color.White;
            this.cbo_ViewPartition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ViewPartition.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ViewPartition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cbo_ViewPartition.FormattingEnabled = true;
            this.cbo_ViewPartition.ItemHeight = 28;
            this.cbo_ViewPartition.Location = new System.Drawing.Point(4, 3);
            this.cbo_ViewPartition.Margin = new System.Windows.Forms.Padding(0);
            this.cbo_ViewPartition.Name = "cbo_ViewPartition";
            this.cbo_ViewPartition.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ViewPartition.Size = new System.Drawing.Size(98, 34);
            this.cbo_ViewPartition.TabIndex = 927;
            this.cbo_ViewPartition.SelectedValueChanged += new System.EventHandler(this.cbo_ViewPartition_SelectedValueChanged);
            // 
            // dgd_ImageDeleted
            // 
            this.dgd_ImageDeleted.AllowUserToAddRows = false;
            this.dgd_ImageDeleted.AllowUserToDeleteRows = false;
            this.dgd_ImageDeleted.AllowUserToResizeColumns = false;
            this.dgd_ImageDeleted.AllowUserToResizeRows = false;
            this.dgd_ImageDeleted.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgd_ImageDeleted.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgd_ImageDeleted.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_ImageDeleted.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.dgd_ImageDeleted.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgd_ImageDeleted.Location = new System.Drawing.Point(0, 42);
            this.dgd_ImageDeleted.Name = "dgd_ImageDeleted";
            this.dgd_ImageDeleted.ReadOnly = true;
            this.dgd_ImageDeleted.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgd_ImageDeleted.RowHeadersVisible = false;
            this.dgd_ImageDeleted.RowHeadersWidth = 40;
            this.dgd_ImageDeleted.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgd_ImageDeleted.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgd_ImageDeleted.Size = new System.Drawing.Size(635, 366);
            this.dgd_ImageDeleted.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn3.DividerWidth = 1;
            this.dataGridViewTextBoxColumn3.HeaderText = "FolderName";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn3.Width = 430;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DividerWidth = 1;
            this.dataGridViewTextBoxColumn4.HeaderText = "Deleted Time";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn4.Width = 200;
            // 
            // tp_Graph
            // 
            this.tp_Graph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Graph.Controls.Add(this.Chart_Space);
            this.tp_Graph.Location = new System.Drawing.Point(4, 34);
            this.tp_Graph.Name = "tp_Graph";
            this.tp_Graph.Size = new System.Drawing.Size(766, 429);
            this.tp_Graph.TabIndex = 3;
            this.tp_Graph.Text = "Graph";
            // 
            // Chart_Space
            // 
            chartArea1.AxisX.MajorGrid.LineWidth = 0;
            chartArea1.AxisY.MajorGrid.LineWidth = 0;
            chartArea1.Name = "ChartArea1";
            this.Chart_Space.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.Chart_Space.Legends.Add(legend1);
            this.Chart_Space.Location = new System.Drawing.Point(3, 3);
            this.Chart_Space.Name = "Chart_Space";
            this.Chart_Space.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
            series1.Legend = "Legend1";
            series1.MarkerBorderColor = System.Drawing.Color.White;
            series1.MarkerColor = System.Drawing.Color.White;
            series1.Name = "Save Image";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
            series2.Legend = "Legend1";
            series2.Name = "Data";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
            series3.Legend = "Legend1";
            series3.Name = "Others";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
            series4.Legend = "Legend1";
            series4.Name = "Empty";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            this.Chart_Space.Series.Add(series1);
            this.Chart_Space.Series.Add(series2);
            this.Chart_Space.Series.Add(series3);
            this.Chart_Space.Series.Add(series4);
            this.Chart_Space.Size = new System.Drawing.Size(760, 426);
            this.Chart_Space.TabIndex = 0;
            this.Chart_Space.Text = "Chart_Space";
            this.Chart_Space.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart_Space_MouseMove);
            // 
            // dgd_DisplaySpaceUsed
            // 
            this.dgd_DisplaySpaceUsed.AllowUserToAddRows = false;
            this.dgd_DisplaySpaceUsed.AllowUserToDeleteRows = false;
            this.dgd_DisplaySpaceUsed.AllowUserToResizeRows = false;
            this.dgd_DisplaySpaceUsed.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgd_DisplaySpaceUsed.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgd_DisplaySpaceUsed.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgd_DisplaySpaceUsed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgd_DisplaySpaceUsed.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Partition,
            this.Image,
            this.Data,
            this.Others,
            this.Empty});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgd_DisplaySpaceUsed.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgd_DisplaySpaceUsed.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgd_DisplaySpaceUsed.Location = new System.Drawing.Point(8, 486);
            this.dgd_DisplaySpaceUsed.Name = "dgd_DisplaySpaceUsed";
            this.dgd_DisplaySpaceUsed.ReadOnly = true;
            this.dgd_DisplaySpaceUsed.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgd_DisplaySpaceUsed.RowHeadersVisible = false;
            this.dgd_DisplaySpaceUsed.Size = new System.Drawing.Size(767, 107);
            this.dgd_DisplaySpaceUsed.TabIndex = 197;
            this.dgd_DisplaySpaceUsed.Visible = false;
            // 
            // Partition
            // 
            this.Partition.HeaderText = "Partition";
            this.Partition.Name = "Partition";
            this.Partition.ReadOnly = true;
            this.Partition.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Partition.Width = 120;
            // 
            // Image
            // 
            this.Image.HeaderText = "Save Image";
            this.Image.Name = "Image";
            this.Image.ReadOnly = true;
            this.Image.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Image.Width = 180;
            // 
            // Data
            // 
            this.Data.HeaderText = "Data";
            this.Data.Name = "Data";
            this.Data.ReadOnly = true;
            this.Data.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Others
            // 
            this.Others.HeaderText = "Others";
            this.Others.Name = "Others";
            this.Others.ReadOnly = true;
            this.Others.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Others.Width = 180;
            // 
            // Empty
            // 
            this.Empty.HeaderText = "Empty";
            this.Empty.Name = "Empty";
            this.Empty.ReadOnly = true;
            this.Empty.Width = 180;
            // 
            // DisplayImageAndDataSizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(803, 605);
            this.Controls.Add(this.dgd_DisplaySpaceUsed);
            this.Controls.Add(this.tabCtrl_History);
            this.Name = "DisplayImageAndDataSizeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Image And Data Folder Size";
            this.tabCtrl_History.ResumeLayout(false);
            this.tp_Image.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ImageFolder)).EndInit();
            this.tp_Data.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_Data)).EndInit();
            this.tp_ImageDeleted.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgd_ImageDeleted)).EndInit();
            this.tp_Graph.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Space)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgd_DisplaySpaceUsed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMTabControl tabCtrl_History;
        private System.Windows.Forms.TabPage tp_Image;
        private System.Windows.Forms.TabPage tp_ImageDeleted;
        private System.Windows.Forms.TabPage tp_Data;
        private System.Windows.Forms.DataGridView dgd_ImageFolder;
        private System.Windows.Forms.DataGridView dgd_Data;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView dgd_ImageDeleted;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.TabPage tp_Graph;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart_Space;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImageFolderName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FolderSize;
        private System.Windows.Forms.DataGridView dgd_DisplaySpaceUsed;
        private SRMControl.SRMComboBox cbo_ViewPartition;
        private System.Windows.Forms.DataGridViewTextBoxColumn Partition;
        private System.Windows.Forms.DataGridViewTextBoxColumn Image;
        private System.Windows.Forms.DataGridViewTextBoxColumn Data;
        private System.Windows.Forms.DataGridViewTextBoxColumn Others;
        private System.Windows.Forms.DataGridViewTextBoxColumn Empty;
    }
}