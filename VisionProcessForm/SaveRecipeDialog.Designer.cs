namespace VisionProcessForm
{
    partial class SaveRecipeDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveRecipeDialog));
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.lvw_Recipe = new SRMControl.SRMListView();
            this.ils_ImageIcon = new System.Windows.Forms.ImageList(this.components);
            this.lbl_RecipeName = new SRMControl.SRMLabel();
            this.txt_RecipeName = new SRMControl.SRMTextBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.cbo_SaveAsType = new SRMControl.SRMComboBox();
            this.SuspendLayout();
            // 
            // btn_Save
            // 
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // lvw_Recipe
            // 
            this.lvw_Recipe.EnableChecked = true;
            this.lvw_Recipe.LargeImageList = this.ils_ImageIcon;
            resources.ApplyResources(this.lvw_Recipe, "lvw_Recipe");
            this.lvw_Recipe.MultiSelect = false;
            this.lvw_Recipe.Name = "lvw_Recipe";
            this.lvw_Recipe.SmallImageList = this.ils_ImageIcon;
            this.lvw_Recipe.UseCompatibleStateImageBehavior = false;
            this.lvw_Recipe.View = System.Windows.Forms.View.List;
            this.lvw_Recipe.SelectedIndexChanged += new System.EventHandler(this.lvw_Recipe_SelectedIndexChanged);
            // 
            // ils_ImageIcon
            // 
            this.ils_ImageIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ils_ImageIcon.ImageStream")));
            this.ils_ImageIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.ils_ImageIcon.Images.SetKeyName(0, "Recipe.png");
            // 
            // lbl_RecipeName
            // 
            resources.ApplyResources(this.lbl_RecipeName, "lbl_RecipeName");
            this.lbl_RecipeName.Name = "lbl_RecipeName";
            this.lbl_RecipeName.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_RecipeName
            // 
            this.txt_RecipeName.BackColor = System.Drawing.Color.White;
            this.txt_RecipeName.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_RecipeName, "txt_RecipeName");
            this.txt_RecipeName.Name = "txt_RecipeName";
            this.txt_RecipeName.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_SaveAsType
            // 
            this.cbo_SaveAsType.BackColor = System.Drawing.Color.White;
            this.cbo_SaveAsType.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SaveAsType.FormattingEnabled = true;
            this.cbo_SaveAsType.Items.AddRange(new object[] {
            resources.GetString("cbo_SaveAsType.Items"),
            resources.GetString("cbo_SaveAsType.Items1")});
            resources.ApplyResources(this.cbo_SaveAsType, "cbo_SaveAsType");
            this.cbo_SaveAsType.Name = "cbo_SaveAsType";
            this.cbo_SaveAsType.NormalBackColor = System.Drawing.Color.White;
            // 
            // SaveRecipeDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.cbo_SaveAsType);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.txt_RecipeName);
            this.Controls.Add(this.lbl_RecipeName);
            this.Controls.Add(this.lvw_Recipe);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SaveRecipeDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.RecipeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMListView lvw_Recipe;
        private SRMControl.SRMLabel lbl_RecipeName;
        private SRMControl.SRMTextBox txt_RecipeName;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMComboBox cbo_SaveAsType;
        private System.Windows.Forms.ImageList ils_ImageIcon;
    }
}