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
using Microsoft.Win32;
namespace VisionProcessForm
{
    public partial class LearnSealForm : Form
    {

        #region Member Variables
        private bool[] m_blnLoadSealROI = new bool[2] { true, true };
        private bool m_blnLoadDistanceROI = true;
        private bool m_blnLoadCircleROI = true;
        private bool[] m_arrLoadOverHeatROI = new bool[5] { true, true, true, true, true };
        private bool m_blnLoadSealEdgeStraightnessROI = true;
        private bool m_blnLoadTestROI = true;
        private bool m_blnPatternPass = false;

        private int m_intLearnType = 0; // 0 = Learn All, 1 = Position Search ROI, 2 = Position Pattern ROI, 3 = Seal ROI, 4 = Seal Segmentation, 5 = Distance ROI, 6 = Circle Gauge ROI, 7 = Over Heat ROI, 8 = Test ROI
        private bool m_blnInitDone = false;
        private bool m_blnNextButton = false;
        private int m_intCurrentPreciseDeg = 0;
        private int m_intUserGroup;
        private int m_intDisplayStepNo = 1;
        private int m_intROIStartPointX = 0;
        private int m_intROIRealWidth = 0;
        private int m_intVisionType = 0; // 0 = learn all (except mark and empty pattern), 1 = learn empty pattern only, 2 = learn mark pattern only
        private bool m_intBackToStep1 = false;
        private int m_intOrientDirection = 4;
        private int m_intCurrentAngle = 0;
        private int m_intSelectedDontCareROIIndexPrev = 0;
        private bool m_blnUpdateImage = false;
        //private bool m_blnLearnPocketPattern = false;
        //private bool m_blnLearnMarkPattern = false;

        //seal template 
        private int m_intMarkSelectedIndex = 0;
        private int m_intTempMarkSelectedIndex = 0;
        private int m_intPocketSelectedIndex = 0;
        private int m_intTempPocketSelectedIndex = 0;
        private PictureBox[] pic_Template = new PictureBox[8];
        private Panel[] pic_Panel = new Panel[8];
        private PictureBox[] pic_Template2 = new PictureBox[4];
        private Panel[] pic_Panel2 = new Panel[4];
        private Bitmap[] bmp = new Bitmap[8];
        private Bitmap[] Oribmp = new Bitmap[8];

        // Gauge
        private int m_intGaugeFilter = 1;
        private int m_intGaugeMinArea = 0;
        private int m_intGaugeMinAmp = 0;
        private int m_intGaugeThickness = 13;
        private int m_intGaugeTransChoice = 0;
        private int m_intGaugeTransType = 0;
        private int m_intGaugeNumFilteringPass = 0;
        private int m_intGaugeThreshold = 20;
        private float m_fGaugeFilteringThreshold = 3f;

        private string m_strSelectedRecipe;

        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private DataSet m_dsPackage = new DataSet();

        private AdvancedPostSealLGaugeForm m_objAdvancedForm;

        private AdvancedSealCircleGaugeForm m_objAdvancedCircleGaugeForm;
        private Graphics m_Graphic;
        private ImageDrawing m_objThresholdImage;
        private ROI m_objROI;
        #endregion

        public LearnSealForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intVisionType, int intLearnType)
        {
            InitializeComponent();

            m_intLearnType = intLearnType;
            m_intVisionType = intVisionType;
            m_intUserGroup = intUserGroup;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            m_Graphic = Graphics.FromHwnd(pic_ThresholdImage.Handle);

            if (m_intVisionType == 0)
            {
                //m_smVisionInfo.g_intLearnStepNo = 0;
                m_smVisionInfo.g_intLearnStepNo = 1;
            }
            else if (m_intVisionType == 1)  // Learn Empty Pocket
            {
                m_smVisionInfo.g_intLearnStepNo = 13;
                m_smVisionInfo.g_intSelectedImage = 1;  // 2020 06 22 - CCENG: Auto use image 2
            }
            else if (m_intVisionType == 2)  // Learn Mark
            {
                m_smVisionInfo.g_intLearnStepNo = 14;
                m_smVisionInfo.g_intSelectedImage = 1;  // 2020 06 22 - CCENG: Auto use image 2
            }

            if (m_intLearnType == 0)
                m_smVisionInfo.g_intLearnStepNo = 1;
            else if (m_intLearnType == 1)
                m_smVisionInfo.g_intLearnStepNo = 1;
            else if (m_intLearnType == 2)
                m_smVisionInfo.g_intLearnStepNo = 1;
            else if (m_intLearnType == 3)
                m_smVisionInfo.g_intLearnStepNo = 3;
            else if (m_intLearnType == 4)
                m_smVisionInfo.g_intLearnStepNo = 3; // Step 3 -> 4
            else if (m_intLearnType == 5)
                m_smVisionInfo.g_intLearnStepNo = 5;
            else if (m_intLearnType == 6)
                m_smVisionInfo.g_intLearnStepNo = 6;
            else if (m_intLearnType == 7)
                m_smVisionInfo.g_intLearnStepNo = 10;
            else if (m_intLearnType == 8)
                m_smVisionInfo.g_intLearnStepNo = 12;
            else if (m_intLearnType == 9)
                m_smVisionInfo.g_intLearnStepNo = 7;
            else if (m_intLearnType == 10)
                m_smVisionInfo.g_intLearnStepNo = 8;
            else if (m_intLearnType == 11)
                m_smVisionInfo.g_intLearnStepNo = 9;
            else if (m_intLearnType == 12)
                m_smVisionInfo.g_intLearnStepNo = 11;

            DisableField2();
            UpdateGUI();
            LoadGaugeGlobalSetting();
            m_blnInitDone = true;
        }



        /// <summary>
        ///  Setup each learning steps
        /// </summary>
        /// <returns>true = no error, false = error during learning step</returns>
        private bool SetupStep()
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {
                case 0:
                    lbl_TitleStep0.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step0;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_blnViewROI = false;
                    m_smVisionInfo.g_blnViewSealImage = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    btn_Previous.Enabled = false;
                    btn_Next.Enabled = true;
                    break;
                case 1:
                    lbl_TitleStep1.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step1;
                    pictureBox14.BringToFront();
                    if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch == 2)
                    {
                        pictureBox14.SendToBack();
                        pictureBox19.BringToFront();
                    }
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_intSelectedImage = 0;
                    m_smVisionInfo.g_objSeal.SetGrabImageIndex(0, 0);

