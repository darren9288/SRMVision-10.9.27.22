using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;
using VisionProcessing;
using Microsoft.Win32;
using System.IO;
namespace VisionProcessForm
{
    public partial class LearnPackageForm : Form
    {
        #region Member Variables
        private bool m_blnGaugeResult = false;
        private List<ImageDrawing> m_arrRotatedImage = new List<ImageDrawing>();
        private List<CImageDrawing> m_arrColorRotatedImage = new List<CImageDrawing>();
        private int m_intOrientAngle = 0;
        private int m_intLearnType = 0; // 0 = Learn All, 1 : Side Gauge ROI, 2 : Top Gauge ROI, 3 : Package ROI Tolerance, 4 : Chipped Off ROI Tolerance, 5 : Mold Flash ROI Tolerance, 6 : Dont Care ROI, 7: Dark Field 2 ROI Tolerance
        private int m_intSelectedDontCareROIIndexPrev = 0;
        private bool m_blnInitGaugeAngle = false;
        private bool m_blnInitDone = false;
        private bool m_blnDisablePkgGaugeSetting = true;    // For advance setting
        private bool m_blnIgnoreComboboxIndexChange = false;
        private bool m_blnVisible_pnl_BrightThreshold = true;
        private bool m_blnVisible_pnl_DarkThreshold = true;
        private bool m_blnProductionTestPrev = false;
        private int m_intDisplayStep;
        private int m_intUserGroup;
        private float m_fUnitPRResultCenterX = 0;
        private float m_fUnitPRResultCenterY = 0;
        private string m_strFolderPath = "";
        private string m_strSelectedRecipe;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        
        private AdvancedRectGaugeM4LForm m_objAdvancedRectGaugeForm = null;
        private ColorPackageMultiThresholdForm m_objColorMultiThresholdForm;

        private int m_intVisionType = 0; // 0 = Normal, 1 = Color
        #endregion

        public LearnPackageForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intLearnType, int intVisionType)
        {
            m_intLearnType = intLearnType;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_intVisionType = intVisionType;
            m_intDisplayStep = 1;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\";    

            InitializeComponent();

            InitVariable();
            DisableField2();
           
            UpdateGUI();

            if (m_intVisionType == 0) // Normal
            {
                switch (m_intLearnType)
                {
                    case 0:
                        m_smVisionInfo.g_intLearnStepNo = 0;
                        break;
                    case 1:
                        m_smVisionInfo.g_intLearnStepNo = 0;
                        break;
                    case 2:
                        m_smVisionInfo.g_intLearnStepNo = 1;
                        break;
                    case 3:
                        m_smVisionInfo.g_intLearnStepNo = 2;
                        break;
                    case 4:
                        m_smVisionInfo.g_intLearnStepNo = 8;
                        break;
                    case 5:
                        m_smVisionInfo.g_intLearnStepNo = 12;
                        break;
                    case 6:
                        m_smVisionInfo.g_intLearnStepNo = 20;
                        break;
                    case 7:
                        m_smVisionInfo.g_intLearnStepNo = 14;
                        break;
                }
            }
            else //Color
            {
                m_smVisionInfo.g_intLearnStepNo = 0;
            }

            m_blnInitDone = true;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Package";
            string strChild2 = "Learn Page";
            string strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_Save.Enabled = false;
                btn_Save.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Gauge Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_ShowDraggingBox.Enabled = false;
                chk_ShowSamplePoints.Enabled = false;
                btn_GaugeAdvanceSetting.Enabled = false;
                chk_ProductionTest.Enabled = false;
                chk_ShowDraggingBox_2.Enabled = false;
                chk_ShowSamplePoints_2.Enabled = false;
                btn_GaugeAdvanceSetting2.Enabled = false;

                chk_ShowDraggingBox.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_ShowSamplePoints.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_GaugeAdvanceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_ProductionTest.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_ShowDraggingBox_2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_ShowSamplePoints_2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_GaugeAdvanceSetting2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Package ROI Tolerance Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_StartPixelFromEdge.Enabled = false;
                txt_StartPixelFromRight.Enabled = false;
                txt_StartPixelFromBottom.Enabled = false;
                txt_StartPixelFromLeft.Enabled = false;
                chk_SetToAll.Enabled = false;
                chk_SetToBrightDark.Enabled = false;
                cbo_PkgDefectType.Enabled = false;

                txt_StartPixelFromEdge.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromRight.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromBottom.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromLeft.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAll.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToBrightDark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                cbo_PkgDefectType.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_PkgTop.Visible = lbl_PkgBottom.Visible = lbl_PkgLeft.Visible = lbl_PkgRight.Visible = srmLabel39.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Gray Value Sensitivity Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_PackageGrayValueSensitivitySetting.Enabled = false;
                btn_PackageGrayValueSensitivitySetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Bright Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_BrightMinArea.Enabled = false;
                txt_BrightMinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_Bright.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Bright Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_BrightThreshold.Enabled = false;
                pnl_BrightThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                m_blnVisible_pnl_BrightThreshold = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_BrightMinArea.Enabled)
                {
                    lbl_Image2_BrightField_Simple.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_BrightDefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Dark Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_DarkMinArea.Enabled = false;
                txt_DarkMinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_Dark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Dark Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_DarkThreshold.Enabled = false;
                pnl_DarkThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                m_blnVisible_pnl_DarkThreshold = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_DarkMinArea.Enabled)
                {
                    lbl_Image3_DarkField_Simple.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_DarkDefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Dark 2 ROI Tolerance Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_StartPixelFromEdge_DarkField2.Enabled = false;
                txt_StartPixelFromRight_DarkField2.Enabled = false;
                txt_StartPixelFromBottom_DarkField2.Enabled = false;
                txt_StartPixelFromLeft_DarkField2.Enabled = false;
                chk_SetToAll_DarkField2.Enabled = false;

                txt_StartPixelFromEdge_DarkField2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromRight_DarkField2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromBottom_DarkField2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromLeft_DarkField2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAll_DarkField2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_PkgDarkTop.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_PkgDarkBottom.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_PkgDarkLeft.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_PkgDarkRight.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel65.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

            }

            strChild3 = "Dark 3 ROI Tolerance Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_StartPixelFromEdge_DarkField3.Enabled = false;
                txt_StartPixelFromRight_DarkField3.Enabled = false;
                txt_StartPixelFromBottom_DarkField3.Enabled = false;
                txt_StartPixelFromLeft_DarkField3.Enabled = false;
                chk_SetToAll_DarkField3.Enabled = false;

                txt_StartPixelFromEdge_DarkField3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromRight_DarkField3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromBottom_DarkField3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromLeft_DarkField3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAll_DarkField3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel17.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel19.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel20.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel38.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Dark 4 ROI Tolerance Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_StartPixelFromEdge_DarkField4.Enabled = false;
                txt_StartPixelFromRight_DarkField4.Enabled = false;
                txt_StartPixelFromBottom_DarkField4.Enabled = false;
                txt_StartPixelFromLeft_DarkField4.Enabled = false;
                chk_SetToAll_DarkField4.Enabled = false;

