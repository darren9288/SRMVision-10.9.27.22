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


    public partial class Calibration5SForm : Form
    {
        #region Member Variables   
        private bool m_blnIs5S = true;
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

        private int m_intMergeType;
        private bool m_blnInitDone = false;
        private bool m_blnCalibrationDone = false;
        private bool m_blnApplyOffsetValueDone = false;
        private int m_intThreshold = -4;
        private int m_intUserGroup = 5;
        private float m_fXResolutionPrev;
        private float m_fYResolutionPrev;
        private float m_fZResolutionPrev;
        private string m_strFolderPath = "";
        private string m_strSelectedRecipe = "Default";
        private float m_MaxSizeX = -1;
        private float m_MaxSizeY = -1;
        private float m_MaxSizeZ = -1;
        private float m_MinSizeX = -1;
        private float m_MinSizeY = -1;
        private float m_MinSizeZ = -1;
        private int m_intSelectedROIPrev = 0;

        private float m_fXResolution_Ori = 0;
        private float m_fYResolution_Ori = 0;
        private float m_fZResolution_Ori = 0;

        private VisionInfo m_smVisionInfo;
        private Calibration m_objCalibrate = new Calibration();
        private UserRight m_objUserRight = new UserRight();
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private AVTVimba m_objAVTFireGrab;
        private IDSuEyeCamera m_objIDSCamera;
        private TeliCamera m_objTeliCamera;
        #endregion

        public Calibration5SForm(VisionInfo smVisionInfo, string recipe, int grp, CustomOption smCustomizeInfo, AVTVimba objAVTFireGrab, IDSuEyeCamera objIDSCamera, TeliCamera objTeliCamera, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + recipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            m_strSelectedRecipe = recipe;
            m_intUserGroup = grp;

            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                m_strFilePath = m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Camera.xml";
            else
                m_strFilePath = m_strFolderPath + "Camera.xml";

            m_smCustomizeInfo = smCustomizeInfo;
            m_objAVTFireGrab = objAVTFireGrab;
            m_objIDSCamera = objIDSCamera;
            m_objTeliCamera = objTeliCamera;
            m_intMergeType = m_smVisionInfo.g_intImageMergeType;
            //m_smVisionInfo.g_intImageMergeType = 0;
            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            //Thread.Sleep(5);
            // m_smVisionInfo.AT_PR_StartLiveImage = true;
            // m_smVisionInfo.AT_PR_TriggerLiveImage = false;

            GetPreviousCameraValue();
            GetCalibrationCameraValue();
            GrabOneTime(false);// GrabOneTime(true);

            //m_smVisionInfo.AT_PR_GrabImage = true;
            //Thread.Sleep(5);
            //m_smVisionInfo.AT_PR_GrabImage = false;

            DisableField2();
            LoadSettings();
            LoadGlobalSettings();
            UpdateGUI();

            m_smVisionInfo.g_intCalibrationType = 0;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnDragROI = true;
            //m_smVisionInfo.g_intSelectedImage = 0;  // Only image 1 will be used for calibration.

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_intSelectedROIPrev = m_smVisionInfo.g_intSelectedROI;
            /*
             * Unit display as mm, not mil or micron because most of the jigs come with mm dimension.
             * 
             */
            m_blnInitDone = true;
            m_blnApplyOffsetValueDone = false;
        }

        private void GrabOneTime(bool blnSkip)
        {
            /////Set Gain
            UInt32 intGainValue = Convert.ToUInt32(m_smVisionInfo.g_arrCameraGain[m_smVisionInfo.g_intSelectedImage]);

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
                m_smVisionInfo.g_arrCameraGain[m_smVisionInfo.g_intSelectedImage] = intGainValue;
            }
            else
            {
                m_smVisionInfo.g_arrImageGain[0] = 1 + (intGainValue - 50) * 0.18f;
            }

            ///////Set Shuttle
            float fShuttleValue = m_smVisionInfo.g_arrCameraShuttle[m_smVisionInfo.g_intSelectedImage] / 100;

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
            m_smVisionInfo.g_arrCameraShuttle[m_smVisionInfo.g_intSelectedImage] = fShuttleValue;

            /////Set Light source
            //for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)      // 2019 05 10-CCENG: Bug: Should loop using image
            {
                if (blnSkip && i > 0)
                    break;

                if (m_smVisionInfo.g_intLightControllerType == 1)
                {
                    // 2019 06 17 - CCENG: This function no support Normal Controller Setting.
                    //LightSource objLight = m_smVisionInfo.g_arrLightSource[i];
                    //int intValue = objLight.ref_arrValue[0];

                    ////if (m_smVisionInfo.g_arrImages.Count == 1)    // Prevent both side intensity during camera live (Warning: This condition should apply if intensity change during grab image)
                    //{
                    //    if (m_smCustomizeInfo.g_blnLEDiControl)
                    //        LEDi_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, Convert.ToByte(intValue));
                    //    else
                    //        TCOSIO_Control.SetIntensity(objLight.ref_intPortNo, objLight.ref_intChannel, intValue);
                    //}
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

                        List<int> arrCOMList = new List<int>();
                        for (int k = 0; k < m_smVisionInfo.g_arrLightSource.Count; k++)
                        {
                            bool blnFound = false;
                            for (int c = 0; c < arrCOMList.Count; c++)
                            {
                                if (arrCOMList[c] == m_smVisionInfo.g_arrLightSource[k].ref_intPortNo)
                                {
                                    blnFound = true;
                                    break;
                                }
                            }

                            if (!blnFound)
                                arrCOMList.Add(m_smVisionInfo.g_arrLightSource[k].ref_intPortNo);
                        }

                        if (arrCOMList.Count > 0)
                        {
                            int intValue1 = 0, intValue2 = 0, intValue3 = 0, intValue4 = 0;
                            if (m_smVisionInfo.g_arrLightSource.Count > 0 && i < m_smVisionInfo.g_arrLightSource[0].ref_arrValue.Count)
                                intValue1 = m_smVisionInfo.g_arrLightSource[0].ref_arrValue[i];
                            if (m_smVisionInfo.g_arrLightSource.Count > 1 && i < m_smVisionInfo.g_arrLightSource[1].ref_arrValue.Count)
                                intValue2 = m_smVisionInfo.g_arrLightSource[1].ref_arrValue[i];
                            if (m_smVisionInfo.g_arrLightSource.Count > 2 && i < m_smVisionInfo.g_arrLightSource[2].ref_arrValue.Count)
                                intValue3 = m_smVisionInfo.g_arrLightSource[2].ref_arrValue[i];
                            if (m_smVisionInfo.g_arrLightSource.Count > 3 && i < m_smVisionInfo.g_arrLightSource[3].ref_arrValue.Count)
                                intValue4 = m_smVisionInfo.g_arrLightSource[3].ref_arrValue[i];


                            LEDi_Control.RunStop(arrCOMList[0], 0, false);
                            Thread.Sleep(10);

                            int intValue5 = 0;
                            int intValue6 = 0;
                            int intValue7 = 0;
                            int intValue8 = 0;
                            // 2021 04 20 - CCENG: Reupdate intensity according to image display mode
                            LEDi_Control.UpdateIntensityValueAccordingToImageDisplayMode(m_smVisionInfo.g_intImageDisplayMode, i,
                                                                                        ref intValue1, ref intValue2, ref intValue3, ref intValue4,
                                                                                        ref intValue5, ref intValue6, ref intValue7, ref intValue8);

                            LEDi_Control.SetSeqIntensity(arrCOMList[0], 0, i, intValue1, intValue2, intValue3, intValue4);
                            Thread.Sleep(10);
                            LEDi_Control.SaveIntensity(arrCOMList[0], 0);
                            Thread.Sleep(100);
                            LEDi_Control.RunStop(arrCOMList[0], 0, true);
                            Thread.Sleep(10);
                        }

                        if (arrCOMList.Count > 1)
                        {
                            int intValue5 = 0, intValue6 = 0, intValue7 = 0, intValue8 = 0;
                            if (m_smVisionInfo.g_arrLightSource.Count > 4 && i < m_smVisionInfo.g_arrLightSource[4].ref_arrValue.Count)
                                intValue5 = m_smVisionInfo.g_arrLightSource[4].ref_arrValue[i];
                            if (m_smVisionInfo.g_arrLightSource.Count > 5 && i < m_smVisionInfo.g_arrLightSource[5].ref_arrValue.Count)
                                intValue6 = m_smVisionInfo.g_arrLightSource[5].ref_arrValue[i];
                            if (m_smVisionInfo.g_arrLightSource.Count > 6 && i < m_smVisionInfo.g_arrLightSource[6].ref_arrValue.Count)
                                intValue7 = m_smVisionInfo.g_arrLightSource[6].ref_arrValue[i];
                            if (m_smVisionInfo.g_arrLightSource.Count > 7 && i < m_smVisionInfo.g_arrLightSource[7].ref_arrValue.Count)
                                intValue8 = m_smVisionInfo.g_arrLightSource[7].ref_arrValue[i];


                            LEDi_Control.RunStop(arrCOMList[1], 0, false);
                            Thread.Sleep(10);

                            int intValue1 = 0;
                            int intValue2 = 0;
                            int intValue3 = 0;
                            int intValue4 = 0;
                            // 2021 04 20 - CCENG: Reupdate intensity according to image display mode
                            LEDi_Control.UpdateIntensityValueAccordingToImageDisplayMode(m_smVisionInfo.g_intImageDisplayMode, i,
                                                                                        ref intValue1, ref intValue2, ref intValue3, ref intValue4,
                                                                                        ref intValue5, ref intValue6, ref intValue7, ref intValue8);

                            LEDi_Control.SetSeqIntensity(arrCOMList[1], 0, i, intValue5, intValue6, intValue7, intValue8);
                            Thread.Sleep(10);
                            LEDi_Control.SaveIntensity(arrCOMList[1], 0);
                            Thread.Sleep(100);
                            LEDi_Control.RunStop(arrCOMList[1], 0, true);
                            Thread.Sleep(10);
                        }
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

            for (int i = 0; i < m_intMergeType + 1; i++)
            {
                m_smVisionInfo.g_arrCameraShuttle[i] = objFile.GetValueAsFloat("Shutter" + i.ToString(), 200f);
                m_smVisionInfo.g_arrCameraGain[i] = objFile.GetValueAsUInt("Gain" + i.ToString(), 20);
                m_smVisionInfo.g_arrImageGain[i] = objFile.GetValueAsFloat("ImageGain" + i.ToString(), 1f);
            }


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
            for (int j = 0; j < m_smVisionInfo.g_arrImageGain.Count; j++)
                m_arrImageGain.Add(m_smVisionInfo.g_arrImageGain[j]);
            for (int k = 0; k < m_smVisionInfo.g_arrCameraGain.Count; k++)
                m_arrCameraGain.Add(m_smVisionInfo.g_arrCameraGain[k]);

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
            m_smVisionInfo.g_arrCameraShuttle = m_arrCameraShuttle;
            m_smVisionInfo.g_arrImageGain = m_arrImageGain;
            m_smVisionInfo.g_arrCameraGain = m_arrCameraGain;

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
        /// Perfrom grid calibration
        /// </summary>
        private void Calibrate()
        {

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        /// <summary>
        /// Close this form and set back the variables default value
        /// </summary>
        private void CloseForm()
        {
            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            m_smVisionInfo.g_intImageMergeType = m_intMergeType;
            SaveBackPreviousCameraValue();
            GrabOneTime(false);
            SaveBackPreviousCameraValue();
            m_arrImageNo.Clear();
            m_arrValue.Clear();
            m_smVisionInfo.g_intImageMergeType = m_intMergeType;
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

            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;

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

            if (m_intUserGroup != 1)    // SRM User
            {
                srmTabControl1.TabPages.Remove(tabPage_OffSetSetting);
            }

            strChild1 = "Calibration Page";
            strChild2 = "5S Gauge Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_PackageGaugeSetting.Enabled = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "5S Gauge DraggingBox Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_ShowDraggingBox.Enabled = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "5S Gauge Sampling Point Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                chk_ShowSamplePoints.Enabled = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "5S X Distance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_XDistance.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "5S Y Distance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_YDistance.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "5S Z Distance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_ZDistance.Enabled = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "5S Diameter Distance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_DiameterDistance.Enabled = false;
            }

            strChild1 = "Calibration Page";
            strChild2 = "5S Camera Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Camera.Enabled = false;
            }
            strChild1 = "Calibration Page";
            strChild2 = "5S Calibrate Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Calibrate.Enabled = false;
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

            if (m_intUserGroup != 1)    // SRM User
            {
                srmTabControl1.TabPages.Remove(tabPage_OffSetSetting);
            }

            strChild3 = "5S Gauge Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_PackageGaugeSetting.Enabled = false;
            }

            strChild3 = "5S Gauge Dragging Box";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                chk_ShowDraggingBox.Enabled = false;
            }

            strChild3 = "5S Gauge Sampling Point";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                chk_ShowSamplePoints.Enabled = false;
            }

            strChild3 = "5S X Distance Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_XDistance.Enabled = false;
            }

            strChild3 = "5S Y Distance Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_YDistance.Enabled = false;
            }

            strChild3 = "5S Z Distance Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_ZDistance.Enabled = false;
            }

            strChild3 = "5S Diameter Distance Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_DiameterDistance.Enabled = false;
            }

            strChild3 = "5S Camera Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Camera.Enabled = false;
            }

            strChild3 = "5S Calibrate Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Calibrate.Enabled = false;
            }

            strChild3 = "5S Gauge Tool";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                cbo_SelectGaugeTool.Enabled = false;
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
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPkg":
                    m_smVisionInfo.g_objCalibration = new Calibration(2, m_smVisionInfo.g_WorldShape);
                    // 01-07-2019 ZJYEOH : Hide Z Dimension when no 5S
                    label3.Visible = false;
                    label4.Visible = false;
                    txt_MeasuredZ.Visible = false;
                    label21.Visible = false;
                    label17.Visible = false;
                    txt_ZResolution.Visible = false;
                    panel6.Visible = false;
                    m_blnIs5S = false;

                    cbo_SelectGaugeTool.Items.RemoveAt(2);
                    cbo_SelectGaugeTool.Items.RemoveAt(1);
                    cbo_SelectGaugeTool.Items.Add("Circle Gauge");
                    break;
                case "Pad5S":
                case "Pad5SPkg":
                default:
                    m_smVisionInfo.g_objCalibration = new Calibration(1, m_smVisionInfo.g_WorldShape);
                    m_blnIs5S = true;
                    break;
            }

            XmlParser objXml;
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                objXml = new XmlParser(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                objXml = new XmlParser(m_strFolderPath + "Calibration.xml");

            objXml.GetFirstSection("Settings");
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objCalibration.ref_intSelectedImage = objXml.GetValueAsInt("SelectedImage", 0);
            if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrImages.Count)
                m_smVisionInfo.g_intSelectedImage = 0;  // Make sure selected image index is not out of range.
            m_smVisionInfo.g_objCalibration.ref_fImageGain = objXml.GetValueAsFloat("ImageGain", 1f);
            m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool = objXml.GetValueAsInt("SelectedGaugeTool", 0);
            m_smVisionInfo.g_objCalibration.ref_fSizeX = objXml.GetValueAsFloat("SizeX", 1);
            m_smVisionInfo.g_objCalibration.ref_fSizeY = objXml.GetValueAsFloat("SizeY", 1);
            m_smVisionInfo.g_objCalibration.ref_fSizeZ = objXml.GetValueAsFloat("SizeZ", 1);
            m_smVisionInfo.g_objCalibration.ref_fSizeDiameter = objXml.GetValueAsFloat("SizeDiameter", 1);

            //if (m_smVisionInfo.g_intSelectedImage < m_smVisionInfo.g_arrImages.Count)
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objCalibration.ref_fImageGain);

            //m_smVisionInfo.g_blnViewPackageImage = true; //30-05-2019 ZJYEOH: Removed this line to disable the add gain function as added camera setting

            // Update GUI
            cbo_SelectPadROIs.SelectedIndex = 0;
            cbo_SelectImage.SelectedIndex = m_smVisionInfo.g_objCalibration.ref_intSelectedImage;
            cbo_SelectGaugeTool.SelectedIndex = m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool;
            if (!m_smVisionInfo.g_strVisionName.Contains("5S") && cbo_SelectGaugeTool.SelectedIndex > 1)
                cbo_SelectGaugeTool.SelectedIndex = 0;
            chk_ShowDraggingBox.Checked = m_smVisionInfo.g_objCalibration.ref_blnDrawDraggingBox;
            chk_ShowSamplePoints.Checked = m_smVisionInfo.g_objCalibration.ref_blnDrawSamplingPoint;
            txt_ImageGain.Value = (decimal)m_smVisionInfo.g_objCalibration.ref_fImageGain;
            txt_XDistance.Text = m_smVisionInfo.g_objCalibration.ref_fSizeX.ToString();
            txt_YDistance.Text = m_smVisionInfo.g_objCalibration.ref_fSizeY.ToString();
            txt_ZDistance.Text = m_smVisionInfo.g_objCalibration.ref_fSizeZ.ToString();
            txt_DiameterDistance.Text = m_smVisionInfo.g_objCalibration.ref_fSizeDiameter.ToString();

            m_smVisionInfo.g_intThresholdValue = objXml.GetValueAsInt("Threshold", -4);
            if (m_smVisionInfo.g_intThresholdValue == 255)
                m_smVisionInfo.g_intThresholdValue = 254;

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

            if (radioBtn_pixelpermm.Checked)
            {
                txt_WidthOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_XResolution.Text) * 0.1f);
                txt_HeightOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_YResolution.Text) * 0.1f);
                txt_ZOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_ZResolution.Text) * 0.1f);
            }
            else
            {
                txt_WidthOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_XResolution.Text)) * 0.1f);
                txt_HeightOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_YResolution.Text)) * 0.1f);
                txt_ZOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_ZResolution.Text)) * 0.1f);
            }

            //if (radioBtn_pixelpermm.Checked)
            //{
            //    m_fXResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_XResolution.Text));
            //    m_fYResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_YResolution.Text));
            //    m_fZResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_ZResolution.Text));
            //}
            //else
            //{
            //    m_fXResolution_Ori = (float)Convert.ToDouble(txt_XResolution.Text);
            //    m_fYResolution_Ori = (float)Convert.ToDouble(txt_YResolution.Text);
            //    m_fZResolution_Ori = (float)Convert.ToDouble(txt_ZResolution.Text);
            //}

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

            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                ROI.LoadFile(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", m_smVisionInfo.g_objCalibration.ref_arrROIs);
            else
                ROI.LoadFile(m_strFolderPath + "Calibration.xml", m_smVisionInfo.g_objCalibration.ref_arrROIs);

            if (m_smVisionInfo.g_objCalibration.ref_objCirGauge != null)
            {
                m_smVisionInfo.g_objCalibration.ref_objCirGauge.LoadCircleGauge(m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\CalibrationCircleGauge.xml", "CalibrationCircleGauge");
            }

            if (m_smVisionInfo.g_objCalibration.ref_arrROIs.Count == 0)
            {
                m_smVisionInfo.g_objCalibration.AddGaugeROI(m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_objCalibration.AddRectGauge4L(m_smVisionInfo.g_WorldShape);
                m_smVisionInfo.g_objCalibration.AddLGauge(m_smVisionInfo.g_WorldShape);
            }
            else if (m_smVisionInfo.g_objCalibration.ref_arrROIs.Count != m_smVisionInfo.g_objCalibration.ref_intROICount)
            {
                m_smVisionInfo.g_objCalibration.AddGaugeROI(m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_objCalibration.AddRectGauge4L(m_smVisionInfo.g_WorldShape);
                m_smVisionInfo.g_objCalibration.AddLGauge(m_smVisionInfo.g_WorldShape);
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrROIs.Count; i++)
                {
                    m_smVisionInfo.g_objCalibration.ref_arrROIs[i].AttachImage(m_smVisionInfo.g_objPackageImage);
                }

                m_smVisionInfo.g_objCalibration.AddRectGauge4L(m_smVisionInfo.g_WorldShape);

                string strPath = m_smProductionInfo.g_strRecipePath +
                     m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\CalibrationRectGauge4L.xml";

                for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L.Count; i++)
                    m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].LoadRectGauge4LMeasurementSettingOnly(strPath,
                            "CalibrationRectGauge4L"); //+ i.ToString()

                strPath = m_smProductionInfo.g_strRecipePath +
     m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\CalibrationLGauge4L.xml";

                LGauge.LoadFile(strPath, m_smVisionInfo.g_objCalibration.ref_arrLGauge, m_smVisionInfo.g_WorldShape);
                if (m_smVisionInfo.g_objCalibration.ref_arrLGauge != null && m_smVisionInfo.g_objCalibration.ref_arrLGauge.Count == 0)
                    m_smVisionInfo.g_objCalibration.AddLGauge(m_smVisionInfo.g_WorldShape);
            }

            m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();
            m_smVisionInfo.g_objCalibration.SetCircleGaugePlace();
            m_smVisionInfo.g_objCalibration.SetLGaugePlace();

            for (int i = 0; i < m_smVisionInfo.g_objCalibration.ref_arrROIs.Count; i++)
            {
                // 2019-11-04 ZJYEOH : Set gauge tolerance based on image resolution
                if (m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].GetGaugeTolerance(i) != m_smVisionInfo.g_intCameraResolutionWidth / 640f * 10)
                {
                    m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SetGaugeTolerance(m_smVisionInfo.g_intCameraResolutionWidth / 640f * 10);
                    m_smVisionInfo.g_objCalibration.ref_arrRectGauge4L[i].SaveRectGauge4L(
                        m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\CalibrationRectGauge4L.xml",
                        false,
                        "CalibrationRectGauge4L" + i.ToString(),
                        true,
                        true);
                }
            }

            objXml.GetFirstSection("FactoryCalibration");
            m_fXResolution_Ori = objXml.GetValueAsFloat("XResolution_Ori", 0);
            m_fYResolution_Ori = objXml.GetValueAsFloat("YResolution_Ori", 0);
            m_fZResolution_Ori = objXml.GetValueAsFloat("ZResolution_Ori", 0);
            m_MaxSizeX = objXml.GetValueAsFloat("MaxSizeX", -1);
            m_MaxSizeY = objXml.GetValueAsFloat("MaxSizeY", -1);
            m_MaxSizeZ = objXml.GetValueAsFloat("MaxSizeZ", -1);
            m_MinSizeX = objXml.GetValueAsFloat("MinSizeX", -1);
            m_MinSizeY = objXml.GetValueAsFloat("MinSizeY", -1);
            m_MinSizeZ = objXml.GetValueAsFloat("MinSizeZ", -1);
            txt_CalTol.Text = objXml.GetValueAsFloat("CalibrateTolerance", 0.002f).ToString();

            // 2021 10 25 - CCENG: For old calibraton xml file, Resolution_Ori variable is not exist. 
            //                     If Resolution_Ori no exist, then Resolution_Ori will direct calculate from m_MaxSize and m_MinSizeX. 
            if (m_fXResolution_Ori == 0)
            {
                if (m_MaxSizeX != -1)
                {
                    m_fXResolution_Ori = (m_MaxSizeX + m_MinSizeX) / 2;
                    m_fYResolution_Ori = (m_MaxSizeY + m_MinSizeY) / 2;
                    m_fZResolution_Ori = (m_MaxSizeZ + m_MinSizeZ) / 2;
                }
            }

        }

        private void UpdateGUI()
        {
            btn_Save.Enabled = false;   // 2018 10 18 - CCENG: save button will be enabled after calibration pass.
            btn_Set.Enabled = false;

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPkg":
                    label2.Visible = false;
                    txt_ZDistance.Visible = false;
                    label21.Visible = false;
                    txt_ZResolution.Visible = false;
                    label17.Visible = false;
                    label20.Visible = false;
                    txt_ZOffSet.Visible = false;
                    label19.Visible = false;
                    label4.Visible = false;
                    txt_MeasuredZ.Visible = false;
                    label3.Visible = false;
                    break;
                case "Pad5S":
                case "Pad5SPkg":

                    //// 2019 06 26 - CCENG: At this moment, Pad 5S have to use SQUARE Calibration Jig for calibration.
                    //panel5.Visible = false; // 2019-11-04 ZJYEOH , Hide Diameter as Pad5S only use square jig
                    //cbo_SelectGaugeTool.SelectedIndex = 0;
                    //cbo_SelectGaugeTool.Visible = false;
                    //srmLabel1.Visible = false;
                    break;
            }

            if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 0)
            {
                panel3.Visible = true;
                panel4.Visible = true;
                panel5.Visible = false;
                panel6.Visible = true;

                if ((m_smCustomizeInfo.g_intWantLead3D & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    btn_Lead3DGaugeSetting.Visible = true;
                    btn_Lead3DGaugeSetting.BringToFront();
                }
                else
                {
                    btn_PackageGaugeSetting.Visible = true;
                    btn_PackageGaugeSetting.BringToFront();
                }

                //btn_CircleGaugeSetting.Visible = false;
                chk_ShowDraggingBox.Visible = true;
                chk_ShowSamplePoints.Visible = true;

                m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();
            }
            else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 1)
            {
                panel3.Visible = false;
                panel4.Visible = false;
                panel5.Visible = true;
                panel6.Visible = true;
                // 2019 05 03-CCENG: Temperary hide this gauge button setting for circle gauge bcos currently circle gauge setting is hard coded.
                //                   Can add gauge setting for circle gauge in future. Need to consider 5S gauge which have "5 RectGauge4L" or "1 Circle Gauge + 4 RectGauge4L".
                //btn_PackageGaugeSetting.Visible = false;
                //btn_Lead3DGaugeSetting.Visible = false;

                if (m_smVisionInfo.g_intSelectedROI == 0)
                {
                    btn_CircleGaugeSetting.Visible = true;
                    btn_CircleGaugeSetting.BringToFront();
                }
                else
                {
                    btn_PackageGaugeSetting.Visible = true;
                    btn_PackageGaugeSetting.BringToFront();
                }


                chk_ShowDraggingBox.Visible = true;
                chk_ShowSamplePoints.Visible = true;

                m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();
                m_smVisionInfo.g_objCalibration.SetCircleGaugePlace();
            }
            else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 2)
            {
                panel3.Visible = false;
                panel4.Visible = false;
                panel5.Visible = true;
                panel6.Visible = true;
                // 2019 05 03-CCENG: Temperary hide this gauge button setting for circle gauge bcos currently circle gauge setting is hard coded.
                //                   Can add gauge setting for circle gauge in future. Need to consider 5S gauge which have "5 RectGauge4L" or "1 Circle Gauge + 4 RectGauge4L".
                //btn_PackageGaugeSetting.Visible = false;
                //btn_Lead3DGaugeSetting.Visible = false;
                //btn_CircleGaugeSetting.Visible = true;

                if (m_smVisionInfo.g_intSelectedROI == 0)
                {
                    btn_CircleGaugeSetting.Visible = true;
                    btn_CircleGaugeSetting.BringToFront();
                }
                else
                {
                    btn_LGaugeSetting.Visible = true;
                    btn_LGaugeSetting.BringToFront();
                }
                chk_ShowDraggingBox.Visible = true;
                chk_ShowSamplePoints.Visible = true;

                m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();
                m_smVisionInfo.g_objCalibration.SetLGaugePlace();
            }

            if ((m_smCustomizeInfo.g_intWantLead3D & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                btn_Lead3DGaugeSetting.Visible = true;
                btn_Lead3DGaugeSetting.BringToFront();
            }
            else
            {
                btn_Lead3DGaugeSetting.Visible = false;
                btn_Lead3DGaugeSetting.SendToBack();
            }

            if (m_smVisionInfo.g_arrImages.Count <= 1)
            {
                cbo_SelectImage.Visible = false;
                cbo_SelectImage.SelectedIndex = 0;
                srmLabel28.Visible = false;
            }
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
            bool blnResult = false;
            string strFailDetailMsg = "";

            if (Convert.ToSingle(txt_XDistance.Text) == 0.0 || Convert.ToSingle(txt_YDistance.Text) == 0.0 || Convert.ToSingle(txt_ZDistance.Text) == 0.0 || Convert.ToSingle(txt_DiameterDistance.Text) == 0.0)
            {
                SRMMessageBox.Show("X distance, Y distance, Z distance and Diameter cannot be zero. Please enter values.");
                return;
            }

            if (m_smVisionInfo.g_strVisionName == "Pad" || m_smVisionInfo.g_strVisionName == "BottomOrientPad" || m_smVisionInfo.g_strVisionName == "BottomOPadPkg")
                blnResult = m_smVisionInfo.g_objCalibration.Calibrate_5SCircleGaugeAndRectGauge4L(0x01, ref strFailDetailMsg, m_smVisionInfo.g_arrImages[0]);
            else
                blnResult = m_smVisionInfo.g_objCalibration.Calibrate_5SCircleGaugeAndRectGauge4L(0x1F, ref strFailDetailMsg, m_smVisionInfo.g_arrImages[0]);

            if (!blnResult)
            {
                if (strFailDetailMsg == "")
                    strFailDetailMsg = "Fail to measure unit edge.";
            }

            if (blnResult)
            {
                if (radioBtn_pixelpermm.Checked)
                {
                    if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 0)
                    {
                        txt_XResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationX(Convert.ToSingle(txt_XDistance.Text), 0) + Convert.ToSingle(txt_WidthOffSet.Text)).ToString();
                        txt_YResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationY(Convert.ToSingle(txt_YDistance.Text), 0) + Convert.ToSingle(txt_HeightOffSet.Text)).ToString();
                        txt_ZResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationZ(Convert.ToSingle(txt_ZDistance.Text)) + Convert.ToSingle(txt_ZOffSet.Text)).ToString();

                        txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackX(0) / Convert.ToSingle(txt_XResolution.Text)).ToString();
                        txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackY(0) / Convert.ToSingle(txt_YResolution.Text)).ToString();
                        txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ() / Convert.ToSingle(txt_ZResolution.Text)).ToString();

                        txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_YResolution.Text), 1).ToString();
                    }
                    else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 1)
                    {
                        txt_XResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationDiameter(Convert.ToSingle(txt_DiameterDistance.Text)) + Convert.ToSingle(txt_WidthOffSet.Text)).ToString();
                        txt_YResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationDiameter(Convert.ToSingle(txt_DiameterDistance.Text)) + Convert.ToSingle(txt_HeightOffSet.Text)).ToString();
                        txt_ZResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationZ(Convert.ToSingle(txt_ZDistance.Text)) + Convert.ToSingle(txt_ZOffSet.Text)).ToString();

                        txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackDiameter() / Convert.ToSingle(txt_XResolution.Text)).ToString();
                        txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackDiameter() / Convert.ToSingle(txt_YResolution.Text)).ToString();
                        txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ() / Convert.ToSingle(txt_ZResolution.Text)).ToString();

                        txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_YResolution.Text), 1).ToString();
                    }
                    else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 2)
                    {
                        txt_XResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationDiameter(Convert.ToSingle(txt_DiameterDistance.Text)) + Convert.ToSingle(txt_WidthOffSet.Text)).ToString();
                        txt_YResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationDiameter(Convert.ToSingle(txt_DiameterDistance.Text)) + Convert.ToSingle(txt_HeightOffSet.Text)).ToString();
                        txt_ZResolution.Text = (m_smVisionInfo.g_objCalibration.GetCalibrationZ_LineGauge(Convert.ToSingle(txt_ZDistance.Text)) + Convert.ToSingle(txt_ZOffSet.Text)).ToString();

                        txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackDiameter() / Convert.ToSingle(txt_XResolution.Text)).ToString();
                        txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackDiameter() / Convert.ToSingle(txt_YResolution.Text)).ToString();
                        txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ_LineGauge() / Convert.ToSingle(txt_ZResolution.Text)).ToString();

                        txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_XResolution.Text)),1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_YResolution.Text),1).ToString();
                    }
                }
                else
                {
                    if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 0)
                    {
                        txt_XResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationX(Convert.ToSingle(txt_XDistance.Text), 0) + Convert.ToSingle(txt_WidthOffSet.Text))).ToString();
                        txt_YResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationY(Convert.ToSingle(txt_YDistance.Text), 0) + Convert.ToSingle(txt_HeightOffSet.Text))).ToString();
                        txt_ZResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationZ(Convert.ToSingle(txt_ZDistance.Text)) + Convert.ToSingle(txt_ZOffSet.Text))).ToString();

                        txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackX(0) / (1 / Convert.ToSingle(txt_XResolution.Text))).ToString();
                        txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackY(0) / (1 / Convert.ToSingle(txt_YResolution.Text))).ToString();
                        txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ() / (1 / Convert.ToSingle(txt_ZResolution.Text))).ToString();

                        txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_YResolution.Text)), 1).ToString();
                    }
                    else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 1)
                    {
                        txt_XResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationDiameter(Convert.ToSingle(txt_DiameterDistance.Text)) + Convert.ToSingle(txt_WidthOffSet.Text))).ToString();
                        txt_YResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationDiameter(Convert.ToSingle(txt_DiameterDistance.Text)) + Convert.ToSingle(txt_HeightOffSet.Text))).ToString();
                        txt_ZResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationZ(Convert.ToSingle(txt_ZDistance.Text)) + Convert.ToSingle(txt_ZOffSet.Text))).ToString();

                        txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackDiameter() / (1 / Convert.ToSingle(txt_XResolution.Text))).ToString();
                        txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackDiameter() / (1 / Convert.ToSingle(txt_YResolution.Text))).ToString();
                        txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ() / (1 / Convert.ToSingle(txt_ZResolution.Text))).ToString();

                        txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_YResolution.Text)), 1).ToString();
                    }
                    else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 2)
                    {
                        txt_XResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationDiameter(Convert.ToSingle(txt_DiameterDistance.Text)) + Convert.ToSingle(txt_WidthOffSet.Text))).ToString();
                        txt_YResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationDiameter(Convert.ToSingle(txt_DiameterDistance.Text)) + Convert.ToSingle(txt_HeightOffSet.Text))).ToString();
                        txt_ZResolution.Text = (1 / (m_smVisionInfo.g_objCalibration.GetCalibrationZ_LineGauge(Convert.ToSingle(txt_ZDistance.Text)) + Convert.ToSingle(txt_ZOffSet.Text))).ToString();

                        txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackDiameter() / (1 / Convert.ToSingle(txt_XResolution.Text))).ToString();
                        txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackDiameter() / (1 / Convert.ToSingle(txt_YResolution.Text))).ToString();
                        txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ_LineGauge() / (1 / Convert.ToSingle(txt_ZResolution.Text))).ToString();

                        txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_XResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_YResolution.Text)), 1).ToString();
                    }

                }

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

                // ------------ Compare tolerance ------------------------------
                float X, Y, Z;
                if (radioBtn_pixelpermm.Checked)
                {
                    X = 1f / Convert.ToSingle(txt_XResolution.Text);
                    Y = 1f / Convert.ToSingle(txt_YResolution.Text);
                    Z = 1f / Convert.ToSingle(txt_ZResolution.Text);
                }
                else
                {
                    X = Convert.ToSingle(txt_XResolution.Text);
                    Y = Convert.ToSingle(txt_YResolution.Text);
                    Z = Convert.ToSingle(txt_ZResolution.Text);
                }


                if (((m_MaxSizeX == -1 || m_MinSizeX == -1 || m_MaxSizeY == -1 || m_MinSizeY == -1 || m_MaxSizeZ == -1 || m_MinSizeZ == -1) && m_blnIs5S)
                    || ((m_MaxSizeX == -1 || m_MinSizeX == -1 || m_MaxSizeY == -1 || m_MinSizeY == -1) && !m_blnIs5S))
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
                //else if (X > m_MaxSizeX || X < m_MinSizeX || Y > m_MaxSizeY || Y < m_MinSizeY || Z > m_MaxSizeZ || Z < m_MinSizeZ)
                else if (X > (m_fXResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                      X < (m_fXResolution_Ori - Convert.ToDouble(txt_CalTol.Text)) ||
                      Y > (m_fYResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                      Y < (m_fYResolution_Ori - Convert.ToDouble(txt_CalTol.Text)) ||
                      Z > (m_fZResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                      Z < (m_fZResolution_Ori - Convert.ToDouble(txt_CalTol.Text)))
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
                if (m_intUserGroup != 1)    // SRM User
                    btn_Set.Enabled = false;
                else
                    btn_Set.Enabled = true; // 2019 10 21 - CCENG: Set to true for SRM level User to press the "Set As Default" even though calibration fail.
            }

            if (radioBtn_pixelpermm.Checked)
            {
                txt_WidthOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_XResolution.Text) * 0.1f);
                txt_HeightOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_YResolution.Text) * 0.1f);
                txt_ZOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_ZResolution.Text) * 0.1f);
            }
            else
            {
                txt_WidthOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_XResolution.Text)) * 0.1f);
                txt_HeightOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_YResolution.Text)) * 0.1f);
                txt_ZOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_ZResolution.Text)) * 0.1f);
            }

        }




        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (m_blnCalibrationDone)
            {
                if (!m_blnApplyOffsetValueDone)
                {
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].SaveImage(m_strFolderPath + "CalibrationImage.bmp");
                    }
                    else
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].SaveImage(m_strFolderPath + "CalibrationImage.bmp");
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
                objXml.WriteElement1Value("SelectedImage", m_smVisionInfo.g_objCalibration.ref_intSelectedImage);
                objXml.WriteElement1Value("ImageGain", m_smVisionInfo.g_objCalibration.ref_fImageGain);
                objXml.WriteElement1Value("SelectedGaugeTool", m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool);
                objXml.WriteElement1Value("SizeX", m_smVisionInfo.g_objCalibration.ref_fSizeX);
                objXml.WriteElement1Value("SizeY", m_smVisionInfo.g_objCalibration.ref_fSizeY);
                objXml.WriteElement1Value("SizeZ", m_smVisionInfo.g_objCalibration.ref_fSizeZ);
                objXml.WriteElement1Value("SizeDiameter", m_smVisionInfo.g_objCalibration.ref_fSizeDiameter);

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

                // 2021 10 25 - CCENG: Need to save this CalibrationTolerance value to xml also bcos this variable wont auto save during change text.
                objXml.WriteSectionElement("FactoryCalibration");
                objXml.WriteElement1Value("CalibrateTolerance", txt_CalTol.Text);
                objXml.WriteEndElement();

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


                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Calibration", m_smProductionInfo.g_strLotID);
                STDeviceEdit.CopySettingFile(m_strFolderPath, strFileName + "Calibration.xml");
                ROI.SaveFile(m_strFolderPath + strFileName + "Calibration.xml", m_smVisionInfo.g_objCalibration.ref_arrROIs);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Calibration", m_smProductionInfo.g_strLotID);

                if (m_smVisionInfo.g_objCalibration.ref_arrLGauge != null)
                    LGauge.SaveFile(m_strFolderPath + "Pad\\CalibrationLGauge4L.xml", m_smVisionInfo.g_objCalibration.ref_arrLGauge);

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

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            CloseForm();
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

        private void txt_ResolutionOffSet_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (radioBtn_pixelpermm.Checked)
            {
                txt_XResolution.Text = (m_fXResolutionPrev + Convert.ToSingle(txt_WidthOffSet.Text)).ToString();
                txt_YResolution.Text = (m_fYResolutionPrev + Convert.ToSingle(txt_HeightOffSet.Text)).ToString();
                txt_ZResolution.Text = (m_fZResolutionPrev + Convert.ToSingle(txt_ZOffSet.Text)).ToString();

                txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackX(0) / Convert.ToSingle(txt_XResolution.Text)).ToString();
                txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackY(0) / Convert.ToSingle(txt_YResolution.Text)).ToString();
                txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ() / Convert.ToSingle(txt_ZResolution.Text)).ToString();
            }
            else
            {
                txt_XResolution.Text = (1 / (m_fXResolutionPrev + Convert.ToSingle(txt_WidthOffSet.Text))).ToString();
                txt_YResolution.Text = (1 / (m_fYResolutionPrev + Convert.ToSingle(txt_HeightOffSet.Text))).ToString();
                txt_ZResolution.Text = (1 / (m_fZResolutionPrev + Convert.ToSingle(txt_ZOffSet.Text))).ToString();

                txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackX(0) / (1 / Convert.ToSingle(txt_XResolution.Text))).ToString();
                txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackY(0) / (1 / Convert.ToSingle(txt_YResolution.Text))).ToString();
                txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ() / (1 / Convert.ToSingle(txt_ZResolution.Text))).ToString();
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

                //txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackX(0) / Convert.ToSingle(txt_XResolution.Text)).ToString();
                //txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackY(0) / Convert.ToSingle(txt_YResolution.Text)).ToString();
                //txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ() / Convert.ToSingle(txt_ZResolution.Text)).ToString();

            }
            else if (radioBtn_mmperPixel.Checked && label1.Text != "mm/pixel")
            {
                label1.Text = label6.Text = label17.Text = "mm/pixel";

                txt_XResolution.Text = (1 / Convert.ToSingle(txt_XResolution.Text)).ToString();
                txt_YResolution.Text = (1 / Convert.ToSingle(txt_YResolution.Text)).ToString();
                txt_ZResolution.Text = (1 / Convert.ToSingle(txt_ZResolution.Text)).ToString();

                //txt_MeasuredX.Text = (m_smVisionInfo.g_objCalibration.GetBackX(0) / (1 / Convert.ToSingle(txt_XResolution.Text))).ToString();
                //txt_MeasuredY.Text = (m_smVisionInfo.g_objCalibration.GetBackY(0) / (1 / Convert.ToSingle(txt_YResolution.Text))).ToString();
                //txt_MeasuredZ.Text = (m_smVisionInfo.g_objCalibration.GetBackZ() / (1 / Convert.ToSingle(txt_ZResolution.Text))).ToString();
            }

        }

        private void cbo_SelectPadROIs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btn_PackageGaugeSetting_Click(object sender, EventArgs e)
        {
            int intPadSelectedIndex = m_smVisionInfo.g_intSelectedROI;

            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\CalibrationRectGauge4L.xml";

            AdvancedRectGauge4LForm objAdvancedRectGauge4LForm = new AdvancedRectGauge4LForm(m_smVisionInfo, strPath, AdvancedRectGauge4LForm.ReadFrom.Calibration, m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);

            if (objAdvancedRectGauge4LForm.ShowDialog() == DialogResult.Cancel)
            {
                // Set RectGauge4L Placement
                //PointF pCenter = new PointF(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterX,
                //                            m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterY);
                //m_smVisionInfo.g_arrPad[intPadSelectedIndex].SetRectGauge4LPlacement(pCenter, 0, m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROIHeight);
                //m_smVisionInfo.g_arrPad[intPadSelectedIndex].SetRectGauge4LPlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                m_smVisionInfo.g_arrPad[intPadSelectedIndex].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objWhiteImage);
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ImageGain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objCalibration.ref_fImageGain = Convert.ToSingle(txt_ImageGain.Value);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objCalibration.ref_fImageGain);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_SelectImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objCalibration.ref_intSelectedImage = cbo_SelectImage.SelectedIndex;

            m_smVisionInfo.g_objCalibration.ref_fImageGain = Convert.ToSingle(txt_ImageGain.Value);

            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objCalibration.ref_fImageGain);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowDraggingBox_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibration.ref_blnDrawDraggingBox = chk_ShowDraggingBox.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibration.ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void Calibration5SForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blncboImageView = false;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void txt_XDistance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_XDistance.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibration.ref_fSizeX = fValue;
            }
        }

        private void txt_YDistance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_YDistance.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibration.ref_fSizeY = fValue;
            }
        }

        private void txt_ZDistance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_ZDistance.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibration.ref_fSizeZ = fValue;
            }
        }

        private void btn_Camera_Click(object sender, EventArgs e)
        {

            CalibrationCameraSettingsImageView objCameraForm = new CalibrationCameraSettingsImageView(m_smVisionInfo, m_smCustomizeInfo.g_blnLEDiControl,
                m_strSelectedRecipe, m_objAVTFireGrab, m_objIDSCamera, m_objTeliCamera, m_smProductionInfo.g_intUserGroup, m_strFolderPath);//, m_intMergeType + 1);

            objCameraForm.Location = new Point(600, 310);
            if (objCameraForm.ShowDialog() == DialogResult.Cancel)
            {
                GetCalibrationCameraValue();
                GrabOneTime(false);//GrabOneTime(true);

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

        private void txt_CalTol_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            //SaveCurrentResultAsFactoryCalibrationRecord();
        }

        private void SaveCurrentResultAsFactoryCalibrationRecord()
        {
            XmlParser objXml;
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                objXml = new XmlParser(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                objXml = new XmlParser(m_strFolderPath + "Calibration.xml");

            objXml.WriteSectionElement("FactoryCalibration");

            float PX, PY, PZ;
            if (radioBtn_pixelpermm.Checked)
            {
                PX = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_XResolution.Text));
                PY = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_YResolution.Text));
                PZ = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_ZResolution.Text));
            }
            else
            {
                PX = (float)Convert.ToDouble(txt_XResolution.Text);
                PY = (float)Convert.ToDouble(txt_YResolution.Text);
                PZ = (float)Convert.ToDouble(txt_ZResolution.Text);
            }

            m_fXResolution_Ori = PX;
            m_fYResolution_Ori = PY;
            m_fZResolution_Ori = PZ;

            m_MaxSizeX = PX + (float)Convert.ToDouble(txt_CalTol.Text);
            m_MaxSizeY = PY + (float)Convert.ToDouble(txt_CalTol.Text);
            m_MaxSizeZ = PZ + (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSizeX = PX - (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSizeY = PY - (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSizeZ = PZ - (float)Convert.ToDouble(txt_CalTol.Text);

            if (m_MinSizeX < 0)
                m_MinSizeX = 0;
            if (m_MinSizeY < 0)
                m_MinSizeY = 0;
            if (m_MinSizeZ < 0)
                m_MinSizeZ = 0;

            // Keep MaxSize and MinSize due to old calibration value using this variables.
            objXml.WriteElement1Value("MaxSizeX", m_MaxSizeX);
            objXml.WriteElement1Value("MaxSizeY", m_MaxSizeY);
            objXml.WriteElement1Value("MaxSizeZ", m_MaxSizeZ);

            objXml.WriteElement1Value("MinSizeX", m_MinSizeX);
            objXml.WriteElement1Value("MinSizeY", m_MinSizeY);
            objXml.WriteElement1Value("MinSizeZ", m_MinSizeZ);

            // 2021 10 25 - CCENG: Save this Resolution_Ori value when user press "Set As Default" only.
            objXml.WriteElement1Value("XResolution_Ori", m_fXResolution_Ori);
            objXml.WriteElement1Value("YResolution_Ori", m_fYResolution_Ori);
            objXml.WriteElement1Value("ZResolution_Ori", m_fZResolution_Ori);

            objXml.WriteElement1Value("CalibrateTolerance", txt_CalTol.Text);

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

        private void txt_DiameterDistance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_DiameterDistance.Text, out fValue))
            {
                m_smVisionInfo.g_objCalibration.ref_fSizeDiameter = fValue;
            }
        }

        private void cbo_SelectGaugeTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool = cbo_SelectGaugeTool.SelectedIndex;

            if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 0)
            {
                panel3.Visible = true;
                panel4.Visible = true;
                panel5.Visible = false;
                panel6.Visible = true;

                if ((m_smCustomizeInfo.g_intWantLead3D & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    btn_Lead3DGaugeSetting.Visible = true;
                    btn_Lead3DGaugeSetting.BringToFront();
                }
                else
                {
                    btn_PackageGaugeSetting.Visible = true;
                    btn_PackageGaugeSetting.BringToFront();
                }

                //btn_CircleGaugeSetting.Visible = false;
                chk_ShowDraggingBox.Visible = true;
                chk_ShowSamplePoints.Visible = true;

                m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();
            }
            else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 1)
            {
                panel3.Visible = false;
                panel4.Visible = false;
                panel5.Visible = true;
                panel6.Visible = true;
                // 2019 05 03-CCENG: Temperary hide this gauge button setting for circle gauge bcos currently circle gauge setting is hard coded.
                //                   Can add gauge setting for circle gauge in future. Need to consider 5S gauge which have "5 RectGauge4L" or "1 Circle Gauge + 4 RectGauge4L".
                //btn_PackageGaugeSetting.Visible = false;
                //btn_Lead3DGaugeSetting.Visible = false;
                //btn_CircleGaugeSetting.Visible = true;
                if (m_smVisionInfo.g_intSelectedROI == 0)
                {
                    btn_CircleGaugeSetting.Visible = true;
                    btn_CircleGaugeSetting.BringToFront();
                }
                else
                {
                    btn_PackageGaugeSetting.Visible = true;
                    btn_PackageGaugeSetting.BringToFront();
                }
                chk_ShowDraggingBox.Visible = true;
                chk_ShowSamplePoints.Visible = true;

                m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();
                m_smVisionInfo.g_objCalibration.SetCircleGaugePlace();
            }
            else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 2)
            {
                panel3.Visible = false;
                panel4.Visible = false;
                panel5.Visible = true;
                panel6.Visible = true;
                // 2019 05 03-CCENG: Temperary hide this gauge button setting for circle gauge bcos currently circle gauge setting is hard coded.
                //                   Can add gauge setting for circle gauge in future. Need to consider 5S gauge which have "5 RectGauge4L" or "1 Circle Gauge + 4 RectGauge4L".
                //btn_PackageGaugeSetting.Visible = false;
                //btn_Lead3DGaugeSetting.Visible = false;
                //btn_CircleGaugeSetting.Visible = true;
                if (m_smVisionInfo.g_intSelectedROI == 0)
                {
                    btn_CircleGaugeSetting.Visible = true;
                    btn_CircleGaugeSetting.BringToFront();
                }
                else
                {
                    btn_LGaugeSetting.Visible = true;
                    btn_LGaugeSetting.BringToFront();
                }
                chk_ShowDraggingBox.Visible = true;
                chk_ShowSamplePoints.Visible = true;

                m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();
                m_smVisionInfo.g_objCalibration.SetLGaugePlace();
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_CircleGaugeSetting_Click(object sender, EventArgs e)
        {

            int intPadSelectedIndex = m_smVisionInfo.g_intSelectedROI;

            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\CalibrationCircleGauge.xml";

            AdvancedCircleGaugeForm objAdvancedCircleGaugeForm = new AdvancedCircleGaugeForm(m_smVisionInfo, strPath, m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo);

            if (objAdvancedCircleGaugeForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_objCalibration.ref_objCirGauge.Measure(m_smVisionInfo.g_objPackageImage);
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void btn_Lead3DGaugeSetting_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\CalibrationRectGauge4L.xml";

            AdvancedLead3DRectGauge4LForm objAdvancedLead3DRectGauge4LForm = new AdvancedLead3DRectGauge4LForm(m_smVisionInfo, strPath, m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName, m_smProductionInfo, m_smCustomizeInfo, AdvancedLead3DRectGauge4LForm.ReadFrom.Calibration);

            if (objAdvancedLead3DRectGauge4LForm.ShowDialog() == DialogResult.Cancel)
            {
                //// Set RectGauge4L Placement
                //PointF pCenter = new PointF(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterX,
                //                            m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROITotalCenterY);
                ////m_smVisionInfo.g_arrPad[intPadSelectedIndex].SetRectGauge4LPlacement(pCenter, 0, m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1].ref_ROIHeight);
                ////m_smVisionInfo.g_arrPad[intPadSelectedIndex].SetRectGauge4LPlacement(m_smVisionInfo.g_arrPadROIs[intPadSelectedIndex][1]);
                //m_smVisionInfo.g_arrPad[intPadSelectedIndex].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objWhiteImage);
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            m_smVisionInfo.g_objCalibration.SetRectGauge4LPlace();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_LGaugeSetting_Click(object sender, EventArgs e)
        {
            AdvancedLGaugeForm objAdvancedForm = new AdvancedLGaugeForm(m_smVisionInfo, m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\CalibrationLGauge4L.xml", m_smProductionInfo, true);

            if (objAdvancedForm.ShowDialog() == DialogResult.OK)
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

                if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 1)
                {
                    if (m_smVisionInfo.g_intSelectedROI == 0)
                    {
                        btn_CircleGaugeSetting.Visible = true;
                        btn_CircleGaugeSetting.BringToFront();
                    }
                    else
                    {
                        btn_PackageGaugeSetting.Visible = true;
                        btn_PackageGaugeSetting.BringToFront();
                    }

                }
                else if (m_smVisionInfo.g_objCalibration.ref_intSelectedGaugeTool == 2)
                {
                    if (m_smVisionInfo.g_intSelectedROI == 0)
                    {
                        btn_CircleGaugeSetting.Visible = true;
                        btn_CircleGaugeSetting.BringToFront();
                    }
                    else
                    {
                        btn_LGaugeSetting.Visible = true;
                        btn_LGaugeSetting.BringToFront();
                    }
                }
            }

        }

        private void Calibration5SForm_FormClosing(object sender, FormClosingEventArgs e)
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