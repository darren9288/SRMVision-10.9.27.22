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
    public partial class PadOffsetSettingForm : Form
    {
        #region Member Variables
        private bool m_blnShow = false;
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private bool m_blnUpdateInfo = false;
        private int m_intUserGroup = 5;
        private int m_intPadIndex = 0;
        private int m_intDecimal = 3;
        private string m_strSelectedRecipe;
        private string m_strUnitLabel = "";
        private bool m_blnChangeScoreSetting = true;
        private List<int> m_arrPadRowIndex = new List<int>();
        private List<int> m_arrGroupRowIndex = new List<int>();

        private DataGridView[] m_dgdView = new DataGridView[5];
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion
        
        public PadOffsetSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();


            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;

            m_dgdView[0] = dgd_MiddlePad;

            //DisableField();
            UpdateGUI();

            m_blnInitDone = true;
            
        }

        private void UpdateGUI()
        {
            radioBtn_Middle.Checked = true;

            ReadPadTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

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

            UpdateUnitDisplay();
        }
        private void UpdateUnitDisplay()
        {
            switch (m_smCustomizeInfo.g_intUnitDisplay)
            {
                case 1:
                default:
                    m_strUnitLabel = "mm";
                    m_intDecimal = 4;
                    break;
                case 2:
                    m_strUnitLabel = "mil";
                    m_intDecimal = 3;
                    break;
                case 3:
                    m_strUnitLabel = "um";
                    m_intDecimal = 1;
                    break;
            }
        }
        private void ReadPadTemplateDataToGrid(int intPadIndex, DataGridView dgd_PadSetting)
        {
            int intFailOptionMask = m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailOptionMask;

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantEdgeDistance_Pad)
            {
                if ((intFailOptionMask & 0x10000) > 0)
                {
                    dgd_PadSetting.Columns[5].Visible = m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantCheckPadEdgeDistance;
                    dgd_PadSetting.Columns[6].Visible = m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantCheckPadEdgeDistance;
                    dgd_PadSetting.Columns[7].Visible = m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantCheckPadEdgeDistance;
                    dgd_PadSetting.Columns[8].Visible = m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantCheckPadEdgeDistance;
                }
                else
                {
                    dgd_PadSetting.Columns[5].Visible = false;
                    dgd_PadSetting.Columns[6].Visible = false;
                    dgd_PadSetting.Columns[7].Visible = false;
                    dgd_PadSetting.Columns[8].Visible = false;
                }
            }
            else
            {
                dgd_PadSetting.Columns[5].Visible = false;
                dgd_PadSetting.Columns[6].Visible = false;
                dgd_PadSetting.Columns[7].Visible = false;
                dgd_PadSetting.Columns[8].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantEdgeLimit_Pad)
            {
                if ((intFailOptionMask & 0x4000) > 0)
                {
                    dgd_PadSetting.Columns[9].Visible = m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantCheckPadEdgeLimit;
                    dgd_PadSetting.Columns[10].Visible = m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantCheckPadEdgeLimit;
                    dgd_PadSetting.Columns[11].Visible = m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantCheckPadEdgeLimit;
                    dgd_PadSetting.Columns[12].Visible = m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantCheckPadEdgeLimit;
                }
                else
                {
                    m_dgdView[0].Columns[9].Visible = false;
                    m_dgdView[0].Columns[10].Visible = false;
                    m_dgdView[0].Columns[11].Visible = false;
                    m_dgdView[0].Columns[12].Visible = false;
                }
            }
            else
            {
                dgd_PadSetting.Columns[9].Visible = false;
                dgd_PadSetting.Columns[10].Visible = false;
                dgd_PadSetting.Columns[11].Visible = false;
                dgd_PadSetting.Columns[12].Visible = false;
            }

            if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantStandOff_Pad)
            {
                if ((intFailOptionMask & 0x8000) > 0)
                {
                    dgd_PadSetting.Columns[13].Visible = m_smVisionInfo.g_arrPad[intPadIndex].GetWantCheckStandOff(0);
                    dgd_PadSetting.Columns[14].Visible = m_smVisionInfo.g_arrPad[intPadIndex].GetWantCheckStandOff(1);
                    dgd_PadSetting.Columns[15].Visible = m_smVisionInfo.g_arrPad[intPadIndex].GetWantCheckStandOff(2);
                    dgd_PadSetting.Columns[16].Visible = m_smVisionInfo.g_arrPad[intPadIndex].GetWantCheckStandOff(3);
                }
                else
                {
                    dgd_PadSetting.Columns[13].Visible = false;
                    dgd_PadSetting.Columns[14].Visible = false;
                    dgd_PadSetting.Columns[15].Visible = false;
                    dgd_PadSetting.Columns[16].Visible = false;
                }
            }
            else
            {
                dgd_PadSetting.Columns[13].Visible = false;
                dgd_PadSetting.Columns[14].Visible = false;
                dgd_PadSetting.Columns[15].Visible = false;
                dgd_PadSetting.Columns[16].Visible = false;
            }

            dgd_PadSetting.Rows.Clear();

            List<List<string>> arrBlobsFeaturesData;

            arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[intPadIndex].GetPadOffsetSettingValue();

            for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
            {
                dgd_PadSetting.Rows.Add();
                dgd_PadSetting.Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0] + " " + (Convert.ToInt32(arrBlobsFeaturesData[i][1]) + 1).ToString();

                dgd_PadSetting.Rows[i].Cells[0].Value = arrBlobsFeaturesData[i][2];
                dgd_PadSetting.Rows[i].Cells[1].Value = arrBlobsFeaturesData[i][3];
                dgd_PadSetting.Rows[i].Cells[2].Value = arrBlobsFeaturesData[i][4];
                dgd_PadSetting.Rows[i].Cells[3].Value = arrBlobsFeaturesData[i][5];
                dgd_PadSetting.Rows[i].Cells[4].Value = arrBlobsFeaturesData[i][6];

                dgd_PadSetting.Rows[i].Cells[5].Value = arrBlobsFeaturesData[i][7];
                dgd_PadSetting.Rows[i].Cells[6].Value = arrBlobsFeaturesData[i][8];
                dgd_PadSetting.Rows[i].Cells[7].Value = arrBlobsFeaturesData[i][9];
                dgd_PadSetting.Rows[i].Cells[8].Value = arrBlobsFeaturesData[i][10];

                dgd_PadSetting.Rows[i].Cells[9].Value = arrBlobsFeaturesData[i][11];
                dgd_PadSetting.Rows[i].Cells[10].Value = arrBlobsFeaturesData[i][12];
                dgd_PadSetting.Rows[i].Cells[11].Value = arrBlobsFeaturesData[i][13];
                dgd_PadSetting.Rows[i].Cells[12].Value = arrBlobsFeaturesData[i][14];

                dgd_PadSetting.Rows[i].Cells[13].Value = arrBlobsFeaturesData[i][15];
                dgd_PadSetting.Rows[i].Cells[14].Value = arrBlobsFeaturesData[i][16];
                dgd_PadSetting.Rows[i].Cells[15].Value = arrBlobsFeaturesData[i][17];
                dgd_PadSetting.Rows[i].Cells[16].Value = arrBlobsFeaturesData[i][18];

                m_arrPadRowIndex.Add(i);
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
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad " + strSectionName + " Tolerance Settings", m_smProductionInfo.g_strLotID);
                
                //objFile.WriteElement1Value("OrientSetting", true);
                //objFile.WriteElement2Value("MatchMinScore", m_smVisionInfo.g_arrPadOrient[i].ref_fMinScore); 
            }

            //if (m_smVisionInfo.g_arrPin1 != null)
            //{
            //    DeviceEdit objDeviceEdit = new DeviceEdit(m_smProductionInfo.g_intUserGroup, m_smProductionInfo.g_strOperatorID, m_smProductionInfo.g_strRecipeID);
            //    STDeviceEdit.CopySettingFile(strFolderPath, "Pin1Template.xml");
            //    m_smVisionInfo.g_arrPin1[0].SaveTemplate(strFolderPath + "\\Template\\");
            //    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Tolerance Settings", strFolderPath, "Pin1Template.xml");
            //}
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            // Save Pad Setting
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\";

            SavePadSetting(strPath + "Pad\\");
            
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

            //switch (m_intPadIndex)
            //{
            //    case 0:
            //        radioBtn_Middle.Checked = true;
            //        break;
            //    case 1:
            //        radioBtn_Up.Checked = true;
            //        break;
            //    case 2:
            //        radioBtn_Right.Checked = true;
            //        break;
            //    case 3:
            //        radioBtn_Down.Checked = true;
            //        break;
            //    case 4:
            //        radioBtn_Left.Checked = true;
            //        break;
            //}

            if (sender == radioBtn_Middle)
            {
                m_intPadIndex = 0;
            }
            else if (sender == radioBtn_Up)
            {
                m_intPadIndex = 1;
            }
            else if (sender == radioBtn_Right)
            {
                m_intPadIndex = 2;
            }
            else if (sender == radioBtn_Down)
            {
                m_intPadIndex = 3;
            }
            else if (sender == radioBtn_Left)
            {
                m_intPadIndex = 4;
            }

            ReadPadTemplateDataToGrid(m_intPadIndex, m_dgdView[0]);

            m_blnUpdateSelectedROISetting = false;
            
        }

        private void dgd_MiddlePad_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            string strCurrentSetValue = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            string strDisplayMessage = "Set value to " + ((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value.ToString();

            int intSetAllROI;
            if (m_intPadIndex == 0)
                intSetAllROI = 0;
            else
                intSetAllROI = 1;

            SetValueForm objSetValueForm = new SetValueForm(strDisplayMessage, m_strUnitLabel, e.RowIndex, m_intDecimal, strCurrentSetValue, intSetAllROI, false, false);

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

                int intTotalRows;
                if (objSetValueForm.ref_blnSetAllRows)
                {
                    intTotalRows = m_smVisionInfo.g_arrPad[m_intPadIndex].GetBlobsFeaturesNumber();

                    //if (m_smVisionInfo.g_arrPad[m_intPadIndex].ref_blnWantUseGroupToleranceSetting)
                    //{
                    //    intTotalRows += m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intPadGroupCount;  // 2019 12 23 - CCENG: total pad rows + additional group rows.
                    //}
                }
                else
                {
                    intTotalRows = intStartRowNumber + 1;
                }

                // Loop row index
                for (int i = intStartRowNumber; i < intTotalRows; i++)
                {

                    if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
                    {
                        // Set new insert value into table
                        //if (objSetValueForm.ref_blnSetAllEdges)
                        //{
                        //    for (int j = 0; j < ((DataGridView)sender).Columns.Count; j++)
                        //    {
                        //        ((DataGridView)sender).Rows[i].Cells[j].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                        //    }
                        //}
                        //else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                    }
                }

                // --------------------------------------------------------------------------------------

                // Loop Pad index
                for (int j = intStartIndex; j < intEndIndex; j++)
                {
                    intTotalRows = m_smVisionInfo.g_arrPad[j].GetBlobsFeaturesNumber();
                    //if (m_smVisionInfo.g_arrPad[j].ref_blnWantUseGroupToleranceSetting)
                    //{
                    //    intTotalRows = e.RowIndex + m_smVisionInfo.g_arrPad[j].ref_intPadGroupCount;
                    //}
                    //else 
                    if (!objSetValueForm.ref_blnSetAllRows)
                    {
                        intTotalRows = intStartRowNumber + 1;
                    }
                    // Loop row index
                    for (int i = intStartRowNumber; i < intTotalRows; i++)
                    {
                        //if (objSetValueForm.ref_blnSetAllEdges)
                        //{
                        //    for (int k = 0; k < ((DataGridView)sender).Columns.Count; k++)
                        //    {
                        //        m_smVisionInfo.g_arrPad[j].UpdatePadOffsetSettingValueToPixel(i, k,
                        //        objSetValueForm.ref_fSetValue);
                        //    }
                        //}
                        //else
                            m_smVisionInfo.g_arrPad[j].UpdatePadOffsetSettingValueToPixel(i, e.ColumnIndex,
                                    objSetValueForm.ref_fSetValue);
                    }
                }

            }
            
            this.TopMost = true;
        }

        private void dgd_MiddlePad_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!m_blnChangeScoreSetting)
                return;

            if (e.ColumnIndex < 0)
                return;

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
            SetValueForm objSetValueForm = new SetValueForm("Set value to all pads", m_strUnitLabel, e.RowIndex, m_intDecimal, strCurrentSetValue, true, false, false);
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

                        // Set new insert value into table
                        //if (objSetValueForm.ref_blnSetAllEdges)
                        //{
                        //    for (int j = 0; j < ((DataGridView)sender).Columns.Count; j++)
                        //    {
                        //        ((DataGridView)sender).Rows[i].Cells[j].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                        //    }
                        //}
                        //else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");


                    }

                    //Update template setting
                    //if (intLengthMode == 1)
                    {
                        m_smVisionInfo.g_arrPad[m_intPadIndex].UpdatePadOffsetSettingValueToPixel(i,
                             float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[1].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[2].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[4].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[5].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[6].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[7].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[8].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[9].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[10].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[11].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[12].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[13].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[14].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[15].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[16].Value.ToString())
                             );


                    }

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }
            
            this.TopMost = true;
        }
    
    }
}
