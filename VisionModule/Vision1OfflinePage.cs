using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharedMemory;
using VisionProcessing;
using Common;


namespace VisionModule
{
    public partial class Vision1OfflinePage : Form
    {
        #region Member Variables
        private bool[] m_blnFailMark;
        private bool[] m_blnFailOCR;
        private bool[] m_blnFailOrient;
        private bool[] m_blnFailPin1;
        private bool m_blnFailPackage = false;
        private bool m_blnFailLead = false;
        private bool m_blnMarkInspected = false;
        private bool m_blnOrientInspected = false;
        private bool m_blnPackageInspected = false;
        private bool m_blnLeadInspected = false;
        private DataGridView[] m_dgdView = new DataGridView[1];
        private DataGridView[] m_dgdDefectTable = new DataGridView[1];
        private DataGridView m_dgdLeadGroupDimensionTable = new DataGridView();
        private List<string> m_arrMarkResult2Items = new List<string>();    // 2019 10 05 - use this array string in order to support multi language
        
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private int m_intSettingType = 0;
        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private UserRight m_objUserRight = new UserRight();
        #endregion


        public Vision1OfflinePage(CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, VisionInfo smVisionInfo, int intUserGroup)
        {
            InitializeComponent();
            m_intUserGroup = intUserGroup;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            
            m_dgdView[0] = dgd_Lead;
            m_dgdLeadGroupDimensionTable = dgd_Lead_GroupDimension2;
            m_dgdDefectTable[0] = dgd_LeadDefect;
            DisableField();
            UpdateGUI();
            CustomizeGUI();

            m_blnInitDone = true;
        }

        private void DisableField()
        {
            string strChild1 = "Test Page";
            string strChild2 = "";

            //strChild2 = "Inspect";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //    btn_Inspect.Enabled = false;
        }

        private void ReadLeadTemplateDataToGrid(DataGridView dgd_LeadSetting)
        {
            int intBlobCount;
            dgd_LeadSetting.Rows.Clear();
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                intBlobCount = m_smVisionInfo.g_arrLead[i].GetBlobsFeaturesNumber();

                for (int j = 0; j < intBlobCount; j++)
                {
                    dgd_LeadSetting.Rows.Add();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                //Get first lead number
                int intBlobID = m_smVisionInfo.g_arrLead[i].GetBlobsNoID();
                intBlobCount = m_smVisionInfo.g_arrLead[i].GetBlobsFeaturesNumber();

                for (int j = intBlobID - 1; j < intBlobCount + intBlobID - 1; j++)
                {
                    if (j >= 0 && j < dgd_LeadSetting.Rows.Count)
                        dgd_LeadSetting.Rows[j].HeaderCell.Value = "Lead " + (j + 1);
                    else
                    {
                        STTrackLog.WriteLine("Visoin1OfflinePage > ReadLeadTemplateDataToGrid > j out of rows index.");
                        STTrackLog.WriteLine("intBlobID = " + intBlobID.ToString());
                        STTrackLog.WriteLine("intBlobCount = " + intBlobCount.ToString());
                        STTrackLog.WriteLine("Grid Row Count = " + dgd_LeadSetting.Rows.Count);

                    }
                }
            }
        }

