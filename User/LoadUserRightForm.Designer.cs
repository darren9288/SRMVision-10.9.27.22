namespace User
{
    partial class LoadUserRightForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadUserRightForm));
            this.btn_Close = new SRMControl.SRMButton();
            this.btn_Load = new SRMControl.SRMButton();
            this.btn_Browse = new SRMControl.SRMButton();
            this.txt_SelectedPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // btn_Close
            // 
            this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Name = "btn_Close";
            // 
            // btn_Load
            // 
            resources.ApplyResources(this.btn_Load, "btn_Load");
            this.btn_Load.Name = "btn_Load";
            this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
            // 
            // btn_Browse
            // 
            resources.ApplyResources(this.btn_Browse, "btn_Browse");
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // txt_SelectedPath
            // 
            this.txt_SelectedPath.AllowDrop = true;
            resources.ApplyResources(this.txt_SelectedPath, "txt_SelectedPath");
            this.txt_SelectedPath.Name = "txt_SelectedPath";
            this.txt_SelectedPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txt_SelectedPath_DragDrop);
            this.txt_SelectedPath.DragOver += new System.Windows.Forms.DragEventHandler(this.txt_SelectedPath_DragOver);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.AddExtension = false;
            this.openFileDialog1.DefaultExt = "mdb";
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // LoadUserRightForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Browse);
            this.Controls.Add(this.txt_SelectedPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Load);
            this.Controls.Add(this.btn_Close);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LoadUserRightForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMButton btn_Load;
        private SRMControl.SRMButton btn_Browse;
        private System.Windows.Forms.TextBox txt_SelectedPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}