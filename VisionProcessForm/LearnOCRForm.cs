using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;
using VisionProcessing;
using System.IO;

namespace VisionProcessForm
{
    public partial class LearnOCRForm : Form
    {
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private VisionInfo m_smVisionInfo;
        private string m_strFolderPath;
        private string m_strSelectedRecipe;
        private string[] m_strDatabaseName;
        private int m_intDisplayStepNo = 1;
        private int m_intUserGroup = 5;
        private int m_intLearnType = 0;
        private int m_intSelectedIndex = 0;
        private int m_intCurrentAngle = 0;
        private int m_intCurrentPreciseDeg = 0;
        private int m_intSelectedDontCareROIIndexPrev = 0;
        private bool m_blnFirstTimeInit = false;
        private bool m_blnInitDone = false;
        private bool m_blnInitGaugeAngle = false;
        private bool m_blnViewROITool;
        private bool m_blnSetText = false;
        private PictureBox[] pic_Template = new PictureBox[8];
        private Panel[] pic_Panel = new Panel[8];
        private int m_intTempForSelectedIndex;
        private Bitmap bmp_Mark1; // image for draw line
        private Bitmap bmp_Mark2;
        private Bitmap bmp_Mark3;
        private Bitmap[] bmp_pic = new Bitmap[8];
        private Bitmap Ori_Mark1; //if uncheck load back ori image
        private Bitmap Ori_Mark2;
        private Bitmap Ori_Mark3;
        private Bitmap[] Ori_pic = new Bitmap[8];
        private List<List<int>> CharCount = new List<List<int>>();
        private List<List<List<string>>> arrCharType = new List<List<List<string>>>();
        public LearnOCRForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intLearnType)
        {
            InitializeComponent();
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;

            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;
            m_intLearnType = intLearnType;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            DisableField2();
            //LoadMarkTemplate();
            UpdateTemplateGUI();

            if (m_smVisionInfo.g_intMarkDefectInspectionMethod == 1)
            {
                btn_MarkGrayValueSensitivitySetting.Visible = true;
                btn_Threshold.Visible = false;
            }
            else
            {
                btn_MarkGrayValueSensitivitySetting.Visible = false;
                btn_Threshold.Visible = true;
            }

            m_smVisionInfo.g_blnHasRecognise = false;
            m_smVisionInfo.g_blnHasDetected = false;

            switch (m_intLearnType)
            {
                case 0:
                    m_smVisionInfo.g_intLearnStepNo = 0;
                    break;
            }

            m_blnInitDone = true;
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Mark";
            string strChild2 = "Learn Page";
            string strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_Save.Enabled = false;
                btn_ROISaveClose.Enabled = false;
            }

            // ----------------------------- 

            //if (m_smVisionInfo.g_blnWantMultiTemplates)
            //{
            //    chk_DeleteAllTemplates.Visible = true;
            //}
            //else
            {
                m_intSelectedIndex = 0;
                chk_DeleteAllTemplates.Visible = false;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
            }

