namespace VisionProcessForm
{
    partial class AdvancedPositionForm
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
            this.radioBtn_2Directions = new SRMControl.SRMRadioButton();
            this.radioBtn_4Directions = new SRMControl.SRMRadioButton();
            this.srmGroupBox1 = new SRMControl.SRMGroupBox();
            this.srmGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(152, 161);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 18;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(66, 161);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 34);
            this.btn_OK.TabIndex = 17;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // radioBtn_2Directions
            // 
            this.radioBtn_2Directions.Location = new System.Drawing.Point(6, 19);
            this.radioBtn_2Directions.Name = "radioBtn_2Directions";
            this.radioBtn_2Directions.Size = new System.Drawing.Size(148, 24);
            this.radioBtn_2Directions.TabIndex = 19;
            this.radioBtn_2Directions.TabStop = true;
            this.radioBtn_2Directions.Text = "Two (0, 180 in Degree)";
            this.radioBtn_2Directions.UseVisualStyleBackColor = true;
            // 
            // radioBtn_4Directions
            // 
            this.radioBtn_4Directions.Checked = true;
            this.radioBtn_4Directions.Location = new System.Drawing.Point(6, 58);
            this.radioBtn_4Directions.Name = "radioBtn_4Directions";
            this.radioBtn_4Directions.Size = new System.Drawing.Size(201, 24);
            this.radioBtn_4Directions.TabIndex = 20;
            this.radioBtn_4Directions.TabStop = true;
            this.radioBtn_4Directions.Text = "Four (0, 90, 180, 270 in Degree)";
            this.radioBtn_4Directions.UseVisualStyleBackColor = true;
            // 
            // srmGroupBox1
            // 
            this.srmGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox1.Controls.Add(this.radioBtn_4Directions);
            this.srmGroupBox1.Controls.Add(this.radioBtn_2Directions);
            this.srmGroupBox1.Location = new System.Drawing.Point(6, 12);
            this.srmGroupBox1.Name = "srmGroupBox1";
            this.srmGroupBox1.Size = new System.Drawing.Size(226, 123);
            this.srmGroupBox1.TabIndex = 21;
            this.srmGroupBox1.TabStop = false;
            this.srmGroupBox1.Text = "Directions";
            // 
            // AdvancedPositionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(244, 212);
            this.ControlBox = false;
            this.Controls.Add(this.srmGroupBox1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedPositionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AdvancedPositionForm";
            this.srmGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMRadioButton radioBtn_2Directions;
        private SRMControl.SRMRadioButton radioBtn_4Directions;
        private SRMControl.SRMGroupBox srmGroupBox1;
    }
}