using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class PadStandOffSettingForm : Form
    {
        #region Member Variables
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private bool m_blnUpdateInfo = false;
        private int m_intUserGroup = 5;
        private int m_intPadIndex = 0;
     
        private string m_strSelectedRecipe;
 
        private bool m_blnChangeScoreSetting = true;
        private List<int> m_arrPadRowIndex = new List<int>();
        private List<int> m_arrGroupRowIndex = new List<int>();
        
        private DataGridView[] m_dgdView = new DataGridView[5];
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion
        public PadStandOffSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;
            
            m_dgdView[0] = dgd_PadStandOffSetting;

            UpdateGUI();

            m_blnInitDone = true;
            m_blnUpdateInfo = true;

            //// 2020-01-06 ZJYEOH : Trigger Offline Test one time 
            //m_smVisionInfo.g_intViewInspectionSetting = 1;
            //m_smVisionInfo.AT_VM_OfflineTestAllPad = true;
            //TriggerOfflineTest();
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
        private void UpdateGUI()
        {
            radioBtn_Middle.Checked = true;

            ReadPadAndGroupTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);
            
            m_smVisionInfo.PR_TL_UpdateInfo = false;
            
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

            if (m_intPadIndex == 0)
                chk_SetToAllSideROI.Visible = false;
            else
                chk_SetToAllSideROI.Visible = true;

            //// Set this form size according to the max number of rows.
            //int intMaxRow = 0;
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (intMaxRow < m_smVisionInfo.g_arrPad[i].GetBlobsFeaturesNumber())
            //        intMaxRow = m_smVisionInfo.g_arrPad[i].GetBlobsFeaturesNumber();
            //}
            //if (intMaxRow > 21)
            //    intMaxRow = 21; // 20-06-2019 ZJYEOH : To avoid form size larger than screen size 
            //this.Size = new Size(this.Size.Width, 260 + 24 * intMaxRow);
            //tabControl_Pad5S.Size = new Size(this.Size.Width, 204 + 24 * intMaxRow);
            //dgd_MiddlePad.Size = new Size(this.Size.Width, 79 + 24 * intMaxRow);
        }
        private void ReadPadAndGroupTemplateDataToGrid(int intPadIndex, DataGridView dgd_PadSetting)
        {
            dgd_PadSetting.Rows.Clear();

            List<List<string>> arrBlobsFeaturesData;
            arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobsFeaturesStandOff();
            
            for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
            {
                dgd_PadSetting.Rows.Add();
                dgd_PadSetting.Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0] + " " + (Convert.ToInt32(arrBlobsFeaturesData[i][1]) + 1).ToString();

                //Refer Top/Bottom
                dgd_PadSetting.Rows[i].Cells[0].Value = (dgd_PadSetting.Rows[i].Cells[0] as DataGridViewComboBoxCell).Items[Convert.ToInt32(arrBlobsFeaturesData[i][2])];
                // Check Top
                dgd_PadSetting.Rows[i].Cells[1].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][3]);
                // Check Bottom
                dgd_PadSetting.Rows[i].Cells[2].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][4]);

                // Refer Left/Right
                dgd_PadSetting.Rows[i].Cells[4].Value = (dgd_PadSetting.Rows[i].Cells[4] as DataGridViewComboBoxCell).Items[Convert.ToInt32(arrBlobsFeaturesData[i][5])];
                // Check Left
                dgd_PadSetting.Rows[i].Cells[5].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][6]);
                // Check Right
                dgd_PadSetting.Rows[i].Cells[6].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][7]);
                
                m_arrPadRowIndex.Add(i);
            }
            
        }
        private void UpdateDataToGrid(int intPadIndex, DataGridView dgd_PadSetting)
        {
        
            List<List<string>> arrBlobsFeaturesData;
            arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[intPadIndex].GetBlobsFeaturesStandOff();

            for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
            {
                dgd_PadSetting.Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0] + " " + (Convert.ToInt32(arrBlobsFeaturesData[i][1]) + 1).ToString();

                //Refer Top/Bottom
                dgd_PadSetting.Rows[i].Cells[0].Value = (dgd_PadSetting.Rows[i].Cells[0] as DataGridViewComboBoxCell).Items[Convert.ToInt32(arrBlobsFeaturesData[i][2])];
                // Check Top
                dgd_PadSetting.Rows[i].Cells[1].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][3]);
                // Check Bottom
                dgd_PadSetting.Rows[i].Cells[2].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][4]);

                // Refer Left/Right
                dgd_PadSetting.Rows[i].Cells[4].Value = (dgd_PadSetting.Rows[i].Cells[4] as DataGridViewComboBoxCell).Items[Convert.ToInt32(arrBlobsFeaturesData[i][5])];
                // Check Left
                dgd_PadSetting.Rows[i].Cells[5].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][6]);
                // Check Right
                dgd_PadSetting.Rows[i].Cells[6].Value = Convert.ToBoolean(arrBlobsFeaturesData[i][7]);
                
            }

        }
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
                objFile = new XmlParser(strPath + "Template\\Template.xml");
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
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad " + strSectionName + " Tolerance Settings", m_smProductionInfo.g_strLotID);
                
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

        private void PadStandOffSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void PadStandOffSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            // Save Pad Setting
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\";

            SavePadSetting(strPath + "Pad\\");
            
            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;
            
            this.Close();
            this.Dispose();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            // Load Pad Setting
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            LoadPadSetting(strFolderPath + "Pad\\");
            
            this.Close();
            this.Dispose();
        }
        private void radioBtn_PadIndex_Click(object sender, EventArgs e)
        {
            m_blnUpdateSelectedROISetting = true;
            
            if (sender == radioBtn_Middle)
            {
                 m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 0;
            }
            else if (sender == radioBtn_Up)
            {
                 m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 1;
            }
            else if (sender == radioBtn_Right)
            {
                 m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 2;
            }
            else if (sender == radioBtn_Down)
            {
                 m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 3;
            }
            else if (sender == radioBtn_Left)
            {
                 m_intPadIndex = m_smVisionInfo.g_intSelectedPadROIIndex = 4;
            }

            if (m_intPadIndex == 0)
                chk_SetToAllSideROI.Visible = false;
            else
                chk_SetToAllSideROI.Visible = true;

            ReadPadAndGroupTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);
            
            m_blnUpdateSelectedROISetting = false;
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_PadStandOffSetting_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;


            if (!m_blnUpdateInfo)
            {
                //UpdateDataToGrid(m_intPadIndex, m_dgdView[0]);
                //m_dgdView[0].Refresh();
                return;
            }

            if (m_blnUpdateSelectedROISetting)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            if (e.ColumnIndex == 3)
                return;

            m_blnUpdateInfo = false;
            bool boolSetAllROI;
            if (m_intPadIndex == 0)
                boolSetAllROI = false;
            else
                boolSetAllROI = true;

            int intStartRowNumber;
            if (chk_SetToAllRows.Checked)
            {
                intStartRowNumber = 0;
            }
            else
            {
                intStartRowNumber = e.RowIndex;
            }

            int intStartIndex;
            int intEndIndex;
            if (boolSetAllROI && chk_SetToAllSideROI.Checked)
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

            int intTotalRows;
            if (chk_SetToAllRows.Checked)
            {
                intTotalRows = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesNumber();
            }
            else
            {
                intTotalRows = intStartRowNumber + 1;
            }

            // Loop Pad index
            for (int j = intStartIndex; j < intEndIndex; j++)
            {
                intTotalRows = m_smVisionInfo.g_arrPad[j].GetBlobsFeaturesNumber();
                if (!chk_SetToAllRows.Checked)
                {
                    intTotalRows = intStartRowNumber + 1;
                }
                // Loop row index
                for (int i = intStartRowNumber; i < intTotalRows; i++)
                {

                    if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6)
                    {
                        bool blnValue = Convert.ToBoolean(m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                        ////m_dgdView[0].CurrentCell = m_dgdView[0].Rows[0].Cells[e.ColumnIndex];  // 2020 05 16- CCENG: need to reset to 0 rows first then only point to selected rows.
                        ////m_dgdView[0].CurrentCell = m_dgdView[0].Rows[i].Cells[e.ColumnIndex];
                        //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
                        //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
                        //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
                        m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureCheckStandOff(i, e.ColumnIndex, blnValue);
                    }
                    else if (e.ColumnIndex == 0 || e.ColumnIndex == 4)
                    {
                        int intValue = (m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell).Items.IndexOf(m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue);

                        //if (intValue == 0)
                        //    intValue = 1;
                        //else if (intValue == 1)
                        //    intValue = 0;

                        m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureStandOffReferDirection(i, e.ColumnIndex, intValue);
                    }
                }
            }

            UpdateDataToGrid(m_intPadIndex, m_dgdView[0]);
            m_dgdView[0].Refresh();
            m_blnUpdateInfo = true;
        }

        private void dgd_PadStandOffSetting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;

            //if (m_blnUpdateSelectedROISetting)
            //    return;

            //if (e.RowIndex < 0)
            //    return;

            //if (e.ColumnIndex < 0)
            //    return;

            //if (e.ColumnIndex == 3)
            //    return;

            //m_blnUpdateInfo = false;
            //bool boolSetAllROI;
            //if (m_intPadIndex == 0)
            //    boolSetAllROI = false;
            //else
            //    boolSetAllROI = true;

            //int intStartRowNumber;
            //if (chk_SetToAllRows.Checked)
            //{
            //    intStartRowNumber = 0;
            //}
            //else
            //{
            //    intStartRowNumber = e.RowIndex;
            //}

            //int intStartIndex;
            //int intEndIndex;
            //if (boolSetAllROI && chk_SetToAllSideROI.Checked)
            //{
            //    intStartIndex = 1;
            //    intEndIndex = m_smVisionInfo.g_arrPad.Length;
            //}
            //else
            //{
            //    intStartIndex = m_intPadIndex;
            //    intEndIndex = m_intPadIndex + 1;
            //}

            //// ----------------- for selected pad ---------------------------------------------------

            //int intTotalRows;
            //if (chk_SetToAllRows.Checked)
            //{
            //    intTotalRows = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesNumber();
            //}
            //else
            //{
            //    intTotalRows = intStartRowNumber + 1;
            //}

            //// Loop Pad index
            //for (int j = intStartIndex; j < intEndIndex; j++)
            //{
            //    intTotalRows = m_smVisionInfo.g_arrPad[j].GetBlobsFeaturesNumber();
            //    if (!chk_SetToAllRows.Checked)
            //    {
            //        intTotalRows = intStartRowNumber + 1;
            //    }
            //    // Loop row index
            //    for (int i = intStartRowNumber; i < intTotalRows; i++)
            //    {

            //        if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6)
            //        {
            //            bool blnValue = Convert.ToBoolean(m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            //            ////m_dgdView[0].CurrentCell = m_dgdView[0].Rows[0].Cells[e.ColumnIndex];  // 2020 05 16- CCENG: need to reset to 0 rows first then only point to selected rows.
            //            ////m_dgdView[0].CurrentCell = m_dgdView[0].Rows[i].Cells[e.ColumnIndex];
            //            //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
            //            //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
            //            //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
            //            m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureCheckStandOff(i, e.ColumnIndex, !blnValue);
            //        }
            //        else if (e.ColumnIndex == 0 || e.ColumnIndex == 4)
            //        {
            //            int intValue = (m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell).Items.IndexOf(m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

            //            if (intValue == 0)
            //                intValue = 1;
            //            else if (intValue == 1)
            //                intValue = 0;

            //            m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureStandOffReferDirection(i, e.ColumnIndex, intValue);
            //        }
            //    }
            //}

            //UpdateDataToGrid(m_intPadIndex, m_dgdView[0]);
            //m_dgdView[0].Refresh();
            //m_blnUpdateInfo = true;
        }

        private void dgd_PadStandOffSetting_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateDataToGrid(m_intPadIndex, m_dgdView[0]);
            m_dgdView[0].Refresh();
        }

        private void dgd_PadStandOffSetting_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;


            //if (!m_blnUpdateInfo)
            //{
            //    UpdateDataToGrid(m_intPadIndex, m_dgdView[0]);
            //    m_dgdView[0].Refresh();
            //    return;
            //}

            //if (m_blnUpdateSelectedROISetting)
            //    return;

            //if (e.RowIndex < 0)
            //    return;

            //if (e.ColumnIndex < 0)
            //    return;

            //if (e.ColumnIndex == 3)
            //    return;

            //m_blnUpdateInfo = false;
            //bool boolSetAllROI;
            //if (m_intPadIndex == 0)
            //    boolSetAllROI = false;
            //else
            //    boolSetAllROI = true;

            //int intStartRowNumber;
            //if (chk_SetToAllRows.Checked)
            //{
            //    intStartRowNumber = 0;
            //}
            //else
            //{
            //    intStartRowNumber = e.RowIndex;
            //}

            //int intStartIndex;
            //int intEndIndex;
            //if (boolSetAllROI && chk_SetToAllSideROI.Checked)
            //{
            //    intStartIndex = 1;
            //    intEndIndex = m_smVisionInfo.g_arrPad.Length;
            //}
            //else
            //{
            //    intStartIndex = m_intPadIndex;
            //    intEndIndex = m_intPadIndex + 1;
            //}

            //// ----------------- for selected pad ---------------------------------------------------

            //int intTotalRows;
            //if (chk_SetToAllRows.Checked)
            //{
            //    intTotalRows = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesNumber();
            //}
            //else
            //{
            //    intTotalRows = intStartRowNumber + 1;
            //}

            //// Loop Pad index
            //for (int j = intStartIndex; j < intEndIndex; j++)
            //{
            //    intTotalRows = m_smVisionInfo.g_arrPad[j].GetBlobsFeaturesNumber();
            //    if (!chk_SetToAllRows.Checked)
            //    {
            //        intTotalRows = intStartRowNumber + 1;
            //    }
            //    // Loop row index
            //    for (int i = intStartRowNumber; i < intTotalRows; i++)
            //    {

            //        if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 5 || e.ColumnIndex == 6)
            //        {
            //            bool blnValue = Convert.ToBoolean(m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            //            ////m_dgdView[0].CurrentCell = m_dgdView[0].Rows[0].Cells[e.ColumnIndex];  // 2020 05 16- CCENG: need to reset to 0 rows first then only point to selected rows.
            //            ////m_dgdView[0].CurrentCell = m_dgdView[0].Rows[i].Cells[e.ColumnIndex];
            //            //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
            //            //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
            //            //m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !blnValue;
            //            m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureCheckStandOff(i, e.ColumnIndex, !blnValue);
            //        }
            //        else if (e.ColumnIndex == 0 || e.ColumnIndex == 4)
            //        {
            //            int intValue = (m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell).Items.IndexOf(m_dgdView[0].Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue);

            //            //if (intValue == 0)
            //            //    intValue = 1;
            //            //else if (intValue == 1)
            //            //    intValue = 0;

            //            m_smVisionInfo.g_arrPad[j].UpdateBlobFeatureStandOffReferDirection(i, e.ColumnIndex, intValue);
            //        }
            //    }
            //}

            //UpdateDataToGrid(m_intPadIndex, m_dgdView[0]);
            //m_dgdView[0].Refresh();
            //m_blnUpdateInfo = true;
        }

        private void dgd_PadStandOffSetting_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (m_dgdView[0].IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                m_dgdView[0].CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
