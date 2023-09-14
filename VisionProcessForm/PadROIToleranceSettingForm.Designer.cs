namespace VisionProcessForm
{
    partial class PadROIToleranceSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PadROIToleranceSettingForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.panel_SelectEdge = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnl_Bottom = new System.Windows.Forms.Panel();
            this.srmLabel36 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromBottom = new SRMControl.SRMInputBox();
            this.pnl_Right = new System.Windows.Forms.Panel();
            this.srmLabel34 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromRight = new SRMControl.SRMInputBox();
            this.pnl_Left = new System.Windows.Forms.Panel();
            this.srmLabel38 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromLeft = new SRMControl.SRMInputBox();
            this.pnl_Top = new System.Windows.Forms.Panel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_StartPixelFromTop = new SRMControl.SRMInputBox();
            this.picUnitROI = new System.Windows.Forms.PictureBox();
            this.chk_SetToAllEdge = new SRMControl.SRMCheckBox();
            this.chk_SetToAllSideROI = new SRMControl.SRMCheckBox();
            this.cbo_ROI = new SRMControl.SRMComboBox();
            this.lbl_ROI = new SRMControl.SRMLabel();
            this.ils_ImageListTree = new System.Windows.Forms.ImageList(this.components);
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.panel_SelectEdge.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnl_Bottom.SuspendLayout();
            this.pnl_Right.SuspendLayout();
            this.pnl_Left.SuspendLayout();
            this.pnl_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitROI)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // panel_SelectEdge
            // 
            this.panel_SelectEdge.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.panel_SelectEdge, "panel_SelectEdge");
            this.panel_SelectEdge.Name = "panel_SelectEdge";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnl_Bottom);
            this.groupBox1.Controls.Add(this.pnl_Right);
            this.groupBox1.Controls.Add(this.pnl_Left);
            this.groupBox1.Controls.Add(this.pnl_Top);
            this.groupBox1.Controls.Add(this.picUnitROI);
            this.groupBox1.Controls.Add(this.chk_SetToAllEdge);
            this.groupBox1.Controls.Add(this.chk_SetToAllSideROI);
            this.groupBox1.Controls.Add(this.cbo_ROI);
            this.groupBox1.Controls.Add(this.lbl_ROI);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // pnl_Bottom
            // 
            this.pnl_Bottom.Controls.Add(this.srmLabel36);
            this.pnl_Bottom.Controls.Add(this.txt_StartPixelFromBottom);
            resources.ApplyResources(this.pnl_Bottom, "pnl_Bottom");
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
            resources.ApplyResources(this.txt_StartPixelFromBottom, "txt_StartPixelFromBottom");
            this.txt_StartPixelFromBottom.Name = "txt_StartPixelFromBottom";
            this.txt_StartPixelFromBottom.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromBottom.TextChanged += new System.EventHandler(this.txt_StartPixelFromBottom_TextChanged);
            this.txt_StartPixelFromBottom.Enter += new System.EventHandler(this.txt_StartPixel_Enter);
            this.txt_StartPixelFromBottom.Leave += new System.EventHandler(this.txt_StartPixel_Leave);
            // 
            // pnl_Right
            // 
            this.pnl_Right.Controls.Add(this.srmLabel34);
            this.pnl_Right.Controls.Add(this.txt_StartPixelFromRight);
            resources.ApplyResources(this.pnl_Right, "pnl_Right");
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
            resources.ApplyResources(this.txt_StartPixelFromRight, "txt_StartPixelFromRight");
            this.txt_StartPixelFromRight.Name = "txt_StartPixelFromRight";
            this.txt_StartPixelFromRight.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromRight.TextChanged += new System.EventHandler(this.txt_StartPixelFromRight_TextChanged);
            this.txt_StartPixelFromRight.Enter += new System.EventHandler(this.txt_StartPixel_Enter);
            this.txt_StartPixelFromRight.Leave += new System.EventHandler(this.txt_StartPixel_Leave);
            // 
            // pnl_Left
            // 
            this.pnl_Left.Controls.Add(this.srmLabel38);
            this.pnl_Left.Controls.Add(this.txt_StartPixelFromLeft);
            resources.ApplyResources(this.pnl_Left, "pnl_Left");
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
            resources.ApplyResources(this.txt_StartPixelFromLeft, "txt_StartPixelFromLeft");
            this.txt_StartPixelFromLeft.Name = "txt_StartPixelFromLeft";
            this.txt_StartPixelFromLeft.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromLeft.TextChanged += new System.EventHandler(this.txt_StartPixelFromLeft_TextChanged);
            this.txt_StartPixelFromLeft.Enter += new System.EventHandler(this.txt_StartPixel_Enter);
            this.txt_StartPixelFromLeft.Leave += new System.EventHandler(this.txt_StartPixel_Leave);
            // 
            // pnl_Top
            // 
            this.pnl_Top.Controls.Add(this.srmLabel1);
            this.pnl_Top.Controls.Add(this.txt_StartPixelFromTop);
            resources.ApplyResources(this.pnl_Top, "pnl_Top");
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
            resources.ApplyResources(this.txt_StartPixelFromTop, "txt_StartPixelFromTop");
            this.txt_StartPixelFromTop.Name = "txt_StartPixelFromTop";
            this.txt_StartPixelFromTop.NormalBackColor = System.Drawing.Color.White;
            this.txt_StartPixelFromTop.TextChanged += new System.EventHandler(this.txt_StartPixelFromTop_TextChanged);
            this.txt_StartPixelFromTop.Enter += new System.EventHandler(this.txt_StartPixel_Enter);
            this.txt_StartPixelFromTop.Leave += new System.EventHandler(this.txt_StartPixel_Leave);
            // 
            // picUnitROI
            // 
            this.picUnitROI.BackColor = System.Drawing.Color.Black;
            this.picUnitROI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.picUnitROI, "picUnitROI");
            this.picUnitROI.Name = "picUnitROI";
            this.picUnitROI.TabStop = false;
            // 
            // chk_SetToAllEdge
            // 
            this.chk_SetToAllEdge.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_SetToAllEdge, "chk_SetToAllEdge");
            this.chk_SetToAllEdge.Name = "chk_SetToAllEdge";
            this.chk_SetToAllEdge.Selected = false;
            this.chk_SetToAllEdge.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllEdge.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllEdge.UseVisualStyleBackColor = true;
            this.chk_SetToAllEdge.Click += new System.EventHandler(this.chk_SetToAllEdges_Click);
            // 
            // chk_SetToAllSideROI
            // 
            this.chk_SetToAllSideROI.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_SetToAllSideROI, "chk_SetToAllSideROI");
            this.chk_SetToAllSideROI.Name = "chk_SetToAllSideROI";
            this.chk_SetToAllSideROI.Selected = false;
            this.chk_SetToAllSideROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllSideROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllSideROI.UseVisualStyleBackColor = true;
            this.chk_SetToAllSideROI.Click += new System.EventHandler(this.chk_SetToAllSideROI_Click);
            // 
            // cbo_ROI
            // 
            this.cbo_ROI.BackColor = System.Drawing.Color.White;
            this.cbo_ROI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ROI.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.cbo_ROI, "cbo_ROI");
            this.cbo_ROI.FormattingEnabled = true;
            this.cbo_ROI.Name = "cbo_ROI";
            this.cbo_ROI.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ROI.SelectedIndexChanged += new System.EventHandler(this.cbo_ROI_SelectedIndexChanged);
            // 
            // lbl_ROI
            // 
            resources.ApplyResources(this.lbl_ROI, "lbl_ROI");
            this.lbl_ROI.Name = "lbl_ROI";
            this.lbl_ROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // ils_ImageListTree
            // 
            this.ils_ImageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ils_ImageListTree.ImageStream")));
            this.ils_ImageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.ils_ImageListTree.Images.SetKeyName(0, "5S Center Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(1, "5S Top Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(2, "5S Right Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(3, "5S Bottom Pkg ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(4, "5S Left Pkg ROI.png");
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            // 
            // PadROIToleranceSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel_SelectEdge);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PadROIToleranceSettingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PadROIToleranceSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.PadROIToleranceSettingForm_Load);
            this.panel_SelectEdge.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnl_Bottom.ResumeLayout(false);
            this.pnl_Bottom.PerformLayout();
            this.pnl_Right.ResumeLayout(false);
            this.pnl_Right.PerformLayout();
            this.pnl_Left.ResumeLayout(false);
            this.pnl_Left.PerformLayout();
            this.pnl_Top.ResumeLayout(false);
            this.pnl_Top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitROI)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.Panel panel_SelectEdge;
        private System.Windows.Forms.GroupBox groupBox1;
        private SRMControl.SRMCheckBox chk_SetToAllSideROI;
        private SRMControl.SRMComboBox cbo_ROI;
        private SRMControl.SRMLabel lbl_ROI;
        private SRMControl.SRMCheckBox chk_SetToAllEdge;
        private System.Windows.Forms.PictureBox picUnitROI;
        private System.Windows.Forms.ImageList ils_ImageListTree;
        private System.Windows.Forms.Timer Timer;
        private SRMControl.SRMInputBox txt_StartPixelFromLeft;
        private SRMControl.SRMLabel srmLabel38;
        private SRMControl.SRMInputBox txt_StartPixelFromRight;
        private SRMControl.SRMInputBox txt_StartPixelFromBottom;
        private SRMControl.SRMInputBox txt_StartPixelFromTop;
        private SRMControl.SRMLabel srmLabel34;
        private SRMControl.SRMLabel srmLabel36;
        private SRMControl.SRMLabel srmLabel1;
        private System.Windows.Forms.Panel pnl_Bottom;
        private System.Windows.Forms.Panel pnl_Right;
        private System.Windows.Forms.Panel pnl_Left;
        private System.Windows.Forms.Panel pnl_Top;
    }
}