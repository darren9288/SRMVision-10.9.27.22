namespace VisionProcessForm
{
    partial class SetValueForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetValueForm));
            this.txt_SetValue = new SRMControl.SRMInputBox();
            this.lbl_Definition = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.chk_SetAllRows = new SRMControl.SRMCheckBox();
            this.chk_SetAllROI = new SRMControl.SRMCheckBox();
            this.chk_SetAllEdges = new SRMControl.SRMCheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnl_SetAllSameGroup = new System.Windows.Forms.Panel();
            this.chk_SetAllSameGroup = new SRMControl.SRMCheckBox();
            this.pnl_SetAllROI = new System.Windows.Forms.Panel();
            this.pnl_SetAllRows = new System.Windows.Forms.Panel();
            this.pnl_SetAllEdges = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.pnl_SetAllSameGroup.SuspendLayout();
            this.pnl_SetAllROI.SuspendLayout();
            this.pnl_SetAllRows.SuspendLayout();
            this.pnl_SetAllEdges.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_SetValue
            // 
            resources.ApplyResources(this.txt_SetValue, "txt_SetValue");
            this.txt_SetValue.BackColor = System.Drawing.Color.White;
            this.txt_SetValue.DecimalPlaces = 3;
            this.txt_SetValue.DecMaxValue = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.txt_SetValue.DecMinValue = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.txt_SetValue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SetValue.ForeColor = System.Drawing.Color.Black;
            this.txt_SetValue.InputType = SRMControl.InputType.Number;
            this.txt_SetValue.Name = "txt_SetValue";
            this.txt_SetValue.NormalBackColor = System.Drawing.Color.White;
            this.txt_SetValue.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txt_SetValue_MouseClick);
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
            // chk_SetAllRows
            // 
            resources.ApplyResources(this.chk_SetAllRows, "chk_SetAllRows");
            this.chk_SetAllRows.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetAllRows.Name = "chk_SetAllRows";
            this.chk_SetAllRows.Selected = false;
            this.chk_SetAllRows.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetAllRows.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetAllRows.UseVisualStyleBackColor = true;
            this.chk_SetAllRows.Click += new System.EventHandler(this.chk_SetAllRows_Click);
            // 
            // chk_SetAllROI
            // 
            resources.ApplyResources(this.chk_SetAllROI, "chk_SetAllROI");
            this.chk_SetAllROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetAllROI.Name = "chk_SetAllROI";
            this.chk_SetAllROI.Selected = false;
            this.chk_SetAllROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetAllROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetAllROI.UseVisualStyleBackColor = true;
            this.chk_SetAllROI.Click += new System.EventHandler(this.chk_SetAllROI_Click);
            // 
            // chk_SetAllEdges
            // 
            resources.ApplyResources(this.chk_SetAllEdges, "chk_SetAllEdges");
            this.chk_SetAllEdges.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetAllEdges.Name = "chk_SetAllEdges";
            this.chk_SetAllEdges.Selected = false;
            this.chk_SetAllEdges.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetAllEdges.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetAllEdges.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.pnl_SetAllSameGroup);
            this.panel1.Controls.Add(this.pnl_SetAllROI);
            this.panel1.Controls.Add(this.pnl_SetAllRows);
            this.panel1.Controls.Add(this.pnl_SetAllEdges);
            this.panel1.Name = "panel1";
            // 
            // pnl_SetAllSameGroup
            // 
            resources.ApplyResources(this.pnl_SetAllSameGroup, "pnl_SetAllSameGroup");
            this.pnl_SetAllSameGroup.Controls.Add(this.chk_SetAllSameGroup);
            this.pnl_SetAllSameGroup.Name = "pnl_SetAllSameGroup";
            // 
            // chk_SetAllSameGroup
            // 
            resources.ApplyResources(this.chk_SetAllSameGroup, "chk_SetAllSameGroup");
            this.chk_SetAllSameGroup.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetAllSameGroup.Name = "chk_SetAllSameGroup";
            this.chk_SetAllSameGroup.Selected = false;
            this.chk_SetAllSameGroup.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetAllSameGroup.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetAllSameGroup.UseVisualStyleBackColor = true;
            this.chk_SetAllSameGroup.Click += new System.EventHandler(this.chk_SetAllSameGroup_Click);
            // 
            // pnl_SetAllROI
            // 
            resources.ApplyResources(this.pnl_SetAllROI, "pnl_SetAllROI");
            this.pnl_SetAllROI.Controls.Add(this.chk_SetAllROI);
            this.pnl_SetAllROI.Name = "pnl_SetAllROI";
            // 
            // pnl_SetAllRows
            // 
            resources.ApplyResources(this.pnl_SetAllRows, "pnl_SetAllRows");
            this.pnl_SetAllRows.Controls.Add(this.chk_SetAllRows);
            this.pnl_SetAllRows.Name = "pnl_SetAllRows";
            // 
            // pnl_SetAllEdges
            // 
            resources.ApplyResources(this.pnl_SetAllEdges, "pnl_SetAllEdges");
            this.pnl_SetAllEdges.Controls.Add(this.chk_SetAllEdges);
            this.pnl_SetAllEdges.Name = "pnl_SetAllEdges";
            // 
            // SetValueForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.txt_SetValue);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.lbl_Definition);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetValueForm";
            this.ShowInTaskbar = false;
            this.panel1.ResumeLayout(false);
            this.pnl_SetAllSameGroup.ResumeLayout(false);
            this.pnl_SetAllROI.ResumeLayout(false);
            this.pnl_SetAllRows.ResumeLayout(false);
            this.pnl_SetAllEdges.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMInputBox txt_SetValue;
        private SRMControl.SRMLabel lbl_Definition;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMCheckBox chk_SetAllRows;
        private SRMControl.SRMCheckBox chk_SetAllROI;
        private SRMControl.SRMCheckBox chk_SetAllEdges;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnl_SetAllSameGroup;
        private SRMControl.SRMCheckBox chk_SetAllSameGroup;
        private System.Windows.Forms.Panel pnl_SetAllROI;
        private System.Windows.Forms.Panel pnl_SetAllRows;
        private System.Windows.Forms.Panel pnl_SetAllEdges;
    }
}