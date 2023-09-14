namespace VisionProcessForm
{
    partial class LearnCharacterForm
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
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_SetValue = new SRMControl.SRMInputBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.cbo_SortingMode = new SRMControl.SRMComboBox();
            this.SuspendLayout();
            // 
            // srmLabel1
            // 
            this.srmLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel1.Location = new System.Drawing.Point(12, 18);
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.Size = new System.Drawing.Size(85, 25);
            this.srmLabel1.TabIndex = 70;
            this.srmLabel1.Text = "Character";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SetValue
            // 
            this.txt_SetValue.BackColor = System.Drawing.Color.White;
            this.txt_SetValue.DecimalPlaces = 3;
            this.txt_SetValue.DecMaxValue = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.txt_SetValue.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SetValue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SetValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_SetValue.ForeColor = System.Drawing.Color.Black;
            this.txt_SetValue.InputType = SRMControl.InputType.Number;
            this.txt_SetValue.Location = new System.Drawing.Point(99, 17);
            this.txt_SetValue.Name = "txt_SetValue";
            this.txt_SetValue.NormalBackColor = System.Drawing.Color.White;
            this.txt_SetValue.Size = new System.Drawing.Size(106, 26);
            this.txt_SetValue.TabIndex = 69;
            this.txt_SetValue.Text = "0";
            // 
            // srmLabel2
            // 
            this.srmLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel2.Location = new System.Drawing.Point(12, 56);
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.Size = new System.Drawing.Size(51, 25);
            this.srmLabel2.TabIndex = 72;
            this.srmLabel2.Text = "Class";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel3
            // 
            this.srmLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel3.Location = new System.Drawing.Point(12, 99);
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.Size = new System.Drawing.Size(98, 25);
            this.srmLabel3.TabIndex = 73;
            this.srmLabel3.Text = "Char.Width";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            this.srmLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel4.Location = new System.Drawing.Point(12, 137);
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.Size = new System.Drawing.Size(98, 25);
            this.srmLabel4.TabIndex = 74;
            this.srmLabel4.Text = "Char.Height";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel5
            // 
            this.srmLabel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.srmLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel5.Location = new System.Drawing.Point(116, 98);
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.Size = new System.Drawing.Size(99, 26);
            this.srmLabel5.TabIndex = 75;
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel6
            // 
            this.srmLabel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.srmLabel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srmLabel6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel6.Location = new System.Drawing.Point(116, 136);
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.Size = new System.Drawing.Size(99, 26);
            this.srmLabel6.TabIndex = 76;
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(125, 180);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 78;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(39, 180);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 34);
            this.btn_OK.TabIndex = 77;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // cbo_SortingMode
            // 
            this.cbo_SortingMode.BackColor = System.Drawing.Color.White;
            this.cbo_SortingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_SortingMode.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SortingMode.FormattingEnabled = true;
            this.cbo_SortingMode.ItemHeight = 20;
            this.cbo_SortingMode.Items.AddRange(new object[] {
            "Digit",
            "Upper Case",
            "Lower Case",
            "Special",
            "Extended"});
            this.cbo_SortingMode.Location = new System.Drawing.Point(99, 55);
            this.cbo_SortingMode.Name = "cbo_SortingMode";
            this.cbo_SortingMode.NormalBackColor = System.Drawing.Color.White;
            this.cbo_SortingMode.Size = new System.Drawing.Size(133, 26);
            this.cbo_SortingMode.TabIndex = 130;
            // 
            // LearnCharacterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(244, 226);
            this.ControlBox = false;
            this.Controls.Add(this.cbo_SortingMode);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.srmLabel6);
            this.Controls.Add(this.srmLabel5);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.txt_SetValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LearnCharacterForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Learn Character";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_SetValue;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMComboBox cbo_SortingMode;
    }
}