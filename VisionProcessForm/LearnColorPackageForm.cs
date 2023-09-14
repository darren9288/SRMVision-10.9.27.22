using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using VisionProcessing;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class LearnColorPackageForm : Form
    {
        #region Member Variables
        private bool m_blnColorGuideline = false;
        private bool m_blnColorThresholdForm = false;
        private bool m_blnSetCopper = false;
        private bool m_blnSetOxidation = false;
        private bool m_blnInitDone = false;
        private bool m_blnNextButton = false;
        private int m_intDisplayStepNo = 1;
        private int m_intLearnStepNo = 0;
        private int m_intUserGroup = 0;
        private string m_strFolderPath = "";
        private string m_strSelectedRecipe = "";
        
        private UserRight m_objUserRight = new UserRight();
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        #endregion




        public LearnColorPackageForm(VisionInfo smVisionInfo,
                    string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\";
            
            InitializeComponent();

            if (m_smVisionInfo.g_arrColorImages.Count < 2)
            {
                SRMMessageBox.Show("At least 2 images need to be captured for this image. Please go to Admin level and change the settings");
                btn_Next.Enabled = false;
                return;
            }

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrColorImages[i].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[i]);
                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
            }

            ArrayList arrSearchROI = new ArrayList();
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                AddSearchROI(i, m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrColorPackageROIs, m_smVisionInfo.g_arrPackageGauge, "Package");
   

            UpdateGUI();
            SetupSteps();

            m_blnInitDone = true;
        }





        private void AddGauge(ROI objSearchROI, List<RectGauge> arrGauge, string strVisionModule, int intNo)
        {
            XmlParser objFile = new XmlParser(m_strFolderPath + "\\Gauge.xml");
            objFile.GetFirstSection("RectG" + intNo.ToString());

            //create new gauge and attach to selected ROI
            RectGauge objGauge = new RectGauge();

            //attach searcher parent
            objGauge.SetRectGaugePlacement(objSearchROI, objFile.GetValueAsFloat("Tolerance", 25),
                objFile.GetValueAsInt("SizeTolerance", 10));

            //set gauge measurement
            objGauge.ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
            objGauge.ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 0);

            //set gauge setting
            objGauge.SetRectGaugeSetting(objFile.GetValueAsInt("Thickness", 13), objFile.GetValueAsInt("Filter", 1),
                objFile.GetValueAsInt("Threshold", 2), objFile.GetValueAsInt("MinAmp", 10), objFile.GetValueAsInt("MinArea", 0));

            //set gauge fitting sampling step
            objGauge.SetRectGaugeFitting(objFile.GetValueAsInt("SamplingStep", 5));

            arrGauge.Add(objGauge);
        }

        private void AddSearchROI(int intUnitNo, List<List<ROI>> arrROIs, List<List<CROI>> arrColorROIs, List<RectGauge> arrGauge, string strVisionModule)
        {
            ROI objROI;
            CROI objColorROI;

            for (int i = arrROIs.Count; i <= intUnitNo; i++)
            {
                arrROIs.Add(new List<ROI>());
            }

            for (int i = arrColorROIs.Count; i <= intUnitNo; i++)
            {
                arrColorROIs.Add(new List<CROI>());
            }

            if (arrROIs[intUnitNo].Count == 0)
            {
                if (intUnitNo == 0)
                {
                    objROI = new ROI("Package Test ROI", 1);
                    objColorROI = new CROI("Package Test ROI", 1);
                }
                else
                {
                    objROI = new ROI("Package ReTest ROI", 1);
                    objColorROI = new CROI("Package ReTest ROI", 1);
                }

                if (intUnitNo == 0)
                {
                    int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 100;
                    int intPositionY = (480 / 2) - 100;
                    objROI.LoadROISetting(intPositionX, intPositionY, 200, 200);
                    objColorROI.LoadROISetting(intPositionX, intPositionY, 200, 200);
                }
                else if (intUnitNo != 0)
                {
                    int intPositionX = (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - (arrROIs[0][0].ref_ROIWidth / 2);
                    objROI.LoadROISetting(intPositionX, arrROIs[0][0].ref_ROIPositionY,
                        arrROIs[0][0].ref_ROIWidth, arrROIs[0][0].ref_ROIHeight);
                    objColorROI.LoadROISetting(intPositionX, arrROIs[0][0].ref_ROIPositionY,
                       arrROIs[0][0].ref_ROIWidth, arrROIs[0][0].ref_ROIHeight);
                }

                objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                objColorROI.AttachImage(m_smVisionInfo.g_arrColorImages[0]);

                AddGauge(objROI, arrGauge, strVisionModule, 0);
                arrROIs[intUnitNo].Add(objROI);
                arrColorROIs[intUnitNo].Add(objColorROI);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void AddTrainROI(int intGap)
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[i][0];
                objSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[1]);

                float fCenterX = m_smVisionInfo.g_arrPackageGauge[i].ref_ObjectCenterX;
                float fCenterY = m_smVisionInfo.g_arrPackageGauge[i].ref_ObjectCenterY;
                float fWidth = m_smVisionInfo.g_arrPackageGauge[i].ref_ObjectWidth;
                float fHeight = m_smVisionInfo.g_arrPackageGauge[i].ref_ObjectHeight;
                int intPositionX = (int)Math.Round(fCenterX - (fWidth / 2), 0, MidpointRounding.AwayFromZero) - objSearchROI.ref_ROIPositionX + intGap;
                int intPositionY = (int)Math.Round(fCenterY - (fHeight / 2), 0, MidpointRounding.AwayFromZero) - objSearchROI.ref_ROIPositionY + intGap;

                ROI objROI = new ROI("Package ROI " + i, 2);
                objROI.LoadROISetting(intPositionX, intPositionY, (int)Math.Round(fWidth - (intGap * 2), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(fHeight - (intGap * 2),0, MidpointRounding.AwayFromZero));
                objROI.AttachImage(objSearchROI);

                CROI objColorROI = new CROI();
                objROI.CopyToNew(ref objColorROI);
                m_smVisionInfo.g_arrColorPackageROIs[i][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[0]);
                objColorROI.AttachImage(m_smVisionInfo.g_arrColorPackageROIs[i][0]);

                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 1)
                {
                    m_smVisionInfo.g_arrPackageROIs[i].Add(objROI);
                    m_smVisionInfo.g_arrColorPackageROIs[i].Add(objColorROI);                  
                }
                else
                {
                    m_smVisionInfo.g_arrPackageROIs[i][1] = objROI;
                    m_smVisionInfo.g_arrColorPackageROIs[i][1] = objColorROI;
                }

                m_smVisionInfo.g_arrColorPackage[i].ref_intStartPixelFromEdge = intGap;
            }          
        }

        private void AttachImageToROI(List<List<ROI>> arrROIs, ImageDrawing objImage)
        {
            for (int i = 0; i < arrROIs.Count; i++)
            {
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

        private void LoadGaugeSetting(string strPath, List<RectGauge> arrGauge)
        {
            arrGauge.Clear();

            XmlParser objFile = new XmlParser(strPath);
            RectGauge objRectGauge;
            int intParentCount = objFile.GetFirstSectionCount();

            for (int j = 0; j < m_smVisionInfo.g_intUnitsOnImage; j++)
            {
                if (j >= intParentCount)
                    continue;

                //create new ROI base on file read out
                objRectGauge = new RectGauge();
                objFile.GetFirstSection("RectG" + j);

                //set location,size,tolerance to place rect gauge
                objRectGauge.SetRectGaugePlacement(
                    objFile.GetValueAsFloat("CenterX", 0), objFile.GetValueAsFloat("CenterY", 0),
                    objFile.GetValueAsFloat("Width", 100), objFile.GetValueAsFloat("Height", 100),
                    objFile.GetValueAsFloat("Tolerance", 25), objFile.GetValueAsInt("SizeTolerance", 10));

                //set type and choice of measure
                objRectGauge.SetRectGaugeMeasurement(objFile.GetValueAsInt("TransType", 0), objFile.GetValueAsInt("TransChoice", 0));

                //set gauge setting for analysis
                objRectGauge.SetRectGaugeSetting(objFile.GetValueAsInt("Thickness", 13), objFile.GetValueAsInt("Filter", 1),
                    objFile.GetValueAsInt("Threshold", 2), objFile.GetValueAsInt("MinAmp", 10), objFile.GetValueAsInt("MinArea", 0));

                //set sampling step,interval 
                objRectGauge.SetRectGaugeFitting(objFile.GetValueAsInt("SamplingStep", 5));

                //Set rect gauge template
                objRectGauge.SetRectGaugeTemplate(objFile.GetValueAsFloat("ObjectCenterX", 0),
                    objFile.GetValueAsFloat("ObjectCenterY", 0), objFile.GetValueAsFloat("ObjectWidth", 0),
                    objFile.GetValueAsFloat("ObjectHeight", 0));

                //add Rect Gauge into shared memory's Rect Gauge array list 
                arrGauge.Add(objRectGauge);
            }

            objRectGauge = null;
        }

        private void LoadPackageSettings(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("Settings");
          
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrColorPackage[i].ref_bCheckChipAtBorder = objFile.GetValueAsBoolean("CheckChipAtBorder", false);

                m_smVisionInfo.g_arrPackage[i].ref_intStartPixelFromEdge = objFile.GetValueAsInt("PixelFromEdge", 2);
                m_smVisionInfo.g_arrColorPackage[i].ref_intWOBMinArea = objFile.GetValueAsInt("WOBMinArea", 0);
                m_smVisionInfo.g_arrColorPackage[i].ref_intCopperMinArea = objFile.GetValueAsInt("CopperMinArea", 0);
                m_smVisionInfo.g_arrColorPackage[i].ref_intOxidationMinArea = objFile.GetValueAsInt("OxidationMinArea", 0);
                m_smVisionInfo.g_arrColorPackage[i].ref_intWOBThreshold = objFile.GetValueAsInt("WOBThreshold", 2);            

                m_smVisionInfo.g_arrColorPackage[i].SetCopperBlobThreshold(objFile.GetValueAsInt("CopperThreshold1", 0), 
                    objFile.GetValueAsInt("CopperThreshold2", 0), objFile.GetValueAsInt("CopperThreshold3", 0), objFile.GetValueAsInt("CopperTolerance1", 0), 
                    objFile.GetValueAsInt("CopperTolerance2", 0),objFile.GetValueAsInt("CopperTolerance3", 0));

                m_smVisionInfo.g_arrColorPackage[i].SetOxidationBlobThreshold(objFile.GetValueAsInt("OxidationThreshold1", 0),
                  objFile.GetValueAsInt("OxidationThreshold2", 0), objFile.GetValueAsInt("OxidationThreshold3", 0), objFile.GetValueAsInt("OxidationTolerance1", 0),
                  objFile.GetValueAsInt("OxidationTolerance2", 0), objFile.GetValueAsInt("OxidationTolerance3", 0));
            } 
        }

        private void LoadROISetting(string strPath, List<List<ROI>> arrROIList, List<List<CROI>> arrColorROIList)
        {
            arrROIList.Clear();
            arrColorROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            ROI objROI;
            CROI objColorROI;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                arrROIList.Add(new List<ROI>());
                arrColorROIList.Add(new List<CROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new ROI();
                    objColorROI = new CROI();
                    
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
                  
                    arrROIList[i].Add(objROI);

                    objROI.CopyToNew(ref objColorROI);
                    arrColorROIList[i].Add(objColorROI);
                }
            }
        }

        private void ReadSettings()
        {
            LoadROISetting(m_strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrColorPackageROIs);
            LoadGaugeSetting(m_strFolderPath + "Gauge.xml", m_smVisionInfo.g_arrPackageGauge);
            LoadPackageSettings(m_strFolderPath + "Settings.xml");

            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrPackageROIs[i].Count; j++)
                {
                    if (j == 0)
                        m_smVisionInfo.g_arrPackageROIs[i][j].AttachImage(m_smVisionInfo.g_arrImages[0]);
                    else
                        m_smVisionInfo.g_arrPackageROIs[i][j].AttachImage(m_smVisionInfo.g_arrPackageROIs[i][0]);
                }
        }

        private void Rotate0Degree(List<List<ROI>> arrROIs, List<List<CROI>> arrColorROIs, List<RectGauge> arrGauge)
        {            
            for (int i = 0; i < arrROIs.Count; i++)
            {
                arrROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[1]);
                float fGaugeAngle = arrGauge[i].Measure(arrROIs[i][0]);
                if (fGaugeAngle != 0.0f)
                {
                    CROI objColorROI = new CROI();
                    objColorROI.LoadROISetting((int)Math.Round(arrGauge[i].ref_ObjectCenterX - (float)arrColorROIs[i][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                               (int)Math.Round(arrGauge[i].ref_ObjectCenterY - (float)arrColorROIs[i][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                               arrColorROIs[i][0].ref_ROIWidth, arrColorROIs[i][0].ref_ROIHeight);

                    for (int j = 0; j < m_smVisionInfo.g_arrColorImages.Count; j++)
                    {
                        objColorROI.AttachImage(m_smVisionInfo.g_arrColorImages[j]);
                        CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[j], objColorROI, fGaugeAngle, 8, ref m_smVisionInfo.g_arrColorRotatedImages, j);
                        m_smVisionInfo.g_arrColorRotatedImages[j].ConvertColorToMono(m_smVisionInfo.g_arrRotatedImages[j]);
                    }                    
                }
            }
        }

        private void SaveGaugeSettings()
        {
            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "Gauge.xml");

            XmlParser objFile = new XmlParser(m_strFolderPath + "Gauge.xml", false);
            for (int j = 0; j < m_smVisionInfo.g_arrPackageGauge.Count; j++)
            {
                RectGauge objRectGauge = m_smVisionInfo.g_arrPackageGauge[j];
                objFile.WriteSectionElement("RectG" + j);

                objFile.WriteElement1Value("ObjectCenterX", objRectGauge.ref_ObjectCenterX);
                objFile.WriteElement1Value("ObjectCenterY", objRectGauge.ref_ObjectCenterY);
                objFile.WriteElement1Value("ObjectWidth", objRectGauge.ref_ObjectWidth);
                objFile.WriteElement1Value("ObjectHeight", objRectGauge.ref_ObjectHeight);

                objFile.WriteElement1Value("CenterX", objRectGauge.ref_GaugeCenterX);
                objFile.WriteElement1Value("CenterY", objRectGauge.ref_GaugeCenterY);
                objFile.WriteElement1Value("Width", objRectGauge.ref_GaugeWidth);
                objFile.WriteElement1Value("Height", objRectGauge.ref_GaugeHeight);

                objFile.WriteElement1Value("Tolerance", objRectGauge.ref_GaugeTolerance);
                objFile.WriteElement1Value("TransChoice", objRectGauge.ref_GaugeTransChoice);
                objFile.WriteElement1Value("Threshold", objRectGauge.ref_GaugeThreshold);
                objFile.WriteElement1Value("Filter", objRectGauge.ref_GaugeFilterThreshold);
                objFile.WriteElement1Value("MinAmp", objRectGauge.ref_GaugeMinAmplitude);
                objFile.WriteElement1Value("Thickness", objRectGauge.ref_GaugeThickness);
                objFile.WriteElement1Value("MinArea", objRectGauge.ref_GaugeMinArea);
                objFile.WriteElement1Value("SizeTolerance", Convert.ToInt32(objRectGauge.ref_GaugeSizeTolerance * 100));

                objRectGauge.SetRectGaugeTemplate(objRectGauge.ref_ObjectCenterX, objRectGauge.ref_ObjectCenterY,
                    objRectGauge.ref_ObjectWidth, objRectGauge.ref_ObjectHeight);
            }
            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package Gauge", m_smProductionInfo.g_strLotID);
            
        }
        private void SavePackageSettings()
        {
            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "Settings.xml");
            XmlParser objFile = new XmlParser(m_strFolderPath + "Settings.xml", false);
            objFile.WriteSectionElement("Settings");
            objFile.WriteElement1Value("CheckChipAtBorder", m_smVisionInfo.g_arrColorPackage[0].ref_bCheckChipAtBorder);

            objFile.WriteElement1Value("PixelFromEdge", txt_StartPixelFromEdge.Text);
            objFile.WriteElement1Value("WOBMinArea", txt_ScratchArea.Text);
            objFile.WriteElement1Value("CopperMinArea", txt_CopperMinArea.Text);
            objFile.WriteElement1Value("OxidationMinArea", txt_OxidationMinArea.Text);

            objFile.WriteElement1Value("WOBThreshold", m_smVisionInfo.g_arrColorPackage[0].ref_intWOBThreshold);         
            objFile.WriteElement1Value("CopperThreshold1", m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColor[0]);
            objFile.WriteElement1Value("CopperThreshold2", m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColor[1]);
            objFile.WriteElement1Value("CopperThreshold3", m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColor[2]);
            objFile.WriteElement1Value("CopperTolerance1", m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColorTolerance[0]);
            objFile.WriteElement1Value("CopperTolerance2", m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColorTolerance[1]);
            objFile.WriteElement1Value("CopperTolerance3", m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColorTolerance[2]);

            objFile.WriteElement1Value("OxidationThreshold1", m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationColor[0]);
            objFile.WriteElement1Value("OxidationThreshold2", m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationColor[1]);
            objFile.WriteElement1Value("OxidationThreshold3", m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationColor[2]);
            objFile.WriteElement1Value("OxidationTolerance1", m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationColorTolerance[0]);
            objFile.WriteElement1Value("OxidationTolerance2", m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationColorTolerance[1]);
            objFile.WriteElement1Value("OxidationTolerance3", m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationColorTolerance[2]);

            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package", m_smProductionInfo.g_strLotID);
            
        }

        private void SaveROISettings()
        {
            ROI objSearchROI = new ROI();
            ROI objTrainROI = new ROI();
            ROI objROI;
            XmlParser objFile = new XmlParser(m_strFolderPath + "ROI.xml", true);
            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");
            ROI objSelectedROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1];
            for (int x = 0; x < m_smVisionInfo.g_arrPackageROIs.Count; x++)
            {
                objFile.WriteSectionElement("Unit" + x);
                for (int j = 0; j < m_smVisionInfo.g_arrPackageROIs[x].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrPackageROIs[x][j];
                    if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                    {
                        objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
                        objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
                        objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
                        objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
                    }

                    objFile.WriteElement1Value("ROI" + j, "");

                    if (j > 1)
                        objROI.ref_strROIName = Convert.ToString(j - 1);
                    objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                    objFile.WriteElement2Value("Type", objROI.ref_intType);
                    objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                    objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);

                    switch (objROI.ref_intType)
                    {
                        case 1:
                            m_smVisionInfo.g_arrPackageROIs[x][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                            break;
                        case 2:
                            m_smVisionInfo.g_arrPackageROIs[x][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[x][0]);
                            break;
                    }

                    float fPixelAverage = objROI.GetROIAreaPixel();
                    objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                    objROI.SetROIPixelAverage(fPixelAverage);

                    objFile.WriteEndElement();
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package ROI", m_smProductionInfo.g_strLotID);
                    
                }
            }
        }

        private void SetupSteps()
        {
            switch(m_intLearnStepNo)
            {
                case 0:
                    m_smVisionInfo.g_intSelectedImage = 1;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                 
                    lbl_TitleStep1.BringToFront();
                    tabCtrl_Lean.SelectedTab = tp_Step1;
                    btn_Previous.Enabled = false;
                    break;
                case 1:
                    m_smVisionInfo.g_intSelectedImage = 1;
                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;                   
                    m_smVisionInfo.g_blnViewPackageTrainROI = false;
                    m_smVisionInfo.g_blnViewRotatedImage = false;

                    if (m_blnNextButton)
                    {
                        m_smVisionInfo.g_arrPackageGauge[0].SetRectGaugeSize(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0],
                        m_smVisionInfo.g_arrPackageGauge[0].ref_GaugeSizeTolerance);
                    }

                    lbl_TitleStep2.BringToFront();
                    tabCtrl_Lean.SelectedTab = tp_Step2;
                    btn_Previous.Enabled = true;                   
                    break;
                case 2:
                    m_smVisionInfo.g_intSelectedImage = 1;
                    m_blnSetCopper = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewPackageTrainROI = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;                 
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewCopperObject = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    if (m_blnNextButton)
                    {
                        m_blnInitDone = false;
                        Rotate0Degree(m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrColorPackageROIs, m_smVisionInfo.g_arrPackageGauge);
                        AddTrainROI(Convert.ToInt32(txt_StartPixelFromEdge.Text));
                        
                        float fPercentage = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth / m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectWidth * 100;
                        txt_Percentage.Text = fPercentage.ToString("f2");

                        m_blnInitDone = true;
                    }
                 
                    lbl_TitleStep3.BringToFront();
                    tabCtrl_Lean.SelectedTab = tp_Step3;                  
                    break;
                case 3:
                    m_smVisionInfo.g_intSelectedImage = 0;
                    m_blnSetCopper = true;
                    if (m_blnNextButton)
                    {
                        m_smVisionInfo.g_arrColorPackageROIs[0][0].AttachImage(m_smVisionInfo.g_arrColorImages[0]);
                        m_smVisionInfo.g_arrColorPackageROIs[0][1].AttachImage(m_smVisionInfo.g_arrColorPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                    }
                    ImageDrawing objImage = new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                    ROI objNewROI = new ROI();
                    objNewROI.LoadROISetting(m_smVisionInfo.g_arrColorPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalX, 
                        m_smVisionInfo.g_arrColorPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalY,
                        m_smVisionInfo.g_arrColorPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth,
                        m_smVisionInfo.g_arrColorPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);
                    objNewROI.AttachImage(objImage);
                    m_smVisionInfo.g_arrColorPackage[0].BuildExposedCopper(m_smVisionInfo.g_arrColorPackageROIs[m_smVisionInfo.g_intSelectedUnit][1], objNewROI);
                    m_smVisionInfo.g_blnViewCopperObject = true;
                    m_smVisionInfo.g_blnViewWOBObject = false;
                    m_smVisionInfo.g_intSelectedImage = 0;

                    lbl_TitleStep4.BringToFront();
                    tabCtrl_Lean.SelectedTab = tp_Step4;
                    btn_Next.Enabled = true;
                    break;
                case 4:
                    m_smVisionInfo.g_blnViewWOBObject = true;
                    m_smVisionInfo.g_blnViewCopperObject = false;
                    m_smVisionInfo.g_blnViewOxidationObject = false;
                    m_blnSetOxidation = false;
                    m_blnSetCopper = false;
                    m_smVisionInfo.g_intSelectedImage = 1;

                    if (m_blnNextButton)
                    {
                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[1]);
                        m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                    }
                    m_smVisionInfo.g_arrColorPackage[0].BuildWOBObjects(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

                    // Define autothreshold template value for package ROI (Image 2)
                    m_smVisionInfo.g_arrColorPackage[0].ref_intTempAutoTHValue = ROI.GetAutoThresholdValue(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1], 3);

                    lbl_TitleStep5.BringToFront();
                    tabCtrl_Lean.SelectedTab = tp_Step5;
                   
                    btn_Next.Enabled = false;
                    break;
                case 5:
                    m_blnSetOxidation = true;        
                    m_smVisionInfo.g_arrColorPackage[0].BuildOxidazation(m_smVisionInfo.g_arrColorPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                    m_smVisionInfo.g_blnViewOxidationObject = true;                  
                    m_smVisionInfo.g_blnViewWOBObject = false;
                    m_smVisionInfo.g_intSelectedImage = 0;

                    lbl_TitleStep6.BringToFront();
                    tabCtrl_Lean.SelectedTab = tp_Step6;
                    btn_Next.Enabled = false;
                    break;
            }
        }

        private void UpdateGUI()
        {
            txt_StartPixelFromEdge.Text = m_smVisionInfo.g_arrColorPackage[0].ref_intStartPixelFromEdge.ToString();
            txt_GaugeTolerance.Text = m_smVisionInfo.g_arrPackageGauge[0].ref_GaugeTolerance.ToString();
            txt_CopperMinArea.Text = m_smVisionInfo.g_arrColorPackage[0].ref_intCopperMinArea.ToString();
            txt_OxidationMinArea.Text = m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationMinArea.ToString();
            txt_ScratchArea.Text = m_smVisionInfo.g_arrColorPackage[0].ref_intWOBMinArea.ToString();

            string strChild1 = "Learn Page";
            string strChild2 = "";
            strChild2 = "Advance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                
            }

            strChild2 = "Gauge Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_AdvanceGauge.Visible = false;
            }

            strChild2 = "Learn Template";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Previous.Enabled = false;
                btn_Next.Enabled = false;
            }
        }




        private void txt_GaugeTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_arrPackageGauge[0].SetRectGaugeTolerance(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], float.Parse(txt_GaugeTolerance.Text));           
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        
        private void txt_StartPixelFromEdge_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_StartPixelFromEdge.Text == "" || Convert.ToInt32(txt_StartPixelFromEdge.Text) < 0)
                return;

            m_blnInitDone = false;
            //Modify package ROI according to the the value of the pixel start from the edge
            AddTrainROI(Convert.ToInt32(txt_StartPixelFromEdge.Text));
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            float fUnitArea = m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectWidth * m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectHeight;
            float fROIArea = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth * m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight;
            float fPercentage = fROIArea / fUnitArea * 100;
            txt_Percentage.Text = fPercentage.ToString("f2");

            m_blnInitDone = true;
        }

        private void txt_CopperMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrColorPackage[i].ref_intCopperMinArea = Convert.ToInt32(txt_CopperMinArea.Text);
                ImageDrawing objImage = new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                ROI objNewROI = new ROI();
                objNewROI.LoadROISetting(m_smVisionInfo.g_arrColorPackageROIs[i][1].ref_ROITotalX, m_smVisionInfo.g_arrColorPackageROIs[i][1].ref_ROITotalY,
                    m_smVisionInfo.g_arrColorPackageROIs[i][1].ref_ROIWidth, m_smVisionInfo.g_arrColorPackageROIs[i][1].ref_ROIHeight);
                objNewROI.AttachImage(objImage);
                m_smVisionInfo.g_arrColorPackage[i].BuildExposedCopper(m_smVisionInfo.g_arrColorPackageROIs[i][1], objNewROI);
            }
        
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ScratchArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrColorPackage[i].ref_intWOBMinArea = Convert.ToInt32(txt_ScratchArea.Text);
                m_smVisionInfo.g_arrColorPackage[i].BuildWOBObjects(m_smVisionInfo.g_arrPackageROIs[i][1]);
            }
            m_smVisionInfo.g_blnViewWOBObject = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OxidationMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrColorPackage[i].ref_intOxidationMinArea = Convert.ToInt32(txt_OxidationMinArea.Text);
                m_smVisionInfo.g_arrColorPackage[i].BuildOxidazation(m_smVisionInfo.g_arrColorPackageROIs[i][1]);
            }
            m_smVisionInfo.g_blnViewOxidationObject = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Percentage_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_Percentage.Text == "" || float.Parse(txt_Percentage.Text) < 0)
                return;
            m_blnInitDone = false;

           float fPercentage = float.Parse(txt_Percentage.Text);
           float fROIHeight = m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectHeight * fPercentage/100;
           float fROIWidth = m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectWidth * fPercentage/100;

            //float fUnitArea = m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectWidth * m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectHeight;
            //float fNisbah = m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectWidth / m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectHeight;
            //float fROIArea = fPercentage / 100 * fUnitArea;

            //float fROIHeight = (float)Math.Sqrt((fROIArea / fNisbah));
            //float fROIWidth = fROIArea / fROIHeight;

            float fCenterX = m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectCenterX;
            float fCenterY = m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectCenterY;
              
            txt_StartPixelFromEdge.Text = Math.Round((m_smVisionInfo.g_arrPackageGauge[0].ref_ObjectWidth -fROIWidth) / 2).ToString("f0");
            AddTrainROI(Convert.ToInt32(txt_StartPixelFromEdge.Text));
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnInitDone = true;
        }

        


        private void btn_AdvanceGauge_Click(object sender, EventArgs e)
        {
            AdvancedRGaugeForm objForm = new AdvancedRGaugeForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageGauge[0],
                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            objForm.Location = new Point(720, 200);
            objForm.ShowDialog();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            LoadROISetting(m_strFolderPath + "ROI.xml", m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrColorPackageROIs);
            LoadGaugeSetting(m_strFolderPath + "Gauge.xml", m_smVisionInfo.g_arrPackageGauge);
            for (int u = 0; u < m_smVisionInfo.g_arrColorPackage.Count; u++)
            {
                m_smVisionInfo.g_arrColorPackage[u].LoadColorPackage(m_strFolderPath + "Settings.xml", "Settings");
            }

            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrPackageROIs[i].Count; j++)
                {
                    if (j == 0)
                        m_smVisionInfo.g_arrPackageROIs[i][j].AttachImage(m_smVisionInfo.g_arrImages[0]);
                    else
                        m_smVisionInfo.g_arrPackageROIs[i][j].AttachImage(m_smVisionInfo.g_arrPackageROIs[i][0]);
                }

            Close();
            Dispose();
        }

        private void btn_Rotate_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (sender == btn_ClockWise)
                {
                    CImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrColorRotatedImages[i], -90, m_smVisionInfo.g_arrColorRotatedImages[i]);
                    ImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrRotatedImages[i], -90, ref m_smVisionInfo.g_arrRotatedImages, i);
                }
                else if (sender == btn_CounterClockWise)
                {
                    CImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrColorRotatedImages[i], 90, m_smVisionInfo.g_arrColorRotatedImages[i]);
                    ImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrRotatedImages[i], 90, ref m_smVisionInfo.g_arrRotatedImages, i);
                 }
            }
        
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ColorGuideline_Click(object sender, EventArgs e)
        {
            foreach (Form objForm in this.OwnedForms)
            {
                if (objForm.Name == "ColorGuideLine")
                {
                    objForm.TopMost = true;
                    objForm.Focus();
                    return;
                }
            }

            m_smVisionInfo.g_intColorThreshold = m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColor;
            m_smVisionInfo.g_intColorTolerance = m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColorTolerance;
         
            m_blnColorGuideline = true;
            btn_Next.Enabled = false;
            btn_Previous.Enabled = false;
            btn_Threshold.Enabled = false;
            btn_Cancel.Enabled = false;

            ColorGuideLine objGuideForm = new ColorGuideLine(m_smVisionInfo);
            objGuideForm.Location = new Point(720, 120);
            objGuideForm.Show();
            objGuideForm.Owner = this;
            objGuideForm.TopMost = true;
            timer1.Enabled = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            m_blnNextButton = true;
            m_intLearnStepNo++;
            m_intDisplayStepNo++;
            SetupSteps();

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            m_blnNextButton = false;
            m_intLearnStepNo--;
            m_intDisplayStepNo--;
            SetupSteps();

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveGaugeSettings();
            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "Settings.xml");
            m_smVisionInfo.g_arrColorPackage[0].SaveColorPackage(m_strFolderPath + "Settings.xml", false, "Settings", true);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Package", m_smProductionInfo.g_strLotID);
            
            SaveROISettings();

            #region Save Template Image
            // Save Template Image to template folder
            m_smVisionInfo.g_arrColorImages[0].SaveImage(m_strFolderPath + "Template\\OriTemplate0.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 1)
                m_smVisionInfo.g_arrColorImages[1].SaveImage(m_strFolderPath + "Template\\OriTemplate0_Image1.bmp");
            CROI objPackageROI = new CROI();
            objPackageROI.AttachImage(m_smVisionInfo.g_arrColorImages[0]);
            objPackageROI.LoadROISetting(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX,
                                         m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY,
                                         m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                         m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);
            objPackageROI.SaveImage(m_strFolderPath + "Template\\Template0.bmp");
            #endregion

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            Close();
            Dispose();
        }

        private void btn_CopperThreshold_Click(object sender, EventArgs e)
        {
            foreach (Form objForm in this.OwnedForms)
            {
                if (objForm.Name == "ColorThresholdForm")
                {
                    objForm.TopMost = true;
                    objForm.Focus();
                    return;
                }
            }

            m_blnColorThresholdForm = true;
            m_smVisionInfo.g_blnViewCopperObject = false;
            m_smVisionInfo.g_intColorThreshold = m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColor;
            m_smVisionInfo.g_intColorTolerance = m_smVisionInfo.g_arrColorPackage[0].ref_intCopperColorTolerance;

            //m_smVisionInfo.g_blnUseRGBFormat = true;
            //RGBThresholdForm objThresholdForm = new RGBThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrColorPackageROIs[0][1]);
            m_smVisionInfo.g_blnUseRGBFormat = false;
            ColorThresholdForm objThresholdForm = new ColorThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrColorPackageROIs[0][1]);
            objThresholdForm.Location = new Point(750, 120);
            objThresholdForm.Show();
            objThresholdForm.Owner = this;
            objThresholdForm.TopMost = true;
            timer1.Enabled = true;

            btn_Next.Enabled = false;
            btn_Cancel.Enabled = false;
            btn_Previous.Enabled = false;
        }

        private void btn_OxidationThreshold_Click(object sender, EventArgs e)
        {
            foreach (Form objForm in this.OwnedForms)
            {
                if (objForm.Name == "RGBThresholdForm")
                {
                    objForm.TopMost = true;
                    objForm.Focus();
                    return;
                }
            }

            m_blnColorThresholdForm = true;
            m_smVisionInfo.g_blnViewOxidationObject = false;
            m_smVisionInfo.g_intColorThreshold = m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationColor;
            m_smVisionInfo.g_intColorTolerance = m_smVisionInfo.g_arrColorPackage[0].ref_intOxidationColorTolerance;

            //m_smVisionInfo.g_blnUseRGBFormat = true;
            //RGBThresholdForm objThresholdForm = new RGBThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrColorPackageROIs[0][1]);
            m_smVisionInfo.g_blnUseRGBFormat = false;
            ColorThresholdForm objThresholdForm = new ColorThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrColorPackageROIs[0][1]);
            objThresholdForm.Location = new Point(720, 120);
            objThresholdForm.Show();
            objThresholdForm.Owner = this;
            objThresholdForm.TopMost = true;
            timer1.Enabled = true;

            btn_Cancel.Enabled = false;
            btn_Previous.Enabled = false;
            btn_Save.Enabled = false;
        }

        private void btn_ScratchThreshold_Click(object sender, EventArgs e)
        {           
            int intThreshold =  m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrColorPackage[0].ref_intWOBThreshold;

            m_smVisionInfo.g_blnViewWOBObject= false;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(720, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)         
                m_smVisionInfo.g_arrColorPackage[0].ref_intWOBThreshold = m_smVisionInfo.g_intThresholdValue;            
            else          
                m_smVisionInfo.g_intThresholdValue = intThreshold;

            m_smVisionInfo.g_arrColorPackage[0].BuildWOBObjects(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            m_smVisionInfo.g_blnViewWOBObject = true; 
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

     



        private void LearnColorPackageForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;

         
            lbl_StepNo.BringToFront();
            lbl_TitleStep1.BringToFront();
            tabCtrl_Lean.SelectedTab = tp_Step1;

            m_smVisionInfo.VM_AT_SettingInDialog = true;
        }

        private void LearnColorPackageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Color Package Form Closed", "Exit Learn Color Package Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnDragROI = false;       
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewSearchROI = false;             
            m_smVisionInfo.g_blnViewPackageTrainROI = false;           
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewCopperObject = false;
            m_smVisionInfo.g_blnViewWOBObject = false; 
            m_smVisionInfo.g_blnViewOxidationObject = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_blnColorThresholdForm && !m_smVisionInfo.g_blnGetPixel)
            {
                if (m_blnSetCopper)
                {
                    m_smVisionInfo.g_arrColorPackage[0].SetCopperBlobThreshold(m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance);
                    ImageDrawing objImage = new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                    ROI objNewROI = new ROI();
                    objNewROI.LoadROISetting(m_smVisionInfo.g_arrColorPackageROIs[0][1].ref_ROITotalX, m_smVisionInfo.g_arrColorPackageROIs[0][1].ref_ROITotalY,
                        m_smVisionInfo.g_arrColorPackageROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrColorPackageROIs[0][1].ref_ROIHeight);
                    objNewROI.AttachImage(objImage);
                    m_smVisionInfo.g_arrColorPackage[0].BuildExposedCopper(m_smVisionInfo.g_arrColorPackageROIs[0][1], objNewROI);
                    m_smVisionInfo.g_blnViewCopperObject = true;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    timer1.Enabled = false;

                    btn_Cancel.Enabled = true;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;                    
                }
                else if (m_blnSetOxidation)
                {
                    m_smVisionInfo.g_arrColorPackage[0].SetOxidationBlobThreshold(m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance);
                    m_smVisionInfo.g_arrColorPackage[0].BuildOxidazation(m_smVisionInfo.g_arrColorPackageROIs[0][1]);
                    m_smVisionInfo.g_blnViewOxidationObject = true;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    timer1.Enabled = false;

                    btn_Cancel.Enabled = true;
                    btn_Previous.Enabled = true;
                    btn_Save.Enabled = true;
                }
                m_blnColorThresholdForm = false;
            }

            if (m_blnColorGuideline && !m_smVisionInfo.VM_AT_ColorGuideline)
            {
                m_smVisionInfo.g_arrColorPackage[0].SetCopperBlobThreshold(m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance);
                ImageDrawing objImage = new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                ROI objNewROI = new ROI();
                objNewROI.LoadROISetting(m_smVisionInfo.g_arrColorPackageROIs[0][1].ref_ROITotalX, m_smVisionInfo.g_arrColorPackageROIs[0][1].ref_ROITotalY,
                    m_smVisionInfo.g_arrColorPackageROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrColorPackageROIs[0][1].ref_ROIHeight);
                objNewROI.AttachImage(objImage);
                m_smVisionInfo.g_arrColorPackage[0].BuildExposedCopper(m_smVisionInfo.g_arrColorPackageROIs[0][1], objNewROI);
                m_smVisionInfo.g_blnViewCopperObject = true;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                btn_Next.Enabled = true;
                btn_Previous.Enabled = true;
                btn_Cancel.Enabled = true;
                btn_Threshold.Enabled = true;
                m_blnColorGuideline = false;
                timer1.Enabled = false;
            }
        }      
    }
}