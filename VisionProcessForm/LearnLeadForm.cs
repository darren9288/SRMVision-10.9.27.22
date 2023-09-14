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

namespace VisionProcessForm
{
    public partial class LearnLeadForm : Form
    {

        #region Member Variables
        private string[] arrROIDirection = new string[5] { "Center ROI", "Top ROI", "Right ROI", "Bottom ROI", "Left ROI" };
        private float m_fLeadPatternScore = 0;
        private float m_fLeadPatternAngle = 0;
        private bool m_blnWantOCRMark = false;
        private bool m_blnWantOCVMark = false;
        private bool m_blnWantOrient = false;
        private bool m_blnWantColor = false;
        private bool m_blnWantPackage = false;
        private int m_intCurrentPreciseDeg = 0;
        private string m_strFolderPath = "";
        private int m_intTotalTemplateBlobNo;
        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private int m_intDisplayStepNo = 1;
        private string m_strSelectedRecipe;
        private int m_intCurrentAngle = 0;
        private int m_intCurrentAngle2 = 0;
        private bool m_blnKeepPrevObject = false;
        private bool m_blnIdentityLeadsDone = false;
        private string m_strPosition = "";
        private List<ImageDrawing> m_arrCurrentImage = new List<ImageDrawing>();
        private ImageDrawing m_ImageWithMasking = new ImageDrawing(true);

        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private int m_intPitchSelectedRowIndex = 0;
        private DataGridView m_dgdViewPG = new DataGridView();
        private string[] m_strPGRealColName = new string[4];
        private string[] m_strPGColName = { "column_FromLeadNo", "column_ToLeadNo", "column_MinSetPitch", "column_MaxSetPitch" };
        
        LeadLineProfileForm m_objLeadLineProfileForm;
        private AdvancedRectGaugeM4LForm m_objAdvancedRectGaugeForm = null;
        #endregion

        public LearnLeadForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;

