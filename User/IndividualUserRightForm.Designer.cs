namespace User
{
    partial class IndividualUserRightForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndividualUserRightForm));
            this.tre_UserRight = new System.Windows.Forms.TreeView();
            this.btn_Close = new SRMControl.SRMButton();
            this.GroupComboBox = new SRMControl.SRMComboBox();
            this.label6 = new SRMControl.SRMLabel();
            this.groupBox1 = new SRMControl.SRMGroupBox();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.lbl_Engineer = new SRMControl.SRMLabel();
            this.lbl_Admin = new SRMControl.SRMLabel();
            this.lbl_SRM = new SRMControl.SRMLabel();
            this.label2 = new SRMControl.SRMLabel();
            this.label3 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.label4 = new SRMControl.SRMLabel();
            this.label5 = new SRMControl.SRMLabel();
            this.label1 = new SRMControl.SRMLabel();
            this.radio_Technician = new SRMControl.SRMRadioButton();
            this.radio_Operator = new SRMControl.SRMRadioButton();
            this.radio_Engineer = new SRMControl.SRMRadioButton();
            this.radio_Admin = new SRMControl.SRMRadioButton();
            this.radio_SRM = new SRMControl.SRMRadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tre_UserRight
            // 
            resources.ApplyResources(this.tre_UserRight, "tre_UserRight");
            this.tre_UserRight.BackColor = System.Drawing.Color.AliceBlue;
            this.tre_UserRight.CheckBoxes = true;
            this.tre_UserRight.ItemHeight = 24;
            this.tre_UserRight.Name = "tre_UserRight";
            this.tre_UserRight.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.tre_UserRight_BeforeCheck);
            this.tre_UserRight.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tre_UserRight_AfterCheck);
            this.tre_UserRight.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.UserRightTreeView_BeforeSelect);
            this.tre_UserRight.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.UserRightTreeView_AfterSelect);
            this.tre_UserRight.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.UserRightTreeView_NodeMouseClick);
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Close.Name = "btn_Close";
            // 
            // GroupComboBox
            // 
            resources.ApplyResources(this.GroupComboBox, "GroupComboBox");
            this.GroupComboBox.BackColor = System.Drawing.Color.White;
            this.GroupComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupComboBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.GroupComboBox.Name = "GroupComboBox";
            this.GroupComboBox.NormalBackColor = System.Drawing.Color.White;
            this.GroupComboBox.SelectedIndexChanged += new System.EventHandler(this.GroupComboBox_SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.label6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.groupBox1.Controls.Add(this.srmLabel5);
            this.groupBox1.Controls.Add(this.srmLabel4);
            this.groupBox1.Controls.Add(this.lbl_Engineer);
            this.groupBox1.Controls.Add(this.lbl_Admin);
            this.groupBox1.Controls.Add(this.lbl_SRM);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.srmLabel1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radio_Technician);
            this.groupBox1.Controls.Add(this.radio_Operator);
            this.groupBox1.Controls.Add(this.radio_Engineer);
            this.groupBox1.Controls.Add(this.radio_Admin);
            this.groupBox1.Controls.Add(this.radio_SRM);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Engineer
            // 
            resources.ApplyResources(this.lbl_Engineer, "lbl_Engineer");
            this.lbl_Engineer.Name = "lbl_Engineer";
            this.lbl_Engineer.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Admin
            // 
            resources.ApplyResources(this.lbl_Admin, "lbl_Admin");
            this.lbl_Admin.Name = "lbl_Admin";
            this.lbl_Admin.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_SRM
            // 
            resources.ApplyResources(this.lbl_SRM, "lbl_SRM");
            this.lbl_SRM.Name = "lbl_SRM";
            this.lbl_SRM.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.label4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.label5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // radio_Technician
            // 
            resources.ApplyResources(this.radio_Technician, "radio_Technician");
            this.radio_Technician.Name = "radio_Technician";
            this.radio_Technician.Click += new System.EventHandler(this.TechRadioButton_Click);
            // 
            // radio_Operator
            // 
            resources.ApplyResources(this.radio_Operator, "radio_Operator");
            this.radio_Operator.Name = "radio_Operator";
            this.radio_Operator.Click += new System.EventHandler(this.OpRadioButton_Click);
            // 
            // radio_Engineer
            // 
            resources.ApplyResources(this.radio_Engineer, "radio_Engineer");
            this.radio_Engineer.Name = "radio_Engineer";
            this.radio_Engineer.Click += new System.EventHandler(this.EngRadioButton_Click);
            // 
            // radio_Admin
            // 
            resources.ApplyResources(this.radio_Admin, "radio_Admin");
            this.radio_Admin.Name = "radio_Admin";
            this.radio_Admin.Click += new System.EventHandler(this.AdminRadioButton_Click);
            // 
            // radio_SRM
            // 
            resources.ApplyResources(this.radio_SRM, "radio_SRM");
            this.radio_SRM.Name = "radio_SRM";
            this.radio_SRM.Click += new System.EventHandler(this.SRMRadioButton_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // IndividualUserRightForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GroupComboBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.tre_UserRight);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IndividualUserRightForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.IndividualUserRightForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tre_UserRight;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMComboBox GroupComboBox;
        private SRMControl.SRMLabel label6;
        private SRMControl.SRMGroupBox groupBox1;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel lbl_Engineer;
        private SRMControl.SRMLabel lbl_Admin;
        private SRMControl.SRMLabel lbl_SRM;
        private SRMControl.SRMLabel label2;
        private SRMControl.SRMLabel label3;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel label4;
        private SRMControl.SRMLabel label5;
        private SRMControl.SRMLabel label1;
        private SRMControl.SRMRadioButton radio_Technician;
        private SRMControl.SRMRadioButton radio_Operator;
        private SRMControl.SRMRadioButton radio_Engineer;
        private SRMControl.SRMRadioButton radio_Admin;
        private SRMControl.SRMRadioButton radio_SRM;
        private System.Windows.Forms.Timer timer1;
    }
}