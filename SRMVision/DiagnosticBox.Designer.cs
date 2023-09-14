namespace SRMVision
{
    partial class DiagnosticBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosticBox));
            this.DataTextBox = new System.Windows.Forms.TextBox();
            this.SendButton = new SRMControl.SRMButton();
            this.MessageTextBox = new SRMControl.SRMTextBox();
            this.ClientTimer = new System.Windows.Forms.Timer(this.components);
            this.group_SelectTemplate = new SRMControl.SRMGroupBox();
            this.cbo_CommPort = new SRMControl.SRMComboBox();
            this.cbo_Vision = new SRMControl.SRMComboBox();
            this.radioBtn_TCPIP = new SRMControl.SRMRadioButton();
            this.radioBtn_Serial = new SRMControl.SRMRadioButton();
            this.btn_Clear = new SRMControl.SRMButton();
            this.group_SelectTemplate.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataTextBox
            // 
            resources.ApplyResources(this.DataTextBox, "DataTextBox");
            this.DataTextBox.BackColor = System.Drawing.Color.White;
            this.DataTextBox.Name = "DataTextBox";
            // 
            // SendButton
            // 
            resources.ApplyResources(this.SendButton, "SendButton");
            this.SendButton.ForeColor = System.Drawing.SystemColors.Desktop;
            this.SendButton.Name = "SendButton";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // MessageTextBox
            // 
            resources.ApplyResources(this.MessageTextBox, "MessageTextBox");
            this.MessageTextBox.BackColor = System.Drawing.Color.AliceBlue;
            this.MessageTextBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.MessageTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.NormalBackColor = System.Drawing.Color.White;
            // 
            // ClientTimer
            // 
            this.ClientTimer.Enabled = true;
            this.ClientTimer.Interval = 80;
            this.ClientTimer.Tick += new System.EventHandler(this.ClientTimer_Tick);
            // 
            // group_SelectTemplate
            // 
            resources.ApplyResources(this.group_SelectTemplate, "group_SelectTemplate");
            this.group_SelectTemplate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_SelectTemplate.Controls.Add(this.cbo_CommPort);
            this.group_SelectTemplate.Controls.Add(this.cbo_Vision);
            this.group_SelectTemplate.Controls.Add(this.radioBtn_TCPIP);
            this.group_SelectTemplate.Controls.Add(this.radioBtn_Serial);
            this.group_SelectTemplate.Name = "group_SelectTemplate";
            this.group_SelectTemplate.TabStop = false;
            // 
            // cbo_CommPort
            // 
            resources.ApplyResources(this.cbo_CommPort, "cbo_CommPort");
            this.cbo_CommPort.BackColor = System.Drawing.Color.White;
            this.cbo_CommPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_CommPort.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_CommPort.FormattingEnabled = true;
            this.cbo_CommPort.Items.AddRange(new object[] {
            resources.GetString("cbo_CommPort.Items")});
            this.cbo_CommPort.Name = "cbo_CommPort";
            this.cbo_CommPort.NormalBackColor = System.Drawing.Color.White;
            // 
            // cbo_Vision
            // 
            resources.ApplyResources(this.cbo_Vision, "cbo_Vision");
            this.cbo_Vision.BackColor = System.Drawing.Color.White;
            this.cbo_Vision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Vision.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Vision.FormattingEnabled = true;
            this.cbo_Vision.Items.AddRange(new object[] {
            resources.GetString("cbo_Vision.Items")});
            this.cbo_Vision.Name = "cbo_Vision";
            this.cbo_Vision.NormalBackColor = System.Drawing.Color.White;
            // 
            // radioBtn_TCPIP
            // 
            resources.ApplyResources(this.radioBtn_TCPIP, "radioBtn_TCPIP");
            this.radioBtn_TCPIP.Checked = true;
            this.radioBtn_TCPIP.Name = "radioBtn_TCPIP";
            this.radioBtn_TCPIP.TabStop = true;
            this.radioBtn_TCPIP.UseVisualStyleBackColor = true;
            // 
            // radioBtn_Serial
            // 
            resources.ApplyResources(this.radioBtn_Serial, "radioBtn_Serial");
            this.radioBtn_Serial.Name = "radioBtn_Serial";
            this.radioBtn_Serial.UseVisualStyleBackColor = true;
            // 
            // btn_Clear
            // 
            resources.ApplyResources(this.btn_Clear, "btn_Clear");
            this.btn_Clear.ForeColor = System.Drawing.SystemColors.Desktop;
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // DiagnosticBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.btn_Clear);
            this.Controls.Add(this.group_SelectTemplate);
            this.Controls.Add(this.DataTextBox);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.MessageTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiagnosticBox";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DiagnosticBox_FormClosing);
            this.Load += new System.EventHandler(this.DiagnosticBox_Load);
            this.group_SelectTemplate.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DataTextBox;
        private SRMControl.SRMButton SendButton;
        private SRMControl.SRMTextBox MessageTextBox;
        private System.Windows.Forms.Timer ClientTimer;
        private SRMControl.SRMGroupBox group_SelectTemplate;
        private SRMControl.SRMRadioButton radioBtn_TCPIP;
        private SRMControl.SRMRadioButton radioBtn_Serial;
        private SRMControl.SRMButton btn_Clear;
        private SRMControl.SRMComboBox cbo_Vision;
        private SRMControl.SRMComboBox cbo_CommPort;
    }
}