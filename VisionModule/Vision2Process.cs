using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using ImageAcquisition;
using IOMode;
using Lighting;
using SharedMemory;
using VisionProcessing;


namespace VisionModule
{
    public class Vision2Process
    {
        #region Member Variables

        // Thread handle
        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnPause = false;
        private bool m_blnStopped = false;
        private bool m_blnForceStopProduction = false;

        private int m_intGrabRequire = 0;
        private uint m_intCameraGainPrev = 1;
        private float m_fCameraShuttlePrev = 1f;

        private Thread m_thThread;
        private VisionIO m_objVisionIO;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private AVTVimba m_objAVTFireGrab;
        private TeliCamera m_objTeliCamera;
        private VisionComThread m_smComThread;
        private TCPIPIO m_smTCPIPIO;
        private RS232 m_thCOMMPort;

        #endregion


        public Vision2Process(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
            AVTVimba objAVTFireGrab, VisionComThread smComThread, RS232 thCOMMPort, TCPIPIO smTCPIPIO)
        {
            m_smCustomizeInfo = objCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = objVisionInfo;
            m_objAVTFireGrab = objAVTFireGrab;
            m_smComThread = smComThread;
            m_smTCPIPIO = smTCPIPIO;
            m_thCOMMPort = thCOMMPort;

            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                m_smTCPIPIO.ReceiveCommandEvent += new TCPIPIO.ReceiveCommandHandle(TakeAction);
            else
                m_smComThread.ReceiveCommandEvent += new VisionComThread.ReceiveCommandHandle(TakeAction);
            

            //create vision io object
            m_objVisionIO = new VisionIO(m_smVisionInfo.g_strVisionName, m_smVisionInfo.g_strVisionDisplayName,
                                         m_smVisionInfo.g_intVisionIndex, m_smVisionInfo.g_intVisionSameCount,
                                         m_smVisionInfo.g_strVisionNameRemark, 0);


            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Priority = ThreadPriority.Highest;
            m_thThread.Start();
        }

        public Vision2Process(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
            TeliCamera objTeliCamera, VisionComThread smComThread, RS232 thCOMMPort, TCPIPIO smTCPIPIO)
        {
            m_smCustomizeInfo = objCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = objVisionInfo;
            m_objTeliCamera = objTeliCamera;
            m_smComThread = smComThread;
            m_smTCPIPIO = smTCPIPIO;
            m_thCOMMPort = thCOMMPort;

            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                m_smTCPIPIO.ReceiveCommandEvent += new TCPIPIO.ReceiveCommandHandle(TakeAction);
            else
                m_smComThread.ReceiveCommandEvent += new VisionComThread.ReceiveCommandHandle(TakeAction);


            //create vision io object
            m_objVisionIO = new VisionIO(m_smVisionInfo.g_strVisionName, m_smVisionInfo.g_strVisionDisplayName,
                                         m_smVisionInfo.g_intVisionIndex, m_smVisionInfo.g_intVisionSameCount,
                                         m_smVisionInfo.g_strVisionNameRemark, 0);

            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Priority = ThreadPriority.Highest;
            m_thThread.Start();
        }
        private bool IsCameraInitDone()
        {
            switch (m_smVisionInfo.g_strCameraModel)
            {
                case "AVT":
                    return m_objAVTFireGrab.ref_blnCameraInitDone;
                    break;
                case "Teli":
                default:
                    return m_objTeliCamera.IsCameraInitDone();
                    break;
            }
        }
        /// <summary>
        /// Grab image
        /// </summary>      
        /// <returns>true = successfully grab image, false = fail to grab image</returns>
        public bool GrabImage()
        {
#if (DEBUG || Debug_2_12 || RTXDebug)
            return true;
#endif

            // 2021 06 27 - Blank image before new grab.
            if (m_smProductionInfo.g_blnBlankImageBeforeGrab)
            {
                if (!m_smProductionInfo.g_blnAllRunGrabWithoutUseImage && IsCameraInitDone())
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                    {
                        m_smVisionInfo.g_arrImages[i].SetImageToBlack();
                    }
                }
            }

            if (m_smVisionInfo.g_strCameraModel == "Teli")
            {
                return GrabImage_Teli();
            }

            if (!m_objAVTFireGrab.ref_blnCameraInitDone)
            {
                m_smVisionInfo.g_strErrorMessage = "Camera No Connected";
                return true;
            }

            m_smVisionInfo.g_objGrabTime.Start();
            bool blnSuccess = true;

            if (!m_objAVTFireGrab.Grab())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            Thread.Sleep(3);

            if (blnSuccess)
            {
                SetGrabDone();
                m_smVisionInfo.g_objTransferTime.Start();

                if (m_objAVTFireGrab.GetFrame(0))
                {
                    if (m_objAVTFireGrab.ConvertFrame(0))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, 0);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                            m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, 0);
                            m_smVisionInfo.g_arrImages[0].AddGain(m_smVisionInfo.g_arrImageGain[0]);
                            //m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                            //ImageDrawing.Rotate0Degree(m_smVisionInfo.g_objMemoryImage, 90, ref m_smVisionInfo.g_arrImages, 0);
                        }

