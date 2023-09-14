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
    public partial class PadInspectionAreaSettingForm : Form
    {
        #region Member Variables
        private bool m_blnUpdateDrawing = false;
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

        #region Properties

        public bool ref_blnShow { get { return m_blnShow; } set { m_blnShow = value; } }

        #endregion

        public PadInspectionAreaSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
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

            m_smVisionInfo.g_intViewInspectionSetting = 1;
            m_smVisionInfo.AT_VM_OfflineTestAllPad = true;
            TriggerOfflineTest();
            timer1.Start();
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
            m_blnUpdateDrawing = true;
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
            dgd_PadSetting.Rows.Clear();

            List<List<string>> arrBlobsFeaturesData;

            arrBlobsFeaturesData = m_smVisionInfo.g_arrPad[intPadIndex].GetPadInspectionAreaValue();
            
            for (int i = 0; i < arrBlobsFeaturesData.Count; i++)
            {
                dgd_PadSetting.Rows.Add();
                dgd_PadSetting.Rows[i].HeaderCell.Value = arrBlobsFeaturesData[i][0] + " " + (Convert.ToInt32(arrBlobsFeaturesData[i][1]) + 1).ToString();
                
                dgd_PadSetting.Rows[i].Cells[0].Value = arrBlobsFeaturesData[i][2];
                dgd_PadSetting.Rows[i].Cells[1].Value = arrBlobsFeaturesData[i][3];
                dgd_PadSetting.Rows[i].Cells[2].Value = arrBlobsFeaturesData[i][4];
                dgd_PadSetting.Rows[i].Cells[3].Value = arrBlobsFeaturesData[i][5];

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

        private void PadInspectionAreaSettingForm_Load(object sender, EventArgs e)
        {
            m_blnShow = true;
            m_smVisionInfo.g_blnViewPadSettingDrawing = true;
        }

        private void PadInspectionAreaSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_blnShow = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.g_blnViewPadInspectionAreaSetting = false;
            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if ((radioBtn_Middle.ForeColor == Color.Red) || (radioBtn_Up.ForeColor == Color.Red) || (radioBtn_Right.ForeColor == Color.Red) || (radioBtn_Down.ForeColor == Color.Red) || (radioBtn_Left.ForeColor == Color.Red))
            {
                SRMMessageBox.Show("Inspection area tolerance cannot goes beyond opposite direction tolerance. Please check Red Colored box or radio button.");
                return;
            }
            ////update CPK tolerance data (USL & LSL) to the latest when setting change
            //if (m_smVisionInfo.g_blnCPKON)
            //{
            //    string strTemplateBlobsFeatures;
            //    string[] strTemplateFeature = new string[100];
            //    int intPadNumber;

            //    int intGroupIndex = 0;
            //    for (int p = 0; p < m_smVisionInfo.g_arrPad.Length; p++)
            //    {
            //        intPadNumber = m_smVisionInfo.g_arrPad[p].GetBlobsFeaturesNumber();

            //        for (int i = 0; i < intPadNumber; i++)
            //        {
            //            strTemplateBlobsFeatures = m_smVisionInfo.g_arrPad[p].GetBlobFeaturesInspectRealData(i);
            //            strTemplateFeature = strTemplateBlobsFeatures.Split('#');

            //            for (int j = 0; j < strTemplateFeature.Length; j++)
            //            {
            //                if (strTemplateFeature[j] != "")
            //                    if (Convert.ToSingle(strTemplateFeature[j]) == -1)
            //                        strTemplateFeature[j] = "0";
            //            }

            //            m_smVisionInfo.g_objCPK.SetSpecification(0, intGroupIndex, 0f, Convert.ToSingle(strTemplateFeature[0]));
            //            m_smVisionInfo.g_objCPK.SetSpecification(1, intGroupIndex, Convert.ToSingle(strTemplateFeature[1]), Convert.ToSingle(strTemplateFeature[2]));
            //            m_smVisionInfo.g_objCPK.SetSpecification(2, intGroupIndex, Convert.ToSingle(strTemplateFeature[3]), Convert.ToSingle(strTemplateFeature[4]));
            //            m_smVisionInfo.g_objCPK.SetSpecification(3, intGroupIndex, Convert.ToSingle(strTemplateFeature[5]), Convert.ToSingle(strTemplateFeature[6]));
            //            m_smVisionInfo.g_objCPK.SetSpecification(4, intGroupIndex, Convert.ToSingle(strTemplateFeature[7]), Convert.ToSingle(strTemplateFeature[8]));
            //            m_smVisionInfo.g_objCPK.SetSpecification(5, intGroupIndex, Convert.ToSingle(strTemplateFeature[9]), Convert.ToSingle(strTemplateFeature[10]));

            //            intGroupIndex++;
            //        }
            //    }
            //}

            // Save Pad Setting
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\";

            SavePadSetting(strPath + "Pad\\");

            //if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
            //    m_smProductionInfo.g_blnSaveRecipeToServer = true;

            //m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

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
            CheckSettingError();
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

            SetValueForm objSetValueForm = new SetValueForm(strDisplayMessage, m_strUnitLabel, e.RowIndex, m_intDecimal, strCurrentSetValue, intSetAllROI, true, false);

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
                  
                    if (((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value.ToString() != "---")
                    {
                        // Set new insert value into table
                        if (objSetValueForm.ref_blnSetAllEdges)
                        {
                            for (int j = 0; j < ((DataGridView)sender).Columns.Count; j++)
                            {
                                ((DataGridView)sender).Rows[i].Cells[j].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                            }
                        }
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
                    else if (!objSetValueForm.ref_blnSetAllRows)
                    {
                        intTotalRows = intStartRowNumber + 1;
                    }
                    // Loop row index
                    for (int i = intStartRowNumber; i < intTotalRows; i++)
                    {
                        if (objSetValueForm.ref_blnSetAllEdges)
                        {
                            for (int k = 0; k < ((DataGridView)sender).Columns.Count; k++)
                            {
                                m_smVisionInfo.g_arrPad[j].UpdatePadInspectionAreaValueToPixel(i, k,
                                objSetValueForm.ref_fSetValue);
                            }
                        }
                        else
                            m_smVisionInfo.g_arrPad[j].UpdatePadInspectionAreaValueToPixel(i, e.ColumnIndex,
                                    objSetValueForm.ref_fSetValue);
                    }
                }

            }
            m_blnUpdateDrawing = true;
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
            SetValueForm objSetValueForm = new SetValueForm("Set value to all pads", m_strUnitLabel, e.RowIndex, m_intDecimal, strCurrentSetValue, true, true, false);
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
                        if (objSetValueForm.ref_blnSetAllEdges)
                        {
                            for (int j = 0; j < ((DataGridView)sender).Columns.Count; j++)
                            {
                                ((DataGridView)sender).Rows[i].Cells[j].Value = objSetValueForm.ref_fSetValue.ToString("F4");
                            }
                        }
                        else
                            ((DataGridView)sender).Rows[i].Cells[e.ColumnIndex].Value = objSetValueForm.ref_fSetValue.ToString("F4");
     
                    
                    }

                    //Update template setting
                    //if (intLengthMode == 1)
                    {
                        m_smVisionInfo.g_arrPad[m_intPadIndex].UpdatePadInspectionAreaValueToPixel(i,
                             float.Parse(((DataGridView)sender).Rows[i].Cells[0].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[1].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[2].Value.ToString()),
                             float.Parse(((DataGridView)sender).Rows[i].Cells[3].Value.ToString()));

                    
                    }

                    if (!objSetValueForm.ref_blnSetAllRows)
                        break;
                }
            }
            m_blnUpdateDrawing = true;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smVisionInfo.PR_MN_TestDone && m_blnUpdateDrawing)
            {
                m_blnUpdateDrawing = false;
                m_smVisionInfo.g_blnViewPadInspectionAreaSetting = true;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                CheckSettingError();
            }
        }

        private void CheckSettingError()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                bool blnOverallGotError = false;
                for (int j = 0; j < dgd_MiddlePad.Rows.Count; j++)
                {
                    int ErrorCode = m_smVisionInfo.g_arrPad[i].CheckInspectionAreaSettingError(j);
                    blnOverallGotError |= (ErrorCode > 0);

                    if (j == (dgd_MiddlePad.Rows.Count -1))
                    {
                        if (blnOverallGotError)
                        {
                            switch (i)
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
                            switch (i)
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
                   
                    if (m_intPadIndex == i)
                    {

                        if ((Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[0].Value) >= 0) && (Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[2].Value) >= 0))
                        {
                            if ((ErrorCode & 0x01) > 0) //top bottom
                            {
                                dgd_MiddlePad.Rows[j].Cells[0].Style.BackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[2].Style.BackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[0].Style.SelectionBackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[2].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_MiddlePad.Rows[j].Cells[0].Style.BackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[2].Style.BackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[0].Style.SelectionBackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[2].Style.SelectionBackColor = Color.White;
                            }
                        }

                        if ((Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[1].Value) >= 0) && (Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[3].Value) >= 0))
                        {
                            if ((ErrorCode & 0x02) > 0) // left right
                            {
                                dgd_MiddlePad.Rows[j].Cells[1].Style.BackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[3].Style.BackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[1].Style.SelectionBackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[3].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_MiddlePad.Rows[j].Cells[1].Style.BackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[3].Style.BackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[1].Style.SelectionBackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[3].Style.SelectionBackColor = Color.White;
                            }
                        }

                        if ((Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[0].Value) >= 0) && (Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[2].Value) < 0))
                        {
                            if ((ErrorCode & 0x04) > 0) // top
                            {
                                dgd_MiddlePad.Rows[j].Cells[0].Style.BackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[0].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_MiddlePad.Rows[j].Cells[0].Style.BackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[0].Style.SelectionBackColor = Color.White;
                            }
                        }

                        if ((Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[1].Value) >= 0) && (Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[3].Value) < 0))
                        {
                            if ((ErrorCode & 0x08) > 0) // right
                            {
                                dgd_MiddlePad.Rows[j].Cells[1].Style.BackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[1].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_MiddlePad.Rows[j].Cells[1].Style.BackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[1].Style.SelectionBackColor = Color.White;
                            }
                        }

                        if ((Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[0].Value) < 0) && (Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[2].Value) >= 0))
                        {
                            if ((ErrorCode & 0x10) > 0) // bottom
                            {
                                dgd_MiddlePad.Rows[j].Cells[2].Style.BackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[2].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_MiddlePad.Rows[j].Cells[2].Style.BackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[2].Style.SelectionBackColor = Color.White;
                            }
                        }

                        if ((Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[1].Value) < 0) && (Convert.ToDouble(dgd_MiddlePad.Rows[j].Cells[3].Value) >= 0))
                        {
                            if ((ErrorCode & 0x20) > 0) // left
                            {
                                dgd_MiddlePad.Rows[j].Cells[3].Style.BackColor = Color.Red;
                                dgd_MiddlePad.Rows[j].Cells[3].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_MiddlePad.Rows[j].Cells[3].Style.BackColor = Color.White;
                                dgd_MiddlePad.Rows[j].Cells[3].Style.SelectionBackColor = Color.White;
                            }
                        }
                    }
                }
            }
        }
    }
}