            if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                srmGroupBox2.Visible = false;   // Not allow to rotate image image 90 deg since want orient 0 deg only

            }
            else if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
            {
                srmGroupBox2.Visible = false;   // Not allow to rotate image image 90 deg because InPocket accept 1 direction only.
            }
            else
                srmGroupBox2.Visible = true;

            radio_LearnManual.Checked = true;
            btn_LoadDatabase.Visible = false;
        }
        private int GetUserRightGroup_Child3(string Child1, string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    Child1 = "Orient";
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(Child1, Child2, Child3);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild3Group(Child1, Child2, Child3);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(Child1, Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
            }

            return 1;
        }

        private void AddSearchROI(int intUnitNo, List<List<ROI>> arrROIs, List<RectGaugeM4L> arrGauge)
        {
            for (int i = arrROIs.Count - 1; i < intUnitNo; i++)
                arrROIs.Add(new List<ROI>());

            ROI objROI = new ROI();

            if (arrROIs[intUnitNo].Count == 0)
            {
                if (intUnitNo == 0)
                {
                    // 2019 06 21 - CCENG: if 1 unit only, then display Search ROI, if 2 units, then display Test ROI and Retest ROI
                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        objROI = new ROI("Search ROI", 1);
                    else
                        objROI = new ROI("Test ROI", 1);
                }
                else
                    objROI = new ROI("ReTest ROI", 1);

                if (intUnitNo == 0)
                {
                    int intPositionX = 640 - (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - 100;
                    int intPositionY = (480 / 2) - 100;
                    objROI.LoadROISetting(intPositionX, intPositionY, 200, 200);
                }
                else if (intUnitNo != 0)
                {
                    int intPositionX = (640 / (m_smVisionInfo.g_intUnitsOnImage * 2)) - ((arrROIs[0][0]).ref_ROIWidth / 2);
                    objROI.LoadROISetting(intPositionX, arrROIs[0][0].ref_ROIPositionY,
                        arrROIs[0][0].ref_ROIWidth,
                        arrROIs[0][0].ref_ROIHeight);
                }

                objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                arrROIs[intUnitNo].Add(objROI);
            }
            else
            {
                if (intUnitNo == 0)
                {
                    // 2019 06 21 - CCENG: if 1 unit only, then display Search ROI, if 2 units, then display Test ROI and Retest ROI
                    if (m_smVisionInfo.g_intUnitsOnImage == 1)
                        arrROIs[intUnitNo][0].ref_strROIName = "Search ROI";
                    else
                        arrROIs[intUnitNo][0].ref_strROIName = "Test ROI";
                }
                else
                    arrROIs[intUnitNo][0].ref_strROIName = "ReTest ROI";

                arrROIs[intUnitNo][0].AttachImage(m_smVisionInfo.g_arrImages[0]);   // 2021 05 31 - CCENG: Search ROI need to reattached to image. Sometime we dun know the search ROI has been attached to other image in outside test.
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void AddTrainROI(int intUnitNo, List<List<ROI>> arrROIs, int intROIIndex, string strROIName)
        {
            for (int i = arrROIs[intUnitNo].Count; i <= intROIIndex; i++)
            {
                ROI objROI = new ROI();
                if (intUnitNo != 0)
                {
                    objROI.LoadROISetting(arrROIs[0][1].ref_ROIPositionX,
                        arrROIs[0][1].ref_ROIPositionY,
                        arrROIs[0][1].ref_ROIWidth,
                        arrROIs[0][1].ref_ROIHeight);
                }
                arrROIs[intUnitNo].Add(objROI);
            }
            if (m_smVisionInfo.g_blnWantGauge && strROIName == "Unit PR ROI")
            {
                if (m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fRectWidth != 0 && m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fRectHeight != 0)
                    arrROIs[intUnitNo][intROIIndex].LoadROISetting((int)((m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fRectWidth / 2) - (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2)),
                        (int)((m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fRectHeight / 2) - (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2)),
                        (int)m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fRectWidth,
                        (int)m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fRectHeight);
            }
            arrROIs[intUnitNo][intROIIndex].ref_strROIName = strROIName;
            arrROIs[intUnitNo][intROIIndex].ref_intType = 2;
            arrROIs[intUnitNo][intROIIndex].AttachImage(arrROIs[intUnitNo][0]); // Train ROI always attach to Search ROI
        }

        private bool BuildMarkObjects()
        {
            //STTrackLog.WriteLine("BuildMarkObjects - 1");
            m_smVisionInfo.g_blnViewCharsBuilded = false;
            m_smVisionInfo.g_blnViewTextsBuilded = false;
            //STTrackLog.WriteLine("BuildMarkObjects - 2");
            if (!m_smVisionInfo.g_blnWhiteOnBlack)
                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_blnWhiteOnBlack = false;
            else
                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_blnWhiteOnBlack = true;

            if (m_smVisionInfo.g_intMarkDefectInspectionMethod == 0)
            {
                //STTrackLog.WriteLine("BuildMarkObjects - 3");
                //if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].BuildLearnObject(m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1], false, true))
                if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].BuildLearnObject(m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_blnWantRemoveBorderWhenLearnMark, true))    // 2020 11 03 - CCENG: Try remove border for mark with lead test. This will auto remove the lead object during learn mark.
                {
                    //STTrackLog.WriteLine("BuildMarkObjects - 4");
                    m_smVisionInfo.g_blnViewObjectsBuilded = true;
                }
                else
                {
                    //STTrackLog.WriteLine("BuildMarkObjects - 5");
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    return false;
                }
            }
            else
            {
                //STTrackLog.WriteLine("BuildMarkObjects - 3");
                if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].BuildLearnObject_UsingGrayValue(m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_blnWantRemoveBorderWhenLearnMark, true, m_smVisionInfo.g_intMarkInspectionAreaGrayValueSensitivity, m_smVisionInfo.g_intMarkBrightSensitivity))
                {
                    //STTrackLog.WriteLine("BuildMarkObjects - 4");
                    m_smVisionInfo.g_blnViewObjectsBuilded = true;
                }
                else
                {
                    //STTrackLog.WriteLine("BuildMarkObjects - 5");
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    return false;
                }
            }
            return true;
        }

        private void Rotate0Degree(int intImageIndex, List<List<ROI>> arrROIs, List<RectGaugeM4L> arrGauge)
        {
            // Rotate for both unit 1 and 2 (i represent unit index)
            for (int u = 0; u < arrROIs.Count; u++)
            {
                // Make sure search ROI is attached to main ori image before measure by gauge. 
                // ROI should not attach to rotated image which will give not actual unit phyical angle.
                //arrROIs[u][0].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                arrROIs[u][0].AttachImage(m_smVisionInfo.g_objPackageImage);

                // Get gauge angle by measuring search roi
                arrGauge[u].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, arrROIs[u][0], m_smVisionInfo.g_objWhiteImage);
                float fGaugeAngle = arrGauge[u].ref_fRectAngle;
                if (fGaugeAngle != 0.0f)
                {
                    // rotate image
                    if (m_smVisionInfo.g_blnViewRotatedImage)
                    {
                        ImageDrawing objSourcImage = new ImageDrawing();
                        m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref objSourcImage);
                        ROI.Rotate0Degree(objSourcImage, arrROIs[u][0], fGaugeAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);
                        objSourcImage.Dispose();
                    }
                    else
                        ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], arrROIs[u][0], fGaugeAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

                    m_smVisionInfo.g_blnViewRotatedImage = true;
                }
            }

            //Attach search roi to new rotated image
            for (int u = 0; u < arrROIs.Count; u++)
            {
                arrROIs[u][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            }
        }

        private void SaveDontCareROISettings(string strFolderPath, List<ROI> arrROIs, string name)
        {
            //
            //STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "DontCareROI.xml", true);
            int i, j;
            ROI objROI = new ROI();
            objFile.WriteSectionElement("Unit0");
            for (i = 0; i < arrROIs.Count; i++)
            {
                //objROI = arrROIs[i];
                objFile.WriteElement1Value("ROI" + i, "");

                objFile.WriteElement2Value("Name", arrROIs[i].ref_strROIName);
                objFile.WriteElement2Value("Type", arrROIs[i].ref_intType);
                objFile.WriteElement2Value("PositionX", arrROIs[i].ref_ROIPositionX);
                objFile.WriteElement2Value("PositionY", arrROIs[i].ref_ROIPositionY);
                objFile.WriteElement2Value("Width", arrROIs[i].ref_ROIWidth);
                objFile.WriteElement2Value("Height", arrROIs[i].ref_ROIHeight);

                arrROIs[i].ref_intStartOffsetX = m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterX - arrROIs[i].ref_ROITotalX;
                arrROIs[i].ref_intStartOffsetY = m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterY - arrROIs[i].ref_ROITotalY;

                objFile.WriteElement2Value("StartOffsetX", arrROIs[i].ref_intStartOffsetX);
                objFile.WriteElement2Value("StartOffsetY", arrROIs[i].ref_intStartOffsetY);

                float fPixelAverage = arrROIs[i].GetROIAreaPixel();
                objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                arrROIs[i].SetROIPixelAverage(fPixelAverage);

                if (arrROIs[i].ref_intType > 1)
                {
                    arrROIs[i].ref_ROIOriPositionX = arrROIs[i].ref_ROIPositionX;
                    arrROIs[i].ref_ROIOriPositionY = arrROIs[i].ref_ROIPositionY;
                }

                objFile.WriteEndElement();
                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName +"-"+name+ " ROI", strFolderPath, "ROI.xml");
            }
        }
        private void SaveROISettings(string strFolderPath, List<List<ROI>> arrROIs, string name)
        {
            //
            //STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "ROI.xml", true);
            int i, j;
            ROI objROI = new ROI();
            for (i = 0; i < arrROIs.Count; i++)
            {
                objFile.WriteSectionElement("Unit" + i);
                for (j = 0; j < arrROIs[i].Count; j++)
                {
                    objROI = arrROIs[i][j];
                    if (j == 1 && i != m_smVisionInfo.g_intSelectedUnit)
                    {
                        ROI objSelectedROI = arrROIs[m_smVisionInfo.g_intSelectedUnit][j];
                        objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
                        objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
                        objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
                        objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
                    }

                    objFile.WriteElement1Value("ROI" + j, "");

                    objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                    objFile.WriteElement2Value("Type", objROI.ref_intType);
                    objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                    objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
                    float fPixelAverage = objROI.GetROIAreaPixel();
                    objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                    objROI.SetROIPixelAverage(fPixelAverage);

                    if (objROI.ref_intType > 1)
                    {
                        objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                        objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    }
                }
                objFile.WriteEndElement();
                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName +"-"+name+ " ROI", strFolderPath, "ROI.xml");
            }
        }

        private void SaveDontCareSetting(string strFolderPath)
        {
            ImageDrawing objImage = new ImageDrawing(true);
            ImageDrawing objFinalImage = new ImageDrawing(true);
            m_smVisionInfo.g_objBlackImage.CopyTo(objFinalImage);
            for (int i = 0; i < m_smVisionInfo.g_arrMarkDontCareROIs.Count; i++)
            {

                if (m_smVisionInfo.g_intSelectedUnit < m_smVisionInfo.g_arrPolygon_Mark.Count && i < m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count)
                {
                    if (m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i].ref_intFormMode != 2)
                        Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    else
                        Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                    ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);


                    ////Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    //if (m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate].ref_intFormMode != 2)
                    //    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    //else
                    //    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                    //ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);
                }
            }
            objFinalImage.SaveImage(strFolderPath + "DontCareImage0" + "_" + m_intSelectedIndex + ".bmp");
            objImage.Dispose();
            objFinalImage.Dispose();
            objImage = null;
            objFinalImage = null;
            //for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Mark.Count; u++)
            //{
            //    for (int i = m_smVisionInfo.g_arrPolygon_Mark[u].Count - 1; i > 0; i--)
            //    {
            //        if (i > m_intSelectedIndex)
            //        {
            //            m_smVisionInfo.g_arrPolygon_Mark[u].RemoveAt(i);
            //        }
            //    }
            //}

            Polygon.SavePolygon(strFolderPath + "Polygon.xml", m_smVisionInfo.g_arrPolygon_Mark);
        }

        private void LearnOCRForm_Load(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_blnViewROITool = m_smProductionInfo.g_blnViewROITool;
            m_smProductionInfo.g_blnViewROITool = false;
            Cursor.Current = Cursors.Default;
            m_blnFirstTimeInit = true;
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                AddSearchROI(i, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkGaugeM4L);
            }

            //m_smVisionInfo.g_arrMarks[0].AddTemplate(true);

            // Set initial value for learning
            m_smVisionInfo.g_blnCutMode = radioBtn_CutObj.Checked;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = 0;
            //m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnDragROI = true;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnDrawMarkResult = false;    // Make sure drawing set to false in learning form after production set it to true.
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            if (m_smVisionInfo.g_arrMarkDontCareROIs.Count > 0)
            {
                if (m_smVisionInfo.g_intSelectedUnit < m_smVisionInfo.g_arrPolygon_Mark.Count)
                {
                    // 2020 03 16 - CCENG: Sometime g_arrMarkDontCareROIs count != g_arrPolygon_Mark count due to old recipe to new recipe.
                    if (m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex &&
                    m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                    {
                        cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
                    }
                    else
                    {
                        // 2020 03 16 - CCENG: Clear all g_arrMarkDontCareROIs if counter not same
                        //m_smVisionInfo.g_arrMarkDontCareROIs.Clear();
                        cbo_DontCareAreaDrawMethod.SelectedIndex = 0;
                    }
                }
            }
            else
                cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

            if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
            {
                pnl_DontCareFreeShape.Visible = true;
            }
            else
            {
                pnl_DontCareFreeShape.Visible = false;
            }

            m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;

            m_smVisionInfo.g_blnViewGauge = false;
            lbl_StepNo.BringToFront();

            SetupSteps();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }


        private void SetupSteps()
        {
            STTrackLog.WriteLine("STEP=" + m_smVisionInfo.g_intLearnStepNo.ToString());

            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0: //Define Search ROI
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewOrientTrainROI = false;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnViewUnitROI = false;
                    //m_smVisionInfo.g_strContextMenuType = "Empty"; // 2019-10-22 ZJYEOH : Hide to enable Zoom In/Out Tool
                    m_smVisionInfo.g_blnUpdateContextMenu = true;

                    if (m_blnInitGaugeAngle || m_blnFirstTimeInit) //!
                    {
                        m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
                        for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
                        {
                            m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
                        }
                    }

                    // 2018 12 31 - CCENG: No Gain for image without gauge.
                    if (m_smVisionInfo.g_blnViewRotatedImage)
                        m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
                    else
                        m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);

                    btn_Next.Enabled = true;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewMarkTrainROI = false;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    tabCtrl_OCR.SelectedTab = tp_SearchROI;
                    btn_Previous.Enabled = false;
                    lbl_SearchROI.BringToFront();
                    break;

                case 1: //Define Mark ROI

                    Rotate0Degree(0, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkGaugeM4L);

                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        AddTrainROI(i, m_smVisionInfo.g_arrMarkROIs, 1, "Mark ROI");
                    }

                    // 2019 07 31 - JBTAN: Remove Mark ROI border line when click previous from step 7
                    m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewObjectsBuilded = false;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewMarkTrainROI = true;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    m_smVisionInfo.g_blnViewOrientTrainROI = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    m_smVisionInfo.g_blnViewSubROI = false;

                    tabCtrl_OCR.SelectedTab = tp_MarkROI;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    lbl_MarkROI.BringToFront();
                        break;
                case 2: //Build Object
                    // Draw Mark ROI border line with cut mode color to prevent build blob error when mark touch the ROI border.
                    m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPackageImage);

                    // 2020 11 03 - CCENG: Try remove border for mark with lead test. This will auto remove the lead object during learn mark.
                    //                   : Border object will only be removed if no DrawBorderLine.
                    if (!m_smVisionInfo.g_blnWantRemoveBorderWhenLearnMark)
                        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].DrawBorderLine(m_smVisionInfo.g_blnWhiteOnBlack);

                    //temporary create dont care template for build object
                    if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrMarkDontCareROIs.Count; i++)
                        {
                            if (m_smVisionInfo.g_intSelectedUnit < m_smVisionInfo.g_arrPolygon_Mark.Count && i < m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count)
                            {
                                if (m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i].ref_intFormMode != 2)
                                {
                                    m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i].AddPoint(new PointF(m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                                        m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                                    m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i].AddPoint(new PointF((m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROITotalX + m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                                        (m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROITotalY + m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                                    m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i].AddPolygon((int)(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i].ResetPointsUsingOffset(m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                                    m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i].AddPolygon((int)(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                                }
                                ImageDrawing objImage = new ImageDrawing(true);
                                if (m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i].ref_intFormMode != 2)
                                    Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                                else
                                    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][i], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                                ROI objDontCareROI = new ROI();
                                objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                                objDontCareROI.AttachImage(objImage);
                                //2020-06-17 ZJYEOH : Subtract or Add depends on white on black or black on white
                                if (m_smVisionInfo.g_blnWhiteOnBlack)
                                    ROI.SubtractROI(m_smVisionInfo.g_arrMarkROIs[0][1], objDontCareROI);
                                else
                                    ROI.LogicOperationAddROI(m_smVisionInfo.g_arrMarkROIs[0][1], objDontCareROI);
                                objDontCareROI.Dispose();
                                objImage.Dispose();
                            }
                        }
                    }

                    //2020-05-12 ZJYEOH : load previous template threshold setting when learn new template
                    if (Mark1.Image == null && m_intSelectedIndex == 1 || Mark2.Image == null && m_intSelectedIndex == 2 || Mark3.Image == null && m_intSelectedIndex == 3)
                    {
                        int intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, m_intSelectedIndex - 1);
                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                        {
                            m_smVisionInfo.g_arrMarks[u].SetThreshold(intThresholdValue, m_smVisionInfo.g_intSelectedGroup, m_intSelectedIndex);
                        }

                        for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
                        {
                            if (pic_Template[i].Image == null && m_intSelectedIndex == i)
                            {
                                intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetThreshold(m_smVisionInfo.g_intSelectedGroup, m_intSelectedIndex - 1);
                                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                {
                                    m_smVisionInfo.g_arrMarks[u].SetThreshold(intThresholdValue, m_smVisionInfo.g_intSelectedGroup, m_intSelectedIndex);
                                }
                            }

                        }
                    }
                    else
                    {
                        //2020-05-13 ZJYEOH : Set previous template threshold when learn new template after new lot clear all template
                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                        {
                            m_smVisionInfo.g_arrMarks[u].SetCurrentThreshold(m_strFolderPath + "Mark\\Template\\", m_smVisionInfo.g_intSelectedGroup, m_intSelectedIndex);
                        }
                    }

                    if (!BuildMarkObjects())
                    {
                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intMinArea = 1;
                        txt_MinArea.Text = "1";
                        if (!BuildMarkObjects())
                        {
                            //SRMMessageBox.Show("Please define Image area first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //m_smVisionInfo.g_intLearnStepNo--;
                            //m_intDisplayStepNo--;
                            //break;
                        }
                    }

                    //m_smVisionInfo.g_strContextMenuType = "Empty";
                    m_smVisionInfo.g_strContextMenuType = "Production"; // 2019-10-22 ZJYEOH : Enable Zoom In/Out Tool
                    m_smVisionInfo.g_blnUpdateContextMenu = true;

                    m_smVisionInfo.g_blnDragROI = false;


                    tabCtrl_OCR.SelectedTab = tp_Segmentation;
                    btn_Next.Visible = true;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    lbl_Segmentation.BringToFront();
                    break;
                case 3:
                    if (!m_smVisionInfo.g_blnViewObjectsBuilded)
                    {
                        SRMMessageBox.Show("Please build objects first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        m_smVisionInfo.g_intLearnStepNo--;
                        m_intDisplayStepNo--;
                        break;
                    }

                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].AutoDefineTopology();

                    if (!m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRDetect(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMinWidth, m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMaxWidth, m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharHeight, m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_strTopologyValue))
                        SRMMessageBox.Show(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCR2GetMessage(1), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        m_smVisionInfo.g_blnHasDetected = true;

                    DefineCurrentTopology();
                   
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewMarkTrainROI = true;
                    m_smVisionInfo.g_strContextMenuType = "Mark Setting";
                    m_smVisionInfo.g_blnUpdateContextMenu = true;
                    btn_Next.Visible = false;
                    tabCtrl_OCR.SelectedTab = tp_Detection;
                    lbl_DnR.BringToFront();
                    break;
                //case 4: //Define Don Care Area't
                //    for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Mark.Count; u++)
                //    {
                //        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Mark[u].Count; i++)
                //        {
                //            m_smVisionInfo.g_arrPolygon_Mark[u][i].ClearPolygon();
                //        }
                //    }

                //    ////Check is previously learn dont care area out of ROI Area
                //    //for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Mark.Count; u++)
                //    //{
                //    //    for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Mark[u].Count; i++)
                //    //    {
                //    //        m_smVisionInfo.g_arrPolygon_Mark[u][i].CheckDontCarePosition(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                //    //    }
                //    //}

                //    for (int i = 0; i < m_smVisionInfo.g_arrMarkDontCareROIs.Count; i++)
                //    {

                //        m_smVisionInfo.g_arrMarkDontCareROIs[i].AttachImage(m_smVisionInfo.g_arrMarkROIs[0][1]);

                //    }


                //    // 2019 07 31 - JBTAN: Remove Mark ROI border line when click previous from step 7
                //    m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                //    //if (m_smVisionInfo.g_arrMarkDontCareROIs == null)
                //    //{
                //    //    m_smVisionInfo.g_arrMarkDontCareROIs.Add(new ROI());
                //    //}
                //    //if (m_smVisionInfo.g_arrMarkDontCareROIs.Count == 0)
                //    //    m_smVisionInfo.g_arrMarkDontCareROIs.Add(new ROI());
                //    //m_smVisionInfo.g_arrMarkDontCareROIs[0].AttachImage(m_smVisionInfo.g_arrMarkROIs[0][1]);

                //    m_smVisionInfo.g_blnDragROI = true;
                //    m_smVisionInfo.g_blnViewSearchROI = false;
                //    m_smVisionInfo.g_blnViewMarkTrainROI = true;
                //    m_smVisionInfo.g_blnViewMOGauge = false;
                //    m_smVisionInfo.g_blnViewGauge = false;
                //    m_smVisionInfo.g_blnViewPackageImage = true;
                //    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                //    m_smVisionInfo.g_blnViewOrientTrainROI = false;
                //    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                //    m_smVisionInfo.g_blnViewSubROI = false;
                //    tabCtrl_OCR.SelectedTab = tp_DunCare;
                //    lbl_DunCare.BringToFront();
                //    break;
                default:
                    break;
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

        private void btn_ROISaveClose_Click(object sender, EventArgs e)
        {
            //SaveROISettings(m_strFolderPath + "Mark\\", m_smVisionInfo.g_arrMarkROIs, "Mark");
            XmlParser objFile = new XmlParser(m_strFolderPath + "Mark\\" + "Template\\Template.xml");
            ROI objROI;
            //STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
            objFile = new XmlParser(m_strFolderPath + "Mark\\" + "ROI.xml", false);
            for (int x = 0; x < m_smVisionInfo.g_arrMarkROIs.Count; x++)
            {
                objFile.WriteSectionElement("Unit" + x);
                for (int j = 0; j < m_smVisionInfo.g_arrMarkROIs[x].Count; j++)
                {
                    if (j == 0)
                    {
                        objROI = m_smVisionInfo.g_arrMarkROIs[x][j];
                        if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                        {
                            ROI objSelectedROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][j];
                            objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
                            objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
                            objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
                            objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
                        }

                        objFile.WriteElement1Value("ROI" + j, "");

                        objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                        objFile.WriteElement2Value("Type", objROI.ref_intType);
                        objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                        objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                        objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                        objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                        objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                        objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
                        //float fPixelAverage = objROI.GetROIAreaPixel();
                        //objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                        //objROI.SetROIPixelAverage(fPixelAverage);

                        objFile.WriteEndElement();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient ROI", strFolderPath, "ROI.xml");
                    }
                }
            }


            //if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
            //{
            //    LoadMarkSettings(m_strFolderPath + "Mark\\Template\\");
            //}

            // Empty Search ROI
            if (m_smVisionInfo.g_arrPositioningROIs != null)
            {
                if (m_smVisionInfo.g_arrPositioningROIs.Count > 0)
                {
                    m_smVisionInfo.g_arrPositioningROIs[0].LoadROISetting(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY,
                        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);

                    string strEmptyFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Positioning\\";
                    ROI.SaveFile(strEmptyFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPositioningROIs);
                }
            }
            this.Close();
            this.Dispose();

        }

        private void btn_AddDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPolygon_Mark.Count < m_smVisionInfo.g_intUnitsOnImage)
                m_smVisionInfo.g_arrPolygon_Mark.Add(new List<Polygon>());

            // 2020 03 25 - CCENG: Clear array if count not tally. It is bcos old recipe dont care format not compatible with new dont care format.
            if (m_smVisionInfo.g_arrMarkDontCareROIs.Count != m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count)
            {
                m_smVisionInfo.g_arrPolygon_Mark.Clear();
                m_smVisionInfo.g_arrPolygon_Mark.Add(new List<Polygon>());

                m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Clear();
                m_smVisionInfo.g_arrMarkDontCareROIs.Clear();
            }

            m_smVisionInfo.g_arrMarkDontCareROIs.Add(new ROI());
            m_smVisionInfo.g_arrMarkDontCareROIs[m_smVisionInfo.g_arrMarkDontCareROIs.Count - 1].AttachImage(m_smVisionInfo.g_arrMarkROIs[0][1]);

            m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Add(new Polygon());
            m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrMarkDontCareROIs.Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DeleteDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPolygon_Mark.Count < m_smVisionInfo.g_intUnitsOnImage)
                m_smVisionInfo.g_arrPolygon_Mark.Add(new List<Polygon>());


            if (m_smVisionInfo.g_arrMarkDontCareROIs.Count != m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count)
            {
                m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Clear();
                m_smVisionInfo.g_arrMarkDontCareROIs.Clear();

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;  // 2020 03 30 - CCENG: update drawing after g_arrMarkDontCareROIs is clear();
            }

            if (m_smVisionInfo.g_arrMarkDontCareROIs.Count == 0)
            {
                return;
            }

            m_smVisionInfo.g_arrMarkDontCareROIs.RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrMarkDontCareROIs.Count - 1;
            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if (m_smVisionInfo.g_arrMarkDontCareROIs.Count > 0 &&
                    m_smVisionInfo.g_intSelectedDontCareROIIndex < m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count &&
                    m_smVisionInfo.g_intSelectedDontCareROIIndex < m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count)
                {
                    cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
                }
                else
                    cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

                if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
                {
                    pnl_DontCareFreeShape.Visible = true;
                }
                else
                {
                    pnl_DontCareFreeShape.Visible = false;
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            m_blnFirstTimeInit = false;
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Mark":
                case "MarkPkg":
                case "MOPkg":
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 0)
                        {
                                m_smVisionInfo.g_intLearnStepNo = 1;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 1)
                        {
                            m_smVisionInfo.g_intLearnStepNo = 2;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 2)
                        {
                                m_smVisionInfo.g_intLearnStepNo = 3;
                        }
                    }
                    break;
            }



            m_intDisplayStepNo++;

            SetupSteps();

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo == 3)
            {
                Graphics g = pnl_pic.CreateGraphics();
                g.Clear(Color.White);
                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_HitObject = null;
                txt_SetChar.Text = "";
            }

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Mark":
                case "MarkPkg":
                case "MOPkg":
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 1)
                        {
                            m_smVisionInfo.g_intLearnStepNo = 0;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 2)
                        {
                            m_smVisionInfo.g_intLearnStepNo = 1;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 3)
                        {
                            m_smVisionInfo.g_intLearnStepNo = 2;
                        }
                    }
                    break;
            }

            m_intDisplayStepNo--;

            SetupSteps();

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;          
        }

        private void btn_RotateUnit_Click(object sender, EventArgs e)
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;

            ROI objROI = new ROI();
            List<List<ROI>> arrROIs = new List<List<ROI>>();
            objROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0];
            arrROIs = m_smVisionInfo.g_arrMarkROIs;

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                objRotatedImage = m_smVisionInfo.g_arrRotatedImages[i];
                m_smVisionInfo.g_arrImages[i].CopyTo(ref objRotatedImage);
            }

            // Rotate Unit IC
            m_intCurrentAngle += 180;
            m_intCurrentAngle %= 360;

            ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);
            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            // After rotating the image, attach the rotated image into ROI again
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrMarkROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            }

            // 2018 12 31 - CCENG: No Gain for image without gauge.
            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
            else
                m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage); // 02-07-2019 ZJYEOH : Solve rotate first time no effect
            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewGauge = false;  // 2019 06 21 - Gauge will measure according to original image only. Once rotate, gauge wont measure anymore, so not need to view Gauge after that.
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;

            if (m_intCurrentAngle == 270)
                lbl_OrientationAngle.Text = "-90";
            else
                lbl_OrientationAngle.Text = m_intCurrentAngle.ToString();

            RotatePrecise();
        }

        private void RotatePrecise()
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;

            ROI objROI = new ROI();
            List<List<ROI>> arrROIs = new List<List<ROI>>();
            objROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0];
            arrROIs = m_smVisionInfo.g_arrMarkROIs;

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);

            for (int i = 0; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, i);
            }

            if (m_smVisionInfo.g_blnViewPackageImage)
                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            // After rotating the image, attach the rotated image into ROI again
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                m_smVisionInfo.g_arrMarkROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            }

            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewGauge = false;  // 2019 06 21 - Gauge will measure according to original image only. Once rotate, gauge wont measure anymore, so not need to view Gauge after that.
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " deg";
            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
        }

        private void btn_ClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg++;
            RotatePrecise();
        }

        private void btn_CounterClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg--;
            RotatePrecise();
        }

        private void UpdateTemplateGUI()
        {
            //if (!m_smVisionInfo.g_blnWantMultiTemplates)
            //    return;

            //cbo_TemplateNo.Items.Clear();

            //if (chk_DeleteAllTemplates.Checked)
            //{
            //    //cbo_TemplateNo.Items.Add("Template 1");
                m_smVisionInfo.g_intSelectedTemplate = 0;
                m_intSelectedIndex = m_smVisionInfo.g_intSelectedTemplate;
            //}
            //else
            //{
            //    //// Update Template GUI
            //    //for (int i = 0; i < m_smVisionInfo.g_intTotalTemplates; i++)
            //    //{
            //    //    cbo_TemplateNo.Items.Add("Template " + (i + 1));
            //    //}
  
            //    //switch (m_smVisionInfo.g_strVisionName)
            //    //{
            //    //    case "Mark":
            //    //    case "MarkOrient":
            //    //    case "MarkPkg":
            //    //    case "MOPkg":
            //    //    case "MOLiPkg":
            //    //    case "MOLi":
            //    //        if (m_smVisionInfo.g_intTotalTemplates == 0)
            //    //            cbo_TemplateNo.Items.Add("Template 1");
            //    //        else if (m_smVisionInfo.g_intTotalTemplates < 8 && m_smVisionInfo.g_intTotalTemplates < m_smVisionInfo.g_intMaxMarkTemplate)
            //    //            cbo_TemplateNo.Items.Add("New...");

            //    //        if ((m_smVisionInfo.g_intTotalTemplates == 1) || (m_smVisionInfo.g_intTotalTemplates == 8) || m_smVisionInfo.g_intTotalTemplates == m_smVisionInfo.g_intMaxMarkTemplate)
            //    //            m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex = 0;
            //    //        else if ((m_smVisionInfo.g_intSelectedTemplate < m_smVisionInfo.g_intTotalTemplates) && (m_smVisionInfo.g_intSelectedTemplate != -1))
            //    //            m_intSelectedIndex = m_smVisionInfo.g_intSelectedTemplate;
            //    //        else
            //    //            m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex = m_smVisionInfo.g_intTotalTemplates;
            //    //        break;
            //    //    case "InPocket":
            //    //    case "InPocketPkg":
            //    //    case "InPocketPkgPos":
            //    //    case "IPMLi":
            //    //    case "IPMLiPkg":
            //    //        if (m_smVisionInfo.g_intTotalTemplates == 0)
            //    //            cbo_TemplateNo.Items.Add("Template 1");
            //    //        else if (m_smVisionInfo.g_intTotalTemplates < 4 && m_smVisionInfo.g_intTotalTemplates < m_smVisionInfo.g_intMaxMarkTemplate)
            //    //            cbo_TemplateNo.Items.Add("New...");

            //    //        if ((m_smVisionInfo.g_intTotalTemplates == 1) || (m_smVisionInfo.g_intTotalTemplates == 4) || m_smVisionInfo.g_intTotalTemplates == m_smVisionInfo.g_intMaxMarkTemplate)
            //    //            m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex = 0;
            //    //        else if ((m_smVisionInfo.g_intSelectedTemplate < m_smVisionInfo.g_intTotalTemplates) && (m_smVisionInfo.g_intSelectedTemplate != -1))
            //    //            m_intSelectedIndex = m_smVisionInfo.g_intSelectedTemplate;
            //    //        else
            //    //            m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex = m_smVisionInfo.g_intTotalTemplates;
            //    //        break;
            //    //}
            //}

            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
            if (m_smVisionInfo.g_arrMarks.Count > 1)
                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intMinArea = 2;
            if (Convert.ToInt32(txt_MinArea.Text) <= 1)
            {
                intMinArea = 2;
                //txt_MinArea.Text = "2";
            }
            else
                intMinArea = Convert.ToInt32(txt_MinArea.Text);

            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intLearnMinArea = intMinArea;
            BuildMarkObjects();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void btn_MarkGrayValueSensitivitySetting_Click(object sender, EventArgs e)
        {
            GrayValueSensitivitySettingForm objForm = new GrayValueSensitivitySettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, 0);
            objForm.ShowDialog();

            BuildMarkObjects();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewObjectsBuilded = false;

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetThreshold();

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(673, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                    m_smVisionInfo.g_arrMarks[u].SetThreshold(m_smVisionInfo.g_intThresholdValue);
            }
            objThresholdForm.Dispose();

            BuildMarkObjects();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_DontCareAreaDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)// || m_smVisionInfo.g_arrPolygon_Mark.Count == 0)
                return;

            //if (m_smVisionInfo.g_intSelectedTemplate < m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedDontCareROIIndex].Count)
            //{
            //    m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedDontCareROIIndex][m_smVisionInfo.g_intSelectedTemplate].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
            //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //}
            if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
            {
                pnl_DontCareFreeShape.Visible = true;
            }
            else
            {
                pnl_DontCareFreeShape.Visible = false;
            }
        }

        private void chk_DeleteAllTemplates_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            Graphics g1;
            Graphics g2;
            Graphics g3;
            Graphics[] g_pic = new Graphics[8];


            if (chk_DeleteAllTemplates.Checked)
            {
                pic_Panel[0].Visible = true; // 1st template border is green
                for (int j = 1; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
                {
                    pic_Panel[j].Visible = true;
                    pic_Panel[j].BackColor = Color.Red;
                }
                if (Mark1.Image != null)
                {
                    bmp_Mark1 = new Bitmap(Mark1.Image);
                    g1 = Graphics.FromImage(bmp_Mark1);
                    Mark1.Image = bmp_Mark1;
                }
                if (Mark2.Image != null)
                {
                    bmp_Mark2 = new Bitmap(Mark2.Image);
                    g2 = Graphics.FromImage(bmp_Mark2);
                    Mark2.Image = bmp_Mark2;
                }
                if (Mark3.Image != null)
                {
                    bmp_Mark3 = new Bitmap(Mark3.Image);
                    g3 = Graphics.FromImage(bmp_Mark3);
                    Mark3.Image = bmp_Mark3;
                }

                for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
                {
                    if (pic_Template[i].Image != null)
                    {
                        bmp_pic[i] = new Bitmap(pic_Template[i].Image);
                        g_pic[i] = Graphics.FromImage(bmp_pic[i]);
                        pic_Template[i].Image = bmp_pic[i];
                    }
                }

                m_intTempForSelectedIndex = m_intSelectedIndex; // save index for load back when uncheck
                m_intSelectedIndex = 0;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
            }
            else
            {
                for (int j = 0; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
                {
                    pic_Panel[j].Visible = false;
                    pic_Panel[j].BackColor = Color.Lime;
                }

                if (Mark1.Image != null)
                {
                    Ori_Mark1 = (Bitmap)Bitmap.FromFile(Mark1.ImageLocation);
                    Mark1.Image = Ori_Mark1;
                }
                if (Mark2.Image != null)
                {
                    Ori_Mark2 = (Bitmap)Bitmap.FromFile(Mark2.ImageLocation);
                    Mark2.Image = Ori_Mark2;
                }
                if (Mark3.Image != null)
                {
                    Ori_Mark3 = (Bitmap)Bitmap.FromFile(Mark3.ImageLocation);
                    Mark3.Image = Ori_Mark3;
                }

                for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
                {
                    if (pic_Template[i].Image != null)
                    {
                        Ori_pic[i] = (Bitmap)Bitmap.FromFile(pic_Template[i].ImageLocation);
                        pic_Template[i].Image = Ori_pic[i];
                    }
                }
                m_intSelectedIndex = m_intTempForSelectedIndex;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                pic_Panel[m_intSelectedIndex].Visible = true;
            }

            UpdateTemplateGUI();
        }

        private void LoadMarkTemplate()
        {
            string strFolderName = "Mark";
            string strFileName = "OCRTemplate" + m_smVisionInfo.g_intSelectedGroup + "_";
            Mark0.BackColor = Color.Black;
            Mark1.BackColor = Color.Black;
            Mark2.BackColor = Color.Black;
            Mark3.BackColor = Color.Black;

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] +
            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\";

            Label[] lbl_No = new Label[8];
            lbl_No[0] = new Label();
            lbl_No[0].ForeColor = Color.LightGreen;
            lbl_No[0].BackColor = Color.Black;
            lbl_No[0].Font = new Font(lbl_No[0].Font.FontFamily, 10);
            lbl_No[0].Text = "1";
            lbl_No[0].Width = 15;
            lbl_No[0].Location = new Point(Mark0.Location.X + 1, Mark0.Location.Y);
            pnl_Template.Controls.Add(lbl_No[0]);
            lbl_No[0].BringToFront();

            for (int i = 0; i < 4; i++)
            {
                pic_Panel[i] = new Panel();
                pic_Panel[i].Size = new Size(Mark0.Bounds.Width + 2, Mark0.Bounds.Height + 4);
                pic_Panel[i].BackColor = Color.Lime;
            }

            for (int i = 1; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
            {
                lbl_No[i] = new Label();
                lbl_No[i].ForeColor = Color.Red;
                lbl_No[i].BackColor = Color.Black;
                lbl_No[i].Font = new Font(lbl_No[0].Font.FontFamily, 10);
                lbl_No[i].Text = (i + 1).ToString();
                lbl_No[i].Width = 15;
            }

            if (m_smVisionInfo.g_intMaxMarkTemplate > 4)
            {
                for (int j = 4; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
                {
                    pic_Template[j] = new PictureBox();
                    pic_Panel[j] = new Panel();
                    pic_Template[j].BackColor = Color.Black;
                    pic_Template[j].BorderStyle = BorderStyle.FixedSingle;
                    pic_Template[j].Name = "Mark" + j.ToString();
                    pic_Template[j].Size = new Size(Mark0.Bounds.Width, Mark0.Bounds.Height);
                    pic_Panel[j].Size = new Size(Mark0.Bounds.Width + 2, Mark0.Bounds.Height + 4);
                    pic_Panel[j].BackColor = Color.Lime;
                    if (File.Exists(strFolderPath + strFileName + j.ToString() + ".bmp"))
                    {
                        pic_Template[j].Load(strFolderPath + strFileName + j.ToString() + ".bmp");
                        pic_Template[j].SizeMode = PictureBoxSizeMode.Zoom;
                        lbl_No[j].ForeColor = Color.LightGreen;
                    }
                    switch (j)
                    {
                        case 4:
                            pic_Template[4].Location = new Point(Mark0.Location.X, Mark0.Location.Y + 83);
                            pnl_Template.Controls.Add(pic_Template[4]);
                            break;
                        case 5:
                            pic_Template[5].Location = new Point(Mark1.Location.X, Mark1.Location.Y + 83);
                            pnl_Template.Controls.Add(pic_Template[5]);
                            break;
                        case 6:
                            pic_Template[6].Location = new Point(Mark2.Location.X, Mark2.Location.Y + 83);
                            pnl_Template.Controls.Add(pic_Template[6]);
                            break;
                        case 7:
                            pic_Template[7].Location = new Point(Mark3.Location.X, Mark3.Location.Y + 83);
                            pnl_Template.Controls.Add(pic_Template[7]);
                            break;
                    }
                }

            }

            for (int i = 0; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
            {

                switch (i)
                {
                    case 0:
                        if (File.Exists(strFolderPath + strFileName + i.ToString() + ".bmp"))
                        {
                            //Mark0.BackColor = Color.FromArgb(210, 230, 255);
                            Mark0.Load(strFolderPath + strFileName + i.ToString() + ".bmp");
                            Mark0.SizeMode = PictureBoxSizeMode.Zoom;
                            lbl_No[i].ForeColor = Color.LightGreen;
                        }
                        break;
                    case 1:
                        if (File.Exists(strFolderPath + strFileName + i.ToString() + ".bmp"))
                        {
                            //Mark1.BackColor = Color.FromArgb(210, 230, 255);
                            Mark1.Load(strFolderPath + strFileName + i.ToString() + ".bmp");
                            Mark1.SizeMode = PictureBoxSizeMode.Zoom;
                            lbl_No[i].ForeColor = Color.LightGreen;
                        }
                        break;
                    case 2:
                        if (File.Exists(strFolderPath + strFileName + i.ToString() + ".bmp"))
                        {
                            //Mark2.BackColor = Color.FromArgb(210, 230, 255);
                            Mark2.Load(strFolderPath + strFileName + i.ToString() + ".bmp");
                            Mark2.SizeMode = PictureBoxSizeMode.Zoom;
                            lbl_No[i].ForeColor = Color.LightGreen;
                        }
                        break;
                    case 3:
                        if (File.Exists(strFolderPath + strFileName + i.ToString() + ".bmp"))
                        {
                            //Mark3.BackColor = Color.FromArgb(210, 230, 255);
                            Mark3.Load(strFolderPath + strFileName + i.ToString() + ".bmp");
                            Mark3.SizeMode = PictureBoxSizeMode.Zoom;
                            lbl_No[i].ForeColor = Color.LightGreen;
                        }
                        break;
                }
            }

            for (int i = 1; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
            {
                switch (i)
                {
                    case 1:
                        if (Mark1.Bounds.Height > lbl_No[i].Height)
                            lbl_No[i].Location = new Point(Mark1.Location.X + 1, Mark1.Location.Y);
                        break;
                    case 2:
                        if (Mark2.Bounds.Height > lbl_No[i].Height)
                            lbl_No[i].Location = new Point(Mark2.Location.X + 1, Mark2.Location.Y);
                        break;
                    case 3:
                        if (Mark3.Bounds.Height > lbl_No[i].Height)
                            lbl_No[i].Location = new Point(Mark3.Location.X + 1, Mark3.Location.Y);
                        break;
                    case 4:
                        if (pic_Template[4].Bounds.Height > lbl_No[i].Height)
                            lbl_No[i].Location = new Point(pic_Template[4].Location.X + 1, pic_Template[4].Location.Y);
                        break;
                    case 5:
                        if (pic_Template[5].Bounds.Height > lbl_No[i].Height)
                            lbl_No[i].Location = new Point(pic_Template[5].Location.X + 1, pic_Template[5].Location.Y);
                        break;
                    case 6:
                        if (pic_Template[6].Bounds.Height > lbl_No[i].Height)
                            lbl_No[i].Location = new Point(pic_Template[6].Location.X + 1, pic_Template[6].Location.Y);
                        break;
                    case 7:
                        if (pic_Template[7].Bounds.Height > lbl_No[i].Height)
                            lbl_No[i].Location = new Point(pic_Template[7].Location.X + 1, pic_Template[7].Location.Y);
                        break;
                }
                pnl_Template.Controls.Add(lbl_No[i]);
                lbl_No[i].BringToFront();
            }

            if (m_smVisionInfo.g_intMaxMarkTemplate > 4)
            {
                pnl_Template.Size = new Size(360, 170);
                chk_DeleteAllTemplates.Location = new Point(6, 460);
            }
            else
            {
                pnl_Template.Size = new Size(360, 86);
                chk_DeleteAllTemplates.Location = new Point(8, 448);
            }

            if (m_smVisionInfo.g_strVisionName != "Orient" && m_smVisionInfo.g_strVisionName != "BottomOrient")
                AutoSelectTemplate();
            else
            {
                pnl_Template.Visible = false;
                chk_DeleteAllTemplates.Visible = false;
                //srmGroupBox2.Location = new Point(6, 275);
            }
        }
        private void MarkTemplate_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox clickedPictureBox = sender as PictureBox;

            if (Mark3.Image == null)
                return;

            int temphold = m_intSelectedIndex;
            for (int j = 1; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                if (pic_Panel[j].BackColor == Color.Red)
                    return;
            }

            for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
            {
                switch (i)
                {
                    case 4:
                        pic_Panel[4].Location = new Point(Mark0.Location.X - 1, Mark0.Location.Y + 81);
                        break;
                    case 5:
                        pic_Panel[5].Location = new Point(Mark1.Location.X - 1, Mark1.Location.Y + 81);
                        break;
                    case 6:
                        pic_Panel[6].Location = new Point(Mark2.Location.X - 1, Mark2.Location.Y + 81);
                        break;
                    case 7:
                        pic_Panel[7].Location = new Point(Mark3.Location.X - 1, Mark3.Location.Y + 81);
                        break;
                }
            }

            for (int j = 0; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                pic_Panel[j].Visible = false;
                if (clickedPictureBox.Name.ToString().Contains(j.ToString()))
                    m_intSelectedIndex = j;
            }


            for (int i = 5; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
            {
                if (m_intSelectedIndex == 4)
                    break;
                else if (m_intSelectedIndex == i && pic_Template[i - 1].Image == null)
                {
                    m_intSelectedIndex = temphold;
                    pic_Panel[m_intSelectedIndex].Visible = true;
                    return;
                }
            }

            pic_Panel[m_intSelectedIndex].Visible = true;

        }
        private void Mark0_MouseClick(object sender, MouseEventArgs e)
        {
            for (int j = 1; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                if (pic_Panel[j].BackColor == Color.Red)
                    return;
            }

            m_intSelectedIndex = 0;
            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;
            if (m_smVisionInfo.g_arrMarks.Count > 1)
                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;

            for (int j = 0; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                pnl_Template.Controls.Add(pic_Panel[j]);
                pic_Panel[j].SendToBack();
                pic_Panel[j].Visible = false;
            }
            pic_Panel[0].Location = new Point(Mark0.Location.X - 1, Mark0.Location.Y - 2);

            pic_Panel[m_intSelectedIndex].Visible = true;

        }

        private void Mark1_MouseClick(object sender, MouseEventArgs e)
        {
            if (pic_Panel[1].BackColor == Color.Red)
                return;

            m_intSelectedIndex = 1;
            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 1;
            if (m_smVisionInfo.g_arrMarks.Count > 1)
                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 1;

            for (int j = 0; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                pnl_Template.Controls.Add(pic_Panel[j]);
                pic_Panel[j].SendToBack();
                pic_Panel[j].Visible = false;
            }

            pic_Panel[1].Location = new Point(Mark1.Location.X - 1, Mark1.Location.Y - 2);

            pic_Panel[m_intSelectedIndex].Visible = true;
        }

        private void Mark2_MouseClick(object sender, MouseEventArgs e)
        {
            if (pic_Panel[2].BackColor == Color.Red)
                return;

            if (Mark1.Image == null)
                return;

            m_intSelectedIndex = 2;
            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 2;
            if (m_smVisionInfo.g_arrMarks.Count > 1)
                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 2;

            for (int j = 0; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                pnl_Template.Controls.Add(pic_Panel[j]);
                pic_Panel[j].SendToBack();
                pic_Panel[j].Visible = false;
            }

            pic_Panel[2].Location = new Point(Mark2.Location.X - 1, Mark2.Location.Y - 2);

            pic_Panel[m_intSelectedIndex].Visible = true;
        }

        private void Mark3_MouseClick(object sender, MouseEventArgs e)
        {
            if (pic_Panel[3].BackColor == Color.Red)
                return;

            if (Mark2.Image == null)
                return;

            m_intSelectedIndex = 3;
            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 3;
            if (m_smVisionInfo.g_arrMarks.Count > 1)
                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 3;

            for (int j = 0; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                pnl_Template.Controls.Add(pic_Panel[j]);
                pic_Panel[j].SendToBack();
                pic_Panel[j].Visible = false;
            }

            pic_Panel[3].Location = new Point(Mark3.Location.X - 1, Mark3.Location.Y - 2);

            pic_Panel[m_intSelectedIndex].Visible = true;
        }
        private void DisposePictureBoxImage()
        {
            //2021-01-05 ZJYEOH : condition need check object is null or not first, if check IsDiposed first will cause error as object is null
            if (Mark0 != null && !Mark0.IsDisposed)
            {
                Mark0.Dispose();
                Mark0 = null;
            }

            if (Mark1 != null && !Mark1.IsDisposed)
            {
                Mark1.Dispose();
                Mark1 = null;
            }

            if (Mark2 != null && !Mark2.IsDisposed)
            {
                Mark2.Dispose();
                Mark2 = null;
            }

            if (Mark3 != null && !Mark3.IsDisposed)
            {
                Mark3.Dispose();
                Mark3 = null;
            }

            for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
            {
                if (pic_Template[i] != null && !pic_Template[i].IsDisposed)
                {
                    pic_Template[i].Dispose();
                    pic_Template[i] = null;
                }
            }

            if (Ori_Mark1 != null)
            {
                Ori_Mark1.Dispose();
                Ori_Mark1 = null;
                bmp_Mark1.Dispose();
                bmp_Mark1 = null;
            }
            if (Ori_Mark2 != null)
            {
                Ori_Mark2.Dispose();
                Ori_Mark2 = null;
                bmp_Mark2.Dispose();
                bmp_Mark2 = null;
            }
            if (Ori_Mark3 != null)
            {
                Ori_Mark3.Dispose();
                Ori_Mark3 = null;
                bmp_Mark3.Dispose();
                bmp_Mark3 = null;
            }

            for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
            {
                if (Ori_pic[i] != null)
                {
                    Ori_pic[i].Dispose();
                    Ori_pic[i] = null;
                    bmp_pic[i].Dispose();
                    bmp_pic[i] = null;
                }
            }
        }

        private void AutoSelectTemplate()
        {
            for (int i = 0; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
            {
                switch (i)
                {
                    case 4:
                        pic_Panel[4].Location = new Point(Mark0.Location.X - 1, Mark0.Location.Y + 81);
                        break;
                    case 5:
                        pic_Panel[5].Location = new Point(Mark1.Location.X - 1, Mark1.Location.Y + 81);
                        break;
                    case 6:
                        pic_Panel[6].Location = new Point(Mark2.Location.X - 1, Mark2.Location.Y + 81);
                        break;
                    case 7:
                        pic_Panel[7].Location = new Point(Mark3.Location.X - 1, Mark3.Location.Y + 81);
                        break;
                }
            }

            pic_Panel[0].Location = new Point(Mark0.Location.X - 1, Mark0.Location.Y - 2);
            pic_Panel[1].Location = new Point(Mark1.Location.X - 1, Mark1.Location.Y - 2);
            pic_Panel[2].Location = new Point(Mark2.Location.X - 1, Mark2.Location.Y - 2);
            pic_Panel[3].Location = new Point(Mark3.Location.X - 1, Mark3.Location.Y - 2);


            for (int j = 0; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                pnl_Template.Controls.Add(pic_Panel[j]);
                pic_Panel[j].SendToBack();
                pic_Panel[j].Visible = false;
            }

            if (Mark0.Image == null)
            {
                m_intSelectedIndex = 0;
                pic_Panel[0].Visible = true;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                return;
            }
            if (Mark0.Image != null && Mark1.Image != null && Mark2.Image != null && Mark3.Image != null && m_smVisionInfo.g_intMaxMarkTemplate <= 4)
            {
                m_intSelectedIndex = 0;
                pic_Panel[0].Visible = true;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                return;
            }
            else if (Mark1.Image == null && Mark2.Image == null && Mark3.Image == null)
            {
                m_intSelectedIndex = 0;
                pic_Panel[0].Visible = true;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                return;
            }
            else if (Mark1.Image != null && Mark0.Image != null && Mark2.Image == null)
            {
                m_intSelectedIndex = 1;
                pic_Panel[1].Visible = true;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                return;
            }
            else if (Mark1.Image != null && Mark2.Image != null && Mark3.Image == null)
            {
                m_intSelectedIndex = 2;
                pic_Panel[2].Visible = true;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                return;
            }
            else if (Mark2.Image != null && Mark3.Image != null && pic_Template[4].Image == null)
            {
                m_intSelectedIndex = 3;
                pic_Panel[3].Visible = true;
                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                return;
            }
            else
            {
                if (m_smVisionInfo.g_intMaxMarkTemplate <= 4)
                    return;

                for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
                {
                    pic_Panel[i].Visible = false;
                    if (i == 4)
                    {
                        if (pic_Template[5] == null)
                        {
                            m_intSelectedIndex = 0;
                            pic_Panel[m_intSelectedIndex].Visible = true;
                            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                            if (m_smVisionInfo.g_arrMarks.Count > 1)
                                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                            return;
                        }

                        if (pic_Template[5].Image == null && pic_Template[4].Image != null)
                        {
                            m_intSelectedIndex = 4;
                            pic_Panel[m_intSelectedIndex].Visible = true;
                            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                            if (m_smVisionInfo.g_arrMarks.Count > 1)
                                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                            return;
                        }
                        else if (pic_Template[5].Image != null && pic_Template[4].Image != null)
                        {
                            if (pic_Template[6] == null)
                                continue;
                            else if (pic_Template[6].Image == null)
                            {
                                m_intSelectedIndex = 5;
                                pic_Panel[m_intSelectedIndex].Visible = true;
                                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                                if (m_smVisionInfo.g_arrMarks.Count > 1)
                                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                                return;
                            }
                            else
                                continue;
                        }

                        if (Mark3.Image != null)
                        {
                            if (pic_Template[4].Image != null)
                            {
                                m_intSelectedIndex = i;
                                pic_Panel[m_intSelectedIndex].Visible = true;
                                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                                if (m_smVisionInfo.g_arrMarks.Count > 1)
                                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                                return;
                            }
                            else
                            {
                                m_intSelectedIndex = i - 1;
                                pic_Panel[m_intSelectedIndex].Visible = true;
                                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                                if (m_smVisionInfo.g_arrMarks.Count > 1)
                                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                                return;
                            }
                        }

                        else
                        {
                            m_intSelectedIndex = 2;
                            pic_Panel[2].Visible = true;
                            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                            if (m_smVisionInfo.g_arrMarks.Count > 1)
                                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                            return;
                        }
                    }
                    else
                    {
                        switch (m_smVisionInfo.g_intMaxMarkTemplate)
                        {
                            case 6:
                                if (pic_Template[4].Image != null && pic_Template[5].Image != null)
                                {
                                    m_intSelectedIndex = 0;
                                }
                                else if (pic_Template[i].Image != null && pic_Template[i - 1].Image != null)
                                {
                                    m_intSelectedIndex = i;
                                }
                                break;

                            case 7:
                                if (pic_Template[5].Image != null && pic_Template[6].Image != null)
                                {
                                    m_intSelectedIndex = 0;
                                }
                                else if (pic_Template[i].Image != null && pic_Template[i - 1].Image != null)
                                {
                                    m_intSelectedIndex = i;
                                }
                                break;

                            case 8:
                                if (pic_Template[7].Image != null && pic_Template[6].Image != null)
                                {
                                    m_intSelectedIndex = 0;
                                }
                                else if (pic_Template[i].Image != null && pic_Template[i - 1].Image != null)
                                {
                                    m_intSelectedIndex = i;
                                }
                                break;
                        }
                    }
                }
                pic_Panel[m_intSelectedIndex].Visible = true;
            }
            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
            if (m_smVisionInfo.g_arrMarks.Count > 1)
                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int j = 4; j < m_smVisionInfo.g_intMaxMarkTemplate; j++)
            {
                this.pic_Template[j].MouseClick += new MouseEventHandler(this.MarkTemplate_MouseClick);
            }
        }

        private void btn_OCRSettings_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnHasDetected = false;

            OCRSettingForm NewForm = new OCRSettingForm(m_smVisionInfo,m_smProductionInfo,m_strSelectedRecipe);

            if(NewForm.ShowDialog() == DialogResult.OK)
            {
                if (!m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRDetect(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMinWidth
                     , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMaxWidth
                     , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharHeight, m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1]
                     , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_strTopologyValue))
                {
                    SRMMessageBox.Show(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCR2GetMessage(1), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            DefineCurrentTopology();
          
            m_smVisionInfo.g_blnHasDetected = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radio_LearnManual_Click(object sender, EventArgs e)
        {
            btn_LoadDatabase.Visible = false;
            btn_LoadDatabase.Enabled = false;
            pnl_LearnManual.Visible = true;
        }

        private void radio_FontDetection_Click(object sender, EventArgs e)
        {
            pnl_LearnManual.Visible = false;
            btn_LoadDatabase.Visible = true;
            btn_LoadDatabase.Enabled = true;
        }
        private void timer_MarkOrient_Tick(object sender, EventArgs e)
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

            if (m_smVisionInfo.g_blnViewRotatedImage_AfterMouseUp)
            {
                m_smVisionInfo.g_blnViewRotatedImage_AfterMouseUp = false;
                RotatePrecise();
            }

            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if (m_smVisionInfo.g_arrMarkDontCareROIs.Count > 0)
                {
                    if (m_smVisionInfo.g_intSelectedUnit < m_smVisionInfo.g_arrPolygon_Mark.Count)
                    {
                        if (m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex)
                        {
                            cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
                        }
                    }

                    //if (m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedDontCareROIIndex].Count > m_smVisionInfo.g_intSelectedTemplate)
                    //{
                    //    cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedDontCareROIIndex][m_smVisionInfo.g_intSelectedTemplate].ref_intFormMode;
                    //}
                }
                else
                    cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

                if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
                {
                    pnl_DontCareFreeShape.Visible = true;
                }
                else
                {
                    pnl_DontCareFreeShape.Visible = false;
                }
            }
            if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_HitObject != null && !m_blnSetText)
            {
                Graphics g = pnl_pic.CreateGraphics();
                g.Clear(Color.White);
                float temp = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRGetLearnROILengthWidth(0);
                float temp2 = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRGetLearnROILengthWidth(1);

                if (temp == 0 || temp2 == 0)
                    return;

                float zoomX = (pnl_pic.Width - 6) / (float)temp;
                float zoomY = (pnl_pic.Height - 6) / (float)temp2;

                float temp3 = Math.Min(zoomX, zoomY);
                IntPtr hdc = g.GetHdc();
                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DrawOCRLearnROI(hdc, temp3);
                g.ReleaseHdc(hdc);
            }
        }

        private void btn_LoadDatabase_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                m_smVisionInfo.g_blnHasDetected = false;
                dlg.Filter = "OCR2database|*.o2d|TrueTypeFont|*.ttf";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    pnl_LearnManual.Visible = true;
                    if (dlg.FileName != null)
                    {
                        //m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRClearDatabase();
                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRLoad(dlg.FileName);

                        if (!m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRDetect(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMinWidth
                            , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMaxWidth
                            , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharHeight, m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1]
                            , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_strTopologyValue))
                        {
                            SRMMessageBox.Show(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCR2GetMessage(1), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        m_smVisionInfo.g_blnHasDetected = true;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
            }

        }

        private void btn_Recognise_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_blnHasDetected)
            {
                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRRegconise();
                m_smVisionInfo.g_blnHasRecognise = true;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRClearDatabase();
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].LoadTemplateOCR(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\");
            m_strDatabaseName = Directory.GetFiles(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", "*.o2?");

            if(m_strDatabaseName.Length != 0)
               m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRLoad(m_strDatabaseName);
        }

        private void LearnOCRForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnViewRotatedImage_AfterMouseUp = false;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = 0;
            m_smVisionInfo.g_blnViewMOGauge = false;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewOrientTrainROI = false;
            m_smVisionInfo.g_blnViewPin1TrainROI = false;
            m_smVisionInfo.g_blnViewMarkTrainROI = false;
            m_smVisionInfo.g_blnViewMark2DCodeROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewObjectsBuilded = false;
            m_smVisionInfo.g_blnViewCharsBuilded = false;
            m_smVisionInfo.g_blnViewTextsBuilded = false;
            m_smVisionInfo.g_blnMarkInspected = false;
            m_smVisionInfo.g_blnDrawMarkResult = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;
            m_smVisionInfo.g_blnDrawMark2DCodeResult = false;
            m_smVisionInfo.g_blnViewOrientObject = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.g_strContextMenuType = "Production";
            m_smVisionInfo.g_blnUpdateContextMenu = true;
            m_smVisionInfo.g_blnHasDetected = false;
            m_smVisionInfo.g_blnHasRecognise = false;
            m_smProductionInfo.g_blnViewROITool = m_blnViewROITool;

            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }
        
        private void txt_SetChar_MouseEnter(object sender, EventArgs e)
        {
            m_blnSetText = true;
        }

        private void txt_SetChar_MouseLeave(object sender, EventArgs e)
        {
            m_blnSetText = false;
        }

        private void btn_Learn_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRLearn(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_HitObject,txt_SetChar.Text);
        }

        private void pnl_pic_Paint(object sender, PaintEventArgs e)
        {
            if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_HitObject == null)
                return;

            e.Graphics.Clear(Color.White);
            float temp = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRGetLearnROILengthWidth(0);
            float temp2 = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRGetLearnROILengthWidth(1);

            if (temp == 0 || temp2 == 0)
                return;

            float zoomX = (pnl_pic.Width - 6) / (float)temp;
            float zoomY = (pnl_pic.Height - 6) / (float)temp2;
            
            float temp3 = Math.Min(zoomX, zoomY);
            IntPtr hdc = e.Graphics.GetHdc();
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DrawOCRLearnROI(hdc, temp3);
            e.Graphics.ReleaseHdc(hdc);
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            if (SRMMessageBox.Show("Are you sure you want to clear all database?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRClearDatabase();
                if (!m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRDetect(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMinWidth
                    , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharMaxWidth
                    , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intCharHeight, m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1]
                    , m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_strTopologyValue))
                {
                    SRMMessageBox.Show(m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCR2GetMessage(1), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                if (radio_LearnManual.Checked)
                    return;
                else
                    radio_FontDetection_Click(sender, e);          
            }
            else
                return;
        }

        public void CopyDirectory(string strSource, string strDestination)
        {
            if (!Directory.Exists(strDestination))
                Directory.CreateDirectory(strDestination);

            String[] strFiles = Directory.GetFileSystemEntries(strSource);
            foreach (string strElement in strFiles)
            {
                // Sub directories
                if (Directory.Exists(strElement))
                    CopyDirectory(strElement, strDestination + "\\" + Path.GetFileName(strElement));
                // Files in directory
                else
                    File.Copy(strElement, strDestination + "\\" + Path.GetFileName(strElement), true);
            }
        }

        private void SaveGeneralSetting(string strFolderPath)
        {
            //
            //STDeviceEdit.CopySettingFile(strFolderPath, "General.xml");

            XmlParser objFile = new XmlParser(strFolderPath + "General.xml");
            objFile.WriteSectionElement("TemplateCounting");

            objFile.WriteElement1Value("TotalUnits", m_smVisionInfo.g_intUnitsOnImage);

            m_smVisionInfo.g_intTotalGroup = 1; //Group count is always 1
            objFile.WriteElement1Value("TotalGroups", m_smVisionInfo.g_intTotalGroup);

            if (m_intSelectedIndex == m_smVisionInfo.g_intTotalTemplates)
                m_smVisionInfo.g_intTotalTemplates++;
            objFile.WriteElement1Value("TotalTemplates", m_smVisionInfo.g_intTotalTemplates);

            m_smVisionInfo.g_intTemplateMask |= (0x01 << m_intSelectedIndex);
            objFile.WriteElement1Value("TemplateMask", m_smVisionInfo.g_intTemplateMask);

            if ((m_smVisionInfo.g_intTemplatePriority & (0x0F << (0x04 * m_intSelectedIndex))) == 0)
                m_smVisionInfo.g_intTemplatePriority |= ((long)(m_intSelectedIndex + 1) << (0x04 * m_intSelectedIndex));
            objFile.WriteElement1Value("TemplatePriority", m_smVisionInfo.g_intTemplatePriority);

            objFile.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-General", strFolderPath, "General.xml");
        }

        private void SaveTemplateImage(string strFolderPath, ROI objROI)
        {
            // Save Template Image to template folder
            m_smVisionInfo.g_arrImages[0].SaveImage(strFolderPath + "OCROriTemplate0" + "_" + m_intSelectedIndex + ".bmp");
            if (m_smVisionInfo.g_arrImages.Count > 1)
                m_smVisionInfo.g_arrImages[1].SaveImage(strFolderPath + "OCROriTemplate0" + "_" + m_intSelectedIndex + "_Image1.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 2)
                m_smVisionInfo.g_arrImages[2].SaveImage(strFolderPath + "OCROriTemplate0" + "_" + m_intSelectedIndex + "_Image2.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 3)
                m_smVisionInfo.g_arrImages[3].SaveImage(strFolderPath + "OCROriTemplate0" + "_" + m_intSelectedIndex + "_Image3.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 4)
                m_smVisionInfo.g_arrImages[4].SaveImage(strFolderPath + "OCROriTemplate0" + "_" + m_intSelectedIndex + "_Image4.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 5)
                m_smVisionInfo.g_arrImages[5].SaveImage(strFolderPath + "OCROriTemplate0" + "_" + m_intSelectedIndex + "_Image5.bmp");
            if (m_smVisionInfo.g_arrImages.Count > 6)
                m_smVisionInfo.g_arrImages[6].SaveImage(strFolderPath + "OCROriTemplate0" + "_" + m_intSelectedIndex + "_Image6.bmp");
            objROI.SaveImage(strFolderPath + "OCRTemplate0_" + m_intSelectedIndex + ".bmp");
        }


        private void SaveMarkSettings(string strFolderPath)
        {
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Mark\\";


            if (File.Exists(strFolderPath + "Template\\OriTemplate0" + "_" + m_intSelectedIndex + ".bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template " + (m_intSelectedIndex + 1).ToString(), m_smVisionInfo.g_strVisionFolderName + ">Learn Mark", "OriTemplate0" + "_" + m_intSelectedIndex + ".bmp", "OriTemplate0" + "_" + m_intSelectedIndex + ".bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template " + (m_intSelectedIndex + 1).ToString(), m_smVisionInfo.g_strVisionFolderName + ">Learn Mark", "", "OriTemplate0" + "_" + m_intSelectedIndex + ".bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            // Delete Existing Char Image before save to prevent previous char mix with new char image
            if (Directory.Exists(strFolderPath + "Template\\"))
            {
                string[] strImageFiles = Directory.GetFiles(strFolderPath + "Template\\", "*_" + m_intSelectedIndex.ToString() + "_Char*.bmp");

                m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "Old\\");

                foreach (string ImagePath in strImageFiles)
                    File.Delete(ImagePath);
            }
            // Save Mark Settings
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SaveTemplateOCR(strFolderPath + "Template\\",true);
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRSaveDatabase(strFolderPath + "Template\\");
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].LoadTemplateOCR(strFolderPath + "Template\\");

            // Save ROI Settings
            SaveROISettings(strFolderPath, m_smVisionInfo.g_arrMarkROIs, "Mark");
            SaveDontCareROISettings(strFolderPath, m_smVisionInfo.g_arrMarkDontCareROIs, "Mark");
            // Save learn template Image
            SaveTemplateImage(strFolderPath + "Template\\", m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "New\\");

            // Save Dont Care ROI and Dont Care Image
            if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                SaveDontCareSetting(strFolderPath + "Template\\");
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString());

            // ------------ Copy Temporary File ---------------------------------------

            string strFolderPathTemp = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "_Temp\\";
            CopyDirectory(m_strFolderPath, strFolderPathTemp);
            // -----------------------------------------------------------------------------

            //if (chk_DeleteAllTemplates.Checked)
            //{
            //    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DeleteAllPreviousTemplate(strFolderPathTemp + "Mark\\");
            //    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SaveSingleTemplate(strFolderPathTemp + "Mark\\Template\\", false); // 2020-05-11 ZJYEOH: Save first template setting 
            //    STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_e");
            //}

             SaveMarkSettings(strFolderPathTemp + "Mark\\");
             SaveGeneralSetting(strFolderPathTemp);

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                   m_smProductionInfo.g_blnSaveRecipeToServer = true;

           m_smVisionInfo.g_blnUpdateSECSGEMFile = true;
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DisposeLearnBlob();
                   

            // Rename folder

            // Delete Vision#_Backup if exist.
            string strFolderBackup = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "_Backup\\";
            if (Directory.Exists(strFolderBackup))
                Directory.Delete(strFolderBackup, true);

            while (true)
            {
                try
                {
                    DisposePictureBoxImage();
                    // check if Vision# able to rename or not. 
                    //If can rename, mean can proceed to overwrite with new learn information.
                    //If cannot rename, mean the original Vision# is locked. Need to display message to ask want continue or cancel. 
                    //                  If user want to continue, need to make sure the original Vision# is not locked by closing outside application, or folder or file.
                    Directory.Move(m_strFolderPath, strFolderBackup);
                    Directory.Move(strFolderPathTemp, m_strFolderPath);
                    Directory.Delete(strFolderBackup, true);
                    break;
                }
                catch (Exception ex)
                {
                    if (SRMMessageBox.Show("Fail to save!. Please make sure recipe file is not used by other.", "", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                    {
                        Directory.Delete(strFolderPathTemp, true);
                        return;
                    }
                }
            }
            this.Close();
            this.Dispose();

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_p");

        }

        private void txt_SetChar_TextChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].OCRsetText(txt_SetChar.Text);
        }

        private void pnl_Topology_Paint(object sender, PaintEventArgs e)
        {
            int intGapWidth = 8;//8
            int intGapHeight = 8;//8
            int intCharWidth = 35;//45
            int intCharHeight = 55;//80
            int intWordHeight = intCharHeight + intGapHeight * 2;
            int intLineHeight = intWordHeight + intGapHeight * 2;
            int intLineStringHeight = 15;//18
            int intTotalHeightForLine = 110;//140
            int intLineStartY = 20;//20
            int intWordStartY = intLineStartY + intGapHeight;
            int intCharStartY = intWordStartY + intGapHeight;
            Font objFont = new Font("Verdana", 7, FontStyle.Bold);
            e.Graphics.Clear(Color.Black);
            for (int i = 0; i < CharCount.Count; i++)
            {
                if (i != 0)
                {
                    intLineStartY += intTotalHeightForLine;
                    intWordStartY += intTotalHeightForLine;
                    intCharStartY += intTotalHeightForLine;
                }
                int intLineStartX = 5;//5
                int intWordStartX = intLineStartX + intGapWidth;
                int intCharStartX = intWordStartX + intGapWidth;

                for (int j = 0; j < CharCount[i].Count; j++)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Red, 2), new Rectangle(intWordStartX, intWordStartY, (CharCount[i][j] * intCharWidth) + intGapWidth + intGapWidth, intWordHeight));
                    intWordStartX += (CharCount[i][j] * intCharWidth) + intGapWidth + intGapWidth;

                    for (int k = 0; k < CharCount[i][j]; k++)
                    {
                        e.Graphics.DrawRectangle(new Pen(Color.LightGreen, 2), new Rectangle(intCharStartX, intCharStartY, intCharWidth, intCharHeight));
                        e.Graphics.DrawString(arrCharType[i][j][k], objFont, new SolidBrush(Color.LightGreen), intCharStartX + intCharWidth / 2 - (objFont.Size * arrCharType[i][j][k].Length) / 2, intCharStartY + intCharHeight / 2 - objFont.Height / 2);
                        intCharStartX += intCharWidth;
                    }
                    if (j == CharCount[i].Count - 1)
                    {
                        e.Graphics.DrawString("Line " + (i + 1), objFont, new SolidBrush(Color.Red), intLineStartX, intLineStartY - intLineStringHeight);
                        e.Graphics.DrawRectangle(new Pen(Color.Aquamarine, 2), new Rectangle(intLineStartX, intLineStartY, intWordStartX, intLineHeight));

                        if (intLineStartX + intWordStartX > pnl_Topology.Size.Width)
                            pnl_Topology.Size = new Size(330 + (intLineStartX + intWordStartX + intGapWidth + intGapWidth - pnl_Topology.Size.Width) + intGapWidth, pnl_Topology.Size.Height);
                    }
                    else if (CharCount[i].Count > 1)
                    {
                        intWordStartX += intGapWidth;
                        intCharStartX = intWordStartX + intGapWidth;
                    }
                }
            }

            if (CharCount.Count * (intTotalHeightForLine) + 20 > pnl_Topology.Size.Height)
                pnl_Topology.Size = new Size(pnl_Topology.Size.Width, CharCount.Count * (intTotalHeightForLine) + 20);
            
        }

        private void DefineCurrentTopology()
        {
            CharCount = new List<List<int>>();
            arrCharType = new List<List<List<string>>>();
            string[] arrLine = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_strTopologyValue.Split('\n');
            for (int i = 0; i < arrLine.Length; i++)
            {
                CharCount.Add(new List<int>());
                arrCharType.Add(new List<List<string>>());
                string[] arrWord = arrLine[i].Split(' ');
                for (int j = 0; j < arrWord.Length; j++)
                {
                    arrCharType[i].Add(new List<string>());
                    char[] arrChar = arrWord[j].ToCharArray();
                    for (int k = 0; k < arrChar.Length; k++)
                    {
                        if (arrChar[k] != '{' && arrChar[k] != '}' && arrChar[k] != '[' && arrChar[k] != ']')
                        {
                            if (arrChar[k] == 'L' && arrChar.Length > (k + 1) && (arrChar[k + 1] == 'u' || arrChar[k + 1] == 'l'))
                            {
                                arrCharType[i][j].Add((arrChar[k].ToString() + arrChar[k + 1].ToString()).ToString());
                                k++;
                            }
                            else
                                arrCharType[i][j].Add(arrChar[k].ToString());
                        }
                        else if (arrChar[k] == '{')
                        {
                            for (int x = 1; x < Convert.ToInt32(arrChar[k + 1].ToString()); x++)
                            {
                                arrCharType[i][j].Add(arrChar[k - 1].ToString());
                            }
                            k++;
                        }
                        else if (arrChar[k] == '[')
                        {
                            string strCombined = "";
                            for (int x = k + 1; x < arrChar.Length; x++)
                            {
                                if (arrChar[x] != ']')
                                    strCombined += arrChar[x].ToString();
                                else
                                {
                                    k++;
                                    break;
                                }
                                k++;
                            }
                            arrCharType[i][j].Add(strCombined);
                        }
                    }
                    CharCount[i].Add(arrCharType[i][j].Count);
                }
            }
            pnl_Topology.Size = new Size(330, 160);
        }
    }
}

