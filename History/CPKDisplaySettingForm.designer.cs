namespace History
{
    partial class CPKDisplaySettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CPKDisplaySettingForm));
            this.group_Parameter = new SRMControl.SRMGroupBox();
            this.chk_Left = new SRMControl.SRMCheckBox();
            this.chk_Right = new SRMControl.SRMCheckBox();
            this.chk_Bottom = new SRMControl.SRMCheckBox();
            this.chk_Top = new SRMControl.SRMCheckBox();
            this.chk_Middle = new SRMControl.SRMCheckBox();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_Ok = new SRMControl.SRMButton();
            this.group_Parameter.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_Parameter
            // 
            resources.ApplyResources(this.group_Parameter, "group_Parameter");
            this.group_Parameter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_Parameter.Controls.Add(this.chk_Left);
            this.group_Parameter.Controls.Add(this.chk_Right);
            this.group_Parameter.Controls.Add(this.chk_Bottom);
            this.group_Parameter.Controls.Add(this.chk_Top);
            this.group_Parameter.Controls.Add(this.chk_Middle);
            this.group_Parameter.Name = "group_Parameter";
            this.group_Parameter.TabStop = false;
            // 
            // chk_Left
            // 
            resources.ApplyResources(this.chk_Left, "chk_Left");
            this.chk_Left.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Left.Name = "chk_Left";
            this.chk_Left.Selected = true;
            this.chk_Left.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Left.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Left.UseVisualStyleBackColor = true;
            // 
            // chk_Right
            // 
            resources.ApplyResources(this.chk_Right, "chk_Right");
            this.chk_Right.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Right.Name = "chk_Right";
            this.chk_Right.Selected = true;
            this.chk_Right.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Right.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Right.UseVisualStyleBackColor = true;
            // 
            // chk_Bottom
            // 
            resources.ApplyResources(this.chk_Bottom, "chk_Bottom");
            this.chk_Bottom.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Bottom.Name = "chk_Bottom";
            this.chk_Bottom.Selected = true;
            this.chk_Bottom.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Bottom.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Bottom.UseVisualStyleBackColor = true;
            // 
            // chk_Top
            // 
            resources.ApplyResources(this.chk_Top, "chk_Top");
            this.chk_Top.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Top.Name = "chk_Top";
            this.chk_Top.Selected = true;
            this.chk_Top.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Top.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Top.UseVisualStyleBackColor = true;
            // 
            // chk_Middle
            // 
            resources.ApplyResources(this.chk_Middle, "chk_Middle");
            this.chk_Middle.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Middle.Name = "chk_Middle";
            this.chk_Middle.Selected = true;
            this.chk_Middle.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Middle.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Middle.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Ok
            // 
            resources.ApplyResources(this.btn_Ok, "btn_Ok");
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // CPKDisplaySettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.group_Parameter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CPKDisplaySettingForm";
            this.ShowInTaskbar = false;
            this.group_Parameter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMGroupBox group_Parameter;
        private SRMControl.SRMCheckBox chk_Bottom;
        private SRMControl.SRMCheckBox chk_Top;
        private SRMControl.SRMCheckBox chk_Middle;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_Ok;
        private SRMControl.SRMCheckBox chk_Left;
        private SRMControl.SRMCheckBox chk_Right;
    }
}