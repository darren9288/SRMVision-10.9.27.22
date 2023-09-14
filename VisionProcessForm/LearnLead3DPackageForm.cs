using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using SharedMemory;
using VisionProcessing;
using Microsoft.Win32;
using System.IO;

namespace VisionProcessForm
{
    public partial class LearnLead3DPackageForm : Form
    {
        #region Member Variables
        private Point m_pPkgTop, m_pPkgRight, m_pPkgBottom, m_pPkgLeft;
        private Point m_pChippedTop, m_pChippedRight, m_pChippedBottom, m_pChippedLeft;
        private Point m_pMoldTop, m_pMoldRight, m_pMoldBottom, m_pMoldLeft;
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitGaugeAngle = false;
        private bool m_blnInitDone = false;
        private bool m_blnDisablePkgGaugeSetting = true;    // For advance setting
        private int m_intDisplayStep;
        private int m_intSelectedROIPrev = -1;
        private int m_intUserGroup;
        private float m_fStartPixelFromEdgePrev = -1;
        private float m_fStartPixelFromRightPrev = -1;
        private float m_fStartPixelFromBottomPrev = -1;
        private float m_fStartPixelFromLeftPrev = -1;
        private float m_fStartPixelFromEdgePrev_Chipped = -1;
        private float m_fStartPixelFromRightPrev_Chipped = -1;
        private float m_fStartPixelFromBottomPrev_Chipped = -1;
        private float m_fStartPixelFromLeftPrev_Chipped = -1;
        private float m_fStartPixelFromEdgePrev_Mold = -1;
        private float m_fStartPixelFromRightPrev_Mold = -1;
        private float m_fStartPixelFromBottomPrev_Mold = -1;
        private float m_fStartPixelFromLeftPrev_Mold = -1;
        private string m_strFolderPath = "";
        private string m_strFolderPathPkg = "";
        private string m_strSelectedRecipe;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        Lead3DLineProfileForm m_objLead3DLineProfileForm;
        
        #endregion

        public LearnLead3DPackageForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            
            m_intDisplayStep = 1;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\";
            m_strFolderPathPkg = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
              "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\";


            InitializeComponent();
            
                pnl_Thickness.Visible = false;

            m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage = false;
            DisableField2();
            CustomizeGUI();

            m_pPkgTop = new Point(pnl_PkgTop.Location.X, pnl_PkgTop.Location.Y);
            m_pPkgRight = new Point(pnl_PkgRight.Location.X, pnl_PkgRight.Location.Y);
            m_pPkgBottom = new Point(pnl_PkgBottom.Location.X, pnl_PkgBottom.Location.Y);
            m_pPkgLeft = new Point(pnl_PkgLeft.Location.X, pnl_PkgLeft.Location.Y);

            m_pChippedTop = new Point(pnl_ChippedTop.Location.X, pnl_ChippedTop.Location.Y);
            m_pChippedRight = new Point(pnl_ChippedRight.Location.X, pnl_ChippedRight.Location.Y);
            m_pChippedBottom = new Point(pnl_ChippedBottom.Location.X, pnl_ChippedBottom.Location.Y);
            m_pChippedLeft = new Point(pnl_ChippedLeft.Location.X, pnl_ChippedLeft.Location.Y);

            m_pMoldTop = new Point(pnl_MoldTop.Location.X, pnl_MoldTop.Location.Y);
            m_pMoldRight = new Point(pnl_MoldRight.Location.X, pnl_MoldRight.Location.Y);
            m_pMoldBottom = new Point(pnl_MoldBottom.Location.X, pnl_MoldBottom.Location.Y);
            m_pMoldLeft = new Point(pnl_MoldLeft.Location.X, pnl_MoldLeft.Location.Y);

            m_blnInitDone = true;
        }