        private void UpdateGUI()
        {
            lbl_ResultStatus1.Text = "-----";
            lbl_ResultStatus1.BackColor = Color.Gray;
            lbl_ResultStatus2.Text = "-----";
            lbl_ResultStatus2.BackColor = Color.White;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                m_smVisionInfo.g_blnInspectAllTemplate = m_smVisionInfo.g_arrMarks[u].ref_blnInspectAllTemplate = true;

            // 2019 04 10 - CCENG: Hide multi template radio button Want Mark is false
            if ((m_smVisionInfo.g_arrMarks[0].GetNumTemplates() <= 1) || (m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) == 0 || !m_smVisionInfo.g_blnWantMultiTemplates)
            {
                pnl_Template.Visible = false;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;
            }
            else
            {
                pnl_Template.Visible = true;
                cbo_TemplateNo.Items.Clear();
                for (int i = 0; i < m_smVisionInfo.g_arrMarks[0].GetNumTemplates(); i++)
                {
                    //if ((m_smVisionInfo.g_arrMarks[0].ref_intTemplateMask & 1 << i) > 0)
                    cbo_TemplateNo.Items.Add((i + 1));
                }

                cbo_TemplateNo.SelectedIndex = m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;
            }

            m_blnFailMark = new bool[cbo_TemplateNo.Items.Count];
            m_blnFailOrient = new bool[cbo_TemplateNo.Items.Count];
            m_blnFailPin1 = new bool[cbo_TemplateNo.Items.Count];
            m_blnFailOCR = new bool[cbo_TemplateNo.Items.Count];

            for (int u = 0; u < m_blnFailMark.Length; u++)
            {
                m_blnFailOrient[u] = false;
                m_blnFailMark[u] = false;
                m_blnFailPin1[u] = false;
                m_blnFailOCR[u] = false;
            }

            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate))
                {
                    if (!tab_Result.TabPages.Contains(tp_Pin1))
                        tab_Result.TabPages.Add(tp_Pin1);
                }
                else
                {
                    if (tab_Result.TabPages.Contains(tp_Pin1))
                        tab_Result.TabPages.Remove(tp_Pin1);
                }
            }
            else
            {
                if (tab_Result.TabPages.Contains(tp_Pin1))
                    tab_Result.TabPages.Remove(tp_Pin1);
            }

            if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_blnWantMark2DCode)
                pnl_Mark2DCode.Visible = true;
            else
                pnl_Mark2DCode.Visible = false;


            //switch (m_smVisionInfo.g_intSelectedTemplate)
            //{
            //    case 0:
            //        if (!radioBtn_Template1.Enabled)
            //        {
            //            if (radioBtn_Template2.Enabled)
            //            {
            //                radioBtn_Template2.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 1;
            //            }
            //            else if (radioBtn_Template3.Enabled)
            //            {
            //                radioBtn_Template3.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 2;
            //            }
            //            else if (radioBtn_Template4.Enabled)
            //            {
            //                radioBtn_Template4.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 3;
            //            }
            //        }
            //        else
            //            radioBtn_Template1.Checked = true;
            //        break;
            //    case 1:
            //        if (!radioBtn_Template2.Enabled)
            //        {
            //            if (radioBtn_Template1.Enabled)
            //            {
            //                radioBtn_Template1.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 0;
            //            }
            //            else if (radioBtn_Template3.Enabled)
            //            {
            //                radioBtn_Template3.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 2;
            //            }
            //            else if (radioBtn_Template4.Enabled)
            //            {
            //                radioBtn_Template4.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 3;
            //            }
            //        }
            //        else
            //            radioBtn_Template2.Checked = true;
            //        break;
            //    case 2:
            //        if (!radioBtn_Template3.Enabled)
            //        {
            //            if (radioBtn_Template1.Enabled)
            //            {
            //                radioBtn_Template1.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 0;
            //            }
            //            else if (radioBtn_Template2.Enabled)
            //            {
            //                radioBtn_Template2.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 1;
            //            }
            //            else if (radioBtn_Template4.Enabled)
            //            {
            //                radioBtn_Template4.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 3;
            //            }
            //        }
            //        else
            //            radioBtn_Template3.Checked = true;
            //        break;
            //    case 3:
            //        if (!radioBtn_Template4.Enabled)
            //        {
            //            if (radioBtn_Template1.Enabled)
            //            {
            //                radioBtn_Template1.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 0;
            //            }
            //            else if (radioBtn_Template2.Enabled)
            //            {
            //                radioBtn_Template2.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 1;
            //            }
            //            else if (radioBtn_Template3.Enabled)
            //            {
            //                radioBtn_Template3.Checked = true;
            //                m_smVisionInfo.g_intSelectedTemplate = 2;
            //            }
            //        }
            //        else
            //            radioBtn_Template4.Checked = true;
            //        break;
            //}

            // If not Inpocket module
            if (m_smVisionInfo.g_intUnitsOnImage == 1)
            {
                dgd_MarkResult1.Columns[2].Visible = false;
                dgd_MarkResult1.Columns[4].Visible = false;
                dgd_MarkResult1.Columns[6].Visible = false;
                dgd_MarkResult1.Columns[8].Visible = false;
                dgd_MarkResult1.Columns[10].Visible = false;
                dgd_MarkResult1.Columns[12].Visible = false;
                dgd_OCRResult.Columns[2].Visible = false;
                dgd_OCRResult.Columns[4].Visible = false;
                dgd_OrientResult.Columns[2].Visible = false;
                dgd_Pin1Result.Columns[2].Visible = false;
                lbl_ResultStatus2.Visible = false;
            }

            //Hide setting column in test page
            dgd_MarkResult1.Columns[1].Visible = false;
            dgd_OCRResult.Columns[1].Visible = false;
            dgd_MarkResult2.Columns[1].Visible = false;
            dgd_OrientResult.Columns[1].Visible = false;
            dgd_Pin1Result.Columns[1].Visible = false;

            int intFailMask = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
            if ((intFailMask & 0x01) == 0)   // 0x01=Excess Mark
            {
                dgd_MarkResult1.Columns[4].Visible = false;
                dgd_MarkResult1.Columns[5].Visible = false;
            }
            else
            {
                if (m_smVisionInfo.g_intUnitsOnImage > 1)
                {
                    dgd_MarkResult1.Columns[4].Visible = true;
                }
                dgd_MarkResult1.Columns[5].Visible = true;
            }

            if ((intFailMask & 0x10) == 0)   // 0x01=Miss Mark
            {
                dgd_MarkResult1.Columns[6].Visible = false;
                dgd_MarkResult1.Columns[7].Visible = false;
            }
            else
            {
                if (m_smVisionInfo.g_intUnitsOnImage > 1)
                {
                    dgd_MarkResult1.Columns[6].Visible = true;
                }
                dgd_MarkResult1.Columns[7].Visible = true;
            }

            if (!m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue || ((intFailMask & 0x200) == 0))
            {
                dgd_MarkResult1.Columns[12].Visible = false;
                dgd_MarkResult1.Columns[13].Visible = false;
            }
            else
            {
                if (m_smVisionInfo.g_intUnitsOnImage > 1)
                {
                    dgd_MarkResult1.Columns[12].Visible = true;
                }
                dgd_MarkResult1.Columns[13].Visible = true;
            }

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetThreshold();

            if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                // 2020 06 28 - Remove mark tabpage if Mark Checking is not selected in Option page
                if (m_smVisionInfo.g_arrMarks != null && m_smVisionInfo.g_arrMarks[0] != null &&
                    m_smVisionInfo.g_arrMarks[0].ref_blnCheckMark)
                {
                    if (!tab_Result.TabPages.Contains(tp_Mark))
                    {
                        tab_Result.TabPages.Add(tp_Mark);
                    }

                    if (((m_smCustomizeInfo.g_intWantOCR2 & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && (m_smVisionInfo.g_blnUseOCR || m_smVisionInfo.g_blnUseOCRandOCV) && !tab_Result.TabPages.Contains(tp_OCR))
                    {
                        tab_Result.TabPages.Add(tp_OCR);
                    }
                    else if (((m_smCustomizeInfo.g_intWantOCR2 & (1 << m_smVisionInfo.g_intVisionPos)) == 0) || (!m_smVisionInfo.g_blnUseOCR && !m_smVisionInfo.g_blnUseOCRandOCV))
                    {
                        if (tab_Result.TabPages.Contains(tp_OCR))
                        {
                            tab_Result.TabPages.Remove(tp_OCR);
                        }
                    }

                    InitMarkResult2GUI();
                }
                else
                {
                    if (tab_Result.TabPages.Contains(tp_Mark))
                    {
                        tab_Result.TabPages.Remove(tp_Mark);
                    }

                    if (tab_Result.TabPages.Contains(tp_OCR))
                    {
                        tab_Result.TabPages.Remove(tp_OCR);
                    }
                }
            }
            else
            {
                if (tab_Result.TabPages.Contains(tp_Mark))
                {
                    tab_Result.TabPages.Remove(tp_Mark);
                }

                if (tab_Result.TabPages.Contains(tp_OCR))
                {
                    tab_Result.TabPages.Remove(tp_OCR);
                }
            }

            if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (tab_Result.TabPages.Contains(tp_Orient))
                    tab_Result.TabPages.Remove(tp_Orient);
            }
            else if (m_smVisionInfo.g_arrOrients != null && m_smVisionInfo.g_arrOrients.Count > 0 && m_smVisionInfo.g_arrOrients[0].Count > 0)
            {
                if (m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientation)
                {
                    if (!tab_Result.TabPages.Contains(tp_Orient))
                        tab_Result.TabPages.Add(tp_Orient);
                }
                else
                {
                    if (tab_Result.TabPages.Contains(tp_Orient))
                        tab_Result.TabPages.Remove(tp_Orient);
                }
            }
            //-----------------------------------Package-------------------------------------------


            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (!tab_Result.TabPages.Contains(tp_Package))
                    tab_Result.TabPages.Add(tp_Package);

                if ((m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || m_smVisionInfo.g_strVisionName.Contains("BottomOrient")) && (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if ((m_smVisionInfo.g_arrPackage[0].ref_intFailMask & 0x1000) == 0 || !m_smVisionInfo.g_blnOrientWantPackage)
                    {
                        if (tab_Result.Contains(tp_Package))
                        {
                            tab_Result.Controls.Remove(tp_Package);
                        }
                    }
                    dgd_Defect.Visible = false;
                }

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
                            if (m_smVisionInfo.g_arrOrientGauge.Count > 0)
                            {
                                if (m_smVisionInfo.g_arrPackageROIs.Count > m_smVisionInfo.g_intSelectedUnit)
                                {
                                    if (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit].Count > 0)
                                    {
                                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].LoadROISetting(
                                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX,
                                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY,
                                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                //if (m_smVisionInfo.g_arrPackageGauge.Count != 0)
                //{
                //    float fX = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fTemplateUnitSizeX;
                //    float fY = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fTemplateUnitSizeY;
                //    dgd_Package.Rows.Clear();
                //    dgd_Package.Rows.Add();
                //    dgd_Package.Rows[0].DefaultCellStyle.BackColor = Color.Lime;
                //    dgd_Package.Rows[0].Cells[0].Value = "Template";
                //    if (m_smVisionInfo.g_arrPackageROIs.Count > 0)
                //    {
                //        if (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit].Count > 1)
                //            dgd_Package.Rows[0].Cells[1].Value = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_fAreaPixel.ToString("f4");
                //    }
                //    dgd_Package.Rows[0].Cells[2].Value = fX.ToString("f4");
                //    dgd_Package.Rows[0].Cells[3].Value = fY.ToString("f4");
                //}
                dgd_Package.Rows.Clear();
                dgd_Defect.Rows.Clear();
            }
            else
            {
                if (tab_Result.TabPages.Contains(tp_Package))
                    tab_Result.TabPages.Remove(tp_Package);
            }

            //------------------------------------------Lead----------------------------------------

            if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (!tab_Result.TabPages.Contains(tp_Lead))
                    tab_Result.TabPages.Add(tp_Lead);

                ReadLeadTemplateDataToGrid(m_dgdView[0]);

                m_dgdLeadGroupDimensionTable.Rows.Clear();
                m_dgdLeadGroupDimensionTable.Visible = true;

                m_dgdDefectTable[0].Rows.Clear();
                m_dgdDefectTable[0].Visible = true;
                if (!m_smVisionInfo.g_arrLead[0].ref_blnWantCheckExtraLeadLength)
                {
                    m_dgdDefectTable[0].Columns[2].Visible = false;
                    m_dgdDefectTable[0].Columns[3].Visible = false;
                }
                else
                {
                    m_dgdDefectTable[0].Columns[2].Visible = true;
                    m_dgdDefectTable[0].Columns[3].Visible = true;
                }
                if (!m_smVisionInfo.g_arrLead[0].ref_blnWantCheckExtraLeadArea && ((m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask & 0x2000) == 0))
                    m_dgdDefectTable[0].Columns[4].Visible = false;
                else
                    m_dgdDefectTable[0].Columns[4].Visible = true;
                int intFailOptionMask = m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask;

               m_dgdView[0].Columns[0].Visible = ((intFailOptionMask & 0x100) > 0);
                m_dgdView[0].Columns[1].Visible = ((intFailOptionMask & 0x8000) > 0);
                m_dgdView[0].Columns[2].Visible = ((intFailOptionMask & 0xC0) > 0);
                m_dgdView[0].Columns[3].Visible = ((intFailOptionMask & 0xC0) > 0);
                m_dgdView[0].Columns[4].Visible = ((intFailOptionMask & 0x600) > 0);
                m_dgdView[0].Columns[5].Visible = ((intFailOptionMask & 0x600) > 0);
                m_dgdView[0].Columns[6].Visible = false; //((intFailOptionMask & 0x800) > 0);
                m_dgdView[0].Columns[7].Visible = ((intFailOptionMask & 0x4000) > 0);
                m_dgdView[0].Columns[8].Visible = ((intFailOptionMask & 0x10000) > 0);
                m_dgdView[0].Columns[9].Visible = ((intFailOptionMask & 0x20000) > 0);

                pnl_Span.Visible = ((intFailOptionMask & 0x1000) > 0);

                if (!m_smVisionInfo.g_arrLead[0].ref_blnWantUseAverageGrayValueMethod)
                {
                    if (m_dgdView[0].Columns[7].Visible)
                        m_dgdView[0].Columns[7].Visible = false;
                }

                if (!m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                {
                    if (m_dgdView[0].Columns[8].Visible)
                        m_dgdView[0].Columns[8].Visible = false;
                    if (m_dgdView[0].Columns[9].Visible)
                        m_dgdView[0].Columns[9].Visible = false;
                }

                if (m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                {
                    if (m_dgdView[0].Columns[1].Visible)
                        m_dgdView[0].Columns[1].Visible = false;
                }
            }
            else
            {
                if (tab_Result.TabPages.Contains(tp_Lead))
                    tab_Result.TabPages.Remove(tp_Lead);
            }
            
        }

        public void CustomizeGUI()
        {
            UpdateMarkResult2Table(true);
            UpdateAllResultTable();

            if (tab_Result.SelectedTab != null)
            {
                switch (tab_Result.SelectedTab.Name)
                {
                    case "tp_Orient":
                        m_intSettingType = 0;
                        break;
                    case "tp_Mark":
                        m_intSettingType = 1;
                        break;
                    case "tp_Pin1":
                        m_intSettingType = 2;
                        break;
                }
            }

            //m_intSettingType = tab_Result.SelectedIndex;
            UpdateTabPage();

            //---------------------------------------------Package---------------------------------------------------

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                UpdatePackageResultTable();
            }

            //---------------------------------------------Lead---------------------------------------------------

            if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                UpdateLeadResultTable(m_dgdView[0]);
                //UpdateLeadDefectTable(0, m_dgdView[0]);

                int intFailOptionMask = m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask;

                m_dgdView[0].Columns[0].Visible = ((intFailOptionMask & 0x100) > 0);
                m_dgdView[0].Columns[1].Visible = ((intFailOptionMask & 0x8000) > 0);
                m_dgdView[0].Columns[2].Visible = ((intFailOptionMask & 0xC0) > 0);
                m_dgdView[0].Columns[3].Visible = ((intFailOptionMask & 0xC0) > 0);
                m_dgdView[0].Columns[4].Visible = ((intFailOptionMask & 0x600) > 0);
                m_dgdView[0].Columns[5].Visible = ((intFailOptionMask & 0x600) > 0);
                m_dgdView[0].Columns[6].Visible = false; //((intFailOptionMask & 0x800) > 0);
                m_dgdView[0].Columns[7].Visible = ((intFailOptionMask & 0x4000) > 0);
                m_dgdView[0].Columns[8].Visible = ((intFailOptionMask & 0x10000) > 0);
                m_dgdView[0].Columns[9].Visible = ((intFailOptionMask & 0x20000) > 0);

                if (!m_smVisionInfo.g_arrLead[0].ref_blnWantCheckExtraLeadLength)
                {
                    m_dgdDefectTable[0].Columns[2].Visible = false;
                    m_dgdDefectTable[0].Columns[3].Visible = false;
                }
                else
                {
                    m_dgdDefectTable[0].Columns[2].Visible = true;
                    m_dgdDefectTable[0].Columns[3].Visible = true;
                }
                if (!m_smVisionInfo.g_arrLead[0].ref_blnWantCheckExtraLeadArea && ((m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask & 0x2000) == 0))
                    m_dgdDefectTable[0].Columns[4].Visible = false;
                else
                    m_dgdDefectTable[0].Columns[4].Visible = true;

                pnl_Span.Visible = ((intFailOptionMask & 0x1000) > 0);

                if (!m_smVisionInfo.g_arrLead[0].ref_blnWantUseAverageGrayValueMethod)
                {
                    if (m_dgdView[0].Columns[7].Visible)
                        m_dgdView[0].Columns[7].Visible = false;
                }

                if (!m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                {
                    if (m_dgdView[0].Columns[8].Visible)
                        m_dgdView[0].Columns[8].Visible = false;
                    if (m_dgdView[0].Columns[9].Visible)
                        m_dgdView[0].Columns[9].Visible = false;
                }

                if (m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
                {
                    if (m_dgdView[0].Columns[1].Visible)
                        m_dgdView[0].Columns[1].Visible = false;
                }

                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    switch (i)
                    {
                        case 1:  // Top
                            if (m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead > 0)
                                m_dgdLeadGroupDimensionTable.Columns[4].Visible = true;
                            else
                                m_dgdLeadGroupDimensionTable.Columns[4].Visible = false;
                            break;
                        case 2: // Right
                            if (m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead > 0)
                                m_dgdLeadGroupDimensionTable.Columns[3].Visible = true;
                            else
                                m_dgdLeadGroupDimensionTable.Columns[3].Visible = false;
                            break;
                        case 3: // Bottom
                            if (m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead > 0)
                                m_dgdLeadGroupDimensionTable.Columns[5].Visible = true;
                            else
                                m_dgdLeadGroupDimensionTable.Columns[5].Visible = false;
                            break;
                        case 4: // Left
                            if (m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead > 0)
                                m_dgdLeadGroupDimensionTable.Columns[2].Visible = true;
                            else
                                m_dgdLeadGroupDimensionTable.Columns[2].Visible = false;
                            break;
                    }
                }
            }
        }

        private int GetBiggestSelectedTemplate()
        {
            int intBiggerSelectedTemplate;
            if (m_smVisionInfo.g_blnInspectAllTemplate)
            {
                intBiggerSelectedTemplate = 0;
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    if (m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex > intBiggerSelectedTemplate)
                        intBiggerSelectedTemplate = m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex;
                }
            }
            else
                intBiggerSelectedTemplate = m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex;

            return intBiggerSelectedTemplate;
        }

        private void UpdateInfo()
        {
            m_blnOrientInspected = m_blnMarkInspected = m_smVisionInfo.g_blnMarkInspected;
            m_blnPackageInspected = m_smVisionInfo.g_blnPackageInspected;
            m_blnLeadInspected = m_smVisionInfo.g_blnLeadInspected;

            m_smVisionInfo.g_intSelectedTemplate = GetBiggestSelectedTemplate();
            //switch (m_smVisionInfo.g_intSelectedTemplate)
            //{
            //    case 0:
            //        radioBtn_Template1.Checked = true;
            //        break;
            //    case 1:
            //        radioBtn_Template2.Checked = true;
            //        break;
            //    case 2:
            //        radioBtn_Template3.Checked = true;
            //        break;
            //    case 3:
            //        radioBtn_Template4.Checked = true;
            //        break;
            //}

            if (cbo_TemplateNo.Items.Contains((m_smVisionInfo.g_intSelectedTemplate + 1)))
                cbo_TemplateNo.SelectedItem = (m_smVisionInfo.g_intSelectedTemplate + 1);
            else
                cbo_TemplateNo.SelectedIndex = 0;

            if (m_smVisionInfo.g_intSelectedImage != m_smVisionInfo.g_intProductionViewImage)
            {
                m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;

                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
            }

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                {
                    if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                        lbl_ResultStatus1.BackColor = Color.Lime;
                    else if (m_smVisionInfo.g_strResult == "Fail" || m_smVisionInfo.g_strResult == "NoEmpty")
                        lbl_ResultStatus1.BackColor = Color.Red;
                }
                else
                {
                    if (m_smVisionInfo.g_strResult2 == "Pass")
                        lbl_ResultStatus2.BackColor = Color.Lime;
                    else if (m_smVisionInfo.g_strResult2 == "Fail")
                        lbl_ResultStatus2.BackColor = Color.Red;
                }

                if (((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0) &&    // Orient 0 deg off mean Orientation ON.
                    (m_smVisionInfo.g_arrOrients != null && m_smVisionInfo.g_arrOrients.Count > 0 && m_smVisionInfo.g_arrOrients[0].Count > 0 &&
                    m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientation))
                    UpdateOrientResult(u);
                else if (m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_WantUsePin1OrientationWhenNoMark &&
                           ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0) &&
                           !m_smVisionInfo.g_arrMarks[0].ref_blnCheckMark && !m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientation && m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate))
                    UpdateOrientResult(u);
                else
                {
                    if (u == 0)
                    {
                        if (m_smVisionInfo.g_strResult == "Pass")
                        {

                            lbl_ResultStatus1.Text = "Pass";

                        }
                        else if (m_smVisionInfo.g_strResult == "Fail")
                        {
                            lbl_ResultStatus1.Text = "Fail";

                        }
                    }
                    else
                    {
                        if (m_smVisionInfo.g_strResult == "Pass")
                        {

                            lbl_ResultStatus2.Text = "Pass";

                        }
                        else if (m_smVisionInfo.g_strResult == "Fail")
                        {
                            lbl_ResultStatus2.Text = "Fail";

                        }
                    }
                }
            }

            if (m_smVisionInfo.g_blnNoGrabTime)
            {
                lbl_GrabDelay.Text = "0";
                lbl_GrabTime.Text = "0";
                lbl_ProcessTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");
            }
            else
            {
                lbl_GrabDelay.Text = m_smVisionInfo.g_intCameraGrabDelay.ToString();
                lbl_GrabTime.Text = (Math.Max(0, m_smVisionInfo.g_objGrabTime.Duration - m_smVisionInfo.g_intCameraGrabDelay)).ToString("f0"); //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
                lbl_ProcessTime.Text = (Math.Max(0, m_smVisionInfo.g_objTotalTime.Duration - m_smVisionInfo.g_objGrabTime.Duration)).ToString("f2");
            }
            lbl_TotalTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");

            m_smVisionInfo.g_blnMarkSelecting = false;
            m_smVisionInfo.g_blnViewSelectedBlobObject = false;
            if (!m_smVisionInfo.g_blnWantSkipMark)
                m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                InitMarkResult2GUI();
                UpdateAllResultTable();
                UpdateMarkResult2Table(false);
            }
            else
            {
                UpdateOrientResultOnly();
            }

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                UpdatePackageResultTable();
            }

            if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                UpdateLeadResultTable(m_dgdView[0]);
            }

            if (dgd_Defect.RowCount == 0)
            {
                if (m_smVisionInfo.PR_MN_TestDone && m_smVisionInfo.AT_VM_ManualTestMode)
                {
                    // 2019 07 15 - Should display selected image view page if no defect.
                    if (m_smVisionInfo.g_intSelectedImage != m_smVisionInfo.g_intProductionViewImage)
                    {
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;

                        m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
                    }
                    //m_smVisionInfo.g_intSelectedImage =0;
                }
            }
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewLeadInspection = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdatePageSetting()
        {
            //Reset Mark Inspection data
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                m_smVisionInfo.g_arrMarks[u].ResetInspectionData(true);

           
            if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                //Reset Lead Inspection data
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    if (i != 0 && !m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        continue;

                    m_smVisionInfo.g_arrLead[i].ResetInspectionData();
                }
            }

            UpdateGUI();
        }

        private void UpdateOrientResult(int intUnitNo)
        {
            if (intUnitNo == 0)
                lbl_ResultStatus1.Text = "";
            else
                lbl_ResultStatus2.Text = "";

            //2019 04 10 - CCENG: All these visions (Bottom Orient, Orient(Top) or Mark Orient) will display orientation deg result
            //if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                switch (m_smVisionInfo.g_intOrientResult[intUnitNo])
                {
                    case 0:
                        if (intUnitNo == 0)
                            lbl_ResultStatus1.Text += "0";
                        else
                            lbl_ResultStatus2.Text += "0";
                        break;
                    case 1:
                        if (m_smCustomizeInfo.g_intOrientIO == 0)
                        {
                            if (intUnitNo == 0)
                                lbl_ResultStatus1.Text += "-90";
                            else
                                lbl_ResultStatus2.Text += "-90";
                        }
                        else
                        {
                            if (intUnitNo == 0)
                                lbl_ResultStatus1.Text += "90";
                            else
                                lbl_ResultStatus2.Text += "90";
                        }
                        break;
                    case 2:
                        if (intUnitNo == 0)
                            lbl_ResultStatus1.Text += "180";
                        else
                            lbl_ResultStatus2.Text += "180";
                        break;
                    case 3:
                        if (m_smCustomizeInfo.g_intOrientIO == 0)
                        {
                            if (intUnitNo == 0)
                                lbl_ResultStatus1.Text += "90";
                            else
                                lbl_ResultStatus2.Text += "90";
                        }
                        else
                        {
                            if (intUnitNo == 0)
                                lbl_ResultStatus1.Text += "-90";
                            else
                                lbl_ResultStatus2.Text += "-90";
                        }
                        break;
                    case 4:
                    default:
                        if (intUnitNo == 0)
                            lbl_ResultStatus1.Text += "Fail";
                        else
                            lbl_ResultStatus2.Text += "Fail";
                        break;
                }
            }
            //else
            //{
            //    switch (m_smVisionInfo.g_intOrientResult[intUnitNo])
            //    {
            //        case 0:
            //        case 1:
            //        case 2:
            //        case 3:
            //            if (intUnitNo == 0)
            //                lbl_ResultStatus1.Text += "Pass";
            //            else
            //                lbl_ResultStatus2.Text += "Pass";
            //            break;
            //        case 4:
            //            if (intUnitNo == 0)
            //                lbl_ResultStatus1.Text += "Fail";
            //            else
            //                lbl_ResultStatus2.Text += "Fail";
            //            break;
            //    }
            //}
       }

        private void UpdateTabPage()
        {
            switch (m_intSettingType)
            {
                case 0: // Orient Page                        
                    tp_Orient.Controls.Add(pnl_Template);
                    //m_smVisionInfo.g_blnDrawPkgResult = true;
                    //m_smVisionInfo.g_blnDrawPin1Result = true;
                    //if (m_blnMarkInspected)
                    //    m_smVisionInfo.g_blnDrawMarkResult = true;
                    //if (m_blnPackageInspected)
                    //    m_smVisionInfo.g_blnPackageInspected = true;
                    break;
                case 1: // Mark Page                        
                    tp_Mark.Controls.Add(pnl_Template);
                    //m_smVisionInfo.g_blnDrawPkgResult = false;
                    //m_smVisionInfo.g_blnDrawPin1Result = false;
                    //if (m_blnMarkInspected)
                    //    m_smVisionInfo.g_blnDrawMarkResult = true;
                    //m_smVisionInfo.g_blnPackageInspected = false;
                    break;
                case 2: // Pin1 Page
                    tp_Pin1.Controls.Add(pnl_Template);
                    //m_smVisionInfo.g_blnDrawPkgResult = false;
                    //m_smVisionInfo.g_blnDrawPin1Result = true;
                    //m_smVisionInfo.g_blnDrawMarkResult = false;
                    //m_smVisionInfo.g_blnPackageInspected = false;
                    break;
                case 3: // Package Page
                    //tp_Package.Controls.Add(pnl_Template);        // 2019 01 11 - CCENG: No multi template for Package
                    //m_smVisionInfo.g_blnDrawPkgResult = true;
                    //m_smVisionInfo.g_blnDrawPin1Result = false;
                    //m_smVisionInfo.g_blnDrawMarkResult = false;
                    //if (m_blnPackageInspected)
                    //    m_smVisionInfo.g_blnPackageInspected = true;
                    break;
                case 4: // Lead Page
                    //tp_Lead.Controls.Add(pnl_Template);           // 2019 01 11 - CCENG: No Multi template for lead
                    //m_smVisionInfo.g_blnDrawPkgResult = false;
                    //m_smVisionInfo.g_blnDrawPin1Result = false;
                    //m_smVisionInfo.g_blnDrawMarkResult = false;
                    //m_smVisionInfo.g_blnPackageInspected = false;
                    break;
            }

            m_smVisionInfo.g_blnMarkSelecting = false;
            m_smVisionInfo.g_blnViewSelectedBlobObject = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateOrientResultOnly()
        {
            //Orient table
            dgd_OrientResult.Rows.Clear();
            dgd_OrientResult.Rows.Add();
            dgd_OrientResult.Rows[0].Cells[0].Value = "Orient";
            dgd_OrientResult.Rows[0].Cells[3].Value = "----";
            dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.White;//Lime
            dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.White;//Lime
            dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Black;
            dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;

            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
            {
                tp_Orient.Text = "Position";
                dgd_OrientResult.Rows[0].Visible = false;
                dgd_OrientResult.Columns[0].HeaderText = "Position";

                if (m_smCustomizeInfo.g_intLanguageCulture != 1)
                {
                    tp_Orient.Text = "位置";
                    dgd_OrientResult.Columns[0].HeaderText = "位置";
                }
            }

            if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                dgd_OrientResult.Rows.Add();
                dgd_OrientResult.Rows[1].Cells[0].Value = "Angle";
                dgd_OrientResult.Rows[1].Cells[3].Value = "----";
                dgd_OrientResult.Rows[1].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[1].Cells[3].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionForeColor = Color.Black;
                dgd_OrientResult.Rows.Add();
                dgd_OrientResult.Rows[2].Cells[0].Value = "X Tol.(mm)";
                dgd_OrientResult.Rows[2].Cells[3].Value = "----";
                dgd_OrientResult.Rows[2].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[2].Cells[3].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionForeColor = Color.Black;
                dgd_OrientResult.Rows.Add();
                dgd_OrientResult.Rows[3].Cells[0].Value = "Y Tol.(mm)";
                dgd_OrientResult.Rows[3].Cells[3].Value = "----";
                dgd_OrientResult.Rows[3].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[3].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[3].Cells[3].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[3].Cells[3].Style.SelectionForeColor = Color.Black;
            }
            float fScore = -1;
            if (m_smVisionInfo.g_blnUnitInspected[0] && m_smVisionInfo.g_intSelectedTemplate < m_smVisionInfo.g_arrOrients[0].Count)
                fScore = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetMinScore() * 100;

            if (fScore >= 0)    //if (fScore > 0) // 2019 09 16 - CCENG: Change > to >= make sure orient result table will be updated when score is 0.
            {
                dgd_OrientResult.Rows[0].Cells[3].Value = fScore.ToString("f2");

                if (Convert.ToDouble(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fMinScore * 100) <= Convert.ToDouble(dgd_OrientResult.Rows[0].Cells[3].Value))
                {
                    dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                    dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                    if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        int intOrientAngle = 0;
                        if (m_smVisionInfo.g_intOrientResult[1] < 4)
                        {
                            switch (m_smVisionInfo.g_intOrientResult[1])
                            {
                                default:
                                case 0:
                                    intOrientAngle = 0;
                                    break;
                                case 1:
                                    intOrientAngle = 90;
                                    break;
                                case 2:
                                    intOrientAngle = 180;
                                    break;
                                case 3:
                                    intOrientAngle = -90;
                                    break;
                            }

                        }
                        //    float CenterX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;
                        //    float CenterY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2;
                        //                                                                                                                                          //2020-09-24 ZJYEOH : Should use current angle to rotate template center point because when get center point different, the object center point is based on current angle
                        //    float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - CenterX) * Math.Cos(intOrientAngle * Math.PI / 180)) - //m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_intRotatedAngle 
                        //((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - CenterY) * Math.Sin(intOrientAngle * Math.PI / 180)));
                        //    float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - CenterX) * Math.Sin(intOrientAngle * Math.PI / 180)) +
                        //      ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - CenterY) * Math.Cos(intOrientAngle * Math.PI / 180)));

                        float fXAfterRotated = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX;
                        float fYAfterRotated = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY;

                        float fAngleResult = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult);//GetResultAngle()
                        float fCenterXDiff = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].GetCenterXDiff(fXAfterRotated, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX); //m_smVisionInfo.g_fOrientCenterX[0];
                        float fCenterYDiff = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].GetCenterYDiff(fYAfterRotated, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY);// m_smVisionInfo.g_fOrientCenterY[0];

                        dgd_OrientResult.Rows[1].Cells[0].Value = "Angle";
                        dgd_OrientResult.Rows[1].Cells[3].Value = fAngleResult.ToString("f4");
                        if (m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientAngleTolerance)
                        {
                            if (m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fAngleTolerance <= fAngleResult)
                            {
                                if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                                {
                                    dgd_OrientResult.Rows[1].Cells[3].Style.BackColor = Color.LimeGreen;
                                    dgd_OrientResult.Rows[1].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                                    dgd_OrientResult.Rows[1].Cells[3].Style.ForeColor = Color.Black;
                                    dgd_OrientResult.Rows[1].Cells[3].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                                    dgd_OrientResult.Rows[1].Cells[3].Style.BackColor = Color.Red;
                                    dgd_OrientResult.Rows[1].Cells[3].Style.SelectionBackColor = Color.Red;
                                    dgd_OrientResult.Rows[1].Cells[3].Style.ForeColor = Color.Yellow;
                                    dgd_OrientResult.Rows[1].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                dgd_OrientResult.Rows[1].Cells[3].Style.BackColor = Color.Lime;
                                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_OrientResult.Rows[1].Cells[3].Style.ForeColor = Color.Black;
                                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        else
                        {
                            dgd_OrientResult.Rows[1].Visible = false;
                        }

                        dgd_OrientResult.Rows[2].Cells[0].Value = "X Tol.(mm)";
                        dgd_OrientResult.Rows[2].Cells[3].Value = fCenterXDiff.ToString("f4");

                        if (m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientXTolerance)
                        {
                            if (Math.Abs(fCenterXDiff) >= m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fXTolerance)
                            {
                                if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                                {
                                    dgd_OrientResult.Rows[2].Cells[3].Style.BackColor = Color.LimeGreen;
                                    dgd_OrientResult.Rows[2].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                                    dgd_OrientResult.Rows[2].Cells[3].Style.ForeColor = Color.Black;
                                    dgd_OrientResult.Rows[2].Cells[3].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                                    dgd_OrientResult.Rows[2].Cells[3].Style.BackColor = Color.Red;
                                    dgd_OrientResult.Rows[2].Cells[3].Style.SelectionBackColor = Color.Red;
                                    dgd_OrientResult.Rows[2].Cells[3].Style.ForeColor = Color.Yellow;
                                    dgd_OrientResult.Rows[2].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                dgd_OrientResult.Rows[2].Cells[3].Style.BackColor = Color.Lime;
                                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_OrientResult.Rows[2].Cells[3].Style.ForeColor = Color.Black;
                                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        else
                        {
                            dgd_OrientResult.Rows[2].Visible = false;
                        }

                        dgd_OrientResult.Rows[3].Cells[0].Value = "Y Tol.(mm)";
                        dgd_OrientResult.Rows[3].Cells[3].Value = fCenterYDiff.ToString("f4");

                        if (m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientYTolerance)
                        {
                            if (Math.Abs(fCenterYDiff) >= m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fYTolerance)
                            {
                                if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                                {
                                    dgd_OrientResult.Rows[3].Cells[3].Style.BackColor = Color.LimeGreen;
                                    dgd_OrientResult.Rows[3].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                                    dgd_OrientResult.Rows[3].Cells[3].Style.ForeColor = Color.Black;
                                    dgd_OrientResult.Rows[3].Cells[3].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                                    dgd_OrientResult.Rows[3].Cells[3].Style.BackColor = Color.Red;
                                    dgd_OrientResult.Rows[3].Cells[3].Style.SelectionBackColor = Color.Red;
                                    dgd_OrientResult.Rows[3].Cells[3].Style.ForeColor = Color.Yellow;
                                    dgd_OrientResult.Rows[3].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                dgd_OrientResult.Rows[3].Cells[3].Style.BackColor = Color.Lime;
                                dgd_OrientResult.Rows[3].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_OrientResult.Rows[3].Cells[3].Style.ForeColor = Color.Black;
                                dgd_OrientResult.Rows[3].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        else
                        {
                            dgd_OrientResult.Rows[3].Visible = false;
                        }
                    }
                }
                else
                {
                    if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                    {
                        dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.LimeGreen;
                        dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                        dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                        dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                        dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.Red;
                        dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                }
            }
            else
            {
                dgd_OrientResult.Rows[0].Cells[3].Value = "----";
                dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
            }

            fScore = -1;
            if (m_smVisionInfo.g_blnUnitInspected[1] && m_smVisionInfo.g_intUnitsOnImage == 2)
                fScore = m_smVisionInfo.g_arrOrients[1][m_smVisionInfo.g_intSelectedTemplate].GetMinScore() * 100;

            if (fScore >= 0)
            {
                dgd_OrientResult.Rows[0].Cells[2].Value = fScore.ToString("f2");

                if (Convert.ToDouble(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fMinScore * 100) <= Convert.ToDouble(dgd_OrientResult.Rows[0].Cells[2].Value))
                {
                    dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                    dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                    {
                        dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.LimeGreen;
                        dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                        dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                        dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                        dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.Red;
                        dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                        dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;
                    }
                }
            }
            else
            {
                dgd_OrientResult.Rows[0].Cells[2].Value = "----";
                dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
            }

            //Pin 1 table
            if (m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate))
            {
                dgd_Pin1Result.Rows.Clear();
                dgd_Pin1Result.Rows.Add();
                dgd_Pin1Result.Rows[0].Cells[0].Value = "Pin 1";
                dgd_Pin1Result.Rows[0].Cells[3].Value = "----";
                dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;

                fScore = -1;
                if (m_smVisionInfo.g_blnUnitInspected[0])
                    fScore = m_smVisionInfo.g_arrPin1[0].GetResultScore(m_smVisionInfo.g_intSelectedTemplate) * 100;

                if (fScore >= 0)
                {
                    dgd_Pin1Result.Rows[0].Cells[3].Value = fScore.ToString("f2");

                    if (Convert.ToDouble(m_smVisionInfo.g_arrPin1[0].GetMinScoreSetting(m_smVisionInfo.g_intSelectedTemplate) * 100) <= Convert.ToDouble(dgd_Pin1Result.Rows[0].Cells[3].Value))
                    {
                        dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                        {
                            dgd_Pin1Result.Rows[3].Cells[3].Style.BackColor = Color.LimeGreen;
                            dgd_Pin1Result.Rows[3].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                            dgd_Pin1Result.Rows[3].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Pin1Result.Rows[3].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            m_blnFailPin1[m_smVisionInfo.g_intSelectedTemplate] = true;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.Red;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                    }
                }
                else
                {
                    dgd_Pin1Result.Rows[0].Cells[3].Value = "----";
                    dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.White;//Lime
                    dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                    dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                fScore = -1;
                if (m_smVisionInfo.g_blnUnitInspected[1] && m_smVisionInfo.g_intUnitsOnImage == 2)
                    fScore = m_smVisionInfo.g_arrPin1[1].GetResultScore(m_smVisionInfo.g_intSelectedTemplate) * 100;

                if (fScore >= 0)
                {
                    dgd_Pin1Result.Rows[0].Cells[2].Value = fScore.ToString("f2");

                    if (Convert.ToDouble(m_smVisionInfo.g_arrPin1[1].GetMinScoreSetting(m_smVisionInfo.g_intSelectedTemplate) * 100) <= Convert.ToDouble(dgd_Pin1Result.Rows[0].Cells[2].Value))
                    {
                        dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                        dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                        dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                        dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                        {
                            dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.LimeGreen;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            m_blnFailPin1[m_smVisionInfo.g_intSelectedTemplate] = true;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.Red;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;
                        }
                    }
                }
                else
                {
                    dgd_Pin1Result.Rows[0].Cells[2].Value = "----";
                    dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.White;//Lime
                    dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                    dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                    dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                }
            }

        }

        //Update orient, mark, and pin 1 result
        private void UpdateAllResultTable()
        {
            dgd_MarkResult1.Rows.Clear();
            int intColumn = 0;
            int intExcessColumn = 0;
            int intBrolenColumn = 0;
            int intMissingColumn = 0;
            int intJointMarkColumn = 0;
            int intAGVColumn = 0;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (!m_smVisionInfo.g_blnUnitInspected[u])
                    continue;

                if (u == 0)
                {
                    intColumn = 3;
                    intExcessColumn = 5;
                    intBrolenColumn = 9;//8
                    intMissingColumn = 7;
                    intJointMarkColumn = 11;
                    intAGVColumn = 13;
                }
                else
                {
                    intColumn = 2;
                    intExcessColumn = 4;
                    intBrolenColumn = 8;//7
                    intMissingColumn = 6;
                    intJointMarkColumn = 10;
                    intAGVColumn = 12;
                }

                if (((m_smCustomizeInfo.g_intWantOCR2 & (1 << m_smVisionInfo.g_intVisionPos)) == 0) || (((m_smCustomizeInfo.g_intWantOCR2 & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_blnUseOCRandOCV))
                {
                    //Mark table
                    for (int i = 0; i < m_smVisionInfo.g_arrMarks[u].GetNumChars(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate); i++)
                {
                    if (dgd_MarkResult1.Rows.Count == i)
                    {
                        dgd_MarkResult1.Rows.Add();

                        string strMarkType = "";
                        switch (m_smVisionInfo.g_arrMarks[0].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate))
                        {
                            case 1:
                                strMarkType = "(L) ";
                                break;
                            case 2:
                                strMarkType = "(S1) ";
                                break;
                            case 3:
                                strMarkType = "(S2) ";
                                break;
                        }

                        dgd_MarkResult1.Rows[i].Cells[0].Value = strMarkType + "Mark " + (i + 1);

                        dgd_MarkResult1.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[u].GetCharSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();

                        dgd_MarkResult1.Rows[i].Cells[3].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[3].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;

                        dgd_MarkResult1.Rows[i].Cells[5].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[5].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[5].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;

                        dgd_MarkResult1.Rows[i].Cells[7].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[7].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[7].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[7].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;

                        dgd_MarkResult1.Rows[i].Cells[9].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[9].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[9].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[9].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[9].Style.SelectionForeColor = Color.Black;

                        dgd_MarkResult1.Rows[i].Cells[13].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[13].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[13].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[13].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[13].Style.SelectionForeColor = Color.Black;
                    }

                    bool blnEnableMark = true;
                    float fScoreValue = -1;
                    float fExcessValue = -1;
                    float fBrokenValue = -1;
                    float fAGVValue = 0;

                    if (m_smVisionInfo.g_blnMarkInspected)
                    {
                        if (m_smVisionInfo.g_arrMarks[u].ref_blnCharResult[m_smVisionInfo.g_intSelectedTemplate] != null && !m_smVisionInfo.g_arrMarks[u].GetCharResult(i, m_smVisionInfo.g_intSelectedTemplate))
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult1.Rows[i].Cells[0].Style.ForeColor = Color.Black;
                                dgd_MarkResult1.Rows[i].Cells[0].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkResult1.Rows[i].Cells[0].Style.ForeColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[0].Style.SelectionForeColor = Color.Red;
                            }
                        }

                        fScoreValue = m_smVisionInfo.g_arrMarks[u].GetCharScore(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        blnEnableMark = m_smVisionInfo.g_arrMarks[u].GetEnableMarkSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        fExcessValue = m_smVisionInfo.g_arrMarks[u].GetCharExcessArea(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        fBrokenValue = m_smVisionInfo.g_arrMarks[u].GetCharBrokenArea(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
                        fAGVValue = m_smVisionInfo.g_arrMarks[u].GetCharAverageGrayDiff(i, m_smVisionInfo.g_intSelectedTemplate);
                    }

                    if ((m_smVisionInfo.g_arrMarks[u].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate) & 0x80) > 0 && m_smVisionInfo.g_blnMarkInspected &&
                        (m_smVisionInfo.g_arrMarks[u].ref_intFailResultMask & 0x80) > 0)
                    {
                        dgd_MarkResult1.Columns[intJointMarkColumn].Visible = true;
                        if (!m_smVisionInfo.g_arrMarks[u].GetCharJointMark(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate))
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.ForeColor = Color.Black;
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkResult1.Rows[i].Cells[0].Style.ForeColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[0].Style.SelectionForeColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Value = "Joint Mark";
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.BackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.SelectionForeColor = Color.Yellow;
                                if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                    dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Value = "桥接字模";
                            }
                        }
                        else
                        {
                            dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Value = "---";
                            dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.BackColor = Color.White;//Lime
                            dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.SelectionBackColor = Color.White;//Lime
                            dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.ForeColor = Color.Black;
                            dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Style.SelectionForeColor = Color.Black;
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_MarkResult1.Rows[i].Cells[intJointMarkColumn].Value = "---";
                        }
                    }
                    else
                    {
                        dgd_MarkResult1.Columns[intJointMarkColumn].Visible = false;

                    }

                    if (fScoreValue >= 0 && blnEnableMark && m_smVisionInfo.g_arrMarks[u].ref_blnCheckMark)
                    {
                        dgd_MarkResult1.Rows[i].Cells[intColumn].Value = fScoreValue.ToString("F2");

                        //if (m_smVisionInfo.g_arrMarks[u].GetCharResult(i))
                        if (Convert.ToDouble(dgd_MarkResult1.Rows[i].Cells[1].Value) <= Convert.ToDouble(dgd_MarkResult1.Rows[i].Cells[intColumn].Value))
                        {
                            dgd_MarkResult1.Rows[i].Cells[intColumn].Style.BackColor = Color.Lime;
                            dgd_MarkResult1.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult1.Rows[i].Cells[intColumn].Style.ForeColor = Color.Black;
                            dgd_MarkResult1.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult1.Rows[i].Cells[intColumn].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intColumn].Style.ForeColor = Color.Black;
                                dgd_MarkResult1.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkResult1.Rows[i].Cells[0].Style.ForeColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[0].Style.SelectionForeColor = Color.Red;
                                if (dgd_MarkResult1.Columns[intColumn].Visible)
                                    m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult1.Rows[i].Cells[intColumn].Style.BackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intColumn].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult1.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                    }
                    else
                    {
                        dgd_MarkResult1.Rows[i].Cells[intColumn].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[intColumn].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intColumn].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intColumn].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[intColumn].Style.SelectionForeColor = Color.Black;
                    }

                    if (fExcessValue >= 0 && blnEnableMark && m_smVisionInfo.g_arrMarks[u].ref_blnCheckMark && m_smVisionInfo.g_arrMarks[u].CheckWantInspectExcessMark(m_smVisionInfo.g_arrMarks[u].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                    {
                        dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Value = fExcessValue.ToString(GetDecimalFormat());
                        float fCharMaxExcessAreaSetting = m_smVisionInfo.g_arrMarks[u].GetCharMaxExcessAreaSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

                        if (fCharMaxExcessAreaSetting >= fExcessValue)
                        {
                            dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.BackColor = Color.Lime;
                            dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.ForeColor = Color.Black;
                            dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.ForeColor = Color.Black;
                                dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkResult1.Rows[i].Cells[0].Style.ForeColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[0].Style.SelectionForeColor = Color.Red;
                                if (dgd_MarkResult1.Columns[intExcessColumn].Visible)
                                    m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.BackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                    }
                    else
                    {
                        dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[intExcessColumn].Style.SelectionForeColor = Color.Black;
                    }

                    //2021-03-05 ZJYEOH : If Excess fail, Missing will not check, so display ----
                    if (((m_smVisionInfo.g_arrMarks[u].ref_intFailResultMask & 0x01) == 0) && fBrokenValue >= 0 && blnEnableMark && m_smVisionInfo.g_arrMarks[u].ref_blnCheckMark && m_smVisionInfo.g_arrMarks[u].CheckWantInspectMissingMark(m_smVisionInfo.g_arrMarks[u].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                    {
                        dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Value = fBrokenValue.ToString(GetDecimalFormat());
                        float fCharMaxBrokenAreaSetting = m_smVisionInfo.g_arrMarks[u].GetCharMaxBrokenAreaSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

                        if (fCharMaxBrokenAreaSetting >= fBrokenValue)
                        {
                            dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.BackColor = Color.Lime;
                            dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.ForeColor = Color.Black;
                            dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.ForeColor = Color.Black;
                                dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkResult1.Rows[i].Cells[0].Style.ForeColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[0].Style.SelectionForeColor = Color.Red;
                                if (dgd_MarkResult1.Columns[intMissingColumn].Visible)
                                    m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.BackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                    }
                    else
                    {
                        dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[intMissingColumn].Style.SelectionForeColor = Color.Black;
                    }

                    if (!m_smVisionInfo.g_blnWantCheckMarkBroken || (m_smVisionInfo.g_arrMarks[u].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate) & 0x20) <= 0)
                    {
                        dgd_MarkResult1.Columns[8].Visible = false;
                        dgd_MarkResult1.Columns[9].Visible = false;
                    }
                    else
                    {
                        if (m_smVisionInfo.g_intUnitsOnImage > 1)
                            dgd_MarkResult1.Columns[8].Visible = true;
                        dgd_MarkResult1.Columns[9].Visible = true;
                    }

                    bool blnCharMaxBrokenAreaSetting = m_smVisionInfo.g_arrMarks[u].GetCharWantBrokenMarkSetting(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

                    if (m_smVisionInfo.g_blnWantCheckMarkBroken && (m_smVisionInfo.g_arrMarks[u].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate) & 0x20) > 0 && m_smVisionInfo.g_blnMarkInspected
                        && blnEnableMark && m_smVisionInfo.g_arrMarks[u].CheckWantInspectBrokenMark(m_smVisionInfo.g_arrMarks[u].GetMarktype(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate)))
                    {
                        if (blnCharMaxBrokenAreaSetting)
                        {
                            if (m_smVisionInfo.g_arrMarks[u].ref_blnCharResult == null ||
                                m_smVisionInfo.g_arrMarks[u].ref_blnCharResult[m_smVisionInfo.g_intSelectedTemplate] == null ||
                                m_smVisionInfo.g_arrMarks[u].ref_blnCharResult[m_smVisionInfo.g_intSelectedTemplate].Length == 0)
                                break; //2020-10-25 ZJYEOH: Need to break when ref_blnCharResult[m_smVisionInfo.g_intSelectedTemplate].Length is 0 because we will always reset to zero when start inspection

                            //if (!m_smVisionInfo.g_arrMarks[u].ref_blnCharResult[m_smVisionInfo.g_intSelectedTemplate][i])
                            if (!m_smVisionInfo.g_arrMarks[u].GetCharBrokenMark(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate))
                            {
                                if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                                {
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.LimeGreen;
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.LimeGreen;
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Black;
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    dgd_MarkResult1.Rows[i].Cells[0].Style.ForeColor = Color.Red;
                                    dgd_MarkResult1.Rows[i].Cells[0].Style.SelectionForeColor = Color.Red;
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Value = "Fail";
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.Red;
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Yellow;
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.SelectionForeColor = Color.Yellow;
                                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                        dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Value = "不合格";
                                }
                            }
                            else
                            {
                                dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Value = "Pass";
                                dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.Lime;
                                dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Black;
                                dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.SelectionForeColor = Color.Black;
                                if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                    dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Value = "合格";
                            }
                        }
                        else
                        {
                            dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Value = "----";
                            dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.White;//Lime
                            dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.White;//Lime
                            dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Black;
                            dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[intBrolenColumn].Style.ForeColor = Color.Black;

                    }

                    if (m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue && ((m_smVisionInfo.g_arrMarks[u].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate) & 0x200) > 0) &&
                        blnEnableMark && m_smVisionInfo.g_arrMarks[u].ref_blnCheckMark && fAGVValue != -999)
                    {
                        dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Value = fAGVValue.ToString("F2");

                        if (m_smVisionInfo.g_arrMarks[u].GetMaxAGVPercent(i, m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate) >= Math.Abs(Convert.ToDouble(dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Value)))
                        {
                            dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.Lime;
                            dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.ForeColor = Color.Black;
                            dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.ForeColor = Color.Black;
                                dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_MarkResult1.Rows[i].Cells[0].Style.ForeColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[0].Style.SelectionForeColor = Color.Red;
                                if (dgd_MarkResult1.Columns[intAGVColumn].Visible)
                                    m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                    }
                    else
                    {
                        dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Value = "----";
                        dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.BackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.SelectionBackColor = Color.White;//Lime
                        dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.ForeColor = Color.Black;
                        dgd_MarkResult1.Rows[i].Cells[intAGVColumn].Style.SelectionForeColor = Color.Black;
                    }

                }
                if (((m_smCustomizeInfo.g_intWant2DCode & (0x01 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_blnWantMark2DCode)
                {
                    lbl_Mark2DCodeResult.Text = m_smVisionInfo.g_arrMarks[u].Get2DCodeResult();
                    if (lbl_Mark2DCodeResult.Text == "-----")
                    {
                        lbl_Mark2DCodeResult.BackColor = Color.Red;
                        m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                    }
                    else
                        lbl_Mark2DCodeResult.BackColor = Color.Lime;
                }
            }
            }    

            //Orient table
            dgd_OrientResult.Rows.Clear();
            dgd_OrientResult.Rows.Add();
            dgd_OrientResult.Rows[0].Cells[0].Value = "Orient";
            dgd_OrientResult.Rows[0].Cells[3].Value = "----";
            dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.White;//Lime
            dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.White;//Lime
            dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Black;
            dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;

            if (m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                dgd_OrientResult.Rows[0].Visible = false;

            if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                dgd_OrientResult.Rows.Add();
                dgd_OrientResult.Rows[1].Cells[0].Value = "Angle";
                dgd_OrientResult.Rows[1].Cells[3].Value = "----";
                dgd_OrientResult.Rows[1].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[1].Cells[3].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionForeColor = Color.Black;
                dgd_OrientResult.Rows.Add();
                dgd_OrientResult.Rows[2].Cells[0].Value = "X Tol.(mm)";
                dgd_OrientResult.Rows[2].Cells[3].Value = "----";
                dgd_OrientResult.Rows[2].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[2].Cells[3].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionForeColor = Color.Black;
                dgd_OrientResult.Rows.Add();
                dgd_OrientResult.Rows[3].Cells[0].Value = "Y Tol.(mm)";
                dgd_OrientResult.Rows[3].Cells[3].Value = "----";
                dgd_OrientResult.Rows[3].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[3].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[3].Cells[3].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[3].Cells[3].Style.SelectionForeColor = Color.Black;
            }
            float fScore = -1;
            if (m_smVisionInfo.g_blnUnitInspected[0])
            {
                if (m_smVisionInfo.g_intSelectedTemplate < m_smVisionInfo.g_arrOrients[0].Count)
                    fScore = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetMinScore() * 100;
            }

            if (fScore >= 0)
            {
                dgd_OrientResult.Rows[0].Cells[3].Value = fScore.ToString("f2");

                if (Convert.ToDouble(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fMinScore * 100) <= Convert.ToDouble(dgd_OrientResult.Rows[0].Cells[3].Value))
                {
                    dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                    dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                    if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        int intOrientAngle = 0;
                        if (m_smVisionInfo.g_intOrientResult[1] < 4)
                        {
                            switch (m_smVisionInfo.g_intOrientResult[1])
                            {
                                default:
                                case 0:
                                    intOrientAngle = 0;
                                    break;
                                case 1:
                                    intOrientAngle = 90;
                                    break;
                                case 2:
                                    intOrientAngle = 180;
                                    break;
                                case 3:
                                    intOrientAngle = -90;
                                    break;
                            }

                        }
                        //    float CenterX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;// (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;
                        //    float CenterY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2;
                        //                                                                                                                                          //2020-09-24 ZJYEOH : Should use current angle to rotate template center point because when get center point different, the object center point is based on current angle
                        //    float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - CenterX) * Math.Cos(intOrientAngle * Math.PI / 180)) - //m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_intRotatedAngle 
                        //((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - CenterY) * Math.Sin(intOrientAngle * Math.PI / 180)));
                        //    float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX - CenterX) * Math.Sin(intOrientAngle * Math.PI / 180)) +
                        //      ((m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY - CenterY) * Math.Cos(intOrientAngle * Math.PI / 180)));

                        float fXAfterRotated = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateX;
                        float fYAfterRotated = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fTemplateY;

                        float fAngleResult = Math.Abs(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult);//GetResultAngle()
                        float fCenterXDiff = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].GetCenterXDiff(fXAfterRotated, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX);// m_smVisionInfo.g_fOrientCenterX[0];
                        float fCenterYDiff = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].GetCenterYDiff(fYAfterRotated, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY);// m_smVisionInfo.g_fOrientCenterY[0];

                        dgd_OrientResult.Rows[1].Cells[0].Value = "Angle";
                        dgd_OrientResult.Rows[1].Cells[3].Value = fAngleResult.ToString("f4");
                        if (m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fAngleTolerance <= fAngleResult)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_OrientResult.Rows[1].Cells[3].Style.BackColor = Color.LimeGreen;
                                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_OrientResult.Rows[1].Cells[3].Style.ForeColor = Color.Black;
                                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_OrientResult.Rows[1].Cells[3].Style.BackColor = Color.Red;
                                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_OrientResult.Rows[1].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_OrientResult.Rows[1].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_OrientResult.Rows[1].Cells[3].Style.BackColor = Color.Lime;
                            dgd_OrientResult.Rows[1].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_OrientResult.Rows[1].Cells[3].Style.ForeColor = Color.Black;
                            dgd_OrientResult.Rows[1].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        dgd_OrientResult.Rows[2].Cells[0].Value = "X Tol.(mm)";
                        dgd_OrientResult.Rows[2].Cells[3].Value = fCenterXDiff.ToString("f4");
                        if (Math.Abs(fCenterXDiff) >= m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fXTolerance)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_OrientResult.Rows[2].Cells[3].Style.BackColor = Color.LimeGreen;
                                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_OrientResult.Rows[2].Cells[3].Style.ForeColor = Color.Black;
                                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_OrientResult.Rows[2].Cells[3].Style.BackColor = Color.Red;
                                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_OrientResult.Rows[2].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_OrientResult.Rows[2].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_OrientResult.Rows[2].Cells[3].Style.BackColor = Color.Lime;
                            dgd_OrientResult.Rows[2].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_OrientResult.Rows[2].Cells[3].Style.ForeColor = Color.Black;
                            dgd_OrientResult.Rows[2].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        dgd_OrientResult.Rows[3].Cells[0].Value = "Y Tol.(mm)";
                        dgd_OrientResult.Rows[3].Cells[3].Value = fCenterYDiff.ToString("f4");

                        if (Math.Abs(fCenterYDiff) >= m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fYTolerance)
                        {
                            m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                            dgd_OrientResult.Rows[3].Cells[3].Style.BackColor = Color.Red;
                            dgd_OrientResult.Rows[3].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_OrientResult.Rows[3].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_OrientResult.Rows[3].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_OrientResult.Rows[3].Cells[3].Style.BackColor = Color.Lime;
                            dgd_OrientResult.Rows[3].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_OrientResult.Rows[3].Cells[3].Style.ForeColor = Color.Black;
                            dgd_OrientResult.Rows[3].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }
                else
                {
                    if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                    {
                        dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.LimeGreen;
                        dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                        dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                        dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                        dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.Red;
                        dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                }
            }
            else
            {
                dgd_OrientResult.Rows[0].Cells[3].Value = "----";
                dgd_OrientResult.Rows[0].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
            }

            fScore = -1;
            if (m_smVisionInfo.g_blnUnitInspected[1] && m_smVisionInfo.g_intUnitsOnImage == 2)
            {
                if (m_smVisionInfo.g_intSelectedTemplate < m_smVisionInfo.g_arrOrients[1].Count)
                    fScore = m_smVisionInfo.g_arrOrients[1][m_smVisionInfo.g_intSelectedTemplate].GetMinScore() * 100;
            }

            if (fScore >= 0)
            {
                dgd_OrientResult.Rows[0].Cells[2].Value = fScore.ToString("f2");

                if (Convert.ToDouble(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_fMinScore * 100) <= Convert.ToDouble(dgd_OrientResult.Rows[0].Cells[2].Value))
                {
                    dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                    dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                    dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                    {
                        dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.LimeGreen;
                        dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                        dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                        dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = true;
                        dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.Red;
                        dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                        dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;
                    }
                }
            }
            else
            {
                dgd_OrientResult.Rows[0].Cells[2].Value = "----";
                dgd_OrientResult.Rows[0].Cells[2].Style.BackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                dgd_OrientResult.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_OrientResult.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
            }

            //Pin 1 table
            if (m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate))
            {
                dgd_Pin1Result.Rows.Clear();
                dgd_Pin1Result.Rows.Add();
                dgd_Pin1Result.Rows[0].Cells[0].Value = "Pin 1";
                dgd_Pin1Result.Rows[0].Cells[3].Value = "----";
                dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.White;//Lime
                dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;

                fScore = -1;
                if (m_smVisionInfo.g_blnUnitInspected[0])
                    fScore = m_smVisionInfo.g_arrPin1[0].GetResultScore(m_smVisionInfo.g_intSelectedTemplate) * 100;

                if (fScore >= 0)
                {
                    dgd_Pin1Result.Rows[0].Cells[3].Value = fScore.ToString("f2");

                    if (Convert.ToDouble(m_smVisionInfo.g_arrPin1[0].GetMinScoreSetting(m_smVisionInfo.g_intSelectedTemplate) * 100) <= Convert.ToDouble(dgd_Pin1Result.Rows[0].Cells[3].Value))
                    {
                        dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.Lime;
                        dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.Lime;
                        dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                        {
                            dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.LimeGreen;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.LimeGreen;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            m_blnFailPin1[m_smVisionInfo.g_intSelectedTemplate] = true;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.Red;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                    }
                }
                else
                {
                    dgd_Pin1Result.Rows[0].Cells[3].Value = "----";
                    dgd_Pin1Result.Rows[0].Cells[3].Style.BackColor = Color.White;//Lime
                    dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionBackColor = Color.White;//Lime
                    dgd_Pin1Result.Rows[0].Cells[3].Style.ForeColor = Color.Black;
                    dgd_Pin1Result.Rows[0].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                fScore = -1;
                if (m_smVisionInfo.g_blnUnitInspected[1] && m_smVisionInfo.g_intUnitsOnImage == 2)
                    fScore = m_smVisionInfo.g_arrPin1[1].GetResultScore(m_smVisionInfo.g_intSelectedTemplate) * 100;

                if (fScore >= 0)
                {
                    dgd_Pin1Result.Rows[0].Cells[2].Value = fScore.ToString("f2");

                    if (Convert.ToDouble(m_smVisionInfo.g_arrPin1[1].GetMinScoreSetting(m_smVisionInfo.g_intSelectedTemplate) * 100) <= Convert.ToDouble(dgd_Pin1Result.Rows[0].Cells[2].Value))
                    {
                        dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                        dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                        dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                        dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                    }
                    else
                    {
                        if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                        {
                            dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.LimeGreen;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            m_blnFailPin1[m_smVisionInfo.g_intSelectedTemplate] = true;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.Red;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                            dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;
                        }
                    }
                }
                else
                {
                    dgd_Pin1Result.Rows[0].Cells[2].Value = "----";
                    dgd_Pin1Result.Rows[0].Cells[2].Style.BackColor = Color.White;//Lime
                    dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionBackColor = Color.White;//Lime
                    dgd_Pin1Result.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                    dgd_Pin1Result.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;
                }
            }
            
            
            if (((m_smCustomizeInfo.g_intWantOCR2 & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && (m_smVisionInfo.g_blnUseOCR || m_smVisionInfo.g_blnUseOCRandOCV))
            {
                dgd_OCRResult.Rows.Clear();
                int counter = 0;
                int counter2 = 0;
                char[] s = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCR2GetMessage(0).ToCharArray();
                string[] split;

                //OCR2 Inspection
                List<int> s1 = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetOCRResult();

                if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCR2GetMessage(0).Contains("\n"))
                {
                    split = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCR2GetMessage(0).Split('\n');
                    txt_StringText.Text = "";

                    for (int i = 0; i < split.Length; i++)
                    {
                        txt_StringText.Text += split[i] + "\r\n";
                    }
                }

                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '\n' || s[i] == ' ')
                    {
                        counter++;
                        continue;
                    }
                    else
                    {
                        dgd_OCRResult.Rows.Add();
                        dgd_OCRResult.Rows[counter2].Cells[5].Style.BackColor = Color.White;//Lime
                        dgd_OCRResult.Rows[counter2].Cells[5].Style.SelectionBackColor = Color.White;//Lime
                        dgd_OCRResult.Rows[counter2].Cells[5].Style.ForeColor = Color.Black;
                        dgd_OCRResult.Rows[counter2].Cells[5].Style.SelectionForeColor = Color.Black;
                        dgd_OCRResult.Rows[counter2].Cells[1].Value = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetOCRCharSetting(counter2).ToString();
                        dgd_OCRResult.Rows[counter2].Cells[0].Value = "OCR " + (counter2 + 1);
                        dgd_OCRResult.Rows[counter2].Cells[3].Value = s[counter].ToString();

                        if (s1.Count > counter2 && s1[counter2] >= 0 && m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_blnCheckMark && m_smVisionInfo.g_blnMarkInspected)
                        {
                            dgd_OCRResult.Rows[counter2].Cells[5].Value = s1[counter2];

                            if (Convert.ToDouble(dgd_OCRResult.Rows[counter2].Cells[1].Value) <= Convert.ToDouble(dgd_OCRResult.Rows[counter2].Cells[5].Value))
                            {
                                dgd_OCRResult.Rows[counter2].Cells[5].Style.BackColor = Color.Lime;
                                dgd_OCRResult.Rows[counter2].Cells[5].Style.SelectionBackColor = Color.Lime;
                                dgd_OCRResult.Rows[counter2].Cells[5].Style.ForeColor = Color.Black;
                                dgd_OCRResult.Rows[counter2].Cells[5].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                dgd_OCRResult.Rows[counter2].Cells[0].Style.ForeColor = Color.Red;
                                dgd_OCRResult.Rows[counter2].Cells[0].Style.SelectionForeColor = Color.Red;
                                dgd_OCRResult.Rows[counter2].Cells[3].Style.ForeColor = Color.Red;
                                dgd_OCRResult.Rows[counter2].Cells[3].Style.SelectionForeColor = Color.Red;
                                if (dgd_OCRResult.Columns[5].Visible)
                                    m_blnFailOCR[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_OCRResult.Rows[counter2].Cells[5].Style.BackColor = Color.Red;
                                dgd_OCRResult.Rows[counter2].Cells[5].Style.SelectionBackColor = Color.Red;
                                dgd_OCRResult.Rows[counter2].Cells[5].Style.ForeColor = Color.Yellow;
                                dgd_OCRResult.Rows[counter2].Cells[5].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_OCRResult.Rows[counter2].Cells[5].Value = "----";
                            dgd_OCRResult.Rows[counter2].Cells[5].Style.BackColor = Color.White;//Lime
                            dgd_OCRResult.Rows[counter2].Cells[5].Style.SelectionBackColor = Color.White;//Lime
                            dgd_OCRResult.Rows[counter2].Cells[5].Style.ForeColor = Color.Black;
                            dgd_OCRResult.Rows[counter2].Cells[5].Style.SelectionForeColor = Color.Black;
                            txt_StringText.Text = "";
                        }

                        counter2++;
                        counter++;
                    }
                }
            }
        }

        private void UpdateSelectedTemplateChange()
        {
            // 2020-06-01 ZJYEOH : Reset for those might have more than one template
            m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = false;
            m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate] = false;
            m_blnFailPin1[m_smVisionInfo.g_intSelectedTemplate] = false;


            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate))
                {
                    if (!tab_Result.TabPages.Contains(tp_Pin1))
                        tab_Result.TabPages.Add(tp_Pin1);
                }
                else
                {
                    if (tab_Result.TabPages.Contains(tp_Pin1))
                        tab_Result.TabPages.Remove(tp_Pin1);
                }
            }
            else
            {
                if (tab_Result.TabPages.Contains(tp_Pin1))
                    tab_Result.TabPages.Remove(tp_Pin1);
            }

            //2020-06-01 ZJYEOH : Need to init the GUI for dgd_MarkResult2
            InitMarkResult2GUI();

            UpdateAllResultTable();
            UpdateMarkResult2Table(false);

            UpdateTabPageHeaderImage();
        }

        private void UpdatePackageResultTable()
        {
            dgd_Defect.Rows.Clear();
            m_smVisionInfo.g_blnViewSelectedBlobObject = false;
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (i >= m_smVisionInfo.g_arrPackageROIs.Count)
                    continue;

                if (m_smVisionInfo.g_arrPackageROIs[i].Count < 2)
                    continue;

                m_smVisionInfo.g_arrPackageROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                m_smVisionInfo.g_arrPackageROIs[i][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[i][0]);

                Package objPackage = m_smVisionInfo.g_arrPackage[i];
                if (((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) == 0 && !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantInspectPackage()) || (((m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || m_smVisionInfo.g_strVisionName.Contains("BottomOrient")) && (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && !m_smVisionInfo.g_blnOrientWantPackage))
                {
                    dgd_Package.Rows.Clear();

                    if (tab_Result.Contains(tp_Package))
                    {
                        tab_Result.Controls.Remove(tp_Package);
                        return;
                    }
                }
                else
                {
                    if (!tab_Result.Contains(tp_Package))
                        tab_Result.Controls.Add(tp_Package);

                    if (i >= dgd_Package.Rows.Count)
                    {
                        dgd_Package.Rows.Add();
                    }

                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        dgd_Package.Rows[i].Cells[0].Value = "Result";
                    else
                    {
                        if (i == 0)
                            dgd_Package.Rows[i].Cells[0].Value = "Test";
                        else
                            dgd_Package.Rows[i].Cells[0].Value = "Retest";
                    }

                    dgd_Package.Rows[i].Cells[0].Style.BackColor = Color.White;
                    dgd_Package.Rows[i].Cells[0].Style.SelectionBackColor = Color.White;
                    dgd_Package.Rows[i].Cells[0].Style.ForeColor = Color.Black;
                    dgd_Package.Rows[i].Cells[0].Style.SelectionForeColor = Color.Black;

                    dgd_Package.Rows[i].Cells[1].Value = "";

                    if (m_smVisionInfo.g_arrPackage[i].ref_blnPkgSizeInspectionDone)
                    {
                        if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) > 0)
                        {
                            dgd_Package.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPackage[i].GetResultWidth(1).ToString("f4");
                            dgd_Package.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPackage[i].GetResultHeight(1).ToString("f4");

                            if (m_smVisionInfo.g_arrPackage[i].GetResultWidth(1) > m_smVisionInfo.g_arrPackage[i].GetUnitWidthMax(1) ||
                                m_smVisionInfo.g_arrPackage[i].GetResultWidth(1) < m_smVisionInfo.g_arrPackage[i].GetUnitWidthMin(1))
                            {
                                m_blnFailPackage = true;
                                dgd_Package.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_Package.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_Package.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_Package.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_Package.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_Package.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_Package.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_Package.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }

                            if (m_smVisionInfo.g_arrPackage[i].GetResultHeight(1) > m_smVisionInfo.g_arrPackage[i].GetUnitHeightMax(1) ||
                                m_smVisionInfo.g_arrPackage[i].GetResultHeight(1) < m_smVisionInfo.g_arrPackage[i].GetUnitHeightMin(1))
                            {
                                m_blnFailPackage = true;
                                dgd_Package.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_Package.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_Package.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_Package.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_Package.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_Package.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_Package.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_Package.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        else
                        {
                            dgd_Package.Rows[i].Cells[2].Value = "---";
                            dgd_Package.Rows[i].Cells[3].Value = "---";
                            //m_blnFailPackage = true;
                            dgd_Package.Rows[i].Cells[2].Style.BackColor = Color.White;// Lime
                            dgd_Package.Rows[i].Cells[2].Style.SelectionBackColor = Color.White;// Lime
                            dgd_Package.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_Package.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            dgd_Package.Rows[i].Cells[3].Style.BackColor = Color.White;// Lime
                            dgd_Package.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;// Lime
                            dgd_Package.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Package.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        if ((m_smVisionInfo.g_arrPackage[i].ref_intFailMask & 0x2000) > 0)
                        {
                            if (!dgd_Package.Columns[4].Visible)
                                dgd_Package.Columns[4].Visible = true;
                            //2020-11-11 ZJYEOH : Change dgd_Package.RowCount - 1 to i so that both retest and test Angle result will be display
                            dgd_Package.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPackage[i].GetResultAngle().ToString("f4");

                            if (Math.Abs(m_smVisionInfo.g_arrPackage[i].GetResultAngle()) > m_smVisionInfo.g_arrPackage[i].GetUnitAngleMax())
                            {
                                m_blnFailPackage = true;
                                dgd_Package.Rows[i].Cells[4].Style.BackColor = Color.Red;
                                dgd_Package.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                                dgd_Package.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                                dgd_Package.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_Package.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                                dgd_Package.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                                dgd_Package.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                                dgd_Package.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        else
                        {
                            dgd_Package.Columns[4].Visible = false;
                        }
                    }
                    else
                    {
                        //dgd_Package.Rows[i].Cells[2].Value = "---";
                        //dgd_Package.Rows[i].Cells[3].Value = "---";
                        //m_blnFailPackage = true;
                        //dgd_Package.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        //dgd_Package.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                        //dgd_Package.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                        //dgd_Package.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                        //dgd_Package.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        //dgd_Package.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        //dgd_Package.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        //dgd_Package.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;

                        dgd_Package.Rows[i].Cells[2].Value = "---";
                        dgd_Package.Rows[i].Cells[3].Value = "---";
                        //m_blnFailPackage = true;
                        dgd_Package.Rows[i].Cells[2].Style.BackColor = Color.White;// Lime
                        dgd_Package.Rows[i].Cells[2].Style.SelectionBackColor = Color.White;// Lime
                        dgd_Package.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                        dgd_Package.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        dgd_Package.Rows[i].Cells[3].Style.BackColor = Color.White;// Lime
                        dgd_Package.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;// Lime
                        dgd_Package.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Package.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;

                        if ((m_smVisionInfo.g_arrPackage[i].ref_intFailMask & 0x2000) > 0)
                        {
                            dgd_Package.Rows[i].Cells[4].Value = "---";
                            dgd_Package.Rows[i].Cells[4].Style.BackColor = Color.White;// Lime
                            dgd_Package.Rows[i].Cells[4].Style.SelectionBackColor = Color.White;// Lime
                            dgd_Package.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                            dgd_Package.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_Package.Columns[4].Visible = false;
                        }
                    }
                }

                bool blnPackageDefectFail = false;
                for (int j = 0; j < objPackage.ref_arrDefectList.Count; j++)
                {
                    dgd_Defect.Rows.Add();
                    int intRow = dgd_Defect.RowCount - 1;
                    // dgd_Defect.Rows[intRow].Cells[0].Value = Convert.ToString(i + 1);

                    int intFailedImage = 0;
                    float fWidth = 0.0f, fHeight = 0.0f, fArea = 0.0f, fAngle = 0.0f;
                    int intFailMask = 0;
                    string strDefectName = "";
                    objPackage.GetDefectInfo(j, ref fWidth, ref fHeight, ref fArea, ref intFailMask, ref strDefectName, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY, ref intFailedImage, ref fAngle);
                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        dgd_Defect.Rows[intRow].Cells[0].Value = Convert.ToString(intFailedImage.ToString());
                    else if (m_smVisionInfo.g_intUnitsOnImage == 2)
                        dgd_Defect.Rows[intRow].Cells[0].Value = Convert.ToString(i + 1 + "_" + intFailedImage.ToString());

                    // 2019 03 29 - CCENG: g_intSelectedImage value will only be reassigned if detected defect is fail (intFailMask > 0)
                    if (m_smVisionInfo.PR_MN_TestDone && m_smVisionInfo.AT_VM_ManualTestMode && intFailMask != 0x00)
                    {
                        if (m_smVisionInfo.g_intSelectedImage != (intFailedImage - 1))
                        {
                            m_smVisionInfo.g_intSelectedImage = intFailedImage - 1;
                            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
                        }
                    }

                    dgd_Defect.Rows[intRow].Cells[1].Value = strDefectName;  //Convert.ToString(j + 1);

                    if (intFailMask > 0)
                    {
                        blnPackageDefectFail = true;
                        m_blnFailPackage = true;
                        dgd_Defect.Rows[intRow].Cells[1].Style.BackColor = Color.Red;
                        dgd_Defect.Rows[intRow].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_Defect.Rows[intRow].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_Defect.Rows[intRow].Cells[1].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Defect.Rows[intRow].Cells[1].Style.BackColor = Color.Lime;
                        dgd_Defect.Rows[intRow].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_Defect.Rows[intRow].Cells[1].Style.ForeColor = Color.Black;
                        dgd_Defect.Rows[intRow].Cells[1].Style.SelectionForeColor = Color.Black;
                    }

                    if (fWidth < 0)
                        dgd_Defect.Rows[intRow].Cells[2].Value = "---";
                    else
                        dgd_Defect.Rows[intRow].Cells[2].Value = fWidth.ToString("f4");
                    if ((intFailMask & 0x01) > 0)
                    {
                        blnPackageDefectFail = true;
                        m_blnFailPackage = true;
                        dgd_Defect.Rows[intRow].Cells[2].Style.BackColor = Color.Red;
                        dgd_Defect.Rows[intRow].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_Defect.Rows[intRow].Cells[2].Style.ForeColor = Color.Yellow;
                        dgd_Defect.Rows[intRow].Cells[2].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Defect.Rows[intRow].Cells[2].Style.BackColor = GetColor(dgd_Defect.Rows[intRow].Cells[2].Value.ToString());
                        dgd_Defect.Rows[intRow].Cells[2].Style.SelectionBackColor = GetColor(dgd_Defect.Rows[intRow].Cells[2].Value.ToString());
                        dgd_Defect.Rows[intRow].Cells[2].Style.ForeColor = Color.Black;
                        dgd_Defect.Rows[intRow].Cells[2].Style.SelectionForeColor = Color.Black;
                    }

                    if (fHeight < 0)
                        dgd_Defect.Rows[intRow].Cells[3].Value = "---";
                    else
                        dgd_Defect.Rows[intRow].Cells[3].Value = fHeight.ToString("f4");
                    if ((intFailMask & 0x02) > 0)
                    {
                        blnPackageDefectFail = true;
                        m_blnFailPackage = true;
                        dgd_Defect.Rows[intRow].Cells[3].Style.BackColor = Color.Red;
                        dgd_Defect.Rows[intRow].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_Defect.Rows[intRow].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_Defect.Rows[intRow].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Defect.Rows[intRow].Cells[3].Style.BackColor = GetColor(dgd_Defect.Rows[intRow].Cells[3].Value.ToString());
                        dgd_Defect.Rows[intRow].Cells[3].Style.SelectionBackColor = GetColor(dgd_Defect.Rows[intRow].Cells[3].Value.ToString());
                        dgd_Defect.Rows[intRow].Cells[3].Style.ForeColor = Color.Black;
                        dgd_Defect.Rows[intRow].Cells[3].Style.SelectionForeColor = Color.Black;
                    }

                    dgd_Defect.Rows[intRow].Cells[4].Value = fArea.ToString("f4");
                    if ((intFailMask & 0x04) > 0)
                    {
                        blnPackageDefectFail = true;
                        m_blnFailPackage = true;
                        dgd_Defect.Rows[intRow].Cells[4].Style.BackColor = Color.Red;
                        dgd_Defect.Rows[intRow].Cells[4].Style.SelectionBackColor = Color.Red;
                        dgd_Defect.Rows[intRow].Cells[4].Style.ForeColor = Color.Yellow;
                        dgd_Defect.Rows[intRow].Cells[4].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_Defect.Rows[intRow].Cells[4].Style.BackColor = Color.Lime;
                        dgd_Defect.Rows[intRow].Cells[4].Style.SelectionBackColor = Color.Lime;
                        dgd_Defect.Rows[intRow].Cells[4].Style.ForeColor = Color.Black;
                        dgd_Defect.Rows[intRow].Cells[4].Style.SelectionForeColor = Color.Black;
                    }

                }

                if (((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && !blnPackageDefectFail)
                {
                    //dgd_Defect.Rows.Clear();
                    List<List<string>> arrDefectList = m_smVisionInfo.g_arrPackage[i].GetColorDefectList();
                    for (int j = 0; j < arrDefectList.Count; j++)
                    {
                        dgd_Defect.Rows.Add();
                        int intRow = dgd_Defect.RowCount - 1;


                        int intFailedImage = 0;
                        float fWidth = 0.0f, fHeight = 0.0f, fArea = 0.0f, fAngle = 0.0f;
                        int intFailMask = 0;
                        string strDefectName = "";
                        intFailMask = Convert.ToInt32(arrDefectList[j][0]);
                        strDefectName = arrDefectList[j][1];
                        fWidth = (float)Convert.ToDouble(arrDefectList[j][2]);
                        fHeight = (float)Convert.ToDouble(arrDefectList[j][3]);
                        fArea = (float)Convert.ToDouble(arrDefectList[j][4]);
                        intFailedImage = Convert.ToInt32(arrDefectList[j][5]);
                        // 2019 03 29 - CCENG: g_intSelectedImage value will only be reassigned if detected defect is fail (intFailMask > 0)
                        if (m_smVisionInfo.PR_MN_TestDone && m_smVisionInfo.AT_VM_ManualTestMode && intFailMask != 0x00)
                        {
                            if (m_smVisionInfo.g_intSelectedImage != (intFailedImage - 1))
                            {
                                m_smVisionInfo.g_intSelectedImage = intFailedImage - 1;
                                m_smVisionInfo.g_blnUpdateImageNoComboBox = true;
                            }
                        }

                        dgd_Defect.Rows[intRow].Cells[0].Value = Convert.ToString(intFailedImage.ToString()); // dgd_Defect.Rows[intRow].Cells[0].Value = Convert.ToString(i + 1+"_"+intFailedImage.ToString());
                        dgd_Defect.Rows[intRow].Cells[1].Value = strDefectName;  //Convert.ToString(j + 1);

                        if (intFailMask > 0)
                        {
                            m_blnFailPackage = true;
                            dgd_Defect.Rows[intRow].Cells[1].Style.BackColor = Color.Red;
                            dgd_Defect.Rows[intRow].Cells[1].Style.SelectionBackColor = Color.Red;
                            dgd_Defect.Rows[intRow].Cells[1].Style.ForeColor = Color.Yellow;
                            dgd_Defect.Rows[intRow].Cells[1].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Defect.Rows[intRow].Cells[1].Style.BackColor = Color.Lime;
                            dgd_Defect.Rows[intRow].Cells[1].Style.SelectionBackColor = Color.Lime;
                            dgd_Defect.Rows[intRow].Cells[1].Style.ForeColor = Color.Black;
                            dgd_Defect.Rows[intRow].Cells[1].Style.SelectionForeColor = Color.Black;
                        }

                        if (fWidth < 0)
                            dgd_Defect.Rows[intRow].Cells[2].Value = "---";
                        else
                            dgd_Defect.Rows[intRow].Cells[2].Value = fWidth.ToString("f4");
                        if ((intFailMask & 0x01) > 0)
                        {
                            m_blnFailPackage = true;
                            dgd_Defect.Rows[intRow].Cells[2].Style.BackColor = Color.Red;
                            dgd_Defect.Rows[intRow].Cells[2].Style.SelectionBackColor = Color.Red;
                            dgd_Defect.Rows[intRow].Cells[2].Style.ForeColor = Color.Yellow;
                            dgd_Defect.Rows[intRow].Cells[2].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Defect.Rows[intRow].Cells[2].Style.BackColor = Color.Lime;
                            dgd_Defect.Rows[intRow].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_Defect.Rows[intRow].Cells[2].Style.ForeColor = Color.Black;
                            dgd_Defect.Rows[intRow].Cells[2].Style.SelectionForeColor = Color.Black;
                        }

                        if (fHeight < 0)
                            dgd_Defect.Rows[intRow].Cells[3].Value = "---";
                        else
                            dgd_Defect.Rows[intRow].Cells[3].Value = fHeight.ToString("f4");
                        if ((intFailMask & 0x02) > 0)
                        {
                            m_blnFailPackage = true;
                            dgd_Defect.Rows[intRow].Cells[3].Style.BackColor = Color.Red;
                            dgd_Defect.Rows[intRow].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_Defect.Rows[intRow].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_Defect.Rows[intRow].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Defect.Rows[intRow].Cells[3].Style.BackColor = Color.Lime;
                            dgd_Defect.Rows[intRow].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_Defect.Rows[intRow].Cells[3].Style.ForeColor = Color.Black;
                            dgd_Defect.Rows[intRow].Cells[3].Style.SelectionForeColor = Color.Black;
                        }

                        dgd_Defect.Rows[intRow].Cells[4].Value = fArea.ToString("f5");
                        if (((intFailMask & 0x04) > 0) || ((intFailMask & 0x08) > 0))
                        {
                            m_blnFailPackage = true;
                            dgd_Defect.Rows[intRow].Cells[4].Style.BackColor = Color.Red;
                            dgd_Defect.Rows[intRow].Cells[4].Style.SelectionBackColor = Color.Red;
                            dgd_Defect.Rows[intRow].Cells[4].Style.ForeColor = Color.Yellow;
                            dgd_Defect.Rows[intRow].Cells[4].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_Defect.Rows[intRow].Cells[4].Style.BackColor = Color.Lime;
                            dgd_Defect.Rows[intRow].Cells[4].Style.SelectionBackColor = Color.Lime;
                            dgd_Defect.Rows[intRow].Cells[4].Style.ForeColor = Color.Black;
                            dgd_Defect.Rows[intRow].Cells[4].Style.SelectionForeColor = Color.Black;
                        }

                    }
                }

                if (dgd_Defect.RowCount > 0)
                    m_smVisionInfo.g_intSelectedBlobNo = 0;
                else
                    m_smVisionInfo.g_intSelectedBlobNo = -1;
               
            }
        }

        private void UpdateLeadResultTable(DataGridView dgd_LeadSetting)
        {
            List<string> arrResultList = new List<string>();
            int intFailMask = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLead.Length; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                    continue;

                int intBlobID = m_smVisionInfo.g_arrLead[j].GetBlobsNoID();
                int intBlobCount = m_smVisionInfo.g_arrLead[j].GetBlobsFeaturesNumber();

                for (int i = 0; i < intBlobCount; i++)
                {
                    if ((i + intBlobID - 1) < 0)
                    {
                        STTrackLog.WriteLine("Visoin1OfflinePage > UpdateLeadResultTable > (i + intBlobID - 1) out of rows index.");
                        STTrackLog.WriteLine("intBlobID = " + intBlobID.ToString());
                        STTrackLog.WriteLine("intBlobCount = " + intBlobCount.ToString());
                        STTrackLog.WriteLine("Grid Row Count = " + dgd_LeadSetting.Rows.Count);

                    }

                    if ((i + intBlobID - 1) < 0 || (i + intBlobID - 1) >= dgd_LeadSetting.Rows.Count)
                        continue;

                    arrResultList = m_smVisionInfo.g_arrLead[j].GetBlobFeaturesResult_WithPassFailIndicator(i);
                    intFailMask = Convert.ToInt32(arrResultList[arrResultList.Count - 1]);

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Value = arrResultList[0];
                    if ((intFailMask & 0x01) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[0].Style.SelectionForeColor = Color.Black;
                    }

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Value = arrResultList[1];
                    if ((intFailMask & 0x02) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[1].Style.SelectionForeColor = Color.Black;
                    }

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Value = arrResultList[2];
                    if ((intFailMask & 0x04) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[2].Style.SelectionForeColor = Color.Black;
                    }

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Value = arrResultList[3];
                    if ((intFailMask & 0x08) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[3].Style.SelectionForeColor = Color.Black;
                    }

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Value = arrResultList[4];
                    if ((intFailMask & 0x10) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[4].Style.SelectionForeColor = Color.Black;
                    }

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Value = arrResultList[5];
                    if ((intFailMask & 0x20) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[5].Style.SelectionForeColor = Color.Black;
                    }

                    //dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Value = arrResultList[6];
                    //if ((intFailMask & 0x40) > 0)
                    //{
                    //    m_blnFailLead = true;
                    //    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Style.BackColor = Color.Red;
                    //    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Style.SelectionBackColor = Color.Red;
                    //    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Style.ForeColor = Color.Yellow;
                    //    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Style.SelectionForeColor = Color.Yellow;
                    //}
                    //else
                    //{
                    //    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Value.ToString());
                    //    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Value.ToString());
                    //    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Style.ForeColor = Color.Black;
                    //    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[6].Style.SelectionForeColor = Color.Black;
                    //}

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Value = arrResultList[7];
                    if ((intFailMask & 0x200) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[7].Style.SelectionForeColor = Color.Black;
                    }

                    //Base Lead
                    arrResultList = m_smVisionInfo.g_arrLead[j].GetBlobFeaturesResult_WithPassFailIndicator_BaseLead(i);
                    int intFailMask_BaseLead = Convert.ToInt32(arrResultList[arrResultList.Count - 1]);

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Value = arrResultList[0];
                    if (((intFailMask_BaseLead & 0x01) > 0) || ((intFailMask_BaseLead & 0x04) > 0) || ((intFailMask_BaseLead & 0x08) > 0))
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[8].Style.SelectionForeColor = Color.Black;
                    }

                    dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Value = arrResultList[1];
                    if (((intFailMask_BaseLead & 0x02) > 0) || ((intFailMask_BaseLead & 0x04) > 0) || ((intFailMask_BaseLead & 0x08) > 0))
                    {
                        m_blnFailLead = true;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Style.BackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Style.SelectionBackColor = Color.Red;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Style.ForeColor = Color.Yellow;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Style.BackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Style.SelectionBackColor = GetColor(dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Value.ToString());
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Style.ForeColor = Color.Black;
                        dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[9].Style.SelectionForeColor = Color.Black;
                    }

                    //Missing Lead
                    if ((intFailMask & 0x400) > 0)
                    {
                        for (int k = 0; k < dgd_LeadSetting.ColumnCount; k++)
                        {
                            dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[k].Style.BackColor = Color.Red;
                            dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[k].Style.SelectionBackColor = Color.Red;
                            dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[k].Style.ForeColor = Color.Yellow;
                            dgd_LeadSetting.Rows[i + intBlobID - 1].Cells[k].Style.SelectionForeColor = Color.Yellow;
                        }
                    }

                }
            }

            if (arrResultList.Count > 8)
            {
                lbl_ResultMinSpan.Text = arrResultList[8];
                if ((intFailMask & 0x80) > 0)
                {
                    m_blnFailLead = true;
                    lbl_ResultMinSpan.ForeColor = Color.Red;
                }
                else
                {
                    lbl_ResultMinSpan.ForeColor = Color.Black;
                }
            }

            if (arrResultList.Count > 9)
            {
                lbl_ResultMaxSpan.Text = arrResultList[9];
                if ((intFailMask & 0x100) > 0)
                {
                    m_blnFailLead = true;
                    lbl_ResultMaxSpan.ForeColor = Color.Red;
                }
                else
                {
                    lbl_ResultMaxSpan.ForeColor = Color.Black;
                }
            }
            
        }

        //private void UpdateLeadDefectTable(int intLeadIndex, DataGridView dgd_DefectTable)
        //{
        //    dgd_DefectTable.Rows.Clear();

        //    List<List<string>> arrDefectList = m_smVisionInfo.g_arrLead[intLeadIndex].GetDefectList();
        //    for (int i = 0; i < arrDefectList.Count; i++)
        //    {
        //        dgd_DefectTable.Rows.Add();
        //        dgd_DefectTable.Rows[i].Cells[0].Value = (i + 1).ToString();

        //        int intFailMask = Convert.ToInt32(arrDefectList[i][0]);

        //        dgd_DefectTable.Rows[i].Cells[1].Value = arrDefectList[i][1];
        //        if (intFailMask > 0)
        //        {
        //            dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.Red;
        //            dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
        //            dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
        //            dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
        //        }
        //        else
        //        {
        //            dgd_DefectTable.Rows[i].Cells[1].Style.BackColor = Color.Lime;
        //            dgd_DefectTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
        //            dgd_DefectTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
        //            dgd_DefectTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
        //        }

        //        dgd_DefectTable.Rows[i].Cells[2].Value = arrDefectList[i][2];
        //        if ((intFailMask & 0x01) > 0)
        //        {
        //            dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.Red;
        //            dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
        //            dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
        //            dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
        //        }
        //        else
        //        {
        //            dgd_DefectTable.Rows[i].Cells[2].Style.BackColor = Color.Lime;
        //            dgd_DefectTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
        //            dgd_DefectTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
        //            dgd_DefectTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
        //        }

        //        dgd_DefectTable.Rows[i].Cells[3].Value = arrDefectList[i][3];
        //        if ((intFailMask & 0x02) > 0)
        //        {
        //            dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.Red;
        //            dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
        //            dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
        //            dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
        //        }
        //        else
        //        {
        //            dgd_DefectTable.Rows[i].Cells[3].Style.BackColor = Color.Lime;
        //            dgd_DefectTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
        //            dgd_DefectTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
        //            dgd_DefectTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
        //        }

        //        dgd_DefectTable.Rows[i].Cells[4].Value = arrDefectList[i][4];
        //        if ((intFailMask & 0x04) > 0)
        //        {
        //            dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.Red;
        //            dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
        //            dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
        //            dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
        //        }
        //        else
        //        {
        //            dgd_DefectTable.Rows[i].Cells[4].Style.BackColor = Color.Lime;
        //            dgd_DefectTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
        //            dgd_DefectTable.Rows[i].Cells[4].Style.ForeColor = Color.Black;
        //            dgd_DefectTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
        //        }
        //    }
        //}

        //Initialize MarkResult2
        private void InitMarkResult2GUI()
        {
            if (m_smVisionInfo.g_intSelectedTemplate < 0)
                return;

            int intSelectedUnit = GetBiggerTemplateUnitNo();

            dgd_MarkResult2.Rows.Clear();
            m_arrMarkResult2Items.Clear();

            int intFailMask = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

            int i = 0;

            // 2019 08 17 - CCENG: Change to Excess Mark for each char
            //if ((intFailMask & 0x01) > 0)   // 0x01=Excess Mark
            //{
            //    dgd_MarkResult2.Rows.Add();
            //    dgd_MarkResult2.Rows[i].Cells[0].Value = "Excess Area";
            //    dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetExcessMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
            //    dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
            //    dgd_MarkResult2.Rows[i].Cells[3].Value = "pix";
            //    i++;
            //}

            if ((intFailMask & 0x06) > 0)   // 0x02 = Extra Mark Center Area, 0x04=Extra Mark Side Area
            {
                m_arrMarkResult2Items.Add("Extra Area");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Extra Area");
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetExtraMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                dgd_MarkResult2.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, true);
                i++;
            }

            if ((intFailMask & 0x08) > 0)   // 0x08=Extra Mark Group Area
            {
                m_arrMarkResult2Items.Add("Total Extra Area");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Total Extra Area");
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetGroupExtraMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                dgd_MarkResult2.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, true);
                i++;
            }

            if (((intFailMask & 0x100) > 0) && m_smVisionInfo.g_blnWantCheckMarkTotalExcess)   // 0x08=Excess Mark Group Area
            {
                m_arrMarkResult2Items.Add("Total Excess Area");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Total Excess Area");
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetGroupExcessMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                dgd_MarkResult2.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, true);
                i++;
            }

            // 2019 08 17 - CCENG: Change to Broken Mark for each char
            //if ((intFailMask & 0x10) > 0)
            //{
            //    dgd_MarkResult2.Rows.Add();
            //    dgd_MarkResult2.Rows[i].Cells[0].Value = "Miss Area";
            //    dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetMissingMinArea(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
            //    dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
            //    dgd_MarkResult2.Rows[i].Cells[3].Value = "pix";
            //    i++;
            //}

            // 2019 07 19 - CCENG: No more Text Score because it is unnecessary.
            //dgd_MarkResult2.Rows.Add();
            //dgd_MarkResult2.Rows[i].Cells[0].Value = "Text Score";
            //dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetTextMinScore(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString();
            //dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
            //dgd_MarkResult2.Rows[i].Cells[3].Value = "%";
            //i++;

            if ((intFailMask & 0x40) > 0)
            {
                m_arrMarkResult2Items.Add("Top Edge Distance");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Top Edge Distance");
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetUnCheckAreaTop(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                dgd_MarkResult2.Rows[i].Cells[2].Value = "NA";
                dgd_MarkResult2.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, false);
                i++;

                m_arrMarkResult2Items.Add("Btm Edge Distance");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Btm Edge Distance"); 
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetUnCheckAreaBottom(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                dgd_MarkResult2.Rows[i].Cells[2].Value = "NA";
                dgd_MarkResult2.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, false);
                i++;

                m_arrMarkResult2Items.Add("Left Edge Distance");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Left Edge Distance");
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetUnCheckAreaLeft(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                dgd_MarkResult2.Rows[i].Cells[2].Value = "NA";
                dgd_MarkResult2.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, false);
                i++;

                m_arrMarkResult2Items.Add("Right Edge Distance");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Right Edge Distance");
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetUnCheckAreaRight(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                dgd_MarkResult2.Rows[i].Cells[2].Value = "NA";
                dgd_MarkResult2.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, false);
                i++;
            }

            if (m_smVisionInfo.g_blnWantCheckMarkAngle && (intFailMask & 0x2000) > 0)   // 0x2000 = Mark Angle  // 2021 02 23 - CCENG: Display angle result if advance setting want check angle ON
            {
                m_arrMarkResult2Items.Add("Mark Angle");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "Mark Angle");
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetMarkAngleTolerance(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate).ToString("f4");
                dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                dgd_MarkResult2.Rows[i].Cells[3].Value = "deg";
                i++;
            }

            if (m_smVisionInfo.g_blnWantCheckNoMark)
            {
                m_arrMarkResult2Items.Add("No Mark Area");
                dgd_MarkResult2.Rows.Add();
                dgd_MarkResult2.Rows[i].Cells[0].Value = LanguageLibrary.Convert(m_smCustomizeInfo.g_intLanguageCulture, "No Mark Area");
                dgd_MarkResult2.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetNoMarkMaximumBlobArea().ToString(GetDecimalFormat());
                dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                dgd_MarkResult2.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetDisplayUnitName(m_smCustomizeInfo.g_intMarkUnitDisplay, true);
                i++;
            }
        }

        //Update MarkResult2
        private void UpdateMarkResult2Table(bool blnReset)
        {
            int intSelectedUnit = GetBiggerTemplateUnitNo();
            for (int i = 0; i < dgd_MarkResult2.Rows.Count; i++)
            {
                if (i >= m_arrMarkResult2Items.Count)
                    break;

                //switch (dgd_MarkResult2.Rows[i].Cells[0].Value.ToString())
                switch (m_arrMarkResult2Items[i])
                {
                    case "Excess Area":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultBiggestExcessArea(m_smVisionInfo.g_intSelectedTemplate).ToString();
                        if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) > Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Total Excess Area":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultGroupExcessArea(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) > Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Extra Area":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultBiggestExtraArea(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) > Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Total Extra Area":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultGroupExtraArea(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                        if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) > Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Miss Area":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultBiggestMissingArea(m_smVisionInfo.g_intSelectedTemplate).ToString();
                        if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) > Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Char Shift Tolerance":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = "NA";
                        break;
                    case "Text Shift Tole X":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = "NA";
                        break;
                    case "Text Shift Tole Y":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = "NA";
                        break;
                    case "Text Score":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextMatchScore(m_smVisionInfo.g_intSelectedTemplate).ToString("F1");
                        if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) < Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Top Edge Distance":
                        if (m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextShiftedTop(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextShiftedTop(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) < Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                            {
                                if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                                {
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Btm Edge Distance":
                        if (m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextShiftedBottom(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextShiftedBottom(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) < Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                            {
                                if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                                {
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Left Edge Distance":
                        if (m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextShiftedLeft(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextShiftedLeft(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) < Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                            {
                                if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                                {
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Right Edge Distance":
                        if (m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextShiftedRight(m_smVisionInfo.g_intSelectedTemplate).ToString() == "-999")
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Value = "0";
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[intSelectedUnit].GetResultTextShiftedRight(m_smVisionInfo.g_intSelectedTemplate).ToString(GetDecimalFormat());
                            if (!blnReset && Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) < Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                            {
                                if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                                {
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                                }
                                else
                                {
                                    m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                    dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                                }
                            }
                            else
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Mark Angle":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_fOrientAngle[0].ToString("f4");
                        if (!blnReset && Math.Abs(Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value)) > Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value) && m_smVisionInfo.g_arrMarks[intSelectedUnit].ref_blnCheckMark)
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "No Mark Area":
                        dgd_MarkResult2.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrMarks[0].GetResultTotalBlobArea().ToString(GetDecimalFormat());
                        if ((float)Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[2].Value) < (float)Convert.ToSingle(dgd_MarkResult2.Rows[i].Cells[1].Value))
                        {
                            if (m_smVisionInfo.g_strResult == "Pass" || m_smVisionInfo.g_strResult == "Empty")
                            {
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.LimeGreen;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                            }
                            else
                            {
                                m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate] = true;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                                dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            dgd_MarkResult2.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                            dgd_MarkResult2.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                        }

                        break;

                }
            }
            
        }

        private int GetBiggerTemplateUnitNo()
        {
            if (m_smVisionInfo.g_intUnitsOnImage == 1)
                return 0;
            else
            {
                if (m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex >= m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex)
                    return 0;
                else
                    return 1;
            }
        }
        private void Inspect()
        {
            m_smVisionInfo.g_blnMarkInspected = false;
            m_smVisionInfo.g_blnMarkSelecting = false;
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.g_blnDrawPkgResult = false;
            m_smVisionInfo.g_blnViewOrientObject = false;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_intMarkTextSelectedNo[u] = -1;
                m_smVisionInfo.g_intMarkCharSelectedNo[u] = -1;
                m_smVisionInfo.g_blnUnitInspected[u] = false;
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }

        public void OnOffTimer(bool blnOn)
        {
            timer_MarkTestResult.Enabled = blnOn;
        }

        public bool GetTimerStatus()
        {
            return timer_MarkTestResult.Enabled;
        }

        public void CloseOfflinePage()
        {
            m_smVisionInfo.g_blnViewROI = m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.g_blnViewMarkInspection = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;

            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewThreshold = false;
            m_smVisionInfo.g_blnTemplateManualSelect = false;
            m_smVisionInfo.g_blnMarkInspected = false;
            m_smVisionInfo.g_blnLeadInspected = false;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                m_smVisionInfo.g_blnInspectAllTemplate = m_smVisionInfo.g_arrMarks[u].ref_blnInspectAllTemplate = true;

            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.g_blnDrawPkgResult = false;
            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;

            if (m_smVisionInfo.g_objPositioning != null)
                m_smVisionInfo.g_objPositioning.ref_bFinalResult = false;

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            m_smVisionInfo.VM_AT_OfflinePageView = false; // 2019 01 10 - CCENG: Dont put this event in Form_Closing, bcos pressing close button is just to hide the form, not close the form.

            if (m_smVisionInfo.g_arrPackage != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
                {
                    m_smVisionInfo.g_arrPackage[i].ref_blnViewUnitPosition = false;
                }
            }

            if (m_smVisionInfo.g_arrLead != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_blnViewLeadResultDrawing = false;
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            this.Hide();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            for (int u = 0; u < m_blnFailMark.Length; u++)
            {
                m_blnFailMark[u] = false;
                m_blnFailOrient[u] = false;
                m_blnFailPin1[u] = false;
                m_blnFailOCR[u] = false;
            }
            m_blnFailPackage = false;
            m_blnFailLead = false;
            UpdateTabPageHeaderImage();

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Offline Test Page Closed", "Exit Offline Test Page", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_blnViewROI = m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.g_blnViewMarkInspection = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;

            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewThreshold = false;
            m_smVisionInfo.g_blnTemplateManualSelect = false;
            m_smVisionInfo.g_blnMarkInspected = false;
            m_smVisionInfo.g_blnLeadInspected = false;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                m_smVisionInfo.g_blnInspectAllTemplate = m_smVisionInfo.g_arrMarks[u].ref_blnInspectAllTemplate = true;

            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.g_blnDrawPkgResult = false;
            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;

            if (m_smVisionInfo.g_objPositioning != null)
                m_smVisionInfo.g_objPositioning.ref_bFinalResult = false;

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            m_smVisionInfo.VM_AT_OfflinePageView = false; // 2019 01 10 - CCENG: Dont put this event in Form_Closing, bcos pressing close button is just to hide the form, not close the form.

            if (m_smVisionInfo.g_arrPackage != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
                {
                    m_smVisionInfo.g_arrPackage[i].ref_blnViewUnitPosition = false;
                }
            }

            if (m_smVisionInfo.g_arrLead != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_blnViewLeadResultDrawing = false;
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            this.Hide();
        }

        private void btn_Inspect_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_blnMarkDrawing)
                return;

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Start Offline Test", " Pressed Test Button", "", "", m_smProductionInfo.g_strLotID);

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            btn_Inspect.Enabled = false;

            if (chk_Grab.Checked)
                m_smVisionInfo.MN_PR_GrabImage = true;

            Inspect();
        }

        private void tab_Result_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tp_Orient)
            {
                m_intSettingType = 0;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Mark)
            {
                m_intSettingType = 1;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Pin1)
            {
                m_intSettingType = 2;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Package)
            {
                m_intSettingType = 3;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Lead)
            {
                m_intSettingType = 4;
                UpdateTabPage();
            }
        }

        private void timer_MarkTestResult_Tick(object sender, EventArgs e)
        {
            if (!m_smVisionInfo.VM_AT_OfflinePageView)
                return;
           
            if (m_smVisionInfo.g_blnUpdateMarkTolerance)
            {
                if (m_smVisionInfo.g_blnViewMarkInspection)
                {
                }

                m_smVisionInfo.g_blnUpdateMarkTolerance = false;
            }

            if (m_smVisionInfo.PR_MN_UpdateSettingInfo)
            {
                UpdatePageSetting();
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true; // 2020-11-22 ZJYEOH : After exit any setting form Update image combo box because g_intProductionViewImage may not same with g_intSelectedImage
                m_smVisionInfo.PR_MN_UpdateSettingInfo = false;
            
            }

            if (m_smVisionInfo.PR_MN_UpdateInfo)
            {
                for (int u = 0; u < m_blnFailMark.Length; u++)
                {
                    m_blnFailMark[u] = false;
                    m_blnFailOrient[u] = false;
                    m_blnFailPin1[u] = false;
                    m_blnFailOCR[u] = false;
                }
                m_blnFailPackage = false;
                m_blnFailLead = false;
                UpdateTabPageHeaderImage();
                UpdateInfo();
                if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    UpdateLeadGroupDimensionTable(0, m_dgdLeadGroupDimensionTable);
                    UpdateLeadDefectTable(0, m_dgdDefectTable[0]);
                }
                UpdateTabPageHeaderImage();
                btn_Inspect.Enabled = true;
                if (!m_smVisionInfo.AT_VM_OfflineTestAllLead) // 2020-08-12 ZJYEOH : if tolerance form is opening then update info boolean will be controlled by tolerance form
                    m_smVisionInfo.PR_MN_UpdateInfo = false;

            }
            if (!m_smVisionInfo.PR_MN_UpdateInfo && !btn_Inspect.Enabled)
            {
                btn_Inspect.Enabled = true;
            }
        }

        private void radioBtn_SelectTemplate_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (radioBtn_Template1.Checked)
                    m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;
                else if (radioBtn_Template2.Checked)
                    m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 1;
                else if (radioBtn_Template3.Checked)
                    m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 2;
                else if (radioBtn_Template4.Checked)
                    m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 3;
            }

            UpdateSelectedTemplateChange();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_TemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = Convert.ToInt32(cbo_TemplateNo.SelectedItem) - 1;
            }

            UpdateSelectedTemplateChange();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Defect_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || (m_smVisionInfo.g_intSelectedBlobNo == e.RowIndex && e.RowIndex>0))
                return;

            m_smVisionInfo.g_intSelectedImage = Convert.ToInt32(dgd_Defect.Rows[e.RowIndex].Cells[0].Value) - 1;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            m_smVisionInfo.g_intSelectedBlobNo = e.RowIndex;
            m_smVisionInfo.g_blnViewSelectedBlobObject = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        public void LoadEvent()
        {
            // 2019 01 11 - CCENG: Don't put this function in Load Form Event function because the Form_Load event will cannot be triggered when you hide and show it again.
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.g_blnViewMarkInspection = true;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
        }
        private void UpdateLeadDefectTable(int intLeadIndex, DataGridView dgd_LeadDefectTable)
        {
            dgd_LeadDefectTable.Rows.Clear();

            List<List<string>> arrDefectList = new List<List<string>>();


            for (int j = 0; j < m_smVisionInfo.g_arrLead.Length; j++)
            {
                if (!m_smVisionInfo.g_arrLead[j].ref_blnSelected)
                    continue;

                arrDefectList = m_smVisionInfo.g_arrLead[j].GetDefectList();

                for (int i = 0; i < arrDefectList.Count; i++)
                {
                    dgd_LeadDefectTable.Rows.Add();
                    dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[0].Value = (dgd_LeadDefectTable.Rows.Count).ToString();

                    int intFailMask = Convert.ToInt32(arrDefectList[i][0]);

                    dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Value = arrDefectList[i][1];
                    if (intFailMask > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Style.BackColor = Color.Red;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Yellow;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Style.BackColor = Color.Lime;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Black;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[1].Style.SelectionForeColor = Color.Black;
                    }

                    if (arrDefectList[i][2] == "-999.0000")
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Value = "---";
                    else
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Value = arrDefectList[i][2];
                    if ((intFailMask & 0x01) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Style.BackColor = Color.Red;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Style.ForeColor = Color.Yellow;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Style.BackColor = GetColor(dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Value.ToString());
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Style.SelectionBackColor = GetColor(dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Value.ToString());
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Style.ForeColor = Color.Black;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[2].Style.SelectionForeColor = Color.Black;
                    }

                    if (arrDefectList[i][3] == "-999.0000")
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Value = "---";
                    else
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Value = arrDefectList[i][3];
                    if ((intFailMask & 0x02) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Style.BackColor = Color.Red;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Style.SelectionBackColor = Color.Red;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Style.ForeColor = Color.Yellow;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Style.BackColor = GetColor(dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Value.ToString());
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Style.SelectionBackColor = GetColor(dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Value.ToString());
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Style.ForeColor = Color.Black;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[3].Style.SelectionForeColor = Color.Black;
                    }

                    if (arrDefectList[i][4] == "0.000000")
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Value = "---";
                    else
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Value = arrDefectList[i][4];
                    if ((intFailMask & 0x04) > 0)
                    {
                        m_blnFailLead = true;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Style.BackColor = Color.Red;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Style.SelectionBackColor = Color.Red;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Style.ForeColor = Color.Yellow;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Style.SelectionForeColor = Color.Yellow;
                    }
                    else
                    {
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Style.BackColor = GetColor(dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Value.ToString());
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Style.SelectionBackColor = GetColor(dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Value.ToString());
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Style.ForeColor = Color.Black;
                        dgd_LeadDefectTable.Rows[dgd_LeadDefectTable.Rows.Count - 1].Cells[4].Style.SelectionForeColor = Color.Black;
                    }
                }
            }
        }
        private void dgd_LeadDefect_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            int r = e.RowIndex;
            if (dgd_LeadDefect.Rows[r].Cells[2].Value.ToString() == "---" && dgd_LeadDefect.Rows[r].Cells[3].Value.ToString() == "---")
                m_smVisionInfo.g_intSelectedLeadExtraBlobID = -Convert.ToInt32(dgd_LeadDefect.Rows[r].Cells[0].Value);
            else
                m_smVisionInfo.g_intSelectedLeadExtraBlobID = Convert.ToInt32(dgd_LeadDefect.Rows[r].Cells[0].Value);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void UpdateTabPageHeaderImage()
        {
            if (m_blnFailOrient[m_smVisionInfo.g_intSelectedTemplate])
                tp_Orient.ImageIndex = 1;
            else
                tp_Orient.ImageIndex = 0;

            if (m_blnFailMark[m_smVisionInfo.g_intSelectedTemplate])
                tp_Mark.ImageIndex = 1;
            else
                tp_Mark.ImageIndex = 0;

            if (m_blnFailOCR[m_smVisionInfo.g_intSelectedTemplate])
                tp_OCR.ImageIndex = 1;
            else
                tp_OCR.ImageIndex = 0;

            if (m_blnFailPin1[m_smVisionInfo.g_intSelectedTemplate])
                tp_Pin1.ImageIndex = 1;
            else
                tp_Pin1.ImageIndex = 0;

            if (m_blnFailPackage)
                tp_Package.ImageIndex = 1;
            else
                tp_Package.ImageIndex = 0;

            if (m_blnFailLead)
                tp_Lead.ImageIndex = 1;
            else
                tp_Lead.ImageIndex = 0;
        }
        private void UpdateLeadGroupDimensionTable(int intLeadIndex, DataGridView dgdLeadGroupDimensionTable)
        {

            dgdLeadGroupDimensionTable.Rows.Clear();

            int i = 0;
            if ((m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask & 0x800) > 0)
            {
                // ---------- Length Variance -------------------------------------------
                dgdLeadGroupDimensionTable.Rows.Add();
                dgdLeadGroupDimensionTable.Rows[i].Cells[0].Value = (i + 1).ToString();
                dgdLeadGroupDimensionTable.Rows[i].Cells[1].Value = "Length Variance";

                if ((m_smVisionInfo.g_arrLead[0].ref_intFailResultMask & 0x800) > 0 ||
                    (m_smVisionInfo.g_arrLead[1].ref_intFailResultMask & 0x800) > 0 ||
                    (m_smVisionInfo.g_arrLead[2].ref_intFailResultMask & 0x800) > 0 ||
                    (m_smVisionInfo.g_arrLead[3].ref_intFailResultMask & 0x800) > 0 ||
                    (m_smVisionInfo.g_arrLead[4].ref_intFailResultMask & 0x800) > 0)
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

                // Length Variance Bottom Column

                if (m_smVisionInfo.g_arrLead[1].ref_fSampleLengthVarianceMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrLead[1].ref_fSampleLengthVarianceMM.ToString("F4");
                if ((m_smVisionInfo.g_arrLead[1].ref_intFailResultMask & 0x800) > 0)
                {
                    if (dgdLeadGroupDimensionTable.Columns[4].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[4].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[4].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                }

                // Length Variance Right Column
                if (m_smVisionInfo.g_arrLead[2].ref_fSampleLengthVarianceMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[2].ref_fSampleLengthVarianceMM.ToString("F4");
                if ((m_smVisionInfo.g_arrLead[2].ref_intFailResultMask & 0x800) > 0)
                {
                    if (dgdLeadGroupDimensionTable.Columns[3].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[3].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[3].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                // Length Variance Bottom Column
                if (m_smVisionInfo.g_arrLead[3].ref_fSampleLengthVarianceMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Value = m_smVisionInfo.g_arrLead[3].ref_fSampleLengthVarianceMM.ToString("F4");
                if ((m_smVisionInfo.g_arrLead[3].ref_intFailResultMask & 0x800) > 0)
                {
                    if (dgdLeadGroupDimensionTable.Columns[5].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[5].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[5].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;
                }

                // Length Variance Left Column
                if (m_smVisionInfo.g_arrLead[4].ref_fSampleLengthVarianceMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[4].ref_fSampleLengthVarianceMM.ToString("F4");
                if ((m_smVisionInfo.g_arrLead[4].ref_intFailResultMask & 0x800) > 0)
                {
                    if (dgdLeadGroupDimensionTable.Columns[2].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[2].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[2].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                }

                // Length Variance Unit Column
                if (m_smVisionInfo.g_arrLead[0].ref_fSampleLengthVarianceMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Value = m_smVisionInfo.g_arrLead[0].ref_fSampleLengthVarianceMM.ToString("F4");
                if ((m_smVisionInfo.g_arrLead[0].ref_intFailResultMask & 0x800) > 0)
                {
                    if (dgdLeadGroupDimensionTable.Columns[6].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[6].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[6].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;
                }
                i++;
            }

            if ((m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask & 0x1000) > 0)
            {
                // ---------- Span -------------------------------------------
                dgdLeadGroupDimensionTable.Rows.Add();
                dgdLeadGroupDimensionTable.Rows[i].Cells[0].Value = (i + 1).ToString();
                dgdLeadGroupDimensionTable.Rows[i].Cells[1].Value = "Span";

                if ((m_smVisionInfo.g_arrLead[0].ref_intFailResultMask & 0x1000) > 0 ||
                    (m_smVisionInfo.g_arrLead[1].ref_intFailResultMask & 0x1000) > 0 ||
                    (m_smVisionInfo.g_arrLead[2].ref_intFailResultMask & 0x1000) > 0 ||
                    (m_smVisionInfo.g_arrLead[3].ref_intFailResultMask & 0x1000) > 0 ||
                    (m_smVisionInfo.g_arrLead[4].ref_intFailResultMask & 0x1000) > 0)
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                }

                // Span Top Column
                if (m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM.ToString("F4");
                if (((m_smVisionInfo.g_arrLead[1].ref_intFailResultMask & 0x1000) > 0) &&
                    (m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMinSpanLimit) ||
                    (m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMaxSpanLimit))
                {
                    if (dgdLeadGroupDimensionTable.Columns[4].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[4].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[4].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                }

                // Span Right Column
                if (m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM.ToString("F4");
                if (((m_smVisionInfo.g_arrLead[2].ref_intFailResultMask & 0x1000) > 0) &&
                    (m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMinSpanLimit) ||
                    (m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMaxSpanLimit))
                {
                    if (dgdLeadGroupDimensionTable.Columns[3].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[3].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[3].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                }

                // Span Bottom Column
                if (m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Value = m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM.ToString("F4");
                if (((m_smVisionInfo.g_arrLead[3].ref_intFailResultMask & 0x1000) > 0) &&
                    (m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMinSpanLimit) ||
                    (m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMaxSpanLimit))
                {
                    if (dgdLeadGroupDimensionTable.Columns[5].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[5].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[5].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[5].Style.SelectionForeColor = Color.Black;
                }

                // Span Left Column
                if (m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM.ToString("F4");
                if (((m_smVisionInfo.g_arrLead[4].ref_intFailResultMask & 0x1000) > 0) &&
                    (m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMinSpanLimit) ||
                    (m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMaxSpanLimit))
                {
                    if (dgdLeadGroupDimensionTable.Columns[2].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[2].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[2].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[2].Style.SelectionForeColor = Color.Black;
                }

                // Span Unit Column
                if (m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM == -999)
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Value = "---";
                else
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Value = m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM.ToString("F4");
                if ((m_smVisionInfo.g_arrLead[0].ref_intFailResultMask & 0x1000) > 0)
                {
                    if (dgdLeadGroupDimensionTable.Columns[6].Visible)
                        m_blnFailLead = true;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.BackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.SelectionBackColor = Color.Red;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.ForeColor = Color.Yellow;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.BackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[6].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.SelectionBackColor = GetColor(dgdLeadGroupDimensionTable.Rows[i].Cells[6].Value.ToString());
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                    dgdLeadGroupDimensionTable.Rows[i].Cells[6].Style.SelectionForeColor = Color.Black;
                }
            }

            if (dgdLeadGroupDimensionTable.Rows.Count > 0)
            {
                dgdLeadGroupDimensionTable.Visible = true;
                if (dgdLeadGroupDimensionTable.Columns[2].Visible == true && dgdLeadGroupDimensionTable.Columns[3].Visible == true && dgdLeadGroupDimensionTable.Columns[4].Visible == true && dgdLeadGroupDimensionTable.Columns[5].Visible == true)
                    dgdLeadGroupDimensionTable.Size = new Size(dgdLeadGroupDimensionTable.Size.Width, (dgdLeadGroupDimensionTable.Rows.Count + 2) * 22);
                else if ((dgdLeadGroupDimensionTable.Columns[2].Visible == true && dgdLeadGroupDimensionTable.Columns[3].Visible == true) || (dgdLeadGroupDimensionTable.Columns[4].Visible == true && dgdLeadGroupDimensionTable.Columns[5].Visible == true))
                    dgdLeadGroupDimensionTable.Size = new Size(dgdLeadGroupDimensionTable.Size.Width, (dgdLeadGroupDimensionTable.Rows.Count + 1) * 22);
            }
            else
                dgdLeadGroupDimensionTable.Visible = false;
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
        private Color GetColor(string strValue)
        {
            if (strValue == "---")
                return Color.White;
            else
                return Color.Lime;

        }
    }
}