                        m_objAVTFireGrab.ReleaseImage(0);
                    }
                }
            }
            else
                SetGrabDone();

            if (m_objAVTFireGrab.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objAVTFireGrab.ref_strErrorText;
                //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return true;
            }
        }

        public bool GrabImage_Teli()
        {
            m_smVisionInfo.g_objGrabTime.Start();
            bool blnSuccess = true;

            if (!m_objTeliCamera.Grab())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            Thread.Sleep(3);

            if (blnSuccess)
            {
                SetGrabDone();
                m_smVisionInfo.g_objTransferTime.Start();

                if (m_objTeliCamera.WaitFrameReady())
                {
                    if (m_objTeliCamera.GetFrame(0))
                    {
                        if (m_objTeliCamera.ConvertFrame(0))
                        {
                            if (m_smVisionInfo.g_blnViewColorImage)
                            {
                                m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                                m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, 0);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, 0);
                                m_smVisionInfo.g_arrImages[0].AddGain(m_smVisionInfo.g_arrImageGain[0]);
                            }

                            m_objTeliCamera.ReleaseImage(0);
                        }
                    }
                }
            }
            else
                SetGrabDone();

            //2021-10-21 ZJYEOH : Set Image to Black if camera fail
            if (!blnSuccess)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        m_smVisionInfo.g_arrColorImages[i].SetImageToBlack();
                        m_smVisionInfo.g_arrColorRotatedImages[i].SetImageToBlack();
                    }
                    m_smVisionInfo.g_arrImages[i].SetImageToBlack();
                    m_smVisionInfo.g_arrRotatedImages[i].SetImageToBlack();
                }
            }

            if (m_objTeliCamera.GetErrorMessage() != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objTeliCamera.GetErrorMessage();
                //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return true;
            }
        }

        public bool InitCamera(int intPort, String SerialNo, int intResolutionX, int intResolutionY, bool blnFirstTime)
        {
            //SRMMessageBox.Show("Before InitializaCamera2");
            bool blnInitSuccess = true;
            //if (!m_objAVTFireGrab.InitializeCamera(intPort))
            //    blnInitSuccess = false;

            //SRMMessageBox.Show("After InitializaCamera.2
            if (blnFirstTime)
            {

                if (m_smVisionInfo.g_strCameraModel == "AVT")
                {
                    if (!m_objAVTFireGrab.InitializeCamera(intPort, false))
                        blnInitSuccess = false;
                }
                else if (m_smVisionInfo.g_strCameraModel == "Teli")
                {
                    if (!m_objTeliCamera.InitializeCamera(SerialNo, intResolutionX, intResolutionY))
                    {
                        blnInitSuccess = false;
                        SRMMessageBox.Show("Serial No. " + SerialNo + " - " + m_smVisionInfo.g_strVisionDisplayName + " " + m_objTeliCamera.GetErrorMessage());
                    }
                }
            }
            RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = Key.OpenSubKey("SVG\\LightControl", true);
            string[] strLightControlMaskList = subKey.GetValueNames();

            string strCameraFilePath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\Camera.xml";
            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                strCameraFilePath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml";

            XmlParser fileHandle = new XmlParser(m_smProductionInfo.g_strRecipePath + "Camera.xml");
            XmlParser objFileHandle = new XmlParser(strCameraFilePath);
            fileHandle.GetFirstSection(m_smVisionInfo.g_strVisionName);
            objFileHandle.GetFirstSection(m_smVisionInfo.g_strVisionFolderName);

            m_smVisionInfo.g_fCameraShuttle = objFileHandle.GetValueAsFloat("Shutter", 200f);
            if (m_smVisionInfo.g_strCameraModel == "AVT")
            {
                m_objAVTFireGrab.SetCameraParameter(1, Convert.ToUInt32(m_smVisionInfo.g_fCameraShuttle));
                m_objAVTFireGrab.SetCameraParameter(4, objFileHandle.GetValueAsUInt("Gamma", 0));
                if (m_smVisionInfo.g_blnViewColorImage)
                {
                    m_objAVTFireGrab.SetCameraParameter(5, objFileHandle.GetValueAsUInt("UBValue", 0));
                    m_objAVTFireGrab.SetCameraParameter(6, objFileHandle.GetValueAsUInt("VRValue", 0));
                }
            }
            else if (m_smVisionInfo.g_strCameraModel == "Teli")
            {

            }
            m_smVisionInfo.g_intCameraGrabDelay = objFileHandle.GetValueAsInt("GrabDelay", 5);

            m_smVisionInfo.g_arrLightSource.Clear();

            string[] arrName = subKey.GetSubKeyNames();          // Get Related CommPort List
            for (int x = 0; x < arrName.Length; x++)
            {
                RegistryKey child = subKey.OpenSubKey(arrName[x], true);
                RegistryKey grandChild = child.CreateSubKey(m_smVisionInfo.g_strVisionFolderName);

                string[] arrType = grandChild.GetValueNames();
                for (int i = 0; i < arrType.Length; i++)
                {
                    LightSource objLightSource = new LightSource();
                    objLightSource.ref_strCommPort = arrName[x];
                    objLightSource.ref_strType = arrType[i];
                    objLightSource.ref_intChannel = Convert.ToInt32(grandChild.GetValue(arrType[i], 1));

                    string strSearch = arrType[i].Replace(" ", string.Empty);
                    bool blnLightMaskingFound = false;
                    for (int y = 0; y < strLightControlMaskList.Length; y++)
                    {
                        if (strLightControlMaskList[y].Contains(m_smVisionInfo.g_strVisionName + " - " + arrType[i]))
                            blnLightMaskingFound = true;
                    }
                    if (blnLightMaskingFound)
                        objLightSource.ref_intSeqNo = Convert.ToInt32(subKey.GetValue(m_smVisionInfo.g_strVisionName + " - " + arrType[i], 1));
                    else
                    {
                        objLightSource.ref_intSeqNo = fileHandle.GetValueAsInt(strSearch, 1);
                        subKey.SetValue(m_smVisionInfo.g_strVisionName + " - " + arrType[i], objLightSource.ref_intSeqNo);
                    }
                    objLightSource.ref_intValue = objFileHandle.GetValueAsInt(arrType[i], 31);
                    objLightSource.ref_intPortNo = x;
                    int intCameraOutNo = Convert.ToInt32(objLightSource.ref_strType.Substring(objLightSource.ref_strType.Length - 1));

                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                    {
                        if (intCameraOutNo == 0)
                            m_objAVTFireGrab.OutPort(intCameraOutNo, m_smVisionInfo.g_intTriggerMode);
                        else
                            m_objAVTFireGrab.OutPort(intCameraOutNo, 0);
                    }
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                    {
                        if (intCameraOutNo == 0)
                            m_objTeliCamera.OutPort(intCameraOutNo, m_smVisionInfo.g_intTriggerMode);
                        else
                            m_objTeliCamera.OutPort(intCameraOutNo, 0);
                    }

                    objLightSource.ref_arrValue = new List<int>();
                    objLightSource.ref_arrImageNo = new List<int>();
                    // Maximum grab 7 times
                    for (int j = 0; j < 7; j++)
                    {
                        if ((objLightSource.ref_intSeqNo & (0x01 << j)) > 0)
                        {
                            if (m_intGrabRequire < (j + 1))
                            {
                                m_intGrabRequire = j + 1; // Get highest grab number
                                for (int k = m_smVisionInfo.g_arrImages.Count; k < m_intGrabRequire; k++)
                                {
                                    if (m_smVisionInfo.g_blnViewColorImage)
                                    {
                                        m_smVisionInfo.g_arrColorImages.Add(new CImageDrawing());
                                        m_smVisionInfo.g_arrColorRotatedImages.Add(new CImageDrawing());
                                    }

                                    m_smVisionInfo.g_arrImages.Add(new ImageDrawing());
                                    m_smVisionInfo.g_arrRotatedImages.Add(new ImageDrawing());

                                    m_smVisionInfo.g_arrCameraShuttle.Add(new float());
                                    m_smVisionInfo.g_arrCameraGain.Add(new int());
                                    m_smVisionInfo.g_arrImageGain.Add(new float());
                                }
                            }
                            int intCount = objLightSource.ref_arrValue.Count;

                            objLightSource.ref_arrValue.Add(new int());
                            objLightSource.ref_arrImageNo.Add(new int());
                            objFileHandle.GetSecondSection(objLightSource.ref_strType);
                            objLightSource.ref_arrValue[intCount] = objFileHandle.GetValueAsInt("Seq" + intCount.ToString(), 31, 2);
                            objLightSource.ref_arrImageNo[intCount] = j;
                        }
                    }

                    m_smVisionInfo.g_arrLightSource.Add(objLightSource);

                    if (m_smCustomizeInfo.g_blnLEDiControl)
                    {
                        LEDi_Control.SetIntensity(x, objLightSource.ref_intChannel, Convert.ToByte(objLightSource.ref_arrValue[0]));
                    }
                    else if (m_smCustomizeInfo.g_blnVTControl)
                    {
                        VT_Control.SetConfigMode(objLightSource.ref_intPortNo);
                        VT_Control.SetIntensity(x, objLightSource.ref_intChannel, objLightSource.ref_arrValue[0]);
                        VT_Control.SetRunMode(objLightSource.ref_intPortNo);
                    }
                    else
                    {
                        TCOSIO_Control.SetIntensity(x, objLightSource.ref_intChannel, objLightSource.ref_arrValue[0]);
                        TCOSIO_Control.SendMessage(x, "@ST" + objLightSource.ref_intChannel + "1*");    // Set Strobe ON
                        TCOSIO_Control.SendMessage(x, "@SI" + objLightSource.ref_intChannel + "00*");   // Set Constant Intensity to 0
                    }

                    Thread.Sleep(5); // Delay after set intensity to light source controller
                }
            }
            if (m_smCustomizeInfo.g_blnVTControl)
            {
                //channel grouping
                int intChannelNum = 0;
                uint uintGroupNum = 0;
                for (int k = 0; k < m_smVisionInfo.g_arrLightSource.Count; k++)
                {
                    intChannelNum = m_smVisionInfo.g_arrLightSource[k].ref_intChannel;
                    //intChannelNum -= 1;
                    uintGroupNum += Convert.ToUInt32(Math.Pow(2, intChannelNum));
                }
                VT_Control.SetConfigMode(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);
                VT_Control.SetGroupsAvailable(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, uintGroupNum);
                //Setting active flag
                for (int m = 0; m < m_intGrabRequire; m++)
                {
                    VT_Control.SetActiveOutFlag(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, uintGroupNum, m, 1);
                }
                VT_Control.SetRunMode(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
            {
                m_smVisionInfo.g_arrCameraShuttle[i] = objFileHandle.GetValueAsFloat("Shutter" + i.ToString(), 0f);

                if (m_smVisionInfo.g_arrCameraShuttle[i] == 0)
                {
                    m_smVisionInfo.g_arrCameraShuttle[i] = m_smVisionInfo.g_fCameraShuttle;
                }

                if (i == 0)
                {
                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                        m_objAVTFireGrab.SetCameraParameter(1, (uint)m_smVisionInfo.g_arrCameraShuttle[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
            {
                m_smVisionInfo.g_arrCameraGain[i] = (uint)objFileHandle.GetValueAsInt("Gain" + i.ToString(), 1);

                if (i == 0)
                {
                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                        m_objAVTFireGrab.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                        m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                m_smVisionInfo.g_arrImageGain[i] = objFileHandle.GetValueAsFloat("ImageGain" + i.ToString(), 1);
            }

            return blnInitSuccess;
        }

        /// <summary>
        /// Returns whether the worker thread has stopped.
        /// </summary>
        public bool IsThreadStopped
        {
            get
            {
                lock (m_objStopLock)
                {
                    return m_blnStopped;
                }
            }
        }
        /// <summary>
        /// Tells the thread to stop, typically after completing its 
        /// current work item.
        /// </summary>
        public void StopThread()
        {
            lock (m_objStopLock)
            {
                m_blnStopping = true;
            }
        }

        public void PauseThread()
        {
            m_blnPause = true;
        }

        public void StartThread()
        {
            m_blnPause = false;
        }

        private void AttachImageToROI()
        {
            m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
            m_smVisionInfo.g_objCalibrateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

            if ((m_smCustomizeInfo.g_intWantPositioning & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (m_smVisionInfo.g_arrPositioningROIs.Count > 0)
                    AttachToROI(m_smVisionInfo.g_arrPositioningROIs, m_smVisionInfo.g_arrImages[0]);
            }

            m_smVisionInfo.g_objPositioning.ref_objCrosshair.AttachImage(m_smVisionInfo.g_arrImages[0]);

            if (m_smVisionInfo.g_intMachineStatus != 2)
            {
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            m_smVisionInfo.VS_VM_UpdateSmallPictureBox = true;
        }

        private void AttachToROI(List<ROI> arrROI, ImageDrawing objImage)
        {
            for (int i = 0; i < arrROI.Count; i++)
            {
                if (arrROI[i].ref_intType == 1)
                    arrROI[i].AttachImage(objImage);
            }
        }

        private void CheckLowYield()
        {
            if (!m_smVisionInfo.g_blnStopLowYield)//m_smCustomizeInfo.g_blnStopLowYield)
                return;

            if (m_smVisionInfo.g_intTestedTotal != 0)
            {
                float fYield = m_smVisionInfo.g_intPassTotal / (float)m_smVisionInfo.g_intTestedTotal * 100;
                if ((fYield <= m_smVisionInfo.g_fLowYield) && (m_smVisionInfo.g_intLowYieldUnitCount >= m_smVisionInfo.g_intMinUnitCheck)) //m_smCustomizeInfo.g_fLowYield , m_smCustomizeInfo.g_intMinUnitCheck
                {
                    m_smProductionInfo.PR_AT_StopProduction = true;
                    m_smVisionInfo.g_intMachineStatus = 1;
                    m_smVisionInfo.g_intLowYieldUnitCount = 0;
                    m_smVisionInfo.g_strErrorMessage += "Low Yield Fail!";
                }
            }
        }
        private void CheckContinuousPass()
        {
            if (!m_smVisionInfo.g_blnStopContinuousPass)
                return;

            if (m_smVisionInfo.g_intTestedTotal != 0)
            {
                if (m_smVisionInfo.g_intContinuousPassUnitCount >= m_smVisionInfo.g_intMinPassUnit)
                {
                    m_smProductionInfo.PR_AT_StopProduction = true;
                    m_smVisionInfo.g_intMachineStatus = 1;
                    m_smVisionInfo.g_intContinuousPassUnitCount = 0;
                    m_smVisionInfo.g_strErrorMessage += "*Continuous Pass " + m_smVisionInfo.g_intMinPassUnit.ToString() + " Unit(s)!";
                }
            }
        }

        private void CheckContinuousFail()
        {
            if (!m_smVisionInfo.g_blnStopContinuousFail)
                return;

            if (m_smVisionInfo.g_intTestedTotal != 0)
            {
                if (m_smVisionInfo.g_intContinuousFailUnitCount >= m_smVisionInfo.g_intMinFailUnit)
                {
                    m_smProductionInfo.PR_AT_StopProduction = true;
                    m_smVisionInfo.g_intMachineStatus = 1;
                    m_smVisionInfo.g_intContinuousFailUnitCount = 0;
                    m_smVisionInfo.g_strErrorMessage += "*Continuous Fail " + m_smVisionInfo.g_intMinFailUnit.ToString() + " Unit(s)!";
                }
            }
        }
        private void StartTest_CheckPresent(bool blnAuto)
        {
            m_smVisionInfo.g_blnViewUnitPresentObjectsBuilded = false;
            bool blnTemplateLearnt = true;
            if (m_smVisionInfo.g_arrPositioningROIs.Count == 0)          // Check whether template is learnt
            {
                m_smVisionInfo.g_strErrorMessage += "*Check Present : No Template Found";
                if (blnAuto)
                    m_smVisionInfo.g_intNoTemplateFailureTotal++;
                blnTemplateLearnt = false;
            }

            bool blnResultOK = true;

            if (!blnTemplateLearnt)
                blnResultOK = false;

            if (blnAuto)
            {
                Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

                if (!m_smProductionInfo.g_blnAllRunWithoutGrabImage)
                    if (!GrabImage())
                    {
                        return;
                    }
            }
            else if (m_smVisionInfo.MN_PR_GrabImage)
            {
                m_smVisionInfo.MN_PR_GrabImage = false;
                if (!GrabImage())
                    return;
            }
            else
                AttachImageToROI();

            m_smVisionInfo.g_objProcessTime.Start();


            if (blnAuto)
            {
                // Reset Result Indicator
                if (m_objVisionIO.ResultBit1 != null)
                    m_objVisionIO.ResultBit1.SetOff("V2 ");
                if (m_objVisionIO.ResultBit2 != null)
                    m_objVisionIO.ResultBit2.SetOff("V2 ");
                if (m_objVisionIO.ResultBit3 != null)
                    m_objVisionIO.ResultBit3.SetOff("V2 ");
                if (m_objVisionIO.ResultBit4 != null)
                    m_objVisionIO.ResultBit4.SetOff("V2 ");
                if (m_objVisionIO.ResultBit5 != null)
                    m_objVisionIO.ResultBit5.SetOff("V2 ");
                if (m_objVisionIO.ResultBit6 != null)
                    m_objVisionIO.ResultBit6.SetOff("V2 ");
            }

            m_smVisionInfo.g_objUnitPresent.ResetInspectionResult();

            if (blnTemplateLearnt)
                m_smVisionInfo.g_arrPositioningROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);

            if (m_smVisionInfo.g_objUnitPresent.ref_intDefineUnitMethod == 0)
            {
                if (blnTemplateLearnt)
                {
                    if (!m_smVisionInfo.g_objUnitPresent.DoInspection_UnitROI2(m_smVisionInfo.g_arrPositioningROIs[0]))
                    {
                        blnResultOK = false;
                        m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objUnitPresent.ref_strErrorMessage;
                    }
                    m_smVisionInfo.g_blnViewUnitPresentObjectsBuilded = true;
                }
            }
            else
            {
                if (blnTemplateLearnt)
                {
                    if (!m_smVisionInfo.g_objUnitPresent.DoInspection_BlobObjects(m_smVisionInfo.g_arrPositioningROIs[0]))
                    {
                        blnResultOK = false;
                        m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objUnitPresent.ref_strErrorMessage;
                    }
                }
            }

            if (blnAuto)
            {
                if (m_smVisionInfo.g_objUnitPresent.ref_intDefineUnitMethod == 0)
                {
                    // Set indicator
                    if ((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x01)) > 0)
                    {
                        if (m_objVisionIO.ResultBit1 != null)
                            m_objVisionIO.ResultBit1.SetOn("V2 ");
                    }

                    if ((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x02)) > 0)
                    {
                        if (m_objVisionIO.ResultBit2 != null)
                            m_objVisionIO.ResultBit2.SetOn("V2 ");
                    }

                    if ((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x04)) > 0)
                    {
                        if (m_objVisionIO.ResultBit3 != null)
                            m_objVisionIO.ResultBit3.SetOn("V2 ");
                    }

                    if ((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x08)) > 0)
                    {
                        if (m_objVisionIO.ResultBit4 != null)
                            m_objVisionIO.ResultBit4.SetOn("V2 ");
                    }

                    if ((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x10)) > 0)
                    {
                        if (m_objVisionIO.ResultBit5 != null)
                            m_objVisionIO.ResultBit5.SetOn("V2 ");
                    }

                    if ((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x20)) > 0)
                    {
                        if (m_objVisionIO.ResultBit6 != null)
                            m_objVisionIO.ResultBit6.SetOn("V2 ");
                    }
                }
                else
                {
                    // Set indicator
                    if (((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x01)) > 0) ||
                        ((m_smVisionInfo.g_objUnitPresent.ref_intUnitOffSetFailMask & (0x01)) > 0))
                    {
                        if (m_objVisionIO.ResultBit1 != null)
                            m_objVisionIO.ResultBit1.SetOn("V2 ");
                    }

                    if (((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x02)) > 0) ||
                        ((m_smVisionInfo.g_objUnitPresent.ref_intUnitOffSetFailMask & (0x02)) > 0))
                    {
                        if (m_objVisionIO.ResultBit2 != null)
                            m_objVisionIO.ResultBit2.SetOn("V2 ");
                    }

                    if (((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x04)) > 0) ||
                        ((m_smVisionInfo.g_objUnitPresent.ref_intUnitOffSetFailMask & (0x04)) > 0))
                    {
                        if (m_objVisionIO.ResultBit3 != null)
                            m_objVisionIO.ResultBit3.SetOn("V2 ");
                    }

                    if (((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x08)) > 0) ||
                        ((m_smVisionInfo.g_objUnitPresent.ref_intUnitOffSetFailMask & (0x08)) > 0))
                    {
                        if (m_objVisionIO.ResultBit4 != null)
                            m_objVisionIO.ResultBit4.SetOn("V2 ");
                    }

                    if (((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x10)) > 0) ||
                        ((m_smVisionInfo.g_objUnitPresent.ref_intUnitOffSetFailMask & (0x10)) > 0))
                    {
                        if (m_objVisionIO.ResultBit5 != null)
                            m_objVisionIO.ResultBit5.SetOn("V2 ");
                    }

                    if (((m_smVisionInfo.g_objUnitPresent.ref_intUnitPresentFailMask & (0x20)) > 0) ||
                        ((m_smVisionInfo.g_objUnitPresent.ref_intUnitOffSetFailMask & (0x20)) > 0))
                    {
                        if (m_objVisionIO.ResultBit6 != null)
                            m_objVisionIO.ResultBit6.SetOn("V2 ");
                    }
                }
            }

            if (blnResultOK)
            {
                m_smVisionInfo.g_strResult = "Pass";

                if (blnAuto)
                {
                    m_smVisionInfo.g_intPassTotal++;
                    m_smVisionInfo.g_intContinuousPassUnitCount++;
                    SavePassImage();
                }
                else
                {
                    m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                    m_smVisionInfo.g_strErrorMessage = "Offline Test Pass!";
                }
            }
            else
            {
                m_smVisionInfo.g_strResult = "Fail";

                if (blnAuto)
                {
                    if (blnTemplateLearnt)
                        m_smVisionInfo.g_intPositionFailureTotal++;
                    m_smVisionInfo.g_intContinuousFailUnitCount++;
                    SaveRejectImage("Fail", m_smVisionInfo.g_strErrorMessage);
                }
            }

            if (blnAuto)
            {
                m_smVisionInfo.g_intTestedTotal++;
                m_smVisionInfo.g_intLowYieldUnitCount++;
                CheckLowYield();
                CheckContinuousPass();
                CheckContinuousFail();
            }

            m_smVisionInfo.g_objProcessTime.Stop();

            m_smVisionInfo.VS_AT_UpdateQuantity = true;
            m_smVisionInfo.PR_VM_UpdateQuantity = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;


        }



        private void StartTest(bool blnAuto)
        {
            bool blnResultOK = true;

            if (blnAuto)
            {
                Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

                if (!m_smProductionInfo.g_blnAllRunWithoutGrabImage)
                    if (!GrabImage())
                    {
                        return;
                    }
            }
            else if (m_smVisionInfo.MN_PR_GrabImage)
            {
                m_smVisionInfo.MN_PR_GrabImage = false;
                if (!GrabImage())
                    return;
            }
            else
                AttachImageToROI();

            m_smVisionInfo.g_objProcessTime.Start();

            if (blnAuto)
            {
                // Reset orientation IO
                if (m_objVisionIO.OrientResult1 != null)
                    m_objVisionIO.OrientResult1.SetOff("V2 ");
                if (m_objVisionIO.OrientResult2 != null)
                    m_objVisionIO.OrientResult2.SetOff("V2 ");
            }

            bool blnCheckDiffOrient;
            bool blnIncludeLimitInspection;
            bool blnCheckDoubleUnit;
            bool blnCheckUnitFlip;

            if (m_smVisionInfo.g_strVisionName == "BottomPosition")
            {
                blnCheckDiffOrient = true;
                blnIncludeLimitInspection = true;
                if (m_smVisionInfo.g_objPositioning.ref_intPRSMode == 0) // Noise Surface
                    blnCheckDoubleUnit = true;
                else
                    blnCheckDoubleUnit = false;
                blnCheckUnitFlip = true;
            }
            else if (m_smVisionInfo.g_strVisionName == "BottomPositionOrient")
            {
                blnCheckDiffOrient = true;
                blnIncludeLimitInspection = true;
                blnCheckDoubleUnit = false;
                blnCheckUnitFlip = false;

            }
            else // For tape pocket position
            {
                blnCheckDiffOrient = false;
                blnIncludeLimitInspection = true;
                blnCheckDoubleUnit = false;
                blnCheckUnitFlip = true;
            }

            if (m_smVisionInfo.g_strVisionName == "BottomPositionOrient")
            {

                if (!m_smVisionInfo.g_objPositioning.DoInspection_BottomPositionOrient(m_smVisionInfo.g_arrImages[0],
                                                              m_smVisionInfo.g_arrPositioningROIs,
                                                              m_smVisionInfo.g_arrPositioningGauges,
                                                              m_smVisionInfo.g_fCalibPixelXInUM,
                                                              m_smVisionInfo.g_fCalibPixelYInUM,
                                                              false,    // Use blob to find pre location
                                                              blnIncludeLimitInspection,
                                                              blnCheckDiffOrient,
                                                              blnCheckDoubleUnit,
                                                              true,
                                                              false,     // Check unit flip
                                                              true))    // Want Test Orientation
                {
                    blnResultOK = false;
                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPositioning.ref_strErrorMessage;
                }
            }
            else
            {
                if (!m_smVisionInfo.g_objPositioning.DoInspection_BottomPosition(m_smVisionInfo.g_arrImages[0],
                                                              m_smVisionInfo.g_arrPositioningROIs,
                                                              m_smVisionInfo.g_arrPositioningGauges,
                                                              m_smVisionInfo.g_fCalibPixelXInUM,
                                                              m_smVisionInfo.g_fCalibPixelYInUM, false,
                                                              blnIncludeLimitInspection, blnCheckDiffOrient, blnCheckDoubleUnit, true, true))
                {
                    blnResultOK = false;
                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objPositioning.ref_strErrorMessage;
                }
            }


            if (blnResultOK)
            {
                if (m_smVisionInfo.g_objPositioning.ref_intHighestScoreDirection > 0 && m_objVisionIO.OrientResult1 != null && m_objVisionIO.OrientResult2 != null)
                {
                    switch (m_smVisionInfo.g_objPositioning.ref_intHighestScoreDirection)
                    {
                        case 1:
                            if (m_smCustomizeInfo.g_intOrientIO == 0)
                            {
                                m_objVisionIO.OrientResult1.SetOff("V2 ");
                                m_objVisionIO.OrientResult2.SetOn("V2 ");
                            }
                            else
                            {
                                m_objVisionIO.OrientResult1.SetOn("V2 ");
                                m_objVisionIO.OrientResult2.SetOff("V2 ");
                            }
                            break;
                        case 2:
                            m_objVisionIO.OrientResult1.SetOn("V2 ");
                            m_objVisionIO.OrientResult2.SetOn("V2 ");
                            break;
                        case 3:
                            if (m_smCustomizeInfo.g_intOrientIO == 0)
                            {
                                m_objVisionIO.OrientResult1.SetOn("V2 ");
                                m_objVisionIO.OrientResult2.SetOff("V2 ");
                            }
                            else
                            {
                                m_objVisionIO.OrientResult1.SetOff("V2 ");
                                m_objVisionIO.OrientResult2.SetOn("V2 ");
                            }
                            break;
                    }
                }

                if (m_objVisionIO.IOPass1 != null)
                    m_objVisionIO.IOPass1.SetOn("V2 ");

                if (m_objVisionIO.UnitPresent != null)
                    m_objVisionIO.UnitPresent.SetOn("V2 ");

                m_smVisionInfo.g_strResult = "Pass";
                if (blnAuto)
                {
                    m_smVisionInfo.g_intPassTotal++;
                    m_smVisionInfo.g_intContinuousPassUnitCount++;
                    SavePassImage();
                }
                else
                {
                    m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                    m_smVisionInfo.g_strErrorMessage = "Offline Test Pass!";
                }
            }
            else
            {
                if (m_objVisionIO.IOPass1 != null)
                    m_objVisionIO.IOPass1.SetOff("V2 ");

                if (m_objVisionIO.UnitPresent != null)
                {
                    if (m_smVisionInfo.g_objPositioning.ref_bEmptyUnit)
                        m_objVisionIO.UnitPresent.SetOff("V2 ");
                    else
                        m_objVisionIO.UnitPresent.SetOn("V2 ");
                }

                m_smVisionInfo.g_strResult = "Fail";

                if (blnAuto)
                {
                    if (!m_smVisionInfo.g_objPositioning.ref_bEmptyUnit)
                    {
                        m_smVisionInfo.g_intPositionFailureTotal++;
                        m_smVisionInfo.g_intContinuousFailUnitCount++;
                    }
                    if (m_smVisionInfo.g_objPositioning.ref_bEmptyUnit)
                        SaveRejectImage("EmptyUnit", m_smVisionInfo.g_strErrorMessage);
                    else
                        SaveRejectImage("Fail", m_smVisionInfo.g_strErrorMessage);
                }
            }

#if(RTXDebug || RTXRelease)
            if (m_smVisionInfo.g_strVisionName == "TapePocketPosition")
            {
                string strMessage = "<T";

                if (!blnResultOK)
                    strMessage += "FN0000N0000";
                else
                {
                    strMessage += "P";
                    //strMessage += "P0000P0000";
                    if (m_smVisionInfo.g_objPositioning.ref_fObjectOffsetX < 0)
                        strMessage += "N";
                    else
                        strMessage += "P";
                    strMessage += string.Format("{0:0000}", Math.Abs(Math.Round(m_smVisionInfo.g_objPositioning.ref_fObjectOffsetX)));

                    float fObjectOffsetY = m_smVisionInfo.g_objPositioning.ref_fObjectOffsetY;

                    if ((fObjectOffsetY) < 0)
                        strMessage += "N";
                    else
                        strMessage += "P";
                    strMessage += string.Format("{0:0000}", Math.Abs(Math.Round(fObjectOffsetY)));
                }

                strMessage += ">";
                //m_thCOMMPort.SendData(strMessage);

                RS232.RTXSendData(strMessage, m_smVisionInfo.g_intVisionPos);
            }
#endif
            //TrackLog objTL = new TrackLog(AppDomain.CurrentDomain.BaseDirectory + "\\TrackLog\\" +  // Start 0603 Debug (Requested by CS Ang)
            //            m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
            //            "\\" + m_smVisionInfo.g_strVisionFolderName + "\\");
            //objTL.WriteLine("X= " + m_smVisionInfo.g_objPositioning.ref_fObjectOffsetX.ToString() +
            //                "   Y= " + m_smVisionInfo.g_objPositioning.ref_fObjectOffsetY.ToString() +
            //                "   T= " + m_smVisionInfo.g_objPositioning.ref_fObjectAngle.ToString()); // End 0603 Debug

            if (blnAuto)
            {
                if (!m_smVisionInfo.g_objPositioning.ref_bEmptyUnit)
                {
                    m_smVisionInfo.g_intTestedTotal++;
                    m_smVisionInfo.g_intLowYieldUnitCount++;
                    CheckLowYield();
                    CheckContinuousPass();
                    CheckContinuousFail();
                }
            }
            m_smVisionInfo.g_objProcessTime.Stop();

            m_smVisionInfo.VS_AT_UpdateQuantity = true;
            m_smVisionInfo.PR_VM_UpdateQuantity = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            return;
        }

        /// <summary>
        /// Save different category of reject image
        /// </summary>
        /// <param name="strModule">Reject Image category name</param>
        private void SaveRejectImage(string strModule, string strRejectMessage)
        {
            if (m_smCustomizeInfo.g_blnSaveFailImage)
            {
                //string strPath = m_smVisionInfo.g_strSaveImageLocation +
                //     m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                //     "\\" + m_smVisionInfo.g_strVisionFolderName +
                //     "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" +
                //     "\\" + strModule + "\\";

                // 2020 03 27 - JBTAN: Save to different folder if reset count
                string strPath;
                if (m_smVisionInfo.g_intVisionResetCount == 0)
                {
                    strPath = m_smVisionInfo.g_strSaveImageLocation +
                        m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                        "\\" + m_smVisionInfo.g_strVisionFolderName +
                        "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime +
                        "\\" + strModule + "\\";
                }
                else
                {
                    string strLotSaveImagePath = m_smVisionInfo.g_strSaveImageLocation + m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;
                    string strVisionImageFolderName = m_smVisionInfo.g_strVisionFolderName + "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smVisionInfo.g_strVisionResetCountTime;

                    if (!Directory.Exists(strLotSaveImagePath + "\\" + strVisionImageFolderName))
                        Directory.CreateDirectory(strLotSaveImagePath + "\\" + strVisionImageFolderName);
                    if (!Directory.Exists(strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\" + strModule + "\\"))
                        Directory.CreateDirectory(strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\" + strModule + "\\");

                    strPath = strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\" + strModule + "\\";
                }

                if (m_smCustomizeInfo.g_intSaveImageMode == 0)
                {
                    if (m_smVisionInfo.g_intFailImageCount > m_smCustomizeInfo.g_intFailImagePics)
                        return;
                }
                else
                {
                    if (m_smVisionInfo.g_intFailImageCount >= m_smCustomizeInfo.g_intFailImagePics)
                        m_smVisionInfo.g_intFailImageCount = 0;
                }

                if (m_smVisionInfo.g_blnViewColorImage)
                    m_smVisionInfo.g_arrColorImages[0].SaveImage(strPath + m_smVisionInfo.g_intFailImageCount + ".bmp");
                else
                    m_smVisionInfo.g_arrImages[0].SaveImage(strPath + m_smVisionInfo.g_intFailImageCount + ".bmp");

                for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[i].SaveImage(strPath + m_smVisionInfo.g_intFailImageCount + "_Image" + i.ToString() + ".bmp");
                    else
                        m_smVisionInfo.g_arrImages[i].SaveImage(strPath + m_smVisionInfo.g_intFailImageCount + "_Image" + i.ToString() + ".bmp");
                }

                if (m_smCustomizeInfo.g_blnSaveFailImageErrorMessage)
                {
                    XmlParser objFile = new XmlParser(strPath + m_smVisionInfo.g_intFailImageCount + ".xml");
                    objFile.WriteSectionElement("ErrorMessage");
                    objFile.WriteElement1Value("Message_" + m_smVisionInfo.g_intFailImageCount, strRejectMessage);
                    objFile.WriteEndElement();
                }

                m_smVisionInfo.g_strLastImageFolder = strPath;
                m_smVisionInfo.g_strLastImageName = m_smVisionInfo.g_intFailImageCount + ".bmp";

                m_smVisionInfo.g_intFailImageCount++;
            }
        }

        private void SavePassImage()
        {
            if (m_smCustomizeInfo.g_blnSavePassImage)
            {
                if (m_smVisionInfo.g_intPassImageCount < m_smCustomizeInfo.g_intPassImagePics)
                {
                    //string strPath = m_smVisionInfo.g_strSaveImageLocation +
                    //    m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                    //    "\\" + m_smVisionInfo.g_strVisionFolderName +
                    //    "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" +
                    //    "\\Pass\\";

                    // 2020 03 27 - JBTAN: Save to different folder if reset count
                    string strPath;
                    if (m_smVisionInfo.g_intVisionResetCount == 0)
                    {
                        strPath = m_smVisionInfo.g_strSaveImageLocation +
                                m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                                "\\" + m_smVisionInfo.g_strVisionFolderName +
                                "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime +
                                "\\Pass\\";
                    }
                    else
                    {
                        string strLotSaveImagePath = m_smVisionInfo.g_strSaveImageLocation + m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;
                        string strVisionImageFolderName = m_smVisionInfo.g_strVisionFolderName + "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smVisionInfo.g_strVisionResetCountTime;

                        if (!Directory.Exists(strLotSaveImagePath + "\\" + strVisionImageFolderName))
                            Directory.CreateDirectory(strLotSaveImagePath + "\\" + strVisionImageFolderName);
                        if (!Directory.Exists(strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\Pass"))
                            Directory.CreateDirectory(strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\Pass");

                        strPath = strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\Pass\\";
                    }

                    m_smVisionInfo.g_arrImages[0].SaveImage(strPath + m_smVisionInfo.g_intPassImageCount + ".bmp");

                    m_smVisionInfo.g_strLastImageFolder = strPath;
                    m_smVisionInfo.g_strLastImageName = m_smVisionInfo.g_intPassImageCount + ".bmp";

                    m_smVisionInfo.g_intPassImageCount++;
                }
            }
        }

        private void SetGrabDone()
        {
            m_smVisionInfo.g_objGrabTime.Stop();
            m_objVisionIO.IOGrabbing.SetOff("V2 IOGrabbing 1");
        }

        /// <summary>
        /// Called by the thread to indicate when it has stopped.
        /// </summary>
        private void SetStopped()
        {
            lock (m_objStopLock)
            {
                m_blnStopped = true;
            }
        }





        private void TakeAction(string strMessage)
        {
            if (m_smVisionInfo.g_intMachineStatus == 2 && m_smVisionInfo.AT_PR_StartLiveImage && strMessage == "Client disconnected.")
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            if (!(strMessage.StartsWith("<") && strMessage.EndsWith(">")))
                return;

            try
            {
                string[] strString = strMessage.Split(',', '>');
                switch (strString[0])
                {
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }



        private void UpdateProgress()
        {
            try
            {
                while (!m_blnStopping)
                {
                    if (!m_blnPause)
                    {

                        if (m_smVisionInfo.MN_PR_GrabImage || m_smVisionInfo.AT_PR_GrabImage)
                        {
                            if (m_smVisionInfo.AT_PR_GrabImage) // 01-08-2019 ZJYEOH : Only clear drawing result when user pressed grab button, solved "grab before test" no drawings 
                                m_smVisionInfo.g_blnClearResult = true;

                            GrabImage();
                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            m_smVisionInfo.MN_PR_GrabImage = false;
                            m_smVisionInfo.AT_PR_GrabImage = false;
                        }

                        if (m_smVisionInfo.AT_PR_StartLiveImage && !m_smVisionInfo.AT_PR_PauseLiveImage)
                        {
                            m_smVisionInfo.g_blnGrabbing = true;
                            m_smVisionInfo.g_blnClearResult = true;

                            GrabImage();
                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            m_smVisionInfo.g_blnGrabbing = false;
                            System.Threading.Thread.Sleep(50);
                        }

                        if (m_smVisionInfo.AT_PR_AttachImagetoROI)
                        {
                            AttachImageToROI();
                            m_smVisionInfo.AT_PR_AttachImagetoROI = false;
                        }

                        if (m_smVisionInfo.MN_PR_StartTest && !m_smVisionInfo.AT_PR_AttachImagetoROI)
                        {
                            m_smVisionInfo.g_objTotalTime.Start();
                            m_smVisionInfo.MN_PR_StartTest = false;

                            if (m_smVisionInfo.g_strVisionName == "UnitPresent")
                                StartTest_CheckPresent(false);
                            else
                                StartTest(false);

                            m_smVisionInfo.g_objTotalTime.Stop();
                            m_smVisionInfo.VM_AT_BlockImageUpdate = false;
                            m_smVisionInfo.PR_MN_UpdateInfo = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }

                        if ((m_objVisionIO.IOStartVision.IsOn() && m_smVisionInfo.g_intMachineStatus == 2) ||
                            (m_smVisionInfo.g_blnDebugRUN && m_smVisionInfo.g_intMachineStatus == 2))
                        {
                            m_smVisionInfo.g_objTotalTime.Start();
                            m_objVisionIO.IOEndVision.SetOff("V2 ");
                            m_objVisionIO.IOGrabbing.SetOn("V2 IOGrabbing 2");


                            if (m_smVisionInfo.g_strVisionName == "UnitPresent")
                                StartTest_CheckPresent(true);
                            else
                                StartTest(true);

                            if (!m_blnForceStopProduction)
                            {
                                m_objVisionIO.IOEndVision.SetOn("V2 ");
                            }
                            else
                            {
                                STTrackLog.WriteLine("Vision2Process > Force Stop Production");
                                m_blnForceStopProduction = false;
                                m_smVisionInfo.g_intMachineStatus = 1;
                            }

                            m_smVisionInfo.g_objTotalTime.Stop();
                            m_smProductionInfo.VM_TH_UpdateCount = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;

                            if (m_smVisionInfo.g_blnDebugRUN)
                                Thread.Sleep(m_smVisionInfo.g_intSleep);
                            else
                            {
                                if (m_smVisionInfo.g_intSleep > 1)              // Change to > 1 to prevent calling this sleep function during production when not necessary.
                                    Thread.Sleep(m_smVisionInfo.g_intSleep);
                            }

                            if (m_smProductionInfo.g_blnAllRunFromCenter)
                                m_smVisionInfo.g_blnDebugRUN = false;
                        }

                        ////if (m_thCOMMPort.m_blnStartTest)
                        //if (m_thCOMMPort.m_blnStartTest && m_smVisionInfo.g_strVisionName == "TapePocketPosition")
                        //{
                        //    m_thCOMMPort.m_blnStartTest = false;

                        //    if (m_thCOMMPort.m_blnStartTest)
                        //    {
                        //    }
                        //    m_objVisionIO.IOEndVision.SetOff("V2 ");
                        //    m_objVisionIO.IOGrabbing.SetOn("V2 ");
                        //    StartTest(false);

                        //    m_objVisionIO.IOEndVision.SetOn("V2 ");

                        //    m_smProductionInfo.VM_TH_UpdateCount = true;
                        //    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        //}


                        if (m_objVisionIO.IOEndVision.IsOn() && m_smVisionInfo.g_intMachineStatus != 2)
                            m_objVisionIO.IOEndVision.SetOff("V2 ");
                        else if (m_objVisionIO.IOEndVision.IsOff() && m_smVisionInfo.g_intMachineStatus == 2)
                        {
                            m_objVisionIO.IOEndVision.SetOn("V2 ");
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Vision2Process->UpdateProgress() :" + ex.ToString());
                SRMMessageBox.Show("Vision2Process has been terminated. Please Exit SRMVision software and Run again!", "Vision2Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_smVisionInfo.g_strCameraModel == "AVT")
                    m_objAVTFireGrab.OFFCamera();
                else if (m_smVisionInfo.g_strCameraModel == "Teli")
                    m_objTeliCamera.OFFCamera();
                SetStopped();
                m_thThread = null;
            }
        }
    }
}
