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
    public partial class Lead3DToleranceSettingForm : Form
    {
        #region Member Variables
        private bool m_blnChangeScoreSetting = true;
        private bool m_blnFormOpen = false;
        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private int m_intLeadIndex = 0;
        //private int m_intGDSelectedIndex = 0;   // Golden Data Set Selected index
        private int m_intDecimal = 3;
        private int m_intDecimal2 = 6;
        private int m_intDecimalPlaces = 4;
        private string m_strSelectedRecipe;
        private string m_strUnitLabel = "mm";
        
        private DataGridView m_dgdWholeLeadSettingView = new DataGridView();
        private DataGridView m_dgdLeadSettingView = new DataGridView();
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private GoldenUnitCompensationForm objGoldenUnitForm;
        #endregion


        #region Properties

        public bool ref_blnFormOpen { get { return m_blnFormOpen; } set { m_blnFormOpen = value; } }

        #endregion

        public Lead3DToleranceSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, int intLeadSelectedMask, ProductionInfo smProductionInfo)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;
            m_smProductionInfo = smProductionInfo;
            m_dgdLeadSettingView = dgd_TopLead;
            m_dgdWholeLeadSettingView = dgd_WholeLeadSetting;
            
            //LoadGoldenData();
            DisableField2();
            UpdateGUI();

            m_blnInitDone = true;

            // 2020-01-06 ZJYEOH : Trigger Offline Test one time 
            m_smVisionInfo.g_intViewInspectionSetting = 1;
            m_smVisionInfo.AT_VM_OfflineTestAllLead3D = true;
            TriggerOfflineTest();
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            //NewUserRight objUserRight = new NewUserRight(false);
            string strChild1 = "Tol.Lead3D";
            string strChild2 = "";

            strChild2 = "Lead3D TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
            {
                m_dgdWholeLeadSettingView.Enabled = false;
                m_blnChangeScoreSetting = false;
                btn_GoldenUnitSetting.Enabled = false;
                btn_LoadTolFromFile.Enabled = false;
                btn_SaveAccuracyReport.Enabled = false;
                btn_SaveTolToFile.Enabled = false;
                btn_UpdateTolerance.Enabled = false;
            }

            strChild2 = "Lead3D Offset Setting";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
            {
                btn_OffsetSetting.Visible = false;
                //m_dgdLeadSettingView.Columns[45].Visible = false;
                //m_dgdLeadSettingView.Columns[46].Visible = false;
                //m_dgdLeadSettingView.Columns[47].Visible = false;
                //m_dgdLeadSettingView.Columns[48].Visible = false;
                //m_dgdLeadSettingView.Columns[49].Visible = false;
                //m_dgdLeadSettingView.Columns[50].Visible = false;
                //m_dgdLeadSettingView.Columns[51].Visible = false;
                //m_dgdLeadSettingView.Columns[52].Visible = false;
                //m_dgdLeadSettingView.Columns[53].Visible = false;
                //m_dgdLeadSettingView.Columns[54].Visible = false;
                //m_dgdLeadSettingView.Columns[55].Visible = false;
                //m_dgdLeadSettingView.Columns[56].Visible = false;
            }

            strChild2 = "Save Button";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
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
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }

        private void UpdateGUI()
        {
            // -------------- Update individual lead table -------------------------------------------------------------------------------

            ReadLeadTemplateDataToGrid(m_intLeadIndex, m_dgdLeadSettingView);

            m_dgdLeadSettingView.Columns.Remove(column_GolWidth);
            m_dgdLeadSettingView.Columns.Remove(column_GolLength);
            m_dgdLeadSettingView.Columns.Remove(column_GolPitch);
            m_dgdLeadSettingView.Columns.Remove(column_GolGap);
            if (m_intUserGroup != 1)    // for SRM only
            {
                btn_GoldenUnitSetting.Visible = false;
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

            UpdateScore(m_intLeadIndex, m_dgdLeadSettingView);

            // ------------ Update whole lead table ----------------------------------------------------------------------------
            PreUpdateWholeLeadSettingTable();
            UpdateWholdLeadsSettingGUI();
            UpdateWholeLeadScore();

            m_smVisionInfo.PR_TL_UpdateInfo = false;

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_DisplayResult.Checked = Convert.ToBoolean(subkey1.GetValue("CurrentDisplayResult_LeadTolerance", false));
            ViewOrHideResultColumn(chk_DisplayResult.Checked);
        }

        private void PreUpdateWholeLeadSettingTable()
        {
            dgd_WholeLeadSetting.Rows.Clear();

            int intFailMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;

            int i = 0;
            if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0)
            {
                if ((intFailMask & 0x2000) > 0)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Pitch Variance (Left)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";


                    i++;


                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Pitch Variance (Right)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if ((intFailMask & 0x800) > 0)
                {
                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadLengthVarianceMethod != 1)
                    {
                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Left)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;

                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Right)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;
                    }

                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadLengthVarianceMethod != 0)
                    {
                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Unit)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;
                    }
                }

                if ((intFailMask & 0x4000) > 0)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Stand Off Variance (Left)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;

                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Stand Off Variance (Right)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;

                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Stand Off Variance (Unit)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if ((intFailMask & 0x04) > 0)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Leads Sweep (Left)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;

                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Leads Sweep (Right)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }


                if ((intFailMask & 0x1000) > 0)
                {
                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadSpanMethod != 1)
                    {
                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Left)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;

                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Right)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;
                    }

                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadSpanMethod != 0)
                    {
                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Unit)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;
                    }
                }
            }
            else
            {
                if ((intFailMask & 0x2000) > 0)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Pitch Variance (Top)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;

                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Pitch Variance (Bottom)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if ((intFailMask & 0x800) > 0)
                {
                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadLengthVarianceMethod != 1)
                    {
                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Top)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;

                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Bottom)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;
                    }

                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadLengthVarianceMethod != 0)
                    {
                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Length Variance (Unit)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;
                    }
                }

                if ((intFailMask & 0x4000) > 0)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Stand Off Variance (Top)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;

                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Stand Off Variance (Bottom)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;

                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Stand Off Variance (Unit)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if ((intFailMask & 0x04) > 0)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Leads Sweep (Top)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;

                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Leads Sweep (Bottom)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if ((intFailMask & 0x1000) > 0)
                {
                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadSpanMethod != 1)
                    {
                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Top)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;

                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Bottom)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;
                    }

                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadSpanMethod != 0)
                    {
                        dgd_WholeLeadSetting.Rows.Add();
                        dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "Span (Unit)";
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                        dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                        i++;
                    }
                }
            }

            if ((intFailMask & 0x8000) > 0)
            {
                if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadLength)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "F.Material / Cont. (Length)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantCheckExtraLeadArea)
                {
                    dgd_WholeLeadSetting.Rows.Add();
                    dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "F.Material / Cont. (Area)";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";
                    i++;
                }
            }

            if ((intFailMask & 0x10000) > 0)
            {
                dgd_WholeLeadSetting.Rows.Add();
                dgd_WholeLeadSetting.Rows[i].Cells[0].Value = "F.Material / Cont. (Total Area)";
                dgd_WholeLeadSetting.Rows[i].Cells[1].Value = "0";
                dgd_WholeLeadSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_WholeLeadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_WholeLeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_WholeLeadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholeLeadSetting.Rows[i].Cells[3].Value = "0";
                dgd_WholeLeadSetting.Rows[i].Cells[4].Value = "mm2";
                i++;
            }
        }

        private void UpdateWholdLeadsSettingGUI()
        {
            for (int i = 0; i < dgd_WholeLeadSetting.Rows.Count; i++)
            {
                switch (dgd_WholeLeadSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Pitch Variance (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftMaxSetting.ToString();
                        break;
                    case "Pitch Variance (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceRightMaxSetting.ToString();
                        break;
                    case "Pitch Variance (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceTopMaxSetting.ToString();
                        break;
                    case "Pitch Variance (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceBottomMaxSetting.ToString();
                        break;
                    case "Length Variance (Unit)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceOverallMaxSetting.ToString();
                        break;
                    case "Length Variance (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceLeftMaxSetting.ToString();
                        break;
                    case "Length Variance (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceRightMaxSetting.ToString();
                        break;
                    case "Length Variance (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceTopMaxSetting.ToString();
                        break;
                    case "Length Variance (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceBottomMaxSetting.ToString();
                        break;
                    case "Stand Off Variance (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceLeftMaxSetting.ToString();
                        break;
                    case "Stand Off Variance (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceRightMaxSetting.ToString();
                        break;
                    case "Stand Off Variance (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceTopMaxSetting.ToString();
                        break;
                    case "Stand Off Variance (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceBottomMaxSetting.ToString();
                        break;
                    case "Stand Off Variance (Unit)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceUnitMaxSetting.ToString();
                        break;
                    case "Leads Sweep (Left)":
                        //dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftMaxSetting.ToString();
                        break;
                    case "Leads Sweep (Right)":
                        //dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightMaxSetting.ToString();
                        break;
                    case "Leads Sweep (Top)":
                        //dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopMaxSetting.ToString();
                        break;
                    case "Leads Sweep (Bottom)":
                        //dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomMaxSetting.ToString();
                        break;
                    case "Span (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftMaxSetting.ToString();
                        break;
                    case "Span (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightMaxSetting.ToString();
                        break;
                    case "Span (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopMaxSetting.ToString();
                        break;
                    case "Span (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomMaxSetting.ToString();
                        break;
                    case "Span (Unit)":
                        dgd_WholeLeadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallMinSetting.ToString();
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallMaxSetting.ToString();
                        break;
                    case "F.Material / Cont. (Length)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fExtraLeadSetLength.ToString("F" + m_intDecimal);
                        break;
                    case "F.Material / Cont. (Area)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fExtraLeadSetArea.ToString("F" + m_intDecimal2);
                        break;
                    case "F.Material / Cont. (Total Area)":
                        dgd_WholeLeadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].ref_fTotalExtraLeadSetArea.ToString("F" + m_intDecimal2);
                        break;
                   
                }
            }
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

            //txt_MinSpan.DecimalPlaces = m_intDecimal;
            //txt_MaxSpan.DecimalPlaces = m_intDecimal;
        }

        //private void LoadGoldenData()
        //{
        //    // Load Golden Data
        //    string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
        //            m_smVisionInfo.g_strVisionFolderName + "\\Lead\\GoldenData.xml";

        //    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
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

        //        m_smVisionInfo.g_arrLead3D[i].LoadLeadGoldenData(strPath, strSectionName);
        //    }
        //}

        //private void UpdateGoldenDataIntoGridTable(int intLeadIndex, DataGridView dgd_LeadSetting)
        //{
        //    if (objGoldenUnitForm == null)
        //        return;

        //    if (!objGoldenUnitForm.ref_intViewGoldenDataColumn)
        //        return;

        //    if (m_intGDSelectedIndex >= m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData.Count)
        //        return;

        //    for (int r = 0; r < dgd_LeadSetting.Rows.Count; r++)
        //    {
        //        if (r < m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Count)
        //        {
        //            if (dgd_LeadSetting.Rows[r].Cells.Count <= 25)
        //                continue;

        //            if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 0)
        //                dgd_LeadSetting.Rows[r].Cells[25].Value = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][0];
        //            else
        //                dgd_LeadSetting.Rows[r].Cells[25].Value = 0;

        //            if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 1)
        //                dgd_LeadSetting.Rows[r].Cells[26].Value = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][1];
        //            else
        //                dgd_LeadSetting.Rows[r].Cells[26].Value = 0;

        //            if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 2)
        //                dgd_LeadSetting.Rows[r].Cells[27].Value = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][2];
        //            else
        //                dgd_LeadSetting.Rows[r].Cells[27].Value = 0;

        //            if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 3)
        //                dgd_LeadSetting.Rows[r].Cells[28].Value = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][3];
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
        //    m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Clear();

        //    for (int r = 0; r < dgd_LeadSetting.Rows.Count; r++)
        //    {
        //        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Add(new List<float>());
        //        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[25].Value));
        //        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[26].Value));
        //        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[27].Value));
        //        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[28].Value));

        //    }
        //}

        //private void CheckSaveGoldenData(int intLeadIndex, DataGridView dgd_LeadSetting)
        //{
        //    if (objGoldenUnitForm == null)
        //        return;

        //    if (!objGoldenUnitForm.ref_intViewGoldenDataColumn)
        //        return;

        //    bool blnIsDataChanged = false;
        //    if (m_intGDSelectedIndex >= 0 && m_intGDSelectedIndex < m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData.Count)
        //    {
        //        if (dgd_LeadSetting.Rows.Count != m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Count)
        //        {
        //            blnIsDataChanged = true;
        //        }
        //        else
        //        {

        //            for (int r = 0; r < dgd_LeadSetting.Rows.Count; r++)
        //            {
        //                if (Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[25].Value) != m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][0])
        //                {
        //                    blnIsDataChanged = true;
        //                    break;
        //                }

        //                if (Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[26].Value) != m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][1])
        //                {
        //                    blnIsDataChanged = true;
        //                    break;
        //                }

        //                if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 2)
        //                {
        //                    if (Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[27].Value) != m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][2])
        //                    {
        //                        blnIsDataChanged = true;
        //                        break;
        //                    }
        //                }

        //                if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 3)
        //                {
        //                    if (Convert.ToSingle(dgd_LeadSetting.Rows[r].Cells[28].Value) != m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][3])
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

        //    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
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

        //        m_smVisionInfo.g_arrLead3D[i].SaveLeadGoldenData(strPath, false, strSectionName, true);
        //    }
        //}

        private void UpdateWholeLeadScore()
        {
            for (int i = 0; i < dgd_WholeLeadSetting.Rows.Count; i++)
            {
                switch (dgd_WholeLeadSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Pitch Variance (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftResult > m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftMaxSetting)
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
                    case "Pitch Variance (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceRightResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceRightResult > m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceRightMaxSetting)
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
                    case "Pitch Variance (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceTopResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceTopResult > m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceTopMaxSetting)
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
                    case "Pitch Variance (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceBottomResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceBottomResult > m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceBottomMaxSetting)
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceOverallResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceOverallResult > m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceOverallMaxSetting)
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceLeftResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceLeftResult > m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceLeftMaxSetting)
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceRightResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceRightResult > m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceRightMaxSetting)
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceTopResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceTopResult > m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceTopMaxSetting)
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceBottomResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceBottomResult > m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceBottomMaxSetting)
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
                    case "Stand Off Variance (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceLeftResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceLeftResult > m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceLeftMaxSetting)
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
                    case "Stand Off Variance (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceRightResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceRightResult > m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceRightMaxSetting)
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
                    case "Stand Off Variance (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceTopResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceTopResult > m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceTopMaxSetting)
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
                    case "Stand Off Variance (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceBottomResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceBottomResult > m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceBottomMaxSetting)
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
                    case "Stand Off Variance (Unit)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceUnitResult.ToString("F4");
                        if (m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceUnitResult > m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceUnitMaxSetting)
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
                    case "Leads Sweep (Left)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftResult.ToString("F4");
                        if (//(m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftResult < m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftMinSetting) || 
                            (m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftResult > m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftMaxSetting))
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
                    case "Leads Sweep (Right)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightResult.ToString("F4");
                        if (//(m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightResult < m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightMinSetting) ||
                            (m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightResult > m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightMaxSetting))
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
                    case "Leads Sweep (Top)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopResult.ToString("F4");
                        if (//(m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopResult < m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopMinSetting) ||
                            (m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopResult > m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopMaxSetting))
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
                    case "Leads Sweep (Bottom)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomResult.ToString("F4");
                        if (//(m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomResult < m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomMinSetting) ||
                            (m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomResult > m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomMaxSetting))
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallResult.ToString("F4");
                        if ((m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallResult < m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallMinSetting) ||
                            (m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallResult > m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallMaxSetting))
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopResult.ToString("F4");
                        if ((m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopResult < m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopMinSetting) ||
                            (m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopResult > m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopMaxSetting))
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomResult.ToString("F4");
                        if ((m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomResult < m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomMinSetting) ||
                            (m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomResult > m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomMaxSetting))
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftResult.ToString("F4");
                        if ((m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftResult < m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftMinSetting) ||
                            (m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftResult > m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftMaxSetting))
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
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightResult.ToString("F4");
                        if ((m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightResult < m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightMinSetting) ||
                            (m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightResult > m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightMaxSetting))
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
                    case "F.Material / Cont. (Length)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        break;
                    case "F.Material / Cont. (Area)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        break;
                    case "F.Material / Cont. (Total Area)":
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholeLeadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        break;

                }
            }
        }

        private void UpdateScore(int intLeadIndex, DataGridView dgd_LeadSetting)
        {

            TrackLog objTL = new TrackLog();
            objTL.WriteLine("                 ");

            int intLeadNoIndex = 0;
            int intGroupNoIndex = -1;
            for (int i = 0; i < dgd_LeadSetting.Rows.Count; i++)
            {
                if (dgd_LeadSetting.Rows[i].HeaderCell.Value.ToString().IndexOf("Group") >= 0)
                {
                    intGroupNoIndex = i;
                    continue;
                }

                intLeadNoIndex = Convert.ToInt32(dgd_LeadSetting.Rows[i].HeaderCell.Value.ToString().Substring(4)) - 1;  // substring start with 4 because "Lead" length is 4

                List<string> arrResultList = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetBlobFeaturesResult_WithPassFailIndicator(intLeadNoIndex);

                int intFailMask = Convert.ToInt32(arrResultList[arrResultList.Count - 1]);

                // Offset
                dgd_LeadSetting.Rows[i].Cells[1].Value = arrResultList[0];
                if ((intFailMask & 0x20000) > 0)
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

                // Skew
                if (!m_smVisionInfo.g_arrLead3D[0].GetWantCheckSkew(i))
                {
                    m_dgdLeadSettingView.Rows[i].Cells[3].Value = "---";
                    m_dgdLeadSettingView.Rows[i].Cells[4].Value = "---";
                }
                else
                {
                    dgd_LeadSetting.Rows[i].Cells[4].Value = arrResultList[1];
                    if ((intFailMask & 0x100) > 0)
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

                // Width
                dgd_LeadSetting.Rows[i].Cells[7].Value = arrResultList[2];
                if ((intFailMask & 0x01) > 0)
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

                // Length
                dgd_LeadSetting.Rows[i].Cells[11].Value = arrResultList[3];
                if ((intFailMask & 0x02) > 0)
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

                // Pitch
                dgd_LeadSetting.Rows[i].Cells[15].Value = arrResultList[4];
                if ((intFailMask & 0x04) > 0)
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

                // Gap
                dgd_LeadSetting.Rows[i].Cells[19].Value = arrResultList[5];
                if ((intFailMask & 0x08) > 0)
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

                // Stand Off
                dgd_LeadSetting.Rows[i].Cells[23].Value = arrResultList[6];
                if ((intFailMask & 0x10) > 0)
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

                // Solder Pad Length
                dgd_LeadSetting.Rows[i].Cells[27].Value = arrResultList[7];
                if ((intFailMask & 0x20) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[27].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[27].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[27].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[27].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_LeadSetting.Rows[i].Cells[27].Style.BackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[27].Style.SelectionBackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[27].Style.ForeColor = Color.Black;
                    dgd_LeadSetting.Rows[i].Cells[27].Style.SelectionForeColor = Color.Black;
                }

                // Coplan
                dgd_LeadSetting.Rows[i].Cells[31].Value = arrResultList[8];
                if ((intFailMask & 0x40) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[31].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[31].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[31].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[31].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_LeadSetting.Rows[i].Cells[31].Style.BackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[31].Style.SelectionBackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[31].Style.ForeColor = Color.Black;
                    dgd_LeadSetting.Rows[i].Cells[31].Style.SelectionForeColor = Color.Black;
                }

                // Average Gray Value
                dgd_LeadSetting.Rows[i].Cells[34].Value = arrResultList[9];
                if ((intFailMask & 0x80) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[34].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[34].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[34].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[34].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_LeadSetting.Rows[i].Cells[34].Style.BackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[34].Style.SelectionBackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[34].Style.ForeColor = Color.Black;
                    dgd_LeadSetting.Rows[i].Cells[34].Style.SelectionForeColor = Color.Black;
                }

                // Lead Min Width
                dgd_LeadSetting.Rows[i].Cells[38].Value = arrResultList[10];
                if ((intFailMask & 0x200) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[38].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[38].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[38].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[38].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_LeadSetting.Rows[i].Cells[38].Style.BackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[38].Style.SelectionBackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[38].Style.ForeColor = Color.Black;
                    dgd_LeadSetting.Rows[i].Cells[38].Style.SelectionForeColor = Color.Black;
                }

                // Lead Max Width
                dgd_LeadSetting.Rows[i].Cells[39].Value = arrResultList[11];
                if ((intFailMask & 0x400) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[39].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[39].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[39].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[39].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_LeadSetting.Rows[i].Cells[39].Style.BackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[39].Style.SelectionBackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[39].Style.ForeColor = Color.Black;
                    dgd_LeadSetting.Rows[i].Cells[39].Style.SelectionForeColor = Color.Black;
                }

                // Lead Max Burr
                dgd_LeadSetting.Rows[i].Cells[43].Value = arrResultList[12];
                if ((intFailMask & 0x800) > 0)
                {
                    dgd_LeadSetting.Rows[i].Cells[43].Style.BackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[43].Style.SelectionBackColor = Color.Red;
                    dgd_LeadSetting.Rows[i].Cells[43].Style.ForeColor = Color.Yellow;
                    dgd_LeadSetting.Rows[i].Cells[43].Style.SelectionForeColor = Color.Yellow;
                }
                else
                {
                    dgd_LeadSetting.Rows[i].Cells[43].Style.BackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[43].Style.SelectionBackColor = Color.Lime;
                    dgd_LeadSetting.Rows[i].Cells[43].Style.ForeColor = Color.Black;
                    dgd_LeadSetting.Rows[i].Cells[43].Style.SelectionForeColor = Color.Black;
                }

            }

            //if (lbl_ResultMinSpan.Text != "---" && lbl_ResultMaxSpan.Text != "---")
            //{
            //    //Span
            //    float fMinSpanValue, fMinResultValue, fMaxResultValue, fMaxSpanValue;
            //    fMinSpanValue = Convert.ToSingle(txt_MinSpan.Text);
            //    fMaxSpanValue = Convert.ToSingle(txt_MaxSpan.Text);
            //    fMinResultValue = Convert.ToSingle(lbl_ResultMinSpan.Text);
            //    fMaxResultValue = Convert.ToSingle(lbl_ResultMaxSpan.Text);

            //    if (fMinResultValue < fMinSpanValue)
            //        lbl_ResultMinSpan.ForeColor = Color.Red;
            //    else
            //        lbl_ResultMinSpan.ForeColor = Color.Black;

            //    if (fMaxResultValue > fMaxSpanValue)
            //        lbl_ResultMaxSpan.ForeColor = Color.Red;
            //    else
            //        lbl_ResultMaxSpan.ForeColor = Color.Black;
            //}
        }

        /// <summary>
        /// Read Lead template data to datagridview
        /// </summary>
        /// <param name="intLeadIndex">Lead position</param>
        /// <param name="dgd_LeadSetting">datagridview</param>
        private void ReadLeadTemplateDataToGrid(int intLeadIndex, DataGridView dgd_LeadSetting)
        {
            List<List<string>> arrBlobsFeaturesData;
            arrBlobsFeaturesData = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetBlobsFeaturesInspectRealData();

            int intFailMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;
            dgd_LeadSetting.Rows.Clear();

            for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
            {
                dgd_LeadSetting.Rows.Add();
                dgd_LeadSetting.Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0] + " " + (Convert.ToInt32(arrBlobsFeaturesData[i][1]) + 1).ToString();

                // Max OffSet
                dgd_LeadSetting.Rows[i].Cells[0].Value = arrBlobsFeaturesData[i][2];
                if ((intFailMask & 0x20000) == 0)
                {
                    dgd_LeadSetting.Columns[0].Visible = false;
                    dgd_LeadSetting.Columns[1].Visible = false;
                    dgd_LeadSetting.Columns[2].Visible = false;
                }

                // Max Skew
                dgd_LeadSetting.Rows[i].Cells[3].Value = arrBlobsFeaturesData[i][3];
                if ((intFailMask & 0x100) == 0)
                {
                    dgd_LeadSetting.Columns[3].Visible = false;
                    dgd_LeadSetting.Columns[4].Visible = false;
                    dgd_LeadSetting.Columns[5].Visible = false;
                }

                // Min Max Width
                dgd_LeadSetting.Rows[i].Cells[6].Value = arrBlobsFeaturesData[i][4];
                dgd_LeadSetting.Rows[i].Cells[8].Value = arrBlobsFeaturesData[i][5];
                if ((intFailMask & 0x40) == 0)
                {
                    dgd_LeadSetting.Columns[6].Visible = false;
                    dgd_LeadSetting.Columns[7].Visible = false;
                    dgd_LeadSetting.Columns[8].Visible = false;
                    dgd_LeadSetting.Columns[9].Visible = false;
                }

                // Min Max Length
                dgd_LeadSetting.Rows[i].Cells[10].Value = arrBlobsFeaturesData[i][6];
                dgd_LeadSetting.Rows[i].Cells[12].Value = arrBlobsFeaturesData[i][7];
                if ((intFailMask & 0x80) == 0)
                {
                    dgd_LeadSetting.Columns[10].Visible = false;
                    dgd_LeadSetting.Columns[11].Visible = false;
                    dgd_LeadSetting.Columns[12].Visible = false;
                    dgd_LeadSetting.Columns[13].Visible = false;
                }

                // Min Max Pitch
                if (Convert.ToSingle(arrBlobsFeaturesData[i][8]) == -1)
                    dgd_LeadSetting.Rows[i].Cells[14].Value = "---";
                else
                    dgd_LeadSetting.Rows[i].Cells[14].Value = arrBlobsFeaturesData[i][8];

                if (Convert.ToSingle(arrBlobsFeaturesData[i][9]) == -1)
                    dgd_LeadSetting.Rows[i].Cells[16].Value = "---";
                else
                    dgd_LeadSetting.Rows[i].Cells[16].Value = arrBlobsFeaturesData[i][9];

                if ((intFailMask & 0x600) == 0)
                {
                    dgd_LeadSetting.Columns[14].Visible = false;
                    dgd_LeadSetting.Columns[15].Visible = false;
                    dgd_LeadSetting.Columns[16].Visible = false;
                    dgd_LeadSetting.Columns[17].Visible = false;
                }

                // Min Max Gap
                if (Convert.ToSingle(arrBlobsFeaturesData[i][10]) == -1)
                    dgd_LeadSetting.Rows[i].Cells[18].Value = "---";
                else
                    dgd_LeadSetting.Rows[i].Cells[18].Value = arrBlobsFeaturesData[i][10];

                if (Convert.ToSingle(arrBlobsFeaturesData[i][11]) == -1)
                    dgd_LeadSetting.Rows[i].Cells[20].Value = "---";
                else
                    dgd_LeadSetting.Rows[i].Cells[20].Value = arrBlobsFeaturesData[i][11];

                if ((intFailMask & 0x600) == 0)
                {
                    dgd_LeadSetting.Columns[18].Visible = false;
                    dgd_LeadSetting.Columns[19].Visible = false;
                    dgd_LeadSetting.Columns[20].Visible = false;
                    dgd_LeadSetting.Columns[21].Visible = false;
                }

                // Min Max Stand Off
                dgd_LeadSetting.Rows[i].Cells[22].Value = arrBlobsFeaturesData[i][12];
                dgd_LeadSetting.Rows[i].Cells[24].Value = arrBlobsFeaturesData[i][13];
                if ((intFailMask & 0x01) == 0)
                {
                    dgd_LeadSetting.Columns[22].Visible = false;
                    dgd_LeadSetting.Columns[23].Visible = false;
                    dgd_LeadSetting.Columns[24].Visible = false;
                    dgd_LeadSetting.Columns[25].Visible = false;
                }

                // Min Max Solfer Pad Length
                dgd_LeadSetting.Rows[i].Cells[26].Value = arrBlobsFeaturesData[i][14];
                dgd_LeadSetting.Rows[i].Cells[28].Value = arrBlobsFeaturesData[i][15];
                {
                    dgd_LeadSetting.Columns[26].Visible = false;
                    dgd_LeadSetting.Columns[27].Visible = false;
                    dgd_LeadSetting.Columns[28].Visible = false;
                    dgd_LeadSetting.Columns[29].Visible = false;
                }

                // Max Coplan
                dgd_LeadSetting.Rows[i].Cells[30].Value = arrBlobsFeaturesData[i][16];
                if ((intFailMask & 0x02) == 0)
                {
                    dgd_LeadSetting.Columns[30].Visible = false;
                    dgd_LeadSetting.Columns[31].Visible = false;
                    dgd_LeadSetting.Columns[32].Visible = false;
                }

                // Min Max Average Gray Value
                dgd_LeadSetting.Rows[i].Cells[33].Value = arrBlobsFeaturesData[i][17];
                dgd_LeadSetting.Rows[i].Cells[35].Value = arrBlobsFeaturesData[i][18];
                if (!m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod || ((intFailMask & 0x40000) == 0))
                {
                    dgd_LeadSetting.Columns[33].Visible = false;
                    dgd_LeadSetting.Columns[34].Visible = false;
                    dgd_LeadSetting.Columns[35].Visible = false;
                    dgd_LeadSetting.Columns[36].Visible = false;
                }

                // Max Lead Min Max Width Limit
                dgd_LeadSetting.Rows[i].Cells[37].Value = arrBlobsFeaturesData[i][19];
                dgd_LeadSetting.Rows[i].Cells[40].Value = arrBlobsFeaturesData[i][20];
                if ((intFailMask & 0x100000) == 0)
                {
                    dgd_LeadSetting.Columns[37].Visible = false;
                    dgd_LeadSetting.Columns[38].Visible = false;
                    dgd_LeadSetting.Columns[39].Visible = false;
                    dgd_LeadSetting.Columns[40].Visible = false;
                    dgd_LeadSetting.Columns[41].Visible = false;
                }

                // Max Burr Width
                dgd_LeadSetting.Rows[i].Cells[42].Value = arrBlobsFeaturesData[i][21];
                if ((intFailMask & 0x200000) == 0)
                {
                    dgd_LeadSetting.Columns[42].Visible = false;
                    dgd_LeadSetting.Columns[43].Visible = false;
                    dgd_LeadSetting.Columns[44].Visible = false;
                }

                //// Offset Setting
                //dgd_LeadSetting.Rows[i].Cells[45].Value = arrBlobsFeaturesData[i][22];
                //if ((intFailMask & 0x40) == 0)
                //{
                //    dgd_LeadSetting.Columns[45].Visible = false;
                //    dgd_LeadSetting.Columns[46].Visible = false;
                //}

                //dgd_LeadSetting.Rows[i].Cells[47].Value = arrBlobsFeaturesData[i][23];
                //if ((intFailMask & 0x80) == 0)
                //{
                //    dgd_LeadSetting.Columns[47].Visible = false;
                //    dgd_LeadSetting.Columns[48].Visible = false;
                //}

                //dgd_LeadSetting.Rows[i].Cells[49].Value = arrBlobsFeaturesData[i][24];
                //dgd_LeadSetting.Rows[i].Cells[51].Value = arrBlobsFeaturesData[i][25];
                //if ((intFailMask & 0x600) == 0)
                //{
                //    dgd_LeadSetting.Columns[49].Visible = false;
                //    dgd_LeadSetting.Columns[50].Visible = false;
                //    dgd_LeadSetting.Columns[51].Visible = false;
                //    dgd_LeadSetting.Columns[52].Visible = false;
                //}

                //dgd_LeadSetting.Rows[i].Cells[53].Value = arrBlobsFeaturesData[i][26];
                //if ((intFailMask & 0x01) == 0)
                //{
                //    dgd_LeadSetting.Columns[53].Visible = false;
                //    dgd_LeadSetting.Columns[54].Visible = false;
                //}

                //dgd_LeadSetting.Rows[i].Cells[55].Value = arrBlobsFeaturesData[i][27];
                //if ((intFailMask & 0x02) == 0)
                //{
                //    dgd_LeadSetting.Columns[55].Visible = false;
                //    dgd_LeadSetting.Columns[56].Visible = false;
                //}

            }
        }

        private void ColorGrid(DataGridView dgd_LeadSetting)
        {
            for (int i = 0; i < dgd_LeadSetting.Rows.Count; i++)
            {
                // OffSet
                dgd_LeadSetting.Rows[i].Cells[0].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[1].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.White;

                // Skew
                dgd_LeadSetting.Rows[i].Cells[3].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[4].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.White;

                // Width
                dgd_LeadSetting.Rows[i].Cells[6].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[6].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[7].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[8].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.White;

                // Length
                dgd_LeadSetting.Rows[i].Cells[10].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[10].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[11].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[12].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.White;

                // Pitch
                dgd_LeadSetting.Rows[i].Cells[14].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[14].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[15].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[16].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[16].Style.SelectionBackColor = Color.White;

                // Gap
                dgd_LeadSetting.Rows[i].Cells[18].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[18].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[20].Style.BackColor = Color.White;
                dgd_LeadSetting.Rows[i].Cells[20].Style.SelectionBackColor = Color.White;

                //// Variance
                //dgd_LeadSetting.Rows[i].Cells[18].Style.BackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[18].Style.SelectionBackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[19].Style.BackColor = Color.White;
                //dgd_LeadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.White;
            }
        }

        private void LoadLeadSetting(string strFolderPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                //m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                //              m_smVisionInfo.g_fCalibPixelX,
                //              m_smVisionInfo.g_fCalibPixelY,
                //              m_smVisionInfo.g_fCalibOffSetX,
                //              m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
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

                m_smVisionInfo.g_arrLead3D[i].LoadLead3D(strFolderPath + "Template\\Template.xml", strSectionName);

                m_smVisionInfo.g_arrLead3D[i].LoadLeadTemplateImage(strFolderPath + "Template\\", i);
                if (i == 0)
                    m_smVisionInfo.g_arrLead3D[i].LoadUnitPattern(strFolderPath + "Template\\PatternMatcher0.mch");
            }
        }

        private bool SaveLeadSettings(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
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

                
                m_smVisionInfo.g_arrLead3D[i].SaveLead3D(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                
            }
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Tolerance Setting", m_smProductionInfo.g_strLotID);
            
            return true;
        }

        private void UpdateLeadMeasurementIntoTable()
        {

        }


        private void LeadToleranceSettingForm_Load(object sender, EventArgs e)
        {
            m_blnFormOpen = true;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
        }

        private void LeadToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.AT_VM_OfflineTestAllLead3D = false;
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Tolerance Setting Closed", "Exit Lead3D Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_blnFormOpen = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            // Save Lead Setting
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\";

            SaveLeadSettings(strPath + "Lead3D\\");

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

            //CheckSaveGoldenData(m_intLeadIndex, m_dgdLeadSettingView);
            // Load Lead Setting
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            LoadLeadSetting(strFolderPath + "Lead3D\\");

            this.Close();
            this.Dispose();
        }

        public bool IsSettingError()
        {
            int[] arrColumnIndex = { 6, 10, 14, 18, 22, 26, 37 };

            for (int c = 0; c < arrColumnIndex.Length; c++)
            {
                for (int i = 0; i < m_dgdLeadSettingView.Rows.Count; i++)
                {
                    if (m_dgdLeadSettingView.Rows[i].Cells[arrColumnIndex[c]].Value.ToString() != "---")
                    {
                        // Check insert data valid or not
                        float fMin = float.Parse(m_dgdLeadSettingView.Rows[i].Cells[arrColumnIndex[c]].Value.ToString());
                        float fMax;
                        if (arrColumnIndex[c] == 37)    // Limit Min Width 
                            fMax = float.Parse(m_dgdLeadSettingView.Rows[i].Cells[arrColumnIndex[c] + 3].Value.ToString()); // Middle of Min and Max have 2 column for this setting.
                        else
                            fMax = float.Parse(m_dgdLeadSettingView.Rows[i].Cells[arrColumnIndex[c] + 2].Value.ToString());

                        if (fMin > fMax)
                        {
                            return true;
                        }
                    }

                    ////Update template pitch gap setting
                    //m_smVisionInfo.g_arrLead3D[m_intLeadIndex].SetPitchGapDataFrom(i,
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
            int intFailMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;

            // Individual Lead Setting Table
            if ((intFailMask & 0x20000) == 0)
                m_dgdLeadSettingView.Columns[1].Visible = false;
            else
                m_dgdLeadSettingView.Columns[1].Visible = blnWantView;

            if ((intFailMask & 0x100) == 0)
                m_dgdLeadSettingView.Columns[4].Visible = false;
            else
                m_dgdLeadSettingView.Columns[4].Visible = blnWantView;

            if ((intFailMask & 0x40) == 0)
                m_dgdLeadSettingView.Columns[7].Visible = false;
            else
                m_dgdLeadSettingView.Columns[7].Visible = blnWantView;

            if ((intFailMask & 0x80) == 0)
                m_dgdLeadSettingView.Columns[11].Visible = false;
            else
                m_dgdLeadSettingView.Columns[11].Visible = blnWantView;

            if ((intFailMask & 0x600) == 0)
            {
                m_dgdLeadSettingView.Columns[15].Visible = false;
                m_dgdLeadSettingView.Columns[19].Visible = false;
            }
            else
            {
                m_dgdLeadSettingView.Columns[15].Visible = blnWantView;
                m_dgdLeadSettingView.Columns[19].Visible = blnWantView;
            }

            if ((intFailMask & 0x01) == 0)
                m_dgdLeadSettingView.Columns[23].Visible = false;
            else
                m_dgdLeadSettingView.Columns[23].Visible = blnWantView;

            //m_dgdLeadSettingView.Columns[27].Visible = blnWantView;
            if ((intFailMask & 0x02) == 0)
                m_dgdLeadSettingView.Columns[31].Visible = false;
            else
                m_dgdLeadSettingView.Columns[31].Visible = blnWantView;

            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod || ((intFailMask & 0x40000) == 0))
                m_dgdLeadSettingView.Columns[34].Visible = false;
            else
                m_dgdLeadSettingView.Columns[34].Visible = blnWantView;

            if ((intFailMask & 0x100000) == 0)
            {
                m_dgdLeadSettingView.Columns[38].Visible = false; // Lead Min Width Result
                m_dgdLeadSettingView.Columns[39].Visible = false; // Lead Max Width Result
            }
            else
            {
                m_dgdLeadSettingView.Columns[38].Visible = blnWantView; // Lead Min Width Result
                m_dgdLeadSettingView.Columns[39].Visible = blnWantView; // Lead Max Width Result
            }

            if ((intFailMask & 0x200000) == 0)
                m_dgdLeadSettingView.Columns[42].Visible = false; // Lead Burr Width Result
            else
                m_dgdLeadSettingView.Columns[42].Visible = blnWantView; // Lead Burr Width Result

            // Whole Leads Setting Table
            m_dgdWholeLeadSettingView.Columns[3].Visible = blnWantView;

        }

        private void timer_LeadResult_Tick(object sender, EventArgs e)
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
            if (m_smVisionInfo.PR_TL_UpdateInfo)
            {
                m_smVisionInfo.PR_TL_UpdateInfo = false;

                UpdateScore(m_intLeadIndex, m_dgdLeadSettingView);
                UpdateWholeLeadScore();
            }
        }

        private void chk_ViewGoldenColumn_Click(object sender, EventArgs e)
        {
            //if (objGoldenUnitForm.ref_intViewGoldenDataColumn)
            //{
            //    m_dgdLeadSettingView.Columns.Add(column_GolWidth);
            //    m_dgdLeadSettingView.Columns.Add(column_GolLength);
            //    m_dgdLeadSettingView.Columns.Add(column_GolPitch);
            //    m_dgdLeadSettingView.Columns.Add(column_GolGap);

            //    UpdateGoldenDataIntoGridTable(m_intLeadIndex, m_dgdLeadSettingView);
            //}
            //else
            //{
            //    m_dgdLeadSettingView.Columns.Remove(column_GolLength);
            //    m_dgdLeadSettingView.Columns.Remove(column_GolWidth);
            //    m_dgdLeadSettingView.Columns.Remove(column_GolPitch);
            //    m_dgdLeadSettingView.Columns.Remove(column_GolGap);

            //    CheckSaveGoldenData(m_intLeadIndex, m_dgdLeadSettingView);
            //}
        }

        private void btn_GoldenUnitSetting_Click(object sender, EventArgs e)
        {
            //if (objGoldenUnitForm == null)
            //{
            //    objGoldenUnitForm = new GoldenUnitCompensationForm(m_smCustomizeInfo,
            //                        m_smVisionInfo, m_strSelectedRecipe, m_intUserGroup);
            //}

            //CheckSaveGoldenData(m_intLeadIndex, m_dgdLeadSettingView);

            //objGoldenUnitForm.SetSelectedLeadIndex(m_intLeadIndex);

            //objGoldenUnitForm.TopMost = true;
            //objGoldenUnitForm.ShowDialog();

            //SaveGoldenData();

            //m_intGDSelectedIndex = objGoldenUnitForm.ref_intGoldenUnitSelectedIndex;

            //if (m_intGDSelectedIndex >= 0 && objGoldenUnitForm.ref_intViewGoldenDataColumn)
            //{
            //    if (m_dgdLeadSettingView.Columns.Count <= 25)
            //    {
            //        m_dgdLeadSettingView.Columns.Add(column_GolWidth);
            //        m_dgdLeadSettingView.Columns.Add(column_GolLength);
            //        m_dgdLeadSettingView.Columns.Add(column_GolPitch);
            //        m_dgdLeadSettingView.Columns.Add(column_GolGap);
            //    }
            //    UpdateGoldenDataIntoGridTable(m_intLeadIndex, m_dgdLeadSettingView);

            //    UpdateScore(m_intLeadIndex, m_dgdLeadSettingView);

            //    btn_SaveAccuracyReport.Visible = true;
            //}
            //else
            //{
            //    if (m_dgdLeadSettingView.Columns.Count > 25)
            //    {
            //        m_dgdLeadSettingView.Columns.Remove(column_GolLength);
            //        m_dgdLeadSettingView.Columns.Remove(column_GolWidth);
            //        m_dgdLeadSettingView.Columns.Remove(column_GolPitch);
            //        m_dgdLeadSettingView.Columns.Remove(column_GolGap);
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
                arrData.Add("Total LeadS: " + m_dgdLeadSettingView.Rows.Count.ToString());
                arrData.Add("");

                float fGoldenUnitValue = 0;
                float fVisionValue = 0;
                for (int i = 0; i < m_dgdLeadSettingView.Rows.Count; i++)
                {
                    arrData.Add("Lead " + (i + 1).ToString().PadRight(2, ' ') +
                        "\t\tGolden Unit    \t\tVision System  \t\tDeviation");

                    if (!float.TryParse(m_dgdLeadSettingView.Rows[i].Cells[45].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdLeadSettingView.Rows[i].Cells[8].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Width \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdLeadSettingView.Rows[i].Cells[46].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdLeadSettingView.Rows[i].Cells[12].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Length\t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdLeadSettingView.Rows[i].Cells[47].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdLeadSettingView.Rows[i].Cells[16].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Pitch \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdLeadSettingView.Rows[i].Cells[48].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdLeadSettingView.Rows[i].Cells[20].Value.ToString(), out fVisionValue))
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
                e.ColumnIndex == 15 || e.ColumnIndex == 19 || e.ColumnIndex == 23 ||
                e.ColumnIndex == 27 || e.ColumnIndex == 31 || e.ColumnIndex == 34 ||
                e.ColumnIndex == 38 || e.ColumnIndex == 39 || e.ColumnIndex == 43)
                return;

            // Skip if col is separation
            if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 9 || e.ColumnIndex == 13 ||
                e.ColumnIndex == 17 || e.ColumnIndex == 21 || e.ColumnIndex == 25 ||
                e.ColumnIndex == 29 || e.ColumnIndex == 32 || e.ColumnIndex == 36 || e.ColumnIndex == 41 || e.ColumnIndex == 44)//||
                //e.ColumnIndex == 46 || e.ColumnIndex == 48 || e.ColumnIndex == 50 || e.ColumnIndex == 52 || e.ColumnIndex == 54 || e.ColumnIndex == 56)
                return;

            // Skip if col is golden unit data
            if (e.ColumnIndex == 45 || e.ColumnIndex == 46 || e.ColumnIndex == 47 || e.ColumnIndex == 48)
                return;

            //Skip if cell value is ---
            if ((((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "---") ||
                (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == ""))
                return;

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

            string strCurrentSetValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            //SetValueForm objSetValueForm = new SetValueForm(e.RowIndex, intDecimalPlaces, strCurrentSetValue);
            SetValueForm objSetValueForm = new SetValueForm("Set value to Lead " + (e.RowIndex + 1).ToString(), m_strUnitLabel, e.RowIndex, m_intDecimalPlaces, strCurrentSetValue);
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
                        if (e.ColumnIndex == 6 || e.ColumnIndex == 10 || e.ColumnIndex == 14 || e.ColumnIndex == 18 || e.ColumnIndex == 22 || e.ColumnIndex == 26)
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
                        else if (e.ColumnIndex == 8 || e.ColumnIndex == 12 || e.ColumnIndex == 16 || e.ColumnIndex == 20 || e.ColumnIndex == 24 || e.ColumnIndex == 28)
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

                    float fMinGap, fMaxGap;
                    if (((DataGridView)sender).Rows[i].Cells[18].Value.ToString() == "---")
                        fMinGap = -1;
                    else
                        fMinGap = float.Parse(((DataGridView)sender).Rows[i].Cells[18].Value.ToString());

                    if (((DataGridView)sender).Rows[i].Cells[20].Value.ToString() == "---")
                        fMaxGap = -1;
                    else
                        fMaxGap = float.Parse(((DataGridView)sender).Rows[i].Cells[20].Value.ToString());

                    float fSkew = -999;
                    if (((DataGridView)sender).Rows[i].Cells[3].Value.ToString() != "---")
                        fSkew = float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString());

                    for (int h = 0; h < m_smVisionInfo.g_arrLead3D.Length; h++)
                    {
                        //Update template setting
                        m_smVisionInfo.g_arrLead3D[h].UpdateBlobFeatureToPixel(i,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                            fSkew,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[6].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[8].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[10].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[12].Value.ToString()),
                            fMinPitch,
                            fMaxPitch,
                            fMinGap,
                            fMaxGap,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[22].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[24].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[26].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[28].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[30].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[33].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[35].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[37].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[40].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[42].Value.ToString())//,
                            //float.Parse(((DataGridView)sender).Rows[i].Cells[45].Value.ToString()),
                            //float.Parse(((DataGridView)sender).Rows[i].Cells[47].Value.ToString()),
                            //float.Parse(((DataGridView)sender).Rows[i].Cells[49].Value.ToString()),
                            //float.Parse(((DataGridView)sender).Rows[i].Cells[51].Value.ToString()),
                            //float.Parse(((DataGridView)sender).Rows[i].Cells[53].Value.ToString()),
                            //float.Parse(((DataGridView)sender).Rows[i].Cells[55].Value.ToString())
                            );

                        //Update template pitch gap setting
                        m_smVisionInfo.g_arrLead3D[h].SetPitchGapDataFrom(i,
                           fMinPitch,
                           fMaxPitch,
                           fMinGap,
                           fMaxGap);
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

                    m_smVisionInfo.g_arrLead3D[i].LoadLeadToleranceFromFile(dlg_LoadToleranceFile.FileName, strSectionName);

                    // Update table
                    ReadLeadTemplateDataToGrid(m_intLeadIndex, m_dgdLeadSettingView);
                    UpdateScore(m_intLeadIndex, m_dgdLeadSettingView);
                    UpdateWholdLeadsSettingGUI();
                    UpdateWholeLeadScore();
                    //txt_MinSpan.Text = m_smVisionInfo.g_arrLead3D[0].ref_fTemplateLeadMinSpanLimit.ToString("F" + m_intDecimal);
                    //txt_MaxSpan.Text = m_smVisionInfo.g_arrLead3D[0].ref_fTemplateLeadMaxSpanLimit.ToString("F" + m_intDecimal);
                }
            }

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void btn_SaveTolToFile_Click(object sender, EventArgs e)
        {
            if (dlg_SaveToleranceFile.ShowDialog() == DialogResult.OK)
            {
                
                //STDeviceEdit.CopySettingFile(dlg_SaveToleranceFile.FileName, "");
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

                    
                    m_smVisionInfo.g_arrLead3D[i].SaveLeadToleranceToFile(dlg_SaveToleranceFile.FileName, false, strSectionName, true);
                    
                }
                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Lead3D Tolerance Setting", m_smProductionInfo.g_strLotID);
                
            }

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void btn_UpdateTolerance_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            Point p = btn_UpdateTolerance.PointToScreen(Point.Empty);
            LeadUpdateToleranceUpperLowerLimitForm objForm = new LeadUpdateToleranceUpperLowerLimitForm(m_smVisionInfo, m_smCustomizeInfo, m_strSelectedRecipe, m_intUserGroup,m_smProductionInfo, true);
            objForm.StartPosition = FormStartPosition.Manual;
            int intMainFormLocationX = 0;
            int intMainFormLocationY = 0;
            objForm.Location = GetFormStartPoint(p, btn_UpdateTolerance.Size, objForm.Size, ref intMainFormLocationX, ref intMainFormLocationY);

            if (objForm.ShowDialog() == DialogResult.Yes)
            {
                int[] intResultColIndex = { 7, 11, 15, 19 }; // Width, Length, Pitch, Gap

                for (int i = 0; i < m_dgdLeadSettingView.Rows.Count; i++)
                {
                    for (int c = 0; c < intResultColIndex.Length; c++)
                    {

                        if (m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c]].Value.ToString() != "---")
                        {
                            float fNorminalResult = float.Parse(m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c]].Value.ToString());

                            if (fNorminalResult != 0)
                            {
                                switch (intResultColIndex[c])
                                {
                                    case 7:
                                        {
                                            m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c] - 1].Value = Math.Round(fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100, 4, MidpointRounding.AwayFromZero);
                                            m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c] + 1].Value = Math.Round(fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100, 4, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                    case 11:
                                        {
                                            m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c] - 1].Value = Math.Round(fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100, 4, MidpointRounding.AwayFromZero);
                                            m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c] + 1].Value = Math.Round(fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100, 4, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                    case 15:
                                        {
                                            m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c] - 1].Value = Math.Round(fNorminalResult * objForm.ref_fLowerLimit_Pitch / 100, 4, MidpointRounding.AwayFromZero);
                                            m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c] + 1].Value = Math.Round(fNorminalResult * objForm.ref_fUpperLimit_Pitch / 100, 4, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                    case 19:
                                        {
                                            m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c] - 1].Value = Math.Round(fNorminalResult * objForm.ref_fLowerLimit_Gap / 100, 4, MidpointRounding.AwayFromZero);
                                            m_dgdLeadSettingView.Rows[i].Cells[intResultColIndex[c] + 1].Value = Math.Round(fNorminalResult * objForm.ref_fUpperLimit_Gap / 100, 4, MidpointRounding.AwayFromZero);
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    //Set value column selected
                    float fMinPitch, fMaxPitch;
                    if ((m_dgdLeadSettingView.Rows[i].Cells[14].Value.ToString() == "---"))
                        fMinPitch = -1;
                    else
                        fMinPitch = float.Parse(m_dgdLeadSettingView.Rows[i].Cells[14].Value.ToString());

                    if (m_dgdLeadSettingView.Rows[i].Cells[16].Value.ToString() == "---")
                        fMaxPitch = -1;
                    else
                        fMaxPitch = float.Parse(m_dgdLeadSettingView.Rows[i].Cells[16].Value.ToString());

                    //int intLengthMode = m_smVisionInfo.g_arrLead3D[m_intLeadIndex].GetSampleLengthMode(i);

                    float fMinGap, fMaxGap;
                    if (m_dgdLeadSettingView.Rows[i].Cells[18].Value.ToString() == "---")
                        fMinGap = -1;
                    else
                        fMinGap = float.Parse(m_dgdLeadSettingView.Rows[i].Cells[18].Value.ToString());

                    if (m_dgdLeadSettingView.Rows[i].Cells[20].Value.ToString() == "---")
                        fMaxGap = -1;
                    else
                        fMaxGap = float.Parse(m_dgdLeadSettingView.Rows[i].Cells[20].Value.ToString());
                    //Update template setting
                    //m_smVisionInfo.g_arrLead3D[m_intLeadIndex].UpdateBlobFeatureToPixel(i,
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                    //    fMinPitch,
                    //    fMaxPitch,
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[15].Value.ToString()),
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[17].Value.ToString()),
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[19].Value.ToString()),
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[21].Value.ToString()),
                    //    float.Parse(((DataGridView)sender).Rows[i].Cells[23].Value.ToString())
                    //    );

                    for (int h = 0; h < m_smVisionInfo.g_arrLead3D.Length; h++)
                    {
                        m_smVisionInfo.g_arrLead3D[h].UpdateBlobFeatureToPixel(i,
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[0].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[3].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[6].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[8].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[10].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[12].Value.ToString()),
                            fMinPitch,
                            fMaxPitch,
                            fMinGap,
                            fMaxGap,
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[22].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[24].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[26].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[28].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[30].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[33].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[35].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[37].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[40].Value.ToString()),
                            float.Parse(m_dgdLeadSettingView.Rows[i].Cells[42].Value.ToString())//,
                            //float.Parse(m_dgdLeadSettingView.Rows[i].Cells[45].Value.ToString()),
                            //float.Parse(m_dgdLeadSettingView.Rows[i].Cells[47].Value.ToString()),
                            //float.Parse(m_dgdLeadSettingView.Rows[i].Cells[49].Value.ToString()),
                            //float.Parse(m_dgdLeadSettingView.Rows[i].Cells[51].Value.ToString()),
                            //float.Parse(m_dgdLeadSettingView.Rows[i].Cells[53].Value.ToString()),
                            //float.Parse(m_dgdLeadSettingView.Rows[i].Cells[55].Value.ToString())
                            );

                        //Update template pitch gap setting
                        m_smVisionInfo.g_arrLead3D[h].SetPitchGapDataFrom(i,
                           fMinPitch,
                           fMaxPitch,
                           fMinGap,
                           fMaxGap);
                    }
                }

                float fNorminalMinSpanResult = 0;
                float fNorminalMaxSpanResult = 0;

                //if (lbl_ResultMinSpan.Text != "---")
                //    fNorminalMinSpanResult = (float)Math.Round(float.Parse(lbl_ResultMinSpan.Text) * objForm.ref_fLowerLimit_Span / 100, 4, MidpointRounding.AwayFromZero);

                //if (lbl_ResultMaxSpan.Text != "---")
                //    fNorminalMaxSpanResult = (float)Math.Round(float.Parse(lbl_ResultMaxSpan.Text) * objForm.ref_fUpperLimit_Span / 100, 4, MidpointRounding.AwayFromZero);

                //if (fNorminalMinSpanResult != 0)
                //    txt_MinSpan.Text = fNorminalMinSpanResult.ToString("F" + m_intDecimal);
                //if (fNorminalMaxSpanResult != 0)
                //    txt_MaxSpan.Text = fNorminalMaxSpanResult.ToString("F" + m_intDecimal);
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

        private void dgd_WholeLeadSetting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgd_WholeLeadSetting_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgd_WholeLeadSetting_Leave(object sender, EventArgs e)
        {

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
                case "Pitch Variance (Left)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceLeftMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Pitch Variance (Right)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceRightMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Pitch Variance (Top)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceTopMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Pitch Variance (Bottom)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fPitchVarianceBottomMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Unit)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceOverallMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Left)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceLeftMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Right)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceRightMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Top)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceTopMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Length Variance (Bottom)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLengthVarianceBottomMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Stand Off Variance (Left)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceLeftMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Stand Off Variance (Right)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceRightMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Stand Off Variance (Top)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceTopMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Stand Off Variance (Bottom)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceBottomMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Stand Off Variance (Unit)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fStandOffVarianceUnitMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Leads Sweep (Left)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceLeftMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Leads Sweep (Right)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceRightMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Leads Sweep (Top)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceTopMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Leads Sweep (Bottom)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fLeadSweepVarianceBottomMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Unit)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanOverallMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Top)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanTopMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Bottom)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanBottomMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Left)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanLeftMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "Span (Right)":
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightMinSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[1].Value);
                    m_smVisionInfo.g_arrLead3D[0].ref_fSpanRightMaxSetting = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "F.Material / Cont. (Length)":
                    for (int j = 0; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                        m_smVisionInfo.g_arrLead3D[j].ref_fExtraLeadSetLength = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "F.Material / Cont. (Area)":
                    for (int j = 0; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                        m_smVisionInfo.g_arrLead3D[j].ref_fExtraLeadSetArea = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
                case "F.Material / Cont. (Total Area)":
                    for (int j = 0; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                        m_smVisionInfo.g_arrLead3D[j].ref_fTotalExtraLeadSetArea = Convert.ToSingle(dgd_WholeLeadSetting.Rows[i].Cells[2].Value);
                    break;
            }
        }

        private void dgd_TopLead_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;
            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex == 1 || e.ColumnIndex == 4 || e.ColumnIndex == 7 || e.ColumnIndex == 11 ||
                e.ColumnIndex == 15 || e.ColumnIndex == 19 || e.ColumnIndex == 23 || e.ColumnIndex == 27 || e.ColumnIndex == 31 || e.ColumnIndex == 34 || e.ColumnIndex == 38 || e.ColumnIndex == 39)
                return;

            // Skip if col is separation
            if (e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 9 || e.ColumnIndex == 13 ||
                e.ColumnIndex == 17 || e.ColumnIndex == 21 || e.ColumnIndex == 25 || e.ColumnIndex == 29 || e.ColumnIndex == 32 || e.ColumnIndex == 36 ||
                e.ColumnIndex == 41 || e.ColumnIndex == 44 /*|| e.ColumnIndex == 46 || e.ColumnIndex == 48 || e.ColumnIndex == 50 || e.ColumnIndex == 52 || e.ColumnIndex == 54 || e.ColumnIndex == 56*/)
                return;

            // Skip if col is golden unit data
            if (e.ColumnIndex == 45 || e.ColumnIndex == 46 || e.ColumnIndex == 47 || e.ColumnIndex == 48)
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
            if (e.ColumnIndex == 33 || e.ColumnIndex == 35) // no unit label for average gray value
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
                        if (e.ColumnIndex == 6 || e.ColumnIndex == 10 || e.ColumnIndex == 14 || e.ColumnIndex == 18 || e.ColumnIndex == 22 || e.ColumnIndex == 26)
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
                        else if (e.ColumnIndex == 8 || e.ColumnIndex == 12 || e.ColumnIndex == 16 || e.ColumnIndex == 20 || e.ColumnIndex == 24 || e.ColumnIndex == 28)
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

                    //int intLengthMode = m_smVisionInfo.g_arrLead3D[m_intLeadIndex].GetSampleLengthMode(i);

                    //Update template setting
                    //if (intLengthMode == 1)
                    //{

                    float fSkew = -999;
                    if (((DataGridView)sender).Rows[i].Cells[3].Value.ToString() != "---")
                        fSkew = float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString());

                    for (int h = 0; h < m_smVisionInfo.g_arrLead3D.Length; h++)
                    {
                        m_smVisionInfo.g_arrLead3D[h].UpdateBlobFeatureToPixel(i,
                                float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                                fSkew,
                                float.Parse(((DataGridView)sender).Rows[i].Cells[6].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[8].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[10].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[12].Value.ToString()),
                                fMinPitch,
                                fMaxPitch,
                                fMinGap,
                                fMaxGap,
                                float.Parse(((DataGridView)sender).Rows[i].Cells[22].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[24].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[26].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[28].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[30].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[33].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[35].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[37].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[40].Value.ToString()),
                                float.Parse(((DataGridView)sender).Rows[i].Cells[42].Value.ToString())//,
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[45].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[47].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[49].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[51].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[53].Value.ToString()),
                                //float.Parse(((DataGridView)sender).Rows[i].Cells[55].Value.ToString())
                                );
                        //Update template pitch gap setting
                        m_smVisionInfo.g_arrLead3D[h].SetPitchGapDataFrom(i,
                           fMinPitch,
                           fMaxPitch,
                           fMinGap,
                           fMaxGap);
                    }


                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }

            this.TopMost = true;
        }

        private void btn_LeadSkewInspectOption_Click(object sender, EventArgs e)
        {
            Lead3DSkewInspectOptionForm objForm = new Lead3DSkewInspectOptionForm(m_smCustomizeInfo, m_smVisionInfo,
                         m_smProductionInfo, m_strSelectedRecipe, m_intUserGroup);
            if (objForm.ShowDialog() == DialogResult.OK && m_dgdLeadSettingView.Columns[4].Visible)
            {
                m_blnInitDone = false;

                for (int i = 0; i < m_dgdLeadSettingView.RowCount; i++)
                {
                    if (!m_smVisionInfo.g_arrLead3D[0].GetWantCheckSkew(i))
                    {
                        m_dgdLeadSettingView.Rows[i].Cells[3].Value = "---";
                        m_dgdLeadSettingView.Rows[i].Cells[4].Value = "---";
                        m_dgdLeadSettingView.Rows[i].Cells[4].Style.BackColor = Color.White;
                        m_dgdLeadSettingView.Rows[i].Cells[4].Style.SelectionBackColor = Color.White;
                        m_dgdLeadSettingView.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                        m_dgdLeadSettingView.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;

                    }
                    else
                    {
                        float fSkewLimit = m_smVisionInfo.g_arrLead3D[0].GetSkewLimit(i);
                        if (fSkewLimit == -1)
                            m_dgdLeadSettingView.Rows[i].Cells[3].Value = "---";
                        else
                            m_dgdLeadSettingView.Rows[i].Cells[3].Value = fSkewLimit.ToString("F" + m_intDecimal);

                        m_dgdLeadSettingView.Rows[i].Cells[3].Style.BackColor = Color.White;
                        //if (m_dgdLeadSettingView.Rows[i].HeaderCell.Value.ToString().IndexOf("Group") >= 0)
                        //{
                        //    intGroupNoIndex = i;
                        //    continue;
                        //}

                        int intLeadNoIndex = Convert.ToInt32(m_dgdLeadSettingView.Rows[i].HeaderCell.Value.ToString().Substring(4)) - 1;  // substring start with 4 because "Lead" length is 4

                        List<string> arrResultList = m_smVisionInfo.g_arrLead3D[0].GetBlobFeaturesResult_WithPassFailIndicator(intLeadNoIndex);
                        m_dgdLeadSettingView.Rows[i].Cells[4].Value = arrResultList[1];
                        int intFailMask = Convert.ToInt32(arrResultList[arrResultList.Count - 1]);

                        if ((intFailMask & 0x100) > 0)
                        {
                            m_dgdLeadSettingView.Rows[i].Cells[4].Style.BackColor = Color.Red;
                            m_dgdLeadSettingView.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                            m_dgdLeadSettingView.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                            m_dgdLeadSettingView.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            m_dgdLeadSettingView.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                            m_dgdLeadSettingView.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                            m_dgdLeadSettingView.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                            m_dgdLeadSettingView.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                        }
                    }
                }

                m_blnInitDone = true;

            }
        }

        private void btn_OffsetSetting_Click(object sender, EventArgs e)
        {
            Lead3DOffsetSettingForm objForm = new Lead3DOffsetSettingForm(m_smCustomizeInfo, m_smVisionInfo,
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