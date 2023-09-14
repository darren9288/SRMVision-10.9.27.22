namespace User
{
    partial class UserGroupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserGroupForm));
            this.UserlistView = new System.Windows.Forms.ListView();
            this.columnUsername = new System.Windows.Forms.ColumnHeader();
            this.columnFullname = new System.Windows.Forms.ColumnHeader();
            this.columnDescription = new System.Windows.Forms.ColumnHeader();
            this.CloseButton = new SRMControl.SRMButton();
            this.label3 = new SRMControl.SRMLabel();
            this.DescriptionEditBox = new SRMControl.SRMInputBox();
            this.label2 = new SRMControl.SRMLabel();
            this.GroupEditBox = new SRMControl.SRMInputBox();
            this.label1 = new SRMControl.SRMLabel();
            this.SuspendLayout();
            // 
            // UserlistView
            // 
            this.UserlistView.BackColor = System.Drawing.Color.White;
            this.UserlistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnUsername,
            this.columnFullname,
            this.columnDescription});
            resources.ApplyResources(this.UserlistView, "UserlistView");
            this.UserlistView.Name = "UserlistView";
            this.UserlistView.UseCompatibleStateImageBehavior = false;
            this.UserlistView.View = System.Windows.Forms.View.Details;
            // 
            // columnUsername
            // 
            resources.ApplyResources(this.columnUsername, "columnUsername");
            // 
            // columnFullname
            // 
            resources.ApplyResources(this.columnFullname, "columnFullname");
            // 
            // columnDescription
            // 
            resources.ApplyResources(this.columnDescription, "columnDescription");
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.Name = "CloseButton";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // DescriptionEditBox
            // 
            this.DescriptionEditBox.BackColor = System.Drawing.Color.White;
            this.DescriptionEditBox.DecimalPlaces = 2;
            this.DescriptionEditBox.DecMaxValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.DescriptionEditBox.DecMinValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            -2147483648});
            this.DescriptionEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.DescriptionEditBox, "DescriptionEditBox");
            this.DescriptionEditBox.Name = "DescriptionEditBox";
            this.DescriptionEditBox.NormalBackColor = System.Drawing.Color.White;
            this.DescriptionEditBox.ReadOnly = true;
            this.DescriptionEditBox.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // GroupEditBox
            // 
            this.GroupEditBox.BackColor = System.Drawing.Color.White;
            this.GroupEditBox.DecimalPlaces = 2;
            this.GroupEditBox.DecMaxValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.GroupEditBox.DecMinValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            -2147483648});
            this.GroupEditBox.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.GroupEditBox, "GroupEditBox");
            this.GroupEditBox.Name = "GroupEditBox";
            this.GroupEditBox.NormalBackColor = System.Drawing.Color.White;
            this.GroupEditBox.ReadOnly = true;
            this.GroupEditBox.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // UserGroupForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.UserlistView);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DescriptionEditBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.GroupEditBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserGroupForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.GroupFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMInputBox GroupEditBox;
        private SRMControl.SRMLabel label1;
        private SRMControl.SRMButton CloseButton;
        private System.Windows.Forms.ListView UserlistView;
        private System.Windows.Forms.ColumnHeader columnUsername;
        private System.Windows.Forms.ColumnHeader columnFullname;
        private System.Windows.Forms.ColumnHeader columnDescription;
        private SRMControl.SRMLabel label3;
        private SRMControl.SRMInputBox DescriptionEditBox;
        private SRMControl.SRMLabel label2;
    }
}