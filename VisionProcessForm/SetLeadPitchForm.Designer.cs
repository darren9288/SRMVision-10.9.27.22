namespace VisionProcessForm
{
    partial class SetLeadPitchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetLeadPitchForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.lbl_Definition = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.cbo_FromLeadNo = new SRMControl.SRMComboBox();
            this.cbo_ToLeadNo = new SRMControl.SRMComboBox();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
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
            // cbo_FromLeadNo
            // 
            resources.ApplyResources(this.cbo_FromLeadNo, "cbo_FromLeadNo");
            this.cbo_FromLeadNo.BackColor = System.Drawing.Color.White;
            this.cbo_FromLeadNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_FromLeadNo.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_FromLeadNo.FormattingEnabled = true;
            this.cbo_FromLeadNo.Name = "cbo_FromLeadNo";
            this.cbo_FromLeadNo.NormalBackColor = System.Drawing.Color.White;
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
            // SetLeadPitchForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.cbo_ToLeadNo);
            this.Controls.Add(this.cbo_FromLeadNo);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.lbl_Definition);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Name = "SetLeadPitchForm";
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel lbl_Definition;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMComboBox cbo_FromLeadNo;
        private SRMControl.SRMComboBox cbo_ToLeadNo;
    }
}