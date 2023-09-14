namespace VisionProcessForm
{
    partial class AdvancedMarkOCRForm
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
            this.chk_MultiTemplates = new SRMControl.SRMCheckBox();
            this.chk_RemoveBorderMode = new SRMControl.SRMCheckBox();
            this.chk_RecogCharPosition = new SRMControl.SRMCheckBox();
            this.chk_SkipMarkInspection = new SRMControl.SRMCheckBox();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(152, 196);
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
            this.btn_OK.Location = new System.Drawing.Point(66, 196);
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
            // chk_MultiTemplates
            // 
            this.chk_MultiTemplates.Checked = true;
            this.chk_MultiTemplates.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_MultiTemplates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_MultiTemplates.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_MultiTemplates.Location = new System.Drawing.Point(12, 42);
            this.chk_MultiTemplates.Name = "chk_MultiTemplates";
            this.chk_MultiTemplates.Selected = false;
            this.chk_MultiTemplates.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_MultiTemplates.Size = new System.Drawing.Size(184, 24);
            this.chk_MultiTemplates.TabIndex = 134;
            this.chk_MultiTemplates.Text = "Want Multi Templates";
            this.chk_MultiTemplates.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_MultiTemplates.UseVisualStyleBackColor = true;
            // 
            // chk_RemoveBorderMode
            // 
            this.chk_RemoveBorderMode.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_RemoveBorderMode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_RemoveBorderMode.Location = new System.Drawing.Point(12, 72);
            this.chk_RemoveBorderMode.Name = "chk_RemoveBorderMode";
            this.chk_RemoveBorderMode.Selected = false;
            this.chk_RemoveBorderMode.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_RemoveBorderMode.Size = new System.Drawing.Size(209, 24);
            this.chk_RemoveBorderMode.TabIndex = 135;
            this.chk_RemoveBorderMode.Text = "Inspection Remove Border Mode";
            this.chk_RemoveBorderMode.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_RemoveBorderMode.UseVisualStyleBackColor = true;
            // 
            // chk_RecogCharPosition
            // 
            this.chk_RecogCharPosition.Checked = true;
            this.chk_RecogCharPosition.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_RecogCharPosition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_RecogCharPosition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_RecogCharPosition.Location = new System.Drawing.Point(12, 102);
            this.chk_RecogCharPosition.Name = "chk_RecogCharPosition";
            this.chk_RecogCharPosition.Selected = false;
            this.chk_RecogCharPosition.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_RecogCharPosition.Size = new System.Drawing.Size(184, 24);
            this.chk_RecogCharPosition.TabIndex = 136;
            this.chk_RecogCharPosition.Text = "Recognize Char Position";
            this.chk_RecogCharPosition.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_RecogCharPosition.UseVisualStyleBackColor = true;
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
            this.chk_SkipMarkInspection.TabIndex = 140;
            this.chk_SkipMarkInspection.Text = "Skip Mark Inspection";
            this.chk_SkipMarkInspection.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SkipMarkInspection.UseVisualStyleBackColor = true;
            // 
            // AdvancedMarkOCRForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(244, 242);
            this.ControlBox = false;
            this.Controls.Add(this.chk_SkipMarkInspection);
            this.Controls.Add(this.chk_RecogCharPosition);
            this.Controls.Add(this.chk_RemoveBorderMode);
            this.Controls.Add(this.chk_MultiTemplates);
            this.Controls.Add(this.chk_WhiteOnBlack);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedMarkOCRForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMCheckBox chk_WhiteOnBlack;
        private SRMControl.SRMCheckBox chk_MultiTemplates;
        private SRMControl.SRMCheckBox chk_RemoveBorderMode;
        private SRMControl.SRMCheckBox chk_RecogCharPosition;
        private SRMControl.SRMCheckBox chk_SkipMarkInspection;
    }
}