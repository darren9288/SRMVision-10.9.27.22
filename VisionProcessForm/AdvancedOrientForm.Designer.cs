namespace VisionProcessForm
{
    partial class AdvancedOrientForm
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
            this.group_OrientDirections = new SRMControl.SRMGroupBox();
            this.radioBtn_4Directions = new SRMControl.SRMRadioButton();
            this.radioBtn_2Directions = new SRMControl.SRMRadioButton();
            this.chk_WantSubROI = new SRMControl.SRMCheckBox();
            this.group_OrientDirections.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(154, 185);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 14;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(68, 185);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 34);
            this.btn_OK.TabIndex = 13;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // group_OrientDirections
            // 
            this.group_OrientDirections.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_OrientDirections.Controls.Add(this.radioBtn_4Directions);
            this.group_OrientDirections.Controls.Add(this.radioBtn_2Directions);
            this.group_OrientDirections.Location = new System.Drawing.Point(8, 12);
            this.group_OrientDirections.Name = "group_OrientDirections";
            this.group_OrientDirections.Size = new System.Drawing.Size(226, 123);
            this.group_OrientDirections.TabIndex = 15;
            this.group_OrientDirections.TabStop = false;
            this.group_OrientDirections.Text = "Directions";
            // 
            // radioBtn_4Directions
            // 
            this.radioBtn_4Directions.Checked = true;
            this.radioBtn_4Directions.Location = new System.Drawing.Point(19, 71);
            this.radioBtn_4Directions.Name = "radioBtn_4Directions";
            this.radioBtn_4Directions.Size = new System.Drawing.Size(201, 24);
            this.radioBtn_4Directions.TabIndex = 1;
            this.radioBtn_4Directions.TabStop = true;
            this.radioBtn_4Directions.Text = "Four (0, 90, 180, 270 in Degree)";
            this.radioBtn_4Directions.UseVisualStyleBackColor = true;
            // 
            // radioBtn_2Directions
            // 
            this.radioBtn_2Directions.Location = new System.Drawing.Point(19, 32);
            this.radioBtn_2Directions.Name = "radioBtn_2Directions";
            this.radioBtn_2Directions.Size = new System.Drawing.Size(148, 24);
            this.radioBtn_2Directions.TabIndex = 0;
            this.radioBtn_2Directions.TabStop = true;
            this.radioBtn_2Directions.Text = "Two (0, 180 in Degree)";
            this.radioBtn_2Directions.UseVisualStyleBackColor = true;
            // 
            // chk_WantSubROI
            // 
            this.chk_WantSubROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WantSubROI.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_WantSubROI.Location = new System.Drawing.Point(8, 141);
            this.chk_WantSubROI.Name = "chk_WantSubROI";
            this.chk_WantSubROI.Selected = false;
            this.chk_WantSubROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantSubROI.Size = new System.Drawing.Size(184, 24);
            this.chk_WantSubROI.TabIndex = 142;
            this.chk_WantSubROI.Text = "Want SubROI";
            this.chk_WantSubROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantSubROI.UseVisualStyleBackColor = true;
            // 
            // AdvancedOrientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(244, 231);
            this.ControlBox = false;
            this.Controls.Add(this.chk_WantSubROI);
            this.Controls.Add(this.group_OrientDirections);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedOrientForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Settings";
            this.group_OrientDirections.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMGroupBox group_OrientDirections;
        private SRMControl.SRMRadioButton radioBtn_4Directions;
        private SRMControl.SRMRadioButton radioBtn_2Directions;
        private SRMControl.SRMCheckBox chk_WantSubROI;
    }
}