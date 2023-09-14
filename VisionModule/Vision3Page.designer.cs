namespace VisionModule
{
    partial class Vision3Page
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vision3Page));
            this.lbl_ResultStatus = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lbl_LotID = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lbl_OperatorID = new System.Windows.Forms.Label();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.timer_Live = new System.Windows.Forms.Timer(this.components);
            this.chk_ForceYtozero = new SRMControl.SRMCheckBox();
            this.lbl_Pin1ROI = new SRMControl.SRMLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_TotalTime = new System.Windows.Forms.Label();
            this.lbl_GrabDelay = new System.Windows.Forms.Label();
            this.lbl_GrabTime = new System.Windows.Forms.Label();
            this.lbl_V2Grab = new System.Windows.Forms.Label();
            this.lbl_V2Process = new System.Windows.Forms.Label();
            this.lbl_ProcessTime = new System.Windows.Forms.Label();
            this.lbl_Detail = new System.Windows.Forms.Label();
            this.pnl_Detail = new System.Windows.Forms.Panel();
            this.pnl_Pin1 = new System.Windows.Forms.Panel();
            this.lbl_Pin1FailPercent = new System.Windows.Forms.Label();
            this.lbl_Pin1 = new System.Windows.Forms.Label();
            this.lbl_Pin1FailCount = new System.Windows.Forms.Label();
            this.pnl_EmptyUnit = new System.Windows.Forms.Panel();
            this.lbl_Empty = new System.Windows.Forms.Label();
            this.lbl_EmptyUnitFailCount = new System.Windows.Forms.Label();
            this.lbl_EmptyUnitFailPercent = new System.Windows.Forms.Label();
            this.pnl_EdgeNotFound = new System.Windows.Forms.Panel();
            this.lbl_EdgeNotFoundFailPercent = new System.Windows.Forms.Label();
            this.lbl_EdgeNotFound = new System.Windows.Forms.Label();
            this.lbl_EdgeNotFoundFailCount = new System.Windows.Forms.Label();
            this.pnl_NoTemplate = new System.Windows.Forms.Panel();
            this.lbl_NoTemplateFailPercent = new System.Windows.Forms.Label();
            this.lbl_NoTemplate = new System.Windows.Forms.Label();
            this.lbl_NoTemplateFailCount = new System.Windows.Forms.Label();
            this.pnl_Position = new System.Windows.Forms.Panel();
            this.lbl_PositionFailPercent = new System.Windows.Forms.Label();
            this.lbl_Position = new System.Windows.Forms.Label();
            this.lbl_PositionFailCount = new System.Windows.Forms.Label();
            this.pnl_Pad = new System.Windows.Forms.Panel();
            this.lbl_PadFailPercent = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbl_PadFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePkgDimension = new System.Windows.Forms.Panel();
            this.lbl_SidePkgDimensionFailPercent = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lbl_SidePkgDimensionFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePkgDefect = new System.Windows.Forms.Panel();
            this.lbl_SidePkgDefectFailPercent = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.lbl_SidePkgDefectFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPkgDimension = new System.Windows.Forms.Panel();
            this.lbl_CenterPkgDimensionFailPercent = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.lbl_CenterPkgDimensionFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPkgDefect = new System.Windows.Forms.Panel();
            this.lbl_CenterPkgDefectFailPercent = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.lbl_CenterPkgDefectFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadColorDefect = new System.Windows.Forms.Panel();
            this.lbl_SidePadColorDefectFailPercent = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.lbl_SidePadColorDefectFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadMissing = new System.Windows.Forms.Panel();
            this.lbl_SidePadMissingFailPercent = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.lbl_SidePadMissingFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadContamination = new System.Windows.Forms.Panel();
            this.lbl_SidePadContaminationFailPercent = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbl_SidePadContaminationFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadSpan = new System.Windows.Forms.Panel();
            this.lbl_SidePadSpanFailPercent = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.lbl_SidePadSpanFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadEdgeDistance = new System.Windows.Forms.Panel();
            this.lbl_SidePadEdgeDistanceFailPercent = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.lbl_SidePadEdgeDistanceFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadStandOff = new System.Windows.Forms.Panel();
            this.lbl_SidePadStandOffFailPercent = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.lbl_SidePadStandOffFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadEdgeLimit = new System.Windows.Forms.Panel();
            this.lbl_SidePadEdgeLimitFailPercent = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.lbl_SidePadEdgeLimitFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadSmear = new System.Windows.Forms.Panel();
            this.lbl_SidePadSmearFailPercent = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lbl_SidePadSmearFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadExcess = new System.Windows.Forms.Panel();
            this.lbl_SidePadExcessFailPercent = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.lbl_SidePadExcessFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadBroken = new System.Windows.Forms.Panel();
            this.lbl_SidePadBrokenFailPercent = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.lbl_SidePadBrokenFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadPitchGap = new System.Windows.Forms.Panel();
            this.lbl_SidePadPitchGapFailPercent = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.lbl_SidePadPitchGapFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadDimension = new System.Windows.Forms.Panel();
            this.lbl_SidePadDimensionFailPercent = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.lbl_SidePadDimensionFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadArea = new System.Windows.Forms.Panel();
            this.lbl_SidePadAreaFailPercent = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.lbl_SidePadAreaFailCount = new System.Windows.Forms.Label();
            this.pnl_SidePadOffset = new System.Windows.Forms.Panel();
            this.lbl_SidePadOffsetFailPercent = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.lbl_SidePadOffsetFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadColorDefect = new System.Windows.Forms.Panel();
            this.lbl_CenterPadColorDefectFailPercent = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.lbl_CenterPadColorDefectFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadMissing = new System.Windows.Forms.Panel();
            this.lbl_CenterPadMissingFailPercent = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.lbl_CenterPadMissingFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadContamination = new System.Windows.Forms.Panel();
            this.lbl_CenterPadContaminationFailPercent = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.lbl_CenterPadContaminationFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadSpan = new System.Windows.Forms.Panel();
            this.lbl_CenterPadSpanFailPercent = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.lbl_CenterPadSpanFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadEdgeDistance = new System.Windows.Forms.Panel();
            this.lbl_CenterPadEdgeDistanceFailPercent = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.lbl_CenterPadEdgeDistanceFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadStandOff = new System.Windows.Forms.Panel();
            this.lbl_CenterPadStandOffFailPercent = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.lbl_CenterPadStandOffFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadEdgeLimit = new System.Windows.Forms.Panel();
            this.lbl_CenterPadEdgeLimitFailPercent = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.lbl_CenterPadEdgeLimitFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadSmear = new System.Windows.Forms.Panel();
            this.lbl_CenterPadSmearFailPercent = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.lbl_CenterPadSmearFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadExcess = new System.Windows.Forms.Panel();
            this.lbl_CenterPadExcessFailPercent = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.lbl_CenterPadExcessFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadBroken = new System.Windows.Forms.Panel();
            this.lbl_CenterPadBrokenFailPercent = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lbl_CenterPadBrokenFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadPitchGap = new System.Windows.Forms.Panel();
            this.lbl_CenterPadPitchGapFailPercent = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lbl_CenterPadPitchGapFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadDimension = new System.Windows.Forms.Panel();
            this.lbl_CenterPadDimensionFailPercent = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lbl_CenterPadDimensionFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadArea = new System.Windows.Forms.Panel();
            this.lbl_CenterPadAreaFailPercent = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbl_CenterPadAreaFailCount = new System.Windows.Forms.Label();
            this.pnl_CenterPadOffset = new System.Windows.Forms.Panel();
            this.lbl_CenterPadOffsetFailPercent = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lbl_CenterPadOffsetFailCount = new System.Windows.Forms.Label();
            this.pnl_Orient = new System.Windows.Forms.Panel();
            this.lbl_OrientFailPercent = new System.Windows.Forms.Label();
            this.lbl_Orient = new System.Windows.Forms.Label();
            this.lbl_OrientFailCount = new System.Windows.Forms.Label();
            this.lbl_Yield = new System.Windows.Forms.Label();
            this.lbl_FailCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_TestedTotal = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_PassCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.pic_Template = new System.Windows.Forms.PictureBox();
            this.lblOrientationResult = new System.Windows.Forms.Label();
            this.pnl_Detail.SuspendLayout();
            this.pnl_Pin1.SuspendLayout();
            this.pnl_EmptyUnit.SuspendLayout();
            this.pnl_EdgeNotFound.SuspendLayout();
            this.pnl_NoTemplate.SuspendLayout();
            this.pnl_Position.SuspendLayout();
            this.pnl_Pad.SuspendLayout();
            this.pnl_SidePkgDimension.SuspendLayout();
            this.pnl_SidePkgDefect.SuspendLayout();
            this.pnl_CenterPkgDimension.SuspendLayout();
            this.pnl_CenterPkgDefect.SuspendLayout();
            this.pnl_SidePadColorDefect.SuspendLayout();
            this.pnl_SidePadMissing.SuspendLayout();
            this.pnl_SidePadContamination.SuspendLayout();
            this.pnl_SidePadSpan.SuspendLayout();
            this.pnl_SidePadEdgeDistance.SuspendLayout();
            this.pnl_SidePadStandOff.SuspendLayout();
            this.pnl_SidePadEdgeLimit.SuspendLayout();
            this.pnl_SidePadSmear.SuspendLayout();
            this.pnl_SidePadExcess.SuspendLayout();
            this.pnl_SidePadBroken.SuspendLayout();
            this.pnl_SidePadPitchGap.SuspendLayout();
            this.pnl_SidePadDimension.SuspendLayout();
            this.pnl_SidePadArea.SuspendLayout();
            this.pnl_SidePadOffset.SuspendLayout();
            this.pnl_CenterPadColorDefect.SuspendLayout();
            this.pnl_CenterPadMissing.SuspendLayout();
            this.pnl_CenterPadContamination.SuspendLayout();
            this.pnl_CenterPadSpan.SuspendLayout();
            this.pnl_CenterPadEdgeDistance.SuspendLayout();
            this.pnl_CenterPadStandOff.SuspendLayout();
            this.pnl_CenterPadEdgeLimit.SuspendLayout();
            this.pnl_CenterPadSmear.SuspendLayout();
            this.pnl_CenterPadExcess.SuspendLayout();
            this.pnl_CenterPadBroken.SuspendLayout();
            this.pnl_CenterPadPitchGap.SuspendLayout();
            this.pnl_CenterPadDimension.SuspendLayout();
            this.pnl_CenterPadArea.SuspendLayout();
            this.pnl_CenterPadOffset.SuspendLayout();
            this.pnl_Orient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Template)).BeginInit();
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
            // lbl_LotID
            // 
            resources.ApplyResources(this.lbl_LotID, "lbl_LotID");
            this.lbl_LotID.BackColor = System.Drawing.Color.White;
            this.lbl_LotID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_LotID.Name = "lbl_LotID";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Name = "label15";
            this.label15.Click += new System.EventHandler(this.label15_Click);
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
            // chk_ForceYtozero
            // 
            resources.ApplyResources(this.chk_ForceYtozero, "chk_ForceYtozero");
            this.chk_ForceYtozero.BackColor = System.Drawing.Color.Transparent;
            this.chk_ForceYtozero.Checked = true;
            this.chk_ForceYtozero.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_ForceYtozero.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_ForceYtozero.ForeColor = System.Drawing.Color.Blue;
            this.chk_ForceYtozero.Name = "chk_ForceYtozero";
            this.chk_ForceYtozero.Selected = false;
            this.chk_ForceYtozero.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_ForceYtozero.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_ForceYtozero.UseVisualStyleBackColor = false;
            this.chk_ForceYtozero.Click += new System.EventHandler(this.chk_ForceYtozero_Click);
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
            // lbl_Detail
            // 
            resources.ApplyResources(this.lbl_Detail, "lbl_Detail");
            this.lbl_Detail.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_Detail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Detail.Name = "lbl_Detail";
            // 
            // pnl_Detail
            // 
            resources.ApplyResources(this.pnl_Detail, "pnl_Detail");
            this.pnl_Detail.Controls.Add(this.pnl_Pin1);
            this.pnl_Detail.Controls.Add(this.pnl_EmptyUnit);
            this.pnl_Detail.Controls.Add(this.pnl_EdgeNotFound);
            this.pnl_Detail.Controls.Add(this.pnl_NoTemplate);
            this.pnl_Detail.Controls.Add(this.pnl_Position);
            this.pnl_Detail.Controls.Add(this.pnl_Pad);
            this.pnl_Detail.Controls.Add(this.pnl_SidePkgDimension);
            this.pnl_Detail.Controls.Add(this.pnl_SidePkgDefect);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPkgDimension);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPkgDefect);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadColorDefect);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadMissing);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadContamination);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadSpan);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadEdgeDistance);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadStandOff);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadEdgeLimit);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadSmear);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadExcess);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadBroken);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadPitchGap);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadDimension);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadArea);
            this.pnl_Detail.Controls.Add(this.pnl_SidePadOffset);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadColorDefect);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadMissing);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadContamination);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadSpan);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadEdgeDistance);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadStandOff);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadEdgeLimit);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadSmear);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadExcess);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadBroken);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadPitchGap);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadDimension);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadArea);
            this.pnl_Detail.Controls.Add(this.pnl_CenterPadOffset);
            this.pnl_Detail.Controls.Add(this.pnl_Orient);
            this.pnl_Detail.Name = "pnl_Detail";
            // 
            // pnl_Pin1
            // 
            resources.ApplyResources(this.pnl_Pin1, "pnl_Pin1");
            this.pnl_Pin1.Controls.Add(this.lbl_Pin1FailPercent);
            this.pnl_Pin1.Controls.Add(this.lbl_Pin1);
            this.pnl_Pin1.Controls.Add(this.lbl_Pin1FailCount);
            this.pnl_Pin1.Name = "pnl_Pin1";
            // 
            // lbl_Pin1FailPercent
            // 
            resources.ApplyResources(this.lbl_Pin1FailPercent, "lbl_Pin1FailPercent");
            this.lbl_Pin1FailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_Pin1FailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Pin1FailPercent.Name = "lbl_Pin1FailPercent";
            // 
            // lbl_Pin1
            // 
            resources.ApplyResources(this.lbl_Pin1, "lbl_Pin1");
            this.lbl_Pin1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_Pin1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Pin1.Name = "lbl_Pin1";
            // 
            // lbl_Pin1FailCount
            // 
            resources.ApplyResources(this.lbl_Pin1FailCount, "lbl_Pin1FailCount");
            this.lbl_Pin1FailCount.BackColor = System.Drawing.Color.White;
            this.lbl_Pin1FailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Pin1FailCount.Name = "lbl_Pin1FailCount";
            // 
            // pnl_EmptyUnit
            // 
            resources.ApplyResources(this.pnl_EmptyUnit, "pnl_EmptyUnit");
            this.pnl_EmptyUnit.Controls.Add(this.lbl_Empty);
            this.pnl_EmptyUnit.Controls.Add(this.lbl_EmptyUnitFailCount);
            this.pnl_EmptyUnit.Controls.Add(this.lbl_EmptyUnitFailPercent);
            this.pnl_EmptyUnit.Name = "pnl_EmptyUnit";
            // 
            // lbl_Empty
            // 
            resources.ApplyResources(this.lbl_Empty, "lbl_Empty");
            this.lbl_Empty.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_Empty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Empty.Name = "lbl_Empty";
            // 
            // lbl_EmptyUnitFailCount
            // 
            resources.ApplyResources(this.lbl_EmptyUnitFailCount, "lbl_EmptyUnitFailCount");
            this.lbl_EmptyUnitFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_EmptyUnitFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EmptyUnitFailCount.Name = "lbl_EmptyUnitFailCount";
            // 
            // lbl_EmptyUnitFailPercent
            // 
            resources.ApplyResources(this.lbl_EmptyUnitFailPercent, "lbl_EmptyUnitFailPercent");
            this.lbl_EmptyUnitFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_EmptyUnitFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EmptyUnitFailPercent.Name = "lbl_EmptyUnitFailPercent";
            // 
            // pnl_EdgeNotFound
            // 
            resources.ApplyResources(this.pnl_EdgeNotFound, "pnl_EdgeNotFound");
            this.pnl_EdgeNotFound.Controls.Add(this.lbl_EdgeNotFoundFailPercent);
            this.pnl_EdgeNotFound.Controls.Add(this.lbl_EdgeNotFound);
            this.pnl_EdgeNotFound.Controls.Add(this.lbl_EdgeNotFoundFailCount);
            this.pnl_EdgeNotFound.Name = "pnl_EdgeNotFound";
            // 
            // lbl_EdgeNotFoundFailPercent
            // 
            resources.ApplyResources(this.lbl_EdgeNotFoundFailPercent, "lbl_EdgeNotFoundFailPercent");
            this.lbl_EdgeNotFoundFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_EdgeNotFoundFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EdgeNotFoundFailPercent.Name = "lbl_EdgeNotFoundFailPercent";
            // 
            // lbl_EdgeNotFound
            // 
            resources.ApplyResources(this.lbl_EdgeNotFound, "lbl_EdgeNotFound");
            this.lbl_EdgeNotFound.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_EdgeNotFound.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EdgeNotFound.Name = "lbl_EdgeNotFound";
            // 
            // lbl_EdgeNotFoundFailCount
            // 
            resources.ApplyResources(this.lbl_EdgeNotFoundFailCount, "lbl_EdgeNotFoundFailCount");
            this.lbl_EdgeNotFoundFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_EdgeNotFoundFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EdgeNotFoundFailCount.Name = "lbl_EdgeNotFoundFailCount";
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
            // pnl_Position
            // 
            resources.ApplyResources(this.pnl_Position, "pnl_Position");
            this.pnl_Position.Controls.Add(this.lbl_PositionFailPercent);
            this.pnl_Position.Controls.Add(this.lbl_Position);
            this.pnl_Position.Controls.Add(this.lbl_PositionFailCount);
            this.pnl_Position.Name = "pnl_Position";
            // 
            // lbl_PositionFailPercent
            // 
            resources.ApplyResources(this.lbl_PositionFailPercent, "lbl_PositionFailPercent");
            this.lbl_PositionFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_PositionFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PositionFailPercent.Name = "lbl_PositionFailPercent";
            // 
            // lbl_Position
            // 
            resources.ApplyResources(this.lbl_Position, "lbl_Position");
            this.lbl_Position.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_Position.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Position.Name = "lbl_Position";
            // 
            // lbl_PositionFailCount
            // 
            resources.ApplyResources(this.lbl_PositionFailCount, "lbl_PositionFailCount");
            this.lbl_PositionFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_PositionFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PositionFailCount.Name = "lbl_PositionFailCount";
            // 
            // pnl_Pad
            // 
            resources.ApplyResources(this.pnl_Pad, "pnl_Pad");
            this.pnl_Pad.Controls.Add(this.lbl_PadFailPercent);
            this.pnl_Pad.Controls.Add(this.label12);
            this.pnl_Pad.Controls.Add(this.lbl_PadFailCount);
            this.pnl_Pad.Name = "pnl_Pad";
            // 
            // lbl_PadFailPercent
            // 
            resources.ApplyResources(this.lbl_PadFailPercent, "lbl_PadFailPercent");
            this.lbl_PadFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_PadFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PadFailPercent.Name = "lbl_PadFailPercent";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Name = "label12";
            // 
            // lbl_PadFailCount
            // 
            resources.ApplyResources(this.lbl_PadFailCount, "lbl_PadFailCount");
            this.lbl_PadFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_PadFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PadFailCount.Name = "lbl_PadFailCount";
            // 
            // pnl_SidePkgDimension
            // 
            resources.ApplyResources(this.pnl_SidePkgDimension, "pnl_SidePkgDimension");
            this.pnl_SidePkgDimension.Controls.Add(this.lbl_SidePkgDimensionFailPercent);
            this.pnl_SidePkgDimension.Controls.Add(this.label18);
            this.pnl_SidePkgDimension.Controls.Add(this.lbl_SidePkgDimensionFailCount);
            this.pnl_SidePkgDimension.Name = "pnl_SidePkgDimension";
            // 
            // lbl_SidePkgDimensionFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePkgDimensionFailPercent, "lbl_SidePkgDimensionFailPercent");
            this.lbl_SidePkgDimensionFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePkgDimensionFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePkgDimensionFailPercent.Name = "lbl_SidePkgDimensionFailPercent";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label18.Name = "label18";
            // 
            // lbl_SidePkgDimensionFailCount
            // 
            resources.ApplyResources(this.lbl_SidePkgDimensionFailCount, "lbl_SidePkgDimensionFailCount");
            this.lbl_SidePkgDimensionFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePkgDimensionFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePkgDimensionFailCount.Name = "lbl_SidePkgDimensionFailCount";
            // 
            // pnl_SidePkgDefect
            // 
            resources.ApplyResources(this.pnl_SidePkgDefect, "pnl_SidePkgDefect");
            this.pnl_SidePkgDefect.Controls.Add(this.lbl_SidePkgDefectFailPercent);
            this.pnl_SidePkgDefect.Controls.Add(this.label26);
            this.pnl_SidePkgDefect.Controls.Add(this.lbl_SidePkgDefectFailCount);
            this.pnl_SidePkgDefect.Name = "pnl_SidePkgDefect";
            // 
            // lbl_SidePkgDefectFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePkgDefectFailPercent, "lbl_SidePkgDefectFailPercent");
            this.lbl_SidePkgDefectFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePkgDefectFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePkgDefectFailPercent.Name = "lbl_SidePkgDefectFailPercent";
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label26.Name = "label26";
            // 
            // lbl_SidePkgDefectFailCount
            // 
            resources.ApplyResources(this.lbl_SidePkgDefectFailCount, "lbl_SidePkgDefectFailCount");
            this.lbl_SidePkgDefectFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePkgDefectFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePkgDefectFailCount.Name = "lbl_SidePkgDefectFailCount";
            // 
            // pnl_CenterPkgDimension
            // 
            resources.ApplyResources(this.pnl_CenterPkgDimension, "pnl_CenterPkgDimension");
            this.pnl_CenterPkgDimension.Controls.Add(this.lbl_CenterPkgDimensionFailPercent);
            this.pnl_CenterPkgDimension.Controls.Add(this.label32);
            this.pnl_CenterPkgDimension.Controls.Add(this.lbl_CenterPkgDimensionFailCount);
            this.pnl_CenterPkgDimension.Name = "pnl_CenterPkgDimension";
            // 
            // lbl_CenterPkgDimensionFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPkgDimensionFailPercent, "lbl_CenterPkgDimensionFailPercent");
            this.lbl_CenterPkgDimensionFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPkgDimensionFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPkgDimensionFailPercent.Name = "lbl_CenterPkgDimensionFailPercent";
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label32.Name = "label32";
            // 
            // lbl_CenterPkgDimensionFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPkgDimensionFailCount, "lbl_CenterPkgDimensionFailCount");
            this.lbl_CenterPkgDimensionFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPkgDimensionFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPkgDimensionFailCount.Name = "lbl_CenterPkgDimensionFailCount";
            // 
            // pnl_CenterPkgDefect
            // 
            resources.ApplyResources(this.pnl_CenterPkgDefect, "pnl_CenterPkgDefect");
            this.pnl_CenterPkgDefect.Controls.Add(this.lbl_CenterPkgDefectFailPercent);
            this.pnl_CenterPkgDefect.Controls.Add(this.label37);
            this.pnl_CenterPkgDefect.Controls.Add(this.lbl_CenterPkgDefectFailCount);
            this.pnl_CenterPkgDefect.Name = "pnl_CenterPkgDefect";
            // 
            // lbl_CenterPkgDefectFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPkgDefectFailPercent, "lbl_CenterPkgDefectFailPercent");
            this.lbl_CenterPkgDefectFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPkgDefectFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPkgDefectFailPercent.Name = "lbl_CenterPkgDefectFailPercent";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label37.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label37.Name = "label37";
            // 
            // lbl_CenterPkgDefectFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPkgDefectFailCount, "lbl_CenterPkgDefectFailCount");
            this.lbl_CenterPkgDefectFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPkgDefectFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPkgDefectFailCount.Name = "lbl_CenterPkgDefectFailCount";
            // 
            // pnl_SidePadColorDefect
            // 
            resources.ApplyResources(this.pnl_SidePadColorDefect, "pnl_SidePadColorDefect");
            this.pnl_SidePadColorDefect.Controls.Add(this.lbl_SidePadColorDefectFailPercent);
            this.pnl_SidePadColorDefect.Controls.Add(this.label38);
            this.pnl_SidePadColorDefect.Controls.Add(this.lbl_SidePadColorDefectFailCount);
            this.pnl_SidePadColorDefect.Name = "pnl_SidePadColorDefect";
            // 
            // lbl_SidePadColorDefectFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadColorDefectFailPercent, "lbl_SidePadColorDefectFailPercent");
            this.lbl_SidePadColorDefectFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadColorDefectFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadColorDefectFailPercent.Name = "lbl_SidePadColorDefectFailPercent";
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label38.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label38.Name = "label38";
            // 
            // lbl_SidePadColorDefectFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadColorDefectFailCount, "lbl_SidePadColorDefectFailCount");
            this.lbl_SidePadColorDefectFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadColorDefectFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadColorDefectFailCount.Name = "lbl_SidePadColorDefectFailCount";
            // 
            // pnl_SidePadMissing
            // 
            resources.ApplyResources(this.pnl_SidePadMissing, "pnl_SidePadMissing");
            this.pnl_SidePadMissing.Controls.Add(this.lbl_SidePadMissingFailPercent);
            this.pnl_SidePadMissing.Controls.Add(this.label42);
            this.pnl_SidePadMissing.Controls.Add(this.lbl_SidePadMissingFailCount);
            this.pnl_SidePadMissing.Name = "pnl_SidePadMissing";
            // 
            // lbl_SidePadMissingFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadMissingFailPercent, "lbl_SidePadMissingFailPercent");
            this.lbl_SidePadMissingFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadMissingFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadMissingFailPercent.Name = "lbl_SidePadMissingFailPercent";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label42.Name = "label42";
            // 
            // lbl_SidePadMissingFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadMissingFailCount, "lbl_SidePadMissingFailCount");
            this.lbl_SidePadMissingFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadMissingFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadMissingFailCount.Name = "lbl_SidePadMissingFailCount";
            // 
            // pnl_SidePadContamination
            // 
            resources.ApplyResources(this.pnl_SidePadContamination, "pnl_SidePadContamination");
            this.pnl_SidePadContamination.Controls.Add(this.lbl_SidePadContaminationFailPercent);
            this.pnl_SidePadContamination.Controls.Add(this.label8);
            this.pnl_SidePadContamination.Controls.Add(this.lbl_SidePadContaminationFailCount);
            this.pnl_SidePadContamination.Name = "pnl_SidePadContamination";
            // 
            // lbl_SidePadContaminationFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadContaminationFailPercent, "lbl_SidePadContaminationFailPercent");
            this.lbl_SidePadContaminationFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadContaminationFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadContaminationFailPercent.Name = "lbl_SidePadContaminationFailPercent";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Name = "label8";
            // 
            // lbl_SidePadContaminationFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadContaminationFailCount, "lbl_SidePadContaminationFailCount");
            this.lbl_SidePadContaminationFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadContaminationFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadContaminationFailCount.Name = "lbl_SidePadContaminationFailCount";
            // 
            // pnl_SidePadSpan
            // 
            resources.ApplyResources(this.pnl_SidePadSpan, "pnl_SidePadSpan");
            this.pnl_SidePadSpan.Controls.Add(this.lbl_SidePadSpanFailPercent);
            this.pnl_SidePadSpan.Controls.Add(this.label45);
            this.pnl_SidePadSpan.Controls.Add(this.lbl_SidePadSpanFailCount);
            this.pnl_SidePadSpan.Name = "pnl_SidePadSpan";
            // 
            // lbl_SidePadSpanFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadSpanFailPercent, "lbl_SidePadSpanFailPercent");
            this.lbl_SidePadSpanFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadSpanFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadSpanFailPercent.Name = "lbl_SidePadSpanFailPercent";
            // 
            // label45
            // 
            resources.ApplyResources(this.label45, "label45");
            this.label45.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label45.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label45.Name = "label45";
            // 
            // lbl_SidePadSpanFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadSpanFailCount, "lbl_SidePadSpanFailCount");
            this.lbl_SidePadSpanFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadSpanFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadSpanFailCount.Name = "lbl_SidePadSpanFailCount";
            // 
            // pnl_SidePadEdgeDistance
            // 
            resources.ApplyResources(this.pnl_SidePadEdgeDistance, "pnl_SidePadEdgeDistance");
            this.pnl_SidePadEdgeDistance.Controls.Add(this.lbl_SidePadEdgeDistanceFailPercent);
            this.pnl_SidePadEdgeDistance.Controls.Add(this.label43);
            this.pnl_SidePadEdgeDistance.Controls.Add(this.lbl_SidePadEdgeDistanceFailCount);
            this.pnl_SidePadEdgeDistance.Name = "pnl_SidePadEdgeDistance";
            // 
            // lbl_SidePadEdgeDistanceFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadEdgeDistanceFailPercent, "lbl_SidePadEdgeDistanceFailPercent");
            this.lbl_SidePadEdgeDistanceFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadEdgeDistanceFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadEdgeDistanceFailPercent.Name = "lbl_SidePadEdgeDistanceFailPercent";
            // 
            // label43
            // 
            resources.ApplyResources(this.label43, "label43");
            this.label43.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label43.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label43.Name = "label43";
            // 
            // lbl_SidePadEdgeDistanceFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadEdgeDistanceFailCount, "lbl_SidePadEdgeDistanceFailCount");
            this.lbl_SidePadEdgeDistanceFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadEdgeDistanceFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadEdgeDistanceFailCount.Name = "lbl_SidePadEdgeDistanceFailCount";
            // 
            // pnl_SidePadStandOff
            // 
            resources.ApplyResources(this.pnl_SidePadStandOff, "pnl_SidePadStandOff");
            this.pnl_SidePadStandOff.Controls.Add(this.lbl_SidePadStandOffFailPercent);
            this.pnl_SidePadStandOff.Controls.Add(this.label40);
            this.pnl_SidePadStandOff.Controls.Add(this.lbl_SidePadStandOffFailCount);
            this.pnl_SidePadStandOff.Name = "pnl_SidePadStandOff";
            // 
            // lbl_SidePadStandOffFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadStandOffFailPercent, "lbl_SidePadStandOffFailPercent");
            this.lbl_SidePadStandOffFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadStandOffFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadStandOffFailPercent.Name = "lbl_SidePadStandOffFailPercent";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label40.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label40.Name = "label40";
            // 
            // lbl_SidePadStandOffFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadStandOffFailCount, "lbl_SidePadStandOffFailCount");
            this.lbl_SidePadStandOffFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadStandOffFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadStandOffFailCount.Name = "lbl_SidePadStandOffFailCount";
            // 
            // pnl_SidePadEdgeLimit
            // 
            resources.ApplyResources(this.pnl_SidePadEdgeLimit, "pnl_SidePadEdgeLimit");
            this.pnl_SidePadEdgeLimit.Controls.Add(this.lbl_SidePadEdgeLimitFailPercent);
            this.pnl_SidePadEdgeLimit.Controls.Add(this.label30);
            this.pnl_SidePadEdgeLimit.Controls.Add(this.lbl_SidePadEdgeLimitFailCount);
            this.pnl_SidePadEdgeLimit.Name = "pnl_SidePadEdgeLimit";
            // 
            // lbl_SidePadEdgeLimitFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadEdgeLimitFailPercent, "lbl_SidePadEdgeLimitFailPercent");
            this.lbl_SidePadEdgeLimitFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadEdgeLimitFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadEdgeLimitFailPercent.Name = "lbl_SidePadEdgeLimitFailPercent";
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label30.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label30.Name = "label30";
            // 
            // lbl_SidePadEdgeLimitFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadEdgeLimitFailCount, "lbl_SidePadEdgeLimitFailCount");
            this.lbl_SidePadEdgeLimitFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadEdgeLimitFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadEdgeLimitFailCount.Name = "lbl_SidePadEdgeLimitFailCount";
            // 
            // pnl_SidePadSmear
            // 
            resources.ApplyResources(this.pnl_SidePadSmear, "pnl_SidePadSmear");
            this.pnl_SidePadSmear.Controls.Add(this.lbl_SidePadSmearFailPercent);
            this.pnl_SidePadSmear.Controls.Add(this.label13);
            this.pnl_SidePadSmear.Controls.Add(this.lbl_SidePadSmearFailCount);
            this.pnl_SidePadSmear.Name = "pnl_SidePadSmear";
            // 
            // lbl_SidePadSmearFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadSmearFailPercent, "lbl_SidePadSmearFailPercent");
            this.lbl_SidePadSmearFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadSmearFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadSmearFailPercent.Name = "lbl_SidePadSmearFailPercent";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Name = "label13";
            // 
            // lbl_SidePadSmearFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadSmearFailCount, "lbl_SidePadSmearFailCount");
            this.lbl_SidePadSmearFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadSmearFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadSmearFailCount.Name = "lbl_SidePadSmearFailCount";
            // 
            // pnl_SidePadExcess
            // 
            resources.ApplyResources(this.pnl_SidePadExcess, "pnl_SidePadExcess");
            this.pnl_SidePadExcess.Controls.Add(this.lbl_SidePadExcessFailPercent);
            this.pnl_SidePadExcess.Controls.Add(this.label20);
            this.pnl_SidePadExcess.Controls.Add(this.lbl_SidePadExcessFailCount);
            this.pnl_SidePadExcess.Name = "pnl_SidePadExcess";
            // 
            // lbl_SidePadExcessFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadExcessFailPercent, "lbl_SidePadExcessFailPercent");
            this.lbl_SidePadExcessFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadExcessFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadExcessFailPercent.Name = "lbl_SidePadExcessFailPercent";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label20.Name = "label20";
            // 
            // lbl_SidePadExcessFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadExcessFailCount, "lbl_SidePadExcessFailCount");
            this.lbl_SidePadExcessFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadExcessFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadExcessFailCount.Name = "lbl_SidePadExcessFailCount";
            // 
            // pnl_SidePadBroken
            // 
            resources.ApplyResources(this.pnl_SidePadBroken, "pnl_SidePadBroken");
            this.pnl_SidePadBroken.Controls.Add(this.lbl_SidePadBrokenFailPercent);
            this.pnl_SidePadBroken.Controls.Add(this.label24);
            this.pnl_SidePadBroken.Controls.Add(this.lbl_SidePadBrokenFailCount);
            this.pnl_SidePadBroken.Name = "pnl_SidePadBroken";
            // 
            // lbl_SidePadBrokenFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadBrokenFailPercent, "lbl_SidePadBrokenFailPercent");
            this.lbl_SidePadBrokenFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadBrokenFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadBrokenFailPercent.Name = "lbl_SidePadBrokenFailPercent";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label24.Name = "label24";
            // 
            // lbl_SidePadBrokenFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadBrokenFailCount, "lbl_SidePadBrokenFailCount");
            this.lbl_SidePadBrokenFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadBrokenFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadBrokenFailCount.Name = "lbl_SidePadBrokenFailCount";
            // 
            // pnl_SidePadPitchGap
            // 
            resources.ApplyResources(this.pnl_SidePadPitchGap, "pnl_SidePadPitchGap");
            this.pnl_SidePadPitchGap.Controls.Add(this.lbl_SidePadPitchGapFailPercent);
            this.pnl_SidePadPitchGap.Controls.Add(this.label29);
            this.pnl_SidePadPitchGap.Controls.Add(this.lbl_SidePadPitchGapFailCount);
            this.pnl_SidePadPitchGap.Name = "pnl_SidePadPitchGap";
            // 
            // lbl_SidePadPitchGapFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadPitchGapFailPercent, "lbl_SidePadPitchGapFailPercent");
            this.lbl_SidePadPitchGapFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadPitchGapFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadPitchGapFailPercent.Name = "lbl_SidePadPitchGapFailPercent";
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label29.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label29.Name = "label29";
            // 
            // lbl_SidePadPitchGapFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadPitchGapFailCount, "lbl_SidePadPitchGapFailCount");
            this.lbl_SidePadPitchGapFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadPitchGapFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadPitchGapFailCount.Name = "lbl_SidePadPitchGapFailCount";
            // 
            // pnl_SidePadDimension
            // 
            resources.ApplyResources(this.pnl_SidePadDimension, "pnl_SidePadDimension");
            this.pnl_SidePadDimension.Controls.Add(this.lbl_SidePadDimensionFailPercent);
            this.pnl_SidePadDimension.Controls.Add(this.label33);
            this.pnl_SidePadDimension.Controls.Add(this.lbl_SidePadDimensionFailCount);
            this.pnl_SidePadDimension.Name = "pnl_SidePadDimension";
            // 
            // lbl_SidePadDimensionFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadDimensionFailPercent, "lbl_SidePadDimensionFailPercent");
            this.lbl_SidePadDimensionFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadDimensionFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadDimensionFailPercent.Name = "lbl_SidePadDimensionFailPercent";
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label33.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label33.Name = "label33";
            // 
            // lbl_SidePadDimensionFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadDimensionFailCount, "lbl_SidePadDimensionFailCount");
            this.lbl_SidePadDimensionFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadDimensionFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadDimensionFailCount.Name = "lbl_SidePadDimensionFailCount";
            // 
            // pnl_SidePadArea
            // 
            resources.ApplyResources(this.pnl_SidePadArea, "pnl_SidePadArea");
            this.pnl_SidePadArea.Controls.Add(this.lbl_SidePadAreaFailPercent);
            this.pnl_SidePadArea.Controls.Add(this.label36);
            this.pnl_SidePadArea.Controls.Add(this.lbl_SidePadAreaFailCount);
            this.pnl_SidePadArea.Name = "pnl_SidePadArea";
            // 
            // lbl_SidePadAreaFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadAreaFailPercent, "lbl_SidePadAreaFailPercent");
            this.lbl_SidePadAreaFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadAreaFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadAreaFailPercent.Name = "lbl_SidePadAreaFailPercent";
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label36.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label36.Name = "label36";
            // 
            // lbl_SidePadAreaFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadAreaFailCount, "lbl_SidePadAreaFailCount");
            this.lbl_SidePadAreaFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadAreaFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadAreaFailCount.Name = "lbl_SidePadAreaFailCount";
            // 
            // pnl_SidePadOffset
            // 
            resources.ApplyResources(this.pnl_SidePadOffset, "pnl_SidePadOffset");
            this.pnl_SidePadOffset.Controls.Add(this.lbl_SidePadOffsetFailPercent);
            this.pnl_SidePadOffset.Controls.Add(this.label39);
            this.pnl_SidePadOffset.Controls.Add(this.lbl_SidePadOffsetFailCount);
            this.pnl_SidePadOffset.Name = "pnl_SidePadOffset";
            // 
            // lbl_SidePadOffsetFailPercent
            // 
            resources.ApplyResources(this.lbl_SidePadOffsetFailPercent, "lbl_SidePadOffsetFailPercent");
            this.lbl_SidePadOffsetFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadOffsetFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadOffsetFailPercent.Name = "lbl_SidePadOffsetFailPercent";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label39.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label39.Name = "label39";
            // 
            // lbl_SidePadOffsetFailCount
            // 
            resources.ApplyResources(this.lbl_SidePadOffsetFailCount, "lbl_SidePadOffsetFailCount");
            this.lbl_SidePadOffsetFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_SidePadOffsetFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_SidePadOffsetFailCount.Name = "lbl_SidePadOffsetFailCount";
            // 
            // pnl_CenterPadColorDefect
            // 
            resources.ApplyResources(this.pnl_CenterPadColorDefect, "pnl_CenterPadColorDefect");
            this.pnl_CenterPadColorDefect.Controls.Add(this.lbl_CenterPadColorDefectFailPercent);
            this.pnl_CenterPadColorDefect.Controls.Add(this.label35);
            this.pnl_CenterPadColorDefect.Controls.Add(this.lbl_CenterPadColorDefectFailCount);
            this.pnl_CenterPadColorDefect.Name = "pnl_CenterPadColorDefect";
            // 
            // lbl_CenterPadColorDefectFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadColorDefectFailPercent, "lbl_CenterPadColorDefectFailPercent");
            this.lbl_CenterPadColorDefectFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadColorDefectFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadColorDefectFailPercent.Name = "lbl_CenterPadColorDefectFailPercent";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label35.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label35.Name = "label35";
            // 
            // lbl_CenterPadColorDefectFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadColorDefectFailCount, "lbl_CenterPadColorDefectFailCount");
            this.lbl_CenterPadColorDefectFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadColorDefectFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadColorDefectFailCount.Name = "lbl_CenterPadColorDefectFailCount";
            // 
            // pnl_CenterPadMissing
            // 
            resources.ApplyResources(this.pnl_CenterPadMissing, "pnl_CenterPadMissing");
            this.pnl_CenterPadMissing.Controls.Add(this.lbl_CenterPadMissingFailPercent);
            this.pnl_CenterPadMissing.Controls.Add(this.label23);
            this.pnl_CenterPadMissing.Controls.Add(this.lbl_CenterPadMissingFailCount);
            this.pnl_CenterPadMissing.Name = "pnl_CenterPadMissing";
            // 
            // lbl_CenterPadMissingFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadMissingFailPercent, "lbl_CenterPadMissingFailPercent");
            this.lbl_CenterPadMissingFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadMissingFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadMissingFailPercent.Name = "lbl_CenterPadMissingFailPercent";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label23.Name = "label23";
            // 
            // lbl_CenterPadMissingFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadMissingFailCount, "lbl_CenterPadMissingFailCount");
            this.lbl_CenterPadMissingFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadMissingFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadMissingFailCount.Name = "lbl_CenterPadMissingFailCount";
            // 
            // pnl_CenterPadContamination
            // 
            resources.ApplyResources(this.pnl_CenterPadContamination, "pnl_CenterPadContamination");
            this.pnl_CenterPadContamination.Controls.Add(this.lbl_CenterPadContaminationFailPercent);
            this.pnl_CenterPadContamination.Controls.Add(this.label31);
            this.pnl_CenterPadContamination.Controls.Add(this.lbl_CenterPadContaminationFailCount);
            this.pnl_CenterPadContamination.Name = "pnl_CenterPadContamination";
            // 
            // lbl_CenterPadContaminationFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadContaminationFailPercent, "lbl_CenterPadContaminationFailPercent");
            this.lbl_CenterPadContaminationFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadContaminationFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadContaminationFailPercent.Name = "lbl_CenterPadContaminationFailPercent";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label31.Name = "label31";
            // 
            // lbl_CenterPadContaminationFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadContaminationFailCount, "lbl_CenterPadContaminationFailCount");
            this.lbl_CenterPadContaminationFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadContaminationFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadContaminationFailCount.Name = "lbl_CenterPadContaminationFailCount";
            // 
            // pnl_CenterPadSpan
            // 
            resources.ApplyResources(this.pnl_CenterPadSpan, "pnl_CenterPadSpan");
            this.pnl_CenterPadSpan.Controls.Add(this.lbl_CenterPadSpanFailPercent);
            this.pnl_CenterPadSpan.Controls.Add(this.label44);
            this.pnl_CenterPadSpan.Controls.Add(this.lbl_CenterPadSpanFailCount);
            this.pnl_CenterPadSpan.Name = "pnl_CenterPadSpan";
            // 
            // lbl_CenterPadSpanFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadSpanFailPercent, "lbl_CenterPadSpanFailPercent");
            this.lbl_CenterPadSpanFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadSpanFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadSpanFailPercent.Name = "lbl_CenterPadSpanFailPercent";
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label44.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label44.Name = "label44";
            // 
            // lbl_CenterPadSpanFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadSpanFailCount, "lbl_CenterPadSpanFailCount");
            this.lbl_CenterPadSpanFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadSpanFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadSpanFailCount.Name = "lbl_CenterPadSpanFailCount";
            // 
            // pnl_CenterPadEdgeDistance
            // 
            resources.ApplyResources(this.pnl_CenterPadEdgeDistance, "pnl_CenterPadEdgeDistance");
            this.pnl_CenterPadEdgeDistance.Controls.Add(this.lbl_CenterPadEdgeDistanceFailPercent);
            this.pnl_CenterPadEdgeDistance.Controls.Add(this.label41);
            this.pnl_CenterPadEdgeDistance.Controls.Add(this.lbl_CenterPadEdgeDistanceFailCount);
            this.pnl_CenterPadEdgeDistance.Name = "pnl_CenterPadEdgeDistance";
            // 
            // lbl_CenterPadEdgeDistanceFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadEdgeDistanceFailPercent, "lbl_CenterPadEdgeDistanceFailPercent");
            this.lbl_CenterPadEdgeDistanceFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadEdgeDistanceFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadEdgeDistanceFailPercent.Name = "lbl_CenterPadEdgeDistanceFailPercent";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label41.Name = "label41";
            // 
            // lbl_CenterPadEdgeDistanceFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadEdgeDistanceFailCount, "lbl_CenterPadEdgeDistanceFailCount");
            this.lbl_CenterPadEdgeDistanceFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadEdgeDistanceFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadEdgeDistanceFailCount.Name = "lbl_CenterPadEdgeDistanceFailCount";
            // 
            // pnl_CenterPadStandOff
            // 
            resources.ApplyResources(this.pnl_CenterPadStandOff, "pnl_CenterPadStandOff");
            this.pnl_CenterPadStandOff.Controls.Add(this.lbl_CenterPadStandOffFailPercent);
            this.pnl_CenterPadStandOff.Controls.Add(this.label34);
            this.pnl_CenterPadStandOff.Controls.Add(this.lbl_CenterPadStandOffFailCount);
            this.pnl_CenterPadStandOff.Name = "pnl_CenterPadStandOff";
            // 
            // lbl_CenterPadStandOffFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadStandOffFailPercent, "lbl_CenterPadStandOffFailPercent");
            this.lbl_CenterPadStandOffFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadStandOffFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadStandOffFailPercent.Name = "lbl_CenterPadStandOffFailPercent";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label34.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label34.Name = "label34";
            // 
            // lbl_CenterPadStandOffFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadStandOffFailCount, "lbl_CenterPadStandOffFailCount");
            this.lbl_CenterPadStandOffFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadStandOffFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadStandOffFailCount.Name = "lbl_CenterPadStandOffFailCount";
            // 
            // pnl_CenterPadEdgeLimit
            // 
            resources.ApplyResources(this.pnl_CenterPadEdgeLimit, "pnl_CenterPadEdgeLimit");
            this.pnl_CenterPadEdgeLimit.Controls.Add(this.lbl_CenterPadEdgeLimitFailPercent);
            this.pnl_CenterPadEdgeLimit.Controls.Add(this.label27);
            this.pnl_CenterPadEdgeLimit.Controls.Add(this.lbl_CenterPadEdgeLimitFailCount);
            this.pnl_CenterPadEdgeLimit.Name = "pnl_CenterPadEdgeLimit";
            // 
            // lbl_CenterPadEdgeLimitFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadEdgeLimitFailPercent, "lbl_CenterPadEdgeLimitFailPercent");
            this.lbl_CenterPadEdgeLimitFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadEdgeLimitFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadEdgeLimitFailPercent.Name = "lbl_CenterPadEdgeLimitFailPercent";
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label27.Name = "label27";
            // 
            // lbl_CenterPadEdgeLimitFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadEdgeLimitFailCount, "lbl_CenterPadEdgeLimitFailCount");
            this.lbl_CenterPadEdgeLimitFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadEdgeLimitFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadEdgeLimitFailCount.Name = "lbl_CenterPadEdgeLimitFailCount";
            // 
            // pnl_CenterPadSmear
            // 
            resources.ApplyResources(this.pnl_CenterPadSmear, "pnl_CenterPadSmear");
            this.pnl_CenterPadSmear.Controls.Add(this.lbl_CenterPadSmearFailPercent);
            this.pnl_CenterPadSmear.Controls.Add(this.label25);
            this.pnl_CenterPadSmear.Controls.Add(this.lbl_CenterPadSmearFailCount);
            this.pnl_CenterPadSmear.Name = "pnl_CenterPadSmear";
            // 
            // lbl_CenterPadSmearFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadSmearFailPercent, "lbl_CenterPadSmearFailPercent");
            this.lbl_CenterPadSmearFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadSmearFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadSmearFailPercent.Name = "lbl_CenterPadSmearFailPercent";
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label25.Name = "label25";
            // 
            // lbl_CenterPadSmearFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadSmearFailCount, "lbl_CenterPadSmearFailCount");
            this.lbl_CenterPadSmearFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadSmearFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadSmearFailCount.Name = "lbl_CenterPadSmearFailCount";
            // 
            // pnl_CenterPadExcess
            // 
            resources.ApplyResources(this.pnl_CenterPadExcess, "pnl_CenterPadExcess");
            this.pnl_CenterPadExcess.Controls.Add(this.lbl_CenterPadExcessFailPercent);
            this.pnl_CenterPadExcess.Controls.Add(this.label28);
            this.pnl_CenterPadExcess.Controls.Add(this.lbl_CenterPadExcessFailCount);
            this.pnl_CenterPadExcess.Name = "pnl_CenterPadExcess";
            // 
            // lbl_CenterPadExcessFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadExcessFailPercent, "lbl_CenterPadExcessFailPercent");
            this.lbl_CenterPadExcessFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadExcessFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadExcessFailPercent.Name = "lbl_CenterPadExcessFailPercent";
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label28.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label28.Name = "label28";
            // 
            // lbl_CenterPadExcessFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadExcessFailCount, "lbl_CenterPadExcessFailCount");
            this.lbl_CenterPadExcessFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadExcessFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadExcessFailCount.Name = "lbl_CenterPadExcessFailCount";
            // 
            // pnl_CenterPadBroken
            // 
            resources.ApplyResources(this.pnl_CenterPadBroken, "pnl_CenterPadBroken");
            this.pnl_CenterPadBroken.Controls.Add(this.lbl_CenterPadBrokenFailPercent);
            this.pnl_CenterPadBroken.Controls.Add(this.label19);
            this.pnl_CenterPadBroken.Controls.Add(this.lbl_CenterPadBrokenFailCount);
            this.pnl_CenterPadBroken.Name = "pnl_CenterPadBroken";
            // 
            // lbl_CenterPadBrokenFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadBrokenFailPercent, "lbl_CenterPadBrokenFailPercent");
            this.lbl_CenterPadBrokenFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadBrokenFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadBrokenFailPercent.Name = "lbl_CenterPadBrokenFailPercent";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label19.Name = "label19";
            // 
            // lbl_CenterPadBrokenFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadBrokenFailCount, "lbl_CenterPadBrokenFailCount");
            this.lbl_CenterPadBrokenFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadBrokenFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadBrokenFailCount.Name = "lbl_CenterPadBrokenFailCount";
            // 
            // pnl_CenterPadPitchGap
            // 
            resources.ApplyResources(this.pnl_CenterPadPitchGap, "pnl_CenterPadPitchGap");
            this.pnl_CenterPadPitchGap.Controls.Add(this.lbl_CenterPadPitchGapFailPercent);
            this.pnl_CenterPadPitchGap.Controls.Add(this.label14);
            this.pnl_CenterPadPitchGap.Controls.Add(this.lbl_CenterPadPitchGapFailCount);
            this.pnl_CenterPadPitchGap.Name = "pnl_CenterPadPitchGap";
            // 
            // lbl_CenterPadPitchGapFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadPitchGapFailPercent, "lbl_CenterPadPitchGapFailPercent");
            this.lbl_CenterPadPitchGapFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadPitchGapFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadPitchGapFailPercent.Name = "lbl_CenterPadPitchGapFailPercent";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Name = "label14";
            // 
            // lbl_CenterPadPitchGapFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadPitchGapFailCount, "lbl_CenterPadPitchGapFailCount");
            this.lbl_CenterPadPitchGapFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadPitchGapFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadPitchGapFailCount.Name = "lbl_CenterPadPitchGapFailCount";
            // 
            // pnl_CenterPadDimension
            // 
            resources.ApplyResources(this.pnl_CenterPadDimension, "pnl_CenterPadDimension");
            this.pnl_CenterPadDimension.Controls.Add(this.lbl_CenterPadDimensionFailPercent);
            this.pnl_CenterPadDimension.Controls.Add(this.label11);
            this.pnl_CenterPadDimension.Controls.Add(this.lbl_CenterPadDimensionFailCount);
            this.pnl_CenterPadDimension.Name = "pnl_CenterPadDimension";
            // 
            // lbl_CenterPadDimensionFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadDimensionFailPercent, "lbl_CenterPadDimensionFailPercent");
            this.lbl_CenterPadDimensionFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadDimensionFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadDimensionFailPercent.Name = "lbl_CenterPadDimensionFailPercent";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Name = "label11";
            // 
            // lbl_CenterPadDimensionFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadDimensionFailCount, "lbl_CenterPadDimensionFailCount");
            this.lbl_CenterPadDimensionFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadDimensionFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadDimensionFailCount.Name = "lbl_CenterPadDimensionFailCount";
            // 
            // pnl_CenterPadArea
            // 
            resources.ApplyResources(this.pnl_CenterPadArea, "pnl_CenterPadArea");
            this.pnl_CenterPadArea.Controls.Add(this.lbl_CenterPadAreaFailPercent);
            this.pnl_CenterPadArea.Controls.Add(this.label5);
            this.pnl_CenterPadArea.Controls.Add(this.lbl_CenterPadAreaFailCount);
            this.pnl_CenterPadArea.Name = "pnl_CenterPadArea";
            // 
            // lbl_CenterPadAreaFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadAreaFailPercent, "lbl_CenterPadAreaFailPercent");
            this.lbl_CenterPadAreaFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadAreaFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadAreaFailPercent.Name = "lbl_CenterPadAreaFailPercent";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Name = "label5";
            // 
            // lbl_CenterPadAreaFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadAreaFailCount, "lbl_CenterPadAreaFailCount");
            this.lbl_CenterPadAreaFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadAreaFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadAreaFailCount.Name = "lbl_CenterPadAreaFailCount";
            // 
            // pnl_CenterPadOffset
            // 
            resources.ApplyResources(this.pnl_CenterPadOffset, "pnl_CenterPadOffset");
            this.pnl_CenterPadOffset.Controls.Add(this.lbl_CenterPadOffsetFailPercent);
            this.pnl_CenterPadOffset.Controls.Add(this.label22);
            this.pnl_CenterPadOffset.Controls.Add(this.lbl_CenterPadOffsetFailCount);
            this.pnl_CenterPadOffset.Name = "pnl_CenterPadOffset";
            // 
            // lbl_CenterPadOffsetFailPercent
            // 
            resources.ApplyResources(this.lbl_CenterPadOffsetFailPercent, "lbl_CenterPadOffsetFailPercent");
            this.lbl_CenterPadOffsetFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadOffsetFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadOffsetFailPercent.Name = "lbl_CenterPadOffsetFailPercent";
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label22.Name = "label22";
            // 
            // lbl_CenterPadOffsetFailCount
            // 
            resources.ApplyResources(this.lbl_CenterPadOffsetFailCount, "lbl_CenterPadOffsetFailCount");
            this.lbl_CenterPadOffsetFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_CenterPadOffsetFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CenterPadOffsetFailCount.Name = "lbl_CenterPadOffsetFailCount";
            // 
            // pnl_Orient
            // 
            resources.ApplyResources(this.pnl_Orient, "pnl_Orient");
            this.pnl_Orient.Controls.Add(this.lbl_OrientFailPercent);
            this.pnl_Orient.Controls.Add(this.lbl_Orient);
            this.pnl_Orient.Controls.Add(this.lbl_OrientFailCount);
            this.pnl_Orient.Name = "pnl_Orient";
            // 
            // lbl_OrientFailPercent
            // 
            resources.ApplyResources(this.lbl_OrientFailPercent, "lbl_OrientFailPercent");
            this.lbl_OrientFailPercent.BackColor = System.Drawing.Color.White;
            this.lbl_OrientFailPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_OrientFailPercent.Name = "lbl_OrientFailPercent";
            // 
            // lbl_Orient
            // 
            resources.ApplyResources(this.lbl_Orient, "lbl_Orient");
            this.lbl_Orient.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_Orient.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Orient.Name = "lbl_Orient";
            // 
            // lbl_OrientFailCount
            // 
            resources.ApplyResources(this.lbl_OrientFailCount, "lbl_OrientFailCount");
            this.lbl_OrientFailCount.BackColor = System.Drawing.Color.White;
            this.lbl_OrientFailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_OrientFailCount.Name = "lbl_OrientFailCount";
            // 
            // lbl_Yield
            // 
            resources.ApplyResources(this.lbl_Yield, "lbl_Yield");
            this.lbl_Yield.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbl_Yield.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Yield.Name = "lbl_Yield";
            // 
            // lbl_FailCount
            // 
            resources.ApplyResources(this.lbl_FailCount, "lbl_FailCount");
            this.lbl_FailCount.BackColor = System.Drawing.Color.White;
            this.lbl_FailCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_FailCount.Name = "lbl_FailCount";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Name = "label1";
            // 
            // lbl_TestedTotal
            // 
            resources.ApplyResources(this.lbl_TestedTotal, "lbl_TestedTotal");
            this.lbl_TestedTotal.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbl_TestedTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_TestedTotal.Name = "lbl_TestedTotal";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Name = "label6";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Name = "label2";
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
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Name = "label9";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.label17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label17.Name = "label17";
            // 
            // pic_Template
            // 
            resources.ApplyResources(this.pic_Template, "pic_Template");
            this.pic_Template.BackColor = System.Drawing.Color.Black;
            this.pic_Template.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pic_Template.Name = "pic_Template";
            this.pic_Template.TabStop = false;
            // 
            // lblOrientationResult
            // 
            resources.ApplyResources(this.lblOrientationResult, "lblOrientationResult");
            this.lblOrientationResult.BackColor = System.Drawing.Color.White;
            this.lblOrientationResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOrientationResult.Name = "lblOrientationResult";
            // 
            // Vision3Page
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.lblOrientationResult);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lbl_OperatorID);
            this.Controls.Add(this.lbl_Yield);
            this.Controls.Add(this.lbl_FailCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_TestedTotal);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl_PassCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbl_Detail);
            this.Controls.Add(this.pnl_Detail);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbl_TotalTime);
            this.Controls.Add(this.lbl_GrabDelay);
            this.Controls.Add(this.lbl_GrabTime);
            this.Controls.Add(this.lbl_V2Grab);
            this.Controls.Add(this.lbl_V2Process);
            this.Controls.Add(this.lbl_ProcessTime);
            this.Controls.Add(this.lbl_Pin1ROI);
            this.Controls.Add(this.chk_ForceYtozero);
            this.Controls.Add(this.pic_Template);
            this.Controls.Add(this.lbl_ResultStatus);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lbl_LotID);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btn_Reset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Vision3Page";
            this.pnl_Detail.ResumeLayout(false);
            this.pnl_Pin1.ResumeLayout(false);
            this.pnl_EmptyUnit.ResumeLayout(false);
            this.pnl_EdgeNotFound.ResumeLayout(false);
            this.pnl_NoTemplate.ResumeLayout(false);
            this.pnl_Position.ResumeLayout(false);
            this.pnl_Pad.ResumeLayout(false);
            this.pnl_SidePkgDimension.ResumeLayout(false);
            this.pnl_SidePkgDefect.ResumeLayout(false);
            this.pnl_CenterPkgDimension.ResumeLayout(false);
            this.pnl_CenterPkgDefect.ResumeLayout(false);
            this.pnl_SidePadColorDefect.ResumeLayout(false);
            this.pnl_SidePadMissing.ResumeLayout(false);
            this.pnl_SidePadContamination.ResumeLayout(false);
            this.pnl_SidePadSpan.ResumeLayout(false);
            this.pnl_SidePadEdgeDistance.ResumeLayout(false);
            this.pnl_SidePadStandOff.ResumeLayout(false);
            this.pnl_SidePadEdgeLimit.ResumeLayout(false);
            this.pnl_SidePadSmear.ResumeLayout(false);
            this.pnl_SidePadExcess.ResumeLayout(false);
            this.pnl_SidePadBroken.ResumeLayout(false);
            this.pnl_SidePadPitchGap.ResumeLayout(false);
            this.pnl_SidePadDimension.ResumeLayout(false);
            this.pnl_SidePadArea.ResumeLayout(false);
            this.pnl_SidePadOffset.ResumeLayout(false);
            this.pnl_CenterPadColorDefect.ResumeLayout(false);
            this.pnl_CenterPadMissing.ResumeLayout(false);
            this.pnl_CenterPadContamination.ResumeLayout(false);
            this.pnl_CenterPadSpan.ResumeLayout(false);
            this.pnl_CenterPadEdgeDistance.ResumeLayout(false);
            this.pnl_CenterPadStandOff.ResumeLayout(false);
            this.pnl_CenterPadEdgeLimit.ResumeLayout(false);
            this.pnl_CenterPadSmear.ResumeLayout(false);
            this.pnl_CenterPadExcess.ResumeLayout(false);
            this.pnl_CenterPadBroken.ResumeLayout(false);
            this.pnl_CenterPadPitchGap.ResumeLayout(false);
            this.pnl_CenterPadDimension.ResumeLayout(false);
            this.pnl_CenterPadArea.ResumeLayout(false);
            this.pnl_CenterPadOffset.ResumeLayout(false);
            this.pnl_Orient.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Template)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pic_Template;
        private System.Windows.Forms.Label lbl_ResultStatus;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbl_LotID;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lbl_OperatorID;
        private System.Windows.Forms.Button btn_Reset;
        private System.Windows.Forms.Timer timer_Live;
        private SRMControl.SRMCheckBox chk_ForceYtozero;
        private SRMControl.SRMLabel lbl_Pin1ROI;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_TotalTime;
        private System.Windows.Forms.Label lbl_GrabDelay;
        private System.Windows.Forms.Label lbl_GrabTime;
        private System.Windows.Forms.Label lbl_V2Grab;
        private System.Windows.Forms.Label lbl_V2Process;
        private System.Windows.Forms.Label lbl_ProcessTime;
        private System.Windows.Forms.Label lbl_Detail;
        private System.Windows.Forms.Panel pnl_Detail;
        private System.Windows.Forms.Panel pnl_Pin1;
        private System.Windows.Forms.Label lbl_Pin1FailPercent;
        private System.Windows.Forms.Label lbl_Pin1;
        private System.Windows.Forms.Label lbl_Pin1FailCount;
        private System.Windows.Forms.Panel pnl_EdgeNotFound;
        private System.Windows.Forms.Label lbl_EdgeNotFoundFailPercent;
        private System.Windows.Forms.Label lbl_EdgeNotFound;
        private System.Windows.Forms.Label lbl_EdgeNotFoundFailCount;
        private System.Windows.Forms.Panel pnl_NoTemplate;
        private System.Windows.Forms.Label lbl_NoTemplateFailPercent;
        private System.Windows.Forms.Label lbl_NoTemplate;
        private System.Windows.Forms.Label lbl_NoTemplateFailCount;
        private System.Windows.Forms.Panel pnl_Position;
        private System.Windows.Forms.Label lbl_PositionFailPercent;
        private System.Windows.Forms.Label lbl_Position;
        private System.Windows.Forms.Label lbl_PositionFailCount;
        private System.Windows.Forms.Panel pnl_EmptyUnit;
        private System.Windows.Forms.Label lbl_Empty;
        private System.Windows.Forms.Label lbl_EmptyUnitFailCount;
        private System.Windows.Forms.Label lbl_EmptyUnitFailPercent;
        private System.Windows.Forms.Panel pnl_CenterPadSmear;
        private System.Windows.Forms.Label lbl_CenterPadSmearFailPercent;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label lbl_CenterPadSmearFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadExcess;
        private System.Windows.Forms.Label lbl_CenterPadExcessFailPercent;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label lbl_CenterPadExcessFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadBroken;
        private System.Windows.Forms.Label lbl_CenterPadBrokenFailPercent;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lbl_CenterPadBrokenFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadPitchGap;
        private System.Windows.Forms.Label lbl_CenterPadPitchGapFailPercent;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lbl_CenterPadPitchGapFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadDimension;
        private System.Windows.Forms.Label lbl_CenterPadDimensionFailPercent;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbl_CenterPadDimensionFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadArea;
        private System.Windows.Forms.Label lbl_CenterPadAreaFailPercent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_CenterPadAreaFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadOffset;
        private System.Windows.Forms.Label lbl_CenterPadOffsetFailPercent;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lbl_CenterPadOffsetFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadContamination;
        private System.Windows.Forms.Label lbl_CenterPadContaminationFailPercent;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label lbl_CenterPadContaminationFailCount;
        private System.Windows.Forms.Panel pnl_SidePadContamination;
        private System.Windows.Forms.Label lbl_SidePadContaminationFailPercent;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbl_SidePadContaminationFailCount;
        private System.Windows.Forms.Panel pnl_SidePadSmear;
        private System.Windows.Forms.Label lbl_SidePadSmearFailPercent;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lbl_SidePadSmearFailCount;
        private System.Windows.Forms.Panel pnl_SidePadExcess;
        private System.Windows.Forms.Label lbl_SidePadExcessFailPercent;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label lbl_SidePadExcessFailCount;
        private System.Windows.Forms.Panel pnl_SidePadBroken;
        private System.Windows.Forms.Label lbl_SidePadBrokenFailPercent;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label lbl_SidePadBrokenFailCount;
        private System.Windows.Forms.Panel pnl_SidePadPitchGap;
        private System.Windows.Forms.Label lbl_SidePadPitchGapFailPercent;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label lbl_SidePadPitchGapFailCount;
        private System.Windows.Forms.Panel pnl_SidePadDimension;
        private System.Windows.Forms.Label lbl_SidePadDimensionFailPercent;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label lbl_SidePadDimensionFailCount;
        private System.Windows.Forms.Panel pnl_SidePadArea;
        private System.Windows.Forms.Label lbl_SidePadAreaFailPercent;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label lbl_SidePadAreaFailCount;
        private System.Windows.Forms.Panel pnl_SidePadOffset;
        private System.Windows.Forms.Label lbl_SidePadOffsetFailPercent;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label lbl_SidePadOffsetFailCount;
        private System.Windows.Forms.Panel pnl_SidePadMissing;
        private System.Windows.Forms.Label lbl_SidePadMissingFailPercent;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label lbl_SidePadMissingFailCount;
        private System.Windows.Forms.Panel pnl_SidePkgDimension;
        private System.Windows.Forms.Label lbl_SidePkgDimensionFailPercent;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lbl_SidePkgDimensionFailCount;
        private System.Windows.Forms.Panel pnl_SidePkgDefect;
        private System.Windows.Forms.Label lbl_SidePkgDefectFailPercent;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label lbl_SidePkgDefectFailCount;
        private System.Windows.Forms.Panel pnl_CenterPkgDimension;
        private System.Windows.Forms.Label lbl_CenterPkgDimensionFailPercent;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label lbl_CenterPkgDimensionFailCount;
        private System.Windows.Forms.Panel pnl_CenterPkgDefect;
        private System.Windows.Forms.Label lbl_CenterPkgDefectFailPercent;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label lbl_CenterPkgDefectFailCount;
        private System.Windows.Forms.Label lbl_Yield;
        private System.Windows.Forms.Label lbl_FailCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_TestedTotal;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_PassCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnl_Pad;
        private System.Windows.Forms.Label lbl_PadFailPercent;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbl_PadFailCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Panel pnl_CenterPadMissing;
        private System.Windows.Forms.Label lbl_CenterPadMissingFailPercent;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lbl_CenterPadMissingFailCount;
        private System.Windows.Forms.Panel pnl_SidePadEdgeLimit;
        private System.Windows.Forms.Label lbl_SidePadEdgeLimitFailPercent;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label lbl_SidePadEdgeLimitFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadEdgeLimit;
        private System.Windows.Forms.Label lbl_CenterPadEdgeLimitFailPercent;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label lbl_CenterPadEdgeLimitFailCount;
        private System.Windows.Forms.Panel pnl_SidePadStandOff;
        private System.Windows.Forms.Label lbl_SidePadStandOffFailPercent;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label lbl_SidePadStandOffFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadStandOff;
        private System.Windows.Forms.Label lbl_CenterPadStandOffFailPercent;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label lbl_CenterPadStandOffFailCount;
        private System.Windows.Forms.Panel pnl_Orient;
        private System.Windows.Forms.Label lbl_OrientFailPercent;
        private System.Windows.Forms.Label lbl_Orient;
        private System.Windows.Forms.Label lbl_OrientFailCount;
        private System.Windows.Forms.Label lblOrientationResult;
        private System.Windows.Forms.Panel pnl_SidePadColorDefect;
        private System.Windows.Forms.Label lbl_SidePadColorDefectFailPercent;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label lbl_SidePadColorDefectFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadColorDefect;
        private System.Windows.Forms.Label lbl_CenterPadColorDefectFailPercent;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label lbl_CenterPadColorDefectFailCount;
        private System.Windows.Forms.Panel pnl_SidePadEdgeDistance;
        private System.Windows.Forms.Label lbl_SidePadEdgeDistanceFailPercent;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label lbl_SidePadEdgeDistanceFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadEdgeDistance;
        private System.Windows.Forms.Label lbl_CenterPadEdgeDistanceFailPercent;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label lbl_CenterPadEdgeDistanceFailCount;
        private System.Windows.Forms.Panel pnl_SidePadSpan;
        private System.Windows.Forms.Label lbl_SidePadSpanFailPercent;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label lbl_SidePadSpanFailCount;
        private System.Windows.Forms.Panel pnl_CenterPadSpan;
        private System.Windows.Forms.Label lbl_CenterPadSpanFailPercent;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label lbl_CenterPadSpanFailCount;
    }
}