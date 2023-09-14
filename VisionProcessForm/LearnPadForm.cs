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
using System.IO;
using Microsoft.Win32;
using System.Threading;
namespace VisionProcessForm
{
    public partial class LearnPadForm : Form
    {

        #region Member Variables
        private bool m_blnPreviousPadBackup = false;
        private bool m_blnSaveButtonVisible = true;
        private bool m_blnGaugeButtonVisible = true;
        private bool m_blnRotateButtonVisible = true;
        private bool m_blnPadROIButtonVisible = true;

        private ColorMultiThresholdForm m_objColorMultiThresholdForm;
        private bool m_blnColorThreshold = false;
        private bool m_blnOrientTestDone = false;
        private bool m_blnOrientFail = false;
        private bool m_blnUserRightEnableGaugeSetting = true;

        private int m_intCurrentAngle = 0;
        private int m_intCurrentPreciseDeg = 0;
        /// <summary>
        /// Mode 0:
        /// if chk_CheckGauge is true, all Pads will use gauge to measure edge. Gauge tool display.
        /// if chk_CheckGauge is false, all Pads will use PR. Rotator tool display. 
        /// 
        /// Mode 1:
        /// if chk_CheckGauge is true, all Pads will use gauge to measure edge
        /// if chk_CheckGauge is false, side Pads will use PR, center pad still use gauge. 
        /// </summary>
        private Point m_pTop, m_pRight, m_pBottom, m_pLeft;
        private int[] m_intTopPrev = new int[5] { 0, 0, 0, 0, 0 };
        private int[] m_intRightPrev = new int[5] { 0, 0, 0, 0, 0 };
        private int[] m_intBottomPrev = new int[5] { 0, 0, 0, 0, 0 };
        private int[] m_intLeftPrev = new int[5] { 0, 0, 0, 0, 0 };
        private int m_intLearnType = 0; // 0 : Learn All, 1 : Search ROI, 2 : Package Gauge ROI, 3 : Pad ROI, 4 : Dont Care ROI, 5 : Pin 1 ROI, 6 : Identify Pad, 7 : Identify Pitch
        private int m_intAdvSettingEdgeToolMode = 0;
        private int m_intSelectedDontCareROIIndexPrev = 0;
        private bool m_blnInitDone = false;
        private bool m_blnStepPadDone = false;
        private bool m_blnUpdateTab = false;
        private bool m_blnIdentityPadsDone = false;
        private bool m_blnKeepPrevObject = false;
        private bool m_blnKeepPrevPitchGap = false;
        private bool m_blnJumpToPkgPage = false;
        private bool m_blnDragROIPrev = false;
        private bool m_blnWantGaugeMeasurePkgSizePrev = false;
        private int m_intUserGroup = 5;
        private int m_intDisplayStepNo = 1;
        private int m_intPitchGapSelectedRowIndex = 0;
        private bool m_blnViewInspectionPrev;
        private string m_strSelectedRecipe;
        private string m_strPosition = "";
        private string[] m_strPGRealColName = new string[6];
        private string[] m_strSettingRealColName = new string[10];
        private string[] m_strPGColName = { "column_FromPadNo", "column_ToPadNo", "column_MinSetPitch", "column_MaxSetPitch",
                                            "column_MinSetGap", "column_MaxSetGap" };
        private string[] m_strSettingColName = {"column_NewPadNo", "column_OffSet","column_MinArea","column_MaxArea","column_MinWidth","column_MaxWidth",
                                                "column_MinHeight","column_MaxHeight","column_BrokenArea", "column_BrokenLength"};

        private AdvancedRectGauge4LPadForm m_objAdvancedRectGauge4LForm = null;
        private SavingInProgressForm m_objSaveForm = null;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private DataGridView m_dgdView = new DataGridView();
        private DataGridView m_dgdViewPG = new DataGridView();
        private SetPadGroupNoForm m_objSetPadGroupNoForm;
        private List<ImageDrawing> m_arrCurrentImage = new List<ImageDrawing>();
       
        // Positioning
        private PointF m_pMeasureCenterPoint = new PointF();
        private PointF[] m_arrMeasureCornerPoint = new PointF[4];
        private float m_fMeasureWidth = 0;
        private float m_fMeasureHeight = 0;
        private float m_fMeasureAngle = 0;
        private int m_intVisionType = 0; // 0 = Normal, 1 = Orient, 2 = Color
        #endregion

