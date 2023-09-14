namespace VisionProcessForm
{
    partial class PocketPositionMeanStatisticForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PocketPositionMeanStatisticForm));
            this.btn_StartAnalysis = new SRMControl.SRMButton();
            this.lbl_Max = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.lbl_Min = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_TotalDataCount = new System.Windows.Forms.TextBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.lbl_Center = new SRMControl.SRMLabel();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.lbl_Range = new SRMControl.SRMLabel();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.lbl_Mean = new SRMControl.SRMLabel();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.lbl_PocketPositionRef = new SRMControl.SRMLabel();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.lbl_DataCount = new SRMControl.SRMLabel();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.btn_Set = new SRMControl.SRMButton();
            this.SuspendLayout();
            // 
            // btn_StartAnalysis
            // 
            resources.ApplyResources(this.btn_StartAnalysis, "btn_StartAnalysis");
            this.btn_StartAnalysis.Name = "btn_StartAnalysis";
            this.btn_StartAnalysis.UseVisualStyleBackColor = true;
            this.btn_StartAnalysis.Click += new System.EventHandler(this.btn_StartAnalysis_Click);
            // 
            // lbl_Max
            // 
            resources.ApplyResources(this.lbl_Max, "lbl_Max");
            this.lbl_Max.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Max.Name = "lbl_Max";
            this.lbl_Max.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Min
            // 
            resources.ApplyResources(this.lbl_Min, "lbl_Min");
            this.lbl_Min.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Min.Name = "lbl_Min";
            this.lbl_Min.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_TotalDataCount
            // 
            resources.ApplyResources(this.txt_TotalDataCount, "txt_TotalDataCount");
            this.txt_TotalDataCount.Name = "txt_TotalDataCount";
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            this.Timer.Interval = 1;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // lbl_Center
            // 
            resources.ApplyResources(this.lbl_Center, "lbl_Center");
            this.lbl_Center.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Center.Name = "lbl_Center";
            this.lbl_Center.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Range
            // 
            resources.ApplyResources(this.lbl_Range, "lbl_Range");
            this.lbl_Range.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Range.Name = "lbl_Range";
            this.lbl_Range.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Mean
            // 
            resources.ApplyResources(this.lbl_Mean, "lbl_Mean");
            this.lbl_Mean.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Mean.Name = "lbl_Mean";
            this.lbl_Mean.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PocketPositionRef
            // 
            resources.ApplyResources(this.lbl_PocketPositionRef, "lbl_PocketPositionRef");
            this.lbl_PocketPositionRef.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PocketPositionRef.Name = "lbl_PocketPositionRef";
            this.lbl_PocketPositionRef.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel10
            // 
            resources.ApplyResources(this.srmLabel10, "srmLabel10");
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_DataCount
            // 
            resources.ApplyResources(this.lbl_DataCount, "lbl_DataCount");
            this.lbl_DataCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_DataCount.Name = "lbl_DataCount";
            this.lbl_DataCount.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Set
            // 
            resources.ApplyResources(this.btn_Set, "btn_Set");
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            // 
            // PocketPositionMeanStatisticForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.btn_Set);
            this.Controls.Add(this.lbl_DataCount);
            this.Controls.Add(this.srmLabel7);
            this.Controls.Add(this.lbl_PocketPositionRef);
            this.Controls.Add(this.srmLabel10);
            this.Controls.Add(this.lbl_Mean);
            this.Controls.Add(this.srmLabel6);
            this.Controls.Add(this.lbl_Center);
            this.Controls.Add(this.srmLabel5);
            this.Controls.Add(this.lbl_Range);
            this.Controls.Add(this.srmLabel8);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.txt_TotalDataCount);
            this.Controls.Add(this.lbl_Max);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.lbl_Min);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.btn_StartAnalysis);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PocketPositionMeanStatisticForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PocketPositionMeanStatisticForm_FormClosing);
            this.Load += new System.EventHandler(this.PocketPositionMeanStatisticForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_StartAnalysis;
        private SRMControl.SRMLabel lbl_Max;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel lbl_Min;
        private SRMControl.SRMLabel srmLabel1;
        private System.Windows.Forms.TextBox txt_TotalDataCount;
        private SRMControl.SRMLabel srmLabel2;
        private System.Windows.Forms.Timer Timer;
        private SRMControl.SRMLabel lbl_Center;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMLabel lbl_Range;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMLabel lbl_Mean;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMLabel lbl_PocketPositionRef;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel lbl_DataCount;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMButton btn_Set;
    }
}