            if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWantOCVMark = true;
            }
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWantOrient = true;
            }
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWantPackage = true;
            }

            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWantColor = true;
            }
            
            InitVariable();
            DisableField2();
            CustomizeGUI();
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\";

            m_blnInitDone = true;
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
            string strChild2 = "";

            strChild2 = "Learn Template";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Previous.Enabled = false;
                btn_Next.Enabled = false;
            }
            strChild1 = "Learn Page";
            strChild2 = "Save Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
                btn_ROISaveClose.Enabled = false;
           
            }
            if (!File.Exists(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\OriTemplate.bmp"))
                chk_UsePreTemplateImage.Enabled = false;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Lead";
            string strChild2 = "Learn Page";
            string strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_Save.Enabled = false;
                btn_ROISaveClose.Enabled = false;

                btn_Save.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_ROISaveClose.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Use Previous Template Image";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                chk_UsePreTemplateImage.Enabled = false;

                chk_UsePreTemplateImage.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Rotate Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_Rotate.Enabled = false;

                gb_Rotate.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Gauge Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_GaugeAdvanceSetting.Enabled = false;
                chk_ShowDraggingBox.Enabled = false;
                chk_ShowSamplePoints.Enabled = false;

                btn_GaugeAdvanceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_ShowDraggingBox.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_ShowSamplePoints.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Select Direction";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                group_SelectDirection.Enabled = false;

                group_SelectDirection.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Lead Count";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_LeadCount.Enabled = false;

                gb_LeadCount.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Dont Care Area Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_UndoROI.Enabled = false;

                btn_UndoROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Lead Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_Threshold.Enabled = false;

                gb_Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Object Selection";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_ObjectSelection.Enabled = false;

                gb_ObjectSelection.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Lead Labeling";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_LeadLabeling.Enabled = false;

                gb_LeadLabeling.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Lead Line Profile";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_LineProfileGaugeSetting.Enabled = false;

                gb_LineProfileGaugeSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Lead Inward Offset Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_LeadInwardOffsetSetting.Enabled = false;

                gb_LeadInwardOffsetSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Package To Base Tolerance";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                group_PkgToBaseToleranceSetting.Enabled = false;

                group_PkgToBaseToleranceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Pitch Gap Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                dgd_TopPGSetting.Enabled = false;
                dgd_RightPGSetting.Enabled = false;
                dgd_BottomPGSetting.Enabled = false;
                dgd_LeftPGSetting.Enabled = false;
                btn_AddPitch.Enabled = false;
                btn_DeletePitch.Enabled = false;

                dgd_TopPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                dgd_RightPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                dgd_BottomPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                dgd_LeftPGSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_AddPitch.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_DeletePitch.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Base Lead Threshold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_Threshold_BaseLead.Enabled = false;

                gb_Threshold_BaseLead.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Base Lead Object Selection";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_ObjectSelection_BaseLead.Enabled = false;

                gb_ObjectSelection_BaseLead.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
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
                    Child1 = "Orient";
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
            }

            return 1;
        }
        private void AddSearchROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            if (arrROIs[0].Count == 0)
            {
                if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_arrOrientROIs.Count > 0 && m_smVisionInfo.g_arrOrientROIs[0].Count > 0)
                {
                    objROI = new ROI("Search ROI", 1);
                    objROI.LoadROISetting(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY,
                         m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);
                }
                else
                {
                    objROI = new ROI("Search ROI", 1);
                    int intPositionX = (640 / 2) - 100;
                    int intPositionY = (480 / 2) - 100;
                    objROI.LoadROISetting(intPositionX, intPositionY, 200, 200);
                }

                objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

                arrROIs[0].Add(objROI);
            }
            else
            {
                arrROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            }
        }

        private void AddLeadROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 1; i < arrROIs.Count; i++)
            {
                if (arrROIs[i].Count == 0)
                {
                    if (i == 1)
                    {
                        objROI = new ROI("T", 1);
                        int intPostX = (640 / 2) - 65;
                        int intPostY = (480 / 2) - 180;
                        objROI.LoadROISetting(intPostX, intPostY, 150, 50);
                    }
                    else if (i == 2)
                    {
                        objROI = new ROI("R", 1);
                        int intPostXR = (640 / 2) + 120;
                        int intPostYR = (480 / 2) - 70;
                        objROI.LoadROISetting(intPostXR, intPostYR, 50, 150);
                    }
                    else if (i == 3)
                    {
                        objROI = new ROI("B", 1);
                        int intPostXB = (640 / 2) - 65;
                        int intPostYB = (480 / 2) + 120;
                        objROI.LoadROISetting(intPostXB, intPostYB, 150, 50);
                    }
                    else
                    {
                        objROI = new ROI("L", 1);
                        int intPostXL = (640 / 2) - 170;
                        int intPostYL = (480 / 2) - 70;
                        objROI.LoadROISetting(intPostXL, intPostYL, 50, 150);
                    }

                    objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

                    arrROIs[i].Add(objROI);
                }
                else
                {
                    if (m_fLeadPatternScore > 0)
                    {
                        if (m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Top == 0 &&
                        m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Right == 0 &&
                        m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Bottom == 0 &&
                        m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Left == 0)
                        {
                            arrROIs[i][0].LoadROISetting(
                               (int)Math.Round(m_smVisionInfo.g_arrLead[i].GetResultCenterPoint_UnitMatcher().X -
                               ((float)m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth / 2) + m_smVisionInfo.g_arrLead[i].ref_fPatternROIOffsetX, 0, MidpointRounding.AwayFromZero),
                               (int)Math.Round(m_smVisionInfo.g_arrLead[i].GetResultCenterPoint_UnitMatcher().Y -
                               ((float)m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight / 2) + m_smVisionInfo.g_arrLead[i].ref_fPatternROIOffsetY, 0, MidpointRounding.AwayFromZero),
                               m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth,
                               m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight);
                        }
                        else
                        {
                            arrROIs[i][0].LoadROISetting(
                         (int)Math.Round(m_smVisionInfo.g_arrLead[i].GetResultCenterPoint_UnitMatcher().X -
                         ((float)m_smVisionInfo.g_arrLead[i].GetPatternSize_UnitMatcher().Width / 2) - m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Left, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(m_smVisionInfo.g_arrLead[i].GetResultCenterPoint_UnitMatcher().Y -
                         ((float)m_smVisionInfo.g_arrLead[i].GetPatternSize_UnitMatcher().Height / 2) - m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Top, 0, MidpointRounding.AwayFromZero),
                         m_smVisionInfo.g_arrLead[i].GetPatternSize_UnitMatcher().Width + m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Left + m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Right,
                         m_smVisionInfo.g_arrLead[i].GetPatternSize_UnitMatcher().Height + m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Top + m_smVisionInfo.g_arrLead[i].ref_intLeadROITolerance_Bottom);
                        }

                    }

                    if (m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        arrROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
                }
            }
        }

        private void CustomizeGUI()
        {
            cbo_SelectROI_Lead.Items.Clear();
            bool blnfirstROI = true;
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                {
                    m_smVisionInfo.g_arrLeadROIs[i][0].ClearDragHandle();
                }

                if (i == 1)
                    cbo_SelectROI_Lead.Items.Add("Top ROI");
                else if (i == 2)
                    cbo_SelectROI_Lead.Items.Add("Right ROI");
                else if (i == 3)
                    cbo_SelectROI_Lead.Items.Add("Bottom ROI");
                else if (i == 4)
                    cbo_SelectROI_Lead.Items.Add("Left ROI");

                if (blnfirstROI)
                {
                    blnfirstROI = false;

                    m_smVisionInfo.g_intSelectedROI = i;    // 2020 09 30 - Although most of the process in g_arrLead[0], but lead are separated to different direction group, and each direction have different threshold setting, min area etc.

                    if (cbo_SelectROI_Lead.Items.Contains(arrROIDirection[m_smVisionInfo.g_intSelectedROI]))
                    {
                        cbo_SelectROI_Lead.SelectedItem = arrROIDirection[m_smVisionInfo.g_intSelectedROI];
                        SelectLeadROI(m_smVisionInfo.g_intSelectedROI);
                    }
                }
            }
           
            picLeadROI1.Image = ils_ImageListTree.Images[2];
            chk_WantShowPocketDontCareArea.Checked = true;
            m_smVisionInfo.g_blnWantShowPocketDontCareArea = chk_WantShowPocketDontCareArea.Checked;
            //if (m_smVisionInfo.g_arrLead[0].ref_intLeadDirection == 0)
            //{
            //    radioBtn_Horizontal.Checked = true;
            //    picLeadROI2.Image = ils_ImageListTree.Images[3];
            //}
            //else
            //{
            //    radioBtn_Vertical.Checked = true;
            //    picLeadROI2.Image = ils_ImageListTree.Images[4];
            //}

            //Global setting (same for all lead)
            m_smVisionInfo.g_blnCutMode = radioBtn_CutObj.Checked;
            txt_MinArea.Text = m_smVisionInfo.g_arrLead[0].ref_intFilterMinArea.ToString();
            txt_MinArea_BaseLead.Text = m_smVisionInfo.g_arrLead[0].ref_intFilterMinArea_BaseLead.ToString();

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                int BaseValue = m_smVisionInfo.g_arrLead[i].GetBaseInwardOffset(0);
                int TipValue = m_smVisionInfo.g_arrLead[i].GetTipInwardOffset(0);

                if (BaseValue >= 0)
                    txt_BaseOffset.Text = BaseValue.ToString();
                if (TipValue >= 0)
                    txt_TipOffset.Text = TipValue.ToString();
            }
   
            //m_smVisionInfo.g_blnUsedPreTemplate = true;

            if (m_smVisionInfo.g_arrLead[0].ref_blnClockWise)
                cbo_LeadLabelDirection.SelectedIndex = 0;
            else
                cbo_LeadLabelDirection.SelectedIndex = 1;

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                switch (i)
                {
                    case 1:
                        txt_LeadNumber_Top.Text = m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead.ToString();
                        break;
                    case 2:
                        txt_LeadNumber_Right.Text = m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead.ToString();
                        break;
                    case 3:
                        txt_LeadNumber_Bottom.Text = m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead.ToString();
                        break;
                    case 4:
                        txt_LeadNumber_Left.Text = m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead.ToString();
                        break;
                }
            }

            m_smVisionInfo.g_arrLead[0].ref_blnSelected = false;

            UpdateReferenceCornerGUI();

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_SetToAllLeads.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllLead", false));


            if ((((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2) && m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
            {
                txt_PkgToBaseTolerance_Top.Text = m_smVisionInfo.g_arrLead[1].ref_intPkgToBaseTolerance_Top.ToString();
                txt_PkgToBaseTolerance_Bottom.Text = m_smVisionInfo.g_arrLead[3].ref_intPkgToBaseTolerance_Bottom.ToString();
                txt_PkgToBaseTolerance_Left.Text = m_smVisionInfo.g_arrLead[4].ref_intPkgToBaseTolerance_Left.ToString();
                txt_PkgToBaseTolerance_Right.Text = m_smVisionInfo.g_arrLead[2].ref_intPkgToBaseTolerance_Right.ToString();

                if (m_smVisionInfo.g_arrPackageGaugeM4L != null && m_smVisionInfo.g_arrPackageGaugeM4L.Count > 0)
                    txt_ImageGain1.Value = Convert.ToDecimal(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);

                srmGroupBox2.Visible = false;
                srmLabel3.Visible = false;
                txt_RotateAngle.Visible = false;
                btn_RotateAngle.Visible = false;

                //srmLabel5.Visible = false;
                //txt_BaseOffset.Visible = false;
            }
            else
            {
                group_PkgToBaseToleranceSetting.Visible = false;
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
            {
                chk_ShowDraggingBox.Checked = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_blnDrawDraggingBox;
                chk_ShowSamplePoints.Checked = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_blnDrawSamplingPoint;
            }
        }

        private void UpdateReferenceCornerGUI()
        {
            cbo_ReferenceCorner.Items.Clear();

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                switch (i)
                {
                    case 1:
                        cbo_ReferenceCorner.Items.Add("Top");
                        break;
                    case 2:
                        cbo_ReferenceCorner.Items.Add("Right");
                        break;
                    case 3:
                        cbo_ReferenceCorner.Items.Add("Bottom");
                        break;
                    case 4:
                        cbo_ReferenceCorner.Items.Add("Left");
                        break;
                }
            }

            switch (m_smVisionInfo.g_arrLead[0].ref_intFirstLead)
            {
                case 1:
                    if (cbo_ReferenceCorner.Items.Contains("Top"))
                        cbo_ReferenceCorner.SelectedItem = "Top";
                    else
                        cbo_ReferenceCorner.SelectedIndex = 0;
                    break;
                case 2:
                    if (cbo_ReferenceCorner.Items.Contains("Right"))
                        cbo_ReferenceCorner.SelectedItem = "Right";
                    else
                        cbo_ReferenceCorner.SelectedIndex = 0;

                    break;
                case 3:
                    if (cbo_ReferenceCorner.Items.Contains("Bottom"))
                        cbo_ReferenceCorner.SelectedItem = "Bottom";
                    else
                        cbo_ReferenceCorner.SelectedIndex = 0;
                    break;
                case 4:
                    if (cbo_ReferenceCorner.Items.Contains("Left"))
                        cbo_ReferenceCorner.SelectedItem = "Left";
                    else
                        cbo_ReferenceCorner.SelectedIndex = 0;
                    break;
            }

            m_strPosition = cbo_ReferenceCorner.SelectedItem.ToString();
        }

        private void LoadLeadSetting(string strPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].SetCalibrationData(
                              m_smVisionInfo.g_fCalibPixelX,
                              m_smVisionInfo.g_fCalibPixelY,
                              m_smVisionInfo.g_fCalibOffSetX,
                              m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);

                // Load Lead Template Setting
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

                m_smVisionInfo.g_arrLead[i].LoadLead(strPath + "Template\\Template.xml", strSectionName, m_smVisionInfo.g_arrImages.Count);

                m_smVisionInfo.g_arrLead[i].LoadLeadTemplateImage(strPath + "Template\\", i);

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

        private void SaveLeadSetting(string strFolderPath)
        {
            SaveROISetting(strFolderPath);
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Lead\\";
            

            if (File.Exists(strFolderPath + "Template\\OriTemplate.bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Lead", "OriTemplate.bmp", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Lead", "", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "Old\\");
           

            // Save Learn Actual Image
            m_smVisionInfo.g_arrImages[0].SaveImage(strFolderPath + "Template\\OriTemplate.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 1)
                m_smVisionInfo.g_arrImages[1].SaveImage(strFolderPath + "Template\\OriTemplate_Image1.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 2)
                m_smVisionInfo.g_arrImages[2].SaveImage(strFolderPath + "Template\\OriTemplate_Image2.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 3)
                m_smVisionInfo.g_arrImages[3].SaveImage(strFolderPath + "Template\\OriTemplate_Image3.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 4)
                m_smVisionInfo.g_arrImages[4].SaveImage(strFolderPath + "Template\\OriTemplate_Image4.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 5)
                m_smVisionInfo.g_arrImages[5].SaveImage(strFolderPath + "Template\\OriTemplate_Image5.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 6)
                m_smVisionInfo.g_arrImages[6].SaveImage(strFolderPath + "Template\\OriTemplate_Image6.bmp");

            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "New\\");

            float fTotalX = 0, fTotalY = 0;
            int intCount = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;
                ImageDrawing objRotatedImage = new ImageDrawing(true);
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref objRotatedImage);

                ImageDrawing objRotatedImage_BaseLead = new ImageDrawing(true);
                if (m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                {
                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo].CopyTo(ref objRotatedImage_BaseLead);
                }
                // Save Learn Template Search ROI image, unit image
                m_smVisionInfo.g_arrLead[i].SaveLeadTemplateImage(strFolderPath + "Template\\",
                                                                objRotatedImage, objRotatedImage_BaseLead,
                                                                m_smVisionInfo.g_arrLeadROIs[i], i);
                m_smVisionInfo.g_arrLead[i].LoadUnitPattern(strFolderPath + "Template\\UnitMatcher" + i.ToString() + ".mch");
                m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

                m_smVisionInfo.g_arrLead[i].FindUnitUsingPRS(m_smVisionInfo.g_arrLeadROIs[0][0], 0, false, i, m_smVisionInfo.g_arrLeadROIs[i][0]);
                fTotalX += m_smVisionInfo.g_arrLead[i].GetResultCenterPoint_UnitMatcher().X;
                fTotalY += m_smVisionInfo.g_arrLead[i].GetResultCenterPoint_UnitMatcher().Y;
                intCount++;

                objRotatedImage.Dispose();
                objRotatedImage_BaseLead.Dispose();
            }



            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
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

                m_smVisionInfo.g_arrLead[i].ref_fUnitAverageCenterX = fTotalX / intCount;
                m_smVisionInfo.g_arrLead[i].ref_fUnitAverageCenterY = fTotalY / intCount;
                m_smVisionInfo.g_arrLead[i].BuildLeadDistance_PatternMatch(m_smVisionInfo.g_arrLeadROIs[i][0]);
                if (m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                {
                    m_smVisionInfo.g_arrLead[i].BuildLeadDistance_PatternMatch_BaseLead(m_smVisionInfo.g_arrLeadROIs[i][0]);
                }
                //
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrLead[i].SaveLead(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead ROI", m_smProductionInfo.g_strLotID);
                
                //ImageDrawing objRotatedImage = new ImageDrawing(true);
                //    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref objRotatedImage);

                //// Save Learn Template Search ROI image, unit image
                //m_smVisionInfo.g_arrLead[i].SaveLeadTemplateImage(strFolderPath + "Template\\",
                //                                                objRotatedImage,
                //                                                m_smVisionInfo.g_arrLeadROIs[i], i);


            }
            
        }

        private void SaveROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "ROI.xml", false);

            ROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrLeadROIs.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrLeadROIs[t][j];
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
        }

        private void ReadAllLeadTemplateDataToGrid_DefaultMethod()
        {
            //This parameter is used to decide the numbering of lead
            //Clockwise top > right > bottom > left
            //AntiClockwise top > left > bottom > right
            int intLeadNo = 1;
            int intIndex = m_smVisionInfo.g_arrLead[0].ref_intFirstLead;

            //switch (m_strPosition)
            //{
            //    case "Top":
            //        intIndex = 1;
            //        break;
            //    case "Right":
            //        intIndex = 2;
            //        break;
            //    case "Bottom":
            //        intIndex = 3;
            //        break;
            //    case "Left":
            //        intIndex = 4;
            //        break;
            //}

            //Clockwise
            if (cbo_LeadLabelDirection.SelectedIndex == 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length - 1; i++)
                {
                    if (!m_smVisionInfo.g_arrLead[intIndex].ref_blnSelected)
                    {
                        if (intIndex == 4)
                            intIndex = 1;
                        else
                            intIndex++;
                        continue;
                    }

                    //Not use prev setting but use default setting
                    if (!m_smVisionInfo.g_blnUsedPreTemplate)
                    {
                        m_smVisionInfo.g_arrLead[intIndex].SetCalibrationData(
                                                        m_smVisionInfo.g_fCalibPixelX,
                                                        m_smVisionInfo.g_fCalibPixelY,
                                                        m_smVisionInfo.g_fCalibOffSetX,
                                                        m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);

                        m_smVisionInfo.g_arrLead[intIndex].AnalyingLead_DefaultToleranceMethod(intIndex, true, ref intLeadNo);
                        m_smVisionInfo.g_arrLead[intIndex].BuildLeadsParameter(intIndex, m_smVisionInfo.g_arrLeadROIs[intIndex][0]);
                        m_smVisionInfo.g_arrLead[intIndex].DefineLeadVariance(m_smVisionInfo.g_arrLeadROIs[intIndex][0]);
                        //m_smVisionInfo.g_arrLead[intIndex].DefineTolerance(true);

                        m_smVisionInfo.g_arrLead[intIndex].DefineTolerance2();  // use back previous tolerance setting
                    }

                    if (intIndex == 4)
                        intIndex = 1;
                    else
                        intIndex++;
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length - 1; i++)
                {
                    if (!m_smVisionInfo.g_arrLead[intIndex].ref_blnSelected)
                    {
                        if (intIndex == 1)
                            intIndex = 4;
                        else
                            intIndex--;
                        continue;
                    }

                    //Not use prev setting but use default setting
                    if (!m_smVisionInfo.g_blnUsedPreTemplate)
                    {
                        m_smVisionInfo.g_arrLead[intIndex].SetCalibrationData(
                                                        m_smVisionInfo.g_fCalibPixelX,
                                                        m_smVisionInfo.g_fCalibPixelY,
                                                        m_smVisionInfo.g_fCalibOffSetX,
                                                        m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);

                        m_smVisionInfo.g_arrLead[intIndex].AnalyingLead_DefaultToleranceMethod(intIndex, false, ref intLeadNo);
                        m_smVisionInfo.g_arrLead[intIndex].BuildLeadsParameter(intIndex, m_smVisionInfo.g_arrLeadROIs[intIndex][0]);
                        m_smVisionInfo.g_arrLead[intIndex].DefineLeadVariance(m_smVisionInfo.g_arrLeadROIs[intIndex][0]);
                        //m_smVisionInfo.g_arrLead[intIndex].DefineTolerance(true);

                        m_smVisionInfo.g_arrLead[intIndex].DefineTolerance2();  // use back previous tolerance setting
                    }

                    if (intIndex == 1)
                        intIndex = 4;
                    else
                        intIndex--;
                }
            }

            if (true)
            {
                // use back previous span setting
            }
            else
            {

                //Define Span tolerance here
                //Span only exist if 2 opposite lead is used
                //Minimum span is the distance between the shortest lead from 2 opposite side
                //maximum span is the distance between the longest lead from 2 opposite side
                if (m_smVisionInfo.g_arrLead[1].ref_blnSelected && m_smVisionInfo.g_arrLead[3].ref_blnSelected)
                {
                    m_smVisionInfo.g_arrLead[1].DefineLeadSpanStartEnd(true, m_smVisionInfo.g_arrLeadROIs[1][0]);
                    m_smVisionInfo.g_arrLead[3].DefineLeadSpanStartEnd(false, m_smVisionInfo.g_arrLeadROIs[3][0]);

                    float fLeadMinSpanStart = 0;
                    float fLeadMaxSpanStart = 0;
                    float fLeadMinSpanEnd = 0;
                    float fLeadMaxSpanEnd = 0;
                    float fMinSpan = 0;
                    float fMaxSpan = 0;

                    m_smVisionInfo.g_arrLead[1].GetSpanData(1, ref fLeadMinSpanStart, ref fLeadMaxSpanStart);
                    m_smVisionInfo.g_arrLead[3].GetSpanData(3, ref fLeadMinSpanEnd, ref fLeadMaxSpanEnd);

                    fMinSpan = (fLeadMinSpanEnd - fLeadMinSpanStart);
                    fMaxSpan = (fLeadMaxSpanEnd - fLeadMaxSpanStart);

                    //Define template lead span upper/lower limit
                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        m_smVisionInfo.g_arrLead[i].ref_fTemplateLeadMinSpanLimit = (float)Math.Round(fMinSpan * 0.95f / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrLead[i].ref_fTemplateLeadMaxSpanLimit = (float)Math.Round(fMaxSpan * 1.05f / m_smVisionInfo.g_fCalibPixelY, 4, MidpointRounding.AwayFromZero);
                    }
                }
                else if (m_smVisionInfo.g_arrLead[2].ref_blnSelected && m_smVisionInfo.g_arrLead[4].ref_blnSelected)
                {
                    m_smVisionInfo.g_arrLead[2].DefineLeadSpanStartEnd(false, m_smVisionInfo.g_arrLeadROIs[2][0]);
                    m_smVisionInfo.g_arrLead[4].DefineLeadSpanStartEnd(true, m_smVisionInfo.g_arrLeadROIs[4][0]);

                    float fLeadMinSpanStart = 0;
                    float fLeadMaxSpanStart = 0;
                    float fLeadMinSpanEnd = 0;
                    float fLeadMaxSpanEnd = 0;
                    float fMinSpan = 0;
                    float fMaxSpan = 0;

                    m_smVisionInfo.g_arrLead[2].GetSpanData(2, ref fLeadMinSpanEnd, ref fLeadMaxSpanEnd);
                    m_smVisionInfo.g_arrLead[4].GetSpanData(4, ref fLeadMinSpanStart, ref fLeadMaxSpanStart);

                    fMinSpan = (fLeadMinSpanEnd - fLeadMinSpanStart);
                    fMaxSpan = (fLeadMaxSpanEnd - fLeadMaxSpanStart);

                    //Define template lead span

                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        m_smVisionInfo.g_arrLead[i].ref_fTemplateLeadMinSpanLimit = (float)Math.Round(fMinSpan * 0.95f / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrLead[i].ref_fTemplateLeadMaxSpanLimit = (float)Math.Round(fMaxSpan * 1.05f / m_smVisionInfo.g_fCalibPixelX, 4, MidpointRounding.AwayFromZero);
                    }
                }
            }
        }

        private void RotateImage()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                //if (!m_smVisionInfo.g_blnViewRotatedImage)    // Need to re-rotate and re-copy where the ViewRotatedImage is true or not. Bcos Rotated image has been modified for dont care area 
                {
                    if (Math.Abs(m_intCurrentAngle) <= 0 && Math.Abs(m_intCurrentAngle2) <= 0)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[i].ref_intImageViewNo].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrLead[i].ref_intImageViewNo);
                        m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[i].ref_intImageViewNo]);

                        m_smVisionInfo.g_blnViewRotatedImage = true;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
            }
        }

        /// <summary>
        /// Build lead blob objects
        /// For lead position Top and bottom, search for smaller blob within largest blob width in vertical manner
        /// For lead position right and left, search for smaller blob within largest blob height in horizontal manner
        /// combine all the blob can recalculate the blob width, height, center etc.
        /// </summary>
        /// <returns>true = successfully build object, false = fail to build object</returns>
        private bool BuildObjects()
        {
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ClearTemplateBlobsFeatures();

                    if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        continue;

                    m_smVisionInfo.g_arrLead[i].ClearTemplateBlobsFeatures();

                    // Build object using package ROI   //Dun change the thresholdvalue to -4 if build object fail. User will feel weird why the value chnage to auto without their acknowledgement. 
                    if (m_smVisionInfo.g_arrLead[i].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[i][0]))
                    {
                        m_smVisionInfo.g_arrLead[i].SetBlobsFeaturesToArray(m_smVisionInfo.g_arrLeadROIs[i][0]);
                        m_smVisionInfo.g_blnViewObjectsBuilded = true;
                    }
                }
            }
            return true;
        }
        private bool BuildObjects_BaseLead()
        {
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ClearTemplateBlobsFeatures_BaseLead();

                    if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        continue;

                    //m_smVisionInfo.g_arrLead[i].ClearTemplateBlobsFeatures_BaseLead();

                    // Build object using package ROI   //Dun change the thresholdvalue to -4 if build object fail. User will feel weird why the value chnage to auto without their acknowledgement. 
                    if (m_smVisionInfo.g_arrLead[i].BuildOnlyLeadObjects_BaseLead(m_smVisionInfo.g_arrLeadROIs[i][0]))
                    {
                        m_smVisionInfo.g_arrLead[i].SetBlobsFeaturesToArray_BaseLead(m_smVisionInfo.g_arrLeadROIs[i][0]);
                        m_smVisionInfo.g_blnViewObjectsBuilded = true;
                    }
                }
            }
            return true;
        }
        private bool DefineLeadToleranceUsingDefaultSetting(string[] strROIName)
        {
            int intCount = 0;
            //Use prev setting
            if (m_smVisionInfo.g_blnUsedPreTemplate)    // blnUsedPreTemplate will be set to True everytime user go into LearnLeadForm.
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        continue;

                    //Check no of objects is it tally with prev setting
                    if (!m_smVisionInfo.g_arrLead[i].CheckLeadTally(ref intCount))
                    {
                        if (intCount == 0)
                        {
                            if (SRMMessageBox.Show("Selected Leads is not tally with previous Leads record in " + strROIName[i] + ". There are " + intCount + " Leads in previous record." +
                                " Do you want to continue with default tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
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
                            if (SRMMessageBox.Show("Selected Leads is not tally with previous Leads record in " + strROIName[i] + ". There are " + intCount + " Leads in previous record." +
                                " Do you want to continue with previous Leads tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
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
                        if (!m_smVisionInfo.g_arrLead[i].MatchPrevSettings())
                        {
                            if (SRMMessageBox.Show("Unable to match current lead settings with previous lead record in " + strROIName[i] + "! " +
                              "Do you want to continue with default tolerance setting?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
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
                            m_smVisionInfo.g_arrLead[i].RearrangeBlobs();
                            m_smVisionInfo.g_arrLead[i].DefineTolerance(false);
                            m_smVisionInfo.g_arrLead[i].BuildLeadsParameter(i, m_smVisionInfo.g_arrLeadROIs[i][0]);   // Index 0: Search ROI, 1:Gauge ROI, 2: Package ROI
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Define Lead contour
        /// </summary>
        private void DefineLeadContour()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                {
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[i].ref_intImageViewNo]);
                    m_smVisionInfo.g_arrLead[i].DefineTemplateLeadContour2(m_smVisionInfo.g_arrLeadROIs[i][0]);
                }
            }
        }
        private void DefineMostSelectedImageNo()
        {
            int[] intCountImage = { 0, 0, 0, 0, 0 };

            int[] arrPackageSizeImageIndex = { };
            
                arrPackageSizeImageIndex = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeImageNoList();
            

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
            {
                m_smVisionInfo.g_intSelectedImage = 0;
            }

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
        }
        private string GetLeadDefinition(int intLeadIndex)
        {
            switch (intLeadIndex)
            {
                case 1:
                    return "*Top ROI: ";
                case 2:
                    return "*Right ROI: ";
                case 3:
                    return "*Bottom ROI: ";
                case 4:
                    return "*Left ROI: ";
                default:
                    SRMMessageBox.Show("GetLeadDefinition()->Lead Index " + intLeadIndex.ToString() + " no exist.");
                    return "";
            }
        }
        private void SetupSteps(bool blnForward)
        {

            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: //Define Search ROI                  
                    {
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                        // 2020 08 14 - CCENG: sometime user no press rotate button and directly press next. this cause the rotate image keep the old image.
                        if (blnForward)
                        {
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                            RotateAccordingToUnitPattern();
                        }
                        if ((((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2))
                        {
                            gb_Rotate.Visible = false;
                        }
               
                        // 2019-10-01 ZJYEOH : Use user selected Image
                        m_smVisionInfo.g_blnViewPackageImage = false;
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                        AddSearchROI(m_smVisionInfo.g_arrLeadROIs);

                        m_smVisionInfo.g_blnViewROI = true;
                        m_smVisionInfo.g_blnDragROI = true;

                        lbl_StepNo.BringToFront();
                        lbl_SearchROI.BringToFront();
                        tabCtrl_Lead.SelectedTab = tp_SearchROI;
                        btn_Next.Enabled = true;
                        btn_Previous.Enabled = false;
                    }
                    break;
                case 1: // Define package size gauge if use selected package to base method
                        //if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        //    return;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewRotatedImage = false;
                    DefineMostSelectedImageNo();

                    if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && (m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2))
                    {
                        //m_smVisionInfo.g_intSelectedImage = Math.Min(m_smVisionInfo.g_arrImages.Count - 1, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].);  // View Package1 image. g_intSelectedImage should get at least value 1 represent package1 image
                        //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
                        m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_objPackageImage);
                        for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
                        {
                            if (i < m_smVisionInfo.g_arrLeadROIs.Count)
                            {
                                //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                                //m_smVisionInfo.g_arrPackageROIs[i][0].AttachImage(m_smVisionInfo.g_objModifiedPackageImage);

                                m_smVisionInfo.g_arrPackageGaugeM4L[i].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                                //m_smVisionInfo.g_arrPackageGaugeM4L[i].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                                m_smVisionInfo.g_arrPackageGaugeM4L[i].SetGaugePlace_BasedOnEdgeROI();
                                m_smVisionInfo.g_arrPackageGaugeM4L[i].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_objWhiteImage);
                            }
                        }
                    }
                    else
                    {
                        //m_smVisionInfo.g_intSelectedImage = Math.Min(m_smVisionInfo.g_arrImages.Count - 1, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0));  // View Package1 image. g_intSelectedImage should get at least value 1 represent package1 image
                        //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue / 1000);
                        AttachImageToROI(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_objPackageImage);
                        for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                        {
                            if (i < m_smVisionInfo.g_arrPackageGaugeM4L.Count)
                            {
                                //m_smVisionInfo.g_objPackageImage.CopyTo(ref m_smVisionInfo.g_objModifiedPackageImage);
                                //m_smVisionInfo.g_arrPackageROIs[i][0].AttachImage(m_smVisionInfo.g_objModifiedPackageImage);

                                m_smVisionInfo.g_arrPackageGaugeM4L[i].AttachEdgeROI(m_smVisionInfo.g_objPackageImage);
                                //m_smVisionInfo.g_arrPackageGaugeM4L[i].ModifyDontCareImage(m_smVisionInfo.g_objWhiteImage);

                                m_smVisionInfo.g_arrPackageGaugeM4L[i].SetGaugePlace_BasedOnEdgeROI();
                                m_smVisionInfo.g_arrPackageGaugeM4L[i].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[i][0], m_smVisionInfo.g_objWhiteImage);
                            }
                        }
                    }
                    UpdatePackageImage_ForRectGaugeM4L();
                    m_smVisionInfo.g_blnViewGauge = true;
                    lbl_StepNo.BringToFront();
                    lbl_Gauge.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_PkgGauge;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 2: //Define Lead ROI
                        //m_smVisionInfo.g_intSelectedImage = 0;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                    ////m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    ////m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
                    //m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    //m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);

                    if (((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0) &&
                        !m_smVisionInfo.g_strVisionName.Contains("IPM"))
                    {
                        if (!StartOrientTest())
                        {
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }
                    }

                    if (m_smVisionInfo.g_blnWantPocketDontCareAreaFix_Lead)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
                        for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {

                            if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIFixMask & 0x01) == 0))
                                continue;
                            else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIFixMask & 0x02) == 0))
                                continue;
                            else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIFixMask & 0x04) == 0))
                                continue;
                            else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIFixMask & 0x08) == 0))
                                continue;

                            if (!FindFixPocket(i))
                            {

                                //break;//2021-09-12 ZJYEOH : Need to continue find other even one of it fail
                            }
                        }
                        //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\g_objPackageImage2.bmp");
                        //m_ImageWithMasking.SaveImage("D:\\m_ImageWithMasking.bmp");
                    }
                    else if (m_smVisionInfo.g_blnWantPocketDontCareAreaManual_Lead)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
                        for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {

                            if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x01) == 0))
                                continue;
                            else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x02) == 0))
                                continue;
                            else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x04) == 0))
                                continue;
                            else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x08) == 0))
                                continue;

                            if (!FindManualPocketReference(i))
                            {

                                //break;//2021-09-12 ZJYEOH : Need to continue find other even one of it fail
                            }
                        }
                        //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\g_objPackageImage2.bmp");
                        //m_ImageWithMasking.SaveImage("D:\\m_ImageWithMasking.bmp");
                    }
                    else if (m_smVisionInfo.g_blnWantPocketDontCareAreaAuto_Lead)
                    {
                        ImageDrawing objImg = new ImageDrawing(true);
                        //m_smVisionInfo.g_objPackageImage.CopyTo(objImg);
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(objImg);
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
                        for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {

                            if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x01) == 0))
                                continue;
                            else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x02) == 0))
                                continue;
                            else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x04) == 0))
                                continue;
                            else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x08) == 0))
                                continue;

                            if (!FindAutoPocketReference(i, objImg))
                            {

                                //break;//2021-09-12 ZJYEOH : Need to continue find other even one of it fail
                            }
                        }
                        objImg.Dispose();
                        //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\g_objPackageImage2.bmp");
                        //m_ImageWithMasking.SaveImage("D:\\m_ImageWithMasking.bmp");
                    }
                    else if (m_smVisionInfo.g_blnWantPocketDontCareAreaBlob_Lead)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);

                        m_smVisionInfo.g_arrInwardDontCareROIBlobLimit = new List<float>(5) { 0, 0, 0, 0, 0 };

                        for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {

                            if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) == 0))
                                continue;
                            else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) == 0))
                                continue;
                            else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) == 0))
                                continue;
                            else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) == 0))
                                continue;

                            if (!FindPocketShadowBlob(i, ref m_smVisionInfo.g_arrInwardDontCareROIBlobLimit))
                            {

                                //break;//2021-09-12 ZJYEOH : Need to continue find other even one of it fail
                            }
                        }

                        if (m_smVisionInfo.g_arrLead[0].ref_blnFlipToOppositeFunction && m_smVisionInfo.g_arrInwardDontCareROIBlobLimit.Contains(-1))
                        {
                            FlipToOpposite_DontCareBlob(m_smVisionInfo.g_arrInwardDontCareROIBlobLimit);
                        }
                        
                    }
                    else
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
                    }

                    RotatePrecise_AfterPocketDontCare();

                    if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2)//&& m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrLead.Length; j++)
                        {
                            if (m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                            {
                                m_smVisionInfo.g_arrLead[j].AssignLineGaugeDataFromPackageGaugeM4L(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle);
                            }
                        }
                        //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                        //{
                        //    m_intCurrentPreciseDeg = -(int)(Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle));
                        //    m_smVisionInfo.g_fPreciseAngle = m_intCurrentAngle + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle;
                        //    //if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && (m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2))
                        //    //{

                        //        ROI objRotateROI = new ROI();
                        //        objRotateROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                        //        objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                        //                                       (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                        //                                       m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth,
                        //                                       m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);

                        //        ROI.Rotate0Degree(m_smVisionInfo.g_objPackageImage, objRotateROI,
                        //                                m_intCurrentAngle + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                        //                                  ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                        //        m_smVisionInfo.g_blnViewRotatedImage = true;

                        //        objRotateROI.AttachImage(m_ImageWithMasking);
                        //        ROI.Rotate0Degree(m_ImageWithMasking, objRotateROI,
                        //                                m_intCurrentAngle + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                        //                                  ref m_ImageWithMasking);
                        //        objRotateROI.Dispose();
                        //    //}
                        //    //else
                        //    //{

                        //    //    ROI objRotateROI = new ROI();
                        //    //    objRotateROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                        //    //    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                        //    //                                   (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                        //    //                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                        //    //                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                        //    //    ROI.Rotate0Degree(m_smVisionInfo.g_objPackageImage, objRotateROI,
                        //    //                           m_intCurrentAngle + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                        //    //                              ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                        //    //    m_smVisionInfo.g_blnViewRotatedImage = true;


                        //    //    objRotateROI.AttachImage(m_ImageWithMasking);
                        //    //    ROI.Rotate0Degree(m_ImageWithMasking, objRotateROI,
                        //    //                        m_intCurrentAngle + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                        //    //                              ref m_ImageWithMasking);
                        //    //    objRotateROI.Dispose();
                        //    //}
                        //}
                        //else
                        //{

                        //    m_smVisionInfo.g_objPackageImage.CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                        //}
                    }

                    AddLeadROI(m_smVisionInfo.g_arrLeadROIs);

                    for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
                    {
                        if (m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                        {
                            if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                                m_smVisionInfo.g_arrLeadROIs[j][0].ClearDragHandle();
                        }
                    }

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_LeadROI.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_LeadROI;
                    btn_Previous.Enabled = true;
                    break;
                case 3: //Define Don't Care Area (maybe not need but keep first)
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                    if (blnForward)
                    {
                    }

                    RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                    //// Add Package ROI
                    //AddTrainPackageROI(m_smVisionInfo.g_arrLeadROIs);

                    for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                            continue;

                        if (m_smVisionInfo.g_arrLeadROIs[i].Count > 1) // Dont Care ROI start from index 1
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrLeadROIs[i].Count; j++)
                            {
                                m_smVisionInfo.g_arrLeadROIs[i][j].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
                            }
                        }
                    }

                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;

                    lbl_DontCareArea.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_DontCare;
                    break;
                case 4: //Build Object/Edition
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;

                    if (!gb_Threshold.Controls.Contains(cbo_SelectROI_Lead))
                    {
                        gb_Threshold.Controls.Add(cbo_SelectROI_Lead);
                    }
                    SelectLeadROI(m_smVisionInfo.g_intSelectedROI);
                    //2020-08-07 ZJYEOH : Hide this because need to build blob on rotated image
                    //if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                    //    RotateImage();

                    //for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    //{
                    //    if (i == 2 || i == 4) // Right Or Left
                    //        m_smVisionInfo.g_arrLead[i].ref_intLeadDirection = 0; // Horizontal
                    //    else if (i == 1 || i == 3) // Top Or Bottom
                    //        m_smVisionInfo.g_arrLead[i].ref_intLeadDirection = 1; // Vertical
                    //}

                    if (!m_blnKeepPrevObject)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                continue;

                            // Attach roi on rotated image
                            //    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[i].ref_intImageViewNo]);
                            m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_ImageWithMasking);
                            //if (m_smVisionInfo.g_arrLeadROIs[i].Count > 1) // Dont Care ROI start from index 1
                            //{
                            //    ROI.ModifyImageGain(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[i].ref_intImageViewNo]);

                            //    // Fill dont care area 
                            //    for (int j = 1; j < m_smVisionInfo.g_arrLeadROIs[i].Count; j++)
                            //    {
                            //        m_smVisionInfo.g_arrLeadROIs[i][j].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);

                            //        m_smVisionInfo.g_arrLeadROIs[i][j].FillROI(0);
                            //    }
                            //}
                        }

                        if (!BuildObjects())
                        {
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            break;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                continue;

                            m_smVisionInfo.g_arrLead[i].ReverseBlobs();
                        }
                    }

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_blnIdentityLeadsDone = false;
                    m_blnKeepPrevObject = false;
                    lbl_StepNo.BringToFront();
                    lbl_Segmentation.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Segment;
                    break;
                case 5: // Identify Lead No
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;

                    //2021-07-28 ZJYEOH : no need to check pitch min max anymore because set inside tolerance form
                    //if (!blnForward)
                    //{
                    //    if (IsPitchSettingError())
                    //    {
                    //        m_smVisionInfo.g_intLearnStepNo++;
                    //        m_intDisplayStepNo++;
                    //        break;
                    //    }
                    //}

                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        {
                            if (i == 1)
                            {
                                srmLabel63.Visible = false;
                                txt_PkgToBaseTolerance_Top.Visible = false;
                            }
                            else if (i == 2)
                            {
                                srmLabel61.Visible = false;
                                txt_PkgToBaseTolerance_Right.Visible = false;
                            }
                            else if (i == 3)
                            {
                                srmLabel62.Visible = false;
                                txt_PkgToBaseTolerance_Bottom.Visible = false;
                            }
                            else if (i == 4)
                            {
                                srmLabel60.Visible = false;
                                txt_PkgToBaseTolerance_Left.Visible = false;
                            }
                            continue;
                        }
                        //if (m_smVisionInfo.g_arrLead[i].ref_intLeadDirection == 0) // Horizontal
                        //{
                        //    srmLabel63.Visible = false;
                        //    txt_PkgToBaseTolerance_Top.Visible = false;
                        //    srmLabel62.Visible = false;
                        //    txt_PkgToBaseTolerance_Bottom.Visible = false;
                        //}
                        //else
                        //{
                        //    srmLabel60.Visible = false;
                        //    txt_PkgToBaseTolerance_Left.Visible = false;
                        //    srmLabel61.Visible = false;
                        //    txt_PkgToBaseTolerance_Right.Visible = false;
                        //}

                        if (i == 1)
                        {
                            srmLabel63.Visible = true;
                            txt_PkgToBaseTolerance_Top.Visible = true;
                        }
                        else if (i == 2)
                        {
                            srmLabel61.Visible = true;
                            txt_PkgToBaseTolerance_Right.Visible = true;
                        }
                        else if (i == 3)
                        {
                            srmLabel62.Visible = true;
                            txt_PkgToBaseTolerance_Bottom.Visible = true;
                        }
                        else if (i == 4)
                        {
                            srmLabel60.Visible = true;
                            txt_PkgToBaseTolerance_Left.Visible = true;
                        }
                    }

                    string[] strROIName;
                    //m_strPosition = "";
                    if (!m_blnIdentityLeadsDone)
                    {
                        strROIName = new string[m_smVisionInfo.g_arrLeadROIs.Count];

                        //Define ROI name for display in message box
                        for (int y = 0; y < m_smVisionInfo.g_arrLeadROIs.Count; y++)
                        {
                            if (!m_smVisionInfo.g_arrLead[y].ref_blnSelected)
                                continue;

                            switch (y)
                            {
                                case 1:
                                    strROIName[y] = "Up Leads";
                                    break;
                                case 2:
                                    strROIName[y] = "Right Leads";
                                    break;
                                case 3:
                                    strROIName[y] = "Down Leads";
                                    break;
                                case 4:
                                    strROIName[y] = "Left Leads";
                                    break;
                            }
                        }

                        // Make sure at lease 1 lead is selected.
                        int intTotalObjectNumber = m_intTotalTemplateBlobNo = 0;
                        for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                continue;

                            if (m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead != m_smVisionInfo.g_arrLead[i].GetSelectedObjectNumber())
                            {
                                string strLeadPosition = "";
                                switch (i)
                                {
                                    case 1:
                                        strLeadPosition = "Top";
                                        break;
                                    case 2:
                                        strLeadPosition = "Right";
                                        break;
                                    case 3:
                                        strLeadPosition = "Bottom";
                                        break;
                                    case 4:
                                        strLeadPosition = "Left";
                                        break;
                                }

                                SRMMessageBox.Show(strLeadPosition + " lead number is not tally with the number of object builded.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                m_smVisionInfo.g_intLearnStepNo--;
                                m_intDisplayStepNo--;
                                return;
                            }

                            intTotalObjectNumber += m_smVisionInfo.g_arrLead[i].GetSelectedObjectNumber();
                        }

                        m_intTotalTemplateBlobNo = intTotalObjectNumber;

                        if (intTotalObjectNumber == 0)
                        {
                            SRMMessageBox.Show("Minimum 1 lead is required.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            return;
                        }
                        else
                        {
                            for (int i = 0; i < intTotalObjectNumber; i++)
                            {
                                if (!cbo_LeadNo.Items.Contains("Lead " + (i + 1).ToString()))
                                    cbo_LeadNo.Items.Add("Lead " + (i + 1).ToString());
                            }
                            cbo_LeadNo.SelectedIndex = 0;
                        }

                        m_blnIdentityLeadsDone = true;

                        // Backup current builded blobsFeature before rearrange
                        for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                continue;

                            m_smVisionInfo.g_arrLead[i].BackupBlobsFeatures();

                            //int BaseValue = m_smVisionInfo.g_arrLead[i].GetBaseInwardOffset(0);
                            //int TipValue = m_smVisionInfo.g_arrLead[i].GetTipInwardOffset(0);

                            //if (BaseValue >= 0)
                            //    txt_BaseOffset.Text = BaseValue.ToString();

                            //if (TipValue >= 0)
                            //    txt_TipOffset.Text = TipValue.ToString();
                        }

                        if (!DefineLeadToleranceUsingDefaultSetting(strROIName))
                        {
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            return;
                        }

                        ReadAllLeadTemplateDataToGrid_DefaultMethod();
                    }

                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                            continue;
                        //m_smVisionInfo.g_arrLead[i].RestoreInwardOffsetFromTempBlob();

                        int BaseValue = m_smVisionInfo.g_arrLead[i].GetBaseInwardOffset(0);
                        int TipValue = m_smVisionInfo.g_arrLead[i].GetTipInwardOffset(0);

                        if (BaseValue >= 0)
                        {
                            txt_BaseOffset.Text = BaseValue.ToString();
                        }

                        if (TipValue >= 0)
                            txt_TipOffset.Text = TipValue.ToString();
                    }

                    ReadAllLeadTemplateDataToGrid_DefaultMethod();
                    SelectLeadROI(0);
                    m_smVisionInfo.g_blnViewObjectsBuilded = true;
                    lbl_IdentifyLead.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Lead;
                    break;
                case 6:
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;

                    if (blnForward)
                    {
                        tabCtrl_LeadPitch.Controls.Remove(tp_TopROI);
                        tabCtrl_LeadPitch.Controls.Remove(tp_RightROI);
                        tabCtrl_LeadPitch.Controls.Remove(tp_BottomROI);
                        tabCtrl_LeadPitch.Controls.Remove(tp_LeftROI);

                        for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                continue;

                            switch (i)
                            {
                                case 1:
                                    tabCtrl_LeadPitch.Controls.Add(tp_TopROI);
                                    break;
                                case 2:
                                    tabCtrl_LeadPitch.Controls.Add(tp_RightROI);
                                    break;
                                case 3:
                                    tabCtrl_LeadPitch.Controls.Add(tp_BottomROI);
                                    break;
                                case 4:
                                    tabCtrl_LeadPitch.Controls.Add(tp_LeftROI);
                                    break;
                            }
                        }

                        //// Rearrange pitch gap based on latest selected leads
                        //for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                        //{
                        //    if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        //        continue;

                        //    m_smVisionInfo.g_arrLead[i].BuildLeadPitchLink(false);

                        //}
                    }

                    // Rearrange pitch gap based on latest selected leads
                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead[i].BuildLeadPitchLink(false);

                        if (!chk_Reset.Checked)
                        {
                            m_smVisionInfo.g_arrLead[i].LoadPitchGapLinkTemporary();
                        }
                    }
                    
                    m_strPosition = "";
                    //Load template data
                    for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                            continue;

                        switch (i)
                        {
                            case 1:
                                m_strPosition = "Top";
                                ReadLeadPitchGapToGrid(1, dgd_TopPGSetting);
                                break;
                            case 2:
                                m_strPosition = "Right";
                                ReadLeadPitchGapToGrid(2, dgd_RightPGSetting);
                                break;
                            case 3:
                                m_strPosition = "Bottom";
                                ReadLeadPitchGapToGrid(3, dgd_BottomPGSetting);
                                break;
                            case 4:
                                m_strPosition = "Left";
                                ReadLeadPitchGapToGrid(4, dgd_LeftPGSetting);
                                break;
                        }
                    }

                    for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                            continue;

                        switch (i)
                        {
                            case 1:
                                m_strPosition = "Top";
                                m_dgdViewPG = dgd_TopPGSetting;
                                m_smVisionInfo.g_intSelectedROI = 1;
                                SelectLeadROI(m_smVisionInfo.g_intSelectedROI);
                                break;
                            case 2:
                                m_strPosition = "Right";
                                m_dgdViewPG = dgd_RightPGSetting;
                                m_smVisionInfo.g_intSelectedROI = 2;
                                SelectLeadROI(m_smVisionInfo.g_intSelectedROI);
                                break;
                            case 3:
                                m_strPosition = "Bottom";
                                m_dgdViewPG = dgd_BottomPGSetting;
                                m_smVisionInfo.g_intSelectedROI = 3;
                                SelectLeadROI(m_smVisionInfo.g_intSelectedROI);
                                break;
                            case 4:
                                m_strPosition = "Left";
                                m_dgdViewPG = dgd_LeftPGSetting;
                                m_smVisionInfo.g_intSelectedROI = 4;
                                SelectLeadROI(m_smVisionInfo.g_intSelectedROI);
                                break;
                        }
                        break;
                    }

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;

                    lbl_StepNo.BringToFront();
                    lbl_Pitch.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Pitch;
                    btn_Next.Enabled = true;
                    break;
                case 7: //Build Object/Edition for Base Lead
                    m_smVisionInfo.g_blnViewPackageImage = false;

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo;

                    // Rotate selected base lead image 
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo);
                    ROI.Rotate0Degree(m_smVisionInfo.g_arrLeadROIs[0][0], m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo);

                    //if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2)
                    //{
                    //    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                    //    {
                    //        if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && (m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2))
                    //        {

                    //            ROI objRotateROI = new ROI();
                    //            objRotateROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    //            objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                    //                                           (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                    //                                           m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth,
                    //                                           m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);

                    //            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], objRotateROI,
                    //                                      m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                    //                                      ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    //            m_smVisionInfo.g_blnViewRotatedImage = true;

                    //            objRotateROI.Dispose();
                    //        }
                    //        else
                    //        {

                    //            ROI objRotateROI = new ROI();
                    //            objRotateROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    //            objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                    //                                           (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                    //                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                    //                                           m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                    //            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], objRotateROI,
                    //                                      m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                    //                                      ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    //            m_smVisionInfo.g_blnViewRotatedImage = true;

                    //            objRotateROI.Dispose();
                    //        }
                    //    }
                    //    //else
                    //    //{

                    //    //    m_smVisionInfo.g_objPackageImage.CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    //    //}
                    //}
                  
                    //if (!m_blnKeepPrevObject)
                    //{
                        for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                continue;

                            m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[i].ref_intBaseLeadImageViewNo]);
                        }

                        if (!BuildObjects_BaseLead())
                        {
                            //m_smVisionInfo.g_intLearnStepNo--;
                            //m_intDisplayStepNo--;
                            //break;
                        }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    //    {
                    //        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    //            continue;

                    //        m_smVisionInfo.g_arrLead[i].ReverseBlobs_BaseLead();
                    //    }
                    //}

                    if (blnForward)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                continue;

                            m_smVisionInfo.g_arrLead[i].BackupBlobsFeatures_BaseLead();
                        }
                    }

                    if (!gb_Threshold_BaseLead.Controls.Contains(cbo_SelectROI_Lead))
                    {
                        gb_Threshold_BaseLead.Controls.Add(cbo_SelectROI_Lead);
                    }
                    SelectLeadROI(m_smVisionInfo.g_intSelectedROI);

                    m_blnKeepPrevObject = false;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    lbl_StepNo.BringToFront();
                    lbl_BaseLead.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp__BaseLead;
                    break;
                case 8:
                    if (m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                    {
                        int intLeadNo = 1;
                        int intIndex = m_smVisionInfo.g_arrLead[0].ref_intFirstLead;
                        for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[intIndex].ref_blnSelected)
                            {
                                if (intIndex == 4)
                                    intIndex = 1;
                                else
                                    intIndex++;
                                continue;
                            }

                            if (m_smVisionInfo.g_arrLead[i].MatchPrevSettings_BaseLead())
                            {
                                m_smVisionInfo.g_arrLead[i].SortObjectNumber_BaseLead(intIndex, cbo_LeadLabelDirection.SelectedIndex == 0, ref intLeadNo);
                                m_smVisionInfo.g_arrLead[i].MatchPrevSettings_BaseLead();
                                m_smVisionInfo.g_arrLead[i].RearrangeBlobs_BaseLead(intLeadNo, m_intTotalTemplateBlobNo);
                                m_smVisionInfo.g_arrLead[i].UpdatePreviousToleranceToTemplate_BaseLead();
                               
                            }
                            else
                            {
                                SRMMessageBox.Show("Number of Leads built do not matched with template leads count.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                m_smVisionInfo.g_intLearnStepNo--;
                                m_intDisplayStepNo--;
                                goto Break;
                            }
                        }
                    }
                    SelectLeadROI(0);
                    lbl_SaveTemplate.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Save;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    Break:
                    break;
                default:
                    break;
            }
        }

        public bool IsPitchSettingError()
        {
            int[] arrColumnIndex = { 2, 4 };

            for (int intLeadIndex = 1; intLeadIndex < 5; intLeadIndex++)
            {
                DataGridView objDataGrid = new DataGridView();
                string strLeadName = "";
                switch (intLeadIndex)
                {
                    case 1:
                        strLeadName = "Top Lead";
                        objDataGrid = dgd_TopPGSetting;
                        break;
                    case 2:
                        strLeadName = "Right Lead";
                        objDataGrid = dgd_RightPGSetting;
                        break;
                    case 3:
                        strLeadName = "Bottom Lead";
                        objDataGrid = dgd_BottomPGSetting;
                        break;
                    case 4:
                        strLeadName = "Left Lead";
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
                                //Define lead index
                                int intLeadPitchIndex = tabCtrl_LeadPitch.SelectedIndex;
                                for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
                                {
                                    if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                                        intLeadPitchIndex++;
                                    else
                                    {
                                        if (intLeadPitchIndex == j)
                                            break;
                                    }
                                }

                                tabCtrl_LeadPitch.SelectedIndex = intLeadPitchIndex;
                                SRMMessageBox.Show("Set minimum value or maximum value is not corrects in " + strLeadName + ". Please check the red highlight value is correct or not.");
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Set pitch gap from Lead no, to Lead no
        /// </summary>
        /// <param name="intPitchIndex">pitch gap index</param>
        /// <param name="intCurrentFromLeadNo">current from Lead no</param>
        /// <param name="intCurrentToLeadNo">current to Lead no</param>
        /// <param name="intLeadPosition">Lead position</param>
        private void SetPitch(int intPitchIndex, int intCurrentFromLeadNo, int intCurrentToLeadNo, int intLeadPosition)
        {
            int intLocationX = -1;
            int intLocationY = -1;

            bool blnFirstTime = true;
            while (true)
            {
                //Define lead index
                int intLeadPitchIndex = tabCtrl_LeadPitch.SelectedIndex;
                for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
                {
                    if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                        intLeadPitchIndex++;
                    else
                    {
                        if (intLeadPitchIndex == j)
                            break;
                    }
                }

                intPitchIndex = m_smVisionInfo.g_arrLead[intLeadPitchIndex].GetTotalPitchGap();
                int intTotalLeadNo = m_smVisionInfo.g_arrLead[intLeadPosition].GetBlobsFeaturesNumber();
                int intNoID = m_smVisionInfo.g_arrLead[intLeadPosition].GetBlobsNoID();
                SetLeadPitchForm objSetPitchForm;
                if (blnFirstTime)
                    objSetPitchForm = new SetLeadPitchForm(intTotalLeadNo, intCurrentFromLeadNo, intCurrentToLeadNo, intNoID);
                else if (intCurrentToLeadNo >= (intTotalLeadNo - 1))
                    objSetPitchForm = new SetLeadPitchForm(intTotalLeadNo, intCurrentFromLeadNo, intCurrentToLeadNo, intNoID);
                else if (intCurrentFromLeadNo >= (intTotalLeadNo - 1))
                    objSetPitchForm = new SetLeadPitchForm(intTotalLeadNo, intCurrentFromLeadNo, intCurrentToLeadNo, intNoID);
                else
                    objSetPitchForm = new SetLeadPitchForm(intTotalLeadNo, intCurrentFromLeadNo + 1, intCurrentToLeadNo + 1, intNoID);
                if (intLocationX != -1)
                {
                    objSetPitchForm.StartPosition = FormStartPosition.Manual;
                    objSetPitchForm.Location = new Point(intLocationX, intLocationY);
                }
                if (objSetPitchForm.ShowDialog() == DialogResult.OK)
                {
                    int intFormLeadNo = objSetPitchForm.ref_intFromLeadNo - intNoID;
                    int intToLeadNo = objSetPitchForm.ref_intToLeadNo - intNoID;
                    if (intFormLeadNo == intToLeadNo)
                    {
                        SRMMessageBox.Show("Cannot select same Lead number!");
                    }
                    else if (m_smVisionInfo.g_arrLead[intLeadPosition].CheckPitchGapLinkExist(intFormLeadNo, intToLeadNo))
                    {
                        SRMMessageBox.Show("This Pitch/Gap link already exist.");
                    }
                    else if (m_smVisionInfo.g_arrLead[intLeadPosition].CheckPitchGapLinkInLeadAlready(intFormLeadNo))
                    {
                        SRMMessageBox.Show("Pitch/Gap already defined in Lead number " + (intFormLeadNo + 1));
                    }
                    else if (!m_smVisionInfo.g_arrLead[intLeadPosition].CheckPitchGapLinkAvailable(intFormLeadNo, intToLeadNo))
                    {
                        SRMMessageBox.Show("Pitch/Gap can not be created in between Lead no" + (intFormLeadNo + 1) + " and Lead no" + (intToLeadNo + 1) + ".");
                    }
                    else
                    {
                        m_smVisionInfo.g_arrLead[intLeadPosition].SetPitchGap(intPitchIndex, intFormLeadNo, intToLeadNo);
                    }

                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    intCurrentFromLeadNo = intFormLeadNo;
                    intCurrentToLeadNo = intToLeadNo;
                    intLocationX = objSetPitchForm.Location.X;
                    intLocationY = objSetPitchForm.Location.Y;

                    ReadLeadPitchGapToGrid(intLeadPitchIndex, m_dgdViewPG);
                }
                else
                {
                    ReadLeadPitchGapToGrid(intLeadPitchIndex, m_dgdViewPG);
                    break;
                }

                blnFirstTime = false;
            }
        }

        /// <summary>
        /// Read Lead pitch gap data into datagridview
        /// </summary>
        /// <param name="intLeadPosition">Lead position</param>
        /// <param name="dgd_LeadPitch">datagridview</param>
        private void ReadLeadPitchGapToGrid(int intLeadPosition, DataGridView dgd_LeadPitch)
        {
            int k = 0;
            try
            {

                if (m_smVisionInfo.g_arrLead.Length <= intLeadPosition)
                    return;

                dgd_LeadPitch.Rows.Clear();
                List<int> arrTotalLead = m_smVisionInfo.g_arrLead[intLeadPosition].GetLeadID();
                int intNoID = m_smVisionInfo.g_arrLead[intLeadPosition].GetBlobsNoID();
                for (int i = 0; i < arrTotalLead.Count; i++)
                {
                    int intToLeadNo = m_smVisionInfo.g_arrLead[intLeadPosition].GetPitchGapToLeadNo(i);

                    dgd_LeadPitch.Rows.Add();
                    dgd_LeadPitch.Rows[i].HeaderCell.Value = "Pitch " + (i + 1);
                    if (intToLeadNo >= 0)
                    {
                        dgd_LeadPitch.Rows[i].Cells[0].Value = arrTotalLead[i];
                        dgd_LeadPitch.Rows[i].Cells[1].Value = intToLeadNo + intNoID;
                    }
                    else
                    {
                        dgd_LeadPitch.Rows[i].Cells[0].Value = arrTotalLead[i];
                        dgd_LeadPitch.Rows[i].Cells[1].Value = "NA";
                    }
                }
                m_intPitchSelectedRowIndex = 0;

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("LearnLeadForm.cs->ReadLeadPitchToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("LearnLeadForm.cs->ReadLeadPitchToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
            }

        }
        private void ReadLeadPitchGapToGrid_DisplayAvailablePitchGapRowsOnly(int intLeadPosition, DataGridView dgd_LeadPitch)
        {
            int k = 0;
            try
            {

                if (m_smVisionInfo.g_arrLead.Length <= intLeadPosition)
                    return;

                dgd_LeadPitch.Rows.Clear();
                int intIndex = 0;
                string strPitchData;
                string[] strPitchDataRow;
                int intTotalPitch = m_smVisionInfo.g_arrLead[intLeadPosition].GetTotalPitchGap();
                int intNoID = m_smVisionInfo.g_arrLead[intLeadPosition].GetBlobsNoID();
                //Set pitch gap col name
                for (int j = 0; j < m_strPGColName.Length; j++)
                {
                    if (intLeadPosition == 0)
                        m_strPGRealColName[j] = m_strPGColName[j];
                    else
                        m_strPGRealColName[j] = m_strPGColName[j] + m_strPosition;
                }


                for (int i = 0; i < intTotalPitch; i++)
                {
                    intIndex = 0;
                    strPitchData = m_smVisionInfo.g_arrLead[intLeadPosition].GetPitchGapData(i);
                    strPitchDataRow = strPitchData.Split('#');

                    dgd_LeadPitch.Rows.Add();
                    dgd_LeadPitch.Rows[i].HeaderCell.Value = "Pitch " + (i + 1);
                    for (k = 0; k < m_strPGRealColName.Length; k++)
                    {
                        if (k == 0 || k == 1)
                            dgd_LeadPitch.Rows[i].Cells[m_strPGRealColName[k]].Value = Convert.ToInt32(strPitchDataRow[intIndex++]) + intNoID;
                        else
                            dgd_LeadPitch.Rows[i].Cells[m_strPGRealColName[k]].Value = strPitchDataRow[intIndex++];
                    }
                }
                m_intPitchSelectedRowIndex = 0;

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("LearnLeadForm.cs->ReadLeadPitchToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("LearnLeadForm.cs->ReadLeadPitchToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
            }

        }

        private void LearnLeadForm_Load(object sender, EventArgs e)
        {
            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                AddSearchAndUnitROI();

            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.g_blnDrawPkgResult = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            //m_smVisionInfo.g_blnLeadInspected = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].BackupPreviousTolerance();
                if (m_smVisionInfo.g_arrLead[i].ref_blnWantInspectBaseLead)
                    m_smVisionInfo.g_arrLead[i].BackupPreviousTolerance_BaseLead();
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (i < m_smVisionInfo.g_arrblnImageRotated.Length)
                    m_smVisionInfo.g_arrblnImageRotated[i] = true;
            }

            // Start Setup step 1
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.g_intLearnStepNo = 0;
            SetupSteps(true);

           
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
      
        }

        private void LearnLeadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Lead Form Closed", "Exit Learn Lead Form", "", "", m_smProductionInfo.g_strLotID);
            
            if (chk_UsePreTemplateImage.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (i < m_arrCurrentImage.Count)
                        m_arrCurrentImage[i].CopyTo(ref m_smVisionInfo.g_arrImages, i);
                }
            }
            for (int i = 0; i < m_arrCurrentImage.Count; i++)
            {
                m_arrCurrentImage[i].Dispose();
            }
            m_ImageWithMasking.Dispose();
            m_smVisionInfo.g_fPreciseAngle = 0;
            m_smVisionInfo.g_blnWantShowPocketDontCareArea = false;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;

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
                    m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\OriTemplate.bmp");

                if (m_smVisionInfo.g_arrImages.Count > 1)
                {
                    m_smVisionInfo.g_arrImages[1].CopyTo(ref m_arrCurrentImage, 1);
                    m_smVisionInfo.g_arrImages[1].LoadImage(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\OriTemplate_Image1.bmp");
                }
                if (m_smVisionInfo.g_arrImages.Count > 2)
                {
                    m_smVisionInfo.g_arrImages[2].CopyTo(ref m_arrCurrentImage, 2);
                    m_smVisionInfo.g_arrImages[2].LoadImage(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\OriTemplate_Image2.bmp");
                }
                if (m_smVisionInfo.g_arrImages.Count > 3)
                {
                    m_smVisionInfo.g_arrImages[3].CopyTo(ref m_arrCurrentImage, 3);
                    m_smVisionInfo.g_arrImages[3].LoadImage(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        m_smVisionInfo.g_strVisionFolderName + "\\Lead\\Template\\OriTemplate_Image3.bmp");
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

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadROISetting(strFolderPath + "Lead\\ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead.Length);
            LoadLeadSetting(strFolderPath + "Lead\\");

            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0))// && m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
            {
                LoadROISetting(m_strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPackageROIs);
                LoadGaugeM4LSetting(m_strFolderPath + "GaugeM4L.xml", m_smVisionInfo.g_arrPackageGaugeM4L);
            }
            else if (m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2)
            {
                LoadGaugeM4LSetting(m_strFolderPath + "GaugeM4L.xml", m_smVisionInfo.g_arrPackageGaugeM4L);
            }
            m_smVisionInfo.AT_PR_AttachImagetoROI = true;
            this.Close();
            this.Dispose();
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo == 5 && !m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                m_blnKeepPrevObject = true;

            if (m_smVisionInfo.g_intLearnStepNo == 6)
                m_blnIdentityLeadsDone = true;

            if (m_smVisionInfo.g_intLearnStepNo > 0)
                m_smVisionInfo.g_intLearnStepNo--;

            if (m_smVisionInfo.g_intLearnStepNo == 3)
                m_smVisionInfo.g_intLearnStepNo--;

            if (m_smVisionInfo.g_intLearnStepNo == 1)// && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
            {
                if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && m_smVisionInfo.g_arrLead[0].ref_intRotationMethod != 2)
                    m_smVisionInfo.g_intLearnStepNo--;
            }

            if (m_smVisionInfo.g_intLearnStepNo == 7 && !m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                m_smVisionInfo.g_intLearnStepNo--;

            m_intDisplayStepNo--;

            SetupSteps(false);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo < 99)
                m_smVisionInfo.g_intLearnStepNo++;

            if (m_smVisionInfo.g_intLearnStepNo == 3)
                m_smVisionInfo.g_intLearnStepNo++;

            if (m_smVisionInfo.g_intLearnStepNo == 1)// && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
            {
                if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && m_smVisionInfo.g_arrLead[0].ref_intRotationMethod != 2)
                    m_smVisionInfo.g_intLearnStepNo++;
            }

            if (m_smVisionInfo.g_intLearnStepNo == 7 && !m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                m_smVisionInfo.g_intLearnStepNo++;
            
            m_intDisplayStepNo++;
            
            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void CopyInfoToMark()
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (m_smVisionInfo.g_arrMarkROIs.Count <= i)
                    m_smVisionInfo.g_arrMarkROIs.Add(new List<ROI>());

                for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[i].Count; j++)
                {
                    if (m_smVisionInfo.g_arrMarkROIs[i].Count <= j)
                        m_smVisionInfo.g_arrMarkROIs[i].Add(new ROI());

                    if (j != 1) // Mark ROI
                    {
                        // Copy ROI position 
                        ROI objMarkROI = m_smVisionInfo.g_arrMarkROIs[i][j];
                        m_smVisionInfo.g_arrOrientROIs[i][j].CopyTo(ref objMarkROI);
                    }

                    if (j == 0)
                    {
                        m_smVisionInfo.g_arrMarkROIs[i][j].AttachROITopParrent(m_smVisionInfo.g_arrOrientROIs[i][j]);
                    }
                }

                //RectGauge objGauge = (RectGauge)m_smVisionInfo.g_arrOrientGauge[i];
                //// Copy Gauge
                //if (m_smVisionInfo.g_arrMarkGauge.Count <= i)
                //    m_smVisionInfo.g_arrMarkGauge.Add(objGauge);
                //else
                //    m_smVisionInfo.g_arrMarkGauge[i] = objGauge;

                if (m_smVisionInfo.g_blnWantGauge)
                {
                    RectGaugeM4L objGauge4L = (RectGaugeM4L)m_smVisionInfo.g_arrOrientGaugeM4L[i];
                    // Copy Gauge
                    if (m_smVisionInfo.g_arrMarkGaugeM4L.Count <= i)
                        m_smVisionInfo.g_arrMarkGaugeM4L.Add(objGauge4L);
                    else
                        m_smVisionInfo.g_arrMarkGaugeM4L[i] = objGauge4L;
                }
            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                        m_smVisionInfo.g_strVisionFolderName + "\\";

            //m_smVisionInfo.g_intSelectedImage = 0;
            ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

            DefineLeadContour();
            SaveLeadSetting(strPath + "Lead\\");

            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0))// && m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
            {
                SaveROISettings();
                SaveGaugeSettings();
            }
            else if(m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2)
            {
                SaveGaugeSettings();
            }
            // Save Orient
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_arrOrientROIs.Count > 0 && m_smVisionInfo.g_arrOrientROIs[0].Count > 0)
            {
                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY,
                     m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);
                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                XmlParser objFile = new XmlParser(strPath + "Orient\\" + "Template\\Template.xml");
                ROI objROI;
                //STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
                objFile = new XmlParser(strPath + "Orient\\" + "ROI.xml", false);
                for (int x = 0; x < m_smVisionInfo.g_arrOrientROIs.Count; x++)
                {
                    objFile.WriteSectionElement("Unit" + x);
                    for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[x].Count; j++)
                    {
                        if (j == 0)
                        {
                            objROI = m_smVisionInfo.g_arrOrientROIs[x][j];
                            if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                            {
                                ROI objSelectedROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][j];
                                objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
                                objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
                                objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
                                objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
                            }

                            objFile.WriteElement1Value("ROI" + j, "");

                            objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                            objFile.WriteElement2Value("Type", objROI.ref_intType);
                            objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                            objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                            objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                            objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                            objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                            objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
                            //float fPixelAverage = objROI.GetROIAreaPixel();
                            //objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                            //objROI.SetROIPixelAverage(fPixelAverage);

                            objFile.WriteEndElement();
                            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient ROI", strFolderPath, "ROI.xml");
                        }
                    }
                }
                
                CopyInfoToMark();
            }

            if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0].LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY,
                       m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);
                    m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                }
                //SaveROISettings(m_strFolderPath + "Mark\\", m_smVisionInfo.g_arrMarkROIs, "Mark");
                XmlParser objFile = new XmlParser(strPath + "Mark\\" + "Template\\Template.xml");
                ROI objROI;
                //STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
                objFile = new XmlParser(strPath + "Mark\\" + "ROI.xml", false);
                for (int x = 0; x < m_smVisionInfo.g_arrMarkROIs.Count; x++)
                {
                    objFile.WriteSectionElement("Unit" + x);
                    for (int j = 0; j < m_smVisionInfo.g_arrMarkROIs[x].Count; j++)
                    {
                        if (j == 0)
                        {
                            objROI = m_smVisionInfo.g_arrMarkROIs[x][j];
                            if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                            {
                                ROI objSelectedROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][j];
                                objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
                                objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
                                objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
                                objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
                            }

                            objFile.WriteElement1Value("ROI" + j, "");

                            objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                            objFile.WriteElement2Value("Type", objROI.ref_intType);
                            objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                            objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                            objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                            objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                            objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                            objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
                            //float fPixelAverage = objROI.GetROIAreaPixel();
                            //objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                            //objROI.SetROIPixelAverage(fPixelAverage);

                            objFile.WriteEndElement();
                            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient ROI", strFolderPath, "ROI.xml");
                        }
                    }
                }
            }
            
            // Empty Search ROI
            if (m_smVisionInfo.g_arrPositioningROIs != null)
            {
                if (m_smVisionInfo.g_arrPositioningROIs.Count > 0 && m_smVisionInfo.g_arrOrientROIs.Count > 0 && m_smVisionInfo.g_arrOrientROIs[0].Count > 0)
                {
                    m_smVisionInfo.g_arrPositioningROIs[0].LoadROISetting(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY,
                        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);

                    string strEmptyFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\";
                    ROI.SaveFile(strEmptyFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
                }
            }
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadROISetting(strFolderPath + "Lead\\ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead.Length);
            LoadLeadSetting(strFolderPath + "Lead\\");

            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();

        }

        private void txt_LeadNumber_Top_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intNumberOfLead_Top = Convert.ToInt32(txt_LeadNumber_Top.Text);
            int intNumberOfLead_Bottom = Convert.ToInt32(txt_LeadNumber_Bottom.Text);
            int intNumberOfLead_Left = Convert.ToInt32(txt_LeadNumber_Left.Text);
            int intNumberOfLead_Right = Convert.ToInt32(txt_LeadNumber_Right.Text);

            if (intNumberOfLead_Top == 0 && intNumberOfLead_Bottom == 0 && intNumberOfLead_Left == 0 && intNumberOfLead_Right == 0)
            {
                SRMMessageBox.Show("At least 1 Lead must be selected.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txt_LeadNumber_Top.Text = "1";
                intNumberOfLead_Top = Convert.ToInt32(txt_LeadNumber_Top.Text);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                switch (i)
                {
                    case 1:
                        if (intNumberOfLead_Top > 0)
                            m_smVisionInfo.g_arrLead[i].ref_blnSelected = true;
                        else
                            m_smVisionInfo.g_arrLead[i].ref_blnSelected = false;

                        m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead = intNumberOfLead_Top;
                        break;
                    case 2:
                        if (intNumberOfLead_Right > 0)
                            m_smVisionInfo.g_arrLead[i].ref_blnSelected = true;
                        else
                            m_smVisionInfo.g_arrLead[i].ref_blnSelected = false;

                        m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead = intNumberOfLead_Right;
                        break;
                    case 3:
                        if (intNumberOfLead_Bottom > 0)
                            m_smVisionInfo.g_arrLead[i].ref_blnSelected = true;
                        else
                            m_smVisionInfo.g_arrLead[i].ref_blnSelected = false;

                        m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead = intNumberOfLead_Bottom;
                        break;
                    case 4:
                        if (intNumberOfLead_Left > 0)
                            m_smVisionInfo.g_arrLead[i].ref_blnSelected = true;
                        else
                            m_smVisionInfo.g_arrLead[i].ref_blnSelected = false;

                        m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead = intNumberOfLead_Left;
                        break;
                }
                //2021-04-08 ZJYEOH : Load Point Gauge so that local point gauge inside Lead.cs will tally with current number of lead set by user
                m_smVisionInfo.g_arrLead[i].LoadArrayPointGauge(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead\\" + "PointGauge.xml");
            }
            AddLeadROI(m_smVisionInfo.g_arrLeadROIs);
            UpdateReferenceCornerGUI();
 
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].ref_intFilterMinArea = Convert.ToInt32(txt_MinArea.Text);
            }
            
            BuildObjects();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BaseOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || !m_blnIdentityLeadsDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                if (chk_SetToAllLeads.Checked)
                {
                    for (int j = 0; j < m_intTotalTemplateBlobNo; j++)
                        m_smVisionInfo.g_arrLead[i].SetBaseInwardOffset(j, Convert.ToInt32(txt_BaseOffset.Text));
                }
                else
                    m_smVisionInfo.g_arrLead[i].SetBaseInwardOffset(cbo_LeadNo.SelectedIndex, Convert.ToInt32(txt_BaseOffset.Text));
            }

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void txt_TipOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || !m_blnIdentityLeadsDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                if (chk_SetToAllLeads.Checked)
                {
                    for (int j = 0; j < m_intTotalTemplateBlobNo; j++)
                        m_smVisionInfo.g_arrLead[i].SetTipInwardOffset(j, Convert.ToInt32(txt_TipOffset.Text));
                }
                else
                    m_smVisionInfo.g_arrLead[i].SetTipInwardOffset(cbo_LeadNo.SelectedIndex, Convert.ToInt32(txt_TipOffset.Text));
            }

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void btn_RotateUnit_Click(object sender, EventArgs e)
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;
            btn_RotateAngle.Enabled = false;

            ROI objROI = new ROI();
            objROI = m_smVisionInfo.g_arrLeadROIs[0][0];

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                objRotatedImage = m_smVisionInfo.g_arrRotatedImages[i];
                m_smVisionInfo.g_arrImages[i].CopyTo(ref objRotatedImage);
            }

            // Rotate Unit IC
            if (sender == btn_ClockWise)
                m_intCurrentAngle += 270;
            else
                m_intCurrentAngle += 90;

            m_intCurrentAngle %= 360;
            m_intCurrentAngle2 = 0;

            ROI.RotateROI(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                ROI.RotateROI(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, i);
            }

            // After rotating the image, attach the rotated image into ROI again
            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;
        }

        private void btn_RotateAngle_Click(object sender, EventArgs e)
        {
            if (txt_RotateAngle.Text == null || txt_RotateAngle.Text == "")
                return;

            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;
            btn_RotateAngle.Enabled = false;

            ROI objROI = new ROI();
            objROI = m_smVisionInfo.g_arrLeadROIs[0][0];

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                objRotatedImage = m_smVisionInfo.g_arrRotatedImages[i];
                m_smVisionInfo.g_arrImages[i].CopyTo(ref objRotatedImage); 
            }

            m_intCurrentAngle2 = int.Parse(txt_RotateAngle.Text);

            ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentAngle2, 8, ref m_smVisionInfo.g_arrRotatedImages, 0);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentAngle2, 8, ref m_smVisionInfo.g_arrRotatedImages, i);
            }

            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;
        }

        private void radioBtn_CutObj_CheckedChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnCutMode = radioBtn_CutObj.Checked;
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageImage = true;
            m_ImageWithMasking.CopyTo(ref m_smVisionInfo.g_objPackageImage);
            int intSelectedROI = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    intSelectedROI++;
                else
                    break;
            }

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                        m_smVisionInfo.g_arrLeadROIs[j][0].ClearDragHandle();
                }

                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                {
                    intSelectedROI = j;
                }
            }

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead[intSelectedROI].ref_intThresholdValue;

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[0].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[0].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[1].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[1].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[2].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[2].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[3].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[3].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[4].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[4].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, true, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead[i].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;

                        m_smVisionInfo.g_arrLead[i].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[i][0]);

                        //Clear Temp Blobs Features
                        m_smVisionInfo.g_arrLead[i].ClearTempBlobsFeatures();

                        //Set blobs features data into Temp Blobs Features
                        m_smVisionInfo.g_arrLead[i].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[i][0]);

                        //Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                        m_smVisionInfo.g_arrLead[i].CompareSelectedBlobs();
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead[intSelectedROI].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;

                    m_smVisionInfo.g_arrLead[intSelectedROI].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                    //Clear Temp Blobs Features
                    m_smVisionInfo.g_arrLead[intSelectedROI].ClearTempBlobsFeatures();

                    //Set blobs features data into Temp Blobs Features
                    m_smVisionInfo.g_arrLead[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                    //Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                    m_smVisionInfo.g_arrLead[intSelectedROI].CompareSelectedBlobs();
                }
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead[intSelectedROI].ref_intThresholdValue;

                m_smVisionInfo.g_arrLead[intSelectedROI].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                //Clear Temp Blobs Features
                m_smVisionInfo.g_arrLead[intSelectedROI].ClearTempBlobsFeatures();

                //Set blobs features data into Temp Blobs Features
                m_smVisionInfo.g_arrLead[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                //Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                m_smVisionInfo.g_arrLead[intSelectedROI].CompareSelectedBlobs();
            }

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_Vertical_CheckedChanged(object sender, EventArgs e)
        {
            //if (radioBtn_Horizontal.Checked)
            //{
            //    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            //    {
            //        m_smVisionInfo.g_arrLead[i].ref_intLeadDirection = 0;
            //    }

            //    pic_LeadCount.BackgroundImage = pic_DirectionHorizontal.BackgroundImage;
            //    picLeadROI2.Image = ils_ImageListTree.Images[3];
            //}
            //else
            //{
            //    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            //    {
            //        m_smVisionInfo.g_arrLead[i].ref_intLeadDirection = 1;
            //    }

            //    pic_LeadCount.BackgroundImage = pic_DirectionVertical.BackgroundImage;
            //    picLeadROI2.Image = ils_ImageListTree.Images[4];
            //}
        }

        private void btn_UndoObjects_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                    continue;

                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                {
                    intSelectedROI = j;
                    break;
                }
            }

            m_smVisionInfo.g_arrLead[intSelectedROI].UndoSelectedObject();

            m_smVisionInfo.g_blnViewObjectsBuilded = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BuildObjects_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    intSelectedROI++;
                else
                    break;
            }

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                        m_smVisionInfo.g_arrLeadROIs[j][0].ClearDragHandle();
                }

                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                {
                    intSelectedROI = j;
                }
            }

            m_smVisionInfo.g_arrLead[intSelectedROI].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

            //Clear Temp Blobs Features
            m_smVisionInfo.g_arrLead[intSelectedROI].ClearTempBlobsFeatures();

            //Set blobs features data into Temp Blobs Features
            m_smVisionInfo.g_arrLead[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

            //Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
            m_smVisionInfo.g_arrLead[intSelectedROI].CompareSelectedBlobs();

            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ReferenceCorner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_strPosition = cbo_ReferenceCorner.SelectedItem.ToString();

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                switch (m_strPosition)
                {
                    case "Top":
                        m_smVisionInfo.g_arrLead[j].ref_intFirstLead = 1;
                        break;
                    case "Right":
                        m_smVisionInfo.g_arrLead[j].ref_intFirstLead = 2;
                        break;
                    case "Bottom":
                        m_smVisionInfo.g_arrLead[j].ref_intFirstLead = 3;
                        break;
                    case "Left":
                        m_smVisionInfo.g_arrLead[j].ref_intFirstLead = 4;
                        break;
                }
            }

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void btn_UndoROI_Click(object sender, EventArgs e)
        {
            bool blnSelect = false;

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                {
                    blnSelect = true;
                    if (m_smVisionInfo.g_arrLeadROIs[j].Count > 1)
                    {
                        m_smVisionInfo.g_arrLeadROIs[j].RemoveAt(m_smVisionInfo.g_arrLeadROIs[j].Count - 1);
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
            }

            if (!blnSelect)
            {
                SRMMessageBox.Show("Please select ROI first!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btn_AddPitch_Click(object sender, EventArgs e)
        {
            //Define lead index
            int intLeadPitchIndex = tabCtrl_LeadPitch.SelectedIndex;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                    intLeadPitchIndex++;
                else
                {
                    if (intLeadPitchIndex == j)
                        break;
                }
            }

            int intTotalPitch = m_smVisionInfo.g_arrLead[intLeadPitchIndex].GetTotalPitchGap();
            SetPitch(intTotalPitch, 0, 0, intLeadPitchIndex);
            ReadLeadPitchGapToGrid(intLeadPitchIndex, m_dgdViewPG);
        }

        private void cbo_LeadLabelDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                switch (cbo_LeadLabelDirection.SelectedIndex)
                {
                    case 0:
                        m_smVisionInfo.g_arrLead[j].ref_blnClockWise = true;
                        break;
                    case 1:
                        m_smVisionInfo.g_arrLead[j].ref_blnClockWise = false;
                        break;
                }
            }

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void btn_DeletePitch_Click(object sender, EventArgs e)
        {
            if (m_intPitchSelectedRowIndex >= 0 && m_dgdViewPG.Rows.Count > 0)
            {
                if (SRMMessageBox.Show("Are you sure you want to delete pitch/gap number " + (m_intPitchSelectedRowIndex + 1), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //Define lead index
                    int intLeadPitchIndex = tabCtrl_LeadPitch.SelectedIndex;
                    for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
                    {
                        if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                            intLeadPitchIndex++;
                        else
                        {
                            if (intLeadPitchIndex == j)
                                break;
                        }
                    }

                    m_smVisionInfo.g_arrLead[intLeadPitchIndex].DeletePitchGap(m_intPitchSelectedRowIndex);
                    ReadLeadPitchGapToGrid(intLeadPitchIndex, m_dgdViewPG);
                }
            }
            else
            {
                SRMMessageBox.Show("Pitch/Gap row is empty already.");
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void tabCtrl_LeadPitch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            //Define lead index
            int intLeadPitchIndex = tabCtrl_LeadPitch.SelectedIndex;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                    intLeadPitchIndex++;
                else
                {
                    if (intLeadPitchIndex == j)
                        break;
                }
            }

            // Get tabControl page selected index
            int intIndex = intLeadPitchIndex;
            switch (intIndex)
            {
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
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (intIndex == i)
                {
                    m_smVisionInfo.g_arrLeadROIs[intIndex][0].VerifyROIArea(
                        m_smVisionInfo.g_arrLeadROIs[intIndex][0].ref_ROIPositionX,
                        m_smVisionInfo.g_arrLeadROIs[intIndex][0].ref_ROIPositionY);
                }
                else
                {
                    m_smVisionInfo.g_arrLeadROIs[i][0].VerifyROIArea(0, 0);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_PitchSetting_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_intPitchSelectedRowIndex = e.RowIndex;
        }

        private void dgd_PitchSetting_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 1) // Edit PitchGap "To Lead" no
            {
                int intSelectedLead = 0;
                switch (m_strPosition)
                {
                    case "Top":
                        intSelectedLead = 1;
                        break;
                    case "Right":
                        intSelectedLead = 2;
                        break;
                    case "Bottom":
                        intSelectedLead = 3;
                        break;
                    case "Left":
                        intSelectedLead = 4;
                        break;
                }

                //int intTotalLeadNo = m_smVisionInfo.g_arrLead[intSelectedLead].GetBlobsFeaturesNumber();
                List<int> arrTotalLead = m_smVisionInfo.g_arrLead[intSelectedLead].GetLeadID();
                int intFormLeadNo = Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value);
                string strToLeadNo = ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value.ToString();
                int intNoID = m_smVisionInfo.g_arrLead[intSelectedLead].GetBlobsNoID();
                SetLeadPitchGapForm objSetPitchGapForm = new SetLeadPitchGapForm(arrTotalLead, intFormLeadNo, strToLeadNo);
                if (objSetPitchGapForm.ShowDialog() == DialogResult.OK)
                {
                    bool blnAllowChange = true;
                    int intToLeadNo = 0;
                    intFormLeadNo = intFormLeadNo - intNoID + 1;
                    if (objSetPitchGapForm.ref_strToLeadNo != "NA")
                    {
                        intToLeadNo = Convert.ToInt32(objSetPitchGapForm.ref_strToLeadNo) - intNoID + 1;

                        if (intFormLeadNo == intToLeadNo)
                        {
                            SRMMessageBox.Show("Can not select same lead number!");
                            blnAllowChange = false;
                        }
                        else if (m_smVisionInfo.g_arrLead[intSelectedLead].CheckPitchGapLinkExist(intFormLeadNo - 1, intToLeadNo - 1))
                        {
                            SRMMessageBox.Show("This Pitch/Gap link already exist.");
                            blnAllowChange = false;
                        }
                        else if (m_smVisionInfo.g_arrLead[intSelectedLead].CheckPitchGapLinkInLeadAlready(intFormLeadNo - 1))
                        {
                            //SRMMessageBox.Show("Pitch/Gap already defined in pad number " + intFormPadNo);
                            //blnAllowChange = false;
                        }
                        else if (!m_smVisionInfo.g_arrLead[intSelectedLead].CheckPitchGapLinkAvailable(intFormLeadNo - 1, intToLeadNo - 1))
                        {
                            SRMMessageBox.Show("Pitch/Gap can not be created in between lead no." + intFormLeadNo + " and lead no." + intToLeadNo + ".");
                            blnAllowChange = false;
                        }
                    }
                    if (blnAllowChange)
                    {
                        m_smVisionInfo.g_arrLead[intSelectedLead].SetPitchGap(intFormLeadNo - 1, intToLeadNo - 1);
                        ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value = objSetPitchGapForm.ref_strToLeadNo;
                        ReadLeadPitchGapToGrid(intSelectedLead, m_dgdViewPG);
                    }
                }
            }

            //if (e.ColumnIndex < 2)
            //    return;

            //string m_strUnitLabel;
            //int m_intDecimalPlaces;
            //switch (m_smCustomizeInfo.g_intUnitDisplay)
            //{
            //    default:
            //    case 1:
            //        m_strUnitLabel = "mm";
            //        m_intDecimalPlaces = 4;
            //        break;
            //    case 2:
            //        m_strUnitLabel = "mil";
            //        m_intDecimalPlaces = 4;
            //        break;
            //    case 3:
            //        m_strUnitLabel = "um";
            //        m_intDecimalPlaces = 2;
            //        break;
            //}

            //int intLeadPosition = 0;
            //string strCurrentSetValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            ////SetValueForm objSetValueForm = new SetValueForm(e.RowIndex, m_intDecimalPlaces, strCurrentSetValue);
            //SetValueForm objSetValueForm = new SetValueForm("Set value to pitch/gap " + (e.RowIndex + 1).ToString(), m_strUnitLabel, e.RowIndex, m_intDecimalPlaces, strCurrentSetValue);

            ////Define lead index
            //int intLeadPitchIndex = tabCtrl_LeadPitch.SelectedIndex;
            //for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            //{
            //    if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
            //        intLeadPitchIndex++;
            //    else
            //    {
            //        if (intLeadPitchIndex == j)
            //            break;
            //    }
            //}


            ////Define Lead position
            //intLeadPosition = intLeadPitchIndex;

            //switch (intLeadPosition)
            //{
            //    case 1:
            //        m_strPosition = "Top";
            //        break;
            //    case 2:
            //        m_strPosition = "Right";
            //        break;
            //    case 3:
            //        m_strPosition = "Bottom";
            //        break;
            //    case 4:
            //        m_strPosition = "Left";
            //        break;
            //}

            ////Set pitch gap col name
            //for (int j = 0; j < m_strPGColName.Length; j++)
            //{
            //    if (intLeadPosition == 0)
            //        m_strPGRealColName[j] = m_strPGColName[j];
            //    else
            //        m_strPGRealColName[j] = m_strPGColName[j] + m_strPosition;
            //}

            //if (objSetValueForm.ShowDialog() == DialogResult.OK)
            //{
            //    int intStartRowNumber;
            //    if (objSetValueForm.ref_blnSetAllRows)
            //    {
            //        intStartRowNumber = 0;
            //    }
            //    else
            //    {
            //        intStartRowNumber = e.RowIndex;
            //    }

            //    for (int i = intStartRowNumber; i < ((DataGridView)sender).Rows.Count; i++)
            //    {
            //        if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
            //        {
            //            if (e.ColumnIndex == 2 || e.ColumnIndex == 4)
            //            {
            //                float fMax = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Value.ToString());

            //                if (objSetValueForm.ref_fSetValue > fMax)
            //                {
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Style.BackColor = Color.Pink;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Style.SelectionBackColor = Color.Pink;
            //                }
            //                else
            //                {
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Style.BackColor = Color.White;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 1].Style.SelectionBackColor = Color.White;
            //                }
            //            }
            //            else if (e.ColumnIndex == 3 || e.ColumnIndex == 5)
            //            {
            //                float fMin = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Value.ToString());

            //                if (fMin > objSetValueForm.ref_fSetValue)
            //                {
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Style.BackColor = Color.Pink;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Style.SelectionBackColor = Color.Pink;
            //                }
            //                else
            //                {
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Style.BackColor = Color.White;
            //                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 1].Style.SelectionBackColor = Color.White;
            //                }
            //            }

            //            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F" + m_intDecimalPlaces);
            //        }


            //        //// Update grid value
            //        //((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue;

            //        // Set value column selected
            //        //m_smVisionInfo.g_arrLead[intLeadPosition].SetPitchGapData(i,
            //        //    (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[2]].Value) * m_smVisionInfo.g_fCalibPixelX),
            //        //    (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[3]].Value) * m_smVisionInfo.g_fCalibPixelX),
            //        //    (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[4]].Value) * m_smVisionInfo.g_fCalibPixelX),
            //        //    (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[5]].Value) * m_smVisionInfo.g_fCalibPixelX));
            //        m_smVisionInfo.g_arrLead[intLeadPosition].SetPitchGapData(i,
            //            (Convert.ToInt32(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[0]].Value)) - 1,   // add "-1" bcos Lead index start from 0
            //            (Convert.ToInt32(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[1]].Value)) - 1,   // add "-1" bcos Lead index start from 0
            //                (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[2]].Value)),
            //                (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[3]].Value)),
            //                (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[4]].Value)),
            //                (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[5]].Value)));

            //        if (!objSetValueForm.ref_blnSetAllRows)
            //            break;
            //    }

            //    // Update grid value from database
            //    //ReadLeadPitchToGrid(tabCtrl_LeadPG.SelectedIndex, ((DataGridView)sender));

            //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //}
        }

        private void timer_Lead_Tick(object sender, EventArgs e)
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

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;

                if (m_smVisionInfo.g_intLearnStepNo == 6)
                {
                    if (tabCtrl_LeadPitch.SelectedIndex >= 0)
                    {
                        int intIndex = 0;
                        for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                continue;

                            if (m_smVisionInfo.g_arrLeadROIs[i][0].GetROIHandle())
                            {
                                // Update TabPage Display base on ROI selection
                                tabCtrl_LeadPitch.SelectedIndex = intIndex;
                                break;
                            }
                            intIndex++;
                        }
                    }
                }
            }

            if (m_objLeadLineProfileForm != null)
            {
                if (!m_objLeadLineProfileForm.ref_blnShow)
                {
                    m_smVisionInfo.g_strSelectedPage = "Lead";
                    m_objLeadLineProfileForm.Close();
                    m_objLeadLineProfileForm.Dispose();
                    m_objLeadLineProfileForm = null;
                    this.Show();
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
                else
                {
                    if (m_objLeadLineProfileForm.ref_blnBuildLead)
                    {
                        ReadAllLeadTemplateDataToGrid_DefaultMethod();
                        m_objLeadLineProfileForm.ref_blnBuildLead = false;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
            }

            if (m_objAdvancedRectGaugeForm != null)
            {
                if (!m_objAdvancedRectGaugeForm.ref_blnShowForm)
                {
                    m_objAdvancedRectGaugeForm.Close();
                    m_objAdvancedRectGaugeForm.Dispose();
                    m_objAdvancedRectGaugeForm = null;
                    btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_GaugeAdvanceSetting.Enabled = chk_ShowDraggingBox.Enabled = chk_ShowSamplePoints.Enabled = true;
                }
            }
        }

        private void btn_ROISaveClose_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                           m_smVisionInfo.g_strVisionFolderName + "\\Lead\\";

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
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

                SaveROISetting(strFolderPath);


                
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrLead[i].SaveLead(strFolderPath + "Template\\Template.xml",
                   false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead ROI", m_smProductionInfo.g_strLotID);
                
            }

            // Reload all information (User may go to last page to change setting)
            strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadROISetting(strFolderPath + "Lead\\ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead.Length);
            //LoadLeadOffSetSetting(strFolderPath + "Calibration.xml");
            LoadLeadSetting(strFolderPath + "Lead\\");
            //LoadPositioningSetting(strFolderPath + "Positioning\\");

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                      m_smVisionInfo.g_strVisionFolderName + "\\";

            // Save Orient
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY,
                    m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);
                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                XmlParser objFile = new XmlParser(strPath + "Orient\\" + "Template\\Template.xml");
                ROI objROI;
                //STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
                objFile = new XmlParser(strPath + "Orient\\" + "ROI.xml", false);
                for (int x = 0; x < m_smVisionInfo.g_arrOrientROIs.Count; x++)
                {
                    objFile.WriteSectionElement("Unit" + x);
                    for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[x].Count; j++)
                    {
                        if (j == 0)
                        {
                            objROI = m_smVisionInfo.g_arrOrientROIs[x][j];
                            if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                            {
                                ROI objSelectedROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][j];
                                objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
                                objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
                                objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
                                objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
                            }

                            objFile.WriteElement1Value("ROI" + j, "");

                            objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                            objFile.WriteElement2Value("Type", objROI.ref_intType);
                            objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                            objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                            objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                            objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                            objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                            objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
                            //float fPixelAverage = objROI.GetROIAreaPixel();
                            //objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                            //objROI.SetROIPixelAverage(fPixelAverage);

                            objFile.WriteEndElement();
                            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient ROI", strFolderPath, "ROI.xml");
                        }
                    }
                }

                CopyInfoToMark();
            }

            if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                {
                    m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0].LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY,
                       m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);
                    m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                }
                //SaveROISettings(m_strFolderPath + "Mark\\", m_smVisionInfo.g_arrMarkROIs, "Mark");
                XmlParser objFile = new XmlParser(strPath + "Mark\\" + "Template\\Template.xml");
                ROI objROI;
                //STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
                objFile = new XmlParser(strPath + "Mark\\" + "ROI.xml", false);
                for (int x = 0; x < m_smVisionInfo.g_arrMarkROIs.Count; x++)
                {
                    objFile.WriteSectionElement("Unit" + x);
                    for (int j = 0; j < m_smVisionInfo.g_arrMarkROIs[x].Count; j++)
                    {
                        if (j == 0)
                        {
                            objROI = m_smVisionInfo.g_arrMarkROIs[x][j];
                            if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                            {
                                ROI objSelectedROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][j];
                                objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
                                objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
                                objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
                                objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
                            }

                            objFile.WriteElement1Value("ROI" + j, "");

                            objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                            objFile.WriteElement2Value("Type", objROI.ref_intType);
                            objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                            objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                            objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                            objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                            objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                            objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
                            //float fPixelAverage = objROI.GetROIAreaPixel();
                            //objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                            //objROI.SetROIPixelAverage(fPixelAverage);

                            objFile.WriteEndElement();
                            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient ROI", strFolderPath, "ROI.xml");
                        }
                    }
                }
            }

            // Empty Search ROI
            if (m_smVisionInfo.g_arrPositioningROIs != null)
            {
                if (m_smVisionInfo.g_arrPositioningROIs.Count > 0)
                {
                    m_smVisionInfo.g_arrPositioningROIs[0].LoadROISetting(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY,
                        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);

                    string strEmptyFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\";
                    ROI.SaveFile(strEmptyFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
                }
            }

            //LoadMatcherFile(strFolderPath);
            m_smVisionInfo.AT_PR_AttachImagetoROI = true;
            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;
            this.Close();
            this.Dispose();
        }

        private void cbo_LeadNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || !m_blnIdentityLeadsDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                int BaseValue = m_smVisionInfo.g_arrLead[i].GetBaseInwardOffset(cbo_LeadNo.SelectedIndex);
                int TipValue = m_smVisionInfo.g_arrLead[i].GetTipInwardOffset(cbo_LeadNo.SelectedIndex);

                if (BaseValue >= 0)
                    txt_BaseOffset.Text = BaseValue.ToString();

                if (TipValue >= 0)
                    txt_TipOffset.Text = TipValue.ToString();
            }

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void chk_SetToAllLeads_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllLead", chk_SetToAllLeads.Checked);
        }

        private void btn_LineProfileGaugeSetting_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead\\PointGauge.xml";

            if (m_objLeadLineProfileForm == null)
                m_objLeadLineProfileForm = new LeadLineProfileForm(m_smVisionInfo, m_smVisionInfo.g_arrLead[0].ref_objPointGauge, strPath, m_smProductionInfo, 0);

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            m_objLeadLineProfileForm.Location = new Point(resolution.Width - m_objLeadLineProfileForm.Size.Width, resolution.Height - m_objLeadLineProfileForm.Size.Height);

            m_objLeadLineProfileForm.Show();

            m_smVisionInfo.g_strSelectedPage = "LeadLineProfileGaugeSetting";
            this.Hide();
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
        private void AttachImageToROI(List<List<ROI>> arrROIs, ImageDrawing objImage)
        {
            for (int i = 0; i < arrROIs.Count; i++)
            {
                for (int j = 0; j < arrROIs[i].Count; j++)
                {
                    ROI objROI = arrROIs[i][j];

                    switch (objROI.ref_intType)
                    {
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
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package ROI", m_smProductionInfo.g_strLotID);
                    
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
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package Gauge", m_smProductionInfo.g_strLotID);
                
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

        private void txt_ImageGain1_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fImageGain = Convert.ToSingle(txt_ImageGain1.Value);

            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
            {
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fGainValue = fImageGain * 1000;
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

        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            m_objAdvancedRectGaugeForm = new AdvancedRectGaugeM4LForm(m_smVisionInfo, m_strFolderPath + "GaugeM4L.xml", m_smProductionInfo.g_strRecipePath +
                                 m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);

            m_objAdvancedRectGaugeForm.StartPosition = FormStartPosition.Manual;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            m_objAdvancedRectGaugeForm.Location = new Point(resolution.Width - m_objAdvancedRectGaugeForm.Size.Width, resolution.Height - m_objAdvancedRectGaugeForm.Size.Height);
            m_objAdvancedRectGaugeForm.Show();

            btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_GaugeAdvanceSetting.Enabled = false;
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

        private void txt_PkgToBaseTolerance_Top_TextChanged(object sender, EventArgs e)
        {
            if ((((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && m_smVisionInfo.g_arrLead[0].ref_intRotationMethod != 2) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                return;

            if (!m_blnInitDone)//  || !m_blnIdentityLeadsDone)
                return;

            if ((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) - Convert.ToInt32(txt_PkgToBaseTolerance_Top.Text)) < m_smVisionInfo.g_arrLeadROIs[1][0].ref_ROITotalY)
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Top.Text = m_smVisionInfo.g_arrLead[0].ref_intPkgToBaseTolerance_Top.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead[1].ref_intPkgToBaseTolerance_Top = Convert.ToInt32(txt_PkgToBaseTolerance_Top.Text);

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Left_TextChanged(object sender, EventArgs e)
        {
            if ((((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && m_smVisionInfo.g_arrLead[0].ref_intRotationMethod != 2) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                return;

            if (!m_blnInitDone)//  || !m_blnIdentityLeadsDone)
                return;

            if ((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) - Convert.ToInt32(txt_PkgToBaseTolerance_Left.Text)) < (m_smVisionInfo.g_arrLeadROIs[4][0].ref_ROITotalX))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Left.Text = m_smVisionInfo.g_arrLead[0].ref_intPkgToBaseTolerance_Left.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead[4].ref_intPkgToBaseTolerance_Left = Convert.ToInt32(txt_PkgToBaseTolerance_Left.Text);

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Right_TextChanged(object sender, EventArgs e)
        {
            if ((((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && m_smVisionInfo.g_arrLead[0].ref_intRotationMethod != 2) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                return;

            if (!m_blnInitDone)// || !m_blnIdentityLeadsDone)
                return;

            if ((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X + (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) + Convert.ToInt32(txt_PkgToBaseTolerance_Right.Text)) > (m_smVisionInfo.g_arrLeadROIs[2][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[2][0].ref_ROIWidth))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Right.Text = m_smVisionInfo.g_arrLead[0].ref_intPkgToBaseTolerance_Right.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead[2].ref_intPkgToBaseTolerance_Right = Convert.ToInt32(txt_PkgToBaseTolerance_Right.Text);

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Bottom_TextChanged(object sender, EventArgs e)
        {
            if ((((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && m_smVisionInfo.g_arrLead[0].ref_intRotationMethod != 2) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                return;

            if (!m_blnInitDone)//  || !m_blnIdentityLeadsDone)
                return;

            if ((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y + (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) + Convert.ToInt32(txt_PkgToBaseTolerance_Bottom.Text)) > (m_smVisionInfo.g_arrLeadROIs[3][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[3][0].ref_ROIHeight))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Bottom.Text = m_smVisionInfo.g_arrLead[0].ref_intPkgToBaseTolerance_Bottom.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead[3].ref_intPkgToBaseTolerance_Bottom = Convert.ToInt32(txt_PkgToBaseTolerance_Bottom.Text);

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                ReadAllLeadTemplateDataToGrid_DefaultMethod();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLeadPkgToBaseDrawing = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLeadPkgToBaseDrawing = false;
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
        private void RotatePrecise()
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;
            btn_RotateAngle.Enabled = false;

            ROI objROI = new ROI();
            objROI = m_smVisionInfo.g_arrLeadROIs[0][0];

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo];
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(ref objRotatedImage);
            ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrLead[0].ref_intImageViewNo);

            if (m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
            {
                ImageDrawing objRotatedImage_BaseLead = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo];
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo].CopyTo(ref objRotatedImage_BaseLead);
                ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo);
            }
            // After rotating the image, attach the rotated image into ROI again
            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " deg";
            else
                lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " 度";
            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;
        }
        private void RotatePrecise_AfterPocketDontCare()
        {
            ImageDrawing objRotatedImage = new ImageDrawing(true);
            m_smVisionInfo.g_objPackageImage.CopyTo(ref objRotatedImage);

 
            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2)//&& m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
            {
                if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                {
                    m_intCurrentPreciseDeg = -(int)(Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle));
                    
                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(objRotatedImage);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                   (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                   m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth,
                                                   m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);
                    
                    ROI.Rotate0Degree(objRotateROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_objPackageImage);

                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    ROI.Rotate0Degree(objRotateROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);

                    objRotateROI.Dispose();

                }
                else
                {
                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(objRotatedImage);
                    objRotateROI.LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);
                    ROI.Rotate0Degree(objRotateROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_objPackageImage);

                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    ROI.Rotate0Degree(objRotateROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);

                    objRotateROI.Dispose();
                }
            }
            else
            {
                ROI objRotateROI = new ROI();
                objRotateROI.AttachImage(objRotatedImage);
                objRotateROI.LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);
                ROI.Rotate0Degree(objRotateROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_objPackageImage);

                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                ROI.Rotate0Degree(objRotateROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);

                objRotateROI.Dispose();
            }
            objRotatedImage.Dispose();
            m_smVisionInfo.g_objPackageImage.CopyTo(m_ImageWithMasking);
            m_smVisionInfo.g_fPreciseAngle = m_intCurrentAngle + m_intCurrentPreciseDeg;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_ClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg++;
            m_smVisionInfo.g_fPreciseAngle = m_intCurrentAngle + m_intCurrentPreciseDeg;
            RotatePrecise();
        }

        private void btn_CounterClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg--;
            m_smVisionInfo.g_fPreciseAngle = m_intCurrentAngle + m_intCurrentPreciseDeg;
            RotatePrecise();
        }

        private void UpdatePackageImage_ForRectGaugeM4L()
        {
            RectGaugeM4L RGaugeM4L = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
          
            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }

            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                //if (m_blnSetToAll)
                {
                    ImageDrawing objSourceImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                    for (int j = 0; j < 4; j++)
                    {
                        if ((RGaugeM4L.ref_intSelectedGaugeEdgeMask & (0x01 << j)) > 0)
                        {
                            if (RGaugeM4L.GetGaugeImageMode(j) == 0)
                            {
                                RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                if (RGaugeM4L.GetGaugeWantImageThreshold(j))
                                    RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                            }
                            else
                            {
                                //m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, j);
                                //m_RGaugeM4L.AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, j);

                                RGaugeM4L.AddHighPassForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                RGaugeM4L.AddGainForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                if (RGaugeM4L.GetGaugeWantImageThreshold(j))
                                    RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                            }
                        }
                    }
                }
                //else
                //{
                //    int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                //    ImageDrawing objSourceImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                //    if (RGaugeM4L.GetGaugeImageMode(intLineGaugeSelectedIndex) == 0)
                //    {
                //        RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                //        if (RGaugeM4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                //            RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                //    }
                //    //else
                //    //{
                //    //    m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, intLineGaugeSelectedIndex);
                //    //    m_RGaugeM4L.AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                //    //}
                //}
            }
            else
            {
                //if (m_blnSetToAll)
                {
                    ImageDrawing objSourceImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                    for (int j = 0; j < 4; j++)
                    {
                        if ((RGaugeM4L.ref_intSelectedGaugeEdgeMask & (0x01 << j)) > 0)
                        {
                            if (RGaugeM4L.GetGaugeImageMode(j) == 0)
                            {
                                RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                if (RGaugeM4L.GetGaugeWantImageThreshold(j))
                                    RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                            }
                            else
                            {
                                //m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, j);
                                //m_RGaugeM4L.AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, j);

                                RGaugeM4L.AddHighPassForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                RGaugeM4L.AddGainForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                if (RGaugeM4L.GetGaugeWantImageThreshold(j))
                                    RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                            }
                        }

                    }
                }
                //else
                //{
                //    int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                //    ImageDrawing objSourceImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                //    if (RGaugeM4L.GetGaugeImageMode(intLineGaugeSelectedIndex) == 0)
                //    {
                //        RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                //        if (RGaugeM4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                //            RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                //    }
                //    //else
                //    //{
                //    //    m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, intLineGaugeSelectedIndex);
                //    //    m_RGaugeM4L.AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                //    //}
                //}
            }

        }
        private bool FindFixPocket(int intLeadIndex)
        {
            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsFix.Count < intLeadIndex)
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Please Learn Pocket Dont Care ROI first.";
                return false;
            }

            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex].Count == 0)
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Please Learn Pocket Dont Care ROI first.";
                return false;
            }
            
            //ROI objDontCareROI = new ROI();
            m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0].LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0].ref_ROIPositionY,
                m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0].ref_ROIHeight);
            //m_smVisionInfo.g_objLeadImage.SaveImage("D:\\Img1.bmp");
            m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0].AttachImage(m_ImageWithMasking);
            ROI.SubtractROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0], m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0]);
            //m_smVisionInfo.g_objLeadImage.SaveImage("D:\\Img1.bmp");
            m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_objWhiteImage);
            //objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIPositionY, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIHeight);
            ROI.SubtractROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0], m_smVisionInfo.g_arrLeadPocketDontCareROIsFix[intLeadIndex][0]);
            //ROI.LogicOperationAddROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0], objROI);
            //m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].SaveImage("D:\\ROI.bmp");
            //objROI.SaveImage("D:\\ROI2.bmp");
            //objROI.Dispose();
            //objGaugeROI.Dispose();
            //m_smVisionInfo.g_objLeadImage.SaveImage("D:\\Img2.bmp");
            //objDontCareROI.Dispose();
            return true;

        }
        private bool FindManualPocketReference(int intLeadIndex)
        {
            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0].Count > 0)
            {
                m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            }

            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual.Count < intLeadIndex)
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Please Learn Pocket Dont Care ROI first.";
                return false;
            }

            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex].Count == 0)
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Please Learn Pocket Dont Care ROI first.";
                return false;
            }

            PointF pPatternCenter = new PointF(0, 0);
            if (!m_smVisionInfo.g_arrLead[intLeadIndex].FindManualPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[0][0], ref pPatternCenter))
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Fail to find Pocket Reference." + m_smVisionInfo.g_arrLead[intLeadIndex].ref_strErrorMessage;
                return false;
            }

            //ROI objDontCareROI = new ROI();
            m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].LoadROISetting((int)Math.Round((pPatternCenter.X - m_smVisionInfo.g_arrLead[intLeadIndex].ref_fManualPocketReferenceOffsetX) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIWidth / 2)),
                (int)Math.Round((pPatternCenter.Y - m_smVisionInfo.g_arrLead[intLeadIndex].ref_fManualPocketReferenceOffsetY) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIHeight / 2)),
                m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIHeight);
            //m_smVisionInfo.g_objLeadImage.SaveImage("D:\\Img1.bmp");
            m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].AttachImage(m_ImageWithMasking);
            ROI.SubtractROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0], m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0]);
            //m_smVisionInfo.g_objLeadImage.SaveImage("D:\\Img1.bmp");
            m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_objWhiteImage);
            //objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIPositionY, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].ref_ROIHeight);
            ROI.SubtractROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0], m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0]);
            //ROI.LogicOperationAddROI(m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0], objROI);
            //m_smVisionInfo.g_arrLeadPocketDontCareROIsManual[intLeadIndex][0].SaveImage("D:\\ROI.bmp");
            //objROI.SaveImage("D:\\ROI2.bmp");
            //objROI.Dispose();
            //objGaugeROI.Dispose();
            //m_smVisionInfo.g_objLeadImage.SaveImage("D:\\Img2.bmp");
            //objDontCareROI.Dispose();
            return true;

        }
        private bool FindPocketShadowBlob(int intLeadIndex, ref List<float> arrInwardDontCareROILimit)
        {
            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob.Count < intLeadIndex)
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Please Learn Pocket Dont Care ROI first.";
                return false;
            }

            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex].Count == 0)
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Please Learn Pocket Dont Care ROI first.";
                return false;
            }

            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex].Count > 0)
            {
                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            }

            ROI objROI = new ROI();
            objROI.AttachImage(m_smVisionInfo.g_objPackageImage);
            switch (intLeadIndex)
            {
                case 1:
                    objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROITotalX,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROITotalY,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROIWidth,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROIHeight + m_smVisionInfo.g_arrLead[intLeadIndex].ref_intDontCareBlobROIInward);
                    break;
                case 2:
                    objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROITotalX - m_smVisionInfo.g_arrLead[intLeadIndex].ref_intDontCareBlobROIInward,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROITotalY,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROIWidth + m_smVisionInfo.g_arrLead[intLeadIndex].ref_intDontCareBlobROIInward,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROIHeight);
                    break;
                case 3:
                    objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROITotalX,
                                    m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROITotalY - m_smVisionInfo.g_arrLead[intLeadIndex].ref_intDontCareBlobROIInward,
                                    m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROIWidth,
                                    m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROIHeight + m_smVisionInfo.g_arrLead[intLeadIndex].ref_intDontCareBlobROIInward);
                    break;
                case 4:
                    objROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROITotalX,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROITotalY,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROIWidth + m_smVisionInfo.g_arrLead[intLeadIndex].ref_intDontCareBlobROIInward,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[intLeadIndex][0].ref_ROIHeight);
                    break;
            }

            arrInwardDontCareROILimit[intLeadIndex] = m_smVisionInfo.g_arrLead[intLeadIndex].BuildDontCareArea(objROI, false);

            if (arrInwardDontCareROILimit[intLeadIndex] != -1f)
            {
                ROI objDontCareROI = new ROI();
                objDontCareROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                switch (intLeadIndex)
                {
                    case 1:
                        objDontCareROI.LoadROISetting(objROI.ref_ROITotalX,
                                objROI.ref_ROITotalY,
                                objROI.ref_ROIWidth,
                                (int)arrInwardDontCareROILimit[intLeadIndex]);
                        break;
                    case 2:
                        objDontCareROI.LoadROISetting(objROI.ref_ROITotalX + (int)arrInwardDontCareROILimit[intLeadIndex],
                                objROI.ref_ROITotalY,
                                objROI.ref_ROIWidth,
                                objROI.ref_ROIHeight);
                        break;
                    case 3:
                        objDontCareROI.LoadROISetting(objROI.ref_ROITotalX,
                                        objROI.ref_ROITotalY + (int)arrInwardDontCareROILimit[intLeadIndex],
                                        objROI.ref_ROIWidth,
                                        objROI.ref_ROIHeight);
                        break;
                    case 4:
                        objDontCareROI.LoadROISetting(objROI.ref_ROITotalX,
                                objROI.ref_ROITotalY,
                                (int)arrInwardDontCareROILimit[intLeadIndex],
                                objROI.ref_ROIHeight);
                        break;
                }

                ROI.SubtractROI(objDontCareROI, objDontCareROI);
                //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\Img2_" + intLeadIndex.ToString() + ".bmp");
                objDontCareROI.Dispose();
            }
            objROI.Dispose();
            return true;

        }
        private void FlipToOpposite_DontCareBlob(List<float> arrInwardDontCareROILimit)
        {
            if (((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) > 0) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) > 0) &&
                ((arrInwardDontCareROILimit[1] == -1 && arrInwardDontCareROILimit[3] == -1) || (arrInwardDontCareROILimit[1] != -1 && arrInwardDontCareROILimit[3] != -1)))
                return;
            else if (((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) > 0) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) > 0) &&
                ((arrInwardDontCareROILimit[2] == -1 && arrInwardDontCareROILimit[4] == -1) || (arrInwardDontCareROILimit[2] != -1 && arrInwardDontCareROILimit[4] != -1)))
                return;

            int intOffset = 0;

            if (((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) > 0) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) > 0) && m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceY > 0)
            {
                if (arrInwardDontCareROILimit[1] == -1)
                {
                    intOffset = (int)arrInwardDontCareROILimit[3] - m_smVisionInfo.g_arrLead[3].ref_intDontCareBlobROIInward;
                    ROI objDontCareROI = new ROI();
                    objDontCareROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                    objDontCareROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[1][0].ref_ROITotalX,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[3][0].ref_ROITotalY - m_smVisionInfo.g_arrLead[3].ref_intDontCareBlobROIInward + (int)arrInwardDontCareROILimit[3] - m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceY - intOffset - m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[1][0].ref_ROIHeight,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[1][0].ref_ROIWidth,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[1][0].ref_ROIHeight);

                    //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\ImgTop_1.bmp");
                    ROI.SubtractROI(objDontCareROI, objDontCareROI);
                    //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\ImgTop_2.bmp");
                    objDontCareROI.Dispose();
                }
                else if (arrInwardDontCareROILimit[3] == -1)
                {
                    intOffset = m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[1][0].ref_ROIHeight - (int)arrInwardDontCareROILimit[1];
                    ROI objDontCareROI = new ROI();
                    objDontCareROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                    objDontCareROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[3][0].ref_ROITotalX,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[1][0].ref_ROITotalY + (int)arrInwardDontCareROILimit[1] + m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceY + intOffset,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[3][0].ref_ROIWidth,
                                m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[3][0].ref_ROIHeight);

                    //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\ImgBottom_1.bmp");
                    ROI.SubtractROI(objDontCareROI, objDontCareROI);
                    //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\ImgBottom_2.bmp");
                    objDontCareROI.Dispose();
                }
            }
            else if (((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) > 0) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) > 0) && m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceX > 0)
            {
                if (arrInwardDontCareROILimit[2] == -1)
                {
                    intOffset = m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[4][0].ref_ROIWidth - (int)arrInwardDontCareROILimit[4];
                    ROI objDontCareROI = new ROI();
                    objDontCareROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                    objDontCareROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[4][0].ref_ROITotalX + (int)arrInwardDontCareROILimit[4] + m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceX + intOffset,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[2][0].ref_ROITotalY,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[2][0].ref_ROIWidth,
                            m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[2][0].ref_ROIHeight);

                    //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\ImgRight_1.bmp");
                    ROI.SubtractROI(objDontCareROI, objDontCareROI);
                    //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\ImgRight_2.bmp");
                    objDontCareROI.Dispose();
                }
                else if (arrInwardDontCareROILimit[4] == -1)
                {
                    intOffset = (int)arrInwardDontCareROILimit[2] - m_smVisionInfo.g_arrLead[2].ref_intDontCareBlobROIInward;
                    ROI objDontCareROI = new ROI();
                    objDontCareROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                    objDontCareROI.LoadROISetting(m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[2][0].ref_ROITotalX - m_smVisionInfo.g_arrLead[2].ref_intDontCareBlobROIInward + (int)arrInwardDontCareROILimit[2] - m_smVisionInfo.g_intLeadPocketDontCareROIBlobDistanceX - intOffset - m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[4][0].ref_ROIWidth,
                        m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[4][0].ref_ROITotalY,
                        m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[4][0].ref_ROIWidth,
                        m_smVisionInfo.g_arrLeadPocketDontCareROIsBlob[4][0].ref_ROIHeight);

                    //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\ImgLeft_1.bmp");
                    ROI.SubtractROI(objDontCareROI, objDontCareROI);
                    //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\TS\\ImgLeft_2.bmp");
                    objDontCareROI.Dispose();
                }
            }

        }
        private bool FindAutoPocketReference(int intLeadIndex, ImageDrawing objImg)
        {
            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0].Count > 0)
            {
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            }

            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto.Count < intLeadIndex)
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Please Learn Pocket Dont Care ROI first.";
                return false;
            }

            if (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex].Count == 0)
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Please Learn Pocket Dont Care ROI first.";
                return false;
            }

            PointF pPatternCenter = new PointF(0, 0);
            if (!m_smVisionInfo.g_arrLead[intLeadIndex].FindAutoPocketReferencePattern(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[0][0], ref pPatternCenter))
            {
                //m_smVisionInfo.g_strErrorMessage += "*Lead : Fail to find Pocket Reference." + m_smVisionInfo.g_arrLead[intLeadIndex].ref_strErrorMessage;
                return false;
            }

            //ROI objGaugeROI = new ROI();

            m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].AttachImage(objImg);
            m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].LoadROISetting((int)Math.Round((pPatternCenter.X - m_smVisionInfo.g_arrLead[intLeadIndex].ref_fAutoPocketReferenceOffsetX) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIWidth / 2)),
                (int)Math.Round((pPatternCenter.Y - m_smVisionInfo.g_arrLead[intLeadIndex].ref_fAutoPocketReferenceOffsetY) - (m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIHeight / 2)),
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIHeight);

            m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.SetPGaugePlace(
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIPositionX,
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIPositionY,
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIWidth,
                m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIHeight);

            int m_intWidthLimit = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIWidth;
            int m_intHeightLimit = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIHeight;
            int m_intStartX = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIPositionX;
            int m_intStartY = m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ref_ROIPositionY;

            m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].GainTo_ROIToROISamePosition(ref objImg, m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_fGaugeImageGain);
            m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].ThresholdTo_ROIToROISamePosition(ref objImg, m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_intGaugeImageThreshold);

            //if (m_smVisionInfo.g_blnViewPackageImage)
                m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.MeasurePGauge(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0], m_smVisionInfo.g_objPackageImage);
            //else
            //    m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.MeasurePGauge(m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0], m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

            ROI objDontCareROI = new ROI();
            objDontCareROI.AttachImage(m_smVisionInfo.g_objPackageImage);

            if (intLeadIndex == 1)
            {
                float intStartY = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_objLine.GetPointY(m_intStartX);
                float intEndY = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_objLine.GetPointY(m_intStartX + m_intWidthLimit);
                if (!float.IsNaN(intStartY) && !float.IsInfinity(intStartY) && !float.IsNaN(intEndY) && !float.IsInfinity(intEndY))
                {

                    objDontCareROI.LoadROISetting(m_intStartX, (int)Math.Round(intStartY + m_smVisionInfo.g_arrLead[intLeadIndex].ref_intLineOffset), m_intWidthLimit, m_smVisionInfo.g_arrLead[intLeadIndex].ref_intMaskThickness);

                }
            }
            else if (intLeadIndex == 2)
            {
                float intStartX = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_objLine.GetPointX(m_intStartY);
                float intEndX = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_objLine.GetPointX(m_intStartY + m_intHeightLimit);
                if (!float.IsNaN(intStartX) && !float.IsInfinity(intStartX) && !float.IsNaN(intEndX) && !float.IsInfinity(intEndX))
                {

                    objDontCareROI.LoadROISetting((int)Math.Round(intStartX - m_smVisionInfo.g_arrLead[intLeadIndex].ref_intLineOffset - m_smVisionInfo.g_arrLead[intLeadIndex].ref_intMaskThickness), m_intStartY, m_smVisionInfo.g_arrLead[intLeadIndex].ref_intMaskThickness, m_intHeightLimit);

                }
            }
            else if (intLeadIndex == 3)
            {
                float intStartY = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_objLine.GetPointY(m_intStartX);
                float intEndY = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_objLine.GetPointY(m_intStartX + m_intWidthLimit);
                if (!float.IsNaN(intStartY) && !float.IsInfinity(intStartY) && !float.IsNaN(intEndY) && !float.IsInfinity(intEndY))
                {

                    objDontCareROI.LoadROISetting(m_intStartX, (int)Math.Round(intStartY - m_smVisionInfo.g_arrLead[intLeadIndex].ref_intLineOffset - m_smVisionInfo.g_arrLead[intLeadIndex].ref_intMaskThickness), m_intWidthLimit, m_smVisionInfo.g_arrLead[intLeadIndex].ref_intMaskThickness);

                }
            }
            else if (intLeadIndex == 4)
            {
                float intStartX = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_objLine.GetPointX(m_intStartY);
                float intEndX = m_smVisionInfo.g_arrLead[intLeadIndex].ref_objPocketEdgeGauge.ref_objLine.GetPointX(m_intStartY + m_intHeightLimit);
                if (!float.IsNaN(intStartX) && !float.IsInfinity(intStartX) && !float.IsNaN(intEndX) && !float.IsInfinity(intEndX))
                {

                    objDontCareROI.LoadROISetting((int)Math.Round(intStartX + m_smVisionInfo.g_arrLead[intLeadIndex].ref_intLineOffset), m_intStartY, m_smVisionInfo.g_arrLead[intLeadIndex].ref_intMaskThickness, m_intHeightLimit);

                }
            }
            
            //m_smVisionInfo.g_objLeadImage.SaveImage("D:\\Img1.bmp");
            objDontCareROI.AttachImage(m_ImageWithMasking);
            ROI.SubtractROI(objDontCareROI, objDontCareROI);
            objDontCareROI.AttachImage(m_smVisionInfo.g_objPackageImage);
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_objWhiteImage);
            //objROI.LoadROISetting(objDontCareROI.ref_ROIPositionX, objDontCareROI.ref_ROIPositionY, objDontCareROI.ref_ROIWidth, objDontCareROI.ref_ROIHeight);
            ROI.SubtractROI(objDontCareROI, objDontCareROI); //ROI.LogicOperationAddROI(objDontCareROI, objROI);
            //objROI.Dispose();
            //m_smVisionInfo.g_objLeadImage.SaveImage("D:\\Img2.bmp");
            m_smVisionInfo.g_arrLeadPocketDontCareROIsAuto[intLeadIndex][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            objDontCareROI.Dispose();
            //objGaugeROI.Dispose();
            return true;

        }

        private void txt_MinArea_BaseLead_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].ref_intFilterMinArea_BaseLead = Convert.ToInt32(txt_MinArea_BaseLead.Text);
            }

            BuildObjects_BaseLead();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_UndoObjects_BaseLead_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                    continue;

                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                {
                    intSelectedROI = j;
                    break;
                }
            }

            m_smVisionInfo.g_arrLead[intSelectedROI].UndoSelectedObject_BaseLead();

            m_smVisionInfo.g_blnViewObjectsBuilded = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BuildObjects_BaseLead_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    intSelectedROI++;
                else
                    break;
            }

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                        m_smVisionInfo.g_arrLeadROIs[j][0].ClearDragHandle();
                }

                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                {
                    intSelectedROI = j;
                }
            }

            m_smVisionInfo.g_arrLead[intSelectedROI].BuildOnlyLeadObjects_BaseLead(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

            //Clear Temp Blobs Features
            m_smVisionInfo.g_arrLead[intSelectedROI].ClearTempBlobsFeatures_BaseLead();

            //Set blobs features data into Temp Blobs Features
            m_smVisionInfo.g_arrLead[intSelectedROI].SetBlobsFeaturesToTempArray_BaseLead(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

            //Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
            m_smVisionInfo.g_arrLead[intSelectedROI].CompareSelectedBlobs_BaseLead();

            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Threshold_BaseLead_Click(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_blnViewPackageImage = true;
            //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            int intSelectedROI = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    intSelectedROI++;
                else
                    break;
            }

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                        m_smVisionInfo.g_arrLeadROIs[j][0].ClearDragHandle();
                }

                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                {
                    intSelectedROI = j;
                }
            }

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead[intSelectedROI].ref_intThresholdValue_BaseLead;

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[0].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[0].ref_intThresholdValue_BaseLead);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[1].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[1].ref_intThresholdValue_BaseLead);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[2].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[2].ref_intThresholdValue_BaseLead);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[3].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[3].ref_intThresholdValue_BaseLead);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[4].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[4].ref_intThresholdValue_BaseLead);
            else
                arrrThreshold.Add(-999);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, true, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead[i].ref_intThresholdValue_BaseLead = m_smVisionInfo.g_intThresholdValue;

                        m_smVisionInfo.g_arrLead[i].BuildOnlyLeadObjects_BaseLead(m_smVisionInfo.g_arrLeadROIs[i][0]);

                        //Clear Temp Blobs Features
                        m_smVisionInfo.g_arrLead[i].ClearTempBlobsFeatures_BaseLead();

                        //Set blobs features data into Temp Blobs Features
                        m_smVisionInfo.g_arrLead[i].SetBlobsFeaturesToTempArray_BaseLead(m_smVisionInfo.g_arrLeadROIs[i][0]);

                        //Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                        m_smVisionInfo.g_arrLead[i].CompareSelectedBlobs_BaseLead();
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead[intSelectedROI].ref_intThresholdValue_BaseLead = m_smVisionInfo.g_intThresholdValue;

                    m_smVisionInfo.g_arrLead[intSelectedROI].BuildOnlyLeadObjects_BaseLead(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                    //Clear Temp Blobs Features
                    m_smVisionInfo.g_arrLead[intSelectedROI].ClearTempBlobsFeatures_BaseLead();

                    //Set blobs features data into Temp Blobs Features
                    m_smVisionInfo.g_arrLead[intSelectedROI].SetBlobsFeaturesToTempArray_BaseLead(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                    //Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                    m_smVisionInfo.g_arrLead[intSelectedROI].CompareSelectedBlobs_BaseLead();
                }
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead[intSelectedROI].ref_intThresholdValue_BaseLead;

                m_smVisionInfo.g_arrLead[intSelectedROI].BuildOnlyLeadObjects_BaseLead(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                //Clear Temp Blobs Features
                m_smVisionInfo.g_arrLead[intSelectedROI].ClearTempBlobsFeatures_BaseLead();

                //Set blobs features data into Temp Blobs Features
                m_smVisionInfo.g_arrLead[intSelectedROI].SetBlobsFeaturesToTempArray_BaseLead(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                //Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                m_smVisionInfo.g_arrLead[intSelectedROI].CompareSelectedBlobs_BaseLead();
            }

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_Reset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                m_smVisionInfo.g_arrLead[i].BuildLeadPitchLink(false);

                if (!chk_Reset.Checked)
                {
                    m_smVisionInfo.g_arrLead[i].LoadPitchGapLinkTemporary();
                }
            }

            m_strPosition = "";
            //Load template data
            for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                switch (i)
                {
                    case 1:
                        m_strPosition = "Top";
                        ReadLeadPitchGapToGrid(1, dgd_TopPGSetting);
                        break;
                    case 2:
                        m_strPosition = "Right";
                        ReadLeadPitchGapToGrid(2, dgd_RightPGSetting);
                        break;
                    case 3:
                        m_strPosition = "Bottom";
                        ReadLeadPitchGapToGrid(3, dgd_BottomPGSetting);
                        break;
                    case 4:
                        m_strPosition = "Left";
                        ReadLeadPitchGapToGrid(4, dgd_LeftPGSetting);
                        break;
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_WantShowPocketDontCareArea_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnWantShowPocketDontCareArea = chk_WantShowPocketDontCareArea.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private bool StartOrientTest()
        {
            m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
            }

            // make sure template learn
            if (m_smVisionInfo.g_arrOrients[0].Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage += "*Orient : No Template Found";
                m_smVisionInfo.g_strErrorMessage += "*Please relearn Orient.";
                return false;
            }
            
            // reset all inspection data
            for (int i = 0; i < m_smVisionInfo.g_arrOrients[0].Count; i++)
            {
                ((Orient)m_smVisionInfo.g_arrOrients[0][i]).ResetInspectionData();
            }

            float fUnitSurfaceOffsetX = 0;
            float fUnitSurfaceOffsetY = 0;
            float fUnitPRResultCenterX = 0;
            float fUnitPRResultCenterY = 0;
            float fUnitPRResultAngle = 0;

            // Use Gauge to find unit angle and rotate it to 0 deg
            if (m_smVisionInfo.g_blnWantGauge && ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
            {
                    m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[0]);

                // Add gain value to image and attached all position ROI to gain image.
                float fGaugeAngle;
                bool blnGaugeResult = false;
                //if (m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue != 1000)  // 2019 05 29 - CCENG: Enable this gain feature because hard to get unit edge sometime. Dun know why this feature disabled previously
                //{
                //    m_smVisionInfo.g_arrImages[0].AddGain(ref m_objOrientGainImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                //    m_smVisionInfo.g_arrOrientGaugeM4L[0].SetGaugePlace_BasedOnEdgeROI();
                //    blnGaugeResult = m_smVisionInfo.g_arrOrientGaugeM4L[0].Measure_WithDontCareArea(m_objOrientGainImage, m_smVisionInfo.g_objWhiteImage);
                //    fGaugeAngle = m_fOrientGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectAngle;
                //}
                //else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[0].SetGaugePlace_BasedOnEdgeROI();
                    blnGaugeResult = m_smVisionInfo.g_arrOrientGaugeM4L[0].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage);
                    fGaugeAngle = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectAngle;
                }
                if (!blnGaugeResult)
                {
                    m_smVisionInfo.g_strErrorMessage += "*Orient : " + m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_strErrorMessage;
                    m_smVisionInfo.g_strErrorMessage += "*Please relearn gauge.";
                    
                    return false;
                }
                // RotateROI has same center point with gauge measure center point.
                ROI objRotateROI = new ROI();
                objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                objRotateROI.LoadROISetting(
                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.X -
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.Y -
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth,
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);

                // Rotate unit to exact 0 degree (m_fOrientGauge used in Package)
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, fGaugeAngle, 0, ref m_smVisionInfo.g_arrRotatedImages, 0); // Clear image is not so important in Orient Matching. Use interpolation 0 to save rotation time.
                
                objRotateROI.Dispose();
            }
            else // No rect gauge
            {
                m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            }

            int intMatchCount = 0;
            m_smVisionInfo.g_intOrientResult[0] = -1;  // 0:0deg, 1:90deg, 2:180deg, 3:-90, 4:Fail
            
            if (m_smVisionInfo.g_blnWantSetTemplateBasedOnBinInfo)
            {
                float fHighestScore = -1;
                m_smVisionInfo.g_arrMarks[0].ref_blnInspectAllTemplate = true;
                int intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
                if (intTemplateIndex >= 0)
                {
                    int intAngle;
                    if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                    {
                        m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                        intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.


                        if (m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore() > fHighestScore)
                        {
                            fHighestScore = m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore();
                            m_smVisionInfo.g_intSelectedOcv[0] = intTemplateIndex;
                            m_smVisionInfo.g_intOrientResult[0] = intAngle;

                        }
                    }
                }
            }
            else
            {

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
                                ref blnPreciseAngleResult, ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0),
                                false);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
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
                                m_smVisionInfo.g_intSelectedOcv[0] = intTemplateIndex;
                                m_smVisionInfo.g_intOrientResult[0] = intAngle;

                            }
                        }
                    }
                    intMatchCount++;
                    //} while (((fHighestScore < 0.8) && (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count)) || !blnAngleResult); 
                } while (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count);
            }

            Orient objOrient = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]];

            m_smVisionInfo.g_fOrientCenterX[0] = objOrient.ref_fObjectX;
            m_smVisionInfo.g_fOrientCenterY[0] = objOrient.ref_fObjectY;
            m_smVisionInfo.g_fSubOrientCenterX[0] = objOrient.ref_fSubObjectX;
            m_smVisionInfo.g_fSubOrientCenterY[0] = objOrient.ref_fSubObjectY;
            m_smVisionInfo.g_fOrientScore[0] = objOrient.GetMinScore();
            m_smVisionInfo.g_fOrientAngle[0] = objOrient.ref_fDegAngleResult;
            m_smVisionInfo.g_blnViewOrientObject = true;

            if (true)
            {
                if (m_smVisionInfo.g_intOrientResult[0] < 4)
                {
                    // This corner position orientation checking will only do if Orientation is ON.
                    if (objOrient.ref_blnWantUsePositionCheckOrientation)
                    {
                        // 2020 11 20 - CCENG: Method 2 orientation
                        float fOrientationSeparatorX = (float)m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2;
                        float fOrientationSeparatorY = (float)m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2;

                        for (int m = 0; m < objOrient.ref_arrMatchScoreList.Count; m++)
                        {
                            if (m == m_smVisionInfo.g_intOrientResult[0])
                                continue;

                            if (objOrient.ref_arrMatchScoreList[m] >= objOrient.ref_fMinScore)
                            {
                                if (Math.Abs(objOrient.GetMinScore() - objOrient.ref_arrMatchScoreList[m]) < objOrient.ref_fCheckPositionOrientationWhenBelowDifferentScore)
                                {
                                    if (objOrient.ref_intDirections == 2)
                                    {
                                        bool blnSampleIsTop = objOrient.ref_fObjectY - fOrientationSeparatorY < 0;
                                        bool blnDirectionisTop = objOrient.ref_blnTemplateOrientationIsTop != blnSampleIsTop;

                                        if (blnDirectionisTop)            // rotate 180  from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 2;
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 0;            // no rotate
                                        }
                                    }
                                    else
                                    {
                                        bool blnSampleIsLeft = objOrient.ref_fObjectX - fOrientationSeparatorX < 0;
                                        bool blnSampleIsTop = objOrient.ref_fObjectY - fOrientationSeparatorY < 0;

                                        bool blnDirectionIsLeft = objOrient.ref_blnTemplateOrientationIsLeft != blnSampleIsLeft;
                                        bool blnDirectionisTop = objOrient.ref_blnTemplateOrientationIsTop != blnSampleIsTop;

                                        if (blnDirectionIsLeft && blnDirectionisTop)            // rotate 180  from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 2;
                                        }
                                        else if (blnDirectionIsLeft && !blnDirectionisTop)      // rotate 90 ccw from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 1;            // show result angle 90 when rotate ccw from template point
                                        }
                                        else if (!blnDirectionIsLeft && blnDirectionisTop)      // rotate 90 cw from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 3;            // show result angle -90 when rotate cw from template point
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 0;            // no rotate
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    m_intCurrentAngle = 0;
                    switch (m_smVisionInfo.g_intOrientResult[0])
                    {
                        default:
                        case 0:
                            m_intCurrentAngle = 0;
                            break;
                        case 1:
                            m_intCurrentAngle = 90;
                            break;
                        case 2:
                            m_intCurrentAngle = 180;
                            break;
                        case 3:
                            m_intCurrentAngle = -90;
                            break;
                    }
                
                    float fTotalRotateAngle = 0;
                    if (m_smVisionInfo.g_blnWantGauge)
                    {
                        RectGaugeM4L objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0];

                        // Get Orient Center Point (Final result for next MarkTest and PackageTest)
                        m_smVisionInfo.g_fUnitCenterX[0] = objGauge.ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX;
                        m_smVisionInfo.g_fUnitCenterY[0] = objGauge.ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY;

                        // Calculate total angle 
                        fTotalRotateAngle = m_intCurrentAngle + objGauge.ref_fRectAngle;
                        m_intCurrentPreciseDeg = -(int)(Math.Round(objGauge.ref_fRectAngle));
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " deg";
                        else
                            lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " 度";
                        
                    }
                    else
                    {
                        // For Lead Unit Case
                        if (m_smVisionInfo.g_arrOrientROIs[0].Count > 3 &&
                           (((m_smCustomizeInfo.g_intWantPositioningIndex & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                           (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)))    // Mean Unit Surface ROI exist
                        {
                            // Get Unit Surface Center Point (Final result for next MarkTest and PackageTest)
                            m_smVisionInfo.g_fUnitCenterX[0] = fUnitPRResultCenterX + fUnitSurfaceOffsetX;
                            m_smVisionInfo.g_fUnitCenterY[0] = fUnitPRResultCenterY + fUnitSurfaceOffsetY;

                            // Calculate total angle 
                            fTotalRotateAngle = m_intCurrentAngle + fUnitPRResultAngle;
                            m_intCurrentPreciseDeg = -(int)(Math.Round(fUnitPRResultAngle));
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " deg";
                            else
                                lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " 度";
                            
                        }
                        else // For No Lead Case
                        {
                            // Calculate total angle 
                            fTotalRotateAngle = m_intCurrentAngle + m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult;
                            m_intCurrentPreciseDeg = -(int)(Math.Round(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult));
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " deg";
                            else
                                lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " 度";
                            
                        }
                    }
                }
            }

            if (m_smVisionInfo.g_intOrientResult[0] == 4)
            {
                if (m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage == "" && m_smVisionInfo.g_strErrorMessage == "")
                {
                    m_smVisionInfo.g_strErrorMessage = "*Recipe is corrupted. Please relearn.";
                }
                else
                {
                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage;
                }

                return false;
            }
            return true;
        }
        private void RotateAccordingToUnitPattern()
        {

            if (m_smVisionInfo.g_blnWantPocketDontCareAreaFix_Lead)
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {

                    if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIFixMask & 0x01) == 0))
                        continue;
                    else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIFixMask & 0x02) == 0))
                        continue;
                    else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIFixMask & 0x04) == 0))
                        continue;
                    else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIFixMask & 0x08) == 0))
                        continue;

                    if (!FindFixPocket(i))
                    {

                        //break;//2021-09-12 ZJYEOH : Need to continue find other even one of it fail
                    }
                }
                //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\g_objPackageImage2.bmp");
                //m_ImageWithMasking.SaveImage("D:\\m_ImageWithMasking.bmp");
            }
            else if (m_smVisionInfo.g_blnWantPocketDontCareAreaManual_Lead)
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {

                    if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x01) == 0))
                        continue;
                    else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x02) == 0))
                        continue;
                    else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x04) == 0))
                        continue;
                    else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIManualMask & 0x08) == 0))
                        continue;

                    if (!FindManualPocketReference(i))
                    {

                        //break;//2021-09-12 ZJYEOH : Need to continue find other even one of it fail
                    }
                }
                //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\g_objPackageImage2.bmp");
                //m_ImageWithMasking.SaveImage("D:\\m_ImageWithMasking.bmp");
            }
            else if (m_smVisionInfo.g_blnWantPocketDontCareAreaAuto_Lead)
            {
                ImageDrawing objImg = new ImageDrawing(true);
                //m_smVisionInfo.g_objPackageImage.CopyTo(objImg);
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(objImg);
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {

                    if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x01) == 0))
                        continue;
                    else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x02) == 0))
                        continue;
                    else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x04) == 0))
                        continue;
                    else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIAutoMask & 0x08) == 0))
                        continue;

                    if (!FindAutoPocketReference(i, objImg))
                    {

                        //break;//2021-09-12 ZJYEOH : Need to continue find other even one of it fail
                    }
                }
                objImg.Dispose();
                //m_smVisionInfo.g_objPackageImage.SaveImage("D:\\g_objPackageImage2.bmp");
                //m_ImageWithMasking.SaveImage("D:\\m_ImageWithMasking.bmp");
            }
            else if (m_smVisionInfo.g_blnWantPocketDontCareAreaBlob_Lead)
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);

                m_smVisionInfo.g_arrInwardDontCareROIBlobLimit = new List<float>(5) { 0, 0, 0, 0, 0 };

                for (int i = 1; i < m_smVisionInfo.g_arrLead.Length; i++)
                {

                    if ((i == 1) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x01) == 0))
                        continue;
                    else if ((i == 2) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x02) == 0))
                        continue;
                    else if ((i == 3) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x04) == 0))
                        continue;
                    else if ((i == 4) && ((m_smVisionInfo.g_intLeadPocketDontCareROIBlobMask & 0x08) == 0))
                        continue;

                    if (!FindPocketShadowBlob(i, ref m_smVisionInfo.g_arrInwardDontCareROIBlobLimit))
                    {

                        //break;//2021-09-12 ZJYEOH : Need to continue find other even one of it fail
                    }
                }

                if (m_smVisionInfo.g_arrLead[0].ref_blnFlipToOppositeFunction && m_smVisionInfo.g_arrInwardDontCareROIBlobLimit.Contains(-1))
                {
                    FlipToOpposite_DontCareBlob(m_smVisionInfo.g_arrInwardDontCareROIBlobLimit);
                }

            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_ImageWithMasking);
            }


            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (i == 0)
                    m_smVisionInfo.g_arrLead[i].ResetInspectionData();

                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                m_smVisionInfo.g_arrLead[i].ref_blnLock = true;

                // Reset previous inspection data
                m_smVisionInfo.g_arrLead[i].ResetInspectionData();

                // Identify Lead definition for displaying fail message
                string strPosition = GetLeadDefinition(i);

                //Find unit
                if (!FindUnit(i, m_smVisionInfo.g_arrLead[i].ref_intLeadAngleTolerance))
                {
                    m_smVisionInfo.g_strErrorMessage = strPosition + m_smVisionInfo.g_strErrorMessage;

                }
            }

            if (!m_blnWantPackage && (m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 0) && m_fLeadPatternAngle > 0)
            {
                m_intCurrentPreciseDeg = -(int)Math.Round(m_fLeadPatternAngle, 0, MidpointRounding.AwayFromZero);
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " deg";
                else
                    lbl_AnglePrecise.Text = m_intCurrentPreciseDeg.ToString() + " 度";

                RotatePrecise();
            }
        }
        private bool FindUnit(int intLeadIndex, int intAngleTolerance)
        {
            if (m_smVisionInfo.g_arrLeadROIs[0].Count > 0)
            {
                m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_ImageWithMasking);
            }
            else
            {
                m_smVisionInfo.g_strErrorMessage += "*Lead : No Template Found.";

                return false;
            }

            if (m_smVisionInfo.g_arrLeadROIs[intLeadIndex].Count > 0)
            {
                m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].AttachImage(m_ImageWithMasking);
            }
            else
            {
                m_smVisionInfo.g_strErrorMessage += "*Lead : No Template Found.";

                return false;
            }

            if (!m_smVisionInfo.g_arrLead[intLeadIndex].FindUnitUsingPRS(m_smVisionInfo.g_arrLeadROIs[0][0], intAngleTolerance, false, intLeadIndex, m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0])) // Side Lead angle is a bit only
            {
                m_smVisionInfo.g_strErrorMessage += "*Lead : Fail to find unit." + m_smVisionInfo.g_arrLead[intLeadIndex].ref_strErrorMessage;

                return false;
            }
            if (m_smVisionInfo.g_arrLead[intLeadIndex].ref_fUnitScore >= m_fLeadPatternScore)
            {
                m_fLeadPatternScore = m_smVisionInfo.g_arrLead[intLeadIndex].ref_fUnitScore;
                m_fLeadPatternAngle = m_smVisionInfo.g_arrLead[intLeadIndex].ref_fUnitAngle;
            }
            return true;
        }

        private void cbo_SelectROI_Lead_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intSelectedROI = m_smVisionInfo.g_intSelectedROI;
            switch (cbo_SelectROI_Lead.SelectedItem.ToString())
            {
                case "Top ROI":
                    intSelectedROI = 1;
                    break;
                case "Right ROI":
                    intSelectedROI = 2;
                    break;
                case "Bottom ROI":
                    intSelectedROI = 3;
                    break;
                case "Left ROI":
                    intSelectedROI = 4;
                    break;
            }

            SelectLeadROI(intSelectedROI);
        }
        private void SelectLeadROI(int intSelectedROI)
        {

            if (m_smVisionInfo.g_blnLeadInspected)
            {
                for (int k = 1; k < m_smVisionInfo.g_arrInspectLeadROI.Length; k++)
                {
                    if (!m_smVisionInfo.g_arrLead[k].ref_blnSelected)
                        continue;

                    if (k == intSelectedROI)
                    {
                        m_smVisionInfo.g_arrInspectLeadROI[k].ClearDragHandle();
                        m_smVisionInfo.g_arrInspectLeadROI[k].VerifyROIArea(m_smVisionInfo.g_arrInspectLeadROI[k].ref_ROITotalCenterX, m_smVisionInfo.g_arrInspectLeadROI[k].ref_ROITotalCenterY);

                        m_smVisionInfo.g_intSelectedROI = k;
                        m_smVisionInfo.g_blnUpdateSelectedROI = true;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                        m_smVisionInfo.g_intSelectedUnit = 0;

                        if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        {
                            m_smVisionInfo.g_intSelectedROIMask |= (0x01 << k);
                        }
                        else
                        {
                            m_smVisionInfo.g_intSelectedROIMask = (0x01 << k);
                        }
                    }
                    else
                        m_smVisionInfo.g_arrInspectLeadROI[k].ClearDragHandle();
                }
            }
            else
            {
                for (int k = 0; k < m_smVisionInfo.g_arrLeadROIs.Count; k++)
                {
                    if (!m_smVisionInfo.g_arrLead[k].ref_blnSelected)
                        continue;

                    int intMaxIndex = m_smVisionInfo.g_arrLeadROIs[k].Count - 1;
                    if (intMaxIndex > 1)
                        intMaxIndex = 1;

                    for (int j = intMaxIndex; j >= 0; j--)
                    {
                        if (k == intSelectedROI)
                        {
                            m_smVisionInfo.g_arrLeadROIs[k][j].ClearDragHandle();
                            m_smVisionInfo.g_arrLeadROIs[k][j].VerifyROIArea(m_smVisionInfo.g_arrLeadROIs[k][j].ref_ROITotalCenterX, m_smVisionInfo.g_arrLeadROIs[k][j].ref_ROITotalCenterY);
                            m_smVisionInfo.g_intSelectedROI = k;
                            m_smVisionInfo.g_blnUpdateSelectedROI = true;
                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                            m_smVisionInfo.g_intSelectedUnit = 0;

                            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                            {
                                m_smVisionInfo.g_intSelectedROIMask |= (0x01 << k);
                            }
                            else
                            {
                                m_smVisionInfo.g_intSelectedROIMask = (0x01 << k);
                            }
                        }
                        else
                            m_smVisionInfo.g_arrLeadROIs[k][j].ClearDragHandle();
                    }

                }
            }
        }
    }
}
