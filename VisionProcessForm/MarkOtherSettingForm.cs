using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using VisionProcessing;
using SharedMemory;
using Microsoft.Win32;
using System.IO;

namespace VisionProcessForm
{
    public partial class MarkOtherSettingForm : Form
    {
        #region enum

        public enum selectedTabpageGroup { UnderOther = 0, UnderMark = 1, UnderMark2 = 2, UnderPackage = 3, UnderLead = 4 };

        #endregion

        #region Member Variables
        private string[] arrROIDirection = new string[5] { "Center ROI", "Top ROI", "Right ROI", "Bottom ROI", "Left ROI" };
        private int m_intPreviousSelectedImage = 0;
        private int m_intTotalTemplateBlobNo;
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private bool m_blnDragROIPrev = false;
        private int m_intUserGroup = 5;
        private string m_strSelectedRecipe;
        private bool m_blnTriggerOfflineTest = false;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private selectedTabpageGroup m_eSettingType = selectedTabpageGroup.UnderMark;
        private int m_intSelectedTabPage = 0;
        private bool m_blnWantSet1ToAll = false;
        private RectGaugeM4L m_objGauge;
        LeadLineProfileForm m_objLeadLineProfileForm;
        
        #endregion

        public MarkOtherSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intSelectedTabPage)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_intSelectedTabPage = intSelectedTabPage;
            m_smVisionInfo.g_intViewInspectionSetting = 1;
            m_intPreviousSelectedImage = m_smVisionInfo.g_intSelectedImage;

            
            DisableField2();
            UpdateGUI();
            UpdateTabPage();

            // Set display events
            m_smVisionInfo.g_blnViewROI = m_smVisionInfo.g_blnViewSearchROI = true;

            m_blnInitDone = true;

