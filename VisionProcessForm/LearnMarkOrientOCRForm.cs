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
    public partial class LearnMarkOrientOCRForm : Form
    {
        #region Member Variables
        private bool m_blnRotated = false;
        private int m_intLearnType = 0; // 0 = Learn All, 1 = Learn Search ROI, 2 = Learn Orientation ROI, 3 = Gauge ROI, 4 = Learn Mark ROI, 5 = Mark Dont Care ROI, 6 = Char ROI, 7 = Pin 1 ROI, 8 = Unit PR ROI
        private int m_intSelectedDontCareROIIndexPrev = 0;
        private bool m_blnViewROITool;
        private bool m_blnFirstTimeInit = false;
        // Orient Global Settings
        private bool m_blnInitDone = false;
        private bool m_blnInitGaugeAngle = false;
        private bool m_blnWantGauge = true;
        private bool m_blnWantOCRMark = false;
        private bool m_blnWantOCVMark = false;
        private bool m_blnWantOrient = false;
        private bool m_blnWantPackage = false;
        private bool m_blnWantBottom = false;
        private bool m_blnWant2DCode = false;
        private bool m_blnVisible_chk_DeleteAllTemplate = true;
        private bool m_blnVisible_gb_RotateOrientation = true;
        private bool m_blnVisible_txt_MinArea = true;
        private bool m_blnVisible_btn_Threshold = true;
        private bool m_blnVisible_btn_MarkGrayValueSensitivitySetting = true;
        private float m_fGaugeAngle = 0.0f;
        private int m_intBuildCharsStage = 0;
        private int m_intDisplayStepNo = 1;
        private int m_intOrientDirection;
        private int m_intPatternSelectedIndex = -1;
        private int m_intUserGroup = 5;
        private int m_intCurrentAngle = 0;
        private int m_intCurrentPreciseDeg = 0;
        private string m_strFolderPath;
        private string m_strSelectedRecipe;
        private int m_intSelectedIndex = 0;
        private PictureBox[] pic_Template = new PictureBox[8];
        private Panel[] pic_Panel = new Panel[8];

        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private RectGauge m_objGauge;
        private UserRight m_objUserRight = new UserRight();
        private VisionInfo m_smVisionInfo;

        private int m_intTempForSelectedIndex;
        private Bitmap bmp_Mark1; // image for draw line
        private Bitmap bmp_Mark2;
        private Bitmap bmp_Mark3;
        private Bitmap[] bmp_pic = new Bitmap[8];
        private Bitmap Ori_Mark1; //if uncheck load back ori image
        private Bitmap Ori_Mark2;
        private Bitmap Ori_Mark3;
        private Bitmap[] Ori_pic = new Bitmap[8];
        //DeviceEdit m_objDeviceEdit;

        private AdvancedRectGaugeM4LForm m_objAdvancedRectGaugeForm = null;
        #endregion


        public LearnMarkOrientOCRForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, bool blnWantGauge, int intLearnType)
        {
            InitializeComponent();
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;

            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;
            m_blnWantGauge = blnWantGauge;
            m_intLearnType = intLearnType;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            //m_strFolderPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWantOCVMark = true;
            }
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWantOrient = true;
            }
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWantPackage = true;
            }
            if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWantBottom = true;
            }

            if ((m_smCustomizeInfo.g_intWant2DCode & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnWant2DCode = true;
            }

            if (m_smVisionInfo.g_arrOrientGaugeM4L.Count > 0)
                txt_ImageGain.Value = Convert.ToDecimal(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
            DisableField2();
            LoadMarkTemplate();

            UpdateTemplateGUI();

            if (m_smVisionInfo.g_intMarkDefectInspectionMethod == 1)
            {
                btn_MarkGrayValueSensitivitySetting.Visible = m_blnVisible_btn_MarkGrayValueSensitivitySetting;
                btn_Threshold.Visible = false;
            }
            else
            {
                btn_MarkGrayValueSensitivitySetting.Visible = false;
                btn_Threshold.Visible = m_blnVisible_btn_Threshold;
            }

            txt_MinArea.Text = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intLearnMinArea.ToString();

            switch (m_intLearnType)
            {
                case 0:
                    m_smVisionInfo.g_intLearnStepNo = 0;
                    break;
                case 1:
                    m_smVisionInfo.g_intLearnStepNo = 0;
                    break;
                case 2:
                    m_smVisionInfo.g_intLearnStepNo = 3;
                    break;
                case 3:
                    m_smVisionInfo.g_intLearnStepNo = 12;
                    break;
                case 4:
                    m_smVisionInfo.g_intLearnStepNo = 6;
                    break;
                case 5:
                    m_smVisionInfo.g_intLearnStepNo = 14;
                    break;
                case 6:
                    m_smVisionInfo.g_intLearnStepNo = 8;//6
                    break;
                case 7:
                    m_smVisionInfo.g_intLearnStepNo = 5;
                    break;
                case 8:
                    m_smVisionInfo.g_intLearnStepNo = 1;
                    break;
                case 9:
                    m_smVisionInfo.g_intLearnStepNo = 7;
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
                btn_Save.Enabled = false; btn_Save.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_ROISaveClose.Enabled = false; btn_ROISaveClose.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Rotate Button";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                gb_RotateOrientation.Enabled = false; gb_RotateOrientation.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                //gb_RotatePrecise.Enabled = false; gb_RotatePrecise.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                m_blnVisible_gb_RotateOrientation = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Template Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                pnl_Template.Enabled = false; pnl_Template.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_DeleteAllTemplates.Enabled = false; chk_DeleteAllTemplates.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                m_blnVisible_chk_DeleteAllTemplate = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Gauge Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_GaugeAdvanceSetting.Enabled = false; btn_GaugeAdvanceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_ShowDraggingBox.Enabled = false; chk_ShowDraggingBox.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_ShowSamplePoints.Enabled = false; chk_ShowSamplePoints.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Mark Min Area";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                txt_MinArea.Enabled = false;
                txt_MinArea.Visible = srmLabel4.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                m_blnVisible_txt_MinArea = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Mark Threhsold";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_Threshold.Enabled = false;
                btn_Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                m_blnVisible_btn_Threshold = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                // 2021 05 10 - CCENG: Hide group box if all controls inside the group box are hidden also.
                gb_BuildObjects.Visible = m_blnVisible_btn_Threshold || m_blnVisible_txt_MinArea;

            }

            strChild3 = "Mark Gray Value Sensitivity Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                btn_MarkGrayValueSensitivitySetting.Enabled = false;
                btn_MarkGrayValueSensitivitySetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                m_blnVisible_btn_MarkGrayValueSensitivitySetting = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Mark Edit Tools";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                m_smVisionInfo.g_blnEnableMarkContextMenu = false;
            }

            strChild3 = "Dont Care Area Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild1, strChild2, strChild3))
            {
                cbo_DontCareAreaDrawMethod.Enabled = false; cbo_DontCareAreaDrawMethod.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_Undo.Enabled = false; btn_Undo.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_AddDontCareROI.Enabled = false; btn_AddDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_DeleteDontCareROI.Enabled = false; btn_DeleteDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            // ----------------------------- 

            if (m_smVisionInfo.g_intUnitsOnImage == 1)
            {
                lbl_LearnUnit.Visible = false;
                cbo_SelectLearnUnit.Visible = false;
            }

            if (m_smVisionInfo.g_blnWantMultiTemplates)
            {
                //cbo_TemplateNo.Visible = true;
                if (m_smVisionInfo.g_intMaxMarkTemplate == 1)
                {
                    chk_DeleteAllTemplates.Visible = m_blnVisible_chk_DeleteAllTemplate;// false; // 2021-08-03 ZJYEOH : one template need to display also, for default mark score assign purpose
                    //chk_DeleteAllTemplates.Checked = true;
                }
                else
                    chk_DeleteAllTemplates.Visible = m_blnVisible_chk_DeleteAllTemplate;

                srmLabel1.Visible = true;
                srmGroupBox6.Visible = true;
            }
            else
            {
                //cbo_TemplateNo.Items.Clear();
                //cbo_TemplateNo.Items.Add("Template 1");
                m_intSelectedIndex = 0;
                //cbo_TemplateNo.Visible = false;
                chk_DeleteAllTemplates.Visible = false;
                srmLabel1.Visible = false;
                srmGroupBox6.Visible = false;

                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
            }

            pictureBox1.Visible = false;
            pictureBox4.Visible = false;
            if (m_blnWantBottom)   // If bottom unit 
            {
                pictureBox1.Visible = false;
                pictureBox4.Visible = false;
                pictureBox6.Visible = false;
                pictureBox10.Visible = false;
                pictureBox11.Visible = false;
                pictureBox2.Image = ils_BottomOrient.Images[4];
                pictureBox8.Image = ils_BottomOrient.Images[6];
                pictureBox13.Image = ils_BottomOrient.Images[5];
            }

            if (m_smVisionInfo.g_blnWantGauge)
            {
                // Hide manual rotate tool if Gauge is ON. Unit angle will be rotated automatically using gauge measurement angle result. 
                srmLabel3.Visible = false;
                txt_RotateAngle.Visible = false;
                btn_RotateAngle.Visible = false;
            }
            else
            {
                srmLabel3.Visible = true;
                txt_RotateAngle.Visible = true;
                btn_RotateAngle.Visible = true;
            }


            if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                gb_RotateOrientation.Visible = false;   // Not allow to rotate image image 90 deg since want orient 0 deg only

            }
            else if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
            {
                gb_RotateOrientation.Visible = false;   // Not allow to rotate image image 90 deg because InPocket accept 1 direction only.
            }
            else
                gb_RotateOrientation.Visible = m_blnVisible_gb_RotateOrientation;

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
                case "BottomPosition":
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
        /// <summary>
        /// Add gauge to ROI
        /// </summary>
        /// <param name="objSearchROI">Gauge size and location will based on this ROI</param>
        /// <param name="arrGauge">Keep new gauge in this array list</param>
        private void AddGauge(ROI objSearchROI, List<RectGauge> arrGauge)
        {
            XmlParser objFile = new XmlParser(m_strFolderPath + "\\Gauge.xml");
            objFile.GetFirstSection("RectG");

            //create new gauge and attach to selected ROI
            m_objGauge = new RectGauge(m_smVisionInfo.g_WorldShape);

            //attach searcher parent
            m_objGauge.SetRectGaugePlacement(objSearchROI, objFile.GetValueAsFloat("Tolerance", 25),
                objFile.GetValueAsInt("SizeTolerance", 10));

            //set gauge measurement
            m_objGauge.ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
            m_objGauge.ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 0);

            //set gauge setting
            m_objGauge.SetRectGaugeSetting(objFile.GetValueAsInt("Thickness", 13), objFile.GetValueAsInt("Filter", 1),
                objFile.GetValueAsInt("Threshold", 2), objFile.GetValueAsInt("MinAmp", 10), objFile.GetValueAsInt("MinArea", 0));

            //set gauge fitting sampling step
            m_objGauge.SetRectGaugeFitting(objFile.GetValueAsInt("SamplingStep", 5));

            arrGauge.Add(m_objGauge);
        }

        private void AddGaugeM4L(ROI objSearchROI, List<RectGaugeM4L> arrGauge)
        {
            XmlParser objFile = new XmlParser(m_strFolderPath + "Orient\\GaugeM4L.xml");
            objFile.GetFirstSection("RectG");

            //create new gauge and attach to selected ROI
            RectGaugeM4L objGauge = new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex);

            //attach searcher parent
            objGauge.SetGaugePlace_BasedOnEdgeROI();

            //set gauge measurement
            objGauge.SetGaugeTransType(objFile.GetValueAsInt("TransType", 0));
            objGauge.SetGaugeTransChoice(objFile.GetValueAsInt("TransChoice", 0));

            //set gauge setting
            objGauge.SetGaugeThickness(objFile.GetValueAsInt("Thickness", 13));
            objGauge.SetGaugeFilter(objFile.GetValueAsInt("Filter", 1));
            objGauge.SetGaugeThreshold(objFile.GetValueAsInt("Threshold", 2));
            objGauge.SetGaugeMinAmplitude(objFile.GetValueAsInt("MinAmp", 10));
            objGauge.SetGaugeMinArea(objFile.GetValueAsInt("MinArea", 0));

            //set gauge fitting sampling step
            objGauge.SetGaugeSamplingStep(objFile.GetValueAsInt("SamplingStep", 5));

            arrGauge.Add(objGauge);
        }

        /// <summary>
        /// Add search roi and gauge
        /// </summary>
        /// <param name="intUnitNo">ROI unit index</param>
        /// <param name="arrROIs">Keep search roi in this array list</param>
        /// <param name="arrGauge">Keep new gauge in this array list</param>
        private void AddSearchROI(int intUnitNo, List<List<ROI>> arrROIs, List<RectGauge> arrGauge)
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
                AddGauge(objROI, arrGauge);
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
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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
                if (arrGauge.Count == intUnitNo)
                    AddGaugeM4L(objROI, arrGauge);
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
            if (arrGauge.Count == 0)
                AddGaugeM4L(objROI, arrGauge);
            if (arrGauge.Count == intUnitNo)
                AddGaugeM4L(objROI, arrGauge);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void AddSubROI(int intUnitNo, List<List<ROI>> arrROIs)
        {
            // Sub roi start with index 2. Index 0 for search roi and index 1 for train roi

            // Make sure at least have 2 roi(search roi + train roi) in array
            if (arrROIs[intUnitNo].Count < 2)
                return;

            // Make sure sub roi no more than 2
            if (arrROIs[intUnitNo].Count >= 3)
            {
                //2021-03-23 ZJYEOH : Need to attach again and assign Type to 4, so that orient sub ROI will be saved
                arrROIs[intUnitNo][2].AttachImage(arrROIs[intUnitNo][1]);
                arrROIs[intUnitNo][2].ref_intType = 4;
                return;
            }
            // 2019 06 13 - JBTAN: subROI will be named as Orient ROI
            //int intSubROIIndex = arrROIs[intUnitNo].Count - 1;
            //ROI objROI = new ROI("SubROI" + intSubROIIndex, 4);
            ROI objROI = new ROI("Orient ROI", 4);
            objROI.LoadROISetting(arrROIs[intUnitNo][1].ref_ROIWidth / 4,
                    arrROIs[intUnitNo][1].ref_ROIHeight / 4,
                    arrROIs[intUnitNo][1].ref_ROIWidth / 2,
                    arrROIs[intUnitNo][1].ref_ROIHeight / 2);

            objROI.AttachImage(arrROIs[intUnitNo][1]);

            arrROIs[intUnitNo].Add(objROI);

        }
        /// <summary>
        /// Add train roi
        /// </summary>
        /// <param name="intUnitNo">ROI unit index</param>
        /// <param name="arrROIs">Keep train roi in this array list</param>
        /// <param name="strROIName">Train roi name</param>
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
        /// <summary>
        /// Build blobs on mark
        /// </summary>
        /// <returns></returns>
        private bool BuildMarkObjects()
        {
            //STTrackLog.WriteLine("BuildMarkObjects - 1");
            m_smVisionInfo.g_blnViewCharsBuilded = false;
            m_smVisionInfo.g_blnViewTextsBuilded = false;
            pic_BuildCharsState.Image = ils_ImageListTree.Images[0];
            pic_BuildTextsState.Image = ils_ImageListTree.Images[0];

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
        /// <summary>
        /// Copy ROI and Gauge information from Orient to Mark
        /// </summary>
        private void CopyInfoToMark()
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (m_smVisionInfo.g_arrMarkROIs.Count <= i)
                    m_smVisionInfo.g_arrMarkROIs.Add(new List<ROI>());

                for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[i].Count; j++)
                {
                    if (m_smVisionInfo.g_arrMarkROIs[i].Count <= j)
                        m_smVisionInfo.g_arrMarkROIs[i].Add(new ROI());

                    if (j != 1) // Mark ROI
                    {
                        // Copy ROI position 
                        ROI objMarkROI = m_smVisionInfo.g_arrMarkROIs[i][j];
                        m_smVisionInfo.g_arrOrientROIs[i][j].CopyTo(ref objMarkROI);
                    }

                    if (j == 0)
                    {
                        m_smVisionInfo.g_arrMarkROIs[i][j].AttachROITopParrent(m_smVisionInfo.g_arrOrientROIs[i][j]);
                    }
                }

                //RectGauge objGauge = (RectGauge)m_smVisionInfo.g_arrOrientGauge[i];
                //// Copy Gauge
                //if (m_smVisionInfo.g_arrMarkGauge.Count <= i)
                //    m_smVisionInfo.g_arrMarkGauge.Add(objGauge);
                //else
                //    m_smVisionInfo.g_arrMarkGauge[i] = objGauge;

                if (m_smVisionInfo.g_blnWantGauge)
                {
                    RectGaugeM4L objGauge4L = (RectGaugeM4L)m_smVisionInfo.g_arrOrientGaugeM4L[i];
                    // Copy Gauge
                    if (m_smVisionInfo.g_arrMarkGaugeM4L.Count <= i)
                        m_smVisionInfo.g_arrMarkGaugeM4L.Add(objGauge4L);
                    else
                        m_smVisionInfo.g_arrMarkGaugeM4L[i] = objGauge4L;
                }
            }
        }

        private void CopyOrientUnitROIToPin1SearchROI()
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (m_smVisionInfo.g_arrPin1.Count <= i)
                    m_smVisionInfo.g_arrPin1.Add(new Pin1());

                if (m_smVisionInfo.g_arrPin1[i].ref_objSearchROI == null)
                    m_smVisionInfo.g_arrPin1[i].ref_objSearchROI = new ROI();

                ROI objSearchROI = m_smVisionInfo.g_arrPin1[i].ref_objSearchROI;
                m_smVisionInfo.g_arrOrientROIs[i][0].CopyTo(ref objSearchROI);

                m_smVisionInfo.g_arrPin1[i].ref_objSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrMarks[0].ref_intPin1ImageNo]);
            }
        }

        private void DeleteAllPreviousOrientTemplate(string strFolderPath)
        {
            // Delete OCV objects
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrOrients[u].Clear();
            }

            // Reset variables
            m_smVisionInfo.g_intTotalGroup = 0;
            m_smVisionInfo.g_intTotalTemplates = 0;
            m_smVisionInfo.g_intTemplateMask = 0;
            m_smVisionInfo.g_intTemplatePriority = 0;

            // Delete database
            try
            {
                if (Directory.Exists(strFolderPath + "Template"))
                    Directory.Delete(strFolderPath + "Template", true);
            }
            catch
            {
            }

            Directory.CreateDirectory(strFolderPath + "Template");
        }
        private void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "";

            //strChild2 = "Advance Setting";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    group_BuildTexts.Visible = false;
            //    lbl_Text1.Visible = false;
            //    lbl_Text2.Visible = false;
            //    lbl_Text3.Visible = false;
            //    lbl_Text4.Visible = false;
            //}

            //strChild2 = "Learn Template";  // 2018 10 22 - CCENG: Hide the Learn button instead of disable the next button.
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    btn_ClockWise.Enabled = false;
            //    btn_CounterClockWise.Enabled = false;
            //    btn_Previous.Enabled = false;
            //    btn_Next.Enabled = false;
            //}
            strChild1 = "Learn Page";
            strChild2 = "Save Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;

            }
            if (m_smVisionInfo.g_intUnitsOnImage == 1)
            {
                lbl_LearnUnit.Visible = false;
                cbo_SelectLearnUnit.Visible = false;
            }

            if (m_smVisionInfo.g_blnWantMultiTemplates)
            {
                //cbo_TemplateNo.Visible = true;
                chk_DeleteAllTemplates.Visible = true;
                srmLabel1.Visible = true;
            }
            else
            {
                //cbo_TemplateNo.Items.Clear();
                //cbo_TemplateNo.Items.Add("Template 1");
                m_intSelectedIndex = 0;
                //cbo_TemplateNo.Visible = false;
                chk_DeleteAllTemplates.Visible = false;
                srmLabel1.Visible = false;

                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
                if (m_smVisionInfo.g_arrMarks.Count > 1)
                    m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
            }

            pictureBox1.Visible = false;
            pictureBox4.Visible = false;
            if (m_blnWantBottom)   // If bottom unit 
            {
                pictureBox1.Visible = false;
                pictureBox4.Visible = false;
                pictureBox6.Visible = false;
                pictureBox10.Visible = false;
                pictureBox11.Visible = false;
                pictureBox2.Image = ils_BottomOrient.Images[4];
                pictureBox8.Image = ils_BottomOrient.Images[6];
                pictureBox13.Image = ils_BottomOrient.Images[5];
            }

            if (m_smVisionInfo.g_blnWantGauge)
            {
                // Hide manual rotate tool if Gauge is ON. Unit angle will be rotated automatically using gauge measurement angle result. 
                srmLabel3.Visible = false;
                txt_RotateAngle.Visible = false;
                btn_RotateAngle.Visible = false;
            }
            else
            {
                srmLabel3.Visible = true;
                txt_RotateAngle.Visible = true;
                btn_RotateAngle.Visible = true;
            }


            if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                gb_RotateOrientation.Visible = false;   // Not allow to rotate image image 90 deg since want orient 0 deg only

            }
            else if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
            {
                gb_RotateOrientation.Visible = false;   // Not allow to rotate image image 90 deg because InPocket accept 1 direction only.
            }
            else
                gb_RotateOrientation.Visible = true;
        }

        private void DisplayOCRPatternOnGrid()
        {
            dgd_OCRPatternList.Rows.Clear();
            int intNumPatterns = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetNumPatterns();
            int intClass;
            for (int i = 0; i < intNumPatterns; i++)
            {
                dgd_OCRPatternList.Rows.Add();
                dgd_OCRPatternList.Rows[i].Cells[0].Value = Convert.ToString(i + 1);
                dgd_OCRPatternList.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetPattern(i);
                intClass = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetPatternClass(i);
                switch (intClass)
                {
                    case 0:
                        dgd_OCRPatternList.Rows[i].Cells[2].Value = "Digit";
                        break;
                    case 1:
                        dgd_OCRPatternList.Rows[i].Cells[2].Value = "UpperCase";
                        break;
                    case 2:
                        dgd_OCRPatternList.Rows[i].Cells[2].Value = "LowerCase";
                        break;
                    case 3:
                        dgd_OCRPatternList.Rows[i].Cells[2].Value = "Special";
                        break;
                    case 4:
                        dgd_OCRPatternList.Rows[i].Cells[2].Value = "Extended";
                        break;
                }
            }

            if (dgd_OCRPatternList.Rows.Count > 0)
                dgd_OCRPatternList.Rows[0].Selected = false;
        }

        private void LoadGaugeSetting(string strPath, List<RectGauge> arrGauge, List<List<ROI>> arrROIs)
        {
            arrGauge.Clear();

            XmlParser objFile = new XmlParser(strPath);
            RectGauge objRectGauge;
            int intCount = objFile.GetFirstSectionCount();

            for (int j = 0; j < intCount; j++)
            {
                //create new ROI base on file read out
                objRectGauge = new RectGauge(m_smVisionInfo.g_WorldShape);
                objRectGauge.LoadGauge(strPath, "RectG" + j);

                //add Rect Gauge into shared memory's Rect Gauge array list 
                arrGauge.Add(objRectGauge);
            }

            objRectGauge = null;
        }

        private void LoadGauge4LSetting(string strPath, List<RectGaugeM4L> arrGauge, List<List<ROI>> arrROIs)
        {
            arrGauge.Clear();

            XmlParser objFile = new XmlParser(strPath);
            RectGaugeM4L RectGaugeM4L;
            int intCount = objFile.GetFirstSectionCount();

            for (int j = 0; j < 1; j++) //intCount
            {
                //create new ROI base on file read out
                RectGaugeM4L = new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex);
                RectGaugeM4L.LoadRectGauge4L(strPath, "RectG" + j);

                //add Rect Gauge into shared memory's Rect Gauge array list 
                arrGauge.Add(RectGaugeM4L);
            }

            RectGaugeM4L = null;
        }

        private void LoadGaugeM4LSetting(string strPath, List<RectGaugeM4L> arrGauge)
        {
            // 2019 09 03 - Proper way to dispose unwanted object.
            for (int i = m_smVisionInfo.g_intUnitsOnImage; i < arrGauge.Count; i++)
            {
                arrGauge[i].Dispose();
                arrGauge.RemoveAt(i);
            }

            XmlParser objFile = new XmlParser(strPath);
            RectGaugeM4L objRectGauge;
            //int intCount = Math.Min(m_smVisionInfo.g_intUnitsOnImage, objFile.GetFirstSectionCount());

            for (int j = 0; j < m_smVisionInfo.g_intUnitsOnImage; j++)
            {
                //create new ROI base on file read out
                if (j >= arrGauge.Count)
                {
                    //add Rect Gauge into shared memory's Rect Gauge array list 
                    objRectGauge = new RectGaugeM4L(m_smVisionInfo.g_WorldShape, 0, m_smVisionInfo.g_intVisionIndex);
                    arrGauge.Add(objRectGauge);
                }

                arrGauge[j].LoadRectGauge4L(strPath, "RectG" + j);
            }

            objRectGauge = null;

        }

        private void LoadMarkMaskBlob(string strPath, int intTotalTemplates, List<Package> arrPackage)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                Package objPackage = (Package)arrPackage[u];
                ROI objMarkSearchROI = m_smVisionInfo.g_arrMarkROIs[u][0];
                ROI objMarkTrainROI = m_smVisionInfo.g_arrMarkROIs[u][1];
                int intXDistance = objMarkTrainROI.ref_ROIPositionX + objMarkSearchROI.ref_ROIPositionX;
                int intYDistance = objMarkTrainROI.ref_ROIPositionY + objMarkSearchROI.ref_ROIPositionY;
                objPackage.ClearMarkMaskBlob();

                for (int i = 0; i < intTotalTemplates; i++)
                {
                    XmlParser objFile = new XmlParser(strPath + "Template" + i + ".xml");
                    int intLearnCount = objFile.GetFirstSectionCount();

                    for (int j = 0; j < intLearnCount; j++)
                    {
                        objFile.GetFirstSection("Learn" + j);
                        int intBlobsCount = objFile.GetValueAsInt("CharsCount", 0, 1);
                        for (int k = 0; k < intBlobsCount; k++)
                        {
                            objFile.GetSecondSection("BlobsNo" + k);
                            objPackage.SetMarkMaskBlob(i, j, objFile.GetValueAsFloat("CenterX", 0, 2), objFile.GetValueAsFloat("CenterY", 0, 2),
                                objFile.GetValueAsFloat("Width", 0, 2), objFile.GetValueAsFloat("Height", 0, 2), intXDistance, intYDistance);
                        }
                    }
                }
            }
        }

        private void LoadMarkSettings(string strFolderPath)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                m_smVisionInfo.g_arrMarks[u].LoadTemplateOCR(strFolderPath);
            }

            if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() == 0)
                m_smVisionInfo.VM_AT_TemplateNotLearn = true;
            else
                m_smVisionInfo.VM_AT_TemplateNotLearn = false;
        }

        private void LoadOrientGlobalSettings()
        {
            XmlParser objFileHandle = new XmlParser(m_strFolderPath + "\\Orient\\Settings.xml");
            objFileHandle.GetFirstSection("Advanced");
            m_intOrientDirection = objFileHandle.GetValueAsInt("Direction", 4);
        }

        private void LoadROISetting(string strPath, List<List<ROI>> arrROIList)
        {
            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            ROI objROI;

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                arrROIList.Add(new List<ROI>());
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

                    arrROIList[i].Add(objROI);
                }
            }
        }
        private void LoadROISetting(string strPath, List<ROI> arrROIList)
        {
            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            ROI objROI;
                objFile.GetFirstSection("Unit0");
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

                arrROIList.Add(objROI);
            }
            
        }
        private void ReadMarkSetting()
        {
            LoadROISetting(m_strFolderPath + "Mark\\ROI.xml", m_smVisionInfo.g_arrMarkROIs);
            LoadROISetting(m_strFolderPath + "Mark\\DontCareROI.xml", m_smVisionInfo.g_arrMarkDontCareROIs);
            //LoadGaugeSetting(m_strFolderPath + "Mark\\Gauge.xml", m_smVisionInfo.g_arrMarkGauge, m_smVisionInfo.g_arrMarkROIs);
            //LoadGauge4LSetting(m_strFolderPath + "Mark\\Gauge4L.xml", m_smVisionInfo.g_arrMarkGaugeM4L, m_smVisionInfo.g_arrMarkROIs);
            LoadGaugeM4LSetting(m_strFolderPath + "Mark\\GaugeM4L.xml", m_smVisionInfo.g_arrMarkGaugeM4L);
            for (int i = 0; i < m_smVisionInfo.g_arrMarkROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrMarkROIs[i].Count; j++)
                {
                    if (m_smVisionInfo.g_arrMarkROIs[i][j].ref_intType == 1)
                        m_smVisionInfo.g_arrMarkROIs[i][j].AttachImage(m_smVisionInfo.g_arrImages[0]);
                    else if (m_smVisionInfo.g_arrMarkROIs[i][j].ref_intType == 2)
                        m_smVisionInfo.g_arrMarkROIs[i][j].AttachImage(m_smVisionInfo.g_arrMarkROIs[i][0]);
                    else if (m_smVisionInfo.g_arrMarkROIs[i][j].ref_intType == 4)
                        m_smVisionInfo.g_arrMarkROIs[i][j].AttachImage(m_smVisionInfo.g_arrMarkROIs[i][1]);
                }

            for (int i = 0; i < m_smVisionInfo.g_arrMarkDontCareROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrMarkROIs[0][1] != null)
                    m_smVisionInfo.g_arrMarkDontCareROIs[i].AttachImage(m_smVisionInfo.g_arrMarkROIs[0][1]);
                   
                }

            if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
            {
                LoadMarkSettings(m_strFolderPath + "Mark\\Template\\");
            }

            // 2019 07 10 
            m_smVisionInfo.g_intSelectedTemplate = m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex;
        }

        private void ReadOrientSetting()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
              m_smVisionInfo.g_strVisionFolderName + "\\Orient\\";

            LoadROISetting(m_strFolderPath + "Orient\\ROI.xml", m_smVisionInfo.g_arrOrientROIs);
            //LoadGaugeSetting(m_strFolderPath + "Orient\\Gauge.xml", m_smVisionInfo.g_arrOrientGauge, m_smVisionInfo.g_arrOrientROIs);
            //LoadGauge4LSetting(m_strFolderPath + "Orient\\GaugeM4L.xml", m_smVisionInfo.g_arrOrientGaugeM4L, m_smVisionInfo.g_arrOrientROIs);
            LoadGaugeM4LSetting(m_strFolderPath + "Orient\\GaugeM4L.xml", m_smVisionInfo.g_arrOrientGaugeM4L);

            for (int i = 0; i < m_smVisionInfo.g_arrOrients.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                {
                    m_smVisionInfo.g_arrOrients[i][j].LoadOrient(m_strFolderPath + "Orient\\Settings.xml", "General");
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrOrientROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[i].Count; j++)
                {
                    if (m_smVisionInfo.g_arrOrientROIs[i][j].ref_intType == 1)
                        m_smVisionInfo.g_arrOrientROIs[i][j].AttachImage(m_smVisionInfo.g_arrImages[0]);
                    else if (m_smVisionInfo.g_arrOrientROIs[i][j].ref_intType == 2)
                        m_smVisionInfo.g_arrOrientROIs[i][j].AttachImage(m_smVisionInfo.g_arrOrientROIs[i][0]);
                    else if (m_smVisionInfo.g_arrOrientROIs[i][j].ref_intType == 4)
                        m_smVisionInfo.g_arrOrientROIs[i][j].AttachImage(m_smVisionInfo.g_arrOrientROIs[i][1]);
                }
        }

        private void ResetGeneralSetting(string strFolderPath)
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

        private void Rotate0Degree(int intImageIndex, List<List<ROI>> arrROIs, List<RectGauge> arrGauge)
        {
            if (!m_blnWantGauge || m_blnInitGaugeAngle || (m_blnWantBottom))
                return;

            // Rotate for both unit 1 and 2 (i represent unit index)
            for (int u = 0; u < arrROIs.Count; u++)
            {
                // Make sure search ROI is attached to main ori image before measure by gauge. 
                // ROI should not attach to rotated image which will give not actual unit phyical angle.
                //arrROIs[u][0].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                arrROIs[u][0].AttachImage(m_smVisionInfo.g_objPackageImage);

                // Get gauge angle by measuring search roi
                float fGaugeAngle = ((RectGauge)arrGauge[u]).Measure(arrROIs[u][0], 10);

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

            m_blnInitGaugeAngle = true;  // Only allow the image to be rotate once only
        }

        private void Rotate0Degree(int intImageIndex, List<List<ROI>> arrROIs, List<RectGaugeM4L> arrGauge)
        {
            if (!m_blnWantGauge || m_blnInitGaugeAngle || (m_blnWantBottom))
                return;

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

            m_blnInitGaugeAngle = true;  // Only allow the image to be rotate once only
        }

        private void SaveGaugeSettings(string strFolderPath, List<RectGauge> arrGauge, List<List<ROI>> arrROIs)
        {
            // Get off set between Mark ROI start point and gauge center point
            RectGauge objRectGauge = arrGauge[m_smVisionInfo.g_intSelectedUnit];
            ROI objROI = arrROIs[m_smVisionInfo.g_intSelectedUnit][0];
            objRectGauge.Measure(objROI, 3);    // Need to measure rotateImage to get correct gauge center point
            float fOffSetX = objRectGauge.ref_ObjectCenterX - objROI.ref_ROIPositionX;
            float fOffSetY = objRectGauge.ref_ObjectCenterY - objROI.ref_ROIPositionY;

            //
            //STDeviceEdit.CopySettingFile(strFolderPath, "Gauge.xml");

            // Save Gauge Setting to Gauge.xml file
            XmlParser objFile = new XmlParser(strFolderPath + "Gauge.xml");
            for (int j = 0; j < arrGauge.Count; j++)
            {
                objRectGauge = (RectGauge)arrGauge[j];

                objFile.WriteSectionElement("RectG" + j);

                objFile.WriteElement1Value("ObjectCenterX", (arrROIs[j][0]).ref_ROIPositionX + fOffSetX);
                objFile.WriteElement1Value("ObjectCenterY", (arrROIs[j][0]).ref_ROIPositionY + fOffSetY);

                objFile.WriteElement1Value("CenterX", objRectGauge.ref_GaugeCenterX);
                objFile.WriteElement1Value("CenterY", objRectGauge.ref_GaugeCenterY);
                objFile.WriteElement1Value("Width", objRectGauge.ref_GaugeWidth);
                objFile.WriteElement1Value("Height", objRectGauge.ref_GaugeHeight);
                objFile.WriteElement1Value("Threshold", objRectGauge.ref_GaugeThreshold);
                objFile.WriteElement1Value("Thickness", objRectGauge.ref_GaugeThickness);

                objRectGauge.SetRectGaugeTemplate((arrROIs[j][0]).ref_ROIPositionX + fOffSetX, (arrROIs[j][0]).ref_ROIPositionY + fOffSetY);
            }
            objFile.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Gauge", strFolderPath, "Gauge.xml");
        }
        private void SaveGaugeM4LSettings(string strFolderPath, List<RectGaugeM4L> arrGauge, List<List<ROI>> arrROIs)
        {
            if (m_smVisionInfo.g_intSelectedUnit >= arrGauge.Count)
                return;

            // Get off set between Mark ROI start point and gauge center point
            RectGaugeM4L objRectGauge = arrGauge[m_smVisionInfo.g_intSelectedUnit];
            ROI objROI = arrROIs[m_smVisionInfo.g_intSelectedUnit][0];
            objRectGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrRotatedImages, objROI, m_smVisionInfo.g_objWhiteImage);    // Need to measure rotateImage to get correct gauge center point
            float fOffSetX = objRectGauge.ref_pRectCenterPoint.X - objROI.ref_ROIPositionX;
            float fOffSetY = objRectGauge.ref_pRectCenterPoint.Y - objROI.ref_ROIPositionY;

            // Save Gauge Setting to Gauge.xml file
            XmlParser objFile = new XmlParser(strFolderPath + "Gauge.xml");
            for (int j = 0; j < arrGauge.Count; j++)
            {
                objRectGauge = (RectGaugeM4L)arrGauge[j];

                objRectGauge.SaveRectGauge4L(strFolderPath + "GaugeM4L.xml", false, "RectG" + j, true, false);
                //objFile.WriteSectionElement("RectG" + j);

                //objFile.WriteElement1Value("ObjectCenterX", (arrROIs[j][0]).ref_ROIPositionX + fOffSetX);
                //objFile.WriteElement1Value("ObjectCenterY", (arrROIs[j][0]).ref_ROIPositionY + fOffSetY);

                //objFile.WriteElement1Value("CenterX", objRectGauge.ref_GaugeCenterX);
                //objFile.WriteElement1Value("CenterY", objRectGauge.ref_GaugeCenterY);
                //objFile.WriteElement1Value("Width", objRectGauge.ref_GaugeWidth);
                //objFile.WriteElement1Value("Height", objRectGauge.ref_GaugeHeight);
                //objFile.WriteElement1Value("Threshold", objRectGauge.ref_GaugeThreshold);
                //objFile.WriteElement1Value("Thickness", objRectGauge.ref_GaugeThickness);

                objRectGauge.SetRectGaugeTemplate((arrROIs[j][0]).ref_ROIPositionX + fOffSetX, (arrROIs[j][0]).ref_ROIPositionY + fOffSetY);
            }
            objFile.WriteEndElement();
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

            //2020-05-11 ZJYEOH : Load Previous saved setting first, for new lot clear all template
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].LoadFirstTemplateSettingOnly(strFolderPath + "Template\\", chk_DeleteAllTemplates.Checked);
            
            // Save Mark Settings
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SaveTemplate(strFolderPath + "Template\\", true);

            // Save ROI Settings
            SaveROISettings(strFolderPath, m_smVisionInfo.g_arrMarkROIs, "Mark");
            SaveDontCareROISettings(strFolderPath, m_smVisionInfo.g_arrMarkDontCareROIs, "Mark");
            // Save Gauge Settings
            //SaveGaugeSettings(strFolderPath, m_smVisionInfo.g_arrMarkGauge, m_smVisionInfo.g_arrMarkROIs);
            SaveGaugeM4LSettings(strFolderPath, m_smVisionInfo.g_arrMarkGaugeM4L, m_smVisionInfo.g_arrMarkROIs);

            // Save learn template Image
            SaveTemplateImage(strFolderPath + "Template\\", m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1]);
            m_objCopy.CopyAllImageFiles(strFolderPath + "Template\\", strPath + "New\\");

            // Save Dont Care ROI and Dont Care Image
            if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                SaveDontCareSetting(strFolderPath + "Template\\");

            //2020 05 08 ZJYEOH : Overwrite template 1 settings to other templates
            if (m_smVisionInfo.g_blnWantSet1ToAll)// && m_intSelectedIndex > 0)
            {
                // Mark
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    m_smVisionInfo.g_arrMarks[u].SetTemplate1SetingToOtherTemplate(strFolderPath + "\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_intSelectedIndex, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                    m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath + "\\Template\\", false);
                    m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath + "\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                    m_smVisionInfo.g_arrMarks[u].LoadTemplateOCR(strFolderPath + "\\Template\\");
                    ////m_smVisionInfo.g_arrMarks[u].SetTemplate1SetingToOtherTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_intSelectedIndex);
                    ////m_smVisionInfo.g_arrMarks[u].SaveTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", false);
                    ////m_smVisionInfo.g_arrMarks[u].LoadTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", m_smVisionInfo.g_arrMarkROIs);
                }
            }
            else
            {
                if (Mark0.Image == null && m_intSelectedIndex == 0 || Mark1.Image == null && m_intSelectedIndex == 1 || Mark2.Image == null && m_intSelectedIndex == 2 || Mark3.Image == null && m_intSelectedIndex == 3)
                {
                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                    {
                        m_smVisionInfo.g_arrMarks[u].SetPreviousTemplateSetingToOtherTemplate(strFolderPath + "\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_intSelectedIndex, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                        m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath + "\\Template\\", false);
                        m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath + "\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                        m_smVisionInfo.g_arrMarks[u].LoadTemplateOCR(strFolderPath + "\\Template\\");

                        //m_smVisionInfo.g_arrMarks[u].SetPreviousTemplateSetingToOtherTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_intSelectedIndex);
                        //m_smVisionInfo.g_arrMarks[u].SaveTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", false);
                        //m_smVisionInfo.g_arrMarks[u].LoadTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", m_smVisionInfo.g_arrMarkROIs);
                    }

                    for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
                    {
                        if (pic_Template[i].Image == null && m_intSelectedIndex == i)
                        {
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                m_smVisionInfo.g_arrMarks[u].SetPreviousTemplateSetingToOtherTemplate(strFolderPath + "\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_intSelectedIndex, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                                m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath + "\\Template\\", false);
                                m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath + "\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                                m_smVisionInfo.g_arrMarks[u].LoadTemplateOCR(strFolderPath + "\\Template\\");
                            }
                        }
                    }
                }
                else
                {
                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                    {
                        m_smVisionInfo.g_arrMarks[u].SetCurrentTemplateSetingForExtraCharLearnt(strFolderPath + "\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                        m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath + "\\Template\\", false);
                        m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath + "\\Template\\", m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
                        m_smVisionInfo.g_arrMarks[u].LoadTemplateOCR(strFolderPath + "\\Template\\");

                        //m_smVisionInfo.g_arrMarks[u].SetCurrentTemplateSetingForExtraCharLearnt(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", m_smVisionInfo.g_arrMarkROIs);
                        //m_smVisionInfo.g_arrMarks[u].SaveTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", false);
                        //m_smVisionInfo.g_arrMarks[u].LoadTemplate(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\", m_smVisionInfo.g_arrMarkROIs);
                    }
                }
            }

        }

        private void SaveOrientSettings(string strFolderPath, string strFeatureFolderName)
        {
            int x = 0;

            if (m_smVisionInfo.g_strVisionName.Contains("BottomPosition") && (m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].LoadROISetting(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionX,
                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionY, m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth,
                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);
            }

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1a");
            // Keep previous orient setting 
            bool blnUsePreviousSetting = true;// chk_PreviousSetting.Checked; //2020-05-11 ZJYEOH : Need to use previous setting to keep the setting in new template
            // Get setting from previous OCV object
            Orient[] objOrient = new Orient[m_smVisionInfo.g_intUnitsOnImage];
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_smVisionInfo.g_arrOrients[u].Count == 0 || !blnUsePreviousSetting)
                    objOrient[u] = new Orient(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                else
                {
                    objOrient[u] = new Orient(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                    if (m_smVisionInfo.g_arrOrients[u].Count <= m_intSelectedIndex)
                    {
                        objOrient[u].ref_fMinScore = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex - 1].ref_fMinScore;
                        objOrient[u].ref_intDirections = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex - 1].ref_intDirections;
                    }
                    else
                    {
                        objOrient[u].ref_fMinScore = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex].ref_fMinScore;
                        objOrient[u].ref_intDirections = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex].ref_intDirections;
                    }
                }
            }

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1b");
            // Delete previous template
            if (chk_DeleteAllTemplates.Checked)
            {
                DeleteAllPreviousOrientTemplate(strFolderPath + strFeatureFolderName);
            }

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1c");

            if (m_smVisionInfo.g_arrOrients[0].Count == 0)
                m_smVisionInfo.g_arrOrients[x].Add(objOrient[x]); //2020-05-11 ZJYEOH : If orient object is empty meaning delete all template, load back previous first template to it
            //m_smVisionInfo.g_arrOrients[0].Add(new Orient(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1d");
            if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 2)
            {
                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1e");

                int intCenterX2 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROICenterX;
                int intCenterY2 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROICenterY;
                int intCenterX1 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth / 2; //ref_ROICenterX // 2020-04-09 ZJYEOH : should use half of the width as sub orient ROI is attach on unit ROI
                int intCenterY1 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight / 2; //ref_ROICenterY // 2020-04-09 ZJYEOH : should use half of the height as sub orient ROI is attach on unit ROI
                m_smVisionInfo.g_arrOrients[0][0].ref_intMatcherOffSetCenterX = intCenterX2 - intCenterX1;
                m_smVisionInfo.g_arrOrients[0][0].ref_intMatcherOffSetCenterY = intCenterY2 - intCenterY1;

                if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 3)
                {
                    int intCenterX3 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][3].ref_ROICenterX;
                    int intCenterY3 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][3].ref_ROICenterY;
                    m_smVisionInfo.g_arrOrients[0][0].ref_intUnitSurfaceOffsetX = intCenterX3 - intCenterX2;
                    m_smVisionInfo.g_arrOrients[0][0].ref_intUnitSurfaceOffsetY = intCenterY3 - intCenterY2;
                }

                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1f");
            }

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1g");

            //
            //STDeviceEdit.CopySettingFile(strFolderPath, "Settings.xml");
            //m_smVisionInfo.g_arrOrients[0][0].SaveOrient(strFolderPath + strFeatureFolderName + "Settings.xml", false, "General", true); //2021-01-18 ZJYEOH : Save Orient after save pattern because some setting not yet updated
            // STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient", strFolderPath, "Settings.xml");
            // Must save pattern first before saving ROI settings because when out of page, ROI attach the original image again

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1h");
            #region Save Pattern

            for (x = 0; x < m_smVisionInfo.g_intUnitsOnImage; x++)
            {
                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1i");

                // if orient object count lower than or equal to selected index, mean need add new orient
                // Add new orient
                if (m_smVisionInfo.g_arrOrients[x].Count <= m_intSelectedIndex)
                {
                    m_smVisionInfo.g_arrOrients[x].Add(objOrient[x]);
                }

                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1j");
                // if selected index == last object(index -1)
                // add default value
                if (m_intSelectedIndex == (m_smVisionInfo.g_arrOrients[x].Count - 1))
                {
                    if (!blnUsePreviousSetting)
                    {
                        m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].ref_intDirections = m_intOrientDirection;
                        m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].ref_fMinScore = (float)m_smCustomizeInfo.g_intMarkDefaultTolerance / 100;
                    }
                }

                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1k");
                // if (selected index < last object(index -1)
                // delete last object
                if (m_intSelectedIndex < (m_smVisionInfo.g_arrOrients[x].Count - 1))
                {
                    m_smVisionInfo.g_arrOrients[x].RemoveAt(m_smVisionInfo.g_arrOrients[0].Count - 1);
                }

                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1L");

                if (!Directory.Exists(strFolderPath + strFeatureFolderName + "Template\\"))
                {
                    Directory.CreateDirectory(strFolderPath + strFeatureFolderName + "Template\\");
                }

                if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 2)
                {
                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].LearnUnitPRPattern(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2]);
                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SetUnitPRFinalReduction(2);
                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SetUnitPRAngleSetting(0, 0);// (-10, 10);
                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SaveUnitPRPattern(strFolderPath + strFeatureFolderName + "Template\\Template" + m_intSelectedIndex + "_UnitPR.mch");
                }

                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1m");

                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].LearnPattern(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SetFinalReduction(2);
                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SavePattern(strFolderPath + strFeatureFolderName + "Template\\Template" + m_intSelectedIndex + ".mch");

                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1n");

                // Learn sub pattern match
                if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 2)
                {
                    int intLastSubROIIndex = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count - 1;    // Sub ROI is always in last array
                    bool IndexFound = false;
                    if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastSubROIIndex].ref_intType == 4)
                        IndexFound = true;
                    //if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastSubROIIndex].ref_strROIName.IndexOf("SubROI") > 0)  // Check is the last ROI a Sub ROI?
                    if (IndexFound)
                    {
                        m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].LearnSubPattern(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastSubROIIndex]);
                        m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SetFinalReduction(2);
                        m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SaveSubPattern(strFolderPath + strFeatureFolderName + "Template\\SubTemplate" + m_intSelectedIndex + ".mch");
                    }
                }

                //2020-10-06 ZJYEOH : Need to rotate unit to original position when save pattern
                float CenterX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;// (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;
                float CenterY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2;

                float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX - CenterX) * Math.Cos((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)) -//m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_intRotatedAngle
                                   ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY - CenterY) * Math.Sin((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)));
                float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX - CenterX) * Math.Sin((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)) +
                                    ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY - CenterY) * Math.Cos((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)));

                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1o");
                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].LearnPattern4Direction_SRM(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1], fXAfterRotated, fYAfterRotated);
                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1p");
                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SavePattern4Direction(strFolderPath + strFeatureFolderName + "Template\\", "Template" + m_intSelectedIndex.ToString());
                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1q");
            }
            
            //2021-01-18 ZJYEOH : Save Orient here in order to get updated setting
            m_smVisionInfo.g_arrOrients[0][0].SaveOrient(strFolderPath + strFeatureFolderName + "Settings.xml", false, "General", true);

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1r");

            //STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
            XmlParser objFile = new XmlParser(strFolderPath + strFeatureFolderName + "Template\\Template.xml");
            objFile.WriteSectionElement("Template" + m_intSelectedIndex);
            objFile.WriteElement1Value("TemplateCenterX", m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX + m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterX);
            objFile.WriteElement1Value("TemplateCenterY", m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY + m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterY);
            objFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[0][0].ref_fMinScore);
            objFile.WriteElement1Value("SubMatcherCount", m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count - 2);
            objFile.WriteEndElement();
            //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient Template", strFolderPath, "Template\\Template.xml");

            //2020 05 08 ZJYEOH : Overwrite template 1 settings to other templates
            if (m_smVisionInfo.g_blnWantSet1ToAll)// && m_intSelectedIndex > 0)
            {
                //Orient 
                if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrOrients[u].Count; j++)
                        {
                            XmlParser objOrientFile = new XmlParser(strFolderPath + strFeatureFolderName + "\\Template\\Template.xml");
                            //XmlParser objOrientFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Template\\Template.xml");
                            objOrientFile.WriteSectionElement("Template" + j);
                            objOrientFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[u][0].ref_fMinScore);
                            objOrientFile.WriteEndElement();

                            m_smVisionInfo.g_arrOrients[u][j].ref_fMinScore = objOrientFile.GetValueAsFloat("MinScore", 0.7f);

                        }
                    }
                }
            }
            else
            {
                //if (cbo_TemplateNo.SelectedItem.ToString() == "New...")
                if (Mark0.Image == null && m_intSelectedIndex == 0 || Mark1.Image == null && m_intSelectedIndex == 1 || Mark2.Image == null && m_intSelectedIndex == 2 || Mark3.Image == null && m_intSelectedIndex == 3)
                {
                    //Orient 
                    if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrOrients[u].Count; j++)
                            {
                                XmlParser objOrientFile = new XmlParser(strFolderPath + strFeatureFolderName + "\\Template\\Template.xml");
                                //XmlParser objOrientFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Template\\Template.xml");
                                objOrientFile.WriteSectionElement("Template" + j);
                                if (m_intSelectedIndex > 0 && m_intSelectedIndex == j)
                                    objOrientFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[u][j - 1].ref_fMinScore); // Load Previous template setting 
                                else
                                    objOrientFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[u][j].ref_fMinScore);
                                objOrientFile.WriteEndElement();

                                m_smVisionInfo.g_arrOrients[u][j].ref_fMinScore = objOrientFile.GetValueAsFloat("MinScore", 0.7f);

                            }
                        }
                    }
                }
            }
            #endregion

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1s");

            #region Save Template Image
            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            string strPath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Orient\\";
            

            if (File.Exists(strFolderPath + strFeatureFolderName + "Template\\OriTemplate0" + "_" + m_intSelectedIndex + ".bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template " + (m_intSelectedIndex + 1).ToString(), m_smVisionInfo.g_strVisionFolderName + ">Learn Orient", "OriTemplate0" + "_" + m_intSelectedIndex + ".bmp", "OriTemplate0" + "_" + m_intSelectedIndex + ".bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template " + (m_intSelectedIndex + 1).ToString(), m_smVisionInfo.g_strVisionFolderName + ">Learn Orient", "", "OriTemplate0" + "_" + m_intSelectedIndex + ".bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(strFolderPath + strFeatureFolderName + "Template\\", strPath + "Old\\");

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1t");

            // Delete all previous template image
            string[] arrCurrentTemplateImages = Directory.GetFiles(strFolderPath + strFeatureFolderName + "Template\\", "OriTemplate*.bmp");

            foreach(string strFileName in arrCurrentTemplateImages)
            {
                try
                {
                    if (File.Exists(strFileName))
                        File.Delete(strFileName);
                }
                catch
                {

                }
            }

            // Save Template Image to template folder
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_smVisionInfo.g_arrColorImages[0].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + m_intSelectedIndex + ".bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 1)
                    m_smVisionInfo.g_arrColorImages[1].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + m_intSelectedIndex + "_Image1.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 2)
                    m_smVisionInfo.g_arrColorImages[2].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + m_intSelectedIndex + "_Image2.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 3)
                    m_smVisionInfo.g_arrColorImages[3].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + "_" + m_intSelectedIndex + "_Image3.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 4)
                    m_smVisionInfo.g_arrColorImages[4].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + "_" + m_intSelectedIndex + "_Image4.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 5)
                    m_smVisionInfo.g_arrColorImages[5].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + "_" + m_intSelectedIndex + "_Image5.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 6)
                    m_smVisionInfo.g_arrColorImages[6].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + "_" + m_intSelectedIndex + "_Image6.bmp");
            }
            else
            {
                m_smVisionInfo.g_arrImages[0].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + m_intSelectedIndex + ".bmp");
                if (m_smVisionInfo.g_arrImages.Count > 1)
                    m_smVisionInfo.g_arrImages[1].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + m_intSelectedIndex + "_Image1.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 2)
                    m_smVisionInfo.g_arrImages[2].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + m_intSelectedIndex + "_Image2.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 3)
                    m_smVisionInfo.g_arrImages[3].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + "_" + m_intSelectedIndex + "_Image3.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 4)
                    m_smVisionInfo.g_arrImages[4].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + "_" + m_intSelectedIndex + "_Image4.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 5)
                    m_smVisionInfo.g_arrImages[5].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + "_" + m_intSelectedIndex + "_Image5.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 6)
                    m_smVisionInfo.g_arrImages[6].SaveImage(strFolderPath + strFeatureFolderName + "Template\\OriTemplate" + "_" + m_intSelectedIndex + "_Image6.bmp");
            }

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1u");

            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].SaveImage(strFolderPath + strFeatureFolderName + "Template\\Template" + m_intSelectedIndex + ".bmp");

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1v");

            if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 2)
            {
                int intLastSubROIIndex = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count - 1;    // Sub ROI is always in last array
                bool IndexFound = false;
                if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastSubROIIndex].ref_intType == 4)
                    IndexFound = true;

                if (IndexFound)
                {
                    m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastSubROIIndex].SaveImage(strFolderPath + strFeatureFolderName + "Template\\SubTemplate" + m_intSelectedIndex + ".bmp");
                }
            }

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1w");

            m_objCopy.CopyAllImageFiles(strFolderPath + strFeatureFolderName + "Template\\", strPath + "New\\");

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1x");
            
            #endregion

            #region Save ROI settings

            ROI objROI;
            //STDeviceEdit.CopySettingFile(strFolderPath + strFeatureFolderName, "ROI.xml");
            objFile = new XmlParser(strFolderPath + strFeatureFolderName + "ROI.xml", true);
            for (x = 0; x < m_smVisionInfo.g_arrOrientROIs.Count; x++)
            {
                objFile.WriteSectionElement("Unit" + x);
                for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[x].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrOrientROIs[x][j];
                    if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                    {
                        ROI objSelectedROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][j];
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

                    objFile.WriteEndElement();
                    //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient ROI", strFolderPath + strFeatureFolderName, "ROI.xml");
                }
            }
            #endregion

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1y");

            #region Save Gauge Settings
            if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 0) //  2020 03 13 - CCENG: No need to save Gauge if bottom Orient
            {
                // Save Gauge Settings
                //SaveGaugeSettings(strFolderPath + strFeatureFolderName, m_smVisionInfo.g_arrOrientGauge, m_smVisionInfo.g_arrOrientROIs);
                SaveGaugeM4LSettings(strFolderPath + strFeatureFolderName, m_smVisionInfo.g_arrOrientGaugeM4L, m_smVisionInfo.g_arrOrientROIs);
            }
            #endregion

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1z");

            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (m_smVisionInfo.g_arrPin1 != null)
                {

                    float fOffsetRefPosX = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterX -
                                           m_smVisionInfo.g_arrPin1[m_smVisionInfo.g_intSelectedUnit].ref_objPin1ROI.ref_ROICenterX;

                    float fOffsetRefPosY = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterY -
                                           m_smVisionInfo.g_arrPin1[m_smVisionInfo.g_intSelectedUnit].ref_objPin1ROI.ref_ROICenterY;

                    ROI objPin1ROI = m_smVisionInfo.g_arrPin1[m_smVisionInfo.g_intSelectedUnit].ref_objPin1ROI;
                    // Load Pin 1
                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                    {

                        m_smVisionInfo.g_arrPin1[u].LearnTemplate(m_smVisionInfo.g_intSelectedTemplate, fOffsetRefPosX, fOffsetRefPosY, objPin1ROI, m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                        m_smVisionInfo.g_arrPin1[u].SaveTemplate(strFolderPath + strFeatureFolderName + "Template\\", m_smVisionInfo.g_intSelectedTemplate);
                    }

                    //2020 05 08 ZJYEOH : Overwrite template 1 settings to other templates
                    if (m_smVisionInfo.g_blnWantSet1ToAll)// && m_intSelectedIndex > 0)
                    {
                        string strPin1Path = strFolderPath;
                        //string strPin1Path = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                        //  m_smVisionInfo.g_strVisionFolderName + "\\";

                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                        {
                            m_smVisionInfo.g_arrPin1[u].SetTemplate1SettingtoOtherTemplate(strPin1Path + "Orient\\Template\\");
                            m_smVisionInfo.g_arrPin1[u].SavePin1Setting(strPin1Path + "Orient\\Template\\");
                            m_smVisionInfo.g_arrPin1[u].LoadTemplate(strPin1Path + "Orient\\Template\\");
                        }
                    }
                    else
                    {
                        //if (cbo_TemplateNo.SelectedItem.ToString() == "New...")
                        if (Mark0.Image == null && m_intSelectedIndex == 0 || Mark1.Image == null && m_intSelectedIndex == 1 || Mark2.Image == null && m_intSelectedIndex == 2 || Mark3.Image == null && m_intSelectedIndex == 3)
                        {
                            string strPin1Path = strFolderPath;
                            //string strPin1Path = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            //  m_smVisionInfo.g_strVisionFolderName + "\\";

                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {
                                m_smVisionInfo.g_arrPin1[u].SetPreviousTemplateSettingtoOtherTemplate(strPin1Path + "Orient\\Template\\", m_intSelectedIndex);
                                m_smVisionInfo.g_arrPin1[u].SavePin1Setting(strPin1Path + "Orient\\Template\\");
                                m_smVisionInfo.g_arrPin1[u].LoadTemplate(strPin1Path + "Orient\\Template\\");
                            }
                        }
                    }
                }
            }
            
            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1zz");
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

        //private void SaveROISettings_SECSGEM(string strPath, List<List<ROI>> arrROIs, string strVisionName)
        //{
        //    XmlParser objFile = new XmlParser(strPath);
        //    objFile.WriteSectionElement("SECSGEMData", false);

        //    //XmlParser objFile = new XmlParser(strFolderPath + "ROI.xml", true);
        //    int i, j;
        //    ROI objROI = new ROI();
        //    for (i = 0; i < arrROIs.Count; i++)
        //    {
        //        //objFile.WriteSectionElement("Unit" + i);
        //        for (j = 0; j < arrROIs[i].Count; j++)
        //        {
        //            objROI = arrROIs[i][j];
        //            if (j == 1 && i != m_smVisionInfo.g_intSelectedUnit)
        //            {
        //                ROI objSelectedROI = arrROIs[m_smVisionInfo.g_intSelectedUnit][j];
        //                objROI.ref_ROIPositionX = objSelectedROI.ref_ROIPositionX;
        //                objROI.ref_ROIPositionY = objSelectedROI.ref_ROIPositionY;
        //                objROI.ref_ROIWidth = objSelectedROI.ref_ROIWidth;
        //                objROI.ref_ROIHeight = objSelectedROI.ref_ROIHeight;
        //            }

        //            //objFile.WriteElement1Value("ROI" + j, "");

        //            objFile.WriteElement1Value(strVisionName + "_Unit" + i + "_ROI" + j + "_Name", objROI.ref_strROIName);
        //            objFile.WriteElement1Value(strVisionName + "_Unit" + i + "_ROI" + j + "_Type", objROI.ref_intType);
        //            objFile.WriteElement1Value(strVisionName + "_Unit" + i + "_ROI" + j + "_PositionX", objROI.ref_ROIPositionX);
        //            objFile.WriteElement1Value(strVisionName + "_Unit" + i + "_ROI" + j + "_PositionY", objROI.ref_ROIPositionY);
        //            objFile.WriteElement1Value(strVisionName + "_Unit" + i + "_ROI" + j + "_Width", objROI.ref_ROIWidth);
        //            objFile.WriteElement1Value(strVisionName + "_Unit" + i + "_ROI" + j + "_Height", objROI.ref_ROIHeight);

        //            float fPixelAverage = objROI.GetROIAreaPixel();
        //            objFile.WriteElement1Value(strVisionName + "_Unit" + i + "_ROI" + j + "_AreaPixel", fPixelAverage);
        //            objROI.SetROIPixelAverage(fPixelAverage);

        //            if (objROI.ref_intType > 1)
        //            {
        //                objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
        //                objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
        //            }
        //        }
        //        objFile.WriteEndElement();
        //        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName +"-"+name+ " ROI", strFolderPath, "ROI.xml");
        //    }
        //}

        private void SaveTemplateImage(string strFolderPath, ROI objROI)
        {
            // Save Template Image to template folder
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_smVisionInfo.g_arrColorImages[0].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + ".bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 1)
                    m_smVisionInfo.g_arrColorImages[1].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image1.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 2)
                    m_smVisionInfo.g_arrColorImages[2].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image2.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 3)
                    m_smVisionInfo.g_arrColorImages[3].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image3.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 4)
                    m_smVisionInfo.g_arrColorImages[4].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image4.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 5)
                    m_smVisionInfo.g_arrColorImages[5].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image5.bmp");
                if (m_smVisionInfo.g_arrColorImages.Count > 6)
                    m_smVisionInfo.g_arrColorImages[6].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image6.bmp");
            }
            else
            {
                m_smVisionInfo.g_arrImages[0].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + ".bmp");
                if (m_smVisionInfo.g_arrImages.Count > 1)
                    m_smVisionInfo.g_arrImages[1].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image1.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 2)
                    m_smVisionInfo.g_arrImages[2].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image2.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 3)
                    m_smVisionInfo.g_arrImages[3].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image3.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 4)
                    m_smVisionInfo.g_arrImages[4].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image4.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 5)
                    m_smVisionInfo.g_arrImages[5].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image5.bmp");
                if (m_smVisionInfo.g_arrImages.Count > 6)
                    m_smVisionInfo.g_arrImages[6].SaveImage(strFolderPath + "OriTemplate0" + "_" + m_intSelectedIndex + "_Image6.bmp");
            }
            objROI.SaveImage(strFolderPath + "Template0_" + m_intSelectedIndex + ".bmp");
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

                    //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
                    //if (m_smVisionInfo.g_blnWantGauge)
                    //{
                    //    if (m_smVisionInfo.g_blnViewRotatedImage)
                    //    {
                    //        //m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGauge[0].ref_fGainValue / 1000);
                    //        m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                    //    }
                    //    else
                    //    {
                    //        //m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGauge[0].ref_fGainValue / 1000);
                    //        if(m_smVisionInfo.g_arrOrientGaugeM4L.Count>0)
                    //        m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                    //    }
                    //}
                    //else
                    {
                        // 2018 12 31 - CCENG: No Gain for image without gauge.
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                            m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
                        else
                            m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
                    }

                    // ------------ Rotate image to orient 0 deg--------------------------------------
                    if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        // No need to rotate to orient 0 deg bcos no orientation test.
                    }
                    else if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
                    {
                        // No need to rotate to orient 0 deg bcos in pocket no orientation test.
                    }
                    else
                    {
                        if (m_blnWantBottom)
                        {
                            if (!StartBottomOrientTest())
                            {
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }
                        }
                        else if (m_blnWantOrient)
                        {
                            if (!StartOrientTest())
                            {
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }
                        }

                        m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    }
                    //================================================================================

                    if (m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                    {
                        pnl_Template.Visible = false;
                        gb_RotateOrientation.Visible = false;
                    }

                    if (m_intLearnType == 1)
                    {
                        srmGroupBox6.Visible = false;
                        gb_RotateOrientation.Visible = false;
                        gb_RotatePrecise.Visible = false;
                        //tp_Step1.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    else
                    {
                        btn_Next.Enabled = true;
                    }

                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewMarkTrainROI = false;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    m_blnInitGaugeAngle = false;
                    lbl_TitleStep1.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step1;
                    btn_Previous.Enabled = false;
                    //btn_Next.Enabled = true;

                    break;
                case 1: //Define Unit Pattern ROI
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    if (m_intLearnType == 8)
                    {
                        if (m_blnWantOrient)
                        {
                            if (!StartOrientTestWithUnitPR())
                            {
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }
                        }
                    }

                    if (!chk_UsePatternMatching.Checked)
                        Rotate0Degree(0, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGaugeM4L);
                    /*M4LHide*/ //Rotate0Degree(0, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGauge);
                    if (m_smVisionInfo.g_blnViewPackageImage)
                        m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage); // 2019-10-14 ZJYEOH : need to copy to g_objPackageImage, else will show not rotated image
                    if (m_intLearnType != 8)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                            AddTrainROI(i, m_smVisionInfo.g_arrOrientROIs, 2, "Unit PR ROI");
                    }
                    m_smVisionInfo.g_blnViewOrientTrainROI = true;
                    //m_smVisionInfo.g_objUnitROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewUnitROI = true;
                    m_smVisionInfo.g_blnViewUnitSurfaceROI = false;
                    m_smVisionInfo.g_blnViewMarkTrainROI = false;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    lbl_TitleStep2_0.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_UnitROI;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 8)
                    {
                        tp_UnitROI.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                        btn_Previous.Enabled = false;
                    }
                    break;
                case 2: //Define Unit Surface ROI
                    if (!chk_UsePatternMatching.Checked)
                        Rotate0Degree(0, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGaugeM4L);
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                        AddTrainROI(i, m_smVisionInfo.g_arrOrientROIs, 3, "Unit Surface ROI");

                    //m_smVisionInfo.g_objUnitROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewUnitROI = false;
                    m_smVisionInfo.g_blnViewUnitSurfaceROI = true;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnViewMarkTrainROI = false;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    lbl_TitleStep2_0.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_UnitSurfaceROI;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 3: //Define Orient Train ROI
                    if (m_intLearnType == 2)
                    {
                        if (m_blnWantBottom)
                        {
                            if (!StartBottomOrientTest())
                            {
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }
                        }
                        else if (m_blnWantOrient)
                        {
                            if (!StartOrientTest())
                            {
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }
                        }
                    }

                    if (!chk_UsePatternMatching.Checked)
                    {
                        ////Don't view package image
                        //if (m_smVisionInfo.g_blnViewPackageImage && m_smVisionInfo.g_blnDisableMOGauge)
                        //{
                        //    Rotate0Degree(1, m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrPackageGauge);

                        //    // Attached Orient ROI in RotatedImage after using packageROI to rotate image. (The Rotate0Degree functino will automatically attach packageROi to rotated image.
                        //    for (int u = 0; u < m_smVisionInfo.g_arrOrientROIs.Count; u++)
                        //    {
                        //        m_smVisionInfo.g_arrOrientROIs[u][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                        //    }

                        //}
                        //else
                        Rotate0Degree(0, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGaugeM4L);
                    }
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        if (m_smVisionInfo.g_blnWantSkipMark || (!m_blnWantOCVMark && !m_blnWantOCRMark && !m_blnWantPackage) || m_blnWantOrient && m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                            AddTrainROI(i, m_smVisionInfo.g_arrOrientROIs, 1, "Unit ROI");
                        else
                            AddTrainROI(i, m_smVisionInfo.g_arrOrientROIs, 1, "Orient ROI");
                    }

                    m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                    if (m_smVisionInfo.g_strVisionName.Contains("BottomPosition") && (m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                        {
                            AddSubROI(i, m_smVisionInfo.g_arrOrientROIs);
                        }
                    }

                    cbo_SelectLearnUnit.SelectedIndex = m_smVisionInfo.g_intSelectedUnit;
                    m_smVisionInfo.g_blnViewUnitROI = false;
                    m_smVisionInfo.g_blnViewUnitSurfaceROI = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewMarkTrainROI = false;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    m_smVisionInfo.g_blnViewOrientTrainROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewRotatedImage = true;
                    m_smVisionInfo.g_blnViewSubROI = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;

                    //// Update GUI (if Orient only)
                    //if (m_smVisionInfo.g_blnWantSkipMark || (!m_blnWantOCVMark && !m_blnWantOCRMark && !m_blnWantPackage))
                    //{
                    //    if (!m_smVisionInfo.g_blnWantSubROI)
                    //    {
                    //        tp_Step2.Controls.Add(btn_Save);
                    //        btn_Next.Enabled = false;
                    //    }
                    //    else
                    //    {
                    //        tp_Step2_2.Controls.Add(btn_Save);
                    //        btn_Next.Enabled = true;
                    //    }
                    //}
                    //else
                    //{
                    if (m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                    {
                        tp_UnitROI.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    else
                        btn_Next.Enabled = true;
                    //}

                    if (m_smVisionInfo.g_blnWantSkipMark ||
                        (!m_blnWantOCVMark && !m_blnWantOCRMark && !m_blnWantPackage) ||
                        m_smVisionInfo.g_strVisionName.Contains("BottomPosition") ||
                        m_smVisionInfo.g_strVisionName.Contains("BottomOrient"))
                    {
                        lbl_TitleStep2_0.BringToFront();
                        tabCtrl_MarkOrient.SelectedTab = tp_UnitROI;
                    }
                    else
                    {
                        lbl_TitleStep2.BringToFront();
                        tabCtrl_MarkOrient.SelectedTab = tp_Step2;
                    }

                    if (m_intLearnType == 2)
                    {
                        if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 1)
                        {
                            if (!m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                            {
                                grpbox_OrientResult.Visible = true;
                                tp_UnitROI.Controls.Add(gb_RotatePrecise);
                                tp_UnitROI.Controls.Add(gb_RotateOrientation);
                            }
                        }
                        else
                        {
                            grpbox_OrientResult.Visible = true;
                            tp_Step2.Controls.Add(grpbox_OrientResult);
                            tp_Step2.Controls.Add(gb_RotatePrecise);
                            tp_Step2.Controls.Add(gb_RotateOrientation);
                            tp_Step2.Controls.Add(btn_Save);
                            btn_Next.Enabled = false;
                        }
                        btn_Previous.Enabled = false;

                    }
                    else
                    {
                        btn_Previous.Enabled = true;
                    }
                    break;
                case 4:
                    m_smVisionInfo.g_blnViewOrientTrainROI = true;
                    m_smVisionInfo.g_blnViewSubROI = true;

                    //cbo_SubROIList.Items.Clear();
                    //if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 2)
                    //{
                    //    cbo_SubROIList.Items.Add(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_strROIName);
                    //    cbo_SubROIList.SelectedIndex = 0;
                    //}
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                        AddSubROI(i, m_smVisionInfo.g_arrOrientROIs);

                    // Update GUI //if only Orient
                    if (m_smVisionInfo.g_blnWantSkipMark || (!m_blnWantOCVMark && !m_blnWantOCRMark && !m_blnWantPackage) || m_smVisionInfo.g_strVisionName.Contains("BottomOrient"))
                    {
                        tp_Step2_2.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    else
                    {
                        btn_Next.Enabled = true;
                    }

                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    m_smVisionInfo.g_blnViewMarkTrainROI = false;
                    lbl_TitleStep2_2.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step2_2;
                    btn_Previous.Enabled = true;
                    break;
                case 5: // Pin 1 ROI
                    if (m_intLearnType == 7)
                    {
                        if (m_blnWantOrient)
                        {
                            if (!StartOrientTest())
                            {
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }

                            CopyOrientUnitROIToPin1SearchROI();

                        }
                        StartPin1Test();
                    }
                    else
                    {
                        if (m_blnWantOrient)
                        {
                            CopyOrientUnitROIToPin1SearchROI();
                        }
                    }
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = true;
                    m_smVisionInfo.g_blnViewPin1ROI = true;

                    // Update GUI //if only Orient
                    if (m_smVisionInfo.g_blnWantSkipMark || (!m_blnWantOCVMark && !m_blnWantOCRMark && !m_blnWantPackage))
                    {
                        tp_Step2_3.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    else
                    {
                        btn_Next.Enabled = true;
                    }

                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        if (m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI == null)
                        {
                            m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI = new ROI();
                            m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.LoadROISetting(0, 0, 100, 100);
                        }
                        m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.ref_strROIName = "Pin1";
                        m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.ref_intType = 2;
                        m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.AttachImage(m_smVisionInfo.g_arrPin1[i].ref_objSearchROI); // Train ROI always attach to Search ROI
                    }

                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrMarks[0].ref_intPin1ImageNo].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewUnitROI = false;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    m_smVisionInfo.g_blnViewMarkTrainROI = false;
                    m_smVisionInfo.g_blnViewOrientTrainROI = false;
                    lbl_Pin1ROI.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step2_3;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 7)
                    {
                        tp_Step2_3.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
                case 6: //Define Mark ROI
                    if (m_intLearnType == 4 || m_intLearnType == 6)
                    {
                        if (m_blnWantOrient)
                            StartOrientTest();

                        //if (m_blnWantOCVMark)
                        //    m_smVisionInfo.g_arrMarkROIs[0][1].LoadROISetting(m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionX, m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionY, m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight); //StartMarkTest();
                    }

                    // 2019 08 06 - CCENG: If under InPocket or Not Mark Orient vision, Orient ROI step 3 will be skip and direct go to Mark ROI step 4.
                    //                     but Orient ROI still need to be inited before init for Mark ROI.
                    // ============= Step 3 - Orient ROI ===================================================================================
                    if (((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                        (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM")))
                    {
                        if (!chk_UsePatternMatching.Checked)
                        {
                            if (!m_blnRotated)
                                Rotate0Degree(0, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGaugeM4L);
                        }
                        for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                        {
                            if (m_smVisionInfo.g_blnWantSkipMark || (!m_blnWantOCVMark && !m_blnWantOCRMark && !m_blnWantPackage))
                                AddTrainROI(i, m_smVisionInfo.g_arrOrientROIs, 1, "Unit ROI");
                            else
                                AddTrainROI(i, m_smVisionInfo.g_arrOrientROIs, 1, "Orient ROI");
                        }

                        m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        cbo_SelectLearnUnit.SelectedIndex = m_smVisionInfo.g_intSelectedUnit;
                        m_smVisionInfo.g_blnViewUnitROI = false;
                        m_smVisionInfo.g_blnViewUnitSurfaceROI = false;
                        m_smVisionInfo.g_blnViewSearchROI = false;
                        m_smVisionInfo.g_blnViewMarkTrainROI = false;
                        m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                        m_smVisionInfo.g_blnViewOrientTrainROI = true;
                        m_smVisionInfo.g_blnViewGauge = false;
                        m_smVisionInfo.g_blnViewRotatedImage = true;
                        m_smVisionInfo.g_blnViewSubROI = false;
                        m_smVisionInfo.g_blnViewPackageImage = true;
                    }
                    // ============= End Step 3 ===================================================================================


                    if (m_blnWantOrient)
                    {
                        CopyInfoToMark();
                    }
                    else
                    {
                        //Rotate0Degree(0, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkGauge);
                        Rotate0Degree(0, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkGaugeM4L);
                    }

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

                    lbl_TitleStep3.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step3;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 4)
                    {
                        tp_Step3.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                        btn_Previous.Enabled = false;
                    }
                    if (m_intLearnType == 6)
                    {
                        btn_Next.PerformClick();
                    }
                    break;
                case 7: //Build Object
                        //if (!m_smVisionInfo.g_blnWantGauge) // Step Mark ROI was skipped if WantGauge is false. Mark ROI will automatically set same as SearchROI size.
                        //{
                        //    if (m_blnWantOrient)
                        //    {
                        //        CopyInfoToMark();
                        //    }

                    //    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    //    {
                    //        AddTrainROI(i, m_smVisionInfo.g_arrMarkROIs, 1, "Mark ROI");

                    //        // Set Mark ROI same size as Search ROI
                    //        m_smVisionInfo.g_arrMarkROIs[i][1].LoadROISetting(0, 0, m_smVisionInfo.g_arrMarkROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[i][0].ref_ROIHeight);
                    //    }
                    //    m_smVisionInfo.g_blnViewMarkTrainROI = true;
                    //    m_smVisionInfo.g_blnViewOrientTrainROI = false;
                    //    m_smVisionInfo.g_blnViewSubROI = false;
                    //}

                    if (m_intLearnType == 9) // cxlim 2020/14/16 : attach Mark ROI to Search ROI 
                    {
                        m_smVisionInfo.g_intSelectedImage = 0; // 2020-12-30 ZJYEOH: Always use image 1 for mark
                        AddTrainROI(0, m_smVisionInfo.g_arrMarkROIs, 1, "Mark ROI"); // add this if user click segmentation b4 Mark ROI
                        m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_arrMarkROIs[0][0].AttachImage(m_smVisionInfo.g_objPackageImage);
                    }

                    // 2019 08 06 - CCENG: If under InPocket or Not Mark Orient vision, Orient ROI step 3 will be skip and direct go to Mark ROI step 4.
                    //                     So Orient ROI size is not defined properly yet. In this case, the orient ROI size will follow mark roi size.
                    // ============= Step 3 - Orient ROI ===================================================================================
                    if (((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                        (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM")))
                    {
                        if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_blnWantUseUnitPatternAsMarkPattern && (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM")))
                        {
                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].LoadROISetting(
                                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROIPositionX,
                                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROIPositionY,
                                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROIWidth,
                                m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROIHeight);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].LoadROISetting(
                            m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionX,
                            m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionY,
                            m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth,
                            m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);
                        }
                    }
                    // ============= End Step 3 ===================================================================================

                    // 2020 11 20 - CCENG: (For Position Orientation Used) Define Orient ROI position is in which corner under search ROI.
                    // ---------------------------------------------------------------------------------------------------------------------

                    float fOrientationSeparatorX = (float)m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2;
                    float fOrientationSeparatorY = (float)m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2;

                    if ((m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterX) < fOrientationSeparatorX)
                        m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][m_intSelectedIndex].ref_blnTemplateOrientationIsLeft = true;
                    else
                        m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][m_intSelectedIndex].ref_blnTemplateOrientationIsLeft = false;

                    if ((m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterY) < fOrientationSeparatorY)
                        m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][m_intSelectedIndex].ref_blnTemplateOrientationIsTop = true;
                    else
                        m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][m_intSelectedIndex].ref_blnTemplateOrientationIsTop = false;
                    // =====================================================================================================================




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

                            //if (m_smVisionInfo.g_arrPolygon_Mark[i].Count > m_smVisionInfo.g_intSelectedTemplate)
                            //{
                            //    if (m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate].ref_intFormMode != 2)
                            //    {
                            //        m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate].AddPoint(new PointF(m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                            //            m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                            //        m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate].AddPoint(new PointF((m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROITotalX + m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                            //            (m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROITotalY + m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                            //        m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate].AddPolygon((int)(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                            //    }
                            //    else
                            //    {
                            //        m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate].ResetPointsUsingOffset(m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrMarkDontCareROIs[i].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                            //        m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate].AddPolygon((int)(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                            //    }
                            //    ImageDrawing objImage = new ImageDrawing();
                            //    if (m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate].ref_intFormMode != 2)
                            //        Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                            //    else
                            //        Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_arrPolygon_Mark[i][m_smVisionInfo.g_intSelectedTemplate], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                            //    ROI objDontCareROI = new ROI();
                            //    objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                            //    objDontCareROI.AttachImage(objImage);
                            //    ROI.SubtractROI(m_smVisionInfo.g_arrMarkROIs[0][1], objDontCareROI);
                            //    objDontCareROI.Dispose();
                            //}
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

                    m_intBuildCharsStage = -1;
                    m_smVisionInfo.g_blnDragROI = false;

                    lbl_TitleStep4.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step4;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 9)
                    {
                        btn_Next.Enabled = true;
                        btn_Previous.Enabled = false;
                        m_smVisionInfo.g_blnViewMarkTrainROI = true;
                    }
                    break;
                case 8:
                    // Use blob from case 6 to build ocv and get the mark information and size.
                    STTrackLog.WriteLine("STEP=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_a");

                    if (m_intLearnType == 6)
                    {
                        m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_arrMarkROIs[0][0].AttachImage(m_smVisionInfo.g_objPackageImage);
                        m_smVisionInfo.g_arrMarkROIs[0][1].AttachImage(m_smVisionInfo.g_arrMarkROIs[0][0]);
                        if (!BuildMarkObjects())
                        { }
                    }

                    if (!m_smVisionInfo.g_blnViewObjectsBuilded)
                    {
                        SRMMessageBox.Show("Please build objects first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        m_smVisionInfo.g_intLearnStepNo--;
                        m_intDisplayStepNo--;
                        break;
                    }

                    STTrackLog.WriteLine("STEP=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_b");

                    // 2020 06 01 - m_objOcv data is not tally with selected m_arrOcv if no assign m_arrOcv object to m_objOcv single object.
                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetArrayOcvtoSingleOcv();

                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].BuildOCVAndGetInformation(
                        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0],
                        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1],
                        1);

                    STTrackLog.WriteLine("STEP=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_c");

                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ClearBuildOcvSplitPoints();//2021-09-21 ZJYEOH : Clear Split Points

                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetSplitLineCancel();

                    STTrackLog.WriteLine("STEP=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_d");

                    //2020-05-13 ZJYEOH : set previous char shift setting when learn new template after new lot clear all template
                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetCurrentCharShiftXY(m_strFolderPath + "Mark\\Template\\");
                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetCurrentCharType(m_strFolderPath + "Mark\\Template\\");

                    //2020-05-13 ZJYEOH : Set previous template char shift tolerance to current template
                    if (Mark1.Image == null && m_intSelectedIndex == 1 || Mark2.Image == null && m_intSelectedIndex == 2 || Mark3.Image == null && m_intSelectedIndex == 3)
                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetPreviosCharShiftXY();

                    for (int i = 4; i < m_smVisionInfo.g_intMaxMarkTemplate; i++)
                    {
                        if (pic_Template[i].Image == null && m_intSelectedIndex == i)
                        {
                            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetPreviosCharShiftXY();
                        }
                    }

                    m_smVisionInfo.g_blnUpdateContextMenu = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewMarkTrainROI = true;
                    m_smVisionInfo.g_blnViewCharsBuilded = true;
                    m_smVisionInfo.g_blnViewTextsBuilded = false;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    pic_BuildCharsState.Image = ils_ImageListTree.Images[1];

                    STTrackLog.WriteLine("STEP=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_e");

                    m_smVisionInfo.g_strContextMenuType = "Mark Setting";
                    m_smVisionInfo.g_blnUpdateContextMenu = true;

                    lbl_TitleStep5.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_BuildChar;
                    //if (m_smVisionInfo.g_blnWantBuildTexts)   // 2018 11 28 - CCENG: Not need to display Build Text because cusotmer wont change it at all. 
                    //{
                    //    btn_Next.Enabled = true;
                    //    tp_Step6.Controls.Add(btn_Save);
                    //    tp_Step6.Controls.Add(chk_PreviousSetting);
                    //}
                    //else
                    tp_BuildChar.Controls.Add(chk_PreviousSetting);
                    if (!m_smVisionInfo.g_blnWantMark2DCode || !m_blnWant2DCode)
                    {
                        btn_Next.Enabled = false;
                        btn_Previous.Enabled = true;
                        tp_BuildChar.Controls.Add(btn_Save);
                    }
                    else
                    {
                        btn_Next.Enabled = true;
                    }
                    if (m_intLearnType == 6)
                    {
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    STTrackLog.WriteLine("STEP=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_f");

                    break;
                //case 7: //Build Characters
                //    //if (!m_blnBuildObjectSuccess)
                //    //{
                //    //    SRMMessageBox.Show("Please define Image area first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    //    m_smVisionInfo.g_intLearnStepNo--;
                //    //    m_intDisplayStepNo--;
                //    //    break;
                //    //}

                //    if (!m_smVisionInfo.g_blnViewObjectsBuilded)
                //    {
                //        SRMMessageBox.Show("Please build objects first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        m_smVisionInfo.g_intLearnStepNo--;
                //        m_intDisplayStepNo--;
                //        break;
                //    }

                //    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].BuildOCVChars(1);
                //    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetTemplateImage(
                //        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1],
                //        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionX,
                //        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionY,
                //        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth,
                //        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);


                //    m_smVisionInfo.g_blnViewCharsBuilded = true;
                //    m_smVisionInfo.g_blnViewTextsBuilded = false;
                //    pic_BuildCharsState.Image = ils_ImageListTree.Images[1];

                //    lbl_TitleStep5.BringToFront();
                //    tabCtrl_MarkOrient.SelectedTab = tp_Step5;
                //    if (m_smVisionInfo.g_blnWantBuildTexts)
                //    {
                //        btn_Next.Enabled = true;
                //        tp_Step6.Controls.Add(btn_Save);
                //        tp_Step6.Controls.Add(chk_PreviousSetting);
                //    }
                //    else
                //    {
                //        btn_Next.Enabled = false;
                //        tp_Step5.Controls.Add(btn_Save);
                //        tp_Step5.Controls.Add(chk_PreviousSetting);
                //    }
                //    btn_Previous.Enabled = true;
                //    break;
                case 9: //Build Texts
                    if (m_blnWantOCVMark)
                    {
                        if (!m_smVisionInfo.g_blnViewObjectsBuilded)
                        {
                            SRMMessageBox.Show("Please build objects first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            break;
                        }

                        if (!m_smVisionInfo.g_blnViewCharsBuilded || m_intBuildCharsStage == 0)
                        {
                            SRMMessageBox.Show("Please build characters first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            m_smVisionInfo.g_intLearnStepNo--;
                            m_intDisplayStepNo--;
                            break;
                        }

                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].FormSelectedTexts();
                        m_smVisionInfo.g_blnViewTextsBuilded = true;
                        pic_BuildTextsState.Image = ils_ImageListTree.Images[1];
                    }

                    lbl_TitleStep6.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step6;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    break;
                case 10:
                    //Build characters
                    pic_Pattern.Image = null;

                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].BuildOCRChars(m_smVisionInfo.g_arrRotatedImages[0], m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1]);

                    //Display current pattern
                    DisplayOCRPatternOnGrid();

                    lbl_TitleStep7.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step7;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    break;
                case 11:
                    txt_RefCharManual.Text = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetRefChars(0);

                    lbl_RefCharFromImage.Text = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetOCRRecognizeResult(m_smVisionInfo.g_arrRotatedImages[0], m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1]);

                    tp_Step8.Controls.Add(btn_Save);
                    tp_Step8.Controls.Add(chk_PreviousSetting);

                    lbl_TitleStep8.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step8;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    break;
                case 12:

                    DefineMostSelectedImageNo(); // 2020-02-17 ZJYEOH : Display the image selected from gauge advance setting

                    if (m_blnInitGaugeAngle)
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                        for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
                        {
                            m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
                        }
                    }

                    if (m_intCurrentAngle != 0)
                    {
                        if (m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit].Count != 0)
                            m_intOrientDirection = m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intTemplateIndex].ref_intDirections;

                        ROI objROI = new ROI();
                        List<List<ROI>> arrROIs = new List<List<ROI>>();
                        if (m_blnWantOrient)
                        {
                            objROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0];
                            arrROIs = m_smVisionInfo.g_arrOrientROIs;
                        }
                        else
                        {
                            objROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0];
                            arrROIs = m_smVisionInfo.g_arrMarkROIs;
                        }

                        ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref objRotatedImage);

                        for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
                        {
                            objRotatedImage = m_smVisionInfo.g_arrRotatedImages[i];
                            m_smVisionInfo.g_arrImages[i].CopyTo(ref objRotatedImage);
                        }

                        // Rotate Unit IC
                        if (m_intOrientDirection == 4)
                        {
                            ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

                            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
                            {
                                ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, i);
                            }

                            if (m_smVisionInfo.g_blnViewPackageImage)
                            {
                                //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
                                //{
                                //    m_smVisionInfo.g_arrRotatedImages[1].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                                //}
                                //else
                                {
                                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                                }
                            }
                        }

                        // After rotating the image, attach the rotated image into ROI again
                        for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                        {
                            if (m_blnWantOrient)
                                m_smVisionInfo.g_arrOrientROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                            else //if (m_blnWantOCVMark || m_blnWantOCRMark)
                                m_smVisionInfo.g_arrMarkROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                        }

                        //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
                        if (m_smVisionInfo.g_blnWantGauge)
                        {
                            if (m_smVisionInfo.g_blnViewRotatedImage)
                            {
                                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                            }
                        }
                        else
                        {
                            // 2018 12 31 - CCENG: No Gain for image without gauge.
                            if (m_smVisionInfo.g_blnViewRotatedImage)
                                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
                            else
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
                        }


                    }

                    if (m_smVisionInfo.g_blnViewRotatedImage)
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                    else
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                    AttachImageToROI(m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_objPackageImage);


                    // Modify gauge size same as ROI size
                    for (int i = 0; i < m_smVisionInfo.g_arrOrientROIs.Count; i++)
                    {
                        if (i < m_smVisionInfo.g_arrOrientGaugeM4L.Count)
                        {
                            //m_smVisionInfo.g_arrOrientGaugeM4L[i].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY);
                            m_smVisionInfo.g_arrOrientGaugeM4L[i].SetGaugePlace_BasedOnEdgeROI();
                            if (m_smVisionInfo.g_blnViewRotatedImage)
                                m_smVisionInfo.g_arrOrientGaugeM4L[i].Measure_WithDontCareArea(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrOrientROIs[i][0], m_smVisionInfo.g_objWhiteImage);
                            else
                                m_smVisionInfo.g_arrOrientGaugeM4L[i].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrOrientROIs[i][0], m_smVisionInfo.g_objWhiteImage);
                        }
                    }

                    m_blnInitGaugeAngle = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    //m_smVisionInfo.g_blnViewRotatedImage = false;
                    m_smVisionInfo.g_blnViewMOGauge = true;
                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    tabCtrl_MarkOrient.SelectedTab = tp_MOGauge;
                    lbl_TitleStep12.BringToFront();
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    if (m_intLearnType == 3)
                    {
                        tp_MOGauge.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                        btn_Previous.Enabled = false;
                    }
                    break;
                case 13: //Define 2D code ROI
                    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                    {
                        AddTrainROI(i, m_smVisionInfo.g_arrMarkROIs, 2, "2D Code ROI");
                    }

                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewMarkTrainROI = false;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = true;
                    m_smVisionInfo.g_blnViewOrientTrainROI = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    m_smVisionInfo.g_blnViewSubROI = false;

                    lbl_TitleStep3_1.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step3_1;

                    btn_Next.Enabled = false;
                    tp_Step3_1.Controls.Add(btn_Save);
                    break;
                case 14: //Define Don Care Area't
                    if (m_intLearnType == 5)
                    {
                        if (m_blnWantOrient)
                            StartOrientTest();

                        if (m_blnWantOCVMark)
                            m_smVisionInfo.g_arrMarkROIs[0][1].LoadROISetting(m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionX, m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionY, m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight); //StartMarkTest();
                    }

                    for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Mark.Count; u++)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Mark[u].Count; i++)
                        {
                            m_smVisionInfo.g_arrPolygon_Mark[u][i].ClearPolygon();
                        }
                    }

                    ////Check is previously learn dont care area out of ROI Area
                    //for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Mark.Count; u++)
                    //{
                    //    for (int i = 0; i < m_smVisionInfo.g_arrPolygon_Mark[u].Count; i++)
                    //    {
                    //        m_smVisionInfo.g_arrPolygon_Mark[u][i].CheckDontCarePosition(m_smVisionInfo.g_arrMarkROIs[0][1], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                    //    }
                    //}

                    for (int i = 0; i < m_smVisionInfo.g_arrMarkDontCareROIs.Count; i++)
                    {

                        m_smVisionInfo.g_arrMarkDontCareROIs[i].AttachImage(m_smVisionInfo.g_arrMarkROIs[0][1]);
                        
                    }


                    // 2019 07 31 - JBTAN: Remove Mark ROI border line when click previous from step 7
                    m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                    //if (m_smVisionInfo.g_arrMarkDontCareROIs == null)
                    //{
                    //    m_smVisionInfo.g_arrMarkDontCareROIs.Add(new ROI());
                    //}
                    //if (m_smVisionInfo.g_arrMarkDontCareROIs.Count == 0)
                    //    m_smVisionInfo.g_arrMarkDontCareROIs.Add(new ROI());
                    //m_smVisionInfo.g_arrMarkDontCareROIs[0].AttachImage(m_smVisionInfo.g_arrMarkROIs[0][1]);
                    
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnViewMarkTrainROI = true;
                    m_smVisionInfo.g_blnViewMOGauge = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;
                    m_smVisionInfo.g_blnViewMarkTrainROI = true;
                    m_smVisionInfo.g_blnViewMark2DCodeROI = false;
                    m_smVisionInfo.g_blnViewOrientTrainROI = false;
                    m_smVisionInfo.g_blnViewPin1TrainROI = false;
                    m_smVisionInfo.g_blnViewSubROI = false;
                    lbl_TitleStep9.BringToFront();
                    tabCtrl_MarkOrient.SelectedTab = tp_Step9;
                    if (m_intLearnType == 5)
                    {
                        tp_Step9.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    if (m_intLearnType == 6)
                    {
                        btn_Next.PerformClick();
                    }
                    break;
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
        private void UpdateTemplateGUI()
        {
            if (!m_smVisionInfo.g_blnWantMultiTemplates)
                return;

            //cbo_TemplateNo.Items.Clear();

            if (chk_DeleteAllTemplates.Checked)
            {
                //cbo_TemplateNo.Items.Add("Template 1");
                m_smVisionInfo.g_intSelectedTemplate = 0;
                m_intSelectedIndex = m_smVisionInfo.g_intSelectedTemplate;
            }
            else
            {
                //// Update Template GUI
                //for (int i = 0; i < m_smVisionInfo.g_intTotalTemplates; i++)
                //{
                //    cbo_TemplateNo.Items.Add("Template " + (i + 1));
                //}

                //switch (m_smVisionInfo.g_strVisionName)
                //{
                //    case "Mark":
                //    case "MarkOrient":
                //    case "MarkPkg":
                //    case "MOPkg":
                //    case "MOLiPkg":
                //    case "MOLi":
                //        if (m_smVisionInfo.g_intTotalTemplates == 0)
                //            cbo_TemplateNo.Items.Add("Template 1");
                //        else if (m_smVisionInfo.g_intTotalTemplates < 8 && m_smVisionInfo.g_intTotalTemplates < m_smVisionInfo.g_intMaxMarkTemplate)
                //            cbo_TemplateNo.Items.Add("New...");

                //        if ((m_smVisionInfo.g_intTotalTemplates == 1) || (m_smVisionInfo.g_intTotalTemplates == 8) || m_smVisionInfo.g_intTotalTemplates == m_smVisionInfo.g_intMaxMarkTemplate)
                //            m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex = 0;
                //        else if ((m_smVisionInfo.g_intSelectedTemplate < m_smVisionInfo.g_intTotalTemplates) && (m_smVisionInfo.g_intSelectedTemplate != -1))
                //            m_intSelectedIndex = m_smVisionInfo.g_intSelectedTemplate;
                //        else
                //            m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex = m_smVisionInfo.g_intTotalTemplates;
                //        break;
                //    case "InPocket":
                //    case "InPocketPkg":
                //    case "InPocketPkgPos":
                //    case "IPMLi":
                //    case "IPMLiPkg":
                //        if (m_smVisionInfo.g_intTotalTemplates == 0)
                //            cbo_TemplateNo.Items.Add("Template 1");
                //        else if (m_smVisionInfo.g_intTotalTemplates < 4 && m_smVisionInfo.g_intTotalTemplates < m_smVisionInfo.g_intMaxMarkTemplate)
                //            cbo_TemplateNo.Items.Add("New...");

                //        if ((m_smVisionInfo.g_intTotalTemplates == 1) || (m_smVisionInfo.g_intTotalTemplates == 4) || m_smVisionInfo.g_intTotalTemplates == m_smVisionInfo.g_intMaxMarkTemplate)
                //            m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex = 0;
                //        else if ((m_smVisionInfo.g_intSelectedTemplate < m_smVisionInfo.g_intTotalTemplates) && (m_smVisionInfo.g_intSelectedTemplate != -1))
                //            m_intSelectedIndex = m_smVisionInfo.g_intSelectedTemplate;
                //        else
                //            m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex = m_smVisionInfo.g_intTotalTemplates;
                //        break;
                //}
            }

            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
            if (m_smVisionInfo.g_arrMarks.Count > 1)
                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;

            for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
            {
                chk_ShowDraggingBox.Checked = m_smVisionInfo.g_arrOrientGaugeM4L[i].ref_blnDrawDraggingBox;
                chk_ShowSamplePoints.Checked = m_smVisionInfo.g_arrOrientGaugeM4L[i].ref_blnDrawSamplingPoint;
            }
        }


        private void btn_AddPattern_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intHitCharIndex == -1)
            {
                SRMMessageBox.Show("Please select character on image first!");
                return;
            }

            SetCharForm objSetCharForm = new SetCharForm();
            objSetCharForm.Location = new Point(769, 300);
            if (objSetCharForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].AddOCRPattern(m_smVisionInfo.g_arrRotatedImages[0], m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1],
                        objSetCharForm.ref_cCharacter, objSetCharForm.ref_intClass);

                DisplayOCRPatternOnGrid();
                pic_Pattern.Image = null;
                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intHitCharIndex = -1;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void btn_BuildChar_Click(object sender, EventArgs e)
        {
            if (!m_smVisionInfo.g_blnViewObjectsBuilded)
            {
                SRMMessageBox.Show("Please build objects first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].BuildOCVChars(1);
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetTemplateImage(
                        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1],
                        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionX,
                        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionY,
                        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth,
                        m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);

            m_smVisionInfo.g_blnViewCharsBuilded = true;
            m_smVisionInfo.g_blnViewTextsBuilded = false;
            pic_BuildCharsState.Image = ils_ImageListTree.Images[1];
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BuildObj_Click(object sender, EventArgs e)
        {
            BuildMarkObjects();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BuildText_Click(object sender, EventArgs e)
        {
            if (!m_smVisionInfo.g_blnViewObjectsBuilded)
            {
                SRMMessageBox.Show("Please build objects first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!m_smVisionInfo.g_blnViewCharsBuilded)
            {
                SRMMessageBox.Show("Please build characters first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].FormSelectedTexts();
            pic_BuildTextsState.Image = ils_ImageListTree.Images[1];

            m_smVisionInfo.g_blnViewTextsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                ReadOrientSetting();
            if (((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                m_blnWantOCRMark)
                ReadMarkSetting();

            //if (m_smVisionInfo.g_arrOrients[0].Count > 0)
            //    m_smVisionInfo.g_arrOrients[0].RemoveAt(m_smVisionInfo.g_arrOrients[0].Count - 1);
            //if (m_smVisionInfo.g_intUnitsOnImage > 1)
            //    m_smVisionInfo.g_arrOrients[1].RemoveAt(m_smVisionInfo.g_arrOrients[1].Count - 1);

            if (m_smVisionInfo.g_arrOrients[0].Count > 0)
            {
                m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_arrOrients[0].Count - 1].Dispose();
                m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_arrOrients[0].Count - 1] = null;
                m_smVisionInfo.g_arrOrients[0].RemoveAt(m_smVisionInfo.g_arrOrients[0].Count - 1);
            }
            if (m_smVisionInfo.g_intUnitsOnImage > 1)
            {
                m_smVisionInfo.g_arrOrients[1][m_smVisionInfo.g_arrOrients[1].Count - 1].Dispose();
                m_smVisionInfo.g_arrOrients[1][m_smVisionInfo.g_arrOrients[1].Count - 1] = null;
                m_smVisionInfo.g_arrOrients[1].RemoveAt(m_smVisionInfo.g_arrOrients[1].Count - 1);
            }

            Polygon.LoadPolygon(m_strFolderPath + "\\Mark\\Template\\Polygon.xml", m_smVisionInfo.g_arrPolygon_Mark);

            this.Close();
            this.Dispose();
        }

        private void btn_DelPattern_Click(object sender, EventArgs e)
        {
            if (chk_DelAllOCRPatterns.Checked)
            {
                if (SRMMessageBox.Show("Are you sure you want to delete all patterns?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int intPatternNum = m_smVisionInfo.g_arrMarks[0].GetNumPatterns();
                    for (int i = 0; i < intPatternNum; i++)
                    {
                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].RemoveOCRPattern(0);   // Always remove the first one
                    }
                }

                chk_DelAllOCRPatterns.Checked = false;
            }
            else
            {
                if (m_intPatternSelectedIndex == -1)
                {
                    SRMMessageBox.Show("Please select pattern on grid table first!");
                    return;
                }

                if (SRMMessageBox.Show("Are you sure you want to delete the selected pattern?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].RemoveOCRPattern(m_intPatternSelectedIndex);
                }
            }

            m_intPatternSelectedIndex = -1;
            pic_Pattern.Image = null;
            DisplayOCRPatternOnGrid();
        }

        private void btn_FormMultiChar_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].FormMultiSelectedChars();

            m_smVisionInfo.g_blnViewCharsBuilded = true;
            pic_BuildCharsState.Image = ils_ImageListTree.Images[1];
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_FormSingChar_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].FormSingleSelectedChars();

            m_smVisionInfo.g_blnViewCharsBuilded = true;
            pic_BuildCharsState.Image = ils_ImageListTree.Images[1];
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_FormText_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].FormSelectedTexts();

            m_smVisionInfo.g_blnViewTextsBuilded = true;
            pic_BuildTextsState.Image = ils_ImageListTree.Images[1];
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            m_blnFirstTimeInit = false;
            if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_smVisionInfo.g_blnViewPackageImage) // 2019-10-14 ZJYEOH : Need to Measure gauge on g_objPackageImage if g_blnViewPackageImage == true, because will add gain to that image 
                {
                    if (!m_smVisionInfo.g_arrOrientGaugeM4L[0].Measure_WithDontCareArea(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_objWhiteImage))
                    {
                        SRMMessageBox.Show("Gauge Fail: " + m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_strErrorMessage);

                        return;
                    }
                }
                else
                {
                    if (!m_smVisionInfo.g_arrOrientGaugeM4L[0].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage))
                    {
                        SRMMessageBox.Show("Gauge Fail: " + m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_strErrorMessage);

                        return;
                    }
                }
            }
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                    {
                        // Step 0 - Search ROI
                        // Step 3 - Unit ROI
                        // Step 4 - Orient ROI

                        if (m_smVisionInfo.g_intLearnStepNo == 0)
                            m_smVisionInfo.g_intLearnStepNo = 3;
                        else if (m_smVisionInfo.g_intLearnStepNo == 3)
                            m_smVisionInfo.g_intLearnStepNo = 4;
                    }
                    break;
                case "BottomPosition":
                    {
                        // Step 0 - Search ROI
                        // Step 3 - Unit ROI

                        if (m_smVisionInfo.g_intLearnStepNo == 0)
                            m_smVisionInfo.g_intLearnStepNo = 3;
                    }
                    break;
                case "Mark":
                case "MarkPkg":
                    {
                        //Step 0: Search ROI
                        //Step 6: Mark ROI
                        //Step 7: Build Object
                        //Step 8: Build Chars
                        //Step 13: 2d code
                        if (m_smVisionInfo.g_intLearnStepNo == 0)
                        {
                            if (m_smVisionInfo.g_blnWantGauge)
                                m_smVisionInfo.g_intLearnStepNo = 12;
                            else if (m_smVisionInfo.g_blnWantPin1)
                                m_smVisionInfo.g_intLearnStepNo = 5;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 5)
                        {
                            string strPosition = "";
                            if (!Pin1ROICorrectPosition(ref strPosition))
                            {
                                SRMMessageBox.Show("Pin 1 ROI Position is not at " + strPosition + ". Please learn it properly.");
                                return;
                            }
                            m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 6)
                        {
                            if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                                m_smVisionInfo.g_intLearnStepNo = 14;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 7;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 7)
                            m_smVisionInfo.g_intLearnStepNo = 8;
                        else if (m_smVisionInfo.g_intLearnStepNo == 8)
                        {
                            if (m_blnWant2DCode && m_smVisionInfo.g_blnWantMark2DCode)
                                m_smVisionInfo.g_intLearnStepNo = 13;
                        }
                        else if (m_smVisionInfo.g_blnWantGauge && m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            m_smVisionInfo.g_blnViewMOGauge = false;
                            m_smVisionInfo.g_blnViewGauge = false;
                            if (m_smVisionInfo.g_blnWantPin1)
                                m_smVisionInfo.g_intLearnStepNo = 5;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 14)
                            m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    break;
                case "MarkOrient":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                    {
                        //Step 0: Search ROI
                        //Step 3: Orient ROI
                        //Step 5: Pin 1 ROI
                        //Step 6: Mark ROI
                        //Step 7: Build Object
                        //Step 8: Build Chars
                        //Step 13: 2d code
                        if (m_smVisionInfo.g_intLearnStepNo == 0)
                        {
                            if (m_smVisionInfo.g_blnWantGauge)
                                m_smVisionInfo.g_intLearnStepNo = 12;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 3;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 3)
                        {
                            if (m_smVisionInfo.g_blnWantPin1)
                                m_smVisionInfo.g_intLearnStepNo = 5;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 5)
                        {
                            string strPosition = "";
                            if (!Pin1ROICorrectPosition(ref strPosition))
                            {
                                SRMMessageBox.Show("Pin 1 ROI Position is not at " + strPosition + ". Please learn it properly.");
                                return;
                            }
                            m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 6)
                        {
                            if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                                m_smVisionInfo.g_intLearnStepNo = 14;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 7;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 7)
                            m_smVisionInfo.g_intLearnStepNo = 8;
                        else if (m_smVisionInfo.g_intLearnStepNo == 8)
                        {
                            if (m_blnWant2DCode && m_smVisionInfo.g_blnWantMark2DCode)
                                m_smVisionInfo.g_intLearnStepNo = 13;
                        }
                        else if (m_smVisionInfo.g_blnWantGauge && m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            m_smVisionInfo.g_blnViewMOGauge = false;
                            m_smVisionInfo.g_blnViewGauge = false;
                            m_smVisionInfo.g_intLearnStepNo = 3;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 14)
                            m_smVisionInfo.g_intLearnStepNo = 7;

                    }
                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLi":
                case "IPMLiPkg":
                    {
                        //Step 0: Search ROI
                        //Step 1: Unit ROI
                        //Step 5: Pin 1 ROI
                        //Step 6: Mark ROI
                        //Step 7: Build Object
                        //Step 8: Build Chars
                        //Step 13: 2d code
                        if (m_smVisionInfo.g_intLearnStepNo == 0)
                        {
                            if (m_smVisionInfo.g_blnWantGauge)
                                m_smVisionInfo.g_intLearnStepNo = 12;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 1;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 1)
                        {
                            if (m_smVisionInfo.g_blnWantPin1)
                                m_smVisionInfo.g_intLearnStepNo = 5;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 5)
                        {
                            string strPosition = "";
                            if (!Pin1ROICorrectPosition(ref strPosition))
                            {
                                SRMMessageBox.Show("Pin 1 ROI Position is not at " + strPosition + ". Please learn it properly.");
                                return;
                            }
                            m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 6)
                        {
                            if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                                m_smVisionInfo.g_intLearnStepNo = 14;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 7;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 7)
                            m_smVisionInfo.g_intLearnStepNo = 8;
                        else if (m_smVisionInfo.g_intLearnStepNo == 8)
                        {
                            if (m_blnWant2DCode && m_smVisionInfo.g_blnWantMark2DCode)
                                m_smVisionInfo.g_intLearnStepNo = 13;
                        }
                        else if (m_smVisionInfo.g_blnWantGauge && m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            m_smVisionInfo.g_blnViewMOGauge = false;
                            m_smVisionInfo.g_blnViewGauge = false;

                            m_smVisionInfo.g_intLearnStepNo = 1;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 14)
                            m_smVisionInfo.g_intLearnStepNo = 7;
                    }
                    break;
                case "Orient":
                case "Package":
                default:
                    {
                        if (m_smVisionInfo.g_intLearnStepNo < 10)
                            m_smVisionInfo.g_intLearnStepNo++;

                        if (m_smVisionInfo.g_intLearnStepNo == 4 && !m_smVisionInfo.g_blnWantSubROI)
                            m_smVisionInfo.g_intLearnStepNo++;

                        if (m_smVisionInfo.g_intLearnStepNo == 5 && !m_smVisionInfo.g_blnWantPin1)
                            m_smVisionInfo.g_intLearnStepNo++;

                        if (!m_blnWantOrient)
                        {
                            // Skip orient step if no Orient
                            if (m_smVisionInfo.g_intLearnStepNo == 3)
                                m_smVisionInfo.g_intLearnStepNo++;
                        }

                        if (m_blnWantOCRMark)
                        {
                            // Jump to OCR step if OCR ON
                            if (m_smVisionInfo.g_intLearnStepNo == 7)
                                m_smVisionInfo.g_intLearnStepNo = 9;
                        }

                        //if (!m_smVisionInfo.g_blnWantGauge)
                        //{
                        //    if (m_smVisionInfo.g_intLearnStepNo == 5) // Mark ROI
                        //        m_smVisionInfo.g_intLearnStepNo++;
                        //}

                        if (m_smVisionInfo.g_blnWantGauge)
                        {
                            if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
                            {
                                //Skip Unit Surface ROI
                                if (m_smVisionInfo.g_intLearnStepNo == 2)
                                    m_smVisionInfo.g_intLearnStepNo++;
                            }
                            else
                            {
                                //Skip Unit PR and Unit Surface ROI
                                if (m_smVisionInfo.g_intLearnStepNo == 1)
                                    m_smVisionInfo.g_intLearnStepNo += 2;
                            }

                        }
                        // if No lead or Positioning, then from search ROI page direct go to Orient Page 
                        else if (((m_smCustomizeInfo.g_intWantPositioningIndex & (1 << m_smVisionInfo.g_intVisionPos)) == 0) &&
                            ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
                        {
                            if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
                            {
                                //Skip Unit Surface ROI
                                if (m_smVisionInfo.g_intLearnStepNo == 2)
                                    m_smVisionInfo.g_intLearnStepNo++;
                            }
                            else
                            {
                                //Skip Unit PR and Unit Surface ROI
                                if (m_smVisionInfo.g_intLearnStepNo == 1)
                                    m_smVisionInfo.g_intLearnStepNo += 2;
                            }

                            //// Skip Unit PR and Unit Surface ROI
                            //if (m_smVisionInfo.g_intLearnStepNo == 1)
                            //    m_smVisionInfo.g_intLearnStepNo += 2;
                        }
                        // If have positioning but no lead, then from Unit PR ROI page direct go to orient page.
                        else if (((m_smCustomizeInfo.g_intWantPositioningIndex & (1 << m_smVisionInfo.g_intVisionPos)) > 0) &&
                            ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
                        {
                            // Skip Unit Surface ROI
                            if (m_smVisionInfo.g_intLearnStepNo == 2)
                                m_smVisionInfo.g_intLearnStepNo++;
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
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                    {
                        // Step 0 - Search ROI
                        // Step 3 - Unit ROI
                        // Step 4 - Orient ROI

                        if (m_smVisionInfo.g_intLearnStepNo == 4)
                            m_smVisionInfo.g_intLearnStepNo = 3;
                        else if (m_smVisionInfo.g_intLearnStepNo == 3)
                            m_smVisionInfo.g_intLearnStepNo = 0;
                    }
                    break;
                case "BottomPosition":
                    {
                        // Step 0 - Search ROI
                        // Step 3 - Unit ROI

                        if (m_smVisionInfo.g_intLearnStepNo == 3)
                            m_smVisionInfo.g_intLearnStepNo = 0;
                    }
                    break;
                case "Mark":
                case "MarkPkg":
                    {
                        //Step 0: Search ROI
                        //Step 6: Mark ROI
                        //Step 7: Build Object
                        //Step 8: Build Chars
                        //Step 13: 2d code
                        if (m_smVisionInfo.g_intLearnStepNo == 13)
                            m_smVisionInfo.g_intLearnStepNo = 8;
                        else if (m_smVisionInfo.g_intLearnStepNo == 8)
                            m_smVisionInfo.g_intLearnStepNo = 7;
                        else if (m_smVisionInfo.g_intLearnStepNo == 7)
                        {
                            if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                                m_smVisionInfo.g_intLearnStepNo = 14;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 6)
                        {

                            if (m_smVisionInfo.g_blnWantPin1)
                                m_smVisionInfo.g_intLearnStepNo = 5;
                            else if (m_smVisionInfo.g_blnWantGauge)
                                m_smVisionInfo.g_intLearnStepNo = 12;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 0;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 5)
                        {
                            if (m_smVisionInfo.g_blnWantGauge)
                                m_smVisionInfo.g_intLearnStepNo = 12;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 0;
                        }
                        else if (m_smVisionInfo.g_blnWantGauge && m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            m_smVisionInfo.g_blnViewMOGauge = false;
                            m_smVisionInfo.g_blnViewGauge = false;
                            m_smVisionInfo.g_intLearnStepNo = 0;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 14)
                            m_smVisionInfo.g_intLearnStepNo = 6;
                    }
                    break;
                case "MarkOrient":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                    {
                        //Step 0: Search ROI
                        //Step 3: Orient ROI
                        //Step 5: Pin 1 ROI
                        //Step 6: Mark ROI
                        //Step 7: Build Object
                        //Step 8: Build Chars
                        //Step 13: 2d code
                        if (m_smVisionInfo.g_intLearnStepNo == 13)
                            m_smVisionInfo.g_intLearnStepNo = 8;
                        else if (m_smVisionInfo.g_intLearnStepNo == 8)
                            m_smVisionInfo.g_intLearnStepNo = 7;
                        else if (m_smVisionInfo.g_intLearnStepNo == 7)
                        {
                            if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                                m_smVisionInfo.g_intLearnStepNo = 14;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 6)
                        {
                            if (m_smVisionInfo.g_blnWantPin1)
                                m_smVisionInfo.g_intLearnStepNo = 5;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 3;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 5)
                            m_smVisionInfo.g_intLearnStepNo = 3;
                        else if (m_smVisionInfo.g_intLearnStepNo == 3)
                        {
                            if (m_smVisionInfo.g_blnWantGauge)
                                m_smVisionInfo.g_intLearnStepNo = 12;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 0;
                        }
                        else if (m_smVisionInfo.g_blnWantGauge && m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            m_smVisionInfo.g_blnViewMOGauge = false;
                            m_smVisionInfo.g_blnViewGauge = false;
                            m_smVisionInfo.g_intLearnStepNo = 0;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 14)
                            m_smVisionInfo.g_intLearnStepNo = 6;
                    }
                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLi":
                case "IPMLiPkg":
                    {
                        //Step 0: Search ROI
                        //Step 1: Unit ROI
                        //Step 5: Pin 1 ROI
                        //Step 6: Mark ROI
                        //Step 7: Build Object
                        //Step 8: Build Chars
                        //Step 13: 2d code
                        if (m_smVisionInfo.g_intLearnStepNo == 13)
                            m_smVisionInfo.g_intLearnStepNo = 8;
                        else if (m_smVisionInfo.g_intLearnStepNo == 8)
                            m_smVisionInfo.g_intLearnStepNo = 7;
                        else if (m_smVisionInfo.g_intLearnStepNo == 7)
                        {
                            if (m_smVisionInfo.g_blnWantDontCareArea_Mark)
                                m_smVisionInfo.g_intLearnStepNo = 14;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 6;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 6)
                        {
                            if (m_smVisionInfo.g_blnWantPin1)
                                m_smVisionInfo.g_intLearnStepNo = 5;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 1;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 5)
                            m_smVisionInfo.g_intLearnStepNo = 1;
                        else if (m_smVisionInfo.g_intLearnStepNo == 1)
                        {
                            if (m_smVisionInfo.g_blnWantGauge)
                                m_smVisionInfo.g_intLearnStepNo = 12;
                            else
                                m_smVisionInfo.g_intLearnStepNo = 0;
                        }
                        else if (m_smVisionInfo.g_blnWantGauge && m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            m_smVisionInfo.g_blnViewMOGauge = false;
                            m_smVisionInfo.g_blnViewGauge = false;
                            m_smVisionInfo.g_intLearnStepNo = 0;
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 14)
                            m_smVisionInfo.g_intLearnStepNo = 6;
                    }
                    break;
                case "Orient":
                case "Package":
                default:
                    {
                        if (m_smVisionInfo.g_intLearnStepNo > 0)
                            m_smVisionInfo.g_intLearnStepNo--;

                        if (m_smVisionInfo.g_intLearnStepNo == 5 && !m_smVisionInfo.g_blnWantPin1)
                            m_smVisionInfo.g_intLearnStepNo--;

                        if (m_smVisionInfo.g_intLearnStepNo == 4 && !m_smVisionInfo.g_blnWantSubROI)
                            m_smVisionInfo.g_intLearnStepNo--;

                        // 2019 08 06 - CCENG: g_intLearnStepNo 3 is orient step. Skip orient step if under InPocket Vision or Not Makr Orient Vision.
                        if (m_smVisionInfo.g_intLearnStepNo == 3)
                        {
                            if (((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                                (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM")))
                            {
                                m_smVisionInfo.g_intLearnStepNo = 0;    // Go to first step
                            }
                        }

                        if (!m_blnWantOrient)
                        {
                            // Skip orient step if no Orient
                            if (m_smVisionInfo.g_intLearnStepNo == 3)
                                m_smVisionInfo.g_intLearnStepNo--;
                        }

                        if (!m_blnWantOCVMark)
                        {
                            // Jump to orient step if no mark
                            if (m_smVisionInfo.g_intLearnStepNo == 6)
                                m_smVisionInfo.g_intLearnStepNo = 3;
                        }

                        //if (!m_smVisionInfo.g_blnWantGauge)
                        //{
                        //    if (m_smVisionInfo.g_intLearnStepNo == 5) // Mark ROI
                        //    {
                        //        m_smVisionInfo.g_intLearnStepNo--;   // Skip Mark ROI and direct go to Orient ROI

                        //        if (!m_smVisionInfo.g_blnWantSubROI)
                        //            m_smVisionInfo.g_intLearnStepNo--;
                        //    }
                        //}

                        if (m_blnWantOCRMark)
                        {
                            // Jump to Build Objects step if OCR ON
                            if (m_smVisionInfo.g_intLearnStepNo == 7)
                                m_smVisionInfo.g_intLearnStepNo = 5;
                        }

                        if (m_smVisionInfo.g_blnWantGauge)
                        {

                            if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
                            {
                                //Skip Unit Surface ROI
                                if (m_smVisionInfo.g_intLearnStepNo == 2)
                                    m_smVisionInfo.g_intLearnStepNo--;
                            }
                            else
                            {
                                // Skip Unit PR and Surface ROI 
                                if (m_smVisionInfo.g_intLearnStepNo == 2)
                                    m_smVisionInfo.g_intLearnStepNo -= 2;
                            }

                        }
                        else if (((m_smCustomizeInfo.g_intWantPositioningIndex & (1 << m_smVisionInfo.g_intVisionPos)) == 0) &&
                            ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
                        {
                            if (m_smVisionInfo.g_strVisionName.Contains("InPocket") || m_smVisionInfo.g_strVisionName.Contains("IPM"))
                            {
                                //Skip Unit Surface ROI
                                if (m_smVisionInfo.g_intLearnStepNo == 2)
                                    m_smVisionInfo.g_intLearnStepNo--;
                            }
                            else
                            {
                                // Skip Unit PR and Surface ROI 
                                if (m_smVisionInfo.g_intLearnStepNo == 2)
                                    m_smVisionInfo.g_intLearnStepNo -= 2;
                            }
                        }
                        // If have positioning but no lead, then from Unit PR ROI page direct go to orient page.
                        else if (((m_smCustomizeInfo.g_intWantPositioningIndex & (1 << m_smVisionInfo.g_intVisionPos)) > 0) &&
                            ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
                        {
                            // Skip Unit Surface ROI 
                            if (m_smVisionInfo.g_intLearnStepNo == 3)
                                m_smVisionInfo.g_intLearnStepNo--;
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
            btn_RotateAngle.Enabled = false;

            if (m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit].Count != 0)
                m_intOrientDirection = m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_intTemplateIndex].ref_intDirections;

            ROI objROI = new ROI();
            List<List<ROI>> arrROIs = new List<List<ROI>>();
            if (m_blnWantOrient)
            {
                objROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0];
                arrROIs = m_smVisionInfo.g_arrOrientROIs;
            }
            else
            {
                objROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0];
                arrROIs = m_smVisionInfo.g_arrMarkROIs;
            }

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                objRotatedImage = m_smVisionInfo.g_arrRotatedImages[i];
                m_smVisionInfo.g_arrImages[i].CopyTo(ref objRotatedImage);
            }

            // Rotate Unit IC
            if (m_intOrientDirection == 4)
            {
                if (sender == btn_ClockWise)
                    m_intCurrentAngle += 270;
                else
                    m_intCurrentAngle += 90;

                m_intCurrentAngle %= 360;


                ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

                for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
                {
                    ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, i);
                }

                if (m_smVisionInfo.g_blnViewPackageImage)
                {
                    //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
                    //{
                    //    m_smVisionInfo.g_arrRotatedImages[1].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    //}
                    //else
                    {
                        m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    }
                }
            }
            else
            {
                m_intCurrentAngle += 180;
                m_intCurrentAngle %= 360;

                ROI.RotateROI_Center(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);
                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }
            // After rotating the image, attach the rotated image into ROI again
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (m_blnWantOrient)
                    m_smVisionInfo.g_arrOrientROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                else //if (m_blnWantOCVMark || m_blnWantOCRMark)
                    m_smVisionInfo.g_arrMarkROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            }

            //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
            if (m_smVisionInfo.g_blnWantGauge)
            {
                if (m_smVisionInfo.g_blnViewRotatedImage)
                {
                    m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                }
                else
                {
                    m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                }
            }
            else
            {
                // 2018 12 31 - CCENG: No Gain for image without gauge.
                if (m_smVisionInfo.g_blnViewRotatedImage)
                    m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
                else
                    m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
            }
            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage); // 02-07-2019 ZJYEOH : Solve rotate first time no effect
            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewGauge = false;  // 2019 06 21 - Gauge will measure according to original image only. Once rotate, gauge wont measure anymore, so not need to view Gauge after that.
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;

            if (m_intCurrentAngle == 270)
                lbl_OrientationAngle.Text = "-90";
            else
                lbl_OrientationAngle.Text = m_intCurrentAngle.ToString();

            RotatePrecise();
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

        private void btn_Save_Click(object sender, EventArgs e)
        {
            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString());

            // ------------ Copy Temporary File ---------------------------------------

            string strFolderPathTemp = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "_Temp\\";
            CopyDirectory(m_strFolderPath, strFolderPathTemp);
            // -----------------------------------------------------------------------------

            switch (m_intLearnType)
            {
                case 0:
                    {
                        // Save OCR Setting
                        if (m_blnWantOCRMark)
                        {
                            if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetNumPatterns() == 0)
                            {
                                SRMMessageBox.Show("Please learn at least one pattern before save!");
                                return;
                            }

                            if (radioBtn_ManualKeyIn.Checked)
                                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetRefChars(txt_RefCharManual.Text);
                            else if (radioBtn_RecogFromImage.Checked)
                                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetRefChars(lbl_RefCharFromImage.Text);
                            else
                                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetRefChars(lbl_RefCharFromScanner.Text);


                            SaveMarkSettings(strFolderPathTemp + "Mark\\");
                        }

                        STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_a");
                        // Save Mark Setting
                        if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
                        {
                            // Make sure Char are builded
                            if (!m_smVisionInfo.g_blnViewCharsBuilded)
                            {
                                SRMMessageBox.Show("Please build characters first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_b");

                            // Make sure Text are builded
                            if (!m_smVisionInfo.g_blnWantBuildTexts)
                            {
                                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].FormSelectedTexts();
                            }
                            else if (!m_smVisionInfo.g_blnViewTextsBuilded)
                            {
                                SRMMessageBox.Show("Please build texts first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_c");

                            // Make sure at least 1 chartext is selected
                            if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetNumTexts() == 0)
                            {
                                SRMMessageBox.Show("No Char is selected", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_d");

                            // Delete Previous Template
                            if (chk_DeleteAllTemplates.Checked)
                            {
                                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DeleteAllPreviousTemplate(strFolderPathTemp + "Mark\\");
                                m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SaveSingleTemplate(strFolderPathTemp + "Mark\\Template\\", false, chk_DeleteAllTemplates.Checked); // 2020-05-11 ZJYEOH: Save first template setting 

                                STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_e");

                                //Delete all dont care template except the first(current) one
                                //for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Mark.Count; u++)
                                //{
                                //    for (int i = 1; i < m_smVisionInfo.g_arrPolygon_Mark[u].Count; i++)
                                //    {
                                //        m_smVisionInfo.g_arrPolygon_Mark[u][i].ClearPolygon();
                                //    }
                                //}
                            }

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_f");

                            ////2020-11-19 ZJYEOH : Save another image that contain only blob that wanted to save as OCV Template image, this is to prevent subtract extra area that cause by small noise(become larger after dilate)
                            //m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DrawBlobsROI(m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1], strFolderPathTemp + "Mark\\Template\\" + "OCVTemplate0_" + m_intSelectedIndex+ ".bmp");
                            //ImageDrawing objOCVImage = new ImageDrawing(true);// 2020-11-19 ZJYEOH : OVC Image that contain clean OCV Char without noise
                            //if (File.Exists(strFolderPathTemp + "Mark\\Template\\" + "OCVTemplate0_" + m_intSelectedIndex + ".bmp"))
                            //    objOCVImage.LoadImage(strFolderPathTemp + "Mark\\Template\\" + "OCVTemplate0_" + m_intSelectedIndex + ".bmp");
                            //else
                            //{
                            //    if (File.Exists(strFolderPathTemp + "Mark\\Template\\" + "Template0_" + m_intSelectedIndex + ".bmp"))
                            //        objOCVImage.LoadImage(strFolderPathTemp + "Mark\\Template\\" + "Template0_" + m_intSelectedIndex + ".bmp");
                            //}
                            // OCV object learn image
                            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].LearnOCVTemplate(
                                    m_smVisionInfo.g_arrRotatedImages[0], /*objOCVImage,*/
                                    m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0],
                                    //m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionX,
                                    //m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionY,
                                    //m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth,
                                    //m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight,
                                    m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1], chk_PreviousSetting.Checked,
                                    !m_smVisionInfo.g_blnWantGauge, m_smVisionInfo.g_intDefaultMarkScore, strFolderPathTemp + "Mark\\Template\\", m_intSelectedIndex);
                            //objOCVImage.Dispose();
                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_g");

                            SaveMarkSettings(strFolderPathTemp + "Mark\\");

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_h");
                        }

                        if (m_blnWantOrient)
                        {
                            m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][0].ref_intRotatedAngle = m_intCurrentAngle;
                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1");

                            SaveOrientSettings(strFolderPathTemp, "Orient\\");
                            //SaveOrientSettings(strFolderPathTemp + "Orient\\");

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i2");

                            for (int x = 0; x < m_smVisionInfo.g_arrOrientROIs.Count; x++)
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrOrientROIs[x].Count; i++)
                                {
                                    if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 1)
                                        m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage(m_smVisionInfo.g_arrImages[0]);
                                    else if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 2)
                                        m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage((ROI)m_smVisionInfo.g_arrOrientROIs[x][0]);
                                    else if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 4)
                                        m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage((ROI)m_smVisionInfo.g_arrOrientROIs[x][1]);
                                }
                            }

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_j");
                        }

                        if (m_blnWantPackage && !m_blnWantBottom)
                            LoadMarkMaskBlob(strFolderPathTemp + "Mark\\Template\\", m_smVisionInfo.g_arrMarks[0].GetNumTemplates(), m_smVisionInfo.g_arrPackage);


                        if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            SaveLeadROISettings(strFolderPathTemp);


                        STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_k");

                        SaveGeneralSetting(strFolderPathTemp);

                        STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_L");

                        if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
                        {
                            LoadMarkSettings(strFolderPathTemp + "Mark\\Template\\");
                        }

                        STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_m");

                        if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                            m_smProductionInfo.g_blnSaveRecipeToServer = true;

                        STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_n");

                        m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

                        // 2020 03 16 - CCENG: Dispose LearnBlob object to prevent Close Error Display. (not sure work or not)
                        if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
                        {
                            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DisposeLearnBlob();
                        }

                    }
                    break;
                //case 1:
                case 2:
                    {
                        if (m_blnWantOrient || m_blnWantBottom)
                        {
                            m_smVisionInfo.g_arrOrients[m_smVisionInfo.g_intSelectedUnit][0].ref_intRotatedAngle = m_intCurrentAngle;
                            
                            SaveOrientSettings(strFolderPathTemp, "Orient\\");
                            //SaveOrientSettings(strFolderPathTemp + "Orient\\");
                            
                            for (int x = 0; x < m_smVisionInfo.g_arrOrientROIs.Count; x++)
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrOrientROIs[x].Count; i++)
                                {
                                    if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 1)
                                        m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage(m_smVisionInfo.g_arrImages[0]);
                                    else if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 2)
                                        m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage((ROI)m_smVisionInfo.g_arrOrientROIs[x][0]);
                                    else if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 4)
                                        m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage((ROI)m_smVisionInfo.g_arrOrientROIs[x][1]);
                                }
                            }
                        }
                        else if (m_blnWantOCVMark)
                        {
                            SaveROISettings(strFolderPathTemp + "Mark\\", m_smVisionInfo.g_arrMarkROIs, "Mark");
                        }
                    }
                    break;
                case 3:
                    {
                        if (((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                            SaveGaugeM4LSettings(strFolderPathTemp + "Mark\\", m_smVisionInfo.g_arrMarkGaugeM4L, m_smVisionInfo.g_arrMarkROIs);

                        if (((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 0) && ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                        {
                            // Save Gauge Settings
                            //SaveGaugeSettings(strFolderPath, m_smVisionInfo.g_arrOrientGauge, m_smVisionInfo.g_arrOrientROIs);
                            SaveGaugeM4LSettings(strFolderPathTemp + "Orient\\", m_smVisionInfo.g_arrOrientGaugeM4L, m_smVisionInfo.g_arrOrientROIs);
                        }
                    }
                    break;
                case 4:
                    {
                        SaveROISettings(strFolderPathTemp + "Mark\\", m_smVisionInfo.g_arrMarkROIs, "Mark");
                    }
                    break;
                case 5:
                    {
                        // Save Dont Care Setting
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
                                    //ROI objDontCareROI = new ROI();
                                    //objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                                    //objDontCareROI.AttachImage(objImage);
                                    ////2020-06-17 ZJYEOH : Subtract or Add depends on white on black or black on white
                                    //if (m_smVisionInfo.g_blnWhiteOnBlack)
                                    //    ROI.SubtractROI(m_smVisionInfo.g_arrMarkROIs[0][1], objDontCareROI);
                                    //else
                                    //    ROI.LogicOperationAddROI(m_smVisionInfo.g_arrMarkROIs[0][1], objDontCareROI);
                                    //objDontCareROI.Dispose();
                                    //objImage.Dispose();
                                }
                                
                            }

                            SaveDontCareROISettings(strFolderPathTemp + "Mark\\", m_smVisionInfo.g_arrMarkDontCareROIs, "Mark");
                            SaveDontCareSetting(strFolderPathTemp + "Mark\\" + "Template\\");

                            LoadMarkSettings(strFolderPathTemp + "Mark\\Template\\");
                        }
                    }
                    break;
                case 6:
                case 9:
                    // Save Mark Setting
                    if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
                    {
                        // Make sure Char are builded
                        if (!m_smVisionInfo.g_blnViewCharsBuilded)
                        {
                            SRMMessageBox.Show("Please build characters first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        
                        // Make sure Text are builded
                        if (!m_smVisionInfo.g_blnWantBuildTexts)
                        {
                            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].FormSelectedTexts();
                        }
                        else if (!m_smVisionInfo.g_blnViewTextsBuilded)
                        {
                            SRMMessageBox.Show("Please build texts first", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        
                        // Make sure at least 1 chartext is selected
                        if (m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetNumTexts() == 0)
                        {
                            SRMMessageBox.Show("No Char is selected", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        
                        // Delete Previous Template
                        if (chk_DeleteAllTemplates.Checked)
                        {
                            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DeleteAllPreviousTemplate(strFolderPathTemp + "Mark\\");
                            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SaveSingleTemplate(strFolderPathTemp + "Mark\\Template\\", false, chk_DeleteAllTemplates.Checked); // 2020-05-11 ZJYEOH: Save first template setting 
                            
                            //Delete all dont care template except the first(current) one
                            //for (int u = 0; u < m_smVisionInfo.g_arrPolygon_Mark.Count; u++)
                            //{
                            //    for (int i = 1; i < m_smVisionInfo.g_arrPolygon_Mark[u].Count; i++)
                            //    {
                            //        m_smVisionInfo.g_arrPolygon_Mark[u][i].ClearPolygon();
                            //    }
                            //}
                        }

                        ////2020-11-19 ZJYEOH : Save another image that contain only blob that wanted to save as OCV Template image, this is to prevent subtract extra area that cause by small noise(become larger after dilate)
                        //m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DrawBlobsROI(m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1], strFolderPathTemp + "Mark\\Template\\" + "OCVTemplate0_" + m_intSelectedIndex + ".bmp");
                        //ImageDrawing objOCVImage = new ImageDrawing(true);// 2020-11-19 ZJYEOH : OVC Image that contain clean OCV Char without noise
                        //if (File.Exists(strFolderPathTemp + "Mark\\Template\\" + "OCVTemplate0_" + m_intSelectedIndex + ".bmp"))
                        //    objOCVImage.LoadImage(strFolderPathTemp + "Mark\\Template\\" + "OCVTemplate0_" + m_intSelectedIndex + ".bmp");
                        //else
                        //{
                        //    if (File.Exists(strFolderPathTemp + "Mark\\Template\\" + "Template0_" + m_intSelectedIndex + ".bmp"))
                        //        objOCVImage.LoadImage(strFolderPathTemp + "Mark\\Template\\" + "Template0_" + m_intSelectedIndex + ".bmp");
                        //}
                        // OCV object learn image
                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].LearnOCVTemplate(
                                m_smVisionInfo.g_arrRotatedImages[0], /*objOCVImage,*/
                                m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0],
                                //m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionX,
                                //m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIPositionY,
                                //m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth,
                                //m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight,
                                m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][1], chk_PreviousSetting.Checked,
                                !m_smVisionInfo.g_blnWantGauge, m_smVisionInfo.g_intDefaultMarkScore, strFolderPathTemp + "Mark\\Template\\", m_intSelectedIndex);
                        //objOCVImage.Dispose();
                        SaveMarkSettings(strFolderPathTemp + "Mark\\");
                        
                    }

                    if (m_blnWantPackage && !m_blnWantBottom)
                        LoadMarkMaskBlob(strFolderPathTemp + "Mark\\Template\\", m_smVisionInfo.g_arrMarks[0].GetNumTemplates(), m_smVisionInfo.g_arrPackage);
                    
                    SaveGeneralSetting(strFolderPathTemp);
                    
                    if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
                    {
                        LoadMarkSettings(strFolderPathTemp + "Mark\\Template\\");
                    }
                    
                    // 2020 03 16 - CCENG: Dispose LearnBlob object to prevent Close Error Display. (not sure work or not)
                    if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
                    {
                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].DisposeLearnBlob();
                    }
                    break;
                case 7:
                    if (m_smVisionInfo.g_blnWantPin1)
                    {
                        if (m_smVisionInfo.g_arrPin1 != null)
                        {

                            float fOffsetRefPosX = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterX -
                                                   m_smVisionInfo.g_arrPin1[m_smVisionInfo.g_intSelectedUnit].ref_objPin1ROI.ref_ROICenterX;

                            float fOffsetRefPosY = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterY -
                                                   m_smVisionInfo.g_arrPin1[m_smVisionInfo.g_intSelectedUnit].ref_objPin1ROI.ref_ROICenterY;

                            ROI objPin1ROI = m_smVisionInfo.g_arrPin1[m_smVisionInfo.g_intSelectedUnit].ref_objPin1ROI;
                            // Load Pin 1
                            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                            {

                                m_smVisionInfo.g_arrPin1[u].LearnTemplate(m_smVisionInfo.g_intSelectedTemplate, fOffsetRefPosX, fOffsetRefPosY, objPin1ROI, m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                                m_smVisionInfo.g_arrPin1[u].SaveTemplate(strFolderPathTemp + "Orient\\" + "Template\\", m_smVisionInfo.g_intSelectedTemplate);
                            }

                            //2020 05 08 ZJYEOH : Overwrite template 1 settings to other templates
                            if (m_smVisionInfo.g_blnWantSet1ToAll)// && m_intSelectedIndex > 0)
                            {
                                string strPin1Path = strFolderPathTemp;
                                //string strPin1Path = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                //  m_smVisionInfo.g_strVisionFolderName + "\\";

                                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                {
                                    m_smVisionInfo.g_arrPin1[u].SetTemplate1SettingtoOtherTemplate(strPin1Path + "Orient\\Template\\");
                                    m_smVisionInfo.g_arrPin1[u].SavePin1Setting(strPin1Path + "Orient\\Template\\");
                                    m_smVisionInfo.g_arrPin1[u].LoadTemplate(strPin1Path + "Orient\\Template\\");
                                }
                            }
                            else
                            {
                                //if (cbo_TemplateNo.SelectedItem.ToString() == "New...")
                                if (Mark0.Image == null && m_intSelectedIndex == 0 || Mark1.Image == null && m_intSelectedIndex == 1 || Mark2.Image == null && m_intSelectedIndex == 2 || Mark3.Image == null && m_intSelectedIndex == 3)
                                {
                                    string strPin1Path = strFolderPathTemp;
                                    //string strPin1Path = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                    //  m_smVisionInfo.g_strVisionFolderName + "\\";

                                    for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                    {
                                        m_smVisionInfo.g_arrPin1[u].SetPreviousTemplateSettingtoOtherTemplate(strPin1Path + "Orient\\Template\\", m_intSelectedIndex);
                                        m_smVisionInfo.g_arrPin1[u].SavePin1Setting(strPin1Path + "Orient\\Template\\");
                                        m_smVisionInfo.g_arrPin1[u].LoadTemplate(strPin1Path + "Orient\\Template\\");
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 8:
                    {
                        int x = 0;
                        
                        // Keep previous orient setting 
                        bool blnUsePreviousSetting = true;// chk_PreviousSetting.Checked; //2020-05-11 ZJYEOH : Need to use previous setting to keep the setting in new template
                                                          // Get setting from previous OCV object
                        Orient[] objOrient = new Orient[m_smVisionInfo.g_intUnitsOnImage];
                        for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                        {
                            if (m_smVisionInfo.g_arrOrients[u].Count == 0 || !blnUsePreviousSetting)
                                objOrient[u] = new Orient(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            else
                            {
                                objOrient[u] = new Orient(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                                if (m_smVisionInfo.g_arrOrients[u].Count <= m_intSelectedIndex)
                                {
                                    objOrient[u].ref_fMinScore = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex - 1].ref_fMinScore;
                                    objOrient[u].ref_intDirections = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex - 1].ref_intDirections;
                                }
                                else
                                {
                                    objOrient[u].ref_fMinScore = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex].ref_fMinScore;
                                    objOrient[u].ref_intDirections = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex].ref_intDirections;
                                }
                            }
                        }
                        
                        if (m_smVisionInfo.g_arrOrients[0].Count == 0)
                            m_smVisionInfo.g_arrOrients[x].Add(objOrient[x]); //2020-05-11 ZJYEOH : If orient object is empty meaning delete all template, load back previous first template to it
                                                                              //m_smVisionInfo.g_arrOrients[0].Add(new Orient(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                                                              
                        if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 2)
                        {
                           
                            int intCenterX2 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROICenterX;
                            int intCenterY2 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROICenterY;
                            int intCenterX1 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth / 2; //ref_ROICenterX // 2020-04-09 ZJYEOH : should use half of the width as sub orient ROI is attach on unit ROI
                            int intCenterY1 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight / 2; //ref_ROICenterY // 2020-04-09 ZJYEOH : should use half of the height as sub orient ROI is attach on unit ROI
                            m_smVisionInfo.g_arrOrients[0][0].ref_intMatcherOffSetCenterX = intCenterX2 - intCenterX1;
                            m_smVisionInfo.g_arrOrients[0][0].ref_intMatcherOffSetCenterY = intCenterY2 - intCenterY1;

                            if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 3)
                            {
                                int intCenterX3 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][3].ref_ROICenterX;
                                int intCenterY3 = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][3].ref_ROICenterY;
                                m_smVisionInfo.g_arrOrients[0][0].ref_intUnitSurfaceOffsetX = intCenterX3 - intCenterX2;
                                m_smVisionInfo.g_arrOrients[0][0].ref_intUnitSurfaceOffsetY = intCenterY3 - intCenterY2;
                            }
                            
                        }

                        //
                        //STDeviceEdit.CopySettingFile(strFolderPath, "Settings.xml");
                        //m_smVisionInfo.g_arrOrients[0][0].SaveOrient(strFolderPathTemp + "Orient\\" + "Settings.xml", false, "General", true); //2021-01-18 ZJYEOH : Save Orient after save pattern because some setting not yet updated
                        // STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient", strFolderPath, "Settings.xml");
                        // Must save pattern first before saving ROI settings because when out of page, ROI attach the original image again

                        #region Save Pattern

                        for (x = 0; x < m_smVisionInfo.g_intUnitsOnImage; x++)
                        {
                            
                            // if orient object count lower than or equal to selected index, mean need add new orient
                            // Add new orient
                            if (m_smVisionInfo.g_arrOrients[x].Count <= m_intSelectedIndex)
                            {
                                m_smVisionInfo.g_arrOrients[x].Add(objOrient[x]);
                            }
                            
                            // if selected index == last object(index -1)
                            // add default value
                            if (m_intSelectedIndex == (m_smVisionInfo.g_arrOrients[x].Count - 1))
                            {
                                if (!blnUsePreviousSetting)
                                {
                                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].ref_intDirections = m_intOrientDirection;
                                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].ref_fMinScore = (float)m_smCustomizeInfo.g_intMarkDefaultTolerance / 100;
                                }
                            }
                            
                            // if (selected index < last object(index -1)
                            // delete last object
                            if (m_intSelectedIndex < (m_smVisionInfo.g_arrOrients[x].Count - 1))
                            {
                                m_smVisionInfo.g_arrOrients[x].RemoveAt(m_smVisionInfo.g_arrOrients[0].Count - 1);
                            }
                            
                            if (!Directory.Exists(strFolderPathTemp + "Orient\\" + "Template\\"))
                            {
                                Directory.CreateDirectory(strFolderPathTemp + "Orient\\" + "Template\\");
                            }

                            if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 2)
                            {
                                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].LearnUnitPRPattern(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][2]);
                                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SetUnitPRFinalReduction(2);
                                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SetUnitPRAngleSetting(0, 0);// (-10, 10);
                                m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SaveUnitPRPattern(strFolderPathTemp + "Orient\\" + "Template\\Template" + m_intSelectedIndex + "_UnitPR.mch");
                            }

                            m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].LearnPattern(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1], m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                            m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SetFinalReduction(2);
                            m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SavePattern(strFolderPathTemp + "Orient\\" + "Template\\Template" + m_intSelectedIndex + ".mch");

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1n");

                            // Learn sub pattern match
                            if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count > 2)
                            {
                                int intLastSubROIIndex = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count - 1;    // Sub ROI is always in last array
                                bool IndexFound = false;
                                if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastSubROIIndex].ref_intType == 4)
                                    IndexFound = true;
                                //if (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastSubROIIndex].ref_strROIName.IndexOf("SubROI") > 0)  // Check is the last ROI a Sub ROI?
                                if (IndexFound)
                                {
                                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].LearnSubPattern(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastSubROIIndex]);
                                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SetFinalReduction(2);
                                    m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SaveSubPattern(strFolderPathTemp + "Orient\\" + "Template\\SubTemplate" + m_intSelectedIndex + ".mch");
                                }
                            }

                            //2020-10-06 ZJYEOH : Need to rotate unit to original position when save pattern
                            float CenterX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;// (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;
                            float CenterY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2; //(float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2;

                            float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX - CenterX) * Math.Cos((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)) -//m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_intRotatedAngle
                                               ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY - CenterY) * Math.Sin((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)));
                            float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX - CenterX) * Math.Sin((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)) +
                                                ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY - CenterY) * Math.Cos((m_intCurrentAngle - m_intCurrentPreciseDeg) * Math.PI / 180)));

                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1o");
                            m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].LearnPattern4Direction_SRM(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1], fXAfterRotated, fYAfterRotated);
                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1p");
                            m_smVisionInfo.g_arrOrients[x][m_intSelectedIndex].SavePattern4Direction(strFolderPathTemp + "Orient\\" + "Template\\", "Template" + m_intSelectedIndex.ToString());
                            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_i1q");
                        }

                        //2021-01-18 ZJYEOH : Save Orient here in order to get updated setting
                        m_smVisionInfo.g_arrOrients[0][0].SaveOrient(strFolderPathTemp + "Orient\\" + "Settings.xml", false, "General", true);

                        //STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                        XmlParser objFile = new XmlParser(strFolderPathTemp + "Orient\\" + "Template\\Template.xml");
                        objFile.WriteSectionElement("Template" + m_intSelectedIndex);
                        objFile.WriteElement1Value("TemplateCenterX", m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX + m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterX);
                        objFile.WriteElement1Value("TemplateCenterY", m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY + m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROICenterY);
                        objFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[0][0].ref_fMinScore);
                        objFile.WriteElement1Value("SubMatcherCount", m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count - 2);
                        objFile.WriteEndElement();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient Template", strFolderPath, "Template\\Template.xml");

                        //2020 05 08 ZJYEOH : Overwrite template 1 settings to other templates
                        if (m_smVisionInfo.g_blnWantSet1ToAll)// && m_intSelectedIndex > 0)
                        {
                            //Orient 
                            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            {
                                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                                {
                                    for (int j = 0; j < m_smVisionInfo.g_arrOrients[u].Count; j++)
                                    {
                                        XmlParser objOrientFile = new XmlParser(strFolderPathTemp + "Orient\\" + "\\Template\\Template.xml");
                                        //XmlParser objOrientFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Orient\\Template\\Template.xml");
                                        objOrientFile.WriteSectionElement("Template" + j);
                                        objOrientFile.WriteElement1Value("MinScore", m_smVisionInfo.g_arrOrients[u][0].ref_fMinScore);
                                        objOrientFile.WriteEndElement();

                                        m_smVisionInfo.g_arrOrients[u][j].ref_fMinScore = objOrientFile.GetValueAsFloat("MinScore", 0.7f);

                                    }
                                }
                            }
                        }
                       
                        #endregion
                        

                        #region Save ROI settings

                        ROI objROI;
                        //STDeviceEdit.CopySettingFile(strFolderPath + strFeatureFolderName, "ROI.xml");
                        objFile = new XmlParser(strFolderPathTemp + "Orient\\" + "ROI.xml", true);
                        for (x = 0; x < m_smVisionInfo.g_arrOrientROIs.Count; x++)
                        {
                            objFile.WriteSectionElement("Unit" + x);
                            for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[x].Count; j++)
                            {
                                objROI = m_smVisionInfo.g_arrOrientROIs[x][j];
                                if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                                {
                                    ROI objSelectedROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][j];
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

                                objFile.WriteEndElement();
                                //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Orient ROI", strFolderPath + strFeatureFolderName, "ROI.xml");
                            }
                        }
                        #endregion
                        
                    }
                    break;
            }

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_o");


            // Rename folder

            // Delete Vision#_Backup if exist.
            string strFolderBackup = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "_Backup\\";
            if (Directory.Exists(strFolderBackup))
                Directory.Delete(strFolderBackup, true);
            
            while(true)
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
                }
                catch (Exception ex)
                {
                    if (SRMMessageBox.Show("Fail to save!. Please make sure recipe file is not used by other.", "", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                    {
                        if (Directory.Exists(strFolderPathTemp))
                            Directory.Delete(strFolderPathTemp, true);
                        return;
                    }
                }

                // 2022 01 20 - CCENG: Separate Delete FolderBackup with another try catch. If folder backup not able to deleted happen (weird case), just ignore this error and proceed to complete the save process.
                try
                {
                    Directory.Delete(strFolderBackup, true);
                    break;
                }
                catch (Exception ex)
                {
                    break;
                }
            }

            //try
            //{
            //    DisposePictureBoxImage();
            //    Directory.Move(m_strFolderPath, strFolderBackup);
            //}
            //catch
            //{
            //    DisposePictureBoxImage();
            //    CopyDirectory(m_strFolderPath, strFolderBackup);
            //    Directory.Delete(m_strFolderPath, true);

            //}
            
            //Directory.Move(m_strFolderPath, strFolderBackup);
            //CopyDirectory(m_strFolderPath, strFolderBackup);
            //Directory.Delete(m_strFolderPath, true);
            

            this.Close();
            this.Dispose();

            STTrackLog.WriteLine("Save=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_p");
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

        private void btn_UndoChar_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].UndoTemplateChars();
            m_smVisionInfo.g_blnViewCharsBuilded = false;
            pic_BuildCharsState.Image = ils_ImageListTree.Images[0];
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_UndoText_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].UndoTemplateTexts();
            m_smVisionInfo.g_blnViewTextsBuilded = false;
            pic_BuildTextsState.Image = ils_ImageListTree.Images[0];
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }



        private void cbo_LearnNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;
            if (m_smVisionInfo.g_arrMarks.Count > 1)
                m_smVisionInfo.g_arrMarks[1].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = m_intSelectedIndex;

            if (!m_blnInitDone)
                return;
        }

        private void cbo_SelectLearnUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = cbo_SelectLearnUnit.SelectedIndex;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

        private void radioBtn_CutObj_CheckedChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnCutMode = radioBtn_CutObj.Checked;
        }



        private void dgd_OCRPatternList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_intPatternSelectedIndex = e.RowIndex;

            if (m_intPatternSelectedIndex != -1)
                pic_Pattern.Image = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetPatternImage(m_intPatternSelectedIndex);
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



        private void LearnMarkOrientOCRForm_Load(object sender, EventArgs e)
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
            // Get Orient Rotate Direction Mode
            LoadOrientGlobalSettings();

            //if (m_smVisionInfo.g_blnDisableMOGauge)
            //{
            //    for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            //        AddSearchROI(i, m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_arrPackageGauge);
            //}

            // Add search ROI 
            if (m_blnWantOrient)
            {
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    AddSearchROI(i, m_smVisionInfo.g_arrOrientROIs, m_smVisionInfo.g_arrOrientGaugeM4L);
                    //if (m_smVisionInfo.g_arrPolygon_Mark.Count == i)
                    //{
                    //    m_smVisionInfo.g_arrPolygon_Mark.Add(new List<Polygon>());
                    //}
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    AddSearchROI(i, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkGaugeM4L);
                    //if (m_smVisionInfo.g_arrPolygon_Mark.Count == i)
                    //{
                    //    m_smVisionInfo.g_arrPolygon_Mark.Add(new List<Polygon>());
                    //}
                }
            }

            // Add new template object
            m_smVisionInfo.g_arrMarks[0].AddTemplate(true);
            //m_smVisionInfo.g_arrPolygon_Mark[0].Add(new Polygon());
            if (m_smVisionInfo.g_intUnitsOnImage > 1)
            {
                m_smVisionInfo.g_arrMarks[1].AddTemplate(true);
                //m_smVisionInfo.g_arrPolygon_Mark[1].Add(new Polygon());
            }

            // Add new orient object
            // Get setting from previous OCV object
            Orient[] objOrient = new Orient[m_smVisionInfo.g_intUnitsOnImage];
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (m_smVisionInfo.g_arrOrients[u].Count == 0)
                    objOrient[u] = new Orient(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                else
                {
                    objOrient[u] = new Orient(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                    if (m_smVisionInfo.g_arrOrients[u].Count <= m_intSelectedIndex)
                    {
                        // 2019 12 03 - Happen scenario where m_intSelectedIndex is 3 but m_smVisionInfo.g_arrOrients[u].Count is 1. So need to change from m_intSelectedIndex to m_smVisionInfo.g_arrOrients[u].Count
                        //objOrient[u].ref_fMinScore = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex - 1].ref_fMinScore;
                        //objOrient[u].ref_intDirections = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex - 1].ref_intDirections;
                        objOrient[u].ref_fMinScore = m_smVisionInfo.g_arrOrients[u][m_smVisionInfo.g_arrOrients[u].Count - 1].ref_fMinScore;
                        objOrient[u].ref_intDirections = m_smVisionInfo.g_arrOrients[u][m_smVisionInfo.g_arrOrients[u].Count - 1].ref_intDirections;
                    }
                    else
                    {
                        objOrient[u].ref_fMinScore = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex].ref_fMinScore;
                        objOrient[u].ref_intDirections = m_smVisionInfo.g_arrOrients[u][m_intSelectedIndex].ref_intDirections;
                    }
                }

                m_smVisionInfo.g_arrOrients[u].Add(objOrient[u]);
            }

            //if (m_smVisionInfo.g_arrPolygon_Mark.Count == 0)
            //{
            //    m_smVisionInfo.g_arrPolygon_Mark.Add(new List<Polygon>());
            //    m_smVisionInfo.g_arrPolygon_Mark[0].Add(new Polygon());
            //}

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

            if (!m_blnWantBottom)
            {
                m_smVisionInfo.g_blnViewGauge = true;
            }

            if (m_smVisionInfo.g_arrOrients[0][0].ref_intCorrectAngleMethod == 0)
            {
                chk_UsePatternMatching.Checked = false;
                m_smVisionInfo.g_blnViewGauge = true;

            }
            else
            {
                chk_UsePatternMatching.Checked = true;
                m_smVisionInfo.g_blnViewGauge = false;
            }
            lbl_StepNo.BringToFront();
            cbo_SelectLearnUnit.SelectedIndex = 0;

            SetupSteps();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void LearnMarkOrientOCRForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (!m_blnWantOCVMark)
            //    m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Learn Orient Form Closed", "Exit Learn Orient Form", "", "", m_smProductionInfo.g_strLotID);
            //else
            //    m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Learn Mark Form Closed", "Exit Learn Mark Form", "", "", m_smProductionInfo.g_strLotID);
            STTrackLog.WriteLine("FormClosing=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_a");
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
            m_smVisionInfo.g_blnEnableMarkContextMenu = true;
            m_smVisionInfo.g_strContextMenuType = "Production";
            m_smVisionInfo.g_blnUpdateContextMenu = true;

            m_smProductionInfo.g_blnViewROITool = m_blnViewROITool;

            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;

            STTrackLog.WriteLine("FormClosing=" + m_smVisionInfo.g_intLearnStepNo.ToString() + "_b");
        }

        private void btn_AddSubROI_Click(object sender, EventArgs e)
        {
            if (cbo_SubROIList.Items.Count >= 1)
            {
                SRMMessageBox.Show("Maximum only 1 sub roi can be added.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            AddSubROI(m_smVisionInfo.g_intSelectedUnit, m_smVisionInfo.g_arrOrientROIs);

            int intLastIndex = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count - 1;
            cbo_SubROIList.Items.Add(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][intLastIndex].ref_strROIName);

            cbo_SubROIList.SelectedIndex = cbo_SubROIList.Items.Count - 1;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_RemoveSubROI_Click(object sender, EventArgs e)
        {
            if (cbo_SubROIList.Items.Count == 0)
            {
                SRMMessageBox.Show("Cannot remove because there is no Sub ROI.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (cbo_SubROIList.SelectedIndex < 0)
            {
                SRMMessageBox.Show("Please select Sub ROI before press remove button.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            int intLastIndex = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].Count - 1;
            m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit].RemoveAt(intLastIndex);

            cbo_SubROIList.Items.RemoveAt(cbo_SubROIList.Items.Count - 1);
            if (cbo_SubROIList.Items.Count > 0)
                cbo_SubROIList.SelectedIndex = cbo_SubROIList.Items.Count - 1;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }


        private void chk_UsePatternMatching_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_smVisionInfo.g_arrOrients.Count > 0)
                if (m_smVisionInfo.g_arrOrients[0].Count > 0)
                {
                    if (chk_UsePatternMatching.Checked)
                        m_smVisionInfo.g_arrOrients[0][0].ref_intCorrectAngleMethod = 1;
                    else
                        m_smVisionInfo.g_arrOrients[0][0].ref_intCorrectAngleMethod = 0;
                }

            if (!chk_UsePatternMatching.Checked && !m_smVisionInfo.g_blnViewRotatedImage)
            {
                m_smVisionInfo.g_blnViewGauge = true;
            }
            else
            {
                m_smVisionInfo.g_blnViewGauge = false;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_RotateAngle_Click(object sender, EventArgs e)
        {
            if (txt_RotateAngle.Text == null || txt_RotateAngle.Text == "")
                return;

            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;
            btn_RotateAngle.Enabled = false;

            ROI objROI = new ROI();
            List<List<ROI>> arrROIs = new List<List<ROI>>();
            if (m_blnWantOrient)
            {
                objROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0];
                arrROIs = m_smVisionInfo.g_arrOrientROIs;
            }
            else
            {
                objROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0];
                arrROIs = m_smVisionInfo.g_arrMarkROIs;
            }

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);
            ROI.Rotate0Degree(objROI, m_intCurrentAngle - float.Parse(txt_RotateAngle.Text), 8, ref m_smVisionInfo.g_arrRotatedImages, 0);
            if (m_smVisionInfo.g_blnViewPackageImage)
                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            // After rotating the image, attach the rotated image into ROI again
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (m_blnWantOrient)
                    m_smVisionInfo.g_arrOrientROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                else //if (m_blnWantOCVMark || m_blnWantOCRMark)
                    m_smVisionInfo.g_arrMarkROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            }

            //if (m_smVisionInfo.g_blnDisableMOGauge && m_smVisionInfo.g_arrImages.Count > 1)
            //if (m_smVisionInfo.g_blnWantGauge)
            //{
            //    if (m_smVisionInfo.g_blnViewRotatedImage)
            //    {
            //        m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
            //    }
            //    else
            //    {
            //        m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
            //    }
            //}
            //else
            //{
            //    // 2018 12 31 - CCENG: No Gain for image without gauge.
            //    if (m_smVisionInfo.g_blnViewRotatedImage)
            //        m_smVisionInfo.g_arrRotatedImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
            //    else
            //        m_smVisionInfo.g_arrImages[0].AddGain(ref m_smVisionInfo.g_objPackageImage, 1f);
            //}
            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewGauge = false;  // 2019 06 21 - Gauge will measure according to original image only. Once rotate, gauge wont measure anymore, so not need to view Gauge after that.
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;
        }

        private void tp_Step1_Click(object sender, EventArgs e)
        {

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

            if (m_blnWant2DCode && m_smVisionInfo.g_blnUpdate2DCodeResult)
            {
                m_smVisionInfo.g_blnUpdate2DCodeResult = false;

                lbl_2DCodeResult.Text = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].Get2DCodeResult();
                if (lbl_2DCodeResult.Text == "-----")
                    lbl_2DCodeResult.BackColor = Color.Red;
                else
                    lbl_2DCodeResult.BackColor = Color.Lime;
            }

            if (m_objAdvancedRectGaugeForm != null)
            {
                if (!m_objAdvancedRectGaugeForm.ref_blnShowForm)
                {
                    DefineMostSelectedImageNo(); // 2020-02-17 ZJYEOH : Display the image selected from gauge advance setting

                    m_objAdvancedRectGaugeForm.Close();
                    m_objAdvancedRectGaugeForm.Dispose();
                    m_objAdvancedRectGaugeForm = null;
                    btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_GaugeAdvanceSetting.Enabled = chk_ShowDraggingBox.Enabled = chk_ShowSamplePoints.Enabled = true;
                    if (m_intLearnType == 3)
                    {
                        btn_Next.Enabled = btn_Previous.Enabled = false;
                    }
                }
            }
        }

        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            m_objAdvancedRectGaugeForm = new AdvancedRectGaugeM4LForm(m_smVisionInfo, m_strFolderPath + "Orient\\GaugeM4L.xml", m_smProductionInfo.g_strRecipePath +
                               m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);

            m_objAdvancedRectGaugeForm.StartPosition = FormStartPosition.Manual;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            m_objAdvancedRectGaugeForm.Location = new Point(resolution.Width - m_objAdvancedRectGaugeForm.Size.Width, resolution.Height - m_objAdvancedRectGaugeForm.Size.Height);
            m_objAdvancedRectGaugeForm.Show();

            btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_GaugeAdvanceSetting.Enabled = false;
        }

        private void txt_ImageGain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fImageGain = Convert.ToSingle(txt_ImageGain.Value);

            for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
            {
                m_smVisionInfo.g_arrOrientGaugeM4L[i].ref_fGainValue = fImageGain * 1000;
            }

            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fImageGain);
            }

            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Undo_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawFreeShapeDone = true;
            if (m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedDontCareROIIndex].Count > m_smVisionInfo.g_intSelectedTemplate)
            {
                m_smVisionInfo.g_arrPolygon_Mark[m_smVisionInfo.g_intSelectedDontCareROIIndex][m_smVisionInfo.g_intSelectedTemplate].UndoPolygon();
            }
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

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
            {
                m_smVisionInfo.g_arrOrientGaugeM4L[i].ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
            {
                m_smVisionInfo.g_arrOrientGaugeM4L[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ROISaveClose_Click(object sender, EventArgs e)
        {
            // Save Orient
            if (m_blnWantOrient)
            {
                XmlParser objFile = new XmlParser(m_strFolderPath + "Orient\\" + "Template\\Template.xml");
                ROI objROI;
                //STDeviceEdit.CopySettingFile(strFolderPath, "ROI.xml");
                objFile = new XmlParser(m_strFolderPath + "Orient\\" + "ROI.xml", false);
                for (int x = 0; x < m_smVisionInfo.g_arrOrientROIs.Count; x++)
                {
                    objFile.WriteSectionElement("Unit" + x);
                    for (int j = 0; j < m_smVisionInfo.g_arrOrientROIs[x].Count; j++)
                    {
                        if (j == 0)
                        {
                            objROI = m_smVisionInfo.g_arrOrientROIs[x][j];
                            if (j == 1 && x != m_smVisionInfo.g_intSelectedUnit)
                            {
                                ROI objSelectedROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][j];
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

                //for (int x = 0; x < m_smVisionInfo.g_arrOrientROIs.Count; x++)
                //{
                //    for (int i = 0; i < m_smVisionInfo.g_arrOrientROIs[x].Count; i++)
                //    {
                //        if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 1)
                //            m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage(m_smVisionInfo.g_arrImages[0]);
                //        else if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 2)
                //            m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage((ROI)m_smVisionInfo.g_arrOrientROIs[x][0]);
                //        else if (m_smVisionInfo.g_arrOrientROIs[x][i].ref_intType == 4)
                //            m_smVisionInfo.g_arrOrientROIs[x][i].AttachImage((ROI)m_smVisionInfo.g_arrOrientROIs[x][1]);
                //    }
                //}

                CopyInfoToMark();
            }

            if (m_blnWantOCVMark)
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
            }

            if (m_blnWantOCVMark && !m_smVisionInfo.g_blnWantSkipMark)
            {
                LoadMarkSettings(m_strFolderPath + "Mark\\Template\\");
            }

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

            //2020-05-15 ZJYEOH : Save package ROI too 
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                SavePackageROISettings();

            if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                SaveLeadROISettings(m_strFolderPath);

            this.Close();
            this.Dispose();
        }

        private void DefineMostSelectedImageNo()
        {
            int[] intCountImage = { 0, 0, 0, 0, 0 };

            int[] arrPackageSizeImageIndex = m_smVisionInfo.g_arrOrientGaugeM4L[0].GetGaugeImageNoList();

            for (int j = 0; j < 4; j++)
            {
                switch (arrPackageSizeImageIndex[j])
                {
                    case 0:
                        intCountImage[0]++;
                        break;
                    case 1:
                        intCountImage[1]++;
                        break;
                    case 2:
                        intCountImage[2]++;
                        break;
                    case 3:
                        intCountImage[3]++;
                        break;
                    case 4:
                        intCountImage[4]++;
                        break;
                }
            }
            
            int intMostOccurValue = Math.Max(Math.Max(Math.Max(intCountImage[0], intCountImage[1]), Math.Max(intCountImage[2], intCountImage[3])), intCountImage[4]);
            m_smVisionInfo.g_intSelectedImage = Array.IndexOf(intCountImage, intMostOccurValue);

            //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
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

        private void RotatePrecise()
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;
            btn_RotateAngle.Enabled = false;

            ROI objROI = new ROI();
            List<List<ROI>> arrROIs = new List<List<ROI>>();
            if (m_blnWantOrient)
            {
                objROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0];
                arrROIs = m_smVisionInfo.g_arrOrientROIs;
            }
            else
            {
                objROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0];
                arrROIs = m_smVisionInfo.g_arrMarkROIs;
            }

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);

            for(int i=0;i< m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, i);
            }

            if (m_smVisionInfo.g_blnViewPackageImage)
                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            // After rotating the image, attach the rotated image into ROI again
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (m_blnWantOrient)
                    m_smVisionInfo.g_arrOrientROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                else //if (m_blnWantOCVMark || m_blnWantOCRMark)
                    m_smVisionInfo.g_arrMarkROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            }

            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewGauge = false;  // 2019 06 21 - Gauge will measure according to original image only. Once rotate, gauge wont measure anymore, so not need to view Gauge after that.
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " deg";
            else
                srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " 度";
            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;
        }

        private void btn_ClockWisePrecisely_Click(object sender, EventArgs e)
        {
            if (m_intCurrentPreciseDeg >= 15)
                return;

            m_intCurrentPreciseDeg++;
            RotatePrecise();
        }

        private void btn_CounterClockWisePrecisely_Click(object sender, EventArgs e)
        {
            if (m_intCurrentPreciseDeg <= -15)
                return;

            m_intCurrentPreciseDeg--;
            RotatePrecise();
        }

        private bool StartBottomOrientTest()
        {
            m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
            }

            // make sure template learn
            //if (m_smVisionInfo.g_arrOrients[0].Count < 3)
            if (m_smVisionInfo.g_arrOrientROIs[0].Count < 3)    // 2021 10 19 - CCENG: Should check using OrientROI. If use arrOrient, then should not < 3.
            {
                m_smVisionInfo.g_strErrorMessage = "*Orient : No Template Found";
                m_smVisionInfo.g_strErrorMessage += "*Please relearn template using wizard.";
                return false;
            }

            // reset all inspection data
            for (int i = 0; i < m_smVisionInfo.g_arrOrients[0].Count; i++)
            {
                ((Orient)m_smVisionInfo.g_arrOrients[0][i]).ResetInspectionData();
            }
            //m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            m_smVisionInfo.g_intOrientResult[0] = -1;  // 0:0deg, 1:90deg, 2:180deg, 3:-90, 4:Fail
            m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

            int intAngle = m_smVisionInfo.g_arrOrients[0][0].DoOrientationInspection_WithSubMatcher4(m_smVisionInfo.g_arrRotatedImages[0],
                        //m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrOrientROIs[0][1], m_smVisionInfo.g_arrOrientROIs[0][2], 2);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                        m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrOrientROIs[0][1], m_smVisionInfo.g_arrOrientROIs[0][2], 1);  // 2020 01 08 - CCENG: Change from 2 to 1 bcos for unit 0603 pattern, angle not enough precise if FinalReduction is 2.

            m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_arrOrients[0][0].GetMinScore();
            m_smVisionInfo.g_intSelectedOcv[0] = 0;
            m_smVisionInfo.g_intOrientResult[0] = intAngle;

            Orient objOrient = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]];

            m_smVisionInfo.g_fOrientCenterX[0] = objOrient.ref_fObjectX;
            m_smVisionInfo.g_fOrientCenterY[0] = objOrient.ref_fObjectY;
            m_smVisionInfo.g_fSubOrientCenterX[0] = objOrient.ref_fSubObjectX;
            m_smVisionInfo.g_fSubOrientCenterY[0] = objOrient.ref_fSubObjectY;
            m_smVisionInfo.g_fOrientScore[0] = objOrient.GetMinScore();
            m_smVisionInfo.g_fOrientAngle[0] = objOrient.ref_fDegAngleResult;
            //m_smVisionInfo.g_blnViewOrientObject = true;

            m_smVisionInfo.g_fOrientCenterX[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectX;
            m_smVisionInfo.g_fOrientCenterY[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectY;

            m_intCurrentAngle = 0;
            lbl_OrientationAngle.Text = "";
            switch (m_smVisionInfo.g_intOrientResult[0])
            {
                case 0:
                    lbl_OrientationAngle.Text += "0";
                    m_intCurrentAngle = 0;
                    break;
                case 1:
                    lbl_OrientationAngle.Text += "-90";
                    m_intCurrentAngle = 270;
                    break;
                case 2:
                    lbl_OrientationAngle.Text += "180";
                    m_intCurrentAngle = 180;
                    break;
                case 3:
                    lbl_OrientationAngle.Text += "90";
                    m_intCurrentAngle = 90;
                    break;
                case 4:
                    lbl_OrientationAngle.Text += "Fail";
                    m_intCurrentAngle = 0;
                    break;
            }

            lbl_OrientScore.Text = Math.Round(m_smVisionInfo.g_fOrientScore[0] * 100, 2).ToString();

            if (m_smVisionInfo.g_intOrientResult[0] == 4)
            {
                if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage == "" && m_smVisionInfo.g_strErrorMessage == "")
                    {
                        m_smVisionInfo.g_strErrorMessage = "*Recipe is corrupted. Please relearn.";
                    }
                    else
                    {
                        m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage;
                    }
                }
                else
                {
                    if (m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage == "" && m_smVisionInfo.g_strErrorMessage == "")
                    {
                        m_smVisionInfo.g_strErrorMessage = "*Recipe is corrupted. Please relearn.";
                    }
                    else
                    {
                        m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage;
                    }
                }
                return false;
            }
            else
            {
                ROI objROI = new ROI();
                if (m_blnWantOrient)
                {
                    objROI = m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0];
                }
                else
                {
                    objROI = m_smVisionInfo.g_arrMarkROIs[m_smVisionInfo.g_intSelectedUnit][0];
                }
                ROI.RotateROI_Center(objROI, Convert.ToInt32(lbl_OrientationAngle.Text) + m_smVisionInfo.g_fOrientAngle[0], ref m_smVisionInfo.g_arrRotatedImages, 0);
                m_intCurrentPreciseDeg = -(int)(Math.Round(m_smVisionInfo.g_fOrientAngle[0]));
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " deg";
                else
                    srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " 度";
                float Angle = Convert.ToInt32(lbl_OrientationAngle.Text) + m_smVisionInfo.g_fOrientAngle[0]; // this formula is based on clockwise rotation so the angle need to be inverted, 
                Angle = -Angle;
                float fXAfterRotated = (float)((objROI.ref_ROICenterX - objROI.ref_ROITotalX) + ((m_smVisionInfo.g_fOrientCenterX[0] - (objROI.ref_ROICenterX - objROI.ref_ROITotalX)) * Math.Cos(Angle * Math.PI / 180)) - ((m_smVisionInfo.g_fOrientCenterY[0] - (objROI.ref_ROICenterY - objROI.ref_ROITotalY))) * Math.Sin(Angle * Math.PI / 180));

                float fYAfterRotated = (float)((objROI.ref_ROICenterY - objROI.ref_ROITotalY) + ((m_smVisionInfo.g_fOrientCenterX[0] - (objROI.ref_ROICenterX - objROI.ref_ROITotalX)) * Math.Sin(Angle * Math.PI / 180)) + ((m_smVisionInfo.g_fOrientCenterY[0] - (objROI.ref_ROICenterY - objROI.ref_ROITotalY))) * Math.Cos(Angle * Math.PI / 180));


                m_smVisionInfo.g_arrOrientROIs[0][1].LoadROISetting((int)(fXAfterRotated - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth / 2),
                  (int)(fYAfterRotated - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight / 2),
                   m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth,
                    m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight);

                fXAfterRotated = (float)((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionX) + ((m_smVisionInfo.g_fSubOrientCenterX[0] - (m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionX)) * Math.Cos(Angle * Math.PI / 180)) - ((m_smVisionInfo.g_fSubOrientCenterY[0] - (m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionY))) * Math.Sin(Angle * Math.PI / 180));

                fYAfterRotated = (float)((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionY) + ((m_smVisionInfo.g_fSubOrientCenterX[0] - (m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionX)) * Math.Sin(Angle * Math.PI / 180)) + ((m_smVisionInfo.g_fSubOrientCenterY[0] - (m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIPositionY))) * Math.Cos(Angle * Math.PI / 180));


                m_smVisionInfo.g_arrOrientROIs[0][2].LoadROISetting((int)(fXAfterRotated - m_smVisionInfo.g_arrOrientROIs[0][2].ref_ROIWidth / 2),
                (int)(fYAfterRotated - m_smVisionInfo.g_arrOrientROIs[0][2].ref_ROIHeight / 2),
                 m_smVisionInfo.g_arrOrientROIs[0][2].ref_ROIWidth,
                  m_smVisionInfo.g_arrOrientROIs[0][2].ref_ROIHeight);
            }

            return true;
        }
        private bool StartOrientTest()
        {
            m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
            }
            // make sure template learn
            if (m_smVisionInfo.g_arrOrients[0].Count == 0 || m_smVisionInfo.g_intTotalTemplates == 0)
            {
                m_smVisionInfo.g_strErrorMessage = "*Orient : No Template Found";
                m_smVisionInfo.g_strErrorMessage += "*Please relearn template using wizard.";
                return false;
            }
            
            // reset all inspection data
            for (int i = 0; i < m_smVisionInfo.g_arrOrients[0].Count; i++)
            {
                ((Orient)m_smVisionInfo.g_arrOrients[0][i]).ResetInspectionData();
            }

            float fUnitSurfaceOffsetX = 0;
            float fUnitSurfaceOffsetY = 0;
            float fUnitPRResultCenterX = 0;
            float fUnitPRResultCenterY = 0;
            float fUnitPRResultAngle = 0;

            // Use Gauge to find unit angle and rotate it to 0 deg
            if (m_smVisionInfo.g_blnWantGauge && ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
            {
                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[0]);

                // Add gain value to image and attached all position ROI to gain image.
                float fGaugeAngle;
                bool blnGaugeResult = false;
                //if (m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue != 1000)  // 2019 05 29 - CCENG: Enable this gain feature because hard to get unit edge sometime. Dun know why this feature disabled previously
                //{
                //    m_smVisionInfo.g_arrImages[0].AddGain(ref m_objOrientGainImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                //    m_smVisionInfo.g_arrOrientGaugeM4L[0].SetGaugePlace_BasedOnEdgeROI();
                //    blnGaugeResult = m_smVisionInfo.g_arrOrientGaugeM4L[0].Measure_WithDontCareArea(m_objOrientGainImage, m_smVisionInfo.g_objWhiteImage);
                //    fGaugeAngle = m_fOrientGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectAngle;
                //}
                //else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[0].SetGaugePlace_BasedOnEdgeROI();
                    blnGaugeResult = m_smVisionInfo.g_arrOrientGaugeM4L[0].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage);
                    fGaugeAngle = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectAngle;
                }
                if (!blnGaugeResult)
                {
                    m_smVisionInfo.g_strErrorMessage = "*Orient : " + m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_strErrorMessage;
                    m_smVisionInfo.g_strErrorMessage += "*Please relearn gauge.";
                    return false;
                }
                // RotateROI has same center point with gauge measure center point.
                ROI objRotateROI = new ROI();
                objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                objRotateROI.LoadROISetting(
                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.X -
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.Y -
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth,
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);

                // Rotate unit to exact 0 degree (m_fOrientGauge used in Package)
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, fGaugeAngle, 0, ref m_smVisionInfo.g_arrRotatedImages, 0); // Clear image is not so important in Orient Matching. Use interpolation 0 to save rotation time.
                objRotateROI.Dispose();
            }
            else // No rect gauge
            {
                //// If Lead Unit, use unit lead pattern to find unit surface ROI
                //if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)
                //{
                //    m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrOrientROIs[0][0]);
                //    fUnitPRResultCenterX = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX();
                //    fUnitPRResultCenterY = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY();
                //    fUnitPRResultAngle = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultAngle();
                //    fUnitSurfaceOffsetX = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetX;
                //    fUnitSurfaceOffsetY = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetY;

                //    if (!Math2.GetNewXYAfterRotate(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + fUnitPRResultCenterX,
                //                                  m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + fUnitPRResultCenterY,
                //                                  fUnitSurfaceOffsetX,
                //                                  fUnitSurfaceOffsetY,
                //                                  fUnitPRResultAngle,
                //                                  ref fUnitSurfaceOffsetX,
                //                                  ref fUnitSurfaceOffsetY))
                //    { }

                //    ROI objRotateROI = new ROI();
                //    objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                //    objRotateROI.LoadROISetting(
                //        (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX +
                //        fUnitPRResultCenterX -
                //        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2 +
                //        fUnitSurfaceOffsetX, 0, MidpointRounding.AwayFromZero),
                //        (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY +
                //        fUnitPRResultCenterY -
                //        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2 +
                //        fUnitSurfaceOffsetY, 0, MidpointRounding.AwayFromZero),
                //        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth,
                //        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);


                //    // Rotate unit to exact 0 degree (m_fOrientGauge used in Package)
                //    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, fUnitPRResultAngle, 0, ref m_smVisionInfo.g_arrRotatedImages, 0); // Clear image is not so important in Orient Matching. Use interpolation 0 to save rotation time.

                //    objRotateROI.Dispose();
                //}
                //else if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)    // Bottom Orient
                //{
                //    m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
                //}
                //else // if not lead unit, mean it is QFN. There is no way to find unit surface ROI without gauge tool.
                {
                    m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
                }
            }

            int intMatchCount = 0;
            m_smVisionInfo.g_intOrientResult[0] = -1;  // 0:0deg, 1:90deg, 2:180deg, 3:-90, 4:Fail
            
            //else // Whole active templates test
            if (m_smVisionInfo.g_blnWantSetTemplateBasedOnBinInfo)
            {
                float fHighestScore = -1;
                m_smVisionInfo.g_arrMarks[0].ref_blnInspectAllTemplate = true;
                int intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
                if (intTemplateIndex >= 0)
                {
                    int intAngle;
                    if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                    {
                        m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                        intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.


                        if (m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore() > fHighestScore)
                        {
                            fHighestScore = m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore();
                            m_smVisionInfo.g_intSelectedOcv[0] = intTemplateIndex;
                            m_smVisionInfo.g_intOrientResult[0] = intAngle;

                        }
                    }
                }
            }
            else
            {

                float fHighestScore = -1;
                m_smVisionInfo.g_arrMarks[0].ref_blnInspectAllTemplate = true;
                do
                {
                    int intTemplateIndex = (int)((m_smVisionInfo.g_intTemplatePriority >> (0x04 * intMatchCount)) & 0x0F) - 1;
                    if (intTemplateIndex >= 0)
                    {
                        int intAngle;
                        bool blnPreciseAngleResult = true;
                        if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                        {
                            if (((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || ((m_smCustomizeInfo.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                                (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            {
                                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                                intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_intFinalReduction_Direction, m_smVisionInfo.g_intFinalReduction_MarkDeg, true, !m_smVisionInfo.g_blnWantGauge,
                                m_smVisionInfo.g_arrMarks[0].GetMarkAngleTolerance(0, intTemplateIndex),
                                ref blnPreciseAngleResult, ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0),
                                !m_smVisionInfo.g_blnWhiteOnBlack);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.

                                // 2021 12 15 - CCENG: This one seem not working
                                //intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                //m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge,
                                //m_smVisionInfo.g_arrMarks[0].GetMarkAngleTolerance(0, intTemplateIndex),
                                //ref blnPreciseAngleResult);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                                
                                //intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                //    m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                            }
                            else
                            {
                                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                                intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection_WithSubMatcher4(m_smVisionInfo.g_arrRotatedImages[0],
                                            m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrOrientROIs[0][1], m_smVisionInfo.g_arrOrientROIs[0][2], 2);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.

                                //intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection_WithSubMatcher(
                                //    m_smVisionInfo.g_arrOrientROIs[0][0], 2, true, m_smVisionInfo.g_blnWantSubROI && (m_smVisionInfo.g_arrOrientROIs[0].Count > 2));  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.

                                //intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].MatchWithTemplate(
                                //    m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrRotatedImages[0], true, fHighestScore, true, // false
                                //    m_smVisionInfo.g_blnWantSubROI && (m_smVisionInfo.g_arrOrientROIs[0].Count > 2));
                            }

                            if (m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore() > fHighestScore)
                            {
                                fHighestScore = m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore();
                                m_smVisionInfo.g_intSelectedOcv[0] = intTemplateIndex;
                                m_smVisionInfo.g_intOrientResult[0] = intAngle;

                            }
                        }
                    }
                    intMatchCount++;
                    //} while (((fHighestScore < 0.8) && (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count)) || !blnAngleResult); 
                } while (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count);
            }

            Orient objOrient = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]];

            m_smVisionInfo.g_fOrientCenterX[0] = objOrient.ref_fObjectX;
            m_smVisionInfo.g_fOrientCenterY[0] = objOrient.ref_fObjectY;
            m_smVisionInfo.g_fSubOrientCenterX[0] = objOrient.ref_fSubObjectX;
            m_smVisionInfo.g_fSubOrientCenterY[0] = objOrient.ref_fSubObjectY;
            m_smVisionInfo.g_fOrientScore[0] = objOrient.GetMinScore();
            m_smVisionInfo.g_fOrientAngle[0] = objOrient.ref_fDegAngleResult;
            //m_smVisionInfo.g_blnViewOrientObject = true;

            if ((m_blnWantOCVMark) || ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
            {
                if (m_smVisionInfo.g_intOrientResult[0] < 4)
                {
                    if (objOrient.ref_blnWantUsePositionCheckOrientation)
                    {
                        // 2020 11 20 - CCENG: Method 2 orientation
                        float fOrientationSeparatorX = (float)m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2;
                        float fOrientationSeparatorY = (float)m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2;

                        for (int m = 0; m < objOrient.ref_arrMatchScoreList.Count; m++)
                        {
                            if (m == m_smVisionInfo.g_intOrientResult[0])
                                continue;

                            if (objOrient.ref_arrMatchScoreList[m] >= objOrient.ref_fMinScore)
                            {
                                if (Math.Abs(objOrient.GetMinScore() - objOrient.ref_arrMatchScoreList[m]) < objOrient.ref_fCheckPositionOrientationWhenBelowDifferentScore)
                                {
                                    if (objOrient.ref_intDirections == 2)
                                    {
                                        bool blnSampleIsTop = objOrient.ref_fObjectY - fOrientationSeparatorY < 0;
                                        bool blnDirectionisTop = objOrient.ref_blnTemplateOrientationIsTop != blnSampleIsTop;

                                        if (blnDirectionisTop)            // rotate 180  from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 2;
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 0;            // no rotate
                                        }
                                    }
                                    else
                                    {
                                        bool blnSampleIsLeft = objOrient.ref_fObjectX - fOrientationSeparatorX < 0;
                                        bool blnSampleIsTop = objOrient.ref_fObjectY - fOrientationSeparatorY < 0;

                                        bool blnDirectionIsLeft = objOrient.ref_blnTemplateOrientationIsLeft != blnSampleIsLeft;
                                        bool blnDirectionisTop = objOrient.ref_blnTemplateOrientationIsTop != blnSampleIsTop;

                                        if (blnDirectionIsLeft && blnDirectionisTop)            // rotate 180  from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 2;
                                        }
                                        else if (blnDirectionIsLeft && !blnDirectionisTop)      // rotate 90 ccw from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 1;            // show result angle 90 when rotate ccw from template point
                                        }
                                        else if (!blnDirectionIsLeft && blnDirectionisTop)      // rotate 90 cw from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 3;            // show result angle -90 when rotate cw from template point
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 0;            // no rotate
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    m_intCurrentAngle = 0;
                    lbl_OrientationAngle.Text = "";
                    switch (m_smVisionInfo.g_intOrientResult[0])
                    {
                        default:
                        case 0:
                            lbl_OrientationAngle.Text = "0";
                            m_intCurrentAngle = 0;
                            break;
                        case 1:
                            lbl_OrientationAngle.Text = "90";
                            m_intCurrentAngle = 90;
                            break;
                        case 2:
                            lbl_OrientationAngle.Text = "180";
                            m_intCurrentAngle = 180;
                            break;
                        case 3:
                            lbl_OrientationAngle.Text = "-90";
                            m_intCurrentAngle = 270;
                            break;
                    }
                    lbl_OrientScore.Text = Math.Round(m_smVisionInfo.g_fOrientScore[0] * 100, 2).ToString();

                    ROI objRotatedROI = new ROI();
                    float fTotalRotateAngle = 0;
                    if (m_smVisionInfo.g_blnWantGauge)
                    {
                        RectGaugeM4L objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0];

                        // Get Orient Center Point (Final result for next MarkTest and PackageTest)
                        m_smVisionInfo.g_fUnitCenterX[0] = objGauge.ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX;
                        m_smVisionInfo.g_fUnitCenterY[0] = objGauge.ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY;

                        // Calculate total angle 
                        fTotalRotateAngle = Convert.ToInt32(lbl_OrientationAngle.Text) + objGauge.ref_fRectAngle;
                        m_intCurrentPreciseDeg = -(int)(Math.Round(objGauge.ref_fRectAngle));
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " deg";
                        else
                            srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " 度";

                        // Get RotateROI where the ROI center point == Unit Center Point
                        objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                        float fSizeX, fSizeY;
                        if ((objGauge.ref_fRectWidth + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) > m_smVisionInfo.g_arrImages[0].ref_intImageWidth ||
                            (objGauge.ref_fRectHeight + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) > m_smVisionInfo.g_arrImages[0].ref_intImageHeight)
                        {
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;
                        }
                        else
                        {
                            fSizeX = objGauge.ref_fRectWidth + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = objGauge.ref_fRectHeight + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;
                        }

                        objRotatedROI.LoadROISetting((int)Math.Round(objGauge.ref_pRectCenterPoint.X - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(objGauge.ref_pRectCenterPoint.Y - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));

                    }
                    else
                    {
                        // For Lead Unit Case
                        if (m_smVisionInfo.g_arrOrientROIs[0].Count > 3 &&
                           (((m_smCustomizeInfo.g_intWantPositioningIndex & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                           (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)))    // Mean Unit Surface ROI exist
                        {
                            // Get Unit Surface Center Point (Final result for next MarkTest and PackageTest)
                            m_smVisionInfo.g_fUnitCenterX[0] = fUnitPRResultCenterX + fUnitSurfaceOffsetX;
                            m_smVisionInfo.g_fUnitCenterY[0] = fUnitPRResultCenterY + fUnitSurfaceOffsetY;

                            // Calculate total angle 
                            fTotalRotateAngle = Convert.ToInt32(lbl_OrientationAngle.Text) + fUnitPRResultAngle;
                            m_intCurrentPreciseDeg = -(int)(Math.Round(fUnitPRResultAngle));
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " deg";
                            else
                                srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " 度";

                            // Get RotateROI where the ROI center point == Unit Center Point
                            objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                            float fSizeX, fSizeY;
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;

                            objRotatedROI.LoadROISetting(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX +
                                                            (int)Math.Round(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX() -
                                                            fSizeX / 2 +
                                                            m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetX, 0, MidpointRounding.AwayFromZero),
                                                         m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY +
                                                            (int)Math.Round(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY() -
                                                            fSizeY / 2 +
                                                            m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetY, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));
                        }
                        else // For No Lead Case
                        {
                            // Calculate total angle 
                            fTotalRotateAngle = Convert.ToInt32(lbl_OrientationAngle.Text) + m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult;
                            m_intCurrentPreciseDeg = -(int)(Math.Round(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult));
                            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " deg";
                            else
                                srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " 度";

                            // Get RotateROI where the ROI center point == Unit Center Point
                            objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                            float fSizeX, fSizeY;
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;

                            // 2019 09 06 - CCENG: If no package size center point, then use Orient Search ROI Center Point
                            objRotatedROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));

                        }
                    }

                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, 0);

                    m_blnRotated = true;

                    // 2020 04 19 - cceng: Rotate image to mark 0 deg
                    ROI.Rotate0Degree_Better(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, 4, ref m_smVisionInfo.g_objMarkImage);

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (m_smVisionInfo.g_arrImages.Count > m_smVisionInfo.g_arrLead[0].ref_intImageViewNo)
                        {
                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo], objRotatedROI, fTotalRotateAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrLead[0].ref_intImageViewNo);
                           
                        }
                    }

                    objRotatedROI.Dispose();
                }
            }
            else
            {
                m_smVisionInfo.g_fOrientCenterX[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectX;
                m_smVisionInfo.g_fOrientCenterY[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectY;
            }

            if (m_smVisionInfo.g_intOrientResult[0] == 4)
            {
                lbl_OrientationAngle.Text = "Fail";
                lbl_OrientScore.Text = Math.Round(m_smVisionInfo.g_fOrientScore[0] * 100, 2).ToString();

                if (m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage == "" && m_smVisionInfo.g_strErrorMessage == "")
                {
                    m_smVisionInfo.g_strErrorMessage = "*Recipe is corrupted. Please relearn.";
                }
                else
                {
                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage;
                }

                return false;
            }
            else
            {

                //ROI.RotateROI_Center(objROI, Convert.ToInt32(lbl_OrientationAngle.Text) + m_smVisionInfo.g_fOrientAngle[0], ref m_smVisionInfo.g_arrRotatedImages, 0);

                float Angle = Convert.ToInt32(lbl_OrientationAngle.Text) + m_smVisionInfo.g_fOrientAngle[0]; // this formula is based on clockwise rotation so the angle need to be inverted, 
                Angle = -Angle;
                float fXAfterRotated = (float)((m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX) + ((m_smVisionInfo.g_fOrientCenterX[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX)) * Math.Cos(Angle * Math.PI / 180)) - ((m_smVisionInfo.g_fOrientCenterY[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY))) * Math.Sin(Angle * Math.PI / 180));

                float fYAfterRotated = (float)((m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY) + ((m_smVisionInfo.g_fOrientCenterX[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX)) * Math.Sin(Angle * Math.PI / 180)) + ((m_smVisionInfo.g_fOrientCenterY[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY))) * Math.Cos(Angle * Math.PI / 180));


                m_smVisionInfo.g_arrOrientROIs[0][1].LoadROISetting((int)(fXAfterRotated - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth / 2),
                  (int)(fYAfterRotated - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight / 2),
                   m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth,
                    m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight);

            }
            return true;
        }
        private bool StartPin1Test()
        {
            // make sure template learn
            if (m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting.Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage += "*Pin1 : No Template Found";
                return false;
            }

            m_smVisionInfo.g_arrPin1[0].ResetInspectionData();
            m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

            int intMatchCount = 0;
            bool blnResult;
            string strErrorMessage = "";
            int intTemplateIndex;
            // Single template test
            if (!m_smVisionInfo.g_blnInspectAllTemplate)
            {
                intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
                if (m_smVisionInfo.g_arrPin1[0].ref_objTestROI == null)
                    m_smVisionInfo.g_arrPin1[0].ref_objTestROI = new ROI();

                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.AttachImage(m_smVisionInfo.g_arrOrientROIs[0][0]);
                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                    (int)(m_smVisionInfo.g_fOrientCenterX[0] -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(intTemplateIndex) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetX(intTemplateIndex)),
                    (int)(m_smVisionInfo.g_fOrientCenterY[0] -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(intTemplateIndex) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetY(intTemplateIndex)),
                    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(intTemplateIndex) * 2,
                    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(intTemplateIndex) * 2);

                m_smVisionInfo.g_arrPin1[0].ref_blnFinalResultPassFail = m_smVisionInfo.g_arrPin1[0].MatchWithTemplate(m_smVisionInfo.g_arrPin1[0].ref_objTestROI, m_smVisionInfo.g_intSelectedTemplate);
                m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate = intTemplateIndex;
                strErrorMessage = m_smVisionInfo.g_arrPin1[0].ref_strErrorMessage;
            }
            else // Whole active templates test
            {
                float fHighestScore = 0;
                //do
                //{
                //float fHighestScore = 0; // 2020-04-15 ZJYEOH : move outside the loop so that highest score will not reset
                intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate; // (int)((m_smVisionInfo.g_intTemplatePriority >> (0x04 * intMatchCount)) & 0x0F) - 1;
                    if (intTemplateIndex >= 0)
                    {
                        if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                        {
                            if (m_smVisionInfo.g_arrPin1[0].ref_objTestROI == null)
                                m_smVisionInfo.g_arrPin1[0].ref_objTestROI = new ROI();

                            m_smVisionInfo.g_arrPin1[0].ref_objTestROI.AttachImage(m_smVisionInfo.g_arrOrientROIs[0][0]);
                            float RotatedOrientCenterX = m_smVisionInfo.g_fOrientCenterX[0];
                            float RotatedOrientCenterY = m_smVisionInfo.g_fOrientCenterY[0];
                            float CenterX, CenterY;
                            if (m_smVisionInfo.g_blnWantGauge)
                            {
                                CenterX = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX;
                                CenterY = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY;

                            }
                            else
                            {

                                CenterX = (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) / 2;
                                CenterY = (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) / 2;
                            }

                            float RotationAngle = m_intCurrentAngle;
                            if (RotationAngle == 270)
                                RotationAngle = 90;
                            else if (RotationAngle == 90)
                                RotationAngle = -90;

                            RotatedOrientCenterX = (float)((CenterX) + ((m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Cos(RotationAngle * Math.PI / 180)) -
                   ((m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Sin(RotationAngle * Math.PI / 180)));
                            RotatedOrientCenterY = (float)((CenterY) + ((m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Sin(RotationAngle * Math.PI / 180)) +
                             ((m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Cos(RotationAngle * Math.PI / 180)));

                            m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                                (int)(Math.Abs(RotatedOrientCenterX) -
                                m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(intTemplateIndex) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetX(intTemplateIndex)),
                                (int)(Math.Abs(RotatedOrientCenterY) -
                                m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(intTemplateIndex) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetY(intTemplateIndex)),
                                (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(intTemplateIndex) * 2,
                                (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(intTemplateIndex) * 2);

                            //m_smVisionInfo.g_arrPin1[0].ref_objTestROI.SaveImage("D:\\TestROI.bmp");

                            blnResult = m_smVisionInfo.g_arrPin1[0].MatchWithTemplate(m_smVisionInfo.g_arrPin1[0].ref_objTestROI, intTemplateIndex);

                            if (m_smVisionInfo.g_arrPin1[0].GetResultScore(intTemplateIndex) > 0 && 
                                m_smVisionInfo.g_arrPin1[0].GetResultScore(intTemplateIndex) > fHighestScore)
                            {
                                fHighestScore = m_smVisionInfo.g_arrPin1[0].GetResultScore(intTemplateIndex);
                                m_smVisionInfo.g_arrPin1[0].ref_blnFinalResultPassFail = blnResult;
                                m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate = intTemplateIndex;
                                strErrorMessage = m_smVisionInfo.g_arrPin1[0].ref_strErrorMessage;
                            }
                        }
                    }
                //    intMatchCount++;
                //} while (intMatchCount < m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting.Count);
            }
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI != null)
                {
                    m_smVisionInfo.g_arrPin1[i].ref_objPin1ROI.LoadROISetting(
                           (int)(m_smVisionInfo.g_arrPin1[i].ref_objTestROI.ref_ROITotalX - m_smVisionInfo.g_arrPin1[i].ref_objSearchROI.ref_ROITotalX +
                           m_smVisionInfo.g_arrPin1[i].GetResultPosX(m_smVisionInfo.g_intSelectedTemplate) -
                           m_smVisionInfo.g_arrPin1[i].GetPin1PatternWidth(m_smVisionInfo.g_intSelectedTemplate) / 2),
                           (int)(m_smVisionInfo.g_arrPin1[i].ref_objTestROI.ref_ROITotalY - m_smVisionInfo.g_arrPin1[i].ref_objSearchROI.ref_ROITotalY +
                           m_smVisionInfo.g_arrPin1[i].GetResultPosY(m_smVisionInfo.g_intSelectedTemplate) -
                           m_smVisionInfo.g_arrPin1[i].GetPin1PatternHeight(m_smVisionInfo.g_intSelectedTemplate) / 2),
                           m_smVisionInfo.g_arrPin1[i].GetPin1PatternWidth(m_smVisionInfo.g_intSelectedTemplate),
                           m_smVisionInfo.g_arrPin1[i].GetPin1PatternHeight(m_smVisionInfo.g_intSelectedTemplate));
                }

            }
            //m_smVisionInfo.g_blnDrawPin1Result = true;

            if (m_smVisionInfo.g_arrPin1[0].ref_blnFinalResultPassFail)
            {

                return true;
            }
            else
            {
                m_smVisionInfo.g_strErrorMessage = strErrorMessage;
                return false;
            }
        }
        private void LoadLeadROI(string strPath, List<List<ROI>> arrROIs, int intROICount)
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
        private void SaveLeadROI(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "ROI.xml", false);

            ROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrLeadROIs.Count; t++)
            {
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
        }
        private void SaveLeadROISettings(string strFolderPath)
        {
            if (m_smVisionInfo.g_arrLeadROIs.Count == 0)
            {
                m_smVisionInfo.g_arrLeadROIs.Add(new List<ROI>());
            }

            if (m_smVisionInfo.g_arrLeadROIs[0].Count == 0)
                m_smVisionInfo.g_arrLeadROIs[0].Add(new ROI());
            
            m_smVisionInfo.g_arrLeadROIs[0][0].LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIPositionX, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIPositionY,
                m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIHeight);
            //m_smVisionInfo.g_intSelectedImage = 0;
            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            
            SaveLeadROI(strFolderPath + "Lead\\");
            LoadLeadROI(strFolderPath + "Lead\\ROI.xml", m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrLead.Length);

            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();

        }
        private void SavePackageROISettings()
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                // ---------------- Define Search ROI --------------------------
                ROI objPackageROI = new ROI();
                m_smVisionInfo.g_arrOrientROIs[i][0].CopyTo(ref objPackageROI);

                if (i >= m_smVisionInfo.g_arrPackageROIs.Count)
                    m_smVisionInfo.g_arrPackageROIs.Add(new List<ROI>());

                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 0)
                    m_smVisionInfo.g_arrPackageROIs[i].Add(objPackageROI);
                else
                    m_smVisionInfo.g_arrPackageROIs[i][0] = objPackageROI;



                // ---------------- Define Unit ROI --------------------------

                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 1)
                {
                    m_smVisionInfo.g_arrPackageROIs[i].Add(new ROI());

                    // Set half size of search roi to unit Roi as default.
                    m_smVisionInfo.g_arrPackageROIs[i][1].LoadROISetting(m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROIWidth / 4,
                                                                         m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROIHeight / 4,
                                                                         m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROIWidth / 2,
                                                                         m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROIHeight / 2);
                }

                // Attach search ROI to image
                m_smVisionInfo.g_arrPackageROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);

                // Attach unit ROI to search ROI
                m_smVisionInfo.g_arrPackageROIs[i][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[i][0]);
            }

            ROI objSearchROI = new ROI();
            ROI objTrainROI = new ROI();
            ROI objROI;

            
            STDeviceEdit.CopySettingFile(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\", "ROI.xml");

            XmlParser objFile = new XmlParser(m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe +
                "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\" + "ROI.xml", true);

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
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Package ROI", m_smProductionInfo.g_strLotID);
                    
                }
            }
        }

        private void btn_MarkGrayValueSensitivitySetting_Click(object sender, EventArgs e)
        {
            GrayValueSensitivitySettingForm objForm = new GrayValueSensitivitySettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, 0);
            objForm.ShowDialog();

            BuildMarkObjects();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private bool StartOrientTestWithUnitPR()
        {
            int intUnitNo = 0;
            m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
            }
           
            // make sure template learn
            if (m_smVisionInfo.g_arrOrients[intUnitNo].Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage = "*Unit PR : No Template Found";
                m_smVisionInfo.g_strErrorMessage += "*Please relearn template using wizard.";
                return false;
            }

            // reset all inspection data
            for (int i = 0; i < m_smVisionInfo.g_arrOrients[intUnitNo].Count; i++)
            {
                ((Orient)m_smVisionInfo.g_arrOrients[intUnitNo][i]).ResetInspectionData();
            }

            float fUnitSurfaceOffsetX = 0;
            float fUnitSurfaceOffsetY = 0;
            float fUnitPRResultCenterX = 0;
            float fUnitPRResultCenterY = 0;
            float fUnitPRResultAngle = 0;
            m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            // Use Gauge to find unit angle and rotate it to 0 deg
            if (m_smVisionInfo.g_blnWantGauge) // Use
            {
                    m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
                
                if (m_smVisionInfo.g_blnWantUseUnitPRFindGauge)// (m_smVisionInfo.g_arrOrientROIs[intUnitNo].Count > 2)
                {
                    if (m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0]))
                    {

                        fUnitPRResultCenterX = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX() + m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIPositionX;
                        fUnitPRResultCenterY = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY() + m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIPositionY;
                        int intUnitPRWidth = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRWidth();
                        int intUnitPRHeight = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRHeight();

                        if (m_smVisionInfo.g_arrOrientGaugeM4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0], (int)fUnitPRResultCenterX, (int)fUnitPRResultCenterY, intUnitPRWidth, intUnitPRHeight);

                        if (m_smVisionInfo.g_arrMarkGaugeM4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrMarkGaugeM4L[intUnitNo].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0], (int)fUnitPRResultCenterX, (int)fUnitPRResultCenterY, intUnitPRWidth, intUnitPRHeight);

                        if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrPackageGaugeM4L[intUnitNo].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0], (int)fUnitPRResultCenterX, (int)fUnitPRResultCenterY, intUnitPRWidth, intUnitPRHeight);

                        if (m_smVisionInfo.g_arrPackageGauge2M4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrPackageGauge2M4L[intUnitNo].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0], (int)fUnitPRResultCenterX, (int)fUnitPRResultCenterY, intUnitPRWidth, intUnitPRHeight);
                    }
                    else
                    {
                        //2019-10-14 ZJYEOH : If matching fail, load gauge using template unit position center 

                        if (m_smVisionInfo.g_arrOrientGaugeM4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0],
                                (int)m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_f4LGaugeCenterPointX, (int)m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_f4LGaugeCenterPointY,
                                (int)m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_f4LGaugeUnitWidth, (int)m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_f4LGaugeUnitHeight);

                        if (m_smVisionInfo.g_arrMarkGaugeM4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrMarkGaugeM4L[intUnitNo].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0],
                                (int)m_smVisionInfo.g_arrMarkGaugeM4L[intUnitNo].ref_f4LGaugeCenterPointX, (int)m_smVisionInfo.g_arrMarkGaugeM4L[intUnitNo].ref_f4LGaugeCenterPointY,
                                (int)m_smVisionInfo.g_arrMarkGaugeM4L[intUnitNo].ref_f4LGaugeUnitWidth, (int)m_smVisionInfo.g_arrMarkGaugeM4L[intUnitNo].ref_f4LGaugeUnitHeight);

                        if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrPackageGaugeM4L[intUnitNo].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0],
                                (int)m_smVisionInfo.g_arrPackageGaugeM4L[intUnitNo].ref_f4LGaugeCenterPointX, (int)m_smVisionInfo.g_arrPackageGaugeM4L[intUnitNo].ref_f4LGaugeCenterPointY,
                                (int)m_smVisionInfo.g_arrPackageGaugeM4L[intUnitNo].ref_f4LGaugeUnitWidth, (int)m_smVisionInfo.g_arrPackageGaugeM4L[intUnitNo].ref_f4LGaugeUnitHeight);

                        if (m_smVisionInfo.g_arrPackageGauge2M4L.Count > intUnitNo)
                            m_smVisionInfo.g_arrPackageGauge2M4L[intUnitNo].SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0],
                                (int)m_smVisionInfo.g_arrPackageGauge2M4L[intUnitNo].ref_f4LGaugeCenterPointX, (int)m_smVisionInfo.g_arrPackageGauge2M4L[intUnitNo].ref_f4LGaugeCenterPointY,
                                (int)m_smVisionInfo.g_arrPackageGauge2M4L[intUnitNo].ref_f4LGaugeUnitWidth, (int)m_smVisionInfo.g_arrPackageGauge2M4L[intUnitNo].ref_f4LGaugeUnitHeight);

                    }
                }
               
                // Add gain value to image and attached all position ROI to gain image.
                float fGaugeAngle;
                bool blnGaugeResult = false;
                //if (m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fGainValue != 1000)
                //{
                //    m_smVisionInfo.g_arrImages[0].AddGain(ref m_objOrientGainImage, m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fGainValue / 1000);
                //    m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].SetGaugePlace_BasedOnEdgeROI();
                //    blnGaugeResult = m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].Measure_WithDontCareArea(m_objOrientGainImage, m_smVisionInfo.g_objWhiteImage);
                //    fGaugeAngle = m_fOrientGauge = m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fRectAngle;
                //}
                //else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].SetGaugePlace_BasedOnEdgeROI();
                    blnGaugeResult = m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage);
                    fGaugeAngle = m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_fRectAngle;
                }
                if (!blnGaugeResult)
                {
                    m_smVisionInfo.g_strErrorMessage = "*Orient : " + m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_strErrorMessage;
                    m_smVisionInfo.g_strErrorMessage += "*Please relearn gauge.";
                    return false;
                }
                // RotateROI has same center point with gauge measure center point.
                ROI objRotateROI = new ROI();
                objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                objRotateROI.LoadROISetting(
                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_pRectCenterPoint.X -
                    m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo].ref_pRectCenterPoint.Y -
                    m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                    m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth,
                    m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight);

              
                        // Rotate from main image if unit 1 no tested (scenario == 0x02)
                        ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, fGaugeAngle, 0, ref m_smVisionInfo.g_arrRotatedImages, 0);
                   
                objRotateROI.Dispose();
            }
            else // No rect gauge
            {
                // If Lead Unit, use unit lead pattern to find unit surface ROI
                if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)
                {
                    m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0]);
                    fUnitPRResultCenterX = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX();
                    fUnitPRResultCenterY = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY();
                    fUnitPRResultAngle = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultAngle();
                    fUnitSurfaceOffsetX = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetX;
                    fUnitSurfaceOffsetY = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetY;

                    if (!Math2.GetNewXYAfterRotate(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalX + fUnitPRResultCenterX,
                                                  m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalY + fUnitPRResultCenterY,
                                                  fUnitSurfaceOffsetX,
                                                  fUnitSurfaceOffsetY,
                                                  fUnitPRResultAngle,
                                                  ref fUnitSurfaceOffsetX,
                                                  ref fUnitSurfaceOffsetY))
                    { }

                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                    objRotateROI.LoadROISetting(
                        (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalX +
                        fUnitPRResultCenterX -
                        m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth / 2 +
                        fUnitSurfaceOffsetX, 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalY +
                        fUnitPRResultCenterY -
                        m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight / 2 +
                        fUnitSurfaceOffsetY, 0, MidpointRounding.AwayFromZero),
                        m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth,
                        m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight);


                    // Rotate unit to exact 0 degree (m_fOrientGauge used in Package)
                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, fUnitPRResultAngle, 0, ref m_smVisionInfo.g_arrRotatedImages, 0); // Clear image is not so important in Orient Matching. Use interpolation 0 to save rotation time.

                    objRotateROI.Dispose();
                }
                else // if not lead unit, mean it is QFN. There is no way to find unit surface ROI without gauge tool.
                {
                    m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
                    
                }
            }

            int intMatchCount = 0;
            bool blnRecipeCorrupted = false;
            m_smVisionInfo.g_intOrientResult[intUnitNo] = -1;  // 0:0deg, 1:90deg, 2:180deg, 3:-90, 4:Fail
            
            // Single template test
            if (!m_smVisionInfo.g_blnInspectAllTemplate)
            {
                m_smVisionInfo.g_intOrientResult[intUnitNo] = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplate(
                    m_smVisionInfo.g_arrOrientROIs[intUnitNo][0], m_smVisionInfo.g_arrRotatedImages[0], true);
                m_smVisionInfo.g_intSelectedOcv[intUnitNo] = m_smVisionInfo.g_intSelectedTemplate;
                m_smVisionInfo.g_fOrientScore[intUnitNo] = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetMinScore();
            }
            else // Whole active templates test
            {
                float fHighestScore = -1;

                do
                {
                    int intTemplateIndex = (int)((m_smVisionInfo.g_intTemplatePriority >> (0x04 * intMatchCount)) & 0x0F) - 1;
                    if (intTemplateIndex >= 0)
                    {
                        int intAngle;
                        
                        bool blnPreciseAngleResult = true;
                        if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                        {
                            if (((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || ((m_smCustomizeInfo.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                                (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            {
                                m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                                intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge,
                                m_smVisionInfo.g_arrMarks[0].GetMarkAngleTolerance(0, intTemplateIndex),
                                ref blnPreciseAngleResult);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                                
                                //intAngle = m_smVisionInfo.g_arrOrients[intUnitNo][intTemplateIndex].DoOrientationInspection(
                                //    m_smVisionInfo.g_arrOrientROIs[intUnitNo][0], 2, !m_smVisionInfo.g_blnWantGauge);   // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                            }
                            else
                            {
                                intAngle = m_smVisionInfo.g_arrOrients[intUnitNo][intTemplateIndex].MatchWithTemplate(
                                    m_smVisionInfo.g_arrOrientROIs[intUnitNo][0], m_smVisionInfo.g_arrRotatedImages[0], true, fHighestScore, false,
                                    m_smVisionInfo.g_blnWantSubROI && (m_smVisionInfo.g_arrOrientROIs[intUnitNo].Count > 2));
                            }

                            if (m_smVisionInfo.g_arrOrients[intUnitNo][intTemplateIndex].GetMinScore() > fHighestScore)
                            {
                                fHighestScore = m_smVisionInfo.g_fOrientScore[intUnitNo] = m_smVisionInfo.g_arrOrients[intUnitNo][intTemplateIndex].GetMinScore();
                                m_smVisionInfo.g_intSelectedOcv[intUnitNo] = intTemplateIndex;
                                m_smVisionInfo.g_intOrientResult[intUnitNo] = intAngle;

                            }
                        }
                    }
                    else
                        blnRecipeCorrupted = true;
                    intMatchCount++;
                    //} while ((fHighestScore < 0.8) && (intMatchCount < m_smVisionInfo.g_arrOrients[intUnitNo].Count));
                } while (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count);
            }

            Orient objOrient = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedOcv[intUnitNo]];

            m_smVisionInfo.g_fOrientCenterX[intUnitNo] = objOrient.ref_fObjectX;
            m_smVisionInfo.g_fOrientCenterY[intUnitNo] = objOrient.ref_fObjectY;
            m_smVisionInfo.g_fOrientAngle[intUnitNo] = objOrient.ref_fDegAngleResult;
            m_smVisionInfo.g_fOrientScore[intUnitNo] = objOrient.GetMinScore();
            //m_smVisionInfo.g_blnViewOrientObject = true;
            
                // 2020 05 09 - CCENG : let orientation result always 0
                m_smVisionInfo.g_intOrientResult[intUnitNo] = 0;

            if ((m_blnWantOCVMark) || ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
            {
                if (m_smVisionInfo.g_intOrientResult[intUnitNo] < 4)
                {
                    m_intCurrentAngle = 0;
                    lbl_OrientationAngle.Text = "";
                    // Get OffSet between Orient PRS object center point and Unit Center Point 
                    int intOffSetX, intOffSetY;
                    switch (m_smVisionInfo.g_intOrientResult[intUnitNo])
                    {
                        default:
                        case 0:
                            lbl_OrientationAngle.Text = "0";
                            intOffSetX = m_smVisionInfo.g_arrOrients[intUnitNo][0].ref_intMatcherOffSetCenterX;
                            intOffSetY = m_smVisionInfo.g_arrOrients[intUnitNo][0].ref_intMatcherOffSetCenterY;
                            m_intCurrentAngle = 0;
                            break;
                        case 1:
                            lbl_OrientationAngle.Text = "90";
                            intOffSetX = m_smVisionInfo.g_arrOrients[intUnitNo][0].ref_intMatcherOffSetCenterY;
                            intOffSetY = -m_smVisionInfo.g_arrOrients[intUnitNo][0].ref_intMatcherOffSetCenterX;
                            m_intCurrentAngle = -90;
                            break;
                        case 2:
                            lbl_OrientationAngle.Text = "180";
                            intOffSetX = -m_smVisionInfo.g_arrOrients[intUnitNo][0].ref_intMatcherOffSetCenterX;
                            intOffSetY = -m_smVisionInfo.g_arrOrients[intUnitNo][0].ref_intMatcherOffSetCenterY;
                            m_intCurrentAngle = 180;
                            break;
                        case 3:
                            lbl_OrientationAngle.Text = "-90";
                            intOffSetX = -m_smVisionInfo.g_arrOrients[intUnitNo][0].ref_intMatcherOffSetCenterY;
                            intOffSetY = m_smVisionInfo.g_arrOrients[intUnitNo][0].ref_intMatcherOffSetCenterX;
                            m_intCurrentAngle = 90;
                            break;
                    }
                    lbl_OrientScore.Text = Math.Round(m_smVisionInfo.g_fOrientScore[0] * 100, 2).ToString();
                    // Get image index for measurement unit size
                    int intUnitEdgeImageIndex = 0;  //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);

                    ROI objRotatedROI = new ROI();
                    float fTotalRotateAngle = 0;
                    if (m_smVisionInfo.g_blnWantGauge)
                    {
                        RectGaugeM4L objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[intUnitNo];

                        // Get Orient Center Point (Final result for next MarkTest and PackageTest)
                        m_smVisionInfo.g_fUnitCenterX[intUnitNo] = objGauge.ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalX;
                        m_smVisionInfo.g_fUnitCenterY[intUnitNo] = objGauge.ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalY;

                        // Calculate total angle 
                        fTotalRotateAngle = m_intCurrentAngle + objGauge.ref_fRectAngle;

                        // Get RotateROI where the ROI center point == Unit Center Point
                        objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                        float fSizeX, fSizeY;
                        if ((objGauge.ref_fRectWidth + m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth) > m_smVisionInfo.g_arrImages[0].ref_intImageWidth ||
                            (objGauge.ref_fRectHeight + m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight) > m_smVisionInfo.g_arrImages[0].ref_intImageHeight)
                        {
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight % 2;
                        }
                        else
                        {
                            fSizeX = objGauge.ref_fRectWidth + m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = objGauge.ref_fRectHeight + m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight % 2;
                        }

                        objRotatedROI.LoadROISetting((int)Math.Round(objGauge.ref_pRectCenterPoint.X - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(objGauge.ref_pRectCenterPoint.Y - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));
                    }
                    else
                    {
                        if (m_smVisionInfo.g_arrOrientROIs[intUnitNo].Count > 3)    // Mean Unit Surface ROI exist
                        {
                            // Get Unit Surface Center Point (Final result for next MarkTest and PackageTest)
                            m_smVisionInfo.g_fUnitCenterX[intUnitNo] = fUnitPRResultCenterX + fUnitSurfaceOffsetX;
                            m_smVisionInfo.g_fUnitCenterY[intUnitNo] = fUnitPRResultCenterY + fUnitSurfaceOffsetY;

                            // Calculate total angle 
                            fTotalRotateAngle = m_intCurrentAngle + fUnitPRResultAngle;

                            // Get RotateROI where the ROI center point == Unit Center Point
                            objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                            float fSizeX, fSizeY;
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight % 2;

                            objRotatedROI.LoadROISetting(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalX +
                                                            (int)Math.Round(m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX() -
                                                            fSizeX / 2 +
                                                            m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetX, 0, MidpointRounding.AwayFromZero),
                                                         m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalY +
                                                            (int)Math.Round(m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY() -
                                                            fSizeY / 2 +
                                                            m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetY, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));
                        }
                        else
                        {

                            // Calculate total angle 
                            fTotalRotateAngle = m_intCurrentAngle + m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedOcv[intUnitNo]].ref_fDegAngleResult;

                            // Get RotateROI where the ROI center point == Unit Center Point
                            objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                            float fSizeX, fSizeY;
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight % 2;

                            
                                // 2019 09 06 - CCENG: If no package size center point, then use Orient Search ROI Center Point
                                objRotatedROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalCenterX - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                             (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalCenterY - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                             (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                             (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));
                            
                        }
                    }

                    // Start Rotate image 1, 2 and 3 to zero orientation and zero angle degree.
                    
                       
                            // Rotate from main image if unit 1 no tested (scenario == 0x02)
                            //ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);
                            
                                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

                    m_blnRotated = true;

                    // 2020 04 19 - cceng: Rotate image to mark 0 deg
                    ROI.Rotate0Degree_Better(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, 4, ref m_smVisionInfo.g_objMarkImage);


                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (m_smVisionInfo.g_arrImages.Count > m_smVisionInfo.g_arrLead[0].ref_intImageViewNo)
                        {
                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo], objRotatedROI, fTotalRotateAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrLead[0].ref_intImageViewNo);

                        }
                    }
                        
                    objRotatedROI.Dispose();
                    
                }
                else
                {
                    m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_objMarkImage);
                }
            }
            else
            {
                m_smVisionInfo.g_fOrientCenterX[intUnitNo] = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedOcv[intUnitNo]].ref_fObjectX;
                m_smVisionInfo.g_fOrientCenterY[intUnitNo] = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedOcv[intUnitNo]].ref_fObjectY;
            }

            if (m_smVisionInfo.g_intOrientResult[0] == 4)
            {
                lbl_OrientationAngle.Text = "Fail";
                lbl_OrientScore.Text = Math.Round(m_smVisionInfo.g_fOrientScore[0] * 100, 2).ToString();

                if (m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage == "" && m_smVisionInfo.g_strErrorMessage == "")
                {
                    m_smVisionInfo.g_strErrorMessage = "*Recipe is corrupted. Please relearn.";
                }
                else
                {
                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage;
                }

                return false;
            }
            else
            {
                //float Angle = Convert.ToInt32(lbl_OrientationAngle.Text) + m_smVisionInfo.g_fOrientAngle[0]; // this formula is based on clockwise rotation so the angle need to be inverted, 
                //Angle = -Angle;
                //float fXAfterRotated = (float)((m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX) + ((m_smVisionInfo.g_fOrientCenterX[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX)) * Math.Cos(Angle * Math.PI / 180)) - ((m_smVisionInfo.g_fOrientCenterY[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY))) * Math.Sin(Angle * Math.PI / 180));

                //float fYAfterRotated = (float)((m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY) + ((m_smVisionInfo.g_fOrientCenterX[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX)) * Math.Sin(Angle * Math.PI / 180)) + ((m_smVisionInfo.g_fOrientCenterY[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY))) * Math.Cos(Angle * Math.PI / 180));
                if (m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0]))
                {
                 
                    fUnitPRResultCenterX = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX();
                    fUnitPRResultCenterY = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY();
                    int intUnitPRWidth = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRWidth();
                    int intUnitPRHeight = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRHeight();

                    m_smVisionInfo.g_arrOrientROIs[0][2].LoadROISetting((int)(fUnitPRResultCenterX - intUnitPRWidth / 2),
                      (int)(fUnitPRResultCenterY - intUnitPRHeight / 2),
                       intUnitPRWidth,
                        intUnitPRHeight);

                    m_smVisionInfo.g_arrOrientROIs[0][2].AttachImage(m_smVisionInfo.g_arrOrientROIs[0][0]);
                }
            }
            return true;
        }
        private bool StartUnitPRTest()
        {
            int intUnitNo = 0;
            m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
            }

            // make sure template learn
            if (m_smVisionInfo.g_arrOrients[intUnitNo].Count < 3)
            {
                m_smVisionInfo.g_strErrorMessage = "*Unit PR : No Template Found";
                m_smVisionInfo.g_strErrorMessage += "*Please relearn template using wizard.";
                return false;
            }

            if (m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0]))
            {
                ROI objRotatedROI = new ROI();
                objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                float fSizeX, fSizeY;
                fSizeX = m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIWidth % 2; // why %2? To get "even" number
                fSizeY = m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROIHeight % 2;

                // 2019 09 06 - CCENG: If no package size center point, then use Orient Search ROI Center Point
                objRotatedROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalCenterX - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                             (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[intUnitNo][0].ref_ROITotalCenterY - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                             (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                             (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotatedROI, m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultAngle(), ref m_smVisionInfo.g_arrRotatedImages, 0);

                float fUnitPRResultCenterX = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX();
                float fUnitPRResultCenterY = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY();
                int intUnitPRWidth = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRWidth();
                int intUnitPRHeight = m_smVisionInfo.g_arrOrients[intUnitNo][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRHeight();

                m_smVisionInfo.g_arrOrientROIs[0][2].LoadROISetting((int)(fUnitPRResultCenterX - intUnitPRWidth / 2),
                  (int)(fUnitPRResultCenterY - intUnitPRHeight / 2),
                   intUnitPRWidth,
                    intUnitPRHeight);

                m_smVisionInfo.g_arrOrientROIs[0][2].AttachImage(m_smVisionInfo.g_arrOrientROIs[0][0]);
            }
            else
            {
                m_smVisionInfo.g_arrOrientROIs[0][2].AttachImage(m_smVisionInfo.g_arrOrientROIs[0][0]);
            }

            return true;
        }
        private void LoadMarkTemplate()
        {
            string strFolderName = "Mark";
            string strFileName = "Template" + m_smVisionInfo.g_intSelectedGroup + "_";
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

            // 20220124 CHTan: Task 14
            if (m_smVisionInfo.g_intMaxMarkTemplate < 2)
                return;
            // 20220124 CHTan: End 

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

            // 20220124 CHTan: Task 14
            if (m_smVisionInfo.g_intMaxMarkTemplate < 3)
                return;
            // 20220124 CHTan: End 

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

            // 20220124 CHTan: Task 14
            if (m_smVisionInfo.g_intMaxMarkTemplate < 4)
                return;
            // 20220124 CHTan: End 

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
        private bool Pin1ROICorrectPosition(ref string strPosition)
        {
            switch (m_smVisionInfo.g_arrMarks[0].ref_intPin1PositionControl)
            {
                case 0:
                    return true;
                    break;
                case 1: //Top Left
                    strPosition = "Top Left";
                    if (((m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionX + m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIWidth) < (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIWidth / 2)) &&
                       ((m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionY + m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIHeight) < (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIHeight / 2)))
                        return true;
                    else
                        return false;
                    break;
                case 2: //Top
                    strPosition = "Top";
                    if ((m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionY + m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIHeight) < (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIHeight / 2))
                        return true;
                    else
                        return false;
                    break;
                case 3: //Top Right
                    strPosition = "Top Right";
                    if ((m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionX > (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIWidth / 2)) &&
                       ((m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionY + m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIHeight) < (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIHeight / 2)))
                        return true;
                    else
                        return false;
                    break;
                case 4: //Right
                    strPosition = "Right";
                    if (m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionX > (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIWidth / 2))
                        return true;
                    else
                        return false;
                    break;
                case 5: //Bottom Right
                    strPosition = "Bottom Right";
                    if ((m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionX > (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIWidth / 2)) &&
                       (m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionY > (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIHeight / 2)))
                        return true;
                    else
                        return false;
                    break;
                case 6: //Bottom
                    strPosition = "Bottom";
                    if (m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionY > (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIHeight / 2))
                        return true;
                    else
                        return false;
                    break;
                case 7: //Bottom Left
                    strPosition = "Bottom Left";
                    if (((m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionX + m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIWidth) < (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIWidth / 2)) &&
                       (m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionY > (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIHeight / 2)))
                        return true;
                    else
                        return false;
                    break;
                case 8: //Left
                    strPosition = "Left";
                    if ((m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIPositionX + m_smVisionInfo.g_arrPin1[0].ref_objPin1ROI.ref_ROIWidth) < (m_smVisionInfo.g_arrPin1[0].ref_objSearchROI.ref_ROIWidth / 2))
                        return true;
                    else
                        return false;
                    break;
            }
            return true;
        }
    }
}