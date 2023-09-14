namespace Common
{
    partial class MessageBoxEx
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageBoxEx));
            this.srmButton3 = new SRMControl.SRMButton();
            this.srmButton2 = new SRMControl.SRMButton();
            this.srmButton1 = new SRMControl.SRMButton();
            this.lbl_Message = new SRMControl.SRMLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // srmButton3
            // 
            resources.ApplyResources(this.srmButton3, "srmButton3");
            this.srmButton3.Name = "srmButton3";
            this.srmButton3.UseVisualStyleBackColor = true;
            // 
            // srmButton2
            // 
            resources.ApplyResources(this.srmButton2, "srmButton2");
            this.srmButton2.Name = "srmButton2";
            this.srmButton2.UseVisualStyleBackColor = true;
            // 
            // srmButton1
            // 
            resources.ApplyResources(this.srmButton1, "srmButton1");
            this.srmButton1.Name = "srmButton1";
            this.srmButton1.UseVisualStyleBackColor = true;
            // 
            // lbl_Message
            // 
            resources.ApplyResources(this.lbl_Message, "lbl_Message");
            this.lbl_Message.Name = "lbl_Message";
            this.lbl_Message.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // MessageBoxEx
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.srmButton3);
            this.Controls.Add(this.srmButton2);
            this.Controls.Add(this.srmButton1);
            this.Controls.Add(this.lbl_Message);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageBoxEx";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton srmButton3;
        private SRMControl.SRMButton srmButton2;
        private SRMControl.SRMButton srmButton1;
        private SRMControl.SRMLabel lbl_Message;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}