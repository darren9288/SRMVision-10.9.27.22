using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Common;
using ImageAcquisition;
using IOMode;
using SharedMemory;
using VisionProcessing;
using System.Runtime.InteropServices;
using Lighting;
using Microsoft.Win32;
using System.Linq;

namespace VisionModule
{
    public class Vision6Process
    {

        #region constant variables

        //private const int BUFFERSIZE = 20;

        #endregion

        #region enum

        public enum TCPIPResulID
        {
            Fail = 0, /*FailMark = 1, FailPackage = 2,*/ FailPosition = 3//, FailNotSeatProper = 4, FailCrack = 11, FailForeignMaterial = 12,
            //FailPackageDimension = 13, FailVoid = 14, FailChippedOffOrScractches = 15, FailCopper = 16, FailDiscolouration = 17, FailEmpty = 18,
            //FailNoMark = 19, Fail2DCodeNoFound = 20, Fail2DCodeVerification = 21
        };
        #endregion

        #region Member Variables

        List<IntPtr> m_arrBufferPointer = new List<IntPtr>();

        // Thread handle
        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnPause = false;
        private bool m_blnForceStopProduction = false;
        private bool m_blnLoadRejectImageListPath = false;
        private bool m_blnDisplayOrientFail = false;
        private bool m_blnWantAddFailCount = false;          // Record which fail criteria fail counter is added during last fail inspection
        private bool m_blnLastCheckPresent = false;
        private bool m_blnPassCountAdded = false;
        private int m_intCounter = 0;
        private int m_intPassStartNode = 0;
        private int m_intFailStartNode = 0;
        private int m_intPassEndNode = 0;
        private int m_intFailEndNode = 0;
        private int m_intFileIndex = 0;

        private int m_intGrabRequire = 0;
        private uint m_intCameraGainPrev = 1;
        private float m_fCameraShuttlePrev = 1f;

        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;

        private AVTVimba m_objAVTFireGrab;
        private TeliCamera m_objTeliCamera;
        private VisionIO m_objVisionIO;
        private VisionComThread m_smComThread;
        private TCPIPIO m_smTCPIPIO;
        private RS232 m_thCOMMPort;
        private HiPerfTimer m_objStopMachineTiming = new HiPerfTimer();
        private bool m_blnStopped = false, m_blnStopped_SaveImage = false;
        private Thread m_thThread, m_thSubThread_SaveImage;
        private int m_intCameraOutState = 1;
        private int m_intRetryCount = 0;

        private ImageDrawing[] m_arrPassImage1Buffer = null;
        private ImageDrawing[] m_arrPassImage2Buffer = null;
        private ImageDrawing[] m_arrPassImage3Buffer = null;
        private ImageDrawing[] m_arrPassImage4Buffer = null;
        private ImageDrawing[] m_arrPassImage5Buffer = null;
        private ImageDrawing[] m_arrPassImage6Buffer = null;
        private ImageDrawing[] m_arrPassImage7Buffer = null;
        private ImageDrawing[] m_arrFailImage1Buffer = null;
        private ImageDrawing[] m_arrFailImage2Buffer = null;
        private ImageDrawing[] m_arrFailImage3Buffer = null;
        private ImageDrawing[] m_arrFailImage4Buffer = null;
        private ImageDrawing[] m_arrFailImage5Buffer = null;
        private ImageDrawing[] m_arrFailImage6Buffer = null;
        private ImageDrawing[] m_arrFailImage7Buffer = null;
        private CImageDrawing[] m_arrPassCImage1Buffer = null;
        private CImageDrawing[] m_arrPassCImage2Buffer = null;
        private CImageDrawing[] m_arrPassCImage3Buffer = null;
        private CImageDrawing[] m_arrPassCImage4Buffer = null;
        private CImageDrawing[] m_arrPassCImage5Buffer = null;
        private CImageDrawing[] m_arrPassCImage6Buffer = null;
        private CImageDrawing[] m_arrPassCImage7Buffer = null;
        private CImageDrawing[] m_arrFailCImage1Buffer = null;
        private CImageDrawing[] m_arrFailCImage2Buffer = null;
        private CImageDrawing[] m_arrFailCImage3Buffer = null;
        private CImageDrawing[] m_arrFailCImage4Buffer = null;
        private CImageDrawing[] m_arrFailCImage5Buffer = null;
        private CImageDrawing[] m_arrFailCImage6Buffer = null;
        private CImageDrawing[] m_arrFailCImage7Buffer = null;
        private int[] m_arrPassNoBuffer = null;
        private int[] m_arrFailNoBuffer = null;
        private int[] m_arrRetryCountBuffer = null;
        private string[] m_arrRejectNameBuffer = null;
        private string[] m_arrRejectMessageBuffer = null;
        private List<string> m_arrRejectImageListPath = new List<string>();
        private List<string> m_arrRejectImageErrorMessageListPath = new List<string>();
        //private float[] m_EmptyResult = new float[4];
        //private float[] m_MarkResult = new float[4];
        //private float[] m_OrientResult = new float[4];

        //TCPIPIO
        private int m_intTCPIPResultID = -1;
        private bool m_blnStartVision_In = false;
        private bool m_blnEndVision_Out = true;
        private bool m_blnGrabbing_Out = false;
        private bool m_blnPass1_Out = true;
        private bool m_blnOrientResult2_Out = false;
        private bool m_blnOrientResult1_Out = false;
        private bool m_blnCheckPresentQA_In = false;
        private bool m_blnResetQA_In = false;
        private bool m_blnCheckPresent_In = false;
        private bool m_blnRetest_In = false;

        #endregion

        string m_strResultTrack = "";

        public Vision6Process(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
           AVTVimba objAVTFireGrab, VisionComThread smComThread, RS232 thCOMMPort, TCPIPIO smTCPIPIO)
        {
            m_smCustomizeInfo = objCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_objAVTFireGrab = objAVTFireGrab;
            m_smVisionInfo = objVisionInfo;
            m_smComThread = smComThread;
            m_smTCPIPIO = smTCPIPIO;
            m_thCOMMPort = thCOMMPort;

            // 2019 09 12 - CCENG: Change to init this buffer during Init Camera.
            //if (m_smVisionInfo.g_blnViewColorImage)
            //{
            //    m_arrPassCImage1Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrPassCImage2Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrPassCImage3Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrPassCImage4Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrPassCImage5Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage1Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage2Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage3Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage4Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage5Buffer = new CImageDrawing[BUFFERSIZE];

            //    for (int i = 0; i < BUFFERSIZE; i++)
            //    {
            //        m_arrPassCImage1Buffer[i] = new CImageDrawing(true);
            //        m_arrPassCImage2Buffer[i] = new CImageDrawing(true);
            //        m_arrPassCImage3Buffer[i] = new CImageDrawing(true);
            //        m_arrPassCImage4Buffer[i] = new CImageDrawing(true);
            //        m_arrPassCImage5Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage1Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage2Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage3Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage4Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage5Buffer[i] = new CImageDrawing(true);
            //    }
            //}
            //else
            //{
            //    m_arrPassImage1Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrPassImage2Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrPassImage3Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrPassImage4Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrPassImage5Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage1Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage2Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage3Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage4Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage5Buffer = new ImageDrawing[BUFFERSIZE];

            //    for (int i = 0; i < BUFFERSIZE; i++)
            //    {
            //        m_arrPassImage1Buffer[i] = new ImageDrawing(true);
            //        m_arrPassImage2Buffer[i] = new ImageDrawing(true);
            //        m_arrPassImage3Buffer[i] = new ImageDrawing(true);
            //        m_arrPassImage4Buffer[i] = new ImageDrawing(true);
            //        m_arrPassImage5Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage1Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage2Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage3Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage4Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage5Buffer[i] = new ImageDrawing(true);
            //    }
            //}
            //m_arrPassNoBuffer = new int[BUFFERSIZE];
            //m_arrFailNoBuffer = new int[BUFFERSIZE];
            //m_arrRetryCountBuffer = new int[BUFFERSIZE];
            //m_arrRejectNameBuffer = new string[BUFFERSIZE];

            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                m_smTCPIPIO.ReceiveCommandEvent += new TCPIPIO.ReceiveCommandHandle(TakeAction_TCPIPIO);
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

            m_thSubThread_SaveImage = new Thread(new ThreadStart(UpdateSubProgress_SaveImage));
            m_thSubThread_SaveImage.IsBackground = true;
            m_thSubThread_SaveImage.Priority = ThreadPriority.Lowest;
            m_thSubThread_SaveImage.Start();
        }

        public Vision6Process(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
            TeliCamera objTeliCamera, VisionComThread smComThread, RS232 thCOMMPort, TCPIPIO smTCPIPIO)
        {
            m_smCustomizeInfo = objCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_objTeliCamera = objTeliCamera;
            m_smVisionInfo = objVisionInfo;
            m_smComThread = smComThread;
            m_smTCPIPIO = smTCPIPIO;
            m_thCOMMPort = thCOMMPort;

            // 2019 09 12 - CCENG: Change to init this buffer during Init Camera.
            //if (m_smVisionInfo.g_blnViewColorImage)
            //{
            //    m_arrPassCImage1Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrPassCImage2Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrPassCImage3Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrPassCImage4Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrPassCImage5Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage1Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage2Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage3Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage4Buffer = new CImageDrawing[BUFFERSIZE];
            //    m_arrFailCImage5Buffer = new CImageDrawing[BUFFERSIZE];

            //    for (int i = 0; i < BUFFERSIZE; i++)
            //    {
            //        m_arrPassCImage1Buffer[i] = new CImageDrawing(true);
            //        m_arrPassCImage2Buffer[i] = new CImageDrawing(true);
            //        m_arrPassCImage3Buffer[i] = new CImageDrawing(true);
            //        m_arrPassCImage4Buffer[i] = new CImageDrawing(true);
            //        m_arrPassCImage5Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage1Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage2Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage3Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage4Buffer[i] = new CImageDrawing(true);
            //        m_arrFailCImage5Buffer[i] = new CImageDrawing(true);
            //    }
            //}
            //else
            //{
            //    m_arrPassImage1Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrPassImage2Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrPassImage3Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrPassImage4Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrPassImage5Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage1Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage2Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage3Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage4Buffer = new ImageDrawing[BUFFERSIZE];
            //    m_arrFailImage5Buffer = new ImageDrawing[BUFFERSIZE];

            //    for (int i = 0; i < BUFFERSIZE; i++)
            //    {
            //        m_arrPassImage1Buffer[i] = new ImageDrawing(true);
            //        m_arrPassImage2Buffer[i] = new ImageDrawing(true);
            //        m_arrPassImage3Buffer[i] = new ImageDrawing(true);
            //        m_arrPassImage4Buffer[i] = new ImageDrawing(true);
            //        m_arrPassImage5Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage1Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage2Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage3Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage4Buffer[i] = new ImageDrawing(true);
            //        m_arrFailImage5Buffer[i] = new ImageDrawing(true);
            //    }
            //}
            //m_arrPassNoBuffer = new int[BUFFERSIZE];
            //m_arrFailNoBuffer = new int[BUFFERSIZE];
            //m_arrRetryCountBuffer = new int[BUFFERSIZE];
            //m_arrRejectNameBuffer = new string[BUFFERSIZE];

            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                m_smTCPIPIO.ReceiveCommandEvent += new TCPIPIO.ReceiveCommandHandle(TakeAction_TCPIPIO);
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

            m_thSubThread_SaveImage = new Thread(new ThreadStart(UpdateSubProgress_SaveImage));
            m_thSubThread_SaveImage.IsBackground = true;
            m_thSubThread_SaveImage.Priority = ThreadPriority.Lowest;
            m_thSubThread_SaveImage.Start();
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
        public bool GrabImage(bool blnForInspection)
        {
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

            if (m_smVisionInfo.g_intLightControllerType == 2)
            {
                //if (m_smVisionInfo.g_strCameraModel == "AVT")
                //    return GrabImage_Sequence_GetFrameBeforeNextGrab_NoSetIntensity_AVT();
                //if (m_smVisionInfo.g_strCameraModel == "Teli")
                if (m_smVisionInfo.g_intGrabMode == 0)
                    return GrabImage_Sequence_NoSetIntensity_Teli(blnForInspection);
                else
                    return GrabImage_Sequence_NoSetIntensity_Teli_GrabAllFirst();
            }

            //if (m_smVisionInfo.g_strCameraModel == "Teli")
            {
                return GrabImage_Teli();
            }
            //else
            //{
            //    return GrabImage_Normal_GetFrameBeforeNextGrab_NoSetIntensity_AVTVimba();
            //}
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

            if (m_smVisionInfo.g_strCameraModel == "Teli")
            {
                return GrabImage_TeliNoSetIntensity();
            }

            if (!m_objAVTFireGrab.ref_blnCameraInitDone)
                return true;

            m_smVisionInfo.g_objGrabTime.Start();

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            bool blnSuccess = true;

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                #region if more than 1 image need to be captured
                // Set light source channel ON/OFF
                if (m_intGrabRequire > 1)
                { // change light source intensity for different image's effects
                    for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                    {
                        int intValueNo = 0;

                        // Due to some light source only ON for second image so its intensity value is at array no. 0.
                        // So we need to loop to find which array no. is for that image
                        for (int k = 1; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
                        {
                            // if this image no is in array k
                            if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[k] == i)
                            {
                                intValueNo = k;
                                break;
                            }
                        }
                        // Set camera gain
                        if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                        {
                            m_objAVTFireGrab.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                            m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                            if (m_objAVTFireGrab.ref_intSetGainDelay != 0)
                                Thread.Sleep(m_objAVTFireGrab.ref_intSetGainDelay);
                        }

                        // Set camera shuttle
                        if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                        {
                            m_objAVTFireGrab.SetCameraParameter(1, (uint)m_smVisionInfo.g_arrCameraShuttle[i]);
                            m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                            if (m_objAVTFireGrab.ref_intSetGainDelay != 0)
                                Thread.Sleep(m_objAVTFireGrab.ref_intSetGainDelay);
                        }

                        //switch (m_smCustomizeInfo.g_blnLEDiControl)
                        //{
                        //    case true:
                        //        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                        //            LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                        //                m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                        //                Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]));
                        //        else
                        //            LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                        //                m_smVisionInfo.g_arrLightSource[j].ref_intChannel, Convert.ToByte(0));
                        //        Thread.Sleep(3);
                        //        break;
                        //    case false:
                        //        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                        //            TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                        //                m_smVisionInfo.g_arrLightSource[j].ref_intChannel, m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]);
                        //        else
                        //            TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                        //                m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                        //        Thread.Sleep(3);
                        //        break;
                        //}
                    }
                }
                #endregion

                switch (i)
                {
                    case 0:
                        if ((m_intCameraOutState & (0x01)) == 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(0, m_smVisionInfo.g_intTriggerMode);

                            m_intCameraOutState |= 0x01;
                        }
                        if ((m_intCameraOutState & (0x02)) > 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(1, 0);

                            m_intCameraOutState &= ~0x02;
                        }
                        if ((m_intCameraOutState & (0x04)) > 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(2, 0);

                            m_intCameraOutState &= ~0x04;
                        }
                        break;
                    case 1:
                        if ((m_intCameraOutState & (0x01)) > 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(0, 0);
                            m_intCameraOutState &= ~0x01;
                        }
                        if ((m_intCameraOutState & (0x02)) == 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(1, m_smVisionInfo.g_intTriggerMode);
                            m_intCameraOutState |= 0x02;
                        }
                        if ((m_intCameraOutState & (0x04)) > 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(2, 0);
                            m_intCameraOutState &= ~0x04;
                        }
                        break;
                    case 2:
                        if ((m_intCameraOutState & (0x01)) > 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(0, 0);
                            m_intCameraOutState &= ~0x01;
                        }
                        if ((m_intCameraOutState & (0x02)) > 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(1, 0);
                            m_intCameraOutState &= ~0x02;
                        }
                        if ((m_intCameraOutState & (0x04)) == 0)
                        {
                            m_objAVTFireGrab.QuickOutPort(2, m_smVisionInfo.g_intTriggerMode);
                            m_intCameraOutState |= 0x04;
                        }
                        break;
                }

                if (!m_objAVTFireGrab.Grab())
                {
                    blnSuccess = false;
                    m_blnForceStopProduction = true;
                }

                Thread.Sleep(3);
            }

            if (blnSuccess)
            {
                SetGrabDone();
                m_smVisionInfo.g_objTransferTime.Start();

                for (int i = 0; i < m_intGrabRequire; i++)
                {
                    if (m_objAVTFireGrab.GetFrame(i))
                    {
                        if (m_objAVTFireGrab.ConvertFrame(i))
                        {
                            if (m_smVisionInfo.g_blnViewColorImage)
                            {
                                m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                                m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, i);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i);
                                m_smVisionInfo.g_arrImages[i].AddGain(m_smVisionInfo.g_arrImageGain[i]);
                            }

                            m_objAVTFireGrab.ReleaseImage(i);
                        }
                    }
                }
            }
            else
                SetGrabDone();

