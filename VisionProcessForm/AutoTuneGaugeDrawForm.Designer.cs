namespace VisionProcessForm
{
    partial class AutoTuneGaugeDrawForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoTuneGaugeDrawForm));
            this.pnl_Production = new System.Windows.Forms.Panel();
            this.lbl_SelectImage = new SRMControl.SRMLabel();
            this.cbo_ImageNo = new SRMControl.SRMComboBox();
            this.chk_AutoPlace = new SRMControl.SRMCheckBox();
            this.chk_SetToAll = new SRMControl.SRMCheckBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.btn_OK = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.btn_Undo = new SRMControl.SRMButton();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.pnl_PictureBox = new System.Windows.Forms.Panel();
            this.pic_Image = new System.Windows.Forms.PictureBox();
            this.pnl_PicSideBlock = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnl_Production.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            this.pnl_PictureBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // pnl_Production
            // 
            resources.ApplyResources(this.pnl_Production, "pnl_Production");
            this.pnl_Production.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pnl_Production.Controls.Add(this.lbl_SelectImage);
            this.pnl_Production.Controls.Add(this.cbo_ImageNo);
            this.pnl_Production.Controls.Add(this.chk_AutoPlace);
            this.pnl_Production.Controls.Add(this.chk_SetToAll);
            this.pnl_Production.Controls.Add(this.srmLabel3);
            this.pnl_Production.Controls.Add(this.trackBar1);
            this.pnl_Production.Controls.Add(this.srmLabel2);
            this.pnl_Production.Controls.Add(this.srmLabel1);
            this.pnl_Production.Controls.Add(this.btn_OK);
            this.pnl_Production.Controls.Add(this.btn_Cancel);
            this.pnl_Production.Controls.Add(this.pictureBox11);
            this.pnl_Production.Controls.Add(this.pictureBox12);
            this.pnl_Production.Controls.Add(this.btn_Undo);
            this.pnl_Production.Controls.Add(this.pictureBox8);
            this.pnl_Production.Controls.Add(this.pictureBox10);
            this.pnl_Production.Controls.Add(this.srmLabel7);
            this.pnl_Production.Name = "pnl_Production";
            // 
            // lbl_SelectImage
            // 
            resources.ApplyResources(this.lbl_SelectImage, "lbl_SelectImage");
            this.lbl_SelectImage.Name = "lbl_SelectImage";
            this.lbl_SelectImage.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_ImageNo
            // 
            resources.ApplyResources(this.cbo_ImageNo, "cbo_ImageNo");
            this.cbo_ImageNo.BackColor = System.Drawing.Color.White;
            this.cbo_ImageNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ImageNo.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ImageNo.FormattingEnabled = true;
            this.cbo_ImageNo.Items.AddRange(new object[] {
            resources.GetString("cbo_ImageNo.Items"),
            resources.GetString("cbo_ImageNo.Items1"),
            resources.GetString("cbo_ImageNo.Items2")});
            this.cbo_ImageNo.Name = "cbo_ImageNo";
            this.cbo_ImageNo.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ImageNo.SelectedIndexChanged += new System.EventHandler(this.cbo_ImageNo_SelectedIndexChanged);
            // 
            // chk_AutoPlace
            // 
            resources.ApplyResources(this.chk_AutoPlace, "chk_AutoPlace");
            this.chk_AutoPlace.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_AutoPlace.Name = "chk_AutoPlace";
            this.chk_AutoPlace.Selected = false;
            this.chk_AutoPlace.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_AutoPlace.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_AutoPlace.UseVisualStyleBackColor = true;
            // 
            // chk_SetToAll
            // 
            resources.ApplyResources(this.chk_SetToAll, "chk_SetToAll");
            this.chk_SetToAll.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToAll.Name = "chk_SetToAll";
            this.chk_SetToAll.Selected = false;
            this.chk_SetToAll.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAll.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAll.UseVisualStyleBackColor = true;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar1
            // 
            resources.ApplyResources(this.trackBar1, "trackBar1");
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Maximum = 50;
            this.trackBar1.Minimum = 10;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 10;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // pictureBox11
            // 
            resources.ApplyResources(this.pictureBox11, "pictureBox11");
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.TabStop = false;
            // 
            // pictureBox12
            // 
            resources.ApplyResources(this.pictureBox12, "pictureBox12");
            this.pictureBox12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.TabStop = false;
            // 
            // btn_Undo
            // 
            resources.ApplyResources(this.btn_Undo, "btn_Undo");
            this.btn_Undo.Name = "btn_Undo";
            this.btn_Undo.UseVisualStyleBackColor = true;
            this.btn_Undo.Click += new System.EventHandler(this.btn_Undo_Click);
            // 
            // pictureBox8
            // 
            resources.ApplyResources(this.pictureBox8, "pictureBox8");
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox10
            // 
            resources.ApplyResources(this.pictureBox10, "pictureBox10");
            this.pictureBox10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.TabStop = false;
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
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
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // AutoTuneGaugeDrawForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.pnl_PictureBox);
            this.Controls.Add(this.pnl_Production);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoTuneGaugeDrawForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoTuneGaugeDrawForm_FormClosing);
            this.pnl_Production.ResumeLayout(false);
            this.pnl_Production.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            this.pnl_PictureBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl_Production;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMButton btn_Cancel;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.PictureBox pictureBox12;
        private SRMControl.SRMButton btn_Undo;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox10;
        private SRMControl.SRMLabel srmLabel7;
        private System.Windows.Forms.Panel pnl_PictureBox;
        private System.Windows.Forms.PictureBox pic_Image;
        private System.Windows.Forms.Timer timer1;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel1;
        private System.Windows.Forms.Panel pnl_PicSideBlock;
        private System.Windows.Forms.TrackBar trackBar1;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMCheckBox chk_AutoPlace;
        private SRMControl.SRMCheckBox chk_SetToAll;
        private SRMControl.SRMComboBox cbo_ImageNo;
        private SRMControl.SRMLabel lbl_SelectImage;
    }
}