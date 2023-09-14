namespace VisionProcessForm
{
    partial class SetLead3DPitchGapForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetLead3DPitchGapForm));
            this.lbl_FromLeadNo = new SRMControl.SRMLabel();
            this.cbo_ToLeadNo = new SRMControl.SRMComboBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.lbl_Definition = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.SuspendLayout();
            // 
            // lbl_FromLeadNo
            // 
            resources.ApplyResources(this.lbl_FromLeadNo, "lbl_FromLeadNo");
            this.lbl_FromLeadNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_FromLeadNo.Name = "lbl_FromLeadNo";
            this.lbl_FromLeadNo.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_ToLeadNo
            // 
            resources.ApplyResources(this.cbo_ToLeadNo, "cbo_ToLeadNo");
            this.cbo_ToLeadNo.BackColor = System.Drawing.Color.White;
            this.cbo_ToLeadNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ToLeadNo.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ToLeadNo.FormattingEnabled = true;
            this.cbo_ToLeadNo.Name = "cbo_ToLeadNo";
            this.cbo_ToLeadNo.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Definition
            // 
            resources.ApplyResources(this.lbl_Definition, "lbl_Definition");
            this.lbl_Definition.Name = "lbl_Definition";
            this.lbl_Definition.TextShadowColor = System.Drawing.Color.Gray;
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
            // SetLead3DPitchGapForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.lbl_FromLeadNo);
            this.Controls.Add(this.cbo_ToLeadNo);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.lbl_Definition);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Name = "SetLead3DPitchGapForm";
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMLabel lbl_FromLeadNo;
        private SRMControl.SRMComboBox cbo_ToLeadNo;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel lbl_Definition;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
    }
}