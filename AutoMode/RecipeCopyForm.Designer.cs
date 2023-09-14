namespace AutoMode
{
    partial class RecipeCopyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecipeCopyForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.txt_CopyTo = new SRMControl.SRMTextBox();
            this.txt_CopyFrom = new SRMControl.SRMTextBox();
            this.label2 = new SRMControl.SRMLabel();
            this.label1 = new SRMControl.SRMLabel();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // txt_CopyTo
            // 
            resources.ApplyResources(this.txt_CopyTo, "txt_CopyTo");
            this.txt_CopyTo.BackColor = System.Drawing.Color.White;
            this.txt_CopyTo.FocusBackColor = System.Drawing.Color.Empty;
            this.txt_CopyTo.Name = "txt_CopyTo";
            this.txt_CopyTo.NormalBackColor = System.Drawing.Color.White;
            // 
            // txt_CopyFrom
            // 
            resources.ApplyResources(this.txt_CopyFrom, "txt_CopyFrom");
            this.txt_CopyFrom.BackColor = System.Drawing.Color.White;
            this.txt_CopyFrom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_CopyFrom.Name = "txt_CopyFrom";
            this.txt_CopyFrom.NormalBackColor = System.Drawing.Color.White;
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
            // RecipeCopyForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.txt_CopyTo);
            this.Controls.Add(this.txt_CopyFrom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "RecipeCopyForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMTextBox txt_CopyTo;
        private SRMControl.SRMTextBox txt_CopyFrom;
        private SRMControl.SRMLabel label2;
        private SRMControl.SRMLabel label1;
    }
}