            if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                m_blnTriggerOfflineTest = true;
        }

        private void UpdateGUI()
        {
            switch (m_intSelectedTabPage)
            {
                case 0: // User press btn_OtherSetting 
                    if ((m_smVisionInfo.g_arrMarks[0].GetNumTemplates() <= 1) || !m_smVisionInfo.g_blnWantMultiTemplates)
                    {
                        panel_Template.Visible = false;
                        pnl_PageButton.Size = new Size(pnl_PageButton.Width, pnl_PageButton.Height - panel_Template.Height);
                        pnl_PageButton.Location = new Point(pnl_PageButton.Location.X, pnl_PageButton.Location.Y + panel_Template.Height);
                    }
                    else
                    {
                        cbo_TemplateNo.Items.Clear();
                        for (int i = 0; i < m_smVisionInfo.g_arrMarks[0].GetNumTemplates(); i++)
                        {
                            cbo_TemplateNo.Items.Add((i + 1));

                            if (i == 1)
                            {
                                pnl_Template2.Visible = true;
                                pnl_ExtraTemplate2.Visible = true;
                            }
                            else if (i == 2)
                            {
                                pnl_Template3.Visible = true;
                                pnl_ExtraTemplate3.Visible = true;
                            }
                            else if (i == 3)
                            {
                                pnl_Template4.Visible = true;
                                pnl_ExtraTemplate4.Visible = true;
                            }
                            else if (i == 4)
                            {
                                pnl_Template5.Visible = true;
                                pnl_ExtraTemplate5.Visible = true;
                            }
                            else if (i == 5)
                            {
                                pnl_Template6.Visible = true;
                                pnl_ExtraTemplate6.Visible = true;
                            }
                            else if (i == 6)
                            {
                                pnl_Template7.Visible = true;
                                pnl_ExtraTemplate7.Visible = true;
                            }
                            else if (i == 7)
                            {
                                pnl_Template8.Visible = true;
                                pnl_ExtraTemplate8.Visible = true;
                            }
                        }

                        cbo_TemplateNo.SelectedIndex = m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;
                    }

                    if (m_smVisionInfo.g_intMarkDefectInspectionMethod == 1)
                    {
                        pnl_MarkGrayValue.Visible = true;
                        pnl_Template1.Visible = false;
                        pnl_Template2.Visible = false;
                        pnl_Template3.Visible = false;
                        pnl_Template4.Visible = false;
                        pnl_Template5.Visible = false;
                        pnl_Template6.Visible = false;
                        pnl_Template7.Visible = false;
                        pnl_Template8.Visible = false;
                        pnl_ExtraTemplate1.Visible = false;
                        pnl_ExtraTemplate2.Visible = false;
                        pnl_ExtraTemplate3.Visible = false;
                        pnl_ExtraTemplate4.Visible = false;
                        pnl_ExtraTemplate5.Visible = false;
                        pnl_ExtraTemplate6.Visible = false;
                        pnl_ExtraTemplate7.Visible = false;
                        pnl_ExtraTemplate8.Visible = false;
                    }
                    else
                    {
                        pnl_MarkGrayValue.Visible = false;
                    }

                    chk_SetAllTemplates.Checked = m_blnWantSet1ToAll = m_smVisionInfo.g_blnWantSet1ToAll;
                    chk_SetAllTemplates.Visible = !m_smVisionInfo.g_blnWantSet1ToAll;//2020-05-11 ZJYEOH : Hide Set to All checkbox when user tick set 1 to all in advance setting

                    lbl_MarkThreshold_Template1.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 0).ToString(); //m_smVisionInfo.g_intSelectedTemplate
                  
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 1)
                        lbl_MarkThreshold_Template2.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 1).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 2)
                        lbl_MarkThreshold_Template3.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 2).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 3)
                        lbl_MarkThreshold_Template4.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 3).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 4)
                        lbl_MarkThreshold_Template5.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 4).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 5)
                        lbl_MarkThreshold_Template6.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 5).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 6)
                        lbl_MarkThreshold_Template7.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 6).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 7)
                        lbl_MarkThreshold_Template8.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 7).ToString();

                    lbl_ExtraMarkThreshold_Template1.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 0).ToString(); //m_smVisionInfo.g_intSelectedTemplate

                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 1)
                        lbl_ExtraMarkThreshold_Template2.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 1).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 2)
                        lbl_ExtraMarkThreshold_Template3.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 2).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 3)
                        lbl_ExtraMarkThreshold_Template4.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 3).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 4)
                        lbl_ExtraMarkThreshold_Template5.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 4).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 5)
                        lbl_ExtraMarkThreshold_Template6.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 5).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 6)
                        lbl_ExtraMarkThreshold_Template7.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 6).ToString();
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 7)
                        lbl_ExtraMarkThreshold_Template8.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 7).ToString();

                    txt_MinArea.Text = m_smVisionInfo.g_arrMarks[0].ref_intMinArea.ToString();
                    txt_EnhanceMark_LinkMark_HalfWidth.Text = m_smVisionInfo.g_arrMarks[0].ref_intEnhanceMark_LinkMark_HalfWidth.ToString();
                    txt_EnhanceMark_ReduceMark_HalfWidth.Text = m_smVisionInfo.g_arrMarks[0].ref_intEnhanceMark_ReduceNoise_HalfWidth.ToString();
                    m_eSettingType = selectedTabpageGroup.UnderMark;

                    // 2021 03 17 - CCENG: No long use intMissingMarkInspectionMethod
                    //if (m_smVisionInfo.g_arrMarks[0].ref_intMissingMarkInspectionMethod == 1)
                    //    lbl_ThinIteration.Text = "Missing Mark Thick Iteration";
                    //else
                    //    lbl_ThinIteration.Text = "Thin Iteration";

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
                        //if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && m_smVisionInfo.g_blnWantUseLeadPointOffsetMarkROI)
                        //    pnl_MarkROIOffset.Visible = true;
                        //else
                        //    pnl_MarkROIOffset.Visible = false;

                        if (m_smVisionInfo.g_arrLead != null)
                        {
                            if (m_smVisionInfo.g_arrLead[1].ref_blnSelected && m_smVisionInfo.g_arrLead[3].ref_blnSelected)
                            {
                                pic_Vertical.BringToFront();
                                pnl_LeadDontCareInwardTolerance_Top.Visible = true;
                                pnl_LeadDontCareInwardTolerance_Bottom.Visible = true;
                                pnl_LeadDontCareInwardTolerance_Left.Visible = false;
                                pnl_LeadDontCareInwardTolerance_Right.Visible = false;
                            }
                            else if (m_smVisionInfo.g_arrLead[2].ref_blnSelected && m_smVisionInfo.g_arrLead[4].ref_blnSelected)
                            {
                                pic_Horizontal.BringToFront();
                                pnl_LeadDontCareInwardTolerance_Top.Visible = false;
                                pnl_LeadDontCareInwardTolerance_Bottom.Visible = false;
                                pnl_LeadDontCareInwardTolerance_Left.Visible = true;
                                pnl_LeadDontCareInwardTolerance_Right.Visible = true;
                            }
                        }
                        pnl_LeadDontCareInwardTolerance.Visible = true;
                    }
                    // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
                    //txt_MarkROIOffsetTop.Text = m_smVisionInfo.g_arrMarks[0].ref_intMarkROIOffsetTop.ToString();
                    //txt_MarkROIOffsetRight.Text = m_smVisionInfo.g_arrMarks[0].ref_intMarkROIOffsetRight.ToString();
                    //txt_MarkROIOffsetBottom.Text = m_smVisionInfo.g_arrMarks[0].ref_intMarkROIOffsetBottom.ToString();
                    //txt_MarkROIOffsetLeft.Text = m_smVisionInfo.g_arrMarks[0].ref_intMarkROIOffsetLeft.ToString();

                    txt_LeadDontCareInwardTolerance_Top.Text = m_smVisionInfo.g_arrMarks[0].ref_intLeadDontCareInwardTolerance_Top.ToString();
                    txt_LeadDontCareInwardTolerance_Right.Text = m_smVisionInfo.g_arrMarks[0].ref_intLeadDontCareInwardTolerance_Right.ToString();
                    txt_LeadDontCareInwardTolerance_Bottom.Text = m_smVisionInfo.g_arrMarks[0].ref_intLeadDontCareInwardTolerance_Bottom.ToString();
                    txt_LeadDontCareInwardTolerance_Left.Text = m_smVisionInfo.g_arrMarks[0].ref_intLeadDontCareInwardTolerance_Left.ToString();
                    break;
                case 1: // User press Setting button under Package Button Group
                        //-------------------------------Package-------------------------------------------
                        //if (m_smVisionInfo.g_intPackageDefectInspectionMethod == 1)
                        //{
                        //    pnl_PackageGrayValue.Visible = true;
                        //    pnl_BrightDefectThreshold.Visible = false;
                        //    pnl_DarkDefectThreshold.Visible = false;
                        //}
                        //else
                        //{
                        //    pnl_PackageGrayValue.Visible = false;
                        //    pnl_BrightDefectThreshold.Visible = true;
                        //    pnl_DarkDefectThreshold.Visible = true;
                        //}
                    cbo_SelectROI.SelectedIndex = 0;
                    m_smVisionInfo.g_intSelectedUnit = 0;
                    if (m_smVisionInfo.g_arrPackageROIs != null)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                        {
                            if (m_smVisionInfo.g_arrPackageROIs[i].Count > 0)
                            {
                                if (i == cbo_SelectROI.SelectedIndex)
                                {
                                    m_smVisionInfo.g_arrPackageROIs[i][0].ClearDragHandle();
                                    m_smVisionInfo.g_arrPackageROIs[i][0].VerifyROIArea(m_smVisionInfo.g_arrPackageROIs[i][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrPackageROIs[i][0].ref_ROITotalCenterY);
                                }
                                else
                                    m_smVisionInfo.g_arrPackageROIs[i][0].ClearDragHandle();
                            }
                        }
                    }

                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        cbo_SelectROI.Visible = false;

                    m_objGauge = new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex);

                    switch (m_smVisionInfo.g_strVisionName)
                    {
                        case "MarkPkg":
                        case "MOPkg":
                        case "MOLiPkg":
                        case "InPocketPkg":
                        case "InPocketPkgPos":
                        case "IPMLiPkg":
                            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            {

                                if (m_smVisionInfo.g_blnWantGauge && m_smVisionInfo.g_arrOrientGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                                    m_objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
                                else
                                {
                                    if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                                    {
                                        m_objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
                                    }
                                }

                                if (m_smVisionInfo.g_arrPackageROIs.Count > m_smVisionInfo.g_intSelectedUnit)
                                {
                                    if (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedUnit)
                                    {
                                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].LoadROISetting(
                                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX,
                                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY,
                                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                                {
                                    //for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                                    //{
                                    //    if (i < m_smVisionInfo.g_arrPackageGaugeM4L.Count)
                                    //    {
                                    //        m_smVisionInfo.g_arrPackageGaugeM4L[i].ModifyGauge(m_smVisionInfo.g_arrPackageROIs[i][0]);
                                    //        m_smVisionInfo.g_arrPackageGaugeM4L[i].Measure(m_smVisionInfo.g_arrPackageROIs[i][0]);
                                    //    }
                                    //}

                                    m_objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
                                }
                            }
                            if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                            {

                                AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);

                            }
                            m_eSettingType = selectedTabpageGroup.UnderPackage;
                            break;
                        default:
                            if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > 0)
                                m_objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
                            break;
                    }

                    // 2019 07 25 - No more mark view inspection, but only package view inspection on image 1 or 2. So the panel3 is always invisible
                    // If Side View Bright Field Selected Image is 1, then not need to check defect on Mark View because Side View and Mark View image checking are same.
                    //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) == 0)
                    //{
                    panel3.Visible = false;
                    //}

                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
                    {
                        //pnl_BrightChipped.Visible = false;
                        //pnl_DarkChipped.Visible = false;
                        //tab_VisionControl.TabPages.Remove(tp_PkgSegment2);

                        lbl_Image2_BrightField.Text = "Image 2 : Bright Field";
                        lbl_Image2_BrightField.Text = lbl_Image2_BrightField.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

                        if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                        {
                            lbl_Image3_DarkField_Crack.Text = "Image 3 : Dark Field";
                            lbl_Image3_DarkField_Crack.Text = lbl_Image3_DarkField_Crack.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());
                        }
                    }

                    // Bright Field (simple)
                    lbl_BrightThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intBrightFieldLowThreshold.ToString();
                    txt_BrightMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intBrightFieldMinArea.ToString();
                    lbl_BrightDefect.Text = lbl_BrightDefect.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

                    // Dark Field (simple)
                    lbl_DarkLowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldLowThreshold.ToString();
                    lbl_DarkHighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldHighThreshold.ToString();
                    txt_DarkMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldMinArea.ToString();
                    lbl_DarkDefect.Text = lbl_DarkDefect.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

                    // Dark Field 2 (simple)
                    lbl_Dark2LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2LowThreshold.ToString();
                    lbl_Dark2HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2HighThreshold.ToString();
                    txt_Dark2MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2MinArea.ToString();
                    lbl_Dark2Defect.Text = lbl_Dark2Defect.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(4) + 1).ToString());

                    // Dark Field 3 (simple)
                    lbl_Dark3LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3LowThreshold.ToString();
                    lbl_Dark3HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3HighThreshold.ToString();
                    txt_Dark3MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3MinArea.ToString();
                    lbl_Dark3Defect.Text = lbl_Dark3Defect.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(6) + 1).ToString());

                    // Dark Field 4 (simple)
                    lbl_Dark4LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4LowThreshold.ToString();
                    lbl_Dark4HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4HighThreshold.ToString();
                    txt_Dark4MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4MinArea.ToString();
                    lbl_Dark4Defect.Text = lbl_Dark4Defect.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(7) + 1).ToString());

                    // Mark View
                    lbl_LowMarkViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewLowThreshold.ToString();
                    lbl_HighMarkViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewHighThreshold.ToString();
                    txt_MarkViewMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewMinArea.ToString();

                    // Package View
                    lbl_PackageViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intPkgViewThreshold.ToString();
                    txt_PkgMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intPkgViewMinArea.ToString();
                    lbl_Image2_BrightField.Text = lbl_Image2_BrightField.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

                    // Chip View Image 2
                    lbl_ChipViewImage2Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView1Threshold.ToString();
                    txt_ChipMinArea_1.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView1MinArea.ToString();
                    lbl_Image2_BrightField_Chip.Text = lbl_Image2_BrightField_Chip.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

                    // Chip View Image 3
                    lbl_ChipViewImage3Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView2Threshold.ToString();
                    txt_ChipMinArea_2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView2MinArea.ToString();
                    lbl_Image3_DarkField_Chip.Text = lbl_Image3_DarkField_Chip.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

                    // Crack View
                    lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewLowThreshold.ToString();
                    lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewHighThreshold.ToString();
                    txt_CrackMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewMinArea.ToString();
                    lbl_Image3_DarkField_Crack.Text = lbl_Image3_DarkField_Crack.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

                    // Void View Image 3
                    lbl_VoidViewImage3Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewThreshold.ToString();
                    txt_VoidMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewMinArea.ToString();
                    lbl_Image3_DarkField_Void.Text = lbl_Image3_DarkField_Void.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

                    // Mold Flash View Image 2
                    lbl_MoldFlashViewImage2Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold.ToString();
                    txt_MoldFlashMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashMinArea.ToString();
                    lbl_Image2_BrightField_MF.Text = lbl_Image2_BrightField_MF.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

                    //ROI Setting
                    txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();

                    txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark.ToString();
                    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark.ToString();
                    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark.ToString();
                    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark.ToString();

                    if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateBrightDarkROITolerance)
                    {
                        pnl_PackageROI_Dark.Visible = false;
                    }

                    txt_PkgStartPixelFromEdge_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2.ToString();
                    txt_PkgStartPixelFromRight_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2.ToString();
                    txt_PkgStartPixelFromBottom_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2.ToString();
                    txt_PkgStartPixelFromLeft_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2.ToString();

                    txt_PkgStartPixelFromEdge_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3.ToString();
                    txt_PkgStartPixelFromRight_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3.ToString();
                    txt_PkgStartPixelFromBottom_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3.ToString();
                    txt_PkgStartPixelFromLeft_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3.ToString();

                    txt_PkgStartPixelFromEdge_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4.ToString();
                    txt_PkgStartPixelFromRight_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4.ToString();
                    txt_PkgStartPixelFromBottom_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4.ToString();
                    txt_PkgStartPixelFromLeft_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4.ToString();

                    txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip.ToString();
                    txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip.ToString();
                    txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip.ToString();
                    txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip.ToString();

                    txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip.ToString();
                    txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip.ToString();
                    txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip.ToString();
                    txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip.ToString();

                    txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark.ToString();
                    txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip_Dark.ToString();
                    txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip_Dark.ToString();
                    txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip_Dark.ToString();

                    txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark.ToString();
                    txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip_Dark.ToString();
                    txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip_Dark.ToString();
                    txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip_Dark.ToString();

                    txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();

                    txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                    chk_SetToAll.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_ROI", false));

                    CheckPkgROISetting();
                    CheckPkgDarkROISetting();
                    CheckChipROISetting();
                    CheckChipDarkROISetting();
                    CheckPkgMoldROISetting();
                    break;
                case 2:
                    //---------------------------------Lead----------------------------------------------
                    m_intTotalTemplateBlobNo = 0;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;
                    bool blnfirstROI = true;
                    cbo_SelectROI_Lead.Items.Clear();

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

                    }

                    if (cbo_SelectROI_Lead.Items.Count > 0)
                        cbo_SelectROI_Lead.SelectedIndex = 0;

                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        {
                            if (i == 1)
                            {
                                srmLabel66.Visible = false;
                                txt_PkgToBaseTolerance_Top.Visible = false;
                            }
                            else if (i == 2)
                            {
                                srmLabel64.Visible = false;
                                txt_PkgToBaseTolerance_Right.Visible = false;
                            }
                            else if (i == 3)
                            {
                                srmLabel65.Visible = false;
                                txt_PkgToBaseTolerance_Bottom.Visible = false;
                            }
                            else if (i == 4)
                            {
                                srmLabel63.Visible = false;
                                txt_PkgToBaseTolerance_Left.Visible = false;
                            }
                            continue;
                        }

                        if (blnfirstROI)
                        {
                            lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead[i].ref_intThresholdValue.ToString();
                            txt_LeadMinArea.Text = m_smVisionInfo.g_arrLead[i].ref_intFilterMinArea.ToString();
                            blnfirstROI = false;

                            lbl_BaseLeadThreshold.Text = m_smVisionInfo.g_arrLead[i].ref_intThresholdValue_BaseLead.ToString();
                            txt_BaseLeadMinArea.Text = m_smVisionInfo.g_arrLead[i].ref_intFilterMinArea_BaseLead.ToString();

                            m_smVisionInfo.g_intSelectedROI = i;    // 2020 09 30 - Although most of the process in g_arrLead[0], but lead are separated to different direction group, and each direction have different threshold setting, min area etc.

                            if (cbo_SelectROI_Lead.Items.Contains(arrROIDirection[m_smVisionInfo.g_intSelectedROI]))
                            {
                                cbo_SelectROI_Lead.SelectedItem = arrROIDirection[m_smVisionInfo.g_intSelectedROI];
                                SelectLeadROI(m_smVisionInfo.g_intSelectedROI);
                            }
                        }
                        
                        int BaseValue = m_smVisionInfo.g_arrLead[i].GetBaseInwardOffset(0);
                        int TipValue = m_smVisionInfo.g_arrLead[i].GetTipInwardOffset(0);

                        if (BaseValue > 0)
                            txt_BaseOffset.Text = BaseValue.ToString();
                        if (TipValue > 0)
                            txt_TipOffset.Text = TipValue.ToString();
                        m_intTotalTemplateBlobNo += m_smVisionInfo.g_arrLead[i].GetSelectedObjectNumber();

                        if (i == 1) 
                        {
                            srmLabel66.Visible = true;
                            txt_PkgToBaseTolerance_Top.Visible = true;
                        }
                        else if (i == 2)
                        {
                            srmLabel64.Visible = true;
                            txt_PkgToBaseTolerance_Right.Visible = true;
                        }
                        else if (i == 3)
                        {
                            srmLabel65.Visible = true;
                            txt_PkgToBaseTolerance_Bottom.Visible = true;
                        }
                        else if (i == 4)
                        {
                            srmLabel63.Visible = true;
                            txt_PkgToBaseTolerance_Left.Visible = true;
                        }

                        //if (m_smVisionInfo.g_arrLead[i].ref_intLeadDirection == 0) // Horizontal
                        //{
                        //    srmLabel66.Visible = false;
                        //    txt_PkgToBaseTolerance_Top.Visible = false;
                        //    srmLabel65.Visible = false;
                        //    txt_PkgToBaseTolerance_Bottom.Visible = false;
                        //}
                        //else
                        //{
                        //    srmLabel63.Visible = false;
                        //    txt_PkgToBaseTolerance_Left.Visible = false;
                        //    srmLabel64.Visible = false;
                        //    txt_PkgToBaseTolerance_Right.Visible = false;
                        //}
                    }

                    if (m_intTotalTemplateBlobNo > 0)
                    {
                        for (int i = 0; i < m_intTotalTemplateBlobNo; i++)
                        {
                            if (!cbo_LeadNo.Items.Contains("Lead " + (i + 1).ToString()))
                                cbo_LeadNo.Items.Add("Lead " + (i + 1).ToString());
                        }
                        cbo_LeadNo.SelectedIndex = 0;
                    }
                    m_eSettingType = selectedTabpageGroup.UnderLead;
                    RegistryKey key2 = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey2 = key2.CreateSubKey("SVG\\AutoMode");
                    chk_SetToAllLeads.Checked = Convert.ToBoolean(subkey2.GetValue("SetToAllLead", false));

                    if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                    {
                        txt_PkgToBaseTolerance_Top.Text = m_smVisionInfo.g_arrLead[1].ref_intPkgToBaseTolerance_Top.ToString();
                        txt_PkgToBaseTolerance_Bottom.Text = m_smVisionInfo.g_arrLead[3].ref_intPkgToBaseTolerance_Bottom.ToString();
                        txt_PkgToBaseTolerance_Left.Text = m_smVisionInfo.g_arrLead[4].ref_intPkgToBaseTolerance_Left.ToString();
                        txt_PkgToBaseTolerance_Right.Text = m_smVisionInfo.g_arrLead[2].ref_intPkgToBaseTolerance_Right.ToString();
                    }
                    else
                    {
                        if (m_smVisionInfo.g_arrLead[0].ref_intRotationMethod == 2)
                        {
                            txt_PkgToBaseTolerance_Top.Text = m_smVisionInfo.g_arrLead[1].ref_intPkgToBaseTolerance_Top.ToString();
                            txt_PkgToBaseTolerance_Bottom.Text = m_smVisionInfo.g_arrLead[3].ref_intPkgToBaseTolerance_Bottom.ToString();
                            txt_PkgToBaseTolerance_Left.Text = m_smVisionInfo.g_arrLead[4].ref_intPkgToBaseTolerance_Left.ToString();
                            txt_PkgToBaseTolerance_Right.Text = m_smVisionInfo.g_arrLead[2].ref_intPkgToBaseTolerance_Right.ToString();
                        }
                        else
                        group_PkgToBaseToleranceSetting.Visible = false;
                    }
                    break;
            }
        }

        private void DisableField()
        {
            string strChild1 = "Setting Page";
            string strChild2 = "Save Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }

            switch (m_intSelectedTabPage)
            {
                case 0:
                    tab_VisionControl.TabPages.Remove(tp_LeadSegment);
                    tab_VisionControl.TabPages.Remove(tp_LeadOther);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                    tab_VisionControl.TabPages.Remove(tp_ROI);
                    tab_VisionControl.TabPages.Remove(tp_ROI2);
                    tab_VisionControl.TabPages.Remove(tp_ROI3);
                    if (!m_smVisionInfo.g_blnSeparateExtraMarkThreshold)
                    {
                        tab_VisionControl.TabPages.Remove(tp_MarkSegment2);
                    }
                    break;
                case 1: // Open by Package Button
                    tab_VisionControl.TabPages.Remove(tp_LeadSegment);
                    tab_VisionControl.TabPages.Remove(tp_LeadOther);
                    tab_VisionControl.TabPages.Remove(tp_MarkSegment);
                    tab_VisionControl.TabPages.Remove(tp_MarkSegment2);
                    tab_VisionControl.TabPages.Remove(tp_Other);
                    if (m_smVisionInfo.g_arrPackage != null && m_smVisionInfo.g_arrPackage.Count > 0)
                    {
                        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnUseDetailDefectCriteria)
                        {
                            tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                        }
                        else
                        {


                            pnl_DarkVoid.Visible = false;
                            pnl_BrightChipped.Dock = DockStyle.None;
                            pnl_DarkChipped.Dock = DockStyle.None;


                            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                            {
                                tp_PkgSegmentSimple.Controls.Add(pnl_BrightChipped);
                                tp_PkgSegmentSimple.Controls.Add(pnl_DarkChipped);

                                pnl_BrightChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                                pnl_DarkChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightChipped.Location.Y + pnl_BrightChipped.Size.Height);


                                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                                {
                                    pnl_BrightMold.Dock = DockStyle.None;
                                    tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                                    pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);

                                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                                    {
                                        tp_PkgSegment2.Controls.Add(pnl_DarkCrack);
                                    }

                                }
                                else
                                {
                                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                                    {
                                        pnl_DarkCrack.Dock = DockStyle.None;
                                        tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                                        pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);
                                    }
                                }

                                tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                                if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting || !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                                    tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                            }
                            else
                            {
                                pnl_BrightMold.Dock = DockStyle.None;
                                pnl_DarkCrack.Dock = DockStyle.None;

                                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                                {
                                    tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                                    pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                                }

                                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                                {
                                    tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                                    pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                                }
                                else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                                {
                                    tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                                    pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightMold.Location.Y + pnl_BrightMold.Size.Height);
                                }
                                tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                                tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                            }

                            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                            {
                                gbox_Chip.Visible = false;
                                gbox_Chip_Dark.Visible = false;
                                gbox_Mold.Location = new Point(gbox_Chip.Location.X, gbox_Chip.Location.Y);
                            }

                            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                gbox_Mold.Visible = false;
                            }


                        }
                    }

                    break;
                case 2:
                    tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                    tab_VisionControl.TabPages.Remove(tp_ROI);
                    tab_VisionControl.TabPages.Remove(tp_ROI2);
                    tab_VisionControl.TabPages.Remove(tp_ROI3);
                    tab_VisionControl.TabPages.Remove(tp_MarkSegment);
                    tab_VisionControl.TabPages.Remove(tp_MarkSegment2);
                    tab_VisionControl.TabPages.Remove(tp_Other);
                    break;
            }
        }
        private int GetUserRightGroup_Child3(string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(Child2, Child3);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild3Group(Child2, Child3);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
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
                    return m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(Child2, Child3);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(Child2, Child3);
                    break;
                case "Seal":
                    return m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                case "Barcode":
                    return m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(Child2, Child3);
                    break;
            }

            return 1;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            switch (m_intSelectedTabPage)
            {
                case 0:
                    tab_VisionControl.TabPages.Remove(tp_LeadSegment);
                    tab_VisionControl.TabPages.Remove(tp_LeadOther);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                    tab_VisionControl.TabPages.Remove(tp_ROI);
                    tab_VisionControl.TabPages.Remove(tp_ROI2);
                    tab_VisionControl.TabPages.Remove(tp_ROI3);
                    if (!m_smVisionInfo.g_blnSeparateExtraMarkThreshold)
                    {
                        tab_VisionControl.TabPages.Remove(tp_MarkSegment2);
                    }
                    break;
                case 1: // Open by Package Button
                    if (m_smVisionInfo.g_intPackageDefectInspectionMethod == 1)
                    {
                        pnl_PackageGrayValue.Visible = true;
                        pnl_BrightDefectThreshold.Visible = false;
                        pnl_DarkDefectThreshold.Visible = false;
                    }
                    else
                    {
                        pnl_PackageGrayValue.Visible = false;
                        pnl_BrightDefectThreshold.Visible = true;
                        pnl_DarkDefectThreshold.Visible = true;
                    }

                    tab_VisionControl.TabPages.Remove(tp_LeadSegment);
                    tab_VisionControl.TabPages.Remove(tp_LeadOther);
                    tab_VisionControl.TabPages.Remove(tp_MarkSegment);
                    tab_VisionControl.TabPages.Remove(tp_MarkSegment2);
                    tab_VisionControl.TabPages.Remove(tp_Other);
                    if (m_smVisionInfo.g_arrPackage != null && m_smVisionInfo.g_arrPackage.Count > 0)
                    {
                        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnUseDetailDefectCriteria)
                        {
                            tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                        }
                        else
                        {
                            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                            {
                                tab_VisionControl.TabPages.Remove(tp_ROI2);
                            }

                            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                            { 
                                pnl_DarkDefect2.Visible = true;
                            }
                            else
                            {
                                pnl_DarkDefect2.Visible = false;
                            }
                            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                            {
                                pnl_DarkDefect3.Visible = true;
                            }
                            else
                            {
                                pnl_DarkDefect3.Visible = false;
                            }
                            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                            {
                                pnl_DarkDefect4.Visible = true;
                            }
                            else
                            {
                                pnl_DarkDefect4.Visible = false;
                            }

                            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                pnl_MoldFlashROI.Visible = true;
                            }
                            else
                            {
                                pnl_MoldFlashROI.Visible = false;
                            }

                            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                            {
                                tab_VisionControl.TabPages.Remove(tp_ROI3);
                            }
                            else
                            {
                                if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                                {
                                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                        tp_ROI3.Text = "ROI Setting 2";
                                    else
                                        tp_ROI3.Text = "ROI 设置 2";
                                }

                                if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                                    pnl_DarkField2ROI.Visible = false;
                                else
                                    pnl_DarkField2ROI.Visible = true;

                                if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                                    pnl_DarkField3ROI.Visible = false;
                                else
                                    pnl_DarkField3ROI.Visible = true;

                                if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                                    pnl_DarkField4ROI.Visible = false;
                                else
                                    pnl_DarkField4ROI.Visible = true;
                            }
                            tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                            
                            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                pnl_BrightMold.Visible = false;
                            }

                            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                            {
                                pnl_DarkCrack.Visible = false;
                            }

                            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                            {
                                pnl_DarkChipped.Visible = false;
                                pnl_BrightChipped.Visible = false;
                            }

                            if(!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting &&
                                !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting &&
                                !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                                tab_VisionControl.TabPages.Remove(tp_PkgSegment);

                            //pnl_DarkVoid.Visible = false;
                            //pnl_BrightChipped.Dock = DockStyle.None;
                            //pnl_DarkChipped.Dock = DockStyle.None;

                            //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
                            //{
                            //    tp_PkgSegmentSimple.Controls.Add(pnl_BrightChipped);
                            //    tp_PkgSegmentSimple.Controls.Add(pnl_DarkChipped);

                            //    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                            //    {
                            //        pnl_BrightChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect2.Location.Y + pnl_DarkDefect2.Size.Height);
                            //        pnl_DarkChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightChipped.Location.Y + pnl_BrightChipped.Size.Height);

                            //        //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                            //        //{
                            //        //    tp_PkgSegment2.Controls.Add(pnl_DarkCrack);
                            //        //}

                            //        if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            //        {
                            //            pnl_BrightMold.Visible = false;
                            //        }

                            //        tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                            //        if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            //            tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                            //    }
                            //    else
                            //    {
                            //        pnl_BrightChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            //        pnl_DarkChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightChipped.Location.Y + pnl_BrightChipped.Size.Height);


                            //        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            //        {
                            //            pnl_BrightMold.Dock = DockStyle.None;
                            //            tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                            //            pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);

                            //            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                            //            {
                            //                tp_PkgSegment2.Controls.Add(pnl_DarkCrack);
                            //            }

                            //        }
                            //        else
                            //        {
                            //            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
                            //            {
                            //                pnl_DarkCrack.Dock = DockStyle.None;
                            //                tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                            //                pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);
                            //            }
                            //            //tab_VisionControl.TabPages.Remove(tp_ROI2);
                            //        }

                            //        tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                            //        if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting || !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            //            tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                            //    }
                            //}
                            //else
                            //{
                            //    pnl_BrightMold.Dock = DockStyle.None;
                            //    pnl_DarkCrack.Dock = DockStyle.None;
                            //    //pnl_MoldFlashROI.Dock = DockStyle.None;

                            //    //if (tp_ROI.Contains(pnl_ChippedOffROI))
                            //    //    tp_ROI.Controls.Remove(pnl_ChippedOffROI);
                            //    //if (tp_ROI.Contains(pnl_ChippedOffROI_Dark))
                            //    //    tp_ROI.Controls.Remove(pnl_ChippedOffROI_Dark);

                            //    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            //    {
                            //        tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                            //        //tp_ROI.Controls.Add(pnl_MoldFlashROI);
                            //        //pnl_MoldFlashROI.Location = new Point(pnl_PackageROI_Bright.Location.X, pnl_PackageROI_Bright.Location.Y + pnl_PackageROI_Bright.Size.Height);
                            //        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                            //        {
                            //            pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect2.Location.Y + pnl_DarkDefect2.Size.Height);
                            //        }
                            //        else
                            //        {
                            //            pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            //        }
                            //    }

                            //    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            //    {
                            //        tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                            //        if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
                            //        {
                            //            pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect2.Location.Y + pnl_DarkDefect2.Size.Height);
                            //        }
                            //        else
                            //        {
                            //            pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            //        }
                            //    }
                            //    else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting && m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                            //    {
                            //        tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                            //        pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightMold.Location.Y + pnl_BrightMold.Size.Height);
                            //    }
                            //    tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                            //    tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                            //    //tab_VisionControl.TabPages.Remove(tp_ROI2);
                            //}

                        }
                    }

                    break;
                case 2:
                    tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                    tab_VisionControl.TabPages.Remove(tp_ROI);
                    tab_VisionControl.TabPages.Remove(tp_ROI2);
                    tab_VisionControl.TabPages.Remove(tp_ROI3);
                    tab_VisionControl.TabPages.Remove(tp_MarkSegment);
                    tab_VisionControl.TabPages.Remove(tp_MarkSegment2);
                    tab_VisionControl.TabPages.Remove(tp_Other);

                    if (!m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                    {
                        pnl_BaseLeadThreshold.Visible = false;
                    }
                    break;
            }

            string strChild1 = "Setting Page";
            string strChild2 = "";

            strChild2 = "Mark Threshold button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                //btn_Threshold_Template1.Enabled = false;
                pnl_MarkGrayValue.Enabled = false;
                pnl_Template1.Enabled = false;
                pnl_Template2.Enabled = false;
                pnl_Template3.Enabled = false;
                pnl_Template4.Enabled = false;
                pnl_Template5.Enabled = false;
                pnl_Template6.Enabled = false;
                pnl_Template7.Enabled = false;
                pnl_Template8.Enabled = false;

                pnl_MarkGrayValue.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_Template1.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_Template2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_Template3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_Template4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_Template5.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_Template6.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_Template7.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_Template8.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Extra Mark Threshold button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_ExtraTemplate1.Enabled = false;
                pnl_ExtraTemplate2.Enabled = false;
                pnl_ExtraTemplate3.Enabled = false;
                pnl_ExtraTemplate4.Enabled = false;
                pnl_ExtraTemplate5.Enabled = false;
                pnl_ExtraTemplate6.Enabled = false;
                pnl_ExtraTemplate7.Enabled = false;
                pnl_ExtraTemplate8.Enabled = false;

                pnl_ExtraTemplate1.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_ExtraTemplate2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_ExtraTemplate3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_ExtraTemplate4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_ExtraTemplate5.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_ExtraTemplate6.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_ExtraTemplate7.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_ExtraTemplate8.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Mark Min Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_MinArea.Enabled = false;
                txt_MinArea.Visible = lbl_MinArea.Visible = srmLabel5.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Missing Mark Iteration";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                btn_iterationsettings.Enabled = false;
            }

            strChild2 = "Excess Mark Iteration";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                btn_iterationsettings.Enabled = false;
            }

            strChild2 = "Mark Configuration Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                btn_MarkConfig.Enabled = false;
                btn_MarkConfig.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Enhance Mark - Link Mark";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_EnhanceMark_LinkMark_HalfWidth.Enabled = false;
                txt_EnhanceMark_LinkMark_HalfWidth.Visible = srmLabel78.Visible = srmLabel55.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Enhance Mark - Reduce Noise";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                txt_EnhanceMark_ReduceMark_HalfWidth.Enabled = false;
                txt_EnhanceMark_ReduceMark_HalfWidth.Visible = srmLabel51.Visible = srmLabel9.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            //strChild2 = "Mark ROI Lead Base Point Offset Setting";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            //{
            //    pnl_MarkROIOffset.Enabled = false;
            //    pnl_MarkROIOffset.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            //}

            strChild2 = "Lead Dont Care Inward Tolerance Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                grp_LeadDontCareInwardTolerance.Enabled = false;
                grp_LeadDontCareInwardTolerance.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }
            
            strChild2 = "Bright Field Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_BrightDefect.Enabled = false;
                pnl_BrightDefect.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Dark Field Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_DarkDefect.Enabled = false;
                pnl_DarkDefect.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Dark Field 2 Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_DarkDefect2.Enabled = false;
                pnl_DarkDefect2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Dark Field 3 Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_DarkDefect3.Enabled = false;
                pnl_DarkDefect3.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Dark Field 4 Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_DarkDefect4.Enabled = false;
                pnl_DarkDefect4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Chipped Off Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_BrightChipped.Enabled = false;
                pnl_DarkChipped.Enabled = false;
                pnl_BrightChipped.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_DarkChipped.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Crack Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_DarkCrack.Enabled = false;
                pnl_DarkCrack.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Mold Flash Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_BrightMold.Enabled = false;
                pnl_BrightMold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Package ROI Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                gbox_Pkg_Bright.Enabled = false;
                gbox_Pkg_Dark.Enabled = false;
                gbox_Chip.Enabled = false;
                gbox_Chip_Dark.Enabled = false;
                gbox_Mold.Enabled = false;
                cbo_ImageNo.Enabled = false;
                chk_SetToAll.Enabled = false;
                cbo_SelectROI.Enabled = false;
                gbox_Dark2ROI.Enabled = false;
                gbox_Dark3ROI.Enabled = false;
                gbox_Dark4ROI.Enabled = false;

                gbox_Pkg_Bright.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Pkg_Dark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Chip_Dark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Mold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                cbo_ImageNo.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAll.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                cbo_SelectROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Dark2ROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Dark3ROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Dark4ROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Lead Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                cbo_SelectROI_Lead.Enabled = false;
                pnl_LeadThreshold.Enabled = false;

                cbo_SelectROI_Lead.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_LeadThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Base Lead Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                pnl_BaseLeadThreshold.Enabled = false;
                pnl_BaseLeadThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Line Profile Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                btn_LineProfileGaugeSetting.Enabled = false;
                btn_LineProfileGaugeSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Inward Offset";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                grpBox_InwardOffset.Enabled = false;
                grpBox_InwardOffset.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild2 = "Package To Base Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2))
            {
                group_PkgToBaseToleranceSetting.Enabled = false;
                group_PkgToBaseToleranceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

        }


        private void SaveGeneralSetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "General.xml", false);
            
            STDeviceEdit.CopySettingFile(strFolderPath, "General.xml");
            objFile.WriteSectionElement("TemplateCounting");

            objFile.WriteElement1Value("TotalUnits", m_smVisionInfo.g_intUnitsOnImage);
            m_smVisionInfo.g_intTotalGroup = 1; //Group count is always 1
            objFile.WriteElement1Value("TotalGroups", m_smVisionInfo.g_intTotalGroup);
            objFile.WriteElement1Value("TotalTemplates", m_smVisionInfo.g_intTotalTemplates);
            objFile.WriteElement1Value("TemplateMask", m_smVisionInfo.g_intTemplateMask);
            objFile.WriteElement1Value("TemplatePriority", m_smVisionInfo.g_intTemplatePriority);

            //objFile.WriteSectionElement("PackageSetting");
            //objFile.WriteElement1Value("CheckPackage", m_smVisionInfo.g_blnCheckPackage); // 2019 01 14 - CCENG: No more using m_smVisionInfo.g_blnCheckPackage. Will replace by m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantInspectPackage() in InspectionOptionForm

            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Mark Other Setting", m_smProductionInfo.g_strLotID);
            
        }

        private void SaveMarkSettings(string strFolderPath)
        {
            if (!tab_VisionControl.Controls.Contains(tp_MarkSegment))
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                
                STDeviceEdit.CopySettingFile(strFolderPath, "Template.xml");
                m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath, false);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Mark Other Setting", m_smProductionInfo.g_strLotID);
                
            }
        }

        private void SaveLeadSettings(string strFolderPath)
        {
            if (!tab_VisionControl.Controls.Contains(tp_LeadSegment))
                return;

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

                
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrLead[i].SaveLead(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead Other Setting", m_smProductionInfo.g_strLotID);
                
            }
        }

        private void SavePackageSettings(string strFolderPath)
        {
            if (!tab_VisionControl.Controls.Contains(tp_PkgSegment) && !tab_VisionControl.Controls.Contains(tp_PkgSegment2) && !tab_VisionControl.Controls.Contains(tp_PkgSegmentSimple))
                return;
            
            STDeviceEdit.CopySettingFile(strFolderPath, "\\Settings.xml");
            //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SavePackage(strFolderPath + "\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
            
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].SavePackage(strFolderPath + "\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                else //2020-11-13 ZJYEOH : Unit 2 setting save to another xml file
                    m_smVisionInfo.g_arrPackage[u].SavePackage(strFolderPath + "\\Settings2.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                m_smVisionInfo.g_arrPackage[u].SetBlobSettings(m_smVisionInfo.g_arrPackage[u].ref_intPkgViewThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intPkgViewMinArea,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewHighThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewLowThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewMinArea);
            }
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package Other Setting", m_smProductionInfo.g_strLotID);
            
        }

        /// <summary>
        /// Load general template setting
        /// </summary>
        /// <param name="strPath">selected path</param>
        private void LoadGeneralSetting(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("TemplateCounting");
            m_smVisionInfo.g_intTotalUnits = objFile.GetValueAsInt("TotalUnits", 0, 1);
            m_smVisionInfo.g_intTotalGroup = objFile.GetValueAsInt("TotalGroups", 0, 1);
            m_smVisionInfo.g_intTotalTemplates = objFile.GetValueAsInt("TotalTemplates", 0, 1);
            m_smVisionInfo.g_intTemplateMask = objFile.GetValueAsInt("TemplateMask", 0, 1);
            m_smVisionInfo.g_intTemplatePriority = objFile.GetValueAsLong("TemplatePriority", 0, 1);
        }

        private void LoadMarkSettings(string strFolderPath)
        {
            if (!tab_VisionControl.Controls.Contains(tp_MarkSegment))
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                m_smVisionInfo.g_arrMarks[u].LoadTemplateOCR(strFolderPath);
            }
        }

        private void LoadLeadSetting(string strFolderPath)
        {
            if (!tab_VisionControl.Controls.Contains(tp_LeadSegment))
                return;

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

                m_smVisionInfo.g_arrLead[i].LoadLead(strFolderPath + "Template\\Template.xml", strSectionName, m_smVisionInfo.g_arrImages.Count);

                m_smVisionInfo.g_arrLead[i].LoadLeadTemplateImage(strFolderPath + "Template\\", i);

            }
        }

        public void LoadPackageSetting(string strFolderPath)
        {
            if (!tab_VisionControl.Controls.Contains(tp_PkgSegment) && !tab_VisionControl.Controls.Contains(tp_PkgSegment2) && !tab_VisionControl.Controls.Contains(tp_PkgSegmentSimple))
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].LoadPackage(strFolderPath + "\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                else
                {
                    if (File.Exists(strFolderPath + "\\Settings2.xml"))
                        m_smVisionInfo.g_arrPackage[u].LoadPackage(strFolderPath + "\\Settings2.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                    else
                        m_smVisionInfo.g_arrPackage[u].LoadPackage(strFolderPath + "\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                }
                m_smVisionInfo.g_arrPackage[u].SetBlobSettings(m_smVisionInfo.g_arrPackage[u].ref_intPkgViewThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intPkgViewMinArea,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewHighThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewLowThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewMinArea);

                AddTrainROI(m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge, m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight, m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom, m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft);
            }

        }

        private void UpdateTabPage()
        {
            switch (m_eSettingType)
            {
                case selectedTabpageGroup.UnderMark:
                    lbl_PageLabel.Text = "Mark Threshold Setting";
                    tp_MarkSegment.Controls.Add(pnl_PageButton);
                    m_smVisionInfo.g_intSelectedSetting = 0;
                    break;
                case selectedTabpageGroup.UnderMark2:
                    lbl_PageLabel.Text = "Mark Threshold Setting";
                    tp_MarkSegment2.Controls.Add(pnl_PageButton);
                    m_smVisionInfo.g_intSelectedSetting = 0;
                    break;
                case selectedTabpageGroup.UnderOther:
                    //lbl_PageLabel.Text = "Other Setting";
                    //tp_Other.Controls.Add(pnl_PageButton);
                    m_smVisionInfo.g_intSelectedSetting = 0;
                    break;
                case selectedTabpageGroup.UnderLead:
                    m_smVisionInfo.g_intSelectedSetting = 1;
                    break;
                case selectedTabpageGroup.UnderPackage:
                    lbl_PageLabel.Text = "Package Threshold Setting";
                    tp_MarkSegment.Controls.Add(pnl_PageButton);
                    tp_PkgSegmentSimple.Controls.Add(cbo_SelectROI);
                    m_smVisionInfo.g_intSelectedSetting = 2;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateLeadSelectedROISetting(int intSelectedROIIndex)
        {
            m_blnUpdateSelectedROISetting = true;

            lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead[intSelectedROIIndex].ref_intThresholdValue.ToString();
            txt_LeadMinArea.Text = m_smVisionInfo.g_arrLead[intSelectedROIIndex].ref_intFilterMinArea.ToString();

            lbl_BaseLeadThreshold.Text = m_smVisionInfo.g_arrLead[intSelectedROIIndex].ref_intThresholdValue_BaseLead.ToString();
            txt_BaseLeadMinArea.Text = m_smVisionInfo.g_arrLead[intSelectedROIIndex].ref_intFilterMinArea_BaseLead.ToString();


            m_blnUpdateSelectedROISetting = false;
        }
        private void UpdatePackageSelectedUnitSetting()
        {
            ////-------------------------------Package-------------------------------------------
            ////if (m_smVisionInfo.g_intPackageDefectInspectionMethod == 1)
            ////{
            ////    pnl_PackageGrayValue.Visible = true;
            ////    pnl_BrightDefectThreshold.Visible = false;
            ////    pnl_DarkDefectThreshold.Visible = false;
            ////}
            ////else
            ////{
            ////    pnl_PackageGrayValue.Visible = false;
            ////    pnl_BrightDefectThreshold.Visible = true;
            ////    pnl_DarkDefectThreshold.Visible = true;
            ////}

            //m_objGauge = new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex);

            //switch (m_smVisionInfo.g_strVisionName)
            //{
            //    case "MarkPkg":
            //    case "MOPkg":
            //    case "MOLiPkg":
            //    case "InPocketPkg":
            //    case "InPocketPkgPos":
            //    case "IPMLiPkg":
            //        if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            //        {

            //            if (m_smVisionInfo.g_arrOrientGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
            //                m_objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit];

            //            if (m_smVisionInfo.g_arrPackageROIs.Count > m_smVisionInfo.g_intSelectedUnit)
            //            {
            //                if (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit].Count > 0)
            //                {
            //                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].LoadROISetting(
            //                        m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX,
            //                        m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY,
            //                        m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
            //                        m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
            //            {
            //                //for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
            //                //{
            //                //    if (i < m_smVisionInfo.g_arrPackageGaugeM4L.Count)
            //                //    {
            //                //        m_smVisionInfo.g_arrPackageGaugeM4L[i].ModifyGauge(m_smVisionInfo.g_arrPackageROIs[i][0]);
            //                //        m_smVisionInfo.g_arrPackageGaugeM4L[i].Measure(m_smVisionInfo.g_arrPackageROIs[i][0]);
            //                //    }
            //                //}

            //                m_objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
            //            }
            //        }
            //        if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
            //        {

            //            AddTrainROI(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft);

            //        }
            //        m_eSettingType = selectedTabpageGroup.UnderPackage;
            //        break;
            //    default:
            //        if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > 0)
            //            m_objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
            //        break;
            //}

            //// 2019 07 25 - No more mark view inspection, but only package view inspection on image 1 or 2. So the panel3 is always invisible
            //// If Side View Bright Field Selected Image is 1, then not need to check defect on Mark View because Side View and Mark View image checking are same.
            ////if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) == 0)
            ////{
            //panel3.Visible = false;
            ////}

            //if (!m_smVisionInfo.g_blnWantUseDetailThreshold_Package)
            //{
            //    //pnl_BrightChipped.Visible = false;
            //    //pnl_DarkChipped.Visible = false;
            //    //tab_VisionControl.TabPages.Remove(tp_PkgSegment2);

            //    lbl_Image2_BrightField.Text = "Image 2 : Bright Field";
            //    lbl_Image2_BrightField.Text = lbl_Image2_BrightField.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            //    if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
            //    {
            //        lbl_Image3_DarkField_Crack.Text = "Image 3 : Dark Field";
            //        lbl_Image3_DarkField_Crack.Text = lbl_Image3_DarkField_Crack.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());
            //    }
            //}

            // Bright Field (simple)
            lbl_BrightThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intBrightFieldLowThreshold.ToString();
            txt_BrightMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intBrightFieldMinArea.ToString();
            lbl_BrightDefect.Text = lbl_BrightDefect.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            // Dark Field (simple)
            lbl_DarkLowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldLowThreshold.ToString();
            lbl_DarkHighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldHighThreshold.ToString();
            txt_DarkMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkFieldMinArea.ToString();
            lbl_DarkDefect.Text = lbl_DarkDefect.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            // Dark Field 2 (simple)
            lbl_Dark2LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2LowThreshold.ToString();
            lbl_Dark2HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2HighThreshold.ToString();
            txt_Dark2MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField2MinArea.ToString();
            lbl_Dark2Defect.Text = lbl_Dark2Defect.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(4) + 1).ToString());

            // Dark Field 2 (simple)
            lbl_Dark3LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3LowThreshold.ToString();
            lbl_Dark3HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3HighThreshold.ToString();
            txt_Dark3MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField3MinArea.ToString();
            lbl_Dark3Defect.Text = lbl_Dark3Defect.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(6) + 1).ToString());

            // Dark Field 2 (simple)
            lbl_Dark4LowThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4LowThreshold.ToString();
            lbl_Dark4HighThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4HighThreshold.ToString();
            txt_Dark4MinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intDarkField4MinArea.ToString();
            lbl_Dark4Defect.Text = lbl_Dark4Defect.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(7) + 1).ToString());

            // Mark View
            lbl_LowMarkViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewLowThreshold.ToString();
            lbl_HighMarkViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewHighThreshold.ToString();
            txt_MarkViewMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewMinArea.ToString();

            // Package View
            lbl_PackageViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intPkgViewThreshold.ToString();
            txt_PkgMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intPkgViewMinArea.ToString();
            lbl_Image2_BrightField.Text = lbl_Image2_BrightField.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            // Chip View Image 2
            lbl_ChipViewImage2Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView1Threshold.ToString();
            txt_ChipMinArea_1.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView1MinArea.ToString();
            lbl_Image2_BrightField_Chip.Text = lbl_Image2_BrightField_Chip.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            // Chip View Image 3
            lbl_ChipViewImage3Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView2Threshold.ToString();
            txt_ChipMinArea_2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intChipView2MinArea.ToString();
            lbl_Image3_DarkField_Chip.Text = lbl_Image3_DarkField_Chip.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            // Crack View
            lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewLowThreshold.ToString();
            lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewHighThreshold.ToString();
            txt_CrackMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intCrackViewMinArea.ToString();
            lbl_Image3_DarkField_Crack.Text = lbl_Image3_DarkField_Crack.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            // Void View Image 3
            lbl_VoidViewImage3Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewThreshold.ToString();
            txt_VoidMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intVoidViewMinArea.ToString();
            lbl_Image3_DarkField_Void.Text = lbl_Image3_DarkField_Void.Text.Replace("3", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3) + 1).ToString());

            // Mold Flash View Image 2
            lbl_MoldFlashViewImage2Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold.ToString();
            txt_MoldFlashMinArea.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashMinArea.ToString();
            lbl_Image2_BrightField_MF.Text = lbl_Image2_BrightField_MF.Text.Replace("2", (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2) + 1).ToString());

            //ROI Setting
            txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
            txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
            txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
            txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();

            txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark.ToString();
            txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark.ToString();
            txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark.ToString();
            txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark.ToString();

            txt_PkgStartPixelFromEdge_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2.ToString();
            txt_PkgStartPixelFromRight_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2.ToString();
            txt_PkgStartPixelFromBottom_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2.ToString();
            txt_PkgStartPixelFromLeft_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2.ToString();

            txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip.ToString();
            txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip.ToString();
            txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip.ToString();
            txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip.ToString();

            txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip.ToString();
            txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip.ToString();
            txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip.ToString();
            txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip.ToString();

            txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark.ToString();
            txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip_Dark.ToString();
            txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip_Dark.ToString();
            txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip_Dark.ToString();

            txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark.ToString();
            txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip_Dark.ToString();
            txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip_Dark.ToString();
            txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip_Dark.ToString();

            txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
            txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
            txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
            txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_SetToAll.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_ROI", false));
        }
        private void ViewImage(bool blnMarkView)
        {
            //m_smVisionInfo.g_blnViewPkgProcessImage = blnMarkView;
            //m_smVisionInfo.g_blnViewRotatedPackageImage = !blnMarkView;
            //m_smVisionInfo.g_blnViewRotatedImage = true;
            //m_smVisionInfo.g_blnViewDefect = !blnMarkView;
            //m_smVisionInfo.g_blnViewMarkInspection = false;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void ViewChip()
        {
            //m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            //m_smVisionInfo.g_blnViewPackageImage = false;
            //m_smVisionInfo.g_blnViewPkgProcessImage = false;
            //m_smVisionInfo.g_blnViewRotatedImage = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!IsChipInwardOutwardSettingCorrect(true))
            {
                return;
            }

            if (tab_VisionControl.Controls.Contains(tp_PkgSegment) || tab_VisionControl.Controls.Contains(tp_PkgSegment2) || tab_VisionControl.Controls.Contains(tp_PkgSegmentSimple))
            {
                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
                {
                    if (txt_MoldStartPixelFromEdge.ForeColor == Color.Red ||
                        txt_MoldStartPixelFromRight.ForeColor == Color.Red ||
                        txt_MoldStartPixelFromBottom.ForeColor == Color.Red ||
                        txt_MoldStartPixelFromLeft.ForeColor == Color.Red)
                    {
                        SRMMessageBox.Show("Mold Flash Outer tolerance cannot less than Inner tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    if (txt_MoldStartPixelFromEdgeInner.ForeColor == Color.Red ||
                        txt_MoldStartPixelFromRightInner.ForeColor == Color.Red ||
                        txt_MoldStartPixelFromBottomInner.ForeColor == Color.Red ||
                        txt_MoldStartPixelFromLeftInner.ForeColor == Color.Red)
                    {
                        SRMMessageBox.Show("Mold Flash Inner tolerance cannot more than Outer tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            SaveGeneralSetting(strFolderPath);

            SaveMarkSettings(strFolderPath + "Mark\\Template\\");
            LoadMarkSettings(strFolderPath + "Mark\\Template\\");   // 2019 07 18 - CCENG: Need to reload ocv especially the SetErodeDilateSettings function to make sure the erode and dilate template image is according to latest setting

            if (Directory.Exists(strFolderPath + "Mark\\Template_Temp\\"))
                Directory.Delete(strFolderPath + "Mark\\Template_Temp\\", true); //cxlim : Literation settings use only

            SavePackageSettings(strFolderPath + "Package\\");

            SaveLeadSettings(strFolderPath + "Lead\\");

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadGeneralSetting(strFolderPath + "General.xml");

            LoadMarkSettings(strFolderPath + "Mark\\Template\\");

            LoadPackageSetting(strFolderPath + "Package\\");

            LoadLeadSetting(strFolderPath + "Lead\\");

            if (Directory.Exists(strFolderPath + "Mark\\Template_Temp\\"))
                Directory.Delete(strFolderPath + "Mark\\Template_Temp\\", true); //cxlim : Literation settings use only

            this.Close();
            this.Dispose();
        }

        private void MarkOtherSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void MarkOtherSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            switch (m_intSelectedTabPage)
            {
                case 0:
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark Other Setting Form Closed", "Exit Mark Setting Form", "", "", m_smProductionInfo.g_strLotID);
                    break;
                case 1:
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package Other Setting Form Closed", "Exit Package Setting Form", "", "", m_smProductionInfo.g_strLotID);
                    break;
                case 2:
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead Other Setting Form Closed", "Exit Lead Setting Form", "", "", m_smProductionInfo.g_strLotID);
                    break;
            }
            
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.AT_VM_OfflineTestAllLead = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewROI = m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_intSelectedSetting = 0;
            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewPackageDefectSetting = false;
            m_smVisionInfo.g_blnViewPkgProcessImage = false;
            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_MarkConfig_Click(object sender, EventArgs e)
        {
            MarkConfiguration objMarkConfig = new MarkConfiguration(
                m_smVisionInfo.g_arrMarks[0].ref_intGroupIndex,
                m_smVisionInfo.g_arrMarks[0].GetNumTemplates(),
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateMask,
                m_smVisionInfo.g_arrMarks[0].ref_intTemplatePriority,
                m_smVisionInfo.g_strVisionFolderName,
                m_strSelectedRecipe, m_smProductionInfo.g_strRecipePath);

            if (objMarkConfig.ShowDialog() == DialogResult.OK)
            {
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    m_smVisionInfo.g_intTemplateMask = m_smVisionInfo.g_arrMarks[u].ref_intTemplateMask = objMarkConfig.ref_intTemplateMask;
                    m_smVisionInfo.g_intTemplatePriority = m_smVisionInfo.g_arrMarks[u].ref_intTemplatePriority = objMarkConfig.ref_intTemplatePriority;
                }

                int intCurrentSelectedIndex = m_smVisionInfo.g_intSelectedTemplate;

                if (intCurrentSelectedIndex == 0)
                {
                    if ((m_smVisionInfo.g_intTemplateMask & 0x01) == 0)
                    {
                        if ((m_smVisionInfo.g_intTemplateMask & 0x02) > 0)
                            intCurrentSelectedIndex = 1;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x04) > 0)
                            intCurrentSelectedIndex = 2;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            intCurrentSelectedIndex = 3;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x10) > 0)
                            intCurrentSelectedIndex = 4;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x20) > 0)
                            intCurrentSelectedIndex = 5;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x40) > 0)
                            intCurrentSelectedIndex = 6;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x80) > 0)
                            intCurrentSelectedIndex = 7;
                    }
                }
                else if (intCurrentSelectedIndex == 1)
                {
                    if ((m_smVisionInfo.g_intTemplateMask & 0x02) == 0)
                    {
                        if ((m_smVisionInfo.g_intTemplateMask & 0x01) > 0)
                            intCurrentSelectedIndex = 0;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x04) > 0)
                            intCurrentSelectedIndex = 2;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            intCurrentSelectedIndex = 3;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x10) > 0)
                            intCurrentSelectedIndex = 4;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x20) > 0)
                            intCurrentSelectedIndex = 5;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x40) > 0)
                            intCurrentSelectedIndex = 6;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x80) > 0)
                            intCurrentSelectedIndex = 7;
                    }
                }
                else if (intCurrentSelectedIndex == 2)
                {
                    if ((m_smVisionInfo.g_intTemplateMask & 0x04) == 0)
                    {
                        if ((m_smVisionInfo.g_intTemplateMask & 0x01) > 0)
                            intCurrentSelectedIndex = 0;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x02) > 0)
                            intCurrentSelectedIndex = 1;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            intCurrentSelectedIndex = 3;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x10) > 0)
                            intCurrentSelectedIndex = 4;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x20) > 0)
                            intCurrentSelectedIndex = 5;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x40) > 0)
                            intCurrentSelectedIndex = 6;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x80) > 0)
                            intCurrentSelectedIndex = 7;
                    }
                }
                else if (intCurrentSelectedIndex == 3)
                {
                    if ((m_smVisionInfo.g_intTemplateMask & 0x08) == 0)
                    {
                        if ((m_smVisionInfo.g_intTemplateMask & 0x01) > 0)
                            intCurrentSelectedIndex = 0;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x02) > 0)
                            intCurrentSelectedIndex = 1;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x04) > 0)
                            intCurrentSelectedIndex = 2;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x10) > 0)
                            intCurrentSelectedIndex = 4;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x20) > 0)
                            intCurrentSelectedIndex = 5;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x40) > 0)
                            intCurrentSelectedIndex = 6;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x80) > 0)
                            intCurrentSelectedIndex = 7;
                    }
                }
                else if (intCurrentSelectedIndex == 4)
                {
                    if ((m_smVisionInfo.g_intTemplateMask & 0x10) == 0)
                    {
                        if ((m_smVisionInfo.g_intTemplateMask & 0x01) > 0)
                            intCurrentSelectedIndex = 0;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x02) > 0)
                            intCurrentSelectedIndex = 1;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x04) > 0)
                            intCurrentSelectedIndex = 2;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            intCurrentSelectedIndex = 3;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x20) > 0)
                            intCurrentSelectedIndex = 5;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x40) > 0)
                            intCurrentSelectedIndex = 6;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x80) > 0)
                            intCurrentSelectedIndex = 7;
                    }
                }
                else if (intCurrentSelectedIndex == 5)
                {
                    if ((m_smVisionInfo.g_intTemplateMask & 0x20) == 0)
                    {
                        if ((m_smVisionInfo.g_intTemplateMask & 0x01) > 0)
                            intCurrentSelectedIndex = 0;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x02) > 0)
                            intCurrentSelectedIndex = 1;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x04) > 0)
                            intCurrentSelectedIndex = 2;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            intCurrentSelectedIndex = 3;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x10) > 0)
                            intCurrentSelectedIndex = 4;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x40) > 0)
                            intCurrentSelectedIndex = 6;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x80) > 0)
                            intCurrentSelectedIndex = 7;
                    }
                }
                else if (intCurrentSelectedIndex == 6)
                {
                    if ((m_smVisionInfo.g_intTemplateMask & 0x40) == 0)
                    {
                        if ((m_smVisionInfo.g_intTemplateMask & 0x01) > 0)
                            intCurrentSelectedIndex = 0;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x02) > 0)
                            intCurrentSelectedIndex = 1;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x04) > 0)
                            intCurrentSelectedIndex = 2;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            intCurrentSelectedIndex = 3;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x10) > 0)
                            intCurrentSelectedIndex = 4;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x20) > 0)
                            intCurrentSelectedIndex = 5;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x80) > 0)
                            intCurrentSelectedIndex = 7;
                    }
                }
                else if (intCurrentSelectedIndex == 7)
                {
                    if ((m_smVisionInfo.g_intTemplateMask & 0x80) == 0)
                    {
                        if ((m_smVisionInfo.g_intTemplateMask & 0x01) > 0)
                            intCurrentSelectedIndex = 0;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x02) > 0)
                            intCurrentSelectedIndex = 1;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x04) > 0)
                            intCurrentSelectedIndex = 2;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x08) > 0)
                            intCurrentSelectedIndex = 3;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x10) > 0)
                            intCurrentSelectedIndex = 4;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x20) > 0)
                            intCurrentSelectedIndex = 5;
                        else if ((m_smVisionInfo.g_intTemplateMask & 0x40) > 0)
                            intCurrentSelectedIndex = 6;
                    }
                }

                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = intCurrentSelectedIndex;
            }

            objMarkConfig.Dispose();
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            int intSelectedTemplateNum = 0;

            switch (((Button)sender).Name)
            {
                case "btn_Threshold_Template1":
                    intSelectedTemplateNum = 0;
                    break;
                case "btn_Threshold_Template2":
                    intSelectedTemplateNum = 1;
                    break;
                case "btn_Threshold_Template3":
                    intSelectedTemplateNum = 2;
                    break;
                case "btn_Threshold_Template4":
                    intSelectedTemplateNum = 3;
                    break;
                case "btn_Threshold_Template5":
                    intSelectedTemplateNum = 4;
                    break;
                case "btn_Threshold_Template6":
                    intSelectedTemplateNum = 5;
                    break;
                case "btn_Threshold_Template7":
                    intSelectedTemplateNum = 6;
                    break;
                case "btn_Threshold_Template8":
                    intSelectedTemplateNum = 7;
                    break;
            }

            bool blnSetToAllTemplate = false;
            bool blnMarkInspection = m_smVisionInfo.g_blnViewMarkInspection;
            m_smVisionInfo.g_blnViewMarkInspection = false;
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, intSelectedTemplateNum); //m_smVisionInfo.g_intSelectedTemplate
             ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 1, m_smVisionInfo, (ROI)m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                blnSetToAllTemplate = objThresholdForm.ref_blnSetToAllTemplate;
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    if (blnSetToAllTemplate)
                    {
                        for (int intTemplateIndex = 0; intTemplateIndex < m_smVisionInfo.g_arrMarks[u].GetNumTemplates(); intTemplateIndex++)
                            m_smVisionInfo.g_arrMarks[u].SetThreshold(m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_intSelectedGroup, intTemplateIndex);
                    }
                    else
                        m_smVisionInfo.g_arrMarks[u].SetThreshold(m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_intSelectedGroup, intSelectedTemplateNum); // m_smVisionInfo.g_intSelectedTemplate
                }
            }
            //lbl_MarkThreshold_Template1.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
            if (blnSetToAllTemplate)
            {
                lbl_MarkThreshold_Template1.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_MarkThreshold_Template2.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_MarkThreshold_Template3.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_MarkThreshold_Template4.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_MarkThreshold_Template5.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_MarkThreshold_Template6.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_MarkThreshold_Template7.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_MarkThreshold_Template8.Text = m_smVisionInfo.g_intThresholdValue.ToString();
            }
            else
            {
                switch (((Button)sender).Name)
                {
                    case "btn_Threshold_Template1":
                        lbl_MarkThreshold_Template1.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 0).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_Threshold_Template2":
                        lbl_MarkThreshold_Template2.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 1).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_Threshold_Template3":
                        lbl_MarkThreshold_Template3.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 2).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_Threshold_Template4":
                        lbl_MarkThreshold_Template4.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 3).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_Threshold_Template5":
                        lbl_MarkThreshold_Template5.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 4).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_Threshold_Template6":
                        lbl_MarkThreshold_Template6.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 5).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_Threshold_Template7":
                        lbl_MarkThreshold_Template7.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 6).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_Threshold_Template8":
                        lbl_MarkThreshold_Template8.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 7).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                }
            }
            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewMarkInspection = blnMarkInspection;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_ExtraMarkThreshold_Click(object sender, EventArgs e)
        {
            int intSelectedTemplateNum = 0;

            switch (((Button)sender).Name)
            {
                case "btn_ExtraMarkThreshold_Template1":
                    intSelectedTemplateNum = 0;
                    break;
                case "btn_ExtraMarkThreshold_Template2":
                    intSelectedTemplateNum = 1;
                    break;
                case "btn_ExtraMarkThreshold_Template3":
                    intSelectedTemplateNum = 2;
                    break;
                case "btn_ExtraMarkThreshold_Template4":
                    intSelectedTemplateNum = 3;
                    break;
                case "btn_ExtraMarkThreshold_Template5":
                    intSelectedTemplateNum = 4;
                    break;
                case "btn_ExtraMarkThreshold_Template6":
                    intSelectedTemplateNum = 5;
                    break;
                case "btn_ExtraMarkThreshold_Template7":
                    intSelectedTemplateNum = 6;
                    break;
                case "btn_ExtraMarkThreshold_Template8":
                    intSelectedTemplateNum = 7;
                    break;
            }

            bool blnSetToAllTemplate = false;
            bool blnMarkInspection = m_smVisionInfo.g_blnViewMarkInspection;
            m_smVisionInfo.g_blnViewMarkInspection = false;
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, intSelectedTemplateNum); //m_smVisionInfo.g_intSelectedTemplate
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 1, m_smVisionInfo, (ROI)m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                blnSetToAllTemplate = objThresholdForm.ref_blnSetToAllTemplate;
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    if (blnSetToAllTemplate)
                    {
                        for (int intTemplateIndex = 0; intTemplateIndex < m_smVisionInfo.g_arrMarks[u].GetNumTemplates(); intTemplateIndex++)
                            m_smVisionInfo.g_arrMarks[u].SetExtraMarkThreshold(m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_intSelectedGroup, intTemplateIndex);
                    }
                    else
                        m_smVisionInfo.g_arrMarks[u].SetExtraMarkThreshold(m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_intSelectedGroup, intSelectedTemplateNum); // m_smVisionInfo.g_intSelectedTemplate
                }
            }

            if (blnSetToAllTemplate)
            {
                lbl_ExtraMarkThreshold_Template1.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_ExtraMarkThreshold_Template2.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_ExtraMarkThreshold_Template3.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_ExtraMarkThreshold_Template4.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_ExtraMarkThreshold_Template5.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_ExtraMarkThreshold_Template6.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_ExtraMarkThreshold_Template7.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                lbl_ExtraMarkThreshold_Template8.Text = m_smVisionInfo.g_intThresholdValue.ToString();
            }
            else
            {
                switch (((Button)sender).Name)
                {
                    case "btn_ExtraMarkThreshold_Template1":
                        lbl_ExtraMarkThreshold_Template1.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 0).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_ExtraMarkThreshold_Template2":
                        lbl_ExtraMarkThreshold_Template2.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 1).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_ExtraMarkThreshold_Template3":
                        lbl_ExtraMarkThreshold_Template3.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 2).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_ExtraMarkThreshold_Template4":
                        lbl_ExtraMarkThreshold_Template4.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 3).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_ExtraMarkThreshold_Template5":
                        lbl_ExtraMarkThreshold_Template5.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 4).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_ExtraMarkThreshold_Template6":
                        lbl_ExtraMarkThreshold_Template6.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 5).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_ExtraMarkThreshold_Template7":
                        lbl_ExtraMarkThreshold_Template7.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 6).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                    case "btn_ExtraMarkThreshold_Template8":
                        lbl_ExtraMarkThreshold_Template8.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 7).ToString();// m_smVisionInfo.g_intSelectedTemplate
                        break;
                }
            }
            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewMarkInspection = blnMarkInspection;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void tab_VisionControl_Selected(object sender, TabControlEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.TabPage == tp_MarkSegment)
            {
                m_eSettingType = selectedTabpageGroup.UnderMark;
                UpdateTabPage();
            }
            else if(e.TabPage == tp_MarkSegment2)
            {
                m_eSettingType = selectedTabpageGroup.UnderMark2;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Other)
            {
                m_eSettingType = selectedTabpageGroup.UnderOther;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_LeadSegment)
            {
                m_eSettingType = selectedTabpageGroup.UnderLead;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_PkgSegment || e.TabPage == tp_PkgSegment2 || e.TabPage == tp_PkgSegmentSimple)
            {
                m_eSettingType = selectedTabpageGroup.UnderPackage;
                UpdateTabPage();
            }

            chk_SetToAll.Location = new Point(112, 404);
            cbo_SelectROI.Location = new Point(6, 416);

            if (e.TabPage == tp_ROI)
            {
                if (!tp_ROI.Controls.Contains(chk_SetToAll))
                    tp_ROI.Controls.Add(chk_SetToAll);
                if (!tp_ROI.Controls.Contains(cbo_SelectROI))
                    tp_ROI.Controls.Add(cbo_SelectROI); 
            }
            else if (e.TabPage == tp_ROI2)
            {
                if (!tp_ROI2.Controls.Contains(chk_SetToAll))
                    tp_ROI2.Controls.Add(chk_SetToAll);
                if (!tp_ROI2.Controls.Contains(cbo_SelectROI))
                    tp_ROI2.Controls.Add(cbo_SelectROI);
            }
            else if (e.TabPage == tp_ROI3)
            {
                if (!tp_ROI3.Controls.Contains(chk_SetToAll))
                    tp_ROI3.Controls.Add(chk_SetToAll);
                if (!tp_ROI3.Controls.Contains(cbo_SelectROI))
                    tp_ROI3.Controls.Add(cbo_SelectROI);


                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
                {
                    chk_SetToAll.Location = new Point(112, pnl_DarkField4ROI.Location.Y + pnl_DarkField4ROI.Size.Height);
                    cbo_SelectROI.Location = new Point(6, pnl_DarkField4ROI.Location.Y + pnl_DarkField4ROI.Size.Height + 12);
                }
                else if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
                {
                    chk_SetToAll.Location = new Point(112, pnl_DarkField3ROI.Location.Y + pnl_DarkField3ROI.Size.Height);
                    cbo_SelectROI.Location = new Point(6, pnl_DarkField3ROI.Location.Y + pnl_DarkField3ROI.Size.Height + 12);
                }
                else
                {
                    chk_SetToAll.Location = new Point(112, 404);
                    cbo_SelectROI.Location = new Point(6, 416);
                }
            }
        }

        private void chk_SetAllTemplates_Click(object sender, EventArgs e)
        {
            m_blnWantSet1ToAll = chk_SetAllTemplates.Checked;
        }

        private void cbo_TemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = cbo_TemplateNo.SelectedIndex;
            }

            lbl_MarkThreshold_Template1.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 0).ToString(); //m_smVisionInfo.g_intSelectedTemplate

            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 1)
                lbl_MarkThreshold_Template2.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 1).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 2)
                lbl_MarkThreshold_Template3.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 2).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 3)
                lbl_MarkThreshold_Template4.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 3).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 4)
                lbl_MarkThreshold_Template5.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 4).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 5)
                lbl_MarkThreshold_Template6.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 5).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 6)
                lbl_MarkThreshold_Template7.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 6).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 7)
                lbl_MarkThreshold_Template8.Text = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, 7).ToString();

            lbl_ExtraMarkThreshold_Template1.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 0).ToString(); //m_smVisionInfo.g_intSelectedTemplate

            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 1)
                lbl_ExtraMarkThreshold_Template2.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 1).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 2)
                lbl_ExtraMarkThreshold_Template3.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 2).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 3)
                lbl_ExtraMarkThreshold_Template4.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 3).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 4)
                lbl_ExtraMarkThreshold_Template5.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 4).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 5)
                lbl_ExtraMarkThreshold_Template6.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 5).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 6)
                lbl_ExtraMarkThreshold_Template7.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 6).ToString();
            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() > 7)
                lbl_ExtraMarkThreshold_Template8.Text = m_smVisionInfo.g_arrMarks[0].GetExtraMarkThreshold(m_smVisionInfo.g_intSelectedGroup, 7).ToString();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intMinArea = Convert.ToInt32(txt_MinArea.Text);
            }
        }

        private void btn_LeadThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intImageViewNo;

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue;

            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead.Length > 0 && m_smVisionInfo.g_arrLead[0].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[0].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 1 && m_smVisionInfo.g_arrLead[1].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[1].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 2 && m_smVisionInfo.g_arrLead[2].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[2].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 3 && m_smVisionInfo.g_arrLead[3].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[3].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);
            if (m_smVisionInfo.g_arrLead.Length > 4 && m_smVisionInfo.g_arrLead[4].ref_blnSelected)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead[4].ref_intThresholdValue);
            else
                arrrThreshold.Add(-999);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[m_smVisionInfo.g_intSelectedROI][0], true, true, arrrThreshold);
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
                        lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead[i].ref_intThresholdValue.ToString();
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
                    lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue.ToString();
                }
            }

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;

                if (m_intSelectedTabPage == 2)
                    UpdateLeadSelectedROISetting(m_smVisionInfo.g_intSelectedROI);
                else if (m_intSelectedTabPage == 1)
                {
                    m_blnInitDone = false;
                    UpdatePackageSelectedUnitSetting();
                    m_blnInitDone = true;
                }
            }

            if (m_blnTriggerOfflineTest)
            {
                m_blnTriggerOfflineTest = false;
                m_smVisionInfo.AT_VM_OfflineTestAllLead = true;
                TriggerOfflineTest();
            }

            if (m_smVisionInfo.g_blnDrawMarkResult)
            {
                m_smVisionInfo.g_blnDrawMarkResult = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

            if (m_objLeadLineProfileForm != null)
            {
                if (!m_objLeadLineProfileForm.ref_blnShow)
                {
                    m_smVisionInfo.g_strSelectedPage = "MarkOtherSettingForm";
                    m_objLeadLineProfileForm.Close();
                    m_objLeadLineProfileForm.Dispose();
                    m_objLeadLineProfileForm = null;
                    this.Show();

                    m_smVisionInfo.g_blnDragROI = m_blnDragROIPrev;
                }
            }
        }
        private void TriggerOfflineTest()
        {
            m_smVisionInfo.g_blnViewPointGauge = false;

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }
        private void txt_LeadMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // 2020 10 12 - Change set to all ROI because user always never realise it is set according to selected ROI.
            //m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intFilterMinArea = Convert.ToInt32(txt_LeadMinArea.Text);
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead[i] != null)
                    m_smVisionInfo.g_arrLead[i].ref_intFilterMinArea = Convert.ToInt32(txt_LeadMinArea.Text);
            }
        }

        private void btn_MarkViewThreshold_Click(object sender, EventArgs e)
        {
            bool blnMarkInspection = m_smVisionInfo.g_blnViewMarkInspection;

            if (m_smVisionInfo.g_blnMarkInspected)
            {
                RectGaugeM4L objGauge = m_objGauge;
                int intDisX = (int)(objGauge.ref_pRectCenterPoint.X - m_objGauge.ref_TemplateObjectCenterX);
                int intDisY = (int)(objGauge.ref_pRectCenterPoint.Y - m_objGauge.ref_TemplateObjectCenterY);

                ROI objPackageSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
                ROI objPackageTrainROI = new ROI();
                objPackageTrainROI.LoadROISetting(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalX, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalY,
                    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);
                objPackageSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[1]);

                objPackageTrainROI.ref_ROIPositionX += intDisX;
                objPackageTrainROI.ref_ROIPositionY += intDisY;

                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);
            }
            else
            {
                m_smVisionInfo.g_arrImages[0].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);
            }

            ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
            ROI objTrainROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];
            objSearchROI.AttachImage(m_smVisionInfo.g_objPkgProcessImage);
            objTrainROI.AttachImage(objSearchROI);

            ViewImage(true);
            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewLowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMarkViewHighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, objSearchROI);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intMarkViewLowThreshold = intLowThreshold;
                objPackage.ref_intMarkViewHighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intMarkViewLowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intMarkViewHighThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }

            lbl_LowMarkViewThreshold.Text = objPackage.ref_intMarkViewLowThreshold.ToString();
            lbl_HighMarkViewThreshold.Text = objPackage.ref_intMarkViewHighThreshold.ToString();

            objThresholdForm.Dispose();

            m_smVisionInfo.g_blnViewMarkInspection = blnMarkInspection;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_PackageThreshold_Click(object sender, EventArgs e)
        {
            ViewImage(false);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2); // No 2 is for package image index

            if (!m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
            }

            ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
            ROI objTrainROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];
            objSearchROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objTrainROI.AttachImage(objSearchROI);

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intThreshold = m_smVisionInfo.g_intThresholdValue = objPackage.ref_intPkgViewThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objSearchROI, false);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                objPackage.ref_intPkgViewThreshold = intThreshold;
            else
                objPackage.ref_intPkgViewThreshold = m_smVisionInfo.g_intThresholdValue;

            lbl_PackageViewThreshold.Text = objPackage.ref_intPkgViewThreshold.ToString();

            objThresholdForm.Dispose();

            //m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage; // Display back the production image

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ChipThreshold_1_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageChip = true;
            bool blnMarkInspection = m_smVisionInfo.g_blnViewMarkInspection;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2); // No 2 is for package image index
            if (m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
            }
            else
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, 1);
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            }

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].CreateSideROI(m_objGauge.ref_pRectCenterPoint.X,
                m_objGauge.ref_pRectCenterPoint.Y, m_objGauge.ref_fRectWidth,
                m_objGauge.ref_fRectHeight, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

            ViewChip();

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];

            ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
            ROI objTrainROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];
            objSearchROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objTrainROI.AttachImage(objSearchROI);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objSearchROI, objPackage.ref_intChipView1Threshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
                objPackage.ref_intChipView1Threshold = m_smVisionInfo.g_intThresholdValue;

            lbl_ChipViewImage2Threshold.Text = objPackage.ref_intChipView1Threshold.ToString();

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewPackageChip = false;
            m_smVisionInfo.g_blnViewMarkInspection = blnMarkInspection;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ChipThreshold_2_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageChip = true;
            bool blnMarkInspection = m_smVisionInfo.g_blnViewMarkInspection;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);
            if (m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
            }
            else
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[2], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, 2);
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[2]);
            }

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].CreateSideROI(m_objGauge.ref_pRectCenterPoint.X,
                m_objGauge.ref_pRectCenterPoint.Y, m_objGauge.ref_fRectWidth,
                m_objGauge.ref_fRectHeight, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

            ViewChip();

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];

            ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
            ROI objTrainROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];

            objSearchROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objTrainROI.AttachImage(objSearchROI);
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objSearchROI, objPackage.ref_intChipView2Threshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
                objPackage.ref_intChipView2Threshold = m_smVisionInfo.g_intThresholdValue;

            lbl_ChipViewImage3Threshold.Text = objPackage.ref_intChipView2Threshold.ToString();

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewPackageChip = false;
            m_smVisionInfo.g_blnViewMarkInspection = blnMarkInspection;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_CrackThreshold_Click(object sender, EventArgs e)
        {
            ViewImage(false);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3); // No 3 is for crack image index

            if (!m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
            }

            if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            //ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
            //ROI objTrainROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];
            //objSearchROI.AttachImage(m_smVisionInfo.g_objPkgProcessImage);
            //objTrainROI.AttachImage(objSearchROI);

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
            }

            lbl_LowCrackViewThreshold.Text = objPackage.ref_intCrackViewLowThreshold.ToString();
            lbl_HighCrackViewThreshold.Text = objPackage.ref_intCrackViewHighThreshold.ToString();

            objThresholdForm.Dispose();

            // m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;  // Display back the production image

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MarkViewMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intMarkViewMinArea = Convert.ToInt32(txt_MarkViewMinArea.Text);
            }
        }

        private void txt_PkgMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intPkgViewMinArea = Convert.ToInt32(txt_PkgMinArea.Text);
            }
        }

        private void txt_ChipMinArea_1_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intChipView1MinArea = Convert.ToInt32(txt_ChipMinArea_1.Text);
            }
        }

        private void txt_ChipMinArea_2_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intChipView2MinArea = Convert.ToInt32(txt_ChipMinArea_2.Text);
            }
        }

        private void txt_CrackMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intCrackViewMinArea = Convert.ToInt32(txt_CrackMinArea.Text);
            }
        }

        private void txt_MarkViewMinArea_Enter(object sender, EventArgs e)
        {
            ViewImage(true);
        }

        private void txt_PkgMinArea_Enter(object sender, EventArgs e)
        {
            ViewImage(false);
        }

        private void txt_ChipMinArea_1_Enter(object sender, EventArgs e)
        {
            ViewChip();
        }

        private void txt_ChipMinArea_2_Enter(object sender, EventArgs e)
        {
            ViewChip();
        }

        private void txt_CrackMinArea_Enter(object sender, EventArgs e)
        {
            ViewImage(true);
        }

        private void btn_LineProfileGaugeSetting_Click(object sender, EventArgs e)
        {
            m_blnDragROIPrev = m_smVisionInfo.g_blnDragROI;

            m_smVisionInfo.g_blnDragROI = false;
            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead\\PointGauge.xml";

            if (m_objLeadLineProfileForm == null)
                m_objLeadLineProfileForm = new LeadLineProfileForm(m_smVisionInfo, m_smVisionInfo.g_arrLead[0].ref_objPointGauge, strPath, m_smProductionInfo, 1);
            
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            m_objLeadLineProfileForm.Location = new Point(resolution.Width - m_objLeadLineProfileForm.Size.Width, resolution.Height - m_objLeadLineProfileForm.Size.Height);

            m_objLeadLineProfileForm.Show();

            this.Hide();
        }

        private void btn_VoidThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageVoid = true;
            bool blnMarkInspection = m_smVisionInfo.g_blnViewMarkInspection;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3);
            if (m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
            }
            else
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[2], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, 2);
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[2]);
            }

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].CreateSideROI(m_objGauge.ref_pRectCenterPoint.X,
                m_objGauge.ref_pRectCenterPoint.Y, m_objGauge.ref_fRectWidth,
                m_objGauge.ref_fRectHeight, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

            //ViewChip();

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];

            ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
            ROI objTrainROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];

            objSearchROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objTrainROI.AttachImage(objSearchROI);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objSearchROI, objPackage.ref_intVoidViewThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
                objPackage.ref_intVoidViewThreshold = m_smVisionInfo.g_intThresholdValue;

            lbl_VoidViewImage3Threshold.Text = objPackage.ref_intVoidViewThreshold.ToString();

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewPackageVoid = false;
            m_smVisionInfo.g_blnViewMarkInspection = blnMarkInspection;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_VoidMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intVoidViewMinArea = Convert.ToInt32(txt_VoidMinArea.Text);
            }
        }

        private void AddTrainROI(int intGap, int intGapRight, int intGapBottom, int intGapLeft)
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                int intUnitEdgeImageIndex = m_smVisionInfo.g_arrPackage[i].GetGrabImageIndex(0);    // 2019 06 14 - CCENG: Use GetGrabImageIndex index 0 because looking for unit size here.

                RectGaugeM4L objGauge;
                //switch (intUnitEdgeImageIndex)
                //{
                //    case 0:
                //        objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0];
                //        break;
                //    default:
                //    case 1:
                        objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[i];
                //        break;
                //    case 2:
                //        objGauge = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit];
                //        break;
                //}

                // 2019 06 14 - CCENG: Use g_arrPackageGaugeM4L here because g_arrPackageGaugeM4L is always for unit size 
                ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[i][0];
                m_smVisionInfo.g_arrImages[intUnitEdgeImageIndex].CopyTo(m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrImages[intUnitEdgeImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(objGauge.ref_fGainValue / 1000));
                objSearchROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                objGauge.SetGaugePlace_BasedOnEdgeROI();
                objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, objSearchROI, m_smVisionInfo.g_objWhiteImage);
                int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectWidth; //ref_TemplateObjectWidth
                int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectHeight; //ref_TemplateObjectHeight
                float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.X; //ref_TemplateObjectCenterX
                float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.Y; //ref_TemplateObjectCenterY
                int intPositionX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero) - objSearchROI.ref_ROIPositionX + intGapLeft;
                int intPositionY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero) - objSearchROI.ref_ROIPositionY + intGap;

                ROI objROI = new ROI("Package ROI " + i, 2);
                objROI.LoadROISetting(intPositionX, intPositionY,
                    UnitWidth - intGapLeft - intGapRight,
                    UnitHeight - intGap - intGapBottom);
                objROI.AttachImage(objSearchROI);

                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 2)
                    m_smVisionInfo.g_arrPackageROIs[i].Add(objROI);
                else
                    m_smVisionInfo.g_arrPackageROIs[i][2] = objROI;
            }
        }

        private void txt_PkgStartPixelFromEdge_TextChanged(object sender, EventArgs e)
        {

            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromEdge.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromEdge.Text == "" || fStartPixelFromEdge < 0)
                return;

            //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            //{
            //    if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
            //        return;
            //    }
            //}

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
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


            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromRight.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromRight.Text == "" || fStartPixelFromEdge < 0)
                return;

            //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            //{
            //    if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
            //        return;
            //    }
            //}
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
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


            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromBottom.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromBottom.Text == "" || fStartPixelFromEdge < 0)
                return;

            //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            //{
            //    if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
            //        return;
            //    }
            //}
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
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


            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromLeft.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromLeft.Text == "" || fStartPixelFromEdge < 0)
                return;

            //if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            //{
            //    if (fStartPixelFromEdge >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();
            //        return;
            //    }
            //}
            //else
            //{
            //    if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
            //    {
            //        SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();
            //        return;
            //    }
            //}
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
            }


            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
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
            }
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


            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_PkgStartPixelFromEdge_DarkField2_TextChanged(object sender, EventArgs e)
        {

            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromEdge_DarkField2.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromEdge_DarkField2.Text == "" || fStartPixelFromEdge < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromRight_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField2.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            
            CheckPkgDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_DarkField3_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromEdge_DarkField3.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromEdge_DarkField3.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromRight_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField3.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_DarkField4_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromEdge_DarkField4.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromEdge_DarkField4.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromRight_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField4.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_DarkField2_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromRight_DarkField2.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromRight_DarkField2.Text == "" || fStartPixelFromEdge < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField2.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            
            CheckPkgDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_DarkField3_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromRight_DarkField3.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromRight_DarkField3.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField3.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_DarkField4_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromRight_DarkField4.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromRight_DarkField4.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField4.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_DarkField2_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromBottom_DarkField2.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromBottom_DarkField2.Text == "" || fStartPixelFromEdge < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField2.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromRight_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField2.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckPkgDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_DarkField3_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromBottom_DarkField3.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromBottom_DarkField3.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField3.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromRight_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField3.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_DarkField4_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromBottom_DarkField4.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromBottom_DarkField4.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_DarkField4.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromRight_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromLeft_DarkField4.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromLeft_DarkField2_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromLeft_DarkField2.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromLeft_DarkField2.Text == "" || fStartPixelFromEdge < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField2) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_DarkField2.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField2.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromRight_DarkField2.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField2.Text = fStartPixelFromEdge.ToString();
            }


            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField2 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
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

        private void txt_PkgStartPixelFromLeft_DarkField3_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromLeft_DarkField3.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromLeft_DarkField3.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField3) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_DarkField3.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField3.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromRight_DarkField3.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField3.Text = fStartPixelFromEdge.ToString();
            }


            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField3 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromLeft_DarkField4_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPixelFromLeft_DarkField4.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromLeft_DarkField4.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_DarkField4) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_DarkField4.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_DarkField4.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromRight_DarkField4.Text = fStartPixelFromEdge.ToString();
                txt_PkgStartPixelFromBottom_DarkField4.Text = fStartPixelFromEdge.ToString();
            }


            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_DarkField4 = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PackageStartPixel_DarkField2_Enter(object sender, EventArgs e)
        {
            MeasureGauge();

            if (sender == txt_PkgStartPixelFromBottom_DarkField2 || sender == txt_PkgStartPixelFromEdge_DarkField2 || sender == txt_PkgStartPixelFromLeft_DarkField2 || sender == txt_PkgStartPixelFromRight_DarkField2)
                m_smVisionInfo.g_blnViewDarkField2StartPixelFromEdge = true;
            else if (sender == txt_PkgStartPixelFromBottom_DarkField3 || sender == txt_PkgStartPixelFromEdge_DarkField3 || sender == txt_PkgStartPixelFromLeft_DarkField3 || sender == txt_PkgStartPixelFromRight_DarkField3)
                m_smVisionInfo.g_blnViewDarkField3StartPixelFromEdge = true;
            else if (sender == txt_PkgStartPixelFromBottom_DarkField4 || sender == txt_PkgStartPixelFromEdge_DarkField4 || sender == txt_PkgStartPixelFromLeft_DarkField4 || sender == txt_PkgStartPixelFromRight_DarkField4)
                m_smVisionInfo.g_blnViewDarkField4StartPixelFromEdge = true;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PackageStartPixel_DarkField2_Leave(object sender, EventArgs e)
        {
            if (sender == txt_PkgStartPixelFromBottom_DarkField2 || sender == txt_PkgStartPixelFromEdge_DarkField2 || sender == txt_PkgStartPixelFromLeft_DarkField2 || sender == txt_PkgStartPixelFromRight_DarkField2)
                m_smVisionInfo.g_blnViewDarkField2StartPixelFromEdge = false;
            else if (sender == txt_PkgStartPixelFromBottom_DarkField3 || sender == txt_PkgStartPixelFromEdge_DarkField3 || sender == txt_PkgStartPixelFromLeft_DarkField3 || sender == txt_PkgStartPixelFromRight_DarkField3)
                m_smVisionInfo.g_blnViewDarkField3StartPixelFromEdge = false;
            else if (sender == txt_PkgStartPixelFromBottom_DarkField4 || sender == txt_PkgStartPixelFromEdge_DarkField4 || sender == txt_PkgStartPixelFromLeft_DarkField4 || sender == txt_PkgStartPixelFromRight_DarkField4)
                m_smVisionInfo.g_blnViewDarkField4StartPixelFromEdge = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void MeasureGauge()
        {
            //if (m_smVisionInfo.g_blnWantGauge) // 2019-12-09 ZJYEOH : g_blnWantGauge will only true When Mark Orient want gauge
            {
                int intUnitEdgeImageIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);

                RectGaugeM4L objGauge;
                switch (intUnitEdgeImageIndex)
                {
                    //case 0:
                    //    objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0];
                    //    break;
                    default:
                        //case 1:
                        objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit]; // 2019-12-09 ZJYEOH : Package size gauge will always use m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit]
                        break;
                        //case 2:
                        //    objGauge = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit];
                        //    break;
                }
                ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
                m_smVisionInfo.g_arrImages[intUnitEdgeImageIndex].CopyTo(m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrImages[intUnitEdgeImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(objGauge.ref_fGainValue / 1000));
                objSearchROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                objGauge.SetGaugePlace_BasedOnEdgeROI();
                objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, objSearchROI, m_smVisionInfo.g_objWhiteImage);
            }
        }

        private void txt_PkgStartPixelFromEdge_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_PkgStartPixelFromLeft_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromLeft_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }


        private void txt_MoldStartPixelFromEdge_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromEdge.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromEdge.Text == "" || fStartPixelFromEdge < 0)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth; 
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (chk_SetToAll.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                } 
            }
            else
            {
                if (UnitStartY - fStartPixelFromEdge <= SearchROIStartY)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_MoldStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            
            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromRight.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromRight.Text == "" || fStartPixelFromEdge < 0)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (chk_SetToAll.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    return;
                }
               
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_MoldStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromBottom.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromBottom.Text == "" || fStartPixelFromEdge < 0)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    return;
                }
                
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                }
               
            }
            if (chk_SetToAll.Checked)
            {
                txt_MoldStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromLeft.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromLeft.Text == "" || fStartPixelFromEdge < 0)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold.ToString();
                    txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold.ToString();
                    txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold.ToString();
                    txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold)
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold)
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold)
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartX - fStartPixelFromEdge <= SearchROIStartX)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold.ToString();
                    return;
                }
                
                if (fStartPixelFromEdge < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold)
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_MoldStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromEdge_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5);
            m_smVisionInfo.g_blnViewMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromEdge_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewMoldStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromRight_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5);
            m_smVisionInfo.g_blnViewMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromRight_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewMoldStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_MoldStartPixelFromBottom_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5);
            m_smVisionInfo.g_blnViewMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromBottom_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewMoldStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_MoldStartPixelFromLeft_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5);
            m_smVisionInfo.g_blnViewMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromLeft_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewMoldStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromEdge_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromEdge.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromEdge.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            
            CheckChipROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromRight.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromRight.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromBottom.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromBottom.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromLeft.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromLeft.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromEdge.Text = fStartPixelFromEdge.ToString();
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromEdge_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromEdge_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromRight_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromRight_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ChipStartPixelFromBottom_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromBottom_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ChipStartPixelFromLeft_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromLeft_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAll_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_ROI", chk_SetToAll.Checked);

            CheckPkgROISetting();
            CheckPkgDarkROISetting();
            CheckChipROISetting();
            CheckChipDarkROISetting();
            CheckPkgMoldROISetting();
        }

        private void btn_MoldFlashThreshold_Click(object sender, EventArgs e)
        {
            bool blnMarkInspection = m_smVisionInfo.g_blnViewMarkInspection;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(5); // No 5 is for Mold Flash image index

            if (!m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
            }

            ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
            ROI objTrainROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];
            objSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            objTrainROI.AttachImage(objSearchROI);

            int intThreshold = m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objSearchROI);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold = intThreshold;
            else
            {
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;

                lbl_MoldFlashViewImage2Threshold.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intMoldFlashThreshold.ToString();
            }

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrPackage[i].ref_intMoldFlashMinArea = Convert.ToInt32(txt_MoldFlashMinArea.Text);
            }
        }

        private void btn_BrightThreshold_Click(object sender, EventArgs e)
        {
            ViewImage(false);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2); // No 2 is for Bright Field image index

            //if (!m_smVisionInfo.g_blnMarkInspected)
            //{
            //    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //    m_objGauge.Measure(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            //    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
            //}

            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intThreshold = m_smVisionInfo.g_intThresholdValue = objPackage.ref_intBrightFieldLowThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], false);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
                objPackage.ref_intBrightFieldLowThreshold = intThreshold;
            else
                objPackage.ref_intBrightFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;

            lbl_BrightThreshold.Text = objPackage.ref_intBrightFieldLowThreshold.ToString();

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DarkThreshold_Click(object sender, EventArgs e)
        {
            ViewImage(false);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3); // No 3 is for Dark Field image index

            if (!m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
            }

            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intDarkFieldLowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intDarkFieldHighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intDarkFieldLowThreshold = intLowThreshold;
                objPackage.ref_intDarkFieldHighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intDarkFieldLowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intDarkFieldHighThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }

            lbl_DarkLowThreshold.Text = objPackage.ref_intDarkFieldLowThreshold.ToString();
            lbl_DarkHighThreshold.Text = objPackage.ref_intDarkFieldHighThreshold.ToString();

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_Dark2Threshold_Click(object sender, EventArgs e)
        {
            ViewImage(false);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(4); // No 4 is for Dark Field 2 image index

            if (!m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
            }

            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intDarkField2LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intDarkField2HighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intDarkField2LowThreshold = intLowThreshold;
                objPackage.ref_intDarkField2HighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intDarkField2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intDarkField2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }

            lbl_Dark2LowThreshold.Text = objPackage.ref_intDarkField2LowThreshold.ToString();
            lbl_Dark2HighThreshold.Text = objPackage.ref_intDarkField2HighThreshold.ToString();

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Dark3Threshold_Click(object sender, EventArgs e)
        {
            ViewImage(false);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(6); // No 6 is for Dark Field 3 image index

            if (!m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
            }

            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intDarkField3LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intDarkField3HighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intDarkField3LowThreshold = intLowThreshold;
                objPackage.ref_intDarkField3HighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intDarkField3LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intDarkField3HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }

            lbl_Dark3LowThreshold.Text = objPackage.ref_intDarkField3LowThreshold.ToString();
            lbl_Dark3HighThreshold.Text = objPackage.ref_intDarkField3HighThreshold.ToString();

            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Dark4Threshold_Click(object sender, EventArgs e)
        {
            ViewImage(false);

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(7); // No 7 is for Dark Field 4 image index

            if (!m_smVisionInfo.g_blnMarkInspected)
            {
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                m_objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage);
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_objGauge.ref_fRectAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
            }

            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            Package objPackage = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit];
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = objPackage.ref_intDarkField4LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = objPackage.ref_intDarkField4HighThreshold;

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                objPackage.ref_intDarkField4LowThreshold = intLowThreshold;
                objPackage.ref_intDarkField4HighThreshold = intHighThreshold;
            }
            else
            {
                objPackage.ref_intDarkField4LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                objPackage.ref_intDarkField4HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
            }

            lbl_Dark4LowThreshold.Text = objPackage.ref_intDarkField4LowThreshold.ToString();
            lbl_Dark4HighThreshold.Text = objPackage.ref_intDarkField4HighThreshold.ToString();

            objThresholdForm.Dispose();

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

        private void gbox_Pkg_Enter(object sender, EventArgs e)
        {

        }

        private void txt_BaseOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
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

        }

        private void txt_TipOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
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

        }

        private void chk_SetToAllLeads_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllLead", chk_SetToAllLeads.Checked);
        }

        private void cbo_LeadNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                int BaseValue = m_smVisionInfo.g_arrLead[i].GetBaseInwardOffset(cbo_LeadNo.SelectedIndex);
                int TipValue = m_smVisionInfo.g_arrLead[i].GetTipInwardOffset(cbo_LeadNo.SelectedIndex);

                if (BaseValue > 0)
                    txt_BaseOffset.Text = BaseValue.ToString();

                if (TipValue > 0)
                    txt_TipOffset.Text = TipValue.ToString();
            }

        }

        private void txt_PkgToBaseTolerance_Top_TextChanged(object sender, EventArgs e)
        {
            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
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

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Right_TextChanged(object sender, EventArgs e)
        {
            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
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

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Bottom_TextChanged(object sender, EventArgs e)
        {
            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
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

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Left_TextChanged(object sender, EventArgs e)
        {
            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
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

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            //MeasureGauge();
                RectGaugeM4L objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit]; 
                ROI objSearchROI = m_smVisionInfo.g_arrLeadROIs[0][0];
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].CopyTo(m_smVisionInfo.g_objPackageImage);
                objSearchROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                objGauge.SetGaugePlace_BasedOnEdgeROI();
                objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, objSearchROI, m_smVisionInfo.g_objWhiteImage);
            
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

        private void txt_ChipStartPixelExtendFromEdge_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromEdge.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_ChipStartPixelExtendFromEdge.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_ChipStartPixelExtendFromEdge.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelExtendFromBottom.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromLeft.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromRight.Text = fStartPixelExtendFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromRight.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_ChipStartPixelExtendFromRight.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_ChipStartPixelExtendFromRight.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelExtendFromBottom.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromLeft.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromEdge.Text = fStartPixelExtendFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromBottom.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_ChipStartPixelExtendFromBottom.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_ChipStartPixelExtendFromBottom.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelExtendFromRight.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromLeft.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromEdge.Text = fStartPixelExtendFromEdge.ToString();
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromLeft.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_ChipStartPixelExtendFromLeft.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_ChipStartPixelExtendFromLeft.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelExtendFromRight.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromBottom.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromEdge.Text = fStartPixelExtendFromEdge.ToString();
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromEdge_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromEdge_Dark.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromEdge_Dark.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelFromBottom_Dark.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromLeft_Dark.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromRight_Dark.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }
            
            CheckChipDarkROISetting();
            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromRight_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromRight_Dark.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromRight_Dark.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip_Dark.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip_Dark.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelFromBottom_Dark.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromLeft_Dark.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromEdge_Dark.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromBottom_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromBottom_Dark.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromBottom_Dark.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip_Dark.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip_Dark.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelFromRight_Dark.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromLeft_Dark.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromEdge_Dark.Text = fStartPixelFromEdge.ToString();
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromLeft_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromLeft_Dark.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromLeft_Dark.Text == "" || fStartPixelFromEdge < 0)
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip_Dark.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip_Dark.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelFromRight_Dark.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromBottom_Dark.Text = fStartPixelFromEdge.ToString();
                txt_ChipStartPixelFromEdge_Dark.Text = fStartPixelFromEdge.ToString();
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Chip_Dark = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromEdge_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromEdge_Dark.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_ChipStartPixelExtendFromEdge_Dark.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_ChipStartPixelExtendFromEdge_Dark.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelExtendFromBottom_Dark.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromLeft_Dark.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromRight_Dark.Text = fStartPixelExtendFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromRight_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromRight_Dark.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_ChipStartPixelExtendFromRight_Dark.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_ChipStartPixelExtendFromRight_Dark.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip_Dark.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip_Dark.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelExtendFromBottom_Dark.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromLeft_Dark.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromEdge_Dark.Text = fStartPixelExtendFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromBottom_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromBottom_Dark.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_ChipStartPixelExtendFromBottom_Dark.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_ChipStartPixelExtendFromBottom_Dark.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip_Dark.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip_Dark.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelExtendFromRight_Dark.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromLeft_Dark.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromEdge_Dark.Text = fStartPixelExtendFromEdge.ToString();
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromLeft_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromLeft_Dark.Text, out fStartPixelExtendFromEdge))
                return;

            // 2021 07 27 - Outward allow to set positive and negative value
            //if (!m_blnInitDone || txt_ChipStartPixelExtendFromLeft_Dark.Text == "" || fStartPixelExtendFromEdge < 0)
            if (!m_blnInitDone || txt_ChipStartPixelExtendFromLeft_Dark.Text == "")
                return;

            IsChipInwardOutwardSettingCorrect(false);

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
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
                    txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip_Dark.ToString();
                    return;
                }
            }
            else
            {
                if (fStartPixelExtendFromEdge > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto half of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip_Dark.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_ChipStartPixelExtendFromRight_Dark.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromBottom_Dark.Text = fStartPixelExtendFromEdge.ToString();
                txt_ChipStartPixelExtendFromEdge_Dark.Text = fStartPixelExtendFromEdge.ToString();
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromLeft_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromRight_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromBottom_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelExtendFromEdge_Chip_Dark = (int)Math.Round(fStartPixelExtendFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckChipDarkROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromEdge_Dark_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(3), 0);
            if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                m_smVisionInfo.g_intSelectedImage = 0;
            else
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

            m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromEdge_Dark_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = Math.Max(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(2), 0);
            if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrRotatedImages.Count)
                m_smVisionInfo.g_intSelectedImage = 0;
            else
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPkgProcessImage);

            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPkgProcessImage);
            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);

            m_smVisionInfo.g_blnViewChipStartPixelFromEdge_Dark = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BaseLeadThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead[0].ref_intBaseLeadImageViewNo;

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue_BaseLead;

            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }

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

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[m_smVisionInfo.g_intSelectedROI][0], true, true, arrrThreshold);
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
                        lbl_BaseLeadThreshold.Text = m_smVisionInfo.g_arrLead[i].ref_intThresholdValue_BaseLead.ToString();
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue_BaseLead = m_smVisionInfo.g_intThresholdValue;
                    lbl_BaseLeadThreshold.Text = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue_BaseLead.ToString();
                }
            }

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BaseLeadMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // 2020 10 12 - Change set to all ROI because user always never realise it is set according to selected ROI.
            // m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intFilterMinArea_BaseLead = Convert.ToInt32(txt_BaseLeadMinArea.Text);
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead[i] != null)
                    m_smVisionInfo.g_arrLead[i].ref_intFilterMinArea_BaseLead = Convert.ToInt32(txt_BaseLeadMinArea.Text);
            }

        }

        private void txt_MarkROIOffsetTop_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
            //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //{
            //    m_smVisionInfo.g_arrMarks[u].ref_intMarkROIOffsetTop = Convert.ToInt32(txt_MarkROIOffsetTop.Text);
            //}

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MarkROIOffsetRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
            //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //{
            //    m_smVisionInfo.g_arrMarks[u].ref_intMarkROIOffsetRight = Convert.ToInt32(txt_MarkROIOffsetRight.Text);
            //}
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MarkROIOffsetBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
            //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //{
            //    m_smVisionInfo.g_arrMarks[u].ref_intMarkROIOffsetBottom = Convert.ToInt32(txt_MarkROIOffsetBottom.Text);
            //}
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MarkROIOffsetLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
            //for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            //{
            //    m_smVisionInfo.g_arrMarks[u].ref_intMarkROIOffsetLeft = Convert.ToInt32(txt_MarkROIOffsetLeft.Text);
            //}
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_MarkROIOffset_Enter(object sender, EventArgs e)
        {
            // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
            //m_smVisionInfo.g_blnViewRotatedImage = true;
            //m_smVisionInfo.g_intSelectedImage = 0;//m_smVisionInfo.g_arrLead[0].ref_intImageViewNo; //2021-08-03 ZJYEOH : should display mark image as this is setting for mark
            //m_smVisionInfo.g_blnViewMarkROIOffset = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MarkROIOffset_Leave(object sender, EventArgs e)
        {
            // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
            //m_smVisionInfo.g_blnViewRotatedImage = false;
            //m_smVisionInfo.g_intSelectedImage = m_intPreviousSelectedImage;
            //m_smVisionInfo.g_blnViewMarkROIOffset = false;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void CheckPkgROISetting()
        {
            //bool blnSame = true;

            //if (chk_SetToAll.Checked)
            //{
            //    if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight))
            //        blnSame = false;
            //}

            //if (blnSame)
            //{
            //    lbl_PkgTop.ForeColor = Color.Black;
            //    lbl_PkgRight.ForeColor = Color.Black;
            //    lbl_PkgBottom.ForeColor = Color.Black;
            //    lbl_PkgLeft.ForeColor = Color.Black;
            //}
            //else
            //{
            //    lbl_PkgTop.ForeColor = Color.Red;
            //    lbl_PkgRight.ForeColor = Color.Red;
            //    lbl_PkgBottom.ForeColor = Color.Red;
            //    lbl_PkgLeft.ForeColor = Color.Red;
            //}

        }
        private void CheckPkgDarkROISetting()
        {
            //bool blnSame = true;

            //if (chk_SetToAll.Checked)
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
        private void CheckChipROISetting()
        {
            //bool blnSame = true;
            //if (chk_SetToAll.Checked)
            //{
            //        if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip) ||
            //                    (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip))
            //            blnSame = false;
            //}

            //if (blnSame)
            //{
            //    lbl_ChipTop.ForeColor = Color.Black;
            //    lbl_ChipRight.ForeColor = Color.Black;
            //    lbl_ChipBottom.ForeColor = Color.Black;
            //    lbl_ChipLeft.ForeColor = Color.Black;
            //}
            //else
            //{
            //    lbl_ChipTop.ForeColor = Color.Red;
            //    lbl_ChipRight.ForeColor = Color.Red;
            //    lbl_ChipBottom.ForeColor = Color.Red;
            //    lbl_ChipLeft.ForeColor = Color.Red;
            //}

        }
        private void CheckChipDarkROISetting()
        {
            //bool blnSame = true;
            //if (chk_SetToAll.Checked)
            //{
            //    if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Chip_Dark) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Chip_Dark) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Chip_Dark) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromBottom_Chip_Dark) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromLeft_Chip_Dark) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromEdge_Chip_Dark != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelExtendFromRight_Chip_Dark))
            //        blnSame = false;
            //}

            //if (blnSame)
            //{
            //    lbl_ChipDarkTop.ForeColor = Color.Black;
            //    lbl_ChipDarkRight.ForeColor = Color.Black;
            //    lbl_ChipDarkBottom.ForeColor = Color.Black;
            //    lbl_ChipDarkLeft.ForeColor = Color.Black;
            //}
            //else
            //{
            //    lbl_ChipDarkTop.ForeColor = Color.Red;
            //    lbl_ChipDarkRight.ForeColor = Color.Red;
            //    lbl_ChipDarkBottom.ForeColor = Color.Red;
            //    lbl_ChipDarkLeft.ForeColor = Color.Red;
            //}

        }
        private void CheckPkgMoldROISetting()
        {
            //bool blnSame = true;

            //if (chk_SetToAll.Checked)
            //{
            //    if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold) ||
            //                (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold != m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold))
            //        blnSame = false;
            //}

            //if (blnSame)
            //{
            //    lbl_MoldTop.ForeColor = Color.Black;
            //    lbl_MoldRight.ForeColor = Color.Black;
            //    lbl_MoldBottom.ForeColor = Color.Black;
            //    lbl_MoldLeft.ForeColor = Color.Black;
            //}
            //else
            //{
            //    lbl_MoldTop.ForeColor = Color.Red;
            //    lbl_MoldRight.ForeColor = Color.Red;
            //    lbl_MoldBottom.ForeColor = Color.Red;
            //    lbl_MoldLeft.ForeColor = Color.Red;
            //}

        }

        private void txt_MoldStartPixelFromEdgeInner_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromEdgeInner.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromEdgeInner.Text == "" || fStartPixelFromEdge < 0)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth; 
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (chk_SetToAll.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartY - fStartPixelFromEdge <= SearchROIStartY)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                }
               
            }
            if (chk_SetToAll.Checked)
            {
                txt_MoldStartPixelFromLeftInner.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromRightInner.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromBottomInner.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdgeInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeftInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRightInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottomInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromRightInner_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromRightInner.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromRightInner.Text == "" || fStartPixelFromEdge < 0)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;


            if (chk_SetToAll.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    return;
                }
                
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                }
                
            }
            if (chk_SetToAll.Checked)
            {
                txt_MoldStartPixelFromLeftInner.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromEdgeInner.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromBottomInner.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRightInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeftInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdgeInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottomInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromBottomInner_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromBottomInner.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromBottomInner.Text == "" || fStartPixelFromEdge < 0)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    return;
                }

                
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                }
               
            }
            if (chk_SetToAll.Checked)
            {
                txt_MoldStartPixelFromLeftInner.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromEdgeInner.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromRightInner.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottomInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeftInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdgeInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRightInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromLeftInner_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromLeftInner.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromLeftInner.Text == "" || fStartPixelFromEdge < 0)
                return;

            int SearchROIStartX = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX;
            int SearchROIStartY = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY;
            int SearchROIWidth = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth;
            int SearchROIHeight = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight;
            //int UnitStartX = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterX - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitStartY = (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_ObjectCenterY - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
            //int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectWidth;
            //int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectHeight;
            //float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterX;
            //float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_TemplateObjectCenterY;
            int UnitWidth = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth;
            int UnitHeight = (int)m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight;
            float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X;
            float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y;

            int UnitStartX = (int)Math.Round(fCenterX - (UnitWidth / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX ;
            int UnitStartY = (int)Math.Round(fCenterY - (UnitHeight / 2), 0, MidpointRounding.AwayFromZero);// - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY ;

            if (chk_SetToAll.Checked)
            {
                if ((UnitStartY - fStartPixelFromEdge <= SearchROIStartY) ||
                     (UnitStartX + UnitWidth + fStartPixelFromEdge >= SearchROIStartX + SearchROIWidth) ||
                     (UnitStartY + UnitHeight + fStartPixelFromEdge >= SearchROIStartY + SearchROIHeight) ||
                     (UnitStartX - fStartPixelFromEdge <= SearchROIStartX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdgeInner_Mold.ToString();
                    txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRightInner_Mold.ToString();
                    txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottomInner_Mold.ToString();
                    txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }

                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Mold)
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Mold)
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Mold)
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                }
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                }
            }
            else
            {
                if (UnitStartX - fStartPixelFromEdge <= SearchROIStartX)
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeftInner_Mold.ToString();
                    return;
                }
                
                if (fStartPixelFromEdge > m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Mold)
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                }
                else
                {
                    txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_MoldStartPixelFromBottomInner.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromEdgeInner.Text = fStartPixelFromEdge.ToString();
                txt_MoldStartPixelFromRightInner.Text = fStartPixelFromEdge.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeftInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottomInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdgeInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRightInner_Mold = (int)Math.Round(fStartPixelFromEdge, 0, MidpointRounding.AwayFromZero);
                }
            }

            CheckPkgMoldROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_Dark_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge_Dark = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_Dark_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPackageStartPixelFromEdge_Dark = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_Dark_TextChanged(object sender, EventArgs e)
        {

            float fStartPixelFromEdge_Dark = 0;
            if (!float.TryParse(txt_PkgStartPixelFromEdge_Dark.Text, out fStartPixelFromEdge_Dark))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromEdge_Dark.Text == "" || fStartPixelFromEdge_Dark < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge_Dark + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge_Dark + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark.ToString();
                    return;
                }
            }

            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromRight_Dark.Text = fStartPixelFromEdge_Dark.ToString();
                txt_PkgStartPixelFromBottom_Dark.Text = fStartPixelFromEdge_Dark.ToString();
                txt_PkgStartPixelFromLeft_Dark.Text = fStartPixelFromEdge_Dark.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                }
            }
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


            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge_Dark = 0;
            if (!float.TryParse(txt_PkgStartPixelFromRight_Dark.Text, out fStartPixelFromEdge_Dark))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromRight_Dark.Text == "" || fStartPixelFromEdge_Dark < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge_Dark + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge_Dark + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_Dark.Text = fStartPixelFromEdge_Dark.ToString();
                txt_PkgStartPixelFromBottom_Dark.Text = fStartPixelFromEdge_Dark.ToString();
                txt_PkgStartPixelFromLeft_Dark.Text = fStartPixelFromEdge_Dark.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                }
            }
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


            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge_Dark = 0;
            if (!float.TryParse(txt_PkgStartPixelFromBottom_Dark.Text, out fStartPixelFromEdge_Dark))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromBottom_Dark.Text == "" || fStartPixelFromEdge_Dark < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge_Dark + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge_Dark + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_Dark.Text = fStartPixelFromEdge_Dark.ToString();
                txt_PkgStartPixelFromRight_Dark.Text = fStartPixelFromEdge_Dark.ToString();
                txt_PkgStartPixelFromLeft_Dark.Text = fStartPixelFromEdge_Dark.ToString();
            }
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                }
            }
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


            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromLeft_Dark_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge_Dark = 0;
            if (!float.TryParse(txt_PkgStartPixelFromLeft_Dark.Text, out fStartPixelFromEdge_Dark))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromLeft_Dark.Text == "" || fStartPixelFromEdge_Dark < 0)
                return;
            
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight)
            {
                if ((fStartPixelFromEdge_Dark + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth * m_smVisionInfo.g_fCalibPixelX))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark.ToString();
                    return;
                }
            }
            else
            {
                if ((fStartPixelFromEdge_Dark + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark) >= (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight * m_smVisionInfo.g_fCalibPixelY))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark.ToString();
                    return;
                }
            }
            if (chk_SetToAll.Checked)
            {
                txt_PkgStartPixelFromEdge_Dark.Text = fStartPixelFromEdge_Dark.ToString();
                txt_PkgStartPixelFromRight_Dark.Text = fStartPixelFromEdge_Dark.ToString();
                txt_PkgStartPixelFromBottom_Dark.Text = fStartPixelFromEdge_Dark.ToString();
            }


            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                //04-03-2019 ZJYeoh : remove conversion(* m_smVisionInfo.g_fCalibPixelX) to avoid rounding problem
                m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromLeft_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                if (chk_SetToAll.Checked)
                {

                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromEdge_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromRight_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrPackage[u].ref_intStartPixelFromBottom_Dark = (int)Math.Round(fStartPixelFromEdge_Dark, 0, MidpointRounding.AwayFromZero);
                }
                //m_smVisionInfo.g_arrPackage[u].SetDefectParam(1, Convert.ToSingle(txt_StartPixelFromEdge.Text) * m_smVisionInfo.g_fCalibPixelX);
            }
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


            CheckPkgROISetting();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_EnhanceMark_LinkMark_HalfWidth_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intEnhanceMark_LinkMark_HalfWidth = Convert.ToInt32(txt_EnhanceMark_LinkMark_HalfWidth.Text);
            }
        }

        private void txt_EnhanceMark_ReduceMark_HalfWidth_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intEnhanceMark_ReduceNoise_HalfWidth = Convert.ToInt32(txt_EnhanceMark_ReduceMark_HalfWidth.Text);
            }
        }
        private bool IsChipInwardOutwardSettingCorrect(bool blnShowMessage)
        {
            bool blnSettingOK = true;
            float fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromEdge.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromEdge.BackColor = txt_ChipStartPixelFromEdge.NormalBackColor = txt_ChipStartPixelFromEdge.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            float fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromEdge.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelExtendFromEdge.BackColor = txt_ChipStartPixelExtendFromEdge.NormalBackColor = txt_ChipStartPixelExtendFromEdge.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromEdge.BackColor = txt_ChipStartPixelFromEdge.NormalBackColor = txt_ChipStartPixelFromEdge.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromEdge.BackColor = txt_ChipStartPixelExtendFromEdge.NormalBackColor = txt_ChipStartPixelExtendFromEdge.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromEdge.BackColor = txt_ChipStartPixelFromEdge.NormalBackColor = txt_ChipStartPixelFromEdge.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromEdge.BackColor = txt_ChipStartPixelExtendFromEdge.NormalBackColor = txt_ChipStartPixelExtendFromEdge.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromRight.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromRight.BackColor = txt_ChipStartPixelFromRight.NormalBackColor = txt_ChipStartPixelFromRight.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromRight.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelExtendFromRight.BackColor = txt_ChipStartPixelExtendFromRight.NormalBackColor = txt_ChipStartPixelExtendFromRight.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromRight.BackColor = txt_ChipStartPixelFromRight.NormalBackColor = txt_ChipStartPixelFromRight.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromRight.BackColor = txt_ChipStartPixelExtendFromRight.NormalBackColor = txt_ChipStartPixelExtendFromRight.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }
            else
            {
                txt_ChipStartPixelFromRight.BackColor = txt_ChipStartPixelFromRight.NormalBackColor = txt_ChipStartPixelFromRight.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromRight.BackColor = txt_ChipStartPixelExtendFromRight.NormalBackColor = txt_ChipStartPixelExtendFromRight.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromBottom.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromBottom.BackColor = txt_ChipStartPixelFromBottom.NormalBackColor = txt_ChipStartPixelFromBottom.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromBottom.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelExtendFromBottom.BackColor = txt_ChipStartPixelExtendFromBottom.NormalBackColor = txt_ChipStartPixelExtendFromBottom.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromBottom.BackColor = txt_ChipStartPixelFromBottom.NormalBackColor = txt_ChipStartPixelFromBottom.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromBottom.BackColor = txt_ChipStartPixelExtendFromBottom.NormalBackColor = txt_ChipStartPixelExtendFromBottom.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }
            else
            {
                txt_ChipStartPixelFromBottom.BackColor = txt_ChipStartPixelFromBottom.NormalBackColor = txt_ChipStartPixelFromBottom.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromBottom.BackColor = txt_ChipStartPixelExtendFromBottom.NormalBackColor = txt_ChipStartPixelExtendFromBottom.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromLeft.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromLeft.BackColor = txt_ChipStartPixelFromLeft.NormalBackColor = txt_ChipStartPixelFromLeft.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromLeft.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelExtendFromLeft.BackColor = txt_ChipStartPixelExtendFromLeft.NormalBackColor = txt_ChipStartPixelExtendFromLeft.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromLeft.BackColor = txt_ChipStartPixelFromLeft.NormalBackColor = txt_ChipStartPixelFromLeft.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromLeft.BackColor = txt_ChipStartPixelExtendFromLeft.NormalBackColor = txt_ChipStartPixelExtendFromLeft.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromLeft.BackColor = txt_ChipStartPixelFromLeft.NormalBackColor = txt_ChipStartPixelFromLeft.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromLeft.BackColor = txt_ChipStartPixelExtendFromLeft.NormalBackColor = txt_ChipStartPixelExtendFromLeft.FocusBackColor = Color.White;

            }

            //-----------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromEdge_Dark.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromEdge_Dark.BackColor = txt_ChipStartPixelFromEdge_Dark.NormalBackColor = txt_ChipStartPixelFromEdge_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromEdge_Dark.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelExtendFromEdge_Dark.BackColor = txt_ChipStartPixelExtendFromEdge_Dark.NormalBackColor = txt_ChipStartPixelExtendFromEdge_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromEdge_Dark.BackColor = txt_ChipStartPixelFromEdge_Dark.NormalBackColor = txt_ChipStartPixelFromEdge_Dark.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromEdge_Dark.BackColor = txt_ChipStartPixelExtendFromEdge_Dark.NormalBackColor = txt_ChipStartPixelExtendFromEdge_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromEdge_Dark.BackColor = txt_ChipStartPixelFromEdge_Dark.NormalBackColor = txt_ChipStartPixelFromEdge_Dark.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromEdge_Dark.BackColor = txt_ChipStartPixelExtendFromEdge_Dark.NormalBackColor = txt_ChipStartPixelExtendFromEdge_Dark.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromRight_Dark.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromRight_Dark.BackColor = txt_ChipStartPixelFromRight_Dark.NormalBackColor = txt_ChipStartPixelFromRight_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromRight_Dark.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelExtendFromRight_Dark.BackColor = txt_ChipStartPixelExtendFromRight_Dark.NormalBackColor = txt_ChipStartPixelExtendFromRight_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromRight_Dark.BackColor = txt_ChipStartPixelFromRight_Dark.NormalBackColor = txt_ChipStartPixelFromRight_Dark.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromRight_Dark.BackColor = txt_ChipStartPixelExtendFromRight_Dark.NormalBackColor = txt_ChipStartPixelExtendFromRight_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromRight_Dark.BackColor = txt_ChipStartPixelFromRight_Dark.NormalBackColor = txt_ChipStartPixelFromRight_Dark.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromRight_Dark.BackColor = txt_ChipStartPixelExtendFromRight_Dark.NormalBackColor = txt_ChipStartPixelExtendFromRight_Dark.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromBottom_Dark.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromBottom_Dark.BackColor = txt_ChipStartPixelFromBottom_Dark.NormalBackColor = txt_ChipStartPixelFromBottom_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromBottom_Dark.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelExtendFromBottom_Dark.BackColor = txt_ChipStartPixelExtendFromBottom_Dark.NormalBackColor = txt_ChipStartPixelExtendFromBottom_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromBottom_Dark.BackColor = txt_ChipStartPixelFromBottom_Dark.NormalBackColor = txt_ChipStartPixelFromBottom_Dark.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromBottom_Dark.BackColor = txt_ChipStartPixelExtendFromBottom_Dark.NormalBackColor = txt_ChipStartPixelExtendFromBottom_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromBottom_Dark.BackColor = txt_ChipStartPixelFromBottom_Dark.NormalBackColor = txt_ChipStartPixelFromBottom_Dark.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromBottom_Dark.BackColor = txt_ChipStartPixelExtendFromBottom_Dark.NormalBackColor = txt_ChipStartPixelExtendFromBottom_Dark.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromLeft_Dark.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromLeft_Dark.BackColor = txt_ChipStartPixelFromLeft_Dark.NormalBackColor = txt_ChipStartPixelFromLeft_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromLeft_Dark.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelExtendFromLeft_Dark.BackColor = txt_ChipStartPixelExtendFromLeft_Dark.NormalBackColor = txt_ChipStartPixelExtendFromLeft_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                tab_VisionControl.SelectedTab = tp_ROI2;
                txt_ChipStartPixelFromLeft_Dark.BackColor = txt_ChipStartPixelFromLeft_Dark.NormalBackColor = txt_ChipStartPixelFromLeft_Dark.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromLeft_Dark.BackColor = txt_ChipStartPixelExtendFromLeft_Dark.NormalBackColor = txt_ChipStartPixelExtendFromLeft_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromLeft_Dark.BackColor = txt_ChipStartPixelFromLeft_Dark.NormalBackColor = txt_ChipStartPixelFromLeft_Dark.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromLeft_Dark.BackColor = txt_ChipStartPixelExtendFromLeft_Dark.NormalBackColor = txt_ChipStartPixelExtendFromLeft_Dark.FocusBackColor = Color.White;
            }

            return blnSettingOK;
        }
        private void cbo_SelectROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex)
                {
                    m_smVisionInfo.g_arrPackageROIs[i][0].ClearDragHandle();
                    m_smVisionInfo.g_arrPackageROIs[i][0].VerifyROIArea(m_smVisionInfo.g_arrPackageROIs[i][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrPackageROIs[i][0].ref_ROITotalCenterY);

                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    m_smVisionInfo.g_intSelectedUnit = i;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;
                }
                else
                    m_smVisionInfo.g_arrPackageROIs[i][0].ClearDragHandle();
            }
        }
        private void txt_LeadDontCareInwardTolerance_Top_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intLeadDontCareInwardTolerance_Top = Convert.ToInt32(txt_LeadDontCareInwardTolerance_Top.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_LeadDontCareInwardTolerance_Right_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intLeadDontCareInwardTolerance_Right = Convert.ToInt32(txt_LeadDontCareInwardTolerance_Right.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_LeadDontCareInwardTolerance_Bottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intLeadDontCareInwardTolerance_Bottom = Convert.ToInt32(txt_LeadDontCareInwardTolerance_Bottom.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_LeadDontCareInwardTolerance_Left_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intLeadDontCareInwardTolerance_Left = Convert.ToInt32(txt_LeadDontCareInwardTolerance_Left.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_LeadDontCareInwardTolerance_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_intSelectedImage = 0;// m_smVisionInfo.g_arrLead[0].ref_intImageViewNo; //2021-08-03 ZJYEOH : should display mark image as this is setting for mark
            m_smVisionInfo.g_blnViewLeadDontCareInwardTolerance = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_LeadDontCareInwardTolerance_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_intSelectedImage = m_intPreviousSelectedImage;
            m_smVisionInfo.g_blnViewLeadDontCareInwardTolerance = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

        private void btn_iterationsettings_Click(object sender, EventArgs e)
        {
            if ((m_smVisionInfo.g_arrMarks.Count > 0) && (m_smVisionInfo.g_arrMarkROIs.Count > 0))
            {
                if (m_smVisionInfo.g_arrMarkROIs[0].Count > 0)
                {
                    LiterationSettingsForm objForm = new LiterationSettingsForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe);
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
    }
}