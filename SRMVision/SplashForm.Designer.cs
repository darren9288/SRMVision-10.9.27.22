namespace SRMVision
{
    partial class SplashForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
            this.label7 = new SRMControl.SRMLabel();
            this.ShutDownLabel = new SRMControl.SRMLabel();
            this.PowerOFFLabel = new SRMControl.SRMLabel();
            this.VersionLabel = new SRMControl.SRMLabel();
            this.InitializingProgressBar = new System.Windows.Forms.ProgressBar();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Name = "label7";
            this.label7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // ShutDownLabel
            // 
            this.ShutDownLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ShutDownLabel, "ShutDownLabel");
            this.ShutDownLabel.ForeColor = System.Drawing.Color.Red;
            this.ShutDownLabel.Name = "ShutDownLabel";
            this.ShutDownLabel.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // PowerOFFLabel
            // 
            this.PowerOFFLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.PowerOFFLabel, "PowerOFFLabel");
            this.PowerOFFLabel.ForeColor = System.Drawing.Color.Red;
            this.PowerOFFLabel.Name = "PowerOFFLabel";
            this.PowerOFFLabel.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // VersionLabel
            // 
            this.VersionLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.VersionLabel, "VersionLabel");
            this.VersionLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // InitializingProgressBar
            // 
            resources.ApplyResources(this.InitializingProgressBar, "InitializingProgressBar");
            this.InitializingProgressBar.Name = "InitializingProgressBar";
            this.InitializingProgressBar.Step = 5;
            // 
            // StatusLabel
            // 
            resources.ApplyResources(this.StatusLabel, "StatusLabel");
            this.StatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.StatusLabel.ForeColor = System.Drawing.Color.MediumTurquoise;
            this.StatusLabel.Name = "StatusLabel";
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Name = "label1";
            // 
            // SplashForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InitializingProgressBar);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ShutDownLabel);
            this.Controls.Add(this.PowerOFFLabel);
            this.Controls.Add(this.VersionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel label7;
        private SRMControl.SRMLabel ShutDownLabel;
        private SRMControl.SRMLabel PowerOFFLabel;
        private SRMControl.SRMLabel VersionLabel;
        private System.Windows.Forms.ProgressBar InitializingProgressBar;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Label label1;
    }
}