            if (m_objAVTFireGrab.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objAVTFireGrab.ref_strErrorText;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                if ((m_intCameraOutState & (0x01)) == 0)
                {
                    m_objAVTFireGrab.QuickOutPort(0, m_smVisionInfo.g_intTriggerMode);

                    m_intCameraOutState |= 0x01;
                }
                if ((m_intCameraOutState & (0x02)) > 0)
                {
                    m_objAVTFireGrab.QuickOutPort(1, 0);

                    m_intCameraOutState &= ~0x02;
                }
                if ((m_intCameraOutState & (0x04)) > 0)
                {
                    m_objAVTFireGrab.QuickOutPort(2, 0);

                    m_intCameraOutState &= ~0x04;
                }

                AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return true;
            }
        }

        public bool GrabImage_Teli()
        {
            if (!m_objTeliCamera.IsCameraInitDone())
            {
                //m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = true;
                //m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = true;
                m_smVisionInfo.g_strErrorMessage = "Camera No Connected";
                return true;
            }
            // Using Teli Camera
            m_smVisionInfo.g_objGrabTime.Start();

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            bool blnSuccess = true;
            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            TrackLog objTL = new TrackLog();
            float fTotalGrabTime = 0f;
            HiPerfTimer timer_TotalTime = new HiPerfTimer();
            HiPerfTimer timer_TotalGrabTime = new HiPerfTimer();
            timer_TotalTime.Start();

            int intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[0] * 0.001f);  // For Teli, Shuttle 1 == 1 microsecond

            m_objTeliCamera.DiscardFrame();

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (i > 0) // for second image and third image
                {
                    if (m_objTeliCamera.WaitFrameReady())
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, i - 1);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i - 1);
                            m_smVisionInfo.g_arrImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                        }

                        timer_TotalGrabTime.Stop();
                        fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;

                    }
                    else
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }

                }

                #region if more than 1 image need to be captured
                // Set light source channel ON/OFF
                if (m_intGrabRequire > 1)
                {
                    // Set camera gain
                    if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                    {
                        m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                        m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
                    }

                    // Set camera shuttle
                    if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                    {
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                        m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);

                        intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i] * 0.0001f);
                    }
                }
                #endregion

                timer_TotalGrabTime.Start();

                if (blnSuccess)//2021-10-21 ZJYEOH : No need to grab anymore if not success, as this will reset the camera error message
                {
                    if (!m_objTeliCamera.Grab())
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }

                if (i < m_intGrabRequire - 1)
                {
                    Thread.Sleep(intExposureTime);
                }
                else
                {
                    Thread.Sleep(Math.Max(intExposureTime, 10));
                }
            }

            if (!m_objTeliCamera.WaitFrameReady())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            if (blnSuccess)
            {
                SetGrabDone();
                m_smVisionInfo.g_objTransferTime.Start();

                if (m_objTeliCamera.GetFrame(m_intGrabRequire - 1))
                {
                    if (m_objTeliCamera.ConvertFrame(m_intGrabRequire - 1))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, m_intGrabRequire - 1);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, m_intGrabRequire - 1);
                            m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);
                        }
                    }
                }

                timer_TotalGrabTime.Stop();
                fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
            }
            else
                SetGrabDone();

            //m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = true;
            //m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = true;

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
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + fTotalGrabTime.ToString());
                //objTL.WriteLine("Total time = " + timer_TotalTime.Duration.ToString());
                return false;
            }
            else
            {
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }

                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + fTotalGrabTime.ToString());
                //objTL.WriteLine("Total time = " + timer_TotalTime.Duration.ToString());
                return true;
            }
        }

        public bool GrabImage_Teli_()
        {
#if (DEBUG || Debug_2_12 || RTXDebug)
            return true;
#endif
            // Using Teli Camera
            m_smVisionInfo.g_objGrabTime.Start();
            bool blnSuccess = true;
            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            TrackLog objTL = new TrackLog();
            float fTotalGrabTime = 0f;
            HiPerfTimer timer_TotalTime = new HiPerfTimer();
            HiPerfTimer timer_TotalGrabTime = new HiPerfTimer();
            timer_TotalTime.Start();

            int intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[0] * 0.001f);  // For Teli, Shuttle 1 == 1 microsecond

            m_objTeliCamera.DiscardFrame();

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (i > 0) // for second image and third image
                {
                    //Set intensity for second image and third image
                    for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                    {
                        int intValueNo = 1;

                        // Due to some light source only ON for second image so its intensity value is at array no. 0.
                        // So we need to loop to find which array no. is for that image
                        for (int k = 2; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
                        {
                            // if this image no is in array k
                            if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[k] == i)
                            {
                                intValueNo = k;
                                break;
                            }
                        }

                        // Set camera gain
                        if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                        {
                            m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                            m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                            //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                            //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
                        }

                        // Set camera shuttle
                        if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                        {
                            m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                            m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                            //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                            //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);

                            intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i] * 0.0001f);
                        }

                        switch (m_smCustomizeInfo.g_blnLEDiControl)
                        {
                            case true:
                                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                    LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                        m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                                        Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]));
                                else
                                    LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                        m_smVisionInfo.g_arrLightSource[j].ref_intChannel, Convert.ToByte(0));
                                Thread.Sleep(5);
                                break;
                            case false:
                                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                    TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                        m_smVisionInfo.g_arrLightSource[j].ref_intChannel, m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]);
                                else
                                    TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                        m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                                Thread.Sleep(5);
                                break;
                        }
                    }

                    if (m_objTeliCamera.WaitFrameReady())
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, i - 1);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i - 1);
                            m_smVisionInfo.g_arrImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                        }

                        timer_TotalGrabTime.Stop();
                        fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
                    }
                    else
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }

                #region if more than 1 image need to be captured
                //// Set light source channel ON/OFF
                //if (m_intGrabRequire > 1)
                //{
                //    // Set camera gain
                //    if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                //    {
                //        m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                //        m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                //        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                //        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
                //    }

                //    // Set camera shuttle
                //    if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                //    {
                //        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                //        m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                //        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                //        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);

                //        intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i] * 0.0001f);
                //    }
                //}

                #endregion

                // 2018 07 17 - JBTAN: temporary hide, use set intensity for each grab
                //switch (i)
                //{
                //    case 0:
                //        if ((m_intCameraOutState & (0x01)) == 0)
                //        {
                //            m_objTeliCamera.OutPort(0, m_smVisionInfo.g_intTriggerMode);

                //            m_intCameraOutState |= 0x01;
                //        }
                //        if ((m_intCameraOutState & (0x02)) > 0)
                //        {
                //            m_objTeliCamera.OutPort(1, 0);

                //            m_intCameraOutState &= ~0x02;
                //        }
                //        break;
                //    case 1:
                //        if ((m_intCameraOutState & (0x01)) > 0)
                //        {
                //            m_objTeliCamera.OutPort(0, 0);
                //            m_intCameraOutState &= ~0x01;
                //        }
                //        if ((m_intCameraOutState & (0x02)) == 0)
                //        {
                //            m_objTeliCamera.OutPort(1, m_smVisionInfo.g_intTriggerMode);
                //            m_intCameraOutState |= 0x02;
                //        }
                //        break;
                //        //case 2:
                //        //    if ((m_intCameraOutState & (0x01)) > 0)
                //        //    {
                //        //        m_objTeliCamera.OutPort(0, 0);
                //        //        m_intCameraOutState &= ~0x01;
                //        //    }
                //        //    if ((m_intCameraOutState & (0x02)) > 0)
                //        //    {
                //        //        m_objTeliCamera.OutPort(1, 0);
                //        //        m_intCameraOutState &= ~0x02;
                //        //    }
                //        //break;
                //}

                timer_TotalGrabTime.Start();

                if (blnSuccess)//2021-10-21 ZJYEOH : No need to grab anymore if not success, as this will reset the camera error message
                {
                    if (!m_objTeliCamera.Grab())
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }

                if (i < m_intGrabRequire - 1)
                {
                    Thread.Sleep(intExposureTime);
                }
                else
                {
                    Thread.Sleep(Math.Max(intExposureTime, 10));
                }
            }

            if (!m_objTeliCamera.WaitFrameReady())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            if (blnSuccess)
            {
                SetGrabDone();
                m_smVisionInfo.g_objTransferTime.Start();

                if (m_objTeliCamera.GetFrame(m_intGrabRequire - 1))
                {
                    if (m_objTeliCamera.ConvertFrame(m_intGrabRequire - 1))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, m_intGrabRequire - 1);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, m_intGrabRequire - 1);
                            m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);
                        }
                    }
                }
                timer_TotalGrabTime.Stop();
                fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
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
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }

                //Change to first grab intensity
                for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                {
                    switch (m_smCustomizeInfo.g_blnLEDiControl)
                    {
                        case true:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << 0)) > 0)
                                LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                                    Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0]));
                            else
                                LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, Convert.ToByte(0));
                            break;
                        case false:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << 0)) > 0)
                                TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0]);
                            else
                                TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                            break;
                    }
                }

                // 2018 07 17 - JBTAN: temporary hide, use set intensity for each grab
                //if ((m_intCameraOutState & (0x01)) == 0)
                //{
                //    m_objTeliCamera.OutPort(0, m_smVisionInfo.g_intTriggerMode);

                //    m_intCameraOutState |= 0x01;
                //}
                //if ((m_intCameraOutState & (0x02)) > 0)
                //{
                //    m_objTeliCamera.OutPort(1, 0);

                //    m_intCameraOutState &= ~0x02;
                //}

                AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + fTotalGrabTime.ToString());
                //objTL.WriteLine("Total time = " + timer_TotalTime.Duration.ToString());
                return true;
            }
        }

        public bool GrabImage_TeliNoSetIntensity()
        {
            if (!m_objTeliCamera.IsCameraInitDone())
            {
                m_smVisionInfo.g_strErrorMessage = "Camera No Connected";
                return true;
            }

            // Using Teli Camera
            m_smVisionInfo.g_objGrabTime.Start();

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            bool blnSuccess = true;
            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            TrackLog objTL = new TrackLog();
            float fTotalGrabTime = 0f;
            HiPerfTimer timer_TotalTime = new HiPerfTimer();
            HiPerfTimer timer_TotalGrabTime = new HiPerfTimer();
            timer_TotalTime.Start();

            int intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[0] * 0.001f);  // For Teli, Shuttle 1 == 1 microsecond

            m_objTeliCamera.DiscardFrame();

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (i > 0) // for second image and third image
                {
                    //Set intensity for second image and third image
                    //for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                    //{
                    //    int intValueNo = 1;

                    //    // Due to some light source only ON for second image so its intensity value is at array no. 0.
                    //    // So we need to loop to find which array no. is for that image
                    //    for (int k = 2; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
                    //    {
                    //        // if this image no is in array k
                    //        if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[k] == i)
                    //        {
                    //            intValueNo = k;
                    //            break;
                    //        }
                    //    }

                    //    // Set camera gain
                    //    if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                    //    {
                    //        m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                    //        m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                    //        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                    //        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
                    //    }

                    //    // Set camera shuttle
                    //    if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                    //    {
                    //        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                    //        m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                    //        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                    //        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);

                    //        intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i] * 0.0001f);
                    //    }

                    //    switch (m_smCustomizeInfo.g_blnLEDiControl)
                    //    {
                    //        case true:
                    //            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                    //                LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                    //                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                    //                    Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]));
                    //            else
                    //                LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                    //                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, Convert.ToByte(0));
                    //            Thread.Sleep(5);
                    //            break;
                    //        case false:
                    //            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                    //                TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                    //                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]);
                    //            else
                    //                TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                    //                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                    //            Thread.Sleep(5);
                    //            break;
                    //    }
                    //}

                    if (m_objTeliCamera.WaitFrameReady())
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, i - 1);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());

                            if (m_smProductionInfo.g_blnAllRunGrabWithoutUseImage)
                            {
                                if (m_smVisionInfo.g_arrDebugImages.Count != m_smVisionInfo.g_arrImages.Count)
                                {
                                    for (int d = 0; d < m_smVisionInfo.g_arrImages.Count; d++)
                                    {
                                        m_smVisionInfo.g_arrDebugImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    }
                                }

                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrDebugImages, i - 1);
                                m_smVisionInfo.g_arrDebugImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i - 1);
                                m_smVisionInfo.g_arrImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                            }
                        }

                        timer_TotalGrabTime.Stop();
                        fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
                    }
                    else
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }

                #region if more than 1 image need to be captured
                // Set light source channel ON/OFF
                if (m_intGrabRequire > 1)
                {
                    // Set camera gain
                    if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                    {
                        m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                        m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
                    }

                    // Set camera shuttle
                    if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                    {
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                        m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);

                        intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i] * 0.0001f);
                    }
                }

                #endregion

                // 2018 07 17 - JBTAN: temporary hide, use set intensity for each grab
                switch (i)
                {
                    case 0:
                        if ((m_intCameraOutState & (0x01)) == 0)
                        {
                            m_objTeliCamera.OutPort(0, m_smVisionInfo.g_intTriggerMode);

                            m_intCameraOutState |= 0x01;
                        }
                        if ((m_intCameraOutState & (0x02)) > 0)
                        {
                            m_objTeliCamera.OutPort(1, 0);

                            m_intCameraOutState &= ~0x02;
                        }
                        break;
                    case 1:
                        if ((m_intCameraOutState & (0x01)) > 0)
                        {
                            m_objTeliCamera.OutPort(0, 0);
                            m_intCameraOutState &= ~0x01;
                        }
                        if ((m_intCameraOutState & (0x02)) == 0)
                        {
                            m_objTeliCamera.OutPort(1, m_smVisionInfo.g_intTriggerMode);
                            m_intCameraOutState |= 0x02;
                        }
                        break;
                        //case 2:
                        //    if ((m_intCameraOutState & (0x01)) > 0)
                        //    {
                        //        m_objTeliCamera.OutPort(0, 0);
                        //        m_intCameraOutState &= ~0x01;
                        //    }
                        //    if ((m_intCameraOutState & (0x02)) > 0)
                        //    {
                        //        m_objTeliCamera.OutPort(1, 0);
                        //        m_intCameraOutState &= ~0x02;
                        //    }
                        //break;
                }

                timer_TotalGrabTime.Start();

                if (blnSuccess)//2021-10-21 ZJYEOH : No need to grab anymore if not success, as this will reset the camera error message
                {
                    if (!m_objTeliCamera.Grab())
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }

                if (i < m_intGrabRequire - 1)
                {
                    Thread.Sleep(intExposureTime);
                }
                else
                {
                    Thread.Sleep(Math.Max(intExposureTime, 10));
                }
            }

            if (!m_objTeliCamera.WaitFrameReady())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            if (blnSuccess)
            {
                SetGrabDone();
                m_smVisionInfo.g_objTransferTime.Start();

                if (m_objTeliCamera.GetFrame(m_intGrabRequire - 1))
                {
                    if (m_objTeliCamera.ConvertFrame(m_intGrabRequire - 1))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, m_intGrabRequire - 1);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());

                            if (m_smProductionInfo.g_blnAllRunGrabWithoutUseImage)
                            {
                                if (m_smVisionInfo.g_arrDebugImages.Count != m_smVisionInfo.g_arrImages.Count)
                                {
                                    for (int d = 0; d < m_smVisionInfo.g_arrImages.Count; d++)
                                    {
                                        m_smVisionInfo.g_arrDebugImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    }
                                }

                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrDebugImages, m_intGrabRequire - 1);
                                m_smVisionInfo.g_arrDebugImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, m_intGrabRequire - 1);
                                m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);

                            }
                        }
                    }
                }
                timer_TotalGrabTime.Stop();
                fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
            }
            else
                SetGrabDone();

            if (m_objTeliCamera.GetErrorMessage() != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objTeliCamera.GetErrorMessage();
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }

                ////Change to first grab intensity
                //for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                //{
                //    switch (m_smCustomizeInfo.g_blnLEDiControl)
                //    {
                //        case true:
                //            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << 0)) > 0)
                //                LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                //                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                //                    Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0]));
                //            else
                //                LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                //                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, Convert.ToByte(0));
                //            break;
                //        case false:
                //            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << 0)) > 0)
                //                TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                //                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, m_smVisionInfo.g_arrLightSource[j].ref_arrValue[0]);
                //            else
                //                TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                //                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                //            break;
                //    }
                //}

                // 2018 07 17 - JBTAN: temporary hide, use set intensity for each grab
                if ((m_intCameraOutState & (0x01)) == 0)
                {
                    m_objTeliCamera.OutPort(0, m_smVisionInfo.g_intTriggerMode);

                    m_intCameraOutState |= 0x01;
                }
                if ((m_intCameraOutState & (0x02)) > 0)
                {
                    m_objTeliCamera.OutPort(1, 0);

                    m_intCameraOutState &= ~0x02;
                }

                AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + fTotalGrabTime.ToString());
                //objTL.WriteLine("Total time = " + timer_TotalTime.Duration.ToString());
                return true;
            }
        }

        /// <summary>
        /// Returns whether the worker thread has stopped.
        /// </summary>
        public bool IsThreadStopped
        {
            get
            {
                //lock (m_objStopLock)
                {
                    return m_blnStopped;
                }
            }
        }

        public bool GrabImage_Sequence_NoSetIntensity_Teli(bool blnForInspection)
        {
            if (!m_objTeliCamera.IsCameraInitDone())
            {
                //m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = true;
                //m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = true;
                m_smVisionInfo.g_strErrorMessage = "Camera No Connected";
                return true;
            }

            // Using Teli Camera
            m_smVisionInfo.g_objGrabTime.Start();

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            bool blnSuccess = true;
            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            TrackLog objTL = new TrackLog();
            float fTotalGrabTime = 0f;
            HiPerfTimer timer_TotalTime = new HiPerfTimer();
            HiPerfTimer timer_TotalGrabTime = new HiPerfTimer();
            timer_TotalTime.Start();

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (i > 0) // for second image and third image
                {
                    if (m_objTeliCamera.WaitFrameReady())
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, i - 1);
                            ConvertColorToMono(i - 1);
                            m_smVisionInfo.g_arrImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());

                            if (m_smVisionInfo.g_intRotateFlip == 2)
                            {
                                m_smVisionInfo.g_objMemoryImage.Rotate90Image(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else if (m_smVisionInfo.g_intRotateFlip == 3)
                            {
                                m_smVisionInfo.g_objMemoryImage.RotateMinus90Image(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else if (m_smVisionInfo.g_intRotateFlip == 4)
                            {
                                m_smVisionInfo.g_objMemoryImage.FlipHorizontalImage(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else if (m_smVisionInfo.g_intRotateFlip == 5)
                            {
                                m_smVisionInfo.g_objMemoryImage.FlipVerticalImage(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_ojRotateImage);
                            }

                            if (m_smProductionInfo.g_blnAllRunGrabWithoutUseImage)
                            {
                                if (m_smVisionInfo.g_arrDebugImages.Count != m_smVisionInfo.g_arrImages.Count)
                                {
                                    for (int d = 0; d < m_smVisionInfo.g_arrImages.Count; d++)
                                    {
                                        m_smVisionInfo.g_arrDebugImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    }
                                }

                                m_smVisionInfo.g_ojRotateImage.CopyTo(ref m_smVisionInfo.g_arrDebugImages, i - 1);
                                m_smVisionInfo.g_arrDebugImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                            }
                            else
                            {
                                m_smVisionInfo.g_ojRotateImage.CopyTo(ref m_smVisionInfo.g_arrImages, i - 1);
                                m_smVisionInfo.g_arrImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                            }
                        }

                        timer_TotalGrabTime.Stop();
                        fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
                        //objTL.WriteLine("grab time " + i.ToString() + " = " + fTotalGrabTime.ToString());
                    }
                    else
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }

                    if (i == 1)
                    {
                        if (m_smVisionInfo.g_blnSaveImageAfterGrab && blnForInspection) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TS\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_intCounter + ".bmp");
                    }
                    else
                    {
                        if (m_smVisionInfo.g_blnSaveImageAfterGrab && blnForInspection) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TS\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_intCounter + "_Image" + (i - 1) + ".bmp");
                    }

                    //// STTrackLog.WriteLine("Grab i = " + i.ToString());
                    //if (i == 1)
                    //{
                    //    //      STTrackLog.WriteLine("Grab 1 done.");
                    //    m_bGrabImage1Result = true;
                    //    m_bGrabImage1Done = true;
                    //}
                    //else if (i == 2)
                    //{
                    //    m_bGrabImage2Result = true;
                    //    m_bGrabImage2Done = true;
                    //}
                    //else if (i == 3)
                    //{
                    //    m_bGrabImage3Result = true;
                    //    m_bGrabImage3Done = true;
                    //}
                    //else if (i == 4)
                    //{
                    //    m_bGrabImage4Result = true;
                    //    m_bGrabImage4Done = true;
                    //}
                    //else if (i == 5)
                    //{
                    //    m_bGrabImage5Result = true;
                    //    m_bGrabImage5Done = true;
                    //}
                }

                #region if more than 1 image need to be captured
                // Set light source channel ON/OFF
                if (m_intGrabRequire > 1)
                {
                    // Set camera gain
                    if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                    {
                        m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                        m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
                    }

                    // Set camera shuttle
                    if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                    {
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                        m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
                    }
                }
                #endregion

                timer_TotalGrabTime.Start();
                if (i == 0)
                {
                    if (m_smCustomizeInfo.g_blnVTControl)
                    {
                        m_objTeliCamera.OutPort(1, 3);
                    }
                }

                if (blnSuccess)//2021-10-21 ZJYEOH : No need to grab anymore if not success, as this will reset the camera error message
                {
                    if (!m_objTeliCamera.Grab())
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }

                //objTL.WriteLine("grab time at last line "+i.ToString()+" = " + fTotalGrabTime.ToString());
            }

            if (!m_objTeliCamera.WaitFrameReady())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            if (blnSuccess)
            {
                SetGrabDone();
                m_smVisionInfo.g_objTransferTime.Start();

                if (m_objTeliCamera.GetFrame(m_intGrabRequire - 1))
                {
                    if (m_objTeliCamera.ConvertFrame(m_intGrabRequire - 1))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                            m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, m_intGrabRequire - 1);
                            ConvertColorToMono(m_intGrabRequire - 1);
                            m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);
                        }
                        else
                        {
                            m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                            m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());

                            if (m_smVisionInfo.g_intRotateFlip == 1)
                            {
                                m_smVisionInfo.g_objMemoryImage.Rotate180Image(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else if(m_smVisionInfo.g_intRotateFlip == 2)
                            {
                                m_smVisionInfo.g_objMemoryImage.Rotate90Image(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else if (m_smVisionInfo.g_intRotateFlip == 3)
                            {
                                m_smVisionInfo.g_objMemoryImage.RotateMinus90Image(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else if (m_smVisionInfo.g_intRotateFlip == 4)
                            {
                                m_smVisionInfo.g_objMemoryImage.FlipHorizontalImage(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else if (m_smVisionInfo.g_intRotateFlip == 5)
                            {
                                m_smVisionInfo.g_objMemoryImage.FlipVerticalImage(ref m_smVisionInfo.g_ojRotateImage, false);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_ojRotateImage);
                            }

                            if (m_smProductionInfo.g_blnAllRunGrabWithoutUseImage)
                            {
                                if (m_smVisionInfo.g_arrDebugImages.Count != m_smVisionInfo.g_arrImages.Count)
                                {
                                    for (int d = 0; d < m_smVisionInfo.g_arrImages.Count; d++)
                                    {
                                        m_smVisionInfo.g_arrDebugImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    }
                                }

                                m_smVisionInfo.g_ojRotateImage.CopyTo(ref m_smVisionInfo.g_arrDebugImages, m_intGrabRequire - 1);
                                m_smVisionInfo.g_arrDebugImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);
                            }
                            else
                            {
                                m_smVisionInfo.g_ojRotateImage.CopyTo(ref m_smVisionInfo.g_arrImages, m_intGrabRequire - 1);
                                m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);
                            }
                        }

                        if (m_intGrabRequire == 1)
                        {
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab && blnForInspection) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TS\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_intCounter + ".bmp");
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab && blnForInspection) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TS\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                        }

                        //if (m_intGrabRequire == 1)
                        //{
                        //    m_bGrabImage1Result = true;
                        //    m_bGrabImage1Done = true;
                        //}
                        //else if (m_intGrabRequire == 2)
                        //{
                        //    m_bGrabImage2Result = true;
                        //    m_bGrabImage2Done = true;
                        //}
                        //else if (m_intGrabRequire == 3)
                        //{
                        //    m_bGrabImage3Result = true;
                        //    m_bGrabImage3Done = true;
                        //}
                        //else if (m_intGrabRequire == 4)
                        //{
                        //    m_bGrabImage4Result = true;
                        //    m_bGrabImage4Done = true;
                        //}
                        //else if (m_intGrabRequire == 5)
                        //{
                        //    m_bGrabImage5Result = true;
                        //    m_bGrabImage5Done = true;
                        //}
                    }
                }
                timer_TotalGrabTime.Stop();
                fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
                //objTL.WriteLine("Total grab time after Grab loop = " + fTotalGrabTime.ToString());
            }
            else
                SetGrabDone();

            //m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = true;

            //Reset outport
            if (m_smCustomizeInfo.g_blnLEDiControl)
            {
                m_objTeliCamera.OutPort(1, 3);
                Thread.Sleep(3);
                m_objTeliCamera.OutPort(1, 0);
            }
            else if (m_smCustomizeInfo.g_blnVTControl)
            {
                m_objTeliCamera.OutPort(1, 0);
            }

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
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + fTotalGrabTime.ToString());
                //objTL.WriteLine("Total time = " + timer_TotalTime.Duration.ToString());
                return false;
            }
            else
            {
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }

                if (m_smVisionInfo.g_bImageStatisticAnalysisON)
                {
                    m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo = true;

                    while (true)
                    {
                        if (!m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo)
                            break;

                        Thread.Sleep(1);
                    }
                }

                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + fTotalGrabTime.ToString());
                //objTL.WriteLine("Total time = " + timer_TotalTime.Duration.ToString());

                return true;
            }
        }
        public bool GrabImage_Sequence_NoSetIntensity_Teli_GrabAllFirst()
        {
            if (!m_objTeliCamera.IsCameraInitDone())
            {
                //m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = true;
                //m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = true;
                m_smVisionInfo.g_strErrorMessage = "Camera No Connected";
                return true;
            }

            // Using Teli Camera
            m_smVisionInfo.g_objGrabTime.Start();

            // Set camera gain
            if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
            {
                m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[0]);
                m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
            }

            // Set camera shuttle
            if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
            {
                m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[0]);
                m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
            }

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            bool blnSuccess = true;
            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            TrackLog objTL = new TrackLog();
            float fTotalGrabTime = 0f;
            HiPerfTimer timer_TotalTime = new HiPerfTimer();
            HiPerfTimer timer_TotalGrabTime = new HiPerfTimer();
            timer_TotalTime.Start();
            m_arrBufferPointer = new List<IntPtr>();

            for (int i = 0; i < m_intGrabRequire; i++)
            {

                if (i > 0)
                {
                    if (m_objTeliCamera.WaitTriggerWaitDone())
                    {
                        // Set camera gain
                        if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                        {
                            m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                            m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                        }

                        if (/*!blnStartGrab &&*/ !m_objTeliCamera.Grab(i))
                        {
                            blnSuccess = false;
                            m_blnForceStopProduction = true;
                        }
                    }
                    else
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }
                else
                {
                    if (/*!blnStartGrab &&*/ !m_objTeliCamera.Grab(i))
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }

                //Set light source channel ON / OFF
                if (m_intGrabRequire > 1 && ((i + 1) < m_smVisionInfo.g_arrCameraShuttle.Count))
                {

                    // Set camera shuttle
                    if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i + 1])
                    {
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i + 1]);
                        m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i + 1];
                        //if (m_objTeliCamera.ref_intSetGainDelay != 0)
                        //    Thread.Sleep(m_objTeliCamera.ref_intSetGainDelay);
                    }

                }

                if (!m_objTeliCamera.WaitFrameAcquiredReady(i))
                {
                    blnSuccess = false;
                    m_blnForceStopProduction = true;
                }

            }

            if (blnSuccess)
            {
                //m_objTeliCamera.TriggerImageBufferRead();
                SetGrabDone();
                m_smVisionInfo.g_objTransferTime.Start();

            }
            else
                SetGrabDone();

            if (!m_objTeliCamera.WaitTriggerWaitDone())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (m_smVisionInfo.g_blnViewColorImage)
                {
                    m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                    m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objTeliCamera.GetImagePointer());
                    m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, i);
                    ConvertColorToMono(i);
                    m_smVisionInfo.g_arrImages[i].AddGain(m_smVisionInfo.g_arrImageGain[i]);
                }
                else
                {
                    m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                    m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objTeliCamera.GetImageBufferPointer(i));

                    if (m_smVisionInfo.g_intRotateFlip == 2)
                    {
                        m_smVisionInfo.g_objMemoryImage.Rotate90Image(ref m_smVisionInfo.g_ojRotateImage, false);
                    }
                    else if (m_smVisionInfo.g_intRotateFlip == 3)
                    {
                        m_smVisionInfo.g_objMemoryImage.RotateMinus90Image(ref m_smVisionInfo.g_ojRotateImage, false);
                    }
                    else if (m_smVisionInfo.g_intRotateFlip == 4)
                    {
                        m_smVisionInfo.g_objMemoryImage.FlipHorizontalImage(ref m_smVisionInfo.g_ojRotateImage, false);
                    }
                    else if (m_smVisionInfo.g_intRotateFlip == 5)
                    {
                        m_smVisionInfo.g_objMemoryImage.FlipVerticalImage(ref m_smVisionInfo.g_ojRotateImage, false);
                    }
                    else
                    {
                        m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_ojRotateImage);
                    }

                    if (m_smProductionInfo.g_blnAllRunGrabWithoutUseImage)
                    {
                        if (m_smVisionInfo.g_arrDebugImages.Count != m_smVisionInfo.g_arrImages.Count)
                        {
                            for (int d = 0; d < m_smVisionInfo.g_arrImages.Count; d++)
                            {
                                m_smVisionInfo.g_arrDebugImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                            }
                        }

                        m_smVisionInfo.g_ojRotateImage.CopyTo(ref m_smVisionInfo.g_arrDebugImages, i);
                        m_smVisionInfo.g_arrDebugImages[i].AddGain(m_smVisionInfo.g_arrImageGain[i]);
                    }
                    else
                    {
                        m_smVisionInfo.g_ojRotateImage.CopyTo(ref m_smVisionInfo.g_arrImages, i);
                        m_smVisionInfo.g_arrImages[i].AddGain(m_smVisionInfo.g_arrImageGain[i]);
                    }
                }

            }
            
            //Reset outport
            if (m_smCustomizeInfo.g_blnLEDiControl)
            {
                m_objTeliCamera.OutPort(1, 3);
                Thread.Sleep(3);
                m_objTeliCamera.OutPort(1, 0);
            }
            else if (m_smCustomizeInfo.g_blnVTControl)
            {
                m_objTeliCamera.OutPort(1, 0);
            }

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
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + fTotalGrabTime.ToString());
                //objTL.WriteLine("Total time = " + timer_TotalTime.Duration.ToString());
                return false;
            }
            else
            {
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }

                if (m_smVisionInfo.g_bImageStatisticAnalysisON)
                {
                    m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo = true;

                    while (true)
                    {
                        if (!m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo)
                            break;

                        Thread.Sleep(1);
                    }
                }

                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + fTotalGrabTime.ToString());
                //objTL.WriteLine("Total time = " + timer_TotalTime.Duration.ToString());

                return true;
            }
        }
        public bool InitCamera(int intPort, String SerialNo, int intResolutionX, int intResolutionY, bool blnFirstTime)
        {
            bool blnInitSuccess = true;

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

                                    m_smVisionInfo.g_arrImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    m_smVisionInfo.g_arrRotatedImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

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
                        // 2018 07 18 - JBTAN: some light source is used for grab 1 and grab 2, only set intensity for image grab 1
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

            if (blnFirstTime)
            {
                InitSaveImageBuffer(m_intGrabRequire);
            }

            // Sorting light source
            LightSource[] arrLightSource = new LightSource[m_smVisionInfo.g_arrLightSource.Count];
            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                int intStartIndex = m_smVisionInfo.g_arrLightSource[i].ref_strType.Length - 1;
                int intLightSourceIndex = Convert.ToInt32(m_smVisionInfo.g_arrLightSource[i].ref_strType.Substring(intStartIndex, 1));

                arrLightSource[intLightSourceIndex] = m_smVisionInfo.g_arrLightSource[i];
            }

            m_smVisionInfo.g_arrLightSource.Clear();
            for (int i = 0; i < arrLightSource.Length; i++)
            {
                m_smVisionInfo.g_arrLightSource.Add(arrLightSource[i]);
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
                    m_smVisionInfo.g_arrCameraShuttle[i] = Convert.ToUInt32(m_smVisionInfo.g_fCameraShuttle);
                }

                if (i == 0)
                {
                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                        m_objAVTFireGrab.SetCameraParameter(1, (uint)m_smVisionInfo.g_arrCameraShuttle[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);

                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
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

                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                m_smVisionInfo.g_arrImageGain[i] = objFileHandle.GetValueAsFloat("ImageGain" + i.ToString(), 1);
            }

            // Define Image View Count
            m_smVisionInfo.g_intImageViewCount = m_intGrabRequire;
            ImageDrawing.SetImageCount(m_intGrabRequire, m_smVisionInfo.g_intVisionIndex);
            ImageDrawing.SetImageMergeType(m_smVisionInfo.g_intImageMergeType, m_smVisionInfo.g_intVisionIndex);

            if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrImages.Count)
            {
                m_smVisionInfo.g_intSelectedImage = 0;
            }

            return blnInitSuccess;
        }

        public bool InitCameraSequence(int intPort, String SerialNo, int intResolutionX, int intResolutionY, bool blnFirstTime)
        {
            bool blnInitSuccess = true;

            if (blnFirstTime)
            {
                if (m_smVisionInfo.g_strCameraModel == "AVT")
                {
                    if (!m_objAVTFireGrab.InitializeCamera(intPort, false))
                        blnInitSuccess = false;
                }
                else if (m_smVisionInfo.g_strCameraModel == "Teli")
                {
                    if (m_smVisionInfo.g_intGrabMode == 0)
                    {
                        if (!m_objTeliCamera.InitializeCamera(SerialNo, intResolutionX, intResolutionY))
                        {
                            blnInitSuccess = false;
                            SRMMessageBox.Show("Serial No. " + SerialNo + " - " + m_smVisionInfo.g_strVisionDisplayName + " " + m_objTeliCamera.GetErrorMessage());
                        }
                    }
                    else
                    {
                        if (!m_objTeliCamera.InitializeCamera_LowLevelAPI(SerialNo, intResolutionX, intResolutionY, false))
                        {
                            blnInitSuccess = false;
                            SRMMessageBox.Show("Serial No. " + SerialNo + " - " + m_smVisionInfo.g_strVisionDisplayName + " " + m_objTeliCamera.GetErrorMessage());
                        }
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

            //arrName means comport number
            for (int x = 0; x < arrName.Length; x++)
            {
                RegistryKey child = subKey.OpenSubKey(arrName[x], true);
                RegistryKey grandChild = child.CreateSubKey(m_smVisionInfo.g_strVisionFolderName);

                string[] arrType = grandChild.GetValueNames();

                //arrType means light source type
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
                        if (strLightControlMaskList[y].Contains(m_smVisionInfo.g_strVisionName + m_smVisionInfo.g_strVisionNameNo + " - " + arrType[i]))
                            blnLightMaskingFound = true;
                    }
                    if (blnLightMaskingFound)
                    {
                        // 2021 05 04 - CCENG: need to read VisionNameNo as well due to Seal will have 2 modules.
                        //objLightSource.ref_intSeqNo = Convert.ToInt32(subKey.GetValue(m_smVisionInfo.g_strVisionName + " - " + arrType[i], 1));
                        objLightSource.ref_intSeqNo = Convert.ToInt32(subKey.GetValue(m_smVisionInfo.g_strVisionName + m_smVisionInfo.g_strVisionNameNo + " - " + arrType[i], 1));
                    }
                    else
                    {
                        objLightSource.ref_intSeqNo = fileHandle.GetValueAsInt(strSearch, 1);

                        // 2021 05 04 - CCENG: need to read VisionNameNo as well due to Seal will have 2 modules.
                        //subKey.SetValue(m_smVisionInfo.g_strVisionName + " - " + arrType[i], objLightSource.ref_intSeqNo);
                        subKey.SetValue(m_smVisionInfo.g_strVisionName + m_smVisionInfo.g_strVisionNameNo + " - " + arrType[i], objLightSource.ref_intSeqNo);
                    }
                    objLightSource.ref_intValue = 31;// objFileHandle.GetValueAsInt(arrType[i], 31); 2019-09-12 ZJYEOH : do not read this value from XML file because it combine all intensity value in the light source sequence which will exceed 32 bit integer value when have 5 grab 
                    objLightSource.ref_intPortNo = x;

                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                    {
                        //Need to change
                        m_objAVTFireGrab.OutPort(0, m_smVisionInfo.g_intTriggerMode);
                        Thread.Sleep(10);
                        m_objAVTFireGrab.OutPort(1, 5);
                    }
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                    {
                        m_objTeliCamera.OutPort(0, m_smVisionInfo.g_intTriggerMode);
                        Thread.Sleep(10);
                        m_objTeliCamera.OutPort(1, 0);
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
                                    if (blnFirstTime)
                                    {
                                        if (m_smVisionInfo.g_blnViewColorImage)
                                        {
                                            m_smVisionInfo.g_arrColorImages.Add(new CImageDrawing());
                                            m_smVisionInfo.g_arrColorRotatedImages.Add(new CImageDrawing());
                                        }

                                        m_smVisionInfo.g_arrImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        m_smVisionInfo.g_arrRotatedImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

                                        m_smVisionInfo.g_arrCameraShuttle.Add(new float());
                                        m_smVisionInfo.g_arrCameraGain.Add(new int());
                                        m_smVisionInfo.g_arrImageGain.Add(new float());


                                    }
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

                    //// Keep light source intensity previous setting
                    //m_arrCameraIntensityPrev[i] = objLightSource.ref_intValue;
                }
            }

            if (blnFirstTime)
            {
                InitSaveImageBuffer(m_intGrabRequire);
            }

            // Sorting light source
            LightSource[] arrLightSource = new LightSource[m_smVisionInfo.g_arrLightSource.Count];
            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                int intStartIndex = m_smVisionInfo.g_arrLightSource[i].ref_strType.Length - 1;
                int intLightSourceIndex = Convert.ToInt32(m_smVisionInfo.g_arrLightSource[i].ref_strType.Substring(intStartIndex, 1));

                arrLightSource[intLightSourceIndex] = m_smVisionInfo.g_arrLightSource[i];
            }

            m_smVisionInfo.g_arrLightSource.Clear();
            for (int i = 0; i < arrLightSource.Length; i++)
            {
                m_smVisionInfo.g_arrLightSource.Add(arrLightSource[i]);
            }

            if (m_smCustomizeInfo.g_blnLEDiControl)
            {
                //Set to stop mode
                LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                Thread.Sleep(10);
                for (int i = 0; i < m_intGrabRequire; i++)
                {
                    int intValue1 = 0;
                    int intValue2 = 0;
                    int intValue3 = 0;

                    if (m_intGrabRequire > 0)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                        {
                            int intValueNo = 0;

                            // Due to some light source only ON for second image so its intensity value is at array no. 0.
                            // So we need to loop to find which array no. is for that image
                            for (int k = 0; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
                            {
                                // if this image no is in array k
                                if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo != null)
                                {
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[k] == i)
                                    {
                                        intValueNo = k;

                                        switch (j)
                                        {
                                            case 0:
                                                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                {
                                                    intValue1 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                }
                                                break;
                                            case 1:
                                                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                {
                                                    intValue2 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                }
                                                break;
                                            case 2:
                                                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                {
                                                    intValue3 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                }
                                                break;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                        //Set all light source for sequence light controller for each grab
                        LEDi_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, i, intValue1, intValue2, intValue3, 0);
                        Thread.Sleep(10);
                        TrackLog objTL = new TrackLog();
                        objTL.WriteLine("Vision 1");
                        objTL.WriteLine("Sequence number: " + i.ToString());
                        objTL.WriteLine("Com: " + m_smVisionInfo.g_arrLightSource[0].ref_intPortNo.ToString());
                        objTL.WriteLine("Intensity 1: " + intValue1.ToString());
                        objTL.WriteLine("Intensity 2: " + intValue2.ToString());
                        objTL.WriteLine("Intensity 3: " + intValue3.ToString());
                    }
                }
                LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                Thread.Sleep(100);
                //Set to run mode
                LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);

                Thread.Sleep(10);
            }
            else if (m_smCustomizeInfo.g_blnVTControl)
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

                TrackLog objTL = new TrackLog();
                objTL.WriteLine("m_smVisionInfo.g_arrLightSource[0].ref_intPortNo=" + m_smVisionInfo.g_arrLightSource[0].ref_intPortNo.ToString());
                objTL.WriteLine("uintGroupNum=" + uintGroupNum.ToString());
                VT_Control.SetConfigMode(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);
                VT_Control.SetGroupsAvailable(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, uintGroupNum);

                //Setting active flag
                for (int m = 0; m < m_intGrabRequire; m++)
                {
                    objTL.WriteLine("SetActiveOutFlag m=" + m.ToString());
                    VT_Control.SetActiveOutFlag(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, uintGroupNum, m, 1);
                }

                for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                {
                    int intCount = 0;
                    for (int i = 0; i < m_intGrabRequire; i++)
                    {
                        if (m_intGrabRequire > 0)
                        {
                            // if this image no is in array intCount
                            if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo != null)
                            {
                                if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo.Count != intCount)
                                {
                                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[intCount] == i)
                                    {
                                        objTL.WriteLine("Set Seq Intensity A Port=" + m_smVisionInfo.g_arrLightSource[j].ref_intPortNo.ToString() +
                                                        ", i=" + i.ToString() +
                                                        ", channel=" + m_smVisionInfo.g_arrLightSource[j].ref_intChannel.ToString() +
                                                        ", value=" + m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intCount].ToString());
                                        VT_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo, i, m_smVisionInfo.g_arrLightSource[j].ref_intChannel, m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intCount]);
                                        intCount++;
                                    }
                                    else
                                    {
                                        objTL.WriteLine("Set Seq Intensity B Port=" + m_smVisionInfo.g_arrLightSource[j].ref_intPortNo.ToString() +
                                                        ", i=" + i.ToString() +
                                                        ", channel=" + m_smVisionInfo.g_arrLightSource[j].ref_intChannel.ToString() +
                                                        ", value=0");

                                        VT_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo, i, m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                                    }
                                }
                                else
                                {
                                    objTL.WriteLine("Set Seq Intensity C Port=" + m_smVisionInfo.g_arrLightSource[j].ref_intPortNo.ToString() +
                                                    ", i=" + i.ToString() +
                                                    ", channel=" + m_smVisionInfo.g_arrLightSource[j].ref_intChannel.ToString() +
                                                    ", value=0");

                                    VT_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo, i, m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
                                }
                            }
                        }
                    }
                    //VT_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo, m_smVisionInfo.g_arrLightSource[j].ref_intChannel);
                }

                objTL.WriteLine("SetRunMode=" + m_smVisionInfo.g_arrLightSource[0].ref_intPortNo.ToString());
                VT_Control.SetRunMode(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo);
            }
            for (int i = 0; i < m_smVisionInfo.g_arrCameraShuttle.Count; i++)
            {
                m_smVisionInfo.g_arrCameraShuttle[i] = objFileHandle.GetValueAsFloat("Shutter" + i.ToString(), 0f);

                if (m_smVisionInfo.g_arrCameraShuttle[i] == 0)
                {
                    m_smVisionInfo.g_arrCameraShuttle[i] = Convert.ToUInt32(m_smVisionInfo.g_fCameraShuttle);
                }

                if (i == 0)
                {
                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                        m_objAVTFireGrab.SetCameraParameter(1, (uint)m_smVisionInfo.g_arrCameraShuttle[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
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

                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                m_smVisionInfo.g_arrImageGain[i] = objFileHandle.GetValueAsFloat("ImageGain" + i.ToString(), 1);
            }

            // Define Image View Count
            m_smVisionInfo.g_intImageViewCount = m_intGrabRequire;
            ImageDrawing.SetImageCount(m_intGrabRequire, m_smVisionInfo.g_intVisionIndex);
            ImageDrawing.SetImageMergeType(m_smVisionInfo.g_intImageMergeType, m_smVisionInfo.g_intVisionIndex);

            if (m_smVisionInfo.g_intSelectedImage >= m_smVisionInfo.g_arrImages.Count)
            {
                m_smVisionInfo.g_intSelectedImage = 0;
            }

            return blnInitSuccess;
        }

        /// <summary>
        /// Tells the thread to stop, typically after completing its 
        /// current work item.
        /// </summary>
        public void StopThread()
        {
            //lock (m_objStopLock)
            {
                m_blnStopping = true;
            }

            WaitAllThreadStopped();
        }

        public void PauseThread()
        {
            m_blnPause = true;
        }

        public void StartThread()
        {
            m_blnPause = false;
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
        private void TakeAction_TCPIPIO(string strMessage)
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
                strMessage = strMessage.Remove(0, 1);
                string[] strString = strMessage.Split(',', '>');

                if (strString.Length == 0)
                    return;

                int intTestOption = 0;
                if (strString.Length < 2 || !int.TryParse(strString[1], out intTestOption))
                    intTestOption = -1;

                int intVisionIndex = 0;
                if (strString.Length < 3 || !int.TryParse(strString[2], out intVisionIndex))
                    intVisionIndex = -1;

                switch (strString[0])
                {
                    case "SOV":
                        if (intVisionIndex != Math.Pow(2, m_smVisionInfo.g_intVisionIndex) || intTestOption < 0 || m_smVisionInfo.g_intMachineStatus != 2)
                            m_smTCPIPIO.Send(m_smVisionInfo.g_intVisionIndex, "SOVRP", false, -1/*intTestOption*/);
                        else
                        {
                            m_smTCPIPIO.Send(m_smVisionInfo.g_intVisionIndex, "SOVRP", true, intTestOption);

                            if ((intTestOption & 0x01) > 0)
                                m_blnStartVision_In = true;
                            else
                                m_blnStartVision_In = false;

                            if ((intTestOption & 0x02) > 0)
                                m_blnRetest_In = true;
                            else
                                m_blnRetest_In = false;

                            if ((intTestOption & 0x80) > 0)
                                m_blnCheckPresent_In = true;
                            else
                                m_blnCheckPresent_In = false;

                            if ((intTestOption & 0x100) > 0)
                                m_blnCheckPresentQA_In = true;
                            else
                                m_blnCheckPresentQA_In = false;
                            
                            if ((intTestOption & 0x200) > 0)
                            {
                                //2021-11-19 ZJYEOH : Ignore this due to request from handler, no more using this IO
                                m_blnResetQA_In = false;//true
                                //2021-11-19 ZJYEOH : Ignore start of vision too
                                if (m_blnStartVision_In)
                                    m_blnStartVision_In = false;

                            }
                            else
                                m_blnResetQA_In = false;

                        }
                        break;
                    case "EOV":
                        if (intVisionIndex != Math.Pow(2, m_smVisionInfo.g_intVisionIndex) || m_smVisionInfo.g_intMachineStatus != 2)
                            m_smTCPIPIO.Send(m_smVisionInfo.g_intVisionIndex, "EOVRP", false, intTestOption);
                        else
                        {
                            m_smTCPIPIO.Send(m_smVisionInfo.g_intVisionIndex, "EOVRP", true, intTestOption);
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }
        private void ConvertColorToMono(int intIndex)
        {
            m_smVisionInfo.g_arrColorImages[intIndex].ConvertColorToMono(m_smVisionInfo.g_arrImages[intIndex]);
        }

        /// <summary>
        /// Attach corresponding image to its parent or ROI
        /// </summary>
        private void AttachImageToROI()
        {
            m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

            if ((m_smCustomizeInfo.g_intWantSeal & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                AttachToROI(m_smVisionInfo.g_arrSealROIs, m_smVisionInfo.g_arrImages);
                m_smVisionInfo.g_objCalibrateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
            }

            if (m_smVisionInfo.g_intMachineStatus != 2)
            {
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }

            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.VS_VM_UpdateSmallPictureBox = true;
        }

        /// <summary>
        /// Attach ROI to its parent ROI or parent image
        /// </summary>
        /// <param name="arrROI">ROI</param>
        /// <param name="objImage">parent image</param>
        private void AttachToROI(List<List<ROI>> arrROI, List<ImageDrawing> arrImage)
        {
            ROI objROI;

            for (int i = 0; i < arrROI.Count; i++)
            {
                for (int j = 0; j < arrROI[i].Count; j++)
                {
                    objROI = (ROI)arrROI[i][j];

                    switch (objROI.ref_intType)
                    {
                        case 0:
                        case 1:
                            if (arrImage.Count >= m_smVisionInfo.g_objSeal.GetGrabImageIndex(i))
                                objROI.AttachImage(arrImage[m_smVisionInfo.g_objSeal.GetGrabImageIndex(i)]);
                            else
                                objROI.AttachImage(arrImage[0]);
                            break;
                        case 2:
                            objROI.AttachImage((ROI)arrROI[i][0]);
                            break;
                    }
                    arrROI[i][j] = objROI;
                }
            }

            objROI = null;
        }

        /// <summary>
        /// Trigger alarm if low yield 
        /// </summary>
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

        /// <summary>
        /// Perform production test
        /// </summary>
        /// <returns></returns>
        private void StartTest(bool blnAuto)
        {
            /*
             *  *************** How Post Seal Counter Work (normal method)**************  
             *  Hander Trigger:
             *  > When new unit index come: 
             *  - handler will trigger SOT + Retest IO OFF
             *  - If fail, handler will trigger SOT + Retest IO ON
             *  - If continue fail, handler will remain trigger SOT + Retest IO ON until handler trigger Alarm Post Seal Vision Fail.
             *  > When machine stop and trigger for same unit test:
             *  - hander will trigger SOT + Retest IO ON.
             *  - If fail, handler will remain trigger SOT + Retest IO ON until handler trigger Alarm Post Seal Vision Fail.
             *  IO Senario 1:
             *  > New unit test:
             *    - Retest OFF > Pass > Pass++, Total++   
             *  > New unit test:
             *    - Retest OFF > Fail > Fail++, Total++
             *    - Retest ON  > Fail > no action
             *    - Retest ON  > Pass > Pass++, Fail--
             *  > New unit test:
             *    - Retest OFF > Fail > Fail++, Total++
             *    - Retest ON  > Fail > no action
             *    - Retest ON  > Fail > no action > handler trigger alarm Seal Vision Fail.
             *  > Same unit test:
             *    - Retest ON > Pass >  Pass++, Fail--
             *    .... something wrong 
             *    
             *   *************** How Post Seal Counter Work (Hipertimer calculate machine stop method)**************  
             *  Hander Trigger:
             *  -----------------
             *  > When new unit index come: 
             *  - handler will trigger SOT + Retest IO OFF
             *  - If fail, handler will trigger SOT + Retest IO ON
             *  - If continue fail, handler will remain trigger SOT + Retest IO ON until handler trigger Alarm Post Seal Vision Fail.
             *  > When machine stop and trigger for same unit test:
             *  - hander will trigger SOT + Retest IO ON.
             *  - If fail, handler will remain trigger SOT + Retest IO ON until handler trigger Alarm Post Seal Vision Fail.
             *  IO Senario 1:
             *  ----------------
             *  > New unit test:
             *    - Retest OFF > Pass > Pass++, Total++    (PassCountAdded = true)
             *  > New unit test:
             *    - Retest OFF > Fail > calculate timer... (if less than 1s, handler trigger again, then no action, else, Fail++)
             *    - Retest ON  > Fail > calculate timer... (if less than 1s, handler trigger again, then no action, else, Fail++)
             *    - Retest ON  > Pass > calculate timer... (if less than 1s, handler trigger again, then no action, else, Fail++)
             *  > New unit test:
             *    - Retest OFF > Fail > Fail++, Total++
             *    - Retest ON  > Fail > no action
             *    - Retest ON  > Fail > no action > handler trigger alarm Seal Vision Fail.
             *  > Same unit test:
             *    - Retest ON > Pass >  if (PassCountAdded true) No action, else Pass++
             *  > Same unit test:
             *    - Retest ON > Fail >  calculate timer... (if less than 1s, handler trigger again, then no action, else, Fail++)
             *    - Retest ON  > Fail > calculate timer... (if less than 1s, handler trigger again, then no action, else, Fail++)
             *    - Retest ON  > Pass > calculate timer... (if less than 1s, handler trigger again, then no action, else, Fail++)
             */

            m_strResultTrack = "";
            m_smVisionInfo.g_objProcessTime.Start();

            //if (m_smVisionInfo.g_arrSealROIs.Count == 0 || m_smVisionInfo.g_arrSealROIs[0].Count == 0)
            //{
            //    m_smVisionInfo.g_strErrorMessage += "*Seal : No Template Found";
            //    return;
            //}

            //if (blnAuto && m_smVisionInfo.VM_PR_ByPassUnit)
            //{
            //    m_smVisionInfo.VM_PR_ByPassUnit = false;
            //    m_objVisionIO.IOPass1.SetOn("V6 ");
            //    return;
            //}
            bool blnTotalCountAdded = false;
            bool blnResultOK = true;
            m_blnWantAddFailCount = false;
            m_blnDisplayOrientFail = false;
            m_smVisionInfo.g_blnViewSealObjectsBuilded = false;

            // ------------------ Reset Inspection data ----------------------------------------------
            m_smVisionInfo.g_objSeal.ResetInspectionData();

            if (m_smVisionInfo.g_arrSealROIs.Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage = "*Seal : No Template Found";
                if (blnAuto)
                {
                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                    //m_smVisionInfo.g_intTestedTotal++;
                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                    m_smVisionInfo.g_objSeal.ref_intSealFailMask |= 0x40000;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("g_intTestedTotal++ D= " + m_smVisionInfo.g_intTestedTotal.ToString());
                    }

                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        if (m_intTCPIPResultID == -1)
                            m_intTCPIPResultID = (int)TCPIPResulID.Fail;

                    blnTotalCountAdded = true;
                }
                blnResultOK = false;
            }
            else if (m_smVisionInfo.g_arrSealROIs[0].Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage = "*Seal : No Template Found";
                if (blnAuto)
                {
                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                    //m_smVisionInfo.g_intTestedTotal++;
                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                    m_smVisionInfo.g_objSeal.ref_intSealFailMask |= 0x40000;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("g_intTestedTotal++ = C" + m_smVisionInfo.g_intTestedTotal.ToString());
                    }

                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        if (m_intTCPIPResultID == -1)
                            m_intTCPIPResultID = (int)TCPIPResulID.Fail;

                    blnTotalCountAdded = true;
                }
                //m_smVisionInfo.g_intContinuousFailUnitCount++; //2020-02-21 ZJYEOH : removed this as g_intContinuousFailUnitCount already increased in above "if (blnAuto)" condition
                blnResultOK = false;
            }

            if (blnResultOK)
            {
                if (blnAuto)
                {
                    //Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay); //29-05-2019 ZJYEOH : Moved it to After Grabtime Start counting 

                    if (!m_smProductionInfo.g_blnAllRunWithoutGrabImage)
                        if (!GrabImage(true))
                            return;
                }
                else if (m_smVisionInfo.MN_PR_GrabImage)
                {
                    m_smVisionInfo.MN_PR_GrabImage = false;
                    if (!GrabImage(true))
                        return;
                }
                else
                    AttachImageToROI();
            }
            //float fSealLineAngle = 0;
            //if (!m_smVisionInfo.g_objSeal.GetSealLineAngle(m_smVisionInfo.g_arrSealROIs[2][0], ref fSealLineAngle))   // Temporary
            //{
            //    m_smVisionInfo.g_blnViewRotatedImage = false;
            //    blnResultOK = false;
            //}
            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            if (blnResultOK)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 1.1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }

                //Check position
                if (!CheckPosition())
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 1.2 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        if (m_intTCPIPResultID == -1)
                            m_intTCPIPResultID = (int)TCPIPResulID.FailPosition;

                    blnResultOK = false;
                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objSeal.ref_strErrorMessage;
                }

                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 1.3 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
            }

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 2 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            if (blnResultOK)
            {
                //if (fSealLineAngle != 0)
                //{
                //for (int i = 0; i < m_smVisionInfo.g_arrSealROIs.Count; i++)
                //{
                //    ImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(i)], fSealLineAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);
                //}

                //ImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrImages[0], fSealLineAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);

                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 2.1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }

                AttachToROI(m_smVisionInfo.g_arrSealROIs, m_smVisionInfo.g_arrImages); // AttachToROI(m_smVisionInfo.g_arrSealROIs, m_smVisionInfo.g_arrRotatedImages);

                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 2.2 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }

                //m_smVisionInfo.g_blnViewRotatedImage = true;
                //}
                //else
                //    m_smVisionInfo.g_blnViewRotatedImage = false;
            }

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 3 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            if (blnResultOK)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 3.1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }

                if (m_smVisionInfo.g_blnTrackSealImage)
                {
                    m_smVisionInfo.g_arrSealROIs[3][0].SaveImage("D:\\g_arrSealROIs[3][0]___" + m_smVisionInfo.g_objSeal.GetGrabImageIndex(3).ToString() + ".bmp");
                    m_smVisionInfo.g_arrImages[0].SaveImage("D:\\arrImage0.bmp");
                    m_smVisionInfo.g_arrImages[1].SaveImage("D:\\arrImage1.bmp");

                }


                if (!m_smVisionInfo.g_objSeal.DoInspection(m_smVisionInfo.g_arrSealROIs, m_smVisionInfo.g_arrSealGauges, m_smVisionInfo.g_objSealCircleGauges, m_smVisionInfo.g_fCalibPixelY, m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage))
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 3.2 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        if (m_intTCPIPResultID == -1)
                            m_intTCPIPResultID = (int)TCPIPResulID.Fail;

                    blnResultOK = false;
                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objSeal.ref_strErrorMessage;
                }

                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 3.3 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }
            }

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 4 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            // If CheckPresent on, match with unit mark, fail if no unit in pocket
            // If CheckPresent off, pattern match with empty pocket, fail if unit in pocket

            if (blnResultOK)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 4.1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }

                if (blnAuto && 
                    ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.CheckPresent != null && m_objVisionIO.CheckPresent.IsOn(true)) ||
                    (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnCheckPresent_In)) ||
                    (!blnAuto && !m_smVisionInfo.MN_PR_CheckEmptyUnit))
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 4.2 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x80) > 0 || ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x40) > 0))
                    {
                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("StartTest 4.3 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                        }

                        if (!DoMarkInspection_ForNonOrientationAndOrientationOption())
                        {
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("StartTest 4.4 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                            }

                            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                                if (m_intTCPIPResultID == -1)
                                    m_intTCPIPResultID = (int)TCPIPResulID.Fail;//FailMark;

                            blnResultOK = false;
                            m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objSeal.ref_strErrorMessage;
                        }

                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("StartTest 4.5 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                        }

                    }

                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 4.6 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    //if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMask & 0x80) == 0) //if (m_smVisionInfo.g_objSeal.ref_blnWantSkipOrient)
                    //{
                    //    if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMask & 0x40) > 0)
                    //    {
                    //        if (!CheckUnitPresent())
                    //        {
                    //            blnResultOK = false;
                    //            m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objSeal.ref_strErrorMessage;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (!DoMarkOrientationInspection())
                    //    {
                    //        blnResultOK = false;
                    //        m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objSeal.ref_strErrorMessage;
                    //    }
                    //}
                }
                else
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 4.7 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if (!CheckEmptyPocket())
                    {
                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("StartTest 4.8 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                        }

                        if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                            if (m_intTCPIPResultID == -1)
                                m_intTCPIPResultID = (int)TCPIPResulID.Fail;//FailEmpty;

                        blnResultOK = false;
                        m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objSeal.ref_strErrorMessage;
                    }
                }
            }

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 5 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }


            //if (blnAuto && m_smVisionInfo.VM_PR_ByPassUnit)
            if (m_smVisionInfo.VM_PR_ByPassUnit)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 5.1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }

                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    if (m_intTCPIPResultID != -1)
                        m_intTCPIPResultID = -1;

                m_smVisionInfo.VM_PR_ByPassUnit = false;
                blnResultOK = true;
                //return;
            }
            
            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 6 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            if (blnResultOK)
            {
                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 6.1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }

                if ((blnAuto && ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.CheckPresent != null && m_objVisionIO.CheckPresent.IsOn(true)) ||
                    (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnCheckPresent_In))) ||
                   (!blnAuto && !m_smVisionInfo.MN_PR_CheckEmptyUnit))
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.2 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    m_smVisionInfo.g_strResult = "Pass";
                }
                else
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.3 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    m_smVisionInfo.g_strResult = "Empty";
                }

                if (blnAuto)
                {

                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.4 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.Retest.IsOff(true)) ||
                        (m_smCustomizeInfo.g_blnWantUseTCPIPIO && !m_blnRetest_In))
                    {
                        if (m_smVisionInfo.g_blnTrackSealOption)
                        {
                            STTrackLog.WriteLine("StartTest 6.5 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                        }

                        // 2020 03 13 - JBTAN: empty dont add counter
                        if (((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.CheckPresent != null && m_objVisionIO.CheckPresent.IsOn(true)) ||
                             (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnCheckPresent_In)))
                        {
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("StartTest 6.6 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                            }

                            m_smVisionInfo.g_intPassTotal++;
                            m_smVisionInfo.g_intContinuousPassUnitCount++;
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("g_intPassTotal++ = E" + m_smVisionInfo.g_intPassTotal.ToString());
                            }
                            m_blnPassCountAdded = true;
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("m_blnPassCountAdded = H" + m_blnPassCountAdded.ToString());
                            }

                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("StartTest 6.6a > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                            }

                            m_smVisionInfo.g_intTestedTotal++;
                            m_smVisionInfo.g_intLowYieldUnitCount++;
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("g_intTestedTotal++ 6.6a1= " + m_smVisionInfo.g_intTestedTotal.ToString());
                            }
                        }
                    }
                    else if (!m_blnPassCountAdded)
                    {
                        // 2020 03 13 - JBTAN: empty dont add counter
                        if (((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.CheckPresent != null && m_objVisionIO.CheckPresent.IsOn(true)) ||
                             (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnCheckPresent_In)))
                        {
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("StartTest 6.6b > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                            }

                            m_smVisionInfo.g_intPassTotal++;
                            m_smVisionInfo.g_intContinuousPassUnitCount++;
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("g_intPassTotal++ = F" + m_smVisionInfo.g_intPassTotal.ToString());
                            }
                            m_blnPassCountAdded = true;
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("m_blnPassCountAdded = J" + m_blnPassCountAdded.ToString());
                            }

                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("StartTest 6.6c > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                            }

                            m_smVisionInfo.g_intTestedTotal++;
                            m_smVisionInfo.g_intLowYieldUnitCount++;
                            if (m_smVisionInfo.g_blnTrackSealOption)
                            {
                                STTrackLog.WriteLine("g_intTestedTotal++ 6.6c1= " + m_smVisionInfo.g_intTestedTotal.ToString());
                            }
                        }
                    }

                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.7 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        m_objVisionIO.IOPass1.SetOn("V6 ");
                    else
                    {
                        //2021-03-24 ZJYEOH : Wait at least 5ms before send result
                        float fDelay = 5 - Math.Max(0, m_smVisionInfo.g_objTotalTime.Timing - m_smVisionInfo.g_objGrabTime.Duration);
                        if (fDelay >= 1)
                            Thread.Sleep((int)fDelay);

                        m_blnPass1_Out = true;
                        m_smTCPIPIO.Send_Result(m_smVisionInfo.g_intVisionIndex, true, m_blnOrientResult1_Out, m_blnOrientResult2_Out, m_intTCPIPResultID);
                    }
                    SavePassImage_AddToBuffer();

                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.8 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                }
                else
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.9 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                    m_smVisionInfo.g_strErrorMessage = "Offline Test Pass!";
                }
            }
            else
            {
                // 2020 10 01 - CCENG: When retest is OFf, mean it is new index unit. and when unit fail, mean Pass Count is not added yet.
                if ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.Retest.IsOff(true)) ||
                    (m_smCustomizeInfo.g_blnWantUseTCPIPIO && !m_blnRetest_In))
                {
                    m_blnPassCountAdded = false;

                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("m_blnPassCountAdded = G" + m_blnPassCountAdded.ToString());
                    }
                }

                if (m_smVisionInfo.g_blnTrackSealOption)
                {
                    STTrackLog.WriteLine("StartTest 6.10 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                }

                if (m_blnDisplayOrientFail)
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.11 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    switch (m_smVisionInfo.g_intOrientResult[0])
                    {
                        case 1:
                            m_smVisionInfo.g_strResult = "90";
                            //if (m_smCustomizeInfo.g_intOrientIO == 0)
                            //{
                            //    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                            //    {
                            //        m_blnOrientResult1_Out = false;
                            //        m_blnOrientResult2_Out = true;
                            //    }
                            //}
                            //else
                            //{
                            //    m_blnOrientResult1_Out = true;
                            //    m_blnOrientResult2_Out = false;
                            //}
                            //2021-12-15 ZJYEOH : Always set to false, because handler that side cant accept other than 0 degree
                            m_blnOrientResult1_Out = false;
                            m_blnOrientResult2_Out = false;
                            break;
                        case 2:
                            m_smVisionInfo.g_strResult = "180";
                            //if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                            //{
                            //    m_blnOrientResult1_Out = true;
                            //    m_blnOrientResult2_Out = true;
                            //}
                            //2021-12-15 ZJYEOH : Always set to false, because handler that side cant accept other than 0 degree
                            m_blnOrientResult1_Out = false;
                            m_blnOrientResult2_Out = false;
                            break;
                        case 3:
                            m_smVisionInfo.g_strResult = "-90";
                            //if (m_smCustomizeInfo.g_intOrientIO == 0)
                            //{
                            //    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                            //    {
                            //        m_blnOrientResult1_Out = true;
                            //        m_blnOrientResult2_Out = false;
                            //    }
                            //}
                            //else
                            //{
                            //    m_blnOrientResult1_Out = false;
                            //    m_blnOrientResult2_Out = true;
                            //}
                            //2021-12-15 ZJYEOH : Always set to false, because handler that side cant accept other than 0 degree
                            m_blnOrientResult1_Out = false;
                            m_blnOrientResult2_Out = false;
                            break;
                        default:
                            m_smVisionInfo.g_strResult = "Fail";
                            //2021-12-15 ZJYEOH : Always set to false, because handler that side cant accept other than 0 degree
                            m_blnOrientResult1_Out = false;
                            m_blnOrientResult2_Out = false;
                            break;

                    }
                }
                else
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.12 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    m_smVisionInfo.g_strResult = "Fail";
                }

                if (m_smVisionInfo.g_objSeal.ref_strErrorMessage != "")
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.13 > g_strErrorMessage =" + m_smVisionInfo.g_objSeal.ref_strErrorMessage);
                    }

                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objSeal.ref_strErrorMessage;
                }

                if (blnAuto)
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.14 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if (((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.CheckPresent != null && m_objVisionIO.CheckPresent.IsOn(true)) ||
                           (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnCheckPresent_In)))
                    {
                        m_blnWantAddFailCount = true;
                    }

                    //if (m_objVisionIO.Retest.IsOff(true))
                    //{
                    //    if (m_smVisionInfo.g_blnTrackSealOption)
                    //    {
                    //        STTrackLog.WriteLine("StartTest 6.15 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    //    }

                    //    // 2020 03 13 - JBTAN: empty dont add counter
                    //    if (m_objVisionIO.CheckPresent != null && m_objVisionIO.CheckPresent.IsOn(true))
                    //    {
                    //        if (m_smVisionInfo.g_blnTrackSealOption)
                    //        {
                    //            STTrackLog.WriteLine("StartTest 6.16 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    //        }


                    //        switch (m_smVisionInfo.g_objSeal.ref_intSealFailMask)
                    //        {
                    //            case 0x01:  // Fail position, position pattern no found. 
                    //                m_smVisionInfo.g_intPositionFailureTotal++;
                    //                break;
                    //            //seal distance
                    //            case 0x02:
                    //                m_smVisionInfo.g_intSealDistanceFailureTotal++;
                    //                break;
                    //            //seal bubble
                    //            case 0x04:
                    //                m_smVisionInfo.g_intSealBrokenAreaFailureTotal++;
                    //                break;
                    //            //seal broken
                    //            case 0x08:
                    //                m_smVisionInfo.g_intSealBrokenGapFailureTotal++;
                    //                break;
                    //            //seal width
                    //            //seal width insufficient
                    //            case 0x10:    // 16
                    //            case 0x20:    //32
                    //                m_smVisionInfo.g_intSealFailureTotal++;
                    //                break;
                    //            //seal overheat
                    //            //Tape Scratches
                    //            case 0x40:    // 64 
                    //            case 0x800:
                    //                m_smVisionInfo.g_intSealOverHeatFailureTotal++;
                    //                break;
                    //            //sprocket hole distance
                    //            case 0x200:   //  512
                    //                m_smVisionInfo.g_intSealSprocketHoleFailureTotal++;
                    //                break;
                    //            case 0x080:             // Unit Present using pattern matching or angle fail.
                    //            case 0x400:             // Unit Present using white black pixel count
                    //            case 0x480:  //1152     // Unit Present using white black pixel count
                    //                m_smVisionInfo.g_intCheckPresenceFailureTotal++;
                    //                break;
                    //            case 0x1000:
                    //                m_smVisionInfo.g_intOrientFailureTotal++;
                    //                break;
                    //            default:
                    //                SRMMessageBox.Show("Vision6Process > StartTest > m_smVisionInfo.g_objSeal.ref_intSealFailMask value " + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString() + " no exist in Switch Case");
                    //                break;
                    //        }

                    //        m_smVisionInfo.g_intContinuousFailUnitCount++;
                    //    }
                    //}

                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.17 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    {
                        //2021-03-24 ZJYEOH : Wait at least 5ms before send result
                        float fDelay = 5 - Math.Max(0, m_smVisionInfo.g_objTotalTime.Timing - m_smVisionInfo.g_objGrabTime.Duration);
                        if (fDelay >= 1)
                            Thread.Sleep((int)fDelay);

                        m_smTCPIPIO.Send_Result(m_smVisionInfo.g_intVisionIndex, false, m_blnOrientResult1_Out, m_blnOrientResult2_Out, m_intTCPIPResultID);
                    }

                    SaveRejectImage_AddToBuffer("Seal", m_smVisionInfo.g_objSeal.ref_strErrorMessage);
                }
                else
                {
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("StartTest 6.18 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
                    }

                    if (m_smVisionInfo.g_objSeal.ref_strErrorMessage != "")
                        m_smVisionInfo.g_strErrorMessage = "Offline Test Fail! *" + m_smVisionInfo.g_objSeal.ref_strErrorMessage;
                }
            }

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 7 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }


            //if (blnAuto)
            //{
            //    if (m_smVisionInfo.g_blnTrackSealOption)
            //    {
            //        STTrackLog.WriteLine("StartTest 7.1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            //    }

            //    if (m_objVisionIO.Retest.IsOff(true))
            //    {
            //        if (m_smVisionInfo.g_blnTrackSealOption)
            //        {
            //            STTrackLog.WriteLine("StartTest 7.2 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            //        }

            //        //if (!blnTotalCountAdded)
            //        {
            //            if (m_smVisionInfo.g_blnTrackSealOption)
            //            {
            //                STTrackLog.WriteLine("StartTest 7.3 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            //            }

            //            // 2020 03 13 - JBTAN: empty dont add counter
            //            if (m_objVisionIO.CheckPresent != null && m_objVisionIO.CheckPresent.IsOn(true))
            //            {
            //                if (m_smVisionInfo.g_blnTrackSealOption)
            //                {
            //                    STTrackLog.WriteLine("StartTest 7.4 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            //                }

            //                if (blnResultOK)    // 2020 10 02 - CCENG: Here for pass only. For fail, the g_intTestedTotal will ++ in AddFailCounter()
            //                {
            //                    if (m_smVisionInfo.g_blnTrackSealOption)
            //                    {
            //                        STTrackLog.WriteLine("StartTest 7.5 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            //                    }

            //                    m_smVisionInfo.g_intTestedTotal++;
            //                    m_smVisionInfo.g_intLowYieldUnitCount++;
            //                    if (m_smVisionInfo.g_blnTrackSealOption)
            //                    {
            //                        STTrackLog.WriteLine("g_intTestedTotal++ B= " + m_smVisionInfo.g_intTestedTotal.ToString());
            //                    }

            //                }
            //            }
            //        }
            //    }
            //}

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 8 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            m_smVisionInfo.g_objProcessTime.Stop();

            //m_strResultTrack += m_smVisionInfo.g_objSeal.m_strTrack;
            //TrackLog objTL = new TrackLog();
            //objTL.WriteLine(m_strResultTrack);

            // No rotate image after inspection. Always display arrIamge.
            for (int i = 0; i < m_smVisionInfo.g_arrblnImageRotated.Length; i++)
            {
                if (m_smVisionInfo.g_arrblnImageRotated[i])
                    m_smVisionInfo.g_arrblnImageRotated[i] = false;
            }

            if (!m_smVisionInfo.AT_VM_OfflineTestForSetting) // No need to draw or update quantity when doing offline test for setting.
            {
                m_smVisionInfo.VS_AT_UpdateQuantity = true;
                m_smVisionInfo.PR_VM_UpdateQuantity = true;
                m_smVisionInfo.g_blnViewSealObjectsBuilded = true;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            CheckLowYield();
            CheckContinuousPass();
            CheckContinuousFail();

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("StartTest 9 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString() + ", ref_intSealFailMask=" + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString());
            }

            return;
        }

        private bool CheckEmptyPocket()
        {
            if (m_smVisionInfo.g_arrSealROIs[5].Count > 0)
                m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)]);

            for (int i = 0; i < 4; i++)
            {
                if ((m_smVisionInfo.g_intPocketTemplateMask & (0x01 << i)) <= 0)
                    continue;

                if (m_smVisionInfo.g_objSeal.DoEmptyPocketInspection(m_smVisionInfo.g_arrSealROIs[5][0], 0, i))
                {
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = i;
                    return true;
                }
                else
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = i;

            }

            return false;
        }

        private bool CheckNoEmptyPocket()
        {
            if (m_smVisionInfo.g_arrSealROIs[5].Count > 0)
                m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)]);

            for (int i = 0; i < 4; i++)
            {
                if ((m_smVisionInfo.g_intPocketTemplateMask & (0x01 << i)) <= 0)
                    continue;

                if (m_smVisionInfo.g_objSeal.DoNoEmptyPocketInspection(m_smVisionInfo.g_arrSealROIs[5][0], 0, i))
                {
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = i;
                    return true;
                }
                else
                    m_smVisionInfo.g_objSeal.ref_intPocketTemplateIndex = i;

            }

            return false;
        }

        private bool DoMarkInspection_ForNonOrientationAndOrientationOption()
        {
            bool bWantOrientation = ((m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal & 0x80) > 0);
            /*
             * Question why need to have Orientation Option but not always check the orientation?
             * - if customer confirm wont have orientation mistake in tape, then no need to turn ON the Orientation option.
             * - Turnning ON the Orientation option may detect the Pass unit as wrong orientation especially when mark is not very clear.
             */


            m_smVisionInfo.g_objSeal.ResetMarkInspectionData();

            if (m_smVisionInfo.g_arrSealROIs[5].Count > 0)
                m_smVisionInfo.g_arrSealROIs[5][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(5)]);

            if (m_smVisionInfo.g_arrSealROIs[5].Count <= 2)
            {
                m_smVisionInfo.g_objSeal.ref_strErrorMessage = "Fail Unit Mark Not Learnt";
                m_smVisionInfo.g_objSeal.ref_intSealFailMask |= 0x80;
                return false;
            }

            m_smVisionInfo.g_arrSealROIs[5][2].AttachImage(m_smVisionInfo.g_arrSealROIs[5][0]);
            m_smVisionInfo.g_intOrientResult[0] = 4;  // 0:0deg, 1:90deg, 2:180deg, 3:-90, 4:Fail

            //2020-10-09 ZJYEOH : Pocket Pitch larger than 4mm will use final reduction = 2 because larger unit comsume more time
            int intFinalReduction = 0;
            if (m_smVisionInfo.g_objSeal.ref_intTapePocketPitch > 4)
                intFinalReduction = 2;

            bool blnMarkScoreResult = m_smVisionInfo.g_objSeal.DoSealOrientationInspection_MultiTemplate(m_smVisionInfo.g_arrSealROIs[5][0],
                                                                               m_smVisionInfo.g_arrSealROIs[5][2],
                                                                               intFinalReduction,//0,
                                                                               m_smVisionInfo.g_intMarkTemplateMask,
                                                                               bWantOrientation);

            m_smVisionInfo.g_intOrientResult[0] = m_smVisionInfo.g_objSeal.ref_intAngleResult;

            if (m_smVisionInfo.g_intOrientResult[0] != 0)  // wrong orientation
            {
                // If mark score fail, then display "mark score fail". Dont display wrong orientation. 
                // Bcos if it is wrong mark, not wrong orient, but you display wrong orient msg, then will feel weird.
                if (blnMarkScoreResult && m_smVisionInfo.g_intOrientResult[0] != 4)
                {
                    m_blnDisplayOrientFail = true;
                }
                return false;
            }

            if (blnMarkScoreResult)
            {
                if (m_smVisionInfo.g_objSeal.DoUnitPresentInspection_PixelQuantity(m_smVisionInfo.g_arrSealROIs[5][0], m_smVisionInfo.g_objSeal.ref_intMarkTemplateIndex))
                {
                }
                else
                {
                    blnMarkScoreResult = false;
                }
            }

            return blnMarkScoreResult;

        }

        private bool CheckPosition()
        {
            // 2020-01-20 ZJYEOH : Will Always Check Position Pattern
            //if ((m_smVisionInfo.g_objSeal.ref_intFailOptionMask & 0x04) == 0)
            //    return true;

            if (m_smVisionInfo.g_arrSealROIs[0].Count > 0)
                m_smVisionInfo.g_arrSealROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_objSeal.GetGrabImageIndex(0)]);
            else
                return false;

            if (m_smVisionInfo.g_objSeal.DoPositionInspection_FindNearestToTemplateSprocketHolePosition(m_smVisionInfo.g_arrSealROIs[0][0], 1, 0, m_smVisionInfo.g_fCalibPixelX, false))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Save different category of reject image
        /// </summary>
        /// <param name="strModule">Reject Image category name</param>
        private void SaveRejectImage(string strModule)
        {
            if (m_smCustomizeInfo.g_blnSaveFailImage)
            {
                string strPath = m_smVisionInfo.g_strSaveImageLocation +
                     m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                     "\\" + m_smVisionInfo.g_strVisionFolderName +
                     "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" +
                     "\\" + strModule + "\\";

                if (m_smCustomizeInfo.g_intSaveImageMode == 0)
                {
                    if (m_smVisionInfo.g_intFailImageCount >= m_smCustomizeInfo.g_intFailImagePics)
                        return;
                }
                else
                {
                    if (m_smVisionInfo.g_intFailImageCount >= m_smCustomizeInfo.g_intFailImagePics)
                        m_smVisionInfo.g_intFailImageCount = 0;
                }

                if ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.Retest.IsOn()) ||
                    (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnRetest_In))
                {
                    m_smVisionInfo.g_intFailImageCount--;
                    m_intRetryCount++;
                }
                else
                {
                    if (m_intRetryCount > 0)
                        m_intRetryCount = 0;
                }

                if (m_smVisionInfo.g_blnViewColorImage)
                    m_smVisionInfo.g_arrColorImages[0].SaveImage(strPath + m_smVisionInfo.g_intFailImageCount + "_" + m_intRetryCount + ".bmp");
                else
                    m_smVisionInfo.g_arrImages[0].SaveImage(strPath + m_smVisionInfo.g_intFailImageCount + "_" + m_intRetryCount + ".bmp");

                for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[i].SaveImage(strPath + m_smVisionInfo.g_intFailImageCount + "_Image" + i.ToString() + "_" + m_intRetryCount + ".bmp");
                    else
                        m_smVisionInfo.g_arrImages[i].SaveImage(strPath + m_smVisionInfo.g_intFailImageCount + "_Image" + i.ToString() + "_" + m_intRetryCount + ".bmp");
                }

                m_smVisionInfo.g_strLastImageFolder = strPath;
                m_smVisionInfo.g_strLastImageName = m_smVisionInfo.g_intFailImageCount + "_" + m_intRetryCount + ".bmp";

                m_smVisionInfo.g_intFailImageCount++;
            }
        }

        private void SaveRejectImage_AddToBuffer(string strRejectName, string strRejectMessage)
        {
            if (m_smCustomizeInfo.g_blnSaveFailImage)
            {
                if (m_smCustomizeInfo.g_intSaveImageMode == 0)
                {
                    if (m_smVisionInfo.g_intFailImageCount >= m_smCustomizeInfo.g_intFailImagePics)
                        return;
                }

               if ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.Retest.IsOn()) ||
                    (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnRetest_In))
                {
                    m_smVisionInfo.g_intFailImageCount--;
                    m_intRetryCount++;
                }
                else
                {
                    if (m_intRetryCount > 0)
                        m_intRetryCount = 0;
                }

                WaitImageBufferClear(ref m_intFailStartNode, ref m_intFailEndNode);

                if (m_smVisionInfo.g_blnViewColorImage)
                    m_smVisionInfo.g_arrColorImages[0].CopyTo(ref m_arrFailCImage1Buffer[m_intFailEndNode]);
                else
                    m_smVisionInfo.g_arrImages[0].CopyTo(ref m_arrFailImage1Buffer[m_intFailEndNode]);

                if (m_smVisionInfo.g_arrImages.Count > 1)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[1].CopyTo(ref m_arrFailCImage2Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[1].CopyTo(ref m_arrFailImage2Buffer[m_intFailEndNode]);
                }

                if (m_smVisionInfo.g_arrImages.Count > 2)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[2].CopyTo(ref m_arrFailCImage3Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[2].CopyTo(ref m_arrFailImage3Buffer[m_intFailEndNode]);
                }

                if (m_smVisionInfo.g_arrImages.Count > 3)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[3].CopyTo(ref m_arrFailCImage4Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[3].CopyTo(ref m_arrFailImage4Buffer[m_intFailEndNode]);
                }

                if (m_smVisionInfo.g_arrImages.Count > 4)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[4].CopyTo(ref m_arrFailCImage5Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[4].CopyTo(ref m_arrFailImage5Buffer[m_intFailEndNode]);
                }

                if (m_smVisionInfo.g_arrImages.Count > 5)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[5].CopyTo(ref m_arrFailCImage6Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[5].CopyTo(ref m_arrFailImage6Buffer[m_intFailEndNode]);
                }

                if (m_smVisionInfo.g_arrImages.Count > 6)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[6].CopyTo(ref m_arrFailCImage7Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[6].CopyTo(ref m_arrFailImage7Buffer[m_intFailEndNode]);
                }

                m_arrRejectNameBuffer[m_intFailEndNode] = strRejectName;
                m_arrRejectMessageBuffer[m_intFailEndNode] = strRejectMessage;
                //m_arrFailNoBuffer[m_intFailEndNode] = m_smVisionInfo.g_intFailImageCount;
                m_arrFailNoBuffer[m_intFailEndNode] = m_smVisionInfo.g_intTotalImageCount;  // 2019 09 18 - CCENG: Use total image count instead of Fail Image count so that pass fail image will display in sequence.
                m_smVisionInfo.g_intTotalImageCount++;
                m_smVisionInfo.g_intFailImageCount++;

                m_intFailEndNode++;
                if (m_smVisionInfo.g_blnViewColorImage)
                {
                    if (m_intFailEndNode == m_smVisionInfo.g_intSaveImageBufferSize)
                        m_intFailEndNode = 0;
                }
                else
                {
                    if (m_intFailEndNode == m_smVisionInfo.g_intSaveImageBufferSize)
                        m_intFailEndNode = 0;
                }

                if (m_intFailEndNode == m_intFailStartNode)
                {
                    SRMMessageBox.Show("Fail Image buffer is full.");
                }
            }
        }

        private void SavePassImage()
        {
            if (m_smCustomizeInfo.g_blnSavePassImage)
            {
                if (m_smVisionInfo.g_intPassImageCount < m_smCustomizeInfo.g_intPassImagePics)
                {
                    string strPath = m_smVisionInfo.g_strSaveImageLocation +
                        m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                        "\\" + m_smVisionInfo.g_strVisionFolderName +
                        "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" +
                        "\\Pass\\";

                    if ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.Retest.IsOn()) ||
                   (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnRetest_In))
                    {
                        m_smVisionInfo.g_intPassImageCount--;
                        m_intRetryCount++;
                    }
                    else
                    {
                        if (m_intRetryCount > 0)
                            m_intRetryCount = 0;
                    }

                    m_smVisionInfo.g_arrImages[0].SaveImage(strPath + m_smVisionInfo.g_intPassImageCount + "_" + m_intRetryCount + ".bmp");

                    m_smVisionInfo.g_strLastImageFolder = strPath;
                    m_smVisionInfo.g_strLastImageName = m_smVisionInfo.g_intPassImageCount + "_" + m_intRetryCount + ".bmp";

                    m_smVisionInfo.g_intPassImageCount++;
                }
            }
        }

        private void SavePassImage_AddToBuffer()
        {
            if (m_smCustomizeInfo.g_blnSavePassImage)
            {
                if (m_smVisionInfo.g_intPassImageCount < m_smCustomizeInfo.g_intPassImagePics)
                {
                    if ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.Retest.IsOn()) ||
                    (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnRetest_In))
                    {
                        m_smVisionInfo.g_intPassImageCount--;
                        m_intRetryCount++;
                    }
                    else
                    {
                        if (m_intRetryCount > 0)
                            m_intRetryCount = 0;
                    }

                    WaitImageBufferClear(ref m_intPassStartNode, ref m_intPassEndNode);

                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[0].CopyTo(ref m_arrPassCImage1Buffer[m_intPassEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[0].CopyTo(ref m_arrPassImage1Buffer[m_intPassEndNode]);

                    if (m_smVisionInfo.g_arrImages.Count > 1)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[1].CopyTo(ref m_arrPassCImage2Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[1].CopyTo(ref m_arrPassImage2Buffer[m_intPassEndNode]);
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 2)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[2].CopyTo(ref m_arrPassCImage3Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[2].CopyTo(ref m_arrPassImage3Buffer[m_intPassEndNode]);
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 3)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[3].CopyTo(ref m_arrPassCImage4Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[3].CopyTo(ref m_arrPassImage4Buffer[m_intPassEndNode]);
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 4)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[4].CopyTo(ref m_arrPassCImage5Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[4].CopyTo(ref m_arrPassImage5Buffer[m_intPassEndNode]);
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 5)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[5].CopyTo(ref m_arrPassCImage6Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[5].CopyTo(ref m_arrPassImage6Buffer[m_intPassEndNode]);
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 6)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[6].CopyTo(ref m_arrPassCImage7Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[6].CopyTo(ref m_arrPassImage7Buffer[m_intPassEndNode]);
                    }

                    //m_arrPassNoBuffer[m_intPassEndNode] = m_smVisionInfo.g_intPassImageCount;
                    m_arrPassNoBuffer[m_intPassEndNode] = m_smVisionInfo.g_intTotalImageCount;  // 2019 09 18 - CCENG: Use total image count instead of Pass Image count so that pass fail image will display in sequence.
                    m_smVisionInfo.g_intTotalImageCount++;
                    m_smVisionInfo.g_intPassImageCount++;

                    m_intPassEndNode++;

                    if (m_intPassEndNode == m_smVisionInfo.g_intSaveImageBufferSize)
                        m_intPassEndNode = 0;
                }
            }
        }

        /// <summary>
        /// Stop the grab timer indicate grab action has been done
        /// </summary>
        private void SetGrabDone()
        {
            m_smVisionInfo.g_objGrabTime.Stop();
        }

        /// <summary>
        /// Consistenly check the timer to trigger specific action 
        /// </summary>
        private void UpdateProgress()
        {
            try
            {
                while (!m_blnStopping)
                {
                    if (!m_blnPause)
                    {
                        //Grab image
                        if (m_smVisionInfo.MN_PR_GrabImage || m_smVisionInfo.AT_PR_GrabImage)
                        {
                            m_smVisionInfo.g_blnGrabbing = true;
                            if (m_smVisionInfo.AT_PR_GrabImage) // 01-08-2019 ZJYEOH : Only clear drawing result when user pressed grab button, solved "grab before test" no drawings 
                                m_smVisionInfo.g_blnClearResult = true;

                            GrabImage(false);

                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                            m_smVisionInfo.VS_AT_UpdateQuantity = true;
                            m_smVisionInfo.g_blnGrabbing = false;
                            m_smVisionInfo.MN_PR_GrabImage = false;
                            m_smVisionInfo.AT_PR_GrabImage = false;
                        }

                        //Start live image
                        if (m_smVisionInfo.AT_PR_StartLiveImage && !m_smVisionInfo.AT_PR_PauseLiveImage)
                        {
                            m_smVisionInfo.g_blnGrabbing = true;
                            m_smVisionInfo.g_blnClearResult = true;

                            GrabImage(false);

                            m_smVisionInfo.VS_AT_UpdateQuantity = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            m_smVisionInfo.g_blnGrabbing = false;
                            System.Threading.Thread.Sleep(50);
                        }

                        //Attach each ROI to its parent
                        if (m_smVisionInfo.AT_PR_AttachImagetoROI)
                        {
                            AttachImageToROI();
                            m_smVisionInfo.AT_PR_AttachImagetoROI = false;
                        }

                        if (m_smVisionInfo.CO_PR_DeleteTemplate)
                        {
                            m_smVisionInfo.CO_PR_DeleteTemplate = false;

                            ////For bottom orient 
                            //if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            //{
                            //    if (m_smVisionInfo.g_blnWantClearMarkTemplateWhenNewLot)
                            //        m_smVisionInfo.PR_CO_DeleteProcessSuccess = DeleteTemplate();
                            //    else
                            //        m_smVisionInfo.PR_CO_DeleteProcessSuccess = true;
                            //}
                            //else
                            if (m_smVisionInfo.g_blnWantClearSealTemplateWhenNewLot)
                                m_smVisionInfo.PR_CO_DeleteProcessSuccess = DeleteTemplate();
                            else
                                m_smVisionInfo.PR_CO_DeleteProcessSuccess = true;

                            //m_smVisionInfo.PR_CO_DeleteProcessSuccess = true; // 2020-07-22 ZJYEOH : Simply Send delete success because Seal dont have new lot clear template function

                            m_smVisionInfo.PR_CO_DeleteTemplateDone = true;
                        }

                        //Perform manual post seal inspection
                        if (m_smVisionInfo.MN_PR_StartTest && !m_smVisionInfo.AT_PR_AttachImagetoROI)
                        {
                            m_smVisionInfo.g_objTotalTime.Start();
                            m_smVisionInfo.MN_PR_StartTest = false;

                            StartTest(false);

                            m_smVisionInfo.g_objTotalTime.Stop();
                            m_smVisionInfo.g_blnViewGauge = true;
                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                            m_smVisionInfo.PR_MN_TestDone = true;
                            m_smVisionInfo.VM_AT_BlockImageUpdate = false;
                            if (!m_smVisionInfo.AT_VM_OfflineTestForSetting) // No need to draw or update quantity when doing offline test for setting.
                                m_smVisionInfo.PR_MN_UpdateInfo = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;

                            if (m_smVisionInfo.AT_VM_OfflineTestForSetting)
                                m_smVisionInfo.AT_VM_OfflineTestForSetting = false;
                        }

                        //Perform production test
                        if ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.IOStartVision.IsOn() && m_smVisionInfo.g_intMachineStatus == 2) ||
                            (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnStartVision_In && m_smVisionInfo.g_intMachineStatus == 2) ||
                            (m_smVisionInfo.g_blnDebugRUN && m_smVisionInfo.g_intMachineStatus == 2))
                        {
                            m_objStopMachineTiming.Stop();

                            if (m_smVisionInfo.g_blnDebugRUN && (m_smProductionInfo.g_blnAllRunWithoutGrabImage || m_smProductionInfo.g_blnAllRunGrabWithoutUseImage) && (m_smProductionInfo.g_intDebugImageToUse == 1))
                            {
                                LoadNextImageForDebugRunTest();
                            }

                            m_smVisionInfo.g_objTotalTime.Start();
                            m_smVisionInfo.VS_AT_ProductionTestDone = false;

                            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                            {
                                m_blnStartVision_In = false;
                                m_blnEndVision_Out = false;
                                m_blnPass1_Out = false;
                                m_intTCPIPResultID = -1;

                            }
                            else
                            {
                                m_objVisionIO.IOEndVision.SetOff("V6 ");
                                m_objVisionIO.IOPass1.SetOff("V6 ");
                            }
                            //STTrackLog.WriteLine(m_smVisionInfo.g_strVisionName + "------------ Start");

                            StartTest(true);
                            //STTrackLog.WriteLine(m_smVisionInfo.g_strVisionName + "------------ End");

                            if (!m_blnForceStopProduction)
                            {
                                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                                {
                                    m_blnEndVision_Out = true;
                                }
                                else
                                    m_objVisionIO.IOEndVision.SetOn("V6 ");
                            }
                            else
                            {
                                STTrackLog.WriteLine("Vision6Process > Force Stop Production");
                                m_blnForceStopProduction = false;
                                m_smVisionInfo.g_intMachineStatus = 1;
                            }

                            m_smVisionInfo.g_objTotalTime.Stop();

                            m_smProductionInfo.VM_TH_UpdateCount = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;

                            m_smVisionInfo.VS_AT_ProductionTestDone = true;

                            if (m_smVisionInfo.g_blnDebugRUN)
                                Thread.Sleep(m_smVisionInfo.g_intSleep);

                            if (m_blnWantAddFailCount)
                                m_objStopMachineTiming.Start();

                            if (m_smProductionInfo.g_blnAllRunFromCenter)
                                m_smVisionInfo.g_blnDebugRUN = false;
                        }

                        // Consider machine stop after 
                        if (m_blnWantAddFailCount && m_objStopMachineTiming.Timing > 1000)
                        {
                            m_blnWantAddFailCount = false;
                            m_objStopMachineTiming.Stop();

                            AddFailCounter();
                        }

                        if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        {
                            if (m_blnEndVision_Out == true && m_smVisionInfo.g_intMachineStatus != 2)
                                m_blnEndVision_Out = false;
                            else if (m_blnEndVision_Out == false && m_smVisionInfo.g_intMachineStatus == 2)
                                m_blnEndVision_Out = true;
                        }
                        else
                        {
                            if (m_objVisionIO.IOEndVision.IsOn() && m_smVisionInfo.g_intMachineStatus != 2)
                                m_objVisionIO.IOEndVision.SetOff("V6 ");
                            else if (m_objVisionIO.IOEndVision.IsOff() && m_smVisionInfo.g_intMachineStatus == 2)
                                m_objVisionIO.IOEndVision.SetOn("V6 ");
                        }

                        if (m_smVisionInfo.g_blnWantClearSaveImageInfo)
                        {
                            m_arrPassNoBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
                            m_arrFailNoBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
                            m_arrRejectNameBuffer = new string[m_smVisionInfo.g_intSaveImageBufferSize];
                            m_arrRejectMessageBuffer = new string[m_smVisionInfo.g_intSaveImageBufferSize];

                            m_arrRejectImageListPath.Clear();
                            m_arrRejectImageErrorMessageListPath.Clear();

                            m_smVisionInfo.g_blnWantClearSaveImageInfo = false;
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Vision6Process->UpdateProgress() :" + ex.ToString());
                SRMMessageBox.Show("Vision6Process has been terminated. Please Exit SRMVision software and Run again!", "Vision6Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_smVisionInfo.g_strCameraModel == "AVT")
                    m_objAVTFireGrab.OFFCamera();
                else if (m_smVisionInfo.g_strCameraModel == "Teli")
                {
                    if (m_smVisionInfo.g_intGrabMode == 0)
                        m_objTeliCamera.OFFCamera();
                    else
                        m_objTeliCamera.OFFCamera_LowLevelAPI();
                }
                m_thThread = null;
                m_blnStopped = true;

            }
        }

        private void WaitImageBufferClear(ref int intStartNode, ref int intEndNode)
        {
            while (true)
            {
                int intNextEndNode = intEndNode + 1;
                if (intNextEndNode >= m_smVisionInfo.g_intSaveImageBufferSize)
                    intNextEndNode = 0;

                if (intNextEndNode != intStartNode)
                {
                    return;
                }

                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }
        }

        private void LoadRejectImageListPath()
        {
            //string strRejectImageFolderPath = m_smVisionInfo.g_strSaveImageLocation +
            //               m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
            //               "\\" + m_smVisionInfo.g_strVisionFolderName +
            //               "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")";

            // 2020 03 27 - JBTAN: Load from different folder if reset count
            string strRejectImageFolderPath;
            if (m_smVisionInfo.g_intVisionResetCount == 0)
            {
                strRejectImageFolderPath = m_smVisionInfo.g_strSaveImageLocation +
                    m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                    "\\" + m_smVisionInfo.g_strVisionFolderName +
                    "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;
            }
            else
            {
                strRejectImageFolderPath = m_smVisionInfo.g_strSaveImageLocation +
                    m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                    "\\" + m_smVisionInfo.g_strVisionFolderName +
                    "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smVisionInfo.g_strVisionResetCountTime;
            }

            string[] arrRejectImageFoldderList = Directory.GetDirectories(strRejectImageFolderPath);

            List<int> arrRejectImageNo = new List<int>();
            m_arrRejectImageListPath.Clear();
            m_arrRejectImageErrorMessageListPath.Clear();
            foreach (string strFolderPath in arrRejectImageFoldderList)
            {
                //2021-02-24 ZJYHEOH : No need to delete pass image
                if (strFolderPath.Substring(strFolderPath.LastIndexOf('\\') + 1, strFolderPath.Length - strFolderPath.LastIndexOf('\\') - 1) == "Pass")
                    continue;

                string[] arrRejectImageList = Directory.GetFiles(strFolderPath);

                foreach (string strFilePath in arrRejectImageList)
                {
                    string strFileName = Path.GetFileNameWithoutExtension(strFilePath);

                    if (strFileName.LastIndexOf("_Image") > 0)
                    {
                        continue;
                    }

                    int intFileNo = 0;
                    if (int.TryParse(new string(strFileName
                     .SkipWhile(x => !char.IsDigit(x))
                     .TakeWhile(x => char.IsDigit(x))
                     .ToArray()), out intFileNo))
                    {
                        int intSelectedIndex = arrRejectImageNo.Count;
                        for (int i = 0; i < arrRejectImageNo.Count; i++)
                        {
                            if (intFileNo < arrRejectImageNo[i])
                            {
                                intSelectedIndex = i;
                                break;
                            }

                        }

                        m_arrRejectImageListPath.Insert(intSelectedIndex, strFilePath);
                        arrRejectImageNo.Insert(intSelectedIndex, intFileNo);

                        //for error message xml file
                        string strErrorMessagePath = strFilePath.Replace(".bmp", ".xml");
                        if (File.Exists(strErrorMessagePath))
                        {
                            if (intSelectedIndex > m_arrRejectImageErrorMessageListPath.Count)
                            {
                                if (m_arrRejectImageErrorMessageListPath.Count == 0)
                                    m_arrRejectImageErrorMessageListPath.Insert(0, strErrorMessagePath);
                                else
                                    m_arrRejectImageErrorMessageListPath.Insert(m_arrRejectImageErrorMessageListPath.Count - 1, strErrorMessagePath);
                            }
                            else
                                m_arrRejectImageErrorMessageListPath.Insert(intSelectedIndex, strErrorMessagePath);
                        }
                    }

                }
            }
        }

        private void UpdateSubProgress_SaveImage()
        {
            while (!m_blnStopping)
            {

                try
                {
                    if ((m_intPassStartNode != m_intPassEndNode) ||
                        (m_intFailStartNode != m_intFailEndNode))
                    {
                        SaveImageBuffer();
                    }
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Vision6Process->UpdateSubProgress_GrabImage() :" + ex.ToString());
                }
                Thread.Sleep(1);
            }

            m_thSubThread_SaveImage = null;
            m_blnStopped_SaveImage = true;
        }

        private void SaveImageBuffer()
        {
            try
            {
                if (m_intPassStartNode != m_intPassEndNode)
                {
                    //STTrackLog.WriteLine("Start=" + m_intPassStartNode.ToString() + ", End=" + m_intPassEndNode.ToString());

                    //string strPath = m_smVisionInfo.g_strSaveImageLocation +
                    //            m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                    //            "\\" + m_smVisionInfo.g_strVisionFolderName +
                    //            "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" +
                    //            "\\Pass\\";

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


                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        m_arrPassCImage1Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + ".bmp");
                        //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                        m_arrPassCImage1Buffer[m_intPassStartNode].Dispose();
                        m_arrPassCImage1Buffer[m_intPassStartNode] = new CImageDrawing(true);
                    }
                    else
                    {
                        m_arrPassImage1Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + ".bmp");
                        //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                        m_arrPassImage1Buffer[m_intPassStartNode].Dispose();
                        m_arrPassImage1Buffer[m_intPassStartNode] = new ImageDrawing(true);
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 1)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage2Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image1.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage2Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage2Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage2Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image1.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage2Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage2Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 2)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage3Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image2.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage3Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage3Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage3Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image2.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage3Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage3Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 3)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage4Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image3.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage4Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage4Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage4Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image3.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage4Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage4Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 4)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage5Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image4.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage5Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage5Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage5Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image4.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage5Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage5Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 5)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage6Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image5.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage6Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage6Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage6Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image5.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage6Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage6Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 6)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage7Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image6.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage7Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage7Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage7Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + "_Image6.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage7Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage7Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    m_smVisionInfo.g_strLastImageFolder = strPath;
                    m_smVisionInfo.g_strLastImageName = m_arrPassNoBuffer[m_intPassStartNode] + "_Pass_" + m_arrRetryCountBuffer[m_intPassStartNode] + ".bmp";

                    m_intPassStartNode++;
                    if (m_intPassStartNode == m_smVisionInfo.g_intSaveImageBufferSize)
                    {
                        m_intPassStartNode = 0;
                    }
                }

                if (m_intFailStartNode != m_intFailEndNode)
                {
                    //STTrackLog.WriteLine("Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());

                    //2021-02-24 ZJYEOH : Should use m_smVisionInfo.g_intFailImageCount to compare
                    if (/*m_arrFailNoBuffer[m_intFailStartNode]*/ m_smVisionInfo.g_intFailImageCount > m_smCustomizeInfo.g_intFailImagePics)//>=
                    {
                        if (!m_blnLoadRejectImageListPath)
                        {
                            LoadRejectImageListPath();

                            m_blnLoadRejectImageListPath = true;
                        }

                        if (m_arrRejectImageListPath.Count > 0)
                        {
                            string strDeleteFile = m_arrRejectImageListPath[0];
                            if (File.Exists(strDeleteFile))
                            {
                                File.Delete(strDeleteFile);
                            }

                            if (m_smVisionInfo.g_arrImages.Count > 1)
                            {
                                int intStartIndex = strDeleteFile.LastIndexOf(".bmp");
                                if (intStartIndex > 0)
                                {
                                    string strDeleteFileImage1 = strDeleteFile.Substring(0, intStartIndex) + "_Image1.bmp";
                                    if (File.Exists(strDeleteFileImage1))
                                    {
                                        File.Delete(strDeleteFileImage1);
                                    }
                                }
                            }

                            if (m_smVisionInfo.g_arrImages.Count > 2)
                            {
                                int intStartIndex = strDeleteFile.LastIndexOf(".bmp");
                                if (intStartIndex > 0)
                                {
                                    string strDeleteFileImage2 = strDeleteFile.Substring(0, intStartIndex) + "_Image2.bmp";
                                    if (File.Exists(strDeleteFileImage2))
                                    {
                                        File.Delete(strDeleteFileImage2);
                                    }
                                }
                            }

                            if (m_smVisionInfo.g_arrImages.Count > 3)
                            {
                                int intStartIndex = strDeleteFile.LastIndexOf(".bmp");
                                if (intStartIndex > 0)
                                {
                                    string strDeleteFileImage3 = strDeleteFile.Substring(0, intStartIndex) + "_Image3.bmp";
                                    if (File.Exists(strDeleteFileImage3))
                                    {
                                        File.Delete(strDeleteFileImage3);
                                    }
                                }
                            }

                            if (m_smVisionInfo.g_arrImages.Count > 4)
                            {
                                int intStartIndex = strDeleteFile.LastIndexOf(".bmp");
                                if (intStartIndex > 0)
                                {
                                    string strDeleteFileImage4 = strDeleteFile.Substring(0, intStartIndex) + "_Image4.bmp";
                                    if (File.Exists(strDeleteFileImage4))
                                    {
                                        File.Delete(strDeleteFileImage4);
                                    }
                                }
                            }

                            if (m_smVisionInfo.g_arrImages.Count > 5)
                            {
                                int intStartIndex = strDeleteFile.LastIndexOf(".bmp");
                                if (intStartIndex > 0)
                                {
                                    string strDeleteFileImage5 = strDeleteFile.Substring(0, intStartIndex) + "_Image5.bmp";
                                    if (File.Exists(strDeleteFileImage5))
                                    {
                                        File.Delete(strDeleteFileImage5);
                                    }
                                }
                            }

                            if (m_smVisionInfo.g_arrImages.Count > 6)
                            {
                                int intStartIndex = strDeleteFile.LastIndexOf(".bmp");
                                if (intStartIndex > 0)
                                {
                                    string strDeleteFileImage6 = strDeleteFile.Substring(0, intStartIndex) + "_Image6.bmp";
                                    if (File.Exists(strDeleteFileImage6))
                                    {
                                        File.Delete(strDeleteFileImage6);
                                    }
                                }
                            }

                            m_arrRejectImageListPath.RemoveAt(0);
                        }

                        if (m_arrRejectImageErrorMessageListPath.Count > 0)
                        {
                            string strDeleteFile = m_arrRejectImageErrorMessageListPath[0];
                            if (File.Exists(strDeleteFile))
                            {
                                File.Delete(strDeleteFile);
                            }

                            m_arrRejectImageErrorMessageListPath.RemoveAt(0);
                        }
                    }
                    else if (m_arrFailNoBuffer[m_intFailStartNode] == 0)
                    {
                        m_arrRejectImageListPath.Clear();
                        m_arrRejectImageErrorMessageListPath.Clear();
                    }

                    //string strPath = m_smVisionInfo.g_strSaveImageLocation +
                    //                m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                    //                "\\" + m_smVisionInfo.g_strVisionFolderName +
                    //                "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" +
                    //                "\\" + m_arrRejectNameBuffer[m_intFailStartNode] + "\\";

                    // 2020 03 27 - JBTAN: Save to different folder if reset count
                    string strPath;
                    if (m_smVisionInfo.g_intVisionResetCount == 0)
                    {
                        strPath = m_smVisionInfo.g_strSaveImageLocation +
                            m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                            "\\" + m_smVisionInfo.g_strVisionFolderName +
                            "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime +
                            "\\" + m_arrRejectNameBuffer[m_intFailStartNode] + "\\";
                    }
                    else
                    {
                        string strLotSaveImagePath = m_smVisionInfo.g_strSaveImageLocation + m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;
                        string strVisionImageFolderName = m_smVisionInfo.g_strVisionFolderName + "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smVisionInfo.g_strVisionResetCountTime;

                        if (!Directory.Exists(strLotSaveImagePath + "\\" + strVisionImageFolderName))
                            Directory.CreateDirectory(strLotSaveImagePath + "\\" + strVisionImageFolderName);
                        if (!Directory.Exists(strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\" + m_arrRejectNameBuffer[m_intFailStartNode] + "\\"))
                            Directory.CreateDirectory(strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\" + m_arrRejectNameBuffer[m_intFailStartNode] + "\\");

                        strPath = strLotSaveImagePath + "\\" + strVisionImageFolderName + "\\" + m_arrRejectNameBuffer[m_intFailStartNode] + "\\";
                    }


                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        m_arrFailCImage1Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp");
                        //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                        m_arrFailCImage1Buffer[m_intFailStartNode].Dispose();
                        m_arrFailCImage1Buffer[m_intFailStartNode] = new CImageDrawing(true);
                    }
                    else
                    {
                        m_arrFailImage1Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp");
                        //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                        m_arrFailImage1Buffer[m_intFailStartNode].Dispose();
                        m_arrFailImage1Buffer[m_intFailStartNode] = new ImageDrawing(true);
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 1)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage2Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image1.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage2Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage2Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage2Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image1.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage2Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage2Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 2)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage3Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image2.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage3Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage3Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage3Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image2.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage3Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage3Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 3)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage4Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image3.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage4Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage4Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage4Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image3.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage4Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage4Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 4)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage5Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image4.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage5Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage5Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage5Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image4.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage5Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage5Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 5)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage6Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image5.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage6Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage6Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage6Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image5.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage6Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage6Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 6)
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage7Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image6.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage7Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage7Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage7Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image6.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage7Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage7Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smCustomizeInfo.g_blnSaveFailImageErrorMessage)
                    {
                        XmlParser objFile = new XmlParser(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".xml");
                        objFile.WriteSectionElement("ErrorMessage");
                        objFile.WriteElement1Value("Message_" + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode], m_arrRejectMessageBuffer[m_intFailStartNode]);
                        objFile.WriteEndElement();
                    }

                    m_smVisionInfo.g_strLastImageFolder = strPath;
                    m_smVisionInfo.g_strLastImageName = m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp";
                    m_arrRejectImageListPath.Add(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRetryCountBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp");
                    m_arrRejectImageErrorMessageListPath.Add(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".xml");
                    m_intFailStartNode++;
                    if (m_intFailStartNode == m_smVisionInfo.g_intSaveImageBufferSize)
                    {
                        m_intFailStartNode = 0;
                    }
                }


            }
            catch (Exception ex)
            {
            }
        }

        public void WaitAllThreadStopped()
        {
            HiPerfTimer timesout = new HiPerfTimer();
            timesout.Start();

            while (true)
            {
                if (m_blnStopped &&
                    m_blnStopped_SaveImage)
                {
                    STTrackLog.WriteLine("Vision6Process All threads have stopped.");
                    break;
                }

                if (timesout.Timing > 3000)
                {
                    STTrackLog.WriteLine("Vision6Process : m_blnStopped = " + m_blnStopped.ToString());
                    STTrackLog.WriteLine("Vision6Process : m_blnStopped_SaveImage = " + m_blnStopped_SaveImage.ToString());
                    STTrackLog.WriteLine("Vision6Process : >>>>>>>>>>>>> time out 3");
                    break;
                }

                Thread.Sleep(1);
            }
        }

        private void LoadNextImageForDebugRunTest()
        {
            if (m_smVisionInfo.g_arrImageFiles.Count == 0)
                return;

            if (m_smVisionInfo.g_intFileIndex < 0 || m_smVisionInfo.g_intFileIndex >= m_smVisionInfo.g_arrImageFiles.Count)
                m_smVisionInfo.g_intFileIndex = 0;

            string strFileName = m_smVisionInfo.g_arrImageFiles[m_smVisionInfo.g_intFileIndex].ToString();

            if (!m_smVisionInfo.g_blnViewColorImage)
            {
                m_smVisionInfo.g_arrImages[0].LoadImage(strFileName);
                for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    string strDirPath = Path.GetDirectoryName(strFileName);
                    string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + i.ToString() + ".BMP";

                    if (File.Exists(strPkgView))
                        m_smVisionInfo.g_arrImages[i].LoadImage(strPkgView);
                    else
                        m_smVisionInfo.g_arrImages[i].LoadImage(strFileName);
                }
            }
            else
            {
                m_smVisionInfo.g_arrColorImages[0].LoadImage(strFileName);
                m_smVisionInfo.g_arrColorImages[0].ConvertColorToMono(ref m_smVisionInfo.g_arrImages, 0);
                for (int i = 1; i < m_smVisionInfo.g_arrColorImages.Count; i++)
                {
                    string strDirPath = Path.GetDirectoryName(strFileName);
                    string strPkgView = strDirPath + "\\" + Path.GetFileNameWithoutExtension(strFileName) + "_Image" + i.ToString() + ".BMP";

                    if (File.Exists(strPkgView))
                        m_smVisionInfo.g_arrColorImages[i].LoadImage(strPkgView);
                    else
                        m_smVisionInfo.g_arrColorImages[i].LoadImage(strFileName);
                    m_smVisionInfo.g_arrColorImages[i].ConvertColorToMono(ref m_smVisionInfo.g_arrImages, i);
                }
            }

            if (++m_smVisionInfo.g_intFileIndex == m_smVisionInfo.g_arrImageFiles.Count)
                m_smVisionInfo.g_intFileIndex = 0;
        }


        private void InitSaveImageBuffer(int intGrabRequire)
        {
            if (m_smVisionInfo.g_blnViewColorImage)
            {
                if (intGrabRequire > 0)
                    m_arrPassCImage1Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 1)
                    m_arrPassCImage2Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 2)
                    m_arrPassCImage3Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 3)
                    m_arrPassCImage4Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 4)
                    m_arrPassCImage5Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 5)
                    m_arrPassCImage6Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 6)
                    m_arrPassCImage7Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];

                if (intGrabRequire > 0)
                    m_arrFailCImage1Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 1)
                    m_arrFailCImage2Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 2)
                    m_arrFailCImage3Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 3)
                    m_arrFailCImage4Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 4)
                    m_arrFailCImage5Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 5)
                    m_arrFailCImage6Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 6)
                    m_arrFailCImage7Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];

                for (int i = 0; i < m_smVisionInfo.g_intSaveImageBufferSize; i++)
                {
                    if (intGrabRequire > 0)
                        m_arrPassCImage1Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 1)
                        m_arrPassCImage2Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 2)
                        m_arrPassCImage3Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 3)
                        m_arrPassCImage4Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 4)
                        m_arrPassCImage5Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 5)
                        m_arrPassCImage6Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 6)
                        m_arrPassCImage7Buffer[i] = new CImageDrawing(true);

                    if (intGrabRequire > 0)
                        m_arrFailCImage1Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 1)
                        m_arrFailCImage2Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 2)
                        m_arrFailCImage3Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 3)
                        m_arrFailCImage4Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 4)
                        m_arrFailCImage5Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 5)
                        m_arrFailCImage6Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 6)
                        m_arrFailCImage7Buffer[i] = new CImageDrawing(true);
                }
            }
            else
            {
                if (intGrabRequire > 0)
                    m_arrPassImage1Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 1)
                    m_arrPassImage2Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 2)
                    m_arrPassImage3Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 3)
                    m_arrPassImage4Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 4)
                    m_arrPassImage5Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 5)
                    m_arrPassImage6Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 6)
                    m_arrPassImage7Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];

                if (intGrabRequire > 0)
                    m_arrFailImage1Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 1)
                    m_arrFailImage2Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 2)
                    m_arrFailImage3Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 3)
                    m_arrFailImage4Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 4)
                    m_arrFailImage5Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 5)
                    m_arrFailImage6Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 6)
                    m_arrFailImage7Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];

                for (int i = 0; i < m_smVisionInfo.g_intSaveImageBufferSize; i++)
                {
                    if (intGrabRequire > 0)
                        m_arrPassImage1Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 1)
                        m_arrPassImage2Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 2)
                        m_arrPassImage3Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 3)
                        m_arrPassImage4Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 4)
                        m_arrPassImage5Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 5)
                        m_arrPassImage6Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 6)
                        m_arrPassImage7Buffer[i] = new ImageDrawing(true);

                    if (intGrabRequire > 0)
                        m_arrFailImage1Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 1)
                        m_arrFailImage2Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 2)
                        m_arrFailImage3Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 3)
                        m_arrFailImage4Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 4)
                        m_arrFailImage5Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 5)
                        m_arrFailImage6Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 6)
                        m_arrFailImage7Buffer[i] = new ImageDrawing(true);
                }
            }
            m_arrPassNoBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrFailNoBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrRetryCountBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrRejectNameBuffer = new string[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrRejectMessageBuffer = new string[m_smVisionInfo.g_intSaveImageBufferSize];
        }

        private void AddFailCounter()
        {
            switch (m_smVisionInfo.g_objSeal.ref_intSealFailMask)
            {
                case 0x01:  // Fail position, position pattern no found. 
                    m_smVisionInfo.g_intPositionFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intPositionFailureTotal = " + m_smVisionInfo.g_intPositionFailureTotal.ToString());
                    }
                    break;
                //seal distance
                case 0x02:
                    m_smVisionInfo.g_intSealDistanceFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealDistanceFailureTotal = " + m_smVisionInfo.g_intSealDistanceFailureTotal.ToString());
                    }
                    break;
                //seal bubble
                case 0x04:
                    m_smVisionInfo.g_intSealBrokenAreaFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealBrokenAreaFailureTotal = " + m_smVisionInfo.g_intSealBrokenAreaFailureTotal.ToString());
                    }
                    break;
                //seal broken
                case 0x08:
                    m_smVisionInfo.g_intSealBrokenGapFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealBrokenGapFailureTotal = " + m_smVisionInfo.g_intSealBrokenGapFailureTotal.ToString());
                    }
                    break;
                //seal width
                //seal width insufficient
                case 0x10:    // 16
                case 0x20:    //32
                    m_smVisionInfo.g_intSealFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealFailureTotal = " + m_smVisionInfo.g_intSealFailureTotal.ToString());
                    }
                    break;
                //seal overheat
                //Tape Scratches
                case 0x40:    // 64 
                case 0x800:
                    m_smVisionInfo.g_intSealOverHeatFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealOverHeatFailureTotal = " + m_smVisionInfo.g_intSealOverHeatFailureTotal.ToString());
                    }
                    break;
                //sprocket hole distance
                case 0x200:   //  512
                    m_smVisionInfo.g_intSealSprocketHoleFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealSprocketHoleFailureTotal = " + m_smVisionInfo.g_intSealSprocketHoleFailureTotal.ToString());
                    }
                    break;
                //sprocket hole diameter
                case 0x2000:   
                    m_smVisionInfo.g_intSealSprocketHoleDiameterFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealSprocketHoleDiameterFailureTotal = " + m_smVisionInfo.g_intSealSprocketHoleDiameterFailureTotal.ToString());
                    }
                    break;
                //sprocket hole defect
                case 0x4000:   
                    m_smVisionInfo.g_intSealSprocketHoleDefectFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealSprocketHoleDefectFailureTotal = " + m_smVisionInfo.g_intSealSprocketHoleDefectFailureTotal.ToString());
                    }
                    break;
                //sprocket hole broken
                case 0x8000:
                    m_smVisionInfo.g_intSealSprocketHoleBrokenFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealSprocketHoleBrokenFailureTotal = " + m_smVisionInfo.g_intSealSprocketHoleBrokenFailureTotal.ToString());
                    }
                    break;
                //sprocket hole roundness
                case 0x10000:
                    m_smVisionInfo.g_intSealSprocketHoleRoundnessFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealSprocketHoleRoundnessFailureTotal = " + m_smVisionInfo.g_intSealSprocketHoleRoundnessFailureTotal.ToString());
                    }
                    break;
                //seal edge straightness
                case 0x20000:
                    m_smVisionInfo.g_intSealEdgeStraightnessFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intSealEdgeStraightnessFailureTotal = " + m_smVisionInfo.g_intSealEdgeStraightnessFailureTotal.ToString());
                    }
                    break;
                case 0x080:             // Unit Present using pattern matching or angle fail.
                case 0x400:             // Unit Present using white black pixel count
                case 0x480:  //1152     // Unit Present using white black pixel count
                    m_smVisionInfo.g_intCheckPresenceFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intCheckPresenceFailureTotal = " + m_smVisionInfo.g_intCheckPresenceFailureTotal.ToString());
                    }
                    break;
                case 0x1000:
                    m_smVisionInfo.g_intOrientFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intOrientFailureTotal = " + m_smVisionInfo.g_intOrientFailureTotal.ToString());
                    }
                    break;
                case 0x40000:
                    m_smVisionInfo.g_intNoTemplateFailureTotal++;
                    if (m_smVisionInfo.g_blnTrackSealOption)
                    {
                        STTrackLog.WriteLine("Fail ++ g_intNoTemplateFailureTotal = " + m_smVisionInfo.g_intNoTemplateFailureTotal.ToString());
                    }
                    break;
                default:
                    SRMMessageBox.Show("Vision6Process > StartTest > m_smVisionInfo.g_objSeal.ref_intSealFailMask value " + m_smVisionInfo.g_objSeal.ref_intSealFailMask.ToString() + " no exist in Switch Case");
                    break;
            }

            m_smVisionInfo.g_intContinuousFailUnitCount++;

            m_smVisionInfo.g_intTestedTotal++;
            m_smVisionInfo.g_intLowYieldUnitCount++;

            if (m_smVisionInfo.g_blnTrackSealOption)
            {
                STTrackLog.WriteLine("g_intTestedTotal++ A= " + m_smVisionInfo.g_intTestedTotal.ToString());
            }


            m_smVisionInfo.VS_AT_UpdateQuantity = true;
            m_smVisionInfo.PR_VM_UpdateQuantity = true;

        }
        private bool DeleteTemplate()
        {
            try
            {
                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    m_smVisionInfo.g_objSeal.DeleteAllPreviousTemplate(strFolderPath + "Seal\\");
                    m_smVisionInfo.g_objSeal.ClearTemplateSetting();
                    m_smVisionInfo.g_blnUnitInspected[i] = false;
                }
                // Reset variables
                m_smVisionInfo.g_intMarkTemplateTotal = 0;
                m_smVisionInfo.g_intMarkTemplateMask = 0;

                XmlParser objFile = new XmlParser(strFolderPath + "General.xml");
                objFile.WriteSectionElement("TemplateCounting");
                objFile.WriteElement1Value("TotalMarkTemplates", m_smVisionInfo.g_intMarkTemplateTotal);
                objFile.WriteElement1Value("MarkTemplateMask ", m_smVisionInfo.g_intMarkTemplateMask);
                objFile.WriteEndElement();

                m_smVisionInfo.PG_VM_LoadTemplate = true;
                m_smVisionInfo.VM_AT_TemplateNotLearn = true;
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