        public LearnPadForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intLearnType, int intVisionType)
        {
            InitializeComponent();
            m_intVisionType = intVisionType;
            m_intLearnType = intLearnType;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;
   
            DisableField2();
            CustomizeGUI();

            if (m_intVisionType == 0) // Normal
            {
                m_smVisionInfo.g_intLearnStepNo = 0;
                //gBox_PreciseAngle.Visible = false;
                gBox_OrientAngle.Visible = false;
                grpbox_OrientResult.Visible = false;
            }
            else if (m_intVisionType == 1) // Orient
            {
                m_smVisionInfo.g_intLearnStepNo = 0;
                gBox_PreciseAngle.Visible = m_blnRotateButtonVisible;//true
                gBox_OrientAngle.Visible = m_blnRotateButtonVisible;//true
                grpbox_OrientResult.Visible = false;
            }
            else if (m_intVisionType == 2) // Color
            {
                m_smVisionInfo.g_intLearnStepNo = 0;
                gBox_PreciseAngle.Visible = false;
                gBox_OrientAngle.Visible = false;
                grpbox_OrientResult.Visible = false;
            }

            m_smVisionInfo.g_arrImages[0].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, 0);
            m_smVisionInfo.g_arrImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                m_smVisionInfo.g_arrImages[i].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, i);
            }

            if (((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)) //m_intVisionType == 0 &&
            {
                if (OrientInspection())
                {
                    m_blnOrientFail = false;
                    //gBox_PreciseAngle.Visible = true;
                    //gBox_OrientAngle.Visible = true;
                    ////btn_CounterClockWise.Enabled = false;
                    ////btn_ClockWise.Enabled = false;
                    //grpbox_OrientResult.Visible = true;
                }
                //else
                //{
                //    m_blnOrientFail = true;
                //    gBox_PreciseAngle.Visible = true;
                //    gBox_OrientAngle.Visible = true;
                //    grpbox_OrientResult.Visible = true;
                //    btn_OrientSaveClose.Visible = false;
                //    btn_OrientSaveContinue.Visible = true;
                //}
                m_blnOrientTestDone = true;
            }

            switch (m_intLearnType)
            {
                case 0:
                    m_smVisionInfo.g_intLearnStepNo = 0;
                    break;
                case 1:
                    m_smVisionInfo.g_intLearnStepNo = 0;
                    break;
                case 2:
                    m_smVisionInfo.g_intLearnStepNo = 2;
                    break;
                case 3:
                    m_smVisionInfo.g_intLearnStepNo = 2; // step 2 --> 13
                    break;
                case 4:
                    m_smVisionInfo.g_intLearnStepNo = 2; // Step 2 --> 3
                    break;
                case 5:
                    m_smVisionInfo.g_intLearnStepNo = 2; // Step 2 --> 4
                    break;
                case 6:
                    m_smVisionInfo.g_intLearnStepNo = 2; // Step 2 --> 3(if want dont care) --> 5 --> 6
                    break;
                case 7:
                    m_smVisionInfo.g_intLearnStepNo = 2; // Step 2 --> 3(if want dont care) --> 5 --> 6 --> 7
                    break;
            }

            m_smVisionInfo.g_intViewInspectionSetting = 1;

            m_blnInitDone = true;
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Pad";
            string strChild2 = "Learn Page";
            string strChild3 = "Save Button";
            if (m_smVisionInfo.g_strVisionName == "BottomOrientPad" || m_smVisionInfo.g_strVisionName == "BottomOPadPkg")
            {
                strChild1 = "OPad";
                if (m_intVisionType == 0)
                {
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_ROISaveClose.Enabled = false; btn_ROISaveClose.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_GaugeSaveClose.Enabled = false; btn_GaugeSaveClose.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_Save.Enabled = false; btn_Save.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_SaveSubLearn.Enabled = false; btn_SaveSubLearn.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_OrientSaveContinue.Enabled = false; btn_OrientSaveContinue.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnSaveButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Reset Position";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_ResetPosition.Enabled = false; btn_ResetPosition.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Gauge Setting";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_PackageGaugeSetting.Enabled = false; btn_PackageGaugeSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_UseGauge.Enabled = false; chk_UseGauge.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_ShowDraggingBox.Enabled = false; chk_ShowDraggingBox.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_ShowSamplePoints.Enabled = false; chk_ShowSamplePoints.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnUserRightEnableGaugeSetting = false;
                        m_blnGaugeButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Rotate Button";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        gBox_OrientAngle.Enabled = false; gBox_OrientAngle.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        gBox_PreciseAngle.Enabled = false; gBox_PreciseAngle.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnRotateButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Pad Threshold";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        gb_Threshold.Enabled = false; gb_Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Object Selection";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        gb_ObjectSelection.Enabled = false; gb_ObjectSelection.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Pad ROI Tolerance Setting";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_PadROIToleranceSetting.Enabled = false; btn_PadROIToleranceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        pnl_Top.Enabled = false; pnl_Top.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        pnl_Right.Enabled = false; pnl_Right.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        pnl_Bottom.Enabled = false; pnl_Bottom.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        pnl_Left.Enabled = false; pnl_Left.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_SetToAllEdge.Enabled = false; chk_SetToAllEdge.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_SetToAllSideROI.Enabled = false; chk_SetToAllSideROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnPadROIButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Pad Labeling";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        gb_PadLabeling.Enabled = false; gb_PadLabeling.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Customize Pad";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        chk_ResetPadCustomize.Enabled = false; chk_ResetPadCustomize.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_smVisionInfo.g_blnPadAllowCustomize = false;
                    }

                    strChild3 = "Pitch Gap Setting";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        chk_ResetPitchGap.Enabled = false; chk_ResetPitchGap.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_PitchGapSetting.Enabled = false; dgd_PitchGapSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_TopPGSetting.Enabled = false; dgd_TopPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_RightPGSetting.Enabled = false; dgd_RightPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_BottomPGSetting.Enabled = false; dgd_BottomPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_LeftPGSetting.Enabled = false; dgd_LeftPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Dont Care Area Setting";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        cbo_DontCareAreaDrawMethod.Enabled = false; cbo_DontCareAreaDrawMethod.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_Undo.Enabled = false; btn_Undo.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_AddDontCareROI.Enabled = false; btn_AddDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_DeleteDontCareROI.Enabled = false; btn_DeleteDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }
                }
                else if (m_intVisionType == 1)
                {
                    strChild2 = "Learn Pad Orient Page";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_OrientSaveClose.Enabled = false; btn_OrientSaveClose.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_OrientSaveContinue.Enabled = false; btn_OrientSaveContinue.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnSaveButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Rotate Button";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        gBox_OrientAngle.Enabled = false; gBox_OrientAngle.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        gBox_PreciseAngle.Enabled = false; gBox_PreciseAngle.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnRotateButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Reset Position";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_ResetPosition.Enabled = false; btn_ResetPosition.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }
                }
            }
            else
            {
                if (m_intVisionType == 0)
                {
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_ROISaveClose.Enabled = false; btn_ROISaveClose.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_GaugeSaveClose.Enabled = false; btn_GaugeSaveClose.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_Save.Enabled = false; btn_Save.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_SaveSubLearn.Enabled = false; btn_SaveSubLearn.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_OrientSaveContinue.Enabled = false; btn_OrientSaveContinue.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnSaveButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Reset Position";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_ResetPosition.Enabled = false; btn_ResetPosition.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Gauge Setting";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_PackageGaugeSetting.Enabled = false; btn_PackageGaugeSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_UseGauge.Enabled = false; chk_UseGauge.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_ShowDraggingBox.Enabled = false; chk_ShowDraggingBox.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_ShowSamplePoints.Enabled = false; chk_ShowSamplePoints.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnUserRightEnableGaugeSetting = false;
                        m_blnGaugeButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Rotate Button";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        gBox_OrientAngle.Enabled = false; gBox_OrientAngle.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        gBox_PreciseAngle.Enabled = false; gBox_PreciseAngle.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnRotateButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Pad Threshold";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        gb_Threshold.Enabled = false; gb_Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Object Selection";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        gb_ObjectSelection.Enabled = false; gb_ObjectSelection.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Pad ROI Tolerance Setting";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_PadROIToleranceSetting.Enabled = false; btn_PadROIToleranceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        pnl_Top.Enabled = false; pnl_Top.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        pnl_Right.Enabled = false; pnl_Right.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        pnl_Bottom.Enabled = false; pnl_Bottom.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        pnl_Left.Enabled = false; pnl_Left.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_SetToAllEdge.Enabled = false; chk_SetToAllEdge.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        chk_SetToAllSideROI.Enabled = false; chk_SetToAllSideROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_blnPadROIButtonVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Pad Labeling";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        gb_PadLabeling.Enabled = false; gb_PadLabeling.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Customize Pad";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        chk_ResetPadCustomize.Enabled = false; chk_ResetPadCustomize.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        m_smVisionInfo.g_blnPadAllowCustomize = false;
                    }

                    strChild3 = "Pitch Gap Setting";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        chk_ResetPitchGap.Enabled = false; chk_ResetPitchGap.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_PitchGapSetting.Enabled = false; dgd_PitchGapSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_TopPGSetting.Enabled = false; dgd_TopPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_RightPGSetting.Enabled = false; dgd_RightPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_BottomPGSetting.Enabled = false; dgd_BottomPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        dgd_LeftPGSetting.Enabled = false; dgd_LeftPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Dont Care Area Setting";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        cbo_DontCareAreaDrawMethod.Enabled = false; cbo_DontCareAreaDrawMethod.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_Undo.Enabled = false; btn_Undo.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_AddDontCareROI.Enabled = false; btn_AddDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                        btn_DeleteDontCareROI.Enabled = false; btn_DeleteDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                        srmLabel66.Visible = srmLabel23.Visible = srmLabel6.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }
                }
                else if (m_intVisionType == 2)
                {
                    strChild2 = "Learn Pad Color Page";
                    strChild3 = "Save Button";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_SaveColor.Enabled = false; btn_SaveColor.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }

                    strChild3 = "Color Threshold";
                    if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild1, strChild2, strChild3))
                    {
                        btn_ColorThreshold.Enabled = false; btn_ColorThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }
                }
            }
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                btn_DirectToPackageForm.Visible = false;

            if (!File.Exists(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\OriTemplate.bmp"))
                chk_UsePreTemplateImage.Enabled = false;

        }

        /// <summary>
        /// Build pad blob objects
        /// </summary>
        /// <returns>true = successfully build object, false = fail to build object</returns>
        private bool BuildObjectsAndSetToTemplateArray()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ClearTemplateBlobsFeatures();

                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                    continue;

                // Build object using package ROI   //Dun change the thresholdvalue to -4 if build object fail. User will feel weird why the value chnage to auto without their acknowledgement. 
                if (m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[i][3])) // 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package
                {
                    m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3],
                         0,// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX, // 2019-09-24 ZJYEOH: No more Offset
                         0);// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY); // 2019-09-24 ZJYEOH: No more Offset
                    m_smVisionInfo.g_blnViewObjectsBuilded = true;
                }
                else
                {
                    //2020 10 05 - CCENG: Dun change the thresholdvalue to -4 if build object fail. User will feel weird why the value chnage to auto without their acknowledgement. 
                    //m_smVisionInfo.g_arrPad[i].ref_intThresholdValue = -4;
                    //// Build object using package ROI
                    //if (m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[i][3])) // 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package
                    //{
                    //    m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3],
                    //       0,// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX, // 2019-09-24 ZJYEOH: No more Offset
                    //       0);// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY); // 2019-09-24 ZJYEOH: No more Offset
                    //    m_smVisionInfo.g_blnViewObjectsBuilded = true;
                    //}
                }
            }

            // 2020 03 25 - CCENG: make sure pad gauge point percentage is not missing after press previous button
            string strFolderPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

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
                m_smVisionInfo.g_arrPad[i].LoadPercentageTemporary(strFolderPath + "Pad\\Template\\Template.xml", strSectionName);
            }

            return true;
        }


        private bool BuildObjects_ConsiderImage2()
        {
            bool blnUseDoubleThreshold = false;

            if (blnUseDoubleThreshold)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        continue;

                    m_smVisionInfo.g_arrPad[i].ClearTemplateBlobsFeatures();

                    if (i == 0)
                    {
                        // Build object using package ROI
                        if (m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[i][3])) // 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package
                        {
                            m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3],
                                 0,// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX, // 2019-09-24 ZJYEOH: No more Offset
                            0);// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY); // 2019-09-24 ZJYEOH: No more Offset
                            m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        }
                        //else
                        //{
                        //    m_smVisionInfo.g_arrPad[i].ref_intThresholdValue = -4;
                        //    // Build object using package ROI
                        //    if (m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[i][3]))
                        //    {
                        //        m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3]);
                        //        m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        //    }
                        //}
                    }
                    else
                    {
                        // Build object using package ROI
                        if (m_smVisionInfo.g_arrPad[i].BuildDoubleThresholdPadObjects(m_smVisionInfo.g_arrPadROIs[i][3], i))
                        {
                            m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3],
                                 0,// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                            0);// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset
                            m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        }
                        //else
                        //{
                        //    m_smVisionInfo.g_arrPad[i].ref_intThresholdValue = -4;
                        //    // Build object using package ROI
                        //    if (m_smVisionInfo.g_arrPad[i].BuildDoubleThresholdPadObjects(m_smVisionInfo.g_arrPadROIs[i][3], i))
                        //    {
                        //        m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3]);
                        //        m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        //    }
                        //}
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    m_smVisionInfo.g_arrPad[i].ClearTemplateBlobsFeatures();

                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        continue;

                    if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        continue;

                    m_smVisionInfo.g_arrPad[i].ClearTemplateBlobsFeatures();

                    if (i == 0)
                    {
                        // Build object using package ROI
                        if (m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[i][3]))// 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package
                        {
                            m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3],
                                 0,// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                            0);// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset
                            m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        }
                        //else
                        //{
                        //    m_smVisionInfo.g_arrPad[i].ref_intThresholdValue = -4;
                        //    // Build object using package ROI
                        //    if (m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[i][3]))
                        //    {
                        //        m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3]);
                        //        m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        //    }
                        //}

                    }
                    else
                    {
                        // Build object using package ROI
                        if (m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects_ConsiderImage2(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrRotatedImages[1], i))// 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package
                        {
                            m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3],
                                 0,// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                            0);// m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset
                            m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        }
                        //else
                        //{
                        //    m_smVisionInfo.g_arrPad[i].ref_intThresholdValue = -4;
                        //    // Build object using package ROI
                        //    if (m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects_ConsiderImage2(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrRotatedImages[1], i))
                        //    {
                        //        m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3]);
                        //        m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        //    }
                        //}
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Add search ROI (center, top, right, bottom, left pad roi)
        /// </summary>
        /// <param name="arrROIs">ROI</param>
        private void AddSearchROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (arrROIs[i].Count == 0)
                {
                    if (i == 0)
                    {
                        objROI = new ROI("Center Pad ROI", 1);
                        if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                        {
                            objROI.LoadROISetting(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight);
                        }
                        else
                        {
                            int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 80;
                            int intPositionY = (480 / 2) - 80;
                            objROI.LoadROISetting(intPositionX, intPositionY, 170, 170);
                        }
                    }
                    else if (i == 1)
                    {
                        objROI = new ROI("T", 1);
                        int intPostX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 65;
                        int intPostY = (480 / 2) - 180;
                        objROI.LoadROISetting(intPostX, intPostY, 150, 50);
                    }
                    else if (i == 2)
                    {
                        objROI = new ROI("R", 1);
                        int intPostXR = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) + 120;
                        int intPostYR = (480 / 2) - 70;
                        objROI.LoadROISetting(intPostXR, intPostYR, 50, 150);
                    }
                    else if (i == 3)
                    {
                        objROI = new ROI("B", 1);
                        int intPostXB = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 65;
                        int intPostYB = (480 / 2) + 120;
                        objROI.LoadROISetting(intPostXB, intPostYB, 150, 50);
                    }
                    else
                    {
                        objROI = new ROI("L", 1);
                        int intPostXL = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 170;
                        int intPostYL = (480 / 2) - 70;
                        objROI.LoadROISetting(intPostXL, intPostYL, 50, 150);
                    }

                    //objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                    arrROIs[i].Add(objROI);
                }
                //2020-08-18 ZJYEOH : attach every time enter learn form
                arrROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }
        }
        private void AddSearchROI(List<ROI> arrROIs)
        {
            ROI objROI;
            if (arrROIs.Count == 0)
            {
                objROI = new ROI("Center Pad ROI", 1);
                if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                {
                    if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
                    }
                    else
                    {
                        int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 80;
                        int intPositionY = (480 / 2) - 80;
                        objROI.LoadROISetting(intPositionX, intPositionY, 170, 170);
                    }
                }
                else
                {
                    int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 80;
                    int intPositionY = (480 / 2) - 80;
                    objROI.LoadROISetting(intPositionX, intPositionY, 170, 170);
                }

                objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                arrROIs.Add(objROI);
            }
        }
        private void AddOrientSearchROI(List<ROI> arrROIs)
        {
            ROI objROI;
            if (arrROIs.Count == 0)
            {
                objROI = new ROI("Center Pad ROI", 1);
                if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                {

                    objROI.LoadROISetting(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight);
                    
                }
                else
                {
                    int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 80;
                    int intPositionY = (480 / 2) - 80;
                    objROI.LoadROISetting(intPositionX, intPositionY, 170, 170);
                }
                if(m_blnOrientTestDone)
                    objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                else
                    objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                arrROIs.Add(objROI);
            }
        }
        /// <summary>
        /// Add search ROI (center, top, right, bottom, left train roi)
        /// </summary>
        /// <param name="arrROIs">ROI</param>
        private void AddTrainUnitROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (arrROIs[i].Count == 0)
                {
                    SRMMessageBox.Show("Search ROI no exist!");
                    return;
                }

                if (arrROIs[i].Count == 1)
                {
                    if (i == 0)
                        objROI = new ROI("Gauge ROI", 2);
                    else if (i == 1)
                        objROI = new ROI("T", 2);
                    else if (i == 2)
                        objROI = new ROI("R", 2);
                    else if (i == 3)
                        objROI = new ROI("B", 2);
                    else
                        objROI = new ROI("L", 2);

                    arrROIs[i].Add(objROI);
                    
                }

                arrROIs[i][1].AttachImage(arrROIs[i][0]);
            }
        }

        /// <summary>
        /// Add package roi to unit roi
        /// </summary>
        /// <param name="arrROIs"></param>
        private void AddTrainPackageROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (arrROIs[i].Count == 0)
                {
                    SRMMessageBox.Show("Search ROI no exist!");
                    return;
                }

                if (arrROIs[i].Count == 1)
                {
                    SRMMessageBox.Show("Unit ROI no exist!");
                }

                if (arrROIs[i].Count == 2)
                {
                    objROI = new ROI("Pkg ROI", 2);
                    arrROIs[i].Add(objROI);
                }

                arrROIs[i][2].AttachImage(arrROIs[i][0]);

                if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
                {
                    arrROIs[i][2].LoadROISetting(
                   (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                   (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
                    arrROIs[i][0].ref_ROITotalX, 0, MidpointRounding.AwayFromZero),
                   (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                   (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
                   arrROIs[i][0].ref_ROITotalY, 0, MidpointRounding.AwayFromZero),
                   (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0)),
                   (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0)));
                }
                else
                {
                    // since gauge is not use, so set gauge roi (arrROIs index 1)size to pkg roi.
                    arrROIs[i][2].LoadROISetting(arrROIs[i][1].ref_ROIPositionX, arrROIs[i][1].ref_ROIPositionY,
                        arrROIs[i][1].ref_ROIWidth, arrROIs[i][1].ref_ROIHeight);
                }
            }
        }
        private void AddUnitROI(List<ROI> arrROIs)
        {
            ROI objROI;

            if (arrROIs.Count == 0)
            {
                SRMMessageBox.Show("Search ROI no exist!");
                return;
            }

            if (arrROIs.Count == 1)
            {
                objROI = new ROI("Unit ROI", 2);
                arrROIs.Add(objROI);

                arrROIs[1].LoadROISetting(arrROIs[0].ref_ROIWidth / 4, arrROIs[0].ref_ROIHeight / 4, arrROIs[0].ref_ROIWidth / 2, arrROIs[0].ref_ROIHeight / 2);

            }
            
            arrROIs[1].AttachImage(arrROIs[0]);

        }
        private void AddOrientROI(List<ROI> arrROIs)
        {
            ROI objROI;

            if (arrROIs.Count == 0)
            {
                SRMMessageBox.Show("Search ROI no exist!");
                return;
            }

            if (arrROIs.Count == 1)
            {
                SRMMessageBox.Show("Unit ROI no exist!");
                return;
            }

            if (arrROIs.Count == 2)
            {
                objROI = new ROI("Orient ROI", 2);
                arrROIs.Add(objROI);

                arrROIs[2].LoadROISetting(arrROIs[1].ref_ROIWidth / 4, arrROIs[1].ref_ROIHeight / 4, arrROIs[1].ref_ROIWidth / 2, arrROIs[1].ref_ROIHeight / 2);

            }

            arrROIs[2].AttachImage(arrROIs[1]);

        }
        private void AddColorROI(List<List<CROI>> arrROIs)
        {
            CROI objROI;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i >= arrROIs.Count)
                    arrROIs.Add(new List<CROI>());

                if (arrROIs[i].Count == 0)
                {
                    objROI = new CROI("Color ROI", 1);
                    arrROIs[i].Add(objROI);

                    if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                    {
                        m_smVisionInfo.g_arrPadColorROIs[i][0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIHeight);

                    }
                    else
                    {
                        int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 80;
                        int intPositionY = (480 / 2) - 80;
                        objROI.LoadROISetting(intPositionX, intPositionY, 170, 170);
                    }
                }
                else
                {
                    if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                    {
                        m_smVisionInfo.g_arrPadColorROIs[i][0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIHeight);

                    }
                    else
                    {
                        int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 80;
                        int intPositionY = (480 / 2) - 80;
                        m_smVisionInfo.g_arrPadColorROIs[i][0].LoadROISetting(intPositionX, intPositionY, 170, 170);
                    }
                }

                arrROIs[i][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            }
        }
        private void AddTrainPadROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (arrROIs[i].Count == 0)
                {
                    SRMMessageBox.Show("Search ROI no exist!");
                    return;
                }

                if (arrROIs[i].Count == 1)
                {
                    SRMMessageBox.Show("Unit ROI no exist!");
                    return;
                }

                if (arrROIs[i].Count == 2)
                {
                    SRMMessageBox.Show("Package ROI no exist!");
                    return;
                }

                if (arrROIs[i].Count == 3)
                {
                    objROI = new ROI("Pad ROI", 1); // 2019 06 21 - CCENG: intType is 1 because Pad ROI will attach to image, not search ROI.
                    arrROIs[i].Add(objROI);
                }

                if (arrROIs[i][3].ref_intType != 1)
                {
                    arrROIs[i][3].ref_intType = 1;
                }
                // Attach Pad ROI into search ROI
                //arrROIs[i][3].AttachImage(arrROIs[i][0]);
                arrROIs[i][3].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);    //2019 05 16 - CCENG Attach Pad ROI on image instead of search ROI because Pad ROI may bigger than search ROI if user drag search ROI to close to package size.

                /*
                 * Algorithm:
                 * - Image 1 using side light, image 2 using coaxial light
                 * - Center Pad background is always dark color for image 1 and 2 except when pickup head side is bigger than unit size especially for very small package. 
                 * - Side Pad background is dark color for image 1 but light color for image 2.
                 * - When unit background is dark color, SizeTolerance of Unit ROI can set to 10.
                 * - When unit background is light color, SizeTolerance of Unit ROI must set to 0 
                 *   because if Unit ROI size bigger then physical unit size, then vision will confuse the unit backgroudn as pad objects.
                 */

                // Set Package ROI location and size base on RectGauge4L measurement
                //if ((m_smVisionInfo.g_blnCheckPackage && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)) ||
                //    (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)))
                /* 2019 04 17 - CCENG: Since package learning steps are separated from LearnPadForm, so not need to check m_smCustomizeInfo.g_intWantPackage anymore.
                 *                     User are free to choose want to use gauge size or not whether g_intWantPackage is 1 or 0
                 */
                if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
                {

                    // 2019-05-06 ZJYEOH : Compare the ROI Size Tolerance with value set in Advance Setting
                    if (m_smVisionInfo.g_arrPad[i].ref_intPadROISizeTolerance != m_smVisionInfo.g_arrPad[i].ref_intPadROISizeToleranceADV)
                        m_smVisionInfo.g_arrPad[i].ref_intPadROISizeTolerance = m_smVisionInfo.g_arrPad[i].ref_intPadROISizeToleranceADV;

                    // 2020 02 16 - CCENG: No longer need this formula since the pkg size tolerance able to set by user
                    //bool blnUnitBackgroundPickupHeadColorIsBlack = true; // Unit backgound is black when pickup size smaller than unit.
                    //int intPkgSizeTolerance;

                    //if (m_smVisionInfo.g_intSelectedImage == 0) // Image 1 where center pad and side pad background are dark 
                    //{
                    //    if (i == 0) // Center ROI
                    //    {
                    //        if (blnUnitBackgroundPickupHeadColorIsBlack)
                    //            intPkgSizeTolerance = m_smVisionInfo.g_arrPad[i].ref_intPadROISizeTolerance;    // for pickup head size smaller than unit size, Set package ROI size  pixel higher than RectGauge4L measurement
                    //        else
                    //            intPkgSizeTolerance = 0;    // for pickup head size bigger than unit size, set tolerance to 0. Since the background of unit is white color, tolerance of unit cannot add. or else it will be recognized as defect area.
                    //    }
                    //    else
                    //        intPkgSizeTolerance = m_smVisionInfo.g_arrPad[i].ref_intPadROISizeTolerance;
                    //}
                    //else // Image 2 where center pad background is dark but side pad background is light. 
                    //{
                    //    if (i == 0) // Center ROI
                    //    {
                    //        if (blnUnitBackgroundPickupHeadColorIsBlack)
                    //            intPkgSizeTolerance = m_smVisionInfo.g_arrPad[i].ref_intPadROISizeTolerance;    // for pickup head size smaller than unit size, Set package ROI size 10 pixel higher than RectGauge4L measurement
                    //        else
                    //            intPkgSizeTolerance = 0;    // for pickup head size bigger than unit size, set tolerance to 0. Since the background of unit is white color, tolerance of unit cannot add. or else it will be recognized as defect area.
                    //    }
                    //    else
                    //        intPkgSizeTolerance = 0;
                    //}

                    // arrROIs[i][3].LoadROISetting(
                    //(int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                    //(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
                    // arrROIs[i][0].ref_ROITotalX - intPkgSizeTolerance, 0, MidpointRounding.AwayFromZero),
                    //(int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                    //(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
                    //arrROIs[i][0].ref_ROITotalY - intPkgSizeTolerance, 0, MidpointRounding.AwayFromZero),
                    //(int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) + intPkgSizeTolerance * 2),
                    //(int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) + intPkgSizeTolerance * 2));
                    //arrROIs[i][3].LoadROISetting(
                    //(int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                    //(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
                    //intPkgSizeTolerance, 0, MidpointRounding.AwayFromZero),
                    //(int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                    //(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
                    //intPkgSizeTolerance, 0, MidpointRounding.AwayFromZero),
                    //(int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) + intPkgSizeTolerance * 2),
                    //(int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) + intPkgSizeTolerance * 2));

                    /*
                     * 2020 02 23 - CCENG: when is blnMeasureCenterPkgSizeUsingSidePkg, no need purposely get corner points from side pad. 
                     *                   : during gauge measurement at steps 2, the function MeasureEdge_UsingSidePkgCornerPoint have been called and the corner points from side pad have been transfer to center gauge.
                     */
                    //if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                    //{
                    //    float fLengthTop = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop;
                    //    float fLengthRight = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight;
                    //    float fLengthBottom = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom;
                    //    float fLengthLeft = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft;

                    //    PointF p1 = new PointF(m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X, m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y);
                    //    PointF p2 = new PointF(m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X, m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y);
                    //    PointF p3 = new PointF(m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X, m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y);
                    //    PointF p4 = new PointF(m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X, m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y);

                    //    float fStartX = Math.Min(p1.X, p4.X);
                    //    float fStartY = Math.Min(p1.Y, p2.Y);
                    //    float fWidth = Math.Max(p2.X, p3.X) - fStartX;
                    //    float fHeight = Math.Max(p3.Y, p4.Y) - fStartY;

                    //    arrROIs[i][3].LoadROISetting(
                    //    (int)Math.Round(fStartX - fLengthLeft, 0, MidpointRounding.AwayFromZero),
                    //    (int)Math.Round(fStartY - fLengthTop, 0, MidpointRounding.AwayFromZero),
                    //    (int)Math.Ceiling(fWidth + fLengthLeft + fLengthRight),
                    //    (int)Math.Ceiling(fHeight + fLengthTop + fLengthBottom));
                    //}
                    //else
                    {
                        // 2019-11-01 ZJYEOH : Pad ROI Tolerance now separate into 4 sides
                        arrROIs[i][3].LoadROISetting(
                                (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft, 0, MidpointRounding.AwayFromZero),
                                (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop, 0, MidpointRounding.AwayFromZero),
                                (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight),
                                (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom));
                    }
                }
                else
                {
                    // since gauge is not use, so set gauge roi (arrROIs index 1)size to pad roi.
                    arrROIs[i][3].LoadROISetting(arrROIs[i][0].ref_ROIPositionX + arrROIs[i][1].ref_ROIPositionX - m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft,
                        arrROIs[i][0].ref_ROIPositionY + arrROIs[i][1].ref_ROIPositionY - m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop,
                        arrROIs[i][1].ref_ROIWidth + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight,
                        arrROIs[i][1].ref_ROIHeight + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom);


                }

                if (i > 0)  // For side pad only
                {
                    // Logic Max image 1 and image 2 when WantConsiderPadImage2 is true
                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantConsiderPadImage2)
                    {
                        int intInternalSizeTolerance = 5;
                        ROI objImage2InternalROI = new ROI();
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                            objImage2InternalROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[1]);
                        else
                            objImage2InternalROI.AttachImage(m_smVisionInfo.g_arrImages[1]);

                        objImage2InternalROI.LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                           (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) +
                           intInternalSizeTolerance, 0, MidpointRounding.AwayFromZero),
                           (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                           (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                           intInternalSizeTolerance, 0, MidpointRounding.AwayFromZero),
                           (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) - intInternalSizeTolerance * 2),
                           (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) - intInternalSizeTolerance * 2));

                        ROI objImage1InternalROI = new ROI();
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                            objImage1InternalROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                        else
                            objImage1InternalROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                        objImage1InternalROI.LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                           (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) +
                           intInternalSizeTolerance, 0, MidpointRounding.AwayFromZero),
                           (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                           (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) +
                           intInternalSizeTolerance, 0, MidpointRounding.AwayFromZero),
                           (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) - intInternalSizeTolerance * 2),
                           (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) - intInternalSizeTolerance * 2));

                        ROI.MaxROI(objImage1InternalROI, objImage2InternalROI, ref objImage1InternalROI);
                    }
                }
            }
        }


        private void AddPadROIToPin1SearchROI(List<List<ROI>> arrROIs)
        {
            if (m_smVisionInfo.g_arrPin1.Count == 0)
                m_smVisionInfo.g_arrPin1.Add(new Pin1());

            if (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI == null)
                m_smVisionInfo.g_arrPin1[0].ref_objSearchROI = new ROI();

            ROI objSearchROI = m_smVisionInfo.g_arrPin1[0].ref_objSearchROI;
            arrROIs[0][0].CopyTo(ref objSearchROI);

            m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
        }

        /// <summary>
        /// Put line gauge of 4 borders
        /// </summary>
        private void AddPositioningGauge()
        {
            if (m_smVisionInfo.g_arrPositioningGauges.Count > 12)
                m_smVisionInfo.g_arrPositioningGauges.Clear();

            for (int i = 0; i < 12; i++)
            {
                if (m_smVisionInfo.g_arrPositioningGauges.Count <= i)
                    m_smVisionInfo.g_arrPositioningGauges.Add(new LGauge(m_smVisionInfo.g_WorldShape));

                switch (i)
                {
                    case 0:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 0;
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[0]);
                        break;
                    case 1:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 90;
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[1]);
                        break;
                    case 2:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 180;
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[2]);
                        break;
                    case 3:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 270;
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[3]);
                        break;
                    case 4:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 0;     // For TL ROI Top measurement
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[4]);
                        break;
                    case 5:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 270;   // For TL ROI Left measurement
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[4]);
                        break;
                    case 6:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 0;     // For TR ROI Top measurement
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[5]);
                        break;
                    case 7:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 90;    // For TR ROI Right measurement
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[5]);
                        break;
                    case 8:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 180;   // For BL ROI Bottom measurement
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[6]);
                        break;
                    case 9:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 270;   // For BL ROI Left measurement
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[6]);
                        break;
                    case 10:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 180;  // For BR ROI Bottom measurement
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[7]);
                        break;
                    case 11:
                        m_smVisionInfo.g_arrPositioningGauges[i].ref_GaugeAngle = 90;   // For BR ROI Right measurement
                        m_smVisionInfo.g_arrPositioningGauges[i].SetGaugePlacement(m_smVisionInfo.g_arrPositioningROIs[7]);
                        break;
                }


            }
        }
        /// <summary>
        /// Add 4 search roi for positioning used
        /// </summary>
        private void AddPositioningSearchROI()
        {
            for (int i = 0; i < 8; i++)
            {
                if ((m_smVisionInfo.g_arrPositioningROIs.Count - 1) < i)
                {
                    m_smVisionInfo.g_arrPositioningROIs.Add(new ROI());

                    m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIPositionX = 100;
                    m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIPositionY = 100;
                    m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIWidth = 100;
                    m_smVisionInfo.g_arrPositioningROIs[i].ref_ROIHeight = 100;
                }

                switch (i)
                {
                    case 0:
                        m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "Top";
                        break;
                    case 1:
                        m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "Right";
                        break;
                    case 2:
                        m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "Bottom";
                        break;
                    case 3:
                        m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "Left";
                        break;

                    case 4:
                        m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "TL";
                        break;
                    case 5:
                        m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "TR";
                        break;
                    case 6:
                        m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "BL";
                        break;
                    case 7:
                        m_smVisionInfo.g_arrPositioningROIs[i].ref_strROIName = "BR";
                        break;
                }

                m_smVisionInfo.g_arrPositioningROIs[i].ref_intType = 1;
                m_smVisionInfo.g_arrPositioningROIs[i].AttachImage(m_smVisionInfo.g_objPackageImage);
            }

            AddPositioningGauge();
        }

        private void AutoDefinePitchGap(int intPadPosition)
        {
            m_smVisionInfo.g_arrPad[intPadPosition].ClearTemplatePitchGap();
            int intTotalPadNo = m_smVisionInfo.g_arrPad[intPadPosition].GetBlobsFeaturesNumber() - 1;

            int intFormPadNo = 0;
            int intToPadNo = 1;

            int intPitchGapIndex = 0;
            // bool blnBackward = false;
            while (true)
            {
                //if (blnBackward && intFormPadNo == 0)
                //    break;

                //if (intFormPadNo == intTotalPadNo)
                //{
                //    blnBackward = true;
                //    intToPadNo = 0;
                //}

                if (intFormPadNo >= intTotalPadNo)
                    break;

                if (intFormPadNo == intToPadNo)
                {
                    intFormPadNo++;
                    intToPadNo++;
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].IsPadEnable(intFormPadNo))
                {
                    //if (!blnBackward)
                    //{
                    intFormPadNo++;
                    intToPadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].IsPadEnable(intToPadNo))
                {
                    //if (!blnBackward)
                    //{
                    intFormPadNo++;
                    intToPadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].IsPadAtPackageEdge(intFormPadNo, intToPadNo)) // By default, not auto link pad at center (heatsink) to other pad.
                {
                    //if (!blnBackward)
                    //{
                    intFormPadNo++;
                    intToPadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].IsPadAtPackageEdge(intToPadNo, intFormPadNo))   // By default, not auto link pad at center (heatsink) to other pad.
                {
                    //if (!blnBackward)
                    //{
                    intFormPadNo++;
                    intToPadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else if (m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkExist(intFormPadNo, intToPadNo))
                {
                    //if (!blnBackward)
                    //{
                    intFormPadNo++;
                    intToPadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else if (m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkInPadAlready(intFormPadNo))
                {
                    //if (!blnBackward)
                    //{
                    intFormPadNo++;
                    intToPadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkAvailable(intFormPadNo, intToPadNo))
                {
                    //if (!blnBackward)
                    //{
                    intFormPadNo++;
                    intToPadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intPadPosition].SetPitchGap(intPitchGapIndex, intFormPadNo, intToPadNo);
                    intPitchGapIndex++;
                    //if (!blnBackward)
                    //{
                    intFormPadNo++;
                    intToPadNo++;
                    //}
                    //else
                    //{
                    //    break;
                    //}
                }

                //ReadPadPitchGapToGrid(tabCtrl_PadPG.SelectedIndex, m_dgdViewPG);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        // 2019 05 07 - This backward feature have bug where it will auto pitch between first pad and last pad for the same direction.
        private void AutoDefinePitchGap_Old(int intPadPosition)
        {
            m_smVisionInfo.g_arrPad[intPadPosition].ClearTemplatePitchGap();
            int intTotalPadNo = m_smVisionInfo.g_arrPad[intPadPosition].GetBlobsFeaturesNumber() - 1;

            int intFormPadNo = 0;
            int intToPadNo = 1;

            int intPitchGapIndex = 0;
            bool blnBackward = false;
            while (true)
            {
                if (blnBackward && intFormPadNo == 0)
                    break;

                if (intFormPadNo == intTotalPadNo)
                {
                    blnBackward = true;
                    intToPadNo = 0;
                }

                //if (intFormPadNo >= intTotalPadNo)
                //    break;

                if (intFormPadNo == intToPadNo)
                {
                    intFormPadNo++;
                    intToPadNo++;
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].IsPadEnable(intFormPadNo))
                {
                    if (!blnBackward)
                    {
                        intFormPadNo++;
                        intToPadNo++;
                    }
                    else
                    {
                        intFormPadNo--;
                    }
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].IsPadEnable(intToPadNo))
                {
                    if (!blnBackward)
                    {
                        intFormPadNo++;
                        intToPadNo++;
                    }
                    else
                    {
                        intFormPadNo--;
                    }
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].IsPadAtPackageEdge(intFormPadNo, intToPadNo)) // By default, not auto link pad at center (heatsink) to other pad.
                {
                    if (!blnBackward)
                    {
                        intFormPadNo++;
                        intToPadNo++;
                    }
                    else
                    {
                        intFormPadNo--;
                    }
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].IsPadAtPackageEdge(intToPadNo, intFormPadNo))   // By default, not auto link pad at center (heatsink) to other pad.
                {
                    if (!blnBackward)
                    {
                        intFormPadNo++;
                        intToPadNo++;
                    }
                    else
                    {
                        intFormPadNo--;
                    }
                    continue;
                }
                else if (m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkExist(intFormPadNo, intToPadNo))
                {
                    if (!blnBackward)
                    {
                        intFormPadNo++;
                        intToPadNo++;
                    }
                    else
                    {
                        intFormPadNo--;
                    }
                    continue;
                }
                else if (m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkInPadAlready(intFormPadNo))
                {
                    if (!blnBackward)
                    {
                        intFormPadNo++;
                        intToPadNo++;
                    }
                    else
                    {
                        intFormPadNo--;
                    }
                    continue;
                }
                else if (!m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkAvailable(intFormPadNo, intToPadNo))
                {
                    if (!blnBackward)
                    {
                        intFormPadNo++;
                        intToPadNo++;
                    }
                    else
                    {
                        intFormPadNo--;
                    }
                    continue;
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intPadPosition].SetPitchGap(intPitchGapIndex, intFormPadNo, intToPadNo);
                    intPitchGapIndex++;
                    if (!blnBackward)
                    {
                        intFormPadNo++;
                        intToPadNo++;
                    }
                    else
                    {
                        break;
                    }
                }

                //ReadPadPitchGapToGrid(tabCtrl_PadPG.SelectedIndex, m_dgdViewPG);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        /// <summary>
        /// Customize GUI either pad or Pad5s
        /// </summary>
        private void CustomizeGUI()
        {
            txt_ImageGain.Value = Convert.ToDecimal(m_smVisionInfo.g_arrPad[0].ref_fImageGain);
            if (m_smVisionInfo.g_strVisionName.Contains("Pad5S"))
            {
                //picPadROI.Image = ils_ImageListTree.Images[5];
                //picUnitROI.Image = ils_ImageListTree.Images[7];
                picPadROI.BringToFront();
                picUnitROI.BringToFront();
            }
            else
            {
                //picPadROI.Image = ils_ImageListTree.Images[2];
                //picUnitROI.Image = ils_ImageListTree.Images[4];
                picPadROI1.BringToFront();
                picUnitROI1.BringToFront();
            }

            m_smVisionInfo.g_blnCutMode = radioBtn_CutObj.Checked;
            txt_MinArea.Text = m_smVisionInfo.g_arrPad[0].ref_fFilterMinArea.ToString();
            chk_UsePreviousTolerance.Checked = true;
            tabCtrl_PadSide.SelectedTab = tp_Middle;
            tabCtrl_PadPG.SelectedTab = tp_MiddleROI;

            txt_LineAngle.Text = m_smVisionInfo.g_objPositioning.ref_intDieAngleLimit.ToString();
            txt_LineGaugeMinScore.Text = m_smVisionInfo.g_objPositioning.ref_intMinBorderScore.ToString();

            txt_UnitSizeWidth.Text = m_smVisionInfo.g_objPositioning.ref_fSampleDieWidth.ToString();
            txt_UnitSizeHeight.Text = m_smVisionInfo.g_objPositioning.ref_fSampleDieHeight.ToString();

            txt_StartPixelFromEdge.Text = (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge / m_smVisionInfo.g_fCalibPixelY).ToString();
            txt_StartPixelFromRight.Text = (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight / m_smVisionInfo.g_fCalibPixelX).ToString();
            txt_StartPixelFromBottom.Text = (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom / m_smVisionInfo.g_fCalibPixelY).ToString();
            txt_StartPixelFromLeft.Text = (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft / m_smVisionInfo.g_fCalibPixelX).ToString();

            cbo_ImagesList.Items.Clear();
            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_ImagesList.Items.Add("Grab Image " + (i + 1));
            }
            if (m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex < cbo_ImagesList.Items.Count)
                cbo_ImagesList.SelectedIndex = m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex;
            else
                cbo_ImagesList.SelectedIndex = m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex = 0;

            if (cbo_ImagesList.SelectedIndex == 0)
                trackBar_Gain.Value = (int)m_smVisionInfo.g_objPositioning.ref_fGainValue;
            else
                trackBar_Gain.Value = (int)m_smVisionInfo.g_objPositioning.ref_fGainValue2
                    ;
            lbl_GainValue.Text = (Convert.ToSingle(trackBar_Gain.Value) / 1000).ToString();

            radioBtn_Middle.Checked = true;

            chk_ShowDraggingBox.Checked = true;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }

            radioBtn_Middle.Checked = true;
            m_smVisionInfo.g_intSelectedROI = 0;


            m_blnWantGaugeMeasurePkgSizePrev = chk_UseGauge.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize;
            SetUseGaugeGUI();

            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                // Display "Selected all Pads ROI" if center pad and side pad image index are same
                if (m_smVisionInfo.g_arrPad[0].ref_intCheckPadDimensionImageIndex == m_smVisionInfo.g_arrPad[1].ref_intCheckPadDimensionImageIndex)
                    cbo_SelectPadROIs.SelectedIndex = 0;    // Select all pads
                else
                {
                    cbo_SelectPadROIs.SelectedIndex = 1;    // Display Center ROIs
                }
            }
            else
            {
                btn_ResetPosition.Visible = false;
                // Hide combo box since got 1 pad only.
                srmLabel27.Visible = false;
                cbo_SelectPadROIs.Visible = false;
                cbo_SelectPadROIs.SelectedIndex = 0;    // Select all pads
            }

            cbo_SelectImage.SelectedIndex = 0; // m_smVisionInfo.g_arrPad[0].ref_intCheckPadDimensionImageIndex;      // Select images 1

            if (cbo_SelectPadROIs.SelectedIndex == 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    m_smVisionInfo.g_arrPad[i].ref_blnSelected = true;
                }

            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (i == 0)
                        m_smVisionInfo.g_arrPad[i].ref_blnSelected = true;
                    else
                        m_smVisionInfo.g_arrPad[i].ref_blnSelected = false;
                }
            }
            cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

            if (m_smVisionInfo.g_arrPad.Length > 1)
                cbo_SidePadWidthLengthMode.SelectedIndex = m_smVisionInfo.g_arrPad[1].ref_intPadWidthLengthMode;
            else
            {
                srmLabel54.Visible = false;
                cbo_SidePadWidthLengthMode.Visible = false;
                cbo_SidePadWidthLengthMode.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Define pad contour
        /// </summary>
        private void DefinePadContour()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                {
                    m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrPad[i].ref_intCheckPadDimensionImageIndex]);
                    m_smVisionInfo.g_arrPadROIs[i][3].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrPad[i].ref_intCheckPadDimensionImageIndex]);
                    m_smVisionInfo.g_arrPad[i].DefineTemplatePadContour(m_smVisionInfo.g_arrPadROIs[i][3]);
                }
            }
        }

        /// <summary>
        /// Customize GUI basrd user access level
        /// </summary>
        private void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "";

            //strChild2 = "Learn Template";         // 2018 10 22 - CCENG: Hide the Learn button instead of disable the next button.
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    btn_Previous.Enabled = false;
            //    btn_Next.Enabled = false;
            //}
            strChild1 = "Learn Page";
            strChild2 = "Save Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
                btn_ROISaveClose.Enabled = false;
                btn_SaveOnlyPosition.Enabled = false;
                btn_GaugeSaveClose.Enabled = false;
            }

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                btn_DirectToPackageForm.Visible = false;

            if (!File.Exists(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\OriTemplate.bmp"))
                chk_UsePreTemplateImage.Enabled = false;
        }

        public bool IsSettingError()
        {
            int[] arrColumnIndex = { 2, 4, 6 };

            for (int intPadIndex = 0; intPadIndex < 5; intPadIndex++)
            {
                DataGridView objDataGrid = new DataGridView();
                string strPadName = "";
                switch (intPadIndex)
                {
                    case 0:
                        strPadName = "Center Pad";
                        objDataGrid = dgd_Setting;
                        break;
                    case 1:
                        strPadName = "Top Pad";
                        objDataGrid = dgd_TopSetting;
                        break;
                    case 2:
                        strPadName = "Right Pad";
                        objDataGrid = dgd_RightSetting;
                        break;
                    case 3:
                        strPadName = "Bottom Pad";
                        objDataGrid = dgd_BottomSetting;
                        break;
                    case 4:
                        strPadName = "Left Pad";
                        objDataGrid = dgd_LeftSetting;
                        break;
                }

                for (int c = 0; c < arrColumnIndex.Length; c++)
                {
                    for (int i = 0; i < objDataGrid.Rows.Count; i++)
                    {
                        int intRow = objDataGrid.Rows.Count;
                        if (i < intRow)
                        {
                            int intColumn = objDataGrid.Rows[i].Cells.Count;
                            if (arrColumnIndex[c] >= intColumn)
                                continue;
                        }

                        if (objDataGrid.Rows[i].Cells[arrColumnIndex[c]].Value.ToString() != "---")
                        {
                            // Check insert data valid or not
                            float fMin = float.Parse(objDataGrid.Rows[i].Cells[arrColumnIndex[c]].Value.ToString());
                            float fMax = float.Parse(objDataGrid.Rows[i].Cells[arrColumnIndex[c] + 1].Value.ToString());

                            if (fMin > fMax)
                            {
                                tabCtrl_PadSide.SelectedIndex = intPadIndex;
                                SRMMessageBox.Show("Set minimum value or maximum value is not corrects in " + strPadName + ". Please check the red highlight value is correct or not.");
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool IsPitchGapSettingError()
        {
            int[] arrColumnIndex = { 2, 4 };

            for (int intPadIndex = 0; intPadIndex < 5; intPadIndex++)
            {
                DataGridView objDataGrid = new DataGridView();
                string strPadName = "";
                switch (intPadIndex)
                {
                    case 0:
                        strPadName = "Center Pad";
                        objDataGrid = dgd_PitchGapSetting;
                        break;
                    case 1:
                        strPadName = "Top Pad";
                        objDataGrid = dgd_TopPGSetting;
                        break;
                    case 2:
                        strPadName = "Right Pad";
                        objDataGrid = dgd_RightPGSetting;
                        break;
                    case 3:
                        strPadName = "Bottom Pad";
                        objDataGrid = dgd_BottomPGSetting;
                        break;
                    case 4:
                        strPadName = "Left Pad";
                        objDataGrid = dgd_LeftPGSetting;
                        break;
                }

                for (int c = 0; c < arrColumnIndex.Length; c++)
                {
                    for (int i = 0; i < objDataGrid.Rows.Count; i++)
                    {
                        int intRow = objDataGrid.Rows.Count;
                        if (i < intRow)
                        {
                            int intColumn = objDataGrid.Rows[i].Cells.Count;
                            if (arrColumnIndex[c] >= intColumn)
                                continue;
                        }

                        if (objDataGrid.Rows[i].Cells[arrColumnIndex[c]].Value != null && objDataGrid.Rows[i].Cells[arrColumnIndex[c]].Value.ToString() != "---")
                        {
                            // Check insert data valid or not
                            float fMin = float.Parse(objDataGrid.Rows[i].Cells[arrColumnIndex[c]].Value.ToString());
                            float fMax = float.Parse(objDataGrid.Rows[i].Cells[arrColumnIndex[c] + 1].Value.ToString());

                            if (fMin > fMax)
                            {
                                tabCtrl_PadPG.SelectedIndex = intPadIndex;
                                SRMMessageBox.Show("Set minimum value or maximum value is not corrects in " + strPadName + ". Please check the red highlight value is correct or not.");
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Load matcher file from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        private void LoadMatcherFile(string strPath)
        {
            //for (int i = 0; i < m_smVisionInfo.g_arrPadOrient.Length; i++)
            //{
            //    m_smVisionInfo.g_arrPadOrient[i].LoadPattern(strPath + "Pad\\Template\\Template" + i + ".mch");
            //}
            m_smVisionInfo.g_objPadOrient.LoadPattern4Direction(strPath + "Orient\\Template\\", "PadUnitTemplate0");
            m_smVisionInfo.g_objPadOrient.LoadSubPattern(strPath + "Orient\\Template\\PadOrientTemplate0.mch");

        }

        /// <summary>
        /// Load pad offset settings from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        private void LoadPadOffSetSetting(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("OffSet");

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_arrPad[i].SetWidthOffSet(objFile.GetValueAsFloat("WidthOffSet", 0));
                m_smVisionInfo.g_arrPad[i].SetHeightOffSet(objFile.GetValueAsFloat("HeightOffSet", 0));
                m_smVisionInfo.g_arrPad[i].SetPitchOffSet(objFile.GetValueAsFloat("PitchOffSet", 0));
                m_smVisionInfo.g_arrPad[i].SetGapOffSet(objFile.GetValueAsFloat("GapOffSet", 0));
            }
        }

        /// <summary>
        /// Load pad settings from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        private void LoadPadSetting(string strPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
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

        private void LoadPositioningSetting(string strFolderPath)
        {
            // Load Positioning Setting
            m_smVisionInfo.g_objPositioning.LoadPosition(strFolderPath + "Settings.xml", "General");

            // Load Pattern
            m_smVisionInfo.g_objPositioning.LoadPattern(strFolderPath + "Template\\PRS.mch");

            // Load Positioning ROI Setting
            ROI.LoadFile(strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPositioningROIs);

            // Load Positioning Line Gauge Setting
            LGauge.LoadFile(strFolderPath + "Gauge.xml", m_smVisionInfo.g_arrPositioningGauges, m_smVisionInfo.g_WorldShape);

        }

        /// <summary>
        /// Load ROI settings from selected path into ROI
        /// </summary>
        /// <param name="strPath">selected path</param>
        /// <param name="arrROIs">ROI</param>
        private void LoadROISetting(string strPath, List<List<ROI>> arrROIs)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount, intParentCount = 0;
            ROI objROI;

            intParentCount = objFile.GetFirstSectionCount();
            for (int t = 0; t < intParentCount; t++)
            {
                objFile.GetFirstSection("Unit" + t);

                arrROIs.Add(new List<ROI>());
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

                    arrROIs[t].Add(objROI);
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
        private void LoadROISetting(string strPath, List<List<ROI>> arrROIs, int intROICount)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            ROI objROI;

            for (int i = 0; i < intROICount; i++)
            {
                arrROIs.Add(new List<ROI>());
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

                    arrROIs[i].Add(objROI);
                }
            }
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
        private void LoadROISetting(string strPath, List<ROI> arrROIs)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            ROI objROI;

            objFile.GetFirstSection("Unit" + 0);

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

                arrROIs.Add(objROI);

                if (j == 1)
                    arrROIs[1].AttachImage(arrROIs[0]);
            }

        }
        /// <summary>
        /// Read pad pitch gap data into datagridview
        /// </summary>
        /// <param name="intPadPosition">pad position</param>
        /// <param name="dgd_PadPitchGap">datagridview</param>
        private void ReadPadPitchGapToGrid(int intPadPosition, DataGridView dgd_PadPitchGap)
        {
            int k = 0;
            try
            {
                if (m_smVisionInfo.g_arrPad.Length <= intPadPosition)
                    return;

                dgd_PadPitchGap.Rows.Clear();
                int intTotalPads = m_smVisionInfo.g_arrPad[intPadPosition].GetBlobsFeaturesNumber();

                for (int t = 0; t < intTotalPads; t++)
                {
                    int intToPadNo = m_smVisionInfo.g_arrPad[intPadPosition].GetPitchGapToPadNo(t);
                    int intGroupNo = m_smVisionInfo.g_arrPad[intPadPosition].GetGroupNo(t);

                    dgd_PadPitchGap.Rows.Add();
                    dgd_PadPitchGap.Rows[t].HeaderCell.Value = "Pad " + (t + 1);

                    if (intToPadNo >= 0)
                    {
                        dgd_PadPitchGap.Rows[t].Cells[0].Value = t + 1;
                        dgd_PadPitchGap.Rows[t].Cells[1].Value = intToPadNo + 1;
                        dgd_PadPitchGap.Rows[t].Cells[6].Value = intGroupNo + 1;
                    }
                    else
                    {
                        dgd_PadPitchGap.Rows[t].Cells[0].Value = t + 1;
                        dgd_PadPitchGap.Rows[t].Cells[1].Value = "NA";
                        dgd_PadPitchGap.Rows[t].Cells[6].Value = intGroupNo + 1;
                    }
                }
                m_intPitchGapSelectedRowIndex = 0;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("LearnPadForm.cs->ReadPadPitchGapToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("LearnPadForm.cs->ReadPadPitchGapToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
            }

        }

        private void ReadPadPitchGapToGrid_DisplayAvailablePitchGapRowsOnly(int intPadPosition, DataGridView dgd_PadPitchGap)
        {
            int k = 0;
            try
            {

                if (m_smVisionInfo.g_arrPad.Length <= intPadPosition)
                    return;

                dgd_PadPitchGap.Rows.Clear();
                int intIndex = 0;
                string strPitchGapData;
                string[] strPitchGapDataRow;
                int intTotalPitchGap = m_smVisionInfo.g_arrPad[intPadPosition].GetTotalPitchGap();

                //Set pitch gap col name
                for (int j = 0; j < m_strPGColName.Length; j++)
                {
                    if (intPadPosition == 0)
                        m_strPGRealColName[j] = m_strPGColName[j];
                    else
                        m_strPGRealColName[j] = m_strPGColName[j] + m_strPosition;
                }

                for (int i = 0; i < intTotalPitchGap; i++)
                {
                    intIndex = 0;
                    strPitchGapData = m_smVisionInfo.g_arrPad[intPadPosition].GetPitchGapData(i);
                    strPitchGapDataRow = strPitchGapData.Split('#');

                    dgd_PadPitchGap.Rows.Add();
                    dgd_PadPitchGap.Rows[i].HeaderCell.Value = "PitchGap " + (i + 1);
                    for (k = 0; k < m_strPGRealColName.Length; k++)
                    {
                        if (k == 0 || k == 1)
                            dgd_PadPitchGap.Rows[i].Cells[m_strPGRealColName[k]].Value = Convert.ToInt32(strPitchGapDataRow[intIndex++]) + 1;
                        else
                            dgd_PadPitchGap.Rows[i].Cells[m_strPGRealColName[k]].Value = strPitchGapDataRow[intIndex++];
                    }
                }
                m_intPitchGapSelectedRowIndex = 0;

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("LearnPadForm.cs->ReadPadPitchGapToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("LearnPadForm.cs->ReadPadPitchGapToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
            }

        }

        private void ReadAllPadTemplateDataToGrid_ClosestSizeMethod()
        {
            if (m_smVisionInfo.g_arrPad[0].ref_blnSelected)
            {
                //Load template data
                m_strPosition = "";
                //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                {
                    m_smVisionInfo.g_arrPad[0].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelX,
                                                  m_smVisionInfo.g_fCalibPixelY,
                                                  m_smVisionInfo.g_fCalibOffSetX,
                                                  m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                    m_smVisionInfo.g_arrPad[0].AnalyingPad_ClosestSizeMethod(0);
                    m_smVisionInfo.g_arrPad[0].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[0][3], m_smVisionInfo.g_arrPadROIs[0][0]);

                    m_smVisionInfo.g_arrPad[0].DefineTolerance2();  // ENG
                }

                ReadPadTemplateDataToGrid(0, dgd_Setting);
            }

            if (m_smVisionInfo.g_blnCheck4Sides)
            {
                if (m_smVisionInfo.g_arrPad.Length > 1)
                {
                    if (m_smVisionInfo.g_arrPad[1].ref_blnSelected)
                    {
                        m_dgdView = dgd_TopSetting;
                        m_strPosition = "Top";
                        //Not use prev setting but use default setting
                        //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                        {
                            m_smVisionInfo.g_arrPad[1].SetCalibrationData(
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);

                            m_smVisionInfo.g_arrPad[1].AnalyingPad_ClosestSizeMethod(1);
                            m_smVisionInfo.g_arrPad[1].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[1][3], m_smVisionInfo.g_arrPadROIs[1][0]);

                            m_smVisionInfo.g_arrPad[1].DefineTolerance(true);
                        }

                        ReadPadTemplateDataToGrid(1, dgd_TopSetting);
                    }

                    if (m_smVisionInfo.g_arrPad[2].ref_blnSelected)
                    {
                        m_strPosition = "Right";
                        //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                        {
                            m_smVisionInfo.g_arrPad[2].SetCalibrationData(
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                            m_smVisionInfo.g_arrPad[2].AnalyingPad_ClosestSizeMethod(2);
                            m_smVisionInfo.g_arrPad[2].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[2][3], m_smVisionInfo.g_arrPadROIs[2][0]);

                            m_smVisionInfo.g_arrPad[2].DefineTolerance(true);
                        }
                        ReadPadTemplateDataToGrid(2, dgd_RightSetting);
                    }

                    if (m_smVisionInfo.g_arrPad[3].ref_blnSelected)
                    {
                        m_strPosition = "Bottom";
                        //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                        {
                            m_smVisionInfo.g_arrPad[3].SetCalibrationData(
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                            m_smVisionInfo.g_arrPad[3].AnalyingPad_ClosestSizeMethod(3);
                            m_smVisionInfo.g_arrPad[3].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[3][3], m_smVisionInfo.g_arrPadROIs[3][0]);

                            m_smVisionInfo.g_arrPad[3].DefineTolerance(true);
                        }
                        ReadPadTemplateDataToGrid(3, dgd_BottomSetting);
                    }

                    if (m_smVisionInfo.g_arrPad[4].ref_blnSelected)
                    {
                        m_strPosition = "Left";
                        //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                        {
                            m_smVisionInfo.g_arrPad[4].SetCalibrationData(
                                                   m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                            m_smVisionInfo.g_arrPad[4].AnalyingPad_ClosestSizeMethod(4);
                            m_smVisionInfo.g_arrPad[4].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[4][3], m_smVisionInfo.g_arrPadROIs[4][0]);

                            m_smVisionInfo.g_arrPad[4].DefineTolerance(true);
                        }
                        ReadPadTemplateDataToGrid(4, dgd_LeftSetting);
                    }
                }
            }
            m_strPosition = "";
        }

        private void ReadAllPadTemplateDataToGrid_DefaultMethod()
        {
            if (m_smVisionInfo.g_arrPad[0].ref_blnSelected)
            {
                //Load template data
                m_strPosition = "";
                //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                {
                    m_smVisionInfo.g_arrPad[0].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelX,
                                                  m_smVisionInfo.g_fCalibPixelY,
                                                  m_smVisionInfo.g_fCalibOffSetX,
                                                  m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                    m_smVisionInfo.g_arrPad[0].AnalyingPad_DefaultToleranceMethod(0);
                    m_smVisionInfo.g_arrPad[0].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[0][3], m_smVisionInfo.g_arrPadROIs[0][0]);

                    m_smVisionInfo.g_arrPad[0].DefineTolerance2();  // ENG
                }

                ReadPadTemplateDataToGrid(0, dgd_Setting);
            }

            if (m_smVisionInfo.g_blnCheck4Sides)
            {
                if (m_smVisionInfo.g_arrPad.Length > 1)
                {
                    if (m_smVisionInfo.g_arrPad[1].ref_blnSelected)
                    {
                        m_dgdView = dgd_TopSetting;
                        m_strPosition = "Top";
                        //Not use prev setting but use default setting
                        //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                        {
                            m_smVisionInfo.g_arrPad[1].SetCalibrationData(
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);

                            m_smVisionInfo.g_arrPad[1].AnalyingPad_DefaultToleranceMethod(1);
                            m_smVisionInfo.g_arrPad[1].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[1][3], m_smVisionInfo.g_arrPadROIs[1][0]);

                            m_smVisionInfo.g_arrPad[1].DefineTolerance2();
                        }

                        ReadPadTemplateDataToGrid(1, dgd_TopSetting);
                    }

                    if (m_smVisionInfo.g_arrPad[2].ref_blnSelected)
                    {
                        m_strPosition = "Right";
                        //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                        {
                            m_smVisionInfo.g_arrPad[2].SetCalibrationData(
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                            m_smVisionInfo.g_arrPad[2].AnalyingPad_DefaultToleranceMethod(2);
                            m_smVisionInfo.g_arrPad[2].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[2][3], m_smVisionInfo.g_arrPadROIs[2][0]);

                            m_smVisionInfo.g_arrPad[2].DefineTolerance2();
                        }
                        ReadPadTemplateDataToGrid(2, dgd_RightSetting);
                    }

                    if (m_smVisionInfo.g_arrPad[3].ref_blnSelected)
                    {
                        m_strPosition = "Bottom";
                        //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                        {
                            m_smVisionInfo.g_arrPad[3].SetCalibrationData(
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                            m_smVisionInfo.g_arrPad[3].AnalyingPad_DefaultToleranceMethod(3);
                            m_smVisionInfo.g_arrPad[3].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[3][3], m_smVisionInfo.g_arrPadROIs[3][0]);

                            m_smVisionInfo.g_arrPad[3].DefineTolerance2();
                        }
                        ReadPadTemplateDataToGrid(3, dgd_BottomSetting);
                    }

                    if (m_smVisionInfo.g_arrPad[4].ref_blnSelected)
                    {
                        m_strPosition = "Left";
                        //if (!m_smVisionInfo.g_blnUsedPreTemplate)
                        {
                            m_smVisionInfo.g_arrPad[4].SetCalibrationData(
                                                   m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibPixelZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ,
                                                      m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                            m_smVisionInfo.g_arrPad[4].AnalyingPad_DefaultToleranceMethod(4);
                            m_smVisionInfo.g_arrPad[4].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[4][3], m_smVisionInfo.g_arrPadROIs[4][0]);

                            m_smVisionInfo.g_arrPad[4].DefineTolerance2();
                        }
                        ReadPadTemplateDataToGrid(4, dgd_LeftSetting);
                    }
                }
            }
            m_strPosition = "";
        }
        /// <summary>
        /// Read pad template data into datagridview
        /// </summary>
        /// <param name="intPadPosition">pad position</param>
        /// <param name="dgd_Pad">datagridview</param>
        private void ReadPadTemplateDataToGrid(int intPadPosition, DataGridView dgd_Pad)
        {
            dgd_Pad.Rows.Clear();

            string strBlobsFeatures = m_smVisionInfo.g_arrPad[intPadPosition].GetMicronBlobsFeaturesData();
            string[] strFeature = strBlobsFeatures.Split('#');
            int intBlobsCount = m_smVisionInfo.g_arrPad[intPadPosition].GetBlobsFeaturesNumber();
            int intFeatureIndex = 0;

            //Set setting col name
            for (int j = 0; j < m_strSettingColName.Length; j++)
            {
                m_strSettingRealColName[j] = "";
                if (intPadPosition == 0)
                    m_strSettingRealColName[j] = m_strSettingColName[j];
                else
                    m_strSettingRealColName[j] = m_strSettingColName[j] + m_strPosition;
            }

            string st = dgd_Pad.Name;
            for (int i = 0; i < intBlobsCount; i++)
            {
                dgd_Pad.Rows.Add();
                dgd_Pad.Rows[i].HeaderCell.Value = "Pad " + (i + 1);

                for (int k = 0; k < m_strSettingRealColName.Length; k++)
                {
                    dgd_Pad.Rows[i].Cells[m_strSettingRealColName[k]].Value = strFeature[intFeatureIndex++];
                }
            }
        }

        private void SetLastGaugeMeasureSizeAsTemplateUnitSize()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_arrPad[i].SetCurrentMeasureSizeAsUnitSize();
            }
        }

        private void RotateImage()
        {
            if (m_smVisionInfo.g_intSelectedImage == -1)
            {
                if (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                {
                    // Define rotation ROI with ROI center point same as gauge unit center point
                    float fROIWidth = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X);
                    float fROIHeight = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y);

                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));

                    // Rotate image to zero degree using RectGauge4L angle result
                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, // Middle Pad Search ROI
                                      m_smVisionInfo.g_arrPad[0].GetResultAngle_RectGauge4L(),
                    ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
                else
                {
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
                    {
                        //m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                        m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                        if (i > 0 && m_smVisionInfo.g_intSelectedImage == 1)    // For side pad and when background is white color.
                            m_smVisionInfo.g_arrPad[i].AddGrayColorOuterGaugePoint(m_smVisionInfo.g_arrPadROIs[i][0], (Pad.PadIndex)i);



                        // Define rotation ROI with ROI center point same as gauge unit center point
                        //float fROIWidth = Math.Min(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX,
                        //                    m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X);
                        //float fROIHeight = Math.Min(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY,
                        //                    m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y);

                        //float fROIWidth = m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2 + 3;
                        //float fROIHeight = m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2 + 3;


                        //objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        //objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                        //                                        (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                        //                                        (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                        //                                        (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));

                        // --------- 2018 10 19 - CCENG: Use this new formula define ROI position and size. --------------------
                        // ----------------------------: Rotate at gauge center but using search ROI size ----------------------
                        // ----------------------------: Not using gauge size to prevent rotation defect near unit edge --------

                        int intRotateCenterX = (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X, 0, MidpointRounding.AwayFromZero);
                        int intRotateCenterY = (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y, 0, MidpointRounding.AwayFromZero);

                        int intStartX = Math.Max(0, intRotateCenterX - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intEndX = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageWidth, intRotateCenterX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intHalfWidth = Math.Min(intRotateCenterX - intStartX, intEndX - intRotateCenterX);
                        intStartX = intRotateCenterX - intHalfWidth;

                        int intStartY = Math.Max(0, intRotateCenterY - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        int intEndY = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageHeight, intRotateCenterY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        int intHalfHeight = Math.Min(intRotateCenterY - intStartY, intEndY - intRotateCenterY);
                        intStartY = intRotateCenterY - intHalfHeight;

                        ROI objRotateROI = new ROI();
                        objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);    // 2018 10 18 - CCENG: New empty ROI must attach to any image. Without attach any image, the TotalOrgXY value will be 0.
                        objRotateROI.LoadROISetting(intStartX, intStartY, intHalfWidth * 2, intHalfHeight * 2);


                        // Rotate image to zero degree using RectGauge4L angle result
                        if (i == 0)
                        {
                            if (m_intVisionType == 0 && ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                            {
                                RotatePrecise();
                                ROI.Rotate0Degree(m_smVisionInfo.g_objPackageImage, objRotateROI, // Middle Pad Search ROI
                                       m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                      ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                            }
                            else
                                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, // Middle Pad Search ROI
                                         m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                        ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                        }
                        else
                        {
                            if (m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage)
                            {
                                ROI.Rotate0Degree(objRotateROI, // Middle Pad Search ROI
                                                  m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                                ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                            }
                        }

                        // if side pad + selected learn image is index 0 + WantConsiderImage2 is true, then rotate image index 1 as well before logic Add image index 1 apply to image index 0 
                        if (i > 0 && m_smVisionInfo.g_intSelectedImage == 0 && m_smVisionInfo.g_arrPad[i].ref_blnWantConsiderPadImage2)
                        {
                            if (m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage)
                            {
                                if (i == 1)
                                {
                                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[1], objRotateROI, // Middle Pad Search ROI
                                                      m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                                    ref m_smVisionInfo.g_arrRotatedImages, 1);
                                }
                                else
                                {
                                    ROI.Rotate0Degree(objRotateROI, // Middle Pad Search ROI
                                                      m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                                    ref m_smVisionInfo.g_arrRotatedImages, 1);
                                }
                            }
                        }

                        m_smVisionInfo.g_blnViewRotatedImage = true;

                        m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    }
                    //else
                    //{
                    //    //if (!m_smVisionInfo.g_blnViewRotatedImage)    // Need to re-rotate and re-copy where the ViewRotatedImage is true or not. Bcos Rotated image has been modified for dont care area 
                    //    {
                    //        float fRotatedDegree = float.Parse(txt_RotateDegree.Text) * -1f;
                    //        if (Math.Abs(fRotatedDegree) > 0)
                    //        {
                    //            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], m_smVisionInfo.g_arrPadROIs[0][0], fRotatedDegree, ref m_smVisionInfo.g_arrRotatedImages, 0);
                    //            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain);

                    //            m_smVisionInfo.g_blnViewRotatedImage = true;
                    //            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    //        }
                    //        else
                    //        {

                    //            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    //            m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    //            m_smVisionInfo.g_blnViewRotatedImage = true;
                    //            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    //        }
                    //    }
                    //}
                }
            }
            
        }
        private void RotateColorImage()
        {
            if (m_smVisionInfo.g_intSelectedImage == -1)
            {
                if (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                {
                    // Define rotation ROI with ROI center point same as gauge unit center point
                    float fROIWidth = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X);
                    float fROIHeight = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y);

                    CROI objRotateROI = new CROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));

                    // Rotate image to zero degree using RectGauge4L angle result
                    CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, // Middle Pad Search ROI
                                      m_smVisionInfo.g_arrPad[0].GetResultAngle_RectGauge4L(), 8,
                    ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadColorROIs[0][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
                else
                {
                    m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadColorROIs[0][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
                    {
                        //m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                        //if (i > 0 && m_smVisionInfo.g_intSelectedImage == 1)    // For side pad and when background is white color.
                        //    m_smVisionInfo.g_arrPad[i].AddGrayColorOuterGaugePoint(m_smVisionInfo.g_arrPadROIs[i][0], (Pad.PadIndex)i);

                        // --------- 2018 10 19 - CCENG: Use this new formula define ROI position and size. --------------------
                        // ----------------------------: Rotate at gauge center but using search ROI size ----------------------
                        // ----------------------------: Not using gauge size to prevent rotation defect near unit edge --------

                        int intRotateCenterX = (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X, 0, MidpointRounding.AwayFromZero);
                        int intRotateCenterY = (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y, 0, MidpointRounding.AwayFromZero);

                        int intStartX = Math.Max(0, intRotateCenterX - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intEndX = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageWidth, intRotateCenterX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intHalfWidth = Math.Min(intRotateCenterX - intStartX, intEndX - intRotateCenterX);
                        intStartX = intRotateCenterX - intHalfWidth;

                        int intStartY = Math.Max(0, intRotateCenterY - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        int intEndY = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageHeight, intRotateCenterY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        int intHalfHeight = Math.Min(intRotateCenterY - intStartY, intEndY - intRotateCenterY);
                        intStartY = intRotateCenterY - intHalfHeight;

                        CROI objRotateROI = new CROI();
                        objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage]);    // 2018 10 18 - CCENG: New empty ROI must attach to any image. Without attach any image, the TotalOrgXY value will be 0.
                        objRotateROI.LoadROISetting(intStartX, intStartY, intHalfWidth * 2, intHalfHeight * 2);

                        // Rotate image to zero degree using RectGauge4L angle result
                        if (i == 0)
                        {
                            CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, // Middle Pad Search ROI
                                     m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(), 8,
                    ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                        }
                        else
                        {
                            if (m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage)
                            {
                                CROI.Rotate0Degree(objRotateROI, // Middle Pad Search ROI
                                                  m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(), 8,
                                ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                            }
                        }

                        // if side pad + selected learn image is index 0 + WantConsiderImage2 is true, then rotate image index 1 as well before logic Add image index 1 apply to image index 0 
                        if (i > 0 && m_smVisionInfo.g_intSelectedImage == 0 && m_smVisionInfo.g_arrPad[i].ref_blnWantConsiderPadImage2)
                        {
                            if (m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage)
                            {
                                if (i == 1)
                                {
                                    CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[1], objRotateROI, // Middle Pad Search ROI
                                                      m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(), 8,
                                    ref m_smVisionInfo.g_arrColorRotatedImages, 1);
                                }
                                else
                                {
                                    CROI.Rotate0Degree(objRotateROI, // Middle Pad Search ROI
                                                      m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(), 8,
                                    ref m_smVisionInfo.g_arrColorRotatedImages, 1);
                                }
                            }
                        }

                        m_smVisionInfo.g_blnViewRotatedImage = true;

                        if (m_smVisionInfo.g_arrPadColorROIs[i].Count > 0)
                            m_smVisionInfo.g_arrPadColorROIs[i][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    }
                }
            }

        }
        private void SavePadSettingOnly(string strFolderPath)
        {
            string strOrientPath = strFolderPath + "Orient\\";
            string strPkgPath = strFolderPath + "Package\\";
            strFolderPath = strFolderPath + "Pad\\";

            if (m_intVisionType == 0)
            {
                if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                    if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                        m_smVisionInfo.g_arrPadOrientROIs[0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
            }
            else
            {
                if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                    m_smVisionInfo.g_arrPadROIs[0][0].LoadROISetting(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight);
            }


            SaveROISetting(strFolderPath);
            //SaveOrientROISetting(strOrientPath);

            ////#region Save Orient Pattern Matching

            //////for (int v = 0; v < m_smVisionInfo.g_arrPadROIs.Count; v++)
            //////{
            //////    if (!m_smVisionInfo.g_arrPad[v].ref_blnSelected)
            //////        continue;
            //////    m_smVisionInfo.g_arrPadOrient[v].SetDontCareThreshold(1);
            //////    m_smVisionInfo.g_arrPadOrient[v].LearnPattern(m_smVisionInfo.g_arrPadROIs[v][0]);
            //////    m_smVisionInfo.g_arrPadOrient[v].SavePattern(strFolderPath + "Template\\Template" + v + ".mch");
            //////}
            ////if (m_smVisionInfo.g_arrPadOrientROIs.Count > 1)
            ////{
            ////    m_smVisionInfo.g_objPadOrient.SetDontCareThreshold(1);
            ////    m_smVisionInfo.g_objPadOrient.LearnPattern4Direction_SRM(m_smVisionInfo.g_arrPadOrientROIs[1]);
            ////    m_smVisionInfo.g_objPadOrient.SavePattern4Direction(strOrientPath + "Template\\", "PadOrientTemplate0");
            ////}
            ////#endregion

            //#region Save Unit Pattern Matching
            //if (m_smVisionInfo.g_arrPadOrientROIs.Count > 1)
            //{
            //    m_smVisionInfo.g_objPadOrient.LearnPattern4Direction_SRM(m_smVisionInfo.g_arrPadOrientROIs[1]);
            //    m_smVisionInfo.g_objPadOrient.SetFinalReduction(2);
            //    m_smVisionInfo.g_objPadOrient.SavePattern4Direction(strOrientPath + "Template\\", "PadUnitTemplate0");
            //}
            //#endregion

            //#region Save Orient Pattern Matching

            //if (m_smVisionInfo.g_arrPadOrientROIs.Count > 2)
            //{
            //    m_smVisionInfo.g_objPadOrient.LearnSubPattern(m_smVisionInfo.g_arrPadOrientROIs[2]);
            //    m_smVisionInfo.g_objPadOrient.SetFinalReduction(2);
            //    m_smVisionInfo.g_objPadOrient.SaveSubPattern(strOrientPath + "Template\\PadOrientTemplate0.mch");


            //}
            //#endregion

            #region Save Template Data and Images
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Pad\\";
            

            if (File.Exists(strFolderPath + "Template\\OriTemplate.bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Pad", "OriTemplate.bmp", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Pad", "", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "Old\\");

            // Save Learn Actual Image
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_smVisionInfo.g_arrColorImages[0].SaveImage(strFolderPath + "Template\\OriTemplate.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    m_smVisionInfo.g_arrColorImages[1].SaveImage(strFolderPath + "Template\\OriTemplate_Image1.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    m_smVisionInfo.g_arrColorImages[2].SaveImage(strFolderPath + "Template\\OriTemplate_Image2.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    m_smVisionInfo.g_arrColorImages[3].SaveImage(strFolderPath + "Template\\OriTemplate_Image3.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 4 && WantSaveImageAccordingMergeType(4))
                    m_smVisionInfo.g_arrColorImages[4].SaveImage(strFolderPath + "Template\\OriTemplate_Image4.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 5 && WantSaveImageAccordingMergeType(5))
                    m_smVisionInfo.g_arrColorImages[5].SaveImage(strFolderPath + "Template\\OriTemplate_Image5.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 6 && WantSaveImageAccordingMergeType(6))
                    m_smVisionInfo.g_arrColorImages[6].SaveImage(strFolderPath + "Template\\OriTemplate_Image6.bmp");
            }
            else
            {
                m_smVisionInfo.g_arrImages[0].SaveImage(strFolderPath + "Template\\OriTemplate.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    m_smVisionInfo.g_arrImages[1].SaveImage(strFolderPath + "Template\\OriTemplate_Image1.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    m_smVisionInfo.g_arrImages[2].SaveImage(strFolderPath + "Template\\OriTemplate_Image2.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    m_smVisionInfo.g_arrImages[3].SaveImage(strFolderPath + "Template\\OriTemplate_Image3.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 4 && WantSaveImageAccordingMergeType(4))
                    m_smVisionInfo.g_arrImages[4].SaveImage(strFolderPath + "Template\\OriTemplate_Image4.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 5 && WantSaveImageAccordingMergeType(5))
                    m_smVisionInfo.g_arrImages[5].SaveImage(strFolderPath + "Template\\OriTemplate_Image5.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 6 && WantSaveImageAccordingMergeType(6))
                    m_smVisionInfo.g_arrImages[6].SaveImage(strFolderPath + "Template\\OriTemplate_Image6.bmp");
            }

            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "New\\");

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                    continue;

                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                // 22-08-2019 ZJYEOH : Reset all inspection data so that drawing will not cause error when opening inspection result and learn pad on the same time
                m_smVisionInfo.g_arrPad[i].ResetInspectionData(false);

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

                m_smVisionInfo.g_arrPad[i].SavePadTemplateImage(strFolderPath + "Template\\",
                                                                m_smVisionInfo.g_arrRotatedImages[0],
                                                                m_smVisionInfo.g_arrPadROIs[i], i//);
                                                                , m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage);

                //
                //STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrPad[i].SavePad(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Template", strFolderPath, "Template\\Template.xml");

                // STDeviceEdit.CopySettingFile(strFolderPath, "RectGauge4L.xml");
                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SaveRectGauge4L(
                   strFolderPath + "RectGauge4L.xml",
                   false,
                   "Pad" + i.ToString(),
                   true,
                   true);
                //// STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Gauge4L", strFolderPath, "RectGauge4L.xml");
                //// Save Learn Template Search ROI image, unit image
                //m_smVisionInfo.g_arrPad[i].SavePadTemplateImage(strFolderPath + "Template\\",
                //                                                m_smVisionInfo.g_arrRotatedImages[0],
                //                                                m_smVisionInfo.g_arrPadROIs[i], i//);
                //                                                , m_smVisionInfo.g_objWhiteImage);

                ////if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                ////    m_smVisionInfo.g_arrPad[i].SavePadPackageTemplateImage(strPkgPath + "Template\\",
                ////                                                m_smVisionInfo.g_arrRotatedImages[0],
                ////                                                m_smVisionInfo.g_arrPadROIs[i], i);
            }

            //Load Package Image
            for (int intPadIndex = 0; intPadIndex < m_smVisionInfo.g_arrPad.Length; intPadIndex++)
            {
                if (intPadIndex > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_arrPad[intPadIndex].LoadPadTemplateImage(strFolderPath + "Template\\", intPadIndex);
                if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    m_smVisionInfo.g_arrPad[intPadIndex].LoadPadPackageTemplateImage(strPkgPath + "Template\\", intPadIndex);

            }


            
            #endregion
        }
        /// <summary>
        /// Save all pad settings 
        /// </summary>
        /// <param name="strFolderPath">xml folder path</param>
        private void SavePadSetting(string strFolderPath)
        {
            string strOrientPath = strFolderPath + "Orient\\";
            string strPkgPath = strFolderPath + "Package\\";
            strFolderPath = strFolderPath + "Pad\\";

            if (m_intVisionType == 0)
            {
                if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                    if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                        if(m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                        m_smVisionInfo.g_arrPadOrientROIs[0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
            }
            else
            {
                if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                    if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                        if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                            m_smVisionInfo.g_arrPadROIs[0][0].LoadROISetting(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight);
            }

            SaveROISetting(strFolderPath);
            //SaveOrientROISetting(strOrientPath);

            //#region Save Unit Pattern Matching
            //if (m_smVisionInfo.g_arrPadOrientROIs.Count > 1)
            //{
            //    m_smVisionInfo.g_objPadOrient.LearnPattern4Direction_SRM(m_smVisionInfo.g_arrPadOrientROIs[1]);
            //    m_smVisionInfo.g_objPadOrient.SetFinalReduction(2);
            //    m_smVisionInfo.g_objPadOrient.SavePattern4Direction(strOrientPath + "Template\\", "PadUnitTemplate0");
            //}
            //#endregion

            //#region Save Orient Pattern Matching
            
            //if (m_smVisionInfo.g_arrPadOrientROIs.Count > 2)
            //{
            //    m_smVisionInfo.g_objPadOrient.LearnSubPattern(m_smVisionInfo.g_arrPadOrientROIs[2]);
            //    m_smVisionInfo.g_objPadOrient.SetFinalReduction(2);
            //    m_smVisionInfo.g_objPadOrient.SaveSubPattern(strOrientPath + "Template\\PadOrientTemplate0.mch");
                
                
            //}
            //#endregion

            #region Save Template Data and Images
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Pad\\";
            

            if (File.Exists(strFolderPath + "Template\\OriTemplate.bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Pad", "OriTemplate.bmp", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Pad", "", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "Old\\");

            // Save Learn Actual Image
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_smVisionInfo.g_arrColorImages[0].SaveImage(strFolderPath + "Template\\OriTemplate.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    m_smVisionInfo.g_arrColorImages[1].SaveImage(strFolderPath + "Template\\OriTemplate_Image1.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    m_smVisionInfo.g_arrColorImages[2].SaveImage(strFolderPath + "Template\\OriTemplate_Image2.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    m_smVisionInfo.g_arrColorImages[3].SaveImage(strFolderPath + "Template\\OriTemplate_Image3.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 4 && WantSaveImageAccordingMergeType(4))
                    m_smVisionInfo.g_arrColorImages[4].SaveImage(strFolderPath + "Template\\OriTemplate_Image4.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 5 && WantSaveImageAccordingMergeType(5))
                    m_smVisionInfo.g_arrColorImages[5].SaveImage(strFolderPath + "Template\\OriTemplate_Image5.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 6 && WantSaveImageAccordingMergeType(6))
                    m_smVisionInfo.g_arrColorImages[6].SaveImage(strFolderPath + "Template\\OriTemplate_Image6.bmp");
            }
            else
            {
                m_smVisionInfo.g_arrImages[0].SaveImage(strFolderPath + "Template\\OriTemplate.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    m_smVisionInfo.g_arrImages[1].SaveImage(strFolderPath + "Template\\OriTemplate_Image1.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    m_smVisionInfo.g_arrImages[2].SaveImage(strFolderPath + "Template\\OriTemplate_Image2.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    m_smVisionInfo.g_arrImages[3].SaveImage(strFolderPath + "Template\\OriTemplate_Image3.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 4 && WantSaveImageAccordingMergeType(4))
                    m_smVisionInfo.g_arrImages[4].SaveImage(strFolderPath + "Template\\OriTemplate_Image4.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 5 && WantSaveImageAccordingMergeType(5))
                    m_smVisionInfo.g_arrImages[5].SaveImage(strFolderPath + "Template\\OriTemplate_Image5.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 6 && WantSaveImageAccordingMergeType(6))
                    m_smVisionInfo.g_arrImages[6].SaveImage(strFolderPath + "Template\\OriTemplate_Image6.bmp");
            }

            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "New\\");

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                    continue;

                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break; 

                // 22-08-2019 ZJYEOH : Reset all inspection data so that drawing will not cause error when opening inspection result and learn pad on the same time
                m_smVisionInfo.g_arrPad[i].ResetInspectionData(false);

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

                m_smVisionInfo.g_arrPad[i].SavePadTemplateImage(strFolderPath + "Template\\",
                                                              m_smVisionInfo.g_arrRotatedImages[0],
                                                              m_smVisionInfo.g_arrPadROIs[i], i//);
                                                              , m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage);

                //
                //STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrPad[i].SavePad(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Template", strFolderPath, "Template\\Template.xml");

                // STDeviceEdit.CopySettingFile(strFolderPath, "RectGauge4L.xml");
                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SaveRectGauge4L(
                   strFolderPath + "RectGauge4L.xml",
                   false,
                   "Pad" + i.ToString(),
                   true,
                   true);
                //// STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Gauge4L", strFolderPath, "RectGauge4L.xml");
                //// Save Learn Template Search ROI image, unit image
                //m_smVisionInfo.g_arrPad[i].SavePadTemplateImage(strFolderPath + "Template\\",
                //                                                m_smVisionInfo.g_arrRotatedImages[0],
                //                                                m_smVisionInfo.g_arrPadROIs[i], i//);
                //                                                , m_smVisionInfo.g_objWhiteImage);

                ////if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                ////    m_smVisionInfo.g_arrPad[i].SavePadPackageTemplateImage(strPkgPath + "Template\\",
                ////                                                m_smVisionInfo.g_arrRotatedImages[0],
                ////                                                m_smVisionInfo.g_arrPadROIs[i], i);
            }

            //Load Package Image
            for (int intPadIndex = 0; intPadIndex < m_smVisionInfo.g_arrPad.Length; intPadIndex++)
            {
                if (intPadIndex > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_arrPad[intPadIndex].LoadPadTemplateImage(strFolderPath + "Template\\", intPadIndex);
                if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    m_smVisionInfo.g_arrPad[intPadIndex].LoadPadPackageTemplateImage(strPkgPath + "Template\\", intPadIndex);

            }



            #endregion

            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (m_smVisionInfo.g_arrPin1 != null)
                {
                    // 2019 07 31 - CCENG: Fyi, both g_arrPadROIs[0][2] and ref_objPin1ROI attach to Pad Search ROI.
                    float fOffsetRefPosX = m_smVisionInfo.g_arrPadROIs[0][2].ref_ROICenterX -
                                           m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROICenterX;

                    float fOffsetRefPosY = m_smVisionInfo.g_arrPadROIs[0][2].ref_ROICenterY -
                                           m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROICenterY;

                    ROI objPin1ROI = m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI;
                    // Load Pin 1
                    m_smVisionInfo.g_arrPin1[0].LearnTemplate(m_smVisionInfo.g_intSelectedTemplate, fOffsetRefPosX, fOffsetRefPosY, objPin1ROI);

                    //
                    STDeviceEdit.CopySettingFile(strFolderPath + "Template\\", "Pin1Template.xml");
                    m_smVisionInfo.g_arrPin1[0].SaveTemplate(strFolderPath + "Template\\");
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Pad Pin1", m_smProductionInfo.g_strLotID);
                    
                }
            }
            
            if (m_smVisionInfo.g_blnWantDontCareArea_Pad)
            {
                SaveDontCareROISetting(strFolderPath);
                SaveDontCareSetting(strFolderPath + "Template\\");
            }
        }
        private bool WantSaveImageAccordingMergeType(int intImageIndex)
        {
            switch (m_smVisionInfo.g_intImageMergeType)
            {
                case 0:
                    return true;
                    break;
                case 1:
                    if (intImageIndex == 1)
                        return false;
                    break;
                case 2:
                    if ((intImageIndex == 1) || (intImageIndex == 2))
                        return false;
                    break;
                case 3:
                    if ((intImageIndex == 1) || (intImageIndex == 3))
                        return false;
                    break;
                case 4:
                    if ((intImageIndex == 1) || (intImageIndex == 2) || (intImageIndex == 4))
                        return false;
                    break;
            }

            return true;
        }
        private void SaveDontCareROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "DontCareROI.xml", false);
            
            for (int t = 0; t < m_smVisionInfo.g_arrPadDontCareROIs.Count; t++)
            {
                if (t > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[t].ref_blnSelected)
                    continue;

                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[t].Count; j++)
                {
                    //objROI = m_smVisionInfo.g_arrPadDontCareROIs[t][j];
                    objFile.WriteElement1Value("ROI" + j, "");

                    objFile.WriteElement2Value("Name", m_smVisionInfo.g_arrPadDontCareROIs[t][j].ref_strROIName);
                    objFile.WriteElement2Value("Type", m_smVisionInfo.g_arrPadDontCareROIs[t][j].ref_intType);
                    objFile.WriteElement2Value("PositionX", m_smVisionInfo.g_arrPadDontCareROIs[t][j].ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", m_smVisionInfo.g_arrPadDontCareROIs[t][j].ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", m_smVisionInfo.g_arrPadDontCareROIs[t][j].ref_ROIWidth);
                    objFile.WriteElement2Value("Height", m_smVisionInfo.g_arrPadDontCareROIs[t][j].ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", m_smVisionInfo.g_arrPadDontCareROIs[t][j].ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", m_smVisionInfo.g_arrPadDontCareROIs[t][j].ref_intStartOffsetY);
                    m_smVisionInfo.g_arrPadDontCareROIs[t][j].AttachImage(m_smVisionInfo.g_arrPadROIs[t][3]);

                }
            }
            objFile.WriteEndElement();


        }
        private void SaveROISetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "ROI.xml", false);

            ROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrPadROIs.Count; t++)
            {
                if (t > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[t].ref_blnSelected)
                    continue;

                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPadROIs[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrPadROIs[t][j];
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
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Pad ROI", m_smProductionInfo.g_strLotID);
            
        }
        private void SaveColorROISetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "CROI.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "CROI.xml", false);

            CROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrPadColorROIs.Count; t++)
            {
                if (t > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[t].ref_blnSelected)
                    continue;

                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPadColorROIs[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrPadColorROIs[t][j];
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
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad Color ROI", m_smProductionInfo.g_strLotID);

        }
        private void SaveOrientROISetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "ROI.xml", false);

            ROI objROI;
            objFile.WriteSectionElement("Unit" + 0, true);
            for (int t = 0; t < m_smVisionInfo.g_arrPadOrientROIs.Count; t++)
            {
                objROI = m_smVisionInfo.g_arrPadOrientROIs[t];
                objFile.WriteElement1Value("ROI" + t, "");

                objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                objFile.WriteElement2Value("Type", objROI.ref_intType);
                objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
            }
            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn PadOrient ROI", m_smProductionInfo.g_strLotID);
            
        }
        /// <summary>
        /// Save all positioning setting information such as Position ROI, Position Gauge, Position Setting
        /// </summary>
        /// <param name="strFolderPath"></param>
        private void SavePositioningSetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "Settings.xml");
            // Save Positioning Setting
            m_smVisionInfo.g_objPositioning.SavePosition(strFolderPath + "Settings.xml", false, "General", true);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Positioning", m_smProductionInfo.g_strLotID);

            // Learn Pattern
            m_smVisionInfo.g_objPositioning.LearnPattern(m_smVisionInfo.g_arrPadROIs[0][1]);
            m_smVisionInfo.g_arrPadROIs[0][1].SaveImage(strFolderPath + "Template\\PRSImage.bmp");
            //m_smVisionInfo.g_objPositioning.LearnPattern(m_smVisionInfo.g_arrPadROIs[0][0]);    // ByPass1
            //m_smVisionInfo.g_arrPadROIs[0][0].SaveImage(strFolderPath + "\\Template\\PRSImage.bmp");    // ByPass1
            m_smVisionInfo.g_objPositioning.SavePattern(strFolderPath + "Template\\PRS.mch");

            // Save Positioning ROI Setting
            STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
            ROI.SaveFile(strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Positioning ROI", m_smProductionInfo.g_strLotID);

            // Save Line Gauge Setting
            STDeviceEdit.CopySettingFile(strFolderPath, "Gauge.xml");
            LGauge.SaveFile(strFolderPath + "Gauge.xml", m_smVisionInfo.g_arrPositioningGauges);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Positioning Gauge", m_smProductionInfo.g_strLotID);
            
        }

        /// <summary>
        /// Set orient default settings
        /// </summary>
        private void SetDefaultSetting()
        {
            // Save Pad Orient PRS Setting
            //for (int i = 0; i < m_smVisionInfo.g_arrPadOrient.Length; i++)
            //{
            //    if (m_smVisionInfo.g_arrPad[i].ref_blnSelected)
            //    {
            //        m_smVisionInfo.g_arrPadOrient[i].ref_fMinScore = 0.7f;

            //        if (i == 0)
            //            m_smVisionInfo.g_arrPadOrient[i].SetAngleSetting(-10, 10);
            //        else
            //            m_smVisionInfo.g_arrPadOrient[i].SetAngleSetting(0, 0);

            //        m_smVisionInfo.g_arrPadOrient[i].SetScaleXSetting(1, 1);
            //        m_smVisionInfo.g_arrPadOrient[i].SetScaleYSetting(1, 1);
            //    }
            //}

            m_smVisionInfo.g_objPadOrient.ref_fMinScore = 0.7f;

            m_smVisionInfo.g_objPadOrient.SetAngleSetting(-10, 10);

            m_smVisionInfo.g_objPadOrient.SetScaleXSetting(1, 1);
            m_smVisionInfo.g_objPadOrient.SetScaleYSetting(1, 1);

        }

        /// <summary>
        /// Set pitch gap from pad no, to pad no
        /// </summary>
        /// <param name="intPitchGapIndex">pitch gap index</param>
        /// <param name="intCurrentFromPadNo">current from pad no</param>
        /// <param name="intCurrentToPadNo">current to pad no</param>
        /// <param name="intPadPosition">pad position</param>
        private void SetPitchGap(int intPitchGapIndex, int intCurrentFromPadNo, int intCurrentToPadNo, int intPadPosition)
        {
            int intLocationX = -1;
            int intLocationY = -1;

            bool blnFirstTime = true;
            while (true)
            {
                intPitchGapIndex = m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].GetTotalPitchGap();
                int intTotalPadNo = m_smVisionInfo.g_arrPad[intPadPosition].GetBlobsFeaturesNumber();
                SetPitchGapForm objSetPitchGapForm;
                if (blnFirstTime)
                    objSetPitchGapForm = new SetPitchGapForm(intTotalPadNo, intCurrentFromPadNo, intCurrentToPadNo);
                else if (intCurrentToPadNo >= (intTotalPadNo - 1))
                    objSetPitchGapForm = new SetPitchGapForm(intTotalPadNo, intCurrentFromPadNo, intCurrentToPadNo);
                else if (intCurrentFromPadNo >= (intTotalPadNo - 1))
                    objSetPitchGapForm = new SetPitchGapForm(intTotalPadNo, intCurrentFromPadNo, intCurrentToPadNo);
                else
                    objSetPitchGapForm = new SetPitchGapForm(intTotalPadNo, intCurrentFromPadNo + 1, intCurrentToPadNo + 1);
                if (intLocationX != -1)
                {
                    objSetPitchGapForm.StartPosition = FormStartPosition.Manual;
                    objSetPitchGapForm.Location = new Point(intLocationX, intLocationY);
                }
                if (objSetPitchGapForm.ShowDialog() == DialogResult.OK)
                {
                    int intFormPadNo = objSetPitchGapForm.ref_intFromPadNo;
                    int intToPadNo = objSetPitchGapForm.ref_intToPadNo;
                    if (intFormPadNo == intToPadNo)
                    {
                        SRMMessageBox.Show("Can not select same pad number!");
                    }
                    else if (m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkExist(intFormPadNo - 1, intToPadNo - 1))
                    {
                        SRMMessageBox.Show("This Pitch/Gap link already exist.");
                    }
                    else if (m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkInPadAlready(intFormPadNo - 1))
                    {
                        SRMMessageBox.Show("Pitch/Gap already defined in pad number " + intFormPadNo);
                    }
                    else if (!m_smVisionInfo.g_arrPad[intPadPosition].CheckPitchGapLinkAvailable(intFormPadNo - 1, intToPadNo - 1))
                    {
                        SRMMessageBox.Show("Pitch/Gap cannot be created in between pad no" + intFormPadNo + " and pad no" + intToPadNo + ".");
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPad[intPadPosition].SetPitchGap(intPitchGapIndex, intFormPadNo - 1, intToPadNo - 1);
                    }

                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    intCurrentFromPadNo = intFormPadNo - 1;
                    intCurrentToPadNo = intToPadNo - 1;
                    intLocationX = objSetPitchGapForm.Location.X;
                    intLocationY = objSetPitchGapForm.Location.Y;

                    ReadPadPitchGapToGrid(tabCtrl_PadPG.SelectedIndex, m_dgdViewPG);
                }
                else
                {
                    ReadPadPitchGapToGrid(tabCtrl_PadPG.SelectedIndex, m_dgdViewPG);
                    break;
                }

                blnFirstTime = false;
            }


        }

        /// <summary>
        /// Setup each learning steps
        /// </summary>
        private void SetupSteps(bool blnForward)
        {

            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: //Define Search ROI                  
                    //m_smVisionInfo.g_intSelectedImage = cbo_SelectImage.SelectedIndex;    // 2018 12 19 - JBTAN: For checking Pad and Pad package only will use Image 1
                    m_smVisionInfo.g_intSelectedImage = 0;
                    if (m_intVisionType == 0)
                        AddSearchROI(m_smVisionInfo.g_arrPadROIs);
                    else
                    {
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        else
                            m_smVisionInfo.g_arrImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_blnViewPackageImage = true;
                        AddOrientSearchROI(m_smVisionInfo.g_arrPadOrientROIs);
                        m_smVisionInfo.g_arrPadOrientROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);

                        if (!tp_SearchROI.Contains(gBox_PreciseAngle))
                            tp_SearchROI.Controls.Add(gBox_PreciseAngle);
                    }

                    if (m_blnOrientTestDone && m_blnOrientFail)
                    {
                        if (m_intVisionType == 0)
                        {
                            lbl_OrientFailMsg.Visible = true;
                            lbl_OrientationAngle.Text = "Fail";

                            if (m_smVisionInfo.g_blnViewRotatedImage)
                                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                            else
                                m_smVisionInfo.g_arrImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                            m_smVisionInfo.g_blnViewPackageImage = true;
                            AddOrientSearchROI(m_smVisionInfo.g_arrPadOrientROIs);
                            m_smVisionInfo.g_arrPadOrientROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                        }
                    }
                    else if (m_blnOrientTestDone && !m_blnOrientFail)
                    {
                        lbl_OrientFailMsg.Visible = false;
                        switch (m_intCurrentAngle)
                        {
                            default:
                            case 0:
                                lbl_OrientationAngle.Text = "0";
                                break;
                            case 90:
                                lbl_OrientationAngle.Text = "90";
                                break;
                            case 180:
                                lbl_OrientationAngle.Text = "180";
                                break;
                            case 270:
                                lbl_OrientationAngle.Text = "-90";
                                break;
                        }

                        if (m_smVisionInfo.g_arrPadOrientROIs.Count > 1)
                        {
                            float CenterX = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX + m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth / 2;
                            float CenterY = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY + m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight / 2;
                            float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX + m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Cos(-(m_intCurrentAngle + (int)Math.Round(m_smVisionInfo.g_fOrientAngle[0], 0)) * Math.PI / 180)) -
                   ((m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY + m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Sin(-(m_intCurrentAngle + (int)Math.Round(m_smVisionInfo.g_fOrientAngle[0], 0)) * Math.PI / 180)));
                            float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX + m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Sin(-(m_intCurrentAngle + (int)Math.Round(m_smVisionInfo.g_fOrientAngle[0], 0)) * Math.PI / 180)) +
                             ((m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY + m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Cos(-(m_intCurrentAngle + (int)Math.Round(m_smVisionInfo.g_fOrientAngle[0], 0)) * Math.PI / 180)));

                            m_smVisionInfo.g_arrPadOrientROIs[1].LoadROISetting(
                              (int)(fXAfterRotated - m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX - (m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROIWidth / 2)),
                              (int)(fYAfterRotated - m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY - (m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROIHeight / 2)),
                             m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROIHeight);
                        }
                        m_smVisionInfo.g_arrPadOrientROIs[0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                    }

                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    if (m_intVisionType == 0)
                        m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewRotatedImage = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;

                    if (m_blnOrientTestDone)
                    {
                        m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        //m_smVisionInfo.g_arrRotatedImages[0].SaveImage("D:\\img.bmp");
                        m_smVisionInfo.g_blnViewPackageImage = true;
                    }

                    lbl_StepNo.BringToFront();
                    lbl_TitleStep1.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_SearchROI;
                    btn_Previous.Enabled = false;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 1)
                    {
                        btn_Next.Enabled = false;
                    }
                    if (m_intVisionType == 2)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 2;
                        SetupSteps(true);
                    }
                    break;
                case 1:// Define positioning
                    AddPositioningSearchROI();

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex;
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(lbl_GainValue.Text));

                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewRotatedImage = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;

                    lbl_TitleStep1_1.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_position;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 2: //Define Unit ROI (Package)
                    m_smVisionInfo.g_blnViewROI = true;
                    
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize = chk_UseGauge.Checked;
                    }

                    SetUseGaugeGUI();

                    if (!blnForward)
                    {
                      
                        // 2020 04 01 - CCENG: Due to pad template has been deleted during build blob step, so need to reload back the template data if use press previous button
                        string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

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

                            m_smVisionInfo.g_arrPad[i].LoadPad(strPath + "Pad\\" + "Template\\Template.xml", strSectionName);

                            //2020-08-18 ZJYEOH : Need to set wantusegauge based on checkbox as curent checked value may not same after load previous pad setting
                            m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize = chk_UseGauge.Checked;
                        }

                       

                    }

                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        {
                            //m_smVisionInfo.g_arrPad[i].ref_intCheckPadDimensionImageIndex = m_smVisionInfo.g_intSelectedImage;    // 2020 01 06 - CCENG: pad dimension image index should always 0
                            m_smVisionInfo.g_arrPad[i].ref_intGaugeSizeImageIndex = m_smVisionInfo.g_intSelectedImage;
                        }
                    }

                    // Determine the image use for gauge measurement
                    //if (m_smVisionInfo.g_arrPad.Length > 1)
                    //    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPad[1].ref_intGaugeSizeImageIndex;
                    //else
                    // m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPad[0].ref_intGaugeSizeImageIndex;

                    //m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[0].ref_intPadPkgSizeImageViewNo, m_smVisionInfo.g_intVisionIndex);

                    if (chk_UseGauge.Checked)
                        DefineMostSelectedImageNo(); // 2020-02-17 ZJYEOH : Display the image selected from gauge advance setting

                    m_smVisionInfo.g_blnViewRotatedImage = false;

                    if (!m_blnOrientTestDone)
                        ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    else
                        ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_objPackageImage);

                    // Clear ROI drag handler
                    for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                    {
                        if (m_smVisionInfo.g_arrPadROIs[i].Count >= 2)
                            if (m_smVisionInfo.g_arrPadROIs[i][1].GetROIHandle())
                                m_smVisionInfo.g_arrPadROIs[i][1].ClearDragHandle();
                    }

                    AddTrainUnitROI(m_smVisionInfo.g_arrPadROIs);

                    if (!m_blnOrientTestDone)
                    {
                        if (chk_UseGauge.Checked)
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        else
                            RotatePrecise();
                    }
                    else
                        RotatePrecise();
                    //switch (m_smVisionInfo.g_arrPad[0].ref_objRectGauge4L.GetGaugeImageMode(0))
                    //{
                    //    default:
                    //    case 0:
                    //        {
                    //            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain);
                    //        }
                    //        break;
                    //    case 1:
                    //        {
                    //            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain);
                    //            m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                    //        }
                    //        break;
                    //}

                    m_smVisionInfo.g_blnViewPackageImage = true;

                    if (blnForward)
                    {
                        //// Hide the UseGauge Checkbox if Package is ON
                        //if (m_smVisionInfo.g_blnCheckPackage &&
                        //    ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                        //{
                        //    chk_UseGauge.Checked = true;
                        //    chk_UseGauge.Visible = false;
                        //    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        //    {
                        //        m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize = true;
                        //    }
                        //}
                        //else
                        {
                            chk_UseGauge.Visible = m_smVisionInfo.g_arrPad[0].ref_blnWantShowUseGaugeCheckBox && m_blnGaugeButtonVisible;//true; //2021-07-16 ZJYEOH : Saw this in advance setting so add here
                        }

                        int intLimitStartX = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth;
                        int intLimitStartY = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight;
                        int intLimitEndX = 0;
                        int intLimitEndY = 0;
                        // Set RectGauge4L Placement
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            PointF pCenter = new PointF(m_smVisionInfo.g_arrPadROIs[i][1].ref_ROITotalCenterX,
                                                        m_smVisionInfo.g_arrPadROIs[i][1].ref_ROITotalCenterY);

                            //m_smVisionInfo.g_arrPad[i].SetRectGauge4LPlacement(pCenter, 0, m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIHeight);
                            m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrPadROIs[i][0]);
                            m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();

                            m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ResetGaugeSettingToUserVariables();

                            bool blnGaugeResult;
                            if (m_smVisionInfo.g_blnViewPackageImage)
                            {
                                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                                {
                                    // 2020-03-24 ZJYEOH : no need measure center ROI gauge if use side pkg to measure center pkg
                                    blnGaugeResult = true;
                                }
                                else
                                {   //2020-04-05 ZJYEOH : Need use this new measure gauge function as all ROIs have separated Image/Gain
                                    if (!m_blnOrientTestDone)
                                        blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage, true); //m_smVisionInfo.g_objPackageImage
                                    else
                                        blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_objWhiteImage, true);
                                }
                            }
                            else
                            {
                                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                                {
                                    // 2020-03-24 ZJYEOH : no need measure center ROI gauge if use side pkg to measure center pkg
                                    blnGaugeResult = true;
                                }
                                else
                                {   //2020-04-05 ZJYEOH : Need use this new measure gauge function as all ROIs have separated Image/Gain
                                    if (!m_blnOrientTestDone)
                                        blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage, true);//m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]
                                    else
                                        blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_objWhiteImage, true);
                                }
                            }

                            if (!blnGaugeResult)
                            {
                                // if gauge measurement fail, display only the error message if chk_UseGauge is true.
                                if (chk_UseGauge.Checked)
                                {
                                    m_smVisionInfo.g_strErrorMessage = "*" + m_smVisionInfo.g_arrPad[i].ref_strErrorMessage;
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                            }

                            for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI.Count; j++)
                            {
                                if (intLimitStartX > m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionX)
                                    intLimitStartX = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionX;
                                if (intLimitStartY > m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionY)
                                    intLimitStartY = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionY;

                                if (intLimitEndX < m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionX + m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIWidth)
                                    intLimitEndX = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionX + m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIWidth;
                                if (intLimitEndY < m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionY + m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIHeight)
                                    intLimitEndY = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIPositionY + m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI[j].ref_ROIHeight;
                            }

                        }

                        m_smVisionInfo.g_objTopParentROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.g_objTopParentROI.LoadROISetting(intLimitStartX, intLimitStartY, intLimitEndX - intLimitStartX, intLimitEndY - intLimitStartY);
                    }


                    /* 2019 04 17 - CCENG: Since package learning steps are separated from LearnPadForm, so not need to check m_smCustomizeInfo.g_intWantPackage anymore.
                   *                     User are free to choose want to use gauge size or not whether g_intWantPackage is 1 or 0
                   */
                    //if ((m_smVisionInfo.g_blnCheckPackage && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)) ||
                    //    (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)))
                    if (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                    {
                        m_smVisionInfo.g_blnViewGauge = true;
                    }
                    else
                    {
                        m_smVisionInfo.g_blnViewGauge = false;
                    }
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;   // Update Step 2 Instruction Picture box
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    lbl_StepNo.BringToFront();
                    lbl_TitleStep2.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_Package;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 2)
                    {
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    if (m_intLearnType == 3)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 13;
                        SetupSteps(true);
                    }
                    if (m_intLearnType == 4)
                    {
                        btn_Next.PerformClick();
                    }
                    if (m_intLearnType == 5)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 4;
                        SetupSteps(true);
                    }
                    if (m_intLearnType == 6 || m_intLearnType == 7)
                    {
                        if (m_smVisionInfo.g_blnWantDontCareArea_Pad)
                            m_smVisionInfo.g_intLearnStepNo = 3;
                        else
                            m_smVisionInfo.g_intLearnStepNo = 5;
                        SetupSteps(true);
                    }
                    if (m_intVisionType == 2)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 5;
                        SetupSteps(true);
                    }
                    break;
                case 3: //Define Don't Care Area
                    if (m_intLearnType == 4 || m_intLearnType == 6 || m_intLearnType == 7)
                    {
                        if (chk_UseGauge.Checked)
                        {
                            bool blnGaugeOK = true;
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                                {
                                    if (m_smVisionInfo.g_arrPad[0].MeasureEdge_UsingSidePkgCornerPoint(
                                              m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X,
                                              m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y,
                                              m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X,
                                              m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y,
                                              m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X,
                                              m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y,
                                              m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X,
                                              m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        SRMMessageBox.Show("Cannot form Center ROI. Please adjust Side ROI or gauge setting.");

                                        blnGaugeOK = false;
                                        break;
                                    }
                                }


                                if (!m_smVisionInfo.g_arrPad[i].IsGaugeMeasureOK(i))
                                {
                                    string strDirectionName;
                                    switch (i)
                                    {
                                        case 0:
                                        default:
                                            strDirectionName = "Center";
                                            break;
                                        case 1:
                                            strDirectionName = "Top";
                                            break;
                                        case 2:
                                            strDirectionName = "Right";
                                            break;
                                        case 3:
                                            strDirectionName = "Bottom";
                                            break;
                                        case 4:
                                            strDirectionName = "Left";
                                            break;

                                    }
                                    SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");

                                    blnGaugeOK = false;
                                    break;

                                }
                            }

                            if (!blnGaugeOK)
                            {
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }
                        }

                        // Determine the image use for gauge measurement
                        m_smVisionInfo.g_intSelectedImage = 0;

                    }
                    else
                    {
                        if (chk_UseGauge.Checked)
                        {
                            bool blnGaugeOK = true;
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                                {
                                    if (m_smVisionInfo.g_arrPad[0].MeasureEdge_UsingSidePkgCornerPoint(
                                              m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X,
                                              m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y,
                                              m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X,
                                              m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y,
                                              m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X,
                                              m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y,
                                              m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X,
                                              m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        SRMMessageBox.Show("Cannot form Center ROI. Please adjust Side ROI or gauge setting.");
                                        m_smVisionInfo.g_intLearnStepNo--;
                                        m_intDisplayStepNo--;
                                        blnGaugeOK = false;
                                        break;
                                    }
                                }


                                if (!m_smVisionInfo.g_arrPad[i].IsGaugeMeasureOK(i))
                                {
                                    string strDirectionName;
                                    switch (i)
                                    {
                                        case 0:
                                        default:
                                            strDirectionName = "Center";
                                            break;
                                        case 1:
                                            strDirectionName = "Top";
                                            break;
                                        case 2:
                                            strDirectionName = "Right";
                                            break;
                                        case 3:
                                            strDirectionName = "Bottom";
                                            break;
                                        case 4:
                                            strDirectionName = "Left";
                                            break;

                                    }
                                    SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");
                                    m_smVisionInfo.g_intLearnStepNo--;
                                    m_intDisplayStepNo--;
                                    blnGaugeOK = false;
                                    break;

                                }
                            }

                            if (!blnGaugeOK)
                            {
                                break;
                            }
                        }

                        // Determine the image use for gauge measurement
                        m_smVisionInfo.g_intSelectedImage = cbo_SelectImage.SelectedIndex;

                        if (blnForward)
                        {
                            // Set last gauge measurement size as Unit size template
                            SetLastGaugeMeasureSizeAsTemplateUnitSize();
                        }
                    }
                  
                    RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                    if (m_smVisionInfo.g_blnViewColorImage)
                        RotateColorImage();

                    // Add Package ROI
                    AddTrainPackageROI(m_smVisionInfo.g_arrPadROIs);
                    AddTrainPadROI(m_smVisionInfo.g_arrPadROIs);

                    // 2020 06 18 - Make sure Dont Care ROI have parents. Will have "no image ancestor for this ROI" error if no attach to parents.
                    for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                    {
                        if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                        {
                            if (m_smVisionInfo.g_arrPadDontCareROIs.Count > i)
                            {
                                for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                                {
                                    m_smVisionInfo.g_arrPadDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                                }
                            }

                        }
                    }

                    for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Pad.Count; u++)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Pad[u].Count; i++)
                        {
                            m_smVisionInfo.g_arrPolygon_Pad[u][i].ClearPolygon();
                        }
                    }

                    //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    //{
                    //    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    //        break;

                    //    m_smVisionInfo.g_arrPolygon_Pad[i][0].CheckDontCarePosition(m_smVisionInfo.g_arrPadROIs[i][2], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    //}

                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;

                    lbl_TitleStep3.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_DontCare;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 4)
                    {
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                        tp_DontCare.Controls.Add(btn_SaveSubLearn);
                    }
                    if (m_intLearnType == 6 || m_intLearnType == 7)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 5;
                        SetupSteps(true);
                    }
                    break;
                case 4:
                    if (m_intLearnType == 5)
                    {
                        if (chk_UseGauge.Checked)
                        {
                            bool blnGaugeOK = true;
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                                {
                                    if (m_smVisionInfo.g_arrPad[0].MeasureEdge_UsingSidePkgCornerPoint(
                                         m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X,
                                         m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y,
                                         m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X,
                                         m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y,
                                         m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X,
                                         m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y,
                                         m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X,
                                         m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        SRMMessageBox.Show("Cannot form Center ROI. Please adjust Side ROI or gauge setting.");

                                        blnGaugeOK = false;
                                        break;
                                    }
                                }

                                if (!m_smVisionInfo.g_arrPad[i].IsGaugeMeasureOK(i))
                                {
                                    string strDirectionName;
                                    switch (i)
                                    {
                                        case 0:
                                        default:
                                            strDirectionName = "Center";
                                            break;
                                        case 1:
                                            strDirectionName = "Top";
                                            break;
                                        case 2:
                                            strDirectionName = "Right";
                                            break;
                                        case 3:
                                            strDirectionName = "Bottom";
                                            break;
                                        case 4:
                                            strDirectionName = "Left";
                                            break;

                                    }
                                    SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");

                                    blnGaugeOK = false;
                                    break;

                                }
                            }

                            if (!blnGaugeOK)
                            {
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }
                        }

                        // Determine the image use for gauge measurement
                        m_smVisionInfo.g_intSelectedImage = 0;

                        RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                        if (m_smVisionInfo.g_blnViewColorImage)
                            RotateColorImage();

                        // Add Package ROI
                        AddTrainPackageROI(m_smVisionInfo.g_arrPadROIs);
                        AddTrainPadROI(m_smVisionInfo.g_arrPadROIs);
                        AddPadROIToPin1SearchROI(m_smVisionInfo.g_arrPadROIs);
                        if (!StartPin1Test())
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                    else
                    {
                        if (!m_smVisionInfo.g_blnWantDontCareArea_Pad)  // 2019 12 02 - CCENG: If no dont care area checking, then will check the gauge and defind pad roi in this pin 1 step.
                        {
                            if (chk_UseGauge.Checked)
                            {
                                bool blnGaugeOK = true;
                                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                {
                                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                        break;

                                    if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                                    {
                                        if (m_smVisionInfo.g_arrPad[0].MeasureEdge_UsingSidePkgCornerPoint(
                                             m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X,
                                             m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y,
                                             m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X,
                                             m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y,
                                             m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X,
                                             m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y,
                                             m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X,
                                             m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            SRMMessageBox.Show("Cannot form Center ROI. Please adjust Side ROI or gauge setting.");
                                            m_smVisionInfo.g_intLearnStepNo -= 2;
                                            m_intDisplayStepNo--;
                                            blnGaugeOK = false;
                                            break;
                                        }
                                    }

                                    if (!m_smVisionInfo.g_arrPad[i].IsGaugeMeasureOK(i))
                                    {
                                        string strDirectionName;
                                        switch (i)
                                        {
                                            case 0:
                                            default:
                                                strDirectionName = "Center";
                                                break;
                                            case 1:
                                                strDirectionName = "Top";
                                                break;
                                            case 2:
                                                strDirectionName = "Right";
                                                break;
                                            case 3:
                                                strDirectionName = "Bottom";
                                                break;
                                            case 4:
                                                strDirectionName = "Left";
                                                break;

                                        }
                                        SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");
                                        m_smVisionInfo.g_intLearnStepNo -= 2;
                                        m_intDisplayStepNo--;
                                        blnGaugeOK = false;
                                        break;

                                    }
                                }

                                if (!blnGaugeOK)
                                {
                                    break;
                                }
                            }

                            // Determine the image use for gauge measurement
                            m_smVisionInfo.g_intSelectedImage = cbo_SelectImage.SelectedIndex;

                            if (blnForward)
                            {
                                // Set last gauge measurement size as Unit size template
                                SetLastGaugeMeasureSizeAsTemplateUnitSize();
                            }

                            RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                            if (m_smVisionInfo.g_blnViewColorImage)
                                RotateColorImage();

                            // Add Package ROI
                            AddTrainPackageROI(m_smVisionInfo.g_arrPadROIs);
                            AddTrainPadROI(m_smVisionInfo.g_arrPadROIs);
                        }
                        else
                        {
                           
                            RotateImage(); // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 5. 
                        }
                    }

                    AddPadROIToPin1SearchROI(m_smVisionInfo.g_arrPadROIs);

                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        if (m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI == null)
                        {
                            m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI = new ROI();
                            m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.LoadROISetting(0, 0, 100, 100);
                        }
                        m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.ref_strROIName = "Pin1";
                        m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.ref_intType = 2;
                        m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.AttachImage(m_smVisionInfo.g_arrPin1[i].ref_objSearchROI); // Train ROI always attach to Search ROI
                    }

                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = true;
                    m_smVisionInfo.g_blnViewPin1ROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    lbl_TitleStep3_1.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_PadPin1;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 5)
                    {
                        tp_PadPin1.Controls.Add(btn_SaveSubLearn);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
                case 5: //Build Object/Edition
                    if (!m_smVisionInfo.g_blnWantDontCareArea_Pad && !m_smVisionInfo.g_blnWantPin1)  // 2019 12 13 - JBTAN: if dont want dont care and dont want pin 1, , then will check the gauge and defind pad roi in this step.
                    {
                        if (chk_UseGauge.Checked)
                        {
                            bool blnGaugeOK = true;
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                                {
                                    if (m_smVisionInfo.g_arrPad[0].MeasureEdge_UsingSidePkgCornerPoint(
                                         m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X,
                                         m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y,
                                         m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X,
                                         m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y,
                                         m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X,
                                         m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y,
                                         m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X,
                                         m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (m_intVisionType == 2)
                                        {
                                            break;
                                        }
                                        SRMMessageBox.Show("Cannot form Center ROI. Please adjust Side ROI or gauge setting.");
                                        if (m_smVisionInfo.g_blnWantDontCareArea_Pad)
                                            m_smVisionInfo.g_intLearnStepNo -= 2;
                                        else
                                            m_smVisionInfo.g_intLearnStepNo -= 3;
                                        m_intDisplayStepNo--;
                                        blnGaugeOK = false;
                                        break;
                                    }
                                }

                                if (!m_smVisionInfo.g_arrPad[i].IsGaugeMeasureOK(i))
                                {
                                    if (m_intVisionType == 2)
                                    {
                                        break;
                                    }
                                    string strDirectionName;
                                    switch (i)
                                    {
                                        case 0:
                                        default:
                                            strDirectionName = "Center";
                                            break;
                                        case 1:
                                            strDirectionName = "Top";
                                            break;
                                        case 2:
                                            strDirectionName = "Right";
                                            break;
                                        case 3:
                                            strDirectionName = "Bottom";
                                            break;
                                        case 4:
                                            strDirectionName = "Left";
                                            break;

                                    }
                                    SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");
                                    if (m_smVisionInfo.g_blnWantDontCareArea_Pad)
                                        m_smVisionInfo.g_intLearnStepNo -= 2;
                                    else
                                        m_smVisionInfo.g_intLearnStepNo -= 3;
                                    m_intDisplayStepNo--;
                                    blnGaugeOK = false;
                                    break;

                                }
                            }

                            if (!blnGaugeOK)
                            {
                                break;
                            }
                        }

                        // Determine the image use for gauge measurement
                        m_smVisionInfo.g_intSelectedImage = cbo_SelectImage.SelectedIndex;

                        if (blnForward)
                        {
                            // Set last gauge measurement size as Unit size template
                            SetLastGaugeMeasureSizeAsTemplateUnitSize();
                        }

                        RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                        // Add Package ROI
                        AddTrainPackageROI(m_smVisionInfo.g_arrPadROIs);
                        AddTrainPadROI(m_smVisionInfo.g_arrPadROIs);
                    }
                    else
                    {
                        //2020-05-04 ZJYEOH : Need to attach m_smVisionInfo.g_arrPadROIs[i][3] to m_smVisionInfo.g_objPackageImage, so that drawing will have dont care area
                        for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                                continue;

                            m_smVisionInfo.g_arrPadROIs[i][3].AttachImage(m_smVisionInfo.g_objPackageImage);
                        }
                    }

                    // 2021 04 01 - CCENG: If no gauge, hide the Pad ROI Tolerance Setting button. 
                    btn_PadROIToleranceSetting.Visible = m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && m_blnPadROIButtonVisible;

                    if (!m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                    {
                        // 2021 04 01 - CCENG: if no gauge, no need to set pad ROI tolerance, so all will set to 0.
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = 0;
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = 0;
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = 0;
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = 0;
                        }

                        // 2021 04 01 - CCENG: If no gauge, the arrPadROI[1] == arrPadROI[3] size, mean pad pattern ROI will same as pad Inspection ROI
                        for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                        {
                            if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                            {
                                m_smVisionInfo.g_arrPadROIs[i][3].LoadROISetting(m_smVisionInfo.g_arrPadROIs[i][1].ref_ROITotalX, m_smVisionInfo.g_arrPadROIs[i][1].ref_ROITotalY,
                                    m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIHeight);
                            }
                        }
                    }

                    m_blnStepPadDone = false;

                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                    //temporary create dont care template for build object
                    //if (m_smVisionInfo.g_blnWantDontCareArea_Pad)
                    //{
                    //    ImageDrawing objImage = new ImageDrawing();
                    //    ROI objDontCareROI = new ROI();
                    //    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    //    {
                    //        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    //            break;

                    //        Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][2], m_smVisionInfo.g_arrPolygon_Pad[i][0], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    //        objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight);
                    //        objDontCareROI.AttachImage(objImage);
                    //        ROI.SubtractROI(m_smVisionInfo.g_arrPadROIs[i][2], objDontCareROI);
                    //    }
                    //    objDontCareROI.Dispose();
                    //    objImage.Dispose();
                    //}
                    if (m_intVisionType != 2)
                    {
                        // 2020 06 27 - CCENG: Make sure change image sensitivity before add dont care to image.
                        for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                                continue;

                            // Attach roi on rotated image
                            m_smVisionInfo.g_arrPadROIs[i][3].AttachImage(m_smVisionInfo.g_objPackageImage);

                            ImageDrawing objImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                            m_smVisionInfo.g_arrPad[i].SensitivityOnPadROI(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPadROIs[i][0]);

                            //if (m_smVisionInfo.g_arrPadROIs[i].Count > 4) // Dont Care ROI start from index 3
                            //{
                            //    ROI.ModifyImageGain(m_smVisionInfo.g_arrPadROIs[i][0], m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                            //    // Fill dont care area 
                            //    for (int j = 4; j < m_smVisionInfo.g_arrPadROIs[i].Count; j++)
                            //    {
                            //        m_smVisionInfo.g_arrPadROIs[i][j].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);

                            //        m_smVisionInfo.g_arrPadROIs[i][j].FillROI(0);
                            //    }
                            //}

                            // Backup current builded blobsFeature before rearrange   
                            if (!m_blnPreviousPadBackup)
                                m_smVisionInfo.g_arrPad[i].BackupBlobsFeatures();
                        }
                        m_blnPreviousPadBackup = true;//2021-08-04 ZJYEOH : Load for the first time only

                        if (m_smVisionInfo.g_blnWantDontCareArea_Pad)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPadDontCareROIs.Count; i++)
                            {
                                for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                                {
                                    if (m_smVisionInfo.g_arrPolygon_Pad[i][j].ref_intFormMode != 2)
                                    {
                                        m_smVisionInfo.g_arrPolygon_Pad[i][j].AddPoint(new PointF(m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                                            m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                                        m_smVisionInfo.g_arrPolygon_Pad[i][j].AddPoint(new PointF((m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROITotalX + m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                                            (m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROITotalY + m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                                        m_smVisionInfo.g_arrPolygon_Pad[i][j].AddPolygon((int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrPolygon_Pad[i][j].ResetPointsUsingOffset(m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                                        m_smVisionInfo.g_arrPolygon_Pad[i][j].AddPolygon((int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                                    }
                                    ImageDrawing objImage = new ImageDrawing(true);
                                    if (m_smVisionInfo.g_arrPolygon_Pad[i][j].ref_intFormMode != 2)
                                        Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                                    else
                                        Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                                    ROI objDontCareROI = new ROI();
                                    objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIHeight);
                                    objDontCareROI.AttachImage(objImage);
                                    ROI.SubtractROI(m_smVisionInfo.g_arrPadROIs[i][3], objDontCareROI);
                                    objDontCareROI.Dispose();
                                    objImage.Dispose();
                                }
                            }
                        }

                        if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_arrPad[1].ref_blnWantConsiderPadImage2)
                        {
                            if (!BuildObjects_ConsiderImage2())
                            {
                                m_smVisionInfo.g_intLearnStepNo--;
                                m_intDisplayStepNo--;
                                break;
                            }
                        }
                        else
                        {
                            if (!BuildObjectsAndSetToTemplateArray())
                            {
                                m_smVisionInfo.g_intLearnStepNo--;
                                m_intDisplayStepNo--;
                                break;
                            }
                        }
                    }

                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;

                    m_blnIdentityPadsDone = false;
                    m_blnKeepPrevObject = false;
                    lbl_TitleStep4.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_Edition;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 6 || m_intLearnType == 7)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 6;
                        SetupSteps(true);
                    }
                    if (m_intVisionType == 2)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 16;
                        SetupSteps(true);
                    }
                    break;
                case 6: // Identify Pad No

                    /*
                     * 2018 09 18 - CCENG: 
                     * - If current blobs match with previous template, then will copy the previous template (m_arrTemporaryBlobPads) to current template.
                     * - If user change the template pad number, enable or disable or swap width heigt. Once user change the labeling corner or direction. 
                     *   The setting of each pad will roll back to the previous template. This is because changing the labeling will cause confusing of template pad number setting.
                     * 
                     */

                    if (blnForward)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            m_smVisionInfo.g_arrPad[i].BackupBlobsFeaturesForReset();
                        }

                        if (CheckOutOfSearchROI() != -1)
                        {
                            string name = "";
                            switch (CheckOutOfSearchROI())
                            {
                                case 0:
                                    name = "Center Pad ROI";
                                    break;
                                case 1:
                                    name = "Top Pad ROI";
                                    break;
                                case 2:
                                    name = "Right Pad ROI";
                                    break;
                                case 3:
                                    name = "Bottom Pad ROI";
                                    break;
                                case 4:
                                    name = "Left Pad ROI";
                                    break;
                            }

                            SRMMessageBox.Show(name + ": Search ROI smaller than Pad ROI, Please Readjust Search ROI");
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            break;
                        }

                        //// Remove unused tab page
                        //tabCtrl_PadSide.Controls.Remove(tp_Top);
                        //tabCtrl_PadSide.Controls.Remove(tp_Right);
                        //tabCtrl_PadSide.Controls.Remove(tp_Bottom);
                        //tabCtrl_PadSide.Controls.Remove(tp_Left);

                        //if (m_smVisionInfo.g_blnCheck4Sides && m_smVisionInfo.g_arrPad.Length > 1)
                        //{
                        //    tabCtrl_PadSide.Controls.Add(tp_Top);
                        //    tabCtrl_PadSide.Controls.Add(tp_Right);
                        //    tabCtrl_PadSide.Controls.Add(tp_Bottom);
                        //    tabCtrl_PadSide.Controls.Add(tp_Left);
                        //}
                    }
                    else
                    {
                        if (IsPitchGapSettingError())
                        {
                            m_smVisionInfo.g_intLearnStepNo++;
                            m_intDisplayStepNo++;
                            break;
                        }

                        //TabCtrlPadSideTabPageChange();
                    }

                    int intSelectedROI = GetSelectedROI();
                    cbo_ReferenceCorner.SelectedIndex = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadLabelRefCorner;
                    cbo_PadLabelDirection.SelectedIndex = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadLabelDirection;

                    m_smVisionInfo.g_blnViewObjectsBuilded = true;
                    string[] strROIName;
                    m_strPosition = "";
                    if (m_intLearnType == 6 || m_intLearnType == 7)
                    {
                        if (!m_blnIdentityPadsDone)
                        {
                            strROIName = new string[m_smVisionInfo.g_arrPadROIs.Count];

                            //Define ROI name for display in message box
                            for (int y = 0; y < m_smVisionInfo.g_arrPadROIs.Count; y++)
                            {
                                if (y > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                switch (y)
                                {
                                    case 0:
                                        strROIName[y] = "Center Pads";
                                        break;
                                    case 1:
                                        strROIName[y] = "Up Pads";
                                        break;
                                    case 2:
                                        strROIName[y] = "Right Pads";
                                        break;
                                    case 3:
                                        strROIName[y] = "Down Pads";
                                        break;
                                    case 4:
                                        strROIName[y] = "Left Pads";
                                        break;
                                }
                            }

                            // Make sure at lease 1 pad is selected.
                            int intTotalObjectNumber = 0;
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                                    continue;

                                intTotalObjectNumber += m_smVisionInfo.g_arrPad[i].GetSelectedObjectNumber();
                            }
                            if (intTotalObjectNumber == 0)
                            {
                                m_smVisionInfo.g_strErrorMessage += "*Minimum 1 pad is required. Please relearn using wizard";
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                btn_SaveSubLearn.Enabled = false;
                                return;
                            }

                            if (m_smVisionInfo.g_arrPad[0].ref_blnWantUseClosestSizeDefineTolerance)
                            {
                                //if (!DefinePadToleranceUsingClosestSizeMethod(strROIName))
                                //{
                                //    m_smVisionInfo.g_intLearnStepNo--;
                                //    m_intDisplayStepNo--;
                                //    return;
                                //}

                                ReadAllPadTemplateDataToGrid_ClosestSizeMethod();
                            }
                            else
                            {

                                //if (!DefinePadToleranceUsingDefaultSetting(strROIName))
                                //{
                                //    m_smVisionInfo.g_intLearnStepNo--;
                                //    m_intDisplayStepNo--;
                                //    return;
                                //}

                                ReadAllPadTemplateDataToGrid_DefaultMethod();
                            }

                            if (!chk_ResetPadCustomize.Checked)
                            {
                                string strFolderPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

                                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                {
                                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                        break;

                                    m_smVisionInfo.g_arrPad[i].ResetPercentage();

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
                                    chk_ResetPadCustomize.Enabled = false;
                                    m_smVisionInfo.g_arrPad[i].LoadPercentageTemporary_ByPosition(strFolderPath + "Pad\\Template\\Template.xml", strSectionName);
                                    chk_ResetPadCustomize.Enabled = true;
                                    m_smVisionInfo.g_arrPad[i].RearrangeBlobs();//2021-08-05 ZJYEOH : Remove those pad not in previous recipe
                                }
                            }
                            else
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                {
                                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                        break;

                                    m_smVisionInfo.g_arrPad[i].ResetPercentage();

                                    //m_smVisionInfo.g_arrPad[i].ref_blnTestAllPad = true;
                                }

                                //m_smVisionInfo.AT_VM_OfflineTestAllPad = true;
                                //TriggerOfflineTest(); // Temporary hide until solve the problem when cant find point 

                                //WaitEventDone(ref m_smVisionInfo.PR_MN_UpdateInfo, true, "WaitTestDone");

                                //if (m_smVisionInfo.PR_MN_UpdateInfo)
                                //{
                                //    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                //    {
                                //        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                //            break;

                                //        if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                                //            continue;

                                //        m_smVisionInfo.g_arrPad[i].ResetPointGaugeInwardPercentage();
                                //    }

                                //    m_smVisionInfo.PR_MN_UpdateInfo = false;
                                //}
                            }
                        }
                    }
                    else
                    {
                        if (!m_blnIdentityPadsDone)
                        {
                            strROIName = new string[m_smVisionInfo.g_arrPadROIs.Count];

                            //Define ROI name for display in message box
                            for (int y = 0; y < m_smVisionInfo.g_arrPadROIs.Count; y++)
                            {
                                if (y > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                switch (y)
                                {
                                    case 0:
                                        strROIName[y] = "Center Pads";
                                        break;
                                    case 1:
                                        strROIName[y] = "Up Pads";
                                        break;
                                    case 2:
                                        strROIName[y] = "Right Pads";
                                        break;
                                    case 3:
                                        strROIName[y] = "Down Pads";
                                        break;
                                    case 4:
                                        strROIName[y] = "Left Pads";
                                        break;
                                }
                            }

                            // Make sure at lease 1 pad is selected.
                            int intTotalObjectNumber = 0;
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                                    continue;

                                intTotalObjectNumber += m_smVisionInfo.g_arrPad[i].GetSelectedObjectNumber();
                            }
                            if (intTotalObjectNumber == 0)
                            {
                                SRMMessageBox.Show("Minimum 1 pad is required.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                m_smVisionInfo.g_intLearnStepNo--;
                                m_intDisplayStepNo--;
                                return;
                            }

                            if (m_smVisionInfo.g_arrPad[0].ref_blnWantUseClosestSizeDefineTolerance)
                            {
                                if (!DefinePadToleranceUsingClosestSizeMethod(strROIName))
                                {
                                    m_smVisionInfo.g_intLearnStepNo--;
                                    m_intDisplayStepNo--;
                                    return;
                                }

                                ReadAllPadTemplateDataToGrid_ClosestSizeMethod();
                            }
                            else
                            {

                                if (!DefinePadToleranceUsingDefaultSetting(strROIName))
                                {
                                    m_smVisionInfo.g_intLearnStepNo--;
                                    m_intDisplayStepNo--;
                                    return;
                                }

                                ReadAllPadTemplateDataToGrid_DefaultMethod();
                            }

                            if (!chk_ResetPadCustomize.Checked)
                            {
                                string strFolderPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

                                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                {
                                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                        break;

                                    m_smVisionInfo.g_arrPad[i].ResetPercentage();

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
                                    chk_ResetPadCustomize.Enabled = false;
                                    m_smVisionInfo.g_arrPad[i].LoadPercentageTemporary_ByPosition(strFolderPath + "Pad\\Template\\Template.xml", strSectionName);
                                    chk_ResetPadCustomize.Enabled = true;
                                    m_smVisionInfo.g_arrPad[i].RearrangeBlobs();//2021-08-05 ZJYEOH : Remove those pad not in previous recipe
                                }
                            }
                            else
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                {
                                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                        break;

                                    m_smVisionInfo.g_arrPad[i].ResetPercentage();

                                    //m_smVisionInfo.g_arrPad[i].ref_blnTestAllPad = true;
                                }

                                //m_smVisionInfo.AT_VM_OfflineTestAllPad = true;
                                //TriggerOfflineTest(); // Temporary hide until solve the problem when cant find point 

                                //WaitEventDone(ref m_smVisionInfo.PR_MN_UpdateInfo, true, "WaitTestDone");

                                //if (m_smVisionInfo.PR_MN_UpdateInfo)
                                //{
                                //    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                //    {
                                //        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                //            break;

                                //        if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                                //            continue;

                                //        m_smVisionInfo.g_arrPad[i].ResetPointGaugeInwardPercentage();
                                //    }

                                //    m_smVisionInfo.PR_MN_UpdateInfo = false;
                                //}
                            }

                        }
                    }

                    lbl_TitleStep5.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_CustomizePad; //tp_IdentifyPad;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    m_blnStepPadDone = true;
                    if (m_intLearnType == 6)
                    {
                        tp_CustomizePad.Controls.Add(btn_SaveSubLearn);
                        btn_Next.Enabled = false;
                        btn_Previous.Enabled = false;
                    }
                    if (m_intLearnType == 7)
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                        SetupSteps(true);
                    }
                    break;
                case 7:// Identify Pitch and Gap
                    m_blnStepPadDone = false;

                    if (blnForward)
                    {
                        //// Check is pad datagrid setting ok
                        //if (IsSettingError())
                        //{
                        //    m_smVisionInfo.g_intLearnStepNo--;
                        //    m_intDisplayStepNo--;
                        //    break;
                        //}

                        tabCtrl_PadPG.Controls.Remove(tp_TopROI);
                        tabCtrl_PadPG.Controls.Remove(tp_RightROI);
                        tabCtrl_PadPG.Controls.Remove(tp_BottomROI);
                        tabCtrl_PadPG.Controls.Remove(tp_LeftROI);

                        if (m_smVisionInfo.g_blnCheck4Sides && m_smVisionInfo.g_arrPad.Length > 1)
                        {

                            tabCtrl_PadPG.Controls.Add(tp_TopROI);
                            tabCtrl_PadPG.Controls.Add(tp_RightROI);
                            tabCtrl_PadPG.Controls.Add(tp_BottomROI);
                            tabCtrl_PadPG.Controls.Add(tp_LeftROI);
                        }

                        for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                        {
                            if (i == 0)
                            {
                                m_smVisionInfo.g_arrPadROIs[i][0].VerifyROIArea(
                                    m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX,
                                    m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrPadROIs[i][0].VerifyROIArea(0, 0);
                            }
                        }
                    }

                    m_strPosition = "";

                    //Customize GUI
                    btn_Next.Enabled = true;

                    if (m_smVisionInfo.g_arrPad[0].ref_blnWantUseClosestSizeDefineTolerance)
                    {
                        // Rearrange pitch gap based on latest selected pads
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            m_smVisionInfo.g_arrPad[i].BuildPadPitchGapLink(false);
                        }
                    }

                    bool bRedefinePitchGap = true;
                    if (bRedefinePitchGap)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            // Define Group No first before Define Pitch Gap, because pitch gap rely on group no.
                            m_smVisionInfo.g_arrPad[i].AutoDefineGroupNoToTemplate();

                            //2021-07-29 ZJYEOH : Open back AutoDefinePitchGap(i) because if no auto define will cause index out of range when previous pitch gap setting number(from pad/ to pad index) more than current pad count
                            AutoDefinePitchGap(i);    // 2021 06 08 - CCENG : No auto define. Should base on previous pad setting.

                            if (!chk_ResetPitchGap.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].LoadGroupNoTemporary();
                                //AutoDefinePitchGap_NoReset(i);
                                //2021-07-29 ZJYEOH : Open back LoadPitchGapLinkTemporary() because if no auto define will cause index out of range when previous pitch gap setting number(from pad/ to pad index) more than current pad count
                                m_smVisionInfo.g_arrPad[i].LoadPitchGapLinkTemporary();   // 2021 06 08 - CCENG : Not allow to change pitch gap link. Should base on previous pad setting.
                                m_smVisionInfo.g_arrPad[i].UpdateNewGroupNoListToTemplate();
                                //AutoDefinePitchGap_NoReset(i);
                            }

                            //m_smVisionInfo.g_arrPad[i].AutoDefineGroupNoToTemplate();
                        }
                    }

                    m_blnKeepPrevPitchGap = false;
                    ReadPadPitchGapToGrid(0, dgd_PitchGapSetting);
                    //Load template data
                    m_dgdViewPG = dgd_PitchGapSetting;
                    m_strPosition = "Top";
                    ReadPadPitchGapToGrid(1, dgd_TopPGSetting);
                    m_strPosition = "Right";
                    ReadPadPitchGapToGrid(2, dgd_RightPGSetting);
                    m_strPosition = "Bottom";
                    ReadPadPitchGapToGrid(3, dgd_BottomPGSetting);
                    m_strPosition = "Left";
                    ReadPadPitchGapToGrid(4, dgd_LeftPGSetting);

                    switch (tabCtrl_PadPG.SelectedIndex)
                    {
                        case 0:
                            m_strPosition = "Middle";
                            break;
                        case 1:
                            m_strPosition = "Top";
                            break;
                        case 2:
                            m_strPosition = "Right";
                            break;
                        case 3:
                            m_strPosition = "Bottom";
                            break;
                        case 4:
                            m_strPosition = "Left";
                            break;
                    }

                    lbl_TitleStep6.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_IdentifyPitchGap;
                    btn_Previous.Enabled = true;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    if (m_intLearnType == 7)
                    {
                        tp_IdentifyPitchGap.Controls.Add(btn_SaveSubLearn);
                        btn_SaveSubLearn.BringToFront();
                        btn_Next.Enabled = false;
                        btn_Previous.Enabled = false;
                    }
                    break;
                case 8: // Learn Package ROI
                    AddTrainPackageROI(m_smVisionInfo.g_arrPadROIs);

                    if (m_blnJumpToPkgPage)
                    {
                        // Rotate unit to zero degree
                        for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                        {
                            ImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrImages[i], Convert.ToSingle(lbl_DieAngle.Text),
                                            ref m_smVisionInfo.g_arrRotatedImages, i);
                        }

                        m_smVisionInfo.g_blnViewRotatedImage = true;
                        ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[0]);
                    }

                    int intEdgeDistanceTop = (int)Math.Round(Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX, 0, MidpointRounding.AwayFromZero);
                    int intEdgeDistanceRight = (int)Math.Round(Convert.ToSingle(txt_StartPixelFromRight.Text) * m_smVisionInfo.g_fCalibPixelX, 0, MidpointRounding.AwayFromZero);
                    int intEdgeDistanceBottom = (int)Math.Round(Convert.ToSingle(txt_StartPixelFromBottom.Text) * m_smVisionInfo.g_fCalibPixelX, 0, MidpointRounding.AwayFromZero);
                    int intEdgeDistanceLeft = (int)Math.Round(Convert.ToSingle(txt_StartPixelFromLeft.Text) * m_smVisionInfo.g_fCalibPixelX, 0, MidpointRounding.AwayFromZero);
                    if ((intEdgeDistanceTop >= (int)(m_smVisionInfo.g_arrPadROIs[0][1].ref_ROIHeight / 2)))
                    {
                        txt_StartPixelFromEdge.Text = "0";
                        intEdgeDistanceTop = 0;
                    }
                    if ((intEdgeDistanceRight >= (int)(m_smVisionInfo.g_arrPadROIs[0][1].ref_ROIWidth / 2)))
                    {
                        txt_StartPixelFromRight.Text = "0";
                        intEdgeDistanceRight = 0;
                    }
                    if ((intEdgeDistanceBottom >= (int)(m_smVisionInfo.g_arrPadROIs[0][1].ref_ROIHeight / 2)))
                    {
                        txt_StartPixelFromBottom.Text = "0";
                        intEdgeDistanceBottom = 0;
                    }
                    if ((intEdgeDistanceLeft >= (int)(m_smVisionInfo.g_arrPadROIs[0][1].ref_ROIWidth / 2)))
                    {
                        txt_StartPixelFromLeft.Text = "0";
                        intEdgeDistanceLeft = 0;
                    }
                    m_smVisionInfo.g_arrPadROIs[0][2].LoadROISetting(intEdgeDistanceLeft, intEdgeDistanceTop,
                                                    m_smVisionInfo.g_arrPadROIs[0][1].ref_ROIWidth - intEdgeDistanceRight - intEdgeDistanceLeft,
                                                    m_smVisionInfo.g_arrPadROIs[0][1].ref_ROIHeight - intEdgeDistanceBottom - intEdgeDistanceTop);

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewPackageImage = false;


                    lbl_TitleStepPkg1.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_StepPkg1;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 9:// Learn Image 1 Threshold
                    m_smVisionInfo.g_intSelectedImage = 0;
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[0]);

                    lbl_TitleStepPkg2.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_StepPkg2;
                    if (m_smVisionInfo.g_arrImages.Count == 1)
                    {
                        btn_Next.Enabled = false;
                        tp_StepPkg2.Controls.Add(btn_Save);
                    }
                    else
                        btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 10:// Learn Image 2 Threshold
                    m_smVisionInfo.g_intSelectedImage = 1;
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[1]);

                    lbl_TitleStepPkg3.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_StepPkg3;
                    if (m_smVisionInfo.g_arrImages.Count == 2)
                    {
                        btn_Next.Enabled = false;
                        tp_StepPkg3.Controls.Add(btn_Save);
                    }
                    else
                        btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 11:// Learn Image 3 Threshold
                    m_smVisionInfo.g_intSelectedImage = 2;
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[2]);

                    lbl_TitleStepPkg4.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_StepPkg4;
                    btn_Next.Enabled = false;
                    tp_StepPkg2.Controls.Add(btn_Save);
                    btn_Previous.Enabled = true;
                    break;
                case 12:
                    if (IsPitchGapSettingError())
                    {
                        m_smVisionInfo.g_intLearnStepNo = 7;
                        m_intDisplayStepNo--;
                        break;
                    }

                    // Define Group BlobsFeatures Setting
                    DefineGroupPadTolerance();

                    srmLabel5.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_Save;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    break;
                case 13: // Pad ROI
                    m_smVisionInfo.g_intSelectedImage = 0;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_intTopPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop;
                        m_intRightPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight;
                        m_intBottomPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom;
                        m_intLeftPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft;
                    }

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                    chk_SetToAllEdge.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_Pad", false));
                    if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
                        chk_SetToAllSideROI.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllSide_Pad", false));
                    else
                        chk_SetToAllSideROI.Checked = false;
                    if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
                        chk_SetToAllSideROI.Visible = m_blnPadROIButtonVisible;
                    else
                        chk_SetToAllSideROI.Visible = false;
                    UpdatePadROIGUI();
                    //MeasureGauge();
                    m_pTop = new Point(pnl_Top.Location.X, pnl_Top.Location.Y);
                    m_pRight = new Point(pnl_Right.Location.X, pnl_Right.Location.Y);
                    m_pBottom = new Point(pnl_Bottom.Location.X, pnl_Bottom.Location.Y);
                    m_pLeft = new Point(pnl_Left.Location.X, pnl_Left.Location.Y);
                    bool blnIsGaugeOK = true;
                    if (chk_UseGauge.Checked)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                            {
                                if (m_smVisionInfo.g_arrPad[0].MeasureEdge_UsingSidePkgCornerPoint(
                                     m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X,
                                     m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y,
                                     m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X,
                                     m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y,
                                     m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X,
                                     m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y,
                                     m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X,
                                     m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y))
                                {
                                    continue;
                                }
                                else
                                {
                                    SRMMessageBox.Show("Cannot form Center ROI. Please adjust Side ROI or gauge setting.");

                                    blnIsGaugeOK = false;
                                    break;
                                }
                            }

                            if (!m_smVisionInfo.g_arrPad[i].IsGaugeMeasureOK(i))
                            {
                                string strDirectionName;
                                switch (i)
                                {
                                    case 0:
                                    default:
                                        strDirectionName = "Center";
                                        break;
                                    case 1:
                                        strDirectionName = "Top";
                                        break;
                                    case 2:
                                        strDirectionName = "Right";
                                        break;
                                    case 3:
                                        strDirectionName = "Bottom";
                                        break;
                                    case 4:
                                        strDirectionName = "Left";
                                        break;

                                }
                                SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");
                                blnIsGaugeOK = false;
                                break;

                            }
                        }

                        if (!blnIsGaugeOK)
                        {
                            break;
                        }
                    }

                    if (blnIsGaugeOK)
                    {
                        RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                        // Add Package ROI
                        AddTrainPackageROI(m_smVisionInfo.g_arrPadROIs);
                        AddTrainPadROI(m_smVisionInfo.g_arrPadROIs);
                    }
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;
                        m_smVisionInfo.g_arrPadROIs[i][3].AttachImage(m_smVisionInfo.g_objPackageImage);
                    }

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_TitleStep4.BringToFront();
                    lbl_TitleStep4.Text = "Pad ROI";
                    tabCtrl_Pad.SelectedTab = tp_PadROITolerance;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = false;
                    break;
                case 14: // Unit Pattern
                    AddUnitROI(m_smVisionInfo.g_arrPadOrientROIs);

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_UnitPattern.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_UnitPattern;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 15: // Orient
                    AddOrientROI(m_smVisionInfo.g_arrPadOrientROIs);

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_Orient.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_Orient;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    break;
                case 16: // Color
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;

                    m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    AddColorROI(m_smVisionInfo.g_arrPadColorROIs);

                    RotateColorImage();
                    m_smVisionInfo.g_intSelectedColorThresholdIndex = -1;
                    m_smVisionInfo.g_intSelectedDontCareROIIndex = -1;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    //m_smVisionInfo.g_blnUpdateSelectedROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_Color.BringToFront();
                    tabCtrl_Pad.SelectedTab = tp_Color;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private bool SetSettingBasedONPreviousRecord()
        {
            bool blnMatch;
            string[] strROIName;
            int intCount = 0;
            strROIName = new string[m_smVisionInfo.g_arrPadROIs.Count];

            //Define ROI name for display in message box
            for (int y = 0; y < m_smVisionInfo.g_arrPadROIs.Count; y++)
            {
                if (y > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                switch (y)
                {
                    case 0:
                        strROIName[y] = m_smVisionInfo.g_arrPadROIs[y][1].ref_strROIName;
                        break;
                    case 1:
                        strROIName[y] = "Up Pads";
                        break;
                    case 2:
                        strROIName[y] = "Right Pads";
                        break;
                    case 3:
                        strROIName[y] = "Down Pads";
                        break;
                    case 4:
                        strROIName[y] = "Left Pads";
                        break;
                }
            }


            //Use prev setting
            if (m_smVisionInfo.g_blnUsedPreTemplate)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        continue;

                    //Check no of objects is it tally with prev setting
                    if (!m_smVisionInfo.g_arrPad[i].CheckPadTally(ref intCount))
                    {
                        if (SRMMessageBox.Show("Selected pads is not tally with previous pads record in " + strROIName[i] + ". There are " + intCount + " pads in previous record." +
                            " Do you want to continue with default tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            chk_UsePreviousTolerance.Checked = false;
                            m_smVisionInfo.g_blnUsedPreTemplate = false;
                            break;
                        }
                        else
                        {
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            return false;
                        }
                    }
                    else
                    {
                        blnMatch = m_smVisionInfo.g_arrPad[i].MatchPrevSettings();

                        //Check Blobs position is it match with prev setting
                        if (!blnMatch)
                        {
                            if (SRMMessageBox.Show("Unable to match current pad settings with previous pad record in " + strROIName[i] + "! " +
                              "Do you want to continue with default tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                chk_UsePreviousTolerance.Checked = false;
                                m_smVisionInfo.g_blnUsedPreTemplate = false;
                                break;
                            }
                            else
                            {
                                m_smVisionInfo.g_intLearnStepNo--;
                                m_intDisplayStepNo--;
                                return false;
                            }
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPad[i].RearrangeBlobs();
                            m_smVisionInfo.g_arrPad[i].DefineTolerance(false);
                            m_smVisionInfo.g_arrPad[i].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPadROIs[i][0]);   // Index 0: Search ROI, 1:Gauge ROI, 2: Package ROI
                        }
                    }
                }
            }

            return true;
        }

        private bool SetSettingBasedONPreviousRecord2()
        {
            bool blnMatch;
            string[] strROIName;
            int intCount = 0;
            strROIName = new string[m_smVisionInfo.g_arrPadROIs.Count];

            //Define ROI name for display in message box
            for (int y = 0; y < m_smVisionInfo.g_arrPadROIs.Count; y++)
            {
                if (y > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                switch (y)
                {
                    case 0:
                        strROIName[y] = m_smVisionInfo.g_arrPadROIs[y][1].ref_strROIName;
                        break;
                    case 1:
                        strROIName[y] = "Up Pads";
                        break;
                    case 2:
                        strROIName[y] = "Right Pads";
                        break;
                    case 3:
                        strROIName[y] = "Down Pads";
                        break;
                    case 4:
                        strROIName[y] = "Left Pads";
                        break;
                }
            }


            //Use prev setting
            m_smVisionInfo.g_blnUsedPreTemplate = true;
            if (m_smVisionInfo.g_blnUsedPreTemplate)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        continue;

                    //Check no of objects is it tally with prev setting
                    //if (!m_smVisionInfo.g_arrPad[i].CheckPadTally(ref intCount))
                    //{
                    //    if (SRMMessageBox.Show("Selected pads is not tally with previous pads record in " + strROIName[i] + ". There are " + intCount + " pads in previous record." +
                    //        " Do you want to continue with default tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    //    {
                    //        chk_UsePreviousTolerance.Checked = false;
                    //        m_smVisionInfo.g_blnUsedPreTemplate = false;
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        m_smVisionInfo.g_intLearnStepNo--;
                    //        m_intDisplayStepNo--;
                    //        return false;
                    //    }
                    //}
                    //else
                    {
                        blnMatch = m_smVisionInfo.g_arrPad[i].MatchPrevSettings2();

                        //Check Blobs position is it match with prev setting
                        //if (!blnMatch)
                        //{
                        //    //if (SRMMessageBox.Show("Unable to match current pad settings with previous pad record in " + strROIName[i] + "! " +
                        //    //  "Do you want to continue with default tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        //    //{
                        //    //    chk_UsePreviousTolerance.Checked = false;
                        //    //    m_smVisionInfo.g_blnUsedPreTemplate = false;
                        //    //    break;
                        //    //}
                        //    //else
                        //    //{
                        //    //    m_smVisionInfo.g_intLearnStepNo--;
                        //    //    m_intDisplayStepNo--;
                        //    //    return false;
                        //    //}
                        //}
                        //else
                        {
                            m_smVisionInfo.g_arrPad[i].RearrangeBlobs();
                            m_smVisionInfo.g_arrPad[i].DefineTolerance(false);
                            m_smVisionInfo.g_arrPad[i].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPadROIs[i][0]);   // Index 0: Search ROI, 1:Gauge ROI, 2: Package ROI
                        }

                        // chk_UsePreviousTolerance.Checked = false;
                        //m_smVisionInfo.g_blnUsedPreTemplate = false;
                    }
                }
            }

            return true;
        }

        private void SaveDontCareSetting(string strFolderPath)
        {
            ImageDrawing objImage = new ImageDrawing();
            ImageDrawing objFinalImage = new ImageDrawing();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_objBlackImage.CopyTo(objFinalImage);

                //Check is the ROI for drawing dont care changed
                //m_smVisionInfo.g_arrPolygon_Pad[i][0].CheckDontCarePosition(m_smVisionInfo.g_arrPadROIs[i][2], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                {
                    //Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    if (m_smVisionInfo.g_arrPolygon_Pad[i][j].ref_intFormMode != 2)
                        Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    else
                        Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                    ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);
                }

                objFinalImage.SaveImage(strFolderPath + "DontCareImage" + i.ToString() + "_0.bmp");
            }
            objImage.Dispose();
            objFinalImage.Dispose();
            Polygon.SavePolygon(strFolderPath + "Polygon.xml", m_smVisionInfo.g_arrPolygon_Pad);
        }

        private void btn_AddPitchGap_Click(object sender, EventArgs e)
        {
            int intTotalPitchGap = m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].GetTotalPitchGap();
            SetPitchGap(intTotalPitchGap, 0, 0, tabCtrl_PadPG.SelectedIndex);
            ReadPadPitchGapToGrid(tabCtrl_PadPG.SelectedIndex, m_dgdViewPG);
        }

        private void btn_BuildObjects_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            LoadControlSettings(strFolderPath + "Pad\\Template\\");
            LoadROISetting(strFolderPath + "Pad\\ROI.xml", m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrPad.Length);
            LoadROISetting(strFolderPath + "Orient\\ROI.xml", m_smVisionInfo.g_arrPadOrientROIs);
            LoadROISetting(strFolderPath + "Pad\\DontCareROI.xml", m_smVisionInfo.g_arrPadDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            LoadROISetting(strFolderPath + "Pad\\ColorDontCareROI.xml", m_smVisionInfo.g_arrPadColorDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                        {
                            m_smVisionInfo.g_arrPadDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                        }
                    }

                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                        {
                            for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                            {
                                m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                            }
                        }
                    }

                }
            }

            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                LoadPadOffSetSetting(strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                LoadPadOffSetSetting(strFolderPath + "Calibration.xml");
            LoadPadSetting(strFolderPath);
            LoadPositioningSetting(strFolderPath + "Positioning\\");

            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (m_smVisionInfo.g_arrPin1 != null)
                {
                    m_smVisionInfo.g_arrPin1[0].LoadTemplate(strFolderPath + "Pad\\Template\\");
                }
            }

            if (m_smVisionInfo.g_objPadOrient != null)
            {
                m_smVisionInfo.g_objPadOrient.LoadOrient(strFolderPath + "Orient\\Settings.xml", "General");
                m_smVisionInfo.g_objPadOrient.SetCalibrationData(
                                   m_smVisionInfo.g_fCalibPixelX,
                                   m_smVisionInfo.g_fCalibPixelY, m_smCustomizeInfo.g_intUnitDisplay);
            }
            LoadMatcherFile(strFolderPath);
            Polygon.LoadPolygon(strFolderPath + "Pad\\Template\\Polygon.xml", m_smVisionInfo.g_arrPolygon_Pad);
            Polygon.LoadPolygon(strFolderPath + "Pad\\Template\\ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PadColor, m_smVisionInfo.g_arrPad.Length);
            m_smVisionInfo.AT_PR_AttachImagetoROI = true;
            this.Close();
            this.Dispose();
        }

        private void btn_DeletePitchGap_Click(object sender, EventArgs e)
        {
            if (m_intPitchGapSelectedRowIndex >= 0 && m_dgdViewPG.Rows.Count > 0)
            {
                if (SRMMessageBox.Show("Are you sure you want to delete pitch/gap number " + (m_intPitchGapSelectedRowIndex + 1), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].DeletePitchGap(m_intPitchGapSelectedRowIndex);
                    ReadPadPitchGapToGrid(tabCtrl_PadPG.SelectedIndex, m_dgdViewPG);
                }
            }
            else
            {
                SRMMessageBox.Show("Pitch/Gap row is empty already.");
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (m_intVisionType == 1) // Learn Orient
            {
                if (m_smVisionInfo.g_intLearnStepNo == 0)   // From Step 1 Search ROI To Unit pattern ROI
                    m_smVisionInfo.g_intLearnStepNo = 14;   
                else if (m_smVisionInfo.g_intLearnStepNo == 14)   // From Unit Pattern ROI To Orient ROI
                    m_smVisionInfo.g_intLearnStepNo = 15;   
            }
            else
            {

                if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_intLearnStepNo == 7)   // Pitch Gap Setting Page
                    {
                        m_smVisionInfo.g_intLearnStepNo = 12;    // Go to finish
                    }
                    else if (m_smVisionInfo.g_intLearnStepNo < 13)
                    {
                        m_smVisionInfo.g_intLearnStepNo++;

                        if (m_smVisionInfo.g_intLearnStepNo == 1)
                            m_smVisionInfo.g_intLearnStepNo++;

                        if (m_smVisionInfo.g_intLearnStepNo == 3 && !m_smVisionInfo.g_blnWantDontCareArea_Pad)
                            m_smVisionInfo.g_intLearnStepNo++;

                        if (m_smVisionInfo.g_intLearnStepNo == 4 && !m_smVisionInfo.g_blnWantPin1)
                            m_smVisionInfo.g_intLearnStepNo++;
                    }
                }
                else
                {
                    if (m_smVisionInfo.g_intLearnStepNo < 99)
                        m_smVisionInfo.g_intLearnStepNo++;

                    if ((m_smCustomizeInfo.g_intWantPositioning & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 1)
                            m_smVisionInfo.g_intLearnStepNo++;
                    }

                    if (m_smVisionInfo.g_intLearnStepNo == 3 && !m_smVisionInfo.g_blnWantDontCareArea_Pad)
                        m_smVisionInfo.g_intLearnStepNo++;

                    if (m_smVisionInfo.g_intLearnStepNo == 4 && !m_smVisionInfo.g_blnWantPin1)
                        m_smVisionInfo.g_intLearnStepNo++;

                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 8)
                            m_smVisionInfo.g_intLearnStepNo = 12;
                    }

                    if (m_intVisionType == 1)
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 2)
                            m_smVisionInfo.g_intLearnStepNo = 14;
                    }
                    else
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 2 && m_blnOrientFail)
                            m_smVisionInfo.g_intLearnStepNo = 14;
                    }
                }
            }

            m_intDisplayStepNo++;

            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            if (m_intVisionType == 1)   // Learn Orient
            {
                if (m_smVisionInfo.g_intLearnStepNo == 15)   // From Orient ROI To Unit Pattern ROI
                    m_smVisionInfo.g_intLearnStepNo = 14;   
                else if (m_smVisionInfo.g_intLearnStepNo == 14) // From Unit Pattern ROI to Step 1 Search ROI 
                    m_smVisionInfo.g_intLearnStepNo = 0;
            }
            else
            {
                if (m_smVisionInfo.g_intLearnStepNo == 6)
                    m_blnKeepPrevObject = true;

                if (m_smVisionInfo.g_intLearnStepNo == 7)
                    m_blnIdentityPadsDone = true;

                if (m_smVisionInfo.g_intLearnStepNo >= 8)
                    m_blnKeepPrevPitchGap = true;

                if (m_smVisionInfo.g_intLearnStepNo == 10 && m_blnJumpToPkgPage)
                {
                    m_smVisionInfo.g_intLearnStepNo = 2;
                    m_blnJumpToPkgPage = false;
                }
                else if ((m_smVisionInfo.g_intLearnStepNo == 10) && (m_smVisionInfo.g_arrPad.Length == 1))
                {
                    m_smVisionInfo.g_intLearnStepNo = 7;
                    m_blnKeepPrevPitchGap = true;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 12)
                {
                    //if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    m_smVisionInfo.g_intLearnStepNo = 7;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 13)
                {
                    m_smVisionInfo.g_intLearnStepNo = 0;
                }
                else if (m_smVisionInfo.g_intLearnStepNo > 0)
                {
                    m_smVisionInfo.g_intLearnStepNo--;

                    if (m_smVisionInfo.g_intLearnStepNo == 4 && !m_smVisionInfo.g_blnWantPin1)
                        m_smVisionInfo.g_intLearnStepNo--;

                    if (m_smVisionInfo.g_intLearnStepNo == 3 && !m_smVisionInfo.g_blnWantDontCareArea_Pad)
                        m_smVisionInfo.g_intLearnStepNo--;

                    if (m_smVisionInfo.g_intLearnStepNo == 13)
                        m_smVisionInfo.g_intLearnStepNo = 0;

                    if (m_smVisionInfo.g_intLearnStepNo == 1)
                        m_smVisionInfo.g_intLearnStepNo--;

                }
            }

            m_intDisplayStepNo--;

            SetupSteps(false);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void StartSplash()
        {
            m_objSaveForm = new SavingInProgressForm(m_smVisionInfo, m_smCustomizeInfo);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            m_objSaveForm.Location = new Point(resolution.Width / 2 - m_objSaveForm.Size.Width / 2, resolution.Height / 2 - m_objSaveForm.Size.Height / 2);
            m_objSaveForm.StartSaveForm();
        }
        private void CloseSplash()
        {
            if (m_objSaveForm == null)
                return;

            m_objSaveForm.CloseSaveForm();
            m_objSaveForm.Dispose();
            m_objSaveForm = null;
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            StartSplash();
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\";

            //m_smVisionInfo.g_intSelectedImage = 0;
            m_smVisionInfo.g_intSavingState = 1;
            ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[0]);

            m_smVisionInfo.g_intSavingState = 2;
            DefinePadContour();

            m_smVisionInfo.g_intSavingState = 3;
            SetDefaultSetting();

            m_smVisionInfo.g_intSavingState = 4;
            SavePadSetting(strPath);

            m_smVisionInfo.g_intSavingState = 5;
            SavePositioningSetting(strPath + "Positioning\\");

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            m_smVisionInfo.g_intSavingState = 6;
            LoadROISetting(strFolderPath + "Pad\\ROI.xml", m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrPad.Length);
            LoadROISetting(strFolderPath + "Orient\\ROI.xml", m_smVisionInfo.g_arrPadOrientROIs);
            LoadROISetting(strFolderPath + "Pad\\DontCareROI.xml", m_smVisionInfo.g_arrPadDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                        {
                            m_smVisionInfo.g_arrPadDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                        }
                    }

                }
            }

            m_smVisionInfo.g_intSavingState = 7;
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                LoadPadOffSetSetting(strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                LoadPadOffSetSetting(strFolderPath + "Calibration.xml");

            m_smVisionInfo.g_intSavingState = 8;
            LoadPadSetting(strFolderPath);

            m_smVisionInfo.g_intSavingState = 9;
            LoadPositioningSetting(strFolderPath + "Positioning\\");

            m_smVisionInfo.g_intSavingState = 10;
            LoadMatcherFile(strFolderPath);

            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            CloseSplash();
            m_smVisionInfo.g_intSavingState = 0;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            //If want CPK, reset CPK counter when new lot
            if (m_smVisionInfo.g_blnCPKON)
            {
                //Re-init CPK
                m_smVisionInfo.g_blnReInitCPK = false;
            }

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            int intSelectedROI = -1;
            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            {
                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                    continue;

                if (intSelectedROI == -1)
                    intSelectedROI = j;

                if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
                {
                    intSelectedROI = j;
                    break;
                }
            }

            if (intSelectedROI == -1)
                return;

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intThresholdValue;
            m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_fPadImageGain;

            if (m_smVisionInfo.g_blnViewPackageImage)
            {
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            }
            else
            {
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            }

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrPad.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 1)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 2)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 3)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 4)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intThresholdValue);

            bool blnUseDoubleThreshold = false;
            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            ThresholdWithGainForm objThresholdForm = new ThresholdWithGainForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrPad[i].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrPad[i].ref_fPadImageGain = m_smVisionInfo.g_fThresholdGainValue;

                        ////Rebuild pad objects
                        //if (blnUseDoubleThreshold && i > 0)
                        //    m_smVisionInfo.g_arrPad[i].BuildDoubleThresholdPadObjects(m_smVisionInfo.g_arrPadROIs[i][3], i);
                        //else
                        //    m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[i][3]);// 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package

                        ////Clear Temp Blobs Features
                        //m_smVisionInfo.g_arrPad[i].ClearTempBlobsFeatures();

                        ////Set blobs features data into Temp Blobs Features
                        //m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrPadROIs[i][3],
                        //     0,// m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                        //    0);// m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset

                        ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                        //m_smVisionInfo.g_arrPad[i].CompareSelectedBlobs();
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_fPadImageGain = m_smVisionInfo.g_fThresholdGainValue;

                    //Rebuild pad objects
                    //if (blnUseDoubleThreshold && intSelectedROI > 0)
                    //    m_smVisionInfo.g_arrPad[intSelectedROI].BuildDoubleThresholdPadObjects(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3], intSelectedROI);
                    //else
                    //    m_smVisionInfo.g_arrPad[intSelectedROI].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3]);// 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package

                    ////Clear Temp Blobs Features
                    //m_smVisionInfo.g_arrPad[intSelectedROI].ClearTempBlobsFeatures();

                    ////Set blobs features data into Temp Blobs Features
                    //m_smVisionInfo.g_arrPad[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3],
                    //      0,//m_smVisionInfo.g_arrPadROIs[intSelectedROI][2].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[intSelectedROI][3].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                    //        0);// m_smVisionInfo.g_arrPadROIs[intSelectedROI][2].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[intSelectedROI][3].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset

                    ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                    //m_smVisionInfo.g_arrPad[intSelectedROI].CompareSelectedBlobs();
                }
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intThresholdValue;
                m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_fPadImageGain;

                ////Rebuild pad objects
                //if (blnUseDoubleThreshold && intSelectedROI > 0)
                //    m_smVisionInfo.g_arrPad[intSelectedROI].BuildDoubleThresholdPadObjects(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3], intSelectedROI);
                //else
                //    m_smVisionInfo.g_arrPad[intSelectedROI].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3]);// 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package

                ////Clear Temp Blobs Features
                //m_smVisionInfo.g_arrPad[intSelectedROI].ClearTempBlobsFeatures();

                ////Set blobs features data into Temp Blobs Features
                //m_smVisionInfo.g_arrPad[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3],
                //      0,//m_smVisionInfo.g_arrPadROIs[intSelectedROI][2].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[intSelectedROI][3].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                //       0);// m_smVisionInfo.g_arrPadROIs[intSelectedROI][2].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[intSelectedROI][3].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset

                ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                //m_smVisionInfo.g_arrPad[intSelectedROI].CompareSelectedBlobs();
            }

            BuildObjectsAndSetToTemplateArray();

            objThresholdForm.Dispose();



            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_UndoObjects_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            {
                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                    continue;

                if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
                {
                    intSelectedROI = j;
                    break;
                }
            }

            m_smVisionInfo.g_arrPad[intSelectedROI].UndoSelectedObject();

            m_smVisionInfo.g_blnViewObjectsBuilded = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_UndoROI_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            {
                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                    continue;

                if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
                {
                    intSelectedROI = j;
                    break;
                }
            }

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count > 4)
            {
                m_smVisionInfo.g_arrPadROIs[intSelectedROI].RemoveAt(m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count - 1);
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void chk_UsePreviousTolerance_CheckedChanged(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_blnUsedPreTemplate = chk_UsePreviousTolerance.Checked;
        }

        private void dgd_PitchGapSetting_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_intPitchGapSelectedRowIndex = e.RowIndex;
        }

        private void dgd_PitchGapSetting_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 1) // Edit PitchGap "To Pad" no
            {
                int intTotalPadNo = m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].GetBlobsFeaturesNumber();
                int intFormPadNo = Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value);
                string strToPadNo = ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value.ToString();
                SetPitchGapForm2 objSetPitchGapForm = new SetPitchGapForm2(intTotalPadNo, intFormPadNo, strToPadNo);
                if (objSetPitchGapForm.ShowDialog() == DialogResult.OK)
                {
                    bool blnAllowChange = true;
                    int intToPadNo = 0;
                    if (objSetPitchGapForm.ref_strToPadNo != "NA")
                    {
                        intToPadNo = Convert.ToInt32(objSetPitchGapForm.ref_strToPadNo);

                        if (intFormPadNo == intToPadNo)
                        {
                            SRMMessageBox.Show("Can not select same pad number!");
                            blnAllowChange = false;
                        }
                        else if (m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].CheckPitchGapLinkExist(intFormPadNo - 1, intToPadNo - 1))
                        {
                            SRMMessageBox.Show("This Pitch/Gap link already exist.");
                            blnAllowChange = false;
                        }
                        else if (m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].CheckPitchGapLinkInPadAlready(intFormPadNo - 1))
                        {
                            //SRMMessageBox.Show("Pitch/Gap already defined in pad number " + intFormPadNo);
                            //blnAllowChange = false;
                        }
                        else if (!m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].CheckPitchGapLinkAvailable_LinkDifferentGroup(intFormPadNo - 1, intToPadNo - 1))
                        {
                            SRMMessageBox.Show("Pitch/Gap can not be created in between pad no." + intFormPadNo + " and pad no." + intToPadNo + ".");
                            blnAllowChange = false;
                        }
                    }
                    if (blnAllowChange)
                    {
                        m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].SetPitchGap(intFormPadNo - 1, intToPadNo - 1);
                        ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value = objSetPitchGapForm.ref_strToPadNo;
                        ReadPadPitchGapToGrid(tabCtrl_PadPG.SelectedIndex, m_dgdViewPG);
                    }
                }
            }
            else // Change Group No 
            {
                btn_Next.Enabled = false;
                btn_Cancel.Enabled = false;
                tabCtrl_Pad.Enabled = false;

                if (m_objSetPadGroupNoForm == null)
                {
                    m_objSetPadGroupNoForm = new SetPadGroupNoForm();
                }
                m_objSetPadGroupNoForm.TopMost = true;
                m_objSetPadGroupNoForm.Show();

                int intGroupNo = Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[6].Value);
                m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].SetGroupPadToArrayForDrawing(intGroupNo - 1);
                m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].ref_blnViewGroupClassificationDrawing = true;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Setting_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            string m_strUnitLabel;
            int m_intDecimalPlaces;
            int intDecimalPlaces = 4;
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 8)
            {
                switch (m_smCustomizeInfo.g_intUnitDisplay)
                {
                    default:
                    case 1:
                        m_strUnitLabel = "mm^2";
                        m_intDecimalPlaces = 6;
                        break;
                    case 2:
                        m_strUnitLabel = "mil^2";
                        m_intDecimalPlaces = 6;
                        break;
                    case 3:
                        m_strUnitLabel = "um^2";
                        m_intDecimalPlaces = 2;
                        break;
                }
            }
            else
            {
                switch (m_smCustomizeInfo.g_intUnitDisplay)
                {
                    default:
                    case 1:
                        m_strUnitLabel = "mm";
                        m_intDecimalPlaces = 4;
                        break;
                    case 2:
                        m_strUnitLabel = "mil";
                        m_intDecimalPlaces = 4;
                        break;
                    case 3:
                        m_strUnitLabel = "um";
                        m_intDecimalPlaces = 2;
                        break;
                }
            }

            //Define pad index
            int m_intPadIndex = tabCtrl_PadSide.SelectedIndex;

            string strCurrentSetValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            //SetValueForm objSetValueForm = new SetValueForm(e.RowIndex, intDecimalPlaces, strCurrentSetValue);
            SetValueForm objSetValueForm = new SetValueForm("Set value to pad" + (e.RowIndex + 1).ToString(), m_strUnitLabel, e.RowIndex, m_intDecimalPlaces, strCurrentSetValue);
            objSetValueForm.TopMost = true;
            if (objSetValueForm.ShowDialog() == DialogResult.OK)
            {
                int intStartRowNumber;
                if (objSetValueForm.ref_blnSetAllRows)
                {
                    intStartRowNumber = 0;
                }
                else
                {
                    intStartRowNumber = e.RowIndex;
                }

                //Validate min, max value
                for (int i = intStartRowNumber; i < ((DataGridView)sender).Rows.Count; i++)
                {
                    if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
                    {
                        // Check insert data valid or not
                        if (e.ColumnIndex == 2 || e.ColumnIndex == 4 || e.ColumnIndex == 6)
                        {
                            float fMax = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Value.ToString());

                            if (objSetValueForm.ref_fSetValue > fMax)
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Style.SelectionBackColor = Color.Pink;
                            }
                            else
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Style.SelectionBackColor = Color.White;
                            }
                        }
                        else if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 7)
                        {
                            float fMin = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Value.ToString());

                            if (fMin > objSetValueForm.ref_fSetValue)
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Style.SelectionBackColor = Color.Pink;
                            }
                            else
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Style.SelectionBackColor = Color.White;
                            }
                        }

                        // Set new insert value into table
                        ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F" + m_intDecimalPlaces);
                    }

                    int intLengthMode = m_smVisionInfo.g_arrPad[m_intPadIndex].GetSampleLengthMode(i);

                    //Update template setting
                    if (intLengthMode == 1)
                    {
                        m_smVisionInfo.g_arrPad[m_intPadIndex].UpdateBlobFeatureToPixel_NoPitchGap(i,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[1].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[2].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[4].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[6].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[8].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString())
                            );
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPad[m_intPadIndex].UpdateBlobFeatureToPixel_NoPitchGap(i,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[1].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[2].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[6].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[4].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[8].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString())
                            );
                    }

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }

            this.TopMost = true;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_CutObj_CheckedChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnCutMode = radioBtn_CutObj.Checked;
        }

        private void tabCtrl_PadSide_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabCtrlPadSideTabPageChange();
        }

        private void TabCtrlPadSideTabPageChange()
        {
            //Focus ROI when tab change
            int intIndex = tabCtrl_PadSide.SelectedIndex;

            switch (intIndex)
            {
                case 0:
                    m_strPosition = "Middle";
                    m_dgdView = dgd_Setting;
                    break;

                case 1:
                    m_strPosition = "Top";
                    m_dgdView = dgd_TopSetting;
                    break;

                case 2:
                    m_strPosition = "Right";
                    m_dgdView = dgd_RightSetting;
                    break;

                case 3:
                    m_strPosition = "Bottom";
                    m_dgdView = dgd_BottomSetting;
                    break;

                case 4:
                    m_strPosition = "Left";
                    m_dgdView = dgd_LeftSetting;
                    break;
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (intIndex == i)
                {
                    m_smVisionInfo.g_arrPadROIs[intIndex][0].VerifyROIArea(
                        m_smVisionInfo.g_arrPadROIs[intIndex][0].ref_ROITotalX,
                        m_smVisionInfo.g_arrPadROIs[intIndex][0].ref_ROITotalY);
                }
                else
                {
                    m_smVisionInfo.g_arrPadROIs[i][0].VerifyROIArea(0, 0);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }


        private void tabCtrl_PadPG_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get tabControl page selected index
            int intIndex = tabCtrl_PadPG.SelectedIndex;
            switch (intIndex)
            {
                case 0:
                    m_strPosition = "Middle";
                    m_dgdViewPG = dgd_PitchGapSetting;
                    break;

                case 1:
                    m_strPosition = "Top";
                    m_dgdViewPG = dgd_TopPGSetting;
                    break;

                case 2:
                    m_strPosition = "Right";
                    m_dgdViewPG = dgd_RightPGSetting;
                    break;

                case 3:
                    m_strPosition = "Bottom";
                    m_dgdViewPG = dgd_BottomPGSetting;
                    break;

                case 4:
                    m_strPosition = "Left";
                    m_dgdViewPG = dgd_LeftPGSetting;
                    break;
            }

            // Set ROI to handler True when tabctrl page is changed.
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (intIndex == i)
                {
                    m_smVisionInfo.g_arrPadROIs[intIndex][0].VerifyROIArea(
                        m_smVisionInfo.g_arrPadROIs[intIndex][0].ref_ROITotalX,
                        m_smVisionInfo.g_arrPadROIs[intIndex][0].ref_ROITotalY);
                }
                else
                {
                    m_smVisionInfo.g_arrPadROIs[i][0].VerifyROIArea(0, 0);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void timer_Pad_Tick(object sender, EventArgs e)
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

            if (m_smVisionInfo.g_blnViewRotatedImage_AfterMouseUp && ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
            {
                m_smVisionInfo.g_blnViewRotatedImage_AfterMouseUp = false;
                RotatePrecise();
            }

            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if ((m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                {
                    cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Pad[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
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

            if (m_objSetPadGroupNoForm != null)
            {
                if (!m_objSetPadGroupNoForm.ref_blnShow)
                {
                    btn_Next.Enabled = true;
                    btn_Cancel.Enabled = true;
                    tabCtrl_Pad.Enabled = true;

                    if (m_objSetPadGroupNoForm.DialogResult == DialogResult.OK)
                    {
                        m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].UpdateNewGroupNoListToTemplate();
                    }

                    switch (tabCtrl_PadPG.SelectedIndex)
                    {
                        case 0:
                            ReadPadPitchGapToGrid(0, dgd_PitchGapSetting);
                            break;
                        case 1:
                            ReadPadPitchGapToGrid(1, dgd_TopPGSetting);
                            break;
                        case 2:
                            ReadPadPitchGapToGrid(2, dgd_RightPGSetting);
                            break;
                        case 3:
                            ReadPadPitchGapToGrid(3, dgd_BottomPGSetting);
                            break;
                        case 4:
                            ReadPadPitchGapToGrid(4, dgd_LeftPGSetting);
                            break;
                    }

                    m_objSetPadGroupNoForm.Close();
                    m_objSetPadGroupNoForm.Dispose();
                    m_objSetPadGroupNoForm = null;

                    m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].ref_blnViewGroupClassificationDrawing = false;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
            }

            if (m_smVisionInfo.g_blnUpdatePadSetting)
            {
                m_smVisionInfo.g_blnUpdatePadSetting = false;

                if (m_smVisionInfo.g_intLearnStepNo == 6)    // Pad Setting Page
                    ReadPadTemplateDataToGrid(0, dgd_Setting);
                else if (m_smVisionInfo.g_intLearnStepNo == 7)   // Pitch Gap Setting Page
                    ReadPadTemplateDataToGrid(tabCtrl_PadSide.SelectedIndex + 1, m_dgdView);
            }

            string str = m_dgdView.Name;
            if (m_smVisionInfo.g_blnUpdateSelectedROI && m_smVisionInfo.g_intLearnStepNo != 16)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;

                if (m_smVisionInfo.g_intLearnStepNo == 2)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                    {
                        //if (m_smVisionInfo.g_arrPadROIs[i][0].GetROIHandle())
                        {
                            switch (m_smVisionInfo.g_intSelectedROI)
                            {
                                case 0:
                                    m_smVisionInfo.g_intSelectedROI = 0;
                                    radioBtn_Middle.Checked = true;
                                    break;
                                case 1:
                                    m_smVisionInfo.g_intSelectedROI = 1;
                                    radioBtn_Up.Checked = true;
                                    break;
                                case 2:
                                    m_smVisionInfo.g_intSelectedROI = 2;
                                    radioBtn_Right.Checked = true;
                                    break;
                                case 3:
                                    m_smVisionInfo.g_intSelectedROI = 3;
                                    radioBtn_Down.Checked = true;
                                    break;
                                case 4:
                                    m_smVisionInfo.g_intSelectedROI = 4;
                                    radioBtn_Left.Checked = true;
                                    break;
                            }

                        }
                    }

                    if (chk_UseGauge.Checked)
                    {
                        switch (m_smVisionInfo.g_intSelectedROI)
                        {
                            case 0:
                                picUnitROI7.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[13];
                                break;
                            case 1:
                                picUnitROI8.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[14];
                                break;
                            case 2:
                                picUnitROI9.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[15];
                                break;
                            case 3:
                                picUnitROI10.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[16];
                                break;
                            case 4:
                                picUnitROI11.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[17];
                                break;
                        }


                        int[] arrPackageSizeImageIndex = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetEdgeImageViewNo(), m_smVisionInfo.g_intVisionIndex);

                        for (int j = 0; j < 4; j++)
                        {
                            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objRectGauge4L.ref_arrEdgeROI[j].GetROIHandle())
                            {
                                m_smVisionInfo.g_intSelectedImage = arrPackageSizeImageIndex[j];
                                break;
                            }
                        }

                    }
                    else
                    {
                        switch (m_smVisionInfo.g_intSelectedROI)
                        {
                            case 0:
                                picUnitROI2.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[8];
                                break;
                            case 1:
                                picUnitROI3.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[9];
                                break;
                            case 2:
                                picUnitROI4.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[10];
                                break;
                            case 3:
                                picUnitROI5.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[11];
                                break;
                            case 4:
                                picUnitROI6.BringToFront();
                                //picUnitROI.Image = ils_ImageListTree.Images[12];
                                break;
                        }
                    }
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 6)
                {
                    int intSelectedROI = GetSelectedROI();
                    cbo_ReferenceCorner.SelectedIndex = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadLabelRefCorner;
                    cbo_PadLabelDirection.SelectedIndex = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadLabelDirection;
                }
                else if (m_smVisionInfo.g_intLearnStepNo == 13)
                {
                    int intSelectedROI = GetSelectedROI();
                    switch (intSelectedROI)
                    {
                        case 0:
                        case 1:
                            pnl_Top.Location = m_pTop;
                            pnl_Right.Location = m_pRight;
                            pnl_Bottom.Location = m_pBottom;
                            pnl_Left.Location = m_pLeft;
                            break;
                        case 2:
                            pnl_Top.Location = m_pRight;
                            pnl_Right.Location = m_pBottom;
                            pnl_Bottom.Location = m_pLeft;
                            pnl_Left.Location = m_pTop;
                            break;
                        case 3:
                            pnl_Top.Location = m_pBottom;
                            pnl_Right.Location = m_pLeft;
                            pnl_Bottom.Location = m_pTop;
                            pnl_Left.Location = m_pRight;
                            break;
                        case 4:
                            pnl_Top.Location = m_pLeft;
                            pnl_Right.Location = m_pTop;
                            pnl_Bottom.Location = m_pRight;
                            pnl_Left.Location = m_pBottom;
                            break;
                    }
                    
                    UpdatePadROIGUI();
                }
                else if (m_smVisionInfo.g_intLearnStepNo > 6 && m_smVisionInfo.g_intLearnStepNo < 14)
                {
                    if (tabCtrl_PadSide.SelectedIndex >= 0)
                    {
                        if (!m_smVisionInfo.g_arrPadROIs[tabCtrl_PadSide.SelectedIndex][0].GetROIHandle() ||
                            !m_smVisionInfo.g_arrPadROIs[tabCtrl_PadPG.SelectedIndex][0].GetROIHandle())
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                            {
                                if (m_smVisionInfo.g_arrPadROIs[i][0].GetROIHandle())
                                {
                                    // Update TabPage Display base on ROI selection
                                    tabCtrl_PadSide.SelectedIndex = i;
                                    tabCtrl_PadPG.SelectedIndex = i;
                                }
                            }
                        }
                    }
                }
            }

            if (m_smVisionInfo.g_intLearnStepNo == 1)
            {
                if (m_smVisionInfo.AT_VM_UpdateResult)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                    {
                        if (m_smVisionInfo.g_arrPositioningROIs[i].GetROIHandle())
                        {
                            int intIndex;
                            switch (i)
                            {
                                case 4:
                                    intIndex = 4;
                                    break;
                                case 5:
                                    intIndex = 6;
                                    break;
                                case 6:
                                    intIndex = 8;
                                    break;
                                case 7:
                                    intIndex = 10;
                                    break;
                                default:
                                    intIndex = i;
                                    break;
                            }

                            if (i < 4)
                            {
                                lbl_ValidateScore.Text = m_smVisionInfo.g_arrPositioningGauges[intIndex].ref_ObjectScore.ToString("f2");
                                lbl_LineAngle.Text = m_smVisionInfo.g_arrPositioningGauges[intIndex].ref_ObjectAngle.ToString("f2");
                            }
                            else
                            {
                                lbl_ValidateScore.Text = m_smVisionInfo.g_arrPositioningGauges[intIndex].ref_ObjectScore.ToString("f2") + " / " + m_smVisionInfo.g_arrPositioningGauges[intIndex + 1].ref_ObjectScore.ToString("f2");
                                lbl_LineAngle.Text = m_smVisionInfo.g_arrPositioningGauges[intIndex].ref_ObjectAngle.ToString("f2") + " / " + m_smVisionInfo.g_arrPositioningGauges[intIndex + 1].ref_ObjectAngle.ToString("f2");
                            }
                        }
                    }

                    lbl_DieWidth.Text = Math.Round(m_smVisionInfo.g_objPositioning.ref_fObjectWidth / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero).ToString();
                    lbl_DieHeight.Text = Math.Round(m_smVisionInfo.g_objPositioning.ref_fObjectHeight / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero).ToString();
                    lbl_DieAngle.Text = m_smVisionInfo.g_objPositioning.ref_fObjectAngle.ToString("F2");

                    m_smVisionInfo.AT_VM_UpdateResult = false;
                }
            }

            if (m_objAdvancedRectGauge4LForm != null)
            {
                if (!m_objAdvancedRectGauge4LForm.ref_blnShowForm)
                {
                    m_smVisionInfo.g_blnReferTemplateSize = true;
                    //m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[0].ref_intPadPkgSizeImageViewNo, m_smVisionInfo.g_intVisionIndex);

                    DefineMostSelectedImageNo(); // 2020-02-17 ZJYEOH : Display the image selected from gauge advance setting

                    if(!m_blnOrientTestDone)
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    else
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
                        m_smVisionInfo.g_arrPad[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
                        m_smVisionInfo.g_arrPad[i].ref_blnDrawTransitionTypeArrow = false;
                    }

                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    m_objAdvancedRectGauge4LForm.Close();
                    m_objAdvancedRectGauge4LForm.Dispose();
                    m_objAdvancedRectGauge4LForm = null;

                    btn_Next.Enabled = btn_Previous.Enabled = btn_Save.Enabled = btn_GaugeSaveClose.Enabled = btn_Cancel.Enabled = btn_PackageGaugeSetting.Enabled =
                    chk_ShowDraggingBox.Enabled = chk_ShowSamplePoints.Enabled = chk_UseGauge.Enabled = true;
                }
                else
                    m_smVisionInfo.g_blnReferTemplateSize = false;
            }
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                //m_smVisionInfo.g_arrPad[i].ref_fFilterMinArea = float.Parse(txt_MinArea.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelX;
                m_smVisionInfo.g_arrPad[i].ref_fFilterMinArea = float.Parse(txt_MinArea.Text);

            }

            BuildObjectsAndSetToTemplateArray();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }



        private void LearnPadForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnReferTemplateSize = true;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].BackupPreviousTolerance();
            }

            // Make sure all pad are unlocked
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i] != null)
                {
                    if (m_smVisionInfo.g_arrPad[i].ref_blnInspectLock)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_blnInspectLock = false;
                    }
                }

            }

            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (m_smVisionInfo.g_arrPolygon_Pad.Count == i)
                {
                    m_smVisionInfo.g_arrPolygon_Pad.Add(new List<Polygon>());
                    //m_smVisionInfo.g_arrPolygon_Pad[i].Add(new Polygon());
                }
            }

            if (m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Pad[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
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

            Cursor.Current = Cursors.Default;
            //m_smVisionInfo.g_intLearnStepNo = 0;
            SetupSteps(true);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void LearnPadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Learn Pad Form Closed", "Exit Learn Pad Form", "", "", m_smProductionInfo.g_strLotID);
            if (chk_UsePreTemplateImage.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (i < m_arrCurrentImage.Count)
                        m_arrCurrentImage[i].CopyTo(ref m_smVisionInfo.g_arrImages, i);
                }
            }
            m_smVisionInfo.g_blnViewRotatedImage_AfterMouseUp = false;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = 0;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewObjectsBuilded = false;
            m_smVisionInfo.g_blnViewCharsBuilded = false;
            m_smVisionInfo.g_blnViewTextsBuilded = false;
            m_smVisionInfo.g_blnMarkInspected = false;
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewPin1ROI = false;
            m_smVisionInfo.g_blnViewPin1TrainROI = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_blnPadAllowCustomize = true;
        }

        private void trackBar_Threshold_Scroll(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex == 0)
                m_smVisionInfo.g_objPositioning.ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);
            else
                m_smVisionInfo.g_objPositioning.ref_fGainValue2 = Convert.ToSingle(trackBar_Gain.Value);
            float fGain = Convert.ToSingle(trackBar_Gain.Value) / 1000;
            lbl_GainValue.Text = fGain.ToString();
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, fGain);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            AdvancedLGaugeForm objAdvancedForm = new AdvancedLGaugeForm(m_smVisionInfo, m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\Gauge.xml", m_smProductionInfo, false);

            if (objAdvancedForm.ShowDialog() == DialogResult.OK)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DirectToPackageForm_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intLearnStepNo = 10;

            m_intDisplayStepNo = 10;

            m_blnJumpToPkgPage = true;

            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Image1Threshold_Click(object sender, EventArgs e)
        {
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[0].ref_intPkgImage1LowPadThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[0].ref_intPkgImage1HighPadThreshold;


            ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[0]);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[0][1]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage1LowPadThreshold = intLowThreshold;
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage1HighPadThreshold = intHighThreshold;
            }
            else
            {
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage1LowPadThreshold = m_smVisionInfo.g_intLowThresholdValue;
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage1HighPadThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Image2Threshold_Click(object sender, EventArgs e)
        {
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[0].ref_intPkgImage2LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[0].ref_intPkgImage2HighThreshold;


            ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[1]);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[0][1]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage2LowThreshold = intLowThreshold;
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage2HighThreshold = intHighThreshold;
            }
            else
            {
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Image3Threshold_Click(object sender, EventArgs e)
        {
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[0].ref_intPkgImage3LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[0].ref_intPkgImage3HighThreshold;


            ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[2]);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[0][1]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage3LowThreshold = intLowThreshold;
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage3HighThreshold = intHighThreshold;
            }
            else
            {
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage3LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage3HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Image1SurfaceThreshold_Click(object sender, EventArgs e)
        {
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[0].ref_intPkgImage1LowSurfaceThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[0].ref_intPkgImage1HighSurfaceThreshold;


            ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[0]);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[0][1]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage1LowSurfaceThreshold = intLowThreshold;
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage1HighSurfaceThreshold = intHighThreshold;
            }
            else
            {
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intLowThresholdValue;
                m_smVisionInfo.g_arrPad[0].ref_intPkgImage1HighSurfaceThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PositionSetting_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objPositioning.ref_fGainValue = (float)trackBar_Gain.Value;
            m_smVisionInfo.g_objPositioning.ref_intDieAngleLimit = Convert.ToInt32(txt_LineAngle.Text);
            m_smVisionInfo.g_objPositioning.ref_intMinBorderScore = Convert.ToInt32(txt_LineGaugeMinScore.Text);
            m_smVisionInfo.g_objPositioning.ref_fSampleDieWidth = Convert.ToSingle(txt_UnitSizeWidth.Text);
            m_smVisionInfo.g_objPositioning.ref_fSampleDieHeight = Convert.ToSingle(txt_UnitSizeHeight.Text);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_UsePreTemplateImage_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (i >= m_arrCurrentImage.Count)
                    m_arrCurrentImage.Add(new ImageDrawing(true));
            }

            if (chk_UsePreTemplateImage.Checked)
            {
                m_smVisionInfo.g_arrImages[0].CopyTo(ref m_arrCurrentImage, 0);
                m_smVisionInfo.g_arrImages[0].LoadImage(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\OriTemplate.bmp");

                if (m_smVisionInfo.g_arrImages.Count > 1)
                {
                    m_smVisionInfo.g_arrImages[1].CopyTo(ref m_arrCurrentImage, 1);
                    m_smVisionInfo.g_arrImages[1].LoadImage(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\OriTemplate_Image1.bmp");
                }
                if (m_smVisionInfo.g_arrImages.Count > 2)
                {
                    m_smVisionInfo.g_arrImages[2].CopyTo(ref m_arrCurrentImage, 2);
                    m_smVisionInfo.g_arrImages[2].LoadImage(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Template\\OriTemplate_Image2.bmp");
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (i < m_arrCurrentImage.Count)
                        m_arrCurrentImage[i].CopyTo(ref m_smVisionInfo.g_arrImages, i);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SaveOnlyPosition_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPadROIs[0].Count < 2)
            {
                SRMMessageBox.Show("Save cannot be completed because pad is never learnt before.");
                return;
            }
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\";

            SavePositioningSetting(strPath + "Positioning\\");
            SaveROISetting(strPath + "Pad\\");

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void cbo_ImagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex = m_smVisionInfo.g_intSelectedImage = cbo_ImagesList.SelectedIndex;

            if (m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex == 0)
            {
                trackBar_Gain.Value = (int)m_smVisionInfo.g_objPositioning.ref_fGainValue;
                lbl_GainValue.Text = (m_smVisionInfo.g_objPositioning.ref_fGainValue / 1000).ToString();
            }
            else
            {
                trackBar_Gain.Value = (int)m_smVisionInfo.g_objPositioning.ref_fGainValue2;
                lbl_GainValue.Text = (m_smVisionInfo.g_objPositioning.ref_fGainValue2 / 1000).ToString();
            }

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(lbl_GainValue.Text));

            for (int g = 0; g < m_smVisionInfo.g_arrPositioningGauges.Count; g++)
            {
                if (m_smVisionInfo.g_objPositioning.ref_intPositionImageIndex == 0)
                    m_smVisionInfo.g_arrPositioningGauges[g].ref_GaugeTransType = 1;
                else
                    m_smVisionInfo.g_arrPositioningGauges[g].ref_GaugeTransType = 0;
            }



            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        // New
        private void btn_Rotate_Click(object sender, EventArgs e)
        {
            float fRotatedDegree = float.Parse(txt_RotateDegree.Text) * -1f;

            ROI objROI = new ROI();
            objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
            // Use g_arrPadROIs[0][1] center point, but use g_arrPadROIs[0][0] size. This is to prevent the rotated defect happen at the edge of unit.
            int intRotateCenterX = m_smVisionInfo.g_arrPadROIs[0][1].ref_ROITotalCenterX;
            int intRotateCenterY = m_smVisionInfo.g_arrPadROIs[0][1].ref_ROITotalCenterY;

            int intStartX = Math.Max(0, intRotateCenterX - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth / 2);
            int intEndX = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageWidth, intRotateCenterX + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth / 2);
            int intHalfWidth = Math.Min(intRotateCenterX - intStartX, intEndX - intRotateCenterX);
            intStartX = intRotateCenterX - intHalfWidth;

            int intStartY = Math.Max(0, intRotateCenterY - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight / 2);
            int intEndY = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageHeight, intRotateCenterY + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight / 2);
            int intHalfHeight = Math.Min(intRotateCenterY - intStartY, intEndY - intRotateCenterY);
            intStartY = intRotateCenterY - intHalfHeight;

            objROI.LoadROISetting(intStartX, intStartY, intHalfWidth * 2, intHalfHeight * 2);

            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objROI, fRotatedDegree, ref m_smVisionInfo.g_arrRotatedImages, 0);

            switch (m_smVisionInfo.g_arrPad[0].ref_objRectGauge4L.GetGaugeImageMode(0))
            {
                default:
                case 0:
                    {
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain);
                    }
                    break;
                case 1:
                    {
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain);
                        m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                    }
                    break;
            }

            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            objROI.Dispose();
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_PackageGaugeSetting_Click(object sender, EventArgs e)
        {
            int intPadSelectedIndex = m_smVisionInfo.g_intSelectedROI;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnDrawDraggingBox = true;
                m_smVisionInfo.g_arrPad[i].ref_blnDrawSamplingPoint = true;
                m_smVisionInfo.g_arrPad[i].ref_blnDrawTransitionTypeArrow = true;
            }

            string strRectGaugePath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\RectGauge4L.xml";

            string strVisionFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName;

            m_objAdvancedRectGauge4LForm = new AdvancedRectGauge4LPadForm(m_smVisionInfo, intPadSelectedIndex, strRectGaugePath, strVisionFolderPath, m_smProductionInfo, m_smCustomizeInfo, m_intVisionType);
            m_objAdvancedRectGauge4LForm.StartPosition = FormStartPosition.Manual;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            m_objAdvancedRectGauge4LForm.Location = new Point(resolution.Width - m_objAdvancedRectGauge4LForm.Size.Width, resolution.Height - m_objAdvancedRectGauge4LForm.Size.Height);
            m_objAdvancedRectGauge4LForm.Show();

            btn_Next.Enabled = btn_Previous.Enabled = btn_Save.Enabled = btn_GaugeSaveClose.Enabled = btn_Cancel.Enabled = btn_PackageGaugeSetting.Enabled =
            chk_UseGauge.Enabled = false;
            //chk_ShowDraggingBox.Enabled = chk_ShowSamplePoints.Enabled = true;
        }

        private void tp_position_Click(object sender, EventArgs e)
        {

        }

        private void txt_CameraGain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fImageGain = Convert.ToSingle(txt_ImageGain.Value);

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                    m_smVisionInfo.g_arrPad[i].ref_fImageGain = fImageGain;
            }

            switch (m_smVisionInfo.g_arrPad[0].ref_objRectGauge4L.GetGaugeImageMode(0))
            {
                default:
                case 0:
                    {
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                        {
                            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
                        }
                    }
                    break;
                case 1:
                    {
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                        {
                            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, fImageGain);
                            m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, fImageGain);
                            m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                        }

                    }
                    break;
            }
            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_GaugeSaveClose_Click(object sender, EventArgs e)
        {
            //Check is gauge ok or not
            if (chk_UseGauge.Checked)
            {
                bool blnGaugeOK = true;
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                        continue;

                    if (!m_smVisionInfo.g_arrPad[i].IsGaugeMeasureOK(i))
                    {
                        string strDirectionName;
                        switch (i)
                        {
                            case 0:
                            default:
                                strDirectionName = "Center";
                                break;
                            case 1:
                                strDirectionName = "Top";
                                break;
                            case 2:
                                strDirectionName = "Right";
                                break;
                            case 3:
                                strDirectionName = "Bottom";
                                break;
                            case 4:
                                strDirectionName = "Left";
                                break;

                        }
                        SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");
                        blnGaugeOK = false;
                        break;

                    }
                }

                if (!blnGaugeOK)
                {
                    return;
                }
            }

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\Pad\\";
            string strOrientPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
              m_smVisionInfo.g_strVisionFolderName + "\\Orient\\";

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

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


                if (m_intVisionType == 0)
                {
                    if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                        if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                            if(m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                            m_smVisionInfo.g_arrPadOrientROIs[0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
                }
                else
                {
                    if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                        if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                            if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                                m_smVisionInfo.g_arrPadROIs[0][0].LoadROISetting(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight);
                }

                SaveROISetting(strFolderPath);
                SaveOrientROISetting(strOrientPath);
                
                
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrPad[i].SavePad(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Pad", m_smProductionInfo.g_strLotID);

                STDeviceEdit.CopySettingFile(strFolderPath, "RectGauge4L.xml");
                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SaveRectGauge4L(
                   strFolderPath + "RectGauge4L.xml",
                   false,
                   "Pad" + i.ToString(),
                   true,
                   true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Pad RectGauge4L", m_smProductionInfo.g_strLotID);
                
            }

            // Reload all information (User may go to last page to change setting)
            strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadROISetting(strFolderPath + "Pad\\ROI.xml", m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrPad.Length);
            LoadROISetting(strOrientPath + "ROI.xml", m_smVisionInfo.g_arrPadOrientROIs);

            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                LoadPadOffSetSetting(strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                LoadPadOffSetSetting(strFolderPath + "Calibration.xml");
            LoadPadSetting(strFolderPath);
            LoadPositioningSetting(strFolderPath + "Positioning\\");

            LoadMatcherFile(strFolderPath);
            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void btn_ROISaveClose_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                           m_smVisionInfo.g_strVisionFolderName + "\\Pad\\";
            string strOrientPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                           m_smVisionInfo.g_strVisionFolderName + "\\Orient\\";

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {

                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

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


                if (m_intVisionType == 0)
                {
                    if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                        if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                            if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                                m_smVisionInfo.g_arrPadOrientROIs[0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
                }
                else
                {
                    if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                        if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                            if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                                m_smVisionInfo.g_arrPadROIs[0][0].LoadROISetting(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight);
                }



                SaveROISetting(strFolderPath);
                SaveOrientROISetting(strOrientPath);

                // 2020 02 23 - CCENG: Save ROI only. No need to save gauge. Unit ROI not support affecting gauge edge ROI. (Am I right?)
                // Set RectGauge4L Placement
                //if (m_smVisionInfo.g_arrPadROIs[i].Count > 1)
                //{
                //    //m_smVisionInfo.g_arrPad[i].SetRectGauge4LPlacement(m_smVisionInfo.g_arrPadROIs[i][1]);
                //    m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();
                //    m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_objWhiteImage);
                //}

                
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrPad[i].SavePad(strFolderPath + "Template\\Template.xml",
                   false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Pad", m_smProductionInfo.g_strLotID);
                
                // 2020 04 07 - CCENG: No need to save gauge here.
                //STDeviceEdit.CopySettingFile(strFolderPath, "RectGauge4L.xml");
                //m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SaveRectGauge4L(
                //   strFolderPath + "RectGauge4L.xml",
                //   false,
                //   "Pad" + i.ToString(),
                //   true,
                //   true);
                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad RectGauge4L", strFolderPath, "RectGauge4L.xml", m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantDontCareArea_Pad)
            {
                SaveDontCareROISetting(strFolderPath);
                SaveDontCareSetting(strFolderPath + "Template\\");
            }

            // Reload all information (User may go to last page to change setting)
            strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadROISetting(strFolderPath + "Pad\\ROI.xml", m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrPad.Length);
            LoadROISetting(strOrientPath + "ROI.xml", m_smVisionInfo.g_arrPadOrientROIs);

            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                LoadPadOffSetSetting(strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                LoadPadOffSetSetting(strFolderPath + "Calibration.xml");
            LoadPadSetting(strFolderPath);
            LoadPositioningSetting(strFolderPath + "Positioning\\");

            LoadMatcherFile(strFolderPath);
            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void radioBtn_SelectPadROI_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (radioBtn_Middle.Checked)
            {
                m_smVisionInfo.g_intSelectedROI = 0;
            }
            else if (radioBtn_Up.Checked)
            {
                m_smVisionInfo.g_intSelectedROI = 1;
            }
            else if (radioBtn_Right.Checked)
            {
                m_smVisionInfo.g_intSelectedROI = 2;
            }
            else if (radioBtn_Down.Checked)
            {
                m_smVisionInfo.g_intSelectedROI = 3;
            }
            else if (radioBtn_Left.Checked)
            {
                m_smVisionInfo.g_intSelectedROI = 4;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_UseGauge_Click(object sender, EventArgs e)
        {
            if (m_intAdvSettingEdgeToolMode == 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize = chk_UseGauge.Checked;
                }

                SetUseGaugeGUI();
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    // Center Pad will always use gauge to measure package edge. Side pad can use PR if the edge is not clear. 
                    if (i == 0)
                        m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize = true;
                    else
                        m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize = chk_UseGauge.Checked;
                }

                SetUseGaugeGUI();
            }
            
            m_smVisionInfo.g_blnUpdateSelectedROI = true; // Update Step 2 Instruction Picture box
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_SelectImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage = cbo_SelectImage.SelectedIndex;

            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (m_smVisionInfo.g_arrPad[i].ref_blnSelected)
            //    {
            //        m_smVisionInfo.g_arrPad[i].ref_intCheckPadDimensionImageIndex = m_smVisionInfo.g_intSelectedImage;
            //    }
            //}

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_SelectPadROIs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (cbo_SelectPadROIs.SelectedIndex)
            {
                case 0:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_blnSelected = true;
                    }

                    m_smVisionInfo.g_intSelectedImage = cbo_SelectImage.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intCheckPadDimensionImageIndex;
                    break;
                case 1:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i == 0)
                            m_smVisionInfo.g_arrPad[i].ref_blnSelected = true;
                        else
                            m_smVisionInfo.g_arrPad[i].ref_blnSelected = false;
                    }

                    m_smVisionInfo.g_intSelectedImage = cbo_SelectImage.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intCheckPadDimensionImageIndex;
                    break;
                case 2:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i == 0)
                            m_smVisionInfo.g_arrPad[i].ref_blnSelected = false;
                        else
                            m_smVisionInfo.g_arrPad[i].ref_blnSelected = true;
                    }

                    if (m_smVisionInfo.g_arrPad.Length > 1)
                        m_smVisionInfo.g_intSelectedImage = cbo_SelectImage.SelectedIndex = m_smVisionInfo.g_arrPad[1].ref_intCheckPadDimensionImageIndex;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_PadSurfaceThreshold_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            {
                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                    continue;

                if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
                {
                    intSelectedROI = j;
                    break;
                }
            }

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrPad.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intInterPadThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 1)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intInterPadThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 2)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intInterPadThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 3)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intInterPadThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 4)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intInterPadThresholdValue);

            bool blnUseDoubleThreshold = false;

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intInterPadThresholdValue;
            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_intInterPadThresholdValue = m_smVisionInfo.g_intThresholdValue;

                        ////Rebuild pad objects
                        //if (blnUseDoubleThreshold && i > 0)
                        //    m_smVisionInfo.g_arrPad[i].BuildDoubleThresholdPadObjects(m_smVisionInfo.g_arrPadROIs[i][3], i);
                        //else
                        //    m_smVisionInfo.g_arrPad[i].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[i][3]);// 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package

                        ////Clear Temp Blobs Features
                        //m_smVisionInfo.g_arrPad[i].ClearTempBlobsFeatures();

                        ////Set blobs features data into Temp Blobs Features
                        //m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrPadROIs[i][3],
                        //      0,//m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                        //    0);// m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset

                        ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                        //m_smVisionInfo.g_arrPad[i].CompareSelectedBlobs();
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intInterPadThresholdValue = m_smVisionInfo.g_intThresholdValue;

                    ////Rebuild pad objects
                    //if (blnUseDoubleThreshold && intSelectedROI > 0)
                    //    m_smVisionInfo.g_arrPad[intSelectedROI].BuildDoubleThresholdPadObjects(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3], intSelectedROI);
                    //else
                    //    m_smVisionInfo.g_arrPad[intSelectedROI].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3]);// 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package

                    ////Clear Temp Blobs Features
                    //m_smVisionInfo.g_arrPad[intSelectedROI].ClearTempBlobsFeatures();

                    ////Set blobs features data into Temp Blobs Features
                    //m_smVisionInfo.g_arrPad[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3],
                    //     0,// m_smVisionInfo.g_arrPadROIs[intSelectedROI][2].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[intSelectedROI][3].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                    //       0);// m_smVisionInfo.g_arrPadROIs[intSelectedROI][2].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[intSelectedROI][3].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset

                    ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                    //m_smVisionInfo.g_arrPad[intSelectedROI].CompareSelectedBlobs();
                }
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intInterPadThresholdValue;

                ////Rebuild pad objects
                //if (blnUseDoubleThreshold && intSelectedROI > 0)
                //    m_smVisionInfo.g_arrPad[intSelectedROI].BuildDoubleThresholdPadObjects(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3], intSelectedROI);
                //else
                //    m_smVisionInfo.g_arrPad[intSelectedROI].BuildOnlyPadObjects(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3]);// 2019-09-24 ZJYEOH: use back Pad ROI (m_smVisionInfo.g_arrPadROIs[i][3]) to build blobs as encountered case where pad is outside package

                ////Clear Temp Blobs Features
                //m_smVisionInfo.g_arrPad[intSelectedROI].ClearTempBlobsFeatures();

                ////Set blobs features data into Temp Blobs Features
                //m_smVisionInfo.g_arrPad[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrPadROIs[intSelectedROI][3],
                //    0,//  m_smVisionInfo.g_arrPadROIs[intSelectedROI][2].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[intSelectedROI][3].ref_ROITotalX,// 2019-09-24 ZJYEOH: No more Offset
                //      0);//      m_smVisionInfo.g_arrPadROIs[intSelectedROI][2].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[intSelectedROI][3].ref_ROITotalY);// 2019-09-24 ZJYEOH: No more Offset

                ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                //m_smVisionInfo.g_arrPad[intSelectedROI].CompareSelectedBlobs();
            }

            BuildObjectsAndSetToTemplateArray();

            objThresholdForm.Dispose();



            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_TopSetting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SetUseGaugeGUI()
        {
            chk_UseGauge.Visible = m_smVisionInfo.g_arrPad[0].ref_blnWantShowUseGaugeCheckBox && m_blnGaugeButtonVisible;

            if (chk_UseGauge.Checked)
            {
                chk_ShowDraggingBox.Enabled = m_blnUserRightEnableGaugeSetting;
                chk_ShowSamplePoints.Enabled = m_blnUserRightEnableGaugeSetting;
                btn_PackageGaugeSetting.Enabled = m_blnUserRightEnableGaugeSetting;
                //gp_Rotation.Visible = false;
                gBox_PreciseAngle.Visible = false;
                btn_GaugeSaveClose.Text = "Save Gauge && Close";
                
                DefineMostSelectedImageNo();
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_blnViewGauge = true;
            }
            else
            {
                if (!m_smVisionInfo.g_arrPad[0].ref_blnWantShowUseGaugeCheckBox)
                {
                    chk_ShowDraggingBox.Visible = false;
                    chk_ShowSamplePoints.Visible = false;
                    btn_PackageGaugeSetting.Visible = false;
                }

                chk_ShowDraggingBox.Checked = true;
                //chk_ShowSamplePoints.Checked = false;
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    m_smVisionInfo.g_arrPad[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
                }

                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    m_smVisionInfo.g_arrPad[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
                }
                chk_ShowDraggingBox.Enabled = false;
                chk_ShowSamplePoints.Enabled = false;
                btn_PackageGaugeSetting.Enabled = false;
                //gp_Rotation.Visible = true;
                gBox_PreciseAngle.Visible = m_blnRotateButtonVisible;//true
                btn_GaugeSaveClose.Text = "Save Pattern && Close";
                RotatePrecise();
                m_smVisionInfo.g_intSelectedImage = 0;
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_blnViewGauge = false;
            }

            // 2019 10 30 - CCENG: Force user to learn until last steps if user change from "Want Use guage" to "No Use Guage".
            if (m_blnWantGaugeMeasurePkgSizePrev == chk_UseGauge.Checked)
            {
                btn_GaugeSaveClose.Visible = m_blnSaveButtonVisible;//true
            }
            else
            {
                btn_GaugeSaveClose.Visible = false;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void srmCheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private bool DefineGroupPadTolerance()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                    continue;

                m_smVisionInfo.g_arrPad[i].DefineGroupTolerance();

            }

            return true;
        }

        private bool DefinePadToleranceUsingDefaultSetting(string[] strROIName)
        {
            int intCount = 0;
            //Use prev setting
            if (m_smVisionInfo.g_blnUsedPreTemplate)    // blnUsedPreTemplate will be set to True everytime user go into LearnPadForm.
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        continue;

                    //Check no of objects is it tally with prev setting
                    if (!m_smVisionInfo.g_arrPad[i].CheckPadTally(ref intCount))
                    {
                        if (intCount == 0)
                        {
                            if (SRMMessageBox.Show("Selected pads is not tally with previous pads record in " + strROIName[i] + ". There are " + intCount + " pads in previous record." +
                                " Do you want to continue with default tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                chk_UsePreviousTolerance.Checked = false;
                                m_smVisionInfo.g_blnUsedPreTemplate = false;
                                break;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (SRMMessageBox.Show("Selected pads is not tally with previous pads record in " + strROIName[i] + ". There are " + intCount + " pads in previous record." +
                                " Do you want to continue with previous pads tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                chk_UsePreviousTolerance.Checked = false;
                                m_smVisionInfo.g_blnUsedPreTemplate = false;
                                break;
                            }
                            else
                            {
                                return false;
                            }

                        }
                    }
                    else
                    {
                        //Check Blobs position is it match with prev setting
                        if (!m_smVisionInfo.g_arrPad[i].MatchPrevSettings())
                        {
                            chk_UsePreviousTolerance.Checked = false;
                            m_smVisionInfo.g_blnUsedPreTemplate = false;

                            m_smVisionInfo.g_arrPad[i].ClearTemplateBlobsFeatures();

                            m_smVisionInfo.g_arrPad[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrPadROIs[i][3],
                                  m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX,
                            m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY - m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY);
                            m_smVisionInfo.g_blnViewObjectsBuilded = true;

                            //if (SRMMessageBox.Show("Unable to match current pad settings with previous pad record in " + strROIName[i] + "! " +
                            //  "Do you want to continue with default tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            //{
                            //    chk_UsePreviousTolerance.Checked = false;
                            //    m_smVisionInfo.g_blnUsedPreTemplate = false;
                            //    break;
                            //}
                            //else
                            //{
                            //    return false;
                            //}
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPad[i].RearrangeBlobs();
                            m_smVisionInfo.g_arrPad[i].DefineTolerance(false);
                            m_smVisionInfo.g_arrPad[i].BuildPadsParameter(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPadROIs[i][0]);   // Index 0: Search ROI, 1:Gauge ROI, 2: Package ROI
                        }
                    }
                }
            }

            return true;
        }

        private bool DefinePadToleranceUsingClosestSizeMethod(string[] strROIName)
        {
            int intCount = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                    continue;

                //Check no of objects is it tally with prev setting. (As Warning. User still allow to proceed even pads is not tally)
                if (!m_smVisionInfo.g_arrPad[i].CheckPadTally(ref intCount))
                {
                    if (SRMMessageBox.Show("Selected pads is not tally with previous pads record in " + strROIName[i] + ". There are " + intCount + " pads in previous record." +
                        " Do you want to continue?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        break;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                    continue;


                m_smVisionInfo.g_arrPad[i].MatchPrevSettings_AutoGenerate();
            }

            return true;
        }

        private void cbo_ReferenceCorner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (!m_blnStepPadDone)
                return;
            int intSelectedROI = GetSelectedROI();

            m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadLabelRefCorner = cbo_ReferenceCorner.SelectedIndex;

            if (m_smVisionInfo.g_arrPad[intSelectedROI].ref_blnWantUseClosestSizeDefineTolerance)
            {
                ReadAllPadTemplateDataToGrid_ClosestSizeMethod();
            }
            else
            {
                ReadAllPadTemplateDataToGrid_DefaultMethod();
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_PadLabelDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (!m_blnStepPadDone)
                return;

            int intSelectedROI = GetSelectedROI();

            m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadLabelDirection = cbo_PadLabelDirection.SelectedIndex;

            if (m_smVisionInfo.g_arrPad[intSelectedROI].ref_blnWantUseClosestSizeDefineTolerance)
            {
                ReadAllPadTemplateDataToGrid_ClosestSizeMethod();
            }
            else
            {
                ReadAllPadTemplateDataToGrid_DefaultMethod();
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private int GetSelectedROI()
        {
            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            {
                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                    continue;

                if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
                {
                    intSelectedROI = j;
                    break;
                }
            }

            return intSelectedROI;
        }

        private void srmLabel26_Click(object sender, EventArgs e)
        {

        }

        private void btn_Undo_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawFreeShapeDone = true;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            {
                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (m_smVisionInfo.g_arrPadROIs[j].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadROIs[j][3].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }
            m_smVisionInfo.g_arrPolygon_Pad[intSelectedROI][m_smVisionInfo.g_intSelectedDontCareROIIndex].UndoPolygon();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_DontCareAreaDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            //for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            //{
            //    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    m_smVisionInfo.g_arrPolygon_Pad[i][0].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;

            //}

            if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
            {
                pnl_DontCareFreeShape.Visible = true;
            }
            else
            {
                pnl_DontCareFreeShape.Visible = false;
            }
        }

        private void btn_PadROIToleranceSetting_Click(object sender, EventArgs e)
        {
            PadROIToleranceSettingForm objPadROIToleranceSettingForm = new PadROIToleranceSettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, m_intUserGroup, 0);
            objPadROIToleranceSettingForm.ShowDialog();

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                /*
                 * 2020 02 23 - CCENG: when is blnMeasureCenterPkgSizeUsingSidePkg, no need purposely get corner points from side pad. 
                 *                   : during gauge measurement at steps 2, the function MeasureEdge_UsingSidePkgCornerPoint have been called and the corner points from side pad have been transfer to center gauge.
                 */
                //if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                //{
                //    float fLengthTop = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop;
                //    float fLengthRight = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight;
                //    float fLengthBottom = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom;
                //    float fLengthLeft = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft;

                //    PointF p1 = new PointF(m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X, m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y);
                //    PointF p2 = new PointF(m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X, m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y);
                //    PointF p3 = new PointF(m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X, m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y);
                //    PointF p4 = new PointF(m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X, m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y);

                //    float fStartX = Math.Min(p1.X, p4.X);
                //    float fStartY = Math.Min(p1.Y, p2.Y);
                //    float fWidth = Math.Max(p2.X, p3.X) - fStartX;
                //    float fHeight = Math.Max(p3.Y, p4.Y) - fStartY;

                //    m_smVisionInfo.g_arrPadROIs[i][3].LoadROISetting(
                //    (int)Math.Round(fStartX - fLengthLeft, 0, MidpointRounding.AwayFromZero),
                //    (int)Math.Round(fStartY - fLengthTop, 0, MidpointRounding.AwayFromZero),
                //    (int)Math.Ceiling(fWidth + fLengthLeft + fLengthRight),
                //    (int)Math.Ceiling(fHeight + fLengthTop + fLengthBottom));


                //    m_smVisionInfo.g_arrPadROIs[i][3].SaveImage("D:\\TS\\padROI.bmp");
                //}
                //else
                {
                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
                    {
                        if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                        {
                            m_smVisionInfo.g_arrPad[0].MeasureEdge_UsingSidePkgCornerPoint(
                                  m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X,
                                  m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y,
                                  m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X,
                                  m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y,
                                  m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X,
                                  m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y,
                                  m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X,
                                  m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y);

                        }

                        m_smVisionInfo.g_arrPadROIs[i][3].LoadROISetting(
                        (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                        (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft, 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round(m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                        (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop, 0, MidpointRounding.AwayFromZero),
                        (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight),
                        (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom));
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPadROIs[i][3].LoadROISetting(m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIPositionX + m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIPositionX - m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft,
                     m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIPositionY + m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIPositionY - m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop,
                     m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIWidth + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight,
                     m_smVisionInfo.g_arrPadROIs[i][1].ref_ROIHeight + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop + m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom);
                    }
                }
            }

            BuildObjectsAndSetToTemplateArray();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void DefineMostSelectedImageNo()
        {
            int[] intCountImage = { 0, 0, 0, 0, 0 };

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                int[] arrPackageSizeImageIndex = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[i].GetEdgeImageViewNo(), m_smVisionInfo.g_intVisionIndex);

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
            }

            int intMostOccurValue = Math.Max(Math.Max(Math.Max(intCountImage[0], intCountImage[1]), Math.Max(intCountImage[2], intCountImage[3])), intCountImage[4]);
            m_smVisionInfo.g_intSelectedImage = Array.IndexOf(intCountImage, intMostOccurValue);
        }

        private void btn_AddDontCareROI_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].Add(new ROI());
            m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count - 1].AttachImage(m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3]);
            m_smVisionInfo.g_arrPolygon_Pad[m_smVisionInfo.g_intSelectedROI].Add(new Polygon());
            m_smVisionInfo.g_arrPolygon_Pad[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_arrPolygon_Pad[m_smVisionInfo.g_intSelectedROI].Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DeleteDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count == 0)
                return;

            m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_arrPolygon_Pad[m_smVisionInfo.g_intSelectedROI].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count - 1;
            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if ((m_smVisionInfo.g_arrPadDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                {
                    cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Pad[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
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
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void UpdatePadROIGUI()
        {
            m_blnInitDone = false;
            int intSelectedROI = GetSelectedROI();
            picUnitPadROI.Image = ils_ImageListTree.Images[intSelectedROI];

            switch (intSelectedROI)
            {
                case 0:
                case 1:
                    txt_PadStartPixelFromTop.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromTop.ToString();
                    if (txt_PadStartPixelFromTop.Text == "0")
                    {
                        txt_PadStartPixelFromTop.Text = "1";
                        txt_PadStartPixelFromTop.Text = "0";
                    }
                    txt_PadStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromRight.ToString();
                    if (txt_PadStartPixelFromRight.Text == "0")
                    {
                        txt_PadStartPixelFromRight.Text = "1";
                        txt_PadStartPixelFromRight.Text = "0";
                    }
                    txt_PadStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromBottom.ToString();
                    if (txt_PadStartPixelFromBottom.Text == "0")
                    {
                        txt_PadStartPixelFromBottom.Text = "1";
                        txt_PadStartPixelFromBottom.Text = "0";
                    }
                    txt_PadStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromLeft.ToString();
                    if (txt_PadStartPixelFromLeft.Text == "0")
                    {
                        txt_PadStartPixelFromLeft.Text = "1";
                        txt_PadStartPixelFromLeft.Text = "0";
                    }
                    break;
                case 2:
                    txt_PadStartPixelFromTop.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromRight.ToString();
                    if (txt_PadStartPixelFromTop.Text == "0")
                    {
                        txt_PadStartPixelFromTop.Text = "1";
                        txt_PadStartPixelFromTop.Text = "0";
                    }
                    txt_PadStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromBottom.ToString();
                    if (txt_PadStartPixelFromRight.Text == "0")
                    {
                        txt_PadStartPixelFromRight.Text = "1";
                        txt_PadStartPixelFromRight.Text = "0";
                    }
                    txt_PadStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromLeft.ToString();
                    if (txt_PadStartPixelFromBottom.Text == "0")
                    {
                        txt_PadStartPixelFromBottom.Text = "1";
                        txt_PadStartPixelFromBottom.Text = "0";
                    }
                    txt_PadStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromTop.ToString();
                    if (txt_PadStartPixelFromLeft.Text == "0")
                    {
                        txt_PadStartPixelFromLeft.Text = "1";
                        txt_PadStartPixelFromLeft.Text = "0";
                    }
                    break;
                case 3:
                    txt_PadStartPixelFromTop.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromBottom.ToString();
                    if (txt_PadStartPixelFromTop.Text == "0")
                    {
                        txt_PadStartPixelFromTop.Text = "1";
                        txt_PadStartPixelFromTop.Text = "0";
                    }
                    txt_PadStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromLeft.ToString();
                    if (txt_PadStartPixelFromRight.Text == "0")
                    {
                        txt_PadStartPixelFromRight.Text = "1";
                        txt_PadStartPixelFromRight.Text = "0";
                    }
                    txt_PadStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromTop.ToString();
                    if (txt_PadStartPixelFromBottom.Text == "0")
                    {
                        txt_PadStartPixelFromBottom.Text = "1";
                        txt_PadStartPixelFromBottom.Text = "0";
                    }
                    txt_PadStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromRight.ToString();
                    if (txt_PadStartPixelFromLeft.Text == "0")
                    {
                        txt_PadStartPixelFromLeft.Text = "1";
                        txt_PadStartPixelFromLeft.Text = "0";
                    }
                    break;
                case 4:
                    txt_PadStartPixelFromTop.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromLeft.ToString();
                    if (txt_PadStartPixelFromTop.Text == "0")
                    {
                        txt_PadStartPixelFromTop.Text = "1";
                        txt_PadStartPixelFromTop.Text = "0";
                    }
                    txt_PadStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromTop.ToString();
                    if (txt_PadStartPixelFromRight.Text == "0")
                    {
                        txt_PadStartPixelFromRight.Text = "1";
                        txt_PadStartPixelFromRight.Text = "0";
                    }
                    txt_PadStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromRight.ToString();
                    if (txt_PadStartPixelFromBottom.Text == "0")
                    {
                        txt_PadStartPixelFromBottom.Text = "1";
                        txt_PadStartPixelFromBottom.Text = "0";
                    }
                    txt_PadStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadROIStartPixelFromBottom.ToString();
                    if (txt_PadStartPixelFromLeft.Text == "0")
                    {
                        txt_PadStartPixelFromLeft.Text = "1";
                        txt_PadStartPixelFromLeft.Text = "0";
                    }
                    break;
            }
            m_blnInitDone = true;
        }

        private void txt_PadStartPixelFromTop_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intSelectedROI = GetSelectedROI();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_PadStartPixelFromRight.Text = txt_PadStartPixelFromTop.Text;
                    txt_PadStartPixelFromBottom.Text = txt_PadStartPixelFromTop.Text;
                    txt_PadStartPixelFromLeft.Text = txt_PadStartPixelFromTop.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromTop.Text);
                            }
                            break;
                    }
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PadStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intSelectedROI = GetSelectedROI();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_PadStartPixelFromTop.Text = txt_PadStartPixelFromRight.Text;
                    txt_PadStartPixelFromBottom.Text = txt_PadStartPixelFromRight.Text;
                    txt_PadStartPixelFromLeft.Text = txt_PadStartPixelFromRight.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromRight.Text);
                            }
                            break;
                    }
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PadStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intSelectedROI = GetSelectedROI();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_PadStartPixelFromTop.Text = txt_PadStartPixelFromBottom.Text;
                    txt_PadStartPixelFromRight.Text = txt_PadStartPixelFromBottom.Text;
                    txt_PadStartPixelFromLeft.Text = txt_PadStartPixelFromBottom.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromBottom.Text);
                            }
                            break;
                    }
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PadStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intSelectedROI = GetSelectedROI();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_PadStartPixelFromTop.Text = txt_PadStartPixelFromLeft.Text;
                    txt_PadStartPixelFromRight.Text = txt_PadStartPixelFromLeft.Text;
                    txt_PadStartPixelFromBottom.Text = txt_PadStartPixelFromLeft.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            if (chk_SetToAllEdge.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft = Convert.ToInt32(txt_PadStartPixelFromLeft.Text);
                            }
                            break;
                    }
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixel_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadExtendROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixel_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadExtendROI = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAllEdges_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_Pad", chk_SetToAllEdge.Checked);
        }

        private void chk_SetToAllSideROI_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllSide_Pad", chk_SetToAllSideROI.Checked);
        }

        private void btn_SaveSubLearn_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                   m_smVisionInfo.g_strVisionFolderName + "\\Pad\\";
            switch (m_intLearnType)
            {
                case 3:
                    //Save Pad Setting
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                            continue;

                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        // 22-08-2019 ZJYEOH : Reset all inspection data so that drawing will not cause error when opening inspection result and learn pad on the same time
                        m_smVisionInfo.g_arrPad[i].ResetInspectionData(false);

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

                        m_smVisionInfo.g_arrPad[i].SavePad(strPath + "Template\\Template.xml",
                            false, strSectionName, true);

                    }

                    //Load Pad Setting
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
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

                        m_smVisionInfo.g_arrPad[i].LoadPad(strPath + "Template\\Template.xml", strSectionName);
                    }
                    break;
                case 4:
                    if (m_smVisionInfo.g_blnWantDontCareArea_Pad)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPadDontCareROIs.Count; i++)
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                            {
                                if (m_smVisionInfo.g_arrPolygon_Pad[i][j].ref_intFormMode != 2)
                                {
                                    m_smVisionInfo.g_arrPolygon_Pad[i][j].AddPoint(new PointF(m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                                        m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                                    m_smVisionInfo.g_arrPolygon_Pad[i][j].AddPoint(new PointF((m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROITotalX + m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                                        (m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROITotalY + m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                                    m_smVisionInfo.g_arrPolygon_Pad[i][j].AddPolygon((int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPolygon_Pad[i][j].ResetPointsUsingOffset(m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrPadDontCareROIs[i][j].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                                    m_smVisionInfo.g_arrPolygon_Pad[i][j].AddPolygon((int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                                }
                                ImageDrawing objImage = new ImageDrawing(true);
                                if (m_smVisionInfo.g_arrPolygon_Pad[i][j].ref_intFormMode != 2)
                                    Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                                else
                                    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                                //ROI objDontCareROI = new ROI();
                                //objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIHeight);
                                //objDontCareROI.AttachImage(objImage);
                                //ROI.SubtractROI(m_smVisionInfo.g_arrPadROIs[i][3], objDontCareROI);
                                //objDontCareROI.Dispose();
                                objImage.Dispose();
                            }
                        }

                        SaveDontCareROISetting(strPath);
                        SaveDontCareSetting(strPath + "Template\\");

                        LoadROISetting(strPath + "DontCareROI.xml", m_smVisionInfo.g_arrPadDontCareROIs, m_smVisionInfo.g_arrPad.Length);
                        for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                        {
                            if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                            {
                                if (m_smVisionInfo.g_arrPadDontCareROIs.Count > i)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                                    {
                                        m_smVisionInfo.g_arrPadDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                                    }
                                }

                            }
                        }
                    }
                    break;
                case 5:
                    if (m_smVisionInfo.g_blnWantPin1)
                    {
                        if (m_smVisionInfo.g_arrPin1 != null)
                        {
                            // 2019 07 31 - CCENG: Fyi, both g_arrPadROIs[0][2] and ref_objPin1ROI attach to Pad Search ROI.
                            float fOffsetRefPosX = m_smVisionInfo.g_arrPadROIs[0][2].ref_ROICenterX -
                                                   m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROICenterX;

                            float fOffsetRefPosY = m_smVisionInfo.g_arrPadROIs[0][2].ref_ROICenterY -
                                                   m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROICenterY;

                            ROI objPin1ROI = m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI;
                            // Load Pin 1
                            m_smVisionInfo.g_arrPin1[0].LearnTemplate(m_smVisionInfo.g_intSelectedTemplate, fOffsetRefPosX, fOffsetRefPosY, objPin1ROI);

                            m_smVisionInfo.g_arrPin1[0].SaveTemplate(strPath + "Template\\");
                        }
                    }
                    break;
                case 6:
                case 7:
                    {
                        StartSplash();
                        string strPath2 = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\";

                        //m_smVisionInfo.g_intSelectedImage = 0;
                        m_smVisionInfo.g_intSavingState = 1;
                        ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[0]);

                        m_smVisionInfo.g_intSavingState = 2;
                        DefinePadContour();

                        m_smVisionInfo.g_intSavingState = 3;
                        SetDefaultSetting();

                        m_smVisionInfo.g_intSavingState = 4;
                        SavePadSettingOnly(strPath2);

                        m_smVisionInfo.g_intSavingState = 5;
                        SavePositioningSetting(strPath2 + "Positioning\\");

                        string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
                        m_smVisionInfo.g_intSavingState = 6;
                        LoadROISetting(strFolderPath + "Pad\\ROI.xml", m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrPad.Length);
                        LoadROISetting(strFolderPath + "Orient\\ROI.xml", m_smVisionInfo.g_arrPadOrientROIs);
                        LoadROISetting(strFolderPath + "Pad\\DontCareROI.xml", m_smVisionInfo.g_arrPadDontCareROIs, m_smVisionInfo.g_arrPad.Length);
                        for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                        {
                            if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                            {
                                if (m_smVisionInfo.g_arrPadDontCareROIs.Count > i)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                                    {
                                        m_smVisionInfo.g_arrPadDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                                    }
                                }

                            }
                        }

                        m_smVisionInfo.g_intSavingState = 7;
                        if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                            LoadPadOffSetSetting(strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
                        else
                            LoadPadOffSetSetting(strFolderPath + "Calibration.xml");

                        m_smVisionInfo.g_intSavingState = 8;
                        LoadPadSetting(strFolderPath);

                        m_smVisionInfo.g_intSavingState = 9;
                        LoadPositioningSetting(strFolderPath + "Positioning\\");

                        m_smVisionInfo.g_intSavingState = 10;
                        LoadMatcherFile(strFolderPath);

                        m_smVisionInfo.AT_PR_AttachImagetoROI = true;

                        CloseSplash();
                        m_smVisionInfo.g_intSavingState = 0;

                    }
                    break;
            }
            this.Close();
            this.Dispose();
        }
        private bool StartPin1Test()
        {
            // make sure template learn
            if (m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting.Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage += "*Pin1 : No Template Found";
                return false;
            }
            m_smVisionInfo.g_arrPin1[0].ResetInspectionData();
            m_smVisionInfo.g_arrPadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

            //int intMatchCount = 0;
            //bool blnResult;
            //string strErrorMessage = "";

            //if (!blnAuto)
            //{
            if (m_smVisionInfo.g_arrPin1[0].ref_objTestROI == null)
                m_smVisionInfo.g_arrPin1[0].ref_objTestROI = new ROI();
            
            m_smVisionInfo.g_arrPin1[0].ref_objTestROI.AttachImage(m_smVisionInfo.g_arrPadROIs[0][0]);

            //m_smVisionInfo.g_arrPin1[0].ref_objTestROI.SaveImage("D:\\TS\\TestROI.bmp");
            if (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
            {
                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                    (int)(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(0) * 0.75f - m_smVisionInfo.g_arrPin1[0].GetRefOffsetX(0)),
                    (int)(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(0) * 0.75f - m_smVisionInfo.g_arrPin1[0].GetRefOffsetY(0)),
                    (int)(m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(0) * 1.5),
                    (int)(m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(0) * 1.5));
            }
            else
            {
                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                    (int)(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_UnitMatcher().X - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(0) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetX(0)),
                    (int)(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_UnitMatcher().Y - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(0) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetY(0)),
                    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(0) * 2,
                    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(0) * 2);
            }

            //m_smVisionInfo.g_arrPin1[0].ref_objTestROI.SaveImage("D:\\TS\\TestROI2.bmp");

            bool blnResult = false;
            blnResult = m_smVisionInfo.g_arrPin1[0].MatchWithTemplate(m_smVisionInfo.g_arrPin1[0].ref_objTestROI, 0);

            if (blnResult)
            {
                if (m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI != null)
                {
                    m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.LoadROISetting(
                           (int)(m_smVisionInfo.g_arrPin1[0].ref_objTestROI.ref_ROITotalX - m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROITotalX +
                           m_smVisionInfo.g_arrPin1[0].GetResultPosX(m_smVisionInfo.g_intSelectedTemplate) -
                           m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(m_smVisionInfo.g_intSelectedTemplate) / 2),
                           (int)(m_smVisionInfo.g_arrPin1[0].ref_objTestROI.ref_ROITotalY - m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROITotalY +
                           m_smVisionInfo.g_arrPin1[0].GetResultPosY(m_smVisionInfo.g_intSelectedTemplate) -
                           m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(m_smVisionInfo.g_intSelectedTemplate) / 2),
                           m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(m_smVisionInfo.g_intSelectedTemplate),
                           m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(m_smVisionInfo.g_intSelectedTemplate));
                }
            }
            else
            {
                m_smVisionInfo.g_strErrorMessage += "*Pin1 : " + m_smVisionInfo.g_arrPin1[0].ref_strErrorMessage;
            }
            return blnResult;
        }
        private void btn_RotateUnit_Click(object sender, EventArgs e)
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;

            if (m_blnOrientTestDone && m_blnOrientFail)
            {
                m_smVisionInfo.g_arrPadOrientROIs[0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
            }

            ROI objROI = new ROI();
            List<ROI> arrROIs = new List<ROI>();

            objROI = m_smVisionInfo.g_arrPadOrientROIs[0];
            arrROIs = m_smVisionInfo.g_arrPadOrientROIs;

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                objRotatedImage = m_smVisionInfo.g_arrRotatedImages[i];
                m_smVisionInfo.g_arrImages[i].CopyTo(ref objRotatedImage);
            }

            // Rotate Unit IC
            if (m_smVisionInfo.g_arrPad[0].ref_intOrientDirections == 4)
            {
                if (sender == btn_ClockWise)
                    m_intCurrentAngle += 270;
                else
                    m_intCurrentAngle += 90;

                m_intCurrentAngle %= 360;


                ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

                for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
                {
                    ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, i);
                }

                if (m_smVisionInfo.g_blnViewPackageImage)
                {
                    //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
                    //{
                    //    m_smVisionInfo.g_arrRotatedImages[1].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    //}
                    //else
                    {
                        m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    }
                }
            }
            else
            {
                m_intCurrentAngle += 180;
                m_intCurrentAngle %= 360;

                ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);
                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }
            // After rotating the image, attach the rotated image into ROI again
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPadOrientROIs[0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            }

            //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
            //if (m_smVisionInfo.g_blnWantGauge)
            //{
            //    if (m_smVisionInfo.g_blnViewRotatedImage)
            //    {
            //        m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
            //    }
            //    else
            //    {
            //        m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
            //    }
            //}
            //else
            {
                // 2018 12 31 - CCENG: No Gain for image without gauge.
                if (m_smVisionInfo.g_blnViewRotatedImage)
                    m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
                else
                    m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
            }
            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage); // 02-07-2019 ZJYEOH : Solve rotate first time no effect
            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewGauge = false;  // 2019 06 21 - Gauge will measure according to original image only. Once rotate, gauge wont measure anymore, so not need to view Gauge after that.
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            
            if (m_intCurrentAngle == 270)
                lbl_OrientationAngle.Text = "-90";
            else
                lbl_OrientationAngle.Text = m_intCurrentAngle.ToString();

            RotatePrecise();
        }

        private void btn_OrientSaveClose_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                  m_smVisionInfo.g_strVisionFolderName + "\\";

            string strOrientPath = strFolderPath + "Orient\\";
            string strPkgPath = strFolderPath + "Package\\";
            strFolderPath = strFolderPath + "Pad\\";

            if (m_intVisionType == 0)
            {
                if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                    if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                        if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                            m_smVisionInfo.g_arrPadOrientROIs[0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
            }
            else
            {
                if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                    if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                        if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                            m_smVisionInfo.g_arrPadROIs[0][0].LoadROISetting(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight);
            }
            
            SaveROISetting(strFolderPath);
            SaveOrientROISetting(strOrientPath);

            #region Save Template Data and Images
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Pad\\";
            

            if (File.Exists(strOrientPath + "Template\\OriTemplate.bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Pad", "OriTemplate.bmp", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Pad", "", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(strOrientPath + "Template\\", strPath + "Old\\");
            
            // Save Learn Actual Image
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_smVisionInfo.g_arrColorImages[0].SaveImage(strOrientPath + "Template\\OriTemplate.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    m_smVisionInfo.g_arrColorImages[1].SaveImage(strOrientPath + "Template\\OriTemplate_Image1.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    m_smVisionInfo.g_arrColorImages[2].SaveImage(strOrientPath + "Template\\OriTemplate_Image2.bmp");
                if ((m_smVisionInfo.g_arrColorImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    m_smVisionInfo.g_arrColorImages[3].SaveImage(strOrientPath + "Template\\OriTemplate_Image3.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 4 && WantSaveImageAccordingMergeType(4))
                    m_smVisionInfo.g_arrColorImages[4].SaveImage(strOrientPath + "Template\\OriTemplate_Image4.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 5 && WantSaveImageAccordingMergeType(5))
                    m_smVisionInfo.g_arrColorImages[5].SaveImage(strOrientPath + "Template\\OriTemplate_Image5.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 6 && WantSaveImageAccordingMergeType(6))
                    m_smVisionInfo.g_arrColorImages[6].SaveImage(strOrientPath + "Template\\OriTemplate_Image6.bmp");
            }
            else
            {
                m_smVisionInfo.g_arrImages[0].SaveImage(strOrientPath + "Template\\OriTemplate.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    m_smVisionInfo.g_arrImages[1].SaveImage(strOrientPath + "Template\\OriTemplate_Image1.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    m_smVisionInfo.g_arrImages[2].SaveImage(strOrientPath + "Template\\OriTemplate_Image2.bmp");
                if ((m_smVisionInfo.g_arrImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    m_smVisionInfo.g_arrImages[3].SaveImage(strOrientPath + "Template\\OriTemplate_Image3.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 4 && WantSaveImageAccordingMergeType(4))
                    m_smVisionInfo.g_arrImages[4].SaveImage(strOrientPath + "Template\\OriTemplate_Image4.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 5 && WantSaveImageAccordingMergeType(5))
                    m_smVisionInfo.g_arrImages[5].SaveImage(strOrientPath + "Template\\OriTemplate_Image5.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 6 && WantSaveImageAccordingMergeType(6))
                    m_smVisionInfo.g_arrImages[6].SaveImage(strOrientPath + "Template\\OriTemplate_Image6.bmp");
            }

            m_smVisionInfo.g_arrPadOrientROIs[1].SaveImage(strOrientPath + "Template\\Template0.bmp");
            m_smVisionInfo.g_arrPadOrientROIs[2].SaveImage(strOrientPath + "Template\\SubTemplate0.bmp");

            m_objCopy.CopyAllImageFiles(strOrientPath + "Template\\", strPath + "New\\");

            #endregion

            //#region Save Orient Pattern Matching

            ////for (int v = 0; v < m_smVisionInfo.g_arrPadROIs.Count; v++)
            ////{
            ////    if (!m_smVisionInfo.g_arrPad[v].ref_blnSelected)
            ////        continue;
            ////    m_smVisionInfo.g_arrPadOrient[v].SetDontCareThreshold(1);
            ////    m_smVisionInfo.g_arrPadOrient[v].LearnPattern(m_smVisionInfo.g_arrPadROIs[v][0]);
            ////    m_smVisionInfo.g_arrPadOrient[v].SavePattern(strFolderPath + "Template\\Template" + v + ".mch");
            ////}
            //if (m_smVisionInfo.g_arrPadOrientROIs.Count > 1)
            //{
            //    m_smVisionInfo.g_objPadOrient.SetDontCareThreshold(1);
            //    m_smVisionInfo.g_objPadOrient.LearnPattern4Direction_SRM(m_smVisionInfo.g_arrPadOrientROIs[1]);
            //    m_smVisionInfo.g_objPadOrient.SavePattern4Direction(strOrientPath + "Template\\", "PadOrientTemplate0");
            //}
            //#endregion

            #region Save Unit Pattern Matching
            if (m_smVisionInfo.g_arrPadOrientROIs.Count > 1)
            {
                //2020-10-06 ZJYEOH : Need to rotate unit to original position when save pattern

                float CenterX = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX + (float)(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth) / 2;
                float CenterY = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY + (float)(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight) / 2;
                
                float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROITotalCenterX - CenterX) * Math.Cos((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)) - //m_smVisionInfo.g_objPadOrient.ref_intRotatedAngle
                                   ((m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROITotalCenterY - CenterY) * Math.Sin((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)));
                float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROITotalCenterX - CenterX) * Math.Sin((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)) +
                                    ((m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROITotalCenterY - CenterY) * Math.Cos((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)));

                m_smVisionInfo.g_objPadOrient.LearnPattern4Direction_SRM(m_smVisionInfo.g_arrPadOrientROIs[1], fXAfterRotated, fYAfterRotated);
                m_smVisionInfo.g_objPadOrient.SetFinalReduction(2);
                m_smVisionInfo.g_objPadOrient.SavePattern4Direction(strOrientPath + "Template\\", "PadUnitTemplate0");
            }
            #endregion

            #region Save Orient Pattern Matching

            if (m_smVisionInfo.g_arrPadOrientROIs.Count > 2)
            {
                int intCenterX2 = m_smVisionInfo.g_arrPadOrientROIs[2].ref_ROICenterX;
                int intCenterY2 = m_smVisionInfo.g_arrPadOrientROIs[2].ref_ROICenterY;
                int intCenterX1 = m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROIWidth / 2; //ref_ROICenterX // 2020-04-09 ZJYEOH : should use half of the width as sub orient ROI is attach on unit ROI
                int intCenterY1 = m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROIHeight / 2; //ref_ROICenterY // 2020-04-09 ZJYEOH : should use half of the height as sub orient ROI is attach on unit ROI

                m_smVisionInfo.g_objPadOrient.ref_intMatcherOffSetCenterX = intCenterX2 - intCenterX1;
                m_smVisionInfo.g_objPadOrient.ref_intMatcherOffSetCenterY = intCenterY2 - intCenterY1;
         
                m_smVisionInfo.g_objPadOrient.LearnSubPattern(m_smVisionInfo.g_arrPadOrientROIs[2]);
                m_smVisionInfo.g_objPadOrient.SetFinalReduction(2);
                m_smVisionInfo.g_objPadOrient.SaveSubPattern(strOrientPath + "Template\\PadOrientTemplate0.mch");


            }
            #endregion
            m_smVisionInfo.g_objPadOrient.ref_intRotatedAngle = m_intCurrentAngle;
            m_smVisionInfo.g_objPadOrient.SaveOrient(strOrientPath + "Settings.xml", false, "General", true);

            m_smVisionInfo.g_objPadOrient.SetCalibrationData(
                               m_smVisionInfo.g_fCalibPixelX,
                               m_smVisionInfo.g_fCalibPixelY, m_smCustomizeInfo.g_intUnitDisplay);

            LoadROISetting(strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrPad.Length);
            LoadROISetting(strOrientPath + "ROI.xml", m_smVisionInfo.g_arrPadOrientROIs);
            LoadROISetting(strFolderPath + "DontCareROI.xml", m_smVisionInfo.g_arrPadDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                        {
                            m_smVisionInfo.g_arrPadDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                        }
                    }

                }
            }

            this.Close();
            this.Dispose();
        }

        private void RotatePrecise()
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;

            ROI objROI = new ROI();
            List<ROI> arrROIs = new List<ROI>();

            if ((m_blnOrientTestDone && m_blnOrientFail) || m_intVisionType == 0)
            {
                if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrPadROIs.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI].Count > 0)
                        m_smVisionInfo.g_arrPadOrientROIs[0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
                }
            }
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                    objROI = m_smVisionInfo.g_arrPadOrientROIs[0];
                arrROIs = m_smVisionInfo.g_arrPadOrientROIs;
            }
            else
            {
                if (m_smVisionInfo.g_arrPadROIs.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI].Count > 0)
                    objROI = m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][0];
                if (m_smVisionInfo.g_arrPadROIs.Count > m_smVisionInfo.g_intSelectedROI)
                    arrROIs = m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI];
            }
            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);
            ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, 0);
            
            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                // 2021 03 13 - CCENG: need to copy from original image to rotated image first. If no, the rotated image is blank black color.
                objRotatedImage = m_smVisionInfo.g_arrRotatedImages[i];
                m_smVisionInfo.g_arrImages[i].CopyTo(ref objRotatedImage);

                ROI.RotateROI_Center(objROI, m_intCurrentAngle - m_intCurrentPreciseDeg, ref m_smVisionInfo.g_arrRotatedImages, i);
            }
            
            if (m_smVisionInfo.g_blnViewPackageImage)
                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            // After rotating the image, attach the rotated image into ROI again
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                        m_smVisionInfo.g_arrPadOrientROIs[0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                }
                else
                {
                    if (m_smVisionInfo.g_arrPadROIs.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI].Count > 0)
                        m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                }
            }

            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewGauge = false;  // 2019 06 21 - Gauge will measure according to original image only. Once rotate, gauge wont measure anymore, so not need to view Gauge after that.
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_PreciseAngle.Text = m_intCurrentPreciseDeg.ToString() + " deg";
            else
                lbl_PreciseAngle.Text = m_intCurrentPreciseDeg.ToString() + " 度";
            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
        }

        private void btn_OrientSaveContinue_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                  m_smVisionInfo.g_strVisionFolderName + "\\";

            string strOrientPath = strFolderPath + "Orient\\";
            string strPkgPath = strFolderPath + "Package\\";
            strFolderPath = strFolderPath + "Pad\\";

            if (m_intVisionType == 0)
            {
                if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                    if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                        if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                            m_smVisionInfo.g_arrPadOrientROIs[0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight);
            }
            else
            {
                if (m_smVisionInfo.g_arrPadOrientROIs.Count > 0)
                    if (m_smVisionInfo.g_arrPadROIs.Count > 0)
                        if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                            m_smVisionInfo.g_arrPadROIs[0][0].LoadROISetting(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight);
            }

            SaveROISetting(strFolderPath);
            SaveOrientROISetting(strOrientPath);

            //#region Save Orient Pattern Matching

            ////for (int v = 0; v < m_smVisionInfo.g_arrPadROIs.Count; v++)
            ////{
            ////    if (!m_smVisionInfo.g_arrPad[v].ref_blnSelected)
            ////        continue;
            ////    m_smVisionInfo.g_arrPadOrient[v].SetDontCareThreshold(1);
            ////    m_smVisionInfo.g_arrPadOrient[v].LearnPattern(m_smVisionInfo.g_arrPadROIs[v][0]);
            ////    m_smVisionInfo.g_arrPadOrient[v].SavePattern(strFolderPath + "Template\\Template" + v + ".mch");
            ////}
            //if (m_smVisionInfo.g_arrPadOrientROIs.Count > 1)
            //{
            //    m_smVisionInfo.g_objPadOrient.SetDontCareThreshold(1);
            //    m_smVisionInfo.g_objPadOrient.LearnPattern4Direction_SRM(m_smVisionInfo.g_arrPadOrientROIs[1]);
            //    m_smVisionInfo.g_objPadOrient.SavePattern4Direction(strOrientPath + "Template\\", "PadOrientTemplate0");
            //}
            //#endregion

            #region Save Unit Pattern Matching
            if (m_smVisionInfo.g_arrPadOrientROIs.Count > 1)
            {
                //2020-10-06 ZJYEOH : Need to rotate unit to original position when save pattern

                float CenterX = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX + (float)(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth) / 2;
                float CenterY = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY + (float)(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight) / 2;
                
                float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROITotalCenterX - CenterX) * Math.Cos((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)) - //m_smVisionInfo.g_objPadOrient.ref_intRotatedAngle
                                   ((m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROITotalCenterY - CenterY) * Math.Sin((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)));
                float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROITotalCenterX - CenterX) * Math.Sin((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)) +
                                    ((m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROITotalCenterY - CenterY) * Math.Cos((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)));

                m_smVisionInfo.g_objPadOrient.LearnPattern4Direction_SRM(m_smVisionInfo.g_arrPadOrientROIs[1], fXAfterRotated, fYAfterRotated);
                m_smVisionInfo.g_objPadOrient.SetFinalReduction(2);
                m_smVisionInfo.g_objPadOrient.SavePattern4Direction(strOrientPath + "Template\\", "PadUnitTemplate0");
            }
            #endregion

            #region Save Orient Pattern Matching

            if (m_smVisionInfo.g_arrPadOrientROIs.Count > 2)
            {
                int intCenterX2 = m_smVisionInfo.g_arrPadOrientROIs[2].ref_ROICenterX;
                int intCenterY2 = m_smVisionInfo.g_arrPadOrientROIs[2].ref_ROICenterY;
                int intCenterX1 = m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROIWidth / 2; //ref_ROICenterX // 2020-04-09 ZJYEOH : should use half of the width as sub orient ROI is attach on unit ROI
                int intCenterY1 = m_smVisionInfo.g_arrPadOrientROIs[1].ref_ROIHeight / 2; //ref_ROICenterY // 2020-04-09 ZJYEOH : should use half of the height as sub orient ROI is attach on unit ROI
                m_smVisionInfo.g_objPadOrient.ref_intMatcherOffSetCenterX = intCenterX2 - intCenterX1;
                m_smVisionInfo.g_objPadOrient.ref_intMatcherOffSetCenterY = intCenterY2 - intCenterY1;

                m_smVisionInfo.g_objPadOrient.LearnSubPattern(m_smVisionInfo.g_arrPadOrientROIs[2]);
                m_smVisionInfo.g_objPadOrient.SetFinalReduction(2);
                m_smVisionInfo.g_objPadOrient.SaveSubPattern(strOrientPath + "Template\\PadOrientTemplate0.mch");


            }
            #endregion
            m_smVisionInfo.g_objPadOrient.ref_intRotatedAngle = m_intCurrentAngle;
            m_smVisionInfo.g_objPadOrient.SaveOrient(strOrientPath + "Settings.xml", false, "General", true);

            LoadROISetting(strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrPad.Length);
            LoadROISetting(strOrientPath + "ROI.xml", m_smVisionInfo.g_arrPadOrientROIs);
            LoadROISetting(strFolderPath + "DontCareROI.xml", m_smVisionInfo.g_arrPadDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadDontCareROIs[i].Count; j++)
                        {
                            m_smVisionInfo.g_arrPadDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                        }
                    }

                }
            }

            m_blnOrientFail = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.g_intLearnStepNo = 2;
            SetupSteps(true);
        }

        private void btn_ResetPosition_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                {
                    if (i == 0)
                    {
                        int intPositionX = (m_smVisionInfo.g_intCameraResolutionWidth / 2) - (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intPositionY = (m_smVisionInfo.g_intCameraResolutionHeight / 2) - (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        m_smVisionInfo.g_arrPadROIs[i][0].LoadROISetting(intPositionX, intPositionY, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight);
                    }
                    else if (i == 1)
                    {
                        int intPositionX = (m_smVisionInfo.g_intCameraResolutionWidth / 2) - (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intPositionY = 10;
                        m_smVisionInfo.g_arrPadROIs[i][0].LoadROISetting(intPositionX, intPositionY, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight);
                    }
                    else if (i == 2)
                    {
                        int intPositionX = m_smVisionInfo.g_intCameraResolutionWidth - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - 10;
                        int intPositionY = (m_smVisionInfo.g_intCameraResolutionHeight / 2) - (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        m_smVisionInfo.g_arrPadROIs[i][0].LoadROISetting(intPositionX, intPositionY, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight);
                    }
                    else if (i == 3)
                    {
                        int intPositionX = (m_smVisionInfo.g_intCameraResolutionWidth / 2) - (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intPositionY = m_smVisionInfo.g_intCameraResolutionHeight - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - 10;
                        m_smVisionInfo.g_arrPadROIs[i][0].LoadROISetting(intPositionX, intPositionY, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight);
                    }
                    else
                    {
                        int intPositionX = 10;
                        int intPositionY = (m_smVisionInfo.g_intCameraResolutionHeight / 2) - (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        m_smVisionInfo.g_arrPadROIs[i][0].LoadROISetting(intPositionX, intPositionY, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight);
                    }
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ColorThreshold_Click(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_arrPadColorROIs[0][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[0]);
            //m_smVisionInfo.g_arrPadColorROIs[0][0].LoadROISetting(m_smVisionInfo.g_arrPadROIs[0][3].ref_ROIPositionX, m_smVisionInfo.g_arrPadROIs[0][3].ref_ROIPositionY, m_smVisionInfo.g_arrPadROIs[0][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[0][3].ref_ROIHeight);
            for (int i = 0; i < 3; i++)
            {
                m_smVisionInfo.g_intColorThreshold[i] = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intCopperColor[i];
                m_smVisionInfo.g_intColorTolerance[i] = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intCopperColorTolerance[i];
            }

            m_smVisionInfo.g_intColorFormat = 1;
            m_objColorMultiThresholdForm = new ColorMultiThresholdForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo);//, m_smVisionInfo.g_arrPadColorROIs[m_smVisionInfo.g_intSelectedROI][0]);
            m_objColorMultiThresholdForm.Location = new Point(720, 200);
            m_objColorMultiThresholdForm.Show();
            m_objColorMultiThresholdForm.TopMost = true;

        }

        private void btn_SaveColor_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            SaveControlSettings(strFolderPath + "Pad\\Template\\");
            SaveColorROISetting(strFolderPath + "Pad\\");

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                    continue;

                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                // 22-08-2019 ZJYEOH : Reset all inspection data so that drawing will not cause error when opening inspection result and learn pad on the same time
                m_smVisionInfo.g_arrPad[i].ResetInspectionData(false);

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

                m_smVisionInfo.g_arrPad[i].SaveColorPad(strFolderPath + "Pad\\" + "Template\\Template.xml",
                    false, strSectionName, false);

            }

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                         m_smVisionInfo.g_strVisionFolderName + "\\Pad\\";
            SaveColorDontCareROISetting(strPath);
            SaveColorDontCareSetting(strPath + "Template\\");

            LoadROISetting(strPath + "ColorDontCareROI.xml", m_smVisionInfo.g_arrPadColorDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                        {
                            for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                            {
                                m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                            }
                        }
                    }

                }
            }
            LoadColorROISetting(strFolderPath + "Pad\\" + "CROI.xml", m_smVisionInfo.g_arrPadColorROIs, m_smVisionInfo.g_arrPad.Length);

            LoadPadOffSetSetting(strFolderPath + "Calibration.xml");

            LoadPadSetting(strFolderPath);

            LoadMatcherFile(strFolderPath);

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
        private void SaveColorDontCareROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "ColorDontCareROI.xml", false);

            for (int t = 0; t < m_smVisionInfo.g_arrPadColorDontCareROIs.Count; t++)
            {
                if (t > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (!m_smVisionInfo.g_arrPad[t].ref_blnSelected)
                    continue;

                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[t].Count; j++)
                {
                    objFile.WriteElement1Value("Color Defect" + t + j, "");

                    for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[t][j].Count; k++)
                    {
                        objFile.WriteElement2Value("ROI" + k, "");

                        objFile.WriteElement3Value("Name", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_strROIName);
                        objFile.WriteElement3Value("Type", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intType);
                        objFile.WriteElement3Value("PositionX", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIPositionX);
                        objFile.WriteElement3Value("PositionY", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIPositionY);
                        objFile.WriteElement3Value("Width", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIWidth);
                        objFile.WriteElement3Value("Height", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIHeight);
                        objFile.WriteElement3Value("StartOffsetX", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intStartOffsetX);
                        objFile.WriteElement3Value("StartOffsetY", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intStartOffsetY);
                        m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[t][3]);

                    }
                }
            }
            objFile.WriteEndElement();


        }
        private void SaveColorDontCareSetting(string strFolderPath)
        {
            ImageDrawing objImage = new ImageDrawing();
            ImageDrawing objFinalImage = new ImageDrawing();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {

                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                //Check is the ROI for drawing dont care changed
                //m_smVisionInfo.g_arrPolygon_Pad[i][0].CheckDontCarePosition(m_smVisionInfo.g_arrPadROIs[i][2], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                {
                    m_smVisionInfo.g_objBlackImage.CopyTo(objFinalImage);

                    for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                    {
                        //Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        if (m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ref_intFormMode != 2)
                            Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        else
                            Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                        ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);
                    }
                    objFinalImage.SaveImage(strFolderPath + "ColorDontCareImage" + i.ToString() + "_" + j.ToString() + ".bmp");
                }


            }
            objImage.Dispose();
            objFinalImage.Dispose();
            Polygon.SavePolygon(strFolderPath + "ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PadColor);
        }

        private void chk_ResetPitchGap_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (chk_ResetPitchGap.Checked)
                {
                    if (m_smVisionInfo.g_arrPad[0].ref_blnWantUseClosestSizeDefineTolerance)
                    {
                        m_smVisionInfo.g_arrPad[i].BuildPadPitchGapLink(false);
                    }

                    m_smVisionInfo.g_arrPad[i].AutoDefineGroupNoToTemplate();

                    //m_smVisionInfo.g_arrPad[tabCtrl_PadPG.SelectedIndex].UpdateNewGroupNoListToTemplate();

                    AutoDefinePitchGap(i);

                    //switch (tabCtrl_PadPG.SelectedIndex)
                    //{
                    //    case 0:
                    //        ReadPadPitchGapToGrid(0, dgd_PitchGapSetting);
                    //        break;
                    //    case 1:
                    //        ReadPadPitchGapToGrid(1, dgd_TopPGSetting);
                    //        break;
                    //    case 2:
                    //        ReadPadPitchGapToGrid(2, dgd_RightPGSetting);
                    //        break;
                    //    case 3:
                    //        ReadPadPitchGapToGrid(3, dgd_BottomPGSetting);
                    //        break;
                    //    case 4:
                    //        ReadPadPitchGapToGrid(4, dgd_LeftPGSetting);
                    //        break;
                    //}

                    ReadPadPitchGapToGrid(0, dgd_PitchGapSetting);
                    ReadPadPitchGapToGrid(1, dgd_TopPGSetting);
                    ReadPadPitchGapToGrid(2, dgd_RightPGSetting);
                    ReadPadPitchGapToGrid(3, dgd_BottomPGSetting);
                    ReadPadPitchGapToGrid(4, dgd_LeftPGSetting);

                }
                else
                {
                    m_smVisionInfo.g_arrPad[i].AutoDefineGroupNoToTemplate();
                    
                    m_smVisionInfo.g_arrPad[i].LoadGroupNoTemporary();
                    m_smVisionInfo.g_arrPad[i].LoadPitchGapLinkTemporary();

                    m_smVisionInfo.g_arrPad[i].UpdateNewGroupNoListToTemplate();

                    //AutoDefinePitchGap_NoReset(i);

                    //switch (tabCtrl_PadPG.SelectedIndex)
                    //{
                    //    case 0:
                    //        ReadPadPitchGapToGrid(0, dgd_PitchGapSetting);
                    //        break;
                    //    case 1:
                    //        ReadPadPitchGapToGrid(1, dgd_TopPGSetting);
                    //        break;
                    //    case 2:
                    //        ReadPadPitchGapToGrid(2, dgd_RightPGSetting);
                    //        break;
                    //    case 3:
                    //        ReadPadPitchGapToGrid(3, dgd_BottomPGSetting);
                    //        break;
                    //    case 4:
                    //        ReadPadPitchGapToGrid(4, dgd_LeftPGSetting);
                    //        break;
                    //}

                    ReadPadPitchGapToGrid(0, dgd_PitchGapSetting);
                    ReadPadPitchGapToGrid(1, dgd_TopPGSetting);
                    ReadPadPitchGapToGrid(2, dgd_RightPGSetting);
                    ReadPadPitchGapToGrid(3, dgd_BottomPGSetting);
                    ReadPadPitchGapToGrid(4, dgd_LeftPGSetting);

                }

            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ResetPadCustomize_Click(object sender, EventArgs e)
        {
            string[] strROIName = new string[m_smVisionInfo.g_arrPadROIs.Count];

            //Define ROI name for display in message box
            for (int y = 0; y < m_smVisionInfo.g_arrPadROIs.Count; y++)
            {
                if (y > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                switch (y)
                {
                    case 0:
                        strROIName[y] = "Center Pads";
                        break;
                    case 1:
                        strROIName[y] = "Up Pads";
                        break;
                    case 2:
                        strROIName[y] = "Right Pads";
                        break;
                    case 3:
                        strROIName[y] = "Down Pads";
                        break;
                    case 4:
                        strROIName[y] = "Left Pads";
                        break;
                }
            }
            
            if (!chk_ResetPadCustomize.Checked)
            {
                if (m_smVisionInfo.g_arrPad[0].ref_blnWantUseClosestSizeDefineTolerance)
                {
                    if (!DefinePadToleranceUsingClosestSizeMethod(strROIName))
                    {
                    }

                    ReadAllPadTemplateDataToGrid_ClosestSizeMethod();
                }
                else
                {

                    if (!DefinePadToleranceUsingDefaultSetting(strROIName))
                    {
                        return;
                    }

                    ReadAllPadTemplateDataToGrid_DefaultMethod();
                }

                string strFolderPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    m_smVisionInfo.g_arrPad[i].ResetPercentage();

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
                    chk_ResetPadCustomize.Enabled = false;
                    m_smVisionInfo.g_arrPad[i].LoadPercentageTemporary_ByPosition(strFolderPath + "Pad\\Template\\Template.xml", strSectionName);
                    chk_ResetPadCustomize.Enabled = true;
                    m_smVisionInfo.g_arrPad[i].RearrangeBlobs(); //2021-08-05 ZJYEOH : Remove those pad not in previous recipe
                }
                
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    m_smVisionInfo.g_arrPad[i].LoadTemporaryBlobPadsForReset();
                }
               
                if (m_smVisionInfo.g_arrPad[0].ref_blnWantUseClosestSizeDefineTolerance)
                {
                    if (!DefinePadToleranceUsingClosestSizeMethod(strROIName))
                    {
                    }

                    ReadAllPadTemplateDataToGrid_ClosestSizeMethod();
                }
                else
                {

                    if (!DefinePadToleranceUsingDefaultSetting(strROIName))
                    {
                        return;
                    }

                    ReadAllPadTemplateDataToGrid_DefaultMethod();
                }

                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;
                    
                    m_smVisionInfo.g_arrPad[i].ResetPercentage();

                    //m_smVisionInfo.g_arrPad[i].ref_blnTestAllPad = true;
                }

                //m_smVisionInfo.AT_VM_OfflineTestAllPad = true;
                //TriggerOfflineTest(); // Temporary hide until solve the problem when cant find point 

                //WaitEventDone(ref m_smVisionInfo.PR_MN_UpdateInfo, true, "WaitTestDone");

                //if (m_smVisionInfo.PR_MN_UpdateInfo)
                //{
                //    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                //    {
                //        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                //            break;
                        
                //        if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                //            continue;

                //        m_smVisionInfo.g_arrPad[i].ResetPointGaugeInwardPercentage();
                //    }

                //    m_smVisionInfo.PR_MN_UpdateInfo = false;
                //}
            }
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void TriggerOfflineTest()
        {
            m_smVisionInfo.g_objPositioning.ref_blnDrawPHResult = false;
            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
            m_smVisionInfo.g_blnViewPointGauge = false;

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            m_smVisionInfo.g_blnViewPHImage = false;
            m_smVisionInfo.g_blnCheckPH = false;

            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                    m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }
        private bool WaitEventDone(ref bool bTriggerEvent, bool bBreakResult, string strTrackName)
        {
            HiPerfTimer timesout = new HiPerfTimer();
            timesout.Start();

            while (true)
            {
                if (bTriggerEvent == bBreakResult)
                {
                    return true;
                }

                if (timesout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 11 - " + strTrackName);
                    break;
                }

                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }

            return false;
        }
        private void btn_ClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg++;
            RotatePrecise();
        }

        private void cbo_SidePadWidthLengthMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (!m_blnStepPadDone)
                return;

            for (int i = 1; i<m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_intPadWidthLengthMode = cbo_SidePadWidthLengthMode.SelectedIndex;
            }
        
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_CounterClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg--;
            RotatePrecise();
        }

        private bool OrientInspection()
        {
            // make sure template learn
            if (m_smVisionInfo.g_arrPadOrientROIs.Count < 3)
            {
                m_smVisionInfo.g_strErrorMessage += "*Orient : No Orient Template Found";
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                return false;
            }

            // reset all inspection data
            m_smVisionInfo.g_objPadOrient.ResetInspectionData();

            //m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);

            int intMatchCount = 0;
            m_smVisionInfo.g_intOrientResult[0] = -1;  // 0:0deg, 1:90deg, 2:180deg, 3:-90, 4:Fail

            int intAngle;
            m_smVisionInfo.g_arrPadOrientROIs[0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            m_smVisionInfo.g_arrPadOrientROIs[1].AttachImage(m_smVisionInfo.g_arrPadOrientROIs[0]);
            m_smVisionInfo.g_arrPadOrientROIs[2].AttachImage(m_smVisionInfo.g_arrPadOrientROIs[1]);

            intAngle = m_smVisionInfo.g_objPadOrient.DoOrientationInspection_WithSubMatcher4(m_smVisionInfo.g_arrRotatedImages[0],
                        m_smVisionInfo.g_arrPadOrientROIs[0], m_smVisionInfo.g_arrPadOrientROIs[1], m_smVisionInfo.g_arrPadOrientROIs[2], 1);  // 2020 01 08 - CCENG: Change from 2 to 1 bcos for unit 0603 pattern, angle not enough precise if FinalReduction is 2.

            m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_objPadOrient.GetMinScore();

            lbl_OrientScore.Text = m_smVisionInfo.g_fOrientScore[0].ToString();

            m_smVisionInfo.g_intOrientResult[0] = intAngle;


            Orient objOrient = m_smVisionInfo.g_objPadOrient;

            m_smVisionInfo.g_fOrientCenterX[0] = objOrient.ref_fObjectX;
            m_smVisionInfo.g_fOrientCenterY[0] = objOrient.ref_fObjectY;
            m_smVisionInfo.g_fSubOrientCenterX[0] = objOrient.ref_fSubObjectX;
            m_smVisionInfo.g_fSubOrientCenterY[0] = objOrient.ref_fSubObjectY;
            m_smVisionInfo.g_fOrientScore[0] = objOrient.GetMinScore();
            m_smVisionInfo.g_fOrientAngle[0] = objOrient.ref_fDegAngleResult;

            if (m_smVisionInfo.g_intOrientResult[0] < 4)
            {

                switch (m_smVisionInfo.g_intOrientResult[0])
                {
                    default:
                    case 0:
                        lbl_OrientationAngle.Text = "0";
                        m_intCurrentAngle = 0;
                        break;
                    case 1:
                        lbl_OrientationAngle.Text = "90";
                        m_intCurrentAngle = 90;
                        break;
                    case 2:
                        lbl_OrientationAngle.Text = "180";
                        m_intCurrentAngle = 180;
                        break;
                    case 3:
                        lbl_OrientationAngle.Text = "-90";
                        m_intCurrentAngle = 270;
                        break;
                }

            }
            else
            {
                m_smVisionInfo.g_strErrorMessage += "*Orient : Match Template Fail";
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                lbl_OrientationAngle.Text = "Fail";
                return false;
            }

            ROI objRotatedROI = new ROI();
         
                // Calculate total angle 
               float fTotalRotateAngle = Convert.ToInt32(lbl_OrientationAngle.Text) + m_smVisionInfo.g_objPadOrient.ref_fDegAngleResult;
                m_intCurrentPreciseDeg = -(int)(Math.Round(m_smVisionInfo.g_objPadOrient.ref_fDegAngleResult));
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_PreciseAngle.Text = m_intCurrentPreciseDeg.ToString() + " deg";
            else
                lbl_PreciseAngle.Text = m_intCurrentPreciseDeg.ToString() + " 度";

            // Get RotateROI where the ROI center point == Unit Center Point
            objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                float fSizeX, fSizeY;
                fSizeX = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth - m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth % 2; // why %2? To get "even" number
                fSizeY = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight - m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight % 2;

                // 2019 09 06 - CCENG: If no package size center point, then use Orient Search ROI Center Point
                objRotatedROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROITotalCenterX - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                             (int)Math.Round(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROITotalCenterY - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                             (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                             (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));

            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, 0);
            //m_smVisionInfo.g_arrRotatedImages[0].SaveImage("D:\\img.bmp");

            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[i]);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[i], objRotatedROI, fTotalRotateAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, i);
            }

            m_smVisionInfo.g_blnViewRotatedImage = true;
            return true;
        }

        private int CheckOutOfSearchROI()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX)
                    return i;

                if (m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIWidth > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)
                    return i;

                if (m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY < m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY)
                    return i;

                if (m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIHeight > m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)
                    return i;
            }
            return -1;
        }
    }
}