        private void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "Save Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
                btn_GaugeSaveClose.Enabled = false;
            }
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Package";
            string strChild2 = "Learn Page";
            string strChild3 = "Save Button";

            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild1, strChild2, strChild3))
            {
                btn_Save.Enabled = false;
            }

        }

        /// <summary>
        /// Add search ROI
        /// </summary>
        /// <param name="arrROIs">ROI</param>
        private void AddSearchROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (i != 0)
                    break;
                if (arrROIs[i].Count == 0)
                {
                    if (i == 0)
                    {
                        objROI = new ROI("Center Lead ROI", 1);
                        int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 80;
                        int intPositionY = (480 / 2) - 80;
                        objROI.LoadROISetting(intPositionX, intPositionY, 170, 170);
                    }
                    else if (i == 1)
                    {
                        objROI = new ROI("T", 1);
                        int intPostX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 65;
                        int intPostY = (480 / 2) - 180;
                        objROI.LoadROISetting(intPostX, intPostY, 150, 50);
                    }
                    else if (i == 2)
                    {
                        objROI = new ROI("R", 1);
                        int intPostXR = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) + 120;
                        int intPostYR = (480 / 2) - 70;
                        objROI.LoadROISetting(intPostXR, intPostYR, 50, 150);
                    }
                    else if (i == 3)
                    {
                        objROI = new ROI("B", 1);
                        int intPostXB = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 65;
                        int intPostYB = (480 / 2) + 120;
                        objROI.LoadROISetting(intPostXB, intPostYB, 150, 50);
                    }
                    else
                    {
                        objROI = new ROI("L", 1);
                        int intPostXL = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 170;
                        int intPostYL = (480 / 2) - 70;
                        objROI.LoadROISetting(intPostXL, intPostYL, 50, 150);
                    }

                    objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                    arrROIs[i].Add(objROI);
                }
            }
        }
        /// <summary>
        /// Add search ROI (center, top, right, bottom, left train roi)
        /// </summary>
        /// <param name="arrROIs">ROI</param>
        private void AddTrainUnitROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (i != 0)
                    break;
                if (arrROIs[i].Count == 0)
                {
                    SRMMessageBox.Show("Search ROI no exist!");
                    return;
                }

                if (arrROIs[i].Count == 1)
                {
                    if (i == 0)
                        objROI = new ROI("Gauge ROI", 2);
                    else if (i == 1)
                        objROI = new ROI("T", 2);
                    else if (i == 2)
                        objROI = new ROI("R", 2);
                    else if (i == 3)
                        objROI = new ROI("B", 2);
                    else
                        objROI = new ROI("L", 2);

                    arrROIs[i].Add(objROI);
                    arrROIs[i][1].AttachImage(arrROIs[i][0]);
                }
            }
        }

        private void AddTrainPackageROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (i != 0)
                    break;
                if (arrROIs[i].Count == 0)
                {
                    SRMMessageBox.Show("Search ROI no exist!");
                    return;
                }
                
                if (arrROIs[i].Count == 1)
                {
                    objROI = new ROI("Pkg ROI", 2);
                    arrROIs[i].Add(objROI);
                }

                arrROIs[i][1].AttachImage(arrROIs[i][0]);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    m_smVisionInfo.g_arrLeadROIs[i][1].LoadROISetting(
     (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].ref_pCornerPoint_Center.X -
     (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) -
       m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX),
     (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y -
     (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) -
      m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY),
     (int)Math.Ceiling(m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth),
     (int)Math.Ceiling(m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight));
                else
                    m_smVisionInfo.g_arrLeadROIs[i][1].LoadROISetting(
    (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
    (m_smVisionInfo.g_arrLead3D[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
      m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX),
    (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
    (m_smVisionInfo.g_arrLead3D[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
     m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY),
    (int)Math.Ceiling(m_smVisionInfo.g_arrLead3D[i].GetResultMaxWidth_RectGauge4L(0)),
    (int)Math.Ceiling(m_smVisionInfo.g_arrLead3D[i].GetResultMaxHeight_RectGauge4L(0)));
               
            }
        }

        private void AttachImageToROI(List<List<ROI>> arrROIs, ImageDrawing objImage)
        {
            for (int i = 0; i < arrROIs.Count; i++)
            {
                //if (i != 0)
                //    break;
                for (int j = 0; j < arrROIs[i].Count; j++)
                {
                    ROI objROI = arrROIs[i][j];

                    switch (objROI.ref_intType)
                    {
                        case 1:
                            objROI.AttachImage(objImage);
                            break;
                        case 2:
                            objROI.AttachImage(arrROIs[i][0]);
                            break;
                        case 3:
                            objROI.AttachImage(arrROIs[i][1]);
                            break;
                    }

                }
            }
        }
        /// <summary>
        /// Load Lead settings from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        private void LoadLeadSetting(string strPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
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
                
                // Load lead Template Setting
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

                m_smVisionInfo.g_arrLead3D[i].LoadLead3D(strPath + "Template\\Template.xml", strSectionName);

                if (i == 0)
                    m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.LoadRectGauge4L(strPath + "RectGauge4L.xml",
                                                              "Lead3D" + i.ToString(), false);
            }
        }

        private void LoadROISetting(string strPath, List<List<ROI>> arrROIs, int intROICount)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            ROI objROI;

            for (int i = 0; i < intROICount; i++)
            {
                arrROIs.Add(new List<ROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new ROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
                    objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    if (objROI.ref_intType > 1)
                    {
                        objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                        objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    }

                    arrROIs[i].Add(objROI);
                }
            }
        }
        private void SaveROISetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "ROI.xml", false);

            ROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrLeadROIs.Count; t++)
            {
                if (!m_smVisionInfo.g_arrLead3D[t].ref_blnSelected)
                    continue;

                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrLeadROIs[t][j];
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
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3D ROI", m_smProductionInfo.g_strLotID);
            
        }
        private bool WantSaveImageAccordingMergeType(int intImageIndex)
        {
            switch (m_smVisionInfo.g_intImageMergeType)
            {
                case 0:
                    return true;
                    break;
                case 1:
                    if (intImageIndex == 1)
                        return false;
                    break;
                case 2:
                    if ((intImageIndex == 1) || (intImageIndex == 2))
                        return false;
                    break;
                case 3:
                    if ((intImageIndex == 1) || (intImageIndex == 3))
                        return false;
                    break;
                case 4:
                    if ((intImageIndex == 1) || (intImageIndex == 2) || (intImageIndex == 4))
                        return false;
                    break;
            }

            return true;
        }
        /// <summary>
        /// Save all lead settings 
        /// </summary>
        /// <param name="strFolderPath">xml folder path</param>
        private void SaveLeadSetting(string strFolderPath)
        {
          
            #region Save Template Data and Images

            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Lead3DPkg\\";
            

            if (File.Exists(m_strFolderPath + "Template\\OriTemplate.bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3DPkg", "OriTemplate.bmp", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3DPkg", "", "OriTemplate.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            
            m_objCopy.CopyAllImageFiles(m_strFolderPathPkg + "Template\\", strPath + "Old\\");

            // Save Learn Actual Image
            m_smVisionInfo.g_arrImages[0].SaveImage(m_strFolderPathPkg + "Template\\OriTemplate.bmp");
            if ((m_smVisionInfo.g_arrImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                m_smVisionInfo.g_arrImages[1].SaveImage(m_strFolderPathPkg + "Template\\OriTemplate_Image1.bmp");
            if ((m_smVisionInfo.g_arrImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                m_smVisionInfo.g_arrImages[2].SaveImage(m_strFolderPathPkg + "Template\\OriTemplate_Image2.bmp");
            if ((m_smVisionInfo.g_arrImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                m_smVisionInfo.g_arrImages[3].SaveImage(m_strFolderPathPkg + "Template\\OriTemplate_Image3.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 4 && WantSaveImageAccordingMergeType(4))
                m_smVisionInfo.g_arrImages[4].SaveImage(m_strFolderPathPkg + "Template\\OriTemplate_Image4.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 5 && WantSaveImageAccordingMergeType(5))
                m_smVisionInfo.g_arrImages[5].SaveImage(m_strFolderPathPkg + "Template\\OriTemplate_Image5.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 6 && WantSaveImageAccordingMergeType(6))
                m_smVisionInfo.g_arrImages[6].SaveImage(m_strFolderPathPkg + "Template\\OriTemplate_Image6.bmp");

            m_objCopy.CopyAllImageFiles(m_strFolderPathPkg + "Template\\", strPath + "New\\");

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
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

                SaveROISetting(strFolderPath);
                m_smVisionInfo.g_arrLead3D[i].SaveLead3D(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);

                if (i == 0)
                    m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.SaveRectGauge4L(
                   strFolderPath + "RectGauge4L.xml",
                   false,
                   "Lead3D" + i.ToString(),
                   true,
                   true);

                // Save Learn Template Search ROI image, unit image
                m_smVisionInfo.g_arrLead3D[i].SaveLeadPackageTemplateImage(m_strFolderPathPkg + "Template\\",
                                                                m_smVisionInfo.g_arrRotatedImages[0],
                                                                m_smVisionInfo.g_arrLeadROIs[i], i);
            }

            //Load Package Image
            for (int intLeadIndex = 0; intLeadIndex < m_smVisionInfo.g_arrLead3D.Length; intLeadIndex++)
            {
                m_smVisionInfo.g_arrLead3D[intLeadIndex].LoadLeadTemplateImage(strFolderPath + "Template\\", intLeadIndex);
                m_smVisionInfo.g_arrLead3D[intLeadIndex].LoadLeadPackageTemplateImage(m_strFolderPathPkg + "Template\\", intLeadIndex);
            }
            #endregion
        }
        /// <summary>
        /// Setup each learning steps
        /// </summary>
        private void SetupSteps(bool blnForward)
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: //Define Unit ROI (Package)
                    btn_GaugeSaveClose.Visible = true;
                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    {
                        m_smVisionInfo.g_intSelectedImage = 0;
                    }
                    else
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0);
                    AddSearchROI(m_smVisionInfo.g_arrLeadROIs); // Add Search ROI
                    RotateImage();
                    if (!m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                        {
                            if (i != 0)
                                break;
                            //PointF pCenter = new PointF(m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalCenterX,
                            //                            m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalCenterY);

                            m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrLeadROIs[i][0]);
                            m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();

                        }
                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                        {
                            if (m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_intCheckLeadDimensionImageIndex = m_smVisionInfo.g_intSelectedImage;
                                m_smVisionInfo.g_arrLead3D[i].ref_intGaugeSizeImageIndex = m_smVisionInfo.g_intSelectedImage;
                            }
                        }

                        m_smVisionInfo.g_blnViewRotatedImage = false;
                        ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

                        // Clear ROI drag handler
                        for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                        {
                            if (m_smVisionInfo.g_arrLeadROIs[i].Count >= 2)
                                if (m_smVisionInfo.g_arrLeadROIs[i][1].GetROIHandle())
                                    m_smVisionInfo.g_arrLeadROIs[i][1].ClearDragHandle();
                        }
                    }
                    AddTrainUnitROI(m_smVisionInfo.g_arrLeadROIs);
                    if (!m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    {
                        switch (m_smVisionInfo.g_arrLead3D[0].ref_objRectGauge4L.GetGaugeImageMode(0))
                        {
                            default:
                            case 0:
                                {
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrLead3D[0].ref_fImageGain);
                                }
                                break;
                            case 1:
                                {
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, m_smVisionInfo.g_arrLead3D[0].ref_fImageGain);
                                    m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                                }
                                break;
                        }

                        AttachImageToROI(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_blnViewPackageImage = true;
                    }
                    if (blnForward)
                    {
                        // Set RectGauge4L Placement
                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                        {
                            if (i != 0)
                                break;

                            //PointF pCenter = new PointF(m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalCenterX,
                            //                            m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalCenterY);
                            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            {
                                m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();

                                m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.ResetGaugeSettingToUserVariables();

                                bool blnGaugeResult;
                                if (m_smVisionInfo.g_blnViewPackageImage)
                                    blnGaugeResult = m_smVisionInfo.g_arrLead3D[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objWhiteImage);
                                else
                                    blnGaugeResult = m_smVisionInfo.g_arrLead3D[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_objWhiteImage);

                                if (!blnGaugeResult)
                                {
                                    m_smVisionInfo.g_strErrorMessage = "*" + m_smVisionInfo.g_arrLead3D[i].ref_strErrorMessage;
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                            }
                        }
                    }

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    if (!m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                        m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;
                    if (!m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    {
                        m_smVisionInfo.g_blnViewRotatedImage = false;
                    }

                    lbl_StepNo.BringToFront();
                    lbl_TitleStep1.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Step1;
                    btn_Previous.Enabled = false;

                    break;
                case 1: // Define Package ROI
                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                        m_smVisionInfo.g_intSelectedImage = 0;
                    else
                        m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0);
                    bool blnGaugeOK = true;
                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (i != 0)
                            break;

                        if (!m_smVisionInfo.g_arrLead3D[i].IsGaugeMeasureOK(i) && !m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                        {
                            string strDirectionName;
                            switch (i)
                            {
                                case 0:
                                default:
                                    strDirectionName = "Center";
                                    break;
                                case 1:
                                    strDirectionName = "Top";
                                    break;
                                case 2:
                                    strDirectionName = "Right";
                                    break;
                                case 3:
                                    strDirectionName = "Bottom";
                                    break;
                                case 4:
                                    strDirectionName = "Left";
                                    break;

                            }
                            SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");
                            m_smVisionInfo.g_intLearnStepNo--;
                            //m_intDisplayStepNo--;
                            blnGaugeOK = false;
                            break;
                        }
                    }

                    if (!blnGaugeOK)
                    {
                        break;
                    }


                    btn_GaugeSaveClose.Visible = false;

                    if (blnForward)
                    {
                        // Set last gauge measurement size as Unit size template
                        SetLastGaugeMeasureSizeAsTemplateUnitSize();
                    }

                    RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                    // Add Package ROI
                    AddTrainPackageROI(m_smVisionInfo.g_arrLeadROIs);

                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;

                    lbl_TitleStep2.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Step2;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 2: // Define Chipped ROI
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);

                    if (blnForward)
                    {
                        // Set last gauge measurement size as Unit size template
                        if (!m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            SetLastGaugeMeasureSizeAsTemplateUnitSize();
                    }

                    RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 

                    
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;

                    lbl_TitleStep10.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Step2_1;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 3: // Define Mold Flash ROI
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);


                    if (blnForward)
                    {
                        // Set last gauge measurement size as Unit size template
                        SetLastGaugeMeasureSizeAsTemplateUnitSize();
                    }

                    RotateImage();  // Note: need rotate image as well when user press previouse button to this step bcos image has been modify during step 4. 
                    
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnUpdateSelectedROI = true;

                    lbl_TitleStep11.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Step2_2;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 4:// Learn Image 1 surface Threshold
                    label5.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage1LowSurfaceThreshold.ToString();
                    label4.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage1HighSurfaceThreshold.ToString();
                    if (blnForward)
                    {
                        float fUnitWidth;
                        float fUnitHeight;
                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                        {
                            if (i != 0)
                                break;

                            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                                fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                            else
                                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                                fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                            else
                                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                           
                            //if (fUnitWidth < fUnitHeight)
                            //{
                            if (Convert.ToSingle(txt_StartPixelFromLeft.Text) >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4))// * m_smVisionInfo.g_fCalibPixelX)
                            {
                                SRMMessageBox.Show("Edge tolerance can only goto a quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                txt_StartPixelFromLeft.Text = "0";
                                m_fStartPixelFromLeftPrev = 0;

                                for (int j = 0; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                                {
                                    m_smVisionInfo.g_arrLead3D[j].ref_fPkgStartPixelFromLeft = 0;
                                }
                                break;
                            }
                            if (Convert.ToSingle(txt_StartPixelFromRight.Text) >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4))// * m_smVisionInfo.g_fCalibPixelX)
                            {
                                SRMMessageBox.Show("Edge tolerance can only goto a quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                txt_StartPixelFromRight.Text = "0";
                                m_fStartPixelFromRightPrev = 0;
                                for (int j = 0; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                                {
                                    m_smVisionInfo.g_arrLead3D[j].ref_fPkgStartPixelFromRight = 0;
                                }
                                break;
                            }
                            //}
                            //else
                            //{
                            if (Convert.ToSingle(txt_StartPixelFromEdge.Text) >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4) )//* m_smVisionInfo.g_fCalibPixelY)
                            {
                                SRMMessageBox.Show("Edge tolerance can only goto a quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                txt_StartPixelFromEdge.Text = "0";
                                m_fStartPixelFromEdgePrev = 0;

                                for (int j = 0; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                                {
                                    m_smVisionInfo.g_arrLead3D[j].ref_fPkgStartPixelFromEdge = 0;
                                }
                                break;
                            }
                            if (Convert.ToSingle(txt_StartPixelFromBottom.Text) >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4))// * m_smVisionInfo.g_fCalibPixelY)
                            {
                                SRMMessageBox.Show("Edge tolerance can only goto a quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                txt_StartPixelFromBottom.Text = "0";
                                m_fStartPixelFromBottomPrev = 0;

                                for (int j = 0; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                                {
                                    m_smVisionInfo.g_arrLead3D[j].ref_fPkgStartPixelFromBottom = 0;
                                }
                                break;
                            }
                            //}
                        }
                    }

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[0]);

                    lbl_TitleStep3.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Step3;
                    if (m_smVisionInfo.g_arrImages.Count == 1)
                    {
                        btn_Next.Enabled = false;
                        tp_Step3.Controls.Add(btn_Save);
                    }
                    else
                        btn_Next.Enabled = true;
                    break;
                case 5:// Learn Image 2 Threshold
                   
                    label3.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage2LowThreshold.ToString();
                    label2.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage2HighThreshold.ToString();
                    //m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(1);
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2);
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    lbl_TitleStep4.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Step4;
                    btn_Next.Enabled = true;
                    break;
                case 6:// Learn Mold Flash Threshold
                    label1.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage1MoldFlashThreshold.ToString();
                    //m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(0); // (1)
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    lbl_TitleStep5.BringToFront();
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage && !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        lbl_TitleStep5.Text = srmLabel53.Text;
                    }
                    pic_Image3Picture.SelectedTab = tp_Step5;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    break;
                case 7:// Learn void Threshold
                    lbl_VoidViewThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2VoidThreshold.ToString();
                    //m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(1);
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2);
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    lbl_TitleStep8.BringToFront();

                    pic_Image3Picture.SelectedTab = tp_Step6;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                    {
                        lbl_TitleStep8.Text = srmLabel54.Text;
                        tp_Step6.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    break;
                case 8:// Learn Crack Threshold
                    lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage2LowCrackThreshold.ToString();
                    lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage2HighCrackThreshold.ToString();

                    //m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(1);
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2);
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    lbl_TitleStep9.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Step7;
                    btn_Previous.Enabled = true;
                    if (m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                    {
                        btn_Next.Enabled = false;
                    }
                    else
                    {
                        if (tp_Step7.Controls.Contains(btn_Save))
                            tp_Step7.Controls.Remove(btn_Save);
                    }
                    break;
                case 9:// Learn Bright Threshold Simple
                    txt_BrightMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldMinArea.ToString();
                    lbl_BrightThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldLowThreshold.ToString();
                    //m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(0); // (1)
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    lbl_TitleBrightFieldSimple.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Bright;

                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    break;
                case 10:// Learn Dark Threshold Simple
                    txt_DarkMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldMinArea.ToString();
                    lbl_DarkThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldLowThreshold.ToString();
                    //m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(1);
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2);
                    ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    lbl_TitleDarkFieldSimple.BringToFront();
                    pic_Image3Picture.SelectedTab = tp_Dark;

                    btn_Previous.Enabled = true;
                    tp_Dark.Controls.Add(btn_Save);
                    btn_Next.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void SetLastGaugeMeasureSizeAsTemplateUnitSize()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;
                m_smVisionInfo.g_arrLead3D[i].SetCurrentMeasureSizeAsUnitSize();
            }
        }

        private void RotateImage()
        {
            if (m_smVisionInfo.g_intSelectedImage == -1)
            {
                
                if (true)
                {
                    // Define rotation ROI with ROI center point same as gauge unit center point
                    float fROIWidth = 0;
                    float fROIHeight = 0;

                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                        fROIWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                    else
                        fROIWidth = Math.Min(m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX,
                                     m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().X);

                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                        fROIHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                    else
                        fROIHeight = Math.Min(m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY,
                                          m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().Y);




                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                        objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));
                    else
                        objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                                                           (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));

                    // Rotate image to zero degree using RectGauge4L angle result
                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                        ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, 
                                      m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultAngle(),
                    ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    else
                        ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], objRotateROI,
                                  m_smVisionInfo.g_arrLead3D[0].GetResultAngle_RectGauge4L(),
                ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);

                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
                else
                {
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    if (i != 0)
                        break;
                    if (true)
                    {
                     
                        m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                        int intRotateCenterX = 0;
                        int intRotateCenterY = 0;

                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            intRotateCenterX = (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].ref_pCornerPoint_Center.X, 0, MidpointRounding.AwayFromZero);
                        else
                            intRotateCenterX = (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].GetResultCenterPoint_RectGauge4L().X, 0, MidpointRounding.AwayFromZero);

                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            intRotateCenterY = (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].ref_pCornerPoint_Center.Y, 0, MidpointRounding.AwayFromZero);
                        else
                            intRotateCenterY = (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].GetResultCenterPoint_RectGauge4L().Y, 0, MidpointRounding.AwayFromZero);
                        
                        int intStartX = Math.Max(0, intRotateCenterX - m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth / 2);
                        int intEndX = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageWidth, intRotateCenterX + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth / 2);
                        int intHalfWidth = Math.Min(intRotateCenterX - intStartX, intEndX - intRotateCenterX);
                        intStartX = intRotateCenterX - intHalfWidth;

                        int intStartY = Math.Max(0, intRotateCenterY - m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight / 2);
                        int intEndY = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageHeight, intRotateCenterY + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight / 2);
                        int intHalfHeight = Math.Min(intRotateCenterY - intStartY, intEndY - intRotateCenterY);
                        intStartY = intRotateCenterY - intHalfHeight;

                        ROI objRotateROI = new ROI();
                        objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);    // 2018 10 18 - CCENG: New empty ROI must attach to any image. Without attach any image, the TotalOrgXY value will be 0.
                        objRotateROI.LoadROISetting(intStartX, intStartY, intHalfWidth * 2, intHalfHeight * 2);


                        // Rotate image to zero degree using RectGauge4L angle result
                        if (i == 0)
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrImages.Count; j++)
                            {
                                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[j], objRotateROI, 
                                                  m_smVisionInfo.g_arrLead3D[i].GetUnitPRResultAngle(),
                                ref m_smVisionInfo.g_arrRotatedImages, j);
                                else
                                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[j], objRotateROI,
                                                 m_smVisionInfo.g_arrLead3D[i].GetResultAngle_RectGauge4L(),
                               ref m_smVisionInfo.g_arrRotatedImages, j);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrImages.Count; j++)
                            {
                                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                                    ROI.Rotate0Degree(objRotateROI,
                                               m_smVisionInfo.g_arrLead3D[i].GetUnitPRResultAngle(),
                             ref m_smVisionInfo.g_arrRotatedImages, j);
                                else
                                    ROI.Rotate0Degree(objRotateROI, 
                                              m_smVisionInfo.g_arrLead3D[i].GetResultAngle_RectGauge4L(),
                            ref m_smVisionInfo.g_arrRotatedImages, j);
                            }
                        }
                        
                        m_smVisionInfo.g_blnViewRotatedImage = true;

                        m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    }
                   
                }
            }
        }

        /// <summary>
        /// Customize GUI
        /// </summary>
        private void CustomizeGUI()
        {
            m_blnUpdateSelectedROISetting = true;

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_SetToAll.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_LeadPackage", false));
            chk_SetToAll_Chipped.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_LeadPackage_Chipped", false));
            chk_SetToAll_Mold.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_LeadPackage_Mold", false));

            if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
            {
                if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    srmLabel53.Text = "Image 1 : Bright Field";
                srmLabel54.Text = "Image 2 : Dark Field";
            }

            // Package size
            txt_UnitSizeMinWidth.Text = m_smVisionInfo.g_arrLead3D[0].ref_fUnitWidthMin.ToString("F4");
            txt_UnitSizeMinHeight.Text = m_smVisionInfo.g_arrLead3D[0].ref_fUnitHeightMin.ToString("F4");
            txt_UnitSizeMaxWidth.Text = m_smVisionInfo.g_arrLead3D[0].ref_fUnitWidthMax.ToString("F4");
            txt_UnitSizeMaxHeight.Text = m_smVisionInfo.g_arrLead3D[0].ref_fUnitHeightMax.ToString("F4");
            txt_UnitSizeMinThickness.Text = m_smVisionInfo.g_arrLead3D[0].ref_fUnitThicknessMin.ToString("F4");
            txt_UnitSizeMaxThickness.Text = m_smVisionInfo.g_arrLead3D[0].ref_fUnitThicknessMax.ToString("F4");
            txt_ImageGain.Value = Convert.ToDecimal(m_smVisionInfo.g_arrLead3D[0].ref_fImageGain);
            txt_StartPixelFromEdge.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromEdge).ToString();
            txt_StartPixelFromRight.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromRight).ToString();
            txt_StartPixelFromBottom.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromBottom).ToString();
            txt_StartPixelFromLeft.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromLeft).ToString();

            m_fStartPixelFromEdgePrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromEdge);
            m_fStartPixelFromRightPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromRight);
            m_fStartPixelFromBottomPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromBottom);
            m_fStartPixelFromLeftPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromLeft);

            txt_StartPixelFromEdge_Chipped.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromEdge).ToString();
            txt_StartPixelFromRight_Chipped.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromRight).ToString();
            txt_StartPixelFromBottom_Chipped.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromBottom).ToString();
            txt_StartPixelFromLeft_Chipped.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromLeft).ToString();

            m_fStartPixelFromEdgePrev_Chipped = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromEdge);
            m_fStartPixelFromRightPrev_Chipped = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromRight);
            m_fStartPixelFromBottomPrev_Chipped = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromBottom);
            m_fStartPixelFromLeftPrev_Chipped = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromLeft);

            txt_StartPixelFromEdge_Mold.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromEdge).ToString();
            txt_StartPixelFromRight_Mold.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromRight).ToString();
            txt_StartPixelFromBottom_Mold.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromBottom).ToString();
            txt_StartPixelFromLeft_Mold.Text = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromLeft).ToString();

            m_fStartPixelFromEdgePrev_Mold = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromEdge);
            m_fStartPixelFromRightPrev_Mold = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromRight);
            m_fStartPixelFromBottomPrev_Mold = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromBottom);
            m_fStartPixelFromLeftPrev_Mold = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromLeft);

    
                picUnitROI.Image = ils_ImageListTree.Images[4];

           

            chk_ShowDraggingBox.Checked = true;
            chk_ShowSamplePoints.Checked = true;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }

            m_smVisionInfo.g_intSelectedROI = 0;

            //fix use gauge for measurement
            chk_UseGauge.Checked = true;

            chk_ShowDraggingBox.Enabled = true;
            chk_ShowSamplePoints.Enabled = true;
            btn_GaugeAdvanceSetting.Enabled = true;
           
            m_smVisionInfo.g_blnViewGauge = true;

           

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;
                m_smVisionInfo.g_arrLead3D[i].ref_blnSelected = true;
            }

            m_blnUpdateSelectedROISetting = false;
        }

        private void UpdateGUI()
        {
            m_smVisionInfo.g_intSelectedROI = 0;
            m_blnUpdateSelectedROISetting = true;
            
            // Package size
            txt_UnitSizeMinWidth.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fUnitWidthMin.ToString("F4");
            txt_UnitSizeMinHeight.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fUnitHeightMin.ToString("F4");
            txt_UnitSizeMaxWidth.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fUnitWidthMax.ToString("F4");
            txt_UnitSizeMaxHeight.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fUnitHeightMax.ToString("F4");
            txt_UnitSizeMinThickness.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fUnitThicknessMin.ToString("F4");
            txt_UnitSizeMaxThickness.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fUnitThicknessMax.ToString("F4");
            txt_ImageGain.Value = Convert.ToDecimal(m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fImageGain);
            switch (m_smVisionInfo.g_intSelectedROI)
            {
                case 0:
                    txt_StartPixelFromEdge.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge).ToString();
                    txt_StartPixelFromRight.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight).ToString();
                    txt_StartPixelFromBottom.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom).ToString();
                    txt_StartPixelFromLeft.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft).ToString();

                    m_fStartPixelFromEdgePrev = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge);
                    m_fStartPixelFromRightPrev = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight);
                    m_fStartPixelFromBottomPrev = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom);
                    m_fStartPixelFromLeftPrev = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft);

                    txt_StartPixelFromEdge_Chipped.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge).ToString();
                    txt_StartPixelFromRight_Chipped.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight).ToString();
                    txt_StartPixelFromBottom_Chipped.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom).ToString();
                    txt_StartPixelFromLeft_Chipped.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft).ToString();

                    m_fStartPixelFromEdgePrev_Chipped = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge);
                    m_fStartPixelFromRightPrev_Chipped = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight);
                    m_fStartPixelFromBottomPrev_Chipped = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom);
                    m_fStartPixelFromLeftPrev_Chipped = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft);

                    txt_StartPixelFromEdge_Mold.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge).ToString();
                    txt_StartPixelFromRight_Mold.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight).ToString();
                    txt_StartPixelFromBottom_Mold.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom).ToString();
                    txt_StartPixelFromLeft_Mold.Text = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft).ToString();

                    m_fStartPixelFromEdgePrev_Mold = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge);
                    m_fStartPixelFromRightPrev_Mold = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight);
                    m_fStartPixelFromBottomPrev_Mold = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom);
                    m_fStartPixelFromLeftPrev_Mold = (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft);
                    break;
            }
            m_blnUpdateSelectedROISetting = false;
        }

        private int GetArrayImageIndex(int intUserSelectImageIndex)
        {
            if (intUserSelectImageIndex < 0)
                return 0;

            switch (m_smVisionInfo.g_intImageMergeType)
            {
                default:
                case 0: // No merge
                    {
                        return intUserSelectImageIndex;
                    }
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;
                        else if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                            return intUserSelectImageIndex;
                        else
                            return (intUserSelectImageIndex + 1);
                    }
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;
                        else if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                            return intUserSelectImageIndex;
                        else if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                            return (intUserSelectImageIndex + 1);
                        else
                            return (intUserSelectImageIndex + 2);
                    }
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;

                        if (intUserSelectImageIndex == 2) // select image 2 which is grab 5
                        {
                            if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 2);
                        }
                        else // select image 1 which is grab 3 and 4
                        {
                            if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 1);
                        }
                    }
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 4 center and grab 5 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;

                        if (intUserSelectImageIndex == 1) // select image 1 which is grab 4 and 5
                        {
                            if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 2);
                        }
                        else // select other than image 0 or 1
                        {
                            if (intUserSelectImageIndex + 3 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 3);
                        }
                    }
            }
        }


        private void txt_StartPixelFromEdge_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;

            // int intEdgeDistance = (int)Math.Round(Convert.ToSingle(fStartPixelFromEdge) * m_smVisionInfo.g_fCalibPixelY, 0, MidpointRounding.AwayFromZero);


            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;


                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if (fUnitWidth < fUnitHeight)
                //{
                //    if (fStartPixelFromEdge >= (fUnitWidth / 4))
                //    {
                //        SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //        txt_StartPixelFromEdge.Text = m_fStartPixelFromEdgePrev.ToString("F4");
                //        return;
                //    }
                //}
                //else
                //{
                if ((i == 0 ) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4))// * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
             
                //}
                if (chk_SetToAll.Checked)
                {
                    txt_StartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;
                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromEdge = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromLeft = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromRight = fStartPixelFromEdge;
                        }
                        break;
                    
                }
           

                m_smVisionInfo.g_arrLeadROIs[i][1].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);

            }
           
            m_fStartPixelFromEdgePrev = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromRightPrev = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev = fStartPixelFromEdge;
            }

            
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromRight = 0;
            if (!float.TryParse(txt_StartPixelFromRight.Text, out fStartPixelFromRight))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight.Text == "" || fStartPixelFromRight < 0)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                // if (fUnitWidth < fUnitHeight)
                // {
                if ((i == 0) && fStartPixelFromRight >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4) )//* m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
             
                //  }
          
                if (chk_SetToAll.Checked)
                {
                    txt_StartPixelFromEdge.Text = fStartPixelFromRight.ToString();
                    txt_StartPixelFromBottom.Text = fStartPixelFromRight.ToString();
                    txt_StartPixelFromLeft.Text = fStartPixelFromRight.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;
                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromRight = fStartPixelFromRight;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromBottom = fStartPixelFromRight;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromLeft = fStartPixelFromRight;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromEdge = fStartPixelFromRight;
                        }
                        break;
                }
               
                m_smVisionInfo.g_arrLeadROIs[i][1].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
            }

        

            m_fStartPixelFromRightPrev = fStartPixelFromRight;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromEdgePrev = fStartPixelFromRight;
                m_fStartPixelFromBottomPrev = fStartPixelFromRight;
                m_fStartPixelFromLeftPrev = fStartPixelFromRight;
            }

            
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromBottom = 0;
            if (!float.TryParse(txt_StartPixelFromBottom.Text, out fStartPixelFromBottom))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom.Text == "" || fStartPixelFromBottom < 0)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if ((i == 0 || i == 1 || i == 3) && fStartPixelFromBottom >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4))// * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && fStartPixelFromBottom >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4))// * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
        
                if (chk_SetToAll.Checked)
                {
                    txt_StartPixelFromEdge.Text = fStartPixelFromBottom.ToString();
                    txt_StartPixelFromRight.Text = fStartPixelFromBottom.ToString();
                    txt_StartPixelFromLeft.Text = fStartPixelFromBottom.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromBottom = fStartPixelFromBottom;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromEdge = fStartPixelFromBottom;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromLeft = fStartPixelFromBottom;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromRight = fStartPixelFromBottom;
                        }
                        break;
                 
                }
              
                m_smVisionInfo.g_arrLeadROIs[i][1].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
            }

          

            m_fStartPixelFromBottomPrev = fStartPixelFromBottom;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromRightPrev = fStartPixelFromBottom;
                m_fStartPixelFromEdgePrev = fStartPixelFromBottom;
                m_fStartPixelFromLeftPrev = fStartPixelFromBottom;
            }
           
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromLeft = 0;
            if (!float.TryParse(txt_StartPixelFromLeft.Text, out fStartPixelFromLeft))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft.Text == "" || fStartPixelFromLeft < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromLeft >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4))// * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
               
                if (chk_SetToAll.Checked)
                {
                    txt_StartPixelFromEdge.Text = fStartPixelFromLeft.ToString();
                    txt_StartPixelFromRight.Text = fStartPixelFromLeft.ToString();
                    txt_StartPixelFromBottom.Text = fStartPixelFromLeft.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromLeft = fStartPixelFromLeft;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromEdge = fStartPixelFromLeft;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromBottom = fStartPixelFromLeft;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromRight = fStartPixelFromLeft;
                        }
                        break;
                    
                }
            
                m_smVisionInfo.g_arrLeadROIs[i][1].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
            }

         

            m_fStartPixelFromLeftPrev = fStartPixelFromLeft;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromRightPrev = fStartPixelFromLeft;
                m_fStartPixelFromEdgePrev = fStartPixelFromLeft;
                m_fStartPixelFromBottomPrev = fStartPixelFromLeft;
            }
       
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            LoadROISetting(m_strFolderPath + "ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead3D.Length);
            LoadLeadSetting(m_strFolderPath);

            AttachImageToROI(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[0]);

            DialogResult = DialogResult.Cancel;
            Close();
            Dispose();
        }

        private void btn_RotateUnit_Click(object sender, EventArgs e)
        {

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
        }

        private bool CheckSetting()
        {
            if (Convert.ToDecimal(txt_UnitSizeMinWidth.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMinHeight.Text) <= 0 ||
                   Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= 0)
            {
                SRMMessageBox.Show("Width or Height Min Max setting cannot be zero.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            if (Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= Convert.ToDecimal(txt_UnitSizeMinWidth.Text))
            {
                SRMMessageBox.Show("Min Width cannot be larger than or equal to Max Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= Convert.ToDecimal(txt_UnitSizeMinHeight.Text))
            {
                SRMMessageBox.Show("Min Height cannot be larger than or equal to Max Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (Convert.ToDecimal(txt_UnitSizeMaxWidth.Text) <= Convert.ToDecimal(lbl_SizeWidthResult.Text))
            {
                SRMMessageBox.Show("Max Width cannot be smaller than or equal to measured Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (Convert.ToDecimal(txt_UnitSizeMaxHeight.Text) <= Convert.ToDecimal(lbl_SizeHeightResult.Text))
            {
                SRMMessageBox.Show("Max Height cannot be smaller than or equal to measured Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (Convert.ToDecimal(txt_UnitSizeMinWidth.Text) >= Convert.ToDecimal(lbl_SizeWidthResult.Text))
            {
                SRMMessageBox.Show("Min Width cannot be larger than or equal to measured Width.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (Convert.ToDecimal(txt_UnitSizeMinHeight.Text) >= Convert.ToDecimal(lbl_SizeHeightResult.Text))
            {
                SRMMessageBox.Show("Min Height cannot be larger than or equal to measured Height.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (pnl_Thickness.Visible)
            {
                if (Convert.ToDecimal(txt_UnitSizeMinThickness.Text) <= 0 || Convert.ToDecimal(txt_UnitSizeMaxThickness.Text) <= 0)
                {
                    SRMMessageBox.Show("Thickness Min Max setting cannot be zero.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

                if (Convert.ToDecimal(txt_UnitSizeMaxThickness.Text) <= Convert.ToDecimal(txt_UnitSizeMinThickness.Text))
                {
                    SRMMessageBox.Show("Min Thickness cannot be larger than or equal to Max Thickness.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
                if (lbl_SizeThicknessResult.Text == "NaN")
                {
                    SRMMessageBox.Show("Thickness Measurement is not valid.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
                if (Convert.ToDecimal(txt_UnitSizeMaxThickness.Text) <= Convert.ToDecimal(lbl_SizeThicknessResult.Text))
                {
                    SRMMessageBox.Show("Max Thickness cannot be smaller than or equal to measured Thickness.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
                if (Convert.ToDecimal(txt_UnitSizeMinThickness.Text) >= Convert.ToDecimal(lbl_SizeThicknessResult.Text))
                {
                    SRMMessageBox.Show("Min Thickness cannot be larger than or equal to measured Thickness.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            return true;
        }
        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo == 0)
            {
               if(!CheckSetting())
                    return;
            }

            if (false)//m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage) //temporary use simple mode
            {
                if (m_smVisionInfo.g_intLearnStepNo < 10)
                    m_smVisionInfo.g_intLearnStepNo++;

                //if (m_smVisionInfo.g_intLearnStepNo == 2)
                //    if (m_smVisionInfo.g_arrImages.Count < 3)
                //    {
                //        m_smVisionInfo.g_intLearnStepNo++;
                //    }

                //if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                //{
                //    if (m_smVisionInfo.g_intLearnStepNo == 4)
                //        m_smVisionInfo.g_intLearnStepNo = 6;
                //}

            }
            else
            {
                if (m_smVisionInfo.g_intLearnStepNo < 12)
                    m_smVisionInfo.g_intLearnStepNo++;

                if (m_smVisionInfo.g_intLearnStepNo == 2 &&
                    !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
                    !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting &&
                    !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 9;

                if (m_smVisionInfo.g_intLearnStepNo == 2 &&
             !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
             !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                {
                    m_smVisionInfo.g_intLearnStepNo = 8;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 3 &&
             m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
             !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                {
                    m_smVisionInfo.g_intLearnStepNo = 4;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 2 &&
          !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
                {
                    m_smVisionInfo.g_intLearnStepNo = 3;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 4 &&
         !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
                {
                    m_smVisionInfo.g_intLearnStepNo = 6;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 7 &&
       !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                {
                    m_smVisionInfo.g_intLearnStepNo = 9;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 7 &&
   m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                {
                    m_smVisionInfo.g_intLearnStepNo = 8;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 6 &&
            !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting &&
            !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                {
                    m_smVisionInfo.g_intLearnStepNo = 9;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 6 &&
         m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting &&
            !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                {
                    m_smVisionInfo.g_intLearnStepNo = 8;
                }
            }

            m_intDisplayStep++;
            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStep.ToString() + ":";

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo > 0)
                m_smVisionInfo.g_intLearnStepNo--;

            //if (m_smVisionInfo.g_intLearnStepNo == 2)
            //    if (m_smVisionInfo.g_arrImages.Count < 3)
            //    {
            //        m_smVisionInfo.g_intLearnStepNo--;
            //    }

            if (true)//!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage) //Temporary use simple mode
            {
                //if (m_smVisionInfo.g_intLearnStepNo == 5)
                //    m_smVisionInfo.g_intLearnStepNo = 3;

                if (m_smVisionInfo.g_intLearnStepNo == 8 &&
                   !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
                   !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting &&
                   !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 1;

                if (m_smVisionInfo.g_intLearnStepNo == 7 &&
                  !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
                  !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 1;

                if (m_smVisionInfo.g_intLearnStepNo == 7 &&
                m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 6;

                if (m_smVisionInfo.g_intLearnStepNo == 7 &&
                m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
                !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 5;

                if (m_smVisionInfo.g_intLearnStepNo == 8 &&
                m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting &&
                !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 6;

                if (m_smVisionInfo.g_intLearnStepNo == 8 &&
              m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
              !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting &&
              !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 5;

                if (m_smVisionInfo.g_intLearnStepNo == 5 &&
           !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
           !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 1;

                if (m_smVisionInfo.g_intLearnStepNo == 3 &&
        m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
        !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 2;

                if (m_smVisionInfo.g_intLearnStepNo == 5 &&
         !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting &&
         m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 3;

                if (m_smVisionInfo.g_intLearnStepNo == 2 &&
        !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
                    m_smVisionInfo.g_intLearnStepNo = 1;
            }


            m_intDisplayStep--;
            SetupSteps(false);

            if (m_intDisplayStep > 8)
                lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 3) + m_intDisplayStep.ToString() + ":";
            else
                lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStep.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //float fTotalThickness = 0;
            //for (int i = 1; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            //{
            //    if (i == 1 || i == 3)
            //        fTotalThickness += (m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2;
            //    else if (i == 2 || i == 4)
            //        fTotalThickness += (m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2;
            //}

            //Save template width, height, thickness
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    m_smVisionInfo.g_arrLead3D[i].ref_fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    m_smVisionInfo.g_arrLead3D[i].ref_fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    m_smVisionInfo.g_arrLead3D[i].ref_fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    m_smVisionInfo.g_arrLead3D[i].ref_fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                
                //m_smVisionInfo.g_arrLead3D[i].ref_fUnitThickness = (float)Math.Round(fTotalThickness / 4, 4, MidpointRounding.AwayFromZero);
            }

            SaveLeadSetting(m_strFolderPath);

            //Load Package Image
            for (int intLeadIndex = 0; intLeadIndex < m_smVisionInfo.g_arrLead3D.Length; intLeadIndex++)
            {
                m_smVisionInfo.g_arrLead3D[intLeadIndex].LoadLeadTemplateImage(m_strFolderPath + "Template\\", intLeadIndex);
                m_smVisionInfo.g_arrLead3D[intLeadIndex].LoadLeadPackageTemplateImage(m_strFolderPathPkg + "Template\\", intLeadIndex);
            }

            LoadROISetting(m_strFolderPath + "ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead3D.Length);

            AttachImageToROI(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[0]);

            DialogResult = DialogResult.OK;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            Close();
            Dispose();
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

        private void LearnPackageForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.g_intViewInspectionSetting = 1;
            m_smVisionInfo.g_blnViewROI = true;
            lbl_StepNo.BringToFront();
            
            SetupSteps(true);

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
            {
                m_smVisionInfo.AT_VM_OfflineTestAllLead3D = true;
                TriggerOfflineTest();
                pnl_Gauge.Visible = false;
                pnl_LineProfile.BringToFront();
            }
            else
                pnl_LineProfile.SendToBack();

            m_smVisionInfo.g_blnViewLeadInspection = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void LearnPackageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.AT_VM_OfflineTestAllLead3D = false;
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Lead3D Package Form Closed", "Exit Learn Lead3D Package Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewPackageMaskROI = false;
            m_smVisionInfo.g_blnViewPkgProcessImage = false;
            m_smVisionInfo.g_blnViewPackageTrainROI = false;
            m_smVisionInfo.g_blnViewPackageObjectBuilded = false;
            m_smVisionInfo.g_blnPackageInspected = false;
            m_smVisionInfo.g_blnViewRotatedPackageImage = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;


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

            if (m_intSelectedROIPrev != m_smVisionInfo.g_intSelectedROI)
            {
                m_intSelectedROIPrev = m_smVisionInfo.g_intSelectedROI;

                // 2019 02 15 - CCENG: Display all width, height and thickness. Easier for user setting.
                //if (m_smVisionInfo.g_intSelectedROI != 0)
                //{
                //    pnl_widthHeight.Visible = false;
                //    pnl_Thickness.Visible = true;
                //}
                //else
                //{
                //    pnl_widthHeight.Visible = true;
                //    pnl_Thickness.Visible = false;
                //}
            }

            if (m_objLead3DLineProfileForm != null && m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
            {
                if (!m_objLead3DLineProfileForm.ref_blnShow)
                {
                    m_smVisionInfo.g_strSelectedPage = "LeadPackage";
                    m_objLead3DLineProfileForm.Close();
                    m_objLead3DLineProfileForm.Dispose();
                    m_objLead3DLineProfileForm = null;
                    m_smVisionInfo.g_intViewInspectionSetting = 1;
                    m_smVisionInfo.AT_VM_OfflineTestAllLead3D = true;
                    TriggerOfflineTest();
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    this.Show();
                }
                else
                {
                    if (m_objLead3DLineProfileForm.ref_blnBuildLead)
                    {
                        m_objLead3DLineProfileForm.ref_blnBuildLead = false;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
            }

            if (m_smVisionInfo.g_blnViewGauge)
            {
                float fWidth = 0;
                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fWidth = (m_smVisionInfo.g_arrLead3D[0].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[0].GetResultDownWidth_RectGauge4L(1)) / 2;

                //// 2019-10-25 ZJYEOH : Add Offset to package width
                //fWidth += m_smVisionInfo.g_arrLead3D[0].ref_fPackageWidthOffsetMM;

                string strResult = fWidth.ToString("F4");
                if (strResult != lbl_SizeWidthResult.Text)
                {
                    lbl_SizeWidthResult.Text = strResult;
                }

                float fHeight = 0;
                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fHeight = (m_smVisionInfo.g_arrLead3D[0].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[0].GetResultRightHeight_RectGauge4L(1)) / 2;

                //// 2019-10-25 ZJYEOH : Add Offset to package height
                //fHeight += m_smVisionInfo.g_arrLead3D[0].ref_fPackageHeightOffsetMM;

                strResult = fHeight.ToString("F4");
                if (strResult != lbl_SizeHeightResult.Text)
                {
                    lbl_SizeHeightResult.Text = strResult;
                }


                //float fThickness = 0;
                //float fTotalThickness = 0;
                //for (int i = 1; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                //{
                //    if (i == 1 || i == 3)
                //        fTotalThickness += (m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2;
                //    else if (i == 2 || i == 4)
                //        fTotalThickness += (m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2;
                //}
                //fThickness = fTotalThickness / 4;

                //// 2019-10-25 ZJYEOH : Add Offset to package thickness
                //fThickness += m_smVisionInfo.g_arrLead3D[1].ref_fPackageThicknessOffsetMM;

                //strResult = fThickness.ToString("F4");
                //if (strResult != lbl_SizeThicknessResult.Text)
                //{
                //    lbl_SizeThicknessResult.Text = strResult;
                //}

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    strResult = m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultAngle().ToString("F4");
                else
                    strResult = m_smVisionInfo.g_arrLead3D[0].GetResultAngle_RectGauge4L().ToString("F4");
                if (strResult != lbl_AngleResult.Text)
                {
                    lbl_AngleResult.Text = strResult;
                }

                bool blnResult = true;

                if (!m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    blnResult = m_smVisionInfo.g_arrLead3D[0].GetRectGauge4LPassFail(0);

                if (!blnResult)
                {
                    lbl_ScoreResult.Text = "Fail";
                    lbl_ScoreResult.ForeColor = Color.Red;
                }
                else
                {
                    lbl_ScoreResult.Text = "Pass";
                    lbl_ScoreResult.ForeColor = Color.Green;
                }
            }

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;
                UpdateGUI();
                if (m_smVisionInfo.g_intLearnStepNo == 0)
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 0:
                            picUnitROI.Image = ils_ImageListTree.Images[13];
                            break;
                    }
                }

                if (m_smVisionInfo.g_intLearnStepNo == 1)
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 0:
                            picPkgROI.Image = ils_ImageListTree.Images[18];
                            break;
                       
                    }
                }
                if (m_smVisionInfo.g_intLearnStepNo == 2)
                {

                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 0:
                            picPkgROI_Chip.Image = ils_ImageListTree.Images[23];
                            break;
                     
                    }

                }
                if (m_smVisionInfo.g_intLearnStepNo == 3)
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 0:
                            picPkgROI_Mold.Image = ils_ImageListTree.Images[28];
                            break;
                       
                    }
                }
                if (m_smVisionInfo.g_intLearnStepNo == 4)
                {
                    label5.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1LowSurfaceThreshold.ToString();
                    label4.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1HighSurfaceThreshold.ToString();
                }
                if (m_smVisionInfo.g_intLearnStepNo == 5)
                {
                    label3.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2LowThreshold.ToString();
                    label2.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2HighThreshold.ToString();
                }
                if (m_smVisionInfo.g_intLearnStepNo == 6)
                {
                    label1.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1MoldFlashThreshold.ToString();
                }
                if (m_smVisionInfo.g_intLearnStepNo == 7)
                {
                    lbl_VoidViewThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2VoidThreshold.ToString();
                }
                if (m_smVisionInfo.g_intLearnStepNo == 8)
                {
                    lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2LowCrackThreshold.ToString();
                    lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2HighCrackThreshold.ToString();
                }
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                        pnl_PkgTop.Location = m_pPkgTop;
                        pnl_PkgRight.Location = m_pPkgRight;
                        pnl_PkgBottom.Location = m_pPkgBottom;
                        pnl_PkgLeft.Location = m_pPkgLeft;

                        pnl_ChippedTop.Location = m_pChippedTop;
                        pnl_ChippedRight.Location = m_pChippedRight;
                        pnl_ChippedBottom.Location = m_pChippedBottom;
                        pnl_ChippedLeft.Location = m_pChippedLeft;

                        pnl_MoldTop.Location = m_pMoldTop;
                        pnl_MoldRight.Location = m_pMoldRight;
                        pnl_MoldBottom.Location = m_pMoldBottom;
                        pnl_MoldLeft.Location = m_pMoldLeft;
                        break;
                 
                }
            }
        }

        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            int intLeadSelectedIndex = m_smVisionInfo.g_intSelectedROI;

            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\RectGauge4L.xml";

            AdvancedLead3DRectGauge4LForm objAdvancedLead3DRectGauge4LForm = new AdvancedLead3DRectGauge4LForm(m_smVisionInfo, strPath, m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo, AdvancedLead3DRectGauge4LForm.ReadFrom.Lead3D);

            if (objAdvancedLead3DRectGauge4LForm.ShowDialog() == DialogResult.Cancel)
            {
                // Set RectGauge4L Placement
                //PointF pCenter = new PointF(m_smVisionInfo.g_arrLeadROIs[intLeadSelectedIndex][1].ref_ROITotalCenterX,
                //                            m_smVisionInfo.g_arrLeadROIs[intLeadSelectedIndex][1].ref_ROITotalCenterY);
               
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], m_smVisionInfo.g_objWhiteImage);
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            // 2019 07 24 - CCENG: Reload Point gauge because the value has been modified during go to Gauge Advance Setting.
            strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\PointGauge.xml";

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;

                m_smVisionInfo.g_arrLead3D[i].ref_objPointGauge.LoadPointGauge(strPath,
                        "Lead" + i.ToString());

                // Permanent set minAmp = 0, min area = 0, filter to 1, choice = from begin
                m_smVisionInfo.g_arrLead3D[i].ref_objPointGauge.ref_GaugeMinAmplitude = 0;
                m_smVisionInfo.g_arrLead3D[i].ref_objPointGauge.ref_GaugeMinArea = 0;
                m_smVisionInfo.g_arrLead3D[i].ref_objPointGauge.ref_GaugeFilter = 1;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        
        private void txt_UnitSize_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_fUnitWidthMin = Convert.ToSingle(txt_UnitSizeMinWidth.Text);
                m_smVisionInfo.g_arrLead3D[i].ref_fUnitHeightMin = Convert.ToSingle(txt_UnitSizeMinHeight.Text);
                m_smVisionInfo.g_arrLead3D[i].ref_fUnitWidthMax = Convert.ToSingle(txt_UnitSizeMaxWidth.Text);
                m_smVisionInfo.g_arrLead3D[i].ref_fUnitHeightMax = Convert.ToSingle(txt_UnitSizeMaxHeight.Text);
                //m_smVisionInfo.g_arrLead3D[i].ref_fUnitThicknessMin = Convert.ToSingle(txt_UnitSizeMinThickness.Text);
                //m_smVisionInfo.g_arrLead3D[i].ref_fUnitThicknessMax = Convert.ToSingle(txt_UnitSizeMaxThickness.Text);
            }
        }

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;
                m_smVisionInfo.g_arrLead3D[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;
                m_smVisionInfo.g_arrLead3D[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CameraGain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fImageGain = Convert.ToSingle(txt_ImageGain.Value);

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                    m_smVisionInfo.g_arrLead3D[i].ref_fImageGain = fImageGain;
            }

            switch (m_smVisionInfo.g_arrLead3D[0].ref_objRectGauge4L.GetGaugeImageMode(0))
            {
                default:
                case 0:
                    {
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                        {
                            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
                        }
                    }
                    break;
                case 1:
                    {
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                        {
                            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, fImageGain);
                            m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, fImageGain);
                            m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                        }

                    }
                    break;
            }

            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Image1SurfaceThreshold_Click(object sender, EventArgs e)
        {
            int intLeadSelectedIndex = m_smVisionInfo.g_intSelectedROI;

            m_smVisionInfo.g_intSelectedImage = 0;

            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1LowSurfaceThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1HighSurfaceThreshold;

            ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[0]);
            bool blnWantSetToAllROICheckBox = false;//(m_smVisionInfo.g_arrLeadROIs.Count > 1);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intLeadSelectedIndex][1], false, blnWantSetToAllROICheckBox);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1LowSurfaceThreshold = intLowThreshold;
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1HighSurfaceThreshold = intHighThreshold;
            }
            else
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intLeadSelectedIndex, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intLeadSelectedIndex != 0 && i == 0)
                            continue;
                        
                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1HighSurfaceThreshold = m_smVisionInfo.g_intHighThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1HighSurfaceThreshold = m_smVisionInfo.g_intHighThresholdValue;
                }
                label5.Text = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1LowSurfaceThreshold.ToString();
                label4.Text = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1HighSurfaceThreshold.ToString();
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Image2Threshold_Click(object sender, EventArgs e)
        {
            int intLeadSelectedIndex = m_smVisionInfo.g_intSelectedROI;

            //m_smVisionInfo.g_intSelectedImage = 2;    // 2019 02 13 - CCENG: Not need to set here. g_intSelectedImage will be set in SetupSteps() when user press Next button.

            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2HighThreshold;

            ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            bool blnWantSetToAllROICheckBox = false;// (m_smVisionInfo.g_arrLeadROIs.Count > 1);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intLeadSelectedIndex][1], false, blnWantSetToAllROICheckBox); //[1]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2LowThreshold = intLowThreshold;
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2HighThreshold = intHighThreshold;
            }
            else
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intLeadSelectedIndex, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intLeadSelectedIndex != 0 && i == 0)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
                }
                label3.Text = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2LowThreshold.ToString();
                label2.Text = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2HighThreshold.ToString();
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_MoldFlashThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
               
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }

            //m_smVisionInfo.g_intSelectedImage = 0;    // 2019 02 13 - CCENG: Not need to set here. g_intSelectedImage will be set in SetupSteps() when user press Next button.    
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1MoldFlashThreshold;

            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;

            bool blnWantSetToAllROICheckBox = false;// (m_smVisionInfo.g_arrLeadROIs.Count > 1);

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[1].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[2].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[3].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[4].ref_intPkgImage1MoldFlashThreshold);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intSelectedROI != 0 && i == 0)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1MoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;
                        if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intThresholdValue;
                            m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1HighSurfaceThreshold = 255;
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1MoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                    {
                        m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1HighSurfaceThreshold = 255;
                    }
                }
                label1.Text = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1MoldFlashThreshold.ToString();
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1MoldFlashThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAll_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_LeadPackage", chk_SetToAll.Checked);
        }

        private void chk_SetToAll_Chipped_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_LeadPackage_Chipped", chk_SetToAll_Chipped.Checked);
        }

        private void chk_SetToAll_Mold_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_LeadPackage_Mold", chk_SetToAll_Mold.Checked);
        }

        private void btn_VoidViewThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }

            //m_smVisionInfo.g_intSelectedImage = 0;    // 2019 02 13 - CCENG: Not need to set here. g_intSelectedImage will be set in SetupSteps() when user press Next button.    
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2VoidThreshold;

            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage2VoidThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[1].ref_intPkgImage2VoidThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[2].ref_intPkgImage2VoidThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[3].ref_intPkgImage2VoidThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[4].ref_intPkgImage2VoidThreshold);

            bool blnWantSetToAllROICheckBox = false;//(m_smVisionInfo.g_arrLeadROIs.Count > 1);
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intSelectedROI != 0 && i == 0)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2VoidThreshold = m_smVisionInfo.g_intThresholdValue;
                        if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intThresholdValue;
                            m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2HighThreshold = 255;
                            m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2LowCrackThreshold = m_smVisionInfo.g_intThresholdValue;
                            m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2HighCrackThreshold = 255;
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2VoidThreshold = m_smVisionInfo.g_intThresholdValue;
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                    {
                        m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2HighThreshold = 255;
                        m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2LowCrackThreshold = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2HighCrackThreshold = 255;
                    }
                }
                lbl_VoidViewThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2VoidThreshold.ToString();
            }
            else
            {

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2VoidThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_CrackViewThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
              
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_intSelectedImage = GetArrayImageIndex(1);
            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;

            m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2LowCrackThreshold;
            m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2HighCrackThreshold;

            ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            bool blnWantSetToAllROICheckBox = false;// (m_smVisionInfo.g_arrLeadROIs.Count > 1);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][1], false, blnWantSetToAllROICheckBox);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intSelectedROI != 0 && i == 0)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2LowCrackThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2HighCrackThreshold = m_smVisionInfo.g_intHighThresholdValue;

                        lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_intLowThresholdValue.ToString();
                        lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_intHighThresholdValue.ToString();
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2LowCrackThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2HighCrackThreshold = m_smVisionInfo.g_intHighThresholdValue;

                    lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_intLowThresholdValue.ToString();
                    lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_intHighThresholdValue.ToString();
                }
            }
            else
            {
                m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2LowCrackThreshold;
                m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2HighCrackThreshold;

            }


            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromEdge_Chipped_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge_Chipped.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge_Chipped.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4) )//* m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromEdge_Chipped.Text = m_fStartPixelFromEdgePrev_Chipped.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
             

                if (chk_SetToAll_Chipped.Checked)
                {
                    txt_StartPixelFromRight_Chipped.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromBottom_Chipped.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromLeft_Chipped.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromEdge = fStartPixelFromEdge;
                        if (chk_SetToAll_Chipped.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        if (chk_SetToAll_Chipped.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                        }
                        break;
                  
                }
            }

         

            m_fStartPixelFromEdgePrev_Chipped = fStartPixelFromEdge;
            if (chk_SetToAll_Chipped.Checked)
            {
                m_fStartPixelFromRightPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Chipped = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_Chipped_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRight_Chipped.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight_Chipped.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4))// * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromRight_Chipped.Text = m_fStartPixelFromRightPrev_Chipped.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
               
                if (chk_SetToAll_Chipped.Checked)
                {
                    txt_StartPixelFromEdge_Chipped.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromBottom_Chipped.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromLeft_Chipped.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                        if (chk_SetToAll_Chipped.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;
                  
                }
            }
          
            m_fStartPixelFromRightPrev_Chipped = fStartPixelFromEdge;
            if (chk_SetToAll_Chipped.Checked)
            {
                m_fStartPixelFromEdgePrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Chipped = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_Chipped_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottom_Chipped.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom_Chipped.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4) )//* m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromBottom_Chipped.Text = m_fStartPixelFromBottomPrev_Chipped.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
              
                if (chk_SetToAll_Chipped.Checked)
                {
                    txt_StartPixelFromEdge_Chipped.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromRight_Chipped.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromLeft_Chipped.Text = fStartPixelFromEdge.ToString();
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                        if (chk_SetToAll_Chipped.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;
             
                }
            }
       
            m_fStartPixelFromBottomPrev_Chipped = fStartPixelFromEdge;
            if (chk_SetToAll_Chipped.Checked)
            {
                m_fStartPixelFromEdgePrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromRightPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Chipped = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_Chipped_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeft_Chipped.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft_Chipped.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4))// * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_StartPixelFromLeft_Chipped.Text = m_fStartPixelFromLeftPrev_Chipped.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
               
                if (chk_SetToAll_Chipped.Checked)
                {
                    txt_StartPixelFromEdge_Chipped.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromRight_Chipped.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromBottom_Chipped.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                        if (chk_SetToAll_Chipped.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                        }
                        break;
                 
                }
            }
           

            m_fStartPixelFromLeftPrev_Chipped = fStartPixelFromEdge;
            if (chk_SetToAll_Chipped.Checked)
            {
                m_fStartPixelFromEdgePrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromRightPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Chipped = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromEdge_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromEdge_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromEdge_Mold.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;
                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll_Mold.Checked)
                {
             
                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalY - fStartPixelFromEdge <= m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_StartPixelFromEdge_Mold.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        txt_StartPixelFromRight_Mold.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        txt_StartPixelFromLeft_Mold.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        txt_StartPixelFromBottom_Mold.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                
                }
                else
                {
                 
                    if ((i == 0 ) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalY - fStartPixelFromEdge <= m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_StartPixelFromEdge_Mold.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    
                }
                if (chk_SetToAll_Mold.Checked)
                {
                    txt_StartPixelFromRight_Mold.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromBottom_Mold.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromLeft_Mold.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromEdge = fStartPixelFromEdge;
                        if (chk_SetToAll_Mold.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;
                  
                }
            }

           

            m_fStartPixelFromEdgePrev_Mold = fStartPixelFromEdge;
            if (chk_SetToAll_Mold.Checked)
            {
                m_fStartPixelFromRightPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Mold = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromRight_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromRight_Mold.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll_Mold.Checked)
                {
              
                    if ((i == 0) && ((m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROIWidth) + fStartPixelFromEdge >= (m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_StartPixelFromEdge_Mold.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        txt_StartPixelFromRight_Mold.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        txt_StartPixelFromLeft_Mold.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        txt_StartPixelFromBottom_Mold.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
              
                }
                else
                {
                  
                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROIWidth) + fStartPixelFromEdge >= (m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_StartPixelFromRight_Mold.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                 
                }

                if (chk_SetToAll_Mold.Checked)
                {
                    txt_StartPixelFromEdge_Mold.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromBottom_Mold.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromLeft_Mold.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromRight = fStartPixelFromEdge;
                        if (chk_SetToAll_Mold.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;

                }
            }
            
            m_fStartPixelFromRightPrev_Mold = fStartPixelFromEdge;
            if (chk_SetToAll_Mold.Checked)
            {
                m_fStartPixelFromEdgePrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Mold = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromBottom_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromBottom_Mold.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll_Mold.Checked)
                {
                  
                    if ((i == 0) && ((m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROIHeight) + fStartPixelFromEdge >= (m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_StartPixelFromEdge_Mold.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        txt_StartPixelFromRight_Mold.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        txt_StartPixelFromLeft_Mold.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        txt_StartPixelFromBottom_Mold.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                 
                }
                else
                {
                   
                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROIHeight) + fStartPixelFromEdge >= (m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_StartPixelFromBottom_Mold.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    
                }
                if (chk_SetToAll_Mold.Checked)
                {
                    txt_StartPixelFromEdge_Mold.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromRight_Mold.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromLeft_Mold.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromBottom = fStartPixelFromEdge;
                        if (chk_SetToAll_Mold.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;
                 
                }
            }

      

            m_fStartPixelFromBottomPrev_Mold = fStartPixelFromEdge;
            if (chk_SetToAll_Mold.Checked)
            {
                m_fStartPixelFromEdgePrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromRightPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Mold = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_Mold_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_StartPixelFromLeft_Mold.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_StartPixelFromLeft_Mold.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll_Mold.Checked)
                {
                   
                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalX - fStartPixelFromEdge <= m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_StartPixelFromEdge_Mold.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        txt_StartPixelFromRight_Mold.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        txt_StartPixelFromLeft_Mold.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        txt_StartPixelFromBottom_Mold.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
              
                }
                else
                {
                 
                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalX - fStartPixelFromEdge <= m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_StartPixelFromLeft_Mold.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                   
                }
                if (chk_SetToAll_Mold.Checked)
                {
                    txt_StartPixelFromEdge_Mold.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromRight_Mold.Text = fStartPixelFromEdge.ToString();
                    txt_StartPixelFromBottom_Mold.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromLeft = fStartPixelFromEdge;
                        if (chk_SetToAll_Mold.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromBottom = fStartPixelFromEdge;
                        }
                        break;
                
                }
            }
        
            m_fStartPixelFromLeftPrev_Mold = fStartPixelFromEdge;
            if (chk_SetToAll_Mold.Checked)
            {
                m_fStartPixelFromEdgePrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromRightPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Mold = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackViewMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_CrackViewMinArea.Text == "")
                return;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
               
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fCrackMinArea = Convert.ToInt32(txt_CrackViewMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_LineProfileGaugeSetting_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath +
                               m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\PointGauge.xml";

            if (m_objLead3DLineProfileForm == null)
                m_objLead3DLineProfileForm = new Lead3DLineProfileForm(m_smVisionInfo, m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge, strPath, m_smProductionInfo, 1);

            m_objLead3DLineProfileForm.Show();

            m_smVisionInfo.g_strSelectedPage = "Lead3DLineProfileGaugeSetting";
            this.Hide();
        }

        private void txt_VoidViewMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_VoidViewMinArea.Text == "")
                return;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fVoidMinArea = Convert.ToInt32(txt_VoidViewMinArea.Text);
            if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
            {
                m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fImage2SurfaceMinArea = Convert.ToSingle(txt_VoidViewMinArea.Text);
                m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fCrackMinArea = Convert.ToInt32(txt_VoidViewMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void srmInputBox2_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                {
                    m_smVisionInfo.g_arrLead3D[i].ref_fMoldFlashMinArea = Convert.ToSingle(srmInputBox2.Text);
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                    {
                        m_smVisionInfo.g_arrLead3D[i].ref_fSurfaceMinArea = Convert.ToSingle(srmInputBox2.Text);
                    }
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void srmInputBox4_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                    m_smVisionInfo.g_arrLead3D[i].ref_fImage2SurfaceMinArea = Convert.ToSingle(srmInputBox4.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void srmInputBox5_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                    m_smVisionInfo.g_arrLead3D[i].ref_fSurfaceMinArea = Convert.ToSingle(srmInputBox5.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_GaugeSaveClose_Click(object sender, EventArgs e)
        {
            if (!CheckSetting())
                return;

            //Check is gauge ok or not
            if (chk_UseGauge.Checked && !m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
            {
                bool blnGaugeOK = true;
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    if (i != 0)
                        break;

                    if (!m_smVisionInfo.g_arrLead3D[i].IsGaugeMeasureOK(i))
                    {
                        string strDirectionName;
                        switch (i)
                        {
                            case 0:
                            default:
                                strDirectionName = "Center";
                                break;
                            case 1:
                                strDirectionName = "Top";
                                break;
                            case 2:
                                strDirectionName = "Right";
                                break;
                            case 3:
                                strDirectionName = "Bottom";
                                break;
                            case 4:
                                strDirectionName = "Left";
                                break;

                        }
                        SRMMessageBox.Show("Gauge measurement in " + strDirectionName + " ROI is not good. Please adjust the ROI or gauge setting.");
                        blnGaugeOK = false;
                        break;

                    }
                }

                if (!blnGaugeOK)
                {
                    return;
                }
            }
            //float fTotalThickness = 0;
            //for (int i = 1; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            //{
            //    if (i == 1 || i == 3)
            //        fTotalThickness += (m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2;
            //    else if (i == 2 || i == 4)
            //        fTotalThickness += (m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2;
            //}

            //Save template width, height, thickness
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    m_smVisionInfo.g_arrLead3D[i].ref_fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    m_smVisionInfo.g_arrLead3D[i].ref_fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    m_smVisionInfo.g_arrLead3D[i].ref_fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    m_smVisionInfo.g_arrLead3D[i].ref_fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
             
                //m_smVisionInfo.g_arrLead3D[i].ref_fUnitThickness = (float)Math.Round(fTotalThickness / 4, 4, MidpointRounding.AwayFromZero);
            }

            SaveLeadSetting(m_strFolderPath);

            //Load Package Image
            for (int intLeadIndex = 0; intLeadIndex < m_smVisionInfo.g_arrLead3D.Length; intLeadIndex++)
            {
                if (intLeadIndex != 0)
                    break;
                m_smVisionInfo.g_arrLead3D[intLeadIndex].LoadLeadTemplateImage(m_strFolderPath + "Template\\", intLeadIndex);
                m_smVisionInfo.g_arrLead3D[intLeadIndex].LoadLeadPackageTemplateImage(m_strFolderPathPkg + "Template\\", intLeadIndex);
            }

            LoadROISetting(m_strFolderPath + "ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead3D.Length);

            AttachImageToROI(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[0]);

            DialogResult = DialogResult.OK;

            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            this.Close();
            this.Dispose();
        }

        private void txt_BrightMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_BrightMinArea.Text == "")
                return;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
             
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intBrightFieldMinArea = Convert.ToInt32(txt_BrightMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_DarkMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_DarkMinArea.Text == "")
                return;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intDarkFieldMinArea = Convert.ToInt32(txt_DarkMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BrightThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
               
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }

            //m_smVisionInfo.g_intSelectedImage = 0;    // 2019 02 13 - CCENG: Not need to set here. g_intSelectedImage will be set in SetupSteps() when user press Next button.    
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intBrightFieldLowThreshold;

            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[0].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[1].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[2].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[3].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[4].ref_intBrightFieldLowThreshold);

            bool blnWantSetToAllROICheckBox = false;//(m_smVisionInfo.g_arrLeadROIs.Count > 1);
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intSelectedROI != 0 && i == 0)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intBrightFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intBrightFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                }
                lbl_BrightThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldLowThreshold.ToString();
            }
            else
            {

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intBrightFieldLowThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DarkThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
               
                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }

            //m_smVisionInfo.g_intSelectedImage = 0;    // 2019 02 13 - CCENG: Not need to set here. g_intSelectedImage will be set in SetupSteps() when user press Next button.    
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intDarkFieldLowThreshold;
            m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fDarkFieldImageGain;

            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;

            bool blnWantSetToAllROICheckBox = false;// (m_smVisionInfo.g_arrLeadROIs.Count > 1);
            ThresholdWithGainForm objThresholdForm = new ThresholdWithGainForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intSelectedROI != 0 && i == 0)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intDarkFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrLead3D[i].ref_fDarkFieldImageGain = m_smVisionInfo.g_fThresholdGainValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intDarkFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fDarkFieldImageGain = m_smVisionInfo.g_fThresholdGainValue;
                }
                lbl_DarkThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldLowThreshold.ToString();
            }
            else
            {

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intDarkFieldLowThreshold;
                m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fDarkFieldImageGain;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        
    }
}
