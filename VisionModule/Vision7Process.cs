using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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

namespace VisionModule
{
    class Vision7Process
    {
        #region constant variables

        //private const int BUFFERSIZE = 20;

        #endregion
        #region Member Variables

        // Thread handle
        private bool m_blnAuto = false;
        private bool m_blnPatternResultOK = true;
        private bool[] m_blnBarCodeOut = new bool[10];
        private readonly object m_objStopLock = new object();
        private int m_intCounter = 0;
        private bool m_blnStopping = false;
        private bool m_blnPause = false;
        private bool m_blnForceStopProduction = false;
        private bool m_blnLoadRejectImageListPath = false;
        private bool m_blnDisplayOrientFail = false;
        private int m_intPassStartNode = 0;
        private int m_intFailStartNode = 0;
        private int m_intPassEndNode = 0;
        private int m_intFailEndNode = 0;
        private int m_intFileIndex = 0;

        private Point[] m_arrROIStart = new Point[10];
        private Size[] m_arrROISize = new Size[10];
        private float[] m_arrAngle = new float[10];
        private ImageDrawing m_objImage_Temp = new ImageDrawing(true);

        private int m_intRetestCount = 0;

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
        private bool m_blnStopped = false, m_blnStopped_BarcodeTest = false, m_blnStopped_GrabImage = false, m_blnStopped_SaveImage = false;
        private Thread m_thThread, m_thSubThread_BarcodeTest, m_thSubThread_GrabImage, m_thSubThread_SaveImage;
        private int m_intCameraOutState = 1;
        private int m_intRetryCount = 0;

        private bool m_bSubTh1_GrabImage = false;
        private bool m_bSubTh_BarcodeTest = false;
        private bool m_bSubTh_BarcodeTest_Result = false;
        private bool m_bGrabImage1Done = false;
        private bool m_bGrabImage2Done = false;
        private bool m_bGrabImage3Done = false;
        private bool m_bGrabImage4Done = false;
        private bool m_bGrabImage5Done = false;
        private bool m_bGrabImage6Done = false;
        private bool m_bGrabImage7Done = false;
        private bool m_bGrabImage1Result = false;
        private bool m_bGrabImage2Result = false;
        private bool m_bGrabImage3Result = false;
        private bool m_bGrabImage4Result = false;
        private bool m_bGrabImage5Result = false;
        private bool m_bGrabImage6Result = false;
        private bool m_bGrabImage7Result = false;

        //private ImageDrawing[] m_arrInspectionImage1Buffer = null;
        //private ImageDrawing[] m_arrInspectionImage2Buffer = null;
        //private ImageDrawing[] m_arrInspectionImage3Buffer = null;
        //private ImageDrawing[] m_arrInspectionImage4Buffer = null;
        //private ImageDrawing[] m_arrInspectionImage5Buffer = null;
        //private ImageDrawing[] m_arrInspectionImage6Buffer = null;
        //private ImageDrawing[] m_arrInspectionImage7Buffer = null;

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
        float m_fTimingPrev = 0;
        float m_fTiming = 0;
        #endregion

        string m_strResultTrack = "";
        public Vision7Process(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
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

            m_thSubThread_GrabImage = new Thread(new ThreadStart(UpdateSubProgress_GrabImage));
            m_thSubThread_GrabImage.IsBackground = true;
            m_thSubThread_GrabImage.Priority = ThreadPriority.Highest;
            m_thSubThread_GrabImage.Start();

            m_thSubThread_SaveImage = new Thread(new ThreadStart(UpdateSubProgress_SaveImage));
            m_thSubThread_SaveImage.IsBackground = true;
            m_thSubThread_SaveImage.Priority = ThreadPriority.Lowest;
            m_thSubThread_SaveImage.Start();

            m_thSubThread_BarcodeTest = new Thread(new ThreadStart(UpdateSubProgress_BarcodeTest));
            m_thSubThread_BarcodeTest.IsBackground = true;
            m_thSubThread_BarcodeTest.Priority = ThreadPriority.Highest;
            m_thSubThread_BarcodeTest.Start();
        }

        public Vision7Process(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
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

            m_thSubThread_GrabImage = new Thread(new ThreadStart(UpdateSubProgress_GrabImage));
            m_thSubThread_GrabImage.IsBackground = true;
            m_thSubThread_GrabImage.Priority = ThreadPriority.Highest;
            m_thSubThread_GrabImage.Start();

            m_thSubThread_SaveImage = new Thread(new ThreadStart(UpdateSubProgress_SaveImage));
            m_thSubThread_SaveImage.IsBackground = true;
            m_thSubThread_SaveImage.Priority = ThreadPriority.Lowest;
            m_thSubThread_SaveImage.Start();

            m_thSubThread_BarcodeTest = new Thread(new ThreadStart(UpdateSubProgress_BarcodeTest));
            m_thSubThread_BarcodeTest.IsBackground = true;
            m_thSubThread_BarcodeTest.Priority = ThreadPriority.Highest;
            m_thSubThread_BarcodeTest.Start();
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
                return GrabImage_Sequence_NoSetIntensity_Teli(blnForInspection);
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
        public bool GrabImage_Sequence_NoSetIntensity_Teli(bool blnForInspection)
        {
            if (!m_objTeliCamera.IsCameraInitDone())
            {
                m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
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
            TrackTiming(true, "StartGrab", m_smVisionInfo.g_blnTrackBasic, m_smVisionInfo.g_blnTrackBasic);
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

                    TrackTiming(false, "GrabDone", m_smVisionInfo.g_blnTrackBasic, m_smVisionInfo.g_blnTrackBasic);
                    //// STTrackLog.WriteLine("Grab i = " + i.ToString());
                    if (i == 1)
                    {
                        //      STTrackLog.WriteLine("Grab 1 done.");
                        if (blnSuccess) m_bGrabImage1Result = true;
                        m_bGrabImage1Done = true;
                    }
                    else if (i == 2)
                    {
                        if (blnSuccess) m_bGrabImage2Result = true;
                        m_bGrabImage2Done = true;
                    }
                    else if (i == 3)
                    {
                        if (blnSuccess) m_bGrabImage3Result = true;
                        m_bGrabImage3Done = true;
                    }
                    else if (i == 4)
                    {
                        if (blnSuccess) m_bGrabImage4Result = true;
                        m_bGrabImage4Done = true;
                    }
                    else if (i == 5)
                    {
                        if (blnSuccess) m_bGrabImage5Result = true;
                        m_bGrabImage5Done = true;
                    }
                    else if (i == 6)
                    {
                        if (blnSuccess) m_bGrabImage6Result = true;
                        m_bGrabImage6Done = true;
                    }
                    else if (i == 7)
                    {
                        if (blnSuccess) m_bGrabImage7Result = true;
                        m_bGrabImage7Done = true;
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
                TrackTiming(false, "GrabDone", m_smVisionInfo.g_blnTrackBasic, m_smVisionInfo.g_blnTrackBasic);
                TrackTiming(true, "StartTransfer", m_smVisionInfo.g_blnTrackBasic, m_smVisionInfo.g_blnTrackBasic);
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

                            string strPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" +
                       m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\";
                            if (m_smVisionInfo.g_intLearnStepNo != 3 && m_smVisionInfo.g_objBarcode.ref_blnWantUseReferenceImage && File.Exists(strPath + "\\Template\\" + "BrightReferenceImage.bmp") && File.Exists(strPath + "\\Template\\" + "DarkReferenceImage.bmp"))
                            {
                                ImageDrawing.UniformizeImage(m_smVisionInfo.g_ojRotateImage, m_smVisionInfo.g_objBrightReferenceImage, m_smVisionInfo.g_objDarkReferenceImage, ref m_smVisionInfo.g_ojRotateImage, m_smVisionInfo.g_objBarcode.ref_intUniformizeGain);
                                //objUniformImage.AddGain(ref objUniformImage, 2);
                                
                                //m_smVisionInfo.g_arrImages[0].SaveImage("D:\\img.bmp");
                                //objUniformImage.SaveImage("D:\\objUniformImage.bmp");
                                //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI2.bmp");

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
                        TrackTiming(false, "TransferDone", m_smVisionInfo.g_blnTrackBasic, m_smVisionInfo.g_blnTrackBasic);

                        if (m_intGrabRequire == 1)
                        {
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab && blnForInspection) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TS\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_intCounter + ".bmp");
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab && blnForInspection) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TS\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                        }

                        if (m_intGrabRequire == 1)
                        {
                            if (blnSuccess) m_bGrabImage1Result = true;
                            m_bGrabImage1Done = true;
                        }
                        else if (m_intGrabRequire == 2)
                        {
                            if (blnSuccess) m_bGrabImage2Result = true;
                            m_bGrabImage2Done = true;
                        }
                        else if (m_intGrabRequire == 3)
                        {
                            if (blnSuccess) m_bGrabImage3Result = true;
                            m_bGrabImage3Done = true;
                        }
                        else if (m_intGrabRequire == 4)
                        {
                            if (blnSuccess) m_bGrabImage4Result = true;
                            m_bGrabImage4Done = true;
                        }
                        else if (m_intGrabRequire == 5)
                        {
                            if (blnSuccess) m_bGrabImage5Result = true;
                            m_bGrabImage5Done = true;
                        }
                        else if (m_intGrabRequire == 6)
                        {
                            if (blnSuccess) m_bGrabImage6Result = true;
                            m_bGrabImage6Done = true;
                        }
                        else if (m_intGrabRequire == 7)
                        {
                            if (blnSuccess) m_bGrabImage7Result = true;
                            m_bGrabImage7Done = true;
                        }
                    }
                }
                timer_TotalGrabTime.Stop();
                fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
                //objTL.WriteLine("Total grab time after Grab loop = " + fTotalGrabTime.ToString());
            }
            else
                SetGrabDone();

