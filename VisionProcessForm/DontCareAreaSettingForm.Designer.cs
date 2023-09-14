namespace VisionProcessForm
{
    partial class DontCareAreaSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DontCareAreaSettingForm));
            this.pnl_PictureBox = new System.Windows.Forms.Panel();
            this.pic_Image = new System.Windows.Forms.PictureBox();
            this.pnl_PicSideBlock = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnl_Production = new System.Windows.Forms.Panel();
            this.pnl_Auto = new System.Windows.Forms.Panel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_Threshold = new SRMControl.SRMInputBox();
            this.trackBar_Threshold = new System.Windows.Forms.TrackBar();
            this.lbl_Threshold = new SRMControl.SRMLabel();
            this.txt_Offset = new SRMControl.SRMInputBox();
            this.lbl_Offset = new SRMControl.SRMLabel();
            this.trackBar_Offset = new System.Windows.Forms.TrackBar();
            this.pnl_Manual = new System.Windows.Forms.Panel();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.btn_UndoROI = new SRMControl.SRMButton();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.btn_OK = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.chk_Auto = new SRMControl.SRMCheckBox();
            this.pnl_PictureBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Image)).BeginInit();
            this.pnl_Production.SuspendLayout();
            this.pnl_Auto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Offset)).BeginInit();
            this.pnl_Manual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            this.SuspendLayout();
            // 
            // pnl_PictureBox
            // 
            resources.ApplyResources(this.pnl_PictureBox, "pnl_PictureBox");
            this.pnl_PictureBox.BackColor = System.Drawing.Color.Gray;
            this.pnl_PictureBox.Controls.Add(this.pic_Image);
            this.pnl_PictureBox.Controls.Add(this.pnl_PicSideBlock);
            this.pnl_PictureBox.Name = "pnl_PictureBox";
            this.pnl_PictureBox.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pnl_PictureBox_Scroll);
            // 
            // pic_Image
            // 
            resources.ApplyResources(this.pic_Image, "pic_Image");
            this.pic_Image.BackColor = System.Drawing.Color.Black;
            this.pic_Image.Name = "pic_Image";
            this.pic_Image.TabStop = false;
            this.pic_Image.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_Image_Paint);
            this.pic_Image.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic_Image_MouseDown);
            this.pic_Image.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pic_Image_MouseMove);
            this.pic_Image.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pic_Image_MouseUp);
            // 
            // pnl_PicSideBlock
            // 
            resources.ApplyResources(this.pnl_PicSideBlock, "pnl_PicSideBlock");
            this.pnl_PicSideBlock.BackColor = System.Drawing.Color.Transparent;
            this.pnl_PicSideBlock.Name = "pnl_PicSideBlock";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnl_Production
            // 
            resources.ApplyResources(this.pnl_Production, "pnl_Production");
            this.pnl_Production.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pnl_Production.Controls.Add(this.pnl_Auto);
            this.pnl_Production.Controls.Add(this.pnl_Manual);
            this.pnl_Production.Controls.Add(this.btn_OK);
            this.pnl_Production.Controls.Add(this.btn_Cancel);
            this.pnl_Production.Controls.Add(this.chk_Auto);
            this.pnl_Production.Name = "pnl_Production";
            // 
            // pnl_Auto
            // 
            resources.ApplyResources(this.pnl_Auto, "pnl_Auto");
            this.pnl_Auto.Controls.Add(this.srmLabel1);
            this.pnl_Auto.Controls.Add(this.txt_Threshold);
            this.pnl_Auto.Controls.Add(this.trackBar_Threshold);
            this.pnl_Auto.Controls.Add(this.lbl_Threshold);
            this.pnl_Auto.Controls.Add(this.txt_Offset);
            this.pnl_Auto.Controls.Add(this.lbl_Offset);
            this.pnl_Auto.Controls.Add(this.trackBar_Offset);
            this.pnl_Auto.Name = "pnl_Auto";
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Threshold
            // 
            resources.ApplyResources(this.txt_Threshold, "txt_Threshold");
            this.txt_Threshold.BackColor = System.Drawing.Color.White;
            this.txt_Threshold.DataType = SRMControl.SRMDataType.Int32;
            this.txt_Threshold.DecimalPlaces = 0;
            this.txt_Threshold.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Threshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Threshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Threshold.ForeColor = System.Drawing.Color.Black;
            this.txt_Threshold.InputType = SRMControl.InputType.Number;
            this.txt_Threshold.Name = "txt_Threshold";
            this.txt_Threshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_Threshold.TextChanged += new System.EventHandler(this.txt_Threshold_TextChanged);
            // 
            // trackBar_Threshold
            // 
            resources.ApplyResources(this.trackBar_Threshold, "trackBar_Threshold");
            this.trackBar_Threshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Threshold.LargeChange = 1;
            this.trackBar_Threshold.Maximum = 255;
            this.trackBar_Threshold.Name = "trackBar_Threshold";
            this.trackBar_Threshold.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Threshold.Value = 125;
            this.trackBar_Threshold.Scroll += new System.EventHandler(this.trackBar_Threshold_Scroll);
            // 
            // lbl_Threshold
            // 
            resources.ApplyResources(this.lbl_Threshold, "lbl_Threshold");
            this.lbl_Threshold.Name = "lbl_Threshold";
            this.lbl_Threshold.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Offset
            // 
            resources.ApplyResources(this.txt_Offset, "txt_Offset");
            this.txt_Offset.BackColor = System.Drawing.Color.White;
            this.txt_Offset.DataType = SRMControl.SRMDataType.Int32;
            this.txt_Offset.DecimalPlaces = 0;
            this.txt_Offset.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_Offset.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Offset.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Offset.ForeColor = System.Drawing.Color.Black;
            this.txt_Offset.InputType = SRMControl.InputType.Number;
            this.txt_Offset.Name = "txt_Offset";
            this.txt_Offset.NormalBackColor = System.Drawing.Color.White;
            this.txt_Offset.TextChanged += new System.EventHandler(this.txt_Offset_TextChanged);
            // 
            // lbl_Offset
            // 
            resources.ApplyResources(this.lbl_Offset, "lbl_Offset");
            this.lbl_Offset.Name = "lbl_Offset";
            this.lbl_Offset.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_Offset
            // 
            resources.ApplyResources(this.trackBar_Offset, "trackBar_Offset");
            this.trackBar_Offset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Offset.LargeChange = 1;
            this.trackBar_Offset.Maximum = 100;
            this.trackBar_Offset.Minimum = -100;
            this.trackBar_Offset.Name = "trackBar_Offset";
            this.trackBar_Offset.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Offset.Scroll += new System.EventHandler(this.trackBar_Offset_Scroll);
            // 
            // pnl_Manual
            // 
            resources.ApplyResources(this.pnl_Manual, "pnl_Manual");
            this.pnl_Manual.Controls.Add(this.srmLabel7);
            this.pnl_Manual.Controls.Add(this.pictureBox10);
            this.pnl_Manual.Controls.Add(this.pictureBox8);
            this.pnl_Manual.Controls.Add(this.pictureBox11);
            this.pnl_Manual.Controls.Add(this.btn_UndoROI);
            this.pnl_Manual.Controls.Add(this.pictureBox12);
            this.pnl_Manual.Controls.Add(this.srmLabel8);
            this.pnl_Manual.Name = "pnl_Manual";
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pictureBox10
            // 
            resources.ApplyResources(this.pictureBox10, "pictureBox10");
            this.pictureBox10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox8
            // 
            resources.ApplyResources(this.pictureBox8, "pictureBox8");
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox11
            // 
            resources.ApplyResources(this.pictureBox11, "pictureBox11");
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.TabStop = false;
            // 
            // btn_UndoROI
            // 
            resources.ApplyResources(this.btn_UndoROI, "btn_UndoROI");
            this.btn_UndoROI.Name = "btn_UndoROI";
            this.btn_UndoROI.UseVisualStyleBackColor = true;
            this.btn_UndoROI.Click += new System.EventHandler(this.btn_UndoROI_Click);
            // 
            // pictureBox12
            // 
            resources.ApplyResources(this.pictureBox12, "pictureBox12");
            this.pictureBox12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.TabStop = false;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // chk_Auto
            // 
            resources.ApplyResources(this.chk_Auto, "chk_Auto");
            this.chk_Auto.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Auto.Name = "chk_Auto";
            this.chk_Auto.Selected = false;
            this.chk_Auto.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Auto.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Auto.UseVisualStyleBackColor = true;
            this.chk_Auto.Click += new System.EventHandler(this.chk_Auto_Click);
            // 
            // DontCareAreaSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.pnl_Production);
            this.Controls.Add(this.pnl_PictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DontCareAreaSettingForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DontCareAreaSettingForm_FormClosing);
            this.pnl_PictureBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Image)).EndInit();
            this.pnl_Production.ResumeLayout(false);
            this.pnl_Auto.ResumeLayout(false);
            this.pnl_Auto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Offset)).EndInit();
            this.pnl_Manual.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl_PictureBox;
        private System.Windows.Forms.PictureBox pic_Image;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnl_Production;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.PictureBox pictureBox12;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMButton btn_UndoROI;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox10;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMButton btn_Cancel;
        private System.Windows.Forms.Panel pnl_PicSideBlock;
        private System.Windows.Forms.Panel pnl_Auto;
        private System.Windows.Forms.Panel pnl_Manual;
        private SRMControl.SRMCheckBox chk_Auto;
        private SRMControl.SRMInputBox txt_Offset;
        private SRMControl.SRMLabel lbl_Offset;
        private System.Windows.Forms.TrackBar trackBar_Offset;
        private SRMControl.SRMInputBox txt_Threshold;
        private System.Windows.Forms.TrackBar trackBar_Threshold;
        private SRMControl.SRMLabel lbl_Threshold;
        private SRMControl.SRMLabel srmLabel1;
    }
}