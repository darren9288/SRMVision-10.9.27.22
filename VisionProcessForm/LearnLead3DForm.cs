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

namespace VisionProcessForm
{
    public partial class LearnLead3DForm : Form
    {

        #region Member Variables
        private int m_intSelectedDontCareROIIndexPrev = 0;
        private int m_intCurrentPreciseDeg = 0;
        private bool m_blnDragROIPrev = false;
        private bool m_blnInitDone = false;
        private bool m_blnKeepPrevObject = false;
        private bool m_blnIdentityLeadsDone = false;
        private int m_intUserGroup = 5;
        private int m_intDisplayStepNo = 1;
        private int m_intCurrentAngle = 0;
        private float m_fPreRotatedDegree = 0;
        private string m_strSelectedRecipe;
        private string m_strPosition = "";
        private List<ImageDrawing> m_arrLocalBackupRotatedImage = new List<ImageDrawing>();
        private List<ImageDrawing> m_arrCurrentImage = new List<ImageDrawing>();

        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private int m_intPitchSelectedRowIndex = 0;
        private DataGridView m_dgdViewPG = new DataGridView();
        private string[] m_strPGRealColName = new string[4];
        private string[] m_strPGColName = { "column_FromLeadNo", "column_ToLeadNo", "column_MinSetPitch", "column_MaxSetPitch"};
        
        Lead3DLineProfileForm m_objLead3DLineProfileForm;
        #endregion

        public LearnLead3DForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;
            
            DisableField2();
            CustomizeGUI();
            
            m_blnInitDone = true;
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
                    m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Template\\OriTemplate.bmp"))
                chk_UsePreTemplateImage.Enabled = false;
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Lead3D";
            string strChild2 = "Learn Page";
            string strChild3 = "Save Button";

            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                btn_ROISaveClose.Enabled = false;
                btn_Save.Enabled = false;
            }

