namespace User
{
    partial class UserRightForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserRightForm));
            this.btn_Close = new SRMControl.SRMButton();
            this.tre_UserRight = new System.Windows.Forms.TreeView();
            this.UserRightImageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new SRMControl.SRMGroupBox();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.lbl_Engineer = new SRMControl.SRMLabel();
            this.lbl_Admin = new SRMControl.SRMLabel();
            this.lbl_SRM = new SRMControl.SRMLabel();
            this.label2 = new SRMControl.SRMLabel();
            this.label3 = new SRMControl.SRMLabel();
            this.label6 = new SRMControl.SRMLabel();
            this.label4 = new SRMControl.SRMLabel();
            this.label5 = new SRMControl.SRMLabel();
            this.label1 = new SRMControl.SRMLabel();
            this.radio_Technician = new SRMControl.SRMRadioButton();
            this.radio_Operator = new SRMControl.SRMRadioButton();
            this.radio_Engineer = new SRMControl.SRMRadioButton();
            this.radio_Admin = new SRMControl.SRMRadioButton();
            this.radio_SRM = new SRMControl.SRMRadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Close
            // 
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Name = "btn_Close";
            // 
            // tre_UserRight
            // 
            this.tre_UserRight.BackColor = System.Drawing.Color.AliceBlue;
            resources.ApplyResources(this.tre_UserRight, "tre_UserRight");
            this.tre_UserRight.ImageList = this.UserRightImageList;
            this.tre_UserRight.ItemHeight = 24;
            this.tre_UserRight.Name = "tre_UserRight";
            this.tre_UserRight.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.UserRightTreeView_BeforeSelect);
            this.tre_UserRight.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.UserRightTreeView_AfterSelect);
            this.tre_UserRight.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.UserRightTreeView_NodeMouseClick);
            // 
            // UserRightImageList
            // 
            this.UserRightImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("UserRightImageList.ImageStream")));
            this.UserRightImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.UserRightImageList.Images.SetKeyName(0, "");
            this.UserRightImageList.Images.SetKeyName(1, "");
            this.UserRightImageList.Images.SetKeyName(2, "");
            this.UserRightImageList.Images.SetKeyName(3, "");
            this.UserRightImageList.Images.SetKeyName(4, "");
            this.UserRightImageList.Images.SetKeyName(5, "");
            this.UserRightImageList.Images.SetKeyName(6, "");
            this.UserRightImageList.Images.SetKeyName(7, "");
            this.UserRightImageList.Images.SetKeyName(8, "");
            this.UserRightImageList.Images.SetKeyName(9, "");
            this.UserRightImageList.Images.SetKeyName(10, "VisionFeature.png");
            this.UserRightImageList.Images.SetKeyName(11, "manual_24.png");
            this.UserRightImageList.Images.SetKeyName(12, "Gauge.png");
            this.UserRightImageList.Images.SetKeyName(13, "node_24.png");
            this.UserRightImageList.Images.SetKeyName(14, "key_16.png");
            this.UserRightImageList.Images.SetKeyName(15, "GeneralPage.png");
            this.UserRightImageList.Images.SetKeyName(16, "32x32-free-design-icons (1).png");
            // 
            // groupBox1
            // 
            this.groupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.groupBox1.Controls.Add(this.srmLabel5);
            this.groupBox1.Controls.Add(this.srmLabel4);
            this.groupBox1.Controls.Add(this.lbl_Engineer);
            this.groupBox1.Controls.Add(this.lbl_Admin);
            this.groupBox1.Controls.Add(this.lbl_SRM);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radio_Technician);
            this.groupBox1.Controls.Add(this.radio_Operator);
            this.groupBox1.Controls.Add(this.radio_Engineer);
            this.groupBox1.Controls.Add(this.radio_Admin);
            this.groupBox1.Controls.Add(this.radio_SRM);
            resources.ApplyResources(this.groupBox1, "groupBox1");
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
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.label6.TextShadowColor = System.Drawing.Color.Gray;
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
            // UserRightForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.tre_UserRight);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserRightForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.UserRightForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Close;
        private System.Windows.Forms.TreeView tre_UserRight;
        private SRMControl.SRMGroupBox groupBox1;
        private SRMControl.SRMLabel label2;
        private SRMControl.SRMLabel label3;
        private SRMControl.SRMLabel label6;
        private SRMControl.SRMLabel label4;
        private SRMControl.SRMLabel label5;
        private SRMControl.SRMLabel label1;
        private SRMControl.SRMRadioButton radio_Technician;
        private SRMControl.SRMRadioButton radio_Operator;
        private SRMControl.SRMRadioButton radio_Engineer;
        private SRMControl.SRMRadioButton radio_Admin;
        private SRMControl.SRMRadioButton radio_SRM;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel lbl_Engineer;
        private SRMControl.SRMLabel lbl_Admin;
        private SRMControl.SRMLabel lbl_SRM;
        private System.Windows.Forms.ImageList UserRightImageList;
    }
}