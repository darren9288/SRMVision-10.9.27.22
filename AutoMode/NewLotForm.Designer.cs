namespace AutoMode
{
    partial class NewLotForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewLotForm));
            this.btn_OK = new SRMControl.SRMButton();
            this.CharLimitLabel = new SRMControl.SRMLabel();
            this.NewLotMsgLabel = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.txt_NewLot = new SRMControl.SRMTextBox();
            this.lbl_NewLot = new SRMControl.SRMLabel();
            this.txt_OpID = new SRMControl.SRMTextBox();
            this.lbl_OpID = new SRMControl.SRMLabel();
            this.lbl_RecipeID = new SRMControl.SRMLabel();
            this.cbo_RecipeID = new SRMControl.SRMComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_OSK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // CharLimitLabel
            // 
            resources.ApplyResources(this.CharLimitLabel, "CharLimitLabel");
            this.CharLimitLabel.Name = "CharLimitLabel";
            this.CharLimitLabel.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // NewLotMsgLabel
            // 
            resources.ApplyResources(this.NewLotMsgLabel, "NewLotMsgLabel");
            this.NewLotMsgLabel.ForeColor = System.Drawing.Color.Red;
            this.NewLotMsgLabel.Name = "NewLotMsgLabel";
            this.NewLotMsgLabel.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // txt_NewLot
            // 
            resources.ApplyResources(this.txt_NewLot, "txt_NewLot");
            this.txt_NewLot.BackColor = System.Drawing.Color.White;
            this.txt_NewLot.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_NewLot.Name = "txt_NewLot";
            this.txt_NewLot.NormalBackColor = System.Drawing.Color.White;
            // 
            // lbl_NewLot
            // 
            resources.ApplyResources(this.lbl_NewLot, "lbl_NewLot");
            this.lbl_NewLot.Name = "lbl_NewLot";
            this.lbl_NewLot.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_OpID
            // 
            resources.ApplyResources(this.txt_OpID, "txt_OpID");
            this.txt_OpID.BackColor = System.Drawing.Color.White;
            this.txt_OpID.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_OpID.Name = "txt_OpID";
            this.txt_OpID.NormalBackColor = System.Drawing.Color.White;
            // 
            // lbl_OpID
            // 
            resources.ApplyResources(this.lbl_OpID, "lbl_OpID");
            this.lbl_OpID.Name = "lbl_OpID";
            this.lbl_OpID.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_RecipeID
            // 
            resources.ApplyResources(this.lbl_RecipeID, "lbl_RecipeID");
            this.lbl_RecipeID.Name = "lbl_RecipeID";
            this.lbl_RecipeID.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_RecipeID
            // 
            resources.ApplyResources(this.cbo_RecipeID, "cbo_RecipeID");
            this.cbo_RecipeID.BackColor = System.Drawing.Color.White;
            this.cbo_RecipeID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_RecipeID.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_RecipeID.FormattingEnabled = true;
            this.cbo_RecipeID.Name = "cbo_RecipeID";
            this.cbo_RecipeID.NormalBackColor = System.Drawing.Color.White;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btn_OSK
            // 
            resources.ApplyResources(this.btn_OSK, "btn_OSK");
            this.btn_OSK.Name = "btn_OSK";
            this.btn_OSK.UseVisualStyleBackColor = true;
            this.btn_OSK.Click += new System.EventHandler(this.btn_OSK_Click);
            // 
            // NewLotForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_OSK);
            this.Controls.Add(this.cbo_RecipeID);
            this.Controls.Add(this.lbl_RecipeID);
            this.Controls.Add(this.txt_OpID);
            this.Controls.Add(this.lbl_OpID);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.CharLimitLabel);
            this.Controls.Add(this.NewLotMsgLabel);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.txt_NewLot);
            this.Controls.Add(this.lbl_NewLot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NewLotForm";
            this.Load += new System.EventHandler(this.NewLotForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel CharLimitLabel;
        private SRMControl.SRMLabel NewLotMsgLabel;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMTextBox txt_NewLot;
        private SRMControl.SRMLabel lbl_NewLot;
        private SRMControl.SRMTextBox txt_OpID;
        private SRMControl.SRMLabel lbl_OpID;
        private SRMControl.SRMLabel lbl_RecipeID;
        private SRMControl.SRMComboBox cbo_RecipeID;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btn_OSK;
    }
}