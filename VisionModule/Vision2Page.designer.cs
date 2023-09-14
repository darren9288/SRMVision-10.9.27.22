namespace VisionModule
{
    partial class Vision2Page
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vision2Page));
            this.lbl_ResultStatus = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lbl_CheckPresentFailCount = new System.Windows.Forms.Label();
            this.lbl_LotID = new System.Windows.Forms.Label();
            this.lbl_CheckPresent = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lbl_OperatorID = new System.Windows.Forms.Label();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.timer_Live = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_PassCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_TestedTotal = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbl_Yield = new System.Windows.Forms.Label();
            this.pic_Learn1 = new System.Windows.Forms.PictureBox();
            this.lbl_Pin1ROI = new SRMControl.SRMLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_TotalTime = new System.Windows.Forms.Label();
            this.lbl_GrabDelay = new System.Windows.Forms.Label();
            this.lbl_GrabTime = new System.Windows.Forms.Label();
            this.lbl_V2Grab = new System.Windows.Forms.Label();
            this.lbl_V2Process = new System.Windows.Forms.Label();
            this.lbl_ProcessTime = new System.Windows.Forms.Label();
            this.lbl_FailCount = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.pnl_Detail = new System.Windows.Forms.Panel();
            this.pnl_NoTemplate = new System.Windows.Forms.Panel();
            this.lbl_NoTemplateFailPercent = new System.Windows.Forms.Label();
            this.lbl_NoTemplate = new System.Windows.Forms.Label();
            this.lbl_NoTemplateFailCount = new System.Windows.Forms.Label();
            this.pnl_CheckPresent = new System.Windows.Forms.Panel();
            this.lbl_CheckPresentFailPercent = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lbl_Detail = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Learn1)).BeginInit();
            this.pnl_Detail.SuspendLayout();
            this.pnl_NoTemplate.SuspendLayout();
            this.pnl_CheckPresent.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_ResultStatus
            // 
            resources.ApplyResources(this.lbl_ResultStatus, "lbl_ResultStatus");
            this.lbl_ResultStatus.BackColor = System.Drawing.Color.Gray;
            this.lbl_ResultStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ResultStatus.Name = "lbl_ResultStatus";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Name = "label10";
            // 
            // lbl_CheckPresentFailCount
            // 
            resources.ApplyResources(this.lbl_CheckPresentFailCount, "lbl_CheckPresentFailCount");
            this.lbl_CheckPresentFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CheckPresentFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CheckPresentFailCount.Name = "lbl_CheckPresentFailCount";
            // 
            // lbl_LotID
            // 
            resources.ApplyResources(this.lbl_LotID, "lbl_LotID");
            this.lbl_LotID.BackColor = System.Drawing.Color.White;
            this.lbl_LotID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_LotID.Name = "lbl_LotID";
            // 
            // lbl_CheckPresent
            // 
            resources.ApplyResources(this.lbl_CheckPresent, "lbl_CheckPresent");
            this.lbl_CheckPresent.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_CheckPresent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CheckPresent.Name = "lbl_CheckPresent";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Name = "label15";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label16.Name = "label16";
            // 
            // lbl_OperatorID
            // 
            resources.ApplyResources(this.lbl_OperatorID, "lbl_OperatorID");
            this.lbl_OperatorID.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbl_OperatorID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_OperatorID.Name = "lbl_OperatorID";
            // 
            // btn_Reset
            // 
            resources.ApplyResources(this.btn_Reset, "btn_Reset");
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // timer_Live
            // 
            this.timer_Live.Interval = 20;
            this.timer_Live.Tick += new System.EventHandler(this.timer_Live_Tick);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Name = "label1";
            // 
            // lbl_PassCount
            // 
            resources.ApplyResources(this.lbl_PassCount, "lbl_PassCount");
            this.lbl_PassCount.BackColor = System.Drawing.Color.White;
            this.lbl_PassCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PassCount.Name = "lbl_PassCount";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Name = "label4";
            // 
            // lbl_TestedTotal
            // 
            resources.ApplyResources(this.lbl_TestedTotal, "lbl_TestedTotal");
            this.lbl_TestedTotal.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbl_TestedTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_TestedTotal.Name = "lbl_TestedTotal";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Name = "label7";
            // 
            // lbl_Yield
            // 
            resources.ApplyResources(this.lbl_Yield, "lbl_Yield");
            this.lbl_Yield.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbl_Yield.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Yield.Name = "lbl_Yield";
            // 
            // pic_Learn1
            // 
            resources.ApplyResources(this.pic_Learn1, "pic_Learn1");
            this.pic_Learn1.BackColor = System.Drawing.Color.Black;
            this.pic_Learn1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pic_Learn1.Name = "pic_Learn1";
            this.pic_Learn1.TabStop = false;
            // 
            // lbl_Pin1ROI
            // 
            resources.ApplyResources(this.lbl_Pin1ROI, "lbl_Pin1ROI");
            this.lbl_Pin1ROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lbl_Pin1ROI.ForeColor = System.Drawing.Color.White;
            this.lbl_Pin1ROI.Name = "lbl_Pin1ROI";
            this.lbl_Pin1ROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Name = "label3";
            // 
            // lbl_TotalTime
            // 
            resources.ApplyResources(this.lbl_TotalTime, "lbl_TotalTime");
            this.lbl_TotalTime.BackColor = System.Drawing.Color.White;
            this.lbl_TotalTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_TotalTime.Name = "lbl_TotalTime";
            // 
            // lbl_GrabDelay
            // 
            resources.ApplyResources(this.lbl_GrabDelay, "lbl_GrabDelay");
            this.lbl_GrabDelay.BackColor = System.Drawing.Color.White;
            this.lbl_GrabDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrabDelay.Name = "lbl_GrabDelay";
            // 
            // lbl_GrabTime
            // 
            resources.ApplyResources(this.lbl_GrabTime, "lbl_GrabTime");
            this.lbl_GrabTime.BackColor = System.Drawing.Color.White;
            this.lbl_GrabTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_GrabTime.Name = "lbl_GrabTime";
            // 
            // lbl_V2Grab
            // 
            resources.ApplyResources(this.lbl_V2Grab, "lbl_V2Grab");
            this.lbl_V2Grab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_V2Grab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_V2Grab.Name = "lbl_V2Grab";
            // 
            // lbl_V2Process
            // 
            resources.ApplyResources(this.lbl_V2Process, "lbl_V2Process");
            this.lbl_V2Process.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_V2Process.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_V2Process.Name = "lbl_V2Process";
            // 
            // lbl_ProcessTime
            // 
            resources.ApplyResources(this.lbl_ProcessTime, "lbl_ProcessTime");
            this.lbl_ProcessTime.BackColor = System.Drawing.Color.White;
            this.lbl_ProcessTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ProcessTime.Name = "lbl_ProcessTime";
            // 
            // lbl_FailCount
            // 
            resources.ApplyResources(this.lbl_FailCount, "lbl_FailCount");
            this.lbl_FailCount.BackColor = System.Drawing.Color.White;
            this.lbl_FailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_FailCount.Name = "lbl_FailCount";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Name = "label8";
            // 
            // pnl_Detail
            // 
            resources.ApplyResources(this.pnl_Detail, "pnl_Detail");
            this.pnl_Detail.Controls.Add(this.pnl_NoTemplate);
            this.pnl_Detail.Controls.Add(this.pnl_CheckPresent);
            this.pnl_Detail.Name = "pnl_Detail";
            // 
            // pnl_NoTemplate
            // 
            resources.ApplyResources(this.pnl_NoTemplate, "pnl_NoTemplate");
            this.pnl_NoTemplate.Controls.Add(this.lbl_NoTemplateFailPercent);
            this.pnl_NoTemplate.Controls.Add(this.lbl_NoTemplate);
            this.pnl_NoTemplate.Controls.Add(this.lbl_NoTemplateFailCount);
            this.pnl_NoTemplate.Name = "pnl_NoTemplate";
            // 
            // lbl_NoTemplateFailPercent
            // 
            resources.ApplyResources(this.lbl_NoTemplateFailPercent, "lbl_NoTemplateFailPercent");
            this.lbl_NoTemplateFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_NoTemplateFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_NoTemplateFailPercent.Name = "lbl_NoTemplateFailPercent";
            // 
            // lbl_NoTemplate
            // 
            resources.ApplyResources(this.lbl_NoTemplate, "lbl_NoTemplate");
            this.lbl_NoTemplate.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_NoTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_NoTemplate.Name = "lbl_NoTemplate";
            // 
            // lbl_NoTemplateFailCount
            // 
            resources.ApplyResources(this.lbl_NoTemplateFailCount, "lbl_NoTemplateFailCount");
            this.lbl_NoTemplateFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_NoTemplateFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_NoTemplateFailCount.Name = "lbl_NoTemplateFailCount";
            // 
            // pnl_CheckPresent
            // 
            resources.ApplyResources(this.pnl_CheckPresent, "pnl_CheckPresent");
            this.pnl_CheckPresent.Controls.Add(this.lbl_CheckPresentFailPercent);
            this.pnl_CheckPresent.Controls.Add(this.lbl_CheckPresent);
            this.pnl_CheckPresent.Controls.Add(this.lbl_CheckPresentFailCount);
            this.pnl_CheckPresent.Name = "pnl_CheckPresent";
            // 
            // lbl_CheckPresentFailPercent
            // 
            resources.ApplyResources(this.lbl_CheckPresentFailPercent, "lbl_CheckPresentFailPercent");
            this.lbl_CheckPresentFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CheckPresentFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CheckPresentFailPercent.Name = "lbl_CheckPresentFailPercent";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Name = "label2";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Name = "label9";
            // 
            // lbl_Detail
            // 
            resources.ApplyResources(this.lbl_Detail, "lbl_Detail");
            this.lbl_Detail.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_Detail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Detail.Name = "lbl_Detail";
            // 
            // Vision2Page
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.pnl_Detail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lbl_Detail);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbl_FailCount);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbl_TotalTime);
            this.Controls.Add(this.lbl_GrabDelay);
            this.Controls.Add(this.lbl_GrabTime);
            this.Controls.Add(this.lbl_V2Grab);
            this.Controls.Add(this.lbl_V2Process);
            this.Controls.Add(this.lbl_ProcessTime);
            this.Controls.Add(this.lbl_Pin1ROI);
            this.Controls.Add(this.pic_Learn1);
            this.Controls.Add(this.lbl_Yield);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_PassCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbl_TestedTotal);
            this.Controls.Add(this.lbl_ResultStatus);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lbl_LotID);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lbl_OperatorID);
            this.Controls.Add(this.btn_Reset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Vision2Page";
            ((System.ComponentModel.ISupportInitialize)(this.pic_Learn1)).EndInit();
            this.pnl_Detail.ResumeLayout(false);
            this.pnl_NoTemplate.ResumeLayout(false);
            this.pnl_CheckPresent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl_ResultStatus;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbl_CheckPresentFailCount;
        private System.Windows.Forms.Label lbl_LotID;
        private System.Windows.Forms.Label lbl_CheckPresent;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lbl_OperatorID;
        private System.Windows.Forms.Button btn_Reset;
        private System.Windows.Forms.Timer timer_Live;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_PassCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbl_TestedTotal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbl_Yield;
        private System.Windows.Forms.PictureBox pic_Learn1;
        private SRMControl.SRMLabel lbl_Pin1ROI;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_TotalTime;
        private System.Windows.Forms.Label lbl_GrabDelay;
        private System.Windows.Forms.Label lbl_GrabTime;
        private System.Windows.Forms.Label lbl_V2Grab;
        private System.Windows.Forms.Label lbl_V2Process;
        private System.Windows.Forms.Label lbl_ProcessTime;
        private System.Windows.Forms.Label lbl_FailCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel pnl_Detail;
        private System.Windows.Forms.Panel pnl_NoTemplate;
        private System.Windows.Forms.Label lbl_NoTemplateFailPercent;
        private System.Windows.Forms.Label lbl_NoTemplate;
        private System.Windows.Forms.Label lbl_NoTemplateFailCount;
        private System.Windows.Forms.Panel pnl_CheckPresent;
        private System.Windows.Forms.Label lbl_CheckPresentFailPercent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbl_Detail;
    }
}