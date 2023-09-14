namespace VisionProcessForm
{
    partial class SetPitchGapForm2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetPitchGapForm2));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.lbl_Definition = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.cbo_ToPadNo = new SRMControl.SRMComboBox();
            this.lbl_FromPadNo = new SRMControl.SRMLabel();
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
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // lbl_Definition
            // 
            resources.ApplyResources(this.lbl_Definition, "lbl_Definition");
            this.lbl_Definition.Name = "lbl_Definition";
            this.lbl_Definition.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_ToPadNo
            // 
            resources.ApplyResources(this.cbo_ToPadNo, "cbo_ToPadNo");
            this.cbo_ToPadNo.BackColor = System.Drawing.Color.White;
            this.cbo_ToPadNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ToPadNo.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ToPadNo.FormattingEnabled = true;
            this.cbo_ToPadNo.Name = "cbo_ToPadNo";
            this.cbo_ToPadNo.NormalBackColor = System.Drawing.Color.White;
            // 
            // lbl_FromPadNo
            // 
            resources.ApplyResources(this.lbl_FromPadNo, "lbl_FromPadNo");
            this.lbl_FromPadNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_FromPadNo.Name = "lbl_FromPadNo";
            this.lbl_FromPadNo.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // SetPitchGapForm2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.lbl_FromPadNo);
            this.Controls.Add(this.cbo_ToPadNo);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.lbl_Definition);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Name = "SetPitchGapForm2";
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel lbl_Definition;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMComboBox cbo_ToPadNo;
        private SRMControl.SRMLabel lbl_FromPadNo;
    }
}