            strChild3 = "Select Direction";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                group_SelectDirection.Enabled = false;
            }

            strChild3 = "Lead Count";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                gb_LeadCount.Enabled = false;
            }

            strChild3 = "Dont Care Area Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                cbo_DontCareAreaDrawMethod.Enabled = false;
                btn_Undo.Enabled = false;
                btn_AddDontCareROI.Enabled = false;
                btn_DeleteDontCareROI.Enabled = false;
            }

            strChild3 = "Lead Threshold";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                gb_Threshold.Enabled = false;
            }

            strChild3 = "Object Selection";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                gb_ObjectSelection.Enabled = false;
            }

            strChild3 = "Lead Labeling";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                gb_LeadLabeling.Enabled = false;
            }

            strChild3 = "Lead Measure Edge Tool Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                btn_LineProfileGaugeSetting.Enabled = false;
            }

            strChild3 = "Lead Inward Offset Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                txt_BaseOffset.Enabled = false;
                txt_TipOffset.Enabled = false;
                txt_TipOffsetSide.Enabled = false;
                txt_BaseLineTrimFromEdge.Enabled = false;
                txt_BaseLineSteps.Enabled = false;
            }

            strChild3 = "Tip Build Area Tolerance";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                group_TipBuildAreaTolerance.Enabled = false;
            }

            strChild3 = "Package To Base Tolerance";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                group_PkgToBaseToleranceSetting.Enabled = false;
            }

            strChild3 = "Pitch Gap Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                chk_Reset.Enabled = false;
                dgd_PitchGapSetting.ReadOnly = true;
                dgd_TopPGSetting.ReadOnly = true;
                dgd_RightPGSetting.ReadOnly = true;
                dgd_BottomPGSetting.ReadOnly = true;
                dgd_LeftPGSetting.ReadOnly = true;
            }

        }

        private void AddSearchROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (arrROIs[i].Count == 0)
                {
                    if (i == 0)
                    {
                        objROI = new ROI("Search ROI", 1);
                        int intPositionX = (640 / 2) - 100;
                        int intPositionY = (480 / 2) - 100;
                        objROI.LoadROISetting(intPositionX, intPositionY, 200, 200);
                    }
                    else if (i == 1)
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

                    objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                    arrROIs[i].Add(objROI);
                }
            }
        }

        private void AddLeadROIToPin1SearchROI(List<List<ROI>> arrROIs)
        {
            if (m_smVisionInfo.g_arrPin1.Count == 0)
                m_smVisionInfo.g_arrPin1.Add(new Pin1());

            if (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI == null)
                m_smVisionInfo.g_arrPin1[0].ref_objSearchROI = new ROI();

            ROI objSearchROI = m_smVisionInfo.g_arrPin1[0].ref_objSearchROI;
            arrROIs[0][0].CopyTo(ref objSearchROI);

            m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
        }

        private void CustomizeGUI()
        {
            if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0)
            {
                radioBtn_Horizontal.Checked = true;
            }
            else
            {
                radioBtn_Vertical.Checked = true;
                radioBtn_Vertical.PerformClick();
            }

            if (radioBtn_Horizontal.Checked)
            {
                pic_LeadCount.BackgroundImage = pic_DirectionHorizontal.BackgroundImage;

                txt_LeadNumber_Top.Visible = false;
                txt_LeadNumber_Bottom.Visible = false;
                txt_LeadNumber_Left.Visible = true;
                txt_LeadNumber_Right.Visible = true;
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    m_smVisionInfo.g_arrLead3D[i].ref_intLeadDirection = 1;
                }

                pic_LeadCount.BackgroundImage = pic_DirectionVertical.BackgroundImage;

                txt_LeadNumber_Top.Visible = true;
                txt_LeadNumber_Bottom.Visible = true;

                txt_LeadNumber_Left.Visible = false;
                txt_LeadNumber_Right.Visible = false;
            }

            //Global setting (same for all lead)
            m_smVisionInfo.g_blnCutMode = radioBtn_CutObj.Checked;
            txt_MinArea.Text = m_smVisionInfo.g_arrLead3D[0].ref_intFilterMinArea.ToString();
            txt_BaseOffset.Text = m_smVisionInfo.g_arrLead3D[0].ref_intBaseOffset.ToString();
            txt_TipOffset.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipOffset.ToString();
            txt_TipOffsetSide.Text = m_smVisionInfo.g_arrLead3D[1].ref_intTipOffset.ToString();
            txt_BaseLineTrimFromEdge.Text = m_smVisionInfo.g_arrLead3D[0].ref_intBaseLineTrimFromEdge.ToString();
            txt_BaseLineSteps.Text = m_smVisionInfo.g_arrLead3D[0].ref_intBaseLineSteps.ToString();

            //txt_CornerTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intCornerSearchingTolerance_Top.ToString();
            //txt_CornerTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intCornerSearchingTolerance_Right.ToString();
            //txt_CornerTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intCornerSearchingTolerance_Bottom.ToString();
            //txt_CornerTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intCornerSearchingTolerance_Left.ToString();
            //txt_CornerLength.Text = m_smVisionInfo.g_arrLead3D[0].ref_intCornerSearchingLength.ToString();

            txt_PkgToBaseTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Top.ToString();
            txt_PkgToBaseTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Bottom.ToString();
            txt_PkgToBaseTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Left.ToString();
            txt_PkgToBaseTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Right.ToString();

            txt_TipBuildAreaTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top.ToString();
            txt_TipBuildAreaTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Bottom.ToString();
            txt_TipBuildAreaTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left.ToString();
            txt_TipBuildAreaTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Right.ToString();

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnClockWise)
                cbo_LeadLabelDirection.SelectedIndex = 0;
            else
                cbo_LeadLabelDirection.SelectedIndex = 1;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                switch (i)
                {
                    case 0:
                        //// Assign number of lead from side ROI to center ROI lead to ensure both side of lead numbers area tally.
                        //// For center ROI, intNumberOfLead will keep total lead of 4 side ROI.
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead = m_smVisionInfo.g_arrLead3D[1].ref_intNumberOfLead +
                        //                                                    m_smVisionInfo.g_arrLead3D[2].ref_intNumberOfLead +
                        //                                                    m_smVisionInfo.g_arrLead3D[3].ref_intNumberOfLead +
                        //                                                    m_smVisionInfo.g_arrLead3D[4].ref_intNumberOfLead;
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = m_smVisionInfo.g_arrLead3D[1].ref_intNumberOfLead;
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = m_smVisionInfo.g_arrLead3D[2].ref_intNumberOfLead;
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = m_smVisionInfo.g_arrLead3D[3].ref_intNumberOfLead;
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = m_smVisionInfo.g_arrLead3D[4].ref_intNumberOfLead;

                        //2020-08-19 ZJYEOH : should assign value to center lead only
                        txt_LeadNumber_Top.Text = m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top.ToString();
                        txt_LeadNumber_Right.Text = m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right.ToString();
                        txt_LeadNumber_Bottom.Text = m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom.ToString();
                        txt_LeadNumber_Left.Text = m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left.ToString();
                        break;
                    //case 1:
                    //    txt_LeadNumber_Top.Text = m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead.ToString();
                    //    break;
                    //case 2:
                    //    txt_LeadNumber_Right.Text = m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead.ToString();
                    //    break;
                    //case 3:
                    //    txt_LeadNumber_Bottom.Text = m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead.ToString();
                    //    break;
                    //case 4:
                    //    txt_LeadNumber_Left.Text = m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead.ToString();
                    //    break;
                }

            }

            UpdateReferenceCornerGUI();

            if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0) // Horizontal
            {
                srmLabel63.Visible = false;
                txt_PkgToBaseTolerance_Top.Visible = false;
                srmLabel52.Visible = false;
                txt_TipBuildAreaTolerance_Top.Visible = false;

                srmLabel62.Visible = false;
                txt_PkgToBaseTolerance_Bottom.Visible = false;
                srmLabel35.Visible = false;
                txt_TipBuildAreaTolerance_Bottom.Visible = false;
            }
            else
            {
                srmLabel60.Visible = false;
                txt_PkgToBaseTolerance_Left.Visible = false;
                srmLabel31.Visible = false;
                txt_TipBuildAreaTolerance_Left.Visible = false;

                srmLabel61.Visible = false;
                txt_PkgToBaseTolerance_Right.Visible = false;
                srmLabel34.Visible = false;
                txt_TipBuildAreaTolerance_Right.Visible = false;
            }

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance)
            {
                //srmLabel5.Visible = txt_BaseOffset.Visible = srmLabel14.Visible = false;
                group_TipBuildAreaTolerance.Visible = true;
                group_PkgToBaseToleranceSetting.Visible = true;
            }
            else
            {
                //srmLabel5.Visible = txt_BaseOffset.Visible = srmLabel14.Visible = true;
                group_TipBuildAreaTolerance.Visible = false;
                group_PkgToBaseToleranceSetting.Visible = false;
            }
            cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

            cbo_SelectROI.SelectedIndex = m_smVisionInfo.g_intSelectedROI = 0;

        }
        private void UpdateReferenceCornerGUI()
        {
            cbo_ReferenceCorner.Items.Clear();

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                    continue;

                switch (i)
                {
                    case 1:
                        cbo_ReferenceCorner.Items.Add("Top Left");
                        break;
                    case 2:
                        cbo_ReferenceCorner.Items.Add("Top Right");
                        break;
                    case 3:
                        cbo_ReferenceCorner.Items.Add("Bottom Left");
                        break;
                    case 4:
                        cbo_ReferenceCorner.Items.Add("Bottom Right");
                        break;
                }
            }

            if (cbo_ReferenceCorner.Items.Count == 0)
                return;

            switch (m_smVisionInfo.g_arrLead3D[0].ref_intFirstLead)
            {
                case 0:
                case 1:
                    if(cbo_ReferenceCorner.Items.Contains("Top Left"))
                        cbo_ReferenceCorner.SelectedItem = "Top Left";
                    else
                        cbo_ReferenceCorner.SelectedIndex = 0;
                    break;
                case 2:
                    if (cbo_ReferenceCorner.Items.Contains("Top Right"))
                        cbo_ReferenceCorner.SelectedItem = "Top Right";
                    else
                        cbo_ReferenceCorner.SelectedIndex = 0;

                    break;
                case 3:
                    if (cbo_ReferenceCorner.Items.Contains("Bottom Left"))
                        cbo_ReferenceCorner.SelectedItem = "Bottom Left";
                    else
                        cbo_ReferenceCorner.SelectedIndex = 0;
                    break;
                case 4:
                    if (cbo_ReferenceCorner.Items.Contains("Bottom Right"))
                        cbo_ReferenceCorner.SelectedItem = "Bottom Right";
                    else
                        cbo_ReferenceCorner.SelectedIndex = 0;
                    break;
            }

            m_strPosition = cbo_ReferenceCorner.SelectedItem.ToString();
        }

        private void LoadLead3DSetting(string strPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i == 0)
                {
                    m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelX,
                                                  m_smVisionInfo.g_fCalibPixelY,
                                                  m_smVisionInfo.g_fCalibOffSetX,
                                                  m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelZ,
                                                  m_smVisionInfo.g_fCalibPixelZ,
                                                  m_smVisionInfo.g_fCalibOffSetZ,
                                                  m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                }
                // Load Lead Template Setting
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

                m_smVisionInfo.g_arrLead3D[i].LoadLead3D(strPath + "Template\\Template.xml", strSectionName);

                m_smVisionInfo.g_arrLead3D[i].LoadLeadTemplateImage(strPath + "Template\\", i);
                if (i == 0)
                    m_smVisionInfo.g_arrLead3D[i].LoadUnitPattern(strPath + "Template\\PatternMatcher0.mch");
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
        private void SaveLeadSetting(string strFolderPath)
        {
            SaveROISetting(strFolderPath);
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Lead3D\\";
            

            if (File.Exists(strFolderPath + "Template\\OriTemplate.bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3D", "OriTemplate.bmp", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3D", "", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "Old\\");
         
            // Save Learn Actual Image
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

            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "New\\");

            if (m_smVisionInfo.g_arrLead3D.Length > 0)
            {
                m_smVisionInfo.g_arrLead3D[0].SavePattern(strFolderPath + "Template\\",
                                                                    m_smVisionInfo.g_arrRotatedImages[0],
                                                                    m_smVisionInfo.g_arrLeadROIs[0], 0);

                m_smVisionInfo.g_arrLead3D[0].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrLeadROIs[0][0]);
                m_smVisionInfo.g_arrLead3D[0].SetTemplateMatcherCenterPoint(m_smVisionInfo.g_arrLeadROIs[0][0]);
                //m_smVisionInfo.g_arrLead3D[0].BuildLeadDistance_PatternMatch(m_smVisionInfo.g_arrLeadROIs[0][0]);


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

                //
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrLead3D[i].SaveLead3D(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3D", m_smProductionInfo.g_strLotID);

                // Save Learn Template Search ROI image, unit image
                m_smVisionInfo.g_arrLead3D[i].SaveLeadTemplateImage(strFolderPath + "Template\\",
                                                                m_smVisionInfo.g_arrRotatedImages[0],
                                                                m_smVisionInfo.g_arrLeadROIs[i], i);

            }

            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (m_smVisionInfo.g_arrPin1 != null)
                {
                    float fOffsetRefPosX = 0, fOffsetRefPosY = 0;
                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    {
                        fOffsetRefPosX = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX -
                                               m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROICenterX;

                        fOffsetRefPosY = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY -
                                            m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROICenterY;
                    }
                    else
                    {
                        fOffsetRefPosX = m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX -
                                               m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROICenterX;

                        fOffsetRefPosY = m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY -
                                            m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROICenterY;
                    }

                    ROI objPin1ROI = m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI;
                    // Load Pin 1
                    m_smVisionInfo.g_arrPin1[0].LearnTemplate(m_smVisionInfo.g_intSelectedTemplate, fOffsetRefPosX, fOffsetRefPosY, objPin1ROI);

                    //
                    STDeviceEdit.CopySettingFile(strFolderPath + "Template\\", "Pin1Template.xml");
                    m_smVisionInfo.g_arrPin1[0].SaveTemplate(strFolderPath + "Template\\");
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3D Pin1", m_smProductionInfo.g_strLotID);
                    
                }
            }
            
            if (m_smVisionInfo.g_blnWantDontCareArea_Lead3D)
            {
                SaveDontCareROISetting(strFolderPath);
                SaveDontCareSetting(strFolderPath + "Template\\");
            }
        }
        private void SaveDontCareROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "DontCareROI.xml", false);
            
            for (int t = 0; t < m_smVisionInfo.g_arrLead3DDontCareROIs.Count; t++)
            {
                if (!m_smVisionInfo.g_arrLead3D[t].ref_blnSelected)
                    continue;

                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrLead3DDontCareROIs[t].Count; j++)
                {
                    objFile.WriteElement1Value("ROI" + j, "");

                    objFile.WriteElement2Value("Name", m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].ref_strROIName);
                    objFile.WriteElement2Value("Type", m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].ref_intType);
                    objFile.WriteElement2Value("PositionX", m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].ref_ROIWidth);
                    objFile.WriteElement2Value("Height", m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].ref_intStartOffsetY);
                    m_smVisionInfo.g_arrLead3DDontCareROIs[t][j].AttachImage(m_smVisionInfo.g_arrLeadROIs[t][0]);

                }
            }
            objFile.WriteEndElement();


        }
        private void SaveDontCareSetting(string strFolderPath)
        {
            ImageDrawing objImage = new ImageDrawing();
            ImageDrawing objFinalImage = new ImageDrawing();
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_objBlackImage.CopyTo(objFinalImage);
                
                for (int j = 0; j < m_smVisionInfo.g_arrLead3DDontCareROIs[i].Count; j++)
                {
                    if (m_smVisionInfo.g_arrPolygon_Lead3D[i][j].ref_intFormMode != 2)
                        Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_arrPolygon_Lead3D[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    else
                        Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_arrPolygon_Lead3D[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                    ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);
                }

                objFinalImage.SaveImage(strFolderPath + "DontCareImage" + i.ToString() + "_0.bmp");
            }
            objImage.Dispose();
            objFinalImage.Dispose();
            Polygon.SavePolygon(strFolderPath + "Polygon.xml", m_smVisionInfo.g_arrPolygon_Lead3D);
        }
        private void SaveROISetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
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
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3D ROI", m_smProductionInfo.g_strLotID);
            
        }

        private void ReadAllLeadTemplateDataToGrid_DefaultMethod()
        {
            //This parameter is used to decide the numbering of lead
            //Clockwise top > right > bottom > left
            //AntiClockwise top > left > bottom > right
            int intLeadNo = 1;
            int intIndex = 0;

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
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length - 1; i++)
                {
                    if (!m_smVisionInfo.g_arrLead3D[intIndex].ref_blnSelected)
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
                        //m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                        //                                m_smVisionInfo.g_fCalibPixelX,
                        //                                m_smVisionInfo.g_fCalibPixelY,
                        //                                m_smVisionInfo.g_fCalibOffSetX,
                        //                                m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                        if (i == 0)
                        {
                            m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                                                          m_smVisionInfo.g_fCalibPixelX,
                                                          m_smVisionInfo.g_fCalibPixelY,
                                                          m_smVisionInfo.g_fCalibOffSetX,
                                                          m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                                                          m_smVisionInfo.g_fCalibPixelZ,
                                                          m_smVisionInfo.g_fCalibPixelZ,
                                                          m_smVisionInfo.g_fCalibOffSetZ,
                                                          m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                        }

                        m_smVisionInfo.g_arrLead3D[intIndex].AnalyingLead_DefaultToleranceMethod(intIndex, true, ref intLeadNo);
                        m_smVisionInfo.g_arrLead3D[intIndex].BuildLeadsParameter(intIndex, m_smVisionInfo.g_arrLeadROIs[intIndex][0]);
                        m_smVisionInfo.g_arrLead3D[intIndex].DefineLeadVariance(m_smVisionInfo.g_arrLeadROIs[intIndex][0]);
                        m_smVisionInfo.g_arrLead3D[intIndex].DefineTolerance(true);
                    }

                    if (intIndex == 4)
                        intIndex = 1;
                    else
                        intIndex++;
                }
            }
            else
            {
                int i = 0;
                //for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length - 1; i++)
                {
                    //if (!m_smVisionInfo.g_arrLead3D[intIndex].ref_blnSelected)
                    //{
                    //    if (intIndex == 1)
                    //        intIndex = 4;
                    //    else
                    //        intIndex--;
                    //    continue;
                    //}

                    //Not use prev setting but use default setting
                    if (!m_smVisionInfo.g_blnUsedPreTemplate)
                    {
                        //m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                        //                                m_smVisionInfo.g_fCalibPixelX,
                        //                                m_smVisionInfo.g_fCalibPixelY,
                        //                                m_smVisionInfo.g_fCalibOffSetX,
                        //                                m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                        if (i == 0)
                        {
                            m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                                                          m_smVisionInfo.g_fCalibPixelX,
                                                          m_smVisionInfo.g_fCalibPixelY,
                                                          m_smVisionInfo.g_fCalibOffSetX,
                                                          m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                                                          m_smVisionInfo.g_fCalibPixelZ,
                                                          m_smVisionInfo.g_fCalibPixelZ,
                                                          m_smVisionInfo.g_fCalibOffSetZ,
                                                          m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                        }

                        //m_smVisionInfo.g_arrLead3D[intIndex].AnalyingLead3D();
                        m_smVisionInfo.g_arrLead3D[intIndex].BuildLeadsParameter(intIndex, m_smVisionInfo.g_arrLeadROIs[intIndex][0]);

                        m_smVisionInfo.g_arrLead3D[intIndex].DefineTolerance(true);
                    }

                    if (intIndex == 1)
                        intIndex = 4;
                    else
                        intIndex--;
                }
            }
        }
        private void ReadAllLeadTemplateDataToGrid_DefaultMethod2()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                {
                    continue;
                }

                m_smVisionInfo.g_arrLead3D[i].AnalyingLead_DefaultToleranceMethod(i);
                m_smVisionInfo.g_arrLead3D[i].BuildLeadsParameter(i, m_smVisionInfo.g_arrLeadROIs[i][0]);
                m_smVisionInfo.g_arrLead3D[i].DefineLeadVariance(m_smVisionInfo.g_arrLeadROIs[i][0]);
                m_smVisionInfo.g_arrLead3D[i].DefineTolerance(true);

            }
        }
        private void RotateImage()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                //if (!m_smVisionInfo.g_blnViewRotatedImage)    // Need to re-rotate and re-copy where the ViewRotatedImage is true or not. Bcos Rotated image has been modified for dont care area 
                {
                    if (Math.Abs(m_intCurrentAngle) <= 0)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                        m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

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
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    m_smVisionInfo.g_arrLead3D[i].ClearTemplateBlobsFeatures();

                    if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                        continue;

                    if (i == 0)
                    {
                        m_smVisionInfo.g_arrLead3D[i].ref_blnWhiteOnBlack = true;
                        // Build object using package ROI   //Dun change the thresholdvalue to -4 if build object fail. User will feel weird why the value chnage to auto without their acknowledgement. 
                        if (m_smVisionInfo.g_arrLead3D[i].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[i][0]))
                        {
                            m_smVisionInfo.g_arrLead3D[i].SetBlobsFeaturesToArray_CenterLead3D(m_smVisionInfo.g_arrLeadROIs[i][0]);

                            m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        }
                    }
                    else
                    {
                        m_smVisionInfo.g_arrLead3D[i].ref_blnWhiteOnBlack = false;

                        if (m_smVisionInfo.g_arrLead3D[i].BuildOnlySideLeadObjects_BlackObject(m_smVisionInfo.g_arrLeadROIs[i][0]))
                        {
                            m_smVisionInfo.g_arrLead3D[i].SetBlobsFeaturesToArray_SideLead3D(m_smVisionInfo.g_arrLeadROIs[i][0]);
                            m_smVisionInfo.g_blnViewObjectsBuilded = true;
                        }
                    }
                }
            }
            return true;
        }

       
        private void SetupSteps(bool blnForward)
        {

            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: //Define Search ROI                  
                    {
                        m_smVisionInfo.g_intSelectedImage = 0;
                        AddSearchROI(m_smVisionInfo.g_arrLeadROIs);
                        m_smVisionInfo.g_blnViewPin1TrainROI = false;
                        m_smVisionInfo.g_blnViewROI = true;
                        m_smVisionInfo.g_blnDragROI = true;
                        m_smVisionInfo.g_blnViewRotatedImage = false;
                        lbl_StepNo.BringToFront();
                        lbl_SearchROI.BringToFront();
                        tabCtrl_Lead.SelectedTab = tp_SearchROI;
                        btn_Next.Enabled = true;
                        btn_Previous.Enabled = false;
                    }
                    break;
                case 1: //Define Lead ROI
                    ClearDragHandler();
                    m_smVisionInfo.g_intSelectedImage = 0;

                    if (m_intCurrentPreciseDeg != 0 || m_intCurrentAngle != 0)
                        m_arrLocalBackupRotatedImage[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    else
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    lbl_StepNo.BringToFront();
                    lbl_LeadROI.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_LeadROI;
                    btn_Previous.Enabled = true;
                    break;
                case 2:
                    ClearDragHandler();
                    AddLeadROIToPin1SearchROI(m_smVisionInfo.g_arrLeadROIs);

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
                    lbl_Pin1.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Pin1;
                    btn_Previous.Enabled = true;
                    break;
                case 3: //Define Don't Care Area (maybe not need but keep first)
                    if (!tp_DontCare.Controls.Contains(cbo_SelectROI))
                    {
                        tp_DontCare.Controls.Add(cbo_SelectROI);
                    }
                    SelectROI(m_smVisionInfo.g_intSelectedROI);

                    if (blnForward)
                    {
                    }
                    if(!m_smVisionInfo.g_blnViewRotatedImage)
                    RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                    //// Add Package ROI
                    //AddTrainPackageROI(m_smVisionInfo.g_arrLeadROIs);

                    if (m_intCurrentPreciseDeg != 0 || m_intCurrentAngle != 0)
                        m_arrLocalBackupRotatedImage[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    else
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        
                        m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                        //if (m_smVisionInfo.g_arrLeadROIs[i].Count > 2) // Dont Care ROI start from index 2
                        //{
                        //    for (int j = 2; j < m_smVisionInfo.g_arrLeadROIs[i].Count; j++)
                        //    {
                        //        m_smVisionInfo.g_arrLeadROIs[i][j].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
                        //    }
                        //}
                    }

                    // 2020 06 18 - Make sure Dont Care ROI have parents. Will have "no image ancestor for this ROI" error if no attach to parents.
                    for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                    {
                        if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                        {
                            if (m_smVisionInfo.g_arrLead3DDontCareROIs.Count > i)
                            {
                                for (int j = 0; j < m_smVisionInfo.g_arrLead3DDontCareROIs[i].Count; j++)
                                {
                                    m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
                                }
                            }

                        }
                    }

                    for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Lead3D.Count; u++)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Lead3D[u].Count; i++)
                        {
                            m_smVisionInfo.g_arrPolygon_Lead3D[u][i].ClearPolygon();
                        }
                    }
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;

                    lbl_DontCareArea.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_DontCare;
                    break;
                case 4: //Build Object/Edition
                    if (!gb_Threshold.Controls.Contains(cbo_SelectROI))
                    {
                        gb_Threshold.Controls.Add(cbo_SelectROI);
                    }
                    SelectROI(m_smVisionInfo.g_intSelectedROI);
                    if (!blnForward)
                    {
                        //RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 
                        if (m_intCurrentPreciseDeg != 0 || m_intCurrentAngle != 0)
                            m_arrLocalBackupRotatedImage[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                        else
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    }
                    //if (!m_blnKeepPrevObject)
                    //{
                        for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                        {

                            if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                                continue;

                            // Attach roi on rotated image
                            m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                            //if (m_smVisionInfo.g_arrLeadROIs[i].Count > 2) // Dont Care ROI start from index 2
                            //{
                            //    ROI.ModifyImageGain(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                            //    // Fill dont care area 
                            //    for (int j = 2; j < m_smVisionInfo.g_arrLeadROIs[i].Count; j++)
                            //    {
                            //        m_smVisionInfo.g_arrLeadROIs[i][j].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);

                            //        m_smVisionInfo.g_arrLeadROIs[i][j].FillROI(0);
                            //    }
                            //}
                        }

                    if (m_smVisionInfo.g_blnWantDontCareArea_Lead3D)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrLead3DDontCareROIs.Count; i++)
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrLead3DDontCareROIs[i].Count; j++)
                            {
                                if (m_smVisionInfo.g_arrPolygon_Lead3D[i][j].ref_intFormMode != 2)
                                {
                                    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].AddPoint(new PointF(m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                                        m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                                    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].AddPoint(new PointF((m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROITotalX + m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                                        (m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROITotalY + m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                                    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].AddPolygon((int)(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].ResetPointsUsingOffset(m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                                    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].AddPolygon((int)(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                                }
                                ImageDrawing objImage = new ImageDrawing(true);

                                if (m_smVisionInfo.g_arrPolygon_Lead3D[i][j].ref_intFormMode != 2)
                                    Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_arrPolygon_Lead3D[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                                else
                                    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_arrPolygon_Lead3D[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);

                                ROI objDontCareROI = new ROI();
                                objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight);
                                objDontCareROI.AttachImage(objImage);
                                if (i == 0)
                                    ROI.SubtractROI(m_smVisionInfo.g_arrLeadROIs[i][0], objDontCareROI);
                                else
                                    ROI.LogicOperationAddROI(m_smVisionInfo.g_arrLeadROIs[i][0], objDontCareROI);
                                objDontCareROI.Dispose();
                                objImage.Dispose();
                            }
                        }
                    }

                    if (!BuildObjects())
                        {
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            break;
                        }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    //    {
                    //        m_smVisionInfo.g_arrLead3D[i].ReverseBlobs();
                    //    }
                    //}

                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance)
                        FindCornerPoints();

                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0) // Horizontal
                    {
                        srmLabel52.Visible = false;
                        txt_TipBuildAreaTolerance_Top.Visible = false;

                        srmLabel35.Visible = false;
                        txt_TipBuildAreaTolerance_Bottom.Visible = false;

                        srmLabel31.Visible = true;
                        txt_TipBuildAreaTolerance_Left.Visible = true;

                        srmLabel34.Visible = true;
                        txt_TipBuildAreaTolerance_Right.Visible = true;
                    }
                    else
                    {
                        srmLabel31.Visible = false;
                        txt_TipBuildAreaTolerance_Left.Visible = false;

                        srmLabel34.Visible = false;
                        txt_TipBuildAreaTolerance_Right.Visible = false;

                        srmLabel52.Visible = true;
                        txt_TipBuildAreaTolerance_Top.Visible = true;

                        srmLabel35.Visible = true;
                        txt_TipBuildAreaTolerance_Bottom.Visible = true;

                    }

                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
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

                    //if (!blnForward)
                    //{
                    //    if (IsPitchSettingError())
                    //    {
                    //        m_smVisionInfo.g_intLearnStepNo++;
                    //        m_intDisplayStepNo++;
                    //        break;
                    //    }
                    //}
                    ClearDragHandler();
                    if (blnForward)
                    {
                        string[] strROIName;
                        //m_strPosition = "";
                        if (!m_blnIdentityLeadsDone)
                        {
                            strROIName = new string[m_smVisionInfo.g_arrLeadROIs.Count];

                            //Define ROI name for display in message box
                            for (int y = 0; y < m_smVisionInfo.g_arrLeadROIs.Count; y++)
                            {
                                if (!m_smVisionInfo.g_arrLead3D[y].ref_blnSelected)
                                    continue;

                                switch (y)
                                {
                                    case 0:
                                        strROIName[y] = "Center Leads";
                                        break;
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
                            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                            {
                                if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                                    continue;


                                if (m_smVisionInfo.g_arrLead3D[i].GetSelectedObjectNumber() == 0)
                                {
                                    SRMMessageBox.Show("Minimum 1 lead is required for every ROI", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    m_smVisionInfo.g_intLearnStepNo--;
                                    m_intDisplayStepNo--;
                                    return;
                                }
                            }

                            //2020-08-06 ZJYEOH : Need to get all info just can check Lead Count tally or not
                            BuildLead(true);

                            if (!m_smVisionInfo.g_arrLead3D[0].CheckLeadCountTally())
                            {
                                SRMMessageBox.Show("Total Lead number is not tally with the number of object builded.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                m_smVisionInfo.g_intLearnStepNo--;
                                m_intDisplayStepNo--;
                                SetupSteps(false);
                                return;
                            }

                            // Backup current builded blobsFeature before rearrange
                            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                            {
                                if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                                    continue;

                                m_smVisionInfo.g_arrLead3D[i].BackupBlobsFeatures();
                            }

                            //BuildLead();
                        }
                    }
                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0) // Horizontal
                    {
                        srmLabel63.Visible = false;
                        txt_PkgToBaseTolerance_Top.Visible = false;
                        srmLabel52.Visible = false;
                        txt_TipBuildAreaTolerance_Top.Visible = false;

                        srmLabel62.Visible = false;
                        txt_PkgToBaseTolerance_Bottom.Visible = false;
                        srmLabel35.Visible = false;
                        txt_TipBuildAreaTolerance_Bottom.Visible = false;

                        srmLabel60.Visible = true;
                        txt_PkgToBaseTolerance_Left.Visible = true;
                        srmLabel31.Visible = true;
                        txt_TipBuildAreaTolerance_Left.Visible = true;

                        srmLabel61.Visible = true;
                        txt_PkgToBaseTolerance_Right.Visible = true;
                        srmLabel34.Visible = true;
                        txt_TipBuildAreaTolerance_Right.Visible = true;
                    }
                    else
                    {
                        srmLabel60.Visible = false;
                        txt_PkgToBaseTolerance_Left.Visible = false;
                        srmLabel31.Visible = false;
                        txt_TipBuildAreaTolerance_Left.Visible = false;

                        srmLabel61.Visible = false;
                        txt_PkgToBaseTolerance_Right.Visible = false;
                        srmLabel34.Visible = false;
                        txt_TipBuildAreaTolerance_Right.Visible = false;

                        srmLabel63.Visible = true;
                        txt_PkgToBaseTolerance_Top.Visible = true;
                        srmLabel52.Visible = true;
                        txt_TipBuildAreaTolerance_Top.Visible = true;

                        srmLabel62.Visible = true;
                        txt_PkgToBaseTolerance_Bottom.Visible = true;
                        srmLabel35.Visible = true;
                        txt_TipBuildAreaTolerance_Bottom.Visible = true;

                    }
                    m_smVisionInfo.g_blnViewObjectsBuilded = true;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    lbl_IdentifyLead.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Lead;
                    break;
                case 6:
                    if (blnForward)
                    {
                        tabCtrl_LeadPitch.Controls.Remove(tp_TopROI);
                        tabCtrl_LeadPitch.Controls.Remove(tp_RightROI);
                        tabCtrl_LeadPitch.Controls.Remove(tp_BottomROI);
                        tabCtrl_LeadPitch.Controls.Remove(tp_LeftROI);

                        //for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                        //{
                        //    if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                        //        continue;

                        //    switch (i)
                        //    {
                        //        case 1:
                        //            tabCtrl_LeadPitch.Controls.Add(tp_TopROI);
                        //            break;
                        //        case 2:
                        //            tabCtrl_LeadPitch.Controls.Add(tp_RightROI);
                        //            break;
                        //        case 3:
                        //            tabCtrl_LeadPitch.Controls.Add(tp_BottomROI);
                        //            break;
                        //        case 4:
                        //            tabCtrl_LeadPitch.Controls.Add(tp_LeftROI);
                        //            break;
                        //    }
                        //}
                    }

                    m_strPosition = "";

                    //if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseClosestSizeDefineTolerance)
                    //{
                    //    // Rearrange pitch gap based on latest selected leads
                    //    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    //    {
                    //        m_smVisionInfo.g_arrLead3D[i].BuildLeadPitchLink(false);
                    //    }
                    //}

                    //Load template data
                    //for (int i = 1; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    //{
                    //    if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                    //        continue;

                    //    switch (i)
                    //    {
                    //        case 1:
                    //            m_strPosition = "Top";
                    //            ReadLeadPitchToGrid(1, dgd_TopPGSetting);
                    //            break;
                    //        case 2:
                    //            m_strPosition = "Right";
                    //            ReadLeadPitchToGrid(2, dgd_RightPGSetting);
                    //            break;
                    //        case 3:
                    //            m_strPosition = "Bottom";
                    //            ReadLeadPitchToGrid(3, dgd_BottomPGSetting);
                    //            break;
                    //        case 4:
                    //            m_strPosition = "Left";
                    //            ReadLeadPitchToGrid(4, dgd_LeftPGSetting);
                    //            break;
                    //    }
                    //}

                    AutoDefinePitchGap();

                    if (!chk_Reset.Checked)
                    {
                        m_smVisionInfo.g_arrLead3D[0].LoadPitchGapLinkTemporary();
                    }

                    ReadLeadPitchToGrid(0, dgd_PitchGapSetting);
                    //Load template data
                    m_dgdViewPG = dgd_PitchGapSetting;

                    //for (int i = 1; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    //{
                    //    if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                    //        continue;

                    //    switch (i)
                    //    {
                    //        case 1:
                    //            m_strPosition = "Top";
                    //            m_dgdViewPG = dgd_TopPGSetting;
                    //            break;
                    //        case 2:
                    //            m_strPosition = "Right";
                    //            m_dgdViewPG = dgd_RightPGSetting;
                    //            break;
                    //        case 3:
                    //            m_strPosition = "Bottom";
                    //            m_dgdViewPG = dgd_BottomPGSetting;
                    //            break;
                    //        case 4:
                    //            m_strPosition = "Left";
                    //            m_dgdViewPG = dgd_LeftPGSetting;
                    //            break;
                    //    }
                    //    break;
                    //}

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;

                    lbl_StepNo.BringToFront();
                    lbl_Pitch.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Pitch;
                    btn_Next.Enabled = true;
                    break;
                case 7:
                    lbl_SaveTemplate.BringToFront();
                    tabCtrl_Lead.SelectedTab = tp_Save;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
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

                        if (objDataGrid.Rows[i].Cells[arrColumnIndex[c]].Value.ToString() != "---")
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
                                    if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
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
                    if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
                        intLeadPitchIndex++;
                    else
                    {
                        if (intLeadPitchIndex == j)
                            break;
                    }
                }

                intPitchIndex = m_smVisionInfo.g_arrLead3D[intLeadPitchIndex].GetTotalPitchGap();
                int intTotalLeadNo = m_smVisionInfo.g_arrLead3D[intLeadPosition].GetBlobsFeaturesNumber();
                int intNoID = m_smVisionInfo.g_arrLead3D[intLeadPosition].GetBlobsNoID();
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
                    else if (m_smVisionInfo.g_arrLead3D[intLeadPosition].CheckPitchGapLinkExist(intFormLeadNo, intToLeadNo))
                    {
                        SRMMessageBox.Show("This Pitch/Gap link already exist.");
                    }
                    else if (m_smVisionInfo.g_arrLead3D[intLeadPosition].CheckPitchGapLinkInLeadAlready(intFormLeadNo))
                    {
                        SRMMessageBox.Show("Pitch/Gap already defined in Lead number " + (intFormLeadNo + 1));
                    }
                    else if (!m_smVisionInfo.g_arrLead3D[intLeadPosition].CheckPitchGapLinkAvailable(intFormLeadNo, intToLeadNo))
                    {
                        SRMMessageBox.Show("Pitch/Gap can not be created in between Lead no" + (intFormLeadNo + 1) + " and Lead no" + (intToLeadNo + 1) + ".");
                    }
                    else
                    {
                        m_smVisionInfo.g_arrLead3D[intLeadPosition].SetPitchGap(intPitchIndex, intFormLeadNo, intToLeadNo);
                    }

                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    intCurrentFromLeadNo = intFormLeadNo;
                    intCurrentToLeadNo = intToLeadNo;
                    intLocationX = objSetPitchForm.Location.X;
                    intLocationY = objSetPitchForm.Location.Y;

                    ReadLeadPitchToGrid(intLeadPitchIndex, m_dgdViewPG);
                }
                else
                {
                    ReadLeadPitchToGrid(intLeadPitchIndex, m_dgdViewPG);
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
        private void ReadLeadPitchToGrid(int intLeadPosition, DataGridView dgd_LeadPitch)
        {
            int k = 0;
            try
            {
                if (m_smVisionInfo.g_arrLead3D.Length <= intLeadPosition)
                    return;

                dgd_LeadPitch.Rows.Clear();
                int intTotalLeads = m_smVisionInfo.g_arrLead3D[intLeadPosition].GetBlobsFeaturesNumber();

                for (int t = 0; t < intTotalLeads; t++)
                {
                    int intToLeadNo = m_smVisionInfo.g_arrLead3D[intLeadPosition].GetPitchGapToLeadNo(t);

                    dgd_LeadPitch.Rows.Add();
                    dgd_LeadPitch.Rows[t].HeaderCell.Value = "Lead " + (t + 1);

                    if (intToLeadNo >= 0)
                    {
                        dgd_LeadPitch.Rows[t].Cells[0].Value = t + 1;
                        dgd_LeadPitch.Rows[t].Cells[1].Value = intToLeadNo + 1;
                    }
                    else
                    {
                        dgd_LeadPitch.Rows[t].Cells[0].Value = t + 1;
                        dgd_LeadPitch.Rows[t].Cells[1].Value = "NA";
                    }
                }
                m_intPitchSelectedRowIndex = 0;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("LearnLead3DForm.cs->ReadLeadPitchToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("LearnLead3DForm.cs->ReadLeadPitchToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
            }

        }
        private void ReadLeadPitchToGrid_Old(int intLeadPosition, DataGridView dgd_LeadPitch)
        {
            int k = 0;
            try
            {

                if (m_smVisionInfo.g_arrLead3D.Length <= intLeadPosition)
                    return;

                dgd_LeadPitch.Rows.Clear();
                int intIndex = 0;
                string strPitchData;
                string[] strPitchDataRow;
                int intTotalPitch = m_smVisionInfo.g_arrLead3D[intLeadPosition].GetTotalPitchGap();
                int intNoID = m_smVisionInfo.g_arrLead3D[intLeadPosition].GetBlobsNoID();
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
                    strPitchData = m_smVisionInfo.g_arrLead3D[intLeadPosition].GetPitchGapData(i);
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
                SRMMessageBox.Show("LearnLead3DForm.cs->ReadLeadPitchToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("LearnLead3DForm.cs->ReadLeadPitchToGrid()->Exception: " + ex.ToString() + ". Column Name Error=" + m_strPGRealColName[k]);
            }

        }
        private void LearnLeadForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewLeadInspection = false;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].BackupPreviousTolerance();
            }

            // Make sure all lead are unlocked
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D[i] != null)
                {
                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnLock)
                    {
                        m_smVisionInfo.g_arrLead3D[i].ref_blnLock = false;
                    }
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrPolygon_Lead3D.Count == i)
                {
                    m_smVisionInfo.g_arrPolygon_Lead3D.Add(new List<Polygon>());
                }
            }

            if (m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Lead3D[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
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

            // Start Setup step 1
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.g_intLearnStepNo = 0;
            SetupSteps(true);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
        }

        private void LearnLeadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3D Form Closed", "Exit Learn Lead3D Form", "", "", m_smProductionInfo.g_strLotID);
            
            if (chk_UsePreTemplateImage.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (i < m_arrCurrentImage.Count)
                        m_arrCurrentImage[i].CopyTo(ref m_smVisionInfo.g_arrImages, i);
                }
            }
            m_smVisionInfo.g_arrLead3D[0].ClearFilterBlob();
            // Dispose current image
            for (int i = 0; i < m_arrCurrentImage.Count; i++)
            {
                m_arrCurrentImage[i].Dispose();
            }
            m_smVisionInfo.g_intSelectedDontCareROIIndex = 0;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewPin1ROI = false;
            m_smVisionInfo.g_blnViewPin1TrainROI = false;
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
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    m_smVisionInfo.g_arrImages[i].CopyTo(ref m_arrCurrentImage, i);

                    string strFilePath;
                    if (i == 0)
                    {
                        strFilePath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Template\\OriTemplate.bmp";

                    }
                    else
                    {
                        strFilePath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\Template\\OriTemplate_Image" + i.ToString() + ".bmp";
                    }

                    if (File.Exists(strFilePath))
                        m_smVisionInfo.g_arrImages[i].LoadImage_CopyToTempFolderFirst(strFilePath);
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

            LoadROISetting(strFolderPath + "Lead3D\\ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead3D.Length);
            LoadROISetting(strFolderPath + "Lead3D\\DontCareROI.xml", m_smVisionInfo.g_arrLead3DDontCareROIs, m_smVisionInfo.g_arrLead3D.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrLeadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrLead3DDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrLead3DDontCareROIs[i].Count; j++)
                        {
                            m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
                        }
                    }

                }
            }
            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (m_smVisionInfo.g_arrPin1 != null)
                {
                    m_smVisionInfo.g_arrPin1[0].LoadTemplate(strFolderPath + "Lead3D\\Template\\");
                }
            }

            LoadLead3DSetting(strFolderPath + "Lead3D\\");
            Polygon.LoadPolygon(strFolderPath + "Lead3D\\Template\\Polygon.xml", m_smVisionInfo.g_arrPolygon_Lead3D);
            m_smVisionInfo.AT_PR_AttachImagetoROI = true;
            this.Close();
            this.Dispose();
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo == 4)
                m_blnKeepPrevObject = true;

            if (m_smVisionInfo.g_intLearnStepNo == 5)
                m_blnIdentityLeadsDone = true;

            if (m_smVisionInfo.g_intLearnStepNo > 0)
                m_smVisionInfo.g_intLearnStepNo--;

            m_intDisplayStepNo--;

            if (!m_smVisionInfo.g_blnWantDontCareArea_Lead3D && m_smVisionInfo.g_intLearnStepNo == 3)
            {
                m_smVisionInfo.g_intLearnStepNo--;
                m_intDisplayStepNo--;
            }

            if (m_smVisionInfo.g_intLearnStepNo == 2 && !m_smVisionInfo.g_blnWantPin1)
                m_smVisionInfo.g_intLearnStepNo--;

            SetupSteps(false);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo < 99)
                m_smVisionInfo.g_intLearnStepNo++;

            m_intDisplayStepNo++;

            if (!m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_intLearnStepNo == 2)
            {
                m_smVisionInfo.g_intLearnStepNo++;
                m_intDisplayStepNo++;
            }

            if (!m_smVisionInfo.g_blnWantDontCareArea_Lead3D && m_smVisionInfo.g_intLearnStepNo == 3)
            {
                m_smVisionInfo.g_intLearnStepNo++;
                m_intDisplayStepNo++;
            }

            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrLead3D[0].DefineTolerance(false);

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                        m_smVisionInfo.g_strVisionFolderName + "\\";

            //m_smVisionInfo.g_intSelectedImage = 0;
            SaveLeadSetting(strPath + "Lead3D\\");

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadROISetting(strFolderPath + "Lead3D\\ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead3D.Length);
            LoadROISetting(strFolderPath + "Lead3D\\DontCareROI.xml", m_smVisionInfo.g_arrLead3DDontCareROIs, m_smVisionInfo.g_arrLead3D.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrLeadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrLead3DDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrLead3DDontCareROIs[i].Count; j++)
                        {
                            m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
                        }
                    }

                }
            }
            LoadLead3DSetting(strFolderPath + "Lead3D\\");

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

            //if (intNumberOfLead_Top == 0 && intNumberOfLead_Bottom == 0 && intNumberOfLead_Left == 0 && intNumberOfLead_Right == 0)
            //{
            //    SRMMessageBox.Show("At least 1 Lead must be selected.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    txt_LeadNumber_Top.Text = "1";
            //    intNumberOfLead_Top = Convert.ToInt32(txt_LeadNumber_Top.Text);
            //}

            if ((intNumberOfLead_Top == 0 && intNumberOfLead_Bottom == 0 && radioBtn_Vertical.Checked) || (intNumberOfLead_Left == 0 && intNumberOfLead_Right == 0 && radioBtn_Horizontal.Checked))
            {
                SRMMessageBox.Show("At least 1 Lead must be selected.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                if (radioBtn_Vertical.Checked)
                {
                    txt_LeadNumber_Top.Text = "1";
                    intNumberOfLead_Top = Convert.ToInt32(txt_LeadNumber_Top.Text);
                }
                else
                {
                    txt_LeadNumber_Left.Text = "1";
                    intNumberOfLead_Left = Convert.ToInt32(txt_LeadNumber_Left.Text);
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                        //// For center ROI, intNumberOfLead will keep total lead of 4 side ROI.
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead = intNumberOfLead_Top + intNumberOfLead_Bottom + intNumberOfLead_Left + intNumberOfLead_Right;
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = intNumberOfLead_Top;
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = intNumberOfLead_Bottom;
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = intNumberOfLead_Left;
                        //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = intNumberOfLead_Right;

                        if (radioBtn_Vertical.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead = intNumberOfLead_Top + intNumberOfLead_Bottom;
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = intNumberOfLead_Top;
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = intNumberOfLead_Bottom;
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = 0;
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = 0;
                        }
                        else
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead = intNumberOfLead_Left + intNumberOfLead_Right;
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = 0;
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = 0;
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = intNumberOfLead_Left;
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = intNumberOfLead_Right;
                        }
                        break;
                    case 1:
                        m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                        if (radioBtn_Vertical.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = intNumberOfLead_Top;
                        }
                        else
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = 0;
                        }
                        break;
                    case 2:
                        m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                        if (radioBtn_Vertical.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = 0;
                        }
                        else
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = intNumberOfLead_Right;
                        }
                        break;
                    case 3:
                        m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                        if (radioBtn_Vertical.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = intNumberOfLead_Bottom;
                        }
                        else
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = 0;
                        }
                        break;
                    case 4:
                        m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                        if (radioBtn_Vertical.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = 0;
                        }
                        else
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = intNumberOfLead_Left;
                        }
                        break;
                }
                m_smVisionInfo.g_arrLead3D[i].LoadArrayPointGauge_Center(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\CenterPointGauge.xml", "Lead3D" + i.ToString());
                m_smVisionInfo.g_arrLead3D[i].LoadArrayPointGauge_Side(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\SidePointGauge.xml", "Lead3D" + i.ToString());
                m_smVisionInfo.g_arrLead3D[i].LoadArrayPointGauge_Corner(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\CornerPointGauge.xml", "Lead3D" + i.ToString());
            }

            UpdateReferenceCornerGUI();
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intMinArea = Convert.ToInt32(txt_MinArea.Text);

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_intFilterMinArea = intMinArea;
            }

            BuildObjects();

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance)
                FindCornerPoints();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BaseOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_intBaseOffset = Convert.ToInt32(txt_BaseOffset.Text);
            }

            if (BuildObjects())
            {
                BuildLead(false);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_arrLead3D[0].ref_intTipOffset = Convert.ToInt32(txt_TipOffset.Text);

            if (BuildObjects())
            {
                BuildLead(false);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_RotateUnit_Old_Click(object sender, EventArgs e)
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


            ROI.RotateROI(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                ROI.RotateROI(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, i);
            }

            // After rotating the image, attach the rotated image into ROI again
            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Backup rotated image into local backup
            for (int i = m_arrLocalBackupRotatedImage.Count; i < 1; i++)
            {
                m_arrLocalBackupRotatedImage.Add(new ImageDrawing(true));
            }
            ImageDrawing objLocalBackupRotatedImage = m_arrLocalBackupRotatedImage[0];
            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref objLocalBackupRotatedImage);

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
            ROI.Rotate0Degree(objROI, m_intCurrentAngle - float.Parse(txt_RotateAngle.Text), 8, ref m_smVisionInfo.g_arrRotatedImages, 0);

            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

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
            int intSelectedROI = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                    intSelectedROI++;
                else
                    break;
            }

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                        m_smVisionInfo.g_arrLeadROIs[j][0].ClearDragHandle();
                }

                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                {
                    intSelectedROI = j;
                }
            }

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intThresholdValue;

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[0].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[1].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[2].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[3].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[4].ref_intThresholdValue);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, true, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intSelectedROI != 0 && i == 0)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;

                        //m_smVisionInfo.g_arrLead3D[i].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[i][0]);

                        ////Clear Temp Blobs Features
                        //m_smVisionInfo.g_arrLead3D[i].ClearTempBlobsFeatures();

                        ////Set blobs features data into Temp Blobs Features
                        //m_smVisionInfo.g_arrLead3D[i].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[i][0]);

                        ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                        //m_smVisionInfo.g_arrLead3D[i].CompareSelectedBlobs();
                    }

                    BuildObjects();
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;

                    BuildObjects();

                    //m_smVisionInfo.g_arrLead3D[intSelectedROI].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                    ////Clear Temp Blobs Features
                    //m_smVisionInfo.g_arrLead3D[intSelectedROI].ClearTempBlobsFeatures();

                    ////Set blobs features data into Temp Blobs Features
                    //m_smVisionInfo.g_arrLead3D[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                    ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                    //m_smVisionInfo.g_arrLead3D[intSelectedROI].CompareSelectedBlobs();
                }
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intThresholdValue;

                BuildObjects();
                //m_smVisionInfo.g_arrLead3D[intSelectedROI].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                ////Clear Temp Blobs Features
                //m_smVisionInfo.g_arrLead3D[intSelectedROI].ClearTempBlobsFeatures();

                ////Set blobs features data into Temp Blobs Features
                //m_smVisionInfo.g_arrLead3D[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                //m_smVisionInfo.g_arrLead3D[intSelectedROI].CompareSelectedBlobs();
            }

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance)
                FindCornerPoints();

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_Vertical_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtn_Horizontal.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    m_smVisionInfo.g_arrLead3D[i].ref_intLeadDirection = 0;
                }

                pic_LeadCount.BackgroundImage = pic_DirectionHorizontal.BackgroundImage;
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    m_smVisionInfo.g_arrLead3D[i].ref_intLeadDirection = 1;
                }

                pic_LeadCount.BackgroundImage = pic_DirectionVertical.BackgroundImage;
            }
        }

        private void btn_UndoObjects_Click(object sender, EventArgs e)
        {
            int intSelectedROI = m_smVisionInfo.g_intSelectedROI;//0
            //for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            //{
            //    if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
            //        continue;

            //    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
            //    {
            //        intSelectedROI = j;
            //        break;
            //    }
            //}

            m_smVisionInfo.g_arrLead3D[intSelectedROI].UndoSelectedObject();

            m_smVisionInfo.g_blnViewObjectsBuilded = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BuildObjects_Click(object sender, EventArgs e)
        {
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
                switch(m_strPosition)
                {
                    case "Top Left":
                        m_smVisionInfo.g_arrLead3D[j].ref_intFirstLead = 1;
                        break;
                    case "Top Right":
                        m_smVisionInfo.g_arrLead3D[j].ref_intFirstLead = 2;
                        break;
                    case "Bottom Left":
                        m_smVisionInfo.g_arrLead3D[j].ref_intFirstLead = 3;
                        break;
                    case "Bottom Right":
                        m_smVisionInfo.g_arrLead3D[j].ref_intFirstLead = 4;
                        break;
                }
            }

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                //ReadAllLeadTemplateDataToGrid_DefaultMethod2();
                BuildLead(false);
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
                if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
                    intLeadPitchIndex++;
                else
                {
                    if (intLeadPitchIndex == j)
                        break;
                }
            }

            int intTotalPitch = m_smVisionInfo.g_arrLead3D[intLeadPitchIndex].GetTotalPitchGap();
            SetPitch(intTotalPitch, 0, 0, intLeadPitchIndex);
            ReadLeadPitchToGrid(intLeadPitchIndex, m_dgdViewPG);
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
                        m_smVisionInfo.g_arrLead3D[j].ref_blnClockWise = true;
                        break;
                    case 1:
                        m_smVisionInfo.g_arrLead3D[j].ref_blnClockWise = false;
                        break;
                }
            }

            if (m_smVisionInfo.g_intLearnStepNo == 5)
            {
                //ReadAllLeadTemplateDataToGrid_DefaultMethod2();
                BuildLead(false);
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
                        if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
                            intLeadPitchIndex++;
                        else
                        {
                            if (intLeadPitchIndex == j)
                                break;
                        }
                    }

                    m_smVisionInfo.g_arrLead3D[intLeadPitchIndex].DeletePitchGap(m_intPitchSelectedRowIndex);
                    ReadLeadPitchToGrid(intLeadPitchIndex, m_dgdViewPG);
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
            //Define lead index
            int intLeadPitchIndex = tabCtrl_LeadPitch.SelectedIndex;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
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
                        m_smVisionInfo.g_arrLeadROIs[intIndex][0].ref_ROITotalX,
                        m_smVisionInfo.g_arrLeadROIs[intIndex][0].ref_ROITotalY);
                }
                else
                {
                    m_smVisionInfo.g_arrLeadROIs[i][0].VerifyROIArea(
                        0,0);
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

            //if (e.ColumnIndex < 2)
            //    return;

            if (e.ColumnIndex == 0 || e.ColumnIndex == 1) // Edit PitchGap "To Pad" no
            {
                //int intTotalLeadNo = m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].GetBlobsFeaturesNumber();
                int intFormLeadNo = Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value);
                string strToLeadNo = ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value.ToString();
                List<int> intTotalLeadNo = new List<int>();
                //switch (m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].GetLeadDirection(intFormLeadNo))
                //{
                //    case 4:
                //        intTotalLeadNo = m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].ref_intNumberOfLead_Top;
                //        break;
                //    case 2:
                //        intTotalLeadNo = m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].ref_intNumberOfLead_Right;
                //        break;
                //    case 8:
                //        intTotalLeadNo = m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].ref_intNumberOfLead_Bottom;
                //        break;
                //    case 1:
                //        intTotalLeadNo = m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].ref_intNumberOfLead_Left;
                //        break;
                //    case 0:
                //        return;
                //}
                intTotalLeadNo = m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].GetLeadDirection(intFormLeadNo);

                SetLead3DPitchGapForm objSetPitchGapForm = new SetLead3DPitchGapForm(intTotalLeadNo, intFormLeadNo, strToLeadNo);
                if (objSetPitchGapForm.ShowDialog() == DialogResult.OK)
                {
                    bool blnAllowChange = true;
                    int intToPadNo = 0;
                    if (objSetPitchGapForm.ref_strToLeadNo != "NA")
                    {
                        intToPadNo = Convert.ToInt32(objSetPitchGapForm.ref_strToLeadNo);

                        if (intFormLeadNo == intToPadNo)
                        {
                            SRMMessageBox.Show("Cannot select same lead number!");
                            blnAllowChange = false;
                        }
                        else if (m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].CheckPitchGapLinkExist(intFormLeadNo - 1, intToPadNo - 1))
                        {
                            SRMMessageBox.Show("This Pitch/Gap link already exist.");
                            blnAllowChange = false;
                        }
                        else if (m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].CheckPitchGapLinkInLeadAlready(intFormLeadNo - 1))
                        {
                            //SRMMessageBox.Show("Pitch/Gap already defined in pad number " + intFormPadNo);
                            //blnAllowChange = false;
                        }
                        else if (!m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].CheckPitchGapLinkAvailable(intFormLeadNo - 1, intToPadNo - 1))
                        {
                            SRMMessageBox.Show("Pitch/Gap can not be created in between lead no." + intFormLeadNo + " and lead no." + intToPadNo + ".");
                            blnAllowChange = false;
                        }
                    }
                    if (blnAllowChange)
                    {
                        m_smVisionInfo.g_arrLead3D[tabCtrl_LeadPitch.SelectedIndex].SetPitchGap(intFormLeadNo - 1, intToPadNo - 1);
                        ((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value = objSetPitchGapForm.ref_strToLeadNo;
                        ReadLeadPitchToGrid(tabCtrl_LeadPitch.SelectedIndex, m_dgdViewPG);
                    }
                }
            }

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
            //    if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
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
            //        //m_smVisionInfo.g_arrLead3D[intLeadPosition].SetPitchGapData(i,
            //        //    (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[2]].Value) * m_smVisionInfo.g_fCalibPixelX),
            //        //    (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[3]].Value) * m_smVisionInfo.g_fCalibPixelX),
            //        //    (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[4]].Value) * m_smVisionInfo.g_fCalibPixelX),
            //        //    (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[5]].Value) * m_smVisionInfo.g_fCalibPixelX));
            //        //m_smVisionInfo.g_arrLead3D[intLeadPosition].SetPitchGapData(i,
            //        //    (Convert.ToInt32(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[0]].Value)) - 1,   // add "-1" bcos Lead index start from 0
            //        //    (Convert.ToInt32(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[1]].Value)) - 1,   // add "-1" bcos Lead index start from 0
            //        //        (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[2]].Value)),
            //        //        (Convert.ToSingle(((DataGridView)sender).Rows[i].Cells[m_strPGRealColName[3]].Value)));

            //        if (!objSetValueForm.ref_blnSetAllRows)
            //            break;
            //    }

            //    // Update grid value from database
            //    //ReadLeadPitchToGrid(tabCtrl_LeadPG.SelectedIndex, ((DataGridView)sender));

            //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //}

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

                if (m_smVisionInfo.g_intLearnStepNo > 2)
                {
                    if (tabCtrl_LeadPitch.SelectedIndex >= 0)
                    {
                        int intIndex = 0;
                        for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                        {
                            if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
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

            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if ((m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                {
                    cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Lead3D[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
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

            if (m_objLead3DLineProfileForm != null)
            {
                if (!m_objLead3DLineProfileForm.ref_blnShow)
                {
                    m_smVisionInfo.g_strSelectedPage = "Lead3D";
                    m_objLead3DLineProfileForm.Close();
                    m_objLead3DLineProfileForm.Dispose();
                    m_objLead3DLineProfileForm = null;
                    this.Show();

                    m_smVisionInfo.g_blnDragROI = m_blnDragROIPrev;
                }
                else
                {
                    if (m_objLead3DLineProfileForm.ref_blnBuildLead)
                    {
                        m_objLead3DLineProfileForm.ref_blnBuildLead = false;
                        if (BuildObjects())
                        {
                            BuildLead(false);

                            m_objLead3DLineProfileForm.ref_blnUpdateDrawingGaugeAfterBuildLeadDone = true;
                        }
                        //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
            }
        }

        private void btn_ROISaveClose_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                         m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\";

            SaveROISetting(strFolderPath);

            if (m_smVisionInfo.g_blnWantDontCareArea_Lead3D)
            {
                SaveDontCareROISetting(strFolderPath);
                SaveDontCareSetting(strFolderPath + "Template\\");
            }

            // Reload all information (User may go to last page to change setting)
            LoadROISetting(strFolderPath + "ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead3D.Length);

            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void txt_BaseLineTrimFromEdge_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_intBaseLineTrimFromEdge = Convert.ToInt32(txt_BaseLineTrimFromEdge.Text);
            }

            if (BuildObjects())
            {
                BuildLead(false);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void txt_BaseEdgeStepFromBase_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_intBaseLineSteps = Convert.ToInt32(txt_BaseLineSteps.Text);
            }

            if (BuildObjects())
            {
                BuildLead(false);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void RotateImagesTo0Degree_UsingCenterROI(int intLeadIndex, int intImageIndex)
        {
            int intSearchROICenterX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterX;
            int intSearchROICenterY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterY;
            int intSearchROIStartX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX;
            int intSearchROIStartY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY;
            int intSearchROIEndX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIWidth;
            int intSearchROIEndY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIHeight;
            float fUnitCenterPointX = 0;
            float fUnitCenterPointY = 0;
            float fUnitAngle = 0;
            float fRotateROIHalfWidth;
            float fRotateROIHalfHeight;
            
            fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X;
            fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y;
            fUnitAngle = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle;

            float fAngleResult = -m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle;
            float CenterX = 0;
            float CenterY = 0;
            float fXAfterRotated = 0;
            float fYAfterRotated = 0;

            CenterX = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX + (float)(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth) / 2;
            CenterY = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY + (float)(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight) / 2;
            fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - CenterX) * Math.Cos(fAngleResult * Math.PI / 180)) -
                               ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - CenterY) * Math.Sin(fAngleResult * Math.PI / 180)));
            fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - CenterX) * Math.Sin(fAngleResult * Math.PI / 180)) +
                                ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - CenterY) * Math.Cos(fAngleResult * Math.PI / 180)));

            m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center = new PointF(fXAfterRotated, fYAfterRotated);

            ROI objRotateROI = new ROI();
            // Get rotate roi which center ROI point same as result position unit ROI
            objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
            objRotateROI.LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX,
                                           m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY,
                                           m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth,
                                           m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);

            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, 0);
            
            objRotateROI.Dispose();

            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

        }
        private void RotateImagesTo0Degree_PatternMatch(int intLeadIndex, int intImageIndex)
        {

            int intSearchROICenterX = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalCenterX;
            int intSearchROICenterY = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalCenterY;
            int intSearchROIStartX = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX;
            int intSearchROIStartY = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY;
            int intSearchROIEndX = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth;
            int intSearchROIEndY = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight;
            float fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X;
            float fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y;
            float fUnitAngle = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle;

            float fRotateROIHalfWidth, fRotateROIHalfHeight;
            if (fUnitCenterPointX <= intSearchROICenterX)
            {
                fRotateROIHalfWidth = fUnitCenterPointX - intSearchROIStartX;
            }
            else
            {
                fRotateROIHalfWidth = intSearchROIEndX - fUnitCenterPointX;
            }

            if (fUnitCenterPointY <= intSearchROICenterY)
            {
                fRotateROIHalfHeight = fUnitCenterPointY - intSearchROIStartY;
            }
            else
            {
                fRotateROIHalfHeight = intSearchROIEndY - fUnitCenterPointY;
            }

            // Get rotate roi which center ROI point same as result position unit ROI
            ROI objRotateROI = new ROI();
            objRotateROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            objRotateROI.LoadROISetting((int)Math.Round(fUnitCenterPointX - fRotateROIHalfWidth, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fUnitCenterPointY - fRotateROIHalfHeight, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fRotateROIHalfWidth * 2, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fRotateROIHalfHeight * 2, 0, MidpointRounding.AwayFromZero));

            //2020-08-19 ZJYEOH : below function copy g_arrImages to g_arrRotatedImages before rotate
            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, 0);

            objRotateROI.Dispose();

            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            
        }
        private void FindCornerPoints()
        {
            m_smVisionInfo.g_arrLead3D[1].FindUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[1][0], true);
            m_smVisionInfo.g_arrLead3D[2].FindUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[2][0], true);
            m_smVisionInfo.g_arrLead3D[3].FindUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[3][0], true);
            m_smVisionInfo.g_arrLead3D[4].FindUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[4][0], true);

            m_smVisionInfo.g_arrLead3D[0].DefineCenterUnitEdge(m_smVisionInfo.g_arrLead3D[1].ref_pCornerPoint_Left, m_smVisionInfo.g_arrLead3D[1].ref_pCornerPoint_Right,
                                                            m_smVisionInfo.g_arrLead3D[2].ref_pCornerPoint_Top, m_smVisionInfo.g_arrLead3D[2].ref_pCornerPoint_Bottom,
                                                            m_smVisionInfo.g_arrLead3D[3].ref_pCornerPoint_Left, m_smVisionInfo.g_arrLead3D[3].ref_pCornerPoint_Right,
                                                            m_smVisionInfo.g_arrLead3D[4].ref_pCornerPoint_Top, m_smVisionInfo.g_arrLead3D[4].ref_pCornerPoint_Bottom);

            if (m_smVisionInfo.g_arrLead3D.Length > 0 && m_smVisionInfo.g_arrLead3D[0].ref_blnSelected)
            {
                m_smVisionInfo.g_arrLead3D[0].ClearTemplateBlobsFeatures();

                m_smVisionInfo.g_arrLead3D[0].ref_blnWhiteOnBlack = true;

                ROI objROI = new ROI();
                ROI objROI2 = new ROI();
                ImageDrawing objImage = new ImageDrawing();
                m_smVisionInfo.g_arrImages[0].CopyTo(ref objImage);

                objROI.AttachImage(objImage);
                objROI2.AttachImage(m_smVisionInfo.g_arrImages[0]);
                if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0) // Horizontal
                {
                    objROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2 - m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left, 0, MidpointRounding.AwayFromZero),
                                          m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY,
                                          (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth, 0, MidpointRounding.AwayFromZero) + m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left + m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Right,
                                          m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);

                    objROI2.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2 - m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left, 0, MidpointRounding.AwayFromZero),
                                          m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY,
                                          (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth, 0, MidpointRounding.AwayFromZero) + m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left + m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Right,
                                          m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);
                }
                else
                {
                    objROI.LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX,
                                          (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2 - m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top, 0, MidpointRounding.AwayFromZero),
                                          m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth,
                                          (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight, 0, MidpointRounding.AwayFromZero) + m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top + m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Bottom);

                    objROI2.LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX,
                                        (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2 - m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top, 0, MidpointRounding.AwayFromZero),
                                        m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth,
                                        (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight, 0, MidpointRounding.AwayFromZero) + m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top + m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Bottom);
                }

                ROI.SubtractROI(objROI, objROI2);

                m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(objImage);
                if (m_smVisionInfo.g_arrLead3D[0].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[0][0]))
                {
                    m_smVisionInfo.g_arrLead3D[0].SetBlobsFeaturesToArray_CenterLead3D(m_smVisionInfo.g_arrLeadROIs[0][0]);

                    m_smVisionInfo.g_blnViewObjectsBuilded = true;
                }
                objROI.Dispose();
                objROI2.Dispose();
                objImage.Dispose();
                m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }
        }
        private void BuildLead(bool blnStoreIgnoredBlob)
        {
            /*
             * 1. Search 4 sides ROI unit base line and corner points.
             * 2. Define center unit edge according to 4 side Corner Points
             * 3.  
             * 
             */

            if (blnStoreIgnoredBlob)
            {
                m_smVisionInfo.g_arrLead3D[0].AssignFilterBlob();
            }

            m_smVisionInfo.g_arrLead3D[1].FindUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[1][0], true);
            m_smVisionInfo.g_arrLead3D[2].FindUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[2][0], true);
            m_smVisionInfo.g_arrLead3D[3].FindUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[3][0], true);
            m_smVisionInfo.g_arrLead3D[4].FindUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[4][0], true);

            m_smVisionInfo.g_arrLead3D[0].DefineCenterUnitEdge(m_smVisionInfo.g_arrLead3D[1].ref_pCornerPoint_Left, m_smVisionInfo.g_arrLead3D[1].ref_pCornerPoint_Right,
                                                            m_smVisionInfo.g_arrLead3D[2].ref_pCornerPoint_Top, m_smVisionInfo.g_arrLead3D[2].ref_pCornerPoint_Bottom,
                                                            m_smVisionInfo.g_arrLead3D[3].ref_pCornerPoint_Left, m_smVisionInfo.g_arrLead3D[3].ref_pCornerPoint_Right,
                                                            m_smVisionInfo.g_arrLead3D[4].ref_pCornerPoint_Top, m_smVisionInfo.g_arrLead3D[4].ref_pCornerPoint_Bottom);
            m_smVisionInfo.g_arrLead3D[0].ref_fTemplateCornerPoint_CenterX = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X;
            m_smVisionInfo.g_arrLead3D[0].ref_fTemplateCornerPoint_CenterY = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y;
            float fAngle = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle;

            // Hide this for future use
            // Rotate image according to center unit edge angle
            //m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            //ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            //m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);
            //ROI.Rotate0Degree(m_smVisionInfo.g_arrLeadROIs[0][0], m_fPreRotatedDegree + m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle, 8, ref m_smVisionInfo.g_arrRotatedImages, 0);

            RotateImagesTo0Degree_UsingCenterROI(0, 0);
            //RotateImagesTo0Degree_PatternMatch(0, 0);

            //2020-08-19 ZJYEOH : Need to subtract dont care if want dont care
            if (m_smVisionInfo.g_blnWantDontCareArea_Lead3D)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead3DDontCareROIs.Count; i++)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrLead3DDontCareROIs[i].Count; j++)
                    {
                        //if (m_smVisionInfo.g_arrPolygon_Lead3D[i][j].ref_intFormMode != 2)
                        //{
                        //    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].AddPoint(new PointF(m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                        //        m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                        //    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].AddPoint(new PointF((m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROITotalX + m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                        //        (m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROITotalY + m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                        //    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].AddPolygon((int)(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                        //}
                        //else
                        //{
                        //    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].ResetPointsUsingOffset(m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrLead3DDontCareROIs[i][j].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                        //    m_smVisionInfo.g_arrPolygon_Lead3D[i][j].AddPolygon((int)(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                        //}
                        ImageDrawing objImage = new ImageDrawing(true);

                        if (m_smVisionInfo.g_arrPolygon_Lead3D[i][j].ref_intFormMode != 2)
                            Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_arrPolygon_Lead3D[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        else
                            Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_arrPolygon_Lead3D[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);

                        ROI objDontCareROI = new ROI();
                        objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight);
                        objDontCareROI.AttachImage(objImage);
                        if (i == 0)
                            ROI.SubtractROI(m_smVisionInfo.g_arrLeadROIs[i][0], objDontCareROI);
                        else
                            ROI.LogicOperationAddROI(m_smVisionInfo.g_arrLeadROIs[i][0], objDontCareROI);
                        objDontCareROI.Dispose();
                        objImage.Dispose();
                    }
                }
            }

            m_smVisionInfo.g_blnViewRotatedImage = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_smVisionInfo.g_arrLead3D[0].FindLeads(m_smVisionInfo.g_arrLeadROIs[0][0]);

            m_smVisionInfo.g_arrLead3D[0].SetCalibrationData(
                                            m_smVisionInfo.g_fCalibPixelX,
                                            m_smVisionInfo.g_fCalibPixelY,
                                            m_smVisionInfo.g_fCalibOffSetX,
                                            m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);

            //m_smVisionInfo.g_arrLead3D[0].SortObjectNumber_Lead3D();
            ReadAllLeadTemplateDataToGrid_DefaultMethod2();

            m_smVisionInfo.g_arrLead3D[0].BuildLeadsParameter_CenterLead3D(m_smVisionInfo.g_arrLeadROIs[0][0]);

            if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0)    // Horizontal
            {
                List<PointF> arrLeadCenterPoints = new List<PointF>();
                List<SizeF> arrLeadSizeF = new List<SizeF>();
                List<int> arrLeadID = new List<int>();
                m_smVisionInfo.g_arrLead3D[0].GetTemplateLeadCenterPointsAndSize(1, ref arrLeadID, ref arrLeadCenterPoints, ref arrLeadSizeF); // 1 = Border Left 

                for (int i = 0; i < arrLeadCenterPoints.Count; i++)
                {
                    arrLeadCenterPoints[i] = new PointF(arrLeadCenterPoints[i].X, arrLeadCenterPoints[i].Y + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY);
                }

                m_smVisionInfo.g_arrLead3D[4].FindSideLeads(m_smVisionInfo.g_arrLeadROIs[4][0], arrLeadID, arrLeadCenterPoints, arrLeadSizeF);  // Find Lead in Left side ROI

                m_smVisionInfo.g_arrLead3D[0].GetTemplateLeadCenterPointsAndSize(2, ref arrLeadID, ref arrLeadCenterPoints, ref arrLeadSizeF); // 2 = Border Right

                for (int i = 0; i < arrLeadCenterPoints.Count; i++)
                {
                    arrLeadCenterPoints[i] = new PointF(arrLeadCenterPoints[i].X, arrLeadCenterPoints[i].Y + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY);
                }

                m_smVisionInfo.g_arrLead3D[2].FindSideLeads(m_smVisionInfo.g_arrLeadROIs[2][0], arrLeadID, arrLeadCenterPoints, arrLeadSizeF);  // Find Lead in right side ROI
            }
            else
            {
                List<PointF> arrLeadCenterPoints = new List<PointF>();
                List<SizeF> arrLeadSizeF = new List<SizeF>();
                List<int> arrLeadID = new List<int>();
                m_smVisionInfo.g_arrLead3D[0].GetTemplateLeadCenterPointsAndSize(4, ref arrLeadID, ref arrLeadCenterPoints, ref arrLeadSizeF); // 1 = Border Top

                for (int i = 0; i < arrLeadCenterPoints.Count; i++)
                {
                    arrLeadCenterPoints[i] = new PointF(arrLeadCenterPoints[i].X + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX, arrLeadCenterPoints[i].Y);
                }

                m_smVisionInfo.g_arrLead3D[1].FindSideLeads(m_smVisionInfo.g_arrLeadROIs[1][0], arrLeadID, arrLeadCenterPoints, arrLeadSizeF);  // Find Lead in top side ROI

                m_smVisionInfo.g_arrLead3D[0].GetTemplateLeadCenterPointsAndSize(8, ref arrLeadID, ref arrLeadCenterPoints, ref arrLeadSizeF); // 2 = Border Bottom

                for (int i = 0; i < arrLeadCenterPoints.Count; i++)
                {
                    arrLeadCenterPoints[i] = new PointF(arrLeadCenterPoints[i].X + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX, arrLeadCenterPoints[i].Y);
                }

                m_smVisionInfo.g_arrLead3D[3].FindSideLeads(m_smVisionInfo.g_arrLeadROIs[3][0], arrLeadID, arrLeadCenterPoints, arrLeadSizeF);  // Find Lead in bottom side ROI
            }
        }

        private void txt_TipOffsetSide_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // Scan side ROI only
            for (int i = 1; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_intTipOffset = Convert.ToInt32(txt_TipOffsetSide.Text);
            }

            //if (BuildObjects())
            {
                BuildLead(false);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CornerTolerance_Top_TextChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;

            //for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            //{
            //    m_smVisionInfo.g_arrLead3D[i].ref_intCornerSearchingTolerance_Top = Convert.ToInt32(txt_CornerTolerance_Top.Text);
            //}

            //if (BuildObjects())
            //{
            //    BuildLead(false);
            //}

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CornerTolerance_Right_TextChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;

            //for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            //{
            //    m_smVisionInfo.g_arrLead3D[i].ref_intCornerSearchingTolerance_Right = Convert.ToInt32(txt_CornerTolerance_Right.Text);
            //}

            //if (BuildObjects())
            //{
            //    BuildLead(false);
            //}

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CornerTolerance_Bottom_TextChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;

            //for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            //{
            //    m_smVisionInfo.g_arrLead3D[i].ref_intCornerSearchingTolerance_Bottom = Convert.ToInt32(txt_CornerTolerance_Bottom.Text);
            //}

            //if (BuildObjects())
            //{
            //    BuildLead(false);
            //}

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CornerTolerance_Left_TextChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;

            //for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            //{
            //    m_smVisionInfo.g_arrLead3D[i].ref_intCornerSearchingTolerance_Left = Convert.ToInt32(txt_CornerTolerance_Left.Text);
            //}

            //if (BuildObjects())
            //{
            //    BuildLead(false);
            //}

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CornerLength_TextChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;

            //for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            //{
            //    m_smVisionInfo.g_arrLead3D[i].ref_intCornerSearchingLength = Convert.ToInt32(txt_CornerLength.Text);
            //}

            //if (BuildObjects())
            //{
            //    BuildLead(false);
            //}

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_LineProfileGaugeSetting_Click(object sender, EventArgs e)
        {
            m_blnDragROIPrev = m_smVisionInfo.g_blnDragROI;

            m_smVisionInfo.g_blnDragROI = false;
            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\PointGauge.xml";

            if (m_objLead3DLineProfileForm == null)
                m_objLead3DLineProfileForm = new Lead3DLineProfileForm(m_smVisionInfo, m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge, strPath, m_smProductionInfo, 0);

            m_objLead3DLineProfileForm.Show();

            m_smVisionInfo.g_strSelectedPage = "Lead3DLineProfileGaugeSetting";
            this.Hide();
        }
        private void AutoDefinePitchGap()
        {
            m_smVisionInfo.g_arrLead3D[0].ClearTemplatePitchGap();
            int intTotalPadNo = m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber() - 1;

            int intFormLeadNo = 0;
            int intToLeadNo = 1;

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

                if (intFormLeadNo > intTotalPadNo)
                {
                    break;
                }

                if (intFormLeadNo >= intTotalPadNo)
                {
                    //break;
                    intFormLeadNo = intTotalPadNo;
                    intToLeadNo = 0;
                }

                if (intFormLeadNo == intToLeadNo)
                {
                    intFormLeadNo++;
                    intToLeadNo++;
                    continue;
                }
                
                //else if (!m_smVisionInfo.g_arrLead3D[0].IsPadEnable(intFormPadNo))
                //{
                //    //if (!blnBackward)
                //    //{
                //    intFormPadNo++;
                //    intToPadNo++;
                //    //}
                //    //else
                //    //{
                //    //    intFormPadNo--;
                //    //}
                //    continue;
                //}
                //else if (!m_smVisionInfo.g_arrLead3D[0].IsPadEnable(intToPadNo))
                //{
                //    //if (!blnBackward)
                //    //{
                //    intFormPadNo++;
                //    intToPadNo++;
                //    //}
                //    //else
                //    //{
                //    //    intFormPadNo--;
                //    //}
                //    continue;
                //}
                //else if (!m_smVisionInfo.g_arrLead3D[0].IsLeadAtPackageEdge(intFormLeadNo, intToLeadNo)) 
                //{
                //    //if (!blnBackward)
                //    //{
                //    intFormLeadNo++;
                //    intToLeadNo++;
                //    //}
                //    //else
                //    //{
                //    //    intFormPadNo--;
                //    //}
                //    continue;
                //}
                //else if (!m_smVisionInfo.g_arrLead3D[0].IsLeadAtPackageEdge(intToLeadNo, intFormLeadNo))   
                //{
                //    //if (!blnBackward)
                //    //{
                //    intFormLeadNo++;
                //    intToLeadNo++;
                //    //}
                //    //else
                //    //{
                //    //    intFormPadNo--;
                //    //}
                //    continue;
                //}
                else if (m_smVisionInfo.g_arrLead3D[0].CheckPitchGapLinkExist(intFormLeadNo, intToLeadNo))
                {
                    //if (!blnBackward)
                    //{
                    intFormLeadNo++;
                    intToLeadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else if (m_smVisionInfo.g_arrLead3D[0].CheckPitchGapLinkInLeadAlready(intFormLeadNo))
                {
                    //if (!blnBackward)
                    //{
                    intFormLeadNo++;
                    intToLeadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else if (!m_smVisionInfo.g_arrLead3D[0].CheckPitchGapLinkAvailable(intFormLeadNo, intToLeadNo))
                {
                    //if (!blnBackward)
                    //{
                    intFormLeadNo++;
                    intToLeadNo++;
                    //}
                    //else
                    //{
                    //    intFormPadNo--;
                    //}
                    continue;
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[0].SetPitchGap(intPitchGapIndex, intFormLeadNo, intToLeadNo);
                    intPitchGapIndex++;
                    //if (!blnBackward)
                    //{
                    intFormLeadNo++;
                    intToLeadNo++;
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

        private void txt_TipBuildAreaTolerance_Top_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) - Convert.ToInt32(txt_TipBuildAreaTolerance_Top.Text)) < m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY)
            {
                SRMMessageBox.Show("Tip Build Area Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_TipBuildAreaTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top = Convert.ToInt32(txt_TipBuildAreaTolerance_Top.Text);
            if (BuildObjects())
            {
                //BuildLead(false);
                FindCornerPoints();
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Bottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y + (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) + Convert.ToInt32(txt_TipBuildAreaTolerance_Bottom.Text)) > (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight))
            {
                SRMMessageBox.Show("Tip Build Area Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_TipBuildAreaTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Bottom.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Bottom = Convert.ToInt32(txt_TipBuildAreaTolerance_Bottom.Text);
            if (BuildObjects())
            {
                //BuildLead(false);
                FindCornerPoints();
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Left_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) - Convert.ToInt32(txt_TipBuildAreaTolerance_Left.Text)) < (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX))
            {
                SRMMessageBox.Show("Tip Build Area Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_TipBuildAreaTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left = Convert.ToInt32(txt_TipBuildAreaTolerance_Left.Text);
            if (BuildObjects())
            {
                //BuildLead(false);
                FindCornerPoints();
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Right_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X + (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) + Convert.ToInt32(txt_TipBuildAreaTolerance_Right.Text)) > (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth))
            {
                SRMMessageBox.Show("Tip Build Area Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_TipBuildAreaTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Right.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Right = Convert.ToInt32(txt_TipBuildAreaTolerance_Right.Text);
            if (BuildObjects())
            {
                //BuildLead(false);
                FindCornerPoints();
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Top_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) - Convert.ToInt32(txt_PkgToBaseTolerance_Top.Text)) < m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY)
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Top.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Top = Convert.ToInt32(txt_PkgToBaseTolerance_Top.Text);

            if (BuildObjects())
            {
                BuildLead(false);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Bottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y + (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) + Convert.ToInt32(txt_PkgToBaseTolerance_Bottom.Text)) > (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Bottom.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Bottom = Convert.ToInt32(txt_PkgToBaseTolerance_Bottom.Text);

            if (BuildObjects())
            {
                BuildLead(false);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Left_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) - Convert.ToInt32(txt_PkgToBaseTolerance_Left.Text)) < (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Left.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Left = Convert.ToInt32(txt_PkgToBaseTolerance_Left.Text);

            if (BuildObjects())
            {
                BuildLead(false);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Right_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X + (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) + Convert.ToInt32(txt_PkgToBaseTolerance_Right.Text)) > (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Right.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Right = Convert.ToInt32(txt_PkgToBaseTolerance_Right.Text);

            if (BuildObjects())
            {
                BuildLead(false);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLead3DPkgToBaseDrawing = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLead3DPkgToBaseDrawing = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdatePointGaugeDrawing()
        {

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


                ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

                for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
                {
                    ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, i);
                }

                //if (m_smVisionInfo.g_blnViewPackageImage)
                //{
                //    //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
                //    //{
                //    //    m_smVisionInfo.g_arrRotatedImages[1].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                //    //}
                //    //else
                //    {
                //        m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                //    }
                //}
            
           
            // After rotating the image, attach the rotated image into ROI again
            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Backup rotated image into local backup
            for (int i = m_arrLocalBackupRotatedImage.Count; i < 1; i++)
            {
                m_arrLocalBackupRotatedImage.Add(new ImageDrawing(true));
            }
            ImageDrawing objLocalBackupRotatedImage = m_arrLocalBackupRotatedImage[0];
            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref objLocalBackupRotatedImage);

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;

            if (m_intCurrentAngle == 270)
                lbl_OrientationAngle.Text = "-90";
            else
                lbl_OrientationAngle.Text = m_intCurrentAngle.ToString();

            RotatePrecise();
        }
        private void RotatePrecise()
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;
            btn_RotateAngle.Enabled = false;

            ROI objROI = new ROI();
            objROI = m_smVisionInfo.g_arrLeadROIs[0][0];

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);
            ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, 0);

            // After rotating the image, attach the rotated image into ROI again
            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Backup rotated image into local backup
            for (int i = m_arrLocalBackupRotatedImage.Count; i < 1; i++)
            {
                m_arrLocalBackupRotatedImage.Add(new ImageDrawing(true));
            }
            ImageDrawing objLocalBackupRotatedImage = m_arrLocalBackupRotatedImage[0];
            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref objLocalBackupRotatedImage);

            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                srmLabel37.Text = m_intCurrentPreciseDeg.ToString() + " deg";
            else
                srmLabel37.Text = m_intCurrentPreciseDeg.ToString() + " 度";
            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;
        }
        private void btn_ClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg++;
            RotatePrecise();
        }

        private void btn_CounterClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg--;
            RotatePrecise();
        }

        private void btn_AddDontCareROI_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Add(new ROI());
            m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count - 1].AttachImage(m_smVisionInfo.g_arrLeadROIs[m_smVisionInfo.g_intSelectedROI][0]);
            m_smVisionInfo.g_arrPolygon_Lead3D[m_smVisionInfo.g_intSelectedROI].Add(new Polygon());
            m_smVisionInfo.g_arrPolygon_Lead3D[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_arrPolygon_Lead3D[m_smVisionInfo.g_intSelectedROI].Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DeleteDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count == 0)
                return;

            m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_arrPolygon_Lead3D[m_smVisionInfo.g_intSelectedROI].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count - 1;
            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if ((m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                {
                    cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Lead3D[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
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

        private void cbo_DontCareAreaDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
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

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }
            m_smVisionInfo.g_arrPolygon_Lead3D[intSelectedROI][m_smVisionInfo.g_intSelectedDontCareROIIndex].UndoPolygon();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_Direction_Click(object sender, EventArgs e)
        {
            if (radioBtn_Horizontal.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    m_smVisionInfo.g_arrLead3D[i].ref_intLeadDirection = 0;
                }

                pic_LeadCount.BackgroundImage = pic_DirectionHorizontal.BackgroundImage;
                
                txt_LeadNumber_Top.Visible = false;
                txt_LeadNumber_Bottom.Visible = false;
                txt_LeadNumber_Left.Visible = true;
                txt_LeadNumber_Right.Visible = true;
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    m_smVisionInfo.g_arrLead3D[i].ref_intLeadDirection = 1;
                }

                pic_LeadCount.BackgroundImage = pic_DirectionVertical.BackgroundImage;

                txt_LeadNumber_Top.Visible = true;
                txt_LeadNumber_Bottom.Visible = true;

                txt_LeadNumber_Left.Visible = false;
                txt_LeadNumber_Right.Visible = false;
            }
            
                if (!m_blnInitDone)
                    return;

                int intNumberOfLead_Top = Convert.ToInt32(txt_LeadNumber_Top.Text);
                int intNumberOfLead_Bottom = Convert.ToInt32(txt_LeadNumber_Bottom.Text);
                int intNumberOfLead_Left = Convert.ToInt32(txt_LeadNumber_Left.Text);
                int intNumberOfLead_Right = Convert.ToInt32(txt_LeadNumber_Right.Text);

                //if (intNumberOfLead_Top == 0 && intNumberOfLead_Bottom == 0 && intNumberOfLead_Left == 0 && intNumberOfLead_Right == 0)
                //{
                //    SRMMessageBox.Show("At least 1 Lead must be selected.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    txt_LeadNumber_Top.Text = "1";
                //    intNumberOfLead_Top = Convert.ToInt32(txt_LeadNumber_Top.Text);
                //}

                if ((intNumberOfLead_Top == 0 && intNumberOfLead_Bottom == 0 && radioBtn_Vertical.Checked) || (intNumberOfLead_Left == 0 && intNumberOfLead_Right == 0 && radioBtn_Horizontal.Checked))
                {
                    SRMMessageBox.Show("At least 1 Lead must be selected.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    if (radioBtn_Vertical.Checked)
                    {
                        txt_LeadNumber_Top.Text = "1";
                        intNumberOfLead_Top = Convert.ToInt32(txt_LeadNumber_Top.Text);
                    }
                    else
                    {
                        txt_LeadNumber_Left.Text = "1";
                        intNumberOfLead_Left = Convert.ToInt32(txt_LeadNumber_Left.Text);
                    }
                }

                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                            //// For center ROI, intNumberOfLead will keep total lead of 4 side ROI.
                            //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead = intNumberOfLead_Top + intNumberOfLead_Bottom + intNumberOfLead_Left + intNumberOfLead_Right;
                            //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = intNumberOfLead_Top;
                            //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = intNumberOfLead_Bottom;
                            //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = intNumberOfLead_Left;
                            //m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = intNumberOfLead_Right;

                            if (radioBtn_Vertical.Checked)
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead = intNumberOfLead_Top + intNumberOfLead_Bottom;
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = intNumberOfLead_Top;
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = intNumberOfLead_Bottom;
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = 0;
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = 0;
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead = intNumberOfLead_Left + intNumberOfLead_Right;
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = 0;
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = 0;
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = intNumberOfLead_Left;
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = intNumberOfLead_Right;
                            }
                            break;
                        case 1:
                            m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                            if (radioBtn_Vertical.Checked)
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = intNumberOfLead_Top;
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Top = 0;
                            }
                            break;
                        case 2:
                            m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                            if (radioBtn_Vertical.Checked)
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = 0;
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Right = intNumberOfLead_Right;
                            }
                            break;
                        case 3:
                            m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                            if (radioBtn_Vertical.Checked)
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = intNumberOfLead_Bottom;
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Bottom = 0;
                            }
                            break;
                        case 4:
                            m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;

                            if (radioBtn_Vertical.Checked)
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = 0;
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intNumberOfLead_Left = intNumberOfLead_Left;
                            }
                            break;
                    }
                }

                UpdateReferenceCornerGUI();

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            
        }

        private void chk_Reset_Click(object sender, EventArgs e)
        {
            if (chk_Reset.Checked)
            {
                AutoDefinePitchGap();

                ReadLeadPitchToGrid(0, dgd_PitchGapSetting);
            }
            else
            {
                m_smVisionInfo.g_arrLead3D[0].LoadPitchGapLinkTemporary();

                ReadLeadPitchToGrid(0, dgd_PitchGapSetting);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_SelectROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedROI = cbo_SelectROI.SelectedIndex;
            m_smVisionInfo.g_intSelectedROIMask = 0x01 << m_smVisionInfo.g_intSelectedROI;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if ((m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                {
                    if (i < m_smVisionInfo.g_arrLeadROIs.Count)
                    {
                        if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                        {
                            m_smVisionInfo.g_arrLeadROIs[i][0].VerifyROIArea(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterY);

                            if (m_smVisionInfo.g_arrLead3DDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI)
                            {
                                if (m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > 0)
                                {
                                    m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count - 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (i < m_smVisionInfo.g_arrLeadROIs.Count)
                    {
                        if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                        {
                            m_smVisionInfo.g_arrLeadROIs[i][0].ClearDragHandle();
                        }
                    }
                }

            }

            m_smVisionInfo.g_blnUpdateSelectedROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void ClearDragHandler()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i < m_smVisionInfo.g_arrLeadROIs.Count)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                    {
                        m_smVisionInfo.g_arrLeadROIs[i][0].ClearDragHandle();
                    }
                }
            }
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void SelectROI(int intLeadIndex)
        {
            m_smVisionInfo.g_intSelectedROI = cbo_SelectROI.SelectedIndex = intLeadIndex;
            m_smVisionInfo.g_intSelectedROIMask = 0x01 << m_smVisionInfo.g_intSelectedROI;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if ((m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                {
                    if (i < m_smVisionInfo.g_arrLeadROIs.Count)
                    {
                        if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                        {
                            m_smVisionInfo.g_arrLeadROIs[i][0].VerifyROIArea(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterY);

                            if (m_smVisionInfo.g_arrLead3DDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI)
                            {
                                if (m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > 0)
                                {
                                    m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrLead3DDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count - 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (i < m_smVisionInfo.g_arrLeadROIs.Count)
                    {
                        if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                        {
                            m_smVisionInfo.g_arrLeadROIs[i][0].ClearDragHandle();
                        }
                    }
                }

            }

            m_smVisionInfo.g_blnUpdateSelectedROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}