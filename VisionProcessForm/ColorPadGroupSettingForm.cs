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
    public partial class ColorPadGroupSettingForm : Form
    {
        #region Member Variables
        //private int m_intCenterGroup_Prev = 0x01;
        private int m_intTopGroup_Prev = 0x02;
        private int m_intRightGroup_Prev = 0x04;
        private int m_intBottomGroup_Prev = 0x08;
        private int m_intLeftGroup_Prev = 0x10;
        private int m_intOptionControlMask2 = 0;
        private int m_intOptionControlMask3 = 0;
        private int m_intOptionControlMask4 = 0;
        private int m_intOptionControlMask5 = 0;
        private bool m_blnUpdatingComboBox = false;
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private bool m_blnUpdateInfo = false;
        private int m_intUserGroup = 5;
        private int m_intPadIndex = 0;

        private string m_strSelectedRecipe;

        private bool m_blnChangeScoreSetting = true;
        private List<int> m_arrPadRowIndex = new List<int>();
        private List<int> m_arrGroupRowIndex = new List<int>();
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion

        public ColorPadGroupSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;

            //m_intCenterGroup_Prev = m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            m_intTopGroup_Prev = m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
            m_intRightGroup_Prev = m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
            m_intBottomGroup_Prev = m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
            m_intLeftGroup_Prev = m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
            m_intOptionControlMask2 = m_smVisionInfo.g_intOptionControlMask2;
            m_intOptionControlMask3 = m_smVisionInfo.g_intOptionControlMask3;
            m_intOptionControlMask4 = m_smVisionInfo.g_intOptionControlMask4;
            m_intOptionControlMask5 = m_smVisionInfo.g_intOptionControlMask5;
            UpdateGUI();
            UpdateTable();
            m_blnInitDone = true;
        }
        private void UpdateTable()
        {
            dgd_Defect.Rows.Clear();

            dgd_Defect.Rows.Add();
            dgd_Defect.Rows.Add();
            dgd_Defect.Rows.Add();
            dgd_Defect.Rows.Add();
            dgd_Defect.Rows.Add();

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName.Count; j++)
                {
                    dgd_Defect.Rows[j].Cells[i].Value = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j];
                }
            }
        }
        private void UpdateGUI()
        {
            m_blnUpdatingComboBox = true;

            //cbo_Center.Items.Clear();
            cbo_Top.Items.Clear();
            cbo_Right.Items.Clear();
            cbo_Bottom.Items.Clear();
            cbo_Left.Items.Clear();
            int intGroupCount = 1;
            bool blnSkipTop = false, blnSkipRight = false, blnSkipBottom = false, blnSkipLeft = false;
            
            //cbo_Center.Items.Add("Group "+ intGroupCount.ToString());
            //cbo_Top.Items.Add("Group " + intGroupCount.ToString());
            //cbo_Right.Items.Add("Group " + intGroupCount.ToString());
            //cbo_Bottom.Items.Add("Group " + intGroupCount.ToString());
            //cbo_Left.Items.Add("Group " + intGroupCount.ToString());
            //cbo_Center.SelectedIndex = intGroupCount - 1;
            //if ((m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex & 0x01) > 0)
            //{
            //    cbo_Top.SelectedIndex = intGroupCount - 1;
            //    blnSkipTop = true;
            //}
            //if ((m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex & 0x01) > 0)
            //{
            //    cbo_Right.SelectedIndex = intGroupCount - 1;
            //    blnSkipRight = true;
            //}
            //if ((m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex & 0x01) > 0)
            //{
            //    cbo_Bottom.SelectedIndex = intGroupCount - 1;
            //    blnSkipBottom = true;
            //}
            //if ((m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex & 0x01) > 0)
            //{
            //    cbo_Left.SelectedIndex = intGroupCount - 1;
            //    blnSkipLeft = true;
            //}
            
            if (!blnSkipTop)
            {
                //intGroupCount++;
                //cbo_Center.Items.Add("Group " + intGroupCount.ToString());
                cbo_Top.Items.Add("Group " + intGroupCount.ToString());
                cbo_Right.Items.Add("Group " + intGroupCount.ToString());
                cbo_Bottom.Items.Add("Group " + intGroupCount.ToString());
                cbo_Left.Items.Add("Group " + intGroupCount.ToString());
                cbo_Top.SelectedIndex = intGroupCount - 1;
                if ((m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex & 0x02) > 0)
                {
                    cbo_Right.SelectedIndex = intGroupCount - 1;
                    blnSkipRight = true;
                }
                if ((m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex & 0x02) > 0)
                {
                    cbo_Bottom.SelectedIndex = intGroupCount - 1;
                    blnSkipBottom = true;
                }
                if ((m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex & 0x02) > 0)
                {
                    cbo_Left.SelectedIndex = intGroupCount - 1;
                    blnSkipLeft = true;
                }
            }

            if (!blnSkipRight)
            {
                intGroupCount++;
                //cbo_Center.Items.Add("Group " + intGroupCount.ToString());
                cbo_Top.Items.Add("Group " + intGroupCount.ToString());
                cbo_Right.Items.Add("Group " + intGroupCount.ToString());
                cbo_Bottom.Items.Add("Group " + intGroupCount.ToString());
                cbo_Left.Items.Add("Group " + intGroupCount.ToString());
                cbo_Right.SelectedIndex = intGroupCount - 1;

                if ((m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex & 0x04) > 0)
                {
                    cbo_Bottom.SelectedIndex = intGroupCount - 1;
                    blnSkipBottom = true;
                }
                if ((m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex & 0x04) > 0)
                {
                    cbo_Left.SelectedIndex = intGroupCount - 1;
                    blnSkipLeft = true;
                }
            }

            if (!blnSkipBottom)
            {
                intGroupCount++;
                //cbo_Center.Items.Add("Group " + intGroupCount.ToString());
                cbo_Top.Items.Add("Group " + intGroupCount.ToString());
                cbo_Right.Items.Add("Group " + intGroupCount.ToString());
                cbo_Bottom.Items.Add("Group " + intGroupCount.ToString());
                cbo_Left.Items.Add("Group " + intGroupCount.ToString());
                cbo_Bottom.SelectedIndex = intGroupCount - 1;
                if ((m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex & 0x08) > 0)
                {
                    cbo_Left.SelectedIndex = intGroupCount - 1;
                    blnSkipLeft = true;
                }
            }

            if (!blnSkipLeft)
            {
                intGroupCount++;
                //cbo_Center.Items.Add("Group " + intGroupCount.ToString());
                cbo_Top.Items.Add("Group " + intGroupCount.ToString());
                cbo_Right.Items.Add("Group " + intGroupCount.ToString());
                cbo_Bottom.Items.Add("Group " + intGroupCount.ToString());
                cbo_Left.Items.Add("Group " + intGroupCount.ToString());
                cbo_Left.SelectedIndex = intGroupCount - 1;
            }

            //if (cbo_Center.Items.Count < 5)
            //{
            //    cbo_Center.Items.Add("New Group");
            //    cbo_Top.Items.Add("New Group");
            //    cbo_Right.Items.Add("New Group");
            //    cbo_Bottom.Items.Add("New Group");
            //    cbo_Left.Items.Add("New Group");
            //}
            if (cbo_Top.Items.Count < 4)
            {
                cbo_Top.Items.Add("New Group");
                cbo_Right.Items.Add("New Group");
                cbo_Bottom.Items.Add("New Group");
                cbo_Left.Items.Add("New Group");
            }

            m_blnUpdatingComboBox = false;

        }

        private void ColorPadGroupSettingForm_Load(object sender, EventArgs e)
        {

            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void ColorPadGroupSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void DeleteDontCare(int intPadIndex, int intThresholdIndex)
        {
            if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > intPadIndex)
            {
                if (m_smVisionInfo.g_arrPadColorDontCareROIs[intPadIndex].Count > intThresholdIndex)
                {
                    m_smVisionInfo.g_arrPadColorDontCareROIs[intPadIndex][intThresholdIndex].Clear();
                    m_smVisionInfo.g_arrPolygon_PadColor[intPadIndex][intThresholdIndex].Clear();
                }
                else
                {
                    m_smVisionInfo.g_arrPadColorDontCareROIs[intPadIndex].Add(new List<ROI>());
                    m_smVisionInfo.g_arrPolygon_PadColor[intPadIndex].Add(new List<Polygon>());
                }
            }
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            SaveControlSettings(strFolderPath + "Pad\\Template\\");
            SaveColorROISetting(strFolderPath + "Pad\\");

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                // 22-08-2019 ZJYEOH : Reset all inspection data so that drawing will not cause error when opening inspection result and learn pad on the same time
                m_smVisionInfo.g_arrPad[i].ResetInspectionData(false);

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

                m_smVisionInfo.g_arrPad[i].SaveColorPad(strFolderPath + "Pad\\" + "Template\\Template.xml",
                    false, strSectionName, false);

            }

            string strPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" +
                         m_smVisionInfo.g_strVisionFolderName + "\\Pad\\";
            SaveColorDontCareROISetting(strPath);
            SaveColorDontCareSetting(strPath + "Template\\");

            LoadROISetting(strPath + "ColorDontCareROI.xml", m_smVisionInfo.g_arrPadColorDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                        {
                            for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                            {
                                m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                            }
                        }
                    }

                }
            }
            LoadColorROISetting(strFolderPath + "Pad\\" + "CROI.xml", m_smVisionInfo.g_arrPadColorROIs, m_smVisionInfo.g_arrPad.Length);
            
            this.Close();
            this.Dispose();
        }
        private void SaveColorROISetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "CROI.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "CROI.xml", false);

            CROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrPadColorROIs.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPadColorROIs[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrPadColorROIs[t][j];
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
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad Color ROI", m_smProductionInfo.g_strLotID);

        }
        private void SaveColorDontCareROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "ColorDontCareROI.xml", false);

            for (int t = 0; t < m_smVisionInfo.g_arrPadColorDontCareROIs.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[t].Count; j++)
                {
                    objFile.WriteElement1Value("Color Defect" + t + j, "");

                    for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[t][j].Count; k++)
                    {
                        objFile.WriteElement2Value("ROI" + k, "");

                        objFile.WriteElement3Value("Name", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_strROIName);
                        objFile.WriteElement3Value("Type", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intType);
                        objFile.WriteElement3Value("PositionX", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIPositionX);
                        objFile.WriteElement3Value("PositionY", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIPositionY);
                        objFile.WriteElement3Value("Width", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIWidth);
                        objFile.WriteElement3Value("Height", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIHeight);
                        objFile.WriteElement3Value("StartOffsetX", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intStartOffsetX);
                        objFile.WriteElement3Value("StartOffsetY", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intStartOffsetY);
                        m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[t][3]);

                    }
                }
            }
            objFile.WriteEndElement();


        }
        private void SaveColorDontCareSetting(string strFolderPath)
        {
            ImageDrawing objImage = new ImageDrawing();
            ImageDrawing objFinalImage = new ImageDrawing();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {

                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                //Check is the ROI for drawing dont care changed
                //m_smVisionInfo.g_arrPolygon_Pad[i][0].CheckDontCarePosition(m_smVisionInfo.g_arrPadROIs[i][2], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                {
                    m_smVisionInfo.g_objBlackImage.CopyTo(objFinalImage);

                    for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                    {
                        //Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        if (m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ref_intFormMode != 2)
                            Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        else
                            Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                        ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);
                    }
                    objFinalImage.SaveImage(strFolderPath + "ColorDontCareImage" + i.ToString() + "_" + j.ToString() + ".bmp");
                }


            }
            objImage.Dispose();
            objFinalImage.Dispose();
            Polygon.SavePolygon(strFolderPath + "ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PadColor);
        }
        private void LoadROISetting(string strPath, List<List<List<ROI>>> arrROIs, int intROICount)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            ROI objROI;

            for (int i = 0; i < intROICount; i++)
            {
                arrROIs.Add(new List<List<ROI>>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    arrROIs[i].Add(new List<ROI>());
                    objFile.GetSecondSection("Color Defect" + i + j);
                    int intThirdChildCount = objFile.GetThirdSectionCount();
                    for (int k = 0; k < intThirdChildCount; k++)
                    {
                        objROI = new ROI();
                        objFile.GetThirdSection("ROI" + k);
                        objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 3);
                        objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 3);
                        objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 3);
                        objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 3);
                        objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 3);
                        objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 3);
                        objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 3);
                        objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 3);
                        objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 3));
                        if (objROI.ref_intType > 1)
                        {
                            objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                            objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                        }

                        arrROIs[i][j].Add(objROI);
                    }
                }
            }
        }
        private void LoadColorROISetting(string strPath, List<List<CROI>> arrROIs, int intROICount)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            CROI objROI;

            for (int i = 0; i < intROICount; i++)
            {
                arrROIs.Add(new List<CROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new CROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
                    //objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    //if (objROI.ref_intType > 1)
                    //{
                    //    objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                    //    objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    //}

                    arrROIs[i].Add(objROI);
                }
            }
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intOptionControlMask2 = m_intOptionControlMask2;
            m_smVisionInfo.g_intOptionControlMask3 = m_intOptionControlMask3;
            m_smVisionInfo.g_intOptionControlMask4 = m_intOptionControlMask4;
            m_smVisionInfo.g_intOptionControlMask5 = m_intOptionControlMask5;

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

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

                m_smVisionInfo.g_arrPad[i].LoadColorPadOnly(strFolderPath + "\\Pad\\Template\\Template.xml", strSectionName);
            }

            LoadROISetting(strFolderPath + "\\Pad\\ColorDontCareROI.xml", m_smVisionInfo.g_arrPadColorDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                        {
                            for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                            {
                                m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                            }
                        }
                    }

                }
            }
            Polygon.LoadPolygon(strFolderPath + "\\Pad\\Template\\ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PadColor, m_smVisionInfo.g_arrPad.Length);

            this.Close();
            this.Dispose();
        }

        private void TransferData(int intToPad, int intFromPad)
        {

            m_smVisionInfo.g_arrPad[intToPad].ResetColorThresholdData();
            for (int i = 0; i < m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorThresName.Count; i++)
            {
                m_smVisionInfo.g_arrPad[intToPad].AddColorThresholdData(
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorThresName[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorSystem[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectCloseIteration[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectInvertBlackWhite[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColor[i][0],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColor[i][1],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColor[i][2],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorTolerance[i][0],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorTolerance[i][1],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorTolerance[i][2],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorMinArea[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectType[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectImageNo[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectDontCareMode[i],
                    m_smVisionInfo.g_arrPad[intFromPad].GetColorDefectInspectionFailCondition(i),
                    m_smVisionInfo.g_arrPad[intFromPad].GetColorDefectInspectionWidthLimit(1, i),
                    m_smVisionInfo.g_arrPad[intFromPad].GetColorDefectInspectionLengthLimit(1, i),
                    m_smVisionInfo.g_arrPad[intFromPad].GetColorDefectInspectionMinAreaLimit(1, i),
                    m_smVisionInfo.g_arrPad[intFromPad].GetColorDefectInspectionMaxAreaLimit(1, i),
                    m_smVisionInfo.g_arrPad[intFromPad].GetColorDefectInspectionTotalAreaLimit(1, i),
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorInspection_Top[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorInspection_Right[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorInspection_Bottom[i],
                    m_smVisionInfo.g_arrPad[intFromPad].ref_arrDefectColorInspection_Left[i]
                    );

                DeleteDontCare(intToPad, i);
            }
            m_smVisionInfo.g_arrPad[intToPad].ref_intFailColorOptionMask = m_smVisionInfo.g_arrPad[intFromPad].ref_intFailColorOptionMask;

            int intControlMaskFromPad = 0;
            if (intFromPad == 1)
                intControlMaskFromPad = m_smVisionInfo.g_intOptionControlMask2;
            else if (intFromPad == 2)
                intControlMaskFromPad = m_smVisionInfo.g_intOptionControlMask3;
            if (intFromPad == 3)
                intControlMaskFromPad = m_smVisionInfo.g_intOptionControlMask4;
            if (intFromPad == 4)
                intControlMaskFromPad = m_smVisionInfo.g_intOptionControlMask5;

            if (intToPad == 1)
                m_smVisionInfo.g_intOptionControlMask2 = intControlMaskFromPad;
            else if (intToPad == 2)
                m_smVisionInfo.g_intOptionControlMask3 = intControlMaskFromPad;
            if (intToPad == 3)
                m_smVisionInfo.g_intOptionControlMask4 = intControlMaskFromPad;
            if (intToPad == 4)
                m_smVisionInfo.g_intOptionControlMask5 = intControlMaskFromPad;
            
        }

        private void cbo_Center_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone || m_blnUpdatingComboBox)
            //    return;

            //int Mask = 0;
            //if (m_intCenterGroup_Prev == m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex)
            //{
            //    m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex &= ~0x01;
            //    Mask |= 0x02;
            //}
            //if (m_intCenterGroup_Prev == m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex)
            //{
            //    m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex &= ~0x01;
            //    Mask |= 0x04;
            //}
            //if (m_intCenterGroup_Prev == m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex)
            //{
            //    m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex &= ~0x01;
            //    Mask |= 0x08;
            //}
            //if (m_intCenterGroup_Prev == m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex)
            //{
            //    m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex &= ~0x01;
            //    Mask |= 0x10;
            //}

            //if (Mask > 0)
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex ^= Mask;
            
            //if (cbo_Center.SelectedItem.ToString() == cbo_Top.SelectedItem.ToString())
            //{
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
            //    m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //    TransferData(0, 1);
            //}
            //if (cbo_Center.SelectedItem.ToString() == cbo_Right.SelectedItem.ToString())
            //{
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
            //    m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //    TransferData(0, 2);
            //}
            //if (cbo_Center.SelectedItem.ToString() == cbo_Bottom.SelectedItem.ToString())
            //{
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
            //    m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //    TransferData(0, 3);
            //}
            //if (cbo_Center.SelectedItem.ToString() == cbo_Left.SelectedItem.ToString())
            //{
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
            //    m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //    TransferData(0, 4);
            //}

            //UpdateGUI();
            //UpdateTable();

            //m_intCenterGroup_Prev = m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //m_intTopGroup_Prev = m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
            //m_intRightGroup_Prev = m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
            //m_intBottomGroup_Prev = m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
            //m_intLeftGroup_Prev = m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;

        }

        private void cbo_Top_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_blnUpdatingComboBox)
                return;

            int Mask = 0;
            //if (m_intTopGroup_Prev == m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex)
            //{
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex &= ~0x02;
            //    Mask |= 0x01;
            //}
            if (m_intTopGroup_Prev == m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex &= ~0x02;
                Mask |= 0x04;
            }
            if (m_intTopGroup_Prev == m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex &= ~0x02;
                Mask |= 0x08;
            }
            if (m_intTopGroup_Prev == m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex &= ~0x02;
                Mask |= 0x10;
            }

            if (Mask > 0)
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex ^= Mask;

            //if (cbo_Top.SelectedItem.ToString() == cbo_Center.SelectedItem.ToString())
            //{
            //    m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
            //    TransferData(1, 0);
            //}
            if (cbo_Top.SelectedItem.ToString() == cbo_Right.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
                TransferData(1, 2);
            }
            if (cbo_Top.SelectedItem.ToString() == cbo_Bottom.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
                TransferData(1, 3);
            }
            if (cbo_Top.SelectedItem.ToString() == cbo_Left.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
                TransferData(1, 4);
            }

            UpdateGUI();
            UpdateTable();

            //m_intCenterGroup_Prev = m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            m_intTopGroup_Prev = m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
            m_intRightGroup_Prev = m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
            m_intBottomGroup_Prev = m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
            m_intLeftGroup_Prev = m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;

        }

        private void cbo_Right_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_blnUpdatingComboBox)
                return;

            int Mask = 0;
            //if (m_intRightGroup_Prev == m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex)
            //{
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex &= ~0x04;
            //    Mask |= 0x01;
            //}
            if (m_intRightGroup_Prev == m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex &= ~0x04;
                Mask |= 0x02;
            }
            if (m_intRightGroup_Prev == m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex &= ~0x04;
                Mask |= 0x08;
            }
            if (m_intRightGroup_Prev == m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex &= ~0x04;
                Mask |= 0x10;
            }

            if (Mask > 0)
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex ^= Mask;

            //if (cbo_Right.SelectedItem.ToString() == cbo_Center.SelectedItem.ToString())
            //{
            //    m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
            //    TransferData(2, 0);
            //}
            if (cbo_Right.SelectedItem.ToString() == cbo_Top.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
                TransferData(2, 1);
            }
            if (cbo_Right.SelectedItem.ToString() == cbo_Bottom.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
                TransferData(2, 3);
            }
            if (cbo_Right.SelectedItem.ToString() == cbo_Left.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
                TransferData(2, 4);
            }

            UpdateGUI();
            UpdateTable();

            //m_intCenterGroup_Prev = m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            m_intTopGroup_Prev = m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
            m_intRightGroup_Prev = m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
            m_intBottomGroup_Prev = m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
            m_intLeftGroup_Prev = m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;

        }

        private void cbo_Bottom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_blnUpdatingComboBox)
                return;

            int Mask = 0;
            //if (m_intBottomGroup_Prev == m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex)
            //{
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex &= ~0x08;
            //    Mask |= 0x01;
            //}
            if (m_intBottomGroup_Prev == m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex &= ~0x08;
                Mask |= 0x02;
            }
            if (m_intBottomGroup_Prev == m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex &= ~0x08;
                Mask |= 0x04;
            }
            if (m_intBottomGroup_Prev == m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex &= ~0x08;
                Mask |= 0x10;
            }

            if (Mask > 0)
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex ^= Mask;

            //if (cbo_Bottom.SelectedItem.ToString() == cbo_Center.SelectedItem.ToString())
            //{
            //    m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
            //    TransferData(3, 0);
            //}
            if (cbo_Bottom.SelectedItem.ToString() == cbo_Top.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
                TransferData(3, 1);
            }
            if (cbo_Bottom.SelectedItem.ToString() == cbo_Right.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
                TransferData(3, 2);
            }
            if (cbo_Bottom.SelectedItem.ToString() == cbo_Left.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
                TransferData(3, 4);
            }

            UpdateGUI();
            UpdateTable();

            //m_intCenterGroup_Prev = m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            m_intTopGroup_Prev = m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
            m_intRightGroup_Prev = m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
            m_intBottomGroup_Prev = m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
            m_intLeftGroup_Prev = m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;

        }

        private void cbo_Left_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_blnUpdatingComboBox)
                return;

            int Mask = 0;
            //if (m_intLeftGroup_Prev == m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex)
            //{
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex &= ~0x10;
            //    Mask |= 0x01;
            //}
            if (m_intLeftGroup_Prev == m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex &= ~0x10;
                Mask |= 0x02;
            }
            if (m_intLeftGroup_Prev == m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex &= ~0x10;
                Mask |= 0x04;
            }
            if (m_intLeftGroup_Prev == m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex)
            {
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex &= ~0x10;
                Mask |= 0x08;
            }

            if (Mask > 0)
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex ^= Mask;

            //if (cbo_Left.SelectedItem.ToString() == cbo_Center.SelectedItem.ToString())
            //{
            //    m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            //    m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
            //    TransferData(4, 0);
            //}
            if (cbo_Left.SelectedItem.ToString() == cbo_Top.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
                TransferData(4, 1);
            }
            if (cbo_Left.SelectedItem.ToString() == cbo_Right.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
                TransferData(4, 2);
            }
            if (cbo_Left.SelectedItem.ToString() == cbo_Bottom.SelectedItem.ToString())
            {
                m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
                m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex |= m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;
                TransferData(4, 3);
            }

            UpdateGUI();
            UpdateTable();

            //m_intCenterGroup_Prev = m_smVisionInfo.g_arrPad[0].ref_intColorPadGroupIndex;
            m_intTopGroup_Prev = m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex;
            m_intRightGroup_Prev = m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex;
            m_intBottomGroup_Prev = m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex;
            m_intLeftGroup_Prev = m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex;

        }
        private void SaveControlSettings(string strFolderPath)
        {
            XmlParser objFile;
            objFile = new XmlParser(strFolderPath + "ControlSetting.xml");
            objFile.WriteSectionElement("ControlSetting", false);
            objFile.WriteElement1Value("ControlMask", m_smVisionInfo.g_intOptionControlMask, "Control Mask", true);
            objFile.WriteElement1Value("ControlMask2", m_smVisionInfo.g_intOptionControlMask2, "Control Mask 2", true);
            objFile.WriteElement1Value("ControlMask3", m_smVisionInfo.g_intOptionControlMask3, "Control Mask 3", true);
            objFile.WriteElement1Value("ControlMask4", m_smVisionInfo.g_intOptionControlMask4, "Control Mask 4", true);
            objFile.WriteElement1Value("ControlMask5", m_smVisionInfo.g_intOptionControlMask5, "Control Mask 5", true);
            objFile.WriteElement1Value("PkgControlMask", m_smVisionInfo.g_intPkgOptionControlMask, "Package Control Mask", true);
            objFile.WriteElement1Value("PkgControlMask2", m_smVisionInfo.g_intPkgOptionControlMask2, "Package Control Mask 2", true);
            objFile.WriteElement1Value("LeadControlMask", m_smVisionInfo.g_intLeadOptionControlMask, "Lead Control Mask", true);
            objFile.WriteEndElement();
        }
    }
}
