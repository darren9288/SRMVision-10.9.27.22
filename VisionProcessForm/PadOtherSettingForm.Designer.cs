namespace VisionProcessForm
{
    partial class PadOtherSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PadOtherSettingForm));
            this.tab_VisionControl = new SRMControl.SRMTabControl();
            this.tp_Segment = new System.Windows.Forms.TabPage();
            this.pnl_PadImageMerge2Threshold = new System.Windows.Forms.Panel();
            this.grp_ImageMerge2 = new System.Windows.Forms.GroupBox();
            this.lbl_PadImageMerge2Threshold_High = new System.Windows.Forms.Label();
            this.srmLabel72 = new SRMControl.SRMLabel();
            this.lbl_PadImageMerge2Threshold_Low = new System.Windows.Forms.Label();
            this.srmLabel71 = new SRMControl.SRMLabel();
            this.pnl_MinArea_PadImageMerge2Threshold = new System.Windows.Forms.Panel();
            this.txt_PadImageMerge2MinArea = new SRMControl.SRMInputBox();
            this.lbl_PadImageMerge2AreaTitle = new SRMControl.SRMLabel();
            this.lbl_PadImageMerge2PixTitle = new SRMControl.SRMLabel();
            this.lbl_PadImageMerge2ThresholdTitle = new SRMControl.SRMLabel();
            this.btn_PadImageMerge2Threshold = new SRMControl.SRMButton();
            this.pnl_MindArea = new System.Windows.Forms.Panel();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.txt_MinArea = new SRMControl.SRMInputBox();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.pnl_SurfaceThreshold = new System.Windows.Forms.Panel();
            this.lbl_LowSurfaceThreshold = new System.Windows.Forms.Label();
            this.lbl_HighSurfaceThreshold = new System.Windows.Forms.Label();
            this.btn_SurfaceThreshold = new SRMControl.SRMButton();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.pnl_PadThreshold = new System.Windows.Forms.Panel();
            this.btn_PadThreshold = new SRMControl.SRMButton();
            this.lbl_PinScoreTitle = new SRMControl.SRMLabel();
            this.lbl_PadThreshold = new System.Windows.Forms.Label();
            this.tp_PkgSegmentSimple = new System.Windows.Forms.TabPage();
            this.pnl_DarkDefect = new System.Windows.Forms.Panel();
            this.lbl_DarkThreshold = new System.Windows.Forms.Label();
            this.srmLabel59 = new SRMControl.SRMLabel();
            this.btn_DarkThreshold = new SRMControl.SRMButton();
            this.txt_DarkMinArea = new SRMControl.SRMInputBox();
            this.srmLabel56 = new SRMControl.SRMLabel();
            this.srmLabel57 = new SRMControl.SRMLabel();
            this.lbl_DarkDefect = new SRMControl.SRMLabel();
            this.pnl_BrightDefect = new System.Windows.Forms.Panel();
            this.lbl_BrightThreshold = new System.Windows.Forms.Label();
            this.srmLabel58 = new SRMControl.SRMLabel();
            this.btn_BrightThreshold = new SRMControl.SRMButton();
            this.txt_BrightMinArea = new SRMControl.SRMInputBox();
            this.srmLabel18 = new SRMControl.SRMLabel();
            this.srmLabel22 = new SRMControl.SRMLabel();
            this.lbl_BrightDefect = new SRMControl.SRMLabel();
            this.tp_PkgSegment = new System.Windows.Forms.TabPage();
            this.pnl_DarkCrack = new System.Windows.Forms.Panel();
            this.srmLabel34 = new SRMControl.SRMLabel();
            this.srmLabel35 = new SRMControl.SRMLabel();
            this.txt_CrackMinArea = new SRMControl.SRMInputBox();
            this.lbl_HighCrackViewThreshold = new System.Windows.Forms.Label();
            this.btn_CrackThreshold = new SRMControl.SRMButton();
            this.srmLabel32 = new SRMControl.SRMLabel();
            this.lbl_LowCrackViewThreshold = new System.Windows.Forms.Label();
            this.srmLabel31 = new SRMControl.SRMLabel();
            this.srmLabel33 = new SRMControl.SRMLabel();
            this.pnl_DarkVoid = new System.Windows.Forms.Panel();
            this.srmLabel30 = new SRMControl.SRMLabel();
            this.btn_Void_Threshold = new SRMControl.SRMButton();
            this.srmLabel28 = new SRMControl.SRMLabel();
            this.lbl_Void_Threshold = new System.Windows.Forms.Label();
            this.txt_VoidMinArea = new SRMControl.SRMInputBox();
            this.srmLabel29 = new SRMControl.SRMLabel();
            this.pnl_BrightMold = new System.Windows.Forms.Panel();
            this.srmLabel20 = new SRMControl.SRMLabel();
            this.btn_MoldFlashThreshold = new SRMControl.SRMButton();
            this.lbl_MoldFlashThreshold = new System.Windows.Forms.Label();
            this.srmLabel14 = new SRMControl.SRMLabel();
            this.srmLabel15 = new SRMControl.SRMLabel();
            this.txt_MoldFlashMinArea = new SRMControl.SRMInputBox();
            this.pnl_DarkChipped = new System.Windows.Forms.Panel();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.lbl_PkgImage2LowThreshold = new System.Windows.Forms.Label();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.lbl_PkgImage2HighThreshold = new System.Windows.Forms.Label();
            this.txt_Image2SurfaceMinArea = new SRMControl.SRMInputBox();
            this.btn_Image2Threshold = new SRMControl.SRMButton();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.pnl_BrightChipped = new System.Windows.Forms.Panel();
            this.srmLabel17 = new SRMControl.SRMLabel();
            this.srmLabel16 = new SRMControl.SRMLabel();
            this.lbl_PkgImage1LowThreshold = new System.Windows.Forms.Label();
            this.lbl_PkgImage1HighThreshold = new System.Windows.Forms.Label();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.btn_PackageSurfaceThreshold = new SRMControl.SRMButton();
            this.txt_SurfaceMinArea = new SRMControl.SRMInputBox();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.tp_PkgSegment2 = new System.Windows.Forms.TabPage();
            this.pnl_ForeignMaterialDefect = new System.Windows.Forms.Panel();
            this.lbl_ForeignMaterialThreshold = new System.Windows.Forms.Label();
            this.srmLabel67 = new SRMControl.SRMLabel();
            this.btn_ForeignMaterialThreshold = new SRMControl.SRMButton();
            this.txt_ForeignMaterialMinArea = new SRMControl.SRMInputBox();
            this.srmLabel68 = new SRMControl.SRMLabel();
            this.srmLabel69 = new SRMControl.SRMLabel();
            this.srmLabel70 = new SRMControl.SRMLabel();
            this.tp_other = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_PadInspectionAreaSetting = new SRMControl.SRMButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_PadROIToleranceSetting = new SRMControl.SRMButton();
            this.panel_LIneProfileSetting = new System.Windows.Forms.Panel();
            this.btn_LineProfileGaugeSetting = new SRMControl.SRMButton();
            this.panel_Other = new System.Windows.Forms.Panel();
            this.srmGroupBox16 = new SRMControl.SRMGroupBox();
            this.srmLabel53 = new SRMControl.SRMLabel();
            this.srmLabel52 = new SRMControl.SRMLabel();
            this.lbl_ThickIteration = new SRMControl.SRMLabel();
            this.txt_ThickIteration = new SRMControl.SRMInputBox();
            this.srmLabel54 = new SRMControl.SRMLabel();
            this.txt_ThinIteration = new SRMControl.SRMInputBox();
            this.tp_ROI = new System.Windows.Forms.TabPage();
            this.cbo_SelectROI = new SRMControl.SRMImageComboBox();
            this.grpbox_ForeignMaterialROI = new SRMControl.SRMGroupBox();
            this.pnl_ForeignMaterialBottom = new System.Windows.Forms.Panel();
            this.txt_ForeignMaterialStartPixelFromBottom = new SRMControl.SRMInputBox();
            this.pnl_ForeignMaterialRight = new System.Windows.Forms.Panel();
            this.txt_ForeignMaterialStartPixelFromRight = new SRMControl.SRMInputBox();
            this.pnl_ForeignMaterialTop = new System.Windows.Forms.Panel();
            this.txt_ForeignMaterialStartPixelFromEdge = new SRMControl.SRMInputBox();
            this.lbl_ForeignMaterialROILeft = new SRMControl.SRMLabel();
            this.srmLabel47 = new SRMControl.SRMLabel();
            this.lbl_ForeignMaterialROIBottom = new SRMControl.SRMLabel();
            this.pnl_ForeignMaterialLeft = new System.Windows.Forms.Panel();
            this.txt_ForeignMaterialStartPixelFromLeft = new SRMControl.SRMInputBox();
            this.srmLabel61 = new SRMControl.SRMLabel();
            this.lbl_ForeignMaterialROIRight = new SRMControl.SRMLabel();
            this.srmLabel64 = new SRMControl.SRMLabel();
            this.lbl_ForeignMaterialROITop = new SRMControl.SRMLabel();
            this.srmLabel66 = new SRMControl.SRMLabel();
            this.gbox_Pkg_Dark = new SRMControl.SRMGroupBox();
            this.pnl_PkgBottom_Dark = new System.Windows.Forms.Panel();
            this.txt_PkgStartPixelFromBottom_Dark = new SRMControl.SRMInputBox();
            this.pnl_PkgRight_Dark = new System.Windows.Forms.Panel();
            this.txt_PkgStartPixelFromRight_Dark = new SRMControl.SRMInputBox();
            this.pnl_PkgTop_Dark = new System.Windows.Forms.Panel();
            this.txt_PkgStartPixelFromEdge_Dark = new SRMControl.SRMInputBox();
            this.lbl_PkgLeft_Dark = new SRMControl.SRMLabel();
            this.srmLabel43 = new SRMControl.SRMLabel();
            this.lbl_PkgBottom_Dark = new SRMControl.SRMLabel();
            this.pnl_PkgLeft_Dark = new System.Windows.Forms.Panel();
            this.txt_PkgStartPixelFromLeft_Dark = new SRMControl.SRMInputBox();
            this.srmLabel51 = new SRMControl.SRMLabel();
            this.lbl_PkgRight_Dark = new SRMControl.SRMLabel();
            this.srmLabel60 = new SRMControl.SRMLabel();
            this.lbl_PkgTop_Dark = new SRMControl.SRMLabel();
            this.srmLabel62 = new SRMControl.SRMLabel();
            this.chk_SetToAllSideROI = new SRMControl.SRMCheckBox();
            this.chk_SetToAll = new SRMControl.SRMCheckBox();
            this.gbox_Pkg = new SRMControl.SRMGroupBox();
            this.pnl_PkgBottom = new System.Windows.Forms.Panel();
            this.txt_PkgStartPixelFromBottom = new SRMControl.SRMInputBox();
            this.pnl_PkgRight = new System.Windows.Forms.Panel();
            this.txt_PkgStartPixelFromRight = new SRMControl.SRMInputBox();
            this.pnl_PkgTop = new System.Windows.Forms.Panel();
            this.txt_PkgStartPixelFromEdge = new SRMControl.SRMInputBox();
            this.lbl_PkgLeft = new SRMControl.SRMLabel();
            this.srmLabel23 = new SRMControl.SRMLabel();
            this.lbl_PkgBottom = new SRMControl.SRMLabel();
            this.pnl_PkgLeft = new System.Windows.Forms.Panel();
            this.txt_PkgStartPixelFromLeft = new SRMControl.SRMInputBox();
            this.srmLabel19 = new SRMControl.SRMLabel();
            this.lbl_PkgRight = new SRMControl.SRMLabel();
            this.lbl_UnitDisplay1 = new SRMControl.SRMLabel();
            this.lbl_PkgTop = new SRMControl.SRMLabel();
            this.srmLabel26 = new SRMControl.SRMLabel();
            this.tp_ROI2 = new System.Windows.Forms.TabPage();
            this.gbox_Chip = new SRMControl.SRMGroupBox();
            this.pnl_ChippedTop = new System.Windows.Forms.Panel();
            this.txt_ChipStartPixelExtendFromEdge = new SRMControl.SRMInputBox();
            this.txt_ChipStartPixelFromEdge = new SRMControl.SRMInputBox();
            this.pnl_ChippedLeft = new System.Windows.Forms.Panel();
            this.txt_ChipStartPixelExtendFromLeft = new SRMControl.SRMInputBox();
            this.txt_ChipStartPixelFromLeft = new SRMControl.SRMInputBox();
            this.pnl_ChippedBottom = new System.Windows.Forms.Panel();
            this.txt_ChipStartPixelExtendFromBottom = new SRMControl.SRMInputBox();
            this.txt_ChipStartPixelFromBottom = new SRMControl.SRMInputBox();
            this.pnl_ChippedRight = new System.Windows.Forms.Panel();
            this.txt_ChipStartPixelExtendFromRight = new SRMControl.SRMInputBox();
            this.txt_ChipStartPixelFromRight = new SRMControl.SRMInputBox();
            this.srmLabel36 = new SRMControl.SRMLabel();
            this.srmLabel38 = new SRMControl.SRMLabel();
            this.lbl_ChipBottom = new SRMControl.SRMLabel();
            this.srmLabel40 = new SRMControl.SRMLabel();
            this.srmLabel42 = new SRMControl.SRMLabel();
            this.lbl_ChipRight = new SRMControl.SRMLabel();
            this.lbl_ChipTop = new SRMControl.SRMLabel();
            this.lbl_ChipLeft = new SRMControl.SRMLabel();
            this.srmLabel83 = new SRMControl.SRMLabel();
            this.srmLabel84 = new SRMControl.SRMLabel();
            this.gbox_Chip_Dark = new SRMControl.SRMGroupBox();
            this.srmLabel25 = new SRMControl.SRMLabel();
            this.srmLabel27 = new SRMControl.SRMLabel();
            this.pnl_ChippedTop_Dark = new System.Windows.Forms.Panel();
            this.txt_ChipStartPixelExtendFromEdge_Dark = new SRMControl.SRMInputBox();
            this.txt_ChipStartPixelFromEdge_Dark = new SRMControl.SRMInputBox();
            this.pnl_ChippedLeft_Dark = new System.Windows.Forms.Panel();
            this.txt_ChipStartPixelExtendFromLeft_Dark = new SRMControl.SRMInputBox();
            this.txt_ChipStartPixelFromLeft_Dark = new SRMControl.SRMInputBox();
            this.pnl_ChippedBottom_Dark = new System.Windows.Forms.Panel();
            this.txt_ChipStartPixelExtendFromBottom_Dark = new SRMControl.SRMInputBox();
            this.txt_ChipStartPixelFromBottom_Dark = new SRMControl.SRMInputBox();
            this.pnl_ChippedRight_Dark = new System.Windows.Forms.Panel();
            this.txt_ChipStartPixelExtendFromRight_Dark = new SRMControl.SRMInputBox();
            this.txt_ChipStartPixelFromRight_Dark = new SRMControl.SRMInputBox();
            this.srmLabel37 = new SRMControl.SRMLabel();
            this.lbl_ChipBottom_Dark = new SRMControl.SRMLabel();
            this.srmLabel41 = new SRMControl.SRMLabel();
            this.lbl_ChipRight_Dark = new SRMControl.SRMLabel();
            this.srmLabel45 = new SRMControl.SRMLabel();
            this.lbl_ChipTop_Dark = new SRMControl.SRMLabel();
            this.srmLabel49 = new SRMControl.SRMLabel();
            this.lbl_ChipLeft_Dark = new SRMControl.SRMLabel();
            this.gbox_Mold = new SRMControl.SRMGroupBox();
            this.pnl_MoldLeft = new System.Windows.Forms.Panel();
            this.txt_MoldStartPixelFromLeftInner = new SRMControl.SRMInputBox();
            this.txt_MoldStartPixelFromLeft = new SRMControl.SRMInputBox();
            this.pnl_MoldBottom = new System.Windows.Forms.Panel();
            this.txt_MoldStartPixelFromBottom = new SRMControl.SRMInputBox();
            this.txt_MoldStartPixelFromBottomInner = new SRMControl.SRMInputBox();
            this.pnl_MoldRight = new System.Windows.Forms.Panel();
            this.txt_MoldStartPixelFromRight = new SRMControl.SRMInputBox();
            this.txt_MoldStartPixelFromRightInner = new SRMControl.SRMInputBox();
            this.pnl_MoldTop = new System.Windows.Forms.Panel();
            this.txt_MoldStartPixelFromEdge = new SRMControl.SRMInputBox();
            this.txt_MoldStartPixelFromEdgeInner = new SRMControl.SRMInputBox();
            this.srmLabel44 = new SRMControl.SRMLabel();
            this.srmLabel46 = new SRMControl.SRMLabel();
            this.lbl_MoldBottom = new SRMControl.SRMLabel();
            this.srmLabel48 = new SRMControl.SRMLabel();
            this.lbl_MoldRight = new SRMControl.SRMLabel();
            this.srmLabel50 = new SRMControl.SRMLabel();
            this.lbl_MoldTop = new SRMControl.SRMLabel();
            this.lbl_MoldLeft = new SRMControl.SRMLabel();
            this.srmLabel21 = new SRMControl.SRMLabel();
            this.srmLabel24 = new SRMControl.SRMLabel();
            this.tp_ROI_Pad = new System.Windows.Forms.TabPage();
            this.grpbox_ForeignMaterialROI_Pad = new SRMControl.SRMGroupBox();
            this.pnl_ForeignMaterialBottom_Pad = new System.Windows.Forms.Panel();
            this.txt_ForeignMaterialStartPixelFromBottom_Pad = new SRMControl.SRMInputBox();
            this.pnl_ForeignMaterialRight_Pad = new System.Windows.Forms.Panel();
            this.txt_ForeignMaterialStartPixelFromRight_Pad = new SRMControl.SRMInputBox();
            this.pnl_ForeignMaterialTop_Pad = new System.Windows.Forms.Panel();
            this.txt_ForeignMaterialStartPixelFromEdge_Pad = new SRMControl.SRMInputBox();
            this.lbl_ForeignMaterialROILeft_Pad = new SRMControl.SRMLabel();
            this.srmLabel73 = new SRMControl.SRMLabel();
            this.lbl_ForeignMaterialROIBottom_Pad = new SRMControl.SRMLabel();
            this.pnl_ForeignMaterialLeft_Pad = new System.Windows.Forms.Panel();
            this.txt_ForeignMaterialStartPixelFromLeft_Pad = new SRMControl.SRMInputBox();
            this.srmLabel74 = new SRMControl.SRMLabel();
            this.lbl_ForeignMaterialROIRight_Pad = new SRMControl.SRMLabel();
            this.srmLabel76 = new SRMControl.SRMLabel();
            this.lbl_ForeignMaterialROITop_Pad = new SRMControl.SRMLabel();
            this.srmLabel78 = new SRMControl.SRMLabel();
            this.lbl_Title = new SRMControl.SRMLabel();
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.timer_Pad = new System.Windows.Forms.Timer(this.components);
            this.tab_VisionControl.SuspendLayout();
            this.tp_Segment.SuspendLayout();
            this.pnl_PadImageMerge2Threshold.SuspendLayout();
            this.grp_ImageMerge2.SuspendLayout();
            this.pnl_MinArea_PadImageMerge2Threshold.SuspendLayout();
            this.pnl_MindArea.SuspendLayout();
            this.pnl_SurfaceThreshold.SuspendLayout();
            this.pnl_PadThreshold.SuspendLayout();
            this.tp_PkgSegmentSimple.SuspendLayout();
            this.pnl_DarkDefect.SuspendLayout();
            this.pnl_BrightDefect.SuspendLayout();
            this.tp_PkgSegment.SuspendLayout();
            this.pnl_DarkCrack.SuspendLayout();
            this.pnl_DarkVoid.SuspendLayout();
            this.pnl_BrightMold.SuspendLayout();
            this.pnl_DarkChipped.SuspendLayout();
            this.pnl_BrightChipped.SuspendLayout();
            this.tp_PkgSegment2.SuspendLayout();
            this.pnl_ForeignMaterialDefect.SuspendLayout();
            this.tp_other.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel_LIneProfileSetting.SuspendLayout();
            this.panel_Other.SuspendLayout();
            this.srmGroupBox16.SuspendLayout();
            this.tp_ROI.SuspendLayout();
            this.grpbox_ForeignMaterialROI.SuspendLayout();
            this.pnl_ForeignMaterialBottom.SuspendLayout();
            this.pnl_ForeignMaterialRight.SuspendLayout();
            this.pnl_ForeignMaterialTop.SuspendLayout();
            this.pnl_ForeignMaterialLeft.SuspendLayout();
            this.gbox_Pkg_Dark.SuspendLayout();
            this.pnl_PkgBottom_Dark.SuspendLayout();
            this.pnl_PkgRight_Dark.SuspendLayout();
            this.pnl_PkgTop_Dark.SuspendLayout();
            this.pnl_PkgLeft_Dark.SuspendLayout();
            this.gbox_Pkg.SuspendLayout();
            this.pnl_PkgBottom.SuspendLayout();
            this.pnl_PkgRight.SuspendLayout();
            this.pnl_PkgTop.SuspendLayout();
            this.pnl_PkgLeft.SuspendLayout();
            this.tp_ROI2.SuspendLayout();
            this.gbox_Chip.SuspendLayout();
            this.pnl_ChippedTop.SuspendLayout();
            this.pnl_ChippedLeft.SuspendLayout();
            this.pnl_ChippedBottom.SuspendLayout();
            this.pnl_ChippedRight.SuspendLayout();
            this.gbox_Chip_Dark.SuspendLayout();
            this.pnl_ChippedTop_Dark.SuspendLayout();
            this.pnl_ChippedLeft_Dark.SuspendLayout();
            this.pnl_ChippedBottom_Dark.SuspendLayout();
            this.pnl_ChippedRight_Dark.SuspendLayout();
            this.gbox_Mold.SuspendLayout();
            this.pnl_MoldLeft.SuspendLayout();
            this.pnl_MoldBottom.SuspendLayout();
            this.pnl_MoldRight.SuspendLayout();
            this.pnl_MoldTop.SuspendLayout();
            this.tp_ROI_Pad.SuspendLayout();
            this.grpbox_ForeignMaterialROI_Pad.SuspendLayout();
            this.pnl_ForeignMaterialBottom_Pad.SuspendLayout();
            this.pnl_ForeignMaterialRight_Pad.SuspendLayout();
            this.pnl_ForeignMaterialTop_Pad.SuspendLayout();
            this.pnl_ForeignMaterialLeft_Pad.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab_VisionControl
            // 
            resources.ApplyResources(this.tab_VisionControl, "tab_VisionControl");
            this.tab_VisionControl.Controls.Add(this.tp_Segment);
            this.tab_VisionControl.Controls.Add(this.tp_PkgSegmentSimple);
            this.tab_VisionControl.Controls.Add(this.tp_PkgSegment);
            this.tab_VisionControl.Controls.Add(this.tp_PkgSegment2);
            this.tab_VisionControl.Controls.Add(this.tp_other);
            this.tab_VisionControl.Controls.Add(this.tp_ROI);
            this.tab_VisionControl.Controls.Add(this.tp_ROI2);
            this.tab_VisionControl.Controls.Add(this.tp_ROI_Pad);
            this.tab_VisionControl.Name = "tab_VisionControl";
            this.tab_VisionControl.SelectedIndex = 0;
            this.tab_VisionControl.SelectedIndexChanged += new System.EventHandler(this.tab_VisionControl_SelectedIndexChanged);
            // 
            // tp_Segment
            // 
            this.tp_Segment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Segment.Controls.Add(this.pnl_PadImageMerge2Threshold);
            this.tp_Segment.Controls.Add(this.pnl_MindArea);
            this.tp_Segment.Controls.Add(this.pnl_SurfaceThreshold);
            this.tp_Segment.Controls.Add(this.pnl_PadThreshold);
            resources.ApplyResources(this.tp_Segment, "tp_Segment");
            this.tp_Segment.Name = "tp_Segment";
            // 
            // pnl_PadImageMerge2Threshold
            // 
            this.pnl_PadImageMerge2Threshold.Controls.Add(this.grp_ImageMerge2);
            resources.ApplyResources(this.pnl_PadImageMerge2Threshold, "pnl_PadImageMerge2Threshold");
            this.pnl_PadImageMerge2Threshold.Name = "pnl_PadImageMerge2Threshold";
            // 
            // grp_ImageMerge2
            // 
            this.grp_ImageMerge2.Controls.Add(this.lbl_PadImageMerge2Threshold_High);
            this.grp_ImageMerge2.Controls.Add(this.srmLabel72);
            this.grp_ImageMerge2.Controls.Add(this.lbl_PadImageMerge2Threshold_Low);
            this.grp_ImageMerge2.Controls.Add(this.srmLabel71);
            this.grp_ImageMerge2.Controls.Add(this.pnl_MinArea_PadImageMerge2Threshold);
            this.grp_ImageMerge2.Controls.Add(this.lbl_PadImageMerge2ThresholdTitle);
            this.grp_ImageMerge2.Controls.Add(this.btn_PadImageMerge2Threshold);
            resources.ApplyResources(this.grp_ImageMerge2, "grp_ImageMerge2");
            this.grp_ImageMerge2.Name = "grp_ImageMerge2";
            this.grp_ImageMerge2.TabStop = false;
            // 
            // lbl_PadImageMerge2Threshold_High
            // 
            this.lbl_PadImageMerge2Threshold_High.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PadImageMerge2Threshold_High.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_PadImageMerge2Threshold_High, "lbl_PadImageMerge2Threshold_High");
            this.lbl_PadImageMerge2Threshold_High.Name = "lbl_PadImageMerge2Threshold_High";
            // 
            // srmLabel72
            // 
            resources.ApplyResources(this.srmLabel72, "srmLabel72");
            this.srmLabel72.Name = "srmLabel72";
            this.srmLabel72.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PadImageMerge2Threshold_Low
            // 
            this.lbl_PadImageMerge2Threshold_Low.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PadImageMerge2Threshold_Low.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_PadImageMerge2Threshold_Low, "lbl_PadImageMerge2Threshold_Low");
            this.lbl_PadImageMerge2Threshold_Low.Name = "lbl_PadImageMerge2Threshold_Low";
            // 
            // srmLabel71
            // 
            resources.ApplyResources(this.srmLabel71, "srmLabel71");
            this.srmLabel71.Name = "srmLabel71";
            this.srmLabel71.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_MinArea_PadImageMerge2Threshold
            // 
            this.pnl_MinArea_PadImageMerge2Threshold.Controls.Add(this.txt_PadImageMerge2MinArea);
            this.pnl_MinArea_PadImageMerge2Threshold.Controls.Add(this.lbl_PadImageMerge2AreaTitle);
            this.pnl_MinArea_PadImageMerge2Threshold.Controls.Add(this.lbl_PadImageMerge2PixTitle);
            resources.ApplyResources(this.pnl_MinArea_PadImageMerge2Threshold, "pnl_MinArea_PadImageMerge2Threshold");
            this.pnl_MinArea_PadImageMerge2Threshold.Name = "pnl_MinArea_PadImageMerge2Threshold";
            // 
            // txt_PadImageMerge2MinArea
            // 
            this.txt_PadImageMerge2MinArea.BackColor = System.Drawing.Color.White;
            this.txt_PadImageMerge2MinArea.DecimalPlaces = 0;
            this.txt_PadImageMerge2MinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_PadImageMerge2MinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PadImageMerge2MinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PadImageMerge2MinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_PadImageMerge2MinArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PadImageMerge2MinArea, "txt_PadImageMerge2MinArea");
            this.txt_PadImageMerge2MinArea.Name = "txt_PadImageMerge2MinArea";
            this.txt_PadImageMerge2MinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_PadImageMerge2MinArea.TextChanged += new System.EventHandler(this.txt_PadImageMerge2MinArea_TextChanged);
            // 
            // lbl_PadImageMerge2AreaTitle
            // 
            resources.ApplyResources(this.lbl_PadImageMerge2AreaTitle, "lbl_PadImageMerge2AreaTitle");
            this.lbl_PadImageMerge2AreaTitle.Name = "lbl_PadImageMerge2AreaTitle";
            this.lbl_PadImageMerge2AreaTitle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PadImageMerge2PixTitle
            // 
            resources.ApplyResources(this.lbl_PadImageMerge2PixTitle, "lbl_PadImageMerge2PixTitle");
            this.lbl_PadImageMerge2PixTitle.Name = "lbl_PadImageMerge2PixTitle";
            this.lbl_PadImageMerge2PixTitle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PadImageMerge2ThresholdTitle
            // 
            resources.ApplyResources(this.lbl_PadImageMerge2ThresholdTitle, "lbl_PadImageMerge2ThresholdTitle");
            this.lbl_PadImageMerge2ThresholdTitle.Name = "lbl_PadImageMerge2ThresholdTitle";
            this.lbl_PadImageMerge2ThresholdTitle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_PadImageMerge2Threshold
            // 
            resources.ApplyResources(this.btn_PadImageMerge2Threshold, "btn_PadImageMerge2Threshold");
            this.btn_PadImageMerge2Threshold.Name = "btn_PadImageMerge2Threshold";
            this.btn_PadImageMerge2Threshold.UseVisualStyleBackColor = true;
            this.btn_PadImageMerge2Threshold.Click += new System.EventHandler(this.btn_PadImageMerge2Threshold_Click);
            // 
            // pnl_MindArea
            // 
            this.pnl_MindArea.Controls.Add(this.srmLabel5);
            this.pnl_MindArea.Controls.Add(this.txt_MinArea);
            this.pnl_MindArea.Controls.Add(this.srmLabel4);
            resources.ApplyResources(this.pnl_MindArea, "pnl_MindArea");
            this.pnl_MindArea.Name = "pnl_MindArea";
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinArea
            // 
            this.txt_MinArea.BackColor = System.Drawing.Color.White;
            this.txt_MinArea.DecimalPlaces = 0;
            this.txt_MinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_MinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MinArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MinArea, "txt_MinArea");
            this.txt_MinArea.Name = "txt_MinArea";
            this.txt_MinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_MinArea.TextChanged += new System.EventHandler(this.txt_MinArea_TextChanged);
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_SurfaceThreshold
            // 
            this.pnl_SurfaceThreshold.Controls.Add(this.lbl_LowSurfaceThreshold);
            this.pnl_SurfaceThreshold.Controls.Add(this.lbl_HighSurfaceThreshold);
            this.pnl_SurfaceThreshold.Controls.Add(this.btn_SurfaceThreshold);
            this.pnl_SurfaceThreshold.Controls.Add(this.srmLabel2);
            this.pnl_SurfaceThreshold.Controls.Add(this.srmLabel3);
            this.pnl_SurfaceThreshold.Controls.Add(this.srmLabel1);
            resources.ApplyResources(this.pnl_SurfaceThreshold, "pnl_SurfaceThreshold");
            this.pnl_SurfaceThreshold.Name = "pnl_SurfaceThreshold";
            // 
            // lbl_LowSurfaceThreshold
            // 
            this.lbl_LowSurfaceThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_LowSurfaceThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_LowSurfaceThreshold, "lbl_LowSurfaceThreshold");
            this.lbl_LowSurfaceThreshold.Name = "lbl_LowSurfaceThreshold";
            // 
            // lbl_HighSurfaceThreshold
            // 
            this.lbl_HighSurfaceThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_HighSurfaceThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_HighSurfaceThreshold, "lbl_HighSurfaceThreshold");
            this.lbl_HighSurfaceThreshold.Name = "lbl_HighSurfaceThreshold";
            // 
            // btn_SurfaceThreshold
            // 
            resources.ApplyResources(this.btn_SurfaceThreshold, "btn_SurfaceThreshold");
            this.btn_SurfaceThreshold.Name = "btn_SurfaceThreshold";
            this.btn_SurfaceThreshold.UseVisualStyleBackColor = true;
            this.btn_SurfaceThreshold.Click += new System.EventHandler(this.btn_SurfaceThreshold_Click);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_PadThreshold
            // 
            this.pnl_PadThreshold.Controls.Add(this.btn_PadThreshold);
            this.pnl_PadThreshold.Controls.Add(this.lbl_PinScoreTitle);
            this.pnl_PadThreshold.Controls.Add(this.lbl_PadThreshold);
            resources.ApplyResources(this.pnl_PadThreshold, "pnl_PadThreshold");
            this.pnl_PadThreshold.Name = "pnl_PadThreshold";
            // 
            // btn_PadThreshold
            // 
            resources.ApplyResources(this.btn_PadThreshold, "btn_PadThreshold");
            this.btn_PadThreshold.Name = "btn_PadThreshold";
            this.btn_PadThreshold.UseVisualStyleBackColor = true;
            this.btn_PadThreshold.Click += new System.EventHandler(this.btn_PadThreshold_Click);
            // 
            // lbl_PinScoreTitle
            // 
            resources.ApplyResources(this.lbl_PinScoreTitle, "lbl_PinScoreTitle");
            this.lbl_PinScoreTitle.Name = "lbl_PinScoreTitle";
            this.lbl_PinScoreTitle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PadThreshold
            // 
            this.lbl_PadThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PadThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_PadThreshold, "lbl_PadThreshold");
            this.lbl_PadThreshold.Name = "lbl_PadThreshold";
            // 
            // tp_PkgSegmentSimple
            // 
            this.tp_PkgSegmentSimple.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_PkgSegmentSimple.Controls.Add(this.pnl_DarkDefect);
            this.tp_PkgSegmentSimple.Controls.Add(this.pnl_BrightDefect);
            resources.ApplyResources(this.tp_PkgSegmentSimple, "tp_PkgSegmentSimple");
            this.tp_PkgSegmentSimple.Name = "tp_PkgSegmentSimple";
            // 
            // pnl_DarkDefect
            // 
            this.pnl_DarkDefect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_DarkDefect.Controls.Add(this.lbl_DarkThreshold);
            this.pnl_DarkDefect.Controls.Add(this.srmLabel59);
            this.pnl_DarkDefect.Controls.Add(this.btn_DarkThreshold);
            this.pnl_DarkDefect.Controls.Add(this.txt_DarkMinArea);
            this.pnl_DarkDefect.Controls.Add(this.srmLabel56);
            this.pnl_DarkDefect.Controls.Add(this.srmLabel57);
            this.pnl_DarkDefect.Controls.Add(this.lbl_DarkDefect);
            resources.ApplyResources(this.pnl_DarkDefect, "pnl_DarkDefect");
            this.pnl_DarkDefect.Name = "pnl_DarkDefect";
            // 
            // lbl_DarkThreshold
            // 
            this.lbl_DarkThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_DarkThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_DarkThreshold, "lbl_DarkThreshold");
            this.lbl_DarkThreshold.Name = "lbl_DarkThreshold";
            // 
            // srmLabel59
            // 
            resources.ApplyResources(this.srmLabel59, "srmLabel59");
            this.srmLabel59.Name = "srmLabel59";
            this.srmLabel59.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_DarkThreshold
            // 
            resources.ApplyResources(this.btn_DarkThreshold, "btn_DarkThreshold");
            this.btn_DarkThreshold.Name = "btn_DarkThreshold";
            this.btn_DarkThreshold.UseVisualStyleBackColor = true;
            this.btn_DarkThreshold.Click += new System.EventHandler(this.btn_DarkThreshold_Click);
            // 
            // txt_DarkMinArea
            // 
            this.txt_DarkMinArea.BackColor = System.Drawing.Color.White;
            this.txt_DarkMinArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_DarkMinArea.DecimalPlaces = 0;
            this.txt_DarkMinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_DarkMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_DarkMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_DarkMinArea, "txt_DarkMinArea");
            this.txt_DarkMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_DarkMinArea.InputType = SRMControl.InputType.Number;
            this.txt_DarkMinArea.Name = "txt_DarkMinArea";
            this.txt_DarkMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_DarkMinArea.TextChanged += new System.EventHandler(this.txt_DarkMinArea_TextChanged);
            // 
            // srmLabel56
            // 
            resources.ApplyResources(this.srmLabel56, "srmLabel56");
            this.srmLabel56.Name = "srmLabel56";
            this.srmLabel56.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel57
            // 
            resources.ApplyResources(this.srmLabel57, "srmLabel57");
            this.srmLabel57.Name = "srmLabel57";
            this.srmLabel57.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_DarkDefect
            // 
            resources.ApplyResources(this.lbl_DarkDefect, "lbl_DarkDefect");
            this.lbl_DarkDefect.Name = "lbl_DarkDefect";
            this.lbl_DarkDefect.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_BrightDefect
            // 
            this.pnl_BrightDefect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_BrightDefect.Controls.Add(this.lbl_BrightThreshold);
            this.pnl_BrightDefect.Controls.Add(this.srmLabel58);
            this.pnl_BrightDefect.Controls.Add(this.btn_BrightThreshold);
            this.pnl_BrightDefect.Controls.Add(this.txt_BrightMinArea);
            this.pnl_BrightDefect.Controls.Add(this.srmLabel18);
            this.pnl_BrightDefect.Controls.Add(this.srmLabel22);
            this.pnl_BrightDefect.Controls.Add(this.lbl_BrightDefect);
            resources.ApplyResources(this.pnl_BrightDefect, "pnl_BrightDefect");
            this.pnl_BrightDefect.Name = "pnl_BrightDefect";
            // 
            // lbl_BrightThreshold
            // 
            this.lbl_BrightThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_BrightThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_BrightThreshold, "lbl_BrightThreshold");
            this.lbl_BrightThreshold.Name = "lbl_BrightThreshold";
            // 
            // srmLabel58
            // 
            resources.ApplyResources(this.srmLabel58, "srmLabel58");
            this.srmLabel58.Name = "srmLabel58";
            this.srmLabel58.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_BrightThreshold
            // 
            resources.ApplyResources(this.btn_BrightThreshold, "btn_BrightThreshold");
            this.btn_BrightThreshold.Name = "btn_BrightThreshold";
            this.btn_BrightThreshold.UseVisualStyleBackColor = true;
            this.btn_BrightThreshold.Click += new System.EventHandler(this.btn_BrightThreshold_Click);
            // 
            // txt_BrightMinArea
            // 
            this.txt_BrightMinArea.BackColor = System.Drawing.Color.White;
            this.txt_BrightMinArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_BrightMinArea.DecimalPlaces = 0;
            this.txt_BrightMinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_BrightMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_BrightMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_BrightMinArea, "txt_BrightMinArea");
            this.txt_BrightMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_BrightMinArea.InputType = SRMControl.InputType.Number;
            this.txt_BrightMinArea.Name = "txt_BrightMinArea";
            this.txt_BrightMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_BrightMinArea.TextChanged += new System.EventHandler(this.txt_BrightMinArea_TextChanged);
            // 
            // srmLabel18
            // 
            resources.ApplyResources(this.srmLabel18, "srmLabel18");
            this.srmLabel18.Name = "srmLabel18";
            this.srmLabel18.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel22
            // 
            resources.ApplyResources(this.srmLabel22, "srmLabel22");
            this.srmLabel22.Name = "srmLabel22";
            this.srmLabel22.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_BrightDefect
            // 
            resources.ApplyResources(this.lbl_BrightDefect, "lbl_BrightDefect");
            this.lbl_BrightDefect.Name = "lbl_BrightDefect";
            this.lbl_BrightDefect.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_PkgSegment
            // 
            this.tp_PkgSegment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_PkgSegment.Controls.Add(this.pnl_DarkCrack);
            this.tp_PkgSegment.Controls.Add(this.pnl_DarkVoid);
            this.tp_PkgSegment.Controls.Add(this.pnl_BrightMold);
            this.tp_PkgSegment.Controls.Add(this.pnl_DarkChipped);
            this.tp_PkgSegment.Controls.Add(this.pnl_BrightChipped);
            resources.ApplyResources(this.tp_PkgSegment, "tp_PkgSegment");
            this.tp_PkgSegment.Name = "tp_PkgSegment";
            // 
            // pnl_DarkCrack
            // 
            this.pnl_DarkCrack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_DarkCrack.Controls.Add(this.srmLabel34);
            this.pnl_DarkCrack.Controls.Add(this.srmLabel35);
            this.pnl_DarkCrack.Controls.Add(this.txt_CrackMinArea);
            this.pnl_DarkCrack.Controls.Add(this.lbl_HighCrackViewThreshold);
            this.pnl_DarkCrack.Controls.Add(this.btn_CrackThreshold);
            this.pnl_DarkCrack.Controls.Add(this.srmLabel32);
            this.pnl_DarkCrack.Controls.Add(this.lbl_LowCrackViewThreshold);
            this.pnl_DarkCrack.Controls.Add(this.srmLabel31);
            this.pnl_DarkCrack.Controls.Add(this.srmLabel33);
            resources.ApplyResources(this.pnl_DarkCrack, "pnl_DarkCrack");
            this.pnl_DarkCrack.Name = "pnl_DarkCrack";
            // 
            // srmLabel34
            // 
            resources.ApplyResources(this.srmLabel34, "srmLabel34");
            this.srmLabel34.Name = "srmLabel34";
            this.srmLabel34.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel35
            // 
            resources.ApplyResources(this.srmLabel35, "srmLabel35");
            this.srmLabel35.Name = "srmLabel35";
            this.srmLabel35.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_CrackMinArea
            // 
            this.txt_CrackMinArea.BackColor = System.Drawing.Color.White;
            this.txt_CrackMinArea.DecimalPlaces = 0;
            this.txt_CrackMinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_CrackMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_CrackMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_CrackMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_CrackMinArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_CrackMinArea, "txt_CrackMinArea");
            this.txt_CrackMinArea.Name = "txt_CrackMinArea";
            this.txt_CrackMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_CrackMinArea.TextChanged += new System.EventHandler(this.txt_CrackMinArea_TextChanged);
            // 
            // lbl_HighCrackViewThreshold
            // 
            this.lbl_HighCrackViewThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_HighCrackViewThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_HighCrackViewThreshold, "lbl_HighCrackViewThreshold");
            this.lbl_HighCrackViewThreshold.Name = "lbl_HighCrackViewThreshold";
            // 
            // btn_CrackThreshold
            // 
            resources.ApplyResources(this.btn_CrackThreshold, "btn_CrackThreshold");
            this.btn_CrackThreshold.Name = "btn_CrackThreshold";
            this.btn_CrackThreshold.UseVisualStyleBackColor = true;
            this.btn_CrackThreshold.Click += new System.EventHandler(this.btn_CrackThreshold_Click);
            // 
            // srmLabel32
            // 
            resources.ApplyResources(this.srmLabel32, "srmLabel32");
            this.srmLabel32.Name = "srmLabel32";
            this.srmLabel32.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_LowCrackViewThreshold
            // 
            this.lbl_LowCrackViewThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_LowCrackViewThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_LowCrackViewThreshold, "lbl_LowCrackViewThreshold");
            this.lbl_LowCrackViewThreshold.Name = "lbl_LowCrackViewThreshold";
            // 
            // srmLabel31
            // 
            resources.ApplyResources(this.srmLabel31, "srmLabel31");
            this.srmLabel31.Name = "srmLabel31";
            this.srmLabel31.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel33
            // 
            resources.ApplyResources(this.srmLabel33, "srmLabel33");
            this.srmLabel33.Name = "srmLabel33";
            this.srmLabel33.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_DarkVoid
            // 
            this.pnl_DarkVoid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_DarkVoid.Controls.Add(this.srmLabel30);
            this.pnl_DarkVoid.Controls.Add(this.btn_Void_Threshold);
            this.pnl_DarkVoid.Controls.Add(this.srmLabel28);
            this.pnl_DarkVoid.Controls.Add(this.lbl_Void_Threshold);
            this.pnl_DarkVoid.Controls.Add(this.txt_VoidMinArea);
            this.pnl_DarkVoid.Controls.Add(this.srmLabel29);
            resources.ApplyResources(this.pnl_DarkVoid, "pnl_DarkVoid");
            this.pnl_DarkVoid.Name = "pnl_DarkVoid";
            // 
            // srmLabel30
            // 
            resources.ApplyResources(this.srmLabel30, "srmLabel30");
            this.srmLabel30.Name = "srmLabel30";
            this.srmLabel30.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Void_Threshold
            // 
            resources.ApplyResources(this.btn_Void_Threshold, "btn_Void_Threshold");
            this.btn_Void_Threshold.Name = "btn_Void_Threshold";
            this.btn_Void_Threshold.UseVisualStyleBackColor = true;
            this.btn_Void_Threshold.Click += new System.EventHandler(this.btn_Void_Threshold_Click);
            // 
            // srmLabel28
            // 
            resources.ApplyResources(this.srmLabel28, "srmLabel28");
            this.srmLabel28.Name = "srmLabel28";
            this.srmLabel28.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Void_Threshold
            // 
            this.lbl_Void_Threshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_Void_Threshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Void_Threshold, "lbl_Void_Threshold");
            this.lbl_Void_Threshold.Name = "lbl_Void_Threshold";
            // 
            // txt_VoidMinArea
            // 
            this.txt_VoidMinArea.BackColor = System.Drawing.Color.White;
            this.txt_VoidMinArea.DecimalPlaces = 0;
            this.txt_VoidMinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_VoidMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_VoidMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_VoidMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_VoidMinArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_VoidMinArea, "txt_VoidMinArea");
            this.txt_VoidMinArea.Name = "txt_VoidMinArea";
            this.txt_VoidMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_VoidMinArea.TextChanged += new System.EventHandler(this.txt_VoidMinArea_TextChanged);
            // 
            // srmLabel29
            // 
            resources.ApplyResources(this.srmLabel29, "srmLabel29");
            this.srmLabel29.Name = "srmLabel29";
            this.srmLabel29.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_BrightMold
            // 
            this.pnl_BrightMold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_BrightMold.Controls.Add(this.srmLabel20);
            this.pnl_BrightMold.Controls.Add(this.btn_MoldFlashThreshold);
            this.pnl_BrightMold.Controls.Add(this.lbl_MoldFlashThreshold);
            this.pnl_BrightMold.Controls.Add(this.srmLabel14);
            this.pnl_BrightMold.Controls.Add(this.srmLabel15);
            this.pnl_BrightMold.Controls.Add(this.txt_MoldFlashMinArea);
            resources.ApplyResources(this.pnl_BrightMold, "pnl_BrightMold");
            this.pnl_BrightMold.Name = "pnl_BrightMold";
            // 
            // srmLabel20
            // 
            resources.ApplyResources(this.srmLabel20, "srmLabel20");
            this.srmLabel20.Name = "srmLabel20";
            this.srmLabel20.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_MoldFlashThreshold
            // 
            resources.ApplyResources(this.btn_MoldFlashThreshold, "btn_MoldFlashThreshold");
            this.btn_MoldFlashThreshold.Name = "btn_MoldFlashThreshold";
            this.btn_MoldFlashThreshold.UseVisualStyleBackColor = true;
            this.btn_MoldFlashThreshold.Click += new System.EventHandler(this.btn_MoldFlashThreshold_Click);
            // 
            // lbl_MoldFlashThreshold
            // 
            this.lbl_MoldFlashThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_MoldFlashThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_MoldFlashThreshold, "lbl_MoldFlashThreshold");
            this.lbl_MoldFlashThreshold.Name = "lbl_MoldFlashThreshold";
            // 
            // srmLabel14
            // 
            resources.ApplyResources(this.srmLabel14, "srmLabel14");
            this.srmLabel14.Name = "srmLabel14";
            this.srmLabel14.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel15
            // 
            resources.ApplyResources(this.srmLabel15, "srmLabel15");
            this.srmLabel15.Name = "srmLabel15";
            this.srmLabel15.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MoldFlashMinArea
            // 
            this.txt_MoldFlashMinArea.BackColor = System.Drawing.Color.White;
            this.txt_MoldFlashMinArea.DecimalPlaces = 0;
            this.txt_MoldFlashMinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_MoldFlashMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldFlashMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldFlashMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldFlashMinArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldFlashMinArea, "txt_MoldFlashMinArea");
            this.txt_MoldFlashMinArea.Name = "txt_MoldFlashMinArea";
            this.txt_MoldFlashMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldFlashMinArea.TextChanged += new System.EventHandler(this.txt_MoldFlashMinArea_TextChanged);
            // 
            // pnl_DarkChipped
            // 
            this.pnl_DarkChipped.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_DarkChipped.Controls.Add(this.srmLabel7);
            this.pnl_DarkChipped.Controls.Add(this.srmLabel6);
            this.pnl_DarkChipped.Controls.Add(this.lbl_PkgImage2LowThreshold);
            this.pnl_DarkChipped.Controls.Add(this.srmLabel11);
            this.pnl_DarkChipped.Controls.Add(this.lbl_PkgImage2HighThreshold);
            this.pnl_DarkChipped.Controls.Add(this.txt_Image2SurfaceMinArea);
            this.pnl_DarkChipped.Controls.Add(this.btn_Image2Threshold);
            this.pnl_DarkChipped.Controls.Add(this.srmLabel12);
            this.pnl_DarkChipped.Controls.Add(this.srmLabel8);
            resources.ApplyResources(this.pnl_DarkChipped, "pnl_DarkChipped");
            this.pnl_DarkChipped.Name = "pnl_DarkChipped";
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgImage2LowThreshold
            // 
            this.lbl_PkgImage2LowThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PkgImage2LowThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_PkgImage2LowThreshold, "lbl_PkgImage2LowThreshold");
            this.lbl_PkgImage2LowThreshold.Name = "lbl_PkgImage2LowThreshold";
            // 
            // srmLabel11
            // 
            resources.ApplyResources(this.srmLabel11, "srmLabel11");
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgImage2HighThreshold
            // 
            this.lbl_PkgImage2HighThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PkgImage2HighThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_PkgImage2HighThreshold, "lbl_PkgImage2HighThreshold");
            this.lbl_PkgImage2HighThreshold.Name = "lbl_PkgImage2HighThreshold";
            // 
            // txt_Image2SurfaceMinArea
            // 
            this.txt_Image2SurfaceMinArea.BackColor = System.Drawing.Color.White;
            this.txt_Image2SurfaceMinArea.DecimalPlaces = 0;
            this.txt_Image2SurfaceMinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_Image2SurfaceMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Image2SurfaceMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Image2SurfaceMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_Image2SurfaceMinArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Image2SurfaceMinArea, "txt_Image2SurfaceMinArea");
            this.txt_Image2SurfaceMinArea.Name = "txt_Image2SurfaceMinArea";
            this.txt_Image2SurfaceMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_Image2SurfaceMinArea.TextChanged += new System.EventHandler(this.txt_Image2SurfaceMinArea_TextChanged);
            // 
            // btn_Image2Threshold
            // 
            resources.ApplyResources(this.btn_Image2Threshold, "btn_Image2Threshold");
            this.btn_Image2Threshold.Name = "btn_Image2Threshold";
            this.btn_Image2Threshold.UseVisualStyleBackColor = true;
            this.btn_Image2Threshold.Click += new System.EventHandler(this.btn_Image2Threshold_Click);
            // 
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_BrightChipped
            // 
            this.pnl_BrightChipped.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_BrightChipped.Controls.Add(this.srmLabel17);
            this.pnl_BrightChipped.Controls.Add(this.srmLabel16);
            this.pnl_BrightChipped.Controls.Add(this.lbl_PkgImage1LowThreshold);
            this.pnl_BrightChipped.Controls.Add(this.lbl_PkgImage1HighThreshold);
            this.pnl_BrightChipped.Controls.Add(this.srmLabel9);
            this.pnl_BrightChipped.Controls.Add(this.btn_PackageSurfaceThreshold);
            this.pnl_BrightChipped.Controls.Add(this.txt_SurfaceMinArea);
            this.pnl_BrightChipped.Controls.Add(this.srmLabel10);
            this.pnl_BrightChipped.Controls.Add(this.srmLabel13);
            resources.ApplyResources(this.pnl_BrightChipped, "pnl_BrightChipped");
            this.pnl_BrightChipped.Name = "pnl_BrightChipped";
            // 
            // srmLabel17
            // 
            resources.ApplyResources(this.srmLabel17, "srmLabel17");
            this.srmLabel17.Name = "srmLabel17";
            this.srmLabel17.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel16
            // 
            resources.ApplyResources(this.srmLabel16, "srmLabel16");
            this.srmLabel16.Name = "srmLabel16";
            this.srmLabel16.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgImage1LowThreshold
            // 
            this.lbl_PkgImage1LowThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PkgImage1LowThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_PkgImage1LowThreshold, "lbl_PkgImage1LowThreshold");
            this.lbl_PkgImage1LowThreshold.Name = "lbl_PkgImage1LowThreshold";
            // 
            // lbl_PkgImage1HighThreshold
            // 
            this.lbl_PkgImage1HighThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PkgImage1HighThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_PkgImage1HighThreshold, "lbl_PkgImage1HighThreshold");
            this.lbl_PkgImage1HighThreshold.Name = "lbl_PkgImage1HighThreshold";
            // 
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_PackageSurfaceThreshold
            // 
            resources.ApplyResources(this.btn_PackageSurfaceThreshold, "btn_PackageSurfaceThreshold");
            this.btn_PackageSurfaceThreshold.Name = "btn_PackageSurfaceThreshold";
            this.btn_PackageSurfaceThreshold.UseVisualStyleBackColor = true;
            this.btn_PackageSurfaceThreshold.Click += new System.EventHandler(this.btn_PackageSurfaceThreshold_Click);
            // 
            // txt_SurfaceMinArea
            // 
            this.txt_SurfaceMinArea.BackColor = System.Drawing.Color.White;
            this.txt_SurfaceMinArea.DecimalPlaces = 0;
            this.txt_SurfaceMinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_SurfaceMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_SurfaceMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SurfaceMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_SurfaceMinArea.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_SurfaceMinArea, "txt_SurfaceMinArea");
            this.txt_SurfaceMinArea.Name = "txt_SurfaceMinArea";
            this.txt_SurfaceMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_SurfaceMinArea.TextChanged += new System.EventHandler(this.txt_SurfaceMinArea_TextChanged);
            // 
            // srmLabel10
            // 
            resources.ApplyResources(this.srmLabel10, "srmLabel10");
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_PkgSegment2
            // 
            this.tp_PkgSegment2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_PkgSegment2.Controls.Add(this.pnl_ForeignMaterialDefect);
            resources.ApplyResources(this.tp_PkgSegment2, "tp_PkgSegment2");
            this.tp_PkgSegment2.Name = "tp_PkgSegment2";
            // 
            // pnl_ForeignMaterialDefect
            // 
            this.pnl_ForeignMaterialDefect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_ForeignMaterialDefect.Controls.Add(this.lbl_ForeignMaterialThreshold);
            this.pnl_ForeignMaterialDefect.Controls.Add(this.srmLabel67);
            this.pnl_ForeignMaterialDefect.Controls.Add(this.btn_ForeignMaterialThreshold);
            this.pnl_ForeignMaterialDefect.Controls.Add(this.txt_ForeignMaterialMinArea);
            this.pnl_ForeignMaterialDefect.Controls.Add(this.srmLabel68);
            this.pnl_ForeignMaterialDefect.Controls.Add(this.srmLabel69);
            this.pnl_ForeignMaterialDefect.Controls.Add(this.srmLabel70);
            resources.ApplyResources(this.pnl_ForeignMaterialDefect, "pnl_ForeignMaterialDefect");
            this.pnl_ForeignMaterialDefect.Name = "pnl_ForeignMaterialDefect";
            // 
            // lbl_ForeignMaterialThreshold
            // 
            this.lbl_ForeignMaterialThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_ForeignMaterialThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_ForeignMaterialThreshold, "lbl_ForeignMaterialThreshold");
            this.lbl_ForeignMaterialThreshold.Name = "lbl_ForeignMaterialThreshold";
            // 
            // srmLabel67
            // 
            resources.ApplyResources(this.srmLabel67, "srmLabel67");
            this.srmLabel67.Name = "srmLabel67";
            this.srmLabel67.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_ForeignMaterialThreshold
            // 
            resources.ApplyResources(this.btn_ForeignMaterialThreshold, "btn_ForeignMaterialThreshold");
            this.btn_ForeignMaterialThreshold.Name = "btn_ForeignMaterialThreshold";
            this.btn_ForeignMaterialThreshold.UseVisualStyleBackColor = true;
            this.btn_ForeignMaterialThreshold.Click += new System.EventHandler(this.btn_ForeignMaterialThreshold_Click);
            // 
            // txt_ForeignMaterialMinArea
            // 
            this.txt_ForeignMaterialMinArea.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialMinArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ForeignMaterialMinArea.DecimalPlaces = 0;
            this.txt_ForeignMaterialMinArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_ForeignMaterialMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.txt_ForeignMaterialMinArea, "txt_ForeignMaterialMinArea");
            this.txt_ForeignMaterialMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialMinArea.InputType = SRMControl.InputType.Number;
            this.txt_ForeignMaterialMinArea.Name = "txt_ForeignMaterialMinArea";
            this.txt_ForeignMaterialMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialMinArea.TextChanged += new System.EventHandler(this.txt_ForeignMaterialMinArea_TextChanged);
            // 
            // srmLabel68
            // 
            resources.ApplyResources(this.srmLabel68, "srmLabel68");
            this.srmLabel68.Name = "srmLabel68";
            this.srmLabel68.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel69
            // 
            resources.ApplyResources(this.srmLabel69, "srmLabel69");
            this.srmLabel69.Name = "srmLabel69";
            this.srmLabel69.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel70
            // 
            resources.ApplyResources(this.srmLabel70, "srmLabel70");
            this.srmLabel70.Name = "srmLabel70";
            this.srmLabel70.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_other
            // 
            this.tp_other.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_other.Controls.Add(this.panel2);
            this.tp_other.Controls.Add(this.panel1);
            this.tp_other.Controls.Add(this.panel_LIneProfileSetting);
            this.tp_other.Controls.Add(this.panel_Other);
            resources.ApplyResources(this.tp_other, "tp_other");
            this.tp_other.Name = "tp_other";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_PadInspectionAreaSetting);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // btn_PadInspectionAreaSetting
            // 
            resources.ApplyResources(this.btn_PadInspectionAreaSetting, "btn_PadInspectionAreaSetting");
            this.btn_PadInspectionAreaSetting.Name = "btn_PadInspectionAreaSetting";
            this.btn_PadInspectionAreaSetting.UseVisualStyleBackColor = true;
            this.btn_PadInspectionAreaSetting.Click += new System.EventHandler(this.btn_PadInspectionAreaSetting_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_PadROIToleranceSetting);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btn_PadROIToleranceSetting
            // 
            resources.ApplyResources(this.btn_PadROIToleranceSetting, "btn_PadROIToleranceSetting");
            this.btn_PadROIToleranceSetting.Name = "btn_PadROIToleranceSetting";
            this.btn_PadROIToleranceSetting.UseVisualStyleBackColor = true;
            this.btn_PadROIToleranceSetting.Click += new System.EventHandler(this.btn_PadROIToleranceSetting_Click);
            // 
            // panel_LIneProfileSetting
            // 
            this.panel_LIneProfileSetting.Controls.Add(this.btn_LineProfileGaugeSetting);
            resources.ApplyResources(this.panel_LIneProfileSetting, "panel_LIneProfileSetting");
            this.panel_LIneProfileSetting.Name = "panel_LIneProfileSetting";
            // 
            // btn_LineProfileGaugeSetting
            // 
            resources.ApplyResources(this.btn_LineProfileGaugeSetting, "btn_LineProfileGaugeSetting");
            this.btn_LineProfileGaugeSetting.Name = "btn_LineProfileGaugeSetting";
            this.btn_LineProfileGaugeSetting.UseVisualStyleBackColor = true;
            this.btn_LineProfileGaugeSetting.Click += new System.EventHandler(this.btn_LineProfileGaugeSetting_Click);
            // 
            // panel_Other
            // 
            this.panel_Other.Controls.Add(this.srmGroupBox16);
            resources.ApplyResources(this.panel_Other, "panel_Other");
            this.panel_Other.Name = "panel_Other";
            this.panel_Other.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Other_Paint);
            // 
            // srmGroupBox16
            // 
            this.srmGroupBox16.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox16.Controls.Add(this.srmLabel53);
            this.srmGroupBox16.Controls.Add(this.srmLabel52);
            this.srmGroupBox16.Controls.Add(this.lbl_ThickIteration);
            this.srmGroupBox16.Controls.Add(this.txt_ThickIteration);
            this.srmGroupBox16.Controls.Add(this.srmLabel54);
            this.srmGroupBox16.Controls.Add(this.txt_ThinIteration);
            resources.ApplyResources(this.srmGroupBox16, "srmGroupBox16");
            this.srmGroupBox16.Name = "srmGroupBox16";
            this.srmGroupBox16.TabStop = false;
            this.srmGroupBox16.Enter += new System.EventHandler(this.srmGroupBox16_Enter);
            // 
            // srmLabel53
            // 
            this.srmLabel53.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.srmLabel53, "srmLabel53");
            this.srmLabel53.Name = "srmLabel53";
            this.srmLabel53.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel52
            // 
            resources.ApplyResources(this.srmLabel52, "srmLabel52");
            this.srmLabel52.Name = "srmLabel52";
            this.srmLabel52.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ThickIteration
            // 
            resources.ApplyResources(this.lbl_ThickIteration, "lbl_ThickIteration");
            this.lbl_ThickIteration.Name = "lbl_ThickIteration";
            this.lbl_ThickIteration.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_ThickIteration
            // 
            this.txt_ThickIteration.BackColor = System.Drawing.Color.White;
            this.txt_ThickIteration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ThickIteration.DecimalPlaces = 0;
            this.txt_ThickIteration.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ThickIteration.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ThickIteration.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ThickIteration.ForeColor = System.Drawing.Color.Black;
            this.txt_ThickIteration.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ThickIteration, "txt_ThickIteration");
            this.txt_ThickIteration.Name = "txt_ThickIteration";
            this.txt_ThickIteration.NormalBackColor = System.Drawing.Color.White;
            this.txt_ThickIteration.TextChanged += new System.EventHandler(this.txt_ThickIteration_TextChanged);
            // 
            // srmLabel54
            // 
            resources.ApplyResources(this.srmLabel54, "srmLabel54");
            this.srmLabel54.Name = "srmLabel54";
            this.srmLabel54.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_ThinIteration
            // 
            this.txt_ThinIteration.BackColor = System.Drawing.Color.White;
            this.txt_ThinIteration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ThinIteration.DecimalPlaces = 0;
            this.txt_ThinIteration.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ThinIteration.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ThinIteration.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ThinIteration.ForeColor = System.Drawing.Color.Black;
            this.txt_ThinIteration.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ThinIteration, "txt_ThinIteration");
            this.txt_ThinIteration.Name = "txt_ThinIteration";
            this.txt_ThinIteration.NormalBackColor = System.Drawing.Color.White;
            this.txt_ThinIteration.TextChanged += new System.EventHandler(this.txt_ThinIteration_TextChanged);
            // 
            // tp_ROI
            // 
            this.tp_ROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_ROI.Controls.Add(this.cbo_SelectROI);
            this.tp_ROI.Controls.Add(this.grpbox_ForeignMaterialROI);
            this.tp_ROI.Controls.Add(this.gbox_Pkg_Dark);
            this.tp_ROI.Controls.Add(this.chk_SetToAllSideROI);
            this.tp_ROI.Controls.Add(this.chk_SetToAll);
            this.tp_ROI.Controls.Add(this.gbox_Pkg);
            resources.ApplyResources(this.tp_ROI, "tp_ROI");
            this.tp_ROI.Name = "tp_ROI";
            // 
            // cbo_SelectROI
            // 
            this.cbo_SelectROI.BackColor = System.Drawing.Color.White;
            this.cbo_SelectROI.DisplayMember = "ItemData";
            this.cbo_SelectROI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_SelectROI.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_SelectROI.FormattingEnabled = true;
            this.cbo_SelectROI.Items.AddRange(new object[] {
            resources.GetString("cbo_SelectROI.Items"),
            resources.GetString("cbo_SelectROI.Items1"),
            resources.GetString("cbo_SelectROI.Items2"),
            resources.GetString("cbo_SelectROI.Items3"),
            resources.GetString("cbo_SelectROI.Items4")});
            resources.ApplyResources(this.cbo_SelectROI, "cbo_SelectROI");
            this.cbo_SelectROI.Name = "cbo_SelectROI";
            this.cbo_SelectROI.NormalBackColor = System.Drawing.Color.White;
            this.cbo_SelectROI.SelectedIndexChanged += new System.EventHandler(this.cbo_SelectROI_SelectedIndexChanged);
            // 
            // grpbox_ForeignMaterialROI
            // 
            this.grpbox_ForeignMaterialROI.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.grpbox_ForeignMaterialROI.Controls.Add(this.pnl_ForeignMaterialBottom);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.pnl_ForeignMaterialRight);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.pnl_ForeignMaterialTop);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.lbl_ForeignMaterialROILeft);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.srmLabel47);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.lbl_ForeignMaterialROIBottom);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.pnl_ForeignMaterialLeft);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.srmLabel61);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.lbl_ForeignMaterialROIRight);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.srmLabel64);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.lbl_ForeignMaterialROITop);
            this.grpbox_ForeignMaterialROI.Controls.Add(this.srmLabel66);
            resources.ApplyResources(this.grpbox_ForeignMaterialROI, "grpbox_ForeignMaterialROI");
            this.grpbox_ForeignMaterialROI.Name = "grpbox_ForeignMaterialROI";
            this.grpbox_ForeignMaterialROI.TabStop = false;
            // 
            // pnl_ForeignMaterialBottom
            // 
            this.pnl_ForeignMaterialBottom.Controls.Add(this.txt_ForeignMaterialStartPixelFromBottom);
            resources.ApplyResources(this.pnl_ForeignMaterialBottom, "pnl_ForeignMaterialBottom");
            this.pnl_ForeignMaterialBottom.Name = "pnl_ForeignMaterialBottom";
            // 
            // txt_ForeignMaterialStartPixelFromBottom
            // 
            this.txt_ForeignMaterialStartPixelFromBottom.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromBottom.DecimalPlaces = 0;
            this.txt_ForeignMaterialStartPixelFromBottom.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromBottom.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromBottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ForeignMaterialStartPixelFromBottom.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialStartPixelFromBottom.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ForeignMaterialStartPixelFromBottom, "txt_ForeignMaterialStartPixelFromBottom");
            this.txt_ForeignMaterialStartPixelFromBottom.Name = "txt_ForeignMaterialStartPixelFromBottom";
            this.txt_ForeignMaterialStartPixelFromBottom.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromBottom.TextChanged += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromBottom_TextChanged);
            this.txt_ForeignMaterialStartPixelFromBottom.Enter += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Enter);
            this.txt_ForeignMaterialStartPixelFromBottom.Leave += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Leave);
            // 
            // pnl_ForeignMaterialRight
            // 
            this.pnl_ForeignMaterialRight.Controls.Add(this.txt_ForeignMaterialStartPixelFromRight);
            resources.ApplyResources(this.pnl_ForeignMaterialRight, "pnl_ForeignMaterialRight");
            this.pnl_ForeignMaterialRight.Name = "pnl_ForeignMaterialRight";
            // 
            // txt_ForeignMaterialStartPixelFromRight
            // 
            this.txt_ForeignMaterialStartPixelFromRight.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromRight.DecimalPlaces = 0;
            this.txt_ForeignMaterialStartPixelFromRight.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromRight.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromRight.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ForeignMaterialStartPixelFromRight.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialStartPixelFromRight.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ForeignMaterialStartPixelFromRight, "txt_ForeignMaterialStartPixelFromRight");
            this.txt_ForeignMaterialStartPixelFromRight.Name = "txt_ForeignMaterialStartPixelFromRight";
            this.txt_ForeignMaterialStartPixelFromRight.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromRight.TextChanged += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromRight_TextChanged);
            this.txt_ForeignMaterialStartPixelFromRight.Enter += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Enter);
            this.txt_ForeignMaterialStartPixelFromRight.Leave += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Leave);
            // 
            // pnl_ForeignMaterialTop
            // 
            this.pnl_ForeignMaterialTop.Controls.Add(this.txt_ForeignMaterialStartPixelFromEdge);
            resources.ApplyResources(this.pnl_ForeignMaterialTop, "pnl_ForeignMaterialTop");
            this.pnl_ForeignMaterialTop.Name = "pnl_ForeignMaterialTop";
            // 
            // txt_ForeignMaterialStartPixelFromEdge
            // 
            this.txt_ForeignMaterialStartPixelFromEdge.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromEdge.DecimalPlaces = 0;
            this.txt_ForeignMaterialStartPixelFromEdge.DecMaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromEdge.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromEdge.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ForeignMaterialStartPixelFromEdge.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialStartPixelFromEdge.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ForeignMaterialStartPixelFromEdge, "txt_ForeignMaterialStartPixelFromEdge");
            this.txt_ForeignMaterialStartPixelFromEdge.Name = "txt_ForeignMaterialStartPixelFromEdge";
            this.txt_ForeignMaterialStartPixelFromEdge.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromEdge.TextChanged += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_TextChanged);
            this.txt_ForeignMaterialStartPixelFromEdge.Enter += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Enter);
            this.txt_ForeignMaterialStartPixelFromEdge.Leave += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Leave);
            // 
            // lbl_ForeignMaterialROILeft
            // 
            resources.ApplyResources(this.lbl_ForeignMaterialROILeft, "lbl_ForeignMaterialROILeft");
            this.lbl_ForeignMaterialROILeft.Name = "lbl_ForeignMaterialROILeft";
            this.lbl_ForeignMaterialROILeft.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel47
            // 
            resources.ApplyResources(this.srmLabel47, "srmLabel47");
            this.srmLabel47.Name = "srmLabel47";
            this.srmLabel47.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ForeignMaterialROIBottom
            // 
            resources.ApplyResources(this.lbl_ForeignMaterialROIBottom, "lbl_ForeignMaterialROIBottom");
            this.lbl_ForeignMaterialROIBottom.Name = "lbl_ForeignMaterialROIBottom";
            this.lbl_ForeignMaterialROIBottom.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_ForeignMaterialLeft
            // 
            this.pnl_ForeignMaterialLeft.Controls.Add(this.txt_ForeignMaterialStartPixelFromLeft);
            resources.ApplyResources(this.pnl_ForeignMaterialLeft, "pnl_ForeignMaterialLeft");
            this.pnl_ForeignMaterialLeft.Name = "pnl_ForeignMaterialLeft";
            // 
            // txt_ForeignMaterialStartPixelFromLeft
            // 
            this.txt_ForeignMaterialStartPixelFromLeft.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromLeft.DecimalPlaces = 0;
            this.txt_ForeignMaterialStartPixelFromLeft.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromLeft.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromLeft.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ForeignMaterialStartPixelFromLeft.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialStartPixelFromLeft.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ForeignMaterialStartPixelFromLeft, "txt_ForeignMaterialStartPixelFromLeft");
            this.txt_ForeignMaterialStartPixelFromLeft.Name = "txt_ForeignMaterialStartPixelFromLeft";
            this.txt_ForeignMaterialStartPixelFromLeft.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromLeft.TextChanged += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromLeft_TextChanged);
            this.txt_ForeignMaterialStartPixelFromLeft.Enter += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Enter);
            this.txt_ForeignMaterialStartPixelFromLeft.Leave += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Leave);
            // 
            // srmLabel61
            // 
            resources.ApplyResources(this.srmLabel61, "srmLabel61");
            this.srmLabel61.Name = "srmLabel61";
            this.srmLabel61.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ForeignMaterialROIRight
            // 
            resources.ApplyResources(this.lbl_ForeignMaterialROIRight, "lbl_ForeignMaterialROIRight");
            this.lbl_ForeignMaterialROIRight.Name = "lbl_ForeignMaterialROIRight";
            this.lbl_ForeignMaterialROIRight.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel64
            // 
            resources.ApplyResources(this.srmLabel64, "srmLabel64");
            this.srmLabel64.Name = "srmLabel64";
            this.srmLabel64.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ForeignMaterialROITop
            // 
            resources.ApplyResources(this.lbl_ForeignMaterialROITop, "lbl_ForeignMaterialROITop");
            this.lbl_ForeignMaterialROITop.Name = "lbl_ForeignMaterialROITop";
            this.lbl_ForeignMaterialROITop.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel66
            // 
            resources.ApplyResources(this.srmLabel66, "srmLabel66");
            this.srmLabel66.Name = "srmLabel66";
            this.srmLabel66.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // gbox_Pkg_Dark
            // 
            this.gbox_Pkg_Dark.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gbox_Pkg_Dark.Controls.Add(this.pnl_PkgBottom_Dark);
            this.gbox_Pkg_Dark.Controls.Add(this.pnl_PkgRight_Dark);
            this.gbox_Pkg_Dark.Controls.Add(this.pnl_PkgTop_Dark);
            this.gbox_Pkg_Dark.Controls.Add(this.lbl_PkgLeft_Dark);
            this.gbox_Pkg_Dark.Controls.Add(this.srmLabel43);
            this.gbox_Pkg_Dark.Controls.Add(this.lbl_PkgBottom_Dark);
            this.gbox_Pkg_Dark.Controls.Add(this.pnl_PkgLeft_Dark);
            this.gbox_Pkg_Dark.Controls.Add(this.srmLabel51);
            this.gbox_Pkg_Dark.Controls.Add(this.lbl_PkgRight_Dark);
            this.gbox_Pkg_Dark.Controls.Add(this.srmLabel60);
            this.gbox_Pkg_Dark.Controls.Add(this.lbl_PkgTop_Dark);
            this.gbox_Pkg_Dark.Controls.Add(this.srmLabel62);
            resources.ApplyResources(this.gbox_Pkg_Dark, "gbox_Pkg_Dark");
            this.gbox_Pkg_Dark.Name = "gbox_Pkg_Dark";
            this.gbox_Pkg_Dark.TabStop = false;
            // 
            // pnl_PkgBottom_Dark
            // 
            this.pnl_PkgBottom_Dark.Controls.Add(this.txt_PkgStartPixelFromBottom_Dark);
            resources.ApplyResources(this.pnl_PkgBottom_Dark, "pnl_PkgBottom_Dark");
            this.pnl_PkgBottom_Dark.Name = "pnl_PkgBottom_Dark";
            // 
            // txt_PkgStartPixelFromBottom_Dark
            // 
            this.txt_PkgStartPixelFromBottom_Dark.BackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromBottom_Dark.DecimalPlaces = 0;
            this.txt_PkgStartPixelFromBottom_Dark.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromBottom_Dark.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromBottom_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PkgStartPixelFromBottom_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_PkgStartPixelFromBottom_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PkgStartPixelFromBottom_Dark, "txt_PkgStartPixelFromBottom_Dark");
            this.txt_PkgStartPixelFromBottom_Dark.Name = "txt_PkgStartPixelFromBottom_Dark";
            this.txt_PkgStartPixelFromBottom_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromBottom_Dark.TextChanged += new System.EventHandler(this.txt_PkgStartPixelFromBottom_Dark_TextChanged);
            this.txt_PkgStartPixelFromBottom_Dark.Enter += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_Enter);
            this.txt_PkgStartPixelFromBottom_Dark.Leave += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_Leave);
            // 
            // pnl_PkgRight_Dark
            // 
            this.pnl_PkgRight_Dark.Controls.Add(this.txt_PkgStartPixelFromRight_Dark);
            resources.ApplyResources(this.pnl_PkgRight_Dark, "pnl_PkgRight_Dark");
            this.pnl_PkgRight_Dark.Name = "pnl_PkgRight_Dark";
            // 
            // txt_PkgStartPixelFromRight_Dark
            // 
            this.txt_PkgStartPixelFromRight_Dark.BackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromRight_Dark.DecimalPlaces = 0;
            this.txt_PkgStartPixelFromRight_Dark.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromRight_Dark.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromRight_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PkgStartPixelFromRight_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_PkgStartPixelFromRight_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PkgStartPixelFromRight_Dark, "txt_PkgStartPixelFromRight_Dark");
            this.txt_PkgStartPixelFromRight_Dark.Name = "txt_PkgStartPixelFromRight_Dark";
            this.txt_PkgStartPixelFromRight_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromRight_Dark.TextChanged += new System.EventHandler(this.txt_PkgStartPixelFromRight_Dark_TextChanged);
            this.txt_PkgStartPixelFromRight_Dark.Enter += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_Enter);
            this.txt_PkgStartPixelFromRight_Dark.Leave += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_Leave);
            // 
            // pnl_PkgTop_Dark
            // 
            this.pnl_PkgTop_Dark.Controls.Add(this.txt_PkgStartPixelFromEdge_Dark);
            resources.ApplyResources(this.pnl_PkgTop_Dark, "pnl_PkgTop_Dark");
            this.pnl_PkgTop_Dark.Name = "pnl_PkgTop_Dark";
            // 
            // txt_PkgStartPixelFromEdge_Dark
            // 
            this.txt_PkgStartPixelFromEdge_Dark.BackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromEdge_Dark.DecimalPlaces = 0;
            this.txt_PkgStartPixelFromEdge_Dark.DecMaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromEdge_Dark.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromEdge_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PkgStartPixelFromEdge_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_PkgStartPixelFromEdge_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PkgStartPixelFromEdge_Dark, "txt_PkgStartPixelFromEdge_Dark");
            this.txt_PkgStartPixelFromEdge_Dark.Name = "txt_PkgStartPixelFromEdge_Dark";
            this.txt_PkgStartPixelFromEdge_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromEdge_Dark.TextChanged += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_TextChanged);
            this.txt_PkgStartPixelFromEdge_Dark.Enter += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_Enter);
            this.txt_PkgStartPixelFromEdge_Dark.Leave += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_Leave);
            // 
            // lbl_PkgLeft_Dark
            // 
            resources.ApplyResources(this.lbl_PkgLeft_Dark, "lbl_PkgLeft_Dark");
            this.lbl_PkgLeft_Dark.Name = "lbl_PkgLeft_Dark";
            this.lbl_PkgLeft_Dark.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel43
            // 
            resources.ApplyResources(this.srmLabel43, "srmLabel43");
            this.srmLabel43.Name = "srmLabel43";
            this.srmLabel43.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgBottom_Dark
            // 
            resources.ApplyResources(this.lbl_PkgBottom_Dark, "lbl_PkgBottom_Dark");
            this.lbl_PkgBottom_Dark.Name = "lbl_PkgBottom_Dark";
            this.lbl_PkgBottom_Dark.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_PkgLeft_Dark
            // 
            this.pnl_PkgLeft_Dark.Controls.Add(this.txt_PkgStartPixelFromLeft_Dark);
            resources.ApplyResources(this.pnl_PkgLeft_Dark, "pnl_PkgLeft_Dark");
            this.pnl_PkgLeft_Dark.Name = "pnl_PkgLeft_Dark";
            // 
            // txt_PkgStartPixelFromLeft_Dark
            // 
            this.txt_PkgStartPixelFromLeft_Dark.BackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromLeft_Dark.DecimalPlaces = 0;
            this.txt_PkgStartPixelFromLeft_Dark.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromLeft_Dark.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromLeft_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PkgStartPixelFromLeft_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_PkgStartPixelFromLeft_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PkgStartPixelFromLeft_Dark, "txt_PkgStartPixelFromLeft_Dark");
            this.txt_PkgStartPixelFromLeft_Dark.Name = "txt_PkgStartPixelFromLeft_Dark";
            this.txt_PkgStartPixelFromLeft_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromLeft_Dark.TextChanged += new System.EventHandler(this.txt_PkgStartPixelFromLeft_Dark_TextChanged);
            this.txt_PkgStartPixelFromLeft_Dark.Enter += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_Enter);
            this.txt_PkgStartPixelFromLeft_Dark.Leave += new System.EventHandler(this.txt_PkgStartPixelFromEdge_Dark_Leave);
            // 
            // srmLabel51
            // 
            resources.ApplyResources(this.srmLabel51, "srmLabel51");
            this.srmLabel51.Name = "srmLabel51";
            this.srmLabel51.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgRight_Dark
            // 
            resources.ApplyResources(this.lbl_PkgRight_Dark, "lbl_PkgRight_Dark");
            this.lbl_PkgRight_Dark.Name = "lbl_PkgRight_Dark";
            this.lbl_PkgRight_Dark.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel60
            // 
            resources.ApplyResources(this.srmLabel60, "srmLabel60");
            this.srmLabel60.Name = "srmLabel60";
            this.srmLabel60.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgTop_Dark
            // 
            resources.ApplyResources(this.lbl_PkgTop_Dark, "lbl_PkgTop_Dark");
            this.lbl_PkgTop_Dark.Name = "lbl_PkgTop_Dark";
            this.lbl_PkgTop_Dark.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel62
            // 
            resources.ApplyResources(this.srmLabel62, "srmLabel62");
            this.srmLabel62.Name = "srmLabel62";
            this.srmLabel62.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_SetToAllSideROI
            // 
            this.chk_SetToAllSideROI.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_SetToAllSideROI, "chk_SetToAllSideROI");
            this.chk_SetToAllSideROI.Name = "chk_SetToAllSideROI";
            this.chk_SetToAllSideROI.Selected = false;
            this.chk_SetToAllSideROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllSideROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllSideROI.UseVisualStyleBackColor = true;
            this.chk_SetToAllSideROI.Click += new System.EventHandler(this.chk_SetToAllSideROI_Click);
            // 
            // chk_SetToAll
            // 
            this.chk_SetToAll.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_SetToAll, "chk_SetToAll");
            this.chk_SetToAll.Name = "chk_SetToAll";
            this.chk_SetToAll.Selected = false;
            this.chk_SetToAll.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAll.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAll.UseVisualStyleBackColor = true;
            this.chk_SetToAll.Click += new System.EventHandler(this.chk_SetToAll_Click);
            // 
            // gbox_Pkg
            // 
            this.gbox_Pkg.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gbox_Pkg.Controls.Add(this.pnl_PkgBottom);
            this.gbox_Pkg.Controls.Add(this.pnl_PkgRight);
            this.gbox_Pkg.Controls.Add(this.pnl_PkgTop);
            this.gbox_Pkg.Controls.Add(this.lbl_PkgLeft);
            this.gbox_Pkg.Controls.Add(this.srmLabel23);
            this.gbox_Pkg.Controls.Add(this.lbl_PkgBottom);
            this.gbox_Pkg.Controls.Add(this.pnl_PkgLeft);
            this.gbox_Pkg.Controls.Add(this.srmLabel19);
            this.gbox_Pkg.Controls.Add(this.lbl_PkgRight);
            this.gbox_Pkg.Controls.Add(this.lbl_UnitDisplay1);
            this.gbox_Pkg.Controls.Add(this.lbl_PkgTop);
            this.gbox_Pkg.Controls.Add(this.srmLabel26);
            resources.ApplyResources(this.gbox_Pkg, "gbox_Pkg");
            this.gbox_Pkg.Name = "gbox_Pkg";
            this.gbox_Pkg.TabStop = false;
            // 
            // pnl_PkgBottom
            // 
            this.pnl_PkgBottom.Controls.Add(this.txt_PkgStartPixelFromBottom);
            resources.ApplyResources(this.pnl_PkgBottom, "pnl_PkgBottom");
            this.pnl_PkgBottom.Name = "pnl_PkgBottom";
            // 
            // txt_PkgStartPixelFromBottom
            // 
            this.txt_PkgStartPixelFromBottom.BackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromBottom.DecimalPlaces = 0;
            this.txt_PkgStartPixelFromBottom.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromBottom.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromBottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PkgStartPixelFromBottom.ForeColor = System.Drawing.Color.Black;
            this.txt_PkgStartPixelFromBottom.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PkgStartPixelFromBottom, "txt_PkgStartPixelFromBottom");
            this.txt_PkgStartPixelFromBottom.Name = "txt_PkgStartPixelFromBottom";
            this.txt_PkgStartPixelFromBottom.NormalBackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromBottom.TextChanged += new System.EventHandler(this.txt_PkgStartPixelFromBottom_TextChanged);
            this.txt_PkgStartPixelFromBottom.Enter += new System.EventHandler(this.txt_PkgStartPointFromBottom_Enter);
            this.txt_PkgStartPixelFromBottom.Leave += new System.EventHandler(this.txt_PkgStartPointFromBottom_Leave);
            // 
            // pnl_PkgRight
            // 
            this.pnl_PkgRight.Controls.Add(this.txt_PkgStartPixelFromRight);
            resources.ApplyResources(this.pnl_PkgRight, "pnl_PkgRight");
            this.pnl_PkgRight.Name = "pnl_PkgRight";
            // 
            // txt_PkgStartPixelFromRight
            // 
            this.txt_PkgStartPixelFromRight.BackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromRight.DecimalPlaces = 0;
            this.txt_PkgStartPixelFromRight.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromRight.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromRight.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PkgStartPixelFromRight.ForeColor = System.Drawing.Color.Black;
            this.txt_PkgStartPixelFromRight.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PkgStartPixelFromRight, "txt_PkgStartPixelFromRight");
            this.txt_PkgStartPixelFromRight.Name = "txt_PkgStartPixelFromRight";
            this.txt_PkgStartPixelFromRight.NormalBackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromRight.TextChanged += new System.EventHandler(this.txt_PkgStartPixelFromRight_TextChanged);
            this.txt_PkgStartPixelFromRight.Enter += new System.EventHandler(this.txt_PkgStartPointFromRight_Enter);
            this.txt_PkgStartPixelFromRight.Leave += new System.EventHandler(this.txt_PkgStartPointFromRight_Leave);
            // 
            // pnl_PkgTop
            // 
            this.pnl_PkgTop.Controls.Add(this.txt_PkgStartPixelFromEdge);
            resources.ApplyResources(this.pnl_PkgTop, "pnl_PkgTop");
            this.pnl_PkgTop.Name = "pnl_PkgTop";
            // 
            // txt_PkgStartPixelFromEdge
            // 
            this.txt_PkgStartPixelFromEdge.BackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromEdge.DecimalPlaces = 0;
            this.txt_PkgStartPixelFromEdge.DecMaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromEdge.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromEdge.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PkgStartPixelFromEdge.ForeColor = System.Drawing.Color.Black;
            this.txt_PkgStartPixelFromEdge.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PkgStartPixelFromEdge, "txt_PkgStartPixelFromEdge");
            this.txt_PkgStartPixelFromEdge.Name = "txt_PkgStartPixelFromEdge";
            this.txt_PkgStartPixelFromEdge.NormalBackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromEdge.TextChanged += new System.EventHandler(this.txt_PkgStartPointFromEdge_TextChanged);
            this.txt_PkgStartPixelFromEdge.Enter += new System.EventHandler(this.txt_PkgStartPointFromEdge_Enter);
            this.txt_PkgStartPixelFromEdge.Leave += new System.EventHandler(this.txt_PkgStartPointFromEdge_Leave);
            // 
            // lbl_PkgLeft
            // 
            resources.ApplyResources(this.lbl_PkgLeft, "lbl_PkgLeft");
            this.lbl_PkgLeft.Name = "lbl_PkgLeft";
            this.lbl_PkgLeft.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel23
            // 
            resources.ApplyResources(this.srmLabel23, "srmLabel23");
            this.srmLabel23.Name = "srmLabel23";
            this.srmLabel23.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgBottom
            // 
            resources.ApplyResources(this.lbl_PkgBottom, "lbl_PkgBottom");
            this.lbl_PkgBottom.Name = "lbl_PkgBottom";
            this.lbl_PkgBottom.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_PkgLeft
            // 
            this.pnl_PkgLeft.Controls.Add(this.txt_PkgStartPixelFromLeft);
            resources.ApplyResources(this.pnl_PkgLeft, "pnl_PkgLeft");
            this.pnl_PkgLeft.Name = "pnl_PkgLeft";
            // 
            // txt_PkgStartPixelFromLeft
            // 
            this.txt_PkgStartPixelFromLeft.BackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromLeft.DecimalPlaces = 0;
            this.txt_PkgStartPixelFromLeft.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromLeft.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PkgStartPixelFromLeft.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PkgStartPixelFromLeft.ForeColor = System.Drawing.Color.Black;
            this.txt_PkgStartPixelFromLeft.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PkgStartPixelFromLeft, "txt_PkgStartPixelFromLeft");
            this.txt_PkgStartPixelFromLeft.Name = "txt_PkgStartPixelFromLeft";
            this.txt_PkgStartPixelFromLeft.NormalBackColor = System.Drawing.Color.White;
            this.txt_PkgStartPixelFromLeft.TextChanged += new System.EventHandler(this.txt_PkgStartPixelFromLeft_TextChanged);
            this.txt_PkgStartPixelFromLeft.Enter += new System.EventHandler(this.txt_PkgStartPointFromLeft_Enter);
            this.txt_PkgStartPixelFromLeft.Leave += new System.EventHandler(this.txt_PkgStartPointFromLeft_Leave);
            // 
            // srmLabel19
            // 
            resources.ApplyResources(this.srmLabel19, "srmLabel19");
            this.srmLabel19.Name = "srmLabel19";
            this.srmLabel19.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgRight
            // 
            resources.ApplyResources(this.lbl_PkgRight, "lbl_PkgRight");
            this.lbl_PkgRight.Name = "lbl_PkgRight";
            this.lbl_PkgRight.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_UnitDisplay1
            // 
            resources.ApplyResources(this.lbl_UnitDisplay1, "lbl_UnitDisplay1");
            this.lbl_UnitDisplay1.Name = "lbl_UnitDisplay1";
            this.lbl_UnitDisplay1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PkgTop
            // 
            resources.ApplyResources(this.lbl_PkgTop, "lbl_PkgTop");
            this.lbl_PkgTop.Name = "lbl_PkgTop";
            this.lbl_PkgTop.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel26
            // 
            resources.ApplyResources(this.srmLabel26, "srmLabel26");
            this.srmLabel26.Name = "srmLabel26";
            this.srmLabel26.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_ROI2
            // 
            this.tp_ROI2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_ROI2.Controls.Add(this.gbox_Chip);
            this.tp_ROI2.Controls.Add(this.gbox_Chip_Dark);
            this.tp_ROI2.Controls.Add(this.gbox_Mold);
            resources.ApplyResources(this.tp_ROI2, "tp_ROI2");
            this.tp_ROI2.Name = "tp_ROI2";
            // 
            // gbox_Chip
            // 
            this.gbox_Chip.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gbox_Chip.Controls.Add(this.pnl_ChippedTop);
            this.gbox_Chip.Controls.Add(this.pnl_ChippedLeft);
            this.gbox_Chip.Controls.Add(this.pnl_ChippedBottom);
            this.gbox_Chip.Controls.Add(this.pnl_ChippedRight);
            this.gbox_Chip.Controls.Add(this.srmLabel36);
            this.gbox_Chip.Controls.Add(this.srmLabel38);
            this.gbox_Chip.Controls.Add(this.lbl_ChipBottom);
            this.gbox_Chip.Controls.Add(this.srmLabel40);
            this.gbox_Chip.Controls.Add(this.srmLabel42);
            this.gbox_Chip.Controls.Add(this.lbl_ChipRight);
            this.gbox_Chip.Controls.Add(this.lbl_ChipTop);
            this.gbox_Chip.Controls.Add(this.lbl_ChipLeft);
            this.gbox_Chip.Controls.Add(this.srmLabel83);
            this.gbox_Chip.Controls.Add(this.srmLabel84);
            resources.ApplyResources(this.gbox_Chip, "gbox_Chip");
            this.gbox_Chip.Name = "gbox_Chip";
            this.gbox_Chip.TabStop = false;
            // 
            // pnl_ChippedTop
            // 
            this.pnl_ChippedTop.Controls.Add(this.txt_ChipStartPixelExtendFromEdge);
            this.pnl_ChippedTop.Controls.Add(this.txt_ChipStartPixelFromEdge);
            resources.ApplyResources(this.pnl_ChippedTop, "pnl_ChippedTop");
            this.pnl_ChippedTop.Name = "pnl_ChippedTop";
            // 
            // txt_ChipStartPixelExtendFromEdge
            // 
            this.txt_ChipStartPixelExtendFromEdge.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromEdge.DecimalPlaces = 0;
            this.txt_ChipStartPixelExtendFromEdge.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelExtendFromEdge.DecMinValue = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_ChipStartPixelExtendFromEdge.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelExtendFromEdge.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelExtendFromEdge.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelExtendFromEdge, "txt_ChipStartPixelExtendFromEdge");
            this.txt_ChipStartPixelExtendFromEdge.Name = "txt_ChipStartPixelExtendFromEdge";
            this.txt_ChipStartPixelExtendFromEdge.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromEdge.TextChanged += new System.EventHandler(this.txt_ChipStartPointExtendFromEdge_TextChanged);
            this.txt_ChipStartPixelExtendFromEdge.Enter += new System.EventHandler(this.txt_ChipStartPointFromEdge_Enter);
            this.txt_ChipStartPixelExtendFromEdge.Leave += new System.EventHandler(this.txt_ChipStartPointFromEdge_Leave);
            // 
            // txt_ChipStartPixelFromEdge
            // 
            this.txt_ChipStartPixelFromEdge.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromEdge.DecimalPlaces = 0;
            this.txt_ChipStartPixelFromEdge.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromEdge.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromEdge.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelFromEdge.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelFromEdge.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelFromEdge, "txt_ChipStartPixelFromEdge");
            this.txt_ChipStartPixelFromEdge.Name = "txt_ChipStartPixelFromEdge";
            this.txt_ChipStartPixelFromEdge.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromEdge.TextChanged += new System.EventHandler(this.txt_ChipStartPointFromEdge_TextChanged);
            this.txt_ChipStartPixelFromEdge.Enter += new System.EventHandler(this.txt_ChipStartPointFromEdge_Enter);
            this.txt_ChipStartPixelFromEdge.Leave += new System.EventHandler(this.txt_ChipStartPointFromEdge_Leave);
            // 
            // pnl_ChippedLeft
            // 
            this.pnl_ChippedLeft.Controls.Add(this.txt_ChipStartPixelExtendFromLeft);
            this.pnl_ChippedLeft.Controls.Add(this.txt_ChipStartPixelFromLeft);
            resources.ApplyResources(this.pnl_ChippedLeft, "pnl_ChippedLeft");
            this.pnl_ChippedLeft.Name = "pnl_ChippedLeft";
            // 
            // txt_ChipStartPixelExtendFromLeft
            // 
            this.txt_ChipStartPixelExtendFromLeft.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromLeft.DecimalPlaces = 0;
            this.txt_ChipStartPixelExtendFromLeft.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelExtendFromLeft.DecMinValue = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_ChipStartPixelExtendFromLeft.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelExtendFromLeft.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelExtendFromLeft.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelExtendFromLeft, "txt_ChipStartPixelExtendFromLeft");
            this.txt_ChipStartPixelExtendFromLeft.Name = "txt_ChipStartPixelExtendFromLeft";
            this.txt_ChipStartPixelExtendFromLeft.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromLeft.TextChanged += new System.EventHandler(this.txt_ChipStartPixelExtendFromLeft_TextChanged);
            this.txt_ChipStartPixelExtendFromLeft.Enter += new System.EventHandler(this.txt_ChipStartPointFromEdge_Enter);
            this.txt_ChipStartPixelExtendFromLeft.Leave += new System.EventHandler(this.txt_ChipStartPointFromEdge_Leave);
            // 
            // txt_ChipStartPixelFromLeft
            // 
            this.txt_ChipStartPixelFromLeft.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromLeft.DecimalPlaces = 0;
            this.txt_ChipStartPixelFromLeft.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromLeft.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromLeft.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelFromLeft.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelFromLeft.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelFromLeft, "txt_ChipStartPixelFromLeft");
            this.txt_ChipStartPixelFromLeft.Name = "txt_ChipStartPixelFromLeft";
            this.txt_ChipStartPixelFromLeft.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromLeft.TextChanged += new System.EventHandler(this.txt_ChipStartPixelFromLeft_TextChanged);
            this.txt_ChipStartPixelFromLeft.Enter += new System.EventHandler(this.txt_ChipStartPointFromLeft_Enter);
            this.txt_ChipStartPixelFromLeft.Leave += new System.EventHandler(this.txt_ChipStartPointFromLeft_Leave);
            // 
            // pnl_ChippedBottom
            // 
            this.pnl_ChippedBottom.Controls.Add(this.txt_ChipStartPixelExtendFromBottom);
            this.pnl_ChippedBottom.Controls.Add(this.txt_ChipStartPixelFromBottom);
            resources.ApplyResources(this.pnl_ChippedBottom, "pnl_ChippedBottom");
            this.pnl_ChippedBottom.Name = "pnl_ChippedBottom";
            // 
            // txt_ChipStartPixelExtendFromBottom
            // 
            this.txt_ChipStartPixelExtendFromBottom.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromBottom.DecimalPlaces = 0;
            this.txt_ChipStartPixelExtendFromBottom.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelExtendFromBottom.DecMinValue = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_ChipStartPixelExtendFromBottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelExtendFromBottom.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelExtendFromBottom.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelExtendFromBottom, "txt_ChipStartPixelExtendFromBottom");
            this.txt_ChipStartPixelExtendFromBottom.Name = "txt_ChipStartPixelExtendFromBottom";
            this.txt_ChipStartPixelExtendFromBottom.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromBottom.TextChanged += new System.EventHandler(this.txt_ChipStartPixelExtendFromBottom_TextChanged);
            this.txt_ChipStartPixelExtendFromBottom.Enter += new System.EventHandler(this.txt_ChipStartPointFromEdge_Enter);
            this.txt_ChipStartPixelExtendFromBottom.Leave += new System.EventHandler(this.txt_ChipStartPointFromEdge_Leave);
            // 
            // txt_ChipStartPixelFromBottom
            // 
            this.txt_ChipStartPixelFromBottom.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromBottom.DecimalPlaces = 0;
            this.txt_ChipStartPixelFromBottom.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromBottom.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromBottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelFromBottom.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelFromBottom.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelFromBottom, "txt_ChipStartPixelFromBottom");
            this.txt_ChipStartPixelFromBottom.Name = "txt_ChipStartPixelFromBottom";
            this.txt_ChipStartPixelFromBottom.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromBottom.TextChanged += new System.EventHandler(this.txt_ChipStartPixelFromBottom_TextChanged);
            this.txt_ChipStartPixelFromBottom.Enter += new System.EventHandler(this.txt_ChipStartPointFromBottom_Enter);
            this.txt_ChipStartPixelFromBottom.Leave += new System.EventHandler(this.txt_ChipStartPointFromBottom_Leave);
            // 
            // pnl_ChippedRight
            // 
            this.pnl_ChippedRight.Controls.Add(this.txt_ChipStartPixelExtendFromRight);
            this.pnl_ChippedRight.Controls.Add(this.txt_ChipStartPixelFromRight);
            resources.ApplyResources(this.pnl_ChippedRight, "pnl_ChippedRight");
            this.pnl_ChippedRight.Name = "pnl_ChippedRight";
            // 
            // txt_ChipStartPixelExtendFromRight
            // 
            this.txt_ChipStartPixelExtendFromRight.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromRight.DecimalPlaces = 0;
            this.txt_ChipStartPixelExtendFromRight.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelExtendFromRight.DecMinValue = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_ChipStartPixelExtendFromRight.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelExtendFromRight.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelExtendFromRight.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelExtendFromRight, "txt_ChipStartPixelExtendFromRight");
            this.txt_ChipStartPixelExtendFromRight.Name = "txt_ChipStartPixelExtendFromRight";
            this.txt_ChipStartPixelExtendFromRight.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromRight.TextChanged += new System.EventHandler(this.txt_ChipStartPixelExtendFromRight_TextChanged);
            this.txt_ChipStartPixelExtendFromRight.Enter += new System.EventHandler(this.txt_ChipStartPointFromEdge_Enter);
            this.txt_ChipStartPixelExtendFromRight.Leave += new System.EventHandler(this.txt_ChipStartPointFromEdge_Leave);
            // 
            // txt_ChipStartPixelFromRight
            // 
            this.txt_ChipStartPixelFromRight.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromRight.DecimalPlaces = 0;
            this.txt_ChipStartPixelFromRight.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromRight.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromRight.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelFromRight.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelFromRight.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelFromRight, "txt_ChipStartPixelFromRight");
            this.txt_ChipStartPixelFromRight.Name = "txt_ChipStartPixelFromRight";
            this.txt_ChipStartPixelFromRight.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromRight.TextChanged += new System.EventHandler(this.txt_ChipStartPixelFromRight_TextChanged);
            this.txt_ChipStartPixelFromRight.Enter += new System.EventHandler(this.txt_ChipStartPointFromRight_Enter);
            this.txt_ChipStartPixelFromRight.Leave += new System.EventHandler(this.txt_ChipStartPointFromRight_Leave);
            // 
            // srmLabel36
            // 
            resources.ApplyResources(this.srmLabel36, "srmLabel36");
            this.srmLabel36.Name = "srmLabel36";
            this.srmLabel36.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel38
            // 
            resources.ApplyResources(this.srmLabel38, "srmLabel38");
            this.srmLabel38.Name = "srmLabel38";
            this.srmLabel38.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ChipBottom
            // 
            resources.ApplyResources(this.lbl_ChipBottom, "lbl_ChipBottom");
            this.lbl_ChipBottom.Name = "lbl_ChipBottom";
            this.lbl_ChipBottom.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel40
            // 
            resources.ApplyResources(this.srmLabel40, "srmLabel40");
            this.srmLabel40.Name = "srmLabel40";
            this.srmLabel40.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel42
            // 
            resources.ApplyResources(this.srmLabel42, "srmLabel42");
            this.srmLabel42.Name = "srmLabel42";
            this.srmLabel42.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ChipRight
            // 
            resources.ApplyResources(this.lbl_ChipRight, "lbl_ChipRight");
            this.lbl_ChipRight.Name = "lbl_ChipRight";
            this.lbl_ChipRight.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ChipTop
            // 
            resources.ApplyResources(this.lbl_ChipTop, "lbl_ChipTop");
            this.lbl_ChipTop.Name = "lbl_ChipTop";
            this.lbl_ChipTop.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ChipLeft
            // 
            resources.ApplyResources(this.lbl_ChipLeft, "lbl_ChipLeft");
            this.lbl_ChipLeft.Name = "lbl_ChipLeft";
            this.lbl_ChipLeft.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel83
            // 
            resources.ApplyResources(this.srmLabel83, "srmLabel83");
            this.srmLabel83.Name = "srmLabel83";
            this.srmLabel83.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel84
            // 
            resources.ApplyResources(this.srmLabel84, "srmLabel84");
            this.srmLabel84.Name = "srmLabel84";
            this.srmLabel84.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // gbox_Chip_Dark
            // 
            this.gbox_Chip_Dark.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gbox_Chip_Dark.Controls.Add(this.srmLabel25);
            this.gbox_Chip_Dark.Controls.Add(this.srmLabel27);
            this.gbox_Chip_Dark.Controls.Add(this.pnl_ChippedTop_Dark);
            this.gbox_Chip_Dark.Controls.Add(this.pnl_ChippedLeft_Dark);
            this.gbox_Chip_Dark.Controls.Add(this.pnl_ChippedBottom_Dark);
            this.gbox_Chip_Dark.Controls.Add(this.pnl_ChippedRight_Dark);
            this.gbox_Chip_Dark.Controls.Add(this.srmLabel37);
            this.gbox_Chip_Dark.Controls.Add(this.lbl_ChipBottom_Dark);
            this.gbox_Chip_Dark.Controls.Add(this.srmLabel41);
            this.gbox_Chip_Dark.Controls.Add(this.lbl_ChipRight_Dark);
            this.gbox_Chip_Dark.Controls.Add(this.srmLabel45);
            this.gbox_Chip_Dark.Controls.Add(this.lbl_ChipTop_Dark);
            this.gbox_Chip_Dark.Controls.Add(this.srmLabel49);
            this.gbox_Chip_Dark.Controls.Add(this.lbl_ChipLeft_Dark);
            resources.ApplyResources(this.gbox_Chip_Dark, "gbox_Chip_Dark");
            this.gbox_Chip_Dark.Name = "gbox_Chip_Dark";
            this.gbox_Chip_Dark.TabStop = false;
            // 
            // srmLabel25
            // 
            resources.ApplyResources(this.srmLabel25, "srmLabel25");
            this.srmLabel25.Name = "srmLabel25";
            this.srmLabel25.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel27
            // 
            resources.ApplyResources(this.srmLabel27, "srmLabel27");
            this.srmLabel27.Name = "srmLabel27";
            this.srmLabel27.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_ChippedTop_Dark
            // 
            this.pnl_ChippedTop_Dark.Controls.Add(this.txt_ChipStartPixelExtendFromEdge_Dark);
            this.pnl_ChippedTop_Dark.Controls.Add(this.txt_ChipStartPixelFromEdge_Dark);
            resources.ApplyResources(this.pnl_ChippedTop_Dark, "pnl_ChippedTop_Dark");
            this.pnl_ChippedTop_Dark.Name = "pnl_ChippedTop_Dark";
            // 
            // txt_ChipStartPixelExtendFromEdge_Dark
            // 
            this.txt_ChipStartPixelExtendFromEdge_Dark.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromEdge_Dark.DecimalPlaces = 0;
            this.txt_ChipStartPixelExtendFromEdge_Dark.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelExtendFromEdge_Dark.DecMinValue = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_ChipStartPixelExtendFromEdge_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelExtendFromEdge_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelExtendFromEdge_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelExtendFromEdge_Dark, "txt_ChipStartPixelExtendFromEdge_Dark");
            this.txt_ChipStartPixelExtendFromEdge_Dark.Name = "txt_ChipStartPixelExtendFromEdge_Dark";
            this.txt_ChipStartPixelExtendFromEdge_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromEdge_Dark.TextChanged += new System.EventHandler(this.txt_ChipStartPointExtendFromEdge_Dark_TextChanged);
            this.txt_ChipStartPixelExtendFromEdge_Dark.Enter += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Enter);
            this.txt_ChipStartPixelExtendFromEdge_Dark.Leave += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Leave);
            // 
            // txt_ChipStartPixelFromEdge_Dark
            // 
            this.txt_ChipStartPixelFromEdge_Dark.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromEdge_Dark.DecimalPlaces = 0;
            this.txt_ChipStartPixelFromEdge_Dark.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromEdge_Dark.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromEdge_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelFromEdge_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelFromEdge_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelFromEdge_Dark, "txt_ChipStartPixelFromEdge_Dark");
            this.txt_ChipStartPixelFromEdge_Dark.Name = "txt_ChipStartPixelFromEdge_Dark";
            this.txt_ChipStartPixelFromEdge_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromEdge_Dark.TextChanged += new System.EventHandler(this.txt_ChipStartPointFromEdge_Dark_TextChanged);
            this.txt_ChipStartPixelFromEdge_Dark.Enter += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Enter);
            this.txt_ChipStartPixelFromEdge_Dark.Leave += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Leave);
            // 
            // pnl_ChippedLeft_Dark
            // 
            this.pnl_ChippedLeft_Dark.Controls.Add(this.txt_ChipStartPixelExtendFromLeft_Dark);
            this.pnl_ChippedLeft_Dark.Controls.Add(this.txt_ChipStartPixelFromLeft_Dark);
            resources.ApplyResources(this.pnl_ChippedLeft_Dark, "pnl_ChippedLeft_Dark");
            this.pnl_ChippedLeft_Dark.Name = "pnl_ChippedLeft_Dark";
            // 
            // txt_ChipStartPixelExtendFromLeft_Dark
            // 
            this.txt_ChipStartPixelExtendFromLeft_Dark.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromLeft_Dark.DecimalPlaces = 0;
            this.txt_ChipStartPixelExtendFromLeft_Dark.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelExtendFromLeft_Dark.DecMinValue = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_ChipStartPixelExtendFromLeft_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelExtendFromLeft_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelExtendFromLeft_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelExtendFromLeft_Dark, "txt_ChipStartPixelExtendFromLeft_Dark");
            this.txt_ChipStartPixelExtendFromLeft_Dark.Name = "txt_ChipStartPixelExtendFromLeft_Dark";
            this.txt_ChipStartPixelExtendFromLeft_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromLeft_Dark.TextChanged += new System.EventHandler(this.txt_ChipStartPixelExtendFromLeft_Dark_TextChanged);
            this.txt_ChipStartPixelExtendFromLeft_Dark.Enter += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Enter);
            this.txt_ChipStartPixelExtendFromLeft_Dark.Leave += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Leave);
            // 
            // txt_ChipStartPixelFromLeft_Dark
            // 
            this.txt_ChipStartPixelFromLeft_Dark.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromLeft_Dark.DecimalPlaces = 0;
            this.txt_ChipStartPixelFromLeft_Dark.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromLeft_Dark.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromLeft_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelFromLeft_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelFromLeft_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelFromLeft_Dark, "txt_ChipStartPixelFromLeft_Dark");
            this.txt_ChipStartPixelFromLeft_Dark.Name = "txt_ChipStartPixelFromLeft_Dark";
            this.txt_ChipStartPixelFromLeft_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromLeft_Dark.TextChanged += new System.EventHandler(this.txt_ChipStartPixelFromLeft_Dark_TextChanged);
            this.txt_ChipStartPixelFromLeft_Dark.Enter += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Enter);
            this.txt_ChipStartPixelFromLeft_Dark.Leave += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Leave);
            // 
            // pnl_ChippedBottom_Dark
            // 
            this.pnl_ChippedBottom_Dark.Controls.Add(this.txt_ChipStartPixelExtendFromBottom_Dark);
            this.pnl_ChippedBottom_Dark.Controls.Add(this.txt_ChipStartPixelFromBottom_Dark);
            resources.ApplyResources(this.pnl_ChippedBottom_Dark, "pnl_ChippedBottom_Dark");
            this.pnl_ChippedBottom_Dark.Name = "pnl_ChippedBottom_Dark";
            // 
            // txt_ChipStartPixelExtendFromBottom_Dark
            // 
            this.txt_ChipStartPixelExtendFromBottom_Dark.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromBottom_Dark.DecimalPlaces = 0;
            this.txt_ChipStartPixelExtendFromBottom_Dark.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelExtendFromBottom_Dark.DecMinValue = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_ChipStartPixelExtendFromBottom_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelExtendFromBottom_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelExtendFromBottom_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelExtendFromBottom_Dark, "txt_ChipStartPixelExtendFromBottom_Dark");
            this.txt_ChipStartPixelExtendFromBottom_Dark.Name = "txt_ChipStartPixelExtendFromBottom_Dark";
            this.txt_ChipStartPixelExtendFromBottom_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromBottom_Dark.TextChanged += new System.EventHandler(this.txt_ChipStartPixelExtendFromBottom_Dark_TextChanged);
            this.txt_ChipStartPixelExtendFromBottom_Dark.Enter += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Enter);
            this.txt_ChipStartPixelExtendFromBottom_Dark.Leave += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Leave);
            // 
            // txt_ChipStartPixelFromBottom_Dark
            // 
            this.txt_ChipStartPixelFromBottom_Dark.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromBottom_Dark.DecimalPlaces = 0;
            this.txt_ChipStartPixelFromBottom_Dark.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromBottom_Dark.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromBottom_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelFromBottom_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelFromBottom_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelFromBottom_Dark, "txt_ChipStartPixelFromBottom_Dark");
            this.txt_ChipStartPixelFromBottom_Dark.Name = "txt_ChipStartPixelFromBottom_Dark";
            this.txt_ChipStartPixelFromBottom_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromBottom_Dark.TextChanged += new System.EventHandler(this.txt_ChipStartPixelFromBottom_Dark_TextChanged);
            this.txt_ChipStartPixelFromBottom_Dark.Enter += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Enter);
            this.txt_ChipStartPixelFromBottom_Dark.Leave += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Leave);
            // 
            // pnl_ChippedRight_Dark
            // 
            this.pnl_ChippedRight_Dark.Controls.Add(this.txt_ChipStartPixelExtendFromRight_Dark);
            this.pnl_ChippedRight_Dark.Controls.Add(this.txt_ChipStartPixelFromRight_Dark);
            resources.ApplyResources(this.pnl_ChippedRight_Dark, "pnl_ChippedRight_Dark");
            this.pnl_ChippedRight_Dark.Name = "pnl_ChippedRight_Dark";
            // 
            // txt_ChipStartPixelExtendFromRight_Dark
            // 
            this.txt_ChipStartPixelExtendFromRight_Dark.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromRight_Dark.DecimalPlaces = 0;
            this.txt_ChipStartPixelExtendFromRight_Dark.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelExtendFromRight_Dark.DecMinValue = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_ChipStartPixelExtendFromRight_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelExtendFromRight_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelExtendFromRight_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelExtendFromRight_Dark, "txt_ChipStartPixelExtendFromRight_Dark");
            this.txt_ChipStartPixelExtendFromRight_Dark.Name = "txt_ChipStartPixelExtendFromRight_Dark";
            this.txt_ChipStartPixelExtendFromRight_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelExtendFromRight_Dark.TextChanged += new System.EventHandler(this.txt_ChipStartPixelExtendFromRight_Dark_TextChanged);
            this.txt_ChipStartPixelExtendFromRight_Dark.Enter += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Enter);
            this.txt_ChipStartPixelExtendFromRight_Dark.Leave += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Leave);
            // 
            // txt_ChipStartPixelFromRight_Dark
            // 
            this.txt_ChipStartPixelFromRight_Dark.BackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromRight_Dark.DecimalPlaces = 0;
            this.txt_ChipStartPixelFromRight_Dark.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromRight_Dark.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ChipStartPixelFromRight_Dark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ChipStartPixelFromRight_Dark.ForeColor = System.Drawing.Color.Black;
            this.txt_ChipStartPixelFromRight_Dark.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ChipStartPixelFromRight_Dark, "txt_ChipStartPixelFromRight_Dark");
            this.txt_ChipStartPixelFromRight_Dark.Name = "txt_ChipStartPixelFromRight_Dark";
            this.txt_ChipStartPixelFromRight_Dark.NormalBackColor = System.Drawing.Color.White;
            this.txt_ChipStartPixelFromRight_Dark.TextChanged += new System.EventHandler(this.txt_ChipStartPixelFromRight_Dark_TextChanged);
            this.txt_ChipStartPixelFromRight_Dark.Enter += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Enter);
            this.txt_ChipStartPixelFromRight_Dark.Leave += new System.EventHandler(this.txt_ChipStartPixelFromEdge_Dark_Leave);
            // 
            // srmLabel37
            // 
            resources.ApplyResources(this.srmLabel37, "srmLabel37");
            this.srmLabel37.Name = "srmLabel37";
            this.srmLabel37.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ChipBottom_Dark
            // 
            resources.ApplyResources(this.lbl_ChipBottom_Dark, "lbl_ChipBottom_Dark");
            this.lbl_ChipBottom_Dark.Name = "lbl_ChipBottom_Dark";
            this.lbl_ChipBottom_Dark.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel41
            // 
            resources.ApplyResources(this.srmLabel41, "srmLabel41");
            this.srmLabel41.Name = "srmLabel41";
            this.srmLabel41.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ChipRight_Dark
            // 
            resources.ApplyResources(this.lbl_ChipRight_Dark, "lbl_ChipRight_Dark");
            this.lbl_ChipRight_Dark.Name = "lbl_ChipRight_Dark";
            this.lbl_ChipRight_Dark.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel45
            // 
            resources.ApplyResources(this.srmLabel45, "srmLabel45");
            this.srmLabel45.Name = "srmLabel45";
            this.srmLabel45.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ChipTop_Dark
            // 
            resources.ApplyResources(this.lbl_ChipTop_Dark, "lbl_ChipTop_Dark");
            this.lbl_ChipTop_Dark.Name = "lbl_ChipTop_Dark";
            this.lbl_ChipTop_Dark.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel49
            // 
            resources.ApplyResources(this.srmLabel49, "srmLabel49");
            this.srmLabel49.Name = "srmLabel49";
            this.srmLabel49.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ChipLeft_Dark
            // 
            resources.ApplyResources(this.lbl_ChipLeft_Dark, "lbl_ChipLeft_Dark");
            this.lbl_ChipLeft_Dark.Name = "lbl_ChipLeft_Dark";
            this.lbl_ChipLeft_Dark.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // gbox_Mold
            // 
            this.gbox_Mold.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gbox_Mold.Controls.Add(this.pnl_MoldLeft);
            this.gbox_Mold.Controls.Add(this.pnl_MoldBottom);
            this.gbox_Mold.Controls.Add(this.pnl_MoldRight);
            this.gbox_Mold.Controls.Add(this.pnl_MoldTop);
            this.gbox_Mold.Controls.Add(this.srmLabel44);
            this.gbox_Mold.Controls.Add(this.srmLabel46);
            this.gbox_Mold.Controls.Add(this.lbl_MoldBottom);
            this.gbox_Mold.Controls.Add(this.srmLabel48);
            this.gbox_Mold.Controls.Add(this.lbl_MoldRight);
            this.gbox_Mold.Controls.Add(this.srmLabel50);
            this.gbox_Mold.Controls.Add(this.lbl_MoldTop);
            this.gbox_Mold.Controls.Add(this.lbl_MoldLeft);
            this.gbox_Mold.Controls.Add(this.srmLabel21);
            this.gbox_Mold.Controls.Add(this.srmLabel24);
            resources.ApplyResources(this.gbox_Mold, "gbox_Mold");
            this.gbox_Mold.Name = "gbox_Mold";
            this.gbox_Mold.TabStop = false;
            // 
            // pnl_MoldLeft
            // 
            this.pnl_MoldLeft.Controls.Add(this.txt_MoldStartPixelFromLeftInner);
            this.pnl_MoldLeft.Controls.Add(this.txt_MoldStartPixelFromLeft);
            resources.ApplyResources(this.pnl_MoldLeft, "pnl_MoldLeft");
            this.pnl_MoldLeft.Name = "pnl_MoldLeft";
            // 
            // txt_MoldStartPixelFromLeftInner
            // 
            this.txt_MoldStartPixelFromLeftInner.BackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromLeftInner.DecimalPlaces = 0;
            this.txt_MoldStartPixelFromLeftInner.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromLeftInner.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromLeftInner.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldStartPixelFromLeftInner.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldStartPixelFromLeftInner.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldStartPixelFromLeftInner, "txt_MoldStartPixelFromLeftInner");
            this.txt_MoldStartPixelFromLeftInner.Name = "txt_MoldStartPixelFromLeftInner";
            this.txt_MoldStartPixelFromLeftInner.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromLeftInner.TextChanged += new System.EventHandler(this.txt_MoldStartPixelFromLeftInner_TextChanged);
            this.txt_MoldStartPixelFromLeftInner.Enter += new System.EventHandler(this.txt_MoldStartPointFromEdge_Enter);
            this.txt_MoldStartPixelFromLeftInner.Leave += new System.EventHandler(this.txt_MoldStartPointFromEdge_Leave);
            // 
            // txt_MoldStartPixelFromLeft
            // 
            this.txt_MoldStartPixelFromLeft.BackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromLeft.DecimalPlaces = 0;
            this.txt_MoldStartPixelFromLeft.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromLeft.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromLeft.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldStartPixelFromLeft.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldStartPixelFromLeft.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldStartPixelFromLeft, "txt_MoldStartPixelFromLeft");
            this.txt_MoldStartPixelFromLeft.Name = "txt_MoldStartPixelFromLeft";
            this.txt_MoldStartPixelFromLeft.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromLeft.TextChanged += new System.EventHandler(this.txt_MoldStartPixelFromLeft_TextChanged);
            this.txt_MoldStartPixelFromLeft.Enter += new System.EventHandler(this.txt_MoldStartPointFromLeft_Enter);
            this.txt_MoldStartPixelFromLeft.Leave += new System.EventHandler(this.txt_MoldStartPointFromLeft_Leave);
            // 
            // pnl_MoldBottom
            // 
            this.pnl_MoldBottom.Controls.Add(this.txt_MoldStartPixelFromBottom);
            this.pnl_MoldBottom.Controls.Add(this.txt_MoldStartPixelFromBottomInner);
            resources.ApplyResources(this.pnl_MoldBottom, "pnl_MoldBottom");
            this.pnl_MoldBottom.Name = "pnl_MoldBottom";
            // 
            // txt_MoldStartPixelFromBottom
            // 
            this.txt_MoldStartPixelFromBottom.BackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromBottom.DecimalPlaces = 0;
            this.txt_MoldStartPixelFromBottom.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromBottom.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromBottom.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldStartPixelFromBottom.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldStartPixelFromBottom.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldStartPixelFromBottom, "txt_MoldStartPixelFromBottom");
            this.txt_MoldStartPixelFromBottom.Name = "txt_MoldStartPixelFromBottom";
            this.txt_MoldStartPixelFromBottom.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromBottom.TextChanged += new System.EventHandler(this.txt_MoldStartPixelFromBottom_TextChanged);
            this.txt_MoldStartPixelFromBottom.Enter += new System.EventHandler(this.txt_MoldStartPointFromBottom_Enter);
            this.txt_MoldStartPixelFromBottom.Leave += new System.EventHandler(this.txt_MoldStartPointFromBottom_Leave);
            // 
            // txt_MoldStartPixelFromBottomInner
            // 
            this.txt_MoldStartPixelFromBottomInner.BackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromBottomInner.DecimalPlaces = 0;
            this.txt_MoldStartPixelFromBottomInner.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromBottomInner.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromBottomInner.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldStartPixelFromBottomInner.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldStartPixelFromBottomInner.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldStartPixelFromBottomInner, "txt_MoldStartPixelFromBottomInner");
            this.txt_MoldStartPixelFromBottomInner.Name = "txt_MoldStartPixelFromBottomInner";
            this.txt_MoldStartPixelFromBottomInner.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromBottomInner.TextChanged += new System.EventHandler(this.txt_MoldStartPixelFromBottomInner_TextChanged);
            this.txt_MoldStartPixelFromBottomInner.Enter += new System.EventHandler(this.txt_MoldStartPointFromEdge_Enter);
            this.txt_MoldStartPixelFromBottomInner.Leave += new System.EventHandler(this.txt_MoldStartPointFromEdge_Leave);
            // 
            // pnl_MoldRight
            // 
            this.pnl_MoldRight.Controls.Add(this.txt_MoldStartPixelFromRight);
            this.pnl_MoldRight.Controls.Add(this.txt_MoldStartPixelFromRightInner);
            resources.ApplyResources(this.pnl_MoldRight, "pnl_MoldRight");
            this.pnl_MoldRight.Name = "pnl_MoldRight";
            // 
            // txt_MoldStartPixelFromRight
            // 
            this.txt_MoldStartPixelFromRight.BackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromRight.DecimalPlaces = 0;
            this.txt_MoldStartPixelFromRight.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromRight.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromRight.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldStartPixelFromRight.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldStartPixelFromRight.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldStartPixelFromRight, "txt_MoldStartPixelFromRight");
            this.txt_MoldStartPixelFromRight.Name = "txt_MoldStartPixelFromRight";
            this.txt_MoldStartPixelFromRight.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromRight.TextChanged += new System.EventHandler(this.txt_MoldStartPixelFromRight_TextChanged);
            this.txt_MoldStartPixelFromRight.Enter += new System.EventHandler(this.txt_MoldStartPointFromRight_Enter);
            this.txt_MoldStartPixelFromRight.Leave += new System.EventHandler(this.txt_MoldStartPointFromRight_Leave);
            // 
            // txt_MoldStartPixelFromRightInner
            // 
            this.txt_MoldStartPixelFromRightInner.BackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromRightInner.DecimalPlaces = 0;
            this.txt_MoldStartPixelFromRightInner.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromRightInner.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromRightInner.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldStartPixelFromRightInner.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldStartPixelFromRightInner.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldStartPixelFromRightInner, "txt_MoldStartPixelFromRightInner");
            this.txt_MoldStartPixelFromRightInner.Name = "txt_MoldStartPixelFromRightInner";
            this.txt_MoldStartPixelFromRightInner.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromRightInner.TextChanged += new System.EventHandler(this.txt_MoldStartPixelFromRightInner_TextChanged);
            this.txt_MoldStartPixelFromRightInner.Enter += new System.EventHandler(this.txt_MoldStartPointFromEdge_Enter);
            this.txt_MoldStartPixelFromRightInner.Leave += new System.EventHandler(this.txt_MoldStartPointFromEdge_Leave);
            // 
            // pnl_MoldTop
            // 
            this.pnl_MoldTop.Controls.Add(this.txt_MoldStartPixelFromEdge);
            this.pnl_MoldTop.Controls.Add(this.txt_MoldStartPixelFromEdgeInner);
            resources.ApplyResources(this.pnl_MoldTop, "pnl_MoldTop");
            this.pnl_MoldTop.Name = "pnl_MoldTop";
            // 
            // txt_MoldStartPixelFromEdge
            // 
            this.txt_MoldStartPixelFromEdge.BackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromEdge.DecimalPlaces = 0;
            this.txt_MoldStartPixelFromEdge.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromEdge.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromEdge.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldStartPixelFromEdge.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldStartPixelFromEdge.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldStartPixelFromEdge, "txt_MoldStartPixelFromEdge");
            this.txt_MoldStartPixelFromEdge.Name = "txt_MoldStartPixelFromEdge";
            this.txt_MoldStartPixelFromEdge.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromEdge.TextChanged += new System.EventHandler(this.txt_MoldStartPointFromEdge_TextChanged);
            this.txt_MoldStartPixelFromEdge.Enter += new System.EventHandler(this.txt_MoldStartPointFromEdge_Enter);
            this.txt_MoldStartPixelFromEdge.Leave += new System.EventHandler(this.txt_MoldStartPointFromEdge_Leave);
            // 
            // txt_MoldStartPixelFromEdgeInner
            // 
            this.txt_MoldStartPixelFromEdgeInner.BackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromEdgeInner.DecimalPlaces = 0;
            this.txt_MoldStartPixelFromEdgeInner.DecMaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromEdgeInner.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MoldStartPixelFromEdgeInner.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MoldStartPixelFromEdgeInner.ForeColor = System.Drawing.Color.Black;
            this.txt_MoldStartPixelFromEdgeInner.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MoldStartPixelFromEdgeInner, "txt_MoldStartPixelFromEdgeInner");
            this.txt_MoldStartPixelFromEdgeInner.Name = "txt_MoldStartPixelFromEdgeInner";
            this.txt_MoldStartPixelFromEdgeInner.NormalBackColor = System.Drawing.Color.White;
            this.txt_MoldStartPixelFromEdgeInner.TextChanged += new System.EventHandler(this.txt_MoldStartPixelFromEdgeInner_TextChanged);
            this.txt_MoldStartPixelFromEdgeInner.Enter += new System.EventHandler(this.txt_MoldStartPointFromEdge_Enter);
            this.txt_MoldStartPixelFromEdgeInner.Leave += new System.EventHandler(this.txt_MoldStartPointFromEdge_Leave);
            // 
            // srmLabel44
            // 
            resources.ApplyResources(this.srmLabel44, "srmLabel44");
            this.srmLabel44.Name = "srmLabel44";
            this.srmLabel44.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel46
            // 
            resources.ApplyResources(this.srmLabel46, "srmLabel46");
            this.srmLabel46.Name = "srmLabel46";
            this.srmLabel46.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_MoldBottom
            // 
            resources.ApplyResources(this.lbl_MoldBottom, "lbl_MoldBottom");
            this.lbl_MoldBottom.Name = "lbl_MoldBottom";
            this.lbl_MoldBottom.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel48
            // 
            resources.ApplyResources(this.srmLabel48, "srmLabel48");
            this.srmLabel48.Name = "srmLabel48";
            this.srmLabel48.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_MoldRight
            // 
            resources.ApplyResources(this.lbl_MoldRight, "lbl_MoldRight");
            this.lbl_MoldRight.Name = "lbl_MoldRight";
            this.lbl_MoldRight.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel50
            // 
            resources.ApplyResources(this.srmLabel50, "srmLabel50");
            this.srmLabel50.Name = "srmLabel50";
            this.srmLabel50.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_MoldTop
            // 
            resources.ApplyResources(this.lbl_MoldTop, "lbl_MoldTop");
            this.lbl_MoldTop.Name = "lbl_MoldTop";
            this.lbl_MoldTop.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_MoldLeft
            // 
            resources.ApplyResources(this.lbl_MoldLeft, "lbl_MoldLeft");
            this.lbl_MoldLeft.Name = "lbl_MoldLeft";
            this.lbl_MoldLeft.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel21
            // 
            resources.ApplyResources(this.srmLabel21, "srmLabel21");
            this.srmLabel21.Name = "srmLabel21";
            this.srmLabel21.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel24
            // 
            resources.ApplyResources(this.srmLabel24, "srmLabel24");
            this.srmLabel24.Name = "srmLabel24";
            this.srmLabel24.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_ROI_Pad
            // 
            this.tp_ROI_Pad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_ROI_Pad.Controls.Add(this.grpbox_ForeignMaterialROI_Pad);
            resources.ApplyResources(this.tp_ROI_Pad, "tp_ROI_Pad");
            this.tp_ROI_Pad.Name = "tp_ROI_Pad";
            // 
            // grpbox_ForeignMaterialROI_Pad
            // 
            this.grpbox_ForeignMaterialROI_Pad.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.pnl_ForeignMaterialBottom_Pad);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.pnl_ForeignMaterialRight_Pad);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.pnl_ForeignMaterialTop_Pad);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.lbl_ForeignMaterialROILeft_Pad);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.srmLabel73);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.lbl_ForeignMaterialROIBottom_Pad);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.pnl_ForeignMaterialLeft_Pad);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.srmLabel74);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.lbl_ForeignMaterialROIRight_Pad);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.srmLabel76);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.lbl_ForeignMaterialROITop_Pad);
            this.grpbox_ForeignMaterialROI_Pad.Controls.Add(this.srmLabel78);
            resources.ApplyResources(this.grpbox_ForeignMaterialROI_Pad, "grpbox_ForeignMaterialROI_Pad");
            this.grpbox_ForeignMaterialROI_Pad.Name = "grpbox_ForeignMaterialROI_Pad";
            this.grpbox_ForeignMaterialROI_Pad.TabStop = false;
            // 
            // pnl_ForeignMaterialBottom_Pad
            // 
            this.pnl_ForeignMaterialBottom_Pad.Controls.Add(this.txt_ForeignMaterialStartPixelFromBottom_Pad);
            resources.ApplyResources(this.pnl_ForeignMaterialBottom_Pad, "pnl_ForeignMaterialBottom_Pad");
            this.pnl_ForeignMaterialBottom_Pad.Name = "pnl_ForeignMaterialBottom_Pad";
            // 
            // txt_ForeignMaterialStartPixelFromBottom_Pad
            // 
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.DecimalPlaces = 0;
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ForeignMaterialStartPixelFromBottom_Pad, "txt_ForeignMaterialStartPixelFromBottom_Pad");
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.Name = "txt_ForeignMaterialStartPixelFromBottom_Pad";
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.TextChanged += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromBottom_Pad_TextChanged);
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.Enter += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_Enter);
            this.txt_ForeignMaterialStartPixelFromBottom_Pad.Leave += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_Leave);
            // 
            // pnl_ForeignMaterialRight_Pad
            // 
            this.pnl_ForeignMaterialRight_Pad.Controls.Add(this.txt_ForeignMaterialStartPixelFromRight_Pad);
            resources.ApplyResources(this.pnl_ForeignMaterialRight_Pad, "pnl_ForeignMaterialRight_Pad");
            this.pnl_ForeignMaterialRight_Pad.Name = "pnl_ForeignMaterialRight_Pad";
            // 
            // txt_ForeignMaterialStartPixelFromRight_Pad
            // 
            this.txt_ForeignMaterialStartPixelFromRight_Pad.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromRight_Pad.DecimalPlaces = 0;
            this.txt_ForeignMaterialStartPixelFromRight_Pad.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromRight_Pad.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromRight_Pad.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ForeignMaterialStartPixelFromRight_Pad.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialStartPixelFromRight_Pad.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ForeignMaterialStartPixelFromRight_Pad, "txt_ForeignMaterialStartPixelFromRight_Pad");
            this.txt_ForeignMaterialStartPixelFromRight_Pad.Name = "txt_ForeignMaterialStartPixelFromRight_Pad";
            this.txt_ForeignMaterialStartPixelFromRight_Pad.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromRight_Pad.TextChanged += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromRight_Pad_TextChanged);
            this.txt_ForeignMaterialStartPixelFromRight_Pad.Enter += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_Enter);
            this.txt_ForeignMaterialStartPixelFromRight_Pad.Leave += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_Leave);
            // 
            // pnl_ForeignMaterialTop_Pad
            // 
            this.pnl_ForeignMaterialTop_Pad.Controls.Add(this.txt_ForeignMaterialStartPixelFromEdge_Pad);
            resources.ApplyResources(this.pnl_ForeignMaterialTop_Pad, "pnl_ForeignMaterialTop_Pad");
            this.pnl_ForeignMaterialTop_Pad.Name = "pnl_ForeignMaterialTop_Pad";
            // 
            // txt_ForeignMaterialStartPixelFromEdge_Pad
            // 
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.DecimalPlaces = 0;
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.DecMaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ForeignMaterialStartPixelFromEdge_Pad, "txt_ForeignMaterialStartPixelFromEdge_Pad");
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.Name = "txt_ForeignMaterialStartPixelFromEdge_Pad";
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.TextChanged += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_TextChanged);
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.Enter += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_Enter);
            this.txt_ForeignMaterialStartPixelFromEdge_Pad.Leave += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_Leave);
            // 
            // lbl_ForeignMaterialROILeft_Pad
            // 
            resources.ApplyResources(this.lbl_ForeignMaterialROILeft_Pad, "lbl_ForeignMaterialROILeft_Pad");
            this.lbl_ForeignMaterialROILeft_Pad.Name = "lbl_ForeignMaterialROILeft_Pad";
            this.lbl_ForeignMaterialROILeft_Pad.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel73
            // 
            resources.ApplyResources(this.srmLabel73, "srmLabel73");
            this.srmLabel73.Name = "srmLabel73";
            this.srmLabel73.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ForeignMaterialROIBottom_Pad
            // 
            resources.ApplyResources(this.lbl_ForeignMaterialROIBottom_Pad, "lbl_ForeignMaterialROIBottom_Pad");
            this.lbl_ForeignMaterialROIBottom_Pad.Name = "lbl_ForeignMaterialROIBottom_Pad";
            this.lbl_ForeignMaterialROIBottom_Pad.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pnl_ForeignMaterialLeft_Pad
            // 
            this.pnl_ForeignMaterialLeft_Pad.Controls.Add(this.txt_ForeignMaterialStartPixelFromLeft_Pad);
            resources.ApplyResources(this.pnl_ForeignMaterialLeft_Pad, "pnl_ForeignMaterialLeft_Pad");
            this.pnl_ForeignMaterialLeft_Pad.Name = "pnl_ForeignMaterialLeft_Pad";
            // 
            // txt_ForeignMaterialStartPixelFromLeft_Pad
            // 
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.BackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.DecimalPlaces = 0;
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.ForeColor = System.Drawing.Color.Black;
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_ForeignMaterialStartPixelFromLeft_Pad, "txt_ForeignMaterialStartPixelFromLeft_Pad");
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.Name = "txt_ForeignMaterialStartPixelFromLeft_Pad";
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.NormalBackColor = System.Drawing.Color.White;
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.TextChanged += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromLeft_Pad_TextChanged);
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.Enter += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_Enter);
            this.txt_ForeignMaterialStartPixelFromLeft_Pad.Leave += new System.EventHandler(this.txt_ForeignMaterialStartPixelFromEdge_Pad_Leave);
            // 
            // srmLabel74
            // 
            resources.ApplyResources(this.srmLabel74, "srmLabel74");
            this.srmLabel74.Name = "srmLabel74";
            this.srmLabel74.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ForeignMaterialROIRight_Pad
            // 
            resources.ApplyResources(this.lbl_ForeignMaterialROIRight_Pad, "lbl_ForeignMaterialROIRight_Pad");
            this.lbl_ForeignMaterialROIRight_Pad.Name = "lbl_ForeignMaterialROIRight_Pad";
            this.lbl_ForeignMaterialROIRight_Pad.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel76
            // 
            resources.ApplyResources(this.srmLabel76, "srmLabel76");
            this.srmLabel76.Name = "srmLabel76";
            this.srmLabel76.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ForeignMaterialROITop_Pad
            // 
            resources.ApplyResources(this.lbl_ForeignMaterialROITop_Pad, "lbl_ForeignMaterialROITop_Pad");
            this.lbl_ForeignMaterialROITop_Pad.Name = "lbl_ForeignMaterialROITop_Pad";
            this.lbl_ForeignMaterialROITop_Pad.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel78
            // 
            resources.ApplyResources(this.srmLabel78, "srmLabel78");
            this.srmLabel78.Name = "srmLabel78";
            this.srmLabel78.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Title
            // 
            this.lbl_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            resources.ApplyResources(this.lbl_Title, "lbl_Title");
            this.lbl_Title.ForeColor = System.Drawing.Color.Black;
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // timer_Pad
            // 
            this.timer_Pad.Enabled = true;
            this.timer_Pad.Tick += new System.EventHandler(this.timer_Pad_Tick);
            // 
            // PadOtherSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.lbl_Title);
            this.Controls.Add(this.tab_VisionControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PadOtherSettingForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PadOtherSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.PadOtherSettingForm_Load);
            this.tab_VisionControl.ResumeLayout(false);
            this.tp_Segment.ResumeLayout(false);
            this.pnl_PadImageMerge2Threshold.ResumeLayout(false);
            this.grp_ImageMerge2.ResumeLayout(false);
            this.pnl_MinArea_PadImageMerge2Threshold.ResumeLayout(false);
            this.pnl_MinArea_PadImageMerge2Threshold.PerformLayout();
            this.pnl_MindArea.ResumeLayout(false);
            this.pnl_MindArea.PerformLayout();
            this.pnl_SurfaceThreshold.ResumeLayout(false);
            this.pnl_PadThreshold.ResumeLayout(false);
            this.tp_PkgSegmentSimple.ResumeLayout(false);
            this.pnl_DarkDefect.ResumeLayout(false);
            this.pnl_DarkDefect.PerformLayout();
            this.pnl_BrightDefect.ResumeLayout(false);
            this.pnl_BrightDefect.PerformLayout();
            this.tp_PkgSegment.ResumeLayout(false);
            this.pnl_DarkCrack.ResumeLayout(false);
            this.pnl_DarkCrack.PerformLayout();
            this.pnl_DarkVoid.ResumeLayout(false);
            this.pnl_DarkVoid.PerformLayout();
            this.pnl_BrightMold.ResumeLayout(false);
            this.pnl_BrightMold.PerformLayout();
            this.pnl_DarkChipped.ResumeLayout(false);
            this.pnl_DarkChipped.PerformLayout();
            this.pnl_BrightChipped.ResumeLayout(false);
            this.pnl_BrightChipped.PerformLayout();
            this.tp_PkgSegment2.ResumeLayout(false);
            this.pnl_ForeignMaterialDefect.ResumeLayout(false);
            this.pnl_ForeignMaterialDefect.PerformLayout();
            this.tp_other.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel_LIneProfileSetting.ResumeLayout(false);
            this.panel_Other.ResumeLayout(false);
            this.srmGroupBox16.ResumeLayout(false);
            this.srmGroupBox16.PerformLayout();
            this.tp_ROI.ResumeLayout(false);
            this.grpbox_ForeignMaterialROI.ResumeLayout(false);
            this.pnl_ForeignMaterialBottom.ResumeLayout(false);
            this.pnl_ForeignMaterialBottom.PerformLayout();
            this.pnl_ForeignMaterialRight.ResumeLayout(false);
            this.pnl_ForeignMaterialRight.PerformLayout();
            this.pnl_ForeignMaterialTop.ResumeLayout(false);
            this.pnl_ForeignMaterialTop.PerformLayout();
            this.pnl_ForeignMaterialLeft.ResumeLayout(false);
            this.pnl_ForeignMaterialLeft.PerformLayout();
            this.gbox_Pkg_Dark.ResumeLayout(false);
            this.pnl_PkgBottom_Dark.ResumeLayout(false);
            this.pnl_PkgBottom_Dark.PerformLayout();
            this.pnl_PkgRight_Dark.ResumeLayout(false);
            this.pnl_PkgRight_Dark.PerformLayout();
            this.pnl_PkgTop_Dark.ResumeLayout(false);
            this.pnl_PkgTop_Dark.PerformLayout();
            this.pnl_PkgLeft_Dark.ResumeLayout(false);
            this.pnl_PkgLeft_Dark.PerformLayout();
            this.gbox_Pkg.ResumeLayout(false);
            this.pnl_PkgBottom.ResumeLayout(false);
            this.pnl_PkgBottom.PerformLayout();
            this.pnl_PkgRight.ResumeLayout(false);
            this.pnl_PkgRight.PerformLayout();
            this.pnl_PkgTop.ResumeLayout(false);
            this.pnl_PkgTop.PerformLayout();
            this.pnl_PkgLeft.ResumeLayout(false);
            this.pnl_PkgLeft.PerformLayout();
            this.tp_ROI2.ResumeLayout(false);
            this.gbox_Chip.ResumeLayout(false);
            this.pnl_ChippedTop.ResumeLayout(false);
            this.pnl_ChippedTop.PerformLayout();
            this.pnl_ChippedLeft.ResumeLayout(false);
            this.pnl_ChippedLeft.PerformLayout();
            this.pnl_ChippedBottom.ResumeLayout(false);
            this.pnl_ChippedBottom.PerformLayout();
            this.pnl_ChippedRight.ResumeLayout(false);
            this.pnl_ChippedRight.PerformLayout();
            this.gbox_Chip_Dark.ResumeLayout(false);
            this.pnl_ChippedTop_Dark.ResumeLayout(false);
            this.pnl_ChippedTop_Dark.PerformLayout();
            this.pnl_ChippedLeft_Dark.ResumeLayout(false);
            this.pnl_ChippedLeft_Dark.PerformLayout();
            this.pnl_ChippedBottom_Dark.ResumeLayout(false);
            this.pnl_ChippedBottom_Dark.PerformLayout();
            this.pnl_ChippedRight_Dark.ResumeLayout(false);
            this.pnl_ChippedRight_Dark.PerformLayout();
            this.gbox_Mold.ResumeLayout(false);
            this.pnl_MoldLeft.ResumeLayout(false);
            this.pnl_MoldLeft.PerformLayout();
            this.pnl_MoldBottom.ResumeLayout(false);
            this.pnl_MoldBottom.PerformLayout();
            this.pnl_MoldRight.ResumeLayout(false);
            this.pnl_MoldRight.PerformLayout();
            this.pnl_MoldTop.ResumeLayout(false);
            this.pnl_MoldTop.PerformLayout();
            this.tp_ROI_Pad.ResumeLayout(false);
            this.grpbox_ForeignMaterialROI_Pad.ResumeLayout(false);
            this.pnl_ForeignMaterialBottom_Pad.ResumeLayout(false);
            this.pnl_ForeignMaterialBottom_Pad.PerformLayout();
            this.pnl_ForeignMaterialRight_Pad.ResumeLayout(false);
            this.pnl_ForeignMaterialRight_Pad.PerformLayout();
            this.pnl_ForeignMaterialTop_Pad.ResumeLayout(false);
            this.pnl_ForeignMaterialTop_Pad.PerformLayout();
            this.pnl_ForeignMaterialLeft_Pad.ResumeLayout(false);
            this.pnl_ForeignMaterialLeft_Pad.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMTabControl tab_VisionControl;
        private System.Windows.Forms.TabPage tp_Segment;
        private SRMControl.SRMLabel lbl_Title;
        private SRMControl.SRMButton btn_Save;
        private System.Windows.Forms.TabPage tp_other;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_PadThreshold;
        private SRMControl.SRMButton btn_SurfaceThreshold;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel lbl_PinScoreTitle;
        private SRMControl.SRMGroupBox srmGroupBox16;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel3;
        private System.Windows.Forms.Label lbl_HighSurfaceThreshold;
        private System.Windows.Forms.Label lbl_LowSurfaceThreshold;
        private System.Windows.Forms.Label lbl_PadThreshold;
        private SRMControl.SRMInputBox txt_MinArea;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel srmLabel5;
        private System.Windows.Forms.Timer timer_Pad;
        private System.Windows.Forms.Panel panel_LIneProfileSetting;
        private SRMControl.SRMButton btn_LineProfileGaugeSetting;
        private System.Windows.Forms.Panel panel_Other;
        private System.Windows.Forms.TabPage tp_PkgSegment;
        private System.Windows.Forms.Label lbl_MoldFlashThreshold;
        private SRMControl.SRMLabel srmLabel20;
        private SRMControl.SRMButton btn_MoldFlashThreshold;
        private System.Windows.Forms.Label lbl_PkgImage1HighThreshold;
        private System.Windows.Forms.Label lbl_PkgImage1LowThreshold;
        private SRMControl.SRMLabel srmLabel16;
        private SRMControl.SRMLabel srmLabel17;
        private SRMControl.SRMLabel srmLabel13;
        private SRMControl.SRMButton btn_PackageSurfaceThreshold;
        private SRMControl.SRMButton btn_Image2Threshold;
        private System.Windows.Forms.Label lbl_PkgImage2HighThreshold;
        private System.Windows.Forms.Label lbl_PkgImage2LowThreshold;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMInputBox txt_SurfaceMinArea;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMInputBox txt_Image2SurfaceMinArea;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMLabel srmLabel14;
        private SRMControl.SRMInputBox txt_MoldFlashMinArea;
        private SRMControl.SRMLabel srmLabel15;
        private System.Windows.Forms.TabPage tp_ROI;
        private SRMControl.SRMCheckBox chk_SetToAll;
        private SRMControl.SRMGroupBox gbox_Mold;
        private SRMControl.SRMInputBox txt_MoldStartPixelFromLeft;
        private SRMControl.SRMInputBox txt_MoldStartPixelFromBottom;
        private SRMControl.SRMInputBox txt_MoldStartPixelFromRight;
        private SRMControl.SRMLabel srmLabel44;
        private SRMControl.SRMLabel lbl_MoldLeft;
        private SRMControl.SRMLabel srmLabel46;
        private SRMControl.SRMLabel lbl_MoldBottom;
        private SRMControl.SRMLabel srmLabel48;
        private SRMControl.SRMLabel lbl_MoldRight;
        private SRMControl.SRMLabel srmLabel50;
        private SRMControl.SRMInputBox txt_MoldStartPixelFromEdge;
        private SRMControl.SRMLabel lbl_MoldTop;
        private SRMControl.SRMGroupBox gbox_Chip;
        private SRMControl.SRMInputBox txt_ChipStartPixelFromLeft;
        private SRMControl.SRMInputBox txt_ChipStartPixelFromBottom;
        private SRMControl.SRMInputBox txt_ChipStartPixelFromRight;
        private SRMControl.SRMLabel srmLabel36;
        private SRMControl.SRMLabel lbl_ChipLeft;
        private SRMControl.SRMLabel srmLabel38;
        private SRMControl.SRMLabel lbl_ChipBottom;
        private SRMControl.SRMLabel srmLabel40;
        private SRMControl.SRMLabel lbl_ChipRight;
        private SRMControl.SRMLabel srmLabel42;
        private SRMControl.SRMInputBox txt_ChipStartPixelFromEdge;
        private SRMControl.SRMLabel lbl_ChipTop;
        private SRMControl.SRMGroupBox gbox_Pkg;
        private SRMControl.SRMInputBox txt_PkgStartPixelFromLeft;
        private SRMControl.SRMInputBox txt_PkgStartPixelFromBottom;
        private SRMControl.SRMInputBox txt_PkgStartPixelFromRight;
        private SRMControl.SRMLabel srmLabel26;
        private SRMControl.SRMLabel lbl_PkgLeft;
        private SRMControl.SRMLabel srmLabel23;
        private SRMControl.SRMLabel lbl_PkgBottom;
        private SRMControl.SRMLabel srmLabel19;
        private SRMControl.SRMLabel lbl_PkgRight;
        private SRMControl.SRMLabel lbl_UnitDisplay1;
        private SRMControl.SRMInputBox txt_PkgStartPixelFromEdge;
        private SRMControl.SRMLabel lbl_PkgTop;
        private SRMControl.SRMLabel srmLabel34;
        private SRMControl.SRMLabel srmLabel28;
        private System.Windows.Forms.Label lbl_HighCrackViewThreshold;
        private SRMControl.SRMInputBox txt_VoidMinArea;
        private System.Windows.Forms.Label lbl_LowCrackViewThreshold;
        private SRMControl.SRMLabel srmLabel29;
        private System.Windows.Forms.Label lbl_Void_Threshold;
        private SRMControl.SRMLabel srmLabel31;
        private SRMControl.SRMLabel srmLabel30;
        private SRMControl.SRMButton btn_Void_Threshold;
        private SRMControl.SRMLabel srmLabel32;
        private SRMControl.SRMLabel srmLabel33;
        private SRMControl.SRMButton btn_CrackThreshold;
        private SRMControl.SRMInputBox txt_CrackMinArea;
        private SRMControl.SRMLabel srmLabel35;
        private System.Windows.Forms.Panel pnl_BrightChipped;
        private System.Windows.Forms.Panel pnl_DarkChipped;
        private System.Windows.Forms.Panel pnl_BrightMold;
        private System.Windows.Forms.Panel pnl_DarkVoid;
        private System.Windows.Forms.Panel pnl_DarkCrack;
        private System.Windows.Forms.TabPage tp_PkgSegmentSimple;
        private System.Windows.Forms.Panel pnl_DarkDefect;
        private SRMControl.SRMButton btn_DarkThreshold;
        private SRMControl.SRMInputBox txt_DarkMinArea;
        private SRMControl.SRMLabel srmLabel56;
        private SRMControl.SRMLabel srmLabel57;
        private SRMControl.SRMLabel lbl_DarkDefect;
        private System.Windows.Forms.Panel pnl_BrightDefect;
        private SRMControl.SRMButton btn_BrightThreshold;
        private SRMControl.SRMInputBox txt_BrightMinArea;
        private SRMControl.SRMLabel srmLabel18;
        private SRMControl.SRMLabel srmLabel22;
        private SRMControl.SRMLabel lbl_BrightDefect;
        private System.Windows.Forms.Label lbl_DarkThreshold;
        private SRMControl.SRMLabel srmLabel59;
        private System.Windows.Forms.Label lbl_BrightThreshold;
        private SRMControl.SRMLabel srmLabel58;
        private SRMControl.SRMLabel srmLabel52;
        private SRMControl.SRMLabel lbl_ThickIteration;
        private SRMControl.SRMInputBox txt_ThickIteration;
        private SRMControl.SRMLabel srmLabel53;
        private SRMControl.SRMLabel srmLabel54;
        private SRMControl.SRMInputBox txt_ThinIteration;
        private System.Windows.Forms.GroupBox grp_ImageMerge2;
        private SRMControl.SRMLabel lbl_PadImageMerge2ThresholdTitle;
        private SRMControl.SRMLabel lbl_PadImageMerge2PixTitle;
        private SRMControl.SRMButton btn_PadImageMerge2Threshold;
        private SRMControl.SRMInputBox txt_PadImageMerge2MinArea;
        private SRMControl.SRMLabel lbl_PadImageMerge2AreaTitle;
        private SRMControl.SRMCheckBox chk_SetToAllSideROI;
        private System.Windows.Forms.Panel panel1;
        private SRMControl.SRMButton btn_PadROIToleranceSetting;
        private System.Windows.Forms.Panel pnl_MoldLeft;
        private System.Windows.Forms.Panel pnl_MoldBottom;
        private System.Windows.Forms.Panel pnl_MoldRight;
        private System.Windows.Forms.Panel pnl_MoldTop;
        private System.Windows.Forms.Panel pnl_ChippedTop;
        private System.Windows.Forms.Panel pnl_ChippedLeft;
        private System.Windows.Forms.Panel pnl_ChippedBottom;
        private System.Windows.Forms.Panel pnl_ChippedRight;
        private System.Windows.Forms.Panel pnl_PkgRight;
        private System.Windows.Forms.Panel pnl_PkgTop;
        private System.Windows.Forms.Panel pnl_PkgLeft;
        private System.Windows.Forms.Panel pnl_PkgBottom;
        private System.Windows.Forms.Label lbl_PadImageMerge2Threshold_Low;
        private System.Windows.Forms.Panel panel2;
        private SRMControl.SRMButton btn_PadInspectionAreaSetting;
        private SRMControl.SRMInputBox txt_MoldStartPixelFromLeftInner;
        private SRMControl.SRMInputBox txt_MoldStartPixelFromBottomInner;
        private SRMControl.SRMInputBox txt_MoldStartPixelFromRightInner;
        private SRMControl.SRMInputBox txt_MoldStartPixelFromEdgeInner;
        private SRMControl.SRMLabel srmLabel21;
        private SRMControl.SRMLabel srmLabel24;
        private SRMControl.SRMInputBox txt_ChipStartPixelExtendFromEdge;
        private SRMControl.SRMInputBox txt_ChipStartPixelExtendFromLeft;
        private SRMControl.SRMInputBox txt_ChipStartPixelExtendFromBottom;
        private SRMControl.SRMInputBox txt_ChipStartPixelExtendFromRight;
        private SRMControl.SRMLabel srmLabel83;
        private SRMControl.SRMLabel srmLabel84;
        private SRMControl.SRMGroupBox gbox_Chip_Dark;
        private SRMControl.SRMLabel srmLabel25;
        private SRMControl.SRMLabel srmLabel27;
        private System.Windows.Forms.Panel pnl_ChippedTop_Dark;
        private SRMControl.SRMInputBox txt_ChipStartPixelExtendFromEdge_Dark;
        private SRMControl.SRMInputBox txt_ChipStartPixelFromEdge_Dark;
        private System.Windows.Forms.Panel pnl_ChippedLeft_Dark;
        private SRMControl.SRMInputBox txt_ChipStartPixelExtendFromLeft_Dark;
        private SRMControl.SRMInputBox txt_ChipStartPixelFromLeft_Dark;
        private System.Windows.Forms.Panel pnl_ChippedBottom_Dark;
        private SRMControl.SRMInputBox txt_ChipStartPixelExtendFromBottom_Dark;
        private SRMControl.SRMInputBox txt_ChipStartPixelFromBottom_Dark;
        private System.Windows.Forms.Panel pnl_ChippedRight_Dark;
        private SRMControl.SRMInputBox txt_ChipStartPixelExtendFromRight_Dark;
        private SRMControl.SRMInputBox txt_ChipStartPixelFromRight_Dark;
        private SRMControl.SRMLabel srmLabel37;
        private SRMControl.SRMLabel lbl_ChipBottom_Dark;
        private SRMControl.SRMLabel srmLabel41;
        private SRMControl.SRMLabel lbl_ChipRight_Dark;
        private SRMControl.SRMLabel srmLabel45;
        private SRMControl.SRMLabel lbl_ChipTop_Dark;
        private SRMControl.SRMLabel srmLabel49;
        private SRMControl.SRMLabel lbl_ChipLeft_Dark;
        private System.Windows.Forms.TabPage tp_ROI2;
        private SRMControl.SRMGroupBox gbox_Pkg_Dark;
        private System.Windows.Forms.Panel pnl_PkgBottom_Dark;
        private SRMControl.SRMInputBox txt_PkgStartPixelFromBottom_Dark;
        private System.Windows.Forms.Panel pnl_PkgRight_Dark;
        private SRMControl.SRMInputBox txt_PkgStartPixelFromRight_Dark;
        private System.Windows.Forms.Panel pnl_PkgTop_Dark;
        private SRMControl.SRMInputBox txt_PkgStartPixelFromEdge_Dark;
        private SRMControl.SRMLabel lbl_PkgLeft_Dark;
        private SRMControl.SRMLabel srmLabel43;
        private SRMControl.SRMLabel lbl_PkgBottom_Dark;
        private System.Windows.Forms.Panel pnl_PkgLeft_Dark;
        private SRMControl.SRMInputBox txt_PkgStartPixelFromLeft_Dark;
        private SRMControl.SRMLabel srmLabel51;
        private SRMControl.SRMLabel lbl_PkgRight_Dark;
        private SRMControl.SRMLabel srmLabel60;
        private SRMControl.SRMLabel lbl_PkgTop_Dark;
        private SRMControl.SRMLabel srmLabel62;
        private SRMControl.SRMGroupBox grpbox_ForeignMaterialROI;
        private System.Windows.Forms.Panel pnl_ForeignMaterialBottom;
        private SRMControl.SRMInputBox txt_ForeignMaterialStartPixelFromBottom;
        private System.Windows.Forms.Panel pnl_ForeignMaterialRight;
        private SRMControl.SRMInputBox txt_ForeignMaterialStartPixelFromRight;
        private System.Windows.Forms.Panel pnl_ForeignMaterialTop;
        private SRMControl.SRMInputBox txt_ForeignMaterialStartPixelFromEdge;
        private SRMControl.SRMLabel lbl_ForeignMaterialROILeft;
        private SRMControl.SRMLabel srmLabel47;
        private SRMControl.SRMLabel lbl_ForeignMaterialROIBottom;
        private System.Windows.Forms.Panel pnl_ForeignMaterialLeft;
        private SRMControl.SRMInputBox txt_ForeignMaterialStartPixelFromLeft;
        private SRMControl.SRMLabel srmLabel61;
        private SRMControl.SRMLabel lbl_ForeignMaterialROIRight;
        private SRMControl.SRMLabel srmLabel64;
        private SRMControl.SRMLabel lbl_ForeignMaterialROITop;
        private SRMControl.SRMLabel srmLabel66;
        private System.Windows.Forms.TabPage tp_PkgSegment2;
        private System.Windows.Forms.Panel pnl_ForeignMaterialDefect;
        private System.Windows.Forms.Label lbl_ForeignMaterialThreshold;
        private SRMControl.SRMLabel srmLabel67;
        private SRMControl.SRMButton btn_ForeignMaterialThreshold;
        private SRMControl.SRMInputBox txt_ForeignMaterialMinArea;
        private SRMControl.SRMLabel srmLabel68;
        private SRMControl.SRMLabel srmLabel69;
        private SRMControl.SRMLabel srmLabel70;
        private System.Windows.Forms.Panel pnl_PadImageMerge2Threshold;
        private System.Windows.Forms.Panel pnl_MindArea;
        private System.Windows.Forms.Panel pnl_SurfaceThreshold;
        private System.Windows.Forms.Panel pnl_PadThreshold;
        private System.Windows.Forms.Panel pnl_MinArea_PadImageMerge2Threshold;
        private System.Windows.Forms.Label lbl_PadImageMerge2Threshold_High;
        private SRMControl.SRMLabel srmLabel72;
        private SRMControl.SRMLabel srmLabel71;
        private SRMControl.SRMImageComboBox cbo_SelectROI;
        private System.Windows.Forms.TabPage tp_ROI_Pad;
        private SRMControl.SRMGroupBox grpbox_ForeignMaterialROI_Pad;
        private System.Windows.Forms.Panel pnl_ForeignMaterialBottom_Pad;
        private SRMControl.SRMInputBox txt_ForeignMaterialStartPixelFromBottom_Pad;
        private System.Windows.Forms.Panel pnl_ForeignMaterialRight_Pad;
        private SRMControl.SRMInputBox txt_ForeignMaterialStartPixelFromRight_Pad;
        private System.Windows.Forms.Panel pnl_ForeignMaterialTop_Pad;
        private SRMControl.SRMInputBox txt_ForeignMaterialStartPixelFromEdge_Pad;
        private SRMControl.SRMLabel lbl_ForeignMaterialROILeft_Pad;
        private SRMControl.SRMLabel srmLabel73;
        private SRMControl.SRMLabel lbl_ForeignMaterialROIBottom_Pad;
        private System.Windows.Forms.Panel pnl_ForeignMaterialLeft_Pad;
        private SRMControl.SRMInputBox txt_ForeignMaterialStartPixelFromLeft_Pad;
        private SRMControl.SRMLabel srmLabel74;
        private SRMControl.SRMLabel lbl_ForeignMaterialROIRight_Pad;
        private SRMControl.SRMLabel srmLabel76;
        private SRMControl.SRMLabel lbl_ForeignMaterialROITop_Pad;
        private SRMControl.SRMLabel srmLabel78;
    }
}