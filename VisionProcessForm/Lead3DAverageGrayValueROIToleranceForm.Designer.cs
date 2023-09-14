namespace VisionProcessForm
{
    partial class Lead3DAverageGrayValueROIToleranceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Lead3DAverageGrayValueROIToleranceForm));
            this.pic_Lead = new System.Windows.Forms.PictureBox();
            this.pnl_Top = new System.Windows.Forms.Panel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromTop = new SRMControl.SRMInputBox();
            this.pnl_Left = new System.Windows.Forms.Panel();
            this.srmLabel38 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromLeft = new SRMControl.SRMInputBox();
            this.pnl_Right = new System.Windows.Forms.Panel();
            this.srmLabel34 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromRight = new SRMControl.SRMInputBox();
            this.pnl_Bottom = new System.Windows.Forms.Panel();
            this.srmLabel36 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromBottom = new SRMControl.SRMInputBox();
            this.chk_SetToAllEdge = new SRMControl.SRMCheckBox();
            this.chk_SetToAllLead = new SRMControl.SRMCheckBox();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cbo_LeadNo = new SRMControl.SRMComboBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Lead)).BeginInit();
            this.pnl_Top.SuspendLayout();
            this.pnl_Left.SuspendLayout();
            this.pnl_Right.SuspendLayout();
            this.pnl_Bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pic_Lead
            // 
            resources.ApplyResources(this.pic_Lead, "pic_Lead");
            this.pic_Lead.Name = "pic_Lead";
            this.pic_Lead.TabStop = false;
            // 
            // pnl_Top
            // 
            resources.ApplyResources(this.pnl_Top, "pnl_Top");
            this.pnl_Top.Controls.Add(this.srmLabel1);
            this.pnl_Top.Controls.Add(this.txt_StartPixelFromTop);
            this.pnl_Top.Name = "pnl_Top";
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_StartPixelFromTop
            // 
            resources.ApplyResources(this.txt_StartPixelFromTop, "txt_StartPixelFromTop");
            this.txt_StartPixelFromTop.BackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromTop.DecimalPlaces = 0;
            this.txt_StartPixelFromTop.DecMaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.txt_StartPixelFromTop.DecMinValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.txt_StartPixelFromTop.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_StartPixelFromTop.ForeColor = System.Drawing.Color.Black;
            this.txt_StartPixelFromTop.InputType = SRMControl.InputType.Number;
            this.txt_StartPixelFromTop.Name = "txt_StartPixelFromTop";
            this.txt_StartPixelFromTop.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromTop.TextChanged += new System.EventHandler(this.txt_StartPixelFromTop_TextChanged);
            this.txt_StartPixelFromTop.Enter += new System.EventHandler(this.txt_StartPixel_Enter);
            // 
            // pnl_Left
            // 
            resources.ApplyResources(this.pnl_Left, "pnl_Left");
            this.pnl_Left.Controls.Add(this.srmLabel38);
            this.pnl_Left.Controls.Add(this.txt_StartPixelFromLeft);
            this.pnl_Left.Name = "pnl_Left";
            // 
            // srmLabel38
            // 
            resources.ApplyResources(this.srmLabel38, "srmLabel38");
            this.srmLabel38.Name = "srmLabel38";
            this.srmLabel38.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_StartPixelFromLeft
            // 
            resources.ApplyResources(this.txt_StartPixelFromLeft, "txt_StartPixelFromLeft");
            this.txt_StartPixelFromLeft.BackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromLeft.DecimalPlaces = 0;
            this.txt_StartPixelFromLeft.DecMaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.txt_StartPixelFromLeft.DecMinValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.txt_StartPixelFromLeft.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_StartPixelFromLeft.ForeColor = System.Drawing.Color.Black;
            this.txt_StartPixelFromLeft.InputType = SRMControl.InputType.Number;
            this.txt_StartPixelFromLeft.Name = "txt_StartPixelFromLeft";
            this.txt_StartPixelFromLeft.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromLeft.TextChanged += new System.EventHandler(this.txt_StartPixelFromLeft_TextChanged);
            this.txt_StartPixelFromLeft.Enter += new System.EventHandler(this.txt_StartPixel_Enter);
            // 
            // pnl_Right
            // 
            resources.ApplyResources(this.pnl_Right, "pnl_Right");
            this.pnl_Right.Controls.Add(this.srmLabel34);
            this.pnl_Right.Controls.Add(this.txt_StartPixelFromRight);
            this.pnl_Right.Name = "pnl_Right";
            // 
            // srmLabel34
            // 
            resources.ApplyResources(this.srmLabel34, "srmLabel34");
            this.srmLabel34.Name = "srmLabel34";
            this.srmLabel34.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_StartPixelFromRight
            // 
            resources.ApplyResources(this.txt_StartPixelFromRight, "txt_StartPixelFromRight");
            this.txt_StartPixelFromRight.BackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromRight.DecimalPlaces = 0;
            this.txt_StartPixelFromRight.DecMaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.txt_StartPixelFromRight.DecMinValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.txt_StartPixelFromRight.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_StartPixelFromRight.ForeColor = System.Drawing.Color.Black;
            this.txt_StartPixelFromRight.InputType = SRMControl.InputType.Number;
            this.txt_StartPixelFromRight.Name = "txt_StartPixelFromRight";
            this.txt_StartPixelFromRight.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromRight.TextChanged += new System.EventHandler(this.txt_StartPixelFromRight_TextChanged);
            this.txt_StartPixelFromRight.Enter += new System.EventHandler(this.txt_StartPixel_Enter);
            // 
            // pnl_Bottom
            // 
            resources.ApplyResources(this.pnl_Bottom, "pnl_Bottom");
            this.pnl_Bottom.Controls.Add(this.srmLabel36);
            this.pnl_Bottom.Controls.Add(this.txt_StartPixelFromBottom);
            this.pnl_Bottom.Name = "pnl_Bottom";
            // 
            // srmLabel36
            // 
            resources.ApplyResources(this.srmLabel36, "srmLabel36");
            this.srmLabel36.Name = "srmLabel36";
            this.srmLabel36.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_StartPixelFromBottom
            // 
            resources.ApplyResources(this.txt_StartPixelFromBottom, "txt_StartPixelFromBottom");
            this.txt_StartPixelFromBottom.BackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromBottom.DecimalPlaces = 0;
            this.txt_StartPixelFromBottom.DecMaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.txt_StartPixelFromBottom.DecMinValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.txt_StartPixelFromBottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_StartPixelFromBottom.ForeColor = System.Drawing.Color.Black;
            this.txt_StartPixelFromBottom.InputType = SRMControl.InputType.Number;
            this.txt_StartPixelFromBottom.Name = "txt_StartPixelFromBottom";
            this.txt_StartPixelFromBottom.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromBottom.TextChanged += new System.EventHandler(this.txt_StartPixelFromBottom_TextChanged);
            this.txt_StartPixelFromBottom.Enter += new System.EventHandler(this.txt_StartPixel_Enter);
            // 
            // chk_SetToAllEdge
            // 
            resources.ApplyResources(this.chk_SetToAllEdge, "chk_SetToAllEdge");
            this.chk_SetToAllEdge.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToAllEdge.Name = "chk_SetToAllEdge";
            this.chk_SetToAllEdge.Selected = false;
            this.chk_SetToAllEdge.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllEdge.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllEdge.UseVisualStyleBackColor = true;
            // 
            // chk_SetToAllLead
            // 
            resources.ApplyResources(this.chk_SetToAllLead, "chk_SetToAllLead");
            this.chk_SetToAllLead.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToAllLead.Name = "chk_SetToAllLead";
            this.chk_SetToAllLead.Selected = false;
            this.chk_SetToAllLead.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllLead.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllLead.UseVisualStyleBackColor = true;
            this.chk_SetToAllLead.Click += new System.EventHandler(this.chk_SetToAllLead_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Lead Top.bmp");
            this.imageList1.Images.SetKeyName(1, "Lead Bottom.bmp");
            this.imageList1.Images.SetKeyName(2, "Lead Left.bmp");
            this.imageList1.Images.SetKeyName(3, "Lead Right.bmp");
            // 
            // cbo_LeadNo
            // 
            resources.ApplyResources(this.cbo_LeadNo, "cbo_LeadNo");
            this.cbo_LeadNo.BackColor = System.Drawing.Color.White;
            this.cbo_LeadNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_LeadNo.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_LeadNo.FormattingEnabled = true;
            this.cbo_LeadNo.Name = "cbo_LeadNo";
            this.cbo_LeadNo.NormalBackColor = System.Drawing.Color.White;
            this.cbo_LeadNo.SelectedIndexChanged += new System.EventHandler(this.cbo_LeadNo_SelectedIndexChanged);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Lead3DAverageGrayValueROIToleranceForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.cbo_LeadNo);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.chk_SetToAllEdge);
            this.Controls.Add(this.chk_SetToAllLead);
            this.Controls.Add(this.pnl_Bottom);
            this.Controls.Add(this.pnl_Right);
            this.Controls.Add(this.pnl_Left);
            this.Controls.Add(this.pnl_Top);
            this.Controls.Add(this.pic_Lead);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Lead3DAverageGrayValueROIToleranceForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Lead3DAverageGrayValueROIToleranceForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Lead)).EndInit();
            this.pnl_Top.ResumeLayout(false);
            this.pnl_Top.PerformLayout();
            this.pnl_Left.ResumeLayout(false);
            this.pnl_Left.PerformLayout();
            this.pnl_Right.ResumeLayout(false);
            this.pnl_Right.PerformLayout();
            this.pnl_Bottom.ResumeLayout(false);
            this.pnl_Bottom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pic_Lead;
        private System.Windows.Forms.Panel pnl_Top;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_StartPixelFromTop;
        private System.Windows.Forms.Panel pnl_Left;
        private SRMControl.SRMLabel srmLabel38;
        private SRMControl.SRMInputBox txt_StartPixelFromLeft;
        private System.Windows.Forms.Panel pnl_Right;
        private SRMControl.SRMLabel srmLabel34;
        private SRMControl.SRMInputBox txt_StartPixelFromRight;
        private System.Windows.Forms.Panel pnl_Bottom;
        private SRMControl.SRMLabel srmLabel36;
        private SRMControl.SRMInputBox txt_StartPixelFromBottom;
        private SRMControl.SRMCheckBox chk_SetToAllEdge;
        private SRMControl.SRMCheckBox chk_SetToAllLead;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.ImageList imageList1;
        private SRMControl.SRMComboBox cbo_LeadNo;
        private SRMControl.SRMLabel srmLabel2;
        private System.Windows.Forms.Timer timer1;
    }
}