            m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;

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
        private void TrackTiming(bool blnResetTiming, string strDataName, bool blnWrite, bool blnRecord)
        {
            if (blnResetTiming)
            {
                m_smVisionInfo.m_tTrackTiming.Start();
                m_smVisionInfo.g_strTrackPad = "";
                m_fTimingPrev = 0;
                m_fTiming = 0;
            }
            else
            {
                if (blnRecord)
                {
                    m_fTiming = m_smVisionInfo.m_tTrackTiming.Timing;
                    m_smVisionInfo.g_strTrackPad += ", " + strDataName + "=" + (m_fTiming - m_fTimingPrev).ToString();
                    m_fTimingPrev = m_fTiming;
                }
            }

            if (blnWrite)
            {
                STTrackLog.WriteLine(m_smVisionInfo.g_strTrackPad);
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
        private void ConvertColorToMono(int intIndex)
        {
            m_smVisionInfo.g_arrColorImages[intIndex].ConvertColorToMono(m_smVisionInfo.g_arrImages[intIndex]);
        }        /// <summary>
                 /// Attach corresponding image to its parent or ROI
                 /// </summary>
        private void AttachImageToROI()
        {
            m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

            if ((m_smCustomizeInfo.g_intWantBarcode & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                AttachToROI(m_smVisionInfo.g_arrBarcodeROIs, m_smVisionInfo.g_arrImages);
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
        private void AttachToROI(List<ROI> arrROI, List<ImageDrawing> arrImage)
        {
            ROI objROI;

            for (int i = 0; i < arrROI.Count; i++)
            {

                objROI = (ROI)arrROI[i];

                switch (objROI.ref_intType)
                {
                    case 1:
                        //if (arrImage.Count >= m_smVisionInfo.g_objBarcode.GetGrabImageIndex(i))
                        //    objROI.AttachImage(arrImage[m_smVisionInfo.g_objBarcode.GetGrabImageIndex(i)]);
                        //else
                        objROI.AttachImage(arrImage[0]);
                        break;
                    case 2:
                        objROI.AttachImage((ROI)arrROI[0]);
                        break;
                }
                arrROI[i] = objROI;
            }

            objROI = null;
        } /// <summary>
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
        private bool WaitEventDone(ref bool bTriggerEvent, bool bBreakResult, ref bool bReturnResult, string strTrackName)
        {
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            while (true)
            {
                if (bTriggerEvent == bBreakResult)
                {
                    return bReturnResult;
                }

                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> Vision 7 time out 10 - " + strTrackName);
                    bTriggerEvent = bBreakResult;
                    break;
                }

                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }

            timeout.Stop();

            return false;
        }
        private void WaitEventDone(ref bool bTriggerEvent, bool bBreakResult, bool blnForDrawEvent)
        {
            while (true)
            {
                if (blnForDrawEvent)
                {
                    if (bTriggerEvent == bBreakResult || m_smVisionInfo.g_intMachineStatus == 1 || !m_smProductionInfo.g_blnInBarCodePage || !m_smVisionInfo.g_blnDrawBarcodeResult)
                    {
                        m_smVisionInfo.g_blnDrawBarcodeResult = false;
                        return;
                    }
                }
                else
                {
                    if (bTriggerEvent == bBreakResult || m_smVisionInfo.g_intMachineStatus == 1 || !m_smProductionInfo.g_blnInBarCodePage)
                    {
                        return;
                    }
                }
                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }
        }
        private void WaitEventDone_ForDisplayAllPage(ref bool bTriggerEvent, bool bBreakResult)
        {
            while (true)
            {
                if (bTriggerEvent == bBreakResult || m_smVisionInfo.g_intMachineStatus == 1 || !m_smProductionInfo.g_blnInDisplayAllPage)
                {
                    return;
                }

                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }
        }

        private string GetCodeType()
        {
            switch (m_smVisionInfo.g_objBarcode.ref_intCodeType)
            {
                case 0:
                    return "Barcode";
                case 1:
                    return "QR Code";
                case 2:
                    return "Matrix Code";
            }
            return "Barcode";
        }

        private void StartTest(bool blnAuto)
        {
            WaitEventDone_ForDisplayAllPage(ref m_smVisionInfo.VS_AT_UpdateQuantity, false);
            WaitEventDone(ref m_smVisionInfo.PR_VM_UpdateQuantity, false, false);
            WaitEventDone(ref m_smVisionInfo.g_blnDrawBarcodeResultDone, true, true);

            m_smVisionInfo.g_objBarcode.ref_strErrorMessage = "";
            m_strResultTrack = "";
            m_smVisionInfo.g_objProcessTime.Start();
            m_smVisionInfo.g_blnDrawBarcodeResultDone = false;
            m_smVisionInfo.g_blnDrawBarcodeResult = false;
            m_smVisionInfo.g_blnViewBarcodeInspection = false;
            bool blnTotalCountAdded = false;
            bool blnResultOK = true;
            bool blnPatternResultOK = true;
            bool[] blnBarCodeOut = new bool[10];
            // ------------------ Reset Inspection data ----------------------------------------------
            m_smVisionInfo.g_objBarcode.ResetInspectionData_Inspection();

            if (m_smVisionInfo.g_arrBarcodeROIs.Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage = "*" + GetCodeType() + " : No Search ROI";
                if (blnAuto)
                {
                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                    m_smVisionInfo.g_intTestedTotal++;
                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                    blnTotalCountAdded = true;
                }
                blnResultOK = false;
            }
            else if (m_smVisionInfo.g_arrBarcodeROIs.Count == 1)
            {
                m_smVisionInfo.g_strErrorMessage = "*" + GetCodeType() + " : No Template Found";
                if (blnAuto)
                {
                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                    m_smVisionInfo.g_intTestedTotal++;
                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                    blnTotalCountAdded = true;
                }
                //m_smVisionInfo.g_intContinuousFailUnitCount++; //2020-02-21 ZJYEOH : removed this as g_intContinuousFailUnitCount already increased in above "if (blnAuto)" condition
                blnResultOK = false;
            }
            else if (m_smVisionInfo.g_arrBarcodeROIs.Count == 2)
            {
                m_smVisionInfo.g_strErrorMessage = "*" + GetCodeType() + " : No " + GetCodeType() + " ROI";
                if (blnAuto)
                {
                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                    m_smVisionInfo.g_intTestedTotal++;
                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                    blnTotalCountAdded = true;
                }
                //m_smVisionInfo.g_intContinuousFailUnitCount++; //2020-02-21 ZJYEOH : removed this as g_intContinuousFailUnitCount already increased in above "if (blnAuto)" condition
                blnResultOK = false;
            }
            else if (m_smVisionInfo.g_objBarcode.ref_intTemplateCount == 0)
            {
                m_smVisionInfo.g_strErrorMessage = "*" + GetCodeType() + " : No " + GetCodeType() + " Template Code Found";
                if (blnAuto)
                {
                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                    m_smVisionInfo.g_intTestedTotal++;
                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                    //m_smVisionInfo.g_intLowYieldUnitCount++;
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

            if (blnResultOK)
            {

                AttachToROI(m_smVisionInfo.g_arrBarcodeROIs, m_smVisionInfo.g_arrImages);

            }

            if (blnResultOK)
            {
                //Check position
                if (!m_smVisionInfo.g_objBarcode.MatchReferencePattern(m_smVisionInfo.g_arrBarcodeROIs[0]))
                {
                    blnPatternResultOK = false;
                    blnResultOK = false;
                    //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                }
                else
                {
                    //int PatternTol = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance; // 20
                    float PatternCenterX = m_smVisionInfo.g_objBarcode.GetMatchingCenterX();
                    float PatternCenterY = m_smVisionInfo.g_objBarcode.GetMatchingCenterY();
                    int PatternWidth = m_smVisionInfo.g_objBarcode.GetMatchingTemplateWidth();
                    int PatternHeight = m_smVisionInfo.g_objBarcode.GetMatchingTemplateHeight();

                    if (((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + PatternCenterX - (PatternWidth) / 2) > (m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left)) &&
                         ((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + PatternCenterX + (PatternWidth) / 2) < (m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIWidth - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right)) &&
                         ((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + PatternCenterY - (PatternHeight) / 2) > (m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top)) &&
                         ((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + PatternCenterY + (PatternHeight) / 2) < (m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIHeight - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom))
                         )
                    {
                        blnPatternResultOK = true;
                    }
                    else
                    {
                        blnPatternResultOK = false;
                    }
                }

            }

            if (blnResultOK && blnPatternResultOK)
            {
                for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                {
                    float fWidth = 0;
                    float fHeight = 0;
                    float fPatternAngle = m_smVisionInfo.g_objBarcode.GetMatchingAngle();
                    float fAngle = 0;
                    if (fPatternAngle > 0)
                    {
                        if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 0)
                        {
                            if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 90)
                                fAngle = 180 - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] - fPatternAngle;
                            else
                                fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                        }
                        else
                        {
                            if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] < -90)
                                fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + 180 + fPatternAngle;
                            else
                                fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                        }
                    }
                    else
                    {
                        if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 0)
                        {
                            if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 90)
                                fAngle = 180 - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] - fPatternAngle;
                            else
                                fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                        }
                        else
                        {
                            if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] < -90)
                                fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + 180 + fPatternAngle;
                            else
                                fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                        }
                    }

