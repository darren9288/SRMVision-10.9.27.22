namespace User
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.PasswordTextBox = new SRMControl.SRMTextBox();
            this.UserNameTextBox = new SRMControl.SRMTextBox();
            this.label2 = new SRMControl.SRMLabel();
            this.label1 = new SRMControl.SRMLabel();
            this.Cancel1Button = new SRMControl.SRMButton();
            this.OkButton = new SRMControl.SRMButton();
            this.LogonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // PasswordTextBox
            // 
            resources.ApplyResources(this.PasswordTextBox, "PasswordTextBox");
            this.PasswordTextBox.BackColor = System.Drawing.Color.White;
            this.PasswordTextBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.NormalBackColor = System.Drawing.Color.White;
            this.LogonToolTip.SetToolTip(this.PasswordTextBox, resources.GetString("PasswordTextBox.ToolTip"));
            // 
            // UserNameTextBox
            // 
            resources.ApplyResources(this.UserNameTextBox, "UserNameTextBox");
            this.UserNameTextBox.BackColor = System.Drawing.Color.White;
            this.UserNameTextBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.UserNameTextBox.Name = "UserNameTextBox";
            this.UserNameTextBox.NormalBackColor = System.Drawing.Color.White;
            this.LogonToolTip.SetToolTip(this.UserNameTextBox, resources.GetString("UserNameTextBox.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.TextShadowColor = System.Drawing.Color.Gray;
            this.LogonToolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.TextShadowColor = System.Drawing.Color.Gray;
            this.LogonToolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // Cancel1Button
            // 
            resources.ApplyResources(this.Cancel1Button, "Cancel1Button");
            this.Cancel1Button.Name = "Cancel1Button";
            this.LogonToolTip.SetToolTip(this.Cancel1Button, resources.GetString("Cancel1Button.ToolTip"));
            this.Cancel1Button.Click += new System.EventHandler(this.Cancel1Button_Click);
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.Name = "OkButton";
            this.LogonToolTip.SetToolTip(this.OkButton, resources.GetString("OkButton.ToolTip"));
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // LoginForm
            // 
            this.AcceptButton = this.OkButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.UserNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancel1Button);
            this.Controls.Add(this.OkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.ShowInTaskbar = false;
            this.LogonToolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMTextBox PasswordTextBox;
        private SRMControl.SRMTextBox UserNameTextBox;
        private SRMControl.SRMLabel label2;
        private SRMControl.SRMLabel label1;
        private SRMControl.SRMButton Cancel1Button;
        private SRMControl.SRMButton OkButton;
        private System.Windows.Forms.ToolTip LogonToolTip;
    }
}