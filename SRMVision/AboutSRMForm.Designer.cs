namespace SRMVision
{
    partial class AboutSRMForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutSRMForm));
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btn_Close = new SRMControl.SRMButton();
            this.lbl_SoftwareVersion = new SRMControl.SRMLabel();
            this.label10 = new SRMControl.SRMLabel();
            this.label8 = new SRMControl.SRMLabel();
            this.label2 = new SRMControl.SRMLabel();
            this.label1 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.tre_VisionsList = new System.Windows.Forms.TreeView();
            this.Image_TreeView = new System.Windows.Forms.ImageList(this.components);
            this.label11 = new SRMControl.SRMLabel();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // lbl_SoftwareVersion
            // 
            resources.ApplyResources(this.lbl_SoftwareVersion, "lbl_SoftwareVersion");
            this.lbl_SoftwareVersion.Name = "lbl_SoftwareVersion";
            this.lbl_SoftwareVersion.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.label10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.label8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Name = "label1";
            this.label1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tre_VisionsList
            // 
            resources.ApplyResources(this.tre_VisionsList, "tre_VisionsList");
            this.tre_VisionsList.BackColor = System.Drawing.Color.AliceBlue;
            this.tre_VisionsList.ImageList = this.Image_TreeView;
            this.tre_VisionsList.ItemHeight = 24;
            this.tre_VisionsList.Name = "tre_VisionsList";
            // 
            // Image_TreeView
            // 
            this.Image_TreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("Image_TreeView.ImageStream")));
            this.Image_TreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.Image_TreeView.Images.SetKeyName(0, "Digital Camera.png");
            this.Image_TreeView.Images.SetKeyName(1, "smallPhoto.png");
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.label11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // AboutSRMForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.tre_VisionsList);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.lbl_SoftwareVersion);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label11);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutSRMForm";
            this.Load += new System.EventHandler(this.AboutSRMForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMLabel lbl_SoftwareVersion;
        private SRMControl.SRMLabel label10;
        private SRMControl.SRMLabel label8;
        private SRMControl.SRMLabel label2;
        private SRMControl.SRMLabel label1;
        private SRMControl.SRMLabel label11;
        private SRMControl.SRMLabel srmLabel1;
        private System.Windows.Forms.TreeView tre_VisionsList;
        private System.Windows.Forms.ImageList Image_TreeView;
    }
}