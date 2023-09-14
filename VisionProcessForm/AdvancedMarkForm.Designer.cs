namespace VisionProcessForm
{
    partial class AdvancedMarkForm
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
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.chk_WhiteOnBlack = new SRMControl.SRMCheckBox();
            this.chk_MultiGroups = new SRMControl.SRMCheckBox();
            this.chk_WantBuildTexts = new SRMControl.SRMCheckBox();
            this.chk_MultiTemplates = new SRMControl.SRMCheckBox();
            this.chk_Set1ToAll = new SRMControl.SRMCheckBox();
            this.chk_SkipMarkInspection = new SRMControl.SRMCheckBox();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(150, 332);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 16;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(64, 332);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 34);
            this.btn_OK.TabIndex = 15;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // chk_WhiteOnBlack
            // 
            this.chk_WhiteOnBlack.Checked = true;
            this.chk_WhiteOnBlack.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WhiteOnBlack.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_WhiteOnBlack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_WhiteOnBlack.Location = new System.Drawing.Point(12, 12);
            this.chk_WhiteOnBlack.Name = "chk_WhiteOnBlack";
            this.chk_WhiteOnBlack.Selected = false;
            this.chk_WhiteOnBlack.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WhiteOnBlack.Size = new System.Drawing.Size(184, 24);
            this.chk_WhiteOnBlack.TabIndex = 59;
            this.chk_WhiteOnBlack.Text = "White On Black";
            this.chk_WhiteOnBlack.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WhiteOnBlack.UseVisualStyleBackColor = true;
            // 
            // chk_MultiGroups
            // 
            this.chk_MultiGroups.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_MultiGroups.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_MultiGroups.Location = new System.Drawing.Point(12, 162);
            this.chk_MultiGroups.Name = "chk_MultiGroups";
            this.chk_MultiGroups.Selected = false;
            this.chk_MultiGroups.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_MultiGroups.Size = new System.Drawing.Size(184, 24);
            this.chk_MultiGroups.TabIndex = 60;
            this.chk_MultiGroups.Text = "Want Multiple Groups";
            this.chk_MultiGroups.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_MultiGroups.UseVisualStyleBackColor = true;
            this.chk_MultiGroups.Visible = false;
            // 
            // chk_WantBuildTexts
            // 
            this.chk_WantBuildTexts.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WantBuildTexts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_WantBuildTexts.Location = new System.Drawing.Point(12, 42);
            this.chk_WantBuildTexts.Name = "chk_WantBuildTexts";
            this.chk_WantBuildTexts.Selected = false;
            this.chk_WantBuildTexts.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantBuildTexts.Size = new System.Drawing.Size(184, 24);
            this.chk_WantBuildTexts.TabIndex = 133;
            this.chk_WantBuildTexts.Text = "Want Build Texts";
            this.chk_WantBuildTexts.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantBuildTexts.UseVisualStyleBackColor = true;
            // 
            // chk_MultiTemplates
            // 
            this.chk_MultiTemplates.Checked = true;
            this.chk_MultiTemplates.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_MultiTemplates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_MultiTemplates.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_MultiTemplates.Location = new System.Drawing.Point(12, 72);
            this.chk_MultiTemplates.Name = "chk_MultiTemplates";
            this.chk_MultiTemplates.Selected = false;
            this.chk_MultiTemplates.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_MultiTemplates.Size = new System.Drawing.Size(184, 24);
            this.chk_MultiTemplates.TabIndex = 134;
            this.chk_MultiTemplates.Text = "Want Multi Templates";
            this.chk_MultiTemplates.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_MultiTemplates.UseVisualStyleBackColor = true;
            // 
            // chk_Set1ToAll
            // 
            this.chk_Set1ToAll.Checked = true;
            this.chk_Set1ToAll.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Set1ToAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Set1ToAll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_Set1ToAll.Location = new System.Drawing.Point(12, 102);
            this.chk_Set1ToAll.Name = "chk_Set1ToAll";
            this.chk_Set1ToAll.Selected = false;
            this.chk_Set1ToAll.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Set1ToAll.Size = new System.Drawing.Size(184, 24);
            this.chk_Set1ToAll.TabIndex = 137;
            this.chk_Set1ToAll.Text = "Set 1 to All Templates";
            this.chk_Set1ToAll.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Set1ToAll.UseVisualStyleBackColor = true;
            // 
            // chk_SkipMarkInspection
            // 
            this.chk_SkipMarkInspection.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SkipMarkInspection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_SkipMarkInspection.Location = new System.Drawing.Point(12, 132);
            this.chk_SkipMarkInspection.Name = "chk_SkipMarkInspection";
            this.chk_SkipMarkInspection.Selected = false;
            this.chk_SkipMarkInspection.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SkipMarkInspection.Size = new System.Drawing.Size(184, 24);
            this.chk_SkipMarkInspection.TabIndex = 139;
            this.chk_SkipMarkInspection.Text = "Skip Mark Inspection";
            this.chk_SkipMarkInspection.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SkipMarkInspection.UseVisualStyleBackColor = true;
            // 
            // AdvancedMarkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(244, 383);
            this.ControlBox = false;
            this.Controls.Add(this.chk_SkipMarkInspection);
            this.Controls.Add(this.chk_Set1ToAll);
            this.Controls.Add(this.chk_MultiTemplates);
            this.Controls.Add(this.chk_WantBuildTexts);
            this.Controls.Add(this.chk_MultiGroups);
            this.Controls.Add(this.chk_WhiteOnBlack);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedMarkForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMCheckBox chk_WhiteOnBlack;
        private SRMControl.SRMCheckBox chk_MultiGroups;
        private SRMControl.SRMCheckBox chk_WantBuildTexts;
        private SRMControl.SRMCheckBox chk_MultiTemplates;
        private SRMControl.SRMCheckBox chk_Set1ToAll;
        private SRMControl.SRMCheckBox chk_SkipMarkInspection;
    }
}