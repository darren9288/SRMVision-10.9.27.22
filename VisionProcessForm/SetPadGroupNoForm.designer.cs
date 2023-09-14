namespace VisionProcessForm
{
    partial class SetPadGroupNoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetPadGroupNoForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.lbl_Definition = new SRMControl.SRMLabel();
            this.pictureBox20 = new System.Windows.Forms.PictureBox();
            this.srmLabel38 = new SRMControl.SRMLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox20)).BeginInit();
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
            this.lbl_Definition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Definition.Name = "lbl_Definition";
            this.lbl_Definition.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pictureBox20
            // 
            resources.ApplyResources(this.pictureBox20, "pictureBox20");
            this.pictureBox20.BackColor = System.Drawing.Color.Black;
            this.pictureBox20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox20.Name = "pictureBox20";
            this.pictureBox20.TabStop = false;
            // 
            // srmLabel38
            // 
            resources.ApplyResources(this.srmLabel38, "srmLabel38");
            this.srmLabel38.Name = "srmLabel38";
            this.srmLabel38.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // SetPadGroupNoForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox20);
            this.Controls.Add(this.srmLabel38);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.lbl_Definition);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetPadGroupNoForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetPadGroupNoForm_FormClosing);
            this.Load += new System.EventHandler(this.SetPadGroupNoForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox20)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel lbl_Definition;
        private System.Windows.Forms.PictureBox pictureBox20;
        private SRMControl.SRMLabel srmLabel38;
    }
}