                    AddPositionSearchROI();
                    AddTrainPositionROI();
                    AddVirtualPositionSearchROI();
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_blnViewSealImage = true;

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    btn_Previous.Enabled = false;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 1)
                    {
                        tp_Step1.Controls.Add(btn_Save);
                        btn_Next.Enabled = false;
                    }
                    if (m_intLearnType == 2)
                    {
                        btn_Next.PerformClick();
                    }
                    break;
                case 2:
                    //learn Position
                    lbl_TitleStep2.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step2;

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(0);
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(0, 0);

                    AddTrainPositionROI();
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 2)
                    {
                        tp_Step2.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
                case 3:
                    lbl_TitleStep3.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step3;

                    if (m_blnNextButton)
                    {
                        if (!CheckPositionSearchROI())
                            return false;


                        m_smVisionInfo.g_objSeal.CheckSprocketPitch(m_smVisionInfo.g_objSealImage, m_smVisionInfo.g_arrSealROIs[0][0], m_smVisionInfo.g_arrSealROIs[0][1], m_smVisionInfo.g_fCalibPixelX);

                    }

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(2);
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(1, 0);
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(2, 0);

                    AddSealROI();
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);

                    m_smVisionInfo.g_objSeal.BuildObjects(m_smVisionInfo.g_arrSealROIs);
                    lbl_FarArea.Text = (m_smVisionInfo.g_objSeal.GetSeal1Area() / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelX)).ToString("f3");
                    lbl_NearArea.Text = (m_smVisionInfo.g_objSeal.GetSeal2Area() / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelX)).ToString("f3");

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    //btn_Previous.Enabled = true;
                    if (m_intLearnType == 3)
                    {
                        tp_Step3.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    if (m_intLearnType == 4)
                    {
                        btn_Next.PerformClick();
                    }
                    break;

                case 4:
                    if (m_blnNextButton)
                        ReadjustSearchROIPosition();

                    lbl_TitleStep4.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step4;
                    lbl_ROISize.Text = "ROI Size : " + m_intROIRealWidth.ToString();
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(2);
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    AddGauge();
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewGauge = true;
                    if (m_intLearnType == 4)
                    {
                        tp_Step4.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
                case 5:
                    lbl_TitleStep5.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step5;

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(3);
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(3, 0);
                    m_smVisionInfo.g_intSelectedROI = 0;
                    AddDistanceROI();
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    if (m_intLearnType == 5)
                    {
                        tp_Step5.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
                case 6:
                    //Temporary removed 
                    //if (m_blnNextButton)
                    //{
                    //    if (!AdjustDistanceROIPosition())
                    //    {
                    //        SRMMessageBox.Show("Please ensure that the threshold value is correct.");
                    //        m_smVisionInfo.g_intLearnStepNo--;
                    //        break;
                    //    }
                    //}
                    lbl_TitleStep6.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step6;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(6, 0);

                    // Add Circle Gauge ROI and Circle Gauge
                    AddCircleGaugeROI();
                    SetCircleGaugePlace();

                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 6)
                    {
                        tp_Step6.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
                case 7:
                    lbl_TitleStep7.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step7;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    if (m_smVisionInfo.g_arrSealROIs[6].Count == 0)
                    {
                        // Add Circle Gauge ROI and Circle Gauge
                        AddCircleGaugeROI();
                        SetCircleGaugePlace();
                        m_smVisionInfo.g_objSealCircleGauges.Measure(m_smVisionInfo.g_objSealImage);
                    }

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
                   
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 9)
                    {
                        tp_Step7.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = true;
                    break;
                case 8:
                    lbl_TitleStep8.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step8;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    if (m_smVisionInfo.g_arrSealROIs[6].Count == 0)
                    {
                        // Add Circle Gauge ROI and Circle Gauge
                        AddCircleGaugeROI();
                        SetCircleGaugePlace();
                        m_smVisionInfo.g_objSealCircleGauges.Measure(m_smVisionInfo.g_objSealImage);
                    }

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);

                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 10)
                    {
                        tp_Step8.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = true;
                    break;
                case 9:
                    lbl_TitleStep9.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step9;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    if (m_smVisionInfo.g_arrSealROIs[6].Count == 0)
                    {
                        // Add Circle Gauge ROI and Circle Gauge
                        AddCircleGaugeROI();
                        SetCircleGaugePlace();
                        m_smVisionInfo.g_objSealCircleGauges.Measure(m_smVisionInfo.g_objSealImage);
                    }

                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);

                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 11)
                    {
                        tp_Step9.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = true;
                    break;
                case 10:
                    lbl_TitleStep10.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step10;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(4);
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(4, 1);

                    // Add Over heat ROI
                    AddOverHeadROI();
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 7)
                    {
                        tp_Step10.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
                case 11:
                    lbl_TitleStep11.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step11;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(7);

                    AddSealEdgeStraightnessROI();
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    btn_Next.Enabled = true;
                    if (m_intLearnType == 12)
                    {
                        tp_Step11.Controls.Add(btn_Save);
                        btn_Previous.Enabled = false;
                        btn_Next.Enabled = false;
                    }
                    break;
                case 12:
                    //Define search ROI
                    lbl_TitleStep12.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step12;
                    m_intCurrentAngle = 0; // Reset angle for image rotation
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(5);
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(5, 1);
                    m_smVisionInfo.g_intSelectedROI = 0;
                    //Add search ROI
                    AddPatternSearchROI();
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);

                    m_smVisionInfo.g_blnViewSealImage = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
                    //m_smVisionInfo.g_blnUpdateZoom = true;

                    btn_Next.Enabled = false;

                    //if (m_blnLearnPocketPattern || m_blnLearnMarkPattern)
                    //{
                    //    btn_Next.Enabled = true;
                    //    btn_Previous.Enabled = false;
                    //}
                    if (m_intLearnType == 8)
                    {
                        btn_Previous.Enabled = false;
                    }
                    break;
                case 13:
                    //learn empty pocket
                    lbl_TitleStep13.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step13;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(5);
                    //m_smVisionInfo.g_intSelectedImage = 1;
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(5, 1);

                    AddPatternSearchROI();
                    AddTrainPocketROI();
                    LoadSealPocketTemplate();
                    if (!backgroundWorker2.IsBusy)
                        backgroundWorker2.RunWorkerAsync();

                    //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnUpdateZoom = true;

                    if (m_smVisionInfo.g_objSeal.ref_blnWantDontCareArea)
                    {
                        btn_Next.Visible = true;
                        btn_Previous.Visible = true;
                        btn_Next.Enabled = true;
                        btn_Previous.Enabled = false;
                        tp_StepEmptyDontCare.Controls.Add(Btn_SaveEmptyPocket);
                    }
                    else
                    {
                        btn_Next.Visible = false;
                        btn_Previous.Visible = false;
                    }
                    //if (m_blnLearnPocketPattern)
                    //{
                    //    Btn_SaveEmptyPocket.Visible = true;
                    //    chk_SkipPocket.Visible = false;
                    //    btn_Next.Enabled = false;
                    //}
                    //else
                    //{
                    //    Btn_SaveEmptyPocket.Visible = false;
                    //    chk_SkipPocket.Visible = true;
                    //    btn_Next.Enabled = true;
                    //}
                    break;
                case 14:
                    //learn marking
                    lbl_TitleStep14.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step14;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(5);
                    //m_smVisionInfo.g_intSelectedImage = 1;
                    //m_smVisionInfo.g_objSeal.SetGrabImageIndex(5, 1);

                    AddPatternSearchROI();
                    AddTrainMarkROI();
                    LoadSealMarkTemplate();
                    if (!backgroundWorker1.IsBusy)
                        backgroundWorker1.RunWorkerAsync();

                    //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);

                    //2021-08-05 ZJYEOH : Only set position if ref_pResultMarkCenterPoint != 0
                    if (m_smVisionInfo.g_objSeal.ref_pResultMarkCenterPoint.X != 0 && m_smVisionInfo.g_objSeal.ref_pResultMarkCenterPoint.Y != 0)
                    {
                        // 2020 10 14 - CCENG: let the Seal Mark ROI draw same as result when user enter Learn Seal Mark Form.
                        m_smVisionInfo.g_arrSealROIs[5][2].SetPosition_Center(m_smVisionInfo.g_objSeal.ref_pResultMarkCenterPoint.X - m_smVisionInfo.g_arrSealROIs[5][0].ref_ROITotalX,
                                                                              m_smVisionInfo.g_objSeal.ref_pResultMarkCenterPoint.Y - m_smVisionInfo.g_arrSealROIs[5][0].ref_ROITotalY);
                    }

                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewGauge = false;
                    m_smVisionInfo.g_blnUpdateZoom = true;
                    btn_Next.Visible = false;
                    btn_Previous.Visible = false;

                    //if (m_blnLearnMarkPattern)
                    //{
                    //    Btn_SaveMark.Visible = true;
                    //    chk_SkipMark.Visible = false;
                    //    btn_Save.Visible = false;
                    //}
                    //else
                    //{
                    //    Btn_SaveMark.Visible = false;
                    //    chk_SkipMark.Visible = true;
                    //    btn_Save.Visible = true;
                    //}
                    break;
                case 15:
                    lbl_DefineDontCareArea.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_StepEmptyDontCare;
                    m_smVisionInfo.g_intSelectedROI = 0;
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(5);
                    // ----Load Dont Care ROI according to template -----------------------------------------------------------------------------
                    string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
                    ROI.LoadFile(strFolderPath + "Seal\\Template\\Pocket\\DontCareROI.xml",
                                 "Template" + m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex.ToString(),
                                 m_smVisionInfo.g_arrSealDontCareROIs);
                    Polygon.LoadPolygon(strFolderPath + "Seal\\Template\\Pocket\\Polygon.xml",
                                        "Template" + m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex.ToString(),
                                        m_smVisionInfo.g_arrPolygon_Seal);
                    for (int i = 0; i < m_smVisionInfo.g_arrSealDontCareROIs.Count; i++)
                    {
                        m_smVisionInfo.g_arrSealDontCareROIs[i].AttachImage(m_smVisionInfo.g_arrSealROIs[5][1]);    //g_arrSealROIs[5][1] is empty pocket ROI
                    }


                    btn_Next.Visible = true;
                    btn_Previous.Visible = true;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            return true;
        }



        /// <summary>
        /// Add distance ROI
        /// </summary>
        private void AddDistanceROI()
        {
            ROI objROI;

            //if (m_smVisionInfo.g_arrSealROIs.Count < 4)
            //    m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            for (int i = m_smVisionInfo.g_arrSealROIs.Count; i < 4; i++)
            {
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());
            }

            if (m_smVisionInfo.g_arrSealROIs[3].Count == 0)
            {
                objROI = new ROI("Distance", 1);
                objROI.LoadROISetting(270, 10, 100, 50);
                m_smVisionInfo.g_arrSealROIs[3].Add(objROI);
            }
            else
            {
                m_smVisionInfo.g_arrSealROIs[3][0].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[3][0]).ref_ROIPositionX, ((ROI)m_smVisionInfo.g_arrSealROIs[3][0]).ref_ROIPositionY,
                    ((ROI)m_smVisionInfo.g_arrSealROIs[3][0]).ref_ROIWidth, ((ROI)m_smVisionInfo.g_arrSealROIs[3][0]).ref_ROIHeight);
            }

            m_smVisionInfo.g_arrSealROIs[3][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            //if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2 && m_blnLoadDistanceROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            if (m_blnLoadDistanceROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            {
                m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedX);
                m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                m_blnLoadDistanceROI = false;
            }

        }

        private void AddOverHeadROI()
        {
            ROI objROI;

            //if (m_smVisionInfo.g_arrSealROIs.Count < 5)
            //    m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            for (int i = m_smVisionInfo.g_arrSealROIs.Count; i < 5; i++)
            {
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());
            }

            if (m_smVisionInfo.g_arrSealROIs[4].Count == 0)
            {
                objROI = new ROI("OverHeat", 1);
                objROI.LoadROISetting(160, 130, 100, 200);
                m_smVisionInfo.g_arrSealROIs[4].Add(objROI);
            }
            else
            {
                m_smVisionInfo.g_arrSealROIs[4][0].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[4][0]).ref_ROIPositionX,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[4][0]).ref_ROIPositionY,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[4][0]).ref_ROIWidth,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[4][0]).ref_ROIHeight);
            }

            m_smVisionInfo.g_arrSealROIs[4][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            //if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2 && m_blnLoadOverHeatROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            if (m_arrLoadOverHeatROI[0] && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            {
                m_smVisionInfo.g_arrSealROIs[4][0].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[4][0].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedX);
                m_smVisionInfo.g_arrSealROIs[4][0].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[4][0].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                m_arrLoadOverHeatROI[0] = false;
            }

            for (int i = 1; i < m_smVisionInfo.g_arrSealROIs[4].Count; i++)
            {
                m_smVisionInfo.g_arrSealROIs[4][i].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[4][i]).ref_ROIPositionX,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[4][i]).ref_ROIPositionY,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[4][i]).ref_ROIWidth,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[4][i]).ref_ROIHeight);
               
                m_smVisionInfo.g_arrSealROIs[4][i].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

                //if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2 && m_blnLoadOverHeatROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
                if (m_arrLoadOverHeatROI[i] && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
                {
                    m_smVisionInfo.g_arrSealROIs[4][i].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[4][i].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedX);
                    m_smVisionInfo.g_arrSealROIs[4][i].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[4][i].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                    m_arrLoadOverHeatROI[i] = false;
                }

            }

            grpbox_OverHeat.Size = new Size(grpbox_OverHeat.Size.Width, 15 + (m_smVisionInfo.g_arrSealROIs[4].Count * 35));

            if (m_smVisionInfo.g_arrSealROIs.Count <= 4 || m_smVisionInfo.g_arrSealROIs[4].Count == 1)
            {
                chk_SetToAllOverHeatROI.Visible = false;
                chk_SetToAllOverHeatROI.Checked = false;
            }
        }
        private void AddOverHeadROI(int intROIIndex)
        {
            if (intROIIndex > 4)
                return;

            ROI objROI;

            //if (m_smVisionInfo.g_arrSealROIs.Count < 5)
            //    m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            for (int i = m_smVisionInfo.g_arrSealROIs.Count; i < 5; i++)
            {
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());
            }

            for (int i = m_smVisionInfo.g_arrSealROIs[4].Count; i < intROIIndex + 1; i++)
            {
                objROI = new ROI("OverHeat " + (i + 1).ToString(), 1);
                objROI.LoadROISetting(100 + (i * 20), 130, 100, 200);
                m_smVisionInfo.g_arrSealROIs[4].Add(objROI);
                m_arrLoadOverHeatROI[intROIIndex] = false;
                m_smVisionInfo.g_objSeal.SetOverHeatLowThreshold(i, m_smVisionInfo.g_objSeal.GetOverHeatLowThreshold(i - 1));
                m_smVisionInfo.g_objSeal.SetOverHeatHighThreshold(i, m_smVisionInfo.g_objSeal.GetOverHeatHighThreshold(i - 1));
                m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(i, m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(i - 1));
                m_smVisionInfo.g_objSeal.SetOverHeatMinArea(i, m_smVisionInfo.g_objSeal.GetOverHeatMinArea(i - 1));
                m_smVisionInfo.g_objSeal.SetScratchesLowThreshold(i, m_smVisionInfo.g_objSeal.GetScratchesLowThreshold(i - 1));
                m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(i, m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(i - 1));
                m_smVisionInfo.g_objSeal.SetScratchesMinArea(i, m_smVisionInfo.g_objSeal.GetScratchesMinArea(i - 1));
            }

            m_smVisionInfo.g_arrSealROIs[4][intROIIndex].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[4][intROIIndex]).ref_ROIPositionX,
                                                             ((ROI)m_smVisionInfo.g_arrSealROIs[4][intROIIndex]).ref_ROIPositionY,
                                                            ((ROI)m_smVisionInfo.g_arrSealROIs[4][intROIIndex]).ref_ROIWidth,
                                                            ((ROI)m_smVisionInfo.g_arrSealROIs[4][intROIIndex]).ref_ROIHeight);
            
            m_smVisionInfo.g_arrSealROIs[4][intROIIndex].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            //if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2 && m_blnLoadOverHeatROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            if (m_arrLoadOverHeatROI[intROIIndex] && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            {
                m_smVisionInfo.g_arrSealROIs[4][intROIIndex].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[4][intROIIndex].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedX);
                m_smVisionInfo.g_arrSealROIs[4][intROIIndex].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[4][intROIIndex].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                m_arrLoadOverHeatROI[intROIIndex] = false;
            }

            grpbox_OverHeat.Size = new Size(grpbox_OverHeat.Size.Width, 15 + (m_smVisionInfo.g_arrSealROIs[4].Count * 35));

            if (m_smVisionInfo.g_arrSealROIs[4].Count > 1)
            {
                chk_SetToAllOverHeatROI.Visible = true;
            }
        }
        private void AddSealEdgeStraightnessROI()
        {
            ROI objROI;

            for (int i = m_smVisionInfo.g_arrSealROIs.Count; i < 8; i++)
            {
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());
            }

            if (m_smVisionInfo.g_arrSealROIs[7].Count == 0)
            {
                objROI = new ROI("Seal Edge Straightness", 1);
                if (m_smVisionInfo.g_arrSealROIs.Count > 2)
                {
                    objROI.LoadROISetting(m_smVisionInfo.g_arrSealROIs[2][0].ref_ROIPositionX, m_smVisionInfo.g_arrSealROIs[2][0].ref_ROIPositionY + m_smVisionInfo.g_arrSealROIs[2][0].ref_ROIHeight + 10, 100, 200);
                }
                else
                    objROI.LoadROISetting(160, 130, 100, 200);
                m_smVisionInfo.g_arrSealROIs[7].Add(objROI);
            }
            else
            {
                m_smVisionInfo.g_arrSealROIs[7][0].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[7][0]).ref_ROIPositionX,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[7][0]).ref_ROIPositionY,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[7][0]).ref_ROIWidth,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[7][0]).ref_ROIHeight);
            }

            m_smVisionInfo.g_arrSealROIs[7][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            //if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2 && m_blnLoadSealEdgeStraightnessROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            if (m_blnLoadSealEdgeStraightnessROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            {
                m_smVisionInfo.g_arrSealROIs[7][0].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[7][0].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedX);
                m_smVisionInfo.g_arrSealROIs[7][0].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[7][0].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                m_blnLoadSealEdgeStraightnessROI = false;
            }
        }
        private void AddCircleGaugeROI()
        {
            ROI objROI;

            for (int i = m_smVisionInfo.g_arrSealROIs.Count; i < 7; i++)
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            if (m_smVisionInfo.g_arrSealROIs[6].Count == 0)
            {
                objROI = new ROI("Circle Gauge", 1);
                objROI.LoadROISetting(160, 130, 100, 100);
                m_smVisionInfo.g_arrSealROIs[6].Add(objROI);
            }
            else
            {
                m_smVisionInfo.g_arrSealROIs[6][0].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[6][0]).ref_ROIPositionX,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[6][0]).ref_ROIPositionY,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[6][0]).ref_ROIWidth,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[6][0]).ref_ROIHeight);
            }

            m_smVisionInfo.g_arrSealROIs[6][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2 && m_blnLoadCircleROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            {
                m_smVisionInfo.g_arrSealROIs[6][0].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[6][0].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedX);
                m_smVisionInfo.g_arrSealROIs[6][0].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[6][0].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                m_blnLoadCircleROI = false;
            }
            else if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch == 2 && m_blnLoadCircleROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            {
                m_smVisionInfo.g_arrSealROIs[6][0].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[6][0].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fCircleGaugeShiftX);
                m_smVisionInfo.g_arrSealROIs[6][0].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[6][0].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                m_blnLoadCircleROI = false;
            }
        }

        private void AddPatternSearchROI()
        {
            ROI objROI;

            //if (m_smVisionInfo.g_arrSealROIs.Count < 6)
            //    m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            for (int i = m_smVisionInfo.g_arrSealROIs.Count; i < 6; i++)
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            for (int i = 0; i < m_smVisionInfo.g_arrSealROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrSealROIs[i].Count == 0)
                    m_smVisionInfo.g_arrSealROIs[i].Add(new ROI());
            }

            if (m_smVisionInfo.g_arrSealROIs[5].Count == 0)
            {
                objROI = new ROI("Pattern Test ROI", 1);
                objROI.LoadROISetting(200, 130, 200, 200);
                m_smVisionInfo.g_arrSealROIs[5].Add(objROI);
            }
            else
            {
                m_smVisionInfo.g_arrSealROIs[5][0].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIPositionX,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIPositionY,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight);
            }

            m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            //if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2 && m_blnLoadTestROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            if (m_blnLoadTestROI && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
            {
                m_smVisionInfo.g_arrSealROIs[5][0].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[5][0].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedX);
                m_smVisionInfo.g_arrSealROIs[5][0].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[5][0].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                m_blnLoadTestROI = false;
            }
        }

        private void AddTrainPositionROI()
        {
            ROI objROI;

            if (m_smVisionInfo.g_arrSealROIs.Count == 0)
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            if (m_smVisionInfo.g_arrSealROIs[0].Count == 1)
            {
                objROI = new ROI("Train Position ROI", 1);
                //objROI.LoadROISetting(200, 130, 100, 100);
                objROI.LoadROISetting(m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionX + m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth / 4, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionY + m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIHeight / 4, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth / 2, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIHeight / 2);
                m_smVisionInfo.g_arrSealROIs[0].Add(objROI);
                m_smVisionInfo.g_arrSealROIs[0][1].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }
            else if (m_smVisionInfo.g_arrSealROIs[0].Count > 1)
            {
                if ((m_smVisionInfo.g_arrSealROIs[0][1].ref_ROIPositionX < m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionX) ||
                     (m_smVisionInfo.g_arrSealROIs[0][1].ref_ROIPositionY < m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionY) ||
                     ((m_smVisionInfo.g_arrSealROIs[0][1].ref_ROIPositionX + m_smVisionInfo.g_arrSealROIs[0][1].ref_ROIWidth) > (m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionX + m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth)) ||
                     ((m_smVisionInfo.g_arrSealROIs[0][1].ref_ROIPositionY + m_smVisionInfo.g_arrSealROIs[0][1].ref_ROIHeight) > (m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionY + m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIHeight)) ||
                     (m_smVisionInfo.g_arrSealROIs[0][1].ref_ROIWidth > m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth) ||
                     (m_smVisionInfo.g_arrSealROIs[0][1].ref_ROIHeight > m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIHeight))
                    m_smVisionInfo.g_arrSealROIs[0][1].LoadROISetting(m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionX + m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth / 4, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionY + m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIHeight / 4, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth / 2, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIHeight / 2);
                else
                    m_smVisionInfo.g_arrSealROIs[0][1].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[0][1]).ref_ROIPositionX,
                                                     ((ROI)m_smVisionInfo.g_arrSealROIs[0][1]).ref_ROIPositionY,
                                                    ((ROI)m_smVisionInfo.g_arrSealROIs[0][1]).ref_ROIWidth,
                                                    ((ROI)m_smVisionInfo.g_arrSealROIs[0][1]).ref_ROIHeight);

                m_smVisionInfo.g_arrSealROIs[0][1].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

                if (m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt)
                {
                    // 2020 04 15 - Find sprocket hole that nearest to image center.
                    //if (m_smVisionInfo.g_objSeal.DoPositionInspection_AlwaysPass(m_smVisionInfo.g_arrSealROIs[0][0], 1, 0, m_smVisionInfo.g_fCalibPixelX, true))
                    if (m_smVisionInfo.g_objSeal.DoPositionInspection_FindSprocketHolePositionNearestToImageCenter(m_smVisionInfo.g_arrSealROIs[0][0], 1, 0, m_smVisionInfo.g_fCalibPixelX, true))
                    {
                        m_blnPatternPass = true;
                        // 2020 04 15 - CCENG: Display Position Pattern size
                        //m_smVisionInfo.g_arrSealROIs[0][1].LoadROISetting((int)(m_smVisionInfo.g_objSeal.ref_pResultPositionCenterPoint.X - (((ROI)m_smVisionInfo.g_arrSealROIs[0][1]).ref_ROIWidth / 2)),
                        //                              (int)(m_smVisionInfo.g_objSeal.ref_pResultPositionCenterPoint.Y - (((ROI)m_smVisionInfo.g_arrSealROIs[0][1]).ref_ROIHeight / 2)),
                        //                            ((ROI)m_smVisionInfo.g_arrSealROIs[0][1]).ref_ROIWidth,
                        //                            ((ROI)m_smVisionInfo.g_arrSealROIs[0][1]).ref_ROIHeight);
                        m_smVisionInfo.g_arrSealROIs[0][1].LoadROISetting((int)(m_smVisionInfo.g_objSeal.ref_pResultPositionCenterPoint.X - (m_smVisionInfo.g_objSeal.GetPositionMatcherWidth(0) / 2)),
                                                      (int)(m_smVisionInfo.g_objSeal.ref_pResultPositionCenterPoint.Y - (m_smVisionInfo.g_objSeal.GetPositionMatcherHeight(0) / 2)),
                                                    m_smVisionInfo.g_objSeal.GetPositionMatcherWidth(0),
                                                    m_smVisionInfo.g_objSeal.GetPositionMatcherHeight(0));
                    }
                }
            }
        }

        private void AddVirtualPositionSearchROI()
        {
            ROI objROI;

            if (m_smVisionInfo.g_arrSealROIs.Count == 0)
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            if (m_smVisionInfo.g_arrSealROIs[0].Count == 1)
            {
                objROI = new ROI("Train Position ROI", 1);
                objROI.LoadROISetting(200, 130, 100, 100);
                m_smVisionInfo.g_arrSealROIs[0].Add(objROI);
                m_smVisionInfo.g_arrSealROIs[0][1].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }

            if (m_smVisionInfo.g_arrSealROIs[0].Count == 2)
            {
                objROI = new ROI("Position Test ROI", 1);
                objROI.LoadROISetting((int)(m_smVisionInfo.g_arrSealROIs[0][0].ref_ROICenterX - (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch * m_smVisionInfo.g_fCalibPixelX) - (m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth / 2)), m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIHeight);
                m_smVisionInfo.g_arrSealROIs[0].Add(objROI);
                m_smVisionInfo.g_arrSealROIs[0][2].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }

            if (m_smVisionInfo.g_arrSealROIs[0].Count > 2)
            {
                m_smVisionInfo.g_arrSealROIs[0][2].LoadROISetting((int)(m_smVisionInfo.g_arrSealROIs[0][0].ref_ROICenterX - (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch * m_smVisionInfo.g_fCalibPixelX) - (m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth / 2)), m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIPositionY, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIHeight);
                m_smVisionInfo.g_arrSealROIs[0][2].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }
        }
        private void AddTrainPocketROI()
        {
            ROI objROI;

            if (m_smVisionInfo.g_arrSealROIs.Count < 6)
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            if (m_smVisionInfo.g_arrSealROIs[5].Count == 1)
            {
                objROI = new ROI("Train Pocket ROI", 1);
                objROI.AttachImage(m_smVisionInfo.g_arrSealROIs[5][0]);
                objROI.LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth / 4,
                                                        ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight / 4,
                                                       ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth / 2,
                                                       ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight / 2);
                m_smVisionInfo.g_arrSealROIs[5].Add(objROI);

                if (m_smVisionInfo.g_blnViewSealImage)
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_objSealImage);
                }
                else
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else if (m_smVisionInfo.g_arrSealROIs[5].Count >= 2)
            {
                m_smVisionInfo.g_arrSealROIs[5][1].AttachImage(m_smVisionInfo.g_arrSealROIs[5][0]);
                if (m_smVisionInfo.g_objSeal.GetPocketMatcherWidth(m_intPocketSelectedIndex) == 0 ||
                  m_smVisionInfo.g_objSeal.GetPocketMatcherHeight(m_intPocketSelectedIndex) == 0)
                {
                    m_smVisionInfo.g_arrSealROIs[5][1].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROIPositionX,
                                                                     ((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROIPositionY,
                                                                    ((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROIWidth,
                                                                    ((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROIHeight);
                }
                else
                {
                    int intWidth = m_smVisionInfo.g_objSeal.GetPocketMatcherWidth(m_intPocketSelectedIndex);
                    int intHeight = m_smVisionInfo.g_objSeal.GetPocketMatcherHeight(m_intPocketSelectedIndex);
                    m_smVisionInfo.g_arrSealROIs[5][1].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROICenterX - intWidth / 2,
                                                                     ((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROICenterY - intHeight / 2,
                                                                    intWidth,
                                                                    intHeight);

                }

                if (m_smVisionInfo.g_blnViewSealImage)
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_objSealImage);
                }
                else
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
        }

        private void AddTrainMarkROI()
        {
            ROI objROI;

            if (m_smVisionInfo.g_arrSealROIs.Count < 6)
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            if (m_smVisionInfo.g_arrSealROIs[5].Count == 1)
            {
                // If seal ROI equal 1, mean previously didnt not add Train Pocket ROI
                // Add train Pocket ROI here, and then only add train Mark ROI
                objROI = new ROI("Train Pocket ROI", 1);
                objROI.LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth / 4,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight / 4,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth / 2,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight / 2);
                m_smVisionInfo.g_arrSealROIs[5].Add(objROI);
                if (m_smVisionInfo.g_blnViewSealImage)
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_objSealImage);
                }
                else
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                }

                objROI = new ROI("Train Mark ROI", 1);
                objROI.AttachImage(m_smVisionInfo.g_arrSealROIs[5][0]);
                objROI.LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth / 4,
                                                            ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight / 4,
                                                           ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth / 2,
                                                           ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight / 2);
                m_smVisionInfo.g_arrSealROIs[5].Add(objROI);
                if (m_smVisionInfo.g_blnViewSealImage)
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_objSealImage);
                }
                else
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else if (m_smVisionInfo.g_arrSealROIs[5].Count == 2)
            {
                objROI = new ROI("Train Mark ROI", 1);
                objROI.AttachImage(m_smVisionInfo.g_arrSealROIs[5][0]);
                objROI.LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth / 4,
                                                       ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight / 4,
                                                      ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIWidth / 2,
                                                      ((ROI)m_smVisionInfo.g_arrSealROIs[5][0]).ref_ROIHeight / 2);
                m_smVisionInfo.g_arrSealROIs[5].Add(objROI);
                if (m_smVisionInfo.g_blnViewSealImage)
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_objSealImage);
                }
                else
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else if (m_smVisionInfo.g_arrSealROIs[5].Count == 3)
            {
                m_smVisionInfo.g_arrSealROIs[5][2].AttachImage(m_smVisionInfo.g_arrSealROIs[5][0]);
                m_smVisionInfo.g_arrSealROIs[5][2].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][2]).ref_ROIPositionX,
                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[5][2]).ref_ROIPositionY,
                                                ((ROI)m_smVisionInfo.g_arrSealROIs[5][2]).ref_ROIWidth,
                                                ((ROI)m_smVisionInfo.g_arrSealROIs[5][2]).ref_ROIHeight);
                if (m_smVisionInfo.g_blnViewSealImage)
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_objSealImage);
                }
                else
                {
                    m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
        }

        /// <summary>
        /// Put first line gauge of Seal from tape's inner.
        /// </summary>
        private void AddGauge()
        {
            int intSectionCount = Convert.ToInt32(txt_SegmentSections.Text);
            int intGaugeLength = (int)Math.Round(Convert.ToDouble(m_intROIRealWidth) / intSectionCount, 0, MidpointRounding.AwayFromZero);
            if (intGaugeLength <= Convert.ToInt32(txt_SamplingSteps.Text))
            {
                SRMMessageBox.Show("Sampling Steps value will be changed to " + (intGaugeLength - 1).ToString() + " to fit into section width size.");

                txt_SamplingSteps.Text = (intGaugeLength - 1).ToString();
                return;
            }
            else if (!btn_Save.Enabled)
                btn_Save.Enabled = true;

            int intAngle, intCount;
            // intCount = 1 is because the inner part is near to ROI Org Y value
            // intCount = 3 is because the inner part is far away from ROI Org Y value            

            // only record down inner seal gauge of each tape seal because outer line can be duplicate
            // check from inner seal to outer seal because it may got noise like double line (refer to image)

            int x = 0;
            int intSectionCount2 = 0;

            for (int i = 0; i < 2; i++) // 0=far seal; 1= near seal
            {
                if ((m_smVisionInfo.g_arrSealGauges.Count - 1) < i)
                    m_smVisionInfo.g_arrSealGauges.Add(new List<LGauge>());

                m_smVisionInfo.g_arrSealGauges[i].Clear();

                x = 0;
                intSectionCount2 = 0;

                while (x < m_smVisionInfo.g_arrSealROIs[i + 1][0].ref_ROIWidth - Convert.ToInt32(txt_SamplingSteps.Text))
                {
                    LGauge objGauge = new LGauge();
                    objGauge.SetGaugeAdvancedSetting(m_intGaugeMinAmp, m_intGaugeMinArea,
                        m_intGaugeFilter, m_intGaugeThickness, m_intGaugeTransChoice, m_intGaugeTransType,
                        m_intGaugeNumFilteringPass, m_intGaugeThreshold, m_fGaugeFilteringThreshold);

                    objGauge.ref_GaugeSamplingStep = float.Parse(txt_SamplingSteps.Text);

                    x += intGaugeLength;

                    if (i == 0)
                    {  //top to bottom
                        intAngle = 0;
                        intCount = 3;
                    }
                    else
                    {  //bottom to top
                        intAngle = 180;
                        intCount = 1;
                    }

                    objGauge.SetGaugePlacement(m_smVisionInfo.g_arrSealROIs[i + 1][0], intGaugeLength, (m_smVisionInfo.g_arrSealROIs[i + 1][0].ref_ROIPositionX + (intGaugeLength * intSectionCount2)), intCount);
                    objGauge.ref_GaugeAngle = intAngle;
                    m_smVisionInfo.g_arrSealGauges[i].Add(objGauge);

                    intSectionCount2++;
                }
            }

            if (m_smVisionInfo.g_objSeal.BuildGauge(m_smVisionInfo.g_arrSealROIs, m_smVisionInfo.g_arrSealGauges, m_smVisionInfo.g_fCalibPixelY, true))
            {
                for (int i = 0; i < 2; i++)
                {
                    m_smVisionInfo.g_objSeal.ref_fTemplateWidth[i] = m_smVisionInfo.g_objSeal.ref_fLineWidthAverage[i];
                }
            }
            else
            {
                SRMMessageBox.Show("Please ensure that you have valid image or settings");
                btn_Save.Enabled = false;
            }
        }

        /// <summary>
        /// Add 2 area of interest on image
        /// </summary>
        private void AddPositionSearchROI()
        {
            ROI objROI;

            if (m_smVisionInfo.g_arrSealROIs.Count == 0)
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

            if (m_smVisionInfo.g_arrSealROIs[0].Count == 0)
            {
                objROI = new ROI("Position Test ROI", 1);
                if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2)
                    objROI.LoadROISetting(200, 130, 200, 200);
                else
                    objROI.LoadROISetting(0, 130, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth, 200);   //2020-07-02 ZJYEOH : Only when TapePocketPitch = 2 will load image size width ROI
                m_smVisionInfo.g_arrSealROIs[0].Add(objROI);
            }
            else
            {
                // 2020 04 15 - CCENG: Position Search ROI X length will same as image size.
                //m_smVisionInfo.g_arrSealROIs[0][0].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIPositionX,
                //                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIPositionY,
                //                                                ((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIWidth,
                //                                                ((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIHeight);

                //2020-07-02 ZJYEOH : Only when TapePocketPitch = 2 will load image size width ROI
                if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch == 2)
                    m_smVisionInfo.g_arrSealROIs[0][0].LoadROISetting(0,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIPositionY,
                                                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIHeight);
                else
                    m_smVisionInfo.g_arrSealROIs[0][0].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIPositionX,
                                                                     ((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIPositionY,
                                                                    ((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIWidth,
                                                                    ((ROI)m_smVisionInfo.g_arrSealROIs[0][0]).ref_ROIHeight);
            }

            m_smVisionInfo.g_arrSealROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

        }


        /// <summary>
        /// Add 2 area of interest on image
        /// </summary>
        private void AddSealROI()
        {
            ROI objROI;

            for (int i = m_smVisionInfo.g_arrSealROIs.Count; i < 3; i++)
            {
                m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());
            }

            // Center Point Y of Upper Part of Image ; if pitch = 4mm, the seals' distance is around 4mm also
            for (int i = 1; i < 3; i++)
            {
                //if ((m_smVisionInfo.g_arrSealROIs.Count - 1) < i)
                //    m_smVisionInfo.g_arrSealROIs.Add(new List<ROI>());

                if (m_smVisionInfo.g_arrSealROIs[i].Count == 0)
                {
                    //float fTapePocketYInPixel = 50;// m_smVisionInfo.g_fCalibPixelY * Convert.ToInt32(cbo_intPocketPitch.SelectedItem.ToString()) / 2;
                    if (i == 1)
                    {
                        objROI = new ROI("Seal 1", 1);
                        //int intPositionY = 240 - (int)fTapePocketYInPixel - 25;  // Calculate start from center point; 240 = 480/2; 640 = image width
                        objROI.LoadROISetting(270, 190, 100, 50);
                    }
                    else
                    {
                        objROI = new ROI("Seal 2", 1);
                        //int intPositionY = 240 + (int)fTapePocketYInPixel - 25;    // 25 = default ROI width
                        objROI.LoadROISetting(270, 290, 100, 50);
                    }

                    m_smVisionInfo.g_arrSealROIs[i].Add(objROI);
                    m_smVisionInfo.g_arrSealROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

                }
                else
                {
                    //if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch != 2 && m_blnLoadSealROI[i - 1] && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
                    if (m_blnLoadSealROI[i - 1] && m_smVisionInfo.g_objSeal.ref_blnPositionPatternLearnt && m_blnPatternPass)
                    {
                        m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIOriPositionX + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedX);
                        m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIPositionY = m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIOriPositionY + Convert.ToInt32(m_smVisionInfo.g_objSeal.ref_fShiftedY);

                        m_blnLoadSealROI[i - 1] = false;
                    }

                    m_smVisionInfo.g_arrSealROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                }

            }
        }

        /// <summary>
        /// Modify distance ROI placement position
        /// </summary>
        private bool AdjustDistanceROIPosition()
        {
            m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIPositionX = m_smVisionInfo.g_arrSealROIs[1][0].ref_ROIPositionX;
            m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIWidth = m_smVisionInfo.g_arrSealROIs[1][0].ref_ROIWidth;
            //m_intROIRealWidth = m_smVisionInfo.g_arrSealROIs[0][0].ref_ROIWidth;

            float fSealTopPositionY = 0;
            m_smVisionInfo.g_objSeal.ref_fTemplateWidth[2] = m_smVisionInfo.g_objSeal.GetDistanceBtwSealAndBorder(m_smVisionInfo.g_arrSealROIs[3][0], ref fSealTopPositionY);

            if (m_smVisionInfo.g_objSeal.ref_fTemplateWidth[2] == 0)
                return false;

            m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIPositionY -= (int)((m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIHeight / 2) - fSealTopPositionY + (m_smVisionInfo.g_objSeal.ref_fTemplateWidth[2] / 2));

            m_smVisionInfo.g_arrSealROIs[3][0].LoadROISetting(m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIPositionX,
                         m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIPositionY,
                         m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[3][0].ref_ROIHeight);
            m_smVisionInfo.g_arrSealROIs[3][0].AttachImage(m_smVisionInfo.g_objSealImage);

            return true;
        }

        /// <summary>
        /// Customize GUI based on user access level
        /// </summary>
        private void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "";

            strChild2 = "Advance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_GaugeAdvanceSetting.Visible = false;
            }

            strChild2 = "Learn Template";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Previous.Enabled = false;
                btn_Next.Enabled = false;
            }

            strChild1 = "Learn Page";
            strChild2 = "Save Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;

            }

        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Learn Page";
            string strChild3 = "Save Button";

            if (m_intVisionType == 0) // seal
            {
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_Save.Enabled = false;
                    btn_Save.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Seal Line Threshold";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_FarSealThreshold.Enabled = false;
                    btn_NearSealThreshold.Enabled = false;

                    srmGroupBox1.Visible = btn_FarSealThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    srmGroupBox4.Visible = btn_NearSealThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Seal Line Segmentation";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    gb_SealLineSegmentation.Enabled = false;
                    chk_SegmentDrawing.Visible = gb_SealLineSegmentation.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Seal Line Gauge Setting";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_GaugeAdvanceSetting.Enabled = false;
                    btn_GaugeAdvanceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Distance Threshold";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_DistanceThreshold.Enabled = false;
                    srmGroupBox5.Visible = btn_DistanceThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Circle Gauge Setting";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    chk_ShowDraggingBox.Enabled = false;
                    chk_ShowSamplePoints.Enabled = false;
                    btn_CircleGaugeSetting.Enabled = false;

                    chk_ShowDraggingBox.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    chk_ShowSamplePoints.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    btn_CircleGaugeSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Sprocket Hole Defect Threshold";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_SprocketHoleDefectThreshold.Enabled = false;
                    btn_SprocketHoleDefectThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Sprocket Hole Inspection Area Inward Tolerance";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    txt_SprocketHoleInspectionAreaInwardTolerance.Enabled = false;
                    txt_SprocketHoleInspectionAreaInwardTolerance.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                    if (!btn_SprocketHoleDefectThreshold.Enabled)
                    {
                        grpbox_SprocketHoleDefectSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }
                }

                strChild3 = "Sprocket Hole Broken Threshold";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_SprocketHoleBrokenThreshold.Enabled = false;
                    btn_SprocketHoleBrokenThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Sprocket Hole Broken Inspection Area Outward Setting";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    grpbox_SprocketHoleBrokenInspectionAreaTolerance.Enabled = false;
                    grpbox_SprocketHoleBrokenInspectionAreaTolerance.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                    if (!btn_SprocketHoleBrokenThreshold.Enabled)
                    {
                        srmGroupBox10.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    }
                }

                strChild3 = "Sprocket Hole Roundness Threshold";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_SprocketHoleRoundnessThreshold.Enabled = false;
                    grpbox_SprocketHoleRoundnessSetting.Visible = btn_SprocketHoleRoundnessThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Over Heat Theshold";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_OverHeatThreshold1.Enabled = false;
                    grpbox_OverHeat.Visible = btn_OverHeatThreshold1.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Seal Edge Straightness Theshold";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_SealEdgeStraightnessThreshold.Enabled = false;
                    srmGroupBox12.Visible = btn_SealEdgeStraightnessThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

            }
            else if (m_intVisionType == 1) // seal empty
            {
                strChild2 = "Learn Seal Empty Page";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    Btn_SaveEmptyPocket.Enabled = false;
                    Btn_SaveEmptyPocket.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Select Empty Pocket Template";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    pnl_Template2.Enabled = false;
                    pnl_Template2.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Delete all Pocket Templates";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    chk_DeleteAllPocketTemplates.Enabled = false;
                    chk_DeleteAllPocketTemplates.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Dont Care Area Setting";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    cbo_DontCareAreaDrawMethod.Enabled = false;
                    btn_AddDontCareROI.Enabled = false;
                    btn_DeleteDontCareROI.Enabled = false;

                    cbo_DontCareAreaDrawMethod.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    btn_AddDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    btn_DeleteDontCareROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    srmLabel41.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    srmLabel42.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                    srmLabel66.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

            }
            else if (m_intVisionType == 2) // seal mark
            {
                strChild2 = "Learn Seal Mark Page";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    Btn_SaveMark.Enabled = false;
                    Btn_SaveMark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Select Mark Template";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    pnl_Template.Enabled = false;
                    pnl_Template.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Delete all Mark Templates";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    chk_DeleteAllMarkTemplates.Enabled = false;
                    chk_DeleteAllMarkTemplates.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Unit Present Threshold";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_UnitPresentThreshold.Enabled = false;
                    btn_UnitPresentThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Unit Present Image Processing Sequence";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    btn_ImageProcessSeq.Enabled = false;
                    btn_ImageProcessSeq.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

                strChild3 = "Rotate Button";
                if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
                {
                    gb_Rotate.Enabled = false;
                    gb_Rotate.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                }

            }
        }

        /// <summary>
        /// Load gauge global settings from xml
        /// </summary>
        private void LoadGaugeGlobalSetting()
        {
            if ((m_smVisionInfo.g_arrSealGauges.Count > 0) &&
                (m_smVisionInfo.g_arrSealGauges[0].Count > 0))
            {
                m_intGaugeTransType = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeTransType;
                m_intGaugeTransChoice = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeTransChoice;
                m_intGaugeThickness = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeThickness;
                m_intGaugeFilter = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeFilter;
                m_intGaugeMinAmp = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeMinAmplitude;
                m_intGaugeMinArea = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeMinArea;
                m_intGaugeNumFilteringPass = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeFilterPasses;
                m_intGaugeThreshold = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeThreshold;
                m_fGaugeFilteringThreshold = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeFilterThreshold;
            }
        }

        /// <summary>
        /// Readjust near seal and far seal roi to be in center of each seal object
        /// </summary>
        /// <returns></returns>
        private void ReadjustSearchROIPosition()
        {
            for (int i = 1; i < 3; i++)
            {
                m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIPositionY -= m_smVisionInfo.g_objSeal.CheckSealBlogInROI(
                    m_smVisionInfo.g_arrSealROIs[i][0], m_smVisionInfo.g_fCalibPixelY);

                m_smVisionInfo.g_arrSealROIs[i][0].LoadROISetting(m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIPositionX,
                             m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIPositionY,
                             m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[i][0].ref_ROIHeight);
                m_smVisionInfo.g_arrSealROIs[i][0].AttachImage(m_smVisionInfo.g_objSealImage);
            }

            m_smVisionInfo.g_objSeal.SetSealAreaTemplate(m_smVisionInfo.g_arrSealROIs);
            m_intROIRealWidth = m_smVisionInfo.g_arrSealROIs[1][0].ref_ROIWidth;
        }

        private void KeepTrainPositionROIPosition()
        {
            m_smVisionInfo.g_objSeal.ref_intPositionCenterPointX = m_smVisionInfo.g_arrSealROIs[0][1].ref_ROICenterX;
            m_smVisionInfo.g_objSeal.ref_intPositionCenterPointY = m_smVisionInfo.g_arrSealROIs[0][1].ref_ROICenterY;
        }

        private bool CheckPositionSearchROI()
        {
            //Save position template here, do pattern matching and calculate pitch
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //    m_smVisionInfo.g_strVisionFolderName + "\\";

            // 2020 01 15 - CCENG: Should not save pattern here. Recipe will only save during press Save button. Else will feel weird recipe change even though user press cancel.
            //m_smVisionInfo.g_objSeal.SaveSealPositionTemplate(strPath + "Seal\\Template\\Position\\", m_smVisionInfo.g_arrSealROIs[0][1]);
            //m_smVisionInfo.g_objSeal.LoadPositionPattern(strPath + "Seal\\Template\\Position\\");

            m_smVisionInfo.g_objSeal.ref_intPositionCenterPointX = m_smVisionInfo.g_arrSealROIs[0][1].ref_ROICenterX;
            m_smVisionInfo.g_objSeal.ref_intPositionCenterPointY = m_smVisionInfo.g_arrSealROIs[0][1].ref_ROICenterY;

            //if the tape pocket pitch is less than 2mm, there will be only 1 sprocket per 2 pocket
            if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch == 2)
            {
                //if (!m_smVisionInfo.g_objSeal.CheckPositionPatternPitch(m_smVisionInfo.g_arrSealROIs[0][0]))
                //{
                //    m_intBackToStep1 = true;
                //    SRMMessageBox.Show("Please ensure that Position Search ROI is in the correct position.");
                //    return false;
                //}
            }
            return true;
        }

        /// <summary>
        /// Save seal settings into general.xml
        /// </summary>
        /// <param name="strFolderPath">folder path</param>
        private void SaveGeneralSettings(string strFolderPath, bool blnSaveMark, bool blnSavePocket)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "General.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "General.xml");
            objFile.WriteSectionElement("TemplateCounting");

            if (blnSaveMark)
            {
                if (m_intMarkSelectedIndex == m_smVisionInfo.g_intMarkTemplateTotal)
                {
                    m_smVisionInfo.g_intMarkTemplateTotal++;
                }
                objFile.WriteElement1Value("TotalMarkTemplates", m_smVisionInfo.g_intMarkTemplateTotal);

                m_smVisionInfo.g_intMarkTemplateMask |= (0x01 << m_intMarkSelectedIndex);
                objFile.WriteElement1Value("MarkTemplateMask", m_smVisionInfo.g_intMarkTemplateMask);
            }

            if (blnSavePocket)
            {
                if (m_intPocketSelectedIndex == m_smVisionInfo.g_intPocketTemplateTotal)
                {
                    m_smVisionInfo.g_intPocketTemplateTotal++;
                }
                objFile.WriteElement1Value("TotalPocketTemplates", m_smVisionInfo.g_intPocketTemplateTotal);

                m_smVisionInfo.g_intPocketTemplateMask |= (0x01 << m_intPocketSelectedIndex);
                objFile.WriteElement1Value("PocketTemplateMask", m_smVisionInfo.g_intPocketTemplateMask);
            }

            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Seal", m_smProductionInfo.g_strLotID);
            
        }

        /// <summary>
        /// Initialize before learn seal
        /// </summary>
        private void UpdateGUI()
        {
            chk_ShowDraggingBox.Checked = m_smVisionInfo.g_objSealCircleGauges.ref_blnDrawDraggingBox;
            chk_ShowSamplePoints.Checked = m_smVisionInfo.g_objSealCircleGauges.ref_blnDrawSamplingPoint;
            txt_SealScoreTolerance.Text = (m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance * 100).ToString();
            txt_ShiftTolerance.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance / m_smVisionInfo.g_fCalibPixelX, 3, MidpointRounding.AwayFromZero).ToString("f3");
            txt_NearMinArea.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intSeal2AreaFilter / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
            txt_FarMinArea.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intSeal1AreaFilter / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");

            // 2020 01 07 - CCENG: Convert from Integer to float will cause missing value.
            //txt_OverHeatMinArea.Text = Math.Round(m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelY), 3, MidpointRounding.AwayFromZero).ToString("f3");
            //txt_OverHeatMinArea.Text = m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea.ToString();
            //txt_OverHeatMinArea.Text = m_smVisionInfo.g_objSeal.ref_fOverHeatAreaMinTolerance.ToString();
            txt_SprocketHoleInspectionAreaInwardTolerance.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleInspectionAreaInwardTolerance.ToString();
            txt_SprocketHoleBrokenOutwardTolerance_Outer.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer.ToString();
            txt_SprocketHoleBrokenOutwardTolerance_Inner.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner.ToString();
            txt_SegmentSections.Text = m_smVisionInfo.g_objSeal.ref_intBuildObjectLength.ToString();
            if (m_smVisionInfo.g_arrSealGauges.Count > 0)
                if (m_smVisionInfo.g_arrSealGauges[0].Count > 0)
                    txt_SamplingSteps.Text = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeSamplingStep.ToString();
            chk_SegmentDrawing.Checked = m_smVisionInfo.g_objSeal.ref_blnViewSegmentDrawing;

            if (m_intOrientDirection == 4)
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    lbl_Degree.Text = "90 deg";
                else
                    lbl_Degree.Text = "90 度";
            }
            else
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    lbl_Degree.Text = "180 deg";
                else
                    lbl_Degree.Text = "180 度";
            }

            m_intSelectedDontCareROIIndexPrev = cbo_DontCareAreaDrawMethod.SelectedIndex = 1;   // 2020 03 31 - CCENG: Default select circle because normally circle dont care roi will be used for empty pocket

            if (m_smVisionInfo.g_objSeal.ref_intCheckMarkMethod != 3)
                btn_ImageProcessSeq.Visible = false;
            else
                btn_ImageProcessSeq.Visible = true;

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");

            m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAllOverHeatROI.Checked = Convert.ToBoolean(subkey.GetValue("SetToAllOverHeatROI_SealLearn", false));

            if (m_smVisionInfo.g_arrSealROIs.Count <= 4 || m_smVisionInfo.g_arrSealROIs[4].Count == 1)
            {
                chk_SetToAllOverHeatROI.Visible = false;
                m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAllOverHeatROI.Checked = false;
            }
        }

        private void UpdatePocketTemplateGUI()
        {
            /*         cbo_PocketTemplateNo.Items.Clear();

                     if (chk_DeleteAllPocketTemplates.Checked)
                     {
                         cbo_PocketTemplateNo.Items.Add("Template 1");
                         m_smVisionInfo.g_intPocketTemplateIndex = m_intPocketSelectedIndex = 0;
                     }
                     else
                     {
                         // Update Template GUI
                         for (int i = 0; i < m_smVisionInfo.g_intPocketTemplateTotal; i++)
                         {
                             cbo_PocketTemplateNo.Items.Add("Template " + (i + 1));
                         }

                         if (m_smVisionInfo.g_intPocketTemplateTotal == 0)
                             cbo_PocketTemplateNo.Items.Add("Template 1");
                         else if (m_smVisionInfo.g_intPocketTemplateTotal < 4)
                             cbo_PocketTemplateNo.Items.Add("New...");

                         if ((m_smVisionInfo.g_intPocketTemplateTotal == 1) || (m_smVisionInfo.g_intPocketTemplateTotal == 4))
                             m_smVisionInfo.g_intPocketTemplateIndex = m_intPocketSelectedIndex = 0;
                         else
                             m_smVisionInfo.g_intPocketTemplateIndex = m_intPocketSelectedIndex = m_smVisionInfo.g_intPocketTemplateTotal;
                     }
                     m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex; */
        }

        private void UpdateMarkTemplateGUI()
        {
            /*    cbo_MarkTemplateNo.Items.Clear();

                if (chk_DeleteAllMarkTemplates.Checked)
                {
                    cbo_MarkTemplateNo.Items.Add("Template 1");
                    m_smVisionInfo.g_intMarkTemplateIndex = m_intMarkSelectedIndex = 0;
                }
                else
                {
                    // Update Template GUI
                    for (int i = 0; i < m_smVisionInfo.g_intMarkTemplateTotal; i++)
                    {
                        cbo_MarkTemplateNo.Items.Add("Template " + (i + 1));
                    }

                    if (m_smVisionInfo.g_intMarkTemplateTotal == 0)
                        cbo_MarkTemplateNo.Items.Add("Template 1");
                    else if (m_smVisionInfo.g_intMarkTemplateTotal < 8)     // 2020 09 26 - CCENG:  Add template maximum to 8
                        cbo_MarkTemplateNo.Items.Add("New...");

                    if ((m_smVisionInfo.g_intMarkTemplateTotal == 1) || (m_smVisionInfo.g_intMarkTemplateTotal == 8))
                        m_smVisionInfo.g_intMarkTemplateIndex = m_intMarkSelectedIndex = 0;
                    else
                        m_smVisionInfo.g_intMarkTemplateIndex = m_intMarkSelectedIndex = m_smVisionInfo.g_intMarkTemplateTotal;
                }
                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;*/
        }

        private void LoadSealGlobalSettings()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            m_intOrientDirection = objFileHandle.GetValueAsInt("Direction", 4);

            if (m_intOrientDirection == 4)
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    lbl_Degree.Text = "90 deg";
                else
                    lbl_Degree.Text = "90 度";
            }
            else
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    lbl_Degree.Text = "180 deg";
                else
                    lbl_Degree.Text = "180 度";
            }
        }

        private void DeleteAllPreviousSealTemplate(string strFolderPath)
        {
            // Delete database
            try
            {
                if (Directory.Exists(strFolderPath))
                    Directory.Delete(strFolderPath, true);
            }
            catch
            {
            }

            Directory.CreateDirectory(strFolderPath);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\";

            ROI.LoadFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);
            for (int i = 0; i < m_smVisionInfo.g_arrSealROIs.Count; i++)
                for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealROIs[i][j].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(i)]);
                }

            LGauge.LoadFile(strPath + "Seal\\Gauge.xml", m_smVisionInfo.g_arrSealGauges, m_smVisionInfo.g_WorldShape);
            m_smVisionInfo.g_objSealCircleGauges.LoadCircleGauge(strPath + "Seal\\CircleGauge.xml", "CircleGauge");
            LoadSealGeneralSetting(strPath + "General.xml");
            m_smVisionInfo.g_objSeal.LoadSeal(strPath + "Seal\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX);
            m_smVisionInfo.g_blnUpdateZoom = true;
            Close();
            Dispose();
        }
        private void LoadSealGeneralSetting(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection("TemplateCounting");
            m_smVisionInfo.g_intPocketTemplateTotal = objFile.GetValueAsInt("TotalPocketTemplates", 0, 1);
            m_smVisionInfo.g_intMarkTemplateTotal = objFile.GetValueAsInt("TotalMarkTemplates", 0, 1);
            m_smVisionInfo.g_intPocketTemplateMask = objFile.GetValueAsInt("PocketTemplateMask", 0, 1);
            m_smVisionInfo.g_intMarkTemplateMask = objFile.GetValueAsInt("MarkTemplateMask", 0, 1);

            if (m_smVisionInfo.g_intPocketTemplateTotal > m_smVisionInfo.g_intMaxSealEmptyTemplate)
                m_smVisionInfo.g_intPocketTemplateTotal = m_smVisionInfo.g_intMaxSealEmptyTemplate;
            if (m_smVisionInfo.g_intMarkTemplateTotal > m_smVisionInfo.g_intMaxSealMarkTemplate)
                m_smVisionInfo.g_intMarkTemplateTotal = m_smVisionInfo.g_intMaxSealMarkTemplate;

        }
        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (m_intVisionType == 1)   // Learn Empty Pocket
            {
                m_smVisionInfo.g_intLearnStepNo = 15;   // Empty Dont Care ROI Step
            }
            else
            {
                //if (m_smVisionInfo.g_intLearnStepNo < 11)
                    m_smVisionInfo.g_intLearnStepNo++;

                if ((m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                    && m_smVisionInfo.g_intLearnStepNo == 6)
                    m_smVisionInfo.g_intLearnStepNo = 10;// m_smVisionInfo.g_intLearnStepNo++;

                if (m_smVisionInfo.g_intLearnStepNo == 7)
                {
                    if(m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                        m_smVisionInfo.g_intLearnStepNo = 8;

                    if (m_smVisionInfo.g_intLearnStepNo == 8 && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                        m_smVisionInfo.g_intLearnStepNo = 10;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 8 && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                    m_smVisionInfo.g_intLearnStepNo = 10;

                if (m_smVisionInfo.g_intLearnStepNo == 11 && !m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
                    m_smVisionInfo.g_intLearnStepNo++;
            }
            //if (m_blnLearnPocketPattern)
            //    m_smVisionInfo.g_intLearnStepNo = 12;
            //else if (m_blnLearnMarkPattern)
            //    m_smVisionInfo.g_intLearnStepNo = 13;
            m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = false;
            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = false;
            m_blnNextButton = true;
            if (SetupStep())
            {
                m_intDisplayStepNo++;
                if (lbl_StepNo.Text.Length < 8)
                    lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
                else
                    lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 3) + m_intDisplayStepNo.ToString() + ":";
            }

        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            if (m_intVisionType == 1)   // Learn Empty Pocket
            {
                m_smVisionInfo.g_intLearnStepNo = 13;    // Learn Empty Pocket Step
            }
            else
            {
                //if (m_smVisionInfo.g_intLearnStepNo > 0)
                //    m_smVisionInfo.g_intLearnStepNo--;

                if (m_smVisionInfo.g_intLearnStepNo > 1)
                    m_smVisionInfo.g_intLearnStepNo--;

                if ((m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                    && m_smVisionInfo.g_intLearnStepNo == 9)
                    m_smVisionInfo.g_intLearnStepNo = 5;//m_smVisionInfo.g_intLearnStepNo--;

                if (m_smVisionInfo.g_intLearnStepNo == 9)
                {
                    if (m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                        m_smVisionInfo.g_intLearnStepNo = 7;

                    if (m_smVisionInfo.g_intLearnStepNo == 7 && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                        m_smVisionInfo.g_intLearnStepNo = 6;
                }

                if (m_smVisionInfo.g_intLearnStepNo == 7 && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                    m_smVisionInfo.g_intLearnStepNo = 6;

                if (m_smVisionInfo.g_intLearnStepNo == 11 && !m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
                    m_smVisionInfo.g_intLearnStepNo--;

                //if (m_blnLearnPocketPattern || m_blnLearnMarkPattern)
                //    m_smVisionInfo.g_intLearnStepNo = 11;
            }
            m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = false;
            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = false;
            m_blnNextButton = false;
            if (SetupStep())
            {
                m_intDisplayStepNo--;
                if (lbl_StepNo.Text.Length < 8)
                    lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
                else
                    lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 3) + m_intDisplayStepNo.ToString() + ":";
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
               m_smVisionInfo.g_strVisionFolderName + "\\";
            switch (m_intLearnType)
            {
                case 0:
                    
                    STDeviceEdit.CopySettingFile(strPath, "Seal\\ROI.xml");
                    ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Seal ROI", m_smProductionInfo.g_strLotID);
                    STDeviceEdit.CopySettingFile(strPath, "Seal\\Gauge.xml");
                    LGauge.SaveFile(strPath + "Seal\\Gauge.xml", m_smVisionInfo.g_arrSealGauges);
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Seal Gauge", m_smProductionInfo.g_strLotID);
                    m_smVisionInfo.g_objSealCircleGauges.SaveCircleGauge(strPath + "Seal\\CircleGauge.xml", false, "CircleGauge", true, true, (!m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole || !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect || !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness));

                    if (!m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole || !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect || !m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                    {
                        m_smVisionInfo.g_objSeal.SaveTemplateCircleImage(m_smVisionInfo.g_objSealCircleGauges, m_smVisionInfo.g_objWhiteImage, strPath + "Seal\\Template\\SprocketHoleCircleImage.bmp");
                    }

                    // Delete previous pocket template
                    if (chk_DeleteAllPocketTemplates.Checked)
                    {
                        m_smVisionInfo.g_intPocketTemplateTotal = 0;
                        m_smVisionInfo.g_intPocketTemplateMask = 0;
                        DeleteAllPreviousSealTemplate(strPath + "Seal\\Template\\Pocket\\");
                    }

                    // Delete previous mark template
                    if (chk_DeleteAllMarkTemplates.Checked)
                    {
                        m_smVisionInfo.g_intMarkTemplateTotal = 0;
                        m_smVisionInfo.g_intMarkTemplateMask = 0;
                        DeleteAllPreviousSealTemplate(strPath + "Seal\\Template\\Mark\\");
                    }
                    SaveGeneralSettings(strPath, false, false); // 2020 01 09 - CCENG: no longer save mark template or empty pocket template during learn seal.
                    STDeviceEdit.CopySettingFile(strPath, "Seal\\Settings.xml");
                    m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Seal", m_smProductionInfo.g_strLotID);

                    //Save Learn Template Search ROI image, unit image
                    CopyFiles m_objCopy = new CopyFiles();
                    string strCurrentDateTIme = DateTime.Now.ToString();
                    DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
                    string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
                    string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
                    string strPathEditLog = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Seal\\";

                    if (File.Exists(strPath + "Seal\\Template\\OriTemplate.bmp"))
                        STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Seal", "OriTemplate0.bmp", "OriTemplate0.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
                    else
                        STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Seal", "", "OriTemplate0.bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
                    
                    m_objCopy.CopyAllImageFiles(strPath + "Seal\\Template\\", strPathEditLog + "Old\\");

                    m_smVisionInfo.g_objSeal.SaveSealTemplateImage(strPath + "Seal\\Template\\", m_smVisionInfo.g_arrImages);

                    m_objCopy.CopyAllImageFiles(strPath + "Seal\\Template\\", strPathEditLog + "New\\");

                    m_smVisionInfo.g_objSeal.SaveSealPositionTemplate(strPath + "Seal\\Template\\Position\\", m_smVisionInfo.g_arrSealROIs[0][1]);
                    m_smVisionInfo.g_objSeal.LoadPositionPattern(strPath + "Seal\\Template\\Position\\");

                    //if (!chk_SkipPocket.Checked)
                    //{
                    //    m_smVisionInfo.g_objSeal.SaveSealPocketTemplate(strPath + "Seal\\Template\\Pocket\\", m_smVisionInfo.g_arrSealROIs[5][1]);
                    //    m_smVisionInfo.g_objSeal.LoadPocketPattern(strPath + "Seal\\Template\\Pocket\\");
                    //}

                    //if (!chk_SkipMark.Checked)
                    //{
                    //    m_smVisionInfo.g_objSeal.SaveSealMarkTemplate4Direction(strPath + "Seal\\Template\\Mark\\", m_smVisionInfo.g_arrSealROIs[5][2]);
                    //    m_smVisionInfo.g_objSeal.LoadMarkPattern4Direction(strPath + "Seal\\Template\\Mark\\");
                    //}

                    m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

                    break;
                case 1:
                    ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);

                    m_smVisionInfo.g_objSeal.SaveSealTemplateImage(strPath + "Seal\\Template\\", m_smVisionInfo.g_arrImages);
                    break;
                case 2:
                    ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);

                    m_smVisionInfo.g_objSeal.SaveSealPositionTemplate(strPath + "Seal\\Template\\Position\\", m_smVisionInfo.g_arrSealROIs[0][1]);
                    m_smVisionInfo.g_objSeal.LoadPositionPattern(strPath + "Seal\\Template\\Position\\");
                    break;
                case 3:
                    ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);

                    m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);
                    break;
                case 4:

                    LGauge.SaveFile(strPath + "Seal\\Gauge.xml", m_smVisionInfo.g_arrSealGauges);

                    m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);

                    break;
                case 5:
                    ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);

                    m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);

                    break;
                case 6:
                    ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);

                    m_smVisionInfo.g_objSealCircleGauges.SaveCircleGauge(strPath + "Seal\\CircleGauge.xml", false, "CircleGauge", true, true, true);
                    m_smVisionInfo.g_objSeal.SaveTemplateCircleImage(m_smVisionInfo.g_objSealCircleGauges, m_smVisionInfo.g_objWhiteImage, strPath + "Seal\\Template\\SprocketHoleCircleImage.bmp");

                    m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);

                    break;
                case 7:
                    ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);

                    m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);

                    break;
                case 8:
                    ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);

                    break;
            }

            Close();
            Dispose();
        }

        private void btn_Segment_Click(object sender, EventArgs e)
        {
            AddGauge();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_FarThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 1;

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(2);

            if (m_smVisionInfo.g_objPackageImage == null)
                m_smVisionInfo.g_objPackageImage = new ImageDrawing(true);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_blnViewPackageImage = true;
            ImageDrawing.CloseBoxImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objSeal.ref_intSealEdgeSensitivity);

            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPackageImage);

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSeal1Threshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[1][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.ref_intSeal1Threshold = m_smVisionInfo.g_intThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.g_objSeal.BuildObjects(m_smVisionInfo.g_arrSealROIs);
            lbl_FarArea.Text = (m_smVisionInfo.g_objSeal.GetSeal1Area() / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelX)).ToString("f3");

            m_smVisionInfo.g_blnViewPackageImage = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_NearThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 2;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(2);

            if (m_smVisionInfo.g_objPackageImage == null)
                m_smVisionInfo.g_objPackageImage = new ImageDrawing(true);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_blnViewPackageImage = true;
            ImageDrawing.CloseBoxImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objSeal.ref_intSealEdgeSensitivity);

            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPackageImage);

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSeal2Threshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[2][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.ref_intSeal2Threshold = m_smVisionInfo.g_intThresholdValue;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.g_objSeal.BuildObjects(m_smVisionInfo.g_arrSealROIs);
            lbl_NearArea.Text = (m_smVisionInfo.g_objSeal.GetSeal2Area() / (m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelX)).ToString("f3");

            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_intPocketPitch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbo_Package_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void txt_FarMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_FarMinArea.Text == "" || txt_FarMinArea.Text == null)
                return;

            try
            {
                m_smVisionInfo.g_objSeal.ref_intSeal1AreaFilter = (int)Math.Round(Convert.ToSingle(txt_FarMinArea.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelX, 0, MidpointRounding.AwayFromZero);
                m_smVisionInfo.g_objSeal.BuildObjects(m_smVisionInfo.g_arrSealROIs);
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            catch
            {
            }
        }

        private void txt_NearMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_NearMinArea.Text == "" || txt_NearMinArea.Text == null)
                return;

            m_smVisionInfo.g_objSeal.ref_intSeal2AreaFilter = (int)Math.Round(Convert.ToSingle(txt_NearMinArea.Text) * m_smVisionInfo.g_fCalibPixelX * m_smVisionInfo.g_fCalibPixelX, 0, MidpointRounding.AwayFromZero);
            m_smVisionInfo.g_objSeal.BuildObjects(m_smVisionInfo.g_arrSealROIs);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SamplingSteps_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SamplingSteps.Text == "" || txt_SamplingSteps.Text == null)
                return;

            AddGauge();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SegmentSections_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SegmentSections.Text == "" || txt_SegmentSections.Text == null)
                return;

            if (Convert.ToInt32(txt_SegmentSections.Text) > (m_intROIRealWidth / 2))
            {
                SRMMessageBox.Show("Segment Sections count cannot higher than " + (m_intROIRealWidth / 2).ToString() + " [half of ROI width].");
                txt_SegmentSections.Text = (m_intROIRealWidth / 2).ToString();
            }
            else if (Convert.ToInt32(txt_SegmentSections.Text) <= 0)
            {
                SRMMessageBox.Show("Segment Sections count cannot less than or equal to 0.");
                txt_SegmentSections.Text = "1";
            }

            m_smVisionInfo.g_objSeal.ref_intBuildObjectLength = Convert.ToInt32(txt_SegmentSections.Text);
            AddGauge();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }



        private void LearnSealForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                             m_smVisionInfo.g_strVisionFolderName + "\\";

            if (m_intVisionType == 0)
            {
                // 2020 03 27 - CCENG: reset all ROI to original position before learn. All ROI position may be shifted after inspection
                //------------------------------------------------------------------------------------------------------------------------

                ROI.LoadFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);
                for (int i = 0; i < m_smVisionInfo.g_arrSealROIs.Count; i++)
                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[i].Count; j++)
                    {
                        m_smVisionInfo.g_arrSealROIs[i][j].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(i)]);
                    }

                LGauge.LoadFile(strPath + "Seal\\Gauge.xml", m_smVisionInfo.g_arrSealGauges, m_smVisionInfo.g_WorldShape);
                m_smVisionInfo.g_objSealCircleGauges.LoadCircleGauge(strPath + "Seal\\CircleGauge.xml", "CircleGauge");
                m_smVisionInfo.g_objSeal.LoadSeal(strPath + "Seal\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX);
                //------------------------------------------------------------------------------------------------------------------------
            }
            else if (m_intVisionType == 1)  // Learn Empty Pocket
            {
                // 2020 10 08 - Trigger Offline Test should done before enter form. (This is because certain variable will be init during enter from, but offline test will accidentally reset back the variable.
                //TriggerOfflineTest();
            }
            else if (m_intVisionType == 2)  // Learn Mark
            {
                // 2020 10 08 - Trigger Offline Test should done before enter form. (This is because certain variable will be init during enter from, but offline test will accidentally reset back the variable.
                //TriggerOfflineTest();
            }

            lbl_StepNo.BringToFront();
            strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Template";
            if (!Directory.Exists(strPath))
            {
                btn_LearnPocket.Enabled = false;
                btn_LearnUnit.Enabled = false;
            }

            m_smVisionInfo.g_intSelectedROI = 0;

            SetupStep();

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (i < m_smVisionInfo.g_arrblnImageRotated.Length)
                    m_smVisionInfo.g_arrblnImageRotated[i] = true;
            }

            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void LearnSealForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (i < m_smVisionInfo.g_arrblnImageRotated.Length)
                    m_smVisionInfo.g_arrblnImageRotated[i] = false;
            }
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = false;
            m_smVisionInfo.g_intSelectedROI = 0;
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Seal Form Closed", "Exit Learn Seal Form", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = false;
            m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = false;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewSealImage = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewSealObjectsBuilded = false;
            m_smVisionInfo.g_objSeal.ref_blnViewSegmentDrawing = false;
            //m_blnLearnPocketPattern = false;
            //m_blnLearnMarkPattern = false;
            //m_smVisionInfo.g_blnUpdateZoom = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void chk_SegmentDrawing_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSeal.ref_blnViewSegmentDrawing = chk_SegmentDrawing.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            m_objAdvancedForm = new AdvancedPostSealLGaugeForm(m_smVisionInfo, m_smProductionInfo.g_strRecipePath +
                            m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\Gauge.xml", m_smProductionInfo, m_smCustomizeInfo);

            if (m_objAdvancedForm.ShowDialog() == DialogResult.OK)
            {
                LoadGaugeGlobalSetting();
                AddGauge();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }
            else
            {
                LoadGaugeGlobalSetting();
                AddGauge();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void btn_LearnPocket_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intLearnStepNo = 13;

            if (SetupStep())
            {
                m_intDisplayStepNo = 8;
                lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            }

            //m_blnLearnPocketPattern = true;
        }

        private void btn_LearnUnit_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intLearnStepNo = 14;

            if (SetupStep())
            {
                m_intDisplayStepNo = 8;
                lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            }

            //m_blnLearnMarkPattern = true;
        }

        private void Btn_SaveEmptyPocket_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\";

            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            //DeviceEdit objDeviceEdit = new DeviceEdit(m_smProductionInfo);
            string strImagePath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Seal Pocket\\";

            if (File.Exists(strPath + "Seal\\Template\\Pocket\\PocketTemplate0_" + m_intPocketSelectedIndex + ".bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Seal Pocket", "PocketTemplate0_" + m_intPocketSelectedIndex + ".bmp", "PocketTemplate0_" + m_intMarkSelectedIndex + ".bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Seal Pocket", "", "PocketTemplate0_" + m_intPocketSelectedIndex + ".bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(strPath + "Seal\\Template\\Pocket\\", strImagePath + "Old\\");

            // Delete previous template
            if (chk_DeleteAllPocketTemplates.Checked)
            {
                m_smVisionInfo.g_intPocketTemplateTotal = 0;
                m_smVisionInfo.g_intPocketTemplateMask = 0;
                DisposePictureBoxImage2();
                DeleteAllPreviousSealTemplate(strPath + "Seal\\Template\\Pocket\\");
            }

            DisposePictureBoxImage2();
            // Alter image if have dont care ROI 
            if (m_smVisionInfo.g_objSeal.ref_blnWantDontCareArea)
            {
                if (m_smVisionInfo.g_arrSealDontCareROIs.Count > 0)
                {
                    if (m_smVisionInfo.g_objPackageImage == null)
                        m_smVisionInfo.g_objPackageImage = new ImageDrawing(true);

                    m_smVisionInfo.g_arrImages[1].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                    m_smVisionInfo.g_arrSealROIs[5][1].AttachImage(m_smVisionInfo.g_objPackageImage);
                    for (int i = 0; i < m_smVisionInfo.g_arrSealDontCareROIs.Count; i++)
                    {
                        m_smVisionInfo.g_arrPolygon_Seal[i].AddPoint(new PointF(m_smVisionInfo.g_arrSealDontCareROIs[i].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                                                                                m_smVisionInfo.g_arrSealDontCareROIs[i].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                        m_smVisionInfo.g_arrPolygon_Seal[i].AddPoint(new PointF((m_smVisionInfo.g_arrSealDontCareROIs[i].ref_ROITotalX + m_smVisionInfo.g_arrSealDontCareROIs[i].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                                                                                (m_smVisionInfo.g_arrSealDontCareROIs[i].ref_ROITotalY + m_smVisionInfo.g_arrSealDontCareROIs[i].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                        m_smVisionInfo.g_arrPolygon_Seal[i].AddPolygon((int)(m_smVisionInfo.g_arrSealROIs[5][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX),
                                                                       (int)(m_smVisionInfo.g_arrSealROIs[5][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));

                        ImageDrawing objImage = new ImageDrawing(true);
                        Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrSealROIs[5][1], m_smVisionInfo.g_arrPolygon_Seal[i], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        ROI objDontCareROI = new ROI();
                        objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrSealROIs[5][1].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][1].ref_ROIHeight);
                        objDontCareROI.AttachImage(objImage);
                        ROI.SubtractROI(m_smVisionInfo.g_arrSealROIs[5][1], objDontCareROI);
                        objDontCareROI.Dispose();
                        objImage.Dispose();
                    }
                }

                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
                ROI.SaveFile(strFolderPath + "Seal\\Template\\Pocket\\DontCareROI.xml",
                            "Template" + m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex.ToString(),
                            m_smVisionInfo.g_arrSealDontCareROIs);
                Polygon.SavePolygon(strFolderPath + "Seal\\Template\\Pocket\\Polygon.xml",
                                    "Template" + m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex.ToString(),
                                    m_smVisionInfo.g_arrPolygon_Seal);
            }

            
            STDeviceEdit.CopySettingFile(strPath, "Seal\\ROI.xml");
            ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Seal ROI", m_smProductionInfo.g_strLotID);
            
            SaveGeneralSettings(strPath, false, true);
            m_smVisionInfo.g_objSeal.SaveSealPocketTemplate(strPath + "Seal\\Template\\Pocket\\", m_smVisionInfo.g_arrSealROIs[5][1]);
            m_smVisionInfo.g_objSeal.LoadPocketPattern(strPath + "Seal\\Template\\Pocket\\");

            m_objCopy.CopyAllImageFiles(strPath + "Seal\\Template\\Pocket\\", strImagePath + "New\\");

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;
            m_smVisionInfo.g_blnUpdateZoom = true;
            Close();
            Dispose();
        }

        private void cbo_PocketTemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex = m_intPocketSelectedIndex;

            if (m_smVisionInfo.g_objSeal.GetPocketMatcherWidth(m_intPocketSelectedIndex) == 0 ||
                  m_smVisionInfo.g_objSeal.GetPocketMatcherHeight(m_intPocketSelectedIndex) == 0)
            {
                m_smVisionInfo.g_arrSealROIs[5][1].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROIPositionX,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROIPositionY,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROIWidth,
                                                                ((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROIHeight);
            }
            else
            {
                int intWidth = m_smVisionInfo.g_objSeal.GetPocketMatcherWidth(m_intPocketSelectedIndex);
                int intHeight = m_smVisionInfo.g_objSeal.GetPocketMatcherHeight(m_intPocketSelectedIndex);
                m_smVisionInfo.g_arrSealROIs[5][1].LoadROISetting(((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROICenterX - intWidth / 2,
                                                                 ((ROI)m_smVisionInfo.g_arrSealROIs[5][1]).ref_ROICenterY - intHeight / 2,
                                                                intWidth,
                                                                intHeight);

            }

            if (m_smVisionInfo.g_blnViewSealImage)
            {
                m_smVisionInfo.g_arrSealROIs[5][1].AttachImage(m_smVisionInfo.g_objSealImage);
            }
            else
            {
                m_smVisionInfo.g_arrSealROIs[5][1].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_DeleteAllPocketTemplates_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            if (chk_DeleteAllPocketTemplates.Checked)
            {
                Graphics[] g_pic = new Graphics[4];
                pic_Panel2[0].Visible = true;

                for (int i = 1; i < m_smVisionInfo.g_intMaxSealEmptyTemplate; i++)//4
                {
                    pic_Panel2[i].BackColor = Color.Red;
                    pic_Panel2[i].Visible = true;
                }

                for (int j = 1; j < m_smVisionInfo.g_intMaxSealEmptyTemplate; j++)//4
                {
                    if (pic_Template2[j].Image != null)
                    {
                        bmp[j] = new Bitmap(pic_Template2[j].Image);
                        g_pic[j] = Graphics.FromImage(bmp[j]);
                        //g_pic[j].DrawLine(new Pen(Color.Red, 2), new Point(0, 0), new Point(pic_Template2[j].PreferredSize.Width, pic_Template2[j].PreferredSize.Height));
                        //g_pic[j].DrawLine(new Pen(Color.Red, 2), new Point(pic_Template2[j].PreferredSize.Width, 0), new Point(0, pic_Template2[j].PreferredSize.Height));
                        pic_Template2[j].Image = bmp[j];
                    }
                    else
                        break;
                }
                m_intTempPocketSelectedIndex = m_intPocketSelectedIndex;
                m_intPocketSelectedIndex = 0;
                m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex;
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_intMaxSealEmptyTemplate; i++)//4
                {
                    pic_Panel2[i].BackColor = Color.Lime;
                    pic_Panel2[i].Visible = false;
                }

                for (int j = 1; j < m_smVisionInfo.g_intMaxSealEmptyTemplate; j++)//4
                {
                    if (pic_Template2[j].Image != null)
                    {
                        Oribmp[j] = (Bitmap)Bitmap.FromFile(pic_Template2[j].ImageLocation);
                        pic_Template2[j].Image = Oribmp[j];
                    }
                }

                m_intPocketSelectedIndex = m_intTempPocketSelectedIndex;
                m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex;
                pic_Panel2[m_intTempPocketSelectedIndex].Visible = true;
            }
            m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
            UpdatePocketTemplateGUI();
        }

        private void chk_DeleteAllMarkTemplates_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (chk_DeleteAllMarkTemplates.Checked)
            {
                Graphics[] g_pic = new Graphics[8];
                pic_Panel[0].Visible = true;

                for (int i = 1; i < m_smVisionInfo.g_intMaxSealMarkTemplate; i++)//8
                {
                    pic_Panel[i].BackColor = Color.Red;
                    pic_Panel[i].Visible = true;
                }

                for (int j = 1; j < m_smVisionInfo.g_intMaxSealMarkTemplate; j++)//8
                {
                    if (pic_Template[j].Image != null)
                    {
                        bmp[j] = new Bitmap(pic_Template[j].Image);
                        g_pic[j] = Graphics.FromImage(bmp[j]);
                        //g_pic[j].DrawLine(new Pen(Color.Red, 4), new Point(0, 0), new Point(pic_Template[j].PreferredSize.Width, pic_Template[j].PreferredSize.Height));
                        //g_pic[j].DrawLine(new Pen(Color.Red, 4), new Point(pic_Template[j].PreferredSize.Width, 0), new Point(0, pic_Template[j].PreferredSize.Height));
                        pic_Template[j].Image = bmp[j];
                    }
                    else
                        break;
                }
                m_intTempMarkSelectedIndex = m_intMarkSelectedIndex;
                m_intMarkSelectedIndex = 0;
                m_smVisionInfo.g_intMarkTemplateIndex = /*m_smVisionInfo.g_intMarkTemplateTotal = */m_intMarkSelectedIndex;
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_intMaxSealMarkTemplate; i++)//8
                {
                    pic_Panel[i].BackColor = Color.Lime;
                    pic_Panel[i].Visible = false;
                }

                for (int j = 1; j < m_smVisionInfo.g_intMaxSealMarkTemplate; j++)//8
                {
                    if (pic_Template[j].Image != null)
                    {
                        Oribmp[j] = (Bitmap)Bitmap.FromFile(pic_Template[j].ImageLocation);
                        pic_Template[j].Image = Oribmp[j];
                    }
                }

                m_intMarkSelectedIndex = m_intTempMarkSelectedIndex;
                m_smVisionInfo.g_intMarkTemplateIndex = /*m_smVisionInfo.g_intMarkTemplateTotal =*/ m_intMarkSelectedIndex;
                pic_Panel[m_intMarkSelectedIndex].Visible = true;
            }
            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
            UpdateMarkTemplateGUI();
        }

        private void cbo_MarkTemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex = m_intMarkSelectedIndex;
        }

        private void btn_OverHeatThreshold_Click(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_intSelectedUnit = 4;
            //m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intOverHeatThreshold;

            //ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[4][0]);
            //Rectangle resolution = Screen.PrimaryScreen.Bounds;
            //objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            ////objThresholdForm.Location = new Point(769, 310);
            //if (objThresholdForm.ShowDialog() == DialogResult.OK)
            //{
            //    m_smVisionInfo.g_objSeal.ref_intOverHeatThreshold = m_smVisionInfo.g_intThresholdValue;
            //}
            //objThresholdForm.Dispose();

            switch (Convert.ToInt32(((Button)sender).Tag))
            {
                case 0:
                    m_smVisionInfo.g_intSelectedROI = 0;
                    break;
                case 1:
                    m_smVisionInfo.g_intSelectedROI = 1;
                    break;
                case 2:
                    m_smVisionInfo.g_intSelectedROI = 2;
                    break;
                case 3:
                    m_smVisionInfo.g_intSelectedROI = 3;
                    break;
                case 4:
                    m_smVisionInfo.g_intSelectedROI = 4;
                    break;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.g_intSelectedUnit = 4;
            m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_objSeal.GetOverHeatLowThreshold(m_smVisionInfo.g_intSelectedROI);
            m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_objSeal.GetOverHeatHighThreshold(m_smVisionInfo.g_intSelectedROI);

            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[4][m_smVisionInfo.g_intSelectedROI]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (chk_SetToAllOverHeatROI.Checked)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrSealROIs[4].Count; j++)
                    {
                        m_smVisionInfo.g_objSeal.SetScratchesLowThreshold(j, m_smVisionInfo.g_intLowThresholdValue);
                        m_smVisionInfo.g_objSeal.SetOverHeatLowThreshold(j, m_smVisionInfo.g_intLowThresholdValue);
                        m_smVisionInfo.g_objSeal.SetOverHeatHighThreshold(j, m_smVisionInfo.g_intHighThresholdValue);
                    }
                }
                else
                {
                    m_smVisionInfo.g_objSeal.SetScratchesLowThreshold(m_smVisionInfo.g_intSelectedROI, m_smVisionInfo.g_intLowThresholdValue);
                    m_smVisionInfo.g_objSeal.SetOverHeatLowThreshold(m_smVisionInfo.g_intSelectedROI, m_smVisionInfo.g_intLowThresholdValue);
                    m_smVisionInfo.g_objSeal.SetOverHeatHighThreshold(m_smVisionInfo.g_intSelectedROI, m_smVisionInfo.g_intHighThresholdValue);
                }
            }
            objThresholdForm.Dispose();
        }

        private void txt_OverHeatMinArea_TextChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone || txt_OverHeatMinArea.Text == "" || txt_OverHeatMinArea.Text == null)
            //    return;

            //try
            //{
            //    // 2020 01 07 - CCENG: Convert from Integer to float will cause missing value.
            //    m_smVisionInfo.g_objSeal.ref_intOverHeatMinArea = (int)Math.Floor(Convert.ToSingle(txt_OverHeatMinArea.Text) * m_smVisionInfo.g_fCalibPixelY * m_smVisionInfo.g_fCalibPixelY);
            //    m_smVisionInfo.g_objSeal.ref_fOverHeatAreaMinTolerance = Convert.ToSingle(txt_OverHeatMinArea.Text);
            //}
            //catch
            //{
            //}
        }

        private void Btn_SaveMark_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            m_smVisionInfo.g_strVisionFolderName + "\\";

            CopyFiles m_objCopy = new CopyFiles();
            string strCurrentDateTIme = DateTime.Now.ToString();
            DateTime DT = Convert.ToDateTime(strCurrentDateTIme);
            string strCurrentDateTIme1 = strCurrentDateTIme.Replace(':', '.');
            string strCurrentDateTIme2 = strCurrentDateTIme.ToString();
            //DeviceEdit objDeviceEdit = new DeviceEdit(m_smProductionInfo);
            string strImagePath = m_smProductionInfo.g_strHistoryDataLocation + "Data\\" + DT.ToString("yyyy-MM") + "\\" + strCurrentDateTIme1 + "\\" + m_strSelectedRecipe + "-" + m_smVisionInfo.g_strVisionFolderName + "-Learn Seal Mark\\";

            if (File.Exists(strPath + "Seal\\Template\\Mark\\MarkTemplate0_" + m_intMarkSelectedIndex + ".bmp"))
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Seal Mark", "MarkTemplate0_" + m_intMarkSelectedIndex + ".bmp", "MarkTemplate0_" + m_intMarkSelectedIndex + ".bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);
            else
                STDeviceEdit.SaveDeviceEditLogForImage(m_smVisionInfo.g_strVisionFolderName + ">Learn Template", m_smVisionInfo.g_strVisionFolderName + ">Learn Seal Mark", "", "MarkTemplate0_" + m_intMarkSelectedIndex + ".bmp", strCurrentDateTIme2, m_smProductionInfo.g_strLotID);

            m_objCopy.CopyAllImageFiles(strPath + "Seal\\Template\\Mark\\", strImagePath + "Old\\");

            // Delete previous template
            if (chk_DeleteAllMarkTemplates.Checked)
            {
                m_smVisionInfo.g_intMarkTemplateTotal = 0;
                m_smVisionInfo.g_intMarkTemplateMask = 0;
                DisposePictureBoxImage();
                DeleteAllPreviousSealTemplate(strPath + "Seal\\Template\\Mark\\");
            }

            DisposePictureBoxImage();
            
            STDeviceEdit.CopySettingFile(strPath, "Seal\\ROI.xml");
            //ROI.SaveFile(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs);
            SaveROI(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs, 5, 1);
            SaveROI(strPath + "Seal\\ROI.xml", m_smVisionInfo.g_arrSealROIs, 5, 2);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Seal ROI", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_objSeal.SetTemplateMarkROIPositionAndSize(m_smVisionInfo.g_arrSealROIs[5][2]);
            m_smVisionInfo.g_objSeal.SaveSeal(strPath + "Seal\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);
            SaveGeneralSettings(strPath, true, false);
            m_smVisionInfo.g_objSeal.SaveSealMarkTemplate4Direction(strPath + "Seal\\Template\\Mark\\", m_smVisionInfo.g_arrSealROIs[5][2]);
            m_smVisionInfo.g_objSeal.LoadMarkPattern4Direction(strPath + "Seal\\Template\\Mark\\");
            m_smVisionInfo.g_objSeal.LoadMarkPatternImage(strPath + "Seal\\Template\\Mark\\");

            m_objCopy.CopyAllImageFiles(strPath + "Seal\\Template\\Mark\\", strImagePath + "New\\");

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;
            m_smVisionInfo.g_blnUpdateZoom = true;
            Close();
            Dispose();
        }
        private static void SaveROI(string strFilePath, List<List<ROI>> arrROIList, int ROIParentIndex, int intROIChildIndex)
        {
            if (arrROIList.Count <= ROIParentIndex)
                return;

            XmlParser objFile = new XmlParser(strFilePath);

            objFile.WriteSectionElement("Unit" + ROIParentIndex, false);

            for (int j = 0; j < arrROIList[ROIParentIndex].Count; j++)
            {
                if (intROIChildIndex == j)
                {
                    objFile.WriteElement1Value("ROI" + j, "");

                    objFile.WriteElement2Value("Name", arrROIList[ROIParentIndex][j].ref_strROIName);
                    objFile.WriteElement2Value("Type", arrROIList[ROIParentIndex][j].ref_intType);
                    objFile.WriteElement2Value("PositionX", arrROIList[ROIParentIndex][j].ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", arrROIList[ROIParentIndex][j].ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", arrROIList[ROIParentIndex][j].ref_ROIWidth);
                    objFile.WriteElement2Value("Height", arrROIList[ROIParentIndex][j].ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", arrROIList[ROIParentIndex][j].ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", arrROIList[ROIParentIndex][j].ref_intStartOffsetY);
                    if (arrROIList[ROIParentIndex][j].ref_intType == 1)
                    {
                        float fPixelAverage = arrROIList[ROIParentIndex][j].GetROIAreaPixel();
                        objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                        arrROIList[ROIParentIndex][j].SetROIPixelAverage(fPixelAverage);
                    }

                    arrROIList[ROIParentIndex][j].ref_ROIOriPositionX = arrROIList[ROIParentIndex][j].ref_ROIPositionX;
                    arrROIList[ROIParentIndex][j].ref_ROIOriPositionY = arrROIList[ROIParentIndex][j].ref_ROIPositionY;
                }
            }

            objFile.WriteEndElement();
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

            if (m_intBackToStep1)
            {
                m_intBackToStep1 = false;
                m_smVisionInfo.g_intLearnStepNo = 1;
                if (SetupStep())
                {
                    m_intDisplayStepNo = 2;
                    lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
                }
            }

            if (m_objAdvancedForm != null)
            {
                if (m_objAdvancedForm.ref_blnBuildGauge)
                {
                    m_objAdvancedForm.ref_blnBuildGauge = false;
                    LoadGaugeGlobalSetting();
                    AddGauge();
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                }
            }

            if (m_objAdvancedCircleGaugeForm != null)
            {
                if (m_objAdvancedCircleGaugeForm.ref_blnShowForm)
                {
                    if (m_objAdvancedCircleGaugeForm.ref_blnMeasureGauge)
                    {
                        SetCircleGaugePlace();
                        m_objAdvancedCircleGaugeForm.ref_blnMeasureGauge = false;
                        m_smVisionInfo.g_objSealCircleGauges.Measure(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    }
                }
                else
                {
                    SetCircleGaugePlace();
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    m_objAdvancedCircleGaugeForm.Close();
                    m_objAdvancedCircleGaugeForm.Dispose();
                    m_objAdvancedCircleGaugeForm = null;

                    btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_CircleGaugeSetting.Enabled = chk_ShowDraggingBox.Enabled = chk_ShowSamplePoints.Enabled = true;
                }
            }

            if (m_intVisionType == 2)   // Learn Seal Mark 
            {
                if (m_smVisionInfo.g_arrSealROIs != null && m_smVisionInfo.g_arrSealROIs.Count > 5 && m_smVisionInfo.g_arrSealROIs[5].Count > 2)
                {
                    if (m_objThresholdImage == null)
                        m_objThresholdImage = new ImageDrawing(true, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight);

                    if (m_smVisionInfo.g_objSeal.ref_intCheckMarkMethod == 3)
                    {
                        if (m_smVisionInfo.g_objSeal.GetTemplateImageProcessingSeq().Count == 0)
                            m_smVisionInfo.g_arrSealROIs[5][2].ThresholdTo_ROIToImage(ref m_objThresholdImage, m_smVisionInfo.g_objSeal.GetMarkTemplateThreshold());
                        else
                        {
                            if (m_objROI == null)
                                m_objROI = new ROI();
                            m_objROI.LoadROISetting(m_smVisionInfo.g_arrSealROIs[5][2].ref_ROITotalX, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROITotalY, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight);
                            m_objROI.AttachImage(m_objThresholdImage);
                            m_smVisionInfo.g_arrSealROIs[5][2].CopyImage(ref m_objROI);
                            m_smVisionInfo.g_objSeal.DoImageProcessingSequence(ref m_objROI, m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex);
                        }
                    }
                    else
                        m_smVisionInfo.g_arrSealROIs[5][2].ThresholdTo_ROIToImage(ref m_objThresholdImage, m_smVisionInfo.g_objSeal.GetMarkTemplateThreshold());

                    int Max = Math.Max((int)(m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIWidth), (int)(m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight));
                    //m_Graphic.Clear(pic_ThresholdImage.BackColor);
                    //m_objThresholdImage.RedrawImage(m_Graphic, pic_ThresholdImage.Size.Width / Max2, pic_ThresholdImage.Size.Height / Max2);

                    pic_ThresholdImage.Size = new Size(Math.Min((int)Math.Ceiling((150f / Max) * (m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIWidth)), 150), Math.Min((int)Math.Ceiling((150f / Max) * (m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight)), 150));

                    m_objThresholdImage.SetImageSize(m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIWidth, m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight);
                    panel1.Height = Math.Min((int)Math.Ceiling((150f / Max) * (m_smVisionInfo.g_arrSealROIs[5][2].ref_ROIHeight)), 150);

                    m_objThresholdImage.RedrawImage(m_Graphic, 150f / Max, 150f / Max);

                    pic_ThresholdImage.Location = new Point(panel1.Size.Width / 2 - pic_ThresholdImage.Width / 2, panel1.Size.Height / 2 - pic_ThresholdImage.Height / 2);


                }
            }
        }

        private void btn_DistanceThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 3;
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intDistanceThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[3][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.ref_intDistanceThreshold = m_smVisionInfo.g_intThresholdValue;
            }
            objThresholdForm.Dispose();
        }

        private void txt_ShiftTolerance_TextChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSeal.ref_fShiftPositionTolerance = Convert.ToSingle(txt_ShiftTolerance.Text) * m_smVisionInfo.g_fCalibPixelX;
        }

        private void txt_SealScoreTolerance_TextChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSeal.ref_fSealScoreTolerance = Convert.ToSingle(txt_SealScoreTolerance.Text) / 100.0f;
        }

        private void txt_PitchPerSproket_TextChanged(object sender, EventArgs e)
        {

        }

        private void chk_SkipPocketTemplate_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chk_SkipMark_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_PadAdvancedSettings_Click(object sender, EventArgs e)
        {

        }

        private void btn_RotateUnit_Click(object sender, EventArgs e)
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;
            btn_RotateAngle.Enabled = false;

            ROI objROI = new ROI();
            objROI = m_smVisionInfo.g_arrSealROIs[5][0];

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[1];
            m_smVisionInfo.g_arrImages[1].CopyTo(ref objRotatedImage);

            // Rotate Unit IC
            if (m_intOrientDirection == 4)
            {
                if (sender == btn_ClockWise)
                    m_intCurrentAngle += 270;
                else
                    m_intCurrentAngle += 90;

                m_intCurrentAngle %= 360;
            }
            else
            {
                m_intCurrentAngle += 180;
                m_intCurrentAngle %= 360;
            }

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                ROI.RotateROI(objROI, m_intCurrentAngle, ref m_smVisionInfo.g_arrRotatedImages, i);
                // After rotating the image, attach the rotated image into ROI again
                m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[i]);
            }

            if (m_smVisionInfo.g_blnViewSealImage)
            {
                m_smVisionInfo.g_arrRotatedImages[1].CopyTo(ref m_smVisionInfo.g_objSealImage);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;
        }

        private void btn_RotateAngle_Click(object sender, EventArgs e)
        {
            if (txt_RotateAngle.Text == null || txt_RotateAngle.Text == "")
                return;

            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;
            btn_RotateAngle.Enabled = false;

            ROI objROI = new ROI();
            objROI = m_smVisionInfo.g_arrSealROIs[5][0];

            ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[1];
            m_smVisionInfo.g_arrImages[1].CopyTo(ref objRotatedImage);

            for (int i = 1; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
            {
                ROI.Rotate0Degree(objROI, m_intCurrentAngle - float.Parse(txt_RotateAngle.Text), 8, ref m_smVisionInfo.g_arrRotatedImages, i);
                // After rotating the image, attach the rotated image into ROI again
                m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[i]);
            }

            if (m_smVisionInfo.g_blnViewSealImage)
            {
                m_smVisionInfo.g_arrRotatedImages[1].CopyTo(ref m_smVisionInfo.g_objSealImage);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
            btn_RotateAngle.Enabled = true;
        }

        private void srmLabel32_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void tp_Step8_Click(object sender, EventArgs e)
        {

        }

        private void SetCircleGaugePlace()
        {
            if (m_smVisionInfo.g_arrSealROIs[6].Count > 0)
                m_smVisionInfo.g_objSealCircleGauges.SetGaugePlacement(m_smVisionInfo.g_arrSealROIs[6][0], (int)m_smVisionInfo.g_objSealCircleGauges.ref_GaugeTolerance, m_smVisionInfo.g_objSeal.ref_intSpocketHolePosition == 0);
        }

        private void btn_CircleGaugeSetting_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath +
                            m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Seal\\CircleGauge.xml";

            m_objAdvancedCircleGaugeForm = new AdvancedSealCircleGaugeForm(m_smVisionInfo, strPath, m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);

            m_objAdvancedCircleGaugeForm.Show();

            btn_Next.Enabled = btn_Previous.Enabled = btn_Cancel.Enabled = btn_CircleGaugeSetting.Enabled = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSealCircleGauges.ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSealCircleGauges.ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_AddDontCareROI_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_arrSealDontCareROIs.Add(new ROI());
            m_smVisionInfo.g_arrSealDontCareROIs[m_smVisionInfo.g_arrSealDontCareROIs.Count - 1].AttachImage(m_smVisionInfo.g_arrSealROIs[5][1]);   // Attach Dont care roi to pocket ROI
            m_smVisionInfo.g_arrPolygon_Seal.Add(new Polygon());
            m_smVisionInfo.g_arrPolygon_Seal[m_smVisionInfo.g_arrPolygon_Seal.Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrSealDontCareROIs.Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void btn_DeleteDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrSealDontCareROIs.Count != m_smVisionInfo.g_arrPolygon_Seal.Count)
            {
                m_smVisionInfo.g_arrPolygon_Seal.Clear();
                m_smVisionInfo.g_arrSealDontCareROIs.Clear();

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;  // 2020 03 30 - CCENG: update drawing after g_arrSealDontCareROIs is clear();
            }

            if (m_smVisionInfo.g_arrSealDontCareROIs.Count == 0)
            {
                return;
            }

            if (m_smVisionInfo.g_intSelectedDontCareROIIndex == -1)
                return;

            m_smVisionInfo.g_arrSealDontCareROIs.RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_arrPolygon_Seal.RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrSealDontCareROIs.Count - 1;
            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if (m_smVisionInfo.g_arrSealDontCareROIs.Count > 0)
                {
                    cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_Seal[m_smVisionInfo.g_intSelectedTemplate].ref_intFormMode;
                }

            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_DontCareAreaDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void btn_UnitPresentThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 5;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(5);
            if (m_intCurrentPreciseDeg != 0)
                m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.GetMarkTemplateThreshold();
            m_smVisionInfo.g_fThresholdRelativeValue = m_smVisionInfo.g_objSeal.GetMarkTemplateThresholdRelative();

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, true, m_smVisionInfo.g_objSeal.GetTemplateWantAutoThresholdRelative(), m_smVisionInfo.g_arrSealROIs[5][2]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.SetTemplateWantAutoThresholdRelative(objThresholdForm.ref_blnWantAutoThreshold);
                m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(m_smVisionInfo.g_intThresholdValue);
                m_smVisionInfo.g_objSeal.SetMarkTemplateThresholdRelative(m_smVisionInfo.g_fThresholdRelativeValue);
            }
            objThresholdForm.Dispose();
        }

        private void btn_ImageProcessSeq_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 5;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(5);
            if (m_intCurrentPreciseDeg != 0)
                m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            int intPreviousThreshold = m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.GetMarkTemplateThreshold();

            ImageProcessingSequenceForm objImageProcessingSequenceForm = new ImageProcessingSequenceForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, m_intCurrentPreciseDeg != 0);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objImageProcessingSequenceForm.Location = new Point(resolution.Width - objImageProcessingSequenceForm.Size.Width, resolution.Height - objImageProcessingSequenceForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objImageProcessingSequenceForm.ShowDialog() == DialogResult.OK)
            {
                //m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(m_smVisionInfo.g_intThresholdValue);

            }
            else
            {
                //m_smVisionInfo.g_objSeal.SetMarkTemplateThreshold(intPreviousThreshold); 
                //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                //           m_smVisionInfo.g_strVisionFolderName + "\\";
                //m_smVisionInfo.g_objSeal.LoadSealImageProcessSeqOnly(strPath + "Seal\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX);
            }

            objImageProcessingSequenceForm.Dispose();
        }
        private void LoadSealPocketTemplate()
        {
            string strFolderName = "Seal";
            string strFileName = "PocketTemplate" + m_smVisionInfo.g_intSelectedGroup + "_";
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] +
          "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\Pocket\\";

            Label[] lbl_No = new Label[4];

            for (int i = 0; i < m_smVisionInfo.g_intMaxSealEmptyTemplate; i++)//4
            {
                lbl_No[i] = new Label();
                lbl_No[i].ForeColor = Color.Red;
                lbl_No[i].BackColor = Color.Black;
                lbl_No[i].Font = new Font(lbl_No[0].Font.FontFamily, 10);
                lbl_No[i].Text = (i + 1).ToString();
                lbl_No[i].Width = 15;
            }

            for (int j = 0; j < m_smVisionInfo.g_intMaxSealEmptyTemplate; j++)//4
            {
                pic_Template2[j] = new PictureBox();
                pic_Panel2[j] = new Panel();
                pic_Template2[j].BackColor = Color.Black;
                pic_Template2[j].BorderStyle = BorderStyle.FixedSingle;
                pic_Template2[j].Name = "Pocket" + j.ToString();
                pic_Template2[j].Size = new Size(85, 76);
                pic_Panel2[j].Size = new Size(87, 80);
                pic_Panel2[j].BackColor = Color.Lime;

                if (File.Exists(strFolderPath + strFileName + j.ToString() + ".bmp"))
                {
                    pic_Template2[j].Load(strFolderPath + strFileName + j.ToString() + ".bmp");
                    pic_Template2[j].SizeMode = PictureBoxSizeMode.Zoom;
                    lbl_No[j].ForeColor = Color.LightGreen;

                    if (j >= m_smVisionInfo.g_intPocketTemplateTotal)
                    {
                        m_smVisionInfo.g_intPocketTemplateTotal = j + 1;
                    }
                }

                switch (j)
                {
                    case 0:
                        pic_Template2[0].Location = new Point(4, 5);
                        pnl_Template2.Controls.Add(pic_Template2[0]);
                        break;
                    case 1:
                        pic_Template2[1].Location = new Point(92, 5);
                        pnl_Template2.Controls.Add(pic_Template2[1]);
                        break;
                    case 2:
                        pic_Template2[2].Location = new Point(181, 5);
                        pnl_Template2.Controls.Add(pic_Template2[2]);
                        break;
                    case 3:
                        pic_Template2[3].Location = new Point(270, 5);
                        pnl_Template2.Controls.Add(pic_Template2[3]);
                        break;
                    default:
                        break;
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_intMaxSealEmptyTemplate; i++)//4
            {
                if (pic_Template2[i].Bounds.Height > lbl_No[i].Height)
                    lbl_No[i].Location = new Point(pic_Template2[i].Location.X + 1, pic_Template2[i].Location.Y);

                pnl_Template2.Controls.Add(lbl_No[i]);
                lbl_No[i].BringToFront();
            }
            AutoSelectTemplate2();
        }
        private void LoadSealMarkTemplate()
        {
            string strFolderName = "Seal";
            string strFileName = "MarkTemplate" + m_smVisionInfo.g_intSelectedGroup + "_";
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] +
          "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + strFolderName + "\\Template\\Mark\\";

            Label[] lbl_No = new Label[8];

            for (int i = 0; i < m_smVisionInfo.g_intMaxSealMarkTemplate; i++)//8
            {
                lbl_No[i] = new Label();
                lbl_No[i].ForeColor = Color.Red;
                lbl_No[i].BackColor = Color.Black;
                lbl_No[i].Font = new Font(lbl_No[0].Font.FontFamily, 10);
                lbl_No[i].Text = (i + 1).ToString();
                lbl_No[i].Width = 15;
            }

            for (int j = 0; j < m_smVisionInfo.g_intMaxSealMarkTemplate; j++)//8
            {
                pic_Template[j] = new PictureBox();
                pic_Panel[j] = new Panel();
                pic_Template[j].BackColor = Color.Black;
                pic_Template[j].BorderStyle = BorderStyle.FixedSingle;
                pic_Template[j].Name = "Mark" + j.ToString();
                pic_Template[j].Size = new Size(85, 76);
                pic_Panel[j].Size = new Size(87, 80);
                pic_Panel[j].BackColor = Color.Lime;

                if (File.Exists(strFolderPath + strFileName + j.ToString() + ".bmp"))
                {
                    pic_Template[j].Load(strFolderPath + strFileName + j.ToString() + ".bmp");
                    pic_Template[j].SizeMode = PictureBoxSizeMode.Zoom;
                    lbl_No[j].ForeColor = Color.LightGreen;

                    if (j >= m_smVisionInfo.g_intMarkTemplateTotal)
                    {
                        m_smVisionInfo.g_intMarkTemplateTotal = j + 1;
                    }
                }

                switch (j)
                {
                    case 0:
                        pic_Template[0].Location = new Point(4, 5);
                        pnl_Template.Controls.Add(pic_Template[0]);
                        break;
                    case 1:
                        pic_Template[1].Location = new Point(92, 5);
                        pnl_Template.Controls.Add(pic_Template[1]);
                        break;
                    case 2:
                        pic_Template[2].Location = new Point(181, 5);
                        pnl_Template.Controls.Add(pic_Template[2]);
                        break;
                    case 3:
                        pic_Template[3].Location = new Point(270, 5);
                        pnl_Template.Controls.Add(pic_Template[3]);
                        break;
                    case 4:
                        pic_Template[4].Location = new Point(359, 5);
                        pnl_Template.Controls.Add(pic_Template[4]);
                        break;
                    case 5:
                        pic_Template[5].Location = new Point(448, 5);
                        pnl_Template.Controls.Add(pic_Template[5]);
                        break;
                    case 6:
                        pic_Template[6].Location = new Point(537, 5);
                        pnl_Template.Controls.Add(pic_Template[6]);
                        break;
                    case 7:
                        pic_Template[7].Location = new Point(626, 5);
                        pnl_Template.Controls.Add(pic_Template[7]);
                        break;
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_intMaxSealMarkTemplate; i++)//8
            {
                if (pic_Template[i].Bounds.Height > lbl_No[i].Height)
                    lbl_No[i].Location = new Point(pic_Template[i].Location.X + 1, pic_Template[i].Location.Y);

                pnl_Template.Controls.Add(lbl_No[i]);
                lbl_No[i].BringToFront();
            }

            AutoSelectTemplate();
        }

        private void PocketTemplate_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox clickedPictureBox = sender as PictureBox;
            int temphold = m_intPocketSelectedIndex;

            for (int i = 1; i < m_smVisionInfo.g_intMaxSealEmptyTemplate; i++)//4
            {
                if (pic_Panel2[i].BackColor == Color.Red)
                    return;
            }

            for (int i = 0; i < m_smVisionInfo.g_intMaxSealEmptyTemplate; i++)//4
            {
                pic_Panel2[i].Location = new Point(pic_Template2[i].Location.X - 1, pic_Template2[i].Location.Y - 2);
                pnl_Template2.Controls.Add(pic_Panel2[i]);
                pic_Panel2[i].SendToBack();
            }

            for (int j = 0; j < m_smVisionInfo.g_intMaxSealEmptyTemplate; j++)//4
            {
                pic_Panel2[j].Visible = false;
                if (clickedPictureBox.Name.ToString().Contains(j.ToString()))
                    m_intPocketSelectedIndex = j;
            }


            for (int i = 0; i < m_smVisionInfo.g_intMaxSealEmptyTemplate; i++)//4
            {
                if (m_intPocketSelectedIndex == 0)
                    break;

                else if (m_intPocketSelectedIndex == i && pic_Template2[i - 1].Image == null)
                {
                    //m_intPocketSelectedIndex = 0;
                    m_intPocketSelectedIndex = temphold;
                    m_smVisionInfo.g_intPocketTemplateIndex = /*m_smVisionInfo.g_intPocketTemplateTotal =*/ m_intPocketSelectedIndex;
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
                    pic_Panel2[m_intPocketSelectedIndex].Visible = true;
                    return;
                }
            }


            m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex;
            m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
            pic_Panel2[m_intPocketSelectedIndex].Visible = true;
        }
        private void MarkTemplate_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox clickedPictureBox = sender as PictureBox;
            int temphold = m_intMarkSelectedIndex;

            for (int i = 1; i < m_smVisionInfo.g_intMaxSealMarkTemplate; i++)//8
            {
                if (pic_Panel[i].BackColor == Color.Red)
                    return;
            }

            for (int i = 0; i < m_smVisionInfo.g_intMaxSealMarkTemplate; i++)//8
            {
                pic_Panel[i].Location = new Point(pic_Template[i].Location.X - 1, pic_Template[i].Location.Y - 2);
                pnl_Template.Controls.Add(pic_Panel[i]);
                pic_Panel[i].SendToBack();
            }

            for (int j = 0; j < m_smVisionInfo.g_intMaxSealMarkTemplate; j++)//8
            {
                pic_Panel[j].Visible = false;
                if (clickedPictureBox.Name.ToString().Contains(j.ToString()))
                    m_intMarkSelectedIndex = j;
            }


            for (int i = 0; i < m_smVisionInfo.g_intMaxSealMarkTemplate; i++)//8
            {
                if (m_intMarkSelectedIndex == 0)
                    break;

                else if (m_intMarkSelectedIndex == i && pic_Template[i - 1].Image == null)
                {
                    m_intMarkSelectedIndex = temphold;
                    m_smVisionInfo.g_intMarkTemplateIndex = /*m_smVisionInfo.g_intMarkTemplateTotal =*/ m_intMarkSelectedIndex;
                    m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                    pic_Panel[m_intMarkSelectedIndex].Visible = true;
                    return;
                }
            }

            m_smVisionInfo.g_intMarkTemplateIndex = /*m_smVisionInfo.g_intMarkTemplateTotal =*/ m_intMarkSelectedIndex;
            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
            pic_Panel[m_intMarkSelectedIndex].Visible = true;
        }

        private void AutoSelectTemplate2()
        {
            for (int j = 0; j < m_smVisionInfo.g_intMaxSealEmptyTemplate; j++)//4
            {
                pic_Panel2[j].Location = new Point(pic_Template2[j].Location.X - 1, pic_Template2[j].Location.Y - 2);
                pic_Panel2[j].Visible = false;
                pnl_Template2.Controls.Add(pic_Panel2[j]);
                pic_Panel2[j].SendToBack();
            }

            if (pic_Template2[0] != null && pic_Template2[0].Image == null)
            {
                m_intPocketSelectedIndex = 0;
                pic_Panel2[0].Visible = true;
                m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex = Math.Max(0, m_smVisionInfo.g_intPocketTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
                return;
            }
            else if (pic_Template2[1] != null && pic_Template2[1].Image == null && pic_Template2[2] != null && pic_Template2[2].Image == null && pic_Template2[3] != null && pic_Template2[3].Image == null)
            {
                m_intPocketSelectedIndex = 0;
                pic_Panel2[0].Visible = true;
                m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex = Math.Max(0, m_smVisionInfo.g_intPocketTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
                return;
            }
            else if (pic_Template2[1] != null && pic_Template2[1].Image != null && pic_Template2[0] != null && pic_Template2[0].Image != null && pic_Template2[2] != null && pic_Template2[2].Image == null)
            {
                m_intPocketSelectedIndex = 1;
                pic_Panel2[1].Visible = true;
                m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex = Math.Max(0, m_smVisionInfo.g_intPocketTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
                return;
            }
            else if (pic_Template2[1] != null && pic_Template2[1].Image != null && pic_Template2[2] != null && pic_Template2[2].Image != null && pic_Template2[3] != null && pic_Template2[3].Image == null)
            {
                m_intPocketSelectedIndex = 2;
                pic_Panel2[2].Visible = true;
                m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex = Math.Max(0, m_smVisionInfo.g_intPocketTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
                return;
            }
            else if (pic_Template2[0] != null && pic_Template2[0].Image != null && pic_Template2[1] != null && pic_Template2[1].Image != null && pic_Template2[2] != null && pic_Template2[2].Image != null && pic_Template2[3] != null && pic_Template2[3].Image != null)
            {
                m_intPocketSelectedIndex = 3;
                pic_Panel2[3].Visible = true;
                m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex = Math.Max(0, m_smVisionInfo.g_intPocketTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
                return;
            }
            else
            {
                m_intPocketSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMaxSealEmptyTemplate - 1);
                pic_Panel2[Math.Max(0, m_smVisionInfo.g_intMaxSealEmptyTemplate - 1)].Visible = true;
                m_smVisionInfo.g_intPocketTemplateIndex /*= m_smVisionInfo.g_intPocketTemplateTotal*/ = m_intPocketSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMaxSealEmptyTemplate - 1);
                m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = m_smVisionInfo.g_intPocketTemplateIndex;
                return;
            }
        }

        private void AutoSelectTemplate()
        {
            for (int j = 0; j < m_smVisionInfo.g_intMaxSealMarkTemplate; j++)//8
            {
                pic_Panel[j].Location = new Point(pic_Template[j].Location.X - 1, pic_Template[j].Location.Y - 2);
                pic_Panel[j].Visible = false;
                pnl_Template.Controls.Add(pic_Panel[j]);
                pic_Panel[j].SendToBack();
            }

            if (pic_Template[0] != null && pic_Template[0].Image == null)
            {
                m_intMarkSelectedIndex = 0;
                pic_Panel[0].Visible = true;
                m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                return;
            }
            else if (pic_Template[1] != null && pic_Template[1].Image == null && pic_Template[2] != null && pic_Template[2].Image == null && pic_Template[3] != null && pic_Template[3].Image == null)
            {
                m_intMarkSelectedIndex = 0;
                pic_Panel[0].Visible = true;
                m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                return;
            }
            else if (pic_Template[1] != null && pic_Template[1].Image != null && pic_Template[0] != null && pic_Template[0].Image != null && pic_Template[2] != null && pic_Template[2].Image == null)
            {
                m_intMarkSelectedIndex = 1;
                pic_Panel[1].Visible = true;
                m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                return;
            }
            else if (pic_Template[1] != null && pic_Template[1].Image != null && pic_Template[2] != null && pic_Template[2].Image != null && pic_Template[3] != null && pic_Template[3].Image == null)
            {
                m_intMarkSelectedIndex = 2;
                pic_Panel[2].Visible = true;
                m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                return;
            }
            else if (pic_Template[2] != null && pic_Template[2].Image != null && pic_Template[3] != null && pic_Template[3].Image != null && pic_Template[4] != null && pic_Template[4].Image == null)
            {
                m_intMarkSelectedIndex = 3;
                pic_Panel[3].Visible = true;
                m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                return;
            }
            else
            {
                if (m_smVisionInfo.g_intMaxSealMarkTemplate <= 4)
                {
                    m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMaxSealMarkTemplate - 1);
                    pic_Panel[Math.Max(0, m_smVisionInfo.g_intMaxSealMarkTemplate - 1)].Visible = true;
                    m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMaxSealMarkTemplate - 1);
                    m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                    return;
                }

                for (int i = 4; i < m_smVisionInfo.g_intMaxSealMarkTemplate; i++)//8
                {
                    pic_Panel[i].Visible = false;
                    if (i == 4)
                    {
                        if (pic_Template[5] == null)
                        {
                            m_intMarkSelectedIndex = 0;
                            pic_Panel[m_intMarkSelectedIndex].Visible = true;
                            m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                            return;
                        }


                        if (pic_Template[5].Image == null && pic_Template[4].Image != null)
                        {
                            m_intMarkSelectedIndex = 4;
                            pic_Panel[m_intMarkSelectedIndex].Visible = true;
                            m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                            return;
                        }
                        else if (pic_Template[5].Image != null && pic_Template[4].Image != null)
                        {
                            if (pic_Template[6] == null)
                                continue;
                            else if (pic_Template[6].Image == null)
                            {
                                m_intMarkSelectedIndex = 5;
                                pic_Panel[m_intMarkSelectedIndex].Visible = true;
                                m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                                return;
                            }
                            else
                                continue;
                        }


                        if (pic_Template[3].Image != null)
                        {
                            if (pic_Template[4].Image != null)
                            {
                                m_intMarkSelectedIndex = i;
                                pic_Panel[m_intMarkSelectedIndex].Visible = true;
                                m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                                return;
                            }
                            else
                            {
                                m_intMarkSelectedIndex = i - 1;
                                pic_Panel[m_intMarkSelectedIndex].Visible = true;
                                m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                                m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                                return;
                            }
                        }

                        else
                        {
                            m_intMarkSelectedIndex = 2;
                            pic_Panel[2].Visible = true;
                            m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                            return;
                        }
                    }
                    else
                    {
                        if (pic_Template[7].Image != null && pic_Template[6].Image != null)
                        {
                            m_intMarkSelectedIndex = 0;
                            m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                        }
                        else if (pic_Template[i].Image != null && pic_Template[i - 1].Image != null)
                        {
                            m_intMarkSelectedIndex = i;
                            m_smVisionInfo.g_intMarkTemplateIndex /*= m_smVisionInfo.g_intMarkTemplateTotal*/ = m_intMarkSelectedIndex = Math.Max(0, m_smVisionInfo.g_intMarkTemplateTotal - 1);
                            m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex = m_smVisionInfo.g_intMarkTemplateIndex;
                        }
                    }
                }
                pic_Panel[m_intMarkSelectedIndex].Visible = true;
            }
        }

        private void DisposePictureBoxImage2()
        {
            for (int i = 0; i < 4; i++)
            {
                if (pic_Template2[i] == null)
                    return;

                if (pic_Template2[i].Image != null)
                {
                    pic_Template2[i].Dispose();
                    pic_Template2[i] = null;
                }

                if (i == 0)
                    continue;
                else
                {
                    if (Oribmp[i] != null)
                    {
                        bmp[i].Dispose();
                        bmp[i] = null;
                        Oribmp[i].Dispose();
                        Oribmp[i] = null;
                    }
                }
            }
        }
        private void DisposePictureBoxImage()
        {
            for (int i = 0; i < 8; i++)
            {
                if (pic_Template[i] == null)
                    return;

                if (pic_Template[i].Image != null)
                {
                    pic_Template[i].Dispose();
                    pic_Template[i] = null;
                }

                if (i == 0)
                    continue;
                else
                {
                    if (Oribmp[i] != null)
                    {
                        bmp[i].Dispose();
                        bmp[i] = null;
                        Oribmp[i].Dispose();
                        Oribmp[i] = null;
                    }
                }
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int j = 0; j < m_smVisionInfo.g_intMaxSealMarkTemplate; j++)//8
            {
                this.pic_Template[j].MouseClick += new MouseEventHandler(this.MarkTemplate_MouseClick);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int j = 0; j < m_smVisionInfo.g_intMaxSealEmptyTemplate; j++)//4
            {
                this.pic_Template2[j].MouseClick += new MouseEventHandler(this.PocketTemplate_MouseClick);
            }

        }

        private void btn_SprocketHoleDefectThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            ImageDrawing objImg_Temp = new ImageDrawing(true, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight);

            float fScale = Math.Max(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter - m_smVisionInfo.g_objSeal.ref_intSprocketHoleInspectionAreaInwardTolerance, 5) / m_smVisionInfo.g_objSealCircleGauges.ref_fTemplateObjectDiameter;

            ImageDrawing.ScaleImage(m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage, ref objImg_Temp, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth / 2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight / 2,
                                  objImg_Temp.ref_intImageWidth / 2, objImg_Temp.ref_intImageHeight / 2,
                                  fScale, fScale);

            ROI objCircleROI1 = new ROI();
            objCircleROI1.AttachImage(objImg_Temp);
            objCircleROI1.LoadROISetting((int)Math.Round((objImg_Temp.ref_intImageWidth / 2) - (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter / 2)),
                                                (int)Math.Round((objImg_Temp.ref_intImageHeight / 2) - (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter / 2)),
                                                (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter),
                                                (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter));

            ROI objCircleROI2 = new ROI();
            objCircleROI2.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objCircleROI2.LoadROISetting((int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX - (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter / 2)),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY - (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter / 2)),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter));

            ROI.LogicOperationAddROI(objCircleROI1, objCircleROI2);

            STTrackLog.WriteLine("SprocketHoleDefectThreshold Click A. ref_intSprocketHoleDefectThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold.ToString());
            m_smVisionInfo.g_intSelectedUnit = 6;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold;
            m_smVisionInfo.g_blnViewSprocketHoleDefectThreshold = true;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objCircleROI1);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                STTrackLog.WriteLine("SprocketHoleDefectThreshold Click B. ref_intSprocketHoleDefectThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold.ToString());
                m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold = m_smVisionInfo.g_intThresholdValue;
            }

            STTrackLog.WriteLine("SprocketHoleDefectThreshold Click C. ref_intSprocketHoleDefectThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleDefectThreshold.ToString());
            objThresholdForm.Dispose();
            objCircleROI2.Dispose();
            objCircleROI1.Dispose();
            objImg_Temp.Dispose();
            m_smVisionInfo.g_blnViewSprocketHoleDefectThreshold = false;
        }

        private void btn_SprocketHoleBrokenThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            ImageDrawing objImg_Temp = new ImageDrawing(true, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight);

            float fScale = (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / m_smVisionInfo.g_objSealCircleGauges.ref_fTemplateObjectDiameter;

            ImageDrawing.ScaleImage(m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage, ref objImg_Temp, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth / 2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight / 2,
                                  m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX, m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY,
                                  fScale, fScale);

            ROI objCircleROI1 = new ROI();
            objCircleROI1.AttachImage(objImg_Temp);
            objCircleROI1.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));

            ROI objCircleROI2 = new ROI();
            objCircleROI2.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objCircleROI2.LoadROISetting((int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                        (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                        (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));

            ROI.SubtractROI2(objCircleROI2, objCircleROI1);

            ImageDrawing objImg_Temp2 = new ImageDrawing(true, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight);

            fScale = (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / m_smVisionInfo.g_objSealCircleGauges.ref_fTemplateObjectDiameter;

            ImageDrawing.ScaleImage(m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage, ref objImg_Temp2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth / 2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight / 2,
                                  m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX, m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY,
                                  fScale, fScale);

            objCircleROI1.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)));
            objCircleROI2.AttachImage(objImg_Temp2);
            objCircleROI2.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)));

            ROI.LogicOperationBitwiseAndROI(objCircleROI1, objCircleROI2);

            objCircleROI1.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));


            STTrackLog.WriteLine("SprocketHoleBrokenThreshold Click A. ref_intSprocketHoleBrokenThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold.ToString());
            m_smVisionInfo.g_intSelectedUnit = 6;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold;
            m_smVisionInfo.g_blnViewSprocketHoleBrokenThreshold = true;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objCircleROI1);//m_smVisionInfo.g_arrSealROIs[6][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                STTrackLog.WriteLine("SprocketHoleBrokenThreshold Click B. ref_intSprocketHoleBrokenThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold.ToString());
                m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold = m_smVisionInfo.g_intThresholdValue;
            }

            STTrackLog.WriteLine("SprocketHoleBrokenThreshold Click C. ref_intSprocketHoleBrokenThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenThreshold.ToString());
            objThresholdForm.Dispose();
            objImg_Temp2.Dispose();
            objCircleROI2.Dispose();
            objCircleROI1.Dispose();
            objImg_Temp.Dispose();
            m_smVisionInfo.g_blnViewSprocketHoleBrokenThreshold = false;
        }

        private void btn_SprocketHoleRoundnessThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            ImageDrawing objImg_Temp = new ImageDrawing(true, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight);

            float fScale = (m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / m_smVisionInfo.g_objSealCircleGauges.ref_fTemplateObjectDiameter;

            ImageDrawing.ScaleImage(m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage, ref objImg_Temp, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageWidth / 2, m_smVisionInfo.g_objSeal.ref_objTemplateCircleImage.ref_intImageHeight / 2,
                                  m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX, m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY,
                                  fScale, fScale);

            ROI objCircleROI1 = new ROI();
            objCircleROI1.AttachImage(objImg_Temp);
            objCircleROI1.LoadROISetting((int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY) - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));

            ROI objCircleROI2 = new ROI();
            objCircleROI2.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objCircleROI2.LoadROISetting((int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterX - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                        (int)Math.Round(m_smVisionInfo.g_objSealCircleGauges.ref_ObjectCenterY - ((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                        (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                        (int)Math.Round((m_smVisionInfo.g_objSealCircleGauges.ref_fDiameter + m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)));

            ROI.SubtractROI2(objCircleROI2, objCircleROI1);

            STTrackLog.WriteLine("SprocketHoleRoundnessThreshold Click A. ref_intSprocketHoleRoundnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold.ToString());
            m_smVisionInfo.g_intSelectedUnit = 6;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objSeal.GetGrabImageIndex(6);
            m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold;
            m_smVisionInfo.g_blnViewSprocketHoleRoundnessThreshold = true;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objCircleROI1);//m_smVisionInfo.g_arrSealROIs[6][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                STTrackLog.WriteLine("SprocketHoleRoundnessThreshold Click B. ref_intSprocketHoleRoundnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold.ToString());
                m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold = m_smVisionInfo.g_intThresholdValue;
            }

            STTrackLog.WriteLine("SprocketHoleRoundnessThreshold Click C. ref_intSprocketHoleRoundnessThreshold = " + m_smVisionInfo.g_objSeal.ref_intSprocketHoleRoundnessThreshold.ToString());
            objThresholdForm.Dispose();
            objCircleROI2.Dispose();
            objCircleROI1.Dispose();
            objImg_Temp.Dispose();
            m_smVisionInfo.g_blnViewSprocketHoleRoundnessThreshold = false;
        }

        private void txt_SprocketHoleInspectionAreaInwardTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SprocketHoleInspectionAreaInwardTolerance.Text == "")
                return;

            m_smVisionInfo.g_objSeal.ref_intSprocketHoleInspectionAreaInwardTolerance = Convert.ToInt32(txt_SprocketHoleInspectionAreaInwardTolerance.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleInspectionAreaInwardTolerance_Enter(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleInspectionAreaInwardTolerance_Leave(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleBrokenOutwardTolerance_Outer_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SprocketHoleBrokenOutwardTolerance_Outer.Text == "")
                return;

            m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer = Convert.ToInt32(txt_SprocketHoleBrokenOutwardTolerance_Outer.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleBrokenOutwardTolerance_Inner_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_SprocketHoleBrokenOutwardTolerance_Inner.Text == "")
                return;

            if (Convert.ToInt32(txt_SprocketHoleBrokenOutwardTolerance_Inner.Text) > m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Outer)
            {
                SRMMessageBox.Show("Sprocket Hole Broken Outward Tolerance Inner Setting Cannot Larger Than Outer Setting.", "Error Setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_SprocketHoleBrokenOutwardTolerance_Inner.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner.ToString();
                return;
            }

            m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner = Convert.ToInt32(txt_SprocketHoleBrokenOutwardTolerance_Inner.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleBrokenOutwardTolerance_Enter(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SprocketHoleBrokenOutwardTolerance_Leave(object sender, EventArgs e)
        {

            if (Convert.ToInt32(txt_SprocketHoleBrokenOutwardTolerance_Outer.Text) < m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner)
            {
                SRMMessageBox.Show("Sprocket Hole Broken Outward Tolerance Outer Setting Cannot Smaller Than Inner Setting.", "Error Setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_SprocketHoleBrokenOutwardTolerance_Outer.Text = m_smVisionInfo.g_objSeal.ref_intSprocketHoleBrokenOutwardTolerance_Inner.ToString();
                return;
            }


            //m_smVisionInfo.g_blnViewSealSprocketHoleBrokenOutwardTolerance = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_CounterClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg--;
            RotatePrecise();
        }

        private void btn_ClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg++;
            RotatePrecise();
        }
        private void RotatePrecise()
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;

            ROI objROI = new ROI();

            objROI = m_smVisionInfo.g_arrSealROIs[5][0];
            objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
          ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref objRotatedImage);
            ROI.Rotate0Degree(objROI, m_intCurrentAngle - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);

            if (m_smVisionInfo.g_blnViewSealImage)
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objSealImage);
            }
            objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                srmLabel51.Text = m_intCurrentPreciseDeg.ToString() + " deg";
            else
                srmLabel51.Text = m_intCurrentPreciseDeg.ToString() + " 度";
            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;
        }

        private void btn_SealEdgeStraightnessThreshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intSelectedUnit = 7;
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objSeal.ref_intSealEdgeStraightnessThreshold;

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrSealROIs[7][0]);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_objSeal.ref_intSealEdgeStraightnessThreshold = m_smVisionInfo.g_intThresholdValue;
            }
            objThresholdForm.Dispose();
        }

        private void btn_AddOverHeatROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrSealROIs[4].Count > 4)
            {
                SRMMessageBox.Show("Maximum Over Heat ROI allowed is 5 only!");
                return;
            }
            AddOverHeadROI(m_smVisionInfo.g_arrSealROIs[4].Count);
            m_smVisionInfo.g_intSelectedROI = m_smVisionInfo.g_arrSealROIs[4].Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DeleteOverHeatROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrSealROIs.Count <= 4 || m_smVisionInfo.g_arrSealROIs[4].Count == 1)
            {
                SRMMessageBox.Show("Minimum of 1 ROI is required!");
                return;
            }

            if (m_smVisionInfo.g_intSelectedROI < m_smVisionInfo.g_arrSealROIs[4].Count)
            {
                for (int i = m_smVisionInfo.g_intSelectedROI; i < m_smVisionInfo.g_arrSealROIs[4].Count - 1; i++)
                {
                    m_smVisionInfo.g_objSeal.SetOverHeatLowThreshold(i, m_smVisionInfo.g_objSeal.GetOverHeatLowThreshold(i + 1));
                    m_smVisionInfo.g_objSeal.SetOverHeatHighThreshold(i, m_smVisionInfo.g_objSeal.GetOverHeatHighThreshold(i + 1));
                    m_smVisionInfo.g_objSeal.SetOverHeatAreaMinTolerance(i, m_smVisionInfo.g_objSeal.GetOverHeatAreaMinTolerance(i + 1));
                    m_smVisionInfo.g_objSeal.SetOverHeatMinArea(i, m_smVisionInfo.g_objSeal.GetOverHeatMinArea(i + 1));
                    m_smVisionInfo.g_objSeal.SetScratchesLowThreshold(i, m_smVisionInfo.g_objSeal.GetScratchesLowThreshold(i + 1));
                    m_smVisionInfo.g_objSeal.SetScratchesAreaMinTolerance(i, m_smVisionInfo.g_objSeal.GetScratchesAreaMinTolerance(i + 1));
                    m_smVisionInfo.g_objSeal.SetScratchesMinArea(i, m_smVisionInfo.g_objSeal.GetScratchesMinArea(i + 1));
                }
                m_smVisionInfo.g_arrSealROIs[4].RemoveAt(m_smVisionInfo.g_intSelectedROI);
                m_smVisionInfo.g_intSelectedROI = m_smVisionInfo.g_arrSealROIs[4].Count - 1;
                grpbox_OverHeat.Size = new Size(grpbox_OverHeat.Size.Width, 15 + (m_smVisionInfo.g_arrSealROIs[4].Count * 35));

                for (int i = 0; i < m_smVisionInfo.g_arrSealROIs[4].Count; i++)
                {
                    if (i == 0)
                        m_smVisionInfo.g_arrSealROIs[4][i].ref_strROIName = "OverHeat";
                    else
                        m_smVisionInfo.g_arrSealROIs[4][i].ref_strROIName = "OverHeat " + (i + 1).ToString();
                }

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

            if (m_smVisionInfo.g_arrSealROIs.Count <= 4 || m_smVisionInfo.g_arrSealROIs[4].Count == 1)
            {
                chk_SetToAllOverHeatROI.Visible = false;
                m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAllOverHeatROI.Checked = false;
            }
        }

        private void chk_SetToAllOverHeatROI_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllOverHeatROI_SealLearn", chk_SetToAllOverHeatROI.Checked);
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAllOverHeatROI.Checked;
        }
    }
}