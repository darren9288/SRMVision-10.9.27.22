namespace SRMVision
{
    partial class WaitShutdownForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaitShutdownForm));
            this.ShutDownLabel = new SRMControl.SRMLabel();
            this.PowerOFFLabel = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.btn_Stop = new SRMControl.SRMButton();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.lbl_Timer = new SRMControl.SRMLabel();
            this.btn_ShutdownNow = new SRMControl.SRMButton();
            this.SuspendLayout();
            // 
            // ShutDownLabel
            // 
            resources.ApplyResources(this.ShutDownLabel, "ShutDownLabel");
            this.ShutDownLabel.BackColor = System.Drawing.Color.Transparent;
            this.ShutDownLabel.ForeColor = System.Drawing.Color.Red;
            this.ShutDownLabel.Name = "ShutDownLabel";
            this.ShutDownLabel.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // PowerOFFLabel
            // 
            resources.ApplyResources(this.PowerOFFLabel, "PowerOFFLabel");
            this.PowerOFFLabel.BackColor = System.Drawing.Color.Transparent;
            this.PowerOFFLabel.ForeColor = System.Drawing.Color.Red;
            this.PowerOFFLabel.Name = "PowerOFFLabel";
            this.PowerOFFLabel.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.BackColor = System.Drawing.Color.Transparent;
            this.srmLabel1.ForeColor = System.Drawing.Color.Black;
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Stop
            // 
            resources.ApplyResources(this.btn_Stop, "btn_Stop");
            this.btn_Stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.BackColor = System.Drawing.Color.Transparent;
            this.srmLabel2.ForeColor = System.Drawing.Color.Red;
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Timer
            // 
            resources.ApplyResources(this.lbl_Timer, "lbl_Timer");
            this.lbl_Timer.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Timer.ForeColor = System.Drawing.Color.Red;
            this.lbl_Timer.Name = "lbl_Timer";
            this.lbl_Timer.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_ShutdownNow
            // 
            resources.ApplyResources(this.btn_ShutdownNow, "btn_ShutdownNow");
            this.btn_ShutdownNow.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ShutdownNow.Name = "btn_ShutdownNow";
            this.btn_ShutdownNow.Click += new System.EventHandler(this.btn_ShutdownNow_Click);
            // 
            // WaitShutdownForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_ShutdownNow);
            this.Controls.Add(this.lbl_Timer);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.ShutDownLabel);
            this.Controls.Add(this.PowerOFFLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitShutdownForm";
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMLabel ShutDownLabel;
        private SRMControl.SRMLabel PowerOFFLabel;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMButton btn_Stop;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel lbl_Timer;
        private SRMControl.SRMButton btn_ShutdownNow;
    }
}