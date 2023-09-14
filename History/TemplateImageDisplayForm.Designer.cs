namespace History
{
    partial class TemplateImageDisplayForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateImageDisplayForm));
            this.pic_ImageOld = new System.Windows.Forms.PictureBox();
            this.pic_ImageNew = new System.Windows.Forms.PictureBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.cbo_TemplateNo = new SRMControl.SRMComboBox();
            this.lbl_Template = new SRMControl.SRMLabel();
            this.cbo_ViewImage = new SRMControl.SRMComboBox();
            this.pnl_PictureBoxOld = new System.Windows.Forms.Panel();
            this.pnl_PictureBoxNew = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImageOld)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImageNew)).BeginInit();
            this.pnl_PictureBoxOld.SuspendLayout();
            this.pnl_PictureBoxNew.SuspendLayout();
            this.SuspendLayout();
            // 
            // pic_ImageOld
            // 
            resources.ApplyResources(this.pic_ImageOld, "pic_ImageOld");
            this.pic_ImageOld.BackColor = System.Drawing.Color.Black;
            this.pic_ImageOld.Name = "pic_ImageOld";
            this.pic_ImageOld.TabStop = false;
            // 
            // pic_ImageNew
            // 
            resources.ApplyResources(this.pic_ImageNew, "pic_ImageNew");
            this.pic_ImageNew.BackColor = System.Drawing.Color.Black;
            this.pic_ImageNew.Name = "pic_ImageNew";
            this.pic_ImageNew.TabStop = false;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_TemplateNo
            // 
            resources.ApplyResources(this.cbo_TemplateNo, "cbo_TemplateNo");
            this.cbo_TemplateNo.BackColor = System.Drawing.Color.White;
            this.cbo_TemplateNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_TemplateNo.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_TemplateNo.Name = "cbo_TemplateNo";
            this.cbo_TemplateNo.NormalBackColor = System.Drawing.Color.White;
            this.cbo_TemplateNo.SelectedIndexChanged += new System.EventHandler(this.cbo_TemplateNo_SelectedIndexChanged);
            // 
            // lbl_Template
            // 
            resources.ApplyResources(this.lbl_Template, "lbl_Template");
            this.lbl_Template.Name = "lbl_Template";
            this.lbl_Template.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_ViewImage
            // 
            resources.ApplyResources(this.cbo_ViewImage, "cbo_ViewImage");
            this.cbo_ViewImage.BackColor = System.Drawing.Color.White;
            this.cbo_ViewImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ViewImage.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ViewImage.FormattingEnabled = true;
            this.cbo_ViewImage.Name = "cbo_ViewImage";
            this.cbo_ViewImage.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ViewImage.SelectedIndexChanged += new System.EventHandler(this.cbo_ViewImage_SelectedIndexChanged);
            // 
            // pnl_PictureBoxOld
            // 
            resources.ApplyResources(this.pnl_PictureBoxOld, "pnl_PictureBoxOld");
            this.pnl_PictureBoxOld.BackColor = System.Drawing.Color.Gray;
            this.pnl_PictureBoxOld.Controls.Add(this.pic_ImageOld);
            this.pnl_PictureBoxOld.Name = "pnl_PictureBoxOld";
            // 
            // pnl_PictureBoxNew
            // 
            resources.ApplyResources(this.pnl_PictureBoxNew, "pnl_PictureBoxNew");
            this.pnl_PictureBoxNew.BackColor = System.Drawing.Color.Gray;
            this.pnl_PictureBoxNew.Controls.Add(this.pic_ImageNew);
            this.pnl_PictureBoxNew.Name = "pnl_PictureBoxNew";
            // 
            // TemplateImageDisplayForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.pnl_PictureBoxNew);
            this.Controls.Add(this.pnl_PictureBoxOld);
            this.Controls.Add(this.cbo_ViewImage);
            this.Controls.Add(this.cbo_TemplateNo);
            this.Controls.Add(this.lbl_Template);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.srmLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateImageDisplayForm";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImageOld)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ImageNew)).EndInit();
            this.pnl_PictureBoxOld.ResumeLayout(false);
            this.pnl_PictureBoxNew.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pic_ImageOld;
        private System.Windows.Forms.PictureBox pic_ImageNew;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMComboBox cbo_TemplateNo;
        private SRMControl.SRMLabel lbl_Template;
        private SRMControl.SRMComboBox cbo_ViewImage;
        private System.Windows.Forms.Panel pnl_PictureBoxOld;
        private System.Windows.Forms.Panel pnl_PictureBoxNew;
    }
}