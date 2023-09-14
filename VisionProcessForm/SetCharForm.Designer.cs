namespace VisionProcessForm
{
    partial class SetCharForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetCharForm));
            this.lbl_Character = new SRMControl.SRMLabel();
            this.txt_Character = new SRMControl.SRMInputBox();
            this.lbl_Class = new SRMControl.SRMLabel();
            this.cbo_Class = new SRMControl.SRMComboBox();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.SuspendLayout();
            // 
            // lbl_Character
            // 
            resources.ApplyResources(this.lbl_Character, "lbl_Character");
            this.lbl_Character.Name = "lbl_Character";
            this.lbl_Character.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Character
            // 
            this.txt_Character.BackColor = System.Drawing.Color.White;
            this.txt_Character.DecimalPlaces = 0;
            this.txt_Character.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_Character.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Character.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_Character, "txt_Character");
            this.txt_Character.ForeColor = System.Drawing.Color.Black;
            this.txt_Character.Name = "txt_Character";
            this.txt_Character.NormalBackColor = System.Drawing.Color.White;
            this.txt_Character.TextChanged += new System.EventHandler(this.txt_Character_TextChanged);
            // 
            // lbl_Class
            // 
            resources.ApplyResources(this.lbl_Class, "lbl_Class");
            this.lbl_Class.Name = "lbl_Class";
            this.lbl_Class.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_Class
            // 
            this.cbo_Class.BackColor = System.Drawing.Color.White;
            this.cbo_Class.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Class.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_Class.FormattingEnabled = true;
            resources.ApplyResources(this.cbo_Class, "cbo_Class");
            this.cbo_Class.Items.AddRange(new object[] {
            resources.GetString("cbo_Class.Items"),
            resources.GetString("cbo_Class.Items1"),
            resources.GetString("cbo_Class.Items2"),
            resources.GetString("cbo_Class.Items3")});
            this.cbo_Class.Name = "cbo_Class";
            this.cbo_Class.NormalBackColor = System.Drawing.Color.White;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // SetCharForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.lbl_Class);
            this.Controls.Add(this.cbo_Class);
            this.Controls.Add(this.lbl_Character);
            this.Controls.Add(this.txt_Character);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetCharForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel lbl_Character;
        private SRMControl.SRMInputBox txt_Character;
        private SRMControl.SRMLabel lbl_Class;
        private SRMControl.SRMComboBox cbo_Class;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
    }
}