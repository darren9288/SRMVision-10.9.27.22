namespace Common
{
    partial class WaitingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaitingForm));
            this.StopButton = new SRMControl.SRMButton();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.DisplayLabel = new SRMControl.SRMLabel();
            this.CommonPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CommonPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // StopButton
            // 
            resources.ApplyResources(this.StopButton, "StopButton");
            this.StopButton.BackColor = System.Drawing.Color.Transparent;
            this.StopButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.StopButton.Name = "StopButton";
            this.StopButton.UseVisualStyleBackColor = false;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // DisplayLabel
            // 
            resources.ApplyResources(this.DisplayLabel, "DisplayLabel");
            this.DisplayLabel.BackColor = System.Drawing.Color.Transparent;
            this.DisplayLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DisplayLabel.Name = "DisplayLabel";
            this.DisplayLabel.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // CommonPictureBox
            // 
            resources.ApplyResources(this.CommonPictureBox, "CommonPictureBox");
            this.CommonPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.CommonPictureBox.Name = "CommonPictureBox";
            this.CommonPictureBox.TabStop = false;
            // 
            // WaitingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.DisplayLabel);
            this.Controls.Add(this.CommonPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WaitingForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.CommonPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton StopButton;
        private System.Windows.Forms.Timer timer;
        private SRMControl.SRMLabel DisplayLabel;
        private System.Windows.Forms.PictureBox CommonPictureBox;
    }
}