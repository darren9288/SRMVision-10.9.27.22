namespace AutoMode
{
    partial class RecipeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecipeForm));
            this.txt_RecipeSelected = new SRMControl.SRMTextBox();
            this.txt_NewRecipe = new SRMControl.SRMTextBox();
            this.btn_Close = new SRMControl.SRMButton();
            this.btn_Rename = new SRMControl.SRMButton();
            this.btn_CopyLocal = new SRMControl.SRMButton();
            this.btn_Delete = new SRMControl.SRMButton();
            this.btn_Select = new SRMControl.SRMButton();
            this.btn_New = new SRMControl.SRMButton();
            this.lst_RecipeAvailable = new SRMControl.SRMListBox();
            this.label3 = new SRMControl.SRMLabel();
            this.label2 = new SRMControl.SRMLabel();
            this.label1 = new SRMControl.SRMLabel();
            this.btn_CopyServer = new SRMControl.SRMButton();
            this.lbl_Source = new SRMControl.SRMLabel();
            this.radioBtn_NetworkRecipe = new SRMControl.SRMRadioButton();
            this.radioBtn_LocalRecipe = new SRMControl.SRMRadioButton();
            this.btn_Copy = new SRMControl.SRMButton();
            this.btn_Import = new SRMControl.SRMButton();
            this.btn_Export = new SRMControl.SRMButton();
            this.btn_CopyToExisting = new SRMControl.SRMButton();
            this.SuspendLayout();
            // 
            // txt_RecipeSelected
            // 
            resources.ApplyResources(this.txt_RecipeSelected, "txt_RecipeSelected");
            this.txt_RecipeSelected.BackColor = System.Drawing.Color.White;
            this.txt_RecipeSelected.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_RecipeSelected.Name = "txt_RecipeSelected";
            this.txt_RecipeSelected.NormalBackColor = System.Drawing.Color.White;
            // 
            // txt_NewRecipe
            // 
            resources.ApplyResources(this.txt_NewRecipe, "txt_NewRecipe");
            this.txt_NewRecipe.BackColor = System.Drawing.Color.White;
            this.txt_NewRecipe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txt_NewRecipe.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_NewRecipe.Name = "txt_NewRecipe";
            this.txt_NewRecipe.NormalBackColor = System.Drawing.Color.White;
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Rename
            // 
            resources.ApplyResources(this.btn_Rename, "btn_Rename");
            this.btn_Rename.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Rename.Name = "btn_Rename";
            this.btn_Rename.Click += new System.EventHandler(this.btn_Rename_Click);
            // 
            // btn_CopyLocal
            // 
            resources.ApplyResources(this.btn_CopyLocal, "btn_CopyLocal");
            this.btn_CopyLocal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CopyLocal.Name = "btn_CopyLocal";
            this.btn_CopyLocal.Click += new System.EventHandler(this.btn_Copy_Click);
            // 
            // btn_Delete
            // 
            resources.ApplyResources(this.btn_Delete, "btn_Delete");
            this.btn_Delete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_Select
            // 
            resources.ApplyResources(this.btn_Select, "btn_Select");
            this.btn_Select.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Select.Name = "btn_Select";
            this.btn_Select.Click += new System.EventHandler(this.btn_Select_Click);
            // 
            // btn_New
            // 
            resources.ApplyResources(this.btn_New, "btn_New");
            this.btn_New.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_New.Name = "btn_New";
            this.btn_New.Click += new System.EventHandler(this.btn_New_Click);
            // 
            // lst_RecipeAvailable
            // 
            resources.ApplyResources(this.lst_RecipeAvailable, "lst_RecipeAvailable");
            this.lst_RecipeAvailable.BackColor = System.Drawing.Color.White;
            this.lst_RecipeAvailable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lst_RecipeAvailable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lst_RecipeAvailable.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lst_RecipeAvailable.Name = "lst_RecipeAvailable";
            this.lst_RecipeAvailable.NormalBackColor = System.Drawing.Color.White;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_CopyServer
            // 
            resources.ApplyResources(this.btn_CopyServer, "btn_CopyServer");
            this.btn_CopyServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CopyServer.Name = "btn_CopyServer";
            this.btn_CopyServer.Click += new System.EventHandler(this.btn_CopyServer_Click);
            // 
            // lbl_Source
            // 
            resources.ApplyResources(this.lbl_Source, "lbl_Source");
            this.lbl_Source.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Source.Name = "lbl_Source";
            this.lbl_Source.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // radioBtn_NetworkRecipe
            // 
            resources.ApplyResources(this.radioBtn_NetworkRecipe, "radioBtn_NetworkRecipe");
            this.radioBtn_NetworkRecipe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioBtn_NetworkRecipe.Name = "radioBtn_NetworkRecipe";
            this.radioBtn_NetworkRecipe.Tag = "4";
            this.radioBtn_NetworkRecipe.UseVisualStyleBackColor = true;
            this.radioBtn_NetworkRecipe.Click += new System.EventHandler(this.radioBtn_Click);
            // 
            // radioBtn_LocalRecipe
            // 
            resources.ApplyResources(this.radioBtn_LocalRecipe, "radioBtn_LocalRecipe");
            this.radioBtn_LocalRecipe.Checked = true;
            this.radioBtn_LocalRecipe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioBtn_LocalRecipe.Name = "radioBtn_LocalRecipe";
            this.radioBtn_LocalRecipe.TabStop = true;
            this.radioBtn_LocalRecipe.Tag = "4";
            this.radioBtn_LocalRecipe.UseVisualStyleBackColor = true;
            this.radioBtn_LocalRecipe.Click += new System.EventHandler(this.radioBtn_Click);
            // 
            // btn_Copy
            // 
            resources.ApplyResources(this.btn_Copy, "btn_Copy");
            this.btn_Copy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Copy.Name = "btn_Copy";
            this.btn_Copy.Click += new System.EventHandler(this.btn_Copy_Click);
            // 
            // btn_Import
            // 
            resources.ApplyResources(this.btn_Import, "btn_Import");
            this.btn_Import.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Import.Name = "btn_Import";
            this.btn_Import.Click += new System.EventHandler(this.btn_LoadFrom_Click);
            // 
            // btn_Export
            // 
            resources.ApplyResources(this.btn_Export, "btn_Export");
            this.btn_Export.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Export.Name = "btn_Export";
            this.btn_Export.Click += new System.EventHandler(this.btn_SaveAs_Click);
            // 
            // btn_CopyToExisting
            // 
            resources.ApplyResources(this.btn_CopyToExisting, "btn_CopyToExisting");
            this.btn_CopyToExisting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_CopyToExisting.Name = "btn_CopyToExisting";
            this.btn_CopyToExisting.Click += new System.EventHandler(this.btn_CopyToExisting_Click);
            // 
            // RecipeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_CopyToExisting);
            this.Controls.Add(this.btn_Export);
            this.Controls.Add(this.btn_Import);
            this.Controls.Add(this.btn_Copy);
            this.Controls.Add(this.radioBtn_NetworkRecipe);
            this.Controls.Add(this.radioBtn_LocalRecipe);
            this.Controls.Add(this.lbl_Source);
            this.Controls.Add(this.btn_CopyServer);
            this.Controls.Add(this.txt_RecipeSelected);
            this.Controls.Add(this.txt_NewRecipe);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Rename);
            this.Controls.Add(this.btn_CopyLocal);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Select);
            this.Controls.Add(this.btn_New);
            this.Controls.Add(this.lst_RecipeAvailable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RecipeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMTextBox txt_RecipeSelected;
        private SRMControl.SRMTextBox txt_NewRecipe;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMButton btn_Rename;
        private SRMControl.SRMButton btn_CopyLocal;
        private SRMControl.SRMButton btn_Delete;
        private SRMControl.SRMButton btn_Select;
        private SRMControl.SRMButton btn_New;
        private SRMControl.SRMListBox lst_RecipeAvailable;
        private SRMControl.SRMLabel label3;
        private SRMControl.SRMLabel label2;
        private SRMControl.SRMLabel label1;
        private SRMControl.SRMButton btn_CopyServer;
        private SRMControl.SRMLabel lbl_Source;
        private SRMControl.SRMRadioButton radioBtn_NetworkRecipe;
        private SRMControl.SRMRadioButton radioBtn_LocalRecipe;
        private SRMControl.SRMButton btn_Copy;
        private SRMControl.SRMButton btn_Import;
        private SRMControl.SRMButton btn_Export;
        private SRMControl.SRMButton btn_CopyToExisting;
    }
}