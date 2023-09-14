using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;
using ImageAcquisition;
using Lighting;
using System.Threading;
using System.IO;

namespace VisionProcessForm
{


    public partial class CalibrationForm : Form
    {
        #region Member Variables
        private int m_intSelectedImage;
        private float m_fCameraShuttle;
        private List<float> m_arrCameraShuttle = new List<float>();
        private List<float> m_arrImageGain = new List<float>();
        private List<uint> m_arrCameraGain = new List<uint>();
        private List<string> m_arrstrCommPort = new List<string>();
        private List<string> m_arrstrType = new List<string>();
        private List<int> m_arrintChannel = new List<int>();
        private List<int> m_arrintValue = new List<int>();
        private List<int> m_arrintPortNo = new List<int>();
        private List<int> m_arrintSeqNo = new List<int>();
        private List<int> m_arrImageNo = new List<int>();
        private List<int> m_arrValue = new List<int>();
        private string m_strFilePath = "";

        private bool m_blnInitDone = false;
        private bool m_blnApplyOffsetValueDone = false;
        private int m_intThreshold = -4;
        private int m_intUserGroup = 5;
        private float m_fXResolutionPrev;
        private float m_fYResolutionPrev;
        private float m_fZResolutionPrev;
        private string m_strFolderPath = "";
        private string m_strSelectedRecipe = "Default";
        private bool m_blnCalibrationDone = false;

        private float m_MaxSizeX = -1;
        private float m_MaxSizeY = -1;

        private float m_MinSizeX = -1;
        private float m_MinSizeY = -1;

        private float m_fXResolution_Ori = 0;
        private float m_fYResolution_Ori = 0;

        //private float m_MaxCircleX = -1;
        //private float m_MaxCircleY = -1;

        //private float m_MinCircleX = -1;
        //private float m_MinCircleY = -1;

        private VisionInfo m_smVisionInfo;
        private Calibration m_objCalibrate = new Calibration();
        private UserRight m_objUserRight = new UserRight();
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private AVTVimba m_objAVTFireGrab;
        private IDSuEyeCamera m_objIDSCamera;
        private TeliCamera m_objTeliCamera;
        #endregion

        public CalibrationForm(VisionInfo smVisionInfo, string recipe, int grp, CustomOption smCustomizeInfo, AVTVimba objAVTFireGrab, IDSuEyeCamera objIDSCamera, TeliCamera objTeliCamera, ProductionInfo smProductionInfo)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            
            m_smVisionInfo.g_blncboImageView = false;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + recipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                m_strFilePath = m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Camera.xml";
            else
                m_strFilePath = m_strFolderPath + "Camera.xml";
            m_strSelectedRecipe = recipe;
            m_intUserGroup = grp;
            m_smCustomizeInfo = smCustomizeInfo;
            m_objAVTFireGrab = objAVTFireGrab;
            m_objIDSCamera = objIDSCamera;
            m_objTeliCamera = objTeliCamera;

            srmTabControl1.TabPages.Remove(tabPage_Calibration);
            srmTabControl1.TabPages.Remove(tabPage_Calibration4S);

            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            //Thread.Sleep(5);
            // m_smVisionInfo.AT_PR_StartLiveImage = true;
            // m_smVisionInfo.AT_PR_TriggerLiveImage = false;

            GetPreviousCameraValue();
            GetCalibrationCameraValue();
            GrabOneTime();

            //m_smVisionInfo.AT_PR_GrabImage = true;
            //Thread.Sleep(5);
            //m_smVisionInfo.AT_PR_GrabImage = false;

            DisableField2();
            //BuildROI();
            LoadSettings();
            LoadGlobalSettings();

            //Circle Gauge
            m_smVisionInfo.g_objCalibrateCircleGauge.SetGaugePlacement(m_smVisionInfo.g_objCalibrateROI);
            m_smVisionInfo.g_objCalibrateCircleGauge.Measure(m_smVisionInfo.g_objCalibrateROI);

            //Rectangle Gauge
            m_smVisionInfo.g_objCalibrateRectGauge.ModifyGauge(m_smVisionInfo.g_objCalibrateROI);
            m_smVisionInfo.g_objCalibrateRectGauge.Measure(m_smVisionInfo.g_objCalibrateROI);

            m_smVisionInfo.g_intCalibrationType = 0;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnDragROI = true;
            m_smVisionInfo.g_intSelectedImage = 0;  // Only image 1 will be used for calibration.
           
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;

            if (srmTabControl1.SelectedTab == tabPage_Calibration)
            {
                m_smVisionInfo.g_intCalibrationType = 0;
                m_smVisionInfo.g_blnDragROI = true;

            }
            else if (srmTabControl1.SelectedTab == tabPage_CalibrationByCircle)
            {
                chk_ShowDraggingBox.Checked = m_smVisionInfo.g_objCalibrateCircleGauge.ref_blnDrawDraggingBox;
                chk_ShowSamplePoints.Checked = m_smVisionInfo.g_objCalibrateCircleGauge.ref_blnDrawSamplingPoint;
                m_smVisionInfo.g_intCalibrationType = 2;
                m_smVisionInfo.g_blnDragROI = true;
            }
            else if (srmTabControl1.SelectedTab == tabPage_CalibrationByXY)
            {
                chk_ShowDraggingBox.Checked = m_smVisionInfo.g_objCalibrateRectGauge.ref_blnDrawDraggingBox;
                chk_ShowSamplePoints.Checked = m_smVisionInfo.g_objCalibrateRectGauge.ref_blnDrawSamplingPoint;
                m_smVisionInfo.g_intCalibrationType = 3;
                m_smVisionInfo.g_blnDragROI = true;
            }
            else if (srmTabControl1.SelectedTab == tabPage_Calibration4S)
            {
                m_smVisionInfo.g_intCalibrationType = 4;
                m_smVisionInfo.g_blnDragROI = true;
            }

            /*
             * Unit display as mm, not mil or micron because most of the jigs come with mm dimension.
             * 
             */
            m_blnInitDone = true;
            m_blnApplyOffsetValueDone = false;
            btn_Save.Enabled = false;
            btn_Set.Enabled = false;
        }

