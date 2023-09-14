namespace User
{
    partial class UserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserForm));
            this.GroupComboBox = new SRMControl.SRMComboBox();
            this.label6 = new SRMControl.SRMLabel();
            this.Cancel1Button = new SRMControl.SRMButton();
            this.SaveButton = new SRMControl.SRMButton();
            this.DeleteButton = new SRMControl.SRMButton();
            this.Password2EditBox = new SRMControl.SRMInputBox();
            this.label5 = new SRMControl.SRMLabel();
            this.Password1EditBox = new SRMControl.SRMInputBox();
            this.label4 = new SRMControl.SRMLabel();
            this.DescriptionEditBox = new SRMControl.SRMInputBox();
            this.label3 = new SRMControl.SRMLabel();
            this.FullnameEditBox = new SRMControl.SRMInputBox();
            this.label2 = new SRMControl.SRMLabel();
            this.UsernameEditBox = new SRMControl.SRMInputBox();
            this.label1 = new SRMControl.SRMLabel();
            this.SuspendLayout();
            // 
            // GroupComboBox
            // 
            resources.ApplyResources(this.GroupComboBox, "GroupComboBox");
            this.GroupComboBox.BackColor = System.Drawing.Color.White;
            this.GroupComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupComboBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.GroupComboBox.Name = "GroupComboBox";
            this.GroupComboBox.NormalBackColor = System.Drawing.Color.White;
            this.GroupComboBox.SelectedIndexChanged += new System.EventHandler(this.GroupComboBox_SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.label6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // Cancel1Button
            // 
            resources.ApplyResources(this.Cancel1Button, "Cancel1Button");
            this.Cancel1Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel1Button.Name = "Cancel1Button";
            // 
            // SaveButton
            // 
            resources.ApplyResources(this.SaveButton, "SaveButton");
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // Password2EditBox
            // 
            resources.ApplyResources(this.Password2EditBox, "Password2EditBox");
            this.Password2EditBox.BackColor = System.Drawing.Color.White;
            this.Password2EditBox.DecimalPlaces = 2;
            this.Password2EditBox.DecMaxValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.Password2EditBox.DecMinValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            -2147483648});
            this.Password2EditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Password2EditBox.Name = "Password2EditBox";
            this.Password2EditBox.NormalBackColor = System.Drawing.Color.White;
            this.Password2EditBox.TextChanged += new System.EventHandler(this.Password2EditBox_TextChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.label5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // Password1EditBox
            // 
            resources.ApplyResources(this.Password1EditBox, "Password1EditBox");
            this.Password1EditBox.BackColor = System.Drawing.Color.White;
            this.Password1EditBox.DecimalPlaces = 2;
            this.Password1EditBox.DecMaxValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.Password1EditBox.DecMinValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            -2147483648});
            this.Password1EditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Password1EditBox.Name = "Password1EditBox";
            this.Password1EditBox.NormalBackColor = System.Drawing.Color.White;
            this.Password1EditBox.TextChanged += new System.EventHandler(this.Password1EditBox_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.label4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // DescriptionEditBox
            // 
            resources.ApplyResources(this.DescriptionEditBox, "DescriptionEditBox");
            this.DescriptionEditBox.BackColor = System.Drawing.Color.White;
            this.DescriptionEditBox.DecimalPlaces = 2;
            this.DescriptionEditBox.DecMaxValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.DescriptionEditBox.DecMinValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            -2147483648});
            this.DescriptionEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.DescriptionEditBox.Name = "DescriptionEditBox";
            this.DescriptionEditBox.NormalBackColor = System.Drawing.Color.White;
            this.DescriptionEditBox.TextChanged += new System.EventHandler(this.DescriptionEditBox_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // FullnameEditBox
            // 
            resources.ApplyResources(this.FullnameEditBox, "FullnameEditBox");
            this.FullnameEditBox.BackColor = System.Drawing.Color.White;
            this.FullnameEditBox.DecimalPlaces = 2;
            this.FullnameEditBox.DecMaxValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.FullnameEditBox.DecMinValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            -2147483648});
            this.FullnameEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.FullnameEditBox.Name = "FullnameEditBox";
            this.FullnameEditBox.NormalBackColor = System.Drawing.Color.White;
            this.FullnameEditBox.TextChanged += new System.EventHandler(this.FullnameEditBox_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // UsernameEditBox
            // 
            resources.ApplyResources(this.UsernameEditBox, "UsernameEditBox");
            this.UsernameEditBox.BackColor = System.Drawing.Color.White;
            this.UsernameEditBox.DecimalPlaces = 2;
            this.UsernameEditBox.DecMaxValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.UsernameEditBox.DecMinValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            -2147483648});
            this.UsernameEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.UsernameEditBox.Name = "UsernameEditBox";
            this.UsernameEditBox.NormalBackColor = System.Drawing.Color.White;
            this.UsernameEditBox.TextChanged += new System.EventHandler(this.UsernameEditBox_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // UserForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.GroupComboBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Cancel1Button);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.Password2EditBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Password1EditBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DescriptionEditBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.FullnameEditBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UsernameEditBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.UserForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMComboBox GroupComboBox;
        private SRMControl.SRMLabel label6;
        private SRMControl.SRMButton Cancel1Button;
        private SRMControl.SRMButton SaveButton;
        private SRMControl.SRMButton DeleteButton;
        private SRMControl.SRMInputBox Password2EditBox;
        private SRMControl.SRMLabel label5;
        private SRMControl.SRMInputBox Password1EditBox;
        private SRMControl.SRMLabel label4;
        private SRMControl.SRMInputBox DescriptionEditBox;
        private SRMControl.SRMLabel label3;
        private SRMControl.SRMInputBox FullnameEditBox;
        private SRMControl.SRMLabel label2;
        private SRMControl.SRMInputBox UsernameEditBox;
        private SRMControl.SRMLabel label1;
    }
}