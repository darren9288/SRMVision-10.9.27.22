using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class LeadToleranceSettingForm : Form
    {
        #region Member Variables
        private bool m_blnChangeScoreSetting = true;
        private bool m_blnFormOpen = false;
        private bool m_blnInitDone = false;
        private bool m_blnUpdateInfo = false;
        private int m_intUserGroup = 5;
        //private int m_intLeadIndex = 0;
        //private int m_intGDSelectedIndex = 0;   // Golden Data Set Selected index
        private int m_intDecimal = 3;
        private int m_intDecimal2 = 6;
        private int m_intDecimalPlaces = 4;
        private string m_strSelectedRecipe;
        private string m_strUnitLabel = "mm";
        
        private DataGridView[] m_dgdView = new DataGridView[4];
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private GoldenUnitCompensationForm objGoldenUnitForm;
        #endregion


        #region Properties

        public bool ref_blnFormOpen { get { return m_blnFormOpen; } set { m_blnFormOpen = value; } }
        public bool ref_blnUpdateInfo { get { return m_blnUpdateInfo; } set { m_blnUpdateInfo = value; } }

        #endregion

        public LeadToleranceSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, int intLeadSelectedMask, ProductionInfo smProductionInfo)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;
            m_smProductionInfo = smProductionInfo;
            
            m_dgdView[0] = dgd_TopLead;
            m_dgdView[1] = dgd_RightLead;
            m_dgdView[2] = dgd_BottomLead;
            m_dgdView[3] = dgd_LeftLead;

            //for (int i = 0; i < 5; i++)
            //{
            //    if ((intLeadSelectedMask & (1 << i)) > 0)
            //    {
            //        switch (i)
            //        {
            //            case 1:
            //                m_intLeadIndex = 1;
            //                radioBtn_Up.Enabled = true;
            //                radioBtn_Up.Checked = true;
            //                break;
            //            case 2:
            //                m_intLeadIndex = 2;
            //                radioBtn_Right.Enabled = true;
            //                radioBtn_Right.Checked = true;
            //                break;
            //            case 3:
            //                radioBtn_Down.Enabled = true;
            //                break;
            //            case 4:
            //                radioBtn_Left.Enabled = true;
            //                break;
            //        }
            //    }
            //}

            //LoadGoldenData();
            DisableField2();
            UpdateGUI();

            m_blnInitDone = true;

            // 2020-01-06 ZJYEOH : Trigger Offline Test one time 
            m_smVisionInfo.g_intViewInspectionSetting = 1;
            m_smVisionInfo.AT_VM_OfflineTestAllLead = true;
            TriggerOfflineTest();
        }
        private int GetUserRightGroup_Child2(string Child1, string Child2)
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
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild2Group(Child1, Child2);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild2Group(Child1, Child2, m_smVisionInfo.g_intVisionNameNo);
                    break;
            }

            return 1;
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Tolerance";
            string strChild2 = "";

            strChild2 = "Lead TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                m_blnChangeScoreSetting = false;
                dgd_WholeLeadSetting.Enabled = false;
                btn_UpdateTolerance.Enabled = false;
                btn_LoadTolFromFile.Enabled = false;
                btn_SaveTolToFile.Enabled = false;
                btn_SaveAccuracyReport.Enabled = false;
                btn_GoldenUnitSetting.Enabled = false;
            }
            
            strChild2 = "Save Button";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
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
        private void UpdateGUI()
        {
            ReadLeadTemplateDataToGrid(m_dgdView[0]); //ReadLeadTemplateDataToGrid(m_intLeadIndex, m_dgdView[0]);

            m_dgdView[0].Columns.Remove(column_GolWidth);
            m_dgdView[0].Columns.Remove(column_GolLength);
            m_dgdView[0].Columns.Remove(column_GolPitch);
            m_dgdView[0].Columns.Remove(column_GolGap);
            if (m_intUserGroup != 1)    // for SRM only
            {
                btn_GoldenUnitSetting.Visible = false;
            }

            if (m_smVisionInfo.g_arrLead.Length == 1)
            {
                srmGroupBox5.Visible = false;
            }

            switch (m_smCustomizeInfo.g_intUnitDisplay)
            {
                default:
                case 1:
                    lbl_SetDescription.Text = lbl_SetDescription.Text + "(mm/mm^2)";
                    break;
                case 2:
                    lbl_SetDescription.Text = lbl_SetDescription.Text + "(mil/mil^2)";
                    break;
                case 3:
                    lbl_SetDescription.Text = lbl_SetDescription.Text + "(um/um^2)";
                    break;
            }

            UpdateUnitDisplay();
            
            txt_MinSpan.Text = m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
            txt_MaxSpan.Text = m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);

            UpdateScore(m_dgdView[0]); //UpdateScore(m_intLeadIndex, m_dgdView[0]);

            // ------------ Update whole lead table ----------------------------------------------------------------------------
            PreUpdateWholeLeadSettingTable();
            UpdateWholdLeadsSettingGUI();
            UpdateWholeLeadScore();

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_DisplayResult.Checked = Convert.ToBoolean(subkey1.GetValue("CurrentDisplayResult_LeadTolerance", false));
            ViewOrHideResultColumn(chk_DisplayResult.Checked);
        }

        private void UpdateUnitDisplay()
        {
            string strUnitDisplay;
            switch (m_smCustomizeInfo.g_intUnitDisplay)
            {
                case 1:
                default:
                    strUnitDisplay = "mm";
                    m_intDecimal = 4;
                    m_intDecimal2 = 6;
                    break;
                case 2:
                    strUnitDisplay = "mil";
                    m_intDecimal = 3;
                    m_intDecimal2 = 6;
                    break;
                case 3:
                    strUnitDisplay = "um";
                    m_intDecimal = 1;
                    m_intDecimal2 = 2;
                    break;
            }

            txt_MinSpan.DecimalPlaces = m_intDecimal;
            txt_MaxSpan.DecimalPlaces = m_intDecimal;
        }

        //private void LoadGoldenData()
        //{
        //    // Load Golden Data
        //    string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
        //            m_smVisionInfo.g_strVisionFolderName + "\\Lead\\GoldenData.xml";

        //    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
        //    {
        //        string strSectionName = "";
        //        if (i == 0)
        //            strSectionName = "CenterGoldenData";
        //        else if (i == 1)
        //            strSectionName = "TopGoldenData";
        //        else if (i == 2)
        //            strSectionName = "RightGoldenData";
        //        else if (i == 3)
        //            strSectionName = "BottomGoldenData";
        //        else if (i == 4)
        //            strSectionName = "LeftGoldenData";

        //        m_smVisionInfo.g_arrLead[i].LoadLeadGoldenData(strPath, strSectionName);
        //    }
        //}

        //private void UpdateGoldenDataIntoGridTable(int intLeadIndex, DataGridView dgd_LeadSetting)
        //{
        //    if (objGoldenUnitForm == null)
        //        return;

        //    if (!objGoldenUnitForm.ref_intViewGoldenDataColumn)
        //        return;

        //    if (m_intGDSelectedIndex >= m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData.Count)
        //        return;

        //    for (int r = 0; r < dgd_LeadSetting.Rows.Count; r++)
        //    {
        //        if (r < m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Count)
        //        {
        //            if (dgd_LeadSetting.Rows[r].Cells.Count <= 25)
        //                continue;

        //            if (m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 0)
        //                dgd_LeadSetting.Rows[r].Cells[25].Value = m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][0];
        //            else
        //                dgd_LeadSetting.Rows[r].Cells[25].Value = 0;

        //            if (m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 1)
        //                dgd_LeadSetting.Rows[r].Cells[26].Value = m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][1];
        //            else
        //                dgd_LeadSetting.Rows[r].Cells[26].Value = 0;

        //            if (m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 2)
        //                dgd_LeadSetting.Rows[r].Cells[27].Value = m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][2];
        //            else
        //                dgd_LeadSetting.Rows[r].Cells[27].Value = 0;

        //            if (m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 3)
        //                dgd_LeadSetting.Rows[r].Cells[28].Value = m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][3];
        //            else
        //                dgd_LeadSetting.Rows[r].Cells[28].Value = 0;
        //        }
        //        else
        //        {
        //            dgd_LeadSetting.Rows[r].Cells[25].Value = 0;
        //            dgd_LeadSetting.Rows[r].Cells[26].Value = 0;
        //            dgd_LeadSetting.Rows[r].Cells[27].Value = 0;
        //            dgd_LeadSetting.Rows[r].Cells[28].Value = 0;
        //        }
        //    }
        //}

        //private void UpdateGridTableIntoGoldenData(int intLeadIndex, DataGridView dgd_LeadSetting)
        //{
        //    m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Clear();

        //    for (int r = 0; r < dgd_LeadSetting.Rows.Count; r++)
        //    {
        //        m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Add(new List<float>());
        //        m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[25].Value));
        //        m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[26].Value));
        //        m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[27].Value));
        //        m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[28].Value));

        //    }
        //}

        //private void CheckSaveGoldenData(int intLeadIndex, DataGridView dgd_LeadSetting)
        //{
        //    if (objGoldenUnitForm == null)
        //        return;

        //    if (!objGoldenUnitForm.ref_intViewGoldenDataColumn)
        //        return;

        //    bool blnIsDataChanged = false;
        //    if (m_intGDSelectedIndex >= 0 && m_intGDSelectedIndex < m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData.Count)
        //    {
        //        if (dgd_LeadSetting.Rows.Count != m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Count)
        //        {
        //            blnIsDataChanged = true;
        //        }
        //        else
        //        {

        //            for (int r = 0; r < dgd_LeadSetting.Rows.Count; r++)
        //            {
        //                if (Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[25].Value) != m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][0])
        //                {
        //                    blnIsDataChanged = true;
        //                    break;
        //                }

        //                if (Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[26].Value) != m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][1])
        //                {
        //                    blnIsDataChanged = true;
        //                    break;
        //                }

        //                if (m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 2)
        //                {
        //                    if (Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[27].Value) != m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][2])
        //                    {
        //                        blnIsDataChanged = true;
        //                        break;
        //                    }
        //                }

        //                if (m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 3)
        //                {
        //                    if (Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[28].Value) != m_smVisionInfo.g_arrLead[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][3])
        //                    {
        //                        blnIsDataChanged = true;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (blnIsDataChanged)
        //    {
        //        if (SRMMessageBox.Show("Do you want to save the golden data changes you have made?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //        {
        //            UpdateGridTableIntoGoldenData(intLeadIndex, dgd_LeadSetting);

        //            SaveGoldenData();
        //        }
        //        else
        //        {
        //            UpdateGoldenDataIntoGridTable(intLeadIndex, dgd_LeadSetting);
        //        }

        //    }
        //}

        //private void SaveGoldenData()
        //{
        //    string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
        //                               m_smVisionInfo.g_strVisionFolderName + "\\Lead\\GoldenData.xml";

        //    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
        //    {
        //        string strSectionName = "";
        //        if (i == 0)
        //            strSectionName = "CenterGoldenData";
        //        else if (i == 1)
        //            strSectionName = "TopGoldenData";
        //        else if (i == 2)
        //            strSectionName = "RightGoldenData";
        //        else if (i == 3)
        //            strSectionName = "BottomGoldenData";
        //        else if (i == 4)
        //            strSectionName = "LeftGoldenData";

        //        m_smVisionInfo.g_arrLead[i].SaveLeadGoldenData(strPath, false, strSectionName, true);
        //    }
        //}

        //private void UpdateScore(int intLeadIndex, DataGridView dgd_LeadSetting)
        //{

        //    TrackLog objTL = new TrackLog();
        //    objTL.WriteLine("                 ");

        //    for (int i = 0; i < dgd_LeadSetting.Rows.Count; i++)
        //    {
        //        string strBlobsFeatures = "---#---#---#---#---#---#---#---#---#---#---#";

        //        if ((m_smVisionInfo.g_arrLead[intLeadIndex].ref_intFailResultMask & 0x1000) == 0)
        //            strBlobsFeatures = m_smVisionInfo.g_arrLead[intLeadIndex].GetBlobFeaturesResult(i);

        //        string[] strFeature = strBlobsFeatures.Split('#');


        //        //objTL.WriteLine(strBlobsFeatures);



        //        int intFeatureIndex = 0;

        //        #region Update value to grid
        //        intFeatureIndex++;

        //        int[] intGridResultColumnIndex = { 1, 4, 7, 11, 15, 19, 23, 26};
        //        for (int u = 0; u < intGridResultColumnIndex.Length; u++)
        //        {
        //            dgd_LeadSetting.Rows[i].Cells[intGridResultColumnIndex[u]].Value = strFeature[intFeatureIndex++];
        //        }

        //        if (m_smVisionInfo.g_arrLead[0].ref_fLeadMinSpanResult == 0)
        //        {
        //            lbl_ResultMinSpan.Text = "---";
        //        }
        //        else
        //        {
        //            lbl_ResultMinSpan.Text = m_smVisionInfo.g_arrLead[0].ref_fLeadMinSpanResult.ToString("F" + m_intDecimal);
        //        }

        //        if (m_smVisionInfo.g_arrLead[0].ref_fLeadMaxSpanResult == 0)
        //        {
        //            lbl_ResultMaxSpan.Text = "---";
        //        }
        //        else
        //        {
        //            lbl_ResultMaxSpan.Text = m_smVisionInfo.g_arrLead[0].ref_fLeadMaxSpanResult.ToString("F" + m_intDecimal);
        //        }

        //        #endregion

        //        #region Update grid font color

        //        float fAccuracySpec = 0.0125f;

        //        float fMinValue, fResultValue, fMaxValue;
        //        // OffSet
        //        if (dgd_LeadSetting.Rows[i].Cells[1].Value.ToString() != "---")
        //        {
        //            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[1].Value.ToString());
        //            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[0].Value.ToString());
        //            if (fResultValue > fMaxValue)
        //            {
        //                dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Red;
        //                dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
        //                dgd_LeadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
        //                dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
        //                dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
        //                dgd_LeadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
        //                dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
        //            }
        //        }

        //        // Skew
        //        if (dgd_LeadSetting.Rows[i].Cells[4].Value.ToString() != "---")
        //        {
        //            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[4].Value.ToString());
        //            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[3].Value.ToString());
        //            if (fResultValue > fMaxValue)
        //            {
        //                dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
        //                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
        //                dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
        //                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
        //                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
        //                dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
        //                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
        //            }
        //        }

        //        // Width
        //        if (dgd_LeadSetting.Rows[i].Cells[7].Value.ToString() != "---")
        //        {
        //            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[6].Value.ToString());
        //            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[7].Value.ToString());
        //            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[8].Value.ToString());
        //            //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //            //    dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Red;
        //            //else
        //            {
        //                if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_LeadSetting.Rows[i].Cells.Count < 37)
        //                {
        //                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //                    {
        //                        dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Red;
        //                        dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Red;
        //                        dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Yellow;
        //                        dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Yellow;
        //                    }
        //                    else
        //                    {
        //                        dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Lime;
        //                        dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Lime;
        //                        dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
        //                        dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;
        //                    }
        //                }
        //                else
        //                {
        //                    if (dgd_LeadSetting.Rows[i].Cells.Count > 37)
        //                    {
        //                        if (dgd_LeadSetting.Rows[i].Cells[37].Value != null)
        //                        {
        //                            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[37].Value.ToString()) - fAccuracySpec;
        //                            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[7].Value.ToString());
        //                            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[37].Value.ToString()) + fAccuracySpec;
        //                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //                            {
        //                                dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Red;
        //                                dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Red;
        //                                dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Yellow;
        //                                dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Yellow;
        //                            }
        //                            else
        //                            {
        //                                dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Lime;
        //                                dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Lime;
        //                                dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
        //                                dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // Length
        //        if (dgd_LeadSetting.Rows[i].Cells[11].Value.ToString() != "---")
        //        {
        //            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[10].Value.ToString());
        //            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[11].Value.ToString());
        //            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[12].Value.ToString());
        //            //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //            //    dgd_LeadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Red;
        //            //else
        //            {
        //                if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_LeadSetting.Rows[i].Cells.Count < 37)
        //                {
        //                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //                    {
        //                        dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Red;
        //                        dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Red;
        //                        dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Yellow;
        //                        dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Yellow;
        //                    }
        //                    else
        //                    {
        //                        dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Lime;
        //                        dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Lime;
        //                        dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
        //                        dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;
        //                    }
        //                }
        //                else
        //                {
        //                    if (dgd_LeadSetting.Rows[i].Cells.Count > 38)
        //                    {
        //                        if (dgd_LeadSetting.Rows[i].Cells[38].Value != null)
        //                        {
        //                            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[38].Value.ToString()) - fAccuracySpec;
        //                            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[11].Value.ToString());
        //                            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[38].Value.ToString()) + fAccuracySpec;
        //                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //                            {
        //                                dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Red;
        //                                dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Red;
        //                                dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Yellow;
        //                                dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Yellow;
        //                            }
        //                            else
        //                            {
        //                                dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Lime;
        //                                dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Lime;
        //                                dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
        //                                dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //        }

        //        // Pitch
        //        if (!((dgd_LeadSetting.Rows[i].Cells[14].Value.ToString() == "---") ||
        //            (dgd_LeadSetting.Rows[i].Cells[15].Value.ToString() == "---") ||
        //            (dgd_LeadSetting.Rows[i].Cells[16].Value.ToString() == "---")))
        //        {
        //            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[14].Value.ToString());
        //            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[15].Value.ToString());
        //            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[16].Value.ToString());
        //            //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //            //    dgd_LeadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Red;
        //            //else
        //            {
        //                if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_LeadSetting.Rows[i].Cells.Count < 37)
        //                {
        //                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //                    {
        //                        dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.Red;
        //                        dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Red;
        //                        dgd_LeadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Yellow;
        //                        dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Yellow;
        //                    }
        //                    else
        //                    {
        //                        dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.Lime;
        //                        dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Lime;
        //                        dgd_LeadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Black;
        //                        dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Black;
        //                    }
        //                }
        //                else
        //                {
        //                    if (dgd_LeadSetting.Rows[i].Cells.Count > 39 && (dgd_LeadSetting.Rows[i].Cells[39].Value != null))
        //                    {
        //                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[39].Value.ToString()) - fAccuracySpec;
        //                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[15].Value.ToString());
        //                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[39].Value.ToString()) + fAccuracySpec;
        //                        if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //                        {
        //                            dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.Red;
        //                            dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Red;
        //                            dgd_LeadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Yellow;
        //                            dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Yellow;
        //                        }
        //                        else
        //                        {
        //                            dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.Lime;
        //                            dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Lime;
        //                            dgd_LeadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Black;
        //                            dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Black;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // Gap
        //        if (!((dgd_LeadSetting.Rows[i].Cells[18].Value.ToString() == "---") ||
        //            (dgd_LeadSetting.Rows[i].Cells[19].Value.ToString() == "---") ||
        //            (dgd_LeadSetting.Rows[i].Cells[20].Value.ToString() == "---")))
        //        {
        //            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[18].Value.ToString());
        //            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[19].Value.ToString());
        //            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[20].Value.ToString());
        //            //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //            //    dgd_LeadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Red;
        //            //else
        //            {
        //                if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_LeadSetting.Rows[i].Cells.Count < 37)
        //                {
        //                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //                    {
        //                        dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.Red;
        //                        dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Red;
        //                        dgd_LeadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Yellow;
        //                        dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Yellow;
        //                    }
        //                    else
        //                    {
        //                        dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.Lime;
        //                        dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Lime;
        //                        dgd_LeadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Black;
        //                        dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Black;
        //                    }
        //                }
        //                else
        //                {
        //                    if (dgd_LeadSetting.Rows[i].Cells.Count > 40 && (dgd_LeadSetting.Rows[i].Cells[40].Value != null))
        //                    {
        //                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[40].Value.ToString()) - fAccuracySpec;
        //                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[19].Value.ToString());
        //                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[40].Value.ToString()) + fAccuracySpec;
        //                        if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //                        {
        //                            dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.Red;
        //                            dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Red;
        //                            dgd_LeadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Yellow;
        //                            dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Yellow;
        //                        }
        //                        else
        //                        {
        //                            dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.Lime;
        //                            dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Lime;
        //                            dgd_LeadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Black;
        //                            dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Black;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // Variance
        //        if (dgd_LeadSetting.Rows[i].Cells[23].Value.ToString() != "---")
        //        {
        //            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[23].Value.ToString());
        //            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[22].Value.ToString());
        //            if (fResultValue > fMaxValue)
        //            {
        //                dgd_LeadSetting.Rows[i].Cells[23].Style.BackColor = Color.Red;
        //                dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionBackColor = Color.Red;
        //                dgd_LeadSetting.Rows[i].Cells[23].Style.ForeColor = Color.Yellow;
        //                dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_LeadSetting.Rows[i].Cells[23].Style.BackColor = Color.Lime;
        //                dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionBackColor = Color.Lime;
        //                dgd_LeadSetting.Rows[i].Cells[23].Style.ForeColor = Color.Black;
        //                dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionForeColor = Color.Black;
        //            }
        //        }

        //        // Average Grat=y Value
        //        if (dgd_LeadSetting.Rows[i].Cells[26].Value.ToString() != "---")
        //        {
        //            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[25].Value.ToString());
        //            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[26].Value.ToString());
        //            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[27].Value.ToString());
        //            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
        //            {
        //                dgd_LeadSetting.Rows[i].Cells[26].Style.BackColor = Color.Red;
        //                dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionBackColor = Color.Red;
        //                dgd_LeadSetting.Rows[i].Cells[26].Style.ForeColor = Color.Yellow;
        //                dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionForeColor = Color.Yellow;
        //            }
        //            else
        //            {
        //                dgd_LeadSetting.Rows[i].Cells[26].Style.BackColor = Color.Lime;
        //                dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionBackColor = Color.Lime;
        //                dgd_LeadSetting.Rows[i].Cells[26].Style.ForeColor = Color.Black;
        //                dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionForeColor = Color.Black;
        //            }
        //        }

        //        #endregion

        //    }

        //    if (lbl_ResultMinSpan.Text != "---" && lbl_ResultMaxSpan.Text != "---")
        //    {
        //        //Span
        //        float fMinSpanValue, fMinResultValue, fMaxResultValue, fMaxSpanValue;
        //        fMinSpanValue = Convert.ToSingle(txt_MinSpan.Text);
        //        fMaxSpanValue = Convert.ToSingle(txt_MaxSpan.Text);
        //        fMinResultValue = Convert.ToSingle(lbl_ResultMinSpan.Text);
        //        fMaxResultValue = Convert.ToSingle(lbl_ResultMaxSpan.Text);

        //        if (fMinResultValue < fMinSpanValue)
        //            lbl_ResultMinSpan.ForeColor = Color.Red;
        //        else
        //            lbl_ResultMinSpan.ForeColor = Color.Black;

        //        if (fMaxResultValue > fMaxSpanValue)
        //            lbl_ResultMaxSpan.ForeColor = Color.Red;
        //        else
        //            lbl_ResultMaxSpan.ForeColor = Color.Black;
        //    }
        //}
        private void UpdateScore(DataGridView dgd_LeadSetting)
        {

            TrackLog objTL = new TrackLog();
            objTL.WriteLine("                 ");
            
            for (int x = 0; x < m_smVisionInfo.g_arrLead.Length; x++)
            {
                if (!m_smVisionInfo.g_arrLead[x].ref_blnSelected)
                    continue;
                
                int intBlobsCount = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesNumber();
                int intBlobID = m_smVisionInfo.g_arrLead[x].GetBlobsNoID();
                int intCount = 0;
                for (int i = intBlobID - 1; i < intBlobsCount +(intBlobID - 1); i++)
                {
                    if (i < 0)
                        continue;

                    string strBlobsFeatures = "---#---#---#---#---#---#---#---#---#---#";

                    // 2020 11 18 - CCENG: Not understand why need to check lead span fail first then only can get blob feature result.
                    //                   : When span fail, all lead inspection data will not able to display due to this code, so I hide it temporary.
                    //if ((m_smVisionInfo.g_arrLead[x].ref_intFailResultMask & 0x1000) == 0)    
                    {
                        strBlobsFeatures = m_smVisionInfo.g_arrLead[x].GetBlobFeaturesResult(intCount);
                        
                        if (m_smVisionInfo.g_arrLead[x].ref_blnWantInspectBaseLead)
                        {
                            float fOffset = -999, fArea = -999;
                            m_smVisionInfo.g_arrLead[x].GetBaseLeadResult(intCount, ref fOffset, ref fArea);

                            if (fOffset != -999)
                                dgd_LeadSetting.Rows[i].Cells[30].Value = fOffset;
                            else
                                dgd_LeadSetting.Rows[i].Cells[30].Value = "---";

                            if (fArea != -999)
                                dgd_LeadSetting.Rows[i].Cells[33].Value = fArea;
                            else
                                dgd_LeadSetting.Rows[i].Cells[33].Value = "---";
                        }
                    }

                    intCount++;
                    string[] strFeature = strBlobsFeatures.Split('#');


                    //objTL.WriteLine(strBlobsFeatures);



                    int intFeatureIndex = 0;

                    #region Update value to grid
                    intFeatureIndex++;

                    int[] intGridResultColumnIndex = { 1, 4, 7, 11, 15, 19, 23, 26 };
                    for (int u = 0; u < intGridResultColumnIndex.Length; u++)
                    {
                        dgd_LeadSetting.Rows[i].Cells[intGridResultColumnIndex[u]].Value = strFeature[intFeatureIndex++];
                    }

                    if (m_smVisionInfo.g_arrLead[0].ref_fLeadMinSpanResult == 0)
                    {
                        lbl_ResultMinSpan.Text = "---";
                    }
                    else
                    {
                        lbl_ResultMinSpan.Text = m_smVisionInfo.g_arrLead[0].ref_fLeadMinSpanResult.ToString("F" + m_intDecimal);
                    }

                    if (m_smVisionInfo.g_arrLead[0].ref_fLeadMaxSpanResult == 0)
                    {
                        lbl_ResultMaxSpan.Text = "---";
                    }
                    else
                    {
                        lbl_ResultMaxSpan.Text = m_smVisionInfo.g_arrLead[0].ref_fLeadMaxSpanResult.ToString("F" + m_intDecimal);
                    }

                    #endregion

                    #region Update grid font color

                    float fAccuracySpec = 0.0125f;

                    float fMinValue, fResultValue, fMaxValue;
                    // OffSet
                    if (dgd_LeadSetting.Rows[i].Cells[1].Value != null && dgd_LeadSetting.Rows[i].Cells[1].Value.ToString() != "---")
                    {
                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[1].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[0].Value.ToString());
                        if (fResultValue > fMaxValue)
                        {
                            dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                        }
                    }

                    // Skew
                    if (dgd_LeadSetting.Rows[i].Cells[4].Value != null && dgd_LeadSetting.Rows[i].Cells[4].Value.ToString() != "---" && dgd_LeadSetting.Rows[i].Cells[4].Value.ToString() != "----")
                    {
                        //if (dgd_LeadSetting.Rows[i].Cells[4].Value.ToString() != "---" && dgd_LeadSetting.Rows[i].Cells[4].Value.ToString() != "----")
                        {
                            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[4].Value.ToString());
                            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[3].Value.ToString());
                            if (fResultValue > fMaxValue)
                            {
                                dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
                                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                                dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                                dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        //else
                        //{
                        //    if (dgd_LeadSetting.Rows[i].Cells[4].Value.ToString() == "----")
                        //    {
                        //        dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
                        //        dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                        //        dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                        //        dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                        //    }
                        //}
                    }

                    // Width
                    if (dgd_LeadSetting.Rows[i].Cells[7].Value != null && dgd_LeadSetting.Rows[i].Cells[7].Value.ToString() != "---")
                    {
                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[6].Value.ToString());
                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[7].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[8].Value.ToString());
                        //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                        //    dgd_LeadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Red;
                        //else
                        {
                            if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_LeadSetting.Rows[i].Cells.Count < 36)
                            {
                                if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                {
                                    dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Red;
                                    dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Red;
                                    dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Yellow;
                                    dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Lime;
                                    dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Lime;
                                    dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
                                    dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                if (dgd_LeadSetting.Rows[i].Cells.Count > 36)
                                {
                                    if (dgd_LeadSetting.Rows[i].Cells[36].Value != null)
                                    {
                                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[36].Value.ToString()) - fAccuracySpec;
                                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[7].Value.ToString());
                                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[36].Value.ToString()) + fAccuracySpec;
                                        if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                        {
                                            dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Red;
                                            dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Red;
                                            dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Yellow;
                                            dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Yellow;
                                        }
                                        else
                                        {
                                            dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.Lime;
                                            dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.Lime;
                                            dgd_LeadSetting.Rows[i].Cells[7].Style.ForeColor = Color.Black;
                                            dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionForeColor = Color.Black;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Length
                    if (dgd_LeadSetting.Rows[i].Cells[11].Value != null && dgd_LeadSetting.Rows[i].Cells[11].Value.ToString() != "---")
                    {
                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[10].Value.ToString());
                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[11].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[12].Value.ToString());
                        //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                        //    dgd_LeadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Red;
                        //else
                        {
                            if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_LeadSetting.Rows[i].Cells.Count < 36)
                            {
                                if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                {
                                    dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Red;
                                    dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Red;
                                    dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Yellow;
                                    dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Yellow;
                                }
                                else
                                {
                                    dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Lime;
                                    dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Lime;
                                    dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                    dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                if (dgd_LeadSetting.Rows[i].Cells.Count > 37)
                                {
                                    if (dgd_LeadSetting.Rows[i].Cells[37].Value != null)
                                    {
                                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[37].Value.ToString()) - fAccuracySpec;
                                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[11].Value.ToString());
                                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[37].Value.ToString()) + fAccuracySpec;
                                        if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                        {
                                            dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Red;
                                            dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Red;
                                            dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Yellow;
                                            dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Yellow;
                                        }
                                        else
                                        {
                                            dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.Lime;
                                            dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.Lime;
                                            dgd_LeadSetting.Rows[i].Cells[11].Style.ForeColor = Color.Black;
                                            dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionForeColor = Color.Black;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Pitch
                    if ((dgd_LeadSetting.Rows[i].Cells[14].Value != null) &&
                        (dgd_LeadSetting.Rows[i].Cells[15].Value != null) &&
                        (dgd_LeadSetting.Rows[i].Cells[26].Value != null))
                    {
                        if (!((dgd_LeadSetting.Rows[i].Cells[14].Value.ToString() == "---") ||
                        (dgd_LeadSetting.Rows[i].Cells[15].Value.ToString() == "---") ||
                        (dgd_LeadSetting.Rows[i].Cells[16].Value.ToString() == "---")))
                        {
                            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[14].Value.ToString());
                            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[15].Value.ToString());
                            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[16].Value.ToString());
                            //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                            //    dgd_LeadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Red;
                            //else
                            {
                                if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_LeadSetting.Rows[i].Cells.Count < 36)
                                {
                                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                    {
                                        dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.Red;
                                        dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Red;
                                        dgd_LeadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Yellow;
                                        dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Yellow;
                                    }
                                    else
                                    {
                                        dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.Lime;
                                        dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Lime;
                                        dgd_LeadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Black;
                                        dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Black;
                                    }
                                }
                                else
                                {
                                    if (dgd_LeadSetting.Rows[i].Cells.Count > 38 && (dgd_LeadSetting.Rows[i].Cells[38].Value != null))
                                    {
                                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[38].Value.ToString()) - fAccuracySpec;
                                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[15].Value.ToString());
                                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[38].Value.ToString()) + fAccuracySpec;
                                        if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                        {
                                            dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.Red;
                                            dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Red;
                                            dgd_LeadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Yellow;
                                            dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Yellow;
                                        }
                                        else
                                        {
                                            dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.Lime;
                                            dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.Lime;
                                            dgd_LeadSetting.Rows[i].Cells[15].Style.ForeColor = Color.Black;
                                            dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionForeColor = Color.Black;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Gap
                    if ((dgd_LeadSetting.Rows[i].Cells[18].Value != null) &&
                        (dgd_LeadSetting.Rows[i].Cells[19].Value != null) &&
                        (dgd_LeadSetting.Rows[i].Cells[20].Value != null))
                    {
                        if (!((dgd_LeadSetting.Rows[i].Cells[18].Value.ToString() == "---") ||
                        (dgd_LeadSetting.Rows[i].Cells[19].Value.ToString() == "---") ||
                        (dgd_LeadSetting.Rows[i].Cells[20].Value.ToString() == "---")))
                        {
                            fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[18].Value.ToString());
                            fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[19].Value.ToString());
                            fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[20].Value.ToString());
                            //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                            //    dgd_LeadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Red;
                            //else
                            {
                                if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_LeadSetting.Rows[i].Cells.Count < 36)
                                {
                                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                    {
                                        dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.Red;
                                        dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Red;
                                        dgd_LeadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Yellow;
                                        dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Yellow;
                                    }
                                    else
                                    {
                                        dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.Lime;
                                        dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Lime;
                                        dgd_LeadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Black;
                                        dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Black;
                                    }
                                }
                                else
                                {
                                    if (dgd_LeadSetting.Rows[i].Cells.Count > 39 && (dgd_LeadSetting.Rows[i].Cells[39].Value != null))
                                    {
                                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[39].Value.ToString()) - fAccuracySpec;
                                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[19].Value.ToString());
                                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[39].Value.ToString()) + fAccuracySpec;
                                        if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                        {
                                            dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.Red;
                                            dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Red;
                                            dgd_LeadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Yellow;
                                            dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Yellow;
                                        }
                                        else
                                        {
                                            dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.Lime;
                                            dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.Lime;
                                            dgd_LeadSetting.Rows[i].Cells[19].Style.ForeColor = Color.Black;
                                            dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionForeColor = Color.Black;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Variance
                    if (dgd_LeadSetting.Rows[i].Cells[23].Value != null && dgd_LeadSetting.Rows[i].Cells[23].Value.ToString() != "---")
                    {
                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[23].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[22].Value.ToString());
                        if (fResultValue > fMaxValue)
                        {
                            dgd_LeadSetting.Rows[i].Cells[23].Style.BackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionBackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[23].Style.ForeColor = Color.Yellow;
                            dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_LeadSetting.Rows[i].Cells[23].Style.BackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionBackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[23].Style.ForeColor = Color.Black;
                            dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionForeColor = Color.Black;
                        }
                    }

                    // Average Grat=y Value
                    if (dgd_LeadSetting.Rows[i].Cells[26].Value != null && dgd_LeadSetting.Rows[i].Cells[26].Value.ToString() != "---")
                    {
                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[25].Value.ToString());
                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[26].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[27].Value.ToString());
                        if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                        {
                            dgd_LeadSetting.Rows[i].Cells[26].Style.BackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionBackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[26].Style.ForeColor = Color.Yellow;
                            dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_LeadSetting.Rows[i].Cells[26].Style.BackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionBackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[26].Style.ForeColor = Color.Black;
                            dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionForeColor = Color.Black;
                        }
                    }

                    // Base Lead Offset
                    if (dgd_LeadSetting.Rows[i].Cells[30].Value != null && dgd_LeadSetting.Rows[i].Cells[30].Value.ToString() != "---")
                    {
                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[30].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[29].Value.ToString());
                        if (fResultValue > fMaxValue)
                        {
                            dgd_LeadSetting.Rows[i].Cells[30].Style.BackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[30].Style.SelectionBackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[30].Style.ForeColor = Color.Yellow;
                            dgd_LeadSetting.Rows[i].Cells[30].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_LeadSetting.Rows[i].Cells[30].Style.BackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[30].Style.SelectionBackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[30].Style.ForeColor = Color.Black;
                            dgd_LeadSetting.Rows[i].Cells[30].Style.SelectionForeColor = Color.Black;
                        }
                    }

                    // Base Lead Area
                    if (dgd_LeadSetting.Rows[i].Cells[33].Value != null && dgd_LeadSetting.Rows[i].Cells[33].Value.ToString() != "---")
                    {
                        fMinValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[32].Value.ToString());
                        fResultValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[33].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_LeadSetting.Rows[i].Cells[34].Value.ToString());
                        if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                        {
                            dgd_LeadSetting.Rows[i].Cells[33].Style.BackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[33].Style.SelectionBackColor = Color.Red;
                            dgd_LeadSetting.Rows[i].Cells[33].Style.ForeColor = Color.Yellow;
                            dgd_LeadSetting.Rows[i].Cells[33].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_LeadSetting.Rows[i].Cells[33].Style.BackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[33].Style.SelectionBackColor = Color.Lime;
                            dgd_LeadSetting.Rows[i].Cells[33].Style.ForeColor = Color.Black;
                            dgd_LeadSetting.Rows[i].Cells[33].Style.SelectionForeColor = Color.Black;
                        }
                    }

                    #endregion

                }
            }
            if (lbl_ResultMinSpan.Text != "---" && lbl_ResultMaxSpan.Text != "---")
            {
                //Span
                float fMinSpanValue, fMinResultValue, fMaxResultValue, fMaxSpanValue;
                fMinSpanValue = Convert.ToSingle(txt_MinSpan.Text);
                fMaxSpanValue = Convert.ToSingle(txt_MaxSpan.Text);
                fMinResultValue = Convert.ToSingle(lbl_ResultMinSpan.Text);
                fMaxResultValue = Convert.ToSingle(lbl_ResultMaxSpan.Text);

                if (fMinResultValue < fMinSpanValue)
                    lbl_ResultMinSpan.ForeColor = Color.Red;
                else
                    lbl_ResultMinSpan.ForeColor = Color.Black;

                if (fMaxResultValue > fMaxSpanValue)
                    lbl_ResultMaxSpan.ForeColor = Color.Red;
                else
                    lbl_ResultMaxSpan.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Read Lead template data to datagridview
        /// </summary>
        /// <param name="intLeadIndex">Lead position</param>
        /// <param name="dgd_LeadSetting">datagridview</param>
        //private void ReadLeadTemplateDataToGrid(int intLeadIndex, DataGridView dgd_LeadSetting)
        //{
        //    dgd_LeadSetting.Rows.Clear();
        //    string strBlobsFeatures = m_smVisionInfo.g_arrLead[intLeadIndex].GetBlobsFeaturesInspectRealData();
        //    string[] strFeature = strBlobsFeatures.Split('#');
        //    int intBlobsCount = m_smVisionInfo.g_arrLead[intLeadIndex].GetBlobsFeaturesNumber();
        //    int intBlobID = m_smVisionInfo.g_arrLead[intLeadIndex].GetBlobsNoID();
        //    int intFeatureIndex = 0;

        //    for (int i = 0; i < intBlobsCount; i++)
        //    {
        //        dgd_LeadSetting.Rows.Add();
        //        dgd_LeadSetting.Rows[i].HeaderCell.Value = "Lead " + (i + intBlobID);

        //        // Max OffSet
        //        dgd_LeadSetting.Rows[i].Cells[0].Value = strFeature[intFeatureIndex++];

        //        // Max Skew
        //        dgd_LeadSetting.Rows[i].Cells[3].Value = strFeature[intFeatureIndex++];

        //        // Min Max Width
        //        dgd_LeadSetting.Rows[i].Cells[6].Value = strFeature[intFeatureIndex++];
        //        dgd_LeadSetting.Rows[i].Cells[8].Value = strFeature[intFeatureIndex++];

        //        // Min Max Height
        //        dgd_LeadSetting.Rows[i].Cells[10].Value = strFeature[intFeatureIndex++];
        //        dgd_LeadSetting.Rows[i].Cells[12].Value = strFeature[intFeatureIndex++];

        //        // Min Max Pitch
        //        if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
        //            dgd_LeadSetting.Rows[i].Cells[14].Value = "---";
        //        else
        //            dgd_LeadSetting.Rows[i].Cells[14].Value = strFeature[intFeatureIndex];
        //        intFeatureIndex++;

        //        if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
        //            dgd_LeadSetting.Rows[i].Cells[16].Value = "---";
        //        else
        //            dgd_LeadSetting.Rows[i].Cells[16].Value = strFeature[intFeatureIndex];
        //        intFeatureIndex++;

        //        // Min Max Gap
        //        if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
        //            dgd_LeadSetting.Rows[i].Cells[18].Value = "---";
        //        else
        //            dgd_LeadSetting.Rows[i].Cells[18].Value = strFeature[intFeatureIndex];
        //        intFeatureIndex++;

        //        if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
        //            dgd_LeadSetting.Rows[i].Cells[20].Value = "---";
        //        else
        //            dgd_LeadSetting.Rows[i].Cells[20].Value = strFeature[intFeatureIndex];
        //        intFeatureIndex++;

        //        // Max Variance
        //        dgd_LeadSetting.Rows[i].Cells[22].Value = strFeature[intFeatureIndex++];

        //        // Min Max Average Gray Value
        //        dgd_LeadSetting.Rows[i].Cells[25].Value = strFeature[intFeatureIndex++];
        //        dgd_LeadSetting.Rows[i].Cells[27].Value = strFeature[intFeatureIndex++];

        //        // Offset Setting
        //        dgd_LeadSetting.Rows[i].Cells[29].Value = strFeature[intFeatureIndex++];
        //        dgd_LeadSetting.Rows[i].Cells[31].Value = strFeature[intFeatureIndex++];
        //        dgd_LeadSetting.Rows[i].Cells[33].Value = strFeature[intFeatureIndex++];
        //        dgd_LeadSetting.Rows[i].Cells[35].Value = strFeature[intFeatureIndex++];
        //    }

        //    ColorGrid(dgd_LeadSetting);
        //}
        private void ReadLeadTemplateDataToGrid(DataGridView dgd_LeadSetting)
        {
            dgd_LeadSetting.Rows.Clear();
            for (int x = 0; x < m_smVisionInfo.g_arrLead.Length; x++)
            {
                if (!m_smVisionInfo.g_arrLead[x].ref_blnSelected)
                    continue;

                int intLeadFailMask = m_smVisionInfo.g_arrLead[x].ref_intFailOptionMask;

                string strBlobsFeatures = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesInspectRealData();
                string[] strFeature = strBlobsFeatures.Split('#');
                string[] strFeature_BaseLead = { };
                if (m_smVisionInfo.g_arrLead[x].ref_blnWantInspectBaseLead)
                {
                    string strBlobsFeatures_BaseLead = m_smVisionInfo.g_arrLead[x].GetBaseLeadTolerance();
                    strFeature_BaseLead = strBlobsFeatures_BaseLead.Split('#');
                }
                int intBlobsCount = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesNumber();
                int intBlobID = m_smVisionInfo.g_arrLead[x].GetBlobsNoID();
                int intFeatureIndex = 0;
                for (int k = 0; k < intBlobsCount + (intBlobID - 1); k++)
                {
                    if(dgd_LeadSetting.Rows.Count < intBlobsCount + (intBlobID - 1))
                    dgd_LeadSetting.Rows.Add();
                }
                int intFeatureIndex_BaseLead = 0;
                for (int i = intBlobID - 1; i < intBlobsCount + (intBlobID - 1); i++)
                {
                    if (i < 0)
                        continue;

                    dgd_LeadSetting.Rows[i].HeaderCell.Value = "Lead " + (i + 1);

                    // Max OffSet
                    dgd_LeadSetting.Rows[i].Cells[0].Value = strFeature[intFeatureIndex++];
                    if ((intLeadFailMask & 0x100) == 0)
                    {
                        dgd_LeadSetting.Columns[0].Visible = false;
                        dgd_LeadSetting.Columns[1].Visible = false;
                        dgd_LeadSetting.Columns[2].Visible = false;
                    }

                    // Max Skew
                    dgd_LeadSetting.Rows[i].Cells[3].Value = strFeature[intFeatureIndex++];
                    if ((intLeadFailMask & 0x8000) == 0)
                    {
                        dgd_LeadSetting.Columns[3].Visible = false;
                        dgd_LeadSetting.Columns[4].Visible = false;
                        dgd_LeadSetting.Columns[5].Visible = false;
                    }

                    // Min Max Width
                    dgd_LeadSetting.Rows[i].Cells[6].Value = strFeature[intFeatureIndex++];
                    dgd_LeadSetting.Rows[i].Cells[8].Value = strFeature[intFeatureIndex++];
                    if ((intLeadFailMask & 0xC0) == 0)
                    {
                        dgd_LeadSetting.Columns[6].Visible = false;
                        dgd_LeadSetting.Columns[7].Visible = false;
                        dgd_LeadSetting.Columns[8].Visible = false;
                        dgd_LeadSetting.Columns[9].Visible = false;
                    }

                    // Min Max Height
                    dgd_LeadSetting.Rows[i].Cells[10].Value = strFeature[intFeatureIndex++];
                    dgd_LeadSetting.Rows[i].Cells[12].Value = strFeature[intFeatureIndex++];
                    if ((intLeadFailMask & 0xC0) == 0)
                    {
                        dgd_LeadSetting.Columns[10].Visible = false;
                        dgd_LeadSetting.Columns[11].Visible = false;
                        dgd_LeadSetting.Columns[12].Visible = false;
                        dgd_LeadSetting.Columns[13].Visible = false;
                    }

                    // Min Max Pitch
                    if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
                        dgd_LeadSetting.Rows[i].Cells[14].Value = "---";
                    else
                        dgd_LeadSetting.Rows[i].Cells[14].Value = strFeature[intFeatureIndex];
                    intFeatureIndex++;

                    if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
                        dgd_LeadSetting.Rows[i].Cells[16].Value = "---";
                    else
                        dgd_LeadSetting.Rows[i].Cells[16].Value = strFeature[intFeatureIndex];
                    intFeatureIndex++;
                    if ((intLeadFailMask & 0x600) == 0)
                    {
                        dgd_LeadSetting.Columns[14].Visible = false;
                        dgd_LeadSetting.Columns[15].Visible = false;
                        dgd_LeadSetting.Columns[16].Visible = false;
                        dgd_LeadSetting.Columns[17].Visible = false;
                    }

                    // Min Max Gap
                    if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
                        dgd_LeadSetting.Rows[i].Cells[18].Value = "---";
                    else
                        dgd_LeadSetting.Rows[i].Cells[18].Value = strFeature[intFeatureIndex];
                    intFeatureIndex++;

                    if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
                        dgd_LeadSetting.Rows[i].Cells[20].Value = "---";
                    else
                        dgd_LeadSetting.Rows[i].Cells[20].Value = strFeature[intFeatureIndex];
                    intFeatureIndex++;
                    if ((intLeadFailMask & 0x600) == 0)
                    {
                        dgd_LeadSetting.Columns[18].Visible = false;
                        dgd_LeadSetting.Columns[19].Visible = false;
                        dgd_LeadSetting.Columns[20].Visible = false;
                        dgd_LeadSetting.Columns[21].Visible = false;
                    }

                    // Max Variance
                    dgd_LeadSetting.Rows[i].Cells[22].Value = strFeature[intFeatureIndex++];
                    //if ((intLeadFailMask & 0x800) == 0)
                    {
                        dgd_LeadSetting.Columns[22].Visible = false;
                        dgd_LeadSetting.Columns[23].Visible = false;
                        dgd_LeadSetting.Columns[24].Visible = false;
                    }

                    // Min Max Average Gray Value
                    dgd_LeadSetting.Rows[i].Cells[25].Value = strFeature[intFeatureIndex++];
                    dgd_LeadSetting.Rows[i].Cells[27].Value = strFeature[intFeatureIndex++];
                    if (!m_smVisionInfo.g_arrLead[x].ref_blnWantUseAverageGrayValueMethod || ((intLeadFailMask & 0x4000) == 0))
                    {
                        dgd_LeadSetting.Columns[25].Visible = false;
                        dgd_LeadSetting.Columns[26].Visible = false;
                        dgd_LeadSetting.Columns[27].Visible = false;
                        dgd_LeadSetting.Columns[28].Visible = false;
                    }

                    //Base Lead
                    if (m_smVisionInfo.g_arrLead[x].ref_blnWantInspectBaseLead)
                    {
                        dgd_LeadSetting.Rows[i].Cells[29].Value = strFeature_BaseLead[intFeatureIndex_BaseLead++];
                        dgd_LeadSetting.Rows[i].Cells[32].Value = strFeature_BaseLead[intFeatureIndex_BaseLead++];
                        dgd_LeadSetting.Rows[i].Cells[34].Value = strFeature_BaseLead[intFeatureIndex_BaseLead++];

                        if ((intLeadFailMask & 0x10000) == 0)
                        {
                            dgd_LeadSetting.Columns[29].Visible = false;
                            dgd_LeadSetting.Columns[30].Visible = false;
                            dgd_LeadSetting.Columns[31].Visible = false;
                        }

                        if ((intLeadFailMask & 0x20000) == 0)
                        {
                            dgd_LeadSetting.Columns[32].Visible = false;
                            dgd_LeadSetting.Columns[33].Visible = false;
                            dgd_LeadSetting.Columns[34].Visible = false;
                            dgd_LeadSetting.Columns[35].Visible = false;
                        }
                    }
                    else
                    {
                        dgd_LeadSetting.Columns[29].Visible = false;
                        dgd_LeadSetting.Columns[30].Visible = false;
                        dgd_LeadSetting.Columns[31].Visible = false;
                        dgd_LeadSetting.Columns[32].Visible = false;
                        dgd_LeadSetting.Columns[33].Visible = false;
                        dgd_LeadSetting.Columns[34].Visible = false;
                        dgd_LeadSetting.Columns[35].Visible = false;
                    }

                    //// Offset Setting
                    //dgd_LeadSetting.Rows[i].Cells[36].Value = strFeature[intFeatureIndex++];
                    //dgd_LeadSetting.Rows[i].Cells[38].Value = strFeature[intFeatureIndex++];
                    //if ((intLeadFailMask & 0xC0) == 0)
                    //{
                    //    dgd_LeadSetting.Columns[36].Visible = false;
                    //    dgd_LeadSetting.Columns[37].Visible = false;
                    //    dgd_LeadSetting.Columns[38].Visible = false;
                    //    dgd_LeadSetting.Columns[39].Visible = false;
                    //}

                    //dgd_LeadSetting.Rows[i].Cells[40].Value = strFeature[intFeatureIndex++];
                    //dgd_LeadSetting.Rows[i].Cells[42].Value = strFeature[intFeatureIndex++];
                    //if ((intLeadFailMask & 0x600) == 0)
                    //{
                    //    dgd_LeadSetting.Columns[40].Visible = false;
                    //    dgd_LeadSetting.Columns[41].Visible = false;
                    //    dgd_LeadSetting.Columns[42].Visible = false;
                    //    dgd_LeadSetting.Columns[43].Visible = false;
                    //}

                }
            }
            ColorGrid(dgd_LeadSetting);
        }
        private void ColorGrid(DataGridView dgd_LeadSetting)
        {
            for (int i = 0; i < dgd_LeadSetting.Rows.Count; i++)
            {
                // OffSet
                dgd_LeadSetting.Rows[i].Cells[0].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.LightGray;

                // Skew
                dgd_LeadSetting.Rows[i].Cells[3].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.LightGray;

                // Width
                dgd_LeadSetting.Rows[i].Cells[6].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[6].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[8].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.White;

                // Length
                dgd_LeadSetting.Rows[i].Cells[10].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[10].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[12].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.White;

                // Pitch
                dgd_LeadSetting.Rows[i].Cells[14].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[14].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[16].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[16].Style.SelectionBackColor = Color.White;

                // Gap
                dgd_LeadSetting.Rows[i].Cells[18].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[18].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[20].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[20].Style.SelectionBackColor = Color.White;

                // Variance
                dgd_LeadSetting.Rows[i].Cells[22].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[22].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[23].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[23].Style.SelectionBackColor = Color.LightGray;
                
                // Average Gray Value
                dgd_LeadSetting.Rows[i].Cells[25].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[25].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[26].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[26].Style.SelectionBackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[27].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[27].Style.SelectionBackColor = Color.White;

                //// Offset Setting
                //dgd_LeadSetting.Rows[i].Cells[36].Style.BackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[36].Style.SelectionBackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[38].Style.BackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[38].Style.SelectionBackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[40].Style.BackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[40].Style.SelectionBackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[42].Style.BackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[42].Style.SelectionBackColor = Color.White;

                // Base Lead Offset
                dgd_LeadSetting.Rows[i].Cells[29].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[29].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[30].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[30].Style.SelectionBackColor = Color.LightGray;

                // Base Lead Area
                dgd_LeadSetting.Rows[i].Cells[32].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[32].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[33].Style.BackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[33].Style.SelectionBackColor = Color.LightGray;
                dgd_LeadSetting.Rows[i].Cells[34].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[34].Style.SelectionBackColor = Color.White;

            }
        }

        private void LoadLeadSetting(string strFolderPath)
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

                m_smVisionInfo.g_arrLead[i].LoadLead(strFolderPath + "Template\\Template.xml", strSectionName, m_smVisionInfo.g_arrImages.Count);

                m_smVisionInfo.g_arrLead[i].LoadLeadTemplateImage(strFolderPath + "Template\\", i);

            }
        }

        private bool SaveLeadSettings(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
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

                
                m_smVisionInfo.g_arrLead[i].SaveLead(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                
            }
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead Tolerance Setting", m_smProductionInfo.g_strLotID);
            
            return true;
        }

        private void UpdateLeadMeasurementIntoTable()
        {

        }


        private void LeadToleranceSettingForm_Load(object sender, EventArgs e)
        {
            m_blnFormOpen = true;
        }

        private void LeadToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.AT_VM_OfflineTestAllLead = false;
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead Tolerance Setting Closed", "Exit Lead Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_blnFormOpen = false;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            // Save Lead Setting
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\";

            SaveLeadSettings(strPath + "Lead\\");

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            if (IsSettingError())
            {
                SRMMessageBox.Show("Set minimum value or maximum value is not corrects. Please check the red highlight value is correct or not.");
                return;

            }

            //CheckSaveGoldenData(m_intLeadIndex, m_dgdView[0]);
            // Load Lead Setting
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            LoadLeadSetting(strFolderPath + "Lead\\");

            this.Close();
            this.Dispose();
        }

        private void radioBtn_LeadIndex_Click(object sender, EventArgs e)
        {
           // if (IsSettingError())
           // {
           //     SRMMessageBox.Show("Set minimum value or maximum value is not corrects. Please check the red highlight value is correct or not.");

           //     switch (m_intLeadIndex)
           //     {
           //         case 1:
           //             radioBtn_Up.Checked = true;
           //             break;
           //         case 2:
           //             radioBtn_Right.Checked = true;
           //             break;
           //         case 3:
           //             radioBtn_Down.Checked = true;
           //             break;
           //         case 4:
           //             radioBtn_Left.Checked = true;
           //             break;
           //     }
           //     return;
           // }

           // //CheckSaveGoldenData(m_intLeadIndex, m_dgdView[0]);

           // if (sender == radioBtn_Up)
           // {
           //     tabControl_Lead.SelectedIndex = m_intLeadIndex = 1;
           // }
           // else if (sender == radioBtn_Right)
           // {
           //     tabControl_Lead.SelectedIndex = m_intLeadIndex = 2;
           // }
           // else if (sender == radioBtn_Down)
           // {
           //     tabControl_Lead.SelectedIndex = m_intLeadIndex = 3;
           // }
           // else if (sender == radioBtn_Left)
           // {
           //     tabControl_Lead.SelectedIndex = m_intLeadIndex = 4;
           // }

           // tabControl_Lead.SelectedIndex = 0;

           // ReadLeadTemplateDataToGrid(m_intLeadIndex, m_dgdView[0]);

           // if (objGoldenUnitForm != null)
           // {
           //     if (objGoldenUnitForm.ref_intViewGoldenDataColumn)
           //     {
           //         if (m_dgdView[0].Columns.Count <= 25)
           //         {
           //             m_dgdView[0].Columns.Add(column_GolWidth);
           //             m_dgdView[0].Columns.Add(column_GolLength);
           //             m_dgdView[0].Columns.Add(column_GolPitch);
           //             m_dgdView[0].Columns.Add(column_GolGap);
           //         }
           //     }
           // }

           ////UpdateGoldenDataIntoGridTable(m_intLeadIndex, m_dgdView[0]);

           // UpdateScore(m_intLeadIndex, m_dgdView[0]);

        }      

        public bool IsSettingError()
        {
            int[] arrColumnIndex = { 6, 10, 14, 18, 25, 32};

            for (int c = 0; c < arrColumnIndex.Length; c++)
            {
                for (int i = 0; i < m_dgdView[0].Rows.Count; i++)
                {
                    if (m_dgdView[0].Rows[i].Cells[arrColumnIndex[c]].Value != null && m_dgdView[0].Rows[i].Cells[arrColumnIndex[c]].Value.ToString() != "---")
                    {
                        // Check insert data valid or not
                        float fMin = float.Parse(m_dgdView[0].Rows[i].Cells[arrColumnIndex[c]].Value.ToString());
                        float fMax = float.Parse(m_dgdView[0].Rows[i].Cells[arrColumnIndex[c] + 2].Value.ToString());

                        if (fMin > fMax)
                        {
                            return true;
                        }
                    }

                    ////Update template pitch gap setting
                    //m_smVisionInfo.g_arrLead[m_intLeadIndex].SetPitchGapDataFrom(i,
                    //   fMinPitch,
                    //   fMaxPitch,
                    //   fMinGap,
                    //   fMaxGap);
                }
            }

            return false;
        }

        private void ViewOrHideResultColumn(bool blnWantView)
        {
            int intLeadFailMask = m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask;

            if ((intLeadFailMask & 0x100) == 0)
                m_dgdView[0].Columns[1].Visible = false;
            else
                m_dgdView[0].Columns[1].Visible = blnWantView;

            if ((intLeadFailMask & 0x8000) == 0)
                m_dgdView[0].Columns[4].Visible = false;
            else
                m_dgdView[0].Columns[4].Visible = blnWantView;

            if ((intLeadFailMask & 0xC0) == 0)
            {
                m_dgdView[0].Columns[7].Visible = false;
                m_dgdView[0].Columns[11].Visible = false;
            }
            else
            {
                m_dgdView[0].Columns[7].Visible = blnWantView;
                m_dgdView[0].Columns[11].Visible = blnWantView;
            }

            if ((intLeadFailMask & 0x600) == 0)
            {
                m_dgdView[0].Columns[15].Visible = false;
                m_dgdView[0].Columns[19].Visible = false;
            }
            else
            {
                m_dgdView[0].Columns[15].Visible = blnWantView;
                m_dgdView[0].Columns[19].Visible = blnWantView;
            }

            //m_dgdView[0].Columns[23].Visible = false;// blnWantView;
            if (m_smVisionInfo.g_arrLead[0].ref_blnWantUseAverageGrayValueMethod || ((intLeadFailMask & 0x4000) > 0))
                m_dgdView[0].Columns[26].Visible = blnWantView;
            else
                m_dgdView[0].Columns[26].Visible = false;
            if (m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
            {
                if ((intLeadFailMask & 0x10000) > 0)
                    m_dgdView[0].Columns[30].Visible = blnWantView;
                else
                    m_dgdView[0].Columns[30].Visible = false;

                if ((intLeadFailMask & 0x20000) > 0)
                    m_dgdView[0].Columns[33].Visible = blnWantView;
                else
                    m_dgdView[0].Columns[33].Visible = false;
            }
            else
            {
                m_dgdView[0].Columns[30].Visible = false;
                m_dgdView[0].Columns[33].Visible = false;
            }
            //lbl_ResultMinSpanText.Visible = blnWantView;
            //lbl_ResultMaxSpanText.Visible = blnWantView;
            //lbl_ResultMinSpan.Visible = blnWantView;
            //lbl_ResultMaxSpan.Visible = blnWantView;
            //lbl_UnitDisplay1.Visible = blnWantView;
            //lbl_UnitDisplay2.Visible = blnWantView;

            // Whole Leads Setting Table
            dgd_WholeLeadSetting.Columns[3].Visible = blnWantView;
        }

        private void timer_LeadResult_Tick(object sender, EventArgs e)
        {
            if (m_smVisionInfo.PR_MN_UpdateInfo)//(m_blnUpdateInfo)
            {
                m_smVisionInfo.PR_MN_UpdateInfo = false;
                UpdateScore(m_dgdView[0]); //UpdateScore(m_intLeadIndex, m_dgdView[0]);
                UpdateWholeLeadScore();
                //m_blnUpdateInfo = false;
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
        }

        private void chk_ViewGoldenColumn_Click(object sender, EventArgs e)
        {
            //if (objGoldenUnitForm.ref_intViewGoldenDataColumn)
            //{
            //    m_dgdView[0].Columns.Add(column_GolWidth);
            //    m_dgdView[0].Columns.Add(column_GolLength);
            //    m_dgdView[0].Columns.Add(column_GolPitch);
            //    m_dgdView[0].Columns.Add(column_GolGap);

            //    UpdateGoldenDataIntoGridTable(m_intLeadIndex, m_dgdView[0]);
            //}
            //else
            //{
            //    m_dgdView[0].Columns.Remove(column_GolLength);
            //    m_dgdView[0].Columns.Remove(column_GolWidth);
            //    m_dgdView[0].Columns.Remove(column_GolPitch);
            //    m_dgdView[0].Columns.Remove(column_GolGap);

            //    CheckSaveGoldenData(m_intLeadIndex, m_dgdView[0]);
            //}
        }

        private void dgd_MiddleLead_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            //if (!m_blnInitDone)
            //    return;

            //if (e.RowIndex >= m_dgdView[0].Rows.Count)
            //    return;

            //if (e.ColumnIndex < 0)
            //    return;

            //float fValue;
            //if (m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            //{
            //    if (float.TryParse(m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
            //    {
            //        m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = fValue;
            //    }
            //    else if (m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "---")
            //    {
            //    }
            //    else
            //    {
            //        SRMMessageBox.Show("Please key in correct number format.");
            //        m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";
            //    }
            //}
        }

        private void btn_GoldenUnitSetting_Click(object sender, EventArgs e)
        {
            //if (objGoldenUnitForm == null)
            //{
            //    objGoldenUnitForm = new GoldenUnitCompensationForm(m_smCustomizeInfo,
            //                        m_smVisionInfo, m_strSelectedRecipe, m_intUserGroup);
            //}

            //CheckSaveGoldenData(m_intLeadIndex, m_dgdView[0]);

            //objGoldenUnitForm.SetSelectedLeadIndex(m_intLeadIndex);

            //objGoldenUnitForm.TopMost = true;
            //objGoldenUnitForm.ShowDialog();

            //SaveGoldenData();

            //m_intGDSelectedIndex = objGoldenUnitForm.ref_intGoldenUnitSelectedIndex;

            //if (m_intGDSelectedIndex >= 0 && objGoldenUnitForm.ref_intViewGoldenDataColumn)
            //{
            //    if (m_dgdView[0].Columns.Count <= 25)
            //    {
            //        m_dgdView[0].Columns.Add(column_GolWidth);
            //        m_dgdView[0].Columns.Add(column_GolLength);
            //        m_dgdView[0].Columns.Add(column_GolPitch);
            //        m_dgdView[0].Columns.Add(column_GolGap);
            //    }
            //    UpdateGoldenDataIntoGridTable(m_intLeadIndex, m_dgdView[0]);

            //    UpdateScore(m_intLeadIndex, m_dgdView[0]);

            //    btn_SaveAccuracyReport.Visible = true;
            //}
            //else
            //{
            //    if (m_dgdView[0].Columns.Count > 25)
            //    {
            //        m_dgdView[0].Columns.Remove(column_GolLength);
            //        m_dgdView[0].Columns.Remove(column_GolWidth);
            //        m_dgdView[0].Columns.Remove(column_GolPitch);
            //        m_dgdView[0].Columns.Remove(column_GolGap);
            //    }
            //    btn_SaveAccuracyReport.Visible = false;
            //}
        }

        private void btn_SaveAccuracyReport_Click(object sender, EventArgs e)
        {
            if (dlg_SaveTextFile.ShowDialog() == DialogResult.OK)
            {
                FileInfo file = new FileInfo(dlg_SaveTextFile.FileName);
                if (file.Exists)
                {
                    FileStream stream = null;
                    try
                    {
                        stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    }
                    catch (Exception ex)
                    {
                        SRMMessageBox.Show("The selected file is being used. Please close the file first before replace it.");
                        return;

                    }

                    if (stream != null)
                    {
                        stream.Close();
                    }
                }

                List<string> arrData = new List<string>();

                arrData.Add("Golden Unit Accuracy Report");
                arrData.Add("");
                arrData.Add("Total LeadS: " + m_dgdView[0].Rows.Count.ToString());
                arrData.Add("");

                float fGoldenUnitValue = 0;
                float fVisionValue = 0;
                for (int i = 0; i < m_dgdView[0].Rows.Count; i++)
                {
                    arrData.Add("Lead " + (i + 1).ToString().PadRight(2, ' ') +
                        "\t\tGolden Unit    \t\tVision System  \t\tDeviation");

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[36].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[7].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Width \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[37].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[11].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Length\t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[38].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[15].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Pitch \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[39].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[19].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Gap   \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    arrData.Add("");
                }

                PdfWriter objPdfWriter = new PdfWriter(450, 660, 20, 10);
                objPdfWriter.Write(arrData.ToArray(), dlg_SaveTextFile.FileName);
            }
        }

        private void dgd_TopLead_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex == 1 || e.ColumnIndex == 4 || e.ColumnIndex == 7 || e.ColumnIndex == 11 ||
                e.ColumnIndex == 15 || e.ColumnIndex == 19 || e.ColumnIndex == 23 || e.ColumnIndex == 26 || e.ColumnIndex == 30 || e.ColumnIndex == 33)
                return;

            // Skip if col is separation
            if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 9 || e.ColumnIndex == 13 ||
                e.ColumnIndex == 17 || e.ColumnIndex == 21 || e.ColumnIndex == 24 || e.ColumnIndex == 28 || e.ColumnIndex == 31 || e.ColumnIndex == 35)// ||
                //e.ColumnIndex == 37 || e.ColumnIndex == 39 || e.ColumnIndex == 41 || e.ColumnIndex == 43)
                return;

            // Skip if col is golden unit data
            if (e.ColumnIndex == 36 || e.ColumnIndex == 37 || e.ColumnIndex == 38 || e.ColumnIndex == 39)
                return;

            //Skip if cell value is ---
            if ((((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "---") ||
                (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == ""))
                return;


            ////Min, max area, broken area has 6 decimal places 
            //if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
            //{
            //    switch (m_smCustomizeInfo.g_intUnitDisplay)
            //    {
            //        default:
            //        case 1:
            //            m_strUnitLabel = "mm^2";
            //            m_intDecimalPlaces = 6;
            //            break;
            //        case 2:
            //            m_strUnitLabel = "mil^2";
            //            m_intDecimalPlaces = 6;
            //            break;
            //        case 3:
            //            m_strUnitLabel = "um^2";
            //            m_intDecimalPlaces = 2;
            //            break;
            //    }
            //}
            //else
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
            string strUnitLabel = m_strUnitLabel;
            if (e.ColumnIndex == 25 || e.ColumnIndex == 27) // no unit label for average gray value
            {
                strUnitLabel = "";
            }

            string strCurrentSetValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            //SetValueForm objSetValueForm = new SetValueForm(e.RowIndex, intDecimalPlaces, strCurrentSetValue);
            SetValueForm objSetValueForm = new SetValueForm("Set value to Lead " + (e.RowIndex + 1).ToString(), strUnitLabel, e.RowIndex, m_intDecimalPlaces, strCurrentSetValue);
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
                        if (e.ColumnIndex == 6 || e.ColumnIndex == 10 || e.ColumnIndex == 14 || e.ColumnIndex == 18 || e.ColumnIndex == 25 || e.ColumnIndex == 32)
                        {
                            float fMax = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Value.ToString());

                            if (objSetValueForm.ref_fSetValue > fMax)
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.SelectionBackColor = Color.Pink;
                            }
                            else
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.SelectionBackColor = Color.White;
                            }
                        }
                        else if (e.ColumnIndex == 8 || e.ColumnIndex == 12 || e.ColumnIndex == 16 || e.ColumnIndex == 20 || e.ColumnIndex == 27 || e.ColumnIndex == 34)
                        {
                            float fMin = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Value.ToString());
                            if (fMin > objSetValueForm.ref_fSetValue)
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.SelectionBackColor = Color.Pink;
                            }
                            else
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.SelectionBackColor = Color.White;
                            }
                        }

                        //// Set new insert value into table
                        //if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
                        //    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F6");
                        //else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                    }

                    //Set value column selected
                    float fMinPitch, fMaxPitch;
                    if (((DataGridView)sender).Rows[i].Cells[14].Value.ToString() == "---")
                        fMinPitch = -1;
                    else
                        fMinPitch = float.Parse(((DataGridView)sender).Rows[i].Cells[14].Value.ToString());

                    if (((DataGridView)sender).Rows[i].Cells[16].Value.ToString() == "---")
                        fMaxPitch = -1;
                    else
                        fMaxPitch = float.Parse(((DataGridView)sender).Rows[i].Cells[16].Value.ToString());

                    //Set value column selected
                    float fMinGap, fMaxGap;
                    if (((DataGridView)sender).Rows[i].Cells[18].Value.ToString() == "---")
                        fMinGap = -1;
                    else
                        fMinGap = float.Parse(((DataGridView)sender).Rows[i].Cells[18].Value.ToString());

                    if (((DataGridView)sender).Rows[i].Cells[20].Value.ToString() == "---")
                        fMaxGap = -1;
                    else
                        fMaxGap = float.Parse(((DataGridView)sender).Rows[i].Cells[20].Value.ToString());

                    //int intLengthMode = m_smVisionInfo.g_arrLead[m_intLeadIndex].GetSampleLengthMode(i);
                    for (int x = 0; x < m_smVisionInfo.g_arrLead.Length; x++)
                    {
                        if (!m_smVisionInfo.g_arrLead[x].ref_blnSelected)
                            continue;

                        int intBlobsCount = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesNumber();
                        int intBlobIndex = 0;
                        //int intStartIndex = m_smVisionInfo.g_arrLead[x].GetBlobsNoID();
                        bool blnFound = false;
                        for (int k = 0; k < intBlobsCount; k++) // for (int k = intStartIndex - 1; k < intBlobsCount + (intStartIndex - 1); k++)
                        {
                            int intBlobID = m_smVisionInfo.g_arrLead[x].GetBlobsNoID(k);
                            if (intBlobID == (i + 1))
                            {
                                intBlobIndex = k;
                                blnFound = true;
                                break;
                            }
                        }
                        //Update template setting
                        //if (intLengthMode == 1)
                        //{
                        if (blnFound)
                        {
                            m_smVisionInfo.g_arrLead[x].UpdateBlobFeatureToPixel(intBlobIndex,
                                float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[6].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[8].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[10].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[12].Value.ToString()),
                                fMinPitch,
                                fMaxPitch,
                                fMinGap,
                                fMaxGap,
                                float.Parse(((DataGridView)sender).Rows[i].Cells[22].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[25].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[27].Value.ToString())//,
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[36].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[38].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[40].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[42].Value.ToString())
                                );

                            if (m_smVisionInfo.g_arrLead[x].ref_blnWantInspectBaseLead)
                            {
                                m_smVisionInfo.g_arrLead[x].UpdateBlobFeatureToPixel_BaseLead(intBlobIndex,
                                    float.Parse(((DataGridView)sender).Rows[i].Cells[29].Value.ToString()),
                                    float.Parse(((DataGridView)sender).Rows[i].Cells[32].Value.ToString()),
                                    float.Parse(((DataGridView)sender).Rows[i].Cells[34].Value.ToString())
                                    );
                            }
                            //}
                            //else
                            //{
                            //    m_smVisionInfo.g_arrLead[m_intLeadIndex].UpdateBlobFeatureToPixel(i,
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                            //        fMinPitch,
                            //        fMaxPitch,
                            //        fMinGap,
                            //        fMaxGap,
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[19].Value.ToString())
                            //        );
                            //}

                            //Update template pitch gap setting
                            m_smVisionInfo.g_arrLead[x].SetPitchGapDataFrom(intBlobIndex,
                               fMinPitch,
                               fMaxPitch,
                               fMinGap,
                               fMaxGap);
                        }
                    }

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }

            this.TopMost = true;
        }
        

        private void dgd_TopLead_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txt_MinSpan_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            try
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_fTemplateLeadMinSpanLimit = Convert.ToSingle(txt_MinSpan.Text);
                }
            }
            catch
            {
            }
        }

        private void txt_MaxSpan_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            try
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].ref_fTemplateLeadMaxSpanLimit = Convert.ToSingle(txt_MaxSpan.Text);
                }
            }
            catch
            {
            }
        }

        private void chk_DisplayResult_Click(object sender, EventArgs e)
        {
            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("CurrentDisplayResult_LeadTolerance", chk_DisplayResult.Checked);
        }

        private void btn_LoadTolFromFile_Click(object sender, EventArgs e)
        {
            if (dlg_LoadToleranceFile.ShowDialog() == DialogResult.OK)
            {
                string strFileName = dlg_LoadToleranceFile.FileName;

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

                    m_smVisionInfo.g_arrLead[i].LoadLeadToleranceFromFile(dlg_LoadToleranceFile.FileName, strSectionName);

                    // Update table
                    ReadLeadTemplateDataToGrid(m_dgdView[0]); //ReadLeadTemplateDataToGrid(m_intLeadIndex, m_dgdView[0]);
                    UpdateScore(m_dgdView[0]); //    UpdateScore(m_intLeadIndex, m_dgdView[0]);
                    UpdateWholeLeadScore();
                    txt_MinSpan.Text = m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
                    txt_MaxSpan.Text = m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);
                }
            }

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void btn_SaveTolToFile_Click(object sender, EventArgs e)
        {
            
            STDeviceEdit.CopySettingFile(dlg_SaveToleranceFile.FileName, "");
            if (dlg_SaveToleranceFile.ShowDialog() == DialogResult.OK)
            {
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

                    
                    m_smVisionInfo.g_arrLead[i].SaveLeadToleranceToFile(dlg_SaveToleranceFile.FileName, false, strSectionName, true);
                    
                }
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead Tolerance Setting", m_smProductionInfo.g_strLotID);
                
            }

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void btn_UpdateTolerance_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            Point p = btn_UpdateTolerance.PointToScreen(Point.Empty);
            LeadUpdateToleranceUpperLowerLimitForm objForm = new LeadUpdateToleranceUpperLowerLimitForm(m_smVisionInfo, m_smCustomizeInfo, m_strSelectedRecipe, m_intUserGroup,m_smProductionInfo, false);
            objForm.StartPosition = FormStartPosition.Manual;
            int intMainFormLocationX = 0;
            int intMainFormLocationY = 0;
            objForm.Location = GetFormStartPoint(p, btn_UpdateTolerance.Size, objForm.Size, ref intMainFormLocationX, ref intMainFormLocationY);

            if (objForm.ShowDialog() == DialogResult.Yes)
            {
                int[] intResultColIndex = { 7, 11, 15, 19}; // Width, Length, Pitch, Gap

                for (int i = 0; i < m_dgdView[0].Rows.Count; i++)
                {
                    for (int c = 0; c < intResultColIndex.Length; c++)
                    {

                        if (m_dgdView[0].Rows[i].Cells[intResultColIndex[c]].Value.ToString() != "---")
                        {
                            float fNorminalResult = float.Parse(m_dgdView[0].Rows[i].Cells[intResultColIndex[c]].Value.ToString());

                            if (fNorminalResult != 0)
                            {
                                switch (intResultColIndex[c])
                                {
                                    case 7:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = Math.Round(fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100, 4, MidpointRounding.AwayFromZero);
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = Math.Round(fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100, 4, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                    case 11:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = Math.Round(fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100, 4, MidpointRounding.AwayFromZero);
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = Math.Round(fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100, 4, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                    case 15:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = Math.Round(fNorminalResult * objForm.ref_fLowerLimit_Pitch / 100, 4, MidpointRounding.AwayFromZero);
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = Math.Round(fNorminalResult * objForm.ref_fUpperLimit_Pitch / 100, 4, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                    case 19:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = Math.Round(fNorminalResult * objForm.ref_fLowerLimit_Gap / 100, 4, MidpointRounding.AwayFromZero);
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = Math.Round(fNorminalResult * objForm.ref_fUpperLimit_Gap / 100, 4, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    //Set value column selected
                    float fMinPitch, fMaxPitch;
                    if ((m_dgdView[0].Rows[i].Cells[14].Value.ToString() == "---"))
                        fMinPitch = -1;
                    else
                        fMinPitch = float.Parse(m_dgdView[0].Rows[i].Cells[14].Value.ToString());

                    if (m_dgdView[0].Rows[i].Cells[16].Value.ToString() == "---")
                        fMaxPitch = -1;
                    else
                        fMaxPitch = float.Parse(m_dgdView[0].Rows[i].Cells[16].Value.ToString());

                    //Set value column selected
                    float fMinGap, fMaxGap;
                    if ((m_dgdView[0].Rows[i].Cells[18].Value.ToString() == "---"))
                        fMinGap = -1;
                    else
                        fMinGap = float.Parse(m_dgdView[0].Rows[i].Cells[18].Value.ToString());

                    if (m_dgdView[0].Rows[i].Cells[20].Value.ToString() == "---")
                        fMaxGap = -1;
                    else
                        fMaxGap = float.Parse(m_dgdView[0].Rows[i].Cells[20].Value.ToString());

                    //int intLengthMode = m_smVisionInfo.g_arrLead[m_intLeadIndex].GetSampleLengthMode(i);
                    for (int x = 0; x < m_smVisionInfo.g_arrLead.Length; x++)
                    {
                        if (!m_smVisionInfo.g_arrLead[x].ref_blnSelected)
                            continue;

                        int intBlobsCount = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesNumber();
                        int intBlobIndex = 0;
                        //int intStartIndex = m_smVisionInfo.g_arrLead[x].GetBlobsNoID();
                        bool blnFound = false;
                        for (int k = 0; k < intBlobsCount; k++)
                        {
                            int intBlobID = m_smVisionInfo.g_arrLead[x].GetBlobsNoID(k);
                            if (intBlobID == (i + 1))
                            {
                                intBlobIndex = k;
                                blnFound = true;
                                break;
                            }
                        }
                        //Update template setting
                        //if (intLengthMode == 1)
                        //{
                        if (blnFound)
                        {
                            m_smVisionInfo.g_arrLead[x].UpdateBlobFeatureToPixel(intBlobIndex,
                            float.Parse(m_dgdView[0].Rows[i].Cells[0].Value.ToString()),
                            float.Parse(m_dgdView[0].Rows[i].Cells[3].Value.ToString()),
                            float.Parse(m_dgdView[0].Rows[i].Cells[6].Value.ToString()),
                            float.Parse(m_dgdView[0].Rows[i].Cells[8].Value.ToString()),
                            float.Parse(m_dgdView[0].Rows[i].Cells[10].Value.ToString()),
                            float.Parse(m_dgdView[0].Rows[i].Cells[12].Value.ToString()),
                            fMinPitch,
                            fMaxPitch,
                            fMinGap,
                            fMaxGap,
                            float.Parse(m_dgdView[0].Rows[i].Cells[22].Value.ToString()),
                            float.Parse(m_dgdView[0].Rows[i].Cells[25].Value.ToString()),
                            float.Parse(m_dgdView[0].Rows[i].Cells[27].Value.ToString())//,
                            //float.Parse(m_dgdView[0].Rows[i].Cells[36].Value.ToString()),
                            //float.Parse(m_dgdView[0].Rows[i].Cells[38].Value.ToString()),
                            //float.Parse(m_dgdView[0].Rows[i].Cells[40].Value.ToString()),
                            //float.Parse(m_dgdView[0].Rows[i].Cells[42].Value.ToString())
                            );

                            if (m_smVisionInfo.g_arrLead[x].ref_blnWantInspectBaseLead)
                            {
                                m_smVisionInfo.g_arrLead[x].UpdateBlobFeatureToPixel_BaseLead(intBlobIndex,
                                float.Parse(m_dgdView[0].Rows[i].Cells[29].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[32].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[34].Value.ToString())
                                );
                            }
                            //}
                            //else
                            //{
                            //    m_smVisionInfo.g_arrLead[m_intLeadIndex].UpdateBlobFeatureToPixel(i,
                            //        float.Parse(m_dgdView[0].Rows[i].Cells[0].Value.ToString()),
                            //        float.Parse(m_dgdView[0].Rows[i].Cells[7].Value.ToString()),
                            //        float.Parse(m_dgdView[0].Rows[i].Cells[9].Value.ToString()),
                            //        float.Parse(m_dgdView[0].Rows[i].Cells[3].Value.ToString()),
                            //        float.Parse(m_dgdView[0].Rows[i].Cells[5].Value.ToString()),
                            //        fMinPitch,
                            //        fMaxPitch,
                            //        fMinGap,
                            //        fMaxGap,
                            //        float.Parse(m_dgdView[0].Rows[i].Cells[19].Value.ToString())
                            //        );
                            //}

                            //Update template pitch gap setting
                            m_smVisionInfo.g_arrLead[x].SetPitchGapDataFrom(intBlobIndex,
                               fMinPitch,
                               fMaxPitch,
                               fMinGap,
                               fMaxGap);
                        }
                    }

                }

                //float fNorminalMinSpanResult = 0;
                //float fNorminalMaxSpanResult = 0;

                //if (lbl_ResultMinSpan.Text != "---")
                //    fNorminalMinSpanResult = (float)Math.Round(float.Parse(lbl_ResultMinSpan.Text) * objForm.ref_fLowerLimit_Span / 100, 4, MidpointRounding.AwayFromZero);

                //if (lbl_ResultMaxSpan.Text != "---")
                //    fNorminalMaxSpanResult = (float)Math.Round(float.Parse(lbl_ResultMaxSpan.Text) * objForm.ref_fUpperLimit_Span / 100, 4, MidpointRounding.AwayFromZero);

                //if (fNorminalMinSpanResult != 0)
                //    txt_MinSpan.Text = fNorminalMinSpanResult.ToString("F" + m_intDecimal);
                //if (fNorminalMaxSpanResult != 0)
                //    txt_MaxSpan.Text = fNorminalMaxSpanResult.ToString("F" + m_intDecimal);

                //if (dgd_WholeLeadSetting.Rows[3].Cells[3].Value.ToString() != "-")
                //    dgd_WholeLeadSetting.Rows[3].Cells[1].Value = fNorminalMinSpanResult.ToString("F" + m_intDecimal);
                //if (dgd_WholeLeadSetting.Rows[4].Cells[3].Value.ToString() != "-")
                //    dgd_WholeLeadSetting.Rows[4].Cells[2].Value = fNorminalMaxSpanResult.ToString("F" + m_intDecimal);
                
                float fNorminalMinSpanResult = 0;
                float fNorminalMaxSpanResult = 0;

                if (m_smVisionInfo.g_arrLead[1].ref_intNumberOfLead > 0)
                {
                    fNorminalMinSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM * objForm.ref_fLowerLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                    fNorminalMaxSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM * objForm.ref_fUpperLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMinSpanLimit = fNorminalMinSpanResult;
                    m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMaxSpanLimit = fNorminalMaxSpanResult;
                }

                if (m_smVisionInfo.g_arrLead[2].ref_intNumberOfLead > 0)
                {
                    fNorminalMinSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM * objForm.ref_fLowerLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                    fNorminalMaxSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM * objForm.ref_fUpperLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMinSpanLimit = fNorminalMinSpanResult;
                    m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMaxSpanLimit = fNorminalMaxSpanResult;
                }

                if (m_smVisionInfo.g_arrLead[3].ref_intNumberOfLead > 0)
                {
                    fNorminalMinSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM * objForm.ref_fLowerLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                    fNorminalMaxSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM * objForm.ref_fUpperLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMinSpanLimit = fNorminalMinSpanResult;
                    m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMaxSpanLimit = fNorminalMaxSpanResult;
                }

                if (m_smVisionInfo.g_arrLead[4].ref_intNumberOfLead > 0)
                {
                    fNorminalMinSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM * objForm.ref_fLowerLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                    fNorminalMaxSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM * objForm.ref_fUpperLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                    m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMinSpanLimit = fNorminalMinSpanResult;
                    m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMaxSpanLimit = fNorminalMaxSpanResult;
                }

                if (m_smVisionInfo.g_arrLead[1].ref_intNumberOfLead > 0 || m_smVisionInfo.g_arrLead[2].ref_intNumberOfLead > 0 ||
                 m_smVisionInfo.g_arrLead[3].ref_intNumberOfLead > 0 || m_smVisionInfo.g_arrLead[4].ref_intNumberOfLead > 0)
                {
                    if (m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM != -999)
                    {
                        fNorminalMinSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM * objForm.ref_fLowerLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                        fNorminalMaxSpanResult = (float)Math.Round(m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM * objForm.ref_fUpperLimit_Span / 100, 4, MidpointRounding.AwayFromZero);
                        m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMinSpanLimit = fNorminalMinSpanResult;
                        m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMaxSpanLimit = fNorminalMaxSpanResult;
                    }
                }
                m_blnInitDone = false;
                UpdateWholdLeadsSettingGUI();
                m_blnInitDone = true;
            }

            this.TopMost = true;
        }

        public static Point GetFormStartPoint(Point p, Size szButton, Size szForm, ref int intMainFormLocationOffSetX, ref int intMainFormLocationOffSetY)
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            Point pForm = new Point(p.X + szButton.Width, p.Y + szButton.Height);
            int intOffSetX = 0;
            int intOffSetY = 0;
            if (pForm.X + szForm.Width >= resolution.Width)
                intOffSetX = pForm.X + szForm.Width - resolution.Width;
            if (pForm.Y + szForm.Height + 50 >= resolution.Height)
                intOffSetY = pForm.Y + szForm.Height + 50 - resolution.Height;

            intMainFormLocationOffSetX = intOffSetX;
            intMainFormLocationOffSetY = intOffSetY;

            return new Point(pForm.X - intOffSetX, pForm.Y - intOffSetY);
        }
        private void PreUpdateWholeLeadSettingTable()
        {
            dgd_WholeLeadSetting.Rows.Clear();

            int intLeadFailMask = m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask;

            int i = 0;

            if ((intLeadFailMask & 0x01) > 0)
            {
                if (m_smVisionInfo.g_arrLead[0].ref_blnWantCheckExtraLeadLength)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Foreign Material / Contamination (Length)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";


                    i++;
                }
            }

            if ((intLeadFailMask & 0x01) > 0)
            {
                if (m_smVisionInfo.g_arrLead[0].ref_blnWantCheckExtraLeadArea)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Foreign Material / Contamination (Area)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";


                    i++;
                }
            }

            if ((intLeadFailMask & 0x2000) > 0)
            {
                dgd_WholeLeadSetting.Rows.Add();
                dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Foreign Material / Contamination (Total Area)";
                dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
                dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";
                i++;
            }

            if ((m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask & 0x800) > 0)
            {
                // Length Variance Top
                if (m_smVisionInfo.g_arrLead[1].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Top)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";
                    i++;
                }
                // Length Variance Bottom
                if (m_smVisionInfo.g_arrLead[3].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Bottom)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";
                    i++;
                }

                // Length Variance Left
                if (m_smVisionInfo.g_arrLead[4].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Left)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";
                    i++;
                }
                // Length Variance Right
                if (m_smVisionInfo.g_arrLead[2].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Right)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";
                    i++;
                }

                // Length Variance Unit
                if (m_smVisionInfo.g_arrLead[1].ref_intNumberOfLead > 0 || m_smVisionInfo.g_arrLead[2].ref_intNumberOfLead > 0 ||
                    m_smVisionInfo.g_arrLead[3].ref_intNumberOfLead > 0 || m_smVisionInfo.g_arrLead[4].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Unit)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";
                    i++;
                }
            }

            if ((m_smVisionInfo.g_arrLead[0].ref_intFailOptionMask & 0x1000) > 0)
            {
                if (m_smVisionInfo.g_arrLead[1].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Top)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if (m_smVisionInfo.g_arrLead[3].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Bottom)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if (m_smVisionInfo.g_arrLead[2].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Left)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if (m_smVisionInfo.g_arrLead[4].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Right)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if (m_smVisionInfo.g_arrLead[1].ref_intNumberOfLead > 0 || m_smVisionInfo.g_arrLead[2].ref_intNumberOfLead > 0 ||
                    m_smVisionInfo.g_arrLead[3].ref_intNumberOfLead > 0 || m_smVisionInfo.g_arrLead[4].ref_intNumberOfLead > 0)
                {
                    //i++;
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Unit)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }
            }

            //i++;
            //dgd_WholeLeadSetting.Rows.Add();
            //dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Min Span";
            //dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
            //dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "-";
            //dgd_WholeLeadSetting.Rows[i].Cells[2].ReadOnly = true;
            //dgd_WholeLeadSetting.Rows[i].Cells[2].Style.BackColor = Color.Gray;
            //dgd_WholeLeadSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.Gray;
            //dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
            //dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
            //i++;
            //dgd_WholeLeadSetting.Rows.Add();
            //dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Max Span";
            //dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "-";
            //dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
            //dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
            //dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
            //dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
            //dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "-";
            //dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
            //i++;
        }
        private void UpdateWholdLeadsSettingGUI()
        {
            for (int i = 0; i < dgd_WholeLeadSetting.Rows.Count; i++)
            {
                switch (dgd_WholeLeadSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Foreign Material / Contamination (Length)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[0].ref_fExtraLeadSetLength.ToString("F" + m_intDecimal);
                        break;
                    case "Foreign Material / Contamination (Area)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[0].ref_fExtraLeadSetArea.ToString("F" + m_intDecimal2);
                        break;
                    case "Foreign Material / Contamination (Total Area)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[0].ref_fTotalExtraLeadSetArea.ToString("F" + m_intDecimal2);
                        break;
                    case "Min Span":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Max Span":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Span (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Span (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Span (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Span (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Span (Unit)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Length Variance (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[1].ref_fTemplateLengthVarianceMaxLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Length Variance (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[2].ref_fTemplateLengthVarianceMaxLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Length Variance (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[3].ref_fTemplateLengthVarianceMaxLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Length Variance (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[4].ref_fTemplateLengthVarianceMaxLimit.ToString("F" + m_intDecimal);
                        break;
                    case "Length Variance (Unit)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead[0].ref_fTemplateLengthVarianceMaxLimit.ToString("F" + m_intDecimal);
                        break;
                }
            }
        }
        private void UpdateWholeLeadScore()
        {
            for (int i = 0; i < dgd_WholeLeadSetting.Rows.Count; i++)
            {
                switch (dgd_WholeLeadSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Foreign Material / Contamination (Length)":
                        //dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftResult.ToString();
                        //if (m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftResult > m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftMaxSetting)
                        //{
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        //}
                        //else
                        //{
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        //}
                        break;
                    case "Foreign Material / Contamination (Area)":
                        //dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftResult.ToString();
                        //if (m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftResult > m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftMaxSetting)
                        //{
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        //}
                        //else
                        //{
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        //}
                        break;
                    case "Foreign Material / Contamination (Total Area)":
                        //dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftResult.ToString();
                        //if (m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftResult > m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftMaxSetting)
                        //{
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                        //    dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        //}
                        //else
                        //{
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        //}
                        break;
                    case "Min Span":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[0].ref_fLeadMinSpanResult.ToString("F" + m_intDecimal);
                        if ((m_smVisionInfo.g_arrLead[0].ref_fLeadMinSpanResult < m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMinSpanLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Max Span":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[0].ref_fLeadMaxSpanResult.ToString("F" + m_intDecimal);
                        if ((m_smVisionInfo.g_arrLead[0].ref_fLeadMaxSpanResult > m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMaxSpanLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Span (Top)":
                        if (m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM != -999 &&
                            (m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMinSpanLimit) ||
                            (m_smVisionInfo.g_arrLead[1].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMaxSpanLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Span (Right)":
                        if (m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM != -999 &&
                            (m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMinSpanLimit) ||
                            (m_smVisionInfo.g_arrLead[2].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMaxSpanLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Span (Bottom)":
                        if (m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM != -999 &&
                            (m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMinSpanLimit) ||
                            (m_smVisionInfo.g_arrLead[3].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMaxSpanLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Span (Left)":
                        if (m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM != -999 &&
                            (m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMinSpanLimit) ||
                            (m_smVisionInfo.g_arrLead[4].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMaxSpanLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Span (Unit)":
                        if (m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM != -999 &&
                            (m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM < m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMinSpanLimit) ||
                            (m_smVisionInfo.g_arrLead[0].ref_fLeadSpanResultMM > m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMaxSpanLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Length Variance (Top)":
                        if (m_smVisionInfo.g_arrLead[1].ref_fSampleLengthVarianceMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[1].ref_fSampleLengthVarianceMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[1].ref_fSampleLengthVarianceMM != -999 && (m_smVisionInfo.g_arrLead[1].ref_fSampleLengthVarianceMM > m_smVisionInfo.g_arrLead[1].ref_fTemplateLengthVarianceMaxLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Length Variance (Right)":
                        if (m_smVisionInfo.g_arrLead[2].ref_fSampleLengthVarianceMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[2].ref_fSampleLengthVarianceMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[2].ref_fSampleLengthVarianceMM != -999 && (m_smVisionInfo.g_arrLead[2].ref_fSampleLengthVarianceMM > m_smVisionInfo.g_arrLead[2].ref_fTemplateLengthVarianceMaxLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Length Variance (Bottom)":
                        if (m_smVisionInfo.g_arrLead[3].ref_fSampleLengthVarianceMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[3].ref_fSampleLengthVarianceMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[3].ref_fSampleLengthVarianceMM != -999 && (m_smVisionInfo.g_arrLead[3].ref_fSampleLengthVarianceMM > m_smVisionInfo.g_arrLead[3].ref_fTemplateLengthVarianceMaxLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Length Variance (Left)":
                        if (m_smVisionInfo.g_arrLead[4].ref_fSampleLengthVarianceMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[4].ref_fSampleLengthVarianceMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[4].ref_fSampleLengthVarianceMM != -999 && (m_smVisionInfo.g_arrLead[4].ref_fSampleLengthVarianceMM > m_smVisionInfo.g_arrLead[4].ref_fTemplateLengthVarianceMaxLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Length Variance (Unit)":
                        if (m_smVisionInfo.g_arrLead[0].ref_fSampleLengthVarianceMM == -999)
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "---";
                        else
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead[0].ref_fSampleLengthVarianceMM.ToString("F" + m_intDecimal);

                        if (m_smVisionInfo.g_arrLead[0].ref_fSampleLengthVarianceMM != -999 && (m_smVisionInfo.g_arrLead[0].ref_fSampleLengthVarianceMM > m_smVisionInfo.g_arrLead[0].ref_fTemplateLengthVarianceMaxLimit))
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                }
            }
        }
        private void dgd_WholeLeadSetting_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            /*
           * Please take note dgd_MarkSetting_CellValueChanged event will be triggered during loading and when value change in cell.
           * dgd_MarkSetting_CellEndEdit will not be triggred although value change. It will only been triggered after user finish change cell value manually (even same value), 
           * 2018 12 14 -CCENG: change to use CellValueChanged because the CellEndEdit event is not triggered when user press other form.
           */

            if (!m_blnInitDone)
                return;

            if (e.ColumnIndex != 1 && e.ColumnIndex != 2)
                return;

            float fValue = 0;
            if (!float.TryParse(dgd_WholeLeadSetting.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                return;

            int i = e.RowIndex;
            switch (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString())
            {
                case "Foreign Material / Contamination (Length)":
                    for (int j = 0; j < m_smVisionInfo.g_arrLead.Length; j++)
                        m_smVisionInfo.g_arrLead[j].ref_fExtraLeadSetLength = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Foreign Material / Contamination (Area)":
                    for (int j = 0; j < m_smVisionInfo.g_arrLead.Length; j++)
                        m_smVisionInfo.g_arrLead[j].ref_fExtraLeadSetArea = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Foreign Material / Contamination (Total Area)":
                    for (int j = 0; j < m_smVisionInfo.g_arrLead.Length; j++)
                        m_smVisionInfo.g_arrLead[j].ref_fTotalExtraLeadSetArea = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Min Span":
                    for (int j = 0; j < m_smVisionInfo.g_arrLead.Length; j++)
                        m_smVisionInfo.g_arrLead[j].ref_fTemplateLeadMinSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    break;
                case "Max Span":
                    for (int j = 0; j < m_smVisionInfo.g_arrLead.Length; j++)
                        m_smVisionInfo.g_arrLead[j].ref_fTemplateLeadMaxSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Top)":
                    m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMinSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead[1].ref_fTemplateLeadMaxSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Right)":
                    m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMinSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead[2].ref_fTemplateLeadMaxSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Bottom)":
                    m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMinSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead[3].ref_fTemplateLeadMaxSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Left)":
                    m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMinSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead[4].ref_fTemplateLeadMaxSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Unit)":
                    m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMinSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead[0].ref_fTemplateLeadMaxSpanLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Top)":
                    m_smVisionInfo.g_arrLead[1].ref_fTemplateLengthVarianceMaxLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Right)":
                    m_smVisionInfo.g_arrLead[2].ref_fTemplateLengthVarianceMaxLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Bottom)":
                    m_smVisionInfo.g_arrLead[3].ref_fTemplateLengthVarianceMaxLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Left)":
                    m_smVisionInfo.g_arrLead[4].ref_fTemplateLengthVarianceMaxLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Unit)":
                    m_smVisionInfo.g_arrLead[0].ref_fTemplateLengthVarianceMaxLimit = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
            }
        }

        private void dgd_TopLead_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            //if (e.RowIndex < 0)
            //    return;

            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex == 1 || e.ColumnIndex == 4 || e.ColumnIndex == 7 || e.ColumnIndex == 11 ||
                e.ColumnIndex == 15 || e.ColumnIndex == 19 || e.ColumnIndex == 23 || e.ColumnIndex == 26 || e.ColumnIndex == 30 || e.ColumnIndex == 33)
                return;

            // Skip if col is separation
            if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 9 || e.ColumnIndex == 13 ||
                e.ColumnIndex == 17 || e.ColumnIndex == 21 || e.ColumnIndex == 24 || e.ColumnIndex == 28 || e.ColumnIndex == 31 || e.ColumnIndex == 35)// ||
                //e.ColumnIndex == 37 || e.ColumnIndex == 39 || e.ColumnIndex == 41 || e.ColumnIndex == 43)
                return;

            // Skip if col is golden unit data
            if (e.ColumnIndex == 36 || e.ColumnIndex == 37 || e.ColumnIndex == 38 || e.ColumnIndex == 39)
                return;

            ////Skip if cell value is ---
            //if ((((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "---") ||
            //    (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == ""))
            //    return;

            ////Min, max area, broken area has 6 decimal places 
            //if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
            //{
            //    switch (m_smCustomizeInfo.g_intUnitDisplay)
            //    {
            //        default:
            //        case 1:
            //            m_strUnitLabel = "mm^2";
            //            m_intDecimalPlaces = 6;
            //            break;
            //        case 2:
            //            m_strUnitLabel = "mil^2";
            //            m_intDecimalPlaces = 6;
            //            break;
            //        case 3:
            //            m_strUnitLabel = "um^2";
            //            m_intDecimalPlaces = 2;
            //            break;
            //    }
            //}
            //else
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
            string strUnitLabel = m_strUnitLabel;
            if (e.ColumnIndex == 25 || e.ColumnIndex == 27) // no unit label for average gray value
            {
                strUnitLabel = "";
            }

            //string strCurrentSetValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();


            string strCurrentSetValue = "";
            for (int i = 0; i < ((DataGridView)sender).Rows.Count; i++)
            {
                float fSetValue = 0;
                if (float.TryParse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString(), out fSetValue))
                {
                    strCurrentSetValue = fSetValue.ToString();
                    break;
                }
            }
            if (strCurrentSetValue == "")
                return;


            //SetValueForm objSetValueForm = new SetValueForm(e.RowIndex, intDecimalPlaces, strCurrentSetValue);
            SetValueForm objSetValueForm = new SetValueForm("Set value to Lead " + (e.RowIndex + 1).ToString(), strUnitLabel, e.RowIndex, m_intDecimalPlaces, strCurrentSetValue, true, false, false);
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
                        if (e.ColumnIndex == 6 || e.ColumnIndex == 10 || e.ColumnIndex == 14 || e.ColumnIndex == 18 || e.ColumnIndex == 25 || e.ColumnIndex == 32)
                        {
                            float fMax = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Value.ToString());

                            if (objSetValueForm.ref_fSetValue > fMax)
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.SelectionBackColor = Color.Pink;
                            }
                            else
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.SelectionBackColor = Color.White;
                            }
                        }
                        else if (e.ColumnIndex == 8 || e.ColumnIndex == 12 || e.ColumnIndex == 16 || e.ColumnIndex == 20 || e.ColumnIndex == 27 || e.ColumnIndex == 34)
                        {
                            float fMin = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Value.ToString());
                            if (fMin > objSetValueForm.ref_fSetValue)
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.BackColor = Color.Pink;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.SelectionBackColor = Color.Pink;
                            }
                            else
                            {
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.BackColor = Color.White;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.SelectionBackColor = Color.White;
                            }
                        }

                        //// Set new insert value into table
                        //if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
                        //    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F6");
                        //else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                    }

                    //Set value column selected
                    float fMinPitch, fMaxPitch;
                    if (((DataGridView)sender).Rows[i].Cells[14].Value.ToString() == "---")
                        fMinPitch = -1;
                    else
                        fMinPitch = float.Parse(((DataGridView)sender).Rows[i].Cells[14].Value.ToString());

                    if (((DataGridView)sender).Rows[i].Cells[16].Value.ToString() == "---")
                        fMaxPitch = -1;
                    else
                        fMaxPitch = float.Parse(((DataGridView)sender).Rows[i].Cells[16].Value.ToString());

                    //Set value column selected
                    float fMinGap, fMaxGap;
                    if (((DataGridView)sender).Rows[i].Cells[18].Value.ToString() == "---")
                        fMinGap = -1;
                    else
                        fMinGap = float.Parse(((DataGridView)sender).Rows[i].Cells[18].Value.ToString());

                    if (((DataGridView)sender).Rows[i].Cells[20].Value.ToString() == "---")
                        fMaxGap = -1;
                    else
                        fMaxGap = float.Parse(((DataGridView)sender).Rows[i].Cells[20].Value.ToString());

                    //int intLengthMode = m_smVisionInfo.g_arrLead[m_intLeadIndex].GetSampleLengthMode(i);
                    for (int x = 0; x < m_smVisionInfo.g_arrLead.Length; x++)
                    {
                        if (!m_smVisionInfo.g_arrLead[x].ref_blnSelected)
                            continue;

                        int intBlobsCount = m_smVisionInfo.g_arrLead[x].GetBlobsFeaturesNumber();
                        int intBlobIndex = 0;
                        //int intStartIndex = m_smVisionInfo.g_arrLead[x].GetBlobsNoID();
                        bool blnFound = false;
                        for (int k = 0; k < intBlobsCount; k++) // for (int k = intStartIndex - 1; k < intBlobsCount + (intStartIndex - 1); k++)
                        {
                            int intBlobID = m_smVisionInfo.g_arrLead[x].GetBlobsNoID(k);
                            if (intBlobID == (i + 1))
                            {
                                intBlobIndex = k;
                                blnFound = true;
                                break;
                            }
                        }
                        //Update template setting
                        //if (intLengthMode == 1)
                        //{
                        if (blnFound)
                        {
                            m_smVisionInfo.g_arrLead[x].UpdateBlobFeatureToPixel(intBlobIndex,
                                float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[6].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[8].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[10].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[12].Value.ToString()),
                                fMinPitch,
                                fMaxPitch,
                                fMinGap,
                                fMaxGap,
                                float.Parse(((DataGridView)sender).Rows[i].Cells[22].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[25].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[27].Value.ToString())//,
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[36].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[38].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[40].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[42].Value.ToString())
                                );

                            if (m_smVisionInfo.g_arrLead[x].ref_blnWantInspectBaseLead)
                            {
                                m_smVisionInfo.g_arrLead[x].UpdateBlobFeatureToPixel_BaseLead(intBlobIndex,
                                float.Parse(((DataGridView)sender).Rows[i].Cells[29].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[32].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[34].Value.ToString())
                                );
                            }
                            //}
                            //else
                            //{
                            //    m_smVisionInfo.g_arrLead[m_intLeadIndex].UpdateBlobFeatureToPixel(i,
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                            //        fMinPitch,
                            //        fMaxPitch,
                            //        fMinGap,
                            //        fMaxGap,
                            //        float.Parse(((DataGridView)sender).Rows[i].Cells[19].Value.ToString())
                            //        );
                            //}

                            //Update template pitch gap setting
                            m_smVisionInfo.g_arrLead[x].SetPitchGapDataFrom(intBlobIndex,
                               fMinPitch,
                               fMaxPitch,
                               fMinGap,
                               fMaxGap);
                        }
                    }

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }

            this.TopMost = true;
        }

        private void btn_OffsetSetting_Click(object sender, EventArgs e)
        {
            LeadOffsetSettingForm objForm = new LeadOffsetSettingForm(m_smCustomizeInfo, m_smVisionInfo,
                m_strSelectedRecipe, m_intUserGroup, m_smProductionInfo);
            Rectangle objScreenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            objForm.StartPosition = FormStartPosition.Manual;
            objForm.Location = new Point(objScreenRect.Width - objForm.Width - 10,
                objScreenRect.Height - objForm.Height - 10);
            objForm.TopMost = true;
            objForm.ShowDialog();

            TriggerOfflineTest();
        }
    }
}