namespace VisionProcessForm
{
    partial class ZoomImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZoomImageForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.cbo_ZoomValue = new SRMControl.SRMComboBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // cbo_ZoomValue
            // 
            this.cbo_ZoomValue.BackColor = System.Drawing.Color.White;
            this.cbo_ZoomValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ZoomValue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.cbo_ZoomValue, "cbo_ZoomValue");
            this.cbo_ZoomValue.FormattingEnabled = true;
            this.cbo_ZoomValue.Items.AddRange(new object[] {
            resources.GetString("cbo_ZoomValue.Items"),
            resources.GetString("cbo_ZoomValue.Items1"),
            resources.GetString("cbo_ZoomValue.Items2"),
            resources.GetString("cbo_ZoomValue.Items3"),
            resources.GetString("cbo_ZoomValue.Items4"),
            resources.GetString("cbo_ZoomValue.Items5"),
            resources.GetString("cbo_ZoomValue.Items6"),
            resources.GetString("cbo_ZoomValue.Items7"),
            resources.GetString("cbo_ZoomValue.Items8"),
            resources.GetString("cbo_ZoomValue.Items9"),
            resources.GetString("cbo_ZoomValue.Items10"),
            resources.GetString("cbo_ZoomValue.Items11"),
            resources.GetString("cbo_ZoomValue.Items12"),
            resources.GetString("cbo_ZoomValue.Items13"),
            resources.GetString("cbo_ZoomValue.Items14"),
            resources.GetString("cbo_ZoomValue.Items15"),
            resources.GetString("cbo_ZoomValue.Items16"),
            resources.GetString("cbo_ZoomValue.Items17"),
            resources.GetString("cbo_ZoomValue.Items18"),
            resources.GetString("cbo_ZoomValue.Items19")});
            this.cbo_ZoomValue.Name = "cbo_ZoomValue";
            this.cbo_ZoomValue.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ZoomValue.SelectedIndexChanged += new System.EventHandler(this.cbo_ZoomValue_SelectedIndexChanged);
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // ZoomImageForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.cbo_ZoomValue);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ZoomImageForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMComboBox cbo_ZoomValue;
        private SRMControl.SRMLabel srmLabel3;
    }
}