        private void GrabOneTime()
        {
            /////Set Gain
            UInt32 intGainValue = Convert.ToUInt32(m_smVisionInfo.g_arrCameraGain[0]);
            if (intGainValue <= 50)
            {
                //m_smVisionInfo.g_arrImageGain[0] = 1;
                if (m_objIDSCamera != null)
                {
                    m_objIDSCamera.SetGain((int)intGainValue);
                }
                else if (m_objAVTFireGrab != null)
                {

                    intGainValue = ((uint)Math.Round((float)intGainValue, 0, MidpointRounding.AwayFromZero));

                    m_objAVTFireGrab.SetCameraParameter(2, intGainValue);

                }
                else if (m_objTeliCamera != null)
                {

                    intGainValue = ((uint)Math.Round((float)intGainValue, 0, MidpointRounding.AwayFromZero));

                    m_objTeliCamera.SetCameraParameter(2, intGainValue);

                }
                m_smVisionInfo.g_arrCameraGain[0] = intGainValue;
            }
            else
            {
                m_smVisionInfo.g_arrImageGain[0] = 1 + (intGainValue - 50) * 0.18f;
            }

            ///////Set Shuttle
            float fShuttleValue = m_smVisionInfo.g_arrCameraShuttle[0] / 100;

            if (m_objIDSCamera != null)
            {
                //m_objIDSCamera.SetShuttle(Convert.ToSingle(((NumericUpDown)sender).Value));

                fShuttleValue = fShuttleValue * 0.1f;
            }
            else if (m_objAVTFireGrab != null)
            {
                fShuttleValue = fShuttleValue * 100;

                m_objAVTFireGrab.SetCameraParameter(1, (uint)fShuttleValue);

            }
            else if (m_objTeliCamera != null)
            {
                fShuttleValue = fShuttleValue * 100;

                m_objTeliCamera.SetCameraParameter(1, (uint)fShuttleValue);

            }
            m_smVisionInfo.g_arrCameraShuttle[0] = fShuttleValue;

            /////Set Light source
            //for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)      // 2019 05 10-CCENG: Bug: Should loop using image
            {

                if (m_smVisionInfo.g_intLightControllerType == 1)
                {
                    LightSource objLight = m_smVisionInfo.g_arrLightSource[i];
                    int intValue = objLight.ref_arrValue[0];

                    //if (m_smVisionInfo.g_arrImages.Count == 1)    // Prevent both side intensity during camera live (Warning: This condition should apply if intensity change during grab image)
                    {
                        if (m_smCustomizeInfo.g_blnLEDiControl)
                            LEDi_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, Convert.ToByte(intValue));
                        else
                            TCOSIO_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, intValue);
                    }
                }
                else
                {
                    if (m_smCustomizeInfo.g_blnLEDiControl)
                    {
                        m_smVisionInfo.AT_PR_PauseLiveImage = true;

                        while (m_smVisionInfo.g_blnGrabbing)
                        {
                            Thread.Sleep(1);
                        }

                        int intValue1 = 0, intValue2 = 0, intValue3 = 0, intValue4 = 0;
                        if (m_smVisionInfo.g_arrLightSource.Count > 0 && i < m_smVisionInfo.g_arrLightSource[0].ref_arrValue.Count)
                            intValue1 = m_smVisionInfo.g_arrLightSource[0].ref_arrValue[i];
                        if (m_smVisionInfo.g_arrLightSource.Count > 1 && i < m_smVisionInfo.g_arrLightSource[1].ref_arrValue.Count)
                            intValue2 = m_smVisionInfo.g_arrLightSource[1].ref_arrValue[i];
                        if (m_smVisionInfo.g_arrLightSource.Count > 2 && i < m_smVisionInfo.g_arrLightSource[2].ref_arrValue.Count)
                            intValue3 = m_smVisionInfo.g_arrLightSource[2].ref_arrValue[i];
                        if (m_smVisionInfo.g_arrLightSource.Count > 3 && i < m_smVisionInfo.g_arrLightSource[3].ref_arrValue.Count)
                            intValue4 = m_smVisionInfo.g_arrLightSource[3].ref_arrValue[i];


                        LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                        Thread.Sleep(10);
                        LEDi_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, i, intValue1, intValue2, intValue3, intValue4);
                        Thread.Sleep(10);
                        LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                        Thread.Sleep(100);
                        LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                        Thread.Sleep(10);
                        m_smVisionInfo.AT_PR_PauseLiveImage = false;
                    }
                }
            }
        }
        private void GetCalibrationCameraValue()
        {
            // Get LED-i light source controller's intensity setting from Option file
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);

            // Get camera grab delay, shuttle and gain setting from camera file to keep as previous setting

            objFile = new XmlParser(m_strFilePath);
            objFile.GetFirstSection(m_smVisionInfo.g_strVisionFolderName);

            m_smVisionInfo.g_arrCameraShuttle[0] = objFile.GetValueAsFloat("Shutter0", 200f);


            m_smVisionInfo.g_arrCameraGain[0] = objFile.GetValueAsUInt("Gain", 20);
            m_smVisionInfo.g_arrImageGain[0] = objFile.GetValueAsFloat("ImageGain", 1);
            for (int a = 0; a < m_smVisionInfo.g_arrLightSource.Count; a++)
            {
                objFile.GetSecondSection(m_smVisionInfo.g_arrLightSource[a].ref_strType);

                for (int j = 0; j < m_smVisionInfo.g_arrLightSource[a].ref_arrValue.Count; j++)
                    m_smVisionInfo.g_arrLightSource[a].ref_arrValue[j] = objFile.GetValueAsInt("Seq" + j.ToString(), 0, 2);
            }


        }

        private void GetPreviousCameraValue()
        {
            m_intSelectedImage = m_smVisionInfo.g_intSelectedImage;
            m_fCameraShuttle = m_smVisionInfo.g_fCameraShuttle;
            for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
                m_arrCameraShuttle.Add(m_smVisionInfo.g_arrCameraShuttle[i]);
            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
                m_arrImageGain.Add(m_smVisionInfo.g_arrImageGain[i]);
            for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
                m_arrCameraGain.Add(m_smVisionInfo.g_arrCameraGain[i]);


            for (int a = 0; a < m_smVisionInfo.g_arrLightSource.Count; a++)
            {
                m_arrstrCommPort.Add(m_smVisionInfo.g_arrLightSource[a].ref_strCommPort);
                m_arrstrType.Add(m_smVisionInfo.g_arrLightSource[a].ref_strType);
                m_arrintChannel.Add(m_smVisionInfo.g_arrLightSource[a].ref_intChannel);
                m_arrintValue.Add(m_smVisionInfo.g_arrLightSource[a].ref_intValue);
                m_arrintPortNo.Add(m_smVisionInfo.g_arrLightSource[a].ref_intPortNo);
                m_arrintSeqNo.Add(m_smVisionInfo.g_arrLightSource[a].ref_intSeqNo);

                for (int i = 0; i < m_smVisionInfo.g_arrLightSource[a].ref_arrImageNo.Count; i++)
                    m_arrImageNo.Add(m_smVisionInfo.g_arrLightSource[a].ref_arrImageNo[i]);

                for (int j = 0; j < m_smVisionInfo.g_arrLightSource[a].ref_arrValue.Count; j++)
                    m_arrValue.Add(m_smVisionInfo.g_arrLightSource[a].ref_arrValue[j]);
            }


        }

        private void SaveBackPreviousCameraValue()
        {
            m_smVisionInfo.g_intSelectedImage = m_intSelectedImage;
            m_smVisionInfo.g_fCameraShuttle = m_fCameraShuttle;
            for (int i = 0; i < m_arrCameraShuttle.Count; i++)
                m_smVisionInfo.g_arrCameraShuttle[i] = m_arrCameraShuttle[i];
            for (int i = 0; i < m_arrImageGain.Count; i++)
                m_smVisionInfo.g_arrImageGain[i] = m_arrImageGain[i];
            for (int i = 0; i < m_arrCameraGain.Count; i++)
                m_smVisionInfo.g_arrCameraGain[i] = m_arrCameraGain[i];

            int m_arrImageCount = 0;
            int m_arrValueCount = 0;
            for (int a = 0; a < m_smVisionInfo.g_arrLightSource.Count; a++)
            {
                m_smVisionInfo.g_arrLightSource[a].ref_strCommPort = m_arrstrCommPort[a];
                m_smVisionInfo.g_arrLightSource[a].ref_strType = m_arrstrType[a];
                m_smVisionInfo.g_arrLightSource[a].ref_intChannel = m_arrintChannel[a];
                m_smVisionInfo.g_arrLightSource[a].ref_intValue = m_arrintValue[a];
                m_smVisionInfo.g_arrLightSource[a].ref_intPortNo = m_arrintPortNo[a];
                m_smVisionInfo.g_arrLightSource[a].ref_intSeqNo = m_arrintSeqNo[a];

                for (int i = 0; i < m_smVisionInfo.g_arrLightSource[a].ref_arrImageNo.Count; i++)
                {
                    m_smVisionInfo.g_arrLightSource[a].ref_arrImageNo[i] = m_arrImageNo[m_arrImageCount];
                    m_arrImageCount++;
                }
                for (int j = 0; j < m_smVisionInfo.g_arrLightSource[a].ref_arrValue.Count; j++)
                {
                    m_smVisionInfo.g_arrLightSource[a].ref_arrValue[j] = m_arrValue[m_arrValueCount];
                    m_arrValueCount++;
                }
            }


        }

        /// <summary>
        /// Build calibrate ROI
        /// </summary>
        private void BuildROI()
        {
            m_smVisionInfo.g_objCalibrateROI = new ROI("Calibrate ROI", 1);

            int intX = 640 - (640 / 2) - 100;
            int intY = (480 / 2) - 100;

            m_smVisionInfo.g_objCalibrateROI.LoadROISetting(intX, intY, 200, 200);
            m_smVisionInfo.g_objCalibrateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
        }

        /// <summary>
        /// Perfrom grid calibration
        /// </summary>
        private void Calibrate()
        {
            int intMin = Convert.ToInt32(txt_MinArea.Text);
            int intMax = Convert.ToInt32(txt_MaxArea.Text);

            if (intMin > intMax)
            {
                SRMMessageBox.Show("Min area cannot bigger than max area. Please re-enter min max area values.");
                return;
            }

            if (intMin == 0 || intMax == 0 || float.Parse(txt_GridPitchX.Text) == 0.0 || float.Parse(txt_GridPitchY.Text) == 0)
            {
                SRMMessageBox.Show("Minimum Area, Maximum Area, Grid PitchX and Grid PitchY cannot be zero. Please enter values.");
                return;
            }

            if (m_objCalibrate.Calibrate(m_intThreshold, intMin, intMax, float.Parse(txt_GridPitchX.Text), float.Parse(txt_GridPitchY.Text),
                m_smVisionInfo.g_objCalibrateROI, m_smVisionInfo.g_intCalibrationMode))
            {

                if (radioBtn_pixelpermm.Checked)
                {
                    txt_XResolution.Text = (m_objCalibrate.ref_fPixelX + Convert.ToSingle(txt_WidthOffSet.Text)).ToString();
                    txt_YResolution.Text = (m_objCalibrate.ref_fPixelY + Convert.ToSingle(txt_HeightOffSet.Text)).ToString();

                    txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_YResolution.Text), 1).ToString();
                }
                else
                {
                    txt_XResolution.Text = (1 / (m_objCalibrate.ref_fPixelX + Convert.ToSingle(txt_WidthOffSet.Text))).ToString();
                    txt_YResolution.Text = (1 / (m_objCalibrate.ref_fPixelY + Convert.ToSingle(txt_HeightOffSet.Text))).ToString();

                    txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_YResolution.Text)), 1).ToString();
                }

                //txt_PixelSizeX.Text = (1 / Convert.ToSingle(txt_XResolution.Text)).ToString();
                //txt_PixelSizeY.Text = (1 / Convert.ToSingle(txt_YResolution.Text)).ToString();

                m_blnCalibrationDone = true;

                lbl_ResultStatus.Text = "Pass";
                lbl_ResultStatus.BackColor = Color.Lime;

                btn_Save.Enabled = true;
                btn_Set.Enabled = true;

                m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                m_smVisionInfo.g_strErrorMessage = "Calibration Pass!";
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;

            }
            else
            {
                lbl_ResultStatus.Text = "Fail";
                lbl_ResultStatus.BackColor = Color.Red;

                m_smVisionInfo.g_strErrorMessage = "Calibration Fail!";

                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;

                btn_Save.Enabled = false;
                btn_Set.Enabled = false;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        /// <summary>
        /// Perfrom circle calibration
        /// </summary>
        private void CalibrateCircle()
        {
            string strFailDetailMsg = "";
            float fRealDiameter = float.Parse(txt_Diameter.Text);

            bool blnResult = true;

            if (fRealDiameter == 0.0)
            {
                SRMMessageBox.Show("Diameter cannot be zero. Please enter values.");
                return;
            }

            //2021-06-31 ZJYEOH : Temporary use score to fail calibration
            if (m_smVisionInfo.g_objCalibrateCircleGauge.ref_GaugeScore < 70)
            {
                blnResult = false;
                strFailDetailMsg = "Gauge point measurement is poor.";
            }

            // ---------- check result gauge points at result gauge line or not
            if (blnResult)
            {
                if (m_smVisionInfo.g_objCalibrateCircleGauge.GetPreciseGaugeScore(m_smVisionInfo.g_arrImages[0]) < 80)
                {
                    blnResult = false;
                    strFailDetailMsg = "Gauge point measurement is not at result line.";
                }
            }

            if (blnResult)
            {
                if (radioBtn_pixelpermm.Checked)
                {
                    txt_XResolution.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateCircleGauge.ref_fDiameter / fRealDiameter + Convert.ToSingle(txt_WidthOffSet.Text));
                    txt_YResolution.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateCircleGauge.ref_fDiameter / fRealDiameter + Convert.ToSingle(txt_HeightOffSet.Text));

                    txt_MeasuredX.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateCircleGauge.ref_fDiameter / (Convert.ToDouble(txt_XResolution.Text) - Convert.ToSingle(txt_WidthOffSet.Text)));
                    txt_MeasuredY.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateCircleGauge.ref_fDiameter / (Convert.ToDouble(txt_YResolution.Text) - Convert.ToSingle(txt_HeightOffSet.Text)));

                    txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_YResolution.Text), 1).ToString();
                }
                else
                {
                    txt_XResolution.Text = Convert.ToString(1 / (m_smVisionInfo.g_objCalibrateCircleGauge.ref_fDiameter / fRealDiameter + Convert.ToSingle(txt_WidthOffSet.Text)));
                    txt_YResolution.Text = Convert.ToString(1 / (m_smVisionInfo.g_objCalibrateCircleGauge.ref_fDiameter / fRealDiameter + Convert.ToSingle(txt_HeightOffSet.Text)));

                    txt_MeasuredX.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateCircleGauge.ref_fDiameter / ((1 / Convert.ToDouble(txt_XResolution.Text)) - Convert.ToSingle(txt_WidthOffSet.Text)));
                    txt_MeasuredY.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateCircleGauge.ref_fDiameter / ((1 / Convert.ToDouble(txt_YResolution.Text)) - Convert.ToSingle(txt_HeightOffSet.Text)));

                    txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_YResolution.Text)), 1).ToString();
                }
                //txt_PixelSizeX.Text = (1 / Convert.ToSingle(txt_XResolution.Text)).ToString();
                //txt_PixelSizeY.Text = (1 / Convert.ToSingle(txt_YResolution.Text)).ToString();

                // ------------ Compare tolerance ------------------------------
                float X, Y;
                if (radioBtn_pixelpermm.Checked)
                {
                    X = 1f / Convert.ToSingle(txt_XResolution.Text);
                    Y = 1f / Convert.ToSingle(txt_YResolution.Text);
                }
                else
                {
                    X = Convert.ToSingle(txt_XResolution.Text);
                    Y = Convert.ToSingle(txt_YResolution.Text);
                }

                if (m_MaxSizeX == -1 || m_MinSizeX == -1 || m_MaxSizeY == -1 || m_MinSizeY == -1)
                {
                    if (SRMMessageBox.Show("No Factory Calibration Record in database. Would you like to store this calibration result as default?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SaveCurrentResultAsFactoryCalibrationRecord();
                        blnResult = true;
                    }
                    else
                    {
                        strFailDetailMsg = "No Factory Calibration record in database.";
                        blnResult = false;
                    }
                }
                //else if (X > m_MaxSizeX || X < m_MinSizeX || Y > m_MaxSizeY || Y < m_MinSizeY)
                else if (X > (m_fXResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) || 
                         X < (m_fXResolution_Ori - Convert.ToDouble(txt_CalTol.Text)) ||
                         Y > (m_fYResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                         Y < (m_fYResolution_Ori - Convert.ToDouble(txt_CalTol.Text)))
                {
                    strFailDetailMsg = "Result out of tolerance. Please make sure the Distance set value is correct.";
                    blnResult = false;
                }
            }

            if (blnResult)
            {
                m_blnCalibrationDone = true;

                lbl_ResultStatus.Text = "Pass";
                lbl_ResultStatus.BackColor = Color.Lime;

                btn_Save.Enabled = true;
                btn_Set.Enabled = true;
                m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                m_smVisionInfo.g_strErrorMessage = "Calibration Pass!";
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
            }
            else
            {
                lbl_ResultStatus.Text = "Fail";
                lbl_ResultStatus.BackColor = Color.Red;

                m_smVisionInfo.g_strErrorMessage = "Calibration Fail!" + "*" + strFailDetailMsg;

                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;

                btn_Save.Enabled = false;
                btn_Set.Enabled = false;
            }
        }

        private void CalibrateXY()
        {
            string strFailDetailMsg = "";
            bool blnResult = true;

            if ((float.Parse(txt_XDistance.Text) == 0.0f) || (float.Parse(txt_YDistance.Text) == 0.0f))
            {
                SRMMessageBox.Show("X Y Distance Text Box cannot be zero. Please enter values.");
                return;
            }

            if (blnResult)
            {
                if (chk_UseGauge1.Checked)
                {

                    if (radioBtn_pixelpermm.Checked)
                    {
                        txt_XResolution.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateRectGauge.ref_ObjectWidth / float.Parse(txt_XDistance.Text) + Convert.ToSingle(txt_WidthOffSet.Text));
                        txt_YResolution.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateRectGauge.ref_ObjectHeight / float.Parse(txt_YDistance.Text) + Convert.ToSingle(txt_HeightOffSet.Text));

                        txt_MeasuredX.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateRectGauge.ref_ObjectWidth / (Convert.ToDouble(txt_XResolution.Text) - Convert.ToSingle(txt_WidthOffSet.Text)));
                        txt_MeasuredY.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateRectGauge.ref_ObjectHeight / (Convert.ToDouble(txt_YResolution.Text) - Convert.ToSingle(txt_HeightOffSet.Text)));

                        txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_YResolution.Text), 1).ToString();
                    }
                    else
                    {
                        txt_XResolution.Text = Convert.ToString(1 / (m_smVisionInfo.g_objCalibrateRectGauge.ref_ObjectWidth / float.Parse(txt_XDistance.Text) + Convert.ToSingle(txt_WidthOffSet.Text)));
                        txt_YResolution.Text = Convert.ToString(1 / (m_smVisionInfo.g_objCalibrateRectGauge.ref_ObjectHeight / float.Parse(txt_YDistance.Text) + Convert.ToSingle(txt_HeightOffSet.Text)));

                        txt_MeasuredX.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateRectGauge.ref_ObjectWidth / ((1 / Convert.ToDouble(txt_XResolution.Text)) - Convert.ToSingle(txt_WidthOffSet.Text)));
                        txt_MeasuredY.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateRectGauge.ref_ObjectHeight / ((1 / Convert.ToDouble(txt_YResolution.Text)) - Convert.ToSingle(txt_HeightOffSet.Text)));

                        txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_YResolution.Text)), 1).ToString();
                    }
                }
                else
                {
                    if (radioBtn_pixelpermm.Checked)
                    {
                        txt_XResolution.Text = Convert.ToString((float)m_smVisionInfo.g_objCalibrateROI.ref_ROIWidth / float.Parse(txt_XDistance.Text) + Convert.ToSingle(txt_WidthOffSet.Text));
                        txt_YResolution.Text = Convert.ToString((float)m_smVisionInfo.g_objCalibrateROI.ref_ROIHeight / float.Parse(txt_YDistance.Text) + Convert.ToSingle(txt_HeightOffSet.Text));

                        txt_MeasuredX.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateROI.ref_ROIWidth / (Convert.ToDouble(txt_XResolution.Text) - Convert.ToSingle(txt_WidthOffSet.Text)));
                        txt_MeasuredY.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateROI.ref_ROIHeight / (Convert.ToDouble(txt_YResolution.Text) - Convert.ToSingle(txt_HeightOffSet.Text)));

                        txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_YResolution.Text), 1).ToString();
                    }
                    else
                    {
                        txt_XResolution.Text = Convert.ToString(1 / ((float)m_smVisionInfo.g_objCalibrateROI.ref_ROIWidth / float.Parse(txt_XDistance.Text) + Convert.ToSingle(txt_WidthOffSet.Text)));
                        txt_YResolution.Text = Convert.ToString(1 / ((float)m_smVisionInfo.g_objCalibrateROI.ref_ROIHeight / float.Parse(txt_YDistance.Text) + Convert.ToSingle(txt_HeightOffSet.Text)));

                        txt_MeasuredX.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateROI.ref_ROIWidth / ((1 / Convert.ToDouble(txt_XResolution.Text)) - Convert.ToSingle(txt_WidthOffSet.Text)));
                        txt_MeasuredY.Text = Convert.ToString(m_smVisionInfo.g_objCalibrateROI.ref_ROIHeight / ((1 / Convert.ToDouble(txt_YResolution.Text)) - Convert.ToSingle(txt_HeightOffSet.Text)));

                        txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_YResolution.Text)), 1).ToString();
                    }
                }
                //txt_PixelSizeX.Text = (1 / Convert.ToSingle(txt_XResolution.Text)).ToString();
                //txt_PixelSizeY.Text = (1 / Convert.ToSingle(txt_YResolution.Text)).ToString();

                // ------------ Compare tolerance ------------------------------
                float X, Y;
                if (chk_UseGauge1.Checked)
                {
                    if (radioBtn_pixelpermm.Checked)
                    {
                        X = 1f / Convert.ToSingle(txt_XResolution.Text);
                        Y = 1f / Convert.ToSingle(txt_YResolution.Text);
                    }
                    else
                    {
                        X = Convert.ToSingle(txt_XResolution.Text);
                        Y = Convert.ToSingle(txt_YResolution.Text);
                    }
                }
                else
                {
                    if (radioBtn_pixelpermm.Checked)
                    {
                        X = 1f / Convert.ToSingle(txt_XResolution.Text);
                        Y = 1f / Convert.ToSingle(txt_YResolution.Text);
                    }
                    else
                    {
                        X = Convert.ToSingle(txt_XResolution.Text);
                        Y = Convert.ToSingle(txt_YResolution.Text);
                    }
                }
                if (m_MaxSizeX == -1 || m_MinSizeX == -1 || m_MaxSizeY == -1 || m_MinSizeY == -1)
                {
                    if (SRMMessageBox.Show("No Factory Calibration Record in database. Would you like to store this calibration result as default?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SaveCurrentResultAsFactoryCalibrationRecord();
                        blnResult = true;
                    }
                    else
                    {
                        strFailDetailMsg = "No Factory Calibration record in database.";
                        blnResult = false;
                    }
                }
                //else if (!m_smVisionInfo.g_objCalibrateRectGauge.CalculateValidSamplePoint(50))
                else if (chk_UseGauge1.Checked && !m_smVisionInfo.g_objCalibrateRectGauge.CalculateValidSamplePoint(50))
                {
                    blnResult = false;
                    strFailDetailMsg = "Result not valid, Please Place The Rectangle Gauge at Correct Position";
                }
                //else if (X > m_MaxSizeX || X < m_MinSizeX || Y > m_MaxSizeY || Y < m_MinSizeY)
                else if (X > (m_fXResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                        X < (m_fXResolution_Ori - Convert.ToDouble(txt_CalTol.Text)) ||
                        Y > (m_fYResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                        Y < (m_fYResolution_Ori - Convert.ToDouble(txt_CalTol.Text)))
                {
                    strFailDetailMsg = "Result out of tolerance. Please make sure the Distance set value is correct.";
                    blnResult = false;
                }
            }
            if (blnResult)
            {
                m_blnCalibrationDone = true;

                lbl_ResultStatus.Text = "Pass";
                lbl_ResultStatus.BackColor = Color.Lime;

                btn_Save.Enabled = true;
                btn_Set.Enabled = true;
                m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                m_smVisionInfo.g_strErrorMessage = "Calibration Pass!";
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
            }
            else
            {
                lbl_ResultStatus.Text = "Fail";
                lbl_ResultStatus.BackColor = Color.Red;

                m_smVisionInfo.g_strErrorMessage = "Calibration Fail!" + "*" + strFailDetailMsg;

                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;

                btn_Save.Enabled = false;
                btn_Set.Enabled = false;
            }
        }

        private void CalibrateZ()
        {
            if (chk_UseGauge.Checked)
            {

            }
            else
            {
                float fZResolution;
                if (!chk_Rotate90Deg.Checked)    // check X
                {
                    fZResolution = (float)m_smVisionInfo.g_objCalibrateROI.ref_ROIWidth / float.Parse(txt_ZDistance.Text) + Convert.ToSingle(txt_ZOffSet.Text);
                    //txt_ZResolution.Text = Convert.ToString();
                }
                else // Check Y
                {
                    fZResolution = (float)m_smVisionInfo.g_objCalibrateROI.ref_ROIHeight / float.Parse(txt_ZDistance.Text) + Convert.ToSingle(txt_ZOffSet.Text);
                    //txt_ZResolution.Text = Convert.ToString();
                }

                if (radioBtn_pixelpermm.Checked)
                {
                    txt_ZResolution.Text = fZResolution.ToString();
                    txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_ZResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_ZResolution.Text), 1).ToString();
                }
                else
                {
                    txt_ZResolution.Text = (1f / fZResolution).ToString();
                    txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_ZResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_ZResolution.Text)), 1).ToString();
                }
            }
            m_blnCalibrationDone = true;

            lbl_ResultStatus.Text = "Pass";
            lbl_ResultStatus.BackColor = Color.Lime;

            btn_Save.Enabled = true;
            btn_Set.Enabled = true;
            m_smVisionInfo.g_cErrorMessageColor = Color.Black;
            m_smVisionInfo.g_strErrorMessage = "Calibration Pass!";
            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
        }

        /// <summary>
        /// Close this form and set back the variables default value
        /// </summary>
        private void CloseForm()
        {
            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_smVisionInfo.AT_PR_TriggerLiveImage = true;

            SaveBackPreviousCameraValue();
            GrabOneTime();
            SaveBackPreviousCameraValue();
            m_arrImageNo.Clear();
            m_arrValue.Clear();
            Thread.Sleep(5);
            m_smVisionInfo.AT_PR_GrabImage = true;
            Thread.Sleep(5);
            m_smVisionInfo.AT_PR_GrabImage = false;

            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_blncboImageView = true;
            this.Close();

            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewGauge = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
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
                btn_AdvancedSettings.Visible = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "Measurement Color Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_TransitionType.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "Measurement Position Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                cbo_TransitionChoice.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "Thickness Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_GaugeThickness.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "Threshold Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_GaugeThreshold.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "Circle Diameter Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_Diameter.Enabled = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "Camera Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Camera.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "Calibrate Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Calibrate.Enabled = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "X Distance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_XDistance.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "Y Distance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_YDistance.Enabled = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "Gauge Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_UseGauge1.Enabled = false;
            }

        }
        private int GetUserRightGroup_Child3(string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                case "BottomPosition":
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(Child2, Child3);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild3Group(Child2, Child3);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                //case "BottomOrientPad":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(Child2, Child3);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(Child2, Child3);
                    break;
                case "Seal":
                    return m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                case "Barcode":
                    return m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(Child2, Child3);
                    break;
            }

            return 1;
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Calibration Page";
            string strChild3 = "";

            strChild3 = "Advance Setting Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_AdvancedSettings.Visible = false;
            }

            strChild3 = "Circle Gauge Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                cbo_TransitionType.Enabled = false;
                cbo_TransitionChoice.Enabled = false;
                txt_GaugeThickness.Enabled = false;
                txt_GaugeThreshold.Enabled = false;
                txt_Diameter.Enabled = false;
                chk_ShowDraggingBox.Enabled = false;
                chk_ShowSamplePoints.Enabled = false;
            }

            strChild3 = "Camera Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Camera.Enabled = false;
            }

            strChild3 = "Calibrate Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Calibrate.Enabled = false;
            }


            strChild3 = "X Distance Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_XDistance.Enabled = false;
            }

            strChild3 = "Y Distance Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_YDistance.Enabled = false;
            }

            strChild3 = "Gauge Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                chk_UseGauge1.Enabled = false;
                btn_GaugeAdvanceSetting.Enabled = false;
            }

            strChild3 = "Resolution Offset";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                groupBox14.Enabled = false;
            }

            strChild3 = "Calibration Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                groupBox1.Enabled = false;
            }

        }
        /// <summary>
        /// Load calibration mode from xml
        /// </summary>
        private void LoadGlobalSettings()
        {
            XmlParser objFileHandle;
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                objFileHandle = new XmlParser(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                objFileHandle = new XmlParser(m_strFolderPath + "Calibration.xml");
            objFileHandle.GetFirstSection("Advanced");
            m_smVisionInfo.g_intCalibrationMode = objFileHandle.GetValueAsInt("Mode", 2);
        }

        /// <summary>
        /// Load calibration settings from xml
        /// </summary>
        private void LoadSettings()
        {
            XmlParser objXml;
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                objXml = new XmlParser(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                objXml = new XmlParser(m_strFolderPath + "Calibration.xml");

            objXml.GetFirstSection("Settings");
            txt_MinArea.Text = objXml.GetValueAsString("MinArea", "10");
            txt_MaxArea.Text = objXml.GetValueAsString("MaxArea", "1500");
            txt_GridPitchX.Text = objXml.GetValueAsString("PitchX", "5");
            txt_GridPitchY.Text = objXml.GetValueAsString("PitchY", "5");
            txt_Diameter.Text = objXml.GetValueAsString("Diameter", "0");
            txt_XDistance.Text = objXml.GetValueAsString("XDistance", "0");
            txt_YDistance.Text = objXml.GetValueAsString("YDistance", "0");
            srmTabControl1.SelectedIndex = objXml.GetValueAsInt("SelectedTab", 0);

            m_smVisionInfo.g_intThresholdValue = objXml.GetValueAsInt("Threshold", -4);
            if (m_smVisionInfo.g_intThresholdValue == 255)
                m_smVisionInfo.g_intThresholdValue = 254;

            cbo_TransitionChoice.SelectedItem = objXml.GetValueAsString("TransactionChoice", "From Begin");
            cbo_TransitionType.SelectedItem = objXml.GetValueAsString("TransactionType", "White to Black");
            txt_GaugeThickness.Text = objXml.GetValueAsString("Thickness", "13");
            txt_GaugeThreshold.Text = objXml.GetValueAsString("Threshold", "1");

            objXml.GetFirstSection("Calibrate");
            txt_XResolution.Text = objXml.GetValueAsString("PixelX", "5");
            txt_YResolution.Text = objXml.GetValueAsString("PixelY", "5");
            txt_ZResolution.Text = objXml.GetValueAsString("PixelZ", "5");
            txt_FOV.Text = objXml.GetValueAsString("FOV", "1x1");

            if (radioBtn_mmperPixel.Checked)
            {
                txt_XResolution.Text = (1 / Convert.ToSingle(txt_XResolution.Text)).ToString();
                txt_YResolution.Text = (1 / Convert.ToSingle(txt_YResolution.Text)).ToString();
                txt_ZResolution.Text = (1 / Convert.ToSingle(txt_ZResolution.Text)).ToString();
            }

            //if (radioBtn_pixelpermm.Checked)
            //{
            //    m_fXResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_XResolution.Text));
            //    m_fYResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_YResolution.Text));

            //}
            //else
            //{
            //    m_fXResolution_Ori = (float)Convert.ToDouble(txt_XResolution.Text);
            //    m_fYResolution_Ori = (float)Convert.ToDouble(txt_YResolution.Text);

            //}

            if (radioBtn_pixelpermm.Checked)
            {
                txt_WidthOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_XResolution.Text) * 0.1f);
                txt_HeightOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_YResolution.Text) * 0.1f);
            }
            else
            {
                txt_WidthOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_XResolution.Text)) * 0.1f);
                txt_HeightOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_YResolution.Text)) * 0.1f);
            }

            txt_WidthOffSet.Text = objXml.GetValueAsString("OffSetX", "0");
            txt_HeightOffSet.Text = objXml.GetValueAsString("OffSetY", "0");
            txt_ZOffSet.Text = objXml.GetValueAsString("OffSetZ", "0");

            if (radioBtn_pixelpermm.Checked)
            {
                m_fXResolutionPrev = Convert.ToSingle(txt_XResolution.Text) - Convert.ToSingle(txt_WidthOffSet.Text);
                m_fYResolutionPrev = Convert.ToSingle(txt_YResolution.Text) - Convert.ToSingle(txt_HeightOffSet.Text);
                m_fZResolutionPrev = Convert.ToSingle(txt_ZResolution.Text) - Convert.ToSingle(txt_ZOffSet.Text); ;
            }
            else
            {
                m_fXResolutionPrev = 1 / Convert.ToSingle(txt_XResolution.Text) - Convert.ToSingle(txt_WidthOffSet.Text);
                m_fYResolutionPrev = 1 / Convert.ToSingle(txt_YResolution.Text) - Convert.ToSingle(txt_HeightOffSet.Text);
                m_fZResolutionPrev = 1 / Convert.ToSingle(txt_ZResolution.Text) - Convert.ToSingle(txt_ZOffSet.Text); ;
            }
            objXml.GetFirstSection("Advanced");
            m_smVisionInfo.g_intCalibrationMode = objXml.GetValueAsInt("Mode", 2);

            objXml.GetFirstSection("FactoryCalibration");
            m_fXResolution_Ori = objXml.GetValueAsFloat("XResolution_Ori", 0);
            m_fYResolution_Ori = objXml.GetValueAsFloat("YResolution_Ori", 0);
            m_MaxSizeX = objXml.GetValueAsFloat("MaxSizeX", -1);
            m_MaxSizeY = objXml.GetValueAsFloat("MaxSizeY", -1);
            m_MinSizeX = objXml.GetValueAsFloat("MinSizeX", -1);
            m_MinSizeY = objXml.GetValueAsFloat("MinSizeY", -1);
            txt_CalTol.Text = objXml.GetValueAsFloat("CalibrateTolerance", 0.002f).ToString();

            // 2021 10 25 - CCENG: For old calibraton xml file, Resolution_Ori variable is not exist. 
            //                     If Resolution_Ori no exist, then Resolution_Ori will direct calculate from m_MaxSize and m_MinSizeX. 
            if (m_fXResolution_Ori == 0)
            {
                if (m_MaxSizeX != -1)
                {
                    m_fXResolution_Ori = (m_MaxSizeX + m_MinSizeX) / 2;
                    m_fYResolution_Ori = (m_MaxSizeY + m_MinSizeY) / 2;
                }
            }

            objXml.GetFirstSection("CalibrateROI");
            m_smVisionInfo.g_objCalibrateROI.ref_ROIPositionX = objXml.GetValueAsInt("PositionX", 0);
            m_smVisionInfo.g_objCalibrateROI.ref_ROIPositionY = objXml.GetValueAsInt("PositionY", 0);
            m_smVisionInfo.g_objCalibrateROI.ref_ROIWidth = objXml.GetValueAsInt("Width", 100);
            m_smVisionInfo.g_objCalibrateROI.ref_ROIHeight = objXml.GetValueAsInt("Height", 100);

            m_smVisionInfo.g_objCalibrateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

            LoadGaugeSetting(m_strFolderPath + "Gauge.xml");
        }

        private void SetUseGaugeGUI()
        {
            if (chk_UseGauge1.Checked)
            {
                btn_GaugeAdvanceSetting.Visible = true;
                chk_ShowDraggingBox.Visible = true;
                chk_ShowSamplePoints.Visible = true;
                m_smVisionInfo.g_blnViewGauge = true;
                m_smVisionInfo.g_blnViewPackageImage = true;
                if(m_smVisionInfo.g_objPackageImage!=null)
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(m_smVisionInfo.g_objCalibrateRectGauge.ref_fGainValue) / 1000);
            }
            else
            {
                btn_GaugeAdvanceSetting.Visible = false;
                chk_ShowDraggingBox.Visible = false;
                chk_ShowSamplePoints.Visible = false;
                m_smVisionInfo.g_blnViewGauge = false;
                m_smVisionInfo.g_blnViewPackageImage = false;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void LoadGaugeSetting(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);
            int intCount = objFile.GetFirstSectionCount();

            for (int j = 0; j < intCount; j++)
            {
                //create new ROI base on file read out
                m_smVisionInfo.g_objCalibrateRectGauge = new RectGauge(m_smVisionInfo.g_WorldShape);
                m_smVisionInfo.g_objCalibrateRectGauge.LoadGauge(strPath, "RectG" + j);
            }
        }

        private void SaveGaugeSettings(string strPath)
        {
            m_smVisionInfo.g_objCalibrateRectGauge.SaveGauge(strPath, false, "RectG0", true);
        }

        private void btn_AdvancedSettings_Click(object sender, EventArgs e)
        {
            AdvancedCalibrationForm objAdvance = new AdvancedCalibrationForm(m_smVisionInfo.g_strVisionFolderName, m_strSelectedRecipe, m_smProductionInfo, m_smVisionInfo);

            if (objAdvance.ShowDialog() == DialogResult.OK)
            {
                XmlParser objXml;
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                    objXml = new XmlParser(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
                else
                    objXml = new XmlParser(m_strFolderPath + "Calibration.xml");

                objXml.GetFirstSection("Advanced");
                m_smVisionInfo.g_intCalibrationMode = objXml.GetValueAsInt("Mode", 2);

            }
        }

        private void btn_Calibrate_Click(object sender, EventArgs e)
        {
            if (srmTabControl1.SelectedTab == tabPage_Calibration)
            {
                if (float.Parse(txt_GridPitchX.Text) > 0.0f && float.Parse(txt_GridPitchY.Text) > 0.0f &&
               Convert.ToInt32(txt_MaxArea.Text) > 0 && Convert.ToInt32(txt_MinArea.Text) > 0)
                    Calibrate();
            }
            else if (srmTabControl1.SelectedTab == tabPage_CalibrationByCircle)
            {
                if (float.Parse(txt_Diameter.Text) > 0.0f)
                    CalibrateCircle();
            }
            else if (srmTabControl1.SelectedTab == tabPage_CalibrationByXY)
            {
                if ((float.Parse(txt_XDistance.Text) > 0.0f) || (float.Parse(txt_YDistance.Text) > 0.0f))
                    CalibrateXY();
            }
            else if (srmTabControl1.SelectedTab == tabPage_Calibration4S)
            {
                float fValue;
                if (float.TryParse(txt_ZDistance.Text, out fValue))
                {
                    CalibrateZ();
                }
            }
            else if (srmTabControl1.SelectedTab == tp_Offset)
            {
                m_smVisionInfo.g_blnDragROI = true;
                if (m_smVisionInfo.g_intCalibrationType == 2)
                {
                    if (float.Parse(txt_Diameter.Text) > 0.0f)
                        CalibrateCircle();
                }
                else if (m_smVisionInfo.g_intCalibrationType == 3)
                {
                    if ((float.Parse(txt_XDistance.Text) > 0.0f) || (float.Parse(txt_YDistance.Text) > 0.0f))
                        CalibrateXY();
                }
            }
            if (radioBtn_pixelpermm.Checked)
            {
                m_fXResolutionPrev = Convert.ToSingle(txt_XResolution.Text) - Convert.ToSingle(txt_WidthOffSet.Text);
                m_fYResolutionPrev = Convert.ToSingle(txt_YResolution.Text) - Convert.ToSingle(txt_HeightOffSet.Text);
                m_fZResolutionPrev = Convert.ToSingle(txt_ZResolution.Text) - Convert.ToSingle(txt_ZOffSet.Text);

            }
            else
            {
                m_fXResolutionPrev = 1 / Convert.ToSingle(txt_XResolution.Text) - Convert.ToSingle(txt_WidthOffSet.Text);
                m_fYResolutionPrev = 1 / Convert.ToSingle(txt_YResolution.Text) - Convert.ToSingle(txt_HeightOffSet.Text);
                m_fZResolutionPrev = 1 / Convert.ToSingle(txt_ZResolution.Text) - Convert.ToSingle(txt_ZOffSet.Text);
            }


            if (radioBtn_pixelpermm.Checked)
            {
                txt_WidthOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_XResolution.Text) * 0.1f);
                txt_HeightOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_YResolution.Text) * 0.1f);
            }
            else
            {
                txt_WidthOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_XResolution.Text)) * 0.1f);
                txt_HeightOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_YResolution.Text)) * 0.1f);
            }

        }



        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            CloseForm();
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            ThresholdForm objThreshold = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_objCalibrateROI);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThreshold.Location = new Point(resolution.Width - objThreshold.Size.Width, resolution.Height - objThreshold.Size.Height);
            //objThreshold.Location = new Point(769, 310);

            if (objThreshold.ShowDialog() == DialogResult.OK)
            {
                if (m_smVisionInfo.g_intThresholdValue == 255)
                    m_smVisionInfo.g_intThresholdValue = 254;
                m_intThreshold = m_smVisionInfo.g_intThresholdValue;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (m_blnCalibrationDone)
            {
                if (!m_blnApplyOffsetValueDone)
                {
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        m_smVisionInfo.g_arrColorImages[0].SaveImage(m_strFolderPath + "CalibrationImage.bmp");
                    }
                    else
                    {
                        m_smVisionInfo.g_arrImages[0].SaveImage(m_strFolderPath + "CalibrationImage.bmp");
                    }
                }
                string strFileName;
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                    strFileName = m_smVisionInfo.g_strVisionFolderName;
                else
                    strFileName = "";

                
                STDeviceEdit.CopySettingFile(m_strFolderPath, strFileName + "Calibration.xml");

                //Save all calibration values into xml
                XmlParser objXml = new XmlParser(m_strFolderPath + strFileName + "Calibration.xml");

                objXml.WriteSectionElement("Settings");
                objXml.WriteElement1Value("MinArea", txt_MinArea.Text);
                objXml.WriteElement1Value("MaxArea", txt_MaxArea.Text);
                objXml.WriteElement1Value("PitchX", txt_GridPitchX.Text);
                objXml.WriteElement1Value("PitchY", txt_GridPitchY.Text);
                objXml.WriteElement1Value("Diameter", txt_Diameter.Text);
                objXml.WriteElement1Value("XDistance", txt_XDistance.Text);
                objXml.WriteElement1Value("YDistance", txt_YDistance.Text);
                objXml.WriteElement1Value("Threshold", m_smVisionInfo.g_intThresholdValue);
                objXml.WriteElement1Value("TransactionChoice", cbo_TransitionChoice.SelectedItem.ToString());
                objXml.WriteElement1Value("TransactionType", cbo_TransitionType.SelectedItem.ToString());
                objXml.WriteElement1Value("Thickness", txt_GaugeThickness.Text);
                objXml.WriteElement1Value("Threshold", txt_GaugeThreshold.Text);
                objXml.WriteElement1Value("SelectedTab", srmTabControl1.SelectedIndex.ToString());

                objXml.WriteSectionElement("Calibrate");
                objXml.WriteElement1Value("FOV", txt_FOV.Text);

                if (radioBtn_pixelpermm.Checked)
                {
                    objXml.WriteElement1Value("PixelX", txt_XResolution.Text);
                    objXml.WriteElement1Value("PixelY", txt_YResolution.Text);
                    objXml.WriteElement1Value("PixelZ", txt_ZResolution.Text);
                }
                else
                {
                    objXml.WriteElement1Value("PixelX", 1 / Convert.ToSingle(txt_XResolution.Text));
                    objXml.WriteElement1Value("PixelY", 1 / Convert.ToSingle(txt_YResolution.Text));
                    objXml.WriteElement1Value("PixelZ", 1 / Convert.ToSingle(txt_ZResolution.Text));
                }
                objXml.WriteElement1Value("OffSetX", txt_WidthOffSet.Text);
                objXml.WriteElement1Value("OffSetY", txt_HeightOffSet.Text);
                objXml.WriteElement1Value("OffSetZ", txt_ZOffSet.Text);

                //objXml.WriteSectionElement("OffSet");
                //objXml.WriteElement1Value("WidthOffSet", txt_WidthOffSet.Text);
                //objXml.WriteElement1Value("HeightOffSet", txt_HeightOffSet.Text);
                if (radioBtn_pixelpermm.Checked)
                {
                    m_smVisionInfo.g_fCalibPixelX = float.Parse(txt_XResolution.Text);
                    m_smVisionInfo.g_fCalibPixelY = float.Parse(txt_YResolution.Text);
                    m_smVisionInfo.g_fCalibPixelZ = float.Parse(txt_ZResolution.Text);
                }
                else
                {
                    m_smVisionInfo.g_fCalibPixelX = 1 / float.Parse(txt_XResolution.Text);
                    m_smVisionInfo.g_fCalibPixelY = 1 / float.Parse(txt_YResolution.Text);
                    m_smVisionInfo.g_fCalibPixelZ = 1 / float.Parse(txt_ZResolution.Text);
                }

                m_smVisionInfo.g_fCalibPixelXInUM = m_smVisionInfo.g_fCalibPixelX / 1000;
                m_smVisionInfo.g_fCalibPixelYInUM = m_smVisionInfo.g_fCalibPixelY / 1000;
                m_smVisionInfo.g_fCalibPixelZInUM = m_smVisionInfo.g_fCalibPixelZ / 1000;

                objXml.WriteSectionElement("CalibrateROI");
                objXml.WriteElement1Value("PositionX", m_smVisionInfo.g_objCalibrateROI.ref_ROIPositionX);
                objXml.WriteElement1Value("PositionY", m_smVisionInfo.g_objCalibrateROI.ref_ROIPositionY);
                objXml.WriteElement1Value("Width", m_smVisionInfo.g_objCalibrateROI.ref_ROIWidth);
                objXml.WriteElement1Value("Height", m_smVisionInfo.g_objCalibrateROI.ref_ROIHeight);

                // 2021 10 25 - CCENG: Need to save this CalibrationTolerance value to xml also bcos this variable wont auto save during change text.
                objXml.WriteSectionElement("FactoryCalibration");
                objXml.WriteElement1Value("CalibrateTolerance", txt_CalTol.Text);

                objXml.WriteEndElement();

                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Calibration", m_smProductionInfo.g_strLotID);

                // 2021 10 25 - CCENG: During save, should not set result as factory calibration record. The Factory calibration will only be recorded when user press the "Set to Default" button.
                //SaveCurrentResultAsFactoryCalibrationRecord();

#if (RELEASE || Release_2_12 || RTXRelease)
                //2020-12-10 ZJYEOH : Overwrite global xml and other recipe local-global xml
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                {
                    File.Copy(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", true);

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (m_strFolderPath == (arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                            continue;

                        if (Directory.Exists(arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                        {
                            File.Copy(m_strFolderPath + "\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", true);
                        }
                    }
                }
#endif

            }
            else
            {
                string strFileName;
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                    strFileName = m_smVisionInfo.g_strVisionFolderName;
                else
                    strFileName = "";

                if (!m_blnApplyOffsetValueDone)
                    m_smVisionInfo.g_arrImages[0].SaveImage(m_strFolderPath + "CalibrationImage.bmp");

                
                STDeviceEdit.CopySettingFile(m_strFolderPath, strFileName + "Calibration.xml");

                //Save all calibration values into xml
                XmlParser objXml = new XmlParser(m_strFolderPath + strFileName + "Calibration.xml");

                objXml.WriteSectionElement("Calibrate");
                objXml.WriteElement1Value("FOV", txt_FOV.Text);
                if (radioBtn_pixelpermm.Checked)
                {
                    objXml.WriteElement1Value("PixelX", txt_XResolution.Text);
                    objXml.WriteElement1Value("PixelY", txt_YResolution.Text);
                    objXml.WriteElement1Value("PixelZ", txt_ZResolution.Text);
                }
                else
                {
                    objXml.WriteElement1Value("PixelX", 1 / Convert.ToSingle(txt_XResolution.Text));
                    objXml.WriteElement1Value("PixelY", 1 / Convert.ToSingle(txt_YResolution.Text));
                    objXml.WriteElement1Value("PixelZ", 1 / Convert.ToSingle(txt_ZResolution.Text));
                }
                objXml.WriteElement1Value("OffSetX", txt_WidthOffSet.Text);
                objXml.WriteElement1Value("OffSetY", txt_HeightOffSet.Text);
                objXml.WriteElement1Value("OffSetZ", txt_ZOffSet.Text);

                //objXml.WriteSectionElement("OffSet");
                //objXml.WriteElement1Value("WidthOffSet", txt_WidthOffSet.Text);
                //objXml.WriteElement1Value("HeightOffSet", txt_HeightOffSet.Text);
                if (radioBtn_pixelpermm.Checked)
                {
                    m_smVisionInfo.g_fCalibPixelX = float.Parse(txt_XResolution.Text);
                    m_smVisionInfo.g_fCalibPixelY = float.Parse(txt_YResolution.Text);
                    m_smVisionInfo.g_fCalibPixelZ = float.Parse(txt_ZResolution.Text);
                }
                else
                {
                    m_smVisionInfo.g_fCalibPixelX = 1 / float.Parse(txt_XResolution.Text);
                    m_smVisionInfo.g_fCalibPixelY = 1 / float.Parse(txt_YResolution.Text);
                    m_smVisionInfo.g_fCalibPixelZ = 1 / float.Parse(txt_ZResolution.Text);
                }

                m_smVisionInfo.g_fCalibPixelXInUM = m_smVisionInfo.g_fCalibPixelX / 1000;
                m_smVisionInfo.g_fCalibPixelYInUM = m_smVisionInfo.g_fCalibPixelY / 1000;
                m_smVisionInfo.g_fCalibPixelZInUM = m_smVisionInfo.g_fCalibPixelZ / 1000;

                // 2021 10 25 - CCENG: Need to save this CalibrationTolerance value to xml also bcos this variable wont auto save during change text.
                objXml.WriteSectionElement("FactoryCalibration");
                objXml.WriteElement1Value("CalibrateTolerance", txt_CalTol.Text);

                objXml.WriteEndElement();

                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Calibration", m_smProductionInfo.g_strLotID);

#if (RELEASE || Release_2_12 || RTXRelease)
                //2020-12-10 ZJYEOH : Overwrite global xml and other recipe local-global xml
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                {
                    File.Copy(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", true);

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (m_strFolderPath == (arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                            continue;

                        if (Directory.Exists(arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                        {
                            File.Copy(m_strFolderPath + "\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", true);
                        }
                    }
                }
#endif
                
            }

            SaveGaugeSettings(m_strFolderPath + "Gauge.xml");
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;
            CloseForm();

        }

        private void cbo_TransitionChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibrateCircleGauge.SetTransitionChoice(cbo_TransitionChoice.SelectedIndex);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_TransitionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibrateCircleGauge.SetTransitionType(cbo_TransitionType.SelectedIndex);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void srmTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (srmTabControl1.SelectedTab == tabPage_Calibration)
            {
                m_smVisionInfo.g_intCalibrationType = 0;
                m_smVisionInfo.g_blnDragROI = true;

            }
            else if (srmTabControl1.SelectedTab == tabPage_CalibrationByCircle)
            {
                if (!tabPage_CalibrationByCircle.Controls.Contains(chk_ShowSamplePoints))
                    tabPage_CalibrationByCircle.Controls.Add(chk_ShowSamplePoints);
                if (!tabPage_CalibrationByCircle.Controls.Contains(chk_ShowDraggingBox))
                    tabPage_CalibrationByCircle.Controls.Add(chk_ShowDraggingBox);
                chk_ShowSamplePoints.Visible = true;
                chk_ShowDraggingBox.Visible = true;
                chk_ShowDraggingBox.Checked = m_smVisionInfo.g_objCalibrateCircleGauge.ref_blnDrawDraggingBox;
                chk_ShowSamplePoints.Checked = m_smVisionInfo.g_objCalibrateCircleGauge.ref_blnDrawSamplingPoint;
                m_smVisionInfo.g_intCalibrationType = 2;
                m_smVisionInfo.g_blnDragROI = true;
            }
            else if (srmTabControl1.SelectedTab == tabPage_CalibrationByXY)
            {
                if (!tabPage_CalibrationByXY.Controls.Contains(chk_ShowSamplePoints))
                    tabPage_CalibrationByXY.Controls.Add(chk_ShowSamplePoints);
                if (!tabPage_CalibrationByXY.Controls.Contains(chk_ShowDraggingBox))
                    tabPage_CalibrationByXY.Controls.Add(chk_ShowDraggingBox);
                chk_ShowSamplePoints.Visible = chk_UseGauge1.Checked;
                chk_ShowDraggingBox.Visible = chk_UseGauge1.Checked;
                chk_ShowDraggingBox.Checked = m_smVisionInfo.g_objCalibrateRectGauge.ref_blnDrawDraggingBox;
                chk_ShowSamplePoints.Checked = m_smVisionInfo.g_objCalibrateRectGauge.ref_blnDrawSamplingPoint;
                m_smVisionInfo.g_intCalibrationType = 3;
                m_smVisionInfo.g_blnDragROI = true;
            }
            else if (srmTabControl1.SelectedTab == tabPage_Calibration4S)
            {
                m_smVisionInfo.g_intCalibrationType = 4;
                m_smVisionInfo.g_blnDragROI = true;
            }


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void txt_GaugeThickness_TextChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibrateCircleGauge.SetThickness(Convert.ToInt32(txt_GaugeThickness.Text));
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GaugeThreshold_TextChanged(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibrateCircleGauge.SetThreshold(Convert.ToInt32(txt_GaugeThreshold.Text));
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Diameter_TextChanged(object sender, EventArgs e)
        {

        }

        private void label56_Click(object sender, EventArgs e)
        {

        }

        private void txt_ResolutionOffSet_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (radioBtn_pixelpermm.Checked)
            {
                txt_XResolution.Text = (m_fXResolutionPrev + Convert.ToSingle(txt_WidthOffSet.Text)).ToString();
                txt_YResolution.Text = (m_fYResolutionPrev + Convert.ToSingle(txt_HeightOffSet.Text)).ToString();
                txt_ZResolution.Text = (m_fZResolutionPrev + Convert.ToSingle(txt_ZOffSet.Text)).ToString();
            }
            else
            {
                txt_XResolution.Text = (1 / (m_fXResolutionPrev + Convert.ToSingle(txt_WidthOffSet.Text))).ToString();
                txt_YResolution.Text = (1 / (m_fYResolutionPrev + Convert.ToSingle(txt_HeightOffSet.Text))).ToString();
                txt_ZResolution.Text = (1 / (m_fZResolutionPrev + Convert.ToSingle(txt_ZOffSet.Text))).ToString();
            }
            //txt_PixelSizeX.Text = (1 / Convert.ToSingle(txt_XResolution.Text)).ToString();
            //txt_PixelSizeY.Text = (1 / Convert.ToSingle(txt_YResolution.Text)).ToString();
        }

        private void radioBtn_mmperPixel_Click(object sender, EventArgs e)
        {

            if (radioBtn_pixelpermm.Checked && label1.Text != "pixel/mm")
            {
                label1.Text = label6.Text = label17.Text = "pixel/mm";

                txt_XResolution.Text = (1 / Convert.ToSingle(txt_XResolution.Text)).ToString();
                txt_YResolution.Text = (1 / Convert.ToSingle(txt_YResolution.Text)).ToString();
                txt_ZResolution.Text = (1 / Convert.ToSingle(txt_ZResolution.Text)).ToString();

            }
            else if (radioBtn_mmperPixel.Checked && label1.Text != "mm/pixel")
            {
                label1.Text = label6.Text = label17.Text = "mm/pixel";

                txt_XResolution.Text = (1 / Convert.ToSingle(txt_XResolution.Text)).ToString();
                txt_YResolution.Text = (1 / Convert.ToSingle(txt_YResolution.Text)).ToString();
                txt_ZResolution.Text = (1 / Convert.ToSingle(txt_ZResolution.Text)).ToString();

            }

        }

        private void btn_Camera_Click(object sender, EventArgs e)
        {
            CalibrationCameraSettingsImageView objCameraForm = new CalibrationCameraSettingsImageView(m_smVisionInfo, m_smCustomizeInfo.g_blnLEDiControl,
                m_strSelectedRecipe, m_objAVTFireGrab, m_objIDSCamera, m_objTeliCamera, m_smProductionInfo.g_intUserGroup, m_strFolderPath); //, 1);

            objCameraForm.Location = new Point(600, 310);
            if (objCameraForm.ShowDialog() == DialogResult.Cancel)
            {
                GetCalibrationCameraValue();
                GrabOneTime();

                m_smVisionInfo.AT_PR_GrabImage = true;
                Thread.Sleep(5);
                m_smVisionInfo.AT_PR_GrabImage = false;
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }
        }

        private void chk_UseGauge1_Click(object sender, EventArgs e)
        {
            SetUseGaugeGUI();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_intSelectedImage != m_smVisionInfo.g_intSelectedImage)
            {
                m_intSelectedImage = m_smVisionInfo.g_intSelectedImage;
                m_smVisionInfo.g_objCalibrateROI.AttachImage(m_smVisionInfo.g_arrImages[m_intSelectedImage]);
                if (m_smVisionInfo.g_blnViewGauge)
                {
                    m_smVisionInfo.g_objCalibrateRectGauge.ModifyGauge(m_smVisionInfo.g_objCalibrateROI);
                    m_smVisionInfo.g_objCalibrateRectGauge.Measure(m_smVisionInfo.g_objCalibrateROI);
                }
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void btn_GaugeAdvanceSetting_Click(object sender, EventArgs e)
        {
            AdvancedRGaugeForm objForm = new AdvancedRGaugeForm(m_smVisionInfo, m_smVisionInfo.g_objCalibrateRectGauge, m_smVisionInfo.g_objCalibrateROI);
            objForm.Location = new Point(720, 200);
            objForm.ShowDialog();
        }

        private void btn_Set_Click(object sender, EventArgs e)
        {
            if (SRMMessageBox.Show("Confirm to store this calibration result as factory default?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SaveCurrentResultAsFactoryCalibrationRecord();
            }
            else
                return;
        }
        private void SaveCurrentResultAsFactoryCalibrationRecord()
        {
            //Save all calibration values into xml
            XmlParser objXml;
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                objXml = new XmlParser(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                objXml = new XmlParser(m_strFolderPath + "Calibration.xml");

            objXml.WriteSectionElement("FactoryCalibration");
            objXml.WriteElement1Value("CalibrateTolerance", txt_CalTol.Text);
            float PX, PY;
            if (radioBtn_pixelpermm.Checked)
            {
                PX = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_XResolution.Text));
                PY = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_YResolution.Text));

            }
            else
            {
                PX = (float)Convert.ToDouble(txt_XResolution.Text);
                PY = (float)Convert.ToDouble(txt_YResolution.Text);

            }

            m_fXResolution_Ori = PX;
            m_fYResolution_Ori = PY;

            m_MaxSizeX = PX + (float)Convert.ToDouble(txt_CalTol.Text); // txt_XResolution.Text
            m_MaxSizeY = PY + (float)Convert.ToDouble(txt_CalTol.Text);

            m_MinSizeX = PX - (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSizeY = PY - (float)Convert.ToDouble(txt_CalTol.Text);

            if (m_MinSizeX < 0)
                m_MinSizeX = 0;
            if (m_MinSizeY < 0)
                m_MinSizeY = 0;

            // Keep MaxSize and MinSize due to old calibration value using this variables.
            objXml.WriteElement1Value("MaxSizeX", m_MaxSizeX);
            objXml.WriteElement1Value("MaxSizeY", m_MaxSizeY);
            objXml.WriteElement1Value("MinSizeX", m_MinSizeX);
            objXml.WriteElement1Value("MinSizeY", m_MinSizeY);

            // 2021 10 25 - CCENG: Save this Resolution_Ori value when user press "Set As Default" only.
            objXml.WriteElement1Value("XResolution_Ori", m_fXResolution_Ori);
            objXml.WriteElement1Value("YResolution_Ori", m_fYResolution_Ori);

            objXml.WriteEndElement();

#if (RELEASE || Release_2_12 || RTXRelease)
                //2020-12-10 ZJYEOH : Overwrite global xml and other recipe local-global xml
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                {
                    File.Copy(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", true);

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (m_strFolderPath == (arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                            continue;

                        if (Directory.Exists(arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                        {
                            File.Copy(m_strFolderPath + "\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", true);
                        }
                    }
                }
#endif

        }

        private void txt_CalTol_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            //SaveCurrentResultAsFactoryCalibrationRecord();
        }

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            if (srmTabControl1.SelectedTab == tabPage_CalibrationByCircle)
            {
                m_smVisionInfo.g_objCalibrateCircleGauge.ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }
            else if (srmTabControl1.SelectedTab == tabPage_CalibrationByXY)
            {
                m_smVisionInfo.g_objCalibrateRectGauge.ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            if (srmTabControl1.SelectedTab == tabPage_CalibrationByCircle)
            {
                m_smVisionInfo.g_objCalibrateCircleGauge.ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }
            else if (srmTabControl1.SelectedTab == tabPage_CalibrationByXY)
            {
                m_smVisionInfo.g_objCalibrateRectGauge.ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }
            

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void timer1_Tick_1(object sender, EventArgs e)
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
        }

        private void CalibrationForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void CalibrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Calibration Page Closed", "Exit Calibration Page", "", "", m_smProductionInfo.g_strLotID);
            
        }

        private void btn_ApplyCurrentValue_Click(object sender, EventArgs e)
        {
            btn_Save.Enabled = true;
            m_blnApplyOffsetValueDone = true;
        }
    }
}