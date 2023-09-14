namespace VisionProcessForm
{
    partial class SetCharsExcessMissAreaValueForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetCharsExcessMissAreaValueForm));
            this.txt_SetValue = new SRMControl.SRMInputBox();
            this.lbl_Definition = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.lbl_SetValue = new SRMControl.SRMLabel();
            this.chk_SetAllRows = new SRMControl.SRMCheckBox();
            this.SuspendLayout();
            // 
            // txt_SetValue
            // 
            resources.ApplyResources(this.txt_SetValue, "txt_SetValue");
            this.txt_SetValue.BackColor = System.Drawing.Color.White;
            this.txt_SetValue.DecimalPlaces = 0;
            this.txt_SetValue.DecMaxValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txt_SetValue.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SetValue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SetValue.ForeColor = System.Drawing.Color.Black;
            this.txt_SetValue.InputType = SRMControl.InputType.Number;
            this.txt_SetValue.Name = "txt_SetValue";
            this.txt_SetValue.NormalBackColor = System.Drawing.Color.White;
            // 
            // lbl_Definition
            // 
            resources.ApplyResources(this.lbl_Definition, "lbl_Definition");
            this.lbl_Definition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Definition.Name = "lbl_Definition";
            this.lbl_Definition.TextShadowColor = System.Drawing.Color.Gray;
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
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_SetValue
            // 
            resources.ApplyResources(this.lbl_SetValue, "lbl_SetValue");
            this.lbl_SetValue.Name = "lbl_SetValue";
            this.lbl_SetValue.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_SetAllRows
            // 
            resources.ApplyResources(this.chk_SetAllRows, "chk_SetAllRows");
            this.chk_SetAllRows.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetAllRows.Name = "chk_SetAllRows";
            this.chk_SetAllRows.Selected = false;
            this.chk_SetAllRows.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetAllRows.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetAllRows.UseVisualStyleBackColor = true;
            // 
            // SetCharsExcessMissAreaValueForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.chk_SetAllRows);
            this.Controls.Add(this.txt_SetValue);
            this.Controls.Add(this.lbl_SetValue);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.lbl_Definition);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.srmLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetCharsExcessMissAreaValueForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMInputBox txt_SetValue;
        private SRMControl.SRMLabel lbl_Definition;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel lbl_SetValue;
        private SRMControl.SRMCheckBox chk_SetAllRows;
    }
}