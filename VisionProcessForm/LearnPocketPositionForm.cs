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
using System.IO;

namespace VisionProcessForm
{
    public partial class LearnPocketPositionForm : Form
    {
        #region Member Variables
        private bool m_blnInitDone = false;
        private int m_intDisplayStepNo = 1;
        private int m_intUserGroup;
        private string m_strFolderPath;
        private string m_strSelectedRecipe;
        private string path;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private UserRight m_objUserRight = new UserRight();
        private ProductionInfo m_smProductionInfo;
        
        #endregion
        public LearnPocketPositionForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
            
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\PocketPosition\\";
            path = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\";

            DisableField2();
            AddROI("Search ROI", 1, 0);
            AddROI("Pattern ROI", 2, 1);
            AddROI("Pocket Gauge ROI", 2, 2);
            AddGauge(0);
            AddROI("Gauge ROI", 1, 3);
            AddGauge(1);
            UpdateGUI();

            m_blnInitDone = true;
        }

        private void DisableField()
        {
            string strChild1 = "Learn Page";
            string strChild2 = "";
            
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

            string strChild2 = "Learn Pocket Position Page";
            string strChild3 = "Save Button";

            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
            {
                btn_Save.Enabled = false;
            }

            strChild3 = "Pocket Gauge Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
            {
                chk_ShowDraggingBox_Pocket.Enabled = false;
                chk_ShowSamplePoints_Pocket.Enabled = false;
                gb_Polarity_Pocket.Enabled = false;
                gb_Search_Pocket.Enabled = false;
                txt_MeasThickness_Pocket.Enabled = false;
                trackBar_Thickness_Pocket.Enabled = false;
                txt_threshold_Pocket.Enabled = false;
                trackBar_Derivative_Pocket.Enabled = false;
                txt_MeasMinAmp_Pocket.Enabled = false;
                trackBar_MinAmp_Pocket.Enabled = false;
            }

            strChild3 = "Plate Gauge Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(strChild2, strChild3, m_smVisionInfo.g_intVisionNameNo))
            {
                chk_ShowDraggingBox_Plate.Enabled = false;
                chk_ShowSamplePoints_Plate.Enabled = false;
                gb_Polarity_Plate.Enabled = false;
                gb_Search_Plate.Enabled = false;
                txt_MeasThickness_Plate.Enabled = false;
                trackBar_Thickness_Plate.Enabled = false;
                txt_threshold_Plate.Enabled = false;
                trackBar_Derivative_Plate.Enabled = false;
                txt_MeasMinAmp_Plate.Enabled = false;
                trackBar_MinAmp_Plate.Enabled = false;
            }
        }
        private void UpdateGUI()
        {
            // Pocket Gauge
            if (m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeTransType == 0)
                radioBtn_BW_Pocket.Checked = true;
            else
                radioBtn_WB_Pocket.Checked = true;

            if (m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeTransChoice == 0)
                radioBtn_FromBegin_Pocket.Checked = true;
            else if (m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeTransChoice == 1)
                radioBtn_FromEnd_Pocket.Checked = true;
            else
                radioBtn_LargeAmplitude_Pocket.Checked = true;

            txt_MeasThickness_Pocket.Text = m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeThickness.ToString();
            trackBar_Thickness_Pocket.Value = Convert.ToInt32(txt_MeasThickness_Pocket.Text);

            txt_MeasMinAmp_Pocket.Text = m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeMinAmplitude.ToString();
            trackBar_MinAmp_Pocket.Value = Convert.ToInt32(txt_MeasMinAmp_Pocket.Text);

            txt_threshold_Pocket.Text = m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeThreshold.ToString();
            trackBar_Derivative_Pocket.Value = Convert.ToInt32(txt_threshold_Pocket.Text);

            // Plate Gauge
            if (m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeTransType == 0)
                radioBtn_BW_Plate.Checked = true;
            else
                radioBtn_WB_Plate.Checked = true;

            if (m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeTransChoice == 0)
                radioBtn_FromBegin_Plate.Checked = true;
            else if (m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeTransChoice == 1)
                radioBtn_FromEnd_Plate.Checked = true;
            else
                radioBtn_LargeAmplitude_Plate.Checked = true;

            txt_MeasThickness_Plate.Text = m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeThickness.ToString();
            trackBar_Thickness_Plate.Value = Convert.ToInt32(txt_MeasThickness_Plate.Text);

            txt_MeasMinAmp_Plate.Text = m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeMinAmplitude.ToString();
            trackBar_MinAmp_Plate.Value = Convert.ToInt32(txt_MeasMinAmp_Plate.Text);

            txt_threshold_Plate.Text = m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeThreshold.ToString();
            trackBar_Derivative_Plate.Value = Convert.ToInt32(txt_threshold_Plate.Text);
        }
        private void SetupSteps()
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {

                case 0: // Define Search ROI
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0);

