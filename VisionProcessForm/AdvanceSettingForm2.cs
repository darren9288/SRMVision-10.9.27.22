using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;
using VisionProcessing;
using System.IO;

namespace VisionProcessForm
{
    public partial class AdvanceSettingForm2 : Form
    {
        #region Member Variables
        private bool m_blnInitDone = false;
        private int m_intBarcodeInspectionArea_Prev = 0, m_intBarcodePatternInspectionArea_Top_Prev = 0, m_intBarcodePatternInspectionArea_Right_Prev = 0, m_intBarcodePatternInspectionArea_Bottom_Prev = 0, m_intBarcodePatternInspectionArea_Left_Prev = 0;
        private bool m_blnWantSkipMarkPrev;
        private bool m_blnDragROIPrev = false;
        private int m_intUserGroup = 5;
        private int m_intSelectedTabPage = 0;
        private string m_strSelectedRecipe;
        private int[] m_arrSensitivityOnPadMethod;
        private int[] m_arrSensitivityOnPadValue;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;

        LineProfileForm m_objLineProfileForm;
        LeadLineProfileForm m_objLeadLineProfileForm;
        private DataSet m_dsPackage = new DataSet();
        private int intPocketPitchPrev = 0;
        private int intSprocketPitchPrev = 0;
        private UserRight m_objUserRight = new UserRight();
        
        #endregion

        #region Properties

        // Orient
        public int ref_intDirection { get { if (radioBtn_2Directions.Checked) return 2; else return 4;}}
        public bool ref_blnWantSubROI { get { return chk_WantSubROI.Checked; } }

        // Mark
        public bool ref_blnWhiteOnBlack { get { return chk_WhiteOnBlack.Checked; } }
        public bool ref_blnWantMultiGroups { get { return chk_MultiGroups.Checked; } }
        public bool ref_blnWantBuildText { get { return chk_WantBuildTexts.Checked; } }
        public bool ref_blnWantMultiTemplates { get { return chk_MultiTemplates.Checked; } }
        public bool ref_blnWantSet1ToAll { get { return chk_Set1ToAll.Checked; } }
        public bool ref_blnWantSkipMark { get { return chk_SkipMarkInspection.Checked; } }
        public bool ref_blnWantPin1 { get { return chk_WantPin1.Checked; } }
        public bool ref_blnWantPadPin1 { get { return chk_WantPadPin1.Checked; } }

        // Package
        public bool ref_blnCheckPackage { get { return chk_CheckPackage.Checked; } }

        // Pad
        public bool ref_blnPadWhiteOnBlack { get { return chk_PadWhiteOnBlack.Checked; } }
        public bool ref_blnWantCheckPackage { get { return chk_WantCheckPackage.Checked; } }
        public bool ref_blnWantCheckPad { get { return chk_WantCheckPad.Checked; } }
        public bool ref_blnWantCheck4Sides { get { return chk_WantCheck4Sides.Checked; } }
        public bool ref_blnWantTestRunTightSetting { get { return chk_WantTestRunTightSetting.Checked; } } 
        public bool ref_blnWantShowGRR { get { return chk_WantGRR.Checked; } }
        public bool ref_blnWantConsiderPadImage2 { get { return chk_WantConsiderPadImage2.Checked; } }
        public bool ref_blnWantPRUnitLocationBeforeGauge { get { return chk_WantPRUnitLocationBeforeGauge.Checked; } }
        public bool ref_blnWantUseGaugeMeasureDimension
        {
            get
            {
                if (cbo_PadMeasurementMethod.SelectedIndex <= 0)
                    return false;
                else
                    return true;
            }
        }
        public bool ref_blnWantUseClosestSizeDefineTolerance { get { return chk_DefinePadToleranceUsingClosestMethod.Checked; } }
        public bool ref_blnWantRotateSidePadImage { get { return chk_WantRotateSidePadImage.Checked; } }
        public bool ref_blnWantAutoGauge { get { return chk_WantAutoGauge.Checked; } } 
        public float ref_fDefaultPixelTole { get { return float.Parse(txt_DefaultPixelTolerance.Text); } }
        public int ref_intPadROITolerance { get { return int.Parse(txt_PadROITolerance.Text); } }
        //public int ref_intPadSizeHalfWidthTolerance{ get { return int.Parse(txt_PadSizeHalfWidthTolerance.Text); } }
        public float ref_fTightSettingDimensionTolerance { get { return float.Parse(txt_TightSettingDimensionTolerance.Text); } }
        public int ref_intTightSettingThresholdTolerance { get { return int.Parse(txt_TightSettingThresholdTolerance.Text); } }
        public int ref_intSensitivityOnPadMethod { get { return cbo_SensitivityOnPad.SelectedIndex; } }
        public int ref_intSensitivityOnPadValue { get { return int.Parse(txt_SensitivityValue.Text); } }

        // Seal
        public int ref_intDirectionSeal { get { if (radioBtn_2DirectionsSeal.Checked) return 2; else return 4; } }
        public int ref_intTapePocketPitch { get { return Convert.ToInt32(cbo_intPocketPitch.SelectedItem.ToString()); } }
        //public string ref_strPackageName { get { return cbo_Package.SelectedItem.ToString(); } }
        public bool ref_blnWantSkipOrient { get { return chk_SkipOrientInspection.Checked; } }

        // Lead
        public bool ref_blnWantCheckLead { get { return chk_WantCheckLead.Checked; } }
        public bool ref_blnWantUseGaugeMeasureLeadDimension { get { return chk_WantUseGaugeMeasureLeadDimension.Checked; } }

        #endregion

        public AdvanceSettingForm2(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intSelectedTabPage)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_intSelectedTabPage = intSelectedTabPage;
            
            //DisableField();
            DisableTabPage();
            UpdateGUI();
            DisableField2();

            m_blnInitDone = true;
        }
        private void DisableField()
        {
            string strChild1 = "Advance Setting Page";
            string strChild2 = "Save Advance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_OK.Enabled = false;
            }

