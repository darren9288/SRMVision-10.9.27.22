namespace VisionProcessForm
{
    partial class PadSubSettingForm2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PadSubSettingForm2));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.panel_SelectEdge = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.chk_UseBorderLimit_Center = new SRMControl.SRMCheckBox();
            this.chk_UseBorderLimit_Top = new SRMControl.SRMCheckBox();
            this.chk_UseBorderLimit_Left = new SRMControl.SRMCheckBox();
            this.chk_UseBorderLimit_Right = new SRMControl.SRMCheckBox();
            this.chk_UseBorderLimit_Bottom = new SRMControl.SRMCheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.ils_ImageListTree = new System.Windows.Forms.ImageList(this.components);
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.panel_SelectEdge.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            this.btn_OK.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // panel_SelectEdge
            // 
            resources.ApplyResources(this.panel_SelectEdge, "panel_SelectEdge");
            this.panel_SelectEdge.Controls.Add(this.panel2);
            this.panel_SelectEdge.Controls.Add(this.panel1);
            this.panel_SelectEdge.Name = "panel_SelectEdge";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.srmLabel13);
            this.panel2.Controls.Add(this.chk_UseBorderLimit_Center);
            this.panel2.Controls.Add(this.chk_UseBorderLimit_Top);
            this.panel2.Controls.Add(this.chk_UseBorderLimit_Left);
            this.panel2.Controls.Add(this.chk_UseBorderLimit_Right);
            this.panel2.Controls.Add(this.chk_UseBorderLimit_Bottom);
            this.panel2.Name = "panel2";
            // 
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_UseBorderLimit_Center
            // 
            resources.ApplyResources(this.chk_UseBorderLimit_Center, "chk_UseBorderLimit_Center");
            this.chk_UseBorderLimit_Center.Checked = true;
            this.chk_UseBorderLimit_Center.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_UseBorderLimit_Center.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_UseBorderLimit_Center.Name = "chk_UseBorderLimit_Center";
            this.chk_UseBorderLimit_Center.Selected = false;
            this.chk_UseBorderLimit_Center.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Center.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Center.UseVisualStyleBackColor = true;
            // 
            // chk_UseBorderLimit_Top
            // 
            resources.ApplyResources(this.chk_UseBorderLimit_Top, "chk_UseBorderLimit_Top");
            this.chk_UseBorderLimit_Top.Checked = true;
            this.chk_UseBorderLimit_Top.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_UseBorderLimit_Top.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_UseBorderLimit_Top.Name = "chk_UseBorderLimit_Top";
            this.chk_UseBorderLimit_Top.Selected = false;
            this.chk_UseBorderLimit_Top.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Top.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Top.UseVisualStyleBackColor = true;
            this.chk_UseBorderLimit_Top.CheckedChanged += new System.EventHandler(this.srmCheckBox1_CheckedChanged);
            // 
            // chk_UseBorderLimit_Left
            // 
            resources.ApplyResources(this.chk_UseBorderLimit_Left, "chk_UseBorderLimit_Left");
            this.chk_UseBorderLimit_Left.Checked = true;
            this.chk_UseBorderLimit_Left.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_UseBorderLimit_Left.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_UseBorderLimit_Left.Name = "chk_UseBorderLimit_Left";
            this.chk_UseBorderLimit_Left.Selected = false;
            this.chk_UseBorderLimit_Left.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Left.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Left.UseVisualStyleBackColor = true;
            // 
            // chk_UseBorderLimit_Right
            // 
            resources.ApplyResources(this.chk_UseBorderLimit_Right, "chk_UseBorderLimit_Right");
            this.chk_UseBorderLimit_Right.Checked = true;
            this.chk_UseBorderLimit_Right.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_UseBorderLimit_Right.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_UseBorderLimit_Right.Name = "chk_UseBorderLimit_Right";
            this.chk_UseBorderLimit_Right.Selected = false;
            this.chk_UseBorderLimit_Right.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Right.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Right.UseVisualStyleBackColor = true;
            // 
            // chk_UseBorderLimit_Bottom
            // 
            resources.ApplyResources(this.chk_UseBorderLimit_Bottom, "chk_UseBorderLimit_Bottom");
            this.chk_UseBorderLimit_Bottom.Checked = true;
            this.chk_UseBorderLimit_Bottom.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_UseBorderLimit_Bottom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_UseBorderLimit_Bottom.Name = "chk_UseBorderLimit_Bottom";
            this.chk_UseBorderLimit_Bottom.Selected = false;
            this.chk_UseBorderLimit_Bottom.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Bottom.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_UseBorderLimit_Bottom.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.srmLabel3);
            this.panel1.Controls.Add(this.srmLabel5);
            this.panel1.Controls.Add(this.srmLabel7);
            this.panel1.Controls.Add(this.srmLabel9);
            this.panel1.Controls.Add(this.srmLabel1);
            this.panel1.Name = "panel1";
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.BackColor = System.Drawing.Color.Transparent;
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.BackColor = System.Drawing.Color.Transparent;
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.BackColor = System.Drawing.Color.Transparent;
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.BackColor = System.Drawing.Color.Transparent;
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.BackColor = System.Drawing.Color.Transparent;
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
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
            // PadSubSettingForm2
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
            this.Name = "PadSubSettingForm2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.panel_SelectEdge.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.Panel panel_SelectEdge;
        private System.Windows.Forms.ImageList ils_ImageListTree;
        private System.Windows.Forms.Timer Timer;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel13;
        private SRMControl.SRMCheckBox chk_UseBorderLimit_Left;
        private SRMControl.SRMCheckBox chk_UseBorderLimit_Bottom;
        private SRMControl.SRMCheckBox chk_UseBorderLimit_Right;
        private SRMControl.SRMCheckBox chk_UseBorderLimit_Top;
        private SRMControl.SRMCheckBox chk_UseBorderLimit_Center;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
    }
}