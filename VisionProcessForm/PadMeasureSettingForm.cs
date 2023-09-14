using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class PadMeasureSettingForm : Form
    {
        #region Member Variables

        private bool m_blnFormOpen = false;
        private bool m_blnInitDone = false;
        private bool m_blnUpdateInfo = false;
        private int m_intUserGroup = 5;
        private int m_intPadIndex = 0;
        private int m_intGDSelectedIndex = 0;   // Golden Data Set Selected index
        private int m_intDecimalPlaces = 4;
        private string m_strSelectedRecipe;
        private string m_strUnitLabel = "mm";

        private DataGridView[] m_dgdView = new DataGridView[5];
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private GoldenUnitCompensationForm objGoldenUnitForm;
        #endregion


        #region Properties

        public bool ref_blnFormOpen { get { return m_blnFormOpen; } set { m_blnFormOpen = value; } }
        public bool ref_blnUpdateInfo { get { return m_blnUpdateInfo; } set { m_blnUpdateInfo = value; } }

        #endregion

        public PadMeasureSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;

            m_dgdView[0] = dgd_MiddlePad;
            m_dgdView[1] = dgd_TopPad;
            m_dgdView[2] = dgd_RightPad;
            m_dgdView[3] = dgd_BottomPad;
            m_dgdView[4] = dgd_LeftPad;

            LoadGoldenData();
            UpdateGUI();

            m_blnInitDone = true;
        }


        private void UpdateGUI()
        {
            radioBtn_Middle.Checked = true;

            ReadPadTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

            UpdateScore(m_intPadIndex, m_dgdView[0]);

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

            if (m_smVisionInfo.g_arrPad.Length == 1)
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
                    if (dgd_PadSetting.Rows[r].Cells.Count <= 25)
                        continue;

                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 0)
                        dgd_PadSetting.Rows[r].Cells[25].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][0];
                    else
                        dgd_PadSetting.Rows[r].Cells[25].Value = 0;

                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 1)
                        dgd_PadSetting.Rows[r].Cells[26].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][1];
                    else
                        dgd_PadSetting.Rows[r].Cells[26].Value = 0;

                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 2)
                        dgd_PadSetting.Rows[r].Cells[27].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][2];
                    else
                        dgd_PadSetting.Rows[r].Cells[27].Value = 0;

                    if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 3)
                        dgd_PadSetting.Rows[r].Cells[28].Value = m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][3];
                    else
                        dgd_PadSetting.Rows[r].Cells[28].Value = 0;
                }
                else
                {
                    dgd_PadSetting.Rows[r].Cells[25].Value = 0;
                    dgd_PadSetting.Rows[r].Cells[26].Value = 0;
                    dgd_PadSetting.Rows[r].Cells[27].Value = 0;
                    dgd_PadSetting.Rows[r].Cells[28].Value = 0;
                }
            }
        }

        private void UpdateGridTableIntoGoldenData(int intPadIndex, DataGridView dgd_PadSetting)
        {
            m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Clear();

            for (int r = 0; r < dgd_PadSetting.Rows.Count; r++)
            {
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex].Add(new List<float>());
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[25].Value));
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[26].Value));
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[27].Value));
                m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Add(Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[28].Value));

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
                        if (Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[25].Value) != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][0])
                        {
                            blnIsDataChanged = true;
                            break;
                        }

                        if (Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[26].Value) != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][1])
                        {
                            blnIsDataChanged = true;
                            break;
                        }

                        if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 2)
                        {
                            if (Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[27].Value) != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][2])
                            {
                                blnIsDataChanged = true;
                                break;
                            }
                        }

                        if (m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r].Count > 3)
                        {
                            if (Convert.ToSingle(dgd_PadSetting.Rows[r].Cells[28].Value) != m_smVisionInfo.g_arrPad[intPadIndex].ref_arrGoldenData[m_intGDSelectedIndex][r][3])
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
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad Measure Settings", m_smProductionInfo.g_strLotID);
                
            }
            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;
        }

        private void UpdateScore(int intPadIndex, DataGridView dgd_PadSetting)
        {

            TrackLog objTL = new TrackLog();
            objTL.WriteLine("                 ");

            for (int i = 0; i < dgd_PadSetting.Rows.Count; i++)
            {
                string strBlobsFeatures = "---#---#---#---#---#---#---#---#";

                if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailResultMask & 0x1000) == 0)
                    strBlobsFeatures = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobFeaturesResult(i);

                string[] strFeature = strBlobsFeatures.Split('#');


                //objTL.WriteLine(strBlobsFeatures);



                int intFeatureIndex = 0;

                #region Update value to grid
                intFeatureIndex++;

                int[] intGridResultColumnIndex = { 1, 4, 8, 12, 16, 20 };
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
                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[0].Value.ToString());
                    if (fResultValue > fMaxValue)
                        dgd_PadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Red;
                    else
                        dgd_PadSetting.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                }

                // Area
                if (dgd_PadSetting.Rows[i].Cells[4].Value.ToString() != "---")
                {
                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[3].Value.ToString());
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[4].Value.ToString());
                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[5].Value.ToString());
                    if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                        dgd_PadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Red;
                    else
                        dgd_PadSetting.Rows[i].Cells[4].Style.ForeColor = Color.Black;
                }

                // Width
                if (dgd_PadSetting.Rows[i].Cells[8].Value.ToString() != "---")
                {
                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[7].Value.ToString());
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[8].Value.ToString());
                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[9].Value.ToString());
                    //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    //    dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Red;
                    //else
                    {
                        if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_PadSetting.Rows[i].Cells.Count < 25)
                        {
                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Red;
                            else
                                dgd_PadSetting.Rows[i].Cells[8].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            if (dgd_PadSetting.Rows[i].Cells.Count > 25)
                            {
                                if (dgd_PadSetting.Rows[i].Cells[25].Value != null)
                                {
                                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[25].Value.ToString()) - fAccuracySpec;
                                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[8].Value.ToString());
                                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[25].Value.ToString()) + fAccuracySpec;
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
                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[11].Value.ToString());
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[12].Value.ToString());
                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[13].Value.ToString());
                    //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    //    dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Red;
                    //else
                    {
                        if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_PadSetting.Rows[i].Cells.Count < 25)
                        {
                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Red;
                            else
                                dgd_PadSetting.Rows[i].Cells[12].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            if (dgd_PadSetting.Rows[i].Cells.Count > 25)
                            {
                                if (dgd_PadSetting.Rows[i].Cells[26].Value != null)
                                {
                                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[26].Value.ToString()) - fAccuracySpec;
                                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[12].Value.ToString());
                                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[26].Value.ToString()) + fAccuracySpec;
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
                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[15].Value.ToString());
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[16].Value.ToString());
                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[17].Value.ToString());
                    //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    //    dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Red;
                    //else
                    {
                        if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_PadSetting.Rows[i].Cells.Count < 25)
                        {
                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Red;
                            else
                                dgd_PadSetting.Rows[i].Cells[16].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            if (dgd_PadSetting.Rows[i].Cells.Count > 27 && (dgd_PadSetting.Rows[i].Cells[27].Value != null))
                            {
                                fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[27].Value.ToString()) - fAccuracySpec;
                                fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[16].Value.ToString());
                                fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[27].Value.ToString()) + fAccuracySpec;
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
                    fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[19].Value.ToString());
                    fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[20].Value.ToString());
                    fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[21].Value.ToString());
                    //if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                    //    dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Red;
                    //else
                    {
                        if (objGoldenUnitForm == null || !objGoldenUnitForm.ref_intViewGoldenDataColumn || dgd_PadSetting.Rows[i].Cells.Count < 25)
                        {
                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Red;
                            else
                                dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Black;
                        }
                        else if (dgd_PadSetting.Rows[i].Cells.Count > 28 && (dgd_PadSetting.Rows[i].Cells[28].Value != null))
                        {
                            fMinValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[28].Value.ToString()) - fAccuracySpec;
                            fResultValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[20].Value.ToString());
                            fMaxValue = Convert.ToSingle(dgd_PadSetting.Rows[i].Cells[28].Value.ToString()) + fAccuracySpec;
                            if ((fResultValue < fMinValue) || (fResultValue > fMaxValue))
                                dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.DarkOrange;
                            else
                                dgd_PadSetting.Rows[i].Cells[20].Style.ForeColor = Color.Black;
                        }

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
                dgd_PadSetting.Rows[i].Cells[24].Value = strFeature[intFeatureIndex++];
            }

            ColorGrid(dgd_PadSetting);
        }

        private void ColorGrid(DataGridView dgd_PadSetting)
        {
            for (int i = 0; i < dgd_PadSetting.Rows.Count; i++)
            {
                // OffSet
                dgd_PadSetting.Rows[i].Cells[0].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[1].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.LightGreen;

                // Area
                dgd_PadSetting.Rows[i].Cells[3].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[4].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[5].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[5].Style.SelectionBackColor = Color.LightGreen;

                // Width
                dgd_PadSetting.Rows[i].Cells[7].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[7].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[8].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[8].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[9].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[9].Style.SelectionBackColor = Color.LightGreen;

                // Length
                dgd_PadSetting.Rows[i].Cells[11].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[11].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[12].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[12].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[13].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[13].Style.SelectionBackColor = Color.LightGreen;

                // Pitch
                dgd_PadSetting.Rows[i].Cells[15].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[15].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[16].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[16].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[17].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[17].Style.SelectionBackColor = Color.LightGreen;

                // Gap
                dgd_PadSetting.Rows[i].Cells[19].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[19].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[20].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[20].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[21].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[21].Style.SelectionBackColor = Color.LightGreen;

                // Broken
                dgd_PadSetting.Rows[i].Cells[23].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[23].Style.SelectionBackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[24].Style.BackColor = Color.LightGreen;
                dgd_PadSetting.Rows[i].Cells[24].Style.SelectionBackColor = Color.LightGreen;
            }
        }

        private void UpdatePadMeasurementIntoTable()
        {

        }


        private void PadMeasureSettingForm_Load(object sender, EventArgs e)
        {
            m_blnFormOpen = true;
        }

        private void PadMeasureSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_blnFormOpen = false;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            // Save Pad Setting

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


            CheckSaveGoldenData(m_intPadIndex, m_dgdView[0]);
            // Load Pad Setting

            this.Close();
            this.Dispose();
        }

        private void radioBtn_PadIndex_Click(object sender, EventArgs e)
        {
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
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = 0;
            }
            else if (sender == radioBtn_Up)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = 1;
            }
            else if (sender == radioBtn_Right)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = 2;
            }
            else if (sender == radioBtn_Down)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = 3;
            }
            else if (sender == radioBtn_Left)
            {
                tabControl_Pad5S.SelectedIndex = m_intPadIndex = 4;
            }

            tabControl_Pad5S.SelectedIndex = 0;

            ReadPadTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

            if (objGoldenUnitForm != null)
            {
                if (objGoldenUnitForm.ref_intViewGoldenDataColumn)
                {
                    if (m_dgdView[0].Columns.Count <= 25)
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


        }

        private void dgd_MiddlePad_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            //Skip if col is result score
            if (e.ColumnIndex == 1 || e.ColumnIndex == 4 || e.ColumnIndex == 8 ||
                e.ColumnIndex == 12 || e.ColumnIndex == 16 || e.ColumnIndex == 20)
                return;

            // Skip if col is separation
            if (e.ColumnIndex == 2 || e.ColumnIndex == 6 || e.ColumnIndex == 10 ||
                e.ColumnIndex == 14 || e.ColumnIndex == 18 || e.ColumnIndex == 22)
                return;

            // Skip if col is golden unit data
            if (e.ColumnIndex == 25 || e.ColumnIndex == 26 || e.ColumnIndex == 27 || e.ColumnIndex == 28)
                return;

            //Skip if cell value is ---
            if ((((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "---") ||
                (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == ""))
                return;


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
            //SetValueForm objSetValueForm = new SetValueForm(e.RowIndex, intDecimalPlaces, strCurrentSetValue);
            SetValueForm objSetValueForm = new SetValueForm("Set value to pad " + (e.RowIndex + 1).ToString(), m_strUnitLabel, e.RowIndex, m_intDecimalPlaces, strCurrentSetValue);
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
                        if (e.ColumnIndex == 3 || e.ColumnIndex == 7 || e.ColumnIndex == 11 || e.ColumnIndex == 15 || e.ColumnIndex == 19)  // 3=
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
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.LightGreen;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.BackColor = Color.LightGreen;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex + 2].Style.SelectionBackColor = Color.LightGreen;
                            }
                        }
                        else if (e.ColumnIndex == 5 || e.ColumnIndex == 9 || e.ColumnIndex == 13 || e.ColumnIndex == 17 || e.ColumnIndex == 21)
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
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.LightGreen;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.BackColor = Color.LightGreen;
                                ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex - 2].Style.SelectionBackColor = Color.LightGreen;
                            }
                        }

                        // Set new insert value into table
                        if (e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 23)
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F6");
                        else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                    }

                    //Set value column selected
                    float fMinPitch, fMaxPitch, fMinGap, fMaxGap;
                    if (((DataGridView)sender).Rows[i].Cells[15].Value.ToString() == "---")
                        fMinPitch = -1;
                    else
                        fMinPitch = float.Parse(((DataGridView)sender).Rows[i].Cells[15].Value.ToString());

                    if (((DataGridView)sender).Rows[i].Cells[17].Value.ToString() == "---")
                        fMaxPitch = -1;
                    else
                        fMaxPitch = float.Parse(((DataGridView)sender).Rows[i].Cells[17].Value.ToString());

                    if (((DataGridView)sender).Rows[i].Cells[19].Value.ToString() == "---")
                        fMinGap = -1;
                    else
                        fMinGap = float.Parse(((DataGridView)sender).Rows[i].Cells[19].Value.ToString());

                    if (((DataGridView)sender).Rows[i].Cells[21].Value.ToString() == "---")
                        fMaxGap = -1;
                    else
                        fMaxGap = float.Parse(((DataGridView)sender).Rows[i].Cells[21].Value.ToString());

                    int intLengthMode = m_smVisionInfo.g_arrPad[m_intPadIndex].GetSampleLengthMode(i);

                    //Update template setting
                    if (intLengthMode == 1)
                    {
                        m_smVisionInfo.g_arrPad[m_intPadIndex].UpdateBlobFeatureToPixel(i,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[11].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[13].Value.ToString()),
                            fMinPitch,
                            fMaxPitch,
                            fMinGap,
                            fMaxGap,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[23].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[24].Value.ToString()),0,0,0,0,0,0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                            );
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPad[m_intPadIndex].UpdateBlobFeatureToPixel(i,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[11].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[13].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                            fMinPitch,
                            fMaxPitch,
                            fMinGap,
                            fMaxGap,
                            float.Parse(((DataGridView)sender).Rows[i].Cells[23].Value.ToString()),
                            float.Parse(((DataGridView)sender).Rows[i].Cells[24].Value.ToString()),0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                            );
                    }

                    //Update template pitch gap setting
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetPitchGapDataFrom(i,
                       fMinPitch,
                       fMaxPitch,
                       fMinGap,
                       fMaxGap);

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }

            this.TopMost = true;
        }

        public bool IsSettingError()
        {
            int[] arrColumnIndex = { 3, 7, 11, 15, 19 };

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

        private void timer_PadResult_Tick(object sender, EventArgs e)
        {
            if (m_blnUpdateInfo)
            {
                UpdateScore(m_intPadIndex, m_dgdView[0]);
                m_blnUpdateInfo = false;
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
                if (m_dgdView[0].Columns.Count <= 25)
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
                if (m_dgdView[0].Columns.Count > 25)
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

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[25].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[8].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Width \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[26].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[12].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Length\t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[27].Value.ToString(), out fGoldenUnitValue))
                        continue;

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[16].Value.ToString(), out fVisionValue))
                        continue;

                    arrData.Add("Pitch \t\t" +
                                fGoldenUnitValue.ToString().PadRight(15, ' ') + "\t\t" +
                                fVisionValue.ToString().PadRight(15, ' ') + "\t\t" +
                                (fVisionValue - fGoldenUnitValue).ToString("f4")
                                );

                    if (!float.TryParse(m_dgdView[0].Rows[i].Cells[28].Value.ToString(), out fGoldenUnitValue))
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
    }
}