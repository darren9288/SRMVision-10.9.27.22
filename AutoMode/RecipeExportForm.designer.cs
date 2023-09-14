namespace AutoMode
{
    partial class RecipeExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecipeExportForm));
            this.btn_Browse = new SRMControl.SRMButton();
            this.txt_SelectedPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chk_SaveLearnedSetting = new SRMControl.SRMCheckBox();
            this.chk_SaveCalibrationSetting = new SRMControl.SRMCheckBox();
            this.chk_SaveToleranceSetting = new SRMControl.SRMCheckBox();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.chk_SaveLighting = new SRMControl.SRMCheckBox();
            this.chk_Vision1 = new SRMControl.SRMCheckBox();
            this.chk_Vision2 = new SRMControl.SRMCheckBox();
            this.chk_Vision3 = new SRMControl.SRMCheckBox();
            this.chk_Vision4 = new SRMControl.SRMCheckBox();
            this.chk_Vision5 = new SRMControl.SRMCheckBox();
            this.chk_Vision10 = new SRMControl.SRMCheckBox();
            this.chk_Vision6 = new SRMControl.SRMCheckBox();
            this.chk_Vision7 = new SRMControl.SRMCheckBox();
            this.chk_Vision8 = new SRMControl.SRMCheckBox();
            this.chk_Vision9 = new SRMControl.SRMCheckBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_RecipeID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_Browse
            // 
            resources.ApplyResources(this.btn_Browse, "btn_Browse");
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // txt_SelectedPath
            // 
            resources.ApplyResources(this.txt_SelectedPath, "txt_SelectedPath");
            this.txt_SelectedPath.Name = "txt_SelectedPath";
            this.txt_SelectedPath.TextChanged += new System.EventHandler(this.txt_SelectedPath_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // chk_SaveLearnedSetting
            // 
            resources.ApplyResources(this.chk_SaveLearnedSetting, "chk_SaveLearnedSetting");
            this.chk_SaveLearnedSetting.Checked = true;
            this.chk_SaveLearnedSetting.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SaveLearnedSetting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_SaveLearnedSetting.Name = "chk_SaveLearnedSetting";
            this.chk_SaveLearnedSetting.Selected = false;
            this.chk_SaveLearnedSetting.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveLearnedSetting.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SaveLearnedSetting.UseVisualStyleBackColor = true;
            // 
            // chk_SaveCalibrationSetting
            // 
            resources.ApplyResources(this.chk_SaveCalibrationSetting, "chk_SaveCalibrationSetting");
            this.chk_SaveCalibrationSetting.Checked = true;
            this.chk_SaveCalibrationSetting.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SaveCalibrationSetting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_SaveCalibrationSetting.Name = "chk_SaveCalibrationSetting";
            this.chk_SaveCalibrationSetting.Selected = false;
            this.chk_SaveCalibrationSetting.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveCalibrationSetting.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SaveCalibrationSetting.UseVisualStyleBackColor = true;
            // 
            // chk_SaveToleranceSetting
            // 
            resources.ApplyResources(this.chk_SaveToleranceSetting, "chk_SaveToleranceSetting");
            this.chk_SaveToleranceSetting.Checked = true;
            this.chk_SaveToleranceSetting.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SaveToleranceSetting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_SaveToleranceSetting.Name = "chk_SaveToleranceSetting";
            this.chk_SaveToleranceSetting.Selected = false;
            this.chk_SaveToleranceSetting.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveToleranceSetting.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SaveToleranceSetting.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // chk_SaveLighting
            // 
            resources.ApplyResources(this.chk_SaveLighting, "chk_SaveLighting");
            this.chk_SaveLighting.Checked = true;
            this.chk_SaveLighting.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SaveLighting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_SaveLighting.Name = "chk_SaveLighting";
            this.chk_SaveLighting.Selected = false;
            this.chk_SaveLighting.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveLighting.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SaveLighting.UseVisualStyleBackColor = true;
            // 
            // chk_Vision1
            // 
            resources.ApplyResources(this.chk_Vision1, "chk_Vision1");
            this.chk_Vision1.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision1.Name = "chk_Vision1";
            this.chk_Vision1.Selected = false;
            this.chk_Vision1.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision1.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision1.UseVisualStyleBackColor = true;
            // 
            // chk_Vision2
            // 
            resources.ApplyResources(this.chk_Vision2, "chk_Vision2");
            this.chk_Vision2.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision2.Name = "chk_Vision2";
            this.chk_Vision2.Selected = false;
            this.chk_Vision2.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision2.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision2.UseVisualStyleBackColor = true;
            // 
            // chk_Vision3
            // 
            resources.ApplyResources(this.chk_Vision3, "chk_Vision3");
            this.chk_Vision3.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision3.Name = "chk_Vision3";
            this.chk_Vision3.Selected = false;
            this.chk_Vision3.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision3.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision3.UseVisualStyleBackColor = true;
            // 
            // chk_Vision4
            // 
            resources.ApplyResources(this.chk_Vision4, "chk_Vision4");
            this.chk_Vision4.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision4.Name = "chk_Vision4";
            this.chk_Vision4.Selected = false;
            this.chk_Vision4.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision4.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision4.UseVisualStyleBackColor = true;
            // 
            // chk_Vision5
            // 
            resources.ApplyResources(this.chk_Vision5, "chk_Vision5");
            this.chk_Vision5.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision5.Name = "chk_Vision5";
            this.chk_Vision5.Selected = false;
            this.chk_Vision5.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision5.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision5.UseVisualStyleBackColor = true;
            // 
            // chk_Vision10
            // 
            resources.ApplyResources(this.chk_Vision10, "chk_Vision10");
            this.chk_Vision10.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision10.Name = "chk_Vision10";
            this.chk_Vision10.Selected = false;
            this.chk_Vision10.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision10.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision10.UseVisualStyleBackColor = true;
            // 
            // chk_Vision6
            // 
            resources.ApplyResources(this.chk_Vision6, "chk_Vision6");
            this.chk_Vision6.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision6.Name = "chk_Vision6";
            this.chk_Vision6.Selected = false;
            this.chk_Vision6.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision6.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision6.UseVisualStyleBackColor = true;
            // 
            // chk_Vision7
            // 
            resources.ApplyResources(this.chk_Vision7, "chk_Vision7");
            this.chk_Vision7.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision7.Name = "chk_Vision7";
            this.chk_Vision7.Selected = false;
            this.chk_Vision7.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision7.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision7.UseVisualStyleBackColor = true;
            // 
            // chk_Vision8
            // 
            resources.ApplyResources(this.chk_Vision8, "chk_Vision8");
            this.chk_Vision8.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision8.Name = "chk_Vision8";
            this.chk_Vision8.Selected = false;
            this.chk_Vision8.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision8.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision8.UseVisualStyleBackColor = true;
            // 
            // chk_Vision9
            // 
            resources.ApplyResources(this.chk_Vision9, "chk_Vision9");
            this.chk_Vision9.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Vision9.Name = "chk_Vision9";
            this.chk_Vision9.Selected = false;
            this.chk_Vision9.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Vision9.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Vision9.UseVisualStyleBackColor = true;
            // 
            // folderBrowserDialog1
            // 
            resources.ApplyResources(this.folderBrowserDialog1, "folderBrowserDialog1");
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lbl_RecipeID
            // 
            resources.ApplyResources(this.lbl_RecipeID, "lbl_RecipeID");
            this.lbl_RecipeID.Name = "lbl_RecipeID";
            // 
            // RecipeExportForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.lbl_RecipeID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chk_Vision9);
            this.Controls.Add(this.chk_Vision8);
            this.Controls.Add(this.chk_Vision7);
            this.Controls.Add(this.chk_Vision6);
            this.Controls.Add(this.chk_Vision10);
            this.Controls.Add(this.chk_Vision5);
            this.Controls.Add(this.chk_Vision4);
            this.Controls.Add(this.chk_Vision3);
            this.Controls.Add(this.chk_Vision2);
            this.Controls.Add(this.chk_Vision1);
            this.Controls.Add(this.chk_SaveLighting);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.chk_SaveToleranceSetting);
            this.Controls.Add(this.chk_SaveLearnedSetting);
            this.Controls.Add(this.chk_SaveCalibrationSetting);
            this.Controls.Add(this.btn_Browse);
            this.Controls.Add(this.txt_SelectedPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecipeExportForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Browse;
        private System.Windows.Forms.TextBox txt_SelectedPath;
        private System.Windows.Forms.Label label1;
        private SRMControl.SRMCheckBox chk_SaveLearnedSetting;
        private SRMControl.SRMCheckBox chk_SaveCalibrationSetting;
        private SRMControl.SRMCheckBox chk_SaveToleranceSetting;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMCheckBox chk_SaveLighting;
        private SRMControl.SRMCheckBox chk_Vision1;
        private SRMControl.SRMCheckBox chk_Vision2;
        private SRMControl.SRMCheckBox chk_Vision3;
        private SRMControl.SRMCheckBox chk_Vision4;
        private SRMControl.SRMCheckBox chk_Vision5;
        private SRMControl.SRMCheckBox chk_Vision10;
        private SRMControl.SRMCheckBox chk_Vision6;
        private SRMControl.SRMCheckBox chk_Vision7;
        private SRMControl.SRMCheckBox chk_Vision8;
        private SRMControl.SRMCheckBox chk_Vision9;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_RecipeID;
    }
}