using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public partial class CalibrationLead3DForm : Form
    {
        #region Member Variables   
        private CalibrationLead3DHelpForm m_objHelpForm;
        private ImageDrawing m_objImgHorizontal = new ImageDrawing(true);
        private ImageDrawing m_objImgVertical = new ImageDrawing(true);
        private bool m_blnPassHorizontal = false;
        private bool m_blnPassVertical = false;
        private bool m_blnHorizontal = false;
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
        private float m_f2DXResolutionPrev;
        private float m_f2DYResolutionPrev;
        private float m_f3DTResolutionPrev;
        private float m_f3DBResolutionPrev;
        private float m_f3DLResolutionPrev;
        private float m_f3DRResolutionPrev;
        private float m_f3DTAnglePrev;
        private float m_f3DBAnglePrev;
        private float m_f3DLAnglePrev;
        private float m_f3DRAnglePrev;
        private string m_strFolderPath = "";
        private string m_strSelectedRecipe = "Default";
        private float m_MaxSize2DX = -1;
        private float m_MaxSize2DY = -1;
        private float m_MaxSize3DT = -1;
        private float m_MaxSize3DB = -1;
        private float m_MaxSize3DL = -1;
        private float m_MaxSize3DR = -1;
        private float m_MinSize2DX = -1;
        private float m_MinSize2DY = -1;
        private float m_MinSize3DT = -1;
        private float m_MinSize3DB = -1;
        private float m_MinSize3DL = -1;
        private float m_MinSize3DR = -1;
        private float m_MaxTopAngle = -1;
        private float m_MaxBottomAngle = -1;
        private float m_MaxLeftAngle = -1;
        private float m_MaxRightAngle = -1;
        private float m_MinTopAngle = -1;
        private float m_MinBottomAngle = -1;
        private float m_MinLeftAngle = -1;
        private float m_MinRightAngle = -1;
        private int m_intSelectedROIPrev = 0;

        private float m_f2DXResolution_Ori;
        private float m_f2DYResolution_Ori;
        private float m_f3DTResolution_Ori;
        private float m_f3DBResolution_Ori;
        private float m_f3DLResolution_Ori;
        private float m_f3DRResolution_Ori;
        private float m_f3DTAngle_Ori;
        private float m_f3DBAngle_Ori;
        private float m_f3DLAngle_Ori;
        private float m_f3DRAngle_Ori;

        private VisionInfo m_smVisionInfo;
        //private Calibration m_objCalibrate = new Calibration();
        private UserRight m_objUserRight = new UserRight();
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private AVTVimba m_objAVTFireGrab;
        private IDSuEyeCamera m_objIDSCamera;
        private TeliCamera m_objTeliCamera;
        #endregion
        public CalibrationLead3DForm(VisionInfo smVisionInfo, string recipe, int grp, CustomOption smCustomizeInfo, AVTVimba objAVTFireGrab, IDSuEyeCamera objIDSCamera, TeliCamera objTeliCamera, ProductionInfo smProductionInfo)
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
            UpdateGUI();
            
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnDragROI = true;
            //m_smVisionInfo.g_intSelectedImage = 0;  // Only image 1 will be used for calibration.

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_intSelectedROIPrev = m_smVisionInfo.g_intSelectedROI;
            m_blnHorizontal = m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal = true;
            if (m_blnHorizontal)
            {
                btn_Horizontal.Image = imageList1.Images[2];
                btn_Vertical.Image = imageList1.Images[1];
                //pnl_Horizontal.BackColor = Color.Orange;
                //pnl_Vertical.BackColor = Color.FromArgb(210, 230, 255);
                grpBox_3DJigMeasurementTopBottom.Visible = true;
                grpBox_3DJigMeasurementLeftRight.Visible = false;
            }
            else
            {
                btn_Horizontal.Image = imageList1.Images[0];
                btn_Vertical.Image = imageList1.Images[3];
                //pnl_Vertical.BackColor = Color.Orange;
                //pnl_Horizontal.BackColor = Color.FromArgb(210, 230, 255);
                grpBox_3DJigMeasurementTopBottom.Visible = false;
                grpBox_3DJigMeasurementLeftRight.Visible = true;
            }
            
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

        private void CloseForm()
        {
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                LoadLead3DCalibrationSetting(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                LoadLead3DCalibrationSetting(m_strFolderPath + "Calibration.xml");
            m_objImgHorizontal.Dispose();
            m_objImgVertical.Dispose();
            //m_smVisionInfo.g_objCalibrationLead3D.Dispose();
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
            
            strChild3 = "Camera Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Camera.Enabled = false;
            }

            strChild3 = "Calibrate Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Calibrate.Enabled = false;
                btn_Calibrate_Hor.Enabled = false;
                btn_Calibrate_Ver.Enabled = false;
            }

            strChild3 = "Threshold Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Threshold.Enabled = false;
            }

            strChild3 = "Gauge Setting Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Lead3DGaugeSetting.Enabled = false;
            }

            strChild3 = "Orientation";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                grpBox_Orientation.Enabled = false;
            }

            strChild3 = "2D Measurement";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                grpBox_2DJigMeasurement.Enabled = false;
            }

            strChild3 = "3D Measurement";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                grpBox_3DJigMeasurementLeftRight.Enabled = false;
                grpBox_3DJigMeasurementTopBottom.Enabled = false;
            }

            strChild3 = "Resolution OffSet Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                groupBox14.Enabled = false;
                btn_ApplyCurrentValue.Enabled = false;
            }

            strChild3 = "Angle OffSet Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                groupBox3.Enabled = false;
                btn_ApplyCurrentValue.Enabled = false;
            }

            strChild3 = "Calibration Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                groupBox1.Enabled = false;
            }

            strChild3 = "Apply Offset To Current Calibration Value";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_ApplyCurrentValue.Enabled = false;
            }
        }

        /// <summary>
        /// Load calibration settings from xml
        /// </summary>
        private void LoadSettings()
        {
            if (m_smVisionInfo.g_objCalibrationLead3D == null)
                m_smVisionInfo.g_objCalibrationLead3D = new CalibrationLead3D();

            XmlParser objXml;
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                objXml = new XmlParser(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                objXml = new XmlParser(m_strFolderPath + "Calibration.xml");

            objXml.GetFirstSection("Settings");
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_objCalibrationLead3D.ref_intSelectedImage = objXml.GetValueAsInt("SelectedImage", 0);
            if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrImages.Count)
                m_smVisionInfo.g_intSelectedImage = 0;  // Make sure selected image index is not out of range.
                                                        //m_smVisionInfo.g_objCalibrationLead3D.ref_fImageGain = objXml.GetValueAsFloat("ImageGain", 1f);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DA1 = objXml.GetValueAsFloat("Size2DA1", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DA2 = objXml.GetValueAsFloat("Size2DA2", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC1 = objXml.GetValueAsFloat("Size2DC1", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC2 = objXml.GetValueAsFloat("Size2DC2", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC3 = objXml.GetValueAsFloat("Size2DC3", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC4 = objXml.GetValueAsFloat("Size2DC4", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC5 = objXml.GetValueAsFloat("Size2DC5", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC6 = objXml.GetValueAsFloat("Size2DC6", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC7 = objXml.GetValueAsFloat("Size2DC7", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC8 = objXml.GetValueAsFloat("Size2DC8", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC9 = objXml.GetValueAsFloat("Size2DC9", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC10 = objXml.GetValueAsFloat("Size2DC10", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT1 = objXml.GetValueAsFloat("Size3DT1", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT2 = objXml.GetValueAsFloat("Size3DT2", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT3 = objXml.GetValueAsFloat("Size3DT3", 0);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB1 = objXml.GetValueAsFloat("Size3DB1", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB2 = objXml.GetValueAsFloat("Size3DB2", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB3 = objXml.GetValueAsFloat("Size3DB3", 0);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL1 = objXml.GetValueAsFloat("Size3DL1", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL2 = objXml.GetValueAsFloat("Size3DL2", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL3 = objXml.GetValueAsFloat("Size3DL3", 0);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR1 = objXml.GetValueAsFloat("Size3DR1", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR2 = objXml.GetValueAsFloat("Size3DR2", 1);
            m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR3 = objXml.GetValueAsFloat("Size3DR3", 0);
            // Update GUI
            //cbo_SelectImage.SelectedIndex = m_smVisionInfo.g_objCalibrationLead3D.ref_intSelectedImage;
            txt_ADistance1.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DA1.ToString();
            txt_ADistance2.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DA2.ToString();
            txt_CDistance1.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC1.ToString();
            txt_CDistance2.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC2.ToString();
            txt_CDistance3.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC3.ToString();
            txt_CDistance4.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC4.ToString();
            txt_CDistance5.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC5.ToString();
            txt_CDistance6.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC6.ToString();
            txt_CDistance7.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC7.ToString();
            txt_CDistance8.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC8.ToString();
            txt_CDistance9.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC9.ToString();
            txt_CDistance10.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC10.ToString();
            txt_TDistance1.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT1.ToString();
            txt_TDistance2.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT2.ToString();
            txt_TDistance3.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT3.ToString();
            txt_BDistance1.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB1.ToString();
            txt_BDistance2.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB2.ToString();
            txt_BDistance3.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB3.ToString();
            txt_LDistance1.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL1.ToString();
            txt_LDistance2.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL2.ToString();
            txt_LDistance3.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL3.ToString();
            txt_RDistance1.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR1.ToString();
            txt_RDistance2.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR2.ToString();
            txt_RDistance3.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR3.ToString();

            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdCenter = objXml.GetValueAsInt("ThresholdCenter", -4);
            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdTop = objXml.GetValueAsInt("ThresholdTop", -4);
            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdBottom = objXml.GetValueAsInt("ThresholdBottom", -4);
            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdLeft = objXml.GetValueAsInt("ThresholdLeft", -4);
            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdRight = objXml.GetValueAsInt("ThresholdRight", -4);
            
            objXml.GetFirstSection("Calibrate");
            txt_2DXResolution.Text = objXml.GetValueAsString("Pixel2DX", "5");
            txt_2DYResolution.Text = objXml.GetValueAsString("Pixel2DY", "5");
            txt_3DTResolution.Text = objXml.GetValueAsString("Pixel3DT", "5");
            txt_3DBResolution.Text = objXml.GetValueAsString("Pixel3DB", "5");
            txt_3DLResolution.Text = objXml.GetValueAsString("Pixel3DL", "5");
            txt_3DRResolution.Text = objXml.GetValueAsString("Pixel3DR", "5");
            txt_FOV.Text = objXml.GetValueAsString("FOV", "1x1");

            if (radioBtn_mmperPixel.Checked)
            {
                txt_2DXResolution.Text = (1 / Convert.ToSingle(txt_2DXResolution.Text)).ToString();
                txt_2DYResolution.Text = (1 / Convert.ToSingle(txt_2DYResolution.Text)).ToString();
                txt_3DTResolution.Text = (1 / Convert.ToSingle(txt_3DTResolution.Text)).ToString();
                txt_3DBResolution.Text = (1 / Convert.ToSingle(txt_3DBResolution.Text)).ToString();
                txt_3DLResolution.Text = (1 / Convert.ToSingle(txt_3DLResolution.Text)).ToString();
                txt_3DRResolution.Text = (1 / Convert.ToSingle(txt_3DRResolution.Text)).ToString();
            }

            txt_3DTopAngle.Text = objXml.GetValueAsString("TopAngle", "0");
            txt_3DBottomAngle.Text = objXml.GetValueAsString("BottomAngle", "0");
            txt_3DLeftAngle.Text = objXml.GetValueAsString("LeftAngle", "0");
            txt_3DRightAngle.Text = objXml.GetValueAsString("RightAngle", "0");

            if (radioBtn_pixelpermm.Checked)
            {
                txt_2DXOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_2DXResolution.Text) * 0.1f);
                txt_2DYOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_2DYResolution.Text) * 0.1f);
                txt_3DTOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DTResolution.Text) * 0.1f);
                txt_3DBOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DBResolution.Text) * 0.1f);
                txt_3DLOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DLResolution.Text) * 0.1f);
                txt_3DROffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DRResolution.Text) * 0.1f);
            }
            else
            {
                txt_2DXOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_2DXResolution.Text)) * 0.1f);
                txt_2DYOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_2DYResolution.Text)) * 0.1f);
                txt_3DTOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_3DTResolution.Text)) * 0.1f);
                txt_3DBOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_3DBResolution.Text)) * 0.1f);
                txt_3DLOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_3DLResolution.Text)) * 0.1f);
                txt_3DROffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_3DRResolution.Text)) * 0.1f);
            }
            txt_3DTAngleOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DTopAngle.Text) * 0.1f);
            txt_3DBAngleOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DBottomAngle.Text) * 0.1f);
            txt_3DLAngleOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DLeftAngle.Text) * 0.1f);
            txt_3DRAngleOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DRightAngle.Text) * 0.1f);

            //if (radioBtn_pixelpermm.Checked)
            //{
            //    m_f2DXResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_2DXResolution.Text));
            //    m_f2DYResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_2DYResolution.Text));
            //    m_f3DTResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_3DTResolution.Text));
            //    m_f3DBResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_3DBResolution.Text));
            //    m_f3DLResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_3DLResolution.Text));
            //    m_f3DRResolution_Ori = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_3DRResolution.Text));
            //}
            //else
            //{
            //    m_f2DXResolution_Ori = (float)Convert.ToDouble(txt_2DXResolution.Text);
            //    m_f2DYResolution_Ori = (float)Convert.ToDouble(txt_2DYResolution.Text);
            //    m_f3DTResolution_Ori = (float)Convert.ToDouble(txt_3DTResolution.Text);
            //    m_f3DBResolution_Ori = (float)Convert.ToDouble(txt_3DBResolution.Text);
            //    m_f3DLResolution_Ori = (float)Convert.ToDouble(txt_3DLResolution.Text);
            //    m_f3DRResolution_Ori = (float)Convert.ToDouble(txt_3DRResolution.Text);
            //}
            //m_f3DTAngle_Ori = (float)Convert.ToDouble(txt_3DTopAngle.Text);
            //m_f3DBAngle_Ori = (float)Convert.ToDouble(txt_3DBottomAngle.Text);
            //m_f3DLAngle_Ori = (float)Convert.ToDouble(txt_3DLeftAngle.Text);
            //m_f3DRAngle_Ori = (float)Convert.ToDouble(txt_3DRightAngle.Text);

            txt_2DXOffSet.Text = objXml.GetValueAsString("OffSet2DX", "0");
            txt_2DYOffSet.Text = objXml.GetValueAsString("OffSet2DY", "0");
            txt_3DTOffSet.Text = objXml.GetValueAsString("OffSet3DT", "0");
            txt_3DBOffSet.Text = objXml.GetValueAsString("OffSet3DB", "0");
            txt_3DLOffSet.Text = objXml.GetValueAsString("OffSet3DL", "0");
            txt_3DROffSet.Text = objXml.GetValueAsString("OffSet3DR", "0");
            txt_3DTAngleOffSet.Text = objXml.GetValueAsString("AngleOffSet3DT", "0");
            txt_3DBAngleOffSet.Text = objXml.GetValueAsString("AngleOffSet3DB", "0");
            txt_3DLAngleOffSet.Text = objXml.GetValueAsString("AngleOffSet3DL", "0");
            txt_3DRAngleOffSet.Text = objXml.GetValueAsString("AngleOffSet3DR", "0");

            m_f3DTAnglePrev = Convert.ToSingle(txt_3DTopAngle.Text) - Convert.ToSingle(txt_3DTAngleOffSet.Text);
            m_f3DBAnglePrev = Convert.ToSingle(txt_3DBottomAngle.Text) - Convert.ToSingle(txt_3DBAngleOffSet.Text);
            m_f3DLAnglePrev = Convert.ToSingle(txt_3DLeftAngle.Text) - Convert.ToSingle(txt_3DLAngleOffSet.Text);
            m_f3DRAnglePrev = Convert.ToSingle(txt_3DRightAngle.Text) - Convert.ToSingle(txt_3DRAngleOffSet.Text);

            if (radioBtn_pixelpermm.Checked)
            {
                m_f2DXResolutionPrev = Convert.ToSingle(txt_2DXResolution.Text) - Convert.ToSingle(txt_2DXOffSet.Text);
                m_f2DYResolutionPrev = Convert.ToSingle(txt_2DYResolution.Text) - Convert.ToSingle(txt_2DYOffSet.Text);
                m_f3DTResolutionPrev = Convert.ToSingle(txt_3DTResolution.Text) - Convert.ToSingle(txt_3DTOffSet.Text);
                m_f3DBResolutionPrev = Convert.ToSingle(txt_3DBResolution.Text) - Convert.ToSingle(txt_3DBOffSet.Text);
                m_f3DLResolutionPrev = Convert.ToSingle(txt_3DLResolution.Text) - Convert.ToSingle(txt_3DLOffSet.Text);
                m_f3DRResolutionPrev = Convert.ToSingle(txt_3DRResolution.Text) - Convert.ToSingle(txt_3DROffSet.Text);
            }
            else
            {
                m_f2DXResolutionPrev = 1 / (Convert.ToSingle(txt_2DXResolution.Text) - Convert.ToSingle(txt_2DXOffSet.Text));
                m_f2DYResolutionPrev = 1 / (Convert.ToSingle(txt_2DYResolution.Text) - Convert.ToSingle(txt_2DYOffSet.Text));
                m_f3DTResolutionPrev = 1 / (Convert.ToSingle(txt_3DTResolution.Text) - Convert.ToSingle(txt_3DTOffSet.Text));
                m_f3DBResolutionPrev = 1 / (Convert.ToSingle(txt_3DBResolution.Text) - Convert.ToSingle(txt_3DBOffSet.Text));
                m_f3DLResolutionPrev = 1 / (Convert.ToSingle(txt_3DLResolution.Text) - Convert.ToSingle(txt_3DLOffSet.Text));
                m_f3DRResolutionPrev = 1 / (Convert.ToSingle(txt_3DRResolution.Text) - Convert.ToSingle(txt_3DROffSet.Text));
            }

            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                ROI.LoadFile(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs);
            else
                ROI.LoadFile(m_strFolderPath + "Calibration.xml", m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs);
            
            if (m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs.Count == 0)
            {
                m_smVisionInfo.g_objCalibrationLead3D.AddCalibrationROI(m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_objCalibrationLead3D.AddLGauge(m_smVisionInfo.g_WorldShape);
            }
            else if (m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs.Count != m_smVisionInfo.g_objCalibrationLead3D.ref_intROICount)
            {
                m_smVisionInfo.g_objCalibrationLead3D.AddCalibrationROI(m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_objCalibrationLead3D.AddLGauge(m_smVisionInfo.g_WorldShape);
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs.Count; i++)
                {
                    m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs[i].AttachImage(m_smVisionInfo.g_objPackageImage);
                }

                //m_smVisionInfo.g_objCalibrationLead3D.AddRectGauge4L(m_smVisionInfo.g_WorldShape);

                string strPath = m_smProductionInfo.g_strRecipePath +
                     m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\CalibrationGauge.xml";
                m_smVisionInfo.g_objCalibrationLead3D.AddLGauge(m_smVisionInfo.g_WorldShape);
                m_smVisionInfo.g_objCalibrationLead3D.LoadGauge(strPath, "CalibrationGauge");

     //           strPath = m_smProductionInfo.g_strRecipePath +
     //m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\CalibrationLGauge4L.xml";

     //           LGauge.LoadFile(strPath, m_smVisionInfo.g_objCalibrationLead3D.ref_arrLGauge, m_smVisionInfo.g_WorldShape);
            }
          
            objXml.GetFirstSection("FactoryCalibration");
            m_f2DXResolution_Ori = objXml.GetValueAsFloat("X2DResolution_Ori", 0);
            m_f2DYResolution_Ori = objXml.GetValueAsFloat("Y2DResolution_Ori", 0);
            m_f3DTResolution_Ori = objXml.GetValueAsFloat("T3DResolution_Ori", 0);
            m_f3DBResolution_Ori = objXml.GetValueAsFloat("B3DResolution_Ori", 0);
            m_f3DLResolution_Ori = objXml.GetValueAsFloat("L3DResolution_Ori", 0);
            m_f3DRResolution_Ori = objXml.GetValueAsFloat("R3DResolution_Ori", 0);
            m_f3DTAngle_Ori = objXml.GetValueAsFloat("T3DAngle_Ori", 0);
            m_f3DBAngle_Ori = objXml.GetValueAsFloat("B3DAngle_Ori", 0);
            m_f3DLAngle_Ori = objXml.GetValueAsFloat("L3DAngle_Ori", 0);
            m_f3DRAngle_Ori = objXml.GetValueAsFloat("R3DAngle_Ori", 0);
            m_MaxSize2DX = objXml.GetValueAsFloat("MaxSize2DX", -1);
            m_MaxSize2DY = objXml.GetValueAsFloat("MaxSize2DY", -1);
            m_MaxSize3DT = objXml.GetValueAsFloat("MaxSize3DT", -1);
            m_MaxSize3DB = objXml.GetValueAsFloat("MaxSize3DB", -1);
            m_MaxSize3DL = objXml.GetValueAsFloat("MaxSize3DL", -1);
            m_MaxSize3DR = objXml.GetValueAsFloat("MaxSize3DR", -1);
            m_MinSize2DX = objXml.GetValueAsFloat("MinSize2DX", -1);
            m_MinSize2DY = objXml.GetValueAsFloat("MinSize2DY", -1);
            m_MinSize3DT = objXml.GetValueAsFloat("MinSize3DT", -1);
            m_MinSize3DB = objXml.GetValueAsFloat("MinSize3DB", -1);
            m_MinSize3DL = objXml.GetValueAsFloat("MinSize3DL", -1);
            m_MinSize3DR = objXml.GetValueAsFloat("MinSize3DR", -1);
            m_MaxTopAngle = objXml.GetValueAsFloat("MaxTopAngle", -1);
            m_MaxBottomAngle = objXml.GetValueAsFloat("MaxBottomAngle", -1);
            m_MaxLeftAngle = objXml.GetValueAsFloat("MaxLeftAngle", -1);
            m_MaxRightAngle = objXml.GetValueAsFloat("MaxRightAngle", -1);
            m_MinTopAngle = objXml.GetValueAsFloat("MinTopAngle", -1);
            m_MinBottomAngle = objXml.GetValueAsFloat("MinBottomAngle", -1);
            m_MinLeftAngle = objXml.GetValueAsFloat("MinLeftAngle", -1);
            m_MinRightAngle = objXml.GetValueAsFloat("MinRightAngle", -1);
            txt_CalTol.Text = objXml.GetValueAsFloat("CalibrateTolerance", 0.002f).ToString();
            txt_AngleTol.Text = objXml.GetValueAsFloat("AngleTolerance", 0.5f).ToString();

            // 2021 10 25 - CCENG: For old calibraton xml file, Resolution_Ori variable is not exist. 
            //                     If Resolution_Ori no exist, then Resolution_Ori will direct calculate from m_MaxSize and m_MinSizeX. 
            if (m_f2DXResolution_Ori == 0)
            {
                if (m_MaxSize2DX != -1)
                {
                    m_f2DXResolution_Ori = (m_MaxSize2DX + m_MinSize2DX) / 2;
                    m_f2DYResolution_Ori = (m_MaxSize2DY + m_MinSize2DY) / 2;
                    m_f3DTResolution_Ori = (m_MaxSize3DT + m_MinSize3DT) / 2;
                    m_f3DBResolution_Ori = (m_MaxSize3DB + m_MinSize3DB) / 2;
                    m_f3DLResolution_Ori = (m_MaxSize3DL + m_MinSize3DL) / 2;
                    m_f3DRResolution_Ori = (m_MaxSize3DR + m_MinSize3DR) / 2;

                    m_f3DTAngle_Ori = (m_MaxTopAngle + m_MinTopAngle) / 2;
                    m_f3DBAngle_Ori = (m_MaxBottomAngle + m_MinBottomAngle) / 2;
                    m_f3DLAngle_Ori = (m_MaxLeftAngle + m_MinLeftAngle) / 2;
                    m_f3DRAngle_Ori = (m_MaxRightAngle + m_MinRightAngle) / 2;
                }
            }
        }

        private void UpdateGUI()
        {
            btn_Save.Enabled = false;   // 2018 10 18 - CCENG: save button will be enabled after calibration pass.
            btn_Set.Enabled = false;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (m_blnCalibrationDone)
            {
                //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].SaveImage(m_strFolderPath + "CalibrationImage.bmp");

                if (!m_blnApplyOffsetValueDone)
                {
                    m_objImgHorizontal.SaveImage(m_strFolderPath + "CalibrationImageHorizontal.bmp");
                    m_objImgVertical.SaveImage(m_strFolderPath + "CalibrationImageVertical.bmp");
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
                //objXml.WriteElement1Value("SelectedImage", m_smVisionInfo.g_objCalibrationLead3D.ref_intSelectedImage);
                objXml.WriteElement1Value("Size2DC1", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC1);
                objXml.WriteElement1Value("Size2DC2", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC2);
                objXml.WriteElement1Value("Size2DC3", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC3);
                objXml.WriteElement1Value("Size2DC4", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC4);
                objXml.WriteElement1Value("Size2DC5", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC5);
                objXml.WriteElement1Value("Size2DC6", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC6);
                objXml.WriteElement1Value("Size2DC7", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC7);
                objXml.WriteElement1Value("Size2DC8", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC8);
                objXml.WriteElement1Value("Size2DC9", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC9);
                objXml.WriteElement1Value("Size2DC10", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC10);
                objXml.WriteElement1Value("Size2DA1", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DA1);
                objXml.WriteElement1Value("Size2DA2", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DA2);
                objXml.WriteElement1Value("Size3DT1", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT1);
                objXml.WriteElement1Value("Size3DT2", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT2);
                objXml.WriteElement1Value("Size3DT3", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT3);
                objXml.WriteElement1Value("Size3DB1", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB1);
                objXml.WriteElement1Value("Size3DB2", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB2);
                objXml.WriteElement1Value("Size3DB3", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB3);
                objXml.WriteElement1Value("Size3DL1", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL1);
                objXml.WriteElement1Value("Size3DL2", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL2);
                objXml.WriteElement1Value("Size3DL3", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL3);
                objXml.WriteElement1Value("Size3DR1", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR1);
                objXml.WriteElement1Value("Size3DR2", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR2);
                objXml.WriteElement1Value("Size3DR3", m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR3);

                objXml.WriteElement1Value("ThresholdCenter", m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdCenter);
                objXml.WriteElement1Value("ThresholdTop", m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdTop);
                objXml.WriteElement1Value("ThresholdBottom", m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdBottom);
                objXml.WriteElement1Value("ThresholdLeft", m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdLeft);
                objXml.WriteElement1Value("ThresholdRight", m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdRight);

                objXml.WriteSectionElement("Calibrate");
                objXml.WriteElement1Value("FOV", txt_FOV.Text);

                if (radioBtn_pixelpermm.Checked)
                {
                    objXml.WriteElement1Value("Pixel2DX", txt_2DXResolution.Text);
                    objXml.WriteElement1Value("Pixel2DY", txt_2DYResolution.Text);
                    objXml.WriteElement1Value("Pixel3DT", txt_3DTResolution.Text);
                    objXml.WriteElement1Value("Pixel3DB", txt_3DBResolution.Text);
                    objXml.WriteElement1Value("Pixel3DL", txt_3DLResolution.Text);
                    objXml.WriteElement1Value("Pixel3DR", txt_3DRResolution.Text);
                }
                else
                {
                    objXml.WriteElement1Value("Pixel2DX", 1 / Convert.ToSingle(txt_2DXResolution.Text));
                    objXml.WriteElement1Value("Pixel2DY", 1 / Convert.ToSingle(txt_2DYResolution.Text));
                    objXml.WriteElement1Value("Pixel3DT", 1 / Convert.ToSingle(txt_3DTResolution.Text));
                    objXml.WriteElement1Value("Pixel3DB", 1 / Convert.ToSingle(txt_3DBResolution.Text));
                    objXml.WriteElement1Value("Pixel3DL", 1 / Convert.ToSingle(txt_3DLResolution.Text));
                    objXml.WriteElement1Value("Pixel3DR", 1 / Convert.ToSingle(txt_3DRResolution.Text));
                }

                objXml.WriteElement1Value("TopAngle", txt_3DTopAngle.Text);
                objXml.WriteElement1Value("BottomAngle", txt_3DBottomAngle.Text);
                objXml.WriteElement1Value("LeftAngle", txt_3DLeftAngle.Text);
                objXml.WriteElement1Value("RightAngle", txt_3DRightAngle.Text);

                objXml.WriteElement1Value("OffSet2DX", txt_2DXOffSet.Text);
                objXml.WriteElement1Value("OffSet2DY", txt_2DYOffSet.Text);
                objXml.WriteElement1Value("OffSet3DT", txt_3DTOffSet.Text);
                objXml.WriteElement1Value("OffSet3DB", txt_3DBOffSet.Text);
                objXml.WriteElement1Value("OffSet3DL", txt_3DLOffSet.Text);
                objXml.WriteElement1Value("OffSet3DR", txt_3DROffSet.Text);
                objXml.WriteElement1Value("AngleOffSet3DT", txt_3DTAngleOffSet.Text);
                objXml.WriteElement1Value("AngleOffSet3DB", txt_3DBAngleOffSet.Text);
                objXml.WriteElement1Value("AngleOffSet3DL", txt_3DLAngleOffSet.Text);
                objXml.WriteElement1Value("AngleOffSet3DR", txt_3DRAngleOffSet.Text);

                objXml.WriteElement1Value("CPixelX1", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX1 + (Convert.ToSingle(txt_2DXOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC1));
                objXml.WriteElement1Value("CPixelX2", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX2 + (Convert.ToSingle(txt_2DXOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC2));
                objXml.WriteElement1Value("CPixelX3", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX3 + (Convert.ToSingle(txt_2DXOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC3));
                objXml.WriteElement1Value("CPixelX4", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX4 + (Convert.ToSingle(txt_2DXOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC4));
                objXml.WriteElement1Value("CPixelX5", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX5 + (Convert.ToSingle(txt_2DXOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC5));
                objXml.WriteElement1Value("CPixelY1", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountY1 + (Convert.ToSingle(txt_2DYOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC1));
                objXml.WriteElement1Value("CPixelY2", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountY2 + (Convert.ToSingle(txt_2DYOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC2));
                objXml.WriteElement1Value("CPixelY3", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountY3 + (Convert.ToSingle(txt_2DYOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC3));
                objXml.WriteElement1Value("CPixelY4", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountY4 + (Convert.ToSingle(txt_2DYOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC4));
                objXml.WriteElement1Value("CPixelY5", m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountY5 + (Convert.ToSingle(txt_2DYOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC5));
                objXml.WriteElement1Value("TPixel1", m_smVisionInfo.g_objCalibrationLead3D.ref_f3DTopPixelCount1 + (Convert.ToSingle(txt_3DTOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT1));
                objXml.WriteElement1Value("TPixel2", m_smVisionInfo.g_objCalibrationLead3D.ref_f3DTopPixelCount2 + (Convert.ToSingle(txt_3DTOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT2));
                objXml.WriteElement1Value("BPixel1", m_smVisionInfo.g_objCalibrationLead3D.ref_f3DBottomPixelCount1 + (Convert.ToSingle(txt_3DBOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB1));
                objXml.WriteElement1Value("BPixel2", m_smVisionInfo.g_objCalibrationLead3D.ref_f3DBottomPixelCount2 + (Convert.ToSingle(txt_3DBOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB2));
                objXml.WriteElement1Value("LPixel1", m_smVisionInfo.g_objCalibrationLead3D.ref_f3DLeftPixelCount1 + (Convert.ToSingle(txt_3DLOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL1));
                objXml.WriteElement1Value("LPixel2", m_smVisionInfo.g_objCalibrationLead3D.ref_f3DLeftPixelCount2 + (Convert.ToSingle(txt_3DLOffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL2));
                objXml.WriteElement1Value("RPixel1", m_smVisionInfo.g_objCalibrationLead3D.ref_f3DRightPixelCount1 + (Convert.ToSingle(txt_3DROffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR1));
                objXml.WriteElement1Value("RPixel2", m_smVisionInfo.g_objCalibrationLead3D.ref_f3DRightPixelCount2 + (Convert.ToSingle(txt_3DROffSet.Text)));// * m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR2));

                //if (radioBtn_pixelpermm.Checked)
                //{
                //    m_smVisionInfo.g_fCalibPixelX = float.Parse(txt_2DXResolution.Text);
                //    m_smVisionInfo.g_fCalibPixelY = float.Parse(txt_2DYResolution.Text);
                //}
                //else
                //{
                //    m_smVisionInfo.g_fCalibPixelX = 1 / float.Parse(txt_2DXResolution.Text);
                //    m_smVisionInfo.g_fCalibPixelY = 1 / float.Parse(txt_2DYResolution.Text);
                //}

                //m_smVisionInfo.g_fCalibPixelXInUM = m_smVisionInfo.g_fCalibPixelX / 1000;
                //m_smVisionInfo.g_fCalibPixelYInUM = m_smVisionInfo.g_fCalibPixelY / 1000;
                //m_smVisionInfo.g_fCalibPixelZInUM = m_smVisionInfo.g_fCalibPixelZ / 1000;

                // 2021 10 25 - CCENG: Need to save this CalibrationTolerance value to xml also bcos this variable wont auto save during change text.
                objXml.WriteSectionElement("FactoryCalibration");
                objXml.WriteElement1Value("CalibrateTolerance", txt_CalTol.Text);

                objXml.WriteEndElement();

                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Calibration", m_smProductionInfo.g_strLotID);
                STDeviceEdit.CopySettingFile(m_strFolderPath, strFileName + "Calibration.xml");
                ROI.SaveFile(m_strFolderPath + strFileName + "Calibration.xml", m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Calibration", m_smProductionInfo.g_strLotID);

                //LGauge.SaveFile(m_strFolderPath + "Lead3D\\CalibrationLGauge4L.xml", m_smVisionInfo.g_objCalibrationLead3D.ref_arrLGauge);

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
                if (radioBtn_pixelpermm.Checked)
                {
                    objXml.WriteElement1Value("Pixel2DX", txt_2DXResolution.Text);
                    objXml.WriteElement1Value("Pixel2DY", txt_2DYResolution.Text);
                    objXml.WriteElement1Value("Pixel3DT", txt_3DTResolution.Text);
                    objXml.WriteElement1Value("Pixel3DB", txt_3DBResolution.Text);
                    objXml.WriteElement1Value("Pixel3DL", txt_3DLResolution.Text);
                    objXml.WriteElement1Value("Pixel3DR", txt_3DRResolution.Text);
                }
                else
                {
                    objXml.WriteElement1Value("Pixel2DX", 1 / Convert.ToSingle(txt_2DXResolution.Text));
                    objXml.WriteElement1Value("Pixel2DY", 1 / Convert.ToSingle(txt_2DYResolution.Text));
                    objXml.WriteElement1Value("Pixel3DT", 1 / Convert.ToSingle(txt_3DTResolution.Text));
                    objXml.WriteElement1Value("Pixel3DB", 1 / Convert.ToSingle(txt_3DBResolution.Text));
                    objXml.WriteElement1Value("Pixel3DL", 1 / Convert.ToSingle(txt_3DLResolution.Text));
                    objXml.WriteElement1Value("Pixel3DR", 1 / Convert.ToSingle(txt_3DRResolution.Text));
                }

                objXml.WriteElement1Value("OffSet2DX", txt_2DXOffSet.Text);
                objXml.WriteElement1Value("OffSet2DY", txt_2DYOffSet.Text);
                objXml.WriteElement1Value("OffSet3DT", txt_3DTOffSet.Text);
                objXml.WriteElement1Value("OffSet3DB", txt_3DBOffSet.Text);
                objXml.WriteElement1Value("OffSet3DL", txt_3DLOffSet.Text);
                objXml.WriteElement1Value("OffSet3DR", txt_3DROffSet.Text);


                //if (radioBtn_pixelpermm.Checked)
                //{
                //    m_smVisionInfo.g_fCalibPixelX = float.Parse(txt_2DXResolution.Text);
                //    m_smVisionInfo.g_fCalibPixelY = float.Parse(txt_2DYResolution.Text);
                //}
                //else
                //{
                //    m_smVisionInfo.g_fCalibPixelX = 1 / float.Parse(txt_2DXResolution.Text);
                //    m_smVisionInfo.g_fCalibPixelY = 1 / float.Parse(txt_2DYResolution.Text);
                //}

                //m_smVisionInfo.g_fCalibPixelXInUM = m_smVisionInfo.g_fCalibPixelX / 1000;
                //m_smVisionInfo.g_fCalibPixelYInUM = m_smVisionInfo.g_fCalibPixelY / 1000;
                //m_smVisionInfo.g_fCalibPixelZInUM = m_smVisionInfo.g_fCalibPixelZ / 1000;

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
                txt_2DXResolution.Text = (m_f2DXResolutionPrev + Convert.ToSingle(txt_2DXOffSet.Text)).ToString();
                txt_2DYResolution.Text = (m_f2DYResolutionPrev + Convert.ToSingle(txt_2DYOffSet.Text)).ToString();
                txt_3DTResolution.Text = (m_f3DTResolutionPrev + Convert.ToSingle(txt_3DTOffSet.Text)).ToString();
                txt_3DBResolution.Text = (m_f3DBResolutionPrev + Convert.ToSingle(txt_3DBOffSet.Text)).ToString();
                txt_3DLResolution.Text = (m_f3DLResolutionPrev + Convert.ToSingle(txt_3DLOffSet.Text)).ToString();
                txt_3DRResolution.Text = (m_f3DRResolutionPrev + Convert.ToSingle(txt_3DROffSet.Text)).ToString();
                
            }
            else
            {
                txt_2DXResolution.Text = (1 / (m_f2DXResolutionPrev + Convert.ToSingle(txt_2DXOffSet.Text))).ToString();
                txt_2DYResolution.Text = (1 / (m_f2DYResolutionPrev + Convert.ToSingle(txt_2DYOffSet.Text))).ToString();
                txt_3DTResolution.Text = (1 / (m_f3DTResolutionPrev + Convert.ToSingle(txt_3DTOffSet.Text))).ToString();
                txt_3DBResolution.Text = (1 / (m_f3DBResolutionPrev + Convert.ToSingle(txt_3DBOffSet.Text))).ToString();
                txt_3DLResolution.Text = (1 / (m_f3DLResolutionPrev + Convert.ToSingle(txt_3DLOffSet.Text))).ToString();
                txt_3DRResolution.Text = (1 / (m_f3DRResolutionPrev + Convert.ToSingle(txt_3DROffSet.Text))).ToString();
            }

        }
        private void txt_AngleOffSet_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fResult = 0;
            if (float.TryParse(txt_3DTAngleOffSet.Text, out fResult))
            {
                txt_3DTopAngle.Text = (m_f3DTAnglePrev + Convert.ToSingle(txt_3DTAngleOffSet.Text)).ToString();
            }
            if (float.TryParse(txt_3DBAngleOffSet.Text, out fResult))
            {
                txt_3DBottomAngle.Text = (m_f3DBAnglePrev + Convert.ToSingle(txt_3DBAngleOffSet.Text)).ToString();
            }
            if (float.TryParse(txt_3DLAngleOffSet.Text, out fResult))
            {
                txt_3DLeftAngle.Text = (m_f3DLAnglePrev + Convert.ToSingle(txt_3DLAngleOffSet.Text)).ToString();
            }
            if (float.TryParse(txt_3DRAngleOffSet.Text, out fResult))
            {
                txt_3DRightAngle.Text = (m_f3DRAnglePrev + Convert.ToSingle(txt_3DRAngleOffSet.Text)).ToString();
            }
        }
        private void radioBtn_mmperPixel_Click(object sender, EventArgs e)
        {

            if (radioBtn_pixelpermm.Checked && label1.Text != "pixel/mm")
            {
                label46.Text = "pixel/mm";
                label47.Text = "pixel/mm";
                label44.Text = "pixel/mm";
                label41.Text = "pixel/mm";
                label17.Text = "pixel/mm";
                label1.Text = "pixel/mm";

                txt_2DXResolution.Text = (1 / Convert.ToSingle(txt_2DXResolution.Text)).ToString();
                txt_2DYResolution.Text = (1 / Convert.ToSingle(txt_2DYResolution.Text)).ToString();
                txt_3DTResolution.Text = (1 / Convert.ToSingle(txt_3DTResolution.Text)).ToString();
                txt_3DBResolution.Text = (1 / Convert.ToSingle(txt_3DBResolution.Text)).ToString();
                txt_3DLResolution.Text = (1 / Convert.ToSingle(txt_3DLResolution.Text)).ToString();
                txt_3DRResolution.Text = (1 / Convert.ToSingle(txt_3DRResolution.Text)).ToString();

            }
            else if (radioBtn_mmperPixel.Checked && label1.Text != "mm/pixel")
            {
                label46.Text = "mm/pixel";
                label47.Text = "mm/pixel";
                label44.Text = "mm/pixel";
                label41.Text = "mm/pixel";
                label17.Text = "mm/pixel";
                label1.Text = "mm/pixel";

                txt_2DXResolution.Text = (1 / Convert.ToSingle(txt_2DXResolution.Text)).ToString();
                txt_2DYResolution.Text = (1 / Convert.ToSingle(txt_2DYResolution.Text)).ToString();
                txt_3DTResolution.Text = (1 / Convert.ToSingle(txt_3DTResolution.Text)).ToString();
                txt_3DBResolution.Text = (1 / Convert.ToSingle(txt_3DBResolution.Text)).ToString();
                txt_3DLResolution.Text = (1 / Convert.ToSingle(txt_3DLResolution.Text)).ToString();
                txt_3DRResolution.Text = (1 / Convert.ToSingle(txt_3DRResolution.Text)).ToString();

            }

        }
        private void btn_Camera_Click(object sender, EventArgs e)
        {
            if (m_objHelpForm != null && !m_objHelpForm.IsDisposed)
            {
                m_objHelpForm.TopMost = false;
            }
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
            if (m_objHelpForm != null && !m_objHelpForm.IsDisposed)
            {
                m_objHelpForm.TopMost = true;
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

            float P2DX, P2DY, P3DT, P3DB, P3DL, P3DR;
            if (radioBtn_pixelpermm.Checked)
            {
                P2DX = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_2DXResolution.Text));
                P2DY = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_2DYResolution.Text));
                P3DT = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_3DTResolution.Text));
                P3DB = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_3DBResolution.Text));
                P3DL = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_3DLResolution.Text));
                P3DR = (float)Convert.ToDouble(1 / Convert.ToSingle(txt_3DRResolution.Text));
            }
            else
            {
                P2DX = (float)Convert.ToDouble(txt_2DXResolution.Text);
                P2DY = (float)Convert.ToDouble(txt_2DYResolution.Text);
                P3DT = (float)Convert.ToDouble(txt_3DTResolution.Text);
                P3DB = (float)Convert.ToDouble(txt_3DBResolution.Text);
                P3DL = (float)Convert.ToDouble(txt_3DLResolution.Text);
                P3DR = (float)Convert.ToDouble(txt_3DRResolution.Text);
            }

            m_f2DXResolution_Ori = P2DX;
            m_f2DYResolution_Ori = P2DY;
            m_f3DTResolution_Ori = P3DT;
            m_f3DBResolution_Ori = P3DB;
            m_f3DLResolution_Ori = P3DL;
            m_f3DRResolution_Ori = P3DR;

            m_f3DTAngle_Ori = (float)Convert.ToDouble(txt_3DTopAngle.Text);
            m_f3DBAngle_Ori = (float)Convert.ToDouble(txt_3DBottomAngle.Text);
            m_f3DLAngle_Ori = (float)Convert.ToDouble(txt_3DLeftAngle.Text);
            m_f3DRAngle_Ori = (float)Convert.ToDouble(txt_3DRightAngle.Text);

            m_MaxSize2DX = P2DX + (float)Convert.ToDouble(txt_CalTol.Text);
            m_MaxSize2DY = P2DY + (float)Convert.ToDouble(txt_CalTol.Text);
            m_MaxSize3DT = P3DT + (float)Convert.ToDouble(txt_CalTol.Text);
            m_MaxSize3DB = P3DB + (float)Convert.ToDouble(txt_CalTol.Text);
            m_MaxSize3DL = P3DL + (float)Convert.ToDouble(txt_CalTol.Text);
            m_MaxSize3DR = P3DR + (float)Convert.ToDouble(txt_CalTol.Text);

            m_MinSize2DX = P2DX - (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSize2DY = P2DY - (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSize3DT = P3DT - (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSize3DB = P3DB - (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSize3DL = P3DL - (float)Convert.ToDouble(txt_CalTol.Text);
            m_MinSize3DR = P3DR - (float)Convert.ToDouble(txt_CalTol.Text);

            if (m_MinSize2DX < 0)
                m_MinSize2DX = 0;
            if (m_MinSize2DY < 0)
                m_MinSize2DY = 0;
            if (m_MinSize3DT < 0)
                m_MinSize3DT = 0;
            if (m_MinSize3DB < 0)
                m_MinSize3DB = 0;
            if (m_MinSize3DL < 0)
                m_MinSize3DL = 0;
            if (m_MinSize3DR < 0)
                m_MinSize3DR = 0;

            m_MaxTopAngle = (float)Convert.ToDouble(txt_3DTopAngle.Text) + (float)Convert.ToDouble(txt_AngleTol.Text);
            m_MaxBottomAngle = (float)Convert.ToDouble(txt_3DBottomAngle.Text) + (float)Convert.ToDouble(txt_AngleTol.Text);
            m_MaxLeftAngle = (float)Convert.ToDouble(txt_3DLeftAngle.Text) + (float)Convert.ToDouble(txt_AngleTol.Text);
            m_MaxRightAngle = (float)Convert.ToDouble(txt_3DRightAngle.Text) + (float)Convert.ToDouble(txt_AngleTol.Text);

            m_MinTopAngle = (float)Convert.ToDouble(txt_3DTopAngle.Text) - (float)Convert.ToDouble(txt_AngleTol.Text);
            m_MinBottomAngle = (float)Convert.ToDouble(txt_3DBottomAngle.Text) - (float)Convert.ToDouble(txt_AngleTol.Text);
            m_MinLeftAngle = (float)Convert.ToDouble(txt_3DLeftAngle.Text) - (float)Convert.ToDouble(txt_AngleTol.Text);
            m_MinRightAngle = (float)Convert.ToDouble(txt_3DRightAngle.Text) - (float)Convert.ToDouble(txt_AngleTol.Text);

            // Keep MaxSize and MinSize due to old calibration value using this variables.
            objXml.WriteElement1Value("MaxSize2DX", m_MaxSize2DX);
            objXml.WriteElement1Value("MaxSize2DY", m_MaxSize2DY);
            objXml.WriteElement1Value("MaxSize3DT", m_MaxSize3DT);
            objXml.WriteElement1Value("MaxSize3DB", m_MaxSize3DB);
            objXml.WriteElement1Value("MaxSize3DL", m_MaxSize3DL);
            objXml.WriteElement1Value("MaxSize3DR", m_MaxSize3DR);

            objXml.WriteElement1Value("MinSize2DX", m_MinSize2DX);
            objXml.WriteElement1Value("MinSize2DY", m_MinSize2DY);
            objXml.WriteElement1Value("MinSize3DT", m_MinSize3DT);
            objXml.WriteElement1Value("MinSize3DB", m_MinSize3DB);
            objXml.WriteElement1Value("MinSize3DL", m_MinSize3DL);
            objXml.WriteElement1Value("MinSize3DR", m_MinSize3DR);

            objXml.WriteElement1Value("MaxTopAngle", m_MaxTopAngle);
            objXml.WriteElement1Value("MaxBottomAngle", m_MaxBottomAngle);
            objXml.WriteElement1Value("MaxLeftAngle", m_MaxLeftAngle);
            objXml.WriteElement1Value("MaxRightAngle", m_MaxRightAngle);

            objXml.WriteElement1Value("MinTopAngle", m_MinTopAngle);
            objXml.WriteElement1Value("MinBottomAngle", m_MinBottomAngle);
            objXml.WriteElement1Value("MinLeftAngle", m_MinLeftAngle);
            objXml.WriteElement1Value("MinRightAngle", m_MinRightAngle);

            objXml.WriteElement1Value("CalibrateTolerance", txt_CalTol.Text);
            objXml.WriteElement1Value("AngleTolerance", txt_AngleTol.Text);

            // 2021 10 25 - CCENG: Save this Resolution_Ori value when user press "Set As Default" only.
            objXml.WriteElement1Value("X2DResolution_Ori", m_f2DXResolution_Ori);
            objXml.WriteElement1Value("Y2DResolution_Ori", m_f2DYResolution_Ori);
            objXml.WriteElement1Value("T3DResolution_Ori", m_f3DTResolution_Ori);
            objXml.WriteElement1Value("B3DResolution_Ori", m_f3DBResolution_Ori);
            objXml.WriteElement1Value("L3DResolution_Ori", m_f3DLResolution_Ori);
            objXml.WriteElement1Value("R3DResolution_Ori", m_f3DRResolution_Ori);

            objXml.WriteElement1Value("T3DAngle_Ori", m_f3DTAngle_Ori);
            objXml.WriteElement1Value("B3DAngle_Ori", m_f3DBAngle_Ori);
            objXml.WriteElement1Value("L3DAngle_Ori", m_f3DLAngle_Ori);
            objXml.WriteElement1Value("R3DAngle_Ori", m_f3DRAngle_Ori);

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
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

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
                
            }

            if (m_blnHorizontal != m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal)
            {
                m_blnHorizontal = m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal;
                if (m_blnHorizontal)
                {
                    btn_Horizontal.Image = imageList1.Images[2];
                    btn_Vertical.Image = imageList1.Images[1];
                    //pnl_Horizontal.BackColor = Color.Orange;
                    ////pnl_Vertical.BackColor = Color.FromArgb(210, 230, 255);
                    grpBox_3DJigMeasurementTopBottom.Visible = true;
                    grpBox_3DJigMeasurementLeftRight.Visible = false;
                }
                else
                {
                    btn_Horizontal.Image = imageList1.Images[0];
                    btn_Vertical.Image = imageList1.Images[3];
                    //pnl_Vertical.BackColor = Color.Orange;
                    //pnl_Horizontal.BackColor = Color.FromArgb(210, 230, 255);
                    grpBox_3DJigMeasurementTopBottom.Visible = false;
                    grpBox_3DJigMeasurementLeftRight.Visible = true;
                }

            }
        }
        
        private void CalibrationLead3DForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_objHelpForm != null)
            {
                m_objHelpForm.Close();
                m_objHelpForm.Dispose();
            }
            m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Calibration Page Closed", "Exit Calibration Page", "", "", m_smProductionInfo.g_strLotID);
            
        }

        private void CalibrationLead3DForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
            m_smVisionInfo.g_blnViewPackageImage = true;
            m_smVisionInfo.g_blncboImageView = false;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void btn_Calibrate_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            AttachImage();

            if (m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal)
            {
                if (m_smVisionInfo.g_objCalibrationLead3D.MeasureTopBlob(m_smVisionInfo.g_objPackageImage, true) &&
                m_smVisionInfo.g_objCalibrationLead3D.MeasureCenterHorizontalTop(m_smVisionInfo.g_objPackageImage, true) &&
                m_smVisionInfo.g_objCalibrationLead3D.MeasureBottomBlob(m_smVisionInfo.g_objPackageImage, true) &&
                m_smVisionInfo.g_objCalibrationLead3D.MeasureCenterHorizontalBottom(m_smVisionInfo.g_objPackageImage, true))
                {
                    bool blnResult = true;

                    txt_3DTopAngle.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fTopAngle + Convert.ToSingle(txt_3DTAngleOffSet.Text)).ToString();
                    txt_3DBottomAngle.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fBottomAngle + Convert.ToSingle(txt_3DBAngleOffSet.Text)).ToString();

                    m_f3DTAnglePrev = Convert.ToSingle(txt_3DTopAngle.Text) - Convert.ToSingle(txt_3DTAngleOffSet.Text);
                    m_f3DBAnglePrev = Convert.ToSingle(txt_3DBottomAngle.Text) - Convert.ToSingle(txt_3DBAngleOffSet.Text);

                    if (radioBtn_pixelpermm.Checked)
                    {
                        txt_2DXResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration2DX + Convert.ToSingle(txt_2DXOffSet.Text)).ToString();
                        txt_3DTResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DTop + Convert.ToSingle(txt_3DTOffSet.Text)).ToString();
                        txt_3DBResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DBottom + Convert.ToSingle(txt_3DBOffSet.Text)).ToString();
                    }
                    else
                    {
                        txt_2DXResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration2DX + Convert.ToSingle(txt_2DXOffSet.Text))).ToString();
                        txt_3DTResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DTop + Convert.ToSingle(txt_3DTOffSet.Text))).ToString();
                        txt_3DBResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DBottom + Convert.ToSingle(txt_3DBOffSet.Text))).ToString();
                    }

                    if (radioBtn_pixelpermm.Checked)
                    {
                        m_f2DXResolutionPrev = Convert.ToSingle(txt_2DXResolution.Text) - Convert.ToSingle(txt_2DXOffSet.Text);
                        m_f3DTResolutionPrev = Convert.ToSingle(txt_3DTResolution.Text) - Convert.ToSingle(txt_3DTOffSet.Text);
                        m_f3DBResolutionPrev = Convert.ToSingle(txt_3DBResolution.Text) - Convert.ToSingle(txt_3DBOffSet.Text);
                    }
                    else
                    {
                        m_f2DXResolutionPrev = 1 / (Convert.ToSingle(txt_2DXResolution.Text) - Convert.ToSingle(txt_2DXOffSet.Text));
                        m_f3DTResolutionPrev = 1 / (Convert.ToSingle(txt_3DTResolution.Text) - Convert.ToSingle(txt_3DTOffSet.Text));
                        m_f3DBResolutionPrev = 1 / (Convert.ToSingle(txt_3DBResolution.Text) - Convert.ToSingle(txt_3DBOffSet.Text));
                    }

                    // ------------ Compare tolerance ------------------------------
                    float f2DX, f3DT, f3DB;
                    if (radioBtn_pixelpermm.Checked)
                    {
                        f2DX = 1f / Convert.ToSingle(txt_2DXResolution.Text);
                        f3DT = 1f / Convert.ToSingle(txt_3DTResolution.Text);
                        f3DB = 1f / Convert.ToSingle(txt_3DBResolution.Text);
                    }
                    else
                    {
                        f2DX = Convert.ToSingle(txt_2DXResolution.Text);
                        f3DT = Convert.ToSingle(txt_3DTResolution.Text);
                        f3DB = Convert.ToSingle(txt_3DBResolution.Text);
                    }

                    string strFailDetailMsg = "";
                    if (m_MaxSize2DX == -1 || m_MinSize2DX == -1 || m_MaxSize3DT == -1 || m_MinSize3DT == -1 || m_MaxSize3DB == -1 || m_MinSize3DB == -1 ||
                        m_MaxTopAngle == -1 || m_MinTopAngle == -1 || m_MaxBottomAngle == -1 || m_MinBottomAngle == -1)
                    {
                        if (SRMMessageBox.Show("No Factory Calibration Record in database. Would you like to store this calibration result as default?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaveCurrentResultAsFactoryCalibrationRecord();
                            blnResult = true;
                        }
                        else
                        {
                            strFailDetailMsg = "*No Factory Calibration record in database.";
                            blnResult = false;
                        }
                    }
                    else if (f2DX > m_MaxSize2DX || f2DX < m_MinSize2DX || f3DT > m_MaxSize3DT || f3DT < m_MinSize3DT || f3DB > m_MaxSize3DB || f3DB < m_MinSize3DB)
                    {
                        strFailDetailMsg = "*Result out of tolerance. Please make sure the Distance set value is correct.";
                        blnResult = false;
                    }
                    else if ((float)Convert.ToDouble(txt_3DTopAngle.Text) > m_MaxTopAngle || (float)Convert.ToDouble(txt_3DTopAngle.Text) < m_MinTopAngle ||
                        (float)Convert.ToDouble(txt_3DBottomAngle.Text) > m_MaxBottomAngle || (float)Convert.ToDouble(txt_3DBottomAngle.Text) < m_MinBottomAngle)
                    {
                        strFailDetailMsg = "*Angle Result out of tolerance. Please make sure the Distance set value is correct.";
                        blnResult = false;
                    }

                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = true;
                    if (blnResult)
                    {
                        m_smVisionInfo.g_objPackageImage.CopyTo(m_objImgHorizontal);
                        m_blnPassHorizontal = true;

                        lbl_HorizontalResultStatus.Text = "Pass";
                        lbl_HorizontalResultStatus.BackColor = Color.Lime;

                        m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                        m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Pass!";
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                    else
                    {
                        m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                        m_blnPassHorizontal = false;

                        lbl_HorizontalResultStatus.Text = "Fail";
                        lbl_HorizontalResultStatus.BackColor = Color.Red;

                        m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Fail!" + strFailDetailMsg;
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                }
                else
                {
                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                    m_blnPassHorizontal = false;

                    lbl_HorizontalResultStatus.Text = "Fail";
                    lbl_HorizontalResultStatus.BackColor = Color.Red;

                    m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Fail!" + m_smVisionInfo.g_objCalibrationLead3D.ref_strErrorMessage;
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                }
            }
            else
            {
                if (m_smVisionInfo.g_objCalibrationLead3D.MeasureLeftBlob(m_smVisionInfo.g_objPackageImage, true) &&
                m_smVisionInfo.g_objCalibrationLead3D.MeasureCenterVerticalLeft(m_smVisionInfo.g_objPackageImage, true) &&
                m_smVisionInfo.g_objCalibrationLead3D.MeasureRightBlob(m_smVisionInfo.g_objPackageImage, true) &&
                m_smVisionInfo.g_objCalibrationLead3D.MeasureCenterVerticalRight(m_smVisionInfo.g_objPackageImage, true))
                {
                    bool blnResult = true;

                    txt_3DLeftAngle.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fLeftAngle + Convert.ToSingle(txt_3DLAngleOffSet.Text)).ToString();
                    txt_3DRightAngle.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fRightAngle + Convert.ToSingle(txt_3DRAngleOffSet.Text)).ToString();

                    m_f3DLAnglePrev = Convert.ToSingle(txt_3DLeftAngle.Text) - Convert.ToSingle(txt_3DLAngleOffSet.Text);
                    m_f3DRAnglePrev = Convert.ToSingle(txt_3DRightAngle.Text) - Convert.ToSingle(txt_3DRAngleOffSet.Text);

                    if (radioBtn_pixelpermm.Checked)
                    {
                        txt_2DYResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration2DY + Convert.ToSingle(txt_2DYOffSet.Text)).ToString();
                        txt_3DLResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DLeft + Convert.ToSingle(txt_3DLOffSet.Text)).ToString();
                        txt_3DRResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DRight + Convert.ToSingle(txt_3DROffSet.Text)).ToString();
                    }
                    else
                    {
                        txt_2DYResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration2DY + Convert.ToSingle(txt_2DYOffSet.Text))).ToString();
                        txt_3DLResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DLeft + Convert.ToSingle(txt_3DLOffSet.Text))).ToString();
                        txt_3DRResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DRight + Convert.ToSingle(txt_3DROffSet.Text))).ToString();
                    }

                    if (radioBtn_pixelpermm.Checked)
                    {
                        m_f2DYResolutionPrev = Convert.ToSingle(txt_2DYResolution.Text) - Convert.ToSingle(txt_2DYOffSet.Text);
                        m_f3DLResolutionPrev = Convert.ToSingle(txt_3DLResolution.Text) - Convert.ToSingle(txt_3DLOffSet.Text);
                        m_f3DRResolutionPrev = Convert.ToSingle(txt_3DRResolution.Text) - Convert.ToSingle(txt_3DROffSet.Text);
                    }
                    else
                    {
                        m_f2DYResolutionPrev = 1 / Convert.ToSingle(txt_2DYResolution.Text) - Convert.ToSingle(txt_2DYOffSet.Text);
                        m_f3DLResolutionPrev = 1 / Convert.ToSingle(txt_3DLResolution.Text) - Convert.ToSingle(txt_3DLOffSet.Text);
                        m_f3DRResolutionPrev = 1 / Convert.ToSingle(txt_3DRResolution.Text) - Convert.ToSingle(txt_3DROffSet.Text);
                    }

                    // ------------ Compare tolerance ------------------------------
                    float f2DY, f3DL, f3DR;
                    if (radioBtn_pixelpermm.Checked)
                    {
                        f2DY = 1f / Convert.ToSingle(txt_2DYResolution.Text);
                        f3DL = 1f / Convert.ToSingle(txt_3DLResolution.Text);
                        f3DR = 1f / Convert.ToSingle(txt_3DRResolution.Text);
                    }
                    else
                    {
                        f2DY = Convert.ToSingle(txt_2DYResolution.Text);
                        f3DL = Convert.ToSingle(txt_3DLResolution.Text);
                        f3DR = Convert.ToSingle(txt_3DRResolution.Text);
                    }

                    string strFailDetailMsg = "";
                    if (m_MaxSize2DY == -1 || m_MinSize2DY == -1 || m_MaxSize3DL == -1 || m_MinSize3DL == -1 || m_MaxSize3DR == -1 || m_MinSize3DR == -1 ||
                        m_MaxLeftAngle == -1 || m_MinLeftAngle == -1 || m_MaxRightAngle == -1 || m_MinRightAngle == -1)
                    {
                        if (SRMMessageBox.Show("No Factory Calibration Record in database. Would you like to store this calibration result as default?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaveCurrentResultAsFactoryCalibrationRecord();
                            blnResult = true;
                        }
                        else
                        {
                            strFailDetailMsg = "*No Factory Calibration record in database.";
                            blnResult = false;
                        }
                    }
                    else if (f2DY > m_MaxSize2DY || f2DY < m_MinSize2DY || f3DL > m_MaxSize3DL || f3DL < m_MinSize3DL || f3DR > m_MaxSize3DR || f3DR < m_MinSize3DR)
                    {
                        strFailDetailMsg = "*Result out of tolerance. Please make sure the Distance set value is correct.";
                        blnResult = false;
                    }
                    else if ((float)Convert.ToDouble(txt_3DLeftAngle.Text) > m_MaxLeftAngle || (float)Convert.ToDouble(txt_3DLeftAngle.Text) < m_MinLeftAngle ||
                        (float)Convert.ToDouble(txt_3DRightAngle.Text) > m_MaxRightAngle || (float)Convert.ToDouble(txt_3DRightAngle.Text) < m_MinRightAngle)
                    {
                        strFailDetailMsg = "*Angle Result out of tolerance. Please make sure the Distance set value is correct.";
                        blnResult = false;
                    }

                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = true;
                    if (blnResult)
                    {
                        m_smVisionInfo.g_objPackageImage.CopyTo(m_objImgVertical);
                        m_blnPassVertical = true;

                        lbl_VerticalResultStatus.Text = "Pass";
                        lbl_VerticalResultStatus.BackColor = Color.Lime;

                        m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                        m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Pass!";
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                    else
                    {
                        m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                        m_blnPassVertical = false;

                        lbl_VerticalResultStatus.Text = "Fail";
                        lbl_VerticalResultStatus.BackColor = Color.Red;

                        m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Fail!" + strFailDetailMsg;
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                }
                else
                {
                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                    m_blnPassVertical = false;

                    lbl_VerticalResultStatus.Text = "Fail";
                    lbl_VerticalResultStatus.BackColor = Color.Red;

                    m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Fail!" + m_smVisionInfo.g_objCalibrationLead3D.ref_strErrorMessage;
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                }
            }
            if (m_blnPassHorizontal && m_blnPassVertical)
            {
                btn_Save.Enabled = true;
                m_blnCalibrationDone = true;
                btn_Set.Enabled = true;
                //chart1.Visible = true;
                //chart1.Series[0].Points.Clear();
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC1, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX1);
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC2, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX2);
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC3, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX3);
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC4, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX4);
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC5, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX5);
            }
            else
            {
                btn_Save.Enabled = false;
                m_blnCalibrationDone = false;
                btn_Set.Enabled = false;
                //chart1.Visible = false;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void AttachImage()
        {
            for (int i = 0; i < m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs.Count; i++)
            {
                m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs[i].AttachImage(m_smVisionInfo.g_objPackageImage);
            }
        }

        private void btn_Horizontal_Click(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
            m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Vertical_Click(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
            m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            if (m_objHelpForm != null && !m_objHelpForm.IsDisposed)
            {
                m_objHelpForm.TopMost = false;
            }

            int intSelectedROI = m_smVisionInfo.g_intSelectedROI;
          
            //for (int j = 0; j < m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs.Count; j++)
            //{
            //    if (m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs[j].GetROIHandle())   // 0: search ROI
            //    {
            //        intSelectedROI = j;
            //    }
            //}

            if (intSelectedROI == 0)
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdCenter;
            else if (intSelectedROI == 1)
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdTop;
            else if (intSelectedROI == 2)
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdRight;
            else if (intSelectedROI == 3)
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdBottom;
            else if (intSelectedROI == 4)
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdLeft;

            List<int> arrrThreshold = new List<int>();
            arrrThreshold.Add(m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdCenter);
            arrrThreshold.Add(m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdTop);
            arrrThreshold.Add(m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdRight);
            arrrThreshold.Add(m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdBottom);
            arrrThreshold.Add(m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdLeft);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs[intSelectedROI], true, true, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    for (int i = 0; i < m_smVisionInfo.g_objCalibrationLead3D.ref_arrROIs.Count; i++)
                    {
                      
                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intSelectedROI != 0 && i == 0)
                            continue;

                        if (i == 0)
                            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdCenter = m_smVisionInfo.g_intThresholdValue;
                        else if (i == 1)
                            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdTop = m_smVisionInfo.g_intThresholdValue;
                        else if (i == 2)
                            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdRight = m_smVisionInfo.g_intThresholdValue;
                        else if (i == 3)
                            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdBottom = m_smVisionInfo.g_intThresholdValue;
                        else if (i == 4)
                            m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdLeft = m_smVisionInfo.g_intThresholdValue;
                    }
                    
                }
                else
                {
                    if (intSelectedROI == 0)
                        m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdCenter = m_smVisionInfo.g_intThresholdValue;
                    else if (intSelectedROI == 1)
                        m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdTop = m_smVisionInfo.g_intThresholdValue;
                    else if (intSelectedROI == 2)
                        m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdRight = m_smVisionInfo.g_intThresholdValue;
                    else if (intSelectedROI == 3)
                        m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdBottom = m_smVisionInfo.g_intThresholdValue;
                    else if (intSelectedROI == 4)
                        m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdLeft = m_smVisionInfo.g_intThresholdValue;

                }
            }
            else
            {

                if (intSelectedROI == 0)
                    m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdCenter;
                else if (intSelectedROI == 1)
                    m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdTop;
                else if (intSelectedROI == 2)
                    m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdRight;
                else if (intSelectedROI == 3)
                    m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdBottom;
                else if (intSelectedROI == 4)
                    m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_objCalibrationLead3D.ref_intThresholdLeft;

            }

            objThresholdForm.Dispose();
            //m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_objHelpForm != null && !m_objHelpForm.IsDisposed)
            {
                m_objHelpForm.TopMost = true;
            }
        }

        private void btn_Lead3DGaugeSetting_Click(object sender, EventArgs e)
        {
            if (m_objHelpForm != null && !m_objHelpForm.IsDisposed)
            {
                m_objHelpForm.TopMost = false;
            }

            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\CalibrationGauge.xml";

            AdvancedLead3DCalibrationGaugeForm objAdvancedLead3DRectGauge4LForm = new AdvancedLead3DCalibrationGaugeForm(m_smVisionInfo, strPath, m_smProductionInfo, m_smCustomizeInfo);

            if (objAdvancedLead3DRectGauge4LForm.ShowDialog() == DialogResult.Cancel)
            {
              
            }
            else
            {
                if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                    m_smProductionInfo.g_blnSaveRecipeToServer = true;
            }
            objAdvancedLead3DRectGauge4LForm.Dispose();
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_objHelpForm != null && !m_objHelpForm.IsDisposed)
            {
                m_objHelpForm.TopMost = true;
            }

        }

        private void txt_CDistance1_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance1.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC1 = fValue;
            }
        }

        private void txt_CDistance2_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance2.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC2 = fValue;
            }
        }

        private void txt_CDistance3_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance3.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC3 = fValue;
            }
        }

        private void txt_CDistance4_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance4.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC4 = fValue;
            }
        }

        private void txt_CDistance5_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance5.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC5 = fValue;
            }
        }

        private void txt_CDistance6_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance6.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC6 = fValue;
            }
        }

        private void txt_CDistance7_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance7.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC7 = fValue;
            }
        }

        private void txt_CDistance8_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance8.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC8 = fValue;
            }
        }

        private void txt_CDistance9_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance9.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC9 = fValue;
            }
        }

        private void txt_CDistance10_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CDistance10.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC10 = fValue;
            }
        }

        private void txt_TDistance1_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_TDistance1.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT1 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL1 = fValue;
                txt_LDistance1.Text = fValue.ToString();
            }
        }

        private void txt_TDistance2_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_TDistance2.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT2 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL2 = fValue;
                txt_LDistance2.Text = fValue.ToString();
            }
        }

        private void txt_BDistance1_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_BDistance1.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB1 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR1 = fValue;
                txt_RDistance1.Text = fValue.ToString();
            }
        }

        private void txt_BDistance2_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_BDistance2.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB2 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR2 = fValue;
                txt_RDistance2.Text = fValue.ToString();
            }
        }

        private void txt_LDistance1_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_LDistance1.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL1 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT1 = fValue;
                txt_TDistance1.Text = fValue.ToString();
            }
        }

        private void txt_LDistance2_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_LDistance2.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL2 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT2 = fValue;
                txt_TDistance2.Text = fValue.ToString();
            }
        }

        private void txt_RDistance1_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_RDistance1.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR1 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB1 = fValue;
                txt_BDistance1.Text = fValue.ToString();
            }
        }

        private void txt_RDistance2_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_RDistance2.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR2 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB2 = fValue;
                txt_BDistance2.Text = fValue.ToString();
            }
        }

        private void LoadLead3DCalibrationSetting(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);


            if (m_smVisionInfo.g_arrLead3D.Length > 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {

                    if (i == 0)
                    {
                        objFile.GetFirstSection("Calibrate");
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX1 = objFile.GetValueAsFloat("CPixelX1", 5);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX2 = objFile.GetValueAsFloat("CPixelX2", 10);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX3 = objFile.GetValueAsFloat("CPixelX3", 15);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX4 = objFile.GetValueAsFloat("CPixelX4", 20);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX5 = objFile.GetValueAsFloat("CPixelX5", 25);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY1 = objFile.GetValueAsFloat("CPixelY1", 5);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY2 = objFile.GetValueAsFloat("CPixelY2", 10);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY3 = objFile.GetValueAsFloat("CPixelY3", 15);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY4 = objFile.GetValueAsFloat("CPixelY4", 20);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY5 = objFile.GetValueAsFloat("CPixelY5", 25);

                        objFile.GetFirstSection("Settings");

                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMX1 = objFile.GetValueAsFloat("Size2DC1", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMX2 = objFile.GetValueAsFloat("Size2DC2", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMX3 = objFile.GetValueAsFloat("Size2DC3", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMX4 = objFile.GetValueAsFloat("Size2DC4", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMX5 = objFile.GetValueAsFloat("Size2DC5", 1);

                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMY1 = objFile.GetValueAsFloat("Size2DC1", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMY2 = objFile.GetValueAsFloat("Size2DC2", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMY3 = objFile.GetValueAsFloat("Size2DC3", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMY4 = objFile.GetValueAsFloat("Size2DC4", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterMMY5 = objFile.GetValueAsFloat("Size2DC5", 1);

                        m_smVisionInfo.g_arrLead3D[i].Set2DCalibrationDataToFormLine(
                            new PointF(objFile.GetValueAsFloat("Size2DC1", 1), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX1),
                            new PointF(objFile.GetValueAsFloat("Size2DC2", 2), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX2),
                            new PointF(objFile.GetValueAsFloat("Size2DC3", 3), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX3),
                            new PointF(objFile.GetValueAsFloat("Size2DC4", 4), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX4),
                            new PointF(objFile.GetValueAsFloat("Size2DC5", 5), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountX5),
                            new PointF(objFile.GetValueAsFloat("Size2DC1", 1), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY1),
                        new PointF(objFile.GetValueAsFloat("Size2DC2", 2), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY2),
                        new PointF(objFile.GetValueAsFloat("Size2DC3", 3), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY3),
                        new PointF(objFile.GetValueAsFloat("Size2DC4", 4), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY4),
                        new PointF(objFile.GetValueAsFloat("Size2DC5", 5), m_smVisionInfo.g_arrLead3D[i].ref_f2DCenterPixelCountY5), m_smCustomizeInfo.g_intUnitDisplay);

                    }
                    else if (i == 1)
                    {
                        objFile.GetFirstSection("Calibrate");
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount1 = objFile.GetValueAsFloat("TPixel1", 5);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount2 = objFile.GetValueAsFloat("TPixel2", 10);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DAngle = objFile.GetValueAsFloat("TopAngle", 0);
                        objFile.GetFirstSection("Settings");

                        m_smVisionInfo.g_arrLead3D[i].ref_f3DMM1 = objFile.GetValueAsFloat("Size3DT1", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DMM2 = objFile.GetValueAsFloat("Size3DT2", 1);

                        m_smVisionInfo.g_arrLead3D[i].Set3DCalibrationDataToFormLine(
                          new PointF(objFile.GetValueAsFloat("Size3DT1", 1), m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount1),
                          new PointF(objFile.GetValueAsFloat("Size3DT2", 2), m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount2), m_smCustomizeInfo.g_intUnitDisplay);
                    }
                    else if (i == 3)
                    {
                        objFile.GetFirstSection("Calibrate");
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount1 = objFile.GetValueAsFloat("BPixel1", 5);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount2 = objFile.GetValueAsFloat("BPixel2", 10);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DAngle = objFile.GetValueAsFloat("BottomAngle", 0);
                        objFile.GetFirstSection("Settings");

                        m_smVisionInfo.g_arrLead3D[i].ref_f3DMM1 = objFile.GetValueAsFloat("Size3DB1", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DMM2 = objFile.GetValueAsFloat("Size3DB2", 1);

                        m_smVisionInfo.g_arrLead3D[i].Set3DCalibrationDataToFormLine(
                        new PointF(objFile.GetValueAsFloat("Size3DB1", 1), m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount1),
                        new PointF(objFile.GetValueAsFloat("Size3DB2", 2), m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount2), m_smCustomizeInfo.g_intUnitDisplay);
                    }
                    else if (i == 4)
                    {
                        objFile.GetFirstSection("Calibrate");
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount1 = objFile.GetValueAsFloat("LPixel1", 5);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount2 = objFile.GetValueAsFloat("LPixel2", 10);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DAngle = objFile.GetValueAsFloat("LeftAngle", 0);
                        objFile.GetFirstSection("Settings");

                        m_smVisionInfo.g_arrLead3D[i].ref_f3DMM1 = objFile.GetValueAsFloat("Size3DL1", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DMM2 = objFile.GetValueAsFloat("Size3DL2", 1);

                        m_smVisionInfo.g_arrLead3D[i].Set3DCalibrationDataToFormLine(
                        new PointF(objFile.GetValueAsFloat("Size3DL1", 1), m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount1),
                        new PointF(objFile.GetValueAsFloat("Size3DL2", 2), m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount2), m_smCustomizeInfo.g_intUnitDisplay);
                    }
                    else if (i == 2)
                    {
                        objFile.GetFirstSection("Calibrate");
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount1 = objFile.GetValueAsFloat("RPixel1", 5);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount2 = objFile.GetValueAsFloat("RPixel2", 10);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DAngle = objFile.GetValueAsFloat("RightAngle", 0);
                        objFile.GetFirstSection("Settings");

                        m_smVisionInfo.g_arrLead3D[i].ref_f3DMM1 = objFile.GetValueAsFloat("Size3DR1", 1);
                        m_smVisionInfo.g_arrLead3D[i].ref_f3DMM2 = objFile.GetValueAsFloat("Size3DR2", 1);

                        m_smVisionInfo.g_arrLead3D[i].Set3DCalibrationDataToFormLine(
                        new PointF(objFile.GetValueAsFloat("Size3DR1", 1), m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount1),
                        new PointF(objFile.GetValueAsFloat("Size3DR2", 2), m_smVisionInfo.g_arrLead3D[i].ref_f3DPixelCount2), m_smCustomizeInfo.g_intUnitDisplay);
                    }
                }
            }
        }

        private void pic_Help_Click(object sender, EventArgs e)
        {
            if (m_objHelpForm == null)
            {
                m_objHelpForm = new CalibrationLead3DHelpForm();
            }

            if (m_objHelpForm.IsDisposed)
            {
                m_objHelpForm = new CalibrationLead3DHelpForm();
            }

            m_objHelpForm.Show();
        }

        private void txt_ADistance1_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_ADistance1.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DA1 = fValue;
            }
        }

        private void txt_ADistance2_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_ADistance2.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DA2 = fValue;
            }
        }

        private void txt_TDistance3_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_TDistance3.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT3 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL3 = fValue;
                txt_LDistance3.Text = fValue.ToString();
            }
        }

        private void txt_BDistance3_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_BDistance3.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB3 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR3 = fValue;
                txt_RDistance3.Text = fValue.ToString();
            }
        }

        private void txt_LDistance3_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_LDistance3.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DL3 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DT3 = fValue;
                txt_TDistance3.Text = fValue.ToString();
            }
        }

        private void txt_RDistance3_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = 0;
            if (float.TryParse(txt_RDistance3.Text, out fValue))
            {

                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DR3 = fValue;
                m_smVisionInfo.g_objCalibrationLead3D.ref_fSize3DB3 = fValue;
                txt_BDistance3.Text = fValue.ToString();
            }
        }

        private void btn_Calibrate_Hor_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibrationLead3D.ref_fRotateAngle = 0;
            m_smVisionInfo.g_objCalibrationLead3D.ref_fRotateAngleSide = 0;
            m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal = true;
            Calibrate(m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal);
        }

        private void btn_Calibrate_Ver_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibrationLead3D.ref_fRotateAngle = 0;
            m_smVisionInfo.g_objCalibrationLead3D.ref_fRotateAngleSide = 0;
            m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal = false;
            Calibrate(m_smVisionInfo.g_objCalibrationLead3D.ref_blnHorizontal);
        }

        private void Calibrate(bool blnHorizontal)
        {
            if (!m_blnInitDone)
                return;

            AttachImage();

            if (blnHorizontal)
            {
                if (m_smVisionInfo.g_objCalibrationLead3D.MeasureTopBlob(m_smVisionInfo.g_objPackageImage, true) &&
                 m_smVisionInfo.g_objCalibrationLead3D.MeasureCenterHorizontalTop(m_smVisionInfo.g_objPackageImage, true) &&
                 m_smVisionInfo.g_objCalibrationLead3D.MeasureBottomBlob(m_smVisionInfo.g_objPackageImage, true) &&
                 m_smVisionInfo.g_objCalibrationLead3D.MeasureCenterHorizontalBottom(m_smVisionInfo.g_objPackageImage, true))
                {
                    m_smVisionInfo.g_objCalibrationLead3D.calaculateAngleAverage(0, true);
                    if (Math.Abs(m_smVisionInfo.g_objCalibrationLead3D.ref_fRotateAngle) < 1.5)
                    {
                        bool blnResult = true;

                        txt_3DTopAngle.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fTopAngle + Convert.ToSingle(txt_3DTAngleOffSet.Text)).ToString();
                        txt_3DBottomAngle.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fBottomAngle + Convert.ToSingle(txt_3DBAngleOffSet.Text)).ToString();

                        m_f3DTAnglePrev = Convert.ToSingle(txt_3DTopAngle.Text) - Convert.ToSingle(txt_3DTAngleOffSet.Text);
                        m_f3DBAnglePrev = Convert.ToSingle(txt_3DBottomAngle.Text) - Convert.ToSingle(txt_3DBAngleOffSet.Text);

                        if (radioBtn_pixelpermm.Checked)
                        {
                            txt_2DXResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration2DX + Convert.ToSingle(txt_2DXOffSet.Text)).ToString();
                            txt_3DTResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DTop + Convert.ToSingle(txt_3DTOffSet.Text)).ToString();
                            txt_3DBResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DBottom + Convert.ToSingle(txt_3DBOffSet.Text)).ToString();

                            //2020-12-10 ZJYEOH : FOV should based on txt_2DXResolution and txt_2DYResolution accordingly
                            //txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_2DXResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_2DXResolution.Text), 1).ToString();
                            txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_2DXResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_2DYResolution.Text), 1).ToString();
                        }
                        else
                        {
                            txt_2DXResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration2DX + Convert.ToSingle(txt_2DXOffSet.Text))).ToString();
                            txt_3DTResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DTop + Convert.ToSingle(txt_3DTOffSet.Text))).ToString();
                            txt_3DBResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DBottom + Convert.ToSingle(txt_3DBOffSet.Text))).ToString();

                            //2020-12-10 ZJYEOH : FOV should based on txt_2DXResolution and txt_2DYResolution accordingly
                            //txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_2DXResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_2DXResolution.Text)), 1).ToString();
                            txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_2DXResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_2DYResolution.Text)), 1).ToString();
                        }

                        if (radioBtn_pixelpermm.Checked)
                        {
                            m_f2DXResolutionPrev = Convert.ToSingle(txt_2DXResolution.Text) - Convert.ToSingle(txt_2DXOffSet.Text);
                            m_f3DTResolutionPrev = Convert.ToSingle(txt_3DTResolution.Text) - Convert.ToSingle(txt_3DTOffSet.Text);
                            m_f3DBResolutionPrev = Convert.ToSingle(txt_3DBResolution.Text) - Convert.ToSingle(txt_3DBOffSet.Text);
                        }
                        else
                        {
                            m_f2DXResolutionPrev = 1 / (Convert.ToSingle(txt_2DXResolution.Text) - Convert.ToSingle(txt_2DXOffSet.Text));
                            m_f3DTResolutionPrev = 1 / (Convert.ToSingle(txt_3DTResolution.Text) - Convert.ToSingle(txt_3DTOffSet.Text));
                            m_f3DBResolutionPrev = 1 / (Convert.ToSingle(txt_3DBResolution.Text) - Convert.ToSingle(txt_3DBOffSet.Text));
                        }

                        // ------------ Compare tolerance ------------------------------
                        float f2DX, f3DT, f3DB;
                        if (radioBtn_pixelpermm.Checked)
                        {
                            f2DX = 1f / Convert.ToSingle(txt_2DXResolution.Text);
                            f3DT = 1f / Convert.ToSingle(txt_3DTResolution.Text);
                            f3DB = 1f / Convert.ToSingle(txt_3DBResolution.Text);
                        }
                        else
                        {
                            f2DX = Convert.ToSingle(txt_2DXResolution.Text);
                            f3DT = Convert.ToSingle(txt_3DTResolution.Text);
                            f3DB = Convert.ToSingle(txt_3DBResolution.Text);
                        }

                        string strFailDetailMsg = "";
                        if (m_MaxSize2DX == -1 || m_MinSize2DX == -1 || m_MaxSize3DT == -1 || m_MinSize3DT == -1 || m_MaxSize3DB == -1 || m_MinSize3DB == -1 ||
                            m_MaxTopAngle == -1 || m_MinTopAngle == -1 || m_MaxBottomAngle == -1 || m_MinBottomAngle == -1)
                        {
                            if (SRMMessageBox.Show("No Factory Calibration Record in database. Would you like to store this calibration result as default?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                SaveCurrentResultAsFactoryCalibrationRecord();
                                blnResult = true;
                            }
                            else
                            {
                                strFailDetailMsg = "*No Factory Calibration record in database.";
                                blnResult = false;
                            }
                        }
                        //else if (f2DX > m_MaxSize2DX || f2DX < m_MinSize2DX || f3DT > m_MaxSize3DT || f3DT < m_MinSize3DT || f3DB > m_MaxSize3DB || f3DB < m_MinSize3DB)
                        else if (f2DX > (m_f2DXResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                         f2DX < (m_f2DXResolution_Ori - Convert.ToDouble(txt_CalTol.Text)) ||
                         f3DT > (m_f3DTResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                         f3DT < (m_f3DTResolution_Ori - Convert.ToDouble(txt_CalTol.Text)) ||
                         f3DB > (m_f3DBResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                         f3DB < (m_f3DBResolution_Ori - Convert.ToDouble(txt_CalTol.Text)))
                        {
                            strFailDetailMsg = "*Result out of tolerance. Please make sure the Distance set value is correct.";
                            blnResult = false;
                        }
                        //else if ((float)Convert.ToDouble(txt_3DTopAngle.Text) > m_MaxTopAngle || (float)Convert.ToDouble(txt_3DTopAngle.Text) < m_MinTopAngle ||
                        //    (float)Convert.ToDouble(txt_3DBottomAngle.Text) > m_MaxBottomAngle || (float)Convert.ToDouble(txt_3DBottomAngle.Text) < m_MinBottomAngle)
                        else if ((float)Convert.ToDouble(txt_3DTopAngle.Text) > (m_f3DTAngle_Ori + Convert.ToDouble(txt_AngleTol.Text)) ||
                        (float)Convert.ToDouble(txt_3DTopAngle.Text) < (m_f3DTAngle_Ori - Convert.ToDouble(txt_AngleTol.Text)) ||
                        (float)Convert.ToDouble(txt_3DBottomAngle.Text) > (m_f3DBAngle_Ori + Convert.ToDouble(txt_AngleTol.Text)) ||
                        (float)Convert.ToDouble(txt_3DBottomAngle.Text) < (m_f3DBAngle_Ori - Convert.ToDouble(txt_AngleTol.Text)))
                        {
                            strFailDetailMsg = "*Angle Result out of tolerance. Please make sure the Distance set value is correct.";
                            blnResult = false;
                        }
                        else if (m_smVisionInfo.g_objCalibrationLead3D.ref_arrLGauge[0][7].ref_ObjectCenterY - m_smVisionInfo.g_objCalibrationLead3D.ref_arrLGauge[0][1].ref_ObjectCenterY > 0)
                        {
                            strFailDetailMsg = "*Jig is inverse. Please place it properly.";
                            blnResult = false;
                        }

                        m_smVisionInfo.g_blnDrawLead3DCalibrationResult = true;
                        if (blnResult)
                        {
                            if (lbl_VerticalResultStatus.Text == "Pass")
                            {
                                double dVerticalAngleAverage = (Convert.ToDouble(txt_3DLeftAngle.Text) + Convert.ToDouble(txt_3DRightAngle.Text)) / 2;
                                double diffTop = Math.Abs(Convert.ToDouble(txt_3DTopAngle.Text) - dVerticalAngleAverage);
                                double diffBottom = Math.Abs(Convert.ToDouble(txt_3DBottomAngle.Text) - dVerticalAngleAverage);

                                if (diffTop < 5 && diffBottom < 5) //2021-07-18 ZJYEOH : temporary change 2 to 5, so that jig use for 26 degree top plate can pass
                                {
                                    m_smVisionInfo.g_objPackageImage.CopyTo(m_objImgHorizontal);
                                    m_blnPassHorizontal = true;

                                    lbl_HorizontalResultStatus.Text = "Pass";
                                    lbl_HorizontalResultStatus.BackColor = Color.Lime;

                                    m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                                    m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Pass!";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                                    m_blnPassHorizontal = false;

                                    lbl_HorizontalResultStatus.Text = "Fail";
                                    lbl_HorizontalResultStatus.BackColor = Color.Red;

                                    m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Fail!";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                    //////////////////////////////////////////////////////////
                                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                                    m_blnPassVertical = false;

                                    lbl_VerticalResultStatus.Text = "Fail";
                                    lbl_VerticalResultStatus.BackColor = Color.Red;

                                    m_smVisionInfo.g_strErrorMessage += "*Vertical Calibration Fail!" + "*Jig Angle Fail. Please adjust jig to 0 degree.";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                            }
                            else
                            {
                                if (Math.Abs(Convert.ToDouble(txt_3DTopAngle.Text) - Convert.ToDouble(txt_3DBottomAngle.Text)) < 5)//2021-07-18 ZJYEOH : temporary change 2 to 5, so that jig use for 26 degree top plate can pass
                                {
                                    m_smVisionInfo.g_objPackageImage.CopyTo(m_objImgHorizontal);
                                    m_blnPassHorizontal = true;

                                    lbl_HorizontalResultStatus.Text = "Pass";
                                    lbl_HorizontalResultStatus.BackColor = Color.Lime;

                                    m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                                    m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Pass!";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                                    m_blnPassHorizontal = false;

                                    lbl_HorizontalResultStatus.Text = "Fail";
                                    lbl_HorizontalResultStatus.BackColor = Color.Red;

                                    m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Fail!" + "*Jig Angle Fail. Please adjust jig to 0 degree.";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                            }
                        }
                        else
                        {
                            m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                            m_blnPassHorizontal = false;

                            lbl_HorizontalResultStatus.Text = "Fail";
                            lbl_HorizontalResultStatus.BackColor = Color.Red;

                            m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Fail!" + strFailDetailMsg;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }
                    }
                    else
                    {
                        m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                        m_blnPassHorizontal = false;

                        lbl_HorizontalResultStatus.Text = "Fail";
                        lbl_HorizontalResultStatus.BackColor = Color.Red;

                        m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Fail!" + "*Jig Angle Fail. Please adjust jig to 0 degree.";
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                }
                else
                {
                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                    m_blnPassHorizontal = false;

                    lbl_HorizontalResultStatus.Text = "Fail";
                    lbl_HorizontalResultStatus.BackColor = Color.Red;

                    m_smVisionInfo.g_strErrorMessage = "Horizontal Calibration Fail!" + m_smVisionInfo.g_objCalibrationLead3D.ref_strErrorMessage;
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                }
            }
            else
            {
                if (m_smVisionInfo.g_objCalibrationLead3D.MeasureLeftBlob(m_smVisionInfo.g_objPackageImage, true) &&
               m_smVisionInfo.g_objCalibrationLead3D.MeasureCenterVerticalLeft(m_smVisionInfo.g_objPackageImage, true) &&
               m_smVisionInfo.g_objCalibrationLead3D.MeasureRightBlob(m_smVisionInfo.g_objPackageImage, true) &&
               m_smVisionInfo.g_objCalibrationLead3D.MeasureCenterVerticalRight(m_smVisionInfo.g_objPackageImage, true))
                {
                    m_smVisionInfo.g_objCalibrationLead3D.calaculateAngleAverage(1, true);
                    if (Math.Abs(m_smVisionInfo.g_objCalibrationLead3D.ref_fRotateAngle) < 1.5)
                    {
                        bool blnResult = true;

                        txt_3DLeftAngle.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fLeftAngle + Convert.ToSingle(txt_3DLAngleOffSet.Text)).ToString();
                        txt_3DRightAngle.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fRightAngle + Convert.ToSingle(txt_3DRAngleOffSet.Text)).ToString();

                        m_f3DLAnglePrev = Convert.ToSingle(txt_3DLeftAngle.Text) - Convert.ToSingle(txt_3DLAngleOffSet.Text);
                        m_f3DRAnglePrev = Convert.ToSingle(txt_3DRightAngle.Text) - Convert.ToSingle(txt_3DRAngleOffSet.Text);

                        if (radioBtn_pixelpermm.Checked)
                        {
                            txt_2DYResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration2DY + Convert.ToSingle(txt_2DYOffSet.Text)).ToString();
                            txt_3DLResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DLeft + Convert.ToSingle(txt_3DLOffSet.Text)).ToString();
                            txt_3DRResolution.Text = (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DRight + Convert.ToSingle(txt_3DROffSet.Text)).ToString();

                            //2020-12-10 ZJYEOH : FOV should based on txt_2DXResolution and txt_2DYResolution accordingly
                            //txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_2DYResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_2DYResolution.Text), 1).ToString();
                            txt_FOV.Text = Math.Round((m_smVisionInfo.g_intCameraResolutionWidth / Convert.ToSingle(txt_2DXResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / Convert.ToSingle(txt_2DYResolution.Text), 1).ToString();
                        }
                        else
                        {
                            txt_2DYResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration2DY + Convert.ToSingle(txt_2DYOffSet.Text))).ToString();
                            txt_3DLResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DLeft + Convert.ToSingle(txt_3DLOffSet.Text))).ToString();
                            txt_3DRResolution.Text = (1 / (m_smVisionInfo.g_objCalibrationLead3D.ref_fCalibration3DRight + Convert.ToSingle(txt_3DROffSet.Text))).ToString();

                            //2020-12-10 ZJYEOH : FOV should based on txt_2DXResolution and txt_2DYResolution accordingly
                            //txt_FOV.Text = txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_2DYResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_2DYResolution.Text)), 1).ToString();
                            txt_FOV.Text = txt_FOV.Text = Math.Round(m_smVisionInfo.g_intCameraResolutionWidth / (1 / Convert.ToSingle(txt_2DXResolution.Text)), 1).ToString() + " x" + Math.Round(m_smVisionInfo.g_intCameraResolutionHeight / (1 / Convert.ToSingle(txt_2DYResolution.Text)), 1).ToString();
                        }

                        if (radioBtn_pixelpermm.Checked)
                        {
                            m_f2DYResolutionPrev = Convert.ToSingle(txt_2DYResolution.Text) - Convert.ToSingle(txt_2DYOffSet.Text);
                            m_f3DLResolutionPrev = Convert.ToSingle(txt_3DLResolution.Text) - Convert.ToSingle(txt_3DLOffSet.Text);
                            m_f3DRResolutionPrev = Convert.ToSingle(txt_3DRResolution.Text) - Convert.ToSingle(txt_3DROffSet.Text);
                        }
                        else
                        {
                            m_f2DYResolutionPrev = 1 / Convert.ToSingle(txt_2DYResolution.Text) - Convert.ToSingle(txt_2DYOffSet.Text);
                            m_f3DLResolutionPrev = 1 / Convert.ToSingle(txt_3DLResolution.Text) - Convert.ToSingle(txt_3DLOffSet.Text);
                            m_f3DRResolutionPrev = 1 / Convert.ToSingle(txt_3DRResolution.Text) - Convert.ToSingle(txt_3DROffSet.Text);
                        }

                        // ------------ Compare tolerance ------------------------------
                        float f2DY, f3DL, f3DR;
                        if (radioBtn_pixelpermm.Checked)
                        {
                            f2DY = 1f / Convert.ToSingle(txt_2DYResolution.Text);
                            f3DL = 1f / Convert.ToSingle(txt_3DLResolution.Text);
                            f3DR = 1f / Convert.ToSingle(txt_3DRResolution.Text);
                        }
                        else
                        {
                            f2DY = Convert.ToSingle(txt_2DYResolution.Text);
                            f3DL = Convert.ToSingle(txt_3DLResolution.Text);
                            f3DR = Convert.ToSingle(txt_3DRResolution.Text);
                        }

                        string strFailDetailMsg = "";
                        if (m_MaxSize2DY == -1 || m_MinSize2DY == -1 || m_MaxSize3DL == -1 || m_MinSize3DL == -1 || m_MaxSize3DR == -1 || m_MinSize3DR == -1 ||
                        m_MaxLeftAngle == -1 || m_MinLeftAngle == -1 || m_MaxRightAngle == -1 || m_MinRightAngle == -1)
                        {
                            if (SRMMessageBox.Show("No Factory Calibration Record in database. Would you like to store this calibration result as default?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                SaveCurrentResultAsFactoryCalibrationRecord();
                                blnResult = true;
                            }
                            else
                            {
                                strFailDetailMsg = "*No Factory Calibration record in database.";
                                blnResult = false;
                            }
                        }
                        //else if (f2DY > m_MaxSize2DY || f2DY < m_MinSize2DY || f3DL > m_MaxSize3DL || f3DL < m_MinSize3DL || f3DR > m_MaxSize3DR || f3DR < m_MinSize3DR)
                        else if (f2DY > (m_f2DYResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                        f2DY < (m_f2DYResolution_Ori - Convert.ToDouble(txt_CalTol.Text)) ||
                        f3DL > (m_f3DLResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                        f3DL < (m_f3DLResolution_Ori - Convert.ToDouble(txt_CalTol.Text)) ||
                        f3DR > (m_f3DRResolution_Ori + Convert.ToDouble(txt_CalTol.Text)) ||
                        f3DR < (m_f3DRResolution_Ori - Convert.ToDouble(txt_CalTol.Text)))
                        {
                            strFailDetailMsg = "*Result out of tolerance. Please make sure the Distance set value is correct.";
                            blnResult = false;
                        }
                        //else if ((float)Convert.ToDouble(txt_3DLeftAngle.Text) > m_MaxLeftAngle || (float)Convert.ToDouble(txt_3DLeftAngle.Text) < m_MinLeftAngle ||
                        //    (float)Convert.ToDouble(txt_3DRightAngle.Text) > m_MaxRightAngle || (float)Convert.ToDouble(txt_3DRightAngle.Text) < m_MinRightAngle)
                        else if ((float)Convert.ToDouble(txt_3DLeftAngle.Text) > (m_f3DLAngle_Ori + Convert.ToDouble(txt_AngleTol.Text)) ||
                        (float)Convert.ToDouble(txt_3DLeftAngle.Text) < (m_f3DLAngle_Ori - Convert.ToDouble(txt_AngleTol.Text)) ||
                        (float)Convert.ToDouble(txt_3DRightAngle.Text) > (m_f3DRAngle_Ori + Convert.ToDouble(txt_AngleTol.Text)) ||
                        (float)Convert.ToDouble(txt_3DRightAngle.Text) < (m_f3DRAngle_Ori - Convert.ToDouble(txt_AngleTol.Text)))
                        {
                            strFailDetailMsg = "*Angle Result out of tolerance. Please make sure the Distance set value is correct.";
                            blnResult = false;
                        }
                        else if (m_smVisionInfo.g_objCalibrationLead3D.ref_arrLGauge[0][0].ref_ObjectCenterX - m_smVisionInfo.g_objCalibrationLead3D.ref_arrLGauge[0][7].ref_ObjectCenterX > 0)
                        {
                            strFailDetailMsg = "*Jig is inverse. Please place it properly.";
                            blnResult = false;
                        }

                        m_smVisionInfo.g_blnDrawLead3DCalibrationResult = true;
                        if (blnResult)
                        {
                            if (lbl_HorizontalResultStatus.Text == "Pass")
                            {
                                double dHorizontalAngleAverage = (Convert.ToDouble(txt_3DTopAngle.Text) + Convert.ToDouble(txt_3DBottomAngle.Text)) / 2;
                                double diffLeft = Math.Abs(Convert.ToDouble(txt_3DLeftAngle.Text) - dHorizontalAngleAverage);
                                double diffRight = Math.Abs(Convert.ToDouble(txt_3DRightAngle.Text) - dHorizontalAngleAverage);

                                if (diffLeft < 5 && diffRight < 5)//2021-07-18 ZJYEOH : temporary change 2 to 5, so that jig use for 26 degree top plate can pass
                                {
                                    m_smVisionInfo.g_objPackageImage.CopyTo(m_objImgVertical);
                                    m_blnPassVertical = true;

                                    lbl_VerticalResultStatus.Text = "Pass";
                                    lbl_VerticalResultStatus.BackColor = Color.Lime;

                                    m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                                    m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Pass!";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                                    m_blnPassVertical = false;

                                    lbl_VerticalResultStatus.Text = "Fail";
                                    lbl_VerticalResultStatus.BackColor = Color.Red;

                                    m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Fail!";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                    ///////////////////////////////////////////////////
                                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                                    m_blnPassHorizontal = false;

                                    lbl_HorizontalResultStatus.Text = "Fail";
                                    lbl_HorizontalResultStatus.BackColor = Color.Red;

                                    m_smVisionInfo.g_strErrorMessage += "*Horizontal Calibration Fail!" + "*Jig Angle Fail. Please adjust jig to 0 degree.";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                            }
                            else
                            {
                                if (Math.Abs(Convert.ToDouble(txt_3DLeftAngle.Text) - Convert.ToDouble(txt_3DRightAngle.Text)) < 5)//2021-07-18 ZJYEOH : temporary change 2 to 5, so that jig use for 26 degree top plate can pass
                                {
                                    m_smVisionInfo.g_objPackageImage.CopyTo(m_objImgVertical);
                                    m_blnPassVertical = true;

                                    lbl_VerticalResultStatus.Text = "Pass";
                                    lbl_VerticalResultStatus.BackColor = Color.Lime;

                                    m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                                    m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Pass!";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                                    m_blnPassVertical = false;

                                    lbl_VerticalResultStatus.Text = "Fail";
                                    lbl_VerticalResultStatus.BackColor = Color.Red;

                                    m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Fail!" + "*Jig Angle Fail. Please adjust jig to 0 degree.";
                                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                }
                            }
                        }
                        else
                        {
                            m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                            m_blnPassVertical = false;

                            lbl_VerticalResultStatus.Text = "Fail";
                            lbl_VerticalResultStatus.BackColor = Color.Red;

                            m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Fail!" + strFailDetailMsg;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }
                    }
                    else
                    {
                        m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                        m_blnPassVertical = false;

                        lbl_VerticalResultStatus.Text = "Fail";
                        lbl_VerticalResultStatus.BackColor = Color.Red;

                        m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Fail!" + "*Jig Angle Fail. Please adjust jig to 0 degree.";
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                }
                else
                {
                    m_smVisionInfo.g_blnDrawLead3DCalibrationResult = false;
                    m_blnPassVertical = false;

                    lbl_VerticalResultStatus.Text = "Fail";
                    lbl_VerticalResultStatus.BackColor = Color.Red;

                    m_smVisionInfo.g_strErrorMessage = "Vertical Calibration Fail!" + m_smVisionInfo.g_objCalibrationLead3D.ref_strErrorMessage;
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                }
            }
            if (m_blnPassHorizontal && m_blnPassVertical)
            {
                btn_Save.Enabled = true;
                m_blnCalibrationDone = true;
                btn_Set.Enabled = true;
                //chart1.Visible = true;
                //chart1.Series[0].Points.Clear();
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC1, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX1);
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC2, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX2);
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC3, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX3);
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC4, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX4);
                //chart1.Series[0].Points.AddXY(m_smVisionInfo.g_objCalibrationLead3D.ref_fSize2DC5, m_smVisionInfo.g_objCalibrationLead3D.ref_f2DCenterPixelCountX5);
            }
            else
            {
                btn_Save.Enabled = false;
                m_blnCalibrationDone = false;
                btn_Set.Enabled = false;
                //chart1.Visible = false;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (radioBtn_pixelpermm.Checked)
            {
                txt_2DXOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_2DXResolution.Text) * 0.1f);
                txt_2DYOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_2DYResolution.Text) * 0.1f);
                txt_3DTOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DTResolution.Text) * 0.1f);
                txt_3DBOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DBResolution.Text) * 0.1f);
                txt_3DLOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DLResolution.Text) * 0.1f);
                txt_3DROffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DRResolution.Text) * 0.1f);
            }
            else
            {
                txt_2DXOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_2DXResolution.Text)) * 0.1f);
                txt_2DYOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_2DYResolution.Text)) * 0.1f);
                txt_3DTOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_3DTResolution.Text)) * 0.1f);
                txt_3DBOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_3DBResolution.Text)) * 0.1f);
                txt_3DLOffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_3DLResolution.Text)) * 0.1f);
                txt_3DROffSet.DecMaxValue = (decimal)((1 / Convert.ToDouble(txt_3DRResolution.Text)) * 0.1f);
            }
            txt_3DTAngleOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DTopAngle.Text) * 0.1f);
            txt_3DBAngleOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DBottomAngle.Text) * 0.1f);
            txt_3DLAngleOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DLeftAngle.Text) * 0.1f);
            txt_3DRAngleOffSet.DecMaxValue = (decimal)(Convert.ToDouble(txt_3DRightAngle.Text) * 0.1f);

        }

        private void btn_ApplyCurrentValue_Click(object sender, EventArgs e)
        {
            btn_Save.Enabled = true;
            m_blnApplyOffsetValueDone = true;
        }

        private void txt_AngleTol_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            //SaveCurrentResultAsFactoryCalibrationRecord();
        }
    }
}
