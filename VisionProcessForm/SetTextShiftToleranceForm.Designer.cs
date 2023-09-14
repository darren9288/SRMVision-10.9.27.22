namespace VisionProcessForm
{
    partial class SetTextShiftToleranceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetTextShiftToleranceForm));
            this.txt_SetYValue = new SRMControl.SRMInputBox();
            this.txt_SetXValue = new SRMControl.SRMInputBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.lbl_SetValue = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.lbl_Definition = new SRMControl.SRMLabel();
            this.btn_OK = new SRMControl.SRMButton();
            this.SuspendLayout();
            // 
            // txt_SetYValue
            // 
            resources.ApplyResources(this.txt_SetYValue, "txt_SetYValue");
            this.txt_SetYValue.BackColor = System.Drawing.Color.White;
            this.txt_SetYValue.DecimalPlaces = 0;
            this.txt_SetYValue.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_SetYValue.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SetYValue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SetYValue.ForeColor = System.Drawing.Color.Black;
            this.txt_SetYValue.Name = "txt_SetYValue";
            this.txt_SetYValue.NormalBackColor = System.Drawing.Color.White;
            // 
            // txt_SetXValue
            // 
            resources.ApplyResources(this.txt_SetXValue, "txt_SetXValue");
            this.txt_SetXValue.BackColor = System.Drawing.Color.White;
            this.txt_SetXValue.DecimalPlaces = 0;
            this.txt_SetXValue.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_SetXValue.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SetXValue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SetXValue.ForeColor = System.Drawing.Color.Black;
            this.txt_SetXValue.Name = "txt_SetXValue";
            this.txt_SetXValue.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
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
            // lbl_SetValue
            // 
            resources.ApplyResources(this.lbl_SetValue, "lbl_SetValue");
            this.lbl_SetValue.Name = "lbl_SetValue";
            this.lbl_SetValue.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // lbl_Definition
            // 
            resources.ApplyResources(this.lbl_Definition, "lbl_Definition");
            this.lbl_Definition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Definition.Name = "lbl_Definition";
            this.lbl_Definition.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // SetTextShiftToleranceForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.txt_SetYValue);
            this.Controls.Add(this.txt_SetXValue);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.lbl_SetValue);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.lbl_Definition);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetTextShiftToleranceForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMInputBox txt_SetYValue;
        private SRMControl.SRMInputBox txt_SetXValue;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel lbl_SetValue;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMLabel lbl_Definition;
        private SRMControl.SRMButton btn_OK;
    }
}