namespace VisionProcessForm
{
    partial class AdvancedCalibrationForm
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
            this.chk_Skewed = new SRMControl.SRMCheckBox();
            this.chk_Scaled = new SRMControl.SRMCheckBox();
            this.chk_Anisotropic = new SRMControl.SRMCheckBox();
            this.chk_Tilted = new SRMControl.SRMCheckBox();
            this.chk_Inverse = new SRMControl.SRMCheckBox();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.chk_Radial = new SRMControl.SRMCheckBox();
            this.srmGroupBox1 = new SRMControl.SRMGroupBox();
            this.srmGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chk_Skewed
            // 
            this.chk_Skewed.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Skewed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_Skewed.Location = new System.Drawing.Point(18, 79);
            this.chk_Skewed.Name = "chk_Skewed";
            this.chk_Skewed.Selected = false;
            this.chk_Skewed.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Skewed.Size = new System.Drawing.Size(103, 24);
            this.chk_Skewed.TabIndex = 2;
            this.chk_Skewed.Text = "Skewed";
            this.chk_Skewed.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Skewed.UseVisualStyleBackColor = true;
            // 
            // chk_Scaled
            // 
            this.chk_Scaled.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Scaled.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_Scaled.Location = new System.Drawing.Point(18, 49);
            this.chk_Scaled.Name = "chk_Scaled";
            this.chk_Scaled.Selected = false;
            this.chk_Scaled.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Scaled.Size = new System.Drawing.Size(103, 24);
            this.chk_Scaled.TabIndex = 1;
            this.chk_Scaled.Text = "Scaled";
            this.chk_Scaled.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Scaled.UseVisualStyleBackColor = true;
            // 
            // chk_Anisotropic
            // 
            this.chk_Anisotropic.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Anisotropic.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_Anisotropic.Location = new System.Drawing.Point(148, 19);
            this.chk_Anisotropic.Name = "chk_Anisotropic";
            this.chk_Anisotropic.Selected = true;
            this.chk_Anisotropic.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Anisotropic.Size = new System.Drawing.Size(119, 24);
            this.chk_Anisotropic.TabIndex = 3;
            this.chk_Anisotropic.Text = "Anisotropic";
            this.chk_Anisotropic.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Anisotropic.UseVisualStyleBackColor = true;
            // 
            // chk_Tilted
            // 
            this.chk_Tilted.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Tilted.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_Tilted.Location = new System.Drawing.Point(148, 49);
            this.chk_Tilted.Name = "chk_Tilted";
            this.chk_Tilted.Selected = false;
            this.chk_Tilted.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Tilted.Size = new System.Drawing.Size(119, 24);
            this.chk_Tilted.TabIndex = 4;
            this.chk_Tilted.Text = "Tilted";
            this.chk_Tilted.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Tilted.UseVisualStyleBackColor = true;
            this.chk_Tilted.Visible = false;
            // 
            // chk_Inverse
            // 
            this.chk_Inverse.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Inverse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_Inverse.Location = new System.Drawing.Point(18, 19);
            this.chk_Inverse.Name = "chk_Inverse";
            this.chk_Inverse.Selected = false;
            this.chk_Inverse.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Inverse.Size = new System.Drawing.Size(103, 24);
            this.chk_Inverse.TabIndex = 0;
            this.chk_Inverse.Text = "Inverse";
            this.chk_Inverse.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Inverse.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(211, 149);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 140;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(125, 149);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 34);
            this.btn_OK.TabIndex = 139;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // chk_Radial
            // 
            this.chk_Radial.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Radial.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_Radial.Location = new System.Drawing.Point(148, 79);
            this.chk_Radial.Name = "chk_Radial";
            this.chk_Radial.Selected = false;
            this.chk_Radial.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Radial.Size = new System.Drawing.Size(119, 24);
            this.chk_Radial.TabIndex = 5;
            this.chk_Radial.Text = "Radial";
            this.chk_Radial.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Radial.UseVisualStyleBackColor = true;
            this.chk_Radial.Visible = false;
            // 
            // srmGroupBox1
            // 
            this.srmGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox1.Controls.Add(this.chk_Inverse);
            this.srmGroupBox1.Controls.Add(this.chk_Radial);
            this.srmGroupBox1.Controls.Add(this.chk_Tilted);
            this.srmGroupBox1.Controls.Add(this.chk_Anisotropic);
            this.srmGroupBox1.Controls.Add(this.chk_Scaled);
            this.srmGroupBox1.Controls.Add(this.chk_Skewed);
            this.srmGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.srmGroupBox1.Name = "srmGroupBox1";
            this.srmGroupBox1.Size = new System.Drawing.Size(279, 120);
            this.srmGroupBox1.TabIndex = 141;
            this.srmGroupBox1.TabStop = false;
            this.srmGroupBox1.Text = "Calibration Mode";
            // 
            // AdvancedCalibrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(311, 444);
            this.ControlBox = false;
            this.Controls.Add(this.srmGroupBox1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedCalibrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Settings";
            this.srmGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMCheckBox chk_Skewed;
        private SRMControl.SRMCheckBox chk_Scaled;
        private SRMControl.SRMCheckBox chk_Anisotropic;
        private SRMControl.SRMCheckBox chk_Tilted;
        private SRMControl.SRMCheckBox chk_Inverse;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMCheckBox chk_Radial;
        private SRMControl.SRMGroupBox srmGroupBox1;
    }
}