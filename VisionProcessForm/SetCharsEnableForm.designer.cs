namespace VisionProcessForm
{
    partial class SetCharsEnableForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetCharsEnableForm));
            this.lbl_Definition = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.chk_EnableDisable = new SRMControl.SRMCheckBox();
            this.chk_SetAllRows = new SRMControl.SRMCheckBox();
            this.SuspendLayout();
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
            // chk_EnableDisable
            // 
            resources.ApplyResources(this.chk_EnableDisable, "chk_EnableDisable");
            this.chk_EnableDisable.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_EnableDisable.Name = "chk_EnableDisable";
            this.chk_EnableDisable.Selected = false;
            this.chk_EnableDisable.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_EnableDisable.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_EnableDisable.UseVisualStyleBackColor = true;
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
            // SetCharsEnableForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.chk_SetAllRows);
            this.Controls.Add(this.chk_EnableDisable);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.lbl_Definition);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetCharsEnableForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion
        private SRMControl.SRMLabel lbl_Definition;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMCheckBox chk_EnableDisable;
        private SRMControl.SRMCheckBox chk_SetAllRows;
    }
}