                    lbl_TitleStep0.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step0;
                    
                    //AddROI("Pattern ROI", 1, 1);

                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;

                    btn_Previous.Enabled = false;
                    btn_Next.Enabled = true;
                    btn_Save.Visible = false;
                    break;

                case 1: // Define Pattern ROI
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0);
                    lbl_TitleStep1.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step1;

                    //AddROI("Pattern ROI", 1, 1);

                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;

                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    btn_Save.Visible = false;
                    break;

                case 2: // Define Pocket Gauge ROI
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0);
                    lbl_TitleStep2.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step2;
                    //AddROI("Gauge ROI", 1, 2);
                    //AddGauge(0);
                    //m_smVisionInfo.g_arrPocketPositionROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]); //m_smVisionInfo.g_intSelectedImage
                    if (m_smVisionInfo.g_blnWantUsePocketPattern)
                    {
                        m_smVisionInfo.g_objPocketPosition.LearnPattern(m_smVisionInfo.g_arrPocketPositionROIs[1]);

                        //#region This part is match pattern then directly place ROI on the pattern center
                        //if (m_smVisionInfo.g_objPocketPosition.MatchPocketPattern(m_smVisionInfo.g_arrPocketPositionROIs[0]))
                        //{
                        //    m_smVisionInfo.g_arrPocketPositionROIs[2].SetPosition_Center(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX(),
                        //m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY());
                        //    m_smVisionInfo.g_arrPocketPositionGauges[0].SetGaugeCenter(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX(),
                        //        m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY());
                        //}
                        //#endregion
                 //       #region This part is place ROI based on the pattern and gauge offset

                 //       m_smVisionInfo.g_arrPocketPositionROIs[2].LoadROISetting((int)(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetX - (m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIWidth / 2)),
                 //(int)(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetY - (m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIHeight / 2)),
                 //m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIWidth, m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIHeight);

                 //       m_smVisionInfo.g_arrPocketPositionGauges[0].SetGaugeCenter(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetX,
                 //           m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetY);
                 //       #endregion
                    }
                    //if (m_smVisionInfo.g_objPocketPosition.MatchPocketPattern(m_smVisionInfo.g_arrPocketPositionROIs[0]))
                    //{
                    //    if (m_smVisionInfo.g_objPocketPosition.MeasurePlatePosition(m_smVisionInfo.g_arrImages[0], m_smVisionInfo.g_arrPocketPositionGauges))
                    //    {
                    //        lbl_PocketPositionTolerance.Text = m_smVisionInfo.g_objPocketPosition.ref_fResultXTolerance.ToString();
                    //    }
                    //    else
                    //    {
                    //        //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                    //        //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    //        lbl_PocketPositionTolerance.Text = "---";
                    //    }
                    //}
                    //else
                    //{
                    //    //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                    //    //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    //    lbl_PocketPositionTolerance.Text = "---";
                    //}

                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    btn_Save.Visible = false;
                    break;
                case 3: // Define Plate Gauge ROI
                    m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1);
                    lbl_TitleStep3.BringToFront();
                    tabCtrl_Setup.SelectedTab = tp_Step3;
                    //AddROI("Gauge ROI", 1, 2);
                    //AddGauge(0);
                    //m_smVisionInfo.g_arrPocketPositionROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]); //m_smVisionInfo.g_intSelectedImage
                    if (m_smVisionInfo.g_blnWantUsePocketPattern)
                        m_smVisionInfo.g_objPocketPosition.LearnPattern(m_smVisionInfo.g_arrPocketPositionROIs[1]);

                    m_smVisionInfo.g_objPocketPosition.SetPatternGaugeOffset(m_smVisionInfo.g_arrPocketPositionROIs[2] , m_smVisionInfo.g_arrPocketPositionROIs[1]);

                    Measure();

                    m_smVisionInfo.g_blnViewGauge = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = false;
                    btn_Save.Visible = true;
                    break;
         
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void Measure()
        {
            if (m_smVisionInfo.g_blnWantUsePocketPattern && !m_smVisionInfo.g_blnWantUsePocketGauge)
            {
                if (m_smVisionInfo.g_objPocketPosition.MatchPocketPattern(m_smVisionInfo.g_arrPocketPositionROIs[0]))
                {
                    if (m_smVisionInfo.g_objPocketPosition.MeasurePlatePosition(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1)], m_smVisionInfo.g_arrPocketPositionGauges, true,false))
                    {
                        lbl_PocketPositionTolerance.Text = (m_smVisionInfo.g_objPocketPosition.ref_fResultXDistance * 1000 / m_smVisionInfo.g_fCalibPixelX).ToString();
                    }
                    else
                    {
                        //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                        //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        lbl_PocketPositionTolerance.Text = "---";
                    }
                }
                else
                {
                    //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                    //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    lbl_PocketPositionTolerance.Text = "---";
                }

            }

            if (!m_smVisionInfo.g_blnWantUsePocketPattern && m_smVisionInfo.g_blnWantUsePocketGauge)
            {
                if (m_smVisionInfo.g_objPocketPosition.MeasurePocketPosition(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0)], m_smVisionInfo.g_arrPocketPositionGauges))
                {
                    if (m_smVisionInfo.g_objPocketPosition.MeasurePlatePosition(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1)], m_smVisionInfo.g_arrPocketPositionGauges,false, true))
                    {
                        lbl_PocketPositionTolerance.Text = m_smVisionInfo.g_objPocketPosition.ref_fResultXDistance.ToString();
                    }
                    else
                    {
                        //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                        //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        lbl_PocketPositionTolerance.Text = "---";
                    }
                }
                else
                {
                    //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                    //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    lbl_PocketPositionTolerance.Text = "---";
                }

            }

            if (m_smVisionInfo.g_blnWantUsePocketPattern && m_smVisionInfo.g_blnWantUsePocketGauge)
            {
                if (m_smVisionInfo.g_objPocketPosition.MatchPocketPattern(m_smVisionInfo.g_arrPocketPositionROIs[0]))
                {
                    //#region This part is match pattern then directly place ROI on the pattern center
                    //m_smVisionInfo.g_arrPocketPositionROIs[2].SetPosition_Center(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX(),
                    //    m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY());
                    //m_smVisionInfo.g_arrPocketPositionGauges[0].SetGaugeCenter(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX(),
                    //    m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY());
                    //#endregion

                    #region This part is place ROI based on the pattern and gauge offset

                    m_smVisionInfo.g_arrPocketPositionROIs[2].LoadROISetting((int)(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetX - (m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIWidth / 2)),
                   (int)(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetY - (m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIHeight / 2)),
                   m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIWidth, m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIHeight);

                    m_smVisionInfo.g_arrPocketPositionGauges[0].SetGaugeCenter(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetX,
                        m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetY);
                    #endregion

                    if (m_smVisionInfo.g_objPocketPosition.MeasurePocketPosition(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0)], m_smVisionInfo.g_arrPocketPositionGauges))
                    {
                        if (m_smVisionInfo.g_objPocketPosition.MeasurePlatePosition(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1)], m_smVisionInfo.g_arrPocketPositionGauges, true, true))
                        {
                            lbl_PocketPositionTolerance.Text = (m_smVisionInfo.g_objPocketPosition.ref_fResultXDistance * 1000 / m_smVisionInfo.g_fCalibPixelX).ToString();
                        }
                        else
                        {
                            //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                            //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            lbl_PocketPositionTolerance.Text = "---";
                        }
                    }
                    else
                    {
                        //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                        //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        lbl_PocketPositionTolerance.Text = "---";
                    }
                }
                else
                {
                    #region This part is place ROI based on the pattern and gauge offset

                    m_smVisionInfo.g_arrPocketPositionROIs[2].LoadROISetting((int)(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetX - (m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIWidth/2)),
                     (int)(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetY - (m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIHeight / 2)),
                     m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIWidth, m_smVisionInfo.g_arrPocketPositionROIs[2].ref_ROIHeight);

                    m_smVisionInfo.g_arrPocketPositionGauges[0].SetGaugeCenter(m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalX + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterX() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetX,
                        m_smVisionInfo.g_arrPocketPositionROIs[0].ref_ROITotalY + m_smVisionInfo.g_objPocketPosition.GetMatchingCenterY() + m_smVisionInfo.g_objPocketPosition.ref_fPatternGaugeOffsetY);
                    #endregion

                    if (m_smVisionInfo.g_objPocketPosition.MeasurePocketPosition(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0)], m_smVisionInfo.g_arrPocketPositionGauges))
                    {
                        if (m_smVisionInfo.g_objPocketPosition.MeasurePlatePosition(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1)], m_smVisionInfo.g_arrPocketPositionGauges, true, true))
                        {
                            lbl_PocketPositionTolerance.Text = (m_smVisionInfo.g_objPocketPosition.ref_fResultXDistance * 1000 / m_smVisionInfo.g_fCalibPixelX).ToString();
                        }
                        else
                        {
                            //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                            //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            lbl_PocketPositionTolerance.Text = "---";
                        }
                    }
                    else
                    {
                        //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPocketPosition.ref_strErrorMessage;
                        //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        lbl_PocketPositionTolerance.Text = "---";
                    }
                }

            }
        }

        private void AddGauge(int intIndex)
        {
            if ((m_smVisionInfo.g_arrPocketPositionGauges.Count - 1) < intIndex)
            {
                m_smVisionInfo.g_arrPocketPositionGauges.Add(new LGauge(m_smVisionInfo.g_WorldShape));
                m_smVisionInfo.g_arrPocketPositionGauges[intIndex].ref_GaugeAngle = 90;
                m_smVisionInfo.g_arrPocketPositionGauges[intIndex].ref_GaugeFilter = 1;
                m_smVisionInfo.g_arrPocketPositionGauges[intIndex].ref_GaugeFilterPasses = 3;
                m_smVisionInfo.g_arrPocketPositionGauges[intIndex].ref_GaugeFilterThreshold = 3;
                if (m_smVisionInfo.g_arrPocketPositionGauges[intIndex].ref_GaugeTransType > 1)
                    m_smVisionInfo.g_arrPocketPositionGauges[intIndex].ref_GaugeTransType = 0;
                m_smVisionInfo.g_arrPocketPositionGauges[intIndex].SetGaugePlacement(m_smVisionInfo.g_arrPocketPositionROIs[intIndex+2]);
            }
        }
        
        private void AddROI(string strROIName, int intROIType, int intArrayIndex)
        {
            if (intArrayIndex >= m_smVisionInfo.g_arrPocketPositionROIs.Count)
            {
                for (int i = m_smVisionInfo.g_arrPocketPositionROIs.Count; i <= intArrayIndex; i++)
                {
                    if (i == intArrayIndex)
                    {
                        ROI objROI = new ROI();
                        objROI.ref_strROIName = strROIName;
                        objROI.ref_intType = intROIType;
                        objROI.ref_ROIPositionX = 100;
                        objROI.ref_ROIPositionY = 100;
                        objROI.ref_ROIWidth = 100;
                        objROI.ref_ROIHeight = 100;
                        m_smVisionInfo.g_arrPocketPositionROIs.Add(objROI);
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPocketPositionROIs.Add(new ROI());
                    }
                }
            }

            if (m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_strROIName != strROIName)
            {
                m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_strROIName = strROIName;
            }

            if (m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_intType != intROIType)
            {
                m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_intType = intROIType;
            }


            if (strROIName == "Search ROI")
            {
                // Attach Search ROI to image
                m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0)]);
            }
            else if (strROIName == "Pattern ROI")
            {
              m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrPocketPositionROIs[0]);
            }
            else
            {
                m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_strROIName = strROIName;
                if (strROIName == "Pocket Gauge ROI")
                    m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0)]);
                else
                    m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(1)]);
                if (m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_ROIWidth == 0 || m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_ROIHeight == 0)
                {
                    m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_ROIWidth = 100;
                    m_smVisionInfo.g_arrPocketPositionROIs[intArrayIndex].ref_ROIHeight = 100;

                }
                
            }
        }
        private void LearnPositionForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blnDrawPocketPositionResult = false;
            lbl_StepNo.BringToFront();
            SetupSteps();

            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }
        private void LearnPositionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Learn Pocket Position Form Closed", "Exit Learn Pocket Position Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnDrawPocketPositionResult = false;
            m_smVisionInfo.g_blnViewEmptyImage = false;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_blnViewDontCareArea = false;
        }
        private void radioBtn_Polarity_Plate_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
            int intTransTypeIndex;
            if (radioBtn_BW_Plate.Checked)
                intTransTypeIndex = 0;
            else
                intTransTypeIndex = 1;

            m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeTransType = intTransTypeIndex;
            m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void radioBtn_Search_Plate_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
            int intTransChoiceIndex;
            if (radioBtn_FromBegin_Plate.Checked)
                intTransChoiceIndex = 0;
            else if (radioBtn_FromEnd_Plate.Checked)
                intTransChoiceIndex = 1;
            else
                intTransChoiceIndex = 2;
            
            m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeTransChoice = intTransChoiceIndex;
            m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void radioBtn_Polarity_Pocket_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTransTypeIndex;
            if (radioBtn_BW_Pocket.Checked)
                intTransTypeIndex = 0;
            else
                intTransTypeIndex = 1;

            m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeTransType = intTransTypeIndex;
            //m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void radioBtn_Search_Pocket_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTransChoiceIndex;
            if (radioBtn_FromBegin_Pocket.Checked)
                intTransChoiceIndex = 0;
            else if (radioBtn_FromEnd_Pocket.Checked)
                intTransChoiceIndex = 1;
            else
                intTransChoiceIndex = 2;

            m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeTransChoice = intTransChoiceIndex;
            //m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!m_smVisionInfo.g_objPocketPosition.ref_blnGaugeResult)
            {
                SRMMessageBox.Show("Please make sure gauge measurement is good!", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Save template images
            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objPocketPosition.GetGrabImageIndex(0)].SaveImage(m_strFolderPath + "\\Template\\OriTemplate.bmp");
            m_smVisionInfo.g_objPocketPosition.SetTemplateResult();
            // Save Setting
            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "Settings.xml");
            m_smVisionInfo.g_objPocketPosition.SavePocketPosition(m_strFolderPath + "Settings.xml", false, "Settings", true);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Pocket Position", m_smProductionInfo.g_strLotID);
            
            STDeviceEdit.CopySettingFile(m_strFolderPath, "ROI.xml");
            ROI.SaveFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPocketPositionROIs);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Pocket Position ROI", m_smProductionInfo.g_strLotID);

            STDeviceEdit.CopySettingFile(m_strFolderPath, "Gauge.xml");
            LGauge.SaveFile(m_strFolderPath + "\\Gauge.xml", m_smVisionInfo.g_arrPocketPositionGauges);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Pocket Position Gauge", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_objPocketPosition.LearnPattern(m_smVisionInfo.g_arrPocketPositionROIs[1]);
            m_smVisionInfo.g_objPocketPosition.SavePattern(m_strFolderPath + "\\Template\\Template0.mch");
            m_smVisionInfo.g_arrPocketPositionROIs[1].SaveImage(m_strFolderPath + "\\Template\\Template0.bmp"); 

            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPocketPosition.LoadPocketPosition(m_strFolderPath + "Settings.xml", "Settings");

           
                LGauge.LoadFile(m_strFolderPath + "\\Gauge.xml", m_smVisionInfo.g_arrPocketPositionGauges, m_smVisionInfo.g_WorldShape);
                ROI.LoadFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPocketPositionROIs);
          
                m_smVisionInfo.g_objPocketPosition.LoadPattern(m_strFolderPath + "\\Template\\Template0.mch");
                ROI.LoadFile(m_strFolderPath + "\\ROI.xml", m_smVisionInfo.g_arrPocketPositionROIs);
            
            Close();
            Dispose();
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {
           
                    m_smVisionInfo.g_intLearnStepNo--;

            if (m_smVisionInfo.g_blnWantUsePocketGauge && !m_smVisionInfo.g_blnWantUsePocketPattern && m_smVisionInfo.g_intLearnStepNo == 1)
                m_smVisionInfo.g_intLearnStepNo--;

            if (!m_smVisionInfo.g_blnWantUsePocketGauge && m_smVisionInfo.g_blnWantUsePocketPattern && m_smVisionInfo.g_intLearnStepNo == 2)
                m_smVisionInfo.g_intLearnStepNo--;

            m_intDisplayStepNo--;
            SetupSteps();

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {

                    m_smVisionInfo.g_intLearnStepNo++;
              
            if(m_smVisionInfo.g_blnWantUsePocketGauge && !m_smVisionInfo.g_blnWantUsePocketPattern && m_smVisionInfo.g_intLearnStepNo == 1)
                m_smVisionInfo.g_intLearnStepNo++;

            if (!m_smVisionInfo.g_blnWantUsePocketGauge && m_smVisionInfo.g_blnWantUsePocketPattern && m_smVisionInfo.g_intLearnStepNo == 2)
                m_smVisionInfo.g_intLearnStepNo++;

            m_intDisplayStepNo++;
            SetupSteps();

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasThickness_Plate_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness_Plate.Text);
            trackBar_Thickness_Plate.Value = m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeThickness;
            m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_threshold_Plate_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeThreshold = Convert.ToInt32(txt_threshold_Plate.Text);
            trackBar_Derivative_Plate.Value = m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeThreshold;
            m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinAmp_Plate_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp_Plate.Text);
            trackBar_MinAmp_Plate.Value = m_smVisionInfo.g_arrPocketPositionGauges[1].ref_GaugeMinAmplitude;
            m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Thickness_Plate_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            txt_MeasThickness_Plate.Text = trackBar_Thickness_Plate.Value.ToString();
        }

        private void trackBar_MinAmp_Plate_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            txt_MeasMinAmp_Plate.Text = trackBar_MinAmp_Plate.Value.ToString();
        }

        private void trackBar_Derivative_Plate_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            txt_threshold_Plate.Text = trackBar_Derivative_Plate.Value.ToString();
        }
        private void txt_MeasThickness_Pocket_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness_Pocket.Text);
            trackBar_Thickness_Pocket.Value = m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeThickness;
            //m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_threshold_Pocket_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeThreshold = Convert.ToInt32(txt_threshold_Pocket.Text);
            trackBar_Derivative_Pocket.Value = m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeThreshold;
            //m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinAmp_Pocket_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;


            m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp_Pocket.Text);
            trackBar_MinAmp_Pocket.Value = m_smVisionInfo.g_arrPocketPositionGauges[0].ref_GaugeMinAmplitude;
            //m_smVisionInfo.g_blnMeasurePocketPosition = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Thickness_Pocket_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            txt_MeasThickness_Pocket.Text = trackBar_Thickness_Pocket.Value.ToString();
        }

        private void trackBar_MinAmp_Pocket_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            txt_MeasMinAmp_Pocket.Text = trackBar_MinAmp_Pocket.Value.ToString();
        }

        private void trackBar_Derivative_Pocket_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            txt_threshold_Pocket.Text = trackBar_Derivative_Pocket.Value.ToString();
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

            if (m_smVisionInfo.g_blnMeasurePocketPosition)
            {
                m_smVisionInfo.g_blnMeasurePocketPosition = false;

                Measure();

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void chk_ShowDraggingBox_Plate_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPocketPosition.ref_blnDrawDraggingBox_Plate = chk_ShowDraggingBox_Plate.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Plate_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPocketPosition.ref_blnDrawSamplingPoint_Plate = chk_ShowSamplePoints_Plate.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }
        private void chk_ShowDraggingBox_Pocket_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPocketPosition.ref_blnDrawDraggingBox_Pocket = chk_ShowDraggingBox_Pocket.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Pocket_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objPocketPosition.ref_blnDrawSamplingPoint_Pocket = chk_ShowSamplePoints_Pocket.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }
    }
}