                    if (fAngle < 0)
                        fAngle = 180 + fAngle;



                    if (fAngle < 90)
                    {
                        fWidth = (int)Math.Round((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * Math.Cos(fAngle * Math.PI / 180)) + (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * Math.Sin(fAngle * Math.PI / 180)));
                        fHeight = (int)Math.Round((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * Math.Sin(fAngle * Math.PI / 180)) + (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * Math.Cos(fAngle * Math.PI / 180)));
                    }
                    else if (fAngle == 90)
                    {
                        fWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i];
                        fHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i];
                    }
                    else if (fAngle > 90)
                    {
                        float fWidth_1 = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i];
                        float fHeight_1 = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i];
                        float fAngle_1 = fAngle - 90;
                        fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle_1 * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle_1 * Math.PI / 180)));
                        fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle_1 * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle_1 * Math.PI / 180)));
                    }

                    int intOffsetX = (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i]);
                    int intOffsetY = (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i]);

                    //m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                    //    (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i] - (fWidth / 2)),
                    //    (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i] - (fHeight / 2)),
                    //    (int)fWidth,
                    //    (int)fHeight);

                    //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI"+i.ToString()+".bmp");

                    float CenterX = 0;
                    float CenterY = 0;
                    float fXAfterRotated = 0;
                    float fYAfterRotated = 0;

                    CenterX = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX();

                    CenterY = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY();
                    
                    //fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterX - CenterX) * Math.Cos(fPatternAngle * Math.PI / 180)) -
                    //                   ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterY - CenterY) * Math.Sin(fPatternAngle * Math.PI / 180)));

                    //fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterX - CenterX) * Math.Sin(fPatternAngle * Math.PI / 180)) +
                    //                    ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterY - CenterY) * Math.Cos(fPatternAngle * Math.PI / 180)));

                    fXAfterRotated = (float)((CenterX) + ((intOffsetX - CenterX) * Math.Cos(fPatternAngle * Math.PI / 180)) -
                                       ((intOffsetY - CenterY) * Math.Sin(fPatternAngle * Math.PI / 180)));

                    fYAfterRotated = (float)((CenterY) + ((intOffsetX - CenterX) * Math.Sin(fPatternAngle * Math.PI / 180)) +
                                        ((intOffsetY - CenterY) * Math.Cos(fPatternAngle * Math.PI / 180)));

                    fXAfterRotated = fXAfterRotated - (fWidth / 2);
                    fYAfterRotated = fYAfterRotated - (fHeight / 2);

                    m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                        (int)fXAfterRotated,
                        (int)fYAfterRotated,
                        (int)fWidth,
                        (int)fHeight);

                    //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI" + i.ToString() + ".bmp");
                    
                    int intTolerance = m_smVisionInfo.g_objBarcode.ref_intBarcodeDetectionAreaTolerance;
                    // 250
                    //if (!blnAuto)
                    //{
                    //    intTolerance = 15; 
                    //}
                    //if (((int)(fXAfterRotated - (fWidth / 2) - intTolerance) > 0) &&
                    //    ((int)(fXAfterRotated + (fWidth / 2) + intTolerance) < m_smVisionInfo.g_intCameraResolutionWidth) &&
                    //    ((int)(fYAfterRotated - (fHeight / 2) - intTolerance) > 0) &&
                    //    ((int)(fYAfterRotated + (fHeight / 2) + intTolerance) < m_smVisionInfo.g_intCameraResolutionHeight)
                    //    )
                    if (((int)(fXAfterRotated - intTolerance) > 0) &&
                   ((int)(fXAfterRotated + intTolerance) < m_smVisionInfo.g_intCameraResolutionWidth) &&
                   ((int)(fYAfterRotated - intTolerance) > 0) &&
                   ((int)(fYAfterRotated + intTolerance) < m_smVisionInfo.g_intCameraResolutionHeight)
                   )
                    {
                        //m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                        //   (int)(fXAfterRotated - (fWidth / 2) - (fWidth * 0.1)),
                        //   (int)(fYAfterRotated - (fHeight / 2) - (fHeight * 0.1)),
                        //   (int)(fWidth + (fWidth * 0.1 * 2)),
                        //   (int)(fHeight + (fHeight * 0.1 * 2)));

                        m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                        (int)(fXAfterRotated - (fWidth * 0.08)),
                        (int)(fYAfterRotated - (fHeight * 0.08)),
                        (int)(fWidth + (fWidth * 0.08 * 2)),
                        (int)(fHeight + (fHeight * 0.08 * 2)));

                        //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI" + i.ToString() + ".bmp");

                        //ImageDrawing objUniformImage = new ImageDrawing(true, m_smVisionInfo.g_arrImages[0].ref_intImageWidth, m_smVisionInfo.g_arrImages[0].ref_intImageHeight);
                        ////m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI1.bmp");
                        //m_smVisionInfo.g_arrImages[0].CopyTo(objUniformImage);
                        //string strPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_strRecipeID + "\\" +
                        //           m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\";
                        //if (m_smVisionInfo.g_objBarcode.ref_blnWantUseReferenceImage && File.Exists(strPath + "\\Template\\" + "BrightReferenceImage.bmp") && File.Exists(strPath + "\\Template\\" + "DarkReferenceImage.bmp"))
                        //{
                        //    ImageDrawing.UniformizeImage(m_smVisionInfo.g_arrImages[0], m_smVisionInfo.g_objBrightReferenceImage, m_smVisionInfo.g_objDarkReferenceImage, ref objUniformImage);
                        //    //objUniformImage.AddGain(ref objUniformImage, 2);

                        //    m_smVisionInfo.g_arrBarcodeROIs[2].AttachImage(objUniformImage);

                        //    //m_smVisionInfo.g_arrImages[0].SaveImage("D:\\img.bmp");
                        //    //objUniformImage.SaveImage("D:\\objUniformImage.bmp");
                        //    //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI2.bmp");

                        //}
                        m_smVisionInfo.g_objBarcode.SetBarcodePosition(Math.Abs(fXAfterRotated), Math.Abs(fYAfterRotated), fWidth, fHeight, i, /*m_smVisionInfo.g_objBarcode.GetMatchingAngle() -*/ m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i]);

                        int intFailType = 0;
                        if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 0)
                            intFailType = m_smVisionInfo.g_objBarcode.ReadBarcodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], false, i, false, fAngle, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage);
                        else if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 1)
                            intFailType = m_smVisionInfo.g_objBarcode.ReadQRCodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], false);
                        else
                            intFailType = m_smVisionInfo.g_objBarcode.ReadMatrixCodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], false);
                        if (intFailType == 1 || intFailType == 2 || intFailType == 3)
                        {
                            blnResultOK = false;
                            m_smVisionInfo.g_strErrorMessage += "*" + m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                        }
                        //m_smVisionInfo.g_arrBarcodeROIs[2].AttachImage(m_smVisionInfo.g_arrImages[0]);
                        //objUniformImage.Dispose();
                    }
                    else
                    {
                        m_smVisionInfo.g_objBarcode.SetBarcodePosition(Math.Abs(fXAfterRotated), Math.Abs(fYAfterRotated), fWidth, fHeight, i, /*m_smVisionInfo.g_objBarcode.GetMatchingAngle() -*/ m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i]);

                        blnBarCodeOut[i] = true;
                        //blnResultOK = true;
                        if (!blnAuto)
                        {
                            blnResultOK = false;
                            m_smVisionInfo.g_strErrorMessage += "*" + GetCodeType() + " Template " + (i + 1).ToString() + " out of image range";
                        }
                    }
                }

            }
            if (blnAuto && m_smVisionInfo.VM_PR_ByPassUnit)
            {
               
                m_smVisionInfo.VM_PR_ByPassUnit = false;
                blnResultOK = true;
                //return;
            }

            if (!blnPatternResultOK && blnAuto)
            {
                
                blnResultOK = true;
            }
            
            if (blnResultOK)
            {
                if (blnPatternResultOK && !blnBarCodeOut.Contains(true))
                    m_smVisionInfo.g_strResult = "Pass";
                else
                {
                    m_smVisionInfo.g_strResult = "Idle";
                    for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                    {
                        if (blnBarCodeOut[i])
                            m_smVisionInfo.g_objBarcode.ref_strResultCode[i] = "----";
                    }
                }

                if (blnAuto)
                {
                    if (blnPatternResultOK)
                    {
                        m_smVisionInfo.g_intPassTotal++;
                        m_smVisionInfo.g_intTestedTotal++;
                        m_intRetestCount = 0;
                        if (m_objVisionIO.IOPass1.IsOff())
                            m_objVisionIO.IOPass1.SetOn("V7 ");
                        if (m_objVisionIO.MarkFail.IsOn())
                            m_objVisionIO.MarkFail.SetOff("V7 ");
                        SavePassImage_AddToBuffer();
                    }
                    
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
                if (!blnPatternResultOK /*|| blnBarCodeOut*/)
                {
                    for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                        m_smVisionInfo.g_objBarcode.ref_strResultCode[i] = "----";
                }
                else
                {
                    for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                    {
                        if (blnBarCodeOut[i])
                            m_smVisionInfo.g_objBarcode.ref_strResultCode[i] = "----";
                    }
                }
                //if (m_smVisionInfo.g_objBarcode.ref_strErrorMessage != "")
                //{
                //    m_smVisionInfo.g_strErrorMessage += "*" + GetCodeType() + m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                //}

                if (blnAuto)
                {
                    bool blnAddFailNode = false;
                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                    m_smVisionInfo.g_intTestedTotal++;
                    if (m_objVisionIO.IOPass1.IsOn())
                        m_objVisionIO.IOPass1.SetOff("V7 ");
                    if (m_intRetestCount == (m_smVisionInfo.g_objBarcode.ref_intRetestCount) || m_smVisionInfo.g_objBarcode.ref_intRetestCount == 0)
                    {
                        if (m_smCustomizeInfo.g_blnSaveFailImage && m_smVisionInfo.g_intFailImageCount > 0)
                            blnAddFailNode = true;
                        m_intRetestCount = 0;
                        if (m_objVisionIO.MarkFail.IsOff())
                            m_objVisionIO.MarkFail.SetOn("V7 ");
                    }
                    else
                    {
                        //if (m_smVisionInfo.g_intFailImageCount > 0)
                        //    blnAddFailNode = true;
                        m_smVisionInfo.g_strResult = "Idle";
                        for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                            m_smVisionInfo.g_objBarcode.ref_strResultCode[i] = "----";
                        //m_smVisionInfo.g_objBarcode.ref_blnTestDone = false;
                        m_smVisionInfo.g_strErrorMessage = "";
                        if (m_smCustomizeInfo.g_blnSaveFailImage && m_smVisionInfo.g_intFailImageCount > 0)
                            m_intRetestCount++;
                        else if (!m_smCustomizeInfo.g_blnSaveFailImage)
                            m_intRetestCount++;
                        if (m_objVisionIO.MarkFail.IsOn())
                            m_objVisionIO.MarkFail.SetOff("V7 ");
                    }

                    SaveRejectImage_AddToBuffer(GetCodeType(), m_smVisionInfo.g_objBarcode.ref_strErrorMessage, blnAddFailNode);

                }
                else
                {

                    if (m_smVisionInfo.g_objBarcode.ref_strErrorMessage != "" && m_smVisionInfo.g_strErrorMessage == "")
                        m_smVisionInfo.g_strErrorMessage = "Offline Test Fail! *" + m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                    else
                        m_smVisionInfo.g_strErrorMessage = "Offline Test Fail! *" + m_smVisionInfo.g_strErrorMessage;
                }
            }
            
            m_smVisionInfo.g_objProcessTime.Stop();
            
            // No rotate image after inspection. Always display arrIamge.
            if (m_smVisionInfo.g_arrblnImageRotated[0])
                m_smVisionInfo.g_arrblnImageRotated[0] = false;
            
            m_smVisionInfo.VS_AT_UpdateQuantity = true;
            m_smVisionInfo.PR_VM_UpdateQuantity = true;
            m_smVisionInfo.g_blnDrawBarcodeResult = true;
            m_smVisionInfo.g_blnViewBarcodeInspection = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            //CheckLowYield();
            //CheckContinuousPass();
            //CheckContinuousFail();
            
            return;
        }
        private void StartTest_MultiThreading(bool blnAuto)
        {
            WaitEventDone_ForDisplayAllPage(ref m_smVisionInfo.VS_AT_UpdateQuantity, false);
            WaitEventDone(ref m_smVisionInfo.PR_VM_UpdateQuantity, false, false);
            WaitEventDone(ref m_smVisionInfo.g_blnDrawBarcodeResultDone, true, true);

            m_blnAuto = blnAuto;
            m_smVisionInfo.g_objBarcode.ref_strErrorMessage = "";
            m_strResultTrack = "";
            m_smVisionInfo.g_objProcessTime.Start();
            m_smVisionInfo.g_blnDrawBarcodeResultDone = false;
            m_smVisionInfo.g_blnDrawBarcodeResult = false;
            m_smVisionInfo.g_blnViewBarcodeInspection = false;
            
            bool blnResultOK = true;
            m_blnPatternResultOK = true;
            m_blnBarCodeOut = new bool[10];

            m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = false;
            m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = false;
            m_smVisionInfo.g_blnNoGrabTime = false;

            if (blnAuto)
            {
                //Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay); //29-05-2019 ZJYEOH : Moved it to After Grabtime Start counting 

                if (!m_smProductionInfo.g_blnAllRunWithoutGrabImage)
                {
                    m_bSubTh1_GrabImage = true;  // Trigger Sub Thread 1 to grab images
                }
                else
                {
                    m_smVisionInfo.g_blnGrabbing = false;
                    m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                    m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
                    m_smVisionInfo.g_blnNoGrabTime = true;
                }
            }
            else if (m_smVisionInfo.MN_PR_GrabImage)
            {
                m_smVisionInfo.MN_PR_GrabImage = false;

                m_bSubTh1_GrabImage = true;  // Trigger Sub Thread 1 to grab images
            }
            else
            {
                m_smVisionInfo.g_blnGrabbing = false;
                m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
                m_smVisionInfo.g_blnNoGrabTime = true;
            }

            // ------------------ Reset Inspection data ----------------------------------------------
            m_smVisionInfo.g_objBarcode.ResetInspectionData_Inspection();

#if (DEBUG || Debug_2_12 || RTXDebug)
            m_bSubTh_BarcodeTest_Result = false;

            if ((m_smCustomizeInfo.g_intWantBarcode & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_bSubTh_BarcodeTest = true;
            }
            else
            {
                m_bSubTh_BarcodeTest_Result = true;
            }

#else
            m_bSubTh_BarcodeTest_Result = false;

            if ((m_smCustomizeInfo.g_intWantBarcode & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_bSubTh_BarcodeTest = true;
            }
            else
            {
                m_bSubTh_BarcodeTest_Result = true;
            }

#endif
            // ------------------ Wait Sub Thread Test done --------------------------------------------------
            bool blnInspectionDone = false;
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();
            while (true)
            {
                if (!m_bSubTh_BarcodeTest)
                {
                    blnInspectionDone = true;
                    break;
                }

#if (RELEASE || RTXRelease || Release_2_12)

                if (timeout.Timing > 10000) // 2020 02 18 - CCENG: Change to 10ms to prevent inspection time out due to debug run too fast.
                {
                    string strFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                    for (int m = 0; m < m_smVisionInfo.g_arrImages.Count; m++)
                    {
                        m_smVisionInfo.g_arrImages[m].SaveImage("D:\\ErrorImage\\V7_image_" + strFileName + "_" + m.ToString() + ".bmp");
                    }
                    STTrackLog.WriteLine(">>>>>>>>>>>>> Vision 7 - time out 7.");
                    STTrackLog.WriteLine(">>>>>>>>>>>>> Vision 7 - m_bSubTh_BarcodeTest=" + m_bSubTh_BarcodeTest.ToString());

                    break;
                }

#endif
                Thread.Sleep(1);
            }
            // ------------------ Set IO to handler (set before save image for faster UPH) ----------------------------------------
            // Define Final result is pass or fail.
            if (blnInspectionDone && m_bSubTh_BarcodeTest_Result)
            {
                blnResultOK = true;
            }
            else
            {
                blnResultOK = false;
            }

            if (blnResultOK)
            {
                if (m_blnPatternResultOK && !m_blnBarCodeOut.Contains(true))
                    m_smVisionInfo.g_strResult = "Pass";
                else
                {
                    m_smVisionInfo.g_strResult = "Idle";
                    for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                    {
                        if (m_blnBarCodeOut[i])
                            m_smVisionInfo.g_objBarcode.ref_strResultCode[i] = "----";
                    }
                }

                if (blnAuto)
                {
                    if (m_blnPatternResultOK)
                    {
                        m_smVisionInfo.g_intPassTotal++;
                        m_smVisionInfo.g_intTestedTotal++;
                        m_intRetestCount = 0;
                        if (m_objVisionIO.IOPass1.IsOff())
                            m_objVisionIO.IOPass1.SetOn("V7 ");
                        if (m_objVisionIO.MarkFail.IsOn())
                            m_objVisionIO.MarkFail.SetOff("V7 ");
                        SavePassImage_AddToBuffer();
                    }

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
                if (!m_blnPatternResultOK /*|| blnBarCodeOut*/)
                {
                    for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                        m_smVisionInfo.g_objBarcode.ref_strResultCode[i] = "----";
                }
                else
                {
                    for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                    {
                        if (m_blnBarCodeOut[i])
                            m_smVisionInfo.g_objBarcode.ref_strResultCode[i] = "----";
                    }
                }
                //if (m_smVisionInfo.g_objBarcode.ref_strErrorMessage != "")
                //{
                //    m_smVisionInfo.g_strErrorMessage += "*" + GetCodeType() + m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                //}

                if (blnAuto)
                {
                    bool blnAddFailNode = false;
                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                    m_smVisionInfo.g_intTestedTotal++;
                    if (m_objVisionIO.IOPass1.IsOn())
                        m_objVisionIO.IOPass1.SetOff("V7 ");
                    if (m_intRetestCount == (m_smVisionInfo.g_objBarcode.ref_intRetestCount) || m_smVisionInfo.g_objBarcode.ref_intRetestCount == 0)
                    {
                        if (m_smCustomizeInfo.g_blnSaveFailImage && m_smVisionInfo.g_intFailImageCount > 0)
                            blnAddFailNode = true;
                        m_intRetestCount = 0;
                        if (m_objVisionIO.MarkFail.IsOff())
                            m_objVisionIO.MarkFail.SetOn("V7 ");
                    }
                    else
                    {
                        //if (m_smVisionInfo.g_intFailImageCount > 0)
                        //    blnAddFailNode = true;
                        m_smVisionInfo.g_strResult = "Idle";
                        for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                            m_smVisionInfo.g_objBarcode.ref_strResultCode[i] = "----";
                        //m_smVisionInfo.g_objBarcode.ref_blnTestDone = false;
                        m_smVisionInfo.g_strErrorMessage = "";
                        if (m_smCustomizeInfo.g_blnSaveFailImage && m_smVisionInfo.g_intFailImageCount > 0)
                            m_intRetestCount++;
                        else if (!m_smCustomizeInfo.g_blnSaveFailImage)
                            m_intRetestCount++;
                        if (m_objVisionIO.MarkFail.IsOn())
                            m_objVisionIO.MarkFail.SetOff("V7 ");
                    }

                    SaveRejectImage_AddToBuffer(GetCodeType(), m_smVisionInfo.g_objBarcode.ref_strErrorMessage, blnAddFailNode);

                }
                else
                {

                    if (m_smVisionInfo.g_objBarcode.ref_strErrorMessage != "" && m_smVisionInfo.g_strErrorMessage == "")
                        m_smVisionInfo.g_strErrorMessage = "Offline Test Fail! *" + m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                    else
                        m_smVisionInfo.g_strErrorMessage = "Offline Test Fail! *" + m_smVisionInfo.g_strErrorMessage;
                }
            }

            m_smVisionInfo.g_objProcessTime.Stop();

            // No rotate image after inspection. Always display arrIamge.
            if (m_smVisionInfo.g_arrblnImageRotated[0])
                m_smVisionInfo.g_arrblnImageRotated[0] = false;

            m_smVisionInfo.VS_AT_UpdateQuantity = true;
            m_smVisionInfo.PR_VM_UpdateQuantity = true;
            m_smVisionInfo.g_blnDrawBarcodeResult = true;
            m_smVisionInfo.g_blnViewBarcodeInspection = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            //CheckLowYield();
            //CheckContinuousPass();
            //CheckContinuousFail();

            return;
        }
        private void SaveRejectImage_AddToBuffer(string strRejectName, string strRejectMessage, bool blnAddFailNode)
        {
            if (m_smCustomizeInfo.g_blnSaveFailImage)
            {
                if (m_smCustomizeInfo.g_intSaveImageMode == 0)
                {
                    if (m_smVisionInfo.g_intFailImageCount >= m_smCustomizeInfo.g_intFailImagePics)
                        return;
                }

                //if (m_objVisionIO.Retest.IsOn())
                //{
                //    m_smVisionInfo.g_intFailImageCount--;
                //    m_intRetryCount++;
                //}
                //else
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

                if (blnAddFailNode)
                    m_smVisionInfo.g_intTotalImageCount++;
                m_arrRejectNameBuffer[m_intFailEndNode] = strRejectName;
                m_arrRejectMessageBuffer[m_intFailEndNode] = strRejectMessage;
                m_arrRetryCountBuffer[m_intFailEndNode] = m_intRetestCount;
                //m_arrFailNoBuffer[m_intFailEndNode] = m_smVisionInfo.g_intFailImageCount;
                m_arrFailNoBuffer[m_intFailEndNode] = m_smVisionInfo.g_intTotalImageCount;  // 2019 09 18 - CCENG: Use total image count instead of Fail Image count so that pass fail image will display in sequence.
                //if (blnAddFailNode)
                //    m_smVisionInfo.g_intTotalImageCount++;
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
        private void SavePassImage_AddToBuffer()
        {
            if (m_smCustomizeInfo.g_blnSavePassImage)
            {
                if (m_smVisionInfo.g_intPassImageCount < m_smCustomizeInfo.g_intPassImagePics)
                {
                    //if (m_objVisionIO.Retest.IsOn())
                    //{
                    //    m_smVisionInfo.g_intPassImageCount--;
                    //    m_intRetryCount++;
                    //}
                    //else
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
                            m_smVisionInfo.PR_CO_DeleteProcessSuccess = true; // 2020-07-22 ZJYEOH : Simply Send delete success because Barcode dont have new lot clear template function

                            m_smVisionInfo.PR_CO_DeleteTemplateDone = true;
                        }

                        //Perform manual Barcode inspection
                        if (m_smVisionInfo.MN_PR_StartTest && !m_smVisionInfo.AT_PR_AttachImagetoROI)
                        {
                            m_smVisionInfo.g_objTotalTime.Start();
                            m_smVisionInfo.MN_PR_StartTest = false;

                            StartTest_MultiThreading(false);

                            m_smVisionInfo.g_objTotalTime.Stop();
                            m_smVisionInfo.g_blnViewGauge = true;
                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                            m_smVisionInfo.PR_MN_TestDone = true;
                            m_smVisionInfo.VM_AT_BlockImageUpdate = false;
                            m_smVisionInfo.PR_MN_UpdateInfo = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }

                        //Perform production test
                        if ((m_smVisionInfo.g_intMachineStatus == 2) ||
                            (m_smVisionInfo.g_blnDebugRUN && m_smVisionInfo.g_intMachineStatus == 2))
                        {
                            
                            if (m_smVisionInfo.g_blnDebugRUN && (m_smProductionInfo.g_blnAllRunWithoutGrabImage || m_smProductionInfo.g_blnAllRunGrabWithoutUseImage) && (m_smProductionInfo.g_intDebugImageToUse == 1))
                            {
                                LoadNextImageForDebugRunTest();
                            }
                            if (m_smVisionInfo.g_strResult == "Pass")
                                Thread.Sleep(m_smVisionInfo.g_objBarcode.ref_intDelayTimeAfterPass);
                            m_smVisionInfo.g_objTotalTime.Start();
                            m_smVisionInfo.VS_AT_ProductionTestDone = false;
                            m_objVisionIO.IOEndVision.SetOn("V7 ");
                            //m_objVisionIO.IOPass1.SetOn("V7 ");
                            //2020-10-27 ZJYEOH : No need always set Pass to on
                            if (m_objVisionIO.IOPass1.IsOn())
                                m_objVisionIO.IOPass1.SetOff("V7 ");
                            if (m_objVisionIO.MarkFail.IsOn())
                                m_objVisionIO.MarkFail.SetOff("V7 ");

                            StartTest_MultiThreading(true);

                            //m_objVisionIO.IOEndVision.SetOn("V7 ");
                            if (!m_blnForceStopProduction)
                            {
                                // Do nothing.
                            }
                            else
                            {
                                STTrackLog.WriteLine("Vision7Process > Force Stop Production");
                                m_blnForceStopProduction = false;
                                m_smVisionInfo.g_intMachineStatus = 1;
                            }

                            m_smVisionInfo.g_objTotalTime.Stop();

                            m_smProductionInfo.VM_TH_UpdateCount = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;

                            m_smVisionInfo.VS_AT_ProductionTestDone = true;

                            if (m_smVisionInfo.g_blnDebugRUN)
                                Thread.Sleep(m_smVisionInfo.g_intSleep);

                            if (m_smProductionInfo.g_blnAllRunFromCenter)
                                m_smVisionInfo.g_blnDebugRUN = false;

                        }

                        if (m_objVisionIO.IOEndVision.IsOn() && m_smVisionInfo.g_intMachineStatus != 2)
                            m_objVisionIO.IOEndVision.SetOff("V7 ");
                        else if (m_objVisionIO.IOEndVision.IsOff() && m_smVisionInfo.g_intMachineStatus == 2)
                            m_objVisionIO.IOEndVision.SetOn("V7 ");

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
                SRMMessageBox.Show("Vision7Process->UpdateProgress() :" + ex.ToString());
                SRMMessageBox.Show("Vision7Process has been terminated. Please Exit SRMVision software and Run again!", "Vision7Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_smVisionInfo.g_strCameraModel == "AVT")
                    m_objAVTFireGrab.OFFCamera();
                else if (m_smVisionInfo.g_strCameraModel == "Teli")
                    m_objTeliCamera.OFFCamera();
                m_thThread = null;
                m_blnStopped = true;

            }
        }
        private void UpdateSubProgress_BarcodeTest()
        {
            while (!m_blnStopping)
            {

                try
                {
                    if (m_bSubTh_BarcodeTest)
                    {
                        m_bSubTh_BarcodeTest_Result = false;

                        //WaitEventDone(ref m_bGrabImage1Done, true);
                        bool blnResultOK = true;
                        bool blnTotalCountAdded = false;

                        // STTrackLog.WriteLine("start wait grab image result");
                        blnResultOK = WaitEventDone(ref m_bGrabImage1Done, true, ref m_bGrabImage1Result, "UpdateSubProgress_BarcodeTest > m_bGrabImage1Done 8670");
                        if (blnResultOK)
                        {
                            AttachToROI(m_smVisionInfo.g_arrBarcodeROIs, m_smVisionInfo.g_arrImages);
                        }
                        else
                            AttachImageToROI();

                        if (blnResultOK)
                        {
                            if (m_smVisionInfo.g_arrBarcodeROIs.Count == 0)
                            {
                                m_smVisionInfo.g_strErrorMessage = "*" + GetCodeType() + " : No Search ROI";
                                if (m_blnAuto)
                                {
                                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                                    m_smVisionInfo.g_intTestedTotal++;
                                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                                    blnTotalCountAdded = true;
                                }
                                blnResultOK = false;
                            }
                            else if (m_smVisionInfo.g_arrBarcodeROIs.Count == 1)
                            {
                                m_smVisionInfo.g_strErrorMessage = "*" + GetCodeType() + " : No Template Found";
                                if (m_blnAuto)
                                {
                                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                                    m_smVisionInfo.g_intTestedTotal++;
                                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                                    blnTotalCountAdded = true;
                                }
                                blnResultOK = false;
                            }
                            else if (m_smVisionInfo.g_arrBarcodeROIs.Count == 2)
                            {
                                m_smVisionInfo.g_strErrorMessage = "*" + GetCodeType() + " : No " + GetCodeType() + " ROI";
                                if (m_blnAuto)
                                {
                                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                                    m_smVisionInfo.g_intTestedTotal++;
                                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                                    blnTotalCountAdded = true;
                                }
                                blnResultOK = false;
                            }
                            else if (m_smVisionInfo.g_objBarcode.ref_intTemplateCount == 0)
                            {
                                m_smVisionInfo.g_strErrorMessage = "*" + GetCodeType() + " : No " + GetCodeType() + " Template Code Found";
                                if (m_blnAuto)
                                {
                                    //m_smVisionInfo.g_intNoTemplateFailureTotal++;
                                    //m_smVisionInfo.g_intContinuousFailUnitCount++;
                                    m_smVisionInfo.g_intTestedTotal++;
                                    m_smVisionInfo.g_intBarcodeFailureTotal++;
                                    //m_smVisionInfo.g_intLowYieldUnitCount++;
                                    blnTotalCountAdded = true;
                                }
                                blnResultOK = false;
                            }
                        }

                        if (blnResultOK)
                        {
                            //Check position
                            if (!m_smVisionInfo.g_objBarcode.MatchReferencePattern(m_smVisionInfo.g_arrBarcodeROIs[0]))
                            {
                                m_blnPatternResultOK = false;
                                blnResultOK = false;
                                //m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                            }
                            else
                            {
                                //int PatternTol = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance; // 20
                                float PatternCenterX = m_smVisionInfo.g_objBarcode.GetMatchingCenterX();
                                float PatternCenterY = m_smVisionInfo.g_objBarcode.GetMatchingCenterY();
                                int PatternWidth = m_smVisionInfo.g_objBarcode.GetMatchingTemplateWidth();
                                int PatternHeight = m_smVisionInfo.g_objBarcode.GetMatchingTemplateHeight();

                                if (((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + PatternCenterX - (PatternWidth) / 2) > (m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left)) &&
                                   ((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + PatternCenterX + (PatternWidth) / 2) < (m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIWidth - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right)) &&
                                   ((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + PatternCenterY - (PatternHeight) / 2) > (m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top)) &&
                                   ((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + PatternCenterY + (PatternHeight) / 2) < (m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIHeight - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom))
                                   )
                                {
                                    m_blnPatternResultOK = true;
                                }
                                else
                                {
                                    m_blnPatternResultOK = false;
                                }
                            }

                        }
                        if (blnResultOK && m_blnPatternResultOK)
                        {

                            for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                            {
                                float fWidth = 0;
                                float fHeight = 0;
                                float fPatternAngle = m_smVisionInfo.g_objBarcode.GetMatchingAngle();
                                float fAngle = 0;
                                if (fPatternAngle > 0)
                                {
                                    if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 0)
                                    {
                                        if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 90)
                                            fAngle = 180 - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] - fPatternAngle;
                                        else
                                            fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] < -90)
                                            fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + 180 + fPatternAngle;
                                        else
                                            fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 0)
                                    {
                                        if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 90)
                                            fAngle = 180 - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] - fPatternAngle;
                                        else
                                            fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] < -90)
                                            fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + 180 + fPatternAngle;
                                        else
                                            fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                                    }
                                }

                                if (fAngle < 0)
                                    fAngle = 180 + fAngle;

                                m_arrAngle[i] = fAngle;

                                if (fAngle < 90)
                                {
                                    fWidth = (int)Math.Round((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * Math.Cos(fAngle * Math.PI / 180)) + (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * Math.Sin(fAngle * Math.PI / 180)));
                                    fHeight = (int)Math.Round((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * Math.Sin(fAngle * Math.PI / 180)) + (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * Math.Cos(fAngle * Math.PI / 180)));
                                }
                                else if (fAngle == 90)
                                {
                                    fWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i];
                                    fHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i];
                                }
                                else if (fAngle > 90)
                                {
                                    float fWidth_1 = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i];
                                    float fHeight_1 = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i];
                                    float fAngle_1 = fAngle - 90;
                                    fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle_1 * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle_1 * Math.PI / 180)));
                                    fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle_1 * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle_1 * Math.PI / 180)));
                                }

                                int intOffsetX = (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i]);
                                int intOffsetY = (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i]);

                                //m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                                //    (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i] - (fWidth / 2)),
                                //    (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i] - (fHeight / 2)),
                                //    (int)fWidth,
                                //    (int)fHeight);

                                //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI"+i.ToString()+".bmp");

                                float CenterX = 0;
                                float CenterY = 0;
                                float fXAfterRotated = 0;
                                float fYAfterRotated = 0;

                                CenterX = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX();

                                CenterY = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY();

                                //fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterX - CenterX) * Math.Cos(fPatternAngle * Math.PI / 180)) -
                                //                   ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterY - CenterY) * Math.Sin(fPatternAngle * Math.PI / 180)));

                                //fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterX - CenterX) * Math.Sin(fPatternAngle * Math.PI / 180)) +
                                //                    ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterY - CenterY) * Math.Cos(fPatternAngle * Math.PI / 180)));

                                fXAfterRotated = (float)((CenterX) + ((intOffsetX - CenterX) * Math.Cos(fPatternAngle * Math.PI / 180)) -
                                                   ((intOffsetY - CenterY) * Math.Sin(fPatternAngle * Math.PI / 180)));

                                fYAfterRotated = (float)((CenterY) + ((intOffsetX - CenterX) * Math.Sin(fPatternAngle * Math.PI / 180)) +
                                                    ((intOffsetY - CenterY) * Math.Cos(fPatternAngle * Math.PI / 180)));

                                fXAfterRotated = fXAfterRotated - (fWidth / 2);
                                fYAfterRotated = fYAfterRotated - (fHeight / 2);

                                m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                                    (int)fXAfterRotated,
                                    (int)fYAfterRotated,
                                    (int)fWidth,
                                    (int)fHeight);

                                //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI" + i.ToString() + ".bmp");

                                int intTolerance = m_smVisionInfo.g_objBarcode.ref_intBarcodeDetectionAreaTolerance;

                                float fScale = 0.08f;

                                if (((int)(fXAfterRotated - (fWidth * fScale) - intTolerance) > 0) &&
                               ((int)(fXAfterRotated + fWidth + (fWidth * fScale * 2) + intTolerance) < m_smVisionInfo.g_intCameraResolutionWidth) &&
                               ((int)(fYAfterRotated - (fHeight * fScale) - intTolerance) > 0) &&
                               ((int)(fYAfterRotated + fHeight + (fHeight * fScale * 2) + intTolerance) < m_smVisionInfo.g_intCameraResolutionHeight)
                               )
                                {
                                    fScale = 0.2f;
                                    m_arrROIStart[i] = new Point((int)(fXAfterRotated - (fWidth * fScale)), (int)(fYAfterRotated - (fHeight * fScale)));
                                    m_arrROISize[i] = new Size((int)(fWidth + (fWidth * fScale * 2)), (int)(fHeight + (fHeight * fScale * 2)));

                                    m_smVisionInfo.g_objBarcode.SetBarcodePosition(Math.Abs(fXAfterRotated), Math.Abs(fYAfterRotated), fWidth, fHeight, i, /*m_smVisionInfo.g_objBarcode.GetMatchingAngle() -*/ m_arrAngle[i]);

                                }
                                else
                                {
                                    m_smVisionInfo.g_objBarcode.SetBarcodePosition(Math.Abs(fXAfterRotated), Math.Abs(fYAfterRotated), fWidth, fHeight, i, /*m_smVisionInfo.g_objBarcode.GetMatchingAngle() -*/ m_arrAngle[i]);

                                    m_blnBarCodeOut[i] = true;

                                    if (!m_blnAuto)
                                    {
                                        blnResultOK = false;
                                        m_smVisionInfo.g_strErrorMessage += "*" + GetCodeType() + " Template " + (i + 1).ToString() + " out of image range";
                                    }
                                }
                            }

                            if (!m_blnBarCodeOut.Contains(true))
                            {
                                for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                                {
                                    m_smVisionInfo.g_objWhiteImage.CopyTo(m_objImage_Temp);
                                    ROI objTestROI = new ROI();
                                    objTestROI.AttachImage(m_objImage_Temp);
                                    //if (m_arrAngle[i] < 90)
                                    {

                                        int intOffsetCenterX = (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i]);
                                        int intOffsetCenterY = (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i]);

                                        float CenterX = 0;
                                        float CenterY = 0;
                                        float fXAfterRotated = 0;
                                        float fYAfterRotated = 0;

                                        CenterX = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX();

                                        CenterY = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY();

                                        fXAfterRotated = (float)((CenterX) + ((intOffsetCenterX - CenterX) * Math.Cos(m_smVisionInfo.g_objBarcode.GetMatchingAngle() * Math.PI / 180)) -
                                                           ((intOffsetCenterY - CenterY) * Math.Sin(m_smVisionInfo.g_objBarcode.GetMatchingAngle() * Math.PI / 180)));

                                        fYAfterRotated = (float)((CenterY) + ((intOffsetCenterX - CenterX) * Math.Sin(m_smVisionInfo.g_objBarcode.GetMatchingAngle() * Math.PI / 180)) +
                                                            ((intOffsetCenterY - CenterY) * Math.Cos(m_smVisionInfo.g_objBarcode.GetMatchingAngle() * Math.PI / 180)));

                                        float fBarcodeWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * 2f;//* 1.5f
                                        float fBarcodeHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * 2f;//* 1.5f

                                        //if (m_arrAngle[i] < 90)
                                        //{
                                        //    fBarcodeWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * 1.5f;
                                        //    fBarcodeHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * 1.5f;
                                        //}

                                        //if (m_arrAngle[i] < 90)
                                        //{
                                        //    fBarcodeWidth = m_arrROISize[i].Width;
                                        //}
                                        //else if (m_arrAngle[i] == 90)
                                        //{
                                        //    fBarcodeWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * 1.5f;
                                        //    fBarcodeHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * 1.5f;
                                        //}
                                        //else if (m_arrAngle[i] > 90)
                                        //{
                                        //    fBarcodeWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * 1.5f;
                                        //    fBarcodeHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * 1.5f;
                                        //}

                                        List<Point> arrPoints = new List<Point>();
                                        arrPoints.Add(new Point((int)(fXAfterRotated - (fBarcodeWidth / 2)), (int)(fYAfterRotated - (fBarcodeHeight / 2))));
                                        arrPoints.Add(new Point((int)(fXAfterRotated + (fBarcodeWidth / 2)), (int)(fYAfterRotated - (fBarcodeHeight / 2))));
                                        arrPoints.Add(new Point((int)(fXAfterRotated + (fBarcodeWidth / 2)), (int)(fYAfterRotated + (fBarcodeHeight / 2))));
                                        arrPoints.Add(new Point((int)(fXAfterRotated - (fBarcodeWidth / 2)), (int)(fYAfterRotated + (fBarcodeHeight / 2))));

                                        DontCareWithoutRotateImage.ProduceImage_ForBarcode(arrPoints, m_objImage_Temp, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage, m_arrAngle[i], false, m_smVisionInfo.g_objBarcode.ref_fDontCareScale);
                                        //DontCareWithoutRotateImage.ProduceImage_WithBiggerScale(arrPoints, m_objImage_Temp, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage, m_arrAngle[i], false, (int)(m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * 0.5f), (int)(m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * 0.5f));
                                        //m_objImage_Temp.SaveImage("D:\\m_objImage_Temp" + i.ToString() + ".bmp");
                                        objTestROI.LoadROISetting(
                                    m_arrROIStart[i].X,
                                    m_arrROIStart[i].Y,
                                    m_arrROISize[i].Width,
                                    m_arrROISize[i].Height);
                                        //objTestROI.SaveImage("D:\\objTestROIBefore" + i.ToString() + ".bmp");
                                    }
                                    //else if (m_arrAngle[i] == 90)
                                    //{
                                    //    fWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i];
                                    //    fHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i];
                                    //}
                                    //else if (m_arrAngle[i] > 90)
                                    //{
                                    //    float fWidth_1 = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i];
                                    //    float fHeight_1 = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i];
                                    //    float fAngle_1 = m_arrAngle[i] - 90;
                                    //    fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle_1 * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle_1 * Math.PI / 180)));
                                    //    fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle_1 * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle_1 * Math.PI / 180)));
                                    //}

                                    m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                                    m_arrROIStart[i].X,
                                    m_arrROIStart[i].Y,
                                    m_arrROISize[i].Width,
                                    m_arrROISize[i].Height);

                                    ROI.LogicOperationAddROI2(m_smVisionInfo.g_arrBarcodeROIs[2], objTestROI);//SubtractROI2

                                    //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI" + i.ToString() + ".bmp");
                                    //objTestROI.SaveImage("D:\\objTestROI" + i.ToString() + ".bmp");

                                    int intFailType = 0;
                                    if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 0)
                                        intFailType = m_smVisionInfo.g_objBarcode.ReadBarcodeObjects(objTestROI, false, i, false, m_arrAngle[i], m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage);
                                    else if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 1)
                                        intFailType = m_smVisionInfo.g_objBarcode.ReadQRCodeObjects(objTestROI, false);
                                    else
                                        intFailType = m_smVisionInfo.g_objBarcode.ReadMatrixCodeObjects(objTestROI, false);
                                    if (intFailType == 1 || intFailType == 2 || intFailType == 3)
                                    {
                                        blnResultOK = false;
                                        m_smVisionInfo.g_strErrorMessage += "*" + m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                                    }
                                    objTestROI.Dispose();
                                }
                            }
                            

                        }

                        //2021-12-06 ZJYEOH: Check Barcode Orientation Angle here
                        if (blnResultOK && m_blnPatternResultOK)
                        {
                            if (!m_blnBarCodeOut.Contains(true))
                            {
                                for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                                {
                                    if (Math.Abs(m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i] - m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                    {
                                        blnResultOK = false;
                                    }
                                }

                                if (!blnResultOK)
                                {
                                    m_smVisionInfo.g_strErrorMessage += "*Barcode Orientation Fail.";
                                }
                            }
                        }

                        if (m_blnAuto && m_smVisionInfo.VM_PR_ByPassUnit)
                        {
                            m_smVisionInfo.VM_PR_ByPassUnit = false;
                            blnResultOK = true;
                            //return;
                        }

                        if (!m_blnPatternResultOK && m_blnAuto)
                        {
                            blnResultOK = true;
                        }
                        m_bSubTh_BarcodeTest_Result = blnResultOK;

                        m_bSubTh_BarcodeTest = false;
                    }
                }
                catch (Exception ex)
                {
                    m_bSubTh_BarcodeTest = false;

                    SRMMessageBox.Show("Vision7Process->UpdateSubProgress_BarcodeTest() :" + ex.ToString());
                }
                Thread.Sleep(1);
            }

            m_thSubThread_BarcodeTest = null;
            m_blnStopped_BarcodeTest = true;
        }
        private void UpdateSubProgress_GrabImage()
        {
            while (!m_blnStopping)
            {

                try
                {
                    if (m_bSubTh1_GrabImage)
                    {
                        m_bSubTh1_GrabImage = false;
                        GrabImage(m_blnAuto);
                    }

                }
                catch (Exception ex)
                {
                    m_bSubTh1_GrabImage = false;

                    SRMMessageBox.Show("Vision1Process->UpdateSubProgress_GrabImage() :" + ex.ToString());
                }
                Thread.Sleep(1);
            }

            m_thSubThread_GrabImage = null;
            m_blnStopped_GrabImage = true;
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
                    SRMMessageBox.Show("Vision7Process->UpdateSubProgress_GrabImage() :" + ex.ToString());
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
                    //if (m_intRetestCount == (m_smVisionInfo.g_objBarcode.ref_intRetestCount - 1) || m_smVisionInfo.g_objBarcode.ref_intRetestCount == 0)
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
                    STTrackLog.WriteLine("Vision7Process All threads have stopped.");
                    break;
                }

                if (timesout.Timing > 3000)
                {
                    STTrackLog.WriteLine("Vision7Process : m_blnStopped = " + m_blnStopped.ToString());
                    STTrackLog.WriteLine("Vision7Process : m_blnStopped_SaveImage = " + m_blnStopped_SaveImage.ToString());
                    STTrackLog.WriteLine("Vision7Process : >>>>>>>>>>>>> time out 3");
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

                //if (intGrabRequire > 0)
                //    m_arrInspectionImage1Buffer = new ImageDrawing[BUFFERSIZE];
                //if (intGrabRequire > 1)
                //    m_arrInspectionImage2Buffer = new ImageDrawing[BUFFERSIZE];
                //if (intGrabRequire > 2)
                //    m_arrInspectionImage3Buffer = new ImageDrawing[BUFFERSIZE];
                //if (intGrabRequire > 3)
                //    m_arrInspectionImage4Buffer = new ImageDrawing[BUFFERSIZE];
                //if (intGrabRequire > 4)
                //    m_arrInspectionImage5Buffer = new ImageDrawing[BUFFERSIZE];
                //if (intGrabRequire > 5)
                //    m_arrInspectionImage6Buffer = new ImageDrawing[BUFFERSIZE];
                //if (intGrabRequire > 6)
                //    m_arrInspectionImage7Buffer = new ImageDrawing[BUFFERSIZE];

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

                    //if (intGrabRequire > 0)
                    //    m_arrInspectionImage1Buffer[i] = new ImageDrawing(true);
                    //if (intGrabRequire > 1)
                    //    m_arrInspectionImage2Buffer[i] = new ImageDrawing(true);
                    //if (intGrabRequire > 2)
                    //    m_arrInspectionImage3Buffer[i] = new ImageDrawing(true);
                    //if (intGrabRequire > 3)
                    //    m_arrInspectionImage4Buffer[i] = new ImageDrawing(true);
                    //if (intGrabRequire > 4)
                    //    m_arrInspectionImage5Buffer[i] = new ImageDrawing(true);
                    //if (intGrabRequire > 5)
                    //    m_arrInspectionImage6Buffer[i] = new ImageDrawing(true);
                    //if (intGrabRequire > 6)
                    //    m_arrInspectionImage7Buffer[i] = new ImageDrawing(true);
                }
            }
            m_arrPassNoBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrFailNoBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrRetryCountBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrRejectNameBuffer = new string[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrRejectMessageBuffer = new string[m_smVisionInfo.g_intSaveImageBufferSize];
        }
    }
}
