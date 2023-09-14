namespace VisionProcessForm
{
    partial class RectGaugeM4LImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RectGaugeM4LImageForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_Center = new System.Windows.Forms.Panel();
            this.txt_CenterROI_Right = new System.Windows.Forms.NumericUpDown();
            this.txt_CenterROI_Bottom = new System.Windows.Forms.NumericUpDown();
            this.txt_CenterROI_Top = new System.Windows.Forms.NumericUpDown();
            this.txt_CenterROI_Left = new System.Windows.Forms.NumericUpDown();
            this.panel10 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.pnl_Button = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.pnl_Center.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterROI_Right)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterROI_Bottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterROI_Top)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterROI_Left)).BeginInit();
            this.pnl_Button.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.pnl_Center);
            this.panel1.Name = "panel1";
            // 
            // pnl_Center
            // 
            resources.ApplyResources(this.pnl_Center, "pnl_Center");
            this.pnl_Center.BackColor = System.Drawing.Color.Black;
            this.pnl_Center.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Center.Controls.Add(this.txt_CenterROI_Right);
            this.pnl_Center.Controls.Add(this.txt_CenterROI_Bottom);
            this.pnl_Center.Controls.Add(this.txt_CenterROI_Top);
            this.pnl_Center.Controls.Add(this.txt_CenterROI_Left);
            this.pnl_Center.Controls.Add(this.panel10);
            this.pnl_Center.Name = "pnl_Center";
            // 
            // txt_CenterROI_Right
            // 
            resources.ApplyResources(this.txt_CenterROI_Right, "txt_CenterROI_Right");
            this.txt_CenterROI_Right.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.txt_CenterROI_Right.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_CenterROI_Right.Name = "txt_CenterROI_Right";
            this.txt_CenterROI_Right.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txt_CenterROI_Bottom
            // 
            resources.ApplyResources(this.txt_CenterROI_Bottom, "txt_CenterROI_Bottom");
            this.txt_CenterROI_Bottom.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.txt_CenterROI_Bottom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_CenterROI_Bottom.Name = "txt_CenterROI_Bottom";
            this.txt_CenterROI_Bottom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txt_CenterROI_Top
            // 
            resources.ApplyResources(this.txt_CenterROI_Top, "txt_CenterROI_Top");
            this.txt_CenterROI_Top.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.txt_CenterROI_Top.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_CenterROI_Top.Name = "txt_CenterROI_Top";
            this.txt_CenterROI_Top.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txt_CenterROI_Left
            // 
            resources.ApplyResources(this.txt_CenterROI_Left, "txt_CenterROI_Left");
            this.txt_CenterROI_Left.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.txt_CenterROI_Left.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_CenterROI_Left.Name = "txt_CenterROI_Left";
            this.txt_CenterROI_Left.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // panel10
            // 
            resources.ApplyResources(this.panel10, "panel10");
            this.panel10.Name = "panel10";
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
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // pnl_Button
            // 
            resources.ApplyResources(this.pnl_Button, "pnl_Button");
            this.pnl_Button.Controls.Add(this.btn_OK);
            this.pnl_Button.Controls.Add(this.btn_Cancel);
            this.pnl_Button.Name = "pnl_Button";
            // 
            // RectGaugeM4LImageForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.pnl_Button);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RectGaugeM4LImageForm";
            this.ShowInTaskbar = false;
            this.panel1.ResumeLayout(false);
            this.pnl_Center.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterROI_Right)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterROI_Bottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterROI_Top)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterROI_Left)).EndInit();
            this.pnl_Button.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnl_Center;
        private System.Windows.Forms.NumericUpDown txt_CenterROI_Right;
        private System.Windows.Forms.NumericUpDown txt_CenterROI_Bottom;
        private System.Windows.Forms.NumericUpDown txt_CenterROI_Top;
        private System.Windows.Forms.NumericUpDown txt_CenterROI_Left;
        private System.Windows.Forms.Panel panel10;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.Panel pnl_Button;
    }
}