            // Mark
            strChild2 = "Mark White On Black";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WhiteOnBlack.Enabled = false;
            }

            strChild2 = "Mark Multi Templates";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_MultiTemplates.Enabled = false;
            }

            strChild2 = "Mark Set Templates Based On BinInfo";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SetTemplateBasedOnBinInfo.Enabled = false;
            }

            strChild2 = "Mark Set 1 to All Templates";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_Set1ToAll.Enabled = false;
            }

            strChild2 = "Mark Skip Inspection";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SkipMarkInspection.Enabled = false;
            }

            strChild2 = "Mark Use Gauge Measure Dimension";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantUseGaugeForMeasureMarkPkg.Enabled = false;
            }

            strChild2 = "Clear Mark Template When New Lot";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantClearMarkTemplateWhenNewLot.Enabled = false;
            }

            strChild2 = "Default Mark Score";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_DefaultMarkScore.Enabled = false;
            }

            strChild2 = "Minimum Mark Score";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_MinMarkScore.Enabled = false;
            }

            strChild2 = "Maximum Mark Template";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_MaxMarkTemplate.Enabled = false;
            }

            strChild2 = "Mark Check No Mark Feature";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantCheckNoMark.Enabled = false;
                txt_NoMarkMaximumBlobArea.Enabled = false;
            }

            strChild2 = "Mark 2D Code";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantMark2DCode.Enabled = false;
                cbo_2DCodeType.Enabled = false;
            }

            strChild2 = "Mark Dont Care Area";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantDontCareArea_Mark.Enabled = false;
            }

            strChild2 = "Mark Check Bar Pin 1";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantCheckBarPin1.Enabled = false;
            }

            strChild2 = "Want Rotate Mark Image Using Package Angle";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantRotateMarkImageUsingPkgAngle.Enabled = false;
            }

            strChild2 = "Want Check Mark Angle";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantCheckMarkAngle.Enabled = false;
            }

            strChild2 = "Want Check Broken Mark";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantCheckBrokenMark.Enabled = false;
            }

            strChild2 = "Missing Mark Inspection Method";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_MissingMarkInspectionMethod.Enabled = false;
            }

            //Orient
            strChild2 = "Orient Direction";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                group_OrientDirections.Enabled = false;
            }

            strChild2 = "Orient Pin 1";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                //chk_WantPin1.Enabled = false;
                gb_Pin1.Enabled = false;
            }

            strChild2 = "Orient SubROI";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantSubROI.Enabled = false;
            }

            //Package
            strChild2 = "Package Size Image";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_PackageSizeImageNo.Enabled = false;
            }

            strChild2 = "Package Side Light Image";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_SideLightPackageBrightDefectImageNo.Enabled = false;
            }

            strChild2 = "Package Side Light Image";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_SideLightPackageDarkDefectImageNo.Enabled = false;
            }

            strChild2 = "Package Direct Light Image";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_DirectLightPackageDefectImageNo.Enabled = false;
            }


            strChild2 = "Package Use Side Light Gauge";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantUseSideLightGauge.Enabled = false;
            }

            strChild2 = "Package Check Dark Field On Mark Area";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_CheckVoidOnMarkArea.Enabled = false;
            }

            strChild2 = "Package Use Detail Threshold Setting Form";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantUseDetailThreshold_Package.Enabled = false;
            }

            strChild2 = "Package Use Link Defect Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantLinkBrightDefect.Enabled = false;
                pnl_WantLinkDarkDefect.Enabled = false;
                pnl_WantLinkDark2Defect.Enabled = false;
                pnl_WantLinkCrackDefect.Enabled = false;
                pnl_WantLinkMoldFlashDefect.Enabled = false;
            }

            strChild2 = "Package Separate Dark Field 2 Defect Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SeperateDarkField2DefectSetting_Package.Enabled = false;
            }

            strChild2 = "Package Separate Crack Defect Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SeperateCrackDefectSetting_Package.Enabled = false;
            }

            //strChild2 = "Package Separate Void Defect Setting";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    chk_SeperateVoidDefectSetting_Package.Enabled = false;
            //}

            strChild2 = "Package Separate Mold Flash Defect Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SeperateMoldFlashDefectSetting_Package.Enabled = false;
            }

            strChild2 = "Package Separate Chipped Off Defect Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SeperateChippedOffDefectSetting_Package.Enabled = false;
            }

            strChild2 = "Package Dont Care Area";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantDontCareArea_Package.Enabled = false;
            }

            strChild2 = "Package Angle";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantCheckPackageAngle_Package.Enabled = false;
            }

            //General 
            strChild2 = "General Want Check Empty";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantCheckEmpty.Enabled = false;
            }

            strChild2 = "General Want Check Unit Sit Proper";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantCheckUnitSitProper.Enabled = false;
            }

            strChild2 = "General Want Use Unit PR Find Gauge";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantUseUnitPRFindGauge.Enabled = false;
            }

            //Pad
            strChild2 = "Pad White On Black";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_PadWhiteOnBlack.Enabled = false;
            }

            strChild2 = "Pad Want Check PH";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantCheckPH.Enabled = false;
            }

            strChild2 = "Pad Want Check Pad";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantCheckPad.Enabled = false;
            }

            strChild2 = "Pad Want Check Pad Color";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantCheckPadColor.Enabled = false;
            }

            strChild2 = "Pad Want Consider Image2 for SidePad";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantConsiderPadImage2.Enabled = false;
            }

            strChild2 = "Pad Want PR Before Gauge";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantPRUnitLocationBeforeGauge.Enabled = false;
            }

            strChild2 = "Pad Closest Method for Tolerance";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_DefinePadToleranceUsingClosestMethod.Enabled = false;
            }

            strChild2 = "Pad Want Show Pin 1";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantPadPin1.Enabled = false;
            }

            strChild2 = "Pad Want Check Side Pad";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantCheck4Sides.Enabled = false;
            }

            strChild2 = "Pad Want Tight Setting for Test Run";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantTestRunTightSetting.Enabled = false;
            }

            strChild2 = "Pad Want GRR";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantGRR.Enabled = false;
            }

            strChild2 = "Pad Want CPK";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantCPK.Enabled = false;
            }

            strChild2 = "Pad Want Rotate Side Pad Image";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantRotateSidePadImage.Enabled = false;
            }

            strChild2 = "Pad Measurement Method";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_PadMeasurementMethod.Enabled = false;
                cbo_PadMeasurementMethod_Side.Enabled = false;
            }

            strChild2 = "Pad Sensitivity On Pad";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_SensitivityOnPad.Enabled = false;
                txt_SensitivityValue.Enabled = false;
                btn_PadSensitivity.Enabled = false;
            }

            strChild2 = "Pad Tight Setting Dimension Tolerance";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_TightSettingDimensionTolerance.Enabled = false;
            }

            strChild2 = "Pad Tight Setting Threshold Tolerance";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_TightSettingThresholdTolerance.Enabled = false;
            }

            //strChild2 = "Pad Size Tolerance";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    txt_PadSizeHalfWidthTolerance.Enabled = false;
            //}

            //strChild2 = "Pad Default Pixel Tolerance";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    txt_DefaultPixelTolerance.Enabled = false;
            //}

            strChild2 = "Pad ROI Tolerance";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_PadROITolerance.Enabled = false;
            }

            strChild2 = "Pad Dont Care Area";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantDontCareArea_Pad.Enabled = false;
            }

            strChild2 = "Broken Pad Image No";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_BrokenPadImageNo.Enabled = false;
            }

            //PadPackage
            strChild2 = "Pad Package Use Detail Threshold Setting Form";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantUseDetailThreshold_PadPackage.Enabled = false;
            }

            strChild2 = "Pad Package Separate Crack Defect Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SeperateCrackDefectSetting_PadPackage.Enabled = false;
            }

            strChild2 = "Pad Package Separate Mold Flash Defect Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SeperateMoldFlashDefectSetting_PadPackage.Enabled = false;
            }

            strChild2 = "Pad Package Separate Chipped Off Defect Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SeperateChippedOffDefectSetting_PadPackage.Enabled = false;
            }

            strChild2 = "Pad Package Size Image No";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_PadPkgSizeImageViewNo_Center.Enabled = false;
                cbo_PadPkgSizeImageViewNo_Side.Enabled = false;
            }

            strChild2 = "Pad Package Bright Field Image No";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_PadPkgBrightFieldImageViewNo_Center.Enabled = false;
                cbo_PadPkgBrightFieldImageViewNo_Side.Enabled = false;
            }

            strChild2 = "Pad Package Dark Field Image No";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_PadPkgDarkFieldImageNo_Center.Enabled = false;
                cbo_PadPkgDarkFieldImageNo_Side.Enabled = false;
            }

            strChild2 = "Pad Package Dont Care Area";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantDontCareArea_PadPackage.Enabled = false;
            }

            //Lead
            strChild2 = "Lead Want Check Lead";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantCheckLead.Enabled = false;
            }

            strChild2 = "Lead Want Use Gauge Measurement";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantUseGaugeMeasureLeadDimension.Enabled = false;
            }

            //Seal 
            strChild2 = "Seal Orientation Direction";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                group_OrientDirectionsSeal.Enabled = false;
            }


            strChild2 = "Seal Package Dimension";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_Package.Enabled = false;
            }

            strChild2 = "Seal Pocket Pitch Dimension";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_intPocketPitch.Enabled = false;
            }

            strChild2 = "Seal Skip Orient Inspection";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_SkipOrientInspection.Enabled = false;
            }

            strChild2 = "Seal Dont Care Area";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantDontCareArea_Seal.Enabled = false;
            }

            strChild2 = "Seal Mark Area Below Percent";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_MarkAreaBelowPercent.Enabled = false;
            }
        }
        private int GetUserRightGroup_Child3(string Child1, string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                case "BottomPosition":
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(Child1, Child2, Child3);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild3Group(Child1, Child2, Child3);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(Child1, Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                //case "BottomOrientPad":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(Child1, Child2, Child3);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(Child1, Child2, Child3);
                    break;
                case "Seal":
                    return m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(Child1, Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                case "Barcode":
                    return m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(Child1, Child2, Child3);
                    break;
            }

            return 1;
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "";

            if (m_intSelectedTabPage == 0)
            {
                switch (m_smVisionInfo.g_strVisionName)
                {
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                        strChild1 = "OPad";
                        break;
                    case "Orient":
                    case "BottomOrient":
                    case "BottomPosition":
                        strChild1 = "Orient";
                        break;
                    case "Mark":
                    case "MarkOrient":
                    case "MOLi":
                    case "Package":
                    case "MarkPkg":
                    case "MOPkg":
                    case "MOLiPkg":
                        strChild1 = "Mark";
                        break;
                    case "IPMLi":
                    case "IPMLiPkg":
                    case "InPocket":
                    case "InPocketPkg":
                    case "InPocketPkgPos":
                        strChild1 = "Mark";
                        break;
                    //case "BottomOrientPad":
                    case "Pad":
                    case "PadPos":
                    case "PadPkg":
                    case "PadPkgPos":
                    case "Pad5S":
                    case "Pad5SPos":
                    case "Pad5SPkg":
                    case "Pad5SPkgPos":
                        strChild1 = "Pad";
                        break;
                    case "Li3D":
                    case "Li3DPkg":
                        strChild1 = "Lead3D";
                        break;
                    case "Seal":
                        strChild1 = "Seal";
                        break;
                    case "Barcode":
                        strChild1 = "Barcode";
                        break;
                }

            }
            else if (m_intSelectedTabPage == 1)
                strChild1 = "Package";
            else if (m_intSelectedTabPage == 2)
                strChild1 = "Lead";

            string strChild2 = "Advance Setting Page";
            string strChild3 = "Save Advance Setting";

            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_OK.Enabled = false;
            }

            // Mark
            strChild3 = "Mark White On Black";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WhiteOnBlack.Enabled = false;
            }

            strChild3 = "Mark Multi Templates";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_MultiTemplates.Enabled = false;
            }

            strChild3 = "Mark Set Templates Based On BinInfo";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SetTemplateBasedOnBinInfo.Enabled = false;
            }

            strChild3 = "Mark Set 1 to All Templates";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_Set1ToAll.Enabled = false;
            }

            //strChild3 = "Mark Skip Inspection";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_SkipMarkInspection.Enabled = false;
            //}

            strChild3 = "Mark Use Gauge Measure Dimension";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseGaugeForMeasureMarkPkg.Enabled = false;
            }

            strChild3 = "Clear Mark Template When New Lot";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantClearMarkTemplateWhenNewLot.Enabled = false;
            }

            strChild3 = "Use Default Mark Score After Delete All Template";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_UseDefaultMarkScoreAfterClearTemplate.Enabled = false;
            }

            strChild3 = "Default Mark Score";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_DefaultMarkScore.Enabled = false;
            }

            strChild3 = "Minimum Mark Score";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MinMarkScore.Enabled = false;
            }

            strChild3 = "Maximum Mark Template";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MaxMarkTemplate.Enabled = false;
            }

            strChild3 = "Mark Check Contour On Mark";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckContourOnMark.Enabled = false;
            }

            strChild3 = "Mark Check No Mark Feature";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckNoMark.Enabled = false;
                txt_NoMarkMaximumBlobArea.Enabled = false;
            }

            strChild3 = "Mark 2D Code";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantMark2DCode.Enabled = false;
                cbo_2DCodeType.Enabled = false;
            }

            strChild3 = "Mark Dont Care Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantDontCareArea_Mark.Enabled = false;
            }

            strChild3 = "Mark Check Bar Pin 1";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckBarPin1.Enabled = false;
            }

            strChild3 = "Mark Check Angle";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckMarkAngle.Enabled = false;
            }

            //strChild3 = "Want Rotate Mark Image Using Package Angle";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_WantRotateMarkImageUsingPkgAngle.Enabled = false;
            //}

            strChild3 = "Missing Mark Inspection Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_MissingMarkInspectionMethod.Enabled = false;
            }

            strChild3 = "Mark Want Sample Area Score";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantSampleAreaScore.Enabled = false;
            }

            strChild3 = "Mark Inspection Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_MarkDefectInspectionMethod.Enabled = false;
                btn_MarkGrayValueSensitivitySetting.Enabled = false;
            }

            strChild3 = "Mark Text Shift Tolerance Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_MarkTextShiftMethod.Enabled = false;
            }

            strChild3 = "Extra/Excess Mark Insepction Area Cut Mode";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_MarkTextShiftMethod.Enabled = false;
            }

            strChild3 = "Separate Extra Mark Threshold";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeparateExtraMarkThreshold.Enabled = false;
            }

            strChild3 = "Want Excess Mark Threshold Follow Extra Mark Threshold";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Enabled = false;
            }
            
            strChild3 = "Mark Score Offset";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MarkScoreOffset.Enabled = false;
            }

            //strChild3 = "Mark Use Lead Point Offset Mark ROI";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_WantUseLeadPointOffsetMarkROI.Enabled = false;
            //}

            strChild3 = "Mark Dont Care Ignored Mark Whole Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantDontCareIgnoredMarkWholeArea.Enabled = false;
            }

            strChild3 = "Mark Remove Border When Learn Mark";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantRemoveBorderWhenLearnMark.Enabled = false;
            }

            strChild3 = "Mark Check Total Excess Mark";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckTotalExcessMark.Enabled = false;
            }

            strChild3 = "Mark Check Broken Mark";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckBrokenMark.Enabled = false;
            }

            strChild3 = "Mark Type Inspection Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseMarkTypeInspectionSetting.Enabled = false;
                btn_MarkTypeInspectionSetting.Enabled = false;
            }

            strChild3 = "Mark Char ROI Offset Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MarkCharROIOffsetX.Enabled = false;
                txt_MarkCharROIOffsetY.Enabled = false;
            }

            strChild3 = "Final Reduction Or Interpolation Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_FinalReduction_Direction.Enabled = false;
                cbo_FinalReduction_MarkDeg.Enabled = false;
                cbo_RotationInterpolation_Mark.Enabled = false;
                cbo_RotationInterpolation_PkgBright.Enabled = false;
                cbo_RotationInterpolation_PkgDark.Enabled = false;
            }

            strChild3 = "Check Mark Average Gray Value";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckMarkAverageGrayValue.Enabled = false;
            }

            strChild3 = "Compensate Mark Different Size Mode";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_CompensateMarkDiffSizeMode.Enabled = false;
            }

            strChild3 = "Mark Score Mode";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_MarkScoreMode.Enabled = false;
            }

            strChild3 = "Check Mark Ori Position When Lower Score";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MarkOriPositionScore.Enabled = false;
            }

            strChild3 = "Check Mark Angle Min Max Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_CheckMarkAngleMinMaxTolerance.Enabled = false;
            }

            strChild3 = "Use Unit Pattern As Mark Pattern";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseUnitPatternAsMarkPattern.Enabled = false;
            }

            strChild3 = "Want Use Excess/Missing Mark Affect Score";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseExcessMissingMarkAffectScore.Enabled = false;
            }

            //Orient
            strChild3 = "Orient Direction";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                group_OrientDirections.Enabled = false;
            }

            strChild3 = "Pin 1";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantPin1.Enabled = false;
            }

            strChild3 = "Want Use Pin 1 Orientation When No Mark";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUsePin1OrientationWhenNoMark.Enabled = false;
            }

            strChild3 = "Pin 1 Image View No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_Pin1ImageNo.Enabled = false;
            }

            strChild3 = "Pin 1 Position Control";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_Pin1PositionControl.Enabled = false;
            }

            //strChild3 = "Orient SubROI";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_WantSubROI.Enabled = false;
            //}

            strChild3 = "Position Orientation";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                grp_PositionOrientation.Enabled = false;
            }

            //strChild3 = "Check Package Using Gauge";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_PositionWantGauge.Enabled = false;
            //}

            strChild3 = "Bottom Orient Want Package";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_OrientWantPackage.Enabled = false;
            }
            
            //Package
            //strChild3 = "Package Size Image";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    cbo_PackageSizeImageNo.Enabled = false;
            //}

            strChild3 = "Package Side Light Image";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SideLightPackageBrightDefectImageNo.Enabled = false;
            }

            strChild3 = "Package Side Light Image";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SideLightPackageDarkDefectImageNo.Enabled = false;
            }

            strChild3 = "Package Side Light Image";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SideLightPackageDarkDefect2ImageNo.Enabled = false;
            }

            strChild3 = "Package Side Light Image";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SideLightPackageDarkDefect3ImageNo.Enabled = false;
            }

            strChild3 = "Package Direct Light Image";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_DirectLightPackageDefectImageNo.Enabled = false;
            }


            strChild3 = "Package Use Side Light Gauge";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseSideLightGauge.Enabled = false;
            }

            strChild3 = "Package Check Dark Field On Mark Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_CheckVoidOnMarkArea.Enabled = false;
            }

            strChild3 = "Package Check Dark Field2 On Mark Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_CheckVoidOnMarkArea_Dark2.Enabled = false;
            }

            strChild3 = "Package Check Dark Field3 On Mark Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_CheckVoidOnMarkArea_Dark3.Enabled = false;
            }
            strChild3 = "Package Check Dark Field4 On Mark Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_CheckVoidOnMarkArea_Dark4.Enabled = false;
            }

            //strChild3 = "Package Use Detail Threshold Setting Form";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_WantUseDetailThreshold_Package.Enabled = false;
            //}

            strChild3 = "Package Use Link Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantLinkBrightDefect.Enabled = false;
                pnl_WantLinkDarkDefect.Enabled = false;
                pnl_WantLinkDark2Defect.Enabled = false;
                pnl_WantLinkCrackDefect.Enabled = false;
                pnl_WantLinkMoldFlashDefect.Enabled = false;
            }

            strChild3 = "Package Separate Dark Field 2 Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateDarkField2DefectSetting_Package.Enabled = false;
            }

            strChild3 = "Package Separate Dark Field 3 Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateDarkField3DefectSetting_Package.Enabled = false;
            }

            strChild3 = "Package Separate Dark Field 4 Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateDarkField4DefectSetting_Package.Enabled = false;
            }

            strChild3 = "Package Separate Crack Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateCrackDefectSetting_Package.Enabled = false;
            }

            //strChild3 = "Package Separate Void Defect Setting";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2, strChild3))
            //{
            //    chk_SeperateVoidDefectSetting_Package.Enabled = false;
            //}

            strChild3 = "Package Separate Mold Flash Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateMoldFlashDefectSetting_Package.Enabled = false;
            }

            strChild3 = "Package Separate Chipped Off Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateChippedOffDefectSetting_Package.Enabled = false;
            }

            strChild3 = "Package Dont Care Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantDontCareArea_Package.Enabled = false;
            }

            strChild3 = "Package Check Angle";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckPackageAngle_Package.Enabled = false;
            }

            strChild3 = "Package Defect Inspection Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_PackageDefectInspectionMethod.Enabled = false;
                btn_PackageGrayValueSensitivitySetting.Enabled = false;
            }

            strChild3 = "Package Square Unit";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SquareUnit.Enabled = false;
            }

            strChild3 = "Package Separate Bright And Dark Defect ROI Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateBrightDarkROITolerance_Package.Enabled = false;
            }

            strChild3 = "Package Check Package Color";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantCheckPackageColor.Enabled = false;
            }

            strChild3 = "Package Color Defect Link Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_ColorDefectLink_Package.Enabled = false;
            }

            //Barcode
            strChild3 = "Use Gain Range";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseGainRange.Enabled = false;
            }

            strChild3 = "Use Angle Range";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseAngleRange.Enabled = false;
            }

            strChild3 = "2D Code Detection Area Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_BarcodeDetectionAreaTolerance.Enabled = false;
            }

            strChild3 = "Pattern Detection Area Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_PatternDetectionAreaTolerance.Enabled = false;
            }

            strChild3 = "Restest Count";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_BarcodeRestestCount.Enabled = false;
            }

            strChild3 = "Delay Time After Pass";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_BarcodeDelayTimeAfterPass.Enabled = false;
            }

            strChild3 = "Uniformize Gain";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_UniformizeGain.Enabled = false;
            }

            strChild3 = "2D Code Dont Care Scale";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_BarcodeDontCareScale.Enabled = false;
            }

            strChild3 = "Uniformize 3x3";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseUniformize3x3.Enabled = false;
            }

            if (m_intSelectedTabPage == 0)
            {
                switch (m_smVisionInfo.g_strVisionName)
                {
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                        strChild1 = "OPad";
                        break;
                    case "IPMLi":
                    case "IPMLiPkg":
                    case "InPocket":
                    case "InPocketPkg":
                    case "InPocketPkgPos":
                        strChild1 = "Mark";
                        break;
                    //case "BottomOrientPad":
                    case "Pad":
                    case "PadPos":
                    case "PadPkg":
                    case "PadPkgPos":
                    case "Pad5S":
                    case "Pad5SPos":
                    case "Pad5SPkg":
                    case "Pad5SPkgPos":
                        strChild1 = "Pad";
                        break;
                    case "Li3D":
                    case "Li3DPkg":
                        strChild1 = "Lead3D";
                        break;
                    case "Seal":
                        strChild1 = "Seal";
                        break;
                    case "Barcode":
                        strChild1 = "Barcode";
                        break;
                }

            }
            else if (m_intSelectedTabPage == 1)
                strChild1 = "Package";
            else if (m_intSelectedTabPage == 2)
                strChild1 = "Lead";

            //General 
            strChild3 = "General Want Check Empty";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantCheckEmpty.Enabled = false;
            }

            strChild3 = "General Want Check Unit Sit Proper";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantCheckUnitSitProper.Enabled = false;
            }

            strChild3 = "General Want Use Unit PR Find Gauge";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantUseUnitPRFindGauge.Enabled = false;
            }

            if (m_intSelectedTabPage == 0)
            {
                switch (m_smVisionInfo.g_strVisionName)
                {
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                        strChild1 = "OPad";
                        break;
                    case "IPMLi":
                    case "IPMLiPkg":
                    case "InPocket":
                    case "InPocketPkg":
                    case "InPocketPkgPos":
                        strChild1 = "Mark";
                        break;
                    //case "BottomOrientPad":
                    case "Pad":
                    case "PadPos":
                    case "PadPkg":
                    case "PadPkgPos":
                    case "Pad5S":
                    case "Pad5SPos":
                    case "Pad5SPkg":
                    case "Pad5SPkgPos":
                        strChild1 = "Pad";
                        break;
                    case "Li3D":
                    case "Li3DPkg":
                        strChild1 = "Lead3D";
                        break;
                    case "Seal":
                        strChild1 = "Seal";
                        break;
                    case "Barcode":
                        strChild1 = "Barcode";
                        break;
                }

            }
            else if (m_intSelectedTabPage == 1)
                strChild1 = "Package";
            else if (m_intSelectedTabPage == 2)
                strChild1 = "Lead";

            //Pocket Position 
            strChild3 = "Want Check Pocket Position";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckPocketPosition.Enabled = false;
                chk_WantUsePocketPattern.Enabled = false;
                chk_WantUsePocketGauge.Enabled = false;
            }

            strChild3 = "Pocket Pattern Image View No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_PocketPatternAndGaugeImageNo.Enabled = false;
            }

            strChild3 = "Plate Gauge Image View No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_PlateGaugeImageNo.Enabled = false;
            }

            //Pad
            strChild3 = "Pad White On Black";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_PadWhiteOnBlack.Enabled = false;
            }

            strChild3 = "Pad Want Check PH";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantCheckPH.Enabled = false;
            }

            strChild3 = "Pad Want Check Pad";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantCheckPad.Enabled = false;
            }

            //strChild3 = "Pad Want Check Pad Color";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    pnl_WantCheckPadColor.Enabled = false;
            //}

            strChild3 = "Pad Want Consider Pad Image2 for SidePad";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantConsiderPadImage2.Enabled = false;
            }

            strChild3 = "Pad Want PR Before Gauge";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantPRUnitLocationBeforeGauge.Enabled = false;
            }

            strChild3 = "Pad Closest Method for Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_DefinePadToleranceUsingClosestMethod.Enabled = false;
            }

            strChild3 = "Pad Want Show Pin 1";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantPadPin1.Enabled = false;
            }

            strChild3 = "Pad Want Check Side Pad";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantCheck4Sides.Enabled = false;
            }

            strChild3 = "Pad Want Tight Setting for Test Run";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantTestRunTightSetting.Enabled = false;
            }

            strChild3 = "Pad Want GRR";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantGRR.Enabled = false;
            }

            strChild3 = "Pad Want CPK";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantCPK.Enabled = false;
            }

            strChild3 = "Pad Want Rotate Side Pad Image";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantRotateSidePadImage.Enabled = false;
            }

            strChild3 = "Pad Measurement Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_PadMeasurementMethod.Enabled = false;
                cbo_PadMeasurementMethod_Side.Enabled = false;
            }

            strChild3 = "Pad Sensitivity On Pad";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SensitivityOnPad.Enabled = false;
                txt_SensitivityValue.Enabled = false;
                btn_PadSensitivity.Enabled = false;
            }

            strChild3 = "Pad Tight Setting Dimension Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_TightSettingDimensionTolerance.Enabled = false;
            }

            strChild3 = "Pad Tight Setting Threshold Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_TightSettingThresholdTolerance.Enabled = false;
            }

            strChild3 = "Pad Want Individual Side Thickness";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantIndividualSideThickness.Enabled = false;
            }

            //strChild3 = "Pad Size Tolerance";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2, strChild3))
            //{
            //    txt_PadSizeHalfWidthTolerance.Enabled = false;
            //}

            //strChild3 = "Pad Default Pixel Tolerance";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    txt_DefaultPixelTolerance.Enabled = false;
            //}

            //strChild3 = "Pad ROI Tolerance";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    txt_PadROITolerance.Enabled = false;
            //}

            strChild3 = "Pad Dont Care Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantDontCareArea_Pad.Enabled = false;
            }

            strChild3 = "Broken Pad Image No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_BrokenPadImageNo.Enabled = false;
            }

            strChild3 = "Pad Edge Limit";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantEdgeLimit_Pad.Enabled = false;
            }

            strChild3 = "Pad Edge Distance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantEdgeDistance_Pad.Enabled = false;
            }

            strChild3 = "Pad Stand Off";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantStandOff_Pad.Enabled = false;
                btn_PadStandOffDirectionSetting.Enabled = false;
            }

            strChild3 = "Pad Subtract Method";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_PadSubtractMethod_Center.Enabled = false;
                cbo_PadSubtractMethod_Side.Enabled = false;
            }

            strChild3 = "Pad Offset Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_PadOffsetSetting.Enabled = false;
            }

            strChild3 = "Pad Link Different Group Pitch Gap";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantLinkDifferentGroupPitchGap.Enabled = false;
            }

            strChild3 = "Save Pad Template Image Method";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_SavePadTemplateImageMethod.Enabled = false;
            }

            strChild3 = "Pad Offset Reference Point";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_PadOffsetReferencePoint.Enabled = false;
            }

            strChild3 = "Pad View Foreign Material Option";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantViewCheckForeignMaterialOptionWhenPackageON.Enabled = false;
            }

            strChild3 = "Pad Show Use Gauge Checkbox";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_ShowUseGaugeCheckBoxInLearnForm.Enabled = false;
            }

            strChild3 = "Want Separate Broken Pad Threshold Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_WantSeparateBrokenPadThresholdSetting.Enabled = false;
            }

            strChild3 = "Pad Check Pad Color";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_WantCheckPadColor.Enabled = false;
            }

            strChild3 = "Pad Color Group Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_PadColorGroupSetting.Enabled = false;
            }

            strChild3 = "Pad Color Defect Link Method";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                pnl_ColorDefectLink_Pad.Enabled = false;
            }

            //Pad Orient

            strChild2 = "Pad Orient Direction";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                group_PadOrientDirections.Enabled = false;
            }

            //PadPackage
            //strChild3 = "Pad Package Use Detail Threshold Setting Form";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_WantUseDetailThreshold_PadPackage.Enabled = false;
            //}

            strChild3 = "Pad Package Measure Center Pkg Size Using Side Pkg";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_MeasureCenterPkgSizeUsingSidePkg.Enabled = false;
            }

            strChild3 = "Pad Package Separate Crack Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateCrackDefectSetting_PadPackage.Enabled = false;
            }
            
            strChild3 = "Pad Package Separate Chipped Off Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateChippedOffDefectSetting_PadPackage.Enabled = false;
            }

            strChild3 = "Pad Package Separate Mold Flash Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateMoldFlashDefectSetting_PadPackage.Enabled = false;
            }

            strChild3 = "Pad Package Seperate Foreign Material Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateForeignMaterialDefectSetting_PadPackage.Enabled = false;
            }

            strChild3 = "Pad Package Dont Care Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantDontCareArea_PadPackage.Enabled = false;
            }

            strChild3 = "Pad Package Separate Bright And Dark Defect ROI Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SeperateBrightDarkROITolerance_PadPackage.Enabled = false;
            }

            //strChild3 = "Pad Package Size Image No";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    cbo_PadPkgSizeImageViewNo_Center.Enabled = false;
            //    cbo_PadPkgSizeImageViewNo_Side.Enabled = false;
            //}

            strChild3 = "Pad Package Bright Field Image No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_PadPkgBrightFieldImageViewNo_Center.Enabled = false;
                cbo_PadPkgBrightFieldImageViewNo_Side.Enabled = false;
            }

            strChild3 = "Pad Package Dark Field Image No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_PadPkgDarkFieldImageNo_Center.Enabled = false;
                cbo_PadPkgDarkFieldImageNo_Side.Enabled = false;
            }

            strChild3 = "Pad Package Mold Flash Defect Image View No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_PadPkgMoldFlashDefectImageView.Enabled = false;
            }

            strChild3 = "Pad Package Use Link Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantLinkBrightDefect_PadPackage.Enabled = false;
                pnl_WantLinkDarkDefect_PadPackage.Enabled = false;
                pnl_WantLinkCrackDefect_PadPackage.Enabled = false;
                pnl_WantLinkMoldFlashDefect_PadPackage.Enabled = false;
            }

            //Lead3D
            strChild3 = "Lead3D Want Use Center Package To Base Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantPkgToBaseTolerance_Lead3D.Enabled = false;
            }

            strChild3 = "Lead3D Want Show Pin 1";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantLead3DPin1.Enabled = false;
            }

            strChild3 = "Lead3D Want Check PH";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantCheckPH_Lead3D.Enabled = false;
            }

            strChild3 = "Lead3D Want GRR";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantGRR_Lead3D.Enabled = false;
            }

            strChild3 = "Lead3D Width Display Option";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_Lead3DWidthDisplayOption.Enabled = false;
            }

            strChild3 = "Lead3D Length Variance Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_Lead3DLengthVarianceMethod.Enabled = false;
            }

            strChild3 = "Lead3D Span Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_Lead3DSpanMethod.Enabled = false;
            }

            strChild3 = "Lead3D Contamination Region";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_Lead3DContaminationRegion.Enabled = false;
            }

            strChild3 = "Lead3D Stand Off Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_Lead3DStandOffMethod.Enabled = false;
            }

            strChild3 = "Lead3D Width Range Selection";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_LeadWidthRangeSelection.Enabled = false;
            }

            strChild3 = "Lead3D Use Average Gray Value Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_LeadAverageGrayValue.Enabled = false;
            }

            strChild3 = "Lead3D Dont Care Area"; //strChild3 = Do not use ’ sintax bcos it is chinese language and will create error. 
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantDontCareArea_Lead3D.Enabled = false;
            }

            strChild3 = "Lead3D Image Rotate Option";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_ImageRotateOption_Lead3D.Enabled = false;
            }

            strChild3 = "Lead3D Matching Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MatchingXTolerance_Lead3D.Enabled = false;
                txt_MatchingYTolerance_Lead3D.Enabled = false;
            }

            //Lead3D Package
            strChild3 = "Measure Center Pkg Size Using Corner";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_Lead3DPkg_UseCornerToGetCenterPkgEdge.Enabled = false;
            }

            strChild3 = "Lead3D Package Separate Crack Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_SeperateCrackDefectSetting_LeadPackage.Enabled = false;
            }

            strChild3 = "Lead3D Package Separate Mold Flash Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_SeperateChippedOffDefectSetting_LeadPackage.Enabled = false;
            }

            strChild3 = "Lead3D Package Separate Chipped Off Defect Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_SeperateMoldFlashDefectSetting_LeadPackage.Enabled = false;
            }

            strChild3 = "Lead3D Package Bright Field Image No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_LeadPkgBrightFieldImageNo.Enabled = false;
            }

            strChild3 = "Lead3D Package Dark Field Image No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_LeadPkgDarkFieldImageNo.Enabled = false;
            }

            strChild3 = "Lead3D Package Dont Care Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_WantDontCareArea_LeadPackage.Enabled = false;
            }

            //Lead
            strChild3 = "Lead Defect Image View No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_LeadDefectImageNo.Enabled = false;
            }

            strChild3 = "Want Check Lead";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckLead.Enabled = false;
            }

            //strChild3 = "Use Gauge Measure Lead Dimension";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_WantUseGaugeMeasureLeadDimension.Enabled = false;
            //}

            strChild3 = "Want Use Center Package To Base Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantPkgToBaseTolerance_Lead.Enabled = false;
            }

            strChild3 = "Use Average Gray Value Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantUseAverageGrayValueMethod_Lead.Enabled = false;
                btn_AverageGrayValueROITolerance_Lead.Enabled = false;
                chk_WantUseMasking_Lead.Enabled = false;
            }

            strChild3 = "Lead Rotation Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_RotationMethod_Lead.Enabled = false;
            }

            strChild3 = "Dont Care Area Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                grp_PocketDontCare.Enabled = false;
            }

            strChild3 = "Base Lead Image View No";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_BaseLeadImageNo.Enabled = false;
            }

            strChild3 = "Inspect Base Lead";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantInspectBaseLead.Enabled = false;
            }

            strChild3 = "Lead Angle Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_LeadAngleTolerance.Enabled = false;
            }

            //Seal 
            strChild3 = "Seal Orientation Direction";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                group_OrientDirectionsSeal.Enabled = false;
            }

            strChild3 = "Seal White On Black";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SealWhiteOnBlack.Enabled = false;
            }

            //strChild3 = "Seal Package Dimension";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    cbo_Package.Enabled = false;
            //}

            strChild3 = "Seal Pocket Pitch Dimension";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_intPocketPitch.Enabled = false;
            }

            //strChild3 = "Seal Skip Orient Inspection";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            //{
            //    chk_SkipOrientInspection.Enabled = false;
            //}

            strChild3 = "Seal Skip Sprocket Hole Distance Inspection";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SkipSprocketHoleInspection.Enabled = false;
            }

            strChild3 = "Seal Skip Sprocket Hole Diameter And Defect Inspection";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SkipSprocketHoleDiameterAndDefectInspection.Enabled = false;
            }

            strChild3 = "Seal Skip Sprocket Hole Broken And Roundness Inspection";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_SkipSprocketHoleBrokenAndRoundnessInspection.Enabled = false;
            }

            strChild3 = "Seal Check Unit Present Method";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_CheckSealMarkMethod.Enabled = false;
                srmCheckBox2.Enabled = false;
                srmCheckBox1.Enabled = false;
                txt_PatternAngleTolerance.Enabled = false;
            }

            strChild3 = "Seal Dont Care Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantDontCareArea_Seal.Enabled = false;
            }

            strChild3 = "Seal Mark Area Below Percent";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MarkAreaBelowPercent.Enabled = false;
            }

            strChild3 = "Seal Clear Seal Template When New Lot";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantClearSealTemplateWhenNewLot.Enabled = false;
            }

            strChild3 = "Seal Sprocket Hole Image View";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SprocketHoleViewNo.Enabled = false;
            }

            strChild3 = "Seal Distance Image View";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SealDistanceViewNo.Enabled = false;
            }

            strChild3 = "Seal Broken Image View";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SealBrokenViewNo.Enabled = false;
            }

            strChild3 = "Seal Overheat Image View";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SealOverheatViewNo.Enabled = false;
            }

            strChild3 = "Seal Check Unit Image View";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_CheckUnitViewNo.Enabled = false;
            }
            
            strChild3 = "Seal Want Check Seal Edge Straightness";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_WantCheckSealEdgeStraightness.Enabled = false;
            }

            strChild3 = "Seal Edge Straightness Image View";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SealEdgeStraightnessViewNo.Enabled = false;
            }

            strChild3 = "Maximum Seal Mark Template";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MaxSealMarkTemplate.Enabled = false;
            }

            strChild3 = "Maximum Seal Empty Template";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MaxSealEmptyTemplate.Enabled = false;
            }

            strChild3 = "Seal Sprocket Hole Position";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_SealSprocketHolePosition.Enabled = false;
            }

        }
        private void DisableTabPage()
        {
            switch (m_intSelectedTabPage)
            {
                case 0:
                    if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0 || (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM")) || m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Orient);
                    }
                    tabControl_SettingForm.Controls.Remove(tab_Package);
                    tabControl_SettingForm.Controls.Remove(tab_Package2);
                    tabControl_SettingForm.Controls.Remove(tab_Package3);
                    tabControl_SettingForm.Controls.Remove(tab_Lead);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage2);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3DPackage);
                    if (!tabControl_SettingForm.TabPages.Contains(tab_Orient) && tabControl_SettingForm.TabPages.Contains(tab_Mark))// && (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM")))
                    {
                        tab_Mark3.Controls.Add(gb_Pin1);//tab_Mark //chk_WantPin1
                        gb_Pin1.Location = new Point(6, chk_WantUseExcessMissingMarkAffectScore.Location.Y + chk_WantUseExcessMissingMarkAffectScore.Size.Height + 7);// srmLabel117 //chk_WantPin1 //srmLabel19
                    }
                    if ((m_smCustomizeInfo.g_intWantLead3D & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Lead3D);
                        tabControl_SettingForm.Controls.Remove(tab_Lead3D2);
                    }
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tp_Barcode);
                        tabControl_SettingForm.Controls.Remove(tab_Orient);
                        tabControl_SettingForm.Controls.Remove(tab_Mark);
                        tabControl_SettingForm.Controls.Remove(tab_Mark2);
                        tabControl_SettingForm.Controls.Remove(tab_Mark3);
                        tabControl_SettingForm.Controls.Remove(tab_Pad);
                        tabControl_SettingForm.Controls.Remove(tab_Pad2);
                        tabControl_SettingForm.Controls.Remove(tab_Pad3);
                        tabControl_SettingForm.Controls.Remove(tab_General);
                        tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                        tabControl_SettingForm.Controls.Remove(tab_Seal);
                        tabControl_SettingForm.Controls.Remove(tab_BottomPosition);
                        tabControl_SettingForm.Controls.Remove(tab_Lead3DPackage);
                        tabControl_SettingForm.Controls.Remove(tp_Seal2);
                    }

                    if ((m_smCustomizeInfo.g_intWantBarcode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        tabControl_SettingForm.Controls.Remove(tp_Barcode);
                    }
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Orient);
                        tabControl_SettingForm.Controls.Remove(tab_Mark);
                        tabControl_SettingForm.Controls.Remove(tab_Mark2);
                        tabControl_SettingForm.Controls.Remove(tab_Mark3);
                        tabControl_SettingForm.Controls.Remove(tab_Pad);
                        tabControl_SettingForm.Controls.Remove(tab_Pad2);
                        tabControl_SettingForm.Controls.Remove(tab_Pad3);
                        tabControl_SettingForm.Controls.Remove(tab_General);
                        tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                        tabControl_SettingForm.Controls.Remove(tab_Seal);
                        tabControl_SettingForm.Controls.Remove(tab_BottomPosition);
                        tabControl_SettingForm.Controls.Remove(tab_Lead3DPackage);
                        tabControl_SettingForm.Controls.Remove(tp_Seal2);
                    }
                    break;
                case 1:
                    tabControl_SettingForm.Controls.Remove(tp_Barcode);
                    tabControl_SettingForm.Controls.Remove(tab_Orient);
                    tabControl_SettingForm.Controls.Remove(tab_Mark);
                    tabControl_SettingForm.Controls.Remove(tab_Mark2);
                    tabControl_SettingForm.Controls.Remove(tab_Mark3);
                    tabControl_SettingForm.Controls.Remove(tab_Pad);
                    tabControl_SettingForm.Controls.Remove(tab_Pad2);
                    tabControl_SettingForm.Controls.Remove(tab_Pad3);
                    tabControl_SettingForm.Controls.Remove(tab_Lead);
                    tabControl_SettingForm.Controls.Remove(tab_General);
                    tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3D);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3D2);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3DPackage);
                    break;
                case 2:
                    tabControl_SettingForm.Controls.Remove(tp_Barcode);
                    tabControl_SettingForm.Controls.Remove(tab_Orient);
                    tabControl_SettingForm.Controls.Remove(tab_Mark);
                    tabControl_SettingForm.Controls.Remove(tab_Mark2);
                    tabControl_SettingForm.Controls.Remove(tab_Mark3);
                    tabControl_SettingForm.Controls.Remove(tab_Pad);
                    tabControl_SettingForm.Controls.Remove(tab_Pad2);
                    tabControl_SettingForm.Controls.Remove(tab_Pad3);
                    tabControl_SettingForm.Controls.Remove(tab_Package);
                    tabControl_SettingForm.Controls.Remove(tab_Package2);
                    tabControl_SettingForm.Controls.Remove(tab_Package3);
                    tabControl_SettingForm.Controls.Remove(tab_General);
                    tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage2);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3D);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3D2);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3DPackage);
                    break;
                case 3:
                    tabControl_SettingForm.Controls.Remove(tp_Barcode);
                    tabControl_SettingForm.Controls.Remove(tab_Orient);
                    tabControl_SettingForm.Controls.Remove(tab_Mark);
                    tabControl_SettingForm.Controls.Remove(tab_Mark2);
                    tabControl_SettingForm.Controls.Remove(tab_Mark3);
                    tabControl_SettingForm.Controls.Remove(tab_Pad);
                    tabControl_SettingForm.Controls.Remove(tab_Pad2);
                    tabControl_SettingForm.Controls.Remove(tab_Pad3);
                    tabControl_SettingForm.Controls.Remove(tab_Package);
                    tabControl_SettingForm.Controls.Remove(tab_Package2);
                    tabControl_SettingForm.Controls.Remove(tab_Package3);
                    tabControl_SettingForm.Controls.Remove(tab_General);
                    tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage2);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3D);
                    tabControl_SettingForm.Controls.Remove(tab_Lead3D2);
                    tabControl_SettingForm.Controls.Remove(tab_Lead);
                    tabControl_SettingForm.Controls.Remove(tab_Seal);
                    tabControl_SettingForm.Controls.Remove(tab_BottomPosition);
                    tabControl_SettingForm.Controls.Remove(tp_Seal2);
                    break;
            }
        }

        private void UpdateGUI()
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Barcode":
                    tabControl_SettingForm.Controls.Remove(tp_OrientPad);
                    UpdateBarcodeGUI();
                    break;
                case "Li3DPkg":
                    tabControl_SettingForm.Controls.Remove(tp_OrientPad);
                    UpdateLead3DGUI();
                    UpdateLead3DPackageGUI();
                    break;
                case "Li3D":
                    tabControl_SettingForm.Controls.Remove(tp_OrientPad);
                    UpdateLead3DGUI();
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0 || (m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_intSelectedTabPage == 1)
                        tabControl_SettingForm.Controls.Remove(tp_OrientPad);
                    tabControl_SettingForm.Controls.Remove(tab_Orient);
                    tabControl_SettingForm.Controls.Remove(tab_Mark);
                    tabControl_SettingForm.Controls.Remove(tab_Mark2);
                    tabControl_SettingForm.Controls.Remove(tab_Mark3);
                    tabControl_SettingForm.Controls.Remove(tab_Package);
                    tabControl_SettingForm.Controls.Remove(tab_Package2);
                    tabControl_SettingForm.Controls.Remove(tab_Package3);
                    tabControl_SettingForm.Controls.Remove(tab_BottomPosition);
                    tabControl_SettingForm.Controls.Remove(tab_General);
                    tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                    tabControl_SettingForm.Controls.Remove(tab_Seal);
                    tabControl_SettingForm.Controls.Remove(tp_Seal2);
                    UpdatePadGUI();

                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePadPackageGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_PadPackage);
                        tabControl_SettingForm.Controls.Remove(tab_PadPackage2);
                    }


                    break;
                case "Orient":
                case "BottomOrient":
                case "BottomPosition":
                case "Mark":
                case "MarkOrient":
                case "MarkPkg":
                case "MOPkg":
                case "MOLi":
                case "MOLiPkg":
                case "Package":
                case "Seal":
                    tabControl_SettingForm.Controls.Remove(tp_OrientPad);

                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateOrientGUI();

                        if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        {
                            /// 2019 06 21 - No orient direction setting if Want Orient 0 deg only
                            //if (!tab_Mark.Controls.Contains(group_OrientDirections))
                            //    tab_Mark.Controls.Add(group_OrientDirections);
                            //group_OrientDirections.Location = new Point(group_OrientDirections.Location.X, 230);
                        }
                        else
                        {
                            if (tab_Mark.Controls.Contains(group_OrientDirections))
                                tab_Mark.Controls.Remove(group_OrientDirections);
                        }
                    }
                    else
                        tabControl_SettingForm.Controls.Remove(tab_Orient);

                    if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateMarkGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Mark);
                        tabControl_SettingForm.Controls.Remove(tab_Mark2);
                        tabControl_SettingForm.Controls.Remove(tab_Mark3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPad & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePadGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Pad);
                        tabControl_SettingForm.Controls.Remove(tab_Pad2);
                        tabControl_SettingForm.Controls.Remove(tab_Pad3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePackageGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Package);
                        tabControl_SettingForm.Controls.Remove(tab_Package2);
                        tabControl_SettingForm.Controls.Remove(tab_Package3);
                    }

                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                        UpdateOrientGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_BottomPosition);

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateLeadGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_Lead);

                    if ((m_smCustomizeInfo.g_intWantSeal & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateSealGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Seal);
                        tabControl_SettingForm.Controls.Remove(tp_Seal2);
                    }

                    if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        pnl_2DCode.Visible = false;
                    chk_WantCheckPocketPosition.Checked = false;
                    chk_WantUsePocketPattern.Checked = false;
                    chk_WantUsePocketGauge.Checked = false;
                    chk_WantCheckEmpty.Checked = false;
                    chk_WantUseEmptyPattern.Checked = false;
                    chk_WantUseEmptyThreshold.Checked = false;
                    chk_WantCheckUnitSitProper.Checked = false;
                    chk_UseAutoRepalceCounter.Checked = false;
                    pnl_WantCheckEmpty.Visible = false;
                    pnl_WantCheckPocketPosition.Visible = false;
                    pnl_WantCheckUnitSitProper.Visible = false;
                    pnl_UseAutoRepalceCounter.Visible = false;
                    tabControl_SettingForm.Controls.Remove(tab_General);
                    tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage2);
                    break;
                case "IPMLi":
                    tabControl_SettingForm.Controls.Remove(tp_OrientPad);
                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateOrientGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_Orient);

                    if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateMarkGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Mark);
                        tabControl_SettingForm.Controls.Remove(tab_Mark2);
                        tabControl_SettingForm.Controls.Remove(tab_Mark3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPad & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePadGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Pad);
                        tabControl_SettingForm.Controls.Remove(tab_Pad2);
                        tabControl_SettingForm.Controls.Remove(tab_Pad3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePackageGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Package);
                        tabControl_SettingForm.Controls.Remove(tab_Package2);
                        tabControl_SettingForm.Controls.Remove(tab_Package3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPositioning & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateOrientGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_BottomPosition);

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateLeadGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_Lead);

                    if ((m_smCustomizeInfo.g_intWantSeal & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateSealGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Seal);
                        tabControl_SettingForm.Controls.Remove(tp_Seal2);
                    }

                    if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        pnl_2DCode.Visible = false;

                    tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage2);
                    chk_WantCheckEmpty.Checked = m_smVisionInfo.g_blnWantCheckEmpty;
                    chk_WantUseEmptyPattern.Checked = m_smVisionInfo.g_blnWantUseEmptyPattern;
                    chk_WantUseEmptyThreshold.Checked = m_smVisionInfo.g_blnWantUseEmptyThreshold;
                    chk_WantUseEmptyPattern.Enabled = chk_WantCheckEmpty.Checked;
                    chk_WantUseEmptyThreshold.Enabled = chk_WantCheckEmpty.Checked;

                    chk_WantUnitPRFindGauge.Visible = chk_WantUnitPRFindGauge.Checked = false;
                    chk_WantCheckUnitSitProper.Visible = chk_WantCheckUnitSitProper.Checked = false;
                    chk_UseAutoRepalceCounter.Visible = chk_UseAutoRepalceCounter.Checked = false;
                    break;
                case "InPocket":
                    tabControl_SettingForm.Controls.Remove(tp_OrientPad);
                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateOrientGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_Orient);

                    if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateMarkGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Mark);
                        tabControl_SettingForm.Controls.Remove(tab_Mark2);
                        tabControl_SettingForm.Controls.Remove(tab_Mark3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPad & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePadGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Pad);
                        tabControl_SettingForm.Controls.Remove(tab_Pad2);
                        tabControl_SettingForm.Controls.Remove(tab_Pad3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePackageGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Package);
                        tabControl_SettingForm.Controls.Remove(tab_Package2);
                        tabControl_SettingForm.Controls.Remove(tab_Package3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPositioning & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateOrientGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_BottomPosition);

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateLeadGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_Lead);

                    if ((m_smCustomizeInfo.g_intWantSeal & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateSealGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Seal);
                        tabControl_SettingForm.Controls.Remove(tp_Seal2);
                    }

                    if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        pnl_2DCode.Visible = false;

                    chk_WantCheckPocketPosition.Checked = false;
                    chk_WantUsePocketPattern.Checked = false;
                    chk_WantUsePocketGauge.Checked = false;
                    chk_WantCheckEmpty.Checked = false;
                    chk_WantUseEmptyPattern.Checked = false;
                    chk_WantUseEmptyThreshold.Checked = false;
                    chk_WantCheckUnitSitProper.Checked = false;
                    chk_UseAutoRepalceCounter.Checked = false;
                    pnl_WantCheckEmpty.Visible = false;
                    pnl_WantCheckPocketPosition.Visible = false;
                    pnl_WantCheckUnitSitProper.Visible = false;
                    pnl_UseAutoRepalceCounter.Visible = false;
                    tabControl_SettingForm.Controls.Remove(tab_General);
                    tabControl_SettingForm.Controls.Remove(tab_PocketPosition);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage2);
                    break;
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLiPkg":
                default:
                    tabControl_SettingForm.Controls.Remove(tp_OrientPad);
                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateOrientGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_Orient);

                    if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateMarkGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Mark);
                        tabControl_SettingForm.Controls.Remove(tab_Mark2);
                        tabControl_SettingForm.Controls.Remove(tab_Mark3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPad & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePadGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Pad);
                        tabControl_SettingForm.Controls.Remove(tab_Pad2);
                        tabControl_SettingForm.Controls.Remove(tab_Pad3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdatePackageGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Package);
                        tabControl_SettingForm.Controls.Remove(tab_Package2);
                        tabControl_SettingForm.Controls.Remove(tab_Package3);
                    }

                    if ((m_smCustomizeInfo.g_intWantPositioning & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateOrientGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_BottomPosition);

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateLeadGUI();
                    else
                        tabControl_SettingForm.Controls.Remove(tab_Lead);

                    if ((m_smCustomizeInfo.g_intWantSeal & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        UpdateSealGUI();
                    else
                    {
                        tabControl_SettingForm.Controls.Remove(tab_Seal);
                        tabControl_SettingForm.Controls.Remove(tp_Seal2);
                    }

                    if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        pnl_2DCode.Visible = false;



                    UpdatePocketPositionGUI();



                    tabControl_SettingForm.Controls.Remove(tab_PadPackage);
                    tabControl_SettingForm.Controls.Remove(tab_PadPackage2);
                    chk_WantUnitPRFindGauge.Checked = m_smVisionInfo.g_blnWantUseUnitPRFindGauge;
                    chk_WantCheckEmpty.Checked = m_smVisionInfo.g_blnWantCheckEmpty;
                    chk_WantUseEmptyPattern.Checked = m_smVisionInfo.g_blnWantUseEmptyPattern;
                    chk_WantUseEmptyThreshold.Checked = m_smVisionInfo.g_blnWantUseEmptyThreshold;
                    chk_WantUseEmptyPattern.Enabled = chk_WantCheckEmpty.Checked;
                    chk_WantUseEmptyThreshold.Enabled = chk_WantCheckEmpty.Checked;
                    chk_WantCheckUnitSitProper.Checked = m_smVisionInfo.g_blnWantCheckUnitSitProper;
                    chk_UseAutoRepalceCounter.Checked = m_smVisionInfo.g_blnUseAutoRepalceCounter;
                    break;

            }

        }

        private void UpdateOrientGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            radioBtn_2Directions.Checked = (objFileHandle.GetValueAsInt("Direction", 4) == 2);
            radioBtn_4Directions.Checked = (objFileHandle.GetValueAsInt("Direction", 4) == 4);
            chk_WantSubROI.Checked = objFileHandle.GetValueAsBoolean("WantSubROI", false);
            chk_WantUsePositionCheckOrientation.Checked = objFileHandle.GetValueAsBoolean("WantUsePositionCheckOrientation", false);
            txt_CheckPosOrientBlowDifferentScore.Text = (objFileHandle.GetValueAsFloat("CheckPositionOrientationWhenBelowDifferentScore", 0.1f) * 100).ToString();
            
            // if vision just orient
            if (((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) == 0) &&
                ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
            {
                //chk_WantSubROI.Checked = false;
                //chk_WantSubROI.Visible = false;
                chk_WantPin1.Checked = false;
                chk_WantPin1.Visible = false;
                chk_WantUsePin1OrientationWhenNoMark.Checked = false;
                chk_WantUsePin1OrientationWhenNoMark.Visible = false;
                gb_Pin1.Visible = false;
            }
            else if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                chk_WantUsePin1OrientationWhenNoMark.Visible = true;

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
            {
                chk_PositionWantGauge.Visible = true;
                chk_PositionWantGauge.Checked = objFileHandle.GetValueAsBoolean("PositionWantGauge", false);
            }

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomOrient"))
            {
                chk_OrientWantPackage.Visible = true;
                chk_OrientWantPackage.Checked = objFileHandle.GetValueAsBoolean("OrientWantGauge", false);
                chk_WantPin1.Checked = false;
                chk_WantPin1.Visible = false;
                chk_WantUsePin1OrientationWhenNoMark.Checked = false;
                chk_WantUsePin1OrientationWhenNoMark.Visible = false;
                gb_Pin1.Visible = false;
            }

            //Disable this, for bottom orient always must learn ROI
            chk_WantSubROI.Checked = false;
            chk_WantSubROI.Visible = false;

            if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                group_OrientDirections.Visible = false;  // Not allow to rotate image 90 deg since want orient 0 deg only
            }
            else if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
            {
                group_OrientDirections.Visible = false;  // No allow to rotate iamge 90 deg because InPocket accept 1 direction only
            }
        }

        private void UpdatePackageGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\Package\\Settings.xml";

            // 2019 07 21 - CCENG: Should display this check box as long as when have package 
            if (m_smVisionInfo.g_strVisionName.Contains("Pkg"))
                //if (m_smVisionInfo.g_strVisionName == "MOPkg"|| m_smVisionInfo.g_strVisionName == "InPocketPkg")
                chk_CheckVoidOnMarkArea.Visible = true;
            else
                chk_CheckVoidOnMarkArea.Visible = false;

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WantPackageGRR.Checked = objFileHandle.GetValueAsBoolean("WantShowGRR", false);
            m_smVisionInfo.g_blnWantUseSideLightGauge = chk_WantUseSideLightGauge.Checked = objFileHandle.GetValueAsBoolean("WantUseSideLightGauge", false);
            m_smVisionInfo.g_blnWantCheckVoidOnMarkArea = chk_CheckVoidOnMarkArea.Checked = objFileHandle.GetValueAsBoolean("WantCheckVoidOnMark", false);
            m_smVisionInfo.g_blnWantUseDetailThreshold_Package = chk_WantUseDetailThreshold_Package.Checked = objFileHandle.GetValueAsBoolean("WantUseDetailThresholdPackage", false);
            m_smVisionInfo.g_blnWantDontCareArea_Package = chk_WantDontCareArea_Package.Checked = objFileHandle.GetValueAsBoolean("WantDontCareAreaPackage", false);
            m_smVisionInfo.g_blnWantCheckPackageAngle = chk_WantCheckPackageAngle_Package.Checked = objFileHandle.GetValueAsBoolean("WantCheckPackageAngle", false);
            m_smVisionInfo.g_blnSquareUnit = chk_SquareUnit.Checked = objFileHandle.GetValueAsBoolean("SquareUnit", false);

            for (int i = 0; i < 3; i++)
            {
                m_smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[i] = objFileHandle.GetValueAsBoolean("WantCheckVoidOnMark_SideLight" + i, false);
            }

            chk_CheckVoidOnMarkArea_Dark2.Checked = m_smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[0];
            chk_CheckVoidOnMarkArea_Dark3.Checked = m_smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[1];
            chk_CheckVoidOnMarkArea_Dark4.Checked = m_smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[2];
            m_smVisionInfo.g_intPackageDefectInspectionMethod = cbo_PackageDefectInspectionMethod.SelectedIndex = objFileHandle.GetValueAsInt("PackageDefectInspectionMethod", 0);
            //btn_PackageGrayValueSensitivitySetting.Visible = (cbo_PackageDefectInspectionMethod.SelectedIndex == 1);
            chk_WantCheckPackageColor.Checked = objFileHandle.GetValueAsBoolean("WantCheckPackageColor", true);
            pnl_WantCheckPackageColor.Visible = chk_WantCheckPackageColor.Visible = ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0);
            cbo_ColorDefectLinkMethod_Package.SelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intColorDefectLinkMethod;
            pnl_ColorDefectLink_Package.Visible = ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0);
            txt_ColorDefectLinkTolerance_Package.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intColorDefectLinkTolerance.ToString();
            
            if (cbo_ColorDefectLinkMethod_Package.SelectedIndex == 0)
            {
                pnl_ColorDefectLinkTolerance_Package.Visible = false;
            }
            else
            {
                pnl_ColorDefectLinkTolerance_Package.Visible = true;
            }
            txt_BrightDefectLinkTolerance.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intBrightDefectLinkTolerance.ToString();
            txt_DarkDefectLinkTolerance.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkDefectLinkTolerance.ToString();
            txt_Dark2DefectLinkTolerance.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDark2DefectLinkTolerance.ToString();
            txt_Dark3DefectLinkTolerance.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDark3DefectLinkTolerance.ToString();
            txt_Dark4DefectLinkTolerance.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDark4DefectLinkTolerance.ToString();
            txt_CrackDefectLinkTolerance.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackDefectLinkTolerance.ToString();
            txt_MoldFlashDefectLinkTolerance.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashDefectLinkTolerance.ToString();
            chk_WantLinkBrightDefect.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnWantLinkBrightDefect;
            chk_WantLinkDarkDefect.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnWantLinkDarkDefect;
            chk_WantLinkDark2Defect.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnWantLinkDark2Defect;
            chk_WantLinkDark3Defect.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnWantLinkDark3Defect;
            chk_WantLinkDark4Defect.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnWantLinkDark4Defect;
            chk_WantLinkCrackDefect.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnWantLinkCrackDefect;
            chk_WantLinkMoldFlashDefect.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnWantLinkMoldFlashDefect;
            pnl_WantLinkDark2Defect.Visible = pnl_SideLightPackageDarkImage.Visible = chk_SeperateDarkField2DefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting;
            pnl_WantLinkDark3Defect.Visible = pnl_SideLightPackageDark2Image.Visible = chk_SeperateDarkField3DefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting;
            pnl_WantLinkDark4Defect.Visible = pnl_SideLightPackageDark3Image.Visible = chk_SeperateDarkField4DefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting;
            pnl_WantLinkCrackDefect.Visible = chk_SeperateCrackDefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting;
            chk_SeperateVoidDefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateVoidDefectSetting;
            pnl_WantLinkMoldFlashDefect.Visible = pnl_MoldFlashDefectImageView.Visible = chk_SeperateMoldFlashDefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting;
            chk_SeperateChippedOffDefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting;
            chk_SeperateBrightDarkROITolerance_Package.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateBrightDarkROITolerance;
            pnl_SideLightPackageDarkImage.Visible = chk_SeperateDarkField2DefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[0].ref_blnSeperateDarkField2DefectSetting;
            pnl_SideLightPackageDark2Image.Visible = chk_SeperateDarkField3DefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[0].ref_blnSeperateDarkField3DefectSetting;
            pnl_SideLightPackageDark3Image.Visible = chk_SeperateDarkField4DefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[0].ref_blnSeperateDarkField4DefectSetting;
            chk_SeperateCrackDefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[0].ref_blnSeperateCrackDefectSetting;
            chk_SeperateVoidDefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[0].ref_blnSeperateVoidDefectSetting;
            chk_SeperateMoldFlashDefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[0].ref_blnSeperateMoldFlashDefectSetting;
            chk_SeperateChippedOffDefectSetting_Package.Checked = m_smVisionInfo.g_arrPackage[0].ref_blnSeperateChippedOffDefectSetting;
            cbo_ChippedOffDefectInspectionMethod_Package.SelectedIndex = m_smVisionInfo.g_arrPackage[0].ref_intChippedOffDefectInspectionMethod;
            pnl_ChippedOffDefectInspectionMethod_Package.Visible = m_smVisionInfo.g_arrPackage[0].ref_blnSeperateChippedOffDefectSetting;

            cbo_PackageSizeImageNo.Items.Clear();
            cbo_SideLightPackageBrightDefectImageNo.Items.Clear();
            cbo_SideLightPackageDarkDefectImageNo.Items.Clear();
            cbo_SideLightPackageDarkDefect2ImageNo.Items.Clear();
            cbo_SideLightPackageDarkDefect3ImageNo.Items.Clear();
            cbo_DirectLightPackageDefectImageNo.Items.Clear();
            cbo_MoldFlashDefectImageNo.Items.Clear();
            cbo_PackageSizeImageNo.Items.Add("Image 1");
            cbo_SideLightPackageBrightDefectImageNo.Items.Add("Image 1");
            cbo_SideLightPackageDarkDefectImageNo.Items.Add("Image 1");
            cbo_SideLightPackageDarkDefect2ImageNo.Items.Add("Image 1");
            cbo_SideLightPackageDarkDefect3ImageNo.Items.Add("Image 1");
            cbo_DirectLightPackageDefectImageNo.Items.Add("Image 1");
            cbo_MoldFlashDefectImageNo.Items.Add("Image 1");
            for (int i = 2; i <= m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_PackageSizeImageNo.Items.Add("Image " + i.ToString());
                cbo_SideLightPackageBrightDefectImageNo.Items.Add("Image " + i.ToString());
                cbo_SideLightPackageDarkDefectImageNo.Items.Add("Image " + i.ToString());
                cbo_SideLightPackageDarkDefect2ImageNo.Items.Add("Image " + i.ToString());
                cbo_SideLightPackageDarkDefect3ImageNo.Items.Add("Image " + i.ToString());
                cbo_DirectLightPackageDefectImageNo.Items.Add("Image " + i.ToString());
                cbo_MoldFlashDefectImageNo.Items.Add("Image " + i.ToString());
            }

            int intSelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);
            if (intSelectedIndex < cbo_PackageSizeImageNo.Items.Count)
                cbo_PackageSizeImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_PackageSizeImageNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2);
            if (intSelectedIndex < cbo_SideLightPackageBrightDefectImageNo.Items.Count)
                cbo_SideLightPackageBrightDefectImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SideLightPackageBrightDefectImageNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(4);
            if (intSelectedIndex < cbo_SideLightPackageDarkDefectImageNo.Items.Count)
                cbo_SideLightPackageDarkDefectImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SideLightPackageDarkDefectImageNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(6);
            if (intSelectedIndex < cbo_SideLightPackageDarkDefect2ImageNo.Items.Count)
                cbo_SideLightPackageDarkDefect2ImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SideLightPackageDarkDefect2ImageNo.SelectedIndex = 0;


            intSelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(7);
            if (intSelectedIndex < cbo_SideLightPackageDarkDefect3ImageNo.Items.Count)
                cbo_SideLightPackageDarkDefect3ImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SideLightPackageDarkDefect3ImageNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);
            if (intSelectedIndex < cbo_DirectLightPackageDefectImageNo.Items.Count)
                cbo_DirectLightPackageDefectImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_DirectLightPackageDefectImageNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5);
            if (intSelectedIndex < cbo_MoldFlashDefectImageNo.Items.Count)
                cbo_MoldFlashDefectImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_MoldFlashDefectImageNo.SelectedIndex = 0;

            if (m_intUserGroup != 1)    // SRM User
            {
                chk_WantPackageGRR.Visible = false;
            }

            strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
        m_smVisionInfo.g_strVisionFolderName + "\\General.xml";

            //XmlParser objFileHandle = new XmlParser(strPath);
            //objFileHandle.GetFirstSection("PackageSetting");
            //chk_CheckPackage.Checked = objFileHandle.GetValueAsBoolean("CheckPackage", false);
        }

        private void UpdateMarkGUI()
        {
            m_blnWantSkipMarkPrev = chk_SkipMarkInspection.Checked;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WhiteOnBlack.Checked = objFileHandle.GetValueAsBoolean("WhiteOnBlack", true);
            chk_MultiGroups.Checked = objFileHandle.GetValueAsBoolean("WantMultiGroups", false);
            chk_WantBuildTexts.Checked = objFileHandle.GetValueAsBoolean("WantBuildTexts", false);
            chk_MultiTemplates.Checked = true; // objFileHandle.GetValueAsBoolean("WantMultiTemplates", true);
            chk_SetTemplateBasedOnBinInfo.Checked = objFileHandle.GetValueAsBoolean("WantSetTemplateBasedOnBinInfo_PurposelyRename", false);
            chk_Set1ToAll.Checked = objFileHandle.GetValueAsBoolean("WantSet1ToAll", false);
            chk_SkipMarkInspection.Checked = objFileHandle.GetValueAsBoolean("WantSkipMark", false);
            chk_WantPin1.Checked = objFileHandle.GetValueAsBoolean("WantPin1", false);
            chk_WantUsePin1OrientationWhenNoMark.Checked = objFileHandle.GetValueAsBoolean("WantUsePin1OrientationWhenNoMark", false);
            cbo_Pin1PositionControl.SelectedIndex = objFileHandle.GetValueAsInt("Pin1PositionControl", 0);
            chk_WantDontCareIgnoredMarkWholeArea.Checked = objFileHandle.GetValueAsBoolean("WantDontCareIgnoredMarkWholeArea", false);
            chk_WantUseGaugeForMeasureMarkPkg.Checked = objFileHandle.GetValueAsBoolean("WantGaugeMeasureMarkDimension", false);
            chk_WantClearMarkTemplateWhenNewLot.Checked = objFileHandle.GetValueAsBoolean("WantClearMarkTemplateWhenNewLot", false);
            chk_WantCheckNoMark.Checked = objFileHandle.GetValueAsBoolean("WantCheckNoMark", false);
            chk_WantCheckContourOnMark.Checked = objFileHandle.GetValueAsBoolean("WantCheckContourOnMark", false);
            chk_WantMark2DCode.Checked = objFileHandle.GetValueAsBoolean("WantMark2DCode", false);
            chk_WantDontCareArea_Mark.Checked = objFileHandle.GetValueAsBoolean("WantDontCareAreaMark", false);
            chk_WantSampleAreaScore.Checked = objFileHandle.GetValueAsBoolean("WantSampleAreaScore", false);
            chk_WantRotateMarkImageUsingPkgAngle.Checked = objFileHandle.GetValueAsBoolean("WantRotateMarkImageUsingPkgAngle", false);
            chk_WantCheckMarkAngle.Checked = objFileHandle.GetValueAsBoolean("WantCheckMarkAngle", false);
            chk_UseDefaultMarkScoreAfterClearTemplate.Checked = objFileHandle.GetValueAsBoolean("UseDefaultMarkScoreAfterNewLotClearTemplate", false);
            chk_SeparateExtraMarkThreshold.Checked = objFileHandle.GetValueAsBoolean("SeparateExtraMarkThreshold", false);
            chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Visible = chk_SeparateExtraMarkThreshold.Checked;
            chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Checked = objFileHandle.GetValueAsBoolean("WantExcessMarkThresholdFollowExtraMarkThreshold", false);
            //chk_WantUseLeadPointOffsetMarkROI.Checked = objFileHandle.GetValueAsBoolean("WantUseLeadPointOffsetMarkROI", false);
            chk_WantRemoveBorderWhenLearnMark.Checked = objFileHandle.GetValueAsBoolean("WantRemoveBorderWhenLearnMark", false);
            chk_WantCheckBrokenMark.Checked = objFileHandle.GetValueAsBoolean("WantCheckBrokenMark", false);
            chk_WantCheckTotalExcessMark.Checked = objFileHandle.GetValueAsBoolean("WantCheckTotalExcessMark", false);
            chk_WantUseOCR.Checked = objFileHandle.GetValueAsBoolean("WantUseOCROnly", false);
            chk_WantUseOCRnOCV.Checked = objFileHandle.GetValueAsBoolean("WantUseOCRandOCV", false);
            if (chk_WantUseOCR.Checked)
                chk_WantUseOCRnOCV.Checked = false;
            chk_WantCheckMarkAverageGrayValue.Checked = objFileHandle.GetValueAsBoolean("WantCheckMarkAverageGrayValue", false);
            chk_WantUseUnitPatternAsMarkPattern.Checked = objFileHandle.GetValueAsBoolean("WantUseUnitPatternAsMarkPattern", false);
            if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
                chk_WantUseUnitPatternAsMarkPattern.Visible = true;
            else
                chk_WantUseUnitPatternAsMarkPattern.Visible = false;
            chk_WantUseExcessMissingMarkAffectScore.Checked = objFileHandle.GetValueAsBoolean("WantUseExcessMissingMarkAffectScore", false);
            txt_MarkCharROIOffsetX.Text = m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetX_InMM(objFileHandle.GetValueAsFloat("MarkCharROIOffsetX", 5)).ToString(GetDecimalFormat());
            txt_MarkCharROIOffsetY.Text = m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetY_InMM(objFileHandle.GetValueAsFloat("MarkCharROIOffsetY", 5)).ToString(GetDecimalFormat());
            btn_MarkTypeInspectionSetting.Visible = chk_WantUseMarkTypeInspectionSetting.Checked = objFileHandle.GetValueAsBoolean("WantUseMarkTypeInspectionSetting", false);
            chk_WantCheckBarPin1.Checked = objFileHandle.GetValueAsBoolean("WantCheckBarPin1", false);
            cbo_ExtraExcessMarkInspectionAreaCutMode.SelectedIndex = objFileHandle.GetValueAsInt("ExtraExcessMarkInspectionAreaCutMode", 0);
            cbo_CompensateMarkDiffSizeMode.SelectedIndex = objFileHandle.GetValueAsInt("CompensateMarkDiffSizeMode", 0);
            cbo_MarkScoreMode.SelectedIndex = objFileHandle.GetValueAsInt("MarkScoreMode", 1);  // Default Mode is Gradient Only
            m_smVisionInfo.g_intMarkDefectInspectionMethod = cbo_MarkDefectInspectionMethod.SelectedIndex = objFileHandle.GetValueAsInt("MarkDefectInspectionMethod", 0);
            m_smVisionInfo.g_intMarkTextShiftMethod = cbo_MarkTextShiftMethod.SelectedIndex = objFileHandle.GetValueAsInt("MarkTextShiftMethod", 0);
            //btn_MarkGrayValueSensitivitySetting.Visible = (cbo_MarkDefectInspectionMethod.SelectedIndex == 1);

            // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
            //if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) /*&& ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)*/)
            //    pnl_WantUseLeadPointOffsetMarkROI.Visible = true;
            //else
            //    pnl_WantUseLeadPointOffsetMarkROI.Visible = false;

            if (objFileHandle.GetValueAsInt("CodeType", 0) == 0)
                cbo_2DCodeType.SelectedIndex = 0; // Matrix code
            else
                cbo_2DCodeType.SelectedIndex = 1; // Qr code
            if (objFileHandle.GetValueAsInt("MissingMarkInspectionMethod", 0) == 0)
                cbo_MissingMarkInspectionMethod.SelectedIndex = 0; // Thin On Template Mark (Thin Templae Mark - Normal Sample Mark = missing mark) 
            else
                cbo_MissingMarkInspectionMethod.SelectedIndex = 1; // Thick On Sample Mark  (Normal Template Mark - Thick Sample Mark = missing mark)
            txt_DefaultMarkScore.Text = objFileHandle.GetValueAsInt("DefaultMarkScore", 50).ToString();
            txt_MarkScoreOffset.Text = objFileHandle.GetValueAsInt("MarkScoreOffset", 0).ToString();
            txt_MinMarkScore.Text = objFileHandle.GetValueAsInt("MinMarkScore", 30).ToString();
            txt_MaxMarkTemplate.Text = objFileHandle.GetValueAsInt("MaxMarkTemplate", 4).ToString();
            txt_NoMarkMaximumBlobArea.Text = m_smVisionInfo.g_arrMarks[0].GetNoMarkMaximumBlobArea_InMM(objFileHandle.GetValueAsFloat("NoMarkMaximumBlob", 200)).ToString(GetDecimalFormat());
            txt_MarkOriPositionScore.Text = objFileHandle.GetValueAsInt("MarkOriPositionScore", 70).ToString();
            txt_CheckMarkAngleMinMaxTolerance.Text = objFileHandle.GetValueAsInt("CheckMarkAngleMinMaxTolerance", 10).ToString();

            switch (m_smCustomizeInfo.g_intMarkUnitDisplay)
            {
                case 0:
                    txt_NoMarkMaximumBlobArea.DecimalPlaces = 0;
                    txt_MarkCharROIOffsetX.DecimalPlaces = 0;
                    txt_MarkCharROIOffsetY.DecimalPlaces = 0;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    {
                        srmLabel27.Text = "像素";
                        srmLabel115.Text = "像素";
                        srmLabel113.Text = "像素";
                    }
                    else
                    {
                        srmLabel27.Text = "pix";
                        srmLabel115.Text = "pix";
                        srmLabel113.Text = "pix";
                    }
                    break;
                case 1:
                    txt_NoMarkMaximumBlobArea.DecimalPlaces = 4;
                    txt_MarkCharROIOffsetX.DecimalPlaces = 4;
                    txt_MarkCharROIOffsetY.DecimalPlaces = 4;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    {
                        srmLabel27.Text = "毫米";
                        srmLabel115.Text = "毫米";
                        srmLabel113.Text = "毫米";
                    }
                    else
                    {
                        srmLabel27.Text = "mm";
                        srmLabel115.Text = "mm";
                        srmLabel113.Text = "mm";
                    }
                    break;
                case 2:
                    txt_NoMarkMaximumBlobArea.DecimalPlaces = 3;
                    txt_MarkCharROIOffsetX.DecimalPlaces = 3;
                    txt_MarkCharROIOffsetY.DecimalPlaces = 3;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    {
                        srmLabel27.Text = "mil";
                        srmLabel115.Text = "mil";
                        srmLabel113.Text = "mil";
                    }
                    else
                    {
                        srmLabel27.Text = "mil";
                        srmLabel115.Text = "mil";
                        srmLabel113.Text = "mil";
                    }
                    break;
                case 3:
                    txt_NoMarkMaximumBlobArea.DecimalPlaces = 1;
                    txt_MarkCharROIOffsetX.DecimalPlaces = 1;
                    txt_MarkCharROIOffsetY.DecimalPlaces = 1;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    {
                        srmLabel27.Text = "微米";
                        srmLabel115.Text = "微米";
                        srmLabel113.Text = "微米";
                    }
                    else
                    {
                        srmLabel27.Text = "micron";
                        srmLabel115.Text = "micron";
                        srmLabel113.Text = "micron";
                    }
                    break;
            }

            int intSelectedIndex = objFileHandle.GetValueAsInt("FinalReduction_Direction", 2);  // Final Reduction For Orientation Default value is 2
            if (intSelectedIndex < cbo_FinalReduction_Direction.Items.Count)
                cbo_FinalReduction_Direction.SelectedIndex = intSelectedIndex;
            else
                cbo_FinalReduction_Direction.SelectedIndex = 2; // Final Reduction For Orientation Default value is 2

            intSelectedIndex = objFileHandle.GetValueAsInt("FinalReduction_MarkDeg", 0);    // Final Reduction For Mark Deg Default value is 0
            if (intSelectedIndex < cbo_FinalReduction_MarkDeg.Items.Count)
                cbo_FinalReduction_MarkDeg.SelectedIndex = intSelectedIndex;
            else
                cbo_FinalReduction_MarkDeg.SelectedIndex = 0;   // Final Reduction For Mark Deg Default value is 0

            int intInterpolationValue = objFileHandle.GetValueAsInt("RotationInterpolation_Mark", 4);   // Ratation Interpolation For Mark Default value is 4 (== index 1)
            if (intInterpolationValue == 0)
                cbo_RotationInterpolation_Mark.SelectedIndex = 0;
            else
                cbo_RotationInterpolation_Mark.SelectedIndex = 1;                                       // Ratation Interpolation For Mark Default value is 4 (== index 1)

            intInterpolationValue = objFileHandle.GetValueAsInt("RotationInterpolation_PkgBright", 4);  // Ratation Interpolation For package Default value is 4 (== index 1)
            if (intInterpolationValue == 0)
                cbo_RotationInterpolation_PkgBright.SelectedIndex = 0;
            else
                cbo_RotationInterpolation_PkgBright.SelectedIndex = 1;                                  // Ratation Interpolation For package Default value is 4 (== index 1)

            intInterpolationValue = objFileHandle.GetValueAsInt("RotationInterpolation_PkgDark", 4);    // Ratation Interpolation For package Default value is 4 (== index 1)
            if (intInterpolationValue == 0)
                cbo_RotationInterpolation_PkgDark.SelectedIndex = 0;
            else
                cbo_RotationInterpolation_PkgDark.SelectedIndex = 1;                                    // Ratation Interpolation For package Default value is 4 (== index 1)

            cbo_Pin1ImageNo.Items.Clear();
            cbo_Pin1ImageNo.Items.Add("Image 1");
            for (int i = 2; i <= m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_Pin1ImageNo.Items.Add("Image " + i.ToString());
            }

            if (m_smVisionInfo.g_arrMarks[0].ref_intPin1ImageNo < cbo_Pin1ImageNo.Items.Count)
            {
                if (intSelectedIndex < cbo_Pin1ImageNo.Items.Count)
                    cbo_Pin1ImageNo.SelectedIndex = intSelectedIndex;
                else
                    cbo_Pin1ImageNo.SelectedIndex = 0;
            }
            else
                cbo_Pin1ImageNo.SelectedIndex = 0;

            if ((m_smCustomizeInfo.g_intWantOCR2 & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                chk_WantUseOCR.Visible = chk_WantUseOCR.Checked = false;
                chk_WantUseOCRnOCV.Visible = chk_WantUseOCRnOCV.Checked = false;
            }
        }

        private void UpdatePadGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WantCheckPad.Checked = objFileHandle.GetValueAsBoolean("WantCheckPad", true);
            chk_WantCheckPadColor.Checked = objFileHandle.GetValueAsBoolean("WantCheckPadColor", true);
            pnl_WantCheckPadColor.Visible = chk_WantCheckPadColor.Visible = ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0);
            chk_WantCheck4Sides.Checked = objFileHandle.GetValueAsBoolean("WantCheck4Sides", false);
            chk_WantGRR.Checked = objFileHandle.GetValueAsBoolean("WantShowGRR", false);
            chk_WantCPK.Checked = objFileHandle.GetValueAsBoolean("WantCheckCPK", false);
            chk_CheckAllPadCPK.Checked = objFileHandle.GetValueAsBoolean("CheckAllPadCPK", false);
            txt_PadCPKCount.Text = objFileHandle.GetValueAsInt("PadCPKCount", 100).ToString();
            chk_WantDontCareArea_Pad.Checked = objFileHandle.GetValueAsBoolean("WantDontCareAreaPad", false);
            chk_WantEdgeLimit_Pad.Checked = objFileHandle.GetValueAsBoolean("WantEdgeLimitPad", false);
            chk_WantEdgeDistance_Pad.Checked = objFileHandle.GetValueAsBoolean("WantEdgeDistancePad", false);
            chk_WantSpan_Pad.Checked = objFileHandle.GetValueAsBoolean("WantSpanPad", false);
            pnl_PadColorGroupSetting.Visible = m_smVisionInfo.g_arrPad.Length > 1 && ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0);
            cbo_ColorDefectLinkMethod_Pad.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intColorDefectLinkMethod;
            pnl_ColorDefectLink_Pad.Visible = ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0);
            txt_ColorDefectLinkTolerance_Pad.Text = m_smVisionInfo.g_arrPad[0].ref_intColorDefectLinkTolerance.ToString();
            if (cbo_ColorDefectLinkMethod_Pad.SelectedIndex == 0)
            {
                pnl_ColorDefectLinkTolerance_Pad.Visible = false;
            }
            else
            {
                pnl_ColorDefectLinkTolerance_Pad.Visible = true;
            }

            if (objFileHandle.GetValueAsInt("OrientDirection", 4) == 4)
            {
                radioBtn_4Directions_Pad.Checked = true;
            }
            else
            {
                radioBtn_2Directions_Pad.Checked = true;
            }

            if (!chk_WantEdgeLimit_Pad.Checked)
            {
                chk_WantStandOff_Pad.Checked = objFileHandle.GetValueAsBoolean("WantStandOffPad", false);
                if (chk_WantStandOff_Pad.Enabled)
                {
                    btn_PadStandOffDirectionSetting.Enabled = chk_WantStandOff_Pad.Checked;
                }
                else
                    btn_PadStandOffDirectionSetting.Enabled = false;
            }
            else
            {
                chk_WantStandOff_Pad.Checked = false;
                btn_PadStandOffDirectionSetting.Enabled = false;
            }

            chk_PadWhiteOnBlack.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWhiteOnBlack;
            chk_WantTestRunTightSetting.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantTightSetting;
            if (m_smVisionInfo.g_arrPad.Length > 1)
                chk_WantConsiderPadImage2.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantConsiderPadImage2;    // Use index 1 because Is is for side image only
            chk_WantPRUnitLocationBeforeGauge.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantPRUnitLocationBeforeGauge;
            chk_WantUseGaugeMeasureDimension.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantUseGaugeMeasureDimension;
            if (m_smVisionInfo.g_arrPad[0].ref_blnWantUseGaugeMeasureDimension)
                cbo_PadMeasurementMethod.SelectedIndex = 1; // Point Method
            else
                cbo_PadMeasurementMethod.SelectedIndex = 0; // Max Method

            if (m_smVisionInfo.g_arrPad.Length > 1)
            {
                if (m_smVisionInfo.g_arrPad[1].ref_blnWantUseGaugeMeasureDimension)
                    cbo_PadMeasurementMethod_Side.SelectedIndex = 1; // Point Method
                else
                    cbo_PadMeasurementMethod_Side.SelectedIndex = 0; // Max Method
            }
            else
            {
                cbo_PadMeasurementMethod_Side.Visible = false;
                srmLabel48.Visible = false;
            }

            cbo_SavePadTemplateImageMethod.SelectedIndex = objFileHandle.GetValueAsInt("SavePadTemplateImageMethod", 0);
            cbo_PadOffsetReferencePoint.SelectedIndex = objFileHandle.GetValueAsInt("ReferencePoint", 0);
            cbo_PadSubtractMethod_Center.SelectedIndex = objFileHandle.GetValueAsInt("PadSubtractMethod_Center", 0); // Default is 0 = pad individual method;
            cbo_PadSubtractMethod_Side.SelectedIndex = objFileHandle.GetValueAsInt("PadSubtractMethod_Side", 0); // Default is 0 = pad individual method;
            if (m_smVisionInfo.g_arrPad.Length == 1)
            {
                srmLabel79.Visible = cbo_PadSubtractMethod_Side.Visible = false;
            }


            chk_DefinePadToleranceUsingClosestMethod.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantUseClosestSizeDefineTolerance;
            chk_WantAutoGauge.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantAutoGauge;
            chk_WantRotateSidePadImage.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantRotateSidePadImage;
            chk_WantLinkDifferentGroupPitchGap.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantLinkDifferentGroupPitchGap;
            chk_ShowUseGaugeCheckBoxInLearnForm.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantShowUseGaugeCheckBox;
            chk_WantSeparateBrokenPadThresholdSetting.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantSeparateBrokenPadThresholdSetting;
            chk_WantViewCheckForeignMaterialOptionWhenPackageON.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON;
            chk_MeasureCenterPkgSizeUsingSidePkg.Checked = m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg;
            txt_DefaultPixelTolerance.Text = m_smVisionInfo.g_arrPad[0].ref_fDefaultPixelTolerance.ToString();
            txt_PadROITolerance.Text = m_smVisionInfo.g_arrPad[0].ref_intPadROISizeToleranceADV.ToString();
            //txt_PadSizeHalfWidthTolerance.Text = m_smVisionInfo.g_arrPad[0].ref_intPadSizeHalfWidthTolerance.ToString();
            txt_TightSettingThresholdTolerance.Text = m_smVisionInfo.g_arrPad[0].ref_intTightSettingThresholdTolerance.ToString();
            txt_TightSettingDimensionTolerance.Text = m_smVisionInfo.g_arrPad[0].ref_fTightSettingTolerance.ToString();
            txt_SensitivityValue.Text = m_smVisionInfo.g_arrPad[0].ref_intSensitivityOnPadValue.ToString();
            cbo_SensitivityOnPad.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intSensitivityOnPadMethod;
            m_arrSensitivityOnPadMethod = new int[m_smVisionInfo.g_arrPad.Length];
            m_arrSensitivityOnPadValue = new int[m_smVisionInfo.g_arrPad.Length];
            for (int i = 0; i < m_arrSensitivityOnPadMethod.Length; i++)
            {
                m_arrSensitivityOnPadMethod[i] = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod;
                m_arrSensitivityOnPadValue[i] = m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue;
            }
            chk_WantPadPin1.Checked = objFileHandle.GetValueAsBoolean("WantPin1", false);
            chk_WantCheckPH.Checked = m_smVisionInfo.g_blnWantCheckPH;
            if (m_intUserGroup != 1)    // SRM User
            {
                pnl_WantGRR.Visible = false;
                txt_TightSettingDimensionTolerance.Visible = srmLabel10.Visible = srmLabel9.Visible = false;
                txt_TightSettingThresholdTolerance.Visible = srmLabel8.Visible = srmLabel7.Visible = false;
                pnl_WantUseGaugeMeasureDimension.Visible = false;
                btn_LineProfileGaugeSetting.Visible = false;
            }

            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
                chk_WantIndividualSideThickness.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantIndividualSideThickness;
            else
                pnl_WantIndividualSideThickness.Visible = false;

            //if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            //    pnl_WantCheckPackage.Visible = false;

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPkg":
                    pnl_WantConsiderPadImage2.Visible = false;
                    pnl_WantCheck4Sides.Visible = false;
                    pnl_WantRotateSidePadImage.Visible = false;
                    break;
            }

            cbo_CenterROIBrokenPadImageViewNo.Items.Clear();
            cbo_SideROIBrokenPadImageViewNo.Items.Clear();
            cbo_CenterROIBrokenPadImageViewNo.Items.Add("Image 1");
            cbo_SideROIBrokenPadImageViewNo.Items.Add("Image 1");
            for (int i = 2; i <= m_smVisionInfo.g_intImageViewCount; i++)
            {
                cbo_CenterROIBrokenPadImageViewNo.Items.Add("Image " + i.ToString());
                cbo_SideROIBrokenPadImageViewNo.Items.Add("Image " + i.ToString());
            }

            if (m_smVisionInfo.g_arrPad.Length > 0)
            {
                if (m_smVisionInfo.g_arrPad[0].ref_intBrokenPadImageViewNo < cbo_CenterROIBrokenPadImageViewNo.Items.Count)
                    cbo_CenterROIBrokenPadImageViewNo.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intBrokenPadImageViewNo;
                else
                    cbo_CenterROIBrokenPadImageViewNo.SelectedIndex = 0;
            }

            if (m_smVisionInfo.g_arrPad.Length == 5)
            {
                if (m_smVisionInfo.g_arrPad[1].ref_intBrokenPadImageViewNo < cbo_SideROIBrokenPadImageViewNo.Items.Count)
                    cbo_SideROIBrokenPadImageViewNo.SelectedIndex = m_smVisionInfo.g_arrPad[1].ref_intBrokenPadImageViewNo;
                else
                    cbo_SideROIBrokenPadImageViewNo.SelectedIndex = 0;
            }
            else
            {
                srmLabel34.Visible = false;
                cbo_SideROIBrokenPadImageViewNo.Visible = false;
            }

        }

        private void UpdatePadPackageGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WantUseDetailThreshold_PadPackage.Checked = objFileHandle.GetValueAsBoolean("WantUseDetailThresholdPadPackage", false);
            chk_WantCheckPackage.Checked = objFileHandle.GetValueAsBoolean("WantCheckPackage", false);
            pnl_WantLinkCrackDefect_PadPackage.Visible = chk_SeperateCrackDefectSetting_PadPackage.Checked = objFileHandle.GetValueAsBoolean("SeperateCrackDefectSetting", false);
            chk_SeperateForeignMaterialDefectSetting_PadPackage.Checked = objFileHandle.GetValueAsBoolean("SeperateForeignMaterialDefectSetting", false);
            chk_SeperateChippedOffDefectSetting_PadPackage.Checked = objFileHandle.GetValueAsBoolean("SeperateChippedOffDefectSetting", false);
            pnl_WantLinkMoldFlashDefect_PadPackage.Visible = pnl_PadPkgMoldFlashDefectImageView.Visible = chk_SeperateMoldFlashDefectSetting_PadPackage.Checked = objFileHandle.GetValueAsBoolean("SeperateMoldFlashDefectSetting", false);
            chk_WantDontCareArea_PadPackage.Checked = objFileHandle.GetValueAsBoolean("WantDontCareAreaPadPackage", false);
            chk_WantDontCarePadForPackage.Checked = objFileHandle.GetValueAsBoolean("WantDontCarePadForPackage", false);
            chk_SeperateBrightDarkROITolerance_PadPackage.Checked = objFileHandle.GetValueAsBoolean("SeperateBrightDarkROITolerancePadPackage", false);

            chk_WantLinkBrightDefect_PadPackage.Checked = objFileHandle.GetValueAsBoolean("WantLinkBrightDefect", false);
            chk_WantLinkDarkDefect_PadPackage.Checked = objFileHandle.GetValueAsBoolean("WantLinkDarkDefect", false);
            chk_WantLinkCrackDefect_PadPackage.Checked = objFileHandle.GetValueAsBoolean("WantLinkCrackDefect", false);
            chk_WantLinkMoldFlashDefect_PadPackage.Checked = objFileHandle.GetValueAsBoolean("WantLinkMoldFlashDefect", false);

            txt_BrightDefectLinkTolerance_PadPackage.Text = objFileHandle.GetValueAsInt("BrightDefectLinkTolerance", 10).ToString();
            txt_DarkDefectLinkTolerance_PadPackage.Text = objFileHandle.GetValueAsInt("DarkDefectLinkTolerance", 10).ToString();
            txt_CrackDefectLinkTolerance_PadPackage.Text = objFileHandle.GetValueAsInt("CrackDefectLinkTolerance", 10).ToString();
            txt_MoldFlashDefectLinkTolerance_PadPackage.Text = objFileHandle.GetValueAsInt("MoldFlashDefectLinkTolerance", 10).ToString();

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPkg":
                    pnl_PadPkg_UseSideROIEdgeToGetCenterROIEdge.Visible = false;
                    break;
            }

            cbo_PadPkgSizeImageViewNo_Center.Items.Clear();
            cbo_PadPkgBrightFieldImageViewNo_Center.Items.Clear();
            cbo_PadPkgDarkFieldImageNo_Center.Items.Clear();
            cbo_PadPkgMoldFlashImageViewNo_Center.Items.Clear();
            cbo_PadPkgSizeImageViewNo_Side.Items.Clear();
            cbo_PadPkgBrightFieldImageViewNo_Side.Items.Clear();
            cbo_PadPkgDarkFieldImageNo_Side.Items.Clear();
            cbo_PadPkgMoldFlashImageViewNo_Side.Items.Clear();

            cbo_PadPkgSizeImageViewNo_Center.Items.Add("Image 1");
            cbo_PadPkgBrightFieldImageViewNo_Center.Items.Add("Image 1");
            cbo_PadPkgDarkFieldImageNo_Center.Items.Add("Image 1");
            cbo_PadPkgMoldFlashImageViewNo_Center.Items.Add("Image 1");
            cbo_PadPkgSizeImageViewNo_Side.Items.Add("Image 1");
            cbo_PadPkgBrightFieldImageViewNo_Side.Items.Add("Image 1");
            cbo_PadPkgDarkFieldImageNo_Side.Items.Add("Image 1");
            cbo_PadPkgMoldFlashImageViewNo_Side.Items.Add("Image 1");

            for (int i = 2; i <= m_smVisionInfo.g_intImageViewCount; i++)
            {
                cbo_PadPkgSizeImageViewNo_Center.Items.Add("Image " + i.ToString());
                cbo_PadPkgBrightFieldImageViewNo_Center.Items.Add("Image " + i.ToString());
                cbo_PadPkgDarkFieldImageNo_Center.Items.Add("Image " + i.ToString());
                cbo_PadPkgMoldFlashImageViewNo_Center.Items.Add("Image " + i.ToString());

                cbo_PadPkgSizeImageViewNo_Side.Items.Add("Image " + i.ToString());
                cbo_PadPkgBrightFieldImageViewNo_Side.Items.Add("Image " + i.ToString());
                cbo_PadPkgDarkFieldImageNo_Side.Items.Add("Image " + i.ToString());
                cbo_PadPkgMoldFlashImageViewNo_Side.Items.Add("Image " + i.ToString());
            }

            int intSelectedIndex = objFileHandle.GetValueAsInt("PadPkgSizeImageViewNo_Center", 0); // Default is 0
            if (intSelectedIndex < cbo_PadPkgSizeImageViewNo_Center.Items.Count)
                cbo_PadPkgSizeImageViewNo_Center.SelectedIndex = intSelectedIndex;
            else
                cbo_PadPkgSizeImageViewNo_Center.SelectedIndex = 0;

            intSelectedIndex = objFileHandle.GetValueAsInt("PadPkgBrightFieldImageViewNo_Center", 0);  // Default is 0
            if (intSelectedIndex < cbo_PadPkgBrightFieldImageViewNo_Center.Items.Count)
                cbo_PadPkgBrightFieldImageViewNo_Center.SelectedIndex = intSelectedIndex;
            else
                cbo_PadPkgBrightFieldImageViewNo_Center.SelectedIndex = 0;

            intSelectedIndex = objFileHandle.GetValueAsInt("PadPkgDarkFieldImageNo_Center", 1);    // Default is 1
            if (intSelectedIndex < cbo_PadPkgDarkFieldImageNo_Center.Items.Count)
                cbo_PadPkgDarkFieldImageNo_Center.SelectedIndex = intSelectedIndex;
            else
                cbo_PadPkgDarkFieldImageNo_Center.SelectedIndex = 0;

            intSelectedIndex = objFileHandle.GetValueAsInt("PadPkgMoldFlashImageViewNo_Center", 0);  // Default is 0
            if (intSelectedIndex < cbo_PadPkgMoldFlashImageViewNo_Center.Items.Count)
                cbo_PadPkgMoldFlashImageViewNo_Center.SelectedIndex = intSelectedIndex;
            else
                cbo_PadPkgMoldFlashImageViewNo_Center.SelectedIndex = 0;

            intSelectedIndex = objFileHandle.GetValueAsInt("PadPkgSizeImageViewNo_Side", 0); // Default is 0
            if (intSelectedIndex < cbo_PadPkgSizeImageViewNo_Side.Items.Count)
                cbo_PadPkgSizeImageViewNo_Side.SelectedIndex = intSelectedIndex;
            else
                cbo_PadPkgSizeImageViewNo_Side.SelectedIndex = 0;

            intSelectedIndex = objFileHandle.GetValueAsInt("PadPkgBrightFieldImageViewNo_Side", 0);  // Default is 0
            if (intSelectedIndex < cbo_PadPkgBrightFieldImageViewNo_Side.Items.Count)
                cbo_PadPkgBrightFieldImageViewNo_Side.SelectedIndex = intSelectedIndex;
            else
                cbo_PadPkgBrightFieldImageViewNo_Side.SelectedIndex = 0;

            intSelectedIndex = objFileHandle.GetValueAsInt("PadPkgDarkFieldImageNo_Side", 1);    // Default is 1
            if (intSelectedIndex < cbo_PadPkgDarkFieldImageNo_Side.Items.Count)
                cbo_PadPkgDarkFieldImageNo_Side.SelectedIndex = intSelectedIndex;
            else
                cbo_PadPkgDarkFieldImageNo_Side.SelectedIndex = 0;

            intSelectedIndex = objFileHandle.GetValueAsInt("PadPkgMoldFlashImageViewNo_Side", 0);  // Default is 0
            if (intSelectedIndex < cbo_PadPkgMoldFlashImageViewNo_Side.Items.Count)
                cbo_PadPkgMoldFlashImageViewNo_Side.SelectedIndex = intSelectedIndex;
            else
                cbo_PadPkgMoldFlashImageViewNo_Side.SelectedIndex = 0;

            cbo_PadPkgMoldFlashDefectType_Center.SelectedIndex = objFileHandle.GetValueAsInt("PadPkgMoldFlashDefectType_Center", 0);
            cbo_PadPkgMoldFlashDefectType_Side.SelectedIndex = objFileHandle.GetValueAsInt("PadPkgMoldFlashDefectType_Side", 0);

            if (m_smVisionInfo.g_arrPad.Length == 1)
            {
                srmLabel39.Visible = false;
                cbo_PadPkgBrightFieldImageViewNo_Side.Visible = false;
                srmLabel40.Visible = false;
                cbo_PadPkgDarkFieldImageNo_Side.Visible = false;
                srmLabel102.Visible = false;
                cbo_PadPkgMoldFlashImageViewNo_Side.Visible = false;
                srmLabel154.Visible = false;
                cbo_PadPkgMoldFlashDefectType_Side.Visible = false;
            }
        }

        private void UpdateLead3DGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WantGRR_Lead3D.Checked = objFileHandle.GetValueAsBoolean("WantShowGRR", false);
            chk_WantDontCareArea_Lead3D.Checked = objFileHandle.GetValueAsBoolean("WantDontCareAreaLead3D", false);
            chk_WantLead3DPin1.Checked = objFileHandle.GetValueAsBoolean("WantPin1", false);
            chk_WantCheckPH_Lead3D.Checked = m_smVisionInfo.g_blnWantCheckPH;
            chk_WantUseGaugeMeasureBase_Lead3D.Visible = chk_WantPkgToBaseTolerance_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance;
            chk_WantUseGaugeMeasureBase_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseGaugeMeasureBase;
            btn_AverageGrayValueROITolerance_Lead3D.Enabled = chk_WantUseAverageGrayValueMethod_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod;
            cbo_ImageRotateOption_Lead3D.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].ref_intImageRotateOption;
            if (!chk_WantUseAverageGrayValueMethod_Lead3D.Enabled)
            {
                btn_AverageGrayValueROITolerance_Lead3D.Enabled = false;
            }
            cbo_Lead3DWidthDisplayOption.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].ref_intLeadWidthDisplayOption;
            cbo_Lead3DLengthVarianceMethod.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].ref_intLeadLengthVarianceMethod; 
            cbo_Lead3DSpanMethod.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].ref_intLeadSpanMethod;
            cbo_Lead3DContaminationRegion.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].ref_intLeadContaminationRegion;
            cbo_Lead3DStandOffMethod.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].ref_intLeadStandOffMethod;
            cbo_LeadWidthRangeSelection.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].ref_intLeadWidthRangeSelection;
            txt_LeadWidthRange.Text = m_smVisionInfo.g_arrLead3D[0].ref_intLeadWidthRange.ToString();
            chk_WantUseMasking_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseAGVMasking;
            txt_MatchingXTolerance_Lead3D.Text = m_smVisionInfo.g_arrLead3D[0].ref_intMatchingXTolerance.ToString();
            txt_MatchingYTolerance_Lead3D.Text = m_smVisionInfo.g_arrLead3D[0].ref_intMatchingYTolerance.ToString();

            if (m_intUserGroup != 1)    // SRM User
            {
                pnl_WantGRR_Lead3D.Visible = false;
            }
        }
        private void UpdateLead3DPackageGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WantUseDetailThreshold_LeadPackage.Checked = objFileHandle.GetValueAsBoolean("WantUseDetailThresholdLeadPackage", false);
            chk_SeperateCrackDefectSetting_LeadPackage.Checked = objFileHandle.GetValueAsBoolean("SeperateCrackDefectSetting", false);
            chk_SeperateChippedOffDefectSetting_LeadPackage.Checked = objFileHandle.GetValueAsBoolean("SeperateChippedOffDefectSetting", false);
            chk_SeperateMoldFlashDefectSetting_LeadPackage.Checked = objFileHandle.GetValueAsBoolean("SeperateMoldFlashDefectSetting", false);

            cbo_LeadPkgSizeImageNo.Items.Clear();
            cbo_LeadPkgBrightFieldImageNo.Items.Clear();
            cbo_LeadPkgDarkFieldImageNo.Items.Clear();
            cbo_LeadPkgSizeImageNo.Items.Add("Image 1");
            cbo_LeadPkgBrightFieldImageNo.Items.Add("Image 1");
            cbo_LeadPkgDarkFieldImageNo.Items.Add("Image 1");
            for (int i = 2; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_LeadPkgSizeImageNo.Items.Add("Image " + i.ToString());
                cbo_LeadPkgBrightFieldImageNo.Items.Add("Image " + i.ToString());
                cbo_LeadPkgDarkFieldImageNo.Items.Add("Image " + i.ToString());
            }

            if (m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0) >= cbo_LeadPkgSizeImageNo.Items.Count)
                cbo_LeadPkgSizeImageNo.SelectedIndex = cbo_LeadPkgSizeImageNo.Items.Count - 1;
            else
                cbo_LeadPkgSizeImageNo.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0);

            if (m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1) >= cbo_LeadPkgSizeImageNo.Items.Count)
                cbo_LeadPkgBrightFieldImageNo.SelectedIndex = cbo_LeadPkgBrightFieldImageNo.Items.Count - 1;
            else
                cbo_LeadPkgBrightFieldImageNo.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);

            if (m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2) >= cbo_LeadPkgDarkFieldImageNo.Items.Count)
                cbo_LeadPkgDarkFieldImageNo.SelectedIndex = cbo_LeadPkgDarkFieldImageNo.Items.Count - 1;
            else
                cbo_LeadPkgDarkFieldImageNo.SelectedIndex = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2);

            chk_MeasureCenterPkgSizeUsingCorner.Checked = m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner;
        }

        private void UpdateLeadGUI()
        {
            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && (m_smVisionInfo.g_arrLead[0].ref_intRotationMethod != 2))
                chk_WantPkgToBaseTolerance_Lead.Visible = false;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WantCheckLead.Checked = objFileHandle.GetValueAsBoolean("WantCheckLead", false);
            chk_WantPocketDontCareAreaFix_Lead.Checked = objFileHandle.GetValueAsBoolean("WantPocketDontCareAreaFix_Lead", false);
            chk_WantPocketDontCareAreaManual_Lead.Checked = objFileHandle.GetValueAsBoolean("WantPocketDontCareAreaManual_Lead", false);
            chk_WantPocketDontCareAreaAuto_Lead.Checked = objFileHandle.GetValueAsBoolean("WantPocketDontCareAreaAuto_Lead", false);
            chk_WantPocketDontCareAreaBlob_Lead.Checked = objFileHandle.GetValueAsBoolean("WantPocketDontCareAreaBlob_Lead", false);
            chk_WantUseGaugeMeasureLeadDimension.Checked = m_smVisionInfo.g_arrLead[0].ref_blnWantUseGaugeMeasureLeadDimension;
            chk_WantPkgToBaseTolerance_Lead.Checked = m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance;
            btn_AverageGrayValueROITolerance_Lead.Enabled = chk_WantUseAverageGrayValueMethod_Lead.Checked = m_smVisionInfo.g_arrLead[0].ref_blnWantUseAverageGrayValueMethod;
            if (!chk_WantUseAverageGrayValueMethod_Lead.Enabled)
            {
                btn_AverageGrayValueROITolerance_Lead.Enabled = false;
            }
            chk_WantInspectBaseLead.Checked = m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead;
            chk_WantUseMasking_Lead.Checked = m_smVisionInfo.g_arrLead[0].ref_blnWantUseAGVMasking;
            cbo_RotationMethod_Lead.SelectedIndex = m_smVisionInfo.g_arrLead[0].ref_intRotationMethod;
            txt_LeadAngleTolerance.Text = m_smVisionInfo.g_arrLead[0].ref_intLeadAngleTolerance.ToString();
            cbo_PocketDontCareMethod_Lead.SelectedIndex = m_smVisionInfo.g_arrLead[0].ref_intPocketDontCareMethod;

            if (chk_WantPocketDontCareAreaFix_Lead.Checked)
            {
                if (chk_WantPocketDontCareAreaManual_Lead.Checked)
                    chk_WantPocketDontCareAreaManual_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaAuto_Lead.Checked)
                    chk_WantPocketDontCareAreaAuto_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaBlob_Lead.Checked)
                    chk_WantPocketDontCareAreaBlob_Lead.Checked = false;
            }
            else if (chk_WantPocketDontCareAreaManual_Lead.Checked)
            {
                if (chk_WantPocketDontCareAreaFix_Lead.Checked)
                    chk_WantPocketDontCareAreaFix_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaAuto_Lead.Checked)
                    chk_WantPocketDontCareAreaAuto_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaBlob_Lead.Checked)
                    chk_WantPocketDontCareAreaBlob_Lead.Checked = false;
            }
            else if (chk_WantPocketDontCareAreaAuto_Lead.Checked)
            {
                if (chk_WantPocketDontCareAreaFix_Lead.Checked)
                    chk_WantPocketDontCareAreaFix_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaManual_Lead.Checked)
                    chk_WantPocketDontCareAreaManual_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaBlob_Lead.Checked)
                    chk_WantPocketDontCareAreaBlob_Lead.Checked = false;
            }
            else if (chk_WantPocketDontCareAreaBlob_Lead.Checked)
            {
                if (chk_WantPocketDontCareAreaFix_Lead.Checked)
                    chk_WantPocketDontCareAreaFix_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaManual_Lead.Checked)
                    chk_WantPocketDontCareAreaManual_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaAuto_Lead.Checked)
                    chk_WantPocketDontCareAreaAuto_Lead.Checked = false;
            }

            if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
            {
                //chk_WantPocketDontCareAreaManual_Lead.Visible = true;
                //chk_WantPocketDontCareAreaAuto_Lead.Visible = true;
                //grp_PocketDontCare.Visible = true;

                srmLabel64.Visible = true;
                cbo_BaseLeadImageNo.Visible = true;
                chk_WantInspectBaseLead.Visible = true;
            }
            else
            {
                //chk_WantPocketDontCareAreaManual_Lead.Visible = false;
                //chk_WantPocketDontCareAreaAuto_Lead.Visible = false;
                //grp_PocketDontCare.Visible = false;

                srmLabel64.Visible = false;
                cbo_BaseLeadImageNo.Visible = false;
                chk_WantInspectBaseLead.Visible = false;
            }

            if (m_intUserGroup != 1)    // SRM User
            {
                chk_WantUseGaugeMeasureLeadDimension.Visible = false;
                btn_LeadLineProfileGaugeSetting.Visible = false;
            }

            cbo_LeadDefectImageNo.Items.Clear();
            cbo_LeadDefectImageNo.Items.Add("Image 1");
            for (int i = 2; i <= m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_LeadDefectImageNo.Items.Add("Image " + i.ToString());
            }

            int intSelectedIndex = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
            if (intSelectedIndex < cbo_LeadDefectImageNo.Items.Count)
                cbo_LeadDefectImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_LeadDefectImageNo.SelectedIndex = 0;

            cbo_BaseLeadImageNo.Items.Clear();
            cbo_BaseLeadImageNo.Items.Add("Image 1");
            for (int i = 2; i <= m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_BaseLeadImageNo.Items.Add("Image " + i.ToString());
            }

            intSelectedIndex = m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo;
            if (intSelectedIndex < cbo_BaseLeadImageNo.Items.Count)
                cbo_BaseLeadImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_BaseLeadImageNo.SelectedIndex = 0;
        }

        private void UpdatePocketPositionGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\PocketPosition\\Settings.xml";

            //XmlParser objFileHandle = new XmlParser(strPath);
            chk_WantCheckPocketPosition.Checked = m_smVisionInfo.g_blnWantCheckPocketPosition;
            chk_WantUsePocketPattern.Checked = m_smVisionInfo.g_blnWantUsePocketPattern;
            chk_WantUsePocketGauge.Checked = m_smVisionInfo.g_blnWantUsePocketGauge;
            chk_WantUsePocketPattern.Enabled = chk_WantCheckPocketPosition.Checked;
            chk_WantUsePocketGauge.Enabled = chk_WantCheckPocketPosition.Checked;

            cbo_PocketPatternAndGaugeImageNo.Items.Clear();
            cbo_PlateGaugeImageNo.Items.Clear();
            cbo_PocketPatternAndGaugeImageNo.Items.Add("Image 1");
            cbo_PlateGaugeImageNo.Items.Add("Image 1");
            for (int i = 2; i <= m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_PocketPatternAndGaugeImageNo.Items.Add("Image " + i.ToString());
                cbo_PlateGaugeImageNo.Items.Add("Image " + i.ToString());
            }

            int intSelectedIndex = m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0);
            if (intSelectedIndex < cbo_PocketPatternAndGaugeImageNo.Items.Count)
                cbo_PocketPatternAndGaugeImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_PocketPatternAndGaugeImageNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1);
            if (intSelectedIndex < cbo_PlateGaugeImageNo.Items.Count)
                cbo_PlateGaugeImageNo.SelectedIndex = intSelectedIndex;
            else
                cbo_PlateGaugeImageNo.SelectedIndex = 0;
        }

        private void UpdateSealGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");

            chk_SealWhiteOnBlack.Checked = objFileHandle.GetValueAsBoolean("SealWhiteOnBlack", true);

            radioBtn_2DirectionsSeal.Checked = (objFileHandle.GetValueAsInt("Direction", 4) == 2);
            radioBtn_4DirectionsSeal.Checked = (objFileHandle.GetValueAsInt("Direction", 4) == 4);
            chk_SkipOrientInspection.Checked = objFileHandle.GetValueAsBoolean("WantSkipOrient", false);
            chk_SkipSprocketHoleInspection.Checked = objFileHandle.GetValueAsBoolean("WantSkipSprocketHole", true);
            chk_SkipSprocketHoleDiameterAndDefectInspection.Checked = objFileHandle.GetValueAsBoolean("WantSkipSprocketHoleDiameterAndDefect", true);
            chk_SkipSprocketHoleBrokenAndRoundnessInspection.Checked = objFileHandle.GetValueAsBoolean("WantSkipSprocketHoleBrokenAndRoundness", true);

            txt_MaxSealMarkTemplate.Text = objFileHandle.GetValueAsInt("MaxSealMarkTemplate", 8).ToString();
            txt_MaxSealEmptyTemplate.Text = objFileHandle.GetValueAsInt("MaxSealEmptyTemplate", 4).ToString();

            int intSelectedIndex = objFileHandle.GetValueAsInt("CheckMarkMethod", 0);
            if (intSelectedIndex >= cbo_CheckSealMarkMethod.Items.Count)
                intSelectedIndex = 1;   // If use SRM1 mode if out of index.
            cbo_CheckSealMarkMethod.SelectedIndex = intSelectedIndex;
            chk_WantDontCareArea_Seal.Checked = objFileHandle.GetValueAsBoolean("WantDontCareAreaSeal", false);
            txt_MarkAreaBelowPercent.Text = objFileHandle.GetValueAsString("MarkAreaBelowPercent", "3");
            txt_PatternAngleTolerance.Text = objFileHandle.GetValueAsString("PatternAngleTolerance", "10");
            chk_WantClearSealTemplateWhenNewLot.Checked = objFileHandle.GetValueAsBoolean("ClearSealTemplateWhenNewLot", false);
            chk_WantCheckSealEdgeStraightness.Checked = objFileHandle.GetValueAsBoolean("WantCheckSealEdgeStraightness", false);

            cbo_SprocketHoleViewNo.Items.Clear();
            cbo_SealDistanceViewNo.Items.Clear();
            cbo_SealOverheatViewNo.Items.Clear();
            cbo_CheckUnitViewNo.Items.Clear();
            cbo_SealBrokenViewNo.Items.Clear();
            cbo_SealEdgeStraightnessViewNo.Items.Clear();
            cbo_SprocketHoleViewNo.Items.Add("Image 1");
            cbo_SealDistanceViewNo.Items.Add("Image 1");
            cbo_SealOverheatViewNo.Items.Add("Image 1");
            cbo_CheckUnitViewNo.Items.Add("Image 1");
            cbo_SealBrokenViewNo.Items.Add("Image 1");
            cbo_SealEdgeStraightnessViewNo.Items.Add("Image 1");

            for (int i = 2; i <= m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_SprocketHoleViewNo.Items.Add("Image " + i.ToString());
                cbo_SealDistanceViewNo.Items.Add("Image " + i.ToString());
                cbo_SealOverheatViewNo.Items.Add("Image " + i.ToString());
                cbo_CheckUnitViewNo.Items.Add("Image " + i.ToString());
                cbo_SealBrokenViewNo.Items.Add("Image " + i.ToString());
                cbo_SealEdgeStraightnessViewNo.Items.Add("Image " + i.ToString());
            }

            cbo_SealSprocketHolePosition.SelectedIndex = m_smVisionInfo.g_objSeal.ref_intSpocketHolePosition;

            intSelectedIndex = m_smVisionInfo.g_objSeal.GetGrabImageIndex(0);
            if (intSelectedIndex < cbo_SprocketHoleViewNo.Items.Count)
                cbo_SprocketHoleViewNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SprocketHoleViewNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_objSeal.GetGrabImageIndex(1);
            if (intSelectedIndex < cbo_SealBrokenViewNo.Items.Count)
                cbo_SealBrokenViewNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SealBrokenViewNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_objSeal.GetGrabImageIndex(3);
            if (intSelectedIndex < cbo_SealDistanceViewNo.Items.Count)
                cbo_SealDistanceViewNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SealDistanceViewNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_objSeal.GetGrabImageIndex(4);
            if (intSelectedIndex < cbo_SealOverheatViewNo.Items.Count)
                cbo_SealOverheatViewNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SealOverheatViewNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_objSeal.GetGrabImageIndex(5);
            if (intSelectedIndex < cbo_CheckUnitViewNo.Items.Count)
                cbo_CheckUnitViewNo.SelectedIndex = intSelectedIndex;
            else
                cbo_CheckUnitViewNo.SelectedIndex = 0;

            intSelectedIndex = m_smVisionInfo.g_objSeal.GetGrabImageIndex(7);
            if (intSelectedIndex < cbo_SealEdgeStraightnessViewNo.Items.Count)
                cbo_SealEdgeStraightnessViewNo.SelectedIndex = intSelectedIndex;
            else
                cbo_SealEdgeStraightnessViewNo.SelectedIndex = 0;

            //bool blnPackageFound = false;

            //DataSet dsPackage = new DataSet();
            //DBCall dbCall = new DBCall(@"access\setting.mdb");
            //dbCall.Select("SELECT * FROM Package", m_dsPackage);

            //// delete all the content of combo boxes before new insertion
            //if (cbo_Package.Items.Count != 0)
            //    cbo_Package.Items.Clear();

            //DataRow[] drPackageList = m_dsPackage.Tables[0].Select("Type >= 0", "Type");
            //foreach (DataRow package in drPackageList)
            //{
            //    string strName = package["Name"].ToString();
            //    cbo_Package.Items.Add(strName);

            //    if (strName == (objFileHandle.GetValueAsString("PackageName", "")))
            //    {
            //        blnPackageFound = true;
            //        cbo_Package.SelectedIndex = cbo_Package.FindStringExact(strName);
            //    }
            //}

            //if (!blnPackageFound)
            //{
            //    SRMMessageBox.Show("Invalid Package Type. Auto Select First Package Type", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    cbo_Package.SelectedIndex = 0;
            //}

            //// Fill intPocketPitch From Package Table
            //int intPocketPitch = 0;

            //DataRow[] drPackage = m_dsPackage.Tables[0].Select("Name = '" + cbo_Package.SelectedItem.ToString() + "'");
            //foreach (DataRow package in drPackage)
            //{
            //    intPocketPitch = Convert.ToInt32(package["Pocket Pitch"]);
            //}

            //int newintPocketPitch = objFileHandle.GetValueAsInt("PocketPitch", 0);

            //// delete all the content of combo boxes before new insertion
            //if (cbo_intPocketPitch.Items.Count != 0)
            //    cbo_intPocketPitch.Items.Clear();

            //if ((intPocketPitch & 0x01) == 1)
            //    cbo_intPocketPitch.Items.Add("4");
            //else
            //    cbo_intPocketPitch.Items.Add("Not Used");

            //if ((intPocketPitch & 0x02) == 2)
            //    cbo_intPocketPitch.Items.Add("8");
            //else
            //    cbo_intPocketPitch.Items.Add("Not Used");

            //if ((intPocketPitch & 0x04) == 4)
            //    cbo_intPocketPitch.Items.Add("12");
            //else
            //    cbo_intPocketPitch.Items.Add("Not Used");

            //if ((intPocketPitch & 0x08) == 8)
            //    cbo_intPocketPitch.Items.Add("2");
            //else
            //    cbo_intPocketPitch.Items.Add("Not Used");

            //if (cbo_intPocketPitch.SelectedItem == null || cbo_intPocketPitch.SelectedItem.ToString() == "Not Used")
            //{
            //    intPocketPitch >>= 4;
            //    if ((intPocketPitch & 0x01) == 1)
            //        newintPocketPitch = 0;
            //    else if ((intPocketPitch & 0x02) == 2)
            //        newintPocketPitch = 1;
            //    else if ((intPocketPitch & 0x04) == 4)
            //        newintPocketPitch = 2;
            //}

            cbo_intPocketPitch.SelectedIndex = objFileHandle.GetValueAsInt("PocketPitch", 0); //newintPocketPitch;
            intPocketPitchPrev = cbo_intPocketPitch.SelectedIndex;

            srmCheckBox2.Checked = objFileHandle.GetValueAsBoolean("WantUsePatternCheckUnitPresent", true); //m_smVisionInfo.g_objSeal.ref_blnWantUsePatternCheckUnitPresent;
            srmCheckBox1.Checked = objFileHandle.GetValueAsBoolean("WantUsePixelCheckUnitPresent", true);// _smVisionInfo.g_objSeal.ref_blnWantUsePixelCheckUnitPresent;
        }
        private void UpdateBarcodeGUI()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\Settings.xml";
            m_intBarcodeInspectionArea_Prev = m_smVisionInfo.g_objBarcode.ref_intBarcodeDetectionAreaTolerance;
            m_intBarcodePatternInspectionArea_Top_Prev = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top;
            m_intBarcodePatternInspectionArea_Right_Prev = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right;
            m_intBarcodePatternInspectionArea_Bottom_Prev = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom;
            m_intBarcodePatternInspectionArea_Left_Prev = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left;
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WantUseAngleRange.Checked = objFileHandle.GetValueAsBoolean("WantUseAngleRange", false);
            chk_WantUseGainRange.Checked = objFileHandle.GetValueAsBoolean("WantUseGainRange", false);
            chk_WantUseReferenceImage.Checked = objFileHandle.GetValueAsBoolean("WantUseReferenceImage", false);
            chk_WantUseUniformize3x3.Checked = objFileHandle.GetValueAsBoolean("WantUseUniformize3x3", false);
            txt_BarcodeDetectionAreaTolerance.Text = m_smVisionInfo.g_objBarcode.ref_intBarcodeDetectionAreaTolerance.ToString();
            txt_PatternDetectionAreaTolerance_Top.Text = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top.ToString();
            txt_PatternDetectionAreaTolerance_Right.Text = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right.ToString();
            txt_PatternDetectionAreaTolerance_Bottom.Text = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom.ToString();
            txt_PatternDetectionAreaTolerance_Left.Text = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left.ToString();
            txt_BarcodeRestestCount.Text = m_smVisionInfo.g_objBarcode.ref_intRetestCount.ToString();
            txt_BarcodeDelayTimeAfterPass.Text = m_smVisionInfo.g_objBarcode.ref_intDelayTimeAfterPass.ToString();
            txt_UniformizeGain.Value = m_smVisionInfo.g_objBarcode.ref_intUniformizeGain;

            if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 0)
                pnl_WantUseAngleRange.Visible = true;
            else
                pnl_WantUseAngleRange.Visible = false;
            txt_BarcodeDontCareScale.Value = Convert.ToInt32(m_smVisionInfo.g_objBarcode.ref_fDontCareScale * 100);
            txt_BarcodeOrientationAngle.Text = m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle.ToString();
        }
        /// <summary>
        /// Fill pocket pitch
        /// </summary>
        private void FillPocketPitch()
        {
            // Fill intPocketPitch From Package Table
            int intPocketPitch = 0;

            DataRow[] drPackage = m_dsPackage.Tables[0].Select("Name = '" + cbo_Package.SelectedItem.ToString() + "'");
            foreach (DataRow package in drPackage)
            {
                intPocketPitch = Convert.ToInt32(package["Pocket Pitch"]);
            }

            int newintPocketPitch = 0;

            // delete all the content of combo boxes before new insertion
            if (cbo_intPocketPitch.Items.Count != 0)
                cbo_intPocketPitch.Items.Clear();

            if ((intPocketPitch & 0x01) == 1)
                cbo_intPocketPitch.Items.Add("4");
            else
                cbo_intPocketPitch.Items.Add("Not Used");

            if ((intPocketPitch & 0x02) == 2)
                cbo_intPocketPitch.Items.Add("8");
            else
                cbo_intPocketPitch.Items.Add("Not Used");

            if ((intPocketPitch & 0x04) == 4)
                cbo_intPocketPitch.Items.Add("12");
            else
                cbo_intPocketPitch.Items.Add("Not Used");

            if ((intPocketPitch & 0x08) == 8)
                cbo_intPocketPitch.Items.Add("2");
            else
                cbo_intPocketPitch.Items.Add("Not Used");

            if (cbo_intPocketPitch.SelectedItem == null || cbo_intPocketPitch.SelectedItem.ToString() == "Not Used")
            {
                intPocketPitch >>= 4;
                if ((intPocketPitch & 0x01) == 1)
                    newintPocketPitch = 0;
                else if ((intPocketPitch & 0x02) == 2)
                    newintPocketPitch = 1;
                else if ((intPocketPitch & 0x04) == 4)
                    newintPocketPitch = 2;
            }

            cbo_intPocketPitch.SelectedIndex = newintPocketPitch;
            intPocketPitchPrev = cbo_intPocketPitch.SelectedIndex;
        }

        private void SaveGeneralAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_General))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                    m_smVisionInfo.g_strVisionFolderName + "\\General.xml";
            
            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            if (pnl_WantCheckEmpty.Visible)
            {
                objFileHandle.WriteElement1Value("WantCheckEmpty", chk_WantCheckEmpty.Checked);
                objFileHandle.WriteElement1Value("WantUseEmptyPattern", chk_WantUseEmptyPattern.Checked);
                objFileHandle.WriteElement1Value("WantUseEmptyThreshold", chk_WantUseEmptyThreshold.Checked);
            }
            
            objFileHandle.WriteElement1Value("WantUnitPRFindGauge", chk_WantUnitPRFindGauge.Checked);
           

            if (pnl_WantCheckUnitSitProper.Visible)
                objFileHandle.WriteElement1Value("WantCheckUnitSitProper", chk_WantCheckUnitSitProper.Checked);
            if (pnl_UseAutoRepalceCounter.Visible)
                objFileHandle.WriteElement1Value("UseAutoRepalceCounter", chk_UseAutoRepalceCounter.Checked);

            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);
            
            if (m_smVisionInfo.g_blnWantUseUnitPRFindGauge != chk_WantUnitPRFindGauge.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>General>" + chk_WantUnitPRFindGauge.Text, (!chk_WantUnitPRFindGauge.Checked).ToString(), chk_WantUnitPRFindGauge.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckEmpty != chk_WantCheckEmpty.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>General>" + chk_WantCheckEmpty.Text, (!chk_WantCheckEmpty.Checked).ToString(), chk_WantCheckEmpty.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantUseEmptyPattern != chk_WantUseEmptyPattern.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>General>" + chk_WantUseEmptyPattern.Text, (!chk_WantUseEmptyPattern.Checked).ToString(), chk_WantUseEmptyPattern.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantUseEmptyThreshold != chk_WantUseEmptyThreshold.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>General>" + chk_WantUseEmptyThreshold.Text, (!chk_WantUseEmptyThreshold.Checked).ToString(), chk_WantUseEmptyThreshold.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            m_smVisionInfo.g_blnWantUseUnitPRFindGauge = chk_WantUnitPRFindGauge.Checked;
            m_smVisionInfo.g_blnWantCheckEmpty = chk_WantCheckEmpty.Checked;
            m_smVisionInfo.g_blnWantUseEmptyPattern = chk_WantUseEmptyPattern.Checked;
            m_smVisionInfo.g_blnWantUseEmptyThreshold = chk_WantUseEmptyThreshold.Checked;
            
            if (m_smVisionInfo.g_blnWantCheckEmpty) // Load Position Setting if want check empty
            {
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                    LoadCalibrationSetting(strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
                else
                    LoadCalibrationSetting(strFolderPath + "Calibration.xml");
                ROI.LoadFile(strFolderPath + "Positioning\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
                LGauge.LoadFile(strFolderPath + "Positioning\\Gauge.xml", m_smVisionInfo.g_arrPositioningGauges, m_smVisionInfo.g_WorldShape);

                LoadPositioningSettings(strFolderPath + "Positioning\\");
            }
            else
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;


            if (m_smVisionInfo.g_blnWantCheckUnitSitProper != chk_WantCheckUnitSitProper.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>General>" + chk_WantCheckUnitSitProper.Text, (!chk_WantCheckUnitSitProper.Checked).ToString(), chk_WantCheckUnitSitProper.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnUseAutoRepalceCounter != chk_UseAutoRepalceCounter.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>General>" + chk_UseAutoRepalceCounter.Text, (!chk_UseAutoRepalceCounter.Checked).ToString(), chk_UseAutoRepalceCounter.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            m_smVisionInfo.g_blnWantCheckUnitSitProper = chk_WantCheckUnitSitProper.Checked;
            m_smVisionInfo.g_blnUseAutoRepalceCounter = chk_UseAutoRepalceCounter.Checked;
        }
        private void LoadCalibrationSetting(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("Calibrate");
            m_smVisionInfo.g_fCalibPixelX = objFile.GetValueAsFloat("PixelX", 5);
            m_smVisionInfo.g_fCalibPixelY = objFile.GetValueAsFloat("PixelY", 5);
            m_smVisionInfo.g_fCalibPixelZ = objFile.GetValueAsFloat("PixelZ", 5);
            m_smVisionInfo.g_fCalibOffSetX = objFile.GetValueAsFloat("OffSetX", 0);
            m_smVisionInfo.g_fCalibOffSetY = objFile.GetValueAsFloat("OffSetY", 0);
            m_smVisionInfo.g_fCalibOffSetZ = objFile.GetValueAsFloat("OffSetZ", 0);

            m_smVisionInfo.g_fCalibPixelXInUM = m_smVisionInfo.g_fCalibPixelX / 1000;
            m_smVisionInfo.g_fCalibPixelYInUM = m_smVisionInfo.g_fCalibPixelY / 1000;
            m_smVisionInfo.g_fCalibPixelZInUM = m_smVisionInfo.g_fCalibPixelZ / 1000;
        }
        private void LoadPositioningSettings(string strFolderPath)
        {
            m_smVisionInfo.g_objPositioning.LoadPosition(strFolderPath + "Settings.xml", "General");

            // Load Pattern
            if (File.Exists(strFolderPath + "\\Template\\PRS.mch"))
                m_smVisionInfo.g_objPositioning.LoadPattern(strFolderPath + "\\Template\\PRS.mch");

            if (m_smVisionInfo.g_objPositioning.ref_intMethod == 1)
                m_smVisionInfo.g_objPositioning.LoadPattern(strFolderPath + "Template\\Template0.mch",
                    strFolderPath + "Template\\OrientTemplate0.mch");

            if (m_smVisionInfo.g_blnWantCheckEmpty) 
            {
                if (File.Exists(strFolderPath + "Template\\EmptyTemplate0.mch"))
                    m_smVisionInfo.g_objPositioning.LoadEmptyPattern(strFolderPath + "Template\\EmptyTemplate0.mch");
            }
          
            if (m_smVisionInfo.g_arrPolygon != null)
                Polygon.LoadPolygon(strFolderPath + "\\Template\\Polygon.xml", m_smVisionInfo.g_arrPolygon);
        }
        private void SaveOrientAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Orient) && !tab_Mark.Controls.Contains(group_OrientDirections) && !(m_smVisionInfo.g_strVisionName.Contains("BottomPosition") && (m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Settings.xml";
            
            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);

            objFileHandle.GetFirstSection("Advanced");
            if (radioBtn_2Directions.Checked)
            {
                if (objFileHandle.GetValueAsInt("Direction", 4) == 4)
                {
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Orient", "Advance Setting>" + "Directions", "4", "2", m_smProductionInfo.g_strLotID);
                }
            }
            else
            {
                if (objFileHandle.GetValueAsInt("Direction", 4) == 2)
                {
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Orient", "Advance Setting>" + "Directions", "2", "4", m_smProductionInfo.g_strLotID);
                }
            }

            objFileHandle.WriteSectionElement("Advanced");
            if (radioBtn_2Directions.Checked)
                objFileHandle.WriteElement1Value("Direction", 2);
            else
                objFileHandle.WriteElement1Value("Direction", 4);

            objFileHandle.WriteElement1Value("WantSubROI", chk_WantSubROI.Checked);
            objFileHandle.WriteElement1Value("WantUsePositionCheckOrientation", chk_WantUsePositionCheckOrientation.Checked);
            objFileHandle.WriteElement1Value("CheckPositionOrientationWhenBelowDifferentScore", (Convert.ToSingle(txt_CheckPosOrientBlowDifferentScore.Text) / 100).ToString());
            objFileHandle.WriteElement1Value("PositionWantGauge", chk_PositionWantGauge.Checked);
            objFileHandle.WriteElement1Value("OrientWantGauge", chk_OrientWantPackage.Checked);
            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                {
                    if (radioBtn_2Directions.Checked)
                        m_smVisionInfo.g_arrOrients[i][j].ref_intDirections = 2;
                    else
                        m_smVisionInfo.g_arrOrients[i][j].ref_intDirections = 4;

                    m_smVisionInfo.g_arrOrients[i][j].ref_blnWantUsePositionCheckOrientation = chk_WantUsePositionCheckOrientation.Checked;
                    m_smVisionInfo.g_arrOrients[i][j].ref_fCheckPositionOrientationWhenBelowDifferentScore = Convert.ToSingle(txt_CheckPosOrientBlowDifferentScore.Text) / 100;
                }
            }
            //m_smVisionInfo.g_blnWantSubROI = chk_WantSubROI.Checked;

            //2020 05 08 ZJYEOH : Overwrite template 1 settings to other templates
            if (chk_Set1ToAll.Checked && m_smVisionInfo.g_intTotalTemplates > 1)
            {
                //Orient 
                if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrOrients[u].Count; j++)
                        {
                            XmlParser objFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Template\\Template.xml");
                            objFile.WriteSectionElement("Template" + j);
                            objFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[u][0].ref_fMinScore);
                            objFile.WriteEndElement();

                            m_smVisionInfo.g_arrOrients[u][j].ref_fMinScore = objFile.GetValueAsFloat("MinScore", 0.7f);

                        }
                    }
                }
            }
        }

        private void SaveSealAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Seal) && !tabControl_SettingForm.Controls.Contains(tp_Seal2))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Settings.xml";

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");

            objFileHandle.WriteElement1Value("SealWhiteOnBlack", chk_SealWhiteOnBlack.Checked);

            if (radioBtn_2DirectionsSeal.Checked)
                objFileHandle.WriteElement1Value("Direction", 2);
            else
                objFileHandle.WriteElement1Value("Direction", 4);

            //objFileHandle.WriteElement1Value("PackageName", cbo_Package.SelectedItem.ToString());
            objFileHandle.WriteElement1Value("PocketPitch", cbo_intPocketPitch.SelectedIndex);
            objFileHandle.WriteElement1Value("WantSkipOrient", chk_SkipOrientInspection.Checked);
            objFileHandle.WriteElement1Value("WantSkipSprocketHole", chk_SkipSprocketHoleInspection.Checked);
            objFileHandle.WriteElement1Value("WantSkipSprocketHoleDiameterAndDefect", chk_SkipSprocketHoleDiameterAndDefectInspection.Checked);
            objFileHandle.WriteElement1Value("WantSkipSprocketHoleBrokenAndRoundness", chk_SkipSprocketHoleBrokenAndRoundnessInspection.Checked); 
            objFileHandle.WriteElement1Value("WantUsePatternCheckUnitPresent", srmCheckBox2.Checked);
            objFileHandle.WriteElement1Value("WantUsePixelCheckUnitPresent", srmCheckBox1.Checked);
            objFileHandle.WriteElement1Value("CheckMarkMethod", cbo_CheckSealMarkMethod.SelectedIndex);
            objFileHandle.WriteElement1Value("WantDontCareAreaSeal", chk_WantDontCareArea_Seal.Checked);
            objFileHandle.WriteElement1Value("MarkAreaBelowPercent", txt_MarkAreaBelowPercent.Text);
            objFileHandle.WriteElement1Value("PatternAngleTolerance", txt_PatternAngleTolerance.Text);
            objFileHandle.WriteElement1Value("ClearSealTemplateWhenNewLot", chk_WantClearSealTemplateWhenNewLot.Checked);
            objFileHandle.WriteElement1Value("WantCheckSealEdgeStraightness", chk_WantCheckSealEdgeStraightness.Checked);

            objFileHandle.WriteElement1Value("MaxSealMarkTemplate", Convert.ToInt32(txt_MaxSealMarkTemplate.Text));
            objFileHandle.WriteElement1Value("MaxSealEmptyTemplate", Convert.ToInt32(txt_MaxSealEmptyTemplate.Text));
            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);

            if (m_smVisionInfo.g_objSeal.ref_intSpocketHolePosition != cbo_SealSprocketHolePosition.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>Sprocket Hole Position", cbo_SealSprocketHolePosition.Items[m_smVisionInfo.g_objSeal.ref_intSpocketHolePosition].ToString(), cbo_SealSprocketHolePosition.Items[cbo_SealSprocketHolePosition.SelectedIndex].ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMaxSealMarkTemplate != Convert.ToInt32(txt_MaxSealMarkTemplate.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Max Seal Mark Template", m_smVisionInfo.g_intMaxSealMarkTemplate.ToString(), txt_MaxSealMarkTemplate.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMaxMarkTemplate != Convert.ToInt32(txt_MaxSealEmptyTemplate.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Max Seal Empty Template", m_smVisionInfo.g_intMaxSealEmptyTemplate.ToString(), txt_MaxSealEmptyTemplate.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_blnWhiteOnBlack != chk_SealWhiteOnBlack.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + chk_SealWhiteOnBlack.Text, (!chk_SealWhiteOnBlack.Checked).ToString(), chk_SealWhiteOnBlack.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (radioBtn_2DirectionsSeal.Checked)
            {
                if (m_smVisionInfo.g_objSeal.ref_intDirections == 4)
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Direcions", "4", "2", m_smProductionInfo.g_strLotID);
            }
            else
            {
                if (m_smVisionInfo.g_objSeal.ref_intDirections == 2)
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Direcions", "2", "4", m_smProductionInfo.g_strLotID);
            }

            //if (m_smVisionInfo.g_objSeal.ref_strPackageName != cbo_Package.SelectedItem.ToString())
            //{
            //    STDeviceEdit.SaveDeviceEditLog("Seal", "Advance Setting>" + "Package", m_smVisionInfo.g_objSeal.ref_strPackageName, cbo_Package.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            //}

            if (m_smVisionInfo.g_objSeal.ref_blnWantSkipOrient != chk_SkipOrientInspection.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + chk_SkipOrientInspection.Text, (!chk_SkipOrientInspection.Checked).ToString(), chk_SkipOrientInspection.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole != chk_SkipSprocketHoleInspection.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + chk_SkipSprocketHoleInspection.Text, (!chk_SkipSprocketHoleInspection.Checked).ToString(), chk_SkipSprocketHoleInspection.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect != chk_SkipSprocketHoleDiameterAndDefectInspection.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + chk_SkipSprocketHoleDiameterAndDefectInspection.Text, (!chk_SkipSprocketHoleDiameterAndDefectInspection.Checked).ToString(), chk_SkipSprocketHoleDiameterAndDefectInspection.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness != chk_SkipSprocketHoleBrokenAndRoundnessInspection.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + chk_SkipSprocketHoleBrokenAndRoundnessInspection.Text, (!chk_SkipSprocketHoleBrokenAndRoundnessInspection.Checked).ToString(), chk_SkipSprocketHoleBrokenAndRoundnessInspection.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_blnWantUsePatternCheckUnitPresent != srmCheckBox2.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + srmCheckBox2.Text, (!srmCheckBox2.Checked).ToString(), srmCheckBox2.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_blnWantUsePixelCheckUnitPresent != srmCheckBox1.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + srmCheckBox1.Text, (!srmCheckBox1.Checked).ToString(), srmCheckBox1.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantClearSealTemplateWhenNewLot != chk_WantClearSealTemplateWhenNewLot.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + chk_WantClearSealTemplateWhenNewLot.Text, (!chk_WantClearSealTemplateWhenNewLot.Checked).ToString(), chk_WantClearSealTemplateWhenNewLot.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            if (m_smVisionInfo.g_objSeal.ref_intCheckMarkMethod != cbo_CheckSealMarkMethod.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Check Unit Present Method", cbo_CheckSealMarkMethod.Items[m_smVisionInfo.g_objSeal.ref_intCheckMarkMethod].ToString(), cbo_CheckSealMarkMethod.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_blnWantDontCareArea != chk_WantDontCareArea_Seal.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + chk_WantDontCareArea_Seal.Text, (!chk_WantDontCareArea_Seal.Checked).ToString(), chk_WantDontCareArea_Seal.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_fMarkAreaBelowPercent != Convert.ToSingle(txt_MarkAreaBelowPercent.Text) / 100f)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Mark Area Below Percent", m_smVisionInfo.g_objSeal.ref_fMarkAreaBelowPercent.ToString(), (Convert.ToSingle(txt_MarkAreaBelowPercent.Text) / 100f).ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objSeal.ref_intPatternAngleTolerance != Convert.ToInt32(txt_PatternAngleTolerance.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Pattern Angle Tolerance", m_smVisionInfo.g_objSeal.ref_intPatternAngleTolerance.ToString(), Convert.ToInt32(txt_PatternAngleTolerance.Text).ToString(), m_smProductionInfo.g_strLotID);
            }

            //switch (cbo_intPocketPitch.SelectedIndex)
            //{
            //    case 0:
            //        if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 4)
            //            STDeviceEdit.SaveDeviceEditLog("Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "4", m_smProductionInfo.g_strLotID);
            //        break;
            //    case 1:
            //        if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 8)
            //            STDeviceEdit.SaveDeviceEditLog("Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "8", m_smProductionInfo.g_strLotID);
            //        break;
            //    case 2:
            //        if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 12)
            //            STDeviceEdit.SaveDeviceEditLog("Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "12", m_smProductionInfo.g_strLotID);
            //        break;
            //    case 3:
            //        if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2)
            //            STDeviceEdit.SaveDeviceEditLog("Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "2", m_smProductionInfo.g_strLotID);
            //        break;
            //}
            switch (cbo_intPocketPitch.SelectedIndex)
            {
                case 0:
                    if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2)
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "2", m_smProductionInfo.g_strLotID);
                    break;
                case 1:
                    if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 4)
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "4", m_smProductionInfo.g_strLotID);
                    break;
                case 2:
                    if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 8)
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "8", m_smProductionInfo.g_strLotID);
                    break;
                case 3:
                    if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 12)
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "12", m_smProductionInfo.g_strLotID);
                    break;
                case 4:
                    if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 16)
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Advance Setting>" + "Pocket Pitch", m_smVisionInfo.g_objSeal.ref_intTapePocketPitch.ToString(), "16", m_smProductionInfo.g_strLotID);
                    break;
            }
            
            m_smVisionInfo.g_objSeal.ref_intSpocketHolePosition = cbo_SealSprocketHolePosition.SelectedIndex;
            m_smVisionInfo.g_objSeal.ref_blnWhiteOnBlack = chk_SealWhiteOnBlack.Checked;

            if (radioBtn_2DirectionsSeal.Checked)
                m_smVisionInfo.g_objSeal.ref_intDirections = 2;
            else
                m_smVisionInfo.g_objSeal.ref_intDirections = 4;

            //m_smVisionInfo.g_objSeal.ref_strPackageName = cbo_Package.SelectedItem.ToString();
            m_smVisionInfo.g_objSeal.ref_blnWantSkipOrient = chk_SkipOrientInspection.Checked;
            m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole = chk_SkipSprocketHoleInspection.Checked;
            m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect = chk_SkipSprocketHoleDiameterAndDefectInspection.Checked;
            m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness = chk_SkipSprocketHoleBrokenAndRoundnessInspection.Checked;
            m_smVisionInfo.g_objSeal.ref_blnWantUsePatternCheckUnitPresent = srmCheckBox2.Checked;
            m_smVisionInfo.g_objSeal.ref_blnWantUsePixelCheckUnitPresent = srmCheckBox1.Checked;
            m_smVisionInfo.g_objSeal.SetGrabImageIndex(0, cbo_SprocketHoleViewNo.SelectedIndex);
            m_smVisionInfo.g_objSeal.SetGrabImageIndex(6, cbo_SprocketHoleViewNo.SelectedIndex);
            m_smVisionInfo.g_objSeal.SetGrabImageIndex(1, cbo_SealBrokenViewNo.SelectedIndex);
            m_smVisionInfo.g_objSeal.SetGrabImageIndex(2, cbo_SealBrokenViewNo.SelectedIndex);
            m_smVisionInfo.g_objSeal.SetGrabImageIndex(3, cbo_SealDistanceViewNo.SelectedIndex);
            m_smVisionInfo.g_objSeal.SetGrabImageIndex(4, cbo_SealOverheatViewNo.SelectedIndex);
            m_smVisionInfo.g_objSeal.SetGrabImageIndex(5, cbo_CheckUnitViewNo.SelectedIndex);
            m_smVisionInfo.g_objSeal.SetGrabImageIndex(7, cbo_SealEdgeStraightnessViewNo.SelectedIndex);
            m_smVisionInfo.g_blnWantClearSealTemplateWhenNewLot = chk_WantClearSealTemplateWhenNewLot.Checked;
            m_smVisionInfo.g_intMaxSealMarkTemplate = Convert.ToInt32(txt_MaxSealMarkTemplate.Text);
            m_smVisionInfo.g_intMaxSealEmptyTemplate = Convert.ToInt32(txt_MaxSealEmptyTemplate.Text);
            if (m_smVisionInfo.g_intPocketTemplateTotal > m_smVisionInfo.g_intMaxSealEmptyTemplate)
                m_smVisionInfo.g_intPocketTemplateTotal = m_smVisionInfo.g_intMaxSealEmptyTemplate;
            if (m_smVisionInfo.g_intMarkTemplateTotal > m_smVisionInfo.g_intMaxSealMarkTemplate)
                m_smVisionInfo.g_intMarkTemplateTotal = m_smVisionInfo.g_intMaxSealMarkTemplate;

            if (m_smVisionInfo.g_objSeal.ref_intCheckMarkMethod != cbo_CheckSealMarkMethod.SelectedIndex)
            {
                m_smVisionInfo.g_objSeal.ref_intCheckMarkMethod = cbo_CheckSealMarkMethod.SelectedIndex;

                // Relearn Seal mark pattern if change Mark Inspect Method
                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Template\\Mark\\";

                m_smVisionInfo.g_objSeal.RelearnMarkMatcherPattern(strFolderPath, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_smVisionInfo.g_objSeal.LoadMarkPattern4Direction(strFolderPath);
            }
            m_smVisionInfo.g_objSeal.ref_blnWantDontCareArea = chk_WantDontCareArea_Seal.Checked;
            m_smVisionInfo.g_objSeal.ref_fMarkAreaBelowPercent = Convert.ToSingle(txt_MarkAreaBelowPercent.Text) / 100f;
            m_smVisionInfo.g_objSeal.ref_intPatternAngleTolerance = Convert.ToInt32(txt_PatternAngleTolerance.Text);
            m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness = chk_WantCheckSealEdgeStraightness.Checked;

            switch (cbo_intPocketPitch.SelectedIndex)
            {
                case 0:
                    m_smVisionInfo.g_objSeal.ref_intTapePocketPitch = 2;
                    break;
                case 1:
                    m_smVisionInfo.g_objSeal.ref_intTapePocketPitch = 4;
                    break;
                case 2:
                    m_smVisionInfo.g_objSeal.ref_intTapePocketPitch = 8;
                    break;
                case 3:
                    m_smVisionInfo.g_objSeal.ref_intTapePocketPitch = 12;
                    break;
                case 4:
                    m_smVisionInfo.g_objSeal.ref_intTapePocketPitch = 16;
                    break;
            }

            m_smVisionInfo.g_objSeal.SaveSeal(strPath, false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);
        }

        private void SaveMarkAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Mark))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Settings.xml";

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WhiteOnBlack", chk_WhiteOnBlack.Checked);
            objFileHandle.WriteElement1Value("WantMultiGroups", chk_MultiGroups.Checked);
            objFileHandle.WriteElement1Value("WantBuildTexts", chk_WantBuildTexts.Checked);
            objFileHandle.WriteElement1Value("WantMultiTemplates", true);   //chk_MultiTemplates.Checked);
            objFileHandle.WriteElement1Value("WantSetTemplateBasedOnBinInfo_PurposelyRename", chk_SetTemplateBasedOnBinInfo.Checked);
            objFileHandle.WriteElement1Value("WantSet1ToAll", chk_Set1ToAll.Checked);
            objFileHandle.WriteElement1Value("WantSkipMark", chk_SkipMarkInspection.Checked);
            objFileHandle.WriteElement1Value("WantDontCareIgnoredMarkWholeArea", chk_WantDontCareIgnoredMarkWholeArea.Checked);
            objFileHandle.WriteElement1Value("WantPin1", chk_WantPin1.Checked);
            objFileHandle.WriteElement1Value("Pin1PositionControl", cbo_Pin1PositionControl.SelectedIndex);
            objFileHandle.WriteElement1Value("WantUsePin1OrientationWhenNoMark", chk_WantUsePin1OrientationWhenNoMark.Checked);
            objFileHandle.WriteElement1Value("Pin1ImageNo", cbo_Pin1ImageNo.SelectedIndex);
            objFileHandle.WriteElement1Value("WantGaugeMeasureMarkDimension", chk_WantUseGaugeForMeasureMarkPkg.Checked);
            objFileHandle.WriteElement1Value("WantClearMarkTemplateWhenNewLot", chk_WantClearMarkTemplateWhenNewLot.Checked);
            objFileHandle.WriteElement1Value("WantCheckNoMark", chk_WantCheckNoMark.Checked);
            objFileHandle.WriteElement1Value("WantCheckContourOnMark", chk_WantCheckContourOnMark.Checked);
            objFileHandle.WriteElement1Value("WantMark2DCode", chk_WantMark2DCode.Checked);
            objFileHandle.WriteElement1Value("WantDontCareAreaMark", chk_WantDontCareArea_Mark.Checked);
            objFileHandle.WriteElement1Value("WantSampleAreaScore", chk_WantSampleAreaScore.Checked);
            objFileHandle.WriteElement1Value("WantRotateMarkImageUsingPkgAngle", chk_WantRotateMarkImageUsingPkgAngle.Checked);
            objFileHandle.WriteElement1Value("WantCheckMarkAngle", chk_WantCheckMarkAngle.Checked);
            objFileHandle.WriteElement1Value("UseDefaultMarkScoreAfterNewLotClearTemplate", chk_UseDefaultMarkScoreAfterClearTemplate.Checked);
            objFileHandle.WriteElement1Value("SeparateExtraMarkThreshold", chk_SeparateExtraMarkThreshold.Checked);
            objFileHandle.WriteElement1Value("WantExcessMarkThresholdFollowExtraMarkThreshold", chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Checked);
            //objFileHandle.WriteElement1Value("WantUseLeadPointOffsetMarkROI", chk_WantUseLeadPointOffsetMarkROI.Checked);
            objFileHandle.WriteElement1Value("WantRemoveBorderWhenLearnMark", chk_WantRemoveBorderWhenLearnMark.Checked);
            objFileHandle.WriteElement1Value("WantCheckBarPin1", chk_WantCheckBarPin1.Checked);
            objFileHandle.WriteElement1Value("WantCheckBrokenMark", chk_WantCheckBrokenMark.Checked);
            objFileHandle.WriteElement1Value("WantCheckTotalExcessMark", chk_WantCheckTotalExcessMark.Checked);
            objFileHandle.WriteElement1Value("WantCheckMarkAverageGrayValue", chk_WantCheckMarkAverageGrayValue.Checked);
            objFileHandle.WriteElement1Value("WantUseOCROnly", chk_WantUseOCR.Checked);
            objFileHandle.WriteElement1Value("WantUseOCRandOCV", chk_WantUseOCRnOCV.Checked);
            objFileHandle.WriteElement1Value("WantUseUnitPatternAsMarkPattern", chk_WantUseUnitPatternAsMarkPattern.Checked);
            objFileHandle.WriteElement1Value("WantUseExcessMissingMarkAffectScore", chk_WantUseExcessMissingMarkAffectScore.Checked);
            objFileHandle.WriteElement1Value("MarkCharROIOffsetX", m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetX_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetX.Text)));
            objFileHandle.WriteElement1Value("MarkCharROIOffsetY", m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetY_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetY.Text)));
            objFileHandle.WriteElement1Value("WantUseMarkTypeInspectionSetting", chk_WantUseMarkTypeInspectionSetting.Checked);
            objFileHandle.WriteElement1Value("ExtraExcessMarkInspectionAreaCutMode", cbo_ExtraExcessMarkInspectionAreaCutMode.SelectedIndex);
            objFileHandle.WriteElement1Value("CompensateMarkDiffSizeMode", cbo_CompensateMarkDiffSizeMode.SelectedIndex);
            objFileHandle.WriteElement1Value("MarkScoreMode", cbo_MarkScoreMode.SelectedIndex);
            objFileHandle.WriteElement1Value("CodeType", cbo_2DCodeType.SelectedIndex);
            objFileHandle.WriteElement1Value("MissingMarkInspectionMethod", cbo_MissingMarkInspectionMethod.SelectedIndex);
            objFileHandle.WriteElement1Value("DefaultMarkScore", Convert.ToInt32(txt_DefaultMarkScore.Text));
            objFileHandle.WriteElement1Value("MarkScoreOffset", Convert.ToInt32(txt_MarkScoreOffset.Text));
            objFileHandle.WriteElement1Value("MarkOriPositionScore", Convert.ToInt32(txt_MarkOriPositionScore.Text));
            objFileHandle.WriteElement1Value("CheckMarkAngleMinMaxTolerance", Convert.ToInt32(txt_CheckMarkAngleMinMaxTolerance.Text));
            objFileHandle.WriteElement1Value("MinMarkScore", Convert.ToInt32(txt_MinMarkScore.Text));
            objFileHandle.WriteElement1Value("MaxMarkTemplate", Convert.ToInt32(txt_MaxMarkTemplate.Text));
            objFileHandle.WriteElement1Value("NoMarkMaximumBlob", m_smVisionInfo.g_arrMarks[0].GetNoMarkMaximumBlobArea_InPixel((float)Convert.ToDouble(txt_NoMarkMaximumBlobArea.Text)));
            objFileHandle.WriteElement1Value("MarkDefectInspectionMethod", cbo_MarkDefectInspectionMethod.SelectedIndex);
            objFileHandle.WriteElement1Value("MarkTextShiftMethod", cbo_MarkTextShiftMethod.SelectedIndex);
            objFileHandle.WriteElement1Value("FinalReduction_Direction", cbo_FinalReduction_Direction.SelectedIndex);
            objFileHandle.WriteElement1Value("FinalReduction_MarkDeg", cbo_FinalReduction_MarkDeg.SelectedIndex);
            objFileHandle.WriteElement1Value("WantCheckCharMissingMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharMissingMark);
            objFileHandle.WriteElement1Value("WantCheckCharBrokenMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharBrokenMark);
            objFileHandle.WriteElement1Value("WantCheckCharExcessMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharExcessMark);
            objFileHandle.WriteElement1Value("WantCheckLogoExcessMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoExcessMark);
            objFileHandle.WriteElement1Value("WantCheckLogoMissingMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoMissingMark);
            objFileHandle.WriteElement1Value("WantCheckLogoBrokenMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoBrokenMark);
            objFileHandle.WriteElement1Value("WantCheckSymbol1ExcessMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1ExcessMark);
            objFileHandle.WriteElement1Value("WantCheckSymbol1MissingMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1MissingMark);
            objFileHandle.WriteElement1Value("WantCheckSymbol1BrokenMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1BrokenMark);
            objFileHandle.WriteElement1Value("WantCheckSymbol2ExcessMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2ExcessMark);
            objFileHandle.WriteElement1Value("WantCheckSymbol2MissingMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2MissingMark);
            objFileHandle.WriteElement1Value("WantCheckSymbol2BrokenMark", m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2BrokenMark);

            if (cbo_RotationInterpolation_Mark.SelectedIndex == 0)
                objFileHandle.WriteElement1Value("RotationInterpolation_Mark", 0);
            else
                objFileHandle.WriteElement1Value("RotationInterpolation_Mark", 4);

            if (cbo_RotationInterpolation_PkgBright.SelectedIndex == 0)
                objFileHandle.WriteElement1Value("RotationInterpolation_PkgBright", 0);
            else
                objFileHandle.WriteElement1Value("RotationInterpolation_PkgBright", 4);

            if (cbo_RotationInterpolation_PkgDark.SelectedIndex == 0)
                objFileHandle.WriteElement1Value("RotationInterpolation_PkgDark", 0);
            else
                objFileHandle.WriteElement1Value("RotationInterpolation_PkgDark", 4);
            objFileHandle.WriteEndElement();

            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);
            
            if (m_smVisionInfo.g_blnWhiteOnBlack != chk_WhiteOnBlack.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WhiteOnBlack.Text, (!chk_WhiteOnBlack.Checked).ToString(), chk_WhiteOnBlack.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantMultiGroups != chk_MultiGroups.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_MultiGroups.Text, (!chk_MultiGroups.Checked).ToString(), chk_MultiGroups.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantBuildTexts != chk_WantBuildTexts.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantBuildTexts.Text, (!chk_WantBuildTexts.Checked).ToString(), chk_WantBuildTexts.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantMultiTemplates != chk_MultiTemplates.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_MultiTemplates.Text, (!chk_MultiTemplates.Checked).ToString(), chk_MultiTemplates.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantSetTemplateBasedOnBinInfo != chk_SetTemplateBasedOnBinInfo.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_SetTemplateBasedOnBinInfo.Text, (!chk_SetTemplateBasedOnBinInfo.Checked).ToString(), chk_SetTemplateBasedOnBinInfo.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantSet1ToAll != chk_Set1ToAll.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_Set1ToAll.Text, (!chk_Set1ToAll.Checked).ToString(), chk_Set1ToAll.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantPin1 != chk_WantPin1.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantPin1.Text, (!chk_WantPin1.Checked).ToString(), chk_WantPin1.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_WantUsePin1OrientationWhenNoMark != chk_WantUsePin1OrientationWhenNoMark.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Orient", "Advance Setting>" + chk_WantUsePin1OrientationWhenNoMark.Text, (!chk_WantUsePin1OrientationWhenNoMark.Checked).ToString(), chk_WantUsePin1OrientationWhenNoMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantGauge != chk_WantUseGaugeForMeasureMarkPkg.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantUseGaugeForMeasureMarkPkg.Text, (!chk_WantUseGaugeForMeasureMarkPkg.Checked).ToString(), chk_WantUseGaugeForMeasureMarkPkg.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantClearMarkTemplateWhenNewLot != chk_WantClearMarkTemplateWhenNewLot.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantClearMarkTemplateWhenNewLot.Text, (!chk_WantClearMarkTemplateWhenNewLot.Checked).ToString(), chk_WantClearMarkTemplateWhenNewLot.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckNoMark != chk_WantCheckNoMark.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantCheckNoMark.Text, (!chk_WantCheckNoMark.Checked).ToString(), chk_WantCheckNoMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckContourOnMark != chk_WantCheckContourOnMark.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantCheckContourOnMark.Text, (!chk_WantCheckContourOnMark.Checked).ToString(), chk_WantCheckContourOnMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantMark2DCode != chk_WantMark2DCode.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantMark2DCode.Text, (!chk_WantMark2DCode.Checked).ToString(), chk_WantMark2DCode.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantDontCareArea_Mark != chk_WantDontCareArea_Mark.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantDontCareArea_Mark.Text, (!chk_WantDontCareArea_Mark.Checked).ToString(), chk_WantDontCareArea_Mark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantRotateMarkImageUsingPkgAngle != chk_WantRotateMarkImageUsingPkgAngle.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantRotateMarkImageUsingPkgAngle.Text, (!chk_WantRotateMarkImageUsingPkgAngle.Checked).ToString(), chk_WantRotateMarkImageUsingPkgAngle.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckMarkAngle != chk_WantCheckMarkAngle.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantCheckMarkAngle.Text, (!chk_WantCheckMarkAngle.Checked).ToString(), chk_WantCheckMarkAngle.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnUseDefaultMarkScoreAfterClearTemplate != chk_UseDefaultMarkScoreAfterClearTemplate.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_UseDefaultMarkScoreAfterClearTemplate.Text, (!chk_UseDefaultMarkScoreAfterClearTemplate.Checked).ToString(), chk_UseDefaultMarkScoreAfterClearTemplate.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnSeparateExtraMarkThreshold != chk_SeparateExtraMarkThreshold.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_SeparateExtraMarkThreshold.Text, (!chk_SeparateExtraMarkThreshold.Checked).ToString(), chk_SeparateExtraMarkThreshold.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            if (m_smVisionInfo.g_blnWantExcessMarkThresholdFollowExtraMarkThreshold != chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Text, (!chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Checked).ToString(), chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            //if (m_smVisionInfo.g_blnWantUseLeadPointOffsetMarkROI != chk_WantUseLeadPointOffsetMarkROI.Checked)
            //{
            //    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantUseLeadPointOffsetMarkROI.Text, (!chk_WantUseLeadPointOffsetMarkROI.Checked).ToString(), chk_WantUseLeadPointOffsetMarkROI.Checked.ToString(), m_smProductionInfo.g_strLotID);
            //}

            if (m_smVisionInfo.g_blnWantRemoveBorderWhenLearnMark != chk_WantRemoveBorderWhenLearnMark.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantRemoveBorderWhenLearnMark.Text, (!chk_WantRemoveBorderWhenLearnMark.Checked).ToString(), chk_WantRemoveBorderWhenLearnMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckMarkBroken != chk_WantCheckBrokenMark.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantCheckBrokenMark.Text, (!chk_WantCheckBrokenMark.Checked).ToString(), chk_WantCheckBrokenMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckMarkTotalExcess != chk_WantCheckTotalExcessMark.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantCheckTotalExcessMark.Text, (!chk_WantCheckTotalExcessMark.Checked).ToString(), chk_WantCheckTotalExcessMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue != chk_WantCheckMarkAverageGrayValue.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantCheckMarkAverageGrayValue.Text, (!chk_WantCheckMarkAverageGrayValue.Checked).ToString(), chk_WantCheckMarkAverageGrayValue.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantUseUnitPatternAsMarkPattern != chk_WantUseUnitPatternAsMarkPattern.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantUseUnitPatternAsMarkPattern.Text, (!chk_WantUseUnitPatternAsMarkPattern.Checked).ToString(), chk_WantUseUnitPatternAsMarkPattern.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_fMarkCharROIOffsetX != m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetX_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetX.Text)))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Mark Char ROI Offset X", (m_smVisionInfo.g_fMarkCharROIOffsetX).ToString(), m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetX_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetX.Text)).ToString(GetDecimalFormat()), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_fMarkCharROIOffsetY != m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetY_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetY.Text)))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Mark Char ROI Offset Y", (m_smVisionInfo.g_fMarkCharROIOffsetY).ToString(), m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetY_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetY.Text)).ToString(GetDecimalFormat()), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantUseMarkTypeInspectionSetting != chk_WantUseMarkTypeInspectionSetting.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantUseMarkTypeInspectionSetting.Text, (!chk_WantUseMarkTypeInspectionSetting.Checked).ToString(), chk_WantUseMarkTypeInspectionSetting.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckBarPin1 != chk_WantCheckBarPin1.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantCheckBarPin1.Text, (!chk_WantCheckBarPin1.Checked).ToString(), chk_WantCheckBarPin1.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intExtraExcessMarkInspectionAreaCutMode != cbo_ExtraExcessMarkInspectionAreaCutMode.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting> Extra/Excess Mark Inspection Area Cut Mode", cbo_ExtraExcessMarkInspectionAreaCutMode.Items[m_smVisionInfo.g_intExtraExcessMarkInspectionAreaCutMode].ToString(), cbo_ExtraExcessMarkInspectionAreaCutMode.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intCompensateMarkDiffSizeMode != cbo_CompensateMarkDiffSizeMode.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting> Compensate Mark Different Size Mode", cbo_CompensateMarkDiffSizeMode.Items[m_smVisionInfo.g_intCompensateMarkDiffSizeMode].ToString(), cbo_CompensateMarkDiffSizeMode.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMarkScoreMode != cbo_MarkScoreMode.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting> Mark Score Mode", cbo_MarkScoreMode.Items[m_smVisionInfo.g_intMarkScoreMode].ToString(), cbo_MarkScoreMode.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_int2DCodeType != cbo_2DCodeType.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "2D Code Type", cbo_2DCodeType.Items[m_smVisionInfo.g_int2DCodeType].ToString(), cbo_2DCodeType.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intDefaultMarkScore != Convert.ToInt32(txt_DefaultMarkScore.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Default Mark Score", m_smVisionInfo.g_intDefaultMarkScore.ToString(), txt_DefaultMarkScore.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMarkScoreOffset != Convert.ToInt32(txt_MarkScoreOffset.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Mark Score Offset", m_smVisionInfo.g_intMarkScoreOffset.ToString(), txt_MarkScoreOffset.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMarkOriPositionScore != Convert.ToInt32(txt_MarkOriPositionScore.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Mark Ori Position Score", m_smVisionInfo.g_intMarkOriPositionScore.ToString(), txt_MarkOriPositionScore.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intCheckMarkAngleMinMaxTolerance != Convert.ToInt32(txt_CheckMarkAngleMinMaxTolerance.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Check Mark Angle Min Max Tolerance", m_smVisionInfo.g_intCheckMarkAngleMinMaxTolerance.ToString(), txt_CheckMarkAngleMinMaxTolerance.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMinMarkScore != Convert.ToInt32(txt_MinMarkScore.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Min Mark Score", m_smVisionInfo.g_intMinMarkScore.ToString(), txt_MinMarkScore.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMaxMarkTemplate != Convert.ToInt32(txt_MaxMarkTemplate.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Max Mark Template", m_smVisionInfo.g_intMaxMarkTemplate.ToString(), txt_MaxMarkTemplate.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMarkDefectInspectionMethod != cbo_MarkDefectInspectionMethod.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Mark Defect Inspection Method", cbo_MarkDefectInspectionMethod.Items[m_smVisionInfo.g_intMarkDefectInspectionMethod].ToString(), cbo_MarkDefectInspectionMethod.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMarkTextShiftMethod != cbo_MarkTextShiftMethod.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Mark Text Shift Method", cbo_MarkTextShiftMethod.Items[m_smVisionInfo.g_intMarkTextShiftMethod].ToString(), cbo_MarkTextShiftMethod.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intFinalReduction_Direction != cbo_FinalReduction_Direction.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Final Reduction Direction", cbo_FinalReduction_Direction.Items[m_smVisionInfo.g_intFinalReduction_Direction].ToString(), cbo_FinalReduction_Direction.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intFinalReduction_MarkDeg != cbo_FinalReduction_MarkDeg.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Final Reduction MarkDeg", cbo_FinalReduction_MarkDeg.Items[m_smVisionInfo.g_intFinalReduction_MarkDeg].ToString(), cbo_FinalReduction_MarkDeg.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnUseOCR != chk_WantUseOCR.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantUseOCR.Text, (!chk_WantUseOCR.Checked).ToString(), chk_WantUseOCR.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            if (m_smVisionInfo.g_blnUseOCRandOCV != chk_WantUseOCRnOCV.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantUseOCRnOCV.Text, (!chk_WantUseOCRnOCV.Checked).ToString(), chk_WantUseOCRnOCV.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            int intInterpolationValue = 4;
            if (cbo_RotationInterpolation_Mark.SelectedIndex == 0)
                intInterpolationValue = 0;
            if (m_smVisionInfo.g_intRotationInterpolation_Mark != intInterpolationValue)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Rotation Interpolation Mark", m_smVisionInfo.g_intRotationInterpolation_Mark.ToString(), intInterpolationValue.ToString(), m_smProductionInfo.g_strLotID);
            }

            intInterpolationValue = 4;
            if (cbo_RotationInterpolation_PkgBright.SelectedIndex == 0)
                intInterpolationValue = 0;
            if (m_smVisionInfo.g_intRotationInterpolation_PkgBright != intInterpolationValue)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Rotation Interpolation Pkg Bright", m_smVisionInfo.g_intRotationInterpolation_PkgBright.ToString(), intInterpolationValue.ToString(), m_smProductionInfo.g_strLotID);
            }

            intInterpolationValue = 4;
            if (cbo_RotationInterpolation_PkgDark.SelectedIndex == 0)
                intInterpolationValue = 0;
            if (m_smVisionInfo.g_intRotationInterpolation_PkgDark != intInterpolationValue)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Rotation Interpolation Pkg Dark", m_smVisionInfo.g_intRotationInterpolation_PkgDark.ToString(), intInterpolationValue.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intRotationInterpolation_PkgDark != intInterpolationValue)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Rotation Interpolation Pkg Dark", m_smVisionInfo.g_intRotationInterpolation_PkgDark.ToString(), intInterpolationValue.ToString(), m_smProductionInfo.g_strLotID);
            }
            if (m_smVisionInfo.g_blnWantCheckCharMissingMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharMissingMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Char Missing Mark", m_smVisionInfo.g_blnWantCheckCharMissingMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharMissingMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckCharBrokenMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharBrokenMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Char Broken Mark", m_smVisionInfo.g_blnWantCheckCharBrokenMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharBrokenMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckLogoExcessMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoExcessMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Logo Excess Mark", m_smVisionInfo.g_blnWantCheckLogoExcessMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoExcessMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckLogoMissingMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoMissingMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Logo Missing Mark", m_smVisionInfo.g_blnWantCheckLogoMissingMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoMissingMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckLogoBrokenMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoBrokenMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Logo Broken Mark", m_smVisionInfo.g_blnWantCheckLogoBrokenMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoBrokenMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckSymbol1ExcessMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1ExcessMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Symbol1 Excess Mark", m_smVisionInfo.g_blnWantCheckSymbol1ExcessMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1ExcessMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckSymbol1MissingMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1MissingMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Symbol1 Missing Mark", m_smVisionInfo.g_blnWantCheckSymbol1MissingMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1MissingMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckSymbol1BrokenMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1BrokenMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Symbol1 Broken Mark", m_smVisionInfo.g_blnWantCheckSymbol1BrokenMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1BrokenMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckSymbol2ExcessMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2ExcessMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Symbol2 Excess Mark", m_smVisionInfo.g_blnWantCheckSymbol2ExcessMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2ExcessMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckSymbol2MissingMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2MissingMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Symbol2 Missing Mark", m_smVisionInfo.g_blnWantCheckSymbol2MissingMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2MissingMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckSymbol2BrokenMark != m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2BrokenMark)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Want Check Symbol2 Broken Mark", m_smVisionInfo.g_blnWantCheckSymbol2BrokenMark.ToString(), m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2BrokenMark.ToString(), m_smProductionInfo.g_strLotID);
            }

            m_smVisionInfo.g_blnWantCheckCharExcessMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharExcessMark;
            m_smVisionInfo.g_blnWantCheckCharMissingMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharMissingMark;
            m_smVisionInfo.g_blnWantCheckCharBrokenMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckCharBrokenMark;
            m_smVisionInfo.g_blnWantCheckLogoExcessMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoExcessMark;
            m_smVisionInfo.g_blnWantCheckLogoMissingMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoMissingMark;
            m_smVisionInfo.g_blnWantCheckLogoBrokenMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckLogoBrokenMark;
            m_smVisionInfo.g_blnWantCheckSymbol1ExcessMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1ExcessMark;
            m_smVisionInfo.g_blnWantCheckSymbol1MissingMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1MissingMark;
            m_smVisionInfo.g_blnWantCheckSymbol1BrokenMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol1BrokenMark;
            m_smVisionInfo.g_blnWantCheckSymbol2ExcessMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2ExcessMark;
            m_smVisionInfo.g_blnWantCheckSymbol2MissingMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2MissingMark;
            m_smVisionInfo.g_blnWantCheckSymbol2BrokenMark = m_smVisionInfo.g_arrMarks[0].ref_blnWantCheckSymbol2BrokenMark;

            m_smVisionInfo.g_blnWhiteOnBlack = chk_WhiteOnBlack.Checked;
            m_smVisionInfo.g_blnWantMultiGroups = chk_MultiGroups.Checked;
            m_smVisionInfo.g_blnWantBuildTexts = chk_WantBuildTexts.Checked;
            m_smVisionInfo.g_blnWantMultiTemplates = true; // chk_MultiTemplates.Checked;   2021 04 23 - CCENG: Always true. If user does not want multi template, can set template count to 1
            m_smVisionInfo.g_blnWantSetTemplateBasedOnBinInfo = chk_SetTemplateBasedOnBinInfo.Checked;
            m_smVisionInfo.g_blnWantSet1ToAll = chk_Set1ToAll.Checked;
            m_smVisionInfo.g_blnWantPin1 = chk_WantPin1.Checked;
            m_smVisionInfo.g_WantUsePin1OrientationWhenNoMark = chk_WantUsePin1OrientationWhenNoMark.Checked;
            m_smVisionInfo.g_blnWantGauge = chk_WantUseGaugeForMeasureMarkPkg.Checked;
            m_smVisionInfo.g_blnWantClearMarkTemplateWhenNewLot = chk_WantClearMarkTemplateWhenNewLot.Checked;
            m_smVisionInfo.g_blnWantCheckNoMark = chk_WantCheckNoMark.Checked;
            m_smVisionInfo.g_blnWantCheckContourOnMark = chk_WantCheckContourOnMark.Checked;
            m_smVisionInfo.g_blnWantMark2DCode = chk_WantMark2DCode.Checked;
            m_smVisionInfo.g_blnWantDontCareArea_Mark = chk_WantDontCareArea_Mark.Checked;
            m_smVisionInfo.g_blnWantRotateMarkImageUsingPkgAngle = chk_WantRotateMarkImageUsingPkgAngle.Checked;
            m_smVisionInfo.g_blnWantCheckMarkAngle = chk_WantCheckMarkAngle.Checked;
            m_smVisionInfo.g_blnUseDefaultMarkScoreAfterClearTemplate = chk_UseDefaultMarkScoreAfterClearTemplate.Checked;
            m_smVisionInfo.g_blnSeparateExtraMarkThreshold = chk_SeparateExtraMarkThreshold.Checked;
            m_smVisionInfo.g_blnWantExcessMarkThresholdFollowExtraMarkThreshold = chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Checked;
            //m_smVisionInfo.g_blnWantUseLeadPointOffsetMarkROI = chk_WantUseLeadPointOffsetMarkROI.Checked;
            m_smVisionInfo.g_blnWantRemoveBorderWhenLearnMark = chk_WantRemoveBorderWhenLearnMark.Checked;
            m_smVisionInfo.g_blnWantCheckBarPin1 = chk_WantCheckBarPin1.Checked;
            m_smVisionInfo.g_intExtraExcessMarkInspectionAreaCutMode = cbo_ExtraExcessMarkInspectionAreaCutMode.SelectedIndex;
            m_smVisionInfo.g_intCompensateMarkDiffSizeMode = cbo_CompensateMarkDiffSizeMode.SelectedIndex;
            m_smVisionInfo.g_intMarkScoreMode = cbo_MarkScoreMode.SelectedIndex;
            m_smVisionInfo.g_blnWantCheckMarkBroken = chk_WantCheckBrokenMark.Checked;
            m_smVisionInfo.g_blnWantCheckMarkTotalExcess = chk_WantCheckTotalExcessMark.Checked;
            m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue = chk_WantCheckMarkAverageGrayValue.Checked;
            m_smVisionInfo.g_blnUseOCR = chk_WantUseOCR.Checked;
            m_smVisionInfo.g_blnUseOCRandOCV = chk_WantUseOCRnOCV.Checked;
            m_smVisionInfo.g_blnWantUseUnitPatternAsMarkPattern = chk_WantUseUnitPatternAsMarkPattern.Checked;
            m_smVisionInfo.g_fMarkCharROIOffsetX = m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetX_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetX.Text));
            m_smVisionInfo.g_fMarkCharROIOffsetY = m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetY_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetY.Text));
            m_smVisionInfo.g_blnWantUseMarkTypeInspectionSetting = chk_WantUseMarkTypeInspectionSetting.Checked;
            m_smVisionInfo.g_int2DCodeType = cbo_2DCodeType.SelectedIndex;
            m_smVisionInfo.g_intDefaultMarkScore = Convert.ToInt32(txt_DefaultMarkScore.Text);
            m_smVisionInfo.g_intMarkScoreOffset = Convert.ToInt32(txt_MarkScoreOffset.Text);
            m_smVisionInfo.g_intMarkOriPositionScore = Convert.ToInt32(txt_MarkOriPositionScore.Text);
            m_smVisionInfo.g_intCheckMarkAngleMinMaxTolerance = Convert.ToInt32(txt_CheckMarkAngleMinMaxTolerance.Text);
            m_smVisionInfo.g_intMinMarkScore = Convert.ToInt32(txt_MinMarkScore.Text);
            m_smVisionInfo.g_intMaxMarkTemplate = Convert.ToInt32(txt_MaxMarkTemplate.Text);
            m_smVisionInfo.g_intMarkDefectInspectionMethod = cbo_MarkDefectInspectionMethod.SelectedIndex;
            m_smVisionInfo.g_intMarkTextShiftMethod = cbo_MarkTextShiftMethod.SelectedIndex;
            m_smVisionInfo.g_intFinalReduction_Direction = cbo_FinalReduction_Direction.SelectedIndex;
            m_smVisionInfo.g_intFinalReduction_MarkDeg = cbo_FinalReduction_MarkDeg.SelectedIndex;
            if (cbo_RotationInterpolation_Mark.SelectedIndex == 0)
                m_smVisionInfo.g_intRotationInterpolation_Mark = 0;
            else
                m_smVisionInfo.g_intRotationInterpolation_Mark = 4;

            if (cbo_RotationInterpolation_PkgBright.SelectedIndex == 0)
                m_smVisionInfo.g_intRotationInterpolation_PkgBright = 0;
            else
                m_smVisionInfo.g_intRotationInterpolation_PkgBright = 4;

            if (cbo_RotationInterpolation_PkgDark.SelectedIndex == 0)
                m_smVisionInfo.g_intRotationInterpolation_PkgDark = 0;
            else
                m_smVisionInfo.g_intRotationInterpolation_PkgDark = 4;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ResetCharShiftXY(m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetX_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetX.Text)), m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetY_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetY.Text)));
                if (u == 0)
                {
                    if ((m_smVisionInfo.g_arrMarks[u].ref_intPin1ImageNo < cbo_Pin1ImageNo.Items.Count) && (m_smVisionInfo.g_arrMarks[u].ref_intPin1ImageNo != cbo_Pin1ImageNo.SelectedIndex))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Pin 1 Image View No", cbo_Pin1ImageNo.Items[m_smVisionInfo.g_arrMarks[u].ref_intPin1ImageNo].ToString(), cbo_Pin1ImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrMarks[u].ref_blnWantSampleAreaScore != chk_WantSampleAreaScore.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantSampleAreaScore.Text, (m_smVisionInfo.g_arrMarks[u].ref_blnWantSampleAreaScore).ToString(), chk_WantSampleAreaScore.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrMarks[u].ref_blnWantDontCareIgnoredMarkWholeArea != chk_WantDontCareIgnoredMarkWholeArea.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_WantDontCareIgnoredMarkWholeArea.Text, (m_smVisionInfo.g_arrMarks[u].ref_blnWantDontCareIgnoredMarkWholeArea).ToString(), chk_WantDontCareIgnoredMarkWholeArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrMarks[u].GetNoMarkMaximumBlobArea() != (float)Convert.ToDouble(txt_NoMarkMaximumBlobArea.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "No Mark Max Blob Area", m_smVisionInfo.g_arrMarks[u].GetNoMarkMaximumBlobArea().ToString(), txt_NoMarkMaximumBlobArea.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrMarks[u].ref_intMissingMarkInspectionMethod != cbo_MissingMarkInspectionMethod.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + "Missing Mark Inspection Method", cbo_MissingMarkInspectionMethod.Items[m_smVisionInfo.g_arrMarks[u].ref_intMissingMarkInspectionMethod].ToString(), cbo_MissingMarkInspectionMethod.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrMarks[u].ref_blnUseDefaultSettingMarkAfterClearTemplate != chk_UseDefaultMarkScoreAfterClearTemplate.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>" + chk_UseDefaultMarkScoreAfterClearTemplate.Text, (m_smVisionInfo.g_arrMarks[u].ref_blnUseDefaultSettingMarkAfterClearTemplate).ToString(), chk_UseDefaultMarkScoreAfterClearTemplate.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrMarks[u].ref_intDefaultCharSetting != Convert.ToInt32(txt_DefaultMarkScore.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Default Mark Score", (m_smVisionInfo.g_arrMarks[u].ref_intDefaultCharSetting).ToString(), txt_DefaultMarkScore.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrMarks[u].ref_intPin1PositionControl != cbo_Pin1PositionControl.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Pin1 Position Control", cbo_Pin1PositionControl.Items[m_smVisionInfo.g_arrMarks[u].ref_intPin1PositionControl].ToString(), cbo_Pin1PositionControl.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }
                }
                m_smVisionInfo.g_arrMarks[u].ref_intPin1PositionControl = cbo_Pin1PositionControl.SelectedIndex;
                m_smVisionInfo.g_arrMarks[u].ref_intPin1ImageNo = cbo_Pin1ImageNo.SelectedIndex;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantCheckMarkAverageGrayValue = chk_WantCheckMarkAverageGrayValue.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantUseUnitPatternAsMarkPattern = chk_WantUseUnitPatternAsMarkPattern.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantUseExcessMissingMarkAffectScore = chk_WantUseExcessMissingMarkAffectScore.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantCheckTotalExcessMark = chk_WantCheckTotalExcessMark.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_fCharROIOffsetX = m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetX_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetX.Text));
                m_smVisionInfo.g_arrMarks[u].ref_fCharROIOffsetY = m_smVisionInfo.g_arrMarks[0].GetCharROIOffsetY_InPixel((float)Convert.ToDouble(txt_MarkCharROIOffsetY.Text));
                m_smVisionInfo.g_arrMarks[u].ref_intDefaultCharSetting = Convert.ToInt32(txt_DefaultMarkScore.Text);
                m_smVisionInfo.g_arrMarks[u].ref_blnUseDefaultSettingMarkAfterClearTemplate = chk_UseDefaultMarkScoreAfterClearTemplate.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantUseMarkTypeInspectionSetting = chk_WantUseMarkTypeInspectionSetting.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantUseGrayValue = (m_smVisionInfo.g_intMarkDefectInspectionMethod == 1);
                m_smVisionInfo.g_arrMarks[u].ref_intMarkTextShiftMethod = m_smVisionInfo.g_intMarkTextShiftMethod;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantSampleAreaScore = chk_WantSampleAreaScore.Checked;
                m_smVisionInfo.g_arrMarks[u].SetNoMarkMaximumBlobArea((float)Convert.ToDouble(txt_NoMarkMaximumBlobArea.Text));
                m_smVisionInfo.g_arrMarks[u].ref_blnWantDontCareArea = chk_WantDontCareArea_Mark.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_intMarkScoreOffset = Convert.ToInt32(txt_MarkScoreOffset.Text);
                m_smVisionInfo.g_arrMarks[u].ref_intMarkOriPositionScore = Convert.ToInt32(txt_MarkOriPositionScore.Text);
                m_smVisionInfo.g_arrMarks[u].ref_blnSeparateExtraMarkThreshold = chk_SeparateExtraMarkThreshold.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantExcessMarkThresholdFollowExtraMarkThreshold = chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantDontCareIgnoredMarkWholeArea = chk_WantDontCareIgnoredMarkWholeArea.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_blnWantCheckBarPin1 = chk_WantCheckBarPin1.Checked;
                m_smVisionInfo.g_arrMarks[u].ref_intExtraExcessMarkInspectionAreaCutMode = cbo_ExtraExcessMarkInspectionAreaCutMode.SelectedIndex;
                m_smVisionInfo.g_arrMarks[u].ref_intCompensateMarkDiffSizeMode = cbo_CompensateMarkDiffSizeMode.SelectedIndex;
                m_smVisionInfo.g_arrMarks[u].ref_intMarkScoreMode = cbo_MarkScoreMode.SelectedIndex;
                m_smVisionInfo.g_arrMarks[u].ref_intMissingMarkInspectionMethod = cbo_MissingMarkInspectionMethod.SelectedIndex;

                if (m_smVisionInfo.g_arrOrients != null && u < m_smVisionInfo.g_arrOrients.Count)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrOrients[u].Count; j++)
                        m_smVisionInfo.g_arrOrients[u][j].ref_intCheckMarkAngleMinMaxTolerance = Convert.ToInt32(txt_CheckMarkAngleMinMaxTolerance.Text);
                }
            }
            
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\";

            //2020 05 08 ZJYEOH : Overwrite template 1 settings to other templates
            if (chk_Set1ToAll.Checked && m_smVisionInfo.g_intTotalTemplates > 1)
            {
                // Mark
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath, false);//2021-08-20 ZJYEOH : Need save first so that changes made in advance setting will be save
                    m_smVisionInfo.g_arrMarks[u].SetTemplate1SetingToOtherTemplate(strFolderPath, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                    m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath, false);
                    m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                }

                //Pin 1
                if (m_smVisionInfo.g_arrPin1 != null)
                {
                    string strPin1Path = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                      m_smVisionInfo.g_strVisionFolderName + "\\";

                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                    {
                        m_smVisionInfo.g_arrPin1[u].SetTemplate1SettingtoOtherTemplate(strPin1Path + "Orient\\Template\\");
                        m_smVisionInfo.g_arrPin1[u].SavePin1Setting(strPin1Path + "Orient\\Template\\");
                        m_smVisionInfo.g_arrPin1[u].LoadTemplate(strPin1Path + "Orient\\Template\\");
                    }
                }

                //Orient 
                if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrOrients[u].Count; j++)
                        {
                            XmlParser objFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Template\\Template.xml");
                            objFile.WriteSectionElement("Template" + j);
                            objFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[u][0].ref_fMinScore);
                            objFile.WriteEndElement();

                            m_smVisionInfo.g_arrOrients[u][j].ref_fMinScore = objFile.GetValueAsFloat("MinScore", 0.7f);

                        }
                    }
                }
            }

            // 2020 03 01 - CCENG: Reload Mark to update template image due to advance setting may affect the template image.
            //string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\";
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath, false);//2021-08-20 ZJYEOH : Need save first so that changes made in advance setting will be save
                m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
            }

            // 2019 07 08 - CCENG: Since skip mark not going to set here, so this checking have to be disabled also.
            //if (m_smVisionInfo.g_blnWantSkipMark != chk_SkipMarkInspection.Checked)
            //{
            //    m_smVisionInfo.g_blnWantSkipMark = chk_SkipMarkInspection.Checked;
            //    string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            //    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DeleteAllPreviousTemplate(strFolderPath + "Mark\\");

            //    //----------------------Delete All Previous Orient Template----------------------------
            //    // Delete OCV objects
            //    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //    {
            //        m_smVisionInfo.g_arrOrients[u].Clear();
            //    }
            //    // Reset variables
            //    m_smVisionInfo.g_intTotalGroup = 0;
            //    m_smVisionInfo.g_intTotalTemplates = 0;
            //    m_smVisionInfo.g_intTemplateMask = 0;
            //    m_smVisionInfo.g_intTemplatePriority = 0;
            //    // Delete database
            //    try
            //    {
            //        if (Directory.Exists(strFolderPath + "Template"))
            //            Directory.Delete(strFolderPath + "Template", true);
            //    }
            //    catch
            //    {
            //    }
            //    Directory.CreateDirectory(strFolderPath + "Template");

            //    //-----------------Reset General Setting-----------------------
            //    STDeviceEdit.CopySettingFile(strFolderPath, "General.xml");
            //    XmlParser objFile = new XmlParser(strFolderPath + "General.xml");
            //    objFile.WriteSectionElement("TemplateCounting");
            //    objFile.WriteElement1Value("TotalUnits", m_smVisionInfo.g_intUnitsOnImage);
            //    m_smVisionInfo.g_intTotalGroup = 0;
            //    objFile.WriteElement1Value("TotalGroups", 0);
            //    m_smVisionInfo.g_intTotalTemplates = 0;
            //    objFile.WriteElement1Value("TotalTemplates", 0);
            //    m_smVisionInfo.g_intTemplateMask = 0;
            //    objFile.WriteElement1Value("TemplateMask", 0);
            //    m_smVisionInfo.g_intTemplatePriority = 0;
            //    objFile.WriteElement1Value("TemplatePriority", 0);
            //    objFile.WriteEndElement();
            //    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strFolderPath, "General.xml");
            //}

            //m_smVisionInfo.g_blnUpdateMarkAdvanceSetting = true;
        }

        private void SavePackageAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Package) && !((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && ((m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || m_smVisionInfo.g_strVisionName.Contains("BottomOrient")))))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Package";

            if ((m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || m_smVisionInfo.g_strVisionName.Contains("BottomOrient")) && (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_smVisionInfo.g_arrPackage[0].SavePackage(strPath + "\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                return;
            }

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath + "\\Settings.xml");
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantShowGRR", chk_WantPackageGRR.Checked);
            objFileHandle.WriteElement1Value("WantUseSideLightGauge", chk_WantUseSideLightGauge.Checked);
            objFileHandle.WriteElement1Value("WantCheckVoidOnMark", chk_CheckVoidOnMarkArea.Checked);
            objFileHandle.WriteElement1Value("WantUseDetailThresholdPackage", chk_WantUseDetailThreshold_Package.Checked);
            objFileHandle.WriteElement1Value("WantDontCareAreaPackage", chk_WantDontCareArea_Package.Checked);
            objFileHandle.WriteElement1Value("WantCheckPackageAngle", chk_WantCheckPackageAngle_Package.Checked);
            objFileHandle.WriteElement1Value("SquareUnit", chk_SquareUnit.Checked);
            objFileHandle.WriteElement1Value("PackageDefectInspectionMethod", cbo_PackageDefectInspectionMethod.SelectedIndex);
            objFileHandle.WriteElement1Value("WantCheckPackageColor", chk_WantCheckPackageColor.Checked);
            objFileHandle.WriteElement1Value("WantCheckVoidOnMark_SideLight0", chk_CheckVoidOnMarkArea_Dark2.Checked);
            objFileHandle.WriteElement1Value("WantCheckVoidOnMark_SideLight1", chk_CheckVoidOnMarkArea_Dark3.Checked);
            objFileHandle.WriteElement1Value("WantCheckVoidOnMark_SideLight2", chk_CheckVoidOnMarkArea_Dark4.Checked);


            if (m_smVisionInfo.g_blnCheckPackageColor != chk_WantCheckPackageColor.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantCheckPackageColor.Text, (!chk_WantCheckPackageColor.Checked).ToString(), chk_WantCheckPackageColor.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantUseSideLightGauge != chk_WantUseSideLightGauge.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantUseSideLightGauge.Text, (!chk_WantUseSideLightGauge.Checked).ToString(), chk_WantUseSideLightGauge.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckVoidOnMarkArea != chk_CheckVoidOnMarkArea.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_CheckVoidOnMarkArea.Text, (!chk_CheckVoidOnMarkArea.Checked).ToString(), chk_CheckVoidOnMarkArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckVoidOnMarkArea != chk_CheckVoidOnMarkArea_Dark2.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_CheckVoidOnMarkArea_Dark2.Text, (!chk_CheckVoidOnMarkArea_Dark2.Checked).ToString(), chk_CheckVoidOnMarkArea_Dark2.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckVoidOnMarkArea != chk_CheckVoidOnMarkArea_Dark3.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_CheckVoidOnMarkArea_Dark3.Text, (!chk_CheckVoidOnMarkArea_Dark3.Checked).ToString(), chk_CheckVoidOnMarkArea_Dark3.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckVoidOnMarkArea != chk_CheckVoidOnMarkArea_Dark4.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_CheckVoidOnMarkArea_Dark4.Text, (!chk_CheckVoidOnMarkArea_Dark4.Checked).ToString(), chk_CheckVoidOnMarkArea_Dark4.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantDontCareArea_Package != chk_WantDontCareArea_Package.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantDontCareArea_Package.Text, (!chk_WantDontCareArea_Package.Checked).ToString(), chk_WantDontCareArea_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckPackageAngle != chk_WantCheckPackageAngle_Package.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantCheckPackageAngle_Package.Text, (!chk_WantCheckPackageAngle_Package.Checked).ToString(), chk_WantCheckPackageAngle_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnSquareUnit != chk_SquareUnit.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_SquareUnit.Text, (!chk_SquareUnit.Checked).ToString(), chk_SquareUnit.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intPackageDefectInspectionMethod != cbo_PackageDefectInspectionMethod.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Package Defect Inspection Method", cbo_PackageDefectInspectionMethod.Items[m_smVisionInfo.g_intPackageDefectInspectionMethod].ToString(), cbo_PackageDefectInspectionMethod.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

          
            m_smVisionInfo.g_blnWantUseSideLightGauge = chk_WantUseSideLightGauge.Checked;
            m_smVisionInfo.g_blnWantCheckVoidOnMarkArea = chk_CheckVoidOnMarkArea.Checked;
            m_smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[0] = chk_CheckVoidOnMarkArea_Dark2.Checked;
            m_smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[1] = chk_CheckVoidOnMarkArea_Dark3.Checked;
            m_smVisionInfo.g_blnWantCheckVoidOnMarkArea_SideLight[2] = chk_CheckVoidOnMarkArea_Dark4.Checked;

            if (m_smVisionInfo.g_arrPackage.Count > 0)
                m_smVisionInfo.g_arrPackage[0].ref_blnUseDetailDefectCriteria = m_smVisionInfo.g_blnWantUseDetailThreshold_Package = chk_WantUseDetailThreshold_Package.Checked;
            m_smVisionInfo.g_blnWantDontCareArea_Package = chk_WantDontCareArea_Package.Checked;
            m_smVisionInfo.g_blnWantCheckPackageAngle = chk_WantCheckPackageAngle_Package.Checked;
            m_smVisionInfo.g_blnSquareUnit = chk_SquareUnit.Checked;
            m_smVisionInfo.g_intPackageDefectInspectionMethod = cbo_PackageDefectInspectionMethod.SelectedIndex;
            m_smVisionInfo.g_blnCheckPackageColor = chk_WantCheckPackageColor.Checked;

            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_blnWantShowGRR = chk_WantPackageGRR.Checked;

            //STDeviceEdit.CopySettingFile(strPath, "");
            for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
            {
                if (i == 0)
                {
                    if (m_smVisionInfo.g_arrPackage[i].ref_intBrightDefectLinkTolerance != Convert.ToInt32(txt_BrightDefectLinkTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Bright Defect Link Tolerance", (m_smVisionInfo.g_arrPackage[i].ref_intBrightDefectLinkTolerance).ToString(), txt_BrightDefectLinkTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_intDarkDefectLinkTolerance != Convert.ToInt32(txt_DarkDefectLinkTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Dark Defect Link Tolerance", (m_smVisionInfo.g_arrPackage[i].ref_intDarkDefectLinkTolerance).ToString(), txt_DarkDefectLinkTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_intDark2DefectLinkTolerance != Convert.ToInt32(txt_Dark2DefectLinkTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Dark 2 Defect Link Tolerance", (m_smVisionInfo.g_arrPackage[i].ref_intDark2DefectLinkTolerance).ToString(), txt_Dark2DefectLinkTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_intDark3DefectLinkTolerance != Convert.ToInt32(txt_Dark3DefectLinkTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Dark 3 Defect Link Tolerance", (m_smVisionInfo.g_arrPackage[i].ref_intDark3DefectLinkTolerance).ToString(), txt_Dark3DefectLinkTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_intDark4DefectLinkTolerance != Convert.ToInt32(txt_Dark4DefectLinkTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Dark 4 Defect Link Tolerance", (m_smVisionInfo.g_arrPackage[i].ref_intDark4DefectLinkTolerance).ToString(), txt_Dark4DefectLinkTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_intCrackDefectLinkTolerance != Convert.ToInt32(txt_CrackDefectLinkTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Crack Defect Link Tolerance", (m_smVisionInfo.g_arrPackage[i].ref_intCrackDefectLinkTolerance).ToString(), txt_CrackDefectLinkTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_intMoldFlashDefectLinkTolerance != Convert.ToInt32(txt_MoldFlashDefectLinkTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Mold Flash Defect Link Tolerance", (m_smVisionInfo.g_arrPackage[i].ref_intMoldFlashDefectLinkTolerance).ToString(), txt_MoldFlashDefectLinkTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkBrightDefect != chk_WantLinkBrightDefect.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantLinkBrightDefect.Text, (!chk_WantLinkBrightDefect.Checked).ToString(), chk_WantLinkBrightDefect.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkDarkDefect != chk_WantLinkDarkDefect.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantLinkDarkDefect.Text, (!chk_WantLinkDarkDefect.Checked).ToString(), chk_WantLinkDarkDefect.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkDark2Defect != chk_WantLinkDark2Defect.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantLinkDark2Defect.Text, (!chk_WantLinkDark2Defect.Checked).ToString(), chk_WantLinkDark2Defect.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkDark3Defect != chk_WantLinkDark3Defect.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantLinkDark3Defect.Text, (!chk_WantLinkDark3Defect.Checked).ToString(), chk_WantLinkDark3Defect.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkDark4Defect != chk_WantLinkDark4Defect.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantLinkDark4Defect.Text, (!chk_WantLinkDark4Defect.Checked).ToString(), chk_WantLinkDark4Defect.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkCrackDefect != chk_WantLinkCrackDefect.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantLinkCrackDefect.Text, (!chk_WantLinkCrackDefect.Checked).ToString(), chk_WantLinkCrackDefect.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkMoldFlashDefect != chk_WantLinkMoldFlashDefect.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_WantLinkMoldFlashDefect.Text, (!chk_WantLinkMoldFlashDefect.Checked).ToString(), chk_WantLinkMoldFlashDefect.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnSeperateBrightDarkROITolerance != chk_SeperateBrightDarkROITolerance_Package.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_SeperateBrightDarkROITolerance_Package.Text, (!chk_SeperateBrightDarkROITolerance_Package.Checked).ToString(), chk_SeperateBrightDarkROITolerance_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnSeperateDarkField2DefectSetting != chk_SeperateDarkField2DefectSetting_Package.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_SeperateDarkField2DefectSetting_Package.Text, (!chk_SeperateDarkField2DefectSetting_Package.Checked).ToString(), chk_SeperateDarkField2DefectSetting_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnSeperateDarkField3DefectSetting != chk_SeperateDarkField3DefectSetting_Package.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_SeperateDarkField3DefectSetting_Package.Text, (!chk_SeperateDarkField3DefectSetting_Package.Checked).ToString(), chk_SeperateDarkField3DefectSetting_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnSeperateDarkField4DefectSetting != chk_SeperateDarkField4DefectSetting_Package.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_SeperateDarkField4DefectSetting_Package.Text, (!chk_SeperateDarkField4DefectSetting_Package.Checked).ToString(), chk_SeperateDarkField4DefectSetting_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnSeperateCrackDefectSetting != chk_SeperateCrackDefectSetting_Package.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_SeperateCrackDefectSetting_Package.Text, (!chk_SeperateCrackDefectSetting_Package.Checked).ToString(), chk_SeperateCrackDefectSetting_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }
                    
                    if (m_smVisionInfo.g_arrPackage[i].ref_blnSeperateMoldFlashDefectSetting != chk_SeperateMoldFlashDefectSetting_Package.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_SeperateMoldFlashDefectSetting_Package.Text, (!chk_SeperateMoldFlashDefectSetting_Package.Checked).ToString(), chk_SeperateMoldFlashDefectSetting_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }
                    
                    if (m_smVisionInfo.g_arrPackage[i].ref_intChippedOffDefectInspectionMethod != cbo_ChippedOffDefectInspectionMethod_Package.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Package Chipped Off Inspection Method", cbo_ChippedOffDefectInspectionMethod_Package.Items[m_smVisionInfo.g_arrPackage[i].ref_intChippedOffDefectInspectionMethod].ToString(), cbo_ChippedOffDefectInspectionMethod_Package.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnSeperateChippedOffDefectSetting != chk_SeperateChippedOffDefectSetting_Package.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + chk_SeperateChippedOffDefectSetting_Package.Text, (!chk_SeperateChippedOffDefectSetting_Package.Checked).ToString(), chk_SeperateChippedOffDefectSetting_Package.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(0) != cbo_PackageSizeImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Package Size Image View No", cbo_PackageSizeImageNo.Items[m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(0)].ToString(), cbo_PackageSizeImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(2) != cbo_SideLightPackageBrightDefectImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Side Light Package Bright Defect Image View No", cbo_SideLightPackageBrightDefectImageNo.Items[m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(2)].ToString(), cbo_SideLightPackageBrightDefectImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(4) != cbo_SideLightPackageDarkDefectImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Side Light Package Dark Defect Image View No", cbo_SideLightPackageDarkDefectImageNo.Items[m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(4)].ToString(), cbo_SideLightPackageDarkDefectImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(6) != cbo_SideLightPackageDarkDefect2ImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Side Light Package Dark Defect Image View No", cbo_SideLightPackageDarkDefect2ImageNo.Items[m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(4)].ToString(), cbo_SideLightPackageDarkDefect2ImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(7) != cbo_SideLightPackageDarkDefect3ImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Side Light Package Dark Defect Image View No", cbo_SideLightPackageDarkDefect3ImageNo.Items[m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(4)].ToString(), cbo_SideLightPackageDarkDefect3ImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(3) != cbo_DirectLightPackageDefectImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Direct Light Package Defect Image View No", cbo_DirectLightPackageDefectImageNo.Items[m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(3)].ToString(), cbo_DirectLightPackageDefectImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(5) != cbo_MoldFlashDefectImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Mold Flash Defect Image View No", cbo_MoldFlashDefectImageNo.Items[m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(5)].ToString(), cbo_MoldFlashDefectImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_intColorDefectLinkMethod != cbo_ColorDefectLinkMethod_Package.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Color Defect Link Method", cbo_ColorDefectLinkMethod_Package.Items[m_smVisionInfo.g_arrPackage[i].ref_intColorDefectLinkMethod].ToString(), cbo_ColorDefectLinkMethod_Package.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPackage[i].ref_intColorDefectLinkTolerance != Convert.ToInt32(txt_ColorDefectLinkTolerance_Package.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Advance Setting>" + "Color Defect Link Tolerance", m_smVisionInfo.g_arrPackage[i].ref_intColorDefectLinkTolerance.ToString(), txt_ColorDefectLinkTolerance_Package.Text, m_smProductionInfo.g_strLotID);
                    }
                    
                }
                m_smVisionInfo.g_arrPackage[i].ref_blnSquareUnit = chk_SquareUnit.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_intBrightDefectLinkTolerance = Convert.ToInt32(txt_BrightDefectLinkTolerance.Text);
                m_smVisionInfo.g_arrPackage[i].ref_intDarkDefectLinkTolerance = Convert.ToInt32(txt_DarkDefectLinkTolerance.Text);
                m_smVisionInfo.g_arrPackage[i].ref_intDark2DefectLinkTolerance = Convert.ToInt32(txt_Dark2DefectLinkTolerance.Text);
                m_smVisionInfo.g_arrPackage[i].ref_intDark3DefectLinkTolerance = Convert.ToInt32(txt_Dark3DefectLinkTolerance.Text);
                m_smVisionInfo.g_arrPackage[i].ref_intDark4DefectLinkTolerance = Convert.ToInt32(txt_Dark4DefectLinkTolerance.Text);
                m_smVisionInfo.g_arrPackage[i].ref_intCrackDefectLinkTolerance = Convert.ToInt32(txt_CrackDefectLinkTolerance.Text);
                m_smVisionInfo.g_arrPackage[i].ref_intMoldFlashDefectLinkTolerance = Convert.ToInt32(txt_MoldFlashDefectLinkTolerance.Text);
                m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkBrightDefect = chk_WantLinkBrightDefect.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkDarkDefect = chk_WantLinkDarkDefect.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkDark2Defect = chk_WantLinkDark2Defect.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkDark3Defect = chk_WantLinkDark3Defect.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkDark4Defect = chk_WantLinkDark4Defect.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkCrackDefect = chk_WantLinkCrackDefect.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnWantLinkMoldFlashDefect = chk_WantLinkMoldFlashDefect.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnSeperateBrightDarkROITolerance = chk_SeperateBrightDarkROITolerance_Package.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnSeperateDarkField2DefectSetting = chk_SeperateDarkField2DefectSetting_Package.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnSeperateDarkField3DefectSetting = chk_SeperateDarkField3DefectSetting_Package.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnSeperateDarkField4DefectSetting = chk_SeperateDarkField4DefectSetting_Package.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnSeperateCrackDefectSetting = chk_SeperateCrackDefectSetting_Package.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnSeperateVoidDefectSetting = chk_SeperateVoidDefectSetting_Package.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnSeperateMoldFlashDefectSetting = chk_SeperateMoldFlashDefectSetting_Package.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_blnSeperateChippedOffDefectSetting = chk_SeperateChippedOffDefectSetting_Package.Checked;
                m_smVisionInfo.g_arrPackage[i].ref_intChippedOffDefectInspectionMethod = cbo_ChippedOffDefectInspectionMethod_Package.SelectedIndex;
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(0, cbo_PackageSizeImageNo.SelectedIndex);
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(1, 0);
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(2, cbo_SideLightPackageBrightDefectImageNo.SelectedIndex);
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(4, cbo_SideLightPackageDarkDefectImageNo.SelectedIndex);
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(3, cbo_DirectLightPackageDefectImageNo.SelectedIndex);
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(5, cbo_MoldFlashDefectImageNo.SelectedIndex);
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(6, cbo_SideLightPackageDarkDefect2ImageNo.SelectedIndex);
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(7, cbo_SideLightPackageDarkDefect3ImageNo.SelectedIndex);

                m_smVisionInfo.g_arrPackage[i].ref_intColorDefectLinkMethod = cbo_ColorDefectLinkMethod_Package.SelectedIndex;
                m_smVisionInfo.g_arrPackage[i].ref_intColorDefectLinkTolerance = Convert.ToInt32(txt_ColorDefectLinkTolerance_Package.Text);

                if (i == 0)
                    m_smVisionInfo.g_arrPackage[i].SavePackage(strPath + "\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                else
                    m_smVisionInfo.g_arrPackage[i].SavePackage(strPath + "\\Settings2.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);

            }
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);
            
            // Save Advance General Setting
            strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\General.xml";

            //objFileHandle = new XmlParser(strPath, false);
            //objFileHandle.WriteSectionElement("PackageSetting", false);
            //objFileHandle.WriteElement1Value("CheckPackage", chk_CheckPackage.Checked);
            //objFileHandle.WriteEndElement();

            m_smVisionInfo.g_blnCheckPackage = chk_CheckPackage.Checked;

        }

        private void SavePadAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Pad) && !tabControl_SettingForm.Controls.Contains(tab_PadPackage) && !tabControl_SettingForm.Controls.Contains(tab_PadPackage2))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

            bool blnCheck4SideSettingChanged = false;
            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantCheckPad", chk_WantCheckPad.Checked);
            objFileHandle.WriteElement1Value("WantCheckPadColor", chk_WantCheckPadColor.Checked);
            objFileHandle.WriteElement1Value("WantCheck4Sides", chk_WantCheck4Sides.Checked);
            objFileHandle.WriteElement1Value("WantShowGRR", chk_WantGRR.Checked);
            objFileHandle.WriteElement1Value("WantCheckCPK", chk_WantCPK.Checked);
            objFileHandle.WriteElement1Value("CheckAllPadCPK", chk_CheckAllPadCPK.Checked);
            objFileHandle.WriteElement1Value("PadCPKCount", Convert.ToInt32(txt_PadCPKCount.Text));
            objFileHandle.WriteElement1Value("WantPin1", chk_WantPadPin1.Checked);
            objFileHandle.WriteElement1Value("WantCheckPH", chk_WantCheckPH.Checked);
            objFileHandle.WriteElement1Value("WantDontCareAreaPad", chk_WantDontCareArea_Pad.Checked);
            objFileHandle.WriteElement1Value("WantEdgeLimitPad", chk_WantEdgeLimit_Pad.Checked);
            objFileHandle.WriteElement1Value("WantEdgeDistancePad", chk_WantEdgeDistance_Pad.Checked);
            objFileHandle.WriteElement1Value("WantSpanPad", chk_WantSpan_Pad.Checked);
            objFileHandle.WriteElement1Value("WantStandOffPad", chk_WantStandOff_Pad.Checked);
            objFileHandle.WriteElement1Value("SavePadTemplateImageMethod", cbo_SavePadTemplateImageMethod.SelectedIndex);
            objFileHandle.WriteElement1Value("PadOffsetReferencePoint", cbo_PadOffsetReferencePoint.SelectedIndex);
            objFileHandle.WriteElement1Value("PadSubtractMethod_Center", cbo_PadSubtractMethod_Center.SelectedIndex);
            objFileHandle.WriteElement1Value("PadSubtractMethod_Side", cbo_PadSubtractMethod_Side.SelectedIndex);



            if (m_smVisionInfo.g_blnCheckPadColor != chk_WantCheckPadColor.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantCheckPadColor.Text, (!chk_WantCheckPadColor.Checked).ToString(), chk_WantCheckPadColor.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (radioBtn_2Directions_Pad.Checked)
            {
                if (objFileHandle.GetValueAsInt("OrientDirection", 4) == 4)
                {
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">OrientPad", "Advance Setting>" + "OrientDirection", "4", "2", m_smProductionInfo.g_strLotID);
                }
            }
            else
            {
                if (objFileHandle.GetValueAsInt("OrientDirection", 4) == 2)
                {
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">OrientPad", "Advance Setting>" + "OrientDirection", "2", "4", m_smProductionInfo.g_strLotID);
                }
            }


            if (radioBtn_2Directions_Pad.Checked)
                objFileHandle.WriteElement1Value("OrientDirection", 2);
            else
                objFileHandle.WriteElement1Value("OrientDirection", 4);

            //objFileHandle.WriteElement1Value("WantTestRunTightSetting", chk_WantTestRunTightSetting.Checked);
            //objFileHandle.WriteElement1Value("WantConsiderPadImage2", chk_WantConsiderPadImage2.Checked);
            //objFileHandle.WriteElement1Value("WantPRUnitLocationBeforeGauge", chk_WantPRUnitLocationBeforeGauge.Checked);
            //objFileHandle.WriteElement1Value("WantUseGaugeMeasureDimension", chk_WantUseGaugeMeasureDimension.Checked);
            //objFileHandle.WriteElement1Value("WantUseClosestSizeDefineTolerance", chk_DefinePadToleranceUsingClosestMethod.Checked);
            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);

            if (m_smVisionInfo.g_blnCheckPad != chk_WhiteOnBlack.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WhiteOnBlack.Text, (!chk_WhiteOnBlack.Checked).ToString(), chk_WhiteOnBlack.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnCheck4Sides != chk_WantCheck4Sides.Checked)
            {
                blnCheck4SideSettingChanged = true;
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantCheck4Sides.Text, (!chk_WantCheck4Sides.Checked).ToString(), chk_WantCheck4Sides.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantShowGRR != chk_WantGRR.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantGRR.Text, (!chk_WantGRR.Checked).ToString(), chk_WantGRR.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnCPKON != chk_WantCPK.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantCPK.Text, (!chk_WantCPK.Checked).ToString(), chk_WantCPK.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnRecordAllPadCPKEvenIfFail != chk_CheckAllPadCPK.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_CheckAllPadCPK.Text, (!chk_CheckAllPadCPK.Checked).ToString(), chk_CheckAllPadCPK.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantPin1 != chk_WantPadPin1.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantPadPin1.Text, (!chk_WantPadPin1.Checked).ToString(), chk_WantPadPin1.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckPH != chk_WantCheckPH.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantCheckPH.Text, (!chk_WantCheckPH.Checked).ToString(), chk_WantCheckPH.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intCPKTestCount != Convert.ToInt32(txt_PadCPKCount.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "CPK Record Count", (m_smVisionInfo.g_intCPKTestCount).ToString(), txt_PadCPKCount.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantDontCareArea_Pad != chk_WantDontCareArea_Pad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantDontCareArea_Pad.Text, (!chk_WantDontCareArea_Pad.Checked).ToString(), chk_WantDontCareArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            m_smVisionInfo.g_blnCheckPad = chk_WantCheckPad.Checked;
            m_smVisionInfo.g_blnCheckPadColor = chk_WantCheckPadColor.Checked;
            m_smVisionInfo.g_blnCheck4Sides = chk_WantCheck4Sides.Checked;
            m_smVisionInfo.g_blnWantShowGRR = chk_WantGRR.Checked;
            m_smVisionInfo.g_blnCPKON = chk_WantCPK.Checked;
            m_smVisionInfo.g_blnRecordAllPadCPKEvenIfFail = chk_CheckAllPadCPK.Checked;
            m_smVisionInfo.g_blnWantPin1 = chk_WantPadPin1.Checked;
            m_smVisionInfo.g_blnWantCheckPH = chk_WantCheckPH.Checked;
            m_smVisionInfo.g_intCPKTestCount = Convert.ToInt32(txt_PadCPKCount.Text);
            m_smVisionInfo.g_blnWantDontCareArea_Pad = chk_WantDontCareArea_Pad.Checked;

            if (m_smVisionInfo.g_blnWantCheckPH) // Load Position Setting if want check PH
            {
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                    LoadCalibrationSetting(strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
                else
                    LoadCalibrationSetting(strFolderPath + "Calibration.xml");
                ROI.LoadFile(strFolderPath + "Positioning\\PHROI.xml", m_smVisionInfo.g_arrPHROIs);

                LoadPositioningSettings(strFolderPath + "Positioning\\");
            }
            else
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == 0)
                {
                    if (radioBtn_2Directions_Pad.Checked)
                    {
                        if (m_smVisionInfo.g_arrPad[i].ref_intOrientDirections != 2)
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">OrientPad", "Advance Setting>Orient Direction", "4", "2", m_smProductionInfo.g_strLotID);
                    }
                    else
                    {
                        if (m_smVisionInfo.g_arrPad[i].ref_intOrientDirections != 4)
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">OrientPad", "Advance Setting>Orient Direction", "2", "4", m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWhiteOnBlack != chk_PadWhiteOnBlack.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_PadWhiteOnBlack.Text, (!chk_PadWhiteOnBlack.Checked).ToString(), chk_PadWhiteOnBlack.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantTightSetting != chk_WantTestRunTightSetting.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantTestRunTightSetting.Text, (!chk_WantTestRunTightSetting.Checked).ToString(), chk_WantTestRunTightSetting.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_fTightSettingTolerance != float.Parse(txt_TightSettingDimensionTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Tight Setting Dimension Tolerance", (m_smVisionInfo.g_arrPad[i].ref_fTightSettingTolerance).ToString(), txt_TightSettingDimensionTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intTightSettingThresholdTolerance != float.Parse(txt_TightSettingThresholdTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Tight Setting Threshold Tolerance", (m_smVisionInfo.g_arrPad[i].ref_intTightSettingThresholdTolerance).ToString(), txt_TightSettingThresholdTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantAutoGauge != chk_WantAutoGauge.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantAutoGauge.Text, (!chk_WantAutoGauge.Checked).ToString(), chk_WantAutoGauge.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage != chk_WantRotateSidePadImage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantRotateSidePadImage.Text, (!chk_WantRotateSidePadImage.Checked).ToString(), chk_WantRotateSidePadImage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantLinkDifferentGroupPitchGap != chk_WantLinkDifferentGroupPitchGap.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantLinkDifferentGroupPitchGap.Text, (!chk_WantLinkDifferentGroupPitchGap.Checked).ToString(), chk_WantLinkDifferentGroupPitchGap.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantShowUseGaugeCheckBox != chk_ShowUseGaugeCheckBoxInLearnForm.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_ShowUseGaugeCheckBoxInLearnForm.Text, (!chk_ShowUseGaugeCheckBoxInLearnForm.Checked).ToString(), chk_ShowUseGaugeCheckBoxInLearnForm.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantSeparateBrokenPadThresholdSetting != chk_WantSeparateBrokenPadThresholdSetting.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantSeparateBrokenPadThresholdSetting.Text, (!chk_WantSeparateBrokenPadThresholdSetting.Checked).ToString(), chk_WantSeparateBrokenPadThresholdSetting.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON != chk_WantViewCheckForeignMaterialOptionWhenPackageON.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantViewCheckForeignMaterialOptionWhenPackageON.Text, (!chk_WantViewCheckForeignMaterialOptionWhenPackageON.Checked).ToString(), chk_WantViewCheckForeignMaterialOptionWhenPackageON.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnMeasureCenterPkgSizeUsingSidePkg != chk_MeasureCenterPkgSizeUsingSidePkg.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_MeasureCenterPkgSizeUsingSidePkg.Text, (!chk_MeasureCenterPkgSizeUsingSidePkg.Checked).ToString(), chk_MeasureCenterPkgSizeUsingSidePkg.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantDontCareArea_Pad != chk_WantDontCareArea_Pad.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantDontCareArea_Pad.Text, (!chk_WantDontCareArea_Pad.Checked).ToString(), chk_WantDontCareArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantEdgeLimit_Pad != chk_WantEdgeLimit_Pad.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantEdgeLimit_Pad.Text, (!chk_WantEdgeLimit_Pad.Checked).ToString(), chk_WantEdgeLimit_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantEdgeDistance_Pad != chk_WantEdgeDistance_Pad.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantEdgeDistance_Pad.Text, (!chk_WantEdgeDistance_Pad.Checked).ToString(), chk_WantEdgeDistance_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantSpan_Pad != chk_WantSpan_Pad.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantSpan_Pad.Text, (!chk_WantSpan_Pad.Checked).ToString(), chk_WantSpan_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantStandOff_Pad != chk_WantStandOff_Pad.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantStandOff_Pad.Text, (!chk_WantStandOff_Pad.Checked).ToString(), chk_WantStandOff_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantPRUnitLocationBeforeGauge != chk_WantPRUnitLocationBeforeGauge.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantPRUnitLocationBeforeGauge.Text, (!chk_WantPRUnitLocationBeforeGauge.Checked).ToString(), chk_WantPRUnitLocationBeforeGauge.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension) // Point
                    {
                        if (cbo_PadMeasurementMethod.SelectedIndex == 0)
                        {
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Pad Measurement Method (Center)", "Point", "Max", m_smProductionInfo.g_strLotID);
                        }
                    }
                    else // Max
                    {
                        if (cbo_PadMeasurementMethod.SelectedIndex == 1)
                        {
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Pad Measurement Method (Center)", "Max", "Point", m_smProductionInfo.g_strLotID);
                        }
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantUseClosestSizeDefineTolerance != chk_DefinePadToleranceUsingClosestMethod.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_DefinePadToleranceUsingClosestMethod.Text, (!chk_DefinePadToleranceUsingClosestMethod.Checked).ToString(), chk_DefinePadToleranceUsingClosestMethod.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo != cbo_CenterROIBrokenPadImageViewNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Broken Pad Image View No (Center)", cbo_CenterROIBrokenPadImageViewNo.Items[m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo].ToString(), cbo_CenterROIBrokenPadImageViewNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intSavePadTemplateImageMethod != cbo_SavePadTemplateImageMethod.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Save Pad Template Image Method", cbo_SavePadTemplateImageMethod.Items[m_smVisionInfo.g_arrPad[i].ref_intSavePadTemplateImageMethod].ToString(), cbo_SavePadTemplateImageMethod.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadOffsetReferencePoint != cbo_PadOffsetReferencePoint.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Pad Offset Reference Point", cbo_PadOffsetReferencePoint.Items[m_smVisionInfo.g_arrPad[i].ref_intPadOffsetReferencePoint].ToString(), cbo_PadOffsetReferencePoint.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadSubtractMethod != cbo_PadSubtractMethod_Center.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Pad Subtract Method (Center)", cbo_PadSubtractMethod_Center.Items[m_smVisionInfo.g_arrPad[i].ref_intPadSubtractMethod].ToString(), cbo_PadSubtractMethod_Center.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intColorDefectLinkMethod != cbo_ColorDefectLinkMethod_Pad.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Color Defect Link Method", cbo_ColorDefectLinkMethod_Pad.Items[m_smVisionInfo.g_arrPad[i].ref_intColorDefectLinkMethod].ToString(), cbo_ColorDefectLinkMethod_Pad.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intColorDefectLinkTolerance != Convert.ToInt32(txt_ColorDefectLinkTolerance_Pad.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Color Defect Link Tolerance", m_smVisionInfo.g_arrPad[i].ref_intColorDefectLinkTolerance.ToString(), txt_ColorDefectLinkTolerance_Pad.Text, m_smProductionInfo.g_strLotID);
                    }
                    
                }
                if (i == 1)
                {

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantConsiderPadImage2 != chk_WantConsiderPadImage2.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + chk_WantConsiderPadImage2.Text, (!chk_WantConsiderPadImage2.Checked).ToString(), chk_WantConsiderPadImage2.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension) // Point
                    {
                        if (cbo_PadMeasurementMethod_Side.SelectedIndex == 0)
                        {
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Pad Measurement Method (Side)", "Point", "Max", m_smProductionInfo.g_strLotID);
                        }
                    }
                    else // Max
                    {
                        if (cbo_PadMeasurementMethod_Side.SelectedIndex == 1)
                        {
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Pad Measurement Method (Side)", "Max", "Point", m_smProductionInfo.g_strLotID);
                        }
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo != cbo_SideROIBrokenPadImageViewNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Broken Pad Image View No (Side)", cbo_SideROIBrokenPadImageViewNo.Items[m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo].ToString(), cbo_SideROIBrokenPadImageViewNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadSubtractMethod != cbo_PadSubtractMethod_Side.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad", "Advance Setting>" + "Pad Subtract Method (Side)", cbo_PadSubtractMethod_Center.Items[m_smVisionInfo.g_arrPad[i].ref_intPadSubtractMethod].ToString(), cbo_PadSubtractMethod_Side.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantIndividualSideThickness != chk_WantIndividualSideThickness.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog("Pad", "Advance Setting>" + chk_WantIndividualSideThickness.Text, (!chk_WantIndividualSideThickness.Checked).ToString(), chk_WantIndividualSideThickness.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }
                }

                if (radioBtn_2Directions_Pad.Checked)
                    m_smVisionInfo.g_arrPad[i].ref_intOrientDirections = 2;
                else
                    m_smVisionInfo.g_arrPad[i].ref_intOrientDirections = 4;

                if (m_smVisionInfo.g_objPadOrient != null)
                {
                    m_smVisionInfo.g_objPadOrient.ref_intDirections = m_smVisionInfo.g_arrPad[0].ref_intOrientDirections;
                }

                m_smVisionInfo.g_arrPad[i].ref_blnWhiteOnBlack = chk_PadWhiteOnBlack.Checked;
                m_smVisionInfo.g_arrPad[i].ref_fDefaultPixelTolerance = float.Parse(txt_DefaultPixelTolerance.Text);
                m_smVisionInfo.g_arrPad[i].ref_intPadROISizeToleranceADV = int.Parse(txt_PadROITolerance.Text); // 2019-05-06 ZJYEOH : Save the setting to another parameter to avoid different Size ROI during inspection
                //m_smVisionInfo.g_arrPad[i].ref_intPadSizeHalfWidthTolerance = int.Parse(txt_PadSizeHalfWidthTolerance.Text);
                m_smVisionInfo.g_arrPad[i].ref_blnWantTightSetting = chk_WantTestRunTightSetting.Checked;
                m_smVisionInfo.g_arrPad[i].ref_fTightSettingTolerance = float.Parse(txt_TightSettingDimensionTolerance.Text);
                m_smVisionInfo.g_arrPad[i].ref_intTightSettingThresholdTolerance = int.Parse(txt_TightSettingThresholdTolerance.Text);
                m_smVisionInfo.g_arrPad[i].ref_blnWantAutoGauge = chk_WantAutoGauge.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage = chk_WantRotateSidePadImage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantLinkDifferentGroupPitchGap = chk_WantLinkDifferentGroupPitchGap.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantShowUseGaugeCheckBox = chk_ShowUseGaugeCheckBoxInLearnForm.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantSeparateBrokenPadThresholdSetting = chk_WantSeparateBrokenPadThresholdSetting.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON = chk_WantViewCheckForeignMaterialOptionWhenPackageON.Checked;

                //m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod = cbo_SensitivityOnPad.SelectedIndex;
                //m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue = int.Parse(txt_SensitivityValue.Text);
                //m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod = m_arrSensitivityOnPadMethod[i];
                //m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue = m_arrSensitivityOnPadValue[i];
                m_smVisionInfo.g_arrPad[i].ref_blnMeasureCenterPkgSizeUsingSidePkg = chk_MeasureCenterPkgSizeUsingSidePkg.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantDontCareArea_Pad = chk_WantDontCareArea_Pad.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantEdgeLimit_Pad = chk_WantEdgeLimit_Pad.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantEdgeDistance_Pad = chk_WantEdgeDistance_Pad.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantSpan_Pad = chk_WantSpan_Pad.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantStandOff_Pad = chk_WantStandOff_Pad.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantIndividualSideThickness = chk_WantIndividualSideThickness.Checked;

                if (i > 0)
                    m_smVisionInfo.g_arrPad[i].ref_blnWantConsiderPadImage2 = chk_WantConsiderPadImage2.Checked;    // For side pad only.
                m_smVisionInfo.g_arrPad[i].ref_blnWantPRUnitLocationBeforeGauge = chk_WantPRUnitLocationBeforeGauge.Checked;
                if (i == 0)
                {
                    if (cbo_PadMeasurementMethod.SelectedIndex == 0)
                        m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension = false;
                    else
                        m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension = true;
                }
                else
                {
                    if (cbo_PadMeasurementMethod_Side.SelectedIndex == 0)
                        m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension = false;
                    else
                        m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension = true;
                }
                m_smVisionInfo.g_arrPad[i].ref_blnWantUseClosestSizeDefineTolerance = chk_DefinePadToleranceUsingClosestMethod.Checked;

                if (i == 0)
                    m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo = cbo_CenterROIBrokenPadImageViewNo.SelectedIndex;
                else
                    m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo = cbo_SideROIBrokenPadImageViewNo.SelectedIndex;

                if (i == 0)
                    m_smVisionInfo.g_arrPad[i].ref_intPadSubtractMethod = cbo_PadSubtractMethod_Center.SelectedIndex;
                else
                    m_smVisionInfo.g_arrPad[i].ref_intPadSubtractMethod = cbo_PadSubtractMethod_Side.SelectedIndex;

                m_smVisionInfo.g_arrPad[i].ref_intSavePadTemplateImageMethod = cbo_SavePadTemplateImageMethod.SelectedIndex;
                m_smVisionInfo.g_arrPad[i].ref_intPadOffsetReferencePoint = cbo_PadOffsetReferencePoint.SelectedIndex;

                m_smVisionInfo.g_arrPad[i].ref_intColorDefectLinkMethod = cbo_ColorDefectLinkMethod_Pad.SelectedIndex;
                m_smVisionInfo.g_arrPad[i].ref_intColorDefectLinkTolerance = Convert.ToInt32(txt_ColorDefectLinkTolerance_Pad.Text);

                string strSectionName = "";

                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                //2021-05-15 ZJYEOH : Assign Center Want use Gauge measure Pkg Side to 4 sides
                if (blnCheck4SideSettingChanged && i > 0 && m_smVisionInfo.g_blnCheck4Sides)
                {
                    m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize = m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize;
                }

                //STDeviceEdit.CopySettingFile(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                //m_smVisionInfo.g_strVisionFolderName, "\\Pad\\Template\\Template.xml");
                m_smVisionInfo.g_arrPad[i].SaveAdvancePad(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\Template.xml", strSectionName);
                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                //m_smVisionInfo.g_strVisionFolderName, "\\Pad\\Template\\Template.xml", m_smProductionInfo.g_strLotID);

                //2021-05-15 ZJYEOH : Load 4 Sides Settings
                if (blnCheck4SideSettingChanged && i > 0 && m_smVisionInfo.g_blnCheck4Sides)
                {
                    LoadPadSetting();
                }
            }
        }
        private void LoadPadSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Pad) && !tabControl_SettingForm.Controls.Contains(tab_PadPackage) && !tabControl_SettingForm.Controls.Contains(tab_PadPackage2))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == 0)
                {
                    m_smVisionInfo.g_arrPad[i].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelX,
                                                  m_smVisionInfo.g_fCalibPixelY,
                                                  m_smVisionInfo.g_fCalibOffSetX,
                                                  m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                }
                else
                {
                    m_smVisionInfo.g_arrPad[i].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelZ,
                                                  m_smVisionInfo.g_fCalibPixelZ,
                                                  m_smVisionInfo.g_fCalibOffSetZ,
                                                  m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                }
                // Load Pad Template Setting
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrPad[i].LoadPad(strPath + "Pad\\" + "Template\\Template.xml", strSectionName);

                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.LoadRectGauge4L(strPath + "Pad\\" + "RectGauge4L.xml",
                                                              "Pad" + i.ToString(), false);
                m_smVisionInfo.g_arrPad[i].LoadPadTemplateImage(strPath + "Pad\\" + "Template\\", i);
                if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    m_smVisionInfo.g_arrPad[i].LoadPadPackageTemplateImage(strPath + "Package\\" + "Template\\", i);
            }
        }
        private void SavePadPackageAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_PadPackage) && !tabControl_SettingForm.Controls.Contains(tab_PadPackage2))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";
           
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantCheckPackage", chk_WantCheckPackage.Checked);
            objFileHandle.WriteElement1Value("WantUseDetailThresholdPadPackage", chk_WantUseDetailThreshold_PadPackage.Checked);
            objFileHandle.WriteElement1Value("SeperateCrackDefectSetting", chk_SeperateCrackDefectSetting_PadPackage.Checked);
            objFileHandle.WriteElement1Value("SeperateForeignMaterialDefectSetting", chk_SeperateForeignMaterialDefectSetting_PadPackage.Checked);
            objFileHandle.WriteElement1Value("SeperateChippedOffDefectSetting", chk_SeperateChippedOffDefectSetting_PadPackage.Checked);
            objFileHandle.WriteElement1Value("SeperateMoldFlashDefectSetting", chk_SeperateMoldFlashDefectSetting_PadPackage.Checked);
            objFileHandle.WriteElement1Value("PadPkgSizeImageViewNo_Center", cbo_PadPkgSizeImageViewNo_Center.SelectedIndex);
            objFileHandle.WriteElement1Value("PadPkgBrightFieldImageViewNo_Center", cbo_PadPkgBrightFieldImageViewNo_Center.SelectedIndex);
            objFileHandle.WriteElement1Value("PadPkgDarkFieldImageNo_Center", cbo_PadPkgDarkFieldImageNo_Center.SelectedIndex);
            objFileHandle.WriteElement1Value("PadPkgMoldFlashImageViewNo_Center", cbo_PadPkgMoldFlashImageViewNo_Center.SelectedIndex);
            objFileHandle.WriteElement1Value("PadPkgSizeImageViewNo_Side", cbo_PadPkgSizeImageViewNo_Side.SelectedIndex);
            objFileHandle.WriteElement1Value("PadPkgBrightFieldImageViewNo_Side", cbo_PadPkgBrightFieldImageViewNo_Side.SelectedIndex);
            objFileHandle.WriteElement1Value("PadPkgDarkFieldImageNo_Side", cbo_PadPkgDarkFieldImageNo_Side.SelectedIndex);
            objFileHandle.WriteElement1Value("WantDontCareAreaPadPackage", chk_WantDontCareArea_PadPackage.Checked);
            objFileHandle.WriteElement1Value("WantDontCarePadForPackage", chk_WantDontCarePadForPackage.Checked);
            objFileHandle.WriteElement1Value("PadPkgMoldFlashImageViewNo_Side", cbo_PadPkgMoldFlashImageViewNo_Side.SelectedIndex);
            objFileHandle.WriteElement1Value("SeperateBrightDarkROITolerancePadPackage", chk_SeperateBrightDarkROITolerance_PadPackage.Checked);

            objFileHandle.WriteElement1Value("WantLinkBrightDefect", chk_WantLinkBrightDefect_PadPackage.Checked);
            objFileHandle.WriteElement1Value("WantLinkDarkDefect", chk_WantLinkDarkDefect_PadPackage.Checked);
            objFileHandle.WriteElement1Value("WantLinkCrackDefect", chk_WantLinkCrackDefect_PadPackage.Checked);
            objFileHandle.WriteElement1Value("WantLinkMoldFlashDefect", chk_WantLinkMoldFlashDefect_PadPackage.Checked);

            objFileHandle.WriteElement1Value("BrightDefectLinkTolerance", Convert.ToInt32(txt_BrightDefectLinkTolerance_PadPackage.Text));
            objFileHandle.WriteElement1Value("DarkDefectLinkTolerance", Convert.ToInt32(txt_DarkDefectLinkTolerance_PadPackage.Text));
            objFileHandle.WriteElement1Value("CrackDefectLinkTolerance", Convert.ToInt32(txt_CrackDefectLinkTolerance_PadPackage.Text));
            objFileHandle.WriteElement1Value("MoldFlashDefectLinkTolerance", Convert.ToInt32(txt_MoldFlashDefectLinkTolerance_PadPackage.Text));

            objFileHandle.WriteElement1Value("PadPkgMoldFlashDefectType_Center", cbo_PadPkgMoldFlashDefectType_Center.SelectedIndex);
            objFileHandle.WriteElement1Value("PadPkgMoldFlashDefectType_Side", cbo_PadPkgMoldFlashDefectType_Side.SelectedIndex);

            objFileHandle.WriteEndElement();
            
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == 0)
                {
                    if (m_smVisionInfo.g_arrPad[i].ref_intBrightDefectLinkTolerance != Convert.ToInt32(txt_BrightDefectLinkTolerance_PadPackage.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>Bright Defect Link Tolerance", (m_smVisionInfo.g_arrPad[i].ref_intBrightDefectLinkTolerance).ToString(), txt_BrightDefectLinkTolerance_PadPackage.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intDarkDefectLinkTolerance != Convert.ToInt32(txt_DarkDefectLinkTolerance_PadPackage.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>Dark Defect Link Tolerance", (m_smVisionInfo.g_arrPad[i].ref_intDarkDefectLinkTolerance).ToString(), txt_DarkDefectLinkTolerance_PadPackage.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intCrackDefectLinkTolerance != Convert.ToInt32(txt_CrackDefectLinkTolerance_PadPackage.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>Crack Defect Link Tolerance", (m_smVisionInfo.g_arrPad[i].ref_intCrackDefectLinkTolerance).ToString(), txt_CrackDefectLinkTolerance_PadPackage.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnSeperateForeignMaterialDefectSetting != chk_SeperateForeignMaterialDefectSetting_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_SeperateForeignMaterialDefectSetting_PadPackage.Text, (!chk_SeperateForeignMaterialDefectSetting_PadPackage.Checked).ToString(), chk_SeperateForeignMaterialDefectSetting_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectLinkTolerance != Convert.ToInt32(txt_MoldFlashDefectLinkTolerance_PadPackage.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>Mold Flash Defect Link Tolerance", (m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectLinkTolerance).ToString(), txt_MoldFlashDefectLinkTolerance_PadPackage.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnSeperateBrightDarkROITolerance != chk_SeperateBrightDarkROITolerance_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_SeperateBrightDarkROITolerance_PadPackage.Text, (!chk_SeperateBrightDarkROITolerance_PadPackage.Checked).ToString(), chk_SeperateBrightDarkROITolerance_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantLinkBrightDefect != chk_WantLinkBrightDefect_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_WantLinkBrightDefect_PadPackage.Text, (!chk_WantLinkBrightDefect_PadPackage.Checked).ToString(), chk_WantLinkBrightDefect_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantLinkDarkDefect != chk_WantLinkDarkDefect_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_WantLinkDarkDefect_PadPackage.Text, (!chk_WantLinkDarkDefect_PadPackage.Checked).ToString(), chk_WantLinkDarkDefect_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantLinkCrackDefect != chk_WantLinkCrackDefect_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_WantLinkCrackDefect_PadPackage.Text, (!chk_WantLinkCrackDefect_PadPackage.Checked).ToString(), chk_WantLinkCrackDefect_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantLinkMoldFlashDefect != chk_WantLinkMoldFlashDefect_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_WantLinkMoldFlashDefect_PadPackage.Text, (!chk_WantLinkMoldFlashDefect_PadPackage.Checked).ToString(), chk_WantLinkMoldFlashDefect_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnSeperateCrackDefectSetting != chk_SeperateCrackDefectSetting_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_SeperateCrackDefectSetting_PadPackage.Text, (!chk_SeperateCrackDefectSetting_PadPackage.Checked).ToString(), chk_SeperateCrackDefectSetting_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnSeperateMoldFlashDefectSetting != chk_SeperateMoldFlashDefectSetting_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_SeperateMoldFlashDefectSetting_PadPackage.Text, (!chk_SeperateMoldFlashDefectSetting_PadPackage.Checked).ToString(), chk_SeperateMoldFlashDefectSetting_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectType != cbo_PadPkgMoldFlashDefectType_Center.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Mold Flash Defect Type (Center)", cbo_PadPkgMoldFlashDefectType_Center.Items[m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectType].ToString(), cbo_PadPkgMoldFlashDefectType_Center.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnSeperateChippedOffDefectSetting != chk_SeperateChippedOffDefectSetting_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_SeperateChippedOffDefectSetting_PadPackage.Text, (!chk_SeperateChippedOffDefectSetting_PadPackage.Checked).ToString(), chk_SeperateChippedOffDefectSetting_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantDontCareArea_Package != chk_WantDontCareArea_PadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_WantDontCareArea_PadPackage.Text, (!chk_WantDontCareArea_PadPackage.Checked).ToString(), chk_WantDontCareArea_PadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantDontCarePadForPackage != chk_WantDontCarePadForPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + chk_WantDontCarePadForPackage.Text, (!chk_WantDontCarePadForPackage.Checked).ToString(), chk_WantDontCarePadForPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadPkgSizeImageViewNo != cbo_PadPkgSizeImageViewNo_Center.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Package Size Image View No (Center)", cbo_PadPkgSizeImageViewNo_Center.Items[m_smVisionInfo.g_arrPad[i].ref_intPadPkgSizeImageViewNo].ToString(), cbo_PadPkgSizeImageViewNo_Center.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadPkgBrightFieldImageViewNo != cbo_PadPkgBrightFieldImageViewNo_Center.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Brigth Field Package Defect Image View No (Center)", cbo_PadPkgBrightFieldImageViewNo_Center.Items[m_smVisionInfo.g_arrPad[i].ref_intPadPkgBrightFieldImageViewNo].ToString(), cbo_PadPkgBrightFieldImageViewNo_Center.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadPkgDarkFieldImageViewNo != cbo_PadPkgDarkFieldImageNo_Center.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Dark Field Package Defect Image View No (Center)", cbo_PadPkgDarkFieldImageNo_Center.Items[m_smVisionInfo.g_arrPad[i].ref_intPadPkgDarkFieldImageViewNo].ToString(), cbo_PadPkgDarkFieldImageNo_Center.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadPkgMoldFlashImageViewNo != cbo_PadPkgMoldFlashImageViewNo_Center.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Mold Flash Package Defect Image View No (Center)", cbo_PadPkgMoldFlashImageViewNo_Center.Items[m_smVisionInfo.g_arrPad[i].ref_intPadPkgMoldFlashImageViewNo].ToString(), cbo_PadPkgMoldFlashImageViewNo_Center.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                }
                if (i == 1)
                {
                    if (m_smVisionInfo.g_arrPad[i].ref_intPadPkgSizeImageViewNo != cbo_PadPkgSizeImageViewNo_Side.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Package Size Image View No (Side)", cbo_PadPkgSizeImageViewNo_Side.Items[m_smVisionInfo.g_arrPad[i].ref_intPadPkgSizeImageViewNo].ToString(), cbo_PadPkgSizeImageViewNo_Side.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadPkgBrightFieldImageViewNo != cbo_PadPkgBrightFieldImageViewNo_Side.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Brigth Field Package Defect Image View No (Side)", cbo_PadPkgBrightFieldImageViewNo_Side.Items[m_smVisionInfo.g_arrPad[i].ref_intPadPkgBrightFieldImageViewNo].ToString(), cbo_PadPkgBrightFieldImageViewNo_Side.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadPkgDarkFieldImageViewNo != cbo_PadPkgDarkFieldImageNo_Side.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Dark Field Package Defect Image View No (Side)", cbo_PadPkgDarkFieldImageNo_Side.Items[m_smVisionInfo.g_arrPad[i].ref_intPadPkgDarkFieldImageViewNo].ToString(), cbo_PadPkgDarkFieldImageNo_Side.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intPadPkgMoldFlashImageViewNo != cbo_PadPkgMoldFlashImageViewNo_Side.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Mold Flash Package Defect Image View No (Center)", cbo_PadPkgMoldFlashImageViewNo_Center.Items[m_smVisionInfo.g_arrPad[i].ref_intPadPkgMoldFlashImageViewNo].ToString(), cbo_PadPkgMoldFlashImageViewNo_Side.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectType != cbo_PadPkgMoldFlashDefectType_Side.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package", "Advance Setting>" + "Mold Flash Defect Type (Side)", cbo_PadPkgMoldFlashDefectType_Side.Items[m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectType].ToString(), cbo_PadPkgMoldFlashDefectType_Side.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                }
                m_smVisionInfo.g_arrPad[i].ref_blnSeperateBrightDarkROITolerance = chk_SeperateBrightDarkROITolerance_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_intBrightDefectLinkTolerance = Convert.ToInt32(txt_BrightDefectLinkTolerance_PadPackage.Text);
                m_smVisionInfo.g_arrPad[i].ref_intDarkDefectLinkTolerance = Convert.ToInt32(txt_DarkDefectLinkTolerance_PadPackage.Text);
                m_smVisionInfo.g_arrPad[i].ref_intCrackDefectLinkTolerance = Convert.ToInt32(txt_CrackDefectLinkTolerance_PadPackage.Text);
                m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectLinkTolerance = Convert.ToInt32(txt_MoldFlashDefectLinkTolerance_PadPackage.Text);
                m_smVisionInfo.g_arrPad[i].ref_blnWantLinkBrightDefect = chk_WantLinkBrightDefect_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantLinkDarkDefect = chk_WantLinkDarkDefect_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantLinkCrackDefect = chk_WantLinkCrackDefect_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantLinkMoldFlashDefect = chk_WantLinkMoldFlashDefect_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnSeperateCrackDefectSetting = chk_SeperateCrackDefectSetting_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnSeperateForeignMaterialDefectSetting = chk_SeperateForeignMaterialDefectSetting_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnSeperateMoldFlashDefectSetting = chk_SeperateMoldFlashDefectSetting_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnSeperateChippedOffDefectSetting = chk_SeperateChippedOffDefectSetting_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnUseDetailDefectCriteria = m_smVisionInfo.g_blnWantUseDetailThreshold_PadPackage = chk_WantUseDetailThreshold_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantDontCareArea_Package = chk_WantDontCareArea_PadPackage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantDontCarePadForPackage = chk_WantDontCarePadForPackage.Checked;

                if (i == 0)
                {
                    m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectType = cbo_PadPkgMoldFlashDefectType_Center.SelectedIndex;
                    m_smVisionInfo.g_arrPad[i].ref_intPadPkgSizeImageViewNo = cbo_PadPkgSizeImageViewNo_Center.SelectedIndex;
                    m_smVisionInfo.g_arrPad[i].ref_intPadPkgBrightFieldImageViewNo = cbo_PadPkgBrightFieldImageViewNo_Center.SelectedIndex;
                    m_smVisionInfo.g_arrPad[i].ref_intPadPkgDarkFieldImageViewNo = cbo_PadPkgDarkFieldImageNo_Center.SelectedIndex;
                    m_smVisionInfo.g_arrPad[i].ref_intPadPkgMoldFlashImageViewNo = cbo_PadPkgMoldFlashImageViewNo_Center.SelectedIndex;
                }
                else
                {
                    m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectType = cbo_PadPkgMoldFlashDefectType_Side.SelectedIndex;
                    m_smVisionInfo.g_arrPad[i].ref_intPadPkgSizeImageViewNo = cbo_PadPkgSizeImageViewNo_Side.SelectedIndex;
                    m_smVisionInfo.g_arrPad[i].ref_intPadPkgBrightFieldImageViewNo = cbo_PadPkgBrightFieldImageViewNo_Side.SelectedIndex;
                    m_smVisionInfo.g_arrPad[i].ref_intPadPkgDarkFieldImageViewNo = cbo_PadPkgDarkFieldImageNo_Side.SelectedIndex;
                    m_smVisionInfo.g_arrPad[i].ref_intPadPkgMoldFlashImageViewNo = cbo_PadPkgMoldFlashImageViewNo_Side.SelectedIndex;
                }
            }
            
            m_smVisionInfo.g_blnCheckPackage = chk_WantCheckPackage.Checked;
            m_smVisionInfo.g_blnWantDontCareArea_Package = chk_WantDontCareArea_PadPackage.Checked;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_arrPad[i].ref_blnWhiteOnBlack = chk_PadWhiteOnBlack.Checked;
                m_smVisionInfo.g_arrPad[i].ref_fDefaultPixelTolerance = float.Parse(txt_DefaultPixelTolerance.Text);
                m_smVisionInfo.g_arrPad[i].ref_intPadROISizeToleranceADV = int.Parse(txt_PadROITolerance.Text); // 2019-05-06 ZJYEOH : Save the setting to another parameter to avoid different Size ROI during inspection
                //m_smVisionInfo.g_arrPad[i].ref_intPadSizeHalfWidthTolerance = int.Parse(txt_PadSizeHalfWidthTolerance.Text);
                m_smVisionInfo.g_arrPad[i].ref_blnWantTightSetting = chk_WantTestRunTightSetting.Checked;
                m_smVisionInfo.g_arrPad[i].ref_fTightSettingTolerance = float.Parse(txt_TightSettingDimensionTolerance.Text);
                m_smVisionInfo.g_arrPad[i].ref_intTightSettingThresholdTolerance = int.Parse(txt_TightSettingThresholdTolerance.Text);
                m_smVisionInfo.g_arrPad[i].ref_blnWantAutoGauge = chk_WantAutoGauge.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage = chk_WantRotateSidePadImage.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantLinkDifferentGroupPitchGap = chk_WantLinkDifferentGroupPitchGap.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantShowUseGaugeCheckBox = chk_ShowUseGaugeCheckBoxInLearnForm.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantSeparateBrokenPadThresholdSetting = chk_WantSeparateBrokenPadThresholdSetting.Checked;
                m_smVisionInfo.g_arrPad[i].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON = chk_WantViewCheckForeignMaterialOptionWhenPackageON.Checked;
                //m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod = cbo_SensitivityOnPad.SelectedIndex;
                //m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue = int.Parse(txt_SensitivityValue.Text);
                //m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadMethod = m_arrSensitivityOnPadMethod[i];
                //m_smVisionInfo.g_arrPad[i].ref_intSensitivityOnPadValue = m_arrSensitivityOnPadValue[i];
                m_smVisionInfo.g_arrPad[i].ref_blnMeasureCenterPkgSizeUsingSidePkg = chk_MeasureCenterPkgSizeUsingSidePkg.Checked;
                if (i > 0)
                    m_smVisionInfo.g_arrPad[i].ref_blnWantConsiderPadImage2 = chk_WantConsiderPadImage2.Checked;    // For side pad only.
                m_smVisionInfo.g_arrPad[i].ref_blnWantPRUnitLocationBeforeGauge = chk_WantPRUnitLocationBeforeGauge.Checked;

                if (i == 0)
                {
                    if (cbo_PadMeasurementMethod.SelectedIndex == 0)
                        m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension = false;
                    else
                        m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension = true;
                }
                else
                {
                    if (cbo_PadMeasurementMethod_Side.SelectedIndex == 0)
                        m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension = false;
                    else
                        m_smVisionInfo.g_arrPad[i].ref_blnWantUseGaugeMeasureDimension = true;
                }

                m_smVisionInfo.g_arrPad[i].ref_blnWantUseClosestSizeDefineTolerance = chk_DefinePadToleranceUsingClosestMethod.Checked;

                if (i == 0)
                    m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo = cbo_CenterROIBrokenPadImageViewNo.SelectedIndex;
                else
                    m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo = cbo_SideROIBrokenPadImageViewNo.SelectedIndex;

                string strSectionName = "";

                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrPad[i].SaveAdvancePad(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\Template.xml", strSectionName);

            }
        }

        private void SaveLeadAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Lead))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                               m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Settings.xml";
           
            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantCheckLead", chk_WantCheckLead.Checked);
            objFileHandle.WriteElement1Value("WantPocketDontCareAreaFix_Lead", chk_WantPocketDontCareAreaFix_Lead.Checked);
            objFileHandle.WriteElement1Value("WantPocketDontCareAreaManual_Lead", chk_WantPocketDontCareAreaManual_Lead.Checked);
            objFileHandle.WriteElement1Value("WantPocketDontCareAreaAuto_Lead", chk_WantPocketDontCareAreaAuto_Lead.Checked);
            objFileHandle.WriteElement1Value("WantPocketDontCareAreaBlob_Lead", chk_WantPocketDontCareAreaBlob_Lead.Checked);
            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);

            if (m_smVisionInfo.g_blnCheckLead != chk_WantCheckLead.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + chk_WantCheckLead.Text, (!chk_WantCheckLead.Checked).ToString(), chk_WantCheckLead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantPocketDontCareAreaFix_Lead != chk_WantPocketDontCareAreaFix_Lead.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Dont Care Method " + chk_WantPocketDontCareAreaFix_Lead.Text, (!chk_WantPocketDontCareAreaFix_Lead.Checked).ToString(), chk_WantPocketDontCareAreaFix_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantPocketDontCareAreaManual_Lead != chk_WantPocketDontCareAreaManual_Lead.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Dont Care Method " + chk_WantPocketDontCareAreaManual_Lead.Text, (!chk_WantPocketDontCareAreaManual_Lead.Checked).ToString(), chk_WantPocketDontCareAreaManual_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantPocketDontCareAreaAuto_Lead != chk_WantPocketDontCareAreaAuto_Lead.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Dont Care Method " + chk_WantPocketDontCareAreaAuto_Lead.Text, (!chk_WantPocketDontCareAreaAuto_Lead.Checked).ToString(), chk_WantPocketDontCareAreaAuto_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantPocketDontCareAreaBlob_Lead != chk_WantPocketDontCareAreaBlob_Lead.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Dont Care Method" + chk_WantPocketDontCareAreaBlob_Lead.Text, (!chk_WantPocketDontCareAreaBlob_Lead.Checked).ToString(), chk_WantPocketDontCareAreaBlob_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            m_smVisionInfo.g_blnCheckLead = chk_WantCheckLead.Checked;
            m_smVisionInfo.g_blnWantPocketDontCareAreaFix_Lead = chk_WantPocketDontCareAreaFix_Lead.Checked;
            m_smVisionInfo.g_blnWantPocketDontCareAreaManual_Lead = chk_WantPocketDontCareAreaManual_Lead.Checked;
            m_smVisionInfo.g_blnWantPocketDontCareAreaAuto_Lead = chk_WantPocketDontCareAreaAuto_Lead.Checked;
            m_smVisionInfo.g_blnWantPocketDontCareAreaBlob_Lead = chk_WantPocketDontCareAreaBlob_Lead.Checked;
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (i == 0)
                {
                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantUseGaugeMeasureLeadDimension != chk_WantUseGaugeMeasureLeadDimension.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + chk_WantUseGaugeMeasureLeadDimension.Text, (!chk_WantUseGaugeMeasureLeadDimension.Checked).ToString(), chk_WantUseGaugeMeasureLeadDimension.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantUsePkgToBaseTolerance != chk_WantPkgToBaseTolerance_Lead.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + chk_WantPkgToBaseTolerance_Lead.Text, (!chk_WantPkgToBaseTolerance_Lead.Checked).ToString(), chk_WantPkgToBaseTolerance_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantUseAverageGrayValueMethod != chk_WantUseAverageGrayValueMethod_Lead.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + chk_WantUseAverageGrayValueMethod_Lead.Text, (!chk_WantUseAverageGrayValueMethod_Lead.Checked).ToString(), chk_WantUseAverageGrayValueMethod_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_intImageViewNo != cbo_LeadDefectImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + "Lead Defect Image View No", cbo_LeadDefectImageNo.Items[m_smVisionInfo.g_arrLead[i].ref_intImageViewNo].ToString(), cbo_LeadDefectImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_intBaseLeadImageViewNo != cbo_BaseLeadImageNo.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + "Base Lead Image View No", cbo_BaseLeadImageNo.Items[m_smVisionInfo.g_arrLead[i].ref_intBaseLeadImageViewNo].ToString(), cbo_BaseLeadImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantPocketDontCareAreaFix_Lead != chk_WantPocketDontCareAreaFix_Lead.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Dont Care Method" + chk_WantPocketDontCareAreaFix_Lead.Text, (!chk_WantPocketDontCareAreaFix_Lead.Checked).ToString(), chk_WantPocketDontCareAreaFix_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantPocketDontCareAreaManual_Lead != chk_WantPocketDontCareAreaManual_Lead.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Dont Care Method" + chk_WantPocketDontCareAreaManual_Lead.Text, (!chk_WantPocketDontCareAreaManual_Lead.Checked).ToString(), chk_WantPocketDontCareAreaManual_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantPocketDontCareAreaAuto_Lead != chk_WantPocketDontCareAreaAuto_Lead.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Dont Care Method" + chk_WantPocketDontCareAreaAuto_Lead.Text, (!chk_WantPocketDontCareAreaAuto_Lead.Checked).ToString(), chk_WantPocketDontCareAreaAuto_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantPocketDontCareAreaBlob_Lead != chk_WantPocketDontCareAreaBlob_Lead.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Dont Care Method" + chk_WantPocketDontCareAreaBlob_Lead.Text, (!chk_WantPocketDontCareAreaBlob_Lead.Checked).ToString(), chk_WantPocketDontCareAreaBlob_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_intRotationMethod != cbo_RotationMethod_Lead.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + "Lead Rotation Method", (cbo_RotationMethod_Lead.Items[m_smVisionInfo.g_arrLead[i].ref_intRotationMethod]).ToString(), cbo_RotationMethod_Lead.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_intLeadAngleTolerance != Convert.ToInt32(txt_LeadAngleTolerance.Text))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + "Lead Angle Tolerance", m_smVisionInfo.g_arrLead[i].ref_intLeadAngleTolerance.ToString(), txt_LeadAngleTolerance.Text, m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_intPocketDontCareMethod != cbo_PocketDontCareMethod_Lead.SelectedIndex)
                    {
                        if (cbo_PocketDontCareMethod_Lead.SelectedIndex == 0)
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Pocket Dont Care Algorithm", "SRM1", "Standard", m_smProductionInfo.g_strLotID);
                        else
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>Pocket Dont Care Algorithm", "Standard", "SRM1", m_smProductionInfo.g_strLotID);

                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantUseAGVMasking != chk_WantUseMasking_Lead.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + chk_WantUseMasking_Lead.Text, (!chk_WantUseMasking_Lead.Checked).ToString(), chk_WantUseMasking_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnWantInspectBaseLead != chk_WantInspectBaseLead.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Advance Setting>" + chk_WantInspectBaseLead.Text, (!chk_WantInspectBaseLead.Checked).ToString(), chk_WantInspectBaseLead.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                }
                
                m_smVisionInfo.g_arrLead[i].ref_blnWantInspectBaseLead = chk_WantInspectBaseLead.Checked;
                m_smVisionInfo.g_arrLead[i].ref_intPocketDontCareMethod = cbo_PocketDontCareMethod_Lead.SelectedIndex;
                m_smVisionInfo.g_arrLead[i].ref_blnWantUseGaugeMeasureLeadDimension = chk_WantUseGaugeMeasureLeadDimension.Checked;
                m_smVisionInfo.g_arrLead[i].ref_blnWantUsePkgToBaseTolerance = chk_WantPkgToBaseTolerance_Lead.Checked;
                m_smVisionInfo.g_arrLead[i].ref_blnWantUseAverageGrayValueMethod = chk_WantUseAverageGrayValueMethod_Lead.Checked;
                m_smVisionInfo.g_arrLead[i].ref_blnWantPocketDontCareAreaFix_Lead = chk_WantPocketDontCareAreaFix_Lead.Checked;
                m_smVisionInfo.g_arrLead[i].ref_blnWantPocketDontCareAreaManual_Lead = chk_WantPocketDontCareAreaManual_Lead.Checked;
                m_smVisionInfo.g_arrLead[i].ref_blnWantPocketDontCareAreaAuto_Lead = chk_WantPocketDontCareAreaAuto_Lead.Checked;
                m_smVisionInfo.g_arrLead[i].ref_blnWantPocketDontCareAreaBlob_Lead = chk_WantPocketDontCareAreaBlob_Lead.Checked;
                m_smVisionInfo.g_arrLead[i].ref_intRotationMethod = cbo_RotationMethod_Lead.SelectedIndex;
                m_smVisionInfo.g_arrLead[i].ref_intLeadAngleTolerance = Convert.ToInt32(txt_LeadAngleTolerance.Text);
                m_smVisionInfo.g_arrLead[i].ref_blnWantUseAGVMasking = chk_WantUseMasking_Lead.Checked;

                string strSectionName = "";
                if (i == 0)
                    strSectionName = "SearchROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                //STDeviceEdit.CopySettingFile(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                //m_smVisionInfo.g_strVisionFolderName, "\\Lead\\Template\\Template.xml");
                m_smVisionInfo.g_arrLead[i].ref_intBaseLeadImageViewNo = cbo_BaseLeadImageNo.SelectedIndex;
                m_smVisionInfo.g_arrLead[i].ref_intImageViewNo = cbo_LeadDefectImageNo.SelectedIndex;
                m_smVisionInfo.g_arrLead[i].SaveLead(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\Template.xml", false, strSectionName, true);
                m_smVisionInfo.g_arrLead[i].LoadLead(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\Template.xml", strSectionName, m_smVisionInfo.g_arrImages.Count);
                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                //m_smVisionInfo.g_strVisionFolderName, "\\Lead\\Template\\Template.xml", m_smProductionInfo.g_strLotID);
            }
        }
        private void SavePocketPositionAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_PocketPosition))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                               m_smVisionInfo.g_strVisionFolderName + "\\PocketPosition\\Settings.xml";

            
            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");

            objFileHandle.WriteElement1Value("WantCheckPocketPosition", chk_WantCheckPocketPosition.Checked);
            objFileHandle.WriteElement1Value("WantUsePocketPattern", chk_WantUsePocketPattern.Checked);
            objFileHandle.WriteElement1Value("WantUsePocketGauge", chk_WantUsePocketGauge.Checked);
         
            objFileHandle.WriteEndElement();
            
            if (m_smVisionInfo.g_blnWantCheckPocketPosition != chk_WantCheckPocketPosition.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Pocket Position>" + chk_WantCheckPocketPosition.Text, (!chk_WantCheckPocketPosition.Checked).ToString(), chk_WantCheckPocketPosition.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantUsePocketPattern != chk_WantUsePocketPattern.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Pocket Position>" + chk_WantUsePocketPattern.Text, (!chk_WantUsePocketPattern.Checked).ToString(), chk_WantUsePocketPattern.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantUsePocketGauge != chk_WantUsePocketGauge.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Pocket Position>" + chk_WantUsePocketGauge.Text, (!chk_WantUsePocketGauge.Checked).ToString(), chk_WantUsePocketGauge.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0) != cbo_PocketPatternAndGaugeImageNo.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Pocket Position>" + "Pocket Pattern Image View No", cbo_PocketPatternAndGaugeImageNo.Items[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0)].ToString(), cbo_PocketPatternAndGaugeImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1) != cbo_PlateGaugeImageNo.SelectedIndex)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Advance Setting>Pocket Position>" + "Plate Gauge Image View No", cbo_PlateGaugeImageNo.Items[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1)].ToString(), cbo_PlateGaugeImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            m_smVisionInfo.g_blnWantCheckPocketPosition = chk_WantCheckPocketPosition.Checked;
            m_smVisionInfo.g_blnWantUsePocketPattern = chk_WantUsePocketPattern.Checked;
            m_smVisionInfo.g_blnWantUsePocketGauge = chk_WantUsePocketGauge.Checked;

            m_smVisionInfo.g_objPocketPosition.SetGrabImageIndex(0, cbo_PocketPatternAndGaugeImageNo.SelectedIndex);
            m_smVisionInfo.g_objPocketPosition.SetGrabImageIndex(1, cbo_PlateGaugeImageNo.SelectedIndex);

            m_smVisionInfo.g_objPocketPosition.SavePocketPosition(strPath, false, "Settings", true);

        }
        private void SaveLead3DAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Lead3D))
                return;


            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                              m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Settings.xml";
     
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantShowGRR", chk_WantGRR_Lead3D.Checked);
            objFileHandle.WriteElement1Value("WantDontCareAreaLead3D", chk_WantDontCareArea_Lead3D.Checked);
            objFileHandle.WriteElement1Value("WantPin1", chk_WantLead3DPin1.Checked);
            objFileHandle.WriteElement1Value("WantCheckPH", chk_WantCheckPH_Lead3D.Checked);
            objFileHandle.WriteEndElement();
            
            if (m_smVisionInfo.g_blnWantShowGRR != chk_WantGRR_Lead3D.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantGRR_Lead3D.Text, (!chk_WantGRR_Lead3D.Checked).ToString(), chk_WantGRR_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantDontCareArea_Lead3D != chk_WantDontCareArea_Lead3D.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantDontCareArea_Lead3D.Text, (!chk_WantDontCareArea_Lead3D.Checked).ToString(), chk_WantDontCareArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantPin1 != chk_WantLead3DPin1.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantLead3DPin1.Text, (!chk_WantLead3DPin1.Checked).ToString(), chk_WantLead3DPin1.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckPH != chk_WantCheckPH_Lead3D.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantCheckPH_Lead3D.Text, (!chk_WantCheckPH_Lead3D.Checked).ToString(), chk_WantCheckPH_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            m_smVisionInfo.g_blnWantShowGRR = chk_WantGRR_Lead3D.Checked;
            m_smVisionInfo.g_blnWantDontCareArea_Lead3D = chk_WantDontCareArea_Lead3D.Checked;
            m_smVisionInfo.g_blnWantCheckPH = chk_WantCheckPH_Lead3D.Checked;
            m_smVisionInfo.g_blnWantPin1 = chk_WantLead3DPin1.Checked;

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                       m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\";

            if (m_smVisionInfo.g_blnWantCheckPH) // Load Position Setting if want check PH
            {
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
                string strPositionPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
                ROI.LoadFile(strPositionPath + "Positioning\\PHROI.xml", m_smVisionInfo.g_arrPHROIs);

                LoadPositioningSettings(strPositionPath + "Positioning\\");
            }
            else
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i == 0)
                {
                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnWantUsePkgToBaseTolerance != chk_WantPkgToBaseTolerance_Lead3D.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantPkgToBaseTolerance_Lead3D.Text, (!chk_WantPkgToBaseTolerance_Lead3D.Checked).ToString(), chk_WantPkgToBaseTolerance_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnWantUseGaugeMeasureBase != chk_WantUseGaugeMeasureBase_Lead3D.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantUseGaugeMeasureBase_Lead3D.Text, (!chk_WantUseGaugeMeasureBase_Lead3D.Checked).ToString(), chk_WantUseGaugeMeasureBase_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].ref_intImageRotateOption != cbo_ImageRotateOption_Lead3D.SelectedIndex)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>Image Rotate Option", cbo_ImageRotateOption_Lead3D.Items[m_smVisionInfo.g_arrLead3D[i].ref_intImageRotateOption].ToString(), cbo_ImageRotateOption_Lead3D.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }
                    
                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnWantUseAverageGrayValueMethod != chk_WantUseAverageGrayValueMethod_Lead3D.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantUseAverageGrayValueMethod_Lead3D.Text, (!chk_WantUseAverageGrayValueMethod_Lead3D.Checked).ToString(), chk_WantUseAverageGrayValueMethod_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }
                    
                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnWantDontCareArea_Lead3D != chk_WantDontCareArea_Lead3D.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantDontCareArea_Lead3D.Text, (!chk_WantDontCareArea_Lead3D.Checked).ToString(), chk_WantDontCareArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnWantUseAGVMasking != chk_WantUseMasking_Lead3D.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Advance Setting>" + chk_WantUseMasking_Lead3D.Text, (!chk_WantUseMasking_Lead3D.Checked).ToString(), chk_WantUseMasking_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }
                }
                m_smVisionInfo.g_arrLead3D[i].ref_blnWantUseAGVMasking = chk_WantUseMasking_Lead3D.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_blnMeasureCenterPkgSizeUsingCorner = chk_MeasureCenterPkgSizeUsingCorner.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_blnWantUsePkgToBaseTolerance = chk_WantPkgToBaseTolerance_Lead3D.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_intImageRotateOption = cbo_ImageRotateOption_Lead3D.SelectedIndex;
                m_smVisionInfo.g_arrLead3D[i].ref_blnWantUseGaugeMeasureBase = chk_WantUseGaugeMeasureBase_Lead3D.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_blnWantUseAverageGrayValueMethod = chk_WantUseAverageGrayValueMethod_Lead3D.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_intLeadWidthDisplayOption = cbo_Lead3DWidthDisplayOption.SelectedIndex;
                m_smVisionInfo.g_arrLead3D[i].ref_intLeadLengthVarianceMethod = cbo_Lead3DLengthVarianceMethod.SelectedIndex;
                m_smVisionInfo.g_arrLead3D[i].ref_intLeadSpanMethod = cbo_Lead3DSpanMethod.SelectedIndex;
                m_smVisionInfo.g_arrLead3D[i].ref_intLeadContaminationRegion = cbo_Lead3DContaminationRegion.SelectedIndex;
                m_smVisionInfo.g_arrLead3D[i].ref_intLeadStandOffMethod = cbo_Lead3DStandOffMethod.SelectedIndex;
                m_smVisionInfo.g_arrLead3D[i].ref_blnWantDontCareArea_Lead3D = chk_WantDontCareArea_Lead3D.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_intLeadWidthRangeSelection = cbo_LeadWidthRangeSelection.SelectedIndex;
                m_smVisionInfo.g_arrLead3D[i].ref_intLeadWidthRange = Convert.ToInt32(txt_LeadWidthRange.Text);
                m_smVisionInfo.g_arrLead3D[i].ref_intMatchingXTolerance = Convert.ToInt32(txt_MatchingXTolerance_Lead3D.Text);
                m_smVisionInfo.g_arrLead3D[i].ref_intMatchingYTolerance = Convert.ToInt32(txt_MatchingYTolerance_Lead3D.Text);

                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrLead3D[i].SaveLead3D(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);

            }
            
        }

        private void SaveLead3DPackageAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Lead3DPackage))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Settings.xml";
            
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantUseDetailThresholdLeadPackage", chk_WantUseDetailThreshold_LeadPackage.Checked);
            objFileHandle.WriteElement1Value("SeperateCrackDefectSetting", chk_SeperateCrackDefectSetting_LeadPackage.Checked);
            objFileHandle.WriteElement1Value("SeperateChippedOffDefectSetting", chk_SeperateChippedOffDefectSetting_LeadPackage.Checked);
            objFileHandle.WriteElement1Value("SeperateMoldFlashDefectSetting", chk_SeperateMoldFlashDefectSetting_LeadPackage.Checked);

            objFileHandle.WriteEndElement();
            
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i == 0)
                {
                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnMeasureCenterPkgSizeUsingCorner != chk_MeasureCenterPkgSizeUsingCorner.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + chk_MeasureCenterPkgSizeUsingCorner.Text, (!chk_MeasureCenterPkgSizeUsingCorner.Checked).ToString(), chk_MeasureCenterPkgSizeUsingCorner.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnSeperateCrackDefectSetting != chk_SeperateCrackDefectSetting_LeadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + chk_SeperateCrackDefectSetting_LeadPackage.Text, (!chk_SeperateCrackDefectSetting_LeadPackage.Checked).ToString(), chk_SeperateCrackDefectSetting_LeadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnSeperateMoldFlashDefectSetting != chk_SeperateMoldFlashDefectSetting_LeadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + chk_SeperateMoldFlashDefectSetting_LeadPackage.Text, (!chk_SeperateMoldFlashDefectSetting_LeadPackage.Checked).ToString(), chk_SeperateMoldFlashDefectSetting_LeadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnSeperateChippedOffDefectSetting != chk_SeperateChippedOffDefectSetting_LeadPackage.Checked)
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + chk_SeperateChippedOffDefectSetting_LeadPackage.Text, (!chk_SeperateChippedOffDefectSetting_LeadPackage.Checked).ToString(), chk_SeperateChippedOffDefectSetting_LeadPackage.Checked.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].GetGrabImageIndex(0) != GetArrayImageIndex(cbo_LeadPkgSizeImageNo.SelectedIndex))
                    {
                        if (m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0) >= cbo_LeadPkgSizeImageNo.Items.Count)
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + "Package Size Image View No", cbo_LeadPkgSizeImageNo.Items[cbo_LeadPkgSizeImageNo.Items.Count - 1].ToString(), cbo_LeadPkgSizeImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                        else
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + "Package Size Image View No", cbo_LeadPkgSizeImageNo.Items[m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0)].ToString(), cbo_LeadPkgSizeImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].GetGrabImageIndex(1) != GetArrayImageIndex(cbo_LeadPkgBrightFieldImageNo.SelectedIndex))
                    {
                        if (m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1) >= cbo_LeadPkgBrightFieldImageNo.Items.Count)
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + "Bright Field Package Defect Image View No", cbo_LeadPkgBrightFieldImageNo.Items[cbo_LeadPkgBrightFieldImageNo.Items.Count - 1].ToString(), cbo_LeadPkgBrightFieldImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                        else
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + "Bright Field Package Defect Image View No", cbo_LeadPkgBrightFieldImageNo.Items[m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1)].ToString(), cbo_LeadPkgBrightFieldImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                    if (m_smVisionInfo.g_arrLead3D[i].GetGrabImageIndex(2) != GetArrayImageIndex(cbo_LeadPkgDarkFieldImageNo.SelectedIndex))
                    {
                        if (m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2) >= cbo_LeadPkgDarkFieldImageNo.Items.Count)
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + "Dark Field Package Defect Image View No", cbo_LeadPkgDarkFieldImageNo.Items[cbo_LeadPkgDarkFieldImageNo.Items.Count - 1].ToString(), cbo_LeadPkgDarkFieldImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                        else
                            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Advance Setting>" + "Dark Field Package Defect Image View No", cbo_LeadPkgDarkFieldImageNo.Items[m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2)].ToString(), cbo_LeadPkgDarkFieldImageNo.SelectedItem.ToString(), m_smProductionInfo.g_strLotID);
                    }

                }
                m_smVisionInfo.g_arrLead3D[i].ref_blnMeasureCenterPkgSizeUsingCorner = chk_MeasureCenterPkgSizeUsingCorner.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_blnSeperateCrackDefectSetting = chk_SeperateCrackDefectSetting_LeadPackage.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_blnSeperateMoldFlashDefectSetting = chk_SeperateMoldFlashDefectSetting_LeadPackage.Checked;
                m_smVisionInfo.g_arrLead3D[i].ref_blnSeperateChippedOffDefectSetting = chk_SeperateChippedOffDefectSetting_LeadPackage.Checked;
                //m_smVisionInfo.g_arrLead3D[i].ref_blnUseDetailDefectCriteria = m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage = chk_WantUseDetailThreshold_LeadPackage.Checked;

                m_smVisionInfo.g_arrLead3D[i].SetGrabImageIndex(0, GetArrayImageIndex(cbo_LeadPkgSizeImageNo.SelectedIndex));
                m_smVisionInfo.g_arrLead3D[i].SetGrabImageIndex(1, GetArrayImageIndex(cbo_LeadPkgBrightFieldImageNo.SelectedIndex));
                m_smVisionInfo.g_arrLead3D[i].SetGrabImageIndex(2, GetArrayImageIndex(cbo_LeadPkgDarkFieldImageNo.SelectedIndex));
            }
            
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                string strSectionName = "";

                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrLead3D[i].SaveLead3D(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Template\\Template.xml", false, strSectionName, true);

            }
        }
        private void SaveBarcodeAdvSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tp_Barcode))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                               m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\Settings.xml";

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantUseAngleRange", chk_WantUseAngleRange.Checked);
            objFileHandle.WriteElement1Value("WantUseGainRange", chk_WantUseGainRange.Checked);
            objFileHandle.WriteElement1Value("WantUseReferenceImage", chk_WantUseReferenceImage.Checked);
            objFileHandle.WriteElement1Value("WantUseUniformize3x3", chk_WantUseUniformize3x3.Checked);
            objFileHandle.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Advance Settings", strPath, "", m_smProductionInfo.g_strLotID);

            if (m_smVisionInfo.g_objBarcode.ref_blnWantUseAngleRange != chk_WantUseAngleRange.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>" + chk_WantUseAngleRange.Text, (!chk_WantUseAngleRange.Checked).ToString(), chk_WantUseAngleRange.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objBarcode.ref_blnWantUseGainRange != chk_WantUseGainRange.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>" + chk_WantUseGainRange.Text, (!chk_WantUseGainRange.Checked).ToString(), chk_WantUseGainRange.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objBarcode.ref_blnWantUseReferenceImage != chk_WantUseReferenceImage.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>" + chk_WantUseReferenceImage.Text, (!chk_WantUseReferenceImage.Checked).ToString(), chk_WantUseReferenceImage.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objBarcode.ref_blnWantUseUniformize3x3 != chk_WantUseUniformize3x3.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>" + chk_WantUseUniformize3x3.Text, (!chk_WantUseUniformize3x3.Checked).ToString(), chk_WantUseUniformize3x3.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objBarcode.ref_intRetestCount != Convert.ToInt32(txt_BarcodeRestestCount.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>Barcode Restest Count", m_smVisionInfo.g_objBarcode.ref_intRetestCount.ToString(), txt_BarcodeRestestCount.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objBarcode.ref_intUniformizeGain != Convert.ToInt32(txt_UniformizeGain.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>Barcode Uniformize Gain", m_smVisionInfo.g_objBarcode.ref_intUniformizeGain.ToString(), txt_UniformizeGain.Value.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objBarcode.ref_fDontCareScale != Convert.ToDouble(txt_BarcodeDontCareScale.Value / 100))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>Barcode Dont Care Scale", (m_smVisionInfo.g_objBarcode.ref_fDontCareScale * 100).ToString(), txt_BarcodeDontCareScale.Value.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle != Convert.ToDouble(txt_BarcodeOrientationAngle.Text))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>Barcode Orientation Angle", m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle.ToString(), txt_BarcodeOrientationAngle.Text, m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_objBarcode.ref_blnWantUseGainRange != chk_WantUseGainRange.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode", "Advance Setting>" + chk_WantUseGainRange.Text, (!chk_WantUseGainRange.Checked).ToString(), chk_WantUseGainRange.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            m_smVisionInfo.g_objBarcode.ref_blnWantUseReferenceImage = chk_WantUseReferenceImage.Checked;
            m_smVisionInfo.g_objBarcode.ref_blnWantUseUniformize3x3 = chk_WantUseUniformize3x3.Checked;
            m_smVisionInfo.g_objBarcode.ref_blnWantUseAngleRange = chk_WantUseAngleRange.Checked;
            m_smVisionInfo.g_objBarcode.ref_blnWantUseGainRange = chk_WantUseGainRange.Checked;
            m_smVisionInfo.g_objBarcode.ref_intBarcodeDetectionAreaTolerance = Convert.ToInt32(txt_BarcodeDetectionAreaTolerance.Text);
            m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top = Convert.ToInt32(txt_PatternDetectionAreaTolerance_Top.Text);
            m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right = Convert.ToInt32(txt_PatternDetectionAreaTolerance_Right.Text);
            m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom = Convert.ToInt32(txt_PatternDetectionAreaTolerance_Bottom.Text);
            m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left = Convert.ToInt32(txt_PatternDetectionAreaTolerance_Left.Text);
            m_smVisionInfo.g_objBarcode.ref_intRetestCount = Convert.ToInt32(txt_BarcodeRestestCount.Text);
            m_smVisionInfo.g_objBarcode.ref_intDelayTimeAfterPass = Convert.ToInt32(txt_BarcodeDelayTimeAfterPass.Text);
            m_smVisionInfo.g_objBarcode.ref_intUniformizeGain = Convert.ToInt32(txt_UniformizeGain.Value);
            m_smVisionInfo.g_objBarcode.ref_fDontCareScale = (float)Convert.ToDouble(txt_BarcodeDontCareScale.Value / 100);
            m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle = (float)Convert.ToDouble(txt_BarcodeOrientationAngle.Text);
            m_smVisionInfo.g_objBarcode.SaveBarcode(strPath, false, "Settings", true);

        }
        private void AdvanceSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void AdvanceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Advance Setting Form Closed", "Exit Advance Setting Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            //if (m_blnWantSkipMarkPrev != chk_SkipMarkInspection.Checked)
            //{
            //    if (SRMMessageBox.Show("Change the Skip Mark Inspection will delete all previous templates. Are you sure you want to continue?",
            //        "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            //    {
            //        return;
            //    }
            //}

            if (Convert.ToInt32(txt_MinMarkScore.Text) > Convert.ToInt32(txt_DefaultMarkScore.Text))
            {
                SRMMessageBox.Show("Default Mark Score cannot less than Minimum Mark Score.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            SaveGeneralAdvSetting();

            SaveBarcodeAdvSetting();

            SavePocketPositionAdvSetting();

            SaveOrientAdvSetting();

            SaveMarkAdvSetting();

            SavePackageAdvSetting();

            SavePadAdvSetting();

            SaveLeadAdvSetting();

            SaveSealAdvSetting();

            SavePadPackageAdvSetting();

            SaveLead3DAdvSetting();

            SaveLead3DPackageAdvSetting();

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            this.DialogResult = DialogResult.OK;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            // note: dont dispose this form because the properties still need to be used by other form.
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            LoadPadPackageSetting();

            if (m_smVisionInfo.g_objBarcode != null)
            {
                m_smVisionInfo.g_objBarcode.ref_intBarcodeDetectionAreaTolerance = m_intBarcodeInspectionArea_Prev;
                m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top = m_intBarcodePatternInspectionArea_Top_Prev;
                m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right = m_intBarcodePatternInspectionArea_Right_Prev;
                m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom = m_intBarcodePatternInspectionArea_Bottom_Prev;
                m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left = m_intBarcodePatternInspectionArea_Left_Prev;
            }
            LoadMarkTypeSetting();
            Close();
            Dispose();
        }

        private void btn_LineProfileGaugeSetting_Click(object sender, EventArgs e)
        {
            m_blnDragROIPrev = m_smVisionInfo.g_blnDragROI;

            m_smVisionInfo.g_blnDragROI = false;
            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\PointGauge.xml";

            if (m_objLineProfileForm == null)
                m_objLineProfileForm = new LineProfileForm(m_smVisionInfo, m_smVisionInfo.g_arrPad[0].ref_objPointGauge, strPath,m_smProductionInfo);

            m_objLineProfileForm.Show();

            this.Hide();
        }

        private void ClientTimer_Tick(object sender, EventArgs e)
        {
            //chk_WantUseEmptyPattern.Enabled = chk_WantCheckEmpty.Checked;
            //chk_WantUseEmptyThreshold.Enabled = chk_WantCheckEmpty.Checked;

            //chk_WantUsePocketPattern.Enabled = chk_WantCheckPocketPosition.Checked;
            //chk_WantUsePocketGauge.Enabled = chk_WantCheckPocketPosition.Checked;

            if (m_objLineProfileForm != null)
            {
                if (!m_objLineProfileForm.ref_blnShow)
                {
                    m_objLineProfileForm.Close();
                    m_objLineProfileForm.Dispose();
                    m_objLineProfileForm = null;
                    this.Show();

                    m_smVisionInfo.g_blnDragROI = m_blnDragROIPrev;
                }
            }

            if (m_objLeadLineProfileForm != null)
            {
                if (!m_objLeadLineProfileForm.ref_blnShow)
                {
                    m_objLeadLineProfileForm.Close();
                    m_objLeadLineProfileForm.Dispose();
                    m_objLeadLineProfileForm = null;
                    this.Show();

                    m_smVisionInfo.g_blnDragROI = m_blnDragROIPrev;
                }
            }
        }

        private void cbo_PadMeasurementMethod_Click(object sender, EventArgs e)
        {
        }

        private void btn_LeadLineProfileGaugeSetting_Click(object sender, EventArgs e)
        {

        }

        private void cbo_intPocketPitch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_intPocketPitch.SelectedItem.Equals("Not Used"))
            {
                SRMMessageBox.Show("Please Select Value other than Not Used.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cbo_intPocketPitch.SelectedIndex = intPocketPitchPrev;
                return;
            }
        }

        private void cbo_Package_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillPocketPitch();
        }

        private void txt_MaxMarkTemplate_TextChanged(object sender, EventArgs e)
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Mark":
                case "MarkOrient":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                    if (Convert.ToInt32(txt_MaxMarkTemplate.Text) > 8)
                    {
                        SRMMessageBox.Show("Maximum Template cannot more than 8.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txt_MaxMarkTemplate.Text = "8";
                    }
                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLi":
                case "IPMLiPkg":
                    if (Convert.ToInt32(txt_MaxMarkTemplate.Text) > 4)
                    {
                        SRMMessageBox.Show("Maximum Template cannot more than 4.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txt_MaxMarkTemplate.Text = "4";
                    }
                    break;
            }
        }

        private void pnl_WantConsiderPadImage2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_PadSensitivity_Click(object sender, EventArgs e)
        {
            PadSubSettingForm objForm = new PadSubSettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, m_intUserGroup, m_intSelectedTabPage, chk_WantCheck4Sides.Checked);
            if (objForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_arrPad[0].ref_intSensitivityOnPadMethod = objForm.ref_intSensitivityOnPadMethod_Center;
                m_smVisionInfo.g_arrPad[0].ref_intSensitivityOnPadValue = objForm.ref_intSensitivityOnPadValue_Center;
                if (objForm.ref_blnAutoSensitivity_Center)
                    m_smVisionInfo.g_arrPad[0].ref_intInspectPadMode = 1;
                else
                    m_smVisionInfo.g_arrPad[0].ref_intInspectPadMode = 0;

                if (m_smVisionInfo.g_arrPad.Length == 5)
                {
                    m_smVisionInfo.g_arrPad[1].ref_intSensitivityOnPadMethod = objForm.ref_intSensitivityOnPadMethod_Top;
                    m_smVisionInfo.g_arrPad[1].ref_intSensitivityOnPadValue = objForm.ref_intSensitivityOnPadValue_Top;
                    if (objForm.ref_blnAutoSensitivity_Top)
                        m_smVisionInfo.g_arrPad[1].ref_intInspectPadMode = 1;
                    else
                        m_smVisionInfo.g_arrPad[1].ref_intInspectPadMode = 0;

                    m_smVisionInfo.g_arrPad[2].ref_intSensitivityOnPadMethod = objForm.ref_intSensitivityOnPadMethod_Right;
                    m_smVisionInfo.g_arrPad[2].ref_intSensitivityOnPadValue = objForm.ref_intSensitivityOnPadValue_Right;
                    if (objForm.ref_blnAutoSensitivity_Right)
                        m_smVisionInfo.g_arrPad[2].ref_intInspectPadMode = 1;
                    else
                        m_smVisionInfo.g_arrPad[2].ref_intInspectPadMode = 0;

                    m_smVisionInfo.g_arrPad[3].ref_intSensitivityOnPadMethod = objForm.ref_intSensitivityOnPadMethod_Bottom;
                    m_smVisionInfo.g_arrPad[3].ref_intSensitivityOnPadValue = objForm.ref_intSensitivityOnPadValue_Bottom;
                    if (objForm.ref_blnAutoSensitivity_Bottom)
                        m_smVisionInfo.g_arrPad[3].ref_intInspectPadMode = 1;
                    else
                        m_smVisionInfo.g_arrPad[3].ref_intInspectPadMode = 0;

                    m_smVisionInfo.g_arrPad[4].ref_intSensitivityOnPadMethod = objForm.ref_intSensitivityOnPadMethod_Left;
                    m_smVisionInfo.g_arrPad[4].ref_intSensitivityOnPadValue = objForm.ref_intSensitivityOnPadValue_Left;
                    if (objForm.ref_blnAutoSensitivity_Left)
                        m_smVisionInfo.g_arrPad[4].ref_intInspectPadMode = 1;
                    else
                        m_smVisionInfo.g_arrPad[4].ref_intInspectPadMode = 0;
                }

                //m_arrSensitivityOnPadMethod[0] = objForm.ref_intSensitivityOnPadMethod_Center;
                //m_arrSensitivityOnPadValue[0] = objForm.ref_intSensitivityOnPadValue_Center;

                //if (m_arrSensitivityOnPadMethod.Length == 5)
                //{
                //    m_arrSensitivityOnPadMethod[1] = objForm.ref_intSensitivityOnPadMethod_Top;
                //    m_arrSensitivityOnPadValue[1] = objForm.ref_intSensitivityOnPadValue_Top;

                //    m_arrSensitivityOnPadMethod[2] = objForm.ref_intSensitivityOnPadMethod_Right;
                //    m_arrSensitivityOnPadValue[2] = objForm.ref_intSensitivityOnPadValue_Right;

                //    m_arrSensitivityOnPadMethod[3] = objForm.ref_intSensitivityOnPadMethod_Bottom;
                //    m_arrSensitivityOnPadValue[3] = objForm.ref_intSensitivityOnPadValue_Bottom;

                //    m_arrSensitivityOnPadMethod[4] = objForm.ref_intSensitivityOnPadMethod_Left;
                //    m_arrSensitivityOnPadValue[4] = objForm.ref_intSensitivityOnPadValue_Left;
                //}
            }
        }

        private void LoadPadPackageSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Pad) && !tabControl_SettingForm.Controls.Contains(tab_PadPackage) && !tabControl_SettingForm.Controls.Contains(tab_PadPackage2))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\";

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                // Load Pad Template Setting
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrPad[i].LoadPad(strPath + "Template.xml", strSectionName);

            }
        }
        private void LoadMarkTypeSetting()
        {
            if (!tabControl_SettingForm.Controls.Contains(tab_Mark3))
                return;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Settings.xml";

            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("Advanced");
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckCharExcessMark = objFile.GetValueAsBoolean("WantCheckCharExcessMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckCharMissingMark = objFile.GetValueAsBoolean("WantCheckCharMissingMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckCharBrokenMark = objFile.GetValueAsBoolean("WantCheckCharBrokenMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckLogoExcessMark = objFile.GetValueAsBoolean("WantCheckLogoExcessMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckLogoMissingMark = objFile.GetValueAsBoolean("WantCheckLogoMissingMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckLogoBrokenMark = objFile.GetValueAsBoolean("WantCheckLogoBrokenMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol1ExcessMark = objFile.GetValueAsBoolean("WantCheckSymbol1ExcessMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol1MissingMark = objFile.GetValueAsBoolean("WantCheckSymbol1MissingMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol1BrokenMark = objFile.GetValueAsBoolean("WantCheckSymbol1BrokenMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol2ExcessMark = objFile.GetValueAsBoolean("WantCheckSymbol2ExcessMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol2MissingMark = objFile.GetValueAsBoolean("WantCheckSymbol2MissingMark", true);
                m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol2BrokenMark = objFile.GetValueAsBoolean("WantCheckSymbol2BrokenMark", true);
            }
        }
        private int GetArrayImageIndex(int intUserSelectImageIndex)
        {
            if (intUserSelectImageIndex < 0)
                return 0;

            switch (m_smVisionInfo.g_intImageMergeType)
            {
                default:
                case 0: // No merge
                    {
                        return intUserSelectImageIndex;
                    }
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;
                        else if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                            return intUserSelectImageIndex;
                        else
                            return (intUserSelectImageIndex + 1);
                    }
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;
                        else if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                            return intUserSelectImageIndex;
                        else if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                            return (intUserSelectImageIndex + 1);
                        else
                            return (intUserSelectImageIndex + 2);
                    }
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;

                        if (intUserSelectImageIndex == 2) // select image 2 which is grab 5
                        {
                            if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 2);
                        }
                        else // select image 1 which is grab 3 and 4
                        {
                            if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 1);
                        }
                    }
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 4 center and grab 5 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;

                        if (intUserSelectImageIndex == 1) // select image 1 which is grab 4 and 5
                        {
                            if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 2);
                        }
                        else // select other than image 0 or 1
                        {
                            if (intUserSelectImageIndex + 3 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 3);
                        }
                    }
            }
        }

        private void chk_WantUsePocketPattern_Click(object sender, EventArgs e)
        {
            if (!chk_WantUsePocketGauge.Checked)
            {
                chk_WantUsePocketPattern.Checked = true;
            }
        }

        private void chk_WantUsePocketGauge_Click(object sender, EventArgs e)
        {
            if (!chk_WantUsePocketPattern.Checked)
            {
                chk_WantUsePocketGauge.Checked = true;
            }
        }

        private void cbo_PocketPatternAndGaugeImageNo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btn_PadOffsetSetting_Click(object sender, EventArgs e)
        {
            PadSubSettingForm2 objForm = new PadSubSettingForm2(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, m_intUserGroup, m_intSelectedTabPage, chk_WantCheck4Sides.Checked);
            if (objForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Center;

                if (m_smVisionInfo.g_arrPad.Length == 5)
                {
                    m_smVisionInfo.g_arrPad[1].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Top;
                    m_smVisionInfo.g_arrPad[2].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Right;
                    m_smVisionInfo.g_arrPad[3].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Bottom;
                    m_smVisionInfo.g_arrPad[4].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Left;
                }
            }
        }

        private void chk_WantCheckEmpty_Click(object sender, EventArgs e)
        {
            chk_WantUseEmptyPattern.Enabled = chk_WantCheckEmpty.Checked;
            chk_WantUseEmptyThreshold.Enabled = chk_WantCheckEmpty.Checked;
        }

        private void chk_WantCheckPocketPosition_Click(object sender, EventArgs e)
        {
            chk_WantUsePocketPattern.Enabled = chk_WantCheckPocketPosition.Checked;
            chk_WantUsePocketGauge.Enabled = chk_WantCheckPocketPosition.Checked;
        }

        private void cbo_DefectInspectionMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            //btn_PackageGrayValueSensitivitySetting.Visible = (cbo_PackageDefectInspectionMethod.SelectedIndex == 1);
        }

        private void btn_PackageGrayValueSensitivitySetting_Click(object sender, EventArgs e)
        {
            if ((m_smVisionInfo.g_arrPackage.Count > 0) && (m_smVisionInfo.g_arrPackageROIs.Count > 0))
            {
                if (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit].Count > 0)
                {
                    GrayValueSensitivitySettingForm objForm = new GrayValueSensitivitySettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, 1);
                    objForm.ShowDialog();
                }
                else
                {
                    SRMMessageBox.Show("Please Learn Package first!", "", MessageBoxButtons.OK);
                }
            }
            else
            {
                SRMMessageBox.Show("Please Learn Package first!", "", MessageBoxButtons.OK);
            }
        }

        private void cbo_MarkDefectInspectionMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            //btn_MarkGrayValueSensitivitySetting.Visible = (cbo_MarkDefectInspectionMethod.SelectedIndex == 1);
        }

        private void btn_MarkGrayValueSensitivitySetting_Click(object sender, EventArgs e)
        {
            if ((m_smVisionInfo.g_arrMarks.Count > 0) && (m_smVisionInfo.g_arrMarkROIs.Count > 0))
            {
                if (m_smVisionInfo.g_arrMarkROIs[0].Count > 0)
                {
                    GrayValueSensitivitySettingForm objForm = new GrayValueSensitivitySettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, 0);
                    objForm.ShowDialog();
                }
                else
                {
                    SRMMessageBox.Show("Please Learn Mark first!", "", MessageBoxButtons.OK);
                }
            }
            else
            {
                SRMMessageBox.Show("Please Learn Mark first!", "", MessageBoxButtons.OK);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.AT_ALL_InAuto && m_smProductionInfo.g_intAutoLogOutMinutes > 0 && m_intUserGroup != 5)
            {
                DateTime t = DateTime.Now;
                TimeSpan tSpan = t - m_smProductionInfo.g_DTStartAutoMode_IndividualForm;

                if (tSpan.Minutes >= m_smProductionInfo.g_intAutoLogOutMinutes)
                {
                    m_smProductionInfo.g_intUserGroup = 5;
                    m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
                    DisableField2();
                }
            }
        }

        private void chk_SeperateDarkField2DefectSetting_Package_Click(object sender, EventArgs e)
        {
            pnl_WantLinkDark2Defect.Visible = pnl_SideLightPackageDarkImage.Visible = chk_SeperateDarkField2DefectSetting_Package.Checked;
        }

        private void chk_SeperateDarkField4DefectSetting_Package_Click(object sender, EventArgs e)
        {
            pnl_WantLinkDark4Defect.Visible = pnl_SideLightPackageDark3Image.Visible = chk_SeperateDarkField4DefectSetting_Package.Checked;
        }

        private void chk_SeperateDarkField3DefectSetting_Package_Click(object sender, EventArgs e)
        {
            pnl_WantLinkDark3Defect.Visible = pnl_SideLightPackageDark2Image.Visible = chk_SeperateDarkField3DefectSetting_Package.Checked;
        }

        private void chk_WantStandoff_Pad_Click(object sender, EventArgs e)
        {
            btn_PadStandOffDirectionSetting.Enabled = chk_WantStandOff_Pad.Checked;
            if (chk_WantEdgeLimit_Pad.Checked && chk_WantStandOff_Pad.Checked)
                chk_WantEdgeLimit_Pad.Checked = !chk_WantStandOff_Pad.Checked;
        }

        private void btn_PadStandOffDirectionSetting_Click(object sender, EventArgs e)
        {
            PadStandOffSettingForm objForm = new PadStandOffSettingForm(m_smCustomizeInfo, m_smVisionInfo,
                        m_strSelectedRecipe, m_intUserGroup, m_smProductionInfo);
            if (objForm.ShowDialog() == DialogResult.OK)
            {
                //m_smVisionInfo.g_arrPad[0].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Center;

                //if (m_smVisionInfo.g_arrPad.Length == 5)
                //{
                //    m_smVisionInfo.g_arrPad[1].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Top;
                //    m_smVisionInfo.g_arrPad[2].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Right;
                //    m_smVisionInfo.g_arrPad[3].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Bottom;
                //    m_smVisionInfo.g_arrPad[4].ref_blnWantUseBorderLimitAsOffset = objForm.ref_blnUseBorderLimit_Left;
                //}
            }
            //objForm.Close();
            //objForm.Dispose();
        }

        private void chk_WantEdgeLimit_Pad_Click(object sender, EventArgs e)
        {
            if (chk_WantStandOff_Pad.Checked && chk_WantEdgeLimit_Pad.Checked)
                chk_WantStandOff_Pad.Checked = !chk_WantEdgeLimit_Pad.Checked;

            if (!chk_WantStandOff_Pad.Checked)
                btn_PadStandOffDirectionSetting.Enabled = false;
        }

        private void btn_AverageGrayValueROITolerance_Lead3D_Click(object sender, EventArgs e)
        {
            Lead3DAverageGrayValueROIToleranceForm objForm = new Lead3DAverageGrayValueROIToleranceForm(m_smCustomizeInfo, m_smVisionInfo, m_smProductionInfo,
                      m_strSelectedRecipe, m_intUserGroup);
            if (objForm.ShowDialog() == DialogResult.OK)
            {
              
            }
            objForm.Dispose();
        }

        private void chk_WantUseAverageGrayValueMethod_Lead3D_Click(object sender, EventArgs e)
        {
            btn_AverageGrayValueROITolerance_Lead3D.Enabled = chk_WantUseAverageGrayValueMethod_Lead3D.Checked;
            if (!chk_WantUseAverageGrayValueMethod_Lead3D.Enabled)
            {
                btn_AverageGrayValueROITolerance_Lead3D.Enabled = false;
            }
        }

        private void btn_AverageGrayValueROITolerance_Lead_Click(object sender, EventArgs e)
        {
            LeadAverageGrayValueROIToleranceForm objForm = new LeadAverageGrayValueROIToleranceForm(m_smCustomizeInfo, m_smVisionInfo, m_smProductionInfo,
                      m_strSelectedRecipe, m_intUserGroup);
            if (objForm.ShowDialog() == DialogResult.OK)
            {

            }
            objForm.Dispose();
        }

        private void chk_WantUseAverageGrayValueMethod_Lead_Click(object sender, EventArgs e)
        {
            btn_AverageGrayValueROITolerance_Lead.Enabled = chk_WantUseAverageGrayValueMethod_Lead.Checked;
            if (!chk_WantUseAverageGrayValueMethod_Lead.Enabled)
            {
                btn_AverageGrayValueROITolerance_Lead.Enabled = false;
            }

        }
        private void chk_WantPocketDontCareAreaFix_Lead_Click(object sender, EventArgs e)
        {
            if (chk_WantPocketDontCareAreaFix_Lead.Checked)
            {
                if (chk_WantPocketDontCareAreaManual_Lead.Checked)
                    chk_WantPocketDontCareAreaManual_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaAuto_Lead.Checked)
                    chk_WantPocketDontCareAreaAuto_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaBlob_Lead.Checked)
                    chk_WantPocketDontCareAreaBlob_Lead.Checked = false;
            }
        }
        private void chk_WantPocketDontCareAreaManual_Lead_Click(object sender, EventArgs e)
        {
            if (chk_WantPocketDontCareAreaManual_Lead.Checked)
            {
                if (chk_WantPocketDontCareAreaFix_Lead.Checked)
                    chk_WantPocketDontCareAreaFix_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaAuto_Lead.Checked)
                    chk_WantPocketDontCareAreaAuto_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaBlob_Lead.Checked)
                    chk_WantPocketDontCareAreaBlob_Lead.Checked = false;
            }
        }

        private void chk_WantPocketDontCareAreaAuto_Lead_Click(object sender, EventArgs e)
        {
            if (chk_WantPocketDontCareAreaAuto_Lead.Checked)
            {
                if (chk_WantPocketDontCareAreaFix_Lead.Checked)
                    chk_WantPocketDontCareAreaFix_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaManual_Lead.Checked)
                    chk_WantPocketDontCareAreaManual_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaBlob_Lead.Checked)
                    chk_WantPocketDontCareAreaBlob_Lead.Checked = false;
            }
        }
        private void chk_WantPocketDontCareAreaBlob_Lead_Click(object sender, EventArgs e)
        {
            if (chk_WantPocketDontCareAreaBlob_Lead.Checked)
            {
                if (chk_WantPocketDontCareAreaFix_Lead.Checked)
                    chk_WantPocketDontCareAreaFix_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaManual_Lead.Checked)
                    chk_WantPocketDontCareAreaManual_Lead.Checked = false;
                if (chk_WantPocketDontCareAreaAuto_Lead.Checked)
                    chk_WantPocketDontCareAreaAuto_Lead.Checked = false;
            }
        }
        private void cbo_RotationMethod_Lead_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_RotationMethod_Lead.SelectedIndex == 2 || ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
            {
                chk_WantPkgToBaseTolerance_Lead.Visible = true;
            }
            else
            {
                chk_WantPkgToBaseTolerance_Lead.Visible = false;
            }
        }

        private void panel22_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txt_BarcodeDetectionAreaTolerance_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewBarcodeInspectionArea = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BarcodeDetectionAreaTolerance_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewBarcodeInspectionArea = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BarcodeDetectionAreaTolerance_TextChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objBarcode.ref_intBarcodeDetectionAreaTolerance = Convert.ToInt32(txt_BarcodeDetectionAreaTolerance.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PatternDetectionAreaTolerance_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewBarcodePatternInspectionArea = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PatternDetectionAreaTolerance_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewBarcodePatternInspectionArea = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SeperateCrackDefectSetting_Package_Click(object sender, EventArgs e)
        {
            pnl_WantLinkCrackDefect.Visible = chk_SeperateCrackDefectSetting_Package.Checked;
        }

        private void chk_SeperateMoldFlashDefectSetting_Package_Click(object sender, EventArgs e)
        {
            pnl_WantLinkMoldFlashDefect.Visible = pnl_MoldFlashDefectImageView.Visible = chk_SeperateMoldFlashDefectSetting_Package.Checked;
        }

        private void chk_SeperateMoldFlashDefectSetting_PadPackage_Click(object sender, EventArgs e)
        {
            pnl_WantLinkMoldFlashDefect_PadPackage.Visible = pnl_PadPkgMoldFlashDefectImageView.Visible = chk_SeperateMoldFlashDefectSetting_PadPackage.Checked;
        }

        private void chk_SeperateCrackDefectSetting_PadPackage_Click(object sender, EventArgs e)
        {
            pnl_WantLinkCrackDefect_PadPackage.Visible = chk_SeperateCrackDefectSetting_PadPackage.Checked;
        }

        private void btn_PadColorGroupSetting_Click(object sender, EventArgs e)
        {
            ColorPadGroupSettingForm objForm = new ColorPadGroupSettingForm(m_smCustomizeInfo, m_smVisionInfo,
                       m_strSelectedRecipe, m_intUserGroup, m_smProductionInfo);
            if (objForm.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void btn_MarkTypeInspectionSetting_Click(object sender, EventArgs e)
        {
            MarkTypeInspectionSettingForm objForm = new MarkTypeInspectionSettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, m_intUserGroup);
            if (objForm.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckCharExcessMark = objForm.ref_blnWantCheckCharExcessMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckCharMissingMark = objForm.ref_blnWantCheckCharMissingMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckCharBrokenMark = objForm.ref_blnWantCheckCharBrokenMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckLogoExcessMark = objForm.ref_blnWantCheckLogoExcessMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckLogoMissingMark = objForm.ref_blnWantCheckLogoMissingMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckLogoBrokenMark = objForm.ref_blnWantCheckLogoBrokenMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol1ExcessMark = objForm.ref_blnWantCheckSymbol1ExcessMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol1MissingMark = objForm.ref_blnWantCheckSymbol1MissingMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol1BrokenMark = objForm.ref_blnWantCheckSymbol1BrokenMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol2ExcessMark = objForm.ref_blnWantCheckSymbol2ExcessMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol2MissingMark = objForm.ref_blnWantCheckSymbol2MissingMark;
                    m_smVisionInfo.g_arrMarks[i].ref_blnWantCheckSymbol2BrokenMark = objForm.ref_blnWantCheckSymbol2BrokenMark;
                }
            }
        }

        private void chk_WantUseMarkTypeInspectionSetting_Click(object sender, EventArgs e)
        {
            btn_MarkTypeInspectionSetting.Visible = chk_WantUseMarkTypeInspectionSetting.Checked;
        }

        private void txt_PatternDetectionAreaTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top = Convert.ToInt32(txt_PatternDetectionAreaTolerance_Top.Text);
            m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right = Convert.ToInt32(txt_PatternDetectionAreaTolerance_Right.Text);
            m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom = Convert.ToInt32(txt_PatternDetectionAreaTolerance_Bottom.Text);
            m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left = Convert.ToInt32(txt_PatternDetectionAreaTolerance_Left.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SeparateExtraMarkThreshold_Click(object sender, EventArgs e)
        {
            chk_WantExcessMarkThresholdFollowExtraMarkThreshold.Visible = chk_SeparateExtraMarkThreshold.Checked;
        }

        private void cbo_ColorDefectLinkMethod_Pad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_ColorDefectLinkMethod_Pad.SelectedIndex == 0)
            {
                pnl_ColorDefectLinkTolerance_Pad.Visible = false;
            }
            else
            {
                pnl_ColorDefectLinkTolerance_Pad.Visible = true;
            }
        }

        private void cbo_ColorDefectLinkMethod_Package_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_ColorDefectLinkMethod_Package.SelectedIndex == 0)
            {
                pnl_ColorDefectLinkTolerance_Package.Visible = false;
            }
            else
            {
                pnl_ColorDefectLinkTolerance_Package.Visible = true;
            }
        }

        private void chk_WantPkgToBaseTolerance_Lead3D_Click(object sender, EventArgs e)
        {
            chk_WantUseGaugeMeasureBase_Lead3D.Visible = chk_WantPkgToBaseTolerance_Lead3D.Checked;
        }

        private void txt_MaxSealMarkTemplate_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txt_MaxSealMarkTemplate.Text) > 8)
            {
                SRMMessageBox.Show("Maximum Template cannot more than 8.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txt_MaxSealMarkTemplate.Text = "8";
            }
        }

        private void chk_WantUseOCRnOCV_Click(object sender, EventArgs e)
        {
            chk_WantUseOCR.Checked = !chk_WantUseOCRnOCV.Checked;
        }

        private void chk_WantUseOCR_Click(object sender, EventArgs e)
        {
            chk_WantUseOCRnOCV.Checked = !chk_WantUseOCR.Checked;
        }

        private void chk_SeperateChippedOffDefectSetting_Package_Click(object sender, EventArgs e)
        {
            pnl_ChippedOffDefectInspectionMethod_Package.Visible = chk_SeperateChippedOffDefectSetting_Package.Checked;
        }

        private void txt_MaxSealEmptyTemplate_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txt_MaxSealEmptyTemplate.Text) > 4)
            {
                SRMMessageBox.Show("Maximum Template cannot more than 4.", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txt_MaxSealEmptyTemplate.Text = "4";
            }
        }
        private void chk_PositionWantGauge_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_PositionWantGauge.Checked)
                m_smVisionInfo.g_blnOrientWantPackage = true;
            else
                m_smVisionInfo.g_blnOrientWantPackage = false;
        }

        private void chk_OrientWantGauge_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_OrientWantPackage.Checked) //check package size onli
                m_smVisionInfo.g_blnOrientWantPackage = true;
            else
                m_smVisionInfo.g_blnOrientWantPackage = false;
        }

        private string GetDecimalFormat()
        {
            switch (m_smCustomizeInfo.g_intMarkUnitDisplay)
            {
                case 0:
                    return string.Empty;
                    break;
                case 1:
                    return ("F" + 4);
                    break;
                case 2:
                    return ("F" + 3);
                    break;
                case 3:
                    return ("F" + 1);
                    break;
            }

            return string.Empty;
        }
    }
}