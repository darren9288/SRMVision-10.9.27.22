namespace AutoMode
{
    partial class TemplateImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateImageForm));
            this.tabCtrl_Templates = new SRMControl.SRMTabControl();
            this.tabPage_Template1 = new System.Windows.Forms.TabPage();
            this.lbl_TemplateMessage = new System.Windows.Forms.Label();
            this.pic_Template1 = new System.Windows.Forms.PictureBox();
            this.tabCtrl_Templates.SuspendLayout();
            this.tabPage_Template1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Template1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabCtrl_Templates
            // 
            resources.ApplyResources(this.tabCtrl_Templates, "tabCtrl_Templates");
            this.tabCtrl_Templates.Controls.Add(this.tabPage_Template1);
            this.tabCtrl_Templates.Name = "tabCtrl_Templates";
            this.tabCtrl_Templates.SelectedIndex = 0;
            // 
            // tabPage_Template1
            // 
            resources.ApplyResources(this.tabPage_Template1, "tabPage_Template1");
            this.tabPage_Template1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tabPage_Template1.Controls.Add(this.lbl_TemplateMessage);
            this.tabPage_Template1.Controls.Add(this.pic_Template1);
            this.tabPage_Template1.Name = "tabPage_Template1";
            // 
            // lbl_TemplateMessage
            // 
            resources.ApplyResources(this.lbl_TemplateMessage, "lbl_TemplateMessage");
            this.lbl_TemplateMessage.BackColor = System.Drawing.Color.Black;
            this.lbl_TemplateMessage.ForeColor = System.Drawing.Color.White;
            this.lbl_TemplateMessage.Name = "lbl_TemplateMessage";
            // 
            // pic_Template1
            // 
            resources.ApplyResources(this.pic_Template1, "pic_Template1");
            this.pic_Template1.BackColor = System.Drawing.Color.Black;
            this.pic_Template1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_Template1.Name = "pic_Template1";
            this.pic_Template1.TabStop = false;
            // 
            // TemplateImageForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.tabCtrl_Templates);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateImageForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TemplateImageForm_FormClosing);
            this.tabCtrl_Templates.ResumeLayout(false);
            this.tabPage_Template1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Template1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMTabControl tabCtrl_Templates;
        private System.Windows.Forms.TabPage tabPage_Template1;
        private System.Windows.Forms.Label lbl_TemplateMessage;
        private System.Windows.Forms.PictureBox pic_Template1;
    }
}