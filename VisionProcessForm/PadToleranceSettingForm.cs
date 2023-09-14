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
    public partial class PadToleranceSettingForm : Form
    {
        #region Member Variables
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnFormOpen = false;
        private bool m_blnInitDone = false;
        private bool m_blnUpdateInfo = false;
        private int m_intUserGroup = 5;
        private int m_intPadIndex = 0;
        private int m_intGDSelectedIndex = 0;   // Golden Data Set Selected index
        private int m_intDecimalPlaces = 4;
        private int m_intDecimal = 3;
        private int m_intDecimal2 = 6;
        private string m_strSelectedRecipe;
        private string m_strUnitLabel = "mm";
        private bool m_blnChangeScoreSetting = true;
        private List<int> m_arrPadRowIndex = new List<int>();
        private List<int> m_arrGroupRowIndex = new List<int>();
        
        private DataGridView[] m_dgdView = new DataGridView[5];
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private GoldenUnitCompensationForm objGoldenUnitForm;
        #endregion


        #region Properties

        public bool ref_blnFormOpen { get { return m_blnFormOpen; } set { m_blnFormOpen = value; } }

        #endregion

        public PadToleranceSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;
            
            m_dgdView[0] = dgd_MiddlePad;

            // 04-07-2019 ZJYEOH : Only dgd_MiddlePad will be used in this form
            //m_dgdView[1] = dgd_TopPad;
            //m_dgdView[2] = dgd_RightPad;
            //m_dgdView[3] = dgd_BottomPad;
            //m_dgdView[4] = dgd_LeftPad;

            LoadGoldenData();
            DisableField2();
            UpdateGUI();
            m_smVisionInfo.g_intSelectedPadROIIndex = 0;
            m_blnInitDone = true;

            // 2020-01-06 ZJYEOH : Trigger Offline Test one time 
            m_smVisionInfo.g_intViewInspectionSetting = 1;
            m_smVisionInfo.AT_VM_OfflineTestAllPad = true;
            TriggerOfflineTest();
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
        private void DisableField()
        {
            string strChild1 = "Test Page";
            string strChild2 = "";


            //strChild2 = "Tolerance Setting Page";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    m_blnChangeScoreSetting = false;
            //}

            strChild1 = "Tolerance Page";
            strChild2 = "Pad Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                m_blnChangeScoreSetting = false;
                txt_ExtraPadMinArea.Enabled = false;
                txt_ExtraPadMinLength.Enabled = false;
                txt_TotalExtraPadMinArea.Enabled = false;
                btn_GoldenUnitSetting.Enabled = false;
                btn_LoadTolFromFile.Enabled = false;
                btn_SaveAccuracyReport.Enabled = false;
                btn_SaveTolToFile.Enabled = false;
                btn_SelectUseToleranceType.Enabled = false;
                btn_UpdateTolerance.Enabled = false;
            }
            strChild1 = "Tolerance Page";
            strChild2 = "Pad Offset Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_OffsetSetting.Enabled = false;
            }
            strChild1 = "Tolerance Page";
            strChild2 = "Save Tolerance Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            //NewUserRight objUserRight = new NewUserRight(false);
            string strChild1 = "Tol.Pad";
            string strChild2 = "";
            if (m_smVisionInfo.g_strVisionName == "BottomOrientPad" || m_smVisionInfo.g_strVisionName == "BottomOPadPkg")
            {
                strChild2 = "Pad TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    m_blnChangeScoreSetting = false;
                    txt_ExtraPadMinArea.Enabled = false;
                    txt_ExtraPadMinLength.Enabled = false;
                    txt_TotalExtraPadMinArea.Enabled = false;
                    btn_GoldenUnitSetting.Enabled = false;
                    btn_LoadTolFromFile.Enabled = false;
                    btn_SaveAccuracyReport.Enabled = false;
                    btn_SaveTolToFile.Enabled = false;
                    btn_SelectUseToleranceType.Enabled = false;
                    btn_UpdateTolerance.Enabled = false;
                    dgd_WholePadSetting.Enabled = false;
                }

                strChild2 = "Pad Offset Setting";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    btn_OffsetSetting.Enabled = false;
                }

                strChild2 = "Save Button";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    btn_Save.Enabled = false;
                }
            }
            else
            {
                strChild2 = "Pad TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    m_blnChangeScoreSetting = false;
                    txt_ExtraPadMinArea.Enabled = false;
                    txt_ExtraPadMinLength.Enabled = false;
                    txt_TotalExtraPadMinArea.Enabled = false;
                    btn_GoldenUnitSetting.Enabled = false;
                    btn_LoadTolFromFile.Enabled = false;
                    btn_SaveAccuracyReport.Enabled = false;
                    btn_SaveTolToFile.Enabled = false;
                    btn_SelectUseToleranceType.Enabled = false;
                    btn_UpdateTolerance.Enabled = false;
                    dgd_WholePadSetting.Enabled = false;
                }

                strChild2 = "Pad Offset Setting";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    btn_OffsetSetting.Enabled = false;
                }

                strChild2 = "Save Button";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    btn_Save.Enabled = false;
                }
            }
        }
        private void UpdateGUI()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_DisplayGroupNo.Checked = Convert.ToBoolean(subkey1.GetValue("CurrentDisplayGroupNo", false));

            radioBtn_Middle.Checked = true;

            ReadPadAndGroupTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

            UpdateScore(m_intPadIndex, m_dgdView[0]);
            m_smVisionInfo.PR_TL_UpdateInfo = false;

            m_dgdView[0].Columns.Remove(column_GolWidth);
            m_dgdView[0].Columns.Remove(column_GolLength);
            m_dgdView[0].Columns.Remove(column_GolPitch);
            m_dgdView[0].Columns.Remove(column_GolGap);
            if (m_intUserGroup != 1)    // for SRM only
            {
                btn_GoldenUnitSetting.Visible = false;
            }

            if (!m_smVisionInfo.g_blnCheck4Sides)
            {
                radioBtn_Down.Enabled = false;
                radioBtn_Left.Enabled = false;
                radioBtn_Right.Enabled = false;
                radioBtn_Up.Enabled = false;
            }

            if (m_smVisionInfo.g_arrPad.Length == 1 || !m_smVisionInfo.g_blnCheck4Sides)
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

            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            UpdateUnitDisplay();

            //txt_ExtraPadMinLength.Text = m_smVisionInfo.g_arrPad[0].GetExtraPadLengthLimit(1).ToString("F" + m_intDecimal);
            //txt_ExtraPadMinArea.Text = m_smVisionInfo.g_arrPad[0].GetExtraPadMinArea(1).ToString("F" + m_intDecimal2);
            //txt_TotalExtraPadMinArea.Text = m_smVisionInfo.g_arrPad[0].GetTotalExtraPadMinArea(1).ToString("F" + m_intDecimal2);
            PreUpdateWholePadSettingTable();
            UpdateWholdPadsSettingGUI();
            UpdateWholePadScore();

            //RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            //RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_DisplayResult.Checked = Convert.ToBoolean(subkey1.GetValue("CurrentDisplayResult_PadTolerance", false));
            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            //if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
            //{
            //    ContaminationControlVisible(false);
            //}
            //else
            //{
            //    ContaminationControlVisible(true);
            //}

            // Set this form size according to the max number of rows.
            int intMaxRow = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (intMaxRow < m_smVisionInfo.g_arrPad[i].GetBlobsFeaturesNumber())
                    intMaxRow = m_smVisionInfo.g_arrPad[i].GetBlobsFeaturesNumber();
            }
            if (intMaxRow > 21)
                intMaxRow = 21; // 20-06-2019 ZJYEOH : To avoid form size larger than screen size 
            this.Size = new Size(this.Size.Width, 260 + 24 * intMaxRow);
            tabControl_Pad5S.Size = new Size(this.Size.Width, 204 + 24 * intMaxRow);
            dgd_MiddlePad.Size = new Size(this.Size.Width, 79 + 24 * intMaxRow);
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

            lbl_UnitSqDisplay1.Text = strUnitDisplay + "^2";
            lbl_UnitDisplay1.Text = strUnitDisplay;

            //txt_TotalExtraPadMinArea.DecimalPlaces = m_intDecimal2;
            //txt_ExtraPadMinArea.DecimalPlaces = m_intDecimal2;
            //txt_ExtraPadMinLength.DecimalPlaces = m_intDecimal;
        }

        private void ViewOrHideResultColumn(bool blnWantView)
        {
            int intFailOptionMask = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intFailOptionMask;

            if ((intFailOptionMask & 0x100) > 0)
            {
                m_dgdView[0].Columns[0].Visible = true;
                m_dgdView[0].Columns[1].Visible = blnWantView;
                m_dgdView[0].Columns[2].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[0].Visible = false;
                m_dgdView[0].Columns[1].Visible = false;
                m_dgdView[0].Columns[2].Visible = false;
            }

            if ((intFailOptionMask & 0x20) > 0)
            {
                m_dgdView[0].Columns[3].Visible = true;
                m_dgdView[0].Columns[4].Visible = blnWantView;
                m_dgdView[0].Columns[5].Visible = true;
                m_dgdView[0].Columns[6].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[3].Visible = false;
                m_dgdView[0].Columns[4].Visible = false;
                m_dgdView[0].Columns[5].Visible = false;
                m_dgdView[0].Columns[6].Visible = false;
            }

            if ((intFailOptionMask & 0xC0) > 0)
            {
                m_dgdView[0].Columns[7].Visible = true;
                m_dgdView[0].Columns[8].Visible = blnWantView;
                m_dgdView[0].Columns[9].Visible = true;
                m_dgdView[0].Columns[10].Visible = true;
                m_dgdView[0].Columns[11].Visible = true;
                m_dgdView[0].Columns[12].Visible = blnWantView;
                m_dgdView[0].Columns[13].Visible = true;
                m_dgdView[0].Columns[14].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[7].Visible = false;
                m_dgdView[0].Columns[8].Visible = false;
                m_dgdView[0].Columns[9].Visible = false;
                m_dgdView[0].Columns[10].Visible = false;
                m_dgdView[0].Columns[11].Visible = false;
                m_dgdView[0].Columns[12].Visible = false;
                m_dgdView[0].Columns[13].Visible = false;
                m_dgdView[0].Columns[14].Visible = false;
            }

            if ((intFailOptionMask & 0x600) > 0)
            {
                m_dgdView[0].Columns[15].Visible = true;
                m_dgdView[0].Columns[16].Visible = blnWantView;
                m_dgdView[0].Columns[17].Visible = true;
                m_dgdView[0].Columns[18].Visible = true;
                m_dgdView[0].Columns[19].Visible = true;
                m_dgdView[0].Columns[20].Visible = blnWantView;
                m_dgdView[0].Columns[21].Visible = true;
                m_dgdView[0].Columns[22].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[15].Visible = false;
                m_dgdView[0].Columns[16].Visible = false;
                m_dgdView[0].Columns[17].Visible = false;
                m_dgdView[0].Columns[18].Visible = false;
                m_dgdView[0].Columns[19].Visible = false;
                m_dgdView[0].Columns[20].Visible = false;
                m_dgdView[0].Columns[21].Visible = false;
                m_dgdView[0].Columns[22].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckBrokenPadArea)
            {
                m_dgdView[0].Columns[23].Visible = true;
                m_dgdView[0].Columns[24].Visible = blnWantView;
                m_dgdView[0].Columns[25].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[23].Visible = false;
                m_dgdView[0].Columns[24].Visible = false;
                m_dgdView[0].Columns[25].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckBrokenPadLength)
            {
                m_dgdView[0].Columns[26].Visible = true;
                m_dgdView[0].Columns[27].Visible = blnWantView;
                m_dgdView[0].Columns[28].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[26].Visible = false;
                m_dgdView[0].Columns[27].Visible = false;
                m_dgdView[0].Columns[28].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckExcessPadArea)
            {
                m_dgdView[0].Columns[29].Visible = true;
                m_dgdView[0].Columns[30].Visible = blnWantView;
                m_dgdView[0].Columns[31].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[29].Visible = false;
                m_dgdView[0].Columns[30].Visible = false;
                m_dgdView[0].Columns[31].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckSmearPadLength)
            {
                m_dgdView[0].Columns[32].Visible = true;
                m_dgdView[0].Columns[33].Visible = blnWantView;
                m_dgdView[0].Columns[34].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[32].Visible = false;
                m_dgdView[0].Columns[33].Visible = false;
                m_dgdView[0].Columns[34].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantEdgeLimit_Pad && ((intFailOptionMask & 0x4000) > 0) && m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeLimit)
            {
                m_dgdView[0].Columns[35].Visible = true;
                m_dgdView[0].Columns[36].Visible = blnWantView;    // Edge result  // DD
                m_dgdView[0].Columns[37].Visible = true;
                m_dgdView[0].Columns[38].Visible = blnWantView;
                m_dgdView[0].Columns[39].Visible = true;
                m_dgdView[0].Columns[40].Visible = blnWantView;
                m_dgdView[0].Columns[41].Visible = true;
                m_dgdView[0].Columns[42].Visible = blnWantView;
                m_dgdView[0].Columns[43].Visible = true;
            }
            else
            {
                //Hide all Edge Limit Column
                m_dgdView[0].Columns[35].Visible = false;
                m_dgdView[0].Columns[36].Visible = false;
                m_dgdView[0].Columns[37].Visible = false;
                m_dgdView[0].Columns[38].Visible = false;
                m_dgdView[0].Columns[39].Visible = false;
                m_dgdView[0].Columns[40].Visible = false;
                m_dgdView[0].Columns[41].Visible = false;
                m_dgdView[0].Columns[42].Visible = false;
                m_dgdView[0].Columns[43].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantStandOff_Pad && ((intFailOptionMask & 0x8000) > 0))
            {
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(0))
                {
                    m_dgdView[0].Columns[44].Visible = true;
                    m_dgdView[0].Columns[45].Visible = blnWantView;
                }
                else
                {
                    m_dgdView[0].Columns[44].Visible = false;
                    m_dgdView[0].Columns[45].Visible = false;
                }
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(1))
                {
                    m_dgdView[0].Columns[46].Visible = true;
                    m_dgdView[0].Columns[47].Visible = blnWantView;
                }
                else
                {
                    m_dgdView[0].Columns[46].Visible = false;
                    m_dgdView[0].Columns[47].Visible = false;
                }
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(2))
                {
                    m_dgdView[0].Columns[48].Visible = true;
                    m_dgdView[0].Columns[49].Visible = blnWantView;
                }
                else
                {
                    m_dgdView[0].Columns[48].Visible = false;
                    m_dgdView[0].Columns[49].Visible = false;
                }
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(3))
                {
                    m_dgdView[0].Columns[50].Visible = true;
                    m_dgdView[0].Columns[51].Visible = blnWantView;
                }
                else
                {
                    m_dgdView[0].Columns[50].Visible = false;
                    m_dgdView[0].Columns[51].Visible = false;
                }
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(0) && m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(1) && m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(2) && m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(3))
                    m_dgdView[0].Columns[52].Visible = true;
                else
                    m_dgdView[0].Columns[52].Visible = false;
            }
            else
            {
                //Hide all Stand Off Column
                m_dgdView[0].Columns[44].Visible = false;
                m_dgdView[0].Columns[45].Visible = false;
                m_dgdView[0].Columns[46].Visible = false;
                m_dgdView[0].Columns[47].Visible = false;
                m_dgdView[0].Columns[48].Visible = false;
                m_dgdView[0].Columns[49].Visible = false;
                m_dgdView[0].Columns[50].Visible = false;
                m_dgdView[0].Columns[51].Visible = false;
                m_dgdView[0].Columns[52].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 2))
            {
                m_dgdView[0].Columns[53].Visible = true;
                m_dgdView[0].Columns[54].Visible = blnWantView;
                m_dgdView[0].Columns[55].Visible = true;
                m_dgdView[0].Columns[56].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[53].Visible = false;
                m_dgdView[0].Columns[54].Visible = false;
                m_dgdView[0].Columns[55].Visible = false;
                m_dgdView[0].Columns[56].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 3))
            {
                m_dgdView[0].Columns[57].Visible = true;
                m_dgdView[0].Columns[58].Visible = blnWantView;
                m_dgdView[0].Columns[59].Visible = true;
                m_dgdView[0].Columns[60].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[57].Visible = false;
                m_dgdView[0].Columns[58].Visible = false;
                m_dgdView[0].Columns[59].Visible = false;
                m_dgdView[0].Columns[60].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 4))
            {
                m_dgdView[0].Columns[61].Visible = true;
                m_dgdView[0].Columns[62].Visible = blnWantView;
                m_dgdView[0].Columns[63].Visible = true;
                m_dgdView[0].Columns[64].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[61].Visible = false;
                m_dgdView[0].Columns[62].Visible = false;
                m_dgdView[0].Columns[63].Visible = false;
                m_dgdView[0].Columns[64].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 5))
            {
                m_dgdView[0].Columns[65].Visible = true;
                m_dgdView[0].Columns[66].Visible = blnWantView;
                m_dgdView[0].Columns[67].Visible = true;
                m_dgdView[0].Columns[68].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[65].Visible = false;
                m_dgdView[0].Columns[66].Visible = false;
                m_dgdView[0].Columns[67].Visible = false;
                m_dgdView[0].Columns[68].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 6))
            {
                m_dgdView[0].Columns[69].Visible = true;
                m_dgdView[0].Columns[70].Visible = blnWantView;
                m_dgdView[0].Columns[71].Visible = true;
                m_dgdView[0].Columns[72].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[69].Visible = false;
                m_dgdView[0].Columns[70].Visible = false;
                m_dgdView[0].Columns[71].Visible = false;
                m_dgdView[0].Columns[72].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 7))
            {
                m_dgdView[0].Columns[73].Visible = true;
                m_dgdView[0].Columns[74].Visible = blnWantView;
                m_dgdView[0].Columns[75].Visible = true;
                m_dgdView[0].Columns[76].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[73].Visible = false;
                m_dgdView[0].Columns[74].Visible = false;
                m_dgdView[0].Columns[75].Visible = false;
                m_dgdView[0].Columns[76].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 8))
            {
                m_dgdView[0].Columns[77].Visible = true;
                m_dgdView[0].Columns[78].Visible = blnWantView;
                m_dgdView[0].Columns[79].Visible = true;
                m_dgdView[0].Columns[80].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[77].Visible = false;
                m_dgdView[0].Columns[78].Visible = false;
                m_dgdView[0].Columns[79].Visible = false;
                m_dgdView[0].Columns[80].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 9))
            {
                m_dgdView[0].Columns[81].Visible = true;
                m_dgdView[0].Columns[82].Visible = blnWantView;
                m_dgdView[0].Columns[83].Visible = true;
                m_dgdView[0].Columns[84].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[81].Visible = false;
                m_dgdView[0].Columns[82].Visible = false;
                m_dgdView[0].Columns[83].Visible = false;
                m_dgdView[0].Columns[84].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 10))
            {
                m_dgdView[0].Columns[85].Visible = true;
                m_dgdView[0].Columns[86].Visible = blnWantView;
                m_dgdView[0].Columns[87].Visible = true;
                m_dgdView[0].Columns[88].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[85].Visible = false;
                m_dgdView[0].Columns[86].Visible = false;
                m_dgdView[0].Columns[87].Visible = false;
                m_dgdView[0].Columns[88].Visible = false;
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intMaxExtraLineCount > 11))
            {
                m_dgdView[0].Columns[89].Visible = true;
                m_dgdView[0].Columns[90].Visible = blnWantView;
                m_dgdView[0].Columns[91].Visible = true;
                //m_dgdView[0].Columns[92].Visible = true;
            }
            else
            {
                m_dgdView[0].Columns[89].Visible = false;
                m_dgdView[0].Columns[90].Visible = false;
                m_dgdView[0].Columns[91].Visible = false;
                //m_dgdView[0].Columns[92].Visible = false;
            }
        }

        private void LoadGoldenData()
        {
            // Load Golden Data
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Pad\\GoldenData.xml";

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterGoldenData";
                else if (i == 1)
                    strSectionName = "TopGoldenData";
                else if (i == 2)
                    strSectionName = "RightGoldenData";
                else if (i == 3)
                    strSectionName = "BottomGoldenData";
                else if (i == 4)
                    strSectionName = "LeftGoldenData";

                m_smVisionInfo.g_arrPad[i].LoadPadGoldenData(strPath, strSectionName);
            }
        }

        private void UpdateGoldenDataIntoGridTable(int intPadIndex, DataGridView dgd_PadSetting)
        {
            if (objGoldenUnitForm == null)
                return;

            if (!objGoldenUnitForm.ref_intViewGoldenDataColumn)
                return;

            if (m_intGDSelectedIndex >= m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData.Count)
                return;

            for (int r = 0; r < dgd_PadSetting.Rows.Count; r++)
            {
                if (r < m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Count)
                {
                    if (dgd_PadSetting.Rows[r].Cells.Count <= 92)
                        continue;

                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 0)
                        dgd_PadSetting.Rows[r].Cells[92].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][0];
                    else
                        dgd_PadSetting.Rows[r].Cells[92].Value = 0;

                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 1)
                        dgd_PadSetting.Rows[r].Cells[93].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][1];
                    else
                        dgd_PadSetting.Rows[r].Cells[93].Value = 0;

                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 2)
                        dgd_PadSetting.Rows[r].Cells[94].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][2];
                    else
                        dgd_PadSetting.Rows[r].Cells[94].Value = 0;

                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 3)
                        dgd_PadSetting.Rows[r].Cells[95].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][3];
                    else
                        dgd_PadSetting.Rows[r].Cells[95].Value = 0;
                }
                else
                {
                    dgd_PadSetting.Rows[r].Cells[92].Value = 0;
                    dgd_PadSetting.Rows[r].Cells[93].Value = 0;
                    dgd_PadSetting.Rows[r].Cells[94].Value = 0;
                    dgd_PadSetting.Rows[r].Cells[95].Value = 0;
                }
            }
        }

        private void UpdateGridTableIntoGoldenData(int intPadIndex, DataGridView dgd_PadSetting)
        {
            m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Clear();

            for (int r = 0; r < dgd_PadSetting.Rows.Count; r++)
            {
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Add(new List<float>());
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[92].Value));
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[93].Value));
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[94].Value));
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[95].Value));

            }
        }

        private void CheckSaveGoldenData(int intPadIndex, DataGridView dgd_PadSetting)
        {
            if (objGoldenUnitForm == null)
                return;

            if (!objGoldenUnitForm.ref_intViewGoldenDataColumn)
                return;

            bool blnIsDataChanged = false;
            if (m_intGDSelectedIndex >= 0 && m_intGDSelectedIndex < m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData.Count)
            {
                if (dgd_PadSetting.Rows.Count != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Count)
                {
                    blnIsDataChanged = true;
                }
                else
                {

                    for (int r = 0; r < dgd_PadSetting.Rows.Count; r++)
                    {
                        if (Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[92].Value) != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][0])
                        {
                            blnIsDataChanged = true;
                            break;
                        }

                        if (Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[93].Value) != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][1])
                        {
                            blnIsDataChanged = true;
                            break;
                        }

                        if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 2)
                        {
                            if (Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[94].Value) != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][2])
                            {
                                blnIsDataChanged = true;
                                break;
                            }
                        }

                        if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 3)
                        {
                            if (Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[95].Value) != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][3])
                            {
                                blnIsDataChanged = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (blnIsDataChanged)
            {
                if (SRMMessageBox.Show("Do you want to save the golden data changes you have made?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    UpdateGridTableIntoGoldenData(intPadIndex, dgd_PadSetting);

                    SaveGoldenData();
                }
                else
                {
                    UpdateGoldenDataIntoGridTable(intPadIndex, dgd_PadSetting);
                }

            }
        }

        private void SaveGoldenData()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                      m_smVisionInfo.g_strVisionFolderName + "\\Pad\\GoldenData.xml";

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterGoldenData";
                else if (i == 1)
                    strSectionName = "TopGoldenData";
                else if (i == 2)
                    strSectionName = "RightGoldenData";
                else if (i == 3)
                    strSectionName = "BottomGoldenData";
                else if (i == 4)
                    strSectionName = "LeftGoldenData";

                
                STDeviceEdit.CopySettingFile(strPath, "");
                m_smVisionInfo.g_arrPad[i].SavePadGoldenData(strPath, false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad Tolerance Setting", m_smProductionInfo.g_strLotID);
                
            }
        }

        private void UpdateScore(int intPadIndex, DataGridView dgd_PadSetting)
        {

            TrackLog objTL = new TrackLog();
            objTL.WriteLine("                 ");

            int intPadNoIndex = 0;
            int intGroupNoIndex = -1;
            for (int i = 0; i < dgd_PadSetting.Rows.Count; i++)
            {
                if (dgd_PadSetting.Rows[i].HeaderCell.Value.ToString().IndexOf("Group") >= 0)
                {
                    intGroupNoIndex = i;
                    continue;
                }

                string strBlobsFeatures = "---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#---#";
                intPadNoIndex = Convert.ToInt32(dgd_PadSetting.Rows[i].HeaderCell.Value.ToString().Substring(dgd_PadSetting.Rows[i].HeaderCell.Value.ToString().IndexOf("Pad") + 3)) - 1;//Convert.ToInt32(dgd_PadSetting.Rows[i].HeaderCell.Value.ToString().Substring(3)) - 1

                //if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailResultMask & 0x1000) == 0    
                strBlobsFeatures = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobFeaturesResult(intPadNoIndex);

                string[] strFeature = strBlobsFeatures.Split('#');


                //objTL.WriteLine(strBlobsFeatures);



                int intFeatureIndex = 0;

                #region Update value to grid
                intFeatureIndex++;

                int[] intGridResultColumnIndex = { 1, 4, 8, 12, 16, 20, 24, 27, 30 ,33, 36, 38, 40, 42, 45, 47, 49, 51, 54, 58, 62, 66, 70, 74, 78, 82, 86, 90 };
                for (int u = 0; u < intGridResultColumnIndex.Length; u++)
                {
                    dgd_PadSetting.Rows[i].Cells[intGridResultColumnIndex[u]].Value = strFeature[intFeatureIndex++];
                }

                #endregion

                #region Update grid font color

                float fAccuracySpec = 0.0125f;

                float fMinValue, fResultValue, fMaxValue;
                // OffSet
                if (dgd_PadSetting.Rows[i].Cells[1].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[1].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[0].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[0].Value.ToString());
                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantUseBorderLimitAsOffset)
                    {
                        if (fResultValue < fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        }

                    }
                    else
                    {
                        if (fResultValue > fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[1].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                }

                // Area
                if (dgd_PadSetting.Rows[i].Cells[4].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[4].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[3].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[5].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[3].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[5].Value.ToString());
                    }
                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[4].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[4].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[4].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Width
                if (dgd_PadSetting.Rows[i].Cells[8].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[8].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[7].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[9].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[7].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[9].Value.ToString());
                    }
                    //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    //    dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Red;
                    //else
                    {
                        if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_PadSetting.Rows[i].Cells.Count < 72)
                        {
                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                            {
                                dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Yellow;
                                dgd_PadSetting.Rows[i].Cells[8].Style.SelectionForeColor = Color.Yellow;
                                dgd_PadSetting.Rows[i].Cells[8].Style.BackColor = Color.Red;
                                dgd_PadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Black;
                                dgd_PadSetting.Rows[i].Cells[8].Style.SelectionForeColor = Color.Black;
                                dgd_PadSetting.Rows[i].Cells[8].Style.BackColor = Color.Lime;
                                dgd_PadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.Lime;
                            }
                        }
                        else
                        {
                            if (dgd_PadSetting.Rows[i].Cells.Count > 72)
                            {
                                if (dgd_PadSetting.Rows[i].Cells[72].Value != null)
                                {
                                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[72].Value.ToString()) - fAccuracySpec;
                                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[8].Value.ToString());
                                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[72].Value.ToString()) + fAccuracySpec;
                                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                        dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.DarkOrange;
                                    else
                                        dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Black;
                                }
                            }
                        }
                    }
                }

                // Length
                if (dgd_PadSetting.Rows[i].Cells[12].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[12].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[11].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[13].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[11].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[13].Value.ToString());
                    }
                    //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    //    dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Red;
                    //else
                    {
                        if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_PadSetting.Rows[i].Cells.Count < 72)
                        {
                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                            {
                                dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Yellow;
                                dgd_PadSetting.Rows[i].Cells[12].Style.SelectionForeColor = Color.Yellow;
                                dgd_PadSetting.Rows[i].Cells[12].Style.BackColor = Color.Red;
                                dgd_PadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Black;
                                dgd_PadSetting.Rows[i].Cells[12].Style.SelectionForeColor = Color.Black;
                                dgd_PadSetting.Rows[i].Cells[12].Style.BackColor = Color.Lime;
                                dgd_PadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.Lime;
                            }
                        }
                        else
                        {
                            if (dgd_PadSetting.Rows[i].Cells.Count > 73)
                            {
                                if (dgd_PadSetting.Rows[i].Cells[73].Value != null)
                                {
                                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[73].Value.ToString()) - fAccuracySpec;
                                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[12].Value.ToString());
                                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[73].Value.ToString()) + fAccuracySpec;
                                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                        dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.DarkOrange;
                                    else
                                        dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Black;
                                }
                            }
                        }
                    }

                }

                // Pitch
                if (!((dgd_PadSetting.Rows[i].Cells[16].Value.ToString() == "---") ||
                    (dgd_PadSetting.Rows[i].Cells[15].Value.ToString() == "---") ||
                    (dgd_PadSetting.Rows[i].Cells[17].Value.ToString() == "---")))
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[16].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[15].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[17].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[15].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[17].Value.ToString());
                    }
                    //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    //    dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Red;
                    //else
                    {
                        if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_PadSetting.Rows[i].Cells.Count < 72)
                        {
                            if (((m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask & 0x600) > 0) && ((fResultValue < fMinValue) || (fResultValue > fMaxValue)))
                            {
                                dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Yellow;
                                dgd_PadSetting.Rows[i].Cells[16].Style.SelectionForeColor = Color.Yellow;
                                dgd_PadSetting.Rows[i].Cells[16].Style.BackColor = Color.Red;
                                dgd_PadSetting.Rows[i].Cells[16].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Black;
                                dgd_PadSetting.Rows[i].Cells[16].Style.SelectionForeColor = Color.Black;
                                dgd_PadSetting.Rows[i].Cells[16].Style.BackColor = Color.Lime;
                                dgd_PadSetting.Rows[i].Cells[16].Style.SelectionBackColor = Color.Lime;
                            }
                        }
                        else
                        {
                            if (dgd_PadSetting.Rows[i].Cells.Count > 74 && (dgd_PadSetting.Rows[i].Cells[74].Value != null))
                            {
                                fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[74].Value.ToString()) - fAccuracySpec;
                                fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[16].Value.ToString());
                                fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[74].Value.ToString()) + fAccuracySpec;
                                if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                    dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.DarkOrange;
                                else
                                    dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Black;
                            }
                        }
                    }
                }

                // Gap
                if (!((dgd_PadSetting.Rows[i].Cells[20].Value.ToString() == "---") ||
                    (dgd_PadSetting.Rows[i].Cells[19].Value.ToString() == "---") ||
                    (dgd_PadSetting.Rows[i].Cells[21].Value.ToString() == "---")))
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[20].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[19].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[21].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[19].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[21].Value.ToString());
                    }
                    //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    //    dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Red;
                    //else
                    {
                        if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_PadSetting.Rows[i].Cells.Count < 72)
                        {
                            if (((m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask & 0x600) > 0) && ((fResultValue < fMinValue) || (fResultValue > fMaxValue)))
                            {
                                dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Yellow;
                                dgd_PadSetting.Rows[i].Cells[20].Style.SelectionForeColor = Color.Yellow;
                                dgd_PadSetting.Rows[i].Cells[20].Style.BackColor = Color.Red;
                                dgd_PadSetting.Rows[i].Cells[20].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Black;
                                dgd_PadSetting.Rows[i].Cells[20].Style.SelectionForeColor = Color.Black;
                                dgd_PadSetting.Rows[i].Cells[20].Style.BackColor = Color.Lime;
                                dgd_PadSetting.Rows[i].Cells[20].Style.SelectionBackColor = Color.Lime;
                            }
                        }
                        else if (dgd_PadSetting.Rows[i].Cells.Count > 75 && (dgd_PadSetting.Rows[i].Cells[75].Value != null))
                        {
                            fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[75].Value.ToString()) - fAccuracySpec;
                            fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[20].Value.ToString());
                            fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[75].Value.ToString()) + fAccuracySpec;
                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.DarkOrange;
                            else
                                dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Black;
                        }

                    }
                }

                // Broken Area
                if (dgd_PadSetting.Rows[i].Cells[24].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[24].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[23].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[23].Value.ToString());

                    if (fResultValue > fMaxValue)
                    {
                        dgd_PadSetting.Rows[i].Cells[24].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[24].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[24].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[24].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[24].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[24].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[24].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[24].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Broken Length
                if (dgd_PadSetting.Rows[i].Cells[27].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[27].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[26].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[26].Value.ToString());
                    if (fResultValue > fMaxValue)
                    {
                        dgd_PadSetting.Rows[i].Cells[27].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[27].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[27].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[27].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[27].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[27].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[27].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[27].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Excess Area
                if (dgd_PadSetting.Rows[i].Cells[30].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[30].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[29].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[29].Value.ToString());

                    if (fResultValue > fMaxValue)
                    {
                        dgd_PadSetting.Rows[i].Cells[30].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[30].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[30].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[30].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[30].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[30].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[30].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[30].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Smear Length
                if (dgd_PadSetting.Rows[i].Cells[33].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[33].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[32].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[32].Value.ToString());

                    if (fResultValue > fMaxValue)
                    {
                        dgd_PadSetting.Rows[i].Cells[33].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[33].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[33].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[33].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[33].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[33].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[33].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[33].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Edge Limit Top
                if (dgd_PadSetting.Rows[i].Cells[36].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[36].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[35].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[35].Value.ToString());

                    if (fResultValue < fMaxValue)
                    {
                        dgd_PadSetting.Rows[i].Cells[36].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[36].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[36].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[36].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[36].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[36].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[36].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[36].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Edge Limit Right
                if (dgd_PadSetting.Rows[i].Cells[38].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[38].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[37].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[37].Value.ToString());

                    if (fResultValue < fMaxValue)
                    {
                        dgd_PadSetting.Rows[i].Cells[38].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[38].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[38].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[38].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[38].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[38].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[38].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[38].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Edge Limit Bottom
                if (dgd_PadSetting.Rows[i].Cells[40].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[40].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[39].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[39].Value.ToString());

                    if (fResultValue < fMaxValue)
                    {
                        dgd_PadSetting.Rows[i].Cells[40].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[40].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[40].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[40].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[40].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[40].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[40].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[40].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Edge Limit Left
                if (dgd_PadSetting.Rows[i].Cells[42].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[42].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[41].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[41].Value.ToString());

                    if (fResultValue < fMaxValue)
                    {
                        dgd_PadSetting.Rows[i].Cells[42].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[42].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[42].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[42].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[42].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[42].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[42].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[42].Style.SelectionBackColor = Color.Lime;
                    }
                }

                // Stand Off Top
                if (dgd_PadSetting.Rows[i].Cells[45].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[45].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[44].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[44].Value.ToString());
                    if (m_smVisionInfo.g_arrPad[intPadIndex].GetReferDirection(i, true) == 0)
                    {
                        if (fResultValue < fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[45].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[45].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[45].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[45].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[45].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[45].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[45].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[45].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                    else
                    {
                        if (fResultValue > fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[45].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[45].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[45].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[45].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[45].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[45].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[45].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[45].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                }

                // Stand Off Bottom
                if (dgd_PadSetting.Rows[i].Cells[47].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[47].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[46].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[46].Value.ToString());

                    if (m_smVisionInfo.g_arrPad[intPadIndex].GetReferDirection(i, true) == 0)
                    {
                        if (fResultValue > fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[47].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[47].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[47].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[47].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[47].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[47].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[47].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[47].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                    else
                    {
                        if (fResultValue < fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[47].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[47].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[47].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[47].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[47].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[47].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[47].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[47].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                }

                // Stand Off Left
                if (dgd_PadSetting.Rows[i].Cells[49].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[49].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[48].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[48].Value.ToString());

                    if (m_smVisionInfo.g_arrPad[intPadIndex].GetReferDirection(i, false) == 0)
                    {
                        if (fResultValue < fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[49].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[49].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[49].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[49].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[49].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[49].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[49].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[49].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                    else
                    {
                        if (fResultValue > fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[49].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[49].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[49].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[49].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[49].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[49].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[49].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[49].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                }

                // Stand Off Right
                if (dgd_PadSetting.Rows[i].Cells[51].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[51].Value.ToString());
                    if (intGroupNoIndex >= 0)
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[50].Value.ToString());
                    else
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[50].Value.ToString());

                    if (m_smVisionInfo.g_arrPad[intPadIndex].GetReferDirection(i, false) == 0)
                    {
                        if (fResultValue > fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[51].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[51].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[51].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[51].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[51].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[51].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[51].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[51].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                    else
                    {
                        if (fResultValue < fMaxValue)
                        {
                            dgd_PadSetting.Rows[i].Cells[51].Style.ForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[51].Style.SelectionForeColor = Color.Yellow;
                            dgd_PadSetting.Rows[i].Cells[51].Style.BackColor = Color.Red;
                            dgd_PadSetting.Rows[i].Cells[51].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PadSetting.Rows[i].Cells[51].Style.ForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[51].Style.SelectionForeColor = Color.Black;
                            dgd_PadSetting.Rows[i].Cells[51].Style.BackColor = Color.Lime;
                            dgd_PadSetting.Rows[i].Cells[51].Style.SelectionBackColor = Color.Lime;
                        }
                    }
                }

                // Dimension 1
                if (dgd_PadSetting.Rows[i].Cells[54].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[54].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[53].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[55].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[53].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[55].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[54].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[54].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[54].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[54].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[54].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[54].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[54].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[54].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x10000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[54].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[54].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[54].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[54].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 2
                if (dgd_PadSetting.Rows[i].Cells[58].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[58].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[57].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[59].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[57].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[59].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[58].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[58].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[58].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[58].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[58].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[58].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[58].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[58].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x20000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[58].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[58].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[58].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[58].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 3
                if (dgd_PadSetting.Rows[i].Cells[62].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[62].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[61].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[63].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[61].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[63].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[62].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[62].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[62].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[62].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[62].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[62].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[62].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[62].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x40000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[62].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[62].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[62].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[62].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 4
                if (dgd_PadSetting.Rows[i].Cells[66].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[66].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[65].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[67].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[65].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[67].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[66].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[66].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[66].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[66].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[66].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[66].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[66].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[66].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x80000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[66].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[66].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[66].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[66].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 5
                if (dgd_PadSetting.Rows[i].Cells[70].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[70].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[69].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[71].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[69].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[71].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[70].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[70].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[70].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[70].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[70].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[70].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[70].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[70].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x100000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[70].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[70].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[70].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[70].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 6
                if (dgd_PadSetting.Rows[i].Cells[74].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[74].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[73].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[75].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[73].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[75].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[74].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[74].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[74].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[74].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[74].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[74].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[74].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[74].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x800000000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[74].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[74].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[74].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[74].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 7
                if (dgd_PadSetting.Rows[i].Cells[78].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[78].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[77].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[79].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[77].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[79].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[78].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[78].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[78].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[78].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[78].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[78].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[78].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[78].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x1000000000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[78].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[78].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[78].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[78].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 8
                if (dgd_PadSetting.Rows[i].Cells[82].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[82].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[81].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[83].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[81].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[83].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[82].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[82].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[82].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[82].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[82].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[82].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[82].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[82].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x2000000000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[82].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[82].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[82].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[82].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 9
                if (dgd_PadSetting.Rows[i].Cells[86].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[86].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[85].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[87].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[85].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[87].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[86].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[86].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[86].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[86].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[86].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[86].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[86].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[86].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x4000000000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[86].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[86].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[86].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[86].Style.SelectionBackColor = Color.Red;
                    }
                }

                // Dimension 10
                if (dgd_PadSetting.Rows[i].Cells[90].Value.ToString() != "---")
                {
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[90].Value.ToString());
                    if (intGroupNoIndex >= 0)
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[89].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[intGroupNoIndex].Cells[91].Value.ToString());
                    }
                    else
                    {
                        fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[89].Value.ToString());
                        fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[91].Value.ToString());
                    }


                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    {
                        dgd_PadSetting.Rows[i].Cells[90].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[90].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[90].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[90].Style.SelectionBackColor = Color.Red;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[90].Style.ForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[90].Style.SelectionForeColor = Color.Black;
                        dgd_PadSetting.Rows[i].Cells[90].Style.BackColor = Color.Lime;
                        dgd_PadSetting.Rows[i].Cells[90].Style.SelectionBackColor = Color.Lime;
                    }
                }
                else
                {
                    if ((m_smVisionInfo.g_arrPad[intPadIndex].GetSampleBlobFailMask(i) & 0x8000000000) > 0)
                    {
                        dgd_PadSetting.Rows[i].Cells[90].Style.ForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[90].Style.SelectionForeColor = Color.Yellow;
                        dgd_PadSetting.Rows[i].Cells[90].Style.BackColor = Color.Red;
                        dgd_PadSetting.Rows[i].Cells[90].Style.SelectionBackColor = Color.Red;
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Read pad template data to datagridview
        /// </summary>
        /// <param name="intPadIndex">pad position</param>
        /// <param name="dgd_PadSetting">datagridview</param>
        private void ReadPadTemplateDataToGrid(int intPadIndex, DataGridView dgd_PadSetting)
        {
            dgd_PadSetting.Rows.Clear();
            string strBlobsFeatures = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobsFeaturesInspectRealData();
            string[] strFeature = strBlobsFeatures.Split('#');
            int intBlobsCount = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobsFeaturesNumber();
            int intFeatureIndex = 0;

            for (int i = 0; i < intBlobsCount; i++)
            {
                dgd_PadSetting.Rows.Add();
                dgd_PadSetting.Rows[i].HeaderCell.Value = "Pad " + (i + 1);

                // Max OffSet
                dgd_PadSetting.Rows[i].Cells[0].Value = strFeature[intFeatureIndex++];

                // Min Max Area
                dgd_PadSetting.Rows[i].Cells[3].Value = strFeature[intFeatureIndex++];
                dgd_PadSetting.Rows[i].Cells[5].Value = strFeature[intFeatureIndex++];

                // Min Max Width
                dgd_PadSetting.Rows[i].Cells[7].Value = strFeature[intFeatureIndex++];
                dgd_PadSetting.Rows[i].Cells[9].Value = strFeature[intFeatureIndex++];

                // Min Max Length
                dgd_PadSetting.Rows[i].Cells[11].Value = strFeature[intFeatureIndex++];
                dgd_PadSetting.Rows[i].Cells[13].Value = strFeature[intFeatureIndex++];

                // Min Max Pitch
                if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
                    dgd_PadSetting.Rows[i].Cells[15].Value = "---";
                else
                    dgd_PadSetting.Rows[i].Cells[15].Value = strFeature[intFeatureIndex];
                intFeatureIndex++;

                if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
                    dgd_PadSetting.Rows[i].Cells[17].Value = "---";
                else
                    dgd_PadSetting.Rows[i].Cells[17].Value = strFeature[intFeatureIndex];
                intFeatureIndex++;

                // Min Max Gap
                if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
                    dgd_PadSetting.Rows[i].Cells[19].Value = "---";
                else
                    dgd_PadSetting.Rows[i].Cells[19].Value = strFeature[intFeatureIndex];
                intFeatureIndex++;

                if (Convert.ToSingle(strFeature[intFeatureIndex]) == -1)
                    dgd_PadSetting.Rows[i].Cells[21].Value = "---";
                else
                    dgd_PadSetting.Rows[i].Cells[21].Value = strFeature[intFeatureIndex];
                intFeatureIndex++;

                // Broken Max
                dgd_PadSetting.Rows[i].Cells[23].Value = strFeature[intFeatureIndex++];
                dgd_PadSetting.Rows[i].Cells[26].Value = strFeature[intFeatureIndex++];
                dgd_PadSetting.Rows[i].Cells[29].Value = strFeature[intFeatureIndex++];
            }

            //ColorGrid(dgd_PadSetting);
        }

        private void ReadPadAndGroupTemplateDataToGrid(int intPadIndex, DataGridView dgd_PadSetting)
        {
            dgd_PadSetting.Rows.Clear();

            List<List<string>> arrBlobsFeaturesData;
            if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantUseGroupToleranceSetting)
                arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobsFeaturesData_PadAndGroup();
            else
                arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobsFeaturesInspectRealData2();



            for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
            {
                dgd_PadSetting.Rows.Add();
                dgd_PadSetting.Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0] + " " + (Convert.ToInt32(arrBlobsFeaturesData[i][1]) + 1).ToString();

                // Max OffSet
                dgd_PadSetting.Rows[i].Cells[0].Value = arrBlobsFeaturesData[i][2];

                // Min Max Area
                dgd_PadSetting.Rows[i].Cells[3].Value = arrBlobsFeaturesData[i][3];
                dgd_PadSetting.Rows[i].Cells[5].Value = arrBlobsFeaturesData[i][4];

                // Min Max Width
                dgd_PadSetting.Rows[i].Cells[7].Value = arrBlobsFeaturesData[i][5];
                dgd_PadSetting.Rows[i].Cells[9].Value = arrBlobsFeaturesData[i][6];

                // Min Max Length
                dgd_PadSetting.Rows[i].Cells[11].Value = arrBlobsFeaturesData[i][7];
                dgd_PadSetting.Rows[i].Cells[13].Value = arrBlobsFeaturesData[i][8];

                // Min Max Pitch
                if (Convert.ToSingle(arrBlobsFeaturesData[i][9]) == -1)
                    dgd_PadSetting.Rows[i].Cells[15].Value = "---";
                else
                    dgd_PadSetting.Rows[i].Cells[15].Value = arrBlobsFeaturesData[i][9];

                if (Convert.ToSingle(arrBlobsFeaturesData[i][10]) == -1)
                    dgd_PadSetting.Rows[i].Cells[17].Value = "---";
                else
                    dgd_PadSetting.Rows[i].Cells[17].Value = arrBlobsFeaturesData[i][10];

                // Min Max Gap
                if (Convert.ToSingle(arrBlobsFeaturesData[i][11]) == -1)
                    dgd_PadSetting.Rows[i].Cells[19].Value = "---";
                else
                    dgd_PadSetting.Rows[i].Cells[19].Value = arrBlobsFeaturesData[i][11];

                if (Convert.ToSingle(arrBlobsFeaturesData[i][12]) == -1)
                    dgd_PadSetting.Rows[i].Cells[21].Value = "---";
                else
                    dgd_PadSetting.Rows[i].Cells[21].Value = arrBlobsFeaturesData[i][12];

                // Broken Max
                dgd_PadSetting.Rows[i].Cells[23].Value = arrBlobsFeaturesData[i][13];
                dgd_PadSetting.Rows[i].Cells[26].Value = arrBlobsFeaturesData[i][14];
                dgd_PadSetting.Rows[i].Cells[29].Value = arrBlobsFeaturesData[i][15];
                dgd_PadSetting.Rows[i].Cells[32].Value = arrBlobsFeaturesData[i][16];

                //Edge Limit Top
                dgd_PadSetting.Rows[i].Cells[35].Value = arrBlobsFeaturesData[i][17];
                //Edge Limit Right
                dgd_PadSetting.Rows[i].Cells[37].Value = arrBlobsFeaturesData[i][18];
                //Edge Limit Bottom
                dgd_PadSetting.Rows[i].Cells[39].Value = arrBlobsFeaturesData[i][19];
                //Edge Limit Left
                dgd_PadSetting.Rows[i].Cells[41].Value = arrBlobsFeaturesData[i][20];

                //Stand Off Top
                dgd_PadSetting.Rows[i].Cells[44].Value = arrBlobsFeaturesData[i][21];
                //Stand Off Bottom
                dgd_PadSetting.Rows[i].Cells[46].Value = arrBlobsFeaturesData[i][22];
                //Stand Off Left
                dgd_PadSetting.Rows[i].Cells[48].Value = arrBlobsFeaturesData[i][23];
                //Stand Off Right
                dgd_PadSetting.Rows[i].Cells[50].Value = arrBlobsFeaturesData[i][24];

                //Dimension 1
                dgd_PadSetting.Rows[i].Cells[53].Value = arrBlobsFeaturesData[i][25];
                dgd_PadSetting.Rows[i].Cells[55].Value = arrBlobsFeaturesData[i][26];
                //Dimension 2
                dgd_PadSetting.Rows[i].Cells[57].Value = arrBlobsFeaturesData[i][27];
                dgd_PadSetting.Rows[i].Cells[59].Value = arrBlobsFeaturesData[i][28];
                //Dimension 3
                dgd_PadSetting.Rows[i].Cells[61].Value = arrBlobsFeaturesData[i][29];
                dgd_PadSetting.Rows[i].Cells[63].Value = arrBlobsFeaturesData[i][30];
                //Dimension 4
                dgd_PadSetting.Rows[i].Cells[65].Value = arrBlobsFeaturesData[i][31];
                dgd_PadSetting.Rows[i].Cells[67].Value = arrBlobsFeaturesData[i][32];
                //Dimension 5
                dgd_PadSetting.Rows[i].Cells[69].Value = arrBlobsFeaturesData[i][33];
                dgd_PadSetting.Rows[i].Cells[71].Value = arrBlobsFeaturesData[i][34];
                //Dimension 6
                dgd_PadSetting.Rows[i].Cells[73].Value = arrBlobsFeaturesData[i][35];
                dgd_PadSetting.Rows[i].Cells[75].Value = arrBlobsFeaturesData[i][36];
                //Dimension 7
                dgd_PadSetting.Rows[i].Cells[77].Value = arrBlobsFeaturesData[i][37];
                dgd_PadSetting.Rows[i].Cells[79].Value = arrBlobsFeaturesData[i][38];
                //Dimension 8
                dgd_PadSetting.Rows[i].Cells[81].Value = arrBlobsFeaturesData[i][39];
                dgd_PadSetting.Rows[i].Cells[83].Value = arrBlobsFeaturesData[i][40];
                //Dimension 9
                dgd_PadSetting.Rows[i].Cells[85].Value = arrBlobsFeaturesData[i][41];
                dgd_PadSetting.Rows[i].Cells[87].Value = arrBlobsFeaturesData[i][42];
                //Dimension 10
                dgd_PadSetting.Rows[i].Cells[89].Value = arrBlobsFeaturesData[i][43];
                dgd_PadSetting.Rows[i].Cells[91].Value = arrBlobsFeaturesData[i][44];


                ////Offset
                //dgd_PadSetting.Rows[i].Cells[73].Value = arrBlobsFeaturesData[i][35];
                //dgd_PadSetting.Rows[i].Cells[75].Value = arrBlobsFeaturesData[i][36];
                //dgd_PadSetting.Rows[i].Cells[77].Value = arrBlobsFeaturesData[i][37];
                //dgd_PadSetting.Rows[i].Cells[79].Value = arrBlobsFeaturesData[i][38];

                m_arrPadRowIndex.Add(i);
            }

            ColorGrid(dgd_PadSetting, m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantUseGroupToleranceSetting);

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantStandOff_Pad)
            {
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(0))
                {
                    m_dgdView[0].Columns[44].Visible = true;
                    m_dgdView[0].Columns[45].Visible = chk_DisplayResult.Checked;
                }
                else
                {
                    m_dgdView[0].Columns[44].Visible = false;
                    m_dgdView[0].Columns[45].Visible = false;
                }
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(1))
                {
                    m_dgdView[0].Columns[46].Visible = true;
                    m_dgdView[0].Columns[47].Visible = chk_DisplayResult.Checked;
                }
                else
                {
                    m_dgdView[0].Columns[46].Visible = false;
                    m_dgdView[0].Columns[47].Visible = false;
                }
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(2))
                {
                    m_dgdView[0].Columns[48].Visible = true;
                    m_dgdView[0].Columns[49].Visible = chk_DisplayResult.Checked;
                }
                else
                {
                    m_dgdView[0].Columns[48].Visible = false;
                    m_dgdView[0].Columns[49].Visible = false;
                }
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(3))
                {
                    m_dgdView[0].Columns[50].Visible = true;
                    m_dgdView[0].Columns[51].Visible = chk_DisplayResult.Checked;
                }
                else
                {
                    m_dgdView[0].Columns[50].Visible = false;
                    m_dgdView[0].Columns[51].Visible = false;
                }
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(0) && m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(1) && m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(2) && m_smVisionInfo.g_arrPad[m_intPadIndex].GetWantCheckStandOff(3))
                    m_dgdView[0].Columns[52].Visible = true;
                else
                    m_dgdView[0].Columns[52].Visible = false;
            }
            else
            {
                //Hide all Stand Off Column
                m_dgdView[0].Columns[44].Visible = false;
                m_dgdView[0].Columns[45].Visible = false;
                m_dgdView[0].Columns[46].Visible = false;
                m_dgdView[0].Columns[47].Visible = false;
                m_dgdView[0].Columns[48].Visible = false;
                m_dgdView[0].Columns[49].Visible = false;
                m_dgdView[0].Columns[50].Visible = false;
                m_dgdView[0].Columns[51].Visible = false;
                m_dgdView[0].Columns[52].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
            {
                chk_DisplayGroupNo.Visible = false;
                //UpdateHeaderCell(false);
            }
            else
            {
                chk_DisplayGroupNo.Visible = true;
                UpdateHeaderCell(chk_DisplayGroupNo.Checked);
            }
        }

        private void ColorGrid(DataGridView dgd_PadSetting, bool blnUseGroupSetting)
        {
            Color cSettingBackColor = Color.White;
            Color cSettingErrorBackColor = Color.Pink;
            Color cResultBackColor = Color.Lime;
            for (int i = 0; i < dgd_PadSetting.Rows.Count; i++)
            {
                if (dgd_PadSetting.Rows[i].HeaderCell.Value.ToString().IndexOf("Group") >= 0)
                {
                    if (blnUseGroupSetting)
                    {
                        cSettingBackColor = Color.White;
                        cResultBackColor = Color.LightGray;
                    }
                    else
                    {
                        cSettingBackColor = Color.LightGray;
                        cResultBackColor = Color.LightGray;
                    }
                }
                else
                {
                    if (blnUseGroupSetting)
                    {
                        cSettingBackColor = Color.LightGray;
                        cResultBackColor = Color.Lime;
                    }
                    else
                    {
                        cSettingBackColor = Color.White;
                        cResultBackColor = Color.Lime;
                    }
                }

                // OffSet
                dgd_PadSetting.Rows[i].Cells[0].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[0].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[1].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[1].Style.SelectionBackColor = cResultBackColor;

                // Area
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[3].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[5].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[3].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[3].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[5].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[5].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[3].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[3].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[5].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[5].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[4].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[4].Style.SelectionBackColor = cResultBackColor;

                // Width
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[7].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[9].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[7].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[7].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[9].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[9].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[7].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[7].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[9].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[9].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[8].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[8].Style.SelectionBackColor = cResultBackColor;

                // Length
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[11].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[13].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[11].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[11].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[13].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[13].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[11].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[11].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[13].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[13].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[12].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[12].Style.SelectionBackColor = cResultBackColor;

                // Pitch
                if (dgd_PadSetting.Rows[i].Cells[15].Value.ToString() != "---" && dgd_PadSetting.Rows[i].Cells[17].Value.ToString() != "---")
                {
                    if (float.Parse(dgd_PadSetting.Rows[i].Cells[15].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[17].Value.ToString()))
                    {
                        dgd_PadSetting.Rows[i].Cells[15].Style.BackColor = cSettingErrorBackColor;
                        dgd_PadSetting.Rows[i].Cells[15].Style.SelectionBackColor = cSettingErrorBackColor;
                        dgd_PadSetting.Rows[i].Cells[17].Style.BackColor = cSettingErrorBackColor;
                        dgd_PadSetting.Rows[i].Cells[17].Style.SelectionBackColor = cSettingErrorBackColor;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[15].Style.BackColor = cSettingBackColor;
                        dgd_PadSetting.Rows[i].Cells[15].Style.SelectionBackColor = cSettingBackColor;
                        dgd_PadSetting.Rows[i].Cells[17].Style.BackColor = cSettingBackColor;
                        dgd_PadSetting.Rows[i].Cells[17].Style.SelectionBackColor = cSettingBackColor;
                    }
                    dgd_PadSetting.Rows[i].Cells[16].Style.BackColor = cResultBackColor;
                    dgd_PadSetting.Rows[i].Cells[16].Style.SelectionBackColor = cResultBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[15].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[15].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[16].Style.BackColor = cResultBackColor;
                    dgd_PadSetting.Rows[i].Cells[16].Style.SelectionBackColor = cResultBackColor;
                    dgd_PadSetting.Rows[i].Cells[17].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[17].Style.SelectionBackColor = cSettingBackColor;
                }

                // Gap
                if (dgd_PadSetting.Rows[i].Cells[19].Value.ToString() != "---" && dgd_PadSetting.Rows[i].Cells[21].Value.ToString() != "---")
                {
                    if (float.Parse(dgd_PadSetting.Rows[i].Cells[19].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[21].Value.ToString()))
                    {
                        dgd_PadSetting.Rows[i].Cells[19].Style.BackColor = cSettingErrorBackColor;
                        dgd_PadSetting.Rows[i].Cells[19].Style.SelectionBackColor = cSettingErrorBackColor;
                        dgd_PadSetting.Rows[i].Cells[21].Style.BackColor = cSettingErrorBackColor;
                        dgd_PadSetting.Rows[i].Cells[21].Style.SelectionBackColor = cSettingErrorBackColor;
                    }
                    else
                    {
                        dgd_PadSetting.Rows[i].Cells[19].Style.BackColor = cSettingBackColor;
                        dgd_PadSetting.Rows[i].Cells[19].Style.SelectionBackColor = cSettingBackColor;
                        dgd_PadSetting.Rows[i].Cells[21].Style.BackColor = cSettingBackColor;
                        dgd_PadSetting.Rows[i].Cells[21].Style.SelectionBackColor = cSettingBackColor;
                    }
                    dgd_PadSetting.Rows[i].Cells[20].Style.BackColor = cResultBackColor;
                    dgd_PadSetting.Rows[i].Cells[20].Style.SelectionBackColor = cResultBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[19].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[19].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[20].Style.BackColor = cResultBackColor;
                    dgd_PadSetting.Rows[i].Cells[20].Style.SelectionBackColor = cResultBackColor;
                    dgd_PadSetting.Rows[i].Cells[21].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[21].Style.SelectionBackColor = cSettingBackColor;
                }

                // Broken Area
                dgd_PadSetting.Rows[i].Cells[23].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[23].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[24].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[24].Style.SelectionBackColor = cResultBackColor;

                // Broken Length
                dgd_PadSetting.Rows[i].Cells[26].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[26].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[27].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[27].Style.SelectionBackColor = cResultBackColor;

                // Excess Area
                dgd_PadSetting.Rows[i].Cells[29].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[29].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[30].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[30].Style.SelectionBackColor = cResultBackColor;

                // Smear Length
                dgd_PadSetting.Rows[i].Cells[32].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[32].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[33].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[33].Style.SelectionBackColor = cResultBackColor;

                // Edge Limit Top
                dgd_PadSetting.Rows[i].Cells[35].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[35].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[36].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[36].Style.SelectionBackColor = cResultBackColor;

                // Edge Limit Right
                dgd_PadSetting.Rows[i].Cells[37].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[37].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[38].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[38].Style.SelectionBackColor = cResultBackColor;

                // Edge Limit Bottom
                dgd_PadSetting.Rows[i].Cells[39].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[39].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[40].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[40].Style.SelectionBackColor = cResultBackColor;

                // Edge Limit Left
                dgd_PadSetting.Rows[i].Cells[41].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[41].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[42].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[42].Style.SelectionBackColor = cResultBackColor;

                // Stand Off Top
                dgd_PadSetting.Rows[i].Cells[44].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[44].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[45].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[45].Style.SelectionBackColor = cResultBackColor;

                // Stand Off Bottom
                dgd_PadSetting.Rows[i].Cells[46].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[46].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[47].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[47].Style.SelectionBackColor = cResultBackColor;

                // Stand Off Left
                dgd_PadSetting.Rows[i].Cells[48].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[48].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[49].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[49].Style.SelectionBackColor = cResultBackColor;

                // Stand Off Right
                dgd_PadSetting.Rows[i].Cells[50].Style.BackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[50].Style.SelectionBackColor = cSettingBackColor;
                dgd_PadSetting.Rows[i].Cells[51].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[51].Style.SelectionBackColor = cResultBackColor;

                //Dimension 1
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[53].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[55].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[53].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[53].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[55].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[55].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[53].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[53].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[55].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[55].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[54].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[54].Style.SelectionBackColor = cResultBackColor;

                //Dimension 2
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[57].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[59].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[57].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[57].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[59].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[59].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[57].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[57].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[59].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[59].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[58].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[58].Style.SelectionBackColor = cResultBackColor;

                //Dimension 3
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[61].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[63].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[61].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[61].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[63].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[63].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[61].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[61].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[63].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[63].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[62].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[62].Style.SelectionBackColor = cResultBackColor;

                //Dimension 4
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[65].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[67].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[65].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[65].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[67].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[67].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[65].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[65].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[67].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[67].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[66].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[66].Style.SelectionBackColor = cResultBackColor;

                //Dimension 5
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[69].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[71].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[69].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[69].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[71].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[71].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[69].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[69].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[71].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[71].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[70].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[70].Style.SelectionBackColor = cResultBackColor;

                //Dimension 6
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[73].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[75].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[73].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[73].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[75].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[75].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[73].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[73].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[75].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[75].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[74].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[74].Style.SelectionBackColor = cResultBackColor;

                //Dimension 7
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[77].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[79].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[77].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[77].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[79].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[79].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[77].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[77].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[79].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[79].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[78].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[78].Style.SelectionBackColor = cResultBackColor;

                //Dimension 8
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[81].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[83].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[81].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[81].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[83].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[83].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[81].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[81].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[83].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[83].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[82].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[82].Style.SelectionBackColor = cResultBackColor;

                //Dimension 9
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[85].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[87].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[85].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[85].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[87].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[87].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[85].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[85].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[87].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[87].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[86].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[86].Style.SelectionBackColor = cResultBackColor;

                //Dimension 10
                if (float.Parse(dgd_PadSetting.Rows[i].Cells[89].Value.ToString()) > float.Parse(dgd_PadSetting.Rows[i].Cells[91].Value.ToString()))
                {
                    dgd_PadSetting.Rows[i].Cells[89].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[89].Style.SelectionBackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[91].Style.BackColor = cSettingErrorBackColor;
                    dgd_PadSetting.Rows[i].Cells[91].Style.SelectionBackColor = cSettingErrorBackColor;
                }
                else
                {
                    dgd_PadSetting.Rows[i].Cells[89].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[89].Style.SelectionBackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[91].Style.BackColor = cSettingBackColor;
                    dgd_PadSetting.Rows[i].Cells[91].Style.SelectionBackColor = cSettingBackColor;
                }
                dgd_PadSetting.Rows[i].Cells[90].Style.BackColor = cResultBackColor;
                dgd_PadSetting.Rows[i].Cells[90].Style.SelectionBackColor = cResultBackColor;

                ////Offset
                //dgd_PadSetting.Rows[i].Cells[73].Style.BackColor = cSettingBackColor;
                //dgd_PadSetting.Rows[i].Cells[73].Style.SelectionBackColor = cSettingBackColor;
                //dgd_PadSetting.Rows[i].Cells[75].Style.BackColor = cSettingBackColor;
                //dgd_PadSetting.Rows[i].Cells[75].Style.SelectionBackColor = cSettingBackColor;
                //dgd_PadSetting.Rows[i].Cells[77].Style.BackColor = cSettingBackColor;
                //dgd_PadSetting.Rows[i].Cells[77].Style.SelectionBackColor = cSettingBackColor;
                //dgd_PadSetting.Rows[i].Cells[79].Style.BackColor = cSettingBackColor;
                //dgd_PadSetting.Rows[i].Cells[79].Style.SelectionBackColor = cSettingBackColor;

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

                XmlParser objFile;
                // Load Pad Advance Setting
                objFile = new XmlParser(strPath + "Settings.xml");
                objFile.GetFirstSection("Advanced");
                m_smVisionInfo.g_arrPad[i].ref_blnWhiteOnBlack = objFile.GetValueAsBoolean("WhiteOnBlack", true, 1);

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
        }

        private void SavePadSetting(string strFolderPath)
        {
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

                
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrPad[i].SavePad(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad "+ strSectionName + " Tolerance Setting", m_smProductionInfo.g_strLotID);
                
                //objFile.WriteElement1Value("OrientSetting", true);
                //objFile.WriteElement2Value("MatchMinScore", m_smVisionInfo.g_arrPadOrient[i].ref_fMinScore); 
            }

            //if (m_smVisionInfo.g_arrPin1 != null)
            //{
            //    
            //    STDeviceEdit.CopySettingFile(strFolderPath, "Pin1Template.xml");
            //    m_smVisionInfo.g_arrPin1[0].SaveTemplate(strFolderPath + "\\Template\\");
            //    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Tolerance Settings", strFolderPath, "Pin1Template.xml");
            //}
        }

        private void PadMeasureSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_blnFormOpen = true;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void PadMeasureSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Tolerance Setting Form Closed", "Exit Pad Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnViewPadEdgeLimitSetting = false;
            m_smVisionInfo.g_blnViewPadStandOffSetting = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_blnFormOpen = false;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //update CPK tolerance data (USL & LSL) to the latest when setting change
            if (m_smVisionInfo.g_blnCPKON)
            {
                string strTemplateBlobsFeatures;
                string[] strTemplateFeature = new string[100];
                int intPadNumber;

                int intGroupIndex = 0;
                for (int p = 0; p < m_smVisionInfo.g_arrPad.Length; p++)
                {
                    if (p > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    intPadNumber = m_smVisionInfo.g_arrPad[p].GetBlobsFeaturesNumber();

                    for (int i = 0; i < intPadNumber; i++)
                    {
                        strTemplateBlobsFeatures = m_smVisionInfo.g_arrPad[p].GetBlobFeaturesInspectRealData(i);
                        strTemplateFeature = strTemplateBlobsFeatures.Split('#');

                        for (int j = 0; j < strTemplateFeature.Length; j++)
                        {
                            if (strTemplateFeature[j] != "")
                                if (Convert.ToSingle(strTemplateFeature[j]) == -1)
                                    strTemplateFeature[j] = "0";
                        }

                        m_smVisionInfo.g_objCPK.SetSpecification(0, intGroupIndex, 0f, Convert.ToSingle(strTemplateFeature[0]));
                        m_smVisionInfo.g_objCPK.SetSpecification(1, intGroupIndex, Convert.ToSingle(strTemplateFeature[1]), Convert.ToSingle(strTemplateFeature[2]));
                        m_smVisionInfo.g_objCPK.SetSpecification(2, intGroupIndex, Convert.ToSingle(strTemplateFeature[3]), Convert.ToSingle(strTemplateFeature[4]));
                        m_smVisionInfo.g_objCPK.SetSpecification(3, intGroupIndex, Convert.ToSingle(strTemplateFeature[5]), Convert.ToSingle(strTemplateFeature[6]));
                        m_smVisionInfo.g_objCPK.SetSpecification(4, intGroupIndex, Convert.ToSingle(strTemplateFeature[7]), Convert.ToSingle(strTemplateFeature[8]));
                        m_smVisionInfo.g_objCPK.SetSpecification(5, intGroupIndex, Convert.ToSingle(strTemplateFeature[9]), Convert.ToSingle(strTemplateFeature[10]));

                        intGroupIndex++;
                    }
                }
            }

            // Save Pad Setting
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\";

            SavePadSetting(strPath + "Pad\\");

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
            this.Close();
            this.Dispose();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            //if (IsSettingError())
            //{
            //    SRMMessageBox.Show("Set minimum value or maximum value is not corrects. Please check the red highlight value is correct or not.");
            //    return;

            //}


            CheckSaveGoldenData(m_intPadIndex, m_dgdView[0]);
            // Load Pad Setting
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            LoadPadSetting(strFolderPath + "Pad\\");


            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
            this.Close();
            this.Dispose();
        }

        private void radioBtn_PadIndex_Click(object sender, EventArgs e)
        {
            m_blnUpdateSelectedROISetting = true;

            if (IsSettingError())
            {
                SRMMessageBox.Show("Set minimum value or maximum value is not corrects. Please check the red highlight value is correct or not.");

                switch (m_intPadIndex)
                {
                    case 0:
                        radioBtn_Middle.Checked = true;
                        break;
                    case 1:
                        radioBtn_Up.Checked = true;
                        break;
                    case 2:
                        radioBtn_Right.Checked = true;
                        break;
                    case 3:
                        radioBtn_Down.Checked = true;
                        break;
                    case 4:
                        radioBtn_Left.Checked = true;
                        break;
                }
                return;
            }

            CheckSaveGoldenData(m_intPadIndex, m_dgdView[0]);

            if (sender == radioBtn_Middle)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 0;
            }
            else if (sender == radioBtn_Up)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 1;
            }
            else if (sender == radioBtn_Right)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 2;
            }
            else if (sender == radioBtn_Down)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 3;
            }
            else if (sender == radioBtn_Left)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 4;
            }

            tabControl_Pad5S.SelectedIndex = 0;

            ReadPadAndGroupTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

            if (objGoldenUnitForm != null)
            {
                if (objGoldenUnitForm.ref_intViewGoldenDataColumn)
                {
                    if (m_dgdView[0].Columns.Count <= 92)
                    {
                        m_dgdView[0].Columns.Add(column_GolWidth);
                        m_dgdView[0].Columns.Add(column_GolLength);
                        m_dgdView[0].Columns.Add(column_GolPitch);
                        m_dgdView[0].Columns.Add(column_GolGap);
                    }
                }
            }

            UpdateGoldenDataIntoGridTable(m_intPadIndex, m_dgdView[0]);

            UpdateScore(m_intPadIndex, m_dgdView[0]);

            //txt_ExtraPadMinLength.Text = m_smVisionInfo.g_arrPad[m_intPadIndex].GetExtraPadLengthLimit(1).ToString("F" + m_intDecimal);
            //txt_ExtraPadMinArea.Text = m_smVisionInfo.g_arrPad[m_intPadIndex].GetExtraPadMinArea(1).ToString("F" + m_intDecimal2);
            //txt_TotalExtraPadMinArea.Text = m_smVisionInfo.g_arrPad[m_intPadIndex].GetTotalExtraPadMinArea(1).ToString("F" + m_intDecimal2);
            m_blnInitDone = false;
            UpdateWholdPadsSettingGUI();
            UpdateWholePadScore();
            m_blnInitDone = true;

            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            m_blnUpdateSelectedROISetting = false;

            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
            m_smVisionInfo.g_blnViewPadEdgeLimitSetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_MiddlePad_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex == 1 || e.ColumnIndex == 4 || e.ColumnIndex == 8 ||
                e.ColumnIndex == 12 || e.ColumnIndex == 16 || e.ColumnIndex == 20 ||
                e.ColumnIndex == 24 || e.ColumnIndex == 27 || e.ColumnIndex == 30 || e.ColumnIndex == 33 ||
                e.ColumnIndex == 36 || e.ColumnIndex == 38 || e.ColumnIndex == 40 || e.ColumnIndex == 42 ||
                e.ColumnIndex == 45 || e.ColumnIndex == 47 || e.ColumnIndex == 49 || e.ColumnIndex == 51 ||
                e.ColumnIndex == 54 || e.ColumnIndex == 58 || e.ColumnIndex == 62 || e.ColumnIndex == 66 || e.ColumnIndex == 70 ||
                e.ColumnIndex == 74 || e.ColumnIndex == 78 || e.ColumnIndex == 82 || e.ColumnIndex == 86 || e.ColumnIndex == 90)
                return;

            // Skip if col is separation
            if (e.ColumnIndex == 2 || e.ColumnIndex == 6 || e.ColumnIndex == 10 ||
                e.ColumnIndex == 14 || e.ColumnIndex == 18 || e.ColumnIndex == 22 ||
                e.ColumnIndex == 25 || e.ColumnIndex == 28 || e.ColumnIndex == 31 || e.ColumnIndex == 34 || e.ColumnIndex == 43 ||
                e.ColumnIndex == 52 || e.ColumnIndex == 56 || e.ColumnIndex == 60 || e.ColumnIndex == 64 || e.ColumnIndex == 68 ||
                e.ColumnIndex == 72 || e.ColumnIndex == 76 || e.ColumnIndex == 80 || e.ColumnIndex == 84 || e.ColumnIndex == 88)
                //|| e.ColumnIndex == 92 || e.ColumnIndex == 94 || e.ColumnIndex == 96 || e.ColumnIndex == 98)
                return;

            // Skip if col is golden unit data
            if (e.ColumnIndex == 92 || e.ColumnIndex == 93 || e.ColumnIndex == 94 || e.ColumnIndex == 95)
                return;

            // Skip if cell value is ---
            if ((((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "---") ||
                (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == ""))
                return;

            bool blnViewSameGroupCheckBox = true;

            // Skip for pad row setting change if Using Group Tolerance Setting
            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
            {
                blnViewSameGroupCheckBox = false;
                if (((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value.ToString().IndexOf("Group") < 0)
                {
                    return;
                }
            }

            //Min, max area, broken area has 6 decimal places 
            if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
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

            string strCurrentSetValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            string strDisplayMessage = "Set value to " + ((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value.ToString();

            int intSetAllROI;
            if (m_intPadIndex == 0)
                intSetAllROI = 0;
            else
                intSetAllROI = 1;

            SetValueForm objSetValueForm = new SetValueForm(strDisplayMessage, m_strUnitLabel, e.RowIndex, m_intDecimalPlaces, strCurrentSetValue, intSetAllROI, false, blnViewSameGroupCheckBox);

            objSetValueForm.TopMost = true;
            if (objSetValueForm.ShowDialog() == DialogResult.OK)
            {
                int intStartRowNumber, intGroupNo = -1;
                if (objSetValueForm.ref_blnSetAllRows || objSetValueForm.ref_blnSetAllSameGroup)
                {
                    intStartRowNumber = 0;
                    if (objSetValueForm.ref_blnSetAllSameGroup)
                        intGroupNo = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesGroupNo(e.RowIndex);
                }
                else
                {
                    intStartRowNumber = e.RowIndex;
                }

                int intStartIndex;
                int intEndIndex;
                if (objSetValueForm.ref_blnSetAllROI)
                {
                    intStartIndex = 1;
                    intEndIndex = m_smVisionInfo.g_arrPad.Length;
                }
                else
                {
                    intStartIndex = m_intPadIndex;
                    intEndIndex = m_intPadIndex + 1;
                }

                // ----------------- for selected pad ---------------------------------------------------

                float fMinPitch = 0, fMaxPitch = 0, fMinGap = 0, fMaxGap = 0;
                //int intTotalRows = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesNumber();
                //if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                //{
                //    intTotalRows = e.RowIndex + m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intPadGroupCount;
                //}
                //else if (!objSetValueForm.ref_blnSetAllRows)
                //{
                //    intTotalRows = intStartRowNumber + 1;
                //}
                int intTotalRows;
                if (objSetValueForm.ref_blnSetAllRows || objSetValueForm.ref_blnSetAllSameGroup)
                {
                    intTotalRows = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesNumber();

                    if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                    {
                        intTotalRows += m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intPadGroupCount;  // 2019 12 23 - CCENG: total pad rows + additional group rows.
                    }
                }
                else
                {
                    intTotalRows = intStartRowNumber + 1;
                }

                // Loop row index
                for (int i = intStartRowNumber; i < intTotalRows; i++)
                {
                    if (objSetValueForm.ref_blnSetAllSameGroup)
                    {
                        if (intGroupNo == -1)
                            break;

                        if (intGroupNo != m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesGroupNo(i))
                            continue;
                    }

                    if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                    {
                        if (((DataGridView)sender).Rows[i].HeaderCell.Value.ToString().IndexOf("Group") < 0)
                        {
                            continue;
                        }
                    }

                    if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
                    {
                        // Check insert data valid or not
                        if (e.ColumnIndex == 3 || e.ColumnIndex == 7 || e.ColumnIndex == 11 || e.ColumnIndex == 15 || e.ColumnIndex == 19
                             || e.ColumnIndex == 53 || e.ColumnIndex == 57 || e.ColumnIndex == 61 || e.ColumnIndex == 65 || e.ColumnIndex == 69
                             || e.ColumnIndex == 73 || e.ColumnIndex == 77 || e.ColumnIndex == 81 || e.ColumnIndex == 85 || e.ColumnIndex == 89)
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
                        else if (e.ColumnIndex == 5 || e.ColumnIndex == 9 || e.ColumnIndex == 13 || e.ColumnIndex == 17 || e.ColumnIndex == 21
                            || e.ColumnIndex == 55 || e.ColumnIndex == 59 || e.ColumnIndex == 63 || e.ColumnIndex == 67 || e.ColumnIndex == 71
                            || e.ColumnIndex == 75 || e.ColumnIndex == 79 || e.ColumnIndex == 83 || e.ColumnIndex == 87 || e.ColumnIndex == 91)
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

                        // Set new insert value into table
                        if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F6");
                        else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                    }
                }

                // --------------------------------------------------------------------------------------

                // Loop Pad index
                for (int j = intStartIndex; j < intEndIndex; j++)
                {
                    intTotalRows = m_smVisionInfo.g_arrPad[j].GetBlobsFeaturesNumber();
                    if (m_smVisionInfo.g_arrPad[j].ref_blnWantUseGroupToleranceSetting)
                    {
                        intTotalRows = e.RowIndex + m_smVisionInfo.g_arrPad[j].ref_intPadGroupCount;
                    }
                    else if (!objSetValueForm.ref_blnSetAllRows && !objSetValueForm.ref_blnSetAllSameGroup)
                    {
                        intTotalRows = intStartRowNumber + 1;
                    }
                    // Loop row index
                    for (int i = intStartRowNumber; i < intTotalRows; i++)
                    {
                        if (objSetValueForm.ref_blnSetAllSameGroup)
                        {
                            if (intGroupNo == -1)
                                break;

                            if (intGroupNo != m_smVisionInfo.g_arrPad[j].GetBlobsFeaturesGroupNo(i))
                                continue;
                        }

                        if (m_smVisionInfo.g_arrPad[j].ref_blnWantUseGroupToleranceSetting)
                        {
                            // 2019 03 01 - JBTAN: update group individual setting
                            int intGroupNoIndex = Convert.ToInt32(((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value.ToString().Substring(5)) - 1; // changed i to e.ROwIndex
                                                                                                                                                         //if (e.ColumnIndex == 15)
                                                                                                                                                         //    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex, fMinPitch);
                                                                                                                                                         //else if (e.ColumnIndex == 17)
                                                                                                                                                         //    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex, fMaxPitch);
                                                                                                                                                         //else if (e.ColumnIndex == 19)
                                                                                                                                                         //    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex, fMinGap);
                                                                                                                                                         //else if (e.ColumnIndex == 21)
                                                                                                                                                         //    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex, fMaxGap);
                                                                                                                                                         //else
                            m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex,
                                objSetValueForm.ref_fSetValue);
                        }
                        else
                        {

                            // 2019 02 28 - JBTAN: update individual setting
                            //if (e.ColumnIndex == 15)
                            //    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex, fMinPitch);
                            //else if (e.ColumnIndex == 17)
                            //    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex, fMaxPitch);
                            //else if (e.ColumnIndex == 19)
                            //    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex, fMinGap);
                            //else if (e.ColumnIndex == 21)
                            //    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex, fMaxGap);
                            //else
                            m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex,
                                objSetValueForm.ref_fSetValue);

                            ////Update template pitch gap setting
                            //m_smVisionInfo.g_arrPad[j].SetPitchGapDataFrom(i,
                            //   fMinPitch,
                            //   fMaxPitch,
                            //   fMinGap,
                            //   fMaxGap);

                            // 2019 02 28 - Update template pitch gap individual setting
                            if (e.ColumnIndex == 15)
                                m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, objSetValueForm.ref_fSetValue);
                            else if (e.ColumnIndex == 17)
                                m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, objSetValueForm.ref_fSetValue);
                            else if (e.ColumnIndex == 19)
                                m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, objSetValueForm.ref_fSetValue);
                            else if (e.ColumnIndex == 21)
                                m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, objSetValueForm.ref_fSetValue);
                        }
                    }
                }




                //Validate min, max value
                //////for (int i = intStartRowNumber; i < intMaxRows; i++)
                //////{
                //////    if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                //////    {
                //////        if (((DataGridView)sender).Rows[i].HeaderCell.Value.ToString().IndexOf("Group") < 0)
                //////        {
                //////            continue;
                //////        }
                //////    }
                //////    //else
                //////    {
                //////        if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
                //////        {
                //////            // Check insert data valid or not
                //////            if (e.ColumnIndex == 3 || e.ColumnIndex == 7 || e.ColumnIndex == 11 || e.ColumnIndex == 15 || e.ColumnIndex == 19)  // 3=
                //////            {
                //////                float fMax = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Value.ToString());

                //////                if (objSetValueForm.ref_fSetValue > fMax)
                //////                {
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.BackColor = Color.Pink;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.SelectionBackColor = Color.Pink;
                //////                }
                //////                else
                //////                {
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.BackColor = Color.White;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.SelectionBackColor = Color.White;
                //////                }
                //////            }
                //////            else if (e.ColumnIndex == 5 || e.ColumnIndex == 9 || e.ColumnIndex == 13 || e.ColumnIndex == 17 || e.ColumnIndex == 21)
                //////            {
                //////                float fMin = float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Value.ToString());
                //////                if (fMin > objSetValueForm.ref_fSetValue)
                //////                {
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.Pink;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.Pink;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.BackColor = Color.Pink;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.SelectionBackColor = Color.Pink;
                //////                }
                //////                else
                //////                {
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.BackColor = Color.White;
                //////                    ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.SelectionBackColor = Color.White;
                //////                }
                //////            }

                //////            // Set new insert value into table
                //////            if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
                //////                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F6");
                //////            else
                //////                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                //////        }

                //////        //Set value column selected
                //////        float fMinPitch, fMaxPitch, fMinGap, fMaxGap;
                //////        if (((DataGridView)sender).Rows[i].Cells[15].Value.ToString() == "---")
                //////            fMinPitch = -1;
                //////        else
                //////            fMinPitch = float.Parse(((DataGridView)sender).Rows[i].Cells[15].Value.ToString());

                //////        if (((DataGridView)sender).Rows[i].Cells[17].Value.ToString() == "---")
                //////            fMaxPitch = -1;
                //////        else
                //////            fMaxPitch = float.Parse(((DataGridView)sender).Rows[i].Cells[17].Value.ToString());

                //////        if (((DataGridView)sender).Rows[i].Cells[19].Value.ToString() == "---")
                //////            fMinGap = -1;
                //////        else
                //////            fMinGap = float.Parse(((DataGridView)sender).Rows[i].Cells[19].Value.ToString());

                //////        if (((DataGridView)sender).Rows[i].Cells[21].Value.ToString() == "---")
                //////            fMaxGap = -1;
                //////        else
                //////            fMaxGap = float.Parse(((DataGridView)sender).Rows[i].Cells[21].Value.ToString());

                //////        for (int j = intStartIndex; j < m_smVisionInfo.g_arrPad.Length; j++)
                //////        {
                //////            if (m_smVisionInfo.g_arrPad[j].ref_blnWantUseGroupToleranceSetting)
                //////            {
                //////                //int intGroupNoIndex = Convert.ToInt32(((DataGridView)sender).Rows[i].HeaderCell.Value.ToString().Substring(5)) - 1;
                //////                //m_smVisionInfo.g_arrPad[j].UpdateGroupBlobFeatureToPixel(intGroupNoIndex,
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[11].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[13].Value.ToString()),
                //////                //        fMinPitch,
                //////                //        fMaxPitch,
                //////                //        fMinGap,
                //////                //        fMaxGap,
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[23].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[26].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[29].Value.ToString())
                //////                //        );

                //////                // 2019 03 01 - JBTAN: update group individual setting
                //////                int intGroupNoIndex = Convert.ToInt32(((DataGridView)sender).Rows[i].HeaderCell.Value.ToString().Substring(5)) - 1;
                //////                if (e.ColumnIndex == 15)
                //////                    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex, fMinPitch);
                //////                else if (e.ColumnIndex == 17)
                //////                    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex, fMaxPitch);
                //////                else if (e.ColumnIndex == 19)
                //////                    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex, fMinGap);
                //////                else if (e.ColumnIndex == 21)
                //////                    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex, fMaxGap);
                //////                else
                //////                    m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex,
                //////                        float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString()));
                //////            }
                //////            else
                //////            {
                //////                //int intLengthMode = m_smVisionInfo.g_arrPad[j].GetSampleLengthMode(i);

                //////                ////Update template setting
                //////                //if (intLengthMode == 1)
                //////                //{
                //////                //    m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureToPixel(i,
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[11].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[13].Value.ToString()),
                //////                //        fMinPitch,
                //////                //        fMaxPitch,
                //////                //        fMinGap,
                //////                //        fMaxGap,
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[23].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[26].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[29].Value.ToString())
                //////                //        );
                //////                //}
                //////                //else
                //////                //{
                //////                //    m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureToPixel(i,
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[11].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[13].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                //////                //        fMinPitch,
                //////                //        fMaxPitch,
                //////                //        fMinGap,
                //////                //        fMaxGap,
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[23].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[26].Value.ToString()),
                //////                //        float.Parse(((DataGridView)sender).Rows[i].Cells[29].Value.ToString())
                //////                //        );
                //////                //}

                //////                // 2019 02 28 - JBTAN: update individual setting
                //////                if (e.ColumnIndex == 15)
                //////                    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex, fMinPitch);
                //////                else if (e.ColumnIndex == 17)
                //////                    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex, fMaxPitch);
                //////                else if (e.ColumnIndex == 19)
                //////                    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex, fMinGap);
                //////                else if (e.ColumnIndex == 21)
                //////                    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex, fMaxGap);
                //////                else
                //////                    m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex,
                //////                        float.Parse(((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString()));

                //////                ////Update template pitch gap setting
                //////                //m_smVisionInfo.g_arrPad[j].SetPitchGapDataFrom(i,
                //////                //   fMinPitch,
                //////                //   fMaxPitch,
                //////                //   fMinGap,
                //////                //   fMaxGap);

                //////                // 2019 02 28 - Update template pitch gap individual setting
                //////                if (e.ColumnIndex == 15)
                //////                    m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, fMinPitch);
                //////                else if (e.ColumnIndex == 17)
                //////                    m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, fMaxPitch);
                //////                else if (e.ColumnIndex == 19)
                //////                    m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, fMinGap);
                //////                else if (e.ColumnIndex == 21)
                //////                    m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, fMaxGap);
                //////            }

                //////            if (!objSetValueForm.ref_blnSetAllROI)
                //////                break;
                //////        }
                //////    }
                //////    if (!objSetValueForm.ref_blnSetAllRows)
                //////        break;
                //////}

                IsSettingError_CheckAllROI();

            }
            this.TopMost = true;
        }

        public bool IsSettingError()
        {
            int[] arrColumnIndex = { 3, 7, 11, 15, 19, 53, 57, 61, 65, 69, 73, 77, 81, 85, 89 };

            for (int c = 0; c < arrColumnIndex.Length; c++)
            {
                for (int i = 0; i < m_dgdView[0].Rows.Count; i++)
                {
                    if (m_dgdView[0].Rows[i].Cells[arrColumnIndex[c]].Value.ToString() != "---")
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
                    //m_smVisionInfo.g_arrPad[m_intPadIndex].SetPitchGapDataFrom(i,
                    //   fMinPitch,
                    //   fMaxPitch,
                    //   fMinGap,
                    //   fMaxGap);
                }
            }

            return false;
        }

        public bool IsSettingError_CheckAllROI()
        {
            bool[] arrError = { false, false, false, false, false};
            int[] arrColumnIndex = { 3, 7, 11, 15, 19, 53, 57, 61, 65, 69, 73, 77, 81, 85, 89 };

            for (int h = 0; h < m_smVisionInfo.g_arrPad.Length; h++)
            {
                if (h > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                List<List<string>> arrBlobsFeaturesData;
                if (m_smVisionInfo.g_arrPad[h].ref_blnWantUseGroupToleranceSetting)
                    arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[h].GetBlobsFeaturesData_PadAndGroup();
                else
                    arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[h].GetBlobsFeaturesInspectRealData2();

                for (int c = 0; c < arrColumnIndex.Length; c++)
                {
                    for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
                    {
                        if (c < 5)
                        {
                            if (arrBlobsFeaturesData[i][arrColumnIndex[c]] != "---")
                            {
                                //// Check insert data valid or not
                                //float fMin = float.Parse(arrBlobsFeaturesData[i][arrColumnIndex[c]]);
                                //float fMax = float.Parse(arrBlobsFeaturesData[i][arrColumnIndex[c] + 1]);
                                // 2020-03-06 ZJYEOH : changed arrColumnIndex[c] to formula arrColumnIndex[c] - (c * 2)
                                float fMin = float.Parse(arrBlobsFeaturesData[i][arrColumnIndex[c] - (c * 2)]);
                                float fMax = float.Parse(arrBlobsFeaturesData[i][arrColumnIndex[c] - (c * 2) + 1]);

                                if (fMin > fMax)
                                {
                                    arrError[h] = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //int index = arrBlobsFeaturesData[i].Count - 4 - (c * 2);
                            // 2020-03-06 ZJYEOH : changed the formula
                            int index = arrColumnIndex[c] / 2;
                            if (arrBlobsFeaturesData[i][index] != "---")
                            {
                                // Check insert data valid or not
                                float fMin = float.Parse(arrBlobsFeaturesData[i][index - 1]);//index
                                float fMax = float.Parse(arrBlobsFeaturesData[i][index]);

                                if (fMin > fMax)
                                {
                                    arrError[h] = true;
                                    break;
                                }
                            }
                        }
                        if (arrError[h])
                            break;
                    }
                }

                if (arrError[h])
                {
                    switch (h)
                    {
                        case 0:
                            radioBtn_Middle.ForeColor = Color.Red;
                            break;
                        case 1:
                            radioBtn_Up.ForeColor = Color.Red;
                            break;
                        case 2:
                            radioBtn_Right.ForeColor = Color.Red;
                            break;
                        case 3:
                            radioBtn_Down.ForeColor = Color.Red;
                            break;
                        case 4:
                            radioBtn_Left.ForeColor = Color.Red;
                            break;
                    }
                }
                else
                {
                    switch (h)
                    {
                        case 0:
                            radioBtn_Middle.ForeColor = Color.Black;
                            break;
                        case 1:
                            radioBtn_Up.ForeColor = Color.Black;
                            break;
                        case 2:
                            radioBtn_Right.ForeColor = Color.Black;
                            break;
                        case 3:
                            radioBtn_Down.ForeColor = Color.Black;
                            break;
                        case 4:
                            radioBtn_Left.ForeColor = Color.Black;
                            break;
                    }
                }
            }

            for (int k = 0; k < arrError.Length; k++)
            {
                if (arrError[k])
                    return true;
            }

            return false;
        }

        private void timer_PadResult_Tick(object sender, EventArgs e)
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

            if (m_smVisionInfo.PR_MN_UpdateSettingInfo)
            {
                ViewOrHideResultColumn(chk_DisplayResult.Checked);
                m_smVisionInfo.PR_MN_UpdateSettingInfo = false;
            }

            if (m_smVisionInfo.PR_TL_UpdateInfo)
            {
                m_smVisionInfo.PR_TL_UpdateInfo = false;

                UpdateScore(m_intPadIndex, m_dgdView[0]);
                m_blnInitDone = false;
                UpdateWholePadScore();
                m_blnInitDone = true;
            }

            if ((m_smVisionInfo.g_blnCheckPackage && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)) && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
            {
                if (txt_ExtraPadMinLength.Enabled)
                    txt_ExtraPadMinLength.Enabled = false;

                if (txt_ExtraPadMinArea.Enabled)
                    txt_ExtraPadMinArea.Enabled = false;

                if (txt_TotalExtraPadMinArea.Enabled)
                    txt_TotalExtraPadMinArea.Enabled = false;
            }
            else
            {
                if (!txt_ExtraPadMinLength.Enabled)
                    txt_ExtraPadMinLength.Enabled = true;

                if (!txt_ExtraPadMinArea.Enabled)
                    txt_ExtraPadMinArea.Enabled = true;

                if (!txt_TotalExtraPadMinArea.Enabled)
                    txt_TotalExtraPadMinArea.Enabled = true;
            }
        }

        private void chk_ViewGoldenColumn_Click(object sender, EventArgs e)
        {
            if (objGoldenUnitForm.ref_intViewGoldenDataColumn)
            {
                m_dgdView[0].Columns.Add(column_GolWidth);
                m_dgdView[0].Columns.Add(column_GolLength);
                m_dgdView[0].Columns.Add(column_GolPitch);
                m_dgdView[0].Columns.Add(column_GolGap);

                UpdateGoldenDataIntoGridTable(m_intPadIndex, m_dgdView[0]);
            }
            else
            {
                m_dgdView[0].Columns.Remove(column_GolLength);
                m_dgdView[0].Columns.Remove(column_GolWidth);
                m_dgdView[0].Columns.Remove(column_GolPitch);
                m_dgdView[0].Columns.Remove(column_GolGap);

                CheckSaveGoldenData(m_intPadIndex, m_dgdView[0]);
            }
        }

        private void dgd_MiddlePad_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone || m_blnUpdateSelectedROISetting)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 35 || e.ColumnIndex == 36 || e.ColumnIndex == 37 || e.ColumnIndex == 38 || e.ColumnIndex == 39 || e.ColumnIndex == 40 || e.ColumnIndex == 41 || e.ColumnIndex == 42)
            {
                m_smVisionInfo.g_blnViewPadSettingDrawing = true;
                m_smVisionInfo.g_blnViewPadEdgeLimitSetting = true;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

            if (e.ColumnIndex == 44 || e.ColumnIndex == 45 || e.ColumnIndex == 46 || e.ColumnIndex == 47 || e.ColumnIndex == 48 || e.ColumnIndex == 49 || e.ColumnIndex == 50 || e.ColumnIndex == 51)
            {
                m_smVisionInfo.g_blnViewPadSettingDrawing = true;
                m_smVisionInfo.g_blnViewPadStandOffSetting = true;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
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
            if (objGoldenUnitForm == null)
            {
                objGoldenUnitForm = new GoldenUnitCompensationForm(m_smCustomizeInfo,
                                    m_smVisionInfo, m_strSelectedRecipe, m_intUserGroup);
            }

            CheckSaveGoldenData(m_intPadIndex, m_dgdView[0]);

            objGoldenUnitForm.SetSelectedPadIndex(m_intPadIndex);

            objGoldenUnitForm.TopMost = true;
            objGoldenUnitForm.ShowDialog();

            SaveGoldenData();

            m_intGDSelectedIndex = objGoldenUnitForm.ref_intGoldenUnitSelectedIndex;

            if (m_intGDSelectedIndex >= 0 && objGoldenUnitForm.ref_intViewGoldenDataColumn)
            {
                if (m_dgdView[0].Columns.Count <= 92)
                {
                    m_dgdView[0].Columns.Add(column_GolWidth);
                    m_dgdView[0].Columns.Add(column_GolLength);
                    m_dgdView[0].Columns.Add(column_GolPitch);
                    m_dgdView[0].Columns.Add(column_GolGap);
                }
                UpdateGoldenDataIntoGridTable(m_intPadIndex, m_dgdView[0]);

                UpdateScore(m_intPadIndex, m_dgdView[0]);

                btn_SaveAccuracyReport.Visible = true;
            }
            else
            {
                if (m_dgdView[0].Columns.Count > 92)
                {
                    m_dgdView[0].Columns.Remove(column_GolLength);
                    m_dgdView[0].Columns.Remove(column_GolWidth);
                    m_dgdView[0].Columns.Remove(column_GolPitch);
                    m_dgdView[0].Columns.Remove(column_GolGap);
                }
                btn_SaveAccuracyReport.Visible = false;
            }
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
                arrData.Add("Total PadS: " + m_dgdView[0].Rows.Count.ToString());
                arrData.Add("");

                float fGoldenUnitValue = 0;
                float fVisionValue = 0;
                for (int i = 0; i < m_dgdView[0].Rows.Count; i++)
                {
                    arrData.Add("Pad " + (i + 1).ToString().PadRight(2, ' ') +
                        "\t\tGolden Unit    \t\tVision System  \t\tDeviation");

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[92].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[8].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Width \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[93].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[12].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Length\t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[94].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[16].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Pitch \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[95].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[20].Value.ToString(), out fVisionValue))
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

        private void chk_DisplayResult_Click(object sender, EventArgs e)
        {
            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("CurrentDisplayResult_PadTolerance", chk_DisplayResult.Checked);

        }

        private void txt_ExtraPadMinLength_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue;
            if (!float.TryParse(txt_ExtraPadMinLength.Text, out fValue))
                return;

            // 2019 02 15 - CCENG: Use radio button selection (m_intPadIndex), not image press roi selection (g_intSelectedROIMask)
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)   
            //        m_smVisionInfo.g_arrPad[i].SetExtraPadLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            //}

            m_smVisionInfo.g_arrPad[m_intPadIndex].SetExtraPadLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ExtraPadMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue;
            if (!float.TryParse(txt_ExtraPadMinArea.Text, out fValue))
                return;

            // 2019 02 15 - CCENG: Use radio button selection (m_intPadIndex), not image press roi selection (g_intSelectedROIMask)
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)   // 2019 02 15 - CCENG: Use radio button selection, no image press roi selection
            //    if (m_smVisionInfo.g_arrPad.Length == 1 || (i == m_intPadIndex))
            //        m_smVisionInfo.g_arrPad[i].SetExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            //}

            m_smVisionInfo.g_arrPad[m_intPadIndex].SetExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void tp_Center_Click(object sender, EventArgs e)
        {

        }

        private void dgd_MiddlePad_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex == 1 || e.ColumnIndex == 4 || e.ColumnIndex == 8 ||
                e.ColumnIndex == 12 || e.ColumnIndex == 16 || e.ColumnIndex == 20 ||
                e.ColumnIndex == 24 || e.ColumnIndex == 27 || e.ColumnIndex == 30 || e.ColumnIndex == 33 ||
                e.ColumnIndex == 36 || e.ColumnIndex == 38 || e.ColumnIndex == 40 || e.ColumnIndex == 42 ||
                e.ColumnIndex == 45 || e.ColumnIndex == 47 || e.ColumnIndex == 49 || e.ColumnIndex == 51 ||
                e.ColumnIndex == 54 || e.ColumnIndex == 58 || e.ColumnIndex == 62 || e.ColumnIndex == 66 || e.ColumnIndex == 70 ||
                e.ColumnIndex == 74 || e.ColumnIndex == 78 || e.ColumnIndex == 82 || e.ColumnIndex == 86 || e.ColumnIndex == 90)
                return;

            // Skip if col is separation
            if (e.ColumnIndex == 2 || e.ColumnIndex == 6 || e.ColumnIndex == 10 ||
                e.ColumnIndex == 14 || e.ColumnIndex == 18 || e.ColumnIndex == 22 ||
                e.ColumnIndex == 25 || e.ColumnIndex == 28 || e.ColumnIndex == 31 || e.ColumnIndex == 34 || e.ColumnIndex == 43 ||
                e.ColumnIndex == 52 || e.ColumnIndex == 56 || e.ColumnIndex == 60 || e.ColumnIndex == 64 || e.ColumnIndex == 68 ||
                e.ColumnIndex == 72 || e.ColumnIndex == 76 || e.ColumnIndex == 80 || e.ColumnIndex == 84 || e.ColumnIndex == 88)
                //|| e.ColumnIndex == 92 || e.ColumnIndex == 94 || e.ColumnIndex == 96 || e.ColumnIndex == 98)
                return;

            // Skip if col is golden unit data
            if (e.ColumnIndex == 92 || e.ColumnIndex == 93 || e.ColumnIndex == 94 || e.ColumnIndex == 95)
                return;

            //////// Skip for pad row setting change if Using Group Tolerance Setting
            //////if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
            //////{
            //////    if (((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value.ToString().IndexOf("Group") < 0)
            //////    {
            //////        return;
            //////    }
            //////}

            //Min, max area, broken area has 6 decimal places 
            if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
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

            SetValueForm objSetValueForm = new SetValueForm("Set value to all pads", m_strUnitLabel, e.RowIndex, m_intDecimalPlaces, strCurrentSetValue, true, false, false);
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

                int intStartIndex;
                int intEndIndex;
                if (objSetValueForm.ref_blnSetAllROI)
                {
                    intStartIndex = 1;
                    intEndIndex = m_smVisionInfo.g_arrPad.Length;
                }
                else
                {
                    intStartIndex = m_intPadIndex;
                    intEndIndex = m_intPadIndex + 1;
                }

                // ----------------- for selected pad ---------------------------------------------------

                float fMinPitch = 0, fMaxPitch = 0, fMinGap = 0, fMaxGap = 0;
                int intTotalRows;
                if (objSetValueForm.ref_blnSetAllRows)
                {
                    intTotalRows = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesNumber();

                    if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                    {
                        intTotalRows += m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intPadGroupCount;  // 2019 12 23 - CCENG: total pad rows + additional group rows.
                    }
                }
                else
                {
                    intTotalRows = intStartRowNumber + 1;
                }

                // Loop row index
                for (int i = intStartRowNumber; i < intTotalRows; i++)
                {
                    if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                    {
                        if (((DataGridView)sender).Rows[i].HeaderCell.Value.ToString().IndexOf("Group") < 0)
                        {
                            continue;
                        }
                    }

                    if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
                    {
                        // Check insert data valid or not
                        if (e.ColumnIndex == 3 || e.ColumnIndex == 7 || e.ColumnIndex == 11 || e.ColumnIndex == 15 || e.ColumnIndex == 19
                             || e.ColumnIndex == 53 || e.ColumnIndex == 57 || e.ColumnIndex == 61 || e.ColumnIndex == 65 || e.ColumnIndex == 69
                             || e.ColumnIndex == 73 || e.ColumnIndex == 77 || e.ColumnIndex == 81 || e.ColumnIndex == 85 || e.ColumnIndex == 89)
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
                        else if (e.ColumnIndex == 5 || e.ColumnIndex == 9 || e.ColumnIndex == 13 || e.ColumnIndex == 17 || e.ColumnIndex == 21
                            || e.ColumnIndex == 55 || e.ColumnIndex == 59 || e.ColumnIndex == 63 || e.ColumnIndex == 67 || e.ColumnIndex == 71
                            || e.ColumnIndex == 75 || e.ColumnIndex == 79 || e.ColumnIndex == 83 || e.ColumnIndex == 87 || e.ColumnIndex == 91)
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

                        // Set new insert value into table
                        if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F6");
                        else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                    }
                }

                // --------------------------------------------------------------------------------------

                // Loop Pad index
                for (int j = intStartIndex; j < intEndIndex; j++)
                {
                    intTotalRows = m_smVisionInfo.g_arrPad[j].GetBlobsFeaturesNumber();
                    if (m_smVisionInfo.g_arrPad[j].ref_blnWantUseGroupToleranceSetting)
                    {
                        intTotalRows = intTotalRows + m_smVisionInfo.g_arrPad[j].ref_intPadGroupCount;
                    }
                    else if (!objSetValueForm.ref_blnSetAllRows)
                    {
                        intTotalRows = intStartRowNumber + 1;
                    }
                    // Loop row index
                    for (int i = intStartRowNumber; i < intTotalRows; i++)
                    {
                        if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                        {
                            if (((DataGridView)sender).Rows[i].HeaderCell.Value.ToString().IndexOf("Group") < 0)
                            {
                                continue;
                            }
                        }

                        if (m_smVisionInfo.g_arrPad[j].ref_blnWantUseGroupToleranceSetting)
                        {
                            // 2019 03 01 - JBTAN: update group individual setting
                            int intGroupNoIndex = Convert.ToInt32(((DataGridView)sender).Rows[i].HeaderCell.Value.ToString().Substring(5)) - 1; // changed i to e.ROwIndex

                            m_smVisionInfo.g_arrPad[j].UpdateGroupBlobSingleFeatureToPixel(intGroupNoIndex, e.ColumnIndex,
                                objSetValueForm.ref_fSetValue);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPad[j].UpdateSingleBlobFeatureToPixel(i, e.ColumnIndex,
                                objSetValueForm.ref_fSetValue);

                            // 2019 02 28 - Update template pitch gap individual setting
                            if (e.ColumnIndex == 15)
                                m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, objSetValueForm.ref_fSetValue);
                            else if (e.ColumnIndex == 17)
                                m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, objSetValueForm.ref_fSetValue);
                            else if (e.ColumnIndex == 19)
                                m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, objSetValueForm.ref_fSetValue);
                            else if (e.ColumnIndex == 21)
                                m_smVisionInfo.g_arrPad[j].SetSinglePitchGapDataFrom(i, e.ColumnIndex, objSetValueForm.ref_fSetValue);
                        }
                    }
                }

                IsSettingError_CheckAllROI();

            }
            this.TopMost = true;
        }

        private void btn_UpdateTolerance_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            Point p = btn_UpdateTolerance.PointToScreen(Point.Empty);
            PadUpdateToleranceUpperLowerLimitForm objForm = new PadUpdateToleranceUpperLowerLimitForm(m_smVisionInfo, m_smCustomizeInfo, m_strSelectedRecipe, m_intUserGroup, m_smProductionInfo);
            objForm.StartPosition = FormStartPosition.Manual;
            int intMainFormLocationX = 0;
            int intMainFormLocationY = 0;
            objForm.Location = GetFormStartPoint(p, btn_UpdateTolerance.Size, objForm.Size, ref intMainFormLocationX, ref intMainFormLocationY); ;

            if (objForm.ShowDialog() == DialogResult.Yes)
            {
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                    return;

                int[] intResultColIndex = { 4, 8, 12, 16, 20, 54, 58, 62, 66, 70, 74, 78, 82, 86, 90 }; // Area, Width, Length, Pitch, Gap result

                for (int j = 0; j < m_smVisionInfo.g_arrPad.Length; j++)
                {
                    if (!objForm.ref_blnSetToCenterROI && (j == 0) && ((m_smCustomizeInfo.g_intWantPad5S & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                        continue;

                    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        continue;

                    if (!objForm.ref_blnSetToSideROI && (j > 0) && ((m_smCustomizeInfo.g_intWantPad5S & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                        continue;

                    // Update table
                    ReadPadAndGroupTemplateDataToGrid(j, m_dgdView[0]);
                    UpdateScore(j, m_dgdView[0]);

                    for (int i = 0; i < m_dgdView[0].Rows.Count; i++)
                    {
                        for (int c = 0; c < intResultColIndex.Length; c++)
                        {

                            if (m_dgdView[0].Rows[i].Cells[intResultColIndex[c]].Value != null && m_dgdView[0].Rows[i].Cells[intResultColIndex[c]].Value.ToString() != "---")
                            {
                                float fNorminalResult = float.Parse(m_dgdView[0].Rows[i].Cells[intResultColIndex[c]].Value.ToString());

                                switch (intResultColIndex[c])
                                {
                                    case 4:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_Area / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_Area / 100;
                                        }
                                        break;
                                    case 8:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 12:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 16:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_PitchGap / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_PitchGap / 100;
                                        }
                                        break;
                                    case 20:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_PitchGap / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_PitchGap / 100;
                                        }
                                        break;
                                    case 54:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 58:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 62:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 66:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 70:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 74:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 78:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 82:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 86:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                    case 90:
                                        {
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] - 1].Value = fNorminalResult * objForm.ref_fLowerLimit_WidthLength / 100;
                                            m_dgdView[0].Rows[i].Cells[intResultColIndex[c] + 1].Value = fNorminalResult * objForm.ref_fUpperLimit_WidthLength / 100;
                                        }
                                        break;
                                }
                            }
                        }

                        //Set value column selected
                        float fMinPitch, fMaxPitch, fMinGap, fMaxGap;
                        if (m_dgdView[0].Rows[i].Cells[15].Value.ToString() == "---")
                            fMinPitch = -1;
                        else
                            fMinPitch = float.Parse(m_dgdView[0].Rows[i].Cells[15].Value.ToString());

                        if (m_dgdView[0].Rows[i].Cells[17].Value.ToString() == "---")
                            fMaxPitch = -1;
                        else
                            fMaxPitch = float.Parse(m_dgdView[0].Rows[i].Cells[17].Value.ToString());

                        if (m_dgdView[0].Rows[i].Cells[19].Value.ToString() == "---")
                            fMinGap = -1;
                        else
                            fMinGap = float.Parse(m_dgdView[0].Rows[i].Cells[19].Value.ToString());

                        if (m_dgdView[0].Rows[i].Cells[21].Value.ToString() == "---")
                            fMaxGap = -1;
                        else
                            fMaxGap = float.Parse(m_dgdView[0].Rows[i].Cells[21].Value.ToString());

                        int intLengthMode = m_smVisionInfo.g_arrPad[j].GetSampleLengthMode(i);

                        //Update template setting
                        //if (intLengthMode == 1)
                        {
                            m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureToPixel(i,
                                float.Parse(m_dgdView[0].Rows[i].Cells[0].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[3].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[5].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[7].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[9].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[11].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[13].Value.ToString()),
                                fMinPitch,
                                fMaxPitch,
                                fMinGap,
                                fMaxGap,
                                float.Parse(m_dgdView[0].Rows[i].Cells[23].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[26].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[29].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[32].Value.ToString()),
                                // Edge Limit Setting
                                float.Parse(m_dgdView[0].Rows[i].Cells[35].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[37].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[39].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[41].Value.ToString()),
                                // Stand Off Setting
                                float.Parse(m_dgdView[0].Rows[i].Cells[44].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[46].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[48].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[50].Value.ToString()),
                                 // Dim Setting
                                float.Parse(m_dgdView[0].Rows[i].Cells[53].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[55].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[57].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[59].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[61].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[63].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[65].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[67].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[69].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[71].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[73].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[75].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[77].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[79].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[81].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[83].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[85].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[87].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[89].Value.ToString()),
                                float.Parse(m_dgdView[0].Rows[i].Cells[91].Value.ToString())
                                //,
                                //float.Parse(m_dgdView[0].Rows[i].Cells[73].Value.ToString()),
                                //float.Parse(m_dgdView[0].Rows[i].Cells[75].Value.ToString()),
                                //float.Parse(m_dgdView[0].Rows[i].Cells[77].Value.ToString()),
                                //float.Parse(m_dgdView[0].Rows[i].Cells[79].Value.ToString())
                                );
                        }
                        //else
                        //{
                        //    m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureToPixel(i,
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[0].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[3].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[5].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[11].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[13].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[7].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[9].Value.ToString()),
                        //        fMinPitch,
                        //        fMaxPitch,
                        //        fMinGap,
                        //        fMaxGap,
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[23].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[26].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[29].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[32].Value.ToString()),
                        //         float.Parse(m_dgdView[0].Rows[i].Cells[35].Value.ToString()),
                        //          float.Parse(m_dgdView[0].Rows[i].Cells[37].Value.ToString()),
                        //           float.Parse(m_dgdView[0].Rows[i].Cells[39].Value.ToString()),
                        //            float.Parse(m_dgdView[0].Rows[i].Cells[41].Value.ToString()),
                        //             float.Parse(m_dgdView[0].Rows[i].Cells[43].Value.ToString()),
                        //              float.Parse(m_dgdView[0].Rows[i].Cells[45].Value.ToString()),
                        //               float.Parse(m_dgdView[0].Rows[i].Cells[47].Value.ToString()),
                        //                float.Parse(m_dgdView[0].Rows[i].Cells[49].Value.ToString()),
                        //                 float.Parse(m_dgdView[0].Rows[i].Cells[51].Value.ToString()),
                        //                  float.Parse(m_dgdView[0].Rows[i].Cells[53].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[55].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[57].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[59].Value.ToString()),
                        //        float.Parse(m_dgdView[0].Rows[i].Cells[61].Value.ToString())
                        //        );
                        //}

                        //Update template pitch gap setting
                        m_smVisionInfo.g_arrPad[j].SetPitchGapDataFrom(i,
                           fMinPitch,
                           fMaxPitch,
                           fMinGap,
                           fMaxGap);
                    }
                }

                // Update table
                ReadPadAndGroupTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);
                UpdateScore(m_intPadIndex, m_dgdView[0]);
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

        private void txt_TotalExtraPadMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue;
            if (!float.TryParse(txt_TotalExtraPadMinArea.Text, out fValue))
                return;

            // 2019 02 15 - CCENG: Use radio button selection (m_intPadIndex), not image press roi selection (g_intSelectedROIMask)
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)   // 2019 02 15 - CCENG: Use radio button selection, no image press roi selection
            //    if (m_smVisionInfo.g_arrPad.Length == 1 || (i == m_intPadIndex))
            //        m_smVisionInfo.g_arrPad[i].SetTotalExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            //}

            m_smVisionInfo.g_arrPad[m_intPadIndex].SetTotalExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SaveTolToFile_Click(object sender, EventArgs e)
        {
            if (dlg_SaveToleranceFile.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        continue;

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


                    //STDeviceEdit.CopySettingFile(dlg_SaveToleranceFile.FileName, "");
                    // 2022 01 17 - CCENG: if WantLoadRefTolWhenNewLot is false, then save pad tolerance only which not affecting previous software version. 
                    if (m_smVisionInfo.g_blnWantLoadRefTolWhenNewLot)
                    {
                        if (m_smVisionInfo.g_blnWantLoadPadRefTol)
                            m_smVisionInfo.g_arrPad[i].SavePadToleranceToFile(dlg_SaveToleranceFile.FileName, false, strSectionName, true);

                        if (m_smVisionInfo.g_blnWantLoadPadPackageRefTol)
                            m_smVisionInfo.g_arrPad[i].SavePackageToleranceToFile(dlg_SaveToleranceFile.FileName, false, strSectionName, true);

                        if (m_smVisionInfo.g_blnWantLoadOtherRefTol)
                        {
                            if (m_smVisionInfo.g_arrPin1 != null && m_smVisionInfo.g_arrPin1.Count > 0)
                                m_smVisionInfo.g_arrPin1[0].SavePin1ToleranceToFile(dlg_SaveToleranceFile.FileName, false, true);

                            if (m_smVisionInfo.g_arrPad.Length > 0)
                                m_smVisionInfo.g_arrPad[0].SaveUnitPositionToleranceToFile(dlg_SaveToleranceFile.FileName, false, "CenterROI", true);

                            if (m_smVisionInfo.g_objPositioning != null)
                                m_smVisionInfo.g_objPositioning.SavePositionToleranceToFile(dlg_SaveToleranceFile.FileName, false, true);
                        }
                    }
                    else
                        m_smVisionInfo.g_arrPad[i].SavePadToleranceToFile(dlg_SaveToleranceFile.FileName, false, strSectionName, true);
                    //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Tolerance Setting", m_smProductionInfo.g_strLotID);

                }
            }

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void dlg_SaveToleranceFile_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btn_LoadTolFromFile_Click(object sender, EventArgs e)
        {
            if (dlg_LoadToleranceFile.ShowDialog() == DialogResult.OK)
            {
                string strFileName = dlg_LoadToleranceFile.FileName;

                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        continue;

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

                    m_smVisionInfo.g_arrPad[i].LoadPadToleranceFromFile(dlg_LoadToleranceFile.FileName, strSectionName);

                    // Update table
                    ReadPadAndGroupTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);
                    UpdateScore(m_intPadIndex, m_dgdView[0]);
                    //txt_ExtraPadMinLength.Text = m_smVisionInfo.g_arrPad[m_intPadIndex].GetExtraPadLengthLimit(1).ToString("F" + m_intDecimal);
                    //txt_ExtraPadMinArea.Text = m_smVisionInfo.g_arrPad[m_intPadIndex].GetExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                    //txt_TotalExtraPadMinArea.Text = m_smVisionInfo.g_arrPad[m_intPadIndex].GetTotalExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                    m_blnInitDone = false;
                    UpdateWholdPadsSettingGUI();
                    UpdateWholePadScore();
                    m_blnInitDone = true;
                }
            }

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void btn_SelectUseToleranceType_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            Point p = btn_SelectUseToleranceType.PointToScreen(Point.Empty);
            PadGroupSettingForm objPadGroupSettingForm = new PadGroupSettingForm(m_smVisionInfo, m_smCustomizeInfo, m_strSelectedRecipe, m_intUserGroup);
            objPadGroupSettingForm.StartPosition = FormStartPosition.Manual;
            int intMainFormLocationX = 0;
            int intMainFormLocationY = 0;
            objPadGroupSettingForm.Location = GetFormStartPoint(p, btn_SelectUseToleranceType.Size, objPadGroupSettingForm.Size, ref intMainFormLocationX, ref intMainFormLocationY); ;
            objPadGroupSettingForm.ShowDialog();
            
            ReadPadAndGroupTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

            UpdateScore(m_intPadIndex, m_dgdView[0]);

            this.TopMost = true;
        }

        private void ContaminationControlVisible(bool blnVisible)
        {
            if (blnVisible)
            {
                //lbl_UnitDisplay1.Visible = blnVisible;

                // Length
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckExtraPadLength)
                {
                    lbl_PinScoreTitle.Visible = blnVisible;
                    txt_ExtraPadMinLength.Visible = blnVisible;
                    lbl_UnitSqDisplay2.Visible = blnVisible;
                }
                else
                {
                    lbl_PinScoreTitle.Visible = !blnVisible;
                    txt_ExtraPadMinLength.Visible = !blnVisible;
                    lbl_UnitSqDisplay2.Visible = !blnVisible;
                }

                // Area
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckExtraPadArea)
                {
                    srmLabel1.Visible = blnVisible;
                    txt_ExtraPadMinArea.Visible = blnVisible;
                    lbl_UnitSqDisplay1.Visible = blnVisible;
                }
                else
                {
                    srmLabel1.Visible = !blnVisible;
                    txt_ExtraPadMinArea.Visible = !blnVisible;
                    lbl_UnitSqDisplay1.Visible = !blnVisible;
                }

                // Total Area
                if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intFailOptionMask & 0x1000) > 0)
                {
                    srmLabel3.Visible = blnVisible;
                    txt_TotalExtraPadMinArea.Visible = blnVisible;
                    lbl_UnitSqDisplay3.Visible = blnVisible;
                }
                else
                {
                    srmLabel3.Visible = !blnVisible;
                    txt_TotalExtraPadMinArea.Visible = !blnVisible;
                    lbl_UnitSqDisplay3.Visible = !blnVisible;
                }
            }
            else
            {
                //lbl_UnitDisplay1.Visible = blnVisible;

                // Length
                lbl_PinScoreTitle.Visible = blnVisible; 
                txt_ExtraPadMinLength.Visible = blnVisible;
                lbl_UnitSqDisplay2.Visible = blnVisible;

                // Area
                srmLabel1.Visible = blnVisible; 
                txt_ExtraPadMinArea.Visible = blnVisible;
                lbl_UnitSqDisplay1.Visible = blnVisible;

                // Total Area
                srmLabel3.Visible = blnVisible; 
                txt_TotalExtraPadMinArea.Visible = blnVisible;
                lbl_UnitSqDisplay3.Visible = blnVisible;
            }
        }

        private void dgd_MiddlePad_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            if (e.ColumnIndex == 35 || e.ColumnIndex == 36 || e.ColumnIndex == 37 || e.ColumnIndex == 38 || e.ColumnIndex == 39 || e.ColumnIndex == 40 || e.ColumnIndex == 41 || e.ColumnIndex == 42)
            {
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                {
                    if (dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().Contains("Group"))
                    {
                        m_smVisionInfo.g_blnViewPadGroupSetting = true;
                        m_smVisionInfo.g_intSelectedBlobNo = Convert.ToInt32(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().Substring(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().IndexOf("Group") + 5)) - 1;
                    }
                    else if (dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().Contains("Pad"))
                    {
                        m_smVisionInfo.g_blnViewPadGroupSetting = false;
                        m_smVisionInfo.g_intSelectedBlobNo = Convert.ToInt32(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().Substring(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().IndexOf("Pad") + 3)) - 1;
                    }
                }
                else
                {
                    m_smVisionInfo.g_blnViewPadGroupSetting = false;
                    m_smVisionInfo.g_intSelectedBlobNo = e.RowIndex;
                }
                m_smVisionInfo.g_blnViewPadSettingDrawing = true;
                //m_smVisionInfo.g_intSelectedBlobNo = e.RowIndex;
                m_smVisionInfo.g_blnViewPadEdgeLimitSetting = true;
                m_smVisionInfo.g_blnViewPadStandOffSetting = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            else if (e.ColumnIndex == 44 || e.ColumnIndex == 45 || e.ColumnIndex == 46 || e.ColumnIndex == 47 || e.ColumnIndex == 48 || e.ColumnIndex == 49 || e.ColumnIndex == 50 || e.ColumnIndex == 51)
            {
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                {
                    if (dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().Contains("Group"))
                    {
                        m_smVisionInfo.g_blnViewPadGroupSetting = true;
                        m_smVisionInfo.g_intSelectedBlobNo = Convert.ToInt32(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().Substring(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().IndexOf("Group") + 5)) - 1; 
                    }
                    else if(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().Contains("Pad"))
                    {
                        m_smVisionInfo.g_blnViewPadGroupSetting = false;
                        m_smVisionInfo.g_intSelectedBlobNo = Convert.ToInt32(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().Substring(dgd_MiddlePad.Rows[e.RowIndex].HeaderCell.Value.ToString().IndexOf("Pad") + 3)) - 1; 
                    }
                }
                else
                {
                    m_smVisionInfo.g_blnViewPadGroupSetting = false;
                    m_smVisionInfo.g_intSelectedBlobNo = e.RowIndex;
                }
                m_smVisionInfo.g_blnViewPadSettingDrawing = true;
                //m_smVisionInfo.g_intSelectedBlobNo = e.RowIndex;
                m_smVisionInfo.g_blnViewPadStandOffSetting = true;
                m_smVisionInfo.g_blnViewPadEdgeLimitSetting = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            else
            {
                m_smVisionInfo.g_blnViewPadSettingDrawing = false;
                m_smVisionInfo.g_intSelectedBlobNo = -1;
                m_smVisionInfo.g_blnViewPadEdgeLimitSetting = false;
                m_smVisionInfo.g_blnViewPadStandOffSetting = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void btn_OffsetSetting_Click(object sender, EventArgs e)
        {
            PadOffsetSettingForm objForm = new PadOffsetSettingForm(m_smCustomizeInfo, m_smVisionInfo,
                      m_strSelectedRecipe, m_intUserGroup, m_smProductionInfo);
            Rectangle objScreenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            objForm.StartPosition = FormStartPosition.Manual;
            objForm.Location = new Point(objScreenRect.Width - objForm.Width - 10,
                objScreenRect.Height - objForm.Height - 10);
            objForm.TopMost = true;
            objForm.ShowDialog();
            //if (objForm.ShowDialog() == DialogResult.OK)
            //{
            //    switch (m_intPadIndex)
            //    {
            //        case 0:
            //            radioBtn_Middle.PerformClick();
            //            break;
            //        case 1:
            //            radioBtn_Up.PerformClick();
            //            break;
            //        case 2:
            //            radioBtn_Right.PerformClick();
            //            break;
            //        case 3:
            //            radioBtn_Down.PerformClick();
            //            break;
            //        case 4:
            //            radioBtn_Left.PerformClick();
            //            break;
            //    }
            //}

        }

        private void UpdateHeaderCell(bool blnViewGroupNo)
        {
            //if (!blnViewGroupNo)
            //    return;

            List<List<string>> arrBlobsFeaturesData;
            arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesGroupNo();
            
            for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
            {
                if (blnViewGroupNo)
                {
                    if (m_dgdView[0].RowCount > i)
                        m_dgdView[0].Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0];
                }
                else
                {
                    if (m_dgdView[0].RowCount > i)
                        m_dgdView[0].Rows[i].HeaderCell.Value = "Pad " + (i + 1);
                }
            }
        }

        private void chk_DisplayGroupNo_Click(object sender, EventArgs e)
        {
            UpdateHeaderCell(chk_DisplayGroupNo.Checked);
            
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("CurrentDisplayGroupNo", chk_DisplayGroupNo.Checked);

        }
        private void PreUpdateWholePadSettingTable()
        {
            dgd_WholePadSetting.Rows.Clear();

            int intFailMask = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intFailOptionMask;

            int i = 0;

            if ((intFailMask & 0x01) > 0)
            {
                if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckExtraPadLength)
                {
                    dgd_WholePadSetting.Rows.Add();
                    dgd_WholePadSetting.Rows[i].Cells[0].Value = "F.Material / Cont. (Length)";
                    dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholePadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholePadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholePadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm";
                    i++;
                }

                if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckExtraPadArea)
                {
                    dgd_WholePadSetting.Rows.Add();
                    dgd_WholePadSetting.Rows[i].Cells[0].Value = "F.Material / Cont. (Area)";
                    dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                    dgd_WholePadSetting.Rows[i].Cells[1].ReadOnly = true;
                    dgd_WholePadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                    dgd_WholePadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                    dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                    dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                    dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm2";
                    i++;
                }
            }

            if ((m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intFailOptionMask & 0x1000) > 0)
            {
                dgd_WholePadSetting.Rows.Add();
                dgd_WholePadSetting.Rows[i].Cells[0].Value = "F.Material / Cont. (Total Area)";
                dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[1].ReadOnly = true;
                dgd_WholePadSetting.Rows[i].Cells[1].Style.BackColor = Color.Gray;
                dgd_WholePadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.Gray;
                dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm2";
                i++;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantEdgeDistance_Pad && m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadEdgeDistance && (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intFailOptionMask & 0x10000) > 0)
            {
                dgd_WholePadSetting.Rows.Add();
                dgd_WholePadSetting.Rows[i].Cells[0].Value = "Pad Edge Distance(Top)";
                dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm";
                i++;

                dgd_WholePadSetting.Rows.Add();
                dgd_WholePadSetting.Rows[i].Cells[0].Value = "Pad Edge Distance(Right)";
                dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm";
                i++;

                dgd_WholePadSetting.Rows.Add();
                dgd_WholePadSetting.Rows[i].Cells[0].Value = "Pad Edge Distance(Bottom)";
                dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm";
                i++;

                dgd_WholePadSetting.Rows.Add();
                dgd_WholePadSetting.Rows[i].Cells[0].Value = "Pad Edge Distance(Left)";
                dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm";
                i++;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantSpan_Pad && m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadSpanX && (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intFailOptionMask & 0x20000) > 0)
            {
                dgd_WholePadSetting.Rows.Add();
                dgd_WholePadSetting.Rows[i].Cells[0].Value = "Pad Span X";
                dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm";
                i++;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantSpan_Pad && m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantCheckPadSpanY && (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intFailOptionMask & 0x40000) > 0)
            {
                dgd_WholePadSetting.Rows.Add();
                dgd_WholePadSetting.Rows[i].Cells[0].Value = "Pad Span Y";
                dgd_WholePadSetting.Rows[i].Cells[1].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[2].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[3].Value = "0";
                dgd_WholePadSetting.Rows[i].Cells[4].Value = "mm";
                i++;
            }
        }
        private void UpdateWholdPadsSettingGUI()
        {
            for (int i = 0; i < dgd_WholePadSetting.Rows.Count; i++)
            {
                switch (dgd_WholePadSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Pad Span X":
                        dgd_WholePadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetMinSpanX(1).ToString("F" + m_intDecimal);
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetMaxSpanX(1).ToString("F" + m_intDecimal);
                        break;
                    case "Pad Span Y":
                        dgd_WholePadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetMinSpanY(1).ToString("F" + m_intDecimal);
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetMaxSpanY(1).ToString("F" + m_intDecimal);
                        break;
                    case "Pad Edge Distance(Top)":
                        dgd_WholePadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetPadEdgeDistanceMin_Top(1).ToString("F" + m_intDecimal);
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetPadEdgeDistanceMax_Top(1).ToString("F" + m_intDecimal);
                        break;
                    case "Pad Edge Distance(Right)":
                        dgd_WholePadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetPadEdgeDistanceMin_Right(1).ToString("F" + m_intDecimal);
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetPadEdgeDistanceMax_Right(1).ToString("F" + m_intDecimal);
                        break;
                    case "Pad Edge Distance(Bottom)":
                        dgd_WholePadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetPadEdgeDistanceMin_Bottom(1).ToString("F" + m_intDecimal);
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetPadEdgeDistanceMax_Bottom(1).ToString("F" + m_intDecimal);
                        break;
                    case "Pad Edge Distance(Left)":
                        dgd_WholePadSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetPadEdgeDistanceMin_Left(1).ToString("F" + m_intDecimal);
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetPadEdgeDistanceMax_Left(1).ToString("F" + m_intDecimal);
                        break;
                    case "F.Material / Cont. (Length)":
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetExtraPadLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case "F.Material / Cont. (Area)":
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                        break;
                    case "F.Material / Cont. (Total Area)":
                        dgd_WholePadSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].GetTotalExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                        break;

                }
            }
        }
        private void UpdateWholePadScore()
        {
            for (int i = 0; i < dgd_WholePadSetting.Rows.Count; i++)
            {
                switch (dgd_WholePadSetting.Rows[i].Cells[0].Value.ToString())
                {
                    case "Pad Span X":
                        if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_fResultPadSpanX == -999)
                            dgd_WholePadSetting.Rows[i].Cells[3].Value = "---";
                        else
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_fResultPadSpanX.ToString("F" + m_intDecimal);
                            if (Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) > Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value) ||
                                Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Pad Span Y":
                        if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_fResultPadSpanY == -999)
                            dgd_WholePadSetting.Rows[i].Cells[3].Value = "---";
                        else
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_fResultPadSpanY.ToString("F" + m_intDecimal);
                            if (Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) > Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value) ||
                                Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value))
                            {
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            }
                            else
                            {
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                                dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            }
                        }
                        break;
                    case "Pad Edge Distance(Top)":
                        dgd_WholePadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_fResultPadEdgeDistance_Top.ToString("F" + m_intDecimal);
                        if (Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) > Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value) ||
                            Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Pad Edge Distance(Right)":
                        dgd_WholePadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_fResultPadEdgeDistance_Right.ToString("F" + m_intDecimal);
                        if (Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) > Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value) ||
                          Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Pad Edge Distance(Bottom)":
                        dgd_WholePadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_fResultPadEdgeDistance_Bottom.ToString("F" + m_intDecimal);
                        if (Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) > Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value) ||
                          Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "Pad Edge Distance(Left)":
                        dgd_WholePadSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_fResultPadEdgeDistance_Left.ToString("F" + m_intDecimal);
                        if (Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) > Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value) ||
                          Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[3].Value) < Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value))
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Red;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Yellow;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        }
                        else
                        {
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Lime;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Black;
                            dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        }
                        break;
                    case "F.Material / Cont. (Length)":
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        break;
                    case "F.Material / Cont. (Area)":
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        break;
                    case "F.Material / Cont. (Total Area)":
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.BackColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.ForeColor = Color.Gray;
                        dgd_WholePadSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Gray;
                        break;
                }
            }
        }
        private void dgd_WholePadSetting_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.ColumnIndex != 1 && e.ColumnIndex != 2)
                return;

            float fValue = 0;
            if (!float.TryParse(dgd_WholePadSetting.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                return;

            int i = e.RowIndex;
            switch (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString())
            {
                case "Pad Span X":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetMinSpanX(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetMaxSpanX(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
                case "Pad Span Y":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetMinSpanY(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetMaxSpanY(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
                case "Pad Edge Distance(Top)":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPadEdgeDistanceMin_Top(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPadEdgeDistanceMax_Top(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
                case "Pad Edge Distance(Right)":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPadEdgeDistanceMin_Right(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPadEdgeDistanceMax_Right(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
                case "Pad Edge Distance(Bottom)":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPadEdgeDistanceMin_Bottom(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPadEdgeDistanceMax_Bottom(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
                case "Pad Edge Distance(Left)":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPadEdgeDistanceMin_Left(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[1].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPadEdgeDistanceMax_Left(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
                case "F.Material / Cont. (Length)":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetExtraPadLengthLimit(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
                case "F.Material / Cont. (Area)":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetExtraPadMinArea(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
                case "F.Material / Cont. (Total Area)":
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetTotalExtraPadMinArea(Convert.ToSingle(dgd_WholePadSetting.Rows[i].Cells[2].Value), m_smCustomizeInfo.g_intUnitDisplay);
                    break;
            }
        }
    }
}