                txt_StartPixelFromEdge_DarkField4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromRight_DarkField4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromBottom_DarkField4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_StartPixelFromLeft_DarkField4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAll_DarkField4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel63.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel49.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel51.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel52.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel48.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Dark 2 Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_Dark2MinArea.Enabled = false;
                txt_Dark2MinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_Dark2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Dark 3 Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_Dark3MinArea.Enabled = false;
                txt_Dark3MinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_Dark3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }


            strChild3 = "Dark 4 Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_Dark4MinArea.Enabled = false;
                txt_Dark4MinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_Dark4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Dark 2 Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_Dark2Threshold.Enabled = false;
                pnl_Dark2Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_Dark2MinArea.Enabled)
                {
                    srmLabel80.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_Dark2DefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }


            strChild3 = "Dark 3 Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_Dark3Threshold.Enabled = false;
                pnl_Dark3Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_Dark3MinArea.Enabled)
                {
                    srmLabel44.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_Dark3DefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }


            strChild3 = "Dark 4 Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_Dark4Threshold.Enabled = false;
                pnl_Dark4Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_Dark4MinArea.Enabled)
                {
                    srmLabel72.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_Dark4DefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Crack Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_CrackViewMinArea.Enabled = false;
                txt_CrackViewMinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_DarkCrack.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Crack Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_CrackViewThreshold.Enabled = false;
                pnl_DarkCrackThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_CrackViewMinArea.Enabled)
                {
                    lbl_Image3_DarkField_Crack.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_CrackDefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Chipped Off ROI Tolerance Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_ROITop_Chip.Enabled = false;
                gb_ROIRight_Chip.Enabled = false;
                gb_ROIBottom_Chip.Enabled = false;
                gb_ROILeft_Chip.Enabled = false;
                chk_SetToAll_Chip.Enabled = false;
                chk_SetToBrightDark_Chip.Enabled = false;
                cbo_ChippedOffDefectType.Enabled = false;

                gb_ROITop_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gb_ROIRight_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gb_ROIBottom_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gb_ROILeft_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAll_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToBrightDark_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                cbo_ChippedOffDefectType.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;


                srmLabel46.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel47.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Chipped Off Bright Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_Image2MinArea.Enabled = false;
                txt_Image2MinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_BrightChippedOff.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Chipped Off Bright Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_ChipViewImage2Threshold.Enabled = false;
                pnl_BrightChippedOffThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_Image2MinArea.Enabled)
                {
                    lbl_Image2_BrightField_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_ChippedOffBrightDefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Chipped Off Dark Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_Image3MinArea.Enabled = false;
                txt_Image3MinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_DarkChippedOff.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Chipped Off Dark Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_ChipViewImage3Threshold.Enabled = false;
                pnl_DarkChippedOffThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_Image3MinArea.Enabled)
                {
                    lbl_Image3_DarkField_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_ChippedOffDarkDefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Mold Flash ROI Tolerance Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                grp_MoldTop.Enabled = false;
                grp_MoldRight.Enabled = false;
                grp_MoldBottom.Enabled = false;
                grp_MoldLeft.Enabled = false;
                chk_SetToAll_Mold.Enabled = false;

                grp_MoldTop.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                grp_MoldRight.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                grp_MoldBottom.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                grp_MoldLeft.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAll_Mold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel35.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Mold Flash Defect Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MoldFlashMinArea.Enabled = false;
                txt_MoldFlashMinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_MinArea_MoldFlash.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Mold Flash Defect Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_MoldFlashThreshold.Enabled = false;
                pnl_BrightMoldFlashThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_MoldFlashMinArea.Enabled)
                {
                    lbl_Image2_BrightField_MF.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    gb_MoldFlashDefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }
            }

            strChild3 = "Dont Care Area Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                radio_Bright.Enabled = false;
                radio_Dark.Enabled = false;
                cbo_DontCareAreaDrawMethod.Enabled = false;
                chk_BrightAndDarkSameDontCareArea.Enabled = false;
                btn_Undo.Enabled = false;
                btn_AddDontCareROI.Enabled = false;
                btn_DeleteDontCareROI.Enabled = false;

                radio_Bright.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                radio_Dark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                cbo_DontCareAreaDrawMethod.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_BrightAndDarkSameDontCareArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_Undo.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_AddDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_DeleteDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_DeleteButton.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_AddButton.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_DrawMethod.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                lbl_DontCareAreaMode.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_DunCareROIBright.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_DunCareROIDark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Mold Flash Dont Care Area Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_AddMoldFlashDontCareROI.Enabled = false;
                btn_DeleteMoldFlashDontCareROI.Enabled = false;
                btn_AddMoldFlashDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_DeleteMoldFlashDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel1.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                srmLabel34.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Learn Color Page";
            strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_SaveColor.Enabled = false;
                btn_SaveColor.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Color Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_ColorThreshold.Enabled = false;
                btn_ColorThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }
        }
        private int GetUserRightGroup_Child3(string Child1, string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
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
            }

            return 1;
        }
        private void InitVariable()
        {
            for (int i = m_smVisionInfo.g_arrPackageGaugeM4L.Count; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackageGaugeM4L.Add(new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex));
            }

            for (int i = m_smVisionInfo.g_arrPackageGauge2M4L.Count; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackageGauge2M4L.Add(new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex));
            }
        }

        private void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "Save Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
       
            }
        }


        private void AddGauge(ROI objSearchROI, List<RectGaugeM4L> arrGauge, string strVisionModule)
        {
            XmlParser objFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\" + strVisionModule + "\\Gauge.xml");
            objFile.GetFirstSection("RectG");

            //create new gauge and attach to selected ROI
            RectGaugeM4L objGauge = new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex);

            //attach searcher parent
            objGauge.SetGaugePlace_BasedOnEdgeROI();

            //set gauge measurement
            objGauge.SetGaugeTransType(objFile.GetValueAsInt("TransType", 0));
            objGauge.SetGaugeTransChoice(objFile.GetValueAsInt("TransChoice", 0));

            //set gauge setting
            objGauge.SetGaugeThickness(objFile.GetValueAsInt("Thickness", 13));
            objGauge.SetGaugeFilter(objFile.GetValueAsInt("Filter", 1));
            objGauge.SetGaugeThreshold(objFile.GetValueAsInt("Threshold", 2));
            objGauge.SetGaugeMinAmplitude(objFile.GetValueAsInt("MinAmp", 10));
            objGauge.SetGaugeMinArea(objFile.GetValueAsInt("MinArea", 0));

            //set gauge fitting sampling step
            objGauge.SetGaugeSamplingStep(objFile.GetValueAsInt("SamplingStep", 5));


            arrGauge.Add(objGauge);
        }

        private void AddSearchAndUnitROI()
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                // ---------------- Define Search ROI --------------------------
                ROI objPackageROI = new ROI();
                m_smVisionInfo.g_arrOrientROIs[i][0].CopyTo(ref objPackageROI);

                if (i >= m_smVisionInfo.g_arrPackageROIs.Count)
                    m_smVisionInfo.g_arrPackageROIs.Add(new List<ROI>());

                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 0)
                    m_smVisionInfo.g_arrPackageROIs[i].Add(objPackageROI);
                else
                    m_smVisionInfo.g_arrPackageROIs[i][0] = objPackageROI;



                // ---------------- Define Unit ROI --------------------------

                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 1)
                {
                    m_smVisionInfo.g_arrPackageROIs[i].Add(new ROI());

                    // Set half size of search roi to unit Roi as default.
                    m_smVisionInfo.g_arrPackageROIs[i][1].LoadROISetting(m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROIWidth / 4,
                                                                         m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROIHeight / 4,
                                                                         m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROIWidth / 2,
                                                                         m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROIHeight / 2);
                }

                // Attach search ROI to image
                m_smVisionInfo.g_arrPackageROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);

                // Attach unit ROI to search ROI
                m_smVisionInfo.g_arrPackageROIs[i][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[i][0]);
            }
        }

        private void AddTrainROI(int intGap, int intGapRight, int intGapBottom, int intGapLeft) 
        {

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[i][0];
                objSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0)]);//1

                float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.X;
                float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.Y;
                float fWidth = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectWidth;
                float fHeight = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectHeight;
                int intPositionX = (int)Math.Round(fCenterX - (fWidth / 2), 0, MidpointRounding.AwayFromZero) - objSearchROI.ref_ROIPositionX + intGapLeft;
                int intPositionY = (int)Math.Round(fCenterY - (fHeight / 2), 0, MidpointRounding.AwayFromZero) - objSearchROI.ref_ROIPositionY + intGap;

                ROI objROI = new ROI("Package ROI", 2);
                objROI.AttachImage(objSearchROI);
                objROI.LoadROISetting(intPositionX, intPositionY,
                    (int)Math.Round(fWidth - intGapLeft - intGapRight, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(fHeight - intGap - intGapBottom, 0, MidpointRounding.AwayFromZero));
             

                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 2)
                    m_smVisionInfo.g_arrPackageROIs[i].Add(objROI);
                else
                    m_smVisionInfo.g_arrPackageROIs[i][2] = objROI;
            }
        }
        private void AddColorROI(List<List<CROI>> arrROIs)
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (i >= arrROIs.Count)
                    arrROIs.Add(new List<CROI>());

                float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.X;
                float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.Y;
                float fWidth = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectWidth;
                float fHeight = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectHeight;
                int intPositionX = (int)Math.Round(fCenterX - (fWidth / 2), 0, MidpointRounding.AwayFromZero);
                int intPositionY = (int)Math.Round(fCenterY - (fHeight / 2), 0, MidpointRounding.AwayFromZero);

                CROI objROI = new CROI("Color ROI", 1);

                objROI.AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                objROI.LoadROISetting(intPositionX, intPositionY,
                    (int)Math.Round(fWidth, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(fHeight, 0, MidpointRounding.AwayFromZero));

                if (arrROIs[i].Count == 0)
                    arrROIs[i].Add(objROI);
                else
                    arrROIs[i][0] = objROI;
            }
        }
        private void AttachImageToROI(List<List<ROI>> arrROIs, ImageDrawing objImage)
        {
            for (int i = 0; i < arrROIs.Count; i++)
            {
                for (int j = 0; j < arrROIs[i].Count; j++)
                {
                    ROI objROI = arrROIs[i][j];

                    switch(objROI.ref_intType)
                    {
                        case 0:
                            objROI.AttachImage(objImage);
                            break;
                        case 1:
                            objROI.AttachImage(objImage);
                            break;
                        case 2:
                             objROI.AttachImage(arrROIs[i][0]);
                            break;
                        case 3:
                            objROI.AttachImage(arrROIs[i][1]);
                            break;
                    }
                  
                }
            }
        }

        private void LoadGaugeM4LSetting(string strPath, List<RectGaugeM4L> arrGauge)
        {
            // 2019 09 03 - Proper way to dispose unwanted object.
            for (int i = m_smVisionInfo.g_intUnitsOnImage; i < arrGauge.Count; i++)
            {
                arrGauge[i].Dispose();
                arrGauge.RemoveAt(i);
            }

            XmlParser objFile = new XmlParser(strPath);
            RectGaugeM4L objRectGauge;
            //int intCount = Math.Min(m_smVisionInfo.g_intUnitsOnImage, objFile.GetFirstSectionCount());

            for (int j = 0; j < m_smVisionInfo.g_intUnitsOnImage; j++)
            {
                //create new ROI base on file read out
                if (j >= arrGauge.Count)
                {
                    //add Rect Gauge into shared memory's Rect Gauge array list 
                    objRectGauge = new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex);
                    arrGauge.Add(objRectGauge);
                }

                arrGauge[j].LoadRectGauge4L(strPath, "RectG" + j);
            }

            objRectGauge = null;

        }

        private void LoadPackageSettings(string strPath)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].LoadPackage(strPath + "Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                else
                {
                    if (File.Exists(strPath + "Settings2.xml"))
                        m_smVisionInfo.g_arrPackage[u].LoadPackage(strPath + "Settings2.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                    else
                        m_smVisionInfo.g_arrPackage[u].LoadPackage(strPath + "Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                }

                m_smVisionInfo.g_arrPackage[u].SetBlobSettings(m_smVisionInfo.g_arrPackage[u].ref_intPkgViewThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intPkgViewMinArea,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewHighThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewLowThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewMinArea);
            }
        }

        private void LoadROISetting(string strPath, List<List<ROI>> arrROIList)
        {
            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            ROI objROI;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                arrROIList.Add(new List<ROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new ROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
                    objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    if (objROI.ref_intType > 1)
                    {
                        objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                        objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    }

                    arrROIList[i].Add(objROI);
                }
            }
        }
        private void LoadDontCareROISetting(string strPath, List<List<ROI>> arrROIList)
        {
            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            ROI objROI;
            int intCount = 0;
            intCount = objFile.GetFirstSectionCount();
            for (int i = 0; i < intCount; i++)
            {
                arrROIList.Add(new List<ROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new ROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
                    objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    if (objROI.ref_intType > 1)
                    {
                        objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                        objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    }

                    arrROIList[i].Add(objROI);
                }
            }
        }
        private void LoadROISetting(string strPath, List<List<List<ROI>>> arrROIs, int intROICount)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            ROI objROI;

            for (int i = 0; i < intROICount; i++)
            {
                arrROIs.Add(new List<List<ROI>>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    arrROIs[i].Add(new List<ROI>());
                    objFile.GetSecondSection("Color Defect" + i + j);
                    int intThirdChildCount = objFile.GetThirdSectionCount();
                    for (int k = 0; k < intThirdChildCount; k++)
                    {
                        objROI = new ROI();
                        objFile.GetThirdSection("ROI" + k);
                        objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 3);
                        objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 3);
                        objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 3);
                        objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 3);
                        objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 3);
                        objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 3);
                        objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 3);
                        objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 3);
                        objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 3));
                        if (objROI.ref_intType > 1)
                        {
                            objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                            objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                        }

                        arrROIs[i][j].Add(objROI);
                    }
                }
            }
        }
        private void SaveGaugeSettings()
        {
            for (int j = 0; j < m_smVisionInfo.g_arrPackageGaugeM4L.Count; j++)
            {
                
                STDeviceEdit.CopySettingFile(m_strFolderPath, "Gauge.xml");

                RectGaugeM4L objRectGauge = m_smVisionInfo.g_arrPackageGaugeM4L[j];
                objRectGauge.SaveRectGauge4L(m_strFolderPath + "GaugeM4L.xml", false, "RectG" + j, true, false);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Package Gauge", m_smProductionInfo.g_strLotID);
                
                //objRectGauge.SetRectGaugeTemplate(objRectGauge.ref_pRectCenterPoint.X, objRectGauge.ref_pRectCenterPoint.Y,
                //    objRectGauge.ref_fRectWidth, objRectGauge.ref_fRectHeight);
            }

            for (int j = 0; j < m_smVisionInfo.g_arrPackageGauge2M4L.Count; j++)
            {
               
                RectGaugeM4L objRectGauge = m_smVisionInfo.g_arrPackageGauge2M4L[j];
                objRectGauge.SaveRectGauge4L(m_strFolderPath + "Gauge2M4L.xml", false, "RectG" + j, true, false);
                //objRectGauge.SetRectGaugeTemplate(objRectGauge.ref_pRectCenterPoint.X, objRectGauge.ref_pRectCenterPoint.Y,
                //    objRectGauge.ref_fRectWidth, objRectGauge.ref_fRectHeight);
            }
        }

        private void SavePackageSettings()
        {
            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "Settings.xml");

            //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SavePackage(m_strFolderPath + "Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
            
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].SavePackage(m_strFolderPath + "Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                else
                    m_smVisionInfo.g_arrPackage[u].SavePackage(m_strFolderPath + "Settings2.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);

                m_smVisionInfo.g_arrPackage[u].SetBlobSettings(m_smVisionInfo.g_arrPackage[u].ref_intPkgViewThreshold, 
                                                               m_smVisionInfo.g_arrPackage[u].ref_intPkgViewMinArea,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewHighThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewLowThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewMinArea);
            }
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Package", m_smProductionInfo.g_strLotID);
            
        }

        private void SaveDontCareSetting(string strFolderPath)
        {
            ImageDrawing objImage = new ImageDrawing();
            ImageDrawing objFinalImage = new ImageDrawing();
            //for (int j = 0; j < 2; j++)
            //{
            //    m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedTemplate][j].CheckDontCarePosition(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
            //    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedTemplate][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
            //    objImage.SaveImage(strFolderPath + "DontCareImage"+ m_smVisionInfo.g_intSelectedTemplate.ToString() + "_" + j.ToString() + ".bmp");
            //}

            //if (chk_BrightAndDarkSameDontCareArea.Checked)
            //{
            //    if (m_smVisionInfo.g_intSelectedType == 0)
            //    {
            //        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1].Clear();
            //        if (m_smVisionInfo.g_arrPackageDontCareROIs.Count > 1)
            //            m_smVisionInfo.g_arrPackageDontCareROIs[1].Clear();
            //    }
            //    else
            //    {
            //        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0].Clear();
            //        if (m_smVisionInfo.g_arrPackageDontCareROIs.Count > 0)
            //            m_smVisionInfo.g_arrPackageDontCareROIs[0].Clear();
            //    }

            //    if (m_smVisionInfo.g_arrPackageDontCareROIs[0].Count == 0)
            //    {
            //        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0].Clear();
            //    }
            //    if (m_smVisionInfo.g_arrPackageDontCareROIs[1].Count == 0)
            //    {
            //        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1].Clear();
            //    }
            //}

            for (int i = 0; i < m_smVisionInfo.g_arrPackageDontCareROIs.Count; i++)
            {
                //if(m_smVisionInfo.g_intSelectedType == i)
                //{
                for (int j = 0; j < m_smVisionInfo.g_arrPackageDontCareROIs[i].Count; j++)
                {
                    if (m_smVisionInfo.g_arrPackageDontCareROIs[i].Count > j)
                    {
                        //if (chk_BrightAndDarkSameDontCareArea.Checked)
                        //{
                        //    if (m_smVisionInfo.g_intSelectedType == 0)
                        //    {
                        //        if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1].Count <= j)
                        //            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1].Add(new Polygon());
                        //    }
                        //    else
                        //    {
                        //        if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0].Count <= j)
                        //            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0].Add(new Polygon());
                        //    }
                        //}

                        if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].ref_intFormMode != 2)
                        {
                            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].AddPoint(new PointF(m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                                m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].AddPoint(new PointF((m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROITotalX + m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                                (m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROITotalY + m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].AddPolygon((int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].ResetPointsUsingOffset(m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].AddPolygon((int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                        }
                    }


                }
                //}
            }

            if (chk_BrightAndDarkSameDontCareArea.Checked)
            {
                if (m_smVisionInfo.g_intSelectedType == 0)
                {
                    if (0 < m_smVisionInfo.g_arrPolygon_Package.Count && 
                        1 < m_smVisionInfo.g_arrPolygon_Package[0].Count && 
                        m_smVisionInfo.g_intSelectedType < m_smVisionInfo.g_arrPolygon_Package[0].Count)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Package[0][1].Count; i++)
                        {

                            //m_smVisionInfo.g_arrPolygon_Package[0][1][i].ClearPolygon();
                            m_smVisionInfo.g_arrPolygon_Package[0][m_smVisionInfo.g_intSelectedType][i].CopyAllTo(m_smVisionInfo.g_arrPolygon_Package[0][1][i]);
                            //m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1][i].AddPolygon((int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                        }
                    }
                }
                else
                {
                    if (0 < m_smVisionInfo.g_arrPolygon_Package.Count && 
                        0 < m_smVisionInfo.g_arrPolygon_Package[0].Count &&
                        m_smVisionInfo.g_intSelectedType < m_smVisionInfo.g_arrPolygon_Package[0].Count)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Package[0][0].Count; i++)
                        {
                            //m_smVisionInfo.g_arrPolygon_Package[0][0][i].ClearPolygon();
                            m_smVisionInfo.g_arrPolygon_Package[0][m_smVisionInfo.g_intSelectedType][i].CopyAllTo(m_smVisionInfo.g_arrPolygon_Package[0][0][i]);
                            //m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0][i].AddPolygon((int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                        }
                    }
                }

            }

            for (int i = 0; i < 2; i++)
            {
                m_smVisionInfo.g_objBlackImage.CopyTo(objFinalImage);
                if (m_smVisionInfo.g_intSelectedUnit < m_smVisionInfo.g_arrPolygon_Package.Count && i < m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i].Count; j++)
                    {
                        //m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].CheckDontCarePosition(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        //Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);

                        //if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].ref_intFormMode != 2)
                        //    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        //else
                        //{
                        //    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                        //}

                        // 2020 06 27 - CCENG: change g_arrPackageROIs 
                        if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j].ref_intFormMode != 2)
                            Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        else
                        {
                            Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                        }

                        ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);
                    }
                }
                //objFinalImage.SaveImage(strFolderPath + "DontCareImage" + m_smVisionInfo.g_intSelectedTemplate.ToString() + "_" + i.ToString() + ".bmp");
                objFinalImage.SaveImage(strFolderPath + "DontCareImage0_" + i.ToString() + ".bmp");
            }
            objImage.Dispose();
            objFinalImage.Dispose();
            if (File.Exists(strFolderPath + "DontCareImage0_0.bmp"))
            {
                m_smVisionInfo.g_objDontCareImage_Package_Bright.LoadImage(strFolderPath + "DontCareImage0_0.bmp");
            }
            if (File.Exists(strFolderPath + "DontCareImage0_1.bmp"))
            {
                m_smVisionInfo.g_objDontCareImage_Package_Dark.LoadImage(strFolderPath + "DontCareImage0_1.bmp");
            }

            Polygon.SavePolygon(strFolderPath + "Polygon.xml", m_smVisionInfo.g_arrPolygon_Package);
        }

        private void chk_SetToAll_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_Package", chk_SetToAll.Checked);

            CheckPkgROISetting();
        }
        private void chk_SetToAll_DarkField2_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_DarkField2", chk_SetToAll_DarkField2.Checked);

            CheckPkgDarkROISetting();
        }
        private void SaveROISettings()
        {
            ROI objSearchROI = new ROI();
            ROI objTrainROI = new ROI();
            ROI objROI;

            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");

            XmlParser objFile = new XmlParser(m_strFolderPath + "ROI.xml", true);
        
            ROI objSelectedROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];
            for (int x = 0; x < m_smVisionInfo.g_arrPackageROIs.Count; x++)
            {
                objFile.WriteSectionElement("Unit" + x);
                for (int j = 0; j < m_smVisionInfo.g_arrPackageROIs[x].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrPackageROIs[x][j];
                    if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                    {
                        objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
                        objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
                        objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
                        objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
                    }

                    objFile.WriteElement1Value("ROI" + j, "");

                    if (j > 1)
                        objROI.ref_strROIName = Convert.ToString(j - 1);
                    objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                    objFile.WriteElement2Value("Type", objROI.ref_intType);
                    objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                    objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);

                    switch (objROI.ref_intType)
                    {
                        case 1:
                            m_smVisionInfo.g_arrPackageROIs[x][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                            break;
                        case 2:
                            m_smVisionInfo.g_arrPackageROIs[x][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[x][0]);
                            break;
                    }

                    float fPixelAverage = objROI.GetROIAreaPixel();
                    objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                    objROI.SetROIPixelAverage(fPixelAverage);

                    objFile.WriteEndElement();
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Package ROI", m_smProductionInfo.g_strLotID);
                    
                }
            }
        }
        private void SaveDontCareROISettings()
        {
            ROI objSearchROI = new ROI();
            ROI objTrainROI = new ROI();
            ROI objROI;

            //
            //STDeviceEdit.CopySettingFile(m_strFolderPath, "DontCareROI.xml");

            XmlParser objFile = new XmlParser(m_strFolderPath + "DontCareROI.xml", true);
            
            for (int i = 0; i < m_smVisionInfo.g_arrPackageDontCareROIs.Count; i++)
            {
                objFile.WriteSectionElement("Unit" + i);
                for (int j = 0; j < m_smVisionInfo.g_arrPackageDontCareROIs[i].Count; j++)
                {
                    //objROI = m_smVisionInfo.g_arrPackageDontCareROIs[i][j];
                    objFile.WriteElement1Value("ROI" + j, "");

                    if (j > 1)
                        m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_strROIName = Convert.ToString(j - 1);
                    objFile.WriteElement2Value("Name", m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_strROIName);
                    objFile.WriteElement2Value("Type", m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_intType);
                    objFile.WriteElement2Value("PositionX", m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROIWidth);
                    objFile.WriteElement2Value("Height", m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", m_smVisionInfo.g_arrPackageDontCareROIs[i][j].ref_intStartOffsetY);

                    m_smVisionInfo.g_arrPackageDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);


                    float fPixelAverage = m_smVisionInfo.g_arrPackageDontCareROIs[i][j].GetROIAreaPixel();
                    objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                    m_smVisionInfo.g_arrPackageDontCareROIs[i][j].SetROIPixelAverage(fPixelAverage);

                    objFile.WriteEndElement();
                    //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Package ROI", m_strFolderPath, "ROI.xml", m_smProductionInfo.g_strLotID);                   
                }
            }
        }
        private void SetupSteps()
        {
            /*
                Detail Threshold
                ----------------
                0 - package edge
                1 - package roi
                2 - Image 1/2 Mark view bright field
                3 - Image 1/2 Package View bright field
                4 - Image 3 Crack View dark field
                5 - chip roi 
                6 - chip bright field
                7 - chip dark field
                8 - void dark field
                9 - mold flash roi
                10- mold flash bright field

                Simple Threshold
                -----------------
                0 - package edge
                1 - package roi
                2 - Image 1 mark view bright field
                3 - Image 1/2 package view bright field
                4 - Image 3 carck view dark field
                5 - chip roi 
                9 - mold flash roi
            */

            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0:
                    //m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0), 0);  // View mark image (first image)
                    //m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);  // view second image
                    //m_smVisionInfo.g_intSelectedImage = Math.Min(m_smVisionInfo.g_arrImages.Count - 1, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0));  // View Package1 image. g_intSelectedImage should get at least value 1 represent package1 image

                    DefineMostSelectedImageNo(); // 2020-02-17 ZJYEOH : Display the image selected from gauge advance setting

                    // PR Unit Pattern Matching
                    if (m_smVisionInfo.g_blnWantUseUnitPRFindGauge)
                    {
                        int intUnitNo = 0;
                        // 2020 02 25 - CCENG: Attach Package group Search ROI to main image
                        m_smVisionInfo.g_arrPackageROIs[intUnitNo][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                        m_fUnitPRResultCenterX = 0;
                        m_fUnitPRResultCenterY = 0;

                        if (m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrPackageROIs[intUnitNo][0]))
                        {
                            m_fUnitPRResultCenterX = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX() +
                                                   m_smVisionInfo.g_arrPackageROIs[intUnitNo][0].ref_ROIPositionX;
                            m_fUnitPRResultCenterY = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY() +
                                                   m_smVisionInfo.g_arrPackageROIs[intUnitNo][0].ref_ROIPositionY;
                            int intUnitPRWidth = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRWidth();
                            int intUnitPRHeight = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRHeight();

                            if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > intUnitNo)
                                m_smVisionInfo.g_arrPackageGaugeM4L[intUnitNo].SetEdgeROIPlacementLimit3(m_smVisionInfo.g_arrPackageROIs[intUnitNo][0], (int)m_fUnitPRResultCenterX, (int)m_fUnitPRResultCenterY, intUnitPRWidth, intUnitPRHeight);

                            if (m_smVisionInfo.g_arrPackageGauge2M4L.Count > intUnitNo)
                                m_smVisionInfo.g_arrPackageGauge2M4L[intUnitNo].SetEdgeROIPlacementLimit3(m_smVisionInfo.g_arrPackageROIs[intUnitNo][0], (int)m_fUnitPRResultCenterX, (int)m_fUnitPRResultCenterY, intUnitPRWidth, intUnitPRHeight);

                        }
                    }

                    bool blnGaugeResult = true;

                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intUseOtherGaugeMeasurePackage > 0)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
                        AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_objPackageImage);

                        //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                        //m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objModifiedPackageImage);

                        m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                        //m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                        m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugePlace_BasedOnEdgeROI();
                        blnGaugeResult = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                    }
                    else
                    {
                        // 2020 01 10 - No longer using ref_fGainValue (1 gain for all edge). Change to different gain differnt edge.
                        //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage); 
                        AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_objPackageImage);

                        if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)   // Lead unit
                        {
                            if (!FindUnitCenterPoint())
                            {
                                SRMMessageBox.Show("Fail to unit surface position using Unit PR. Please relearn PR using Unit PR using Learn Mark Wizard.");
                                btn_Next.Enabled = false;
                            }
                            // Attach to unit ROI
                            //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                            //m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_objModifiedPackageImage);

                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                            //m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugePlace_BasedOnEdgeROI();
                            blnGaugeResult = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_objWhiteImage);
                            m_smVisionInfo.g_blnViewUnitROI = false; // 2019-10-14 ZJYEOH : set to false as it makes edge ROI cannot be dragged, initially is true
                        }
                        else // QFN (no lead)
                        {
                            // Modify gauge size same as ROI size
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                            {
                                if (i < m_smVisionInfo.g_arrPackageGaugeM4L.Count)
                                {
                                    //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                                    //m_smVisionInfo.g_arrPackageROIs[i][0].AttachImage(m_smVisionInfo.g_objModifiedPackageImage);

                                    m_smVisionInfo.g_arrPackageGaugeM4L[i].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                                    //m_smVisionInfo.g_arrPackageGaugeM4L[i].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                                    m_smVisionInfo.g_arrPackageGaugeM4L[i].SetGaugePlace_BasedOnEdgeROI();
                                    blnGaugeResult = m_smVisionInfo.g_arrPackageGaugeM4L[i].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[i][0], m_smVisionInfo.g_objWhiteImage);
                                }
                            }
                            m_smVisionInfo.g_blnViewUnitROI = false;
                        }
                    }
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewRotatedImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    m_smVisionInfo.g_blnDragROI = true;

                    lbl_TitleStep1.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step1;
                    btn_Previous.Enabled = false;
                    if (m_intLearnType == 1 || (m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || m_smVisionInfo.g_strVisionName.Contains("BottomOrient")) && (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (!blnGaugeResult)
                        {
                            m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }
                        btn_Next.Enabled = false;
                        tp_Step1.Controls.Add(btn_Save);
                    }

                    if (m_intVisionType != 0)
                    {
                        m_blnGaugeResult = blnGaugeResult;
                        if (!blnGaugeResult)
                        {
                            m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }
                        m_smVisionInfo.g_intLearnStepNo = 22;
                        SetupSteps();
                    }
                    break;
                case 1:
                    //m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0), 0);  // View mark image (first image)
                    //m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);  // view second image
                    //m_smVisionInfo.g_intSelectedImage = Math.Min(m_smVisionInfo.g_arrImages.Count - 1, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3));  // View Package1 image. g_intSelectedImage should get at least value 1 represent package1 image

                    DefineMostSelectedImageNo(); // 2020-02-17 ZJYEOH : Display the image selected from gauge advance setting
                    bool blnGaugeResult2 = true;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intUseOtherGaugeMeasurePackage > 0)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
                        AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_objPackageImage);

                        //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                        //m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objModifiedPackageImage);

                        m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                        //m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                        m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugePlace_BasedOnEdgeROI();
                        blnGaugeResult2 = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                    }
                    else
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
                        AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_objPackageImage);

                        if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)   // Lead unit
                        {
                            if (!FindUnitCenterPoint())
                            {
                                SRMMessageBox.Show("Fail to unit surface position using Unit PR. Please relearn PR using Unit PR using Learn Mark Wizard.");
                                btn_Next.Enabled = false;
                            }
                            // Attach to unit ROI
                            //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                            //m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_objModifiedPackageImage);

                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                            //m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugePlace_BasedOnEdgeROI();
                            blnGaugeResult2 = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_objWhiteImage);
                            m_smVisionInfo.g_blnViewUnitROI = false; // 2019-10-14 ZJYEOH : set to false as it makes edge ROI cannot be dragged, initially is true
                        }
                        else // QFN (no lead)
                        {
                            // Modify gauge size same as ROI size
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                            {
                                if (i < m_smVisionInfo.g_arrPackageGauge2M4L.Count)
                                {
                                    //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                                    //m_smVisionInfo.g_arrPackageROIs[i][0].AttachImage(m_smVisionInfo.g_objModifiedPackageImage);

                                    m_smVisionInfo.g_arrPackageGauge2M4L[i].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                                    //m_smVisionInfo.g_arrPackageGauge2M4L[i].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                                    m_smVisionInfo.g_arrPackageGauge2M4L[i].SetGaugePlace_BasedOnEdgeROI();
                                    blnGaugeResult2 = m_smVisionInfo.g_arrPackageGauge2M4L[i].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[i][0], m_smVisionInfo.g_objWhiteImage);
                                }
                            }
                            m_smVisionInfo.g_blnViewUnitROI = false;
                        }
                    }
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewRotatedImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    m_smVisionInfo.g_blnDragROI = true;

                    lbl_TitleStep1_1.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step1_1;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 2)
                    {
                        if (!blnGaugeResult2)
                        {
                            m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                        tp_Step1_1.Controls.Add(btn_Save);
                    }
                    break;
                case 2:
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewRotatedPackageImage = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    
                    // PR Unit Pattern Matching
                    if (m_smVisionInfo.g_blnWantUseUnitPRFindGauge)
                    {
                        // record Edge ROI offset to PR Unit
                        int intUnitNo = 0;
                        if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrPackageGaugeM4L[intUnitNo].SetEdgeROIOffset((int)m_fUnitPRResultCenterX, (int)m_fUnitPRResultCenterY);

                        if (m_smVisionInfo.g_arrPackageGauge2M4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrPackageGauge2M4L[intUnitNo].SetEdgeROIOffset((int)m_fUnitPRResultCenterX, (int)m_fUnitPRResultCenterY);
                    }

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);

                    //cxlim 2020/12/15  : nid to measure center point first or else user will get a small package roi 
                    if (m_intLearnType == 3)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_objPackageImage);

                        if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)   // Lead unit
                        {
                            if (!FindUnitCenterPoint())
                            {
                                SRMMessageBox.Show("Fail to unit surface position using Unit PR. Please relearn PR using Unit PR using Learn Mark Wizard.");
                                return;
                            }
                            m_smVisionInfo.g_arrPackageGaugeM4L[0].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                            m_smVisionInfo.g_arrPackageGaugeM4L[0].SetGaugePlace_BasedOnEdgeROI();
                            blnGaugeResult = m_smVisionInfo.g_arrPackageGaugeM4L[0].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[0][1], m_smVisionInfo.g_objWhiteImage);
                        }
                        else
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                            {
                                if (i < m_smVisionInfo.g_arrPackageGaugeM4L.Count)
                                {
                                    m_smVisionInfo.g_arrPackageGaugeM4L[i].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                                    m_smVisionInfo.g_arrPackageGaugeM4L[i].SetGaugePlace_BasedOnEdgeROI();
                                    blnGaugeResult = m_smVisionInfo.g_arrPackageGaugeM4L[i].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[i][0], m_smVisionInfo.g_objWhiteImage);
                                }
                            }
                        }

                    }

                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                        {
                            ROI objRotateROI = new ROI();
                            objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                            objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], objRotateROI,
                                                      m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                                                      ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);

                            m_smVisionInfo.g_arrRotatedImages[intImageIndex].CopyTo(m_arrRotatedImage[intImageIndex]);
                        }
                    }
                    else
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                        {
                            m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                            m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_arrRotatedImage[intImageIndex]);
                        }
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                            {
                                CROI objRotateROI = new CROI();
                                objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                                objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                               (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                                CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[intImageIndex], objRotateROI,
                                                          m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 8,
                                                          ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);

                                m_smVisionInfo.g_arrColorRotatedImages[intImageIndex].CopyTo(m_arrColorRotatedImage[intImageIndex]);
                            }
                        }
                        else
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                            {
                                m_smVisionInfo.g_arrColorImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                                m_smVisionInfo.g_arrColorImages[intImageIndex].CopyTo(m_arrColorRotatedImage[intImageIndex]);
                            }
                        }
                    }

                    //After measure gauge and rotate image, set UnitROI (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]) around the unit to learn unit pattern for PR before gauge
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].LoadROISetting((int)Math.Round((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX, 0, MidpointRounding.AwayFromZero),
                                                 (int)Math.Round((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY, 0, MidpointRounding.AwayFromZero),
                                                 (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth, 0, MidpointRounding.AwayFromZero),
                                                 (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight, 0, MidpointRounding.AwayFromZero));

                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    AddTrainROI((int)Math.Round(Convert.ToSingle(txt_StartPixelFromEdge.Text) , 0, MidpointRounding.AwayFromZero),
                               (int)Math.Round(Convert.ToSingle(txt_StartPixelFromRight.Text) , 0, MidpointRounding.AwayFromZero),
                               (int)Math.Round(Convert.ToSingle(txt_StartPixelFromBottom.Text), 0, MidpointRounding.AwayFromZero),
                                (int)Math.Round(Convert.ToSingle(txt_StartPixelFromLeft.Text) , 0, MidpointRounding.AwayFromZero));
                    lbl_TitleStep2.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step2;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 3)
                    {
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                        tp_Step2.Controls.Add(btn_Save);
                    }
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    break;
                case 3:
                    // Bright Fiedl
                    lbl_BrightThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intBrightFieldLowThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
                 m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
                 m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
                 m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);

                    lbl_TitleBrightFieldSimple.BringToFront();
                    tabControl_Main.SelectedTab = tp_BrightField;
                    btn_Next.Enabled = true;
                    break;
                case 4:
                    // Dark View
                    lbl_DarkLowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldLowThreshold.ToString();
                    lbl_DarkHighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldHighThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateBrightDarkROITolerance)
                    {
                        AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
                     m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
                     m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
                     m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark);
                    }
                    else
                    {
                        AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
                     m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
                     m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
                     m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);
                    }

                    lbl_TitleDarkFieldSimple.BringToFront();
                    tabControl_Main.SelectedTab = tp_DarkField;

                    if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting &&
                        !m_smVisionInfo.g_blnWantDontCareArea_Package
                        )
                    {
                        tp_DarkField.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    else
                        btn_Next.Enabled = true;
                    break;
                case 5:
                    // Mark View
                    lbl_LowMarkViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewLowThreshold.ToString();
                    lbl_HighMarkViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewHighThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewRotatedImage = false;
                    m_smVisionInfo.g_blnViewRotatedPackageImage = true;
                    m_smVisionInfo.g_blnViewPkgProcessImage = true;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    //m_smVisionInfo.g_blnPackageInspected = true;   // 2019 01 04 - not need to draw because not neccesary.
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(1), 0);

                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);


                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildMarkViewObjects(
                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

                    lbl_TitleStep3.BringToFront();
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
                    {
                        lbl_TitleStep3.Text = lbl_Img1BrightFieldDecs.Text;

                    }
                    tabControl_Main.SelectedTab = tp_Step3;
                    btn_Next.Enabled = true;
                    break;
                case 6:
                    // Package View
                    lbl_PackageViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intPkgViewThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    //m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    
                    //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndexCount() < 3)
                    //{
                    //    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetGrabImageIndex(2, 1);
                    //}

                    //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetGrabImageIndex(2, 1);

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildPackageViewObjects(
                    //    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

                    lbl_TitleStep4.BringToFront();
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
                    {
                        lbl_TitleStep4.Text = lbl_Image2_BrightField.Text;
                    }
                    tabControl_Main.SelectedTab = tp_Step4;
                    break;
                case 7:
                    // Crack View
                    lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewLowThreshold.ToString();
                    lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewHighThreshold.ToString();
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    //m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndexCount() < 4)
                    //{
                    //    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetGrabImageIndex(3, 2);
                    //}

                    //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetGrabImageIndex(3, 2);

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else 
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildPackageViewObjects(
                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

                    lbl_TitleStep5.BringToFront();
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    {
                        lbl_TitleStep5.Text = lbl_Image3_DarkField_Void.Text;
                    }
                    tabControl_Main.SelectedTab = tp_Step5;
                    btn_Next.Enabled = true;

                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting &&
                        !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting &&
                        !m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        tp_Step5.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    break;
                case 8:
                    // Chip View ROI
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = true;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewRotatedPackageImage = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnPackageInspected = false;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    cbo_ChippedOffDefectType.SelectedIndex = 0;
                    lbl_TitleStep6.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step6;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 4)
                    {
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                        tp_Step6.Controls.Add(btn_Save);
                    }
                    break;
                case 9:
                    // Chip View Image 2
                    lbl_ChipViewImage2Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView1Threshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = true;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    //m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    lbl_TitleStep6.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step7;
                    btn_Next.Enabled = true;
                    break;
                case 10:
                    // Chip View Image 3
                    lbl_ChipViewImage3Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView2Threshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = true;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    //m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildPackageViewObjects(
                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

                    lbl_TitleStep7.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step8;
                    btn_Next.Enabled = true;
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package &&
                       !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting &&
                       !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting &&
                       !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting &&
                       !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting &&
                       !m_smVisionInfo.g_blnWantDontCareArea_Package
                       )
                    {
                        tp_Step8.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    break;
                case 11:
                    // Void View Image 3
                    lbl_VoidViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;
                    //m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                   
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildPackageViewObjects(
                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                    
                    lbl_TitleStep8.BringToFront();
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package )//&& !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateVoidDefectSetting)
                    {
                        lbl_TitleStep8.Text = lbl_Image3_DarkField_Void.Text;
                    }
                    tabControl_Main.SelectedTab = tp_Step9;
                    if (!tp_Step9.Controls.Contains(btn_Save))
                        btn_Next.Enabled = true;
                    break;
                case 12: // Mold Flash Tolerance Setting
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewRotatedPackageImage = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    
                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                        {
                            ROI objRotateROI = new ROI();
                            objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                            objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], objRotateROI,
                                                      m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                                                      ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);
                        }
                    }
                    else
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                            m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                            {
                                CROI objRotateROI = new CROI();
                                objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                                objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                               (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                                CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[intImageIndex], objRotateROI,
                                                          m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 8,
                                                          ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);
                            }
                        }
                        else
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                                m_smVisionInfo.g_arrColorImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                        }
                    }

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5), 0);
                    lbl_TitleStep9.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step10;
                    btn_Next.Enabled = true;
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting &&
                        !m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        tp_Step10.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    if (m_intLearnType == 5)
                    {
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                        tp_Step10.Controls.Add(btn_Save);
                    }
                    break;
                case 13:
                    lbl_MoldFlashThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    //m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5), 0);  //2020-12-18 ZJYEOH: Mold Flash have own image selection // 2019 06 14 - CCENG: Mold flash is under side light package image which is in index 2
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);


                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildPackageViewObjects(
                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

                    lbl_TitleStep9.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step11;
                    btn_Next.Enabled = true;
                    if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting &&
                        !m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        tp_Step11.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    break;
                case 14:
                    // Dark 2 ROI
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewRotatedPackageImage = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                        {
                            ROI objRotateROI = new ROI();
                            objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                            objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], objRotateROI,
                                                      m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                                                      ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);
                        }
                    }
                    else
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                            m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                            {
                                CROI objRotateROI = new CROI();
                                objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                                objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                               (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                                CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[intImageIndex], objRotateROI,
                                                          m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 8,
                                                          ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);
                            }
                        }
                        else
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                                m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                        }
                    }

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(4), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    lbl_TitleDarkField2.BringToFront();
                    lbl_TitleDarkField2.Text = "Dark 2 ROI";
                    tabControl_Main.SelectedTab = tp_Dark2ROI;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 7)
                    {
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                        tp_Dark2ROI.Controls.Add(btn_Save);
                    }
                    break;
                case 15:
                    // Dark 2
                    lbl_Dark2LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2LowThreshold.ToString();
                    lbl_Dark2HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2HighThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(4), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    lbl_TitleDarkField2.BringToFront();
                    lbl_TitleDarkField2.Text = "Dark 2 Defect";
                    tabControl_Main.SelectedTab = tp_DarkField2;

                    if (!m_smVisionInfo.g_blnWantDontCareArea_Package && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        tp_DarkField2.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    else
                        btn_Next.Enabled = true;
                    break;
                case 16:
                    // Dark 3 ROI
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewRotatedPackageImage = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;

                    txt_StartPixelFromEdge_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3.ToString();
                    txt_StartPixelFromRight_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3.ToString();
                    txt_StartPixelFromBottom_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3.ToString();
                    txt_StartPixelFromLeft_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3.ToString();

                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                        {
                            ROI objRotateROI = new ROI();
                            objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                            objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], objRotateROI,
                                                      m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                                                      ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);
                        }
                    }
                    else
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                            m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                            {
                                CROI objRotateROI = new CROI();
                                objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                                objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                               (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                                CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[intImageIndex], objRotateROI,
                                                          m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 8,
                                                          ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);
                            }
                        }
                        else
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                                m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                        }
                    }

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(6), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    lbl_TitleDarkField3.BringToFront();
                    lbl_TitleDarkField3.Text = "Dark 3 ROI";
                    tabControl_Main.SelectedTab = tp_Dark3ROI;
                    btn_Next.Enabled = true;
                    break;
                case 17:
                    // Dark 3
                    lbl_Dark3LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3LowThreshold.ToString();
                    lbl_Dark3HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3HighThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(6), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    lbl_TitleDarkField3.BringToFront();
                    lbl_TitleDarkField3.Text = "Dark 3 Defect";
                    tabControl_Main.SelectedTab = tp_DarkField3;

                    if (!m_smVisionInfo.g_blnWantDontCareArea_Package && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        tp_DarkField3.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    else
                        btn_Next.Enabled = true;
                    break;
                case 18:
                    // Dark 4 ROI
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewRotatedPackageImage = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;

                    txt_StartPixelFromEdge_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4.ToString();
                    txt_StartPixelFromRight_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4.ToString();
                    txt_StartPixelFromBottom_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4.ToString();
                    txt_StartPixelFromLeft_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4.ToString();

                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                        {
                            ROI objRotateROI = new ROI();
                            objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                            objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], objRotateROI,
                                                      m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                                                      ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);
                        }
                    }
                    else
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                            m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                            {
                                CROI objRotateROI = new CROI();
                                objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                                objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                               (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                                CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[intImageIndex], objRotateROI,
                                                          m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 8,
                                                          ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);
                            }
                        }
                        else
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                                m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                        }
                    }

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(7), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    lbl_TitleDarkField4.BringToFront();
                    lbl_TitleDarkField4.Text = "Dark 4 ROI";
                    tabControl_Main.SelectedTab = tp_Dark4ROI;
                    btn_Next.Enabled = true;
                    break;
                case 19:
                    // Dark 4
                    lbl_Dark4LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4LowThreshold.ToString();
                    lbl_Dark4HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4HighThreshold.ToString();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnViewPackageMaskROI = false;
                    m_smVisionInfo.g_blnViewPkgProcessImage = false;
                    m_smVisionInfo.g_blnPackageInspected = false;
                    m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(7), 0);
                    if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                    lbl_TitleDarkField4.BringToFront();
                    lbl_TitleDarkField4.Text = "Dark 4 Defect";
                    tabControl_Main.SelectedTab = tp_DarkField4;

                    if (!m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        tp_DarkField4.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    else
                        btn_Next.Enabled = true;
                    break;
                case 20: //Define Don Care Area't

                    if (((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && (!m_smVisionInfo.g_strVisionName.Contains("InPocket") && (!m_smVisionInfo.g_strVisionName.Contains("IPM"))))
                    {
                        if (m_smVisionInfo.g_blnWantPin1 &&
                            m_smVisionInfo.g_WantUsePin1OrientationWhenNoMark &&
                                 ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0) &&
                                  m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate))
                        {
                            StartPin1Test_WithOrientation();
                        }
                        else
                            StartOrientTest();
                    }

                    for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Package.Count; u++)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Package[u].Count; i++)
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrPolygon_Package[u][i].Count; j++)
                            {
                                m_smVisionInfo.g_arrPolygon_Package[u][i][j].ClearPolygon();
                            }
                        }
                    }
                    //m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedTemplate][m_smVisionInfo.g_intSelectedType].CheckDontCarePosition(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);

                    //m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
                    if (radio_Bright.Checked)
                    {
                        m_smVisionInfo.g_intSelectedType = 0;
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2);
                    }
                    else
                    {
                        m_smVisionInfo.g_intSelectedType = 1;
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);
                    }

                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;
                    lbl_TitleStep10.BringToFront();
                    tabControl_Main.SelectedTab = tp_Step12;
                    tp_Step12.Controls.Add(btn_Save);
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        if (tp_Step12.Controls.Contains(btn_Save))
                            tp_Step12.Controls.Remove(btn_Save);
                        btn_Next.Enabled = true;
                    }
                    else
                        btn_Next.Enabled = false;
                    if (m_intLearnType == 6)
                    {
                        btn_Previous.Enabled = false;
                    }
                    break;
                case 21: //Define Mold Flash Don Care Area't
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5);
                    for (int i = 0; i < m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.Count; i++)
                    {
                        m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                    }
                    PlaceMoldFlashDontCareROI();
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;
                    lbl_TitleStep10.BringToFront();
                    tabControl_Main.SelectedTab = tp_MoldFlashDontCare;
                    tp_MoldFlashDontCare.Controls.Add(btn_Save);
                    btn_Next.Enabled = false;
                    break;
                case 22: // Color
                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                        {
                            ROI objRotateROI = new ROI();
                            objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                            objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], objRotateROI,
                                                      m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                                                      ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);

                            m_smVisionInfo.g_arrRotatedImages[intImageIndex].CopyTo(m_arrRotatedImage[intImageIndex]);
                        }
                    }
                    else
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                        {
                            m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                            m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_arrRotatedImage[intImageIndex]);
                        }
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                            {
                                CROI objRotateROI = new CROI();
                                objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                                objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                               (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                               m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                                CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[intImageIndex], objRotateROI,
                                                          m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 8,
                                                          ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);

                                m_smVisionInfo.g_arrColorRotatedImages[intImageIndex].CopyTo(m_arrColorRotatedImage[intImageIndex]);
                            }
                        }
                        else
                        {
                            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                            {
                                m_smVisionInfo.g_arrColorImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                                m_smVisionInfo.g_arrColorImages[intImageIndex].CopyTo(m_arrColorRotatedImage[intImageIndex]);
                            }
                        }
                    }
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].LoadROISetting((int)Math.Round((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX, 0, MidpointRounding.AwayFromZero),
                                                 (int)Math.Round((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY, 0, MidpointRounding.AwayFromZero),
                                                 (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth, 0, MidpointRounding.AwayFromZero),
                                                 (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight, 0, MidpointRounding.AwayFromZero));

                    AddColorROI(m_smVisionInfo.g_arrPackageColorROIs);
                    
                    m_smVisionInfo.g_intSelectedColorThresholdIndex = -1;
                    m_smVisionInfo.g_intSelectedDontCareROIIndex = -1;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;

                    lbl_StepNo.BringToFront();
                    lbl_Color.BringToFront();
                    tabControl_Main.SelectedTab = tp_Color;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = false;
                    break;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void PlaceMoldFlashDontCareROI()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.Count; i++)
            {
                
                m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX - m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].ref_intStartOffsetX),
                                              (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY - m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].ref_intStartOffsetY),
                                              m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].ref_ROIWidth, m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].ref_ROIHeight);
                
            }
        }
        private void UpdateGUI()
        {
            if (m_smVisionInfo.g_intPackageDefectInspectionMethod == 1)
            {
                btn_PackageGrayValueSensitivitySetting.Visible = true;
                pnl_BrightThreshold.Visible = false;
                pnl_DarkThreshold.Visible = false;
            }
            else
            {
                btn_PackageGrayValueSensitivitySetting.Visible = false;
                pnl_BrightThreshold.Visible = m_blnVisible_pnl_BrightThreshold;
                pnl_DarkThreshold.Visible = m_blnVisible_pnl_DarkThreshold;
            }

            if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
            {
                lbl_Img1BrightFieldDecs.Text = "Image 1 : Bright Field";
                lbl_Image2_BrightField.Text = "Image 2 : Bright Field";
                if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    lbl_Image3_DarkField_Crack.Text = "Image 3 : Dark Field";
                //if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateVoidDefectSetting)
                    lbl_Image3_DarkField_Void.Text = "Image 3 : Dark Field";

            }

            if (m_blnDisablePkgGaugeSetting)
            {
                srmLabel3.Visible = false;
                cbo_ImagesList.Visible = false;
                //btn_GaugeAdvanceSetting.Visible = false;
            }

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_SetToAll.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_Package", false));
            chk_SetToAll_DarkField2.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_DarkField2", false));
            chk_SetToAll_DarkField3.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_DarkField3", false));
            chk_SetToAll_DarkField4.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_DarkField4", false));
            chk_SetToAll_Mold.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_Mold", false));
            chk_SetToAll_Chip.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_Chip", false));
            chk_SetToBrightDark_Chip.Checked = Convert.ToBoolean(subkey1.GetValue("SetToBrightDark_Chip", false));
            chk_SetToBrightDark.Checked = Convert.ToBoolean(subkey1.GetValue("SetToBrightDarkPkg", false));
            m_smVisionInfo.g_blnUseSameDontCareForBrightAndDark = chk_BrightAndDarkSameDontCareArea.Checked = Convert.ToBoolean(subkey1.GetValue("BrightAndDarkSameDontCareArea_Package", false));
            
            //04-03-2019 ZJYeoh : remove conversion(/ m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
            txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();// txt_StartPixelFromEdge.Text = Math.Round(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero).ToString();
            txt_StartPixelFromRight.Text =m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
            txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
            txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();

            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateBrightDarkROITolerance)
            {
                lbl_PkgROIDefectType.Visible = false;
                chk_SetToBrightDark.Checked = chk_SetToBrightDark.Visible = false;
                cbo_PkgDefectType.Visible = false;
                cbo_PkgDefectType.SelectedIndex = 0;
            }

            txt_StartPixelFromEdge_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2.ToString();
            txt_StartPixelFromRight_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2.ToString();
            txt_StartPixelFromBottom_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2.ToString();
            txt_StartPixelFromLeft_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2.ToString();

            txt_StartPixelFromEdge_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3.ToString();
            txt_StartPixelFromRight_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3.ToString();
            txt_StartPixelFromBottom_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3.ToString();
            txt_StartPixelFromLeft_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3.ToString();

            txt_StartPixelFromEdge_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4.ToString();
            txt_StartPixelFromRight_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4.ToString();
            txt_StartPixelFromBottom_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4.ToString();
            txt_StartPixelFromLeft_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4.ToString();


            txt_StartPixelFromEdge_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold < 0)
                txt_StartPixelFromEdge_Mold.ForeColor = Color.Black;
            txt_StartPixelFromRight_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold < 0)
                txt_StartPixelFromRight_Mold.ForeColor = Color.Black;
            txt_StartPixelFromBottom_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold < 0)
                txt_StartPixelFromBottom_Mold.ForeColor = Color.Black;
            txt_StartPixelFromLeft_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold < 0)
                txt_StartPixelFromLeft_Mold.ForeColor = Color.Black;
            txt_StartPixelFromEdgeInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold < 0)
                txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Black;
            txt_StartPixelFromRightInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold < 0)
                txt_StartPixelFromRightInner_Mold.ForeColor = Color.Black;
            txt_StartPixelFromBottomInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold < 0)
                txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Black;
            txt_StartPixelFromLeftInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold < 0)
                txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Black;
            txt_MoldFlashMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashMinArea.ToString();

            cbo_ChippedOffDefectType.SelectedIndex = 0;
            cbo_PkgDefectType.SelectedIndex = 0;

            txt_StartPixelFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip.ToString();
            txt_StartPixelFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip.ToString();
            txt_StartPixelFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip.ToString();
            txt_StartPixelFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip.ToString();

            txt_StartPixelExtendFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip.ToString();
            txt_StartPixelExtendFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip.ToString();
            txt_StartPixelExtendFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip.ToString();
            txt_StartPixelExtendFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip.ToString();

            txt_BrightMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intBrightFieldMinArea.ToString();
            txt_DarkMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldMinArea.ToString();
            txt_Dark2MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2MinArea.ToString();
            txt_Dark3MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3MinArea.ToString();
            txt_Dark4MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4MinArea.ToString();
            txt_MarkViewMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewMinArea.ToString();
            txt_PkgViewMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intPkgViewMinArea.ToString();
            txt_CrackViewMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewMinArea.ToString();
            txt_UnitSizeMinWidth.Text = Math.Round(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetUnitWidthMin(1), 3, MidpointRounding.AwayFromZero).ToString("F4");
            txt_UnitSizeMaxWidth.Text = Math.Round(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetUnitWidthMax(1), 3, MidpointRounding.AwayFromZero).ToString("F4");
            txt_UnitSizeMinHeight.Text = Math.Round(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetUnitHeightMin(1), 3, MidpointRounding.AwayFromZero).ToString("F4");
            txt_UnitSizeMaxHeight.Text = Math.Round(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetUnitHeightMax(1), 3, MidpointRounding.AwayFromZero).ToString("F4");
            if (m_smVisionInfo.g_arrPackageGaugeM4L != null && m_smVisionInfo.g_arrPackageGaugeM4L.Count >0)
                txt_ImageGain1.Value = Convert.ToDecimal(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
            if (m_smVisionInfo.g_arrPackageGauge2M4L != null && m_smVisionInfo.g_arrPackageGaugeM4L.Count > 0)
                txt_ImageGain2.Value = Convert.ToDecimal(m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
            txt_Image2MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView1MinArea.ToString();
            txt_Image3MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView2MinArea.ToString();
            txt_VoidViewMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewMinArea.ToString();
            //cbo_ImagesList.Items.Clear();
            //cbo_MarkViewImageList.Items.Clear();
            //cbo_PkgViewImageList.Items.Clear();
            //cbo_CrackViewImageList.Items.Clear();
            //for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            //{
            //    //cbo_ImagesList.Items.Add("Grab Image " + (i + 1));
            //    cbo_MarkViewImageList.Items.Add("Grab Image " + (i + 1));
            //    cbo_PkgViewImageList.Items.Add("Grab Image " + (i + 1));
            //    cbo_CrackViewImageList.Items.Add("Grab Image " + (i + 1));
            //}
            srmLabel21.Visible = cbo_MarkViewImageList.Visible = false;
            srmLabel22.Visible = cbo_PkgViewImageList.Visible = false;
            srmLabel23.Visible = cbo_CrackViewImageList.Visible = false;


            if (cbo_ImagesList.Items.Count > 0)
            {
                if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0) < 1) ||    // "< 1" because GetGrabImageIndex will keep value 1 or 2 represent image index 2 (package1 image) and image index 3 (package2 image). 
                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0) >= cbo_ImagesList.Items.Count))
                    cbo_ImagesList.SelectedIndex = 0;   // For Package1 image
                else
                    cbo_ImagesList.SelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0) - 1; // "-1" because GetGrabImageIndex will keep value 1 or 2 represent image index 2 (package1 image) and image index 3 (package2 image). 
            }
            if (cbo_MarkViewImageList.Items.Count > 0)
            {
                if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(1) < 0) ||
                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(1) >= cbo_ImagesList.Items.Count))
                    cbo_MarkViewImageList.SelectedIndex = 0;
                else
                    cbo_MarkViewImageList.SelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(1);
            }
            if (cbo_PkgViewImageList.Items.Count > 0)
            {
                if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) < 0) ||
                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) >= cbo_ImagesList.Items.Count))
                    cbo_PkgViewImageList.SelectedIndex = 0;
                else
                    cbo_PkgViewImageList.SelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2);
            }
            if (cbo_CrackViewImageList.Items.Count > 0)
            {
                if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) < 0) ||
                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) >= cbo_ImagesList.Items.Count))
                    cbo_CrackViewImageList.SelectedIndex = 0;
                else
                    cbo_CrackViewImageList.SelectedIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);
            }

            lbl_Image2_BrightField.Text = lbl_Image2_BrightField.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            lbl_Image2_BrightField_Chip.Text = lbl_Image2_BrightField_Chip.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            lbl_Image3_DarkField_Chip.Text = lbl_Image3_DarkField_Chip.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            lbl_Image3_DarkField_Crack.Text = lbl_Image3_DarkField_Crack.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            lbl_Image3_DarkField_Void.Text = lbl_Image3_DarkField_Void.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            lbl_Image2_BrightField_MF.Text = lbl_Image2_BrightField_MF.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            lbl_Image2_BrightField_Simple.Text = lbl_Image2_BrightField_Simple.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            lbl_Image3_DarkField_Simple.Text = lbl_Image3_DarkField_Simple.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            lbl_TitleBrightFieldSimple.Text = lbl_TitleBrightFieldSimple.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            lbl_TitleDarkFieldSimple.Text = lbl_TitleDarkFieldSimple.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            cbo_DontCareAreaDrawMethod.SelectedIndex = 0;
            radio_Bright.Checked = true;
            m_smVisionInfo.g_intSelectedType = 0; // 2020-02-27 ZJYEOH : Need to reset m_smVisionInfo.g_intSelectedType to 0 so that dark dont care area will not draw on white image when user previously exit form by selecting dark image

            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count == 0)
            {
                txt_DunCareROIBright.Text = 0.ToString();
                txt_DunCareROIDark.Text = 0.ToString();
            }
            else
            {
                if (m_smVisionInfo.g_arrPackageDontCareROIs.Count > 1)
                {
                    txt_DunCareROIBright.Text = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count.ToString();
                    txt_DunCareROIDark.Text = m_smVisionInfo.g_arrPackageDontCareROIs[1].Count.ToString();
                }
                else
                {
                    txt_DunCareROIBright.Text = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count.ToString();
                    txt_DunCareROIDark.Text = 0.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
            {
                chk_ShowDraggingBox.Checked = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_blnDrawDraggingBox = true;
                chk_ShowSamplePoints.Checked = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_blnDrawSamplingPoint = true;
                chk_ProductionTest.Checked = m_smVisionInfo.g_blnReferTemplateSize = true;

                if (i == 0)
                {
                    if (m_smVisionInfo.g_arrPackageGaugeM4L[i].GetGaugeMeasureMode(0) == 3) // Measure Mode Value 0 = Standard 1 = Separation 2 = Scan Points 3 = Multi Lines
                    {
                        chk_ProductionTest.Visible = true;
                    }
                    else
                    {
                        chk_ProductionTest.Visible = false;
                    }

                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
            {
                chk_ShowDraggingBox_2.Checked = m_smVisionInfo.g_arrPackageGauge2M4L[i].ref_blnDrawDraggingBox;
                chk_ShowSamplePoints_2.Checked = m_smVisionInfo.g_arrPackageGauge2M4L[i].ref_blnDrawSamplingPoint;
            }

            CheckPkgROISetting();
            CheckPkgDarkROISetting();
            CheckChipInwardROISetting();
            CheckChipOutwardROISetting();
            CheckPkgMoldOuterROISetting();
            CheckPkgMoldInnerROISetting();
        }





        private void txt_MarkViewMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_MarkViewMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intMarkViewMinArea = Convert.ToInt32(txt_MarkViewMinArea.Text);
            }

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildMarkViewObjects(
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgViewMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_PkgViewMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intPkgViewMinArea = Convert.ToInt32(txt_PkgViewMinArea.Text);
                if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
                {
                    m_smVisionInfo.g_arrPackage[i].ref_intChipView1MinArea = Convert.ToInt32(txt_PkgViewMinArea.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_intMoldFlashMinArea = Convert.ToInt32(txt_PkgViewMinArea.Text);
                }
            }

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildPackageViewObjects(
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromEdge_TextChanged(object sender, EventArgs e)
       {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge.Text == "" || fStartPixelFromEdge < 0)
                return;

            //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            //{
            //    if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
            //        return;
            //    }
            //}

            int intPixelBottom = 0;
            if (cbo_PkgDefectType.SelectedIndex == 0)
                intPixelBottom = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom;
            else
                intPixelBottom = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + intPixelBottom) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (cbo_PkgDefectType.SelectedIndex == 0)
                        txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
                    else
                        txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark.ToString();

                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + intPixelBottom) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (cbo_PkgDefectType.SelectedIndex == 0)
                        txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
                    else
                        txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark.ToString();

                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_StartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_PkgDefectType.SelectedIndex == 0)
                {
                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                    if (chk_SetToAll.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark.Checked)
                    {
                        //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                    if (chk_SetToAll.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark.Checked)
                    {
                        //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }

            if (cbo_PkgDefectType.SelectedIndex == 0)
            {
                if (chk_SetToAll.Checked)
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge);
                else
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);
            }
            else
            {
                if (chk_SetToAll.Checked)
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark);
                else
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark);
            }

            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRight.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight.Text == "" || fStartPixelFromEdge < 0)
                return;

            //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            //{
            //    if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
            //        return;
            //    }
            //}

            int intPixelLeft = 0;
            if (cbo_PkgDefectType.SelectedIndex == 0)
                intPixelLeft = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft;
            else
                intPixelLeft = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + intPixelLeft) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (cbo_PkgDefectType.SelectedIndex == 0)
                        txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
                    else
                        txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark.ToString();

                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + intPixelLeft) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (cbo_PkgDefectType.SelectedIndex == 0)
                        txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
                    else
                        txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark.ToString();

                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_StartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_PkgDefectType.SelectedIndex == 0)
                {
                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                    if (chk_SetToAll.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark.Checked)
                    {
                        //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                    if (chk_SetToAll.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark.Checked)
                    {
                        //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }

            if (cbo_PkgDefectType.SelectedIndex == 0)
            {
                if (chk_SetToAll.Checked)
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight);
                else
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
                  m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
                  m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
                  m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);
            }
            else
            {
                if (chk_SetToAll.Checked)
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark);
                else
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
                  m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
                  m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
                  m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark);
            }

            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottom.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom.Text == "" || fStartPixelFromEdge < 0)
                return;

            //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            //{
            //    if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
            //        return;
            //    }
            //}

            int intPixelEdge = 0;
            if (cbo_PkgDefectType.SelectedIndex == 0)
                intPixelEdge = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge;
            else
                intPixelEdge = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + intPixelEdge) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (cbo_PkgDefectType.SelectedIndex == 0)
                        txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
                    else
                        txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark.ToString();

                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + intPixelEdge) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (cbo_PkgDefectType.SelectedIndex == 0)
                        txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
                    else
                        txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark.ToString();

                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_StartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_PkgDefectType.SelectedIndex == 0)
                {
                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                    if (chk_SetToAll.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if(chk_SetToBrightDark.Checked)
                    {
                        //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                    if (chk_SetToAll.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark.Checked)
                    {
                        //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }

            if (cbo_PkgDefectType.SelectedIndex == 0)
            {
                if (chk_SetToAll.Checked)
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom);
                else
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);
            }
            else
            {
                if (chk_SetToAll.Checked)
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark);
                else
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark);
            }

            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeft.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft.Text == "" || fStartPixelFromEdge < 0)
                return;

            //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            //{
            //    if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();
            //        return;
            //    }
            //}

            int intPixelRight = 0;
            if (cbo_PkgDefectType.SelectedIndex == 0)
                intPixelRight = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight;
            else
                intPixelRight = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + intPixelRight) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (cbo_PkgDefectType.SelectedIndex == 0)
                        txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();
                    else
                        txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark.ToString();

                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + intPixelRight) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (cbo_PkgDefectType.SelectedIndex == 0)
                        txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();
                    else
                        txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark.ToString();

                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_StartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_PkgDefectType.SelectedIndex == 0)
                {
                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }
                    //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);

                    if (chk_SetToBrightDark.Checked)
                    {
                        //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }
                    //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);

                    if (chk_SetToBrightDark.Checked)
                    {
                        //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }

            if (cbo_PkgDefectType.SelectedIndex == 0)
            {
                if (chk_SetToAll.Checked)
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);
                else
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
                            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
                            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
                            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);
            }
            else
            {
                if (chk_SetToAll.Checked)
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark,
              m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark);
                else
                    AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
                            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
                            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
                            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark);
            }

            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromEdge_DarkField2_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge_DarkField2.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge_DarkField2.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField2.Checked)
            {
                txt_StartPixelFromRight_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField2.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3 = m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField2;
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField2.Checked)
                {


                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            CheckPkgDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromEdge_DarkField3_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge_DarkField3.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge_DarkField3.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField3.Checked)
            {
                txt_StartPixelFromRight_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField3.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField3.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromEdge_DarkField4_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge_DarkField4.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge_DarkField4.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField4.Checked)
            {
                txt_StartPixelFromRight_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField4.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField4.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_DarkField2_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRight_DarkField2.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight_DarkField2.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField2.Checked)
            {
                txt_StartPixelFromEdge_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField2.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField2;
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField2.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            CheckPkgDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_DarkField3_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRight_DarkField3.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight_DarkField3.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField3.Checked)
            {
                txt_StartPixelFromEdge_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField3.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField2.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_DarkField4_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRight_DarkField4.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight_DarkField4.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField4.Checked)
            {
                txt_StartPixelFromEdge_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField4.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField4.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_DarkField2_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottom_DarkField2.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom_DarkField2.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField2.Checked)
            {
                txt_StartPixelFromEdge_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField2.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField2;
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField2.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            CheckPkgDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_DarkField3_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottom_DarkField3.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom_DarkField3.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField3.Checked)
            {
                txt_StartPixelFromEdge_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField3.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField3.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3= (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_DarkField4_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottom_DarkField4.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom_DarkField4.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField4.Checked)
            {
                txt_StartPixelFromEdge_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_DarkField4.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll_DarkField4.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_DarkField2_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeft_DarkField2.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft_DarkField2.Text == "" || fStartPixelFromEdge < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField2.Checked)
            {
                txt_StartPixelFromEdge_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField2.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2;
                if (chk_SetToAll_DarkField2.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
            }
            CheckPkgDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_DarkField3_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeft_DarkField3.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft_DarkField3.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField3.Checked)
            {
                txt_StartPixelFromEdge_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField3.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_DarkField3.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_DarkField4_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeft_DarkField4.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft_DarkField4.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4.ToString();
                    return;
                }
            }

            if (chk_SetToAll_DarkField4.Checked)
            {
                txt_StartPixelFromEdge_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_DarkField4.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_DarkField4.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            LoadControlSettings(m_strFolderPath + "Template\\");
            LoadROISetting(m_strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPackageROIs);
            LoadDontCareROISetting(m_strFolderPath + "DontCareROI.xml", m_smVisionInfo.g_arrPackageDontCareROIs);
            LoadROISetting(m_strFolderPath + "ColorDontCareROI.xml", m_smVisionInfo.g_arrPackageColorDontCareROIs, m_smVisionInfo.g_intUnitsOnImage);
            LoadColorROISetting(m_strFolderPath + "CROI.xml", m_smVisionInfo.g_arrPackageColorROIs, m_smVisionInfo.g_intUnitsOnImage);
            ROI.LoadFile(m_strFolderPath + "MoldFlashDontCareROI.xml", m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs);

            for (int i = 0; i < m_smVisionInfo.g_arrPackageDontCareROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrPackageDontCareROIs[i].Count; j++)
                    m_smVisionInfo.g_arrPackageDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

            for (int i = 0; i < m_smVisionInfo.g_arrPackageColorDontCareROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrPackageColorDontCareROIs[i].Count; j++)
                    for (int k = 0; k < m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j].Count; k++)
                        m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

            if (m_smVisionInfo.g_intLearnStepNo == 1)
            {
                if (m_smVisionInfo.g_arrPackageGauge2M4L.Count > 0)
                    LoadGaugeM4LSetting(m_strFolderPath + "Gauge2M4L.xml", m_smVisionInfo.g_arrPackageGauge2M4L);
            }
            else
                LoadGaugeM4LSetting(m_strFolderPath + "GaugeM4L.xml", m_smVisionInfo.g_arrPackageGaugeM4L);

            LoadPackageSettings(m_strFolderPath);
            Polygon.LoadPolygon(m_strFolderPath + "Template\\Polygon.xml", m_smVisionInfo.g_arrPolygon_Package, true);
            Polygon.LoadPolygon(m_strFolderPath + "Template\\ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PackageColor, m_smVisionInfo.g_intUnitsOnImage);

            AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrImages[0]);

            DialogResult = DialogResult.Cancel;
            Close();
            Dispose();
        }

        private void btn_RotateUnit_Click(object sender, EventArgs e)
        {
         
            if (sender == btn_ClockWise)
            {
                
                ROI.RotateROI_Center(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], 270, ref m_smVisionInfo.g_arrRotatedImages, 1); //RotateROI
                ROI.RotateROI_Center(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], 270, ref m_smVisionInfo.g_arrRotatedImages, 0);//RotateROI
               
            }
            else if (sender == btn_CounterClockWise)
            {
               
                ROI.RotateROI_Center(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], 90, ref m_smVisionInfo.g_arrRotatedImages, 1);//RotateROI
                ROI.RotateROI_Center(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], 90, ref m_smVisionInfo.g_arrRotatedImages, 0);//RotateROI
                
            }
          
            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
           
            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
        }

        private void btn_MarkViewThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnPackageInspected = false;
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intMarkViewLowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intMarkViewHighThreshold;
            
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intMarkViewLowThreshold = intLowThreshold;
                objPackage.ref_intCrackViewHighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intMarkViewLowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intMarkViewHighThreshold = m_smVisionInfo.g_intHighThresholdValue;

                lbl_LowMarkViewThreshold.Text = objPackage.ref_intMarkViewLowThreshold.ToString();
                lbl_HighMarkViewThreshold.Text = objPackage.ref_intMarkViewHighThreshold.ToString();

            }
            objThresholdForm.Dispose();

            objPackage.BuildMarkViewObjects(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            m_smVisionInfo.g_blnPackageInspected = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            // Double check setting before allow go to next tabpage
            if (m_smVisionInfo.g_intLearnStepNo == 8) // Chipped off ROI setting form
            {
                if (!IsChipInwardOutwardSettingCorrect(true))
                    return;
            }
            /*
             * 
             * 
             * 
             */

            if (m_smVisionInfo.g_intLearnStepNo == 0)
            {
                // 2020 11 28 - CCENG: Package tolerance setting no need to diplay and checking in step 1, but should display in tolerance page only which is more clean and clear.
                //if (Convert.ToDecimal(txt_UnitSizeMinWidth.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMinHeight.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= 0)
                //{
                //    SRMMessageBox.Show("Width or Height cannot be zero.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                //if (Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= Convert.ToDecimal(txt_UnitSizeMinWidth.Text))
                //{
                //    SRMMessageBox.Show("Min Width cannot be larger than or equal to Max Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                //if (Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= Convert.ToDecimal(txt_UnitSizeMinHeight.Text))
                //{
                //    SRMMessageBox.Show("Min Height cannot be larger than or equal to Max Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                //if (Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= Convert.ToDecimal(lbl_SizeWidthResult.Text))
                //{
                //    SRMMessageBox.Show("Max Width cannot be smaller than or equal to measured Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                //if (Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= Convert.ToDecimal(lbl_SizeHeightResult.Text))
                //{
                //    SRMMessageBox.Show("Max Height cannot be smaller than or equal to measured Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                //if (Convert.ToDecimal(txt_UnitSizeMinWidth.Text) >= Convert.ToDecimal(lbl_SizeWidthResult.Text))
                //{
                //    SRMMessageBox.Show("Min Width cannot be larger than or equal to measured Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                //if (Convert.ToDecimal(txt_UnitSizeMinHeight.Text) >= Convert.ToDecimal(lbl_SizeHeightResult.Text))
                //{
                //    SRMMessageBox.Show("Min Height cannot be larger than or equal to measured Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitWidthMin((float)Convert.ToDecimal(txt_UnitSizeMinWidth.Text), m_smCustomizeInfo.g_intUnitDisplay);
                //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitWidthMax((float)Convert.ToDecimal(txt_UnitSizeMaxWidth.Text), m_smCustomizeInfo.g_intUnitDisplay);
                //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitHeightMin((float)Convert.ToDecimal(txt_UnitSizeMinHeight.Text), m_smCustomizeInfo.g_intUnitDisplay);
                //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitHeightMax((float)Convert.ToDecimal(txt_UnitSizeMaxHeight.Text), m_smCustomizeInfo.g_intUnitDisplay);

                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fTemplateUnitSizeX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / m_smVisionInfo.g_fCalibPixelX;
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fTemplateUnitSizeY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / m_smVisionInfo.g_fCalibPixelY;
            }

            if (m_smVisionInfo.g_intLearnStepNo == 12)
            {
                if (txt_StartPixelFromEdge_Mold.ForeColor == Color.Red ||
                       txt_StartPixelFromRight_Mold.ForeColor == Color.Red ||
                       txt_StartPixelFromBottom_Mold.ForeColor == Color.Red ||
                       txt_StartPixelFromLeft_Mold.ForeColor == Color.Red)
                {
                    SRMMessageBox.Show("Mold Flash Outer tolerance cannot less than Inner tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                if (txt_StartPixelFromEdgeInner_Mold.ForeColor == Color.Red ||
                    txt_StartPixelFromRightInner_Mold.ForeColor == Color.Red ||
                    txt_StartPixelFromBottomInner_Mold.ForeColor == Color.Red ||
                    txt_StartPixelFromLeftInner_Mold.ForeColor == Color.Red)
                {
                    SRMMessageBox.Show("Mold Flash Inner tolerance cannot more than Outer tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

            }

            if (m_smVisionInfo.g_blnWantUseSideLightGauge)
            {
                if (m_smVisionInfo.g_intLearnStepNo == 0)
                    m_smVisionInfo.g_intLearnStepNo++;
            }

            //if (m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
            //{
            //    // 2019 07 09 - CCENG: if Size View Image inddex is 0 same as Mark view, then skip Mark View Threshold setting page.
            //    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) == 0)
            //    {
            //        if (m_smVisionInfo.g_intLearnStepNo == 2)
            //            m_smVisionInfo.g_intLearnStepNo++;
            //    }
            //    if (m_smVisionInfo.g_intLearnStepNo == 2)
            //    {
            //        m_smVisionInfo.g_intLearnStepNo += 2;
            //    }
            //    // 2019 07 25 - CCENG: if simple threshold, then from chip ROI direct go to Mold flash ROI.
            //    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package && m_smVisionInfo.g_intLearnStepNo == 8)
            //    {
            //        m_smVisionInfo.g_intLearnStepNo = 12;
            //    }
            //    else
            //    {

            //        if (m_smVisionInfo.g_intLearnStepNo < 14)
            //            m_smVisionInfo.g_intLearnStepNo++;

            //        if (m_smVisionInfo.g_intLearnStepNo == 3)
            //            if (m_smVisionInfo.g_arrImages.Count < 3)
            //            {
            //                m_smVisionInfo.g_intLearnStepNo++;
            //            }
            //    }
            //}
            //else
            {
                m_smVisionInfo.g_intLearnStepNo++;

                if (m_smVisionInfo.g_intLearnStepNo == 5)
                {
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 8;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 12;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 14;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 16;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 18;
                    }
                    else if (m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 20;
                    }
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 8)
                {
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 8;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 12;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 14;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 16;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 18;
                    }
                    else if (m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 20;
                    }
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 11)
                {
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 12;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 14;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 16;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 18;
                    }
                    else if (m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 20;
                    }
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 14)
                {
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 14;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 16;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 18;
                    }
                    else if (m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 20;
                    }
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 16)
                {
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 16;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 18;
                    }
                    else if (m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 20;
                    }
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 18)
                {
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 18;
                    }
                    else if (m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 20;
                    }
                }
            }
            m_intDisplayStep++;
            SetupSteps();

            if(m_intDisplayStep>10)
            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 3) + m_intDisplayStep.ToString() + ":";
            else
                lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStep.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_PackageViewThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageObjectBuilded = false;   // hide blobs drawing

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intThreshold = m_smVisionInfo.g_intThresholdValue = objPackage.ref_intPkgViewThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                objPackage.ref_intPkgViewThreshold = intThreshold;
            else
            {
                objPackage.ref_intPkgViewThreshold = m_smVisionInfo.g_intThresholdValue;
                if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
                {
                    objPackage.ref_intChipView1Threshold = m_smVisionInfo.g_intThresholdValue;
                    objPackage.ref_intMoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;
                }
                lbl_PackageViewThreshold.Text = objPackage.ref_intPkgViewThreshold.ToString();
            }

            objThresholdForm.Dispose();

            objPackage.BuildPackageViewObjects(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            //m_smVisionInfo.g_blnViewPackageObjectBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            // Double check setting before allow go to previous tabpage
            if (m_smVisionInfo.g_intLearnStepNo == 8) // Chipped off ROI setting form
            {
                if (!IsChipInwardOutwardSettingCorrect(true))
                    return;
            }

            //if (m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
            //{
            //    if (m_smVisionInfo.g_intLearnStepNo > 0)
            //        m_smVisionInfo.g_intLearnStepNo--;

            //    if (m_smVisionInfo.g_intLearnStepNo == 4)
            //    {
            //        m_smVisionInfo.g_intLearnStepNo -= 2;
            //    }

            //    //if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
            //    //{
            //    //    if (m_smVisionInfo.g_intLearnStepNo == 11)
            //    //        m_smVisionInfo.g_intLearnStepNo = 8;
            //    //}

            //    if (m_smVisionInfo.g_intLearnStepNo == 3)
            //    {
            //        if (m_smVisionInfo.g_arrImages.Count < 3)
            //        {
            //            m_smVisionInfo.g_intLearnStepNo--;
            //        }
            //        else
            //        {
            //            // 2019 07 09 - CCENG: if Size View Image inddex is 0 same as Mark view, then skip Mark View Threshold setting page.
            //            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) == 0)
            //            {
            //                m_smVisionInfo.g_intLearnStepNo--;
            //            }
            //        }


            //    }
            //}
            //else
            {
                if (m_smVisionInfo.g_intLearnStepNo > 0)
                    m_smVisionInfo.g_intLearnStepNo--;

                if (m_smVisionInfo.g_intLearnStepNo == 19)
                {
                    for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                    {
                        m_arrRotatedImage[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                        {
                            m_arrColorRotatedImage[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                        }
                    }
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 19;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 17;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 15;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 13;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 10;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    else
                        m_smVisionInfo.g_intLearnStepNo = 4;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 17)
                {
                    for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                    {
                        m_arrRotatedImage[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                        {
                            m_arrColorRotatedImage[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                        }
                    }

                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 17;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 15;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 13;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 10;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    else
                        m_smVisionInfo.g_intLearnStepNo = 4;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 15)
                {
                    for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                    {
                        m_arrRotatedImage[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                        {
                            m_arrColorRotatedImage[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                        }
                    }

                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 15;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 13;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 10;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    else
                        m_smVisionInfo.g_intLearnStepNo = 4;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 13)
                {
                    for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                    {
                        m_arrRotatedImage[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                    }

                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                        {
                            m_arrColorRotatedImage[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                        }
                    }

                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 13;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 10;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    else
                        m_smVisionInfo.g_intLearnStepNo = 4;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 11)
                {
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 10;
                    }
                    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    else
                        m_smVisionInfo.g_intLearnStepNo = 4;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 7)
                {
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    else
                        m_smVisionInfo.g_intLearnStepNo = 4;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 6)
                {
                    m_smVisionInfo.g_intLearnStepNo = 4;
                }
            }

            if (m_smVisionInfo.g_blnWantUseSideLightGauge)
            {
                if (m_smVisionInfo.g_intLearnStepNo == 1)
                    m_smVisionInfo.g_intLearnStepNo--;
            }

            m_intDisplayStep--;
            SetupSteps();

            if (m_intDisplayStep > 8)
                lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 3) + m_intDisplayStep.ToString() + ":";
            else
                lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStep.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            switch (m_intLearnType)
            {
                case 0:
                    if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)  // Lead unit
                        DefineUnitSurfaceOffset();

                    CopyFiles m_objCopy = new CopyFiles();
                    string strCurrentDateTIme = DateTime.Now.ToString();
                    DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
                    string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
                    string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
                    string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Package\\";
                    

                    if (File.Exists(m_strFolderPath + "Template\\OriTemplate0.bmp"))
                        STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Package", "OriTemplate0.bmp", "OriTemplate0.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
                    else
                        STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Package", "", "OriTemplate0.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

                    m_objCopy.CopyAllImageFiles(m_strFolderPath + "Template\\", strPath + "Old\\");

                    #region Save Template Image
                    // Save Template Image to template folder
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        m_smVisionInfo.g_arrColorRotatedImages[0].SaveImage(m_strFolderPath + "Template\\OriTemplate0.bmp");
                        if (m_smVisionInfo.g_arrColorRotatedImages.Count > 1)
                            m_smVisionInfo.g_arrColorRotatedImages[1].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image1.bmp");
                        if (m_smVisionInfo.g_arrColorRotatedImages.Count > 2)
                            m_smVisionInfo.g_arrColorRotatedImages[2].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image2.bmp");
                        if (m_smVisionInfo.g_arrColorRotatedImages.Count > 3)
                            m_smVisionInfo.g_arrColorRotatedImages[3].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image3.bmp");
                        if (m_smVisionInfo.g_arrColorRotatedImages.Count > 4)
                            m_smVisionInfo.g_arrColorRotatedImages[4].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image4.bmp");
                        if (m_smVisionInfo.g_arrColorRotatedImages.Count > 5)
                            m_smVisionInfo.g_arrColorRotatedImages[5].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image5.bmp");
                        if (m_smVisionInfo.g_arrColorRotatedImages.Count > 6)
                            m_smVisionInfo.g_arrColorRotatedImages[6].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image6.bmp");
                    }
                    else
                    {
                        m_smVisionInfo.g_arrRotatedImages[0].SaveImage(m_strFolderPath + "Template\\OriTemplate0.bmp");
                        if (m_smVisionInfo.g_arrRotatedImages.Count > 1)
                            m_smVisionInfo.g_arrRotatedImages[1].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image1.bmp");
                        if (m_smVisionInfo.g_arrRotatedImages.Count > 2)
                            m_smVisionInfo.g_arrRotatedImages[2].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image2.bmp");
                        if (m_smVisionInfo.g_arrRotatedImages.Count > 3)
                            m_smVisionInfo.g_arrRotatedImages[3].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image3.bmp");
                        if (m_smVisionInfo.g_arrRotatedImages.Count > 4)
                            m_smVisionInfo.g_arrRotatedImages[4].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image4.bmp");
                        if (m_smVisionInfo.g_arrRotatedImages.Count > 5)
                            m_smVisionInfo.g_arrRotatedImages[5].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image5.bmp");
                        if (m_smVisionInfo.g_arrRotatedImages.Count > 6)
                            m_smVisionInfo.g_arrRotatedImages[6].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image6.bmp");
                    }

                    ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
                    ROI objROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];
                    objSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0)]);
                    objROI.AttachImage(objSearchROI);
                    objROI.SaveImage(m_strFolderPath + "Template\\Template0.bmp");
                    #endregion

                    #region Save Pattern
                    //Unit PR
                    if (!(m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || m_smVisionInfo.g_strVisionName.Contains("BottomOrient")) && (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].LearnUnitPRPattern(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitPRFinalReduction(2);
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitPRAngleSetting(0, 0);// (-10, 10);
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SaveUnitPRPattern(m_strFolderPath + "Template\\Template0.mch");
                    }
                    #endregion

                    m_objCopy.CopyAllImageFiles(m_strFolderPath + "Template\\", strPath + "New\\");

                    SavePackageSettings();



                    SaveROISettings();

                    if (m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        SaveDontCareSetting(m_strFolderPath + "Template\\");
                        SaveDontCareROISettings();
                        SaveMoldFlashDontCare();
                        RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                        RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
                        subkey.SetValue("BrightAndDarkSameDontCareArea_Package", chk_BrightAndDarkSameDontCareArea.Checked);
                    }
                    SaveGaugeSettings();
                    AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrImages[0]);

                    DialogResult = DialogResult.OK;

                    if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                        m_smProductionInfo.g_blnSaveRecipeToServer = true;

                    m_smVisionInfo.g_blnUpdateSECSGEMFile = true;
                    break;
                case 1:
                case 2:
                    if (m_smVisionInfo.g_intLearnStepNo == 0)
                    {
                        //if (Convert.ToDecimal(txt_UnitSizeMinWidth.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMinHeight.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= 0)
                        //{
                        //    SRMMessageBox.Show("Width or Height cannot be zero.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //    return;
                        //}
                        //if (Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= Convert.ToDecimal(txt_UnitSizeMinWidth.Text))
                        //{
                        //    SRMMessageBox.Show("Min Width cannot be larger than or equal to Max Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //    return;
                        //}
                        //if (Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= Convert.ToDecimal(txt_UnitSizeMinHeight.Text))
                        //{
                        //    SRMMessageBox.Show("Min Height cannot be larger than or equal to Max Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //    return;
                        //}
                        //if (Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= Convert.ToDecimal(lbl_SizeWidthResult.Text))
                        //{
                        //    SRMMessageBox.Show("Max Width cannot be smaller than or equal to measured Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //    return;
                        //}
                        //if (Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= Convert.ToDecimal(lbl_SizeHeightResult.Text))
                        //{
                        //    SRMMessageBox.Show("Max Height cannot be smaller than or equal to measured Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //    return;
                        //}
                        //if (Convert.ToDecimal(txt_UnitSizeMinWidth.Text) >= Convert.ToDecimal(lbl_SizeWidthResult.Text))
                        //{
                        //    SRMMessageBox.Show("Min Width cannot be larger than or equal to measured Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //    return;
                        //}
                        //if (Convert.ToDecimal(txt_UnitSizeMinHeight.Text) >= Convert.ToDecimal(lbl_SizeHeightResult.Text))
                        //{
                        //    SRMMessageBox.Show("Min Height cannot be larger than or equal to measured Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        //    return;
                        //}
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitWidthMin((float)Convert.ToDecimal(txt_UnitSizeMinWidth.Text), m_smCustomizeInfo.g_intUnitDisplay);
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitWidthMax((float)Convert.ToDecimal(txt_UnitSizeMaxWidth.Text), m_smCustomizeInfo.g_intUnitDisplay);
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitHeightMin((float)Convert.ToDecimal(txt_UnitSizeMinHeight.Text), m_smCustomizeInfo.g_intUnitDisplay);
                        //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SetUnitHeightMax((float)Convert.ToDecimal(txt_UnitSizeMaxHeight.Text), m_smCustomizeInfo.g_intUnitDisplay);

                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fTemplateUnitSizeX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / m_smVisionInfo.g_fCalibPixelX;
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fTemplateUnitSizeY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / m_smVisionInfo.g_fCalibPixelY;
                    }
                    SavePackageSettings();
                    SaveROISettings();

                    SaveGaugeSettings();
                    AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrImages[0]);
                    break;
                case 3:
                case 4:
                case 5:
                case 7:
                    SavePackageSettings();
                    SaveROISettings();

                    AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrImages[0]);
                    break;
                case 6:
                    if (m_smVisionInfo.g_blnWantDontCareArea_Package)
                    {
                        SaveDontCareSetting(m_strFolderPath + "Template\\");
                        SaveDontCareROISettings();
                        SaveMoldFlashDontCare();
                        RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                        RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
                        subkey.SetValue("BrightAndDarkSameDontCareArea_Package", chk_BrightAndDarkSameDontCareArea.Checked);
                    }
                    break;
            }
            Close();
            Dispose();
        }

        private void LearnPackageForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                m_arrRotatedImage.Add(new ImageDrawing(true));

            if (m_smVisionInfo.g_blnViewColorImage)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrColorImages.Count; i++)
                    m_arrColorRotatedImage.Add(new CImageDrawing(true));
            }

            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            AddSearchAndUnitROI();

            m_smVisionInfo.g_intSelectedUnit = 0;
            // 2020 06 05 - CCENG: Use only template index 0 bcos currently LearnPackageForm coding is not scanning mark orient template when using the mark orient object.
            //                     Without set g_intSelectedTemplate to 0, the g_intSelectedTemplate may have different value, and the learning will be different also.
            m_smVisionInfo.g_intSelectedTemplate = 0;
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.g_blnDrawPkgResult = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            cbo_DontCareAreaDrawMethod.SelectedIndex = 0;
            lbl_StepNo.BringToFront();

            //for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            //{
            //    if (m_smVisionInfo.g_arrPolygon_Package.Count == i)
            //    {
            //        m_smVisionInfo.g_arrPolygon_Package.Add(new List<List<Polygon>>());
            //        for (int j = 0; j < m_smVisionInfo.g_intTotalTemplates; j++)
            //        {
            //            if (m_smVisionInfo.g_arrPolygon_Package[i].Count == j)
            //            {
            //                m_smVisionInfo.g_arrPolygon_Package[i].Add(new List<Polygon>());
            //                m_smVisionInfo.g_arrPolygon_Package[i][j].Add(new Polygon());
            //                m_smVisionInfo.g_arrPolygon_Package[i][j].Add(new Polygon());
            //            }
            //        }
            //    }
            //}
            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count > m_smVisionInfo.g_intSelectedType)
            {
                if (m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                {
                    if (m_smVisionInfo.g_arrPolygon_Package.Count > m_smVisionInfo.g_intSelectedUnit &&
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedType &&
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                            cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
                        else
                            cbo_DontCareAreaDrawMethod.SelectedIndex = 0;
                }
                else
                    cbo_DontCareAreaDrawMethod.SelectedIndex = 0;
            }
            else
                cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

            if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
            {
                pnl_DontCareFreeShape.Visible = true;
            }
            else
            {
                pnl_DontCareFreeShape.Visible = false;
            }

            m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (i < m_smVisionInfo.g_arrblnImageRotated.Length)
                    m_smVisionInfo.g_arrblnImageRotated[i] = true;
            }

            //m_smVisionInfo.g_intLearnStepNo = 0;

            SetupSteps();

            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void LearnPackageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Package Form Closed", "Exit Learn Package Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_intSelectedDontCareROIIndex = 0;
            m_smVisionInfo.g_intSelectedMoldFlashDontCareROIIndex = 0;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewPackageMaskROI = false;
            m_smVisionInfo.g_blnViewPkgProcessImage = false;
            m_smVisionInfo.g_blnViewPackageTrainROI = false;
            m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewGauge = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void cbo_ImagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage =  cbo_ImagesList.SelectedIndex + 1;
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(0, m_smVisionInfo.g_intSelectedImage);

            m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0), 1);  // View Package1 image. g_intSelectedImage should get at least value 1 represent package1 image
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intUseOtherGaugeMeasurePackage > 0)
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
                AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugePlace_BasedOnEdgeROI();
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
                AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_objPackageImage);

                //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                //m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugePlace_BasedOnEdgeROI();
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void UpdateThresholdSetting()
        {
            lbl_BrightThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intBrightFieldLowThreshold.ToString();
            lbl_DarkLowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldLowThreshold.ToString();
            lbl_DarkHighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldHighThreshold.ToString();
            lbl_LowMarkViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewLowThreshold.ToString();
            lbl_HighMarkViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewHighThreshold.ToString();
            lbl_PackageViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intPkgViewThreshold.ToString();
            lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewLowThreshold.ToString();
            lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewHighThreshold.ToString();
            lbl_ChipViewImage2Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView1Threshold.ToString();
            lbl_ChipViewImage3Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView2Threshold.ToString();
            lbl_MoldFlashThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold.ToString();
            lbl_Dark2LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2LowThreshold.ToString();
            lbl_Dark2HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2HighThreshold.ToString();
            lbl_Dark3LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3LowThreshold.ToString();
            lbl_Dark3HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3HighThreshold.ToString();
            lbl_Dark4LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4LowThreshold.ToString();
            lbl_Dark4HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4HighThreshold.ToString();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_objColorMultiThresholdForm != null)
            {
                if (m_objColorMultiThresholdForm.ref_blnFormOpened)
                {
                    if (btn_ColorThreshold.Enabled)
                    {
                        btn_ColorThreshold.Enabled = false;
                        btn_SaveColor.Enabled = false;
                        btn_Cancel.Enabled = false;
                    }
                    //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].SetCopperBlobThreshold(m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance);
                    //ImageDrawing objImage = new ImageDrawing();
                    //ROI objNewROI = new ROI();
                    //objNewROI.LoadROISetting(m_smVisionInfo.g_arrPadColorROIs[m_smVisionInfo.g_intSelectedROI][0].ref_ROITotalX, m_smVisionInfo.g_arrPadColorROIs[m_smVisionInfo.g_intSelectedROI][0].ref_ROITotalY,
                    //    m_smVisionInfo.g_arrPadColorROIs[m_smVisionInfo.g_intSelectedROI][0].ref_ROIWidth, m_smVisionInfo.g_arrPadColorROIs[m_smVisionInfo.g_intSelectedROI][0].ref_ROIHeight);
                    //objNewROI.AttachImage(objImage);
                    ////m_smVisionInfo.g_arrPad[0].BuildExposedCopper(m_smVisionInfo.g_arrPadColorROIs[0][0], objNewROI);
                    ////m_smVisionInfo.g_blnViewCopperObject = true;
                    //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    //objNewROI.Dispose();
                    //objImage.Dispose();

                }
                else
                {
                    if (!btn_ColorThreshold.Enabled)
                    {
                        btn_ColorThreshold.Enabled = true;
                        btn_SaveColor.Enabled = true;
                        btn_Cancel.Enabled = true;
                    }
                }
            }

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

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;
                m_blnInitDone = false;
                UpdateThresholdSetting();
                m_blnInitDone = true;
            }

            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if (m_smVisionInfo.g_arrPackageDontCareROIs.Count > m_smVisionInfo.g_intSelectedType)
                {
                    if (m_smVisionInfo.g_arrPolygon_Package.Count > m_smVisionInfo.g_intSelectedUnit &&
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedType &&
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                    {
                        if ((m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                            cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
                    }
                }
                else
                    cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

                if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
                {
                    pnl_DontCareFreeShape.Visible = true;
                }
                else
                {
                    pnl_DontCareFreeShape.Visible = false;
                }
            }


            if (m_smVisionInfo.g_blnViewGauge)
            {
                float fPackageSizeWidth = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / m_smVisionInfo.g_fCalibPixelX;
                float fPackgeSizeHeight = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / m_smVisionInfo.g_fCalibPixelY;

                if (fPackageSizeWidth < fPackgeSizeHeight)
                {
                    fPackageSizeWidth += m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fWidthOffsetMM;
                    fPackgeSizeHeight += m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fHeightOffsetMM;
                }
                else
                {
                    fPackgeSizeHeight += m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fWidthOffsetMM;
                    fPackageSizeWidth += m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fHeightOffsetMM;
                }

                string strResult;
                if (fPackageSizeWidth < fPackgeSizeHeight)
                {
                    strResult = fPackageSizeWidth.ToString("F4");
                    if (strResult != lbl_SizeWidthResult.Text)
                    {
                        lbl_SizeWidthResult.Text = strResult;
                    }

                    strResult = fPackgeSizeHeight.ToString("F4");
                    if (strResult != lbl_SizeHeightResult.Text)
                    {
                        lbl_SizeHeightResult.Text = strResult;
                    }
                }
                else
                {
                    strResult = fPackgeSizeHeight.ToString("F4");
                    if (strResult != lbl_SizeWidthResult.Text)
                    {
                        lbl_SizeWidthResult.Text = strResult;
                    }

                    strResult = fPackageSizeWidth.ToString("F4");
                    if (strResult != lbl_SizeHeightResult.Text)
                    {
                        lbl_SizeHeightResult.Text = strResult;
                    }
                }

                strResult = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle.ToString("F4");
                if (strResult != lbl_AngleResult.Text)
                {
                    lbl_AngleResult.Text = strResult;
                }

                //strResult = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref.GetGaugePointValidScore().ToString("F4");
                //if (strResult != lbl_ScoreResult.Text)
                //{
                //    lbl_ScoreResult.Text = strResult;
                //}
            }

            if (m_objAdvancedRectGaugeForm != null)
            {
                if (!m_objAdvancedRectGaugeForm.ref_blnShowForm)
                {
                    DefineMostSelectedImageNo(); // 2020-02-17 ZJYEOH : Display the image selected from gauge advance setting

                    m_objAdvancedRectGaugeForm.Close();
                    m_objAdvancedRectGaugeForm.Dispose();
                    m_objAdvancedRectGaugeForm = null;

                    if ((m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || m_smVisionInfo.g_strVisionName.Contains("BottomOrient")) && (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        btn_Cancel.Enabled = btn_GaugeAdvanceSetting.Enabled = btn_GaugeAdvanceSetting2.Enabled = chk_ShowDraggingBox.Enabled = chk_ShowSamplePoints.Enabled = group_GaugeMeasureResult.Enabled = true;
                    else
                        btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_GaugeAdvanceSetting.Enabled = btn_GaugeAdvanceSetting2.Enabled = chk_ShowDraggingBox.Enabled = chk_ShowSamplePoints.Enabled = group_GaugeMeasureResult.Enabled = true;

                    if (m_smVisionInfo.g_intLearnStepNo == 0) // 2020-02-03 ZJYEOH : Disable previous button when in step 1
                        btn_Previous.Enabled = false;

                    if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > 0)
                    {
                        if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMeasureMode(0) == 3) // Measure Mode Value 0 = Standard 1 = Separation 2 = Scan Points 3 = Multi Lines
                        {
                            chk_ProductionTest.Visible = true;
                        }
                        else
                            chk_ProductionTest.Visible = false;

                    }

                    chk_ProductionTest.Enabled = true;
                    m_smVisionInfo.g_blnReferTemplateSize = chk_ProductionTest.Checked = m_blnProductionTestPrev;

                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
            }
        }

        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            m_blnProductionTestPrev = chk_ProductionTest.Checked;
            m_smVisionInfo.g_blnReferTemplateSize = chk_ProductionTest.Checked = false;
            chk_ProductionTest.Enabled = false;

            m_objAdvancedRectGaugeForm = new AdvancedRectGaugeM4LForm(m_smVisionInfo, m_strFolderPath + "GaugeM4L.xml", m_smProductionInfo.g_strRecipePath +
                                 m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);

            m_objAdvancedRectGaugeForm.StartPosition = FormStartPosition.Manual;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            //m_objAdvancedRectGaugeForm.Location = new Point(resolution.Width - m_objAdvancedRectGaugeForm.Size.Width, resolution.Height - m_objAdvancedRectGaugeForm.Size.Height);
            m_objAdvancedRectGaugeForm.Location = new Point(resolution.Width - m_objAdvancedRectGaugeForm.Size.Width, 0);   // 2020 03 09 - CCENG: display gauge form from top.
            m_objAdvancedRectGaugeForm.Show();

            btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_GaugeAdvanceSetting.Enabled = btn_GaugeAdvanceSetting2.Enabled = group_GaugeMeasureResult.Enabled = false;
        }

        private void cbo_MarkViewImageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage = cbo_MarkViewImageList.SelectedIndex;
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(1, m_smVisionInfo.g_intSelectedImage);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(1);

            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildMarkViewObjects(
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_PkgViewImageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage = cbo_PkgViewImageList.SelectedIndex;
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(2, m_smVisionInfo.g_intSelectedImage);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2);  // View mark image (first image)

            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);


            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildPackageViewObjects(
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_CrackViewImageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage = cbo_CrackViewImageList.SelectedIndex;
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                m_smVisionInfo.g_arrPackage[i].SetGrabImageIndex(3, m_smVisionInfo.g_intSelectedImage);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);  // View mark image (first image)

            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_CrackViewThreshold_Click(object sender, EventArgs e)
        {
            
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intCrackViewLowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intCrackViewHighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intCrackViewLowThreshold = intLowThreshold;
                objPackage.ref_intCrackViewHighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intCrackViewLowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intCrackViewHighThreshold = m_smVisionInfo.g_intHighThresholdValue;

                lbl_LowCrackViewThreshold.Text = objPackage.ref_intCrackViewLowThreshold.ToString();
                lbl_HighCrackViewThreshold.Text = objPackage.ref_intCrackViewHighThreshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_UnitSize_Leave(object sender, EventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;

            ////m_smVisionInfo.g_arrPackage[u].ref_fUnitWidthMin = Convert.ToSingle(txt_UnitSizeMinWidth.Text);
            ////m_smVisionInfo.g_arrPackage[u].ref_fUnitHeightMin = Convert.ToSingle(txt_UnitSizeMinHeight.Text);
            ////m_smVisionInfo.g_arrPackage[u].ref_fUnitWidthMax = Convert.ToSingle(txt_UnitSizeMaxWidth.Text);
            ////m_smVisionInfo.g_arrPackage[u].ref_fUnitHeightMax = Convert.ToSingle(txt_UnitSizeMaxHeight.Text);

            //float fValue =0;
            //float fValuePrev = 0;
            //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //{
               
            //    fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitWidthMin(1);
            //    if (!float.TryParse(Convert.ToString(txt_UnitSizeMinWidth.Text), out fValue))
            //    {
            //        txt_UnitSizeMinWidth.Text = fValuePrev.ToString("F4");
            //        SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    else
            //    {
            //        if (fValue > m_smVisionInfo.g_arrPackage[u].GetUnitWidthMax(1))
            //        {
            //            txt_UnitSizeMinWidth.Text = fValuePrev.ToString("F4");
            //            SRMMessageBox.Show("Package Min Width cannot be larger than Package Max Width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            m_smVisionInfo.g_arrPackage[u].SetUnitWidthMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            //        }
            //    }

            //    fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitWidthMax(1);
            //    if (!float.TryParse(Convert.ToString(txt_UnitSizeMaxWidth.Text), out fValue))
            //    {
            //        txt_UnitSizeMaxWidth.Text = fValuePrev.ToString("F4");
            //        SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    else
            //    {
            //        if (fValue < m_smVisionInfo.g_arrPackage[u].GetUnitWidthMin(1))
            //        {
            //            txt_UnitSizeMaxWidth.Text = fValuePrev.ToString("F4");
            //            SRMMessageBox.Show("Package Max Width cannot be smaller than Package Min Width!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            m_smVisionInfo.g_arrPackage[u].SetUnitWidthMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            //        }
            //    }

            //    fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitHeightMin(1);
            //    if (!float.TryParse(Convert.ToString(txt_UnitSizeMinHeight.Text), out fValue))
            //    {
            //        txt_UnitSizeMinHeight.Text = fValuePrev.ToString("F4");
            //        SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    else
            //    {
            //        if (fValue > m_smVisionInfo.g_arrPackage[u].GetUnitHeightMax(1))
            //        {
            //            txt_UnitSizeMinHeight.Text = fValuePrev.ToString("F4");
            //            SRMMessageBox.Show("Package Min Height cannot be larger than Package Max Height!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            m_smVisionInfo.g_arrPackage[u].SetUnitHeightMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            //        }
            //    }

            //    fValuePrev = m_smVisionInfo.g_arrPackage[u].GetUnitHeightMax(1);
            //    if (!float.TryParse(Convert.ToString(txt_UnitSizeMaxHeight.Text), out fValue))
            //    {
            //        txt_UnitSizeMaxHeight.Text = fValuePrev.ToString("F4");
            //        SRMMessageBox.Show("Please enter numeric value!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    else
            //    {
            //        if (fValue < m_smVisionInfo.g_arrPackage[u].GetUnitHeightMin(1))
            //        {
            //            txt_UnitSizeMaxHeight.Text = fValuePrev.ToString("F4");
            //            SRMMessageBox.Show("Package Max Height cannot be smaller than Package Min Height!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            m_smVisionInfo.g_arrPackage[u].SetUnitHeightMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            //        }
            //    }
               
            //}
        }

        private void txt_CrackViewMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intCrackViewMinArea = Convert.ToInt32(txt_CrackViewMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ChipViewImage2Threshold_Click(object sender, EventArgs e)
        {
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intThreshold = m_smVisionInfo.g_intThresholdValue = objPackage.ref_intChipView1Threshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                objPackage.ref_intChipView1Threshold = intThreshold;
            else
            {
                objPackage.ref_intChipView1Threshold = m_smVisionInfo.g_intThresholdValue;

                lbl_ChipViewImage2Threshold.Text = objPackage.ref_intChipView1Threshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ChipViewImage3Threshold_Click(object sender, EventArgs e)
        {
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intThreshold = m_smVisionInfo.g_intThresholdValue = objPackage.ref_intChipView2Threshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                objPackage.ref_intChipView2Threshold = intThreshold;
            else
            {
                objPackage.ref_intChipView2Threshold = m_smVisionInfo.g_intThresholdValue;

                lbl_ChipViewImage3Threshold.Text = objPackage.ref_intChipView2Threshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Image2MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_Image2MinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intChipView1MinArea = Convert.ToInt32(txt_Image2MinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Image3MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_Image3MinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intChipView2MinArea = Convert.ToInt32(txt_Image3MinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private bool FindUnitCenterPoint()
        {
            ROI objSearchROI = new ROI();
            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].CopyTo(ref objSearchROI);
            int intMarkOrintImageIndex = 0; // 2019 01 03 - CCENG: Image used for MO Inspection is always 0 index
            objSearchROI.AttachImage(m_smVisionInfo.g_arrImages[intMarkOrintImageIndex]);

            m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(objSearchROI);

            return true;
        }

        private void DefineUnitSurfaceOffset()
        {
            float fUnitPRCenterX = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX + 
                                   m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX();
            float fUnitPRCenterY = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY +
                                   m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY();

            float fUnitSurfaceCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fUnitSurfaceCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_fUnitSurfaceOffsetX = fUnitSurfaceCenterX - fUnitPRCenterX;
                m_smVisionInfo.g_arrPackage[i].ref_fUnitSurfaceOffsetY = fUnitSurfaceCenterY - fUnitPRCenterY;
            }
                
        }

        private void txt_VoidViewMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_VoidViewMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intVoidViewMinArea = Convert.ToInt32(txt_VoidViewMinArea.Text);

                if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
                {
                    m_smVisionInfo.g_arrPackage[i].ref_intChipView2MinArea = Convert.ToInt32(txt_VoidViewMinArea.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_intCrackViewMinArea = Convert.ToInt32(txt_VoidViewMinArea.Text);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_VoidViewImage3Threshold_Click(object sender, EventArgs e)
        {
    
            int intThreshold = m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewThreshold = intThreshold;
            else
            {
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewThreshold = m_smVisionInfo.g_intThresholdValue;
                if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewLowThreshold = m_smVisionInfo.g_intThresholdValue;
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewHighThreshold = 255;
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView2Threshold = m_smVisionInfo.g_intThresholdValue;
                }
                lbl_VoidViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewThreshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromEdge_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge_Mold.Text == "" /*|| fStartPixelFromEdge < 0*/)
                return;
            
            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (chk_SetToAll_Mold.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_StartPixelFromRight_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_StartPixelFromBottom_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_StartPixelFromLeft_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartY - fStartPixelFromEdge <= SearchROIStartY)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Black;
                }

            }



            if (chk_SetToAll_Mold.Checked)
            {
                txt_StartPixelFromBottom_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_Mold.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_Mold.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }

            }
            CheckPkgMoldOuterROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRight_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight_Mold.Text == "" /*|| fStartPixelFromEdge < 0*/)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll_Mold.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_StartPixelFromRight_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_StartPixelFromBottom_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_StartPixelFromLeft_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Black;
                }

            }

            if (chk_SetToAll_Mold.Checked)
            {
                txt_StartPixelFromEdge_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_Mold.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_Mold.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }

            }
            CheckPkgMoldOuterROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottom_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom_Mold.Text == "" /*|| fStartPixelFromEdge < 0*/)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll_Mold.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_StartPixelFromRight_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_StartPixelFromBottom_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_StartPixelFromLeft_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Black;
                }

            }



            if (chk_SetToAll_Mold.Checked)
            {
                txt_StartPixelFromEdge_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_Mold.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_Mold.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }

            }
            CheckPkgMoldOuterROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeft_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft_Mold.Text == "" /*|| fStartPixelFromEdge < 0*/)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll_Mold.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_StartPixelFromRight_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_StartPixelFromBottom_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_StartPixelFromLeft_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdge_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRight_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottom_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartX - fStartPixelFromEdge <= SearchROIStartX)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeft_Mold.ForeColor = Color.Black;
                }
            }

            if (chk_SetToAll_Mold.Checked)
            {
                txt_StartPixelFromEdge_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_Mold.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_Mold.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
               
            }
            CheckPkgMoldOuterROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAll_Mold_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_Mold", chk_SetToAll_Mold.Checked);

            CheckPkgMoldOuterROISetting();
            CheckPkgMoldInnerROISetting();
        }

        private void btn_MoldFlashThreshold_Click(object sender, EventArgs e)
        {
            
            int intThreshold = m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold = intThreshold;
            else
            {
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;

                lbl_MoldFlashThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_MoldFlashMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intMoldFlashMinArea = Convert.ToInt32(txt_MoldFlashMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAll_Chip_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_Chip", chk_SetToAll_Chip.Checked);

            CheckChipInwardROISetting();
            CheckChipOutwardROISetting();
        }

        private void txt_StartPixelFromEdge_Chip_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge_Chip.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge_Chip.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip.ToString();
                    return;
                }
            }



            if (chk_SetToAll_Chip.Checked)
            {
                txt_StartPixelFromBottom_Chip.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_Chip.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_Chip.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            CheckChipInwardROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_Chip_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRight_Chip.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight_Chip.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip.ToString();
                    return;
                }
            }

            if (chk_SetToAll_Chip.Checked)
            {
                txt_StartPixelFromEdge_Chip.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_Chip.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_Chip.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            CheckChipInwardROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_Chip_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottom_Chip.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom_Chip.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip.ToString();
                    return;
                }
            }



            if (chk_SetToAll_Chip.Checked)
            {
                txt_StartPixelFromEdge_Chip.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_Chip.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeft_Chip.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            CheckChipInwardROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_Chip_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeft_Chip.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft_Chip.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip.ToString();
                    return;
                }
            }

            if (chk_SetToAll_Chip.Checked)
            {
                txt_StartPixelFromEdge_Chip.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRight_Chip.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottom_Chip.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            CheckChipInwardROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void tp_DarkField_Click(object sender, EventArgs e)
        {

        }

        private void btn_BrightThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageObjectBuilded = false;   // hide blobs drawing

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intThreshold = m_smVisionInfo.g_intThresholdValue = objPackage.ref_intBrightFieldLowThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                objPackage.ref_intBrightFieldLowThreshold = intThreshold;
            else
            {
                objPackage.ref_intBrightFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
                {
                    objPackage.ref_intChipView1Threshold = m_smVisionInfo.g_intThresholdValue;
                    objPackage.ref_intMoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;
                }
                lbl_BrightThreshold.Text = objPackage.ref_intBrightFieldLowThreshold.ToString();
            }

            objThresholdForm.Dispose();

            //objPackage.BuildPackageViewObjects(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            //m_smVisionInfo.g_blnViewPackageObjectBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DarkThreshold_Click(object sender, EventArgs e)
        {
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intDarkFieldLowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intDarkFieldHighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intDarkFieldLowThreshold = intLowThreshold;
                objPackage.ref_intDarkFieldHighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intDarkFieldLowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intDarkFieldHighThreshold = m_smVisionInfo.g_intHighThresholdValue;

                lbl_DarkLowThreshold.Text = objPackage.ref_intDarkFieldLowThreshold.ToString();
                lbl_DarkHighThreshold.Text = objPackage.ref_intDarkFieldHighThreshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_Dark2Threshold_Click(object sender, EventArgs e)
        {
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intDarkField2LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intDarkField2HighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intDarkFieldLowThreshold = intLowThreshold;
                objPackage.ref_intDarkFieldHighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intDarkField2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intDarkField2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;

                lbl_Dark2LowThreshold.Text = objPackage.ref_intDarkField2LowThreshold.ToString();
                lbl_Dark2HighThreshold.Text = objPackage.ref_intDarkField2HighThreshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Dark3Threshold_Click(object sender, EventArgs e)
        {
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intDarkField3LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intDarkField3HighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intDarkFieldLowThreshold = intLowThreshold;
                objPackage.ref_intDarkFieldHighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intDarkField3LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intDarkField3HighThreshold = m_smVisionInfo.g_intHighThresholdValue;

                lbl_Dark3LowThreshold.Text = objPackage.ref_intDarkField3LowThreshold.ToString();
                lbl_Dark3HighThreshold.Text = objPackage.ref_intDarkField3HighThreshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Dark4Threshold_Click(object sender, EventArgs e)
        {
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intDarkField4LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intDarkField4HighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intDarkFieldLowThreshold = intLowThreshold;
                objPackage.ref_intDarkFieldHighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intDarkField4LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intDarkField4HighThreshold = m_smVisionInfo.g_intHighThresholdValue;

                lbl_Dark4LowThreshold.Text = objPackage.ref_intDarkField4LowThreshold.ToString();
                lbl_Dark4HighThreshold.Text = objPackage.ref_intDarkField4HighThreshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark2MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_Dark2MinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intDarkField2MinArea = Convert.ToInt32(txt_Dark2MinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark3MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_Dark3MinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intDarkField3MinArea = Convert.ToInt32(txt_Dark3MinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Dark4MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_Dark4MinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intDarkField4MinArea = Convert.ToInt32(txt_Dark4MinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_DarkMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_DarkMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intDarkFieldMinArea = Convert.ToInt32(txt_DarkMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BrightMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_BrightMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intBrightFieldMinArea = Convert.ToInt32(txt_BrightMinArea.Text);
            }

            //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].BuildMarkViewObjects(
            //    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_GaugeAdvanceSetting2_Click(object sender, EventArgs e)
        {
            m_objAdvancedRectGaugeForm = new AdvancedRectGaugeM4LForm(m_smVisionInfo, m_strFolderPath + "Gauge2M4L.xml", m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);

            m_objAdvancedRectGaugeForm.StartPosition = FormStartPosition.Manual;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            m_objAdvancedRectGaugeForm.Location = new Point(resolution.Width - m_objAdvancedRectGaugeForm.Size.Width, resolution.Height - m_objAdvancedRectGaugeForm.Size.Height);
            m_objAdvancedRectGaugeForm.Show();

            btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_GaugeAdvanceSetting.Enabled = btn_GaugeAdvanceSetting2.Enabled = group_GaugeMeasureResult.Enabled = false;
        }

        private void txt_ImageGain1_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fImageGain = Convert.ToSingle(txt_ImageGain1.Value);

            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
            {
                m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fGainValue = fImageGain * 1000;
            }

            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
            }

            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ImageGain2_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fImageGain = Convert.ToSingle(txt_ImageGain2.Value);

            for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
            {
                m_smVisionInfo.g_arrPackageGauge2M4L[i].ref_fGainValue = fImageGain * 1000;
            }

            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
            }

            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void tp_Step1_Click(object sender, EventArgs e)
        {

        }

        private void btn_UndoROI_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrPackageROIs.Count; j++)
            {
                if (m_smVisionInfo.g_arrPackageROIs[j][0].GetROIHandle())
                {
                    intSelectedROI = j;
                    break;
                }
            }

            if (m_smVisionInfo.g_arrPackageROIs[intSelectedROI].Count > 3)
            {
                m_smVisionInfo.g_arrPackageROIs[intSelectedROI].RemoveAt(m_smVisionInfo.g_arrPackageROIs[intSelectedROI].Count - 1);
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void cbo_DontCareAreaDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_smVisionInfo.g_arrPolygon_Package.Count == 0)
                return;


            //m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;


            if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
            {
                pnl_DontCareFreeShape.Visible = true;
            }
            else
            {
                pnl_DontCareFreeShape.Visible = false;
            }
        }

        private void btn_Undo_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawFreeShapeDone = true;
            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType][m_smVisionInfo.g_intSelectedDontCareROIIndex].UndoPolygon();
            if (chk_BrightAndDarkSameDontCareArea.Checked)
            {
                if (m_smVisionInfo.g_intSelectedType == 0)
                    m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1][m_smVisionInfo.g_intSelectedDontCareROIIndex].UndoPolygon();
                else
                    m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0][m_smVisionInfo.g_intSelectedDontCareROIIndex].UndoPolygon();
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
            {
                m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }
           
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
            {
                m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }
          
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowDraggingBox_2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
            {
                m_smVisionInfo.g_arrPackageGauge2M4L[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox_2.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
            {
                m_smVisionInfo.g_arrPackageGauge2M4L[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints_2.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void DefineMostSelectedImageNo()
        {
            int[] intCountImage = { 0, 0, 0, 0, 0 };

            int[] arrPackageSizeImageIndex = { };

            if (m_smVisionInfo.g_intLearnStepNo == 0)
            {
               arrPackageSizeImageIndex = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeImageNoList(); 
            }
            else if (m_smVisionInfo.g_intLearnStepNo == 1)
            {
                arrPackageSizeImageIndex = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeImageNoList();
            }

            for (int j = 0; j < 4; j++)
            {
                switch (arrPackageSizeImageIndex[j])
                {
                    case 0:
                        intCountImage[0]++;
                        break;
                    case 1:
                        intCountImage[1]++;
                        break;
                    case 2:
                        intCountImage[2]++;
                        break;
                    case 3:
                        intCountImage[3]++;
                        break;
                    case 4:
                        intCountImage[4]++;
                        break;
                }
            }


            int intMostOccurValue = Math.Max(Math.Max(Math.Max(intCountImage[0], intCountImage[1]), Math.Max(intCountImage[2], intCountImage[3])), intCountImage[4]);
            m_smVisionInfo.g_intSelectedImage = Array.IndexOf(intCountImage, intMostOccurValue);
            if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrImages.Count)
                m_smVisionInfo.g_intSelectedImage = 0;

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
        }

        private void chk_BrightAndDarkSameDontCareArea_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // 2020 03 30 - CCENG: Dont save during change, user may cancel the form.
            //RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            //RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            //subkey.SetValue("BrightAndDarkSameDontCareArea_Package", chk_BrightAndDarkSameDontCareArea.Checked);
            m_smVisionInfo.g_blnUseSameDontCareForBrightAndDark = chk_BrightAndDarkSameDontCareArea.Checked;

            if(m_smVisionInfo.g_arrPolygon_Package.Count > 0)
            {
                if (m_smVisionInfo.g_arrPolygon_Package[0].Count < 2)
                {
                    m_smVisionInfo.g_arrPolygon_Package[0].Clear();
                    return;
                }
                if (chk_BrightAndDarkSameDontCareArea.Checked)
                {
                    if (m_smVisionInfo.g_intSelectedType == 0)
                    {
                        if (SRMMessageBox.Show("Bright Field Don't Care ROI will overwrite Dark Field Don’t Care ROI. Do you want to continue?",
                            "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            chk_BrightAndDarkSameDontCareArea.Checked = false;
                            m_smVisionInfo.g_blnUseSameDontCareForBrightAndDark = chk_BrightAndDarkSameDontCareArea.Checked;
                            return;
                        }

                        m_smVisionInfo.g_arrPolygon_Package[0][1].Clear();
                        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Package[0][0].Count; i++)
                        {
                            //m_smVisionInfo.g_arrPolygon_Package[0][1][i].ClearPolygon();
                            m_smVisionInfo.g_arrPolygon_Package[0][1].Add(new Polygon());
                            m_smVisionInfo.g_arrPolygon_Package[0][m_smVisionInfo.g_intSelectedType][i].CopyAllTo(m_smVisionInfo.g_arrPolygon_Package[0][1][i]);
                        }

                        m_smVisionInfo.g_arrPackageDontCareROIs[1].Clear();
                        for (int i = 0; i < m_smVisionInfo.g_arrPackageDontCareROIs[0].Count; i++)
                        {
                            m_smVisionInfo.g_arrPackageDontCareROIs[1].Add(new ROI());
                            m_smVisionInfo.g_arrPackageDontCareROIs[1][i].LoadROISetting(
                                                        m_smVisionInfo.g_arrPackageDontCareROIs[0][i].ref_ROIPositionX,
                                                        m_smVisionInfo.g_arrPackageDontCareROIs[0][i].ref_ROIPositionY,
                                                        m_smVisionInfo.g_arrPackageDontCareROIs[0][i].ref_ROIWidth,
                                                        m_smVisionInfo.g_arrPackageDontCareROIs[0][i].ref_ROIHeight);
                            m_smVisionInfo.g_arrPackageDontCareROIs[1][i].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                        }
                    }
                    else
                    {
                        if (SRMMessageBox.Show("Dark Field Don't Care ROI will overwrite Bright Field Don’t Care ROI. Do you want to continue?",
                            "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            chk_BrightAndDarkSameDontCareArea.Checked = false;
                            m_smVisionInfo.g_blnUseSameDontCareForBrightAndDark = chk_BrightAndDarkSameDontCareArea.Checked;
                            return;
                        }

                        m_smVisionInfo.g_arrPolygon_Package[0][0].Clear();
                        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Package[0][1].Count; i++)
                        {
                            //m_smVisionInfo.g_arrPolygon_Package[0][0][i].ClearPolygon();
                            m_smVisionInfo.g_arrPolygon_Package[0][0].Add(new Polygon());
                            m_smVisionInfo.g_arrPolygon_Package[0][m_smVisionInfo.g_intSelectedType][i].CopyAllTo(m_smVisionInfo.g_arrPolygon_Package[0][0][i]);
                        }

                        m_smVisionInfo.g_arrPackageDontCareROIs[0].Clear();
                        for (int i = 0; i < m_smVisionInfo.g_arrPackageDontCareROIs[1].Count; i++)
                        {
                            m_smVisionInfo.g_arrPackageDontCareROIs[0].Add(new ROI());
                            m_smVisionInfo.g_arrPackageDontCareROIs[0][i].LoadROISetting(
                                                        m_smVisionInfo.g_arrPackageDontCareROIs[1][i].ref_ROIPositionX,
                                                        m_smVisionInfo.g_arrPackageDontCareROIs[1][i].ref_ROIPositionY,
                                                        m_smVisionInfo.g_arrPackageDontCareROIs[1][i].ref_ROIWidth,
                                                        m_smVisionInfo.g_arrPackageDontCareROIs[1][i].ref_ROIHeight);
                            m_smVisionInfo.g_arrPackageDontCareROIs[0][i].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                        }
                    }
                }
            }
            txt_DunCareROIBright.Text = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count.ToString();
            txt_DunCareROIDark.Text = m_smVisionInfo.g_arrPackageDontCareROIs[1].Count.ToString();
        }

        private void chk_ReferTempleteSize_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnReferTemplateSize = chk_ProductionTest.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_AddDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count == 0)
            {
                m_smVisionInfo.g_arrPackageDontCareROIs.Add(new List<ROI>());
            }
            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count == 1)
            {
                m_smVisionInfo.g_arrPackageDontCareROIs.Add(new List<ROI>());
            }
            
            m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Add(new ROI());
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count - 1;
            m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType][m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count - 1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            if (m_smVisionInfo.g_arrPolygon_Package.Count == 0)
                m_smVisionInfo.g_arrPolygon_Package.Add(new List<List<Polygon>>());

            if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count == 0)
                m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Add(new List<Polygon>());
            if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count == 1)
                m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Add(new List<Polygon>());
            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType].Add(new Polygon());
            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType][m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType].Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;

            if (chk_BrightAndDarkSameDontCareArea.Checked)
            {
                if (m_smVisionInfo.g_intSelectedType == 0)
                {
                    m_smVisionInfo.g_arrPackageDontCareROIs[1].Add(new ROI());
                    m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageDontCareROIs[1].Count - 1;
                    m_smVisionInfo.g_arrPackageDontCareROIs[1][m_smVisionInfo.g_arrPackageDontCareROIs[1].Count - 1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                    if (m_smVisionInfo.g_arrPolygon_Package.Count == 0)
                        m_smVisionInfo.g_arrPolygon_Package.Add(new List<List<Polygon>>());

                    if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count == 0)
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Add(new List<Polygon>());
                    if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count == 1)
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Add(new List<Polygon>());
                    m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1].Add(new Polygon());
                    m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1][m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][1].Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
                }
                else
                {
                    m_smVisionInfo.g_arrPackageDontCareROIs[0].Add(new ROI());
                    m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count - 1;
                    m_smVisionInfo.g_arrPackageDontCareROIs[0][m_smVisionInfo.g_arrPackageDontCareROIs[0].Count - 1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                    if (m_smVisionInfo.g_arrPolygon_Package.Count == 0)
                        m_smVisionInfo.g_arrPolygon_Package.Add(new List<List<Polygon>>());

                    if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count == 0)
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Add(new List<Polygon>());
                    if (m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count == 1)
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Add(new List<Polygon>());
                    m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0].Add(new Polygon());
                    m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0][m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][0].Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
                }
            }

            if (!chk_BrightAndDarkSameDontCareArea.Checked)
            {
                if (radio_Bright.Checked)
                    txt_DunCareROIBright.Text = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count.ToString();
                else
                    txt_DunCareROIDark.Text = m_smVisionInfo.g_arrPackageDontCareROIs[1].Count.ToString();
            }
            else
            {
                txt_DunCareROIBright.Text = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count.ToString();
                txt_DunCareROIDark.Text = m_smVisionInfo.g_arrPackageDontCareROIs[1].Count.ToString();
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DeleteDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count == 0)
                return;

            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count <= m_smVisionInfo.g_intSelectedType)
                return;

            if (m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count == 0)
                return;

            m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);

            if (chk_BrightAndDarkSameDontCareArea.Checked)
            {
                if (m_smVisionInfo.g_intSelectedType == 0)
                {
                    if (m_smVisionInfo.g_arrPolygon_Package[0][1].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                        m_smVisionInfo.g_arrPolygon_Package[0][1].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
                    if (m_smVisionInfo.g_arrPackageDontCareROIs[1].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                        m_smVisionInfo.g_arrPackageDontCareROIs[1].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);

                }
                else
                {
                    if (m_smVisionInfo.g_arrPolygon_Package[0][0].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                        m_smVisionInfo.g_arrPolygon_Package[0][0].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
                    if (m_smVisionInfo.g_arrPackageDontCareROIs[0].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                        m_smVisionInfo.g_arrPackageDontCareROIs[0].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);

                }

            }

            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count - 1;
            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if ((m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                {
                    if (m_smVisionInfo.g_arrPolygon_Package.Count > m_smVisionInfo.g_intSelectedUnit &&
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedType &&
                        m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                    {
                        cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Package[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedType][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
                    }
                }
                //else
                //    cbo_DontCareAreaDrawMethod.SelectedIndex = 0; // 2020 03 30 - CCENG: remain Dont Care Area Draw Method

                if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
                {
                    pnl_DontCareFreeShape.Visible = true;
                }
                else
                {
                    pnl_DontCareFreeShape.Visible = false;
                }
            }

            if (!chk_BrightAndDarkSameDontCareArea.Checked)
            {
                if (radio_Bright.Checked)
                    txt_DunCareROIBright.Text = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count.ToString();
                else
                    txt_DunCareROIDark.Text = m_smVisionInfo.g_arrPackageDontCareROIs[1].Count.ToString();
            }
            else
            {
                txt_DunCareROIBright.Text = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count.ToString();
                txt_DunCareROIDark.Text = m_smVisionInfo.g_arrPackageDontCareROIs[1].Count.ToString();
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_PackageGrayValueSensitivitySetting_Click(object sender, EventArgs e)
        {
            GrayValueSensitivitySettingForm objForm = new GrayValueSensitivitySettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, 1);
            objForm.ShowDialog();
        }

        private void txt_StartPixelExtendFromEdge_Chip_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_StartPixelExtendFromEdge_Chip.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_StartPixelExtendFromEdge_Chip.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_StartPixelExtendFromEdge_Chip.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if (fStartPixelExtendFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelExtendFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelExtendFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip.ToString();
                    return;
                }
            }



            if (chk_SetToAll_Chip.Checked)
            {
                txt_StartPixelExtendFromBottom_Chip.Text = fStartPixelExtendFromEdge.ToString();
                txt_StartPixelExtendFromRight_Chip.Text = fStartPixelExtendFromEdge.ToString();
                txt_StartPixelExtendFromLeft_Chip.Text = fStartPixelExtendFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            CheckChipOutwardROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelExtendFromRight_Chip_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_StartPixelExtendFromRight_Chip.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_StartPixelExtendFromRight_Chip.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_StartPixelExtendFromRight_Chip.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if (fStartPixelExtendFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelExtendFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelExtendFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip.ToString();
                    return;
                }
            }

            if (chk_SetToAll_Chip.Checked)
            {
                txt_StartPixelExtendFromEdge_Chip.Text = fStartPixelExtendFromEdge.ToString();
                txt_StartPixelExtendFromLeft_Chip.Text = fStartPixelExtendFromEdge.ToString();
                txt_StartPixelExtendFromBottom_Chip.Text = fStartPixelExtendFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            CheckChipOutwardROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelExtendFromBottom_Chip_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_StartPixelExtendFromBottom_Chip.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_StartPixelExtendFromBottom_Chip.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_StartPixelExtendFromBottom_Chip.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if (fStartPixelExtendFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelExtendFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelExtendFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip.ToString();
                    return;
                }
            }



            if (chk_SetToAll_Chip.Checked)
            {
                txt_StartPixelExtendFromEdge_Chip.Text = fStartPixelExtendFromEdge.ToString();
                txt_StartPixelExtendFromRight_Chip.Text = fStartPixelExtendFromEdge.ToString();
                txt_StartPixelExtendFromLeft_Chip.Text = fStartPixelExtendFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            CheckChipOutwardROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelExtendFromLeft_Chip_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_StartPixelExtendFromLeft_Chip.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_StartPixelExtendFromLeft_Chip.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_StartPixelExtendFromLeft_Chip.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if (fStartPixelExtendFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelExtendFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelExtendFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip.ToString();
                    return;
                }
            }

            if (chk_SetToAll_Chip.Checked)
            {
                txt_StartPixelExtendFromEdge_Chip.Text = fStartPixelExtendFromEdge.ToString();
                txt_StartPixelExtendFromRight_Chip.Text = fStartPixelExtendFromEdge.ToString();
                txt_StartPixelExtendFromBottom_Chip.Text = fStartPixelExtendFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    if (chk_SetToAll_Chip.Checked)
                    {

                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    }

                    if (chk_SetToBrightDark_Chip.Checked)
                    {
                        m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        if (chk_SetToAll_Chip.Checked)
                        {

                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                            m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            CheckChipOutwardROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToBrightDark_Chip_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToBrightDark_Chip", chk_SetToBrightDark_Chip.Checked);

            CheckChipInwardROISetting();
            CheckChipOutwardROISetting();
        }

        private void cbo_ChippedOffDefectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnIgnoreComboboxIndexChange)
            {
                m_blnIgnoreComboboxIndexChange = false;
                return;
            }

            if (!IsChipInwardOutwardSettingCorrect(true))
            {
                m_blnIgnoreComboboxIndexChange = true;

                if (cbo_ChippedOffDefectType.SelectedIndex == 0)
                    cbo_ChippedOffDefectType.SelectedIndex = 1;
                else
                    cbo_ChippedOffDefectType.SelectedIndex = 0;

                return;
            }

            if (cbo_ChippedOffDefectType.SelectedIndex == 0)
            {
                m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);
                if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                    m_smVisionInfo.g_intSelectedImage = 0;
                else
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                txt_StartPixelFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip.ToString();
                txt_StartPixelFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip.ToString();
                txt_StartPixelFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip.ToString();
                txt_StartPixelFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip.ToString();

                txt_StartPixelExtendFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip.ToString();
                txt_StartPixelExtendFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip.ToString();
                txt_StartPixelExtendFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip.ToString();
                txt_StartPixelExtendFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip.ToString();

                m_smVisionInfo.g_blnViewChipStartPixelFromEdge = true;
                m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
            }
            else
            {
                m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3), 0);
                if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                    m_smVisionInfo.g_intSelectedImage = 0;
                else
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                txt_StartPixelFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark.ToString();
                txt_StartPixelFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip_Dark.ToString();
                txt_StartPixelFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip_Dark.ToString();
                txt_StartPixelFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip_Dark.ToString();

                txt_StartPixelExtendFromEdge_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark.ToString();
                txt_StartPixelExtendFromRight_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip_Dark.ToString();
                txt_StartPixelExtendFromBottom_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip_Dark.ToString();
                txt_StartPixelExtendFromLeft_Chip.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip_Dark.ToString();

                m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
                m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = true;
            }

            CheckChipInwardROISetting();
            CheckChipOutwardROISetting();
        }
        private void CheckPkgROISetting()
        {
            bool blnTopSame = true, blnBottomSame = true, blnLeftSame = true, blnRightSame = true;

            if (chk_SetToBrightDark.Checked)
            {
                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark)
                    blnTopSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark)
                    blnBottomSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark)
                    blnLeftSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark)
                    blnRightSame = false;
            }

            if (blnTopSame)
            {
                lbl_PkgTop.ForeColor = Color.Black;
            }
            else
            {
                lbl_PkgTop.ForeColor = Color.Red;
            }
            if (blnBottomSame)
            {
                lbl_PkgBottom.ForeColor = Color.Black;
            }
            else
            {
                lbl_PkgBottom.ForeColor = Color.Red;
            }
            if (blnLeftSame)
            {
                lbl_PkgLeft.ForeColor = Color.Black;
            }
            else
            {
                lbl_PkgLeft.ForeColor = Color.Red;
            }
            if (blnRightSame)
            {
                lbl_PkgRight.ForeColor = Color.Black;
            }
            else
            {
                lbl_PkgRight.ForeColor = Color.Red;
            }
        }
        private void CheckPkgDarkROISetting()
        {
            //bool blnSame = true;

            //if (chk_SetToAll_DarkField2.Checked)
            //{
            //    if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2 != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2 != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2 != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2))
            //        blnSame = false;
            //}

            //if (blnSame)
            //{
            //    lbl_PkgDarkTop.ForeColor = Color.Black;
            //    lbl_PkgDarkRight.ForeColor = Color.Black;
            //    lbl_PkgDarkBottom.ForeColor = Color.Black;
            //    lbl_PkgDarkLeft.ForeColor = Color.Black;
            //}
            //else
            //{
            //    lbl_PkgDarkTop.ForeColor = Color.Red;
            //    lbl_PkgDarkRight.ForeColor = Color.Red;
            //    lbl_PkgDarkBottom.ForeColor = Color.Red;
            //    lbl_PkgDarkLeft.ForeColor = Color.Red;
            //}

        }
        private void CheckChipInwardROISetting()
        {
            bool blnSame = true;
            bool blnBrightDarkTopSame = true;
            bool blnBrightDarkRightSame = true;
            bool blnBrightDarkBottomSame = true;
            bool blnBrightDarkLeftSame = true;
            //if (chk_SetToAll_Chip.Checked)
            //{
            //    if (cbo_ChippedOffDefectType.SelectedIndex == 0)
            //    {
            //        if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip))
            //            blnSame = false;
            //    }
            //    else
            //    {
            //        if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip_Dark) ||
            //                   (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip_Dark) ||
            //                   (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip_Dark))
            //            blnSame = false;
            //    }
            //}

            if (chk_SetToBrightDark_Chip.Checked)
            {
                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark)
                    blnBrightDarkTopSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip_Dark)
                    blnBrightDarkRightSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip_Dark)
                    blnBrightDarkBottomSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip_Dark)
                    blnBrightDarkLeftSame = false;
            }

            if (blnBrightDarkTopSame)
                lbl_ChipInwardTop.ForeColor = Color.Black;
            else
                lbl_ChipInwardTop.ForeColor = Color.Red;

            if (blnBrightDarkRightSame)
                lbl_ChipInwardRight.ForeColor = Color.Black;
            else
                lbl_ChipInwardRight.ForeColor = Color.Red;

            if (blnBrightDarkBottomSame)
                lbl_ChipInwardBottom.ForeColor = Color.Black;
            else
                lbl_ChipInwardBottom.ForeColor = Color.Red;

            if (blnBrightDarkLeftSame)
                lbl_ChipInwardLeft.ForeColor = Color.Black;
            else
                lbl_ChipInwardLeft.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnBrightDarkTopSame)
                    lbl_ChipInwardTop.ForeColor = Color.Black;
                if (blnBrightDarkRightSame)
                    lbl_ChipInwardRight.ForeColor = Color.Black;
                if (blnBrightDarkBottomSame)
                    lbl_ChipInwardBottom.ForeColor = Color.Black;
                if (blnBrightDarkLeftSame)
                    lbl_ChipInwardLeft.ForeColor = Color.Black;
            }
            else
            {
                lbl_ChipInwardTop.ForeColor = Color.Red;
                lbl_ChipInwardRight.ForeColor = Color.Red;
                lbl_ChipInwardBottom.ForeColor = Color.Red;
                lbl_ChipInwardLeft.ForeColor = Color.Red;
            }

        }
        private void CheckChipOutwardROISetting()
        {
            bool blnSame = true;
            bool blnBrightDarkTopSame = true;
            bool blnBrightDarkRightSame = true;
            bool blnBrightDarkBottomSame = true;
            bool blnBrightDarkLeftSame = true;
            //if (chk_SetToAll_Chip.Checked)
            //{
            //    if (cbo_ChippedOffDefectType.SelectedIndex == 0)
            //    {
            //        if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip))
            //            blnSame = false;
            //    }
            //    else
            //    {
            //        if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip_Dark) ||
            //                   (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip_Dark) ||
            //                   (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip_Dark))
            //            blnSame = false;
            //    }
            //}

            if (chk_SetToBrightDark_Chip.Checked)
            {
                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark)
                    blnBrightDarkTopSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip_Dark)
                    blnBrightDarkRightSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip_Dark)
                    blnBrightDarkBottomSame = false;

                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip_Dark)
                    blnBrightDarkLeftSame = false;
            }

            if (blnBrightDarkTopSame)
                lbl_ChipOutwardTop.ForeColor = Color.Black;
            else
                lbl_ChipOutwardTop.ForeColor = Color.Red;

            if (blnBrightDarkRightSame)
                lbl_ChipOutwardRight.ForeColor = Color.Black;
            else
                lbl_ChipOutwardRight.ForeColor = Color.Red;

            if (blnBrightDarkBottomSame)
                lbl_ChipOutwardBottom.ForeColor = Color.Black;
            else
                lbl_ChipOutwardBottom.ForeColor = Color.Red;

            if (blnBrightDarkLeftSame)
                lbl_ChipOutwardLeft.ForeColor = Color.Black;
            else
                lbl_ChipOutwardLeft.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnBrightDarkTopSame)
                    lbl_ChipOutwardTop.ForeColor = Color.Black;
                if (blnBrightDarkRightSame)
                    lbl_ChipOutwardRight.ForeColor = Color.Black;
                if (blnBrightDarkBottomSame)
                    lbl_ChipOutwardBottom.ForeColor = Color.Black;
                if (blnBrightDarkLeftSame)
                    lbl_ChipOutwardLeft.ForeColor = Color.Black;
            }
            else
            {
                lbl_ChipOutwardTop.ForeColor = Color.Red;
                lbl_ChipOutwardRight.ForeColor = Color.Red;
                lbl_ChipOutwardBottom.ForeColor = Color.Red;
                lbl_ChipOutwardLeft.ForeColor = Color.Red;
            }

        }
        private void CheckPkgMoldOuterROISetting()
        {
            //bool blnSame = true;

            //if (chk_SetToAll_Mold.Checked)
            //{
            //    if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold))
            //        blnSame = false;
            //}

            //if (blnSame)
            //{
            //    lbl_StartPixelFromEdgeOuter_Mold.ForeColor = Color.Black;
            //    lbl_StartPixelFromRightOuter_Mold.ForeColor = Color.Black;
            //    lbl_StartPixelFromBottomOuter_Mold.ForeColor = Color.Black;
            //    lbl_StartPixelFromLeftOuter_Mold.ForeColor = Color.Black;
            //}
            //else
            //{
            //    lbl_StartPixelFromEdgeOuter_Mold.ForeColor = Color.Red;
            //    lbl_StartPixelFromRightOuter_Mold.ForeColor = Color.Red;
            //    lbl_StartPixelFromBottomOuter_Mold.ForeColor = Color.Red;
            //    lbl_StartPixelFromLeftOuter_Mold.ForeColor = Color.Red;
            //}

        }
        private void CheckPkgMoldInnerROISetting()
        {
            bool blnSame = true;

            if (chk_SetToAll_Mold.Checked)
            {
                if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold) ||
                            (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold) ||
                            (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold))
                    blnSame = false;
            }

            if (blnSame)
            {
                lbl_StartPixelFromEdgeInner_Mold.ForeColor = Color.Black;
                lbl_StartPixelFromRightInner_Mold.ForeColor = Color.Black;
                lbl_StartPixelFromBottomInner_Mold.ForeColor = Color.Black;
                lbl_StartPixelFromLeftInner_Mold.ForeColor = Color.Black;
            }
            else
            {
                lbl_StartPixelFromEdgeInner_Mold.ForeColor = Color.Red;
                lbl_StartPixelFromRightInner_Mold.ForeColor = Color.Red;
                lbl_StartPixelFromBottomInner_Mold.ForeColor = Color.Red;
                lbl_StartPixelFromLeftInner_Mold.ForeColor = Color.Red;
            }

        }
        private void StartOrientTest()
        {
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                return;
            
            // make sure template learn
            if (m_smVisionInfo.g_arrOrients[0].Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage += "*Orient : No Template Found";
                return;
            }
            
            // reset all inspection data
            for (int i = 0; i < m_smVisionInfo.g_arrOrients[0].Count; i++)
            {
                ((Orient)m_smVisionInfo.g_arrOrients[0][i]).ResetInspectionData();
            }
            
            int intMatchCount = 0;
            
                float fHighestScore = -1;
                m_smVisionInfo.g_arrMarks[0].ref_blnInspectAllTemplate = true;
                do
                {
                    int intTemplateIndex = (int)((m_smVisionInfo.g_intTemplatePriority >> (0x04 * intMatchCount)) & 0x0F) - 1;
                    if (intTemplateIndex >= 0)
                    {
                        int intAngle;
                        bool blnPreciseAngleResult = true;
                        if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                        {
                            if (((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || ((m_smCustomizeInfo.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                                (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            {
                                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                                intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_intFinalReduction_Direction, m_smVisionInfo.g_intFinalReduction_MarkDeg, true, !m_smVisionInfo.g_blnWantGauge,
                                m_smVisionInfo.g_arrMarks[0].GetMarkAngleTolerance(0, intTemplateIndex),
                                ref blnPreciseAngleResult, false, !m_smVisionInfo.g_blnWhiteOnBlack);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                                
                                //intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                //    m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                            }
                            else
                            {
                                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                                intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection_WithSubMatcher4(m_smVisionInfo.g_arrRotatedImages[0],
                                            m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrOrientROIs[0][1], m_smVisionInfo.g_arrOrientROIs[0][2], 2);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                                
                            }

                            if (m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore() > fHighestScore)
                            {
                                fHighestScore = m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore();
                             
                                switch (intAngle)
                                {
                                    default:
                                    case 0:
                                        m_intOrientAngle = 0;
                                        break;
                                    case 1:
                                        m_intOrientAngle = 90;
                                        break;
                                    case 2:
                                        m_intOrientAngle = 180;
                                        break;
                                    case 3:
                                        m_intOrientAngle = -90;
                                        break;
                                }

                            }
                        }
                    }
                    intMatchCount++;
                    //} while (((fHighestScore < 0.8) && (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count)) || !blnAngleResult); 
                } while (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count);


            if (m_intOrientAngle != 0)
            {
                for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrRotatedImages.Count; intImageIndex++)
                {
                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(m_arrRotatedImage[intImageIndex]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                   (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                    ROI.Rotate0Degree(m_arrRotatedImage[intImageIndex], objRotateROI,
                                              m_intOrientAngle,
                                              ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);
                }

                if (m_smVisionInfo.g_blnViewColorImage)
                {
                    for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorRotatedImages.Count; intImageIndex++)
                    {
                        CROI objRotateROI = new CROI();
                        objRotateROI.AttachImage(m_arrColorRotatedImage[intImageIndex]);
                        objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                       (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                       m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                       m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                        CROI.Rotate0Degree(m_arrColorRotatedImage[intImageIndex], objRotateROI,
                                                  m_intOrientAngle, 8,
                                                  ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);
                    }
                }

            }
        }
        private void StartPin1Test_WithOrientation()
        {
            // make sure template learn
            if (m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting.Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage += "*Pin1 : No Template Found";

                return;
            }
            int intAngle = 0;
            m_smVisionInfo.g_arrPin1[0].ResetInspectionData();
            m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrMarks[0].ref_intPin1ImageNo]);
            float ResultX = 0;
            float ResultY = 0;
            int intMatchCount = 0;
            bool blnResult;
            string strErrorMessage = "";
            int intTemplateIndex;
            // Single template test
            if (!m_smVisionInfo.g_blnInspectAllTemplate)
            {
                intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
                if (m_smVisionInfo.g_arrPin1[0].ref_objTestROI == null)
                    m_smVisionInfo.g_arrPin1[0].ref_objTestROI = new ROI();

                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.AttachImage(m_smVisionInfo.g_arrOrientROIs[0][0]);
                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                    (int)(m_smVisionInfo.g_fOrientCenterX[0] -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(intTemplateIndex) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetX(intTemplateIndex)),
                    (int)(m_smVisionInfo.g_fOrientCenterY[0] -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(intTemplateIndex) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetY(intTemplateIndex)),
                    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(intTemplateIndex) * 2,
                    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(intTemplateIndex) * 2);

                m_smVisionInfo.g_arrPin1[0].ref_blnFinalResultPassFail = m_smVisionInfo.g_arrPin1[0].MatchWithTemplate(m_smVisionInfo.g_arrPin1[0].ref_objTestROI, m_smVisionInfo.g_intSelectedTemplate);
                m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate = intTemplateIndex;
                strErrorMessage = m_smVisionInfo.g_arrPin1[0].ref_strErrorMessage;
            }
            else // Whole active templates test
            {
                float fHighestScore = 0;
                do
                {
                    //float fHighestScore = 0; // 2020-04-15 ZJYEOH : move outside the loop so that highest score will not reset
                    intTemplateIndex = (int)((m_smVisionInfo.g_intTemplatePriority >> (0x04 * intMatchCount)) & 0x0F) - 1;
                    if (intTemplateIndex >= 0)
                    {
                        if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_intDirections; i++)
                            {
                                if (m_smVisionInfo.g_arrPin1[0].ref_objTestROI == null)
                                    m_smVisionInfo.g_arrPin1[0].ref_objTestROI = new ROI();

                                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.AttachImage(m_smVisionInfo.g_arrOrientROIs[0][0]);
                                float RotatedOrientCenterX = m_smVisionInfo.g_fOrientCenterX[0];
                                float RotatedOrientCenterY = m_smVisionInfo.g_fOrientCenterY[0];
                                float CenterX, CenterY;
                                float fSizeX, fSizeY;

                                if (m_smVisionInfo.g_blnWantGauge)
                                {
                                    CenterX = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX;
                                    CenterY = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY;
                                    if ((m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectWidth + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) > m_smVisionInfo.g_arrImages[0].ref_intImageWidth ||
                                   (m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectHeight + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) > m_smVisionInfo.g_arrImages[0].ref_intImageHeight)
                                    {
                                        fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                                        fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;
                                    }
                                    else
                                    {
                                        fSizeX = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectWidth + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                                        fSizeY = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectHeight + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;
                                    }
                                    fSizeX /= 2;
                                    fSizeY /= 2;
                                }
                                else
                                {

                                    CenterX = (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;
                                    CenterY = (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2;
                                    fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2;
                                    fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2;
                                }

                                //         float RotationAngle = m_intOrientAngle;
                                //         if (RotationAngle == -90)
                                //             RotationAngle = 90;
                                //         else if (RotationAngle == 90)
                                //             RotationAngle = -90;

                                //         RotatedOrientCenterX = (float)((CenterX) + ((m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Cos(RotationAngle * Math.PI / 180)) -
                                //((m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Sin(RotationAngle * Math.PI / 180)));
                                //         RotatedOrientCenterY = (float)((CenterY) + ((m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Sin(RotationAngle * Math.PI / 180)) +
                                //          ((m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Cos(RotationAngle * Math.PI / 180)));

                                //m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                                //    (int)(Math.Abs(RotatedOrientCenterX) -
                                //    m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(intTemplateIndex) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetX(intTemplateIndex)),
                                //    (int)(Math.Abs(RotatedOrientCenterY) -
                                //    m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(intTemplateIndex) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetY(intTemplateIndex)),
                                //    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(intTemplateIndex) * 2,
                                //    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(intTemplateIndex) * 2);
                                int intStartX = 0;
                                int intStartY = 0;
                                if (i == 0)
                                {
                                    if (m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[intTemplateIndex].blnTemplateOrientationIsLeft)
                                        intStartX = (int)(CenterX - fSizeX);
                                    else
                                        intStartX = (int)(CenterX);

                                    if (m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[intTemplateIndex].blnTemplateOrientationIsTop)
                                        intStartY = (int)(CenterY - fSizeY);
                                    else
                                        intStartY = (int)(CenterY);

                                    m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                                      intStartX,
                                      intStartX,
                                      (int)fSizeX,
                                      (int)fSizeY);
                                }
                                else if (i == 1)
                                {
                                    if (!m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[intTemplateIndex].blnTemplateOrientationIsLeft)
                                        intStartX = (int)(CenterX - fSizeX);
                                    else
                                        intStartX = (int)(CenterX);

                                    if (!m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[intTemplateIndex].blnTemplateOrientationIsTop)
                                        intStartY = (int)(CenterY - fSizeY);
                                    else
                                        intStartY = (int)(CenterY);

                                    m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                                      intStartX,
                                      intStartX,
                                      (int)fSizeX,
                                      (int)fSizeY);
                                }
                                else if (i == 2)
                                {
                                    if (!m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[intTemplateIndex].blnTemplateOrientationIsLeft)
                                        intStartX = (int)(CenterX - fSizeX);
                                    else
                                        intStartX = (int)(CenterX);

                                    if (m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[intTemplateIndex].blnTemplateOrientationIsTop)
                                        intStartY = (int)(CenterY - fSizeY);
                                    else
                                        intStartY = (int)(CenterY);

                                    m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                                      intStartX,
                                      intStartX,
                                      (int)fSizeX,
                                      (int)fSizeY);
                                }
                                else if (i == 3)
                                {
                                    if (m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[intTemplateIndex].blnTemplateOrientationIsLeft)
                                        intStartX = (int)(CenterX - fSizeX);
                                    else
                                        intStartX = (int)(CenterX);

                                    if (!m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[intTemplateIndex].blnTemplateOrientationIsTop)
                                        intStartY = (int)(CenterY - fSizeY);
                                    else
                                        intStartY = (int)(CenterY);

                                    m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                                      intStartX,
                                      intStartX,
                                      (int)fSizeX,
                                      (int)fSizeY);
                                }

                                //m_smVisionInfo.g_arrPin1[0].ref_objTestROI.SaveImage("D:\\TestROI.bmp");

                                blnResult = m_smVisionInfo.g_arrPin1[0].MatchWithTemplate(m_smVisionInfo.g_arrPin1[0].ref_objTestROI, intTemplateIndex, fHighestScore);

                                if (m_smVisionInfo.g_arrPin1[0].GetResultScore(intTemplateIndex) > 0 && 
                                    m_smVisionInfo.g_arrPin1[0].GetResultScore(intTemplateIndex) > fHighestScore)
                                {
                                    ResultX = m_smVisionInfo.g_arrPin1[0].ref_objTestROI.ref_ROIPositionX + m_smVisionInfo.g_arrPin1[0].GetResultPosX(m_smVisionInfo.g_intSelectedTemplate);
                                    ResultY = m_smVisionInfo.g_arrPin1[0].ref_objTestROI.ref_ROIPositionY + m_smVisionInfo.g_arrPin1[0].GetResultPosY(m_smVisionInfo.g_intSelectedTemplate);
                                    fHighestScore = m_smVisionInfo.g_arrPin1[0].GetResultScore(intTemplateIndex);
                                    m_smVisionInfo.g_arrPin1[0].ref_blnFinalResultPassFail = blnResult;
                                    m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate = intTemplateIndex;
                                    strErrorMessage = m_smVisionInfo.g_arrPin1[0].ref_strErrorMessage;
                                }
                            }
                        }
                    }
                    intMatchCount++;
                } while (intMatchCount < m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting.Count);
            }

            //m_smVisionInfo.g_blnDrawPin1Result = true;

            // 2020 11 20 - CCENG: Method 2 orientation
            float fOrientationSeparatorX = (float)m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2;
            float fOrientationSeparatorY = (float)m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2;

            if (m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate >= 0)
            {
                Orient objOrient = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]];
                if (objOrient.ref_intDirections == 2)
                {
                    bool blnSampleIsTop = ResultY - fOrientationSeparatorY < 0;
                    bool blnDirectionisTop = m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate].blnTemplateOrientationIsTop != blnSampleIsTop;

                    if (blnDirectionisTop)            // rotate 180  from template point
                    {
                        intAngle = 2;
                    }
                    else
                    {
                        intAngle = 0;            // no rotate
                    }
                }
                else
                {
                    bool blnSampleIsLeft = ResultX - fOrientationSeparatorX < 0;
                    bool blnSampleIsTop = ResultY - fOrientationSeparatorY < 0;

                    bool blnDirectionIsLeft = m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate].blnTemplateOrientationIsLeft != blnSampleIsLeft;
                    bool blnDirectionisTop = m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting[m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate].blnTemplateOrientationIsTop != blnSampleIsTop;

                    if (blnDirectionIsLeft && blnDirectionisTop)            // rotate 180  from template point
                    {
                        intAngle = 2;
                    }
                    else if (blnDirectionIsLeft && !blnDirectionisTop)      // rotate 90 ccw from template point
                    {
                        intAngle = 1;            // show result angle 90 when rotate ccw from template point
                    }
                    else if (!blnDirectionIsLeft && blnDirectionisTop)      // rotate 90 cw from template point
                    {
                        intAngle = 3;            // show result angle -90 when rotate cw from template point
                    }
                    else
                    {
                        intAngle = 0;            // no rotate
                    }
                }
            }

            switch (intAngle)
            {
                default:
                case 0:
                    m_intOrientAngle = 0;
                    break;
                case 1:
                    m_intOrientAngle = 90;
                    break;
                case 2:
                    m_intOrientAngle = 180;
                    break;
                case 3:
                    m_intOrientAngle = -90;
                    break;
            }

            if (m_intOrientAngle != 0)
            {
                for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrRotatedImages.Count; intImageIndex++)
                {
                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(m_arrRotatedImage[intImageIndex]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                   (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                    ROI.Rotate0Degree(m_arrRotatedImage[intImageIndex], objRotateROI,
                                              m_intOrientAngle,
                                              ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);
                }

                if (m_smVisionInfo.g_blnViewColorImage)
                {
                    for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorRotatedImages.Count; intImageIndex++)
                    {
                        CROI objRotateROI = new CROI();
                        objRotateROI.AttachImage(m_arrColorRotatedImage[intImageIndex]);
                        objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                       (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                       m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                       m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                        CROI.Rotate0Degree(m_arrColorRotatedImage[intImageIndex], objRotateROI,
                                                  m_intOrientAngle, 8,
                                                  ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);
                    }
                }

            }
        }
        private void btn_AddMoldFlashDontCareROI_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.Add(new ROI());
            m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.Count - 1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            m_smVisionInfo.g_intSelectedMoldFlashDontCareROIIndex = m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DeleteMoldFlashDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.Count == 0)
                return;

            m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.RemoveAt(m_smVisionInfo.g_intSelectedMoldFlashDontCareROIIndex);
            m_smVisionInfo.g_intSelectedMoldFlashDontCareROIIndex = m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void SaveMoldFlashDontCare()
        {
            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                return;

            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            for (int i = 0; i < m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs.Count; i++)
            {
                m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].ref_intStartOffsetX = (int)Math.Round(fCenterX - m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].ref_ROITotalX);
                m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].ref_intStartOffsetY = (int)Math.Round(fCenterY - m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs[i].ref_ROITotalY);
            }

            ROI.SaveFile(m_strFolderPath + "MoldFlashDontCareROI.xml", m_smVisionInfo.g_arrPackageMoldFlashDontCareROIs);
        }

        private void txt_StartPixelFromEdgeInner_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdgeInner_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdgeInner_Mold.Text == "" /*|| fStartPixelFromEdge < 0*/)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (chk_SetToAll_Mold.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdgeInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_StartPixelFromRightInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_StartPixelFromBottomInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_StartPixelFromLeftInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartY - fStartPixelFromEdge <= SearchROIStartY)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdgeInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Black;
                }

            }



            if (chk_SetToAll_Mold.Checked)
            {
                txt_StartPixelFromBottomInner_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRightInner_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeftInner_Mold.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdgeInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_Mold.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottomInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRightInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeftInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }

            }
            CheckPkgMoldInnerROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRightInner_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRightInner_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRightInner_Mold.Text == "" /*|| fStartPixelFromEdge < 0*/)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll_Mold.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdgeInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_StartPixelFromRightInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_StartPixelFromBottomInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_StartPixelFromLeftInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRightInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Black;
                }

            }

            if (chk_SetToAll_Mold.Checked)
            {
                txt_StartPixelFromEdgeInner_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeftInner_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottomInner_Mold.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRightInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_Mold.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdgeInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeftInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottomInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }

            }
            CheckPkgMoldInnerROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottomInner_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottomInner_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottomInner_Mold.Text == "" /*|| fStartPixelFromEdge < 0*/)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll_Mold.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdgeInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_StartPixelFromRightInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_StartPixelFromBottomInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_StartPixelFromLeftInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottomInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Black;
                }

            }



            if (chk_SetToAll_Mold.Checked)
            {
                txt_StartPixelFromEdgeInner_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRightInner_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromLeftInner_Mold.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottomInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_Mold.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdgeInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRightInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeftInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }

            }
            CheckPkgMoldInnerROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeftInner_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeftInner_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeftInner_Mold.Text == "" /*|| fStartPixelFromEdge < 0*/)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;


            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll_Mold.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdgeInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_StartPixelFromRightInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_StartPixelFromBottomInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_StartPixelFromLeftInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromEdgeInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromRightInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromBottomInner_Mold.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartX - fStartPixelFromEdge <= SearchROIStartX)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeftInner_Mold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Red;
                }
                else
                {
                    txt_StartPixelFromLeftInner_Mold.ForeColor = Color.Black;
                }
            }

            if (chk_SetToAll_Mold.Checked)
            {
                txt_StartPixelFromEdgeInner_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromRightInner_Mold.Text = fStartPixelFromEdge.ToString();
                txt_StartPixelFromBottomInner_Mold.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeftInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll_Mold.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdgeInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRightInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottomInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }

            }
            CheckPkgMoldInnerROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToBrightDark_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToBrightDarkPkg", chk_SetToBrightDark.Checked);

            CheckPkgROISetting();
        }

        private void cbo_PkgDefectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (cbo_PkgDefectType.SelectedIndex == 0)
            {
                m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);
                if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                    m_smVisionInfo.g_intSelectedImage = 0;
                else
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
                txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();

                AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);
            }
            else
            {
                m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3), 0);
                if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                    m_smVisionInfo.g_intSelectedImage = 0;
                else
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark.ToString();
                txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark.ToString();
                txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark.ToString();
                txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark.ToString();

                AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark,
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark);
            }

            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radio_Bright_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedType = 0;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2);
            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count > m_smVisionInfo.g_intSelectedType)
                if (m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count > 0)
                    m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count - 1;

            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count == 0)
                txt_DunCareROIBright.Text = 0.ToString();
            else
                txt_DunCareROIBright.Text = m_smVisionInfo.g_arrPackageDontCareROIs[0].Count.ToString();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radio_Dark_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedType = 1;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);
            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count > m_smVisionInfo.g_intSelectedType)
                if (m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count > 0)
                    m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageDontCareROIs[m_smVisionInfo.g_intSelectedType].Count - 1;

            if (m_smVisionInfo.g_arrPackageDontCareROIs.Count <= 1)
                txt_DunCareROIDark.Text = 0.ToString();
            else
                txt_DunCareROIDark.Text = m_smVisionInfo.g_arrPackageDontCareROIs[1].Count.ToString();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private bool IsChipInwardOutwardSettingCorrect(bool blnShowMessage)
        {
            bool blnSettingOK = true;
            float fInwardLimit = 0;
            if (!float.TryParse(txt_StartPixelFromEdge_Chip.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_StartPixelFromEdge_Chip.BackColor = txt_StartPixelFromEdge_Chip.NormalBackColor = txt_StartPixelFromEdge_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            float fOutwardLimit = 0;
            if (!float.TryParse(txt_StartPixelExtendFromEdge_Chip.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_StartPixelExtendFromEdge_Chip.BackColor = txt_StartPixelExtendFromEdge_Chip.NormalBackColor = txt_StartPixelExtendFromEdge_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_StartPixelFromEdge_Chip.BackColor = txt_StartPixelFromEdge_Chip.NormalBackColor = txt_StartPixelFromEdge_Chip.FocusBackColor = Color.Pink;
                txt_StartPixelExtendFromEdge_Chip.BackColor = txt_StartPixelExtendFromEdge_Chip.NormalBackColor = txt_StartPixelExtendFromEdge_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }


            }
            else
            {
                txt_StartPixelFromEdge_Chip.BackColor = txt_StartPixelFromEdge_Chip.NormalBackColor = txt_StartPixelFromEdge_Chip.FocusBackColor = Color.White;
                txt_StartPixelExtendFromEdge_Chip.BackColor = txt_StartPixelExtendFromEdge_Chip.NormalBackColor = txt_StartPixelExtendFromEdge_Chip.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_StartPixelFromRight_Chip.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_StartPixelFromRight_Chip.BackColor = txt_StartPixelFromRight_Chip.NormalBackColor = txt_StartPixelFromRight_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }


            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_StartPixelExtendFromRight_Chip.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_StartPixelExtendFromRight_Chip.BackColor = txt_StartPixelExtendFromRight_Chip.NormalBackColor = txt_StartPixelExtendFromRight_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }


            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_StartPixelFromRight_Chip.BackColor = txt_StartPixelFromRight_Chip.NormalBackColor = txt_StartPixelFromRight_Chip.FocusBackColor = Color.Pink;
                txt_StartPixelExtendFromRight_Chip.BackColor = txt_StartPixelExtendFromRight_Chip.NormalBackColor = txt_StartPixelExtendFromRight_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }


            }
            else
            {
                txt_StartPixelFromRight_Chip.BackColor = txt_StartPixelFromRight_Chip.NormalBackColor = txt_StartPixelFromRight_Chip.FocusBackColor = Color.White;
                txt_StartPixelExtendFromRight_Chip.BackColor = txt_StartPixelExtendFromRight_Chip.NormalBackColor = txt_StartPixelExtendFromRight_Chip.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_StartPixelFromBottom_Chip.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_StartPixelFromBottom_Chip.BackColor = txt_StartPixelFromBottom_Chip.NormalBackColor = txt_StartPixelFromBottom_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }


            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_StartPixelExtendFromBottom_Chip.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_StartPixelExtendFromBottom_Chip.BackColor = txt_StartPixelExtendFromBottom_Chip.NormalBackColor = txt_StartPixelExtendFromBottom_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }


            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_StartPixelFromBottom_Chip.BackColor = txt_StartPixelFromBottom_Chip.NormalBackColor = txt_StartPixelFromBottom_Chip.FocusBackColor = Color.Pink;
                txt_StartPixelExtendFromBottom_Chip.BackColor = txt_StartPixelExtendFromBottom_Chip.NormalBackColor = txt_StartPixelExtendFromBottom_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }


            }
            else
            {
                txt_StartPixelFromBottom_Chip.BackColor = txt_StartPixelFromBottom_Chip.NormalBackColor = txt_StartPixelFromBottom_Chip.FocusBackColor = Color.White;
                txt_StartPixelExtendFromBottom_Chip.BackColor = txt_StartPixelExtendFromBottom_Chip.NormalBackColor = txt_StartPixelExtendFromBottom_Chip.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_StartPixelFromLeft_Chip.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_StartPixelFromLeft_Chip.BackColor = txt_StartPixelFromLeft_Chip.NormalBackColor = txt_StartPixelFromLeft_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }

            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_StartPixelExtendFromLeft_Chip.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_StartPixelExtendFromLeft_Chip.BackColor = txt_StartPixelExtendFromLeft_Chip.NormalBackColor = txt_StartPixelExtendFromLeft_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }

            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_StartPixelFromLeft_Chip.BackColor = txt_StartPixelFromLeft_Chip.NormalBackColor = txt_StartPixelFromLeft_Chip.FocusBackColor = Color.Pink;
                txt_StartPixelExtendFromLeft_Chip.BackColor = txt_StartPixelExtendFromLeft_Chip.NormalBackColor = txt_StartPixelExtendFromLeft_Chip.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_StartPixelFromLeft_Chip.BackColor = txt_StartPixelFromLeft_Chip.NormalBackColor = txt_StartPixelFromLeft_Chip.FocusBackColor = Color.White;
                txt_StartPixelExtendFromLeft_Chip.BackColor = txt_StartPixelExtendFromLeft_Chip.NormalBackColor = txt_StartPixelExtendFromLeft_Chip.FocusBackColor = Color.White;
            }


            return blnSettingOK;
        }
        private void btn_ColorThreshold_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                m_smVisionInfo.g_intColorThreshold[i] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCopperColor[i];
                m_smVisionInfo.g_intColorTolerance[i] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCopperColorTolerance[i];
            }

            m_smVisionInfo.g_intColorFormat = 1;
            m_objColorMultiThresholdForm = new ColorPackageMultiThresholdForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_blnGaugeResult);//, m_smVisionInfo.g_arrPadColorROIs[m_smVisionInfo.g_intSelectedROI][0]);
            m_objColorMultiThresholdForm.Location = new Point(720, 200);
            m_objColorMultiThresholdForm.Show();
            m_objColorMultiThresholdForm.TopMost = true;
        }

        private void btn_SaveColor_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\";
            SaveControlSettings(strFolderPath + "Template\\");
            SaveColorROISetting(strFolderPath);

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].SaveColorPackage(m_strFolderPath + "Settings.xml", false, "Settings", false);
                else
                    m_smVisionInfo.g_arrPackage[u].SaveColorPackage(m_strFolderPath + "Settings2.xml", false, "Settings", false);

            }
            
            SaveColorDontCareROISetting(strFolderPath);
            SaveColorDontCareSetting(strFolderPath + "\\Template\\");

            LoadROISetting(strFolderPath + "ColorDontCareROI.xml", m_smVisionInfo.g_arrPackageColorDontCareROIs, m_smVisionInfo.g_intUnitsOnImage);
            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPackageROIs[i].Count > 1)
                {
                    if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPackageColorDontCareROIs[i].Count; j++)
                        {
                            for (int k = 0; k < m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j].Count; k++)
                            {
                                m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPackageROIs[i][1]);
                            }
                        }
                    }

                }
            }
            LoadColorROISetting(strFolderPath + "CROI.xml", m_smVisionInfo.g_arrPackageColorROIs, m_smVisionInfo.g_intUnitsOnImage);
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].LoadColorDontCareImage(strFolderPath + "\\Template\\", u);
            }
            this.Close();
            this.Dispose();
        }
        private void LoadControlSettings(string strFolderPath)
        {
            XmlParser objFile;
            // Load Pad Advance Setting
            objFile = new XmlParser(strFolderPath + "ControlSetting.xml");
            objFile.GetFirstSection("ControlSetting");
            m_smVisionInfo.g_intOptionControlMask = objFile.GetValueAsLong("ControlMask", 0);
            m_smVisionInfo.g_intOptionControlMask2 = objFile.GetValueAsInt("ControlMask2", 0);
            m_smVisionInfo.g_intOptionControlMask3 = objFile.GetValueAsInt("ControlMask3", 0);
            m_smVisionInfo.g_intOptionControlMask4 = objFile.GetValueAsInt("ControlMask4", 0);
            m_smVisionInfo.g_intOptionControlMask5 = objFile.GetValueAsInt("ControlMask5", 0);
            m_smVisionInfo.g_intPkgOptionControlMask = objFile.GetValueAsLong("PkgControlMask", 0);
            m_smVisionInfo.g_intPkgOptionControlMask2 = objFile.GetValueAsInt("PkgControlMask2", 0);
            m_smVisionInfo.g_intLeadOptionControlMask = objFile.GetValueAsInt("LeadControlMask", 0);
        }
        private void SaveControlSettings(string strFolderPath)
        {
            XmlParser objFile;
            objFile = new XmlParser(strFolderPath + "ControlSetting.xml");
            objFile.WriteSectionElement("ControlSetting", false);
            objFile.WriteElement1Value("ControlMask", m_smVisionInfo.g_intOptionControlMask, "Control Mask", true);
            objFile.WriteElement1Value("ControlMask2", m_smVisionInfo.g_intOptionControlMask2, "Control Mask 2", true);
            objFile.WriteElement1Value("ControlMask3", m_smVisionInfo.g_intOptionControlMask3, "Control Mask 3", true);
            objFile.WriteElement1Value("ControlMask4", m_smVisionInfo.g_intOptionControlMask4, "Control Mask 4", true);
            objFile.WriteElement1Value("ControlMask5", m_smVisionInfo.g_intOptionControlMask5, "Control Mask 5", true);
            objFile.WriteElement1Value("PkgControlMask", m_smVisionInfo.g_intPkgOptionControlMask, "Package Control Mask", true);
            objFile.WriteElement1Value("PkgControlMask2", m_smVisionInfo.g_intPkgOptionControlMask2, "Package Control Mask 2", true);
            objFile.WriteElement1Value("LeadControlMask", m_smVisionInfo.g_intLeadOptionControlMask, "Lead Control Mask", true);
            objFile.WriteEndElement();
        }
        private void LoadColorROISetting(string strPath, List<List<CROI>> arrROIs, int intROICount)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            CROI objROI;

            for (int i = 0; i < intROICount; i++)
            {
                arrROIs.Add(new List<CROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new CROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
                    //objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    //if (objROI.ref_intType > 1)
                    //{
                    //    objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                    //    objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    //}

                    arrROIs[i].Add(objROI);
                }
            }
        }
        private void SaveColorROISetting(string strFolderPath)
        {

            STDeviceEdit.CopySettingFile(strFolderPath, "CROI.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "CROI.xml", false);

            CROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrPackageColorROIs.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPackageColorROIs[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrPackageColorROIs[t][j];
                    objFile.WriteElement1Value("ROI" + j, "");

                    objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                    objFile.WriteElement2Value("Type", objROI.ref_intType);
                    objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                    objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
                }
            }
            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package Color ROI", m_smProductionInfo.g_strLotID);

        }
        private void SaveColorDontCareROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "ColorDontCareROI.xml", false);

            for (int t = 0; t < m_smVisionInfo.g_arrPackageColorDontCareROIs.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPackageColorDontCareROIs[t].Count; j++)
                {
                    objFile.WriteElement1Value("Color Defect" + t + j, "");

                    for (int k = 0; k < m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j].Count; k++)
                    {
                        objFile.WriteElement2Value("ROI" + k, "");

                        objFile.WriteElement3Value("Name", m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].ref_strROIName);
                        objFile.WriteElement3Value("Type", m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].ref_intType);
                        objFile.WriteElement3Value("PositionX", m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].ref_ROIPositionX);
                        objFile.WriteElement3Value("PositionY", m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].ref_ROIPositionY);
                        objFile.WriteElement3Value("Width", m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].ref_ROIWidth);
                        objFile.WriteElement3Value("Height", m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].ref_ROIHeight);
                        objFile.WriteElement3Value("StartOffsetX", m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].ref_intStartOffsetX);
                        objFile.WriteElement3Value("StartOffsetY", m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].ref_intStartOffsetY);
                        m_smVisionInfo.g_arrPackageColorDontCareROIs[t][j][k].AttachImage(m_smVisionInfo.g_arrPackageROIs[t][1]);

                    }
                }
            }
            objFile.WriteEndElement();


        }
        private void SaveColorDontCareSetting(string strFolderPath)
        {
            ImageDrawing objImage = new ImageDrawing();
            ImageDrawing objFinalImage = new ImageDrawing();
            for (int i = 0; i < m_smVisionInfo.g_arrPackageColorDontCareROIs.Count; i++)
            {
                //Check is the ROI for drawing dont care changed
                for (int j = 0; j < m_smVisionInfo.g_arrPackageColorDontCareROIs[i].Count; j++)
                {
                    m_smVisionInfo.g_objBlackImage.CopyTo(objFinalImage);

                    for (int k = 0; k < m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j].Count; k++)
                    {
                        if (m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].ref_intFormMode != 2)
                            Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPackageROIs[i][1], m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        else
                            Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPackageROIs[i][1], m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                        ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);
                    }
                    objFinalImage.SaveImage(strFolderPath + "ColorDontCareImage" + i.ToString() + "_" + j.ToString() + ".bmp");
                }


            }
            objImage.Dispose();
            objFinalImage.Dispose();
            Polygon.SavePolygon(strFolderPath + "\\ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PackageColor);
        }
    }
}