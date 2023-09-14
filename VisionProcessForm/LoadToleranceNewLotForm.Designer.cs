namespace VisionProcessForm
{
    partial class LoadPadToleranceNewLotForm
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
            this.Txt_Browse = new SRMControl.SRMTextBox();
            this.label2 = new SRMControl.SRMLabel();
            this.Cancel1Button = new SRMControl.SRMButton();
            this.OkButton = new SRMControl.SRMButton();
            this.btn_Browse = new SRMControl.SRMButton();
            this.chk_WantLoadReference = new SRMControl.SRMCheckBox();
            this.chk_WantSavePad = new SRMControl.SRMCheckBox();
            this.chk_WantSavePadPackage = new SRMControl.SRMCheckBox();
            this.chk_WantSaveOthers = new SRMControl.SRMCheckBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.cbo_stolName = new SRMControl.SRMComboBox();
            this.SuspendLayout();
            // 
            // Txt_Browse
            // 
            this.Txt_Browse.BackColor = System.Drawing.Color.White;
            this.Txt_Browse.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Txt_Browse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Txt_Browse.Location = new System.Drawing.Point(68, 49);
            this.Txt_Browse.Name = "Txt_Browse";
            this.Txt_Browse.NormalBackColor = System.Drawing.Color.White;
            this.Txt_Browse.ReadOnly = true;
            this.Txt_Browse.Size = new System.Drawing.Size(316, 20);
            this.Txt_Browse.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(9, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Path:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // Cancel1Button
            // 
            this.Cancel1Button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Cancel1Button.Location = new System.Drawing.Point(369, 173);
            this.Cancel1Button.Name = "Cancel1Button";
            this.Cancel1Button.Size = new System.Drawing.Size(88, 40);
            this.Cancel1Button.TabIndex = 17;
            this.Cancel1Button.Text = "Cancel";
            this.Cancel1Button.Click += new System.EventHandler(this.Cancel1Button_Click);
            // 
            // OkButton
            // 
            this.OkButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OkButton.Location = new System.Drawing.Point(266, 173);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(88, 40);
            this.OkButton.TabIndex = 16;
            this.OkButton.Text = "Save";
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // btn_Browse
            // 
            this.btn_Browse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Browse.Location = new System.Drawing.Point(390, 38);
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Size = new System.Drawing.Size(88, 40);
            this.btn_Browse.TabIndex = 20;
            this.btn_Browse.Text = "Browse";
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // chk_WantLoadReference
            // 
            this.chk_WantLoadReference.Checked = true;
            this.chk_WantLoadReference.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WantLoadReference.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_WantLoadReference.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_WantLoadReference.Location = new System.Drawing.Point(12, 9);
            this.chk_WantLoadReference.Name = "chk_WantLoadReference";
            this.chk_WantLoadReference.Selected = false;
            this.chk_WantLoadReference.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantLoadReference.Size = new System.Drawing.Size(274, 34);
            this.chk_WantLoadReference.TabIndex = 878;
            this.chk_WantLoadReference.Text = "Load Refernce From Path When New Lot";
            this.chk_WantLoadReference.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantLoadReference.UseVisualStyleBackColor = true;
            // 
            // chk_WantSavePad
            // 
            this.chk_WantSavePad.Checked = true;
            this.chk_WantSavePad.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WantSavePad.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_WantSavePad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_WantSavePad.Location = new System.Drawing.Point(47, 75);
            this.chk_WantSavePad.Name = "chk_WantSavePad";
            this.chk_WantSavePad.Selected = false;
            this.chk_WantSavePad.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantSavePad.Size = new System.Drawing.Size(142, 34);
            this.chk_WantSavePad.TabIndex = 879;
            this.chk_WantSavePad.Text = "Pad";
            this.chk_WantSavePad.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantSavePad.UseVisualStyleBackColor = true;
            // 
            // chk_WantSavePadPackage
            // 
            this.chk_WantSavePadPackage.Checked = true;
            this.chk_WantSavePadPackage.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WantSavePadPackage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_WantSavePadPackage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_WantSavePadPackage.Location = new System.Drawing.Point(48, 101);
            this.chk_WantSavePadPackage.Name = "chk_WantSavePadPackage";
            this.chk_WantSavePadPackage.Selected = false;
            this.chk_WantSavePadPackage.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantSavePadPackage.Size = new System.Drawing.Size(142, 34);
            this.chk_WantSavePadPackage.TabIndex = 880;
            this.chk_WantSavePadPackage.Text = "Pad Package";
            this.chk_WantSavePadPackage.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantSavePadPackage.UseVisualStyleBackColor = true;
            // 
            // chk_WantSaveOthers
            // 
            this.chk_WantSaveOthers.Checked = true;
            this.chk_WantSaveOthers.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WantSaveOthers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_WantSaveOthers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_WantSaveOthers.Location = new System.Drawing.Point(47, 128);
            this.chk_WantSaveOthers.Name = "chk_WantSaveOthers";
            this.chk_WantSaveOthers.Selected = false;
            this.chk_WantSaveOthers.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantSaveOthers.Size = new System.Drawing.Size(142, 34);
            this.chk_WantSaveOthers.TabIndex = 881;
            this.chk_WantSaveOthers.Text = "Others";
            this.chk_WantSaveOthers.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantSaveOthers.UseVisualStyleBackColor = true;
            // 
            // srmLabel1
            // 
            this.srmLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel1.Location = new System.Drawing.Point(231, 96);
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.Size = new System.Drawing.Size(88, 13);
            this.srmLabel1.TabIndex = 882;
            this.srmLabel1.Text = "Tol File Name:";
            this.srmLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            this.srmLabel1.Visible = false;
            // 
            // cbo_stolName
            // 
            this.cbo_stolName.BackColor = System.Drawing.Color.White;
            this.cbo_stolName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_stolName.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_stolName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cbo_stolName.FormattingEnabled = true;
            this.cbo_stolName.Location = new System.Drawing.Point(226, 112);
            this.cbo_stolName.Name = "cbo_stolName";
            this.cbo_stolName.NormalBackColor = System.Drawing.Color.White;
            this.cbo_stolName.Size = new System.Drawing.Size(237, 23);
            this.cbo_stolName.TabIndex = 973;
            this.cbo_stolName.Visible = false;
            // 
            // LoadPadToleranceNewLotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(490, 243);
            this.Controls.Add(this.cbo_stolName);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.chk_WantSaveOthers);
            this.Controls.Add(this.chk_WantSavePadPackage);
            this.Controls.Add(this.chk_WantSavePad);
            this.Controls.Add(this.chk_WantLoadReference);
            this.Controls.Add(this.btn_Browse);
            this.Controls.Add(this.Txt_Browse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Cancel1Button);
            this.Controls.Add(this.OkButton);
            this.Name = "LoadPadToleranceNewLotForm";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.LoadPadToleranceNewLotForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMTextBox Txt_Browse;
        private SRMControl.SRMLabel label2;
        private SRMControl.SRMButton Cancel1Button;
        private SRMControl.SRMButton OkButton;
        private SRMControl.SRMButton btn_Browse;
        private SRMControl.SRMCheckBox chk_WantLoadReference;
        private SRMControl.SRMCheckBox chk_WantSavePad;
        private SRMControl.SRMCheckBox chk_WantSavePadPackage;
        private SRMControl.SRMCheckBox chk_WantSaveOthers;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMComboBox cbo_stolName;
    }
}