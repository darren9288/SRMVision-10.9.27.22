namespace AutoMode
{
    partial class SaveOptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveOptionForm));
            this.chk_SaveRecipe = new SRMControl.SRMCheckBox();
            this.chk_SaveLogFile = new SRMControl.SRMCheckBox();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_SelectedPath = new System.Windows.Forms.TextBox();
            this.btn_Browse = new SRMControl.SRMButton();
            this.chk_SaveCurrentImage = new SRMControl.SRMCheckBox();
            this.chk_SaveEditLogFile = new SRMControl.SRMCheckBox();
            this.dlg_SaveImageFile = new System.Windows.Forms.SaveFileDialog();
            this.txt_ImageName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbo_RecipeSaveMode = new SRMControl.SRMComboBox();
            this.SuspendLayout();
            // 
            // chk_SaveRecipe
            // 
            resources.ApplyResources(this.chk_SaveRecipe, "chk_SaveRecipe");
            this.chk_SaveRecipe.Checked = true;
            this.chk_SaveRecipe.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SaveRecipe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_SaveRecipe.Name = "chk_SaveRecipe";
            this.chk_SaveRecipe.Selected = false;
            this.chk_SaveRecipe.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveRecipe.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SaveRecipe.UseVisualStyleBackColor = true;
            this.chk_SaveRecipe.CheckedChanged += new System.EventHandler(this.chk_SaveRecipe_CheckedChanged);
            // 
            // chk_SaveLogFile
            // 
            resources.ApplyResources(this.chk_SaveLogFile, "chk_SaveLogFile");
            this.chk_SaveLogFile.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SaveLogFile.Name = "chk_SaveLogFile";
            this.chk_SaveLogFile.Selected = false;
            this.chk_SaveLogFile.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveLogFile.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SaveLogFile.UseVisualStyleBackColor = true;
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
            // folderBrowserDialog1
            // 
            resources.ApplyResources(this.folderBrowserDialog1, "folderBrowserDialog1");
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txt_SelectedPath
            // 
            resources.ApplyResources(this.txt_SelectedPath, "txt_SelectedPath");
            this.txt_SelectedPath.Name = "txt_SelectedPath";
            // 
            // btn_Browse
            // 
            resources.ApplyResources(this.btn_Browse, "btn_Browse");
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // chk_SaveCurrentImage
            // 
            resources.ApplyResources(this.chk_SaveCurrentImage, "chk_SaveCurrentImage");
            this.chk_SaveCurrentImage.Checked = true;
            this.chk_SaveCurrentImage.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SaveCurrentImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_SaveCurrentImage.Name = "chk_SaveCurrentImage";
            this.chk_SaveCurrentImage.Selected = false;
            this.chk_SaveCurrentImage.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveCurrentImage.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SaveCurrentImage.UseVisualStyleBackColor = true;
            // 
            // chk_SaveEditLogFile
            // 
            resources.ApplyResources(this.chk_SaveEditLogFile, "chk_SaveEditLogFile");
            this.chk_SaveEditLogFile.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SaveEditLogFile.Name = "chk_SaveEditLogFile";
            this.chk_SaveEditLogFile.Selected = false;
            this.chk_SaveEditLogFile.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SaveEditLogFile.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SaveEditLogFile.UseVisualStyleBackColor = true;
            // 
            // dlg_SaveImageFile
            // 
            resources.ApplyResources(this.dlg_SaveImageFile, "dlg_SaveImageFile");
            // 
            // txt_ImageName
            // 
            resources.ApplyResources(this.txt_ImageName, "txt_ImageName");
            this.txt_ImageName.Name = "txt_ImageName";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cbo_RecipeSaveMode
            // 
            resources.ApplyResources(this.cbo_RecipeSaveMode, "cbo_RecipeSaveMode");
            this.cbo_RecipeSaveMode.BackColor = System.Drawing.Color.White;
            this.cbo_RecipeSaveMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_RecipeSaveMode.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_RecipeSaveMode.FormattingEnabled = true;
            this.cbo_RecipeSaveMode.Items.AddRange(new object[] {
            resources.GetString("cbo_RecipeSaveMode.Items"),
            resources.GetString("cbo_RecipeSaveMode.Items1")});
            this.cbo_RecipeSaveMode.Name = "cbo_RecipeSaveMode";
            this.cbo_RecipeSaveMode.NormalBackColor = System.Drawing.Color.White;
            // 
            // SaveOptionForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.cbo_RecipeSaveMode);
            this.Controls.Add(this.txt_ImageName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chk_SaveEditLogFile);
            this.Controls.Add(this.chk_SaveCurrentImage);
            this.Controls.Add(this.btn_Browse);
            this.Controls.Add(this.txt_SelectedPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.chk_SaveRecipe);
            this.Controls.Add(this.chk_SaveLogFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SaveOptionForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMCheckBox chk_SaveRecipe;
        private SRMControl.SRMCheckBox chk_SaveLogFile;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_SelectedPath;
        private SRMControl.SRMButton btn_Browse;
        private SRMControl.SRMCheckBox chk_SaveCurrentImage;
        private SRMControl.SRMCheckBox chk_SaveEditLogFile;
        private System.Windows.Forms.SaveFileDialog dlg_SaveImageFile;
        private System.Windows.Forms.TextBox txt_ImageName;
        private System.Windows.Forms.Label label2;
        private SRMControl.SRMComboBox cbo_RecipeSaveMode;
    }
}